using System.Collections.Generic;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace Karaboss.Utilities
{
    internal static class TempoUtilities
    {

        /// <summary>
        /// Tempo map : list (ticks, tempo value)
        /// </summary>
        public static List<(int, int)> lstTempos;

        #region deleteme

        /*
        /// <summary>
        /// Export to LRC format
        /// </summary>
        /// <param name="MidiFileName"></param>
        /// <param name="LrcFileName"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        public static void ExportToLRC(string MidiFileName, string LrcFileName, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus)
        {           
            string lrcs = string.Empty;
            string cr = Environment.NewLine;

            var midiFile = Melanchall.DryWetMidi.Core.MidiFile.Read(MidiFileName);
            var tempoMap = midiFile.GetTempoMap();
            var lyricsWithTimestamps = midiFile
                .GetTimedEvents()
                .Where(ee => ee.Event.EventType == MidiEventType.Lyric)
                .Select(ee => new
                {
                    Text = ((LyricEvent)ee.Event).Text,
                    TotalSecondsFromStart = ee.TimeAs<MetricTimeSpan>(tempoMap).TotalSeconds,
                })
                .ToArray();

            // Print midi file infos
            if (Tag_Title != "")
                lrcs += "[Ti:" + Tag_Title + "]" + cr;
            if (Tag_Artist != "")
                lrcs += "[Ar:" + Tag_Artist + "]" + cr;
            if (Tag_Album != "")
                lrcs += "[Al:" + Tag_Album + "]" + cr;
            if (Tag_Lang != "")
                lrcs += "[La:" + Tag_Lang + "]" + cr;
            if (Tag_By != "")
                lrcs += "[By:" + Tag_Album + "]" + cr;
            if (Tag_DPlus != "")
                lrcs += "[D+:" + Tag_DPlus + "]" + cr;

            // Print times
            string tx;
            string tps;

            foreach (var lyric in lyricsWithTimestamps)
            {
                tx = lyric.Text;
                tps = TicksToTime(lyric.TotalSecondsFromStart);

                if (tx.Length > 2 && tx.Substring(0, 1) == "\r")
                {
                    // LineFeed + String
                    lrcs += String.Format("[{0}]{1}", tps, "/") + cr;
                    lrcs += String.Format("[{0}]{1}", tps, tx.Substring(1, tx.Length - 1)) + cr;
                    
                }
                else
                {
                    // Single string
                    lrcs += String.Format("[{0}]{1}", tps, tx) + cr;                    
                }
            }


            // Wrtite to disk and display
            try
            {
                System.IO.File.WriteAllText(LrcFileName, lrcs);
                System.Diagnostics.Process.Start(@LrcFileName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        */

        #endregion deleteme

        /// <summary>
        /// Convert ticks to time
        /// Minutes, seconds, cent of seconds
        /// Ex: 6224 ticks => 00:09.10 (mm:ss.cent)
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private static string TicksToTime(double t)
        {
            int Min = (int)(t / 60);
            int Sec = (int)(t - (Min * 60));
            int Cent = (int)(100 * (t - (Min * 60) - Sec));

            string tx = string.Format("{0:00}:{1:00}.{2:00}", Min, Sec, Cent);
            return tx;
        }


        /// <summary>
        /// Return the list of all Tempo changes - Format: (ticks, tempo value)
        /// </summary>
        /// <returns></returns>
        public static List<(int, int)> GetAllTempoChanges(Sequence seq)
        {
            List<(int, int)> result = new List<(int, int)>();

            List<(int, int)> l = new List<(int, int)>();    // list of tempo events for a track
            List<(int, int)> lt = new List<(int, int)>();   // list of all tempo events for all tracks         

            foreach (Sanford.Multimedia.Midi.Track track in seq.tracks)
            {
                l = track.GetTemposList();
                
                for (int i = 0; i < l.Count; i++)
                {
                    if (!lt.Contains(l[i]))
                    {
                        lt.Add(l[i]);
                        result.Add(l[i]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a duration that takes tempo changes into account
        /// </summary>
        /// <param name="untilticks"></param>
        /// <param name="division"></param>
        /// <returns></returns>
        public static double GetMidiDuration(int untilticks, double division)
        {            
            int _tempovalue;
            int _ticks = 0;
            int _deltaticks;
            int _previousticks = 0;
            int _previoustempo = 0;
            double _duration = 0;            

            if (division == 0)
                return 0;

            // (ticks, tempo value)
            //List<(int, int)> lstTempos = GetAllTempoChanges(seq);
            if (lstTempos == null || lstTempos.Count == 0)
            {
                MessageBox.Show("Tempo map is empty", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            // for each set (ticks, tempo value) compare 'untiltick' with the ticks of each tempo
            for (int i = 0; i < lstTempos.Count; i++)
            {                
                _ticks = lstTempos[i].Item1; // ticks of the tempo

                // If searched tick is less than the tick of the current tempo
                // add partial duration and exit
                if (untilticks <= _ticks)
                {
                    _deltaticks = untilticks - _previousticks;
                    _duration += (_previoustempo) * (_deltaticks / division) / 1000000;
                    break;
                }

                // Searched tick is greater than current tempo tick
                // Add a full duration and continue
                _deltaticks = _ticks - _previousticks;
                _previousticks = _ticks;

                _tempovalue = lstTempos[i].Item2;                
                _duration += (_previoustempo) * (_deltaticks / division) / 1000000;                
                _previoustempo = _tempovalue;

            }

            // The midi file continue after the last change of tempo
            // Add this last duration
            if (_ticks < untilticks)
            {
                _deltaticks = untilticks - _ticks;
                _duration += (_previoustempo) * (_deltaticks / division) / 1000000;
            }


            return _duration;
        }
     
    }
}
