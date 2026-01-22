namespace LaylaApi.DataRepository
{
    public static class RepositoryExtension
    {
        public static IServiceCollection AddDataRepository(this IServiceCollection services) 
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}
