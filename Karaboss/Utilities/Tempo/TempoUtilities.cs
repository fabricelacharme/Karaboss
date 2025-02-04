#region License

/* Copyright (c) 2025 Fabrice Lacharme
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

namespace Karaboss.Utilities
{
    internal static class TempoUtilities
    {

        /// <summary>
        /// Tempo list : list (ticks, tempo)
        /// </summary>
        public static List<(int, int)> lstTempos;     


        /// <summary>
        /// Return the list of all Tempo changes - Format: (ticks, tempo, duration)
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

            // Set tempo to default if no value found
            if (result.Count == 0)
            {
                result.Add((0, 50000));
            }
            
            return result;
        }
       

        /// <summary>
        /// Returns a duration that takes tempo changes into account
        /// </summary>
        /// <param name="untilticks"></param>
        /// <param name="division"></param>
        /// <returns></returns>
        public static double GetMidiDuration(double untilticks, double division)
        {            
            int _tempovalue;
            int _ticks = 0;
            double _deltaticks;
            double _previousticks = 0;
            int _previoustempo = 0;
            double _duration = 0;            

            if (division == 0)
                return 0;

            // (ticks, tempo value)            
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
