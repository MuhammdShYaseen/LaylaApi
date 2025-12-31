using Serilog;

namespace LaylaApi.Logging
{
    public static class SerilogConfiguration
    {
        public static void Configure(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, config) =>
            {
                config
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 14,   // ⏱ عمر اللوغز
                        fileSizeLimitBytes: 10_000_000, // 10MB
                        rollOnFileSizeLimit: true,
                        shared: true
                    ).WriteTo.Seq(context.Configuration["Seq:Endpoint"]!);
                    
                
            });
        }
    }
}
