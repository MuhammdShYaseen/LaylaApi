namespace LaylaApi.DomainEvents.Domain.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }
        public object?[]? Args { get; }

        public BusinessException(string errorCode, int statusCode = 400, params object?[]? args)
            : base(errorCode)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Args = args;
        }
    }
}