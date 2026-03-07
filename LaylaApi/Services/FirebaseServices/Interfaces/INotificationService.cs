namespace LaylaApi.Services.FirebaseServices.Interfaces
{
    public interface INotificationService
    {
        Task SendToTokenAsync(string token, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
        Task SendToUserAsync(int userId, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
        Task SendToAllAsync(string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
        Task SendAdminAsync (string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
        Task SendToTopicAsync(string topic, string title, string body, Dictionary<string, string>? data = null, CancellationToken ct = default);
    }
}
