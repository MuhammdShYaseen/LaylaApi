using LaylaApi.DataRepository;
using LaylaApi.Helper.AuthHelper;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRepository<User> _repository;
        public TokenService(IOptions<JwtSettings> jwtOptions, IRepository<User> repository) 
        { 
            _jwtSettings = jwtOptions.Value;
            _repository = repository;
        }

        public string GenerateRandomToken()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        public RefreshToken CreateRefreshToken(string ipAddress, int userId)
        {
            return new RefreshToken
            {
                Token = GenerateRandomToken(),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedByIp = ipAddress,
                UserId = userId
            };
        }

        public async Task<AuthResponse> GenerateAuthResponseAsync(User user, string originIp)
        {
            var jwt = GenerateJwtToken(user);

            // أنشئ refresh token و خزنه
            var refreshToken = CreateRefreshToken(originIp, user.Id);
            user.RefreshToken ??= new List<RefreshToken>();
            user.RefreshToken.Add(refreshToken);
            _repository.Update(user);
            await _repository.SaveChangesAsync();

            return new AuthResponse
            {
                JwtToken = jwt.Token,
                RefreshToken = refreshToken.Token,
                ExpiresInSeconds = _jwtSettings.TokenExpirationMinutes * 60,
                UserId = user.Id,
                Email = user.Email!.Value
            };
        }


        private (string Token, DateTime Expires) GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email !.Value),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return (token, tokenDescriptor.Expires!.Value);
        }

        

        
    }
}
