using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Datas.ViewModels.Errors;
using System.Net;

namespace Configurations.Middlewares
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
            catch (ArgumentNullException arg_ex)
            {
                await HandleExceptionAsync(context, arg_ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ExceptionResponse response = exception switch
            {
                ApplicationException _ => new ExceptionResponse(StatusCodes.Status400BadRequest, exception.Message),
                KeyNotFoundException _ => new ExceptionResponse(StatusCodes.Status404NotFound, exception.Message),
                ArgumentNullException _ => new ExceptionResponse(StatusCodes.Status404NotFound, exception.Message),
                UnauthorizedAccessException _ => new ExceptionResponse(StatusCodes.Status401Unauthorized, exception.Message),
                _ => new ExceptionResponse(StatusCodes.Status500InternalServerError, exception.Message)
            };

            context.Response.StatusCode = (int)response.StatusCode;            

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
