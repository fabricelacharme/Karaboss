#region License

/* Copyright (c) 2024 Fabrice Lacharme
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
using ChordAnalyser.Properties;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ChordAnalyser.UI
{

    #region delegate    
    public delegate void MapOffsetChangedEventHandler(object sender, int value);
    public delegate void MapWidthChangedEventHandler(object sender, int value);
    public delegate void MapHeightChangedEventHandler(object sender, int value);

    #endregion delegate


    public partial class ChordsMapControl : Control
    {

        #region events
        public event MapOffsetChangedEventHandler OffsetChanged;
        public event MapWidthChangedEventHandler WidthChanged;
        public event MapHeightChangedEventHandler HeightChanged;

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
        private Font m_font;
        private StringFormat sf;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;
        private int NbLines;        // Number of lines of the control        

        private int _LinesWidth = 2;
        private int _MeasureSeparatorWidth = 2;

        private int _currentpos = 0;
        private int _currentmeasure = -1;
        private int _currentTimeInMeasure = -1;

        private string NoChord = "<Chord not found>";
        private string EmptyChord = "<Empty>";

        public const int PageWidth = 800;    /** The width of each page */
        public const int PageHeight = 1050;  /** The height of each page (when printing) */
        public const int TitleHeight = 14; /** The height for the title on the first page */

        #endregion private


        #region properties

        private int _nbcolumns = 4;
        /// <summary>
        /// Number of columns of the control
        /// </summary>
        public int NbColumns
        {
            get { return _nbcolumns; }
            set { _nbcolumns = value; }
        }


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
                    RedimControl();
                    Redraw();
                }
            }
        }

        public Dictionary<int, (string, string)> Gridchords { get; set; }


        private float _cellwidth;
        private float _cellheight;

        private int _columnwidth = 80;
        public int ColumnWidth
        {
            get { return _columnwidth; }
            set
            {
                _columnwidth = value;
                _cellwidth = _columnwidth * zoom;

                RedimControl();
                
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
                
                RedimControl();

                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                
                pnlCanvas.Invalidate();
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
                _zoom = value;

                _cellwidth = _columnwidth * zoom;
                _cellheight = _columnheight * zoom;

                RedimControl();

                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                if (WidthChanged != null)
                    WidthChanged(this, this.Width);

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
            pnlCanvas.Size = new Size(40, Height);
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
            
            int _beatwidth = ((int)(_cellwidth) + (_LinesWidth - 1));
            int _beatheight = ((int)(_cellheight) + (_LinesWidth - 1));
            int _measurewidth = _beatwidth * sequence1.Numerator;

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
                g.DrawRectangle(FillPen, x, y, _cellwidth, _cellheight);
                rect = new Rectangle(x, 0, (int)(_cellwidth), (int)(_cellheight));
                g.FillRectangle(new SolidBrush(Color.Gray), rect);
                x += _beatwidth; //(int)(_cellwidth) + (_LinesWidth - 1);
            }

            // =====================================================
            // 1ere case noire en plus de celles du morceau
            //======================================================            
            g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);
            rect = new Rectangle(x, 0, (int)(_cellwidth), (int)(_cellheight));
            g.FillRectangle(new SolidBrush(Color.Black), rect);

            var src = new Bitmap(Resources.silence_white);
            var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);
            g.DrawImage(src, new Rectangle(x + 10, 10, bmp.Width, bmp.Height));



            // init variables
            compteurmesure = 0;
            x = _measurewidth; //((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator;

            // ********************
            // Begin at 2nd place
            // ********************
            for (int i = 1; i <= NbMeasures; i++)
            {
                compteurmesure++;
                if (compteurmesure > (_nbcolumns - 1))   // 4 measures per line
                {
                    y += _beatheight; //(int)_cellheight + 1;
                    x = 0;
                    compteurmesure = 0;
                }
                
                
                // Dessine autant de cases que le numerateur
                for (int j = 0; j < sequence1.Numerator; j++)
                {
                    // Draw played cell in gray
                    if (i == _currentmeasure && j == _currentTimeInMeasure - 1 && _currentpos > 0)
                    {
                        g.DrawRectangle(FillPen, x, y, _cellwidth, _cellheight);
                        rect = new Rectangle(x, y, (int)(_cellwidth), (int)(_cellheight));
                        g.FillRectangle(new SolidBrush(Color.Gray), rect);

                    }
                    else
                    {
                        // Draw other celles in white                        
                        g.DrawRectangle(FillPen, x, y, _cellwidth, _cellheight);
                    }
                    x += _beatwidth; //(int)(_cellwidth) + (_LinesWidth - 1);
                }
            }


            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================                        
            x = _measurewidth; //sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1));
            y = 0;
            compteurmesure = -1;

            for (int i = 0; i <= NbMeasures + 1; i++)
            {
                compteurmesure++;
                if (compteurmesure > _nbcolumns - 1)
                {
                    y += _beatheight; //(int)_cellheight + 1;
                    x = _measurewidth; //sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1));
                    compteurmesure = 0;
                }

                if (i % _nbcolumns != 0)
                {
                    p1 = new Point(x, y);
                    p2 = new Point(x, y + (int)(_cellheight));
                    g.DrawLine(mesureSeparatorPen, p1, p2);
                    x += _measurewidth; //sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1));
                }
            }
            
            //maxStaffHeight = ((int)_cellsize + 1) * NbLines;            
            //maxStaffWidth = (sequence1.Numerator * ((int)(_cellsize) + (_LinesWidth - 1))) * _nbcolumns;

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
            //int max = 3;
            int compteurmesure = 0;

            SolidBrush ChordBrush = new SolidBrush(Color.Black);
            SolidBrush MeasureBrush = new SolidBrush(Color.Red);

            Font fontChord = new Font("Arial", 20 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontMeasure = new Font("Arial", 12 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

            //int _LinesWidth = 2;            
            
            // Start after the 1st false measure
            int x = ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator;
            int y_chord = ((int)(_cellheight) / 2) - (fontMeasure.Height / 2);
            int y_symbol = 10;
            int y_measurenumber = (int)(_cellheight) - fontMeasure.Height;

            Point p1;

            if (Gridchords != null)
            {
                (string, string) ttx;
                string tx = string.Empty;
                string ChordName = string.Empty;  
                int Offset = 4;
                float w;

                var src = new Bitmap(Resources.silence_black);
                var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);

                for (int i = 1; i <= Gridchords.Count; i++)
                {

                    compteurmesure++;
                    if (compteurmesure > _nbcolumns - 1)   // 4 measures per line
                    {
                        y_chord += (int)_cellheight + 1;
                        y_symbol += (int)_cellheight + 1;
                        y_measurenumber += (int)_cellheight + 1;
                        x = 0;
                        compteurmesure = 0;
                    }

                    // Chord name                    
                    ttx = Gridchords[i];
                    ChordName = ttx.Item1;
                    w = MeasureString(fontChord.FontFamily, ChordName, fontChord.Size);

                    p1 = new Point(x + Offset, y_chord);


                    // If empty, draw symbol
                    if (ChordName == EmptyChord)
                    {
                        g.DrawImage(src, new Rectangle(p1.X, y_symbol, bmp.Width, bmp.Height));

                    }
                    else
                    {
                        // If chord, print chord name
                        g.DrawString(ChordName, fontChord, ChordBrush, x + (_cellwidth - w) / 2, p1.Y);
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
                            ChordName = ttx.Item2;
                            w = MeasureString(fontChord.FontFamily, ChordName, fontChord.Size);

                            int z = ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator / 2;

                            // If empty, draw symbol
                            if (ChordName == EmptyChord)
                            {
                                g.DrawImage(src, new Rectangle(p1.X + z, y_symbol, bmp.Width, bmp.Height));
                            }
                            else
                            {
                                g.DrawString(ChordName, fontChord, ChordBrush, z + x + (_cellwidth - w) / 2, y_chord);
                            }
                        }
                    }


                    // Increment x (go to next measure)
                    x += ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator;

                }
            }
        }

        #endregion drawnotes


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
            OnMouseDown(e);
        }

        #endregion mouse


        #region paint
        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clip =
                 new Rectangle(
                 (int)(e.ClipRectangle.X),
                 (int)(_offsety),
                 (int)(e.ClipRectangle.Width),
                 (int)(e.ClipRectangle.Height));

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.TranslateTransform(0, -clip.Y);

            if (sequence1 != null)
            {
                DrawGrid(g, clip);

                DrawNotes(g, clip);

                g.TranslateTransform(0, clip.Y);

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

                NbLines = (int)(Math.Ceiling((double)(NbMeasures + 1) / _nbcolumns));
            }
        }


        private void RedimControl()
        {
            if (sequence1 != null)
            {
                NbLines = (int)(Math.Ceiling((double)(NbMeasures + 1) / _nbcolumns));
                Height = ((int)_cellheight + 1) * NbLines;             
                Width = (sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1))) * _nbcolumns;
            }
        }

        #endregion Midi


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


        #region print pdf

        public void DoPrint(Graphics g, string fileName, int pagenumber, int numpages)
        {
            int leftmargin = 20;
            int topmargin = 20;
            int rightmargin = 20;
            int bottommargin = 20;

            float scale = (g.VisibleClipBounds.Width - leftmargin - rightmargin) / PageWidth;
            g.PageScale = scale;

            int viewPageHeight = (int)((g.VisibleClipBounds.Height - topmargin - bottommargin) / scale);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, 0, 0,
                            g.VisibleClipBounds.Width,
                            g.VisibleClipBounds.Height);

            Rectangle clip = new Rectangle(0, 0, PageWidth, PageHeight);

            int ypos = TitleHeight;
            int pagenum = 1;
            

            // Print the lines until the height reaches viewPageHeight 
            // Draw title
            if (pagenum == 1)
            {
                DrawTitle(g, fileName);
                ypos = TitleHeight;
            }
            else
            {
                ypos = 0;
            }



        }

        /** Write the MIDI filename at the top of the page */
        private void DrawTitle(Graphics g, string fileName)
        {
            int leftmargin = 20;
            int topmargin = 20;

            string title = Path.GetFileName(fileName);

            //string title = Path.GetFileName(filename);
            title = title.Replace(".mid", "").Replace("_", " ").Replace(".kar", "");
            Font font = new Font("Arial", 10, FontStyle.Bold);

            g.TranslateTransform(leftmargin, topmargin);
            g.DrawString(title, font, Brushes.Black, 0, 0);
            g.TranslateTransform(-leftmargin, -topmargin);
            font.Dispose();
        }
        #endregion print pdf
    }
}
