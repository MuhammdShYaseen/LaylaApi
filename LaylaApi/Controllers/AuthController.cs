using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Models.GenericResponseModels;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IRefreshTokenService _refreshToken;
        public AuthController(IAuthService auth, IRefreshTokenService refreshToken)
        {
            _auth = auth;
            _refreshToken = refreshToken;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _auth.RegisterAsync(request, originIp, ct);

            return Ok(ApiResponse<AuthResponse>.Ok(result));
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token, CancellationToken ct)
        {
            var success = await _auth.VerifyEmailAsync(token, ct);

            if (!success)
                throw new BadHttpRequestException("Invalid or expired token.");

            return Ok(ApiResponse<object>.Ok("Email verified successfully."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
           
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _auth.LoginAsync(request, originIp, ct);

            return Ok(ApiResponse<AuthResponse>.Ok(result));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken, CancellationToken ct)
        {
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var response = await _refreshToken.RefreshTokenAsync(refreshToken, originIp, ct);

            if (response == null)
                throw new BadHttpRequestException("Invalid token");

            return Ok(ApiResponse<AuthResponse>.Ok(response));
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken, CancellationToken ct)
        {
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var result = await _refreshToken.RevokeRefreshTokenAsync(refreshToken, originIp, ct);

            if (!result) 
                throw new KeyNotFoundException("Token not found or already revoked");

            return Ok(ApiResponse<object>.Ok("Token revoked"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email, CancellationToken ct)
        {
            var sent = await _auth.SendPasswordResetAsync(email, ct);

            if (!sent)
                throw new BadHttpRequestException("Account not found.");

            return Ok(ApiResponse<object>.Ok("Password reset email sent."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            var success = await _auth.ResetPasswordAsync(request.Token, request.NewPassword, ct);

            if (!success)
                throw new BadHttpRequestException("Invalid or expired token.");

            return Ok(ApiResponse<object>.Ok("Password has been reset successfully."));
        }
    }
}
