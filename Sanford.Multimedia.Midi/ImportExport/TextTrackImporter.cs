using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi
{
    public class TextTrackImporter
    {
        public delegate void TrackSelectedEventHandler(object sender, Track track);
        public event TrackSelectedEventHandler TrackSelected;

        private Sequence sequence;

        public TextTrackImporter()
        {

        }

        public void Read(string fileName)
        {            
            // Load file
            FileStream fstream = new FileStream(fileName, FileMode.Open,
               FileAccess.Read, FileShare.None);

            StreamReader stream = new StreamReader(fstream);
            using (stream)
            {
                DumpReader dumpreader = new DumpReader();
                sequence = dumpreader.Read(stream);
            }
            if (sequence == null)
            {
                MessageBox.Show("Invalid dump file", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
