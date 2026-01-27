using LaylaApi.Models.MainModels;
using System.Globalization;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ContractSignedEvent : IEvent
    {
        public Guid ContractGuid { get; }
        public bool IsOwner { get; }
        public bool IsFullySigned { get; }



        public ContractSignedEvent(Guid contractGuid, bool isOwner, bool isFullySigned )
        {
            ContractGuid = contractGuid;
            IsOwner = isOwner;
            IsFullySigned = isFullySigned;
        }
    }
}