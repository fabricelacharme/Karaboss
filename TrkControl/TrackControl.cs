#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.Drawing;
using System.Windows.Forms;

namespace TrkControl
{



    public partial class TrackControl : UserControl
    {
        
        #region Declare Delegate

        // Control click
        public delegate void TrackControlClickEventHandler(object sender);
        public event TrackControlClickEventHandler OntrkControlClick;

        // Minimize or Maximize track
        public delegate void btnMaximizeClickedEventHandler(object sender, EventArgs e, bool bmaximized);
        public event btnMaximizeClickedEventHandler OntrkControlbtnMaximizeClicked;

        // Mute channel button
        public delegate void btnMutClickedEventHandler(object sender, EventArgs e, int patch);
        public event btnMutClickedEventHandler OntrkControlbtnMutClicked;

        public delegate void btnSoloClickedEventHandler(object sender, EventArgs e, int patch);
        public event btnSoloClickedEventHandler OntrkControlbtnSoloClicked;

        // Delete track button
        public delegate void btnDelClickedEventHandler(object sender, EventArgs e, int track);
        public event btnDelClickedEventHandler OntrkControlbtnDelClicked;

        // Label change patch (instrument)
        public delegate void lblPatchChangedEventHandler(object sender, EventArgs e, int patch);
        public event lblPatchChangedEventHandler OntrkControllblPatchChanged;

        // change channel
        public delegate void lblChannelChangedEventHandler(object sender, EventArgs e, int midichannel);
        public event lblChannelChangedEventHandler OntrkControllblChannelChanged;
        
        // Knob volume
        public delegate void knobVolumeValueChangedEventHandler(object sender);
        public event knobVolumeValueChangedEventHandler OnknobControlknobVolumeValueChanged;

        // Knob Pan
        public delegate void knobPanValueChangedEventHandler(object sender);
        public event knobPanValueChangedEventHandler OnknobControlknobPanValueChanged;

        // Knob Reverb
        public delegate void knobReverbValueChangedEventHandler(object sender);
        public event knobReverbValueChangedEventHandler OnknobControlknobReverbValueChanged;

        // change Track name
        public delegate void lblTrackNameChangedEventHandler(object sender, EventArgs e, string trackname);
        public event lblTrackNameChangedEventHandler OntrkControllblTrackNameChanged;


        public delegate void btnPianoRollClickedEventHandler(object sender, EventArgs e, int track);
        public event btnPianoRollClickedEventHandler OntrkControlbtnPianoRollClicked;

        #endregion Declare Delegate

        private TextBox editbox = new TextBox();
        bool doEdit = false;


        #region properties
        private Color ColorEditOn = ColorTranslator.FromHtml("#2d89ef");  // Color.LightSteelBlue;  //#00aba9
        private Color ColorEditOff = ColorTranslator.FromHtml("#2b5797"); //Color.DimGray;     //#2b5797
        private Color ColorMutedOn = ColorTranslator.FromHtml("#ee1111");  //Color.Red;         // #ee1111
        private Color ColorMutedOff = ColorTranslator.FromHtml("#2d89ef"); //Color.RoyalBlue;   // #2d89ef
        private Color ColorSoloOn = ColorTranslator.FromHtml("#ffc40d"); //Color.Yellow;        //#00a300
        private Color ColorSoloOff = ColorTranslator.FromHtml("#00a300"); //Color.Green;        // #99b433
        private Color ColorLightOn = ColorTranslator.FromHtml("#ee1111"); //Color.Red; 


        private bool bmaximized = true;
        public bool bMaximized
        {
            get { return bmaximized; }
            set 
            {
                bmaximized = value;
                pnlBottom.Visible = bmaximized;
                if (bmaximized)
                {
                    btnMaximized.Text = "-";                    
                    this.Height = 148;
                }
                else
                {
                    btnMaximized.Text = "+";
                    this.Height = 23;
                }

            }
        }


        private bool muted;
        public bool Muted {
            get
            { return muted; }
            set
            {
                muted = value;
                if (muted)
                { btnMut.BackColor = ColorMutedOn; } //Color.Red
                else
                { btnMut.BackColor = ColorMutedOff; } //Color.RoyalBlue

            }
        }

