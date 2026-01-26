
namespace LaylaApi.DomainEvents.Domain.Events
{
    public class BookingCreatedEvent : IEvent
    {
        public Guid BookingGuid { get; }

        public BookingCreatedEvent(Guid bookingGuid)
        {
            BookingGuid = bookingGuid;
        }
    }
}
