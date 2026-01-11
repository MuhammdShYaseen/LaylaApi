namespace LaylaApi.Models.ErrorModels
{
    public sealed class SafeErrorDetails
    {
        public string ExceptionType { get; init; } = default!;
        public string Message { get; init; } = default!;
        public string Location { get; init; } = "Unknown";
    }
}
