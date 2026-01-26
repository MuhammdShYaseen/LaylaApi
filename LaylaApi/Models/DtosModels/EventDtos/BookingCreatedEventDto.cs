namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class BookingCreatedEventDto
    {
        public string? ApartmentTitle { get; init; }

        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public int OwnerId { get; init; }
        public string? OwnerEmail { get; init; }
        public string? OwnerLanguage { get; init; }

        public int RenterId { get; init; }
        public string? RenterEmail { get; init; }
        public string? RenterName { get; init; }
        public string? RenterLanguage { get; init; }
    }
}
