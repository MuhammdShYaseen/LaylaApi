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
    public class ContractSignedHandler : IEventHandler<ContractSignedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEventDataProvider<ContractSignedEvent, ContractSignedEventDto> _dataProvider;
        public ContractSignedHandler( INotificationService notificationService, IEmailService emailService, IStringLocalizer<Notifications> localizer, IEventDataProvider<ContractSignedEvent, ContractSignedEventDto> dataProvider)
        {
            _notificationService = notificationService;
            _emailService = emailService;
            _localizer = localizer;
            _dataProvider = dataProvider;
        }

        public async Task HandleAsync(ContractSignedEvent @event, CancellationToken ct = default)
        {
            var data = await _dataProvider.GetDataAsync(@event, ct);

            var signerKey = data.IsOwnerSigner ? "ContractSigned_ByOwner" : "ContractSigned_ByRenter";

            var targetUserId = data.IsOwnerSigner ? data.RenterId : data.OwnerId;

            var targetLang = data.IsOwnerSigner ? data.RenterLang : data.OwnerLang;

            var targetEmail = data.IsOwnerSigner ? data.RenterEmail : data.OwnerEmail;

            using (LocalizationHelper.UseCulture(targetLang))
            {
                var title = _localizer["ContractSigned_Title"];
                var body = _localizer[signerKey, data.BookingId];

                await _notificationService.SendToUserAsync(targetUserId, title, body);

                var emailSubject = _localizer["ContractSigned_Email_Subject"];
                var emailBody = _localizer[signerKey, data.ApartmentTitle];

                await _emailService.SendEmailAsync(targetEmail, emailSubject, emailBody);
            }

            if (data.IsFullySigned)
            {
                using (LocalizationHelper.UseCulture(data.OwnerLang))
                {
                    await _notificationService.SendToUserAsync(data.OwnerId, _localizer["ContractCompleted_Title"], _localizer["ContractCompleted_Owner_Body", data.BookingId]);
                }

                using (LocalizationHelper.UseCulture(data.RenterLang))
                {
                    await _notificationService.SendToUserAsync(data.RenterId, _localizer["ContractCompleted_Title"], _localizer["ContractCompleted_Renter_Body", data.ApartmentTitle]);
                }
            }
        }
    }
}
