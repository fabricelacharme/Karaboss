using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi
{
    
    public class MidiTrackImporter
    {
        public delegate void TrackSelectedEventHandler(object sender, Track track);
        public event TrackSelectedEventHandler TrackSelected;
        private Sequence sequence;       

        // constructor
        public MidiTrackImporter()
        {

        }
        
        public void Read(string fileName)
        {            
            sequence = new Sequence();
            sequence.LoadCompleted += HandleLoadCompleted;
            LoadAsyncMidiFile(fileName);
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
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

        /// <summary>
        /// Laod midi file
        /// </summary>
        /// <param name="fileName"></param>
        private void LoadAsyncMidiFile(string fileName)
        {
            try
            {
                if (fileName != "\\")
                {
                    sequence.LoadAsync(fileName);
                }
            }
            catch (Exception)
            {
                throw new Exception("Load Midi File");
            }
        }
        
    }        

}
