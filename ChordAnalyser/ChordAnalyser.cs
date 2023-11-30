using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;


namespace ChordsAnalyser
{
    public class ChordAnalyser
    {

        public ChordAnalyser() { }  

        public ChordAnalyser(List<ChordSymbol> chords) 
        {
            foreach (ChordSymbol chord in chords)
            {
                if (chord.Notes.Count > 1)
                {
                    // Sort notes ascending
                    chord.Notes = SortNotes(chord.Notes);

                    if (IsMajorChord(chord.Notes))
                        Console.WriteLine("Major");
                    else if (IsMinorChord(chord.Notes))
                        Console.WriteLine("Minor");
                    else
                        Console.WriteLine("Unable to guess major or minor");
                }
            }
            
            
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
