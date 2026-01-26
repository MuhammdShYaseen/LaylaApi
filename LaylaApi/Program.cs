
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
            builder.Services.Configure<ChatOptions>(builder.Configuration.GetSection("Chat"));
            builder.AddFirebaseApp();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddModelStateValidationHandler();
            builder.Services.AddCustomRateLimiter();
            builder.Services.AddLaylaContextExtension(builder.Configuration);
            builder.Services.AddDataRepository();
            builder.Services.AddEventDataProviders(typeof(IEvent).Assembly);
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddDomainEvents();
            builder.Services.AddAuthServices();
            builder.Services.AddLocalizationExtension();
            builder.Services.AddDataCRUDServices();
            builder.Services.AddChatServiceExtensions();
            builder.Services.AddAdminDashBoardService();
            builder.Services.AddFirebaseServices();
            builder.Services.AddCustomSwagger();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseCustomSwaggerUI();
            }
            app.UseCorrelationId();
            app.UseErrorHandler();
            app.UseRequestResponseLogging();
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
