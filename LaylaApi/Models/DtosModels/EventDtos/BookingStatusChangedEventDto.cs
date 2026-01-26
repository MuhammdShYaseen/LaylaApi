using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class BookingStatusChangedEventDto
    {
        public Guid BookingId { get; init; }

        public string ApartmentTitle { get; init; } = null!;

        public int OwnerId { get; init; }
        public string OwnerEmail { get; init; } = null!;
        public string OwnerLang { get; init; } = "en";

        public int RenterId { get; init; }
        public string RenterEmail { get; init; } = null!;
        public string RenterLang { get; init; } = "en";

        public BookingStatus NewStatus { get; init; }
    }
}
