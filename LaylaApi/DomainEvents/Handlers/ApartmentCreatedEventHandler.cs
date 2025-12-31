using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ApartmentCreatedEventHandler : IEventHandler<ApartmentCreatedEvent>
    {

        private readonly INotificationService _notificationService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEmailService _emailService;
        public ApartmentCreatedEventHandler(INotificationService notificationService, IStringLocalizer<Notifications> localizer,IEmailService emailService)
        {
            _notificationService = notificationService;
             _emailService = emailService;
            _localizer = localizer;
            
        }

        public async Task HandleAsync(ApartmentCreatedEvent @event, CancellationToken ct = default)
        {
            using (LocalizationHelper.UseCulture(@event.Apartment.Owner!.Lang))
            {
                var title = _localizer["ApartmentCreated_Title"];
                var body = _localizer["ApartmentCreated_Body"] + @event.Apartment.Title;

                await _notificationService.SendToUserAsync(@event.Apartment.OwnerId, title, body);
                await _emailService.SendEmailAsync(@event.Apartment.Owner.Email, title, body);
            }
        }
    }
}
