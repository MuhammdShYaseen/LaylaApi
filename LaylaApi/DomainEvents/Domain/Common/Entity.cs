using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.DomainEvents.Domain.Common
{
    public abstract class Entity
    {
        //common properties
        public int Id { get; protected set; }
        public Guid Guid { get; private set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; private set; } = false;


        //events
        private readonly List<IEvent> _domainEvents = new();
        public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents;
        protected void AddDomainEvent(IEvent domainEvent)
            => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents()
            => _domainEvents.Clear();


        //delete, restore methods for soft delete
        public void Delete()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }
    }
}
