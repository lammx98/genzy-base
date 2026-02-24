// JwtExtensions.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Genzy.Base.Security.Jwt;

public static class JwtExtensions
{
    // New method that returns AuthenticationBuilder so callers can chain Cookie/Google/Facebook etc.
    public static AuthenticationBuilder AddJwtCore(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions>? configure = null)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtOptions>>().Value);

        var options = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                      ?? throw new InvalidOperationException("Jwt config missing");

        byte[] keyBytes = Encoding.UTF8.GetBytes(options.Secret);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        return services.AddAuthentication(authOpts =>
        {
            authOpts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOpts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            authOpts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrEmpty(options.Issuer),
                ValidateAudience = !string.IsNullOrEmpty(options.Audience),
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(60),
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience
            };
            
            // Setup default logging events - only log errors, will be merged with custom configure if provided
            var defaultEvents = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine($"[JWT] Authentication failed: {ctx.Exception.GetType().Name} - {ctx.Exception.Message}");
                    if (ctx.Exception.InnerException != null)
                    {
                        Console.WriteLine($"[JWT] Inner exception: {ctx.Exception.InnerException.Message}");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = ctx =>
                {
                    Console.WriteLine($"[JWT] Challenge: Error={ctx.Error}, ErrorDescription={ctx.ErrorDescription}");
                    return Task.CompletedTask;
                }
            };
            
            cfg.Events = defaultEvents;
            
            // Allow custom configuration to override or extend events
            configure?.Invoke(cfg);
        });
    }

    // Backwards compatible existing method for other services already calling it.
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions>? configure = null)
    {
        var builder = services.AddJwtCore(configuration, configure);
        services.AddScoped<IJwtService, JwtService>();
        return services;
    }
}
