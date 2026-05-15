namespace Genzy.Base.Data;

/// <summary>
/// Async-local suppression of global tenant query filters and the <see cref="TenantContextRequiredCommandInterceptor"/> guard.
/// Nested scopes are supported. Prefer <c>IgnoreQueryFilters()</c> on a single <see cref="Microsoft.EntityFrameworkCore.DbSet{TEntity}"/> when the bypass is local to one query.
/// </summary>
public static class TenantQueryFilterScope
{
    private static readonly AsyncLocal<int> SuppressionDepth = new();

    /// <summary>When true, tenant global filters are not applied for the current async flow.</summary>
    public static bool IsSuppressed => SuppressionDepth.Value > 0;

    /// <summary>Disables tenant global query filters until disposed.</summary>
    public static IDisposable Suppress() => new Suppressor();

    private sealed class Suppressor : IDisposable
    {
        private bool _disposed;

        public Suppressor() => SuppressionDepth.Value++;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            SuppressionDepth.Value--;
        }
    }
}
