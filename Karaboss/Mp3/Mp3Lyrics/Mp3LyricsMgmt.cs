﻿#region License

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
                //m_mp3lyricstype = Mp3LyricsTypes.LyricsWithTimeStamps;
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
        /// Export lyrics issued form a LRC File
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

                    if (lyric.Trim() != "")
                    {
                        // Search for new lines
                        if (lyric.StartsWith("\r") || lyric.StartsWith("\n"))
                        {
                            lyric = lyric.Substring(1);
                            bNewLine = true;
                        }

                        if (lyric.EndsWith("\r") || lyric.EndsWith("\n"))
                        {
                            lyric = lyric.Substring(0, lyric.Length - 1);
                            bNewLine = true;
                        }
                    }

                    sct = new keffect.KaraokeEffect.kSyncText(time, lyric);
                    if (bNewLine)
                    {
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);
                        
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(sct);
                    }
                    else
                    {
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
            // Search for existing LRC file
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;
            
            string line;
            string lyric = string.Empty;
            long time;
            string stime = string.Empty;

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
            //string[] lines = System.IO.File.ReadAllLines(lrcFile);
            string[] lines;

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


            // Regex to capture timestamps and words => for milliseconds having 3 digits or 2
            // Find out what type of digits the file is made out
            string pattern = GetPatternLRC(lines);
            if (pattern == null)
            {
                MessageBox.Show("Invalid lrc file, no timestamps found: " + Path.GetFileName(FileName), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }


            // Create a list of all lines
            // Load lyrics in KaraokeEffect format
            List<keffect.KaraokeEffect.kSyncText> SyncLine;
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();

            string timestamp;
            string word;

            for (int i = 0; i < lines.Length; i++)
            {
                // study line by line
                line = lines[i];

                // Warning lines with only a time stamp, without lyric [00:08.05] is rejected
                if (line.StartsWith("[") && line.EndsWith("]"))
                    line = line + "/";

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
                    
                    // Add a linefeed if timestamp was "[]"
                    if (match.Groups[1].Value != "")
                    {
                        // Why add a \r\n? => Keep information of start new line
                        // This will be replaced by a "/" in frmMp3EditLyrics
                        word = Environment.NewLine + word;    
                    }

                    time = (long)TimeToMs(timestamp);
                    
                    SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, word));
                }
                SyncLyrics.Add(SyncLine);
            }               

            return SyncLyrics;
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

        #region id3v2

        /// <summary>
        /// Save frame of synchronized lyrics in mp3 file
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="SyncLyrics"></param>
        public static bool SetTags(string FullPath, SynchronisedLyricsFrame SyncLyrics)
        {
            TagLib.Tag _tag;

            try
            {
                if (FullPath == null) return false;
                
                TagLib.File file = TagLib.File.Create(FullPath);
                _tag = file.GetTag(TagTypes.Id3v2);

                
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
