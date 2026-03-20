using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Genzy.Base.Converters;

public class JsonDocumentValueConverter : ValueConverter<JsonDocument, string>
{
    public JsonDocumentValueConverter() : base(
        v => v.RootElement.GetRawText(),
        v => JsonDocument.Parse(v, default(JsonDocumentOptions)))
    {
    }
}
