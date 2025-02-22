using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace lyrics
{

    public enum TransitionEffects
    {
        None,
        Progressive,
    }


    public struct SyncText
    {
        public long time;
        public string lyric;

        public SyncText(long time, string lyric)
        {
            this.time = time;
            this.lyric = lyric;
        }
    }

    public partial class KaraokeEffect : UserControl
    {

        #region decl

        private float percent = 0;
        private float lastpercent = 0;

        private List<string[]> Lines;
        private List<long[]> Times;
        private string[] Texts;
        private float[] LinesLengths;        
        
        private int index = 0;
        private int lastindex = -1;        
        private float CurLength;
        private float lastCurLength;

        private Font m_font;   // used to measure strings without changing _karaokeFont
        private float emSize = 40;

        private StringFormat sf;

        private int _FirstLine = 0;
        private int _LastLine = 0;
        private int _line = 0;
        private int _lines = 0;
        private int _lineHeight = 0;
        private int _linesHeight = 0;
        private string _biggestLine =string.Empty;

        #endregion decl


        #region properties
        
        private TransitionEffects _transitionEffect;
        public TransitionEffects TransitionEffect 
        { 
            get { return _transitionEffect; } 
            set { _transitionEffect = value; }
        }
        
        
        private int _position = 0;
        /// <summary>
        /// Player position => highlight lyrics at this position
        /// </summary>
        [Description("Position")]
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }


        private List<SyncText> _SyncLine;
        public List<SyncText> SyncLine
        {
            get { return _SyncLine; }
            set { _SyncLine = value; }
        }

        private List<List<SyncText>> _SyncLyrics;
        public List<List<SyncText>> SyncLyrics
        {
            get { return _SyncLyrics; }
            set 
            { 
                if (value == null) return;

                _SyncLyrics = value; 
                Init();              
            }
        }
      

        private float _steppercent = 0.01F;
        [Description("Increment to display a syllable progressively")]
        public float StepPercent
        {
            get { return _steppercent; }
            set { _steppercent = value; }
        }

        private int _nbLyricsLines = 3;
        [Description("The number of lines to display")]
        public int nbLyricsLines
        {
            get { return _nbLyricsLines; }
            set { _nbLyricsLines = value; }
        }
               
        private Font _karaokeFont;
        [Description("The font of the component")]
        public Font KaraokeFont
        {
            get { return _karaokeFont; }
            set { _karaokeFont = value; }
        }


        private Color _backcolor = Color.Black;
        [Description("The background color of the component")]
        public override Color BackColor
        {
            get { return _backcolor; }
            set { 
                _backcolor = value; 
                pBox.BackColor = value;
                pBox.Invalidate();
            }
        }

        
        private Color _AlreadyPlayedColor = Color.FromArgb(153, 180, 51);  // Green
        [Description("Colour of text already played")]
        public Color TxtAlreadyPlayedColor
        {
            get { return _AlreadyPlayedColor; }
            set 
            { 
                _AlreadyPlayedColor = value; 
                pBox.Invalidate();
            }
        }

        private Color _BeingPlayedColor = Color.FromArgb(238, 17, 17); // Red
        [Description("Colour of text being played")]
        public Color TxtBeingPlayedColor
        {
            get { return _BeingPlayedColor; }
            set {
                pBox.Invalidate();
                _BeingPlayedColor = value; 
            }
        }

        private Color _NotYetPlayedColor = Color.White;
        [Description("Colour of text not yet played")]
        public Color TxtNotYetPlayedColor
        {
            get { return _NotYetPlayedColor; }
            set 
            { 
                _NotYetPlayedColor = value;
                pBox.Invalidate();
            }
        }


        private Image m_CurrentImage;
        [Description("Background image behind the text")]
        public Image Image
        {
            get { return m_CurrentImage; }
            set 
            { 
                m_CurrentImage = value;
                try
                {
                    pBox.BackgroundImage = m_CurrentImage;
                    pBox.BackgroundImageLayout = ImageLayout.Stretch;
                    //pBox.Image = value;
                    pBox.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //public override Image BackgroundImage { get => base.BackgroundImage; set => base.BackgroundImage = value; }



        #endregion properties

        /// <summary>
        /// Constructor
        /// </summary>
        public KaraokeEffect()
        {
            InitializeComponent();

            this.SetStyle(
                 System.Windows.Forms.ControlStyles.UserPaint |
                 System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                 System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                 true);

            SetDefaultValues();

            Init();
                       
        }

        private void SetDefaultValues()
        {
            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
            sf.Alignment = StringAlignment.Center;
            _karaokeFont = new Font("Comic Sans MS", emSize, FontStyle.Regular, GraphicsUnit.Pixel);
            
            _steppercent = 0.01F;

            // Add new line "Hello World"
            SyncLyrics = new List<List<SyncText>>();
            SyncLine = new List<SyncText> { new SyncText(0, "Hello"), new SyncText(500, " World") };                        
            SyncLyrics.Add(SyncLine);

            _nbLyricsLines = 1;

            _transitionEffect = TransitionEffects.Progressive;

         }


        private void Init()
        {            
            Lines = new List<string[]>();
            Times = new List<long[]>();
            
            List<SyncText> syncline = new List<SyncText>();
            string[] s;
            long[] t;

            for (int i = 0; i < _SyncLyrics.Count; i++)
            {
                syncline = _SyncLyrics[i];
                t = new long[syncline.Count];
                s = new string[syncline.Count];

                for (int j = 0; j < syncline.Count; j++ )
                {
                    t[j] = syncline[j].time;
                    s[j] = syncline[j].lyric;
                }
                Times.Add(t);
                Lines.Add(s);                
            }
                          
            
            _lines = Lines.Count;
            if (_lines < _nbLyricsLines) 
                _nbLyricsLines = _lines;

            
            string[] line;
            string Tx;
            Texts = new string[Lines.Count];
            LinesLengths = new float[Lines.Count];

            for (int i = 0; i < Lines.Count; i++)
            {
                line = Lines[i];
                Tx = string.Empty;

                for (int j = 0; j < line.Length; j++)
                {
                    Tx += line[j];
                }
                Texts[i] = Tx;                
            }

            // Biggest line
            _biggestLine = GetBiggestLine();
            AjustText(_biggestLine);

            _LastLine = SetLastLineToShow(_FirstLine, _lines, _nbLyricsLines);

        }

        /// <summary>
        /// Determine the last line that can be displayed according to number of lines, position  of first line
        /// and how many lines we xant to display
        /// </summary>
        /// <param name="FirstLine"></param>
        /// <param name="nbLines"></param>
        /// <param name="nbLinesToShow"></param>
        /// <returns></returns>
        private int SetLastLineToShow(int FirstLine, int nbLines, int nbLinesToShow)
        {
            int LastLine;

            if (nbLines == 0) return FirstLine;

            if (FirstLine + nbLinesToShow <= nbLines)
                LastLine = FirstLine + nbLinesToShow - 1;
            else
                LastLine = nbLines - 1;
                
            return LastLine;
        
        }

        #region measures
        /// <summary>
        /// Measure the length of a string with a specific size
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fSize"></param>
        /// <returns></returns>
        private float MeasureString(string line, float femSize)
        {
            float ret = 0;
            if (line != "")
            {
                using (Graphics g = pBox.CreateGraphics())
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.PageUnit = GraphicsUnit.Pixel;

                    m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);
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
        private float MeasureStringHeight(string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pBox.CreateGraphics())
                {

                    if (femSize > 0)
                        m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    SizeF sz = g.MeasureString(line, m_font, new Point(0, 0), sf);
                    ret = sz.Height;

                    g.Dispose();
                }
            }
            return ret;
        }

        /// <summary>
        /// Measure all lines
        /// </summary>
        /// <param name="curline"></param>
        /// <returns></returns>
        private float MeasureLine(int curline)
        {
            float Sum = 0;
            for (int i = 0; i < Lines[curline].Length; i++)
            {                
                Sum += MeasureString(Lines[curline][i], _karaokeFont.Size);
            }

            return Sum;
        }

        #endregion measures


        #region Control Load Resize paint
        private void KaraokeEffect_Resize(object sender, EventArgs e)
        {
            // Increase _steppercent if Width increase
            //_steppercent = _steppercent * Width/500;

            AjustText(_biggestLine);
            pBox.Invalidate();
        }

        private void pBox_Paint(object sender, PaintEventArgs e)
        {           
            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.PageUnit = GraphicsUnit.Pixel;

            
            int y0 = VCenterText();
            int x0;

            // ======================================================================================================
            // 1. Draw and color all lines from _linedeb to _linefin in white
            // We want to display only a few number of lines (variable _nbLyricsLines = number of lines to display)  
            // ======================================================================================================
            var otherpath = new GraphicsPath();

            for (int i = _FirstLine; i <= _LastLine; i++)
            {
                x0 = HCenterText(Texts[i]);     // Center horizontally
                otherpath.AddString(Texts[i], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0 + (i - _FirstLine) * _lineHeight), StringFormat.GenericDefault);
            }
            e.Graphics.FillPath(new SolidBrush(Color.White), otherpath);

            // Borders of text
            e.Graphics.DrawPath(new Pen(Color.Black, 1), otherpath);

            otherpath.Dispose();


            // =============================================
            // 2. Color in green/Red the current line
            // =============================================
            // Create a graphical path
            var path = new GraphicsPath();

            // Add the full text line to the graphical path            
            x0 = HCenterText(Texts[_FirstLine]);      // Center horizontally
            path.AddString(Texts[_FirstLine], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), StringFormat.GenericDefault);

            // Fill graphical path in white => full text is white
            e.Graphics.FillPath(new SolidBrush(Color.White), path);

            // ===================
            // Color in green syllabes before current syllabe
            // ===================
            // Create a region from the graphical path
            Region r = new Region(path);
            // Create a retangle of the graphical path
            RectangleF rect = r.GetBounds(e.Graphics);

            RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRectBefore);
            e.Graphics.FillRegion(Brushes.Green, r);


            // =======================
            // Color in green current syllabe
            // =======================
            r = new Region(path);

            // Create another rectangle shorter than the 1st one (percent of the first)                       
            RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);


            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRect);

            // Fill updated region in red => percent portion of text is red
            e.Graphics.FillRegion(Brushes.Red, r);

            // Borders of text
            e.Graphics.DrawPath(new Pen(Color.Black, 1), path);

            r.Dispose();
            path.Dispose();
           
        }

        #endregion Control Load Resize


        #region Get infos
        /// <summary>
        /// Search for biggest line
        /// </summary>
        /// <returns></returns>
        private string GetBiggestLine()
        {
            float max = 0;
            string maxline = string.Empty;
            float L = 0;
            for (int i = 0; i < Lines.Count; i++)
            {
                L = MeasureLine(i);

                // Search line having max characteres
                if (L > max)
                {
                    max = L;
                    maxline = Texts[i];
                }
            }
            return maxline;
        }

               
      /// <summary>
      /// Retrieve which line for pos
      /// </summary>
      /// <param name="pos"></param>
      /// <returns></returns>
        private int GetLine(int pos)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (pos < Times[i][Times[i].Count() - 1])
                {
                    return i;
                }
            }
            return 0;
        }


        /// <summary>
        /// Retrive index of current syllabe in the current line
        /// </summary>
        /// <returns></returns>       
        private int GetIndex(int pos)
        {
            // For each line of lyrics
            for (int j = 0; j < Lines.Count; j++)
            {
                // Search for which timespamp is greater than pos
                for (int i = 0; i < Times[_line].Length; i++)
                {
                    if (pos < Times[_line][i])
                    {
                        return i + 1;
                    }
                }
            }
            return Lines.Count - 1;
        }


        /// <summary>
        /// Mesure length of a portion of line
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private float GetCurLength(int idx)
        {
            float res = 0;
            for (int i = 0; i < idx; i++)
            {                
                res += MeasureString(Lines[_line][i], _karaokeFont.Size);
            }
            return res;
        }

        #endregion get infos


        #region adjust text
        /// <summary>
        /// Ajust size of font regarding size of pictureBox1
        /// </summary>
        /// <param name="S"></param>
        private void AjustText(string S)
        {
            if (S != "" && pBox != null)
            {
                Graphics g = pBox.CreateGraphics();
                float femsize;
                long inisize = (long)_karaokeFont.Size;
                femsize = g.DpiX * inisize / 72;

                float textSize = MeasureString(S, femsize);

                // Try to fit inside 90% of client size
                long comp = (long)(0.90 * pBox.ClientSize.Width);

                // Texte trop large
                if (textSize > comp)
                {
                    do
                    {
                        inisize--;
                        if (inisize > 0)
                        {
                            femsize = g.DpiX * inisize / 72;
                            textSize = MeasureString(S, femsize);
                        }
                    } while (textSize > comp && inisize > 0);
                }
                else
                {
                    do
                    {
                        inisize++; //= inisize + 1;                        
                        femsize = g.DpiX * inisize / 72;
                        textSize = MeasureString(S, femsize);
                    } while (textSize < comp);
                }

                // ------------------------------
                // Ajustement in height 
                // ------------------------------
                float textHeight = MeasureStringHeight(S, inisize);
                float totaltextHeight;
                totaltextHeight = _nbLyricsLines * (textHeight + 10);

                long compHeight = (long)(0.95 * pBox.ClientSize.Height);

                if (totaltextHeight > compHeight)
                {
                    do
                    {
                        inisize--;
                        if (inisize > 0)
                        {
                            femsize = g.DpiY * inisize / 72;
                            textHeight = MeasureStringHeight(S, femsize);

                            totaltextHeight = _nbLyricsLines * (textHeight + 10);
                        }
                    } while (totaltextHeight > compHeight && inisize > 0);
                }


                if (inisize > 0)
                {
                    emSize = g.DpiY * inisize / 72;
                    _karaokeFont = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    // Vertical distance between lines                    
                    _lineHeight = (int)(1.2 * emSize);
                    // Height of the full song
                    _linesHeight = _nbLyricsLines * _lineHeight;


                    // Update horizontal measure of lines
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        LinesLengths[i] = MeasureLine(i);
                    }

                }
                g.Dispose();
            }
        }

        /// <summary>
        /// Center text vertically
        /// </summary>
        /// <returns></returns>
        private int VCenterText()
        {
            // Height of control minus height of lines to show
            //int res = (pBox.ClientSize.Height - (_nbLyricsLines + 1) * _lineHeight) / 2;
            int res = (pBox.ClientSize.Height - (_nbLyricsLines) * _lineHeight) / 2;
            return res > 0 ? res : 0;
        }

        /// <summary>
        /// Center text horizontally
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private int HCenterText(string s)
        {            
            int res = -(int)_karaokeFont.Size / 2 + (pBox.ClientSize.Width - (int)MeasureString(s, _karaokeFont.Size)) / 2;
            return res > 0 ? res : 0;
        }

        #endregion ajust text


     
                      

        #region start stop

        // Start Display lyrics
        public void Start()
        {
            _line = 0;
            percent = 0;
            lastpercent = 0;
            index = 0;
            lastindex = -1;
            lastCurLength = 0;
            CurLength = 0;

        }

        public void Stop()
        {
            _line = 0;
            _FirstLine = 0;
            _LastLine = SetLastLineToShow(_FirstLine, _lines, _nbLyricsLines);

            percent = 0;
            lastpercent = 0;
            index = 0;
            lastindex = -1;
            lastCurLength = 0;
            CurLength = 0;
            pBox.Invalidate();
        }

        /// <summary>
        ///  player position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPos(double ms)
        {            
            SetPosition((int)ms);            

        }

        private void SetPosition(int pos)
        {
            // line changed by trackbar            
            int line = GetLine(pos);

            // If pos is greater than last position of currentline
            if (line != _line || pos > Times[_line][Times[_line].Count() - 1])
            {
                if (_line < Lines.Count - 1)
                {
                    //Console.WriteLine("*** New line");
                    percent = 0;
                    lastpercent = 0;
                    index = 0;
                    lastindex = -1;
                    lastCurLength = 0;
                    CurLength = 0;
                    _line = GetLine(pos);
                    _FirstLine = _line;
                    _LastLine = SetLastLineToShow(_FirstLine, _lines, _nbLyricsLines);
                }
                else
                {
                    // Is it the end of the text to display?
                    //Console.WriteLine("*** END");                    
                    _line = 0;
                    _FirstLine = 0;
                    _LastLine = SetLastLineToShow(_FirstLine, _lines, _nbLyricsLines);

                    percent = 0;
                    lastpercent = 0;
                    index = 0;
                    lastindex = -1;
                    lastCurLength = 0;
                    CurLength = 0;
                    pBox.Invalidate();
                    return;
                }

            }

            // Search index of lyric to play
            index = GetIndex(pos);

            // Length of partial line
            CurLength = GetCurLength(index);

            // New word to highlight
            if (index != lastindex)
            {
                // Save last value of percent
                lastpercent = percent;

                // Set new value of percent to the end of the previous word
                // And after that, add a small progressive increment in order to increase the percentage

                // |--- last word ---|--- new word --------------------------|
                //                   | percent => percent+pas => percent+pas

                percent = (lastCurLength / LinesLengths[_line]);



                // Caculate distance between LastCurLength et CurLength
                float d = (float)(CurLength - lastCurLength);
                if (_transitionEffect == TransitionEffects.None)
                {
                    _steppercent = d;
                }
                else if (_transitionEffect == TransitionEffects.Progressive)
                {
                    
                    // Set 3000 occurences to reach the end 
                    _steppercent = d / 3000;
                }
                

                lastCurLength = CurLength;
                lastindex = index;
                pBox.Invalidate();
            }
            else
            {
                // if same index: progressive increase of percent
                percent += _steppercent;

                if (percent > (CurLength / LinesLengths[_line]))
                {
                    percent = (CurLength / LinesLengths[_line]);
                }
                pBox.Invalidate();
            }

        }

        #endregion start stop
    }
}
