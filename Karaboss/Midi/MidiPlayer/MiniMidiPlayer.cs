using Sanford.Multimedia.Midi;
using System;
using System.Windows.Forms;

namespace Karaboss.Midi.MidiLyrics
{
    public class MiniMidiPlayer
    {
        private Sequence sequence1;
        private Sequencer sequencer1;
        private OutputDevice outDevice;

        private bool closing = false;

        private int _channel;
        public int Channel 
        { 
            get { return _channel; }
            set {

                sequencer1.AllSoundOff();
                _channel = value; 
                sequencer1.Continue();
            }

        }

        private Timer timer1;

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

        public MiniMidiPlayer(Sequence sequence, OutputDevice outDev, int channel)
        {
            sequence1 = sequence;
            outDevice = outDev;
            _channel = channel;

            LoadSequencer(sequence1);

            InitTimer();
        }


        #region timer
        private void InitTimer() 
        { 
            timer1 = new Timer(); 
            timer1.Interval = 100;
            timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (closing)
            {
                timer1?.Stop();
                return;
            }

            switch (PlayerState)
            {
                case PlayerStates.Playing:                    
                    break;

                case PlayerStates.Stopped:
                    timer1.Stop();                    
                    break;
            }
        }

        #endregion timer


        #region sequencer

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

                //UpdateMidiTimes();

                //TempoOrig = _tempo;
                //lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

                // Display
                //int Min = (int)(_duration / 60);
                //int Sec = (int)(_duration - (Min * 60));
                //lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ResetSequencer()
        {
            //newstart = 0;
            //laststart = 0;
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        #endregion sequencer


        #region handle messages

        /// <summary>
        /// Event: playing midi file completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            //newstart = 0;
            // Passe éventuellement au morceau suivant, sinon s'arrête
            PlayerState = PlayerStates.Stopped;
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            if (e.Message.MidiChannel == _channel)
            {
                // Play only messages coming from 1 channel: SingleTrackChannel
                outDevice.Send(e.Message);                
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

        #endregion handle messages


        #region play stop
        public void Play()
        {
            PlayerState = PlayerStates.Playing;
            timer1.Start();
            sequencer1.Start();

        }


        public void Stop()
        {
            PlayerState = PlayerStates.Stopped;
            sequencer1.Stop();            
        }
        #endregion play stop


        #region clean resources
        // Clean resources

        public void Dispose()
        {
            timer1.Dispose();
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();
        }

        #endregion clean resources


    }
}
