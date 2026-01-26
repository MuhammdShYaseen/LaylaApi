using LaylaApi.Services.EventsDataProviderServices.Interfaces;
using System.Reflection;

namespace LaylaApi.Services.EventsDataProviderServices.ServiceCollectionExtensions
{
    public static class EventDataProviderExtensions
    {
        public static IServiceCollection AddEventDataProviders(this IServiceCollection services, Assembly assembly)
        {
            services.Scan(scan => scan
                .FromAssemblies(assembly)
                .AddClasses(classes =>
                    classes.AssignableTo(typeof(IEventDataProvider<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
