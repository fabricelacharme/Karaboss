using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;
using System.IO;

namespace Karaboss
{
    public partial class frmPianoTraining : Form
    {

        #region private decl        
        private int TempoOrig = 0;
        private int TempoDelta = 100;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;

        private int resolution = 4;
        private int _measurelen = 0;
        private int xScale = 20;
        private float zoomx = 1.0f;
        private float zoomy = 1.0f;
        private bool scrolling = false;
        private bool closing = false;
        private int newstart = 0;
        private int nbstop = 0;
        private int laststart = 0;      // Start time to play

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,        
        }
        private PlayerStates PlayerState;

        #endregion

        #region controls       
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();
        private Sequence sequence1 = new Sequence();
        #endregion


        public frmPianoTraining(OutputDevice outdevicePiano, string FileName)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;
            this.MouseWheel += new MouseEventHandler(FrmPianoTraining_MouseWheel);

            timer1.Interval = 20;

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);
        
            outDevice = outdevicePiano;

            zoomx = 1.0f;
            zoomy = 10.0f;
            resolution = 4;

            SetTitle(FileName);
            
        }

       

        #region timer        
        private void ScrollView()
        {
            int offset = Convert.ToInt32(sequencer1.Position * vPianoRollControl1.yScale);
            vPianoRollControl1.OffsetY = offset;
        }

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


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!scrolling)
            {
                // Display time elapse
                double dpercent = 100 * sequencer1.Position / (double)_totalTicks;
                DisplayTimeElapse(dpercent);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        ScrollView();
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

                #region position hscrollbar
                try
                {
                    if (PlayerState == PlayerStates.Playing && sequencer1.Position < positionHScrollBar.Maximum - positionHScrollBar.Minimum)
                    {                        
                        positionHScrollBar.Value = sequencer1.Position + positionHScrollBar.Minimum;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("Error positionHScrollBarNew.Value - " + ex.Message);
                }
                #endregion position hscrollbar
            }
        }

        #endregion


        #region DrawControls

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

                TempoOrig = _tempo;
                lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

                // Display
                int Min = (int)(_duration / 60);
                int Sec = (int)(_duration - (Min * 60));
                lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                _measurelen = sequence1.Time.Measure;
        }

        /// <summary>
        /// Calculate BPM
        /// </summary>
        /// <param name="tempo"></param>
        /// <returns></returns>
        private int GetBPM(int tempo)
        {
            const float kOneMinuteInMicroseconds = 60000000;
            float kTimeSignatureNumerator = (float)sequence1.Numerator; 
            float kTimeSignatureDenominator = (float)sequence1.Denominator;  
                                                          
            float BPM = (kOneMinuteInMicroseconds / (float)tempo) * (kTimeSignatureDenominator / 4.0f);
            return (int)BPM;
        }

        private void DrawControls()
        {
            try
            {
                #region top                         
                // Maximum of horizontal scroll bar is a multiple of measures            
                int dur = sequence1.GetLength();
                int lastenoteticks = sequence1.GetLastNoteEndTime();
                int nbmeasures = 0;
                // Conversion to int gives strange results for the number of measures
                // because it round to the closest integer, so 4.1 measures will gives only 4 measures instead of 5
                if (_measurelen > 0)
                    nbmeasures = Convert.ToInt32(lastenoteticks / _measurelen);
                // compares time for all measures with time of last note
                int totaltimemeasures = nbmeasures * _measurelen;
                if (lastenoteticks > totaltimemeasures)
                    nbmeasures++;

                totaltimemeasures = nbmeasures * _measurelen;

                positionHScrollBar.Maximum = totaltimemeasures + _measurelen;
                positionHScrollBar.Minimum = _measurelen;
                positionHScrollBar.TickStyle = TickStyle.TopLeft;
                positionHScrollBar.ScaleDivisions = nbmeasures;
                positionHScrollBar.TickDivide = _measurelen;
                positionHScrollBar.SmallChange = (uint)nbmeasures;
                positionHScrollBar.LargeChange = (uint)nbmeasures;
                if (_measurelen > 0)
                    positionHScrollBar.MouseWheelBarPartitions = _measurelen;

                #endregion

                pianoControl1.Location = new Point(0, pnlRedPianoSep.Height);
                pianoControl1.Orientation = Orientation.Horizontal;
                pianoControl1.Size = new Size(pianoControl1.totalLength, pnlPiano.Height - pnlRedPianoSep.Height);

                pianoControl1.PianoKeyDown += new EventHandler<PianoKeyEventArgs>(PianoControl1_PianoKeyDown);
                pianoControl1.PianoKeyUp += new EventHandler<PianoKeyEventArgs>(PianoControl1_PianoKeyUp);

                // Notes du piano
                pianoControl1.LowNoteID = vPianoRollControl1.LowNoteID = 23;
                pianoControl1.HighNoteID = vPianoRollControl1.HighNoteID = 108;
                vPianoRollControl1.Sequence1 = sequence1;
                vPianoRollControl1.zoomy = zoomy;
                pianoControl1.zoom = zoomx;
                vPianoRollControl1.xScale = pianoControl1.Scale;
                vPianoRollControl1.OffsetChanged += new Sanford.Multimedia.Midi.VPianoRoll.OffsetChangedEventHandler(vPianoRollControl1_OffsetChanged);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
        
        #endregion


        #region Play stop pause

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
            BtnPlay.Parent.Focus();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopMusic();
            BtnStop.Parent.Focus();
        }

        /// <summary>
        /// Initialize sequencer
        /// </summary>
        private void ResetSequencer()
        {
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Play_icon;                    
                    BtnPlay.Enabled = true;  // to allow pause
                    BtnStop.Enabled = true;  // to allow stop 
                    
                    break;

                case PlayerStates.Paused:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Pause_icon;
                    BtnPlay.Enabled = true;  // to allow play
                    BtnStop.Enabled = true;  // to allow stop
                    break;

                case PlayerStates.Stopped:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Play_icon;
                    BtnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        //btnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                        BtnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    break;
               
                default:
                    break;
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
                        pianoControl1.Reset();
                        positionHScrollBar.Value = newstart + positionHScrollBar.Minimum;
                        vPianoRollControl1.OffsetY = Convert.ToInt32(newstart * vPianoRollControl1.yScale);
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
            //sheetmusic.BPlaying = false;

            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);

                positionHScrollBar.Value = positionHScrollBar.Minimum;
                vPianoRollControl1.OffsetY = 0;
                pianoControl1.Reset();              
                laststart = 0;              
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
        }

        #endregion

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


        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        sequencer1.Position = e.NewValue - (int)positionHScrollBar.Minimum;
                        break;
                    case PlayerStates.Paused:
                        sequencer1.Position = e.NewValue - (int)positionHScrollBar.Minimum;
                        vPianoRollControl1.OffsetY = Convert.ToInt32((e.NewValue - (int)positionHScrollBar.Minimum) * vPianoRollControl1.yScale);
                        nbstop = 0;
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        break;
                    case PlayerStates.Stopped:
                        vPianoRollControl1.OffsetY = Convert.ToInt32((e.NewValue - (int)positionHScrollBar.Minimum) * vPianoRollControl1.yScale);
                        nbstop = 0;
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        break;
                }
                positionHScrollBar.Parent.Focus();
                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }

        private void positionHScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (scrolling) return;

            switch (PlayerState)
            {
                case PlayerStates.Playing:                    
                    break;
                case PlayerStates.Paused:
                    break;
                case PlayerStates.Stopped:
                    vPianoRollControl1.OffsetY = Convert.ToInt32((int)(positionHScrollBar.Value - positionHScrollBar.Minimum) * vPianoRollControl1.yScale);
                    newstart = (int)(positionHScrollBar.Value - positionHScrollBar.Minimum);
                    double dpercent = 100 * newstart / (double)_totalTicks;
                    DisplayTimeElapse(dpercent);

                    break;
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
            PlayerState = PlayerStates.Stopped;
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if (closing)
            {
                return;
            }
            outDevice.Send(e.Message);
            pianoControl1.Send(e.Message);
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
            //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                //pianoControl1.Send(message);
            }
        }

        #endregion


        #region pianoControl

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pianoControl1.PressPianoKey(e.KeyCode);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pianoControl1.ReleasePianoKey(e.KeyCode);
            base.OnKeyUp(e);
        }


        private void PianoControl1_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void PianoControl1_PianoKeyUp(object sender, PianoKeyEventArgs e)
        {
            #region Guard
            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        #endregion pianoControl


        #region  pianoRollControl

        private void vPianoRollControl1_OffsetChanged(object sender, int value)
        {
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    break;
                case PlayerStates.Paused:                    
                case PlayerStates.Stopped:
                    int newvalue = Convert.ToInt32((value / vPianoRollControl1.yScale)) + (int)positionHScrollBar.Minimum;
                    if (newvalue > positionHScrollBar.Maximum) newvalue = (int)positionHScrollBar.Maximum;
                    positionHScrollBar.Value = newvalue;
                    newstart = (int)(positionHScrollBar.Value - positionHScrollBar.Minimum); 
                    break;
            }
        }

        #endregion


        #region from load close

        private void FrmPianoTraining_MouseWheel(object sender, MouseEventArgs e)
        {
            Point PpianoRoll = pnlScrollView.PointToClient(Cursor.Position);            
            Rectangle rectPianoRoll = new Rectangle(0, 0, pnlScrollView.Width, pnlScrollView.Height);
            
            if (rectPianoRoll.Contains(PpianoRoll))
            {
                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        break;
                    case PlayerStates.Paused:
                        break;
                    case PlayerStates.Stopped:                        
                        int v = e.Delta / 120 * (int)(positionHScrollBar.Maximum - positionHScrollBar.Minimum) / positionHScrollBar.MouseWheelBarPartitions;
                        positionHScrollBar.Value = SetProperValue((int)positionHScrollBar.Value + v);                                                
                        break;
                }
            }
        }

        private int SetProperValue(int val)
        {
            if (val < positionHScrollBar.Minimum) return (int)positionHScrollBar.Minimum;
            else if (val > positionHScrollBar.Maximum) return (int)positionHScrollBar.Maximum;
            else return val;
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

        private void frmPianoTraining_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location

            // If window is maximized
            if (Properties.Settings.Default.frmPianoTrainingMaximized)
            {
                Location = Properties.Settings.Default.frmPianoTrainingLocation;                
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmPianoTrainingLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmPianoTrainingSize;
            }
        }

        private void frmPianoTraining_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmPianoTrainingLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmPianoTrainingMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmPianoTrainingLocation = Location;
                    Properties.Settings.Default.frmPianoTrainingSize = Size;
                    Properties.Settings.Default.frmPianoTrainingMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }
        

        private void frmPianoTraining_Resize(object sender, EventArgs e)
        {
            positionHScrollBar.Width = pnlTop.Width - positionHScrollBar.Left - 40;
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
            if (keyData == Keys.Left)
            {
                if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped && newstart > 0))
                {
                    StopMusic();
                    return true;                    
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmPianoTraining_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:                    
                      PlayPauseMusic();
                    break;
                    
                case Keys.Left:                    
                    if (PlayerState == PlayerStates.Paused)                        
                        StopMusic();                        
                    break;

                case Keys.Add:                    
                case Keys.Subtract:
                case Keys.D6:
                case Keys.Decimal:
                    // Tempo +-
                    KeyboardSelectTempo(e);
                    break;

            }
        }

        private void frmPianoTraining_KeyUp(object sender, KeyEventArgs e)
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
        }

        #endregion


        #region tempo

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
                    if (TempoDelta < 400)
                        TempoDelta += 10;
                    ModTempo();
                    break;
            }
        }

        /// <summary>
        /// Speed up 
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
        /// Minus speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoMinus_Click(object sender, EventArgs e)
        {
            if (TempoDelta < 400)
                TempoDelta += 10;
            ModTempo();
        }

        private void ModTempo()
        {
            int tempo = TempoDelta * TempoOrig / 100;

            // If no change => out
            if (tempo == sequence1.Tempo)            
                return;
            
            
            lblTempoValue.Text = string.Format("{0}%", TempoDelta);
            _bpm = GetBPM(tempo);
            lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}" , tempo, _bpm);

            // Stop sequencer if it was playing
            if (PlayerState == PlayerStates.Playing)
                sequencer1.Stop();

            _tempo = tempo;
            sequence1.Tempo = _tempo;
            sequence1.Time = new TimeSignature(sequence1.Numerator, sequence1.Denominator, sequence1.Division, _tempo);

            // Remove all tempo events
            foreach (Track trk in sequence1.tracks)
            {
                trk.RemoveTempoEvent();
            }

            // Insert new tempo event in track 0
            sequence1.tracks[0].insertTempo(_tempo);

            // Update Midi Times
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _bpm = GetBPM(_tempo);
            // Update display duration
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

            // Restart sequencer if it was playing
            if (PlayerState == PlayerStates.Playing)
                sequencer1.Continue();

        }


        #endregion

       
    }
}
