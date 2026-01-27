using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReviewCreatedHandler : IEventHandler<ReviewCreatedEvent>
    {

        private readonly INotificationService _notificationService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEventDataProvider<ReviewCreatedEvent, ReviewCreatedEventDto> _dataProvider;
        public ReviewCreatedHandler(INotificationService notificationService, IStringLocalizer<Notifications> localizer, IEventDataProvider<ReviewCreatedEvent, ReviewCreatedEventDto> dataProvider)
        {

            _notificationService = notificationService;
            _localizer = localizer;
            _dataProvider = dataProvider;

        }

        public async Task HandleAsync(ReviewCreatedEvent @event, CancellationToken ct = default)
        {
            var data = await _dataProvider.GetDataAsync(@event, ct);
            using (LocalizationHelper.UseCulture(data.OwnerLang))
            {
                var title = _localizer["New_Review"];
                var body = _localizer["Review_Body", data.Rating];

                await _notificationService.SendToUserAsync(data.OwnerId, title, body);
            }

        }
    }
}
