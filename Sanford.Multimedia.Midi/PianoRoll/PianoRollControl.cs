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
using Sanford.Multimedia.Midi.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.PianoRoll
{
    #region delegate

    public delegate void MouseDownPlayNoteEventHandler(int starttime, int channel, int nnote, int duration, MouseEventArgs e);
    public delegate void MouseUpStopNoteEventHandler(int starttime, int channel, int nnote, int duration, MouseEventArgs e);
    public delegate void InfoNoteEventHandler(string tx);

    public delegate void OffsetChangedEventHandler(object sender, int value);
    public delegate void MouseMoveEventHandler(object sender, int note, MouseEventArgs e);

    #endregion delegate


    public partial class PianoRollControl : Control
    {

        #region events

        public event MouseDownPlayNoteEventHandler MouseDownPlayNote;
        public event MouseUpStopNoteEventHandler MouseUpStopNote;
        public event InfoNoteEventHandler InfoNote;

        public event EventHandler SequenceModified;
        public event EventHandler MyMouseDown;         // ahhhhhhhhhhhhhhhhhhhh
        public event EventHandler MyMouseUp;

        public event OffsetChangedEventHandler OffsetChanged;
        public event MouseMoveEventHandler OnMouseMoved;


        #endregion

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

        public Point CurrentPoint;


        #region properties

        private int _TimeLineHeight = 40;
        /// <summary>
        /// Height of time line
        /// </summary>
        public int TimeLineY
        {
            get
            {
                return _TimeLineHeight;
            }
            set
            {
                if (value > 0 && value != _TimeLineHeight)
                {
                    _TimeLineHeight = value;
                    pnlCanvas.Invalidate();
                }
            }
        }


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
                                        
        private int _totalheight;
        /// <summary>
        /// Gets or sets totalHeight
        /// </summary>
        public int totalHeight
        {
            get
            {
                return _totalheight;
            }
        }

        private float _zoomx = 1.0f;    // zoom for horizontal
        /// <summary>
        /// Gets or sets zoom value
        /// </summary>
        public float zoomx
        {
            get
            { return _zoomx; }
            set
            {
                if (sequence1 != null)
                {
                    _xscale = (value * 20.0 / sequence1.Time.Quarter);
                    if ((int)(lastPosition * _xscale) > 50)
                    {
                        _zoomx = value;
                        _xscale = (_zoomx * 20.0 / sequence1.Time.Quarter);                        
                        _maxstaffwidth = (int)(lastPosition * _xscale);
                        selRect = new Rectangle();
                        pnlCanvas.Invalidate();
                    }
                }
            }
        }

        private double _xscale = 1.0 / 10;
        /// <summary>
        /// Gets horizontal unit
        /// </summary>
        public double xScale { 
            get { return _xscale; } 
            set
            {
                _xscale = value;
                if (sequence1 != null && keysNumber > 0 && _yscale > 0)
                {
                    // Width of control must be a multiple of measures
                    lastPosition = sequence1.GetLength();
                    //Entier immédiatement suppérieur au nombre à virgule flottante
                    int nummeasure = Convert.ToInt32(Math.Ceiling(lastPosition / sequence1.Time.Measure));

                    lastPosition = 4 * ((float)sequence1.Numerator / sequence1.Denominator) * nummeasure;
                    lastPosition = lastPosition * sequence1.Division;

                    // a quarter note is 20 units wide
                    _xscale = (_zoomx * 20.0 / sequence1.Time.Quarter);

                    _maxstaffwidth = (int)(lastPosition * _xscale);
                    pnlCanvas.Invalidate();
                }
            }
        }
                
        private int _yscale;
        /// <summary>
        /// Gets or sets vertical unit
        /// </summary>
        public int yScale
        {
            get { return _yscale; }
            set 
            { 
                _yscale = value;
                if (sequence1 != null && keysNumber > 0 && _yscale > 0)
                {
                    // Width of control must be a multiple of measures
                    lastPosition = sequence1.GetLength();
                    //Entier immédiatement suppérieur au nombre à virgule flottante
                    int nummeasure = Convert.ToInt32(Math.Ceiling(lastPosition / sequence1.Time.Measure));

                    lastPosition = 4 * ((float)sequence1.Numerator / sequence1.Denominator) * nummeasure;
                    lastPosition = lastPosition * sequence1.Division;

                    // a quarter note is 20 units wide
                    _xscale = (_zoomx * 20.0 / sequence1.Time.Quarter);

                    _maxstaffwidth = (int)(lastPosition * _xscale);
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
                     
        private bool bEnterNotes = false;
        /// <summary>
        /// Gets or sets notes edition status
        /// </summary>
        public bool NotesEdition
        {
            get
            {
                return bEnterNotes;
            }
            set
            {
                bEnterNotes = value;                
                if (bEnterNotes == true)
                    Cursor = Cursors.Hand;
                else
                    Cursor = Cursors.Default;
            }
        }

        
        private int _offsetx = 0;
        /// <summary>
        /// Gets or sets horizontal offset
        /// </summary>
        public int OffsetX
        {
            get { return _offsetx; }
            set
            {
                if (value != _offsetx)
                {
                    _offsetx = value;
                    if (OffsetChanged != null)
                        OffsetChanged(this, _offsetx);
                    pnlCanvas.Invalidate();
                }
            }
        }

        private int _offsety = 0;
        
        public int OffsetY
        {
            get { return _offsety; }
            set 
            {
                if (value != _offsety)
                {
                    _offsety = value;
                    pnlCanvas.Invalidate();
                }
            }
        }


        private int _maxstaffwidth;
        /// <summary>
        /// Gets Length of score
        /// </summary>
        public int maxStaffWidth
        {
            get { return _maxstaffwidth; }

        }

        
        private int _velocity = 100;
        public int Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        #endregion properties

        
        #region private

        private MyPanel pnlCanvas;

        // Notes : MIDI maxi = 0 to 127 (C0 to G9)
        // Not all 128 notes taken : 23 to 108 = 86 notes
        private const int DefaultLowNoteID = 23;    //23;
        private const int DefaultHighNoteID = 108;  //108

        private int measurelen = 0;
        private int keysNumber;
        private int curTrack = 0;
        private float lastPosition = 0;
        private Track track1;
        private int channel;

        private bool keyControlDown = false;
        private bool bReadyToPaste = false;

        private MidiNote note;
        private MidiNote newNote;
        private MidiNote originNote;
        private List<MidiNote> selNotes;
        private Rectangle selRect = new Rectangle(0,0,0,0);
               
        private bool drawingNote = false;       // start outside an existing note and draw a new note
        private bool selectingNote = false;     // start outside an existing note and draw a rectangle
        private bool draggingNote = false;      // start inside an existing note and drag this note
        private bool deletingNote = false;      // start inside an existing note and delete this note
        private bool modifyingNote = false;     // start inside an existing note and extend or reduce this note

        private enum editmode
        {
            left,
            right
        }
        private editmode EditMode;              // Modify note from left or from right

        private Point aPos;
        private Panel TimeVLine;

        private static readonly string[] NotesTable =
        {
            "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "B#"
        };

        private ContextMenu prContextMenu;


        #endregion


        public PianoRollControl()
        {  
            _zoomx = 1.0f;   // valeur de zoom
            resolution = 4;  // 4 incréments par noire
            channel = 0;
            

            this.KeyDown += new KeyEventHandler(PianoRollControl_KeyDown);
            this.KeyUp += new KeyEventHandler(PianoRollControl_KeyUp);

            pnlCanvas = new MyPanel();
            pnlCanvas.Location = new Point(0, 0);
            pnlCanvas.Size = new Size(40, 40);
            pnlCanvas.BackColor = Color.White;
            pnlCanvas.Dock = DockStyle.Fill;
            
            pnlCanvas.Paint += new PaintEventHandler(pnlCanvas_Paint);
            pnlCanvas.MouseDown += new MouseEventHandler(pnlCanvas_MouseDown);
            pnlCanvas.MouseUp += new MouseEventHandler(pnlCanvas_MouseUp);
            pnlCanvas.MouseMove += new MouseEventHandler(pnlCanvas_MouseMove);
            pnlCanvas.MouseLeave += new EventHandler(pnlCanvas_MouseLeave);
                           
            this.Controls.Add(pnlCanvas);

            // Draw timeline bar at position 0
            DrawVbar(0);
            
            this.MouseDownPlayNote += new MouseDownPlayNoteEventHandler(MouseDownPlayNote2);
            this.MouseUpStopNote += new MouseUpStopNoteEventHandler(MouseUpStopNote2);
            this.InfoNote += new InfoNoteEventHandler(InfoNote2);

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            keysNumber = 1 + highNoteID - lowNoteID; // 128 or less               
        }
           

        #region draw notes

        /// <summary>
        /// Draw all notes visible in the clip
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawNotes(Graphics g, Rectangle clip)
        {
            int X;
            int W;
            // if one track
            if (track1 != null)
            {
                #region one track
                for (int i = 0; i < track1.Notes.Count; i++)
                {
                    MidiNote nnote = track1.Notes[i];
                    X = (int)(nnote.StartTime * _xscale);
                    W = (int)(nnote.Duration * _xscale);

                    // draw note if note ends after begining of clip and if note begins before end of clip
                    if ((X + W) >= clip.X && X <= clip.X + clip.Width)
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
                        X = (int)(nnote.StartTime * _xscale);
                        W = (int)(nnote.Duration * _xscale);
                        // draw note if note ends before begining of clip and if note begins before end of clip
                        if ((X + W) >= clip.X && X <= clip.X + clip.Width)
                            MakeNoteRectangle(g, nnote.Number, nnote.StartTime, nnote.Duration, nnote.Channel);
                    }
                }

                #endregion
            }

            if (newNote != null)
            {
                MakeNoteRectangle(g, newNote.Number, newNote.StartTime, newNote.Duration, newNote.Channel);
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

            int X = (int)(startTime * _xscale);            
            int Y = _TimeLineHeight + (highNoteID - noteNumber) * _yscale;  //127 - notenumber

            int W = (int)(duration * _xscale);
            int H = _yscale;

            Rectangle rectn = new Rectangle(X, Y, W, H);

            // Note not selected by default
            bool selected = false;

            // Determine if not is selected
            if (selectingNote)
            {
                // Rectangle selection in progress
                if (selRect.Contains(rectn))
                {                    
                    selected = true;                    
                }
            }
            else
            {
                // Selection is finished
                if (track1 != null)
                {
                    int idx = track1.Notes.FindIndex(u => u.Number == noteNumber && u.StartTime == startTime);
                    if (idx != -1 && track1.Notes[idx].Selected)
                        selected = true;
                }
            }

            // If notes are selected => red            
            if (selected)
            {
                StrokePen = new Pen(Color.Red, 2);
                FillBrush = new SolidBrush(System.Drawing.Color.LightPink);

            }
            else if (channel == 9)  // Drums are green
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

            g.DrawRectangle(StrokePen, X, Y, W, H);                         
            g.FillRectangle(FillBrush, rect);
                        
            FillBrush.Dispose();
            StrokePen.Dispose();
        }

        #endregion


        #region Draw Canvas

        /// <summary>
        /// Redraw Canvas
        /// </summary>
        public void Redraw()
        {
            pnlCanvas.Invalidate();
        }


        // The NoteBackgroundCanvas is used for drawing the horizontal lines that divide each of the 128 MIDI notes.
        // We will also shade the lines that represent "black notes" on the piano slightly. 
        // Each of these horizontal lines we will simply make one unit wide, and a ScaleTransform will be used to ensure that they stretch the required amount. 
        // Here's the code that populates the background canvas.        
        private void CreateBackgroundCanvas(Graphics g, Rectangle clip)
        {
            SolidBrush FillBrush;
            Pen FillPen;
            
            Rectangle rect;
            Point p1;
            Point p2;

            Color TimeLineColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Color blackKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Color whiteKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF3b3b3b");

            Pen SeparatorPen = new Pen(blackKeysColor, 1);
            Pen GroupNotesPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF676767"), 1);

            int h = _TimeLineHeight;  // bande horizontale en haut pour afficher les mesures et intervalles
            int H = 0;
            int W = 0; // Width of scores
            
            
            if (clip.Width > _maxstaffwidth)
                W = _maxstaffwidth;
            else
                W = clip.Width;
           
            _totalheight = (1 + highNoteID - lowNoteID) * _yscale;            

            // ==========================
            // Background color : grey
            // ==========================            
            FillPen = new Pen(whiteKeysColor);
            g.DrawRectangle(FillPen, clip.X, clip.Y + h, W, _totalheight);
            rect = new Rectangle(clip.X, clip.Y + h, W, _totalheight);
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
                 || (note % 12 == 6) // F#
                 || (note % 12 == 8) // Ab
                 || (note % 12 == 10)) // Bb
                {                    
                    FillBrush = new SolidBrush(blackKeysColor);
                    FillPen = new Pen(blackKeysColor);

                    H = _yscale;

                    g.DrawRectangle(FillPen, clip.X, h, W, H);                        
                    rect = new Rectangle(clip.X, h, W, H);
                    g.FillRectangle(FillBrush, rect);                    
                }       
                h += _yscale;
            }

            
            // =================================
            // Draw horizontal lines each _yscale
            // =================================
            h = _TimeLineHeight;            

            for (int note = highNoteID; note >= lowNoteID; note--)
            {                                               
                h += _yscale;

                if (h >= clip.Y && h <= clip.Y + clip.Height)
                {                    
                    p1 = new Point(clip.X, h);
                    p2 = new Point(W, h);

                    g.DrawLine(SeparatorPen, p1, p2);
                }
            }

            // Height of the control
            _totalheight = h;             
        }

        // On top of the background canvas comes the GridCanvas. 
        // This contains the vertical lines showing where each measure and beat start.
        // We can't draw this until we have loaded the MIDI events because we need to know how many MIDI ticks are in each quarter note,
        // and also we need to know how many grid lines to draw. 
        // Here is the code that draws the grid.
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int step = 0;
            int h = _TimeLineHeight;  // bande horizontale en haut pour afficher les mesures et intervalles            

            int timespermeasure = sequence1.Numerator;             // nombre de beats par mesures
            float TimeUnit = Sequence1.Denominator;            // 2 = blanche, 4 = noire, 8 = croche, 16 = doucle croche, 32 triple croche

            //Pen TicksPen = new Pen(Color.White);
            Pen mesureSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF676767"), 1);
            Pen beatSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF585858"), 1);            
            Pen intervalSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF464646"), 1);

            // quarter = durée d'une noire
            int quarter = sequence1.Time.Quarter;

            _totalheight = h + (int)((1 + highNoteID - lowNoteID) * _yscale);

            float f_n = 0;

            // Increment of 1 TimeUnit, divided by the resolution, in ticks
            float f_beat = (float)quarter * 4 / TimeUnit;
            float f_increment = f_beat / resolution;

            do
            {
                int x1 = (int)(f_n * _xscale);
                int x2 = x1;
                int y1 = h;
                int y2 = _totalheight;

                if (x1 >= clip.X && x1 <= clip.X + clip.Width)
                {
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y2);

                    if (step % (resolution * timespermeasure) == 0)        // every measure
                    {
                        // Display line
                        g.DrawLine(mesureSeparatorPen, p1, p2);
                    }
                    else if (step % resolution == 0)                        // every time or beat
                    {
                        // Display line
                        g.DrawLine(beatSeparatorPen, p1, p2);
                    }
                    else
                    {
                        g.DrawLine(intervalSeparatorPen, p1, p2);  // every resolution
                    }
                }

                step++;
                f_n += f_increment;

            } while (f_n <= lastPosition);    
        }

        /// <summary>
        /// Draw Time Line at position 0
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawTimeLine(Graphics g, Rectangle clip)
        {
            SolidBrush FillBrush;
            Pen FillPen;
            Color TimeLineColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Rectangle rect;

            Point p1;
            Point p2;

            int h = _TimeLineHeight;  // bande horizontale en haut pour afficher les mesures et intervalles
            int H = 0;
            int W = 0; // Width of scores

            W = clip.Width;

            // ==========================
            // Draw Timeline background color
            // Dessiner en dernier
            // ========================== 
            FillPen = new Pen(TimeLineColor);

            // Gray rectangle
            g.DrawRectangle(FillPen, clip.X, clip.Y , W, h);
            rect = new Rectangle(clip.X, clip.Y , W, h);
            FillBrush = new SolidBrush(TimeLineColor);
            g.FillRectangle(FillBrush, rect);

            // Black line separator
            FillPen = new Pen(Color.Black, 3);
            p1 = new Point(clip.X, clip.Y + _TimeLineHeight - 2 );
            p2 = new Point(clip.X + W, clip.Y +  _TimeLineHeight - 2);
            g.DrawLine(FillPen, p1, p2);

            // ----------------------------------
            // Draw Ticks
            // ----------------------------------
            int step = 0;            
            int timespermeasure = sequence1.Numerator;
            float TimeUnit = Sequence1.Denominator;            // 2 = blanche, 4 = noire, 8 = croche, 16 = doucle croche, 32 triple croche
            Pen TicksPen = new Pen(Color.White);

            // quarter = durée d'une noire
            int quarter = sequence1.Time.Quarter;
            _totalheight = h + (int)((1 + highNoteID - lowNoteID) * _yscale);
            float f_n = 0;

            // Increment of 1 TimeUnit, divided by the resolution, in ticks
            float f_beat = (float)quarter * 4 / TimeUnit;
            float f_increment = f_beat / resolution;

            // Measure number display
            int NumMeasure;
            SolidBrush textBrush = new SolidBrush(Color.White);
            Font fontMeasure = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontInterval = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel);
            int pico = 0;

            do
            {
                int x1 = (int)(f_n * _xscale);
                int x2 = x1;
                int y1 = h;
                int y2 = _totalheight;

                if (x1 >= clip.X && x1 <= clip.X + clip.Width)
                {
                    p1 = new Point(x1, y1);
                    p2 = new Point(x2, y2);

                    if (step % (resolution * timespermeasure) == 0)        // every measure
                    {
                        // Display measure number
                        Point p1pico = new Point(x1, _TimeLineHeight - 14 - _offsety);
                        Point p2pico = new Point(x1, _TimeLineHeight - 1 - _offsety);
                        g.DrawLine(TicksPen, p1pico, p2pico);
                        NumMeasure = 1 + (int)(f_n / measurelen);
                        g.DrawString(NumMeasure.ToString(), fontMeasure, textBrush, p1.X - 5, 7 - _offsety);

                    }
                    else if (step % resolution == 0)                        // every time or beat
                    {
                        // Display beat number
                        Point p1pico = new Point(x1, _TimeLineHeight - 5 - _offsety);
                        Point p2pico = new Point(x1, _TimeLineHeight - 1 - _offsety);
                        g.DrawLine(TicksPen, p1pico, p2pico);
                        NumMeasure = 1 + (int)(f_n / measurelen);
                        pico = 1 + sequence1.Numerator - (int)((NumMeasure * measurelen - f_n) / (measurelen / sequence1.Numerator));
                        g.DrawString(NumMeasure + "." + pico, fontInterval, textBrush, p1.X - 5, 7 - _offsety);
                    }                    
                }

                step++;
                f_n += f_increment;

            } while (f_n <= lastPosition);
        }

        #endregion Canvas


        #region Canvas events

        /// <summary>
        /// Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clip =
                new Rectangle((int)(_offsetx),
                (int)(e.ClipRectangle.Y),
                (int)(e.ClipRectangle.Width),
                (int)(e.ClipRectangle.Height));

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.TranslateTransform(-clip.X, 0);

            if (sequence1 != null)
            {

                CreateBackgroundCanvas(g, clip);
                DrawGrid(g, clip);
                DrawTimeLine(g, clip);
                DrawNotes(g, clip);
                /*
                if (Parent.GetType() == typeof(Panel))
                {
                    CreateBackgroundCanvas(g, clip);
                    DrawGrid(g, clip);
                    DrawTimeLine(g, clip);
                    DrawNotes(g, clip);
                }
                else
                {
                    CreateBackgroundCanvas(g, clip);
                    DrawGrid(g, clip);
                    DrawTimeLine(g, clip);
                    DrawNotes(g, clip);
                }
                */
                g.TranslateTransform(clip.X, 0);

                if (selectingNote == true)
                {
                    // Draw the current rectangle
                    Point pos = PointToClient(Control.MousePosition);
                    using (Pen pen = new Pen(Brushes.White))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        g.DrawLine(pen, aPos.X, aPos.Y, pos.X, aPos.Y);
                        g.DrawLine(pen, pos.X, aPos.Y, pos.X, pos.Y);
                        g.DrawLine(pen, pos.X, pos.Y, aPos.X, pos.Y);
                        g.DrawLine(pen, aPos.X, pos.Y, aPos.X, aPos.Y);
                    }
                }
                setTimeVLinePos(0);
            }
        }


        #endregion


        #region Keyboard

        public void ManageKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    if (bEnterNotes == false)
                    {
                        keyControlDown = true;
                    }
                    break;

                case Keys.Delete:
                    DeleteSelectedNotes();
                    SequenceModified(sender, e);
                    pnlCanvas.Invalidate();
                    break;
            }

        }

        public void ManageKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    if (bEnterNotes == false)
                    {
                        keyControlDown = false;
                    }
                    break;
            }
        }

        private void PianoRollControl_KeyDown(object sender, KeyEventArgs e)
        {
             switch(e.KeyCode)
             {
                 case Keys.ControlKey:
                     if (bEnterNotes == false)
                     {                         
                         keyControlDown = true;
                     }
                     break;         
                     
                 case Keys.Delete:
                     DeleteSelectedNotes();
                     SequenceModified(sender, e);
                     pnlCanvas.Invalidate();
                     break;
             }
        }

        private void PianoRollControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.ControlKey:
                    if (bEnterNotes == false)
                    {
                        keyControlDown = false;
                    }
                    break;
            }
        }

        #endregion keyboard
 

        #region Mouse
           
        private MidiNote FindNote(int nnote, int starttime )
        {
            //MidiNote z;
            List<MidiNote> Lr = new List<MidiNote>();
            List<int> Ct = new List<int>();

            if (track1 != null)
            {
                List<MidiNote> Lt = track1.Notes.FindAll(u => u.Number == nnote && u.StartTime <= starttime && u.StartTime + u.Duration > starttime);
                if (Lt.Count > 0)
                {
                    for (int j = 0; j < Lt.Count; j++)
                    {
                        Lr.Add(Lt[j]);
                        Ct.Add(tracknum); // curTrack
                    }
                }
            }
            else
            {
                for (int i = 0; i < sequence1.tracks.Count; i++)
                {
                    Track track = sequence1.tracks[i];
                    List<MidiNote> Lt = track.Notes.FindAll(u => u.Number == nnote && u.StartTime <= starttime && u.StartTime + u.Duration > starttime);
                    if (Lt.Count > 0)
                    {
                        for (int j = 0; j < Lt.Count; j++)
                        {
                            Lr.Add(Lt[j]);
                            Ct.Add(i); // curTrack
                        }
                    }
                }
            }

            if (Lr.Count == 0)
                return null;

            int idx = 0;
            int minduration = sequence1.Time.Measure;
            for (int i = 0; i < Lr.Count; i++ )
            {
                if (Lr[i].Duration <= minduration)
                {
                    minduration = Lr[i].Duration;
                    idx = i;
                    curTrack =  Ct[i];
                }
            }
            
            return Lr[idx];
        }
        
        
        private void DeleteSelectedNotes()
        {

            if (track1 != null)
            {

                bool oneMoreTime = true;

                while (oneMoreTime)
                {
                    MidiNote toDelete = null;
                    oneMoreTime = false;
                    foreach (MidiNote item in track1.Notes)
                    {
                        
                        if (item.Selected)
                        {
                            toDelete = item;
                            break;
                        }

                    }
                    if (toDelete != null)
                    {
                        track1.deleteNote(toDelete.Number, toDelete.StartTime);
                        oneMoreTime = true;
                    }
                }

            }
            else
            {
                for (int trk = 0; trk < sequence1.tracks.Count; trk++)
                {
                    Track track = sequence1.tracks[trk];
                    bool oneMoreTime = true;

                    while (oneMoreTime)
                    {
                        MidiNote toDelete = null;
                        oneMoreTime = false;
                        foreach (MidiNote item in track.Notes)
                        {
                           
                            if (item.Selected)
                            {
                                toDelete = item;
                                break;
                            }

                        }
                        if (toDelete != null)
                        {
                            track.deleteNote(toDelete.Number, toDelete.StartTime);
                            oneMoreTime = true;
                        }
                    }
                }
            }

            selRect = new Rectangle(0, 0, 0, 0);           
        }
          
        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                #region leftbutton

                //int measurelen = sequence1.Time.Measure;

                // We are in the mode "enter notes"
                if (bEnterNotes == true)
                {
                    #region bEnterNotes = true

                    // MouseDown dans le mode "enterNotes"
                    // Si au dessus d'une note existante : 
                    // - effacement de la note (mouseup)
                    // - déplacement (drag) de la note (mousemove)
                    // - aggrandissement/diminution de la note si en périphérie gauche ou droite
                    //
                    // Si en dehors d'une note
                    // - création d'une note

                    if (MyMouseDown != null)
                        MyMouseDown(sender, e);

                    int X = e.X + _offsetx;
                    int Y = e.Y - _TimeLineHeight;          // Offset for time line
                    int nnote = highNoteID - Y / _yscale;
                    int starttime = (int)(X / _xscale);

                    note = FindNote(nnote, starttime);

                    // the cursor is above a note, Note exists : select one note and allow drag
                    if (note != null)
                    {
                        // test if mouse is exactly on left or right of note bounds => modify note
                        int deb = note.StartTime;
                        int fin = deb + note.Duration;
                        if ((starttime >= deb && starttime <= deb + 10) || (starttime >= fin - 10 && starttime <= fin))
                        {
                            // Assuming that mouse is on the peripheral bounds left or right
                            #region modify note bounds
                            modifyingNote = true;

                            if (starttime >= deb && starttime <= deb + 10)
                                EditMode = editmode.left;
                            else
                                EditMode = editmode.right;

                            draggingNote = false;
                            deletingNote = false;
                            drawingNote = false;
                            selectingNote = false;

                            //newNote
                            newNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);
                            //Console.Write("Duration = " + newNote.Duration + "\r");

                            // Original note
                            originNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);
                            #endregion modify note bounds
                        }
                        else
                        {
                            // Assuming mouse is INSIDE bounds of note => drag note
                            #region drag note
                            draggingNote = true;
                            deletingNote = true;

                            selectingNote = false; // never used in this mode
                            drawingNote = false;
                            modifyingNote = false;

                            // New note to be dragged
                            newNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);
                            // Original note
                            originNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);

                            // Record the start point
                            aPos = PointToClient(Control.MousePosition);
                            aPos.X = aPos.X + _offsetx;

                            selectNote(originNote);
                            pnlCanvas.Invalidate();
                            #endregion drag note
                        }

                    }
                    else if (note == null)
                    {
                        // Note does not exist => start note creation
                        #region start note creation
                        drawingNote = true;

                        selectingNote = false; // never used in this mode
                        deletingNote = false;
                        draggingNote = false;
                        modifyingNote = false;

                        // measure number
                        int nummeasure = starttime / measurelen;

                        // Temps dans la mesure
                        int rest = starttime % measurelen;                        
                        float timeinmeasure = rest / sequence1.Time.Quarter;

                        //timeinmeasure = sequence1.Numerator - (int)((nummeasure * measurelen - starttime) / (measurelen / sequence1.Numerator));

                        // fraction de temps
                        float ffraction = (float)sequence1.Time.Quarter / (float)resolution;                    // Lenght of the smallest division: Lenght of measure divided by the resolution
                        float ffractiontime =  (rest - (timeinmeasure * sequence1.Time.Quarter)) / ffraction;
                        int fractiontime = (int)ffractiontime;

                        // start time  
                        //starttime = nummeasure * measurelen + sequence1.Time.Quarter * timeinmeasure + (int)(ffraction * fractiontime);                        
                        starttime = (int)(nummeasure * measurelen + sequence1.Time.Quarter * timeinmeasure + ffraction * fractiontime);

                        int duration = sequence1.Time.Quarter / resolution;

                        //int velocity = 100;

                        newNote = new MidiNote(starttime, channel, nnote, duration, _velocity, false);  // Start new note creation

                        pnlCanvas.Invalidate();
                        #endregion start note creation
                    }

                    #endregion bEnterNotes = true
                }
                else
                {
                    // We are not in the mode "enter notes" 
                    #region bEnterNotes = false

                    if (MyMouseDown != null)
                        MyMouseDown(sender, e);

                    int X = e.X + _offsetx;
                    int Y = e.Y;
                    int nnote = highNoteID - Y / _yscale;
                    int starttime = (int)(X / _xscale);

                    note = FindNote(nnote, starttime);


                    // Two cases :
                    // 1 : mouse down on an existing note => drag or modify
                    // 2 : mouse down outside a note => draw a rectangle


                    // the cursor is above a note, Note exists : select one note and allow drag
                    if (note != null)
                    {

                        // test if mouse is exactly on left or right of note bounds => modify note
                        int deb = note.StartTime;
                        int fin = deb + note.Duration;
                        if ((starttime >= deb && starttime <= deb + 10) || (starttime >= fin - 10 && starttime <= fin))
                        {
                            // Assuming that mouse is on the peripheral bounds left or right
                            #region modify note bounds
                            modifyingNote = true;

                            if (starttime >= deb && starttime <= deb + 10)
                                EditMode = editmode.left;
                            else
                                EditMode = editmode.right;

                            draggingNote = false;
                            deletingNote = false;
                            drawingNote = false;
                            selectingNote = false;

                            //new Note
                            newNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);

                            // Original note
                            originNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);
                            #endregion modify note bounds
                        }
                        else
                        {
                            // Assuming mouse is INSIDE bounds of note => drag note
                            #region drag note
                            draggingNote = true;

                            if (keyControlDown == false)
                                deletingNote = false;
                            else
                                deletingNote = true;
                            modifyingNote = false;
                            selectingNote = false;
                            drawingNote = false;

                            //new Note 
                            newNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);
                            //original Note 
                            originNote = new MidiNote(note.StartTime, note.Channel, note.Number, note.Duration, note.Velocity, false);


                            // Record the start point
                            aPos = PointToClient(Control.MousePosition);
                            aPos.X = aPos.X + _offsetx;

                            selectNote(originNote);
                            pnlCanvas.Invalidate();
                            #endregion drag note
                        }
                    }
                    else if (keyControlDown == false && note == null)
                    {
                        // the cursor is anywhere else
                        // Selection start
                        // Draw a selection rectangle
                        #region start sel rect  

                        selRect = new Rectangle();
                        selectingNote = true;

                        deletingNote = false;
                        drawingNote = false;
                        draggingNote = false;
                        modifyingNote = false;

                        // save start position of rectangle
                        aPos = PointToClient(Control.MousePosition);
                        pnlCanvas.Invalidate();

                        #endregion start sel rect
                    }
                    else if (keyControlDown == true && note == null)
                    {
                        // Editing mode (keyControlDown = true)

                        // Note does not exist => start note creation
                        #region start note creation
                        drawingNote = true;

                        selectingNote = false;
                        draggingNote = false;
                        modifyingNote = false;
                        deletingNote = false;

                        int nummeasure = starttime / measurelen;

                        // Temps dans la mesure
                        int rest = starttime % measurelen;
                        int timeinmeasure = rest / sequence1.Time.Quarter;

                        //timeinmeasure = sequence1.Numerator - (int)((nummeasure * measurelen - starttime) / (measurelen / sequence1.Numerator));

                        // fraction de temps
                        int fraction = sequence1.Time.Quarter / resolution;
                        int fractiontime = (rest - (timeinmeasure * sequence1.Time.Quarter)) / fraction;

                        // start time  
                        starttime = nummeasure * measurelen + sequence1.Time.Quarter * timeinmeasure + fraction * fractiontime;

                        int duration = sequence1.Time.Quarter / resolution;
                        //int channel = 0;

                        //int velocity = 100;

                        newNote = new MidiNote(starttime, channel, nnote, duration, _velocity, false);  // Start new note creation

                        pnlCanvas.Invalidate();
                        #endregion start note creation             
                    }


                    #endregion bEnterNotes = false
                }

                #endregion
            }
            else if (e.Button == MouseButtons.Right)
            {
                #region rightbutton

                if (selNotes!= null &&  selNotes.Count > 0)
                {
                    prContextMenu = new ContextMenu();
                    prContextMenu.MenuItems.Clear();

                    // Copy
                    MenuItem menuCopy = new MenuItem(Strings.Copy);
                    prContextMenu.MenuItems.Add(menuCopy);
                    menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
                    menuCopy.Shortcut = Shortcut.CtrlC;

                    // Paste
                    MenuItem menuPaste = new MenuItem(Strings.Paste);
                    prContextMenu.MenuItems.Add(menuPaste);
                    menuPaste.Click += new System.EventHandler(this.menuPaste_Click);
                    menuPaste.Shortcut = Shortcut.CtrlV;

                    // Show menu
                    prContextMenu.Show(this, this.PointToClient(Cursor.Position));
                }
                #endregion
            }

        }
        
        #region context menus
        
        /// <summary>
        /// Paste notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuPaste_Click(object sender, EventArgs e)
        {
            if (bReadyToPaste)
            {
                float ticks = sequence1.GetLength();

                // first tick of the copied notes
                MidiNote note;
                for (int i = 0; i < selNotes.Count; i++)
                {
                    note = selNotes[i];
                    if (note.StartTime < ticks)
                        ticks = note.StartTime;
                }

                // source measure of copy
                int NumMeasureorg = 1 + Convert.ToInt32(ticks) / measurelen;
                Console.Write("\nNumMeasureorg = " + NumMeasureorg);

                // Destination paste                
                Point cltPos = PointToClient(Control.MousePosition);
                int X = cltPos.X + OffsetX;

                int tickstarget = (int)(X / _xscale);

                // Numéro de mesure cible                
                int NumMeasure = 1 + Convert.ToInt32(tickstarget) / measurelen;
                Console.Write("\nNumMeasure = " + NumMeasure);

                // delta measures                    
                int deltaticks = Convert.ToInt32((NumMeasure - NumMeasureorg) * measurelen);  // ticks du début de mesure

                foreach (MidiNote n in selNotes)
                {
                    MidiNote newnote = new MidiNote(n.StartTime, n.Channel, n.Number, n.Duration, n.Velocity, false);

                    //noteMeasure = Convert.ToInt32(newnote.StartTime / measurelen);
                    ticks = newnote.StartTime + deltaticks;

                    newnote.StartTime = Convert.ToInt32(ticks);

                    track1.addNote(newnote);
                }

                if (track1.Notes.Count > 1)
                    track1.Notes.Sort(track1.Notes[0]);

                SequenceModified(sender, e);
                pnlCanvas.Invalidate();


            }
            else
            {
                MessageBox.Show("Sorry, nothing to paste");
            }
        }

        /// <summary>
        /// Copy notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCopy_Click(object sender, EventArgs e)
        {
            if (selNotes.Count > 0)
                bReadyToPaste = true;
            else
                bReadyToPaste = false;
        }

        #endregion context menus


        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {                        
            // We are in the mode "enter notes"
            if (bEnterNotes == true)
            {
                #region bEnterNotes = true

                if (deletingNote == false && drawingNote == false && draggingNote == false && selectingNote == false && modifyingNote == false)
                {
                    // Propose note Editing or deleting
                    #region cursor vsplit or vcross
                    int X = e.X + _offsetx;
                    int Y = e.Y - _TimeLineHeight;      // Offset for time line

                    int nnote = highNoteID - Y / _yscale;
                    int starttime = (int)(X / _xscale);

                    note = FindNote(nnote, starttime);
                    
                    // Display note under the mouse                    
                    // Num measure
                    int nummeasure = 1 + starttime / measurelen;
                    // Temps dans la mesure
                    int rest = starttime % measurelen;
                    
                    
                    int timeinmeasure = 1 + rest / sequence1.Time.Quarter;

                    timeinmeasure = sequence1.Numerator - (int)((nummeasure * measurelen - starttime) / (measurelen / sequence1.Numerator));

                    displayNoteValue(nnote, nummeasure, timeinmeasure);

                    // Delegate the event to the caller
                    OnMouseMoved?.Invoke(this, nnote, e);

                    // Note exists => propose to delete with a cursor cross or modify with a vsplit
                    if (note != null)
                    {
                        int deb = note.StartTime;
                        int fin = deb + note.Duration;
                        if ((starttime >= deb && starttime <= deb + 15)  || (starttime >= fin - 15 && starttime <= fin))
                        {
                            Cursor = Cursors.VSplit;
                        }
                        else
                        {
                            Cursor = Cursors.Cross;
                        }
                    }
                    else
                    {
                        // No note under the cursor => propose to create a note with the hand (should be a pen in an ideal world)
                        Cursor = Cursors.Hand;
                    }
                    #endregion cursor vsplit or vcross
                }
                else if (drawingNote == true && draggingNote == false && selectingNote == false && deletingNote == false && modifyingNote == false)
                {
                    // Continue to draw a new note
                    #region draw note
                    Cursor = Cursors.Hand;

                    int X = e.X + _offsetx;
                    int endtime = (int)(X / _xscale);

                    int delta = endtime - newNote.StartTime;
                    float ffraction = sequence1.Time.Quarter / (float)resolution;
                    int nbFractions = (int)(delta / ffraction);                                        
                 
                    int newDuration = (int)((nbFractions + 1) * ffraction);                                        
                    
                    newNote.Duration = newDuration;

                    selectNote(newNote);
                    pnlCanvas.Invalidate();
                    
                    #endregion draw note
                }
                else if (draggingNote == true && drawingNote == false && selectingNote == false && deletingNote == true && modifyingNote == false)
                {
                    // we are dragging the note "note"
                    // deletingNote is set to true because if the note is finally not dragged elsewhere, it must be deleted
                    #region drag note
                    Cursor = Cursors.Hand;

                    int X = e.X + _offsetx;

                    float deltastarttime = ((X - aPos.X) / (float)_xscale);
                    float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                    int nbFractions = (int)(deltastarttime / ffraction);

                    int Y = e.Y;
                    int number = highNoteID - Y / _yscale;
                    newNote.Number = number;


                    // Sometimes StartTime of original note is not on the grid: 
                    // needs to be recalculated if not according to grid resolution
                    int nummeasure = originNote.StartTime / measurelen;
                    // Time in measure
                    int origRest = originNote.StartTime % measurelen;
                    // beat  (si 4 temps = 1 ou 2 ou 3 ou 4)
                    int beat = origRest / sequence1.Time.Quarter;                    
                    // fraction de beat (dépend de la résolution)                    
                    float ffractiontime = (origRest - (beat * sequence1.Time.Quarter)) / ffraction;
                    int nbfraction = (int)ffractiontime;
                    // new orig start time  
                    int newsOrigStartTime = (int)(nummeasure * measurelen + sequence1.Time.Quarter * beat + (ffraction * nbfraction));

                    //newNote.StartTime = originNote.StartTime + (int)(nbFractions * ffraction);
                    newNote.StartTime = newsOrigStartTime + (int)(nbFractions * ffraction);                   

                    pnlCanvas.Invalidate();
                    #endregion drag note
                }
                else if (modifyingNote == true && draggingNote == false && drawingNote == false && deletingNote == false && selectingNote == false)
                {
                    // We are modifying the note "note"
                    #region modify note bounds
                    Cursor = Cursors.VSplit;

                    int X = (int)((e.X + _offsetx)/_xscale);

                    if (EditMode == editmode.left)
                    {
                        #region left extension
                        // Modify start time
                        int deltastarttime = X - newNote.StartTime;
                        float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                        int nbFractions = (int)(deltastarttime / ffraction);
                        int deltatime = (int)(nbFractions * ffraction);
                        if (deltatime != 0)
                        {
                            // Convert newStartTime into multiples of fraction
                            int nbFracStartTime = (int)((newNote.StartTime + deltatime)/ffraction);
                            int newStartTime = (int)(nbFracStartTime * ffraction);

                            // Save old endtime
                            int oldEndTime = newNote.EndTime;
                            //recalculate bounds
                            newNote.StartTime = newStartTime;                            
                            newNote.Duration = oldEndTime - newStartTime;

                            selectNote(newNote);
                        }
                        #endregion
                    }
                    else if (EditMode == editmode.right)
                    {
                        #region right extension
                        // Modify duration
                        int endtime = newNote.StartTime + newNote.Duration;
                        int deltaendtime = X - endtime;
                        float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                        int nbFractions = (int)(deltaendtime / ffraction);
                        int deltatime = (int)(nbFractions * ffraction);
                        if (deltatime != 0)
                        {
                            // Convert new duration into multiples of fraction
                            int NbFracDuration = (int)((newNote.Duration + deltatime)/ffraction);
                            int newDuration = (int)(NbFracDuration * ffraction);
                            
                            newNote.Duration = newDuration;

                            selectNote(newNote);
                        }
                        #endregion
                    }

                    pnlCanvas.Invalidate();
                    #endregion modify note bounds
                }
                #endregion bEnterNotes = true
            }
            else
            {
                // We are not in the mode "enter notes" 
                #region bEnterNotes = false

                if (keyControlDown == false && deletingNote == false && drawingNote == false && draggingNote == false && selectingNote == false && modifyingNote == false)
                {
                    // Change cursor to vsplit if on bounds
                    #region cursor vsplit or vcross
                    int X = e.X + _offsetx;
                    int Y = e.Y - _TimeLineHeight;      // Offset for time line

                    int nnote = highNoteID - Y / _yscale;
                    int starttime = (int)(X / _xscale);

                    note = FindNote(nnote, starttime);

                    // Display note under the mouse
                    // Num measure
                    int nummeasure = 1 + starttime / measurelen;
                    // Temps dans la mesure
                    int rest = starttime % measurelen;
                    
                    int timeinmeasure = 1 + rest / sequence1.Time.Quarter;

                    timeinmeasure = sequence1.Numerator - (int)((nummeasure * measurelen - starttime) / (measurelen / sequence1.Numerator));

                    displayNoteValue(nnote, nummeasure, timeinmeasure);

                    // Delegate the event to the caller
                    OnMouseMoved?.Invoke(this, nnote, e);

                    // Note exists => propose to modify with a vsplit
                    if (note != null)
                    {
                        int deb = note.StartTime;
                        int fin = deb + note.Duration;
                        if ((starttime >= deb && starttime <= deb + 15) || (starttime >= fin - 15 && starttime <= fin))
                        {
                            Cursor = Cursors.VSplit;
                        }
                        else
                        {
                            Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        // No note under the cursor => arrow
                        Cursor = Cursors.Default;
                    }
                    #endregion cursor vsplit or vcross
                }
                else if (keyControlDown == true && deletingNote == false && drawingNote == false && draggingNote == false && selectingNote == false && modifyingNote == false)
                {
                    // Propose note Editing or deleting
                    #region note editing or deleting
                    int X = e.X + _offsetx;
                    int Y = e.Y - TimeLineY;        // Offset for time line

                    int nnote = highNoteID - Y / _yscale;
                    int starttime = (int)(X / _xscale);

                    note = FindNote(nnote, starttime);

                    // Note exists => propose to delete with a cursor cross or modify with a vsplit
                    if (note != null)
                    {
                        int deb = note.StartTime;
                        int fin = deb + note.Duration;
                        if ((starttime >= deb && starttime <= deb + 15) || (starttime >= fin - 15 && starttime <= fin))
                        {
                            Cursor = Cursors.VSplit;
                        }
                        else
                        {
                            Cursor = Cursors.Cross;
                        }
                    }
                    else
                    {
                        // No note under the cursor => propose to create a note with the hand (should be a pen in an ideal world)
                        Cursor = Cursors.Hand;
                    }
                    #endregion note editing or deleting
                }
                else if (keyControlDown == true && drawingNote == true && draggingNote == false && deletingNote == false & selectingNote == false && modifyingNote == false)
                {
                    // Continue to draw new note
                    #region drawnote
                    int X = e.X + _offsetx;
                    int starttime = (int)(X / _xscale);
                    int delta = starttime - newNote.StartTime;
                    float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                    int nbFractions = (int)(delta / ffraction);

                    int newDuration = (int)((nbFractions + 1) * ffraction);
                   
                    newNote.Duration = newDuration;
                    pnlCanvas.Invalidate();
                  
                    #endregion drawnote

                }
                else if (selectingNote == true)
                {
                    // Continue du draw the rectangle selection
                    // this mode only if keycontroldown = false                   
                    #region selecting
                    Point pos = PointToClient(Control.MousePosition);

                    int x = Math.Min(aPos.X, pos.X);
                    int y = Math.Min(aPos.Y, pos.Y);
                    int w = Math.Abs(aPos.X - pos.X);
                    int h = Math.Abs(aPos.Y - pos.Y);

                    x = x + OffsetX;

                    selRect = new Rectangle(x, y, w, h);
                    pnlCanvas.Invalidate();
                    #endregion
                }
                else if (draggingNote == true)
                {
                    // we are dragging the note "note"
                    #region drag note
                    int X = e.X + _offsetx;

                    float deltastarttime = (X - aPos.X) / (float)_xscale;
                    float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                    int nbFractions = (int)(deltastarttime / ffraction);

                    int Y = e.Y;
                    int number = highNoteID - Y / _yscale;
                    newNote.Number = number;

                    // Sometimes StartTime of original note is not on the grid: 
                    // needs to be recalculated if not according to grid resolution
                    int nummeasure = originNote.StartTime / measurelen;
                    // Time in measure
                    int origRest = originNote.StartTime % measurelen;
                    // beat  (si 4 temps = 1 ou 2 ou 3 ou 4)
                    int beat = origRest / sequence1.Time.Quarter;
                    // fraction de beat (dépend de la résolution ch)                
                    float ffractiontime = (origRest - (beat * sequence1.Time.Quarter)) / ffraction;
                    int fractiontime = (int)ffractiontime;
                    // new orig start time  
                    int newsOrigStartTime = (int)(nummeasure * measurelen + sequence1.Time.Quarter * beat + ffraction * fractiontime);
                                        
                    newNote.StartTime = newsOrigStartTime + (int)(nbFractions * ffraction);

                    pnlCanvas.Invalidate();
                    #endregion drag note

                }
                else if (modifyingNote == true && draggingNote == false && drawingNote == false && deletingNote == false && selectingNote == false)
                {
                    // We are modifying the note "note"
                    #region modify notes bound
                    Cursor = Cursors.VSplit;

                    int X = (int)((e.X + _offsetx) / _xscale);

                    if (EditMode == editmode.left)
                    {
                        #region left extension
                        // Modify start time
                        int deltastarttime = X - newNote.StartTime;
                        float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                        int nbFractions = (int)(deltastarttime / ffraction);
                        int deltatime = (int)(nbFractions * ffraction);
                        if (deltatime != 0)
                        {
                            // Convert newStartTime into multiples of fraction
                            int nbFracStartTime = (int)((newNote.StartTime + deltatime) / ffraction);
                            int newStartTime = (int)(nbFracStartTime * ffraction);

                            // Save old endtime
                            int oldEndTime = newNote.EndTime;
                            //recalculate bounds
                            newNote.StartTime = newStartTime;
                            newNote.Duration = oldEndTime - newStartTime;
                       
                            selectNote(newNote);
                        }
                        #endregion
                    }
                    else if (EditMode == editmode.right)
                    {
                        #region right extension
                        // Modify duration
                        int endtime = newNote.StartTime + newNote.Duration;
                        int deltaendtime = X - endtime;
                        float ffraction = (float)sequence1.Time.Quarter / (float)resolution;
                        int nbFractions = (int)(deltaendtime / ffraction);
                        int deltatime = (int)(nbFractions * ffraction);
                        if (deltatime != 0)
                        {
                            // Convert new duration into multiples of fraction
                            int NbFracDuration = (int)((newNote.Duration + deltatime) / ffraction);
                            int newDuration = (int)(NbFracDuration * ffraction);

                            newNote.Duration = newDuration;                           

                            selectNote(newNote);
                        }
                        #endregion
                    }

                    pnlCanvas.Invalidate();
                    #endregion modify notes bound
                }
              

                #endregion bEnterNotes = false
            }

        }


        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (note != null)
                MouseUpStopNote(note.StartTime, note.Channel, note.Number, note.Duration, e);


            // Finish to draw note and add it
            if (newNote != null && drawingNote == true)
            {
                #region end draw note
                if (track1 != null)
                {                    
                    track1.addNote(newNote);
                }
                else
                {                    
                    Track track = sequence1.tracks[0];
                    track.addNote(newNote);
                }

                selectNote(newNote);
                SequenceModified(sender, e);

                drawingNote = false;
                newNote = null;
                #endregion

            }
            else if (selectingNote == true)
            {
                #region End select notes
                // Rectangle selection for notes
                Point pos = PointToClient(Control.MousePosition);                

                int x = Math.Min(aPos.X, pos.X);
                int y = Math.Min(aPos.Y, pos.Y);
                int w = Math.Abs(aPos.X - pos.X);
                int h = Math.Abs(aPos.Y - pos.Y);

                x = x + _offsetx;

                selRect = new Rectangle(x, y, w, h);

                selectingNote = false;

                // Calculate notes selected here
                detectSelectedNotes();

                // Redraw
                pnlCanvas.Invalidate();
                

                #endregion
            }
            else if (newNote != null && originNote != null && draggingNote == true)
            {

                #region end drag note

                // End of drag note
                // The new note is different than the original note => it is a drag
                if (newNote.Number != originNote.Number || newNote.StartTime != originNote.StartTime)
                {
                    if (track1 != null)
                    {
                        track1.deleteNote(originNote.Number, originNote.StartTime);
                       
                        track1.addNote(newNote);
                    }
                    else
                    {
                        Track track = sequence1.tracks[curTrack];
                        track.deleteNote(originNote.Number, originNote.StartTime);
                        
                        track.addNote(newNote);
                    }

                    // Select new note at new place
                    selectNote(newNote);
                    SequenceModified(sender, e);
                    pnlCanvas.Invalidate();

                }
                else if (newNote.Number == originNote.Number && newNote.StartTime == originNote.StartTime && deletingNote == true)
                {
                    // the new note is equal to the original note => it is a delete
                    if (track1 != null)
                    {
                        track1.deleteNote(originNote.Number, originNote.StartTime);
                    }
                    else
                    {
                        Track track = sequence1.tracks[curTrack];
                        track.deleteNote(originNote.Number, originNote.StartTime);
                    }
                    SequenceModified(sender, e);
                    pnlCanvas.Invalidate();

                }

                draggingNote = false;
                newNote = null;
                originNote = null;
                #endregion
            }
            else if (newNote != null && originNote != null && modifyingNote == true)
            {
                #region end modification startime or endtime
                // end of modification of start time or end time of a note
                if (newNote.StartTime != originNote.StartTime || newNote.Duration != originNote.Duration)
                {
                    if (track1 != null)
                    {
                        track1.deleteNote(originNote.Number, originNote.StartTime);
                        
                        track1.addNote(newNote);
                    }
                    else
                    {
                        Track track = sequence1.tracks[curTrack];
                        track.deleteNote(originNote.Number, originNote.StartTime);
                        
                        track.addNote(newNote);
                    }
                    SequenceModified(sender, e);
                    pnlCanvas.Invalidate();
                }

                newNote = null;
                originNote = null;

                #endregion
            }

            drawingNote = false;
            deletingNote = false;
            selectingNote = false;
            draggingNote = false;
            modifyingNote = false;

        }

        /// <summary>
        /// Mouse selection is finished, set selected notes to true and others to false
        /// </summary>
        private void detectSelectedNotes()
        {
            if (track1 != null)
            {

                selNotes = new List<MidiNote>();

                foreach (MidiNote n in track1.Notes)
                {
                    int X = (int)(n.StartTime * _xscale);
                    int Y = (highNoteID - n.Number) * _yscale; 
                    int W = (int)(n.Duration * _xscale);
                    int H = _yscale;
                    Rectangle rectn = new Rectangle(X, Y, W, H);

                    if (selRect.Contains(rectn))
                    {
                        n.Selected = true;
                        selNotes.Add(n);
                    }
                    else
                    {
                        n.Selected = false;
                    }
                }
            }
        }

        private void selectNote(MidiNote n)
        {            
            int x = (int)(n.StartTime * _xscale);
            int y = (highNoteID - n.Number) * _yscale;  //127 - notenumber
            int w = (int)(n.Duration * _xscale);
            int h = _yscale;
            selRect = new Rectangle(x, y, w, h);

            detectSelectedNotes();
            
        }

        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            this.Capture = false;
        }


        
        private void OnMouseDownPlayNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            if (MouseDownPlayNote != null)
            {
                MouseDownPlayNote(starttime, channel, nnote, duration, e);
            }
        }

        private void MouseDownPlayNote2(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            // Nothing ?
        }

 

        private void OnMouseUpStopNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            if (MouseUpStopNote != null)
            {
                MouseUpStopNote(starttime, channel, nnote, duration, e);
            }
        }

        private void MouseUpStopNote2(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            // Nothing ?
        }

        private void InfoNote2(string tx)
        {
            // Nothing ?
        }

        #endregion Mouse


        #region Protected events
    
        protected override void OnResize(EventArgs e)
        {            
            pnlCanvas.Invalidate();
            
            base.OnResize(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private void OnSequenceModified(Object sender, EventArgs e)
        {
            if (SequenceModified != null)
                SequenceModified(sender, e);
        }

        
        private void OnMyMouseDown(object sender, MouseEventArgs e)
        {
            if (MyMouseDown != null)
                MyMouseDown(sender, e);
        }

        private void OnMyMouseUp(object sender, MouseEventArgs e)
        {
            if (MyMouseUp != null)
                MyMouseUp(sender, e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.X < 0 || e.X > Width || e.Y < 0 || e.Y > Height)
            {
                Capture = false;
            }          
            base.OnMouseMove(e);
        }

        private void OnInfoNote(string tx)
        {
            InfoNote?.Invoke(tx);
        }

        #endregion Protected events


        #region vertical bar
        public void setTimeVLinePos(int pos)
        {
            TimeVLine.Height = pnlCanvas.Height - _TimeLineHeight;
            TimeVLine.Location = new Point(pos, _TimeLineHeight - _offsety);
        }        
        
        /// <summary>
        /// Red bar time line
        /// </summary>
        /// <param name="pos"></param>
        private void DrawVbar(int pos)
        {
            TimeVLine = new Panel();
            TimeVLine.Enabled = false;
            TimeVLine.Height = pnlCanvas.Height - _TimeLineHeight;
            TimeVLine.Width = 2;
            TimeVLine.Location = new Point(pos, _TimeLineHeight);
            TimeVLine.BackColor = Color.Red;
            pnlCanvas.Controls.Add(TimeVLine);
            TimeVLine.BringToFront();
        }

        #endregion vertical bar

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

 
        /// <summary>
        /// Raise event on note to display 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nummeasure"></param>
        /// <param name="timeinmeasure"></param>
        private void displayNoteValue(int number, int nummeasure, int timeinmeasure)
        {
            int idx = number % 12;
            int octave = (number + 3) / 12 - 1;

            string timevalue = nummeasure.ToString() + "." + timeinmeasure.ToString();
            
            string tx = timevalue + "    " + NotesTable[idx] + octave.ToString();
            InfoNote(tx);
        }

    }
}
