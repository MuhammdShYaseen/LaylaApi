using LaylaApi.Attributes;
using LaylaApi.Models.MainModels;
using LaylaApi.ValueObjects.ApartmentValueObject;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ApartmentSearchRequestDto
    {
        public enum ApartmentSortBy
        {
            CreatedAt,
            Distance,
            Title,
            PricePerDay,
            PricePerHour,
            Area,
        }

        public enum SortDirections
        {
            Asc,
            Desc
        }
        public decimal? MinPricePerDay { get; set; }
        public decimal? MaxPricePerDay { get; set; }
        public decimal? MinPricePerHour { get; set; }
        public decimal? MaxPricePerHour { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MaxBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
        public int? MaxBathrooms { get; set; }
        public int? MinFloorNumber { get; set; }
        public int? MaxFloorNumber { get; set; }
        public int? MinLivingRooms { get; set; }
        public int? MaxLivingRooms { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public Apartment.BuildingType? Type { get; set; }
        public Apartment.ApartmentView? View { get; set; }
        public Apartment.Amenities? Finishing { get; set; }
        public bool? HasElevator { get; set; }
        public bool? HasParking { get; set; }
        public bool? HasPool { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Orientation { get; set; }
        public string? TitleKeyword { get; set; }
        public double? MinDistance { get; set; }
        public double? MaxDistance { get; set; }
        public double? UserLatitude { get; set; }
        public double? UserLongitude { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? City { get; set; }
        public string? Country { get; set; }
        public ApartmentSortBy? SortBy { get; set; }
        public SortDirections SortDirection { get; set; } = SortDirections.Desc;
    }
}
