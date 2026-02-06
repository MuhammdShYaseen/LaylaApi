using static LaylaApi.Models.MainModels.Apartment;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ApartmentDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Street { get; set; } = string.Empty;
        public string BuildingNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        #region Detiles
        public int NumberOfBedRooms { get;  set; } = 1;
        public int NumberOfLivingRooms { get;  set; } = 1;
        public int NumberOfReceptionRooms { get;  set; } = 1;
        public int NumberOfBathrooms { get;  set; } = 1;
        public int NumberOfBalconies { get;  set; } = 0;
        public int FloorNumber { get;  set; } = 0;
        public double Area { get;  set; }
        public string? Orientation { get;  set; }
        public BuildingType Type { get;  set; } = BuildingType.Apartment;
        public ApartmentView View { get;  set; } = ApartmentView.None;
        public Amenities Finishing { get;  set; } = Amenities.Standard;
        public bool HasElevator { get;  set; } = false;
        public bool HasParking { get;  set; } = false;
        public bool HasPool { get;  set; } = false;

        #endregion
        public decimal PricePerHour { get; set; }
        public decimal PricePerDay { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsChatEnabled { get; set; }
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
