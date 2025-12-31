using LaylaApi.Services.AdminDashboardService.Interfaces;
using LaylaApi.Services.AdminDashboardService.Implementations;

namespace LaylaApi.Services.AdminDashboardService.ServiceCollectionExtensions
{
    public static class AdminDashBoardServiceCollectionExtensions
    {
        public static IServiceCollection AddAdminDashBoardService(this IServiceCollection services) 
        {
            services.AddScoped<IDashboardService, DashboardService>();
            return services;
        }

    }
}
