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

using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MusicXml;

namespace Karaboss.MidiLyrics
{
    public partial class MidiLyricsMgmt
    {

        #region private

        private Dictionary<int, string> LyricsLines = new Dictionary<int, string>();
        private Dictionary<int, int> LyricsTimes = new Dictionary<int, int>();
        private Array LyricsLinesKeys;
        

        private readonly Sequence sequence1;

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

                               
        private readonly string _InternalSepLines = "¼";
        //private string _InternalSepParagraphs = "½";

        private readonly string ChordNotFound = "<Chord not found>";        
        private readonly string EmptyChord = "<Empty>";

        #endregion private


        #region public
        // Default List of lyrics
        public List<plLyric> plLyrics { get; set; }

        public List<plLyric> OrgplLyrics { get; set; }

        public int Numerator { get; set; }
        public int Division { get; set; }
        
        // FAB 21/11/2024 : list of the 2 different types of lyric:
        // 0 = lyric
        // 1 = text        
        public List<List<plLyric>> lstpllyrics { get; set; }

        public List<ChordItem> lstXmlChords { get; set; }

        /// <summary>
        /// Where do the chords come from?
        /// </summary>
        public enum ChordsOrigins
        {
            Lyrics,                 // Found inside the lyrics            
            XmlEmbedded,            // Found embedded inside a musicxml files
            MidiEmbedded,           // Found embedded inside a midi file if I make it (maybe merge these two last ones)
            Discovery,              // Default: no chords, either in the lyrics or embedded in the file (xml or midi). They must be discovered
        }
        public ChordsOrigins ChordsOriginatedFrom;

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

        
        private LyricTypes _lyrictype = LyricTypes.None;            // type lyric or text   
        public LyricTypes LyricType 
        {
            get { return _lyrictype; }
            set { _lyrictype = value; }
        }

        public bool bHasLyrics
        {
            get {
                if (OrgplLyrics.Count > 0)
                    return true;
                else if (plLyrics != null && plLyrics.Count > 0)
                    return true;
                else
                    return false; }
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
        private readonly string patternBracket = @"\[\b([CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*(?:[CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*)*)\]";
        private readonly string patternParenth = @"\(\b([CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*(?:[CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*)*)\)";
        private readonly string patternPercent = @"\%\b([CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*(?:[CDEFGAB](?:b|bb)*(?:#|##|sus|maj|m|min|aug|dim)*[\d\/]*)*)";

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
        // string chord & int ticks
        private Dictionary<int, (string, int)> _gridbeatchords;
        public Dictionary<int, (string, int)> GridBeatChords 
        {
            get { return _gridbeatchords; }
            set { _gridbeatchords = value; } 
        }               
              
        
        #endregion public


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sequence"></param>
        public MidiLyricsMgmt(Sequence sequence) 
        {                                                
            _lyricstracknum = -1;
            _melodytracknum = -1;
            
            _lyrictype = LyricTypes.None;
            ChordsOriginatedFrom = ChordsOrigins.Discovery;  // Default value = chords found nowhere. They must be discovered

            // Default values
            _removechordpattern = patternBracket;
            _chordDelimiter = ("[", "]");

            plLyrics = new List<plLyric>();
            lstpllyrics = new List<List<plLyric>>();
            lstXmlChords = new List<ChordItem>();

            sequence1 = sequence;
            
            UpdateMidiTimes();

            
            #region Minimal lyrics extraction

            OrgplLyrics = ExtractLyrics();

            // Extract chords in lyrics
            bHasChordsInLyrics = HasChordsInLyrics(_lyrics);
            if (bHasChordsInLyrics)
                ChordsOriginatedFrom = ChordsOrigins.Lyrics;

            // Search for the melody track            
            _melodytracknum = GuessMelodyTrack(OrgplLyrics);

            #endregion Minimal lyrics extraction

        }

        /// <summary>
        /// Reload lyrics with choosen options
        /// </summary>
        public void ResetDisplayChordsOptions(bool ShowChords)
        {
            //bShowChords = ShowChords;

            if (ShowChords)
            {
                // ===================
                // Show chords                                
                // ===================
                // Could be replaced by this
                switch (ChordsOriginatedFrom)
                {
                    case ChordsOrigins.Lyrics:
                        // 1. If chords are  already included in lyrics
                        // Add false lyrics in chords alone (instrumental) ???
                        FullExtractLyrics(ShowChords);
                        break;
                    case ChordsOrigins.XmlEmbedded:
                        // Chords are provided by the Xml score
                        if (plLyrics.Count == 0)
                            FullExtractLyrics(ShowChords);
                        // include xml chords in plLyrics
                        PopulateXmlChords(lstXmlChords);
                        CleanLyrics();
                        break;
                    
                    /* FUTURE USE
                    case ChordsOrigins.MidiEmbedded:
                        break;
                    */

                    case ChordsOrigins.Discovery:
                        // Chords have to be discovered
                        // 2. If chords are not included in lyrics,
                        // we have to detect chords and add them to the lyrics or add them to an extra
                        if (plLyrics.Count == 0)
                            FullExtractLyrics(ShowChords);

                        PopulateDetectedChords();
                        // Clean lyrics HERE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        CleanLyrics();
                        break;
                    
                    default:
                        break;
                }                                
            }
            else
            {
                // ===================
                // Do not show chords
                // ===================

                // 1. If chords are already included in lyrics
                // Remove false lyrics added to chords alone (instrumental)
                
                // 2. If chords are not included in lyrics
                // Chords have been added by detection or from xml or midi files to existing lyrics but also on additional false lyrics (chords alone in instrumentals)
                // So we have to delete all additions made by the chord discovery or adddition from files.                
                
                // All could be replaced by FullExtractLyrics();
                FullExtractLyrics(false);

                
            }
        }

        #region arrange lyrics

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
                    if (element.Trim().Length > 0)
                    { 
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
            }

            plLyrics = _tmpL;
        }

