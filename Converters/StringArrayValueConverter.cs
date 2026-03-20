using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Genzy.Base.Converters;

/// <summary>
/// Converts string[] to/from JSON string for DB storage.
/// </summary>
public class StringArrayValueConverter : ValueConverter<string[]?, string?>
{
    private static readonly JsonSerializerOptions Options = new();

    public StringArrayValueConverter() : base(
        v => SerializeArray(v),
        v => DeserializeArray(v))
    {
    }

    private static string? SerializeArray(string[]? arr)
    {
        if (arr == null || arr.Length == 0)
            return null;
        return JsonSerializer.Serialize(arr, Options);
    }

    private static string[]? DeserializeArray(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
            return null;

        try
        {
            var result = JsonSerializer.Deserialize<string[]>(json, Options);
            return result ?? null;
        }
        catch
        {
            return null;
        }
    }
}
