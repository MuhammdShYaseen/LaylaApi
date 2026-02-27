using LaylaApi.Models.DtosModels.AuthDtos;

namespace LaylaApi.Services.AuthServices.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<AuthResponse?> RefreshTokenAsync(string token, string originIp, CancellationToken ct);
        Task<bool> RevokeRefreshTokenAsync(string token, string originIp, CancellationToken ct);
    }
}
