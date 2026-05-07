using Genzy.Base.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Extensions;

public static class SnowflakeServiceCollectionExtensions
{
    /// <summary>
    /// Binds <see cref="SnowflakeOptions"/> from configuration (section <see cref="SnowflakeOptions.SectionName"/>) and registers <see cref="SnowflakeIdGenerator"/> as singleton.
    /// </summary>
    public static IServiceCollection AddSnowflakeIdGenerator(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SnowflakeOptions>(configuration.GetSection(SnowflakeOptions.SectionName));
        services.AddSingleton<SnowflakeIdGenerator>();
        return services;
    }
}
