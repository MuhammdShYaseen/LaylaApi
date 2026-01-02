using Google.Protobuf.WellKnownTypes;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Options;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace LaylaApi.DomainEvents.Handlers
{
    public class PasswordResetHandler : IEventHandler<PasswordResetRequestedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly FrontendOptions _frontendOptions;
        public PasswordResetHandler(IEmailService emailService, IStringLocalizer<Notifications> stringLocalizer, IOptions<FrontendOptions> options)
        {
            _emailService = emailService;
            _localizer = stringLocalizer;
            _frontendOptions = options.Value;
        }

        public async Task HandleAsync(PasswordResetRequestedEvent @event, CancellationToken ct = default)
        {
            var resetUrl = $"{_frontendOptions.RestPasswordURL}{@event.Token}";
            var subject = _localizer["PasswordReset_Email_Subject"];
            var body = _localizer["PasswordReset_Email_Body", resetUrl];
            var userEmail = @event.User.Email;
            await _emailService.SendEmailAsync(userEmail, subject, body);
        }
    }
}
