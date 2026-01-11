using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using System.Net;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.MainDtos;

namespace LaylaApi.Models.MainModels
{
    public class Apartment : Entity
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;} = DateTime.UtcNow;

        [Required]
        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }
        public bool IsChatEnabled { get; set; } = true;
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<MediaFile>? MediaFiles { get; set; }

        public static Apartment Create(CreateApartmentDto dto, User owner)
        {
            var apartment = new Apartment
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = dto.Address,
                PricePerHour = dto.PricePerHour,
                PricePerDay = dto.PricePerDay,
                Description = dto.Description,
                Location = dto!.Location,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Title = dto.Title,
                IsAvailable = dto.IsAvailable,
                Owner = owner,
                OwnerId = owner.Id,
                IsChatEnabled = true
            };

            apartment.AddDomainEvent(new ApartmentCreatedEvent(apartment));
            return apartment;
        }
    }
}