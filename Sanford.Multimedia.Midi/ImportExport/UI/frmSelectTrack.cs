using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi
{
    public partial class frmSelectTrack : Form
    {
        private List<string> lsInstruments = Sanford.Multimedia.Midi.MidiFile.LoadInstruments();

        // the return of this function
        private int tracknumber = -1;
        public int TrackNumber
        {
            get { return tracknumber; }
        }

        public frmSelectTrack(Sequence sequence1)
        {
            InitializeComponent();
            LoadTracks(sequence1);
        }

        private void LoadTracks(Sequence sequence1)
        {
            string name = string.Empty;
            string item = string.Empty;

            for (int i = 0; i < sequence1.tracks.Count; i++)
            {
                Track track = sequence1.tracks[i];

                if (track.Name == null)
                    name = "";
                else
                    name = track.Name;

                int patch = track.ProgramChange;
                if (patch > 127)
                    patch = 0;
                item = i.ToString("00") + " - " + lsInstruments[patch] + " - " + name;
                cbSelectTrack.Items.Add(item);
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbSelectTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            tracknumber = cbSelectTrack.SelectedIndex;
        }
    }
}
