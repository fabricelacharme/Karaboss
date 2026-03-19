using Mozilla.NUniversalCharDet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KFNV.Utilities
{

    public class Syllable
    {
        public int Time { get; set; }
        public string Text { get; set; }

        public Syllable(int time, string text)
        {
            this.Time = time;
            this.Text = text;
        }
    }


    public static  class LyricsUtilities
    {

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


        /// <summary>
        /// Find out what type of digits the file is made out
        /// </summary>
        /// <param name="lines"></param>
        /// <returns>Returns -1 if error, 2 or 3 either</returns>
        public static int GetDigitsLRC(string[] lines)
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
        public static string GetPatternLRC(string[] lines)
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


        public static List<List<Syllable>> ReadLrcFromFile(string FileName)
        {
            if (!File.Exists(FileName))
            {
                MessageBox.Show("Invalid lyrics file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            try
            {
                // Detect encoding                               
                Encoding enc = LyricsUtilities.GetEncodingFromFile(FileName);

                // Read lines with encoding
                string[] lines = System.IO.File.ReadAllLines(FileName, enc);
                if (lines.Count() == 0)
                {
                    MessageBox.Show("Invalid lyrics file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                // Search type of timestamp format for milliseconds (2 or 3 digits)
                int digits = LyricsUtilities.GetDigitsLRC(lines);
                if (digits == -1)
                {
                    MessageBox.Show("Invalid lyrics file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                string patternline = LyricsUtilities.GetPatternLRC(lines);
                if (patternline == null)
                {
                    MessageBox.Show("Invalid lyrics file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                List<(string, string)> result = new List<(string, string)>();

                int time;
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

                                result.Add((timestamp, lyric));
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

                List<Syllable> SyncLine = new List<Syllable>();
                List<List<Syllable>> SyncLines = new List<List<Syllable>>();

                // Now we have to separate the syllables                                               
                for (int i = 0; i < sortedList.Count; i++)
                {
                    time = (int)LyricsUtilities.TimeToMs(sortedList[i].Item1);

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
                            SyncLine = new List<Syllable>();
                            SyncLine.Add(new Syllable(time, lyric));
                            SyncLines.Add(SyncLine);
                        }
                        else
                        {
                            // This is enhanced LRC like "[00:01.03]La <00:01.15>petite <00:01.48>maison"
                            // restline is <00:01.15>petite <00:01.48>maison
                            string restline = lyric.Substring(s);

                            SyncLine = new List<Syllable>();
                            SyncLine.Add(new Syllable(time, firstsyllabe));

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
                                        time = (int)LyricsUtilities.TimeToMs(timestamp);
                                        SyncLine.Add(new Syllable(time, lyric));
                                    }
                                }
                            }
                            SyncLines.Add(SyncLine);
                        }
                    }
                    else
                    {
                        // LRC with only lines
                        // Example: [00:01.03] La petite

                        SyncLine = new List<Syllable>();
                        SyncLine.Add(new Syllable(time, lyric));
                        SyncLines.Add(SyncLine);
                    }
                }

                return SyncLines;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    

    }
}
