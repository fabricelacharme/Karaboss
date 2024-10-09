using ChordAnalyser.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
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
        private string NoChord = "<Chord not found>";

        private float _cellwidth;
        private float _cellheight;

        private Font m_font;
        private StringFormat sf;

        private int _fontSize = 22;
        private int fontPadding = 10;
        private int _fontpadding;

        private string _currentChordName = string.Empty;
        private bool _bFirstPlay = false;

        private float _bitmapwidth = 200;

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
                
                _fontpadding = (int)(fontPadding * zoom);

                _fontChord = new Font(_fontChord.FontFamily, _fontSize * zoom, FontStyle.Regular, GraphicsUnit.Pixel);

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
            _fontChord = new Font("Arial", _fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            _fontpadding = 10;

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
                beat = 1 + (measure - 1) * numerator; // + (timeinmeasure - 1);
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


                for (int i = 1; i <= GridBeatChords.Count; i++)
                {
                    string t = GridBeatChords[i];

                    if (t != "" && t != EmptyChord && t != NoChord && t != currentchord)
                    {
                        currentchord = t;
                        _chordsCount++;
                        _filteredgridbeatchords.Add(i, (_chordsCount, t));
                    }


                }
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
            // Dictionnary _filteredgridbeatchords |key = real beat| index of chord in the grid | chord name  
            // knowing the beat, we can retrieve the position of the chord
            
            int beat = (measure - 1) * numerator + timeinmeasure;
            if (beat == 1)
            {
                _currentChordName = "";
                _bFirstPlay = true;
            }

            if (!GridBeatChords.ContainsKey(beat))
                return;

            string ChordName = GridBeatChords[beat];
            if (ChordName != NoChord && ChordName != "" && ChordName != EmptyChord && ChordName != _currentChordName)
            {
                _currentChordName = ChordName;                

                if (_filteredgridbeatchords.ContainsKey(beat))
                {

                    (int, string) toto = _filteredgridbeatchords[beat];
                    int x = toto.Item1; // index of the chord
                                        //string y = toto.Item2; // chord name

                    int LargeurCellule = (int)(ColumnWidth * zoom) + 2;

                    // Do not offset at first chord
                    if (_bFirstPlay)
                    {
                        _bFirstPlay = false;                        
                    }
                    else
                    {
                        // Offset the control with the value of the position of the filtered chord in the filtered dictionnary
                        this.OffsetX = (x - 1) * LargeurCellule;                // Changing this property will lauch a redraw                                                                                
                    }
                }
            }
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

        #region delete me
        /*

        /// <summary>
        /// Draw a cell by chord
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawGrid2(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;

            int x = 0;
            int _LinesWidth = 2;
            Color TimeLineColor = Color.White;
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);
            
            // 2 cells by GridChord item (2 chords by measure)
            for (int i = 0; i < 2 * Gridchords.Count; i++)
            {
                g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);

                // Increase length of control
                x += (int)(_cellwidth) + (_LinesWidth - 1);
            }

            maxStaffWidth = x;
        }

        private void DrawGrid(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;

            int x = 0;
            int _LinesWidth = 2;
            Color TimeLineColor = Color.White;
            Pen FillPen = new Pen(TimeLineColor, _LinesWidth);

            // 1 cells by Beat 
            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                g.DrawRectangle(FillPen, x, 0, _cellwidth, _cellheight);

                // Increase length of control
                x += (int)(_cellwidth) + (_LinesWidth - 1);
            }

            maxStaffWidth = x;
        }
        */
        #endregion delete me

        #endregion Draw Canvas


        #region draw chords   

        /// <summary>
        /// Draw the all chords of song
        /// </summary>
        /// <param name="g"></param>
        /// <param name="clip"></param>
        private void DrawChordsByHalfMeasure(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;
            
            (string, string) ttx;
            string ChordName;
            string currentChordName = string.Empty;
            int _LinesWidth = 2;
            int x = (_LinesWidth - 1);

            for (int i = 1; i <= Gridchords.Count; i++)
            {
                ttx = Gridchords[i];
                
                // First chord
                ChordName = ttx.Item1;
                if (ChordName != "" && ChordName != EmptyChord && ChordName != currentChordName)
                {
                    currentChordName = ChordName;

                    // Draw Chord bitmap
                    DrawChord(g, ChordName, x);

                    // Draw chord name
                    DrawChordName(g, ChordName, x);

                }
                // Increase x of 1 cell
                x += (int)(_cellwidth) + _LinesWidth;

                // Second chord
                ChordName = ttx.Item2;
                if (ChordName != "" && ChordName != EmptyChord && ChordName != currentChordName)
                {
                    currentChordName = ChordName;

                    // Draw Chord bitmap
                    DrawChord(g, ChordName, x);

                    // Draw chord name
                    DrawChordName(g, ChordName, x);
                }
                // Increase x of 1 cell
                x += (int)(_cellwidth) + _LinesWidth;

            }
        }

        private void DrawChordsByBeat(Graphics g, Rectangle clip)
        {
            if (Gridchords == null)
                return;
            
            string ChordName;
            string currentChordName = string.Empty;
            int _LinesWidth = 2;
            int x = (_LinesWidth - 1);

            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                ChordName = GridBeatChords[i];

                // First chord                
                if (ChordName != "" && ChordName != EmptyChord && ChordName != currentChordName && ChordName != NoChord)
                {
                    currentChordName = ChordName;

                    // Draw Chord bitmap
                    DrawChord(g, ChordName, x);

                    // Draw chord name
                    DrawChordName(g, ChordName, x);

                    // Increase x of 1 cell
                    x += (int)(_cellwidth) + _LinesWidth;                

                }
                // Increase x of 1 cell
                //x += (int)(_cellwidth) + _LinesWidth;                

            }
        }

        /// <summary>
        /// Draw a chord
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ChordName"></param>
        /// <param name="pos"></param>
        private void DrawChord(Graphics g, string ChordName, int pos)
        {
            // Draw Chord bitmap
            if (ChordName != "")
            {
                try
                {
                    ResourceManager rm = Resources.ResourceManager;
                    Bitmap chordImage = (Bitmap)rm.GetObject(ChordName);


                    if (chordImage != null)
                    {
                        Point p = new Point(pos, 0);

                        /*
                        Bitmap Img = new Bitmap(chordImage);                        
                        g.DrawImage(Img, p);
                        Img.Dispose();
                        */
                        Size newSize = new Size((int)(chordImage.Width * zoom), (int)(chordImage.Height * zoom));
                        Bitmap bmp = new Bitmap(chordImage, newSize);

                        //g.DrawImage(chordImage, p);
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
        /// Draw a chord name
        /// </summary>
        /// <param name="g"></param>
        /// <param name="ChordName"></param>
        /// <param name="pos"></param>
        private void DrawChordName(Graphics g, string ChordName, int pos)
        {
            float w;
            float h;
            SolidBrush ChordBrush = new SolidBrush(Color.FromArgb(29, 29, 29));


            // Write the name of the chord at the bottom
            w = MeasureString(_fontChord.FontFamily, ChordName, _fontChord.Size);
            h = MeasureStringHeight(_fontChord.FontFamily, ChordName, _fontChord.Size);

            if (ChordName != EmptyChord)
            {                
                g.DrawString(ChordName, _fontChord, ChordBrush, pos + (_cellwidth - w) / 2, _cellheight - h - _fontpadding);
            }
            else
            {                
                g.DrawString(ChordName, _fontChord, ChordBrush, pos + (_cellwidth - w) / 2, _cellheight - h - _fontpadding);
            }
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

                //DrawGrid(g, clip);

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
