using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;


namespace LaylaApi.Test.Services.MokeDbContext
{
    public class FakeEventDispatcher : IEventDispatcher
    {
        public Task EnqueueAsync(IEvent @event, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    } 
}
