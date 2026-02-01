using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Services.AuthServices.Interfaces
{
    public interface ITokenService
    {
        string GenerateRandomToken();
        RefreshToken CreateRefreshToken(string ipAddress, int userId);
        Task<AuthResponse> GenerateAuthResponseAsync(User user, string originIp);
    }
}
