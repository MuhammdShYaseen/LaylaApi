

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ReviewCreatedEvent : IEvent
    { 
        public Guid ReviewGuid { get; }
        public ReviewCreatedEvent(Guid reviewGuid)
        {
            ReviewGuid = reviewGuid;
        }

       
    }
}
