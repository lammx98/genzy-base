using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Genzy.Base.Extensions;

/// <summary>
/// OpenAPI / Swagger: document <see cref="ulong"/> Snowflake ids as JSON strings (same as <see cref="JsonServiceExtensions.AddUlongAsStringSerialization"/>).
/// </summary>
public static class SnowflakeOpenApiExtensions
{
    /// <summary>
    /// Registers OpenAPI document generation with ulong/ulong? schemas as strings (MapOpenApi /openapi/v1.json).
    /// </summary>
    public static IServiceCollection AddOpenApiWithSnowflakeULongSchemas(this IServiceCollection services)
    {
        services.AddOpenApi(options => options.ConfigureSnowflakeULongSchemas());
        return services;
    }

    public static void ConfigureSnowflakeULongSchemas(this OpenApiOptions options)
    {
        options.AddSchemaTransformer((schema, context, _) =>
        {
            var t = context.JsonTypeInfo.Type;
            if (t == typeof(ulong))
            {
                schema.Type = JsonSchemaType.String;
                schema.Format = null;
            }
            else if (t == typeof(ulong?))
            {
                schema.Type = JsonSchemaType.String | JsonSchemaType.Null;
                schema.Format = null;
            }
            return Task.CompletedTask;
        });
    }

    public static void MapSnowflakeULongSwaggerTypes(this SwaggerGenOptions options)
    {
        options.MapType<ulong>(() => new Microsoft.OpenApi.OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Description = "Snowflake id (serialized as JSON string for JavaScript compatibility).",
            Format = null
        });
        options.MapType<ulong?>(() => new Microsoft.OpenApi.OpenApiSchema
        {
            Type = JsonSchemaType.String | JsonSchemaType.Null,
            Format = null
        });
    }
}
