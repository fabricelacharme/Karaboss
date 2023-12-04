using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser.cnotes;
using ChordsAnalyser.ckeys;
using ChordsAnalyser.cintervals;
using ChordsAnalyser.cmt_exception;
using System.Security.AccessControl;
using ChordAnalyser.cchords;

namespace ChordAnalyser.cchords
{
    public class chords
    {
        /*
        Module to create chords.

        This module is a huge module that builds on the intervals module. It can be
        used to generate and recognise a plethora of chords.

        The following overview groups some of the functions you are most likely to
        use together.

        Generate Diatonic Chords
         * Triads
           * triad
           * triads
         * Sevenths
           * seventh
           * sevenths

        Generate Absolute Chords
         * Triads
           * minor_triad
           * major_triad
           * diminished_triad
         * Sixths
           * minor_sixth
           * major_sixth
         * Sevenths
           * minor_seventh
           * major_seventh
           * dominant_seventh
           * minor_major_seventh
           * minor_seventh_flat_five
           * diminished_seventh
         * Ninths
           * minor_ninth
           * major_ninth
           * dominant_ninth
         * Elevenths
           * minor_eleventh
           * eleventh
         * Thirteenths
           * minor_thirteenth
           * major_thirteenth
           * dominant_thirteenth
         * Augmented chords
           * augmented_triad
           * augmented_major_seventh
           * augmented_minor_seventh
         * Suspended chords
           * suspended_second_triad
           * suspended_fourth_triad
           * suspended_seventh
           * suspended_fourth_ninth
           * suspended_ninth
         * Altered chords
           * dominant_flat_ninth
           * dominant_sharp_ninth
           * dominant_flat_five
           * sixth_ninth
           * hendrix_chord

        Get Chords by Function
         * Function
           * tonic and tonic7
           * supertonic and supertonic7
           * mediant and mediant7
           * subdominant and subdominant7
           * dominant and dominant7
           * submediant and submediant7
         * Aliases
           * I, II, III, IV, V, VI
           * ii, iii, vi, vii
           * I7, II7, III7, IV7, V7, VI7
           * ii7, iii7, vi7

        Useful Functions
         * determine - Can recognize all the chords that can be generated with \
        from_shorthand (a lot) and their inversions.
         * from_shorthand - Generates chords from shorthand (eg. 'Cmin7')
        */

        intervals intervals = new intervals();
        nkeys nkeys = new nkeys();
        notes notes = new notes();

        chords() 
        {

            init_chordshorhand();
        }
        

        List<List<string>> _triads_cache = new List<List<string>>();

        //# A cache for composed sevenths
        List<Tuple<string, string>> _sevenths_cache = new List<Tuple<string, string>>();

        // Triads Augmented chords Suspended chords Sevenths
        // Sixths Ninths Elevenths Thirteenths Altered
        // Chords Special
        List<Tuple<string, string>> chord_shorthand_meaning = new List<Tuple<string, string>>
        {
            Tuple.Create("m", " minor triad"),
            Tuple.Create("M", " major triad"),
            Tuple.Create("", " major triad"),
            Tuple.Create("dim", " diminished triad"),
            Tuple.Create("aug", " augmented triad"),
            Tuple.Create("+", " augmented triad"),
            Tuple.Create("7#5", " augmented minor seventh"),
            Tuple.Create("M7+5", " augmented minor seventh"),
            Tuple.Create("M7+", " augmented major seventh"),
            Tuple.Create("m7+", " augmented minor seventh"),
            Tuple.Create("7+", " augmented major seventh"),
            Tuple.Create("sus47", " suspended seventh"),
            Tuple.Create("7sus4", " suspended seventh"),
            Tuple.Create("sus4", " suspended fourth triad"),
            Tuple.Create("sus2", " suspended second triad"),
            Tuple.Create("sus", " suspended fourth triad"),
            Tuple.Create("11", " eleventh"),
            Tuple.Create("add11", " eleventh"),
            Tuple.Create("sus4b9", " suspended fourth ninth"),
            Tuple.Create("susb9", " suspended fourth ninth"),
            Tuple.Create("m7", " minor seventh"),
            Tuple.Create("M7", " major seventh"),
            Tuple.Create("dom7", " dominant seventh"),
            Tuple.Create("7", " dominant seventh"),
            Tuple.Create("m7b5", " half diminished seventh"),
            Tuple.Create("dim7", " diminished seventh"),
            Tuple.Create("m/M7", " minor/major seventh"),
            Tuple.Create("mM7", " minor/major seventh"),
            Tuple.Create("m6", " minor sixth"),
            Tuple.Create("M6", " major sixth"),
            Tuple.Create("6", " major sixth"),
            Tuple.Create("6/7", " dominant sixth"),
            Tuple.Create("67", " dominant sixth"),
            Tuple.Create("6/9", " sixth ninth"),
            Tuple.Create("69", " sixth ninth"),
            Tuple.Create("9", " dominant ninth"),
            Tuple.Create("add9", " dominant ninth"),
            Tuple.Create("7b9", " dominant flat ninth"),
            Tuple.Create("7#9", " dominant sharp ninth"),
            Tuple.Create("M9", " major ninth"),
            Tuple.Create("m9", " minor ninth"),
            Tuple.Create("7#11", " lydian dominant seventh"),
            Tuple.Create("m11", " minor eleventh"),
            Tuple.Create("M13", " major thirteenth"),
            Tuple.Create("m13", " minor thirteenth"),
            Tuple.Create("13", " dominant thirteenth"),
            Tuple.Create("add13", " dominant thirteenth"),
            Tuple.Create("7b5", " dominant flat five"),
            Tuple.Create("hendrix", " hendrix chord"),
            Tuple.Create("7b12", " hendrix chord"),
            Tuple.Create("5", " perfect fifth"),
        };

        public List<string> triad(string note, string key) {
            /* Return the triad on note in key as a list.

            Examples:
            >>> triad('E', 'C')
            ['E', 'G', 'B']
            >>> triad('E', 'B')
            ['E', 'G#', 'B']
             */
            return new List<string> { note, intervals.third(note, key), intervals.fifth(note, key) };
        }


