using LaylaApi.DomainEvents.Domain.Events;
using System.Threading.Channels;

namespace LaylaApi.DomainEvents.Domain.Dispatcher
{
    public class InMemoryEventDispatcher : IEventDispatcher, IDisposable
    {

        private readonly Channel<IEvent> _channel;
        public InMemoryEventDispatcher(int capacity = 1000)
        {
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
        }

        public void Dispose()
        {
            _channel.Writer.TryComplete();
        }
    }
}
