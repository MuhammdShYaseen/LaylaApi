using Microsoft.AspNetCore.Mvc;

namespace LaylaApi.Middleware.ErrorHandler
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModelStateValidationHandler (this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value?.Errors != null && x.Value.Errors.Count > 0)
                        .Select(x => new
                        {
                            field = x.Key,
                            errors = x.Value!.Errors.Select(e => e.ErrorMessage)
                        });

                    return new BadRequestObjectResult(new
                    {
                        success = false,
                        message = "Validation error",
                        errors
                    });
                };
            });
            return services;
        }
        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlerMiddleware>();
        }
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationIdMiddleware>();
        }
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestResponseLoggingMiddleware>();
        }

    }
}
