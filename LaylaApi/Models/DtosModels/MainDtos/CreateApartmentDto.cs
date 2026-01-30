using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class CreateApartmentDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? Address { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public decimal PricePerHour { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsChatEnabled { get; set; } = true;

    }
}