        public List<List<string>> triads(string key) {
            /* Return all the triads in key.

            Implemented using a cache.
             */
            if (_triads_cache.Contains(key))
                return _triads_cache[key];

            List<List<string>> res = new List<List<string>>();

            foreach (string x in nkeys.get_notes(key))
            {
                res.Add(triad(x, key));
            }

            //res = [triad(x, key) for x in nkeys.get_notes(key)];

            _triads_cache[key] = res;
            return res;
        }


        public List<string> major_triad(string note) {
            /* Build a major triad on note.

            Example:
            >>> major_triad('C')
            ['C', 'E', 'G']
             */
            return new List<string>() { note, intervals.major_third(note), intervals.perfect_fifth(note) };
        }


        public List<string> minor_triad(string note) {
            /* Build a minor triad on note.

            Example:
            >>> minor_triad('C')
            ['C', 'Eb', 'G']
             */
            return new List<string>() { note, intervals.minor_third(note), intervals.perfect_fifth(note) };
        }


        public List<string> diminished_triad(string note) {
            /* Build a diminished triad on note.

            Example:
            >>> diminished_triad('C')
            ['C', 'Eb', 'Gb']
             */
            return new List<string>() { note, intervals.minor_third(note), intervals.minor_fifth(note) };
        }


        public List<string> augmented_triad(string note) {
            /* Build an augmented triad on note.

            Example:
            >>> augmented_triad('C')
            ['C', 'E', 'G#']
             */
            return new List<string>() { note, intervals.major_third(note), notes.augment(intervals.major_fifth(note)) };
        }


        public List<string> seventh(string note, string key) {
            /* Return the seventh chord on note in key.

            Example:
            >>> seventh('C', 'C')
            ['C', 'E', 'G', 'B']
             */
            List<string> res = triad(note, key);
            res.Add(intervals.seventh(note, key));
            return res; //+ [intervals.seventh(note, key)];
        }


        public List<List<string>> sevenths(string key) {
            /* Return all the sevenths chords in key in a list. */
            if (_sevenths_cache.Contains(key))
                return _sevenths_cache[key];

            List<List<string>> res = new List<List<string>>();
            foreach (string x in nkeys.get_notes(key))
            {
                res.Add(seventh(x, key));
            }
            //res = [seventh(x, key) for x in keys.get_notes(key)];
            _sevenths_cache[key] = res;
            return res;
        }


        public List<string> major_seventh(string note) {
            /* Build a major seventh on note.

            Example:
            >>> major_seventh('C')
            ['C', 'E', 'G', 'B']
             */
            List<string> res = major_triad(note);
            res.Add(intervals.major_seventh(note));
            return res; //major_triad(note) + [intervals.major_seventh(note)];
        }


        public List<string> minor_seventh(string note) {
            /* Build a minor seventh on note.

            Example:
            >>> minor_seventh('C')
            ['C', 'Eb', 'G', 'Bb']
             */
            List<string> res = minor_triad(note);
            res.Add(intervals.minor_seventh(note));
            return res; // minor_triad(note) + [intervals.minor_seventh(note)];
        }


        public List<string> dominant_seventh(string note) {
            /* Build a dominant seventh on note.

            Example:
            >>> dominant_seventh('C')
            ['C', 'E', 'G', 'Bb']
             */
            List<string> res = major_triad(note);
            res.Add(intervals.minor_seventh(note));
            return res; // major_triad(note) + [intervals.minor_seventh(note)];
        }


        public List<string> half_diminished_seventh(string note) {
            /* Build a half diminished seventh(also known as "minor seventh flat
            five") chord on note.

            Example:
            >>> half_diminished_seventh('C')
            ['C', 'Eb', 'Gb', 'Bb']
             */
            List<string> res = diminished_triad(note);
            res.Add(intervals.minor_seventh(note));
            return res; // diminished_triad(note) + [intervals.minor_seventh(note)];
        }


        public List<string> minor_seventh_flat_five(string note) {
            /* Build a minor seventh flat five(also known as "half diminished
            seventh") chord on note.

            See half_diminished_seventh(note) for docs.
             */
            return half_diminished_seventh(note);
        }


        public List<string> diminished_seventh(string note) {
            /* Build a diminished seventh chord on note.

            Example:
            >>> diminished_seventh('C')
            ['C', 'Eb', 'Gb', 'Bbb']
             */
            List<string> res = diminished_triad(note);
            res.Add(notes.diminish(intervals.minor_seventh(note)));
            return res; // diminished_triad(note) + [notes.diminish(intervals.minor_seventh(note))];
        }


        public List<string> minor_major_seventh(string note) {
            /* Build a minor major seventh chord on note.

            Example:
            >>> minor_major_seventh('C')
            ['C', 'Eb', 'G', 'B']
             */
            List<string> res = minor_triad(note);
            res.Add(intervals.major_seventh(note));
            return res; // minor_triad(note) + [intervals.major_seventh(note)];
        }


        public List<string> minor_sixth(string note)
        {
            /* Build a minor sixth chord on note.

            Example:
            >>> minor_sixth('C')
            ['C', 'Eb', 'G', 'A']
             */
            List<string> res = minor_triad(note);
            res.Add(intervals.major_sixth(note));
            return res; // minor_triad(note) + [intervals.major_sixth(note)];
        }


        public List<string> major_sixth(string note) {
            /* Build a major sixth chord on note.

            Example:
            >>> major_sixth('C')
            ['C', 'E', 'G', 'A']
             */
            List<string> res = major_triad(note);
            res.Add(intervals.major_sixth(note));
            return res; // major_triad(note) + [intervals.major_sixth(note)];
        }


        public List<string> dominant_sixth(string note) {
            /* Build the altered chord 6/7 on note.

            Example:
            >>> dominant_sixth('C')
            ['C', 'E', 'G', 'A', 'Bb']
             */
            List<string> res = major_sixth(note);
            res.Add(intervals.minor_seventh(note));
            return res; // major_sixth(note) + [intervals.minor_seventh(note)];
        }


