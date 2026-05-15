using System.Data.Common;
using Genzy.Base.Security;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Genzy.Base.Data;

/// <summary>
/// Fails closed: any database command that returns data (reader/scalar) runs only when a tenant is present,
/// unless <see cref="TenantQueryFilterScope"/>, <see cref="IUserContext.BypassTenantQueryFilter"/>, or migration introspection applies.
/// </summary>
public sealed class TenantContextRequiredCommandInterceptor(IUserContext? userContext) : DbCommandInterceptor
{
    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        AssertTenantOrThrow(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        AssertTenantOrThrow(command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        AssertTenantOrThrow(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        AssertTenantOrThrow(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void AssertTenantOrThrow(DbCommand command)
    {
        if (TenantQueryFilterScope.IsSuppressed)
            return;

        if (userContext?.BypassTenantQueryFilter == true)
            return;

        if (userContext?.TenantId is > 0)
            return;

        if (ShouldSkipGuardForCommandText(command.CommandText))
            return;

        throw new InvalidOperationException(
            "Tenant context is required before executing a database query. " +
            "Authenticate with tenant_id, use TenantQueryFilterScope.Suppress(), or set IUserContext.BypassTenantQueryFilter for explicit opt-in (e.g. design-time). " +
            "Note: IgnoreQueryFilters() does not disable this guard.");
    }

    /// <summary>EF migrations and provider introspection run without a JWT tenant.</summary>
    private static bool ShouldSkipGuardForCommandText(string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return true;

        // Pending-migrations checks, history inserts from tooling, etc.
        if (sql.Contains("__EFMigrationsHistory", StringComparison.OrdinalIgnoreCase))
            return true;

        // Common design-time / migration introspection (SQL Server, Postgres)
        if (sql.Contains("INFORMATION_SCHEMA", StringComparison.OrdinalIgnoreCase))
            return true;

        if (sql.Contains("pg_catalog", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
