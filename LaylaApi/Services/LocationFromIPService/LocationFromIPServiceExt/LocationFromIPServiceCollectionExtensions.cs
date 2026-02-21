using LaylaApi.Services.LocationFromIPService.Implementations;
using LaylaApi.Services.LocationFromIPService.Interfaces;

namespace LaylaApi.Services.LocationFromIPService.LocationFromIPServiceExt
{
    public static class LocationFromIPServiceCollectionExtensions
    {
        public static IServiceCollection AddLocationFromApi(this IServiceCollection services) 
        {
            
            services.AddHttpClient();
            services.AddScoped<ILocationFromIPExternalService, LocationFromIPExternalService>();
            return services;
        }
    }
}
