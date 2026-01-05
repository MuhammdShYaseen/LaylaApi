
using LaylaApi.Options;
using LaylaApi.Services.ChatServices.Interfaces;
using Microsoft.Extensions.Options;
using static LaylaApi.Models.MainModels.Message;
using System;
using LaylaApi.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace LaylaApi.Services.BackgroundServices
{
    public class VoiceCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ChatOptions _options;
        public VoiceCleanupService(IServiceScopeFactory scopeFactory, IOptions<ChatOptions> options)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.VoiceMessageRetentionHours == null) return;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<LaylaContext>();

                var storage = scope.ServiceProvider.GetRequiredService<IVoiceStorageService>();

                var threshold = DateTime.UtcNow.AddHours(-_options.VoiceMessageRetentionHours.Value);

                var oldMessages = await db.Messages.Where(x => x.Type == MessageType.Voice && x.SentAt < threshold && x.VoiceFilePath != null).ToListAsync();

                foreach (var msg in oldMessages)
                {
                    await storage.DeleteAsync(msg.VoiceFilePath!);
                    msg.VoiceFilePath = null;
                }
                await db.SaveChangesAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
