using ChordsAnalyser.cchords;
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

            if (notes.Count == 0)
                return null;
            else if (notes.Count == 1 || notes.Count == 2)
                return determine_single(notes);
            else if (notes.Count == 3)
                return determine_triad(notes);
            else if (notes.Count == 4)
                return determine_seventh(notes);
            else
                return determine_others(notes);
            
        }


        private string determine_single(List<int> single)
        {
            return letters[single[0]];
        }
        private string determine_triad(List<int> triad)
        {
            string res = string.Empty;

            if (IsMajorChord(triad))
            {
                res = letters[triad[0]];
            }
            else if (IsMinorChord(triad))
            {
                res = letters[triad[0]] + "m";
            }
            return res;
        }

        private string determine_seventh(List<int> seventh)
        {
            string res = string.Empty;
            List<int> triad = new List<int>() { seventh[0], seventh[1], seventh[2] };
            if (IsMajorChord(triad))
            {
                res = letters[triad[0]];
                if (seventh[3] - seventh[0] == 10)
                    res += "7";
            } 
            else if (IsMinorChord(triad))
            {
                res = letters[triad[0]] + "m";
                if (seventh[3] - seventh[0] == 10)
                    res += "7";
            }

            return res;
        }


        private string determine_others(List<int> others)
        {
            string res = string.Empty;
            List<int> triad = new List<int>() { others[0], others[1], others[2] };
            if (IsMajorChord(triad))
            {
                res = letters[triad[0]];
                if (others[3] - others[0] == 10)
                    res += "7";
            }
            else if (IsMinorChord(triad))
            {
                res = letters[triad[0]] + "m";
                if (others[3] - others[0] == 10)
                    res += "7";
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
