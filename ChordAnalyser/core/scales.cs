#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ChordsAnalyser.ckeys;

namespace ChordsAnalyser.cscales
{
    public class scales
    {
        /// <summary>
        /// Determine the scales containing the notes.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private List<string> determine(List<string> notes) {
            /*
            All major and minor scales are recognized.

            Example:
            >>> determine(['A', 'Bb', 'E', 'F#', 'G'])
            ['G melodic minor', 'G Bachian', 'D harmonic major']
            */
            //List<string> notes = notes;
            List<string> res = new List<string>();

            foreach ((string, string) key in nkeys.keys) 
            {
                foreach (_Scale scale in _Scale.__subclasses__())
                {
                    if (scale.type == "major")
                    {
                        if (notes <= set(scale(key[0]).ascending()) || notes <= set(scale(key[0]).descending()))
                            res.Add(scale(key[0]).name);
                    }
                    else if (scale.type == "minor")
                    {
                        if notes <= set(scale(get_notes(key[1])[0]).ascending()) or notes <= set(scale(get_notes(key[1])[0]).descending()))
                            res.Add(scale(get_notes(key[1])[0]).name);
                    }
                }
            }
            return res;
        }

    }


    public class _Scale
    {
        /*
        General class implementing general methods.
        Not to be used by the final user.
        */
        //string tonic;
        //int octaves;
        //string name;

        _Scale(string inote, int ioctaves)
        {
            if (inote.All(char.IsLower))
                throw new FormatException(string.Format("Unrecognised note '{0}'", inote));


            tonic = inote;
            octaves = ioctaves;
        }

        private string __repr__()
        {
            return "<Scale object ('{0}')>" + this.name;
        }

        private string __str__()
        {
            return string.Format("Ascending:  {0}\nDescending: {1}", this.ascending(), this.descending());
        }

        private bool __eq__(_Scale other) {
            if (this.ascending() == other.ascending())
                if (this.descending() == other.descending())
                    return true;
            return false;
        }

        private bool __ne__(_Scale other) {
            return !this.__eq__(other);
        }

        private int __len__() {
            return this.ascending().Count;
        }

        private List<string> ascending() {
            // Return the list of ascending notes.
            throw new NotImplementedException("");
        }
        private List<string> descending()
        {
            // """Return the list of descending notes."""
            return (reversed(this.ascending()));
        }

        private List<string> reversed(List<string> list)
        {
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Return the asked scale degree.
        /// </summary>
        /// <param name="degree_number"></param>
        /// <param name="direction"></param>
        private string degree(int degree_number, string direction = "a") {
            /*
            The direction of the scale is 'a' for ascending (default) and 'd'
            for descending.
            */
            List<string> n = new List<string>();

            if (degree_number < 1)
                throw new ArgumentOutOfRangeException(string.Format("degree '{0}' out of range", degree_number));

            if (direction == "a") {
                n = this.ascending();
                n.RemoveAt(n.Count - 1);  //  [:-1];
                return n[degree_number - 1];
            }
            else if (direction == "d") {
                n = reversed(this.descending()); //[:-1]);
                n.RemoveAt(n.Count - 1);
                return n[degree_number - 1];
            }
            else
                throw new FormatException(string.Format("Unrecognised direction '{0}'", direction);
        }





        /// <summary>
        /// The diatonic scale.
        /// </summary>
        /// <param name=""></param>
        public class Diatonic() : this()
    {
        /*        
        Example:
        >>> print Diatonic('C', (3, 7))
        Ascending:  C D E F G A B C
        Descending: C B A G F E D C
        */
        int tonic { get; }
        int octaves { get; }
        string name { get; }
        List<int> semitones { get; }

        string type = "diatonic";

        /// <summary>
        /// """Create the diatonic scale starting on the chosen note.
        /// </summary>
        /// <param name="note"></param>
        /// <param name="semitones"></param>
        /// <param name="octaves"></param>
        Diatonic(string inote, List<int> isemitones, int ioctaves = 1) {
            /*

            The second parameter is a tuple representing the position of
            semitones.
            */

            this.semitones = isemitones;
            this.name = string.Format("{0} diatonic, semitones in {1}", tonic, semitones);
        }

        private List<string> ascending()
        {
            int notes = tonic;
            for (int n = 1; n < 7; n++) //in range(1, 7)) 
            {
                if (semitones.Contains(n))
                    notes.Add(intervals.minor_second(notes[-1]));
                else
                    notes.Add(intervals.major_second(notes[-1]));
            }
            return notes * octaves + [notes[0]];
        }

    }
}
