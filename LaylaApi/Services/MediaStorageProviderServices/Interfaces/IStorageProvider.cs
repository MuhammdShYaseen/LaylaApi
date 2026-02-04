using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;
using static LaylaApi.Services.MediaStorageProviderServices.Implementation.CloudinaryStorageProvider;

namespace LaylaApi.Services.MediaStorageProviderServices.Interfaces
{
    public interface IStorageProvider
    {
        Task<UploadSignatureDto> CreateUploadSignatureAsync(int userId, int apartmentId, bool isAdmin);
        Task <bool> DeleteAsync(int mediaId, int CurrentUserId, bool isAdmin);
        Task<WebhookResult> ProcessWebhookAsync(HttpRequest request);
    }
}
