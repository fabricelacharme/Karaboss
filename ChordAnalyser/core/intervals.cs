using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser.cnotes;
using ChordsAnalyser.ckeys;

namespace ChordsAnalyser.cintervals
{
    public class intervals
    {

        notes notes = new notes();
        nkeys nkeys = new nkeys();


        /// <summary>
        /// Return the note found at the interval starting from start_note in the
        ///given key.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public string interval(string key, string start_note, int interval) 
        {
            /*
            Raise a KeyError exception if start_note is not a valid note.

            Example:
            >>> interval('C', 'D', 1)
            'E'
            */

            if (!notes.is_valid_note(start_note))                 
                throw new KeyNotFoundException(string.Format("The start note '{0]' is not a valid note", start_note));
            
            List<string> notes_in_key = nkeys.get_notes(key);
            int index = 0;  

            foreach (string n in notes_in_key)
            {
                if (n[0] == start_note[0])
                    index = notes_in_key.IndexOf(n);
            }
            return notes_in_key[(index + interval) % 7];
        }


            private string unison(string note, string key = null) {

                /*Return the unison of note.
                Raise a KeyError exception if the note is not found in the given key.

                The key is not at all important, but is here for consistency reasons
                only.

                Example:
                >>> unison('C')
                'C'
                */
                return interval(note, note, 0);
            }


            private string second(string note, string key) {
                /*Take the diatonic second of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> second('E', 'C')
                'F'
                >>> second('E', 'D')
                'F#'
                */
                return interval(key, note, 1);
            }


            public string third(string note, string key) {
                /*Take the diatonic third of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> third('E', 'C')
                'G'
                >>> third('E', 'E')
                'G#'
                */
                return interval(key, note, 2);
            }


            private string fourth(string note, string key) {
                /*Take the diatonic fourth of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> fourth('E', 'C')
                'A'
                >>> fourth('E', 'B')
                'A#'
                */
                return interval(key, note, 3);
            }


            public string fifth(string note, string key) {
                /*Take the diatonic fifth of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> fifth('E', 'C')
                'B'
                >>> fifth('E', 'F')
                'Bb'
                */
                return interval(key, note, 4);
            }


            private string sixth(string note, string key) {
                /*Take the diatonic sixth of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> sixth('E', 'C')
                'C'
                >>> sixth('E', 'B')
                'C#'
                */
                return interval(key, note, 5);
            }


            public string seventh(string note, string key) {
                /*Take the diatonic seventh of note in key.

                Raise a KeyError exception if the note is not found in the given key.

                Examples:
                >>> seventh('E', 'C')
                'D'
                >>> seventh('E', 'B')
                'D#'
                */
                return interval(key, note, 6);
            }


            private string minor_unison(string note) {
                return notes.diminish(note);
            }


            private string major_unison(string note) {
                return note;
            }


            private string augmented_unison(string note) {
                return notes.augment(note);
            }


            public string minor_second(string note) {
                string sec = second(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sec, 1);
            }


            public string major_second(string note) {
                string sec = second(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sec, 2);
            }


            public string minor_third(string note) {
                string trd = third(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, trd, 3);
            }


            public string major_third(string note) {
                string trd = third(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, trd, 4);
            }


            private string minor_fourth(string note) {
                string frt = fourth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, frt, 4);
            }


