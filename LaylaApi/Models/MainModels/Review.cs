using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.Models.MainModels
{
    public class Review : Entity
    {

        [Required]
        public int ApartmentId { get; set; }

        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static Review Create(Review review, Apartment apartment, User user)
        {
            review.Apartment = apartment;
            review.User = user;
            review.AddDomainEvent(new ReviewCreatedEvent(review));
            review.CreatedAt = DateTime.UtcNow;
            return review;
        }
    }
}