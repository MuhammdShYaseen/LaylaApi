using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.DomainEvents.Domain.Dispatcher
{
    public interface IEventHandler<in TEvent> where TEvent : IEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken ct = default);
    }
}
