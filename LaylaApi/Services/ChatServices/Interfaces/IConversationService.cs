using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> GetOrCreateAsync(int apartmentId, int userId);
        Task CloseAsync(int conversationId, int ownerId);
    }
}
