using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Services.FirebaseServices.Interfaces;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReportCreatedHandler : IEventHandler<ReportCreatedEvent>
    {
        private readonly LaylaContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReportCreatedHandler> _logger;

        public ReportCreatedHandler(LaylaContext context, INotificationService notificationService, ILogger<ReportCreatedHandler> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(ReportCreatedEvent @event, CancellationToken ct = default)
        {
            using (Helper.Localization.LocalizationHelper.UseCulture("en"))
            {
                var apartmentId = @event._report.ApartmentId;
                var reporterId = @event._report.ReporterId;
                await _notificationService.SendAdminAsync("تبليغ جديد", $"تم إنشاء تبليغ جديد للشقة رقم {apartmentId} من المستخدم {reporterId}");
            }
             
           
        }
    }
}
