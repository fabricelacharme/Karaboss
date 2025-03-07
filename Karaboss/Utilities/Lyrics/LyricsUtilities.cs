#region License

/* Copyright (c) 2025 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion

using Karaboss.Mp3.Mp3Lyrics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Karaboss.MidiLyrics;

namespace Karaboss.Utilities
{

    public enum LrcLinesSyllabesFormats
    {
        Lines = 0,
        Syllabes = 1,
    }

   
    public static class LyricsUtilities
    {
        public static string m_SepLine = "/";
        public static string m_SepParagraph = "\\";

        /// <summary>
        /// Conversion table accented chars to non accentued chars
        /// </summary>
        static Dictionary<string, string> foreign_characters = new Dictionary<string, string>
        {
            { "äæǽ", "ae" },
            { "öœ", "oe" },
            { "ü", "ue" },
            { "Ä", "Ae" },
            { "Ü", "Ue" },
            { "Ö", "Oe" },
            { "ÀÁÂÃÄÅǺĀĂĄǍΑΆẢẠẦẪẨẬẰẮẴẲẶА", "A" },
            { "àáâãåǻāăąǎªαάảạầấẫẩậằắẵẳặа", "a" },
            { "Б", "B" },
            { "б", "b" },
            { "ÇĆĈĊČ", "C" },
            { "çćĉċč", "c" },
            { "Д", "D" },
            { "д", "d" },
            { "ÐĎĐΔ", "Dj" },
            { "ðďđδ", "dj" },
            { "ÈÉÊËĒĔĖĘĚΕΈẼẺẸỀẾỄỂỆЕЭ", "E" },
            { "èéêëēĕėęěέεẽẻẹềếễểệеэ", "e" },
            { "Ф", "F" },
            { "ф", "f" },
            { "ĜĞĠĢΓГҐ", "G" },
            { "ĝğġģγгґ", "g" },
            { "ĤĦ", "H" },
            { "ĥħ", "h" },
            { "ÌÍÎÏĨĪĬǏĮİΗΉΊΙΪỈỊИЫ", "I" },
            { "ìíîïĩīĭǐįıηήίιϊỉịиыї", "i" },
            { "Ĵ", "J" },
            { "ĵ", "j" },
            { "ĶΚК", "K" },
            { "ķκк", "k" },
            { "ĹĻĽĿŁΛЛ", "L" },
            { "ĺļľŀłλл", "l" },
            { "М", "M" },
            { "м", "m" },
            { "ÑŃŅŇΝН", "N" },
            { "ñńņňŉνн", "n" },
            { "ÒÓÔÕŌŎǑŐƠØǾΟΌΩΏỎỌỒỐỖỔỘỜỚỠỞỢО", "O" },
            { "òóôõōŏǒőơøǿºοόωώỏọồốỗổộờớỡởợо", "o" },
            { "П", "P" },
            { "п", "p" },
            { "ŔŖŘΡР", "R" },
            { "ŕŗřρр", "r" },
            { "ŚŜŞȘŠΣС", "S" },
            { "śŝşșšſσςс", "s" },
            { "ȚŢŤŦτТ", "T" },
            { "țţťŧт", "t" },
            { "ÙÚÛŨŪŬŮŰŲƯǓǕǗǙǛŨỦỤỪỨỮỬỰУ", "U" },
            { "ùúûũūŭůűųưǔǖǘǚǜυύϋủụừứữửựу", "u" },
            { "ÝŸŶΥΎΫỲỸỶỴЙ", "Y" },
            { "ýÿŷỳỹỷỵй", "y" },
            { "В", "V" },
            { "в", "v" },
            { "Ŵ", "W" },
            { "ŵ", "w" },
            { "ŹŻŽΖЗ", "Z" },
            { "źżžζз", "z" },
            { "ÆǼ", "AE" },
            { "ẞ", "Ss" },
            { "ß", "ss" },
            { "Ĳ", "IJ" },
            { "ĳ", "ij" },
            { "Œ", "OE" },
            { "ƒ", "f" },
            { "ξ", "ks" },
            { "π", "p" },
            { "β", "v" },
            { "μ", "m" },
            { "ψ", "ps" },
            { "Ё", "Yo" },
            { "ё", "yo" },
            { "Є", "Ye" },
            { "є", "ye" },
            { "Ї", "Yi" },
            { "Ж", "Zh" },
            { "ж", "zh" },
            { "Х", "Kh" },
            { "х", "kh" },
            { "Ц", "Ts" },
            { "ц", "ts" },
            { "Ч", "Ch" },
            { "ч", "ch" },
            { "Ш", "Sh" },
            { "ш", "sh" },
            { "Щ", "Shch" },
            { "щ", "shch" },
            { "ЪъЬь", "" },
            { "Ю", "Yu" },
            { "ю", "yu" },
            { "Я", "Ya" },
            { "я", "ya" },
         };

        /// <summary>
        /// Remove accents for chars
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static char RemoveDiacritics(this char c)
        {
            foreach (KeyValuePair<string, string> entry in foreign_characters)
            {
                if (entry.Key.IndexOf(c) != -1)
                {
                    return entry.Value[0];
                }
            }
            return c;
        }

        /// <summary>
        /// Remove accents for strings
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDiacritics(this string s)
        {
            string text = "";

            foreach (char c in s)
            {
                int len = text.Length;

                foreach (KeyValuePair<string, string> entry in foreign_characters)
                {
                    if (entry.Key.IndexOf(c) != -1)
                    {
                        text += entry.Value;
                        break;
                    }
                }

                if (len == text.Length)
                {
                    text += c;
                }
            }
            return text;
        }

        /// <summary>
        /// Remove all non alphanumerc characters of a string
        /// Except space, -, '
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveNonAlphaNumeric(string str)
        {
            // Cann also be used
            // str = String.Concat(Array.FindAll(str.ToCharArray(), Char.IsLetterOrDigit));
            // str = String.Concat(str.Where(char.IsLetterOrDigit));

            // With regex
            str = Regex.Replace(str, "[^a-zA-Z0-9\\s-']", String.Empty);
            return str;
        }


        /// <summary>
        /// Converts ticks into elapsed time, taking tempo changes into account
        /// Minutes, seconds, ms of seconds
        /// Ex: 6224 ticks => 00:09.152 (mm:ss.ms)
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static string TicksToTime(int ticks, double Division)
        {
            double dur = TempoUtilities.GetMidiDuration(ticks, Division);            
            double Min = (int)(dur / 60);
            double Sec = (int)(dur - (Min * 60));
            double Ms = (1000 * (dur - (Min * 60) - Sec));

            if (Math.Round(Ms) > 999)
            {                
                Ms = 0;
                Sec++;
                if (Sec > 59)
                {
                    Sec = 0;
                    Min++;
                }
            }
            
            return string.Format("{0:00}:{1:00}.{2:000}", Min, Sec, Ms);
        }

        /// <summary>
        /// Convert time to ticks in seconds
        /// 01:15.510 (min 2digits, sec 2 digits, ms 3 digits)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int TimeToTicks(string time, double Division, int max)
        {
            int tic = 0;
            
            // Caculate duration in seconds
            double dur;

            string[] split1 = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split1.Length != 2)
                return tic;

            string min = split1[0];

            string[] split2 = split1[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (split2.Length != 2)
                return tic;

            string sec = split2[0];
            string ms = split2[1];

            // Calculate dur in seconds
            int Min = Convert.ToInt32(min);
            dur = Min * 60;

            int Sec = Convert.ToInt32(sec);
            dur += Sec;

            double Ms = Convert.ToDouble(ms);
            dur += Ms / 1000;
                      
            
            // TODO
            // Find ticks who are giving this time
            // Search convergence
            tic = 0;
            string tm;            
            do
            {
                // Start with step 100
                tm = TicksToTime(tic, Division);                
                if (tm == time)
                    return tic;                
                tic += 100;

                if (TempoUtilities.GetMidiDuration(tic, Division) > dur)
                {
                    tic -= 100;
                    do
                    {
                        // Continue with step 10
                        tm = TicksToTime(tic, Division);
                        if (tm == time)
                            return tic;
                        tic +=10;

                        if(TempoUtilities.GetMidiDuration(tic, Division) > dur)
                        {                                                                                    
                            tic -= 10;
                            do
                            {
                                // Continue with step 1
                                tm = TicksToTime(tic, Division);
                                
                                // Option: take the highest tic giving the good result.
                                // If next value of tic give also the same time, continue
                                if (tm == time && TicksToTime(tic + 1, Division) != time)                               
                                    return tic;
                                
                                tic++;
                            } while (tic <= max);
                            
                        }


                    } while (tic <= max);
                }

            } while (tic <= max);


            MessageBox.Show("Unable to calculate TimeToTick", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return tic;
        }


        #region LRC

        public static List<string> LrcExtractDgRows(List<(double, string)> lstDgRows, int _LrcMillisecondsDigits, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, MidiLyricsMgmt _myLyricsMgmt = null)
        {            
            string sTime;
            double time;
            TimeSpan ts;

            string sLyric;

            bool bHasStartingLink = false;
            bool bPreviousHasTrailingLink = true;
            bool bMerge = false;

            // Put everything into a string tx
            string tx = string.Empty;
            for (int i = 0; i < lstDgRows.Count; i++)
            {
                bMerge = false;
                
                // Mifi format for timestamp is milliseconds
                // Convert from "00:01.123" to "00.01.12" if necessary
                time = lstDgRows[i].Item1;
                ts = TimeSpan.FromMilliseconds(time);
                if (_LrcMillisecondsDigits == 2)
                {
                    //time = (long)Mp3LyricsMgmtHelper.TimeToMs(sTime);                    
                    sTime = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                }
                else
                {
                    sTime = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                }

                    sTime = "[" + sTime + "]";
                sLyric = lstDgRows[i].Item2;

                if (sLyric != "" && sLyric != m_SepLine && sLyric != m_SepParagraph)
                {                   
                    
                    // Remove chords
                    if (_myLyricsMgmt != null && _myLyricsMgmt.RemoveChordPattern != null)
                        sLyric = Regex.Replace(sLyric, _myLyricsMgmt.RemoveChordPattern, @"");

                    // Remove accents
                    sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                    //Uppercase letters
                    sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                    // Lowercase letters
                    sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                    // Remove non alphanumeric chars
                    // Protect underscore
                    sLyric = sLyric.Replace("_", " ");
                    sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;
                    sLyric = sLyric.Replace(" ", "_");

                    // check if syllabes has to be merged because they belong to a word
                    bHasStartingLink = sLyric.StartsWith("_");

                    // if previous syllabe bas a trailing underscore or if the current starts with an underscore => no merge
                    if (bPreviousHasTrailingLink || bHasStartingLink)
                        bMerge = false;
                    else
                        bMerge = true;

                    // Save for next syllabe checking
                    bPreviousHasTrailingLink = sLyric.EndsWith("_");

                    // If the current lyric has not link and previous
                    if (bMerge)
                        tx += sLyric;
                    else
                        tx += sTime + sLyric;
                }
                else if (sLyric == m_SepLine || sLyric == m_SepParagraph)
                {
                    tx += sTime + sLyric;
                    // Avoid "merge = true" for the first syllabe of the next line
                    bPreviousHasTrailingLink = true;
                }                
            }


            // ===================================================
            // Split a string using a pattern (paragraph pattern)
            // ===================================================
            string pattern;
            if (_LrcMillisecondsDigits == 2)
            {
                pattern = @"\[\d{2}[:]\d{2}[.]\d{2}\]\\";
            }
            else
            {
                pattern = @"\[\d{2}[:]\d{2}[.]\d{3}\]\\";
            }

            var match = Regex.Match(tx, pattern);

            string txParagraphs = string.Empty;
            int startx = 0;

            List<string> lstParagraphs = new List<string>();

            // Search paragraphs patterns in tx
            // and create a line each time we encounter a paragraph
            while (match.Success)
            {
                if (match.Success)
                {
                    if (match.Index < tx.Length)
                        txParagraphs = tx.Substring(startx, match.Index - startx);
                    else
                        txParagraphs = tx.Substring(match.Index);

                    // Add text before paragraph
                    if (txParagraphs.Trim() != "")
                        lstParagraphs.Add(txParagraphs);

                    // Add timestamp for paragraph (& remove paragraph character)                    
                    lstParagraphs.Add(match.Value.Replace(m_SepParagraph, ""));

                    // next start
                    startx = match.Index + match.Value.Length;
                }
                match = match.NextMatch();
            };

            // add rest of string tx having no paragraph
            if (startx < tx.Length)
            {
                lstParagraphs.Add(tx.Substring(startx));
            }


            // Split each line by linefeeds
            string line;
            string[] items;
            List<string> lstLyricsItems = new List<string>();
            

            // TODO : Maybe with the same method above ?
           
            // Method 2                       
            for (int i = 0; i < lstParagraphs.Count; i++)
            {
                line = lstParagraphs[i];
                if (line.Trim() != "")
                {
                    items = line.Split('/');
                    for (int j = 0; j < items.Count(); j++)
                    {
                        lstLyricsItems.Add(items[j]);
                    }
                }
            }

            // Return value
            return lstLyricsItems;
        }


        /// <summary>
        /// LRC: Returns lyrics by lines <time, type, lyric)
        /// format: 1 timestamp + full line = 
        /// [00:03.598]
        /// [00:04.598]IT'S BEEN A HARD DAY'S NIGHT
        /// </summary>
        /// <param name="lstLyricsItems"></param>
        /// <param name="strSpaceBetween"></param>
        /// <returns></returns>
        public static List<string> GetLrcLines(List<(string, string, string)> lstLyricsItems, string strSpaceBetween)
        {
            List<string> lstLines = new List<string>();            
            string sTime;            
            string sLyric;
            string sLine = string.Empty;

            // Case full lines
            // ("[00:08.05]", "cr", "/")
            // ("[00:08.06]", "text", "/ENCORE_UN_SOIR")

            // Case syllabes
            // ("[00:04.59]", "cr", "/")
            // ("[00:04.59]", "text", "IT'S")

            string tx = string.Empty;
            for (int i = 0; i < lstLyricsItems.Count; i++)
            {
                sTime = lstLyricsItems[i].Item1;
                sLyric = lstLyricsItems[i].Item3;
                tx += sTime + sLyric;
            }
            
            // Split by "[" to have lines
            string[] lines = tx.Split('/');

            // [00:05.67]HEY [00:06.48]JUDE [00:08.51]DON'T [00:08.91]MAKE [00:09.32]IT [00:09.73]BAD
            // [00:09.73]/
            // [00:12.16]TAKE [00:12.56]A [00:12.97]SAD [00:13.78]SONG [00:15.00]AND [00:15.40]MAKE [00:15.81]IT [00:16.21]BET[00:17.02]TER
            // [00:17.02]/
            // [00:17.02]/
            // [00:19.05]RE[00:19.45]MEM[00:19.86]BER [00:20.67]TO [00:21.08]LET

            string removepattern3 = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string removepattern2 = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
            string replace = @"";

            int digits = GetDigitsLRC(lines);

            string line;
            // Treatment for each line
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                line = line.Trim();
                if (line.Length == 0) continue;
                

                if (digits == 2)
                {
                    if (line.Length < 10) continue;
                    sTime = line.Substring(0, 10);
                    line = Regex.Replace(line, removepattern2, replace);
                }
                else
                {
                    if (line.Length < 11) continue;
                    sTime = line.Substring(0, 11);
                    line = Regex.Replace(line, removepattern3, replace);
                }
                                               
                line = sTime + line.Trim(); 
                lstLines.Add(line); ;

            }

            /*
            try
            {
                // sTime, sType, sLyric
                for (int i = 0; i < lstLyricsItems.Count; i++)
                {
                    sTime = lstLyricsItems[i].Item1;                    
                    sLyric = lstLyricsItems[i].Item3;

                    if (sLyric.Trim() == m_SepParagraph)
                    {
                        // Save current line
                        if (sLine != "")
                        {
                            // Add new line
                            lstLines.Add(sLine);
                        }
                        sLine = sTime;
                        // Add new line
                        //lstLines.Add(sLine);
                        //sLine = string.Empty;
                    }
                    else if (sLyric.Trim() == m_SepLine)
                    {
                        // Save current line
                        if (sLine != "")
                        {
                            // Add new line
                            lstLines.Add(sLine);
                        }
                        sLine = sTime;
                        // Add new line
                        //lstLines.Add(sLine);
                        //sLine = string.Empty;
                    }
                    else if (sLyric.Trim().StartsWith(m_SepParagraph))
                    {
                        // Save current line
                        if (sLine != "")
                        {
                            // Add new line
                            lstLines.Add(sLine);
                        }
                        // Start a new line
                        sLyric = sLyric.Replace(m_SepParagraph, "");
                        if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                            sLyric = sLyric.Remove(0, 1);
                        sLine = sTime + strSpaceBetween + sLyric;
                    }
                    else if (sLyric.Trim().StartsWith(m_SepLine))
                    {
                        // Save current line
                        if (sLine != "")
                        {
                            // Add new line
                            lstLines.Add(sLine);
                        }
                        // Start a new line
                        sLyric = sLyric.Replace(m_SepLine, "");
                        if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                            sLyric = sLyric.Remove(0, 1);
                        sLine = sTime + strSpaceBetween + sLyric;

                    }
                    else
                    {
                        // Line continuation
                        sLine += sLyric; // only lyric for the continuation of a line   
                    }                   
                }
                // Save last line
                if (sLine != "")
                {
                    lstLines.Add(sLine);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
            }

            */

            return lstLines;
        }
      

        public static List<string> GetLrcLines(List<string> lstLyricsItems, int _LrcMillisecondsDigits)
        {
            // [00:04.59]
            // [00:04.59]IT'S[00:04.83]BEEN[00:05.05]A[00:05.27]HARD[00:06.15]DAY'S[00:06.81]NIGHT[00:08.14]
            // [00:08.14]AND[00:08.37]I'VE[00:08.60]BEEN[00:08.79]WOR[00:09.04]KING[00:09.91]LIKE[00:10.14]A[00:10.34]DOG[00:11.66]
            // [00:11.66]IT'S[00:11.88]BEEN[00:12.09]A[00:12.32]HARD[00:13.22]DAY'S[00:13.89]NIGHT[00:15.19]

            List<string> lstLines = new List<string>();
            string line;
            string sTime;
            string tx;
            string removepattern3 = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string removepattern2 = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
            string removePattern;
            int removeChars;
            string replace = @"";

            if (_LrcMillisecondsDigits == 2)
            {
                removePattern = removepattern2;
                removeChars = 10;
            }
            else
            {
                removePattern = removepattern3;
                removeChars = 11;
            }

            for (int i = 0; i < lstLyricsItems.Count; i++)
            {
                line = lstLyricsItems[i];
                if (line != "" && line.Length >= removeChars)
                {
                    sTime = line.Substring(0, removeChars);

                    // Remove all timestamps form line 
                    line = Regex.Replace(line, removePattern, replace);
                    line = sTime + line;
                    // Store result
                    lstLines.Add(line);
                }
            }
            return lstLines;
        }

        /// <summary>
        /// Find out what type of digits the file is made out
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static int GetDigitsLRC(string[] lines)
        {
            string line;
            string pattern3digits = @"(?:\[(\d{2}:\d{2}\.\d{3})\])";
            string pattern2digits = @"(?:\[(\d{2}:\d{2}\.\d{2})\])";

            // Select right pattern
            int digits3 = 0;
            int digits2 = 0;

            // Find out what type of digits the file is made of: 2 or 3
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                MatchCollection matches3digits = Regex.Matches(line, pattern3digits);
                MatchCollection matches2digits = Regex.Matches(line, pattern2digits);
                if (matches3digits.Count > 0) digits3++;
                else if (matches2digits.Count > 0) digits2++;
            }

            if (digits3 == 0 && digits2 == 0)
                return -1;

            return digits3 > digits2 ? 3 : 2;
        }


        /// <summary>
        /// Find out what type of digits the file is made out
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static string GetPatternLRC(string[] lines)
        {
            string line;
            string pattern3digits = @"(?:\[(\d{2}:\d{2}\.\d{3})\]|<(\d{2}:\d{2}\.\d{3})>)(\S+)";
            string pattern2digits = @"(?:\[(\d{2}:\d{2}\.\d{2})\]|<(\d{2}:\d{2}\.\d{2})>)(\S+)";

            // Select right pattern
            int digits3 = 0;
            int digits2 = 0;

            // Find out what type of digits the file is made of: 2 or 3
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                MatchCollection matches3digits = Regex.Matches(line, pattern3digits);
                MatchCollection matches2digits = Regex.Matches(line, pattern2digits);
                if (matches3digits.Count > 0) digits3++;
                else if (matches2digits.Count > 0) digits2++;
            }

            if (digits3 == 0 && digits2 == 0)
                return null;

            return digits3 > digits2 ? pattern3digits : pattern2digits;
        }

        /// <summary>
        /// Return lyrics by line with their timestamps
        /// Format [00:08.834]QUAND [00:09.107]J'AI [00:09.196]REN[00:09.469]CON[00:09.558]TRE [00:09.926]JO[00:10.107]SE[00:10.307]PHI[00:10.656]NE
        /// This is needed by the next function GetLrcLinesCut in order to cut a line in two lines 
        /// </summary>
        /// <param name="lstLyricsItems"></param>
        /// <param name="strSpaceBetween"></param>
        /// <returns></returns>
        public static List<string> GetLrcTimeLines(List<(string, string, string)> lstLyricsItems, int _LrcMillisecondsDigits)
        {
            List<string> lstTimeLines = new List<string>();            
            string sTime;
            string sLyric;
            string sTimeLine = string.Empty;


            string tx = string.Empty;
            for (int i = 0; i < lstLyricsItems.Count; i++)
            {
                sTime = lstLyricsItems[i].Item1;
                sLyric = lstLyricsItems[i].Item3.Replace(" ", "_");
                tx += sTime + sLyric;
            }

            tx = tx.Replace(m_SepParagraph, m_SepLine);

            // Split by "[" to have lines
            string[] lines = tx.Split('/');
            string line;
            string[] items;
            string item;
            tx = string.Empty;

            // Regex to capture timestamps and words => for milliseconds having 3 digits or 2
            // Find out what type of digits the file is made out
            int digits = GetDigitsLRC(lines);
            string pattern = GetPatternLRC(lines);
            if (pattern == null)
            {
                MessageBox.Show("Invalid lrc file, no timestamps found", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            string timestamp;
            string word;            

            for (int i = 0; i < lines.Count(); i++)
            {
                line = lines[i];
                tx = string.Empty;

                
                // Case lines with only a timestamp [00:03.12]
                if (digits == 2)
                {
                    if (line.Length == 10 && line.StartsWith("[") && line.EndsWith("]"))
                    {
                        tx = line;
                    }
                    else
                    {
                        line = line.Replace("] ", "]");  // Match does not work with lyrics having a space before it ([00:12.45] Hello)
                        line = line.Replace("[", " [");
                        line = line.Replace("  [", " [");
                        line = line.Trim();

                        MatchCollection matches2 = Regex.Matches(line, pattern);
                        if (matches2.Count == 0) continue;

                        foreach (Match match in matches2)
                        {
                            timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                            word = match.Groups[3].Value;

                            tx += "[" + timestamp + "]" + word + " ";
                        }
                    }
                }
                else if (digits == 3)
                {
                    if (line.Length == 11 && line.StartsWith("[") && line.EndsWith("]"))
                    {
                        tx = line;
                    }
                    else
                    {
                        line = line.Replace("] ", "]");  // Match does not work with lyrics having a space before it ([00:12.45] Hello)
                        line = line.Replace("[", " [");
                        line = line.Replace("  [", " [");
                        line = line.Trim();

                        MatchCollection matches3 = Regex.Matches(line, pattern);
                        if (matches3.Count == 0) continue;

                        foreach (Match match in matches3)
                        {
                            timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                            word = match.Groups[3].Value;

                            tx += "[" + timestamp + "]" + word + " ";
                        }
                    }
                }

                
                lstTimeLines.Add(tx);
            }

            /*
            try
            {
                // sTime, sType, sLyric
                for (int i = 0; i < lstLyricsItems.Count; i++)
                {
                    sTime = lstLyricsItems[i].Item1;
                    //sType = lstLyricsItems[i].Item2;
                    sLyric = lstLyricsItems[i].Item3;


                    if (sLyric.Trim() == m_SepParagraph)
                    {
                        if (sTimeLine != "")
                        {
                            // Add new line
                            lstTimeLines.Add(sTimeLine);
                        }
                        sTimeLine = sTime;
                    }
                    else if (sLyric.Trim() == m_SepLine)
                    {
                        if (sTimeLine != "")
                        {
                            // Add new line
                            lstTimeLines.Add(sTimeLine);
                        }
                        sTimeLine = sTime;
                    }
                    else if (sLyric.Trim().StartsWith(m_SepParagraph))
                    {
                        if (sTimeLine != "")
                        {
                            // Add new line
                            lstTimeLines.Add(sTimeLine);
                        }
                        sLyric = sLyric.Replace(m_SepParagraph, "");
                        sTimeLine = sTime + strSpaceBetween + sLyric;
                    }
                    else if (sLyric.Trim().StartsWith(m_SepLine))
                    {
                        if (sTimeLine != "")
                        {
                            // Add new line
                            lstTimeLines.Add(sTimeLine);
                        }
                        sLyric = sLyric.Replace(m_SepLine, "");
                        sTimeLine = sTime + strSpaceBetween + sLyric;
                    }
                    else
                    {
                        // Line continuation
                        // Case of spaces at the left of the lyrics
                        // Add a space to the left to allow split by space
                        if (sLyric.Length > 0 && sLyric.StartsWith(" "))
                            sTimeLine += " " + sTime + strSpaceBetween + sLyric.Remove(0, 1);
                        else
                            sTimeLine += sTime + strSpaceBetween + sLyric;
                    }
                }

                // Save last line
                if (sTimeLine != "")
                {
                    // Remove last space
                    if (sTimeLine.Length > 0 && sTimeLine.EndsWith(" "))
                        sTimeLine = sTimeLine.Remove(sTimeLine.Length - 1, 1);
                    lstTimeLines.Add(sTimeLine);
                }

            }
            catch (Exception e) { MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            */

            return lstTimeLines;
        }

        public static List<string> GetLrcTimeLines(string[] lstLyricsItems, int _LrcMillisecondsDigits)
        {
            List<string> lstTimeLines = new List<string>();
            string line;
            string sTime;
            string[] items;
            string item;
            string tx;

            string pattern3digits = @"(?:\[(\d{2}:\d{2}\.\d{3})\]|<(\d{2}:\d{2}\.\d{3})>)(\S+)";
            string pattern2digits = @"(?:\[(\d{2}:\d{2}\.\d{2})\]|<(\d{2}:\d{2}\.\d{2})>)(\S+)";
            string pattern;

            string removepattern3 = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string removepattern2 = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
            string removePattern;
            int removeChars;
            string replace = @"";

            string timestamp;
            string word;

            if (_LrcMillisecondsDigits == 2)
            {
                removeChars = 10;
                removePattern = removepattern2;
                pattern = pattern2digits;
            }
            else
            {
                removeChars = 11;
                removePattern = removepattern3;
                pattern = pattern3digits;
            }


            for (int i = 0; i < lstLyricsItems.Count(); i++)
            {
                // Clean
                line = lstLyricsItems[i];
                sTime = line.Substring(0, removeChars);

                tx = string.Empty;

                if (line.Length > removeChars)
                {
                    line = line.Replace("] ", "]");  // Match does not work with lyrics having a space before it ([00:12.45] Hello)
                    line = line.Replace("[", " [");
                    line = line.Replace("  [", " [");
                    line = line.Trim();

                    MatchCollection matches = Regex.Matches(line, pattern);
                    if (matches.Count == 0) continue;

                    foreach (Match match in matches)
                    {
                        timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                        word = match.Groups[3].Value;

                        tx += "[" + timestamp + "]" + word + " ";
                    }

                    line = tx;
                }
                

                lstTimeLines.Add(line);
            }

            return lstTimeLines;
        }

        public static List<string> GetLrcTimeLines(List<string> lstLyricsItems, int _LrcMillisecondsDigits)
        {
            List<string> lstTimeLines = new List<string>();
            string line;
            string sTime;
            string tx;

            string pattern3digits = @"(?:\[(\d{2}:\d{2}\.\d{3})\]|<(\d{2}:\d{2}\.\d{3})>)(\S+)";
            string pattern2digits = @"(?:\[(\d{2}:\d{2}\.\d{2})\]|<(\d{2}:\d{2}\.\d{2})>)(\S+)";
            string pattern;

            string removepattern3 = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string removepattern2 = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
            string removePattern;
            int removeChars;
            string timestamp;
            string word;

            if (_LrcMillisecondsDigits == 2)
            {
                removeChars = 10;
                removePattern = removepattern2;
                pattern = pattern2digits;
            }
            else
            {
                removeChars = 11;
                removePattern = removepattern3;
                pattern = pattern3digits;
            }
            
            for (int i = 0; i < lstLyricsItems.Count; i++)
            {
                // Clean
                line = lstLyricsItems[i];

                if (line.Length > removeChars)
                {
                    sTime = line.Substring(0, removeChars);
                    tx = string.Empty;

                    line = line.Replace("] ", "]");  // Remove space after ] : Match does not work with lyrics having a space before it ([00:12.45] Hello)
                    line = line.Replace("[", " [");  // Add a space before [
                    line = line.Replace("  [", " [");  // Clean if added space was useless
                    line = line.Trim();
                    
                    // initial [00:04.59]It's[00:04.83]_been[00:05.05]_a[00:05.27]_hard[00:06.15]_day's[00:06.81]_night[00:08.14]
                    // result [00:04.59]It's [00:04.83]_been [00:05.05]_a [00:05.27]_hard [00:06.15]_day's [00:06.81]_night [00:08.14]

                    MatchCollection matches = Regex.Matches(line, pattern);
                    if (matches.Count == 0) continue;

                    foreach (Match match in matches)
                    {
                        timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                        word = match.Groups[3].Value;

                        tx += "[" + timestamp + "]" + word + " ";
                    }
                    line = tx;
                }
                lstTimeLines.Add(line);
            }
            return lstTimeLines;
        }


        /// <summary>
        /// Return lyrics by line and cut lines to MaxLength characters
        /// [00:04.598]IT'S BEEN A HARD DAY'S NIGHT
        /// [00:08.148]AND I'VE BEEN WORKING LIKE A
        /// [00:10.349]DOG
        /// </summary>
        /// <param name="lstTimeLines"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static List<string> GetLrcLinesCut(List<string> lstTimeLines, int MaxLength, int _LrcMillisecondsDigits)
        {
            List<string[]> lstWords = new List<string[]>();
            List<string[]> lstTimes = new List<string[]>();

            string sTimeLine;
            string strPartialLine;
            string sLine;
            bool bStartLine;
            string sLyric;
            string sTime;

            string[] words;
            string[] Times;
            string removepattern3 = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
            string removepattern2 = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
            string removepattern;
            string replace = @"";
            int removeChars; ;

            List<string> lstLinesCut = new List<string>();

            if (_LrcMillisecondsDigits == 2) 
            {
                removepattern = removepattern2;
                removeChars = 10;
            }
            else
            {
                removepattern = removepattern3;
                removeChars = 11;
            }

            try
            {
                for (int i = 0; i < lstTimeLines.Count; i++)
                {
                    sTimeLine = lstTimeLines[i].Trim();

                    // Remove underscores ???
                    //sTimeLine = sTimeLine.Replace("_", "");

                    MatchCollection mc = Regex.Matches(sTimeLine, removepattern);

                    // pb 'bet' 'ter' is a single word, 2 syllabes should be merged in better
                    //[00:12.16]Take_ [00:12.56]a_ [00:12.97]sad_ [00:13.78]song_ [00:15.00]and_ [00:15.40]make_ [00:15.81]it_ [00:16.21]bet [00:17.02]ter._"

                    // Split by space character
                    words = sTimeLine.Split(' ');                   
                    Times = new string[words.Length];

                    if (mc.Count > 0)
                    {
                        for (int j = 0; j < words.Length; j++)
                        {
                            if (words[j].Length >= removeChars)
                            {
                                Times[j] = words[j].Substring(0, removeChars);
                                words[j] = Regex.Replace(words[j], removepattern, replace);
                            }
                        }
                    }                        
                    lstWords.Add(words);
                    lstTimes.Add(Times);
                }

                // Manage length
                // for each line, test if its length is greater than MaxLength
                // If yes, create a second line
                strPartialLine = string.Empty;   // Words of a line without timestamps in order to estimate length
                sLine = string.Empty;
                string[] ItemsW;
                string[] ItemsT;
                for (int i = 0; i < lstWords.Count; i++)
                {
                    ItemsT = lstTimes[i];
                    ItemsW = lstWords[i];
                    sLine = string.Empty;

                    for (int j = 0; j < ItemsW.Count(); j++)
                    {
                        bStartLine = (j == 0);
                        sLyric = ItemsW[j];
                        sTime = ItemsT[j];

                        if (!bStartLine && (strPartialLine + " " + sLyric).Length > MaxLength)
                        {
                            // if length of words is greater than MaxLength
                            // Remove last space
                            if (sLine.Length > 0 && sLine.EndsWith(" "))
                                sLine = sLine.Remove(sLine.Length - 1, 1);
                            lstLinesCut.Add(sLine);

                            // Restart a new line
                            sLine = sTime + sLyric; // + " ";
                            strPartialLine = sLyric; // + " ";

                        }
                        else
                        {
                            if (bStartLine)
                            {
                                sLine = sTime + sLyric; // + " ";
                                strPartialLine = sLyric; // + " ";
                            }
                            else
                            {
                                sLine += sLyric; // + " ";
                                strPartialLine += sLyric; // + " ";
                            }
                        }
                    }

                    // Remove last space
                    if (sLine.Length > 0 && sLine.EndsWith(" "))
                        sLine = sLine.Remove(sLine.Length - 1, 1);
                    lstLinesCut.Add(sLine);
                    sLine = string.Empty;
                }

                // Save last line
                if (sLine != string.Empty)
                {
                    // Remove last space
                    if (sLine.Length > 0 && sLine.EndsWith(" "))
                        sLine = sLine.Remove(sLine.Length - 1, 1);
                    lstLinesCut.Add(sLine);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
            }
            return lstLinesCut;
        }      


        /// <summary>
        /// Extraxt artist and song frem file name
        /// </summary>
        /// <param name="fPath"></param>
        /// <returns></returns>
        public static List<string> GetTagsFromFileName(string fPath)
        {
            // Classic Karaoke Midi tags
            /*
            @K	(multiple) K1: FileType ex MIDI KARAOKE FILE, K2: copyright of Karaoke file
            @L	(single) Language	FRAN, ENGL        
            @W	(multiple) Copyright (of Karaoke file, not song)        
            @T	(multiple) Title1 @T<title>, Title2 @T<author>, Title3 @T<copyright>		
            @I	Information  ex Date(of Karaoke file, not song)
            @V	(single) Version ex 0100 ?             
            */
            List<string> lstTags = new List<string>();
            string Tag_Artist = string.Empty;
            string Tag_Title = string.Empty;
            int pos;
            
            string fName = Path.GetFileNameWithoutExtension(fPath);
            string sep = " - ";

            try
            {
                if (fName.IndexOf(sep) == -1)
                {
                    Tag_Title = fName;
                    Tag_Artist = "";
                }
                else
                {
                    pos = fName.IndexOf(sep);           // First index
                    Tag_Artist = fName.Substring(0, pos);
                    Tag_Title = fName.Substring(pos + sep.Length, fName.Length - pos - sep.Length);
                }
            }
            catch (Exception e) 
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }

            return new List<string> { Tag_Artist, Tag_Title };
        }


        #region save lrc

        /// <summary>
        /// Save Lyrics .lrc file format and by lines
        /// </summary>
        /// <param name="File"></param>
        /// <param name="lstDgRows"></param>
        /// <param name="bRemoveAccents"></param>
        /// <param name="bUpperCase"></param>
        /// <param name="bLowerCase"></param>
        /// <param name="bRemoveNonAlphaNumeric"></param>
        /// <param name="Tag_Tool"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        /// <param name="bControlLength"></param>
        /// <param name="MaxLength"></param>
        /// <param name="_LrcMillisecondsDigits"></param>
        /// <param name="_myLyricsMgmt"></param>
        public static void SaveLRCLines(string File, List<(double, string)> lstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus, bool bControlLength, int MaxLength, int _LrcMillisecondsDigits, MidiLyricsMgmt _myLyricsMgmt = null)
        {
            string sLine;

            string lrcs;
            string cr = "\r\n";

            #region meta data

            // List to store lines
            List<string> lstHeaderLines = new List<string>();

            // Store meta datas
            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "Tool:", "Ti:", "Ar:", "Al:", "La:", "By:", "D+:" };
            string Tag;
            string TagName;
            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                {
                    sLine = "[" + TagName + Tag + "]";
                    lstHeaderLines.Add(sLine);
                }
            }
            #endregion meta data


            // Make treatment of lyrics (same for frmLyricsEdit and frmMp3LyricsEdit)
            List<string> lstLyricsItems = Utilities.LyricsUtilities.LrcExtractDgRows(lstDgRows, _LrcMillisecondsDigits, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _myLyricsMgmt);

            // Store lyrics in lines (remove timestamps from lines, except for the first word)
            // [00:04.59]It's_been_a_hard_day's_night
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store timestamps + lyrics in lines (add spaces if not existing)
            // initial [00:04.59]It's[00:04.83]_been[00:05.05]_a[00:05.27]_hard[00:06.15]_day's[00:06.81]_night[00:08.14]
            // result [00:04.59]It's [00:04.83]_been [00:05.05]_a [00:05.27]_hard [00:06.15]_day's [00:06.81]_night [00:08.14]
            List<string> lstTimeLines = Utilities.LyricsUtilities.GetLrcTimeLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store lyrics by line and cut lines to MaxLength characters using lstTimeLines
            List<string> lstLinesCut = new List<string>();
            if (bControlLength)
            {
                lstLinesCut = Utilities.LyricsUtilities.GetLrcLinesCut(lstTimeLines, MaxLength, _LrcMillisecondsDigits);
            }


            #region send all to string 

            // Header
            lrcs = string.Empty;
            for (int i = 0; i < lstHeaderLines.Count; i++)
            {
                lrcs += lstHeaderLines[i] + cr;
            }

            // Select cut or not cut
            if (bControlLength)
            {
                // If cut lines to 32 chars
                for (int i = 0; i < lstLinesCut.Count; i++)
                {
                    lrcs += lstLinesCut[i].Replace("_", " ").Replace("] ", "]") + cr;
                }
            }
            else
            {
                // No cut
                for (int i = 0; i < lstLines.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"                    
                    lrcs += lstLines[i].Replace("]_", "]").Replace(" ", "").Replace("_", " ") + cr;
                }
            }
            #endregion send all to string


            // Open file
            #region open file
            try
            {
                System.IO.File.WriteAllText(File, lrcs);
                System.Diagnostics.Process.Start(@File);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion open file
        }



        #endregion save lrc


        #endregion LRC


    }


}

