using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class BookingCreatedHandler : IEventHandler<BookingCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _notificationsLocalizer;
        public BookingCreatedHandler(INotificationService notificationService, IEmailService emailService , IStringLocalizer<Notifications> notificationsLocalizer)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _notificationsLocalizer = notificationsLocalizer;
            _notificationsLocalizer = notificationsLocalizer;
        }

        public async Task HandleAsync(BookingCreatedEvent @event, CancellationToken ct = default)
        {
           
            var apartmentTitle = @event.Booking.Apartment!.Title;
            var renterName = @event.Booking.User!.FullName;
            var startDate = @event.Booking.StartDate.ToString("yyyy-MM-dd");
            var endDate = @event.Booking.EndDate.ToString("yyyy-MM-dd");
            var ownerId = @event.Booking.Apartment.OwnerId;
            var ownerEmail = @event.Booking.Apartment.Owner!.Email;
            var renterEmail = @event.Booking.User.Email;
            var renterId = @event.Booking.UserId;
            var ownerLanguage = @event.Booking.Apartment.Owner.Lang;
            var renterLanguage = @event.Booking.User.Lang;

            // 🔔 إشعار المالك
            using (LocalizationHelper.UseCulture(ownerLanguage))
            {
                var title = _notificationsLocalizer["BookingCreated_Owner_Title"];
                var body = _notificationsLocalizer["BookingCreated_Owner_Body" , renterName, apartmentTitle , startDate ,  endDate];

                await _notificationService.SendToUserAsync(ownerId, title, body);
                await _emailService.SendEmailAsync(ownerEmail, title, body);
            }

            // 🔔 إشعار المستأجر
            using (LocalizationHelper.UseCulture(renterLanguage))
            {
                var title = _notificationsLocalizer["BookingCreated_Renter_Title"];
                var body = _notificationsLocalizer["BookingCreated_Renter_Body" , apartmentTitle , startDate ,endDate];

                await _notificationService.SendToUserAsync(renterId, title, body);
                await _emailService.SendEmailAsync(renterEmail, title, body);
            }
        }
    }
}