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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
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
        private string NoChord = "<Chord not found>";

        private float _cellwidth;
        private float _cellheight;
        private int _LinesWidth = 2;

        private Font m_font;
        private StringFormat sf = new StringFormat();

        private int _fontSize = 22;
        private int fontPadding = 0;//10;
        private int _fontpadding;

        private string _currentChordName = string.Empty;
        private int _currentbeat;

        public enum DiplayModes
        {
            Guitar,
            Piano
        }

        public DiplayModes DisplayMode { get; set; }


        private bool _bPlaying = false;

        #endregion private


        #region properties        

        // Chords
        // 2 chords by measure : Chord 1, chord 2
        public Dictionary<int, (string, string)> Gridchords { get; set; }

        // New search (by beat)        
        private int _chordsCount = 0;
        private Dictionary<int, (int, string)> _filteredgridbeatchords;

        public Dictionary<int, string> GridBeatChords { get; set; }
        
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
                _fontpadding = (int)(fontPadding * zoom);
                _fontChord = new Font(_fontChord.FontFamily, _fontSize * zoom, FontStyle.Regular, GraphicsUnit.Pixel);
                
                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                // No need to manage width: controls position on frmChords depends only on its height

                pnlCanvas.Invalidate();
            }
        }

        #endregion properties


        public ChordRenderer()
        {
            _fontChord = new Font("Arial", _fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontpadding = fontPadding;

            DisplayMode = DiplayModes.Guitar;

            // Draw pnlCanvas
            DrawCanvas();

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);

        }


        public void TransferByMeasureToByBeat(int numerator, int measures)
        {
            GridBeatChords = new Dictionary<int, string>();

            int beats = numerator * measures;
            for (int i = 1; i <= beats; i++)
            {
                GridBeatChords.Add(i, "");
            }

            int beat;
            
            for (int measure = 1; measure <= Gridchords.Count; measure++ )
            {
                beat = 1 + (measure - 1) * numerator; 
                string item1 = Gridchords[measure].Item1;
                GridBeatChords[beat] = item1;

                beat = 1 + (measure - 1) * numerator + (numerator/2);
                string item2 = Gridchords[measure].Item2;
                GridBeatChords[beat] = item2;

            }
        }
        
        /// <summary>
        /// Create a new dictionnary with only real chords (eliminate empty & no chords)
        /// </summary>
        public void FilterChords()
        {
            try
            {                
                _chordsCount = 0;
                _filteredgridbeatchords = new Dictionary<int, (int, string)>();
                string currentchord = "-1";

                int x = _LinesWidth - 1;

                for (int i = 1; i <= GridBeatChords.Count; i++)
                {
                    string t = GridBeatChords[i];

                    if (t != "" && t != EmptyChord && t != NoChord && t != currentchord)
                    {
                        currentchord = t;
                        _chordsCount++;
                        _filteredgridbeatchords.Add(i, (_chordsCount, t));

                        // Increase x of 1 cell
                        x += (int)(_cellwidth) + _LinesWidth;
                    }
                }
                // Set Width of control
                Width = x;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }      


        /// <summary>
        /// Offset control
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="pos"></param>
        /// <param name="measure"></param>
        /// <param name="timeinmeasure"></param>
        public void OffsetControl(int numerator, int pos, int measure, int timeinmeasure)
        {
            // Dictionnary _filteredgridbeatchords |key = real beat| index of the chord in the grid | chord name  
            // knowing the beat, we can retrieve the position of the chord
            
            _bPlaying = true;
            int beat = (measure - 1) * numerator + timeinmeasure;
            _currentbeat = beat;

            if (beat == 1)
            {
                _currentChordName = "<>";                
            }

            if (!GridBeatChords.ContainsKey(beat))
                return;

            string ChordName = GridBeatChords[beat];
            // If the chord chord played is different than the previous one, we have to offset the control
            if (ChordName != NoChord && ChordName != "" && ChordName != EmptyChord && ChordName != _currentChordName)
            {
                _currentChordName = ChordName;                

                if (_filteredgridbeatchords.ContainsKey(beat))
                {

                    // The chord played is different than the previous one.

                    (int, string) toto = _filteredgridbeatchords[beat];
                    int index = toto.Item1; // index of the chord //string y = toto.Item2; // chord name

                    int LargeurCellule = (int)(ColumnWidth * zoom) + _LinesWidth;
                    
                    // Display 2 chords on the left : the last chord played, the chord being played => index - 2
                    int offset = (index - 2) * LargeurCellule;
                    int remainingwidth = Width - offset;

                    if (offset <= 0)
                    {
                        // At start, do not offset until we have passed 2 cells
                        pnlCanvas.Invalidate();
                    }
                    else
                    {
                        if (remainingwidth >= Parent.Width - LargeurCellule)
                        {                            
                            // if the remaining display width of the control is greater than that of the parent control, then you can shift
                            this.OffsetX = offset;
                        }
                        else
                        {
                            // if the remaining display width of the control is less than that of the parent control, then we no longer shift
                            int z = (Width - Parent.Width) / LargeurCellule;                            
                            this.OffsetX = (z + 1) * LargeurCellule;
                            pnlCanvas.Invalidate();
                        }
                    }                    
                }
            }
        }

        public void AfterStopped()
        {
            OffsetX = 0;
            _bPlaying = false;
            Redraw();
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

        #endregion Draw Canvas


        #region draw chords   

        /// <summary>
        /// Draw the all chords of song
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawChordsByBeat(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;
            
            string ChordName;
            string currentChordName = string.Empty;
           
            int x = _LinesWidth - 1;
            bool bChordPlayed = false;

            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                ChordName = GridBeatChords[i];

                // Draw chords if they are different from previous               
                if (ChordName != "" && ChordName != EmptyChord && ChordName != currentChordName && ChordName != NoChord)
                {
                                                                
                    bChordPlayed = (i == _currentbeat && _bPlaying);
                                        
                    currentChordName = ChordName;

                    // Draw Chord bitmap
                    switch (DisplayMode)
                    {
                        case DiplayModes.Guitar:
                            DrawChord(g, ChordName, x, bChordPlayed);
                            break;
                        case DiplayModes.Piano:
                            DrawChord(g, "p" + ChordName, x, bChordPlayed);                            
                            break;

                    }
                    
                    // Draw chord name
                    DrawChordName(g, ChordName, x, bChordPlayed);

                    // Increase x of 1 cell
                    x += (int)(_cellwidth) + _LinesWidth;                

                }
            }            
        }


        /// <summary>
        /// Draw a chord
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ChordName"></param>
        /// <param name="pos"></param>
        private void DrawChord(Graphics g, string ChordName, int pos, bool bChordPlayed)
        {
            float enlarge = 1;
            int w;
            int h;
            int middlex;
            int middley;

            // Draw Chord bitmap
            if (ChordName != "")
            {
                try
                {
                    ResourceManager rm = Resources.ResourceManager;
                    Bitmap chordImage = (Bitmap)rm.GetObject(ChordName);

                    // chord played is bigger
                    if (bChordPlayed)
                        enlarge = 1.24f;

                    if (chordImage != null)
                    {
                        w = (int)(chordImage.Width * zoom * enlarge);
                        h = (int)(chordImage.Height * zoom * enlarge);                        
                        Size newSize = new Size(w, h);

                        Bitmap bmp = new Bitmap(chordImage, newSize);
                        
                        middlex = pos + (int)(_cellwidth - w) / 2;
                        middley = (int)((_cellheight - h) / 2);                        

                        Point p = new Point(middlex, middley);

                        g.DrawImage(bmp, p);
                        chordImage.Dispose();
                        bmp.Dispose();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Write the name of the chord
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ChordName"></param>
        /// <param name="pos"></param>
        private void DrawChordName(Graphics g, string ChordName, int pos, bool bChordPlayed)
        {
            float w;
            float h;
            int y;
            SolidBrush ChordBrush = new SolidBrush(Color.FromArgb(29, 29, 29));
            
            if (bChordPlayed)
                ChordBrush = new SolidBrush(Color.Red);           
            
            w = MeasureString(_fontChord.FontFamily, ChordName, _fontChord.Size);
            h = MeasureStringHeight(_fontChord.FontFamily, ChordName, _fontChord.Size);
            y = (int)(_cellheight + _fontpadding);

            g.DrawString(ChordName, _fontChord, ChordBrush, pos + (_cellwidth - w) / 2, y);
        }

        #endregion draw chords


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

                DrawChordsByBeat(g, clip);

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
