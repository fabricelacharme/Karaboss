using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChordAnalyser.UI
{
    public partial class ChordRenderer : Control
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

        private string EmptyChord = "<Empty>";
        private float _cellwidth;
        private float _cellheight;

        private Font m_font;
        private StringFormat sf;

        #endregion private


        #region properties        

        // Chords
        public Dictionary<int, (string, string)> Gridchords { get; set; }

        private Font _fontChord;
        public Font fontChord
        {
            get { return _fontChord; }
            set
            {
                _fontChord = value;
                pnlCanvas.Invalidate();
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



        private int _columnwidth = 80;
        public int ColumnWidth
        {
            get { return _columnwidth; }
            set
            {
                _columnwidth = value;
                _cellwidth = _columnwidth * zoom;
                if (WidthChanged != null)
                    WidthChanged(this, this.Width);
                pnlCanvas.Invalidate();

            }
        }

        private int _columnheight = 80;
        public int ColumnHeight
        {
            get { return _columnheight; }
            set
            {
                _columnheight = value;
                _cellheight = _columnheight * zoom;

                this.Height = (int)(_cellheight);
                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                pnlCanvas.Invalidate();
            }
        }


        private int _maxstaffwidth = 80;
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
                if (value <= 0.0f)
                    return;

                _zoom = value;

                _cellwidth = _columnwidth * zoom;
                _cellheight = _columnheight * zoom;

                _fontChord = new Font(_fontChord.FontFamily, 40 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

                this.Height = (int)_cellheight;
                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                // No need to manage width: controls position on frmChords depends only on its height

                pnlCanvas.Invalidate();
            }
        }

        #endregion properties


        public ChordRenderer()
        {
            _fontChord = new Font("Arial", 40, FontStyle.Regular, GraphicsUnit.Pixel);

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

        private void DrawCanvas()
        {
            Height = (int)_columnheight;

            // Draw pnlCanvas            
            pnlCanvas = new MyPanel();
            pnlCanvas.Location = new Point(0, 0);
            pnlCanvas.Size = new Size(40, Height);
            pnlCanvas.BackColor = Color.White;
            pnlCanvas.Dock = DockStyle.Fill;

            pnlCanvas.Paint += new PaintEventHandler(pnlCanvas_Paint);            

            this.Controls.Add(pnlCanvas);
        }

        private void DrawGrid(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;

            int x = 0;
            int _LinesWidth = 2;
            Color TimeLineColor = Color.White;
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);
            


            for (int i = 0; i < Gridchords.Count; i++)
            {
                g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);

                // Increase length of control
                x += (int)(_cellwidth) + (_LinesWidth - 1);
            }

            maxStaffWidth = x;

        }

        #endregion Draw Canvas

        #region drawnotes   

        /// <summary>
        /// Draw the name of the notes
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawNotes(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;

            SolidBrush ChordBrush = new SolidBrush(Color.FromArgb(29, 29, 29));

            (string, string) ttx;
            string ChordName;
            float w;
            float h;
            int _LinesWidth = 2;
            int x = (_LinesWidth - 1);

            for (int i = 1; i <= Gridchords.Count; i++)
            {
                ttx = Gridchords[i];
                ChordName = ttx.Item1;
                w = MeasureString(_fontChord.FontFamily, ChordName, _fontChord.Size);
                h = MeasureStringHeight(_fontChord.FontFamily, ChordName, _fontChord.Size);

                if (ChordName != EmptyChord)
                {
                    g.DrawString(ChordName, _fontChord, ChordBrush, x + (_cellwidth - w) / 2, (_cellheight / 2 - h) / 2);
                }

                x += (int)(_cellwidth) + _LinesWidth;

            }
        }

        #endregion drawnotes


        #region paint

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {                        
            if (Gridchords !=null && Gridchords.Count > 0)
            {
                Rectangle clip =
                    new Rectangle(
                    (int)(_offsetx),
                    (int)(e.ClipRectangle.Y),
                    (int)(e.ClipRectangle.Width),
                    (int)(e.ClipRectangle.Height));

                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


                g.TranslateTransform(-clip.X, 0);

                DrawGrid(g, clip);

                DrawNotes(g, clip);

                g.TranslateTransform(clip.X, 0);
            }
            
        }

        #endregion paint


        #region Protected events

        protected override void OnResize(EventArgs e)
        {
            if (pnlCanvas != null)
                pnlCanvas.Invalidate();

            base.OnResize(e);
        }

        protected override void Dispose(bool disposing)
        {            
            base.Dispose(disposing);
        }

        #endregion Protected events


        #region mesures
        /// <summary>
        /// Measure the length of a string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fSize"></param>
        /// <returns></returns>
        private float MeasureString(FontFamily fnt, string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pnlCanvas.CreateGraphics())
                {
                    m_font = new Font(fnt, femSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    SizeF sz = g.MeasureString(line, m_font, new Point(0, 0), sf);
                    ret = sz.Width;

                    g.Dispose();
                }

            }
            return ret;
        }

        /// <summary>
        /// Measure the height of a string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="femSize"></param>
        /// <returns></returns>
        private float MeasureStringHeight(FontFamily fnt, string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pnlCanvas.CreateGraphics())
                {

                    if (femSize > 0)
                        m_font = new Font(fnt, femSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    SizeF sz = g.MeasureString(line, m_font, new Point(0, 0), sf);
                    ret = sz.Height;

                    g.Dispose();
                }
            }
            return ret;
        }

        #endregion mesures

    }
}
