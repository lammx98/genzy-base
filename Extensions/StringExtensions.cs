namespace Genzy.Base.Extensions;

public static class StringExtensions
{
    public static string PadBase64(this string input)
    {
        var padding = (4 - input.Length % 4) % 4;
        return input + new string('=', padding);
    }
}
