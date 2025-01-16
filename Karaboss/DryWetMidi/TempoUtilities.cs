using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Karaboss.DryWetMidi
{
    internal static class TempoUtilities
    {
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
        private static List<(int, int)> GetAllTempoChanges(Sequence seq)
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

        // Return midi file duration in seconds until ticks
        public static double GetMidiDuration(Sequence seq, int untilticks)
        {            
            int _tempovalue;
            int _ticks = 0;
            int _deltaticks; // = 0;
            int _previousticks = 0;
            int _previoustempo = 0;
            double _duration = 0;
            double _ppqn = seq.Division;

            if (_ppqn == 0)
                return 0;

            // (ticks, tempo value)
            List<(int, int)> tempos = GetAllTempoChanges(seq);
            
            for (int i = 0; i < tempos.Count; i++)
            {                
                _ticks = tempos[i].Item1; // ticks between new tempo and previous one

                if (_ticks > untilticks)
                {
                    _deltaticks = untilticks - _previousticks;
                    _duration += (_previoustempo) * (_deltaticks / _ppqn) / 1000000;
                    break;
                }
                _deltaticks = _ticks - _previousticks;
                _previousticks = _ticks;

                _tempovalue = tempos[i].Item2;
                
                _duration += (_previoustempo) * (_deltaticks / _ppqn) / 1000000;
                
                _previoustempo = _tempovalue;

            }

            if (_ticks < untilticks)
            {
                _deltaticks = untilticks - _ticks;
                _duration += (_previoustempo) * (_deltaticks / _ppqn) / 1000000;
            }


            return _duration;
        }

    }
}
