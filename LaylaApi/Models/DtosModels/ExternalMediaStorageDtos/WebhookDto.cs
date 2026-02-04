using System.Text.Json.Serialization;

namespace LaylaApi.Models.DtosModels.ExternalMediaStorageDtos
{
    public class WebhookDto
    {

        [JsonPropertyName("notification_type")]
        public string? NotificationType { get; set; }


        [JsonPropertyName("public_id")]
        public string? PublicId { get; set; }


        [JsonPropertyName("secure_url")]
        public string? SecureUrl { get; set; }


        [JsonPropertyName("bytes")]
        public long Bytes { get; set; }


        [JsonPropertyName("width")]
        public int? Width { get; set; }


        [JsonPropertyName("height")]
        public int? Height { get; set; }


        [JsonPropertyName("resource_type")]
        public string? ResourceType { get; set; }


        [JsonPropertyName("format")]
        public string? Format { get; set; }


        [JsonPropertyName("notification_context")]
        public NotificationContext? NotificationContext { get; set; }

        [JsonPropertyName("context")]
        public Dictionary<string, string>? Context { get; set; }


    }
}
