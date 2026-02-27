using LaylaApi.DataRepository;
using LaylaApi.Helper.AuthHelper;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRepository<RefreshToken> _repository;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        public RefreshTokenService(IRepository<RefreshToken> repository, ITokenService tokenService, IOptions<JwtSettings> jwtSettings) 
        { 
            _repository = repository;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<AuthResponse?> RefreshTokenAsync(string token, string originIp, CancellationToken ct)
        {
            var refreshToken = await _repository.Query().Include(r => r.User).FirstOrDefaultAsync(rt => rt.Token == token, ct);
            if (refreshToken == null || !refreshToken.IsActive) return null;

            // استبدال التوكن القديم بآخر جديد (rotate)
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = originIp;

            var newRefreshToken = _tokenService.CreateRefreshToken(originIp, refreshToken.UserId);
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await _repository.AddAsync(newRefreshToken);
            await _repository.SaveChangesAsync();

            // اصدار JWT جديد
            var jwt = _tokenService.GenerateJwtToken(refreshToken.User!);
            return new AuthResponse
            {
                JwtToken = jwt.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresInSeconds = _jwtSettings.TokenExpirationMinutes * 60,
                UserId = refreshToken.User!.Id,
                Email = refreshToken.User.Email!.Value
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string originIp, CancellationToken ct)
        {
            var refreshToken = await _repository.Query().FirstOrDefaultAsync(rt => rt.Token == token, ct);
            if (refreshToken == null || !refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = originIp;
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
