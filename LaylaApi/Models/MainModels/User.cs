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

        public string? PendingEmail { get; private set; }
        public string? EmailChangeToken { get; private set; }
        public DateTime? EmailChangeTokenExpires { get; private set; }

        public ICollection<Apartment>? Apartments { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<RefreshToken>? RefreshToken { get; set; }
        public static User Create(RegisterRequest request, string passwordHash, string emailVerificationToken, ISupportedLanguagePolicy languagePolicy)
        {
            ValidatePassword(request.Password);
            ValidateRequest(request, languagePolicy);
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
            PhoneNumber = PhoneNumber.Create(phoneNumber);
            Lang =Language.Create(lang, languagePolicy);
            Touch();
        }

        public void RequestEmailChange(string newEmail)
        {
            if (Email!.Value == newEmail)
                return;

            PendingEmail = Email.Create(newEmail).Value;

            EmailChangeToken = Guid.NewGuid().ToString();

            EmailChangeTokenExpires = DateTime.UtcNow.AddHours(1);

            AddDomainEvent(new UserEmailChangedEvent(
                Guid,
                PendingEmail,
                FullName,
                Lang!.Code,
                EmailChangeToken,
                EmailChangeTokenExpires
            ));

            Touch();
        }

        public void ConfirmEmailChange(string token)
        {
            if (EmailChangeToken != token)
                throw new InvalidOperationException("Invalid token");

            if (EmailChangeTokenExpires < DateTime.UtcNow)
                throw new InvalidOperationException("Token expired");

            Email = Email.Create(PendingEmail!);
            EmailConfirmed = true;

            PendingEmail = null;
            EmailChangeToken = null;
            EmailChangeTokenExpires = null;

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
        private static void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required");

            if (password.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters");
        }

        private static void ValidateRequest(RegisterRequest request, ISupportedLanguagePolicy languagePolicy)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Validate FullName
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required");

            if (request.FullName.Length < 2 || request.FullName.Length > 100)
                throw new ArgumentException("Full name must be between 2 and 100 characters");

            // Validate Email
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");

            // Validate PhoneNumber
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                throw new ArgumentException("Phone number is required");

            // Validate Language
            if (string.IsNullOrWhiteSpace(request.Lang))
                throw new ArgumentException("Language is required");
        }
    }
}
