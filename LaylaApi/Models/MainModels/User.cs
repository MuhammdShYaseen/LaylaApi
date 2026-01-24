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
        public string FullName { get; private set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; private set; } = string.Empty;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; private set; } = string.Empty;

        [Required]
        public string PasswordHash { get; private set; } = string.Empty;

        [Required]
        public string Role { get; private set; } = "User"; // "Renter" or "Owner"
        public bool EmailConfirmed { get; private set; } = false;
        public string Lang { get; private set; } = "en";
        public string? EmailVerificationToken { get; private set; }
        public string? ResetPasswordToken { get; private set; }
        public DateTime? ResetPasswordTokenExpires { get; private set; }
        public DateTime? EmailVerificationTokenExpires { get; private set; }
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

            user.AddDomainEvent(new UserRegisteredEvent(user, user.EmailVerificationToken));
            return user;
        }

        public void Update(string fullName, string email, string phoneNumber, string lang)
        {
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            Lang = Lang;
        }

        public void ForgotPassword(User user, string resetPasswordToken, DateTime resetPasswordTokenExpires)
        {
            ResetPasswordToken = resetPasswordToken;
            ResetPasswordTokenExpires = resetPasswordTokenExpires;
            user.AddDomainEvent(new PasswordResetRequestedEvent(user, resetPasswordToken));
        }

        public void ResetPassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            ResetPasswordToken = null;
            ResetPasswordTokenExpires = null;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
            EmailVerificationToken = null;
            EmailVerificationTokenExpires = null;
        }
    }
}
