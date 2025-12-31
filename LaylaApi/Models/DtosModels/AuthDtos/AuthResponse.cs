namespace LaylaApi.Models.DtosModels.AuthDtos
{
    public class AuthResponse
    {
        public string JwtToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresInSeconds { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
