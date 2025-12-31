using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.NotificationsModels
{
    public class DeviceToken
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
