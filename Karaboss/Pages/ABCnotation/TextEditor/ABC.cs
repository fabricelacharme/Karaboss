using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Karaboss.Pages.ABCnotation
{
    public class ABC
    {

        public enum Octave { UNKNOWN, LOW, MED, HIGH }
        public class Pitch
        {
            public string Note { get; private set; }
            public Octave Octave { get; private set; }
            public Pitch(string s, Octave o)
            {
                Octave = o;
                Note = null;
                if (PITCH_REGEX.IsMatch(s)) Note = s;
            }
            public static implicit operator string(Pitch rhs) { return rhs.Note == null ? String.Empty : rhs.Note; }
        }

        private static Regex HEADER_REGEX = new Regex("^[ \t]*([^ \t]:|%)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static Regex PITCH_REGEX = new Regex("[_=^]*[zZa-gA-G][,']*", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public static bool IsHeader(String s) { return s.Length == 0 || HEADER_REGEX.IsMatch(s); }

        public static bool IsTitle(String s) { return s.StartsWith(Resources.ABCResource.ABCTagTitle, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsNotes(String s) { return s.StartsWith(Resources.ABCResource.ABCTagNote, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsKey(String s) { return s.StartsWith(Resources.ABCResource.ABCTagKey, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsUnit(String s) { return s.StartsWith(Resources.ABCResource.ABCTagUnit, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsTempo(String s) { return s.StartsWith(Resources.ABCResource.ABCTagTempo, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsMeter(String s) { return s.StartsWith(Resources.ABCResource.ABCTagMeter, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsAuthor(String s) { return s.StartsWith(Resources.ABCResource.ABCTagAuthor, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsOrigin(String s) { return s.StartsWith(Resources.ABCResource.ABCTagOrigin, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsHistory(String s) { return s.StartsWith(Resources.ABCResource.ABCTagHistory, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsTranscriber(String s) { return s.StartsWith(Resources.ABCResource.ABCTagTranscriber, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsLyrics(String s) { return s.StartsWith(Resources.ABCResource.ABCTagLyrics, StringComparison.CurrentCultureIgnoreCase); }
        public static bool IsIndex(String s) { return s.StartsWith(Resources.ABCResource.ABCTagIndex, StringComparison.CurrentCultureIgnoreCase); }

        public static List<String> ParseLineAsPitches(String s)
        {//====================================================================
            List<String> astr = new List<string>();
            if (!IsHeader(s))
            {
                MatchCollection mc = PITCH_REGEX.Matches(s);
                foreach (Match m in mc)
                {
                    astr.Add(m.Value);
                }
            }
            return astr;
        }

        public static String RemoveHeaderTag(String s)
        {   //--------------------------------------------------------------------
            if (!IsHeader(s)) return s;
            try
            {
                //TODO: Remove %% headers as well
                int nStartHeader = s.IndexOf(':');
                return s.Substring(nStartHeader + 1);
            }
            catch { }
            return s;
        }

        public static String RemoveComments(String s)
        {//--------------------------------------------------------------------
            int iComment = s.IndexOf('%');
            if (-1 != iComment) return s.Substring(0, iComment - 1);
            return s;
        }

        //====================================================================
        public enum PITCH_ERROR { NOERROR, TOO_HIGH, TOO_LOW };
        public class PitchError : IComparable
        {
            public String Pitch { get; private set; }
            public int LocStart { get; private set; }
            public int Length { get; private set; }
            public PITCH_ERROR Error { get; private set; }

            public PitchError() { Pitch = String.Empty; LocStart = -1; Length = -1; Error = PITCH_ERROR.NOERROR; return; }
            public PitchError(String sPitch, int nStart, PITCH_ERROR err)
            {
                Pitch = sPitch;
                LocStart = nStart;
                Length = Pitch.Length;
                Error = err;
                return;
            }

            #region IComparable Members
            public int CompareTo(object obj)
            {
                return ((PitchError)obj).LocStart - LocStart;
            }
            #endregion
        }
    }

    class ABCLine
    {
        public String Text { get; set; }
        public int SourceLine { get; set; }

        public ABCLine() { Text = ""; SourceLine = -1; }
        public ABCLine(String s) { Text = s; SourceLine = -1; }
        public ABCLine(String s, int line) { Text = s; SourceLine = line; }


        public bool IsHeader { get { return ABC.IsHeader(Text); } private set { } }
        public bool IsTitle { get { return ABC.IsTitle(Text); } private set { } }
        public bool IsNotes { get { return ABC.IsNotes(Text); } private set { } }
        public bool IsKey { get { return ABC.IsKey(Text); } private set { } }
        public bool IsUnit { get { return ABC.IsUnit(Text); } private set { } }
        public bool IsTempo { get { return ABC.IsTempo(Text); } private set { } }
        public bool IsMeter { get { return ABC.IsMeter(Text); } private set { } }
        public bool IsAuthor { get { return ABC.IsAuthor(Text); } private set { } }
        public bool IsOrigin { get { return ABC.IsOrigin(Text); } private set { } }
        public bool IsHistory { get { return ABC.IsHistory(Text); } private set { } }
        public bool IsTranscriber { get { return ABC.IsTranscriber(Text); } private set { } }
        public bool IsLyrics { get { return ABC.IsLyrics(Text); } private set { } }

        public List<String> Pitches { get { return ABC.ParseLineAsPitches(Text); } private set { } }

        public override string ToString()
        {
            return Text.ToString();
        }
    }

    public class SongDetails
    {
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
        public String Lyrics { get; set; }
        public String Index { get; set; }

        public SongDetails()
        {
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
            Lyrics = String.Empty;
        }
    }

    [Serializable()]
    public class FavoriteSongs
    {
        [XmlArray()] public List<FavoriteSong> Items { get; set; }
        public FavoriteSongs() { Items = new List<FavoriteSong>(); }
    }

    [Serializable()]
    public class FavoriteSong
    {  // Minimum description of a song. Needs to have value equals so we can compare two different ones in the List<>
        public String FileName { get; set; }
        [XmlIgnore()] public String SongName { get; set; }
        public String Index { get; set; }

        public FavoriteSong() { FileName = String.Empty; SongName = String.Empty; Index = String.Empty; }
        public FavoriteSong(String strFileName) { FileName = strFileName; SongName = String.Empty; Index = String.Empty; }
        public FavoriteSong(String strFileName, String strIndex) { FileName = strFileName; SongName = String.Empty; Index = strIndex; }
        public FavoriteSong(String strFileName, String strIndex, String strSongName) { FileName = strFileName; SongName = strSongName; Index = strIndex; }

        // Value equals so we can just add and remove from the list
        public override bool Equals(object obj)
        {
            if (!(obj is FavoriteSong)) return false;
            return Equals((FavoriteSong)obj);
        }

        public bool Equals(FavoriteSong fav)
        {
            return (fav.FileName == FileName) && (fav.Index == Index);
        }

        public override int GetHashCode()
        {
            return FileName.GetHashCode() ^ Index.GetHashCode();
        }

        public override string ToString()
        {   // I don't think this is called anymore, but it doesn't hurt to have it for the debugger
            return SongName + "\t   (" + FileName + " - " + Index + ")";
        }
    }
}
