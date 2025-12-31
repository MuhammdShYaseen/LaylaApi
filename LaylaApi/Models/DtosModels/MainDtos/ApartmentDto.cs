namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ApartmentDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string Location { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }

        public bool IsAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        // Owner
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;

        // Collections mapped
        public List<string>? MediaUrls { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
