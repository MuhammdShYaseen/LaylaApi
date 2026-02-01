using Google.Protobuf.WellKnownTypes;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Models.MainModels;
using LaylaApi.Options;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LaylaApi.DomainEvents.Handlers
{
    public class UserEmailChangedEventHandler : IEventHandler<UserEmailChangedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IOptions<FrontendOptions> _frontendOptions;
        private readonly IStringLocalizer _localizer;
        public UserEmailChangedEventHandler(IEmailService emailService, IOptions<FrontendOptions> frontendOptions, IStringLocalizer stringLocalizer)
        {
            _emailService = emailService;
            _frontendOptions = frontendOptions;
            _localizer = stringLocalizer;
        }
        public async Task HandleAsync(UserEmailChangedEvent @event, CancellationToken ct = default)
        {
            var verifyUrl = $"{_frontendOptions.Value.Verify}{@event.EmailVerificationToken}";
            using (LocalizationHelper.UseCulture(@event.Language))
            {
                var subject = _localizer["UserUpdateEmailAddress_Email_Subject"];
                var body = _localizer["UserUpdateEmailAddress_Email_Body", @event.FullName, verifyUrl, @event.EmailVerificationTokenExpires?.ToString("yyyy - MM - dd HH: mm") ?? "N/A"];
                await _emailService.SendEmailAsync(@event.NewEmail, subject, body);
            }
        }
    }
}
