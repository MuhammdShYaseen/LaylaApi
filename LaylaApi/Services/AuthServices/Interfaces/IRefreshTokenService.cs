using LaylaApi.Models.DtosModels.AuthDtos;

namespace LaylaApi.Services.AuthServices.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<AuthResponse?> RefreshTokenAsync(string token, string originIp);
        Task<bool> RevokeRefreshTokenAsync(string token, string originIp);
    }
}
