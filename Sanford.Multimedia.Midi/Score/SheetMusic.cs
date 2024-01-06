/*
 * Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi.Resources;

namespace Sanford.Multimedia.Midi.Score
{
    public class SheetMusic : Control
    {

        #region Create Delegate Reference
        // Default velocity changed
        public delegate void CurrentDefaultVelocityChangedEventHandler(int velocity);
        public event CurrentDefaultVelocityChangedEventHandler CurrentDefaultVelocityChanged;

        //Event Selection changed (SelectedNotes changed)
        public delegate void SelectionChangedEventHandler(List<MidiNote> lstMidiNotes);
        public event SelectionChangedEventHandler SelectionChanged;

        // Event: current note was changed
        public delegate void CurrentNoteChangedEventHandler(MidiNote n);
        public event CurrentNoteChangedEventHandler CurrentNoteChanged;

        public delegate void CurrentTrackChangedEventHandler(int tracknum);
        public event CurrentTrackChangedEventHandler CurrentTrackChanged;

        // Event: midi file was modified
        public delegate void FileModifiedEventHandler(object sender);
        public event FileModifiedEventHandler FileModified;

        public delegate void smMouseDownEventHandler(object sender, MouseEventArgs e, int staffnum, int note, float ticks);
        public event smMouseDownEventHandler OnSMMouseDown;

        public delegate void smMouseUpEventHandler(object sender, EventArgs e, int staffnum, int note, float ticks);
        public event smMouseUpEventHandler OnSMMouseUp;

        public delegate void smMouseDoubleClickEventHandler(object sender, EventArgs e, int staffnum, float ticks);
        public event smMouseDoubleClickEventHandler OnSMMouseDoubleClick;


        public delegate void smMouseMoveEventHandler(object sender, EventArgs e);
        public event smMouseMoveEventHandler OnSMMouseMove;

        // Event: display PianoRoll required
        public delegate void mnuPianoRollClickEventHandler(object sender, EventArgs e, int staffnum);
        public event mnuPianoRollClickEventHandler MnuPianoRollClick;

        // Width of sheetmusic has changed
        public delegate void WidthChangedEventHandler(int Width);
        public event WidthChangedEventHandler WidthChanged;


        #endregion Create Delegate Reference         

        #region Reglages
        private class _reglages
        {
            public int volume = 100;
            public int pan = 64;
            public int reverb = 0;
            public int channel = 0;
            public bool muted = false;
            public bool maximized = true;
        }
        private List<_reglages> lstTrkReglages;
        private _reglages TrkReglages;
        #endregion

        public class CurrNote
        {
            public int numstaff;
            public int lastnote;
            public MidiNote midinote = new MidiNote(0, 0, 0, 0, 0, false);
        };

        #region properties

        private int _selectedstaff = -1;
        /// <summary>
        /// Gets or sets current selected staff or track
        /// </summary>
        public int SelectedStaff
        {
            get { return _selectedstaff; }
            set {

                if (value != _selectedstaff)
                {
                    if (staffs != null && staffs.Count > 0 && value >= 0 && value < staffs.Count)
                    {
                        _selectedstaff = value;                        
                        CurrentTrackChanged?.Invoke(_selectedstaff);
                        MidiNote n = sequence1.tracks[_selectedstaff].GetFirstNote();
                        if (n != null)
                        {
                            // ResetSelection = false to be able to paste to another track
                            UpdateCurrentNote(_selectedstaff, n.Number, n.StartTime, false);                           

                            this.Invalidate();
                        }
                    }
                }
            }
        }

        private int _staffhminimized = 25;
        public int StaffHMinimized
        {
            get { return _staffhminimized; }
            set { _staffhminimized = value; }
        }

        private int _staffhmaximized = 150;
        public int StaffHMaximized
        {
            get { return _staffhmaximized; }
            set { _staffhmaximized = value; }
        }

        // Offset of Horizontal scrollbar
        private int offsetx = 0;
        public int OffsetX
        {
            get { return offsetx; }
            set {
                if (value != offsetx) {
                    offsetx = value;
                    Invalidate();
                }
            }
        }

        private int maxstaffwidth;
        public int MaxStaffWidth {
            get { return maxstaffwidth; }
            
        }

        private int x_shade = 0;
        public int X_shade
        {
            get
            {
                return x_shade;
            }
        }

        private bool _bvisible = false;
        public bool BVisible
        {
            get { return _bvisible; }
            set { _bvisible = value; }
        }

        private int _velocity = 100;
        public int Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public int DefaultVelocity
        {
            set
            {
                _velocity = value;
                CurrentDefaultVelocityChanged?.Invoke(_velocity);
            }
        }

        private bool _beditmode = false;
        public bool bEditMode
        {
            get { return _beditmode; }
            set { _beditmode = value;

                // Show/Hide form toolbox notes edition
                if (!_beditmode)
                    CloseFrmNoteEdit();
                else if (_beditmode && !_benternotes)
                    ShowFrmNoteEdit();
            }
        }

        private bool _benternotes = false;
        public bool bEnterNotes
        {
            get { return _benternotes; }
            set { _benternotes = value;

                // Show/Hide form toolbox notes edition
                if (_benternotes)
                    CloseFrmNoteEdit();
                else if (_beditmode && !_benternotes)
                    ShowFrmNoteEdit();
            }
        }

        // Selected Notes
        private List<MidiNote> _selnotes;
        public List<MidiNote> SelectedNotes
        {
            get { return _selnotes; }
        }


        // List of chords for each track
        private List<ChordSymbol>[] _lstchords;
        public List<ChordSymbol>[] lstChords { 
            get { return _lstchords; }
         }

        #endregion

        #region private dec

        private List<Staff> staffs; /** The array of staffs to display (from top to bottom) */
        private KeySignature mainkey; /** The main key signature */
        private ClefMeasures clefs;

        private int numtracks;     /** The number of tracks */

        private float zoom;          /** The zoom level to draw at (1.0 == 100%) */
        private bool scrollVert = false;    /** Whether to scroll vertically or horizontally */

        private int showNoteLetters;    /** Display the note letters */
        private Color[] NoteColors;     /** The note colors to use */
        private SolidBrush shadeBrush;  /** The brush for shading */
        private SolidBrush shade2Brush; /** The brush for shading left-hand piano */
        private Pen pen;                /** The black pen for drawing */
        
        private int[] AllNotes;
        private List<int> AllTracks;       

        private Sequence sequence1;
        private int seqlength;          // Length of sequence
        private int measurelen;
        private int nbMeasures;

        private int xbox;
        private int ybox;
        private int yboxnote;

        private Point aPos;                                         // Notes selection
        private Rectangle selRect = new Rectangle(0, 0, 0, 0);      // Notes selection
       
        private bool bReadyToPaste = false;
        private ContextMenuStrip smContextMenu;

        private int selectedX = 0;
        private int selectedY = 0;

        
        

        private Sanford.Multimedia.Midi.MidiOptions options;
        private int transpose = 0;


        #region controls
        Sanford.Multimedia.Midi.Score.UI.frmNoteEdit FrmNoteEdit;
        #endregion

        #endregion private dec


        #region public dec

        public const int TitleHeight = 14; /** The height for the title on the first page */

        public const int LineWidth = 1;    /** The width of a line */
        public const int LeftMargin = 4;   /** The left margin */
        


        public static int LineSpace;        /** The space between lines in the staff */
        public static int StaffHeight;      /** The height between the 5 horizontal lines of the staff */
        public static int NoteHeight;      /** The height of a whole note */
        public static int NoteWidth;       /** The width of a whole note */
        public int StaffMaxHeight;         /** FAB: hauteur max pour toutes les portées */

        public const int PageWidth = 800;    /** The width of each page */
        public const int PageHeight = 1050;  /** The height of each page (when printing) */

        public static Font LetterFont;       /** The font for drawing the letters */

        private bool bplaying = false;
        public bool BPlaying
        {
            get { return bplaying; }
            set
            {
                bplaying = value;
                if (bplaying)
                {
                    ClearSelectedNotes();
                }
            }
        }

        public bool bShowHelpGrid = false;

       
        public bool bSelectingNote = false;        

        public CurrNote CurrentNote;         

        public bool ScrollVert
        {
            get { return scrollVert; }
            set { scrollVert = value; }
        }

        #endregion public dec


        /** Initialize the default note sizes.  */
        static SheetMusic()
        {
            SetNoteSize(false);
        }


        public SheetMusic(Sequence seq, MidiOptions Options, int staffHeight)
        {

            this.MouseClick += new MouseEventHandler(SheetMusic_MouseClick);
            this.MouseDoubleClick += new MouseEventHandler(SheetMusic_MouseDoubleClick);
            this.MouseDown += new MouseEventHandler(SheetMusic_MouseDown);
            this.MouseUp += new MouseEventHandler(SheetMusic_MouseUp);                    
            this.MouseMove += new MouseEventHandler(SheetMusic_MouseMove);

            this.KeyDown += new KeyEventHandler(SheetMusic_KeyDown);

           

            this.SetStyle(
                  System.Windows.Forms.ControlStyles.UserPaint |
                  System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                  System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                  true);

            

            sequence1 = seq;
            options = Options;
            _bvisible = options.bVisible;

            Init(sequence1, options, staffHeight);
        }


        #region init    

        /// <summary>
        /// Calculate number of tracks
        /// If scrolling horizontal : all tracks 
        /// If Scrolling vertical : only those contaioning notes
        /// </summary>
        private void GetAllTracks()
        {
            numtracks = 0;
            AllTracks = new List<int>();


            if (ScrollVert == false)
            {
                // Horizontal scroll
                numtracks = sequence1.tracks.Count;
                for (int i = 0; i < sequence1.tracks.Count; i++)
                {
                    AllTracks.Add(i);
                }
            }
            else
            {
                // Vertical Scroll => remove empty tracks
                for (int i = 0; i < sequence1.tracks.Count; i++)
                {
                    if (sequence1.tracks[i].Notes.Count > 0)
                    {
                        numtracks++;
                        AllTracks.Add(i);
                    }
                }
            }
        }

        private void Init(Sequence sequence1, MidiOptions options, int staffHeight)
        {            
            // Tableau des notes de 0 à 127
            InitAllNotes();

            // Réglages tracks
            InitTracksStuff();

            zoom = 1.0f;
            _staffhmaximized = staffHeight;

            SetColors(options.colors, options.shadeColor, options.shade2Color);
            pen = new Pen(Color.Black, 1);

            // Calculate tracks to be kept according to scrolling vertical or horizontal
            scrollVert = options.scrollVert;
            GetAllTracks();

            // FAB : à corriger
            List<Track> tracks = ChangeMidiNotes(sequence1, sequence1.Time, options);
            
            SetNoteSize(options.largeNoteSize);
            
            this.showNoteLetters = options.showNoteLetters;

            // FAB : à corriger
            TimeSignature time = sequence1.Time;                     

            if (options.time != null)
            {
                time = options.time;
            }
            
            if (options.key == -1)
            {
                mainkey = GetKeySignature(sequence1);
            }
            else
            {
                mainkey = new KeySignature(options.key);
            }


            // FAB : à corriger
            //int lastStart = file.EndTime() + options.shifttime;
            if (time != null)
            {
                seqlength = sequence1.GetLength();
                measurelen = time.Measure;
                nbMeasures = 1 + seqlength / measurelen;
            }

            int lastStart = seqlength;

            /* Create all the music symbols (notes, rests, vertical bars, and
             * clef changes).  The symbols variable contains a list of music 
             * symbols for each track.  The list does not include the left-side 
             * Clef and key signature symbols.  Those can only be calculated 
             * when we create the staffs.
             */
            List<MusicSymbol>[] symbols = new List<MusicSymbol>[numtracks];
            _lstchords = new List<ChordSymbol>[numtracks];

            for (int tracknum = 0; tracknum < AllTracks.Count; tracknum++)
            {
                Track track = sequence1.tracks[AllTracks[tracknum]];
                
                // Clef for tracks
                clefs = new ClefMeasures(track.Notes, time.Measure, track.Clef);
             
                // FAB : à corriger
                List<ChordSymbol> chords = CreateChords(track.Notes, mainkey, time, clefs, track.Clef);
                _lstchords[tracknum] = chords;
                
                symbols[tracknum] = CreateSymbols(chords, clefs, time, lastStart, track.Clef);                
            }

            List<LyricSymbol>[] lyrics = null;
            if (options.showLyrics)
            {
                lyrics = GetLyrics(tracks);
            }

            /* Vertically align the music symbols */

            SymbolWidths widths = new SymbolWidths(symbols, lyrics, measurelen);
            AlignSymbols(symbols, widths, options, measurelen);           

            // FAB : à corriger
            if (time != null)
            {
                staffs = CreateStaffs(symbols, mainkey, options, time.Measure);

                // FAB : à corriger
                CreateAllBeamedChords(symbols, time);
            }

            if (lyrics != null && staffs != null)
            {
                AddLyricsToStaffs(staffs, lyrics);
            }

            /* After making chord pairs, the stem directions can change,
             * which affects the staff height.  Re-calculate the staff height.
             */

            StaffMaxHeight = 0;
            BackColor = Color.White;


            if (staffs != null)
            {
                SetZoom(zoom);
            }


            // Note courante
            CurrentNote = new CurrNote();
            UpdateCurrentNote(0, 0, 0, true);

            if (staffs != null && staffs.Count > 0)
            {                
                // Width of sheetmusic control
                maxstaffwidth = staffs[0].Width;
            }

            // Staff maximized/minimized
            for (int tracknum = 0; tracknum < AllTracks.Count; tracknum++)
            {
                staffs[tracknum].Maximized = lstTrkReglages[tracknum].maximized;
                if (lstTrkReglages[tracknum].maximized)                
                    staffs[tracknum].Height = _staffhmaximized;                                    
                else
                    staffs[tracknum].Height = _staffhminimized;
            }

        }


        /// <summary>
        /// Reset all things related to tracks when the number of tracks evolve
        /// </summary>
        private void InitTracksStuff()
        {
            lstTrkReglages = new List<_reglages>();
            int nbTrk = sequence1.tracks.Count;

            for (int i = 0; i < nbTrk; i++)
            {
                Track track = sequence1.tracks[i];
                TrkReglages = new _reglages();
                TrkReglages.maximized = true;
                TrkReglages.volume = track.Volume;
                TrkReglages.pan = track.Pan;
                TrkReglages.reverb = track.Reverb;
                TrkReglages.muted = false;
                TrkReglages.channel = track.MidiChannel;
                lstTrkReglages.Add(TrkReglages);
            }          
        }

        public void Redraw()
        {
            this.Invalidate();
        }

        /// <summary>
        /// Refresh SheetMusic display
        /// </summary>
        public override void Refresh()
        {
            //options.transpose = 0;

            TimeSignature time = sequence1.Time;


            // Calculate tracks to be kept according to scrolling vertical or horizontal
            GetAllTracks();

            List<Track> tracks = ChangeMidiNotes(sequence1, sequence1.Time, options);
            SetNoteSize(options.largeNoteSize);


            if (options.time != null)
            {
                time = options.time;
            }

            if (options.key == -1)
            {
                mainkey = GetKeySignature(sequence1);
            }
            else
            {
                mainkey = new KeySignature(options.key);
            }
    

            if (time != null)
            {
                seqlength = sequence1.GetLength();
                measurelen = time.Measure;
                nbMeasures = 1 + seqlength / measurelen;
            }
            int lastStart = seqlength;

            /* Create all the music symbols (notes, rests, vertical bars, and
              * clef changes).  The symbols variable contains a list of music 
              * symbols for each track.  The list does not include the left-side 
              * Clef and key signature symbols.  Those can only be calculated 
              * when we create the staffs.
              */
            List<MusicSymbol>[] symbols = new List<MusicSymbol>[numtracks];

            for (int tracknum = 0; tracknum < AllTracks.Count; tracknum++)
            {
                Track track = sequence1.tracks[AllTracks[tracknum]];
                
                // Réglages maximized/minimized
                lstTrkReglages[tracknum].maximized = track.Maximized;

                // List of clef by measures
                clefs = new ClefMeasures(track.Notes, time.Measure, track.Clef);

                // FAB : à corriger
                List<ChordSymbol> chords = CreateChords(track.Notes, mainkey, time, clefs, track.Clef);
                symbols[tracknum] = CreateSymbols(chords, clefs, time, lastStart, track.Clef);

            }

            List<LyricSymbol>[] lyrics = null;
            if (options.showLyrics)
            {
                lyrics = GetLyrics(tracks);
            }

            /* Vertically align the music symbols */
            //int measurelen = 0;
            //measurelen = time.Measure;
            //nbMeasures = 1 + seqlength / measurelen;

            SymbolWidths widths = new SymbolWidths(symbols, lyrics, measurelen);
            AlignSymbols(symbols, widths, options, measurelen);

            // FAB : à corriger
            if (time != null)
            {
                staffs = CreateStaffs(symbols, mainkey, options, time.Measure);

                // FAB : à corriger
                CreateAllBeamedChords(symbols, time);

                
                // Height of staffs maximized or minimized
                for (int tracknum = 0; tracknum < AllTracks.Count; tracknum++) 
                {
                    staffs[tracknum].Maximized = lstTrkReglages[tracknum].maximized;

                    if (lstTrkReglages[tracknum].maximized)
                        staffs[tracknum].Height = _staffhmaximized;
                    else
                        staffs[tracknum].Height = _staffhminimized;

                }
            }

            if (lyrics != null && staffs != null)
            {
                AddLyricsToStaffs(staffs, lyrics);
            }

            /* After making chord pairs, the stem directions can change,
             * which affects the staff height.  Re-calculate the staff height.
             */

            StaffMaxHeight = 0;
            BackColor = Color.White;

            if (staffs != null && staffs.Count > 0)
            {
                SetZoom(zoom);

                // Width of sheetmusic control                
                maxstaffwidth = staffs[0].Width;
            }

            Parent.Focus();
            this.Invalidate();
        }


        /** Apply the given sheet music options to the MidiNotes.
          *  Return the midi tracks with the changes applied.
          */
        public List<Track> ChangeMidiNotes(Sequence sequence1, TimeSignature timesig , MidiOptions options)
        {
            List<Track> newtracks = new List<Track>();

            int numtrack = 0;

            for (int track = 0; track < AllTracks.Count; track++)
            {
                numtrack = AllTracks[track];
                options.tracks[numtrack] = true;

                if (options.tracks[numtrack])
                {
                    newtracks.Add(sequence1.tracks[numtrack].Clone());

                }
            }

            /* To make the sheet music look nicer, we round the start times
             * so that notes close together appear as a single chord.  We
             * also extend the note durations, so that we have longer notes
             * and fewer rest symbols.
             */
            TimeSignature time = timesig;
            if (options.time != null)
            {
                time = options.time;
         
                RoundStartTimes(newtracks, options.combineInterval, timesig, options.scrollVert);
                RoundDurations(newtracks, time.Quarter);
            }

            /*
            if (options.twoStaffs)
            {
                newtracks = MidiFile.CombineToTwoTracks(newtracks, timesig.Measure);
            }
            
            
            if (options.shifttime != 0)
            {
                MidiFile.ShiftTime(newtracks, options.shifttime);
            }
            
            */
            if (options.transpose != transpose)
            {
                

                Transpose(newtracks, options.transpose);
                Transpose(sequence1.tracks, options.transpose);

                // reset options
                transpose = options.transpose;

                // Raise event
                FileModified?.Invoke(this);
            }
            
            return newtracks;
        }

        #endregion init


        #region transpose

        /** Shift the note keys up/down by the given amount */
        public static void Transpose(List<Track> tracks, int amount)
        {
            int newNumber = 0;

            foreach (Track track in tracks)
            {

                if (track.MidiChannel != 9)
                {
                    List<MidiNote> L = new List<MidiNote>();
                    L = track.Notes;

                    for (int i = 0; i < L.Count; i++)
                    {

                        MidiNote note = L[i];
                        newNumber = note.Number + amount;

                        if (newNumber < 0)
                        {
                            newNumber = 0;
                        }

                        MidiNote n = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, note.Selected);
                        n = note;

                        // Change note value without checking if note can be created according to previous note and note after
                        track.changeNoteNumber(n, newNumber, false);

                    }
                }
        
            }
            
        }

        #endregion transpose


        #region durations

        /** In Midi Files, time is measured in pulses.  Notes that have
         * pulse times that are close together (like within 10 pulses)
         * will sound like they're the same chord.  We want to draw
         * these notes as a single chord, it makes the sheet music much
         * easier to read.  We don't want to draw notes that are close
         * together as two separate chords.
         *
         * The SymbolSpacing class only aligns notes that have exactly the same
         * start times.  Notes with slightly different start times will
         * appear in separate vertical columns.  This isn't what we want.
         * We want to align notes with approximately the same start times.
         * So, this function is used to assign the same starttime for notes
         * that are close together (timewise).
         */
        public static void RoundStartTimes(List<Track> tracks, int millisec, TimeSignature time, bool scrollvert)
        {
            /* Get all the starttimes in all tracks, in sorted order */
            List<int> starttimes = new List<int>();
            foreach (Track track in tracks)
            {
                foreach (MidiNote note in track.Notes)
                {
                    starttimes.Add(note.StartTime);
                }
            }
            starttimes.Sort();

            /* Notes within "millisec" milliseconds apart will be combined. */
            int interval = time.Quarter * millisec * 1000 / time.Tempo;

            /* If two starttimes are within interval millisec, make them the same */
            for (int i = 0; i < starttimes.Count - 1; i++)
            {
                if (starttimes[i + 1] - starttimes[i] <= interval)
                {
                    starttimes[i + 1] = starttimes[i];
                }
            }

            CheckStartTimes(tracks);

            /* Adjust the note starttimes, so that it matches one of the starttimes values */
            foreach (Track track in tracks)
            {
                int i = 0;

                if (track.Notes.Count > 0)
                {

                    foreach (MidiNote note in track.Notes)
                    {
                        while (i < starttimes.Count &&
                               note.StartTime - interval > starttimes[i])
                        {
                            i++;
                        }

                        if (note.StartTime > starttimes[i] &&
                            note.StartTime - starttimes[i] <= interval)
                        {

                            note.StartTime = starttimes[i];
                        }
                    }
                    track.Notes.Sort(track.Notes[0]);
                }

            }

            /* FAB Align to perfect values */
            
            //if (scrollvert == false)
            //{
                #region align starttime

                int t = 0;
                int j = 0;
                int ins = -1;
                
                int measurelen = time.Measure;
                int nummeasure = 0;
                int triplecroche = measurelen / 32;
                List<int> tpcList = new List<int>();

                for (j = 0; j < 32; j++)
                {
                    t = j * triplecroche;
                    tpcList.Add(t);
                }

                foreach (Track track in tracks)
                {
                    if (track.Notes.Count > 0)
                    {
                        foreach (MidiNote note in track.Notes)
                        {
                            t = note.StartTime;
                            nummeasure = t / measurelen;
                            int zerot = t - nummeasure * measurelen;

                            ins = tpcList.FindIndex(s => s >= zerot);
                            if (ins == -1)
                            {
                                // not found
                                //realstart = 0;
                            }
                            else if (ins > 0 && tpcList[ins] > zerot)
                            {
                                if (tpcList[ins] - zerot < zerot - tpcList[ins - 1])
                                {
                                    zerot = tpcList[ins];
                                    note.StartTime = zerot + nummeasure * measurelen;
                                }
                                else
                                {
                                    zerot = tpcList[ins - 1];
                                    note.StartTime = zerot + nummeasure * measurelen;
                                }

                            }


                        }
                    }
                }

            #endregion align starttime
            //}

        }


        /** We want note durations to span up to the next note in general.
         * The sheet music looks nicer that way.  In contrast, sheet music
         * with lots of 16th/32nd notes separated by small rests doesn't
         * look as nice.  Having nice looking sheet music is more important
         * than faithfully representing the Midi File data.
         *
         * Therefore, this function rounds the duration of MidiNotes up to
         * the next note where possible.
         */
        public static void RoundDurations(List<Track> tracks, int quarternote)
        {

            foreach (Track track in tracks)
            {
                MidiNote prevNote = null;
                for (int i = 0; i < track.Notes.Count - 1; i++)
                {
                    MidiNote note1 = track.Notes[i];
                    if (prevNote == null)
                    {
                        prevNote = note1;
                    }

                    /* Get the next note that has a different start time */
                    MidiNote note2 = note1;
                    for (int j = i + 1; j < track.Notes.Count; j++)
                    {
                        note2 = track.Notes[j];
                        if (note1.StartTime < note2.StartTime)
                        {
                            break;
                        }
                    }
                    int maxduration = note2.StartTime - note1.StartTime;

                    int dur = 0;
                    if (quarternote <= maxduration)
                        dur = quarternote;
                    else if (quarternote / 2 <= maxduration)
                        dur = quarternote / 2;
                    else if (quarternote / 3 <= maxduration)
                        dur = quarternote / 3;
                    else if (quarternote / 4 <= maxduration)
                        dur = quarternote / 4;


                    if (dur < note1.Duration)
                    {
                        dur = note1.Duration;
                    }

                    /* Special case: If the previous note's duration
                     * matches this note's duration, we can make a notepair.
                     * So don't expand the duration in that case.
                     */
                    if ((prevNote.StartTime + prevNote.Duration == note1.StartTime) &&
                        (prevNote.Duration == note1.Duration))
                    {

                        dur = note1.Duration;
                    }
                    note1.Duration = dur;
                    if (track.Notes[i + 1].StartTime != note1.StartTime)
                    {
                        prevNote = note1;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize le tableau des valeurs de notes
        /// Retourne la note en fonction de la coordonnée de la souris
        /// </summary>
        private void InitAllNotes()
        {
            //0,   2,  4,  5,  7,  9, 11
            //12, 14, 16, 17, 19, 21, 23
            //24
            // 36
            // 48
            //int noteC1 = 60; // Do du bas (60, 62, 64, 65, 67, 69, 71)
            //int noteC2 = 72; // aigus
            //int noteD2 = 74;
            //int noteE2 = 76; // Mi du haut : position = 33
            //int noteF2 = 77;
            //int noteG2 = 79;
            //int noteA2 = 81;
            //int noteB2 = 83;
            //84;
            //96;
            //108
            //120
            //132            

            int n = -2;
            int idx = 0;
            AllNotes = new int[75];

            for (int i = 0; i < 75; i++)
            {
                idx++;

                if (idx == 4)
                {
                    n++; // 1/2 ton de mi à fa

                }
                else if (idx == 8)
                {
                    n++;
                    idx = 1; // 1/2 ton de si à do
                }
                else
                {
                    n = n + 2;
                }
                AllNotes[i] = n;
            }

        }

        /** Check that the MidiNote start times are in increasing order.
           * This is for debugging purposes.
           */
        private static void CheckStartTimes(List<Track> tracks)
        {
            foreach (Track track in tracks)
            {
                int prevtime = -1;
                foreach (MidiNote note in track.Notes)
                {
                    if (note.StartTime < prevtime)
                    {
                        //throw new System.ArgumentException("start times not in increasing order");
                        Console.Write("\nERROR: start times not in increasing order (SheetMusic.cs CheckStartTimes");
                    }
                    prevtime = note.StartTime;
                }
            }
        }

        /** Get the best key signature given the midi notes in all the tracks. */
        private KeySignature GetKeySignature(Sequence sequence1)
        {
            List<int> notenums = new List<int>();
            

            foreach (Track track in sequence1)
            {
                foreach (MidiNote note in track.Notes)
                {
                    notenums.Add(note.Number);
                }                
            }
            return KeySignature.Guess(notenums);
        }

        /** Given MusicSymbols for a track, create the staffs for that track.
         *  Each Staff has a maxmimum width of PageWidth (800 pixels).
         *  Also, measures should not span multiple Staffs.
         */
        private List<Staff> CreateStaffsForTrack(List<MusicSymbol> symbols, int measurelen, KeySignature key, MidiOptions options,  int track, int totaltracks)
        {
            int keysigWidth = KeySignatureWidth(key);
            int startindex = 0;
            List<Staff> thestaffs = new List<Staff>(symbols.Count / 50);

            while (startindex < symbols.Count)
            {
                /* startindex is the index of the first symbol in the staff.
                 * endindex is the index of the last symbol in the staff.
                 */
                int endindex = startindex;
                int width = keysigWidth;
                int maxwidth;

                /* If we're scrolling vertically, the maximum width is PageWidth. */
                if (scrollVert)
                {
                    maxwidth = SheetMusic.PageWidth;
                }
                else
                {
                    //maxwidth = 2000000;
                    maxwidth = 4000000;
                }

                while (endindex < symbols.Count &&
                       width + symbols[endindex].Width < maxwidth)
                {

                    width += symbols[endindex].Width;
                    endindex++;
                }
                endindex--;

                /* There's 3 possibilities at this point:
                 * 1. We have all the symbols in the track.
                 *    The endindex stays the same.
                 *
                 * 2. We have symbols for less than one measure.
                 *    The endindex stays the same.
                 *
                 * 3. We have symbols for 1 or more measures.
                 *    Since measures cannot span multiple staffs, we must
                 *    make sure endindex does not occur in the middle of a
                 *    measure.  We count backwards until we come to the end
                 *    of a measure.
                 */

                if (endindex == symbols.Count - 1)
                {
                    /* endindex stays the same */
                }
                else if (symbols[startindex].StartTime / measurelen ==
                         symbols[endindex].StartTime / measurelen)
                {
                    /* endindex stays the same */
                }
                else
                {
                    int endmeasure = symbols[endindex + 1].StartTime / measurelen;
                    while (symbols[endindex].StartTime / measurelen ==
                           endmeasure)
                    {
                        endindex--;
                    }
                }
                int range = endindex + 1 - startindex;
                if (scrollVert)
                {
                    width = SheetMusic.PageWidth;
                }

                // Tenir compte de Maximized/minimized pour track
                Staff staff = new Staff(symbols.GetRange(startindex, range), key, options, _staffhmaximized ,track, totaltracks);
                //Staff staff = new Staff(symbols.GetRange(startindex, range), key, options, _staffhminimized, track, totaltracks);
                thestaffs.Add(staff);
                startindex = endindex + 1;
            }
            return thestaffs;
        }

        /** Given all the MusicSymbols for every track, create the staffs
         * for the sheet music.  There are two parts to this:
         *
         * - Get the list of staffs for each track.
         *   The staffs will be stored in trackstaffs as:
         *
         *   trackstaffs[0] = { Staff0, Staff1, Staff2, ... } for track 0
         *   trackstaffs[1] = { Staff0, Staff1, Staff2, ... } for track 1
         *   trackstaffs[2] = { Staff0, Staff1, Staff2, ... } for track 2
         *
         * - Store the Staffs in the staffs list, but interleave the
         *   tracks as follows:
         *
         *   staffs = { Staff0 for track 0, Staff0 for track1, Staff0 for track2,
         *              Staff1 for track 0, Staff1 for track1, Staff1 for track2,
         *              Staff2 for track 0, Staff2 for track1, Staff2 for track2,
         *              ... } 
         */
        private List<Staff> CreateStaffs(List<MusicSymbol>[] allsymbols, KeySignature key, MidiOptions options, int measurelen)
        {

            List<Staff>[] trackstaffs = new List<Staff>[allsymbols.Length];
            int totaltracks = trackstaffs.Length;

            for (int track = 0; track < totaltracks; track++)
            {
                List<MusicSymbol> symbols = allsymbols[track];
                trackstaffs[track] = CreateStaffsForTrack(symbols, measurelen, key, options, track, totaltracks);
            }

            /* Update the EndTime of each Staff. EndTime is used for playback */
            foreach (List<Staff> list in trackstaffs)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    list[i].EndTime = list[i + 1].StartTime;
                }
            }

            /* Interleave the staffs of each track into the result array. */
            int maxstaffs = 0;
            for (int i = 0; i < trackstaffs.Length; i++)
            {
                if (maxstaffs < trackstaffs[i].Count)
                {
                    maxstaffs = trackstaffs[i].Count;
                }
            }
            List<Staff> result = new List<Staff>(maxstaffs * trackstaffs.Length);
            for (int i = 0; i < maxstaffs; i++)
            {
                foreach (List<Staff> list in trackstaffs)
                {
                    if (i < list.Count)
                    {
                        result.Add(list[i]);
                    }
                }
            }
            return result;
        }

        /** Set the size of the notes, large or small.  Smaller notes means
         * more notes per staff.
         */
        public static void SetNoteSize(bool largenotes)
        {
            if (largenotes)
                LineSpace = 7;
            else
                LineSpace = 5;

            StaffHeight = LineSpace * 4 + LineWidth * 5;
            NoteHeight = LineSpace + LineWidth;
            NoteWidth = 3 * LineSpace / 2;
            LetterFont = new Font("Arial", 8, FontStyle.Regular);
        }

        #endregion durations


        #region events
    
        /// <summary>
        /// Keydown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SheetMusic_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
              
                case Keys.Delete:
                    {
                        List<MidiNote> L = new List<MidiNote>();
                        int i = 0;
                        foreach (Staff stf in staffs) {
                            Track track = sequence1.tracks[i];
                            L = stf.getSelectedNotes();
                            if (L.Count > 0)
                            {
                                foreach (MidiNote n in L)
                                {
                                    track.deleteNote(n.Number, n.StartTime);
                                }
                            }
                            i++;
                        }
                        selRect = new Rectangle();

                        this.Refresh();

                        // Raise event
                        FileModified?.Invoke(this);
                        WidthChanged?.Invoke(maxstaffwidth);

                        break;
                    }
            }
        }

        /// <summary>
        /// Add or remove half tone to a single note
        /// </summary>
        /// <param name="KeyCode"></param>
        public void SheetMusic_UpDownCurrentNote(string KeyCode)
        {
            // alter note
            int numstaff = CurrentNote.numstaff;
            if (numstaff >= sequence1.tracks.Count)
                return;

            Track track = sequence1.tracks[numstaff];

            int note = CurrentNote.midinote.Number;
            int newnote = note;

            float ticks = CurrentNote.midinote.StartTime;
            float time = ticks / sequence1.Division;

            float duration = GetAdjustedNoteDuration(track, note, ticks) / (float)sequence1.Division;
            duration = AjustQuarterToGrid(duration);


            switch (KeyCode)
            {
                // increase note value, don't change ticks & duration
                case "Up":
                    newnote++;
                    break;

                // decrease note value, don't change ticks & duration
                case "Down":
                    newnote--;
                    break;
            }

            //delete old note
            track.deleteNote(note, Convert.ToInt32(ticks));

            // Create new one
            int starttime = Convert.ToInt32(time * sequence1.Division);
            int dur = Convert.ToInt32(duration * sequence1.Division);
            int channel = track.MidiChannel;
            int notenumber = newnote;

            int velocity = CurrentNote.midinote.Velocity;

            MidiNote mdnote = new MidiNote(starttime, channel, notenumber, dur, velocity, false);
            track.addNote(mdnote);

            // Update current note
            UpdateCurrentNote(CurrentNote.numstaff, notenumber, starttime, true);

        }
        

        /// <summary>
        /// Add or remove half tone to a list of selected notes
        /// </summary>
        /// <param name="KeyCode"></param>
        public void SheetMusic_UpDownSelectedNote(string KeyCode)
        {
            int notenumber = 0;
            int newnote = 0;
            int starttime = 0;
            int dur = 0;
            int channel = 0;

            if (_selectedstaff == -1)
                return;

            // alter note
            int numstaff = _selectedstaff;
            Track track = sequence1.tracks[numstaff];

            Staff staff = this.staffs[numstaff];
            List<MidiNote> L = new List<MidiNote>();
            L = staff.getSelectedNotes();

            for (int i = 0; i < L.Count; i++)
            {
                MidiNote mdnote = L[i];
                notenumber = mdnote.Number;
                newnote = notenumber;

                float ticks = mdnote.StartTime;
                float time = ticks / sequence1.Division;

                float duration = GetAdjustedNoteDuration(track, notenumber, ticks) / (float)sequence1.Division;
                duration = AjustQuarterToGrid(duration);


                switch (KeyCode)
                {
                    // increase note value, don't change ticks & duration
                    case "Up":
                        newnote++;
                        break;

                    // decrease note value, don't change ticks & duration
                    case "Down":
                        newnote--;
                        break;
                }

                //delete old note
                track.deleteNote(notenumber, Convert.ToInt32(ticks));

                // Create new one
                starttime = Convert.ToInt32(time * sequence1.Division);
                dur = Convert.ToInt32(duration * sequence1.Division);
                channel = track.MidiChannel;
                notenumber = newnote;

                int velocity = mdnote.Velocity;

                // Add new note with selected = true
                mdnote = new MidiNote(starttime, channel, notenumber, dur, velocity, true);
                track.addNote(mdnote);

                // Set current note to first note of the selection
                if (i == 0)
                {
                    // Update current note
                    UpdateCurrentNote(numstaff, notenumber, starttime, false);
                }
            }                       
        }


        private float AjustQuarterToGrid(float adjust)
        {
            if (adjust > 4)
                adjust = 6;
            else if (adjust > 3)
                adjust = 4;

            else if (adjust > 2)
                adjust = 3;
            else if (adjust > 1.5)
                adjust = 2;


            else if (adjust > 1.4)
                adjust = 1.5f;
            else if (adjust > 1.33)
                adjust = 1.333f;
            else if (adjust > 0.9)
                adjust = 1;


            else if (adjust > 0.7)
                adjust = 0.75f;
            else if (adjust > 0.66)
                adjust = 0.666666667f;
            else if (adjust > 0.4)
                adjust = 0.5f;       


            else if (adjust > 0.34)
                adjust = 0.375f;
            else if (adjust > 0.33)
                adjust = 0.333f;
            else if (adjust > 0.2)
                adjust = 0.25f;
         

            else if (adjust > 0.17)
                adjust = 0.1825f;
            else if (adjust > 0.166)
                adjust = 0.166666667f;
            else if (adjust > 0.1)
                adjust = 0.125f;

            else if (adjust > 0.09)
                adjust = 0.09375f;
            else if (adjust > 0.08)
                adjust = 0.0833f;
            else
                adjust = 0.0625f;

            return adjust;
        }

        private int AdjustDurationToGrid(int dur)
        {
            int noire = sequence1.Division;
            float duration = (float)dur;

            // Ronde pointée
            if (duration > 4 * noire)
                duration = 6 * noire;
            // ronde
            else if (duration > 3 * noire)
                duration = 4 * noire;

            // Blanche pointée
            else if (duration > 2 * noire)
                duration = 3 * noire;
            // Blanche
            else if (duration > 1.5 * noire)
                duration = 2 * noire;
            
            // Noire pointée
            else if (duration > 1.4f * noire)
                duration = 1.5f * noire;
            // ** Blanche triolet
            else if (duration > 1.33 * noire)
                duration = 1.333f * noire;
            // Noire
            else if (duration > 0.9 * noire)
                duration = noire;

            // Croche pointée
            else if (duration > 0.7 * noire)
                duration = 0.75f * noire;
            // ** Noire triolet
            else if (duration > 0.66 * noire)
                duration = 0.666666667f * noire;
            // Croche
            else if (duration > 0.4 * noire)
                duration = 0.5f * noire;
            

            // Double croche pointée
            else if (duration > 0.34 * noire)
                duration = 0.375f * noire;
            // ** Croche triolet
            else if (duration > 0.33 * noire)
                duration = 0.333f * noire;
            // Double croche
            else if (duration > 0.2 * noire)
                duration = 0.25f * noire;
            

            // Triple croche pointée
            else if (duration > 0.17 * noire)
                duration = 0.1825f * noire;
            // ** Double croche triolet
            else if (duration > 0.166 * noire)
                duration = 0.166666667f * noire;
            // Triple croche
            else if (duration > 0.1 * noire)
                duration = 0.125f * noire;


            // Quadruple croche pointée
            else if (duration > 0.09 * noire)
                duration = 0.09375f * noire;
            // ** Triple croche triolet
            else if (duration > 0.08 * noire)
                duration = 0.0833f * noire;
            // Quadruple croche
            else 
                duration = 0.0625f * noire;

            return Convert.ToInt32(duration);
        }

        /// <summary>
        /// Get adjusted note duration in pulses for a given note
        /// </summary>
        /// <param name="track"></param>
        /// <param name="note"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private int GetAdjustedNoteDuration(Track track, int note, float ticks)
        {
            int ret = 0;

            MidiNote item = track.findMidiNote(note, Convert.ToInt32(ticks));
            if (item != null)
            {
                ret = AdjustDurationToGrid(item.Duration);
            }

            return ret;
        }
  

        /** Draw the SheetMusic.
         * Scale the graphics by the current zoom factor.
         * Get the vertical start and end points of the clip area.
         * Only draw Staffs which lie inside the clip area.
         */
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle clip =
              new Rectangle((int)(OffsetX / zoom),
                (int)(e.ClipRectangle.Y / zoom),
                (int)(e.ClipRectangle.Width / zoom),
                (int)(e.ClipRectangle.Height / zoom));


            Graphics g = e.Graphics;
            g.ScaleTransform(zoom, zoom);           
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            int ypos = 0;
            foreach (Staff staff in staffs)
            {                
                if ((ypos + staff.Height < clip.Y) || (ypos > clip.Y + clip.Height))
                {
                    // Staff is not in the clip, don't need to draw it 
                }
                else
                {
                    // Draw only if maximized
                    if (staff.Maximized)
                    {
                        g.TranslateTransform(-clip.X, ypos);

                        // Dessine la portée  
                        staff.Draw(g, clip, selRect, pen);

                        g.TranslateTransform(clip.X, -ypos);
                    }
                }
                
                ypos += staff.Height;                                
            }           

            g.ScaleTransform(1.0f / zoom, 1.0f / zoom);

            #region Draw rectangle selection

            if (bSelectingNote == true)
            {
                // Draw the current rectangle
                // aPos is the starting point
                Point pos = PointToClient(Control.MousePosition);

                // Must stay in the first staff selected
                int numstaff = GetStaffClicked(pos.Y);

                if (numstaff != _selectedstaff)                                                        
                    pos.Y = _staffhmaximized + _selectedstaff * _staffhmaximized;
                

                using (Pen pen = new Pen(Brushes.Black))
                {
                    pen.DashStyle = DashStyle.Dot;
                    g.DrawLine(pen, aPos.X, aPos.Y, pos.X, aPos.Y);
                    g.DrawLine(pen, pos.X, aPos.Y, pos.X, pos.Y);
                    g.DrawLine(pen, pos.X, pos.Y, aPos.X, pos.Y);
                    g.DrawLine(pen, aPos.X, pos.Y, aPos.X, aPos.Y);
                }
            }
            #endregion selection
        }

        #endregion events


        #region timeline

        private void Func1() {
            // choix du remplissage à blanc
            // noire 4, croche 8, doublecroche 16, triplecroche 32
            int i = 0;
            int t = 0;

            int intervall = 32;
            int duration = measurelen / intervall;
            List<int> tpcList = new List<int>();
            for (i = 0; i < intervall; i++)
            {
                t = i * duration;
                tpcList.Add(t);
            }
        }

        #endregion timeline


        #region edit score

        #region Mouse click, down, up

        /// <summary>
        /// Event: click on the staff => delete or create notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetMusic_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int X = e.Location.X;
                int Y = e.Location.Y;
                X = Convert.ToInt32(X / zoom);
                Y = Convert.ToInt32(Y / zoom);

                Point pos = PointToClient(Control.MousePosition);
                X = pos.X;
                Y = pos.Y;
                X = X + OffsetX;

                float ticks = 0;               

                if (_selectedstaff != -1)
                {
                    
                    // Find horizontal position                    
                    ticks = this.staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));

                    // Find the note
                    int note = GetNoteClicked(Y, _selectedstaff, ticks);
                    Track track = sequence1.tracks[_selectedstaff];

                    if (track.findMidiNote(note, (int)ticks) != null)
                        UpdateCurrentNote(_selectedstaff, note, ticks, false);
                    else 
                    {
                        MidiNote n = track.findPreviousMidiNote((int)ticks);
                        if (n != null)
                            UpdateCurrentNote(_selectedstaff, n.Number, n.StartTime, false);
                    }
                }
            }

        }

        /// <summary>
        /// Event: mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetMusic_MouseDown(object sender, MouseEventArgs e)
        {
            if (OnSMMouseDown != null)
            {
                
                if (e.Button == MouseButtons.Left)
                {
                    #region left button

                    if (bShowHelpGrid == true)
                    {
                        #region edit notes

                        bSelectingNote = false;
                        
                        int X = e.Location.X;
                        int Y = e.Location.Y;

                        X = Convert.ToInt32(X / zoom);
                        Y = Convert.ToInt32(Y / zoom);

                        float ticks = 0;

                        // Find the staff clicked
                        int numstaff = GetStaffClicked(Y);

                        if (numstaff != -1)
                        {
                            // Change staff selected
                            SelectedStaff = numstaff;

                            CurrentNote.numstaff = numstaff;
                            if (X < 0)
                                X = -X;

                            Point pPos = PointToClient(Control.MousePosition);
                            pPos.X = Convert.ToInt32((pPos.X + OffsetX)/zoom);

                            ticks = this.staffs[numstaff].PulseTimeForPoint(pPos);

                            // Find the note
                            int note = GetNoteClicked(Y, numstaff, ticks);
                            // Change current note
                            UpdateCurrentNote(numstaff, note, ticks, true);

                            OnSMMouseDown(this, e, numstaff, note, ticks);
                        }
                        #endregion edit
                    }
                    else if (bEditMode == true)
                    {
                        #region start selection draw rect or select note
                        // Selection start
                        // Draw a selection rectangle
                        selRect = new Rectangle();
                        bSelectingNote = true;

                        ClearSelectedNotes();                          

                        // save start position of rectangle
                        aPos = PointToClient(Control.MousePosition);
                        
                        // Change selected staff
                        SelectedStaff = GetStaffClicked(e.Y);

                        this.Invalidate();

                        #endregion

                    }
                    else
                    {
                        #region draw vertical blue bar
                        int X = e.Location.X;
                        int Y = e.Location.Y;
                        X = Convert.ToInt32(X / zoom);
                        Y = Convert.ToInt32(Y / zoom);

                        float ticks = 0;

                        // Find the staff
                        int numstaff = GetStaffClicked(Y);

                        if (numstaff != -1)
                        {
                            // Find horizontal position to draw blue vertical bar                            
                            Point pPos = PointToClient(Control.MousePosition);                            
                            pPos.X = pPos.X + OffsetX;

                            ticks = this.staffs[numstaff].PulseTimeForPoint(pPos);                            
                            OnSMMouseDown(this, e, numstaff, 0, ticks);
                        }
                        #endregion
                    }

                    #endregion left button
                }

                else if (e.Button == MouseButtons.Right && bEditMode == true)
                {
                    #region right button

                    // Change staff selection
                    SelectedStaff = GetStaffClicked(e.Y);
                    // Save real X, Y because menus can be displayed on another staff
                    selectedX = e.X;
                    selectedY = e.Y;

                    // Affiche menu contextuel copy paste sur une portée
                    smContextMenu = new ContextMenuStrip();
                    smContextMenu.Items.Clear();

                    /*
                    // Insert measure                    
                    ToolStripMenuItem menuInsertMeasure = new ToolStripMenuItem(Strings.InsertMeasure);
                    smContextMenu.Items.Add(menuInsertMeasure);
                    // -> this track
                    ToolStripMenuItem menuInsertMeasureThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuInsertMeasure.DropDownItems.Add(menuInsertMeasureThisTrack);
                    menuInsertMeasureThisTrack.Click += new System.EventHandler(this.MnuInsertMeasureThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuInsertMeasureAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuInsertMeasure.DropDownItems.Add(menuInsertMeasureAllTracks);
                    menuInsertMeasureAllTracks.Click += new System.EventHandler(this.MnuInsertMeasureAllTracks_Click);
                    menuInsertMeasureAllTracks.ShortcutKeys = Keys.Control | Keys.I;     // Shortcut.CtrlI;
                    menuInsertMeasure.ShortcutKeyDisplayString = "Ctrl+I";
                    */

                    // IMPROVEMENT 230423 Insert Measures
                    ToolStripMenuItem menuInsertMeasures = new ToolStripMenuItem(Strings.InsertMeasures);
                    smContextMenu.Items.Add(menuInsertMeasures);
                    menuInsertMeasures.Click += new EventHandler(this.MnuInsertMeasures_Click);
                    menuInsertMeasures.ShortcutKeys = Keys.Control | Keys.I;     // Shortcut.CtrlI;
                    menuInsertMeasures.ShortcutKeyDisplayString = "Ctrl+I";

                    // Delete measures
                    ToolStripMenuItem menuDeleteMeasures = new ToolStripMenuItem(Strings.DeleteMeasures);
                    smContextMenu.Items.Add(menuDeleteMeasures);
                    menuDeleteMeasures.Click += new EventHandler(this.MnuDeleteMeasures_Click);
                    menuDeleteMeasures.ShortcutKeys = Keys.Control | Keys.D;     // Shortcut.CtrlD;
                    menuDeleteMeasures.ShortcutKeyDisplayString = "Ctrl+D";

                    /*
                    // Delete measure
                    ToolStripMenuItem menuDeleteMeasure = new ToolStripMenuItem(Strings.DeleteMeasure);
                    smContextMenu.Items.Add(menuDeleteMeasure);
                    // -> this track
                    ToolStripMenuItem menuDeleteMeasureThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuDeleteMeasure.DropDownItems.Add(menuDeleteMeasureThisTrack);
                    menuDeleteMeasureThisTrack.Click += new System.EventHandler(this.MnuDeleteMeasureThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuDeleteMeasureAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuDeleteMeasure.DropDownItems.Add(menuDeleteMeasureAllTracks);
                    menuDeleteMeasureAllTracks.Click += new System.EventHandler(this.MnuDeleteMeasureAllTracks_Click);
                    menuDeleteMeasureAllTracks.ShortcutKeys = Keys.Control | Keys.D;     // Shortcut.CtrlD;
                    menuDeleteMeasure.ShortcutKeyDisplayString = "Ctrl+D";
                    */


                    // Sep 1
                    ToolStripSeparator menusep1 = new ToolStripSeparator();
                    smContextMenu.Items.Add(menusep1);


                    // Insert 1 time
                    ToolStripMenuItem menuInsertTime = new ToolStripMenuItem(Strings.InsertOneTime);
                    smContextMenu.Items.Add(menuInsertTime);
                    // -> this track
                    ToolStripMenuItem menuInsertTimeThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuInsertTime.DropDownItems.Add(menuInsertTimeThisTrack);
                    menuInsertTimeThisTrack.Click += new System.EventHandler(this.MnuInsertTimeThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuInsertTimeAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuInsertTime.DropDownItems.Add(menuInsertTimeAllTracks);
                    menuInsertTimeAllTracks.Click += new System.EventHandler(this.MnuInsertTimeAllTracks_Click);

                    

                    // Insert 1/2 time
                    ToolStripMenuItem menuInsertHalfTime = new ToolStripMenuItem(Strings.InsertHalfTime);
                    smContextMenu.Items.Add(menuInsertHalfTime);
                    // -> this track
                    ToolStripMenuItem menuInsertHalfTimeThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuInsertHalfTime.DropDownItems.Add(menuInsertHalfTimeThisTrack);
                    menuInsertHalfTimeThisTrack.Click += new System.EventHandler(this.MnuInsertHalfTimeThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuInsertHalfTimeAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuInsertHalfTime.DropDownItems.Add(menuInsertHalfTimeAllTracks);
                    menuInsertHalfTimeAllTracks.Click += new System.EventHandler(this.MnuInsertHalfTimeAllTracks_Click);
                    


                    // Delete 1 time
                    ToolStripMenuItem menuDeleteTime = new ToolStripMenuItem(Strings.DeleteOneTime);
                    smContextMenu.Items.Add(menuDeleteTime);
                    // -> this track
                    ToolStripMenuItem menuDeleteTimeThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuDeleteTime.DropDownItems.Add(menuDeleteTimeThisTrack);
                    menuDeleteTimeThisTrack.Click += new System.EventHandler(this.MnuDeleteTimeThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuDeleteTimeAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuDeleteTime.DropDownItems.Add(menuDeleteTimeAllTracks);
                    menuDeleteTimeAllTracks.Click += new System.EventHandler(this.MnuDeleteTimeAllTracks_Click);


                    // Delete 1/2 time 
                    ToolStripMenuItem menuDeleteHalfTime = new ToolStripMenuItem(Strings.DeleteHalfTime);
                    smContextMenu.Items.Add(menuDeleteHalfTime);
                    // -> this track
                    ToolStripMenuItem menuDeleteHalfTimeThisTrack = new ToolStripMenuItem(Strings.ThisTrack);
                    menuDeleteHalfTime.DropDownItems.Add(menuDeleteHalfTimeThisTrack);
                    menuDeleteHalfTimeThisTrack.Click += new System.EventHandler(this.MnuDeleteHalfTimeThisTrack_Click);
                    // -> all tracks
                    ToolStripMenuItem menuDeleteHalfTimeAllTracks = new ToolStripMenuItem(Strings.AllTracks);
                    menuDeleteHalfTime.DropDownItems.Add(menuDeleteHalfTimeAllTracks);
                    menuDeleteHalfTimeAllTracks.Click += new System.EventHandler(this.MnuDeleteHalfTimeAllTracks_Click);


                    // Offset start times of all notes                    
                    //ToolStripMenuItem menuOffsetNotes = new ToolStripMenuItem("Offset start times");
                    ToolStripMenuItem menuOffsetNotes = new ToolStripMenuItem(Strings.OffsetStartTimesOfNotes);
                    smContextMenu.Items.Add(menuOffsetNotes);
                    menuOffsetNotes.Click += new EventHandler(this.MnuOffsetNotes_Click);



                    // Sep 2
                    ToolStripSeparator menusep2 = new ToolStripSeparator();
                    smContextMenu.Items.Add(menusep2);

                    // PianoRoll
                    ToolStripMenuItem menuPianoRoll = new ToolStripMenuItem("PianoRoll");
                    smContextMenu.Items.Add(menuPianoRoll);
                    menuPianoRoll.Click += new EventHandler(this.MnuPianoRoll_Clicked);
                    //menuPianoRoll.Shortcut = Shortcut.CtrlD;


                    // If notes selected, draw menu Copy/Paste
                    if (_selnotes == null || _selnotes.Count == 0)
                        _selnotes =  GetSelectedNotes(e);

                    if (_selnotes.Count > 0)
                    {
                        ToolStripSeparator menusep3 = new ToolStripSeparator();
                        smContextMenu.Items.Add(menusep3);

                        // Copy
                        ToolStripMenuItem menuCopy = new ToolStripMenuItem(Strings.Copy);
                        smContextMenu.Items.Add(menuCopy);
                        menuCopy.Click += new System.EventHandler(this.MnuCopy_Click);
                        menuCopy.ShortcutKeys = Keys.Control | Keys.C;       // Shortcut.CtrlC;
                        menuCopy.ShortcutKeyDisplayString = "Ctrl+C";

                        // Paste
                        ToolStripMenuItem menuPaste = new ToolStripMenuItem(Strings.Paste);
                        smContextMenu.Items.Add(menuPaste);
                        menuPaste.Click += new System.EventHandler(this.MnuPaste_Click);                        
                        menuPaste.ShortcutKeys = Keys.Control | Keys.V;                           // Shortcut.CtrlV;
                        menuPaste.ShortcutKeyDisplayString = "Ctrl+V";

                        // Triolet
                        ToolStripMenuItem menuTriolet = new ToolStripMenuItem("Triolet");
                        smContextMenu.Items.Add(menuTriolet);
                        menuTriolet.Click += new System.EventHandler(this.MnuTriolet_Click);


                    }

                    // select measures
                    ToolStripMenuItem menuSelectMeasures = new ToolStripMenuItem(Strings.SelectMeasures);
                    smContextMenu.Items.Add(menuSelectMeasures);
                    menuSelectMeasures.Click += new EventHandler(this.MnuSelectMeasures_Click);


                    // Show menu
                    smContextMenu.Show(this, this.PointToClient(Cursor.Position));

                    #endregion right button
                }
            }
        }

 



        /// <summary>
        /// Event: mouse up = note stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetMusic_MouseUp(object sender, MouseEventArgs e)
        {
            if (OnSMMouseUp != null && e.Button == MouseButtons.Left)
            {
                if (bShowHelpGrid == true)
                {
                    #region edit

                    bSelectingNote = false;

                    int X = e.Location.X;
                    int Y = e.Location.Y;

                    X = X + OffsetX;

                    X = Convert.ToInt32(X / zoom);
                    Y = Convert.ToInt32(Y / zoom);

                    float ticks = 0;

                    // Find the staff
                    int numstaff = GetStaffClicked(Y);

                    if (numstaff != -1)
                    {
                        // Find the note
                        int note = GetNoteClicked(Y, numstaff, ticks);

                        OnSMMouseUp(this, e, numstaff, note, ticks);
                    }
                    #endregion edit
                }
                else
                {
                    if (bSelectingNote)
                    {
                        try
                        {
                            //int Y = e.Location.Y;                            

                            if (_selectedstaff != -1)
                            {                                
                                // Rectangle selection for notes
                                Point pos = PointToClient(Control.MousePosition);

                                int x = Math.Min(aPos.X, pos.X);
                                int y = Math.Min(aPos.Y, pos.Y);
                                int w = Math.Abs(aPos.X - pos.X);
                                int h = Math.Abs(aPos.Y - pos.Y);

                                x = x + OffsetX;

                                selRect = new Rectangle(x, y, w, h);

                                // If several notes are selected, select the first one
                                if (_selnotes != null && _selnotes.Count > 0)
                                {
                                    CurrentNote.midinote = _selnotes[0];
                                    UpdateCurrentNote(_selectedstaff, CurrentNote.midinote.Number, CurrentNote.midinote.StartTime, false);
                                }

                                this.Invalidate();
                                bSelectingNote = false;
                            }
                        }
                        catch (Exception er)
                        {
                            Console.Write("\n" + er.ToString() + "\n");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Mouse move event => draw help grid to seize notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetMusic_MouseMove(object sender, MouseEventArgs e)
        {
            if (OnSMMouseMove != null)
            {
                OnSMMouseMove(this, e);

                if (bShowHelpGrid == true)
                {
                    #region edit
                    Point pPos = PointToClient(Control.MousePosition);
                    int Xpos = pPos.X;

                    int X = e.Location.X;
                    int Y = e.Location.Y;

                    bool bdrawgrid = false;
                    bool bredraw = false;

                    // Find the staff
                    int numstaff = GetStaffClicked(Convert.ToInt32(Y / zoom));
                    if (numstaff != -1)
                    {
                        bool bredrawnote = false;
                        int yline = 0;
                        int ynote = 0;

                        // FAB
                        int ytop = _staffhmaximized / 3; //33;

                        int lw = SheetMusic.LineWidth;
                        int ls = SheetMusic.LineSpace;
                        lw = Convert.ToInt32(lw * zoom);
                        ls = Convert.ToInt32(ls * zoom);

                        // ---------------------------------
                        // GRID Déplacement horiz
                        // ---------------------------------
                        // Redessiner si on sort de la plage X
                        if (Xpos < xbox - 5)
                        {
                            xbox = Xpos - 5;
                            bredraw = true;     // redessiner la grille car on bougé de 20 pixels versl la gauche ou la droite
                        }
                        else if (Xpos > xbox + 5)
                        {
                            xbox = Xpos + 5;
                            bredraw = true;     // redessiner la grille car on bougé de 20 pixels versl la gauche ou la droite
                        }

                        // ---------------------------------
                        // GRID Déplacement vert
                        // ---------------------------------
                        int staffheight = 5 * lw + 4 * ls;
                        int topstaff = ytop + numstaff * _staffhmaximized;
                        topstaff = Convert.ToInt32(topstaff * zoom);

                        int yh = topstaff - staffheight;
                        int yb = topstaff + staffheight;

                        // Si au dessus
                        if (Y <= topstaff)
                        {
                            yline = yh - ls;
                            if (ybox != 0)
                                bredraw = true;     // redessiner la grille car on a changé de côté (on était dessous)
                            bdrawgrid = true;           // il faut dessiner la grille car on est en dehors de la portée
                            ybox = 0;
                        }
                        else if (Y >= topstaff + staffheight)
                        {
                            // Si au dessous
                            yline = yb + ls;
                            if (ybox != 1)
                                bredraw = true;     // redessiner la grille car on a changé de côté (on était dessus)
                            bdrawgrid = true;           // il faut dessiner la grille car on est en dehors de la portée
                            ybox = 1;
                        }

                        int space = lw + ls;

                        // 15 lignes (5 au dessus, 5 au milieu, 5 dessous)
                        // en partant de la ligne située à ys
                        List<int> valeurs = new List<int>();
                        int ys = yh - ls;
                        for (int i = 0; i < 15; i++)
                        {
                            valeurs.Add(ys);
                            ys += space;
                        }
                        // Always draw notes, even on the portee
                        // Ne dessiner que pour les valeurs de Y au milieu d'une ligne ou sur une ligne                        
                        if (Y < valeurs[0] - space / 4)
                        {
                            ynote = valeurs[0] - space / 2; ;
                        }
                        else if (Y < valeurs[valeurs.Count - 1] + 3 * space / 4)
                        {
                            for (int i = 0; i < valeurs.Count; i++)
                            {
                                if (Y >= valeurs[i] - 3 * space / 4 && Y < valeurs[i] - space / 4)
                                {
                                    ynote = valeurs[i] - space / 2;
                                    if (ynote != yboxnote)
                                    {
                                        yboxnote = ynote;
                                        bredrawnote = true;     // la note a changée => redessiner
                                    }
                                }
                                else if (Y >= valeurs[i] - space / 4 && Y < valeurs[i] + space / 4)
                                {
                                    ynote = valeurs[i] - space / 2;
                                    if (ynote != yboxnote)
                                    {
                                        yboxnote = ynote;
                                        bredrawnote = true;     // la note a changée => redessiner
                                    }
                                    break;

                                }
                                else if (Y >= valeurs[i] + space / 4 && Y < valeurs[i] + 3 * space / 4)
                                {
                                    ynote = valeurs[i];
                                    if (ynote != yboxnote)
                                    {
                                        yboxnote = ynote;
                                        bredrawnote = true;     // la note a changée => redessiner
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            ynote = valeurs[valeurs.Count - 1];
                        }


                        // xbox : position x fixe (on bouge verticalement sur le même axe)
                        // yline : 1ere ligne de la grid mobile
                        // space espace entre 2 lignes (dessin grid mobile)
                        // ynote ordonnée de la note à dessiner
                        // bdrawgrid : dessiner ou pas la grille selon que l'on est au dessus ou au dessous de la portée
                        // bredrawnote : dessiner ou pas la note fictive
                        // bredraw : forcer le redessin                                   
                        //float ticks = this.staffs[numstaff].PulseTimeForPoint(new Point(X, Y));
                        //Point pTicks = new Point(pPos.X + Clip.X, pPos.Y);
                        Point pTicks = new Point(pPos.X + OffsetX, pPos.Y);

                        pTicks.X = Convert.ToInt32(pTicks.X / zoom);

                        float ticks = this.staffs[numstaff].PulseTimeForPoint(pTicks);

                        int note = GetNoteClicked(Y, numstaff, ticks);

                        DrawNoteGrid(xbox, yline, space, ynote, bdrawgrid, bredrawnote, bredraw, ticks, note);


                    }   //numstaff -1
                    #endregion edit

                }
                else
                {
                    if (bSelectingNote == true && e.Button == MouseButtons.Left)
                    {
                        // Continue du draw the rectangle selection                        
                        Point pos = PointToClient(Control.MousePosition);

                        // Must stay in the first staff selected
                        int numstaff = GetStaffClicked(e.Y);
                        if (numstaff != _selectedstaff)
                            pos.Y = _staffhmaximized + _selectedstaff * _staffhmaximized;

                        int x = Math.Min(aPos.X, pos.X);
                        int y = Math.Min(aPos.Y, pos.Y);

                        int w = Math.Abs(aPos.X - pos.X);
                        int h = Math.Abs(aPos.Y - pos.Y);

                        x = x + OffsetX;

                        selRect = new Rectangle(x, y, w, h);
                        this.Invalidate();
                    }
                }
            }
        }

        #endregion


        #region Menus

        /// <summary>
        /// Menu: Insert several measures in current track or all tracks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MnuInsertMeasures_Click(object sender, EventArgs e)
        {
            aPos = PointToClient(Control.MousePosition);
            int X = aPos.X;
            int Y = aPos.Y;

            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);

            // Click on menu can be located on wrong staff if menu is very long           
            Y = selectedY;

            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                Staff staff = this.staffs[_selectedstaff];

                if (X < 0) X = -X;

                int ticks = staff.PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure de départ par défaut
                decimal MeasureFrom = 1 + Convert.ToInt32(ticks / measurelen);
                

                // Display Dialog form                
                DialogResult dr = new DialogResult();               
                UI.frmInsertMeasuresDialog InsertMeasuresDialog = new UI.frmInsertMeasuresDialog(MeasureFrom);
                dr = InsertMeasuresDialog.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;

                // Select all measures ?
                bool bAllTracks = InsertMeasuresDialog.bAllTracks;
                MeasureFrom = InsertMeasuresDialog.startMeasure - 1;
                decimal nbMeasures = InsertMeasuresDialog.nbMeasures;
                int startticks = (int)MeasureFrom * measurelen;

                if (bAllTracks)
                {
                    foreach (Track track in sequence1.tracks)
                    {
                        track.insertMeasure(startticks, (int)nbMeasures*measurelen);
                    }
                    
                }
                else
                {
                    Track track = sequence1.tracks[_selectedstaff];
                    track.insertMeasure(startticks, (int)nbMeasures*measurelen);
                }

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;                                  

                Invalidate();
            }
        }

        /// <summary>
        /// Menu: Delete several measueres in current track or all tracks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void MnuDeleteMeasures_Click(object sender, EventArgs e)
        {
            aPos = PointToClient(Control.MousePosition);
            int X = aPos.X;
            int Y = aPos.Y;

            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);

            // Click on menu can be located on wrong staff if menu is very long           
            Y = selectedY;

            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                Staff staff = this.staffs[_selectedstaff];

                if (X < 0) X = -X;

                int ticks = staff.PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure de départ par défaut
                decimal MeasureFrom = 1 + Convert.ToInt32(ticks / measurelen);


                // Display Dialog form                
                DialogResult dr = new DialogResult();
                UI.frmDeleteMeasuresDialog DeleteMeasuresDialog = new UI.frmDeleteMeasuresDialog(MeasureFrom);
                dr = DeleteMeasuresDialog.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;

                // Select all measures ?
                bool bAllTracks = DeleteMeasuresDialog.bAllTracks;
                MeasureFrom = DeleteMeasuresDialog.startMeasure - 1;
                decimal nbMeasures = DeleteMeasuresDialog.nbMeasures;
                int startticks = (int)MeasureFrom * measurelen;

                if (bAllTracks)
                {
                    foreach (Track track in sequence1.tracks)
                    {
                        track.deleteMeasure(startticks, (int)nbMeasures * measurelen);
                    }
                }
                else
                {
                    Track track = sequence1.tracks[_selectedstaff];
                    track.deleteMeasure(startticks, (int)nbMeasures * measurelen);
                }

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;

                Invalidate();
            }
        }

        /*
        /// <summary>
        /// Menu: insert a measure in a track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertMeasureThisTrack_Click(object sender, EventArgs e)
        {
            int X = selectedX;
            int Y = selectedY;
            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);
            Y = Convert.ToInt32(Y / zoom);

            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (X < 0) X = -X;
                int ticks = staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure à décaler
                int NumMeasure = Convert.ToInt32(ticks / measurelen);
                int startticks = NumMeasure * measurelen;

                Track track = sequence1.tracks[_selectedstaff];
                track.insertMeasure(startticks, measurelen);
                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Menu: insert a measure in all tracks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertMeasureAllTracks_Click(object sender, EventArgs e)
        {
            int X = selectedX;
            int Y = selectedY;
            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);
            Y = Convert.ToInt32(Y / zoom);

            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (X < 0) X = -X;
                int ticks = staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure à décaler
                int NumMeasure = Convert.ToInt32(ticks / measurelen);
                int startticks = NumMeasure * measurelen;

                foreach (Track track in sequence1.tracks)
                {
                    track.insertMeasure(startticks, measurelen);
                }
                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Menu: delete a measure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDeleteMeasureThisTrack_Click(object sender, EventArgs e)
        {
            int X = selectedX;
            int Y = selectedY;

            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);
            Y = Convert.ToInt32(Y / zoom);
            
            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (X < 0)
                    X = -X;
                int ticks = staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure à supprimer
                int NumMeasure = Convert.ToInt32(ticks / measurelen);
                int startticks = NumMeasure * measurelen;
                Track track = sequence1.tracks[_selectedstaff];
                track.deleteMeasure(startticks, measurelen);

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }

        /// <summary>
        /// Menu: delete a mesure for all tracks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDeleteMeasureAllTracks_Click(object sender, EventArgs e)
        {
            int X = selectedX;
            int Y = selectedY;

            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);
            Y = Convert.ToInt32(Y / zoom);

            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (X < 0)
                    X = -X;
                int ticks = staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));

                // Numéro de mesure à supprimer
                int NumMeasure = Convert.ToInt32(ticks / measurelen);
                int startticks = NumMeasure * measurelen;

                foreach (Track track in sequence1.tracks)
                {
                    track.deleteMeasure(startticks, measurelen);
                }

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }
        */

        private void MnuDeleteTimeThisTrack_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division;
            DeleteTimeThisTrack(dur);
        }

        private void MnuDeleteTimeAllTracks_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division;
            DeleteTimeAllTracks(dur);
        }



        private void MnuInsertTimeThisTrack_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division;
            InsertTimeThisTrack(dur);
        }

        /// <summary>
        /// Insert 1 time to all tracks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertTimeAllTracks_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division;
            InsertTimeAllTracks(dur);
        }


        private void InsertTimeThisTrack(int dur)
        {
            int Y = selectedY;
            Y = Convert.ToInt32(Y / zoom);
            
            if (_selectedstaff != -1 && CurrentNote.numstaff == _selectedstaff)
            {
                Cursor.Current = Cursors.WaitCursor;

                int startticks = (int)CurrentNote.midinote.StartTime;
                Track track = sequence1.tracks[_selectedstaff];
                // insert dur duration
                track.insertMeasure(startticks + 1, dur);

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }

        }

        private void InsertTimeAllTracks(int dur)
        {
            int Y = selectedY;
            Y = Convert.ToInt32(Y / zoom);

            if (_selectedstaff != -1 && CurrentNote.numstaff == _selectedstaff)
            {
                Cursor.Current = Cursors.WaitCursor;

                int startticks = (int)CurrentNote.midinote.StartTime;

                foreach (Track track in sequence1.tracks)
                {
                    track.insertMeasure(startticks + 1, dur);
                }

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }

        }


        /// <summary>
        /// Delete time starting from currentnote
        /// </summary>
        /// <param name="dur"></param>
        private void DeleteTimeThisTrack(int dur)
        {
            int Y = selectedY;
            Y = Convert.ToInt32(Y / zoom);
           
            if (_selectedstaff != -1 && CurrentNote.numstaff == _selectedstaff)
            {
                Cursor.Current = Cursors.WaitCursor;

                int startticks = (int)CurrentNote.midinote.StartTime;
                Track track = sequence1.tracks[_selectedstaff];
                // delete dur duration
                track.deleteMeasure(startticks + 1, dur);

                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }


        private void DeleteTimeAllTracks(int dur)
        {
            int Y = selectedY;
            Y = Convert.ToInt32(Y / zoom);

            if (_selectedstaff != -1 && CurrentNote.numstaff == _selectedstaff)
            {
                Cursor.Current = Cursors.WaitCursor;

                int startticks = (int)CurrentNote.midinote.StartTime;
                
                foreach (Track track in sequence1.tracks)
                {
                    track.deleteMeasure(startticks + 1, dur);
                }
                
                this.Refresh();

                // Raise event
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);

                Cursor.Current = Cursors.Default;
            }
        }

        private void MnuDeleteHalfTimeThisTrack_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division / 2;
            DeleteTimeThisTrack(dur);
        }

        private void MnuDeleteHalfTimeAllTracks_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division / 2;
            DeleteTimeAllTracks(dur);
        }


        private void MnuInsertHalfTimeThisTrack_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division / 2;
            InsertTimeThisTrack(dur);
        }

        private void MnuInsertHalfTimeAllTracks_Click(object sender, EventArgs e)
        {
            int dur = sequence1.Division / 2;
            InsertTimeAllTracks(dur);
        }



        /// <summary>
        /// Menu: paste notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuPaste_Click(object sender, EventArgs e)
        {
            if (bReadyToPaste)
            {
                float ticks = sequence1.GetLength();
                float srcstarttime = sequence1.GetLength();
                float srcendtime = 0;

                // first tick and last tick of the copied notes
                MidiNote note;
                for (int i = 0; i < _selnotes.Count; i++)
                {
                    note = _selnotes[i];
                    if (note.StartTime < srcstarttime)
                        srcstarttime = note.StartTime;

                    if (note.EndTime > srcendtime)
                        srcendtime = note.EndTime;
                }
                
                // measure of copy
                int NumMeasureorg = 1 + Convert.ToInt32(srcstarttime) / measurelen;


                // Destination paste
                Point cltPos = PointToClient(Control.MousePosition);

                int X = cltPos.X;
                int Y = selectedY; // calculated in mousedown

                X = X + OffsetX;

                X = Convert.ToInt32(X / zoom);
                Y = Convert.ToInt32(Y / zoom);

                int noteMeasure = 0;
                int destnumstaff = GetStaffClicked(Y);

                if (destnumstaff != -1)
                {
                    

                    
                    Cursor.Current = Cursors.WaitCursor;
                    if (X < 0) X = -X;
                    ticks = staffs[destnumstaff].PulseTimeForPoint(new Point(X, Y));

                    // Numéro de mesure                 
                    int NumMeasure = 1 + Convert.ToInt32(ticks) / measurelen;

                    // delta measures                    
                    int deltaticks = Convert.ToInt32((NumMeasure - NumMeasureorg) * measurelen);  // ticks du début de mesure

                    Track desttrack = sequence1.tracks[destnumstaff];

                    // Copy all events
                    desttrack.CopyEvents(srcstarttime, srcendtime, srcstarttime + deltaticks);

                    /*
                    // Copy notes
                    foreach (MidiNote n in _selnotes)
                    {
                        // Create new notes having the channel of the target track in case the paste is done on two different tracks!                        
                        MidiNote newnote = new MidiNote(n.StartTime, track.MidiChannel, n.Number, n.Duration, n.Velocity, false);

                        noteMeasure = Convert.ToInt32(newnote.StartTime / measurelen);
                        ticks = newnote.StartTime + deltaticks;
                        newnote.StartTime = Convert.ToInt32(ticks);
                        track.addNote(newnote);
                    }

                    if (track.Notes.Count > 1)
                        track.Notes.Sort(track.Notes[0]);
                    
                    */

                    // Refresh track notes
                    desttrack.ExtractNotes();

                    this.Refresh();
                    // Redraw selected notes in red
                    RestoreSelectedNotes(destnumstaff);

                    MidiNote nn = _selnotes[_selnotes.Count - 1];
                    UpdateCurrentNote(destnumstaff, nn.Number, nn.StartTime, false);

                    // Raise Event
                    FileModified?.Invoke(this);
                    WidthChanged?.Invoke(maxstaffwidth);

                    Cursor.Current = Cursors.Default;
                }
                else
                {
                    MessageBox.Show("Sorry, no staff selected ! : Y = " + Y.ToString());
                }
            }
            else
            {
                MessageBox.Show("Sorry, nothing to paste");
            }
        }

        /// <summary>
        /// Menu: copy notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuCopy_Click(object sender, EventArgs e)
        {
            if (_selnotes.Count > 0)
                bReadyToPaste = true;
            else
                bReadyToPaste = false;
        }



        /// <summary>
        /// Menu => PianoRoll show required event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuPianoRoll_Clicked(object sender, EventArgs e)
        {
            // Click on Menu         

            aPos = PointToClient(Control.MousePosition);
            int X = aPos.X;
            int Y = aPos.Y;

            X = Convert.ToInt32(X / zoom);

            // Click on menu can be located on wrong staff if menu is very long
            Y = selectedY;

            int numstaff = GetStaffClicked(Y);
            if (numstaff != -1)
            {
                MnuPianoRollClick(sender, e, numstaff);
            }


        }

        /// <summary>
        /// Transform selected notes into a triolet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuTriolet_Click(object sender, EventArgs e)
        {
            if (_selnotes.Count == 0)
                return;

            int Y = selectedY; // calculated in mousedown
            int numstaff = GetStaffClicked(Y);
            if (numstaff == -1)
                return;
            Track track = sequence1.tracks[numstaff];


            // 1 - Delete notes of triolet
            int maxDuration = 0;        // the biggest note duration in the list will determine the duration of the triolet
            List<MidiNote> lstdel = new List<MidiNote>();
            for (int i = 0; i < _selnotes.Count; i++)
            {
                lstdel.Add(_selnotes[i]);
                if (_selnotes[i].Duration > maxDuration)
                    maxDuration = _selnotes[i].Duration;
            }
            foreach(MidiNote n in lstdel)
            {
                track.deleteNote(n.Number, n.StartTime);
            }

            // 2 - recreate each note by changing its duration and start time
            // Duration of each standard notes of the triolet is 1/3 of upper note
            // 3 half notes (1/3 + 1/3 + 1/3) => 1 black
            // 2 crochets (1/6 + 1/6)  + 2 half notes (1/3 + 1/3) => 1 black

            int standardDuration = AdjustDurationToGrid(maxDuration);   // Align to grid the max duration. value = 1/2 for egg for a crochet
            int UpperDuration = 2 * standardDuration;                   // Duration of the triolet determined with the biggest note duration of the list. value = 2 * 1/2 for egg

            int elapse = 0;
            int ticksStart = _selnotes[0].StartTime;
            int currentChordStartTime = ticksStart;
            int newStartTime = currentChordStartTime;
            int lastnoteduration = 0;


            foreach (MidiNote n in _selnotes)
            {
                int noteduration = AdjustDurationToGrid(n.Duration);

                if (noteduration == standardDuration)
                    noteduration = UpperDuration / 3;                   // if standart duration, new value = 1/3
                else
                    noteduration = standardDuration / 3;                // if half of standard duration, new value = 1/6

                // New duration of the note
                n.Duration = noteduration - 1;

                // If the note does not belong to the current chord
                if (n.StartTime > currentChordStartTime)
                {
                    // Save start time of new chord
                    currentChordStartTime = n.StartTime;

                    // Add to elapse the duration of the previous note
                    elapse += lastnoteduration;

                    // new start time of all notes of this new chord (can be a single note)
                    newStartTime = ticksStart + elapse;                    
                }
                
                // Save the duration of the current note/chord 
                // it will be used in case a change of chord
                lastnoteduration = noteduration;
                
                // Modified start time to fit the triolet duration
                n.StartTime = newStartTime;
                // Create note
                track.addNote(n);                
            }
          
            this.Refresh();
            // Raise Event
            FileModified?.Invoke(this);
        }

        /// <summary>
        /// Select several measures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuSelectMeasures_Click(object sender, EventArgs e)
        {
            aPos = PointToClient(Control.MousePosition);
            int X = aPos.X;
            int Y = aPos.Y;

            X = X + OffsetX;
            X = Convert.ToInt32(X / zoom);
            
            // Click on menu can be located on wrong staff if menu is very long           
            Y = selectedY;
            
            if (_selectedstaff != -1)
            {
                Cursor.Current = Cursors.WaitCursor;

                Staff staff = this.staffs[_selectedstaff];

                if (X < 0) X = -X;

                int ticks = staff.PulseTimeForPoint(new Point(X, Y));                

                // Numéro de mesure de départ par défaut
                int MeasureFrom = 1 + Convert.ToInt32(ticks / measurelen);
                int MeasureTo = 0;
                int ticksFrom = 0;
                int ticksTo = 0;

                // Display Dialog form
                int maxi = nbMeasures;
                DialogResult dr = new DialogResult();
                UI.selectMeasuresDialog SelectMeasuresDialog = new UI.selectMeasuresDialog(MeasureFrom, maxi);
                dr = SelectMeasuresDialog.ShowDialog();

                if (dr == System.Windows.Forms.DialogResult.Cancel)
                    return;

                // Select all measures ?
                bool bAllMeasures = SelectMeasuresDialog.bAllMeasures;

                if (!bAllMeasures)
                {
                    MeasureFrom = Convert.ToInt32(SelectMeasuresDialog.MeasureFrom);
                    MeasureTo = Convert.ToInt32(SelectMeasuresDialog.MeasureTo);

                    ticksFrom = (MeasureFrom - 1) * measurelen;
                    ticksTo = (MeasureTo) * measurelen;

                    ClearSelectedNotes();
                    staff.selectNotes(ticksFrom, ticksTo);
                }
                else
                {
                    ticksFrom = 0;
                    ticksTo = staff.EndTime;
                    ClearSelectedNotes();
                    staff.selectNotes(ticksFrom, ticksTo);
                }

                _selnotes = staff.getSelectedNotes();

                if (_selnotes != null && _selnotes.Count > 0)
                {
                    CurrentNote.midinote = _selnotes[0];
                    UpdateCurrentNote(_selectedstaff, CurrentNote.midinote.Number, CurrentNote.midinote.StartTime, false);
                }
               
                bReadyToPaste = true;

                //Console.Write("\nSortie selection mesures : selNotes.Count = " + selNotes.Count);

                Invalidate();                
            }
        }


        private void MnuOffsetNotes_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            UI.ModifyStartTimesDialog ModifyStartTimesDialog = new UI.ModifyStartTimesDialog();
            dr = ModifyStartTimesDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            int StartTime = ModifyStartTimesDialog.StartTime;
            int Offset = ModifyStartTimesDialog.Offset;
            Track track = sequence1.tracks[_selectedstaff];
            track.OffsetStartTimes(StartTime, Offset);

            this.Refresh();

            // Raise event
            FileModified?.Invoke(this);
            WidthChanged?.Invoke(maxstaffwidth);

            Cursor.Current = Cursors.Default;
        }

        #endregion


        #region frmNoteEdit

        /// <summary>
        /// frmNoteEdit - Replace currentNote by n
        /// </summary>
        /// <param name="n"></param>
        public void ModifyCurrentNote(MidiNote n, bool resetselection)
        {
            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];

            // Delete currentnote
            trk.deleteNote(CurrentNote.midinote.Number, CurrentNote.midinote.StartTime);
            // Create new note n (starttime can be different)
            trk.addNote(n, false);

            // Update current note
            UpdateCurrentNote(numstaff, n.Number, n.StartTime, resetselection);

            // Raise event
            FileModified?.Invoke(this);
        }

        /// <summary>
        /// frmNoteEdit - Modify the velocity of a note
        /// </summary>
        /// <param name="Velocity"></param>
        public void ModifyVelocitySelectedNotes(int Velocity)
        {
            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];
            List<MidiNote> _lstmidinotes = new List<MidiNote>();
            foreach (MidiNote n in _selnotes)
            {
                _lstmidinotes.Add(n);
            }

            foreach (MidiNote mn in _lstmidinotes)
            {
                // Delete original selected note
                MidiNote n = new MidiNote(mn.StartTime, mn.Channel, mn.Number, mn.Duration, Velocity, true);
                // Create new note
                trk.deleteNote(mn.Number, mn.StartTime);
                trk.addNote(n, false);
            }

            // Update current note
            UpdateCurrentNote(numstaff, CurrentNote.midinote.Number, CurrentNote.midinote.StartTime, false);

            // Raise event
            FileModified?.Invoke(this);
        }

        #region Effects
        
        public bool IsPitchBend(int channel, int starttime, int endtime)
        {
            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];
            MidiNote mn = CurrentNote.midinote;
            return trk.IsPitchBend(mn.Channel, mn.StartTime, mn.EndTime);

        }
        
        public List<MidiEvent> findPitchBendValues(int Channel, int StartTime, int EndTime)
        {
            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];
            MidiNote mn = CurrentNote.midinote;
            return trk.findPitchBendValues(mn.Channel, mn.StartTime, mn.EndTime);

        }

        /// <summary>
        /// frmNoteEdit - set Pitch Bend to a note
        /// </summary>
        public void SetPitchBend(int pitchBend)
        {
            // No pitch = 8192

            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];
            MidiNote mn = CurrentNote.midinote;
            trk.SetPitchBend(mn.Channel, mn.Number, mn.StartTime, mn.EndTime, pitchBend);
        }

        /// <summary>
        /// frmNoteEdit - Remove pitch bend to a note
        /// </summary>
        public void UnsetPitchBend()
        {
            int numstaff = CurrentNote.numstaff;
            Track trk = sequence1.tracks[numstaff];
            MidiNote mn = CurrentNote.midinote;
            trk.RemovePitchBend(mn.Channel, mn.StartTime, mn.EndTime);
        }

        #endregion

        #endregion

        public void AddSelectedNote(MidiNote midinote)
        {          
            if (_selnotes == null)
                _selnotes = new List<MidiNote>();

            if (_selnotes.Find(z => z == midinote) == null)
                _selnotes.Add(midinote);
        }

        private List<MidiNote> GetSelectedNotes(MouseEventArgs e)
        {
            List<MidiNote> L = new List<MidiNote>();
            int Y = e.Location.Y;
            Y = Convert.ToInt32(Y / zoom);
            // Find the staff
            int numstaff = GetStaffClicked(Y);
            if (numstaff != -1)
            {
                Staff staff = this.staffs[numstaff];
                L = staff.getSelectedNotes();
            }
            return L;
        }

        private void ClearSelectedNotes()
        {           
            if (_selnotes != null)
                _selnotes.Clear();

            if (staffs != null &&  staffs.Count > 0)
            {
                for (int i = 0; i < staffs.Count; i++)
                {
                    staffs[i].ClearSelectedNotes();
                }
            }
        }

        private void RestoreSelectedNotes(int staffnum)
        {
            staffs[staffnum].RestoreSelectedNotes(_selnotes);
        }


        /// <summary>
        /// Guess staff clicked
        /// </summary>
        /// <param name="Y"></param>
        /// <returns></returns>       
        private int GetStaffClicked(int Y)
        {                
            return Y/_staffhmaximized;           
        }

        /// <summary>
        /// Guess note clicked
        /// </summary>
        /// <param name="Y"></param>
        /// <returns></returns>
        private int GetNoteClicked(int Y, int numstaff, float ticks)
        {
            int note = 0;
            int Ystaff = 0;

            Staff staff = this.staffs[numstaff];

            // épaisseur pour une note = 3 pixels
            // Hauteur staff = 150 (StaffHMaximized)
            // L'offset de 23 est purement spéculatif
            int offset = 0;
            Clef clef;

            if (sequence1.tracks[numstaff].Clef == Clef.None)
                clef = clefs.GetClef(Convert.ToInt32(ticks));
            else
                clef = staff.Clef;
            
            if (clef == Clef.Treble)
            {                
                offset = 23 - 11 * (_staffhmaximized - 100) / 50;
            }
            else
            {
                offset = 11 - 11 * (_staffhmaximized - 100) / 50;
            }
                        
            Ystaff = (offset + ((numstaff + 1) * _staffhmaximized - Y) / 3);            
            
            if (Ystaff > 0 && Ystaff < AllNotes.Length)
                note = AllNotes[Ystaff];          

            return note;
        }
    

        public float GetTimeInMeasure(float ticks)
        {
            // Display note under the mouse                    
            // Num measure
            //int nummeasure = Convert.ToInt32(ticks) / measurelen;
            // Temps dans la mesure
            int rest = Convert.ToInt32(ticks) % measurelen;
            float timeinmeasure = (float)rest / sequence1.Time.Quarter;

            return timeinmeasure;
        }

        public void UpdateCurrentNote(int numstaff, int note, float ticks, bool resetSelection)
        {
            CurrentNote.numstaff = numstaff;
                        
            int duration = 0;

            if (numstaff >= sequence1.tracks.Count)
                return;
             
            Track track = sequence1.tracks[numstaff];
            MidiNote midinote = track.findMidiNote(note, Convert.ToInt32(ticks));

            if (midinote == null)
            {                
                CurrentNote.midinote = new MidiNote(0, 0, 0, 0, 0, false);
                return;
            }

            if (resetSelection)
                ClearSelectedNotes();

            duration = midinote.Duration;                
            CurrentNote.midinote = midinote;
                
            // Raise event
            CurrentNoteChanged?.Invoke(midinote);

            // Raise event even if selection = null
            if (!resetSelection)
                SelectionChanged?.Invoke(_selnotes);            
        }


        private string CurrentNoteToString(int note, string noteLetter, float ticks, int duration, int velocity)
        {
            float timeinmeasure = GetTimeInMeasure(ticks);
            return string.Format("note {0} ({1}) - time {2} - ticks {3} - duration {4} - velocity {5}", note, noteLetter, timeinmeasure, ticks, duration, velocity);
        }
       

        /// <summary>
        /// Double Click: raise event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SheetMusic_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnSMMouseDoubleClick != null)
            {
                int X = e.Location.X;
                int Y = e.Location.Y;
                X = Convert.ToInt32(X / zoom);
                Y = Convert.ToInt32(Y / zoom);

                Point pos = PointToClient(Control.MousePosition);
                X = pos.X;
                Y = pos.Y;
                X = X + OffsetX;
                float ticks = 0;


                // Find the staff
                int numstaff = GetStaffClicked(Y);
                if (numstaff != -1)
                {
                    _selectedstaff = numstaff;
                    // Find horizontal position                    
                    ticks = this.staffs[_selectedstaff].PulseTimeForPoint(new Point(X, Y));
                    OnSMMouseDoubleClick(this, e, numstaff, ticks);
                }
            }
        }



       /// <summary>
        /// Dessine une grille de ligne pour aider à saisir les notes
       /// </summary>
       /// <param name="numstaff"></param>
       /// <param name="X"></param>
       /// <param name="Y"></param>
       /// <param name="bdraw">dessiner la grille si on est au dessus ou au dessous de la portée</param>
       /// <param name="bredraw">forcer le redessin</param>
        public void DrawNoteGrid(int xbox, int yline, int space, int ynote, bool bdrawgrid, bool bredrawnote, bool bredraw, float ticks, int note)
        {           
            Graphics g = this.CreateGraphics();            
            Pen peno = new Pen(Color.Blue, 1);            
            SolidBrush myBrush = new SolidBrush(Color.Blue);
            Font font = new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Regular);

            // Problème affichage au delà d'une certaine position X
            Point pPos = PointToClient(Control.MousePosition);
            pPos.X = xbox;

            int delta = Convert.ToInt32(10 * zoom);

            // draw grid only if above or under portee
            if (bdrawgrid == true && yline > 0)
            {                
                // draw 5 lines
                for (int line = 1; line <= 5; line++)
                {
                    g.DrawLine(peno, xbox - delta, yline, xbox + delta, yline);
                    yline += space;
                }
            }
        
            int xe = Convert.ToInt32(7 * zoom);
            int ye = Convert.ToInt32(6 * zoom);

            // draw ellipse
            g.DrawEllipse(peno, xbox, ynote, xe, ye);
            Point PCenter = new Point(xbox + 2*xe, ynote - ye / 2);

            // draw time in measure
            float timeinmeasure = GetTimeInMeasure(ticks);

            string[] scale = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
            string notename = scale[(note + 3) % 12];
            string tx = timeinmeasure.ToString() + " - " + notename;

            g.DrawString(tx, font, myBrush, PCenter);
            
            if (bredraw == true || bredrawnote == true)
            {
                this.Invalidate();
            }
        }


        /// <summary>
        /// Delete note
        /// </summary>
        /// <param name="CurNote"></param>
        public void DeleteNote (int numstaff, int notenumber, float ticks)
        {
            // note to delete
            //int note = CurNote.note;
            //int numstaff = CurNote.numstaff;
            //float ticks = CurNote.ticks;

            Track track = sequence1.tracks[numstaff];
            
            // save last deleted note
            CurrentNote.lastnote = notenumber;

            // Set new current note = note before            
            MidiNote n = track.getPrevNote(notenumber, Convert.ToInt32(ticks));
            // il n'y a pas de note précédante
            if (n == null)
            {
                UpdateCurrentNote(numstaff, 60, 0, true);
            }
            else if (track.Notes.Count > 0)
            {
                UpdateCurrentNote(numstaff, n.Number, n.StartTime, true);
            }

            // Delete note
            track.deleteNote(notenumber, Convert.ToInt32(ticks));

            // Raise Events
            FileModified?.Invoke(this);
            WidthChanged?.Invoke(maxstaffwidth);
        }

        /// <summary>
        /// returns midi note for a new note
        /// </summary>
        /// <param name="notenumber">note value</param>
        /// <param name="newduration">note duration</param>
        /// <returns></returns>
        public MidiNote BuildNewNote(int notenumber, float newduration)
        {           

            // alter note
            int numstaff = CurrentNote.numstaff;
            Track track = sequence1.tracks[numstaff];
            int prevnote = CurrentNote.midinote.Number;            

            // ticks note précédante
            float prevticks = CurrentNote.midinote.StartTime;
            
            // Temps note précédante
            float prevtime = prevticks / sequence1.Division;

            // Durée note précédante            
            // pourquoi faire tous ces calculs ???
            // parceque la note précédante n'exista pas toujours (en début de portée par ex)
            float prevduration = GetAdjustedNoteDuration(track, prevnote, prevticks) / (float)sequence1.Division;
            
            // s'il n'y a pas de note précédante, pas de duration
            if (prevduration > 0)
                prevduration = AjustQuarterToGrid(prevduration);

            // newduration is new note duration according to button selected

            // New time = last note time + it's duration
            float newtime = prevtime + prevduration;

            int starttime = Convert.ToInt32(newtime * sequence1.Division);
            int dur = Convert.ToInt32(newduration * sequence1.Division);
            int channel = track.MidiChannel;

            //int velocity = 100; 

            MidiNote mdnote = new MidiNote(starttime, channel, notenumber, dur, _velocity, false);

            return mdnote;

        }

        /// <summary>
        /// Add a new note on the score
        /// </summary>
        /// <param name="track"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public int AddNote(Track track, MidiNote note)
        {
            int ret = track.addNote(note);
            if (ret > 0)
            {
                // Raise Events
                FileModified?.Invoke(this);
                WidthChanged?.Invoke(maxstaffwidth);
            }
            return ret;
        }

        #endregion edit score


        #region colors

        /** Change the note colors for the sheet music, and redraw. */
        private void SetColors(Color[] newcolors, Color newshade, Color newshade2)
        {
            if (NoteColors == null)
            {
                NoteColors = new Color[12];
                for (int i = 0; i < 12; i++)
                {
                    NoteColors[i] = Color.Black;
                }
            }
            if (newcolors != null)
            {
                for (int i = 0; i < 12; i++)
                {
                    NoteColors[i] = newcolors[i];
                }
            }
            else
            {
                for (int i = 0; i < 12; i++)
                {
                    NoteColors[i] = Color.Black;
                }
            }
            if (shadeBrush != null)
            {
                shadeBrush.Dispose();
                shade2Brush.Dispose();
            }
            shadeBrush = new SolidBrush(newshade);
            shade2Brush = new SolidBrush(newshade2);
        }

        /** Get the color for a given note number */
        public Color NoteColor(int number)
        {
            return NoteColors[NoteScale.FromNumber(number)];
        }

        /// <summary>
        /// check if this is the last note seized or selected
        /// </summary>
        /// <param name="tracknum"></param>
        /// <param name="note"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public bool IsCurrentNote(int tracknum, int note, int ticks)
        {
            if (CurrentNote != null)
                if (note != CurrentNote.midinote.Number || bplaying == true || tracknum != CurrentNote.numstaff || ticks != CurrentNote.midinote.StartTime)            
                    return false;

            return true;
        }

        /// <summary>
        /// Check if note is selected
        /// </summary>
        /// <param name="tracknum"></param>
        /// <param name="number"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public bool IsSelectedNote(int tracknum, int number, int ticks)
        {
            if (bplaying == false)
            {
                Track track = sequence1.tracks[tracknum];
                MidiNote m = track.Notes.Find(u => u.Number == number && u.StartTime == ticks);
                if (m != null && m.Selected)
                    return true;
            }
            return false;
        }

        #endregion colors

        #region lyrics
        /** Get the lyrics for each track */
        private static List<LyricSymbol>[] GetLyrics(List<Track> tracks)
        {
            bool hasLyrics = false;
            List<LyricSymbol>[] result = new List<LyricSymbol>[tracks.Count];
            
            for (int tracknum = 0; tracknum < tracks.Count; tracknum++)
            {
                
                // FAB : à corriger
                
                Track track = tracks[tracknum];
                if (track.Lyrics == null || track.Lyrics.Count == 0)
                {
                    continue;
                }
                hasLyrics = true;
                result[tracknum] = new List<LyricSymbol>();
                
                // Nouveau code
                foreach (Track.Lyric ev in track.Lyrics)
                {

                    string text = ev.Element;

                    if (text != "")
                    {
                        LyricSymbol sym = new LyricSymbol(ev.TicksOn, text);
                        result[tracknum].Add(sym);
                    }
                }

                /*
                // Ancien code                 
                foreach (MidiEvent ev in track.Lyrics)
                {
                    String text = UTF8Encoding.UTF8.GetString(ev.Value, 0, ev.Value.Length);
                    LyricSymbol sym = new LyricSymbol(ev.StartTime, text);
                    result[tracknum].Add(sym);
                }
                */
            }

            if (!hasLyrics)
            {
                return null;
            }
            else
            {
                return result;
            }
        }


        /** Add the lyric symbols to the corresponding staffs */
        static void AddLyricsToStaffs(List<Staff> staffs, List<LyricSymbol>[] tracklyrics)
        {
            foreach (Staff staff in staffs)
            {
                List<LyricSymbol> lyrics = tracklyrics[staff.Track];
                staff.AddLyrics(lyrics);
            }
        }

        #endregion lyrics


        #region zoom

        /** Set the zoom level to display at (1.0 == 100%).
         * Recalculate the SheetMusic width and height based on the
         * zoom level.  Then redraw the SheetMusic. 
         */
        public void SetZoom(float value)
        {
            if (staffs != null)
            {
                zoom = value;
                float width = 0;
                float height = 0;
                foreach (Staff staff in staffs)
                {
                    width = Math.Max(width, staff.Width * zoom);
                    height += (staff.Height * zoom);
                }
                Width = (int)(width + 2);
                Height = ((int)height) + LeftMargin;
                

                this.Invalidate();
            }
        }

        #endregion zoom


        #region chords

        /** Get the width (in pixels) needed to display the key signature */
        public static int KeySignatureWidth(KeySignature key)
        {
            ClefSymbol clefsym = new ClefSymbol(Clef.Treble, 0, false);
            int result = clefsym.MinWidth;
            AccidSymbol[] keys = key.GetSymbols(Clef.Treble);
            foreach (AccidSymbol symbol in keys)
            {
                result += symbol.MinWidth;
            }
            return result + SheetMusic.LeftMargin + 5;
        }


        private static bool IsChord(MusicSymbol symbol)
        {
            return symbol is ChordSymbol;
        }


        /** Find 2, 3, 4, or 6 chord symbols that occur consecutively (without any
         *  rests or bars in between).  There can be BlankSymbols in between.
         *
         *  The startIndex is the index in the symbols to start looking from.
         *
         *  Store the indexes of the consecutive chords in chordIndexes.
         *  Store the horizontal distance (pixels) between the first and last chord.
         *  If we failed to find consecutive chords, return false.
         */
        private static bool FindConsecutiveChords(List<MusicSymbol> symbols, TimeSignature time, int startIndex, int[] chordIndexes, ref int horizDistance)
        {

            int i = startIndex;
            int numChords = chordIndexes.Length;

            while (true)
            {
                horizDistance = 0;

                /* Find the starting chord */
                while (i < symbols.Count - numChords)
                {
                    if (symbols[i] is ChordSymbol)
                    {
                        ChordSymbol c = (ChordSymbol)symbols[i];
                        if (c.Stem != null)
                        {
                            break;
                        }
                    }
                    i++;
                }
                if (i >= symbols.Count - numChords)
                {
                    chordIndexes[0] = -1;
                    return false;
                }
                chordIndexes[0] = i;
                bool foundChords = true;
                for (int chordIndex = 1; chordIndex < numChords; chordIndex++)
                {
                    i++;
                    int remaining = numChords - 1 - chordIndex;
                    while ((i < symbols.Count - remaining) && (symbols[i] is BlankSymbol))
                    {
                        horizDistance += symbols[i].Width;
                        i++;
                    }
                    if (i >= symbols.Count - remaining)
                    {
                        return false;
                    }
                    if (!(symbols[i] is ChordSymbol))
                    {
                        foundChords = false;
                        break;
                    }
                    chordIndexes[chordIndex] = i;
                    horizDistance += symbols[i].Width;
                }
                if (foundChords)
                {
                    return true;
                }

                /* Else, start searching again from index i */
            }
        }

        /** Connect chords of the same duration with a horizontal beam.
         *  numChords is the number of chords per beam (2, 3, 4, or 6).
         *  if startBeat is true, the first chord must start on a quarter note beat.
         */
        private static void CreateBeamedChords(List<MusicSymbol>[] allsymbols, TimeSignature time, int numChords, bool startBeat)
        {
            int[] chordIndexes = new int[numChords];
            ChordSymbol[] chords = new ChordSymbol[numChords];

            foreach (List<MusicSymbol> symbols in allsymbols)
            {
                int startIndex = 0;
                while (true)
                {
                    int horizDistance = 0;
                    bool found = FindConsecutiveChords(symbols, time,
                                                       startIndex,
                                                       chordIndexes,
                                                       ref horizDistance);
                    if (!found)
                    {
                        break;
                    }
                    for (int i = 0; i < numChords; i++)
                    {
                        chords[i] = (ChordSymbol)symbols[chordIndexes[i]];
                    }

                    if (ChordSymbol.CanCreateBeam(chords, time, startBeat))
                    {
                        ChordSymbol.CreateBeam(chords, horizDistance);
                        startIndex = chordIndexes[numChords - 1] + 1;
                    }
                    else
                    {
                        startIndex = chordIndexes[0] + 1;
                    }

                    /* What is the value of startIndex here?
                     * If we created a beam, we start after the last chord.
                     * If we failed to create a beam, we start after the first chord.
                     */
                }
            }
        }

        /** Connect chords of the same duration with a horizontal beam.
         *
         *  We create beams in the following order:
         *  - 6 connected 8th note chords, in 3/4, 6/8, or 6/4 time
         *  - Triplets that start on quarter note beats
         *  - 3 connected chords that start on quarter note beats (12/8 time only)
         *  - 4 connected chords that start on quarter note beats (4/4 or 2/4 time only)
         *  - 2 connected chords that start on quarter note beats
         *  - 2 connected chords that start on any beat
         */
        private static void CreateAllBeamedChords(List<MusicSymbol>[] allsymbols, TimeSignature time)
        {
            if ((time.Numerator == 3 && time.Denominator == 4) ||
                (time.Numerator == 6 && time.Denominator == 8) ||
                (time.Numerator == 6 && time.Denominator == 4))
            {

                CreateBeamedChords(allsymbols, time, 6, true);
            }
            CreateBeamedChords(allsymbols, time, 3, true);
            CreateBeamedChords(allsymbols, time, 4, true);
            CreateBeamedChords(allsymbols, time, 2, true);
            CreateBeamedChords(allsymbols, time, 2, false);
        }
       
        /** Create the chord symbols for a single track.
         * @param midinotes  The Midinotes in the track.
         * @param key        The Key Signature, for determining sharps/flats.
         * @param time       The Time Signature, for determining the measures.
         * @param clefs      The clefs to use for each measure.
         * @ret An array of ChordSymbols
         */
        private List<ChordSymbol> CreateChords(List<MidiNote> midinotes, KeySignature key, TimeSignature time, ClefMeasures clefs, Clef fixedClef)
        {

            int i = 0;
            List<ChordSymbol> chords = new List<ChordSymbol>();
            List<MidiNote> notegroup = new List<MidiNote>(12);
            int len = midinotes.Count;

            Clef clef;

            while (i < len)
            {

                int starttime = midinotes[i].StartTime;

                // FAB
                // ici pour empecher changement de clé
                if (starttime >= 0 && fixedClef == Clef.None)
                    clef = clefs.GetClef(starttime);
                else
                    clef = fixedClef;

                //Clef clef = clefs.mainCle;


                /* Group all the midi notes with the same start time
                 * into the notes list.
                 */
                notegroup.Clear();
                notegroup.Add(midinotes[i]);
                i++;
                while (i < len && midinotes[i].StartTime == starttime)
                {
                    notegroup.Add(midinotes[i]);
                    i++;
                }

                /* Create a single chord from the group of midi notes with
                 * the same start time.
                 */
                ChordSymbol chord = new ChordSymbol(notegroup, key, time, clef, this);
                chords.Add(chord);
            }

            return chords;
        }

        /** Given the chord symbols for a track, create a new symbol list
          * that contains the chord symbols, vertical bars, rests, and clef changes.
          * Return a list of symbols (ChordSymbol, BarSymbol, RestSymbol, ClefSymbol)
          */
        private List<MusicSymbol> CreateSymbols(List<ChordSymbol> chords, ClefMeasures clefs, TimeSignature time, int lastStart, Clef fixedClef)
        {

            List<MusicSymbol> symbols = new List<MusicSymbol>();
            symbols = AddBars(chords, time, lastStart);
            symbols = AddRests(symbols, time);


            // In case of clef change for each measure
            if (fixedClef == Clef.None)
                symbols = AddClefChanges(symbols, clefs, time);

            return symbols;
        }

        #endregion chord


        #region symbols

        /** Add in the vertical bars delimiting measures. 
         *  Also, add the time signature symbols.
         */
        private List<MusicSymbol> AddBars(List<ChordSymbol> chords, TimeSignature time, int lastStart)
        {

            List<MusicSymbol> symbols = new List<MusicSymbol>();

            TimeSigSymbol timesig = new TimeSigSymbol(time.Numerator, time.Denominator);
            symbols.Add(timesig);

            /* The starttime of the beginning of the measure */
            int measuretime = 0;

            int i = 0;
            int number = 0;            
            
            while (i < chords.Count)
            {
                if (measuretime <= chords[i].StartTime)
                {
                    symbols.Add(new BarSymbol(measuretime, number));
                    measuretime += time.Measure;
                    number++;
                }
                else
                {
                    symbols.Add(chords[i]);
                    i++;
                }
            }
            

            /* Keep adding bars until the last StartTime (the end of the song) */
            while (measuretime < lastStart)
            {
                symbols.Add(new BarSymbol(measuretime, number));
                measuretime += time.Measure;
                number++;
            }

            /* Add the final vertical bar to the last measure */
            symbols.Add(new BarSymbol(measuretime, number));
            return symbols;
        }

        /** Add rest symbols between notes.  All times below are 
         * measured in pulses.
         */
        private List<MusicSymbol> AddRests(List<MusicSymbol> symbols, TimeSignature time)
        {
            int prevtime = 0;

            List<MusicSymbol> result = new List<MusicSymbol>(symbols.Count);

            foreach (MusicSymbol symbol in symbols)
            {
                int starttime = symbol.StartTime;
                RestSymbol[] rests = GetRests(time, prevtime, starttime);
                if (rests != null)
                {
                    foreach (RestSymbol r in rests)
                    {
                        result.Add(r);
                    }
                }

                result.Add(symbol);

                /* Set prevtime to the end time of the last note/symbol. */
                if (symbol is ChordSymbol)
                {
                    ChordSymbol chord = (ChordSymbol)symbol;
                    // FAB prevtime = Math.Max(chord.EndTime, prevtime);
                    prevtime = Math.Max(chord.EndTime + 10, prevtime);
                }
                else
                {
                    prevtime = Math.Max(starttime, prevtime);
                }
            }
            return result;
        }

        /** Return the rest symbols needed to fill the time interval between
         * start and end.  If no rests are needed, return nil.
         */
        private RestSymbol[] GetRests(TimeSignature time, int start, int end)
        {
            RestSymbol[] result;
            RestSymbol r1, r2;

            if (end - start < 0)
                return null;

            NoteDuration dur = time.GetNoteDuration(end - start);
            switch (dur)
            {
                case NoteDuration.Whole:
                case NoteDuration.Half:
                case NoteDuration.Quarter:
                case NoteDuration.Eighth:
                    r1 = new RestSymbol(start, dur);
                    result = new RestSymbol[] { r1 };
                    return result;

                case NoteDuration.DottedHalf:
                    r1 = new RestSymbol(start, NoteDuration.Half);
                    r2 = new RestSymbol(start + time.Quarter * 2,
                                        NoteDuration.Quarter);
                    result = new RestSymbol[] { r1, r2 };
                    return result;

                case NoteDuration.DottedQuarter:
                    r1 = new RestSymbol(start, NoteDuration.Quarter);
                    r2 = new RestSymbol(start + time.Quarter,
                                        NoteDuration.Eighth);
                    result = new RestSymbol[] { r1, r2 };
                    return result;

                case NoteDuration.DottedEighth:
                    r1 = new RestSymbol(start, NoteDuration.Eighth);
                    r2 = new RestSymbol(start + time.Quarter / 2,
                                        NoteDuration.Sixteenth);
                    result = new RestSymbol[] { r1, r2 };
                    return result;

                default:
                    return null;
            }
        }

        /** The current clef is always shown at the beginning of the staff, on
         * the left side.  However, the clef can also change from measure to 
         * measure. When it does, a Clef symbol must be shown to indicate the 
         * change in clef.  This function adds these Clef change symbols.
         * This function does not add the main Clef Symbol that begins each
         * staff.  That is done in the Staff() contructor.
         */
        private List<MusicSymbol> AddClefChanges(List<MusicSymbol> symbols, ClefMeasures clefs, TimeSignature time)
        {

            List<MusicSymbol> result = new List<MusicSymbol>(symbols.Count);
            Clef prevclef = clefs.GetClef(0);
            foreach (MusicSymbol symbol in symbols)
            {
                /* A BarSymbol indicates a new measure */
                if (symbol is BarSymbol)
                {
                    Clef clef = clefs.GetClef(symbol.StartTime);
                    if (clef != prevclef)
                    {
                        result.Add(new ClefSymbol(clef, symbol.StartTime - 1, true));
                    }
                    prevclef = clef;
                }
                result.Add(symbol);
            }
            return result;
        }


        /** Notes with the same start times in different staffs should be
         * vertically aligned.  The SymbolWidths class is used to help 
         * vertically align symbols.
         *
         * First, each track should have a symbol for every starttime that
         * appears in the Midi File.  If a track doesn't have a symbol for a
         * particular starttime, then add a "blank" symbol for that time.
         *
         * Next, make sure the symbols for each start time all have the same
         * width, across all tracks.  The SymbolWidths class stores
         * - The symbol width for each starttime, for each track
         * - The maximum symbol width for a given starttime, across all tracks.
         *
         * The method SymbolWidths.GetExtraWidth() returns the extra width
         * needed for a track to match the maximum symbol width for a given
         * starttime.
         */
        private void AlignSymbols(List<MusicSymbol>[] allsymbols, SymbolWidths widths, MidiOptions options, int measurelen)
        {

            // If we show measure numbers, increase bar symbol width
            if (options.showMeasures)
            {
                for (int track = 0; track < allsymbols.Length; track++)
                {
                    List<MusicSymbol> symbols = allsymbols[track];
                    foreach (MusicSymbol sym in symbols)
                    {
                        if (sym is BarSymbol)
                        {
                            sym.Width += SheetMusic.NoteWidth;
                        }
                    }
                }
            }

            for (int track = 0; track < allsymbols.Length; track++)
            {
                List<MusicSymbol> symbols = allsymbols[track];
                List<MusicSymbol> result = new List<MusicSymbol>();

                int i = 0;                                
                
                /* If a track doesn't have a symbol for a starttime,
                 * add a blank symbol.
                 */
                foreach (int start in widths.StartTimes)
                {

                    /* BarSymbols are not included in the SymbolWidths calculations */
                    while (i < symbols.Count && (symbols[i] is BarSymbol) &&
                        symbols[i].StartTime <= start)
                    {
                        result.Add(symbols[i]);
                        i++;
                    }

                    if (i < symbols.Count && symbols[i].StartTime == start)
                    {

                        while (i < symbols.Count &&
                               symbols[i].StartTime == start)
                        {

                            result.Add(symbols[i]);
                            i++;
                        }
                    }
                    else
                    {
                        result.Add(new BlankSymbol(start, 0));
                    }
                }

                // FAB : Modification proposée :
                // Ajouter un blank symbol à tous les temps de chaque mesure si pas de note
                // Il faudrait avoir un tableau des StartTimes de la mesure contenant le plus de notes                                                

                #region addblank
                
                // Add additional blanks only if the sheet is visible
                // Often not visible if used only as a karaoke ...
                if (_bvisible)
                {
                    int intervall = 32;
                    //int intervall = 4;
                    //int intervall = 0;

                    if (scrollVert == false && intervall > 0)
                    {

                        int realstart = 0;
                        int ins = -1;
                        int t = 0;

                        // choix du remplissage à blanc
                        // noire 4, croche 8, doublecroche 16, triplecroche 32


                        int duration = measurelen / intervall;
                        List<int> tpcList = new List<int>();
                        for (i = 0; i < intervall; i++)
                        {
                            t = i * duration;
                            tpcList.Add(t);
                        }

                        for (int nummeasure = 0; nummeasure < nbMeasures; nummeasure++)
                        {
                            for (i = 0; i < tpcList.Count; i++)
                            {
                                t = tpcList[i];
                                ins = -1;
                                realstart = t + nummeasure * measurelen;

                                // Add a blank symbol in each measure if there is nothing to match the maxi measure 
                                ins = result.FindIndex(s => s.StartTime >= realstart);
                                if (ins == -1)
                                    result.Add(new BlankSymbol(realstart, 0));
                                else if (result[ins].StartTime > realstart)
                                    result.Insert(ins, new BlankSymbol(realstart, 0));

                            }
                        }

                    }

                }
                #endregion addblank

                /* For each starttime, increase the symbol width by
                 * SymbolWidths.GetExtraWidth().
                 */
                i = 0;
                while (i < result.Count)
                {
                    if (result[i] is BarSymbol)
                    {
                        i++;
                        continue;
                    }
                    int start = result[i].StartTime;
                    
                    int extra = widths.GetExtraWidth(track, start);                    

                    result[i].Width += extra;

                    /* Skip all remaining symbols with the same starttime. */
                    while (i < result.Count && result[i].StartTime == start)
                    {
                        i++;
                    }
                }
                allsymbols[track] = result;
            }
        }

        #endregion symbols


        #region notes
       

        /** Get whether to show note letters or not */
        public int ShowNoteLetters
        {
            get { return showNoteLetters; }
        }

        /** Get the main key signature */
        public KeySignature MainKey
        {
            get { return mainkey; }
        }

   
        
        /** Shade all the chords played at the given pulse time.
         *  Loop through all the staffs and call staff.Shade().
         *  If scrollGradually is true, scroll gradually (smooth scrolling)
         *  to the shaded notes.
         */
        public void ShadeNotes(Rectangle clip, int currentPulseTime, int prevPulseTime, bool scrollGradually)
        {
            try
            {
                Graphics g = CreateGraphics();
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.ScaleTransform(zoom, zoom);
                int ypos = 0;

                x_shade = 0;
                int y_shade = 0;

                Rectangle selrect = new Rectangle(0, 0, 0, 0);
                int OffsetX = 0;

                foreach (Staff staff in staffs)
                {
                    g.TranslateTransform(0, ypos);
                                        
                    staff.ShadeNotes(g, clip ,shadeBrush, pen, currentPulseTime, prevPulseTime, ref x_shade, selrect, OffsetX);

                    g.TranslateTransform(0, -ypos);
                    ypos += staff.Height;
                    if (currentPulseTime >= staff.EndTime)
                    {
                        y_shade += staff.Height;
                    }
                }

                g.ScaleTransform(1.0f / zoom, 1.0f / zoom);
                g.Dispose();
                x_shade = (int)(x_shade * zoom);
                y_shade -= NoteHeight;
                y_shade = (int)(y_shade * zoom);
                
                if (currentPulseTime >= 0)
                {
                    ScrollToShadedNotes(currentPulseTime, x_shade, y_shade, scrollGradually);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);
            }
        }


        #endregion notes

        #region print PDF


        /** Write the MIDI filename at the top of the page */
        private void DrawTitle(Graphics g, string fileName)
        {
            int leftmargin = 20;
            int topmargin = 20;
           
            string title = Path.GetFileName(fileName);

            //string title = Path.GetFileName(filename);
            title = title.Replace(".mid", "").Replace("_", " ").Replace(".kar", "");
            Font font = new Font("Arial", 10, FontStyle.Bold);

            g.TranslateTransform(leftmargin, topmargin);
            g.DrawString(title, font, Brushes.Black, 0, 0);
            g.TranslateTransform(-leftmargin, -topmargin);
            font.Dispose();
        }


        /** Print the given page of the sheet music. 
          * Page numbers start from 1.
          * A staff should fit within a single page, not be split across two pages.
          * If the sheet music has exactly 2 tracks, then two staffs should
          * fit within a single page, and not be split across two pages.
          */
        public void DoPrint(Graphics g, string fileName, int pagenumber, int numpages)
        {
            int leftmargin = 20;
            int topmargin = 20;
            int rightmargin = 20;
            int bottommargin = 20;

            float scale = (g.VisibleClipBounds.Width - leftmargin - rightmargin) / PageWidth;
            g.PageScale = scale;

            int viewPageHeight = (int)((g.VisibleClipBounds.Height - topmargin - bottommargin) / scale);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, 0, 0,
                            g.VisibleClipBounds.Width,
                            g.VisibleClipBounds.Height);

            Rectangle clip = new Rectangle(0, 0, PageWidth, PageHeight);

            int ypos = TitleHeight;
            int pagenum = 1;
            int staffnum = 0;

            if (numtracks == 2 && (staffs.Count % 2) == 0)
            {
                /* Skip the staffs until we reach the given page number */
                while (staffnum + 1 < staffs.Count && pagenum < pagenumber)
                {
                    int heights = staffs[staffnum].Height + staffs[staffnum + 1].Height;
                    if (ypos + heights >= viewPageHeight)
                    {
                        pagenum++;
                        ypos = 0;
                    }
                    else
                    {
                        ypos += heights;
                        staffnum += 2;
                    }
                }

                /* Print the staffs until the height reaches viewPageHeight */
                if (pagenum == 1)
                {
                    DrawTitle(g, fileName);
                    ypos = TitleHeight;
                }
                else
                {
                    ypos = 0;
                }

                for (; staffnum + 1 < staffs.Count; staffnum += 2)
                {
                    int heights = staffs[staffnum].Height + staffs[staffnum + 1].Height;

                    if (ypos + heights >= viewPageHeight)
                        break;

                    g.TranslateTransform(leftmargin, topmargin + ypos);
                    staffs[staffnum].Draw(g, clip, selRect,pen);
                    g.TranslateTransform(-leftmargin, -(topmargin + ypos));
                    ypos += staffs[staffnum].Height;
                    g.TranslateTransform(leftmargin, topmargin + ypos);
                    staffs[staffnum + 1].Draw(g, clip, selRect,pen);
                    g.TranslateTransform(-leftmargin, -(topmargin + ypos));
                    ypos += staffs[staffnum + 1].Height;
                }
            }
            else
            {
                /* Skip the staffs until we reach the given page number */
                while (staffnum < staffs.Count && pagenum < pagenumber)
                {
                    if (ypos + staffs[staffnum].Height >= viewPageHeight)
                    {
                        pagenum++;
                        ypos = 0;
                    }
                    else
                    {
                        ypos += staffs[staffnum].Height;
                        staffnum++;
                    }
                }

                /* Print the staffs until the height reaches viewPageHeight */
                if (pagenum == 1)
                {
                    DrawTitle(g, fileName);
                    ypos = TitleHeight;
                }
                else
                {
                    ypos = 0;
                }

                for (; staffnum < staffs.Count; staffnum++)
                {
                    if (ypos + staffs[staffnum].Height >= viewPageHeight)
                        break;

                    g.TranslateTransform(leftmargin, topmargin + ypos);
                    staffs[staffnum].Draw(g, clip, selRect,pen);
                    g.TranslateTransform(-leftmargin, -(topmargin + ypos));
                    ypos += staffs[staffnum].Height;
                }
            }

            // Draw the title at the bottom lef
            Font font = new Font("Arial", 10, FontStyle.Bold);
            string title = Path.GetFileName(fileName);
            g.DrawString(title , font, Brushes.Black, leftmargin, topmargin + viewPageHeight - 12);

            /* Draw the page number */
            g.DrawString("" + pagenumber + "/" + numpages, font, Brushes.Black, PageWidth - leftmargin, topmargin + viewPageHeight - 12);

            font.Dispose();
        }



        /**
        * Return the number of pages needed to print this sheet music.
        * A staff should fit within a single page, not be split across two pages.
        * If the sheet music has exactly 2 tracks, then two staffs should
        * fit within a single page, and not be split across two pages.
        */
        public int GetTotalPages()
        {
            int num = 1;
            int currheight = TitleHeight;

            if (numtracks == 2 && (staffs.Count % 2) == 0)
            {
                for (int i = 0; i < staffs.Count; i += 2)
                {
                    int heights = staffs[i].Height + staffs[i + 1].Height;
                    if (currheight + heights > PageHeight)
                    {
                        num++;
                        currheight = heights;
                    }
                    else
                    {
                        currheight += heights;
                    }
                }
            }
            else
            {
                foreach (Staff staff in staffs)
                {
                    if (currheight + staff.Height > PageHeight)
                    {
                        num++;
                        currheight = staff.Height;
                    }
                    else
                    {
                        currheight += staff.Height;
                    }
                }
            }
            return num;
        }


        #endregion print PDF

        #region scroll

        public void ScrollTo(int currentPulseTime, int prevPulseTime)
        {            
            if (staffs == null) return;

            try
            {
                x_shade = 0;

                if (currentPulseTime >= 0)
                {
                    foreach (Staff staff in staffs)
                    {
                        staff.findScroll(currentPulseTime, prevPulseTime, ref x_shade);

                        // FAB
                        if (x_shade > 0)
                            break;
                        
                        //doScroll(currentPulseTime, x_shade);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error " + e.Message);                
            }
        }

        private void DoScroll(int currentPulseTime, int x_shade)
        {
            Panel pnlScrollview = (Panel)this.Parent;
            Point scrollPos = pnlScrollview.AutoScrollPosition;

            /* The scroll position is in negative coordinates for some reason */
            scrollPos.X = -scrollPos.X;
            scrollPos.Y = -scrollPos.Y;
            Point newPos = scrollPos;

            int scrollDist = 0;
            
            // Scrolling horizontal

            // FAB: je veux scroller tel que le milieu du panel soit la note jouée
            int x_middle = scrollPos.X + (pnlScrollview.Width / 2);
            scrollDist = (x_shade - x_middle)/2;

            newPos = new Point(scrollPos.X + scrollDist, scrollPos.Y);

            if (newPos.X < 0)
            {
                newPos.X = 0;
            }

            pnlScrollview.AutoScrollPosition = newPos;
            
        }

        /** Scroll the sheet music so that the shaded notes are visible.
          * If scrollGradually is true, scroll gradually (smooth scrolling)
          * to the shaded notes.
          */
        void ScrollToShadedNotes(int currentPulseTime, int x_shade, int y_shade, bool scrollGradually)
        {
            Panel pnlScrollview = (Panel)this.Parent;
            Point scrollPos = pnlScrollview.AutoScrollPosition;

            /* The scroll position is in negative coordinates for some reason */
            scrollPos.X = -scrollPos.X;
            scrollPos.Y = -scrollPos.Y;
            Point newPos = scrollPos;
            
            int x_view = 0;
            int scrollDist = 0;

            if (scrollVert)
            {
                // Scrolling vertical
                scrollDist = (int)(y_shade - scrollPos.Y);

                if (scrollGradually)
                {
                    if (scrollDist > (zoom * StaffHeight * 8))
                        scrollDist = scrollDist / 2;
                    else if (scrollDist > (NoteHeight * 3 * zoom))
                        scrollDist = (int)(NoteHeight * 3 * zoom);
                }
                newPos = new Point(scrollPos.X, scrollPos.Y + scrollDist);
            }
            else
            {
                // Scrolling horizontal
                x_view = scrollPos.X + 40 * pnlScrollview.Width / 100;          // 40% de la largeur
                int xmax = scrollPos.X + 65 * pnlScrollview.Width / 100;        // 65% de la largeur
                scrollDist = x_shade - x_view;                                  // abscisse note active - 40% largeur

                
                if (scrollGradually)
                {
                    if (x_shade > xmax)
                        scrollDist = (x_shade - x_view) / 3;
                    else if (x_shade > x_view)
                        scrollDist = (x_shade - x_view) / 6;
                }
                

                // FAB: je veux scroller tel que le milieu du panel soit la note jouée
                int x_middle = scrollPos.X + (pnlScrollview.Width / 2);
                //scrollDist = (x_shade - x_middle)/3;
                //scrollDist = (x_shade - x_middle) / 6;
                //scrollDist = (x_shade - x_middle)/2;
                scrollDist = (x_shade - x_middle);
                //Console.Write("x_shade = " + x_shade.ToString() + "\r");
   
                

                newPos = new Point(scrollPos.X + scrollDist, scrollPos.Y);
                
                if (newPos.X < 0)
                {
                    newPos.X = 0;
                }
            }
            
            pnlScrollview.AutoScrollPosition = newPos;

        }

      

        #endregion scroll

        #region edit notes 

        private void ShowFrmNoteEdit()
        {

            if (Application.OpenForms["frmNoteEdit"] == null)
            {
                
                FrmNoteEdit = new UI.frmNoteEdit(this);
                FrmNoteEdit.Show();
                FrmNoteEdit.Refresh();                

                Focus();
            }
        }
      

        /// <summary>
        /// Close note modification windows
        /// </summary>
        private void CloseFrmNoteEdit()
        {            
            // ferme le formulaire frmNoteEdit
            if (Application.OpenForms.OfType<UI.frmNoteEdit>().Count() > 0)
            {
                Application.OpenForms["frmNoteEdit"].Close();
            }            
        }


        #endregion


        #region Dispose

        protected override void Dispose(bool disposing)
        {
            // Ferme le formulaire frmNoteEdit
            CloseFrmNoteEdit();
            
            base.Dispose(disposing);
        }

        #endregion


    }
}
