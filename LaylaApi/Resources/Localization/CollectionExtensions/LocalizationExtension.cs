using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace LaylaApi.Resources.Localization.CollectionExtensions
{
    public static class LocalizationExtension
    {
        public static IServiceCollection AddLocalizationExtension(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources/Localization";
            });

            services.AddSingleton<IStringLocalizer>(sp =>
            {
                var assembly = typeof(Notifications).Assembly;
                var resourceManager = new ResourceManager("LaylaApi.Resources.Localization.Notifications", assembly);
                var logger = sp.GetRequiredService<ILogger<ResourceManagerStringLocalizer>>();
                var cache = new ResourceNamesCache();
                return new ResourceManagerStringLocalizer(resourceManager, assembly, "LaylaApi.Resources.Localization.Notifications", cache, logger);
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                   new CultureInfo("ar"),
                   new CultureInfo("en")
                };
                options.DefaultRequestCulture = new RequestCulture("en"); // اللغة الافتراضية
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;

                // تحديد مصدر اللغة (Header → Query → Cookie → Default)
                options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });
            return services;
        }   
    } 
}
