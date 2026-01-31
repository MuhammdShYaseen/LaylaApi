using Azure.Core;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Services.LanguageServices;
using LaylaApi.ValueObjects.UserValueObject;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.MainModels
{
    public class User : Entity
    {

        [Required, MaxLength(100)]
        public string FullName { get; private set; } = string.Empty;

        [Required, MaxLength(100)]
        public Email? Email { get; private set; } 

        [Required, MaxLength(20)]
        public PhoneNumber? PhoneNumber { get; private set; }

        [Required]
        public string PasswordHash { get; private set; } = string.Empty;

        [Required]
        public string Role { get; private set; } = "User"; // "Renter" or "Owner"
        public bool EmailConfirmed { get; private set; } = false;
        public Language? Lang { get; private set; }
        public string? EmailVerificationToken { get; private set; }
        public string? ResetPasswordToken { get; private set; }
        public DateTime? ResetPasswordTokenExpires { get; private set; }
        public DateTime? EmailVerificationTokenExpires { get; private set; }
        public ICollection<Apartment>? Apartments { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<RefreshToken>? RefreshToken { get; set; }
        public static User Create(RegisterRequest request, string passwordHash, string emailVerificationToken, ISupportedLanguagePolicy languagePolicy)
        {
            var user = new User
            {
                FullName = request.FullName,
                Email = Email.Create(request.Email),
                PhoneNumber = PhoneNumber.Create(request.PhoneNumber),
                PasswordHash = passwordHash,
                Role = "User",
                EmailConfirmed = false,
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                Lang = Language.Create (request.Lang, languagePolicy)
            };

            user.AddDomainEvent(new UserRegisteredEvent(user.Guid, user.EmailVerificationToken));
            return user;
        }

        public void Update(string fullName, string email, string phoneNumber, string lang, ISupportedLanguagePolicy languagePolicy)
        {
            FullName = fullName;
            Email = Email.Create(email);
            PhoneNumber = PhoneNumber.Create(phoneNumber);
            Lang =Language.Create(lang, languagePolicy);
            Touch();
        }

        public void ForgotPassword(string resetPasswordToken, DateTime resetPasswordTokenExpires)
        {
            ResetPasswordToken = resetPasswordToken;
            ResetPasswordTokenExpires = resetPasswordTokenExpires;
            AddDomainEvent(new PasswordResetRequestedEvent(Guid, resetPasswordToken));
            Touch();
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
