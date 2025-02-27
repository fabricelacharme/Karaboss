#region License

/* Copyright (c) 2025 Fabrice Lacharme
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace keffect
{
   

    public partial class KaraokeEffect : UserControl, IMessageFilter
    {

        #region Move form without title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private HashSet<Control> controlsToMove = new HashSet<Control>();
        #endregion


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

        private int _FirstLineToShow = 0;
        private int _LastLineToShow = 0;

        private int _lastLine = -1;
        private int _line = 0;
        private int _lines = 0;
        private int _lineHeight = 0;
        private int _linesHeight = 0;
        private string _biggestLine =string.Empty;

        #endregion decl
     

        [Serializable()]
        public struct kSyncText
        {
            public long Time { get; set; }
            public string Text { get; set; }

            public kSyncText(long time, string text)
            {
                this.Time = time;
                this.Text = text;
            }
        }


        #region properties


        #region SlideShow
        public Rectangle m_DisplayRectangle { get; set; }

        private BackgroundWorker backgroundWorkerSlideShow;

        private Random random;
        private string[] bgFiles;
        private string DefaultDirSlideShow;

        private List<string> m_ImageFilePaths;
        private MemoryStream m_ImageStream = null;
        
        private ManualResetEvent m_FinishEvent = new ManualResetEvent(false);

        private bool m_Cancel = false;
        private bool m_Restart = false;

        delegate void UpdateTimerEnableCallback(bool enabled);

        public bool IsBusy
        {
            get
            {

                if (backgroundWorkerSlideShow != null)
                    return backgroundWorkerSlideShow.IsBusy;
                else
                    return false;
            }
        }


        /// <summary>
        /// SlideShow frequency
        /// </summary>
        private int _freqdirslideshow = 10;
        public int FreqDirSlideShow
        {
            get { return _freqdirslideshow; }
            set { _freqdirslideshow = value; }
        }

        /// <summary>
        /// Size mode of picturebox
        /// </summary>
        private PictureBoxSizeMode _sizemode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizemode; }
            set
            {
                _sizemode = value;
                pBox.SizeMode = _sizemode;
            }
        }

        private int PAUSE_TIME;
        private int rndIter = 0;
        private string strCurrentImage; // current image to insure that random will provide a different one

        public ImageLayout imgLayout { get; set; }
        public Image m_CurrentImage { get; set; }
        public int m_Alpha { get; set; }


        #endregion SlideShow


      


        // Text To Upper
        private bool _bforceUppercase = false;
        public bool bforceUppercase
        {
            get { return _bforceUppercase; }
            set 
            {
                if (value != _bforceUppercase)
                {
                    _bforceUppercase = value;
                    Init();
                    pBox.Invalidate();
                }
            
            }
        }

        private bool _bTextBackGround = false;
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set { _bTextBackGround = value; }
        }
       

        /// <summary>
        /// Transparency color
        /// </summary>
        private Color _transparencykey = Color.Lime;
        public Color TransparencyKey
        {
            get { return _transparencykey; }
            set { _transparencykey = value; }
        }

        private string _optionbackground;
        public string OptionBackground
        {
            get { return _optionbackground; }
            set
            {
                _optionbackground = value;

                switch (_optionbackground)
                {
                    case "Diaporama":
                        break;
                    case "SolidColor":
                        m_Cancel = true;
                        Terminate();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        pBox.BackColor = _txtbackcolor;
                        pBox.Invalidate();
                        break;
                    case "Transparent":
                        m_Cancel = true;
                        Terminate();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        pBox.BackColor = _transparencykey;
                        pBox.Invalidate();
                        break;
                    default:
                        break;
                }

            }
        }

        /// <summary>
        /// Display lyrics option: top, Center, Bottom
        /// </summary>
        public enum OptionsDisplay
        {
            Top = 0,
            Center = 1,
            Bottom = 2,
        }

        private OptionsDisplay _OptionDisplay;
        /// <summary>
        /// Display lyrics option: top, Center, Bottom
        /// </summary>
        public OptionsDisplay OptionDisplay
        {
            get { return _OptionDisplay; }
            set
            {
                _OptionDisplay = value;
                pBox.Invalidate();
            }
        }

        private List<kSyncText> _SyncLine;
        public List<kSyncText> SyncLine
        {
            get { return _SyncLine; }
            set { _SyncLine = value; }
        }

        private List<List<kSyncText>> _SyncLyrics;
        public List<List<kSyncText>> SyncLyrics
        {
            get { return _SyncLyrics; }
            set
            {
                if (value == null) return;

                _SyncLyrics = value;
                Init();
            }
        }

        public enum TransitionEffects
        {
            None,
            Progressive,
        }

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
            set { 
                _nbLyricsLines = value;
                Init();
                pBox.Invalidate();
            }
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


        private Color _txtbackcolor;
        public Color TxtBackColor
        {
            get { return _txtbackcolor; }
            set { _txtbackcolor = value; }
        }


        private Color _txtcontourcolor;
        public Color TxtContourColor
        {
            get { return _txtcontourcolor; }
            set { _txtcontourcolor = value; }
        }

        private bool _bColorContour = false;
        public bool bColorContour
        {
            get { return _bColorContour; }
            set 
            {
                if (value != _bColorContour)
                {
                    _bColorContour = value;
                    pBox.Invalidate();
                }
            }
        }

        #endregion properties




        /// <summary>
        /// Constructor
        /// </summary>
        public KaraokeEffect()
        {
            InitializeComponent();


            #region Move form without title bar
            
            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.pBox);
            
            #endregion


            this.SetStyle(
                 System.Windows.Forms.ControlStyles.UserPaint |
                 System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                 System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                 true);

            SetDefaultValues();

            Init();
                       
        }

        #region Move Window

        /// <summary>
        /// Move form without title bar
        /// The message is sent to the parent Form (this.ParentForm.Handle)
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.ParentForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        #endregion Move Window

        private void SetDefaultValues()
        {

            m_ImageFilePaths = new List<string>();
            m_Alpha = 255;
            imgLayout = ImageLayout.Stretch;


            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
            //sf.Alignment = StringAlignment.Center;
            _karaokeFont = new Font("Comic Sans MS", emSize, FontStyle.Regular, GraphicsUnit.Pixel);
            
            _steppercent = 0.01F;

            // Add new line "Hello World"
            SyncLyrics = new List<List<kSyncText>>();
            SyncLine = new List<kSyncText> { new kSyncText(0, "Hello"), new kSyncText(500, " World") };                        
            SyncLyrics.Add(SyncLine);

            _nbLyricsLines = 1;

            _transitionEffect = TransitionEffects.Progressive;

         }


        private void Init()
        {            
            Lines = new List<string[]>();
            Times = new List<long[]>();
            
            List<kSyncText> syncline = new List<kSyncText>();
            string[] s;
            long[] t;
            string tx;


            for (int i = 0; i < _SyncLyrics.Count; i++)
            {
                syncline = _SyncLyrics[i];
                t = new long[syncline.Count];
                s = new string[syncline.Count];

                for (int j = 0; j < syncline.Count; j++ )
                {
                    t[j] = syncline[j].Time;

                    // Clean text
                    tx = syncline[j].Text;
                    tx = tx.Replace(Environment.NewLine, "");
                    if (_bforceUppercase)
                        tx = tx.ToUpper();

                    s[j] = tx;
                }
                Times.Add(t);
                Lines.Add(s);                
            }
                          
            
            _lines = Lines.Count;           
            
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

            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _lines, _nbLyricsLines);

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
            AjustText(_biggestLine);
            pBox.Invalidate();
        }

        private void pBox_Paint(object sender, PaintEventArgs e)
        {           
            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.PageUnit = GraphicsUnit.Pixel;           

            SolidBrush colorBrush;

            #region draw background image
            int x;
            int y;

            if (m_CurrentImage != null)
            {
                #region sizemode
                switch (_sizemode)
                {
                    case PictureBoxSizeMode.AutoSize:
                        x = (this.ClientSize.Width - m_CurrentImage.Width) / 2;
                        y = (this.ClientSize.Height - m_CurrentImage.Height) / 2;
                        m_DisplayRectangle = new Rectangle(x, y, m_CurrentImage.Width, m_CurrentImage.Height);
                        break;
                    case PictureBoxSizeMode.CenterImage:
                        x = (this.ClientSize.Width - m_CurrentImage.Width) / 2;
                        y = (this.ClientSize.Height - m_CurrentImage.Height) / 2;
                        m_DisplayRectangle = new Rectangle(x, y, m_CurrentImage.Width, m_CurrentImage.Height);
                        break;
                    case PictureBoxSizeMode.Normal:
                        // coin superieur gauche
                        m_DisplayRectangle = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        //  l'image est étirée ou réduite pour s'ajuster à PictureBox.
                        m_DisplayRectangle = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                        break;
                    case PictureBoxSizeMode.Zoom:
                        m_DisplayRectangle = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
                        break;
                }
                #endregion

                try
                {
                    e.Graphics.DrawImage(m_CurrentImage, m_DisplayRectangle, 0, 0, m_CurrentImage.Width, m_CurrentImage.Height, GraphicsUnit.Pixel);

                }
                catch (Exception dr)
                {
                    Console.Write("Error drawing image: " + dr.Message);
                }

            }


            #endregion draw background image


            #region draw text

            int y0 = VCenterText();
            int x0 = 0;

            // ======================================================================================================
            // 1. Draw and color all lines from _linedeb to _linefin in white
            // We want to display only a few number of lines (variable _nbLyricsLines = number of lines to display)  
            // ======================================================================================================
            var otherpath = new GraphicsPath();

            for (int i = _FirstLineToShow; i <= _LastLineToShow; i++)
            {
                if (i < Texts.Count()) {
                    x0 = HCenterText(Texts[i]);     // Center horizontally
                    otherpath.AddString(Texts[i], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0 + (i - _FirstLineToShow) * _lineHeight), sf);
                }
            }

            colorBrush = new SolidBrush(_NotYetPlayedColor);
            //e.Graphics.FillPath(new SolidBrush(Color.White), otherpath);
            e.Graphics.FillPath(colorBrush, otherpath);

            // Borders of text
            if (_bColorContour)
                e.Graphics.DrawPath(new Pen(Color.Black, 1), otherpath);

            otherpath.Dispose();


            // =============================================
            // 2. Color in green/Red the current line
            // =============================================
            // Create a graphical path
            var path = new GraphicsPath();

            // Add the full text line to the graphical path            
            if (_FirstLineToShow < Texts.Count())
            {
                x0 = HCenterText(Texts[_FirstLineToShow]);      // Center horizontally
                path.AddString(Texts[_FirstLineToShow], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);
            }

            // Fill graphical path in white => full text is white
            //e.Graphics.FillPath(new SolidBrush(Color.White), path);
            e.Graphics.FillPath(colorBrush, path);



            // ======================================================
            // Color in GREEN the syllabes before current syllabe
            // ======================================================
            // Create a region from the graphical path
            Region r = new Region(path);
            // Create a retangle of the graphical path
            RectangleF rect = r.GetBounds(e.Graphics);

            RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRectBefore);

            colorBrush = new SolidBrush(_AlreadyPlayedColor);
            //e.Graphics.FillRegion(Brushes.Green, r);
            e.Graphics.FillRegion(colorBrush, r);

           


            // ======================================================
            // Color in RED the  current syllabe
            // ======================================================
            r = new Region(path);

            // Create another rectangle shorter than the 1st one (percent of the first)                       
            RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);


            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRect);

            // Fill updated region in red => percent portion of text is red
            colorBrush = new SolidBrush(_BeingPlayedColor);
            //e.Graphics.FillRegion(Brushes.Red, r);
            e.Graphics.FillRegion(colorBrush, r);

            


            // Borders of text
            if (_bColorContour)
                e.Graphics.DrawPath(new Pen(_txtcontourcolor, 1), path);
            


            colorBrush.Dispose();
            r.Dispose();
            path.Dispose();

            #endregion draw text
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


        #region deleteme

        /// <summary>
        /// Retrieve which line for pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetLine(int pos)
        {
            /*
            for (int i = 0; i < Lines.Count; i++)
            {
                if (pos < Times[i][Times[i].Count() - 1])
                {
                    return i;                                        
                }
            }
            return 0;
            */
            /*
            for (int i = 0; i < Lines.Count; i++)
            {
                if (pos >= Times[i][0])
                {
                    return i;
                }
            }
            return 0;
            */

            for (int j = 0; j < Lines.Count; j++)
            {
                // Search for which timespamp is greater than pos
                for (int i = 0; i < Times[j].Length; i++)
                {
                    if (pos < Times[j][i])
                    {
                        return j;
                    }
                }
            }
            return Lines.Count - 1;

            /*
            for (int i = Lines.Count -1; i >= 0; i--)
            {
                if (pos >= Times[i][0])
                {
                    return i;
                }

            }
            return 0;
            */
        }

        #endregion deleteme

        /// <summary>
        /// Retrieve index of current syllabe in the current line
        /// </summary>
        /// <returns></returns>       
        private int GetIndex(int pos)
        {
          
            
            for (int j = Lines.Count - 1; j >= 0; j--)
            {
                for (int i = Times[j].Length - 1; i >= 0; i--)
                {
                    if (Times[j][i] > 0 && pos > Times[j][i])
                    {                        
                        _line = j;
                        return i + 1;
                    }
                }
            }            
            return 0;            
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
                if (i < Lines[_line].Count())                
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
            int y = 0;
            
            // Height of control minus height of lines to show
            switch (_OptionDisplay)
            {
                case OptionsDisplay.Center:
                    y = (pBox.ClientSize.Height - (_nbLyricsLines) * _lineHeight) / 2;
                    break;
                
                case OptionsDisplay.Top:
                    y = 0;
                    break;
                
                case OptionsDisplay.Bottom:
                    y = pBox.ClientSize.Height - (_nbLyricsLines * (_lineHeight + 1));
                    break;
            }


            
            return y > 0 ? y : 0;
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
            _FirstLineToShow = 0;
            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _lines, _nbLyricsLines);

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
            // Search index of lyric to play
            index = GetIndex(pos);

            // Length of partial line
            CurLength = GetCurLength(index);

            // New word to highlight
            // Warning: in cas of full lines, index is always the same and not different than lastIndex
            if (index != lastindex || _line != _lastLine)
            {
                                
                // Line changed
                if (_line !=  _lastLine)
                {
                    _lastLine = _line;
                    percent = 0;
                    lastpercent = 0;
                    index = 0;
                    lastindex = -1;
                    lastCurLength = 0;
                    CurLength = 0;
                }
                
                _FirstLineToShow = _line;
                _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _lines, _nbLyricsLines);


                //Console.WriteLine("test Line : " + line);
                Console.WriteLine("Line : " + _line + " - index : " + index);

                
                // Save last value of percent
                lastpercent = percent;

                // Set new value of percent to the end of the previous word
                // And after that, add a small progressive increment in order to increase the percentage

                // |--- last word ---|--- new word --------------------------|
                //                   | percent => percent+pas => percent+pas

                if (_line < LinesLengths.Count())
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


        #region SlideShow


        private void LoadImageList(string dir)
        {
            bgFiles = Directory.GetFiles(@dir, "*.jpg");
            m_ImageFilePaths.Clear();
            for (int i = 0; i < bgFiles.Length; ++i)
            {
                string file = bgFiles[i];
                m_ImageFilePaths.Add(file);
            }
        }

        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void SetBackground(string dirImages)
        {
            try
            {
                UpdateTimerEnable(false);

                m_Cancel = true;
                m_Restart = true;

                m_CurrentImage = null;
                strCurrentImage = string.Empty;
                rndIter = 0;

                pBox.Image = null;
                pBox.Invalidate();
                m_ImageFilePaths.Clear();

                if (dirImages == null)
                {
                    pBox.BackColor = Color.Black;
                }
                else if (Directory.Exists(dirImages))
                {
                    int C = 0;

                    if (_optionbackground == "Diaporama")
                    {
                        LoadImageList(dirImages);
                        C = m_ImageFilePaths.Count;
                    }

                    switch (C)
                    {
                        case 0:
                            // No image, just background color
                            m_Cancel = true;
                            break;
                        case 1:
                            // Single image
                            m_Cancel = true;
                            pBox.Image = Image.FromFile(m_ImageFilePaths[0]);
                            break;
                        default:
                            // Slideshow => backgroundworker

                            //m_Cancel = true;
                            m_Cancel = false;

                            // Initialize backgroundworker
                            InitBackGroundWorker();
                            random = new Random();
                            StartBgW();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

        }

        private void InitBackGroundWorker()
        {
            backgroundWorkerSlideShow = new System.ComponentModel.BackgroundWorker();
            backgroundWorkerSlideShow.WorkerSupportsCancellation = true;
            backgroundWorkerSlideShow.WorkerReportsProgress = true;
            backgroundWorkerSlideShow.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSlideShow_DoWork);
            backgroundWorkerSlideShow.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSlideShow_RunWorkerCompleted);
        }

        private string SelectRndFile(List<string> files)
        {            
            if (files.Count > 0)
            {
                int rand = random.Next(0, files.Count);
                if (files[rand] != strCurrentImage || rndIter > 10)
                {
                    rndIter = 0;
                    strCurrentImage = files[rand];
                    return files[rand];
                }
                else
                {
                    rndIter++;
                    return SelectRndFile(files);
                }
            }
            else
            {
                return null;
            }
        }

        private void backgroundWorkerSlideShow_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (m_Restart == true)
            {
                StopBgW();

                int C = m_ImageFilePaths.Count;

                switch (C)
                {
                    case 0:
                        break;
                    case 1:
                        pBox.Image = Image.FromFile(m_ImageFilePaths[0]);
                        break;
                    default:
                        StartBgW();
                        break;
                }
            }
            else
            {
                backgroundWorkerSlideShow.Dispose();
                SetBackground(DefaultDirSlideShow);
            }
        }

        private void backgroundWorkerSlideShow_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> files = (List<string>)e.Argument;

            do
            {
                if (m_Cancel == true)
                {
                    break;
                }

                string file = SelectRndFile(files);


                UpdateTimerEnable(true);
                m_FinishEvent.Reset();

                if (m_ImageStream != null)
                {
                    m_ImageStream.Dispose();
                    m_ImageStream = null;
                }

                try
                {
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] ba = new byte[fs.Length];
                        fs.Read(ba, 0, ba.Length);
                        m_ImageStream = new MemoryStream(ba);

                        if (m_CurrentImage != null)
                        {
                            m_CurrentImage.Dispose();
                            m_CurrentImage = null;
                        }

                        m_CurrentImage = Image.FromStream(m_ImageStream);
                        fs.Dispose();
                        pBox.Invalidate();
                    }
                }
                catch (Exception op)
                {
                    m_CurrentImage = null;
                    Console.WriteLine("Error opening image " + op.Message);
                }


                // do not launch if new slideshow is required
                if (m_Restart == false)
                {
                    //UpdateTimerEnable(true);
                    //m_FinishEvent.WaitOne();
                    m_FinishEvent.Reset();

                    PAUSE_TIME = 1000 * _freqdirslideshow;
                    Thread.Sleep(PAUSE_TIME);
                }
            } while (m_Cancel == false);
        }


        private void UpdateTimerEnable(bool enabled)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    UpdateTimerEnableCallback d = new UpdateTimerEnableCallback(UpdateTimerEnable);
                    pBox.Invoke(d, new object[] { enabled });
                }
                catch (Exception u)
                {
                    Console.WriteLine("Error UpdateTimerEnable " + u.Message);
                }
            }
        }


        private void StartBgW()
        {
            if (backgroundWorkerSlideShow.IsBusy)
            {
                StopBgW();
            }

            try
            {
                if (!backgroundWorkerSlideShow.IsBusy)
                {
                    m_Restart = false;
                    m_Cancel = false;
                    backgroundWorkerSlideShow.RunWorkerAsync(m_ImageFilePaths);
                }
            }
            catch (Exception est)
            {
                //m_Restart = true;
                Console.WriteLine("Error starting backgroundworker: " + est.Message);
            }
        }

        private void StopBgW()
        {
            if (backgroundWorkerSlideShow.IsBusy)
            {
                backgroundWorkerSlideShow.CancelAsync();

                m_Cancel = true;
            }
        }

        /// <summary>
        /// Terminate
        /// </summary>
        public void Terminate()
        {
            m_Cancel = true;
            m_Restart = false;

            m_ImageFilePaths = new List<string>();
            if (m_ImageStream != null)
            {
                m_ImageStream.Dispose();
                m_ImageStream = null;
            }

            if (backgroundWorkerSlideShow != null)
            {
                backgroundWorkerSlideShow.CancelAsync();
            }

        }

        #endregion SlideShow

    }
}