        /// <summary>
        /// Fix ticksoff of lyrics with melody notes
        /// </summary>
        private void FixTimes()
        {
            int ticksoff;
            int nexttickson;
            string elm; // = string.Empty;
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
                                //                                 
                                if (elm.Substring(elm.Length - 1, 1) != " ")
                                {
                                    plLyrics[k - 1].Element = (plLyrics[k - 1].Element.Item1, elm + " ");                                   
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
            if (_melodytracknum != -1 && _melodytracknum < sequence1.tracks.Count)
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
            //nbmodified = 0;


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
                    //Console.WriteLine("************** Error beat < previous " + plLyrics[i].CharType);
                    beat = previousbeat;
                }

                plLyrics[i].Beat = beat;
                previousbeat = beat;

            }
        }
        
        /// <summary>
        /// Set linefeeds & paragraphs to the end of the beat of the previous lyric
        /// /lyric -> beat/linefeed/lyric
        /// </summary>
        private void FixLinefeeds()
        {                    
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int TicksOn;       
            int TicksOff;
            int EndBeatTicks;            
            int beat;

            int measure;
            int ticks;

            for (int k = 0; k < plLyrics.Count; k++)
            {
                
                if (plLyrics[k].CharType != plLyric.CharTypes.Text)
                {
                                        
                    // If previous is a text
                    if (k > 0 && plLyrics[k - 1].CharType == plLyric.CharTypes.Text)
                    {
                        
                        // 1. Try to move the CR to the beginning of the measure
                        measure = 1 + plLyrics[k].TicksOn/_measurelen;
                        // ticks of start measure
                        ticks = (measure - 1) * _measurelen;
                        // End of previous lyric
                        TicksOff = plLyrics[k - 1].TicksOff;
                        if (ticks > TicksOff)
                        {
                            plLyrics[k].TicksOn = ticks;
                            plLyrics[k].TicksOff = ticks;
                        }
                        else
                        {


                            // 2. Try to move the CR to the end of Beat of the previous lyric
                            TicksOff = plLyrics[k - 1].TicksOff;
                            beat = 1 + TicksOff / beatDuration;

                            //beat = plLyrics[k - 1].Beat;
                            EndBeatTicks = beat * beatDuration;

                            if (k < plLyrics.Count - 1)
                            {
                                TicksOn = plLyrics[k + 1].TicksOn;
                                if (EndBeatTicks < TicksOn)
                                {
                                    plLyrics[k].TicksOn = EndBeatTicks;
                                    plLyrics[k].TicksOff = EndBeatTicks;
                                }
                            }
                        }



                    }
                }
            }
        }

        #endregion arrange lyrics


        #region Full extract lyrics
       
