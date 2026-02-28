namespace LaylaApi.Models.DtosModels.MessageDtos
{
    public record MessagesReadEvent(int ConversationId, int ReaderId, DateTime ReadAt);
}
