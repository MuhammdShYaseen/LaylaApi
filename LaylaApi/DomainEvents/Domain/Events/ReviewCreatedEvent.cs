using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ReviewCreatedEvent : IEvent
    {
       public Review Review { get; }

        public ReviewCreatedEvent(Review review)
        {
            Review = review;
        }   
    }
}
