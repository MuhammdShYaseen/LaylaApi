using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Models.MainModels;
using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.NotificationsModels
{
    public class DeviceToken : Entity
    {
        [Required]
        public int UserId { get; private set; }

        [Required]
        public string Token { get; private set; } = string.Empty;
        public string Platform { get; private set; } = null!;
        public string DeviceId { get; private set; } = null!;
        public DateTime LastSeenAt { get; private set; }
        public User User { get; set; } = null!;

        public static DeviceToken Create(int userId, string token, string platform, string deviceId)
        {
            return new DeviceToken 
            {
                UserId = userId,
                Token = token,
                Platform = platform,
                DeviceId = deviceId,
                LastSeenAt = DateTime.UtcNow,
            };
        }
        public void UpdateToken (string token)
        {
            Token = token;
            Touch();
            LastSeenAt = DateTime.UtcNow;
        }
    }
}
