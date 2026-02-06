using Microsoft.EntityFrameworkCore;

namespace LaylaApi.DataAccess.ServiceCollectionExtensions
{
    public static class LaylaContextExtensions
    {
        public static IServiceCollection AddLaylaContextExtension(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<LaylaContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
             sqlOptions => sqlOptions.UseNetTopologySuite()));
            return services;
        }
    }
}