        private bool solo;
        public bool Solo
        {
            get { return solo; }
            set {
                solo = value;
                if (solo)
                {
                    btnSolo.BackColor = ColorSoloOn;  //Color.Yellow;
                    btnSolo.ForeColor = Color.Black;
                }
                else
                {
                    btnSolo.BackColor = ColorSoloOff; //Color.Green;
                    btnSolo.ForeColor = Color.White;
                }

            }
        }

        private string trackname;
        public string TrackName
        {
            get { return trackname; }
            set { trackname = value;
                this.lblTrackName.Text = value;
            }
        }

        private string tracklabel;
        public string TrackLabel
        {
            get { return tracklabel; }
            set
            {
                tracklabel = value;
                this.lblTrackLabel.Text = value;
            }
        }

        private int midichannel;
        public int MidiChannel
        {
            get { return midichannel; }
            set { midichannel = value;
                this.lblChannel.Text = value.ToString();
            }
        }

        private static List<int> LoadChannels()
        {
            List<int> list1 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            return list1;
        }

        private int patch;
        public int Patch
        {
            get { return patch; }
            set
            {
                patch = value;
                //lblProgram.Text = value.ToString();
                //lblTrackLabel.Text = lslIntrumentNames[patch];
            }
        }

        private int volume;
        public int Volume
        {
            get
            { return volume; }
            set
            {
                if (value >= knobVolume.Minimum && value <= knobVolume.Maximum)
                {
                    volume = value;
                    this.knobVolume.Value = volume;
                }
            }
        }

        private int pan;
        public int Pan
        {
            get
            { return pan; }
            set
            {
                if (value >= knobPan.Minimum && value <= knobPan.Maximum)
                {
                    pan = value;
                    this.knobPan.Value = pan;
                }
            }
        }

        private int reverb;
        public int Reverb
        {
            get
            { return reverb; }
            set
            {
                if (value >= knobReverb.Minimum && value <= knobReverb.Maximum)
                {
                    reverb = value;
                    this.knobReverb.Value = reverb;
                }
            }
        }


        private int track;
        public int Track
        {
            get
            { return track; }
            set
            {
                track = value;
                this.lblTrack.Text = value.ToString();
            }
        }

        private int yorg;
        public int yOrg
        {
            get
            {
                return yorg;
            }
            set { yorg = value; }
        }


        private bool selected = false;
        public bool Selected
        {
            get { return selected; }
            set { selected = value;
                if (selected)
                {
                    pnlBottom.BackColor = ColorEditOn; //Color.LightSteelBlue;

                }
                else
                {
                    pnlBottom.BackColor = ColorEditOff; //Color.DimGray;
                }
            }
        }

        private bool _enabled = false;
        public new bool Enabled
        {
            get {return _enabled;}
            set { _enabled = value; }
        }

        #endregion properties
        

        #region instruments list
        private List<string> lslIntrumentNames = new List<string>()
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


        private static string instrument;
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

        #endregion instruments list


        public TrackControl()
        {
            InitializeComponent();

            // Edit box
            #region editbox
            editbox.Parent = this;
            editbox.Hide();
            doEdit = false;
            editbox.LostFocus += new EventHandler(editbox_LostFocus);
            editbox.KeyDown += new KeyEventHandler(editbox_KeyDown);
            editbox.KeyPress += new KeyPressEventHandler(editbox_KeyPress);
            editbox.VisibleChanged += new EventHandler(editbox_VisibleChanged);
            #endregion

            #region volume, pan, reverb
            // Volume           
            knobVolume.Minimum = 0;
            knobVolume.Maximum = 127;
            knobVolume.LargeChange = 8;
            knobVolume.SmallChange = 4;            
            knobVolume.MouseWheelBarPartitions = knobVolume.Maximum;

            // Pan
            knobPan.Minimum = 0;
            knobPan.Maximum = 127;
            knobPan.LargeChange = 8;
            knobPan.SmallChange = 4;            
            knobPan.MouseWheelBarPartitions = knobPan.Maximum;

            // Reverb
            knobReverb.Minimum = 0;
            knobReverb.Maximum = 127;
            knobReverb.LargeChange = 8;
            knobReverb.SmallChange = 4;            
            knobReverb.MouseWheelBarPartitions = knobReverb.Maximum;
            #endregion

            InitlstInstruments();
            InitlstChannels();

            CtlAllowDrop();
        }

