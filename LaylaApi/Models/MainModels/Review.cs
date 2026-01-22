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

        public static Review Create(int userId, int apartmentId,  int rating, string comment)
        {
            var review = new Review
            {
                UserId = userId,
                Comment = comment,
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
            };

            review.AddDomainEvent(new ReviewCreatedEvent(review));
            
            return review;
        }

        public void Update(int rating, string comment)
        {
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}