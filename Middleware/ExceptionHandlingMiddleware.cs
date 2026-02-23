using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Genzy.Base.Exceptions;
using Genzy.Base.Wrappers;

namespace Genzy.Base.Middleware
{
    /// <summary>
    /// Middleware for handling all exceptions globally and returning consistent API responses
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly ExceptionHandlingOptions _options;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            ExceptionHandlingOptions? options = null)
        {
            _next = next;
            _logger = logger;
            _options = options ?? new ExceptionHandlingOptions();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, response) = exception switch
            {
                AppException appEx => HandleAppException(appEx),
                _ => HandleGenericException(exception)
            };

            // Log the exception
            LogException(exception, statusCode, context);

            // Allow custom exception handler to modify response
            if (_options.CustomExceptionHandler != null)
            {
                var customResult = await _options.CustomExceptionHandler(context, exception, statusCode, response);
                if (customResult.HasValue)
                {
                    (statusCode, response) = customResult.Value;
                }
            }

            // Set response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _options.IncludeJsonIndentation
            };

            var json = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(json);
        }

        private (int statusCode, ApiResponse response) HandleAppException(AppException appException)
        {
            var statusCode = appException.ErrorCode;

            // Map error code to HTTP status code if needed
            if (_options.ErrorCodeMapper != null)
            {
                statusCode = _options.ErrorCodeMapper(appException.ErrorCode);
            }

            var errorDetail = _options.IncludeExceptionDetails
                ? new
                {
                    Type = appException.GetType().Name,
                    ErrorCode = appException.ErrorCode,
                    Detail = appException.Detail,
                    StackTrace = _options.IncludeStackTrace ? appException.StackTrace : null
                }
                : appException.Detail;

            return (statusCode, new ApiResponse(appException.Message, errorDetail));
        }

        private (int statusCode, ApiResponse response) HandleGenericException(Exception exception)
        {

            // Determine status code based on exception type
            int statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ArgumentNullException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                NotImplementedException => (int)HttpStatusCode.NotImplemented,
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var message = _options.ExposeGenericExceptionMessages
                ? exception.Message
                : "An error occurred while processing your request.";

            var errorDetail = _options.IncludeExceptionDetails
                ? new
                {
                    Type = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = _options.IncludeStackTrace ? exception.StackTrace : null,
                    InnerException = exception.InnerException?.Message
                }
                : (object?)null;

            return (statusCode, new ApiResponse(message, errorDetail));
        }

        private void LogException(Exception exception, int statusCode, HttpContext context)
        {
            var logLevel = statusCode >= 500 ? LogLevel.Error : LogLevel.Warning;

            if (_options.CustomLogger != null)
            {
                _options.CustomLogger(exception, statusCode, context, _logger);
            }
            else
            {
                _logger.Log(
                    logLevel,
                    exception,
                    "Exception occurred: {ExceptionType} - {Message} | Path: {Path} | StatusCode: {StatusCode}",
                    exception.GetType().Name,
                    exception.Message,
                    context.Request.Path,
                    statusCode
                );
            }
        }
    }
}
