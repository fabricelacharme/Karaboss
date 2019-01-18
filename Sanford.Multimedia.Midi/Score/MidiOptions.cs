/*
 * Copyright (c) 2007-2011 Madhav Vaidyanathan
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License version 2.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 */
using System.Drawing;

namespace Sanford.Multimedia.Midi
{
    public class MidiOptions
    {
        // The possible values for showNoteLetters
        public const int NoteNameNone = 0;
        public const int NoteNameLetter = 1;
        public const int NoteNameFixedDoReMi = 2;
        public const int NoteNameMovableDoReMi = 3;
        public const int NoteNameFixedNumber = 4;
        public const int NoteNameMovableNumber = 5;

        // Sheet Music Options
        //public string filename;       /** The full Midi filename */
        private bool _bvisible = false;
        public bool bVisible
        {
            get { return _bvisible; }
            set { _bvisible = value; }
        } 

       
        public string title;          /** The Midi song title */
        public bool[] tracks;         /** Which tracks to display (true = display) */
        public bool scrollVert;       /** Whether to scroll vertically or horizontally */
        public bool largeNoteSize;    /** Display large or small note sizes */
        public bool twoStaffs;        /** Combine tracks into two staffs ? */
        public int showNoteLetters;     /** Show the name (A, A#, etc) next to the notes */
        public bool showLyrics;       /** Show the lyrics under each note */
        public bool showMeasures;     /** Show the measure numbers for each staff */
        //public int shifttime;         /** Shift note starttimes by the given amount */


        public int transpose;         /** Shift note key up/down by given amount */

        public int key;               /** Use the given KeySignature (notescale) */
        public TimeSignature time;    /** Use the given time signature */
        public int combineInterval;   /** Combine notes within given time interval (msec) */
        public Color[] colors;        /** The note colors to use */
        public Color shadeColor;      /** The color to use for shading. */
        public Color shade2Color;     /** The color to use for shading the left hand piano */

        // Sound options
        //public bool[] mute;            /** Which tracks to mute (true = mute) */
        public int tempo;              /** The tempo, in microseconds per quarter note */
        public int pauseTime;          /** Start the midi music at the given pause time */
        public int[] instruments;      /** The instruments to use per track */
        public bool useDefaultInstruments;  /** If true, don't change instruments */
        //public bool playMeasuresInLoop;     /** Play the selected measures in a loop */
        //public int playMeasuresInLoopStart; /** Start measure to play in loop */
        //public int playMeasuresInLoopEnd;   /** End measure to play in loop */

        


        public MidiOptions(Sequence sequence1)
        {
            int numtracks = sequence1.tracks.Count;
            tracks = new bool[numtracks];

            useDefaultInstruments = true;
            scrollVert = false;
            largeNoteSize = false;
            
            if (tracks.Length == 1)
            {
                twoStaffs = true;
            }
            else
            {
                twoStaffs = false;
            }
            
            showNoteLetters = NoteNameNone;
            showLyrics = true;
            showMeasures = true;
            //shifttime = 0;

            transpose = 0;            

            key = -1;
            //time = midifile.Time;
            colors = null;
            shadeColor = Color.FromArgb(210, 205, 220);
            shade2Color = Color.FromArgb(80, 100, 250);
            combineInterval = 40;
            //tempo = midifile.Time.Tempo;
            pauseTime = 0;
            //playMeasuresInLoop = false;
            //playMeasuresInLoopStart = 0;
            //playMeasuresInLoopEnd = midifile.EndTime() / midifile.Time.Measure;

        }



    }
}
