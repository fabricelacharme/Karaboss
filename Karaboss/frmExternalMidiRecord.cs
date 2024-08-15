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
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;

namespace Karaboss
{
    /*
     * Recording session
     * 
     * This windows allows to record an external instrument connected with a MIDI cable to Karaboss
     *  
     * The recording time starts with the first event received.
     * 
     * 
     * For the moment this allow to create a sequence and a single track having fixed time characteristics
     * Instrument: grand piano
     * numerator = 4;
     * denominator = 4;
     * BPM de 120
     * division = 960;
     * tempo = 750000;
     * 
     * TODO : 
     * - allow to parametrize times characteristics
     * - allow to select the instrument?
     * - allow to record a track within an existing sequence (play and record piano while hearing the other tracks)
     * 
     */

    public partial class frmExternalMidiRecord : Form
    {
        private const int SysExBufferSize = 128;
        private SynchronizationContext context;

        #region Controls
        private Sequencer sequencer1 = new Sequencer();
        private Sequence sequence1 = new Sequence();
        private Track track1 = new Track();
        private InputDevice inDevice;
        //private int inDeviceID = 0;
        private OutputDevice outDevice;
        #endregion


        #region private decl

        private bool scrolling = false;
        private bool closing = false;
        private int newstart = 0;
        private int nbstop = 0;
        //private int laststart = 0;      // Start time to play
        float TimeToTicksConversion = 0;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        #endregion


        #region midi values

        private int _bpm = 0;           // 60000000 / _tempo
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _tempo = 0;
        private int _division = 0;      // ticks per beat - ex 96
        private int _ppqn = 0;          // idem _division (sequence1.Division)
        private int _measurelen = 0;
        private int _TempoOrig = 0;
        //private int _TempoDelta = 100;
        #endregion


        #region Time counter

        private bool bFirstEventReceived = false;
        DateTime _StartTime;

        #endregion


        #region player
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


        #region recorder

        private enum RecorderStates
        {
            Recording,
            Saving,
            Stopped,
        }
        private RecorderStates RecorderState;

        #endregion
        

        public frmExternalMidiRecord(InputDevice inputdevice, OutputDevice outputdevice)
        {
            InitializeComponent();

            ResizeObjects();
            stopButton.Enabled = false;
            btnSave.Enabled = false;

            context = SynchronizationContext.Current;

            // Input Midi device
            inDevice = inputdevice;            
            if (!InitInputDevice())
            {
                startButton.Enabled = false;
                stopButton.Enabled = false;
            }

            // player
            PlayerState = PlayerStates.Stopped;
            timerPlayer.Interval = 20;

            RecorderState = RecorderStates.Stopped;
            timerRecorder.Interval = 100;

            // Output Midi Device
            outDevice = outputdevice;
            InitOutputDevice();
        }

        #region initializations

        /// <summary>
        /// Initialize input Midi device
        /// </summary>
        private bool InitInputDevice()
        {
            //inDeviceID = 0;

            if (InputDevice.DeviceCount == 0)
                return false;

            try
            {
                //inDevice = new InputDevice(inDeviceID);

                inDevice.ChannelMessageReceived += HandleChannelMessageReceived;
                inDevice.SysCommonMessageReceived += HandleSysCommonMessageReceived;
                inDevice.SysExMessageReceived += HandleSysExMessageReceived;
                inDevice.SysRealtimeMessageReceived += HandleSysRealtimeMessageReceived;
                inDevice.Error += new EventHandler<Sanford.Multimedia.ErrorEventArgs>(inDevice_Error);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the MIDI input device", "Karaboss\n" + ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //Close();
                return false;
            }
        }

