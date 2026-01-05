
using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class MessageSentDomainEvent : IEvent
    {
        public int ConversationId { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }
        public string Content { get; }
        public int ApartmentId {  get; }

        public MessageSentDomainEvent(int conversationId, int senderId, int receiverId, string content, int apartmentId)
        {
            ConversationId = conversationId;
            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            ApartmentId = apartmentId;
        }
    }
}
