using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Genzy.Base.Utils
{
    public static class Extensions
    {
        public static string RemoveVNUnicodeSymbol(this string vnText)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    vnText = vnText.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return vnText;
        }

        public static string ConvertStringToRouteValue(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            var arr = str.Where(o => Char.IsLetterOrDigit(o) || Char.IsWhiteSpace(o)).ToArray();
            return string.Join("", arr).Trim().ConvertSpaceToRouteValue();
        }

        public static string ConvertSpaceToRouteValue(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            str = regex.Replace(str, " ");
            return str.Replace(" ", "-").ToLower();
        }
        /// <summary>
        /// only text and '-' (3 limited)
        /// Route must be: texta-textb-textc-textd
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ValidateName(this string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z]+(-[a-zA-Z]+){0,3}$");
        }

        public static string? ValidateDatabaseName(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "Invalid value";
            if (str.Length > 50)
                return $"Maximum characters is 50";
            var except_list = new List<string> { "admin", "config", "local" };
            if (except_list.Any(o => str.Equals(o)))
                return $"Cannot use this name '{str}'";

            return ValidateName(str) ? null : "Maximum character '-' is 3";
        }
        public static string? ValidateCollectionName(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "Invalid value";
            if (str.Length > 50)
                return $"Maximum characters is 50";
            var except_list = new List<string>
            {
                "admin",
                "config",
                "local"
            };
            if (except_list.Any(o => str.Equals(o)))
                return $"Cannot use this name '{str}'";

            return ValidateName(str) ? null : "Maximum character '-' is 3";
        }

        public static bool IsAlphaNumeric(this string str)
        {
            return Regex.IsMatch(str, @"^[a-zA-Z0-9]+$");
        }

        public static bool ContainsIgnoreCase(this string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public static string? GetDescription<T>(this T enumValue)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString()!);

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        public static bool IsValidRoute(this string text)
        {
            // Define a regex pattern to match only letters and '-'
            string pattern = @"^[a-zA-Z0-9\-]+$";

            // Use Regex.IsMatch to check if the input matches the pattern
            return Regex.IsMatch(text, pattern);
        }
        public static string GenerateRandomCode(int length = 10)
        {
            const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(characters, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static int GenerateRandomNumber(int max = 1000)
        {
            return new Random().Next(max);
        }
        /****** function helper ******/
        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
    }
}

