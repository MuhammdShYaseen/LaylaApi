using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ContractCreatedHandler : IEventHandler<ContractCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;

        public ContractCreatedHandler(INotificationService notificationService, IEmailService emailService,IStringLocalizer<Notifications> notificationLocalizer)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _localizer = notificationLocalizer;
        }

        public async Task HandleAsync(ContractCreatedEvent @event, CancellationToken ct = default)
        {
           
            var booking = @event.Contract.Booking;
            var apartment = booking!.Apartment!;
            var renter = booking.User!;
            var renterId = renter.Id;
            var renterEmail = renter.Email;
            var ownerId = apartment.OwnerId;
            var ownerLanguage = apartment.Owner!.Lang;
            var renterLanguage = booking.User!.Lang;
            var apartmentTitle = apartment.Title;
            var bookingId = booking.Id;
            var ownerEmail = apartment.Owner.Email;


            // 🔔 إشعار + إيميل المالك
            using (LocalizationHelper.UseCulture(ownerLanguage))
            {
                var title = _localizer["ContractCreated_Owner_Title"];
                var body = _localizer[ "ContractCreated_Owner_Body", bookingId, apartmentTitle];

                await _notificationService.SendToUserAsync(ownerId, title, body);

                var emailSubject = _localizer["ContractCreated_Owner_Title"];
                var emailBody = _localizer["ContractCreated_Owner_Body", bookingId, apartmentTitle];

                await _emailService.SendEmailAsync(ownerEmail, emailSubject, emailBody);
            }

            // 🔔 إشعار + إيميل المستأجر
            using (LocalizationHelper.UseCulture(renterLanguage))
            {
                var title = _localizer["ContractCreated_Renter_Title"];
                var body = _localizer["ContractCreated_Renter_Body", apartmentTitle];

                await _notificationService.SendToUserAsync(renterId, title, body);

                var emailSubject = _localizer["ContractCreated_Renter_Title"];
                var emailBody = _localizer["ContractCreated_Renter_Body", apartmentTitle];

                await _emailService.SendEmailAsync(renterEmail, emailSubject, emailBody);
            }
        }
    }
}
