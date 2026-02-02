
using LaylaApi.DataAccess;
using LaylaApi.DomainEvents.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;

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
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task CleanupAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<LaylaContext>();

            var threshold = DateTime.UtcNow.AddDays(-RetentionDays);

            var deletedCount = await context
                .Set<Entity>()
                .IgnoreQueryFilters()
                .Where(e =>
                    e.IsDeleted &&
                    e.UpdatedAt <= threshold)
                .ExecuteDeleteAsync(ct);

            _logger.LogInformation(
                "SoftDelete cleanup removed {Count} records",
                deletedCount);
        }
    }
}
