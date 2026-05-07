using System.Text.Json;
using Genzy.Base.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Genzy.Base.Extensions;

/// <summary>
/// Extension methods for JSON serialization configuration (ulong / ulong? as JSON strings).
/// Pair with <see cref="SnowflakeOpenApiExtensions"/> for matching OpenAPI/Swagger schemas.
/// </summary>
public static class JsonServiceExtensions
{
    /// <summary>
    /// Adds converters for ulong/ulong? used by both request and response:
    /// - Response: serializes as string (avoids JS number precision loss for values &gt; 2^53).
    /// - Request: accepts number (123) or string ("123") from client, converts to ulong.
    /// </summary>
    public static IMvcBuilder AddUlongAsStringSerialization(this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new UlongAsStringJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new NullableUlongAsStringJsonConverter());
        });
        return builder;
    }
}