        public List<string> sixth_ninth(string note) {
            /* Build the sixth/ninth chord on note.

            Example:
            >>> sixth_ninth('C')
            ['C', 'E', 'G', 'A', 'D']
             */
            List<string> res = major_sixth(note);
            res.Add(intervals.major_second(note));
            return res; // major_sixth(note) + [intervals.major_second(note)];
        }


        public List<string> minor_ninth(string note) {
            /* Build a minor ninth chord on note.

            Example:
            >>> minor_ninth('C')
            ['C', 'Eb', 'G', 'Bb', 'D']
             */
            List<string> res = minor_seventh(note);
            res.Add(intervals.major_second(note));
            return res; // minor_seventh(note) + [intervals.major_second(note)];
        }


        public List<string> major_ninth(string note) {
            /* Build a major ninth chord on note.

            Example:
            >>> major_ninth('C')
            ['C', 'E', 'G', 'B', 'D']
             */
            List<string> res = major_seventh(note);
            res.Add(intervals.major_second(note));
            return res; // major_seventh(note) + [intervals.major_second(note)];
        }


        public List<string> dominant_ninth(string note) {
            /* Build a dominant ninth chord on note.

            Example:
            >>> dominant_ninth('C')
            ['C', 'E', 'G', 'Bb', 'D']
             */
            List<string> res = dominant_seventh(note);
            res.Add(intervals.major_second(note));
            return res; // dominant_seventh(note) + [intervals.major_second(note)];
        }


        public List<string> dominant_flat_ninth(string note) {
            /* Build a dominant flat ninth chord on note.

            Example:
            >>> dominant_flat_ninth('C')
            ['C', 'E', 'G', 'Bb', 'Db']
             */
            List<string> res = dominant_ninth(note);
            res[4] = intervals.minor_second(note);
            return res;
        }


        public List<string> dominant_sharp_ninth(string note) {
            /* Build a dominant sharp ninth chord on note.

            Example:
            >>> dominant_sharp_ninth('C')
            ['C', 'E', 'G', 'Bb', 'D#']
             */
            List<string> res = dominant_ninth(note);
            res[4] = notes.augment(intervals.major_second(note));
            return res;
        }


        public List<string> eleventh(string note) {
            /* Build an eleventh chord on note.

            Example:
            >>> eleventh('C')
            ['C', 'G', 'Bb', 'F']
             */
            return new List<string>() {
                note,
                intervals.perfect_fifth(note),
                intervals.minor_seventh(note),
                intervals.perfect_fourth(note),
            };
        }


        public List<string> minor_eleventh(string note) {
            /* Build a minor eleventh chord on note.

            Example:
            >>> minor_eleventh('C')
            ['C', 'Eb', 'G', 'Bb', 'F']
             */
            List<string> res = minor_seventh(note);
            res.Add(intervals.perfect_fourth(note));
            return res; // minor_seventh(note) + [intervals.perfect_fourth(note)];
        }


        public List<string> minor_thirteenth(string note) {
            /* Build a minor thirteenth chord on note.

            Example:
            >>> minor_thirteenth('C')
            ['C', 'Eb', 'G', 'Bb', 'D', 'A']
             */
            List<string> res = minor_ninth(note);
            res.Add(intervals.major_sixth(note));
            return res; // minor_ninth(note) + [intervals.major_sixth(note)];
        }


        public List<string> major_thirteenth(string note) {
            /* Build a major thirteenth chord on note.

            Example:
            >>> major_thirteenth('C')
            ['C', 'E', 'G', 'B', 'D', 'A']
             */
            List<string> res = major_ninth(note);
            res.Add(intervals.major_sixth(note));
            return res; // major_ninth(note) + [intervals.major_sixth(note)];
        }


        public List<string> dominant_thirteenth(string note) {
            /* Build a dominant thirteenth chord on note.

            Example:
            >>> dominant_thirteenth('C')
            ['C', 'E', 'G', 'Bb', 'D', 'A']
             */
            List<string> res = dominant_ninth(note);
            res.Add(intervals.major_sixth(note));
            return res; // dominant_ninth(note) + [intervals.major_sixth(note)];
        }


        public List<string> suspended_triad(string note) {
            /* An alias for suspended_fourth_triad. */
            return suspended_fourth_triad(note);
        }


        public List<string> suspended_second_triad(string note) {
            /* Build a suspended second triad on note.

            Example:
            >>> suspended_second_triad('C')
            ['C', 'D', 'G']
             */
            return new List<string>() { note, intervals.major_second(note), intervals.perfect_fifth(note) }; // [note, intervals.major_second(note), intervals.perfect_fifth(note)];
        }


        public List<string> suspended_fourth_triad(string note) {
            /* Build a suspended fourth triad on note.

            Example:
            >>> suspended_fourth_triad('C')
            ['C', 'F', 'G']
             */
            return new List<string>() { note, intervals.perfect_fourth(note), intervals.perfect_fifth(note) };
        }


        public List<string> suspended_seventh(string note) {
            /* Build a suspended(flat) seventh chord on note.

            Example:
            >>> suspended_seventh('C')
            ['C', 'F', 'G', 'Bb']
             */
            List<string> res = suspended_fourth_triad(note);
            res.Add(intervals.minor_seventh(note));
            return res; // suspended_fourth_triad(note) + [intervals.minor_seventh(note)];
        }


        public List<string> suspended_fourth_ninth(string note) {
            /* Build a suspended fourth flat ninth chord on note.

            Example:
            >>> suspended_fourth_ninth('C')
            ['C', 'F', 'G', 'Db']
             */
            List<string> res = suspended_fourth_triad(note);
            res.Add(intervals.minor_second(note));
            return res; // suspended_fourth_triad(note) + [intervals.minor_second(note)];
        }


        public List<string> augmented_major_seventh(string note) {
            /* Build an augmented major seventh chord on note.

            Example:
            >>> augmented_major_seventh('C')
            ['C', 'E', 'G#', 'B']
             */
            List<string> res = augmented_triad(note);
            res.Add(intervals.major_seventh(note));
            return res; // augmented_triad(note) + [intervals.major_seventh(note)];
        }


