﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AnalyzerAndReplaceUnicodeString
{
    public class StringHelper
    {
        private static readonly Dictionary<char, char> UnicodeToAsciiMap = new Dictionary<char, char>
    {
        { 'á', 'a' }, { 'à', 'a' }, { 'ả', 'a' }, { 'ã', 'a' }, { 'ạ', 'a' },
        { 'ă', 'a' }, { 'ắ', 'a' }, { 'ằ', 'a' }, { 'ẳ', 'a' }, { 'ẵ', 'a' }, { 'ặ', 'a' },
        { 'â', 'a' }, { 'ấ', 'a' }, { 'ầ', 'a' }, { 'ẩ', 'a' }, { 'ẫ', 'a' }, { 'ậ', 'a' },
        { 'é', 'e' }, { 'è', 'e' }, { 'ẻ', 'e' }, { 'ẽ', 'e' }, { 'ẹ', 'e' },
        { 'ê', 'e' }, { 'ế', 'e' }, { 'ề', 'e' }, { 'ể', 'e' }, { 'ễ', 'e' }, { 'ệ', 'e' },
        { 'í', 'i' }, { 'ì', 'i' }, { 'ỉ', 'i' }, { 'ĩ', 'i' }, { 'ị', 'i' },
        { 'ó', 'o' }, { 'ò', 'o' }, { 'ỏ', 'o' }, { 'õ', 'o' }, { 'ọ', 'o' },
        { 'ô', 'o' }, { 'ố', 'o' }, { 'ồ', 'o' }, { 'ổ', 'o' }, { 'ỗ', 'o' }, { 'ộ', 'o' },
        { 'ơ', 'o' }, { 'ớ', 'o' }, { 'ờ', 'o' }, { 'ở', 'o' }, { 'ỡ', 'o' }, { 'ợ', 'o' },
        { 'ú', 'u' }, { 'ù', 'u' }, { 'ủ', 'u' }, { 'ũ', 'u' }, { 'ụ', 'u' },
        { 'ư', 'u' }, { 'ứ', 'u' }, { 'ừ', 'u' }, { 'ử', 'u' }, { 'ữ', 'u' }, { 'ự', 'u' },
        { 'ý', 'y' }, { 'ỳ', 'y' }, { 'ỷ', 'y' }, { 'ỹ', 'y' }, { 'ỵ', 'y' },
        { 'đ', 'd' }, { 'Á', 'A' }, { 'À', 'A' }, { 'Ả', 'A' }, { 'Ã', 'A' }, { 'Ạ', 'A' },
        { 'Ă', 'A' }, { 'Ắ', 'A' }, { 'Ằ', 'A' }, { 'Ẳ', 'A' }, { 'Ẵ', 'A' }, { 'Ặ', 'A' },
        { 'Â', 'A' }, { 'Ấ', 'A' }, { 'Ầ', 'A' }, { 'Ẩ', 'A' }, { 'Ẫ', 'A' }, { 'Ậ', 'A' },
        { 'É', 'E' }, { 'È', 'E' }, { 'Ẻ', 'E' }, { 'Ẽ', 'E' }, { 'Ẹ', 'E' },
        { 'Ê', 'E' }, { 'Ế', 'E' }, { 'Ề', 'E' }, { 'Ể', 'E' }, { 'Ễ', 'E' }, { 'Ệ', 'E' },
        { 'Í', 'I' }, { 'Ì', 'I' }, { 'Ỉ', 'I' }, { 'Ĩ', 'I' }, { 'Ị', 'I' },
        { 'Ó', 'O' }, { 'Ò', 'O' }, { 'Ỏ', 'O' }, { 'Õ', 'O' }, { 'Ọ', 'O' },
        { 'Ô', 'O' }, { 'Ố', 'O' }, { 'Ồ', 'O' }, { 'Ổ', 'O' }, { 'Ỗ', 'O' }, { 'Ộ', 'O' },
        { 'Ơ', 'O' }, { 'Ớ', 'O' }, { 'Ờ', 'O' }, { 'Ở', 'O' }, { 'Ỡ', 'O' }, { 'Ợ', 'O' },
        { 'Ú', 'U' }, { 'Ù', 'U' }, { 'Ủ', 'U' }, { 'Ũ', 'U' }, { 'Ụ', 'U' },
        { 'Ư', 'U' }, { 'Ứ', 'U' }, { 'Ừ', 'U' }, { 'Ử', 'U' }, { 'Ữ', 'U' }, { 'Ự', 'U' },
        { 'Ý', 'Y' }, { 'Ỳ', 'Y' }, { 'Ỷ', 'Y' }, { 'Ỹ', 'Y' }, { 'Ỵ', 'Y' },
        { 'Đ', 'D' }
    };
        public static string ConvertToSlug(string input, string router)
        {
            var stringBuilder = new StringBuilder();

            foreach (var c in router)
            {
                if (char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
            }

            stringBuilder.Append(".");

            foreach (var c in input)
            {
                if (UnicodeToAsciiMap.TryGetValue(c, out char replacement))
                {
                    stringBuilder.Append(replacement);
                }
                else if (char.IsLetterOrDigit(c))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("_");
                }
            }

            string slug = stringBuilder.ToString().ToLower();

            slug = Regex.Replace(slug, @"\s+", "");

            return slug;
        }
    }
}
