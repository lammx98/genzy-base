using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Data;

public static class DbContextOptionsBuilderTenantGuardExtensions
{
    /// <summary>
    /// Registers <see cref="TenantContextRequiredCommandInterceptor"/> so missing tenant fails with an exception instead of silent empty results.
    /// Requires <see cref="Genzy.Base.Security.UserContextServiceCollectionExtensions.AddUserContext(Microsoft.Extensions.DependencyInjection.IServiceCollection)"/> (or manual <c>AddScoped&lt;TenantContextRequiredCommandInterceptor&gt;()</c>).
    /// </summary>
    public static DbContextOptionsBuilder UseGenzyTenantContextRequiredGuard(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        return options.AddInterceptors(serviceProvider.GetRequiredService<TenantContextRequiredCommandInterceptor>());
    }
}
