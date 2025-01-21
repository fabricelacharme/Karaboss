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
        private StringFormat sf = new StringFormat();

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;        
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

        private string ChordNotFound = "<Chord not found>";
        private string EmptyChord = "<Empty>";

        public int PageWidth = 800;    /** The width of each page */
        public int PageHeight = 1050;  /** The height of each page (when printing) */
        public const int TitleHeight = 14; /** The height for the title on the first page */        

        private string _filename;        
        private const int topmargin = 20;

        #endregion private


        #region properties

        
        #region Fonts

        private Font _fontChord;
        public Font FontChord
        {
            get { return _fontChord; }
            set
            {
                _fontChord = value;
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

        #endregion Fonts


        private bool _displaylyrics = true;
        public bool Displaylyrics
        {
            get { return _displaylyrics; }
            set {
                if (value != _displaylyrics)
                {
                    _displaylyrics = value;
                    Refresh();
                }
            }
        }

        private const int _leftmargin = 20;
        public int LeftMargin
        {
            get { return _leftmargin; }
        }

        private int _nbcolumns = 4;
        /// <summary>
        /// Number of columns of the control
        /// </summary>
        public int NbColumns
        {
            get { return _nbcolumns; }
            set { 
                _nbcolumns = value;
                RedimControl();
                Refresh();
            }
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

        //public Dictionary<int, (string, string)> Gridchords { get; set; }

        private Dictionary<int, (string, int)> _gridbeatchords;
        public Dictionary<int, (string, int)> GridBeatChords {
            get { return _gridbeatchords; }
            set { 
                _gridbeatchords = value;
                Refresh();
            }
        }

        //Lyrics
        public Dictionary<int, string> GridLyrics { get; set; }
        

        private float _cellwidth;
        private int _columnwidth = 100;
        public int ColumnWidth      
        {
            //get { return _columnwidth; }
            get { return (int)_cellwidth; }
            set
            {
                _columnwidth = value;
                _cellwidth = _columnwidth * _zoom;

                RedimControl();
                
                if (WidthChanged != null)
                    WidthChanged(this, this.Width);
                
                pnlCanvas.Invalidate();
            }
        }

        private float _headerheight;
        private int _cheaderheight = 100;   // devient la variable
        public int HeaderHeight
        {
            get { return (int)_headerheight; }
            set 
            { 
                _cheaderheight = value;
                _headerheight = _cheaderheight * _zoom;

                RedimControl();

                pnlCanvas?.Invalidate();
            }
        }

        private float _cellheight;
        private int _columnheight = 80;
        public int ColumnHeight
        {
            //get { return _columnheight; }
            get { return (int)_cellheight; }
            set
            {
                _columnheight = value;
                _cellheight = _columnheight * _zoom;
                
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
        public float Zoom
        {
            get
            { return _zoom; }
            set
            {
                _zoom = value;

                _cellwidth = _columnwidth * _zoom;
                _cellheight = _columnheight * _zoom;
                _headerheight = _cheaderheight * _zoom;

                _fontChord = new Font(_fontChord.FontFamily, 20 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);
                _fontMeasure = new Font(_fontMeasure.FontFamily, 12 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);
                _fontLyric = new Font(_fontLyric.FontFamily, 16 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);

                RedimControl();

                if (HeightChanged != null)
                    HeightChanged(this, this.Height);
                if (WidthChanged != null)
                    WidthChanged(this, this.Width);

                pnlCanvas.Invalidate();
            }
        }

        #endregion properties


        public ChordsMapControl(string FileName)
        {
            _filename = FileName;

            _fontChord = new Font("Arial", 24, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontMeasure = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontLyric = new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Pixel);

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
        /// print Title, tempo, TimeSignature in Header
        /// </summary>
        private void PrintFileInfos(Graphics g)
        {
            // All must be in
            // HeaderHeight
            // PageWidth, leftmargin, topmargin

            Font fontTitle = new Font("Arial", 20 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);
            Font fontText = new Font("Arial", 14 * _zoom, FontStyle.Regular, GraphicsUnit.Pixel);

            float w;
            String TimeSignature = sequence1.Numerator + "/" + sequence1.Denominator;
            string Tempo = GetBPM(_tempo).ToString();

            int _beatwidth = ((int)(_cellwidth) + (_LinesWidth - 1));            
            PageWidth = 2*_leftmargin + _nbcolumns * (_beatwidth * sequence1.Numerator);

            string title = Path.GetFileName(_filename);
            w = MeasureString(fontTitle.FontFamily, title, fontTitle.Size);

            title = title.Replace(".mid", "").Replace("_", " ").Replace(".kar", "").Replace("musicxml", "").Replace("xml", "");            

            g.TranslateTransform(PageWidth/2 - w/2, topmargin);
            g.DrawString(title, fontTitle, Brushes.Black, 0, 0);
            g.TranslateTransform(-(PageWidth/2 - w/2), -topmargin);

            int ypos = 50;
            Point p1 = new Point(_leftmargin, ypos);
            g.DrawString("TimeSignature: " + TimeSignature, fontText, new SolidBrush(Color.Black), p1.X, p1.Y);
            g.DrawString("Tempo: " + Tempo, fontText, new SolidBrush(Color.Black), p1.X, p1.Y + 16 * _zoom);

            fontTitle.Dispose();

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
            int x = _leftmargin;
            int y = (int)_headerheight;

            int compteurmesure = -1;

            FillPen = new Pen(Color.Gray, _LinesWidth);
           
            // init variables
            compteurmesure = -1;                    

            // ********************
            // Begin at 2nd place
            // ********************
            for (int i = 1; i <= NbMeasures; i++)
            {
                compteurmesure++;
                if (compteurmesure > (_nbcolumns - 1))   // 4 measures per line
                {
                    y += _beatheight;
                    x = _leftmargin;
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
                    x += _beatwidth; 
                }
            }


            // ====================================================
            // Ligne noire sur la dernière case de chaque mesure
            // ====================================================                        
            x = _leftmargin + _measurewidth;
            y = (int)_headerheight; 
            int nbMeasuresPerLine = 1;

            for (int mes = 1; mes <= NbMeasures; mes++)
            {
                // Draw a black line every _measurewidth;
                p1 = new Point(x, y);
                p2 = new Point(x, y + (int)(_cellheight));
                g.DrawLine(mesureSeparatorPen, p1, p2);
                x += _measurewidth;

                // Increase line counter
                nbMeasuresPerLine++;

                // change line every _nbcolumns columns
                if (nbMeasuresPerLine > _nbcolumns)
                {                    
                    y += _beatheight;
                    x = _leftmargin + _measurewidth;                     
                    nbMeasuresPerLine = 1;                    
                }
            }                        
        }

        #endregion draw canvas


        #region Draw chords 

        /// <summary>
        /// Draw the name of the chords
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawChords(Graphics g, Rectangle clip)
        {
            int compteurmesure = -1; 
            
            SolidBrush ChordBrush = new SolidBrush(Color.Black);
            SolidBrush MeasureBrush = new SolidBrush(Color.Red);
            SolidBrush LyricBrush = new SolidBrush(Color.FromArgb(43, 87, 151)); // (45, 137, 239))
            
            int x = _leftmargin;//0;
            int y_chord = (int)_headerheight + ((int)(_cellheight) / 2) - (_fontMeasure.Height / 2);
            int y_symbol = 10;
           
            int y_measurenumber = (int)_headerheight + _fontMeasure.Height / 3;
            float y_lyric = (int)_headerheight + _cellheight - _fontLyric.Height;

            int m = -1;

            Point p1;

            if (_gridbeatchords != null)
            {                
                string tx = string.Empty;
                string chordName = string.Empty;  
                int Offset = 4;
                float w;
                float h;                
                int d = (int)(_cellwidth) + (_LinesWidth - 1);

                var src = new Bitmap(Resources.silence_black);
                var bmp = new Bitmap((int)(src.Width * _zoom), (int)(src.Height * _zoom), PixelFormat.Format32bppPArgb);

                // Filter chords
                string _currentChordName = "<>";

                for (int i = 1; i <= _gridbeatchords.Count; i++)
                {
                    compteurmesure++;
                    if (compteurmesure > -1 + _nbcolumns * sequence1.Numerator)   // _nbcolumns measures per line
                    {
                        y_chord += (int)_cellheight + 1;
                        y_symbol += (int)_cellheight + 1;
                        y_measurenumber += (int)_cellheight + 1;                        
                        x = _leftmargin;
                        compteurmesure = 0;
                    }

                    // Chord name                                        
                    chordName = _gridbeatchords[i].Item1;

                    w = MeasureString(_fontChord.FontFamily, chordName, _fontChord.Size);

                    p1 = new Point(x + Offset, y_chord);


                    // If empty, draw symbol
                    if (chordName == EmptyChord)
                    {
                        g.DrawImage(src, new Rectangle(p1.X, y_symbol, bmp.Width, bmp.Height));

                    }
                    else if (chordName != "" && chordName != _currentChordName)
                    {
                        _currentChordName = chordName;
                        // If chord, print chord name
                        g.DrawString(chordName, _fontChord, ChordBrush, x + (_cellwidth - w) / 2, p1.Y);
                    }

                    // Draw measure number                    
                    m++;
                    if (m % sequence1.Numerator == 0)
                    {
                        tx = (1 + i/sequence1.Numerator).ToString();
                        p1 = new Point(x + Offset, y_measurenumber);
                        g.DrawString(tx, _fontMeasure, MeasureBrush, p1.X, p1.Y);
                        m = 0;
                    }

                    // Increment x (go to next beat / cell)                    
                    x += d;
                }

                // ==============================
                // Display Lyrics                
                // ==============================
                int currentbeat;
                string currentlyric = string.Empty;
                int _measure;
                int nbBeatsPerMeasure = sequence1.Numerator;
                int line;
                int prevline = 1;                

                if (GridLyrics != null && _displaylyrics)
                {
                    foreach (var z in GridLyrics)
                    {
                        currentbeat = z.Key;
                        currentlyric = z.Value;
                        _measure = (currentbeat - 1) / nbBeatsPerMeasure;
                        
                        line = 1 + _measure/_nbcolumns;                  
                        if (line > prevline)
                        {
                            prevline = line;
                            y_lyric += ((int)_cellheight + 1);
                        }

                        w = MeasureString(_fontLyric.FontFamily, currentlyric, _fontLyric.Size);
                        h = MeasureStringHeight(_fontLyric.FontFamily, currentlyric, _fontLyric.Size);

                        x =  -1 + currentbeat - (line - 1) * (nbBeatsPerMeasure * _nbcolumns);
                        x = _leftmargin + x * d;

                        g.DrawString(currentlyric, _fontLyric, LyricBrush, x + (_cellwidth - w) / 2, y_lyric);
                    }
                }

            }
        }

        #endregion Draw Chords


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
                PrintFileInfos(g);
                
                DrawGrid(g, clip);

                DrawChords(g, clip);

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

        /// <summary>
        /// Calculate BPM
        /// </summary>
        /// <param name="tempo"></param>
        /// <returns></returns>
        private int GetBPM(int tempo)
        {
            // see http://midi.teragonaudio.com/tech/midifile/ppqn.htm
            const float kOneMinuteInMicroseconds = 60000000;
            float BPM = kOneMinuteInMicroseconds / (float)tempo;

            return (int)BPM;
        }

        private void RedimControl()
        {
            if (sequence1 != null)
            {
                NbLines = (int)(Math.Ceiling((double)(NbMeasures + 1) / _nbcolumns));
                Height = (int)_headerheight + ((int)_cellheight + 1) * NbLines;             
                Width = 2*_leftmargin + (sequence1.Numerator * ((int)(_cellwidth) + (_LinesWidth - 1))) * _nbcolumns;
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


        #region print pdf

        public void DoPrint(Graphics g, string fileName, int pagenumber, int numpages)
        {
            //int leftmargin = 20;
            //int topmargin = 20;
            int rightmargin = 20;
            int bottommargin = 20;

            float scale = (g.VisibleClipBounds.Width - _leftmargin - rightmargin) / PageWidth;
            g.PageScale = scale;

            int viewPageHeight = (int)((g.VisibleClipBounds.Height - topmargin - bottommargin) / scale);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillRectangle(Brushes.White, 0, 0,
                            g.VisibleClipBounds.Width,
                            g.VisibleClipBounds.Height);

            Rectangle clip = new Rectangle(0, 0, PageWidth, PageHeight);

            //int ypos = TitleHeight;
            int pagenum = 1;
            

            // Print the lines until the height reaches viewPageHeight 
            // Draw title
            if (pagenum == 1)
            {
                DrawTitle(g, fileName);
                //ypos = TitleHeight;
            }
            else
            {
                //ypos = 0;
            }



        }

        /** Write the MIDI filename at the top of the page */
        private void DrawTitle(Graphics g, string fileName)
        {
            
            string title = Path.GetFileName(fileName);

            //string title = Path.GetFileName(filename);
            title = title.Replace(".mid", "").Replace("_", " ").Replace(".kar", "");
            Font font = new Font("Arial", 10, FontStyle.Bold);

            g.TranslateTransform(_leftmargin, topmargin);
            g.DrawString(title, font, Brushes.Black, 0, 0);
            g.TranslateTransform(-_leftmargin, -topmargin);
            font.Dispose();
        }
        #endregion print pdf
    }
}
