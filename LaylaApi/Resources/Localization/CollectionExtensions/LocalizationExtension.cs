using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace LaylaApi.Resources.Localization.CollectionExtensions
{
    public static class LocalizationExtension
    {
        public static IServiceCollection AddLocalizationExtension(this IServiceCollection services)
        {
            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
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

                // تحديد مصدر اللغة (Header → Query → Cookie → Default)
                options.RequestCultureProviders.Insert(0, new AcceptLanguageHeaderRequestCultureProvider());
            });
            return services;
        }   
    } 
}
