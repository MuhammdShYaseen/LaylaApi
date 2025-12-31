using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using LaylaApi.Services.FirebaseServices.Implementations;
using LaylaApi.Services.FirebaseServices.Interfaces;

namespace LaylaApi.Services.FirebaseServices.ServiceCollectionExtensions
{
    
    public static class FirebaseServicesCollectionExtensions 
    {
        public static IServiceCollection AddFirebaseServices(this IServiceCollection services) 
        {
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }
        public static void AddFirebaseApp(this WebApplicationBuilder builder)
        {
            var firebaseConfig = builder.Configuration.GetSection("Firebase");
            var credentialsPath = firebaseConfig["CredentialsPath"];

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(credentialsPath)
                });
            }
        }
    }
}
