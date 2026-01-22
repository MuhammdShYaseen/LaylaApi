using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;

namespace LaylaApi.Models.MainModels
{
    public class Payment : Entity
    {

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Method { get; set; } = "Online"; // Online, Cash

        public string? TransactionId { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

    }
}
