#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Sanford.Multimedia.Midi.Score;

namespace Karaboss
{
    public partial class frmTest : Form
    {
       

        float zoom = 1.0f;
        bool zoomMode;
        int mesurelen = 80;
        int nbMesures = 45;

        int pageHeight = 800;

       
        private int initX;
        
        public frmTest()
        {
            InitializeComponent();

            tlControl1.OntlControlPanelClick += new tlControl.OntlControlPanelClickHandler(tlControlClickedEvent);
            
            // Largeur mesure fixe hors zoom
            tlControl1.mesurelen = mesurelen;
            
            initX = 0;
            zoomMode = false;

            this.MouseWheel += new MouseEventHandler(frmTest_MouseWheel);

           // Panneau droit                      
            pnlRight.AutoScroll = true;
            pnlRight.HorizontalScroll.Enabled = true;
            pnlRight.VerticalScroll.Enabled = true;
            
           

            // Panneau gauche
            pnlLeft.AutoScroll = false;
            

            pictureBox1.Top = 0;
            pictureBox1.Left = 0;
            pictureBox1.Height = pageHeight;
            pictureBox1.Width = nbMesures*mesurelen;

            pictureBox2.Top = 18;
            pictureBox2.Left = 0;
            pictureBox2.Height = pageHeight;
            pictureBox2.Width = 100;

            pnlLeft.VerticalScroll.LargeChange = pnlRight.VerticalScroll.LargeChange;
            pnlLeft.VerticalScroll.SmallChange = pnlRight.VerticalScroll.SmallChange;
            pnlLeft.VerticalScroll.Minimum = pnlRight.VerticalScroll.Minimum;
            pnlLeft.VerticalScroll.Maximum = pnlRight.VerticalScroll.Maximum;


           

            tlControl1.Left = 0;
            tlControl1.Width = pictureBox1.Width;
        }


        /// <summary>
        /// Event mouse down on the time line control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="newX"></param>
        private void tlControlClickedEvent(object sender, EventArgs e, int newX)
        {     
            // The new left position of current mesure
            initX = newX;
            pictureBox1.Invalidate();
        }

   
  
        private void frmTest_MouseWheel(object sender, MouseEventArgs e)
        {           
            if (zoomMode == true)
            {
                zoom = tlControl1.zoom;
                zoom += (e.Delta > 0 ? 0.1f : -0.1f);
                this.tlControl1.zoom = zoom;

                lblZoomValue.Text = zoom.ToString();

                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
                tlControl1.Invalidate();
            }
        
        }


        private void drawwarpedtext(PaintEventArgs e)
        {
            //create a path
            GraphicsPath pth = new GraphicsPath();
            string s = "Bob Powell's GDI+ FAQ ";
            FontFamily ff = new FontFamily("Verdana");
            //Add the text strings
            for (int y = 0; y < 5; y++)
            {
                pth.AddString(s, ff, 0, 70, new Point(0, 90 * y), StringFormat.GenericTypographic);
            }
            //Create the warp array
            PointF[] points = new PointF[]{
              new PointF(this.ClientSize.Width/2-this.ClientSize.Width/4,0),
              new PointF(this.ClientSize.Width/2+this.ClientSize.Width/4,0),
              new PointF(0,this.ClientSize.Height),
              new PointF(this.ClientSize.Width,this.ClientSize.Height)
              };
            //Warp the path
            pth.Warp(points, new RectangleF(0, 0, 820, 450));
            //Fill the background
            e.Graphics.FillRectangle(Brushes.Black, this.ClientRectangle);
            //Paint the warped path by filling it
            e.Graphics.FillPath(Brushes.Yellow, pth);
            pth.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            zoom = tlControl1.zoom;
            zoom = zoom + 0.1f;
            this.tlControl1.zoom = zoom;
            lblZoomValue.Text = zoom.ToString();
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            zoom = tlControl1.zoom;
            zoom = zoom - 0.1f;
            this.tlControl1.zoom = zoom;
            lblZoomValue.Text = zoom.ToString();
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();

        }

   
        // Droite
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {                        
            Graphics g = e.Graphics;
            g.ScaleTransform(zoom, zoom);            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Ajust width to zoom
            int W = Convert.ToInt32(nbMesures * mesurelen * zoom);
            if (W > 0)
                pictureBox1.Width = W;

            // Adjust height to zoom
            int H = Convert.ToInt32(pageHeight * zoom);
            if (H > 0)
                pictureBox1.Height = H;

            // Draw a small rectangle
            float epaisseurtrait = 3;
            W = mesurelen - Convert.ToInt32(epaisseurtrait);
            e.Graphics.DrawRectangle(new Pen(Color.Blue, epaisseurtrait), initX, 100, W, 40);           
        }


        private void pnlRight_Scroll(object sender, ScrollEventArgs e)
        {            
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                lblScroll.Text = pnlRight.AutoScrollPosition.X.ToString();
                
                // Scroll droit du controle time line
                int X = pnlRight.AutoScrollPosition.X;
                tlControl1.Location = new Point(X, tlControl1.Location.Y);
                                
            }
            
            pictureBox2.Invalidate();
        }

  
        
        // Gauche
        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {            
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            
            // ZOOM
            // Zoom x et y
            //g.ScaleTransform(zoom, zoom);
            
            // Zoom que y
            g.ScaleTransform(1.0f, zoom);                       
            
            
            // Translation verticale - synchronise le scrolling vertical
            int Y = Convert.ToInt32(pnlRight.AutoScrollPosition.Y / zoom);
            Point pos = new Point(0, Y);                        
            e.Graphics.TranslateTransform(pos.X, pos.Y);            
            
            
            // Dessine un petit tectangle
            e.Graphics.DrawRectangle(new Pen(Color.Blue, 3), 10, 100, 80, 40);

        }

        /// <summary>
        /// Validate or invalidate Zoom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (chkZoom.Checked == true)
            {
                zoomMode = true;
            }
            else
            {
                zoomMode = false;
            }
        }

        private void frmTest_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            tlControl1.Invalidate();
        }

        private void tlControl1_Paint(object sender, PaintEventArgs e)
        {
            int X = pnlRight.AutoScrollPosition.X;
            tlControl1.Location = new Point(X, tlControl1.Location.Y);

        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            tlControl1.Width = pictureBox1.Width;
            pictureBox2.Height = pictureBox1.Height;
        }

      
  

   


        
    }
}
