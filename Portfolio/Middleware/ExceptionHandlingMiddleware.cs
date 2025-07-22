using Portfolio.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace Portfolio.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // Continue down the pipeline
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    status = HttpStatusCode.NotFound; // 404
                    break;
                case ValidationException:
                    status = HttpStatusCode.BadRequest; // 400
                    break;
                case UnauthorizedAccessAppException:
                    status = HttpStatusCode.Unauthorized; // 401
                    break;
                case ConflictException:
                    status = HttpStatusCode.Conflict; // 409
                    break;
                case BusinessRuleViolationException:
                    status = HttpStatusCode.UnprocessableEntity; // 422
                    break;
                default:
                    status = HttpStatusCode.InternalServerError; // 500
                    break;
            }

            var problemDetails = new
            {
                status = (int)status,
                title = status.ToString(),
                detail = message
            };

            context.Response.ContentType = "application/json; charset-utf-8";
            context.Response.StatusCode = (int)status;

            var result = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(result);
        }
    }

}
