using CloudinaryDotNet;
using LaylaApi.Options;
using LaylaApi.Services.MediaStorageProviderServices.Implementation;
using LaylaApi.Services.MediaStorageProviderServices.Interfaces;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace LaylaApi.Services.MediaStorageProviderServices.ServiceCollectionExtensions
{
    public static class CollectionExtensions
    {
        public static IServiceCollection AddCloudinaryProvider(this IServiceCollection services)
        {
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<CloudinaryOptions>>().Value;

                if (string.IsNullOrWhiteSpace(options.CloudName) ||
                    string.IsNullOrWhiteSpace(options.ApiKey) ||
                    string.IsNullOrWhiteSpace(options.ApiSecret))
                    throw new InvalidOperationException("Cloudinary configuration is invalid.");

                var account = new Account(
                    options.CloudName,
                    options.ApiKey,
                    options.ApiSecret
                );

                return new Cloudinary(account)
                {
                    Api = { Secure = true }
                };
            });
            services.AddScoped<IStorageProvider, CloudinaryStorageProvider>();
            return services;
        }
    }
}
