using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser.cnotes;
using ChordsAnalyser.ckeys;

namespace ChordAnalyser.cintervals
{
    public class intervals
    {                     
        
        /// <summary>
        /// Return the note found at the interval starting from start_note in the
        ///given key.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        public List<string> interval(string key, string start_note, int interval) 
        {
            /*
            Raise a KeyError exception if start_note is not a valid note.

            Example:
            >>> interval('C', 'D', 1)
            'E'
            */
            notes notes = new notes();
            nkeys nkeys = new nkeys();

            if (!notes.is_valid_note(start_note))                 
                throw new KeyNotFoundException(string.Format("The start note '{0]' is not a valid note", start_note));
            
            List<string> notes_in_key = nkeys.get_notes(key);
            int index;  

            foreach (string n in notes_in_key)
            {
                if (n[0] == start_note[0])
                    index = notes_in_key.IndexOf(n);
            }
            return notes_in_key[(index + interval) % 7];
        }


            private void unison(string note, key = null) {

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


            private void second(note, key) {
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


            private void third(note, key) {
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


            private void fourth(note, key) {
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


            private void fifth(note, key) {
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


            private void sixth(note, key) {
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


            private void seventh(note, key) {
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


            private void minor_unison(note) {
                return notes.diminish(note);
            }


            private void major_unison(note) {
                return note;
            }


            private void augmented_unison(note) {
                return notes.augment(note);
            }


            private void minor_second(note) {
                sec = second(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sec, 1);
            }


            private void major_second(note) {
                sec = second(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sec, 2);
            }


            private void minor_third(note) {
                trd = third(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, trd, 3);
            }


            private void major_third(note) {
                trd = third(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, trd, 4);
            }


            private void minor_fourth(note) {
                frt = fourth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, frt, 4);
            }


            private void major_fourth(note) {
                frt = fourth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, frt, 5);
            }


            private void perfect_fourth(note) {
                return major_fourth(note);
            }


            private void minor_fifth(note) {
                fif = fifth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, fif, 6);
            }


            private void major_fifth(note) {
                fif = fifth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, fif, 7);
            }


            private void perfect_fifth(note) {
                return major_fifth(note);
            }


            private void minor_sixth(note) {
                sth = sixth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 8);
            }


            private void major_sixth(note) {
                sth = sixth(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 9);
            }


            private void minor_seventh(note) {
                sth = seventh(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 10);
            }


            private void major_seventh(note) {
                sth = seventh(note[0], "C");
                return augment_or_diminish_until_the_interval_is_right(note, sth, 11);
            }


            private void get_interval(note, interval, key= "C") {
                /*Return the note an interval(in half notes) away from the given note.

                This will produce mostly theoretical sound results, but you should use
                the minor and major functions to work around the corner cases.
                */
                intervals = [(notes.note_to_int(key) + x) % 12 for x in [0, 2, 4, 5, 7, 9, 11]];
                key_notes = keys.get_notes(key);
                for (x in key_notes)
                        if (x[0] == note[0])
                            result = (intervals[key_notes.index(x)] + interval) % 12;
                if (result in intervals)
            return key_notes[intervals.index(result)] + note[1:];
    else
                return notes.diminish(key_notes[intervals.index((result + 1) % 12)] + note[1:]);
        }

        private void measure(note1, note2) {
            /*Return an integer in the range of 0-11, determining the half note steps
            between note1 and note2.

            Examples:
            >>> measure('C', 'D')
            2
            >>> measure('D', 'C')
            10
            */
            res = notes.note_to_int(note2) - notes.note_to_int(note1);
            if (res < 0)
                return 12 - res * -1;
            else
                return res;
        }


        private void augment_or_diminish_until_the_interval_is_right(note1, note2, interval) {
            /*A helper function for the minor and major functions.

            You should probably not use this directly.
            */
            cur = measure(note1, note2);
            while (cur != interval)
                if (cur > interval)
                    note2 = notes.diminish(note2);
                else if (cur < interval)
                    note2 = notes.augment(note2);
            cur = measure(note1, note2);

            // We are practically done right now, but we need to be able to create the
            // minor seventh of Cb and get Bbb instead of B######### as the result
            val = 0;
            for (token in note2[1:])
                if (token == "#")
                    val += 1;
                else if (token == "b")
                    val -= 1;

            // These are some checks to see if we have generated too much #'s or too much
            // b's. In these cases we need to convert #'s to b's and vice versa.
            if (val > 6)
                val = val % 12;
            val = -12 + val;
    else if (val < -6)
                val = val % -12;
            val = 12 + val;

            // Rebuild the note
            result = note2[0];
            while (val > 0)
                result = notes.augment(result);
            val -= 1;
            while (val < 0)
                result = notes.diminish(result);
            val += 1;
            return result;
        }

        private void invert(interval) {
            /*Invert an interval.

            Example:
            >>> invert(['C', 'E'])
            ['E', 'C']
            */
            interval.reverse();
            res = list(interval);
            interval.reverse();
            return res;
        }

        private int get_val(string note)
        {
            /* Private function: count the value of accidentals.*/
            int r = 0;
            foreach (string x in note[1:])
            {
                if (x == "b")
                    r -= 1;
                else if (x == "#")
                    r += 1;
            }
            return r;            
        }

        private string determine(string note1, string note2, bool shorthand = false) {
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
            n1 = notes.fifths.index(note1[0]);
            n2 = notes.fifths.index(note2[0]);
            number_of_fifth_steps = n2 - n1;
            if (n2 < n1)
                number_of_fifth_steps = len(notes.fifths) - n1 + n2;

            // [name, shorthand_name, half notes for major version of this interval]
            fifth_steps = [
                ["unison", "1", 0],
                ["fifth", "5", 7],
                ["second", "2", 2],
                ["sixth", "6", 9],
                ["third", "3", 4],
                ["seventh", "7", 11],
                ["fourth", "4", 5],
            ];

            // Count half steps between note1 and note2
            half_notes = measure(note1, note2);

            // Get the proper list from the number of fifth steps
            current = fifth_steps[number_of_fifth_steps];

            // maj = number of major steps for this interval
            maj = current[2];

            // if maj is equal to the half steps between note1 and note2 the interval is
            // major or perfect
            if (maj == half_notes)
            {
                // Corner cases for perfect fifths and fourths
                if (current[0] == "fifth")
                {
                    if (!shorthand)
                        return "perfect fifth";
                }
                else if (current[0] == "fourth")
                {
                    if (!shorthand)
                        return "perfect fourth";
                    if (!shorthand)
                        return "major " + current[0];
                    return current[1];
                }
            }
            else if (maj + 1 <= half_notes)
            {
                // if maj + 1 is equal to half_notes, the interval is augmented.
                if (!shorthand)
                    return "augmented " + current[0];
                return "#" * (half_notes - maj) + current[1];
            }
            else if (maj - 1 == half_notes)
            {
                // etc.
                if (!shorthand)
                    return "minor " + current[0];
                return "b" + current[1];
            }
            else if (maj - 2 >= half_notes)
            {
                if (!shorthand)
                    return "diminished " + current[0];
                return "b" * (maj - half_notes) + current[1];
            }
        }

        private bool from_shorthand(string note, string interval, bool up = true) {
            /* Return the note on interval up or down.

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
                return false;

            // [shorthand, interval function up, interval function down]
            shorthand_lookup = [
                ["1", major_unison, major_unison],
                ["2", major_second, minor_seventh],
                ["3", major_third, minor_sixth],
                ["4", major_fourth, major_fifth],
                ["5", major_fifth, major_fourth],
                ["6", major_sixth, minor_third],
                ["7", major_seventh, minor_second],
            ];

            // Looking up last character in interval in shorthand_lookup and calling that
            // function.
            bool val = false;
            foreach (shorthand in shorthand_lookup) {
                if (shorthand[0] == interval[-1]) {
                    if (up)
                        val = shorthand[1](note);
                    else
                        val = shorthand[2](note);
                }
            }
            // warning Last character in interval should be 1-7
            if (val == false)
                return false;

            // Collect accidentals
            foreach (string x in interval) {
                if (x == "#")
                {
                    if (up)
                        val = notes.augment(val);
                    else
                        val = notes.diminish(val);
                }
                else if (x == "b")
                {
                    if (up)
                        val = notes.diminish(val);
                    else
                        val = notes.augment(val);
                }
                else
                    return val;
            }
        }

        private bool is_consonant(int note1, int note2, bool include_fourths = true) {
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


        private bool is_perfect_consonant(int note1, int note2, bool include_fourths = true) {
            /* Return True if the interval is a perfect consonant one.

            Perfect consonances are either unisons, perfect fourths or fifths, or
            octaves (which is the same as a unison in this model).

            Perfect fourths are usually included as well, but are considered
            dissonant when used contrapuntal, which is why you can exclude them.
            */
            dhalf = measure(note1, note2);
            return (dhalf in [0, 7]) || (include_fourths && dhalf == 5);
        }


        private bool is_imperfect_consonant(int note1, int note2) {
            /* Return True id the interval is an imperfect consonant one.

            Imperfect consonances are either minor or major thirds or minor or major
            sixths.
            */
            return measure(note1, note2) in [3, 4, 8, 9];
        }


        private bool is_dissonant(int note1, int note2, bool include_fourths = false) {
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
