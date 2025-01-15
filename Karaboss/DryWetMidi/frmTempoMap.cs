using Karaboss.DryWetMidi.Core;
using Karaboss.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.DryWetMidi
{
    public partial class frmTempoMap : Form
    {
        public frmTempoMap()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var midiFile = MidiFile.Read("C:\\tmp\\Abba - Fernando.kar");
            var tempoMap = midiFile.GetTempoMap();
            var lyricsWithTimestamps = midiFile
                .GetTimedEvents()
                .Where(ee => ee.Event.EventType == MidiEventType.Lyric)
                .Select(ee => new
                {
                    Text = ((LyricEvent)ee.Event).Text,
                    TotalSecondsFromStart = ee.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds
                })
                .ToArray();

            Console.WriteLine(lyricsWithTimestamps.ToString());
        }
    }
}
