using Karaboss.GuitarTraining;
using Karaboss.Resources.Localization;
using Karaboss.Utilities;
using MusicTxt;
using MusicXml;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;


namespace Karaboss
{
    public partial class frmGuitarTraining : Form
    {

        #region private decl        
        MusicXmlReader MXmlReader;
        MusicTxtReader MTxtReader;

        List<MessageDto> messageList = new List<MessageDto>();
        Dictionary<int, int> dicChannel = new Dictionary<int, int>();

        class InstrumentInfo
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Label { get; set; }
            public string Instrument { get; set; }
            public int TrackNumber { get; set; }
            public int Channel { get; set; }
            public int ProgramChange { get; set; }   // Utilisé pour dessiné les controles, mais peut changer en cours de morceau
            public bool Display { get; set; }

        }

        List<InstrumentInfo> InstrumentInfos = new List<InstrumentInfo>();

        private const int LowNoteID = 21;
        private const int HighNoteID = 109;

        private int TempoOrig = 0;
        private int TempoDelta = 100;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;

        //private int resolution = 4;
        private int _measurelen = 0;
        //private int xScale = 20;
        //private float zoomx = 1.0f;
        //private float zoomy = 1.0f;
        private bool scrolling = false;
        private bool closing = false;
        private int newstart = 0;
        private int nbstop = 0;
        //private int laststart = 0;      // Start time to play

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        // Nombre d'instruments trouvés 
        private int nbGuitar = 0;
        private int nbBass = 0;

        private bool bAlltracks;
        private int tracknum = -1;
        private Track SingleTrack;
        private int SingleTrackNumber;
        private int SingleTrackChannel;


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

        Dictionary<string, GuitarControl> dicGuitar = new Dictionary<string, GuitarControl>();
        Dictionary<string, BassControl> dicBass = new Dictionary<string, BassControl>();
        int leftWith = 110;

        public frmGuitarTraining(OutputDevice outdeviceGuitar, string FileName)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;            

