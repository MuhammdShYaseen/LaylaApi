using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ReviewCreatedEvent : IEvent
    {
        public int ReviewId { get; }
        public int ApartmentId { get; }
        public int UserId { get; }
        public int Rating { get; }

        public ReviewCreatedEvent(int reviewId, int apartmentId, int userId, int rating)
        {
            ReviewId = reviewId;
            ApartmentId = apartmentId;
            UserId = userId;
            Rating = rating;
        }   
    }
}
