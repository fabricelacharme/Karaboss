using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Karaboss.Resources;

namespace Karaboss.Pages.ABCnotation
{
    public class Song
    {

        public enum PlayType { Immediate, Sync };

        public String FileName { get; set; }
        public String Title { get; set; }
        public String Notes { get; set; }
        public String Key { get; set; }
        public String Unit { get; set; }
        public String Tempo { get; set; }
        public String Meter { get; set; }
        public String Author { get; set; }
        public String Origin { get; set; }
        public String History { get; set; }
        public String Transcriber { get; set; }
        public String Index { get; set; }
        public String Text { get; set; }
        public String ShortName
        {
            private set { return; }
            get { return FileName.Substring((Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Resources.ABCResource.MusicSubfolder).Length); }
        }
        public String ToolTip
        {
            private set { return; }
            get { return Index + " " + Title + "\t" + Author + "\n" + Meter + " time in " + Key + " at " + Tempo + (Notes.Length > 0 ? "\n" + Notes : ""); }
        }
        public int FirstLine { get; set; }
        public Song()
        {   //====================================================================
            FileName = String.Empty;
            Index = String.Empty;
            Title = String.Empty;
            Notes = String.Empty;
            Key = String.Empty;
            Unit = String.Empty;
            Tempo = String.Empty;
            Meter = String.Empty;
            Author = String.Empty;
            Origin = String.Empty;
            History = String.Empty;
            Transcriber = String.Empty;
            Text = String.Empty;
            FirstLine = 0;
            return;
        }


        public bool IsValid()
        {   //====================================================================
            // Fairly brain-dead test... did we get any of the interesting tags?
            if (Index.Length > 0 || Title.Length > 0 || Meter.Length > 0 || Key.Length > 0) return true;
            return false;
        }

        public void Play(PlayType type)
        {   //====================================================================
            String strCommand = String.Empty;
            switch (type)
            {
                default:
                    throw new Exception("Unknown type of performance");

                case PlayType.Immediate:
                    //strCommand = String.Format(Resources.PlayFileCommand, ShortName, Index);
                    break;
                case PlayType.Sync:
                    //strCommand = String.Format(Resources.PlaySyncCommand, ShortName, Index);
                    break;
            }
            //RemoteController.SendText(strCommand);
            return;
        }

        public bool Save()
        {   //====================================================================
            return SaveAs(FileName);
        }

        public bool SaveAs(String strFileName)
        {   //====================================================================
            // Okay, we may have the whole file or we may not.
            // Open the base file, get everything up to the tag, get our text, get everything after our text
            // Read the file into a string 
            // Test cases:
            //  - Empty file to full
            //  - Emptying file
            //  - Empty file to empty
            //  - X: first char of old and new, one part
            //  - Text before first X:, one part
            //  - X: first char of old and new, multi-part
            //  - Last part
            //  - Middle part
            FileInfo fi = new FileInfo(FileName);
            StreamReader sr = new StreamReader(fi.FullName);
            String s = StringExtensions.ConvertNonDosFile(sr.ReadToEnd());
            sr.Close();

            Regex regexStart = new Regex(Resources.ABCResource.ABCTagIndex + @"\s*" + Index + @"\s");
            Match matchStart = regexStart.Match(s);
            int iEndPartOne = matchStart.Index;

            Regex regexEnd = new Regex(Resources.ABCResource.ABCTagIndex + @"\s*\d");
            Match matchEnd = regexEnd.Match(s, iEndPartOne + 1);
            int iStartPartTwo = matchEnd.Index > 0 ? matchEnd.Index : s.Length;

            String sSave = s.Substring(0, iEndPartOne) + Text + s.Substring(iStartPartTwo);
            StreamWriter sw = new StreamWriter(strFileName);
            sw.Write(sSave);
            sw.Flush();
            sw.Close();
            return true;
        }

        public bool SaveAsSeparateFile(String strFileName)
        {   //====================================================================
            StreamWriter sw = new StreamWriter(strFileName);
            sw.Write(Text);
            sw.Flush();
            sw.Close();
            return false;
        }

        public bool Delete()
        {
            return false;
        }


        //====================================================================
        //====================================================================
        //====================================================================
        public static Song CreateFromText(String str, int nFirstLine, String strFileName)
        {   //====================================================================
            Song song = new Song();
            song.Text = str;
            song.FileName = strFileName;
            song.FirstLine = nFirstLine;

            String[] asLines = song.Text.Split('\n'); //TODO: Is this too slow?
            foreach (String s in asLines)
            {
                if (ABC.IsHeader(s)) //TODO: Is this test a speedup or a slowdown?
                {
                    if (ABC.IsTitle(s)) { song.Title = StringExtensions.ConcatLines(song.Title, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsAuthor(s)) { song.Author = StringExtensions.ConcatList(song.Author, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsHistory(s)) { song.History = StringExtensions.ConcatLines(song.History, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsKey(s)) { song.Key = StringExtensions.RightOf(ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsMeter(s)) { song.Meter = StringExtensions.RightOf(ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsNotes(s)) { song.Notes = StringExtensions.ConcatLines(song.Notes, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsOrigin(s)) { song.Origin = StringExtensions.ConcatLines(song.Notes, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsTempo(s)) { song.Tempo = StringExtensions.RightOf(ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsUnit(s)) { song.Unit = StringExtensions.RightOf(ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsTranscriber(s)) { song.Transcriber = StringExtensions.ConcatLines(song.Transcriber, ABC.RemoveComments(s), ":"); }
                    else
                    if (ABC.IsIndex(s)) { song.Index = StringExtensions.RightOf(ABC.RemoveComments(s), ":"); }
                } // Header line
            }
            return song.IsValid() ? song : null;
        }

        public static List<Song> SongsFromFile(FileInfo fi)
        {   //====================================================================
            List<Song> lst = new List<Song>();

            // Read the file into a string 
            StreamReader sr = new StreamReader(fi.FullName);
            String s = sr.ReadToEnd();
            s = StringExtensions.ConvertNonDosFile(s);

            int iStart = 0;
            int iEnd = 0;

            char[] achLineSeparators = new char[] { '\n' };

            do
            {
                iEnd = s.IndexOf(Resources.ABCResource.ABCTagIndex, iStart + 1, StringComparison.CurrentCultureIgnoreCase);

                int nLinesBefore = s.Substring(0, iStart).Split(achLineSeparators, StringSplitOptions.RemoveEmptyEntries).Length + 1;

                Song song = null;
                if (-1 != iEnd)
                {
                    song = CreateFromText(s.Substring(iStart, iEnd - iStart), nLinesBefore, fi.FullName);
                }
                else
                {
                    song = CreateFromText(s.Substring(iStart), nLinesBefore, fi.FullName);
                }
                if (song != null) lst.Add(song);

                iStart = iEnd;
            } while (iEnd != -1);

            return lst;
        }
    }
}
