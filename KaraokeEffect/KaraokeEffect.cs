#region License

/* Copyright (c) 2026 Fabrice Lacharme
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
using System.Drawing.Imaging;
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


        #region Slideshow

        System.Timers.Timer timerTransition;
        System.Timers.Timer timerChangeImage;

        private float mBlend;
        private int mDir = 1;
        private int count = 0;

        private Image mImg1;
        private Image mImg2;
        private Image Image1
        {
            get { return mImg1; }
            set { mImg1 = value; Invalidate(); }
        }
        private Image Image2
        {
            get { return mImg2; }
            set { mImg2 = value; Invalidate(); }
        }
        private float m_Blend
        {
            get { return mBlend; }
            set { mBlend = value; Invalidate(); }
        }
        private Bitmap[] pictures;

        #endregion slideshow


        #region SlideShow old
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


        #endregion SlideShow old


        #region decl

        private float percent = 0;
        private float lastpercent = 0;
        private long _timerintervall = 50;      // Intervall of timer of frmMp3Player

        public long timerIntervall
        {
            get { return _timerintervall; }
            set { 
                
                if (value >= 10)
                    _timerintervall = value; }
        }

        private List<string[]> Lines;
        private List<long[]> Times;
        private string[] Texts;
        private float[] LinesLengths;        
        
        private int nextindex = 0;
        private int lastindex = 0;        
        private float CurLength;
        private float lastCurLength;

        long _nexttime;
        long _lasttime;

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


        private string current_fragment = string.Empty;

        #region Gradient & Rhythm

        readonly System.Windows.Forms.Timer _timerGradient = new System.Windows.Forms.Timer();

        // Default angle for the gradient
        private float _angle = 45.0f;
        private int W;
        private int H;
        private int speed;
        private int _beat;
        public int Beat
        {
            get { return _beat; }
            set
            {
                _beat = value;
                speed = (int)(_beat / 12.0);
            }
        }
        public float GradientAngle { get { return _angle; } set { _angle = value; pBox.Invalidate(); } }

        private Color _gradientColor0;
        public Color GradientColor0
        {
            get { return _gradientColor0; }
            set
            {
                _gradientColor0 = value;
                pBox.Invalidate();
            }
        }
        private Color _gradientColor1;
        public Color GradientColor1
        {
            get { return _gradientColor1; }
            set
            {
                _gradientColor1 = value;
                pBox.Invalidate();
            }
        }

        private Color _rhythmColor0;
        public Color RhythmColor0
        {
            get { return _rhythmColor0; }
            set
            {
                _rhythmColor0 = value;
                pBox.Invalidate();
            }
        }

        private Color _rhythmColor1;
        public Color RhythmColor1
        {
            get { return _rhythmColor1; }
            set
            {
                _rhythmColor1 = value;
                pBox.Invalidate();
            }
        }
        #endregion Gradient & Rhythm


        private int _beatNumber = 1;

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
      
        #region text color

        /// <summary>
        /// Text sung color
        /// </summary>
        private Color _ActiveColor = Color.FromArgb(153, 180, 51);
        [Description("text color for lyrics that have already been sung")]
        public Color ActiveColor
        {
            get
            { return _ActiveColor; }
            set
            {
                _ActiveColor = value;
                pBox.Invalidate();
            }
        }

        /// <summary>
        /// Text color
        /// </summary>
        private Color _HighlightColor;
        [Description("the color of the lyrics currently being sung")]
        public Color HighlightColor
        {
            get
            { return _HighlightColor; }
            set
            {
                _HighlightColor = value;
                pBox.Invalidate();
            }
        }

        /// <summary>
        /// Text to sing color
        /// </summary>
        private Color _InactiveColor;
        [Description("text color for the remaining lyrics")]
        public Color InactiveColor
        {
            get
            { return _InactiveColor; }
            set
            {
                _InactiveColor = value;
                pBox.Invalidate();
            }
        }

        // Border Color
        private Color _ActiveBorderColor;
        public Color ActiveBorderColor
        {
            get
            { return _ActiveBorderColor; }
            set
            {
                _ActiveBorderColor = value;
                pBox.Invalidate();
            }
        }

        private Color _InactiveBorderColor;
        public Color InactiveBorderColor
        {
            get
            { return _InactiveBorderColor; }
            set
            {
                _InactiveBorderColor = value;
                pBox.Invalidate();
            }
        }


        #endregion text color


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


        #region Frame type

        // "NoBorder":
        // "FrameThin":
        // "Frame1":
        // "Frame2":
        // "Frame3":
        // "Frame4":
        // "Frame5":
        // "Shadow":
        // "Neon":
        private string _frametype = "Frame1";
        public string FrameType
        {
            get { return _frametype; }
            set
            {
                _frametype = value;

                switch (_frametype)
                {
                    case "NoBorder":
                        _borderthick = 0;
                        break;
                    case "FrameThin":
                        _borderthick = 0;
                        break;
                    case "Frame1":
                        _borderthick = 1;
                        break;
                    case "Frame2":
                        _borderthick = 2;
                        break;
                    case "Frame3":
                        _borderthick = 3;
                        break;
                    case "Frame4":
                        _borderthick = 4;
                        break;
                    case "Frame5":
                        _borderthick = 5;
                        break;
                    case "Shadow":
                        _borderthick = 1;
                        break;
                    case "Neon":
                        _borderthick = 1;
                        break;
                    default:
                        _borderthick = 1;
                        break;
                }
                pBox?.Invalidate();
            }
        }

        private int _borderthick = 1;
        public int BorderThick
        {
            get { return _borderthick; }
            set
            {
                try
                {
                    _borderthick = value;
                    pBox?.Invalidate();
                }
                catch (Exception e)
                {
                    Console.Write("Error: " + e.Message);
                }
            }
        }

        #endregion Frame type


        // Background color
        private int _bpm;
        private Color _BgColor;
        public Color BgColor
        {
            get
            { return _BgColor; }
            set
            {
                _BgColor = value;
                if (_optionbackground == "SolidColor")
                {
                    pBox.BackColor = _BgColor;
                    pBox.Invalidate();
                }
            }
        }


        #region gradient

        private Color _Grad0Color;
        public Color Grad0Color
        {
            get { return _Grad0Color; }
            set
            {
                _Grad0Color = value;
                pBox.Invalidate();
            }
        }
        private Color _Grad1Color;
        public Color Grad1Color
        {
            get { return _Grad1Color; }
            set
            {
                _Grad1Color = value;
                pBox.Invalidate();
            }
        }
        private Color _Rhythm0Color;
        public Color Rhythm0Color
        {
            get { return _Rhythm0Color; }
            set
            {
                _Rhythm0Color = value;
                pBox.BackColor = _Rhythm0Color;
                ResetSize();
                pBox.Invalidate();
            }
        }
        private Color _Rhythm1Color;
        public Color Rhythm1Color
        {
            get { return _Rhythm1Color; }
            set
            {
                _Rhythm1Color = value;
                ResetSize();
                pBox.Invalidate();
            }
        }


        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set { _beatDuration = value; }
        }

        #endregion gradient


        private bool _bTextBackGround = false;
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set 
            {                 
                _bTextBackGround = value;
                pBox.Invalidate();                
            }
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
                        _timerGradient.Stop();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        pBox.BackColor = _BgColor;
                        pBox.Invalidate();
                        break;

                    case "Gradient":
                        m_Cancel = true;
                        Terminate();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        _timerGradient.Start();
                        pBox.Invalidate();
                        break;

                    case "Rhythm":
                        m_Cancel = true;
                        Terminate();
                        _timerGradient.Start();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        ResetSize();
                        pBox.BackColor = _Rhythm0Color;
                        pBox.Invalidate();
                        break;

                    case "Transparent":
                        m_Cancel = true;
                        Terminate();
                        _timerGradient.Stop();
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


        #region Font

        private Font _karaokeFont;
        [Description("The font of the component")]
        public Font KaraokeFont
        {
            get { return _karaokeFont; }
            set { 
                _karaokeFont = value; 
                pBox.Invalidate();
            }
        }

        #endregion Font
      
     
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

            Beat = 200; // Default speed for rhythm animation
            
            _timerGradient.Interval = 60; // 60 ms
            _timerGradient.Tick += new EventHandler(_timerGradient_Tick);

            /*
            this.SetStyle(
                 System.Windows.Forms.ControlStyles.UserPaint |
                 System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                 System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                 true);                        
            */
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);


            SetDefaultValues();
            Init();                       
        }


        #region Timer gradient
        private void _timerGradient_Tick(object sender, EventArgs e)
        {
            switch (_optionbackground)
            {
                case "Gradient":
                    // For diagonal gradients, we can use the angle property to set the gradient direction
                    _angle = (_angle + 1) % 360; // Increment the angle by 1 degree, wrapping around if it exceeds 360 degrees
                    pBox.Invalidate(); // Force the panel to redraw with the new gradient
                    break;

                case "Rhythm":
                    // For radial gradients, we don't use the angle, but we can still animate the size of the ellipse
                    if (W > speed) W -= speed; // Minor the width of the client rectangle at each tick with the speed value
                    if (H > speed) H -= speed; // Minor the height of the client rectangle at each tick with the speed value 


                    break;
            }
            Invalidate(); // Force the panel to redraw with the new gradient
        }

        private void ResetSize()
        {
            // Reset the width and height to the current client rectangle size
            W = ClientRectangle.Width / 2;
            H = ClientRectangle.Height / 2;
        }

        #endregion Timer gradient


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
        private float MeasureString(string fragment, float femSize)
        {
            float ret = 0;
            if (fragment != "")
            {
                using (Graphics g = pBox.CreateGraphics())
                {
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    g.PageUnit = GraphicsUnit.Pixel;

                    m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);
                    SizeF sz = g.MeasureString(fragment, m_font, new Point(0, 0), sf);
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
        /// Measure the length of line "curline"
        /// </summary>
        /// <param name="curline"></param>
        /// <returns></returns>
        private float MeasureLine(int curline)
        {
            float Sum = 0;
            
            // Calculate the lengh of a line
            for (int i = 0; i < Lines[curline].Length; i++)
            {                
                Sum += MeasureString(Lines[curline][i], _karaokeFont.Size);
            }

            return Sum;
        }

        #endregion measures


        #region Control Load Resize paint

        /// <summary>
        /// Resize control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KaraokeEffect_Resize(object sender, EventArgs e)
        {

            if (_optionbackground == "Rhythm")
            {
                // Reset the width and height to the current client rectangle size
                AdjustSpeed();

                // Adapt speed to the new size               
                double hypo = Math.Sqrt(ClientSize.Width * ClientSize.Width + ClientSize.Height * ClientSize.Height);

                if (_bpm > 0 && hypo > 0)
                {
                    ResetSize();
                }

                pBox.Invalidate(); // Invalidate the panel to force a redraw with the new size
            }

            // Increase _steppercent if Width increase
            if (this.ParentForm != null && this.ParentForm.WindowState != FormWindowState.Minimized)
            {
                AjustText(_biggestLine);
                pBox.Invalidate();
            }
        }

        /// <summary>
        /// Paint control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pBox_Paint(object sender, PaintEventArgs e)
        {
            // Antialiasing      
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            #region draw background image

            // Create a GraphicsPath to define the area to fill
            GraphicsPath gp;

            switch (_optionbackground)
            {
                case "Diaporama":
                   
                    if (pictures.Length == 1)
                    {
                        if (m_CurrentImage != null)
                        {                           
                            try
                            {
                                m_DisplayRectangle = GetRectangleForSizeMode(m_CurrentImage.Width, m_CurrentImage.Height);
                                e.Graphics.DrawImage(m_CurrentImage, m_DisplayRectangle, 0, 0, m_CurrentImage.Width, m_CurrentImage.Height, GraphicsUnit.Pixel);
                            }
                            catch (Exception dr)
                            {
                                Console.Write("Error drawing image: " + dr.Message);
                            }
                        }
                    }
                    else
                    {
                        if (mImg1 == null || mImg2 == null)
                            e.Graphics.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, 0, this.Width, this.Height));
                        else
                        {
                            //Rectangle rc = new Rectangle(0, 0, this.Width, this.Height);
                            ColorMatrix cm = new ColorMatrix();
                            ImageAttributes ia = new ImageAttributes();
                            cm.Matrix33 = mBlend;
                            ia.SetColorMatrix(cm);

                            Rectangle rc = GetRectangleForSizeMode(mImg2.Width, mImg2.Height);
                            e.Graphics.DrawImage(mImg2, rc, 0, 0, mImg2.Width, mImg2.Height, GraphicsUnit.Pixel, ia);
                            cm.Matrix33 = 1F - mBlend;
                            ia.SetColorMatrix(cm);

                            rc = GetRectangleForSizeMode(mImg1.Width, mImg1.Height);
                            e.Graphics.DrawImage(mImg1, rc, 0, 0, mImg1.Width, mImg1.Height, GraphicsUnit.Pixel, ia);
                        }
                    }
                    break;

                case "Gradient":
                    // Draw gradient background
                    // Create a GraphicsPath to define the area to fill
                    gp = new GraphicsPath();
                    gp.AddRectangle(ClientRectangle);
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    e.Graphics.FillPath(new LinearGradientBrush(ClientRectangle, _Grad0Color, _Grad1Color, _angle), gp);
                    gp.Dispose(); // Dispose the GraphicsPath to free resources
                    break;

                case "Rhythm":
                    int w = ClientRectangle.Width / 2;
                    int h = ClientRectangle.Height / 2;
                    int d = Math.Min(2 * W, 2 * H);

                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    // Radial gradients are handled differently, so we won't set an angle here                    
                    if (_beatNumber != 1)
                    {
                        //RectangleF rect = new RectangleF((ClientRectangle.Width - W) / 2, (ClientRectangle.Height - H) / 2, W, H);
                        RectangleF rect = new RectangleF((ClientRectangle.Width - d) / 2, (ClientRectangle.Height - d) / 2, d, d);
                        gp = new GraphicsPath();
                        gp.AddEllipse(rect);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = _Rhythm1Color; // Center color of the radial gradient
                            pgb.SurroundColors = new Color[] { _Rhythm0Color }; // Surrounding color of the radial gradient
                            e.Graphics.FillPath(pgb, gp); // Fill the path with the radial gradient
                            pgb.Dispose(); // Dispose the PathGradientBrush to free resources                        
                        }
                        gp.Dispose(); // Dispose the GraphicsPath to free resources
                    }
                    else
                    {
                        RectangleF rect1 = new RectangleF((w - W) / 2, (h - H) / 2, W, H); // Top-left corner
                        RectangleF rect2 = new RectangleF(w + (w - W) / 2, (h - H) / 2, W, H); // Top-right corner                            
                        RectangleF rect3 = new RectangleF((w - W) / 2, h + (h - H) / 2, W, H); // Bottom-left corner                                                                                    
                        RectangleF rect4 = new RectangleF(w + (w - W) / 2, h + (h - H) / 2, W, H); // Bottom-right corner   

                        gp = new GraphicsPath();
                        gp.AddEllipse(rect1);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = _Rhythm1Color; // Center color of the radial gradient
                            pgb.SurroundColors = new Color[] { _Rhythm0Color }; // Surrounding color of the radial gradient
                            e.Graphics.FillPath(pgb, gp); // Fill the path with the radial gradient
                            pgb.Dispose(); // Dispose the PathGradientBrush to free resources                        
                        }
                        gp.Dispose(); // Dispose the GraphicsPath to free resources

                        gp = new GraphicsPath();
                        gp.AddEllipse(rect2);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = _Rhythm1Color; // Center color of the radial gradient
                            pgb.SurroundColors = new Color[] { _Rhythm0Color }; // Surrounding color of the radial gradient
                            e.Graphics.FillPath(pgb, gp); // Fill the path with the radial gradient
                            pgb.Dispose(); // Dispose the PathGradientBrush to free resources                        
                        }
                        gp.Dispose(); // Dispose the GraphicsPath to free resources

                        gp = new GraphicsPath();
                        gp.AddEllipse(rect3);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = _Rhythm1Color; // Center color of the radial gradient
                            pgb.SurroundColors = new Color[] { _Rhythm0Color }; // Surrounding color of the radial gradient
                            e.Graphics.FillPath(pgb, gp); // Fill the path with the radial gradient
                            pgb.Dispose(); // Dispose the PathGradientBrush to free resources                        
                        }
                        gp.Dispose(); // Dispose the GraphicsPath to free resources

                        gp = new GraphicsPath();
                        gp.AddEllipse(rect4);
                        using (PathGradientBrush pgb = new PathGradientBrush(gp))
                        {
                            pgb.CenterColor = _Rhythm1Color; // Center color of the radial gradient
                            pgb.SurroundColors = new Color[] { _Rhythm0Color }; // Surrounding color of the radial gradient
                            e.Graphics.FillPath(pgb, gp); // Fill the path with the radial gradient
                            pgb.Dispose(); // Dispose the PathGradientBrush to free resources                        
                        }
                        gp.Dispose(); // Dispose the GraphicsPath to free resources                                       
                    }
                    break;
            }
            #endregion draw background image


            #region draw text

            switch (FrameType)
            {
                case "NoBorder":                    
                case "FrameThin":
                case "Frame1":                    
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5": 
                    DrawTextWithBorder(e);
                    break;

                case "Shadow":
                    DrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    DrawTextWithNeon(e);
                    break; ;

                default:
                    DrawTextWithBorder(e);
                    break;
            }
            //DrawLyrics(e);
           
            #endregion draw text
        }


        /// <summary>
        /// Return rectangle for image
        /// </summary>
        /// <param name="imgWidth"></param>
        /// <param name="imgHeight"></param>
        /// <returns></returns>
        private Rectangle GetRectangleForSizeMode(int imgWidth, int imgHeight)
        {
            int x;
            int y;

            switch (_sizemode)
            {
                case PictureBoxSizeMode.Normal:
                    // coin superieur gauche
                    return new Rectangle(0, 0, imgWidth, imgHeight);

                case PictureBoxSizeMode.StretchImage:
                    //  l'image est étirée ou réduite pour s'ajuster à PictureBox.
                    return new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);


                case PictureBoxSizeMode.CenterImage:
                    x = (this.ClientSize.Width - imgWidth) / 2;
                    y = (this.ClientSize.Height - imgHeight) / 2;
                    return new Rectangle(x, y, imgWidth, imgHeight);


                case PictureBoxSizeMode.AutoSize:
                    x = (this.ClientSize.Width - imgWidth) / 2;
                    y = (this.ClientSize.Height - imgHeight) / 2;
                    return new Rectangle(x, y, imgWidth, imgHeight);

                case PictureBoxSizeMode.Zoom:
                    float zoomFactor = (float)ClientSize.Height / (float)imgHeight;
                    x = (this.ClientSize.Width - (int)(imgWidth * zoomFactor)) / 2;
                    y = 0;
                    return new Rectangle(x, 0, (int)(imgWidth * zoomFactor), this.ClientSize.Height);

                default:
                    return new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            }
        }

        /*
        /// <summary>
        /// Draw lyrics
        /// </summary>
        /// <param name="e"></param>
        private void DrawLyrics(PaintEventArgs e)
        {            
            // path made for the portion of a line of lyrics
            // in order to draw the ActiveBorder color only on it
            var pathFragment = new GraphicsPath();            
            SolidBrush colorBrush;
            Pen penActiveBorder = new Pen(_ActiveBorderColor, _borderthick);    // pen for active border color
            Pen penInactiveBorder = new Pen(_InactiveBorderColor, _borderthick); // pen for inactive border color

            // Center text vertically
            int y0 = VCenterText();
            int x0 = 0;

            int Wbg;
            RectangleF Rbg;
                                   
            // =============================================
            // WHITE
            // 1. Color the current line in whithe
            // and than color portions above the white in green (already played) and red (currently played) 
            // =============================================
            // Create a graphical path
            var path = new GraphicsPath();

            // Add the full text line to the graphical path            
            if (_FirstLineToShow < Texts.Count())
            {
                x0 = HCenterText(Texts[_FirstLineToShow]);      // Center horizontally

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[_FirstLineToShow]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }
                #endregion

                // full line in GraphicsPath path
                path.AddString(Texts[_FirstLineToShow], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);

                // part of line (sung part) in GraphicsPath pathFragment  
                pathFragment.AddString(current_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);
            }
           
            // Fill GraphicsPath path in white => full text is white
            colorBrush = new SolidBrush(_InactiveColor);
            e.Graphics.FillPath(colorBrush, path);

            // ======================================================
            // GREEN
            // 2. Color in GREEN (ActiveColor) the syllabes before current syllabe
            // ======================================================
            // Create a region from the graphical path
            Region r = new Region(path);
            // Create a retangle of the graphical path
            RectangleF rect = r.GetBounds(e.Graphics);

            RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRectBefore);

            colorBrush = new SolidBrush(_ActiveColor);
            e.Graphics.FillRegion(colorBrush, r);


            // ======================================================
            // RED
            // 3. Color in RED (HighlightColor) the current syllabe
            // ======================================================
            r = new Region(path);

            // Create another rectangle shorter than the 1st one (percent of the first)                       
            RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRect);

            // Fill updated region in red => percent portion of text is red
            colorBrush = new SolidBrush(_HighlightColor);
            e.Graphics.FillRegion(colorBrush, r);


            // text outline
            if (_borderthick > 0)
            {
                e.Graphics.DrawPath(penInactiveBorder, path);                       // Draw the entire line using the inactive border color
                e.Graphics.DrawPath(penActiveBorder, pathFragment);                 // Next, draw the current fragment of line on top using the active border color
            }

            // Rest of line in white
           
            path.Dispose();


            // ======================================================================================================
            // NEXT LINES
            // 4. Draw and color (InactiveColor) all lines from _linedeb + 1 to _linefin in white
            // We want to display only a few number of lines (variable _nbLyricsLines = number of lines to display)  
            // linedeb which is the current line is displayed in the previous paragraph
            // ======================================================================================================
            path = new GraphicsPath();                        

            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                if (i < Texts.Count())
                {
                    x0 = HCenterText(Texts[i]);     // Center text horizontally

                    #region background of syllabe                              
                    if (_bTextBackGround)
                    {
                        Wbg = (int)(1.04 * LinesLengths[i]);
                        // Black background to make text more visible
                        Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0) + (i - _FirstLineToShow) * _lineHeight, Wbg, _lineHeight);
                        // background
                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                    }
                    #endregion

                    // Draw lines of lyrics
                    path.AddString(Texts[i], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0 + (i - _FirstLineToShow) * _lineHeight), sf);
                }
            }

            // _InactiveColor is the color for text not yet sung (typically white)
            colorBrush = new SolidBrush(_InactiveColor);
            e.Graphics.FillPath(colorBrush, path);

            // text outline
            if (_borderthick > 0)
                e.Graphics.DrawPath(penInactiveBorder, path);            

            // Clean all
            colorBrush.Dispose();
            penActiveBorder.Dispose();
            penInactiveBorder.Dispose();
            r.Dispose();
            path.Dispose();            
            pathFragment.Dispose();
        }
        */


        private void DrawTextWithBorder(PaintEventArgs e)
        {
            // path made for the portion of a line of lyrics
            // in order to draw the ActiveBorder color only on it
            var pathFragment = new GraphicsPath();
            SolidBrush colorBrush;
            Pen penActiveBorder = new Pen(_ActiveBorderColor, _borderthick);    // pen for active border color
            Pen penInactiveBorder = new Pen(_InactiveBorderColor, _borderthick); // pen for inactive border color

            // Center text vertically
            int y0 = VCenterText();
            int x0 = 0;

            int Wbg;
            RectangleF Rbg;

            // =============================================
            // WHITE
            // 1. Color the current line in whithe
            // and than color portions above the white in green (already played) and red (currently played) 
            // =============================================
            // Create a graphical path
            var path = new GraphicsPath();

            // Add the full text line to the graphical path            
            if (_FirstLineToShow < Texts.Count())
            {
                x0 = HCenterText(Texts[_FirstLineToShow]);      // Center horizontally

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[_FirstLineToShow]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }
                #endregion

                // full line in GraphicsPath path
                path.AddString(Texts[_FirstLineToShow], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);

                // part of line (sung part) in GraphicsPath pathFragment  
                pathFragment.AddString(current_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);
            }

            // Fill GraphicsPath path in white => full text is white
            colorBrush = new SolidBrush(_InactiveColor);
            e.Graphics.FillPath(colorBrush, path);

            // ======================================================
            // GREEN
            // 2. Color in GREEN (ActiveColor) the syllabes before current syllabe
            // ======================================================
            // Create a region from the graphical path
            Region r = new Region(path);
            // Create a retangle of the graphical path
            RectangleF rect = r.GetBounds(e.Graphics);

            RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRectBefore);

            colorBrush = new SolidBrush(_ActiveColor);
            e.Graphics.FillRegion(colorBrush, r);


            // ======================================================
            // RED
            // 3. Color in RED (HighlightColor) the current syllabe
            // ======================================================
            r = new Region(path);

            // Create another rectangle shorter than the 1st one (percent of the first)                       
            RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRect);

            // Fill updated region in red => percent portion of text is red
            colorBrush = new SolidBrush(_HighlightColor);
            e.Graphics.FillRegion(colorBrush, r);


            // text outline
            if (_borderthick > 0)
            {
                e.Graphics.DrawPath(penInactiveBorder, path);                       // Draw the entire line using the inactive border color
                e.Graphics.DrawPath(penActiveBorder, pathFragment);                 // Next, draw the current fragment of line on top using the active border color
            }


            // Rest of line in white



            path.Dispose();


            // ======================================================================================================
            // NEXT LINES
            // 4. Draw and color (InactiveColor) all lines from _linedeb + 1 to _linefin in white
            // We want to display only a few number of lines (variable _nbLyricsLines = number of lines to display)  
            // linedeb which is the current line is displayed in the previous paragraph
            // ======================================================================================================
            path = new GraphicsPath();


            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                if (i < Texts.Count())
                {
                    x0 = HCenterText(Texts[i]);     // Center text horizontally

                    #region background of syllabe                              
                    if (_bTextBackGround)
                    {
                        Wbg = (int)(1.04 * LinesLengths[i]);
                        // Black background to make text more visible
                        Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0) + (i - _FirstLineToShow) * _lineHeight, Wbg, _lineHeight);
                        // background
                        e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                    }
                    #endregion

                    // Draw lines of lyrics
                    path.AddString(Texts[i], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0 + (i - _FirstLineToShow) * _lineHeight), sf);
                }
            }

            // _InactiveColor is the color for text not yet sung (typically white)
            colorBrush = new SolidBrush(_InactiveColor);
            e.Graphics.FillPath(colorBrush, path);

            // text outline
            if (_borderthick > 0)
                e.Graphics.DrawPath(penInactiveBorder, path);


            // Clean all
            colorBrush.Dispose();
            penActiveBorder.Dispose();
            penInactiveBorder.Dispose();
            r.Dispose();
            path.Dispose();
            pathFragment.Dispose();
        }

        private void DrawTextWithShadow(PaintEventArgs e)
        {

        }

        private void DrawTextWithNeon(PaintEventArgs e)
        {

            #region declarations

            string line;
            float thick = 2.0f;
            Region r;
            Matrix mx;

            Color HaloColor;
            Brush HaloBrush;

            Brush ActiveColorBrush = new SolidBrush(ActiveColor);
            Brush HighlightColorBrush = new SolidBrush(HighlightColor);
            Brush InactiveColorBrush = new SolidBrush(InactiveColor);
            Brush ActiveColorBorderBrush = new SolidBrush(ActiveBorderColor);
            Brush InactiveColorBorderBrush = new SolidBrush(InactiveBorderColor);
            //Brush brush;
            //Pen pen;
            Pen ActiveBorderPen = new Pen(ActiveColorBorderBrush, thick);
            Pen InactiveBorderPen = new Pen(InactiveColorBorderBrush, thick);
            Pen penHaloColor;

            #endregion declarations

            // path made for the portion of a line of lyrics
            // in order to draw the ActiveBorder color only on it
            var pathFragment = new GraphicsPath();
            //SolidBrush colorBrush;
            Pen penActiveBorder = new Pen(_ActiveBorderColor, thick);    // pen for active border color
            Pen penInactiveBorder = new Pen(_InactiveBorderColor, thick); // pen for inactive border color

            // Center text vertically
            int y0 = VCenterText();
            int x0 = 0;

            int Wbg;
            RectangleF Rbg;

            //Create a bitmap in a fixed ratio to the original drawing area.
            Bitmap bm = new Bitmap(pBox.ClientSize.Width / 5, pBox.ClientSize.Height / 5);
            //Get the graphics object for the image. 
            Graphics gimg = Graphics.FromImage(bm);

            // =============================================
            // WHITE
            // 1. Color the current line in whithe
            // and than color portions above the white in green (already played) and red (currently played) 
            // =============================================
            // Create a graphical path
            GraphicsPath pth = new GraphicsPath();

            // Create a Graphics object from e
            Graphics ge = e.Graphics;


            // Add the full text line to the graphical path            
            if (_FirstLineToShow < Texts.Count())
            {
                x0 = HCenterText(Texts[_FirstLineToShow]);      // Center horizontally

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[_FirstLineToShow]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0), Wbg, _lineHeight);
                    // background
                    ge.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }
                #endregion

                // full line in GraphicsPath path
                pth.AddString(Texts[_FirstLineToShow], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);

                // part of line (sung part) in GraphicsPath pathFragment  
                pathFragment.AddString(current_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0), sf);
          
            }
                               

            // Fill GraphicsPath path in white => full text is white            
            ge.FillPath(InactiveColorBrush, pth);


            // ======================================================
            // GREEN
            // 2. Color in GREEN (ActiveColor) the syllabes before current syllabe
            // ======================================================
            #region green
            // Create a region from the graphical path
            r = new Region(pth);
            // Create a retangle of the graphical path
            RectangleF rect = r.GetBounds(ge);

            RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRectBefore);

            // Fill updated region in green
            ge.FillRegion(ActiveColorBrush, r);
            #endregion green

            // ======================================================
            // RED
            // 3. Color in RED (HighlightColor) the current syllabe
            // ======================================================
            #region red
            r = new Region(pth);

            // Create another rectangle shorter than the 1st one (percent of the first)                       
            RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);

            // update region on the intersection between region and 2nd rectangle
            r.Intersect(intersectRect);

            // Fill updated region in red => percent portion of text is red            
            ge.FillRegion(HighlightColorBrush, r);
            #endregion red

            // Outline text
            if (thick > 0)
            {
                ge.DrawPath(penInactiveBorder, pth);                       // Draw the entire line using the inactive border color
                ge.DrawPath(penActiveBorder, pathFragment);                 // Next, draw the current fragment of line on top using the active border color
            }

            pth.Dispose();


            // ======================================================================================================
            // NEXT LINES
            // 4. Draw and color (InactiveColor) all lines from _linedeb + 1 to _linefin in white
            // We want to display only a few number of lines (variable _nbLyricsLines = number of lines to display)  
            // linedeb which is the current line is displayed in the previous paragraph
            // ======================================================================================================
            pth = new GraphicsPath();

            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                if (i < Texts.Count())
                {
                    x0 = HCenterText(Texts[i]);     // Center text horizontally

                    #region background of syllabe                              
                    if (_bTextBackGround)
                    {
                        Wbg = (int)(1.04 * LinesLengths[i]);
                        // Black background to make text more visible
                        Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y0) + (i - _FirstLineToShow) * _lineHeight, Wbg, _lineHeight);
                        // background
                        ge.FillRectangle(new SolidBrush(Color.Black), Rbg);
                    }
                    #endregion

                    // Draw lines of lyrics
                    pth.AddString(Texts[i], _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y0 + (i - _FirstLineToShow) * _lineHeight), sf);

                    #region Neon effect

                    //Create a matrix that shrinks the drawing output by the fixed ratio. 
                    mx = new Matrix(1.0f / 5, 0, 0, 1.0f / 5, -(1.0f / 5), -(1.0f / 5));

                    //Choose an appropriate smoothing mode for the halo. 
                    gimg.SmoothingMode = SmoothingMode.AntiAlias;

                    //Transform the graphics object so that the same half may be used for both halo and text output. 
                    gimg.Transform = mx;

                    //Using a suitable pen...
                    HaloColor = InactiveBorderColor;
                    HaloBrush = new SolidBrush(HaloColor);                    

                    penHaloColor = new Pen(HaloColor, 3);

                    //Draw around the outline of the path
                    gimg.DrawPath(penHaloColor, pth);

                    //and then fill in for good measure. 
                    gimg.FillPath(HaloBrush, pth);

                    //We no longer need this graphics object
                    //g.Dispose();

                    //setup the smoothing mode for path drawing
                    ge.SmoothingMode = SmoothingMode.AntiAlias;

                    //and the interpolation mode for the expansion of the halo bitmap
                    ge.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    //expand the halo making the edges nice and fuzzy. 
                    ge.DrawImage(bm, pBox.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);

                    #endregion Neon effect

                    
                    // Draw the text
                    // _InactiveColor is the color for text not yet sung (typically white)
                    ge.FillPath(InactiveColorBrush, pth);

                    // Outline the text
                    if (thick > 0)
                        ge.DrawPath(penInactiveBorder, pth);
                }
            }
            
            // Clean all            
            ActiveColorBrush.Dispose();
            HighlightColorBrush.Dispose();
            InactiveColorBrush.Dispose();
            
            penActiveBorder.Dispose();
            penInactiveBorder.Dispose();
            
            r?.Dispose();
            pth?.Dispose();
            pathFragment?.Dispose();
            gimg?.Dispose();            
        }

        /// <summary>
        /// Apply the beat effect.
        /// </summary>
        public void OnBeat(int beat, int bpm)
        {
            if (bpm > 0 && bpm != _bpm)
            {
                _bpm = bpm;
                AdjustSpeed();
            }

            _beatNumber = beat;

            BeatEffect(beat);
        }


        /// <summary>
        /// Applies a visual effect in response to a beat event.
        /// </summary>
        /// <remarks>This method is a placeholder for implementing beat-based visual effects.  Depending
        /// on the gradient style, different effects can be applied, such as  resetting dimensions or altering colors.
        /// Currently, it resets the width and  height for radial gradients and provides a framework for future
        /// extensions.</remarks>
        private void BeatEffect(int beat)
        {
            switch (_optionbackground)
            {
                case "Gradient":
                    // For diagonal gradients, you can implement a different effect if needed
                    // For example, you could change the angle or colors on each beat
                    // Change the colors of the radial gradient on each beat
                    //Color temp = _color0;
                    //_color0 = _color1;
                    //_color1 = temp;
                    break;
                case "Rhythm":
                    // Radial gradients can have a different effect, such as changing colors or sizes
                    // For diagonal gradients, you can implement a different effect if needed
                    // W & H are reset to their maximum at each beat
                    //if (beat == 1) ResetSize(); // Reset the width and height to the original size
                    ResetSize(); // Reset the width and height to the original size
                    break;
            }
        }

        private void AdjustSpeed()
        {
            if (_bpm > 0)
            {
                double hypo = Math.Sqrt(ClientSize.Width * ClientSize.Width + ClientSize.Height * ClientSize.Height);
                if (hypo <= 0) return;
                //2600.0F                
                //speed = (int)(_bpm * hypo / 5200.0F); // Speed depends on the BPM and the size of the screen
                speed = (int)(_bpm * hypo / 7000.0F); // Speed depends on the BPM and the size of the screen
                //speed = (int)(_bpm * hypo / 10400.0F); // Speed depends on the BPM and the size of the screen
                Console.WriteLine("BPM changed to: " + _bpm + " - Speed: " + speed);
            }
        }

        #endregion Control Load Resize paint


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
        /// Retrieve nextindex of current syllabe in the current line
        /// </summary>
        /// <returns></returns>       
        private int GetNextIndex(int pos)
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

            current_fragment = string.Empty;

            for (int i = 0; i < idx; i++)
            {
                if (i < Lines[_line].Count())
                {
                    res += MeasureString(Lines[_line][i], _karaokeFont.Size);
                    current_fragment += Lines[_line][i];
                }
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
        /// Center text vertically, according to number of lines to display and line height
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
            nextindex = 0;
            lastindex = 0;
            _lasttime = 0;
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
            nextindex = 0;
            lastindex = 0;
            _lasttime = 0;
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
            // Search nextindex of lyric to play
            nextindex = GetNextIndex(pos);

            // Length of partial line
            CurLength = GetCurLength(nextindex);

            // New word to highlight
            // Warning: in case of full lines, nextindex is allways the same and not different than lastIndex
            if (nextindex != lastindex || _line != _lastLine)
            {                                
                // Line changed
                if (_line !=  _lastLine)
                {
                    _lastLine = _line;
                    percent = 0;
                    lastpercent = 0;
                    nextindex = 0;
                    lastindex = 0;
                    lastCurLength = 0;
                    CurLength = 0;

                    _lasttime = _nexttime;
                }
                
                _FirstLineToShow = _line;
                _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _lines, _nbLyricsLines);

                //Console.WriteLine("nexttime before: " + _nexttime);
                if (nextindex < Times[_line].Count())
                {
                    _nexttime = Times[_line][nextindex];                                      
                }
                //Console.WriteLine("nexttime after: " + _nexttime);
                //Console.WriteLine("lasttime: " + _lasttime);


                //Console.WriteLine("Line : " + _line + " - nextindex : " + nextindex + " - nexttime : " + _nexttime + " - lasttime " + _lasttime);


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
                    //_steppercent = d / 3000;

                    if (d > 0 && (_nexttime - _lasttime) > 0)
                    {
                        //Console.WriteLine("_nexttime = " + _nexttime + " - _lasttime = " + _lasttime);

                        //_steppercent = (_nexttime - _lasttime) / (d * _timerintervall);
                        //_steppercent = (float)(_nexttime - _lasttime) / 1000000*(float)(_timerintervall);

                        _steppercent = (_nexttime - _lasttime) / (d*(float)_timerintervall);

                        //Console.WriteLine("timerintervall = " + _timerintervall);
                        //Console.WriteLine("_steppercent = " + _steppercent);
                    }
                    
                }
                
                lastCurLength = CurLength;
                lastindex = nextindex;
                
                pBox.Invalidate();                
            }
            else
            {
                // if same nextindex: progressive increase of percent
                percent += _steppercent;
                //Console.WriteLine("percent = " + percent);

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

            // new slideshow

            count = 0;
            //mBlend = 0.0F;
            pictures = new Bitmap[bgFiles.Length];
            for (int i = 0; i < bgFiles.Length; ++i)
            {
                pictures[i] = new Bitmap(bgFiles[i]);
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
                //UpdateTimerEnable(false);

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
                        //C = m_ImageFilePaths.Count;
                        C = pictures.Length;
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
                            /*                            
                            // Slideshow => backgroundworker                            
                            m_Cancel = false;
                            // Initialize backgroundworker
                            InitBackGroundWorker();
                            random = new Random();
                            StartBgW();
                            */

                            InitSlideShow();

                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

        }

        #region SlideShow with timer 

        // New Slideshow
        private void InitSlideShow()
        {
            mBlend = 0;
            count = 0;

            timerChangeImage?.Dispose();
            timerChangeImage = new System.Timers.Timer();
            timerChangeImage.Interval = _freqdirslideshow * 1000;
            timerChangeImage.Elapsed += (sender, e) => OnTimerChangeImage();

            timerTransition?.Dispose();
            timerTransition = new System.Timers.Timer();
            timerTransition.Interval = 50;
            timerTransition.Elapsed += (sender, e) => OnTimerTransition();

            try
            {
                Image1 = pictures[count];
                Image2 = pictures[++count];
            }
            catch
            {

            }
            timerTransition.Enabled = false;
            timerChangeImage.Enabled = true;
        }

        private void OnTimerTransition()
        {
            mBlend += mDir * 0.02F;

            if (mBlend > 1)
            {
                // When mBlend is greater than 1, we change the images
                // and stop the timer "timerTransition" to prevent a new change before time elapse of "timerChangeImage"
                mBlend = 0.0F;

                if ((count + 1) < pictures.Length)
                {
                    Image1 = pictures[count];
                    Image2 = pictures[++count];
                }
                else
                {
                    Image1 = pictures[count];
                    Image2 = pictures[0];
                    count = 0;
                }

                timerTransition.Enabled = false;
            }

            m_Blend = mBlend;
        }

        private void OnTimerChangeImage()
        {
            timerTransition.Enabled = true;
        }


        #endregion SlideShow with timer 


        /*

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
      
        */


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

            timerChangeImage?.Stop();
            timerTransition?.Stop();

            //if (backgroundWorkerSlideShow != null)
            //{
            //    backgroundWorkerSlideShow.CancelAsync();
            //}
        }


        #endregion SlideShow


    }
}