        public List<string> augmented_minor_seventh(string note) {
            /* Build an augmented minor seventh chord on note.

            Example:
            >>> augmented_minor_seventh('C')
            ['C', 'E', 'G#', 'Bb']
             */
            List<string> res = augmented_triad(note);
            res.Add(intervals.minor_seventh(note));
            return res; // augmented_triad(note) + [intervals.minor_seventh(note)];
        }


        public List<string> dominant_flat_five(string note) {
            /* Build a dominant flat five chord on note.

            Example:
            >>> dominant_flat_five('C')
            ['C', 'E', 'Gb', 'Bb']
             */
            List<string> res = dominant_seventh(note);
            res[2] = notes.diminish(res[2]);
            return res;
        }


        public List<string> lydian_dominant_seventh(string note) {
            /* Build the lydian dominant seventh(7#11) on note.

            Example:
            >>> lydian_dominant_seventh('C')
            ['C', 'E', 'G', 'Bb', 'F#']
             */
            List<string> res = dominant_seventh(note);
            res.Add(notes.augment(intervals.perfect_fourth(note)));
            return res; // dominant_seventh(note) + [notes.augment(intervals.perfect_fourth(note))];
        }


        public List<string> hendrix_chord(string note) {
            /* Build the famous Hendrix chord(7b12).

            Example:
            >>> hendrix_chord('C')
            ['C', 'E', 'G', 'Bb', 'Eb']
             */
            List<string> res = dominant_seventh(note);
            res.Add(intervals.minor_third(note));
            return res; // dominant_seventh(note) + [intervals.minor_third(note)];
        }


        public List<string> tonic(string key) {
            /* Return the tonic chord in key.

            Examples:
            >>> tonic('C')
            ['C', 'E', 'G']
            >>> tonic('c')
            ['C', 'Eb', 'G']
             */
            List<List<string>> res = triads(key);
            return res[0];
        }


        public List<string> tonic7(string key) {
            /* Return the seventh chord in key. */
            return sevenths(key)[0];
        }


        public List<string> supertonic(string key) {
            /* Return the supertonic chord in key.

            Example:
            >>> supertonic('C')
            ['D', 'F', 'A']
             */
            return triads(key)[1];
        }


        public List<string> supertonic7(string key) {
            /* Return the supertonic seventh chord in key. */
            return sevenths(key)[1];
        }


        public List<string> mediant(string key) {
            /* Return the mediant chord in key.

            Example:
            >>> mediant('C')
            ['E', 'G', 'B']
             */
            return triads(key)[2];
        }


        public List<string> mediant7(string key) {
            /* Returns the mediant seventh chord in key. */
            return sevenths(key)[2];
        }


        public List<string> subdominant(string key) {
            /* Return the subdominant chord in key.

            Example:
            >>> subdominant('C')
            ['F', 'A', 'C']
             */
            return triads(key)[3];
        }


        public List<string> subdominant7(string key) {
            /* Return the subdominant seventh chord in key. */
            return sevenths(key)[3];
        }


        public List<string> dominant(string key) {
            /* Return the dominant chord in key.

            Example:
            >>> dominant('C')
            ['G', 'B', 'D']
             */
            return triads(key)[4];
        }


        public List<string> dominant7(string key) {
            /* Return the dominant seventh chord in key. */
            return sevenths(key)[4];
        }


        public List<string> submediant(string key) {
            /* Return the submediant chord in key.

            Example:
            >>> submediant('C')
            ['A', 'C', 'E']
             */
            return triads(key)[5];
        }


        public List<string> submediant7(string key) {
            /* Return the submediant seventh chord in key. */
            return sevenths(key)[5];
        }


        public List<string> subtonic(string key) {
            /* Return the subtonic chord in key.

            Example:
            >>> subtonic('C')
            ['B', 'D', 'F']
             */
            return triads(key)[6];
        }


        public List<string> subtonic7(string key) {
            /* Return the subtonic seventh chord in key. */
            return sevenths(key)[6];
        }


        public List<string> I(string key) {
            return tonic(key);
        }


        public List<string> I7(string key) {
            return tonic7(key);
        }


        public List<string> ii(string key) {
            return supertonic(key);
        }


        public List<string> II(string key) {
            return supertonic(key);
        }


        public List<string> ii7(string key) {
            return supertonic7(key);
        }


        public List<string> II7(string key) {
            return supertonic7(key);
        }


        public List<string> iii(string key) {
            return mediant(key);
        }


        public List<string> III(string key) {
            return mediant(key);
        }


        public List<string> iii7(string key) {
            return mediant7(key);
        }


        public List<string> III7(string key) {
            return mediant7(key);
        }


        public List<string> IV(string key) {
            return subdominant(key);
        }


        public List<string> IV7(string key) {
            return subdominant7(key);
        }


        public List<string> V(string key) {
            return dominant(key);
        }


        public List<string> V7(string key) {
            return dominant7(key);
        }


        public List<string> vi(string key) {
            return submediant(key);
        }


        public List<string> VI(string key) {
            return submediant(key);
        }


        public List<string> vi7(string key) {
            return submediant7(key);
        }


        public List<string> VI7(string key) {
            return submediant7(key);
        }


        public List<string> vii(string key) {
            return subtonic(key);
        }


        public List<string> VII(string key) {
            return subtonic(key);
        }


        public List<string> vii7(string key) {
            return subtonic(key);
        }


        public List<string> VII7(string key) {
            return subtonic7(key);
        }


        public string invert(string chord) {
            /* Invert a given chord one time. */
            return chord.Substring(1, chord.Length - 1) + chord.Substring(0, 1);
            //return chord[1:] + [chord[0]];
        }


        public string first_inversion(string chord) {
            /* Return the first inversion of a chord. */
            return invert(chord);
        }


        public string second_inversion(string chord) {
            /* Return the second inversion of chord. */
            return invert(invert(chord));
        }


        public string third_inversion(string chord) {
            /* Return the third inversion of chord. */
            return invert(invert(invert(chord)));
        }

        /*
        public List<string> from_shorthand(List<string> shorthand_string, string slash = null)
        {

            if (shorthand_string == new List<string>() { "NC", "N.C." })
                return new List<string>();

            
            // warning reduce??            
            // List<string> res = new List<string>();
            // foreach (string x in shorthand_string)
            //    res.Add(from_shorthand(x));
            // return res;
            
            return new List<string>();
        }
        */

