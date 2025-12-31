using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.DomainEvents.Domain.Common
{
    public abstract class Entity
    {
        public int Id { get; protected set; }

        private readonly List<IEvent> _domainEvents = new();
        public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents;

        protected void AddDomainEvent(IEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