        /// <summary>
        /// Lyrics extraction & display
        /// </summary>
        private List<plLyric> ExtractLyrics()
        {
            double l_text = 1;
            double l_lyric = 1;
            //string lyrics = string.Empty;

            List<plLyric> lst = new List<plLyric>();            

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
                    _lyrics = sequence1.tracks[trklyric].TotalLyricsL;
                }
                else
                {
                    // Elimine lyrics et choisi les textes                    
                    _lyrictype = LyricTypes.Text;
                    _lyricstracknum = trktext;
                    _lyrics = sequence1.tracks[trktext].TotalLyricsT;
                }
            }
            else if (trktext >= 0)
            {
                _lyrictype = LyricTypes.Text;
                _lyricstracknum = trktext;
                _lyrics = sequence1.tracks[trktext].TotalLyricsT;
            }
            else if (trklyric >= 0)
            {
                _lyrictype = LyricTypes.Lyric;
                _lyricstracknum = trklyric;
                _lyrics = sequence1.tracks[trklyric].TotalLyricsL;
            }
            else
            {
                _lyrictype = LyricTypes.None;
                _lyrics = string.Empty;
            }

            if (_lyrictype == LyricTypes.Lyric)
                lst = lstpllyrics[0];
            else if (_lyrictype == LyricTypes.Text)
                lst = lstpllyrics[1];
                   

            return lst;
        }

        private List<plLyric> LoadLyricsText(Sanford.Multimedia.Midi.Track track)
        {                                    
            List<plLyric> pll = new List<plLyric>();

            int plTicksOn; // = 0;
            int plTicksOff; // = 0;
            
            for (int k = 0; k < track.LyricsText.Count; k++)
            {
                // Stockage dans liste plLyrics
                plLyric.CharTypes plType = (plLyric.CharTypes)track.LyricsText[k].Type;
                string plElement = track.LyricsText[k].Element;

                // Start time for a lyric
                plTicksOn = track.LyricsText[k].TicksOn;

                // Stop time for a lyric (maxi 1 beat ?)
                //if (plType == plLyric.CharTypes.Text)
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

            int plTicksOn; // = 0;
            int plTicksOff; // = 0;            
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
                    plTicksOff = plTicksOn + _measurelen;

                    // Check if this is a chord (IsChord)
                    //Console.WriteLine("");
                    if (plElement.Contains("--"))
                        pll.Add(new plLyric() { CharType = plType, Element = ("", plElement), TicksOn = plTicksOn, TicksOff = plTicksOff, IsChord = true });
                    else
                        pll.Add(new plLyric() { CharType = plType, Element = ("", plElement), TicksOn = plTicksOn, TicksOff = plTicksOff, IsChord = false });
                }
            }            

            return pll;
        }

        /// <summary>
        /// Full extract of lyrics. Launch all functions
        /// </summary>
        public void FullExtractLyrics(bool ShowChords)
        {
            try
            {
                // plLyrics is initialized with a deep copy of OrgplLyrics
                // We can't use plLyrics = OrgplLyrics; This does not work, because objects remain linked            
                plLyrics = new List<plLyric>();
                for (int i = 0; i < OrgplLyrics.Count; i++)
                {
                    plLyric p = OrgplLyrics[i];
                    plLyrics.Add(new plLyric() { Beat = p.Beat, CharType = p.CharType, Element = (p.Element.Item1, p.Element.Item2), TicksOn = p.TicksOn, TicksOff = p.TicksOff, IsChord = p.IsChord });
                }
               
                if (plLyrics.Count ==  0)
                    return;
                
                // Remove first and last linefeed/paragrpah if exists
                if (plLyrics[0].CharType != plLyric.CharTypes.Text)
                    plLyrics.RemoveAt(0);

                if (plLyrics.Count == 0) 
                    return ;
                    
                if (plLyrics[plLyrics.Count - 1].CharType != plLyric.CharTypes.Text)
                    plLyrics.RemoveAt(plLyrics.Count - 1);

                if (plLyrics.Count == 0)
                    return;

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
                //_melodytracknum = GuessMelodyTrack(plLyrics);

                // Fix lyrics endtime to notes of the melody track end time
                FixTimes();

                // Move linefeeds to the end of the previous lyric
                FixLinefeeds();


                // Extract chords from lyrics ALWAYS ????                
                if (bHasChordsInLyrics && ShowChords)
                {
                    // Add chords found in lyrics in the list pllyrics
                    ExtractChordsInLyrics(ShowChords);
                }

                //TestCheckTimes();                             

                #region clean lyrics

                // Remove empty lyrics
                plLyrics = RemoveEmptyLyrics(plLyrics);
                plLyrics = RemoveExtraLinefeeds(plLyrics);

                #endregion clean lyrics
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Full extract lyrics


        #region analyse lyrics

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
                if (btest2 && btest1)
                    return;
            }


        }

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
                    if (sequence1.tracks[i].TotalLyricsT.Length > 0 && sequence1.tracks[i].TotalLyricsT.Length > max)
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
            //string tx; // = string.Empty;
            int max = -1;
            int track = -1;

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {                
                if (sequence1.tracks[i].TotalLyricsL != null)
                {
                    //tx = sequence1.tracks[i].TotalLyricsL;
                    if (sequence1.tracks[i].TotalLyricsL.Length > 0 && sequence1.tracks[i].TotalLyricsL.Length > max)
                    {
                        max = sequence1.tracks[i].TotalLyricsL.Length;
                        track = i;
                    }
                }
            }
                        
            return track;
                        
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

            // With Percent
            Regex chordCheck3 = new Regex(patternPercent);


            MatchCollection mc = chordCheck.Matches(s);
            MatchCollection mc2 = chordCheck2.Matches(s);
            MatchCollection mc3 = chordCheck3.Matches(s);

            // Exlude copyright (C) => greater than 1
            if (mc.Count > 1 || mc2.Count > 1 || mc3.Count > 1) 
                return true;
            else
                return false;
        }

        /// <summary>
        /// Guess which track contains the melody
        /// A very complex search :-)
        /// </summary>
        /// <returns></returns>
        private int GuessMelodyTrack(List<plLyric> l)
        {
            // Comparer timing pistes à pistes
            //int tracknum = _lyricstracknum;
            int nbfound; // = 0;
            int nbnotes; // = 0;
            int diff; // = 0;

            float fRatioNotes; //= 0;
            float maxRatioNotes = 0;

            int maxDiff = -1;
            int trackfnote = -1;

            int delta = 30; // 20 origin

            // Eliminer les cr
            int nblyrics = 0;
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].CharType == plLyric.CharTypes.Text && l[i].TicksOn > 0)
                    nblyrics++;
            }

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                nbfound = 0;
                //nbnotes = 0;

                Sanford.Multimedia.Midi.Track track = sequence1.tracks[i];

                if (track.ContainsNotes == true && track.MidiChannel != 9)
                {
                    // comparaison 1 : nombre de notes versus nombre de lyrics
                    // Plus le nombre de notes se rapproche de celui de lyrics, plus c'est mieux
                    nbnotes = track.Notes.Count;

                    // Avoid tracks with not enough notes and those having too many notes compared to lyrics                    
                    if (nbnotes > nblyrics / 2 && nbnotes < nblyrics * 3)
                    {
                        //diff = nbnotes - nblyrics;
                        //if (diff < 0) diff = -diff;

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
                                for (int k = 0; k < l.Count; k++)
                                {
                                    int tl = l[k].TicksOn;
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

        #endregion analyse lyrics


        #region clean detected chords

        /// <summary>
        /// Clean for detected chords; launch all functions
        /// </summary>
        public void CleanLyrics()
        {            
            plLyrics = RemoveEmptyLyrics(plLyrics);            
            plLyrics = AddLineFeedAfterInstrumental(plLyrics);
            plLyrics = TruncateInstrumental(plLyrics);
            plLyrics = AddLineFeedBeforeInstrumental(plLyrics);            
            plLyrics = RemoveExtraLinefeeds(plLyrics);            
        }


        /// <summary>
        /// Remove empty lyrics
        /// To be launch first
        /// </summary>
        /// <returns></returns>
        private List<plLyric> RemoveEmptyLyrics(List<plLyric> l)
        {
            string lyric; 
            List<plLyric> lst = new List<plLyric>();            
            for (int i = 0; i < l.Count; i++)
            {
                lyric = l[i].Element.Item2;
                if (lyric.Trim().Length > 0)
                {
                    lst.Add(l[i]);
                }
            }
            return lst;
        }


        /// <summary>
        /// Add a Linefeed after an instrumental
        /// </summary>
        /// <returns></returns>
        private List<plLyric> AddLineFeedAfterInstrumental(List<plLyric> l)
        {
            //string chord;
            //string lyric;
            int nbChords = 0;            
            List<plLyric> lst = new List<plLyric>();
            plLyric plL; // = new plLyric();
            plLyric pcL2; // = new plLyric();

            for (int i = 0; i < l.Count; i++)
            {
                plL = l[i];
                //chord = plL.Element.Item1;
                //lyric = plL.Element.Item2;

                if (plL.IsChord)
                    nbChords++;
                else if (plL.CharType == plLyric.CharTypes.Text)
                {
                    // Add Linefeed before the normal element
                    //if (plL.CharType == plLyric.CharTypes.Text && nbChords > 1 && lyric.IndexOf("-") == -1)
                    if (nbChords > 1 && !plL.IsChord)
                    {
                        nbChords = 0;

                        pcL2 = new plLyric() {
                            Beat = plL.Beat,
                            CharType = plLyric.CharTypes.LineFeed,
                            Element = ("", _InternalSepLines),
                            TicksOn = plL.TicksOn,
                            TicksOff = plL.TicksOff,
                        };
                        lst.Add(pcL2);
                    }
                    else
                    {
                        nbChords = 0;
                    }
                }
                //else if(plL.CharType == plLyric.CharTypes.ParagraphSep)
                else if (plL.CharType != plLyric.CharTypes.Text)
                {
                    nbChords = 0;
                }

                // Add normal element
                lst.Add(plL);
            }

            return lst;
        }


        /// <summary>
        /// Truncate instrumental / Add a linefeed 
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private List<plLyric> TruncateInstrumental(List<plLyric> l)
        {
            int nbChords = 0;
            //string chord;
            //string lyric;
            List<plLyric> lst = new List<plLyric>();
            plLyric plL; // = new plLyric();
            plLyric pcL2; // = new plLyric();

            for (int i = 0; i < l.Count; i++)
            {
                plL = l[i];
                //chord = plL.Element.Item1;
                //lyric = plL.Element.Item2;

                // Search for a long list of chords (instrumental)
                if (plL.IsChord)
                    nbChords++;
                else
                    nbChords = 0;                

                // Add normal element
                lst.Add(plL);

                // Add a linefeed each serie of 4 chords
                if (i < l.Count - 1)
                {
                    plLyric p = l[i + 1];
                    if (p.CharType != plLyric.CharTypes.LineFeed && p.CharType != plLyric.CharTypes.ParagraphSep)
                    {
                        if ((Numerator > 2 && nbChords > Numerator - 1) || Numerator < 3 && nbChords > 3)
                        {
                            nbChords = 0;
                            pcL2 = new plLyric() {
                                Beat = plL.Beat,
                                CharType = plLyric.CharTypes.LineFeed,
                                Element = ("", _InternalSepLines),
                                TicksOn = plL.TicksOn,
                                TicksOff = plL.TicksOff,
                            };
                            lst.Add(pcL2);
                        }
                    }
                }
            }

            return lst;
        }


        /// <summary>
        /// Add a linefeed before an instrumental
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private List<plLyric> AddLineFeedBeforeInstrumental(List<plLyric> l)
        {
            int nbChords = 0;            
            //string chord;
            //string lyric;
            List<plLyric> lst = new List<plLyric>();
            plLyric plL; // = new plLyric();
            plLyric pcL2; // = new plLyric();

            // Descending loop !!!!!
            for (int i = l.Count - 1; i >= 0; i--)
            {
                              
                plL = l[i];
                //chord = plL.Element.Item1;
                //lyric = plL.Element.Item2;

                // Search for a long list of chords (instrumental)
                if (plL.IsChord)
                {
                    nbChords++;
                }
                else if (plL.CharType == plLyric.CharTypes.Text)
                {
                    // on the way down, we found a lyric preceded by several chords
                    // We must have <lyric> <linefeed> <chord1> <chord2> ....
                    // So insert a linefeed at position 0 and the lyric at position 0
                    if (nbChords > 1 && !plL.IsChord)
                    {
                        nbChords = 0;

                        pcL2 = new plLyric()
                        {
                            Beat = plL.Beat,
                            CharType = plLyric.CharTypes.LineFeed,
                            Element = ("", _InternalSepLines),
                            TicksOn = plL.TicksOn,
                            TicksOff = plL.TicksOff,
                        };
                        lst.Insert(0, pcL2);
                    }
                    else
                    {
                        nbChords = 0;
                    }

                }
                //else if (plL.CharType == plLyric.CharTypes.ParagraphSep)
                else if (plL.CharType != plLyric.CharTypes.Text)
                {
                    nbChords = 0;
                }
                // Add normal element at position 0
                lst.Insert(0, plL);
            }

            return lst;
        }


        /// <summary>
        /// Eliminate consecutive carriage returns
        /// To be launch at the end
        /// </summary>
        /// <returns></returns>
        private List<plLyric> RemoveExtraLinefeeds(List<plLyric> l)
        {
            string lyric; 
            string lastlyric = "<>";
            bool bAdd; // = true;
            List<plLyric> lst = new List<plLyric>(); 
            
            for (int i = 0; i < l.Count; i++)
            {
                lyric = l[i].Element.Item2;
                bAdd = true;
                             
                if (lyric.Trim() == _InternalSepLines)
                {
                    if (lastlyric == _InternalSepLines)
                        bAdd = false;

                    lastlyric = _InternalSepLines;
                }
                else
                {
                    lastlyric = "<>";
                }

                if (bAdd)
                {
                    lst.Add(l[i]);
                }
            }            
            return lst;
        }


        #endregion clean


        #region extract chords in lyrics
        /// <summary>
        /// Extract chords [A], [B] ... written in the lyrics
        /// and add them to plLyric
        /// </summary>
        /// <param name="tracknum"></param>
        private void ExtractChordsInLyrics(bool ShowChords)
        {
            // TODO improve performance
            // avoid to recalculate pattern & Chorddelimiter
            
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

                    // With Percent
                    Regex chordCheck3 = new Regex(patternPercent);

                    MatchCollection mc = chordCheck.Matches(lyricElement);
                    MatchCollection mc2 = chordCheck2.Matches(lyricElement);
                    MatchCollection mc3 = chordCheck3.Matches(lyricElement);

                    bFound = false;

                    if (mc.Count > 0)
                    {
                        //for (int j = 0; j < mc.Count; j++) 
                        //{ 

                        _chordDelimiter = ("[", "]");
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
                        _chordDelimiter = ("(", ")");
                        chordElement = mc2[0].Value;
                        _removechordpattern = patternParenth;

                        bFound = true;

                        if (chordElement.Length > 2)
                        {
                            chordElement = chordElement.Substring(1, chordElement.Length - 2);
                        }
                    }
                    else if (mc3.Count > 0)
                    {
                        _chordDelimiter = ("%", "");
                        chordElement = mc3[0].Value;
                        _removechordpattern = patternPercent;

                        bFound = true;

                        if (chordElement.Length >= 2)
                        {
                            chordElement = chordElement.Substring(1, chordElement.Length - 1);
                        }
                    }

                    if (bFound)
                    {
                        if (ShowChords)
                        {
                            lyricElement = formateLyricOfDetectedChord(chordElement, lyricElement);
                        }

                        // Update list item with chord                        
                        plLyrics[i].Element = (chordElement, lyricElement);
                        
                    }

                }
            }     
        }

        #endregion extract chords in lyrics
     

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
            //int tickson;
            //int ticksoff;
            int beat;
            int nbBeatsPerMeasure = sequence1.Numerator;
            int currentbeat = 1;
            string currenttext = string.Empty;
            int currentmeasure;// = 0;
            string cr = Environment.NewLine;

            string lyricElement;
            string replace = @"";

            for (int i = 0; i < plLyrics.Count; i++)
            {
                if (plLyrics[i].CharType == plLyric.CharTypes.Text)
                {
                    //tickson = plLyrics[i].TicksOn;
                    //ticksoff = plLyrics[i].TicksOff;
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
            int newtime; // = 0;
            string cr = Environment.NewLine;
            string lyricElement; // = string.Empty;
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
                //LyricsTimesKeys = LyricsTimes.Keys.ToArray();
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
            int lyricticksoff; // = 0;            
            bool bfound = false;
            //string txtcontent; // = string.Empty;

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
                GridBeatChords[i] = (InterpreteChord(GridBeatChords[i].Item1), GridBeatChords[i].Item2);
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

        #endregion clean chords labels
      

        #region include remove detected chords in plLyrics

        /// <summary>
        /// Include chords updated from frmChords
        /// </summary>
        public void PopulateUpdatedChords(Dictionary<int, (string, int)> gbc)
        {
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            int ticks;
            int TicksOn;
            int TicksOff = 0;

            string chordName;
            string lyric;
            string lastChordName = "<>";
            bool bFound;
            int insertIndex;
            int removeIndex;

            // Beat
            // (chord, ticks)
            GridBeatChords = gbc;
            removeIndex = -1;
            bool bExit = false;

            if (ChordDelimiter == (null, null) || ChordDelimiter == ("", "") || RemoveChordPattern == null)
            {
                ChordDelimiter = ("[", "]");
                RemoveChordPattern = patternBracket;
            }

            // Remove chords from plLyrics that have been deleted
            do
            {
                // Loop on each chord of plLyrics
                for (int j = 0; j < plLyrics.Count; j++)
                {
                    plLyric pll = plLyrics[j];

                    if (pll.CharType == plLyric.CharTypes.Text)
                    {
                        chordName = pll.Element.Item1;
                        lyric = pll.Element.Item2;
                        TicksOn = pll.TicksOn;
                        bFound = false;

                        if (chordName != "")
                        {
                            // Search if this chord exists in GridBeatChord
                            for (int beat = 1; beat <= GridBeatChords.Count; beat++)
                            {
                                if (GridBeatChords[beat].Item2 > TicksOn)
                                    break;

                                if (chordName == GridBeatChords[beat].Item1 && TicksOn == GridBeatChords[beat].Item2)
                                {
                                    bFound = true;
                                    break;
                                }
                            }

                            // chord does not exists in GridBeatChord
                            if (!bFound)
                            {
                                removeIndex = j;
                                break;
                            }
                        }
                    }

                    if (j == plLyrics.Count - 1)
                    {                        
                        bExit = true;
                    }
                }

                if (!bExit)
                {
                    // If pure chord => remove the element
                    // If chod + lyric => keep the lyric and remove the chord
                    if (removeIndex >= 0 && removeIndex < plLyrics.Count) 
                    {
                        if (plLyrics[removeIndex].IsChord)
                        {
                            plLyrics.RemoveAt(removeIndex);
                        }
                        else
                        {
                            plLyrics[removeIndex].Element = ("", Regex.Replace(plLyrics[removeIndex].Element.Item2, RemoveChordPattern, @""));

                        }
                    }
                    
                }

            } while (!bExit);


            // 2. Add new chords
            for (int beat = 1; beat <= GridBeatChords.Count; beat++)
            {
                if (GridBeatChords.ContainsKey(beat))
                {
                    chordName = GridBeatChords[beat].Item1;

                    if (chordName != string.Empty && chordName != EmptyChord && chordName != ChordNotFound && chordName != lastChordName)
                    {
                        lastChordName = chordName;
                        
                        //ticks = (beat - 1) * beatDuration;
                        ticks = GridBeatChords[beat].Item2;
                        
                        bFound = false;
                        insertIndex = -1;

                        for (int j = 0; j < plLyrics.Count; j++)
                        {
                            // Consider only Text
                            plLyric pll = plLyrics[j];
                            
                            TicksOn = pll.TicksOn;
                            TicksOff = pll.TicksOff;

                            if (ticks == TicksOn)
                            {
                                // Chord must be replaced [C]La maison => [D]La maison          NON !!!  toujours !
                                if (pll.CharType == plLyric.CharTypes.Text)
                                {
                                    
                                    // Action always needed, not only if the chord is updated
                                    //if (chordName != pll.Element.Item1)
                                    //{
                                    lyric = pll.Element.Item2;
                                    // Remove chord in the lyric
                                    lyric = Regex.Replace(lyric, RemoveChordPattern, @"");
                                    lyric = formateLyricOfChord(chordName, lyric);                                    
                                    lyric = ChordDelimiter.Item1 + chordName + ChordDelimiter.Item2 + lyric;
                                    
                                    pll.Element = (chordName, lyric);
                                    //}
                                    bFound = true;

                                }                                                                                                               
                                else
                                {
                                    // This is a linefeed or paragraph => insert a new lyric
                                    // Case of Alexandrie Alexandra song                                

                                    // Insert the chord after the linefeed of the lyrics line
                                    // Test if the element after the linefeed hast not the same TicksOn
                                    if (j + 1 < plLyrics.Count)
                                    {
                                        if (ticks == plLyrics[j + 1].TicksOn)
                                        {
                                            lyric = plLyrics[j + 1].Element.Item2;
                                            lyric = Regex.Replace(lyric, RemoveChordPattern, @"");
                                            lyric = formateLyricOfChord(chordName, lyric);
                                            lyric = ChordDelimiter.Item1 + chordName + ChordDelimiter.Item2 + lyric;

                                            plLyrics[j + 1].Element = (chordName, lyric);
                                            plLyrics[j + 1].IsChord = false;
                                            bFound = true;
                                        }
                                        else
                                        {
                                            insertIndex = j + 1;
                                            bFound = false;
                                        }
                                    }
                                    else
                                    {
                                        insertIndex = j;
                                        bFound = false;
                                    }
                                }
                                break;
                            }

                            else if (ticks > TicksOn && ticks < TicksOff && plLyrics[j].CharType == plLyric.CharTypes.Text && plLyrics[j].IsChord == false)
                            {
                                lyric = plLyrics[j].Element.Item2;
                                lyric = Regex.Replace(lyric, RemoveChordPattern, @"");
                                lyric = formateLyricOfChord(chordName, lyric);
                                lyric = ChordDelimiter.Item1 + chordName + ChordDelimiter.Item2 + lyric;
                                plLyrics[j].Element = (chordName, lyric);
                                plLyrics[j].IsChord = false;
                                bFound = true;
                                break;
                            }
                            else if (ticks < TicksOn)
                            {
                                // ticks is smaller than this TicksOn => the chord has to be inserted at its place as a new element
                                insertIndex = j;
                                bFound = false;
                                break;
                            }
                        }

                        if (!bFound)
                        {                            
                            lyric = formateLyricOfChord(chordName, "");
                            
                            // Add chord name to the lyric: Replace lyric '-- ' by '[A]-- '
                            if (ChordDelimiter == (null, null) || ChordDelimiter == ("", ""))
                                ChordDelimiter = ("[", "]");

                            lyric = ChordDelimiter.Item1 + chordName + ChordDelimiter.Item2 + lyric;

                            // Ticks was never smaller or equal to an existing TickOn => is has to be added at the end
                            if (insertIndex == -1)
                            {
                                plLyrics.Add(new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                            }
                            else
                            {
                                // ticks was found smaller or equal to an existing TicksOn
                                plLyrics.Insert(insertIndex, new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                            }
                        }
                    }
                }
            }

            //TestCheckTimes();
            // Add tickoff to new elements chord ?
            //CheckTimes();
        }

        /// <summary>
        /// Include detected chords into the list plLyrics
        /// </summary>
        public void PopulateDetectedChords()
        {            
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            int ticks;
            int TicksOn;
            int TicksOff = 0;

            string chordName; 
            string lyric;
            string lastChordName = "<>";
            bool bFound; 
            int insertIndex;
            
            // Launch chords discovery
            ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);

            // Beat
            // chord
            GridBeatChords = Analyser.GridBeatChords;
                        
            for (int beat = 1; beat <= GridBeatChords.Count; beat++)
            {
                if (GridBeatChords.ContainsKey(beat))
                {
                    chordName = GridBeatChords[beat].Item1;

                    if (chordName != string.Empty && chordName != EmptyChord && chordName != ChordNotFound && chordName != lastChordName)
                    {
                        lastChordName = chordName;
                        
                        // With Analyser, the ticks precision is a beat
                        ticks = (beat - 1) * beatDuration;
                        
                        bFound = false;
                        insertIndex = -1;

                        for (int j = 0; j < plLyrics.Count; j++)
                        {
                            TicksOn = plLyrics[j].TicksOn;
                            TicksOff = plLyrics[j].TicksOff;                          

                            if (ticks == TicksOn)
                            {
                                // ticks has the same value than a TicksOn, the chord has to be added to the lyric in the field 'chord'
                                if (plLyrics[j].CharType == plLyric.CharTypes.Text)
                                {
                                    lyric = plLyrics[j].Element.Item2;
                                    lyric = formateLyricOfChord(chordName, lyric);
                                    plLyrics[j].Element = (chordName, lyric);
                                    plLyrics[j].IsChord = false;
                                    bFound = true;
                                }
                                else
                                {
                                    // This is a linefeed or paragraph => insert a new lyric
                                    // Case of Alexandrie Alexandra song                                

                                    // Insert the chord after the linefeed of the lyrics line
                                    // Test if the element after the linefeed hast not the same TicksOn
                                    if (j + 1 < plLyrics.Count)
                                    {
                                        if (ticks == plLyrics[j + 1].TicksOn)
                                        {
                                            lyric = plLyrics[j + 1].Element.Item2;
                                            lyric = formateLyricOfChord(chordName, lyric);
                                            plLyrics[j + 1].Element = (chordName, lyric);
                                            plLyrics[j + 1].IsChord = false;
                                            bFound = true;
                                        }
                                        else
                                        {
                                            insertIndex = j + 1;
                                            bFound = false;
                                        }
                                    }
                                    else
                                    {
                                        insertIndex = j;
                                        bFound = false;
                                    }
                                }
                                break;
                            }
                            
                            else if (ticks > TicksOn && ticks < TicksOff && plLyrics[j].CharType == plLyric.CharTypes.Text && plLyrics[j].IsChord == false)
                            {
                                lyric = plLyrics[j].Element.Item2;
                                lyric = formateLyricOfChord(chordName, lyric);
                                plLyrics[j].Element = (chordName, lyric);
                                plLyrics[j].IsChord = false;
                                bFound = true;
                                break;
                            }
                            else if (ticks < TicksOn)
                            {
                                // ticks is smaller than this TicksOn => the chord has to be inserted at its place as a new element
                                insertIndex = j;
                                bFound = false;
                                break;
                            }                            
                        }

                        if (!bFound)
                        {
                            lyric = formateLyricOfChord(chordName, "");

                            // Ticks was never smaller or equal to an existing TickOn => is has to be added at the end
                            if (insertIndex == -1)
                            {
                                plLyrics.Add(new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                            }
                            else
                            {
                                // ticks was found smaller or equal to an existing TicksOn
                                plLyrics.Insert(insertIndex, new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                            }
                        }
                    }
                }
            }
            
            //TestCheckTimes();
            // Add tickoff to new elements chord ?
            //CheckTimes();
        }

        /// <summary>
        /// Use case: chords are detected
        /// Create false lyrics when chord is alone (instrumental)
        /// </summary>
        /// <param name="chord"></param>
        /// <param name="lyric"></param>
        /// <returns></returns>
        private string formateLyricOfChord(string chord, string lyric)
        {                                    
            if (chord != string.Empty)
            {
                int L = (chord.Length > 3) ? chord.Length + 2 : chord.Length + 1;

                // Add character '-' to lyrics when a chord and no lyric
                if (lyric.Trim() == "")
                {
                    lyric = new string('-', L) + " ";                 
                }
                else if (lyric.Trim() == "-" || lyric.Trim() == ".")
                {
                    lyric = new string('-', L) + " ";
                }
            }
            return lyric;
        }

        /// <summary>
        /// Use case: Chords are in lyrics
        /// Create false lyric when chord is alone (instrumental)
        /// </summary>
        /// <param name="chord"></param>
        /// <param name="lyric"></param>
        /// <returns></returns>
        private string formateLyricOfDetectedChord(string chord, string lyric)
        {            
            string s = lyric;

            if (chord != string.Empty)
            {
                int L = (chord.Length > 3) ? chord.Length + 2 : chord.Length + 1;

                // Remove chord in the lyric
                s = Regex.Replace(lyric, RemoveChordPattern, @"");
                
                // Add character '-' to lyrics when a chord and no lyric
                if (s.Trim() == "")
                {
                    s = new string('-', L) + " ";
                }       
                else if (s.Trim() == "-" || s.Trim() == ".")
                {
                    s = new string('-', L) + " ";
                }
                return _chordDelimiter.Item1 + chord + _chordDelimiter.Item2 + s;
            }

            return s;
        }


        #endregion include remove detected chords in plLyrics



        #region include remove xml chords in plLyrics

        public void PopulateXmlChords(List<MusicXml.ChordItem> lstChords)
        {
            string chordName;
            string lyric;            

            int ticks;
            int TicksOn;
            int TicksOff = 0;

            bool bFound;
            int insertIndex;

            int beat;

            if (sequence1 == null || sequence1.Numerator == 0) 
            { 
                MessageBox.Show("Invalid sequence: null or Numerator = 0", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int nbBeatsPerMeasure = sequence1.Numerator;
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            foreach (ChordItem chord in lstChords)
            {
                chordName = chord.ChordName;
                ticks = chord.TicksOn;
                bFound = false;
                insertIndex = -1;
                beat = 1 + ticks / beatDuration;

                for (int j = 0; j < plLyrics.Count; j++)
                {
                    TicksOn = plLyrics[j].TicksOn;
                    TicksOff = plLyrics[j].TicksOff;

                    if (ticks == TicksOn)
                    {
                        // ticks has the same value than a TicksOn, the chord has to be added to the lyric in the field 'chord'
                        if (plLyrics[j].CharType == plLyric.CharTypes.Text)
                        {
                            lyric = plLyrics[j].Element.Item2;
                            lyric = formateLyricOfChord(chordName, lyric);
                            plLyrics[j].Element = (chordName, lyric);
                            plLyrics[j].IsChord = false;
                            bFound = true;
                        }
                        else
                        {
                            // This is a linefeed or paragraph => insert a new lyric
                            // Case of Alexandrie Alexandra song                                

                            // Insert the chord after the linefeed of the lyrics line
                            // Test if the element after the linefeed hast not the same TicksOn
                            if (j + 1 < plLyrics.Count)
                            {
                                if (ticks == plLyrics[j + 1].TicksOn)
                                {
                                    lyric = plLyrics[j + 1].Element.Item2;
                                    lyric = formateLyricOfChord(chordName, lyric);
                                    plLyrics[j + 1].Element = (chordName, lyric);
                                    plLyrics[j + 1].IsChord = false;
                                    bFound = true;
                                }
                                else
                                {
                                    insertIndex = j + 1;
                                    bFound = false;
                                }
                            }
                            else
                            {
                                insertIndex = j;
                                bFound = false;
                            }
                        }
                        break;
                    }

                    else if (ticks > TicksOn && ticks < TicksOff && plLyrics[j].CharType == plLyric.CharTypes.Text && plLyrics[j].IsChord == false)
                    {
                        lyric = plLyrics[j].Element.Item2;
                        lyric = formateLyricOfChord(chordName, lyric);
                        plLyrics[j].Element = (chordName, lyric);
                        plLyrics[j].IsChord = false;
                        bFound = true;
                        break;
                    }
                    else if (ticks < TicksOn)
                    {
                        // ticks is smaller than this TicksOn => the chord has to be inserted at its place as a new element
                        insertIndex = j;
                        bFound = false;
                        break;
                    }
                }

                if (!bFound)
                {
                    lyric = formateLyricOfChord(chordName, "");

                    // Ticks was never smaller or equal to an existing TickOn => is has to be added at the end
                    if (insertIndex == -1)
                    {
                        plLyrics.Add(new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                    }
                    else
                    {
                        // ticks was found smaller or equal to an existing TicksOn
                        plLyrics.Insert(insertIndex, new plLyric() { Beat = beat, CharType = plLyric.CharTypes.Text, Element = (chordName, lyric), TicksOn = ticks, TicksOff = TicksOff, IsChord = true });
                    }
                }
            }



        }

        #endregion include remove xml chords in plLyrics



        #region tests

        /*
        private void CheckTimes()
        {
            int lastTime = -1;
            int t = -1;
            for (int i = 0; i < plLyrics.Count; i++)
            {
                t = plLyrics[i].TicksOn;
                if (t < lastTime)
                {
                    MessageBox.Show("Error: times not in order", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                lastTime = t;
            }
        }
        */
        #endregion tests


        #region Display text of lyrics & chords

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
            //string lyricElement;// = string.Empty;
            string replace = @"";

            int interval;
            int ticksoff;
            int tickson;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int _measure;

            string chordName;// = string.Empty;

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

                        pll = new plLyric() {
                            CharType = plLyric.CharTypes.LineFeed,
                            Beat = lastbeat,                            
                        };
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
                                    pll = new plLyric() {
                                        CharType = plLyric.CharTypes.LineFeed,
                                        Beat = lastbeat,
                                        TicksOn = beat * beatDuration,
                                    };                                    

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
                                    pll = new plLyric() {
                                        CharType = plLyric.CharTypes.LineFeed,
                                        Beat = lastbeat,
                                        TicksOn = beat * beatDuration,
                                    };
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
                                //_measure = 1 + (beat - 1) / nbBeatsPerMeasure;
                                //Console.WriteLine(string.Format("**** Instrumental after line : measure: {0} Beat: {1} **************", _measure, beat));

                                // TODO : add a linefeed to 1st time of this measure (this beat ?)
                                // Do not forget the end of the song : no linefeed
                                pll = new plLyric() {
                                    CharType = plLyric.CharTypes.ParagraphSep,
                                    Beat = beat,
                                    TicksOn = beat * beatDuration,
                                };
                                diclyr[beat].Add(pll);
                            }
                        }
                    }
                }
            }

            // Add a cr to the Last lyric (in case of instrumental after the last lyric)
            if (plLyrics.Count > 0 && plLyrics[plLyrics.Count - 1].CharType == plLyric.CharTypes.Text)
            {
                pll = new plLyric() {
                    CharType = plLyric.CharTypes.ParagraphSep,
                    Beat = plLyrics[plLyrics.Count - 1].Beat,
                };

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
                    if (beat <= _nbBeats && beat <= GridBeatChords.Count)
                    {
                        chordName = GridBeatChords[beat].Item1;


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

                                    if (chordName != "" && chordName != _currentChordName)
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

        /// <summary>
        /// Use case : Display Lyrics & chords. 
        /// Returns dictionnary _gridbeatchords filled with chords issued from lyrics
        /// Used in frmChords TAB1, 2 & 3 and frmLyrics
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, (string, int)> FillGridBeatChordsWithLyricsChords()
        {
            int nbBeatsPerMeasure = sequence1.Numerator;
            //int measure;
            int beat;
            //int timeinmeasure;
            int beatDuration = _measurelen / nbBeatsPerMeasure;
            int beats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);

            string chordName; // = string.Empty;
            //int tickson;
            //int numerator = sequence1.Numerator;

            _gridbeatchords = new Dictionary<int, (string, int)>();

            #region check
            if (plLyrics == null || plLyrics.Count == 0)
                return _gridbeatchords;
            #endregion check

            // Initialize dictionary
            if (plLyrics[plLyrics.Count - 1].Beat > beats)
                beats = plLyrics[plLyrics.Count - 1].Beat;

            for (int i = 1; i <= beats; i++)
            {
                _gridbeatchords[i] = (ChordNotFound, 0);
            }

            for (int i = 0; i < plLyrics.Count; i++)
            {
                plLyric pll = plLyrics[i];

                chordName = pll.Element.Item1;
                if (chordName != string.Empty)
                {
                    //tickson = pll.TicksOn;
                    //measure = 1 + tickson / _measurelen;
                    //timeinmeasure = (int)GetTimeInMeasure(tickson);
                    beat = pll.Beat;

                    _gridbeatchords[beat] = (chordName, pll.TicksOn);
                }
            }

            return _gridbeatchords;
        }


        /// <summary>
        /// Use case: Display Lyrics & chords
        /// </summary>
        /// <returns></returns>
        public string GetLyricsLinesWithChords()
        {
            string tx = string.Empty;
            string chord;
            string lyric;
            string lineChords = string.Empty;
            string lineLyrics = string.Empty;
            string cr = Environment.NewLine;

            for (int i = 0; i < plLyrics.Count; i++)
            {
                plLyric pll = plLyrics[i];

                if (pll.CharType == plLyric.CharTypes.Text)
                {
                    chord = pll.Element.Item1;
                    lyric = pll.Element.Item2;

                    if (bHasChordsInLyrics)
                    {
                        lyric = Regex.Replace(lyric, RemoveChordPattern, @"");
                    }

                    if (chord.Length > lyric.Length)
                    {
                        lyric += new string(' ', chord.Length - lyric.Length);
                    }
                    else if (chord.Length < lyric.Length)
                    {
                        chord += new string(' ', lyric.Length - chord.Length);
                    }

                    lineChords += chord;
                    lineLyrics += lyric;
                }
                else if (pll.CharType == plLyric.CharTypes.LineFeed)
                {
                    // New line
                    if (lineChords.Trim() != string.Empty)
                        tx += lineChords + cr;
                    if (lineLyrics.Trim() != string.Empty)
                        tx += lineLyrics + cr;

                    //tx += lineChords + cr + lineLyrics + cr;
                    lineChords = string.Empty;
                    lineLyrics = string.Empty;
                }
                else if (pll.CharType == plLyric.CharTypes.ParagraphSep)
                {
                    // Paragraph
                    if (lineChords.Trim() != string.Empty)
                        tx += lineChords + cr;
                    if (lineLyrics.Trim() != string.Empty)
                        tx += lineLyrics + cr;

                    //tx += lineChords + cr + lineLyrics + cr + cr;
                    tx += cr;
                    lineChords = string.Empty;
                    lineLyrics = string.Empty;
                }

            }

            if (lineChords.Length > 0 || lineLyrics.Length > 0)
            {
                //tx += lineChords + cr + lineLyrics;
                if (lineChords.Trim() != string.Empty)
                    tx += lineChords + cr;
                if (lineLyrics.Trim() != string.Empty)
                    tx += lineLyrics;

            }

            return tx;
        }



        #endregion Display text of lyrics & chords


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
                //int beatDuration = _measurelen / nbBeatsPerMeasure;
                //_nbBeats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);
                _nbBeats = _nbMeasures * nbBeatsPerMeasure;
            }
        }     

        #endregion midi mesures
    }
}
