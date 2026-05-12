using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Genzy.Base.Exceptions;
using Genzy.Base.Json;
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

            LogException(exception, statusCode, context);

            if (_options.CustomExceptionHandler != null)
            {
                var customResult = await _options.CustomExceptionHandler(context, exception, statusCode, response);
                if (customResult.HasValue)
                {
                    (statusCode, response) = customResult.Value;
                }
            }

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

        private static int GetHttpStatusForAppException(AppException ex) => ex switch
        {
            BadException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            NotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        private (int statusCode, ApiResponse response) HandleAppException(AppException appException)
        {
            var statusCode = GetHttpStatusForAppException(appException);
            if (_options.HttpStatusCodeOverride?.Invoke(appException) is { } overridden)
                statusCode = overridden;

            object errorDetail;
            if (_options.IncludeExceptionDetails)
            {
                errorDetail = new
                {
                    ErrorCode = appException.ErrorCode,
                    Detail = appException.Detail,
                    Type = appException.GetType().Name,
                    StackTrace = _options.IncludeStackTrace ? appException.StackTrace : null
                };
            }
            else
            {
                errorDetail = new
                {
                    ErrorCode = appException.ErrorCode,
                    Detail = appException.Detail
                };
            }

            return (statusCode, new ApiResponse(appException.Message, errorDetail));
        }

        private static int GetHttpStatusForGenericException(Exception exception) => exception switch
        {
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            ArgumentNullException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            _ => (int)HttpStatusCode.InternalServerError
        };

        private static string? GetErrorCodeForGenericException(Exception exception) => exception switch
        {
            UnauthorizedAccessException => AppErrorCodes.UnauthorizedAccess,
            ArgumentNullException => AppErrorCodes.ArgumentNull,
            ArgumentException => AppErrorCodes.ArgumentInvalid,
            InvalidOperationException => AppErrorCodes.InvalidOperation,
            NotImplementedException => AppErrorCodes.NotImplemented,
            TimeoutException => AppErrorCodes.Timeout,
            _ => AppErrorCodes.InternalError
        };

        private (int statusCode, ApiResponse response) HandleGenericException(Exception exception)
        {
            var statusCode = GetHttpStatusForGenericException(exception);

            var message = _options.ExposeGenericExceptionMessages
                ? exception.Message
                : "An error occurred while processing your request.";

            var errorCode = GetErrorCodeForGenericException(exception);

            object errorDetail;
            if (_options.IncludeExceptionDetails)
            {
                errorDetail = new
                {
                    ErrorCode = errorCode,
                    Type = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = _options.IncludeStackTrace ? exception.StackTrace : null,
                    InnerException = exception.InnerException?.Message
                };
            }
            else
            {
                errorDetail = new { ErrorCode = errorCode };
            }

            return (statusCode, new ApiResponse(message, errorDetail));
        }

        private void LogException(Exception exception, int statusCode, HttpContext context)
        {
            var logLevel = ResolveLogLevel(exception, statusCode);

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

        private static LogLevel ResolveLogLevel(Exception exception, int statusCode)
        {
            return exception switch
            {
                NotFoundException => LogLevel.Information,
                BadException or ValidationException => LogLevel.Warning,
                UnauthorizedException or ForbiddenException => LogLevel.Warning,
                UnauthorizedAccessException => LogLevel.Warning,
                ArgumentNullException or ArgumentException or InvalidOperationException => LogLevel.Warning,
                TimeoutException => LogLevel.Warning,
                NotImplementedException => LogLevel.Warning,
                AppException => statusCode >= 500 ? LogLevel.Error : LogLevel.Warning,
                _ when statusCode >= 500 => LogLevel.Error,
                _ => LogLevel.Warning
            };
        }
    }
}
