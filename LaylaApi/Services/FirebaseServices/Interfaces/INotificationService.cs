namespace LaylaApi.Services.FirebaseServices.Interfaces
{
    public interface INotificationService
    {
        Task SendToTokenAsync(string token, string title, string body);
        Task SendToUserAsync(int userId, string title, string body);
        Task SendToAllAsync(string title, string body);
        Task SendAdminAsync (string title, string body);
        Task SendToTopicAsync(string topic, string title, string body);
    }
}
