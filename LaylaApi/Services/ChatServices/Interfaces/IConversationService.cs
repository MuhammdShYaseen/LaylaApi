using LaylaApi.Models.DtosModels.ConversationDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IConversationService
    {
        Task<Conversation> GetOrCreateAsync(int apartmentId, int userId, CancellationToken ct);
        Task<IReadOnlyList<ConversationDto>> GetUserConversationsAsync(int userId, CancellationToken ct);

        Task<ConversationDetailsDto?> GetDetailsAsync(int conversationId, int userId, CancellationToken ct);

        Task CloseAsync(int conversationId, int ownerId, CancellationToken ct);
        Task OpenAsync(int conversationId, int ownerId, CancellationToken ct);
    }
}
