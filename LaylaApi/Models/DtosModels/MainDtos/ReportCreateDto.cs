namespace LaylaApi.Models.DtosModels.MainDtos
{
    public class ReportCreateDto
    {
        // لإنشاء بلاغ جديد
        public int ReporterId { get; set; }
        public int ApartmentId { get; set; }

        // سبب البلاغ
        public string Reason { get; set; } = string.Empty;

        // فقط عند التحديث (للأدمِن)
        public string? Status { get; set; }  // Pending, Reviewed, Rejected, Resolved
    }
}
