using ChordAnalyser.UI;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Karaboss
{
    public partial class frmChords : Form
    {

        #region private
        private bool closing = false;
        private bool scrolling = false;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        private int bouclestart = 0;
        private int newstart = 0;
        private int laststart = 0;      // Start time to play
        private int nbstop = 0;


        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,
            NextSong,
            Waiting,
            WaitingPaused
        }
        private PlayerStates PlayerState;

        #region controls
        private Sequence sequence1 = new Sequence();
        private ChordAnalyser.UI.ChordsControl chordAnalyserControl1;
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();

        private System.Windows.Forms.Timer timer1;
        private Karaboss.NoSelectButton btnPlay;
        private Karaboss.NoSelectButton btnRewind;

        private Label lblNumMeasure;
        private Label lblElapsed;
        private Label lblPercent;
        private Label lblBeat;

        #endregion controls

        private Panel pnlTop;
        private Panel pnlDisplay;
        private Panel pnlBottom;

        private ColorSlider.ColorSlider positionHScrollBar;

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

        #endregion private


        public frmChords(OutputDevice OtpDev, string FileName)
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);

            // Sequence
            //LoadSequencer(seq);
            outDevice = OtpDev;

            // Title
            SetTitle(FileName);

            UpdateMidiTimes();
            
        }

        #region Display Controls

        /// <summary>
        /// Sets title of form
        /// </summary>
        /// <param name="fileName"></param>
        private void SetTitle(string fileName)
        {
            Text = "Karaboss - " + Path.GetFileName(fileName);
        }

        private void LoadSequencer(Sequence seq)
        {
            try
            {
                sequence1 = seq;

                sequencer1 = new Sequencer();
                sequencer1.Sequence = sequence1;    // primordial !!!!!
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                UpdateMidiTimes();

                //TempoOrig = _tempo;
                //lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

                // Display
                int Min = (int)(_duration / 60);
                int Sec = (int)(_duration - (Min * 60));
                //lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

                // PlayerState = stopped
                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawControls()
        {
            // Timer
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Tick += new EventHandler(timer1_Tick);



            #region Panel Top
            pnlTop = new Panel();
            pnlTop.Parent = this.tabPageDiagrams;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Size = new Size(tabPageDiagrams.Width, 54);
            pnlTop.BackColor = Color.FromArgb(70, 77, 95);
            pnlTop.Dock = DockStyle.Top;
            tabPageDiagrams.Controls.Add(pnlTop);

            // Button Rewind
            btnRewind = new NoSelectButton();            
            btnRewind.FlatAppearance.BorderSize = 0;
            btnRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatStyle = FlatStyle.Flat;   
            btnRewind.Parent = pnlTop;
            btnRewind.Location = new Point(2, 2);
            btnRewind.Size = new Size(50, 50);
            btnRewind.Image = Properties.Resources.btn_black_prev;
            btnRewind.Click += new EventHandler(btnRewind_Click);
            btnRewind.MouseHover += new EventHandler(btnRewind_MouseHover);
            btnRewind.MouseLeave += new EventHandler(btnRewind_MouseLeave);
            pnlTop.Controls.Add(btnRewind);

            // Button play
            btnPlay = new NoSelectButton();
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnPlay.Parent = pnlTop;
            btnPlay.Location = new Point(2 + btnRewind.Width, 2);
            btnPlay.Size = new Size(50, 50);
            btnPlay.Image = Properties.Resources.btn_black_play;
            btnPlay.Click += new EventHandler(btnPlay_Click);
            btnPlay.MouseHover += new EventHandler(btnPlay_MouseHover);
            btnPlay.MouseLeave += new EventHandler(btnPlay_MouseLeave);
            pnlTop.Controls.Add(btnPlay);    

            #endregion

            #region Panel Bottom
            pnlBottom = new Panel();
            pnlBottom.Parent = this.tabPageDiagrams;
            pnlBottom.Location = new Point(0, 0);
            pnlBottom.Size = new Size(tabPageDiagrams.Width, 20);
            pnlBottom.BackColor = Color.Red;
            pnlBottom.Dock = DockStyle.Bottom;
            tabPageDiagrams.Controls.Add(pnlBottom);
            
            lblNumMeasure = new Label();
            lblNumMeasure.Location = new Point(1, 1);
            lblNumMeasure.Text = "measure";
            lblNumMeasure.Parent = pnlBottom;
            pnlBottom.Controls.Add(lblNumMeasure);

            lblElapsed = new Label();
            lblElapsed.Location = new Point(100, 1);
            lblElapsed.Text = "00:00";
            lblElapsed.Parent = pnlBottom;
            pnlBottom.Controls.Add(lblElapsed);

            lblPercent = new Label();
            lblPercent.Location = new Point(200, 1);
            lblPercent.Text = "0%";
            lblPercent.Parent = pnlBottom;
            pnlBottom.Controls.Add(lblPercent);

            lblBeat = new Label();
            lblBeat.Location = new Point(300, 1);
            lblBeat.Text = "1";
            lblBeat.Parent = pnlBottom;
            pnlBottom.Controls.Add(lblBeat);

            #endregion

            #region Panel Display
            pnlDisplay = new Panel();
            pnlDisplay.Parent = tabPageDiagrams;
            pnlDisplay.Location = new Point(tabPageDiagrams.Margin.Left, pnlTop.Height);
            pnlDisplay.Size = new Size(pnlTop.Width, tabPageDiagrams.Height - pnlTop.Height - pnlBottom.Height);
            pnlDisplay.BackColor = Color.FromArgb(70, 77, 95);            
            tabPageDiagrams.Controls.Add(pnlDisplay);
            #endregion


            // MIDDLE

            #region ChordControl
            chordAnalyserControl1 = new ChordsControl();
            chordAnalyserControl1.Parent = pnlDisplay;
            chordAnalyserControl1.Sequence1 = this.sequence1;
            chordAnalyserControl1.Size = new Size(pnlDisplay.Width, 80);
            chordAnalyserControl1.Location = new Point(0, 0);
            chordAnalyserControl1.WidthChanged += new WidthChangedEventHandler(chordAnalyserControl1_WidthChanged);
            pnlDisplay.Controls.Add(chordAnalyserControl1);
            #endregion


            #region positionHScrollBar
            positionHScrollBar = new ColorSlider.ColorSlider();
            positionHScrollBar.Parent = pnlDisplay;
            positionHScrollBar.ThumbImage =  Properties.Resources.BTN_Thumb_Blue;            
            positionHScrollBar.Size = new Size(pnlDisplay.Width, 20);
            positionHScrollBar.Location = new Point(0, chordAnalyserControl1.Height);
            positionHScrollBar.Value = 0;
            positionHScrollBar.Minimum = 0;

            // Set maximum & visibility
            SetScrollBarValues();

            positionHScrollBar.TickStyle = TickStyle.None;
            positionHScrollBar.SmallChange = 1;
            positionHScrollBar.LargeChange = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.ShowDivisionsText = false;
            positionHScrollBar.ShowSmallScale = false;
            positionHScrollBar.MouseWheelBarPartitions = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(PositionHScrollBar_Scroll);
            positionHScrollBar.ValueChanged += new EventHandler(PositionHScollBar_ValueChanged);
            pnlDisplay.Controls.Add(positionHScrollBar);
            
            #endregion
        }


        #endregion Display Controls


        #region timer

 

        /// <summary>
        /// Timer tick management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!scrolling)
            {
                // Display time elapse               
                DisplayTimeElapse(sequencer1.Position);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        DisplayPositionHScrollBar(sequencer1.Position);
                        break;

                    case PlayerStates.Stopped:
                        timer1.Stop();
                        AfterStopped();
                        break;

                    case PlayerStates.Paused:
                        sequencer1.Stop();
                        timer1.Stop();
                        break;
                }

               
            }
        }


        #endregion timer


        #region Scroll ChordsControl 

        /// <summary>
        /// Display 
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayPositionHScrollBar(int pos)
        {           
            // pos is in which measure?
            int curmeasure = 1 + pos / _measurelen;


            // Quel temps dans la mesure ?
            int rest = pos % _measurelen;
            int TimeInMeasure = 1 + (int)((float)rest / sequence1.Time.Quarter);
            lblBeat.Text = TimeInMeasure.ToString();

            // change time in measure => draw cell in control
            if (TimeInMeasure != _currentTimeInMeasure)
            {
                _currentTimeInMeasure = TimeInMeasure;
                chordAnalyserControl1.DisplayNotes(pos, curmeasure, TimeInMeasure);
            }
            

            // Change measure => offset control
            if (curmeasure != _currentMeasure)
            {
                
                //if (curmeasure != _currentMeasure)
                    _currentMeasure = curmeasure;
                
                int val = 0;

                int LargeurCellule = chordAnalyserControl1.TimeLineY + 1;
                int LargeurMesure = LargeurCellule * sequence1.Numerator; // keep one measure on the left
                int offsetx = LargeurCellule + (_currentMeasure - 1) * (LargeurMesure);

                val = LargeurCellule + (int)((_currentMeasure/(float)NbMeasures) * (int)(positionHScrollBar.Maximum - positionHScrollBar.Minimum));

                if ( positionHScrollBar.Minimum <= val && val <= positionHScrollBar.Maximum)
                    positionHScrollBar.Value = val;


                // ensure to Keep 1 measure on the left
                int W = chordAnalyserControl1.maxStaffWidth - offsetx - pnlDisplay.Width;

                if (offsetx > LargeurMesure)
                {
                    if (offsetx < chordAnalyserControl1.maxStaffWidth - pnlDisplay.Width)
                    {
                        chordAnalyserControl1.OffsetX = offsetx - LargeurMesure;
                    }
                    else
                    {
                        chordAnalyserControl1.OffsetX = chordAnalyserControl1.maxStaffWidth - pnlDisplay.Width;
                    }

                  
                }

                lblNumMeasure.Text = "Measure: " + _currentMeasure;
            }
            
        }

        #endregion Scroll ChordsControl


        #region handle messages

        /// <summary>
        /// Event: loading of midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //loading = true;
            try
            {
                //toolStripProgressBarPlayer.Value = e.ProgressPercentage;
                //progressBarPlayer.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: loading of midi file terminated: launch song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            LoadSequencer(sequence1);
            
            DrawControls();

            DisplayResults();

            //AnalyseInstruments();
            
            //InitCbTracks();
        }

        /// <summary>
        /// Load the midi file in the sequencer
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncFile(string fileName)
        {
            try
            {
                //progressBarPlayer.Visible = true;

                ResetSequencer();
                if (fileName != "\\")
                {
                    sequence1.LoadAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            outDevice.Send(e.Message);

        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            _currentMeasure = -1;
            PlayerState = PlayerStates.Stopped;
        }

        #endregion handle messages


        #region buttons
        private void btnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_pause;

        }

        private void btnRewind_MouseHover(object sender, EventArgs e)
        {
            if(PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnRewind.Image = Properties.Resources.btn_blue_prev;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_green_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_pause;

        }

        private void btnRewind_MouseLeave(object sender, EventArgs e)
        {
            btnRewind.Image = Properties.Resources.btn_black_prev;
        }


        #endregion buttons


        #region positionHSCrollBar

        private void SetScrollBarValues()
        {
            if (pnlDisplay == null || chordAnalyserControl1 == null)
                return;

            // Width of control
            int W = chordAnalyserControl1.maxStaffWidth;

            if (W <= pnlDisplay.Width)
            {
                positionHScrollBar.Visible = false;
                positionHScrollBar.Maximum = 0;
                chordAnalyserControl1.OffsetX = 0;
                positionHScrollBar.Value = 0;
            }
            else if (W > pnlDisplay.Width)
            {
                positionHScrollBar.Maximum = W - pnlDisplay.Width;
                positionHScrollBar.Visible = true;
            }

        }

        private void PositionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {            
            chordAnalyserControl1.OffsetX = e.NewValue;
        }

        private void PositionHScollBar_ValueChanged(object sender, EventArgs e)
        {
            //ColorSlider.ColorSlider c = (ColorSlider.ColorSlider)sender;
            //chordAnalyserControl1.OffsetX = Convert.ToInt32(c.Value);
        }

        /// <summary>
        /// Set positionHScrollbar Width equal to chord control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void chordAnalyserControl1_WidthChanged(object sender, int value)
        {
            positionHScrollBar.Width = (pnlDisplay.Width > chordAnalyserControl1.Width ? chordAnalyserControl1.Width : pnlDisplay.Width);

            // Set maximum & visibility
            SetScrollBarValues();
        }


        #endregion positionHScrollBar


        #region Display results

        private void DisplayResults()
        {

            // Display chods in the textbox
            ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);
            Dictionary<int, (string, string)> Gridchords = Analyser.Gridchords;

            string res = string.Empty;
            foreach (KeyValuePair<int, (string, string)> pair in Gridchords)
            {
                res += string.Format("{0} - {1}", pair.Key, pair.Value) + "\r\n";
            }
            txtOverview.Text = res;


            // Display Chords in boxes
            this.chordAnalyserControl1.Gridchords = Gridchords;


        }

        #endregion Display results


        #region Form

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {            
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();
            base.OnClosed(e);
        }

        /// <summary>
        /// Override form load event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sequence1.LoadProgressChanged += HandleLoadProgressChanged;
            sequence1.LoadCompleted += HandleLoadCompleted;

            LoadAsyncFile(MIDIfileFullPath);

            base.OnLoad(e);
        }

        private void frmChords_Load(object sender, EventArgs e)
        {
            #region setwindowlocation
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmChordsMaximized)
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmChordsSize;
                
            }
            #endregion

            this.Activate();
        }

        private void frmChords_Resize(object sender, EventArgs e)
        {
            if (pnlDisplay != null)
            {
                pnlDisplay.Width = pnlTop.Width;
                pnlDisplay.Height = tabPageDiagrams.Height - pnlTop.Height - pnlBottom.Height;
            }

            if (chordAnalyserControl1 != null)
                positionHScrollBar.Width = (pnlDisplay.Width > chordAnalyserControl1.Width ? chordAnalyserControl1.Width : pnlDisplay.Width);

            // Set maximum & visibility
            SetScrollBarValues();
        }

        private void frmChords_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmChordsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmChordsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmChordsLocation = Location;
                    Properties.Settings.Default.frmChordsSize = Size;
                    Properties.Settings.Default.frmChordsMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }

        #endregion Form


        #region Midi

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


        #endregion Midi


        #region Play stop pause

        private void ResetSequencer()
        {
            if (timer1 != null)
                timer1.Stop();
            newstart = 0;
            laststart = 0;
            _currentMeasure = -1;
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            StopMusic(); 
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
                    sequencer1.Continue();
                    break;

                default:
                    // First play                
                    FirstPlaySong(newstart);
                    break;
            }
        }

        /// <summary>
        /// PlaySong for first time
        /// </summary>
        public void FirstPlaySong(int ticks)
        {
            try
            {
                PlayerState = PlayerStates.Playing;
                nbstop = 0;
                _currentMeasure = -1;
                BtnStatus();
                sequencer1.Start();

                if (ticks > 0)
                    sequencer1.Position = ticks;

                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Display according to play, pause, stop status
        /// </summary>
        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    btnPlay.Image = Properties.Resources.btn_green_pause;
                    //btnStop.Image = Properties.Resources.btn_black_stop;
                    btnPlay.Enabled = true;  // to allow pause
                    //btnStop.Enabled = true;  // to allow stop 
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnPlay.Enabled = true;  // to allow play
                    //btnStop.Enabled = true;  // to allow stop
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        //btnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                    {
                        //btnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    }
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Stop Music player
        /// </summary>
        private void StopMusic()
        {
            PlayerState = PlayerStates.Stopped;
            try
            {
                sequencer1.Stop();

                // Si point de départ n'est pas le début du morceau
                if (newstart > 0)
                {
                    if (nbstop > 0)
                    {
                        newstart = 0;
                        nbstop = 0;
                        AfterStopped();
                    }
                    else
                    {
                        positionHScrollBar.Value = newstart + positionHScrollBar.Minimum;
                        nbstop = 1;
                    }
                }
                else
                {
                    // Point de départ = début du morceau
                    AfterStopped();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        /// <summary>
        /// Things to do at the end of a song
        /// </summary>
        private void AfterStopped()
        {
            // Buttons play & stop 
            BtnStatus();
            
            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);

                DisplayPositionHScrollBar(0);

                positionHScrollBar.Value = positionHScrollBar.Minimum;
                chordAnalyserControl1.OffsetX = 0;
                
                laststart = 0;
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
        }

        /// <summary>
        /// Display Time elapse
        /// </summary>
        private void DisplayTimeElapse(int pos)
        {
            double dpercent = 100 * pos / (double)_totalTicks;
            lblPercent.Text = string.Format("{0}%", (int)dpercent);

            double maintenant = (dpercent * _duration) / 100;  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            lblElapsed.Text = string.Format("{0:00}:{1:00}", Min, Sec);
        }

        #endregion Play stop pause

    }
}