            timer1.Interval = 20;
            timer1.Tick += new EventHandler(timer1_Tick);

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);

            outDevice = outdeviceGuitar;            

            SetTitle(FileName);

            tracknum = -1;
            // All tracks                      
            SingleTrack = null;
            SingleTrackNumber = -1;
            SingleTrackChannel = -1;
            bAlltracks = true;

        }

        /// <summary>
        /// Remove flickering https://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

      

        #region timer        

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
                        //ScrollView();
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
            Text = "Karaboss - " + Strings.GuitarTraining + " - " + Path.GetFileName(fileName);
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

            // Load tempos map
            TempoUtilities.lstTempos = TempoUtilities.GetAllTempoChanges(sequence1);            

            //_duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _duration = TempoUtilities.GetMidiDuration(_totalTicks, _ppqn);

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
            // see http://midi.teragonaudio.com/tech/midifile/ppqn.htm
            const float kOneMinuteInMicroseconds = 60000000;
            //float kTimeSignatureNumerator = (float)sequence1.Numerator;
            //float kTimeSignatureDenominator = (float)sequence1.Denominator;

            //float BPM = (kOneMinuteInMicroseconds / (float)tempo) * (kTimeSignatureDenominator / 4.0f);            
            float BPM = kOneMinuteInMicroseconds / (float)tempo;

            return (int)BPM;
        }

        private void DrawControls()
        {
            try
            {
                #region top 

                lblVersion.Text = Application.ProductVersion;

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

                DrawInstruments();


            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// Analyse list of instruments needed
        /// </summary>
        private void AnalyseInstruments()
        {           
            int idx = 0;
            nbGuitar = 0;
            nbBass = 0;

            // Create list of InstrumentInfo
            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                Track trk = sequence1.tracks[i];
                if (trk.ContainsNotes)
                {
                    int PC = trk.ProgramChange;                    
                    switch (PC)
                    {
                        // Guitars
                        case (int)MIDIInstrument.AcousticGuitarNylon://25
                        case (int)MIDIInstrument.AcousticGuitarSteel://26
                        case (int)MIDIInstrument.ElectricGuitarJazz://27
                        case (int)MIDIInstrument.ElectricGuitarClean://28
                        case (int)MIDIInstrument.ElectricGuitarMuted://29
                        case (int)MIDIInstrument.OverdrivenGuitar://30
                        case (int)MIDIInstrument.DistortionGuitar://31
                        case (int)MIDIInstrument.GuitarHarmonics://32                            
                            InstrumentInfos.Add(new InstrumentInfo()
                            {
                                Type = "guitar",
                                Name = "Track" + i,
                                Label = (trk.Name != null ? trk.Name : "<NoName>"),
                                Instrument = Enum.GetName(typeof(MIDIInstrument), PC),
                                TrackNumber = i,
                                Channel = trk.MidiChannel,
                                ProgramChange = PC,
                                Display = true,
                            });

                            nbGuitar++;
                            idx++;
                            break;

                        // Bass
                        case (int)MIDIInstrument.AcousticBass://33
                        case (int)MIDIInstrument.ElectricBassFinger://34
                        case (int)MIDIInstrument.ElectricBassPick://35
                        case (int)MIDIInstrument.FretlessBass://36
                        case (int)MIDIInstrument.SlapBass1://37
                        case (int)MIDIInstrument.SlapBass2://38
                        case (int)MIDIInstrument.SynthBass1://39
                        case (int)MIDIInstrument.SynthBass2://40                                                      
                            InstrumentInfos.Add(new InstrumentInfo()
                            {
                                Type = "bass",
                                Name = "Track" + i,
                                Label = trk.Name,
                                Instrument = Enum.GetName(typeof(MIDIInstrument), PC),
                                TrackNumber = i,
                                Channel = trk.MidiChannel,
                                ProgramChange = PC,
                                Display = true,
                            });

                            nbBass++;
                            idx++;
                            break;


                    }
                }
            }

            if (InstrumentInfos.Count == 0)
            {
                // Add an item to InstrumentInfos
                InstrumentInfos.Add(new InstrumentInfo()
                {
                    Type = "guitar",
                    Name = "All Guitars",
                    Label = "All Guitars",
                    Instrument = "Guitar",
                    TrackNumber = -1,
                    Channel = -1,
                    ProgramChange = -1,
                    Display = true,
                });


                // Add an item to InstrumentInfos
                InstrumentInfos.Add(new InstrumentInfo()
                {
                    Type = "bass",
                    Name = "All Bass",
                    Label = "All Bass",
                    Instrument = "Bass",
                    TrackNumber = -2,
                    Channel = -2,
                    ProgramChange = -2,
                    Display = true,
                });

               
            }


        }

        /// <summary>
        /// Draw guitars then basses
        /// </summary>
        private void DrawInstruments()
        {
            int Y = 20; ;
            int i = 0;
            InstrumentInfo ii;
            InfoTrackPanel pnlLeft;

            pnlMiddle.BackColor = System.Drawing.ColorTranslator.FromHtml("#1d1d1d");
            dicGuitar.Clear();
            dicBass.Clear();

            // Remove all controls (InfoTrac, GuitarControl, BassControl
            pnlMiddle.Controls.Clear();

            

            // Cas ou aucune guitare et aucune basse            
            if (nbGuitar == 0 && nbBass == 0)
            {
                #region no instrument found

                for (i = 0; i < InstrumentInfos.Count; i++)
                {
                    ii = InstrumentInfos[i];
                    if (ii.Display)
                    {
                        switch (ii.Type)
                        {
                            case "guitar":
                                pnlLeft = new InfoTrackPanel
                                {
                                    Width = leftWith,
                                    Height = 150,
                                    BackColor = System.Drawing.ColorTranslator.FromHtml("#937641"), //System.Drawing.ColorTranslator.FromHtml("#e3a21a"),
                                    TextColor = Color.White,
                                    Left = 0,
                                    Top = Y,
                                    TrackNumber = -1,
                                    TrackName = "All Guitars",
                                    Instrument = "Any",
                                    Channel = string.Format("Channel: {0}", -1),
                                };

                                pnlLeft.OnRemoveTrack += new InfoTrackPanel.RemoveTrackEventHandler(Display_RemoveTrack);
                                pnlMiddle.Controls.Add(pnlLeft);


                                // Add a guitar control
                                GuitarControl GC = new GuitarControl();
                                GC.Name = "All Guitars";
                                GC.Left = pnlLeft.Width;
                                GC.Top = Y;
                                GC.Width = Width - leftWith - 40;
                                pnlMiddle.Controls.Add(GC);

                                // Add control to dictionary
                                dicGuitar.Add("All Guitars", GC);
                                Y = Y + GC.Height + 40;
                                break;

                            case "bass":
                                pnlLeft = new InfoTrackPanel
                                {
                                    Width = leftWith,
                                    Height = 150,
                                    BackColor = System.Drawing.ColorTranslator.FromHtml("#748C6C"), //System.Drawing.ColorTranslator.FromHtml("#2b5797"),
                                    TextColor = Color.White,
                                    Left = 0,
                                    Top = Y,
                                    TrackNumber = -2,
                                    TrackName = "All Bass",
                                    Instrument = "Any",
                                    Channel = string.Format("Channel: {0}", -1),
                                };
                                pnlLeft.OnRemoveTrack += new InfoTrackPanel.RemoveTrackEventHandler(Display_RemoveTrack);
                                pnlMiddle.Controls.Add(pnlLeft);

                                // Add a bass control
                                BassControl BC = new BassControl();
                                BC.Name = "All Bass";
                                BC.Left = pnlLeft.Width;
                                BC.Top = Y;
                                BC.Width = Width - leftWith - 40;
                                pnlMiddle.Controls.Add(BC);

                                // Add control to dictionary
                                dicBass.Add("All Bass", BC);
                                Y = Y + BC.Height + 40;
                                break;
                        }
                    }
                }             
            }
            else
            {
                #region Guitars and Bass found
                // Draw first guitars
                for (i = 0; i < InstrumentInfos.Count; i++)
                {
                    ii = InstrumentInfos[i];

                    if (ii.Display)
                    {
                        switch (ii.ProgramChange)
                        {
                            case (int)MIDIInstrument.AcousticGuitarNylon://25
                            case (int)MIDIInstrument.AcousticGuitarSteel://26
                            case (int)MIDIInstrument.ElectricGuitarJazz://27
                            case (int)MIDIInstrument.ElectricGuitarClean://28
                            case (int)MIDIInstrument.ElectricGuitarMuted://29
                            case (int)MIDIInstrument.OverdrivenGuitar://30
                            case (int)MIDIInstrument.DistortionGuitar://31
                            case (int)MIDIInstrument.GuitarHarmonics://32        
                                // Add a panel information                        
                                pnlLeft = new InfoTrackPanel
                                {
                                    Width = leftWith,
                                    Height = 150,
                                    BackColor = System.Drawing.ColorTranslator.FromHtml("#937641"), //System.Drawing.ColorTranslator.FromHtml("#e3a21a"),
                                    TextColor = Color.White,
                                    Left = 0,
                                    Top = Y,
                                    TrackNumber = ii.TrackNumber,
                                    TrackName = ii.Label,
                                    Instrument = ii.Instrument,
                                    Channel = string.Format("Channel: {0}", ii.Channel),
                                };

                                pnlLeft.OnRemoveTrack += new InfoTrackPanel.RemoveTrackEventHandler(Display_RemoveTrack);
                                pnlMiddle.Controls.Add(pnlLeft);

                                // Add a guitar control
                                GuitarControl GC = new GuitarControl();
                                GC.Name = ii.Name;
                                GC.Left = pnlLeft.Width;
                                GC.Top = Y;
                                GC.Width = Width - leftWith - 40;
                                pnlMiddle.Controls.Add(GC);

                                // Add control to dictionary
                                dicGuitar.Add(ii.Name, GC);

                                Y = Y + GC.Height + 40;
                                break;
                        }
                    }
                }

                // Draw Bass
                for (i = 0; i < InstrumentInfos.Count; i++)
                {
                    ii = InstrumentInfos[i];

                    if (ii.Display)
                    {
                        switch (ii.ProgramChange)
                        {
                            case (int)MIDIInstrument.AcousticBass://33
                            case (int)MIDIInstrument.ElectricBassFinger://34
                            case (int)MIDIInstrument.ElectricBassPick://35
                            case (int)MIDIInstrument.FretlessBass://36
                            case (int)MIDIInstrument.SlapBass1://37
                            case (int)MIDIInstrument.SlapBass2://38
                            case (int)MIDIInstrument.SynthBass1://39
                            case (int)MIDIInstrument.SynthBass2://40
                                                                // Add a panel information
                                pnlLeft = new InfoTrackPanel
                                {
                                    Width = leftWith,
                                    Height = 150,
                                    BackColor = System.Drawing.ColorTranslator.FromHtml("#748C6C"), //System.Drawing.ColorTranslator.FromHtml("#2b5797"),
                                    TextColor = Color.White,
                                    Left = 0,
                                    Top = Y,
                                    TrackNumber = ii.TrackNumber,
                                    TrackName = ii.Label,
                                    Instrument = ii.Instrument,
                                    Channel = string.Format("Channel: {0}", ii.Channel),
                                };
                                pnlLeft.OnRemoveTrack += new InfoTrackPanel.RemoveTrackEventHandler(Display_RemoveTrack);
                                pnlMiddle.Controls.Add(pnlLeft);

                                // Add a bass control
                                BassControl BC = new BassControl();
                                BC.Name = ii.Name;
                                BC.Left = pnlLeft.Width;
                                BC.Top = Y;
                                BC.Width = Width - leftWith - 40;
                                pnlMiddle.Controls.Add(BC);

                                // Add control to dictionary
                                dicBass.Add(ii.Name, BC);

                                Y = Y + BC.Height + 40;
                                break;
                        }
                    }
                }
                
                #endregion

                #endregion
            }
        }

        /// <summary>
        ///  Remove a track from the display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="track"></param>
        private void Display_RemoveTrack(object sender, EventArgs e, int track)
        {
            // Search by track number
            for (int i = 0; i < InstrumentInfos.Count; i++)
            {                
                if (track == InstrumentInfos[i].TrackNumber)
                {
                    InstrumentInfos[i].Display = false;
                    break;
                }
            }          

            // Redraw all controls
            DrawInstruments();
        }

        /// <summary>
        /// Clear all controls
        /// </summary>
        private void ClearInstruments()
        {
            dicChannel.Clear();            
            foreach (Control item in pnlMiddle.Controls)
            {
                if (item.GetType() == typeof(GuitarControl))
                {
                    ((GuitarControl)item).Clear();
                }
                else if (item.GetType() == typeof(GuitarControl))
                {
                    ((BassControl)item).Clear();
                }
            }
            
        }


        #region Play stop pause

        /// <summary>
        /// Play button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();            
        }

        /// <summary>
        /// Stop button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopMusic();            
        }

        /// <summary>
        /// Initialize sequencer
        /// </summary>
        private void ResetSequencer()
        {
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
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
                    BtnPlay.Image = Properties.Resources.btn_green_play;
                    BtnStop.Image = Properties.Resources.btn_black_stop;
                    BtnPlay.Enabled = true;  // to allow pause
                    BtnStop.Enabled = true;  // to allow stop 
                    break;

                case PlayerStates.Paused:
                    BtnPlay.Image = Properties.Resources.btn_red_pause;
                    BtnPlay.Enabled = true;  // to allow play
                    BtnStop.Enabled = true;  // to allow stop
                    break;

                case PlayerStates.Stopped:
                    BtnPlay.Image = Properties.Resources.btn_black_play;
                    BtnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        BtnStop.Image = Properties.Resources.btn_red_stop;
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
            //sheetmusic.BPlaying = false;

            // Clear all
            ClearInstruments();

            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);

                positionHScrollBar.Value = positionHScrollBar.Minimum;                                
                //laststart = 0;
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
            AnalyseInstruments();
            DrawControls();
            InitCbTracks();
        }

        /// <summary>
        /// Load the midi file in the sequencer
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncMidiFile(string fileName)
        {
            try
            {                
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


        /// <summary>
        /// Event: end loading XML music file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadXmlCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            if (MXmlReader.seq == null)
                return;

            if (e.Error == null && e.Cancelled == false)
            {                
                CommonLoadCompleted(MXmlReader.seq);
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ResetSequencer();
                if (fileName != "\\")
                {
                    MXmlReader = new MusicXmlReader();
                    MXmlReader.LoadXmlCompleted += HandleLoadXmlCompleted;
                    MXmlReader.LoadXmlAsync(fileName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: TXT dump sequence loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            if (MTxtReader.seq == null) return;

            if (e.Error == null && e.Cancelled == false)
            {
                CommonLoadCompleted(MTxtReader.seq);
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Common to MIDI, XML 
        /// </summary>
        /// <param name="seq"></param>& TXT
        private void CommonLoadCompleted(Sequence seq)
        {
            if (seq == null) return;

            try
            {
                LoadSequencer(seq);
                AnalyseInstruments();
                DrawControls();
                InitCbTracks();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        nbstop = 0;
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        break;
                    case PlayerStates.Stopped:                        
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

            var cArray = new string(' ', 88).ToCharArray();
            List<string> noteList = new List<string>();
            List<string> fretList = new List<string>();
            var count = messageList.Count;
            foreach (var message in messageList)
            {
                if (message.MidiChannel == 1)
                {
                    var noteId = message.Data1 - LowNoteID;

                    string s = string.Format("{0}: {1}", message.Ticks.ToString("000000000"), new string(cArray));

                    noteList.Add(s);
                }
            }

            //Calculate diffs
            MessageDto currentMessageNoteOn = null;
            MessageDto previousMessageNoteOn = null;
            count = messageList.Count();
            for (var i = 0; i < count; i++)
            {
                var message = messageList[i];
                if (message.ChannelCommand == ChannelCommand.NoteOn &&
                    message.Data2 > 0)
                {
                    if (currentMessageNoteOn != null)
                        previousMessageNoteOn = currentMessageNoteOn;

                    currentMessageNoteOn = messageList[i];

                    if (previousMessageNoteOn == null)
                    {
                        currentMessageNoteOn.FretPosition = 3; //first note must fall at the middle of the fret
                    }
                    else
                    {
                        currentMessageNoteOn.NoteDiffToPrevious = currentMessageNoteOn.Data1 - previousMessageNoteOn.Data1;
                        previousMessageNoteOn.NoteDiffToNext = previousMessageNoteOn.Data1 - currentMessageNoteOn.Data1;
                        currentMessageNoteOn.TickDiffToPrevious = (int)(currentMessageNoteOn.Ticks - previousMessageNoteOn.Ticks);
                        previousMessageNoteOn.TickDiffToNext = (int)(previousMessageNoteOn.Ticks - currentMessageNoteOn.Ticks);

                        if (currentMessageNoteOn.Data1 == previousMessageNoteOn.Data1)
                        {
                            currentMessageNoteOn.FretPosition = previousMessageNoteOn.FretPosition; //keep the same fret position as the previous note
                        }
                        else if (currentMessageNoteOn.Data1 > previousMessageNoteOn.Data1)
                        {
                            currentMessageNoteOn.FretPosition = previousMessageNoteOn.FretPosition + 1; //one fret to the right
                        }
                        else if (currentMessageNoteOn.Data1 < previousMessageNoteOn.Data1)
                        {
                            currentMessageNoteOn.FretPosition = previousMessageNoteOn.FretPosition - 1; //one fret to the left
                        }
                    }
                }
            }

        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            //outDevice.Send(e.Message);

            if (bAlltracks)
            {
                outDevice.Send(e.Message);
                //pianoControl1.Send(e.Message);
            }
            else if (e.Message.MidiChannel == SingleTrackChannel)
            {
                outDevice.Send(e.Message);
                //pianoControl1.Send(e.Message);
            }


            // ProgramChange modifié en cours de route pour un Channel
            // Met à jour un dictionnaire des ProgramChange
            // Clé = Channel, valeur = ProgramChange (e.Message.Data1)
            if (e.Message.Command == ChannelCommand.ProgramChange)
            {
                if (!dicChannel.ContainsKey(e.Message.MidiChannel))
                {
                    dicChannel.Add(e.Message.MidiChannel, e.Message.Data1);                  
                }
            }

            
            if (e.Message.MidiChannel == 9) // Channel 9 is reserved for drums
            {
                // Channel 9 is reserved for drums                
            }
            else if (dicChannel.ContainsKey(e.Message.MidiChannel))
            {
                switch (dicChannel[e.Message.MidiChannel])
                {
                    case (int)MIDIInstrument.AcousticGrandPiano://1
                    case (int)MIDIInstrument.BrightAcousticPiano://2
                    case (int)MIDIInstrument.ElectricGrandPiano://3
                    case (int)MIDIInstrument.HonkyTonkPiano://4
                    case (int)MIDIInstrument.ElectricPiano1://5
                    case (int)MIDIInstrument.ElectricPiano2://6
                    case (int)MIDIInstrument.Harpsichord://7
                    case (int)MIDIInstrument.Clavinet://8
                        //pianoControl1.Send(e.Message);
                        break;

                     // Send infos to Guitar control
                    case (int)MIDIInstrument.AcousticGuitarNylon://25
                    case (int)MIDIInstrument.AcousticGuitarSteel://26
                    case (int)MIDIInstrument.ElectricGuitarJazz://27
                    case (int)MIDIInstrument.ElectricGuitarClean://28
                    case (int)MIDIInstrument.ElectricGuitarMuted://29
                    case (int)MIDIInstrument.OverdrivenGuitar://30
                    case (int)MIDIInstrument.DistortionGuitar://31
                    case (int)MIDIInstrument.GuitarHarmonics://32
                        if (nbBass == 0 && nbGuitar == 0)
                        {
                            // No guitar & Bass were found => single control for all guitars
                            if (dicGuitar.ContainsKey("All Guitars"))
                            {
                                var item = dicGuitar["All Guitars"];
                                this.BeginInvoke(
                                        new Action(
                                            delegate ()
                                            {
                                                ((GuitarControl)item).Send(e.Message);
                                            }
                                        ));
                            }
                        }
                        else
                        {
                            // Guitars found
                            // Pas la peine d'utiliser ProgramChange, il peut être réaffecté n'importe quand
                            var iiQuery = from ii in InstrumentInfos
                                          where e.Message.MidiChannel == ii.Channel //&& dicChannel[e.Message.MidiChannel] == ii.ProgramChange
                                          select ii;
                            if (iiQuery.Any())
                            {
                                var ii = iiQuery.First();
                                if (dicGuitar.ContainsKey(ii.Name))
                                {
                                    var item = dicGuitar[ii.Name];
                                    this.BeginInvoke(
                                            new Action(
                                                delegate ()
                                                {
                                                    ((GuitarControl)item).Send(e.Message);
                                                }
                                            ));
                                }
                            }
                        }
                        break;

                    // Send infos to Bass control
                    case (int)MIDIInstrument.AcousticBass://33
                    case (int)MIDIInstrument.ElectricBassFinger://34
                    case (int)MIDIInstrument.ElectricBassPick://35
                    case (int)MIDIInstrument.FretlessBass://36
                    case (int)MIDIInstrument.SlapBass1://37
                    case (int)MIDIInstrument.SlapBass2://38
                    case (int)MIDIInstrument.SynthBass1://39
                    case (int)MIDIInstrument.SynthBass2://40
                        if (nbGuitar == 0 && nbBass == 0)
                        {
                            // No Guitar & Bass were found => single control for Bass
                            if (dicBass.ContainsKey("All Bass"))
                            {
                                var item = dicBass["All Bass"];
                                this.BeginInvoke(
                                        new Action(
                                            delegate ()
                                            {
                                                ((BassControl)item).Send(e.Message);
                                            }
                                        ));
                            }
                        }
                        else
                        {
                            // Pas la peine d'utiliser ProgramChange, il peut être réaffecté n'importe quand
                            var iibQuery = from ii in InstrumentInfos
                                           where e.Message.MidiChannel == ii.Channel //&& dicChannel[e.Message.MidiChannel] == ii.ProgramChange
                                           select ii;
                            if (iibQuery.Any())
                            {
                                var ii = iibQuery.First();
                                if (dicBass.ContainsKey(ii.Name))
                                {
                                    var item = dicBass[ii.Name];
                                    this.BeginInvoke(
                                            new Action(
                                                delegate ()
                                                {
                                                    ((BassControl)item).Send(e.Message);
                                                }
                                            ));
                                }
                            }
                        }
                        break;

                    default:
                        //pianoControl1.Send(e.Message);
                        break;
                }
            }
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
            }
        }

        #endregion


        #region from load close
      
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
                    sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;
                    
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


        private void frmGuitarTraining_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location

            // If window is maximized
            if (Properties.Settings.Default.frmGuitarTrainingMaximized)
            {
                Location = Properties.Settings.Default.frmGuitarTrainingLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmGuitarTrainingLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmGuitarTrainingSize;
            }
        }

        private void frmGuitarTraining_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmGuitarTrainingLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmGuitarTrainingMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmGuitarTrainingLocation = Location;
                    Properties.Settings.Default.frmGuitarTrainingSize = Size;
                    Properties.Settings.Default.frmGuitarTrainingMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }

            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmExplorer"].Restore();
                Application.OpenForms["frmExplorer"].Activate();
            }


        }

        /// <summary>
        /// Form resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmGuitarTraining_Resize(object sender, EventArgs e)
        {
            positionHScrollBar.Width = pnlTop.Width - positionHScrollBar.Left - 10;

            int X1 = pnlDisplay.Left + pnlDisplay.Width;
            int X2 = pnlTop.Width - pnlAppName.Width - 10;
            if (X2 > X1)
                pnlAppName.Left = pnlTop.Width - pnlAppName.Width - 10;
            else
                pnlAppName.Left = X1 + 10;

            try
            {                
                foreach (Control item in pnlMiddle.Controls)
                {
                    if (item.GetType() == typeof(GuitarControl))
                    {
                        ((GuitarControl)item).Left = leftWith;
                        ((GuitarControl)item).Width = Width - leftWith - 40;
                    }
                    else if (item.GetType() == typeof(BassControl))
                    {
                        ((BassControl)item).Left = leftWith;
                        ((BassControl)item).Width = Width - leftWith - 40;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

        /// <summary>
        /// Form keydown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmGuitarTraining_KeyDown(object sender, KeyEventArgs e)
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

        /// <summary>
        /// Form keyup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmGuitarTraining_KeyUp(object sender, KeyEventArgs e)
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

        /// <summary>
        /// Change tempo with keyboard
        /// </summary>
        /// <param name="e"></param>
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

        private void ModTempo()
        {
            _tempo = TempoDelta * TempoOrig / 100;

            // Change clock tempo
            sequencer1.Tempo = _tempo;

            lblTempoValue.Text = string.Format("{0}%", TempoDelta);
                                   

            // Update Midi Times            
            _bpm = GetBPM(_tempo);
            lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

            // Update display duration
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

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

        #endregion


        #region CbTrack

        private void InitCbTracks()
        {
            int i = 1;
            string N;
            CbTracks.Items.Clear();
            CbTracks.Items.Add("All tracks");
            foreach (Track trk in sequence1.tracks)
            {
                N = "<NoName>";
                if (trk.Name != null)
                {
                    if (trk.Name.Trim() != "")
                        N = trk.Name.Trim();
                }
                CbTracks.Items.Add(i.ToString("00") + " " + "[" + trk.MidiChannel.ToString("00") + "]" + " - " + N + " - " + "(" + MidiFile.PCtoInstrument(trk.ProgramChange) + ")");
                i++;
            }

            CbTracks.SelectedIndex = 0;
        }

        private void CbTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (vPianoRollControl1 == null)
            //    return;

            sequencer1.AllSoundOff();
            //pianoControl1.Reset();

            tracknum = CbTracks.SelectedIndex;

            // All track
            if (tracknum == 0)
            {
                // All tracks                      
                SingleTrack = null;
                SingleTrackNumber = -1;
                tracknum = -1;
                SingleTrackChannel = -1;
                bAlltracks = true;
            }
            else
            {
                // One track
                if (tracknum > 0)
                {
                    tracknum = tracknum - 1;
                    SingleTrack = sequence1.tracks[tracknum];
                    SingleTrackNumber = tracknum;
                    SingleTrackChannel = SingleTrack.MidiChannel;
                    bAlltracks = false;
                }
            }

            // Track pour pianoRoll
            if (tracknum != -1)
            {
                //vPianoRollControl1.TrackNum = tracknum;
            }
            else
            {
                //vPianoRollControl1.TrackNum = -1;
            }
            
            if (CbTracks.Parent != null) 
                CbTracks.Parent.Focus();
        }

        #endregion


        #region boutons
        private void BtnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                BtnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                BtnPlay.Image = Properties.Resources.btn_blue_pause;
            else if (PlayerState == PlayerStates.Playing)
                BtnPlay.Image = Properties.Resources.btn_blue_play;

        }

        private void BtnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                BtnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                BtnPlay.Image = Properties.Resources.btn_red_pause;
            else if (PlayerState == PlayerStates.Playing)
                BtnPlay.Image = Properties.Resources.btn_green_play;

        }

        private void BtnStop_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                BtnStop.Image = Properties.Resources.btn_blue_stop;

        }

        private void BtnStop_MouseLeave(object sender, EventArgs e)
        {
            BtnStop.Image = Properties.Resources.btn_black_stop;
        }

        #endregion
    }

    #region MIDIInstrument
    public enum MIDIInstrument
    {
        //PIANO
        AcousticGrandPiano,//1
        BrightAcousticPiano,//2
        ElectricGrandPiano,//3
        HonkyTonkPiano,//4
        ElectricPiano1,//5
        ElectricPiano2,//6
        Harpsichord,//7
        Clavinet,//8


        //CHROMATICPERCUSSION
        Celesta,//9
        Glockenspiel,//10
        MusicBox,//11
        Vibraphone,//12
        Marimba,//13
        Xylophone,//14
        TubularBells,//15
        Dulcimer,//16


        //ORGAN
        DrawbarOrgan,//17
        PercussiveOrgan,//18
        RockOrgan,//19
        ChurchOrgan,//20
        ReedOrgan,//21
        Accordion,//22
        Harmonica,//23
        TangoAccordion,//24


        //GUITAR
        AcousticGuitarNylon,//25
        AcousticGuitarSteel,//26
        ElectricGuitarJazz,//27
        ElectricGuitarClean,//28
        ElectricGuitarMuted,//29
        OverdrivenGuitar,//30
        DistortionGuitar,//31
        GuitarHarmonics,//32

        //BASS
        AcousticBass,//33
        ElectricBassFinger,//34
        ElectricBassPick,//35
        FretlessBass,//36
        SlapBass1,//37
        SlapBass2,//38
        SynthBass1,//39
        SynthBass2,//40

        //STRINGS
        Violin,//41
        Viola,//42
        Cello,//43
        Contrabass,//44
        TremoloStrings,//45
        PizzicatoStrings,//46
        OrchestralHarp,//47
        Timpani,//48

        //ENSEMBLE
        StringEnsemble1,//49
        StringEnsemble2,//50
        SynthStrings1,//51
        SynthStrings2,//52
        ChoirAahs,//53
        VoiceOohs,//54
        SynthCVoice,//55
        OrchestraHit,//56
        //BRASS
        Trumpet,//57
        Trombone,//58
        Tuba,//59
        MutedTrumpet,//60
        FrenchHorn,//61
        BrassSection,//62
        SynthBrass1,//63
        SynthBrass2,//64
        //REED
        SopranoSax,//65
        AltoSax,//66
        TenorSax,//67
        BaritoneSax,//68
        Oboe,//69
        EnglishHorn,//70
        Bassoon,//71
        Clarinet,//72

        //PIPE
        Piccolo,//73
        Flute,//74
        Recorder,//75
        PanFlute,//76
        BlownBottle,//77
        Shakuhachi,//78
        Whistle,//79
        Ocarina,//80
        //SYNTHLEAD
        Lead1Square,//81
        Lead2Sawtooth,//82
        Lead3Calliope,//83
        Lead4Chiff,//84
        Lead5Charang,//85
        Lead6Voice,//86
        Lead7Fifths,//87
        Lead8BassAndlead,//88
        //SYNTHPAD
        Pad1NewAge,//89
        Pad2Warm,//90
        Pad3Polysynth,//91
        Pad4Choir,//92
        Pad5Bowed,//93
        Pad6Metallic,//94
        Pad7Halo,//95
        Pad8Sweep,//96

        //SynthEffects,
        FX1Rain,//97
        FX2Soundtrack,//98
        FX3Crystal,//99
        FX4Atmosphere,//100
        FX5Brightness,//101
        FX6Goblins,//102
        FX7Echoes,//103
        FX8Scifi,//104
        //ETHNIC
        Sitar,//105
        Banjo,//106
        Shamisen,//107
        Koto,//108
        Kalimba,//109
        BagPipe,//110
        Fiddle,//111
        Shanai,//112
        //PERCUSSIVE
        TinkleBell,//113
        Agogo,//114
        SteelDrums,//115
        Woodblock,//116
        TaikoDrum,//117
        MelodicTom,//118
        SynthDrum,//119
        //SOUNDEFFECTS
        ReverseCymbal,//120
        GuitarFretNoise,//121
        BreathNoise,//122
        Seashore,//123
        BirdTweet,//124
        TelephoneRing,//125
        Helicopter,//126
        Applause,//127
        Gunshot//128
    }
    #endregion MIDIInstrument
}