        public List<string> from_shorthand(string shorthand_string, string slash = null) {
            /* Take a chord written in shorthand and return the notes in the chord.

            The function can recognize triads, sevenths, sixths, ninths, elevenths,
            thirteenths, slashed chords and a number of altered chords.

            The second argument should not be given and is only used for a recursive
            call when a slashed chord or polychord is found.

            See http://tinyurl.com/3hn6v8u for a nice overview of chord patterns.

            Examples:
            >>> from_shorthand('Amin')
            ['A', 'C', 'E']
            >>> from_shorthand('Am/M7')
            ['A', 'C', 'E', 'G#']
            >>> from_shorthand('A')
            ['A', 'C#', 'E']
            >>> from_shorthand('A/G')
            ['G', 'A', 'C#', 'E']
            >>> from_shorthand('Dm|G')
            ['G', 'B', 'D', 'F', 'A']

            Recognised abbreviations: the letters "m" and "M" in the following
            abbreviations can always be substituted by respectively "min", "mi" or
            "-" and "maj" or "ma".

            Example:
            >>> from_shorthand('Amin7') == from_shorthand('Am7')
            True

            Triads: 'm', 'M' or '', 'dim'

            Sevenths: 'm7', 'M7', '7', 'm7b5', 'dim7', 'm/M7' or 'mM7'

            Augmented chords: 'aug' or '+', '7#5' or 'M7+5', 'M7+', 'm7+', '7+'

            Suspended chords: 'sus4', 'sus2', 'sus47' or '7sus4', 'sus', '11',
            'sus4b9' or 'susb9'

            Sixths: '6', 'm6', 'M6', '6/7' or '67', '6/9' or '69'

            Ninths: '9' or 'add9', 'M9', 'm9', '7b9', '7#9'

            Elevenths: '11' or 'add11', '7#11', 'm11'

            Thirteenths: '13' or 'add13', 'M13', 'm13'

            Altered chords: '7b5', '7b9', '7#9', '67' or '6/7'

            Special: '5', 'NC', 'hendrix'
             */

            /*
            // warning reduce??            
            if (shorthand_string is List<string>)
            {
                List<string> res = new List<string>();
                foreach (string x in shorthand_string)
                    res.Add(from_shorthand(x));
                return res;
            }

            if (shorthand_string == new List<string>() { "NC", "N.C." })
                return new List<string>();
            */

            // Shrink shorthand_string to a format recognised by chord_shorthand
            shorthand_string = shorthand_string.Replace("min", "m");
            shorthand_string = shorthand_string.Replace("mi", "m");
            shorthand_string = shorthand_string.Replace("-", "m");
            shorthand_string = shorthand_string.Replace("maj", "M");
            shorthand_string = shorthand_string.Replace("ma", "M");

            // Get the note name
            if (!notes.is_valid_note(shorthand_string[0].ToString()))
                throw new FormatException(string.Format("Unrecognised note '{0}' in chord '{1}'", shorthand_string[0], shorthand_string));

            string name = shorthand_string[0].ToString();

            // Look for accidentals
            foreach (char n in shorthand_string.Substring(1, shorthand_string.Length - 1))
            {
                if (n.ToString() == "#")
                    name += n;
                else if (n.ToString() == "b")
                    name += n;
                else
                    break;
            }
            // Look for slashes and polychords '|'
            int slash_index = -1;
            int s = 0;
            string rest_of_string = shorthand_string.Substring(name.Length, shorthand_string.Length - name.Length); // [name.Length :];

            foreach (char n in rest_of_string)
            {
                if (n.ToString() == "/")
                    slash_index = s;
                else if (n.ToString() == "|")
                {
                    // Generate polychord
                    string a = shorthand_string.Substring(0, name.Length + s);
                    string b = shorthand_string.Substring(name.Length + s + 1, shorthand_string.Length - (name.Length + s + 1));
                    List<string> c = from_shorthand(a);
                    c.Add(b);
                    return c; //  [name.Length + s + 1 :]),);
                }
                s += 1;
            }

            // Generate slash chord
            if (slash_index != -1 && !new List<string> { "m/M7", "6/9", "6/7" }.Contains(rest_of_string))
            {
                string res = shorthand_string.Substring(0, name.Length + slash_index); //   [: len(name) + slash_index];
                return from_shorthand(shorthand_string.Substring(0, name.Length + slash_index), shorthand_string.Substring(name.Length + slash_index + 1, shorthand_string.Length - (name.Length + slash_index + 1)));  //[name.Length + slash_index + 1 :],);
            }

            int shorthand_start = name.Length;

            string short_chord = shorthand_string.Substring(shorthand_start, shorthand_string.Length - shorthand_start); //[shorthand_start:];
            if (chord_shorthand.ContainsKey(short_chord))
            {

                string res = chord_shorthand[short_chord](name);
                if (slash != null)
                {
                    // Add slashed chords
                    if (slash is string)
                    {
                        if (notes.is_valid_note(slash))
                            res = slash + res;
                        else
                            throw new FormatException(string.Format("Unrecognised note '{0}' in slash chord'{1}'", slash, slash + shorthand_string));
                    }

                    return new List<string> { res };
                }
                else
                    throw new FormatException(string.Format("Unknown shorthand: {0]", shorthand_string));
            }
        }


        public List<string> determine(List<string> chord, bool shorthand = false, bool no_inversions = false, bool no_polychords = false) {
            /* Name a chord.

            This function can determine almost every chord, from a simple triad to a
            fourteen note polychord. */
            if (chord == new List<string>())
                return new List<string>();
            else if (chord.Count == 1)
                return chord;
            else if (chord.Count == 2)
                return [intervals.determine(chord[0], chord[1])];
            else if (chord.Count == 3)
                return determine_triad(chord, shorthand, no_inversions, no_polychords);
            else if (chord.Count == 4)
                return determine_seventh(chord, shorthand, no_inversions, no_polychords);
            else if (chord.Count == 5)
                return determine_extended_chord5(chord, shorthand, no_inversions, no_polychords);
            else if (chord.Count == 6)
                return determine_extended_chord6(chord, shorthand, no_inversions, no_polychords);
            else if (chord.Count == 7)
                return determine_extended_chord7(chord, shorthand, no_inversions, no_polychords);
            else
                return determine_polychords(chord, shorthand);
        }

