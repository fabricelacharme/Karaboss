using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Karaboss.Lyrics
{
    public partial class LyricsMgmt
    {


        #region private

        public List<plLyric> plLyrics {get; set;}

        private Dictionary<int, string> LyricsLines = new Dictionary<int, string>();
        private Dictionary<int, int> LyricsTimes = new Dictionary<int, int>();
        private Array LyricsLinesKeys;
        private Array LyricsTimesKeys;

        private Sequence sequence1;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        //private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;
        
        // Are lyrics stored with a space or not ?
        private lyricsSpacings _lyricsspacing = lyricsSpacings.WithoutSpace;

        #endregion private

        #region public
        private string _lyrics = string.Empty;
        public string Lyrics
        {
            get { return _lyrics; }
        }


        private int _lyricstracknum = -1;     // num of track containing lyrics
        public int LyricsTrackNum 
        { 
            get { return _lyricstracknum; }
            set { _lyricstracknum = value; } 
        }

        private int _melodytracknum = -1;     // num  of track containing the melody       
        public int MelodyTrackNum 
        {
            get { return _melodytracknum; }
            set {_melodytracknum = value;} 
        }

        
        private LyricTypes _lyrictype;            // type lyric or text   
        public LyricTypes LyricType 
        {
            get { return _lyrictype; }
            set { _lyrictype = value; }
        }

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
            
            // FAB 28/08
            //_lyrictype = LyricTypes.Text;
            _lyrictype = LyricTypes.None;

            plLyrics = new List<plLyric>();

            sequence1 = sequence;
            
            UpdateMidiTimes();

            // Extract lyrics
            _lyrics = ExtractLyrics();

            // Guess spacing or not
            if (plLyrics.Count > 0)
            {
                _lyricsspacing = GetLyricsSpacingModel();
                if (_lyricsspacing == lyricsSpacings.WithoutSpace)
                {
                    SetTrailingSpace();
                }
            }

            // Search for the melody track
            _melodytracknum = GuessMelodyTrack();

            // Fix lyrics endtime to notes of the melody track end time
            CheckTimes();

            
            //LoadLyricsPerBeat();

            //LoadLyricsLines();
        }


        #region private func

        private lyricsSpacings GetLyricsSpacingModel() 
        {
            // What is the type of separator between lyrics ? space or nothing
            // If there is a space before or after the string, the lyrics are separated by a space
            for (int k = 0; k < plLyrics.Count; k++)
            {
                string s = plLyrics[k].Element;
                
                if (plLyrics[k].CharType == plLyric.CharTypes.Text)
                {
                    if (s.StartsWith(" ") || s.EndsWith(" ")) 
                    {
                        return lyricsSpacings.WithSpace;
                    }
                    /*
                    if (!(s.StartsWith(" ") || s.EndsWith(" ")) && (!s.EndsWith("-")))
                    {
                        return lyricsSpacings.WithSpace;
                    }
                    */
                }
            }
            return lyricsSpacings.WithoutSpace;

        }

        /// <summary>
        /// Add a trailing space when the lyrics stored in the midi file have no space.
        /// </summary>
        private void SetTrailingSpace()
        {
            for (int k = 0; k < plLyrics.Count; k++)
            {
                string s = plLyrics[k].Element;

                if (plLyrics[k].CharType == plLyric.CharTypes.Text)
                {
                    if (s != string.Empty)
                    {
                        //FAB 28/05/2024 : lyriques sans espace ?
                        if (_lyricsspacing == lyricsSpacings.WithoutSpace)
                        {
                            if (!(s.StartsWith(" ") || s.EndsWith(" ")) && (!s.EndsWith("-")))
                            {
                                s += " ";
                            }
                            else if (s.EndsWith("-") && s.Length > 1)
                            {
                                s = s.Substring(0, s.Length - 1);
                            }
                            plLyrics[k].Element = s;
                        }
                    }
                }
            }
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
                    plLyric.CharTypes plType = (plLyric.CharTypes)track.LyricsText[k].Type;
                    string plElement = track.LyricsText[k].Element;

                    // Start time for a lyric
                    plTicksOn = track.LyricsText[k].TicksOn;

                    // Stop time for a lyric (maxi 1 beat ?)
                    if (plType == plLyric.CharTypes.Text)
                        plTicksOff = plTicksOn + _measurelen;

                    plLyrics.Add(new plLyric() { CharType = plType, Element = plElement, TicksOn = plTicksOn, TicksOff = plTicksOff });
                }

                
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
                            plLyric.CharTypes plType = (plLyric.CharTypes)track.Lyrics[k].Type;
                            string plElement = track.Lyrics[k].Element;

                            // Start time for a lyric
                            plTicksOn = track.Lyrics[k].TicksOn;

                            // Stop time for the lyric
                            if (plType == plLyric.CharTypes.Text)
                                plTicksOff = plTicksOn + _measurelen;    

                            plLyrics.Add(new plLyric() { CharType = plType, Element = plElement, TicksOn = plTicksOn, TicksOff = plTicksOff });
                        }
                    }

                    
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

            // ===============================================
            // reduce ticksoff to  tickson of next lyric
            // ===============================================
            for (int k = 0; k < plLyrics.Count; k++)
            {
                ticksoff = plLyrics[k].TicksOff;
                if (k < plLyrics.Count - 1)
                {
                    if (plLyrics[k + 1].CharType == plLyric.CharTypes.Text)
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
                if (plLyrics[k].CharType == plLyric.CharTypes.LineFeed || plLyrics[k].CharType == plLyric.CharTypes.ParagraphSep)
                {
                    if (k > 0)
                    {
                        if (plLyrics[k - 1].CharType == plLyric.CharTypes.Text)
                        {
                            elm = plLyrics[k - 1].Element;
                            if (elm.Length > 0)
                            {
                                if (elm.Substring(1, elm.Length - 1) != " ")
                                {
                                    plLyrics[k - 1].Element = elm + " ";
                                    //nbmodified++;
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("******** lyrics ticksoff modified with next lyric : " + nbmodified.ToString());
            nbmodified = 0;


            // ===============================================
            // Reduce lyric Ticksoff to the tickson of the  corresponding melody note
            // If a melody track exists, and if it is less than the actual value.
            // ===============================================
            if (_melodytracknum != -1)
            {
                Sanford.Multimedia.Midi.Track trk = sequence1.tracks[_melodytracknum];
                List<MidiNote> notes = trk.Notes;


                for (int i = 0; i < notes.Count; i++)
                {
                    for (int j = 0; j < plLyrics.Count; j++)
                    {
                        if (plLyrics[j].CharType == plLyric.CharTypes.Text)
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
            Console.WriteLine("******** lyrics ticksoff modified with associated note : " + nbmodified.ToString());
            nbmodified = 0;


            //==========================================================
            // Check beat of lyric in case of overcoming in next beat
            //==========================================================
            int beat;
            int beatend;
            int beatoff;
            int tickson;            
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int previousbeat = -1;

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
                if (beatoff > beat && (beatend - tickson < 0.8 * beatDuration) && (ticksoff - beatend > 1 * (beatend - tickson)) )
                {
                    beat += 1;
                }                                

                if (beat < previousbeat)
                {
                    Console.WriteLine("************** Error beat < previous " + plLyrics[i].CharType);
                    beat = previousbeat;
                }

                plLyrics[i].Beat = beat;
                previousbeat = beat;

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
                if (plLyrics[i].CharType == plLyric.CharTypes.Text && plLyrics[i].TicksOn > 0)
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





        #endregion private func


        #region public func

        /// <summary>
        /// Load lyrics in a dictionnary
        /// key : beat
        /// Value : lyrics in this beat
        /// </summary>
        public void LoadLyricsPerBeat()
        {
            _gridlyrics = new Dictionary<int, string>();
            int tickson;
            int ticksoff;
            int beat;
            int nbBeatsPerMeasure = sequence1.Numerator;
            int currentbeat = 1;
            string currenttext = string.Empty;
            int currentmeasure = 0;
            string cr = Environment.NewLine;

            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].CharType == plLyric.CharTypes.Text)
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
                            string tx = ex.Message + cr + "Syllab :" + currenttext + cr + "Measure: " + currentmeasure;
                            MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string tx = ex.Message + cr + "Syllab :" + currenttext + cr + "Measure: " + currentmeasure;
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load Lyrics lines in Dictionaries LyrictLines & LyricsTimes
        /// </summary>
        public void LoadLyricsLines()
        {
            LyricsLines = new Dictionary<int, string>();  // tickson of the first lyric, line of lyrics
            LyricsTimes = new Dictionary<int, int>();     // tickson of the first lyric, ticksoff of the last lyric

            string line = string.Empty;
            bool newline = false;
            int ticksoff = 0;
            int curindex = -1;

            int currenttime = 0;
            int newtime = 0;
            string cr = Environment.NewLine;

            for (int i = 0; i < plLyrics.Count; i++)
            {
                // if new line
                if (plLyrics[i].CharType == plLyric.CharTypes.LineFeed || plLyrics[i].CharType == plLyric.CharTypes.ParagraphSep)
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
                                string tx = ex.Message + cr + "Line: " + line;
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


        #endregion public func


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
                    res += var.Value + Environment.NewLine;
                }
            }
            return res;
           
        }


           


        /// <summary>
        /// TAB 3: Display words & lyrics
        /// </summary>
        /// <returns></returns>
        public string DisplayWordsAndChords()
        {
            // New version with all beats
            string res = string.Empty;
            string cr = Environment.NewLine;//"\r\n";
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beat;
            string chord = string.Empty;
            string beatchord = string.Empty;
            string beatlyr = string.Empty;
            string linebeatlyr = string.Empty;
            string linebeatchord = string.Empty;
            plLyric pll;
            string lyr = string.Empty;
            int interval = 0;
            int ticksoff;
            int tickson;
            int beatDuration = _measurelen / nbBeatsPerMeasure;            
            int beats = (int)Math.Ceiling(_totalTicks/(float)beatDuration);
            int _measure;

            // Create a dictionary key = beat, value = list of lyrics in this beat
            Dictionary<int, List<plLyric>> diclyr = new Dictionary<int, List<plLyric>>();            
            for (int i = 1; i <= beats; i++)
            {
                diclyr[i] = new List<plLyric>();                
            }

            // Load lyrics in each beat
            int _prevmeasure;
            for (int i = 0; i < plLyrics.Count; i++)
            {
                beat = plLyrics[i].Beat;

                // Set all linefeeds to start of measure?
                // What if a line ends in the measure and the next line starts in the same measure ?
                if (plLyrics[i].CharType == plLyric.CharTypes.LineFeed || plLyrics[i].CharType == plLyric.CharTypes.ParagraphSep)
                {
                    _measure = 1 + (beat - 1) / nbBeatsPerMeasure;

                    // if prev line is not on the same measure                    
                    if ( i > 0 && plLyrics[i - 1].CharType == plLyric.CharTypes.Text)
                    {
                        // Measure of previous text lyrics
                        _prevmeasure = 1 + (plLyrics[i - 1].Beat - 1) / nbBeatsPerMeasure;
                        if (_prevmeasure < _measure)
                        {
                            // if measure of previous text lyric is before measure
                            // Last beat of prev measure
                            beat = (_measure - 1) * nbBeatsPerMeasure;
                            plLyrics[i].Beat = beat;
                            plLyrics[i].TicksOn = beat * beatDuration;
                        }
                    } 
                    else if (i == 0)
                    {
                        // First pll is a cr => move it at the beginning of previous measure
                        _prevmeasure = _measure -1;
                        if (_prevmeasure > 0)
                        {
                            // Last beat of prevmeasure
                            beat = (_measure - 1) * nbBeatsPerMeasure;
                            plLyrics[i].Beat = beat;
                            plLyrics[i].TicksOn = beat * beatDuration;
                        }                        
                    }
                }


                if (beat < beats)
                {                    
                    diclyr[beat].Add(plLyrics[i]); 
                }
            }            

                // INSTRUMENTAL BEFORE THE FIRST LINE
                // Add a linefeed to the first line
            if (plLyrics.Count > 0)
            {
                // Fist lyric is a Text
                if (plLyrics[0].CharType == plLyric.CharTypes.Text)
                {
                    // Add a linefeed at the end of previous measure                    
                    pll = new plLyric()
                    {
                        CharType = plLyric.CharTypes.LineFeed
                    };
                    beat = plLyrics[0].Beat;                    
                    _measure = 1 + (beat - 1) / nbBeatsPerMeasure;
                    
                    int lastbeat = (_measure - 1) * nbBeatsPerMeasure;
                    if (lastbeat == 0)
                        lastbeat = 1;                    
                    
                    pll.Beat = lastbeat;

                    pll.TicksOn = pll.Beat * beatDuration;
                    diclyr[pll.Beat].Insert(0, pll);
                    
                }
                else
                {
                    // First lyric is not a text
                    // should be ok, but sometimes not
                    
                    // Kill first cr
                    beat = plLyrics[0].Beat;
                    
                    if (diclyr[beat].Count > 1)
                        diclyr[beat].RemoveAt(0);
                    else
                        diclyr[beat] = new List<plLyric>(); 

                    int i = 0;
                    while (plLyrics[i].CharType != plLyric.CharTypes.Text)
                    {
                        i++;
                    }
                    if (plLyrics[i].CharType == plLyric.CharTypes.Text)
                    {
                        beat = plLyrics[i].Beat;
                        _measure = 1 + (beat - 1) / nbBeatsPerMeasure;
                        int lastbeat = (_measure - 1) * nbBeatsPerMeasure;
                        if (lastbeat == 0)
                            lastbeat = 1;

                        pll = new plLyric();
                        pll.CharType = plLyric.CharTypes.LineFeed;
                        pll.Beat = lastbeat;
                        pll.TicksOn = pll.Beat * beatDuration;
                        diclyr[pll.Beat].Insert(0, pll);
                    }
                }
            }

            //==========================================================
            // Check next linefeed: if next linefeed is too far, it means that there is a instrumental before next lyric
            // So add an additional linefeed
            // TODO : if next lyric is too far => there is also an instrumental
            //==========================================================            
            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].CharType == plLyric.CharTypes.Text)
                {                                        
                    if (i < plLyrics.Count - 1)
                    {
                        // INSTRUMENTAL BEFORE A LINE
                        // If the next LINE lyric is very far, add a linefeed at the begining of the next LINE lyric                        
                        // use case : 'lyric','cr',chord chord,chord,'lyric' => 'lyric','cr',chord chord,chord,***<new cr>***,lyric
                        // Contrary : 'lyric',chord chord,chord,'lyric' must stay as is

                        if ( (plLyrics[i + 1].CharType == plLyric.CharTypes.LineFeed || plLyrics[i + 1].CharType == plLyric.CharTypes.ParagraphSep))
                        {

                            if (i < plLyrics.Count - 2 && plLyrics[i + 2].CharType == plLyric.CharTypes.Text)
                            {
                                ticksoff = plLyrics[i].TicksOff;
                                tickson = plLyrics[i + 2].TicksOn;  // begining of next line
                                interval = tickson - ticksoff;
                                if (interval > 2 * _measurelen)
                                {
                                    beat = 1 + tickson / beatDuration;
                                    _measure = 1 + (beat - 1) / nbBeatsPerMeasure;

                                    int lastbeat = (_measure - 1) * nbBeatsPerMeasure;

                                    Console.WriteLine(string.Format("**** Instrumental before line : measure: {0} Beat: {1} **************", _measure, beat));

                                    // Add a linefeed at the beginning of the the next lyric
                                    pll = new plLyric();
                                    pll.CharType = plLyric.CharTypes.LineFeed;
                                    //pll.Beat = beat;
                                    pll.Beat = lastbeat;
                                    pll.TicksOn = beat * beatDuration;

                                    diclyr[pll.Beat].Insert(0, pll);
                                }
                            }
                            else if (i < plLyrics.Count - 3 && plLyrics[i + 3].CharType == plLyric.CharTypes.Text)
                            {
                                ticksoff = plLyrics[i].TicksOff;
                                tickson = plLyrics[i + 3].TicksOn;  // begining of next line
                                interval = tickson - ticksoff;
                                if (interval > 2 * _measurelen)
                                {
                                    beat = 1 + tickson / beatDuration;
                                    _measure = 1 + (beat - 1) / nbBeatsPerMeasure;

                                    int lastbeat = (_measure - 1) * nbBeatsPerMeasure;

                                    Console.WriteLine(string.Format("**** Instrumental before line : measure: {0} Beat: {1} **************", _measure, beat));

                                    // Add a linefeed at the beginning of the the next lyric
                                    pll = new plLyric();
                                    pll.CharType = plLyric.CharTypes.LineFeed;
                                    //pll.Beat = beat;
                                    pll.Beat = lastbeat;
                                    pll.TicksOn = beat * beatDuration;

                                    diclyr[pll.Beat].Insert(0, pll);
                                }
                            }

                        }
                    }
                    


                    if (i < plLyrics.Count - 1)
                    {
                        // INSTRUMENTAL AFTER A LINE
                        // CR AT END OF LINE TOO FAR, INSTRUMENTAL TRAILING
                        // If the next plLyric is a linefeed and very far, meaning there is an instrumental before next line
                        // interval checked is 2 measures
                        // If greater than 2 measure => add a linefeed the the end of the current lyric, in order to have the instrumental in a separate line
                        // use case : lyric,chord,chord,chord,cr,lyric => lyric,***<new cr>***,chord,chord,chord,cr,lyric 
                        if (plLyrics[i + 1].CharType == plLyric.CharTypes.LineFeed || plLyrics[i + 1].CharType == plLyric.CharTypes.ParagraphSep)
                        {
                            ticksoff = plLyrics[i].TicksOff;
                            tickson = plLyrics[i + 1].TicksOn;
                            interval = tickson - ticksoff;
                            if (interval > 2 * _measurelen)
                            {
                                beat = 1 + ticksoff / beatDuration;
                                _measure = 1 + (beat - 1) / nbBeatsPerMeasure;
                                Console.WriteLine(string.Format("**** Instrumental after line : measure: {0} Beat: {1} **************", _measure, beat));

                                // TODO : add a linefeed to 1st time of this measure (this beat ?)
                                // Do not forget the end of the song : no linefeed
                                pll = new plLyric();
                                pll.CharType = plLyric.CharTypes.ParagraphSep;
                                pll.Beat = beat;
                                pll.TicksOn = beat * beatDuration;

                                diclyr[beat].Add(pll);
                            }
                        }
                    }                   
                }
            }

            // Add a cr to the Last lyric (in case of instrumental after the last lyric)
            if (plLyrics.Count > 0 && plLyrics[plLyrics.Count - 1].CharType == plLyric.CharTypes.Text)
            {
                pll = new plLyric();
                pll.CharType = plLyric.CharTypes.ParagraphSep;
                pll.Beat = plLyrics[plLyrics.Count - 1].Beat;
                if (pll.Beat < beats)
                {
                    pll.TicksOn = pll.Beat * beatDuration;
                    diclyr[pll.Beat].Add(pll);
                }
            }


            // =================================================
            // Extract chords & lyrics and format in text mode
            // =================================================
            for (int measure = 1; measure <= NbMeasures; measure++)
            {                                
                for (int timeinmeasure = 1; timeinmeasure <= nbBeatsPerMeasure; timeinmeasure++)
                {
                    beat = (measure - 1) * nbBeatsPerMeasure + timeinmeasure;
                    if (beat <= beats)
                    {
                        var kvChord = Gridchords[measure];

                        // ===========================
                        // 1 - Search lyrics
                        // ===========================
                        if (diclyr[beat].Count > 0)
                        {
                            string lastchord = string.Empty;

                            // foreach lyric in this beat
                            for (int i = 0; i < diclyr[beat].Count; i++)
                            {
                                pll = diclyr[beat][i];


                                // LINEFEED => STORE RESULT
                                #region Store result
                                if (pll.CharType == plLyric.CharTypes.LineFeed || pll.CharType == plLyric.CharTypes.ParagraphSep)
                                {
                                    // Store line
                                    linebeatlyr += beatlyr;
                                    linebeatchord += beatchord;
                                    if (linebeatlyr != "" || linebeatchord != "")
                                    {
                                        if (pll.CharType == plLyric.CharTypes.LineFeed)
                                        {

                                            // If Linefeed, one cr
                                            if (linebeatchord != "" && linebeatlyr != "")
                                                res += linebeatchord + cr + linebeatlyr + cr;
                                            else if (linebeatchord != "")
                                                res += linebeatchord + cr;
                                            else if (linebeatlyr != "")
                                                res += linebeatlyr + cr;


                                        }
                                        else if (pll.CharType == plLyric.CharTypes.ParagraphSep)
                                        {

                                            // If paragraph, 2 cr
                                            if (linebeatchord != "" && linebeatlyr != "")
                                                res += linebeatchord + cr + linebeatlyr + cr + cr;
                                            else if (linebeatchord != "")
                                                res += linebeatchord + cr + cr;
                                            else if (linebeatlyr != "")
                                                res += linebeatlyr + cr + cr;


                                        }
                                    }
                                    // Reset all
                                    linebeatchord = string.Empty;
                                    linebeatlyr = string.Empty;
                                    beatlyr = string.Empty;
                                    beatchord = string.Empty;
                                    lastchord = string.Empty;
                                }
                                #endregion store result
                                else if (pll.CharType == plLyric.CharTypes.Text)
                                {
                                    lyr = pll.Element;
                                    //beatlyr += lyr;


                                    // ===========================
                                    // 2 - Search chords
                                    // ===========================                                        
                                    if (timeinmeasure == 1)
                                    {
                                        // Chord 1
                                        chord = kvChord.Item1;

                                        // Do not repeat chord on all lyrics of this beat
                                        if (lastchord == chord)
                                            chord = "";
                                        else
                                            lastchord = chord;

                                        if (chord == "<Empty>")
                                            chord = "";
                                    }
                                    else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                                    {
                                        // Chord 2
                                        if (kvChord.Item2 != kvChord.Item1)
                                        {
                                            chord = kvChord.Item2;

                                            // Do not repeat chord on all lyrics of this beat
                                            if (lastchord == chord)
                                                chord = "";
                                            else
                                                lastchord = chord;

                                            if (chord == "<Empty>")
                                                chord = "";
                                        }
                                    }

                                    // ===========================
                                    // 3 - Manage lyrics &
                                    //  Add spaces to harminize lenght of items
                                    // ===========================
                                    // lyric AND chord
                                    // F1 if (beatlyr.Trim() != "" && chord.Trim() != "")
                                    if (lyr.Trim() != "" && chord.Trim() != "")
                                    {
                                        // F1 if (beatlyr.Length > chord.Length)
                                        if (lyr.Length > chord.Length)
                                        {
                                            // Case of lyrics starting with a space (instead of trailing space)
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                beatchord += " " + chord;
                                                beatchord += new string(' ', lyr.Length - 1 - chord.Length);
                                                beatlyr += lyr;
                                            }
                                            else
                                            {
                                                // F1 beatchord += new string(' ', beatlyr.Length - chord.Length);
                                                beatchord += chord;
                                                beatchord += new string(' ', lyr.Length - chord.Length);
                                                beatlyr += lyr;
                                            }
                                        }
                                        // F1 else if (beatlyr.Length < chord.Length)
                                        else if (lyr.Length < chord.Length)
                                        {
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                Console.WriteLine("lyric left space");
                                                beatchord += " " + chord;
                                                beatlyr += lyr;
                                                beatlyr += new string(' ', chord.Length + 1 - lyr.Length);
                                            }
                                            else
                                            {
                                                beatchord += chord;
                                                beatlyr += lyr;
                                                beatlyr += new string(' ', chord.Length - lyr.Length);
                                            }
                                        }
                                        else
                                        {
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                Console.WriteLine("lyric left space");
                                                beatchord += " " + chord;
                                                beatlyr += lyr + " ";
                                            }
                                            else
                                            {
                                                beatchord += chord;
                                                beatlyr += lyr;
                                            }

                                        }
                                    }
                                    // lyric, no chord
                                    //F1 else if (beatlyr.Trim() != "" && chord.Trim() == "")
                                    else if (lyr.Trim() != "" && chord.Trim() == "")
                                    {
                                        beatchord += new string(' ', lyr.Length);
                                        beatlyr += lyr;
                                    }
                                    // no lyric, chord
                                    //F1 else if (beatlyr.Trim() == "" && chord.Trim() != "")
                                    else if (lyr.Trim() == "" && chord.Trim() != "")
                                    {
                                        beatchord += chord + " ";
                                        beatlyr += new string(' ', chord.Length + 1);
                                    }

                                    // Reset all
                                    linebeatlyr += beatlyr;
                                    linebeatchord += beatchord;
                                    beatlyr = string.Empty;
                                    beatchord = string.Empty;
                                    lyr = string.Empty;
                                    chord = string.Empty;
                                }
                            } // foreach beatlyr
                        }
                        else
                        {
                            // diclyr[beat] is null
                            // Chords ?

                            // ===========================
                            // 2 - Search chords
                            // ===========================                                        
                            if (timeinmeasure == 1)
                            {
                                // Chord 1
                                chord = kvChord.Item1;
                                if (chord == "<Empty>")
                                    chord = "";
                            }
                            else if (timeinmeasure == 1 + nbBeatsPerMeasure / 2)
                            {
                                // Chord 2
                                if (kvChord.Item2 != kvChord.Item1)
                                {
                                    chord = kvChord.Item2;
                                    if (chord == "<Empty>")
                                        chord = "";
                                }
                            }


                            // ===========================
                            // 3 - Manage lyrics &
                            //  Add spaces to rech lenght of items
                            // ===========================
                            // lyric
                            if (beatlyr.Trim() != "" && chord.Trim() != "")
                            {
                                if (beatlyr.Length > chord.Length)
                                {
                                    beatchord += chord;
                                    beatchord += new string(' ', beatlyr.Length - chord.Length);
                                }
                                else if (beatlyr.Length < chord.Length)
                                {
                                    beatchord += chord;
                                    beatlyr += new string(' ', chord.Length - lyr.Length);
                                }
                                else
                                {
                                    beatchord += chord;

                                }
                            }
                            // lyric, no chord
                            else if (beatlyr.Trim() != "" && chord.Trim() == "")
                            {
                                beatchord += new string(' ', lyr.Length);
                            }
                            // no lyric, chord
                            else if (beatlyr.Trim() == "" && chord.Trim() != "")
                            {
                                beatchord += chord + " ";
                                beatlyr += new string(' ', chord.Length + 1);
                            }

                            // Reset all
                            linebeatlyr += beatlyr;
                            linebeatchord += beatchord;
                            beatlyr = string.Empty;
                            beatchord = string.Empty;
                            lyr = string.Empty;
                            chord = string.Empty;
                        }
                    }
                }
            }

            // Store last line
            linebeatlyr += beatlyr;
            //linebeatchord += beatchord;
            if (linebeatlyr != "" || linebeatchord != "")
            {
                // New Line => store result                
                res += linebeatchord + cr + linebeatlyr;
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
