using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review?>> GetByUserIdAsync(int id);
        Task<IEnumerable<Review>> GetByApartmentIdAsync(int apartmentId);
        Task<object> GetAverageRatingAsync(int apartmentId);
        Task<bool> ExistsAsync(int userId,int ApartmentId);
        Task<ReviewDto> AddAsync(ReviewCreateDto dto, int userId, bool isAdmin);
        Task<ReviewDto> UpdateAsync(int id, ReviewCreateDto dto, int userId, bool isAdmin);

        Task DeleteAsync(int id, int userId, bool isAdmin);
    }
}
