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
            Price,
            Distance
        }

        public enum SortDirections
        {
            Asc,
            Desc
        }
        [IgnoreIfNonPositive]
        public decimal? MinPricePerDay { get; set; }

        [IgnoreIfNonPositive]
        public decimal? MaxPricePerDay { get; set; }

        [IgnoreIfNonPositive]
        public decimal? MinPricePerHour { get; set; }

        [IgnoreIfNonPositive]
        public decimal? MaxPricePerHour { get; set; }

        [IgnoreIfNonPositive]
        public double? MinArea { get; set; }

        [IgnoreIfNonPositive]
        public double? MaxArea { get; set; }

        [IgnoreIfNonPositive]
        public int? MinBedrooms { get; set; }

        [IgnoreIfNonPositive]
        public int? MaxBedrooms { get; set; }

        [IgnoreIfNonPositive]
        public int? MinBathrooms { get; set; }
        [IgnoreIfNonPositive]
        public int? MaxBathrooms { get; set; }

        [IgnoreIfNonPositive]
        public int? MinFloorNumber { get; set; }

        [IgnoreIfNonPositive]
        public int? MaxFloorNumber { get; set; }

        [IgnoreIfNonPositive]
        public int? MinLivingRooms { get; set; }

        [IgnoreIfNonPositive]
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

        [IgnoreIfNonPositive]
        public double? MinDistance { get; set; }

        [IgnoreIfNonPositive]
        public double? MaxDistance { get; set; }
        public double? UserLatitude { get; set; }
        public double? UserLongitude { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? SortBy { get; set; }
        public SortDirections SortDirection { get; set; } = SortDirections.Desc;
    }
}
