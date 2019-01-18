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
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.Score.UI
{
    public partial class frmNewTrackDialog : Form
    {
        private static List<int> LoadChannels()
        {
            List<int> list1 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            return list1;
        }
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

        private decimal MaxIndex = 0;

        #region properties
        public string TrackName
        {
            get
            {
                return this.txtTrackName.Text;
            }
        }
        public int ProgramChange
        {
            get
            { return this.cbInstruments.SelectedIndex; }
        }
        public string InstrumentName
        {
            get
            { return this.cbInstruments.Text; }
        }
        public int MidiChannel
        {
            get
            { return this.cbChannels.SelectedIndex; }
        }

        public decimal trackindex
        {
            get
            { return this.updIndex.Value; }
        }

        public int cle
        {
            get { return this.cbClef.SelectedIndex; }
        }


        #endregion properties

        public frmNewTrackDialog(string trackname, int programchange, int channel, decimal indexmax, int clef)
        {
            InitializeComponent();

            InitCbInstrunments();
            InitCbChannels();
            
            MaxIndex = indexmax;

            txtTrackName.Text = trackname;
            cbInstruments.SelectedIndex = programchange;
            cbChannels.SelectedIndex = channel;
            updIndex.Value = indexmax;
            cbClef.SelectedIndex = clef;

            
        }


        private void InitCbInstrunments()
        {
            List<string> lsI = LoadInstruments();
            for (int i = 0; i < lsI.Count; i++)
            {
                cbInstruments.Items.Add(lsI[i]);
            }
        }
        private void InitCbChannels()
        {
            List<int> lsC = LoadChannels();
            for (int i = 0; i < lsC.Count; i++)
            {
                cbChannels.Items.Add(lsC[i]);
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }

        private void cbChannels_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cbInstruments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void updIndex_ValueChanged(object sender, EventArgs e)
        {
            if (updIndex.Value > MaxIndex)
            {
                updIndex.Value = MaxIndex;
            }
        }

        private void cbClef_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
