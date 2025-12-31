using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Services.AuthServices.Interfaces;

namespace LaylaApi.DomainEvents.Handlers
{
    public class PasswordResetHandler : IEventHandler<PasswordResetRequestedEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<PasswordResetHandler> _logger;

        public PasswordResetHandler(IEmailService emailService, ILogger<PasswordResetHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task HandleAsync(PasswordResetRequestedEvent @event, CancellationToken ct = default)
        {
            try
            {
                await _emailService.SendEmailAsync(@event.Email,"إعادة تعيين كلمة المرور", $"استخدم هذا الرمز لإعادة كلمة المرور: {@event.Token}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reset password email to user {Id}", @event.UserId);
            }
        }
    }
}
