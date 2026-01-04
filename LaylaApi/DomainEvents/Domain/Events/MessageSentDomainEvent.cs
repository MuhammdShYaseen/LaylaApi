
using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class MessageSentDomainEvent : IEvent
    {
        public Message Message { get; }
        public int ReceiverId { get; }
        public MessageSentDomainEvent(Message message, int ReceiverId)
        {
            Message = message;
        }
    }
}
