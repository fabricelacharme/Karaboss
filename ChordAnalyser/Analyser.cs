using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChordAnalyser
{
    public class Analyser
    {

        List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public Analyser() { }


        public string determine(List<int> notes)
        {
            string res = string.Empty;

            if (IsMajorChord(notes))
            {
                res = letters[notes[0]];
            }
            else if (IsMinorChord(notes))
            {
                res = letters[notes[0]] + "m";
            }


            return res;
        }


        static bool IsMajorChord(List<int> notes)
        {
            // Un coup ça marche sans % 12, un coup avec
            // Si les number des notes sont dans le bon ordre, c'est bon 
            if (notes.Count < 3) return false;

            // A major chord consists of the root, major third, and perfect fifth
            return (notes[1] - notes[0] == 4) && (notes[2] - notes[0] == 7);
        }

        static bool IsMinorChord(List<int> notes)
        {
            if (notes.Count < 3) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1] - notes[0] == 3) && (notes[2] - notes[0] == 7);
        }

    }
}
