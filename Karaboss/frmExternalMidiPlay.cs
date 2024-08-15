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
using System.Threading;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace Karaboss
{

    /*
    * Playing session
    * 
    * This windows allows to play an external instrument connected with a MIDI cable to Karaboss
    *      .        
    * For the moment this allow to select the instrument to be played independantly from the connected instrument
    * 
    * Main problems encountered: 
    * - Latency: sounds are heard a fraction of second after having hit a key.
    * 
    */

    public partial class frmExternalMidiPlay : Form
    {
        private const int SysExBufferSize = 128;

        private SynchronizationContext context;

        #region Controls       
        private InputDevice inDevice;
        //private int inDeviceID = 0;
        private OutputDevice outDevice;
        #endregion


        #region private decl
        
        //private bool scrolling = false;
        private bool closing = false;
        //private int newstart = 0;
        //private int nbstop = 0;
        //private int laststart = 0;      // Start time to play
        //float TimeToTicksConversion = 0;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        #region instruments
        private int _patch = 0;
        private static List<string> LoadInstruments()
        {
            List<string> list1 = new List<string>()
            {
        "AcousticGrandPiano",
        "BrightAcousticPiano",
        "ElectricGrandPiano",
        "HonkyTonkPiano",
        "ElectricPiano1",
        "ElectricPiano2",
        "Harpsichord",
        "Clavinet",
        "Celesta",
        "Glockenspiel",
        "MusicBox",
        "Vibraphone",
        "Marimba",
        "Xylophone",
        "TubularBells",
        "Dulcimer",
        "DrawbarOrgan",
        "PercussiveOrgan",
        "RockOrgan",
        "ChurchOrgan",
        "ReedOrgan",
        "Accordion",
        "Harmonica",
        "TangoAccordion",
        "AcousticGuitarNylon",
        "AcousticGuitarSteel",
        "ElectricGuitarJazz",
        "ElectricGuitarClean",
        "ElectricGuitarMuted",
        "OverdrivenGuitar",
        "DistortionGuitar",
        "GuitarHarmonics",
        "AcousticBass",
        "ElectricBassFinger",
        "ElectricBassPick",
        "FretlessBass",
        "SlapBass1",
        "SlapBass2",
        "SynthBass1",
        "SynthBass2",
        "Violin",
        "Viola",
        "Cello",
        "Contrabass",
        "TremoloStrings",
        "PizzicatoStrings",
        "OrchestralHarp",
        "Timpani",
        "StringEnsemble1",
        "StringEnsemble2",
        "SynthStrings1",
        "SynthStrings2",
        "ChoirAahs",
        "VoiceOohs",
        "SynthVoice",
        "OrchestraHit",
        "Trumpet",
        "Trombone",
        "Tuba",
        "MutedTrumpet",
        "FrenchHorn",
        "BrassSection",
        "SynthBrass1",
        "SynthBrass2",
        "SopranoSax",
        "AltoSax",
        "TenorSax",
        "BaritoneSax",
        "Oboe",
        "EnglishHorn",
        "Bassoon",
        "Clarinet",
        "Piccolo",
        "Flute",
        "Recorder",
        "PanFlute",
        "BlownBottle",
        "Shakuhachi",
        "Whistle",
        "Ocarina",
        "Lead1Square",
        "Lead2Sawtooth",
        "Lead3Calliope",
        "Lead4Chiff",
        "Lead5Charang",
        "Lead6Voice",
        "Lead7Fifths",
        "Lead8BassAndLead",
        "Pad1NewAge",
        "Pad2Warm",
        "Pad3Polysynth",
        "Pad4Choir",
        "Pad5Bowed",
        "Pad6Metallic",
        "Pad7Halo",
        "Pad8Sweep",
        "Fx1Rain",
        "Fx2Soundtrack",
        "Fx3Crystal",
        "Fx4Atmosphere",
        "Fx5Brightness",
        "Fx6Goblins",
        "Fx7Echoes",
        "Fx8SciFi",
        "Sitar",
        "Banjo",
        "Shamisen",
        "Koto",
        "Kalimba",
        "BagPipe",
        "Fiddle",
        "Shanai",
        "TinkleBell",
        "Agogo",
        "SteelDrums",
        "Woodblock",
        "TaikoDrum",
        "MelodicTom",
        "SynthDrum",
        "ReverseCymbal",
        "GuitarFretNoise",
        "BreathNoise",
        "Seashore",
        "BirdTweet",
        "TelephoneRing",
        "Helicopter",
        "Applause",
        "Gunshot"};
            return list1;
        }
        #endregion

        #endregion

        #region midi values

        //private int _bpm = 0;           // 60000000 / _tempo
        //private double _duration = 0;  // en secondes
        //private int _totalTicks = 0;
        //private int _tempo = 0;
        //private int _division = 0;      // ticks per beat - ex 96
        //private int _ppqn = 0;          // idem _division (sequence1.Division)
        //private int _measurelen = 0;
        //private int _TempoOrig = 0;
        //private int _TempoDelta = 100;
        #endregion


        #region Time counter
        //float _maintenant = 0;
        //float _startTime = 0;
        //DateTime _StartTime;

        #endregion
      

        #region Receiver status

        private enum ReceiverStates
        {
            Receiving,   
            Stopping,
            Stopped,
        }
        private ReceiverStates ReceiverState;

        #endregion
       
        public frmExternalMidiPlay(InputDevice inputdevice ,OutputDevice outputdevice)
        {
            InitializeComponent();

            stopButton.Enabled = false;
            startButton.Enabled = true;

            outDevice = outputdevice;
            inDevice = inputdevice;

            context = SynchronizationContext.Current;

            // Input Midi device
            if (!InitInputDevice())
            {
                startButton.Enabled = false;
                stopButton.Enabled = false;
            }

            // Init patch & Channel of Output device
            InitOutputDevice();

            InitlstInstruments();

            ReceiverState = ReceiverStates.Stopped;
            timerReceiver.Interval = 100;            
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
                return false;
            }
        }

        private void InitOutputDevice()
        {
            //_patch = 56;// 16;
            SelectInstrument(_patch, 0);

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, 60, 80));
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, 60, 80));
        }


        private void InitlstInstruments()
        {
            List<string> lsI = LoadInstruments();
            cbInstruments.Font = new Font("Segoe UI", 9F);
            cbInstruments.TabStop = true;

            //lstInstruments.Items.Add("");
            for (int i = 0; i < lsI.Count; i++)
            {
                cbInstruments.Items.Add(lsI[i]);
            }

            cbInstruments.SelectedIndex = 0;
        }

        private void SelectInstrument(int patch, int MidiChannel)
        {
            // _patch 16 = DrawbarOrgan
            outDevice.Send(new ChannelMessage(ChannelCommand.ProgramChange, MidiChannel, patch, 0));
        }

        #endregion


        #region Form load close        

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {            
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
                if (ReceiverState == ReceiverStates.Stopped)
                    StartReceiving();
                else if (ReceiverState == ReceiverStates.Receiving)
                    StopReceiving();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        #endregion


        #region buttons record stop

        /// <summary>
        /// Start Receiving MIDI data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            StartReceiving();
            startButton.Parent.Focus();
        }


        private void StartReceiving()
        {
            #region guard
            if (ReceiverState != ReceiverStates.Stopped)
                return;
            #endregion


            // Reset all
            InitReceiveTasks();

            ReceiverState = ReceiverStates.Receiving;
            timerReceiver.Start();

            try
            {
                inDevice.StartRecording();
            }
            catch (Exception ex)
            {
                ReceiverState = ReceiverStates.Stopped;
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Stop record MIDI data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            StopReceiving();
            stopButton.Parent.Focus();
        }

        private void StopReceiving()
        {
            #region guard
            if (ReceiverState != ReceiverStates.Receiving)
                return;
            #endregion

            try
            {                
                inDevice.StopRecording();
                inDevice.Reset();

                // tells timer receiver to stop all
                ReceiverState = ReceiverStates.Stopping;
            }
            catch (Exception ex)
            {
                ReceiverState = ReceiverStates.Stopped;
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Start recording: reset all
        /// </summary>
        private void InitReceiveTasks()
        {
            startButton.Text = "Receiving";
            startButton.Enabled = false;
            stopButton.Enabled = true;
            
            channelListBox.Items.Clear();
            sysExRichTextBox.Clear();
            sysCommonListBox.Items.Clear();
            sysRealtimeListBox.Items.Clear();
        }

        /// <summary>
        /// Tasks to perform when user stop playing
        /// </summary>
        private void EndReceiveTasks()
        {
            startButton.Text = "Start receiving";
            startButton.Enabled = true;
            stopButton.Enabled = false;
            ReceiverState = ReceiverStates.Stopped;
        }

        #endregion


        #region errors handling

        private void inDevice_Error(object sender, Sanford.Multimedia.ErrorEventArgs e)
        {
            MessageBox.Show(e.Error.Message, "Error!",
                   MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        #endregion


        #region Handle messages

        #region messages received from remote Midi controler

        /// <summary>
        /// Channel messages (notes)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            if (closing)
            {
                return;
            }
            /*
            context.Post(delegate (object dummy)
            {
                
                // Display notes
                channelListBox.Items.Add(
                    e.Message.Command.ToString() + '\t' + '\t' +
                    e.Message.MidiChannel.ToString() + '\t' +
                    e.Message.Data1.ToString() + '\t' +
                    e.Message.Data2.ToString()                    
                    );

                channelListBox.SelectedIndex = channelListBox.Items.Count - 1;

                outDevice.Send(e.Message);
                
            }, null);
            */
            outDevice.Send(e.Message);
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
                string result = "\n\n"; ;

                foreach (byte b in e.Message)
                {
                    result += string.Format("{0:X2} ", b);
                }
                sysExRichTextBox.Text += result;              

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
                sysCommonListBox.Items.Add(
                    e.Message.SysCommonType.ToString() + '\t' + '\t' +
                    e.Message.Data1.ToString() + '\t' +
                    e.Message.Data2.ToString());

                sysCommonListBox.SelectedIndex = sysCommonListBox.Items.Count - 1;               

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

        #endregion


        #region cbInstruments

        private void cbInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {
            _patch = cbInstruments.SelectedIndex;

            InitOutputDevice();

            cbInstruments.Parent.Focus();
        }


        #endregion


        #region timer receiver          

        private void timerReceiver_Tick(object sender, EventArgs e)
        {
            switch (ReceiverState)
            {
                case ReceiverStates.Receiving:
                    break;
                case ReceiverStates.Stopping:
                    timerReceiver.Stop();
                    EndReceiveTasks();
                    break;
                case ReceiverStates.Stopped:
                    timerReceiver.Stop();
                    break;
            }
        }

        #endregion
    }

}
