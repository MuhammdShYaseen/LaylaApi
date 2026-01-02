using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Resources.Localization;
using LaylaApi.Services.FirebaseServices.Interfaces;
using Microsoft.Extensions.Localization;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReportCreatedHandler : IEventHandler<ReportCreatedEvent>
    {
        private readonly INotificationService _notificationService;
        private readonly IStringLocalizer<Notifications> _localizer;
        public ReportCreatedHandler(LaylaContext context, INotificationService notificationService, IStringLocalizer<Notifications> localizer)
        {
            _notificationService = notificationService;
            _localizer = localizer;
        }

        public async Task HandleAsync(ReportCreatedEvent @event, CancellationToken ct = default)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture("en"))
            {
                var apartmentId = @event._report.ApartmentId;
                var reporterId = @event._report.ReporterId;
                var title = _localizer["Report_New"];
                var body = _localizer["Report_Body", apartmentId, reporterId];
                await _notificationService.SendAdminAsync(title, body);
            }
             
           
        }
    }
}
