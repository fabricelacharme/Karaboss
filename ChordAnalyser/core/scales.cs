#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ChordsAnalyser.cintervals;
using ChordsAnalyser.ckeys;
using System.Management;
using System.Runtime.ExceptionServices;
using ChordsAnalyser.cnotes;

namespace ChordsAnalyser.cscales
{
    public class scales
    {

        ckeys.nkeys k = new ckeys.nkeys();

        /// <summary>
        /// Determine the scales containing the notes.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public List<string> determine(List<string> notes) {
            /*
            All major and minor scales are recognized.

            Example:
            >>> determine(['A', 'Bb', 'E', 'F#', 'G'])
            ['G melodic minor', 'G Bachian', 'D harmonic major']
            */

            
            List<string> res = new List<string>();

            //var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.IsSubclassOf(typeof(_Scale)) select type;

            
            var subclasses =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.IsSubclassOf(typeof(_Scale))
                select type;


            foreach ((string, string) key in nkeys.keys)
            {                
                foreach (Type sc in subclasses)
                {
                    _Scale scale = (_Scale)sc;                    
                    
                    //Ionian io = new Ionian("a");
                    //List<string> list = new Ionian("b").ascending();


                    if (scale.type == "major")
                    {                        
                        if (notes <= scale(key.Item1).ascending() || notes <= scale(key.Item1).descending())
                            res.Add(scale(key.Item1).name);
                    }
                    else if (scale.type == "minor")
                    {
                        if (notes <= scale(k.get_notes(key.Item2)[0]).ascending() || notes <= scale(k.get_notes(key.Item2)[0]).descending()) 
                            res.Add(scale(k.get_notes(key.Item2)[0]).name);
                    }
                }
            }
            return res;
        }


        public class _Scale
        {
            /*
            General class implementing general methods.
            Not to be used by the final user.
            */
            public string tonic { get; set; }
            public int octaves { get; set; }
            public string name { get; set; }
            public string type { get; set; }



            public _Scale(string note, int octaves)
            {
                if (note.All(char.IsLower))
                    throw new FormatException(string.Format("Unrecognised note '{0}'", note));

                this.tonic = note;
                this.octaves = octaves;
            }

            public string __repr__()
            {
                return "<Scale object ('{0}')>" + name;
            }

            public string __str__()
            {
                return string.Format("Ascending:  {0}\nDescending: {1}", this.ascending(), this.descending());
            }

            public bool __eq__(_Scale other) {
                if (this.ascending() == other.ascending())
                    if (this.descending() == other.descending())
                        return true;
                return false;
            }

            public bool __ne__(_Scale other) {
                return !this.__eq__(other);
            }

            public int __len__() {
                return this.ascending().Count;
            }

            public List<string> ascending() {
                // Return the list of ascending notes.
                throw new NotImplementedException("");
            }
            public List<string> descending()
            {
                // """Return the list of descending notes."""
                return reversed(this.ascending());
            }

            public List<string> reversed(List<string> list)
            {
                list.Reverse();
                return list;
            }

            /// <summary>
            /// Return the asked scale degree.
            /// </summary>
            /// <param name="degree_number"></param>
            /// <param name="direction"></param>
            public string degree(int degree_number, string direction = "a") {
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
                    throw new FormatException(string.Format("Unrecognised direction '{0}'", direction));
            }

            public static explicit operator _Scale(Type v)
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// The diatonic scale.
        /// </summary>
        /// <param name=""></param>
        public class Diatonic : _Scale
        {
            /*        
            Example:
            >>> print Diatonic('C', (3, 7))
            Ascending:  C D E F G A B C
            Descending: C B A G F E D C
            */
            List<int> semitones { get; set; }

            
            cintervals.intervals intervals = new cintervals.intervals();

