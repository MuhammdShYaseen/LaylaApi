using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace LaylaApi.Middleware.ErrorHandler
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var errorId = Guid.NewGuid().ToString();

            var statusCode = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                BadHttpRequestException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var safeErrorDetails = BuildSafeErrorDetails(ex);
            _logger.LogError(ex,"Unhandled exception | ErrorId: {ErrorId} | Path: {Path} | User: {User} | Summary: {@Summary}",
                                                     errorId,context.Request.Path, context.User?.Identity?.Name ?? "Anonymous", safeErrorDetails);

               #if DEBUG
                 var developerMessage = ex.ToString();
               #else
                  string? developerMessage = null;
               #endif

            
            var response = new
            {
                success = false,
                errorId,
                errorCode = statusCode.ToString(), // Frontend يعتمد عليه
                message = "An unexpected error occurred.",
                developerMessage,
                errorSummary = safeErrorDetails,
                statusCode = (int)statusCode,
                timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static object BuildSafeErrorDetails(Exception ex)
        {
            var stackFrame = new StackTrace(ex, true)
                                .GetFrames()?
                                .FirstOrDefault(f => f.GetFileLineNumber() > 0);

            return new
            {
                exceptionType = ex.GetType().Name,
                message = ex.Message,
                source = ex.Source,
                location = stackFrame is not null
                    ? $"{stackFrame.GetMethod()?.DeclaringType?.Name}.{stackFrame.GetMethod()?.Name}"
                    : "Unknown"
            };
        }
    }
}
