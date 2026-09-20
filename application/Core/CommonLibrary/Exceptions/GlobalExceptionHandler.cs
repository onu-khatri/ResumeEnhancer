using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ResumeEnhancer.Core.CommonLibrary.Exceptions;

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
                var correlationId = context.TraceIdentifier;
                _logger.LogError(ex, "Unhandled request failure. CorrelationId: {CorrelationId}", correlationId);

                // 2. Set Response Metadata
                await ApiProblemDetails.WriteAsync(context, "UNEXPECTED_ERROR", StatusCodes.Status500InternalServerError);
            }
        }
    }
}
