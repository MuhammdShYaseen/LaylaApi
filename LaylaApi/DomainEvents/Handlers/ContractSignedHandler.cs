using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ContractSignedHandler : IEventHandler<ContractSignedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;

        public ContractSignedHandler( INotificationService notificationService, IEmailService emailService, IStringLocalizer<Notifications> localizer)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _localizer = localizer;
        }

        public async Task HandleAsync(ContractSignedEvent @event, CancellationToken ct = default)
        {
           
            var booking = @event.Contract.Booking;
            var apartment = booking!.Apartment!;
            var renter = booking.User;
            var ownerId = apartment.OwnerId;
            var signerKey = @event.IsOwner ? "ContractSigned_ByOwner" : "ContractSigned_ByRenter";
            var targetUserId = @event.IsOwner ? renter!.Id: ownerId;

            var targetLanguage = @event.IsOwner ? renter!.Lang : apartment.Owner!.Lang;

            // 🔔 إشعار الطرف الآخر
            using (LocalizationHelper.UseCulture(targetLanguage))
            {
                var title = _localizer["ContractSigned_Title"];
                var body = _localizer[ signerKey, booking.Id];

                await _notificationService.SendToUserAsync(targetUserId, title, body);

                var emailSubject = _localizer["ContractSigned_Email_Subject"];
                var emailBody = _localizer[ signerKey, apartment.Title];

                var targetEmail = @event.IsOwner ? renter!.Email : apartment.Owner!.Email;

                await _emailService.SendEmailAsync(targetEmail, emailSubject, emailBody);
            }

            // ✅ إذا تم التوقيع من الطرفين
            if (@event.IsFullySigned == true)
            {
                using (LocalizationHelper.UseCulture(apartment.Owner!.Lang))
                {
                    await _notificationService.SendToUserAsync(ownerId, _localizer["ContractCompleted_Title"], _localizer["ContractCompleted_Owner_Body", booking.Id] );
                }

                using (LocalizationHelper.UseCulture(renter!.Lang))
                {
                    await _notificationService.SendToUserAsync(renter.Id, _localizer["ContractCompleted_Title"], _localizer["ContractCompleted_Renter_Body", apartment.Title]);
                }
            }
        }
    }
}
