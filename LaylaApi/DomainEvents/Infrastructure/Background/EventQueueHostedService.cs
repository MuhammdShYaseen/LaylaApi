using LaylaApi.DomainEvents.Domain.Dispatcher;

namespace LaylaApi.DomainEvents.Infrastructure.Background
{
    public class EventQueueHostedService : BackgroundService
    {
        private readonly InMemoryEventDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventQueueHostedService> _logger;
        private readonly TimeSpan _delayOnError = TimeSpan.FromSeconds(2);

        public EventQueueHostedService(InMemoryEventDispatcher dispatcher,
            IServiceProvider serviceProvider,
            ILogger<EventQueueHostedService> logger)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EventQueueHostedService started.");

            var reader = _dispatcher.Reader;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var @event = await reader.ReadAsync(stoppingToken);

                    // Resolve handlers for the event type
                    using var scope = _serviceProvider.CreateScope();
                    var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
                    var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();

                    if (!handlers.Any())
                    {
                        _logger.LogWarning("No handlers found for event {EventType}", @event.GetType().Name);
                        continue;
                    }

                    foreach (var handler in handlers)
                    {
                        try
                        {
                            var method = handlerType.GetMethod("HandleAsync");
                            var task = (Task)method!.Invoke(handler, new object[] { @event, stoppingToken })!;
                            await task;
                        }
                        catch (Exception exHandler)
                        {
                            _logger.LogError(exHandler, "Handler {HandlerType} failed for event {EventType}",
                                handler!.GetType().Name, @event.GetType().Name);
                        }
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // shutting down
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing event from queue. delaying...");
                    await Task.Delay(_delayOnError, stoppingToken);
                }
            }

            _logger.LogInformation("EventQueueHostedService stopping.");
        }
    }
}