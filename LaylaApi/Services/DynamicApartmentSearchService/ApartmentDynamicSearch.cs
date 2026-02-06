namespace LaylaApi.Services.DynamicApartmentSearchService
{
    public static class ApartmentDynamicSearch
    {
        public static IServiceCollection AddApartmentDynamicSearch(this IServiceCollection services)
        {
            services.AddScoped<IApartmentSearchService, ApartmentSearchService>();
            return services;
        }
    }
}
