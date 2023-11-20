using Sanford.Multimedia.Midi;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace MusicTxt
{
    public class MidiTrackTextImporter
    {
        public delegate void TrackSelectedEventHandler(object sender, Track track);
        public event TrackSelectedEventHandler TrackSelected;

        MusicTxtReader MTxtReader;       
        private Sequence sequence;

        public MidiTrackTextImporter()
        {

        }

        public void Read(string fileName)
        {
            // Load file
            LoadAsyncTxtFile(fileName);
        }

        /// <summary>
        /// Load async a TXT file
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadAsyncTxtFile(string fileName)
        {
            try
            {                             
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

        private void HandleLoadTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (MTxtReader.seq == null)
            {
                MessageBox.Show("Invalid dump file", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            sequence = MTxtReader.seq;
            sequence.Clean();

            // Propose to select tracks
            DialogResult dr = new DialogResult();
            frmSelectTrack frmSelect = new frmSelectTrack(sequence);
            dr = frmSelect.ShowDialog();
            if (dr == DialogResult.Cancel)
                return;

            // Raise event track selected
            TrackSelected?.Invoke(this, sequence.tracks[frmSelect.TrackNumber]);
        }

  
    }
}
