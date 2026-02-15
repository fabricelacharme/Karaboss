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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TagLib;
using TagLib.Id3v2;
using keffect;
using AzLyrics.Api;

namespace Karaboss.Mp3.Mp3Lyrics
{

    public enum Mp3LyricsTypes
    {
        None,
        LyricsWithTimeStamps,
        LRCFile,
        LyricsWithoutTimeStamps,
    }
    
    [Serializable()]
    public struct SyncText
    {
        public long Time { get; set; }
        public string Text { get; set; }
        public SyncText(long time, string text)
        {
            Time = time;
            Text = text;
        }
    }

    public static class Mp3LyricsMgmtHelper
    {
               
        //public static SyncText[] SyncTexts;
        public static SynchronisedLyricsFrame MySyncLyricsFrame;
        public static string m_SepLine = "/";
        public static string m_SepParagraph = "\\";
        public static Mp3LyricsTypes m_mp3lyricstype = Mp3LyricsTypes.None;

        
        #region kEffect

        // Line of struct SyncText
        public static List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
        // List of lines of struct SyncText
        public static List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();

        #endregion KEffect


        /// <summary>
        /// Get lyrics type
        /// </summary>
        /// <param name="SyncLyricsFrame"></param>
        /// <param name="TagLyrics"></param>
        /// <param name="TagSubTitles"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static Mp3LyricsTypes GetLyricsType(SynchronisedLyricsFrame SyncLyricsFrame, string TagLyrics, string TagSubTitles, string FileName)
        {
            Mp3LyricsTypes lType = Mp3LyricsTypes.None;
            string lrcFile;

            if (SyncLyricsFrame != null && SyncLyricsFrame.Text.Count() > 0)
            {                
                return Mp3LyricsTypes.LyricsWithTimeStamps;
            }

            if (lType == Mp3LyricsTypes.None) { 
                lrcFile = Path.ChangeExtension(FileName, ".lrc");
                if (System.IO.File.Exists(lrcFile))
                    return Mp3LyricsTypes.LRCFile;
            }

            if (lType == Mp3LyricsTypes.None)
            {
                if (TagLyrics != null && TagLyrics.Trim() != "" || TagSubTitles != null && TagSubTitles.Trim() != "")
                    return Mp3LyricsTypes.LyricsWithoutTimeStamps;
            }

            return lType;
        }

        #region export lyrics to text

