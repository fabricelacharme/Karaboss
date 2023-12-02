#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChordsAnalyser.cnotes
{

    public class notes
    {
               
        public notes() { }

        public static Dictionary<string,int> _note_dict = new Dictionary<string,int>() { { "C", 0 }, { "D", 2 }, { "E", 4 }, { "F", 5 }, { "G", 7 }, { "A", 9 }, { "B", 11 } };
        public static List<string> fifths = new List<string>() { "F", "C", "G", "D", "A", "E", "B" };

        /// <summary>
        /// Convert integers in the range of 0-11 to notes in the form of C or C# or Db.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        private string int_to_note(int note_int, string accidentals = "#") { 
            /*    
            Throw a RangeError exception if the note_int is not in the range 0-11.
            If not specified, sharps will be used.

            Examples:
            >>> int_to_note(0)
            'C'
            >>> int_to_note(3)
            'D#'
            >>> int_to_note(3, 'b')
            'Eb'
            */
            if (note_int <0 || note_int > 11) 
                throw new ArgumentOutOfRangeException(string.Format("int out of bounds (0-11): {0}", note_int));

            List<string> ns = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            List<string> nf = new List<string>() { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };
            if (accidentals == "#")
                return ns[note_int];
            else if (accidentals == "b")
                return nf[note_int];
            else
                throw new FormatException(string.Format("'{0}' not valid as accidental", accidentals));
        }

        /// <summary>
        /// Test whether note1 and note2 are enharmonic, i.e. they sound the same.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        private bool is_enharmonic(string note1, string note2) 
        {
            return note_to_int(note1) == note_to_int(note2);
        }

        /// <summary>
        /// Return True if note is in a recognised format. False if not.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public  bool is_valid_note(string note) {            
            if (!_note_dict.ContainsKey(note.Substring(0,1)))
                return false;
            if (note.Length > 1) {                
                foreach (char post in note.Substring(1))
                    if (post.ToString() != "b" && post.ToString() != "#")
                        return false;
            }
            return true;
        }

        /// <summary>
        /// Convert notes in the form of C, C#, Cb, C##, etc. to an integer in the range of 0-11.
        /// Throw a NoteFormatError exception if the note format is not recognised.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private int note_to_int(string note) {
            int val;
            if (is_valid_note(note))
                val = _note_dict[note.Substring(0, 1)];
            else
                throw new FormatException(string.Format("Unknown note format '{0}'", note));

            // Check for '#' and 'b' postfixes
            if (note.Length > 1) {
                foreach (char post in note.Substring(1)) 
                {
                    if (post.ToString() == "b")
                        val -= 1;
                    else if (post.ToString() == "#")
                        val += 1;
                }
             }
             return val % 12;
        }

        /// <summary>
        /// Reduce any extra accidentals to proper notes.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private string reduce_accidentals(string note) {
            /*
            Example:
            >>> reduce_accidentals('C####')
            'E'
            */
            int val = note_to_int(note.Substring(0, 1));
            if (note.Length > 1) {
                foreach (char token in note.Substring(1)) {
                    if (token.ToString() == "b")
                        val -= 1;
                    else if (token.ToString() == "#")
                        val += 1;
                    else
                        throw new FormatException(string.Format("Unknown note format '{0}'", note));
                }
            }
            if (val >= note_to_int(note.Substring(0, 1)))
                return int_to_note(val % 12);
            else
                return int_to_note(val % 12, "b");
        }

        /// <summary>
        /// Remove redundant sharps and flats from the given note.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private string remove_redundant_accidentals(string note)
        {
            /*
            Examples:
            >>> remove_redundant_accidentals('C##b')
            'C#'
            >>> remove_redundant_accidentals('Eb##b')
            'E'
            */
            int val = 0;
            foreach (char token in note.Substring(1))
            {
                if (token.ToString() == "b")
                    val -= 1;
                else if (token.ToString() == "#")
                    val += 1;
            }
            string result = note.Substring(0, 1);
            while (val > 0)
            {
                result = augment(result);
                val -= 1;
            }
            while (val < 0)
            {
                result = diminish(result);
                val += 1;
            }
            return result;
        }


        /// <summary>
        /// Augment a given note.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private string augment(string note) {
            /*
            Examples:
            >>> augment('C')
            'C#'
            >>> augment('Cb')
            'C'
            */
            if (note.Substring(note.Length - 1) != "b")
                return note + "#";
            else
                return note.Substring(0, note.Length - 1);
        }

        /// <summary>
        /// Diminish a given note.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private string diminish(string note)
        {
            /*
             Examples:
            >>> diminish('C')
            'Cb'
            >>> diminish('C#')
            'C'
            */
            if (note.Substring(note.Length - 1) != "#")
                return note + "b";
            else
                return note.Substring(0,note.Length - 1);

        }

    }
}
