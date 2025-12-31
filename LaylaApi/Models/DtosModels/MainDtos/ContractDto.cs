namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ContractDto
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public string ContractUrl { get; set; } = string.Empty;
        public string? SpecialTerms { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSignedByOwner { get; set; }
        public bool IsSignedByRenter { get; set; }
    }
}
