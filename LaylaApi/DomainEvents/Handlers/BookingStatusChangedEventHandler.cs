using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
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

        public BookingStatusChangedEventHandler(INotificationService notification, IEmailService email, IStringLocalizer<Notifications> localizer)
        {
            _notification = notification;
            _email = email;
            _localizer = localizer;
        }

        public async Task HandleAsync(BookingStatusChangedEvent @event, CancellationToken ct = default)
        {
            switch (@event.NewStatus)
            {
                case BookingStatus.Accepted:
                    await NotifyRenterAccepted(@event);
                    break;

                case BookingStatus.Confirmed:
                    await NotifyRenterConfirmed(@event);
                    break;

                case BookingStatus.Rejected:
                    await NotifyRenterRejected(@event);
                    break;

                case BookingStatus.CancelledByRenter:
                    await NotifyOwnerCancelledByRenter(@event);
                    break;

                case BookingStatus.CancelledByOwner:
                    await NotifyRenterCancelledByOwner(@event);
                    break;
                case BookingStatus.Completed:
                    await NotifyRenterAndOwnerCompleted(@event);
                    break;



            }
        }

        private async Task NotifyRenterAccepted(BookingStatusChangedEvent e)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture(e.RenterLang ?? "en")) 
            {
                var title = _localizer["Booking_Accepted_Title"];
                var body = _localizer["Booking_Accepted_Body", e.ApartmentTitle];
                await NotifyUser(e.RenterId, e.RenterEmail, title, body);
            }   
        }

        private async Task NotifyRenterRejected(BookingStatusChangedEvent e)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture(e.RenterLang ?? "en"))
            {
                var title = _localizer["Booking_Rejected_Title"];
                var body = _localizer["Booking_Rejected_Body", e.ApartmentTitle];

                await NotifyUser(e.RenterId, e.RenterEmail, title, body);
            }
            
        }

        private async Task NotifyRenterConfirmed(BookingStatusChangedEvent e)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture(e.RenterLang ?? "en"))
            {
                var title = _localizer["Booking_Confirmed_Title"];
                var body = _localizer["Booking_Confirmed_Body", e.ApartmentTitle];

                await NotifyUser(e.RenterId, e.RenterEmail, title, body);
            }
               
        }


        private async Task NotifyRenterCancelledByOwner(BookingStatusChangedEvent e)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture(e.RenterLang ?? "en"))
            {
                var title = _localizer["Booking_Cancelled_By_Owner_Title"];
                var body = _localizer["Booking_Cancelled_By_Owner_Body", e.ApartmentTitle];

                await NotifyUser(e.RenterId, e.RenterEmail, title, body);
            }
                
        }


        private async Task NotifyOwnerCancelledByRenter(BookingStatusChangedEvent e)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture(e.OwnerLang ?? "en"))
            {
                var title = _localizer["Booking_Cancelled_By_Renter_Title"];
                var body = _localizer["Booking_Cancelled_ByRenter_Body", e.ApartmentTitle];

                await NotifyUser(e.OwnerId, e.OwnerEmail, title, body);
            }
                
        }

        private async Task NotifyRenterAndOwnerCompleted(BookingStatusChangedEvent e)
        {
            var title = _localizer["Booking_Completed_Title"];
            var body = _localizer["Booking_Completed_Body", e.ApartmentTitle];

            using (Helper.Localization.LocalizationHelper.UseCulture(e.OwnerLang ?? "en"))
            {
                await NotifyUser(e.OwnerId, e.OwnerEmail, title, body);
            }

            using (Helper.Localization.LocalizationHelper.UseCulture(e.RenterLang ?? "en"))
            {
                await NotifyUser(e.RenterId, e.RenterEmail, title, body);
            }
        }

        private async Task NotifyUser(int userId, string emailAddress,  string title, string body)
        {
            await _email.SendEmailAsync(emailAddress, title, body);
            await _notification.SendToUserAsync(userId, title, body);
        }


    }
}
