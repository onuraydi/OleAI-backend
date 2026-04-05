using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.MiddleWares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpExceptionHandler _httpExceptionHandler;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, HttpExceptionHandler httpExceptionHandler, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _httpExceptionHandler = httpExceptionHandler;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An exception occurred while processing the request.");
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            _httpExceptionHandler.Response = context.Response;
            await _httpExceptionHandler.HandleExceptionAsync(exception);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionMiddleWare(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
