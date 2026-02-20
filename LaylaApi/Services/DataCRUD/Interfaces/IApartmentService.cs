using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;
namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IApartmentService
    {
        Task<PagedResult<ApartmentDto>> GetAllAsync(string userIp, CancellationToken ct);
        Task<ApartmentDto> GetByIdAsync(int id, CancellationToken ct);
        Task<Apartment> GetEntityByIdAsync(int id);
        Task<IEnumerable<ApartmentDto>> GetByOwnerIdAsync(int id, CancellationToken ct);
        Task<IEnumerable<ApartmentDto>> SearchAsync(string keyword, CancellationToken ct);
        Task<ApartmentDto> AddAsync(CreateApartmentDto dto, int ownerId, CancellationToken ct);
        Task<ApartmentDto?> UpdateAsync(int id, CreateApartmentDto dto, int ownerId, bool isAdmin, CancellationToken ct);
        Task<IEnumerable<ApartmentDto>> GetNearbyAsync(double userLat, double userLng, double maxDistanceKm, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, int ownerId, CancellationToken ct);
        Task<PagedResult<ApartmentDto>>GetTopBookedApartmentsAsync(CancellationToken ct = default);
        Task<PagedResult<ApartmentDto>> GetTopRatedApartmentsAsync(CancellationToken ct = default);
    }
}
