namespace LaylaApi.Services.DynamicApartmentSearchService
{
    public static class ApartmentDynamicSearchExtensions
    {
        public static IServiceCollection AddApartmentDynamicSearch(this IServiceCollection services)
        {
            services.AddScoped<IApartmentSearchService, ApartmentSearchService>();
            return services;
        }
    }
}
