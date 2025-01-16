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
        private string file;

        public frmTempoMap(string fileName)
        {
            InitializeComponent();

            file = fileName;
        }

        // See also lyric.TimeAs<MetricTimeSpan>;
        private void button1_Click(object sender, EventArgs e)
        {
            var midiFile = MidiFile.Read(file);
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

            string s;
            string tps;            
            foreach ( var lyric in lyricsWithTimestamps )
            {
                s = lyric.Text;
                tps = TicksToTime(lyric.TotalSecondsFromStart);

                if (s.Length > 2 && s.Substring(0, 1) == "\r")
                {                                         
                    Console.WriteLine(String.Format("[{0}]{1}", tps, "/"));
                    Console.WriteLine(String.Format("[{0}]{1}", tps, s.Substring(1, s.Length - 1)));
                } else
                {
                    Console.WriteLine(String.Format("[{0}]{1}", tps, s));
                }

               
            }

        }

        /// <summary>
        /// Convert ticks to time
        /// Minutes, seconds, cent of seconds
        /// Ex: 6224 ticks => 00:09.10 (mm:ss.cent)
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private string TicksToTime(double t)
        {            
            int Min = (int)(t / 60);
            int Sec = (int)(t - (Min * 60));
            int Cent = (int)(100 * (t - (Min * 60) - Sec));

            string tx = string.Format("{0:00}:{1:00}.{2:00}", Min, Sec, Cent);
            return tx;
        }
    }
}
