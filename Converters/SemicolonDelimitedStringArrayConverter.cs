using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Genzy.Base.Converters;

/// <summary>
/// Converts string[] to/from delimiter-separated string for DB storage.
/// Example: ["a","b","c"] with separator ";" stored as "a;b;c"
/// </summary>
public class SemicolonDelimitedStringArrayConverter : ValueConverter<string[]?, string?>
{
    public string Separator { get; }

    public SemicolonDelimitedStringArrayConverter(string separator = ";") : base(
        v => SerializeArray(v, separator),
        v => DeserializeArray(v, separator))
    {
        Separator = separator;
    }

    private static string? SerializeArray(string[]? arr, string separator)
    {
        if (arr == null || arr.Length == 0)
            return null;
        return string.Join(separator, arr);
    }

    private static string[]? DeserializeArray(string? value, string separator)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
