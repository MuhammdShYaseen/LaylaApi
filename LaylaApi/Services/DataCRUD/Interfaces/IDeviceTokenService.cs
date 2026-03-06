using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.DataCRUD.Interfaces
{
    public interface IDeviceTokenService
    {
        Task<IEnumerable<DeviceTokenDto>> GetByUserIdAsync(int userId, CancellationToken ct);
        Task<DeviceTokenDto> UpsertAsync(DeviceTokenUpsertDto dto, int currentUserId, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);
        Task CleanupInactiveAsync(TimeSpan maxAge);
    }
}
