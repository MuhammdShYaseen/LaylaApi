namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class PasswordResetRequestedEventDto
    {
        public int UserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string Lang { get; init; } = "en";
        public string Token { get; init; } = string.Empty;
    }
}
