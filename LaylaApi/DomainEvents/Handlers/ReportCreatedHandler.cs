using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Models.DtosModels.EventDtos;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReportCreatedHandler : IEventHandler<ReportCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IStringLocalizer<Notifications> _localizer;
        private readonly IEventDataProvider<ReportCreatedEvent, ReportCreatedEventDto> _dataProvider;
        public ReportCreatedHandler(LaylaContext context, INotificationService notificationService, IStringLocalizer<Notifications> localizer, IEventDataProvider<ReportCreatedEvent, ReportCreatedEventDto> dataProvider)
        {
            _notificationService = notificationService;
            _localizer = localizer;
            _dataProvider = dataProvider;
        }

        public async Task HandleAsync(ReportCreatedEvent @event, CancellationToken ct = default)
        {
            var data = await _dataProvider.GetDataAsync(@event, ct);
            using (Helper.Localization.LocalizationHelper.UseCulture("en"))
            {
                var apartmentId = data.ApartmentId;
                var reporterId = data.ReporterId;
                var title = _localizer["Report_New"];
                var body = _localizer["Report_Body", apartmentId, reporterId];
                await _notificationService.SendAdminAsync(title, body);
            }
             
           
        }
    }
}
