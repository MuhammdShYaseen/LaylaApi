using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.MainModels;
using LaylaApi.Options;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace LaylaApi.DomainEvents.Handlers
{
    public class UserRegisteredHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly FrontendOptions _frontendOptions;

        public UserRegisteredHandler(IEmailService emailService, IStringLocalizer<Notifications> stringLocalizer, IOptions<FrontendOptions> options)
        {
            _emailService = emailService;
            _localizer = stringLocalizer;
            _frontendOptions = options.Value;
        }

        public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken ct = default)
        {
            var userName = @event.User.FullName;
            var userEmail = @event.User.Email;
            var verificationUrl = $"{_frontendOptions.Verify} { @event.Token }";
            var subject = _localizer["UserRegistered_Email_Subject",verificationUrl];
            var body = _localizer["UserRegistered_Email_Body", userName, verificationUrl];
            await _emailService.SendEmailAsync(userEmail, subject, body); 
        }
    }
}
