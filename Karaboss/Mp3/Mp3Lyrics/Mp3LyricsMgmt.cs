using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TagLib;
using TagLib.Id3v2;

namespace Karaboss.Mp3.Mp3Lyrics
{

    public enum Mp3LyricsTypes
    {
        None,
        LyricsWithTimeStamps,
        LRCFile,
        LyricsWithoutTimeStamps,
    }

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
               
        public static SyncText[] SyncTexts;
        public static SynchronisedLyricsFrame MySyncLyricsFrame;
        public static string m_SepLine = "/";
        public static Mp3LyricsTypes m_mp3lyricstype = Mp3LyricsTypes.None;


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
                return Mp3LyricsTypes.LyricsWithTimeStamps;
            
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
            long time;
            string tx = string.Empty;
            string line = string.Empty;

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\mp3lyrics.txt";

            for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
            {
                lyric = SyncLyricsFrame.Text[i].Text;
                lyric = lyric.Replace("\r\n", m_SepLine);
                lyric = lyric.Replace("\r", m_SepLine);
                lyric = lyric.Replace("\n", m_SepLine);


                time = SyncLyricsFrame.Text[i].Time;
                line = time.ToString() + " " + lyric;
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


        /// <summary>
        /// Get sync lyrics
        /// </summary>
        /// <param name="SyncLyricsFrame"></param>
        /// <returns></returns>
        public static SyncText[] GetSyncLyrics(SynchronisedLyricsFrame SyncLyricsFrame)
        {
            SyncText[] synchedTexts;
            string lyric;
            long time;

            synchedTexts = new SyncText[SyncLyricsFrame.Text.Count()];

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

            // 2. Display lyrics
            // If linefeeds, display lyrics with linefeeds
            if (bHasLineFeeds)
            {
                for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
                {
                    lyric = SyncLyricsFrame.Text[i].Text;
                    time = SyncLyricsFrame.Text[i].Time;

                    if (lyric.Trim() != "")
                    {
                        if (lyric.StartsWith("\r") || lyric.StartsWith("\n"))
                            lyric = "\r\n" + lyric.Substring(1);

                        if (lyric.EndsWith("\r") || lyric.EndsWith("\n"))

                            lyric = "\r\n" + lyric.Substring(0, lyric.Length - 1);
                    }
                    synchedTexts[i] = new SyncText(time, lyric);

                }
            }
            else
            {
                // If no linefeeds, display lyrics with \r\n
                for (int i = 0; i < SyncLyricsFrame.Text.Count(); i++)
                {
                    lyric = "\r\n" + SyncLyricsFrame.Text[i].Text.Trim();
                    time = SyncLyricsFrame.Text[i].Time;
                    synchedTexts[i] = new SyncText(time, lyric);
                }
            }
            
            return synchedTexts;

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
        /// Get lyrics from LRC file if exists
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>        
        public static SyncText[] GetLrcLyrics(string FileName)
        {            
            // Search for existing LRC file
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;

            SyncText[] synchedTexts;            
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
            } catch (Exception e) { MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); return null; }

            // Regex to capture timestamps and words => for milliseconds having 3 digits or 2
            // Find out what type of digits the file is made out
            string pattern = GetPatternLRC(lines);
            if (pattern == null)
            {
                MessageBox.Show("Invalid lrc file, no timestamps found: " + Path.GetFileName(FileName), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Create a list of all lines
            List<List<(string, string)>> lstLines = new List<List<(string, string)>>();
            List<(string Timestamp, string Word)> results = new List<(string Timestamp, string Word)>();

            for (int i = 0; i < lines.Length; i++)
            {
                // study line by line
                line = lines[i];

                MatchCollection matches = Regex.Matches(line, pattern);
                if (matches.Count == 0) continue;   
                                
                foreach (Match match in matches)
                {
                    // Try with "[]", than with "<>"
                    string timestamp = match.Groups[1].Value != "" ? match.Groups[1].Value : match.Groups[2].Value;
                    string word = match.Groups[3].Value;
                    
                    // Clean word
                    word = word.Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("_", " ");
                    
                    // Add a linefeed if timestamp was "[]"
                    if (match.Groups[1].Value != "")
                        word = "\r\n" + word;               // POURQUOI ajouter \r\n? => needed by PictureBox1_Paint event of frmLyrics
                    
                    
                    results.Add((timestamp, word));
                }                
            }
            

            // Load synchronized lyrics into synchedTextx
            synchedTexts = new SyncText[results.Count];
            
            for (int i = 0; i < results.Count; i++)
            {                                
                // Timestamp
                stime = results[i].Timestamp;
                time = (long)TimeToMs(stime);
                
                // Lyric
                lyric = results[i].Word;                  

                // Create a new synchedText
                synchedTexts[i] = new SyncText(time, lyric);                                
            }
                           
            return synchedTexts;
        }


        /// <summary>
        /// Convert a time stamp 01:15.510 (min 2digits, sec 2 digits, ms 3 digits) to milliseconds
        /// </summary>
        /// <param name="stime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double TimeToMs(string time)
        {
            double dur = 0;

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

            double Ms = Convert.ToDouble(ms);
            dur += Ms;

            return dur;
        }


        #region id3v2

        /// <summary>
        /// Save frame of synchronized lyrics in mp3 file
        /// </summary>
        /// <param name="FullPath"></param>
        /// <param name="SyncLyrics"></param>
        public static void SetTags(string FullPath, SynchronisedLyricsFrame SyncLyrics)
        {
            TagLib.Tag _tag;

            try
            {
                if (FullPath == null) return;
                
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

            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _tag = null;
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
