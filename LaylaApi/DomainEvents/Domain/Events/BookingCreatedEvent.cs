using LaylaApi.Models.MainModels;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class BookingCreatedEvent : IEvent
    {
        public Booking Booking { get; }

        public BookingCreatedEvent(Booking booking)
        {
            Booking = booking;
        }
    }
}
}
