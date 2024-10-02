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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ChordAnalyser.Properties;
using Sanford.Multimedia.Midi;

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

        private Font _fontChord;
        public Font fontChord
        {
            get { return _fontChord; } 
            set 
            {  _fontChord = value;
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

        // Chords
        public Dictionary<int, (string, string)> Gridchords { get; set; }
        //Lyrics
        public Dictionary<int,string> GridLyrics { get; set; }
        

        private float _cellwidth;
        private float _cellheight;

        private int _columnwidth = 80;
        public int ColumnWidth
        {
            get { return _columnwidth; }
            set { 
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
            set { 
                _columnheight = value;
                _cellheight = _columnheight * zoom;

                this.Height = (int)(_cellheight);
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
            _fontChord = new Font("Arial", 40, FontStyle.Regular, GraphicsUnit.Pixel);
            
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
            Height = (int)_columnheight;

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
              
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int _MeasureSeparatorWidth = 2;
            int _LinesWidth = 2;

            Color blackKeysColor = System.Drawing.ColorTranslator.FromHtml("#FF313131");
            Color TimeLineColor = Color.White;

            Pen mesureSeparatorPen = new Pen(Color.Black, _MeasureSeparatorWidth);
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);
            Rectangle rect;

            Point p1;
            Point p2;

            int W = 0;

            W = clip.Width;

            // ==========================
            // Draw Timeline background color
            // Dessiner en dernier
            // ========================== 
            int x = 0;

            // =====================================================
            // 1ere case noire en plus de celles du morceau
            //======================================================            
            g.DrawRectangle(FillPen, 0, 0, _cellwidth, _cellheight);
            rect = new Rectangle(0, 0, (int)(_cellwidth), (int)(_cellheight));
            g.FillRectangle(new SolidBrush(Color.Black), rect);

            var src = new Bitmap(Resources.silence_white);
            var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);
            g.DrawImage(src, new Rectangle(10, 10, bmp.Width, bmp.Height));


            x += (int)(_cellwidth) + (_LinesWidth - 1);

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
                        g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);
                        rect = new Rectangle(x, 0, (int)(_cellwidth), (int)(_cellheight));
                        g.FillRectangle(new SolidBrush(Color.Gray), rect);

                    }
                    else
                    {
                        // Draw other cells in white
                        //g.DrawRectangle(FillPen, clip.X + x, clip.Y, _TimeLineHeight, _TimeLineHeight);
                        g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);
                    }
                    x += (int)(_cellwidth) + (_LinesWidth - 1);
                }
            }

            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================
            x = (int)(_cellwidth) + (_LinesWidth - 1);
            for (int i = 0; i < NbMeasures; i++)
            {
                p1 = new Point(x, clip.Y);
                p2 = new Point(x, clip.Y + (int)(_cellheight));
                g.DrawLine(mesureSeparatorPen, p1, p2);
                x += sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1));
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
            //SolidBrush ChordBrush = new SolidBrush(Color.Black);
            SolidBrush ChordBrush = new SolidBrush(Color.FromArgb(29,29,29));
            //SolidBrush MeasureBrush = new SolidBrush(Color.Red);
            SolidBrush MeasureBrush = new SolidBrush(Color.FromArgb(238,17,17));
            //SolidBrush LyricBrush = new SolidBrush(Color.Blue);
            //SolidBrush LyricBrush = new SolidBrush(Color.FromArgb(43,87,151));
            SolidBrush LyricBrush = new SolidBrush(Color.FromArgb(45, 137, 239));

            //Font _fontChord = new Font("Arial", 40 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontMeasure = new Font("Arial", 14 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontLyric = new Font("Arial", 14 * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

            int _LinesWidth = 2;
            int x = (int)(_cellwidth) + (_LinesWidth - 1);            

            if (Gridchords != null)
            {
                (string, string) ttx;
                string ChordName;
                string tx;
                int Offset = 4;

                float w;
                float h;

                var src = new Bitmap(Resources.silence_black);
                var bmp = new Bitmap((int)(src.Width * zoom), (int)(src.Height * zoom), PixelFormat.Format32bppPArgb);

                for (int i = 1; i <= Gridchords.Count; i++)
                {
                    // Chord name
                                       
                    ttx = Gridchords[i];
                    
                    ChordName = ttx.Item1;
                    w = MeasureString(_fontChord.FontFamily, ChordName, _fontChord.Size);
                    h = MeasureStringHeight(_fontChord.FontFamily, ChordName, _fontChord.Size);                    

                    // If empty, draw symbol
                    if (ChordName == EmptyChord)
                    {                        
                        g.DrawImage(src, new Rectangle(x + Offset, 10, bmp.Width, bmp.Height));
                    }
                    else
                    {                        
                        g.DrawString(ChordName, _fontChord, ChordBrush, x + (_cellwidth - w)/2, (_cellheight/2 - h)/2);
                    }

                    // Draw measure number
                    tx = i.ToString();                    
                    g.DrawString(tx, fontMeasure, MeasureBrush, x + Offset, (int)(_cellheight) - fontMeasure.Height);

                    // ===============================
                    // Second part of mesure
                    // ==============================
                    //if (sequence1.Numerator % 2 == 0)
                    //{
                    if (ttx.Item1 != ttx.Item2)
                    {
                        ChordName = ttx.Item2;
                        w = MeasureString(_fontChord.FontFamily, ChordName, _fontChord.Size);
                        h = MeasureStringHeight(_fontChord.FontFamily, ChordName, _fontChord.Size);

                    // Move to 2nd part of the measure
                        int z = 0;
                        if (sequence1.Numerator % 2 == 0)
                            z = ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator / 2;
                        else
                            z = ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator / 3;
                            
                        // If empty, draw symbol
                        if (ChordName == EmptyChord)
                        {
                            g.DrawImage(src, new Rectangle(x + Offset + z, 10, bmp.Width, bmp.Height));
                        }
                        else
                        {
                            g.DrawString(ChordName, _fontChord, ChordBrush, z + x + (_cellwidth - w) / 2, (_cellheight / 2 - h) / 2);
                        }
                    }
                    //}


                    // Increment x (go to next measure)
                    x += ((int)(_cellwidth) + (_LinesWidth - 1)) * sequence1.Numerator;

                }

                // ==============================
                // Display Lyrics                
                // ==============================
                int currentbeat;
                string currentlyric = string.Empty;
                int d = ((int)(_cellwidth) + (_LinesWidth - 1));

                if (GridLyrics != null)
                {
                    foreach (var z in GridLyrics)
                    {
                        currentbeat = z.Key;
                        currentlyric = z.Value;
                        w = MeasureString(fontLyric.FontFamily, currentlyric, fontLyric.Size);
                        h = MeasureStringHeight(fontLyric.FontFamily, currentlyric, fontLyric.Size);

                        //x = (currentbeat + 1) * d;
                        x = currentbeat * d;
                        g.DrawString(currentlyric, fontLyric, LyricBrush, x + (_cellwidth - w)/2, _cellheight/2 + h);
                    }
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


        #endregion Mouse


        #region paint

        private void pnlCanvas_Paint(object sender, PaintEventArgs e)
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
