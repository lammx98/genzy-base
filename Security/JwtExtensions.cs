// JwtExtensions.cs
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Genzy.Base.Security.Jwt;

public static class JwtExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind options from configuration (appsettings / env)
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtOptions>>().Value);

        var options = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                      ?? throw new InvalidOperationException("Jwt config missing");

        var keyBytes = Convert.FromBase64String(options.Key);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(cfg =>
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
            // Optional: events for logging/inspection
            cfg.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    // You can log here
                    return Task.CompletedTask;
                }
            };
        });

        // Add token service for creation / refresh
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