        private void InitOutputDevice()
        {            
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 60, 80));
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 60, 80));
        }

        /// <summary>
        /// Create a new sequence
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <param name="division"></param>
        /// <param name="tempo"></param>
        private Sequence CreateSequence(int numerator, int denominator, int division, int tempo)
        {
            Sequence seq = new Sequence();
            seq = new Sequence(division)
            {
                Format = 1,
                Numerator = numerator,
                Denominator = denominator,
                Tempo = tempo,
                Time = new TimeSignature(numerator, denominator, division, tempo),
            };
            return seq;
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
        /// <param name="clef"></param>
        /// <returns></returns>
        private Track CreateTrack(string trackname, string instrumentname, int channel, int programchange, int volume, int tempo, TimeSignature timesig, Clef clef)
        {
            // Add tack to sequence
            Track track = new Track()
            {
                MidiChannel = channel,
                Name = trackname,
                InstrumentName = instrumentname,
                ProgramChange = programchange,
                Volume = volume,
                Pan = 64,
                Reverb = 64,
            };


            track.Clef = clef;

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

            return track;
        }        

        private void ResetMidi()
        {            
            // Add sequence
            int numerator = 4;
            int denominator = 4;

            // BPM de 120
            _division = 960;
            _ppqn = _division;
            _tempo = 750000;
            sequence1 = CreateSequence(numerator, denominator, _division, _tempo);

            TimeToTicksConversion = 1000 * ((float)_division / (float)_tempo);

            // Add track
            string trackName = "track1";
            string instrumentName = "AcousticGrandPiano";
            int channel = 0;
            int programChange = 0;
            int volume = 100;
            TimeSignature timesig = sequence1.Time;
            Clef clef = Clef.Treble;
            track1 = CreateTrack(trackName, instrumentName, channel, programChange, volume, _tempo, timesig, clef);

            sequence1.Add(track1);         
        }

        private void LoadSequencer()
        {
            try
            {
                sequencer1 = new Sequencer
                {
                    Sequence = sequence1    // primordial !!!!!
                };

                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                UpdateMidiTimes();

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
        /// Initialize sequencer
        /// </summary>
        private void ResetSequencer()
        {
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        /// <summary>
        /// Upadate midi times
        /// </summary>
        private void UpdateMidiTimes()
        {
            //sequence1.ExtractNotes();

            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _TempoOrig = _tempo;
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
            // see http://midi.teragonaudio.com/tech/midifile/ppqn.htm
            const float kOneMinuteInMicroseconds = 60000000;
            float kTimeSignatureNumerator = (float)sequence1.Numerator;
            float kTimeSignatureDenominator = (float)sequence1.Denominator;

            //float BPM = (kOneMinuteInMicroseconds / (float)tempo) * (kTimeSignatureDenominator / 4.0f);            
            float BPM = kOneMinuteInMicroseconds / (float)tempo;

            return (int)BPM;
        }


        private void DrawControls()
        {
            try
            {
                #region top                         

                int lastenoteticks = sequence1.GetLength(); //sequence1.GetLastNoteEndTime();
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
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        #endregion


        #region Form load close

        private void frmExternalMidiRecord_Resize(object sender, EventArgs e)
        {
            ResizeObjects();
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

            if (inDevice != null)
            {
                inDevice.Close();
            }
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();
            base.OnClosed(e);

            base.OnClosed(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {            
            if (keyData == Keys.Space)
            {
                if (RecorderState == RecorderStates.Stopped)
                    StartRecording();
                else if (RecorderState == RecorderStates.Recording)
                    StopRecording();
                return true;
            }            
            return base.ProcessCmdKey(ref msg, keyData);
        }

       

        #endregion


        #region Display objects

        private void ResizeObjects()
        {
            positionHScrollBar.Width = pnlTop.Width - positionHScrollBar.Left - 40;
        }

        #endregion


        #region buttons record stop

        /// <summary>
        /// Start recording MIDI data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            StartRecording();
            startButton.Parent.Focus();
        }       

        /// <summary>
        /// Stop record MIDI data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            StopRecording();
            stopButton.Parent.Focus();
        }

        /// <summary>
        /// Start recording session
        /// </summary>
        private void StartRecording()
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped)
            {
                MessageBox.Show("Please stop the player before", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            #endregion

            // Reset all
            InitRecordTasks();

            RecorderState = RecorderStates.Recording;
            timerRecorder.Start();

            try
            {
                inDevice.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Stop recording session
        /// </summary>
        private void StopRecording()
        {
            #region guard
            if (RecorderState != RecorderStates.Recording)
                return;
            #endregion

            try
            {
                inDevice.StopRecording();
                inDevice.Reset();

                // Tell timerRecorder to update and display sequence characteristics
                RecorderState = RecorderStates.Saving;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Start recording: reset all
        /// </summary>
        private void InitRecordTasks()
        {
            startButton.Text = "Recording";
            startButton.Enabled = false;
            stopButton.Enabled = true;
            btnSave.Enabled = false;
            channelListBox.Items.Clear();
            sysExRichTextBox.Clear();
            sysCommonListBox.Items.Clear();
            sysRealtimeListBox.Items.Clear();

            
            // Reset sequence
            ResetMidi();

            bFirstEventReceived = false;
            _StartTime = DateTime.Now;
            channelListBox.Items.Clear();
        }

        /// <summary>
        /// End record operations
        /// </summary>
        private void EndRecordTasks()
        {
            startButton.Text = "Start recording";
            startButton.Enabled = true;
            stopButton.Enabled = false;
            btnSave.Enabled = true;

            LoadSequencer();
            DrawControls();
           
            RecorderState = RecorderStates.Stopped;
        }


        #endregion


        #region errors handling

        private void inDevice_Error(object sender,  Sanford.Multimedia.ErrorEventArgs e)
        {
            MessageBox.Show(e.Error.Message, "Error!",
                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        #endregion


        #region Handle messages

        #region Save file

        /// <summary>
        /// Save MIDI file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message);            
            
        }

        // Save MIDI file in progress
        private void HandleSaveProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region messages received from remote Midi controler

        private int GetTicks()
        {
            // Start recording time at first event received
            if (bFirstEventReceived == false)
            {
                bFirstEventReceived = true;
                _StartTime = DateTime.Now;
            }

            TimeSpan DeltaTime = DateTime.Now - _StartTime;
            double deltatime = DeltaTime.TotalMilliseconds;
            // convert to ticks                
            return (int)(deltatime * TimeToTicksConversion);
        }


        /// <summary>
        /// Channel messages (notes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {                
                int ticks = GetTicks();

                // Display notes
                channelListBox.Items.Add(
                    e.Message.Command.ToString() + '\t' + '\t' +
                    e.Message.MidiChannel.ToString() + '\t' +
                    e.Message.Data1.ToString() + '\t' +
                    e.Message.Data2.ToString() + '\t' +
                    ticks.ToString()
                    );

                channelListBox.SelectedIndex = channelListBox.Items.Count - 1;

                // Insert event
                ChannelMessage ChMsg = new ChannelMessage(e.Message.Command, e.Message.MidiChannel, e.Message.Data1, e.Message.Data2);
                track1.Insert(ticks, ChMsg);                             

            }, null);
        }

        /// <summary>
        /// Sysex messages received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSysExMessageReceived(object sender, SysExMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                int ticks = GetTicks();

                string result = "\n\n"; ;
                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }

                sysExRichTextBox.Text += result;

                // insert event                               
                SysExMessage SMsg = new SysExMessage(e.Message.GetBytes());
                track1.Insert(ticks, SMsg);                

            }, null);
        }

        /// <summary>
        /// SysCommon messages received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSysCommonMessageReceived(object sender, SysCommonMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                int ticks = GetTicks();

                sysCommonListBox.Items.Add(
                    e.Message.SysCommonType.ToString() + '\t' + '\t' +
                    e.Message.Data1.ToString() + '\t' +
                    e.Message.Data2.ToString());

                sysCommonListBox.SelectedIndex = sysCommonListBox.Items.Count - 1;

                // insert event                
                SysCommonMessage CMsg = new SysCommonMessage(e.Message.SysCommonType);
                track1.Insert(ticks, CMsg);                

            }, null);
        }

        /// <summary>
        /// Real time messages received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSysRealtimeMessageReceived(object sender, SysRealtimeMessageEventArgs e)
        {
            context.Post(delegate (object dummy)
            {
                sysRealtimeListBox.Items.Add(
                    e.Message.SysRealtimeType.ToString());

                sysRealtimeListBox.SelectedIndex = sysRealtimeListBox.Items.Count - 1;               

            }, null);
        }

        #endregion

        #region sequencer play

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
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

        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if (closing)
            {
                return;
            }
            outDevice.Send(e.Message);
            
        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            PlayerState = PlayerStates.Stopped;
        }

        #endregion

        #endregion


        #region notes
        /*
        /// <summary>
        /// A NoteOff event occured.  Find the MidiNote of the corresponding
        /// NoteOn event, and update the duration of the MidiNote.
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="noteNumber"></param>
        /// <param name="endTime"></param>
        private void NoteOff(int Channel, int noteNumber, int endTime)
        {
            for (int i = listNotes.Count - 1; i >= 0; i--)
            {
                MidiNote note = listNotes[i];
                if (note.Channel == Channel && note.Number == noteNumber && note.Duration == 0)
                {
                    note.NoteOff(endTime);
                    break;
                }
            }
        }
        */
        #endregion


        #region position hscrollbar

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

        #endregion


        #region play pause stop

        /// <summary>
        /// Play recordeed song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            #region guard
            if (RecorderState != RecorderStates.Stopped)
                return;
            #endregion
            
            PlayPauseMusic();
            BtnPlay.Parent.Focus();
        }

        /// <summary>
        /// Stop playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStop_Click(object sender, EventArgs e)
        {
            #region guard
            if (PlayerState != PlayerStates.Stopped && RecorderState != RecorderStates.Stopped)
                return;
            #endregion

            StopMusic();
            BtnStop.Parent.Focus();
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
                positionHScrollBar.Value = positionHScrollBar.Minimum;                
                //laststart = 0;
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
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
                    timerPlayer.Start();
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

                // bizarre, aucun son, si on ne fait pas ça
                outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, track1.MidiChannel, 60, 80));
                outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, track1.MidiChannel, 60, 80));

                PlayerState = PlayerStates.Playing;
                nbstop = 0;
                BtnStatus();
                sequencer1.Start();

                if (ticks > 0)
                    sequencer1.Position = ticks;

                timerPlayer.Start();
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

        #endregion


        #region timer

        /// <summary>
        /// Timer for record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerRecorder_Tick(object sender, EventArgs e)
        {
            switch (RecorderState)
            {
                case RecorderStates.Recording:
                    break;

                case RecorderStates.Saving:
                    timerRecorder.Stop();
                    EndRecordTasks();
                    break;

                case RecorderStates.Stopped:
                    timerRecorder.Stop();
                    break;
            }
        }

        /// <summary>
        /// Timer for playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerPlayer_Tick(object sender, EventArgs e)
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
                        timerPlayer.Stop();
                        AfterStopped();
                        break;

                    case PlayerStates.Paused:
                        sequencer1.Stop();
                        timerPlayer.Stop();
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



        #endregion


        #region Save File

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileProc();
            btnSave.Parent.Focus();
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
            //progressBarPlayer.Visible = true;

            sequence1.SaveProgressChanged += HandleSaveProgressChanged;
            sequence1.SaveCompleted += HandleSaveCompleted;

            //addTags();

            SaveFile(fileName);
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



        #endregion


        #region Tempo

        private void btnTempoPlus_Click(object sender, EventArgs e)
        {
            Parent.Focus();
        }

        private void btnTempoMinus_Click(object sender, EventArgs e)
        {
            Parent.Focus();
        }

        #endregion

    }

}
