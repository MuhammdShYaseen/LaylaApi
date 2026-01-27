using System.Diagnostics.Contracts;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ContractCreatedEvent : IEvent
    {
        public Guid ContractGuid { get; }

        public ContractCreatedEvent(Guid contractGuid)
        {
           ContractGuid = contractGuid;
        }
    }
}
