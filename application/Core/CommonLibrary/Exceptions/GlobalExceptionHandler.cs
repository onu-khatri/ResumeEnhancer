using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmptyProjectTesting.Middleware
{
    public class GlobalExceptionHandler: IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        // Factory-based middleware me DI constructor ke throw direct inject hoti hai
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // 1. Log the Error
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // 2. Set Response Metadata
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                // 3. Prepare Error Response Body
                var errorResponse = new
                {
                    context.Response.StatusCode,
                    ex.Message,
                    InnerException = ex.InnerException?.Message,
                    // Security Best Practice: StackTrace sirf Development Environment me show hoga
                    DetailMessage = _env.IsDevelopment() ? ex.StackTrace : null
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
