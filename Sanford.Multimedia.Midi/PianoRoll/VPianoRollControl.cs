#region License

/* Copyright (c) 2017 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.VPianoRoll
{

    public delegate void OffsetChangedEventHandler(object sender,  int value);

    public partial class VPianoRollControl : Control
    {

        public event OffsetChangedEventHandler OffsetChanged;
        
        /// <summary>
        /// Double buffer panel
        /// </summary>
        class MyPanel : System.Windows.Forms.Panel
        {
            public MyPanel()
            {
                this.SetStyle(
                     System.Windows.Forms.ControlStyles.UserPaint |
                     System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                     System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                     true);
            }
        }
        MyPanel pnlCanvas;


        #region properties

        private int lowNoteID = DefaultLowNoteID;
        /// <summary>
        /// Gets or sets lower note
        /// </summary>
        public int LowNoteID
        {
            get
            {
                return lowNoteID;
            }
            set
            {
                lowNoteID = value;
                pnlCanvas.Invalidate();
            }
        }

        private int highNoteID = DefaultHighNoteID;
        /// <summary>
        /// Gets or sets higher note
        /// </summary>
        public int HighNoteID
        {
            get
            {
                return highNoteID;
            }
            set
            {
                highNoteID = value;
                pnlCanvas.Invalidate();
            }
        }
    

        private int totalwidth;
        /// <summary>
        /// Gets Width of control
        /// </summary>
        public int totalWidth
        {
            get { return totalwidth; }
        }

        private float _zoomy = 1.0f;    // zoom for vertical
        /// <summary>
        /// Gets or sets zoom value
        /// </summary>
        [Description("Gets or sets the vertical zoom")]
        [Category("VPianoRollControl")]
        [DefaultValue(1.0f)]
        public float zoomy
        {
            get
            { return _zoomy; }
            set
            {
                if (sequence1 != null && sequence1.Time != null)
                {
                    yscale = (value * 20.0 / sequence1.Time.Quarter);
                    lastPosition = sequence1.GetLength();
                    //Entier immédiatement suppérieur au nombre à virgule flottante
                    int nbmeasures = Convert.ToInt32(Math.Ceiling(lastPosition / sequence1.Time.Measure));

                    lastPosition = 4 * ((float)sequence1.Numerator / sequence1.Denominator) * nbmeasures;
                    lastPosition = lastPosition * sequence1.Division;

                    if ((int)(lastPosition * yscale) > 50)
                    {
                        _zoomy = value;
                        yscale = (_zoomy * 20.0 / sequence1.Time.Quarter);
                        maxstafflength = (int)(lastPosition * yscale);
                        pnlCanvas.Invalidate();
                    }
                }
            }
        }

        private double xscale = 1.0 / 10;
        /// <summary>
        /// Gets horizontal unit
        /// </summary>
        public double xScale {
            get { return xscale; }
            set { xscale = value; }
        }

        private double yscale = 1.0 / 10;
        /// <summary>
        /// Gets or sets vertical unit
        /// </summary>
        public double yScale
        {
            get
            {
                return yscale;
            }
            set
            {
                yscale = value;
                if (sequence1 != null && keysNumber > 0 && yscale > 0)
                {
                    // Width of control must be a multiple of measures
                    lastPosition = sequence1.GetLength();
                    //Entier immédiatement suppérieur au nombre à virgule flottante
                    int nbmeasures = Convert.ToInt32(Math.Ceiling(lastPosition / sequence1.Time.Measure));

                    lastPosition = 4 * ((float)sequence1.Numerator / sequence1.Denominator) * nbmeasures;
                    lastPosition = lastPosition * sequence1.Division;

                    // a quarter note is 20 units wide
                    yscale = (_zoomy * 20.0 / sequence1.Time.Quarter);
                    maxstafflength = (int)(lastPosition * yscale);

                    pnlCanvas.Invalidate();
                }
            }
        }

        private Sequence sequence1;
        /// <summary>
        /// Gets or sets sequence
        /// </summary>
        public Sequence Sequence1
        {
            get
            {
                return sequence1;
            }
            set
            {
                sequence1 = value;
                if (sequence1 != null && sequence1.Time != null)
                    measurelen = sequence1.Time.Measure;
            }
        }

        private int tracknum = -1;
        /// <summary>
        /// Gets or sets track number to be drawn
        /// </summary>
        public int TrackNum
        {
            get
            {
                return tracknum;
            }
            set
            {
                if (value != tracknum)
                {
                    tracknum = value;
                    if (sequence1 != null && tracknum != -1)
                    {
                        if (tracknum < sequence1.tracks.Count)
                        {
                            track1 = sequence1.tracks[tracknum];
                            channel = track1.MidiChannel;
                        }
                    }
                    else
                    {
                        track1 = null;
                    }
                    pnlCanvas.Invalidate();
                }
            }
        }

        private int resolution = 4; // resolution of 4 by quarter note
        /// <summary>
        /// Gets or sets resolution (dafault = 4 by quarter note)
        /// </summary>
        public int Resolution
        {
            get
            {
                return resolution;
            }
            set
            {
                if (value > 0)
                {
                    resolution = value;
                    pnlCanvas.Invalidate();
                }
            }
        } 

        private int offsety = 0;
        /// <summary>
        /// Gets or sets horizontal offset
        /// </summary>
        public int OffsetY
        {
            get { return offsety; }
            set
            {
                if (value != offsety)
                {
                    offsety = value;
                    pnlCanvas.Invalidate();
                }
            }
        }

        private int maxstafflength;
        /// <summary>
        /// Gets Length of score
        /// </summary>
        public int maxStaffLength
        {
            get { return maxstafflength; }

        }
    
        #endregion properties

        public Point CurrentPoint;
        


        #region private

        // Notes : MIDI maxi = 0 to 127 (C0 to G9)
        // Not all 128 notes taken : 23 to 108 = 86 notes
        private const int DefaultLowNoteID = 23;    //23;
        private const int DefaultHighNoteID = 108;  //108

        private int measurelen = 0;
        private int keysNumber;
        //private int curTrack = 0;
        private float lastPosition = 0;
        private Track track1;
        private int channel;        
        
        //private MidiNote note;
        //private MidiNote originNote;

        private Point aPos;
        private bool bMoveScore;
       // private int MoveScoreDirection;
        private Point previousPosition;
        private int previousDelta;
        //private Panel TimeVLine;

        #endregion


        private static readonly string[] NotesTable =
        {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "B#"
        };

        //private ContextMenu prContextMenu;

        public VPianoRollControl()
        {
            
            _zoomy = 1.0f;   // valeur de zoom
            resolution = 4;  // 4 incréments par noire
            channel = 0;

            pnlCanvas = new MyPanel();
            pnlCanvas.Location = new Point(0, 0);
            pnlCanvas.Size = new Size(40, 40);
            pnlCanvas.BackColor = Color.White;
            pnlCanvas.Dock = DockStyle.Fill;

            pnlCanvas.Paint += new PaintEventHandler(pnlCanvas_Paint);
            pnlCanvas.MouseLeave += new EventHandler(pnlCanvas_MouseLeave);
            pnlCanvas.MouseDown += new MouseEventHandler(pnlCanvas_MouseDown);
            pnlCanvas.MouseMove += new MouseEventHandler(pnlCanvas_MouseMove);
            pnlCanvas.MouseUp += new MouseEventHandler(pnlCanvas_MouseUp);

            this.Controls.Add(pnlCanvas);

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            keysNumber = 1 + highNoteID - lowNoteID; // 128 or less               
        }

      

        public void Redraw()
        {
            pnlCanvas.Invalidate();
        }


        /// <summary>
        /// Draw all notes visible in the clip
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawNotes(Graphics g, Rectangle clip)
        {
            int Y = 0;
            int H = 0;
            int PH = pnlCanvas.Height;

            // if one track
            if (track1 != null)
            {
                #region one track
                for (int i = 0; i < track1.Notes.Count; i++)
                {
                    MidiNote nnote = track1.Notes[i];
                    Y = (int)(PH - nnote.StartTime * yscale);
                    H = (int)(nnote.Duration * yscale);

                    // draw note if note ends after begining of clip and if note begins before end of clip
                    if (Y + H >= clip.Y && (Y - H) <= clip.Y + clip.Height)
                        MakeNoteRectangle(g, nnote.Number, nnote.StartTime, nnote.Duration, nnote.Channel);
                }
                #endregion
            }
            else
            {
                #region alltracks

                // if all tracks
                for (int trk = 0; trk < sequence1.tracks.Count; trk++)
                {
                    Track track = sequence1.tracks[trk];
                    for (int i = 0; i < track.Notes.Count; i++)
                    {
                        MidiNote nnote = track.Notes[i];
                        Y = (int)(PH - nnote.StartTime * yscale);
                        H = (int)(nnote.Duration * yscale);
                        // draw note if note ends before begining of clip and if note begins before end of clip
                        if (Y + H >= clip.Y && (Y - H) <= clip.Y + clip.Height)
                            MakeNoteRectangle(g, nnote.Number, nnote.StartTime, nnote.Duration, nnote.Channel);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Draw rectangles for notes
        /// </summary>
        /// <param name="e"></param>
        /// <param name="noteNumber"></param>
        /// <param name="startTime"></param>
        /// <param name="duration"></param>
        /// <param name="channel"></param>
        private void MakeNoteRectangle(Graphics g, int noteNumber, long startTime, int duration, int channel)
        {
            Rectangle rect = new Rectangle();
            SolidBrush FillBrush;
            Pen StrokePen;
            int PH = pnlCanvas.Height;

            int X = (int)((noteNumber - lowNoteID) * xscale);           // X = notenumber - 23 (graves à gauche aigues à droite)                      
            int Y = (int)(PH - (startTime + duration) * yscale);     // hauteur - starttime            

            int W = (int)xscale;
            int H = (int)(duration * yscale);
            
            Rectangle rectn = new Rectangle(X, Y, W, H);
                      
            if (channel == 9)  // Drums are green
            {
                StrokePen = new Pen(Color.DarkGreen, 2);
                FillBrush = new SolidBrush(Color.LightGreen);
                duration = sequence1.Time.Quarter / 4;
            }
            else
            {
                // Other 
                #region colors per channel
                switch (channel)
                {
                    case 0:
                        StrokePen = new Pen(Color.DarkGreen, 2); 
                        FillBrush = new SolidBrush(System.Drawing.Color.LightGreen); 
                        break;
                    case 1:
                        StrokePen = new Pen(Color.DarkBlue, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.CornflowerBlue); 
                        break;
                    case 2:
                        StrokePen = new Pen(Color.DarkGoldenrod, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightGoldenrodYellow);
                        break;
                    case 3:
                        StrokePen = new Pen(Color.DarkGreen, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.SlateBlue);
                        break;
                    case 4:
                        StrokePen = new Pen(Color.Pink, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightPink);
                        break;
                    case 5:
                        StrokePen = new Pen(Color.Salmon, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightSalmon);
                        break;
                    case 6:
                        StrokePen = new Pen(Color.SeaGreen, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightSeaGreen);
                        break;
                    case 7:
                        StrokePen = new Pen(Color.SlateBlue, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightSteelBlue);
                        break;
                    case 8:
                        StrokePen = new Pen(Color.BlueViolet, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.Plum);
                        break;

                    case 10:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.Maroon);
                        break;
                    case 11:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.Red);
                        break;
                    case 12:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.Chocolate);
                        break;
                    case 13:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.NavajoWhite);
                        break;
                    case 14:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.Green);
                        break;

                    default:
                        StrokePen = new Pen(Color.DarkGray, 2);
                        FillBrush = new SolidBrush(System.Drawing.Color.LightGray);
                        break;
                }
                #endregion colors per channel
            }

            rect = new Rectangle(X, Y, W, H);

            //g.DrawRectangle(StrokePen, X, Y, W, H);
            //g.FillRectangle(FillBrush, rect);
            // Draw rectangles with rounded corners
            RectRoutines.DrawRoundedRectangle(g, StrokePen, rect, 5);
            RectRoutines.FillRoundedRectangle(g, FillBrush, rect, 5);
            

            FillBrush.Dispose();
            StrokePen.Dispose();
        }


        #region Canvas

        // The NoteBackgroundCanvas is used for drawing the horizontal lines that divide each of the 128 MIDI notes.
        // We will also shade the lines that represent "black notes" on the piano slightly. 
        // Each of these horizontal lines we will simply make one unit wide, and a ScaleTransform will be used to ensure that they stretch the required amount. 
        // Here's the code that populates the background canvas.        
        private void CreateBackgroundCanvas(Graphics g, Rectangle clip)
        {
            SolidBrush FillBrush;
            Pen FillPen;

            Rectangle rect;

            Color blackKeysColor = Color.DimGray;
            Color whiteKeysColor = Color.Gray;

            blackKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            whiteKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF3b3b3b");
            Pen SeparatorPen = new Pen(blackKeysColor, 1);
            Pen GroupNotesPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF676767"), 1);

            int w = 0;
            int H = 0; // Length of scores
            int W = 0; 
            //if (clip.Height > maxstafflength)
            //    H = maxstafflength;
            //else
                H = clip.Height;
            
            totalwidth = (int)((1 + highNoteID - lowNoteID) * xscale);

            // ==========================
            // Backgroud color : grey
            // ==========================            
            FillPen = new Pen(whiteKeysColor);
            g.DrawRectangle(FillPen, clip.X, clip.Y, totalwidth, H);
            rect = new Rectangle(clip.X, clip.Y, totalwidth, H);
            FillBrush = new SolidBrush(whiteKeysColor);
            g.FillRectangle(FillBrush, rect);


            // ===================================
            // Draw black areas for # & b notes
            // ===================================
            for (int note = highNoteID; note >= lowNoteID; note--)
            {
                // Black keys are filled
                if ((note % 12 == 1) // C#
                 || (note % 12 == 3) // Eb
                 || (note % 12 == 5) // F#      ???? devrait être 6, mais marche quand même
                 || (note % 12 == 8) // Ab
                 || (note % 12 == 10)) // Bb
                {
                    FillBrush = new SolidBrush(blackKeysColor);
                    FillPen = new Pen(blackKeysColor);

                    W = (int)xscale;

                    g.DrawRectangle(FillPen, w, clip.Y, W, H);
                    rect = new Rectangle(w, clip.Y, W, H);
                    g.FillRectangle(FillBrush, rect);                    
                }
                w += (int)xscale;
            }


            // =================================
            // Draw vertical lines each xscale
            // =================================
            w = 0;

            for (int note = highNoteID; note >= lowNoteID; note--)
            {
                w += (int)xscale;

                if (w >= clip.X && w <= clip.X + clip.Width)
                {
                    Point p1 = new Point(w, clip.Y);
                    Point p2 = new Point(w, H);

                    g.DrawLine(SeparatorPen, p1, p2);
                }
            }

            // ==================================================
            // Draw vertical lines to separate groups of notes 
            // C, D, E - F G A B
            // ==================================================
            w = 0;

            for (int note = highNoteID; note >= lowNoteID; note--)
            {
                w += (int)xscale;

                if ((note % 12 == 0) // C
                || (note % 12 == 7)) // F      devrait être 5 car en fait 7 c'est G ?????
                {
                    if (w >= clip.X && w <= clip.X + clip.Width)
                    {
                        Point p1 = new Point(w, clip.Y);
                        Point p2 = new Point(w, H);

                        g.DrawLine(GroupNotesPen, p1, p2);
                    }
                }
            }
        }

        // On top of the background canvas comes the GridCanvas. 
        // This contains the vertical lines showing where each measure and beat start.
        // We can't draw this until we have loaded the MIDI events because we need to know how many MIDI ticks are in each quarter note,
        // and also we need to know how many grid lines to draw. 
        // Here is the code that draws the grid.
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int beat = 0;
            int timespermeasure = (4 * sequence1.Numerator) / sequence1.Denominator; // 4 si 4/4, 6 si 3/2, 8 si 4/2

            Pen mesureSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF676767"), 1);
            Pen beatSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF585858"), 1);
            Pen intervalSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF464646"), 1);

            int quarter = sequence1.Time.Quarter; // noire            

            float f_n = 0;
            float f_increment = (float)quarter / (float)resolution;     // (1/resolution) de noire - par exemple 1/4 = 4 doubles croches => 960/4 = 240 ticks

            totalwidth = (int)((1 + highNoteID - lowNoteID) * xscale);
            int PH = pnlCanvas.Height;

            // Measure number display
            int nbMeasure = 0;
            SolidBrush textBrush = new SolidBrush(Color.DimGray);
            Font fontNoteLetter = new Font("Arial", 20, FontStyle.Regular, GraphicsUnit.Pixel);

            do
            {
                int y1 = PH - (int)(f_n * yscale);
                int y2 = y1;
                int x1 = 0;
                int x2 = totalwidth;

                if (y1 >= clip.Y && y1 <= clip.Y + clip.Height)
                {
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y2);

                    if (beat % (resolution * timespermeasure) == 0)        // every measure
                    {
                        // Display line
                        g.DrawLine(mesureSeparatorPen, p1, p2);
                        // Display measure number
                        nbMeasure = 1 + Convert.ToInt32(f_n / measurelen);
                        g.DrawString("Measure " + nbMeasure, fontNoteLetter, textBrush, p1.X + 5, p1.Y - fontNoteLetter.Height);                        
                    }
                    else if (beat % resolution == 0)                       // every time or beat
                    {
                        g.DrawLine(beatSeparatorPen, p1, p2);
                    }
                    else
                    {
                        g.DrawLine(intervalSeparatorPen, p1, p2);          // every resolution
                    }
                }

                beat++;
                f_n += f_increment;

            } while (f_n <= lastPosition);

          
        }

        #endregion Canvas
     
    
        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            this.Capture = false;
        }

        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                bMoveScore = true;
                previousDelta = 0;
                // save start position of rectangle
                aPos = PointToClient(Control.MousePosition);
                Cursor = Cursors.Hand;

            }
        }

        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (bMoveScore &&  e.Button == MouseButtons.Left)
            {
                Point mousePos = MousePosition;
                if (mousePos == previousPosition)
                    return;

                Point pos = PointToClient(Control.MousePosition);
                int Delta = aPos.Y - pos.Y;

                if (Math.Abs(Delta) > Math.Abs(previousDelta))
                    MovePanel(Delta);                       // Delta increases => same direction
                else
                    ResetMovePanel();                   // Delta decreases => a change of direction was performed
              
                previousPosition = mousePos;                
            }
        }

        private void MovePanel(int Delta)
        {
            previousDelta = Delta;
            int newval = OffsetY + Delta;
            if (newval < 0) newval = 0;
            OffsetY = newval;
            OffsetChanged(this, newval);
        }

        private void ResetMovePanel()
        {
            previousDelta = 0;
            aPos = PointToClient(Control.MousePosition);            
        }

        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (bMoveScore && e.Button == MouseButtons.Left)
            {
                bMoveScore = false;
                previousDelta = 0;
                Cursor = Cursors.Default;
            }
        }



        #region events

        /// <summary>
        /// Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {            
            Rectangle clip =
                new Rectangle(
                e.ClipRectangle.X,                
                 - offsety,
                e.ClipRectangle.Width,
                e.ClipRectangle.Height);

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TranslateTransform(0, -clip.Y);            

            if (sequence1 != null)
            {
                if (Parent.GetType() == typeof(Panel))
                {
                    CreateBackgroundCanvas(g, clip);
                    DrawGrid(g, clip);
                    DrawNotes(g, clip);
                }
                else
                {
                    CreateBackgroundCanvas(g, clip);
                    DrawGrid(g, clip);
                    DrawNotes(g, clip);
                }
                g.TranslateTransform(0, clip.Y);                
            }
        }


        protected override void OnResize(EventArgs e)
        {
            pnlCanvas.Invalidate();

            base.OnResize(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }    

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height)
            {
                Capture = false;
            }
            base.OnMouseMove(e);
        }

        #endregion events
    

        private Rectangle GetVisibleRectangle(Control c)
        {
            // rectangle du controle en coordonnées écran
            Rectangle rect = c.RectangleToScreen(c.ClientRectangle);
            c = c.Parent;
            while (c != null)
            {
                rect = Rectangle.Intersect(rect, c.RectangleToScreen(c.ClientRectangle));
                c = c.Parent;
            }
            // rectangle en coordonnées relatives au client
            rect = pnlCanvas.RectangleToClient(rect);
            return rect;
        }    




    }
}
