using LaylaApi.DataAccess;
using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.MainModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Helper.AuthHelper;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Options;
using Google.Protobuf.WellKnownTypes;
using LaylaApi.Services.LanguageServices;
using LaylaApi.ValueObjects.UserValueObject;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly LaylaContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;
        private readonly ISupportedLanguagePolicy _languagePolicy;
        public AuthService(LaylaContext context, IOptions<JwtSettings> jwtOptions, IEmailService emailService, IUserService userService, ISupportedLanguagePolicy languagePolicy)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
            _userService = userService;
            _languagePolicy = languagePolicy;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string originIp)
        {
           
            // تحقق وجود المستخدم
            if (await _context.Users.AnyAsync(u => u.Email!.Value == request.Email))
                throw new  BadHttpRequestException("Email is already registered.");

            if (await _context.Users.AnyAsync(u => u.PhoneNumber!.Value == request.PhoneNumber))
                throw new BadHttpRequestException("Phone number is already registered.");


            // تجزئة كلمة المرور (BCrypt)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = User.Create(request, passwordHash, GenerateRandomToken(), _languagePolicy);
            await _userService.AddAsync(user);

            var authResponse = await GenerateAuthResponseAsync(user, originIp);
            return authResponse;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailVerificationToken == token);
            if (user == null) return false;
            if (user.EmailVerificationTokenExpires == null || user.EmailVerificationTokenExpires < DateTime.UtcNow) return false;

            user.ConfirmEmail();
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string originIp)
        {
            //var user = await _context.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Email == request.Email);
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null) throw new 
                    BadHttpRequestException("Invalid credentials.");

            // تحقق من كلمة المرور
            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!validPassword) throw new 
                    BadHttpRequestException("Invalid credentials.");

            // (اختياري) تحقق من EmailConfirmed
            if (!user.EmailConfirmed) throw new 
                    BadHttpRequestException("Email not confirmed.");

            var authResponse = await GenerateAuthResponseAsync(user, originIp);
            return authResponse;
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string token, string originIp)
        {
            var refreshToken = await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken == null || !refreshToken.IsActive) return null;

            // استبدال التوكن القديم بآخر جديد (rotate)
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = originIp;

            var newRefreshToken = CreateRefreshToken(originIp, refreshToken.UserId);
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            // اصدار JWT جديد
            var jwt = GenerateJwtToken(refreshToken.User!);
            return new AuthResponse
            {
                JwtToken = jwt.Token,
                RefreshToken = newRefreshToken.Token,
                ExpiresInSeconds = _jwtSettings.TokenExpirationMinutes * 60,
                UserId = refreshToken.User!.Id,
                Email = refreshToken.User.Email!.Value
            };
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string originIp)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            if (refreshToken == null || !refreshToken.IsActive) return false;

            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = originIp;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SendPasswordResetAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email!.Value == email);
            if (user == null) return false;

           var resetPasswordToken = GenerateRandomToken();
           var resetPasswordTokenExpires = DateTime.UtcNow.AddHours(1);

            
            user.ForgotPassword(resetPasswordToken, resetPasswordTokenExpires);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.ResetPasswordToken == token &&
                u.ResetPasswordTokenExpires > DateTime.UtcNow);

            if (user == null) return false;

            user.ResetPassword(BCrypt.Net.BCrypt.HashPassword(newPassword));

            await _context.SaveChangesAsync();
            return true;
        }

        #region Helpers

        private string GenerateRandomToken()
            => Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

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

        private RefreshToken CreateRefreshToken(string ipAddress, int userId)
        {
            return new RefreshToken
            {
                Token = GenerateRandomToken(),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedByIp = ipAddress,
                UserId = userId
            };
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(User user, string originIp)
        {
            var jwt = GenerateJwtToken(user);

            // أنشئ refresh token و خزنه
            var refreshToken = CreateRefreshToken(originIp, user.Id);
            user.RefreshToken ??= new List<RefreshToken>();
            user.RefreshToken.Add(refreshToken);

            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                JwtToken = jwt.Token,
                RefreshToken = refreshToken.Token,
                ExpiresInSeconds = _jwtSettings.TokenExpirationMinutes * 60,
                UserId = user.Id,
                Email = user.Email!.Value
            };
        }
        #endregion
    }
}
