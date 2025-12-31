using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class PasswordResetHandler : IEventHandler<PasswordResetRequestedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;
        public PasswordResetHandler(IEmailService emailService, IStringLocalizer<Notifications> stringLocalizer)
        {
            _emailService = emailService;
            _localizer = stringLocalizer;
        }

        public async Task HandleAsync(PasswordResetRequestedEvent @event, CancellationToken ct = default)
        {
            var resetUrl = @event.
            var subject = _localizer["PasswordReset_Email_Subject"];
            var body = _localizer["PasswordReset_Email_Body", resetUrl];
            await _emailService.SendEmailAsync(@event.Email,"إعادة تعيين كلمة المرور", $"استخدم هذا الرمز لإعادة كلمة المرور: {@event.Token}");
        }
    }
}