        public List<string> determine_triad(List<string> triad, bool shorthand = false, bool no_inversions = false, string placeholder = null) {
            /* Name the triad; return answers in a list.

            The third argument should not be given. If shorthand is True the answers
            will be in abbreviated form.

            This function can determine major, minor, diminished and suspended
            triads. Also knows about invertions.

            Examples:
            >>> determine_triad(['A', 'C', 'E'])
            ['A minor triad', 'C major sixth, second inversion']
            >>> determine_triad(['C', 'E', 'A'])
            ['C major sixth', 'A minor triad, first inversion']
            >>> determine_triad(['A', 'C', 'E'], True)
            ['Am', 'CM6']
             
            if (triad.Count != 3)
                //# warning: raise exception: not a triad
                return false;
            */
            return triad.Count == 3;
        }

        private List<string> add_result1(int tries, List<string> triad)
        {
            result.Add((tries, triad[0]));
        }

        public List<string> inversion_exhauster(List<string> triad, bool shorthand, int tries, result)
        {
            /* Run tries every inversion and save the result. */
            string intval1 = intervals.determine(triad[0], triad[1], true);
            string intval2 = intervals.determine(triad[0], triad[2], true);

            List<string> result = new List<string>();

            string intval = intval1 + intval2;
            if (intval == "25")
                add_result1("sus2");
            else if (intval == "3b7")
                add_result("dom7");  //# changed from just '7'
            else if (intval == "3b5")
                add_result1("7b5");  //# why not b5?
            else if (intval == "35")
                add_result1("M");
            else if (intval == "3#5")
                add_result1("aug");
            else if (intval == "36")
                add_result1("M6");
            else if (intval == "37")
                add_result1("M7");
            else if (intval == "b3b5")
                add_result1("dim");
            else if (intval == "b35")
                add_result1("m");
            else if (intval == "b36")
                add_result1("m6");
            else if (intval == "b3b7")
                add_result1("m7");
            else if (intval == "b37")
                add_result1("m/M7");
            else if (intval == "45")
                add_result1("sus4");
            else if (intval == "5b7")
                add_result1("m7");
            else if (intval == "57")
                add_result1("M7");

            if (tries != 3 && !no_inversions)
                return inversion_exhauster([triad[-1]] + triad[:-1], shorthand, tries + 1, result);
            else
            {
                List<string> res = new List<string>();
                foreach (string r in result)
                {
                    if (shorthand)
                        res.Add(r[2] + r[0]);
                    else
                        res.Add(r[2] + chord_shorthand_meaning[r[0]] + int_desc(r[1]));
                }
                return res;
            }

            return inversion_exhauster(triad, shorthand, 1, []);
        }


        public List<string> determine_seventh(List<string> seventh, bool shorthand = false, bool no_inversion = false, bool no_polychords = false)
        {
            /* Determine the type of seventh chord; return the results in a list,
            ordered on inversions.

            This function expects seventh to be a list of 4 notes.

            If shorthand is set to True, results will be returned in chord shorthand
            ('Cmin7', etc.); inversions will be dropped in that case.

            Example:
            >>> determine_seventh(['C', 'E', 'G', 'B'])
            ['C major seventh', 'Em|CM']
             */
            if (seventh.Count != 4)
                //# warning raise exception: seventh chord is not a seventh chord
                return null;

            List<string> inversion_exhauster(List<string> seventh, bool shorthand, int tries, List<string> result, List<string> polychords)
            {
                /* Determine sevenths recursive functions. */
                //# Check whether the first three notes of seventh are part of some triad.

                List<string> l = new List<string>();
                l.Add(seventh[0]);
                l.Add(seventh[1]);
                l.Add(seventh[2]);

                List<string> triads = determine_triad(l, true, true);

                // Get the interval between the first and last note
                string intval3 = intervals.determine(seventh[0], seventh[3]);

                void add_result(bool poly = false) {
                    /* Helper function. */
                    result.Add((tries, seventh[0], poly));
                }

                // Recognizing polychords
                if (tries == 1 && !no_polychords)
                    polychords = polychords + determine_polychords(seventh[0], shorthand);

                // Recognizing sevenths
                foreach (string triad in triads)
                {
                    // Basic triads
                    string triad = triad.Substring(1, triad.Length - 1); //[len(seventh[0]) :];
                    if (triad == "m") {
                        if (intval3 == "minor seventh")
                            add_result("m7");
                        else if (intval3 == "major seventh")
                            add_result("m/M7");
                        else if (intval3 == "major sixth")
                            add_result("m6");
                    }
                    else if (triad == "M")
                    {
                        if (intval3 == "major seventh")
                            add_result("M7");
                        else if (intval3 == "minor seventh")
                            add_result("7");
                        else if (intval3 == "major sixth")
                            add_result("M6");
                    }
                    else if (triad == "dim")
                    {
                        if (intval3 == "minor seventh")
                            add_result("m7b5");
                        else if (intval3 == "diminished seventh")
                            add_result("dim7");
                    }
                    else if (triad == "aug") {
                        if (intval3 == "minor seventh")
                            add_result("m7+");
                        if (intval3 == "major seventh")
                            add_result("M7+");
                    }
                    else if (triad == "sus4")
                    {
                        if intval3 == "minor seventh")
                            add_result("sus47");
                        else if (intval3 == "minor second")
                            add_result("sus4b9");
                    }
                    else if (triad == "m7") {
                        // Other
                        if (intval3 == "perfect fourth")
                            add_result("11");
                    }
                    else if (triad == "7b5")
                    {
                        if (intval3 == "minor seventh")
                            add_result("7b5");
                    }
                }

                if (tries != 4 && !no_inversion) {

                    List<string> ll = new List<string>();
                    ll.Add(seventh[seventh.Count - 1]);
                    seventh.RemoveAt(seventh.Count - 1);
                    ll.AddRange(seventh);

                    return inversion_exhauster(ll, shorthand, tries + 1, result, polychords);
                }
                else
                {
                    //# Return results
                    List<string> res = new List<string>();

                    // Reset seventh
                    List<string> ll = new List<string>();
                    ll.Add(seventh[3]);
                    ll.Add(seventh[0]);
                    ll.Add(seventh[1]);
                    ll.Add(seventh[2]);
                    seventh = ll;
                    foreach (string x in result) {
                        if (shorthand)
                            res.Add(x[2] + x[0]);
                        else
                            res.Add(x[2] + chord_shorthand_meaning[x[0]] + int_desc(x[1]));
                    }
                    return res + polychords;
                }
            }

