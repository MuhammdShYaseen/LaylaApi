using LaylaApi.Services.AuthServices.Interfaces;
using System.Net.Mail;
using System.Net;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class SmtpEmailService:IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection.GetValue<string>("Host");
            var port = smtpSection.GetValue<int>("Port");
            var user = smtpSection.GetValue<string>("Username");
            var pass = smtpSection.GetValue<string>("Password");
            var from = smtpSection.GetValue<string>("From");

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };
            
            var mail = new MailMessage(from!, to, subject, body) { IsBodyHtml = true };

            try
            {
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }
    }
}
