
using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class MessageSentDomainEvent : IEvent
    {
        public int MessageId { get; }
        public int ConversationId { get; }
        public int SenderId { get; }
        public int ReceiverId { get; }

        public MessageSentDomainEvent(int messageId, int conversationId, int senderId, int receiverId)
        {
            MessageId = messageId;
            ConversationId = conversationId;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}
