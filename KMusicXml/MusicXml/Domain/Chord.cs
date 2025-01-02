using MusicXml.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KMusicXml.MusicXml.Domain
{
    public class Chord
    {
        internal Chord()
        {
            Pitch = new Pitch();
            BassPitch = new Pitch();

            //Notes = GetNotes(Pitch);
        }

        public string Kind { get; internal set; }


        public List<int> Notes { get; internal set; }

        // Time until next chord or end of measure
        public int RemainDuration { get; internal set; }

        /// <summary>
        /// Chord
        /// </summary>
        public Pitch Pitch { get; internal set; }

        /// <summary>
        /// Bass of chord
        /// </summary>
        public Pitch BassPitch { get; internal set; }


        // Return the list of notes composing the chord 
        public List<int> GetNotes() 
        {                        
            List<int> l = new List<int>();
            char root = Pitch.Step;

            switch (Kind)
            {
                case "major":
                    break;
                
                default:
                    break;
            }

            l.Add(root);

            return l;
        
        }
    }
}
