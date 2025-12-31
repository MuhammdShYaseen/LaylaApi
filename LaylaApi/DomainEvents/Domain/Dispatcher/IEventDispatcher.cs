using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.DomainEvents.Domain.Dispatcher
{
    public interface IEventDispatcher
    {
        Task EnqueueAsync(IEvent @event, CancellationToken ct = default);
    }
}
