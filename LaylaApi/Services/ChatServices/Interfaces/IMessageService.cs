using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendTextAsync(int conversationId, int senderId, string content, CancellationToken ct);
        Task<Message> SendVoiceAsync(int conversationId, int senderId, IFormFile file, int duration, CancellationToken ct);
    }
}
