using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> GetOrCreateAsync(int apartmentId, int userId, CancellationToken ct);
        Task CloseAsync(int conversationId, int ownerId, CancellationToken ct);
        Task OpenAsync(int conversationId, int ownerId, CancellationToken ct);
    }
}
