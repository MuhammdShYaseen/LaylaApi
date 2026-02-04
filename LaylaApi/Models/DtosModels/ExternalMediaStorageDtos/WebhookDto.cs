namespace LaylaApi.Models.DtosModels.ExternalMediaStorageDtos
{
    public class WebhookDto
    {
        public string? PublicId { get; set; }
        public string? ResourceType { get; set; }

        public string? Format { get; set; }
        public long Bytes { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }

        public double? Duration { get; set; }

        public string? SecureUrl { get; set; }

        public Dictionary<string, string>? Context { get; set; }
    }
}
