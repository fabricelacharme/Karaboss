using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser.cnotes;

namespace ChordsAnalyser.ckeys
{
    
    
    public class nkeys
    {


        readonly List<(string,string)> keys = new List<(string, string)>() {
        ("Cb", "ab" ),  //  7 b
        ("Gb", "eb"),  //  6 b
        ("Db", "bb"),  //  5 b
        ("Ab", "f"),   //  4 b
        ("Eb", "c"),   //  3 b
        ("Bb", "g"),   //  2 b
        ("F", "d"),    //  1 b
        ("C", "a"),    //  nothing
        ("G", "e"),    //  1 //
        ("D", "b"),    //  2 #
        ("A", "f#"),   //  3 #
        ("E", "c#"),   //  4 #
        ("B", "g#"),   //  5 #
        ("F#", "d#"),  //  6 #
        ("C#", "a#"),  //  7 #
        };

        readonly List<string> major_keys = new List<string>() {"Cb","Gb","Db","Ab","Eb","Bb","F","C","G","D","A","E","B","F#","C#" };
        readonly List<string> minor_keys = new List<string>() { "ab", "eb", "bb", "f", "c", "g", "d", "a", "e", "b", "f#", "c#", "g#", "d#", "a#" };

        //static string[] base_scale = { "C", "D", "E", "F", "G", "A", "B" };
        readonly List<string> base_scale = new List<string>() { "C", "D", "E", "F", "G", "A", "B" };

        private Dictionary<string, List<string>> _key_cache = new Dictionary<string, List<string>>();

        /// <summary>
        /// Return True if key is in a recognized format. False if not.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool is_valid_key(string key) {

            foreach ((string, string) couple in keys)
            {
                if (couple.Item1 == key || couple.Item2 == key) 
                    return true;
            }            
            return false;
        }

        /// <summary>
        /// Return the key corrisponding to accidentals.
        /// </summary>
        /// <param name="accidentals"></param>
        /// <returns></returns>
        private (string, string) get_key(int accidentals = 0)
        {
            /*
            Return the tuple containing the major key corrensponding to the
            accidentals put as input, and his relative minor; negative numbers for
            flats, positive numbers for sharps.
            */
            if (accidentals < -7 || accidentals > 7)   //accidentals not in range(-7, 8))
                throw new ArgumentOutOfRangeException("integer not in range (-7)-(+7).");

            return keys[accidentals + 7];
        }

        /// <summary>
        /// Return the key signature.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int get_key_signature(string key = "C") {

            /*
            0 for C or a, negative numbers for flat key signatures, positive numbers
            for sharp key signatures.
            */
            if (!is_valid_key(key))
                throw new FormatException(string.Format("unrecognized format for key '{0}'", key));

            int accidentals;
            foreach ((string, string) couple in keys) {
                if (couple.Item1 == key || couple.Item2 == key)
                {
                    accidentals = keys.IndexOf(couple) - 7;
                    return accidentals;
                }
            }
            return 0;
        }

        /// <summary>
        /// Return the list of accidentals present into the key signature.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        private List<string> get_key_signature_accidentals(string key = "C") {
            
            // fifths : F C G D A EB
            // renvoie une liste genre F#, C#, G# ?
            int accidentals = get_key_signature(key);
            List<string> res = new List<string>();

            if (accidentals < 0)
                for (int i = 0; i < -accidentals; i++)
                {
                    res.Add(string.Format("{0}{1}", reversed(notes.fifths)[i], "b"));
                }
            else if (accidentals > 0) {
                for (int i = 0; i < accidentals; i++) {
                    
                    res.Add(string.Format("{0}{1}", notes.fifths[i], "#"));
                }
            }
            return res;
        }


        private List<string> reversed(List<string> list)
        {
            list.Reverse();
            return list;
        }

        /// <summary>
        /// Return an ordered list of the notes in this natural key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<string> get_notes(string key = "C") {
            /*
            Examples:
            >>> get_notes('F')
            ['F', 'G', 'A', 'Bb', 'C', 'D', 'E']
            >>> get_notes('c')
            ['C', 'D', 'Eb', 'F', 'G', 'Ab', 'Bb']


            // base_scale  { "C", "D", "E", "F", "G", "A", "B" };
            */
            if (_key_cache.ContainsKey(key)) // in _key_cache)
                return _key_cache[key];

            if (!is_valid_key(key))
                throw new FormatException(string.Format("unrecognized format for key '{0}'", key));

            List<string> result = new List<string>();
            string symbol = string.Empty;

            // Calculate notes
            List<string> altered_notes = new List<string>(); 
            foreach (string s in get_key_signature_accidentals(key)) 
            {
                    altered_notes.Add(s.Substring(0, 1)); 
            }
            

            if (get_key_signature(key) < 0)
                symbol = "b";
            else if (get_key_signature(key) > 0)
                symbol = "#";

            int raw_tonic_index = base_scale.IndexOf(key.ToUpper().Substring(0,1));

            // cycle boucle sur { "C", "D", "E", "F", "G", "A", "B" }
            // islice en prend un morceau

            List<string> l = new List<string>(); 
            for (int i = 0; i < base_scale.Count; i++)
            {
                l.Add(base_scale[i]);
            }
            for (int i = 0; i < base_scale.Count; i++)
            {
                l.Add(base_scale[i]);
            }


            for (int i = raw_tonic_index; i < raw_tonic_index + 7; i++)
            {
                string note = l[i];
                if (altered_notes.Contains(note))
                    result.Add(string.Format("{0}{1}", note, symbol));
                else
                    result.Add(note);
            }
            
            // Save result to cache
            _key_cache[key] = result;
            return result;

        }


    }
}
