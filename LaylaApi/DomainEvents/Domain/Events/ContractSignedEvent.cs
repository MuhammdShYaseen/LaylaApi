using LaylaApi.Models.MainModels;
using System.Globalization;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ContractSignedEvent : IEvent
    {
        public Contract Contract { get; }
        public bool IsOwner { get; }
        public bool IsFullySigned { get; }



        public ContractSignedEvent(Contract contract, bool isOwner, bool isFullySigned )
        {
            Contract = contract;
            IsOwner = isOwner;
            IsFullySigned = isFullySigned;
        }
    }
}