namespace LaylaApi.Services.LocalizationServieces.MessagesLocalizerService
{
    public static class MessagesLocalizerExtensions
    {
        public static IServiceCollection AddMessagesLocalizerExtensions(this IServiceCollection services)
        {
            services.AddSingleton<IMessagesLocalizer, MessagesLocalizer>();
            return services;
        }
    }
}
