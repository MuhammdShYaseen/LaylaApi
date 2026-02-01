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
using LaylaApi.Services.LanguageServices;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;
        private readonly ISupportedLanguagePolicy _languagePolicy;
        private readonly ITokenService _tokenService;
        public AuthService(IOptions<JwtSettings> jwtOptions, IEmailService emailService, IUserService userService, ISupportedLanguagePolicy languagePolicy, ITokenService tokenService)
        {
            _jwtSettings = jwtOptions.Value;
            _userService = userService;
            _languagePolicy = languagePolicy;
            _tokenService = tokenService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string originIp)
        {

            // تحقق وجود المستخدم
            if (await _userService.ExistsByEmailAsync(request.Email))
                throw new BadHttpRequestException("Email already registered");

            if (await _userService.ExistsByPhoneAsync(request.PhoneNumber))
                throw new BadHttpRequestException("Phone already registered");


            // تجزئة كلمة المرور (BCrypt)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = User.Create(request.FullName,request.Email,request.PhoneNumber,request.Password, passwordHash,request.Lang, GenerateRandomToken(), _languagePolicy);
            await _userService.AddAsync(user);

            var authResponse = await GenerateAuthResponseAsync(user, originIp);
            return authResponse;
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _userService.GetByEmailTokenAsync(token);
            if (user == null) return false;
            if (user.EmailVerificationTokenExpires == null || user.EmailVerificationTokenExpires < DateTime.UtcNow) return false;

            user.ConfirmEmail();
            await _userService.SaveAsync();
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

       

        public async Task<bool> SendPasswordResetAsync(string email)
        {
            var user = await _userService.GetByEmailAsync(email.ToLower());
            if (user == null) return false;

           var resetPasswordToken = GenerateRandomToken();
           var resetPasswordTokenExpires = DateTime.UtcNow.AddHours(1);

            
            user.ForgotPassword(resetPasswordToken, resetPasswordTokenExpires);

            await _userService.SaveAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userService.GetByResetTokenAsync(token);

            if (user == null) return false;

            user.ResetPassword(BCrypt.Net.BCrypt.HashPassword(newPassword));

            await _userService.SaveAsync();
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
