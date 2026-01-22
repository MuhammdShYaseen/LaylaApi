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

        public async Task<IEnumerable<ReviewDto>> GetAllAsync()
        {
            var reviews = await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).ToListAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
           

        public async Task<ReviewDto> GetByIdAsync(int id)
        {
            var review = await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).FirstOrDefaultAsync(r => r.Id == id);
            return _mapper.Map<ReviewDto>(review);
        }
           
        public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int id)
        {
            var reviews = await _context.Reviews.Include(r => r.User).Include(r => r.Apartment).Where(r => r.UserId == id).ToListAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
            
        public async Task<IEnumerable<ReviewDto>> GetByApartmentIdAsync(int apartmentId)
        {
            var reviews = await _context.Reviews.Where(r => r.ApartmentId == apartmentId).Include(r => r.User).ToListAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
            

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

        public async Task DeleteAsync(int id, int userId, bool isAdmin)
        {
            var review = await _context.Reviews.FindAsync(id)
        ?? throw new KeyNotFoundException();

            if (review.UserId != userId && !isAdmin)
                throw new UnauthorizedAccessException();

            _context.Remove(review); // أو Remove حسب قرارك
            await _context.SaveChangesAsync();
        }

        public async Task<object> GetAverageRatingAsync(int apartmentId)
        {
            var query = _context.Reviews.Where(r => r.ApartmentId == apartmentId);

            var count = await query.CountAsync();
            if (count == 0)
                return new { average = 0.0, count = 0 };

            var avg = await query.AverageAsync(r => r.Rating);

            return new
            {
                average = Math.Round(avg, 2),
                count
            };
        }
    }
}
