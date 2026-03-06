using LaylaApi.Services.ChatServices.Interfaces;

namespace LaylaApi.Services.ChatServices.Implementations
{
    public class VoiceStorageService : IVoiceStorageService
    {
        private readonly string _basePath;
        public VoiceStorageService(IConfiguration config)
        {
            var storageRoot = config.GetValue<string>("VoiceStoragePath") ?? "storage/chat/voice";

            _basePath = Path.Combine(AppContext.BaseDirectory, storageRoot);
            Directory.CreateDirectory(_basePath);
        }
        public Task DeleteAsync(string filePath)
        {
            if (File.Exists(filePath)) File.Delete(filePath);
            return Task.CompletedTask;
        }

        public async Task<string> SaveAsync(IFormFile file, int messageId)
        {
            if (file.Length == 0)
                throw new BadHttpRequestException("Empty voice file");

            if (file.Length > 2 * 1024 * 1024) // 2MB max
                throw new BadHttpRequestException("File too large");


            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

            if (extension != ".webm" && extension != ".ogg" && extension != ".wav")
                throw new BadHttpRequestException("Unsupported audio format");

            var safeFileName = $"msg_{messageId}{extension}";

            var path = Path.Combine(_basePath, safeFileName);

            await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await file.CopyToAsync(stream);

            return path;
        }
    }
}
