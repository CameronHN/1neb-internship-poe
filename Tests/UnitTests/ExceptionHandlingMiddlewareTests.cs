using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Core.Exceptions;
using Portfolio.WebApi.Middleware;
using Xunit;
// Both System.ComponentModel.DataAnnotations and Portfolio.Core.Exceptions define
// ValidationException. The middleware maps the Core one, so name it explicitly.
using CoreValidationException = Portfolio.Core.Exceptions.ValidationException;

namespace Portfolio.Tests.UnitTests
{
    public class ExceptionHandlingMiddlewareTests
    {
        /// <summary>
        /// Every exception type that ExceptionHandlingMiddleware.GetStatusCodeAndMessage maps,
        /// with the status code it must produce.
        /// </summary>
        public static TheoryData<Exception, int> ExceptionMappings()
        {
            return new TheoryData<Exception, int>
            {
                { new ArgumentNullException("param"), StatusCodes.Status400BadRequest },
                { new CoreValidationException("invalid"), StatusCodes.Status400BadRequest },
                { new JsonException("bad json"), StatusCodes.Status400BadRequest },
                { new UnauthorizedAccessAppException("denied"), StatusCodes.Status401Unauthorized },
                { new NotFoundException("missing"), StatusCodes.Status404NotFound },
                { new ConflictException("conflict"), StatusCodes.Status409Conflict },
                {
                    new BusinessRuleViolationException("rule"),
                    StatusCodes.Status422UnprocessableEntity
                },
                {
                    new TemplateTypeNotImplementedException("template"),
                    StatusCodes.Status501NotImplemented
                },
                { new NotSupportedException("unsupported"), StatusCodes.Status501NotImplemented },
                {
                    new InvalidOperationException("unexpected"),
                    StatusCodes.Status500InternalServerError
                },
            };
        }

        [Theory]
        [MemberData(nameof(ExceptionMappings))]
        public async Task MapsExceptionToStatusCode(Exception exception, int expectedStatusCode)
        {
            var (context, body) = await InvokeWithExceptionAsync(exception);

            Assert.Equal(expectedStatusCode, context.Response.StatusCode);
            Assert.Equal("application/json", context.Response.ContentType);

            using var json = JsonDocument.Parse(body);
            var error = json.RootElement.GetProperty("error");
            Assert.Equal(expectedStatusCode, error.GetProperty("statusCode").GetInt32());
            Assert.False(string.IsNullOrWhiteSpace(error.GetProperty("message").GetString()));
        }

        [Fact]
        public async Task UnexpectedException_DoesNotLeakItsMessage()
        {
            var (_, body) = await InvokeWithExceptionAsync(
                new InvalidOperationException("SECRET-INTERNAL-DETAIL")
            );

            Assert.DoesNotContain("SECRET-INTERNAL-DETAIL", body);
            Assert.Contains("An error occurred while processing your request", body);
        }

        /// <summary>
        /// Controllers and repositories throw the Core ValidationException with a message meant
        /// for the client (e.g. "Start date is not a valid date."), so it must reach them.
        /// </summary>
        [Fact]
        public async Task ValidationException_ReturnsItsMessageAs400()
        {
            var (context, body) = await InvokeWithExceptionAsync(
                new CoreValidationException("Start date is not a valid date.")
            );

            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
            Assert.Contains("Start date is not a valid date.", body);
        }

        [Fact]
        public async Task NoException_PassesThroughUnchanged()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var middleware = new ExceptionHandlingMiddleware(
                next: ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                    return Task.CompletedTask;
                },
                logger: NullLogger<ExceptionHandlingMiddleware>.Instance
            );

            await middleware.InvokeAsync(context);

            Assert.Equal(StatusCodes.Status204NoContent, context.Response.StatusCode);
            Assert.Equal(0, context.Response.Body.Length);
        }

        private static async Task<(HttpContext Context, string Body)> InvokeWithExceptionAsync(
            Exception exception
        )
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            var middleware = new ExceptionHandlingMiddleware(
                next: _ => throw exception,
                logger: NullLogger<ExceptionHandlingMiddleware>.Instance
            );

            await middleware.InvokeAsync(context);

            context.Response.Body.Position = 0;
            using var reader = new StreamReader(context.Response.Body);
            var body = await reader.ReadToEndAsync();
            return (context, body);
        }
    }
}
