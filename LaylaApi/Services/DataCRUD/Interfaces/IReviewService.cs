using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review?>> GetByUserIdAsync(int id);
        Task<IEnumerable<Review>> GetByApartmentIdAsync(int apartmentId);
        Task<bool> ExistsAsync(int userId,int ApartmentId);
        Task<Review> AddAsync(Review review);
        Task<Review?> UpdateAsync(int id, Review review);
        Task<bool> DeleteAsync(int id);
    }
}
