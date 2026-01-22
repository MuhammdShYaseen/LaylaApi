using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using System.Globalization;
using System.Diagnostics.Contracts;

namespace LaylaApi.Models.MainModels
{
    public class Contract : Entity
    {

        [Required]
        public int BookingId { get; set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        public string ContractUrl { get; set; } = string.Empty; // رابط ملف PDF مثلاً
        public string SpecialTerms { get; set; } = string.Empty;
        public bool IsSignedByOwner { get; set; } = false;
        public bool IsSignedByRenter { get; set; } = false;

        public static Contract Create(Booking booking, string specialTerms)
        {
            var contract = new Contract
            {
                Booking = booking,
                BookingId = booking.Id,
                SpecialTerms = !string.IsNullOrEmpty(specialTerms.Trim()) ? specialTerms : "",
                IsSignedByOwner = false,
                IsSignedByRenter = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

            };

            contract.AddDomainEvent(new ContractCreatedEvent(contract));
            return contract;
        }
        public void SignByOwner(Contract contract)
        {
            if (IsSignedByOwner)
                throw new InvalidOperationException("Contract already signed by owner.");
            IsSignedByOwner = true;
            AddDomainEvent(new ContractSignedEvent(contract,  true,  IsSignedByRenter));
        }

        public void SignByRenter(Contract contract)
        {
            if (IsSignedByRenter)
                throw new InvalidOperationException("Contract already signed by renter.");
            IsSignedByRenter = true;
            AddDomainEvent(new ContractSignedEvent(contract, false, IsSignedByOwner));
        }

        public void AddPdfUrl(string url)
        {
            ContractUrl = url;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
