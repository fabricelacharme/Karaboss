using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.PianoRoll
{
    public partial class VTimeLine : Control
    {


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




        #region private decl

        private MyPanel pnlCanvas;
        private int measurelen = 0;
        private float lastPosition = 0;
        private int keysNumber;

        #endregion

        #region properties

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



        private float _zoomy = 1.0f;    // zoom for vertical
        /// <summary>
        /// Gets or sets zoom value
        /// </summary>
        [Description("Gets or sets the vertical zoom")]
        [Category("VTimeLine")]
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


        private int maxstafflength;
        /// <summary>
        /// Gets Length of score
        /// </summary>
        public int maxStaffLength
        {
            get { return maxstafflength; }

        }

        #endregion


        public VTimeLine()
        {


            _zoomy = 1.0f;   // valeur de zoom
            resolution = 4;  // 4 incréments par noire

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


        #region events

        private void pnlCanvas_MouseLeave(object sender, EventArgs e)
        {
            this.Capture = false;
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
                -offsety,
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
                    
                }
                else
                {
                    CreateBackgroundCanvas(g, clip);
                    DrawGrid(g, clip);
                    
                }
                g.TranslateTransform(0, clip.Y);
            }

        }

        #endregion


        #region protected

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

        #endregion


        #region canvas

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
            H = clip.Height;

            
            // ==========================
            // Backgroud color : grey
            // ==========================            
            FillPen = new Pen(whiteKeysColor);
            g.DrawRectangle(FillPen, clip.X, clip.Y, Width, H);
            rect = new Rectangle(clip.X, clip.Y, Width, H);
            FillBrush = new SolidBrush(whiteKeysColor);
            g.FillRectangle(FillBrush, rect);          
          
        }


        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int step = 0;

            int timespermeasure = sequence1.Numerator;             // nombre de beats par mesures
            float TimeUnit = Sequence1.Denominator;            // 2 = blanche, 4 = noire, 8 = croche, 16 = doucle croche, 32 triple croche

            Pen mesureSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF676767"), 1);
            Pen beatSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF585858"), 1);
            Pen intervalSeparatorPen = new Pen(System.Drawing.ColorTranslator.FromHtml("#FF464646"), 1);

            // quarter = durée d'une noire
            int quarter = sequence1.Time.Quarter;

            float f_n = 0;

            // Increment of 1 TimeUnit, divided by the resolution, in ticks
            float f_beat = (float)quarter * 4 / TimeUnit;
            float f_increment = f_beat / resolution;
            
            int PH = pnlCanvas.Height;

            // Measure number display
            int NumMeasure = 0;
            SolidBrush textBrush = new SolidBrush(Color.DimGray);
            Font fontMeasure = new Font("Arial", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontInterval = new Font("Arial", 15, FontStyle.Regular, GraphicsUnit.Pixel);

            int pico = 0;

            do
            {
                int y1 = PH - (int)(f_n * yscale);
                int y2 = y1;
                int x1 = 0;
                int x2 = Width;

                if (y1 >= clip.Y && y1 <= clip.Y + clip.Height)
                {
                    Point p1 = new Point(x1, y1);
                    Point p2 = new Point(x2, y2);


                    if (step % (timespermeasure * resolution) == 0)        // every measure
                    {
                        // Display line
                        g.DrawLine(mesureSeparatorPen, p1, p2);
                        // Display measure number
                        NumMeasure = 1 + (int)(f_n / measurelen);
                        g.DrawString("Measure " + NumMeasure, fontMeasure, textBrush, p1.X + 5, p1.Y - fontMeasure.Height);
                    }
                    else if (step % (resolution) == 0)                       // every time or beat
                    {
                        g.DrawLine(beatSeparatorPen, p1, p2);

                        NumMeasure = 1 + (int)(f_n / measurelen);
                        pico = 1 + sequence1.Numerator - (int)((NumMeasure * measurelen - f_n) / (measurelen / sequence1.Numerator));

                        g.DrawString(NumMeasure + "." + pico, fontInterval, textBrush, p1.X + 5, p1.Y - fontInterval.Height);
                    }
                    else
                    {
                        g.DrawLine(intervalSeparatorPen, p1, p2);          // every resolution
                    }
                }

                // increment = beat divisé par la resolution
                // Tous les resolution, on a un beat
                // tous les timepermeasure on a une nouvelle mesure 
                //
                // une mesure = timepermeasure * f_beat
                // un beat = f_beat
                // intermédiaire = increment
                f_n += f_increment;
                step++;

            } while (f_n <= lastPosition);


        }

        #endregion


    }
}
