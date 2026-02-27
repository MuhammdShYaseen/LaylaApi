using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllAsync(CancellationToken ct);
        Task<ReviewDto> GetByIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<ReviewDto>> GetByUserIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<ReviewDto>> GetByApartmentIdAsync(int apartmentId, CancellationToken ct);
        Task<object> GetAverageRatingAsync(int apartmentId, CancellationToken ct);
        Task<bool> ExistsAsync(int userId, int ApartmentId, CancellationToken ct);
        Task<ReviewDto> AddAsync(ReviewCreateDto dto, int userId, bool isAdmin, CancellationToken ct);
        Task<ReviewDto> UpdateAsync(int id, ReviewCreateDto dto, int userId, bool isAdmin, CancellationToken ct);

        Task DeleteAsync(int id, int userId, bool isAdmin, CancellationToken ct);
    }
}
