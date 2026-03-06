using LaylaApi.Services.ChatServices.Implementations;
using LaylaApi.Services.ChatServices.Interfaces;

namespace LaylaApi.Services.ChatServices.ServiceCollectionExtensions
{
    public static class ChatServiceExtensions
    {
        public static IServiceCollection AddChatServiceExtensions(this IServiceCollection services) 
        {
            services.AddScoped<IConversationReadService, ConversationReadService>();
            services.AddSignalR();
            services.AddScoped<IConversationService, ConversationService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IVoiceStorageService, VoiceStorageService>();
            return services;
        }
    }
}
