using StoreBackEnd.Api.Application.Dto.Common;
using StoreBackEnd.Api.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace StoreBackEnd.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await GererExceptionAsync(context, ex);
            }
        }

        private async Task GererExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                BusinessRuleException => (HttpStatusCode.BadRequest, exception.Message),
                UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message),
                ConflictException => (HttpStatusCode.Conflict, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "Une erreur interne est survenue.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Erreur non geree: {Message}", exception.Message);
            else
                _logger.LogWarning("{ExceptionType}: {Message}", exception.GetType().Name, exception.Message);

            var response = new ErrorResponseDTO
            {
                StatusCode = (int)statusCode,
                Message = message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}