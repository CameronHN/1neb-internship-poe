using System.Net;
using System.Text.Json;
using Portfolio.Core.Exceptions;

namespace Portfolio.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception with more context
                _logger.LogError(
                    ex,
                    "Exception occurred for {Method} {Path}. Message: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message
                );

                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Determine status code and message
            var (statusCode, message) = GetStatusCodeAndMessage(exception);

            // Create simple response object
            var response = new { error = new { message = message, statusCode = (int)statusCode } };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(
                response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            await context.Response.WriteAsync(json);
        }

        private static (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(
            Exception exception
        )
        {
            return exception switch
            {
                ValidationException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedAccessAppException => (HttpStatusCode.Unauthorized, "Access denied"),
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, exception.Message),
                BusinessRuleViolationException => (
                    HttpStatusCode.UnprocessableEntity,
                    exception.Message
                ),
                _ => (
                    HttpStatusCode.InternalServerError,
                    "An error occurred while processing your request"
                ),
            };
        }
    }
}
