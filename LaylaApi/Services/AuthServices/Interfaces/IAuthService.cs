using LaylaApi.Models.DtosModels.AuthDtos;

namespace LaylaApi.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string originIp, CancellationToken ct);
        Task<bool> VerifyEmailAsync(string token, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest request, string originIp, CancellationToken ct);
        Task<bool> SendPasswordResetAsync(string email, CancellationToken ct);
        Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken ct);
    }
}