        #region Drag Drop
       
        private void CtlAllowDrop()
        {
            this.AllowDrop = true;

            foreach (Control clt in Controls)
            {
                clt.AllowDrop = true;
            }
        }

        #endregion


        /// <summary>
        /// List of instruments
        /// </summary>
        private void InitlstInstruments()
        {
            List<string> lsI = LoadInstruments();
            lstInstruments.Name = "lstInstruments";
            lstInstruments.Location = new Point(this.Location.X + lblChannel.Location.X, this.Location.Y + this.btnInstrument.Location.Y + this.btnInstrument.Height);
            lstInstruments.Size = new Size(150, 200);
            lstInstruments.Font = new Font("Segoe UI", 9F);

            lstInstruments.TabStop = true;

            //lstInstruments.Items.Add("");
            for (int i = 0; i < lsI.Count; i++)
            {
                lstInstruments.Items.Add(lsI[i]);
            }
        }

        private void InitlstChannels()
        {
            List<int> lsC = LoadChannels();
            lstChannels.Name = "lstChannels";
            lstChannels.Location = new Point(0, this.Location.Y + this.lblChannel.Location.Y + this.lblChannel.Height);
            lstChannels.Size = new Size(50, 200);
            lstChannels.Font = new Font("Segoe UI", 9F);

            lstChannels.TabStop = true;
            
            for (int i = 0; i < lsC.Count; i++)
            {
                lstChannels.Items.Add(lsC[i]);
            }

            
        }

        /// <summary>
        /// lignt on channel
        /// </summary>
        public void LightOn()
        {
            if (!this.Muted)
                lblLight.BackColor = ColorLightOn; //Color.Red;
        }


        public void SetVolume(int vol)
        {            
            this.knobVolume.SetValue(vol);
            lblVolume.Text = String.Format("{0}%", 100 * vol / knobVolume.Maximum);
        }

        public void SetPan(int pan)
        {
            this.knobPan.SetValue(pan);
            lblPan.Text = String.Format("{0}%", 100 * (pan - 64) / knobPan.Maximum);
        }
        public void SetReverb(int reverb)
        {
            this.knobReverb.SetValue(reverb);
            lblReverb.Text = String.Format("{0}%", 100 * reverb / knobReverb.Maximum);
        }

        public void  SetPatch(int p)
        {
            // Only matter of display, don't change the value
            lblProgram.Text = p.ToString();
            lblTrackLabel.Text = lslIntrumentNames[p];
        }

        /// <summary>
        /// light off channel
        /// </summary>
        public void LightOff()
        {
            lblLight.BackColor = Color.Gray;
        }
      

        #region buttons solo mute, del

        private void btnSolo_Click(object sender, EventArgs e)
        {
            // Delegate the event to the caller
            OntrkControlbtnSoloClicked?.Invoke(this, e, patch);
        }

        /// <summary>
        /// Mute channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMut_Click(object sender, EventArgs e)
        {
            // Delegate the event to the caller
            OntrkControlbtnMutClicked?.Invoke(this, e, patch);
        }

        /// <summary>
        /// Delete this control and associated track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            #region guard
            // Allowed only if edit mode set on frmPlayer
            if (!_enabled)
                return;
            #endregion

