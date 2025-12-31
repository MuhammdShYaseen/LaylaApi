using LaylaApi.DomainEvents.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.MainModels
{
    public class User : Entity
    {

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; // "Renter" or "Owner"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool EmailConfirmed { get; set; } = false;
        public string Lang { get; set; } = "en";
        public string? EmailVerificationToken { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpires { get; set; }
        public DateTime? EmailVerificationTokenExpires { get; set; }
        public ICollection<Apartment>? Apartments { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<RefreshToken>? RefreshToken { get; set; }
        public static User Create(string fullName, string email, string phoneNumber, string passwordHash, string role = "Renter", bool generateVerificationToken = true)
        {
            var user = new User
            {
                FullName = fullName.Trim(),
                Email = email.Trim().ToLower(),
                PhoneNumber = phoneNumber.Trim(),
                PasswordHash = passwordHash,
                Role = role,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                Lang = "en"
            };

            //user.AddDomainEvent(new UserRegisteredEvent(user));
            return user;
        }
    }
}
