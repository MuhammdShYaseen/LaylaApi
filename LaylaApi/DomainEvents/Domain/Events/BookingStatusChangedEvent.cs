using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class BookingStatusChangedEvent : IEvent
    {
        public int BookingId { get; }
        public BookingStatus OldStatus { get; }
        public BookingStatus NewStatus { get; }

        public int ApartmentId { get; }
        public string ApartmentTitle { get; }

        public int OwnerId { get; }
        public string OwnerEmail { get; }
        public string OwnerLang { get; }

        public int RenterId { get; }
        public string RenterEmail { get; }
        public string RenterLang { get; }

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateTime OccurredAt { get; } = DateTime.UtcNow;

        public BookingStatusChangedEvent(int bookingId, BookingStatus oldStatus, BookingStatus newStatus, int apartmentId,
                                         string apartmentTitle, int ownerId, string ownerEmail, string ownerLang, int renterId,
                                         string renterEmail,  string renterLang, DateTime startDate, DateTime endDate)
        {
            BookingId = bookingId;
            OldStatus = oldStatus;
            NewStatus = newStatus;

            ApartmentId = apartmentId;
            ApartmentTitle = apartmentTitle;

            OwnerId = ownerId;
            OwnerEmail = ownerEmail;
            OwnerLang = ownerLang;

            RenterId = renterId;
            RenterEmail = renterEmail;
            RenterLang = renterLang;

            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
