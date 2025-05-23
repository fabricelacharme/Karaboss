﻿#region License

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

    /*
     *  This control display chords horizontally on a line of cells
     * 
     * 
     * 
     * 
     * */


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
        public Font FontChord
        {
            get { return _fontChord; } 
            set 
            {  _fontChord = value;
                pnlCanvas.Invalidate();
            }
        }

        private Font _fontMeasure;
        public Font FontMeasure
        {
            get { return _fontMeasure; }
            set
            {
                _fontMeasure = value;
                pnlCanvas.Invalidate();
            }
        }
        
        private Font _fontLyric;
        public Font FontLyric
        {
            get { return _fontLyric; }
            set
            {
                _fontLyric = value;
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
        // 2 chords by measure : Chord 1, chord 2
        //public Dictionary<int, (string, string)> Gridchords { get; set; }        
        public Dictionary<int, (string, int)> GridBeatChords { get; set; }

        //Lyrics
        public Dictionary<int,string> GridLyrics { get; set; }

        private string EmptyChord = "<Empty>";
        private readonly string ChordNotFound = "<Chord not found>";

        private float _cellwidth;
        private float _cellheight;
        private readonly int _LinesWidth = 2;


        private int _columnwidth = 80;
        public int ColumnWidth
        {
            get { return _columnwidth; }
            set { 
                _columnwidth = value;
                _cellwidth = _columnwidth * Zoom;
                WidthChanged?.Invoke(this, this.Width);
                pnlCanvas.Invalidate();

            }
        }

        private int _columnheight = 80;
        public int ColumnHeight
        {
            get { return _columnheight; }
            set { 
                _columnheight = value;
                _cellheight = _columnheight * Zoom;

                this.Height = (int)(_cellheight);
                HeightChanged?.Invoke(this, this.Height);
                pnlCanvas.Invalidate();
            }
        }
     

        /// <summary>
        /// zoom
        /// </summary>
        private float _zoom = 1.0f;    // zoom for horizontal
        public float Zoom
        {
            get
            { return _zoom; }
            
            set
            {
                if (value <= 0.0f)
                    return;

                _zoom = value;                

                _cellwidth = _columnwidth * _zoom;
                _cellheight = _columnheight * _zoom;

                _fontChord = new Font(_fontChord.FontFamily, 40 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);
                _fontMeasure = new Font(_fontMeasure.FontFamily, 14 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);
                _fontLyric = new Font(_fontLyric.FontFamily, 14 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);

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
        private StringFormat sf = new StringFormat();

        // Midifile characteristics
        //private double _duration = 0;  // en secondes
        private int _totalTicks = 0;        
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int _nbMeasures;
        private int _nbBeats;

        private int _currentpos = 0;
        private int _currentmeasure = -1;
        private int _currentTimeInMeasure = -1;
       
        #endregion private
        

        public ChordsControl()
        {
            _fontChord = new Font("Arial", 40, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontMeasure = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontLyric = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Pixel);

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
            pnlCanvas.MouseDown += new MouseEventHandler(pnlCanvas_MouseDown);
            pnlCanvas.MouseUp += new MouseEventHandler(pnlCanvas_MouseUp);
            pnlCanvas.MouseMove += new MouseEventHandler(pnlCanvas_MouseMove);
            pnlCanvas.MouseLeave += new EventHandler(pnlCanvas_MouseLeave);

            this.Controls.Add(pnlCanvas);
        }
        
        
        /// <summary>
        /// Draw a line of cells
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawGrid(Graphics g, Rectangle clip)
        {
            int _MeasureSeparatorWidth = 2;
            //int _LinesWidth = 2;

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
            var bmp = new Bitmap((int)(src.Width * Zoom), (int)(src.Height * Zoom), PixelFormat.Format32bppPArgb);
            g.DrawImage(src, new Rectangle(10, 10, bmp.Width, bmp.Height));


            x += (int)(_cellwidth) + (_LinesWidth - 1);

            // ======================================================
            // Draw measures
            // ======================================================
            // 4 temps = 4 carrés gris
            // Chaque mesure, une ligne verticale gris foncé

            FillPen = new Pen(Color.Gray, _LinesWidth);

            for (int i = 0; i < _nbMeasures; i++)
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
                        g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);
                    }
                    x += (int)(_cellwidth) + (_LinesWidth - 1);
                }
            }

            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================
            x = (int)(_cellwidth) + (_LinesWidth - 1);
            for (int i = 0; i < _nbMeasures; i++)
            {
                p1 = new Point(x, clip.Y);
                p2 = new Point(x, clip.Y + (int)(_cellheight));
                g.DrawLine(mesureSeparatorPen, p1, p2);
                x += sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1));
            }

            maxStaffWidth = x;
        }


        #endregion Draw Canvas    


        #region drawnchords   
      
        private void DrawChords(Graphics g, Rectangle clip)
        {
            SolidBrush ChordBrush = new SolidBrush(Color.FromArgb(29, 29, 29));
            SolidBrush MeasureBrush = new SolidBrush(Color.FromArgb(238, 17, 17));
            SolidBrush LyricBrush = new SolidBrush(Color.FromArgb(45, 137, 239));       

            int x = (int)(_cellwidth) + (_LinesWidth - 1);

            if (GridBeatChords != null)
            {                
                string chordName;                
                int Offset = 4;
                string tx;
                float w;
                float h;

                int d = (int)(_cellwidth) + (_LinesWidth - 1);
                int m = -1;
                var src = new Bitmap(Resources.silence_black);
                var bmp = new Bitmap((int)(src.Width * Zoom), (int)(src.Height * Zoom), PixelFormat.Format32bppPArgb);

                // Filter chords
                string _currentChordName = "<>";

                for (int i = 1; i <= GridBeatChords.Count; i++)
                {
                    // Chord name                                                           
                    chordName = GridBeatChords[i].Item1;
                    
                    w = MeasureString(_fontChord.FontFamily, chordName, _fontChord.Size);
                    h = MeasureStringHeight(_fontChord.FontFamily, chordName, _fontChord.Size);

                    // If empty, draw symbol
                    if (chordName == EmptyChord)
                    {
                        g.DrawImage(src, new Rectangle(x + Offset, 10, bmp.Width, bmp.Height));
                    }
                    else if (chordName != "" && chordName != _currentChordName)
                    {
                        // Draw a chord only if different than previous one
                        _currentChordName = chordName;
                        g.DrawString(chordName, _fontChord, ChordBrush, x + (_cellwidth - w) / 2, (_cellheight / 2 - h) / 2);
                    }

                    // Draw measure number
                    m++;
                    if (m % sequence1.Numerator == 0)
                    {
                        tx = (1 + i / sequence1.Numerator).ToString();
                        g.DrawString(tx, _fontMeasure, MeasureBrush, x + Offset, (int)(_cellheight) - _fontMeasure.Height);
                        m = 0;
                    }

                    // Increment x (go to the next beat / cell
                    x += d;
                }

                // ==============================
                // Display Lyrics                
                // ==============================
                int currentbeat;
                string currentlyric = string.Empty;                

                if (GridLyrics != null)
                {
                    foreach (var z in GridLyrics)
                    {
                        currentbeat = z.Key;
                        currentlyric = z.Value;
                        w = MeasureString(_fontLyric.FontFamily, currentlyric, _fontLyric.Size);
                        h = MeasureStringHeight(_fontLyric.FontFamily, currentlyric, _fontLyric.Size);

                        x = currentbeat * d;
                        g.DrawString(currentlyric, _fontLyric, LyricBrush, x + (_cellwidth - w) / 2, _cellheight / 2 + h);
                    }
                }
            }
        }

        #endregion drawchords


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

        private void ChordControl_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();            
        }

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

                DrawChords(g, clip);

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
            //_duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                _nbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer

                int nbBeatsPerMeasure = sequence1.Numerator;
                int beatDuration = _measurelen / nbBeatsPerMeasure;
                _nbBeats = (int)Math.Ceiling(_totalTicks / (float)beatDuration);
            }
        }

        #endregion Midi


        #region mesure strings
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

        #endregion mesure strings
    }
}
