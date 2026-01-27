namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class ContractCreatedEventDto
    {
        public Guid ContractId { get; init; }
        public int BookingId { get; init; }

        public string ApartmentTitle { get; init; } = null!;

        public int OwnerId { get; init; }
        public string OwnerEmail { get; init; } = null!;
        public string OwnerLang { get; init; } = "en";

        public int RenterId { get; init; }
        public string RenterEmail { get; init; } = null!;
        public string RenterLang { get; init; } = "en";
    }
}
