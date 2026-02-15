using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<IEnumerable<DeviceToken>> GetByUserIdAsync(int userId);
        Task<DeviceToken> UpsertAsync(DeviceTokenUpsertDto dto, int currentUserId);
        Task<bool> DeleteAsync(int id, bool isAdmin);
        Task CleanupInactiveAsync(TimeSpan maxAge);
    }
}
