using Genzy.Base.Conventers;
using Microsoft.EntityFrameworkCore;

namespace Genzy.Base.Extensions;

/// <summary>
/// EF Core model configuration extensions.
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
    /// <summary>
    /// Adds the StoredAsInt convention. Properties or enum types marked with [StoredAsInt]
    /// will automatically use int (or int?) for DB storage instead of string.
    /// </summary>
    public static ModelConfigurationBuilder AddStoredAsIntConvention(
        this ModelConfigurationBuilder builder)
    {
        builder.Conventions.Add(_ => new StoredAsIntConvention());
        return builder;
    }

    /// <summary>
    /// Adds the SemicolonDelimited convention. Properties marked with [SemicolonDelimited]
    /// (string[] or string[]?) will be stored as semicolon-delimited strings.
    /// </summary>
    public static ModelConfigurationBuilder AddSemicolonDelimitedConvention(
        this ModelConfigurationBuilder builder)
    {
        builder.Conventions.Add(_ => new SemicolonDelimitedConvention());
        return builder;
    }

    /// <summary>
    /// Adds the StoredAsJson convention. Properties marked with [StoredAsJson]
    /// (string[] or string[]?) will be stored as JSON strings.
    /// </summary>
    public static ModelConfigurationBuilder AddStoredAsJsonConvention(
        this ModelConfigurationBuilder builder)
    {
        builder.Conventions.Add(_ => new StoredAsJsonConvention());
        return builder;
    }
}
