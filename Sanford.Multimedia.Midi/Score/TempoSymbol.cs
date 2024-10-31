/*
 * Copyright (c) 2024 - Fabrice Lacharme
 * 
 * Attempt to display the Tempo/BPM of a song
 * 
 * 
 * 
 * 
 * 
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;

namespace Sanford.Multimedia.Midi.Score
{
    public class TempoSymbol
    {
        private int starttime;
        private int bpm;         // BPM value
        private int tempo;
        private int x;           // The x (horizontal) position within the staff 
        private const float kOneMinuteInMicroseconds = 60000000;
        private bool selected = false;
        private int width;       // Width of symbol
        private int height;      // Height of symbol

        public TempoSymbol(int starttime, int tempo) 
        {
            this.starttime = starttime;
            this.tempo = tempo;
            if (tempo >  0)
                this.bpm = (int)(kOneMinuteInMicroseconds / (float)tempo);

        }

        /// <summary>
        /// Get the time (in pulses) this symbol occurs at.
        /// This is used to determine the measure this symbol belongs to.
        /// </summary>
        public int StartTime
        {
            get { return starttime; }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Get/Set the width (in pixels) of this symbol.
        /// </summary>
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Get/Set the height (in pixels) of this symbol.
        /// 
        /// </summary>
        public int Height
        {
            get{ return height; }
            set { height = value; }
        }

        /// <summary>
        /// Get/Set the BPM value of this symbol.         
        /// </summary>        
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

        /// <summary>
        /// Get the bpm value
        /// </summary>
        public int BPM
        {
            get { return bpm; }
        }

        public bool Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;                
            }
        }


        /// <summary>
        /// Draw a tempo symbol
        /// </summary>
        /// <param name="g"></param>
        /// <param name="pen"></param>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        public void Draw (Graphics g, System.Drawing.Pen pen, int xpos, int ypos) 
        {
            string tx = string.Empty;
            System.Drawing.Brush brush = System.Drawing.Brushes.Black;            
            
                        
            if (Selected) 
            { 
                pen.Color = System.Drawing.Color.Red;
                brush = System.Drawing.Brushes.Red;
            }

            // Display the BPM value, not the tempo value
            tx = "= " + BPM.ToString();

            // Offset                    
            g.TranslateTransform(xpos + X + SheetMusic.NoteWidth / 2 + 1, ypos - SheetMusic.LineWidth + SheetMusic.NoteHeight / 2);

            // draw ellipse
            g.RotateTransform(-45);
            g.FillEllipse(brush, -SheetMusic.NoteWidth / 2, -SheetMusic.NoteHeight / 2, SheetMusic.NoteWidth, SheetMusic.NoteHeight - 1);
            g.RotateTransform(45);

            // Draw stem
            g.DrawLine(pen, SheetMusic.NoteWidth / 2, -SheetMusic.NoteHeight / 2, SheetMusic.NoteWidth / 2, -14);

            //Draw text (= 120)
            g.DrawString(tx, SheetMusic.LetterFont, brush, SheetMusic.NoteWidth, -SheetMusic.NoteHeight - SheetMusic.LineWidth);

            // Offset back                    
            g.TranslateTransform(-(xpos + X + SheetMusic.NoteWidth / 2 + 1), -(ypos - SheetMusic.LineWidth + SheetMusic.NoteHeight / 2));


            // Restore black color for other Music symbols
            brush = System.Drawing.Brushes.Black;
            pen.Color = System.Drawing.Color.Black;
            

        }


        public override string ToString()
        {
            return string.Format("TempoSymbol starttime={0} x={1} tempo={2} bpm={3}",
                                 starttime, x, tempo, bpm);
        }


    }
}
