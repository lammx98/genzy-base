using System.Text.Json;
using System.Text.Json.Serialization;

namespace Genzy.Base.Json;

/// <summary>
/// Bidirectional converter for ulong:
/// - Response: serializes ulong as string (avoids JavaScript number precision loss for values &gt; 2^53).
/// - Request: accepts both number (123) and string ("123") from client, converts to ulong.
/// </summary>
public sealed class UlongAsStringJsonConverter : JsonConverter<ulong>
{
    public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                throw new JsonException("Cannot convert null or empty string to ulong.");
            if (!ulong.TryParse(s, out var v))
                throw new JsonException($"The value '{s}' is not a valid ulong.");
            return v;
        }
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetUInt64();
        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing ulong.");
    }

    public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

/// <summary>
/// Bidirectional converter for ulong?:
/// - Response: serializes ulong? as string (null remains null).
/// - Request: accepts number (123), string ("123"), or null from client, converts to ulong?.
/// </summary>
public sealed class NullableUlongAsStringJsonConverter : JsonConverter<ulong?>
{
    public override ulong? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (string.IsNullOrWhiteSpace(s))
                return null;
            if (!ulong.TryParse(s, out var v))
                throw new JsonException($"The value '{s}' is not a valid ulong.");
            return v;
        }
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetUInt64();
        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing ulong?.");
    }

    public override void Write(Utf8JsonWriter writer, ulong? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.Value.ToString());
    }
}
