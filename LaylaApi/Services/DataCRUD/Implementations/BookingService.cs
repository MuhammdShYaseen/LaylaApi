using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Exceptions;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
        public BookingService(LaylaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> GetAllAsync()
        {

            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Apartment)
                .ToListAsync();
            
            return _mapper.Map<IEnumerable<BookingDto>>(booking);
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Apartment)
                .FirstOrDefaultAsync(b => b.Id == id);
            return _mapper.Map<BookingDto>(booking);
        }
        public async Task<IEnumerable<BookingDto>> GetBookingsForOwnerAsync(int ownerId)
        {
            var bookings = await _context.Bookings
                          .AsNoTracking()
                          .Include(b => b.Apartment)
                          .Include(b => b.User)
                          .Where(b => b.Apartment != null && b.Apartment.OwnerId == ownerId)
                          .ToListAsync();

            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        public async Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId)
        {
            var booking = await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Apartment)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(booking);
        }

        public async Task<IEnumerable<BookingDto>> GetByApartmentIdAsync(int apartmentId)
        {
            var booking = await _context.Bookings
                .Where(b => b.ApartmentId == apartmentId)
                .Include(b => b.User)
                .Include(b=> b.Apartment)
                .ToListAsync();
            return _mapper.Map<IEnumerable<BookingDto>>(booking);
        }

        public async Task<BookingDto> AddAsync(CreateBookingDto dto, int UserID)
        {
            var booking = _mapper.Map<Booking>(dto);
            booking.UserId = UserID;

            var apartment = await _context.Apartments
                .Include(u => u.Owner)
                .FirstOrDefaultAsync(a => a.Id == booking.ApartmentId);

            var renter = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserID);

            if (apartment == null)
                throw new BadHttpRequestException("Apartment does not exist.");

            if (renter == null)
                throw new BadHttpRequestException("renter does not exist.");

            if (apartment.OwnerId == booking.UserId)
                throw new BadHttpRequestException("Cannot Book Own Apartment.");

            if (booking.StartDate >= booking.EndDate)
                throw new BadHttpRequestException("Start date must be earlier than end date.");

            

            bool available = await IsDateAvailableAsync(booking.ApartmentId, booking.StartDate, booking.EndDate);

            if (!available)
                throw new BadHttpRequestException("The selected dates overlap with another booking.");

            booking = Booking.Create(apartment, renter, dto.StartDate, dto.EndDate);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto?> UpdateAsync(int id, CreateBookingDto dto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            if (!await IsDateAvailableAsync(booking.ApartmentId, dto.StartDate, dto.EndDate))
                throw new BusinessException("BookingTimeOverlap", 400);

            booking.StartDate = dto.StartDate;
            booking.EndDate = dto.EndDate;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Bookings.FindAsync(id);
            if (existing == null) return false;

            _context.Bookings.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsDateAvailableAsync(int apartmentId, DateTime startDate, DateTime endDate)
        {
            //return !await _context.Bookings.AnyAsync(b => b.ApartmentId == apartmentId && b.Status != "Cancelled" && ((startDate >= b.StartDate && startDate < b.EndDate) ||(endDate > b.StartDate && endDate <= b.EndDate)));
            //return !await _context.Bookings.AnyAsync(b => b.ApartmentId == apartmentId && b.Status == BookingStatus.Confirmed && b.StartDate < endDate && b.EndDate > startDate);
            var forbiddenStatuses = new[]
            {
                BookingStatus.Accepted,
                BookingStatus.Confirmed
            };
            return !await _context.Bookings.AnyAsync(b => b.ApartmentId == apartmentId && forbiddenStatuses.Contains(b.Status) && b.StartDate < endDate && b.EndDate > startDate);
        }

        // 🔄 تحديث حالة الحجز (Confirm / Cancel / Complete)
        public async Task<BookingDto?> UpdateStatusAsync(int bookingId, BookingStatus newStatus, int actorUserId, bool isAdmin)
        {

            var booking = await _context.Bookings
                .Include(b => b.Apartment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return null;

            // Authorization (Business Rule)
            var isOwner = booking.Apartment!.OwnerId == actorUserId;

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException();

            // Business Rule: valid status transition
            if (!IsValidStatusTransition(booking.Status, newStatus))
                throw new InvalidOperationException("Invalid status transition");

            booking.ChangeStatus(newStatus);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }
        public async Task<bool> CancelAsync(int id, int userId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            if (booking.UserId != userId)
                throw new UnauthorizedAccessException("You cannot cancel this booking.");

            booking.Status = BookingStatus.CancelledByRenter;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CancelByOwnerAsync(int bookingId, int ownerId, string? reason = null)
        {
            var booking = await _context.Bookings
                .Include(b => b.Apartment)
                .Where(b => b.Apartment != null && b.Apartment.OwnerId == ownerId)   // ← حماية قوية
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return false;

            if (booking.Status == BookingStatus.Completed)
                return false;

            booking.Status = BookingStatus.CancelledByOwner;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Booking?> GetEntityByIdAsync(int id)
        {
            var booking = await _context.Bookings
                                 .Include(b => b.Apartment)
                                 .Include(b => b.User)
                                 .FirstOrDefaultAsync(b => b.Id == id);
            if (booking == null) return null;
            return booking;
        }

        public async Task<IEnumerable<CalendarEventDto>> GetCalendarAsync(int apartmentId)
        {
            var bookingsDto = await GetByApartmentIdAsync(apartmentId);

            var calendarEvents = bookingsDto
                .Where(b => b.Status != BookingStatus.CancelledByRenter
                         && b.Status != BookingStatus.CancelledByOwner)
                .Select(b => new CalendarEventDto
                {
                    Id = b.Id,
                    Title = "Booked",
                    Start = b.StartDate,
                    End = b.EndDate,
                    Status = b.Status,
                    Color = GetStatusColor(b.Status)
                }).ToList();
            return calendarEvents;
        }

        private static string GetStatusColor(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "#FACC15",          // أصفر
                BookingStatus.Accepted => "#3B82F6",         // أزرق
                BookingStatus.Confirmed => "#16A34A",        // أخضر
                BookingStatus.Completed => "#10B981",        // أخضر فاتح
                BookingStatus.CancelledByRenter => "#EF4444",// أحمر
                BookingStatus.CancelledByOwner => "#DC2626", // أحمر داكن
                _ => "#6B7280"                               // رمادي
            };
        }

        private static bool IsValidStatusTransition(BookingStatus current, BookingStatus next)
        {
            return current switch
            {
                BookingStatus.Pending =>
                    next is BookingStatus.Accepted
                        or BookingStatus.CancelledByOwner
                        or BookingStatus.CancelledByRenter,

                BookingStatus.Accepted =>
                    next is BookingStatus.Confirmed
                        or BookingStatus.CancelledByOwner
                        or BookingStatus.CancelledByRenter,

                BookingStatus.Confirmed =>
                    next is BookingStatus.Completed
                        or BookingStatus.CancelledByOwner,

                _ => false
            };
        }
    }
}
