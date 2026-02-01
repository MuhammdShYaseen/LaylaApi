using LaylaApi.Models.DtosModels.AuthDtos;

namespace LaylaApi.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, string originIp);
        Task<bool> VerifyEmailAsync(string token);
        Task<AuthResponse> LoginAsync(LoginRequest request, string originIp);
        Task<bool> SendPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
