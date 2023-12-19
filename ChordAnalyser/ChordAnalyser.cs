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

        public ChordAnalyser() { }  

        public ChordAnalyser(List<ChordSymbol> chords) 
        {
            //tests t = new tests();

            int x = 0;
            foreach (ChordSymbol chord in chords)
            {
                if (chord.Notes.Count > 1)
                {
                    x++;
                    // Sort notes ascending
                    chord.Notes = SortNotes(chord.Notes);

                    if (IsMajorChord(chord.Notes))
                        Console.WriteLine("Major");
                    else if (IsMinorChord(chord.Notes))
                        Console.WriteLine("Minor");
                    else
                        Console.WriteLine("Unable to guess major or minor");

                    // Transpose to letters
                    List<string> notes = TransposeToLetter(chord.Notes);

                    List<string> res = ch.determine(notes);
                    Console.WriteLine(res[0]);
                    Console.WriteLine(x.ToString()) ;
                }

                


            }
            
            
        }

        private List<string> TransposeToLetter(List<MidiNote> chord)
        {
            List<string> letters = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"};
            List<string> notes = new List<string>();
            int n;
            string l = string.Empty;
            foreach (MidiNote note in chord)
            {
                n = note.Number;
                n = n % 12;
                l = letters[n];
                notes.Add(l);
            }
            
            // BUG... if notes are not in the good order, chords are not recognized
            // NO !!!! notes.Sort();
            return notes;
        }

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
