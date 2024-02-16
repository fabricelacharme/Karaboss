using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChordAnalyser.Properties;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.PianoRoll;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using static ChordsAnalyser.cscales.scales;

namespace ChordAnalyser.UI
{

    #region delegate    
    public delegate void OffsetChangedEventHandler(object sender, int value);   
    public delegate void WidthChangedEventHandler(object sender, int value);
    public delegate void HeightChangedEventHandler(object sender, int value);

    #endregion delegate


    public partial class ChordsControl : Control
    {

        #region events
        public event OffsetChangedEventHandler OffsetChanged;
        public event WidthChangedEventHandler WidthChanged;
        public event HeightChangedEventHandler HeightChanged;

        #endregion events

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

        #region properties

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

       
        private int _maxstaffwidth;
        /// <summary>
        /// Gets Length of score
        /// </summary>
        public int maxStaffWidth
        {
            get { return _maxstaffwidth; }
            set 
            {                 
                if (value != _maxstaffwidth)
                {
                    _maxstaffwidth = value;
                    if(WidthChanged != null)
                    {
                        Width = _maxstaffwidth;
                        WidthChanged(this, _maxstaffwidth);
                    }
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
                {
                    UpdateMidiTimes();
                    Redraw();
                }
            }
        }

        public Dictionary<int, (string, string)> Gridchords { get; set; }


        private float _cellsize = 80;
        public float CellSize
        {
            get { return _cellsize; }
        }

        /// <summary>
        /// zoom
        /// </summary>
        private float _zoom = 1.0f;    // zoom for horizontal
        public float zoom
        {
            get
            { return _zoom; }
            set
            {                
                _zoom = value;
                _cellsize = 80 * zoom;
                this.Height = (int)(_cellsize);
                HeightChanged(this, this.Height);
                pnlCanvas.Invalidate();                               
            }
        }

        #endregion properties


        #region private
        private MyPanel pnlCanvas;        

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;

        private int _currentpos = 0;
        private int _currentmeasure = -1;
        private int _currentTimeInMeasure = -1;

        private string NoChord = "<Chord not found>";
        private string EmptyChord = "<Empty>";

        #endregion private


        #region Events
        //public delegate void MouseDownEventHandler(object sender, MouseEventArgs e);
        //public event EventHandler MouseDownHandler;

        #endregion events

        public ChordsControl()
        {

            

            // Draw pnlCanvas
            DrawCanvas();         

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        private void ChordControl_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();            
        }


        #region Draw Canvas

        /// <summary>
        /// Redraw Canvas
        /// </summary>
        public void Redraw()
        {
            pnlCanvas.Invalidate();
        }



        private void DrawCanvas()
        {
            // Draw pnlCanvas            
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

        }
              
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int _MeasureSeparatorWidth = 2;
            int _LinesWidth = 2;

            Color blackKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Color TimeLineColor = Color.White;

            Pen mesureSeparatorPen = new Pen(Color.Black, _MeasureSeparatorWidth);
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);

            //SolidBrush FillBrush;            
            Rectangle rect;

            Point p1;
            Point p2;

            int h = 0;
            int H = 0;
            int W = 0;

            W = clip.Width;

            // ==========================
            // Draw Timeline background color
            // Dessiner en dernier
            // ========================== 
            int x = 0;

            // =====================================================
            // 1ere case noire en plus ce celles du morceau
            //======================================================            
            g.DrawRectangle(FillPen, 0, 0, _cellsize, _cellsize);
            rect = new Rectangle(0, 0, (int)(_cellsize), (int)(_cellsize));
            g.FillRectangle(new SolidBrush(Color.Black), rect);

            var src = new Bitmap(Resources.silence_white);
            var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);
            g.DrawImage(src, new Rectangle(10, 10, bmp.Width, bmp.Height));


            x += (int)(_cellsize) + (_LinesWidth - 1);

            // ======================================================
            // Draw measures
            // ======================================================
            // 4 temps = 4 carrés gris
            // Chaque mesure, une ligne verticale gris foncé

            FillPen = new Pen(Color.Gray, _LinesWidth);

