using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Helper.Localization;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using Microsoft.Extensions.Localization;


namespace LaylaApi.DomainEvents.Handlers
{
    public class UserRegisteredHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEventDataProvider<UserRegisteredEvent, UserRegisteredEventDto> _dataProvider;

        public UserRegisteredHandler(IEmailService emailService, IStringLocalizer<Notifications> stringLocalizer, IEventDataProvider<UserRegisteredEvent, UserRegisteredEventDto> dataProvider)
        {
            _emailService = emailService;
            _localizer = stringLocalizer;
            _dataProvider = dataProvider;
        }

        public async Task HandleAsync(UserRegisteredEvent @event, CancellationToken ct = default)
        {
            var data = await _dataProvider.GetDataAsync(@event, ct);
            using (LocalizationHelper.UseCulture(data.Lang))
            {
                var subject = _localizer["UserRegistered_Email_Subject"];

                var body = _localizer["UserRegistered_Email_Body", data.FullName, data.VerificationUrl];

                await _emailService.SendEmailAsync(data.Email, subject, body);
            }
        }
    }
}
