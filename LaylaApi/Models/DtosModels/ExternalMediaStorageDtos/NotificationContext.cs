using System.Text.Json.Serialization;

namespace LaylaApi.Models.DtosModels.ExternalMediaStorageDtos
{
    public class NotificationContext
    {
        [JsonPropertyName("triggered_at")]
        public DateTime TriggeredAt { get; set; }

        [JsonPropertyName("triggered_by")]
        public TriggeredBy? TriggeredBy { get; set; }
    }
}
