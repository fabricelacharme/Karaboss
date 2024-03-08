using ChordsAnalyser.cchords;
using MusicXml.Domain;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass.AddOn.Enc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Karaboss.Lyrics
{
    public partial class LyricsMgmt
    {


        #region private

        /// <summary>
        /// A class to store all lyric's syllabes
        /// </summary>
        private class plLyric
        {
            public enum Types
            {
                Text = 1,
                LineFeed = 2,
                Paragraph = 3,
            }
            public Types Type { get; set; }
            public string Element { get; set; }
            public int TicksOn { get; set; }
            public int TicksOff { get; set; }
            public int Beat { get; set; }
        }

  

        private List<plLyric> plLyrics;
        private Dictionary<int, string> LyricsLines = new Dictionary<int, string>();
        private Dictionary<int, int> LyricsTimes = new Dictionary<int, int>();
        private Array LyricsLinesKeys;
        private Array LyricsTimesKeys;

        private Sequence sequence1;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;
        private int _currentMeasure = -1;
        private int _currentTimeInMeasure = -1;
        private int _currentLine = 1;

        #endregion private

        #region public
        public enum LyricTypes
        {
            Text = 0,
            Lyric = 1
        }

        private int _lyricstracknum = -1;     // num of track containing lyrics
        public int LyricsTracknum { get; set; }

        private int _melodytracknum = -1;     // num  of track containing the melody       
        public int MelodyTrackNum { get; set; }

        private LyricTypes _lyrictype;            // type lyric or text   
        public LyricTypes LyricType { get; set; }

        // Lyrics : int = time, string = syllabes in corresponding time
        private Dictionary<int, string> _gridlyrics;
        public Dictionary<int,string> Gridlyrics
        {
            get { return _gridlyrics; }
        }

        private Dictionary<int, (string, string)> _gridchords;
        public Dictionary<int, (string, string)> Gridchords
        {
            get { return _gridchords; }
            set { _gridchords = value; }
        }

        #endregion public


        public LyricsMgmt(Sequence sequence) 
        {
            _lyricstracknum = -1;
            _melodytracknum = -1;
            _lyrictype = LyricTypes.Text;
            plLyrics = new List<plLyric>();

            sequence1 = sequence;
            
            UpdateMidiTimes();

            // Extract lyrics
            ExtractLyrics();

            // Search for the melody track
            _melodytracknum = GuessMelodyTrack();

            // Fix lyrics endtime to notes of the melody track end time
            CheckTimes();

            LoadLyricsPerBeat();

            LoadLyricsLines();
        }


        /// <summary>
        /// Lyrics extraction & display
        /// </summary>
        private string ExtractLyrics()
        {
            string retval = string.Empty; //ret value (lyrics)

            string lyrics = string.Empty;
            string lyricstext = string.Empty;

            double l_text = 1;
            double l_lyric = 1;

            int nbBeatsPerMeasure = sequence1.Numerator;
            //int OneBeat = _measurelen / sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            // ----------------------------------------------------------------------
            // Objectif : comparer texte et lyriques et choisir la meilleure solution
            // ----------------------------------------------------------------------

            // track for text
            int trktext = HasLyricsText();     // Recherche si Textes
            if (trktext >= 0)
            {
                lyricstext = sequence1.tracks[trktext].TotalLyricsT;
                l_text = lyricstext.Length;
            }

            // track for lyrics
            int trklyric = HasLyrics();              // Recherche si lyrics  
            if (trklyric >= 0)
            {
                lyrics = sequence1.tracks[trklyric].TotalLyricsL;
                l_lyric = lyrics.Length;
            }

            if (trktext >= 0 && trklyric >= 0)
            {
                // regarde lequel est le plus gros... lol                
                if (l_lyric >= l_text)
                {
                    // Elimine texte et choisi les lyrics
                    trktext = -1;
                }
                else
                {
                    // Elimine lyrics et choisi les textes
                    trklyric = -1;
                }
            }

            // if lyrics are in text events
            if (trktext >= 0)
            {
                _melodytracknum = -1;
                _lyricstracknum = trktext;
                _lyrictype = LyricTypes.Text;

                lyrics = sequence1.tracks[trktext].TotalLyricsT;
                // Charge listes           
                if (plLyrics != null)
                    plLyrics.Clear();

                Sanford.Multimedia.Midi.Track track = sequence1.tracks[_lyricstracknum];

                int plTicksOn = 0;
                int plTicksOff = 0;
                
                for (int k = 0; k < track.LyricsText.Count; k++)
                {
                    // Stockage dans liste plLyrics
                    plLyric.Types plType = (plLyric.Types)track.LyricsText[k].Type;
                    string plElement = track.LyricsText[k].Element;

                    // Start time for a lyric
                    plTicksOn = track.LyricsText[k].TicksOn;

                    // Stop time for a lyric (maxi 1 beat ?)
                    if (plType == plLyric.Types.Text)
                        plTicksOff = plTicksOn + _measurelen;

                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTicksOn, TicksOff = plTicksOff });
                }

                // reduce ticksoff to tickson of next lyric
                /*
                int ticksoff;
                int nexttickson;
                string elm = string.Empty;
                for (int k = 0; k < plLyrics.Count; k++)
                {
                    ticksoff = plLyrics[k].TicksOff;
                    if (k < plLyrics.Count - 1)
                    {
                        if (plLyrics[k + 1].Type == plLyric.Types.Text)
                        {
                            nexttickson = plLyrics[k + 1].TicksOn;
                            if (ticksoff > nexttickson)
                                plLyrics[k].TicksOff = nexttickson;
                        }
                    }

                    // Add a trailing space to syllabs at the end of the lines if missing
                    if (plLyrics[k].Type == plLyric.Types.LineFeed || plLyrics[k].Type == plLyric.Types.Paragraph)
                    {
                        if (k > 0)
                        {
                            if (plLyrics[k - 1].Type == plLyric.Types.Text)
                            {
                                elm = plLyrics[k - 1].Element;
                                if (elm.Length > 0)
                                {
                                    if (elm.Substring(1, elm.Length - 1) != " ")
                                        plLyrics[k - 1].Element = elm + " ";
                                }
                            }
                        }
                    }
                }
                */
                return lyrics;
            }
            // if lyrics are in lyric events
            else
            {
                if (trklyric >= 0)
                {
                    lyrics = sequence1.tracks[trklyric].TotalLyricsL;

                    _melodytracknum = -1;
                    _lyricstracknum = trklyric;
                    _lyrictype = LyricTypes.Lyric;
                    

                    // Charge listes            
                    if (plLyrics != null)
                        plLyrics.Clear();

                    // Remove "[]" for the letter by letter lyrics
                    Sanford.Multimedia.Midi.Track track = sequence1.tracks[_lyricstracknum];
                    for (int k = 0; k < track.Lyrics.Count - 1; k++)
                    {
                        if (track.Lyrics[k].Element == "[]")
                        {
                            if (track.Lyrics[k + 1].Type == Sanford.Multimedia.Midi.Track.Lyric.Types.Text)
                            {
                                track.Lyrics[k + 1].Element = " " + track.Lyrics[k + 1].Element;
                            }
                        }
                    }

                    int plTicksOn = 0;
                    int plTicksOff = 0;
                    string elm = string.Empty;
                    for (int k = 0; k < track.Lyrics.Count; k++)
                    {
                        if (track.Lyrics[k].Element != "[]")
                        {
                            // Stockage dans liste plLyrics
                            plLyric.Types plType = (plLyric.Types)track.Lyrics[k].Type;
                            string plElement = track.Lyrics[k].Element;

                            // Start time for a lyric
                            plTicksOn = track.Lyrics[k].TicksOn;

                            // Stop time for the lyric
                            if (plType == plLyric.Types.Text)
                                plTicksOff = plTicksOn + _measurelen;    

                            plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTicksOn, TicksOff = plTicksOff });
                        }
                    }

                    // reduce ticksoff to  tickson of next lyric
                    /*
                    int ticksoff;
                    int nexttickson;
                    for (int k = 0; k < plLyrics.Count; k++)
                    {
                        ticksoff = plLyrics[k].TicksOff;
                        if (k < plLyrics.Count - 1)
                        {
                            if (plLyrics[k + 1].Type == plLyric.Types.Text)
                            {
                                nexttickson = plLyrics[k + 1].TicksOn;
                                if (ticksoff > nexttickson)
                                    plLyrics[k].TicksOff = nexttickson;
                            }
                        }

                        // Add a trailing space to syllabs at the end of the lines if missing
                        if (plLyrics[k].Type == plLyric.Types.LineFeed || plLyrics[k].Type == plLyric.Types.Paragraph)
                        {
                            if (k > 0)
                            {
                                if (plLyrics[k - 1].Type == plLyric.Types.Text)
                                {
                                    elm = plLyrics[k - 1].Element;
                                    if (elm.Length > 0)
                                    {
                                        if (elm.Substring(1, elm.Length - 1) != " ")
                                            plLyrics[k - 1].Element = elm + " ";
                                    }
                                }
                            }
                        }
                    }
                    */
                    return lyrics;

                }
                // no choice was possible
                else
                {
                    if (trklyric >= 0)
                    {
                        MessageBox.Show("This file contains lyrics events, but I am unable to use them.");
                    }

                    if (trktext >= 0)
                    {
                        MessageBox.Show("This file contains text events, but I am unable to use them.");
                    }
                }
            }
            return retval;
        }


        /// <summary>
        /// Lyrics type = Text
        /// </summary>
        /// <returns></returns>
        private int HasLyricsText()
        {
            int max = -1;
            int track = -1;
            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                if (sequence1.tracks[i].TotalLyricsT != null)
                {
                    if (sequence1.tracks[i].TotalLyricsT.Length > max)
                    {
                        // BUG : on écrit des lyrics text dans n'importe quelle piste  ???
                        max = sequence1.tracks[i].TotalLyricsT.Length;
                        track = i;
                    }
                }
            }
            return track;
        }

        /// <summary>
        /// Lyrics type = Lyric
        /// </summary>
        /// <returns></returns>
        private int HasLyrics()
        {
            string tx = string.Empty;
            int max = 0;
            int trk = -1;

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                tx = string.Empty;
                if (sequence1.tracks[i].TotalLyricsL != null)
                {
                    tx = sequence1.tracks[i].TotalLyricsL;
                    if (tx.Length > max)
                    {
                        max = tx.Length;
                        trk = i;
                    }
                }
            }
            if (max > 0)
            {
                return trk;
            }

            return -1;
        }


        /// <summary>
        /// Fix ticksoff of lyrics with melody notes
        /// </summary>
        private void CheckTimes()
        {
            int ticksoff;
            int nexttickson;
            string elm = string.Empty;
            int nbmodified = 0;

            // reduce ticksoff to  tickson of next lyric
            for (int k = 0; k < plLyrics.Count; k++)
            {
                ticksoff = plLyrics[k].TicksOff;
                if (k < plLyrics.Count - 1)
                {
                    if (plLyrics[k + 1].Type == plLyric.Types.Text)
                    {
                        nexttickson = plLyrics[k + 1].TicksOn;
                        if (ticksoff > nexttickson)
                        {
                            plLyrics[k].TicksOff = nexttickson;
                            nbmodified++;
                        }
                    }
                }


                // Add a trailing space to syllabs at the end of the lines if missing
                if (plLyrics[k].Type == plLyric.Types.LineFeed || plLyrics[k].Type == plLyric.Types.Paragraph)
                {
                    if (k > 0)
                    {
                        if (plLyrics[k - 1].Type == plLyric.Types.Text)
                        {
                            elm = plLyrics[k - 1].Element;
                            if (elm.Length > 0)
                            {
                                if (elm.Substring(1, elm.Length - 1) != " ")
                                {
                                    plLyrics[k - 1].Element = elm + " ";
                                    nbmodified++;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("******** lyrics ticksoff modified with next lyric : " + nbmodified.ToString());
            nbmodified = 0;

            // Reduce lyric Ticksoff to the tickson of the  corresponding melody note
            // If a melody track exists, and if it is less than the actual value.

            if (_melodytracknum != -1)
            {
                Sanford.Multimedia.Midi.Track trk = sequence1.tracks[_melodytracknum];
                List<MidiNote> notes = trk.Notes;


                for (int i = 0; i < notes.Count; i++)
                {
                    for (int j = 0; j < plLyrics.Count; j++)
                    {
                        if (plLyrics[j].Type == plLyric.Types.Text)
                        {
                            // Search the note starttime corresponding to the lyric (not always true)
                            if (notes[i].StartTime == plLyrics[j].TicksOn)
                            {
                                // lyric tickoff was already reduced to the next lyric tickson
                                // Set ticksoff of the lyric to the endtime of the note only if it reduces it again 
                                if (notes[i].EndTime < plLyrics[j].TicksOff)
                                {
                                    plLyrics[j].TicksOff = notes[i].EndTime;
                                    nbmodified++;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("******** lyrics ticksoff modified : " + nbmodified.ToString());
            nbmodified = 0;

            int beat;
            int beatend;
            int beatoff;
            int tickson;            
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            // Sets the beat of the Lyric
            for (int i = 0; i < plLyrics.Count; i++)
            {
                tickson = plLyrics[i].TicksOn;
                ticksoff = plLyrics[i].TicksOff;    

                // Real beat                    
                beat = 1 + tickson / beatDuration;
                beatoff = 1 + ticksoff / beatDuration;

                // In case the note is located on 2 beats, is the note more in the first one or in the second one ?
                beatend = beat * beatDuration;
                if (beatoff > beat && ticksoff - beatend > beatend - tickson)
                {
                    beat += 1;
                }
                
                //int measure = 1 + (beat - 1) / nbBeatsPerMeasure;

                plLyrics[i].Beat = beat;

            }
        }

        /// <summary>
        /// Guess which track contains the melody
        /// A very complex search :-)
        /// </summary>
        /// <returns></returns>
        private int GuessMelodyTrack()
        {
            // Comparer timing pistes à pistes
            int tracknum = _lyricstracknum;
            //Track trackly = sequence1.tracks[tracknum];
            int nbfound = 0;
            //int trackm = -1;
            int max = 0;
            int min = 5000;
            int nbnotes = 0;
            int diff = 0;

            float fRatioNotes = 0;
            float maxRatioNotes = 0;

            int maxDiff = -1;
            int trackfnote = -1;

            int delta = 30; // 20 origin

            // Eliminer les cr
            int nblyrics = 0;
            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].Type == plLyric.Types.Text && plLyrics[i].TicksOn > 0)
                    nblyrics++;
            }

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                nbfound = 0;
                nbnotes = 0;

                Sanford.Multimedia.Midi.Track track = sequence1.tracks[i];

                if (track.ContainsNotes == true && track.MidiChannel != 9)
                {

                    // comparaison 1 : nombre de notes versus nombre de lyrics
                    // Plus le nombre de notes se rapproche de celui de lyrics, plus c'est mieux
                    nbnotes = track.Notes.Count;

                    // Avoid tracks with not enough notes and those having too many notes compared to lyrics                    
                    if (nbnotes > nblyrics / 2 && nbnotes < nblyrics * 3)
                    {
                        diff = nbnotes - nblyrics;
                        if (diff < 0) diff = -diff;

                        int oldtn = -1;

                        // Search if notes have a start time corresponding of those of lytics 
                        // Search is performed in a time frame of 20 plus or minus
                        for (int j = 0; j < track.Notes.Count; j++)
                        {
                            MidiNote n = track.Notes[j];
                            int tn = n.StartTime;
                            if (tn > oldtn) // Avoid to search for all the notes belonging to a chords having the same time
                            {
                                // Search lyrics 
                                for (int k = 0; k < plLyrics.Count; k++)
                                {
                                    int tl = plLyrics[k].TicksOn;
                                    if (tl > tn - delta && tl < tn + delta)
                                    {
                                        nbfound++;
                                        break;
                                    }
                                    else if (tl > tn)
                                    {
                                        break;
                                    }
                                }

                                oldtn = tn;
                            }
                        }

                        // FAB 04/07/20
                        //nbnotes = nbfound;   // empeche d'éliminer les pistes qui ont trop de notes
                        //diff = nbnotes - nblyrics;
                        //if (diff < 0) diff = -diff;

                        // Il faudrait supprimer les lyrics qui n'ont pas de notes                        
                        diff = nbnotes - nbfound;
                        if (diff < 0) diff = -diff;

                        // TODO, which algoritm is the best ????
                        bool bchoice = false;
                        bchoice = true;

                        if (bchoice)
                        {
                            // 1st criteria "diff": tracks having the nearest number of notes than number of lyrics
                            if (diff < maxDiff || maxDiff == -1)
                            {
                                // 2nd criteria: 
                                // ratio between the number of notes having the same start time than lyrics 
                                // and the number of lyrics (ideally same number, ie 1)
                                fRatioNotes = (float)nbfound / (float)nblyrics;
                                if (fRatioNotes > 1) fRatioNotes = 1;   // FAB 04/07 origin = 1
                                if (fRatioNotes >= maxRatioNotes)
                                {
                                    maxRatioNotes = fRatioNotes;
                                    maxDiff = diff;
                                    trackfnote = i;
                                }
                            }
                        }
                        else
                        {
                            #region delete

                            if (nbfound > 0)
                            {
                                // Plus diff est petit, mieux c'est (différence entre nombre de notes et lyrics)
                                // Plus nbfound est grand, mieux c'est (notes jouées au même moment que les lyrics)
                                if (nbfound > max || diff < min)
                                {


                                    if (nbfound > 4 * nblyrics / 5)
                                    {
                                        if (nbfound - diff > max - min)
                                        {
                                            min = diff;
                                            max = nbfound;
                                            trackfnote = i;
                                        }
                                    }
                                }
                            }

                            #endregion
                        }
                    }


                } // contains notes                
            }
            //return trackm;
            return trackfnote;
        }


        /// <summary>
        /// Load lyrics in a dictionnary
        /// key : beat
        /// Value : lyrics in this beat
        /// </summary>
        private void LoadLyricsPerBeat()
        {
            _gridlyrics = new Dictionary<int, string>();
            int tickson;
            int ticksoff;
            int beat;
            int nbBeatsPerMeasure = sequence1.Numerator;
            int currentbeat = 1;
            string currenttext = string.Empty;
            int currentmeasure = 0;           

            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].Type == plLyric.Types.Text)
                {
                    tickson = plLyrics[i].TicksOn;
                    ticksoff = plLyrics[i].TicksOff;                    
                    beat = plLyrics[i].Beat;
                                        
                    // New beat
                    // Store previous syllabes
                    if (beat != currentbeat)
                    {
                        currentmeasure = 1 + (currentbeat - 1) / nbBeatsPerMeasure;

                        try
                        {
                            _gridlyrics.Add(currentbeat, currenttext);
                        }
                        catch (Exception ex)
                        {
                            string tx = ex.Message + "\r\n" + "Syllab :" + currenttext + "\r\n" + "Measure: " + currentmeasure;
                            MessageBox.Show(tx, "Karaboss",MessageBoxButtons.OK, MessageBoxIcon.Error); 
                        }
                        currentbeat = beat;
                        currenttext = string.Empty;
                    }
                    // Add syllabe to currenttext
                    currenttext += plLyrics[i].Element;
                }
            }

            // Last word ?
            currentmeasure = 1 + (currentbeat - 1) / nbBeatsPerMeasure;
            try
            {
                _gridlyrics.Add(currentbeat, currenttext);
            }
            catch (Exception ex)
            {
                string tx = ex.Message + "\r\n" + "Syllab :" + currenttext + "\r\n" + "Measure: " + currentmeasure;
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        /// <summary>
        /// Load Lyrics lines in Dictionaries LyrictLines & LyricsTimes
        /// </summary>
        private void LoadLyricsLines()
        {
            LyricsLines = new Dictionary<int, string>();  // tickson of the first lyric, line of lyrics
            LyricsTimes = new Dictionary<int, int>();     // tickson of the first lyric, ticksoff of the last lyric

            string line = string.Empty;
            bool newline = false;
            int ticksoff = 0;
            int curindex = -1;

            int currenttime = 0;
            int newtime = 0;

            for (int i = 0; i < plLyrics.Count; i++)
            {
                // if new line
                if (plLyrics[i].Type == plLyric.Types.LineFeed || plLyrics[i].Type == plLyric.Types.Paragraph)
                {
                    // Case several lines with the same start time
                    newtime = plLyrics[i].TicksOn;
                    if (newtime != currenttime)
                    {
                        newline = true;
                        currenttime = newtime;
                    }
                }
                else
                {
                    // the first item after newline
                    if (newline)
                    {
                        if (line.Trim() != "")
                        {
                            try
                            {
                                LyricsLines.Add(curindex, line);
                                LyricsTimes.Add(curindex, ticksoff);
                            }
                            catch (Exception ex)
                            {
                                string tx = ex.Message + "\r\n" + "Line: " + line;
                                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        newline = false;
                        line = string.Empty;
                    }

                    // Last line (the last line will not have a new line event)
                    if (line.Trim() == "")
                    {
                        curindex = plLyrics[i].TicksOn;
                    }

                    // others items of a line
                    line += plLyrics[i].Element;
                    ticksoff = plLyrics[i].TicksOff;
                }
            }


            // Do not forget last line
            if (line.Trim() != "")
            {
                LyricsLines.Add(curindex, line);
                LyricsTimes.Add(curindex, ticksoff);
            }

            if (LyricsLines.Count > 0)
            {
                LyricsLinesKeys = LyricsLines.Keys.ToArray();
                LyricsTimesKeys = LyricsTimes.Keys.ToArray();
            }
        }

 

        #region Display Lyrics

        /// <summary>
        /// return the line of lyrics related to pos
        /// </summary>
        /// <param name="pos"></param>
        public string DisplayLineLyrics(int pos)
        {
            if (LyricsLinesKeys == null)
                return "";

            int lyrictickson = 0;
            int lyricticksoff = 0;
            //lLyrics.Text = "";
            bool bfound = false;
            string txtcontent = string.Empty;

            for (int i = LyricsLinesKeys.Length - 1; i >= 0; i--)
            {
                lyrictickson = (int)LyricsLinesKeys.GetValue(i);
                lyricticksoff = LyricsTimes[lyrictickson];

                if (lyrictickson <= pos && pos <= lyricticksoff)
                {
                    bfound = true;
                    break;
                }
            }

            if (bfound)
            {
                return LyricsLines[lyrictickson];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Display lyrics lines except line being played
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public string DisplayOtherLinesLyrics(int pos)
        {
            if (LyricsLinesKeys == null)
                return "";
            string res = string.Empty;
            foreach (KeyValuePair<int,string> var in LyricsLines)
            {
                if (var.Key > pos)
                {
                    res += var.Value + "\r\n";
                }
            }
            return res;
           
        }


           
        private float GetTimeInMeasure(int ticks)
        {
            // Num measure
            int curmeasure = 1 + ticks / _measurelen;
            // Temps dans la mesure
            float timeinmeasure = sequence1.Numerator - ((curmeasure * _measurelen - ticks) / (float)(_measurelen / sequence1.Numerator));

            return timeinmeasure;
        }

        /// <summary>
        /// TAB 3: Display words & lyrics
        /// </summary>
        /// <returns></returns>
        public string DisplayWordsAndChords()
        {
            // New version with all beats
            string res = string.Empty;
            string cr = "\r\n";
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beats = NbMeasures * nbBeatsPerMeasure;
            int beat;
            string chord = string.Empty;
            string beatchord = string.Empty;
            string beatlyr = string.Empty;
            string linebeatlyr = string.Empty;
            string linebeatchord = string.Empty;
            plLyric pll;
            string lyr;
            bool bfound = false;

            for (int measure = 1; measure <= NbMeasures; measure++)
            {
                for (int timeinmeasure = 1; timeinmeasure <= nbBeatsPerMeasure; timeinmeasure++)
                {
                    beat = (measure - 1) * nbBeatsPerMeasure + timeinmeasure;                    
                    var kvChord = Gridchords[measure];
                    bfound = false;

                    for (int i = 0; i < plLyrics.Count; i++)
                    {
                        pll = plLyrics[i];
                        if (pll.Beat == beat)
                        {
                            bfound = true; 
                            
                            if (pll.Type == plLyric.Types.Paragraph || pll.Type == plLyric.Types.LineFeed) 
                            {
                                linebeatlyr += beatlyr;
                                linebeatchord += beatchord;

                                if (linebeatlyr != "" || linebeatchord != "")
                                {
                                    // New Line => stor result
                                    if (pll.Type == plLyric.Types.LineFeed)
                                    {
                                        if (linebeatchord != "" && linebeatlyr != "")
                                            res += linebeatchord + cr + linebeatlyr + cr;
                                        else if (linebeatchord != "")
                                            res += linebeatchord + cr;
                                        else if (linebeatlyr != "")
                                            res += linebeatlyr + cr;
                                    }
                                    else
                                    {
                                        if (linebeatchord != "" && linebeatlyr != "")
                                            res += linebeatchord + cr + linebeatlyr + cr + cr;
                                        else if (linebeatchord != "")
                                            res += linebeatchord + cr + cr;
                                        else if (linebeatlyr != "")
                                            res += linebeatlyr + cr + cr;
                                    }
                                }

                                linebeatchord = string.Empty;
                                linebeatlyr = string.Empty;
                                beatlyr = string.Empty;
                                beatchord = string.Empty;

                                #region store line
                                /*
                                if (timeinmeasure == 1)
                                {
                                    // Chord 1
                                    chord = kvChord.Item1;
                                    if (chord == "<Empty>")
                                        chord = "";
                                    beatchord = chord;

                                    if (beatlyr.Length > chord.Length)
                                    {
                                        beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                                    }
                                    else if (beatlyr.Length < chord.Length)
                                    {
                                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                                    }
                                }
                                else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                                {
                                    // Chord 2
                                    chord = kvChord.Item2;
                                    if (chord == "<Empty>")
                                        chord = "";
                                    beatchord = chord;

                                    if (beatlyr.Length > chord.Length)
                                    {
                                        beatchord += new string(' ', beatlyr.Length - chord.Length);
                                    }
                                    else if (beatlyr.Length < chord.Length)
                                    {
                                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                                    }
                                }
                                else
                                {
                                    // No chord
                                    if (beatlyr.Length > 0)
                                        beatchord += new string(' ', beatlyr.Length);
                                }
                                */
                                #endregion store line

                                //linebeatlyr += beatlyr;
                                //linebeatchord += beatchord;

                                
                            }
                            else if (pll.Type == plLyric.Types.Text)
                            {
                                #region store previous line
                                if (timeinmeasure == 1)
                                {
                                    // Chord 1
                                    chord = kvChord.Item1;
                                    if (chord == "<Empty>")
                                        chord = "";
                                    beatchord = chord;

                                    if (beatlyr.Length > chord.Length)
                                    {
                                        beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                                    }
                                    else if (beatlyr.Length < chord.Length)
                                    {
                                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                                    }
                                }
                                else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                                {
                                    // Chord 2
                                    chord = kvChord.Item2;
                                    if (chord == "<Empty>")
                                        chord = "";
                                    beatchord = chord;

                                    if (beatlyr.Length > chord.Length)
                                    {
                                        beatchord += new string(' ', beatlyr.Length - chord.Length);
                                    }
                                    else if (beatlyr.Length < chord.Length)
                                    {
                                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                                    }
                                }
                                else
                                {
                                    // No chord
                                    if (beatlyr.Length > 0)
                                        beatchord += new string(' ', beatlyr.Length);
                                }
                                #endregion store line

                                linebeatlyr += beatlyr;
                                linebeatchord += beatchord;
                                beatlyr = string.Empty;
                                beatchord = string.Empty;


                                lyr = pll.Element;
                                beatlyr += lyr;
                            }
                            
                        }
                        else if (pll.Beat > beat)
                        {
                            break;
                        }
                    }

                    if(!bfound)
                    {
                        // No lyrics found, maybe chords ?
                        if (timeinmeasure == 1)
                        {
                            // Chord 1
                            chord = kvChord.Item1;
                            if (chord == "<Empty>")
                                chord = "";
                            beatchord = chord + " ";
                        }
                        else if (timeinmeasure == 2)
                        {
                            // Chord 2
                            chord = kvChord.Item2;
                            if (chord == "<Empty>")
                                chord = "";
                            beatchord = chord + " ";
                        }

                        if (beatchord.Trim() != "")
                        {
                            if (timeinmeasure == nbBeatsPerMeasure)
                                linebeatchord += beatchord + cr;
                            else
                                linebeatchord += beatchord;
                        }
                    }

                    

                }
            }

            // Store last line
            #region store result
            var ttz = Gridchords[NbMeasures];
            int timeinmeasur = nbBeatsPerMeasure;

            if (timeinmeasur == 1)
            {
                // Chord 1
                chord = ttz.Item1;
                if (chord == "<Empty>")
                    chord = "";
                beatchord = chord;

                if (beatlyr.Length > chord.Length)
                {
                    beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                }
                else if (beatlyr.Length < chord.Length)
                {
                    beatlyr += new string(' ', chord.Length - beatlyr.Length);
                }
            }
            else if (timeinmeasur == 1 + nbBeatsPerMeasure / 2)
            {
                // Chord 2
                chord = ttz.Item2;
                if (chord == "<Empty>")
                    chord = "";
                beatchord = chord;

                if (beatlyr.Length > chord.Length)
                {
                    beatchord += new string(' ', beatlyr.Length - chord.Length);
                }
                else if (beatlyr.Length < chord.Length)
                {
                    beatlyr += new string(' ', chord.Length - beatlyr.Length);
                }
            }
            else
            {
                // No chord
                if (beatlyr.Length > 0)
                    beatchord += new string(' ', beatlyr.Length);
            }
            #endregion store result

            linebeatlyr += beatlyr;
            linebeatchord += beatchord;
            if (linebeatlyr != "" || linebeatchord != "")
            {
                // New Line => store result                
                res += linebeatchord + cr + linebeatlyr;
            }

            return res;
        }
        public string DisplayWordsAndChords2()
        {
            string res = string.Empty;
            string cr = "\r\n";
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int tickson;
            int ticksoff;           
            int beat;            
            int currentbeat = -1;
            int currentmeasure = 1;
            int measure;
            string chord = string.Empty;
            int timeinmeasure;
            string beatlyr = string.Empty;
            string beatchord = string.Empty;
            string linebeatchord = string.Empty;
            string linebeatlyr = string.Empty;
            string lyr = string.Empty;            

            for (int i = 0; i < plLyrics.Count; i++)
            {
                plLyric pll = plLyrics[i];

                // =========================================
                // Linefeed
                // =========================================
                if (pll.Type == plLyric.Types.LineFeed || pll.Type == plLyric.Types.Paragraph) 
                {                                                                                                                                            
                    if (currentmeasure < Gridchords.Count)
                    {
                        // Store results of previous beat
                        #region store previous
                        timeinmeasure = nbBeatsPerMeasure + currentbeat - currentmeasure * nbBeatsPerMeasure;
                        var tty = Gridchords[currentmeasure];

                        if (timeinmeasure == 1)
                        {
                            // Chord 1
                            chord = tty.Item1;
                            if (chord == "<Empty>")
                                chord = "";
                            beatchord = chord;

                            if (beatlyr.Length > chord.Length)
                            {
                                beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                            }
                            else if (beatlyr.Length < chord.Length)
                            {
                                beatlyr += new string(' ', chord.Length - beatlyr.Length);
                            }
                        }
                        else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                        {
                            // Chord 2
                            chord = tty.Item2;
                            if (chord == "<Empty>")
                                chord = "";

                            beatchord = chord;

                            if (beatlyr.Length > chord.Length)
                            {
                                beatchord += new string(' ', beatlyr.Length - chord.Length);
                            }
                            else if (beatlyr.Length < chord.Length)
                            {
                                beatlyr += new string(' ', chord.Length - beatlyr.Length);
                            }
                        }
                        else
                        {
                            // No chord
                            if (beatlyr.Length > 0)
                                beatchord += new string(' ', beatlyr.Length);
                        }

                        #endregion store result

                        linebeatlyr += beatlyr;
                        linebeatchord += beatchord;

                        if (linebeatlyr != "" || linebeatchord != "")
                        {
                            // New Line => stor result
                            if (pll.Type == plLyric.Types.LineFeed)
                                res += linebeatchord + cr + linebeatlyr + cr;
                            else
                                res += linebeatchord + cr + linebeatlyr + cr + cr;
                        }
                    }

                    linebeatchord = string.Empty;
                    linebeatlyr = string.Empty;
                    beatlyr = string.Empty;
                    beatchord = string.Empty;
                    currentbeat = -1;

                }
                // =========================================
                // Text
                // =========================================
                else if (pll.Type == plLyric.Types.Text)
                {
                    tickson = pll.TicksOn;
                    ticksoff = pll.TicksOff;

                    beat = pll.Beat;                   
                    measure = 1 + (beat - 1) / nbBeatsPerMeasure;
                    lyr = pll.Element;

                    if (currentbeat == -1)
                    {
                        currentbeat = beat;
                        currentmeasure = measure;
                    }

                    // If beat change
                    if (beat != currentbeat)
                    {
                        // Store results of previous beat
                        #region store result
                        // Store results of previous beat                        
                        timeinmeasure = nbBeatsPerMeasure + currentbeat - currentmeasure * nbBeatsPerMeasure;

                        var tty = Gridchords[currentmeasure];

                        if (timeinmeasure == 1)
                        {
                            // Chord 1
                            chord = tty.Item1;
                            if (chord == "<Empty>")
                                chord = "";
                            beatchord = chord;                            

                            if (beatlyr.Length > chord.Length)
                            {
                                beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                            }
                            else if (beatlyr.Length < chord.Length)
                            {
                                beatlyr += new string(' ', chord.Length - beatlyr.Length);
                            }
                        }
                        else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                        {
                            // Chord 2
                            chord = tty.Item2;
                            if (chord == "<Empty>")
                                chord = "";
                            beatchord = chord;                            

                            if (beatlyr.Length > chord.Length)
                            {
                                beatchord += new string(' ', beatlyr.Length - chord.Length);
                            }
                            else if (beatlyr.Length < chord.Length)
                            {
                                beatlyr += new string(' ', chord.Length - beatlyr.Length);
                            }
                        }
                        else
                        {
                            // No chord
                            if (beatlyr.Length > 0)
                                beatchord += new string(' ', beatlyr.Length);
                        }

                        #endregion store result

                        // update current
                        currentbeat = beat;
                        currentmeasure = measure;

                        linebeatlyr += beatlyr;
                        linebeatchord += beatchord;
                        beatlyr = string.Empty;
                        beatchord = string.Empty;
                    }

                    // If same beat and same measure
                    beatlyr += lyr;                                        
                }
            }

            // Do not forget the last line
            if (currentmeasure < Gridchords.Count)
            {
                // Store last line
                #region store result
                var ttz = Gridchords[currentmeasure];
                timeinmeasure = nbBeatsPerMeasure + currentbeat - currentmeasure * nbBeatsPerMeasure;

                if (timeinmeasure == 1)
                {
                    // Chord 1
                    chord = ttz.Item1;
                    if (chord == "<Empty>")
                        chord = "";
                    beatchord = chord;

                    if (beatlyr.Length > chord.Length)
                    {
                        beatchord += new string(' ', beatlyr.Length - beatchord.Length);
                    }
                    else if (beatlyr.Length < chord.Length)
                    {
                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                    }
                }
                else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                {
                    // Chord 2
                    chord = ttz.Item2;
                    if (chord == "<Empty>")
                        chord = "";
                    beatchord = chord;

                    if (beatlyr.Length > chord.Length)
                    {
                        beatchord += new string(' ', beatlyr.Length - chord.Length);
                    }
                    else if (beatlyr.Length < chord.Length)
                    {
                        beatlyr += new string(' ', chord.Length - beatlyr.Length);
                    }
                }
                else
                {
                    // No chord
                    if (beatlyr.Length > 0)
                        beatchord += new string(' ', beatlyr.Length);
                }
                #endregion store result

                linebeatlyr += beatlyr;
                linebeatchord += beatchord;
                if (linebeatlyr != "" || linebeatchord != "")
                {
                    // New Line => store result                
                    res += linebeatchord + cr + linebeatlyr;
                }
            }

            return res;
        }

        #endregion Display Lyrics


        #region midi
        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                NbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer
            }
        }

        #endregion midi
    }
}
