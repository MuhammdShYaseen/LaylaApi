using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.DataCRUD.Interfaces;

namespace LaylaApi.DomainEvents.Handlers
{
    public class UserRegisteredHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly LaylaContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserRegisteredHandler> _logger;

        public UserRegisteredHandler(LaylaContext context, IEmailService emailService, ILogger<UserRegisteredHandler> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken ct = default)
        {
            var user = await _context.Users.FindAsync(new object[] { @event.UserId }, ct);

            if (user == null)
            {
                _logger.LogWarning("User not found for registration event: {Id}", @event.UserId);
                return;
            }

            if (string.IsNullOrEmpty(user.EmailVerificationToken))
            {
                _logger.LogWarning("User {Id} has no verification token generated.", user.Id);
                return;
            }

            // رابط التفعيل
            string verificationUrl =
                $"https://your-frontend-domain/verify-email?token={user.EmailVerificationToken}";

            string subject = "مرحباً بك في منصتنا!";
            string body =
                $"<p>مرحباً <b>{user.FullName}</b>،</p>" +
                "<p>شكراً لتسجيلك في منصتنا.</p>" +
                "<p>يرجى الضغط على الرابط التالي لتفعيل بريدك الإلكتروني:</p>" +
                $"<p><a href=\"{verificationUrl}\">تفعيل البريد الإلكتروني</a></p>";

            try
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to user {Id}", @event.UserId);
            }
        }
    }
}
