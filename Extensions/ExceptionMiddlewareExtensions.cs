using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Genzy.Base.Middleware;

namespace Genzy.Base.Extensions
{
    /// <summary>
    /// Extension methods for registering exception handling middleware
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Adds exception handling middleware with default options
        /// </summary>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        /// <summary>
        /// Adds exception handling middleware with custom options
        /// </summary>
        public static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder app,
            ExceptionHandlingOptions options)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>(options);
        }

        /// <summary>
        /// Adds exception handling middleware with options configuration
        /// </summary>
        public static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder app,
            Action<ExceptionHandlingOptions> configureOptions)
        {
            var options = new ExceptionHandlingOptions();
            configureOptions(options);
            return app.UseMiddleware<ExceptionHandlingMiddleware>(options);
        }

        /// <summary>
        /// Adds exception handling middleware for development environment
        /// </summary>
        public static IApplicationBuilder UseExceptionHandlingDevelopment(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>(ExceptionHandlingOptions.Development());
        }

        /// <summary>
        /// Adds exception handling middleware for production environment
        /// </summary>
        public static IApplicationBuilder UseExceptionHandlingProduction(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>(ExceptionHandlingOptions.Production());
        }
    }
}
