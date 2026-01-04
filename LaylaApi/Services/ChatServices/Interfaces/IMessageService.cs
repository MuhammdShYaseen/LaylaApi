using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IMessageService
    {
        Task<Message> SendTextAsync(int conversationId, int senderId, string content);
        Task<Message> SendVoiceAsync(int conversationId, int senderId, IFormFile file, int duration);
    }
}
