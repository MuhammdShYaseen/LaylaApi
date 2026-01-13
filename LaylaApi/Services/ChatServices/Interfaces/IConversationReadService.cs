namespace LaylaApi.Services.ChatServices.Interfaces
{
    public interface IConversationReadService
    {
        Task<bool> IsParticipantAsync(int conversationId, int userId);
    }
}
