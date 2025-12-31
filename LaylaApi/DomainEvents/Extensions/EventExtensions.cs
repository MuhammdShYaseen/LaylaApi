using LaylaApi.DomainEvents.Domain.Dispatcher;
using LaylaApi.DomainEvents.Domain.Events;
using LaylaApi.DomainEvents.Handlers;
using LaylaApi.DomainEvents.Infrastructure.Background;

namespace LaylaApi.DomainEvents.Extensions
{
    public static class EventExtensions
    {
        public static IServiceCollection AddDomainEvents(this IServiceCollection services)
        {
            // Dispatcher (singleton Channel wrapper)
            services.AddSingleton<InMemoryEventDispatcher>();
            services.AddSingleton<IEventDispatcher>(sp => sp.GetRequiredService<InMemoryEventDispatcher>());

            // Hosted service that consumes the dispatcher.Reader and invokes handlers
            services.AddHostedService<EventQueueHostedService>();

            // Register handlers
            services.AddScoped<IEventHandler<ApartmentCreatedEvent>, ApartmentCreatedEventHandler>();
            services.AddScoped<IEventHandler<BookingCreatedEvent>, BookingCreatedHandler>();
            services.AddScoped<IEventHandler<PaymentCompletedEvent>, PaymentCompletedHandler>();
            services.AddScoped<IEventHandler<ContractCreatedEvent>, ContractCreatedHandler>();
            services.AddScoped<IEventHandler<ContractSignedEvent>, ContractSignedHandler>();
            services.AddScoped<IEventHandler<MediaUploadedEvent>, MediaUploadedHandler>();
            services.AddScoped<IEventHandler<ReportCreatedEvent>, ReportCreatedHandler>();
            services.AddScoped<IEventHandler<ReviewCreatedEvent>, ReviewCreatedHandler>();
            services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredHandler>();
            services.AddScoped<IEventHandler<PasswordResetRequestedEvent>, PasswordResetHandler>();

            return services;
        }
    }
}
