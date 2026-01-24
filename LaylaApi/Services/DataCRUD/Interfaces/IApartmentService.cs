using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IApartmentService
    {
        Task<IEnumerable<ApartmentDto>> GetAllAsync();
        Task<ApartmentDto> GetByIdAsync(int id);
        Task<Apartment> GetEntityByIdAsync(int id);
        Task<IEnumerable<ApartmentDto>> GetByOwnerIdAsync(int id);
        Task<IEnumerable<ApartmentDto>> SearchAsync(string keyword);
        Task<ApartmentDto> AddAsync(CreateApartmentDto dto, int ownerId);
        Task<ApartmentDto?> UpdateAsync(int id, CreateApartmentDto dto, int ownerId, bool isAdmin);
        Task<IEnumerable<ApartmentDto>> GetNearbyAsync(double userLat, double userLng, double maxDistanceKm);
        Task<bool> DeleteAsync(int id, int ownerId);
    }
}
