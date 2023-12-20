#region license
/*
 * Based on https://github.com/bspaans/python-mingus/
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChordsAnalyser;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;
using ChordsAnalyser.cchords;

namespace ChordsAnalyser
{
    public class ChordAnalyser
    {

        cchords.chords ch = new cchords.chords();

        static List<MidiNote[]> ln = new List<MidiNote[]>();        

        public ChordAnalyser() { }  

        public ChordAnalyser(List<ChordSymbol> chords) 
        {
            //tests t = new tests();

            int x = 0;
            foreach (ChordSymbol chord in chords)
            {
                if (chord.Notes.Count >= 3)
                {
                    x++;

                    // Sort notes ascending in each chord
                    chord.Notes = SortNotes(chord.Notes);
                    
                    

                    // Create a list only for permutations
                    MidiNote[] midiNotes = new MidiNote[chord.Notes.Count];
                    for (int i = 0; i < midiNotes.Length; i++)
                        midiNotes[i] = chord.Notes[i];

                    //lnotes = lnotes.Distinct().ToList();                  
                  

                    ln = new List<MidiNote[]>();                  
                    // Build ln = list of all combinations
                    Permute(midiNotes, 0, 2);

                    List<List<int>> notes = new List<List<int>>();
                    foreach (MidiNote[] arry in ln)
                    {
                        List<int> lll = new List<int>();
                        for (int i =0; i < arry.Length; i++)
                        {
                            lll.Add(arry[i].Number);
                        }
                        notes.Add(lll);
                    }

                    //Remove duplicates
                    notes = RemoveDoubles(notes);

                    // Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
                    notes = ChangeNotesNumber(notes);

                    // Search root note                    
                    List<int> lroot = DetermineRoot(notes);

                    if (lroot != null)
                    {
                        // Transpose to letters
                        List<string> notesletters = TransposeToLetter(lroot);

                        List<string> res = ch.determine(notesletters);

                        if (res.Count > 0)
                        {
                            Console.WriteLine(res[0]);
                            Console.WriteLine(x.ToString());
                        }
                        else
                        {
                            Console.WriteLine("ERREUR : Determine n'a pas trouvé d'accord");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Chord not found");
                    }
                }                
            }
            
            
        }


        /// <summary>
        /// Remove a note if in double inside a chord
        /// If two C exists, remove one
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<List<int>> RemoveDoubles(List<List<int>> lschords)
        {
            List<List<int>> res = new List<List<int>>();
            int n = 0;
            for (int j = 0; j < lschords.Count; j++)
            {
                List<int> lsnotes = lschords[j];
                for (int i = 0; i < lsnotes.Count; i++)                
                    lsnotes[i] = lsnotes[i] % 12;
                
                lsnotes = lsnotes.Distinct().ToList();
                res.Add(lsnotes);
            }

            return res;
        }

        /// <summary>
        /// Retrieve note letter
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<string> TransposeToLetter(List<int> notes)
        {
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
            List<string> letternotes = new List<string>();            
            string l = string.Empty;
            int x = 0;
            foreach (int n in notes)
            {                
                x = n % 12;
                l = letters[x];
                letternotes.Add(l);
            }
            
            return letternotes;
        }



        /// <summary>
        /// Get all combinations of a set of values
        /// </summary>
        /// <param name="arry"></param>
        /// <param name="i"></param>
        /// <param name="n"></param>
        static void Permute(MidiNote[] arry, int i, int n)
        {            
            int j;
            if (i == n)
            {                
                MidiNote[] m = new MidiNote[arry.Count()];
                for (int x = 0; x < arry.Count();x++)
                    m[x] = arry[x];
                ln.Add(m);
            }
            else
            {
                for (j = i; j <= n; j++)
                {
                    Swap(ref arry[i], ref arry[j]);
                    Permute(arry, i + 1, n);
                    Swap(ref arry[i], ref arry[j]); //backtrack
                }
            }            
        }

        static void Swap(ref MidiNote a, ref MidiNote b)
        {
            MidiNote tmp;
            tmp = a;
            a = b;
            b = tmp;
        }

        /// <summary>
        /// Minor the value of the notes of a chord and ensure that each note has a value greater than the previous one.
        /// </summary>
        /// <param name="ll"></param>
        /// <returns></returns>
        private List<List<int>> ChangeNotesNumber(List<List<int>> ll)
        {            
            int n;
            int prevnumber = 0;
            List<List<int>> res = new List<List<int>>();

            for (int j = 0; j < ll.Count; j++)          
            {               
                List<int> lsnotes = ll[j];                
                int t = lsnotes[0];
                t = t % 12;
                lsnotes[0] = t;
                prevnumber = t;

                for (int i = 1; i < lsnotes.Count; i++)
                {
                    n = lsnotes[i] % 12;
                    if (n < prevnumber)
                    {
                        while (n < prevnumber)
                            n += 12;
                    }
                    lsnotes[i] = n;
                    prevnumber = n;
                }
                res.Add(lsnotes);
            }
            return res;
        }


        /// <summary>
        /// Select the chord existing in the proposed list
        /// </summary>
        /// <param name="lsnotes"></param>
        /// <returns></returns>
        private List<int> DetermineRoot(List<List<int>> lsnotes)
        {
            /* {0,1,2} => 1 - 0, 2 - 0 ET {0,2,1} ????
             * {1,2,0} => 2 - 1, 0 - 1 ET {1,0,2}
             * {2,0,1} => 0 - 2, 1 - 2
             * 
             */

            foreach (List<int> chord in lsnotes)
            {                                          
                // this test needs that chord have only 3 notes
                if (IsMajorChord(chord) || IsMinorChord(chord))                    
                    return chord;                    
            }
            return null;  
        }

        List<MidiNote> Rotate(List<MidiNote> notes)
        {
            MidiNote first = notes[0];
            notes.RemoveAt(0);
            notes.Add(first);

            return notes;

        }

        /// <summary>
        /// Sort Midi notes by number
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        private List<MidiNote> SortNotes(List<MidiNote> notes)
        {
            List<MidiNote> l = new List<MidiNote>();
            var res = from n in notes
                        orderby n.Number
                        ascending
                        select n;
            foreach (var x in res)
            {
                l.Add(x);
            }
            return l;
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
            if (notes.Count <3) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1] - notes[0] == 3) && (notes[2] - notes[0] == 7);
        }


    }
}
