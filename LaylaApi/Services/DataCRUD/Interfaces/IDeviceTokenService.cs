using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<IEnumerable<DeviceToken>> GetByUserIdAsync(int userId);
        Task<DeviceToken> UpsertAsync(DeviceTokenUpsertDto dto, int currentUserId, CancellationToken ct);
        Task<bool> DeleteAsync(int id);
        Task CleanupInactiveAsync(TimeSpan maxAge);
    }
}
