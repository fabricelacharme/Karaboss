#region License

/* Copyright (c) 2026 Fabrice Lacharme
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

using FlShell.Interop;
using Karaboss.MidiLyrics;
using Karaboss.Mp3.Mp3Lyrics;
using Karaboss.SRT;
using Mozilla.NUniversalCharDet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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

        #region common functions

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
                // Start with step 1000
                tm = TicksToTime(tic, Division);
                
                if (tm == time)
                    return tic;
                tic += 1000;




                // If the last increase of 1000 is too big
                if (tic > max || TempoUtilities.GetMidiDuration(tic, Division) > dur)
                {
                    // Go back to previous value
                    tic -= 1000;

                    do 
                    { 
                        // Continue with step 100
                        tm = TicksToTime(tic, Division);
                        if (tm == time)
                            return tic;
                        tic += 100;

                        // If the last increase of 100 is too big
                        if (tic > max || TempoUtilities.GetMidiDuration(tic, Division) > dur)
                        {
                            // Go back to previous value
                            tic -= 100;
                            do
                            {
                                // Continue to increase with a step of 10
                                tm = TicksToTime(tic, Division);
                                if (tm == time)
                                    return tic;
                                tic += 10;

                                // If the last increase of 10 is too big
                                if (tic > max || TempoUtilities.GetMidiDuration(tic, Division) > dur)
                                {
                                    int maxistep = tic; // Next line, we decrease tic by 10. No need to go above maxistep 
                                    int besttry = -1;

                                    // Go back to the previous value
                                    tic -= 10;
                                    do
                                    {
                                        // Continue with a step of 1
                                        tm = TicksToTime(tic, Division);

                                        // Option: take the highest tic giving the right result.
                                        // If next value of tic give also the same time, continue
                                        if (tm == time && TicksToTime(tic + 1, Division) != time)
                                            return tic;

                                        // If it is not possible to get a valid tick for time
                                        // keep the nearest value 
                                        // This situation occurs with LRC files having only 2 digits for milliseconds
                                        //if (TimeToMs(tm) - TimeToMs(time) >= 1 && besttry == -1)
                                        //    besttry = tic;

                                        if (Math.Abs(TimeToMs(time) - TimeToMs(tm)) == 1)
                                            besttry = tic;


                                        if (tic <= maxistep) // no need to go further maxistep 
                                            tic++;
                                        else
                                        {
                                            if (besttry > -1)
                                                return besttry;
                                            else
                                                return tic;
                                        }

                                    } while (tic <= max);

                                }


                            } while (tic <= max);
                        }
                    
                    
                    } while (tic <= max);
                }

            } while (tic <= max);


            MessageBox.Show("Unable to calculate TimeToTick for this timestamp: " + time, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Return -1 if not found, but it should never happen because we are sure to find a tic with the same time as the one we are looking for (we are sure to find it with step 1)
            return -1;

        }


        /// <summary>
        /// Convert a time stamp 01:15.510 (min 2digits, sec 2 digits, ms 2 or 3 digits) to milliseconds
        /// </summary>
        /// <param name="stime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double TimeToMs(string time)
        {
            string pattern3digits = @"(?:(\d{2}:\d{2}\.\d{3}))";
            string pattern2digits = @"(?:(\d{2}:\d{2}\.\d{2}))";

            double dur = 0;

            MatchCollection matches3digits = Regex.Matches(time, pattern3digits);
            MatchCollection matches2digits = Regex.Matches(time, pattern2digits);


            string[] split1 = time.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (split1.Length != 2)
                return 0;

            string min = split1[0];

            string[] split2 = split1[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (split2.Length != 2)
                return 0;

            string sec = split2[0];
            string ms = split2[1];

            // Calculate dur in seconds
            int Min = Convert.ToInt32(min);
            dur = Min * 60 * 1000;

            int Sec = Convert.ToInt32(sec);
            dur += Sec * 1000;

            double Ms;
            if (matches3digits.Count > 0)
                Ms = Convert.ToDouble(ms);
            else
                Ms = Convert.ToDouble(ms) * 10;

            dur += Ms;

            return dur;
        }


        /// <summary>
        /// Convert milliseconds to a LRC timespan
        /// </summary>
        /// <param name="ms"></param>
        /// <param name="_LrcMillisecondsDigits"></param>
        /// <returns></returns>
        public static string MsToTime(double ms, int _LrcMillisecondsDigits)
        {
            int mls;
            int sec;
            int min;

            TimeSpan ts = TimeSpan.FromMilliseconds(ms);

            if (_LrcMillisecondsDigits == 2)
            {
                mls = (int)Math.Round(ts.Milliseconds / (double)10);
                sec = ts.Seconds;
                min = ts.Minutes;
                if (mls == 100)
                {
                    mls = 0;
                    sec += 1;
                    if (sec == 60)
                        min += 1;
                }
                return string.Format("{0:00}:{1:00}.{2:00}", min, sec, mls);
            }
            else
                return string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
        }

        /// <summary>
        /// Converts a time duration, specified in milliseconds, to the SubRip Subtitle (SRT) time format.
        /// </summary>
        /// <remarks>The returned string is formatted according to the SRT subtitle standard, which is
        /// commonly used for video subtitles. The method does not validate that the input is within a typical SRT time
        /// range; negative values may produce unexpected results.</remarks>
        /// <param name="ms">The duration in milliseconds to convert. Must be a non-negative value.</param>
        /// <returns>A string representing the equivalent time in SRT format ("HH:MM:SS,mmm").</returns>
        public static string MsToSrtTime(double ms)
        {
            // Convert milliseconds to SRT time format "00:00:01,848"
            TimeSpan ts = TimeSpan.FromMilliseconds(ms);

            return string.Format("{0:00}:{1:00}:{2:00},{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

        }


        /// <summary>
        /// Returns default encoding saved in Properties.settings.Default
        /// </summary>
        /// <returns>(UTF8 (default) or ANSI (1252)</returns>
        public static Encoding GetDefaultEncoding()
        {
            string DefaultEncoding = "UTF8";
            if (Properties.Settings.Default.DefaultEncoding != null)
                DefaultEncoding = Properties.Settings.Default.DefaultEncoding;

            switch (DefaultEncoding)
            {
                case "UTF8":
                    return Encoding.UTF8;

                case "ANSI":
                    //return Encoding.GetEncoding("iso-8859-1");
                    return Encoding.GetEncoding("Windows-1252");

                default:
                    return Encoding.UTF8;
            }
        }

        /// <summary>
        /// Determines the character encoding of a specified file based on its content.
        /// </summary>
        /// <remarks>This method uses a universal encoding detector to analyze the file's byte data and
        /// identify the character set. If the detected encoding is not supported, the method defaults to
        /// UTF-8.</remarks>
        /// <param name="filename">The path to the file for which the encoding is to be detected. The file must exist and be accessible.</param>
        /// <returns>An Encoding object representing the detected character encoding of the file. If the encoding cannot be
        /// determined, UTF-8 is returned as a fallback.</returns>
        public static Encoding GetEncodingFromFile(string filename)
        {
            try
            {
                // Read file into a byte array
                byte[] data = File.ReadAllBytes(filename);
                // UTF-8
                int detEncoding = 65001;

                UniversalDetector Det = new UniversalDetector(null);
                Det.HandleData(data, 0, data.Length);
                Det.DataEnd();
                string enc = Det.GetDetectedCharset();
                if (enc != null && enc != "Not supported")
                {
                    // fix encoding for 1251 upper case and MAC
                    //if (enc == "KOI8-R" || enc == "X-MAC-CYRILLIC") { enc = "WINDOWS-1251"; }
                    Encoding denc = Encoding.GetEncoding(enc);
                    detEncoding = denc.CodePage;
                }
                return Encoding.GetEncoding(detEncoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Encoding.UTF8;
            }
        }


        #endregion commun functions


        /// <summary>
        /// Reads the contents of the data grid and extracts a list of time and lyric pairs, filtering out invalid or
        /// improperly formatted entries.
        /// </summary>
        /// <remarks>The method ensures that only rows with valid time formats and non-empty lyrics are
        /// included. It also normalizes certain characters in the lyrics to maintain consistency with expected
        /// formats.</remarks>
        /// <returns>A list of tuples, each containing the time in milliseconds and the corresponding lyric string. The list
        /// excludes rows with missing or invalid data.</returns>
        public static List<(double Time, string lyric)> ReadDataGridContent(DataGridView dgView, int colTime, int colText)
        {
            // Col time must be the column containing times in the format "00:00.000"

            List<(double Time, string lyric)> Result = new List<(double Time, string lyric)>();

            string sTime;
            double time;
            string sLyric;

            object vLyric;
            object vTime;

            // Verify format of time "00:00.000"
            // can be also "00:00.00" !!!
            string pattern3digits = @"\d{2}:\d{2}.\d{3}";
            string pattern2digits = @"\d{2}:\d{2}.\d{2}";

            string AllLyrics = string.Empty;

            // Store rows of dgView in a list
            // the aim is to have the same procedure between midi Lyrics edition and mp3 Lyrics edition            
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vTime = dgView.Rows[i].Cells[colTime].Value;
                vLyric = dgView.Rows[i].Cells[colText].Value;

                if (vLyric == null) continue;
                if (vTime == null) continue;

                // Don not keep empty cells ?
                if (vTime.ToString().Trim() == "") continue;
                if (vLyric.ToString().Trim() == "") continue;

                sTime = vTime.ToString().Trim();

                // Verify format of time "00:00.000" or "00:00.00"
                var match2digits = Regex.Match(sTime, pattern2digits);
                var match3digits = Regex.Match(sTime, pattern3digits);
                if (!match2digits.Success && !match3digits.Success) continue;

                // Convert times to milliseconds (to have the same entry format with mp3 Lyrics edition)
                //time = Mp3LyricsMgmtHelper.TimeToMs(sTime);
                time = LyricsUtilities.TimeToMs(sTime);

                // Clean separators for the format of separator on single lines
                sLyric = vLyric.ToString();
                if (sLyric.Trim() == m_SepLine) sLyric = m_SepLine;
                if (sLyric.Trim().Trim() == m_SepParagraph) sLyric = m_SepParagraph;

                // Do not keep if first cell is a separator
                if (AllLyrics.Length == 0 && sLyric == m_SepLine) continue;
                if (AllLyrics.Length == 0 && sLyric == m_SepParagraph) continue;


                // Eliminate some characters
                sLyric = sLyric.Replace("_", " ");
                if (sLyric.Trim().Length == 0) continue;

                // Replace characters used in LRC format
                sLyric = sLyric.Replace("[", "@");
                sLyric = sLyric.Replace("]", "@");
                sLyric = sLyric.Replace("<", "@");
                sLyric = sLyric.Replace(">", "@");


                // Case of sparators on the same line of the lyrics (MP3 module)
                if (sLyric != m_SepParagraph && sLyric.IndexOf(m_SepParagraph) != -1)
                {
                    // add a new line with a paragraph (except first line)
                    if (AllLyrics.Length > 0)
                        Result.Add((time, m_SepParagraph));

                    sLyric = sLyric.Replace(m_SepParagraph, "");
                }
                else if (sLyric != m_SepLine && sLyric.IndexOf(m_SepLine) != -1)
                {
                    // Add a new line with a linefeed (except first line)
                    if (AllLyrics.Length > 0)
                        Result.Add((time, m_SepLine));

                    sLyric = sLyric.Replace(m_SepLine, "");
                }


                // Add to result
                Result.Add((time, sLyric));
                AllLyrics += sLyric;
            }

            // At this level we can have several following linefeeds and or Paragraphs, they will be eliminated further
            // in CreateLrcLines
            return Result;
        }


        public class LyricsItem
        {
            public string Time { get; set; }         
            public string Lyric { get; set; }
        }


        public static List<List<LyricsItem>> ExtractDgRows(List<(double Time, string lyric)> lstDgRows, int _LrcMillisecondsDigits)
        {
            string sTime;
            double time;
            string sLyric;
            List<List<LyricsItem>> lstLyricsItems = new List<List<LyricsItem>>();
            List<LyricsItem> lstLyricsItemsLine = new List<LyricsItem>();

            for (int i = 0; i < lstDgRows.Count; i++)
            {
                time = lstDgRows[i].Time;
                sTime = MsToTime(time, _LrcMillisecondsDigits);
                sLyric = lstDgRows[i].lyric;

                if (sLyric == "") continue;

                // Check if sLyric contains a line or paragraph separator
                if (sLyric.Contains(m_SepLine) || sLyric.Contains(m_SepParagraph))
                {
                    if (sLyric == m_SepLine)
                    {
                        // If the first thing of the lyrics is a separator, continue to next line, otherwise we will have an empty line at the beginning of the lyrics
                        if (lstLyricsItems.Count == 0 && lstLyricsItemsLine.Count == 0) continue;

                        // sLyric is a pure line separator                        
                        // It means that the next syllabe will be on a new line for lstLyricsItems and not on the same line as previous syllabes
                        // Add previous line to list of lyrics items
                        lstLyricsItems.Add(lstLyricsItemsLine);
                        // Start a new line
                        lstLyricsItemsLine = new List<LyricsItem>();
                    }
                    else if (sLyric == m_SepParagraph)
                    {
                        // If the first thing of the lyrics is a separator, continue to next line, otherwise we will have an empty line at the beginning of the lyrics
                        if (lstLyricsItems.Count == 0 && lstLyricsItemsLine.Count == 0) continue;


                        // sLyric is a pure paragraph separator
                        // Same as for line separator but with a new paragraph instead of a new line
                        // So we have to add an empty line for the new line and then add the new paragraph line
                        lstLyricsItems.Add(lstLyricsItemsLine);
                        lstLyricsItemsLine = new List<LyricsItem>();

                        lstLyricsItemsLine.Add(new LyricsItem { Time = sTime, Lyric = " " });
                        lstLyricsItems.Add(lstLyricsItemsLine);
                        lstLyricsItemsLine = new List<LyricsItem>();

                    }
                    else
                    {
                        // sLyric contains a line or paragraph separator, split it and add each part to the list of lyrics items with its timestamp
                        // Manage only lines starting with a separator
                        // Ex: "/ENCORE_UN_SOIR" => this is a new line for lstLyricsItems
                        lstLyricsItems.Add(lstLyricsItemsLine);
                        lstLyricsItemsLine = new List<LyricsItem>();
                        string[] parts = sLyric.Split(new char[] { m_SepLine[0], m_SepParagraph[0] }, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < parts.Length; j++)
                        {
                            lstLyricsItemsLine.Add(new LyricsItem { Time = sTime, Lyric = parts[j] });
                        }

                    }
                }
                else
                {
                    // sLyric does not contain any line or paragraph separator, add it to the list of lyrics items with its timestamp
                    lstLyricsItemsLine.Add(new LyricsItem { Time = sTime, Lyric = sLyric });
                }
            }

            // Add last line if not empty
            if (lstLyricsItemsLine.Count > 0)
            {
                lstLyricsItems.Add(lstLyricsItemsLine);
            }

            return lstLyricsItems;
        }


        /// <summary>
        /// Saves the specified string content to a file at the given path and optionally opens the file after saving.
        /// </summary>
        /// <remarks>If the file already exists at the specified path, it is deleted before writing the
        /// new content to ensure the correct encoding is applied. The method uses the application's default encoding
        /// when writing the file.</remarks>
        /// <param name="fullPath">The full file path, including the file name, where the content will be saved. Cannot be null or empty.</param>
        /// <param name="Content">The string content to write to the file.</param>
        /// <param name="bShow">A value indicating whether to open the file after saving. If set to <see langword="true"/>, the file is
        /// opened after it is saved; otherwise, it is not.</param>
        public static void SaveStringToFile(string fullPath, string Content, bool bShow = false)
        {
            try
            {
                Encoding encoding = GetDefaultEncoding();

                // Strange behaviour: if the file is not deleted before saving, encoding is not applied
                // It seems that WriteAllText append data to the existing file
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);

                System.IO.File.WriteAllText(fullPath, Content, encoding);

                if (bShow ) 
                    System.Diagnostics.Process.Start(@fullPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region LRC


        #region import LRC
      
        public static List<List<keffect.KaraokeEffect.kSyncText>> ReadLrcFromFile(string FileName)
        {            
            // Cases 
            // A line with a single timestamp like "[00:44.22]" is a paragraph                                                      => OK
            // Empty line: is it a paragraph ?                                                                                      => NOT TREATED, line is removed
            // Use of "[ ]" characters in the lyrics: [02:16.09]Qui vont danser le chachacha [x4]                                   => NOT TREATED, ([x4] is removed)
            // Repetitions of lyrics: [01:15.21][02:04.21][02:11.09][02:28.46][02:35.34][02:42.28][02:49.21][03:06.53]Mamma mia,    => OK
            // Repetitions of paragraphs: [01:15.21][02:04.21][02:11.09][02:28.46][02:35.34][02:42.28][02:49.21][03:06.53]          => OK

            #region guard
            // Search for existing LRC file
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;
            #endregion guard

            try
            {
                // Detect encoding                               
                Encoding enc = GetEncodingFromFile(FileName);

                // Read lines with encoding
                string[] lines = System.IO.File.ReadAllLines(FileName, enc);
                if (lines.Count() == 0)
                {
                    MessageBox.Show("Invalid LRC file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }


                // Search type of timestamp format for milliseconds (2 or 3 digits)
                int digits = GetDigitsLRC(lines);
                if (digits == -1)
                {
                    MessageBox.Show("Invalid LRC file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                string patterntime;
                if (digits == 2)
                {
                    patterntime = @"\[\d{2}[:]\d{2}[.]\d{2}\]";
                }
                else
                {
                    patterntime = @"\[\d{2}[:]\d{2}[.]\d{3}\]";
                }

                var regextime = new Regex(patterntime);

                string patternline = GetPatternLRC(lines);
                if (patternline == null)
                {
                    MessageBox.Show("Invalid LRC file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }


                List<(string, string)> result = new List<(string, string)>();

                List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();
                long time;
                string lyric;
                string timestamp;
                string element;

                var regexCrotchets = new Regex(@"(?<=\[).*?(?=\])");

                string line;
                for (int i = 0; i < lines.Count(); i++)
                {
                    line = lines[i];
                    // Eliminate empty lines
                    if (line.Trim() == string.Empty) continue;

                    var matchcrotchets = regexCrotchets.Match(line);
                    if (!matchcrotchets.Success) continue;

                    // Lyric is after the last crotchet "]" 
                    lyric = line.Split(']').Last();

                    if (lyric.Length == 0)
                    {

                        var matchtime = Regex.Match(line, patterntime);
                        if (matchtime.Success)
                        {
                            // Case 1: single timestamp [00:33.84]
                            // Case 2: numerous timestamps [00:33.84][00:55.47][01:08.41][01:29.12][01:42.06][01:53.08][02:15.08]

                            string[] paragraphs = line.Split('[').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                            // Bug in [02:16.09]Qui vont danser le chachacha [x4] 
                            // This is like [02:16.09][03:22.00]
                            // Result is 2 lines "02:16.09]Qui vont danser le chachacha " and "x4]"                           

                            for (int j = 0; j < paragraphs.Length; j++)
                            {                                                                
                                timestamp = paragraphs[j];
                                lyric = timestamp;

                                if (timestamp.Length == 0) continue;
                                if (!timestamp.Contains("]")) continue;
                                timestamp = timestamp.Substring(0, timestamp.IndexOf("]"));                               
                                // Timestamp must look like a timestamp (so unfortunately, it eliminates lyrics like "[x4]"
                                if (!Regex.IsMatch(timestamp, @"(?:^\d{2}:\d{2}.\d{2})") && !Regex.IsMatch(timestamp, @"(?:^\d{2}:\d{2}.\d{3})")) continue;

                                lyric = lyric.Substring(lyric.IndexOf("]") + 1);

                                result.Add((timestamp,lyric));
                            }
                            
                        }
                        else
                        {
                            // Else metadata: do nothing
                        }
                    }
                    else
                    {
                        // Lines with one or several timestamps and text
                        var matchline = Regex.Match(line, patternline);
                        if (!matchline.Success) continue;

                        // Search for all timestamps                        

                        foreach (var match in regexCrotchets.Matches(line))
                        {
                            timestamp = match.ToString();
                            result.Add((timestamp, lyric));
                        }
                    }
                }

                // Sort SyncLyrics by time
                // In case of repetead lines like: [00:16.42][00:44.90][01:13.21][01:41.51]Comme dans un film de la Metro,"
                // We have to sort the result by the first item
                List<(string, string)> sortedList = new List<(string, string)>();
                sortedList = result.OrderBy(o => o.Item1).ToList();


                // Now we have to separate the syllables                                               
                for (int i = 0; i < sortedList.Count; i++)
                {
                    time = (long)TimeToMs(sortedList[i].Item1);

                    lyric = (string)sortedList[i].Item2;
                    int s = lyric.IndexOf("<");

                    // This is enhanced LRC like "[00:01.03]La <00:01.15>petite <00:01.48>maison"
                    if (s > -1)
                    {
                        // firstsyllabe is "La"
                        string firstsyllabe = lyric.Substring(0, s);

                        // Case use of "<" character in the lyrics without timestamp like "<00:01.04>"
                        // Example: [00:00.09]<º))))><.·´¯`·..Lugosh..·´¯`·.><((((º>
                        if (firstsyllabe.Length == 0)
                        {
                            SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                            SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, lyric));
                            SyncLyrics.Add(SyncLine);
                        }
                        else
                        {
                            // This is enhanced LRC like "[00:01.03]La <00:01.15>petite <00:01.48>maison"
                            // restline is <00:01.15>petite <00:01.48>maison
                            string restline = lyric.Substring(s);

                            SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                            SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, firstsyllabe));

                            // Search for all pairs of <00:00.00>text
                            string[] elements = restline.Split('<');
                            
                            for (int j = 0; j < elements.Count(); j++)
                            {
                                element = elements[j];
                                if (element.Length > 0)
                                {
                                    lyric = element.Split('>').Last();
                                    if (lyric.Length > 0)
                                    {
                                        timestamp = element.Substring(0, element.IndexOf(">"));
                                        time = (long)TimeToMs(timestamp);
                                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, lyric));
                                    }
                                }
                            }
                            SyncLyrics.Add(SyncLine);
                        }
                    }
                    else
                    {                        
                        // LRC with only lines
                        // Example: [00:01.03] La petite

                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, lyric));
                        SyncLyrics.Add(SyncLine);
                    }
                }

                return SyncLyrics;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        /// <summary>
        /// Formate SyncLyrics to meet Midi editor needs (line separators and paragraphs on dedicated lines)
        /// </summary>
        /// <param name="synclyrics"></param>
        /// <returns></returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> FormateSyncLyricsForMidi(List<List<keffect.KaraokeEffect.kSyncText>> synclyrics)
        {

            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<keffect.KaraokeEffect.kSyncText> tmpSyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLines = new List<List<keffect.KaraokeEffect.kSyncText>>();

            long time;
            string lyric;
            bool bParagraph = false;

            if (synclyrics == null) return null;

            for (int i = 0; i < synclyrics.Count; i++)
            {

                SyncLine = new List<keffect.KaraokeEffect.kSyncText>();

                tmpSyncLine = synclyrics[i];
                time = tmpSyncLine[0].Time;
                lyric = tmpSyncLine[0].Text;

                
                if (lyric.Trim().Length > 0)
                {
                    if (i > 0 && !bParagraph)
                    {
                        // Add Line separator
                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, m_SepLine));
                        SyncLines.Add(SyncLine);
                    }
                    // Add existing line
                    SyncLines.Add(tmpSyncLine);
                    bParagraph = false;
                }
                else
                {
                    if (i > 0)
                    {
                        // Add a paragraph
                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, m_SepParagraph));
                        SyncLines.Add(SyncLine);

                        bParagraph = true;
                    }
                }                                                
            }

            return SyncLines;

        }


        /// <summary>
        /// Find out what type of digits the file is made out
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>Returns -1 if error, 2 or 3 either</returns>
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

            if (digits3 == 0 && digits2 == 0) return null;
            if (digits3 > 0 && digits2 > 0) return null;

            return digits3 > digits2 ? pattern3digits : pattern2digits;
        }


        #endregion import LRC


            
        /// <summary>
        /// Extract artist and song from file name
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
            // Remove all (1) (2) etc.. in fName
            string pattern = @"[(\d)]";
            string replace = @"";
            fName = Regex.Replace(fName, pattern, replace).Trim();

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
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }

            return new List<string> { Tag_Artist, Tag_Title };
        }


        #region export lrc

        /// <summary>
        /// Extracts and formats individual lyric words at word level, not syllable, from a list of timestamped lyric rows, applying optional
        /// transformations such as accent removal, case conversion, and filtering of non-alphanumeric characters.
        /// </summary>
        /// <remarks>This method is useful for preparing lyric data for export or further analysis,
        /// ensuring consistent formatting and optional filtering. The method processes each lyric row individually and
        /// supports merging of syllables based on underscore conventions. If both bUpperCase and bLowerCase are set to
        /// true, the last applied transformation will determine the case of the output.</remarks>
        /// <param name="lstDgRows">A list of tuples where each tuple contains a timestamp, in milliseconds, and the corresponding lyric text to
        /// process.</param>
        /// <param name="_LrcMillisecondsDigits">The number of digits to use for milliseconds in the formatted timestamp. Determines the precision of the
        /// timestamp in the output.</param>
        /// <param name="bRemoveAccents">true to remove diacritical marks from the lyric text; otherwise, false.</param>
        /// <param name="bUpperCase">true to convert the lyric text to uppercase; otherwise, false.</param>
        /// <param name="bLowerCase">true to convert the lyric text to lowercase; otherwise, false.</param>
        /// <param name="bRemoveNonAlphaNumeric">true to remove all non-alphanumeric characters from the lyric text, except underscores; otherwise, false.</param>
        /// <param name="_myLyricsMgmt">An optional MidiLyricsMgmt instance used to apply additional lyric processing, such as removing chord
        /// patterns. If null, no additional processing is performed.</param>
        /// <returns>A list of strings, each representing a processed lyric word or segment, formatted according to the specified
        /// options.</returns>
        public static List<string> ExtractDgRowsWordsLevel(List<(double, string)> lstDgRows, int _LrcMillisecondsDigits, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, MidiLyricsMgmt _myLyricsMgmt = null)
        {
            string sTime;
            double time;

            string sLyric;

            bool bHasStartingLink = false;
            bool bPreviousHasTrailingLink = true;
            bool bMerge = false;

            // Put everything into a string tx
            string tx = string.Empty;
            for (int i = 0; i < lstDgRows.Count; i++)
            {
                bMerge = false;

                // Midi format for timestamp is milliseconds
                // Convert from "00:01.123" to "00.01.12" if necessary
                time = lstDgRows[i].Item1;
                sTime = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);

                sTime = "[" + sTime + "]";
                sLyric = lstDgRows[i].Item2;

                if (sLyric != "" && sLyric != m_SepLine && sLyric != m_SepParagraph)
                {

                    // Remove chords
                    if (_myLyricsMgmt != null && _myLyricsMgmt.RemoveChordPattern != null)
                        sLyric = Regex.Replace(sLyric, _myLyricsMgmt.RemoveChordPattern, @"");

                    // Remove accents
                    sLyric = bRemoveAccents ? LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                    //Uppercase letters
                    sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                    // Lowercase letters
                    sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                    // Remove non alphanumeric chars
                    // Protect underscore
                    sLyric = sLyric.Replace("_", " ");
                    sLyric = bRemoveNonAlphaNumeric ? LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;
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
            }
            ;

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
        /// Save Lyrics to .lrc file format and by lines
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
        public static void SaveLRCLines(string File, List<(double, string)> lstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, uint Tag_Year, string Tag_DPlus, bool bControlLength, int MaxLength, int _LrcMillisecondsDigits, MidiLyricsMgmt _myLyricsMgmt = null)
        {
            string sLine;

            string lrcs;
            string cr = "\r\n";

            #region meta data

            // List to store lines
            List<string> lstHeaderLines = new List<string>();

            // Store meta datas
            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "tool:", "ti:", "ar:", "al:", "la:", "by:", "D+:" };
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


            // Make treatment of lyrics (same for midi Lyrics edition and mp3 Lyrics edition)
            List<string> lstLyricsItems = Utilities.LyricsUtilities.ExtractDgRowsWordsLevel(lstDgRows, _LrcMillisecondsDigits, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _myLyricsMgmt);

            // Store lyrics in lines (remove timestamps from lines, except for the first word)
            // [00:04.59]It's_been_a_hard_day's_night
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store timestamps + lyrics in lines (add spaces if not existing)
            // initial [00:04.59]It's[00:04.83]_been[00:05.05]_a[00:05.27]_hard[00:06.15]_day's[00:06.81]_night[00:08.14]
            // result  [00:04.59]It's [00:04.83]_been [00:05.05]_a [00:05.27]_hard [00:06.15]_day's [00:06.81]_night [00:08.14]
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

            // Save file
            SaveStringToFile(File, lrcs, true);            
        }


     
        /// <summary>
        /// Save lyrics to .lrc file format and by syllables
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
        /// <param name="Tag_Year"></param>
        /// <param name="Tag_DPlus"></param>
        /// <param name="_LrcMillisecondsDigits"></param>
        /// <param name="_myLyricsMgmt"></param>
        public static void SaveLRCSyllabes(string File, List<(double, string)> lstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, uint Tag_Year, string Tag_DPlus,int _LrcMillisecondsDigits, MidiLyricsMgmt _myLyricsMgmt = null)
        {
            string Content;
            // Header of the LRC files containing the tags
            Content = CreateTagString(bRemoveAccents, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus);

            // Apply treatments choosen to the lyrics
            List<(double time, string lyric)> lstDgRowsTreated = ApplyTextTreatments(lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _myLyricsMgmt);

            // Create lines with time and lyrics to save in lrc file like [00:05.886]Hello <00:08.544>it's <00:08.924>me
            List<(string stime, string lyric)> lstTimeLines = CreateLrcLines(lstDgRowsTreated, _LrcMillisecondsDigits);

            // 
            //Content += CreateLrcStringWordLevel(lstTimeLines);
            Content += CreateLrcStringSyllableLevel(lstTimeLines);

            // Save file
            SaveStringToFile(File, Content, true);            
        }

        


        /// <summary>
        /// Create the header of the LRC file composed of tags tool, title, artist, album, lang
        /// </summary>
        /// <param name="bRemoveAccents"></param>
        /// <param name="bRemoveNonAlphaNumeric"></param>
        /// <param name="Tag_Tool"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        /// <returns>Returns a string</returns>
        private static string CreateTagString(bool bRemoveAccents, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus)
        {
            string lrcs = string.Empty;
            //string cr = "\r\n";
            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "tool:", "ti:", "ar:", "al:", "la:", "by:", "D+:" };
            string Tag;
            string TagName;


            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                    lrcs += "[" + TagName + Tag + "]" + Environment.NewLine;
            }

            return lrcs;
        }

        /// <summary>
        /// Applies a series of text transformations to a list of karaoke text objects based on the specified options.
        /// </summary>
        /// <remarks>If both bUpperCase and bLowerCase are set to true, the text will be converted to
        /// lowercase, as the lowercase transformation is applied after the uppercase transformation. Separator lines
        /// and paragraphs are not modified.</remarks>
        /// <param name="LstDgRows">A list of karaoke text objects to be processed. Each object contains the text and its associated timing
        /// information.</param>
        /// <param name="bRemoveAccents">true to remove diacritical marks from the text; otherwise, false.</param>
        /// <param name="bUpperCase">true to convert the text to uppercase; otherwise, false.</param>
        /// <param name="bLowerCase">true to convert the text to lowercase; otherwise, false.</param>
        /// <param name="bRemoveNonAlphaNumeric">true to remove all non-alphanumeric characters from the text; otherwise, false.</param>
        /// <returns>A new list of karaoke text objects with the specified text treatments applied. The timing information is
        /// preserved.</returns>
        private static List<(double time, string lyric)> ApplyTextTreatments(List<(double time, string lyric)> LstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, MidiLyricsMgmt _myLyricsMgmt)
        {
            List<(double time, string lyric)> Result = new List<(double time, string lyric)>();
            string sLyric;

            for (int i = 0; i < LstDgRows.Count; i++)
            {
                sLyric = LstDgRows[i].lyric;

                if (sLyric.Trim() != m_SepLine && sLyric.Trim() != m_SepParagraph)
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

                    // Remove non-alphanumeric chars
                    sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;

                }

                Result.Add((LstDgRows[i].time, sLyric));
            }

            return Result;
        }


        /// <summary>
        /// Create lines like [00:05.886]Hello <00:08.544>it's <00:08.924>me by merging the syllabes
        /// </summary>
        /// <remarks>Handles line feed and paragraph separators to ensure correct formatting of lyric
        /// lines. The method adds empty lines for paragraph breaks and distinguishes between line starts and syllable
        /// continuations using different timestamp formats.</remarks>
        /// <param name="lstDgRowsTreated">A list of tuples, each containing a timestamp and its associated lyric line. The list represents the
        /// processed lyric rows to be converted.</param>
        /// <param name="LrcMillisecondsDigits">The number of digits to use for milliseconds in the timestamp format. Determines the precision of the
        /// timestamp in the output.</param>
        /// <returns>A list of tuples, where each tuple contains a formatted timestamp and its corresponding lyric line. The list
        /// is structured for use in LRC files.</returns>
        private static List<(string sTime, string lyric)> CreateLrcLines(List<(double time, string lyric)> lstDgRowsTreated, int _LrcMillisecondsDigits)
        {
            List<(string stime, string lyric)> Result = new List<(string stime, string lyric)>();

            string sLyric;
            string sTime;
            double time;
            TimeSpan ts;

            bool bLineFeed = true;
            bool bParagraph = true;
            bool bFirstLine = true;

            for (int i = 0; i < lstDgRowsTreated.Count; i++)
            {
                time = lstDgRowsTreated[i].time;
                sLyric = lstDgRowsTreated[i].lyric;

                sTime = MsToTime(time, _LrcMillisecondsDigits);                

                if (sLyric.Trim() != m_SepLine && sLyric.Trim() != m_SepParagraph)
                {
                    // Normal syllabe, not a separator line or paragraph
                    if (bParagraph)
                    {
                        if (!bFirstLine)
                        {
                            // Add an empty line before the line with timestamp to create a paragraph                                        
                            Result.Add(("[" + sTime + "]", string.Empty));
                        }
                        else bFirstLine = false;
                        // Start a new line with timestamp           
                        Result.Add(("[" + sTime + "]", sLyric));
                    }
                    else if (bLineFeed)
                    {
                        // Start a new line with timestamp                                                  
                        Result.Add(("[" + sTime + "]", sLyric));
                    }
                    else
                    {
                        // Continue a line with a syllabe, add time in <> and not in [] to differenciate with the start of line
                        Result.Add(("<" + sTime + ">", sLyric));
                    }
                    bLineFeed = false;
                    bParagraph = false;
                }
                else if (sLyric.Trim() == m_SepLine)
                {
                    // Line feed
                    if (!bParagraph && !bLineFeed)
                        bLineFeed = true;
                    else if (bLineFeed && !bParagraph)
                    {
                        bLineFeed = false;
                        bParagraph = true;
                    }

                }
                else if (sLyric.Trim() == m_SepParagraph)
                {
                    // Paragraph
                    bParagraph = true;
                    bLineFeed = false;
                }                
            }
            return Result;
        }

        /// <summary>
        /// Generates an LRC-formatted string containing only syllable-level timing and lyrics information.
        /// </summary>
        /// <remarks>This method is intended for scenarios where only syllable-level timing is required in
        /// the LRC output. It processes the input list to create a continuous LRC string, handling both new lines and
        /// continuations based on the timestamp format.</remarks>
        /// <param name="lstTimeLines">A list of tuples where each tuple consists of a timestamp and its associated lyric segment. The timestamp
        /// should be in the format '[mm:ss.xx]' to indicate the start of a new line, or in another format to indicate a
        /// continuation within the same line.</param>
        /// <returns>A string representing the complete LRC-formatted lyrics, with each line beginning with its corresponding
        /// timestamp and containing the associated lyric segments.</returns>
        private static string CreateLrcStringSyllableLevel(List<(string stime, string lyric)> lstTimeLines)
        {
            string nextLyric = string.Empty;
            string nextTime = string.Empty;
            
            string keepLyric = string.Empty;
            string keepTime = string.Empty;

            string sTime;
            string sLyric;
            string lrcs = string.Empty;
            string lines = string.Empty;
            string cr = "\r\n";

            for (int i = 0; i < lstTimeLines.Count; i++)
            {
                sTime = lstTimeLines[i].stime;
                sLyric = lstTimeLines[i].lyric;

                // This is the start of a new line
                if (sTime.IndexOf("[") > -1)
                {
                    // Store previous line 
                    if (lrcs.Trim().Length > 0)
                    {
                        lines += lrcs + Environment.NewLine;
                    }
                    
                    // Start a new line with [00:00.00]La
                    lrcs = sTime + sLyric;
                }
                else
                {
                    // This is the continuation of a line with <00:00.00>pe
                    lrcs += sTime + sLyric;
                }


            }
            if (lrcs.Trim().Length > 0)
                lines += lrcs + cr;

            return lines;

        }

        /// <summary>
        /// Generates a formatted LRC string from a list of timestamped lyric segments, arranging them for synchronized
        /// lyric display.
        /// </summary>
        /// <remarks>This method merges consecutive lyric segments that form a single word and ensures
        /// that new lines and timestamps are correctly formatted according to LRC standards. Syllables that are part of
        /// the same word are combined, and appropriate line breaks are inserted to maintain the structure of the
        /// lyrics.</remarks>
        /// <param name="lstTimeLines">A list of tuples, where each tuple contains a timestamp and the corresponding lyric segment. Each timestamp
        /// should be in a format compatible with LRC files, and lyric segments may represent syllables or words.</param>
        /// <returns>A string containing the formatted LRC lyrics, with timestamps and lyric segments arranged for proper
        /// synchronization and display.</returns>
        private static string CreateLrcStringWordLevel(List<(string stime, string lyric)> lstTimeLines)
        {
            string nextLyric = string.Empty;
            string nextTime = string.Empty;

            bool bKeepForNextSyllabe = false;
            string keepLyric = string.Empty;
            string keepTime = string.Empty;

            string sTime;
            string sLyric;
            string lrcs = string.Empty;
            string lines = string.Empty;
            string cr = "\r\n";

            // Add a trailing "-" to syllabes without space with the next syllabe (ie it is a word composed of several syllabes)
            for (int i = 0; i < lstTimeLines.Count; i++)
            {
                sTime = lstTimeLines[i].stime;
                sLyric = lstTimeLines[i].lyric;

                if (i < lstTimeLines.Count - 1)
                {
                    nextLyric = lstTimeLines[i + 1].lyric;
                    nextTime = lstTimeLines[i + 1].stime;
                }
                else
                {
                    nextLyric = "";
                    nextTime = "";
                }
                // No trailing space in the current, no starting space in the next and the next is not a new line ([]) 
                // => this syllabe must be merged with the next one
                if (!sLyric.EndsWith(" ") && nextLyric.Length > 0 && !nextLyric.StartsWith(" ") && nextTime.IndexOf("[") == -1)
                {
                    lstTimeLines[i] = (lstTimeLines[i].stime, lstTimeLines[i].lyric + "-");
                }
            }

            for (int i = 0; i < lstTimeLines.Count; i++)
            {
                sTime = lstTimeLines[i].stime;
                sLyric = lstTimeLines[i].lyric;

                // Keep all syllabes ending with a trailing "-" until a syllabe without a "-"
                if (sLyric.EndsWith("-"))
                {
                    sLyric = sLyric.Substring(0, sLyric.Length - 1).Trim();  // remove the "-"
                    keepLyric += sLyric;                                     // add syllabe to previous ones   
                    if (keepTime == "")
                        keepTime = sTime;                                    // keep only the first timestamp (beginning of the word)   

                    if (sTime.IndexOf("[") > -1)                             // if new line, store previous one
                    {
                        // Store previous line 
                        if (lrcs.Trim().Length > 0)
                        {
                            lines += lrcs + cr;
                        }
                        lrcs = "";
                    }

                    // Skip 
                    continue;
                }
                else if (keepLyric != "")
                {
                    // no trailing "-" and there are syllabes into keeplyric => this is the last syllabe of a word
                    bKeepForNextSyllabe = true;
                }

                // This is the start of a new line
                if (sTime.IndexOf("[") > -1)
                {
                    // Store previous line 
                    if (lrcs.Trim().Length > 0)
                    {
                        lines += lrcs + cr;
                    }
                    // Format of timestamp is [] 
                    lrcs = sTime + sLyric.Trim();
                }
                else
                {
                    // This is a normal syllabe 
                    if (!bKeepForNextSyllabe)
                    {
                        // Format of timestamp is <> + space before
                        lrcs += " " + sTime + sLyric.Trim();
                    }
                    else
                    {
                        // this is The last syllabe of a word
                        if (keepTime.IndexOf("[") > -1)
                        {
                            // if the word stored in keeplyric was a starting line []
                            lrcs += keepTime + keepLyric + sLyric.Trim();
                        }
                        else
                        {
                            // if the word stored in keeplyric was a normal word, add a space before the <00:00.000>  
                            lrcs += " " + keepTime + keepLyric + sLyric.Trim();
                        }

                        // Reset variables used to store syllabes of a word
                        bKeepForNextSyllabe = false;
                        keepLyric = "";
                        keepTime = "";
                    }
                }
            }

            if (lrcs.Trim().Length > 0)
                lines += lrcs + cr;


            return lines;
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
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return null;
            }
            return lstLinesCut;
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


        public static List<string> GetLrcLines(List<string> lstLyricsItems, int _LrcMillisecondsDigits)
        {
            // [00:04.59]
            // [00:04.59]IT'S[00:04.83]BEEN[00:05.05]A[00:05.27]HARD[00:06.15]DAY'S[00:06.81]NIGHT[00:08.14]
            // [00:08.14]AND[00:08.37]I'VE[00:08.60]BEEN[00:08.79]WOR[00:09.04]KING[00:09.91]LIKE[00:10.14]A[00:10.34]DOG[00:11.66]
            // [00:11.66]IT'S[00:11.88]BEEN[00:12.09]A[00:12.32]HARD[00:13.22]DAY'S[00:13.89]NIGHT[00:15.19]

            List<string> lstLines = new List<string>();
            string line;
            string sTime;
            //string tx;
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

        #endregion export lrc


        #endregion import export LRC


        #region import export KOK

        //
        // * KOK format example:
        // * 
        // * Times:
        // * Exemple
        // * Dig;207,3672; in;207,5166; the;207,8154; dan;207,9648;cing;208,4130; queen;208,8612;
        // * the last item is 208,8612
        // * It means 208 seconds and 8612 milliseconds
        // * 
        // * 
        // * You;26.294; can;26.892; dance;27.191;
        // * You;28.685; can;29.282; jive;29.581;
        // * Ha;31.075;ving;31.523; the;31.972; time;32.27; of;32.719; your;33.167; life;33.466;
        // * Ooh,;34.661; see;35.856; that;36.304; girl,;36.752; watch;38.246; that;38.695; scene.;39.143;
        // * Dig;40.039; in;40.189; the;40.487; dan;40.637;cing;41.085; queen;41.533;
        // * Fri;50.198;day;50.497; night;50.647; and;50.945; the;51.244; lights;51.394; are;51.692; low;51.991;
        // * Loo;54.979;king;55.278; out;55.427; for;55.726; a;56.025; place;56.174; to;56.473; go;56.772;
        // * Oh,;59.013; where;59.76; they;60.059; play;60.208; the;60.507; right;60.656; mu;60.955;sic;61.254;
        // * Get;62.15;ting;62.449; in;62.599; the;62.897; swing;63.047;
        //


        #region import kok

        /// <summary>
        /// Reads karaoke lyrics and their associated timestamps from a KOK format file and returns a structured list of
        /// synchronized lyric elements.
        /// </summary>
        /// <remarks>Each line in the KOK file is parsed into lyric segments with corresponding
        /// timestamps. Paragraph separations are handled by empty lines in the file. If a timestamp in the file exceeds
        /// the specified duration, an error message is displayed and an empty list is returned.</remarks>
        /// <param name="FileName">The path to the KOK format file containing the karaoke lyrics and timestamps. The file must be accessible
        /// and properly formatted.</param>
        /// <param name="duration">The total duration of the song, in seconds. All timestamps in the file must not exceed this duration.</param>
        /// <returns>A list of lists, where each inner list contains synchronized lyric elements for a line or paragraph. Returns
        /// an empty list if a timestamp exceeds the specified duration.</returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> ReadKokFromFile(string FileName, double duration)
        {
            string sLyric;                               // Syllabe or line separator (first word of the line is prefixed with a line separator ('/'), except if it is the first line of the file)
            string sTimestamp = string.Empty;            // The timestamp is in seconds, with decimals (ex: 26.294)
            long timestamp;
            string line;
            bool bParagraphSepFound = false;

            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText> ();
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>> ();

            int lineNr = 0;


            // Detect encoding                               
            Encoding enc = GetEncodingFromFile(FileName);


            using (StreamReader reader = new StreamReader(FileName, enc, true))
            {                
                while ((line = reader.ReadLine()) != null)
                {
                    lineNr++;
                    
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();

                    // Case empty line; this is a paragraph
                    // The next line will be the first line of the new paragraph, and must be prefixed with a line separator ('/'), except if it is the first line of the file
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        bParagraphSepFound = true;
                        continue;
                    }

                    string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length - 1; i += 2)
                    {
                        sLyric = parts[i];
                        sTimestamp = parts[i + 1].Trim();


                        // Convert timestamp to milliseconds if necessary
                        // time is in the format <seconds>,<milliseconds>
                        // for exeample 208,8612 means 208 seconds and 8612 milliseconds
                        string[] split = sTimestamp.Split(',');
                        timestamp = Convert.ToInt64(split[0]) * 1000 + Convert.ToInt64(split[1]);
                        //timestamp = Convert.ToDouble(sTimestamp) * 1000; // Assuming timestamp is in seconds


                        // Check if timespam is less than the song duration
                        #region guard
                        //timestamp = Convert.ToDouble(sTimestamp);
                        if (timestamp/1000 > duration)
                        {
                            MessageBox.Show("Timestamp " + sTimestamp + " on line " + lineNr + " is higher than song duration (" + duration + " seconds). Please correct it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); sTimestamp = duration.ToString();
                            return new List<List<keffect.KaraokeEffect.kSyncText>>();
                        }
                        #endregion guard



                        if (bParagraphSepFound)
                        {
                            // Create a new line with the current timestamp and an empty string
                            SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                            SyncLine.Add(new keffect.KaraokeEffect.kSyncText((long)timestamp, ""));
                            SyncLyrics.Add(SyncLine);

                            // Create a new line
                            SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                            SyncLine.Add(new keffect.KaraokeEffect.kSyncText((long)timestamp, sLyric));
                            bParagraphSepFound = false;
                        }
                        else
                        {
                            SyncLine.Add(new keffect.KaraokeEffect.kSyncText((long)timestamp, sLyric));
                        }
                    }
                    SyncLyrics.Add(SyncLine);
                }
            }

            return SyncLyrics;

        }


        /// <summary>
        /// Reads a file containing pairs of words and timestamps, and returns a list of tuples representing each word
        /// and its associated timestamp.
        /// </summary>
        /// <remarks>Each line in the file should be formatted as 'word;timestamp; word;timestamp; ...'.
        /// The method processes each line, pairing words with their timestamps in the order they appear. The first word
        /// of each line is prefixed with a line separator ('/').</remarks>
        /// <param name="fileName">The path to the file to read. The file must exist and be accessible.</param>
        /// <parma name="duration">Duration of the song in ms</parma>
        /// <returns>A list of tuples, where each tuple contains a word and its corresponding timestamp extracted from the file.</returns>
        public static List<(string, string)> KokReadFile(string fileName, double duration)
        {
            string word;                                // Syllabe or line separator (first word of the line is prefixed with a line separator ('/'), except if it is the first line of the file)
            string sTimestamp = string.Empty;            // The timestamp is in seconds, with decimals (ex: 26.294)
            double timestamp;
            bool paragraphSepFound = false;
            

            List<(string, string)> lstDgRows = new List<(string, string)>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Each line is expected to be in the format: "word;timestamp; word;timestamp; ..."

                    // Case empty line; this is a paragraph
                    // The next line will be the first line of the new paragraph, and must be prefixed with a line separator ('/'), except if it is the first line of the file
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        paragraphSepFound = true;
                        continue;
                    }

                    string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length - 1; i += 2)
                    {
                        sTimestamp = parts[i + 1].Trim();

                        // Check if timespam is less than the song duration
                        #region guard
                        timestamp = Convert.ToDouble(sTimestamp);
                        if (timestamp > duration)
                        {
                            MessageBox.Show("Timestamp " + sTimestamp + " on line " + (lstDgRows.Count / 2 + 1) + " is higher than song duration (" + duration + " seconds). Please correct it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); sTimestamp = duration.ToString();
                            return new List<(string, string)>();
                        }
                        #endregion guard

                        // Adaptation to frmMidiLyricsEdit
                        // Separate lines separators from the text, to be able to display them in a specific color in the datagridview
                        if (i == 0 && lstDgRows.Count > 0)
                        {

                            // First word of the line, must be prefixed with a line separator ('/'), except if it is the first line of the file
                            if (paragraphSepFound)
                            {
                                word = m_SepParagraph;
                                paragraphSepFound = false;
                            }
                            else
                                word = m_SepLine;

                            lstDgRows.Add((word, sTimestamp));

                            // The word without the line separator is added as a new row, to be able to display it in the datagridview
                            word = parts[i];
                            lstDgRows.Add((word, sTimestamp));
                        }
                        else
                        {
                            // Syllabe in the middle of the line
                            word = parts[i];
                            sTimestamp = parts[i + 1].Trim();
                            lstDgRows.Add((word, sTimestamp));
                        }
                    }
                }
            }
            return lstDgRows;
        }



        #endregion import kok


        #region export kok

        

        public static void SaveKOKSyllabes(string File, List<(double, string)> lstDgRows, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, int _LrcMillisecondsDigits, MidiLyricsMgmt _myLyricsMgmt = null)
        {

            // Apply treatments choosen to the lyrics
            List<(double time, string lyric)> lstDgRowsTreated = ApplyTextTreatments(lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _myLyricsMgmt);

            List<List<Utilities.LyricsUtilities.LyricsItem>> lstLines = ExtractDgRows(lstDgRowsTreated, _LrcMillisecondsDigits);
            
            // Format lines to specific kok format
            string Content = SaveLyricsToKokFormat(lstLines);

            // Save file
            LyricsUtilities.SaveStringToFile(File, Content, true);
        }

        /// <summary>
        /// Converts a collection of lyric lines into a formatted string that adheres to the Kok lyrics file format,
        /// including timing information for each lyric segment.
        /// </summary>
        /// <remarks>Each lyric segment is separated by a semicolon and includes its time in seconds with
        /// millisecond precision, using a comma as the decimal separator. Paragraph breaks are represented by a single
        /// space. Underscores in lyric text are replaced with spaces to match Kok format requirements.</remarks>
        /// <param name="lstLines">A list of lyric lines, where each line is represented as a list of LyricsItem objects containing the lyric
        /// text and its associated time stamp.</param>
        /// <returns>A string containing the formatted lyrics in Kok format, with each lyric segment followed by its
        /// corresponding time value.</returns>
        public static string SaveLyricsToKokFormat(List<List<LyricsItem>> lstLines)
        {
            string sLyric;
            string sLine;
            double time;
            string sTime;
            string result = string.Empty;

            List<LyricsItem> lyricsItems;

            for (int i = 0; i < lstLines.Count; i++)
            {
                lyricsItems = lstLines[i];
                sLine = string.Empty;
                for (int j = 0; j < lyricsItems.Count; j++)
                {
                    sLyric = lyricsItems[j].Lyric;

                    if (sLyric == " ")
                    {
                        // This is a paragraph
                        sLine += " ";
                    }
                    else
                    {
                        // this is a normal line
                        sLyric = sLyric.Replace("_", " "); // Replace underscore by space in lyrics
                        sTime = lyricsItems[j].Time;

                        // Time is in format mm:ss:ms, convert to seconds with milliseconds in decimal
                        //time = TimeSpan.ParseExact(sTime, @"mm\:ss\.fff", CultureInfo.InvariantCulture).TotalSeconds;
                        time = TimeToMs(sTime);
                        // Force to 3 digits
                        sTime = MsToTime(time, 3);
                        time = TimeSpan.ParseExact(sTime, @"mm\:ss\.fff", CultureInfo.InvariantCulture).TotalSeconds;

                        // Convert to string with 3 digits for milliseconds
                        sTime = time.ToString("0.000", CultureInfo.InvariantCulture);

                        // As in KarPbo software, Time must have a comma as decimal separator, replace dot by comma
                        sTime = sTime.Replace(".", ",");


                        // Build line in kok format
                        sLine += sLyric + ";" + sTime + ";";
                    }
                }

                // Add new line to result
                if (result == "")
                    result = sLine;
                else
                    result += Environment.NewLine + sLine;
            }


            return result;

        }

        #endregion export kok


        #endregion import export KOK




        #region import export SRT

        #region export SRT
        public static void SaveLyricsToSRTFormat(string fullPath, List<(double time, string lyric)> lstDgRows, int _LrcMillisecondsDigits, MidiLyricsMgmt _myLyricsMgmt)
        {
            string result = string.Empty;


            // Make treatment of lyrics (same for midi Lyrics edition and mp3 Lyrics edition)
            List<string> lstLyricsItems = Utilities.LyricsUtilities.ExtractDgRowsWordsLevel(lstDgRows, 3, false, false, false, false, _myLyricsMgmt);

            // Store lyrics in lines (remove timestamps from lines, except for the first word)
            // [00:04.59]It's_been_a_hard_day's_night
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, _LrcMillisecondsDigits);
            string line;
           
            string lyric;                       

            string[] parts;
            string StartTime;
            string EndTime;

            double starttime;
            double endtime;

            List<(double starttime, string lyric, double endtime)> lstLyrics = new List<(double stattime, string lyric, double endtime)>();

            for (int i = 0; i < lstLines.Count; i++)
            {
                line = lstLines[i];  //"[00:05.886]Hello_it's_me"

                //Extract timestamp and lyrics form the line

                parts = line.Split(']');
                if (parts.Length == 2)
                {
                    StartTime = parts[0].Substring(1);
                    EndTime = StartTime;
                    lyric = parts[1];

                    if (lyric.Trim() == "") continue;

                    lyric = lyric.Replace("_", " ");

                    starttime = TimeToMs(StartTime);
                    endtime = TimeToMs(EndTime);

                    lstLyrics.Add((starttime, lyric, endtime));
                }
                
            }

            // Fix EndTime of each line
            for (int i = 0; i < lstLyrics.Count ; i++)
            {                
                if (i < lstLyrics.Count - 1)
                {
                    lstLyrics[i] = (lstLyrics[i].starttime, lstLyrics[i].lyric, lstLyrics[i + 1].starttime - 500);
                }
            }

            var srtFile = new SRTFile();
            Subtitle subtitle;

            for (int i = 0; i < lstLyrics.Count; i++)
            {
                // Format of times must be like "00:00:01,848"

                starttime = lstLyrics[i].starttime;
                StartTime = MsToSrtTime(starttime);
                endtime = lstLyrics[i].endtime;
                EndTime = MsToSrtTime(endtime);
                
                lyric = lstLyrics[i].lyric;

                subtitle = new Subtitle(i + 1);
                subtitle.StartTime = new SRTTime(StartTime);
                subtitle.EndTime = new SRTTime(EndTime);
                subtitle.Lines.Add(lyric);
                srtFile.Subtitles.Add(subtitle);                
            }

            // Open file
            try
            {
                Encoding encoding = Encoding.UTF8;
                srtFile.WriteToFile(fullPath, encoding);
                System.Diagnostics.Process.Start(@fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion export SRT

        #endregion import export SRT


    }


}

