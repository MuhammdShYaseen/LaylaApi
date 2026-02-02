using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using System.Net;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.ValueObjects.ApartmentValueObject;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace LaylaApi.Models.MainModels
{
    public class Apartment : Entity
    {

        [Required, MaxLength(200)]
        public string Title { get; private set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; private set; }

        [Required]
        public GeoLocation? Location { get; private set; }

        [Required]
        public Money? PricePerHour { get; private set; }

        [Required]
        public Money? PricePerDay { get; private set; }

        public bool IsAvailable { get; private set; } = true;
        

        [Required]
        public int OwnerId { get; private set; }

        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }
        public bool IsChatEnabled { get; private set; } = true;
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<MediaFile>? MediaFiles { get; set; }

        public static Apartment Create(CreateApartmentDto dto, int ownerId)
        {
            var location = GeoLocation.Create(dto.Street, dto.BuildingNumber,
                                              dto.ApartmentNumber, dto.City,
                                              dto.District, Coordinates.Create(dto.Latitude, dto.Longitude),
                                              dto.Country);
            var apartment = new Apartment
            {
                Location = location,

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