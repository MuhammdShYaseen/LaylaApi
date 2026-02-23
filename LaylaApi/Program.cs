
using LaylaApi.Services.AuthServices.ServiceCollectionExtensions;
using LaylaApi.Services.DataCRUD.ServiceCollectionExtensions;
using LaylaApi.Services.FirebaseServices.ServiceCollectionExtensions;
using LaylaApi.Middleware.ErrorHandler;
using LaylaApi.Middleware.RateLimiter;
using LaylaApi.Middleware.SwaggerEx;
using LaylaApi.DataAccess.ServiceCollectionExtensions;
using LaylaApi.Services.AdminDashboardService.ServiceCollectionExtensions;
using LaylaApi.DomainEvents.Extensions;
using LaylaApi.Resources.Localization.CollectionExtensions;
using LaylaApi.Helper.AuthHelper;
using LaylaApi.Options;
using LaylaApi.Services.ChatServices.ServiceCollectionExtensions;
using LaylaApi.SignalR_Hubs;
using LaylaApi.DataRepository;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.Services.EventsDataProviderServices.ServiceCollectionExtensions;
using LaylaApi.Services.LanguageServices;
using LaylaApi.Services.BackgroundServices;
using LaylaApi.Services.MediaStorageProviderServices.ServiceCollectionExtensions;
using LaylaApi.Services.DynamicApartmentSearchService;
using LaylaApi.DataAccess;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using LaylaApi.Services.LocationFromIPService.LocationFromIPServiceExt;
namespace LaylaApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            Logging.SerilogConfiguration.Configure(builder);
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
            builder.Services.Configure<FrontendOptions>(builder.Configuration.GetSection("FrontEnd"));
            builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));
            builder.Services.Configure<LanguageOptions>(builder.Configuration.GetSection("AppSetting"));
            builder.Services.AddSingleton<GeometryFactory>(_ => NtsGeometryServices.Instance.CreateGeometryFactory(4326));
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
            builder.Services.AddSupportedLanguageService();
            builder.Services.Configure<ChatOptions>(builder.Configuration.GetSection("Chat"));
            builder.AddFirebaseApp();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddModelStateValidationHandler();          
            builder.Services.AddCustomRateLimiter();
            builder.Services.AddLaylaContextExtension(builder.Configuration);
            builder.Services.AddHostedService<SoftDeleteCleanupService>();
            builder.Services.AddHostedService<VoiceCleanupService>();
            builder.Services.AddLocationFromApi();
            builder.Services.AddDataRepository();
            builder.Services.AddCloudinaryProvider();
            builder.Services.AddEventDataProviders(typeof(IEvent).Assembly);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDomainEvents();
            builder.Services.AddAuthServices();
            builder.Services.AddLocalizationExtension();
            builder.Services.AddDataCRUDServices();
            builder.Services.AddApartmentDynamicSearch();
            builder.Services.AddChatServiceExtensions();
            builder.Services.AddAdminDashBoardService();
            builder.Services.AddFirebaseServices();
            builder.Services.AddCustomSwagger();

            var app = builder.Build();
            

            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LaylaContext>();
                db.Database.GetPendingMigrations();
                db.Database.Migrate();

                _ = db.Users.AsNoTracking().Take(1).ToList();
                _ = db.Apartments.AsNoTracking().Take(1).ToList();
                _ = db.Bookings.AsNoTracking().Take(1).ToList();

                app.UseSwagger();
                app.UseCustomSwaggerUI();
            }
            app.UseCorrelationId();
            app.UseErrorHandler();
            app.UseRequestResponseLogging();
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapHub<ChatHub>("/Hubs/chat").DisableRateLimiting();
            app.MapControllers();
            app.Run();
        }
    }
}
