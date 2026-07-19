using System.Net;
using System.Text.Json;

namespace EcommerceAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
                _logger.LogError(ex, "Error no controlado procesando {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var (statusCode, message) = ex switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado en el servidor")
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = (int)statusCode,
                message
            });

            return context.Response.WriteAsync(result);
        }
    }
}