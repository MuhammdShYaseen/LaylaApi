namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class MediaFileDto
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }

        public string FileUrl { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}
