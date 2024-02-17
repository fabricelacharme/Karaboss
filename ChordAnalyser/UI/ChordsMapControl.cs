using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            int max = sequence1.Numerator - 1;

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
            for (int i = 0; i < NbMeasures; i++)
            {
                compteurmesure++;
                if (compteurmesure > max)
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
            x = 0;
            y = 0;
            compteurmesure = -1;

            for (int i = 0; i < NbMeasures; i++)
            {
                compteurmesure++;
                if (compteurmesure > max)
                {
                    y += (int)_cellsize + 1;
                    x = 0;
                    compteurmesure = 0;
                }

                p1 = new Point(x, y);
                p2 = new Point(x, y + (int)(_cellsize));
                g.DrawLine(mesureSeparatorPen, p1, p2);
                x += sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1));
            }

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
