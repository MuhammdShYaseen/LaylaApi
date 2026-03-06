using System.ComponentModel.DataAnnotations;

namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class DeviceTokenDto
    {
        public int UserId { get;  set; }
        public string Token { get;  set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public DateTime? LastSeenAt { get; set; }
    }
}
