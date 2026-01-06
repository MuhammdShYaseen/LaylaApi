using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReviewCreatedHandler : IEventHandler<ReviewCreatedEvent>
    {

        private readonly INotificationService _notificationService;
        private readonly IStringLocalizer<Notifications> _localizer;
        public ReviewCreatedHandler(INotificationService notificationService, IStringLocalizer<Notifications> localizer)
        {

            _notificationService = notificationService;
            _localizer = localizer;

        }

        public async Task HandleAsync(ReviewCreatedEvent @event, CancellationToken ct = default)
        {
            var apartment = @event.Review.Apartment;
            var ownerId = apartment!.OwnerId;
            var ownerLang = apartment.Owner!.Lang;

            using (Helper.Localization.LocalizationHelper.UseCulture(ownerLang))
            {
               
                var rating = @event.Review.Rating;
                var title = _localizer["New_Review"];
                var body = _localizer["Review_Body", rating];

                await _notificationService.SendToUserAsync(ownerId, title, body);
            }
            
        }
    }
}
