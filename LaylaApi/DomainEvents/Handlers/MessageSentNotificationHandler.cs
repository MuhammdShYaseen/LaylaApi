using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Services.FirebaseServices.Interfaces;

namespace LaylaApi.DomainEvents.Handlers
{
    public class MessageSentNotificationHandler : IEventHandler<MessageSentDomainEvent>
    {
        private readonly INotificationService _firebase;
        public MessageSentNotificationHandler(INotificationService firebase)
        {
            _firebase = firebase;
        }
        public async Task HandleAsync(MessageSentDomainEvent @event, CancellationToken ct = default)
        {
            await _firebase.SendToUserAsync(@event.ReceiverId, "", "");
        }
    }
}
