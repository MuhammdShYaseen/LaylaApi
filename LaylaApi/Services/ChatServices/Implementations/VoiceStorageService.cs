using LaylaApi.Services.ChatServices.Interfaces;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class VoiceStorageService : IVoiceStorageService
    {
        private readonly string _basePath;
        public VoiceStorageService()
        {

            _basePath = Path.Combine(Directory.GetCurrentDirectory(), "storage/chat/voice");
            Directory.CreateDirectory(_basePath);
        }
        public Task DeleteAsync(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            return Task.CompletedTask;
        }

        public async Task<string> SaveAsync(IFormFile file, int messageId)
        {

            var path = Path.Combine(_basePath, $"msg_{messageId}.webm");
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return path;
        }
    }
}
