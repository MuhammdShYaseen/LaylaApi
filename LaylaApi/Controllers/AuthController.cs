using LaylaApi.Models.DtosModels.AuthDtos;
using LaylaApi.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LaylaApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService auth, ILogger<AuthController> logger)
        {
            _auth = auth;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var result = await _auth.RegisterAsync(request, originIp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Register failed for {Email}", request.Email);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            var success = await _auth.VerifyEmailAsync(token);
            if (!success) return BadRequest(new { message = "Invalid or expired token." });
            return Ok(new { message = "Email verified successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var result = await _auth.LoginAsync(request, originIp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var response = await _auth.RefreshTokenAsync(refreshToken, originIp);
            if (response == null) return Unauthorized(new { message = "Invalid token" });
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> Revoke([FromBody] string refreshToken)
        {
            var originIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _auth.RevokeRefreshTokenAsync(refreshToken, originIp);
            if (!result) return NotFound(new { message = "Token not found or already revoked" });
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var sent = await _auth.SendPasswordResetAsync(email);
            if (!sent)
                return BadRequest(new { message = "Account not found." });

            return Ok(new { message = "Password reset email sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var success = await _auth.ResetPasswordAsync(request.Token, request.NewPassword);
            if (!success)
                return BadRequest(new { message = "Invalid or expired token." });

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}
