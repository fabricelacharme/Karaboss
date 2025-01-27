using Sanford.Multimedia.Midi;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ChordAnalyser.core
{
    public static class Utilities
    {
        /*
        /// <summary>
        /// Tempo list : list (ticks, tempo)
        /// </summary>
        public static List<(int, int)> lstTempos2;
     

        /// <summary>
        /// Return the list of all Tempo changes - Format: (ticks, tempo value)
        /// </summary>
        /// <returns></returns>
        public static List<(int, int)> GetAllTempoChanges2(Sequence seq)
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
        public static double GetMidiDuration2(int untilticks, double division)
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
            if (lstTempos2 == null || lstTempos2.Count == 0)
            {
                MessageBox.Show("Tempo map is empty", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            // for each set (ticks, tempo value) compare 'untiltick' with the ticks of each tempo
            for (int i = 0; i < lstTempos2.Count; i++)
            {
                _ticks = lstTempos2[i].Item1; // ticks of the tempo

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

                _tempovalue = lstTempos2[i].Item2;
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
        */
    }
        
}
