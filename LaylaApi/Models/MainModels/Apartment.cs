using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.ValueObjects.ApartmentValueObject;

namespace LaylaApi.Models.MainModels
{
    public class Apartment : Entity
    {
        #region Enums
        public enum Amenities
        {
            ShellAndCore,
            Basic,
            Standard,
            Good,
            Luxury,
            SuperLuxury,
            FullyFitted,
        }
        public enum ApartmentView
        {
            SeaView,
            MountainView,
            CityView,
            GardenView,
            StreetView,
            PanoramicView,
            DualAspect,
            CornerView,
            None
        }
        public enum BuildingType
        {
            Apartment,
            Villa,                  // فيلا
            Townhouse,              // تاون هاوس
            TwinHouse,              // منزل ثنائي
            DetachedHouse,          // منزل منفصل
            SemiDetachedHouse,      // منزل شبه منفصل
            Duplex,                 // دوبلكس
            Triplex,                // تريبلوكس
            Penthouse,              // بنتهاوس
            Palace,                 // قصر
            Farmhouse,              // مزرعة
            Chalet,                 // شاليه
        }
        #endregion

        [Required, MaxLength(200)]
        public string Title { get; private set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; private set; }

        [Required]
        public GeoLocation? Location { get; private set; }

        #region Detiles
        public int NumberOfBedRooms { get; private set; } = 1;
        public int NumberOfLivingRooms { get; private set; } = 1;
        public int NumberOfReceptionRooms { get; private set; } = 1;
        public int NumberOfBathrooms { get; private set; } = 1;
        public int NumberOfBalconies { get; private set; } = 0;
        public int FloorNumber { get; private set; } = 0;
        public double Area { get; private set; }
        public string Orientation { get; private set; } = string.Empty;
        public BuildingType Type { get; private set; } = BuildingType.Apartment;
        public ApartmentView View {  get; private set; } = ApartmentView.None;
        public Amenities Finishing { get; private set; } = Amenities.Standard;
        public bool HasElevator { get; private set; } = false;
        public bool HasParking { get; private set; } = false;
        public bool HasPool { get; private set; } = false;

        #endregion

        [Required]
        public Money PricePerHour { get; private set; } =  Money.Create(1);

        [Required]
        public Money PricePerDay { get; private set; } = Money.Create(1);

        public bool IsAvailable { get; private set; } = true;
        

        [Required]
        public int OwnerId { get; private set; }

        [ForeignKey("OwnerId")]
        public User Owner { get; set; } = null!;
        public bool IsChatEnabled { get; private set; } = true;
        public ICollection<Booking> Bookings { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
        public ICollection<MediaFile> MediaFiles { get; set; } = [];

        public static Apartment Create(CreateApartmentDto dto, int ownerId)
        {
            var location = GeoLocation.Create(dto.Street, dto.BuildingNumber,
                                              dto.ApartmentNumber, dto.City,
                                              dto.District, Coordinates.Create(dto.Latitude, dto.Longitude),
                                              dto.Country);
            var apartment = new Apartment
            {
                Location = location,
                Area = dto.Area,
                Finishing = dto.Finishing,
                FloorNumber = dto.FloorNumber,
                HasElevator = dto.HasElevator,
                HasParking = dto.HasParking,
                HasPool = dto.HasPool,
                NumberOfBalconies = dto.NumberOfBalconies,
                NumberOfBathrooms = dto.NumberOfBathrooms,
                NumberOfBedRooms = dto.NumberOfBedRooms,
                NumberOfLivingRooms = dto.NumberOfLivingRooms,
                NumberOfReceptionRooms = dto.NumberOfReceptionRooms,
                Orientation = dto.Orientation,
                View = dto.View,
                Type = dto.Type,
                PricePerHour = Money.Create(dto.PricePerHour),
                PricePerDay = Money.Create(dto.PricePerDay),
                Description = dto.Description,

                Title = string.IsNullOrWhiteSpace(dto.Title) ? throw new BadHttpRequestException("Title must not be empty") : dto.Title,
                IsAvailable = dto.IsAvailable,
                OwnerId = ownerId,
                IsChatEnabled = true
            };

            apartment.AddDomainEvent(new ApartmentCreatedEvent(apartment.Guid));

            return apartment;
        }
        public void Update(CreateApartmentDto dto)
        {

            Location = GeoLocation.Create(dto.Street, dto.BuildingNumber,
                                          dto.ApartmentNumber, dto.City,
                                          dto.District,Coordinates.Create(dto.Latitude, dto.Longitude),
                                          dto.Country);

            PricePerHour = Money.Create(dto.PricePerHour);
            PricePerDay = Money.Create(dto.PricePerDay);
            Description = dto.Description;

            Area = dto.Area;
            Finishing = dto.Finishing;
            FloorNumber = dto.FloorNumber;
            HasElevator = dto.HasElevator;
            HasParking = dto.HasParking;
            HasPool = dto.HasPool;
            NumberOfBalconies = dto.NumberOfBalconies;
            NumberOfBathrooms = dto.NumberOfBathrooms;
            NumberOfBedRooms = dto.NumberOfBedRooms;
            NumberOfLivingRooms = dto.NumberOfLivingRooms;
            NumberOfReceptionRooms = dto.NumberOfReceptionRooms;
            Orientation = dto.Orientation;
            View = dto.View;
            Type = dto.Type;

            Title = dto.Title;
            IsAvailable = dto.IsAvailable;
            IsChatEnabled = dto.IsChatEnabled;
            Touch();

        }

        public void EnableDisableChat(bool isChatEnabled)
        {
            IsChatEnabled = isChatEnabled;
        }
        public void Availability (bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

       
    }
}