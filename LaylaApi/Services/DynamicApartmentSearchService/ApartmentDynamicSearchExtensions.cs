using LaylaApi.Services.DynamicApartmentSearchService.BuilderServices;

namespace LaylaApi.Services.DynamicApartmentSearchService
{
    public static class ApartmentDynamicSearchExtensions
    {
        public static IServiceCollection AddApartmentDynamicSearch(this IServiceCollection services)
        {
            services.AddScoped<IApartmentFilterBuilder, ApartmentFilterBuilder>();
            services.AddScoped<IApartmentSearchService, ApartmentSearchService>();
            return services;
        }
    }
}
