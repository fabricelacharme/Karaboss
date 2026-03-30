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
using Karaboss.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TagLib;
using TagLib.Id3v2;
using static System.Windows.Forms.LinkLabel;

namespace Karaboss.Mp3.Mp3Lyrics
{

    public enum Mp3LyricsTypes
    {
        None,
        LyricsWithTimeStamps,
        LRCFile,                // LRC file having same name than the mp3
        KOKFile,                // KOK file having the same name than the mp3          
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
                       
        public static SynchronisedLyricsFrame MySyncLyricsFrame;
        public static string m_SepLine = "/";
        public static string m_SepParagraph = "\\";
        public static Mp3LyricsTypes m_mp3lyricstype = Mp3LyricsTypes.None;


        // Tags
        public static string Tool;
        public static string Artist;
        public static string Title;
        public static string Description;
        public static string Album;
        public static uint Year;


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
        public static Mp3LyricsTypes GetLyricsType(SynchronisedLyricsFrame SyncLyricsFrame, string TagLyrics, string TagSubTitles, string FileName, string LyricsFileName = null)
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
        public static List<List<keffect.KaraokeEffect.kSyncText>> GetLyricsFromMp3File(SynchronisedLyricsFrame SyncLyricsFrame)
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
        /// Retrieves synchronized lyrics and associated metadata from an LRC file corresponding to the specified file
        /// name.
        /// </summary>
        /// <remarks>If the LRC file is missing or invalid, the method returns null and displays a warning
        /// message. Extracted metadata includes artist, title, album, year, and tool information, if present in the LRC
        /// file.</remarks>
        /// <param name="FileName">The path of the file for which to locate and read the associated LRC lyrics file. The file must exist and
        /// have a .lrc extension.</param>
        /// <returns>A list of karaoke effect synchronization text elements extracted from the LRC file, or null if the LRC file
        /// does not exist or is invalid.</returns>
        public static List<List<keffect.KaraokeEffect.kSyncText>> GetLyricsFromLrcFile(string FileName)
        {
            // Search for existing LRC file
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;

            // Extract Artist, Title, Album, Year from LRC file            
            string[] lines = System.IO.File.ReadAllLines(lrcFile);

            if (lines == null)
            {
                MessageBox.Show("Invalid lrc file: " + Path.GetFileName(lrcFile), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            Artist = string.Empty;
            Title = string.Empty;
            Album = string.Empty;
            Year = 0;
            Description = string.Empty;
            Tool = string.Empty;

            Artist = GetArtistFromLrc(lines);
            Title = GetTitleFromLrc(lines);
            Album = GetAlbumFromLrc(lines);
            Year = GetYearFromLrc(lines);
            Tool = GetToolFromLrc(lines);


            return LyricsUtilities.ReadLrcFromFile(lrcFile);
        }

     
        private static string GetArtistFromLrc(string[] lines)
        {
            return LyricsUtilities.GetTagFromLrc(lines, "ar");
        }

        private static string GetTitleFromLrc(string[] lines)
        {
            return LyricsUtilities.GetTagFromLrc(lines, "ti");
        }

        private static string GetAlbumFromLrc(string[] lines)
        {
            return LyricsUtilities.GetTagFromLrc(lines, "al");
        }

        private static string GetToolFromLrc(string[] lines)
        {
            return LyricsUtilities.GetTagFromLrc(lines, "tool");
        }

        private static uint GetYearFromLrc(string[] lines)
        {
            string yearStr = LyricsUtilities.GetTagFromLrc(lines, "by");
            uint year;
            if (uint.TryParse(yearStr, out year))
                return year;
            else
                return 0;
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
