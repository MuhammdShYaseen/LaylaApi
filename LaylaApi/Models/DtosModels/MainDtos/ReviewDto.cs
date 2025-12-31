namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        // معلومات بسيطة عن الشقة
        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; } = string.Empty;

        // معلومات بسيطة عن المستخدم
        public int UserId { get; set; }
        public string UserFullName { get; set; } = string.Empty;
    }
}
