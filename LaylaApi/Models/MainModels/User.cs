
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

using LaylaApi.Services.LanguageServices;
using LaylaApi.ValueObjects.UserValueObject;

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
        public static User Create(string fullName, string email, string phoneNumber,
                                  string password, string passwordHash,string lang,
                                  string emailVerificationToken, ISupportedLanguagePolicy languagePolicy)
        {
            ValidatePassword(password);
            ValidateRequest(fullName, email, phoneNumber, lang, languagePolicy);
            var user = new User
            {
                FullName = fullName,
                Email = Email.Create(email),
                PhoneNumber = PhoneNumber.Create(phoneNumber),
                PasswordHash = passwordHash,
                Role = "User",
                EmailConfirmed = false,
                EmailVerificationToken = emailVerificationToken,
                EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                Lang = Language.Create (lang, languagePolicy)
            };

            user.AddDomainEvent(new UserRegisteredEvent(user.Guid, user.EmailVerificationToken));
            return user;
        }

        public void Update(string fullName, string phoneNumber, string lang, ISupportedLanguagePolicy languagePolicy)
        {
            ValidateRequest(fullName, Email!.Value, phoneNumber, lang, languagePolicy);

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

        private static void ValidateRequest(string fullName,string email, string phoneNumber,string lang, ISupportedLanguagePolicy languagePolicy)
        {

            // Validate FullName
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required");

            if (fullName.Length < 2 || fullName.Length > 100)
                throw new ArgumentException("Full name must be between 2 and 100 characters");

            // Validate Email
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            // Validate PhoneNumber
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required");

            // Validate Language
            if (string.IsNullOrWhiteSpace(lang))
                throw new ArgumentException("Language is required");
        }
    }
}
