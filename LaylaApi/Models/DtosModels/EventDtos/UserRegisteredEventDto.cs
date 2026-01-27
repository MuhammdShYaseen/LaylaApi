namespace LaylaApi.Models.DtosModels.EventDtos
{
    public class UserRegisteredEventDto
    {
        public string FullName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Lang { get; init; } = "en";
        public string VerificationUrl { get; init; } = string.Empty;
    }
}
