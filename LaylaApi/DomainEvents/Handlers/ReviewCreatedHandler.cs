using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Services.FirebaseServices.Interfaces;

namespace LaylaApi.DomainEvents.Handlers
{
    public class ReviewCreatedHandler : IEventHandler<ReviewCreatedEvent>
    {
        private readonly LaylaContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReviewCreatedHandler> _logger;

        public ReviewCreatedHandler(LaylaContext context, INotificationService notificationService, ILogger<ReviewCreatedHandler> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task HandleAsync(ReviewCreatedEvent @event, CancellationToken ct = default)
        {
            var apartment = await _context.Apartments.FindAsync(@event.ApartmentId);

            if (apartment == null)
            {
                _logger.LogWarning("Apartment not found for review {Id}", @event.ReviewId);
                return;
            }

            try
            {
                await _notificationService.SendToUserAsync(apartment.OwnerId, "تقييم جديد", $"تم إضافة تقييم جديد لشقتك بدرجة {@event.Rating}.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to notify owner for review {Id}", @event.ReviewId);
            }
        }
    }
}
