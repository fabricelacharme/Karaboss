#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using ChordsAnalyser;
using System.Diagnostics;
using Sanford.Multimedia.Midi.Score;
using Karaboss.Resources.Localization;
using System.IO;
using System.Text.RegularExpressions;
using MusicXml;
using MusicTxt;
using System.Linq;
using ChordAnalyser.UI;
using Karaboss.Search;
using System.Text;
using Karaboss.Lyrics;
using MusicXml.Domain;
using System.Xml;
using FlShell.Interop;

namespace Karaboss
{

    public partial class frmPlayer : Form
    {
        // FAB 2710
        MusicXmlReader MXmlReader; // = new MusicXmlReader();
        MusicTxtReader MTxtReader; 
        MusicTxtWriter MTxtWriter;

        public bool bfilemodified = false;

        #region Lyrics declaration

        // Lyrics management
        public LyricsMgmt myLyricsMgmt;
        private bool bHasLyrics = false;  
        private bool bShowChords = false;

        // SlideShow directory
        public string dirSlideShow;

        #endregion Lyrics declaration

        // FAB 20/03/2021
        private class _reglages
        {
            public int volume = 100;
            public int pan = 64;
            public int reverb = 0;
            public int channel = 0;
            public int patch = 0;
            public bool muted = false;
            public bool maximized = true;
        }
        private List<_reglages> lstTrkReglages;
        private _reglages TrkReglages;
        private bool bReglageChanged = false;


        private class _channels
        {
            public bool muted = false;

        }
        private List<_channels> lstChannels;
        private _channels ChannelReglages;

        #region SheetMusic declarations

        //private SheetMusic sheetmusic;                  /* The Control which displays the sheet music */
        private SheetMusic sheetmusic;
        private MidiOptions options;

        private float zoom = 1.0f;                      /* The current zoom level (1.0 == 100%) */

        private double currentPulseTime;    /** Time (in pulses) music is currently at */
        private double prevPulseTime;       /** Time (in pulses) music was last at */
        private double pulsesPerMsec;       /** The number of pulses per millisec */
        private double currentTime;

        #endregion SheetMusic declarations


        #region controls
        // Creation dynamique de controles 
        //private Sanford.Multimedia.Timers.Timer timerBalls;
        private Panel pnlTracks;        // panel left
        private Panel pnlScrollView;    // panel right    
        private Panel pnlHScroll;       // panel horizontal

        private NoSelectVScrollBar vScrollBar;        
        private HScrollBar hScrollBar;

        private TrkControl.TrackControl pTrack;
        private TrkControl.TrackControl draggedTrack;
        private TrkControl.TrackControl droppedTrack;

        #endregion


        #region Player States

        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,
            NextSong,           // select next song of a playlist
            Waiting,            // count down running between 2 songs of a playlist
            WaitingPaused,      // count down paused between 2 songs of a playlist
            LaunchNextSong      // pause between 2 songs of a playlist
        }
        private PlayerStates PlayerState;

        #endregion


        #region Edit status

        /// <summary>
        /// All kind of notes
        /// </summary>
        private enum NoteValues
        {
            None,
            Gomme,
            Ronde,
            Blanche,
            Noire,
            Croche,
            DoubleCroche,
            TripleCroche,
            QuadrupleCroche
        }
        private NoteValues NoteValue;        

        /// <summary>
        /// All kind of alterations
        /// </summary>
        private enum Alterations
        {
            None,
            Dot,
            Diese,
            Bemol,
            Becarre
        }
        private Alterations Alteration;

        #endregion


        #region private decl

        #region External lyrics separators

        private string m_SepLine = "/";
        private string m_SepParagraph = "\\";

        #endregion

        private int TempoDelta = 100;
        private int TempoOrig = 0;        
        
        private int TransposeDelta = 0;
        private int TransposeOrig = 0;


        //private bool bMuted = false;
        private bool bVolumed = false;
        private bool bPlayNow = false;        
        private bool bSequencerAlwaysOn = false;
        private bool bForceShowSequencer = false;
        private bool bKaraokeAlwaysOn = true;
        
        private bool ScrollVert = false;
        private bool bShowVScrollBar = false;
        private bool bShowHScrollBar = false;
        private bool scrolling = false;
        private bool closing = false;
        private bool bClosingRequired = false;
        private bool loading = false; // loading file in progress
        private bool bEditScore = false;
        private bool bEnterNotes = false;

        // Playlists
        private Playlist currentPlaylist;
        private PlaylistItem currentPlaylistItem;        
                        
        private int iStaffHeightMaximized = 150; // = SheetMusic.staffH; 148 en réalité
        //private int iStaffHeightMinimized = 25;  // 23 en réalité
        
        // Dimensions
        private int leftWidth = 179;
        private int SimplePlayerWidth = 520;
        private int SimplePlayerHeight = 194;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;        
        private double _ppqn;
        private int _tempo;
        private int _tempoplayed;
        private int _measurelen;
        

        // Load Instruments list
        private List<string> lsInstruments = MidiFile.LoadInstruments();
        //BEAT
        private int beat = 0;
        private int BeatIntervall = 0;

        // Enter notes
        private int octave = 6;        

        // Output device
        private OutputDevice outDevice;
        //private int outDeviceID = 0;
        private int outDeviceProcessId;
        
        private string songRoot;

        // forms        
        private frmExplorer frmExplorer;
        private frmLyric frmLyric;
        private frmLoading frmLoading;
        private frmPianoRoll frmPianoRoll;
        private frmPianoTraining frmPianoTraining;     
        private frmModifyTempo frmModifyTempo;
        private int NumInstance = 1;

        // To wait between 2 songs (playlists)
        private int w_tick = 0;
        private int w_wait = 10;        
        private int bouclestart = 0;
        private int newstart = 0;
        private int laststart = 0;      // Start time to play
        private int lastscroll = 0;
        private int lastbluestart = 0; // Vertical blue line
        private int nbstop = 0;

        // Play stop notes
        private bool on = false;
        private int playedNote = -1;
        private int playedStaff = -1;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        // FAB 06/07/2024
        private System.Text.Encoding _encoding = System.Text.Encoding.ASCII;

        #endregion

        /// <summary>
        /// Initializations
        /// </summary>
        /// <param name="FileName"></param>        
        public frmPlayer(int numinstance, string FileName, Playlist myPlayList, bool bplay, OutputDevice outputDevice, string songsDir)
        {
            InitializeComponent();

            NumInstance = numinstance;

            // Load saved line and paragraph separators
            m_SepLine = Karaclass.m_SepLine;
            m_SepParagraph = Karaclass.m_SepParagraph;

            songRoot = songsDir;

            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);

            this.MouseWheel += new MouseEventHandler(FrmPlayer_MouseWheel);
            
            outDevice = outputDevice;           

            // If true, launch player
            bPlayNow = bplay;

            // if Edit, force show sequencer 
            if (bPlayNow == false)
                bForceShowSequencer = true;

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            // Allow form keydown
            this.KeyPreview = true;

            // Display of peak level volume
            Init_peakLevel();          
           
            // Playlist
            #region playlists
            if (myPlayList != null)
            {
                currentPlaylist = myPlayList;
                // Search file to play with its filename
                currentPlaylistItem = currentPlaylist.Songs.Where(z => z.File == MIDIfileFullPath).FirstOrDefault();

                MIDIfileFullPath = currentPlaylistItem.File;
                MIDIfileName = currentPlaylistItem.Song; 

                lblPlaylist.Visible = true;
                int idx = currentPlaylist.SelectedIndex(currentPlaylistItem) + 1;
                lblPlaylist.Text = "PLAYLIST: " + idx + "/" + currentPlaylist.Count;

            }
            else
            {
                lblPlaylist.Visible = false;
            }
            #endregion

            // FAB 28/08
            // Reset plLyrics            
            //plLyrics = new List<plLyric>();            

            // Volume de chaque piste
            lstTrkReglages = new List<_reglages>();
            lstChannels = new List<_channels>();

            // Zoom
            zoom = 1.0f;

            // Lyrics
            timer2.Interval = 50;

