using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class MessageSentNotificationHandler : IEventHandler<MessageSentDomainEvent>
    {
        private readonly INotificationService _firebase;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IUserService _userService;
        private readonly IApartmentService _apartmentService;
        public MessageSentNotificationHandler(INotificationService firebase, IStringLocalizer<Notifications> localizer, IUserService userService, IApartmentService apartmentService)
        {
            _firebase = firebase;
            _localizer = localizer;
            _userService = userService;
            _apartmentService = apartmentService;
        }
        public async Task HandleAsync(MessageSentDomainEvent @event, CancellationToken ct = default)
        {
            var receiver = await _userService.GetByIdAsync(@event.ReceiverId);
            var apartment = await _apartmentService.GetByIdAsync(@event.ApartmentId);

            using (LocalizationHelper.UseCulture(receiver!.Lang ?? "en"))
            {
                var title = _localizer["Chat_Message_Title", apartment.Title];
                var body = BuildNotificationBody(@event.Content).Replace("\r", " ").Replace("\n", " ");
                await _firebase.SendToUserAsync(@event.ReceiverId, title, body);
            }
                
        }
        private static string BuildNotificationBody(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            const int maxLength = 100;

            return content.Length <= maxLength
                ? content
                : content.Substring(0, maxLength) + "...";
        }
    }
}
