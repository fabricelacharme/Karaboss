using ChordAnalyser.Properties;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChordAnalyser.UI
{
    public partial class ChordsMapControl : Control
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

        #region properties

        private int _offsety = 0;
        /// <summary>
        /// Gets or sets horizontal offset
        /// </summary>
        public int OffsetY
        {
            get { return _offsety; }
            set
            {
                if (value != _offsety)
                {
                    _offsety = value;
                    if (OffsetChanged != null)
                        OffsetChanged(this, _offsety);
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
                    if (WidthChanged != null)
                    {
                        Width = _maxstaffwidth;
                        WidthChanged(this, _maxstaffwidth);
                    }
                }
            }
        }

        private int _maxstaffheight;
        /// <summary>
        /// Gets Length of score
        /// </summary>
        public int maxStaffHeight
        {
            get { return _maxstaffheight; }
            set
            {
                if (value != _maxstaffheight)
                {
                    _maxstaffheight = value;
                    if (HeightChanged != null)
                    {
                        Height = _maxstaffheight;
                        HeightChanged(this, _maxstaffheight);
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
                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                pnlCanvas.Invalidate();
            }
        }

        #endregion properties


        public ChordsMapControl()
        {
            // Draw pnlCanvas
            DrawCanvas();

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }

        #region Draw Canvas

        /// <summary>
        /// Redraw Canvas
        /// </summary>
        public void Redraw()
        {
            pnlCanvas.Invalidate();
        }

        /// <summary>
        /// Add panel to the control
        /// </summary>
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

        /// <summary>
        /// Draw cells on the panel: 4 cells/measure by line
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int max = 3;

            int _MeasureSeparatorWidth = 2;
            int _LinesWidth = 2;

            Color blackKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Color TimeLineColor = Color.White;

            Pen mesureSeparatorPen = new Pen(Color.Black, _MeasureSeparatorWidth);
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);
            Rectangle rect;
            Point p1;
            Point p2;
            int x = 0;
            int y = 0;

            int compteurmesure = -1;

            FillPen = new Pen(Color.Gray, _LinesWidth);

            // ********************************
            // 1st place = false measuree
            // ********************************
            for (int j = 0; j < sequence1.Numerator - 1; j++)
            {
                g.DrawRectangle(FillPen, x, y, _cellsize, _cellsize);
                rect = new Rectangle(x, 0, (int)(_cellsize), (int)(_cellsize));
                g.FillRectangle(new SolidBrush(Color.Gray), rect);
                x += (int)(_cellsize) + (_LinesWidth - 1);
            }

            // =====================================================
            // 1ere case noire en plus de celles du morceau
            //======================================================            
            g.DrawRectangle(FillPen, x, 0, _cellsize, _cellsize);
            rect = new Rectangle(x, 0, (int)(_cellsize), (int)(_cellsize));
            g.FillRectangle(new SolidBrush(Color.Black), rect);

            var src = new Bitmap(Resources.silence_white);
            var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);
            g.DrawImage(src, new Rectangle(x + 10, 10, bmp.Width, bmp.Height));



            // init variables
            compteurmesure = 0;
            x = ((int)(_cellsize) + (_LinesWidth - 1)) * sequence1.Numerator;

            // ********************
            // Begin at 2nd place
            // ********************
            for (int i = 1; i <= NbMeasures; i++)
            {
                compteurmesure++;
                if (compteurmesure > max)   // 4 measures per line
                {
                    y +=  (int)_cellsize + 1;
                    x = 0;
                    compteurmesure = 0;
                }
                
                
                // Dessine autant de cases que le numerateur
                for (int j = 0; j < sequence1.Numerator; j++)
                {
                    // Draw played cell in gray
                    if (i == _currentmeasure - 1 && j == _currentTimeInMeasure - 1 && _currentpos > 0)
                    {
                        g.DrawRectangle(FillPen, x, y, _cellsize, _cellsize);
                        rect = new Rectangle(x, 0, (int)(_cellsize), (int)(_cellsize));
                        g.FillRectangle(new SolidBrush(Color.Gray), rect);

                    }
                    else
                    {
                        // Draw other celles in white                        
                        g.DrawRectangle(FillPen, x, y, _cellsize, _cellsize);
                    }
                    x += (int)(_cellsize) + (_LinesWidth - 1);
                }
            }


            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================                        
            x = sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1));
            y = 0;
            compteurmesure = -1;

            for (int i = 0; i <= NbMeasures + 1; i++)
            {
                compteurmesure++;
                if (compteurmesure > max)
                {
                    y += (int)_cellsize + 1;
                    x = sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1));
                    compteurmesure = 0;
                }

                if (i % (max + 1) != 0)
                {
                    p1 = new Point(x, y);
                    p2 = new Point(x, y + (int)(_cellsize));
                    g.DrawLine(mesureSeparatorPen, p1, p2);
                    x += sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1));
                }
            }

             maxStaffHeight = (int)( ((int)_cellsize + 1) *   (1 +  Math.Ceiling((double)((NbMeasures + 1)/(max + 1)) )   )   );
             maxStaffWidth = (sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1))) * (max + 1);

        }

        #endregion draw canvas


        #region drawnotes 

        /// <summary>
        /// Draw the name of the notes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawNotes(Graphics g, Rectangle clip)
        {
            int max = 3;
            int compteurmesure = 0;

            SolidBrush ChordBrush = new SolidBrush(Color.Black);
            SolidBrush MeasureBrush = new SolidBrush(Color.Red);

            Font fontChord = new Font("Arial", 16 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontMeasure = new Font("Arial", 12 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

            int _LinesWidth = 2;            
            
            // Start after the 1st false measure
            int x = ((int)(_cellsize) + (_LinesWidth - 1)) * sequence1.Numerator;
            int y_chord = ((int)(_cellsize) / 2) - (fontMeasure.Height / 2);
            int y_symbol = 10;
            int y_measurenumber = (int)(_cellsize) - fontMeasure.Height;

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

                    compteurmesure++;
                    if (compteurmesure > max)   // 4 measures per line
                    {
                        y_chord += (int)_cellsize + 1;
                        y_symbol += (int)_cellsize + 1;
                        y_measurenumber += (int)_cellsize + 1;
                        x = 0;
                        compteurmesure = 0;
                    }

                    // Chord name
                    p1 = new Point(x + Offset, y_chord);

                    ttx = Gridchords[i];
                    tx = ttx.Item1;

                    // If empty, draw symbol
                    if (tx == EmptyChord)
                    {
                        g.DrawImage(src, new Rectangle(p1.X, y_symbol, bmp.Width, bmp.Height));

                    }
                    else
                    {
                        g.DrawString(tx, fontChord, ChordBrush, p1.X, p1.Y);
                    }

                    // Draw measure number
                    tx = i.ToString();
                    p1 = new Point(x + Offset, y_measurenumber);
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
                                g.DrawImage(src, new Rectangle(p1.X + z, y_symbol, bmp.Width, bmp.Height));
                            }
                            else
                            {
                                g.DrawString(tx, fontChord, ChordBrush, p1.X + z, y_chord);
                            }
                        }
                    }


                    // Increment x (go to next measure)
                    x += ((int)(_cellsize) + (_LinesWidth - 1)) * sequence1.Numerator;

                }
            }
        }

        #endregion drawnotes


        #region mouse
        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void pnlCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void pnlCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void pnlCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        #endregion mouse


        #region paint
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clip =
                 new Rectangle((int)(_offsety),
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


        #endregion paint

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
