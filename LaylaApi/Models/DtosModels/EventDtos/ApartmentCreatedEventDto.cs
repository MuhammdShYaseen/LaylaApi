namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class ApartmentCreatedEventDto
    {
        public Guid ApartmentId { get; init; }      // اختياري للتتبع
        public string? ApartmentTitle { get; init; } // لإدراج في الرسالة
        public int OwnerId { get; init; }          // لإرسال Notification
        public string? OwnerEmail { get; init; }     // لإرسال Email
        public string? OwnerLang { get; init; }      // لتحديد اللغة
    }
}
