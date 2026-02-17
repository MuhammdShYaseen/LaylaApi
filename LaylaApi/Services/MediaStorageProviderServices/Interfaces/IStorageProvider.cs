using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using static LaylaApi.Services.MediaStorageProviderServices.Implementation.CloudinaryStorageProvider;

namespace LaylaApi.Services.MediaStorageProviderServices.Interfaces
{
    public interface IStorageProvider
    {
        Task<UploadSignatureDto> CreateUploadSignatureAsync(int userId, int apartmentId, bool isAdmin, CancellationToken ct);
        Task <bool> DeleteAsync(int mediaId, int CurrentUserId, bool isAdmin, CancellationToken ct);
        Task<WebhookResult> ProcessWebhookAsync(HttpRequest request, CancellationToken ct);
    }
}
