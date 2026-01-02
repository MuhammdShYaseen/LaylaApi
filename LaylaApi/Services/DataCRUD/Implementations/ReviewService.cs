using LaylaApi.DataAccess;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.DataCRUD.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.DataCRUD.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly LaylaContext _context;

        public ReviewService(LaylaContext context)
        {
            _context = context;
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
        public async Task<Review> AddAsync(Review review)
        {
            var user = await _context.Users.FirstOrDefaultAsync (u => u.Id == review.UserId);
            if (user == null) 
                throw new KeyNotFoundException("user not found");

            var apartment = await _context.Apartments.Include(a => a.Owner).FirstOrDefaultAsync(a => a.Id == review.ApartmentId);
            if (apartment == null)
                throw new KeyNotFoundException("apartment not found");

            review = Review.Create(review, apartment, user);
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> UpdateAsync(int id, Review review)
        {
            var existing = await _context.Reviews.FindAsync(id);
            if (existing == null) return null;

            existing.Rating = review.Rating;
            existing.Comment = review.Comment;
            await _context.SaveChangesAsync();

            return existing;
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
