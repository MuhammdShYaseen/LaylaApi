using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.DomainEvents.Domain.Events;
using System.Globalization;
using System.Diagnostics.Contracts;
using LaylaApi.Models.DtosModels.MainDtos;

namespace LaylaApi.Models.MainModels
{
    public class Contract : Entity
    {

        [Required]
        public int BookingId { get; private set; }

        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }

        [Required]
        public string ContractUrl { get; private set; } = string.Empty; // رابط ملف PDF مثلاً
        public string SpecialTerms { get; private set; } = string.Empty;
        public bool IsSignedByOwner { get; private set; } = false;
        public bool IsSignedByRenter { get; private set; } = false;

        public static Contract Create(int bookingId, string specialTerms)
        {
            var contract = new Contract
            {
                BookingId = bookingId,
                SpecialTerms = !string.IsNullOrEmpty(specialTerms.Trim()) ? specialTerms : "",
                IsSignedByOwner = false,
                IsSignedByRenter = false
            };

            contract.AddDomainEvent(new ContractCreatedEvent(contract.Guid));
            return contract;
        }
        public void SignByOwner(Contract contract)
        {
            if (IsSignedByOwner)
                throw new InvalidOperationException("Contract already signed by owner.");
            IsSignedByOwner = true;
            Touch();
            AddDomainEvent(new ContractSignedEvent(contract,  true,  IsSignedByRenter));
        }

        public void SignByRenter(Contract contract)
        {
            if (IsSignedByRenter)
                throw new InvalidOperationException("Contract already signed by renter.");
            IsSignedByRenter = true;
            Touch();
            AddDomainEvent(new ContractSignedEvent(contract, false, IsSignedByOwner));
        }

        public void AddPdfUrl(string url)
        {
            ContractUrl = url;
            Touch();
        }

        public void Update(string specialTerms, string contractUrl)
        {
            SpecialTerms = specialTerms;
            ContractUrl = contractUrl;
            Touch();
        }
    }
}
