using FirebaseAdmin.Messaging;
using LaylaApi.DataAccess;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Message = FirebaseAdmin.Messaging.Message;

namespace LaylaApi.Services.FirebaseServices.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly LaylaContext _context;

        public NotificationService(LaylaContext context)
        {
            _context = context;
        }

        public async Task SendToTokenAsync(string token, string title, string body)
        {
            var message = new Message
            {
                Token = token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async Task SendToUserAsync(int userId, string title, string body)
        {
            var tokens = await _context.DeviceTokens.Where(t => t.UserId == userId).Select(t => t.Token).ToListAsync();

            foreach (var token in tokens)
            {
                await SendToTokenAsync(token, title, body);
            }
        }

        public async Task SendToAllAsync(string title, string body)
        {
            var tokens = await _context.DeviceTokens.Select(t => t.Token).ToListAsync();

            foreach (var token in tokens)
            {
                await SendToTokenAsync(token, title, body);
            }
        }

        public async Task SendToTopicAsync(string topic, string title, string body)
        {
            var message = new Message
            {
                Topic = topic,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                }
            };

            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }

        public async Task SendAdminAsync(string title, string body)
        {

            var adminIds = await _context.Users.Where(u => u.Role == "Admin").Select(u => u.Id).ToListAsync();

            if (!adminIds.Any())
                return;

            var tokens = await _context.DeviceTokens.Where(t => adminIds.Contains(t.UserId)).ToListAsync();
            foreach (var token in tokens)
            {
                await SendToTokenAsync(token.Token, title, body);
            }
        }
    }
}
