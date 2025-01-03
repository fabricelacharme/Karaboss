﻿using MusicXml.Domain;
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
            Offset = 0;
        }

        public string Kind { get; internal set; }

        public int Offset { get; internal set; }
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
        public List<int> GetNotes(int notenumber) 
        {            

            List<int> l = new List<int>();
            if (notenumber >= 72)
                notenumber -= 12;

            l.Add(notenumber);
            
            List<int> ln = new List<int>();

            switch (Kind)
            {
                case "augmented":
                    ln = new List<int>() { 4, 8};
                    break;
                case "augmented-seventh":
                    ln = new List<int> { 4, 8, 11, 14};
                    break;
                case "diminished":
                    ln = new List<int>() { 3, 6};
                    break;
                case "diminished-seventh":
                    ln = new List<int> { 3, 6, 9, 10};
                    break;
                case "dominant":
                    ln = new List<int> { 4, 7, 10};
                    break;
                case "dominant-11th":
                    ln = new List<int> { 4, 7, 10, 2, 6};
                    break;
                case "dominant-13th":
                    ln = new List<int> { 4, 7, 10, 2 };
                    break;
                case "dominant-ninth":
                    ln = new List<int> { 4, 7, 10, 2 };
                    break;
                case "French":
                    break;
                case "German":
                    break;
                case "half-diminished":
                    ln = new List<int> { 3, 6, 10};
                    break;
                case "Italian":
                    break;
                case "major":
                    ln = new List<int>() { 4, 7 };
                    break;
                case "major-11th":
                    ln = new List<int> { 4, 7, 11, 2 };
                    break;
                case "major-13th":
                    ln = new List<int> { 4, 7, 11, 2 };
                    break;
                case "major-minor":
                    break;
                case "major-ninth":
                    ln = new List<int> { 4, 7, 11, 2 };
                    break;
                case "major-seventh":
                    ln = new List<int> { 4, 7, 11};
                    break;
                case "major-sixth":
                    ln = new List<int> { 4, 7, 9};
                    break;
                case "minor":
                    ln = new List<int>() { 3, 7 };
                    break;
                case "minor-11th":
                    ln = new List<int> { 3, 7, 10, 2, 6};
                    break;
                case "minor-13th":
                    ln = new List<int> { 3, 7, 10, 2, 6, 9};
                    break;
                case "minor-ninth":
                    ln = new List<int> { 3, 7, 10, 2};
                    break;
                case "minor-seventh":
                    ln = new List<int> { 3, 7, 10};
                    break;
                case "minor-sixth":
                    ln = new List<int> { 3, 7, 9};
                    break;
                case "Neapolitan":
                    break;
                case "none":
                    break;
                case "other":
                    break;
                case "pedal":
                    break;
                case "power":
                    ln = new List<int> { 7 };
                    break;
                case "suspended-fourth":
                    ln = new List<int> { 5, 7 };
                    break;
                case "suspended-second":
                    ln = new List<int> { 2, 7};
                    break;
                case "Tristan":
                    break;
                default:
                    break;
            }

            int n;
            for (int i = 0; i < ln.Count; i++)
            {
                n = ln[i] + notenumber;
                if (n >= 72)
                    n -= 12;
                l.Add(n);
            }

            return l;
        
        }
    }
}