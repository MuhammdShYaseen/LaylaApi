namespace LaylaApi.Services.SoftDeleteService
{
    public static class SoftDeleteServiceExtensions
    {
        public static IServiceCollection AddSoftDeleteService(this IServiceCollection services) 
        { 
            services.AddScoped (typeof(ISoftDeleteService<>),typeof(SoftDeleteService<>));
            return services;
        }
    }
}
