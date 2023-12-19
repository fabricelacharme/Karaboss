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
        //static List<List<MidiNote>> ln = new List<List<MidiNote>>();

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

                    MidiNote[] midiNotes = new MidiNote[chord.Notes.Count];
                    for (int i = 0; i < midiNotes.Length; i++)
                        midiNotes[i] = chord.Notes[i];

                    //List<MidiNote> lnotes = chord.Notes;

                    //lnotes = lnotes.Distinct().ToList();                  

                    // Sort notes ascending
                    //chord.Notes = SortNotes(chord.Notes);


                    ln = new List<MidiNote[]>();                  
                    // Build ln
                    Permute(midiNotes, 0, 2);

                    //Remove duplicates
                    //notes = notes.Distinct().ToList();

                    // Changer les valeurs des notes dans ln
                    ChangeNotesNumber();

                    // Search root note                    
                    List<MidiNote> l = DetermineRoot();

                    if (l != null)
                    {
                        // Transpose to letters
                        List<string> notes = TransposeToLetter(l);

                        List<string> res = ch.determine(notes);
                        Console.WriteLine(res[0]);
                        Console.WriteLine(x.ToString());
                    }
                    else
                    {
                        Console.WriteLine("Chord not found");
                    }
                }                
            }
            
            
        }

        private List<string> TransposeToLetter(List<MidiNote> chord)
        {
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
            List<string> letternotes = new List<string>();
            int n;
            string l = string.Empty;
            foreach (MidiNote note in chord)
            {
                n = note.Number;
                n = n % 12;
                l = letters[n];
                letternotes.Add(l);
            }
            
            return letternotes;
        }


        static void Permute(MidiNote[] arry, int i, int n)
        {            
            int j;
            if (i == n)
            {
                //Console.WriteLine(string.Format("{0} {1} {2}", arry[0].Number, arry[1].Number, arry[2].Number));
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


        private void ChangeNotesNumber()
        {
            foreach(MidiNote[] arry in ln) 
            {
                foreach (MidiNote m in arry)
                {
                    m.Number = m.Number % 12;
                }
            }
        }

        private List<MidiNote> DetermineRoot()
        {
            /* {0,1,2} => 1 - 0, 2 - 0 ET {0,2,1} ????
             * {1,2,0} => 2 - 1, 0 - 1 ET {1,0,2}
             * {2,0,1} => 0 - 2, 1 - 2
             * 
             */

            foreach (MidiNote[] arry in ln)
            {
                List<MidiNote> notes = new List<MidiNote>();
                for (int x = 0; x < arry.Count(); x++)                
                    notes.Add(arry[x]);
                             
                // Il faut mettre le number des notes dans le bon ordre                
                if (IsMajorChord(notes) || IsMinorChord(notes))                    
                    return notes;                    
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

        static bool IsMajorChord(List<MidiNote> notes)
        {
            // Un coup ça marche sans % 12, un coup avec
            // Si les number des notes sont dans le bon ordre, c'est bon 
            if (notes.Count < 3) return false;
            
            // A major chord consists of the root, major third, and perfect fifth
            return (notes[1].Number - notes[0].Number == 4) && (notes[2].Number - notes[0].Number == 7);
        }

        static bool IsMinorChord(List<MidiNote> notes)
        {
            if (notes.Count <3) return false;

            // A minor chord consists of the root, minor third, and perfect fifth
            return (notes[1].Number - notes[0].Number == 3) && (notes[2].Number - notes[0].Number == 7);
        }


    }
}