            string message = "Do you want to delete track number " + track + "?";
            string caption = "Karaboss";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.
            result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.Yes)                            
                OntrkControlbtnDelClicked?.Invoke(this, e, track);
        }

        /// <summary>
        /// Display or hide pianoroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPianoRoll_Click(object sender, EventArgs e)
        {
            OntrkControlbtnPianoRollClicked?.Invoke(this, e, track);
        }

        #endregion


        #region instrument

        /// <summary>
        /// Select another instrument
        /// </summary>
        /// <param name="sebder"></param>
        /// <param name="e"></param>
        private void btnInstrument_Click(object sender, EventArgs e)
        {
            #region guard
            // Allow change instrument while playing
           
            #endregion

            // If aleady visible, hide it
            if (lstInstruments.Visible)
            {
                lstInstruments.Hide();
                return;
            }

            // Else, hide others
            for (int i = 0; i < Parent.Controls.Count; i++)
            {
                if (Parent.Controls[i].GetType() == typeof(ListBox))
                {
                    if (Parent.Controls[i].Name == "lstInstruments")
                    {
                        Parent.Controls[i].Hide();
                        break;
                    }
                }
            }
            // And make its own visible
            DisplaylstInstruments();
        }


        /// <summary>
        /// Display the Listbox to select a new patch
        /// </summary>
        private void DisplaylstInstruments()
        {
            
            lstInstruments.Location = new Point(0, this.Location.Y + this.btnInstrument.Location.Y + this.btnInstrument.Height);
            lstInstruments.SelectedItem = lblTrackLabel.Text;

            Parent.Controls.Add(lstInstruments);
            lstInstruments.Visible = true;
            lstInstruments.BringToFront();
            lstInstruments.Focus();

            lstInstruments.Click += new EventHandler(lstInstruments_Click);
            lstInstruments.KeyDown += new KeyEventHandler(lstInstruments_KeyDown);


        }

        /// <summary>
        /// Event click on Listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstInstruments_Click(object sender, EventArgs e)
        {           
            SelectInstrument();
        }

        /// <summary>
        /// Populate labels with new patch
        /// </summary>
        private void SelectInstrument()
        {
            string ret = string.Empty;
            ret = lstInstruments.Items[lstInstruments.SelectedIndex].ToString();

            if (ret != "")
            {
                instrument = ret;
                lblTrackLabel.Text = instrument;
                
                // Fab pour correction patch = -1 12/04/2014
                if (lstInstruments.SelectedIndex != -1)
                    patch = lstInstruments.SelectedIndex;
            }
            lstInstruments.Hide();
        }

        /// <summary>
        /// Keydown event on lst
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstInstruments_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Enter:
                    {
                        SelectInstrument();
                        break;
                    }

                case Keys.Escape:
                    {
                        lstInstruments.Hide();
                        break;
                    }
            }
        }

        /// <summary>
        /// Text changed = delegate event for patch change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblTrackLabel_TextChanged(object sender, EventArgs e)
        {
            // Fab pour correction patch = -1 12/04/2014
            if (lstInstruments.SelectedIndex != -1)
                patch = lstInstruments.SelectedIndex;

            // Delegate the event to the caller
            OntrkControllblPatchChanged?.Invoke(this, e, patch);
        }

        #endregion instrument


        #region channel

        private void lblChannel_Click(object sender, EventArgs e)
        {
            // If aleady visible, hide it
            if (lstChannels.Visible)
            {
                lstChannels.Hide();
                return;
            }

            // Else, hide others
            for (int i = 0; i < Parent.Controls.Count; i++)
            {
                if (Parent.Controls[i].GetType() == typeof(ListBox))
                {
                    if (Parent.Controls[i].Name == "lstChannels")
                    {
                        Parent.Controls[i].Hide();
                        break;
                    }
                }
            }
            // And make its own visible
            DisplaylstChannels();
        }


        private void DisplaylstChannels()
        {
            lstChannels.Location = new Point(this.Location.X + lblChannel.Location.X, this.Location.Y + this.lblChannel.Location.Y + this.lblChannel.Height);
            lstChannels.SelectedItem = Convert.ToInt32(lblChannel.Text);

            Parent.Controls.Add(lstChannels);
            lstChannels.Visible = true;
            lstChannels.BringToFront();
            lstChannels.Focus();

            lstChannels.Click += new EventHandler(lstChannels_Click);
            lstChannels.KeyDown += new KeyEventHandler(lstChannels_KeyDown);

        }

        private void lblChannel_TextChanged(object sender, EventArgs e)
        {
            // Delegate the event to the caller
            OntrkControllblChannelChanged?.Invoke(this, e, midichannel);
        }

        private void lstChannels_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Enter:
                    {
                        SelectChannel();
                        break;
                    }

                case Keys.Escape:
                    {
                        lstChannels.Hide();
                        break;
                    }
            }
        }

        private void lstChannels_Click(object sender, EventArgs e)
        {
            SelectChannel();
        }

        private void SelectChannel()
        {
            int ret = 0;
            ret = Convert.ToInt32(lstChannels.Items[lstChannels.SelectedIndex]);

            if (ret >= 0 && ret <= 15)
            {
                midichannel = ret;
                lblChannel.Text = midichannel.ToString();
            }
            lstChannels.Hide();

        }

        #endregion channel


        #region track Name

        private void lblTrackName_TextChanged(object sender, EventArgs e)
        {
            OntrkControllblTrackNameChanged?.Invoke(this, e, trackname);
        }

        /// <summary>
        /// Change Track Name
        /// </summary>
        private void RenameTrack(string NewName)
        {
            trackname = NewName;
            lblTrackName.Text = NewName;            
            doEdit = false;
        }

        /// <summary>
        /// Double click on label Track Name => display editbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblTrackName_DoubleClick(object sender, EventArgs e)
        {
            #region guard
            if (!_enabled)
                return;
            #endregion

            //doEdit = true;
            editbox.Bounds = lblTrackName.Bounds;
            editbox.Text = lblTrackName.Text;
            editbox.Show();
            editbox.BringToFront();
            editbox.SelectAll();
            editbox.Focus();
            doEdit = true;
        }


        #endregion track Name       


        #region volume       
        private void knobVolume_ValueChanged(object Sender)
        {
            volume = knobVolume.Value;
            lblVolume.Text = String.Format("{0}%", 100 * volume / knobVolume.Maximum);
            
            OnknobControlknobVolumeValueChanged?.Invoke(this);
        }

        #endregion volume       


        #region pan
        private void knobPan_ValueChanged(object Sender)
        {
            pan = knobPan.Value;
            lblPan.Text = String.Format("{0}%", 100 * (pan - 64) / knobPan.Maximum);
            OnknobControlknobPanValueChanged?.Invoke(this);
        }

        #endregion


        #region Reverb
        private void knobReverb_ValueChanged(object Sender)
        {
            reverb = knobReverb.Value;
            lblReverb.Text = String.Format("{0}%", 100 * reverb / knobReverb.Maximum);
            OnknobControlknobReverbValueChanged?.Invoke(this);
        }

        #endregion


        #region editbox

        /// <summary>
        /// Allow only ascii characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //97 - 122 = Ascii codes for simple letters
            //65 - 90  = Ascii codes for capital letters
            //48 - 57  = Ascii codes for numbers

            if (e.KeyChar != 8)
            {
                if (e.KeyChar < 32 || e.KeyChar > 126)
                {
                    e.Handled = true;
                }
            }
        }

       

        private void editbox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        // Terminate text entering in the listview
                        doEdit = true;
                        
                        editbox.Hide();
                        break;
                    }
                case Keys.Escape:
                    {
                        doEdit = false;
                        editbox.Hide();
                        break;
                    }
            }
        }

        private void editbox_LostFocus(object sender, EventArgs e)
        {            
            editbox.Hide();
        }

        private void editbox_VisibleChanged(object sender, EventArgs e)
        {
            // Rename 1 file
            if (doEdit)
            {
                RenameTrack(editbox.Text);
            }
        }


   
        private void trackControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Abort text entering TrackName
                case Keys.Escape:
                    {
                        if (editbox.Visible == true)
                        {
                            doEdit = false;
                            editbox.Hide();
                        }
                        break;
                    }
            }
        }


        #endregion editbox


        #region panel events
        
        /// <summary>
        /// Select track control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Click(object sender, EventArgs e)
        {
            #region guard
            if (!_enabled)
                return;
            #endregion
            
            OntrkControlClick?.Invoke(this);
        }

        /// <summary>
        /// Initiate drag drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            #region guard
            if (!_enabled)
                return;
            #endregion

            int xTrans = e.X + this.Location.X;
            int yTrans = e.Y + this.Location.Y;
            MouseEventArgs eTrans = new MouseEventArgs(e.Button, e.Clicks, xTrans, yTrans, e.Delta);
            this.OnMouseDown(eTrans);
            
            this.OnMouseMove(e);
        }

        private void panel1_DragOver(object sender, DragEventArgs e)
        {
            this.OnDragOver(e);
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            this.OnDragDrop(e);
        }

        private void panel1_DragLeave(object sender, EventArgs e)
        {
            this.OnDragLeave(e);
        }


        #endregion


        #region Window

        private void btnMaximized_Click(object sender, EventArgs e)
        {
            bMaximized = !bMaximized;

            OntrkControlbtnMaximizeClicked?.Invoke(this, e, bmaximized);
        }

        #endregion
    }
}
