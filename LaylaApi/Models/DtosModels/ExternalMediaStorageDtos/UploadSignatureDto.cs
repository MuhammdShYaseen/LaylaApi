namespace LaylaApi.Models.DtosModels.ExternalMediaStorageDtos
{
    public class UploadSignatureDto
    {
        public string? Signature { get; set; }
        public long Timestamp { get; set; }

        public string? ApiKey { get; set; }
        public string? CloudName { get; set; }

        public string? Folder { get; set; }
        public long MaxFileSize { get; set; }

        public int MediaId { get; set; }
    }
}