            return inversion_exhauster(seventh, shorthand, 1, new List<string>(), new List<string>());
        }

        public List<string> determine_extended_chord5(string chord, bool shorthand = false, bool no_inversions = false, bool no_polychords = false)
        {
            /* Determine the names of an extended chord. */
            if (chord.Length != 5)
                // warning raise exeption: not an extended chord
                return null;


            List<string> inversion_exhauster(string chord, bool shorthand, int tries, List<string> result, List<string> polychords)
            {
                /* Recursive helper function. */

                void add_result()
                {
                    result.Add((tries, chord[0]));
                }

                List<string> triads = determine_triad(chord.Substring(0, 2), true, true);
                List<string> sevenths = determine_seventh(chord.Substring(0, 3), true, true, true);

                // Determine polychords
                if (tries == 1 && !no_polychords)
                    polychords += determine_polychords(chord, shorthand);


                string intval4 = intervals.determine(chord[0], chord[4]);

                foreach (string sseventh in sevenths)
                {
                    string seventh = sseventh.Substring(1, sseventh.Length - 1);   //[len(chord[0]) :]
                    if (seventh == "M7")
                    {
                        if (intval4 == "major second")
                            add_result("M9");
                    }
                    else if (seventh == "m7") {
                        if (intval4 == "major second")
                            add_result("m9");
                    }
                    else if (intval4 == "perfect fourth")
                        add_result("m11");
                    else if (seventh == "7") {
                        if (intval4 == "major second")
                            add_result("9");
                        else if (intval4 == "minor second")
                            add_result("7b9");
                        else if (intval4 == "augmented second")
                            add_result("7#9");
                        else if (intval4 == "minor third")
                            add_result("7b12");
                        else if (intval4 == "augmented fourth")
                            add_result("7#11");
                        else if (intval4 == "major sixth")
                            add_result("13");
                    }
                    else if (seventh == "M6") {
                        if (intval4 == "major second")
                            add_result("6/9");
                        else if (intval4 == "minor seventh")
                            add_result("6/7");
                    }
                }


                if (tries != 5 && !no_inversions)
                    return inversion_exhauster([chord[-1]] + chord[:-1], shorthand, tries + 1, result, polychords);
            else
            {
                List<string> res = new List<string>();
                foreach (string r in result) {
                    if (shorthand)
                        res.Add(r[2] + r[0]);
                    else
                        res.Add(r[2] + chord_shorthand_meaning[r[0]] + int_desc(r[1]));
                }
                return res + polychords;
            }

            return inversion_exhauster(chord, shorthand, 1, [], []);
        }

        public List<string> determine_extended_chord6(string chord, bool shorthand = false, bool no_inversions = false, bool no_polychords = false)
        {
            /* Determine the names of an 6 note chord. */
            if (chord.Length != 6)
                //# warning raise exeption: not an extended chord
                return null;

            List<string> inversion_exhauster(string chord, bool shorthand, int tries, List<string> result, List<string> polychords)
            {
                /* Recursive helper function */

                // Determine polychords
                if (tries == 1 && !no_polychords)
                    polychords += determine_polychords(chord, shorthand);

                void add_result()
                {
                    result.Add((tries, chord[0].ToString()));
                }

                List<string> ch = determine_extended_chord5(chord.Substring(0, 4), true, true, true);
                string intval5 = intervals.determine(chord[0], chord[5]);



                foreach (string c in ch)
                {
                    c = c.Substring(1, c.Length - 1); //[len(chord[0]) :]
                    if (c == "9") {
                        if (intval5 == "perfect fourth")
                            add_result("11");
                        else if (intval5 == "augmented fourth")
                            add_result("7#11");
                        else if (intval5 == "major sixth")
                            add_result("13");
                    }
                    else if (c == "m9") {
                        if intval5 == "perfect fourth")
                            add_result("m11");
                        else if (intval5 == "major sixth")
                            add_result("m13");
                    }
                    else if (c == "M9")
                    {
                        if (intval5 == "perfect fourth")
                            add_result("M11");
                        else if (intval5 == "major sixth")
                            add_result("M13");
                    }
                }

                if (tries != 6 && !no_inversions)
                    return inversion_exhauster(chord.Substring(chord.Length, 1) + chord.Substring(0, chord.Length - 1), shorthand, tries + 1, result, polychords);
                else
                {
                    List<string> res = new List<string>();
                    foreach (string r in result)
                    {
                        if (shorthand)
                            res.Add(r[2] + r[0]);
                        else
                            res.Add(r[2] + chord_shorthand_meaning[r[0]] + int_desc(r[1]));
                    }
                    return res + polychords;
                }
            }

            return inversion_exhauster(chord, shorthand, 1, [], []);
        }


