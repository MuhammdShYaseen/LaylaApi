using LaylaApi.DomainEvents.Domain.Events;
using System.Threading.Channels;

namespace LaylaApi.DomainEvents.Domain.Dispatcher
{
    public class InMemoryEventDispatcher : IEventDispatcher, IDisposable
    {

        private readonly Channel<IEvent> _channel;
        private readonly ILogger<InMemoryEventDispatcher> _logger;
        public InMemoryEventDispatcher(ILogger<InMemoryEventDispatcher> logger, int capacity = 1000)
        {
            _logger = logger;
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<IEvent>(options);
        }

        public ChannelReader<IEvent> Reader => _channel.Reader;

        public async Task EnqueueAsync(IEvent @event, CancellationToken ct = default)
        {
            if (@event == null) throw new ArgumentNullException(nameof(@event));
            await _channel.Writer.WriteAsync(@event, ct);
            _logger.LogDebug("Event enqueued: {EventType}", @event.GetType().Name);
        }

        public void Dispose()
        {
            _channel.Writer.TryComplete();
        }
    }
}
