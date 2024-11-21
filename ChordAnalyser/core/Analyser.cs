#region License

/* Copyright (c) 2024 Fabrice Lacharme
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

using System.Collections.Generic;

namespace ChordAnalyser
{
    public class Analyser
    {

        List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public Analyser() { }

        /// <summary>
        /// Determine the letter of a chord 
        /// </summary>
        /// <param name="notes"></param>
        /// <returns>string</returns>
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
        
        /// <summary>
        /// Return the letter of a chord made with 3 notes (triad), major or minor (+ m)
        /// </summary>
        /// <param name="triad"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return the letter of a chord plus 7
        /// </summary>
        /// <param name="seventh"></param>
        /// <returns></returns>
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

        /// <summary>
        /// notes.count is greater than 4
        /// </summary>
        /// <param name="others"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Return true if Major Chord, false otherwise
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        static bool IsMajorChord(List<int> notes)
        {
            // Un coup ça marche sans % 12, un coup avec
            // Si les number des notes sont dans le bon ordre, c'est bon 
            if (notes.Count < 3) return false;

            // A major chord consists of the root, major third, and perfect fifth
            return (notes[1] - notes[0] == 4) && (notes[2] - notes[0] == 7);
        }


        /// <summary>
        /// Return true if minor chord, false otherwise
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        static bool IsMinorChord(List<int> notes)
        {
            if (notes.Count < 3) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1] - notes[0] == 3) && (notes[2] - notes[0] == 7);
        }

    }
}
