using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class BookingCreatedHandler : IEventHandler<BookingCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _notificationsLocalizer;
        private readonly IEventDataProvider<BookingCreatedEvent, BookingCreatedEventDto> _dataProvider;
        public BookingCreatedHandler(INotificationService notificationService, IEmailService emailService , IStringLocalizer<Notifications> notificationsLocalizer, IEventDataProvider<BookingCreatedEvent, BookingCreatedEventDto> dataProvider)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _notificationsLocalizer = notificationsLocalizer;
            _notificationsLocalizer = notificationsLocalizer;
            _dataProvider = dataProvider;
        }

        public async Task HandleAsync(BookingCreatedEvent @event, CancellationToken ct = default)
        {

            var data = await _dataProvider.GetDataAsync(@event, ct);

            var startDate = data.StartDate.ToString("yyyy-MM-dd");
            var endDate = data.EndDate.ToString("yyyy-MM-dd");

            // 🔔 إشعار المالك
            using (LocalizationHelper.UseCulture(data.OwnerLanguage!))
            {
                var title = _notificationsLocalizer["BookingCreated_Owner_Title"];
                var body = _notificationsLocalizer["BookingCreated_Owner_Body", data.RenterName!, data.ApartmentTitle!, startDate, endDate];

                await _notificationService.SendToUserAsync(data.OwnerId, title, body);
                await _emailService.SendEmailAsync(data.OwnerEmail!, title, body);
            }

            // 🔔 إشعار المستأجر
            using (LocalizationHelper.UseCulture(data.RenterLanguage!))
            {
                var title = _notificationsLocalizer["BookingCreated_Renter_Title"];
                var body = _notificationsLocalizer["BookingCreated_Renter_Body", data.ApartmentTitle!, startDate, endDate];

                await _notificationService.SendToUserAsync(data.RenterId, title, body);
                await _emailService.SendEmailAsync(data.RenterEmail!, title, body);
            }
        }
    }
}