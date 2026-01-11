using LaylaApi.Models.GenericResponseModels;

namespace LaylaApi.Models.ErrorModels
{
    public class ApiErrorResponse : ApiResponse<object>
    {
        public string ErrorCode { get; init; } = default!;
        public string? DeveloperMessage { get; init; }
        public SafeErrorDetails? ErrorSummary { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
