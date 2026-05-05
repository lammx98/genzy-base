using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Extensions;

public static class StartupExtensions
{
    public static void AddBaseCors(this IServiceCollection services, string policyName, string[]? origins = null)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(policyName,
                policy => policy.WithOrigins(origins ?? ["http://localhost:3020", "http://localhost:3000"])
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
        });
    }

    public static void AddBaseConfiguration(this IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }   
}
