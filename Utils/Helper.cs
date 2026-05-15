using System.Security.Cryptography;

namespace Genzy.Base.Utils;

public static class Helper
{
    private const string DefaultRandomChars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    /// <summary>
    /// Tạo chuỗi ngẫu nhiên (cryptographically secure) với độ dài <paramref name="length"/>.
    /// </summary>
    /// <param name="length">Số ký tự. 0 trả về chuỗi rỗng.</param>
    /// <param name="allowedChars">
    /// Tập ký tự được phép; mặc định chữ Latin và chữ số.
    /// </param>
    public static string GenerateRandomString(int length, string? allowedChars = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        if (length == 0)
            return string.Empty;

        var chars = string.IsNullOrEmpty(allowedChars) ? DefaultRandomChars : allowedChars;
        if (chars.Length == 0)
            throw new ArgumentException("Allowed characters must not be empty.", nameof(allowedChars));

        var buffer = new char[length];
        for (var i = 0; i < length; i++)
            buffer[i] = chars[RandomNumberGenerator.GetInt32(0, chars.Length)];

        return new string(buffer);
    }
}
