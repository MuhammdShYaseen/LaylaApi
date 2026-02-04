using LaylaApi.Models.DtosModels.ExternalMediaStorageDtos;

namespace LaylaApi.Services.MediaStorageProviderServices.Interfaces
{
    public interface IStorageProvider
    {
        Task<UploadSignatureDto> CreateUploadSignatureAsync(int userId, int apartmentId);
        Task HandleWebhookAsync(WebhookDto data);
        Task <bool> DeleteAsync(int mediaId);
    }
}
