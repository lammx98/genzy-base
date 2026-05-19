using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Genzy.Base.Converters;

public class UlongListValueConverter : ValueConverter<List<ulong>, string?>
{
    private static readonly JsonSerializerOptions _options = new();

    public UlongListValueConverter() : base(
        v => SerializeList(v),
        v => DeserializeList(v))
    {
    }

    private static string? SerializeList(List<ulong> list)
    {
        if (list.Count == 0)
            return null;
        return JsonSerializer.Serialize(list, _options);
    }

    private static List<ulong> DeserializeList(string? json)
    {
        if (string.IsNullOrWhiteSpace(json) || json.Trim() == "null")
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<ulong>>(json, _options) ?? [];
        }
        catch
        {
            return [];
        }
    }
}
