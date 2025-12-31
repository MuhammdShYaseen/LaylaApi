using Azure.Core;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.AuthDtos;
using Microsoft.AspNetCore.Identity;
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
        public static User Create(RegisterRequest request, string passwordHash, string emailVerificationToken)
        {
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = passwordHash,
                Role = "User",
                EmailConfirmed = false,
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                Lang = request.Lang,
            };

            user.AddDomainEvent(new UserRegisteredEvent(user));
            return user;
        }

        public void PasswordReset(User user, string token)
        {
            user.AddDomainEvent(new PasswordResetRequestedEvent(user, token));
        }
    }
}
