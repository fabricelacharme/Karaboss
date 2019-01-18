#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.Drawing;
using System.Windows.Forms;

namespace Sanford.Multimedia.Midi.Score
{
    public partial class tlControl : UserControl
    {
        
        Panel panel1;

        #region Declare Delegate
        public delegate void OntlControlPanelClickHandler(object sender, EventArgs e, int newX);
        public event OntlControlPanelClickHandler OntlControlPanelClick;

        #endregion Declare Delegate

        private int w = 1;  // epaisseur du trait
        private int newmesurelen = 0;   // largeur mesure selon zoom
        private int currentmesure = 0;
        private int delta;
        private Point p1;
        private Point p2;




        #region properties

        private int measures = 0;

        private Sequence sequence1;
        public Sequence Sequence1
        {
            set
            {
                sequence1 = value;
                
                // Width of control must be a multiple of measures
                float lastPosition = sequence1.GetLength();
                //Entier immédiatement suppérieur au nombre à virgule flottante
                measures = Convert.ToInt32(Math.Ceiling(lastPosition / sequence1.Time.Measure));
                maxstaffwidth = (int)(measures * _zoom * _l);
                panel1.Invalidate();
            }
        }
        

        private int _l;  //largeur fixe mesure
        public int mesurelen
        {
            get { return _l; }
            set 
            {
                if (value > 4 * w)
                {
                    _l = value;
                    maxstaffwidth = (int)(measures * _zoom * _l);
                }
            }
        }

        private float _zoom = 1.0f;    // zoom
        public float zoom
        {
            get
            {  return _zoom;   }
            set
            {
                if (value > 4.0f * w / _l)          // mesure ne peut pas être inférieur à 4w (4 = 2 barres verticales + espace milieu + espace avant ou après) 
                {
                    _zoom = value;
                    maxstaffwidth = (int)(measures * _zoom * _l);
                    panel1.Invalidate();
                }
            }
        }

        private Color _pencolor;
        public Color pencolor
        {
            get { return _pencolor; }
            set { _pencolor = value; }
        }

        private Color _textcolor;
        public Color textColor
        {
            get { return _textcolor; }
            set { _textcolor = value; }

        }

        // Horizontal Offset (Horizontal scrollbar)
        private int offsetx = 0;
        public int OffsetX
        {
            get { return offsetx; }
            set
            {
                if (value != offsetx)
                {
                    offsetx = value;
                    panel1.Invalidate();
                }
            }
        }

        private int maxstaffwidth;
        public int maxStaffWidth
        {
            get { return maxstaffwidth; }

        }

        #endregion properties


        public tlControl()
        {
            InitializeComponent();
            
            panel1 = new Panel();
            panel1.Location = new Point(0, 0);
            panel1.Size = new Size(0, 20);
            panel1.Dock = DockStyle.Fill;
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            this.Controls.Add(panel1);

            w = 1;
            delta = w / 2;
            _zoom = 1.0f;   // valeur de zoom
            _l = 80;        // largeur mesure
            _pencolor = Color.White;
            _textcolor = Color.White;
            panel1.BackColor = Color.Black;
            currentmesure = 0;

            this.DoubleBuffered = true;

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }
      

        public void Redraw()
        {
            panel1.Invalidate();
        }

        /// <summary>
        /// Draw main grid
        /// </summary>
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            //int X = panel1.Width;
            int Y = panel1.Height;            
            
            // Crayon
            Pen selPen = new Pen(_pencolor, w);            
            
            // Ligne sup
            p1 = new Point(clip.X, delta);
            p2 = new Point(clip.X + clip.Width, delta);
            g.DrawLine(selPen, p1, p2);

            // Ligne inf    
            int moins = delta;
            if (w == 1)
                moins = 1;
            p1 = new Point(clip.X, Y-moins);
            p2 = new Point(clip.X + clip.Width, Y-moins);
            g.DrawLine(selPen, p1, p2);

            // Largeur mesure
            newmesurelen = Convert.ToInt32(_zoom*_l);
            // mesure ne peut pas être inférieur à 4w (4 = 2 barres verticales + espace milieu + espace avant ou après)            

            if (newmesurelen > 0)
            {
                // Nombre mesures
                //int nbmesures = X / newmesurelen;

                StringFormat formatter = new StringFormat();
                formatter.LineAlignment = StringAlignment.Center;
                formatter.Alignment = StringAlignment.Center;
                string Text = string.Empty;

                
                for (int i = 0; i <= measures; i++)
                {
                    // Draw vertical bars
                    p1 = new Point(delta + i * newmesurelen, delta);
                    p2 = new Point(delta + i * newmesurelen, Y - delta);
                    g.DrawLine(selPen, p1, p2);
                    
                    // Fill current mesure with red
                    if (i == currentmesure)
                    {                                                
                        int xx = w + i * newmesurelen;                        
                        Point p3 = new Point(xx, w);
                        RectangleF rect = new RectangleF(p3.X , p3.Y , newmesurelen - w, Y - 2*w);
                        g.FillRectangle(new SolidBrush(Color.Red), rect);
                    }

                    // Numéro de mesure ( à droite de la barre verticale)
                    RectangleF rectangle = new RectangleF(p2.X + 7, p2.Y - 6, delta, delta);
                    Text = (i + 1).ToString();
                    
                    g.DrawString(Text, this.Font, new SolidBrush(_textcolor), rectangle, formatter);

                }
                
            }
        }


        /// <summary>
        /// Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clip =
                 new Rectangle((int)(offsetx),
                 (int)(e.ClipRectangle.Y),
                 (int)(e.ClipRectangle.Width),
                 (int)(e.ClipRectangle.Height));

            Graphics g = e.Graphics;

            g.TranslateTransform(-clip.X, 0);
            DrawGrid(g, clip);
            g.TranslateTransform(clip.X, 0);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (newmesurelen > 0)
            {
                currentmesure = (e.X + offsetx) / newmesurelen;
                panel1.Invalidate();

                // Raise Event
                if (OntlControlPanelClick != null)
                {
                    OntlControlPanelClick(this, e, w + currentmesure * mesurelen);
                }
            }
        }

    }
}