            /// <summary>
            /// """Create the diatonic scale starting on the chosen note.
            /// </summary>
            /// <param name="note"></param>
            /// <param name="semitones"></param>
            /// <param name="octaves"></param>
            public Diatonic(string note, List<int> semitons, int octaves = 1) : base(note,octaves)
            {
                /*
                The second parameter is a tuple representing the position of
                semitones.
                */                
                this.semitones = semitons;
                this.octaves = octaves;
                name = string.Format("{0} diatonic, semitones in {1}", tonic, semitones);
                type = "diatonic";
            }

            public new List<string> ascending()
            {
                List<string> notes = new List<string> { tonic.ToString() };


                for (int n = 1; n < 7; n++) //in range(1, 7)) 
                {
                    if (semitones.Contains(n))
                        notes.Add(intervals.minor_second(notes[notes.Count - 1]));
                    else
                        notes.Add(intervals.major_second(notes[-1]));
                }

                //return notes * self.octaves + [notes[0]]
                // * = repeat
                List<string> res = new List<string>();
                for (int i = 0; i < octaves; i++)
                {
                    for (int j = 0; j < notes.Count; j++)
                        res.Add(notes[j]);
                }
                res.Add(notes[0]);
                return res;
            }

        }


        public class Ionian: _Scale 
        {
            public Ionian(string note, int octaves = 1) : base(note,octaves)
            {
                type = "ancient";
                name = string.Format("{0} ionian", tonic);
            }

            public new List<string> ascending() {                                
                List<string> notes = new Diatonic(tonic, new List<int>() { 3, 4, 5, 6 }).ascending();                
                notes.RemoveAt(notes.Count - 1);

                //return notes * self.octaves + [notes[0]]
                // * = repeat
                List<string> res = new List<string>();
                for (int i = 0; i < octaves; i++)
                {
                    for (int j = 0; j < notes.Count; j++)
                        res.Add(notes[j]);
                }
                res.Add(notes[0]);               
                return res;                                
            }

        }


        public class Dorian: _Scale
        {
            public Dorian(string note, int octaves) : base(note,octaves)
            {
                type = "ancient";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Phrygian : _Scale 
        {
            public Phrygian(string note, int octaves) : base (note,octaves)
            {
                type = "ancient";
            }
        }

        public class Lydian : _Scale
        {
            public Lydian(string note, int octaves) : base(note, octaves)
            {
                type = "ancient";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Mixolydian : _Scale
        {
            public Mixolydian(string note, int octaves) : base(note, octaves)
            {
                type = "ancient";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Aeolian : _Scale
        {
            public Aeolian(string note, int octaves) : base(note, octaves)
            {
                type = "ancient";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Locrian : _Scale
        {
            public Locrian(string note, int octaves) : base(note, octaves)
            {
                type = "ancient";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Major : _Scale
        {
            public Major(string note, int octaves) : base(note, octaves)
            {
                type = "major";
            }
        }

        public class HarmonicMajor : _Scale
        {
            public HarmonicMajor(string note, int octaves) : base(note, octaves)
            {
                type = "major";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class NaturalMinor : _Scale
        {
            public NaturalMinor(string note, int octaves) : base(note, octaves)
            {
                type = "minor";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class HarmonicMinor : _Scale
        {
            public HarmonicMinor(string note, int octaves) : base(note, octaves)
            {
                type = "minor";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class MelodicMinor : _Scale
        {
            public MelodicMinor(string note, int octaves) : base(note, octaves)
            {
                type = "minor";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Bachian : _Scale
        {
            public Bachian(string note, int octaves) : base(note, octaves)
            {
                type = "minor";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class MinorNeapolitan : _Scale
        {
            public MinorNeapolitan(string note, int octaves) : base(note, octaves)
            {
                type = "minor";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Chromatic : _Scale
        {
            public Chromatic(string note, int octaves) : base(note, octaves)
            {
                type = "other";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class WholeTone : _Scale
        {
            public WholeTone(string note, int octaves) : base(note, octaves)
            {
                type = "other";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }

        public class Octatonic : _Scale
        {
            public Octatonic(string note, int octaves) : base(note, octaves)
            {
                type = "other";
            }

            public new List<string> ascending()
            {

                return new List<string> { };
            }
        }



    }
}
