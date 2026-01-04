namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IVoiceStorageService
    {
        Task<string> SaveAsync(IFormFile file, int messageId);
        Task DeleteAsync(string filePath);
    }
}
