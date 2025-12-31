using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.Models.MainModels
{
    public class Report : Entity
    {

        [Required]
        public int ReporterId { get; set; } // المستخدم الذي قام بالتبليغ

        [ForeignKey("ReporterId")]
        public User? Reporter { get; set; }

        [Required]
        public int ApartmentId { get; set; } // الشقة المبلغ عنها

        [ForeignKey("ApartmentId")]
        public Apartment? Apartment { get; set; }

        [Required, MaxLength(1000)]
        public string Reason { get; set; } = string.Empty; // سبب التبليغ

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Rejected, Resolved

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static Report Create(Apartment apartment, User reporter ,string reason)
        {
            var report = new Report
            {
                Apartment = apartment,
                Reporter = reporter,
                Reason = reason,
                ApartmentId = apartment.Id,
                CreatedAt = DateTime.UtcNow,
                ReporterId = reporter.Id,
                Status = "Pending"
            };

            report.AddDomainEvent(new ReportCreatedEvent(report));
            return report;
        }
    }
}
