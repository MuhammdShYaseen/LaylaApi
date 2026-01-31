using LaylaApi.Options;

namespace LaylaApi.Services.LanguageServices
{
    public static class SupportedLanguageCollectionExtensions
    {
        public static IServiceCollection AddSupportedLanguageService(this IServiceCollection services) 
        {
            services.AddSingleton<ISupportedLanguagePolicy, SupportedLanguagePolicy>();
           
            return services;
        }
    }
}
