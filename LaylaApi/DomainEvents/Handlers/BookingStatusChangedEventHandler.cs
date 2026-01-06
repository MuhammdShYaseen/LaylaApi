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
                    await NotifyRenterApproved(@event);
                    break;

                case BookingStatus.Rejected:
                    await NotifyRenterRejected(@event);
                    break;
            }
        }

        private async Task NotifyRenterApproved(BookingStatusChangedEvent e)
        {
            var title = _localizer["BookingApprovedTitle"];
            var body = _localizer["BookingApprovedBody", e.ApartmentTitle];

            await _notification.SendToUserAsync(e.RenterId, title, body);
            await _email.SendEmailAsync(e.RenterEmail, title, body);
        }

        private async Task NotifyRenterRejected(BookingStatusChangedEvent e)
        {
            var title = _localizer["BookingRejectedTitle"];
            var body = _localizer["BookingRejectedBody", e.ApartmentTitle];

            await _notification.SendToUserAsync(e.RenterId, title, body);
            await _email.SendEmailAsync(e.RenterEmail, title, body);
        }
    }
}
