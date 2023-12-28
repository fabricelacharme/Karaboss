using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

namespace ChordAnalyser.UI
{

    #region delegate    
    public delegate void OffsetChangedEventHandler(object sender, int value);   
    #endregion delegate


    public partial class ChordsControl : Control
    {

        #region events
        public event OffsetChangedEventHandler OffsetChanged;


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

        #endregion properties


        #region private
        private MyPanel pnlCanvas;

        private int measurelen = 0;

        #endregion private


        public ChordsControl()
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


        private void DrawGrid(Graphics g, Rectangle clip)
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
            g.DrawRectangle(FillPen, clip.X, clip.Y, W, h);
            rect = new Rectangle(clip.X, clip.Y, W, h);
            FillBrush = new SolidBrush(TimeLineColor);
            g.FillRectangle(FillBrush, rect);

            // Black line separator
            FillPen = new Pen(Color.Black, 3);
            p1 = new Point(clip.X, clip.Y + _TimeLineHeight - 2);
            p2 = new Point(clip.X + W, clip.Y + _TimeLineHeight - 2);
            g.DrawLine(FillPen, p1, p2);


        }



        #endregion Draw Canvas


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

                g.TranslateTransform(clip.X, 0);

            }
        }
        #endregion Mouse


    }
}
