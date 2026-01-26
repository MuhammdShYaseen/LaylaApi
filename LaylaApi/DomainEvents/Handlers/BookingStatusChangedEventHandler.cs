using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;
using static LaylaApi.Models.MainModels.Booking;

namespace LaylaApi.DomainEvents.Handlers
{
    public class BookingStatusChangedEventHandler : IEventHandler<BookingStatusChangedEvent>
    {
        private readonly INotificationService _notification;
        private readonly IEmailService _email;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEventDataProvider<BookingStatusChangedEvent, BookingStatusChangedEventDto> _dataProvider;

        public BookingStatusChangedEventHandler(INotificationService notification, IEmailService email, IStringLocalizer<Notifications> localizer, IEventDataProvider<BookingStatusChangedEvent, BookingStatusChangedEventDto> dataProvider)
        {
            _notification = notification;
            _email = email;
            _localizer = localizer;
            _dataProvider = dataProvider;
        }

        public async Task HandleAsync(BookingStatusChangedEvent @event, CancellationToken ct = default)
        {
            var data = await _dataProvider.GetDataAsync(@event, ct);

            var renter = new NotificationTarget(data.RenterId, data.RenterEmail, data.RenterLang);

            var owner = new NotificationTarget( data.OwnerId, data.OwnerEmail, data.OwnerLang);

            switch (@event.NewStatus)
            {
                case BookingStatus.Accepted:
                    await NotifyAsync(
                        renter,
                        "Booking_Accepted_Title",
                        "Booking_Accepted_Body",
                        data.ApartmentTitle);
                    break;

                case BookingStatus.Confirmed:
                    await NotifyAsync(
                        renter,
                        "Booking_Confirmed_Title",
                        "Booking_Confirmed_Body",
                        data.ApartmentTitle);
                    break;

                case BookingStatus.Rejected:
                    await NotifyAsync(
                        renter,
                        "Booking_Rejected_Title",
                        "Booking_Rejected_Body",
                        data.ApartmentTitle);
                    break;

                case BookingStatus.CancelledByOwner:
                    await NotifyAsync(
                        renter,
                        "Booking_Cancelled_By_Owner_Title",
                        "Booking_Cancelled_By_Owner_Body",
                        data.ApartmentTitle);
                    break;

                case BookingStatus.CancelledByRenter:
                    await NotifyAsync(
                        owner,
                        "Booking_Cancelled_By_Renter_Title",
                        "Booking_Cancelled_ByRenter_Body",
                        data.ApartmentTitle);
                    break;

                case BookingStatus.Completed:
                    await NotifyAsync(
                        owner,
                        "Booking_Completed_Title",
                        "Booking_Completed_Body",
                        data.ApartmentTitle);

                    await NotifyAsync(
                        renter,
                        "Booking_Completed_Title",
                        "Booking_Completed_Body",
                        data.ApartmentTitle);
                    break;
            }

        }

        private sealed record NotificationTarget( int UserId, string Email, string Language);

        private async Task NotifyAsync(NotificationTarget target, string titleKey, string bodyKey, params object[] bodyArgs)
        {
            using (LocalizationHelper.UseCulture(target.Language ?? "en"))
            {
                var title = _localizer[titleKey];
                var body = _localizer[bodyKey, bodyArgs];

                await _email.SendEmailAsync(target.Email, title, body);
                await _notification.SendToUserAsync(target.UserId, title, body);
            }
        }
    }
}
