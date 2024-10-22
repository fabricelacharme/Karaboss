/*
 * Copyright (c) 2024 - Fabrice Lacharme
 * 
 * Attempt to display the BPM of a song
 * 
 * 
 * 
 * 
 * 
 */

using System;
using System.Drawing;
using System.Windows.Media;

namespace Sanford.Multimedia.Midi.Score
{
    public class BpmSymbol
    {
        private int starttime;
        private int bpm;         // BPM value
        private int tempo;
        private int x;           // The x (horizontal) position within the staff 
        private const float kOneMinuteInMicroseconds = 60000000;
        //float ttempo = kOneMinuteInMicroseconds / (float)PerMinute;

        public BpmSymbol(int starttime, int tempo) 
        {
            this.starttime = starttime;
            this.tempo = tempo;
            if (tempo >  0)
                this.bpm = (int)(kOneMinuteInMicroseconds / (float)tempo);

        }

        /** Get the time (in pulses) this symbol occurs at.
         * This is used to determine the measure this symbol belongs to.
        */
        public int StartTime
        {
            get { return starttime; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }


        /** Get/Set the BPM value of this symbol. 
         * 
         */
        public int Tempo
        {
            get { return tempo; }
            set 
            {
                if (value > 0)
                {
                    tempo = value;
                    bpm = (int)(kOneMinuteInMicroseconds / (float)tempo);
                }
            }
        }

        public int BPM
        {
            get { return bpm; }
        }

        /** Draw nothing.
         * @param ytop The ylocation (in pixels) where the top of the staff starts.
         */
        //public void Draw (Graphics g, System.Drawing.Pen pen, int ytop, int tracknum, Rectangle selrect, int OffsetX) 
        

        public override string ToString()
        {
            return string.Format("BpmSymbol starttime={0} x={1} tempo={2} bpm={3}",
                                 starttime, x, tempo, bpm);
        }


    }
}
