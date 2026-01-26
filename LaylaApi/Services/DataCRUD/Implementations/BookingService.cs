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

        public async Task<BookingDto> AddAsync(CreateBookingDto dto, int userId)
        {
            if (dto.StartDate >= dto.EndDate)
                throw new BadHttpRequestException("Start date must be earlier than end date.");

            var apartment = await _context.Apartments
                .AsNoTracking()
                .Where(a => a.Id == dto.ApartmentId)
                .Select(a => new { a.Id, a.OwnerId })
                .SingleOrDefaultAsync()
                ?? throw new BadHttpRequestException("Apartment does not exist.");

            if (apartment.OwnerId == userId)
                throw new BadHttpRequestException("Cannot book your own apartment.");

            var renterExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == userId);

            if (!renterExists)
                throw new BadHttpRequestException("Renter does not exist.");

            var isAvailable = await IsDateAvailableAsync(dto.ApartmentId, dto.StartDate, dto.EndDate);

            if (!isAvailable)
                throw new BadHttpRequestException("Selected dates overlap with another booking.");

            var booking = Booking.Create(dto.ApartmentId, userId, dto.StartDate, dto.EndDate);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto?> UpdateAsync(int bookingId, CreateBookingDto dto, int renterId, bool isAdmin)
        {
            var booking = await _context.Bookings
                 .Include(b => b.Apartment)
                 .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return null;

            // Authorization rule
            if (booking.UserId != renterId && isAdmin == false)
                throw new UnauthorizedAccessException();

            if (dto.StartDate >= dto.EndDate)
                throw new BadHttpRequestException("Invalid date range.");

            if (!await IsDateAvailableAsync( booking.ApartmentId, dto.StartDate, dto.EndDate))
                throw new BadHttpRequestException("Booking time overlap.");

            // ✅ التحديث يتم عبر الكيان
            booking.Updated(dto);

            _context.Bookings.Update(booking);

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

            booking.ChangeStatus(BookingStatus.CancelledByRenter);

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

            booking.ChangeStatus(BookingStatus.CancelledByOwner);

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

        
    }
}
