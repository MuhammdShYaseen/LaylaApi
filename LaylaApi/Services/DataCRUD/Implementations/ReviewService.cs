using AutoMapper;
using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly LaylaContext _context;
        private readonly IMapper _mapper;
        public ReviewService(LaylaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken ct)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Apartment)
                .ToListAsync(ct);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
           

        public async Task<ReviewDto> GetByIdAsync(int id, CancellationToken ct)
        {
            var review = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Apartment)
                .FirstOrDefaultAsync(r => r.Id == id, ct);
            return _mapper.Map<ReviewDto>(review);
        }
           
        public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int id, CancellationToken ct)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Apartment)
                .Where(r => r.UserId == id)
                .ToListAsync(ct);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
            
        public async Task<IEnumerable<ReviewDto>> GetByApartmentIdAsync(int apartmentId, CancellationToken ct)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Where(r => r.ApartmentId == apartmentId)
                .Include(r => r.User)
                .ToListAsync(ct);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
            

        public async Task<bool> ExistsAsync(int userId, int apartmentId, CancellationToken ct)
        {
            return await _context.Reviews
                .AsNoTracking()
                .AnyAsync(r => r.UserId == userId && r.ApartmentId == apartmentId, ct);
        }
        public async Task<ReviewDto> AddAsync(ReviewCreateDto dto, int userId, bool isAdmin, CancellationToken ct)
        {
            if (userId == 0)
                throw new UnauthorizedAccessException();

            // تحقق من وجود حجز مكتمل
            var hasCompletedBooking = await _context.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.ApartmentId == dto.ApartmentId &&
                b.Status == BookingStatus.Completed, ct);

            if (!hasCompletedBooking)
                throw new InvalidOperationException("You can only review after a completed booking.");

            // منع التكرار
            var exists = await _context.Reviews.AnyAsync(r =>
                r.UserId == userId &&
                r.ApartmentId == dto.ApartmentId, ct);

            if (exists)
                throw new InvalidOperationException("You already reviewed this apartment.");

            var review = Review.Create(userId, dto.ApartmentId, dto.Rating, dto.Comment ?? "");

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review); 
        }

        public async Task<ReviewDto> UpdateAsync(int id, ReviewCreateDto dto, int userId, bool isAdmin, CancellationToken ct)
        {
            var review = await _context.Reviews.FindAsync(id, ct)
                 ?? throw new KeyNotFoundException();

            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            review.Update(dto.Rating, dto.Comment ?? "");
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct)
        {
            var review = await _context.Reviews.FindAsync(id, ct)
        ?? throw new KeyNotFoundException();

            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            _context.Remove(review); // أو Remove حسب قرارك
            await _context.SaveChangesAsync();
        }

        public async Task<object> GetAverageRatingAsync(int apartmentId, CancellationToken ct)
        {
            var query = _context.Reviews.Where(r => r.ApartmentId == apartmentId);

            var count = await query.CountAsync(ct);
            if (count == 0)
                return new { average = 0.0, count = 0 };

            var avg = await query.AverageAsync(r => r.Rating, ct);

            return new
            {
                average = Math.Round(avg, 2),
                count
            };
        }
    }
}