            for (int i = 0; i < NbMeasures; i++)
            {

                // Dessine autant de cases que le numerateur
                for (int j = 0; j < sequence1.Numerator; j++)
                {

                    // Draw played cell in gray
                    if (i == _currentmeasure - 1 && j == _currentTimeInMeasure - 1 && _currentpos > 0)
                    {
                        g.DrawRectangle(FillPen, x, 0, _cellsize, _cellsize);
                        rect = new Rectangle(x, 0, (int)(_cellsize), (int)(_cellsize));
                        g.FillRectangle(new SolidBrush(Color.Gray), rect);

                    }
                    else
                    {
                        // Draw other celles in white
                        //g.DrawRectangle(FillPen, clip.X + x, clip.Y, _TimeLineHeight, _TimeLineHeight);
                        g.DrawRectangle(FillPen, x, 0, _cellsize, _cellsize);
                    }
                    x += (int)(_cellsize) + (_LinesWidth - 1);
                }
            }

            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================
            x = (int)(_cellsize) + (_LinesWidth - 1);
            for (int i = 0; i < NbMeasures; i++)
            {
                p1 = new Point(x, clip.Y);
                p2 = new Point(x, clip.Y + (int)(_cellsize));
                g.DrawLine(mesureSeparatorPen, p1, p2);
                x += sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1));

            }


            maxStaffWidth = x;
        }


        #endregion Draw Canvas


        #region public

        /// <summary>
        /// Paint black current time played
        /// </summary>
        /// <param name="pos"></param>
        public void DisplayNotes(int pos, int measure, int timeinmeasure)
        {
            _currentpos = pos;
            _currentmeasure = measure;
            _currentTimeInMeasure = timeinmeasure;

            
            this.Redraw();
        }

        #endregion public


        #region drawnotes   

        /// <summary>
        /// Draw the name of the notes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawNotes(Graphics g, Rectangle clip)
        {
            SolidBrush ChordBrush = new SolidBrush(Color.Black);
            SolidBrush MeasureBrush = new SolidBrush(Color.Red);

            Font fontChord = new Font("Arial", 16 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontMeasure = new Font("Arial", 12 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

            int _LinesWidth = 2;
            int x = (int)(_cellsize) + (_LinesWidth - 1);
            Point p1;


            if (Gridchords != null)
            {
                (string, string) ttx;
                string tx = string.Empty;
                int Offset = 4;

                var src = new Bitmap(Resources.silence_black);
                var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);

                for (int i = 1; i <= Gridchords.Count; i++)
                {
                    // Chord name
                    p1 = new Point(x + Offset, ((int)(_cellsize) / 2) - (fontMeasure.Height / 2));

                    ttx = Gridchords[i];
                    tx = ttx.Item1;

                    // If empty, draw symbol
                    if (tx == EmptyChord)
                    {
                        g.DrawImage(src, new Rectangle(p1.X, 10, bmp.Width, bmp.Height));

                    }
                    else
                    {
                        g.DrawString(tx, fontChord, ChordBrush, p1.X, p1.Y);
                    }

                    // Draw measure number
                    tx = i.ToString();
                    p1 = new Point(x + Offset, (int)(_cellsize) - fontMeasure.Height);
                    g.DrawString(tx, fontMeasure, MeasureBrush, p1.X, p1.Y);

                    // ===============================
                    // Second part of mesure
                    // ==============================
                    if (sequence1.Numerator % 2 == 0)
                    {
                        if (ttx.Item1 != ttx.Item2)
                        {
                            tx = ttx.Item2;
                            int z = ((int)(_cellsize) + (_LinesWidth - 1)) * sequence1.Numerator / 2;

                            // If empty, draw symbol
                            if (tx == EmptyChord)
                            {
                                g.DrawImage(src, new Rectangle(p1.X + z, 10, bmp.Width, bmp.Height));
                            }
                            else
                            {
                                g.DrawString(tx, fontChord, ChordBrush, p1.X + z, (_cellsize / 2) - (fontMeasure.Height / 2));
                            }
                        }
                    }


                    // Increment x (go to next measure)
                    x += ((int)(_cellsize) + (_LinesWidth - 1)) * sequence1.Numerator;

                }
            }
        }

        #endregion drawnotes


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

        #endregion Protected events
   



        #region Mouse
        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            OnMouseDown(e);
        }

        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

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
                DrawGrid(g, clip);

                DrawNotes(g, clip);                

                g.TranslateTransform(clip.X, 0);

            }
        }
        #endregion Mouse


        #region Midi

        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                NbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer
            }
        }

        #endregion Midi

    }
}
