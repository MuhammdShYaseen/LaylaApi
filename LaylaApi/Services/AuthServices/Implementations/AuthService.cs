using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.MainModels;
using LaylaApi.Services.AuthServices.Interfaces;
using LaylaApi.Services.DataCRUD.Interfaces;
using LaylaApi.Services.LanguageServices;

namespace LaylaApi.Services.AuthServices.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly ISupportedLanguagePolicy _languagePolicy;
        private readonly ITokenService _tokenService;
        public AuthService(IEmailService emailService, IUserService userService, ISupportedLanguagePolicy languagePolicy, ITokenService tokenService)
        {
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

            var user = User.Create(request.FullName,request.Email,request.PhoneNumber,request.Password, passwordHash,request.Lang,_tokenService.GenerateRandomToken(), _languagePolicy);
            await _userService.AddAsync(user);

            var authResponse = await _tokenService.GenerateAuthResponseAsync(user, originIp);
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

            var authResponse = await _tokenService.GenerateAuthResponseAsync(user, originIp);
            return authResponse;
        }

        public async Task<bool> SendPasswordResetAsync(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return false;

           var resetPasswordToken = _tokenService.GenerateRandomToken();
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
    }
}
