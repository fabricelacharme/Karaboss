using Karaboss.Lrc.SharedFramework;
using MP3GConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        //public static (long[], string[]) SyncLyrics;        
        public static SyncText[] SyncTexts;
        public static SynchronisedLyricsFrame MySyncLyricsFrame;


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
                lyric = lyric.Replace("\r\n", "\\");
                lyric = lyric.Replace("\r", "\\");
                lyric = lyric.Replace("\n", "\\");


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
        /// Get lyrics from LRC file
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>        
        public static SyncText[] GetLrcLyrics(string FileName)
        {            
            string lrcFile = Path.ChangeExtension(FileName, ".lrc");
            if (!System.IO.File.Exists(lrcFile)) return null;

            SyncText[] synchedTexts;
            string[] Lyrics;
            long[] Times;            
            long time;
            string line;
            string lyric = string.Empty;
            string stime = string.Empty;

            // Load Lrc file into list of lines                
            string[] lines = System.IO.File.ReadAllLines(lrcFile);

            List<string> lstLyrics = new List<string>();
            List<long> lstTimes = new List<long>();

            MatchCollection mc3;
            Regex r3 = new Regex(@"\[\d{2}:\d{2}.\d{3}\]");
            MatchCollection mc2;
            Regex r2 = new Regex(@"\[\d{2}:\d{2}.\d{2}\]");

            for (int i = 0; i < lines.Length; i++)
            {
                // Extract lyrics and time stamps
                line = lines[i];

                mc3 = r3.Matches(line);
                mc2 = r2.Matches(line);

                if (mc2.Count == 0 && mc3.Count == 0)
                    continue;

                if (mc3.Count > 0)
                {
                    // Extract time
                    stime = mc3[0].Value;
                    // Extract lyrics
                    lyric = line.Substring(mc3[0].Index + stime.Length);

                }
                else if (mc2.Count > 0)
                {
                    // Extract time
                    stime = mc2[0].Value;
                    // Extract lyrics
                    lyric = line.Substring(mc2[0].Index + stime.Length);
                }

                // Convert stime to long
                stime = stime.Substring(1, stime.Length - 2);
                time = (long)TimeToMs(stime);

                lstLyrics.Add(lyric);
                lstTimes.Add(time);                
            }

            Lyrics = new string[lstLyrics.Count];
            Times = new long[lstTimes.Count];
            synchedTexts = new SyncText[lstLyrics.Count];

            for (int i = 0; i < lstLyrics.Count; i++)
            {
                Lyrics[i] = "\r\n" + lstLyrics[i];
                Times[i] = lstTimes[i];
                synchedTexts[i] = new SyncText(Times[i], Lyrics[i]);
            }
            
            return synchedTexts;
        }


        /// <summary>
        /// Convert a time stamp 01:15.510 (min 2digits, sec 2 digits, ms 3 digits) to milliseconds
        /// </summary>
        /// <param name="stime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static double TimeToMs(string time)
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
            dur = Min * 60;

            int Sec = Convert.ToInt32(sec);
            dur += Sec * 1000;

            double Ms = Convert.ToDouble(ms);
            dur += Ms;

            return dur;
        }

    }

}
