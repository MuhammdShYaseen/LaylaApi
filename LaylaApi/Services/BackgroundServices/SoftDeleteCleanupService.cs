using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Common;
using LaylaApi.Models.MainModels;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.BackgroundServices
{
    public class SoftDeleteCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SoftDeleteCleanupService> _logger;

        private const int RetentionDays = 30;

        public SoftDeleteCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<SoftDeleteCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cleanupQueue = CreateCleanupQueue();

            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteCleanupCycleAsync(cleanupQueue, stoppingToken);
                await WaitForNextCycleAsync(stoppingToken);
            }
        }

        private List<CleanupTask> CreateCleanupQueue()
        {
            return new List<CleanupTask>
            {
                 new(nameof(Apartment), ct => CleanupAsync<Apartment>(ct)),
                 new(nameof(Booking), ct => CleanupAsync<Booking>(ct)),
                 new(nameof(Contract), ct => CleanupAsync<Contract>(ct)),
                 new(nameof(MediaFile), ct => CleanupAsync<MediaFile>(ct)),
                 new(nameof(Message), ct => CleanupAsync<Message>(ct)),
                 new(nameof(Payment), ct => CleanupAsync<Payment>(ct)),
                 new(nameof(RefreshToken), ct => CleanupAsync<RefreshToken>(ct)),
                 new(nameof(Report), ct => CleanupAsync<Report>(ct)),
                 new(nameof(Review), ct => CleanupAsync<Review>(ct)),
                 new(nameof(User), ct => CleanupAsync<User>(ct))
            };
        }

        private async Task ExecuteCleanupCycleAsync(List<CleanupTask> cleanupQueue, CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting cleanup cycle");

            foreach (var task in cleanupQueue)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    await task.ExecuteAsync(stoppingToken);
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    _logger.LogError(ex, "Cleanup failed for {EntityType}", task.EntityType);
                    // الاستمرار في معالجة الكيانات الأخرى
                }
            }

            _logger.LogInformation("Cleanup cycle completed");
        }

        private async Task WaitForNextCycleAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogDebug("Waiting for next cleanup cycle (24 hours)");
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Cleanup service stopped during wait period");
                throw;
            }
        }

        // كلاس مساعد
        private record CleanupTask(string EntityType, Func<CancellationToken, Task> ExecuteAsync);

        private async Task CleanupAsync<T>(CancellationToken ct) where T : Entity
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LaylaContext>();

            var threshold = DateTime.UtcNow.AddDays(-RetentionDays);

            await context.Set<T>()
                .IgnoreQueryFilters()
                .Where(e => e.IsDeleted && e.UpdatedAt <= threshold)
                .ExecuteDeleteAsync(ct);
        }
    }
}
