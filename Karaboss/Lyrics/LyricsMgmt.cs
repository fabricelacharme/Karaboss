#region License

/* Copyright (c) 2024 Fabrice Lacharme
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

using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Lyrics
{
    public partial class LyricsMgmt
    {

        #region private

        private Dictionary<int, string> LyricsLines = new Dictionary<int, string>();
        private Dictionary<int, int> LyricsTimes = new Dictionary<int, int>();
        private Array LyricsLinesKeys;
        private Array LyricsTimesKeys;

        private Sequence sequence1;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;        
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int _nbMeasures;
        private int _nbBeats;
        
        // Are lyrics stored with a space or not ?
        private lyricsSpacings _lyricsspacing = lyricsSpacings.WithoutSpace;

        private bool _bHasCarriageReturn = false;

        private string patternBracket = @"\[\b([CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*(?:[CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*)*)\]";
        private string patternParenth = @"\(\b([CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*(?:[CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*)*)\)";

        private string _InternalSepLines = "¼";
        private string _InternalSepParagraphs = "½";

        private string ChordNotFound = "<Chord not found>";        
        private string EmptyChord = "<Empty>";

        #endregion private


        #region public
        // Default List of lyrics
        public List<plLyric> plLyrics { get; set; }


        public int Numerator { get; set; }
        public int Division { get; set; }
        
        // FAB 21/11/2024 : list of the 2 different types of lyric:
        // 0 = lyric
        // 1 = text        
        public List<List<plLyric>> lstpllyrics { get; set; }

               
        // Is there chords included in lyrics
        public bool bHasChordsInLyrics { get; set; }

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

        // character that surrounds each chord
        // '(,)' pour (C), '[, ]' pour [C]
        private (string, string) _chordDelimiter;
        public (string, string) ChordDelimiter
        {
            get { return _chordDelimiter; }
            set { _chordDelimiter = value; }
        }

        // Pattern to remove chords from lyrics
        private string _removechordpattern;
        public string RemoveChordPattern
        {
            get { return _removechordpattern; }
            set { _removechordpattern = value; }
        }

        // Lyrics : int = time, string = syllabes in corresponding time
        private Dictionary<int, string> _gridlyrics;
        public Dictionary<int,string> Gridlyrics
        {
            get { return _gridlyrics; }
        }       

        // Search by beat
        // int beat
        // string chord
        private Dictionary<int, string> _gridbeatchords;
        public Dictionary<int, string> GridBeatChords 
        {
            get { return _gridbeatchords; }
            set { _gridbeatchords = value; } 
        }

        #endregion public


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sequence"></param>
        public LyricsMgmt(Sequence sequence) 
        {            
            
            _lyricstracknum = -1;
            _melodytracknum = -1;
            
            _lyrictype = LyricTypes.None;

            plLyrics = new List<plLyric>();
            lstpllyrics = new List<List<plLyric>>();

            sequence1 = sequence;
            
            UpdateMidiTimes();

            // Extract lyrics
            _lyrics = ExtractLyrics();


            if (plLyrics.Count > 0)
            {
                #region analyse & rearrange lyrics
                // Guess spacing or not and carriage return or not
                GetLyricsSpacingModel();

                // Add a trailing space to each syllabe
                if (_lyricsspacing == lyricsSpacings.WithoutSpace)
                {
                    SetTrailingSpace();
                }

                // If zero carriage return in the lyrics
                if (!_bHasCarriageReturn)
                {
                    AddCarriageReturn();
                }

                // Remove empty lyrics
                plLyrics = cleanLyrics();

                #endregion analyse & rearrange lyrics

                // Search for the melody track
                _melodytracknum = GuessMelodyTrack();

                // Fix lyrics endtime to notes of the melody track end time
                CheckTimes();

                // Extract chords in lyrics
                bHasChordsInLyrics = HasChordsInLyrics(_lyrics);                

                if (bHasChordsInLyrics)
                {
                    // Add chords found in lyrics in the list pllyrics
                    ExtractChordsInLyrics(_lyricstracknum);
                }
            }                                 
        }
       

        #region arrange lyrics
        /// <summary>
        /// Guess if lyrics have carriage return and spaces between syllabes
        /// </summary>
        private void GetLyricsSpacingModel() 
        {
            
            _bHasCarriageReturn = false;
            _lyricsspacing = lyricsSpacings.WithoutSpace;

            bool btest1 = false;
            bool btest2 = false;

            // What is the type of separator between lyrics ? space or nothing
            // If there is a space before or after the string, the lyrics are separated by a space
            for (int k = 0; k < plLyrics.Count; k++)
            {
                string s = plLyrics[k].Element.Item2;
                
                if (plLyrics[k].CharType == plLyric.CharTypes.Text)
                {
                    if (s.StartsWith(" ") || s.EndsWith(" ")) 
                    {
                        _lyricsspacing = lyricsSpacings.WithSpace;
                        btest1 = true;
                        
                    }                    
                }
                else if (plLyrics[k].CharType == plLyric.CharTypes.LineFeed || plLyrics[k].CharType == plLyric.CharTypes.ParagraphSep)
                {
                    _bHasCarriageReturn = true;
                    btest2 = true;
                }

                // Stop loop if spaces between syllabes AND carriage return is found 
                if(btest2 && btest1)
                    return;
            }
            

        }

        /// <summary>
        /// Add a trailing space when the lyrics have no space.
        /// Remove the '-' character
        /// </summary>
        private void SetTrailingSpace()
        {
            #region check
            // Concerns only lyrics without space
            if (_lyricsspacing != lyricsSpacings.WithoutSpace)
                return;
            #endregion

            for (int k = 0; k < plLyrics.Count; k++)
            {
                string s = plLyrics[k].Element.Item2;

                if (plLyrics[k].CharType == plLyric.CharTypes.Text)
                {
                    if (s != string.Empty)
                    {                        
                        if (!(s.StartsWith(" ") || s.EndsWith(" ")) && (!s.EndsWith("-")))
                        {
                            s += " ";
                        }
                        else if (s.EndsWith("-") && s.Length > 1)
                        {
                            s = s.Substring(0, s.Length - 1);
                        }
                        plLyrics[k].Element = (plLyrics[k].Element.Item1, s);
                        
                    }
                }
            }
        }


        /// <summary>
        /// Add a carriage return if Uppercase, ponctuation, too long
        /// </summary>
        private void AddCarriageReturn()
        {
            #region check
            if (_bHasCarriageReturn)
                return;
            #endregion

            int lyricLengh = 0;
            int linelength = 30;
            int minimumlinelength = 10;
            string element;

            List<plLyric> _tmpL = new List<plLyric>();
            // test 1 : Add a CR to each uppercase
            for (int k = 0; k < plLyrics.Count; k++)
            {
                if (plLyrics[k].CharType == plLyric.CharTypes.Text) 
                {
                    element = plLyrics[k].Element.Item2;                    

                    // Check if uppercase
                    if (char.IsUpper(element, 0))
                    {
                        if (lyricLengh > minimumlinelength)
                        {
                            // Add linefeed first
                            _tmpL.Add(new plLyric() { CharType = plLyric.CharTypes.LineFeed, Element = ("", _InternalSepLines), TicksOn = plLyrics[k].TicksOn, TicksOff = plLyrics[k].TicksOff });
                            lyricLengh = 0;
                            // Add lyric after
                            _tmpL.Add(plLyrics[k]);
                        }
                        else
                        {
                            lyricLengh += element.Length;
                            _tmpL.Add(plLyrics[k]);
                        }
                    }
                    else if (char.IsPunctuation(element.Trim(), element.Trim().Length - 1))
                    {
                        // Add lyric first
                        _tmpL.Add(plLyrics[k]);

                        // Add linefeed after
                        _tmpL.Add(new plLyric() { CharType = plLyric.CharTypes.LineFeed, Element = ("", _InternalSepLines), TicksOn = plLyrics[k].TicksOn, TicksOff = plLyrics[k].TicksOff });
                        lyricLengh = 0;
                    }
                    else 
                    {
                        // No upper case, but too long
                        // Cut if blank
                        if (lyricLengh > linelength && element.IndexOf(" ") > -1)
                        {
                            // Add lyric first
                            _tmpL.Add(plLyrics[k]);

                            // Add linefeed after
                            _tmpL.Add(new plLyric() { CharType = plLyric.CharTypes.LineFeed, Element = ("", _InternalSepLines), TicksOn = plLyrics[k].TicksOn, TicksOff = plLyrics[k].TicksOff });
                            lyricLengh = 0;
                            
                        }
                        else
                        {
                            lyricLengh += element.Length;
                            _tmpL.Add(plLyrics[k]);
                        }
                    }
                }                
            }

            plLyrics = _tmpL;
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
                            elm = plLyrics[k - 1].Element.Item2;
                            if (elm.Length > 0)
                            {
                                // FAB 07/12
                                //if (elm.Substring(1, elm.Length - 1) != " ")
                                if (elm.Substring(elm.Length - 1, 1) != " ")
                                {
                                    plLyrics[k - 1].Element = (plLyrics[k - 1].Element.Item1, elm + " ");
                                    //nbmodified++;
                                }
                            }
                        }
                    }
                }
            }
            //Console.WriteLine("******** lyrics ticksoff modified with next lyric : " + nbmodified.ToString());
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
            //Console.WriteLine("******** lyrics ticksoff modified with associated note : " + nbmodified.ToString());
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
                if (beatoff > beat && (beatend - tickson < 0.8 * beatDuration) && (ticksoff - beatend > 1 * (beatend - tickson)))
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

        #endregion arrange lyrics


        #region extract lyrics
        /// <summary>
        /// Lyrics extraction & display
        /// </summary>
        private string ExtractLyrics()
        {
            double l_text = 1;
            double l_lyric = 1;
            string lyrics = string.Empty;

            plLyrics = new List<plLyric>();

            // ----------------------------------------------------------------------
            // Objectif : comparer texte et lyriques et choisir la meilleure solution
            // ----------------------------------------------------------------------

            // track for text
            int trktext = HasLyricsText();     // Recherche si Textes
            if (trktext >= 0)            
                l_text = sequence1.tracks[trktext].TotalLyricsT.Length;
                            

            // track for lyrics
            int trklyric = HasLyrics();              // Recherche si lyrics  
            if (trklyric >= 0)            
                l_lyric = sequence1.tracks[trklyric].TotalLyricsL.Length;                
            


            // if both types of lyrics are found: text and lyric
            // Keep only the biggest one
            _lyrictype = LyricTypes.None;
            
            // Initialyze the list of plLyrics to 3 items
            lstpllyrics.Add(new List<plLyric>());       // lyric
            lstpllyrics.Add(new List<plLyric>());       // text            


            if (trklyric >= 0)
                lstpllyrics[0] = LoadLyricsLyric(sequence1.tracks[trklyric]);
            if (trktext >= 0)
                lstpllyrics[1] = LoadLyricsText(sequence1.tracks[trktext]);     


            // If both types: lyric & text
            // Take the bigger one
            if (trktext >= 0 && trklyric >= 0)
            {
                // regarde lequel est le plus gros... lol                
                if (l_lyric >= l_text)
                {
                    // Elimine texte et choisi les lyrics                    
                    _lyrictype = LyricTypes.Lyric;
                    _lyricstracknum = trklyric;
                    lyrics = sequence1.tracks[trklyric].TotalLyricsL;
                }
                else
                {
                    // Elimine lyrics et choisi les textes                    
                    _lyrictype = LyricTypes.Text;
                    _lyricstracknum = trktext;
                    lyrics = sequence1.tracks[trktext].TotalLyricsT;
                }
            }
            else if (trktext >= 0)
            {
                _lyrictype = LyricTypes.Text;
                _lyricstracknum = trktext;
                lyrics = sequence1.tracks[trktext].TotalLyricsT;
            }
            else if (trklyric >= 0)
            {
                _lyrictype = LyricTypes.Lyric;
                _lyricstracknum = trklyric;
                lyrics = sequence1.tracks[trklyric].TotalLyricsL;
            }
            else
            {
                _lyrictype = LyricTypes.None;
                lyrics = string.Empty;
            }

            if (_lyrictype == LyricTypes.Lyric)
                plLyrics = lstpllyrics[0];
            else if (_lyrictype == LyricTypes.Text)
                plLyrics = lstpllyrics[1];
                   

            return lyrics;
        }

        private List<plLyric> LoadLyricsText(Sanford.Multimedia.Midi.Track track)
        {                                    
            List<plLyric> pll = new List<plLyric>();            

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

                pll.Add(new plLyric() { CharType = plType, Element = ("", plElement), TicksOn = plTicksOn, TicksOff = plTicksOff });
            }
            
            return pll;
        }

        private List<plLyric> LoadLyricsLyric(Sanford.Multimedia.Midi.Track track)
        {
            List<plLyric> pll = new List<plLyric>();            

            // Remove "[]" for the letter by letter lyrics            
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

                    pll.Add(new plLyric() { CharType = plType, Element = ("", plElement), TicksOn = plTicksOn, TicksOff = plTicksOff });
                }
            }            

            return pll;
        }

        #endregion extract lyrics


        #region analyse lyrics

        /// <summary>
        /// Return track number when lyrics of type = Text are found, -1 otherwise
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
        /// Return track number when lyrics of type = Lyric are found, -1 otherwise
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
        /// Return track number when chords are found in the lyrics, -1 otherwise
        /// </summary>
        /// <returns></returns>
        private bool HasChordsInLyrics (string s)
        {
            // With brakets
            Regex chordCheck = new Regex(patternBracket);
            
            // With parenthesis
            Regex chordCheck2 = new Regex(patternParenth);
                        
            MatchCollection mc = chordCheck.Matches(s);
            MatchCollection mc2 = chordCheck2.Matches(s);

            // Exlude copyright (C)
            if (mc.Count > 1 | mc2.Count > 1) 
                return true;
            else
                return false;                      
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
            int nbfound = 0;
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
                } // contains notes                
            }
            return trackfnote;
        }

        /// <summary>
        /// Remove empty lyrics
        /// </summary>
        /// <returns></returns>
        private List<plLyric> cleanLyrics()
        {
            string lyric = string.Empty;
            List<plLyric> lst = new List<plLyric>();
            //int nbRemoved = 0;
            for (int i = 0; i < plLyrics.Count; i++) 
            {
                lyric = plLyrics[i].Element.Item2;
                if (lyric.Trim().Length > 0)
                {
                    lst.Add(plLyrics[i]);
                }
                //else
                //{
                //    nbRemoved++;
                //}
            }

            //Console.WriteLine("*********************** Empty lyrics removed = " + nbRemoved);
            return lst;

        }

        #endregion analyse lyrics


        #region extract chords
        /// <summary>
        /// Extract chords [A], [B] ... written in the lyrics
        /// and add them to plLyric
        /// </summary>
        /// <param name="tracknum"></param>
        private void ExtractChordsInLyrics(int tracknum)
        {
            string lyricElement;
            string chordElement = string.Empty;           
            bool bFound;

            for (int i = 0; i < plLyrics.Count; i++) 
            { 
                plLyric pl = plLyrics[i];  
                if (pl.CharType == plLyric.CharTypes.Text)
                {
                    lyricElement = pl.Element.Item2;
                    

                    // With brakets
                    Regex chordCheck = new Regex(patternBracket);
                    
                    // With parenthesis                    
                    Regex chordCheck2 = new Regex(patternParenth);


                    MatchCollection mc = chordCheck.Matches(lyricElement);
                    MatchCollection mc2 = chordCheck2.Matches(lyricElement);

                    bFound = false;

                    if (mc.Count > 0) 
                    {
                        //for (int j = 0; j < mc.Count; j++) 
                        //{ 

                        //_chordDelimiter = ("[", "]");
                        chordElement = mc[0].Value;
                        _removechordpattern = patternBracket;
                        bFound = true;

                        if (chordElement.Length > 2)
                        {
                            chordElement = chordElement.Substring(1, chordElement.Length - 2);
                        }

                        //}

                    } 
                    else if (mc2.Count > 0)
                    {
                        //_chordDelimiter = ("(", ")");
                        chordElement = mc2[0].Value;
                        _removechordpattern = patternParenth;

                        bFound = true;

                        if (chordElement.Length > 2)
                        {
                            chordElement = chordElement.Substring(1, chordElement.Length - 2);
                        }
                    }

                    if (bFound)
                    {                       
                        // Update list item with chord                        
                        plLyrics[i].Element = (chordElement, lyricElement);
                        
                    }

                }
            }     
        }

        #endregion extract chords


        #region deleteme

        /*

        /// <summary>
        /// Usage : Display lyrics from frmLyric
        /// Return a text with all the lyrics with chords
        /// </summary>
        /// <returns></returns>
        public string GetLyricsLinesWithChords()
        {
            string line = string.Empty;
            bool newline = false;

            int currenttime = 0;
            int newtime = 0;

            List<string> lines = new List<string>();
            string lineChords = string.Empty;
            string lineLyrics = string.Empty;

            string chordElement = string.Empty;
            string lyricElement = string.Empty;

            string res = string.Empty;
            string sep = string.Empty;
            
            string replace = @"";

            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].CharType == plLyric.CharTypes.LineFeed || plLyrics[i].CharType == plLyric.CharTypes.ParagraphSep)
                {
                    // Case several lines with the same start time
                    newtime = plLyrics[i].TicksOn;
                    if (newtime != currenttime)
                    {
                        newline = true;
                        currenttime = newtime;
                    }

                    // Add linefeed or paragraph                    
                    sep = plLyrics[i].Element.Item2;
                }
                else if (plLyrics[i].CharType == plLyric.CharTypes.Text)
                {
                    // the first item after newline
                    if (newline)
                    {
                        // Separator may be linefeed or paragraph
                        lines.Add(sep);
                        lines.Add(lineChords);
                        // Always a single linefeed between the chords and their lyrics
                        lines.Add(_InternalSepLines);
                        lines.Add(lineLyrics);

                        newline = false;
                        lineChords = string.Empty;
                        lineLyrics = string.Empty;
                    }

                    // Chord
                    chordElement = plLyrics[i].Element.Item1;
                    
                    // Lyric
                    lyricElement = plLyrics[i].Element.Item2;

                    if (bHasChordsInLyrics && _removechordpattern != null)
                    {
                        // Remove chords from lyrics 
                        lyricElement = Regex.Replace(lyricElement, _removechordpattern, replace);
                    }
                    // Adjust length of chord to length of lyric
                    if (chordElement.Length < lyricElement.Length)
                        chordElement += new string(' ', lyricElement.Length - chordElement.Length);
                    else if (chordElement.Length > lyricElement.Length)
                        lyricElement += new string(' ', chordElement.Length - lyricElement.Length);

                    lineChords += chordElement;
                    lineLyrics += lyricElement;
                }
            }

            // Do not forget last line
            lines.Add(sep);
            lines.Add(lineChords);
            lines.Add(sep);
            lines.Add(lineLyrics);


            //return lines;
            for (int i = 0; i < lines.Count; i++)
            {
                res += lines[i];
            }
            
            return res;
        }

        */

        #endregion deleteme


        #region TAB1

        /// <summary>
        /// TAB1 / horizontal cells
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

            string lyricElement;
            string replace = @"";

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

                    // Remove chords from lyrics 
                    lyricElement = plLyrics[i].Element.Item2;

                    if (bHasChordsInLyrics && _removechordpattern != null)
                    {
                        lyricElement = Regex.Replace(lyricElement, _removechordpattern, replace);
                    }
                    currenttext += lyricElement;
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
        /// TAB1 / lyrics bottom
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
            string lyricElement = string.Empty;
            string replace = @"";

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
                else if (plLyrics[i].CharType == plLyric.CharTypes.Text)
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
                    lyricElement = plLyrics[i].Element.Item2;

                    if (bHasChordsInLyrics && _removechordpattern != null)
                    {
                        // Remove chords from lyrics 
                        lyricElement = Regex.Replace(lyricElement, _removechordpattern, replace);
                    }
                    line += lyricElement;
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
        /// TAB 1: Display lyrics lines except line being played
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

        #endregion TAB1


        #region clean chords labels
        public void CleanGridBeatChords()
        {
            //Change labels displayed
            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                GridBeatChords[i] = InterpreteChord(GridBeatChords[i]);
            }
        }

        /// <summary>
        /// Remove useless strings
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private string InterpreteChord(string chord)
        {
            /*
            chord = chord.Replace("sus", "");

            chord = chord.Replace(" major", "");
            chord = chord.Replace(" triad", "");
            chord = chord.Replace("dominant", "");

            chord = chord.Replace("first inversion", "");
            chord = chord.Replace("second inversion", "");
            chord = chord.Replace("third inversion", "");

            chord = chord.Replace(" seventh", "7");
            chord = chord.Replace(" minor", "m");
            chord = chord.Replace("seventh", "7");
            chord = chord.Replace("sixth", "6");
            chord = chord.Replace("ninth", "9");
            chord = chord.Replace("eleventh", "11");

            chord = chord.Replace("6", "");
            chord = chord.Replace("9", "");
            chord = chord.Replace("11", "");
            */

            //chord = chord.Replace("<Chord not found>", "?");
            chord = chord.Replace("<Chord not found>", "");

            chord = chord.Trim();
            return chord;
        }

        #endregion clen chords labels


        #region TAB3
        /// <summary>
        /// TAB 3: Display words & lyrics
        /// </summary>
        /// <returns></returns>      
        public string DisplayWordsAndChords()
        {
            // New version with all beats
            string res = string.Empty;
            string cr = Environment.NewLine;
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beat;
            
            string beatchord = string.Empty;
            string beatlyr = string.Empty;
            string linebeatlyr = string.Empty;
            string linebeatchord = string.Empty;
            plLyric pll;

            string lyr = string.Empty;
            string lyricElement = string.Empty;
            string replace = @"";

            int interval = 0;
            int ticksoff;
            int tickson;
            int beatDuration = _measurelen / nbBeatsPerMeasure;            
            int _measure;

            string chordName = string.Empty;

            #region guard
            if (GridBeatChords == null || plLyrics == null)
            {
                MessageBox.Show("Error: GridBeatChords is null", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            #endregion guard

            // Create a dictionary key = beat, value = list of lyrics in this beat
            Dictionary<int, List<plLyric>> diclyr = new Dictionary<int, List<plLyric>>();
            for (int i = 1; i <= _nbBeats; i++)
            {
                diclyr[i] = new List<plLyric>();
            }

            // Load lyrics in each beat
            int _prevmeasure;
            for (int i = 0; i < plLyrics.Count; i++)
            {
                beat = plLyrics[i].Beat;

                if (beat == 0)
                {
                    MessageBox.Show("Error plLyrics with beat at 0", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return string.Empty;
                }
                    

                // Set all linefeeds to start of measure?
                // What if a line ends in the measure and the next line starts in the same measure ?
                if (plLyrics[i].CharType == plLyric.CharTypes.LineFeed || plLyrics[i].CharType == plLyric.CharTypes.ParagraphSep)
                {
                    _measure = 1 + (beat - 1) / nbBeatsPerMeasure;

                    // if prev line is not on the same measure                    
                    if (i > 0 && plLyrics[i - 1].CharType == plLyric.CharTypes.Text)
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
                        _prevmeasure = _measure - 1;
                        if (_prevmeasure > 0)
                        {
                            // Last beat of prevmeasure
                            beat = (_measure - 1) * nbBeatsPerMeasure;
                            plLyrics[i].Beat = beat;
                            plLyrics[i].TicksOn = beat * beatDuration;
                        }
                    }
                }


                if (beat < _nbBeats)
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

                        if ((plLyrics[i + 1].CharType == plLyric.CharTypes.LineFeed || plLyrics[i + 1].CharType == plLyric.CharTypes.ParagraphSep))
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

                                    //Console.WriteLine(string.Format("**** Instrumental before line : measure: {0} Beat: {1} **************", _measure, beat));

                                    // Add a linefeed at the beginning of the the next lyric
                                    pll = new plLyric();
                                    pll.CharType = plLyric.CharTypes.LineFeed;                                    
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

                                    //Console.WriteLine(string.Format("**** Instrumental before line : measure: {0} Beat: {1} **************", _measure, beat));

                                    // Add a linefeed at the beginning of the the next lyric
                                    pll = new plLyric();
                                    pll.CharType = plLyric.CharTypes.LineFeed;                                    
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
                                //Console.WriteLine(string.Format("**** Instrumental after line : measure: {0} Beat: {1} **************", _measure, beat));

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
                if (pll.Beat < _nbBeats)
                {
                    pll.TicksOn = pll.Beat * beatDuration;
                    diclyr[pll.Beat].Add(pll);
                }
            }


            // =================================================
            // Extract chords & lyrics and format in text mode
            // =================================================
            // Do not repeat chords
            string _currentChordName = "<>";

            for (int measure = 1; measure <= _nbMeasures; measure++)
            {
                for (int timeinmeasure = 1; timeinmeasure <= nbBeatsPerMeasure; timeinmeasure++)
                {
                    beat = (measure - 1) * nbBeatsPerMeasure + timeinmeasure;
                    if (beat <= _nbBeats && beat < GridBeatChords.Count)
                    {                        
                        chordName = GridBeatChords[beat];


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


                                #region Store result
                                // LINEFEED => STORE RESULT
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
                                    lyr = pll.Element.Item2;

                                    if (bHasChordsInLyrics && _removechordpattern != null)
                                    {
                                        // Remove chords from lyrics 
                                        lyr = Regex.Replace(lyr, _removechordpattern, replace);
                                    }

                                    // ===========================
                                    // 2 - Search chords
                                    // ===========================                                        
                                    if (chordName == EmptyChord)
                                        chordName = "";

                                    if (chordName != "" &&  chordName != _currentChordName)
                                    {
                                        _currentChordName = chordName;
                                    }
                                    else
                                    {
                                        chordName = "";
                                    }

                                    // Do not repeat chord on all lyrics of this beat
                                    if (lastchord == chordName)
                                        chordName = "";
                                    else
                                        lastchord = chordName;
                                

                                    // ===========================
                                    // 3 - Manage lyrics &
                                    //  Add spaces to harmonize lenght of items
                                    // ===========================
                                    // lyric AND chord                                    
                                    if (lyr.Trim() != "" && chordName.Trim() != "")
                                    {
                                        // F1 if (beatlyr.Length > chord.Length)
                                        if (lyr.Length > chordName.Length)
                                        {
                                            // Case of lyrics starting with a space (instead of trailing space)
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                beatchord += " " + chordName;
                                                beatchord += new string(' ', lyr.Length - 1 - chordName.Length);
                                                beatlyr += lyr;
                                            }
                                            else
                                            {
                                                // F1 beatchord += new string(' ', beatlyr.Length - chord.Length);
                                                beatchord += chordName;
                                                beatchord += new string(' ', lyr.Length - chordName.Length);
                                                beatlyr += lyr;
                                            }
                                        }
                                        // F1 else if (beatlyr.Length < chord.Length)
                                        else if (lyr.Length < chordName.Length)
                                        {
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                //Console.WriteLine("lyric left space");
                                                beatchord += " " + chordName;
                                                beatlyr += lyr;
                                                beatlyr += new string(' ', chordName.Length + 1 - lyr.Length);
                                            }
                                            else
                                            {
                                                beatchord += chordName;
                                                beatlyr += lyr;
                                                beatlyr += new string(' ', chordName.Length - lyr.Length);
                                            }
                                        }
                                        else
                                        {
                                            if (lyr.Substring(0, 1) == " ")
                                            {
                                                //Console.WriteLine("lyric left space");
                                                beatchord += " " + chordName;
                                                beatlyr += lyr + " ";
                                            }
                                            else
                                            {
                                                beatchord += chordName;
                                                beatlyr += lyr;
                                            }
                                        }
                                    }
                                    // lyric, no chord                                    
                                    else if (lyr.Trim() != "" && chordName.Trim() == "")
                                    {
                                        beatchord += new string(' ', lyr.Length);
                                        beatlyr += lyr;
                                    }
                                    // no lyric, chord                                    
                                    else if (lyr.Trim() == "" && chordName.Trim() != "")
                                    {
                                        beatchord += chordName + " ";
                                        beatlyr += new string(' ', chordName.Length + 1);
                                    }

                                    // Reset all
                                    linebeatlyr += beatlyr;
                                    linebeatchord += beatchord;
                                    beatlyr = string.Empty;
                                    beatchord = string.Empty;
                                    lyr = string.Empty;
                                    chordName = string.Empty;
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
                            if (chordName == EmptyChord)
                                chordName = "";

                            if (chordName != "" && chordName != _currentChordName)
                            {
                                _currentChordName = chordName;
                            }
                            else
                            {
                                chordName = "";
                            }
                        

                            // ===========================
                            // 3 - Manage lyrics &
                            //  Add spaces to rech lenght of items
                            // ===========================
                            // lyric
                            if (beatlyr.Trim() != "" && chordName.Trim() != "")
                            {
                                if (beatlyr.Length > chordName.Length)
                                {
                                    beatchord += chordName;
                                    beatchord += new string(' ', beatlyr.Length - chordName.Length);
                                }
                                else if (beatlyr.Length < chordName.Length)
                                {
                                    beatchord += chordName;
                                    beatlyr += new string(' ', chordName.Length - lyr.Length);
                                }
                                else
                                {
                                    beatchord += chordName;

                                }
                            }
                            // lyric, no chord
                            else if (beatlyr.Trim() != "" && chordName.Trim() == "")
                            {
                                beatchord += new string(' ', lyr.Length);
                            }
                            // no lyric, chord
                            else if (beatlyr.Trim() == "" && chordName.Trim() != "")
                            {
                                beatchord += chordName + " ";
                                beatlyr += new string(' ', chordName.Length + 1);
                            }

                            // Reset all
                            linebeatlyr += beatlyr;
                            linebeatchord += beatchord;
                            beatlyr = string.Empty;
                            beatchord = string.Empty;
                            lyr = string.Empty;
                            chordName = string.Empty;
                        }
                    }
                }
            }

            // Store last line
            linebeatlyr += beatlyr;
            
            if (linebeatlyr != "" || linebeatchord != "")
            {
                // New Line => store result                
                res += linebeatchord + cr + linebeatlyr;
            }

            return res;
        }

        #endregion TAB3


        #region include remove detected chords in plLyrics

        /// <summary>
        /// Include embedded chords into the list plLyrics
        /// </summary>
        public void PopulateEmbeddedChords()
        {            
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            int ticks = 0;
            int TicksOn = 0;
            int TicksOff = 0;

            string chordName = string.Empty;
            string lyric = string.Empty;
            string lastChordName = "<>";
            bool bFound = false;
            int insertIndex = 0;

            ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);

            // Beat
            // chord
            GridBeatChords = Analyser.GridBeatChords;
                        
            for (int beat = 1; beat <= GridBeatChords.Count; beat++)
            {
                chordName = GridBeatChords[beat];
                if (chordName != string.Empty && chordName != EmptyChord && chordName != ChordNotFound && chordName != lastChordName)
                {
                    lastChordName = chordName;
                    ticks = (beat - 1) * beatDuration;
                    bFound = false;

                    for (int j = 0; j < plLyrics.Count; j++)
                    {
                        TicksOn = plLyrics[j].TicksOn;
                        if (ticks < TicksOn)
                        {
                            insertIndex = j;
                            bFound = false;
                            break;
                        }
                        if (ticks == TicksOn)
                        {
                            if (plLyrics[j].CharType == plLyric.CharTypes.Text) 
                            {
                                lyric = plLyrics[j].Element.Item2;
                                lyric = formateLyricOfChord(chordName, lyric);
                                plLyrics[j].Element = (chordName, lyric);
                                bFound = true;
                            }
                            else
                            {
                                // This is a linefeed or paragraph => insert a new lyric
                                // Case of Alexandry Alexandra song                                
                                insertIndex = j;
                                bFound = false;
                            }
                            break;
                        }
                    }

                    if (!bFound)
                    {
                        lyric = formateLyricOfChord(chordName, "");
                        plLyrics.Insert(insertIndex, new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff });
                    }
                }
            }                   

        }

        private string formateLyricOfChord(string chord, string lyric)
        {
            if (chord != "")
            {
                // Add character '-' to lyrics when a chord and no lyric
                if (lyric.Trim() == "")
                {
                    lyric = new string('-', chord.Length + 1) + " ";                 
                }
                else if (lyric.Trim() == "-")
                {
                    lyric = new string('-', chord.Length + 1) + " ";
                }
            }
            return lyric;
        }

        public void RemoveEnbeddedChords()
        {
            _lyrics = ExtractLyrics();


            if (plLyrics.Count > 0)
            {
                #region rearrange lyrics
                // Guess spacing or not and carriage return or not
                GetLyricsSpacingModel();

                // Add a trailing space to each syllabe
                if (_lyricsspacing == lyricsSpacings.WithoutSpace)
                {
                    SetTrailingSpace();
                }

                // If zero carriage return in the lyrics
                if (!_bHasCarriageReturn)
                {
                    AddCarriageReturn();
                }
                #endregion rearrange lyrics

                // Search for the melody track
                _melodytracknum = GuessMelodyTrack();

                // Fix lyrics endtime to notes of the melody track end time
                CheckTimes();

                // Extract chords in lyrics
                bHasChordsInLyrics = HasChordsInLyrics(_lyrics);

                if (bHasChordsInLyrics)
                {
                    // Add chords found in lyrics in the list pllyrics
                    ExtractChordsInLyrics(_lyricstracknum);
                }
            }
        }

        #endregion include remove detected chords in plLyrics

        /// <summary>
        /// Returns dictionnary _gridbeatchords filled with chords issued from lyrics
        /// Used in frmChords TAB1, 2 & 3
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> FillGridBeatChordsWithLyrics()
        {
            int nbBeatsPerMeasure = sequence1.Numerator;
            int measure;
            int beat;
            int timeinmeasure;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int beats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);

            string chordName = string.Empty;
            int tickson;
            int numerator = sequence1.Numerator;

            _gridbeatchords = new Dictionary<int, string>();

            #region check
            if (plLyrics == null || plLyrics.Count == 0)
                return _gridbeatchords;
            #endregion check

            // Initialize dictionary
            if (plLyrics[plLyrics.Count - 1].Beat > beats)
                beats = plLyrics[plLyrics.Count - 1].Beat;

            for (int i = 1; i <= beats; i++)
            {
                _gridbeatchords[i] = ChordNotFound;
            }

            for (int i = 0; i < plLyrics.Count; i++)
            {
                plLyric pll = plLyrics[i];

                chordName = pll.Element.Item1;
                if (chordName != string.Empty)
                {
                    tickson = pll.TicksOn;
                    measure = 1 + tickson / _measurelen;
                    timeinmeasure = (int)GetTimeInMeasure(tickson);                    
                    beat = pll.Beat;

                    _gridbeatchords[beat] = chordName;
                }
            }

            return _gridbeatchords;
        }
       
        


        #region midi mesures

        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            
            Numerator = sequence1.Numerator;
            Division = sequence1.Division;

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                _nbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer

                int nbBeatsPerMeasure = sequence1.Numerator;
                int beatDuration = _measurelen / nbBeatsPerMeasure;
                _nbBeats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);
            }
        }


        /// <summary>
        /// Get time inside measure
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private float GetTimeInMeasure(int ticks)
        {
            // Num measure
            int curmeasure = 1 + ticks / _measurelen;
            // Temps dans la mesure
            float timeinmeasure = sequence1.Numerator - ((curmeasure * _measurelen - ticks) / (float)(_measurelen / sequence1.Numerator));

            return timeinmeasure;
        }

        #endregion midi mesures
    }
}
