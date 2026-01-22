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

        public async Task<IEnumerable<Review>> GetAllAsync() =>
            await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).ToListAsync();

        public async Task<Review?> GetByIdAsync(int id) =>
            await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).FirstOrDefaultAsync(r => r.Id == id);
        public async Task<IEnumerable<Review?>> GetByUserIdAsync(int id) =>
            await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).Where(r => r.UserId == id).ToListAsync();
        public async Task<IEnumerable<Review>> GetByApartmentIdAsync(int apartmentId) =>
            await _context.Reviews.Where(r => r.ApartmentId == apartmentId).Include(r => r.User).ToListAsync();

        public async Task<bool> ExistsAsync(int userId, int apartmentId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .AnyAsync(r => r.UserId == userId && r.ApartmentId == apartmentId);
        }
        public async Task<ReviewDto> AddAsync(ReviewCreateDto dto, int userId, bool isAdmin)
        {
            if (userId == 0)
                throw new UnauthorizedAccessException();

            // تحقق من وجود حجز مكتمل
            var hasCompletedBooking = await _context.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.ApartmentId == dto.ApartmentId &&
                b.Status == BookingStatus.Completed);

            if (!hasCompletedBooking)
                throw new InvalidOperationException("You can only review after a completed booking.");

            // منع التكرار
            var exists = await _context.Reviews.AnyAsync(r =>
                r.UserId == userId &&
                r.ApartmentId == dto.ApartmentId);

            if (exists)
                throw new InvalidOperationException("You already reviewed this apartment.");

            var review = Review.Create(userId, dto.ApartmentId, dto.Rating, dto.Comment ?? "");

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review); 
        }

        public async Task<ReviewDto> UpdateAsync(int id, ReviewCreateDto dto, int userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id)
                 ?? throw new KeyNotFoundException();

            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            review.Update(dto.Rating, dto.Comment ?? "");
            await _context.SaveChangesAsync();

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return false;

            _context.Reviews.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
