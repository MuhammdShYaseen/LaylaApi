using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;

namespace LaylaApi.DomainEvents.Handlers
{
    public class PaymentCompletedHandler : IEventHandler<PaymentCompletedEvent>
    {
        public Task HandleAsync(PaymentCompletedEvent @event, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