        /// <summary>
        /// Export mp3 sync lyrics to text file
        /// </summary>
        /// <param name="SyncLyricsFrame"></param>
        public static void ExportSyncLyricsToText(SynchronisedLyricsFrame SyncLyricsFrame)
        {
            // Export Sync Lyrics to Text
            if (SyncLyricsFrame == null || SyncLyricsFrame.Text.Count() == 0)
            {
                MessageBox.Show("No lyrics to export", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string lyric;            
            string tx = string.Empty;
            string line = string.Empty;
            bool bLineFeed;
            string cr = "\r\n";

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\mp3lyrics.txt";

            for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
            {
                lyric = SyncLyricsFrame.Text[i].Text;
                bLineFeed = false;

                if (lyric.IndexOf(m_SepLine) != -1 || lyric.IndexOf(m_SepParagraph) != -1)
                    bLineFeed = true;

                
                lyric = lyric.Replace("\r\n", "");
                lyric = lyric.Replace(m_SepParagraph, "");
                lyric = lyric.Replace(m_SepLine, "");

                if (bLineFeed)
                {
                    if (line != "")
                        tx += line + cr;

                    line = lyric;
                }
                else
                {
                    line += lyric;
                }
            }

            if (line != "")
                tx += line + cr;



            System.IO.File.WriteAllText(@file, tx);
            try
            {
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Export lyrics issued form a text File
        /// </summary>
        /// <param name="SyncLyrics"></param>
        public static void ExportSyncLyricsToText(List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics)
        {
            // Export Sync Lyrics to Text
            if (SyncLyrics == null || SyncLyrics.Count() == 0)
            {
                MessageBox.Show("No lyrics to export", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string lyric;            
            string tx = string.Empty;
            string line = string.Empty;

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\mp3lyrics.txt";
            List<keffect.KaraokeEffect.kSyncText> SyncLine;

            for (int j = 0; j < SyncLyrics.Count(); j++)
            {
                SyncLine = SyncLyrics[j];
                line = string.Empty;

                for (int i = 0; i < SyncLine.Count; i++) 
                {
                    lyric = SyncLine[i].Text;                   
                    // clean lyrics
                    lyric = lyric.Replace("\r", "");
                    lyric = lyric.Replace("\n", "");
                    
                    line += lyric;                    
                }
                tx += line + "\r\n";
            }
            System.IO.File.WriteAllText(@file, tx);
            try
            {
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #endregion export lyrics to text



        #region get synched lyrics


        /// <summary>
        /// Get sync lyrics in List<List<SyncText>> for KEffect from SynchronisedLyricsFrame
        /// </summary>
        /// <param name="SyncLyricsFrame"></param>
        /// <returns></returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> GetKEffectSyncLyrics(SynchronisedLyricsFrame SyncLyricsFrame)
        {
            string lyric;
            long time;
            keffect.KaraokeEffect.kSyncText sct;
            
            bool bNewLine = false;            
            bool bParagraph = false;

            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();


            bool bHasLineFeeds = false;
            // 1. Search for linefeed
            for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
            {
                lyric = SyncLyricsFrame.Text[i].Text;
                if (lyric.IndexOf("\r") >= 0 || lyric.IndexOf("\n") >= 0)
                {
                    bHasLineFeeds = true;
                    break;
                }
            }

            // If linefeeds,
            if (bHasLineFeeds)
            {
                // Read all items of []
                for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
                {
                    lyric = SyncLyricsFrame.Text[i].Text;
                    time = SyncLyricsFrame.Text[i].Time;
                    
                    bNewLine = false;             
                    bParagraph = false;

                    if (lyric.Trim() != "")
                    {

                        // Search for paragraph separator
                        if (lyric.StartsWith("\n\n") || lyric.StartsWith("\r\r") || lyric.StartsWith("\r\n\r\n"))
                        {
                            //lyric = lyric.Substring(2);
                            lyric = lyric.TrimStart(new char[] { '\r', '\n' });
                            bParagraph = true;
                        }
                        else if (lyric.EndsWith("\n\n") || lyric.EndsWith("\r\r") || lyric.EndsWith("\r\n\r\n"))
                        {
                            lyric = lyric.Substring(0, lyric.Length - 2);
                            bParagraph = true;
                        }                        
                        // Search for new lines
                        else if (lyric.StartsWith("\r") || lyric.StartsWith("\n"))
                        {
                            //lyric = lyric.Substring(1);
                            lyric = lyric.TrimStart(new char[] { '\r', '\n' });
                            bNewLine = true;

                        }
                        else if (lyric.EndsWith("\r") || lyric.EndsWith("\n"))
                        {
                            lyric = lyric.Substring(0, lyric.Length - 1);
                            bNewLine = true;
                        }
                    }

                    sct = new keffect.KaraokeEffect.kSyncText(time, lyric);

                    
                    if (bParagraph)
                    {
                        // Paragraph: add line to list of lines, and create a new line
                        // previous line
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // add a blank line for the paragraph
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, ""));
                        SyncLyrics.Add(SyncLine);

                        // new line
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(sct);
                    }
                    else if (bNewLine)
                    {
                        // New line: add line to list of lines, and create a new line

                        // previous line
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // new line
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(sct);
                    }
                    else
                    {
                        // No new line, add to current line
                        SyncLine.Add(sct);
                    }
                }

                // Store last line
                if(SyncLine.Count > 0)
                    SyncLyrics.Add(SyncLine);
            }
            else
            {
                // If no linefeeds, display lyrics with \r\n
                for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
                {
                    lyric = SyncLyricsFrame.Text[i].Text.Trim();
                    time = SyncLyricsFrame.Text[i].Time;
                    sct = new keffect.KaraokeEffect.kSyncText(time, lyric);
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                    SyncLine.Add(sct);
                    SyncLyrics.Add(SyncLine);
                }
            }


            // Retuns lyrics without any separators
            return SyncLyrics;
        }


        /// <summary>
        ///  Get LRC Lyrics
        ///  Important : LRC files are mandatory composed of full words. Syllabes are not possible because we don't know how to distinguish them from words
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> GetKEffectLrcLyrics(string FileName)
        {
            // Format 1
            // [00:04.598]IT'S <00:04.830>BEEN <00:05.057>A <00:05.271>HARD <00:06.151>DAY'S <00:06.811>NIGHT               // New line                                                                              
            // [00:08.148]AND                                                                                               // New line
            //
            // Format 2:  Can be also ?
            // [00:04.598]It's
            // <00:04.830> been
            // <00:05.057> a
            // <00:05.271> hard
            // <00:06.151> day's
            // <00:06.811> night
            // [00:08.148]And
            //
            // Format 3: and also
            // [00:23.76]J'AI FAIT UNE CHANSON, 
            // [00:25.10]JE SAIS PAS POURQUOI

            // Load Lrc file into list of lines                

            string[] lines = GetLinesFromLrc(FileName);
            if (lines == null)
            {
                MessageBox.Show("Invalid lrc file: " + Path.GetFileName(FileName), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Extract sync lyrics from lines
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = GetSyncLyricsFromLines(lines);

            if (SyncLyrics == null)
            {
                MessageBox.Show("No valid LRC format in file: " + Path.GetFileName(FileName), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            // Remove line separators and add lines for paragraph separators
            SyncLyrics = RemoveSeparators(SyncLyrics);

            return SyncLyrics;
        }

        /// <summary>
        /// Retrieves the lines of lyrics and timing information from an LRC file associated with the specified file
        /// name.
        /// </summary>
        /// <remarks>The method searches for an LRC file by replacing the extension of the provided file
        /// name with ".lrc". If the LRC file is found, its contents are split and formatted according to specific rules
        /// to extract lyric lines. If the file is not found or an error occurs, an error message is displayed and null
        /// is returned.</remarks>
        /// <param name="FileName">The name of the file for which to locate and read the corresponding LRC file. This value should include the
        /// file's extension.</param>
        /// <returns>An array of strings containing the processed lines from the LRC file, or null if the LRC file does not exist
        /// or an error occurs during reading.</returns>
        private static string[] GetLinesFromLrc(string FileName)
        {
            // Search for existing LRC file
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;

            string line;
            string[] lines = null;
            try
            {
                // Load lrc into a single string
                string tx = System.IO.File.ReadAllText(lrcFile);

                // Split by "[" to have lines
                lines = tx.Split('[');

                // Treatment for each line
                for (int i = 0; i < lines.Length; i++)
                {
                    line = lines[i];
                    if (line.Trim().Length == 0) continue;

                    line = line.Trim();

                    // Add "[" removed by the split
                    line = "[" + line;

                    // Use case: format 2
                    line = line.Replace("> ", ">");                 // Remove space after > (format 2)
                    line = line.Replace(Environment.NewLine, " ");  // Remove \r\nb         (format 2)

                    // Use case: LRC full line (format 3)
                    if (line.IndexOf("<") == -1)
                        line = line.Replace(" ", "_");                  // Replace spaces by "_" in order to keep the whole sentences (format 3)
                                                                        // otherwise it will be removed by the pattern
                    lines[i] = line;
                }
            }
            catch (Exception e) { MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }

            return lines;

        }


        /// <summary>
        /// Processes an array of lyric lines and extracts synchronized lyrics with associated timestamps for karaoke
        /// display.
        /// </summary>
        /// <remarks>Lines that consist solely of timestamps are treated as paragraph markers. The method
        /// uses regular expressions to identify timestamps and lyric words, and it cleans up the extracted words by
        /// removing unnecessary characters. The output format is suitable for use with karaoke synchronization
        /// features.</remarks>
        /// <param name="lines">An array of strings representing the lines of lyrics, where each line may contain one or more timestamps and
        /// corresponding lyric words.</param>
        /// <returns>A list of lists, where each inner list contains synchronized text objects representing the timestamp and
        /// associated lyric word for each segment. Returns null if the input format is not recognized.</returns>
        private static List<List<keffect.KaraokeEffect.kSyncText>> GetSyncLyricsFromLines(string[] lines)
        {

            // Regex to capture timestamps and words => for milliseconds having 3 digits or 2
            // Find out what type of digits the file is made out
            string pattern = GetPatternLRC(lines);
            if (pattern == null) return null;

            // Create a list of all lines
            // Load lyrics in KaraokeEffect format
            List<keffect.KaraokeEffect.kSyncText> SyncLine;
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();

            string timestamp;
            string word;

            string line;
            long time;

            bool bParagraph = false;

            for (int i = 0; i < lines.Length; i++)
            {
                // study line by line
                line = lines[i];

                // Warning lines with only a time stamp, without lyric [00:08.05] is rejected by the pattern because it doesn't contain any word.
                // Lines with only a timestamp are paragraphs.
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // Is it a timestamp?
                    string lin = line + "@";
                    MatchCollection matchs = Regex.Matches(lin, pattern);
                    if (matchs.Count == 0) continue;
                    bParagraph = true;
                }

                // Search for timestamps and words in the line
                MatchCollection matches = Regex.Matches(line, pattern);
                if (matches.Count == 0) continue;

                SyncLine = new List<keffect.KaraokeEffect.kSyncText>();

                foreach (Match match in matches)
                {
                    // Try with "[]", than with "<>"
                    timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                    word = match.Groups[3].Value;

                    // Clean word
                    word = word.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("_", " ").Replace("/", "");

                    // Add a space only if a line composed of words and not a full line
                    // => separate different words of a line
                    if (matches.Count > 1)
                        word = word + " ";

                    // Add a paragraph separator if timestamp was "[]"
                    if (match.Groups[1].Value != "")
                    {
                        // If the line is only a timestamp, it is a paragraph, otherwise it is a line
                        if (bParagraph)
                        {
                            word = m_SepParagraph + word;
                            bParagraph = false;
                        }
                        else
                        {
                            word = m_SepLine + word;
                        }
                    }
                    time = (long)TimeToMs(timestamp);
                    SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, word));
                }
                SyncLyrics.Add(SyncLine);
            }

            return SyncLyrics;
        }


        /// <summary>
        /// Processes a collection of synchronized lyric lines and inserts additional lines to represent detected
        /// separator indicators, returning a new collection with these separator lines included.
        /// </summary>
        /// <remarks>Separator lines are added only when a line begins with a recognized separator
        /// indicator and is not the first line in the collection. The method modifies the original lyric lines to
        /// remove the separator indicator from the start of the line before adding them to the result.</remarks>
        /// <param name="SyncLyrics">A list of lyric lines, where each line is a list of synchronized text elements. Lines may begin with special
        /// separator indicators that denote line or paragraph breaks.</param>
        /// <returns>A new list of lyric lines, including the original lyrics with additional lines inserted for each detected
        /// separator indicator. The original lines are modified to remove the separator text from the beginning.</returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> RemoveSeparators(List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics)
        {
            // For mp3, the editor is like that
            // lines begins with a line separator on the same line of the lyric: "/Its been a hard day's night"
            // Paragraphs : blank line 

            List<List<keffect.KaraokeEffect.kSyncText>> result = new List<List<keffect.KaraokeEffect.kSyncText>>();


            List<keffect.KaraokeEffect.kSyncText> tmplst;
            keffect.KaraokeEffect.kSyncText tmp;

            

            
            for (int i = 0; i < SyncLyrics.Count; i++)
            {
                SyncLine = SyncLyrics[i];

                if (SyncLine.Count > 0)
                {
                    // Treatment for a line starting with a Line separator:
                    if (SyncLine[0].Text.StartsWith(m_SepLine))
                    {
                        // Remove the line separator                                                
                        SyncLine[0] = new keffect.KaraokeEffect.kSyncText(SyncLine[0].Time, SyncLine[0].Text.Replace(m_SepLine, ""));     
                        result.Add(SyncLine);

                    }
                    else if (SyncLine[0].Text.StartsWith(m_SepParagraph))
                    {
                        // Add a line containing a line separator, except for the first line
                        if (i == 0)
                        {
                            SyncLine[0] = new keffect.KaraokeEffect.kSyncText(SyncLine[0].Time, SyncLine[0].Text.Replace(m_SepParagraph, ""));
                            result.Add(SyncLine);
                            
                        }
                        else
                        {
                            // Add a line for the paragraph separator
                            tmp = new keffect.KaraokeEffect.kSyncText(SyncLine[0].Time, " ");
                            tmplst = new List<keffect.KaraokeEffect.kSyncText>();
                            tmplst.Add(tmp);
                            result.Add(tmplst);

                            // Remove the paragraph separator from the first words of the line
                            SyncLine[0] = new keffect.KaraokeEffect.kSyncText(SyncLine[0].Time, SyncLine[0].Text.Replace(m_SepParagraph, ""));
                            result.Add(SyncLine);
                            
                        }  
                        
                    }
                    else
                    {
                        result.Add(SyncLine);
                    }                    
                }
            }
            return result;
        }



        /// <summary>
        /// Create SyncLyrics from a string
        /// </summary>
        /// <param name="TagLyrics"></param>
        /// <returns></returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> GetKEffectStringLyrics(string TagLyrics)
        {
            if (TagLyrics == null || TagLyrics == "") return null;

            string cr = Environment.NewLine;
            string[] lines = TagLyrics.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            string line;
            List<keffect.KaraokeEffect.kSyncText> SyncLine;
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();
            long time;
            string text;

            for (int i = 0; i < lines.Count(); i++)
            {
                line = lines[i].Trim();

                time = 0;
                text = cr + line;                
                SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, text));

                SyncLyrics.Add(SyncLine );
            }

            return SyncLyrics;
        }
       

        #endregion get synched lyrics



        #region functions to convert time formats

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

        #endregion functions to convert time formats



        #region id3v2

        /// <summary>
        /// Save frame of synchronized lyrics in mp3 file
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="SyncLyrics"></param>
        public static bool SetTags(string FullPath, string AlbumArtists, string Title, string Album, uint Year, SynchronisedLyricsFrame SyncLyrics)
        {
            TagLib.Tag _tag;

            try
            {
                if (FullPath == null) return false;
                
                TagLib.File file = TagLib.File.Create(FullPath);
                _tag = file.GetTag(TagTypes.Id3v2);

               
                _tag.AlbumArtists = new string[] { AlbumArtists };                
                _tag.Title = Title;
                _tag.Album = Album;
                
                if (Year > 0)
                    _tag.Year = Year;

                // Retrieve tags from file 
                TagLib.Id3v2.Tag id3v2tag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2, true);

                // Retrieve the frame SyncLyrics inside the mp3 file (type of lyrics), create a new one if not found
                SynchronisedLyricsFrame frame = GetSyncLyricsFrame(id3v2tag, SynchedTextType.Lyrics, true);

                frame.TextEncoding = StringType.Latin1; //StringType.UTF8;
                frame.Description = "Karaboss";
                frame.Format = TimestampFormat.AbsoluteMilliseconds;

                frame.Text = new SynchedText[SyncLyrics.Text.Length];
                for (int i = 0; i < SyncLyrics.Text.Length; i++)
                {
                    frame.Text[i] = new SynchedText();
                    frame.Text[i] = SyncLyrics.Text[i];
                }
                                
                // Save file
                file.Save();

                return true;
            }
            catch (Exception e)
            {                
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _tag = null;
                return false;
            }
        }

        /// <summary>
        /// Get a synchronised frame of type lyrics from an mp3 file
        /// Create it if not found
        /// see https://vimsky.com/examples/detail/csharp-ex---TagLib-AddFrame-method.html
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type"></param>
        /// <param name="SyncLyrics"></param>
        /// <returns></returns>       
        public static SynchronisedLyricsFrame GetSyncLyricsFrame(TagLib.Id3v2.Tag tag, SynchedTextType type, bool create)
        {
            IEnumerator<Frame> enumerator = tag.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Frame current = enumerator.Current;
                    SynchronisedLyricsFrame frame = current as SynchronisedLyricsFrame;
                    if (frame != null && type == frame.Type)
                    {                                               
                        return frame;                        
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }

            if (!create)
            {
                return null;
            }

            SynchronisedLyricsFrame newframe = new SynchronisedLyricsFrame(tag.Description, "en", type);
            newframe.TextEncoding = StringType.Latin1;
            newframe.Format = TimestampFormat.AbsoluteMilliseconds;
            tag.AddFrame(newframe);
            return newframe;

        }


        #endregion id3v2

    }

}
