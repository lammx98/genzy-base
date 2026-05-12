using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Genzy.Base.Exceptions;
using Genzy.Base.Wrappers;

namespace Genzy.Base.Middleware
{
    /// <summary>
    /// Configuration options for the exception handling middleware
    /// </summary>
    public class ExceptionHandlingOptions
    {
        /// <summary>
        /// Include detailed exception information in the response (for development environments)
        /// </summary>
        public bool IncludeExceptionDetails { get; set; } = false;

        /// <summary>
        /// Include stack trace in the error response (for development environments)
        /// </summary>
        public bool IncludeStackTrace { get; set; } = false;

        /// <summary>
        /// Expose generic exception messages to clients (default: false for security)
        /// </summary>
        public bool ExposeGenericExceptionMessages { get; set; } = false;

        /// <summary>
        /// Include JSON indentation in response for better readability
        /// </summary>
        public bool IncludeJsonIndentation { get; set; } = false;

        /// <summary>
        /// When set, return value overrides HTTP status inferred from <see cref="Exception"/> type for <see cref="AppException"/> (and subclasses).
        /// Return null to keep the default status for that exception type.
        /// </summary>
        public Func<AppException, int?>? HttpStatusCodeOverride { get; set; }

        /// <summary>
        /// Custom exception handler that can modify the response before it's sent
        /// Returns tuple of (statusCode, response) or null to use default handling
        /// </summary>
        public Func<HttpContext, Exception, int, ApiResponse, Task<(int, ApiResponse)?>>? CustomExceptionHandler { get; set; }

        /// <summary>
        /// Custom logging function for exceptions
        /// </summary>
        public Action<Exception, int, HttpContext, ILogger<ExceptionHandlingMiddleware>>? CustomLogger { get; set; }

        /// <summary>
        /// Create default options for development environment
        /// </summary>
        public static ExceptionHandlingOptions Development()
        {
            return new ExceptionHandlingOptions
            {
                IncludeExceptionDetails = true,
                IncludeStackTrace = true,
                ExposeGenericExceptionMessages = true,
                IncludeJsonIndentation = true
            };
        }

        /// <summary>
        /// Create default options for production environment
        /// </summary>
        public static ExceptionHandlingOptions Production()
        {
            return new ExceptionHandlingOptions
            {
                IncludeExceptionDetails = false,
                IncludeStackTrace = false,
                ExposeGenericExceptionMessages = false,
                IncludeJsonIndentation = false
            };
        }
    }
}
