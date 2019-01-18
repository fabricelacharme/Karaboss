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
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace Karaboss
{
    public partial class frmLyricsSelectTrack : Form
    {

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
        
        public frmLyricsSelectTrack(Sequence sequence1)
        {
            InitializeComponent();
            LoadTracks(sequence1);
        }


        private void LoadTracks(Sequence sequence1)
        {
            string name = string.Empty;
            string item = string.Empty;

            item = "No melody track";
            cbSelectTrack.Items.Add(item);

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {                
                Track track = sequence1.tracks[i];                
                
                if (track.Name == null)
                    name =  "";
                else
                    name = track.Name;

                int patch = track.ProgramChange;
                if (patch > 127)
                    patch = 0;
                item = i.ToString("00") + " - " + lsInstruments[patch] + " - " + name;                                
                cbSelectTrack.Items.Add(item);
            }
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



        private void FrmSelectTrack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bCancel == true)
            {
                e.Cancel = true;
            }
        }
    }
}