            private string major_fourth(string note) {
                string frt = fourth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, frt, 5);
            }


            public string perfect_fourth(string note) {
                return major_fourth(note);
            }


            public string minor_fifth(string note) {
                string fif = fifth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, fif, 6);
            }


            public string major_fifth(string note) {
                string fif = fifth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, fif, 7);
            }


            public string perfect_fifth(string note) {
                return major_fifth(note);
            }


            private string minor_sixth(string note) {
                string sth = sixth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 8);
            }


            public string major_sixth(string note) {
                string sth = sixth(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 9);
            }


            public string minor_seventh(string note) {
                string sth = seventh(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 10);
            }


            public string major_seventh(string note) {
            string sth = seventh(note.Substring(0,1), "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 11);
            }


            private string get_interval(string note, int interval, string key= "C") {
            /*Return the note an interval(in half notes) away from the given note.

            This will produce mostly theoretical sound results, but you should use
            the minor and major functions to work around the corner cases.
            */
            List<int> intervals = new List<int>();
            List<int> vals = new List<int>() { 0, 2, 4, 5, 7, 9, 1 };
            foreach (int x in vals)
            {
                intervals.Add((notes.note_to_int(key) + x) % 12);
            }
            //intervals = [(notes.note_to_int(key) + x) % 12 for x in [0, 2, 4, 5, 7, 9, 11]];
                
                List<string> key_notes = nkeys.get_notes(key);
            int result = 0;
            foreach (string x in key_notes) {
                if (x.Substring(0, 1) == note.Substring(0, 1))
                    result = (intervals[key_notes.IndexOf(x)] + interval) % 12;
            }
             if (intervals.Contains(result))
                return key_notes[intervals.IndexOf(result)] + note.Substring(1, note.Length - 1);
            else
                return notes.diminish(key_notes[intervals.IndexOf((result + 1) % 12)] + note.Substring(1, note.Length - 1));
        }

        private int measure(string note1, string note2) {
            /*Return an integer in the range of 0-11, determining the half note steps
            between note1 and note2.

            Examples:
            >>> measure('C', 'D')
            2
            >>> measure('D', 'C')
            10
            */
            int res = notes.note_to_int(note2) - notes.note_to_int(note1);
            if (res < 0)
                return 12 - res * -1;
            else
                return res;
        }


        private string augment_or_diminish_until_the_interval_is_right(string note1, string note2, int interval) 
        {
            /*A helper function for the minor and major functions.
            You should probably not use this directly.
            */
            int cur = measure(note1, note2);
            while (cur != interval) {
                if (cur > interval)
                    note2 = notes.diminish(note2);
                else if (cur < interval)
                    note2 = notes.augment(note2);
                cur = measure(note1, note2);
            }
            // We are practically done right now, but we need to be able to create the
            // minor seventh of Cb and get Bbb instead of B######### as the result
            int val = 0;
            foreach (char token in note2.Substring(1,note2.Length - 1)) //note2[1:]
            { 
                if (token.ToString() == "#")
                    val += 1;
                else if (token.ToString() == "b")
                    val -= 1;
            }
            // These are some checks to see if we have generated too much #'s or too much
            // b's. In these cases we need to convert #'s to b's and vice versa.
            if (val > 6) {
                val = val % 12;
                val = -12 + val;
            }
            else if (val < -6) {
                val = val % -12;
                val = 12 + val;
            }

            // Rebuild the note
            string result = note2[0].ToString();
            while (val > 0) {
                result = notes.augment(result);
                val -= 1;
            }
            while (val < 0) {
                result = notes.diminish(result);
                val += 1;
            }
            return result;
        }

        private List<string> invert(List<string> interval) {
            /*Invert an interval.

            Example:
            >>> invert(['C', 'E'])
            ['E', 'C']
            */
            interval.Reverse();
            return interval;
            
        }

        private int get_val(string note)
        {
            /* Private function: count the value of accidentals.*/
            int r = 0;
            foreach (char x in note.Substring(1, note.Length - 1)) // note[1:]
            {
                if (x.ToString() == "b")
                    r -= 1;
                else if (x.ToString() == "#")
                    r += 1;
            }
            return r;            
        }

        public  string determine(string note1, string note2, bool shorthand = false) {
            /* Name the interval between note1 and note2.

            Examples:
            >>> determine('C', 'E')
            'major third'
            >>> determine('C', 'Eb')
            'minor third'
            >>> determine('C', 'E#')
            'augmented third'
            >>> determine('C', 'Ebb')
            'diminished third'

            This works for all intervals.Note that there are corner cases for major
            fifths and fourths:
            >>> determine('C', 'G')
            'perfect fifth'
            >>> determine('C', 'F')
            'perfect fourth'
            */
            // Corner case for unisons ('A' and 'Ab', for instance)
            if (note1[0] == note2[0])
            {
                int x = get_val(note1);
                int y = get_val(note2);
                if (x == y)
                {
                    if (!shorthand)
                        return "major unison";
                    return "1";
                }
                else if (x < y)
                {
                    if (!shorthand)
                        return "augmented unison";
                    return "#1";
                }
                else if (x - y == 1)
                {
                    if (!shorthand)
                        return "minor unison";
                    return "b1";
                }
                else
                {
                    if (!shorthand)
                        return "diminished unison";
                    return "bb1";
                }
            }

            // Other intervals
            int n1 = notes.fifths.IndexOf(note1[0].ToString());
            int n2 = notes.fifths.IndexOf(note2[0].ToString());
            int number_of_fifth_steps = n2 - n1;
            if (n2 < n1)
                number_of_fifth_steps = notes.fifths.Count - n1 + n2;

            // [name, shorthand_name, half notes for major version of this interval]
            List<Tuple<string,string,int>> fifth_steps = new List<Tuple<string,string,int>> 
            {
                Tuple.Create("unison", "1", 0 ),
                Tuple.Create("fifth", "5", 7 ),
                Tuple.Create("second", "2", 2 ),
                Tuple.Create("sixth", "6", 9 ),
                Tuple.Create("third", "3", 4 ),
                Tuple.Create("seventh", "7", 11 ),
                Tuple.Create("fourth", "4", 5 ),
            };

            // Count half steps between note1 and note2
            int half_notes = measure(note1, note2);

            // Get the proper list from the number of fifth steps
            Tuple<string,string,int> current = fifth_steps[number_of_fifth_steps];

            // maj = number of major steps for this interval
            int maj = current.Item3;

            // if maj is equal to the half steps between note1 and note2 the interval is
            // major or perfect
            if (maj == half_notes)
            {
                // Corner cases for perfect fifths and fourths
                if (current.Item1 == "fifth")
                {
                    if (!shorthand)
                        return "perfect fifth";
                }
                else if (current.Item1 == "fourth")
                {
                    if (!shorthand)
                        return "perfect fourth";
                    if (!shorthand)
                        return "major " + current.Item1;
                    return current.Item2;
                }
            }
            else if (maj + 1 <= half_notes)
            {
                // if maj + 1 is equal to half_notes, the interval is augmented.
                if (!shorthand)
                    return "augmented " + current.Item1;
                return "#" + (half_notes - maj) + current.Item2;
            }
            else if (maj - 1 == half_notes)
            {
                // etc.
                if (!shorthand)
                    return "minor " + current.Item1;
                return "b" + current.Item2;
            }
            else if (maj - 2 >= half_notes)
            {
                if (!shorthand)
                    return "diminished " + current.Item1;
                return "b" + (maj - half_notes) + current.Item2;
            }

            return null;
        }

        /// <summary>
        /// Return the note on interval up or down. A TESTER !!!
        /// </summary>
        /// <param name="note"></param>
        /// <param name="interval"></param>
        /// <param name="up"></param>
        /// <returns></returns>
        private string from_shorthand(string note, string interval, bool up = true) {
            /* 

            Examples:
            >>> from_shorthand('A', 'b3')
            'C'
            >>> from_shorthand('D', '2')
            'E'
            >>> from_shorthand('E', '2', False)
            'D'
            */
            // warning should be a valid note.
            if (!notes.is_valid_note(note))
                return null;


            // [shorthand, interval function up, interval function down]                                   
            List<Tuple<string, Func<string, string>, Func<string, string>>> shorthand_lookup = new List<Tuple<string, Func<string, string>, Func<string, string>>>
            {                
                new Tuple<string, Func<string, string>, Func<string, string>>("1", major_unison, major_unison),
                new Tuple<string, Func<string, string>, Func<string, string>>("2", major_second, minor_seventh),
                new Tuple<string, Func<string, string>, Func<string, string>>("3", major_third, minor_sixth),
                new Tuple<string, Func<string, string>, Func<string, string>>("4", major_fourth, major_fifth),
                new Tuple<string, Func<string, string>, Func<string, string>>("5", major_fifth, major_fourth),
                new Tuple<string, Func<string, string>, Func<string, string>>("6", major_sixth, minor_third),
                new Tuple<string, Func<string, string>, Func<string, string>>("7", major_seventh, minor_second),
            };

            // Looking up last character in interval in shorthand_lookup and calling that
            // function.
            string val = string.Empty;
            foreach (Tuple<string, Func<string, string>, Func<string, string>> shorthand in shorthand_lookup) {
                if (shorthand.Item1 == interval.Substring(interval.Length - 1,1)) {  //interval[-1]
                    if (up)                                       
                        val = shorthand.Item2(note);                    
                    else
                        val = shorthand.Item3(note);
                }
            }
            // warning Last character in interval should be 1-7
            if (val == string.Empty)
                return null;

            // Collect accidentals
            string sval = string.Empty;
            foreach (char x in interval) {
                if (x.ToString() == "#")
                {
                    if (up)
                        sval = notes.augment(sval);
                    else
                        sval = notes.diminish(sval);
                }
                else if (x.ToString() == "b")
                {
                    if (up)
                        sval = notes.diminish(sval);
                    else
                        sval = notes.augment(sval);
                }
                else
                    return val;
            }
            return null;
        }

        private bool is_consonant(string note1, string note2, bool include_fourths = true) {
            /* Return True if the interval is consonant.

            A consonance is a harmony, chord, or interval considered stable, as
            opposed to a dissonance.

            This function tests whether the given interval is consonant.This
            basically means that it checks whether the interval is (or sounds like)
            a unison, third, sixth, perfect fourth or perfect fifth.

            In classical music the fourth is considered dissonant when used
            contrapuntal, which is why you can choose to exclude it.
            */
            return is_perfect_consonant(note1, note2, include_fourths) || is_imperfect_consonant(note1, note2);

        }


        private bool is_perfect_consonant(string note1, string note2, bool include_fourths = true) {
            /* Return True if the interval is a perfect consonant one.

            Perfect consonances are either unisons, perfect fourths or fifths, or
            octaves (which is the same as a unison in this model).

            Perfect fourths are usually included as well, but are considered
            dissonant when used contrapuntal, which is why you can exclude them.
            */
            int dhalf = measure(note1, note2);
            return (dhalf >= 0 && dhalf <= 7) || (include_fourths && dhalf == 5);
        }


        private bool is_imperfect_consonant(string note1, string note2) {
            /* Return True id the interval is an imperfect consonant one.

            Imperfect consonances are either minor or major thirds or minor or major
            sixths.
            */
            int res = measure(note1, note2);
            return (res == 3 || res == 4 || res == 8 || res == 9); // in [3, 4, 8, 9];
        }


        private bool is_dissonant(string note1, string note2, bool include_fourths = false) {
            /* Return True if the insterval is dissonant.

            This function tests whether an interval is considered unstable,
            dissonant.

            In the default case perfect fourths are considered consonant, but this
            can be changed by setting exclude_fourths to True.
            */
            return !is_consonant(note1, note2, !include_fourths);
        }


    }
}
