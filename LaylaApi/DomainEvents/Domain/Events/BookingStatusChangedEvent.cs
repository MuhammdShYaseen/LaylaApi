using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class BookingStatusChangedEvent : IEvent
    {
        public Guid BookingGuid { get; }
        public BookingStatus NewStatus { get; }

        public BookingStatusChangedEvent(Guid bookingGuid, BookingStatus newStatus)
        {
            BookingGuid = bookingGuid;
            NewStatus = newStatus;
        }
    }
}
