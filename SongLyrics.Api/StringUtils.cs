using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SongLyrics.Api
{
    public static class StringUtils
    {

        public static string RemoveHtmlTags(string text)
        {
            int idx = text.IndexOf('<');
            while (idx >= 0)
            {
                var endIdx = text.IndexOf('>', idx + 1);
                if (endIdx < idx)
                {
                    break;
                }
                text = text.Remove(idx, endIdx - idx + 1);
                idx = text.IndexOf('<', idx);
            }
            return text;
        }

        /// <summary>
        /// Remove accented characters, 
        /// Replace spaces by minus
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string cleanInput(string input)
        {
            input = input.Trim().ToLowerInvariant();
            input = input.Replace(" ", "-");
            input = RemoveDiacritics(input);
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            input = rgx.Replace(input, "");

            input = input.Replace("--", "-");

            return input;
        }

        public static string cleanArtist(string input)
        {
            input = input.Trim().ToLowerInvariant();
            input = input.Replace("the", string.Empty);
            return input;
        }


        /// <summary>
        /// Remove accented characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string RemoveDiacritics(string text)
        {
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

    }
}