            bShowChords = Properties.Settings.Default.bShowChords;

        }      

        #region SheetMusic

        /// <summary>
        /// Display scores
        /// </summary>
        private void DisplayScores()
        {
            DrawingControl.SuspendDrawing(this);

            DrawControls();           
            
            SetTitle(MIDIfileName);

            // Display scores
            RedrawSheetMusic();
            
            // ScrollBar properties
            SetScrollBarValues();

            DrawingControl.ResumeDrawing(this);

            MidiOptions options2 = new MidiOptions(sequence1);
            

            if (sequence1.Time != null)
            {
                double inverse_tempo = 1.0 / sequence1.Time.Tempo;
                double inverse_tempo_scaled = inverse_tempo;

                options2.tempo = (int)(1.0 / inverse_tempo_scaled);
                pulsesPerMsec = sequence1.Time.Quarter * (1000.0 / options2.tempo);
            }
        }


        /// <summary>
        /// Set Title of the form
        /// </summary>
        private void SetTitle(string displayName)
        {
            if (displayName != null)
            {
                displayName = displayName.Replace("__", ": ");
                displayName = displayName.Replace("_", " ");
            }
            if (NumInstance > 1)
                Text = "Karaboss PLAYER (" + NumInstance + ") - " + displayName;
            else
                Text = "Karaboss PLAYER - " + displayName;
        }
        
        /** The Sheet Music needs to be redrawn.  Gather the sheet music
          * options from the menu items.  Then create the sheetmusic
          * control, and add it to this form. Update the MidiPlayer with
          * the new midi file.
          */
        public void RedrawSheetMusic()
        {

            /* Create a new SheetMusic Control from the midifile */
            Cursor = Cursors.AppStarting;

            bool bEditMode = false;
            if (sheetmusic != null)
                bEditMode = sheetmusic.bEditMode;

            if (sheetmusic != null)            
                sheetmusic.Dispose();
                        
            options = GetMidiOptions(ScrollVert);

            #region create new sheet music
            sheetmusic = new SheetMusic(sequence1, options, iStaffHeightMaximized)
            {
                bEditMode = bEditMode,
                Velocity = Karaclass.m_Velocity,
            };
           
            sheetmusic.FileModified += new SheetMusic.FileModifiedEventHandler(Score_Modified);
            sheetmusic.WidthChanged += new SheetMusic.WidthChangedEventHandler(ScoreWidth_Changed);

            // Event handler double click on track            
            sheetmusic.OnSMMouseDoubleClick += new SheetMusic.smMouseDoubleClickEventHandler(Track_DoubleClick);
            sheetmusic.OnSMMouseDoubleClickTempo += new SheetMusic.smMouseDoubleClickTempoEventHandler(Tempo_DoubleClick);

            // Contextual menu on sheetmusic
            sheetmusic.MnuPianoRollClick += new SheetMusic.mnuPianoRollClickEventHandler(PianoRoll_Required);
           
            sheetmusic.OnSMMouseDown += new SheetMusic.smMouseDownEventHandler(Track_MouseDown);
            sheetmusic.OnSMMouseUp += new SheetMusic.smMouseUpEventHandler(Track_MouseUp);
            sheetmusic.OnSMMouseMove += new SheetMusic.smMouseMoveEventHandler(Track_MouseMove);            

            sheetmusic.CurrentNoteChanged += new SheetMusic.CurrentNoteChangedEventHandler(SheetMusic_CurrentNoteChanged);
            sheetmusic.CurrentTrackChanged += new SheetMusic.CurrentTrackChangedEventHandler(SheetMusic_CurrentTrackChanged);

            sheetmusic.CurrentDefaultVelocityChanged += new SheetMusic.CurrentDefaultVelocityChangedEventHandler(SheetMusic_CurrentDefaultVelocityChanged);

            sheetmusic.SetZoom(zoom);
            sheetmusic.Parent = pnlScrollView;
            #endregion

            BackColor = Color.White;
            pnlScrollView.BackColor = Color.White;
            
            if ( ScrollVert == false)
            {
                pnlTracks.Height = sequence1.tracks.Count * iStaffHeightMaximized * Convert.ToInt32(zoom);
                pnlScrollView.Height = pnlTracks.Height;
            }
            else
            {
                pnlTracks.Height = sheetmusic.Height;
                pnlScrollView.Height = sheetmusic.Height;
            }
            SetStartVLinePos(0);
            SetTimeVLinePos(0);

            Cursor = Cursors.Default; 
        }

        
        /// <summary>
        /// Default velocity value was changed in sheetmusic
        /// </summary>
        /// <param name="velocity"></param>
        private void SheetMusic_CurrentDefaultVelocityChanged(int velocity)
        {
            Karaclass.m_Velocity = velocity;
            Properties.Settings.Default.Velocity = velocity;
            Properties.Settings.Default.Save();            
        }


        /// <summary>
        /// Event: score was modified, recalculate scrollbars
        /// </summary>
        /// <param name="Width"></param>
        private void ScoreWidth_Changed(int Width)
        {
            SetScrollBarValues();
        }

        /// <summary>
        /// Contextual menu on sheetmusic: display PianoRoll window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="staffnum"></param>
        private void PianoRoll_Required(object sender, EventArgs e, int staffnum, int ticks)
        {
            //float pos = sequence1.GetLength() * (float)hScrollBar.Value / (float)hScrollBar.Maximum;
            DisplayPianoRoll(staffnum, MIDIfileFullPath, ticks);
        }

        /// <summary>
        /// Event: score was modified, set flag
        /// </summary>
        /// <param name="sender"></param>
        private void Score_Modified(object sender)
        {
            FileModified();
        }


        /// <summary>
        /// Refresh display
        /// </summary>
        public void RefreshDisplay()
        {
            int numstaff = sheetmusic.CurrentNote.numstaff;
            int note = sheetmusic.CurrentNote.midinote.Number;
            int lastnote = sheetmusic.CurrentNote.lastnote;
            float ticks = sheetmusic.CurrentNote.midinote.StartTime; 
                
            sheetmusic.Refresh();
            
            this.Focus();

            // Display song duration
            DisplaySongDuration();
            DisplayFileInfos();
            DisplayLyricsInfos();

            // Set Current Note
            sheetmusic.UpdateCurrentNote(numstaff, note, ticks, false);
            sheetmusic.CurrentNote.lastnote = lastnote;


            // Dimensions            
            pnlTracks.Height = sequence1.tracks.Count * iStaffHeightMaximized * Convert.ToInt32(zoom);
            pnlScrollView.Height = pnlTracks.Height;

            SetStartVLinePos(0);
            SetTimeVLinePos(0);

            SetScrollBarValues();
        }

        /// <summary>
        /// Set midi options choosen in menus
        /// </summary>
        /// <param name="scrollvert"></param>
        /// <returns></returns>
        private MidiOptions GetMidiOptions(bool scrollvert)
        {
            options = new MidiOptions(sequence1) {
                transpose = 0,
                key = -1,
                time = sequence1.Time,
                scrollVert = scrollvert,
                // Display score if bSequencerAlwaysOn or bForceShowSequencer
                bVisible = bSequencerAlwaysOn | bForceShowSequencer,
               
            };

            return options;
        }

        #endregion MidiSheetMusic


        # region track stuff
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
                TrkReglages.maximized = track.Maximized;
                TrkReglages.volume = track.Volume;
                TrkReglages.pan = track.Pan;
                TrkReglages.reverb = track.Reverb;
                TrkReglages.muted = false;
                TrkReglages.channel = track.MidiChannel;
                TrkReglages.patch = track.ProgramChange;
                lstTrkReglages.Add(TrkReglages);
            }

            bReglageChanged = false;

            // Mute Channel
            for (int i = 0; i < 16; i++)
            {
                ChannelReglages = new _channels();
                ChannelReglages.muted = false;
                lstChannels.Add(ChannelReglages);
            }
        }

        /// <summary>
        /// Mouse move event => draw help grid to seize notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Track_MouseMove(object sender, EventArgs e)
        {
            sheetmusic.bShowHelpGrid = (NoteValue != NoteValues.None);           
        }

        /// <summary>
        /// Mouse down : add a new note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="staffnum"></param>
        /// <param name="note"></param>
        /// <param name="ticks"></param>
        private void Track_MouseDown(object sender, MouseEventArgs e, int staffnum, int note, float ticks)
        {                     
            #region draw blue vertical line
            if (PlayerState != PlayerStates.Stopped || bEnterNotes == false)
            {
                if (bEditScore == false && PlayerState != PlayerStates.Playing)
                {
                    newstart = Convert.ToInt32(ticks);
                    if (newstart > 0)
                        BtnStatus();

                    // ticks for new start
                    laststart = newstart;

                    bouclestart = laststart;
                    lastscroll = hScrollBar.Value;


                    // Draw blue vertical line on new start
                    lastbluestart = e.X;
                    SetStartVLinePos(lastbluestart);

                }

                return;
            }
            #endregion guard

            float time = ticks / sequence1.Division;
            Track track = sequence1.tracks[staffnum];

            float duration = 0;

            if (Alteration == Alterations.Diese)
                note++;
            else if (Alteration == Alterations.Bemol)
                note--;


            #region edit

            SelectOctave(note);

            // delete note
            if (NoteValue == NoteValues.Gomme)
            {
                // Delete a note on a track         
                sheetmusic.DeleteNote(staffnum, note, ticks);

            }
            else
            {

                // Retrieve new note duration according to button selected
                duration = GetNewNoteDuration();            
               
                // Add new note
                int starttime = Convert.ToInt32(time * sequence1.Division);
                int dur = Convert.ToInt32(duration * sequence1.Division);
                int channel = track.MidiChannel;
                int notenumber = note;

                int velocity = Karaclass.m_Velocity;

                MidiNote mdnote = new MidiNote(starttime, channel, notenumber, dur, velocity, false);

                if (sheetmusic.AddNote(track, mdnote) > 0)
                {
                    PlayNote(note, staffnum);
                    sheetmusic.UpdateCurrentNote(sheetmusic.CurrentNote.numstaff, mdnote.Number, mdnote.StartTime, true);
                }                
            }
            
            // Redraw scores
            RefreshDisplay();            

            #endregion edit

        }

        private void PlayNote(int note, int staffnum)
        {
            StopNote();

            if (on == false)
            {
                on = true;
                playedNote = note;
                playedStaff = staffnum;

                if (staffnum < sequence1.tracks.Count && note >= 0 && note <= 127)
                {
                    Track track = sequence1.tracks[staffnum];
                    //Console.Write("\nPlay note: " + note);
                    outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, track.MidiChannel, note, 127));
                }
            }
        }

        private void StopNote()
        {
            if (on && playedNote >= 0 && playedNote <= 127)
            {                
                Track track = sequence1.tracks[playedStaff];
                outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, track.MidiChannel, playedNote, 0));
                on = false;
            }
        }

        /// <summary>
        /// Mouse up : stop note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="staffnum"></param>
        /// <param name="note"></param>
        /// <param name="ticks"></param>
        private void Track_MouseUp(object sender, EventArgs e, int staffnum, int note, float ticks)
        {
            #region Guard

            if (PlayerState != PlayerStates.Stopped)
            {
                return;
            }

            #endregion
            
            StopNote();          
        }    

        /// <summary>
        /// Double Click: display piano roll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="staffnum"></param>
        private void Track_DoubleClick(object sender, EventArgs e, int staffnum, float pos)
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped)
            {
                return;
            }
            #endregion guard            

            // Launch PianoRoll Window in order to display this track
            DisplayPianoRoll(staffnum, MIDIfileFullPath, (int)pos);
        }

        /// <summary>
        /// Up/Down on same note
        /// </summary>
        /// <param name="KeyCode"></param>
        private void Key_UpDownNote(string KeyCode)
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped || sheetmusic == null)
            {
                return;
            }
            #endregion guard            

            // First bEnterNotes, because bEditScore is also true...
            if (bEnterNotes && sheetmusic.CurrentNote != null && sheetmusic.CurrentNote.midinote.Duration != 0)
            {
                #region updown current note
                sheetmusic.SheetMusic_UpDownCurrentNote(KeyCode);
                
                // alter note
                int numstaff = sheetmusic.CurrentNote.numstaff;                
                int note = sheetmusic.CurrentNote.midinote.Number;                
                               
                // Play note                
                PlayNote(note, numstaff);
                FileModified();
                RefreshDisplay();                
                SelectOctave(note);
                #endregion updown current note
            }
            else if (bEditScore)
            {
                #region updown selected notes

                sheetmusic.SheetMusic_UpDownSelectedNote(KeyCode);
                FileModified();
                RefreshDisplay();
                
                #endregion updown selected notes
            }
        }

        /// <summary>
        /// Left key: select previous note
        /// </summary>
        private void Key_Left()
        {
            //Console.Write("\nfrmPlayer Key Left"); 

            // alter note
            int numstaff = sheetmusic.CurrentNote.numstaff;
            Track track = sequence1.tracks[numstaff];
            int note = sheetmusic.CurrentNote.midinote.Number;
            int ticks = Convert.ToInt32(sheetmusic.CurrentNote.midinote.StartTime);

            // Set new current note = note before            
            MidiNote n = track.getPrevNote(note, ticks);
            // si il n'y a pas de note précédante on reste sur la même
            if (n != null)
            {                
                sheetmusic.UpdateCurrentNote(numstaff, n.Number, n.StartTime, true);

                RefreshDisplay();                
                ScrollTo(n.StartTime);
            }            
        }


        /// <summary>
        /// Event: key right on keyboard
        /// </summary>
        private void Key_Right()
        {
            // alter note
            int numstaff = sheetmusic.CurrentNote.numstaff;
            Track track = sequence1.tracks[numstaff];
            int note = sheetmusic.CurrentNote.midinote.Number;
            float ticks = sheetmusic.CurrentNote.midinote.StartTime;

            // Set new current note = note before            
            MidiNote n = track.getNextNote(note, Convert.ToInt32(ticks));
            // si il n'y a pas de note suivante : on reste sur la même
            if (n != null)
            {                
                sheetmusic.UpdateCurrentNote(numstaff, n.Number, n.StartTime, true);

                RefreshDisplay();
                ScrollTo(n.StartTime);
            }            
        }

        /// <summary>
        /// Create new notes with keyboard
        /// </summary>
        /// <param name="nnote"></param>
        private void Key_AddNote(int nnote)
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped || bEnterNotes == false)
            {
                return;
            }
            #endregion guard            

            // note number according to current octave
            int lastnote = sheetmusic.CurrentNote.midinote.Number;
            int min = 100;
            int newoctave = 0;            

            // 3 choices
            // Take min of lastnote - a, lastnote - b, lastnote - c ?
            int a = (nnote + (octave - 1) * 12);
            if (Math.Abs(lastnote - a) < min)
            {
                min = Math.Abs(lastnote - a);
                newoctave = octave - 1;
            }
            
            int b = (nnote + octave * 12);
            if (Math.Abs(lastnote - b) < min) 
            {
                min = Math.Abs(lastnote - b);
                newoctave = octave;
            }
            
            int c = (nnote + (octave + 1) * 12);
            if (Math.Abs(lastnote - c) < min)
            {
                min = Math.Abs(lastnote - c);
                newoctave = octave + 1;
            }

            int newnote = nnote + newoctave * 12;                                    

            // Retrieve new note duration according to button selected
            float newduration = GetNewNoteDuration();

            // build new midi note to create
            MidiNote mdnote = sheetmusic.BuildNewNote(newnote, newduration);


            int numstaff = sheetmusic.CurrentNote.numstaff;            
            Track track = sequence1.tracks[numstaff];

            // Add new note
            if (sheetmusic.AddNote(track, mdnote) > 0)
            {
                PlayNote(mdnote.Number, numstaff);
                sheetmusic.UpdateCurrentNote(sheetmusic.CurrentNote.numstaff, mdnote.Number, mdnote.StartTime, true);
                
                UpdateMidiTimes();
                DisplaySongDuration();

                // Redraw scores
                FileModified();
                RefreshDisplay();
                ScrollTo(mdnote.StartTime);
            }
        }
        
        /// <summary>
        /// Delete current note with keyboard
        /// </summary>
        private void Key_DelNote()
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped || bEnterNotes == false)
            {
                return;
            }
            #endregion guard

            sheetmusic.DeleteNote(sheetmusic.CurrentNote.numstaff, sheetmusic.CurrentNote.midinote.Number, sheetmusic.CurrentNote.midinote.StartTime);
            UpdateMidiTimes();
            DisplaySongDuration();

            ScrollTo(sheetmusic.CurrentNote.midinote.StartTime);

            FileModified();

            // Redraw scores
            RefreshDisplay();                     
        }


        /// <summary>
        /// Retrieve new note duration according to button selected
        /// </summary>
        /// <returns></returns>
        private float GetNewNoteDuration()
        {           
            switch (NoteValue)
            {
                case NoteValues.Ronde:
                    if (Alteration == Alterations.Dot)
                        return 6;
                    else
                        return 4;

                case NoteValues.Blanche:
                    if (Alteration == Alterations.Dot)
                        return 3;
                    else
                        return 2;

                case NoteValues.Noire:
                    if (Alteration == Alterations.Dot)
                        return 1.5f;
                    else
                        return 1;

                case NoteValues.Croche:
                    if (Alteration == Alterations.Dot)
                        return 0.75f;
                    else
                        return 0.5f;

                case NoteValues.DoubleCroche:
                    if (Alteration == Alterations.Dot)
                        return 0.375f;
                    else
                        return 0.25f;

                case NoteValues.TripleCroche:
                    if (Alteration == Alterations.Dot)
                        return 0.1825f;
                    else
                        return 0.125f;

                case NoteValues.QuadrupleCroche:
                    if (Alteration == Alterations.Dot)
                        return 0.09375f;
                    else
                        return 0.0625f;

                default:
                    return 1;

            }
        }
               
        /// <summary>
        /// File was modified
        /// </summary>
        public void FileModified()
        {
            bfilemodified = true;
            string fName = MIDIfileName;
            if (fName != null && fName != "")
            {
                string fExt = Path.GetExtension(fName);             // Extension
                fName = Path.GetFileNameWithoutExtension(fName);    // name without extension

                string fShortName = fName.Replace("*", "");
                if (fShortName == fName)
                    fName = fName + "*";

                fName = fName + fExt;
                SetTitle(fName);
            }
        }
     
        /// <summary>
        /// Adapt current octave to notes entered
        /// </summary>
        /// <param name="note"></param>
        private void SelectOctave(int note)
        {
            if (note >= octave*12 && note <= 11 + octave*12 )
                return;
            
            if (note > 11 + octave * 12)
            {
                do
                {
                    if (octave < 12)
                        octave++;
                    else
                        break;
                } while (note > 11 + octave * 12);
            }
            else
            {
                do
                {
                    if (octave > 0)
                        octave--;
                    else
                        break;
                } while (note > 11 + octave * 12);
            }
        }

        #endregion track stuff


        #region Displays objects

        /// <summary>
        /// Draw panels pnlScrollView & pnlTracks
        /// </summary>
        private void DrawControls()
        {
            //important:   use "leftWidth" to position controls                       

            #region volume

            sldMainVolume.Maximum = 130;    // Closer to 127
            sldMainVolume.Minimum = 0;
            sldMainVolume.ScaleDivisions = 13;
            sldMainVolume.Value = 104;
            sldMainVolume.SmallChange = 13;
            sldMainVolume.LargeChange = 13;
            sldMainVolume.MouseWheelBarPartitions = 10;

            sldMainVolume.Left = 249;
            sldMainVolume.Top = 25;
            sldMainVolume.Width = 24;
            sldMainVolume.Height = 80;


            lblMainVolume.Text = String.Format("{0}%", 100 * sldMainVolume.Value / sldMainVolume.Maximum);

            #endregion


            #region left
            // ------------------------------------------------------------
            // then the tracks...
            // Draw a panel on the left to display the track controllers
            // ------------------------------------------------------------
            if (pnlTracks != null)
                pnlTracks.Dispose();

            pnlTracks = new Panel() {
                Parent = pnlMiddle,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = false,
                AutoSize = false,
                BackColor = Color.Black,
            };
            pnlTracks.SetBounds(0, 0, leftWidth, 400);

            #endregion left


            #region right

            // ------------------------------------------------------------
            // first the scores
            // Draw a panel on the right to display the scores
            // ------------------------------------------------------------
            if (pnlScrollView != null)
                pnlScrollView.Dispose();

            pnlScrollView = new Panel()
            {
                Parent = pnlMiddle,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Gray,
                AutoScroll = false,
                AutoSize = false,
            };
            pnlScrollView.Size = new Size(pnlScrollView.Parent.ClientSize.Width - leftWidth, pnlScrollView.Parent.ClientSize.Height);
            pnlScrollView.SetBounds(leftWidth, 0, 200, 400);
            pnlMiddle.Controls.Add(pnlScrollView);


            // Horizontal scrollbar
            if (pnlHScroll != null)
                pnlHScroll.Dispose();

            pnlHScroll = new Panel() {
                Parent = pnlMiddle,
                AutoScroll = false,
                AutoSize = false,
                BackColor = Color.Black,
                Size = new Size(pnlMiddle.Width - pnlTracks.Width, 17),
                Dock = DockStyle.Bottom,
            };
            pnlMiddle.Controls.Add(pnlHScroll);


            hScrollBar = new HScrollBar() {
                Parent = pnlHScroll,
                Left = leftWidth,
                Top = 0,
                Minimum = 0,
            };
            pnlHScroll.Controls.Add(hScrollBar);

            hScrollBar.Scroll += new ScrollEventHandler(HScrollBar_Scroll);
            hScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);


            // ========================
            // Bring to front
            // ========================
            pnlTracks.BringToFront();
            pnlHScroll.BringToFront();


            // Vertical Scrollbar
            if (vScrollBar != null)
                vScrollBar.Dispose();

            vScrollBar = new NoSelectVScrollBar() {
                Parent = pnlMiddle,
                Top = 0,
                Minimum = 0,
                TabStop = false,
            };

            pnlMiddle.Controls.Add(vScrollBar);
            vScrollBar.Dock = DockStyle.Right;
            vScrollBar.BringToFront();

            vScrollBar.Scroll += new ScrollEventHandler(VScrollBar_Scroll);
            vScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            #endregion right

            // Red bar
            TimeVLine.Height = pnlScrollView.Height;
            TimeVLine.Left = pnlScrollView.Left;
            TimeVLine.Top = pnlScrollView.Top;

            // Blue bar
            TimeStartVLine.Height = pnlScrollView.Height;
            TimeStartVLine.Left = pnlScrollView.Left;
            TimeStartVLine.Top = pnlScrollView.Top;

            SetScrollBarValues();            
        }
       

        /// <summary>
        /// Display track controls
        /// </summary>
        private void DisplayTrackControls()
        {
            int nbTrk = sequence1.tracks.Count;
            int nbTrkNotes = 0;
            //int yOffset = 1;

            this.Cursor = Cursors.WaitCursor;
            DrawingControl.SuspendDrawing(this);

            // Remove all existing track controls
            bool oneMoreTime = true;
            while (oneMoreTime)
            {
                Control toDelete = null;
                oneMoreTime = false;
                foreach (Control item in pnlTracks.Controls)
                {
                    if (item.GetType() == typeof(TrkControl.TrackControl))
                    {
                        toDelete = item;
                        break;
                    }
                }
                if (toDelete != null)
                {
                    pnlTracks.Controls.Remove(toDelete);
                    oneMoreTime = true;
                }
            }

            for (int i = 0; i < nbTrk; i++)
            {                
                Track track = sequence1.tracks[i];
                nbTrkNotes++;
                // Add track control
                AddTrackControl(track, i);
            }

            // Ajust height of panel according to number of controls
            pnlTracks.Height = sequence1.tracks.Count * iStaffHeightMaximized * Convert.ToInt32(zoom);

            pnlScrollView.Height = pnlTracks.Height;

            SetScrollBarValues();

            DrawingControl.ResumeDrawing(this);
            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Things to do when song is loaded
        /// </summary>
        private void DisplaySongDuration()
        {
            // Affichage du BEAT
            lblBeat.Text = "1|" + sequence1.Numerator;
          
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));

            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);
            
        }

        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            TempoOrig = _tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _bpm = GetBPM(_tempo);

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;

                foreach (Track track in sequence1.tracks)
                { 
                    track.MeasureLength = _measurelen;
                }

            }
        }

        /// <summary>
        /// Calculate BPM
        /// </summary>
        /// <param name="tempo"></param>
        /// <returns></returns>
        private int GetBPM(int tempo)
        {
            // see http://midi.teragonaudio.com/tech/midifile/ppqn.htm
            const float kOneMinuteInMicroseconds = 60000000;
            float kTimeSignatureNumerator = (float)sequence1.Numerator; 
            float kTimeSignatureDenominator = (float)sequence1.Denominator;

            //float BPM = (kOneMinuteInMicroseconds / (float)tempo) * (kTimeSignatureDenominator / 4.0f);            
            float BPM = kOneMinuteInMicroseconds / (float)tempo;

            return (int)BPM;
        }

        /// <summary>
        /// Display informations on midi file
        /// </summary>
        private void DisplayFileInfos()
        {                        
            // BEAT
            beat = 1;
           
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));

            string tx;
            tx = string.Format("Division: {0}", _ppqn) + "\n";
            tx += string.Format("Tempo: {0}", _tempo) + "\n";
            tx += string.Format("BPM: {0}", _bpm) + "\n";
            tx += string.Format("TotalTicks: {0}", _totalTicks) + "\n";
            tx += "Duree: " + string.Format("{0:00}:{1:00}", Min, Sec) + "\n";

            if (sequence1.Format != sequence1.OrigFormat)
                tx += "Midi Format: " + sequence1.Format.ToString() + " (Orig. Format: " + sequence1.OrigFormat.ToString() + ")";
            else
                tx += "Midi Format: " + sequence1.Format.ToString();

            lblInfosF.Text = tx;
        }

        private void DisplayFileInfos(int tempo)
        {
            // BEAT
            beat = 1;
            int bpm = GetBPM(tempo);

            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));

            string tx;
            tx = string.Format("Division: {0}", _ppqn) + "\n";
            tx += string.Format("Tempo: {0}", tempo) + "\n";
            tx += string.Format("BPM: {0}", bpm) + "\n";
            tx += string.Format("TotalTicks: {0}", _totalTicks) + "\n";
            tx += "Duree: " + string.Format("{0:00}:{1:00}", Min, Sec) + "\n";

            if (sequence1.Format != sequence1.OrigFormat)
                tx += "Midi Format: " + sequence1.Format.ToString() + " (Orig. Format: " + sequence1.OrigFormat.ToString() + ")";
            else
                tx += "Midi Format: " + sequence1.Format.ToString();

            lblInfosF.Text = tx;
        }

        #endregion Displays objects


        #region scroll events

        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Verticall scroll of panel kept
            pnlTracks.Top = -vScrollBar.Value;
            pnlScrollView.Top = -vScrollBar.Value;            
        }

        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            pnlTracks.Top = -vScrollBar.Value;
            pnlScrollView.Top = -vScrollBar.Value;            
        }

        private void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            sheetmusic.OffsetX = hScrollBar.Value;
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {            
            sheetmusic.OffsetX = hScrollBar.Value;
        }
    

        /// <summary>
        /// Set scrollBar maxi values
        /// </summary>
        private void SetScrollBarValues()
        {
            if (pnlScrollView == null || sheetmusic == null)
                return;

            int hH = 0;
            int vW = 0;

            // Width of sheetmusic
            int W = sheetmusic.MaxStaffWidth;
            // display width
            int wMiddle = pnlMiddle.Width - pnlTracks.Width;

            bool bShowHScrollBarIndetermined = false;

            // If display width > sheetmusic width => remove horizontal scrollbar
            if (wMiddle > W + vScrollBar.Width)
                bShowHScrollBar = false;
            else if (wMiddle < W)
                bShowHScrollBar = true;
            else
                bShowHScrollBarIndetermined = true;


            bool bShowVScrollBarIndetermined = false;

            // If display height > sheetmusic height => remove vertical scrollbar
            if (pnlMiddle.Height > pnlScrollView.Height + hScrollBar.Height)
                bShowVScrollBar = false;
            else if (pnlMiddle.Height < pnlScrollView.Height)
                bShowVScrollBar = true;
            else
                bShowVScrollBarIndetermined = true;


            if (bShowVScrollBarIndetermined && bShowHScrollBarIndetermined)
            {
                bShowHScrollBar = false;
                bShowVScrollBar = false;
            }
            else if (bShowVScrollBarIndetermined && pnlMiddle.Height > pnlScrollView.Height)
                bShowVScrollBar = false;
            else if (bShowHScrollBarIndetermined && wMiddle > W)
                bShowHScrollBar = false;


            if (bShowVScrollBar && wMiddle - vScrollBar.Width < W)
                bShowHScrollBar = true;

            if (bShowHScrollBar && pnlMiddle.Height - hScrollBar.Height < pnlScrollView.Height)
                bShowVScrollBar = true;



            if (bShowVScrollBar == false)
            {
                vScrollBar.Visible = false;
                pnlTracks.Top = 0;
                pnlScrollView.Top = 0;
            }
            else
            {
                vScrollBar.Visible = true;
                vScrollBar.Left = pnlMiddle.Width - vScrollBar.Width;
                if (bShowHScrollBar)
                    vScrollBar.Height = pnlMiddle.Height - hScrollBar.Height;
                else
                    vScrollBar.Height = pnlMiddle.Height;

                // Aggrandissement vertical de la fenetre = > la scrollbar verticale doit se positionner en bas
                if (sheetmusic.Height - vScrollBar.Value < pnlMiddle.Height)
                    vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange;
            }


            if (bShowHScrollBar == false)
            {
                pnlHScroll.Visible = false;
                pnlTracks.Left = 0;
                pnlScrollView.Left = pnlTracks.Width;
            }
            else
            {
                pnlHScroll.Visible = true;
                if (bShowVScrollBar)
                    hScrollBar.Width = wMiddle - vScrollBar.Width;
                else
                    hScrollBar.Width = wMiddle;

                // Aggrandissement horizontal de la fenetre => la scrollbar horizontale doit se positionner à droite 
                if (W - hScrollBar.Value < wMiddle)
                    hScrollBar.Value = hScrollBar.Maximum - hScrollBar.LargeChange;
            }


            if (bShowVScrollBar)
                vW = vScrollBar.Width;
            if (bShowHScrollBar)
                hH = hScrollBar.Height;

            // Width of pnlScrollView
            pnlScrollView.Width = pnlMiddle.Width - leftWidth - vW;

            // vScrollBar properties
            if (bShowVScrollBar)
            {
                vScrollBar.Maximum = pnlScrollView.Height - (pnlMiddle.Height - hH) + pnlScrollView.Height / 10;

                vScrollBar.SmallChange = pnlScrollView.Height / 20;
                vScrollBar.LargeChange = pnlScrollView.Height / 10;
                
            }

            // hScrollBar properties
            if (bShowHScrollBar)
            {
                // Properties of hScrollBar: define first Maximum than Large and small change                
                int SC = W / 20;
                int LC = W / 10;

                // Width is limited to 65535                
                hScrollBar.Maximum = W - (wMiddle - vW) + LC;

                hScrollBar.SmallChange = SC;
                hScrollBar.LargeChange = LC;
            }
        }


        /// <summary>
        /// Scroll vertical time bar and sheet music when playing
        /// </summary>
        /// <param name="curtime"></param>
        private void ScrollTimeBar(double curtime)
        {
            if (sheetmusic != null)
            {
                currentTime = curtime * 1000; // Elapsed time en msec

                prevPulseTime = currentPulseTime;
                currentPulseTime = currentTime * pulsesPerMsec;
                if (prevPulseTime != currentPulseTime)
                {

                    // Calcule x_shade
                    sheetmusic.ScrollTo((int)currentPulseTime, (int)prevPulseTime);
                    
                    int x_midle = (pnlMiddle.Width - pnlTracks.Width) / 2;
                    int x_shade = sheetmusic.X_shade;

                    int W = sheetmusic.MaxStaffWidth;   // Longueur totale de la partition
                    int x_last = W - x_midle;            // Derniere moitié de la partition 
                    int delta = x_shade - x_midle;       

                    
                    // scroll horizontal scrollbar
                    if (delta <= 0)
                        hScrollBar.Value = 0;
                    else if (delta > 0 && delta <= hScrollBar.Maximum)
                        hScrollBar.Value = delta;


                    // Scroll vertical red bars
                    if (delta < 0)
                    {
                        // 1ere moitié affichage
                        SetTimeVLinePos(x_shade + 6);
                        
                    }
                    else
                    {
                        // milieu du morceau
                        if (x_shade < x_last)
                        {                            
                            SetTimeVLinePos(x_midle);
                        }
                        else
                        {
                            // Fin du morceau                            
                            SetTimeVLinePos(x_midle + (x_midle - (W - x_shade)));
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Scroll score to a position
        /// </summary>
        /// <param name="curtime"></param>
        public void ScrollTo(double starttime)
        {
            if (sheetmusic != null)
            {
                double dpercent = 0;
                int maxvalue = sequence1.GetLength();
                if (maxvalue > 0)
                    dpercent = starttime / maxvalue;
                
                // Elapsed time
                double maintenant = dpercent * _duration;

                currentTime = maintenant * 1000; // Elapsed time en msec
                
                prevPulseTime = currentPulseTime;   // Save old pulsetime
                currentPulseTime = currentTime * pulsesPerMsec; // Set new pulsetime

                if (prevPulseTime != currentPulseTime)
                {
                    // Calcule x_shade
                    sheetmusic.ScrollTo((int)currentPulseTime, (int)prevPulseTime);                                                           

                    int x_midle = (pnlMiddle.Width - pnlTracks.Width) / 2;
                    int x_shade = sheetmusic.X_shade;                   
                   
                    int delta = x_shade - x_midle;

                    // scroll horizontal scrollbar
                    if (delta <= 0)
                        hScrollBar.Value = 0;
                    else if (delta > 0 && delta <= hScrollBar.Maximum)
                        hScrollBar.Value = delta;                                                
                    
                }
            }
        }

        #endregion scroll events


        #region buttons play pause stop

        #region play
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }

        private void BtnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_play;
        }

        private void BtnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_red_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_play;
        }
       
        #endregion


        #region stop

        private void BtnStop_Click(object sender, EventArgs e)
        {
            //if (PlayerState != PlayerStates.Stopped)
                StopMusic();
        }


        private void BtnStop_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnStop.Image = Properties.Resources.btn_blue_stop;
        }

        private void BtnStop_MouseLeave(object sender, EventArgs e)
        {
            btnStop.Image = Properties.Resources.btn_black_stop;
        }

        #endregion


        #region next

        private void BtnNext_Click(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        private void BtnNext_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing)
                btnNext.Image = Properties.Resources.btn_blue_next;
        }

        private void BtnNext_MouseLeave(object sender, EventArgs e)
        {
            btnNext.Image = Properties.Resources.btn_black_next;
        }

        #endregion


        #region prev

        private void BtnPrev_Click(object sender, EventArgs e)
        {
            PlayPrevSong();
        }

        private void BtnPrev_MouseLeave(object sender, EventArgs e)
        {

            btnPrev.Image = Properties.Resources.btn_black_prev;
        }

        private void BtnPrev_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing)
                btnPrev.Image = Properties.Resources.btn_blue_prev;
        }

        #endregion


        #region Mute               
        
        /// <summary>
        /// Button: Mute Melody
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMute1_Click(object sender, EventArgs e)
        {
            if (myLyricsMgmt != null && myLyricsMgmt.MelodyTrackNum != -1)
            {
                if (!btnMute1.Checked)
                {
                    // Play melody
                    btnMute1.Checked = true;                    
                    UnMuteSomeTracks(sequence1.tracks[myLyricsMgmt.MelodyTrackNum].MidiChannel);
                }
                else
                {
                    // Mute melody
                    btnMute1.Checked = false;                    
                                       
                    // Stop Channel : All notes off                                        
                    sequencer1.AllSoundOff();

                    // Mute other TrackControls having same channel
                    MuteSomeTracks(sequence1.tracks[myLyricsMgmt.MelodyTrackNum].MidiChannel);
                }              

                this.Focus();
            }
        }

        #endregion


        private void BtnStatus()
        {
            // Play and pause are same button

            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnStop.Image = Properties.Resources.btn_black_stop;
                    btnPlay.Enabled = true;  // to allow pause
                    btnStop.Enabled = true;  // to allow stop 
                    lblStatus.Text = "Playing";
                    lblStatus.ForeColor = Color.LightGreen;
                    SetStartVLinePos(0);
                    btnStartRec.Enabled = false;
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Enabled = true;  // to allow stop

                    lblStatus.Text = "Paused";
                    lblStatus.ForeColor = Color.Yellow;
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)                                            
                        btnStop.Image = Properties.Resources.btn_red_stop;                    
                    else
                        btnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    lblStatus.Text = "Stopped";
                    lblStatus.ForeColor = Color.Red;
                    btnStartRec.Enabled = true;
                    VuMasterPeakVolume.Level = 0;
                    break;

                case PlayerStates.LaunchNextSong:       // pause between 2 songs of a playlist
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Enabled = true;   // to allow stop
                    lblStatus.Text = "Paused next singer";
                    lblStatus.ForeColor = Color.Yellow;
                    break;
                 
                case PlayerStates.Waiting:                    
                    // Count down running
                    btnPlay.Image = Properties.Resources.btn_green_play;                    
                    btnPlay.Enabled = true;  // to allow pause
                    btnStop.Enabled = true;   // to allow stop
                    lblStatus.Text = "Next";
                    lblStatus.ForeColor = Color.Violet;
                    VuMasterPeakVolume.Level = 0;
                    break;

                case PlayerStates.WaitingPaused:
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Enabled = true;   // to allow stop
                    lblStatus.Text = "Paused";
                    lblStatus.ForeColor = Color.Yellow;
                    break;

                case PlayerStates.NextSong:     // Select next song of a playlist
                    VuMasterPeakVolume.Level = 0;
                    break;

                default:
                    break;
            }
        }                           

        private void StopMusic()
        {
            PlayerState = PlayerStates.Stopped;
            try
            {
                timer5.Enabled = false;
                sequencer1.Stop();

                // Si point de départ n'est pas le début du morceau
                if (newstart > 0 )
                {
                    if (nbstop > 0)
                    {
                        newstart = 0;
                        nbstop = 0;
                        AfterStopped();
                    }
                    else
                    {                        
                        nbstop = 1;
                        AfterStopped();
                    }
                }
                else
                {
                    // Point de départ = début du moreceau
                    AfterStopped();
                }                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Button play clicked: manage actions according to player status 
        /// </summary>
        private void PlayPauseMusic()
        {
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    // If playing => pause
                    PlayerState = PlayerStates.Paused;
                    BtnStatus();
                    break;

                case PlayerStates.Paused:
                    // if paused => play                
                    nbstop = 0;
                    PlayerState = PlayerStates.Playing;
                    BtnStatus();
                    timer1.Start();
                    timer2.Start(); // Lyrics 
                    timer3.Start(); // Balls
                    timer4.Start(); // Beat
                    sequencer1.Continue();                    
                    break;

                case PlayerStates.Waiting:
                    // if Count down running: pause it
                    // stop timer : status = WaitingPaused
                    PlayerState = PlayerStates.WaitingPaused;
                    BtnStatus();
                    timer5.Enabled = false;                    
                    break;

                case PlayerStates.WaitingPaused:
                    // if Count down was paused
                    // => restart count down timer
                    PlayerState = PlayerStates.Waiting;
                    BtnStatus();
                    timer5.Enabled = true;
                    break;

                case PlayerStates.LaunchNextSong:       // pause between 2 songs of a playlist
                    if (Karaclass.m_CountdownSongs == 0)
                    {
                        // pause removed or no count down => play asap                                            
                        newstart = 0;
                        FirstPlaySong(newstart);
                    }
                    else
                    {
                        // Start  count down timer
                        StartCountDownTimer();                       
                    }
                    break;

                case PlayerStates.Stopped:
                    // First play                
                    FirstPlaySong(newstart);
                    break;
            }        
        }

        /// <summary>
        /// Initialize sequencer
        /// </summary>
        private void ResetSequencer()
        {
            newstart = 0;
            laststart = 0;
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;            
        }

        /// <summary>
        /// PlaySong for first time
        /// </summary>
        public void FirstPlaySong(int ticks)
        {
            if (sheetmusic == null) return;

            try
            {                               
                nbstop = 0;                             
                // stop Edit mode
                DspEdit(false);            
                DisplayFileInfos();
                DisplayLyricsInfos();
                ValideMenus(false);

                // This sis a playlist
                if (currentPlaylist != null)
                {
                    if (Application.OpenForms.OfType<frmLyric>().Count() == 0)
                    {
                        // this is the first item of the playlist => load frmLyrics
                        DisplayLyricsForm();
                    }
                    else if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                    {
                        // Lyric form is already up, we are charging the next song of the playlist                        
                        
                        // Update display of song & current singer
                        // The display of next singer is done 
                        string tx = string.Empty;
                        string sSong = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        if (currentPlaylistItem.KaraokeSinger == "" || currentPlaylistItem.KaraokeSinger == "<Song reserved by>")
                            tx = sSong;
                        else
                            tx = sSong + " - " + Strings.Singer + ": " + currentPlaylistItem.KaraokeSinger;
                        frmLyric.DisplaySinger(tx);
                        
                        // Reload picturebox
                        frmLyric.ResetDisplayChordsOptions(myLyricsMgmt);
                    }
                }
                // N0 playlist
                else if (bKaraokeAlwaysOn && bHasLyrics)
                {
                    DisplayLyricsForm();
                }
                
               
                sheetmusic.BPlaying = true;
                PlayerState = PlayerStates.Playing;
                BtnStatus();

                if (ticks > 0)
                {
                    sequencer1.Position = ticks;
                    sequencer1.Continue();
                }
                else
                {
                    // Start sequencer
                    sequencer1.Start();
                }


                // main timer
                timer1.Start();

                // start Lyrics                   
                timer2.Start();
                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    frmLyric.BeatDuration = sequence1.Division;
                }

                // start animation balls
                StartTimerBalls();

                // start Beat animation           
                BeatIntervall = sequence1.Tempo / 1000;
                if (BeatIntervall > 0)
                {
                    timer4.Interval = BeatIntervall;
                }
                timer4.Start();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        #region Mute
        /// <summary>
        /// Mute melody track
        /// </summary>
        private void MuteMelodyTrack(int melodytracknum)
        {
            // Conditions:
            // melody track exists 
            // AND
            // Mute Melody for all OR this playlistItem mutted
            if ( (myLyricsMgmt != null && myLyricsMgmt.MelodyTrackNum != -1) && (Karaclass.m_MuteMelody == true || (currentPlaylist != null && currentPlaylistItem.MelodyMute == true)))
            {
                btnMute1.Checked = false;                

                // Stop Channel : All notes off                                        
                sequencer1.AllSoundOff();

                // Mute other TrackControls having same channel
                MuteSomeTracks(sequence1.tracks[myLyricsMgmt.MelodyTrackNum].MidiChannel);

            }
        }
        
        #endregion

        /// <summary>
        /// Things to do at the end of a song
        /// </summary>
        private void AfterStopped()
        {
            if (sheetmusic == null)
                return;

            // Buttons play & stop 
            BtnStatus();            
            StopTimerBalls();                      
            sheetmusic.BPlaying = false;

            // Light off all channels
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    if (pnlTracks.Controls[i].Tag != null)
                    {
                        ((TrkControl.TrackControl)pnlTracks.Controls[i]).LightOff();

                    }
                }
            }

            // Stopped to start of score
            if (newstart <= 0)
            {
                ScrollTimeBar(0);                
                DisplayTimeElapse(0);
                DisplayFileInfos();

                lblBeat.Text = "1|" + sequence1.Numerator;

                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    frmLyric.ResetTop();
                    frmLyric.StopDiaporama();
                }
                
                positionHScrollBarNew.Value = 0;

                SetTimeVLinePos(0);
                laststart = 0;
                SetStartVLinePos(0);

                if (PlayerState == PlayerStates.Stopped)
                {
                    ValideMenus(true);

                    for (int i = 0; i < pnlTracks.Controls.Count; i++)
                    {
                        if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                        {
                            if (pnlTracks.Controls[i].Tag != null)
                            {
                                try
                                {
                                    TrkControl.TrackControl trkctrl = ((TrkControl.TrackControl)pnlTracks.Controls[i]);
                                    // Volume                                
                                    Track trk = sequence1.tracks[trkctrl.Track];
                                    trkctrl.SetVolume(trk.Volume);
                                    // Pan
                                    trkctrl.SetPan(trk.Pan);
                                    // Reverb
                                    trkctrl.SetReverb(trk.Reverb);
                                    // Patch
                                    trkctrl.SetPatch(trk.ProgramChange);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.Message.ToString()); }
                            }
                        }
                    }                   
                }

            }
            else
            {
                // Stop to start point newstart (ticks)
                ScrollTo(newstart);
                int x_midle = (pnlMiddle.Width - pnlTracks.Width) / 2;
                // blue bar
                SetStartVLinePos(x_midle);
                // red bar
                SetTimeVLinePos(0);              
            }
        }

        #endregion button play pause stop


        #region menus

        /// <summary>
        /// Valid or not some menus if playing or not
        /// </summary>
        /// <param name="enabled"></param>
        private void ValideMenus(bool enabled)
        {
            menuStrip1.Visible = enabled;
            return;          
        }

        #region Menu File

        /// <summary>
        /// Menu create a new midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileNew_Click(object sender, EventArgs e)
        {
            int numerator = 4;
            int denominator = 4;
            int division = 960;
            int tempo = 750000;
            int measures = 35;

            // Display dialog windows new midi file
            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile MidiFileDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewMidiFile(numerator, denominator, division, tempo, measures);
            dr = MidiFileDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            CreateNewMidiFile.Numerator = MidiFileDialog.Numerator;
            CreateNewMidiFile.Denominator = MidiFileDialog.Denominator;
            CreateNewMidiFile.Division = MidiFileDialog.Division;
            CreateNewMidiFile.Tempo = MidiFileDialog.Tempo;
            CreateNewMidiFile.Measures = MidiFileDialog.Measures;

            // Fab 27/10/2023
            // Display Dialog for new track
            string trackname = "Track1";
            int programchange = 0;
            int channel = 0;
            decimal trkindex = 0;
            int clef = 0;

            dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog TrackDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog(trackname, programchange, channel, trkindex, clef);
            dr = TrackDialog.ShowDialog();

            // TODO : if we are creating a new file, 
            if (dr == DialogResult.Cancel)
                return;

            CreateNewMidiFile.trackname = TrackDialog.TrackName;
            CreateNewMidiFile.programchange = TrackDialog.ProgramChange;
            CreateNewMidiFile.channel = TrackDialog.MidiChannel;
            CreateNewMidiFile.trkindex = trkindex;
            CreateNewMidiFile.clef = TrackDialog.cle;


            // Ferme le formulaire frmLyric
            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                frmLyric.Close();
            }
            // ferme le formulaire frmLyricsEdit
            if (Application.OpenForms.OfType<frmLyricsEdit>().Count() > 0)
            {     
                Application.OpenForms["frmLyricsEdit"].Close();
            }
            // Ferme le formulaire PianoRoll
            if (Application.OpenForms.OfType<frmPianoRoll>().Count() > 0)
            {
                Application.OpenForms["frmPianoRoll"].Close();
            }

            
            //plLyrics.Clear();            
            //myLyric = null;          

            // Create a new Midi File with above parameters
            NewMidiFile();            
        }        
        
        /// <summary>
        /// Menu OpenFile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileOpen_Click(object sender, EventArgs e)
        {

            openMidiFileDialog.Title = "Open MIDI file";
            openMidiFileDialog.DefaultExt = "kar";
            //openMidiFileDialog.Filter = "Kar files|*.kar|MIDI files|*.mid|All files|*.*";
            openMidiFileDialog.Filter = "Kar files|*.kar|MIDI files|*.mid|Xml files|*.xml|MusicXml files|*.musicxml|Text files|*.txt|All files|*.*";


            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;                                

                MIDIfileName = Path.GetFileName(fileName);
                MIDIfilePath = Path.GetDirectoryName(fileName);
                MIDIfileFullPath = fileName;

                // Load file
                sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                sequence1.LoadCompleted += HandleLoadCompleted;
                             
                SelectFileToLoadAsync();

            }
        }

        /// <summary>
        /// Menu File Save
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileSave_Click(object sender, EventArgs e)
        {
            SaveFileProc();
        }

        /// <summary>
        /// Save File
        /// </summary>
        private void SaveFileProc()
        {
            string fName = MIDIfileName;
            string fPath = MIDIfilePath;

            if (fPath == null || fPath == "" || fName == null || fName == "")
            {
                SaveAsFileProc();
                return;
            }

            
            if (File.Exists(MIDIfileFullPath) == false)
            {
                SaveAsFileProc();
                return;
            }

            InitSaveFile(MIDIfileFullPath);
        }

        /// <summary>
        /// Menu File Save As
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveAsFileProc();
        }

        private void SaveAsFileProc()
        {
            string fName = MIDIfileName;
            string fPath = MIDIfilePath;
            
            string fullName = string.Empty;
            string defName = string.Empty;

            #region search name
            // search path
            if (fPath == null || fPath == "")            
                fPath = CreateNewMidiFile.DefaultDirectory;
            
            // Search name
            if (MIDIfileName == null || MIDIfileName == "")            
                fName = "New.mid";
            

            string inifName = fName;                            // Original name with extension
            string defExt = Path.GetExtension(fName);           // Extension
            fName = Path.GetFileNameWithoutExtension(fName);    // name without extension
            defName = fName;                                    // Proposed name for dialog box

            fullName = fPath + "\\" + inifName;

            if (File.Exists(fullName) == true)
            {
                // Remove all (1) (2) etc..
                string pattern = @"[(\d)]";
                string replace = @"";
                inifName = Regex.Replace(fName, pattern, replace);

                int i = 1;
                string addName = "(" + i.ToString() + ")";
                defName = inifName + addName + defExt;
                fullName = fPath + "\\" + defName;

                while (File.Exists(fullName) == true)
                {
                    i++;
                    defName = inifName + "(" + i.ToString() + ")" + defExt;
                    fullName = fPath + "\\" + defName;
                }
            }

            #endregion search name

            string defFilter = "MIDI files (*.mid)|*.mid|Kar files (*.kar)|*.kar|All files (*.*)|*.*";
            if (defExt == ".kar")
                defFilter = "Kar files (*.kar)|*.kar|MIDI files (*.mid)|*.mid|All files (*.*)|*.*";

            saveMidiFileDialog.Title = "Save MIDI file";
            saveMidiFileDialog.Filter = defFilter;
            saveMidiFileDialog.DefaultExt = defExt;
            saveMidiFileDialog.InitialDirectory = @fPath;
            saveMidiFileDialog.FileName = defName;

            if (saveMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveMidiFileDialog.FileName;

                MIDIfileFullPath = fileName;
                MIDIfileName = Path.GetFileName(fileName);
                MIDIfilePath = Path.GetDirectoryName(fileName);

                InitSaveFile(fileName);
            }
        }

        /// <summary>
        /// Save file: initialize events
        /// </summary>
        /// <param name="fileName"></param>
        public void InitSaveFile(string fileName)
        {
           
            progressBarPlayer.Visible = true;

            sequence1.SaveProgressChanged += HandleSaveProgressChanged;
            sequence1.SaveCompleted += HandleSaveCompleted;            
            SaveFile(fileName);
        }

        /// <summary>
        /// Savs as PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuSaveAsPDF_Click(object sender, EventArgs e)
        {
            /** The callback function for the "Save As PDF" menu.
              * When invoked this will save the sheet music as a PDF document,
              * with one image per page.  For each page in the sheet music:
              * - Create a new bitmap, PageWidth by PageHeight
              * - Create a Graphics object for the bitmap
              * - Call the SheetMusic.DoPrint() method to draw the music onto the bitmap
              * - Add the bitmap image to the PDF document.
              * - Save the PDF document
              */

            string message = string.Empty;

            if (sheetmusic == null)
                return;

            if (MIDIfileName == null || MIDIfileName == "")
            {
                MIDIfileName = "new.mid";
            }
          

            // Affiche le formulaire frmPrint 
            if (Application.OpenForms["frmPrint"] != null)
                Application.OpenForms["frmPrint"].Close();

            Form frmPrint = new frmPrint(sequence1, MIDIfileFullPath);
            frmPrint.Show();

        }

        /// <summary>
        /// Load the midi file in the sequencer
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncMidiFile(string fileName)
        {
            try
            {                
                progressBarPlayer.Visible = true;

                ResetSequencer();
                if (fileName != "\\")
                {                                        
                    sequence1.LoadAsync(fileName);                                        
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }            
        }
        

        /// <summary>
        /// Load async a XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncXmlFile(string fileName)
        {
            try
            {
                progressBarPlayer.Visible = true;

                ResetSequencer();
                if (fileName != "\\")
                {
                    // FAB 2710
                    MXmlReader = new MusicXmlReader();
                    MXmlReader.LoadXmlCompleted += HandleLoadXmlCompleted;
                    // ========

                    MXmlReader.LoadXmlAsync(fileName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Load async a TXT file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncTxtFile(string fileName)
        {
            try
            {
                progressBarPlayer.Visible = true;

                ResetSequencer();
                if (fileName != "\\")
                {
                    MTxtReader = new MusicTxtReader(fileName);
                    MTxtReader.LoadTxtCompleted += HandleLoadTxtCompleted;

                    MTxtReader.LoadTxtAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Save the midi file
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveFile(string fileName)
        {
            try
            {
                if (fileName != "")
                {
                    sequence1.SaveAsync(fileName);
                }

            }
            catch (Exception errsave)
            {
                Console.Write(errsave.Message);
            }
        }


        #region import/export

        /// <summary>
        /// Export Midi file to normalized text dump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileExportMidiToText_Click(object sender, EventArgs e)
        {
            ExportMidiToText();
        }

        /// <summary>
        /// Dump Midi to text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDump_Click(object sender, EventArgs e)
        {
            ExportMidiToText();
        }

        /// <summary>
        /// Save async midi dump to text file
        /// </summary>
        private void ExportMidiToText()
        {
            if (MIDIfilePath == null)
                return;

            string name = Path.GetFileNameWithoutExtension(MIDIfileName) + " (Dump)";
            string file = string.Empty;
            file = string.Format("{0}\\{1}{2}", MIDIfilePath, name, ".txt");

            MTxtWriter = new MusicTxtWriter(sequence1, file);
            MTxtWriter.WriteTxtCompleted += MTxtWriter_WriteTxtCompleted;
            MTxtWriter.WriteTxtProgressChanged += MTxtWriter_WriteTxtProgressChanged;
            MTxtWriter.WriteTxtAsync(file);
        }

        private void MTxtWriter_WriteTxtProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Event: dump midi file completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MTxtWriter_WriteTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string file = ((MusicTxtWriter)sender).fileName;            
            try
            {                
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
      

        /// <summary>
        /// Import a normalized text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileImportMidiFromText_Click(object sender, EventArgs e)
        {
            importMidiFileFromText();
        }

        private void importMidiFileFromText()
        {
            openMidiFileDialog.Title = "Open Text file";
            openMidiFileDialog.DefaultExt = "txt";
            openMidiFileDialog.Filter = "Text files|*.txt|All files|*.*";
            openMidiFileDialog.InitialDirectory = MIDIfilePath;

            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                LoadAsyncTxtFile(fileName);
            }
        }

      

        /// <summary>
        /// Import a MusicXml file to Midi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileImportMusicXml_Click(object sender, EventArgs e)
        {
            openMidiFileDialog.Title = "Open MusicXml file";
            openMidiFileDialog.DefaultExt = "xml";
            openMidiFileDialog.Filter = "Xml files|*.xml|MusicXml files|*.musicxml|All files|*.*";            
            openMidiFileDialog.InitialDirectory = MIDIfilePath;

            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                // Load xml file and display messages
                LoadXmlFile(fileName, false);             
            }
        }
        
        private bool LoadXmlFile(string fileName, bool bsilentmode)
        {
            MIDIfilePath = Path.GetDirectoryName(fileName);

            string fExt = Path.GetExtension(fileName);             // Extension
            string fName = Path.GetFileNameWithoutExtension(fileName);    // name without extension
            MIDIfileName = fName + ".mid";
            MIDIfileFullPath = Path.Combine(MIDIfilePath, MIDIfileName);

            string lyrics = string.Empty;
          
            // Load xml file                
            MusicXmlReader M = new MusicXmlReader();
            sequence1 = M.Read(fileName, false);

            if (sequence1 == null)
            {
                if (!bsilentmode)
                    MessageBox.Show("Invalid MusicXml file", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            // load lyrics and chords if included in lyrics
            //  ********************** Why not load embedded chords here if bShowChords is true ? *****************
            myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
            bHasLyrics = myLyricsMgmt.OrgplLyrics.Count > 0; // myLyricsMgmt.LyricType != LyricTypes.None;
                       

            laststart = 0;
            // Remove all MIDI events after last note
            sequence1.Clean();

            ResetSequencer();

            sequencer1.Sequence = sequence1;
            UpdateMidiTimes();
            DisplaySongDuration();

            positionHScrollBarNew.Value = 0;
            positionHScrollBarNew.Maximum = _totalTicks;

            // ----------------------------------------------------------------
            // Display Scores on panel pnlScrollView
            // ----------------------------------------------------------------
            DisplayScores();

            // Display song duration
            DisplaySongDuration();

            // Display track controls             
            DisplayTrackControls();

            // Reset tracks stuff
            InitTracksStuff();

            // Recherche si des lyrics existent et affiche la forme frmLyric
            mnuDisplayLyricsWindows.Checked = bKaraokeAlwaysOn;

            if (bKaraokeAlwaysOn && bHasLyrics)
                DisplayLyricsForm();

            // Display log file
            if (sequence1.Log != "")
            {                
                lblChangesInfos.Text = sequence1.Log;
            }

            DisplayFileInfos();
            DisplayLyricsInfos();

            // Display title
            SetTitle(MIDIfileName);
            
            // File is new
            if (bsilentmode)
                FileModified();

            return true;
        }

        /// <summary>
        /// Export Midi to MusicXml format file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileExportToMusicXml_Click(object sender, EventArgs e)
        {

        }


        #endregion


        /// <summary>
        /// Edit or display MIDI file tags
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnufileProperties_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmTags"] == null)
            {
                Form frmTags = new frmTags(sequence1);
                frmTags.ShowDialog();
            }
        }

        /// <summary>
        /// Menu exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion


        #region Menu Edit
      
        /// <summary>
        /// Reset myLyric
        /// </summary>
        /// <param name="lyricstracknum">notes guide</param>
        /// <param name="melodytracknum">num track to store lyrics</param>
        public void NewMyLyric(int lyricstracknum, int melodytracknum)
        {
            if (myLyricsMgmt == null)
            {
                myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
            }
            myLyricsMgmt.MelodyTrackNum = melodytracknum;
            myLyricsMgmt.LyricsTrackNum = lyricstracknum;            
        }

        /// <summary>
        /// Create new track containing lyrics type text
        /// </summary>
        public void AddTrackWords()
        {            
            int f = sequence1.Format;
            int C = sequence1.tracks.Count;
            int trackindex = 2; // track 2 by default, named "Words"
            bool bCreate = true;

            // Check if enough tracks in sequence
            if (C == 0)
                trackindex = 0;
            else if (C == 1)
                trackindex = 1;            
            else if (C > 2)
            {
                // Check if and empty track exists at position trackindex
                Track wtrack = sequence1.tracks[2];

                if (wtrack.Notes.Count == 0)
                {
                    // If empty track exists at position 2 => take this track
                    bCreate = false;                    
                }
            }

            if (bCreate)
            {
                // Create new track
                int clef = 2; // Clef.None
                Track track = InsertTrack(trackindex, "Words", "AcousticGrandPiano", 0, 0, 79, sequence1.Tempo, sequence1.Time, clef);
                InsertTrackControl(track, trackindex);

                RedrawSheetMusic();
                SetScrollBarValues();

                // If a new track was created, the melody track may have changed

                if (myLyricsMgmt.MelodyTrackNum >= trackindex)
                {
                    //melodytracknum++;
                    myLyricsMgmt.MelodyTrackNum++;
                }
            }

            // Return track number where text lyrics are set (normaly 2)
            //return trackindex;
            myLyricsMgmt.LyricsTrackNum = trackindex;
        }     

        /// <summary>
        /// Menu: open track Word lyric editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuEditAddLyrics_Click(object sender, EventArgs e)
        {          
            DisplayEditLyricsForm();
        }

        /// <summary>
        /// Display the form for lyrics edition
        /// </summary>
        public void DisplayEditLyricsForm()
        {            
            int melodytracknum = 0;
            // Display track editor
            if (Application.OpenForms.OfType<frmLyricsEdit>().Count() == 0)
            {
                try
                {                                                            
                    // Cas
                    if (myLyricsMgmt.plLyrics.Count > 0 && myLyricsMgmt.MelodyTrackNum >= 0)
                    {
                        // Lyrics exist and melody track found
                        // go directly to edition form ?

                    }
                    else if (myLyricsMgmt.plLyrics.Count > 0 && myLyricsMgmt.MelodyTrackNum == -1)
                    {
                        // Some lyrics are found, but no melody
                        // propose to select a track (or not) as a guide
                        // Lyrics does not exist
                        // => select track having melody

                    }
                    else if (myLyricsMgmt.plLyrics.Count == 0)
                    {
                        // Start a new kar file from a mid file
                        // Select a track (or not) as a guide
                        // Lyrics does not exist
                        // => select track having melody
                        DialogResult dr = new DialogResult();
                        frmLyricsSelectTrack TrackDialog = new frmLyricsSelectTrack(sequence1);
                        dr = TrackDialog.ShowDialog();

                        if (dr == System.Windows.Forms.DialogResult.Cancel)
                            return;

                        // Get track number for melody
                        // -1 if no track
                        melodytracknum = TrackDialog.TrackNumber - 1;                        
                        myLyricsMgmt.MelodyTrackNum = melodytracknum;

                        if (TrackDialog.TextLyricFormat == 0)
                        {
                            // TEXT FORMAT
                            // Create track at position 2 for text lyrics           
                            // Set myLyrics.melodytracknum & myMyrics.lyricstracknum
                            //AddTrackWords();
                            myLyricsMgmt.LyricType = LyricTypes.Text;
                        }
                        else
                        {
                            // LYRIC FORMAT
                            // Lyrics set to the same track than notes
                            myLyricsMgmt.MelodyTrackNum = melodytracknum;
                            if (melodytracknum > -1)
                                myLyricsMgmt.LyricsTrackNum = melodytracknum;
                            else
                                myLyricsMgmt.LyricsTrackNum = 0;
                            myLyricsMgmt.LyricType = LyricTypes.Lyric;
                        }
                        DisplayLyricsInfos();
                    }

                    // Lyrics exist
                    if (!myLyricsMgmt.bHasChordsInLyrics)
                    {
                        // Remove detected chords: redo the lyrics extraction
                        myLyricsMgmt.FullExtractLyrics();
                    }

                    frmLyricsEdit frmLyricsEdit;
                    // Caution: Load the original lyrics, not the lyrics internally transformed by FullExtractLyrics
                    frmLyricsEdit = new frmLyricsEdit(sequence1, myLyricsMgmt.OrgplLyrics, myLyricsMgmt, MIDIfileFullPath);

                    frmLyricsEdit.Show();
                }
                catch (Exception fl)
                {
                    Console.Write("Erreur showing frmLyricsEdit: " + fl.Message);
                }
            }
            else
            {
                if (Application.OpenForms["frmLyricsEdit"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmLyricsEdit"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmLyricsEdit"].Show();
                Application.OpenForms["frmLyricsEdit"].Activate();
            }

        }

        /// <summary>
        /// Validate or invalidate EditMode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuEditScore_Click(object sender, EventArgs e)
        {
            if (PlayerState != PlayerStates.Stopped)
                return;

            // display score if not visible
            //if (!bSequencerAlwaysOn)
            //    DisplaySequencer();
            if (!pnlTop.Visible)
            {
                bForceShowSequencer = true;
                RedimIfSequencerVisible();
            }


            DspEdit(!bEditScore);
        }

        /// <summary>
        /// Validate or invalidate entering notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuEditEnterNotes_Click(object sender, EventArgs e)
        {           
            if (PlayerState != PlayerStates.Stopped)
                return;

            // Display score if not visible
            //if (!bSequencerAlwaysOn)                            
            //    DisplaySequencer();
            if (!pnlTop.Visible)
            {
                bForceShowSequencer = true;
                RedimIfSequencerVisible();
            }
            DspEnterNotes();
        }
              
        #endregion


        #region Menu Display

        /// <summary>
        /// Display Sequencer panels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplaySequencer_Click(object sender, EventArgs e)
        {
            DisplaySequencer();           
        }

        private void DisplaySequencer()
        {
            bSequencerAlwaysOn = !bSequencerAlwaysOn;

            // bForceShowSequencer was true, but user decided to hide the sequencer by clicking on the menu
            if (bSequencerAlwaysOn == false && bForceShowSequencer == true)
                bForceShowSequencer = false;

            RedimIfSequencerVisible();
        }

        /// <summary>
        /// Resize according to sequencer visible or not
        /// </summary>
        /// <param name="bSequencerAlwaysOn"></param>
        private void RedimIfSequencerVisible()
        {                       
            /*
             * Bug quand on veut cacher le sequencer en faisant F12 lorsque
             * bSequencerAlwaysOn = false && bForceShowSequencer = true                           
             * 
             */

            if (bSequencerAlwaysOn == true || bForceShowSequencer == true)
            {
                this.MaximizeBox = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                
                // Show sequencer
                pnlTop.Visible = true;
                pnlMiddle.Visible = true;

               
                
                #region window size & location
                // If window is maximized
                if (Properties.Settings.Default.frmPlayerMaximized)
                {
                    Location = Properties.Settings.Default.frmPlayerLocation;                    
                    WindowState = FormWindowState.Maximized;
                }
                else
                {                   
                    try
                    {
                        if (Properties.Settings.Default.frmPlayerSize.Height == SimplePlayerHeight)
                        {
                            this.Size = new Size(Properties.Settings.Default.frmPlayerSize.Width, 600);
                        }
                        else
                            Size = Properties.Settings.Default.frmPlayerSize;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                #endregion

                if (bSequencerAlwaysOn == true)
                    mnuDisplaySequencer.Checked = true;
            }
            else
            {

                // Hide sequencer

                // Save size                
                #region save size
                // Copy window location to app settings                
                if (WindowState != FormWindowState.Minimized)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Properties.Settings.Default.frmPlayerLocation = RestoreBounds.Location;
                        Properties.Settings.Default.frmPlayerMaximized = true;

                    }
                    else if (WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.frmPlayerLocation = Location;
                        if (Height != SimplePlayerHeight)
                            Properties.Settings.Default.frmPlayerSize = Size;
                        Properties.Settings.Default.frmPlayerMaximized = false;
                    }

                    // Show sequencer
                    Properties.Settings.Default.ShowSequencer = bSequencerAlwaysOn;
                    Properties.Settings.Default.ShowKaraoke = bKaraokeAlwaysOn;

                    // Save settings
                    Properties.Settings.Default.Save();
                }
                #endregion

                this.MaximizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                
                
                pnlTop.Visible = false;
                pnlMiddle.Visible = false;

                if (this.WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;

                // Redim size to simple player
                this.Size = new Size(SimplePlayerWidth, SimplePlayerHeight);

                mnuDisplaySequencer.Checked = false;
                MnuEditScore.Checked = false;

                if (sheetmusic != null)
                    DspEdit(false);

                if (sheetmusic != null)
                    sheetmusic.bEditMode = false;
            }
        }

        /// <summary>
        /// Display Lyrics Windows if Hidden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDisplayLyricsWindows_Click(object sender, EventArgs e)
        {
            bKaraokeAlwaysOn = !bKaraokeAlwaysOn;
            DisplayKaraoke(bKaraokeAlwaysOn);
        }

        /// <summary>
        /// Execute menu show karaoke
        /// </summary>
        /// <param name="bKaraokeAlwaysOn"></param>
        private void DisplayKaraoke(bool ShowKaraoke)
        {
            if (ShowKaraoke == true)
            {
                DisplayLyricsForm();
                frmLyric.StartTimerBalls();

                mnuDisplayLyricsWindows.Checked = true;
            }
            else
            {
                // ferme le formulaire frmLyricsEdit
                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    Application.OpenForms["frmLyric"].Close();
                }                
                mnuDisplayLyricsWindows.Checked = false;
            }
        }

        private void MnuDisplayPianoRoll_Click(object sender, EventArgs e)
        {
            int tracknum = -1;
            if (sequence1.tracks.Count == 1)
                tracknum = 0;

            DisplayPianoRoll(tracknum, MIDIfileFullPath, 0);
        }

        /// <summary>
        /// Display the pianoRoll window
        /// </summary>
        /// <param name="tracknum"></param>
        private void DisplayPianoRoll(int tracknum, string fileName, int ticks)
        {                        
            if (Application.OpenForms["frmPianoRoll"] == null)
            {
                int note = sheetmusic.CurrentNote.midinote.Number;          
                
                frmPianoRoll = new frmPianoRoll(sequence1, tracknum, outDevice, fileName);
                frmPianoRoll.Show();
                frmPianoRoll.Refresh();
                frmPianoRoll.StartupPosition(ticks, note);
            }
            else
            {
                frmPianoRoll.Close();
            }
        }

        private void MnuDisplayPianoTraining_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmPianoTraining"] == null)
            {
                frmPianoTraining = new frmPianoTraining(outDevice, MIDIfileFullPath);
                frmPianoTraining.Show();
                frmPianoTraining.Refresh();
               
            }
        }

        private void MnuDisplayZoom_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmZoomDialog frmZoomDialog = new Sanford.Multimedia.Midi.Score.UI.frmZoomDialog(sheetmusic ,zoom);
            dr = frmZoomDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            decimal newzoom = frmZoomDialog.zoom;

            zoom = Convert.ToInt32(newzoom) / 100f;
            RedrawTrackControls();

            RefreshDisplay();
        }

        private void MnuDisplayScrollingHorz_Click(object sender, EventArgs e)
        {
            mnuDisplayScrollingVert.Checked = false;
            mnuDisplayScrollingHorz.Checked = true;

            ScrollVert = false;
            RedrawSheetMusic();
        }

        private void MnuDisplayScrollingVert_Click(object sender, EventArgs e)
        {
            mnuDisplayScrollingVert.Checked = true;
            mnuDisplayScrollingHorz.Checked = false;

            ScrollVert = true;
            RedrawSheetMusic();
        }

        #endregion


        #region Menu Midi

        #region tracks

        /// <summary>
        /// Add a new track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiNewTrack_Click(object sender, EventArgs e)
        {
            // Calcul nombre de mesures
            int duration = sequence1.GetLength();
            float mult = 4.0f / sequence1.Denominator;
            int MeasureLength = sequence1.Division * sequence1.Numerator;
            MeasureLength = Convert.ToInt32(MeasureLength * mult);
            int nbMeasures = 1 + duration / MeasureLength;

            NewTrack(nbMeasures);
        }

        /// <summary>
        /// Add new tracks from a Midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiImporTracksFromMidi_Click(object sender, EventArgs e)
        {
            openMidiFileDialog.Title = "Open MIDI file";
            openMidiFileDialog.DefaultExt = "kar";
            openMidiFileDialog.Filter = "Kar files|*.kar|MIDI files|*.mid|All files|*.*";
            openMidiFileDialog.InitialDirectory = MIDIfilePath;

            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                MidiTrackImporter trckI = new MidiTrackImporter();
                trckI.TrackSelected += new MidiTrackImporter.TrackSelectedEventHandler(MidiTrackImporter_TrackSelected);
                trckI.Read(fileName);
            }
        }

        private void MidiTrackImporter_TrackSelected(object sender, Track track)
        {
            // Add track to existing tracks            
            sequence1.Add(track);
            
            AddTrackControl(track, sequence1.tracks.Count - 1);

            UpdateMidiTimes();
            DisplaySongDuration();

            positionHScrollBarNew.Value = 0;
            positionHScrollBarNew.Maximum = _totalTicks;

            // Reset tacks stuff
            InitTracksStuff();

            // Create a new ShetMusic
            RedrawSheetMusic();

            SetScrollBarValues();

            FileModified();

        }

        /// <summary>
        /// Add new tracks from a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiImportTracksFromText_Click(object sender, EventArgs e)
        {
            openMidiFileDialog.Title = "Open Text file";
            openMidiFileDialog.DefaultExt = "txt";
            openMidiFileDialog.Filter = "Text files|*.txt|All files|*.*";
            openMidiFileDialog.InitialDirectory = MIDIfilePath;

            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                //TextTrackImporter trckI = new TextTrackImporter();
                //trckI.TrackSelected += new TextTrackImporter.TrackSelectedEventHandler(TextTrackImporter_TrackSelected);

                MidiTrackTextImporter trckI = new MidiTrackTextImporter();
                trckI.TrackSelected += new MidiTrackTextImporter.TrackSelectedEventHandler(TextTrackImporter_TrackSelected);

                trckI.Read(fileName);

            }
        }

        private void TextTrackImporter_TrackSelected(object sender, Track track)
        {
            // Add track to existing tracks            
            sequence1.Add(track);

            AddTrackControl(track, sequence1.tracks.Count - 1);


            UpdateMidiTimes();
            DisplaySongDuration();

            positionHScrollBarNew.Value = 0;
            positionHScrollBarNew.Maximum = _totalTicks;

            // Reset tracks stuff
            InitTracksStuff();

            // Create a new ShetMusic
            RedrawSheetMusic();

            SetScrollBarValues();

            FileModified();

        }


        /// <summary>
        /// Add a time line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMIDIAddTimeLine_Click(object sender, EventArgs e)
        {
            string trackname;
            string instrumentname;
            int programchange;
            int channel;
            int volume;

            programchange = 0;
            trackname = "TimeLine";
            instrumentname = "AcousticGrandPiano";
            channel = 15;
            volume = 0;

            sequence1.Format = 1;

            // Pas possible pour midi format 0 : une seule piste
            if (sequence1.Format == 0)
            {
                string msg = "Sorry, i can't add a new track to a midi file format 0";
                string title = "Karaboss";
                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                // Add track to sequence
                int clef = 2; // Clef.None
                Track track = AddTrack(trackname, instrumentname, channel, programchange, volume, sequence1.Tempo, sequence1.Time, clef);

                // Add track control
                int trackindex = sequence1.tracks.Count - 1;
                AddTrackControl(track, trackindex);

                // Add notes
                float dur = 0.25f; // doubles croches
                CreateTimeLineMelody(track, channel, dur);

                // Reset tracks Stuff
                InitTracksStuff();

                RedrawSheetMusic();
                SetScrollBarValues();

                FileModified();
            }

        }

        #endregion

        /// <summary>
        /// Menu: add measures
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiAddMeasures_Click(object sender, EventArgs e)
        {

            // Le numérateur de la fraction (nombre supérieur) indique le « nombre de temps » utilisés dans la mesure :
            // 2/4 signifie « une mesure à deux noires »,
            // 3/2, « une mesure à trois blanches »,
            // 6/8, « une mesure à six croches ».

            // Le dénominateur (nombre inférieur) indique l'unité de temps de la mesure, selon la convention suivante :
            // le nombre 1 représente la ronde ;
            // le nombre 2 représente la blanche (soit une demi-ronde) ;
            // le nombre 4 représente la noire (soit un quart de ronde) ;
            // le nombre 8 représente la croche (soit un huitième de ronde) ;
            // le nombre 16 représente la double croche (soit un seizième de ronde).           

            AddMeasures();
        }

        private void AddMeasures()
        {
            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmAddMeasuresDialog AddMeasuresDialog = new Sanford.Multimedia.Midi.Score.UI.frmAddMeasuresDialog();
            dr = AddMeasuresDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            decimal measures = AddMeasuresDialog.Measures;

            if (measures == 0)
                return;
         
            _totalTicks = sequence1.GetLength();
            _measurelen = sequence1.Time.Measure;
            int ticks = _totalTicks + (int)measures * _measurelen;
            Track track = sequence1.tracks[0];            

            // Ofset end of track            
            track.EndOfTrackOffset = ticks;
            
            // Update GUI
            UpdateMidiTimes();
            DisplaySongDuration();

            RedrawSheetMusic();
            SetScrollBarValues();

            FileModified();
        }

        /// <summary>
        /// Modify Tempo, not Division (read only)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiModifyTempo_Click(object sender, EventArgs e)
        {
            DspEdit(true);

            if (Application.OpenForms["frmModifyTempo"] != null)
                Application.OpenForms["frmModifyTempo"].Close();

            if (Application.OpenForms["frmModifyTempo"] == null)
            {
                frmModifyTempo = new frmModifyTempo(sheetmusic, sequence1);                
                frmModifyTempo.Show();
                frmModifyTempo.Refresh();
            }
            
        }

        
        public void UpdateTimes()
        {
            UpdateMidiTimes();            

            FileModified();
            DisplayFileInfos();

        }

       
        /// <summary>
        /// Modify Time Signature (4/4, 4/2 etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiTimeSignature_Click(object sender, EventArgs e)
        {
            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.ChangeTimeSignature TimeSignatureDialog = new Sanford.Multimedia.Midi.Score.UI.ChangeTimeSignature(sequence1.Numerator, sequence1.Denominator);
            dr = TimeSignatureDialog.ShowDialog();

            if (dr == DialogResult.Cancel)
                return;

            int numerator = TimeSignatureDialog.Numerator;
            int denominator = TimeSignatureDialog.Denominator;

            if (ModTimeSignature(numerator, denominator)) {
                RedrawSheetMusic();
                DisplayFileInfos();
                FileModified();
            }
        }

        /// <summary>
        /// Time signature: remove all time signatures in all tracks, write new values in track 0
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        private bool ModTimeSignature(int numerator, int denominator)
        {
            if (numerator == sequence1.Numerator && denominator == sequence1.Denominator)
                return false;

            sequence1.Numerator = numerator;
            sequence1.Denominator = denominator;
            sequence1.Time = new TimeSignature(sequence1.Numerator, sequence1.Denominator, sequence1.Division, sequence1.Tempo);

            // Remove all time signature messages in all tracks
            foreach (Track track in sequence1.tracks)
            {
                track.RemoveTimesignature();
            }

            // Write new value in track 0            
            sequence1.tracks[0].insertTimesignature(numerator, denominator);
            
            return true;

        }
       
        /// <summary>
        /// Remove Fader in & out (enfin j'espère !)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiRemoveFader_Click(object sender, EventArgs e)
        {
            RemoveFader();
        }


        /// <summary>
        /// Split Hands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuMidiSplitHands_Click(object sender, EventArgs e)
        {
            if (mnuMidiSplitHands.Checked)
            {
                // Split hands off
                mnuMidiSplitHands.Checked = false;
            }
            else
            {
                // Split hands on
                // If we have 1 track, we can split it int 2 tacks
                // to have left hand and rignt hand for piano

                // Check if only one track ....
                if (sequence1.tracks.Count != 1)
                {
                    MessageBox.Show("Error, two many tracks: \nNumber of tracks must be 1.\n\nSplit Hands allows to split one track into two tracks to separate left hand and right hand for example.", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                mnuMidiSplitHands.Checked = true;
            }
                       
            OpenMidiFileOptions.SplitHands = mnuMidiSplitHands.Checked;

            // reload the file according to split hands choice stored in MidiFile properties
            LoadAsyncMidiFile(MIDIfileFullPath);
        }

        #endregion


        #region Menu Help
    
        /// <summary>
        /// Menu About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }
       
        private void MnuHelpAboutSong_Click(object sender, EventArgs e)
        {            
            string tx = string.Empty;
            int i;
            string cr = Environment.NewLine;

            // Karaoke infos
            for (i = 0; i < sequence1.KTag.Count; i++)
            {
                tx += sequence1.KTag[i] + cr;
            }
            
            tx += cr;
            // Version
            for (i = 0; i < sequence1.VTag.Count; i++)
            {
                tx += sequence1.VTag[i] + cr;
            }
            // Lang
            for (i = 0; i < sequence1.LTag.Count; i++)
            {
                tx += sequence1.LTag[i] + cr;
            }
            
            tx += cr;
            // Copyright of karaoke
            for (i = 0; i < sequence1.WTag.Count; i++)
            {
                tx += sequence1.WTag[i] + cr;
            }
            
            tx += cr;
            // Song infos
            for (i = 0; i < sequence1.TTag.Count; i++)
            {
                tx += sequence1.TTag[i] + cr;
            }
            
            tx += cr;
            // Infos
            for (i = 0; i < sequence1.ITag.Count; i++)
            {
                tx += sequence1.ITag[i] + cr;
            }


            MessageBox.Show(tx, "About this song", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        #endregion

        #endregion menus


        #region Lyrics

        /// <summary>
        /// Display informations on Lyrics
        /// </summary>
        public void DisplayLyricsInfos()
        {
            string tx = string.Empty;
            
            // FAB 28/08
            if (myLyricsMgmt != null)
                {
                tx = "Lyrics type: " + myLyricsMgmt.LyricType + "\r";
                tx += "Lyrics track: " + myLyricsMgmt.LyricsTrackNum.ToString() + "\r";
                tx += "Melody track: " + myLyricsMgmt.MelodyTrackNum.ToString();

                lblLyricsInfos.Text = tx;

                // Mute melody track
                MuteMelodyTrack(myLyricsMgmt.MelodyTrackNum);
            }

        }


        /// <summary>
        /// Replace existing lyrics by others
        /// MelodyTrackNum: track hosting the melody
        /// LyricsTrackNum: track hosting the lyrics (text or lyric types)
        /// The target is to host the lyrics in the melody track
        /// </summary>
        /// <param name="pLyrics"></param>
        public void ReplaceLyrics(List<plLyric> newpLyrics, LyricTypes newLyricType, int melodytracknum)
        {
            bool bRefreshDisplay = (newLyricType != myLyricsMgmt.LyricType);

            
            // Delete all lyrics of all types
            foreach (Track T in sequence1.tracks)
            {
                T.deleteLyrics();
                T.LyricsText.Clear();
                T.Lyrics.Clear();
            }
            // Tags associated to the sequence have been deleted
            restoreSequenceTags();


            // By default, insert the lyrics (either text or lyric) into the melodytrack
            Track track = sequence1.tracks[melodytracknum];
            
            // Insert all lyric events
            TrkInsertLyrics(track, newpLyrics, newLyricType);

            // Reload myLyricMgmt
            myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);

            // Refresh frmLyric

            if (myLyricsMgmt.OrgplLyrics.Count > 0)
            {
                // Window closed
                DisplayLyricsForm();
                // REset display
                frmLyric.ResetDisplayChordsOptions(myLyricsMgmt);
            }

            // Refresh display of lyrics
            // if switch between Text & Lyric or
            // if Lyric because we need to display the new lyrics on the scores
            if (bRefreshDisplay || myLyricsMgmt.LyricType == LyricTypes.Lyric)
            {
                RefreshDisplay();
            }


            // File was modified
            FileModified();

        }


        /// <summary>
        /// Insert new lyrics in the target track
        /// </summary>
        /// <param name="Track"></param>
        /// <param name="l"></param>
        /// <param name="LyricType"></param>
        private void TrkInsertLyrics(Track Track, List<plLyric> l, LyricTypes LyricType)
        {
            int currentTick = 0;
            int lastcurrenttick = 0;

            string currentElement = string.Empty;
            string currentType = string.Empty;
            string currentCR = string.Empty;
                        
            Track.Lyrics.Clear();
            Track.LyricsText.Clear();

            Track.TotalLyricsL = "";
            Track.TotalLyricsT = "";


            // Recréé tout les textes et lyrics
            for (int idx = 0; idx < l.Count; idx++)
            {
                plLyric pll = l[idx];

                // Si c'est un CR, le stocke et le collera au prochain lyric
                if (pll.CharType == plLyric.CharTypes.LineFeed)
                {
                    if (LyricType == LyricTypes.Text)
                        currentCR = m_SepLine;
                    else
                        currentCR = "\r";

                    // Update Track.Lyrics List
                    Track.Lyric L = new Track.Lyric()
                    {
                        Element = pll.Element.Item2,
                        TicksOn = pll.TicksOn,
                        Type = (Track.Lyric.Types)pll.CharType,
                    };
                    
                    if (LyricType == LyricTypes.Text)
                    {
                        // si lyrics de type text                     
                        Track.LyricsText.Add(L);
                    }
                    else
                    {
                        // si lyrics de type lyrics
                        Track.Lyrics.Add(L);
                    }

                }
                else if (pll.CharType == plLyric.CharTypes.ParagraphSep)
                {
                    if (LyricType == LyricTypes.Text)
                        currentCR = m_SepParagraph;
                    else
                        currentCR = "\r\r";


                    // Update Track.Lyrics List
                    Track.Lyric L = new Track.Lyric()
                    {
                        Element = pll.Element.Item2,
                        TicksOn = pll.TicksOn,
                        Type = (Track.Lyric.Types)pll.CharType,
                    };

                    if (LyricType == LyricTypes.Text)
                    {
                        // si lyrics de type text                     
                        Track.LyricsText.Add(L);
                    }
                    else
                    {
                        // si lyrics de type lyrics
                        Track.Lyrics.Add(L);
                    }
                }
                else if (pll.CharType == plLyric.CharTypes.Text)
                {
                    // C'est un lyric
                    currentTick = pll.TicksOn;
                    if (currentTick >= lastcurrenttick)
                    {
                        lastcurrenttick = currentTick;
                        currentElement = currentCR + pll.Element.Item2;

                        // Transforme en byte la nouvelle chaine
                        // ERROR FAB 16-01-2021 : must tyake into accout encoding selected by end user !!!
                        byte[] newdata; // = Encoding.Default.GetBytes(currentElement);

                        switch (OpenMidiFileOptions.TextEncoding)
                        {
                            case "Ascii":
                                //sy = System.Text.Encoding.Default.GetString(data);
                                newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                                break;
                            case "Chinese":
                                System.Text.Encoding chinese = System.Text.Encoding.GetEncoding("gb2312");
                                newdata = chinese.GetBytes(currentElement);
                                break;
                            case "Japanese":
                                System.Text.Encoding japanese = System.Text.Encoding.GetEncoding("shift_jis");
                                newdata = japanese.GetBytes(currentElement);
                                break;
                            case "Korean":
                                System.Text.Encoding korean = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                                newdata = korean.GetBytes(currentElement);
                                break;
                            case "Vietnamese":
                                System.Text.Encoding vietnamese = System.Text.Encoding.GetEncoding("windows-1258");
                                newdata = vietnamese.GetBytes(currentElement);
                                break;
                            default:
                                newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                                break;
                        }
                                               

                        MetaMessage mtMsg;

                        // Update Track.Lyrics List
                        Track.Lyric L = new Track.Lyric()
                        {
                            Element = pll.Element.Item2,
                            TicksOn = pll.TicksOn,
                            Type = (Track.Lyric.Types)pll.CharType,
                        };


                        if (LyricType == LyricTypes.Text)
                        {
                            // si lyrics de type text
                            mtMsg = new MetaMessage(MetaType.Text, newdata);
                            Track.LyricsText.Add(L);
                        }
                        else
                        {
                            // si lyrics de type lyrics
                            mtMsg = new MetaMessage(MetaType.Lyric, newdata);
                            Track.Lyrics.Add(L);
                        }

                        // Insert new message
                        Track.Insert(currentTick, mtMsg);
                    }
                    currentCR = "";
                }
            }
        }
      

        #region restore sequence tags
        /// <summary>
        /// Rewrite tags level sequence
        /// </summary>
        private void restoreSequenceTags()
        {
            string tx = string.Empty;
            int i;

            for (i = sequence1.ITag.Count - 1; i >= 0; i--)
            {
                tx = "@I" + sequence1.ITag[i];
                AddTag(tx);
            }
            for (i = sequence1.KTag.Count - 1; i >= 0; i--)
            {
                tx = "@K" + sequence1.KTag[i];
                AddTag(tx);
            }
            for (i = sequence1.LTag.Count - 1; i >= 0; i--)
            {
                tx = "@L" + sequence1.LTag[i];
                AddTag(tx);
            }
            for (i = sequence1.TTag.Count - 1; i >= 0; i--)
            {
                tx = "@T" + sequence1.TTag[i];
                AddTag(tx);
            }
            for (i = sequence1.VTag.Count - 1; i >= 0; i--)
            {
                tx = "@V" + sequence1.VTag[i];
                AddTag(tx);
            }
            for (i = sequence1.WTag.Count - 1; i >= 0; i--)
            {
                tx = "@W" + sequence1.WTag[i];
                AddTag(tx);
            }

        }

        /// <summary>
        /// Insert Tag at tick 0
        /// </summary>
        /// <param name="strTag"></param>
        private void AddTag(string strTag)
        {
            Track track = sequence1.tracks[0];
            int currentTick = 0;
            string currentElement = strTag;

            // Transforme en byte la nouvelle chaine
            byte[] newdata = new byte[currentElement.Length];
            for (int u = 0; u < newdata.Length; u++)
            {
                newdata[u] = (byte)currentElement[u];
            }

            MetaMessage mtMsg;

            mtMsg = new MetaMessage(MetaType.Text, newdata);

            // Insert new message
            track.Insert(currentTick, mtMsg);
        }

        #endregion restore sequence tags

        /// <summary>
        /// Delete all lyrics
        /// </summary>
        public void DeleteAllLyrics()
        {
            foreach (Track trk in sequence1.tracks)
            {
                trk.deleteLyrics();
                trk.Lyrics.Clear();
                trk.LyricsText.Clear();
            }

            myLyricsMgmt.plLyrics.Clear();
            // FAB 28/08
            //myLyric = new CLyric();
            bHasLyrics = false;

            // Ferme le formulaire frmLyric
            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                frmLyric.Close();
            }

            // File was modified
            FileModified();
        }


        
      

        /// <summary>
        /// Load form frmLyrics       
        /// </summary>
        private void DisplayLyricsForm()
        {                       
            string sSong = string.Empty;
            string sSinger = string.Empty;
            
            
            if (currentPlaylistItem != null)
            {                
                sSong = currentPlaylistItem.Song;
                sSinger = currentPlaylistItem.KaraokeSinger;
            }
            else
            {
                sSong = MIDIfileName;
            }
           

            // Window closed
            if (frmLyric == null || Application.OpenForms.OfType<frmLyric>().Count() == 0)
            {
                // Affiche les paroles
                if (currentPlaylistItem != null)
                    frmLyric = new frmLyric(myLyricsMgmt, true);
                else
                    frmLyric = new frmLyric(myLyricsMgmt, false);

                frmLyric.Show();
            }

            // Display song & current singer
            string tx = string.Empty;
            sSong = Path.GetFileNameWithoutExtension(sSong);
            if (sSinger == "" || sSinger == "<Song reserved by>")
                tx = sSong;
            else
                tx = sSong + " - " + Strings.Singer + ": " + sSinger;

            frmLyric.DisplaySinger(tx);
                      
            if (frmLyric.WindowState == FormWindowState.Minimized)
                frmLyric.WindowState = FormWindowState.Normal;

            frmLyric.Show();
            frmLyric.Activate();

            // cas d'une playlist ou non : met à jour le diaporama
            SetSlideShow();

        }

        private void SetSlideShow()
        {
            if (frmLyric != null)
            {
                // cas d'une playlist ou non : met à jour le diaporama
                if (currentPlaylistItem != null)
                    dirSlideShow = currentPlaylistItem.DirSlideShow;
                else
                    dirSlideShow = Properties.Settings.Default.dirSlideShow;

                frmLyric.SetSlideShow(dirSlideShow);

            }
        }

        #region deleteme
            /*
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
                    if ( sequence1.tracks[i].TotalLyricsT != null )
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
            */
            #endregion deleteme

            #endregion Lyrics


            #region handle messages

        private void SheetMusic_CurrentNoteChanged(MidiNote n)
        {            
            // Convert note number to letter
            string[] scale = { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };
            string notename = scale[(n.Number + 3) % 12];
            string s = CurrentNoteToString(n.Number, notename, n.StartTime, n.Duration, n.Velocity);
            lblCurNoteInfo.Text = s;

        }
        private string CurrentNoteToString(int note, string noteLetter, float ticks, int duration, int velocity)
        {
            float timeinmeasure = sheetmusic.GetTimeInMeasure(ticks);
            return string.Format("note {0} ({1}) - time {2} - ticks {3} - duration {4} - velocity {5}", note, noteLetter, timeinmeasure, ticks, duration, velocity);
        }

        /// <summary>
        /// Click on sheetmusic => track changed event
        /// </summary>
        /// <param name="tracknum"></param>
        private void SheetMusic_CurrentTrackChanged(int tracknum)
        {
            if (!bEditScore)
                return;
            
            // Unselect all track controls
            UnselectTrackControls();
            
            // Select track control
            SelectTrackControl(tracknum);
            
            // Color in red the key of the track
            SelectTrackKey(tracknum);
        }

        /// <summary>
        /// Event: loading of midi file terminated: launch song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string lyrics = string.Empty;
            this.Cursor = Cursors.Arrow;
            mnuFileOpen.Enabled = true;            
            progressBarPlayer.Value = 0;            
            progressBarPlayer.Visible = false;

            // Reset settings made for previous song
            ResetPlaySettings();

            if (frmLoading != null)
                frmLoading.Dispose();
            loading = false;

            if (e.Error == null && e.Cancelled == false)
            {                
                laststart = 0;

                // FAB : force le format à 1 hu hu hu sinon on ne peut pas ajouter de paroles            
                sequence1.Format = 1;
                
                myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
                bHasLyrics = myLyricsMgmt.OrgplLyrics.Count > 0; //myLyricsMgmt.LyricType != LyricTypes.None;                

                /*
                * Bug when format is 0, Karaboss change the format to 1.
                * If the file contains lyrics (not text), they are lost when the file is saved
                * Workaround is to rewrite the lyrics
                */

                if ((sequence1.OrigFormat == 0) && (myLyricsMgmt.LyricType == LyricTypes.Lyric))
                {
                    int tracknum = myLyricsMgmt.LyricsTrackNum;
                    Track track = sequence1.tracks[tracknum];
                    // supprime tous les messages text & lyric
                    track.deleteLyrics();

                    // Insert all lyric events
                    //InsTrkEvents(tracknum);
                    TrkInsertLyrics(track, myLyricsMgmt.OrgplLyrics, myLyricsMgmt.LyricType);
                }                

                              
                // Remove all MIDI events after last note
                sequence1.Clean();
                UpdateMidiTimes();


                #region displays controls

                positionHScrollBarNew.Value = 0;
                positionHScrollBarNew.Maximum = _totalTicks;

                // ----------------------------------------------------------------
                // Display Scores on panel pnlScrollView
                // ----------------------------------------------------------------
                DisplayScores();
               
                // Display song duration
                DisplaySongDuration();
                
                // Display track controls             
                DisplayTrackControls();

                // Reset tracks Stuff
                InitTracksStuff();
                #endregion

                // Display log file
                if (sequence1.Log != "")
                    lblChangesInfos.Text = sequence1.Log;

                DisplayFileInfos();


                #region display lyrics
                // Recherche si des lyrics existent et affiche la forme frmLyric
                mnuDisplayLyricsWindows.Checked = bKaraokeAlwaysOn;                           
                
                DisplayLyricsInfos();
                #endregion


                // PLAYLIST
                if (currentPlaylist != null)
                {
                    // Highlight current song in the playlist
                    UpdatePlayListsForm(currentPlaylistItem.Song);

                    // play asap, pause, countdown
                    performPlaylistChainingChoice();
                }
                else
                {
                    // SINGLE FILE

                    // the user asked to play the song immediately                
                    if (bPlayNow)
                        PlayPauseMusic();
                    else
                    {
                        // the user wants to edit the file 
                        if (bKaraokeAlwaysOn && bHasLyrics)
                            DisplayLyricsForm();
                    }
                }
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message);
            }
        }      

        /// <summary>
        /// Event: end loading XML music file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadXmlCompleted(object sender, AsyncCompletedEventArgs e)
        {                               
            if (MXmlReader.seq == null)
                return;

            string lyrics = string.Empty;
            this.Cursor = Cursors.Arrow;
            mnuFileOpen.Enabled = true;
            progressBarPlayer.Value = 0;
            progressBarPlayer.Visible = false;

            // ====================================
            // AJOUT par arraport au standard
            // ====================================
            MIDIfilePath = Path.GetDirectoryName(MIDIfileFullPath);
            string fExt = Path.GetExtension(MIDIfileFullPath);             // Extension
            string fName = Path.GetFileNameWithoutExtension(MIDIfileFullPath);    // name without extension
            MIDIfileName = fName + ".mid";
            MIDIfileFullPath = Path.Combine(MIDIfilePath, MIDIfileName);
            // fin ajout


            // Reset settings made for previous song
            ResetPlaySettings();

            if (frmLoading != null)
                frmLoading.Dispose();
            loading = false;

            sequence1 = MXmlReader.seq;
            sequence1.LoadCompleted += HandleLoadCompleted;  // restore property because info is lost (set in load form)
            sequence1.LoadProgressChanged += HandleLoadProgressChanged;

            if (e.Error == null && e.Cancelled == false)
            {
                laststart = 0;

                // FAB : force le format à 1 hu hu hu sinon on ne peut pas ajouter de paroles            
                sequence1.Format = 1;
                
                myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
                bHasLyrics = myLyricsMgmt.OrgplLyrics.Count > 0; //  myLyricsMgmt.LyricType != LyricTypes.None;                

                /*
                * Bug when format is 0, Karaboss change the format to 1.
                * If the file contains lyrics (not text), they are lost when the file is saved
                * Workaround is to rewrite the lyrics
                */

                if ((sequence1.OrigFormat == 0) && (myLyricsMgmt.LyricType == LyricTypes.Lyric))
                {
                    int tracknum = myLyricsMgmt.LyricsTrackNum;
                    Track track = sequence1.tracks[tracknum];
                    // supprime tous les messages text & lyric
                    track.deleteLyrics();

                    // Insert all lyric events
                    //InsTrkEvents(tracknum);
                    TrkInsertLyrics(track, myLyricsMgmt.OrgplLyrics, myLyricsMgmt.LyricType);
                }                               

                // Remove all MIDI events after last note
                sequence1.Clean();

                // ====================================
                // AJOUT par rapport au standard
                // ====================================
                ResetSequencer();
                sequencer1.Sequence = sequence1;
                // fin ajout

                UpdateMidiTimes();

                #region displays controls

                positionHScrollBarNew.Value = 0;
                positionHScrollBarNew.Maximum = _totalTicks;

                // ----------------------------------------------------------------
                // Display Scores on panel pnlScrollView
                // ----------------------------------------------------------------
                DisplayScores();

                // Display song duration
                DisplaySongDuration();

                // Display track controls             
                DisplayTrackControls();

                // REset tracks Stuff
                InitTracksStuff();
                #endregion


                // Display log file
                if (sequence1.Log != "")
                    lblChangesInfos.Text = sequence1.Log;
                DisplayFileInfos();

                #region display lyrics
                // Recherche si des lyrics existent et affiche la forme frmLyric
                mnuDisplayLyricsWindows.Checked = bKaraokeAlwaysOn;
                DisplayLyricsInfos();
                #endregion

                // PLAYLIST
                if (currentPlaylist != null)
                {
                    // Highlight current song in the playlist
                    UpdatePlayListsForm(currentPlaylistItem.Song);

                    // play asap, pause, countdown
                    performPlaylistChainingChoice();
                }
                else
                {
                    // SINGLE FILE

                    // Lance immédiatement la lecture du morceau                
                    if (bPlayNow)
                        PlayPauseMusic();
                    else
                    {
                        if (bKaraokeAlwaysOn && bHasLyrics)
                            DisplayLyricsForm();
                    }
                }
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message);
            }                
        }

        /// <summary>
        /// End loading dump text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (MTxtReader.seq == null)
                return;

            string lyrics = string.Empty;
            this.Cursor = Cursors.Arrow;
            mnuFileOpen.Enabled = true;
            progressBarPlayer.Value = 0;
            progressBarPlayer.Visible = false;

            // ====================================
            // AJOUT par rapport au standard
            // ====================================
            MIDIfileFullPath = ((MusicTxtReader)sender).fileName;

            MIDIfilePath = Path.GetDirectoryName(MIDIfileFullPath);
            string fExt = Path.GetExtension(MIDIfileFullPath);             // Extension
            string fName = Path.GetFileNameWithoutExtension(MIDIfileFullPath);    // name without extension
            MIDIfileName = fName + ".mid";
            MIDIfileFullPath = Path.Combine(MIDIfilePath, MIDIfileName);
            // fin ajout


            // Reset settings made for previous song
            ResetPlaySettings();

            if (frmLoading != null)
                frmLoading.Dispose();
            loading = false;

            sequence1 = MTxtReader.seq;
            sequence1.LoadCompleted += HandleLoadCompleted;  // restore property because info is lost (set in load form)
            sequence1.LoadProgressChanged += HandleLoadProgressChanged;

            if (e.Error == null && e.Cancelled == false)
            {
                laststart = 0;

                // FAB : force le format à 1 hu hu hu sinon on ne peut pas ajouter de paroles            
                sequence1.Format = 1;
                
                myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
                bHasLyrics = myLyricsMgmt.OrgplLyrics.Count > 0; // myLyricsMgmt.LyricType != LyricTypes.None;                

                /*
                * Bug when format is 0, Karaboss change the format to 1.
                * If the file contains lyrics (not text), they are lost when the file is saved
                * Workaround is to rewrite the lyrics
                */

                if ((sequence1.OrigFormat == 0) && (myLyricsMgmt.LyricType == LyricTypes.Lyric))
                {
                    int tracknum = myLyricsMgmt.LyricsTrackNum;
                    Track track = sequence1.tracks[tracknum];
                    // supprime tous les messages text & lyric
                    track.deleteLyrics();

                    // Insert all lyric events
                    //InsTrkEvents(tracknum);
                    TrkInsertLyrics(track, myLyricsMgmt.OrgplLyrics, myLyricsMgmt.LyricType);
                }
                               

                // Remove all MIDI events after last note
                sequence1.Clean();

                // ====================================
                // AJOUT par rapport au standard
                // ====================================
                ResetSequencer();
                sequencer1.Sequence = sequence1;
                // fin ajout

                UpdateMidiTimes();

                #region displays controls

                positionHScrollBarNew.Value = 0;
                positionHScrollBarNew.Maximum = _totalTicks;

                // ----------------------------------------------------------------
                // Display Scores on panel pnlScrollView
                // ----------------------------------------------------------------
                DisplayScores();

                // Display song duration
                DisplaySongDuration();

                // Display track controls             
                DisplayTrackControls();

                // REset tracks Stuff
                InitTracksStuff();
                #endregion


                #region display lyrics

                // Recherche si des lyrics existent et affiche la forme frmLyric
                mnuDisplayLyricsWindows.Checked = bKaraokeAlwaysOn;

                // Display log file
                if (sequence1.Log != "")
                    lblChangesInfos.Text = sequence1.Log;

                DisplayFileInfos();
                DisplayLyricsInfos();
                #endregion

                // PLAYLIST
                if (currentPlaylist != null)
                {
                    // Highlight current song in the playlist
                    UpdatePlayListsForm(currentPlaylistItem.Song);

                    // play asap, pause, countdown
                    performPlaylistChainingChoice();
                }
                else
                {
                    // SINGLE FILE

                    // Lance immédiatement la lecture du morceau                
                    if (bPlayNow)
                        PlayPauseMusic();
                    else
                    {
                        if (bKaraokeAlwaysOn && bHasLyrics)
                            DisplayLyricsForm();
                    }
                }
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message);
            }        
        }

        /// <summary>
        /// Event: save midi file terminated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            
            if (progressBarPlayer != null)
            {
                try
                {                  
                    progressBarPlayer.Value = 0;
                    progressBarPlayer.Visible = false;
                    
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }

            if (e.Error == null)
            {
                bfilemodified = false;
                if (bClosingRequired == true)
                {
                    this.Close();
                    return;
                }

                SetTitle(MIDIfileName);

                // Active le formulaire frmExplorer
                if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
                {
                    frmExplorer = GetForm<frmExplorer>();
                    frmExplorer.RefreshExplorer();                    
                }

            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        /// <summary>
        /// Ecent: Save Dump to text file terminated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {

            if (progressBarPlayer != null)
            {
                try
                {
                    progressBarPlayer.Value = 0;
                    progressBarPlayer.Visible = false;

                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }

            if (e.Error == null)
            {
                bfilemodified = false;
                if (bClosingRequired == true)
                {
                    this.Close();
                    return;
                }

                SetTitle(MIDIfileName);

                // Active le formulaire frmExplorer
                if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
                {
                    frmExplorer = GetForm<frmExplorer>();
                    frmExplorer.RefreshExplorer();
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }       

        private void PositionHScrollBarNew_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {

                sequencer1.Position = e.NewValue;

                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }

        /// <summary>
        /// Event: loading of midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loading = true;
            try
            {                
                progressBarPlayer.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

       
        /// <summary>
        /// Event: saving midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {               
                if (e.ProgressPercentage >= progressBarPlayer.Minimum && e.ProgressPercentage <= progressBarPlayer.Maximum)
                    progressBarPlayer.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void HandleMetaMessagePlayed(object sender, MetaMessageEventArgs e)
        {
            if (closing)
            {
                return;
            }
            //var a = e.Message.MessageType;

            if (e.Message.MetaType == MetaType.Tempo)
            {
                MetaMessage msg = e.Message;
                byte[] data = msg.GetBytes();
                _tempoplayed = ((data[0] << 16) | (data[1] << 8) | data[2]);

                
                

            }

        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if(closing)
            {
                return;
            }            

            int nChannel = e.Message.MidiChannel;
            string sChannel = nChannel.ToString();

            if (!lstChannels[nChannel].muted)
                outDevice.Send(e.Message);


            // Modify display according to changes during play 
            if (e.Message.Command == ChannelCommand.NoteOn)
            {
                // Allume la diode du channel correspondant                
                for (int i = 0; i < pnlTracks.Controls.Count; i++)
                {
                    if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                    {
                        if (pnlTracks.Controls[i].Tag != null)
                        {
                            string stag = pnlTracks.Controls[i].Tag.ToString();
                            if (stag == sChannel)
                            {
                                ((TrkControl.TrackControl)pnlTracks.Controls[i]).LightOn();
                            }
                        }
                    }
                }
            }
            else if (e.Message.Command == ChannelCommand.NoteOff)
            {
                // Eteint la diode du channel correspondant
                for (int i = 0; i < pnlTracks.Controls.Count; i++)
                {
                    if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                    {
                        if (pnlTracks.Controls[i].Tag != null)
                        {
                            string stag = pnlTracks.Controls[i].Tag.ToString();
                            if (stag == sChannel)
                            {
                                ((TrkControl.TrackControl)pnlTracks.Controls[i]).LightOff();
                            }
                        }
                    }
                }
                
            }
            else if (e.Message.Command == ChannelCommand.Controller)
            {                                                                     
                ChannelMessage Msg = e.Message;                   
                ControllerType ct = (ControllerType)Msg.Data1;
                
                   
                if (ct == ControllerType.Volume)
                {                    
                    int vol = Msg.Data2;
                    int j = -1;

                    for (int i = 0; i < pnlTracks.Controls.Count; i++)
                    {
                        if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                        {
                            j++;
                            if (pnlTracks.Controls[i].Tag != null)
                            {
                                string stag = pnlTracks.Controls[i].Tag.ToString();
                                if (stag == sChannel)
                                {
                                    // Adjust volume for all tracks having this channel
                                    lstTrkReglages[j].volume = vol;                                    
                                }
                            }
                        }                                
                    }
                    bReglageChanged = true;
                }
                else if (ct == ControllerType.Pan)
                {
                    int pan = Msg.Data2;
                    int j = -1;
                    for (int i = 0; i < pnlTracks.Controls.Count; i++)
                    {
                        if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                        {
                            j++;
                            if (pnlTracks.Controls[i].Tag != null)
                            {
                                string stag = pnlTracks.Controls[i].Tag.ToString();
                                if (stag == sChannel)
                                {
                                    // Ajust pan for all tracks having this channel
                                    lstTrkReglages[j].pan = pan;                                    
                                }
                            }
                        }
                    }
                    bReglageChanged = true;
                }
                else if (ct == ControllerType.EffectsLevel)
                {
                    int reverb = Msg.Data2;
                    int j = -1;
                    for (int i = 0; i < pnlTracks.Controls.Count; i++)
                    {
                        if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                        {
                            j++;
                            if (pnlTracks.Controls[i].Tag != null)
                            {
                                string stag = pnlTracks.Controls[i].Tag.ToString();
                                if (stag == sChannel)
                                {
                                    // Ajust reverb for all tracks having this channel
                                    lstTrkReglages[j].reverb = reverb;                                    
                                }
                            }
                        }
                    }
                    bReglageChanged = true;
                }                
            }
            else if (e.Message.Command == ChannelCommand.ProgramChange)
            {
                // Instrument is changed during play !!!!!
                ChannelMessage Msg = e.Message;
                int patch = Msg.Data1;
                int j = -1;                

                for (int i = 0; i < pnlTracks.Controls.Count; i++)
                {
                    if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                    {
                        j++;
                        if (pnlTracks.Controls[i].Tag != null)
                        {
                            string stag = pnlTracks.Controls[i].Tag.ToString();
                            if (stag == sChannel)
                            {
                                // Ajust patch for all tracks having this programchange
                                lstTrkReglages[j].patch = patch;
                            }
                        }
                    }
                }
                bReglageChanged = true;

            }
            
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
       //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);                
            }
        }

        /// <summary>
        /// Event: playing midi file completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            // Select next song of a playlist
            PlayerState = PlayerStates.NextSong;
        }


        #endregion handle messages


        #region timers

        /// <summary>
        /// Display Time Elapse
        /// </summary>
        private void DisplayTimeElapse(double dpercent)
        {
            lblPercent.Text = string.Format("{0}%", (int)dpercent);

            double maintenant = (dpercent * _duration) / 100;  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            lblElapsed.Text = string.Format("{0:00}:{1:00}", Min, Sec);
        }

        /// <summary>
        /// timer: for playing song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(!scrolling)
            {

                // Display time elapse
                double dpercent = 100 * sequencer1.Position / (double)_totalTicks;
                double maintenant = (dpercent * _duration) / 100;  //seconds
                DisplayTimeElapse(dpercent);              

                //Eteint la boule fixe;
                //if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                //    frmLyric.UnlightFixedBall();
                
                switch (PlayerState)
                {
                    case PlayerStates.Playing:                        
                        ScrollTimeBar(maintenant);
                        GetPeakVolume();
                        // Mute muted tracks
                        //if (bMuted)
                        //    CheckMutedTracks();
                        // Volume of tracks
                        if (bVolumed)
                            CheckVolumedTracks();                       
                        break;
                        
                    
                    case PlayerStates.Stopped:                        
                        timer1.Stop();                       
                        timer2.Stop(); // Lyrics                        
                        timer3.Stop(); // Balls                       
                        timer4.Stop(); // BEAT                            
                        AfterStopped();
                        break;
                        
                    
                    case PlayerStates.Paused:                                                                                                        
                        sequencer1.Stop();
                        timer1.Stop();                         
                        timer2.Stop(); // Lyrics                        
                        timer3.Stop(); // Balls                       
                        timer4.Stop(); // BEAT                                 
                        break;                        

                    case PlayerStates.NextSong:                        
                        AfterStopped();                        
                        // Select next song of a playlist                        
                        SelectNextPlaylistSong();                       
                        break;

                    case PlayerStates.Waiting:        // Count down running between 2 songs of a playlist     
                        timer1.Stop();                         
                        timer2.Stop(); // Lyrics                        
                        timer3.Stop(); // Balls                        
                        timer4.Stop(); // BEAT     
                        break;

                    case PlayerStates.WaitingPaused:        // Count down paused between 2 songs of a playlist
                        timer1.Stop();
                        timer2.Stop(); // Lyrics                        
                        timer3.Stop(); // Balls                        
                        timer4.Stop(); // BEAT     
                        break;

                    case PlayerStates.LaunchNextSong:   // pause between 2 songs of a playlist
                        timer1.Stop();
                        timer2.Stop(); // Lyrics                        
                        timer3.Stop(); // Balls                        
                        timer4.Stop(); // BEAT     
                        break;

                }

                #region position hscrollbar
                try
                {
                    if (PlayerState == PlayerStates.Playing && sequencer1.Position < positionHScrollBarNew.Maximum)
                        positionHScrollBarNew.Value = sequencer1.Position;                   
                }
                catch (Exception ex)
                {
                    Console.Write("Error positionHScrollBarNew.Value - " + ex.Message);
                }
                #endregion position hscrollbar
            }
        }


        /// <summary>
        /// timer: for lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer2_Tick(object sender, EventArgs e)
        {
            // Objectif : mettre à jour l'indice courant dans le tableau des Lyrics 
            // et colorier la syllabe à chanter   
            if (PlayerState == PlayerStates.Playing)
            {
                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                    frmLyric.ColorLyric(sequencer1.Position);
            }
        }


        /// <summary>
        /// Timer 3: for balls animation (faster than timer 2) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer3_Tick(object sender, EventArgs e)
        {
            // 21 balls: 1 fix, 20 moving to the fix one
            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                if (frmLyric != null)
                    frmLyric.MoveBalls(sequencer1.Position);
            }
        }

        
        /// <summary>
        /// Timer4 : only display beat
        /// Intervall is equal to tempo of midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer4_Tick(object sender, EventArgs e)
        {

            #region beat animation
            // BEAT
           
            int mb = sequence1.Numerator + 1;

            beat++;
            if (beat == mb)
                beat = 1;

            lblBeat.Text = beat.ToString() + "|" + sequence1.Numerator;

            // Light off all channels
            // Display volume of tracks and other setups
            
            int j = -1;
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    j++;
                    if (pnlTracks.Controls[i].Tag != null)
                    {                        
                        TrkControl.TrackControl trkctrl = ((TrkControl.TrackControl)pnlTracks.Controls[i]);                       
                        // Light Off
                        trkctrl.LightOff();


                        // Change values only if differents?
                        if (bReglageChanged == true)
                        {
                            // Volume                                                
                            trkctrl.SetVolume(lstTrkReglages[j].volume);
                            // Pan
                            trkctrl.SetPan(lstTrkReglages[j].pan);
                            // Reverb
                            trkctrl.SetReverb(lstTrkReglages[j].reverb);
                            // Patch
                            trkctrl.SetPatch(lstTrkReglages[j].patch);
                        }                        
                    }
                }
            }
            bReglageChanged = false;

            #endregion beat animation


            // Tempo change during play
            //if (_tempoplayed != _tempo)
                DisplayFileInfos(_tempoplayed);

        }

        /// <summary>
        /// Timer 5 : time between 2 songs
        /// Interval = 1 sec
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer5_Tick(object sender, EventArgs e)
        {            
            // Si pause next song, afficher le text de la prochaine chansons        
            w_tick++;            

            // Wait until X sec
            if (w_tick < w_wait)
            {
                // color each second
                if (frmLyric != null)
                    frmLyric.ColorLyric(w_tick * 10);
            }
            else if (w_tick == w_wait)
            {
                // set syllabes to null
                if (frmLyric!= null)
                    frmLyric.EndWaitSong();
            }
            else
            {
                // Countdown completed, Play next song of the play list
                timer5.Enabled = false;              
                PlayerState = PlayerStates.Stopped;

                // Restore display options modified by the wait animation
                if (frmLyric != null)
                {
                    frmLyric.LoadKarOptions();
                    SetSlideShow();
                }
                PlayPauseMusic();

            }
        }

       

        #endregion timers


        #region ani balls

        /// <summary>
        /// Start balls animation
        /// </summary>
        private void StartTimerBalls()
        {           
            timer3.Interval = 1;
            timer3.Start();            

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                frmLyric.StartTimerBalls();            
        }
        
        /// <summary>
        /// Terminate balls animation
        /// </summary>
        private void StopTimerBalls()
        {
            timer3.Stop();            

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                frmLyric.StopTimerBalls();

        }

        #endregion ani balls


        #region form load close keydown


        /// <summary>
        /// Mousewheel : scroll vertically if playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (sheetmusic == null) return;

            int newvalue = 0;
            int W = sheetmusic.MaxStaffWidth;


            // If playing, scroll vertically both leftPanel & score
            if (PlayerState == PlayerStates.Playing)
            {
                if (bShowVScrollBar)
                {
                    // Scroll vertically Left panel
                    newvalue = vScrollBar.Value - e.Delta;
                    if (newvalue < 0)
                        newvalue = 0;
                    if (newvalue > vScrollBar.Maximum - vScrollBar.LargeChange)
                        newvalue = vScrollBar.Maximum - vScrollBar.LargeChange;

                    vScrollBar.Value = newvalue;
                }
            }
            else
            {
                // If not playing,
                
                //scroll vertically the left panel if mouse is over it
                if (e.Location.X > 0 & e.Location.X < pnlTracks.Location.X + pnlTracks.Width)
                {
                    if (bShowVScrollBar)
                    {
                        // Scroll vertically Left panel
                        newvalue = vScrollBar.Value - e.Delta;
                        if (newvalue < 0)
                            newvalue = 0;
                        if (newvalue > vScrollBar.Maximum - vScrollBar.LargeChange)
                            newvalue = vScrollBar.Maximum - vScrollBar.LargeChange;

                        vScrollBar.Value = newvalue;
                    }
                }
                else if (e.Location.X > pnlScrollView.Location.X && e.Location.X < pnlScrollView.Location.X + W)
                {
                    if (bShowHScrollBar)
                    {
                        // Scroll horizontaly right panel
                        newvalue = hScrollBar.Value - e.Delta;
                        if (newvalue < 0)
                            newvalue = 0;
                        if (newvalue > hScrollBar.Maximum - hScrollBar.LargeChange)
                            newvalue = hScrollBar.Maximum - hScrollBar.LargeChange;

                        hScrollBar.Value = newvalue;
                    }
                }
            }
        }


        /// <summary>
        /// Override form load event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (outDevice == null)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
            else
            {               
                try
                {
                    outDeviceProcessId = outDevice.Pid;

                    string outDeviceName = OutputDeviceBase.GetDeviceCapabilities(outDevice.DeviceID).name;
                    lblOutputDevice.Text = outDeviceName;

                    AlertOutputDevice(outDeviceName);

                    sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;                    

                    // ==========================================================================
                    // Chargement du fichier midi selectionné depuis frmExplorer
                    // ==========================================================================

                    ResetMidiFile();

                    // ACTIONS TO PERFORM
                    SelectActionOnLoad();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Close();
                }
            }       
            base.OnLoad(e);
        }

     
        /// <summary>
        /// Select what to do on load: new score, play single file, or playlist 
        /// </summary>
        private void SelectActionOnLoad()
        {           
            // Same for start a playlist or a single file (mid, xml, txt)
            if (MIDIfileFullPath != null && MIDIfileFullPath != "")
            {
                SelectFileToLoadAsync();
            }
            else
            {
                // A new file must be created                                                              
                NewMidiFile();
            }
        }

        /// <summary>
        /// Select loader according to extension (mid, xml, txt)
        /// </summary>
        private void SelectFileToLoadAsync()
        {
            string ext = Path.GetExtension(MIDIfileFullPath).ToLower();
            if (ext == ".mid" || ext == ".kar")
            {
                // Play a single MIDI file
                LoadAsyncMidiFile(MIDIfileFullPath);
            }
            else if (ext == ".xml" || ext == ".musicxml")
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                LoadAsyncXmlFile(MIDIfileFullPath);
            }
            else if (ext == ".txt")
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
                LoadAsyncTxtFile(MIDIfileFullPath);
            }
            else
            {
                MessageBox.Show("Unknown extension");
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }        
                
        protected override void OnClosed(EventArgs e)
        {            
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice!= null && !outDevice.IsDisposed)
                outDevice.Reset();
         
            base.OnClosed(e);
        }

        /// <summary>
        /// Event: form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loading)
            {
                e.Cancel = true;
            }
            else
            {
                
                // Remove edit forms like frmNoteEdit, frmModifyTempo etcc (always on top)
                // They can hide the messagebox asking for saving the file
                DspEdit(false);
                
                if (bfilemodified == true)
                {
                    
                    // string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                    String tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;
                    DialogResult dr = MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (dr == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (dr == DialogResult.Yes) 
                    {
                        e.Cancel = true;
                        // turlututu
                        bClosingRequired = true;
                        SaveFileProc();
                        return;
                    }                   
                }
                
                // enregistre la taille et la position de la forme
                // Copy window location to app settings                
                if (WindowState != FormWindowState.Minimized)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Properties.Settings.Default.frmPlayerLocation = RestoreBounds.Location;
                        Properties.Settings.Default.frmPlayerMaximized = true;

                    }
                    else if (WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.frmPlayerLocation = Location;

                        // SDave only if not default size
                        if (Height != SimplePlayerHeight)
                            Properties.Settings.Default.frmPlayerSize = Size;

                        Properties.Settings.Default.frmPlayerMaximized = false;
                    }

                    // Show sequencer
                    Properties.Settings.Default.ShowSequencer = bSequencerAlwaysOn;
                    Properties.Settings.Default.ShowKaraoke = bKaraokeAlwaysOn;

                    // Save settings
                    Properties.Settings.Default.Save();
                }
                
                                            
                // Ferme le formulaire frmLyric
                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    frmLyric.Close();
                    //frmLyric.Dispose();
                }                
                // ferme le formulaire frmLyricsEdit
                if (Application.OpenForms.OfType<frmLyricsEdit>().Count() > 0)
                {                    
                    Application.OpenForms["frmLyricsEdit"].Close();
                }
                // ferme le formulaire frmPianoRoll
                if (Application.OpenForms.OfType<frmPianoRoll>().Count() > 0)
                {
                    Application.OpenForms["frmPianoRoll"].Close();
                }
                // ferme le formulaire frmModifyTempo
                if (Application.OpenForms.OfType<frmModifyTempo>().Count() > 0)
                {
                    Application.OpenForms["frmModifyTempo"].Close();
                }
                // ferme le formulaire frmPrint
                if (Application.OpenForms.OfType<frmPrint>().Count() > 0)
                {
                    Application.OpenForms["frmPrint"].Close();
                }
                // Active le formulaire frmExplorer
                if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
                {                    
                    // Restore form
                    Application.OpenForms["frmExplorer"].Restore();
                    Application.OpenForms["frmExplorer"].Activate();

                }


                Dispose();

            }
        }

        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPlayer_Load(object sender, EventArgs e)
        {
            // Set window location and size
            #region window size & location
            // If window is maximized
            if (Properties.Settings.Default.frmPlayerMaximized)
            {                
                Location = Properties.Settings.Default.frmPlayerLocation;
                //Size = Properties.Settings.Default.frmPlayerSize;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmPlayerLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)                
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);                

                Size = Properties.Settings.Default.frmPlayerSize;
            }
            #endregion

            // Ne pas tenir compte si new file ou edit file
            bSequencerAlwaysOn = Properties.Settings.Default.ShowSequencer;
            bKaraokeAlwaysOn = Properties.Settings.Default.ShowKaraoke;
            
            // Redim form according to the visibility of the sequencer
            RedimIfSequencerVisible();
            
        }

        
        /// <summary>
        /// Form Keydown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            if (bEditScore == true)
            {
                #region edit score
                /*
                // Move up or down a single note
                if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "Up")
                {
                    // move Up or Down a single note 
                    Key_UpDownNote(e.KeyCode.ToString());
                }
                */

                // Edit notes
                if (bEnterNotes == false)
                {
                    // EditScore = true and EnterNotes = false
                    // Ctrl + N > valide EnterNotes
                    // Left, Right > Move the current note
                    // Delete > delete selection
                    switch (e.KeyCode)
                    {                           
                        case Keys.E:
                            if (e.Control)
                                DspEdit(!bEditScore);
                            break;

                        case Keys.N:
                            if (e.Control)
                                DspEnterNotes();
                            break;

                        case Keys.Delete:
                            sheetmusic.SheetMusic_KeyDown(sender, e);
                            break;
                    }                    
                }
                else if (bEnterNotes == true)
                {                    
                    // EditScore = true and EnterNotes = true
                    // enter notes C, D, E F, G, A, B
                    // Delete notes tec...
                    switch (e.KeyCode)
                    {
                        case Keys.C:
                            Key_AddNote(0);
                            break;
                        case Keys.D:
                            Key_AddNote(2);
                            break;
                        case Keys.E:
                            if (e.Control)
                                DspEdit(!bEditScore);
                            else
                                Key_AddNote(4);
                            break;
                        case Keys.F:
                            Key_AddNote(5);
                            break;
                        case Keys.G:
                            Key_AddNote(7);
                            break;
                        case Keys.A:
                            Key_AddNote(9);
                            break;
                        case Keys.B:
                            Key_AddNote(11);
                            break;

                        case Keys.N:
                            // Set EnterNotes = false
                            if (e.Control)
                                DspEnterNotes();
                            break;

                        case Keys.Add:
                        case Keys.Subtract:
                        case Keys.D6:
                        case Keys.Decimal:
                            // plus, minus > select more or less duration for entering notes
                            KeyboardSelectDurations(e);
                            break;

                        case Keys.Back:
                            Key_DelNote();
                            break;

                        case Keys.Delete:
                            Key_DelNote();
                            break;

                        case Keys.Space:
                            // Set enterNotes = false and start playing
                            DspEnterNotes();
                            PlayPauseMusic();
                            break;

                    }                    
                }                
                #endregion edit score
            }
            else 
            {
                // not in editing mode
                // Manage launch player with spacebar
                #region Player
                switch (e.KeyCode)
                {                    
                    case Keys.E:
                        if (e.Control)
                            DspEdit(!bEditScore);
                        break;

                    case Keys.N:
                        // Set enterNotes = true
                        if (e.Control)
                            DspEnterNotes();
                        break;

                    case Keys.Add:
                    case Keys.Subtract:
                    case Keys.D6:
                    case Keys.Decimal:
                        // Tempo +-
                        KeyboardSelectTempo(e);
                        break;

                }
                #endregion Player
            }
        }

        /// <summary>
        /// I am able to detect alpha-numeric keys. However i am not able to detect arrow keys
        /// ProcessCmdKey save my life
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (bEditScore == false)
            {
                if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped && newstart > 0))
                {
                    if (keyData == Keys.Left)
                    {
                        StopMusic();
                        return true;
                    }
                }
            } 
            else if (bEditScore == true)
            {
                switch (keyData)
                {
                    case Keys.Left:
                        Key_Left();
                        return true;
                    case Keys.Right:
                        Key_Right();
                        return true;                       
                }

                if (keyData.ToString() == "Down" || keyData.ToString() == "Up")
                {
                    // move Up or Down a single note 
                    Key_UpDownNote(keyData.ToString());
                }


            }
                
            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// Key Up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPlayer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Oemplus:
                        // Tempo +-
                        KeyboardSelectTempo(e);
                        break;
                }
            }
            else if (bEditScore == false)
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:                        
                        PlayPauseMusic();
                        break;                        

                    case Keys.F12:
                        bSequencerAlwaysOn = !bSequencerAlwaysOn;
                        // bForceShowSequencer was true, but user decided to hide the sequencer by clicking on the menu
                        if (bSequencerAlwaysOn == false && bForceShowSequencer == true)
                            bForceShowSequencer = false;
                        RedimIfSequencerVisible();
                        break;
                }
            }
            else if (bEnterNotes == true)
            {
                #region edit score
                if (e.Modifiers == Keys.Shift)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Oemplus:
                        case Keys.OemPeriod:
                        case Keys.Decimal:
                            KeyboardSelectDurations(e);
                            break;
                    }                        
                    return;                    
                }
                

                if (e.KeyCode.ToString() == "Down" || e.KeyCode.ToString() == "Up" || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {                    
                    StopNote();
                }
                else
                {
                    switch (e.KeyCode)
                    {
                        case Keys.C:                        
                        case Keys.D:
                        case Keys.E:
                        case Keys.F:
                        case Keys.G:
                        case Keys.A:
                        case Keys.B:
                        case Keys.Back:
                            StopNote();
                            break;
                    }
                }
                #endregion
            } 
        }


        /** When the window is resized, adjust the pnlScrollView to fill the window */
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);          

            if (sheetmusic != null)
                sheetmusic.Redraw();
            
            SetScrollBarValues();            
        }    

        #endregion form load close keydown


        #region track control

        /// <summary>
        /// Track Control : change track name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="trackname"></param>
        private void Trk_TrackName(object sender, EventArgs e, string trackname)
        {            
            if (sender is TrkControl.TrackControl pTrack)
            {
                int nChannel = Convert.ToInt32(pTrack.MidiChannel); 

                Track track = sequence1.tracks[pTrack.Track];
                track.Name = trackname;

                // Delete existing trackname MetaMessage
                track.RemoveTrackname();

                // Insert trackname message at position 0
                track.insertTrackname(trackname);

                FileModified();               
            }
        }      
     
        /// <summary>
        /// Track Control : volume
        /// </summary>
        /// <param name="sender"></param>
        void TrackBarVolumeChanged(object sender)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                bVolumed = true;
                int nChannel = Convert.ToInt32(pTrack.Tag);
                int c = 27;
                int v = pTrack.Volume;
                if (v > 127)
                    v = 127;
                SendCC(nChannel, c, v);

                Track track = sequence1.tracks[pTrack.Track];
                track.RemoveVolume();
                track.insertVolume(nChannel, v);

                lstTrkReglages[sequence1.tracks.IndexOf(track)].volume = v;
                track.Volume = v;
                
                //if (!pTrack.Muted)
                FileModified();
            }
        }

        /// <summary>
        /// Track Control: Reverb
        /// </summary>
        /// <param name="sender"></param>
        private void TrackBarReverbChanged(object sender)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                int nChannel = Convert.ToInt32(pTrack.Tag);
                ChannelMessageBuilder builder = new ChannelMessageBuilder()
                {
                    Command = ChannelCommand.Controller,
                    MidiChannel = nChannel,
                    Data1 = (int)ControllerType.EffectsLevel,
                    Data2 = pTrack.Reverb,
                };

                builder.Build();
                outDevice.Send(builder.Result);

                Track track = sequence1.tracks[pTrack.Track];
                track.RemoveReverb();
                track.insertReverb(nChannel, pTrack.Reverb);

                lstTrkReglages[sequence1.tracks.IndexOf(track)].reverb = pTrack.Reverb;
                track.Reverb = pTrack.Reverb;
                FileModified();
            }
        }

        /// <summary>
        /// Track Control: Pan
        /// </summary>
        /// <param name="sender"></param>
        private void TrackBarPanChanged(object sender)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                int nChannel = Convert.ToInt32(pTrack.Tag);
                ChannelMessageBuilder builder = new ChannelMessageBuilder()
                {
                    Command = ChannelCommand.Controller,
                    MidiChannel = nChannel,
                    Data1 = (int)ControllerType.Pan,
                    Data2 = pTrack.Pan,
                };

                builder.Build();
                outDevice.Send(builder.Result);

                Track track = sequence1.tracks[pTrack.Track];
                track.RemovePan();
                track.insertPan(nChannel, pTrack.Pan);

                lstTrkReglages[sequence1.tracks.IndexOf(track)].pan = pTrack.Pan;
                track.Pan = pTrack.Pan;
                FileModified();
            }
        }

        /// <summary>
        /// Click event on a track control
        /// </summary>
        /// <param name="sender"></param>
        private void TrackControl_Click(object sender)
        {
            // unselect all track controls
            UnselectTrackControls();

            TrkControl.TrackControl pTrack = sender as TrkControl.TrackControl;
            pTrack.Selected = true;
            sheetmusic.SelectedStaff = pTrack.Track;            
        }

        /// <summary>
        /// Unselect all track controls
        /// </summary>
        private void UnselectTrackControls()
        {
            foreach (Control ctl in pnlTracks.Controls)
            {
                if (ctl.GetType() == typeof(TrkControl.TrackControl))
                {
                    ((TrkControl.TrackControl)ctl).Selected = false;
                }
            }
        }

        /// <summary>
        /// Select track control having its track number
        /// </summary>
        /// <param name="tracknum"></param>
        private void SelectTrackControl(int tracknum)
        {
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl trk = (TrkControl.TrackControl)pnlTracks.Controls[i];
                    if (trk.Track == tracknum)
                    {
                        trk.Selected = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Enable/Disable track controls depending on edit status
        /// </summary>
        /// <param name="bEnable"></param>
        private void EnableTrackControls(bool bEnable)
        {
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl trk = (TrkControl.TrackControl)pnlTracks.Controls[i];
                    trk.Enabled = bEnable;
                }
            }
        }

        /// <summary>
        /// Track Control: Delete a track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="track"></param>
        void BtnDelClickOneEvent(object sender, EventArgs e, int track)
        {            
            if (sender is TrkControl.TrackControl pTrack)
            {
                if (sequence1.Format != 0)
                {
                    // TODO delete track                                            
                    sequence1.TrackDelete(sequence1.tracks[track]);
                    pTrack.Dispose();

                    DisplayTrackControls();

                    // Reset tracks Stuff
                    InitTracksStuff();

                    sheetmusic.Refresh();

                    SetStartVLinePos(0);
                    SetTimeVLinePos(0);

                    SetScrollBarValues();

                    FileModified();
                }
            }            
        }

        #region Mute & Solo

        /// <summary>
        /// Track Control: button Mute channel click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnMutClickOneEvent(object sender, EventArgs e, int patch)
        {

            if (sender is TrkControl.TrackControl pTrack)
            {
                int nChannel = Convert.ToInt32(pTrack.MidiChannel);
                string sChannel = nChannel.ToString();

                if (pTrack.Muted == false)
                {
                    // Stop Channel : All notes off                                        
                    sequencer1.AllSoundOff();                                        

                    // Mute other TrackControls having same channel
                    MuteSomeTracks(nChannel);
      
                }
                else
                {
                    // Restart channel : play again                    
                                        
                    // Unmute other TrackControls having same channel
                    UnMuteSomeTracks(nChannel);


                }
            }
        }


        /// <summary>
        /// Mute all tracks having the same channel
        /// </summary>
        /// <param name="nChannel"></param>
        private void MuteSomeTracks(int nChannel)
        {
            lstChannels[nChannel].muted = true;

            foreach (Control C in pnlTracks.Controls)
            {
                if (C.GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl T = (TrkControl.TrackControl)C;
                    if (T.MidiChannel == nChannel)
                        T.Muted = true;
                }
            }
        }

        /// <summary>
        /// unmute all tracks having channel sChannel
        /// </summary>
        /// <param name="sChannel"></param>
        private void UnMuteSomeTracks(int nChannel)
        {
            lstChannels[nChannel].muted = false;

            // unmute all tracks having same channel
            foreach (Control C in pnlTracks.Controls)
            {
                if (C.GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl T = (TrkControl.TrackControl)C;
                    
                    if (T.MidiChannel == nChannel)                                            
                        T.Muted = false;                    
                }
            }          
        }

        /// <summary>
        /// Unmute all tracks
        /// </summary>
        private void UnMuteAllTracks()
        {

            for (int i = 0; i < lstChannels.Count; i++)
            {
                lstChannels[i].muted = false;
            }

            // All tracks on                    
            foreach (Control C in pnlTracks.Controls)
            {
                if (C.GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl T = (TrkControl.TrackControl)C;
                    T.Solo = false;
                    T.Muted = false;
                }
            }
        }


        /// <summary>
        /// Click on button Solo track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="patch"></param>
        private void BtnSoloClickOneEvent(object sender, EventArgs e, int patch)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                int nChannel = Convert.ToInt32(pTrack.MidiChannel);                
                
                if (pTrack.Solo == false)
                {

                    // Stop Channel : All notes off                                        
                    sequencer1.AllSoundOff();

                    pTrack.Solo = true;
                    pTrack.Muted = false;

                    // Mute all channels <> nChannel
                    for (int i = 0; i < lstChannels.Count; i++)
                    {
                        if (i != nChannel)
                            lstChannels[i].muted = true;
                        else
                            lstChannels[i].muted = false;
                    }

                    
                    foreach (Control C in pnlTracks.Controls)
                    {
                        if (C.GetType() == typeof(TrkControl.TrackControl))
                        {
                            TrkControl.TrackControl T = (TrkControl.TrackControl)C;
                            if (T.MidiChannel != nChannel)
                            {
                                T.Solo = false;
                                T.Muted = true;
                            }
                        }
                    }   
                }
                else
                {
                    UnMuteAllTracks();                    
                }
            }
        }
        
        private void CheckVolumedTracks()
        {
            bool bfound = false;
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    TrkControl.TrackControl trackCtrl = (TrkControl.TrackControl)pnlTracks.Controls[i];
                    int volume = trackCtrl.Volume;
                    int j = trackCtrl.Track;

                    if (volume != sequence1.tracks[j].Volume )
                    {
                        if (trackCtrl.Tag != null)
                        {
                            bfound = true;

                            int nChannel = Convert.ToInt32(trackCtrl.Tag);
                            int c = (int)ControllerType.Volume;
                            int v = volume;
                            SendCC(nChannel, c, v);
                        }
                    }
                }
            }
            bVolumed = bfound;
        }

        #endregion

        /// <summary>
        /// Display PianoRoll for this track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="track"></param>
        private void BtnPianoRollClickOneEvent(object sender, EventArgs e, int track)
        {            
            float max = sequence1.GetLength();
            float t = max * (sheetmusic.OffsetX + pnlScrollView.Width/2) / (float)sheetmusic.MaxStaffWidth;
            

            //float pcent =  (float)hScrollBar.Value / (uint)(hScrollBar.Maximum - hScrollBar.Minimum);
            //float t = pcent * sequence1.GetLength();
            
            DisplayPianoRoll(track, MIDIfileFullPath, (int)t);
        }


        /// <summary>
        /// Track Control: change instrument
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LstInstrumentClickOneEvent(object sender, EventArgs e, int patch)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                Track track = sequence1.tracks[pTrack.Track];
                if (track.ProgramChange == patch)
                    return;

                // Change the patch while playing
                int nChannel = Convert.ToInt32(pTrack.MidiChannel);

                int c = patch; // instrument
                int v = 0;

                ChannelMessageBuilder builder = new ChannelMessageBuilder()
                {
                    Command = ChannelCommand.ProgramChange,
                    MidiChannel = nChannel,
                    Data1 = c,
                    Data2 = v,
                };
                builder.Build();
                outDevice.Send(builder.Result);

                // Really change the patch into the track                
                track.changePatch(patch);
                FileModified();                             
            }
        }

        /// <summary>
        /// Track Control : change channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="midichannel"></param>
        private void LstChannelClickOneEvent(object sender, EventArgs e, int midichannel)
        {
            //TrkControl.trackControl pTrack = sender as TrkControl.trackControl;
            //if (pTrack != null)
            if (sender is TrkControl.TrackControl pTrack)
            {
                Track track = sequence1.tracks[pTrack.Track];

                // Change the midichannel while playing
                int c = track.ProgramChange; // instrument
                int v = 0;

                ChannelMessageBuilder builder = new ChannelMessageBuilder() {
                    Command = ChannelCommand.ProgramChange,
                    MidiChannel = midichannel,
                    Data1 = c,
                    Data2 = v,
                };

                builder.Build();
                outDevice.Send(builder.Result);

                // Change channel in all messages
                track.ChangeChannel(track.MidiChannel, midichannel);
                track.MidiChannel = midichannel;

                FileModified();
            }
        }

        /// <summary>
        /// Controller : Volume
        /// </summary>
        /// <param name="nChannel"></param>
        /// <param name="data1"></param>
        /// <param name="data2"></param>
        private void SendCC(int nChannel, int data1, int data2)
        {
            ChannelMessageBuilder builder = new ChannelMessageBuilder() {
                Command = ChannelCommand.Controller,
                MidiChannel = nChannel,
                Data1 = (int)ControllerType.Volume,
                Data2 = data2,
            };

            builder.Build();
            outDevice.Send(builder.Result);
        }

        /// <summary>
        /// Set Midi Master Volume
        /// </summary>
        /// <param name="volume"></param>
        private void SetMidiMasterVolume(int volume)
        {
            // https://github.com/loveemu/loveemu-lab/blob/master/spctrans/softcspc/src/libsmfcx.c
            //Master Volume : {0xF0, 0x7F, 0x7F, 0x04, 0x01, 0x00, value, 0xF7}

            if (volume < 0)
                volume = 0;
            if (volume > 127)
                volume = 127;

            if ((volume >= 0) && (volume <= 127))
            {
                byte[] sysexMasterVolume = { 0xF0, 0x7F, 0x7F, 0x04, 0x01, 0x00, 0, 0xF7 };
                sysexMasterVolume[6] = (byte)volume;

                SysExMessage semsg = new SysExMessage(sysexMasterVolume);
                outDevice.Send(semsg);
            }
            
        }
                
        /// <summary>
        /// Main MIDI volume
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldMainVolume_ValueChanged(object sender, EventArgs e)
        {           
            SetMidiMasterVolume((int)sldMainVolume.Value);
            lblMainVolume.Text = String.Format("{0}%", 100*sldMainVolume.Value/sldMainVolume.Maximum);           
        }              

        private void RemoveFader()
        {
            foreach (Track trk in sequence1.tracks)
            {
                trk.RemoveFader();
            }
        }

        #region Maximize, Minimize Track Control
        /// <summary>
        /// Maximize or minimize track control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="bmaximized"></param>
        private void BtnMaximizedClickOneEvent(object sender, EventArgs e, bool bmaximized)
        {
            if (sender is TrkControl.TrackControl pTrack)
            {
                Track track = sequence1.tracks[pTrack.Track];
                int i = sequence1.tracks.IndexOf(track);
                lstTrkReglages[i].maximized = !lstTrkReglages[i].maximized;
                track.Maximized = lstTrkReglages[i].maximized;

                // Redraw all: tracks and SheetMusic according to height of tracks
                RedrawTrackControls2();

                // Refresh SheetMusic                
                RefreshDisplay();

               
            }
        }
        #endregion

        #region drag drop track control

        private void TrkControl_DragLeave(object sender, EventArgs e)
        {
            TrkControl.TrackControl pTrack = sender as TrkControl.TrackControl;
            if (pTrack != draggedTrack)
                pTrack.Selected = false;
        }

        private void TrkControl_DragOver(object sender, DragEventArgs e)
        {
            TrkControl.TrackControl pTrack = sender as TrkControl.TrackControl;
            pTrack.Selected = true;

            e.Effect = DragDropEffects.Move;
        }

        private void TrkControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TrkControl_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            // Here code to put the dragged track here in place of track
            TrkControl.TrackControl pTrack = sender as TrkControl.TrackControl;
            droppedTrack = pTrack;
            if (draggedTrack != null)
            {
                if (droppedTrack == draggedTrack)
                {
                    //MessageBox.Show("Source and target tracks cannot be identical");
                    //sheetmusic.Focus();
                    return;
                }
                string tx = string.Format("Replace track\n<{0} - {1}>\nby track\n<{2} - {3}>?", droppedTrack.TrackName, droppedTrack.TrackLabel, draggedTrack.TrackName, draggedTrack.TrackLabel);
                if (MessageBox.Show(tx, "Darg Drop", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    droppedTrack.Selected = false;
                    return;
                }

                // Switch tracks                
                Track tmp = sequence1.tracks[droppedTrack.Track];
                sequence1.tracks[droppedTrack.Track] = sequence1.tracks[draggedTrack.Track];
                sequence1.tracks[draggedTrack.Track] = tmp;

                draggedTrack.Selected = false;
                droppedTrack.Selected = false;

                // Create a new ShetMusic
                DisplayTrackControls();

                // Reset tracks stuff
                InitTracksStuff();

                RedrawSheetMusic();
                FileModified();

            }
        }

        private void TrkControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // unselect all track controls
                UnselectTrackControls();

                TrkControl.TrackControl pTrack = sender as TrkControl.TrackControl;
                draggedTrack = pTrack;

                pTrack.Selected = true;
                sheetmusic.SelectedStaff = pTrack.Track;

                DoDragDrop(pTrack, DragDropEffects.Move);
            }
        }

        #endregion

        #endregion track control


        #region song next prev load

        // Select and load next playlist item
        private void SelectNextPlaylistSong()
        {
            if (currentPlaylist == null)
            {
                // FAB 30/09/2018 !!!!
                // Ne s'arrête jamais
                PlayerState = PlayerStates.Stopped;
                return;
            }

            PlaylistItem pli = currentPlaylistItem;

            // Select next item            
            currentPlaylistItem = currentPlaylist.Next(pli);

            // Stop if no other song to play
            if (pli == currentPlaylistItem)
            {                
                PlayerState = PlayerStates.Stopped;
                BtnStatus();
                return;
            }
            
            // Load file
            MIDIfileName = currentPlaylistItem.Song;
            MIDIfileFullPath = currentPlaylistItem.File;

            // Select which type a file it is
            SelectFileToLoadAsync();            
        }

        /// <summary>
        /// Select action to perform betwwen 2 songs according to user's choices
        /// Pause, Count Down, play asap
        /// </summary>
        private void performPlaylistChainingChoice()
        {
            string _InternalSepLines = "¼";
            
            // If mode pause between songs of a playlist 
            // Display a waiting information (not the words)
            if (Karaclass.m_PauseBetweenSongs)
            {
                PlayerState = PlayerStates.LaunchNextSong;
                BtnStatus();

                #region display singer in the Lyrics form
                // Display the Lyric form even if no lyrics in order to display the singer
                if (Application.OpenForms.OfType<frmLyric>().Count() == 0)
                {
                    frmLyric = new frmLyric(myLyricsMgmt);
                    frmLyric.Show();
                }

                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    // During the waiting time, display informations about the next singer
                    int nbLines = 0;
                    string toptxt = string.Empty;
                    string centertxt = string.Empty;
                    string song = string.Empty;

                    if (currentPlaylistItem.KaraokeSinger == "" || currentPlaylistItem.KaraokeSinger == "<Song reserved by>")
                    {
                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        nbLines = 1;
                    }
                    else
                    {
                        
                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song) + " - Next singer: " + currentPlaylistItem.KaraokeSinger;
                        //centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song) + "|" + "-" + "|" + Strings.SungBy + "|" + currentPlaylistItem.KaraokeSinger;
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song) 
                            + _InternalSepLines + Strings.SungBy 
                            + _InternalSepLines + currentPlaylistItem.KaraokeSinger;
                        nbLines = 4;
                    }

                    // arriere plan provisoire
                    frmLyric.AlloModifyDirSlideShow = true;
                    frmLyric.DirSlideShow = Properties.Settings.Default.dirSlideShow;
                    frmLyric.AlloModifyDirSlideShow = false;

                    frmLyric.TxtNbLines = nbLines;
                    frmLyric.bTextBackGround = false;

                    // Display singer in top panel
                    frmLyric.DisplaySinger(toptxt);

                    // Display next singer in lyrics form
                    frmLyric.DisplayText(centertxt);
                }
                #endregion

                // Focus on paused windows
                this.Restore();
                this.Activate();
            }
            else
            {
                // NO PAUSE MODE
                if (Karaclass.m_CountdownSongs == 0)
                {
                    // NO Timer => play                    
                    PlayerState = PlayerStates.Stopped;                    
                    PlayPauseMusic();
                }
                else
                {
                    // No pause mode between songs of a playlist, but a timer 
                    // Lauch Countdown timer
                    StartCountDownTimer();
                }
            }
        }

        /// <summary>
        /// Start count down before playing
        /// </summary>
        private void StartCountDownTimer()
        {
            PlayerState = PlayerStates.Waiting;
            BtnStatus();

            w_tick = 0;
            int sec = Karaclass.m_CountdownSongs;  // wait for x seconds
            w_wait = sec + 4;

            if (Application.OpenForms.OfType<frmLyric>().Count() == 0)
            {
                frmLyric = new frmLyric(myLyricsMgmt);
                frmLyric.Show();
            }

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                // Display song & singer
                string nextsong = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                string txt = "Next song: " + nextsong + " - Next singer: " + currentPlaylistItem.KaraokeSinger;
                frmLyric.DisplaySinger(txt);

                frmLyric.LoadWaitSong(sec);
            }

            timer5.Interval = 1000;  // interval = 1 sec      
            timer5.Enabled = true;
        }
      

        /// <summary>
        /// Called by Button NEXT, play immediately the next song
        /// </summary>
        private void PlayNextSong()
        {
            PlaylistItem pli = currentPlaylistItem;
            if (pli == null)
                return;

            currentPlaylistItem = currentPlaylist.Next(pli);

            if (currentPlaylist == null || pli == currentPlaylistItem)
                return;
                                              
            //Next song of the playlist
            MIDIfileName = currentPlaylistItem.Song;
            UpdatePlayListsForm(currentPlaylistItem.Song);

            // Ferme le formulaire frmLyric
            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                frmLyric.Close();
            }

            PlayerState = PlayerStates.Playing;
            MIDIfileFullPath = currentPlaylistItem.File;
            ResetMidiFile();

            SelectFileToLoadAsync();                   
        }


        /// <summary>
        /// Called by button PREVIOUS, play immediately the previous song
        /// </summary>
        private void PlayPrevSong()
        {
            if (PlayerState == PlayerStates.Paused)
            {
                laststart = bouclestart;
                sequencer1.Position = laststart;
                return;
            }
            
            PlaylistItem pli = currentPlaylistItem;
            if (pli == null)
                return;

            currentPlaylistItem = currentPlaylist.Previous(pli);

            if (currentPlaylist == null || pli == currentPlaylistItem)
                return;
                      
            MIDIfileName = currentPlaylistItem.Song;
            MIDIfileFullPath = currentPlaylistItem.File;

            UpdatePlayListsForm(currentPlaylistItem.Song);
            PlayerState = PlayerStates.Playing;
            
            ResetMidiFile();
            
            SelectFileToLoadAsync();
                    
        }

        /// <summary>
        /// Update display of frmPlaylist
        /// </summary>
        /// <param name="song"></param>
        private void UpdatePlayListsForm(string song)
        {
            int idx = currentPlaylist.SelectedIndex(currentPlaylistItem) + 1;
            lblPlaylist.Text = "PLAYLIST: " + idx + "/" + currentPlaylist.Count;

            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                frmExplorer = GetForm<frmExplorer>();
                frmExplorer.DisplaySong(song);
            }
        }


        private void ResetMidiFile()
        {
            //plLyrics = new List<plLyric>();

            OpenMidiFileOptions.TextEncoding = Karaclass.m_textEncoding;            
            OpenMidiFileOptions.SplitHands = false;
        }


        #endregion next prev load


        #region new song

        /// <summary>
        /// Add notes to compose a time line
        /// </summary>
        /// <param name="track"></param>
        /// <param name="channel"></param>
        /// <param name="dur">type of note's duration:  0.5f, 0.25f</param>
        private void CreateTimeLineMelody(Track track, int channel, float dur)
        {
            int noteC = 60;

            int ticks = 0;           
            int duration = 0;
            int number = noteC;

            float time = 0;
 

            int totalduration = sequence1.GetLength();
            int division = sequence1.Division;

            if (totalduration == 1)
            {
                totalduration = division * 5;
            }

            // Number of notes to create
            int nbnotes = Convert.ToInt32((1/dur) * (1 + (totalduration / division)));
            int velocity = Karaclass.m_Velocity;
            
            for (int i = 0 ; i < nbnotes; i++)
            {
                time = i * dur;

                ticks = Convert.ToInt32(time * division);
                duration = Convert.ToInt32(dur * division);
                
                MidiNote note = new MidiNote(ticks, channel, number, duration, velocity, false);
                track.addNote(note);
            }

            // File was modified
            FileModified();

        }

        /// <summary>
        /// Create a demo melody
        /// </summary>
        /// <param name="track"></param>
        /// <param name="channel"></param>
        private void CreateNewMelody(Track track, int channel, int measures)
        {
            #region old code
            /*
            int noteC = 60;
            int ticks = 0;
            int number = noteC;
            int division = sequence1.Division;
            int duration = 1 * division;

            int nbblacks = measures * sequence1.Numerator * 4 / sequence1.Denominator;

            // Ajoute une note au ticks = 0           
            ticks = 0;
            int velocity = Karaclass.m_Velocity;

            MidiNote note = new MidiNote(ticks, channel, number, duration, velocity, false);
            

            // Ajoute une note au ticks -1
            ticks = (nbblacks - 1) * division;
            note = new MidiNote(ticks, channel, number, duration, velocity, false);
            track.addNote(note);

            track.Volume = 80;
            */
            #endregion old code

            SetTrackLength(track, measures);

        }

        /// <summary>
        /// Insert a "EndOfTrack" meta message one tick after the duration
        /// </summary>
        /// <param name="track"></param>
        /// <param name="channel"></param>
        /// <param name="measures"></param>
        private void SetTrackLength(Track track, int measures)
        {            
            if(measures < 1)
                return;
            
            int division = sequence1.Division;
            _measurelen = sequence1.Time.Measure;            

            // ticks + 1            
            int ticks = _measurelen * measures;

            /*
            var split = BitConverter.GetBytes(0);
            byte[] bytes = new byte[3];
            bytes[0] = split[2]; //11;
            bytes[1] = split[1]; //113;
            bytes[2] = split[0]; //176;

            MetaMessage mtMsg = new MetaMessage(MetaType.EndOfTrack, bytes);
            track.Insert(ticks, mtMsg);            
            //track.OffsetEndOfTrack(ticks);
            */

            track.EndOfTrackOffset = ticks - 1;
            


        }

        /// <summary>
        /// Insert a new track
        /// </summary>
        /// <param name="trackindex"></param>
        /// <param name="trackname"></param>
        /// <param name="instrumentname"></param>
        /// <param name="channel"></param>
        /// <param name="programchange"></param>
        /// <param name="volume"></param>
        /// <param name="tempo"></param>
        /// <param name="timesig"></param>
        /// <returns></returns>
        private Track InsertTrack(int trackindex, string trackname, string instrumentname, int channel, int programchange, int volume, int tempo, TimeSignature timesig, int clef)
        {
            Track track = CreateTrack(trackname, instrumentname, channel, programchange, volume, tempo, timesig, clef);
            SequenceInsertTrack(trackindex, track);
 
            return track;
        }

        /// <summary>
        /// Create a new track
        /// </summary>
        /// <param name="trackname"></param>
        /// <param name="instrumentname"></param>
        /// <param name="channel"></param>
        /// <param name="programchange"></param>
        /// <param name="volume"></param>
        /// <param name="tempo"></param>
        /// <param name="timesig"></param>
        /// <returns></returns>
        private Track CreateTrack(string trackname, string instrumentname, int channel, int programchange, int volume, int tempo, TimeSignature timesig, int clef)
        {
            // Add tack to sequence
            Track track = new Track() {
                MidiChannel = channel,
                Name = trackname,
                InstrumentName = instrumentname,
                ProgramChange = programchange,
                Volume = volume,
                Pan = 64,
                Reverb = 0,
            };

            if (clef == 0)
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.Treble;
            else if (clef == 1)
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.Bass;
            else
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.None;

            // Tempo : 
            //ex tempo = 750000;
            track.insertTempo(tempo, 0);
          
            // Keysignature
            track.insertKeysignature(timesig.Numerator, timesig.Denominator);
          
            // Timesignature
            track.Numerator = timesig.Numerator;
            track.Denominator = timesig.Denominator;
            track.insertTimesignature(timesig.Numerator, timesig.Denominator);            

            // Patch
            track.insertPatch(channel, programchange);

            // trackname      
            track.insertTrackname(trackname);

            // Volume
            track.insertVolume(channel, volume);

            // Lengh of measure
            track.MeasureLength = _measurelen;

            return track;
        }
     
        /// <summary>
        /// Insert a new track to a sequence
        /// </summary>
        /// <param name="trackindex"></param>
        /// <param name="track"></param>
        private void SequenceInsertTrack(int trackindex ,Track track)
        {
            //insert track
            if (trackindex < sequence1.tracks.Count)                        
                sequence1.Insert(trackindex, track);            
            else            
                sequence1.Add(track);            

            // File was modified
            FileModified();
        }

        /// <summary>
        /// Add a new track
        /// </summary>
        /// <param name="trackname"></param>
        /// <param name="instrumentname"></param>
        /// <param name="nChannel"></param>
        /// <param name="programchange"></param>
        /// <param name="volume"></param>
        private Track AddTrack(string trackname, string instrumentname, int channel, int programchange, int volume, int tempo, TimeSignature timesig, int clef)
        {            
            Track track = CreateTrack(trackname, instrumentname, channel, programchange, volume, tempo, timesig, clef);
            sequence1.Add(track);
            // File was modified
            FileModified();            
            return track;
        }

        /// <summary>
        /// Insert a track control
        /// </summary>
        /// <param name="track"></param>
        /// <param name="trackindex"></param>
        private void InsertTrackControl(Track track, int trackindex)
        {
            TrkControl.TrackControl pTrack = CreateTrackControl(track, trackindex);            
        }

        /// <summary>
        /// Create a track control
        /// </summary>
        /// <param name="track"></param>
        /// <param name="trackindex"></param>
        /// <returns></returns>
        private TrkControl.TrackControl CreateTrackControl(Track track, int trackindex)
        {
            // Add track control
            pTrack = new TrkControl.TrackControl
            {
                TrackName = track.Name == null ? "<NoName>" : track.Name,
                Patch = track.ProgramChange
            };

            // InstrumentName;
            if (track.ProgramChange >= 0 && track.ProgramChange < 128)
                pTrack.TrackLabel = lsInstruments[track.ProgramChange].Trim();
             
            pTrack.MidiChannel = track.MidiChannel;

            pTrack.Volume = track.Volume;
            pTrack.Pan = track.Pan;
            pTrack.Reverb = track.Reverb;

            pTrack.Muted = false;
            pTrack.Enabled = bEditScore;

            pTrack.Track = trackindex;

            pTrack.OntrkControlClick += new TrkControl.TrackControl.TrackControlClickEventHandler(TrackControl_Click);
            pTrack.OntrkControlbtnMaximizeClicked += new TrkControl.TrackControl.btnMaximizeClickedEventHandler(BtnMaximizedClickOneEvent);
            pTrack.OntrkControlbtnMutClicked += new TrkControl.TrackControl.btnMutClickedEventHandler(BtnMutClickOneEvent);
            pTrack.OntrkControlbtnSoloClicked += new TrkControl.TrackControl.btnSoloClickedEventHandler(BtnSoloClickOneEvent);
            pTrack.OntrkControlbtnDelClicked += new TrkControl.TrackControl.btnDelClickedEventHandler(BtnDelClickOneEvent);
            pTrack.OntrkControllblPatchChanged += new TrkControl.TrackControl.lblPatchChangedEventHandler(LstInstrumentClickOneEvent);
            pTrack.OntrkControllblChannelChanged += new TrkControl.TrackControl.lblChannelChangedEventHandler(LstChannelClickOneEvent);
            pTrack.OntrkControlbtnPianoRollClicked += new TrkControl.TrackControl.btnPianoRollClickedEventHandler(BtnPianoRollClickOneEvent);

            // Knob buttons (volume, pan, reverb)
            pTrack.OnknobControlknobVolumeValueChanged += new TrkControl.TrackControl.knobVolumeValueChangedEventHandler(TrackBarVolumeChanged);
            pTrack.OnknobControlknobPanValueChanged += new TrkControl.TrackControl.knobPanValueChangedEventHandler(TrackBarPanChanged);
            pTrack.OnknobControlknobReverbValueChanged += new TrkControl.TrackControl.knobReverbValueChangedEventHandler(TrackBarReverbChanged);

            pTrack.OntrkControllblTrackNameChanged += new TrkControl.TrackControl.lblTrackNameChangedEventHandler(Trk_TrackName);


            // Drag Drop
            pTrack.MouseDown += new MouseEventHandler(TrkControl_MouseDown);
            pTrack.DragDrop += new DragEventHandler(TrkControl_DragDrop);
            pTrack.DragEnter += new DragEventHandler(TrkControl_DragEnter);
            pTrack.DragLeave += new EventHandler(TrkControl_DragLeave);
            pTrack.DragOver += new DragEventHandler(TrkControl_DragOver);
            pTrack.AllowDrop = true;

            
            pTrack.Tag = track.MidiChannel.ToString();

            try
            {
                pTrack.Parent = pnlTracks;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return pTrack;
        }
    

        /// <summary>
        /// Add a new track control 
        /// </summary>
        /// <param name="track"></param>
        private void AddTrackControl(Track track, int trackindex)
        {
            
            int j = 0;
            int yOffset = 1;

            // Count existing tracks track controls
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    j++;
                }
            }

            try
            {
                TrkControl.TrackControl pTrack = CreateTrackControl(track, trackindex);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int yloc = yOffset + j * iStaffHeightMaximized;

            yloc = Convert.ToInt32(yloc*zoom);

            pTrack.Location = new Point(0, yloc);

            // Ordonnée originelle
            pTrack.yOrg = yloc;
            
        }

        /// <summary>
        /// Redraw track controls
        /// </summary>
        public void RedrawTrackControls()
        {            
            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    if (pnlTracks.Controls[i].Tag != null)
                    {
                        TrkControl.TrackControl trkC = ((TrkControl.TrackControl)pnlTracks.Controls[i]);
                        
                        int yloc = trkC.yOrg;
                        yloc = Convert.ToInt32(yloc*zoom);
                        trkC.Location = new Point(trkC.Location.X, yloc);
                    }
                }
            }                                   
        }
     
        private void RedrawTrackControls2()
        {
            int yloc = 0;
            int h = 0;

            for (int i = 0; i < pnlTracks.Controls.Count; i++)
            {
                if (pnlTracks.Controls[i].GetType() == typeof(TrkControl.TrackControl))
                {
                    if (pnlTracks.Controls[i].Tag != null)
                    {
                        TrkControl.TrackControl trkC = ((TrkControl.TrackControl)pnlTracks.Controls[i]);
                        

                        yloc = h;
                        yloc = Convert.ToInt32(yloc * zoom);
                        trkC.Location = new Point(trkC.Location.X, yloc);

                        h += trkC.Height;
                    }
                }
            }
        }


        /// <summary>
        /// Common for new track
        /// </summary>
        private void NewTrack(int measures)
        {
            #region trackname
            // Find unused name for track name
            int id = 1;
            string trackname = "Track1";
            bool bfound = false;
            bool bfinished = false;
            while (bfinished == false)
            {
                bfound = false;
                for (int i = 0; i < sequence1.tracks.Count; i++)
                {
                    Track track = sequence1.tracks[i];
                    if (track.Name == trackname)
                    {
                        bfound = true;
                        id++;
                        trackname = "Track" + id.ToString();
                        break;
                    }
                }
                if (bfound == false)
                    bfinished = true;
            }

            #endregion trackname

            // propose first patch
            int programchange = 0;

            #region channel
            // Find unused channel for track            
            int channel = 0;
            bfound = false;
            bfinished = false;
            while (bfinished == false)
            {
                bfound = false;
                for (int i = 0; i < sequence1.tracks.Count; i++)
                {
                    Track track = sequence1.tracks[i];
                    if (track.MidiChannel == channel)
                    {
                        bfound = true;
                        channel++;
                        break;
                    }
                }
                if (bfound == false)
                    bfinished = true;
            }
            #endregion channel

            #region trackindex
            decimal trkindex = sequence1.tracks.Count;

            #endregion trackindex

            int clef = 0;

            DialogResult dr = new DialogResult();
            Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog TrackDialog = new Sanford.Multimedia.Midi.Score.UI.frmNewTrackDialog(trackname, programchange, channel, trkindex, clef);
            dr = TrackDialog.ShowDialog();

            // TODO : if we are creating a new file, 
            if (dr == DialogResult.Cancel)            
                return;
            
            // Get infos from dialog
            clef = TrackDialog.cle;
            trackname = TrackDialog.TrackName;
            programchange = TrackDialog.ProgramChange;
            string instrumentname = TrackDialog.InstrumentName;
            channel = TrackDialog.MidiChannel;
            int tindex = Convert.ToInt32(TrackDialog.trackindex);  // index of new track


            int volume = 79;
            sequence1.Format = 1;

            // Pas possible pour midi format 0 : une seule piste
            if (sequence1.Format == 0)
            {
                string msg = "Sorry, i can't add a new track to a midi file format 0";
                string title = "Karaboss";
                MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (tindex == sequence1.tracks.Count)
                {
                    // Add track to sequence
                    Track track = AddTrack(trackname, instrumentname, channel, programchange, volume, sequence1.Tempo, sequence1.Time, clef);

                    // Add track control
                    int trackindex = sequence1.tracks.Count - 1;
                    AddTrackControl(track, trackindex);

                    // Add a little melody
                    if (sequence1.tracks.Count == 1)
                        CreateNewMelody(track, channel, measures);
                }
                else
                {
                    // Insert track at position trkindex
                    Track track = InsertTrack(tindex, trackname, instrumentname, channel, programchange, volume, sequence1.Tempo, sequence1.Time, clef);

                    // Insert track control
                    InsertTrackControl(track, tindex);

                    // Add a little melody
                    if (sequence1.tracks.Count == 1)
                        CreateNewMelody(track, channel, measures);
                }
                


                DisplayTrackControls();

                // Reset tracks stuff
                InitTracksStuff();

                // Create a new ShetMusic
                RedrawSheetMusic();
                
                SetScrollBarValues();

                FileModified();

                // Set Current Note on new track
                int numstrack = sequence1.tracks.Count - 1;
                sheetmusic.UpdateCurrentNote(numstrack, 60, 0, true);
            }            
        }

        /// <summary>
        /// Create a new midi file from the explorer or from the player
        /// </summary>
        private void NewMidiFile()
        {
            // Show sequencer even if bSequencerAlwaysOn is set to False
            bForceShowSequencer = true;

            // Initialize tags
            MidiTags.ResetTags();

            // Create new sequence
            sequence1 = new Sequence(CreateNewMidiFile.Division)
            {
                Format = 1,
                OrigFormat= 1,
                Numerator = CreateNewMidiFile.Numerator,
                Denominator = CreateNewMidiFile.Denominator,
                Tempo = CreateNewMidiFile.Tempo,
                Time = new TimeSignature(CreateNewMidiFile.Numerator, CreateNewMidiFile.Denominator, CreateNewMidiFile.Division, CreateNewMidiFile.Tempo),
            };

            
            sequence1.CloneTags();

            pulsesPerMsec = sequence1.Division * (1000.0 / sequence1.Tempo);

            DrawControls();

            #region add track
           
            // Add track to sequence
            int volume = 79;
            sequence1.Format = 1;
            Track track = AddTrack(CreateNewMidiFile.trackname, CreateNewMidiFile.instrumentname, CreateNewMidiFile.channel, CreateNewMidiFile.programchange, volume, sequence1.Tempo, sequence1.Time, CreateNewMidiFile.clef);

            // Add track control
            int trackindex = sequence1.tracks.Count - 1;
            AddTrackControl(track, trackindex);

            // Add a little melody
            if (sequence1.tracks.Count == 1)
                CreateNewMelody(track, CreateNewMidiFile.channel, CreateNewMidiFile.Measures);

            DisplayTrackControls();

            // Reset tracks stuff
            InitTracksStuff();

            // Create a new ShetMusic
            RedrawSheetMusic();

            SetScrollBarValues();

            FileModified();

            // Set Current Note on new track
            int numstrack = sequence1.tracks.Count - 1;
            sheetmusic.UpdateCurrentNote(numstrack, 60, 0, true);


            #endregion addtrack


            UpdateMidiTimes();
            DisplaySongDuration();

            positionHScrollBarNew.Value = 0;
            positionHScrollBarNew.Maximum = _totalTicks;

            sequencer1.Sequence = sequence1;

            MIDIfileName = "New";
            MIDIfilePath = CreateNewMidiFile.DefaultDirectory; ;
            MIDIfileFullPath = null;

            // FAB
            SetTitle("New.mid");

            myLyricsMgmt = new LyricsMgmt(sequence1, bShowChords);
            bHasLyrics = myLyricsMgmt.OrgplLyrics.Count > 0; // myLyricsMgmt.LyricType != LyricTypes.None;            

            // Display midi file infos
            DisplayFileInfos();
            DisplayLyricsInfos();

            // Display log file
            if (sequence1.Log != "")
            {
                lblChangesInfos.Text = sequence1.Log;
            }

            PlayerState = PlayerStates.Stopped;

        }



        #endregion new song


        #region Tempo
        /// <summary>
        /// Open window of tempo management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="tmps"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void Tempo_DoubleClick(object sender, EventArgs e, TempoSymbol tmps)
        {
            if (Application.OpenForms["frmModifyTempo"] != null)
                Application.OpenForms["frmModifyTempo"].Close();


            if (Application.OpenForms["frmModifyTempo"] == null)
            {
                frmModifyTempo = new frmModifyTempo(sheetmusic, sequence1);                
                frmModifyTempo.Show();
                frmModifyTempo.Refresh();

            }            
        }

        #endregion Tempo


        #region edit partition

        private void KeyboardSelectDurations(KeyEventArgs e)
        {
            if (PlayerState != PlayerStates.Stopped)
                return;

            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    switch (NoteValue)
                    {
                        case NoteValues.QuadrupleCroche:
                            SetTripleCroche();
                            break;
                        case NoteValues.TripleCroche:
                            SetDoubleCroche();
                            break;
                        case NoteValues.DoubleCroche:
                            SetCroche();
                            break;
                        case NoteValues.Croche:
                            SetBlack();
                            break;
                        case NoteValues.Noire:
                            SetWhite();
                            break;
                        case NoteValues.Blanche:
                            SetRond();
                            break;
                        case NoteValues.Ronde:
                            SetQuadrupleCroche();
                            break;
                    }
                    break;

                case Keys.D6:
                case Keys.OemMinus:
                case Keys.Subtract:
                    switch (NoteValue)
                    {
                        case NoteValues.Ronde:
                            SetWhite();
                            break;
                        case NoteValues.Blanche:
                            SetBlack();
                            break;
                        case NoteValues.Noire:
                            SetCroche();
                            break;
                        case NoteValues.Croche:
                            SetDoubleCroche();
                            break;
                        case NoteValues.DoubleCroche:
                            SetTripleCroche();
                            break;
                        case NoteValues.TripleCroche:
                            SetQuadrupleCroche();
                            break;
                        case NoteValues.QuadrupleCroche:
                            SetRond();
                            break;
                    }
                    break;

                case Keys.OemPeriod:
                case Keys.Decimal:
                    SetDotted();
                    break;
            }        
        }

        /// <summary>
        /// Disable editing functions
        /// </summary>
        private void DisableEditButtons()
        {
            NoteValue = NoteValues.None;
            this.Cursor = Cursors.Default;

            lblSaisieNotes.BackColor = Color.White;
            bEnterNotes = false;
            Alteration = Alterations.None;

            lblGomme.BackColor = Color.White;
            lblRondNote.BackColor = Color.White;
            lblWhiteNote.BackColor = Color.White;
            lblBlackNote.BackColor = Color.White;
            lblCrocheNote.BackColor = Color.White;
            lblDoubleCrocheNote.BackColor = Color.White;
            lblTripleCrocheNote.BackColor = Color.White;
            lblQuadrupleCrocheNote.BackColor = Color.White;

            lblDotted.BackColor = Color.White;
            lblBemol.BackColor = Color.White;
            lblDiese.BackColor = Color.White;
            lblBecarre.BackColor = Color.White;           

        }
       
        /// <summary>
        /// Click on button "Edit" => Score edition activation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblEdit_Click(object sender, EventArgs e)
        {
            if (PlayerState != PlayerStates.Stopped)
                return;

            DspEdit(!bEditScore);           

        }
 
        /// <summary>
        /// Validate or invalidate EditMode
        /// </summary>
        /// <param name="status"></param>
        private void DspEdit(bool status)
        {
            if (sheetmusic == null)
                return;

            if (status == true)
            {
                // Enter edit mode
                bEditScore = true;

                lblEdit.BackColor = Color.Red;
                lblEdit.ForeColor = Color.White;
                MnuEditScore.Checked = true;

                // Allow sheetmusic editing
                sheetmusic.bEditMode = true;
                EnableTrackControls(true);

                if (sheetmusic.SelectedStaff != -1)
                {
                    SelectTrackControl(sheetmusic.SelectedStaff);                   
                }
                else
                {
                    SelectTrackControl(0);
                    sheetmusic.SelectedStaff = 0;
                }                
            }
            else
            {
                // Quit edit mode
                bEditScore = false;

                lblEdit.BackColor = Color.White;
                lblEdit.ForeColor = Color.Black;

                MnuEditScore.Checked = false;
                MnuEditEnterNotes.Checked = false;
                
                // Quit enter notes
                DisableEditButtons();
                bEnterNotes = false;

                // Disallow sheetmusic edit mode
                sheetmusic.bEditMode = false;
                sheetmusic.bEnterNotes = false;

                // Disable edit Track Controls
                EnableTrackControls(false);

                // Unselect all track controls
                UnselectTrackControls();

                // Close frmModifyTempo
                if (Application.OpenForms.OfType<frmModifyTempo>().Count() > 0)
                {
                    Application.OpenForms["frmModifyTempo"].Close();
                }

            }
        }

        /// <summary>
        /// Click on button "N" => Notes entering activation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblSaisieNotes_Click(object sender, EventArgs e)
        {
            DspEnterNotes();          
        }

        /// <summary>
        /// Validate or invalidate Entering notes
        /// </summary>
        private void DspEnterNotes()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;

            // stop notes entering 
            if (bEnterNotes == true)
            {
                DisableEditButtons();
                bEnterNotes = false;

                MnuEditEnterNotes.Checked = false;
                
                sheetmusic.bEnterNotes = false;
                sheetmusic.bShowHelpGrid = false;
            }
            else
            {
                // start notes entering 
                DisableEditButtons();
                bEnterNotes = true;
                

                MnuEditEnterNotes.Checked = true;
                MnuEditScore.Checked = true;

                sheetmusic.bEnterNotes = true;
                sheetmusic.bShowHelpGrid = true;

                // Enter edit mode
                DspEdit(true);
                
                lblSaisieNotes.BackColor = Color.Red;
                
                NoteValue = NoteValues.Noire;
                this.Cursor = Cursors.Hand;
                lblBlackNote.BackColor = Color.Red;               
            }


            // Show form edit note only if label "Edit" is red
            //if (bEditScore && !bEnterNotes)
            //    ShowFrmNoteEdit();


        }
    
        private void RestoreSaisie()
       {
           if (bEnterNotes == false)
            {
                // start notes entering
                DisableEditButtons();
                bEnterNotes = true;
                sheetmusic.bEnterNotes = true;
                lblSaisieNotes.BackColor = Color.Red;

                // Enter edit mode
                DspEdit(true);
            }
       }

        /// <summary>
        /// Edit: Erase a note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblGomme_Click(object sender, EventArgs e)
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.Gomme)
            {
                NoteValue = NoteValues.None;
                lblGomme.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            { 
                NoteValue = NoteValues.Gomme;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.Red;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;

            }
        }

        #region Notes Ronde to quadruplecroche

        /// <summary>
        /// Edi: Add a Rond note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblRondNote_Click(object sender, EventArgs e)
        {
            SetRond();
        }

        private void SetRond()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.Ronde)
            {
                NoteValue = NoteValues.None;
                lblRondNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.Ronde;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.Red;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }
        }

        /// <summary>
        /// Edit: Add a white note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblWhiteNote_Click(object sender, EventArgs e)
            {
                SetWhite();
            }

        private void SetWhite()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.Blanche)
            {
                NoteValue = NoteValues.None;
                lblWhiteNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.Blanche;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.Red;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }
        }

        private void LblBlackNote_Click(object sender, EventArgs e)
        {
            SetBlack();
        }

        private void SetBlack()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.Noire)
            {
                NoteValue = NoteValues.None;
                lblBlackNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.Noire;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.Red;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }

        }

        private void LblCrocheNote_Click(object sender, EventArgs e)
        {
            SetCroche();
        }

        private void SetCroche()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.Croche)
            {
                NoteValue = NoteValues.None;
                lblCrocheNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.Croche;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.Red;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }
        }

        private void LblDoubleCrocheNote_Click(object sender, EventArgs e)
        {
            SetDoubleCroche();
        }

        private void SetDoubleCroche()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.DoubleCroche)
            {
                NoteValue = NoteValues.None;
                lblDoubleCrocheNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.DoubleCroche;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.Red;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }
        }

        private void LblTripleCrocheNote_Click(object sender, EventArgs e)
        {
            SetTripleCroche();
        }

        private void SetTripleCroche()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.TripleCroche)
            {
                NoteValue = NoteValues.None;
                lblTripleCrocheNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.TripleCroche;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.Red;
                lblQuadrupleCrocheNote.BackColor = Color.White;
            }
        }

        private void LblQuadrupleCrocheNote_Click(object sender, EventArgs e)
        {
            SetQuadrupleCroche();
        }

        private void SetQuadrupleCroche()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            else
                RestoreSaisie();

            if (NoteValue == NoteValues.QuadrupleCroche)
            {
                NoteValue = NoteValues.None;
                lblQuadrupleCrocheNote.BackColor = Color.White;
                this.Cursor = Cursors.Arrow;
            }
            else
            {
                NoteValue = NoteValues.QuadrupleCroche;
                this.Cursor = Cursors.Hand;

                lblGomme.BackColor = Color.White;
                lblRondNote.BackColor = Color.White;
                lblWhiteNote.BackColor = Color.White;
                lblBlackNote.BackColor = Color.White;
                lblCrocheNote.BackColor = Color.White;
                lblDoubleCrocheNote.BackColor = Color.White;
                lblTripleCrocheNote.BackColor = Color.White;
                lblQuadrupleCrocheNote.BackColor = Color.Red;
            }
        }

        #endregion

        #region alterations
        private void LblDotted_Click(object sender, EventArgs e)
        {
            SetDotted();
        }

        private void SetDotted()
        {
            if (bEnterNotes == false)
                return;

            if (Alteration == Alterations.Dot)
            {
                Alteration = Alterations.None;
                lblDotted.BackColor = Color.White;
            }
            else
            {
                Alteration = Alterations.Dot;
                lblDotted.BackColor = Color.Red;
            }
        }

        private void LblDiese_Click(object sender, EventArgs e)
        {
            if (bEnterNotes == false)
                return;
            
            if (Alteration == Alterations.Diese)
            {
                Alteration = Alterations.None;
                lblDiese.BackColor = Color.White;
            }
            else
            {
                Alteration = Alterations.Diese;
                lblDiese.BackColor = Color.Red;
                lblBemol.BackColor = Color.White;
                lblBecarre.BackColor = Color.White;
            }
        }

        private void LblBemol_Click(object sender, EventArgs e)
        {
            if (bEnterNotes == false)
                return;
            
            if (Alteration == Alterations.Bemol)
            {
                Alteration = Alterations.None;
                lblBemol.BackColor = Color.White;
            }
            else
            {
                Alteration = Alterations.Bemol;
                lblDiese.BackColor = Color.White;
                lblBemol.BackColor = Color.Red;
                lblBecarre.BackColor = Color.White;
            }

        }
      
        private void LblBecarre_Click(object sender, EventArgs e)
        {
            if (bEnterNotes == false)
                return;
            
            if (Alteration == Alterations.Becarre)
            {
                Alteration = Alterations.None;
                lblBecarre.BackColor = Color.White;
            }
            else
            {
                Alteration = Alterations.Becarre;
                lblDiese.BackColor = Color.White;
                lblBemol.BackColor = Color.White;
                lblBecarre.BackColor = Color.Red;
            }

        }
        #endregion

        #region triolet
        /// <summary>
        /// Transform selection of notes to a triolet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LblTriolet_Click(object sender, EventArgs e)
        {
            #region guard
            if (bEditScore == false || sheetmusic == null)
                return;
            
            #endregion
            //  
            
        }

        #endregion

        #region clef

        // Set Clef for Measure
        private void LblTreble_Click(object sender, EventArgs e)
        {
            if (bEditScore == false)
                return;

            int numTrack = sheetmusic.CurrentNote.numstaff;
            Track track = sequence1.tracks[numTrack];

            if (lblTreble.BackColor == Color.Red)
            {
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.None;
                lblTreble.BackColor = Color.White;
            }
            else
            {
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.Treble;
                lblBass.BackColor = Color.White;
                lblTreble.BackColor = Color.Red;
            }
            sheetmusic.Refresh();
        }

        private void LblBass_Click(object sender, EventArgs e)
        {
            if (bEditScore == false)
                return;

            int numTrack = sheetmusic.CurrentNote.numstaff;
            Track track = sequence1.tracks[numTrack];

            if (lblBass.BackColor == Color.Red)
            {
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.None;
                lblBass.BackColor = Color.White;
            }
            else
            {
                track.Clef = Sanford.Multimedia.Midi.Score.Clef.Bass;
                lblTreble.BackColor = Color.White;
                lblBass.BackColor = Color.Red;
            }
            sheetmusic.Refresh();
        }
        
        /// <summary>
        /// Color in red button corresponding to the key of the track
        /// </summary>
        /// <param name="tracknum"></param>
        private void SelectTrackKey(int tracknum)
        {
            Track track = sequence1.tracks[tracknum];

            // Select key
            if (track.Clef == Sanford.Multimedia.Midi.Score.Clef.Bass)
            {
                lblBass.BackColor = Color.Red;
                lblTreble.BackColor = Color.White;
            }
            else if (track.Clef == Sanford.Multimedia.Midi.Score.Clef.Treble)
            {
                lblTreble.BackColor = Color.Red;
                lblBass.BackColor = Color.White;
            }
            else
            {
                lblTreble.BackColor = Color.White;
                lblBass.BackColor = Color.White;
            }
        }

        #endregion

        #endregion edit partition


        #region recording

        private void BtnStartRec_Click(object sender, EventArgs e)
        {          
            LaunchRecorder();
        }

        /// <summary>
        /// Launch the MP3 recorder
        /// </summary>
        private void LaunchRecorder()
        {
            string arguments = string.Empty;
            IntPtr Hwnd = IntPtr.Zero;

            string exeName = "KarabossRecorder.exe";
            string exeNameConfig = "KarabossRecorder.exe.config";

            string path = Path.GetDirectoryName(Application.ExecutablePath);

            string fullPath = Path.Combine(path, exeName);       
            if (!File.Exists(fullPath))
            {                
                string errorText = string.Format("File not found: {0}", exeName);
                MessageBox.Show(errorText, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            fullPath = Path.Combine(path, exeNameConfig);
            if (!File.Exists(fullPath))
            {
                string errorText = string.Format("File not found: {0}", exeNameConfig);
                MessageBox.Show(errorText, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {

                ProcessStartInfo startRecorder = new ProcessStartInfo()
                {
                    Arguments = arguments,
                    FileName = exeName,
                };

                Process.Start(startRecorder);
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion recording


        #region TimeLine

        // red bar
        private void SetTimeVLinePos(int pos)
        {
            TimeVLine.Height = pnlScrollView.Height;
            TimeVLine.Location = new Point(leftWidth + pos, pnlScrollView.Top);
            
        }

        // blue bar
        private void SetStartVLinePos(int pos)
        {
            TimeStartVLine.Height = pnlScrollView.Height;
            TimeStartVLine.Location = new Point(leftWidth + pos, pnlScrollView.Top);
        }

        #endregion TimeLine


        #region peak level master volume
                        
        /// <summary>
        /// Get master peak volume from provider of sound (Karaboss itself or an external one such as VirtualMidiSynth)
        /// </summary>
        private void GetPeakVolume()
        {            
            float? peak = AudioControl.AudioManager.GetApplicationMasterPeakVolume(outDeviceProcessId);
            VuMasterPeakVolume.Level = Convert.ToInt32(peak);
        }

        /// <summary>
        /// Initialize control peak volume level
        /// </summary>
        private void Init_peakLevel()
        {
            this.VuMasterPeakVolume.AnalogMeter = false;
            this.VuMasterPeakVolume.BackColor = System.Drawing.Color.DimGray;
            this.VuMasterPeakVolume.DialBackground = System.Drawing.Color.White;
            this.VuMasterPeakVolume.DialTextNegative = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.DialTextPositive = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.DialTextZero = System.Drawing.Color.DarkGreen;

            // LED 1
            this.VuMasterPeakVolume.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuMasterPeakVolume.Led1ColorOn = System.Drawing.Color.LimeGreen;
            //this.VuMasterPeakVolume.Led1Count = 12;
            this.VuMasterPeakVolume.Led1Count = 14;

            // LED 2
            this.VuMasterPeakVolume.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuMasterPeakVolume.Led2ColorOn = System.Drawing.Color.Yellow;
            //this.VuMasterPeakVolume.Led2Count = 12;
            this.VuMasterPeakVolume.Led2Count = 14;

            // LED 3
            this.VuMasterPeakVolume.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuMasterPeakVolume.Led3ColorOn = System.Drawing.Color.Red;
            //this.VuMasterPeakVolume.Led3Count = 8;
            this.VuMasterPeakVolume.Led3Count = 10;

            // LED size
            this.VuMasterPeakVolume.LedSize = new System.Drawing.Size(12, 2);            

            this.VuMasterPeakVolume.LedSpace = 1;
            this.VuMasterPeakVolume.Level = 0;
            this.VuMasterPeakVolume.LevelMax = 127;

            //this.VuMasterPeakVolume.Location = new System.Drawing.Point(220, 33);
            this.VuMasterPeakVolume.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuMasterPeakVolume.Name = "VuMasterPeakVolume";
            this.VuMasterPeakVolume.NeedleColor = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.PeakHold = false;
            this.VuMasterPeakVolume.Peakms = 1000;
            this.VuMasterPeakVolume.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.ShowDialOnly = false;
            this.VuMasterPeakVolume.ShowLedPeak = false;
            this.VuMasterPeakVolume.ShowTextInDial = false;
            this.VuMasterPeakVolume.Size = new System.Drawing.Size(14, 120);
            this.VuMasterPeakVolume.TabIndex = 5;
            this.VuMasterPeakVolume.TextInDial = new string[] {
            "-40",
            "-20",
            "-10",
            "-5",
            "0",
            "+6"};
            this.VuMasterPeakVolume.UseLedLight = false;
            this.VuMasterPeakVolume.VerticalBar = true;
            this.VuMasterPeakVolume.VuText = "VU";            
            this.VuMasterPeakVolume.Location = new Point(226, 7);

        }       


        #endregion


        #region Tempo, Transpo

        /// <summary>
        /// Speed up tempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoPlus_Click(object sender, EventArgs e)
        {
            if (TempoDelta > 10)
                TempoDelta -= 10;
            ModTempo();
        }

        /// <summary>
        ///  Slow down tempo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoMinus_Click(object sender, EventArgs e)
        {
            if (TempoDelta < 200)
                TempoDelta += 10;
            ModTempo();
        }


      

        /// <summary>
        /// Transpose higher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranspoPlus_Click(object sender, EventArgs e)
        {
            int amount = Properties.Settings.Default.TransposeAmount;

            TransposeDelta += amount;
            ModTranspose(amount);
        }

        /// <summary>
        /// Transpose down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranspoMinus_Click(object sender, EventArgs e)
        {
            int amount = Properties.Settings.Default.TransposeAmount;
            TransposeDelta -= amount;
            ModTranspose(-amount);
        }      

        private void ModTempo()
        {
            _tempo = TempoDelta * TempoOrig / 100;

            // Change clock tempo
            sequencer1.Tempo = _tempo;
            

            lblTempoValue.Text = string.Format("{0}%", TempoDelta);

            // Update Midi Times            
            _bpm = GetBPM(_tempo);            

            // Update display duration
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

            DisplayFileInfos();            

        }

        private void ModTranspose(int amount)
        {
            btnTempoMinus.Enabled = false;
            btnTranspoPlus.Enabled = false;

            int tp = TransposeOrig + TransposeDelta;

            lblTranspoValue.Text = string.Format("{0}", TransposeDelta);

            if (PlayerState == PlayerStates.Playing)
                sequencer1.Stop();

            sequence1.Transpose(amount);

            if (PlayerState == PlayerStates.Playing)
                sequencer1.Continue();

            // FAB : 16/09/2018 fixed redraw of scores
            if (bSequencerAlwaysOn | bForceShowSequencer)
            {
                RedrawSheetMusic();                
            }

            btnTempoMinus.Enabled = true;
            btnTranspoPlus.Enabled = true;

        }

        private void KeyboardSelectTempo(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    if (TempoDelta > 10)
                        TempoDelta -= 10;
                    ModTempo();
                    break;

                case Keys.D6:
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (TempoDelta < 200)
                        TempoDelta += 10;
                    ModTempo();
                    break;
            }
        }

        #endregion


        #region settings

        private void AlertOutputDevice(string Name)
        {
            if (Name == "Microsoft GS Wavetable Synth")
                lblOutputDevice.ForeColor = Color.Red;
            else
                lblOutputDevice.ForeColor = Color.PaleGreen;
        }

        private void ResetPlaySettings()
        {
            // Reset settings made for previous song
            sldMainVolume.Value = 104;
            TempoDelta = 100;
            lblTempoValue.Text = string.Format("{0}%", TempoDelta);
            TransposeDelta = 0;
            lblTranspoValue.Text = string.Format("{0}", TransposeDelta);


            // Mute melody track
            if (Karaclass.m_MuteMelody == true || (currentPlaylist != null && currentPlaylistItem.MelodyMute == true))
            {                
                btnMute1.Checked = false;
            }
            else
            {                
                btnMute1.Checked = true;
            }
            
        }




        #endregion


        #region chords analysis
        private void btnChords_Click(object sender, EventArgs e)
        {

            // Ferme le formulaire frmChords
            if (Application.OpenForms["frmChords"] != null)
                Application.OpenForms["frmChords"].Close();

            if (Application.OpenForms.OfType<frmChords>().Count() == 0)
            {
                try
                {
                    frmChords frmChords = new frmChords(outDevice, MIDIfileFullPath);
                    frmChords.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        #endregion
    }

}