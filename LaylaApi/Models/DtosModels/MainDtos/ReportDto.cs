namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // نعرض فقط بيانات بسيطة عن المستخدم والشقة
        public int ReporterId { get; set; }
        public string ReporterName { get; set; } = string.Empty;

        public int ApartmentId { get; set; }
        public string ApartmentTitle { get; set; } = string.Empty;
    }
}
