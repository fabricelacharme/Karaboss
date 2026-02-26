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
using Karaboss.Midi.MidiLyrics;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmMidiLyricsSelectTrack : Form
    {
        private Sequence sequence1;
        OutputDevice outDevice;
        bool bPlaying = false;
        MiniMidiPlayer player;
        bool closing = false;

        private int currentChannel = 0;

        private List<string> lsInstruments = Sanford.Multimedia.Midi.MidiFile.LoadInstruments();
        private bool bCancel = false;

        public int TrackNumber
        {
            get
            { return this.cbSelectTrack.SelectedIndex; }
        }


        public int TextLyricFormat
        {
            get
            {
                if (this.optLyricFormat.Checked)
                    return 1; // Lyric 
                else 
                    return 0; // Text by default
            }
        }
        
        public frmMidiLyricsSelectTrack(Sequence sequence, OutputDevice outDev)
        {
            sequence1 = sequence;
            outDevice = outDev;

            InitializeComponent();
            LoadTracks(sequence);
        }


        private void LoadTracks(Sequence sequence)
        {
            string name = string.Empty;
            string item = string.Empty;

            //item = "No melody track";
            item = Karaboss.Resources.Localization.Strings.NoMelodyTrack;
            cbSelectTrack.Items.Add(item);

            for (int i = 0; i < sequence.tracks.Count; i++)
            {                
                Track track = sequence.tracks[i];                
                
                if (track.Name == null)
                    name =  "";
                else
                    name = track.Name;

                int patch = track.ProgramChange;
                if (patch > 127)
                    patch = 0;
                item = (i + 1).ToString("00") + " chan [" + track.MidiChannel.ToString("00") + "]" + " - " + lsInstruments[patch] + " - " + name;                                
                cbSelectTrack.Items.Add(item);
            }

            cbSelectTrack.SelectedIndex = 0;

        }


        #region buttons

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if ( cbSelectTrack.SelectedItem == null)
            {
                bCancel = true;
            }
            else
            {
                bCancel = false;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            bCancel = false;
        }

        #endregion buttons


        # region form load close
        private void FrmSelectTrack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bCancel == true)
            {
                e.Cancel = true;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (player != null)
                player.Dispose();

            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();

            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        #endregion form load close


        /// <summary>
        /// Play a single track of the midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlayTrack_Click(object sender, EventArgs e)
        {
                        
            if (cbSelectTrack.SelectedItem == null) return;
            if (cbSelectTrack.SelectedIndex == 0) return;


            int i = cbSelectTrack.SelectedIndex - 1;

            if (i > sequence1.tracks.Count) return;

            Track track = sequence1.tracks[i];
            int channel = track.MidiChannel;

            if (player == null)
                player = new MiniMidiPlayer(sequence1, outDevice, channel);


            bPlaying = !bPlaying;
            if (bPlaying)
            {
                // Play track
                btnPlayTrack.Image = Karaboss.Properties.Resources.Media_Controls_Stop_icon;
                player.Play();
            }
            else
            {
                // Stop playing track
                btnPlayTrack.Image = Karaboss.Properties.Resources.Media_Controls_Play_icon;
                player.Stop();
            }           

        }

        private void cbSelectTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSelectTrack.SelectedItem == null) return;
            if (cbSelectTrack.SelectedIndex == 0) return;

            int i = cbSelectTrack.SelectedIndex;
            Track track = sequence1.tracks[i - 1];
            int channel = track.MidiChannel;

            if (channel != currentChannel)
            {
                currentChannel = channel;
                
                if (player != null) 
                    player.Channel = currentChannel;

            }

        }
    }
}
