using System.Diagnostics.Contracts;

namespace LaylaApi.DomainEvents.Domain.Events
{
    public class ContractCreatedEvent : IEvent
    {
        public LaylaApi.Models.MainModels.Contract Contract { get; }

        public ContractCreatedEvent(LaylaApi.Models.MainModels.Contract contract)
        {
            Contract = contract;
        }
    }
}