        public List<string> determine_extended_chord7(string chord, bool shorthand = false, bool no_inversions = false, bool no_polychords = false) {
            /* Determine the names of an 7 note chord. */
            if (chord.Length != 7)
                //# warning raise exeption: not an extended chord
                return null;

            List<string> inversion_exhauster(string chord, bool shorthand, int tries, List<string> result, bool polychords)
            {
                /* Recursive helper function. */
                // Determine polychords
                if (tries == 1 && !no_polychords)
                    polychords += determine_polychords(chord, shorthand);

                void add_result()
                {
                    result.Add((tries, chord[0]));
                }

                List<string> ch = determine_extended_chord6(chord.Substring(0, 5), true, true, true);
                string intval6 = intervals.determine(chord[0].ToString(), chord[6].ToString());

                foreach (string c in ch) {
                    c = c.Substring(1, c.Length - 1); // [len(chord[0]) :];
                    if (c == "11") {
                        if (intval6 == "major sixth")
                            add_result("13");
                    }
                    else if (c == "m11") {
                        if (intval6 == "major sixth")
                            add_result("m13");
                    }
                    else if (c == "M11") {
                        if (intval6 == "major sixth")
                            add_result("M13");
                    }
                }

                if (tries != 6)
                    return inversion_exhauster(chord(chord.Length, 1) + chord.Substring(0, chord.Length - 1), shorthand, tries + 1, result, polychords);
                else
                {
                    List<string> res = new List<string>;

                    foreach (string r in result)
                    {
                        if (shorthand)
                            res.append([2] + r[0]);
                        else
                            res.append(r[2] + chord_shorthand_meaning[r[0]] + int_desc(r[1]));
                    }
                }
                return res + polychords;
            }

            return inversion_exhauster(chord, shorthand, 1, [], []);
        }

        public string int_desc(int tries)
        {
            /* Return the inversion of the triad in a string. */
            if (tries == 1)
                return "";
            else if (tries == 2)
                return ", first inversion";
            else if (tries == 3)
                return ", second inversion";
            else if (tries == 4)
                return ", third inversion";
            else
                return null;
        }

        public List<string> determine_polychords(string chord, bool shorthand = false)
        {
            /* Determine the polychords in chord.

            This function can handle anything from polychords based on two triads to
            6 note extended chords.
             */
            List<string> polychords = new List<string>;
            List<Func> function_list = new List<Func> {
            determine_triad,
            determine_seventh,
            determine_extended_chord5,
            determine_extended_chord6,
            determine_extended_chord7,
        };

            // Range tracking.
            if (chord.Length <= 3)
                return [];
            else if (chord.Length > 14)
                return [];
            else if (chord.Length - 3 <= 5)
                function_nr = list(range(0, chord.Length - 3));
            else
                function_nr = list(range(0, 5));
            for (f in function_nr)
            {
                for (f2 in function_nr)
                {
                    /*
                    # The clever part: Try the function_list[f] on the len(chord) - (3 +
                    # f) last notes of the chord. Then try the function_list[f2] on the
                    # f2 + 3 first notes of the chord. Thus, trying all possible
                    # combinations.
                    */
                    //for ( chord1 in function_list[f] (chord[len(chord) - (3 + f) :], true, true, true) )
                    for (chord1 in function_list[f])
                    {
                        //for (chord2 in function_list[f2](chord[: f2 + 3], True, True, True)
                        for (chord2 in function_list[f2])
                        {
                            polychords.append("%s|%s" % (chord1, chord2));
                            if (shorthand)
                            {
                                for (p in polychords)
                                    p = p + " polychord";
                            }
                        }
                    }
                }
            }
            return polychords;
        }


        // A dictionairy that can be used to present chord abbreviations. This
        // dictionairy is also used in from_shorthand()
        // Triads Augmented chords Suspended chords Sevenths Sixths
        // Ninths Elevenths Thirteenths Altered Chords Special

        Dictionary<string, Func<string, List<string>>> chord_shorthand = new Dictionary<string, Func<string, List<string>>>();


        private void init_chordshorhand ()
        {

            //chord_shorthand.Add("toto", hendrix_chord);
            //chord_shorthand["m"] = minor_triad;

            chord_shorthand["m"] = minor_triad;
            chord_shorthand["M"] = major_triad;
            chord_shorthand[""] = major_triad;
            chord_shorthand["dim"] = diminished_triad;
            chord_shorthand["aug"] = augmented_triad;
            chord_shorthand["+"] = augmented_triad;
            chord_shorthand["7#5"] = augmented_minor_seventh;
            chord_shorthand["M7+5"] = augmented_minor_seventh;
            chord_shorthand["M7+"] = augmented_major_seventh;
            chord_shorthand["m7+"] = augmented_minor_seventh;
            chord_shorthand["7+"] = augmented_major_seventh;
            chord_shorthand["sus47"] = suspended_seventh;
            chord_shorthand["sus4"] = suspended_fourth_triad;
            chord_shorthand["sus2"] = suspended_second_triad;
            chord_shorthand["sus"] = suspended_triad;
            chord_shorthand["11"] = eleventh;
            chord_shorthand["sus4b9"] = suspended_fourth_ninth;
            chord_shorthand["susb9"] = suspended_fourth_ninth;
            chord_shorthand["m7"] = minor_seventh;
            chord_shorthand["M7"] = major_seventh;
            chord_shorthand["7"] = dominant_seventh;
            chord_shorthand["dom7"] = dominant_seventh;
            chord_shorthand["m7b5"] = minor_seventh_flat_five;
            chord_shorthand["dim7"] = diminished_seventh;
            chord_shorthand["m/M7"] = minor_major_seventh;
            chord_shorthand["mM7"] = minor_major_seventh;
            chord_shorthand["m6"] = minor_sixth;
            chord_shorthand["M6"] = major_sixth;
            chord_shorthand["6"] = major_sixth;
            chord_shorthand["6/7"] = dominant_sixth;
            chord_shorthand["67"] = dominant_sixth;
            chord_shorthand["6/9"] = sixth_ninth;
            chord_shorthand["69"] = sixth_ninth;
            chord_shorthand["9"] = dominant_ninth;
            chord_shorthand["7b9"] = dominant_flat_ninth;
            chord_shorthand["7#9"] = dominant_sharp_ninth;
            chord_shorthand["M9"] = major_ninth;
            chord_shorthand["m9"] = minor_ninth;
            chord_shorthand["7#11"] = lydian_dominant_seventh;
            chord_shorthand["m11"] = minor_eleventh;
            chord_shorthand["M13"] = major_thirteenth;
            chord_shorthand["m13"] = minor_thirteenth;
            chord_shorthand["13"] = dominant_thirteenth;
            chord_shorthand["7b5"] = dominant_flat_five;
            chord_shorthand["hendrix"] = hendrix_chord;
            chord_shorthand["7b12"] = hendrix_chord";
            //chord_shorthand["5"] = "lambda x: [x, intervals.perfect_fifth(x)"


        }
    }
}
