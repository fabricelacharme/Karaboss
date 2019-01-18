using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace LyricsWikia.Api
{
    public static class StringUtils
    {

        public static string RemoveHtmlTags(string text)
        {
            text = text.Replace("<br />", "\r\n");

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
        /// Remove accented characters, spaces
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string cleanInput(string input)
        {
            //input = input.Trim().ToLowerInvariant();
            input = input.Replace(" ", "_");
            input = input.Replace("Œ", "Oe");
            input = input.Replace("œ", "oe");
            input = input.Replace("'", "%27");
            input = input.Replace("?", "%3F");

            //input = RemoveDiacritics(input);
            //Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            //input = rgx.Replace(input, "");

            return input;
        }

        public static string cleanArtist(string input)
        {
            //input = input.Trim().ToLowerInvariant();
            input = input.Replace("the", string.Empty);

            char[] array = input.ToCharArray();
            
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 8208)
                    array[i] = '-';
            }
            input = new string(array);


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


        

        /// <summary>
        /// every first letter to upper case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string UppercaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == 8217)
                    array[i - 1] = '\'';
                if (array[i] == 8208)
                    array[i] = '-';

                //if (array[i - 1] == ' ' || array[i - 1] == '\'' || array[i - 1] == 8217 || array[i - 1] == '(')
                if (array[i - 1] == ' ' || array[i - 1] == '\'' || array[i - 1] == '(' || array[i - 1] == '-')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

    }
}
