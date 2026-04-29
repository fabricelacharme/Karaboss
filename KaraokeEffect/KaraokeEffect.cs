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
using kar;
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
using System.Windows.Forms;


namespace keffect
{   
    
    public delegate void DoubleClickEventHandler(object sender, EventArgs e);
    
    public partial class KaraokeEffect : UserControl, IMessageFilter
    {
        #region Events

        public new event DoubleClickEventHandler DoubleClick;

        #endregion Events


        #region MP3

        // Duration in seconds (org Bass)
        private double _duration;
        public double Duration 
        { get { return _duration; } 
            set { _duration = value; } }

        private int _bitrate;   // genre 192
        public int BitRate { get { return _bitrate; } set { _bitrate = value; } }

        // Frequency
        private float _frequency;
        public float Frequency { get { return _frequency; } set { _frequency = value; } }

        #endregion MP3
        

        #region Instrumentals

        private DateTime _endTime;
        private DateTime _startTime;
        
        private double PlayerPositionMilliseconds;
        private double TargetPositionMilliseconds;

        private bool bInstrumentalStarted = false;
        private int SecondsBeforeSinging = 0;
        private bool bCountDown = false;
        private int _DelayBeforeEndOfInstrumental = 4000; // Delay to draw lines before the end of an instrumental: 4 sec
        private int _MinimumInstrumentalDuration = 5000;  // The minimum duration between two consecutive vocal phrases that mark an instrumental interlude : 5 sec
        private int LastLineOfInformationPosition = 0;


        #endregion Instrumentals


        #region Others

        private float percent = 0;
        private float lastpercent = 0;
        
        private long _timerintervall = 50;      // Intervall of timer of frmMp3Player
        public long timerIntervall
        {
            get { return _timerintervall; }
            set
            {

                if (value >= 10)
                    _timerintervall = value;
            }
        }

        private float[] LinesLengths;

        private int nextindex = 0;
        private int lastindex = 0;
        private float CurLength;
        private float lastCurLength;

        double _nexttime;
        double _lasttime;

        
        private int _FirstLineToShow = 0;
        private int _LastLineToShow = 0;

        private int _lastLine = -1;               
        private int _lineHeight = 0;
        private int _linesHeight = 0;
        private string _biggestLine = string.Empty;


        private string active_fragment = string.Empty;
        private float active_fragment_length = 0;
        private string highlight_fragment = string.Empty;
        private float highlight_fragment_length = 0;
        private string inactive_fragment = string.Empty;
        private float inactive_fragment_length = 0;


        private int _beatNumber = 1;

        #endregion Others


        #region Is used for settings

        private bool _bIsSettings = false;
        [Description("When true, KaraokeEffect is used in a settings window")]
        public bool bIsSettings
        {
            get { return _bIsSettings; }
            set 
            { 
                _bIsSettings = value; 
            }
        }

        #endregion Is used for settings


        #region Karaoke lyrics

        private kLyrics _kLyrics;
        public kLyrics KLyrics
        {
            get { return _kLyrics; }
            set
            {
                if (value == null) return;
                if (value.Lines == null) return;
                if (value.Lines.Count == 0) return;
                _kLyrics = value;
                if(_kLyrics != null && _kLyrics.Lines.Count > 0)
                    Init();
            }
        }

        #endregion Karaoke lyrics


        #region Karaoke display types

        private kar.KaraokeDisplayTypes _karaokeDisplayType = KaraokeDisplayTypes.FixedLines;
        public kar.KaraokeDisplayTypes KaraokeDisplayType
        {
            get { return _karaokeDisplayType; }
            set
            {
                if (value != _karaokeDisplayType)
                {
                    _karaokeDisplayType = value;

                    if (_kLyrics.Lines.Count > 0)
                    {
                        if (_bIsSettings)                        
                            Init();
                        pBox?.Invalidate();
                        AjustText(_biggestLine); // pourquoi ? mystère. Mais ça marche

                    }
                }
            }
        }

        #endregion Karaoke display types


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


        #region Transition effect

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
        
        // Array of bitmaps (images as backgound image)
        private Bitmap[] m_BitmapsArray;

        #endregion Transition effect       


        #region SlideShow

        // Paths of images
        private string[] m_ImageFilePaths;

        private string DefaultDirSlideShow;
        public Rectangle m_DisplayRectangle { get; set; }
        public Image m_CurrentImage { get; set; }

        /// <summary>
        /// SlideShow frequency
        /// </summary>
        private int _freqdirslideshow = 10;
        public int FreqDirSlideShow
        {
            get { return _freqdirslideshow; }
            set { _freqdirslideshow = value; }
        }



        #endregion SlideShow

        
        #region Text color

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


        #endregion Text color


        #region Text transform

        private int _nbLyricsLinesOrg;
        private int _nbLyricsLines = 3;
        [Description("The number of lines to display")]
        public int nbLyricsLines
        {
            get { return _nbLyricsLines; }
            set
            {
                _nbLyricsLines = value;
                _nbLyricsLinesOrg = value;
                if (bIsSettings)
                    Init();
                pBox.Invalidate();
            }
        }

        private bool _bforceUppercase = false;
        [Description("Force uppercase for lyrics")]
        public bool bforceUppercase
        {
            get { return _bforceUppercase; }
            set 
            {
                if (value != _bforceUppercase)
                {
                    _bforceUppercase = value;
                    if (_bIsSettings)
                        LoadDemoText();
                                      
                }            
            }
        }

        private bool _bshowparagraphs = true;
        [Description("show a blank line between paragraphs")]
        public bool bShowParagraphs
        {
            get { return _bshowparagraphs; }
            set { _bshowparagraphs = value; }
        }


        #endregion Text transform


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
                        _borderthick = 2;
                        break;
                    case "Neon":
                        _borderthick = 2;
                        break;
                    default:
                        _borderthick = 1;
                        break;
                }
                pBox?.Invalidate();


                AjustText(_biggestLine);      // pourquoi ? mystère. Mais ça marche
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
     

        #region Gradient

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
       

        readonly System.Windows.Forms.Timer _timerGradient = new System.Windows.Forms.Timer();

        // Default angle for the gradient
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

        private float _angle = 45.0f;
        public float GradientAngle { get { return _angle; } set { _angle = value; pBox.Invalidate(); } }
                                
        #endregion Gradient


        #region Background

        // Background color
        private int _bpm;
        private Color _BgColor; // = Color.Black;
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

        // Color beside text (background of text)
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
                        Terminate();
                        _timerGradient.Stop();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        pBox.BackColor = _BgColor;
                        pBox.Invalidate();
                        break;

                    case "Gradient":                        
                        Terminate();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        _timerGradient.Start();
                        pBox.Invalidate();
                        break;

                    
                    case "Rhythm":                        
                        Terminate();
                        _timerGradient.Start();
                        pBox.Image = null;
                        m_CurrentImage = null;
                        ResetSize();
                        pBox.BackColor = _Rhythm0Color;
                        pBox.Invalidate();
                        break;

                    case "Transparent":                        
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

        #endregion Background


        #region Image display

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

        #endregion Image display


        #region Lyrics transition effects

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

        #endregion Lyrics transition effects        


        #region Font

        private Font m_font;   // used to measure strings without changing _karaokeFont
        private float emSize = 40;

        private StringFormat sf;


        private Font _karaokeFont;
        [Description("Karaoke font")]
        public Font KaraokeFont
        {
            get { return _karaokeFont; }
            set { 
                _karaokeFont = value; 
                pBox.Invalidate();
            }
        }

        #endregion Font
      
             

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

            _kLyrics = new kLyrics();

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

            if (_kLyrics != null && _kLyrics.Lines.Count > 0) 
                Init();                       
        }

       

        #region Events

        /// <summary>
        /// Double click for Windows state of parent form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pBox_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        #endregion Events


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


        #region Initializations

        private void SetDefaultValues()
        {
            m_ImageFilePaths = new string[] { };
            m_BitmapsArray = new Bitmap[] { };

            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };            
            _karaokeFont = new Font("Comic Sans MS", emSize, FontStyle.Regular, GraphicsUnit.Pixel);
            
            _steppercent = 0.01F;          
            _transitionEffect = TransitionEffects.None;
         }

        public void LoadDemoText()
        {
            List<string> lines = new List<string>();
            lines.Add("Lorem ipsum dolor sit amet,");
            lines.Add("consectetur adipisicing elit,");
            lines.Add("sed do eiusmod tempor incididunt");
            lines.Add("ut labore et dolore magna aliqua.");
            lines.Add("Ut enim ad minim veniam,");
            lines.Add("quis nostrud exercitation ullamco");
            lines.Add("laboris nisi ut aliquip");
            lines.Add("ex ea commodo consequat.");
            lines.Add("Duis aute irure dolor in reprehenderit");
            lines.Add("in voluptate velit esse cillum dolore");
            lines.Add("eu fugiat nulla pariatur.");

            // Do not use KLyrics but _kLyrics to be able to use the same LoadSong method for demo and real text
            _kLyrics = StoreDemoText(lines, 500);

            // Load song with demo text
            Init();

            // needs LinesLengths to be set => so launch Init() before

            this.SetPos(500);   // 
            this.SetPos(1010);  // after Lorem
            this.SetPos(1510); // after ipsum
            this.SetPos(2010); // after dolor     
            
            pBox.Invalidate();
        }

        /// <summary>
        /// Store demo text        
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        private kLyrics StoreDemoText(List<string> lines, int step, int tcks = 0)
        {
            int ticks = 0;
            Syllable syll;
            kLine kLine = new kLine();
            kLyrics KL = new kLyrics();

            for (int i = 0; i < lines.Count; i++)
            {
                string l = lines[i];
                string[] words = l.Split(new Char[] { ' ' });

                kLine = new kLine();
                for (int j = 0; j < words.Length; j++)
                {
                    if (bforceUppercase)
                        words[j] = words[j].ToUpper();

                    string w = words[j] + " ";
                    //ticks = tcks + (i + 1) * (j + 1) * 10;
                    syll = new Syllable() { Text = w, TicksOn = ticks };
                    ticks += step;

                    kLine.Add(syll);
                }
                KL.Add(kLine);
            }

            return KL;
        }


        /// <summary>
        /// Remove paragra^hs in some cases
        /// </summary>
        /// <param name="kls"></param>
        /// <returns></returns>
        private kLyrics RemoveParagraphs(kLyrics kls)
        {
            kLyrics klsNoParagraphs = new kLyrics();
            kLine line;
            for (int i = 0; i < kls.Lines.Count; i++)
            {
                //if (kls.Lines[i].Syllables.First().CharType != Syllable.CharTypes.ParagraphSep)
                if (! (kls.Lines[i].Syllables.Count == 1 && kls.Lines[i].Syllables.First().Text == string.Empty))
                {
                    line = new kLine();
                    for (int j = 0; j < kls.Lines[i].Syllables.Count; j++)
                    {
                        line.Add(kls.Lines[i].Syllables[j]);
                    }
                    klsNoParagraphs.Add(line);
                }
            }

            return klsNoParagraphs;
        }

        private kLyrics ForceUpperCase(kLyrics kls)
        {
            kLyrics klsNoParagraphs = new kLyrics();
            kLine line;
            for (int i = 0; i < kls.Lines.Count; i++)
            {                
                line = new kLine();
                for (int j = 0; j < kls.Lines[i].Syllables.Count; j++)
                {
                    if (kls.Lines[i].Syllables[j].CharType != Syllable.CharTypes.ParagraphSep)
                        kls.Lines[i].Syllables[j].Text = kls.Lines[i].Syllables[j].Text.ToUpper();
                    line.Add(kls.Lines[i].Syllables[j]);
                }
                klsNoParagraphs.Add(line);                
            }

            return klsNoParagraphs;
        }

        /// <summary>
        /// Search for introduction and instrumentals in a song
        /// The minimum duration between two consecutive vocal phrases that mark an instrumental interlude
        /// </summary>
        /// <param name="kls"></param>
        /// <returns></returns>
        private kLyrics SearchForInstrumentals(kLyrics kls, int MinimumInstrumentalDuration)
        {
            double tOnPrevious = 0;
            double duration = 0;
            double introDurationMinimum = 1000;
            double t = 0;
            kLyrics klsWithinstrumentals = new kLyrics();
            kLine line;
            string text = string.Empty;
            double tend = 0;

            // Introduction                        
            for (int i = 0; i < kls.Lines.Count; i++)
            {
                line = new kLine();
                for (int j = 0; j < kls.Lines[i].Syllables.Count; j++)
                {
                    if (kls.Lines[i].Syllables[j].CharType != Syllable.CharTypes.ParagraphSep)
                    {

                        t = kls.Lines[i].Syllables[j].StartTime;

                        // Create two lines for introduction (if no syllable at t = 0)
                        if (i == 0 && j == 0 && t > 0)
                        {
                            // Start of intro
                            line.Add(new Syllable() { Text = "(introduction)", StartTime = 0, CharType = Syllable.CharTypes.Information });
                            klsWithinstrumentals.Add(line);

                            if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
                            {
                                // end of intro = just before first lyric
                                tend = t;
                                if (tend > introDurationMinimum)
                                    tend = tend - introDurationMinimum;
                                line = new kLine();
                                line.Add(new Syllable() { Text = "", StartTime = tend, CharType = Syllable.CharTypes.Information });
                                klsWithinstrumentals.Add(line);
                            }

                            line = new kLine();
                        }
                        else if (t - tOnPrevious > MinimumInstrumentalDuration)
                        {
                            // Instrumental must be on line 0 or 2

                            // instrumental allowed
                            // Forbidden
                            // instrumental allowed
                            // Forbidden

                            if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                            {
                                int c = klsWithinstrumentals.Lines.Count;
                                if (c % 4 == 1)
                                {
                                    if (line.Syllables.Count > 0)
                                        klsWithinstrumentals.Add(line);

                                    // if on line 1
                                    line = new kLine();
                                    line.Add(new Syllable() { Text = "", StartTime = t + 45, CharType = Syllable.CharTypes.Information });
                                    klsWithinstrumentals.Add(line);

                                    line = new kLine();

                                }
                                else if (c % 4 == 3)
                                {
                                    if (line.Syllables.Count > 0)
                                        klsWithinstrumentals.Add(line);

                                    // If on line 3
                                    line = new kLine();
                                    line.Add(new Syllable() { Text = "", StartTime = t + 45, CharType = Syllable.CharTypes.Information });
                                    klsWithinstrumentals.Add(line);

                                    line = new kLine();
                                }
                            }



                            // Create two lines for instrumental
                            // An Instrumental part exists from tOnPrevious to t
                            // When can add a lyric called "(Instrumental)" a few time after tPrevious

                            if (line.Syllables.Count > 0)
                                klsWithinstrumentals.Add(line);

                            // First line instrumental
                            line = new kLine();
                            line.Add(new Syllable() { Text = "(instrumental)", StartTime = tOnPrevious + duration, CharType = Syllable.CharTypes.Information });
                            klsWithinstrumentals.Add(line);

                            if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
                            {
                                // Second line instrumental, 2 seconds before the end   => NOT USEFUL ????
                                tend = t;
                                if (tend - 2000 > 0)
                                    tend = tend - 2000;

                                line = new kLine();
                                line.Add(new Syllable() { Text = "", StartTime = tend, CharType = Syllable.CharTypes.Information });
                                klsWithinstrumentals.Add(line);
                            }

                            line = new kLine();
                        }

                        tOnPrevious = t;    // Start time of previous lyric
                        duration = kls.Lines[i].Syllables[j].Duration; // Duration of previous lyric

                    }
                    line.Add(kls.Lines[i].Syllables[j]);
                }
                klsWithinstrumentals.Add(line);
            }

            // ENDING
            t = _kLyrics.Lines.Last().Syllables.Last().StartTime;

            if (_duration * 1000 - t > MinimumInstrumentalDuration)
            {

                // Instrumental must be on line 0 or 2

                // instrumental allowed
                // Forbidden
                // instrumental allowed
                // Forbidden

                if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                {
                    int c = klsWithinstrumentals.Lines.Count;
                    if (c % 4 == 1)
                    {

                        // if on line 1
                        line = new kLine();
                        line.Add(new Syllable() { Text = "", StartTime = t + 45, CharType = Syllable.CharTypes.Information });
                        klsWithinstrumentals.Add(line);
                    }
                    else if (c % 4 == 3)
                    {
                        // If on line 3
                        line = new kLine();
                        line.Add(new Syllable() { Text = "", StartTime = t + 45, CharType = Syllable.CharTypes.Information });
                        klsWithinstrumentals.Add(line);
                    }
                }


                // First line
                line = new kLine();
                line.Add(new Syllable() { Text = "(ending)", StartTime = t + duration, CharType = Syllable.CharTypes.Information });
                klsWithinstrumentals.Add(line);

                if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
                {
                    // 2nd line 1 sec before the end of the song
                    tend = _duration * 1000 - 1000;
                    line = new kLine();
                    line.Add(new Syllable() { Text = "", StartTime = tend, CharType = Syllable.CharTypes.Information });
                    klsWithinstrumentals.Add(line);
                }
            }
            return klsWithinstrumentals;
        }
           

        private void Init()
        {
            if (_kLyrics == null) return;
            if (_kLyrics.Lines == null) return;
            if (_kLyrics.Lines.Count == 0) return;

            // Upadate _nbLyricsLines
            switch (KaraokeDisplayType)
            {
                case KaraokeDisplayTypes.FixedLines:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
                case KaraokeDisplayTypes.FourLinesSwapped:
                    _nbLyricsLines = 4;
                    break;
                case KaraokeDisplayTypes.TwoLinesSwapped:
                    _nbLyricsLines = 2;
                    break;
                case KaraokeDisplayTypes.ScrollingLinesBottomUp:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    _nbLyricsLines += _nbLyricsLinesOrg;
                    break;
                default:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
            }
            
            // Do not display paragraphs for some cases
            if (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped || !bShowParagraphs)
            {
                if (!_bIsSettings)
                    _kLyrics = RemoveParagraphs(_kLyrics);
            }

            // If Upper case required
            if (_bforceUppercase) 
                _kLyrics = ForceUpperCase(_kLyrics);

            // Analyse lyrics to find introduction, instrumentals etc..
           if (!_bIsSettings && (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped))
                _kLyrics = SearchForInstrumentals(_kLyrics, _MinimumInstrumentalDuration);

           
            // Store all lines lengths
            LinesLengths = new float[_kLyrics.Lines.Count];

            // Biggest line
            _biggestLine = GetBiggestLine();
            AjustText(_biggestLine);

            _LastLineToShow = SetLastLineToShow(_FirstLineToShow,  _kLyrics.Lines.Count, _nbLyricsLines);
        }

        #endregion
      

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

                    StringFormat sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

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
            return MeasureString(_kLyrics.Lines[curline].ToString(), _karaokeFont.Size);          
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

                case "SolidColor":
                    e.Graphics.FillRectangle(new SolidBrush(_BgColor), new Rectangle(0, 0, this.Width, this.Height));
                    break;

                case "Diaporama":
                   
                    if (m_BitmapsArray.Length == 1)
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

            switch (KaraokeDisplayType)
            {
                case KaraokeDisplayTypes.FixedLines:
                    DrawTextWithFixedLines(e);
                    break;
                case KaraokeDisplayTypes.ScrollingLinesBottomUp:
                    DrawTextWithScrollingLinesBottomUp(e);
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    DrawTextWithScrollingLinesTopDown(e);
                    break;
                case KaraokeDisplayTypes.TwoLinesSwapped:
                    DrawTextWithTwoLinesSwapped(e);
                    break;
                case KaraokeDisplayTypes.FourLinesSwapped:
                    DrawTextWithFourLinesSwapped(e);
                    break;
            }
                  
            #endregion draw text
        }



        #region Code fragments
       
        private void DrawActiveLineWithBorders(PaintEventArgs e, int lineIndex, int y1)
        {
            #region declarations
            int Wbg;
            RectangleF Rbg;

            Region r;
            RectangleF rect;

            Brush ActiveColorBrush = new SolidBrush(ActiveColor);
            Brush HighlightColorBrush = new SolidBrush(HighlightColor);
            Brush InactiveColorBrush = new SolidBrush(InactiveColor);

            Pen ActiveBorderPen = new Pen(new SolidBrush(ActiveBorderColor), _borderthick);
            Pen InactiveBorderPen = new Pen(new SolidBrush(InactiveBorderColor), _borderthick);

            int x0;
            GraphicsPath pth = new GraphicsPath();

            string s;

            #endregion declarations

            if (lineIndex >= _kLyrics.Lines.Count()) return;


            s = _kLyrics.Lines[lineIndex].ToString();
            x0 = HCenterText(s);      // Center horizontally

            #region background of syllabe                              
            if (_bTextBackGround)
            {
                Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                // Black background to make text more visible
                Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y1), Wbg, _lineHeight);
                // background
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
            }
            #endregion

            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

            // Draw full line in white if no active and highlight fragments
            if (active_fragment == string.Empty && highlight_fragment == string.Empty && inactive_fragment == string.Empty)
            {
                #region Draw static text (no active and highlight fragments)

                #region apply effect
                //CreateShadowEffect(s, _InactiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pth);
                #endregion apply effect

                // Fill GraphicsPath path in white => full text is white                    
                e.Graphics.FillPath(InactiveColorBrush, pth);
                // Outline the text                                
                if (_borderthick > 0)
                    e.Graphics.DrawPath(InactiveBorderPen, pth);

                #endregion Draw static text (no active and highlight fragments)
            }
            else
            {
                #region Draw dynamic text (with active and highlight fragments)

                r = new Region(pth);
                // Create a retangle of the graphical path
                rect = r.GetBounds(e.Graphics);

                #region draw active text
                if (active_fragment != string.Empty)
                {
                    GraphicsPath pathActive = new GraphicsPath();

                    pathActive.AddString(active_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

                    #region Paint in ActiveColor

                    // Rectangle for text befor highlighted text (rect.Width * lastpercent)
                    RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);
                    //RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, active_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectBefore);

                    // Fill updated region in green
                    e.Graphics.FillRegion(ActiveColorBrush, r);

                    #endregion Paint in ActiveColor

                    #region apply effect

                    //CreateShadowEffect(active_fragment, _ActiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pathActive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(ActiveColorBrush, pathActive);

                    // Outline the text                                
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(ActiveBorderPen, pathActive);

                    pathActive.Dispose();
                }
                #endregion Draw active text


                #region draw highlight text      
                if (highlight_fragment != string.Empty)
                {
                    GraphicsPath pathHighlight = new GraphicsPath();
                    pathHighlight.AddString(highlight_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length), y1), sf);

                    #region Paint in HighlightColor    

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);
                    //RectangleF intersectRect = new RectangleF(rect.X + active_fragment_length, rect.Y, highlight_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRect);

                    // Fill updated region in red => percent portion of text is red            
                    e.Graphics.FillRegion(HighlightColorBrush, r);

                    #endregion Paint in HighlightColor


                    #region apply effect

                    //CreateShadowEffect(highlight_fragment, _ActiveBorderColor, (int)(x0 + active_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathHighlight);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(HighlightColorBrush, pathHighlight);

                    // Outline text                
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(ActiveBorderPen, pathHighlight);

                    pathHighlight.Dispose();
                }
                #endregion draw highlight text


                #region Draw inactive text

                if (inactive_fragment != string.Empty)
                {
                    GraphicsPath pathInactive = new GraphicsPath();
                    pathInactive.AddString(inactive_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length + highlight_fragment_length), y1), sf);

                    #region Paint in InactiveColor

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRectAfter = new RectangleF(rect.X + rect.Width * percent, rect.Y, rect.Width - rect.Width * percent, rect.Height);
                    //RectangleF intersectRectAfter = new RectangleF(rect.X + active_fragment_length + highlight_fragment_length, rect.Y, inactive_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectAfter);

                    // Fill updated region in InactiveColor
                    e.Graphics.FillRegion(InactiveColorBrush, r);

                    #endregion Paint in InactiveColor

                    #region apply effect

                    //CreateShadowEffect(inactive_fragment, _InactiveBorderColor, (int)(x0 + active_fragment_length + highlight_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathInactive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(InactiveColorBrush, pathInactive);

                    // Outline the text
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(InactiveBorderPen, pathInactive);

                    pathInactive.Dispose();

                }
                #endregion Draw inactive text


                r.Dispose();

                #endregion Draw dynamic text (with active and highlight fragments)
            }

            #region Clean up resources
            pth.Dispose();
            ActiveColorBrush.Dispose();
            HighlightColorBrush.Dispose();
            InactiveColorBrush.Dispose();
            ActiveBorderPen.Dispose();
            InactiveBorderPen.Dispose();
            #endregion Clean up resources
        }

        private void DrawActiveLineWithShadow(PaintEventArgs e, int lineIndex, int y1)
        {
            #region declarations
            int Wbg;
            RectangleF Rbg;

            Region r;
            RectangleF rect;

            Brush ActiveColorBrush = new SolidBrush(ActiveColor);
            Brush HighlightColorBrush = new SolidBrush(HighlightColor);
            Brush InactiveColorBrush = new SolidBrush(InactiveColor);

            Pen ActiveBorderPen = new Pen(new SolidBrush(ActiveBorderColor), _borderthick);
            Pen InactiveBorderPen = new Pen(new SolidBrush(InactiveBorderColor), _borderthick);

            int x0;
            GraphicsPath pth = new GraphicsPath();

            string s;

            #endregion declarations

            if (lineIndex >= _kLyrics.Lines.Count()) return;

            
            s = _kLyrics.Lines[lineIndex].ToString();                            
            x0 = HCenterText(s);      // Center horizontally

            #region background of syllabe                              
            if (_bTextBackGround)
            {
                Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                // Black background to make text more visible
                Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y1), Wbg, _lineHeight);
                // background
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
            }
            #endregion

            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);          

            // Draw full line in white if no active and highlight fragments
            if (active_fragment == string.Empty && highlight_fragment == string.Empty && inactive_fragment == string.Empty)
            {
                #region Draw static text (no active and highlight fragments)

                #region apply effect
                CreateShadowEffect(s, _InactiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pth);
                #endregion apply effect

                // Fill GraphicsPath path in white => full text is white                    
                e.Graphics.FillPath(InactiveColorBrush, pth);
                // Outline the text                                
                e.Graphics.DrawPath(InactiveBorderPen, pth);

                #endregion Draw static text (no active and highlight fragments)
            }
            else
            {
                #region Draw dynamic text (with active and highlight fragments)

                r = new Region(pth);
                // Create a retangle of the graphical path
                rect = r.GetBounds(e.Graphics);

                #region draw active text
                if (active_fragment != string.Empty)
                {
                    GraphicsPath pathActive = new GraphicsPath();

                    pathActive.AddString(active_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

                    #region Paint in ActiveColor

                    // Rectangle for text befor highlighted text (rect.Width * lastpercent)
                    RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);
                    //RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, active_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectBefore);

                    // Fill updated region in green
                    e.Graphics.FillRegion(ActiveColorBrush, r);

                    #endregion Paint in ActiveColor

                    #region apply effect

                    CreateShadowEffect(active_fragment, _ActiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pathActive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(ActiveColorBrush, pathActive);

                    // Outline the text                                
                    e.Graphics.DrawPath(ActiveBorderPen, pathActive);

                    pathActive.Dispose();
                }
                #endregion Draw active text


                #region draw highlight text      
                if (highlight_fragment != string.Empty)
                {
                    GraphicsPath pathHighlight = new GraphicsPath();
                    pathHighlight.AddString(highlight_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length), y1), sf);

                    #region Paint in HighlightColor    

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);
                    //RectangleF intersectRect = new RectangleF(rect.X + active_fragment_length, rect.Y, highlight_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRect);

                    // Fill updated region in red => percent portion of text is red            
                    e.Graphics.FillRegion(HighlightColorBrush, r);

                    #endregion Paint in HighlightColor


                    #region apply effect

                    CreateShadowEffect(highlight_fragment, _ActiveBorderColor, (int)(x0 + active_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathHighlight);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(HighlightColorBrush, pathHighlight);

                    // Outline text                
                    e.Graphics.DrawPath(ActiveBorderPen, pathHighlight);

                    pathHighlight.Dispose();
                }
                #endregion draw highlight text


                #region Draw inactive text

                if (inactive_fragment != string.Empty)
                {
                    GraphicsPath pathInactive = new GraphicsPath();
                    pathInactive.AddString(inactive_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length + highlight_fragment_length), y1), sf);

                    #region Paint in InactiveColor

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRectAfter = new RectangleF(rect.X + rect.Width * percent, rect.Y, rect.Width - rect.Width * percent, rect.Height);
                    //RectangleF intersectRectAfter = new RectangleF(rect.X + active_fragment_length + highlight_fragment_length, rect.Y, inactive_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectAfter);

                    // Fill updated region in InactiveColor
                    e.Graphics.FillRegion(InactiveColorBrush, r);

                    #endregion Paint in InactiveColor

                    #region apply effect

                    CreateShadowEffect(inactive_fragment, _InactiveBorderColor, (int)(x0 + active_fragment_length + highlight_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathInactive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(InactiveColorBrush, pathInactive);

                    // Outline text                
                    e.Graphics.DrawPath(InactiveBorderPen, pathInactive);

                    pathInactive.Dispose();

                }
                #endregion Draw inactive text


                r.Dispose();

                #endregion Draw dynamic text (with active and highlight fragments)
            }
            
            #region Clean up resources
            pth.Dispose();
            ActiveColorBrush.Dispose();
            HighlightColorBrush.Dispose();
            InactiveColorBrush.Dispose();
            ActiveBorderPen.Dispose();
            InactiveBorderPen.Dispose();
            #endregion Clean up resources
        }
       
        private void DrawActiveLineWithNeon(PaintEventArgs e, int lineIndex, int y1)
        {
            #region declarations
            int Wbg;
            RectangleF Rbg;

            Region r;
            RectangleF rect;

            Brush ActiveColorBrush = new SolidBrush(ActiveColor);
            Brush HighlightColorBrush = new SolidBrush(HighlightColor);
            Brush InactiveColorBrush = new SolidBrush(InactiveColor);

            Pen ActiveBorderPen = new Pen(new SolidBrush(ActiveBorderColor), _borderthick);
            Pen InactiveBorderPen = new Pen(new SolidBrush(InactiveBorderColor), _borderthick);

            int x0;
            GraphicsPath pth = new GraphicsPath();

            string s;

            #endregion declarations

            if (lineIndex >= _kLyrics.Lines.Count()) return;


            s = _kLyrics.Lines[lineIndex].ToString();
            x0 = HCenterText(s);      // Center horizontally

            #region background of syllabe                              
            if (_bTextBackGround)
            {
                Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                // Black background to make text more visible
                Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y1), Wbg, _lineHeight);
                // background
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
            }
            #endregion

            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

            // Draw full line in white if no active and highlight fragments
            if (active_fragment == string.Empty && highlight_fragment == string.Empty && inactive_fragment == string.Empty)
            {
                #region Draw static text (no active and highlight fragments)

                #region apply effect
                //CreateShadowEffect(s, _InactiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pth);
                CreateNeonEffect(_InactiveBorderColor, e, pth);
                #endregion apply effect

                // Fill GraphicsPath path in white => full text is white                    
                e.Graphics.FillPath(InactiveColorBrush, pth);
                // Outline the text                                
                e.Graphics.DrawPath(InactiveBorderPen, pth);

                #endregion Draw static text (no active and highlight fragments)
            }
            else
            {
                #region Draw dynamic text (with active and highlight fragments)

                r = new Region(pth);
                // Create a retangle of the graphical path
                rect = r.GetBounds(e.Graphics);

                #region draw active text
                if (active_fragment != string.Empty)
                {
                    GraphicsPath pathActive = new GraphicsPath();

                    pathActive.AddString(active_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

                    #region Paint in ActiveColor

                    // Rectangle for text befor highlighted text (rect.Width * lastpercent)
                    RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, rect.Width * lastpercent, rect.Height);
                    //RectangleF intersectRectBefore = new RectangleF(rect.X, rect.Y, active_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectBefore);

                    // Fill updated region in green
                    e.Graphics.FillRegion(ActiveColorBrush, r);

                    #endregion Paint in ActiveColor

                    #region apply effect

                    //CreateShadowEffect(active_fragment, _ActiveBorderColor, x0, y1, _karaokeFont, _karaokeFont.Size, e, pathActive);
                    CreateNeonEffect(_ActiveBorderColor, e, pathActive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(ActiveColorBrush, pathActive);

                    // Outline the text                                
                    e.Graphics.DrawPath(ActiveBorderPen, pathActive);

                    pathActive.Dispose();
                }
                #endregion Draw active text


                #region draw highlight text      
                if (highlight_fragment != string.Empty)
                {
                    GraphicsPath pathHighlight = new GraphicsPath();
                    pathHighlight.AddString(highlight_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length), y1), sf);

                    #region Paint in HighlightColor    

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRect = new RectangleF(rect.X + rect.Width * lastpercent, rect.Y, rect.Width * (percent - lastpercent), rect.Height);
                    //RectangleF intersectRect = new RectangleF(rect.X + active_fragment_length, rect.Y, highlight_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRect);

                    // Fill updated region in red => percent portion of text is red            
                    e.Graphics.FillRegion(HighlightColorBrush, r);

                    #endregion Paint in HighlightColor


                    #region apply effect

                    //CreateShadowEffect(highlight_fragment, _ActiveBorderColor, (int)(x0 + active_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathHighlight);
                    CreateNeonEffect(_ActiveBorderColor, e, pathHighlight);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(HighlightColorBrush, pathHighlight);

                    // Outline text                
                    e.Graphics.DrawPath(ActiveBorderPen, pathHighlight);

                    pathHighlight.Dispose();
                }
                #endregion draw highlight text


                #region Draw inactive text

                if (inactive_fragment != string.Empty)
                {
                    GraphicsPath pathInactive = new GraphicsPath();
                    pathInactive.AddString(inactive_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)(x0 + active_fragment_length + highlight_fragment_length), y1), sf);

                    #region Paint in InactiveColor

                    // Create another rectangle shorter than the 1st one (percent of the first)
                    RectangleF intersectRectAfter = new RectangleF(rect.X + rect.Width * percent, rect.Y, rect.Width - rect.Width * percent, rect.Height);
                    //RectangleF intersectRectAfter = new RectangleF(rect.X + active_fragment_length + highlight_fragment_length, rect.Y, inactive_fragment_length, rect.Height);

                    // update region on the intersection between region and 2nd rectangle
                    r.Intersect(intersectRectAfter);

                    // Fill updated region in InactiveColor
                    e.Graphics.FillRegion(InactiveColorBrush, r);

                    #endregion Paint in InactiveColor

                    #region apply effect

                    //CreateShadowEffect(inactive_fragment, _InactiveBorderColor, (int)(x0 + active_fragment_length + highlight_fragment_length), y1, _karaokeFont, _karaokeFont.Size, e, pathInactive);
                    CreateNeonEffect(_InactiveBorderColor, e, pathInactive);

                    #endregion apply effect

                    // Draw the text               
                    e.Graphics.FillPath(InactiveColorBrush, pathInactive);

                    // Outline text                
                    e.Graphics.DrawPath(InactiveBorderPen, pathInactive);

                    pathInactive.Dispose();

                }
                #endregion Draw inactive text


                r.Dispose();

                #endregion Draw dynamic text (with active and highlight fragments)
            }

            #region Clean up resources
            pth.Dispose();
            ActiveColorBrush.Dispose();
            HighlightColorBrush.Dispose();
            InactiveColorBrush.Dispose();
            ActiveBorderPen.Dispose();
            InactiveBorderPen.Dispose();
            #endregion Clean up resources
        }

        private void DrawInactiveLineWithBorders(PaintEventArgs e, int lineIndex, int y2, bool IsActive = false)
        {
            #region Declarations

            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            // Create a graphical path
            var path = new GraphicsPath();
            int x0;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
            int Wbg;
            RectangleF Rbg;
            string s;

            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                s = _kLyrics.Lines[lineIndex].ToString();                
                x0 = HCenterText(s);     // Center text horizontally

                #region Background of text  

                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y2), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }

                #endregion Background of text

                // Add lines of lyrics to the Graphics path
                path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y2), sf);
            
                // Draw the text            
                e.Graphics.FillPath(new SolidBrush(FillColor), path);

                // Outline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, path);
            }

            #region Clean up resources

            path.Dispose();            
            penBorder.Dispose();
            
            #endregion Clean up resources
        }

        private void DrawInactiveLineWithShadow(PaintEventArgs e, int lineIndex, int y2, bool IsActive = false)
        {
            #region Declarations
            GraphicsPath path = new GraphicsPath();
            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            int x0;
            int Wbg;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
            RectangleF Rbg;
            string s;

            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                s = _kLyrics.Lines[lineIndex].ToString();                
                x0 = HCenterText(s);     // Center text horizontally

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y2), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }
                #endregion

                // Add lines of lyrics to the Graphics path
                path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y2), sf);

                #region Shadow effect
                
                CreateShadowEffect(s, BorderColor, x0, y2, _karaokeFont, _karaokeFont.Size, e, path);

                #endregion Shadow effect

                // Draw the text            
                e.Graphics.FillPath(new SolidBrush(FillColor), path);

                // Outline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, path);
            }

            #region Clean up resources

            path.Dispose();
            penBorder.Dispose();
            
            #endregion Clean up resources

        }

        private void DrawInactiveLineWithNeon(PaintEventArgs e, int lineIndex, int y2, bool IsActive = false)
        {
            #region Declarations

            GraphicsPath path = new GraphicsPath();
            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            int x0;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
            int Wbg;
            RectangleF Rbg;
            string s;

            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                s = _kLyrics.Lines[lineIndex].ToString();                
                x0 = HCenterText(s);     // Center text horizontally
                
                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                    // Black background to make text more visible
                    Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y2), Wbg, _lineHeight);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
                }
                #endregion

                // Add lines of lyrics to the Graphics path
                path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y2), sf);

                #region Neon effect

                CreateNeonEffect(BorderColor, e, path);

                #endregion Neon effect

                // Draw the text            
                e.Graphics.FillPath(new SolidBrush(FillColor), path);

                // Outline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, path);

            }

            #region Clean up resources

            path.Dispose();
            penBorder.Dispose();

            #endregion Clean up resources

        }

        
        private void DrawInformation(PaintEventArgs e, string infotext, int y0)
        {
            GraphicsPath path = new GraphicsPath();
            int x0;
            Pen penBorder = new Pen(Color.Black);
            Color FillColor = Color.Gray;

            x0 = HCenterText(infotext);

            // Add lines of lyrics to the Graphics path
            path.AddString(infotext, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0, (int)y0), sf);            

            // Draw the text                    
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text
            if (_borderthick > 0)
                e.Graphics.DrawPath(penBorder, path);
        }
        
        
        #endregion Code fragments


        #region Draw text with Scrolling lines bottom up
        private void DrawTextWithScrollingLinesBottomUp(PaintEventArgs e)
        {
            // To be implemented
            switch (FrameType)
            {
                case "NoBorder":
                case "FrameThin":
                case "Frame1":
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5":
                    SbuDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    SbuDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    SbuDrawTextWithNeon(e);
                    break; ;

                default:
                    SbuDrawTextWithBorder(e);
                    break;
            }
        }

        private void SbuDrawTextWithBorder(PaintEventArgs e)
        {
            // To be implemented
        }

        private void SbuDrawTextWithShadow(PaintEventArgs e)
        {
            // To be implemented
        }

        private void SbuDrawTextWithNeon(PaintEventArgs e)
        {
            // To be implemented
        }


        #endregion Draw text with Scrolling lines bottom up


        #region Draw text with Scrolling lines top down
        private void DrawTextWithScrollingLinesTopDown(PaintEventArgs e)
        {
            switch (FrameType)
            {
                case "NoBorder":
                case "FrameThin":
                case "Frame1":
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5":
                    StdDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    StdDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    StdDrawTextWithNeon(e);
                    break; ;

                default:
                    StdDrawTextWithBorder(e);
                    break;
            }
        }

        private void StdDrawTextWithBorder(PaintEventArgs e)
        {
            // To be implemented
        }

        private void StdDrawTextWithShadow(PaintEventArgs e)
        {
            // To be implemented
        }

        private void StdDrawTextWithNeon(PaintEventArgs e)
        {
            // To be implemented
        }

        #endregion Draw text with Scrolling lines top down


        #region Draw text with Two lines swapped

        private void DrawTextWithTwoLinesSwapped(PaintEventArgs e)
        {
            //_nbLyricsLines = 2;

            switch (FrameType)
            {
                case "NoBorder":
                case "FrameThin":
                case "Frame1":
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5":
                    TlsDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    TlsDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    TlsDrawTextWithNeon(e);
                    break; ;

                default:
                    TlsDrawTextWithBorder(e);
                    break;
            }
        }

        /// <summary>
        /// Draw two lines swapped with borders
        /// </summary>
        /// <param name="e"></param>
        private void TlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Center text vertically
            int y0 = VCenterText();
         
            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition = -1;


            #region Line layout

            // If active line is odd, it is displayed on the first line
            // if active line is even, it is displayed on the second line
            if (_FirstLineToShow % 2 == 0)
            {
                LinePosition = 0;

                y1 = y0;                    // active     _FirstLinetoShow
                y2 = y0 + _lineHeight;      // inactive   _FirstLinetoShow + 1  

                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;

            }
            else
            {
                LinePosition = 1;

                y2 = y0;                    // inactive     _FirstLinetoShow + 1
                y1 = y0 + _lineHeight;      // active       _FirstLinetoShow

                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if ( _FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;
            }

            #endregion Line layout


            // No instrumental
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                // Draw active line with borders
                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);

                // Draw Inactive line with borders
                if (percent > 0)
                    DrawInactiveLineWithBorders(e, _FirstLineToShow + 1, y2);

                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                //CheckIfInstrumentalBegins2();
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();

                TimeSpan tm = DateTime.Now - _startTime;

                // Instrumental found
                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top

                    case 0:                         // Instrumental on line 0
                        switch(LinePosition)
                        {                                                       
                            case 0:
                                // y1 * information
                                // y2 normal
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y2, true);         // keep old line 1 sec                                            
                                        }
                                    }
                                }

                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                // Except for introduction
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithBorders(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental         
                                break;
                                                                                   
                            case 1:
                                // y2 information
                                // y1 * normal

                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y2);
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);                                
                                break;                            
                            
                        }
                        break;

                    #endregion Instrumental on top


                    #region Instrumental on bottom

                    case 1:                         // Instrumental on line 1
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * normal
                                // y2 information
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y1 - _lineHeight , true);         // keep old line 1 sec
                                            //Console.WriteLine(_kLyrics.Lines[_FirstLineToShow - 1].ToString());                                                                                                                                   

                                        }
                                    }
                                }
                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithBorders(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental   
                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }

            }

            LastLineOfInformationPosition = LineOfInformationPosition;
        
        }

        private void TlsDrawTextWithShadow(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Center text vertically
            int y0 = VCenterText();

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition = -1;

            #region Line layout

            // If active line is odd, it is displayed on the first line
            // if active line is even, it is displayed on the second line
            if (_FirstLineToShow % 2 == 0)
            {
                LinePosition = 0;

                y1 = y0;
                y2 = y0 + _lineHeight;

                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;
            }
            else
            {
                LinePosition = 1;

                y2 = y0;
                y1 = y0 + _lineHeight;
                
                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if (_FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;
            }

            #endregion Line layout


            // No instrumental
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                // Draw active line with Shadow
                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);

                // Draw Inactive line with Shadow
                if (percent > 0)
                    DrawInactiveLineWithShadow(e, _FirstLineToShow + 1, y2);

                #endregion Normal drawing

            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                //CheckIfInstrumentalBegins2();
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();

                TimeSpan tm = DateTime.Now - _startTime;

                // Instrumental found
                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top

                    case 0:                         // Instrumental on line 0
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * information
                                // y2 normal
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 1, y2, true);         // keep old line 1 sec                                            
                                        }
                                    }
                                }

                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                // Except for introduction
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithShadow(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental         
                                break;

                            case 1:
                                // y2 information
                                // y1 * normal

                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y2);
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);
                                break;
                        }
                        break;

                    #endregion Instrumental on top

                    #region Instrumental on bottom

                    case 1:                         // Instrumental on line 1
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * normal
                                // y2 information
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 1, y1 - _lineHeight, true);         // keep old line 1 sec
                                            //Console.WriteLine(_kLyrics.Lines[_FirstLineToShow - 1].ToString());                                                                                                                                   

                                        }
                                    }
                                }
                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithShadow(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental   
                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }
        }

        private void TlsDrawTextWithNeon(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Center text vertically
            int y0 = VCenterText();

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition = -1;

            #region Line layout

            // If active line is odd, it is displayed on the first line
            // if active line is even, it is displayed on the second line
            if (_FirstLineToShow % 2 == 0)
            {
                LinePosition = 0;

                y1 = y0;
                y2 = y0 + _lineHeight;

                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;
            }
            else
            {
                LinePosition = 1;

                y2 = y0;
                y1 = y0 + _lineHeight;

                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 1;
                else if (_FirstLineToShow + 1 < _kLyrics.Lines.Count && _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().CharType == Syllable.CharTypes.Information)
                    LineOfInformationPosition = 0;
                else if (_FirstLineToShow + 1 >= _kLyrics.Lines.Count)
                    LineOfInformationPosition = LastLineOfInformationPosition;
            }

            #endregion Line layout

            // No instrumental
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                // Draw active line with Shadow
                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);

                // Draw Inactive line with Shadow
                if (percent > 0)
                    DrawInactiveLineWithNeon(e, _FirstLineToShow + 1, y2);
                
                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                //CheckIfInstrumentalBegins2();
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();

                TimeSpan tm = DateTime.Now - _startTime;

                // Instrumental found
                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top

                    case 0:                         // Instrumental on line 0
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * information
                                // y2 normal
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 1, y2, true);         // keep old line 1 sec                                            
                                        }
                                    }
                                }

                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                // Except for introduction
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithNeon(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental         
                                break;

                            case 1:
                                // y2 information
                                // y1 * normal

                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y2);
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);
                                break;
                        }
                        break;

                    #endregion Instrumental on top

                    #region Instrumental on bottom

                    case 1:                         // Instrumental on line 1
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * normal
                                // y2 information
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 1, y1 - _lineHeight, true);         // keep old line 1 sec
                                            //Console.WriteLine(_kLyrics.Lines[_FirstLineToShow - 1].ToString());                                                                                                                                   

                                        }
                                    }
                                }
                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                DrawInactiveLineWithNeon(e, _FirstLineToShow + 1, y2);      // Draw new line before the end of instrumental   
                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }
        }

        #endregion Draw text with Two lines swapped


        #region Draw text with Four lines swapped
        private void DrawTextWithFourLinesSwapped(PaintEventArgs e)
        {
            //_nbLyricsLines = 4;

            switch (FrameType)
            {
                case "NoBorder":
                case "FrameThin":
                case "Frame1":
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5":
                    FlsDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    FlsDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    FlsDrawTextWithNeon(e);
                    break; ;

                default:
                    FlsDrawTextWithBorder(e);
                    break;
            }
        }


        #region Instrumental functions

        /// <summary>
        /// Search for a line containing an instrumental
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private int SearchLineOfInformation(int [] lines)
        {
            int x;
            for (int i = 0; i < lines.Count(); i++)
            {
                x = lines[i];
                if (x < _kLyrics.Lines.Count)
                {
                    if (_kLyrics.Lines[x].Syllables.Last().CharType == Syllable.CharTypes.Information && _kLyrics.Lines[x].Syllables.Last().Text != string.Empty)
                    {
                        return i;
                    }
                }
            }
            return -2;
        }

        /// <summary>
        /// Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
        /// </summary>
        /// <remarks>This method determines if the current lyric line marks the start of an instrumental
        /// section. If so, it calculates the duration until the next text line and initiates a countdown for when
        /// singing should resume. This is typically used to manage the display and timing of lyric lines during
        /// instrumental breaks.</remarks>
        private void CheckIfInstrumentalBegins()
        {
            if (_kLyrics.Lines.Count == 0) return;
            
            // Check if lines 3 and 4 must be hiden             
            if (!bInstrumentalStarted)
            {
                // If _FirstLineToShow line is an instrumental, we have to wait until the end of the instrumental before drawing lines 1 and 2
                if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information && _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text != string.Empty)
                {
                    // Calculate endTime between _FirstLineToShow and the next Text line located in _FirstLineToShow + 2 when Four Lines swapped and _FirstLineToShow + 1 for Two lines swapped

                    if (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped)
                    {
                        if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                            TargetPositionMilliseconds = _kLyrics.Lines[_FirstLineToShow + 1].Syllables.First().StartTime;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionMilliseconds = _duration * 1000;
                    }
                    else if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                    {
                        // (_FirstLineToShow + 1 cannot be used because it is the 2nd line of information)
                        if (_FirstLineToShow + 2 < _kLyrics.Lines.Count)
                            TargetPositionMilliseconds = _kLyrics.Lines[_FirstLineToShow + 2].Syllables.First().StartTime;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionMilliseconds = _duration * 1000;                                                  // Position in the song to reach = end of song

                    }

                    _endTime = DateTime.Now.AddMilliseconds(TargetPositionMilliseconds - (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().StartTime + _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Duration));

                    _startTime = DateTime.Now;
                    TimeSpan tm = _endTime - DateTime.Now;
                    SecondsBeforeSinging = (int)tm.TotalSeconds;
                    bInstrumentalStarted = true;
                    bCountDown = true;
                }
            }
        }
             

        /// <summary>
        /// Update the CountDown
        /// </summary>
        private void UpdateCountDown()
        {
            if (bCountDown)
            {
                // Real position:  PlayerPositionMilliseconds
                // Position to reach: TargetPositionMilliseconds               

                // Recalculates the remaining time with PlayerPosition
                _endTime = DateTime.Now.AddMilliseconds(TargetPositionMilliseconds - PlayerPositionMilliseconds);

                TimeSpan tm = _endTime - DateTime.Now;

                if (tm.TotalSeconds < 0)
                {
                    _endTime = DateTime.Now;
                    _startTime = DateTime.Now;
                    bInstrumentalStarted = false;
                    SecondsBeforeSinging = -1;
                    bCountDown = false;
                }
                else
                {
                    // Time is about 3 sec before next lyric to sing
                    // Calculate countdown
                    int s = (int)tm.TotalSeconds;
                    if (s != SecondsBeforeSinging)
                    {
                        SecondsBeforeSinging = s;
                    }
                }
            }
        }

        #endregion Instrumental functions


        /// <summary>
        /// Draw four lines swapped
        /// </summary>
        /// <param name="e"></param>
        private void FlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;
            
            // Center text vertically
            int y0 = VCenterText();

            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int LinePosition = -1;

            // Search for information
            // None                         -2
            // _FirstLineToShow - 1         -1
            // _FirstLineToShow              0
            // _FirstLineToShow + 1          1
            // _FirstLineToShow + 2          2
            // _FirstLineToShow + 3          3
            int LineOfInformationPosition = -2;

            int[] LinesNr = new int[4];

            #region Line layout

            if (_FirstLineToShow % 4 == 0)
            {
                // First line is active                
                LinePosition = 0;
                // Position possible for LineOfInformationPosition

                y1 = y0;                            //          _FirstLineToShow              current         (update 3 & 4)
                y2 = y0 + _lineHeight;              // idx2     _FirstLineToShow + 1      inactive
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 2      inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 3      inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow;
                LinesNr[1] = _FirstLineToShow + 1;
                LinesNr[2] = _FirstLineToShow + 2;
                LinesNr[3] = _FirstLineToShow + 3;

            }
            else if (_FirstLineToShow % 4 == 1)
            {
                // 2nd line is active
                LinePosition = 1;
                // Position not possible for LineOfInformationPosition

                y2 = y0;                            // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + _lineHeight;              //          _FirstLineToShow             current         (no update)
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 2     inactive

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow - 1;
                LinesNr[1] = _FirstLineToShow;
                LinesNr[2] = _FirstLineToShow + 1;
                LinesNr[3] = _FirstLineToShow + 2;

            }
            else if (_FirstLineToShow % 4 == 2)
            {
                // 3rd line is active
                LinePosition = 2;
                // Position possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 2     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 3     inactive
                y1 = y0 + 2 * _lineHeight;          //          _FirstLineToShow             current         (update 1 & 2)
                y2 = y0 + 3 * _lineHeight;          // idx2     _FirstLineToShow + 1     inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow + 2;
                LinesNr[1] = _FirstLineToShow + 3;
                LinesNr[2] = _FirstLineToShow;
                LinesNr[3] = _FirstLineToShow + 1;


            }
            else if (_FirstLineToShow % 4 == 3)
            {
                // 4th line is active
                LinePosition = 3;
                // Position not possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 2     inactive
                y2 = y0 + 2 * _lineHeight;          // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + 3 * _lineHeight;          //          _FirstLineToShow             current         (no update)

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow + 1;
                LinesNr[1] = _FirstLineToShow + 2;
                LinesNr[2] = _FirstLineToShow - 1;
                LinesNr[3] = _FirstLineToShow;


            }

            #endregion Line layout

            bool bTooMuch = false;
            for (int i = 0; i < LinesNr.Length; i++)
            {
                if (LinesNr[i] >= _kLyrics.Lines.Count)
                {             
                    bTooMuch = true;
                    break;
                }
            }

            if (!bTooMuch)
                LineOfInformationPosition = SearchLineOfInformation(LinesNr);
            else
                LineOfInformationPosition = LastLineOfInformationPosition;            


            // If no line of information in the 4 lines => normal display in 4 lines swapped
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                bInstrumentalStarted = false;
                bCountDown = false;


                // Draw y1 line: active & highlighted line                
                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);

                // Line y2 must be drawned active when
                bool IsActive = ((_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3)) ? true : false;

                // Draw y2 line: active when before line y1 (already sung), inactive when after line y1 (not yet sung)
                if (idx2 >= 0)
                    DrawInactiveLineWithBorders(e, idx2, y2, IsActive);


                // Draw lines y3 and y4 (always inactives)
                if (idx3 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithBorders(e, idx3, y3);
                if (idx4 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithBorders(e, idx4, y4);

                #endregion Normal drwaing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();


                TimeSpan tm = DateTime.Now - _startTime;

                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top

                    case 0:                             // Instrumental is on line 0
                        // y1 * information
                        // y2 information
                        // y3 normal
                        // y4 normal
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * information1 new
                                // y2 information2   new
                                // y3 normal old than new
                                // y4 normal old than new
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 2, y3, true);         // keep old line 1 sec
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y4, true);         // keep olf line 1 sec

                                        }
                                    }
                                }

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                // Except if introduction (_FirstLineToShow = 0) show
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }

                                DrawInactiveLineWithBorders(e, idx3, y3);      // Draw new line before the end of instrumental         
                                DrawInactiveLineWithBorders(e, idx4, y4);      // Draw new line before the end of instrumental
                                break;

                            case 1:
                                // y2 information1
                                // y1 * information2 no update
                                // y3 normal 
                                // y4 normal 

                                // draw ("instrumental") on previous line and countdown on current line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1 - _lineHeight);

                                DrawInactiveLineWithBorders(e, idx3, y3);
                                DrawInactiveLineWithBorders(e, idx4, y4);
                                break;

                            case 2:
                                // y3 information1 new
                                // y4 information2 new
                                // y1 * normal old          update y3 & y4 
                                // y2 normal   old

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithBorders(e, idx2, y2, false);

                                // Draw instrumental on line 0 (y3)
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 3:
                                // y3 information1
                                // y4 information2
                                // y2 normal
                                // y1 * normal          no update

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithBorders(e, idx2, y2, true);

                                // Draw instrumental on line 0
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;
                        }
                        break;

                    #endregion Instrumental on top


                    #region Instrumental on bottom

                    case 2:                                                         // instrumental on line 2 (3rd line)
                        // y1 normal
                        // y2 normal
                        // y3 * information1
                        // y4 information2
                        switch (LinePosition)
                        {
                            case 0:                                                 // LinePosition is 0
                                // y1 * normal
                                // y2 normal
                                // y3 information1
                                // y4 information2

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);
                                // line y 2 is inactive (not yet played)
                                DrawInactiveLineWithBorders(e, idx2, y2, false);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 1:                                                 // LinePosition is 1
                                // y1 normal
                                // y2 * normal
                                // y3 information1
                                // y4 information2

                                // line y2 is already played
                                DrawInactiveLineWithBorders(e, idx2, y2, true);
                                // line y1 : active a highlighted line
                                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);

                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines
                                        return;
                                    }
                                }
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithBorders(e, idx3, y3);
                                if (idx4 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithBorders(e, idx4, y4);

                                break;

                            case 3:
                                // y1 normal
                                // y2 normal
                                // y3 information1
                                // y4 * information2
                                // Draw "(intrumental)" on active line and countdown on next line                                
                                DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y2);

                                DrawInactiveLineWithBorders(e, idx3, y3);
                                DrawInactiveLineWithBorders(e, idx4, y4);

                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }

            LastLineOfInformationPosition = LineOfInformationPosition;

        }           

        private void FlsDrawTextWithShadow(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Center text vertically
            int y0 = VCenterText();

            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int LinePosition = -1;

            // Search for information
            // None                         -2
            // _FirstLineToShow - 1         -1
            // _FirstLineToShow              0
            // _FirstLineToShow + 1          1
            // _FirstLineToShow + 2          2
            // _FirstLineToShow + 3          3
            int LineOfInformationPosition = -2;

            int[] LinesNr = new int[4];

            #region Line layout

            if (_FirstLineToShow % 4 == 0)
            {
                // First line is active                
                LinePosition = 0;
                // Position possible for LineOfInformationPosition

                y1 = y0;                            //          _FirstLineToShow              current         (update 3 & 4)
                y2 = y0 + _lineHeight;              // idx2     _FirstLineToShow + 1      inactive
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 2      inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 3      inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow;
                LinesNr[1] = _FirstLineToShow + 1;
                LinesNr[2] = _FirstLineToShow + 2;
                LinesNr[3] = _FirstLineToShow + 3;

            }
            else if (_FirstLineToShow % 4 == 1)
            {
                // 2nd line is active
                LinePosition = 1;
                // Position not possible for LineOfInformationPosition

                y2 = y0;                            // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + _lineHeight;              //          _FirstLineToShow             current         (no update)
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 2     inactive

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow - 1;
                LinesNr[1] = _FirstLineToShow;
                LinesNr[2] = _FirstLineToShow + 1;
                LinesNr[3] = _FirstLineToShow + 2;

            }
            else if (_FirstLineToShow % 4 == 2)
            {
                // 3rd line is active
                LinePosition = 2;
                // Position possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 2     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 3     inactive
                y1 = y0 + 2 * _lineHeight;          //          _FirstLineToShow             current         (update 1 & 2)
                y2 = y0 + 3 * _lineHeight;          // idx2     _FirstLineToShow + 1     inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow + 2;
                LinesNr[1] = _FirstLineToShow + 3;
                LinesNr[2] = _FirstLineToShow;
                LinesNr[3] = _FirstLineToShow + 1;


            }
            else if (_FirstLineToShow % 4 == 3)
            {
                // 4th line is active
                LinePosition = 3;
                // Position not possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 2     inactive
                y2 = y0 + 2 * _lineHeight;          // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + 3 * _lineHeight;          //          _FirstLineToShow             current         (no update)

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow + 1;
                LinesNr[1] = _FirstLineToShow + 2;
                LinesNr[2] = _FirstLineToShow - 1;
                LinesNr[3] = _FirstLineToShow;


            }

            #endregion Line layout


            bool bTooMuch = false;
            for (int i = 0; i < LinesNr.Length; i++)
            {
                if (LinesNr[i] >= _kLyrics.Lines.Count)
                {
                    bTooMuch = true;
                    break;
                }
            }

            if (!bTooMuch)
                LineOfInformationPosition = SearchLineOfInformation(LinesNr);
            else
                LineOfInformationPosition = LastLineOfInformationPosition;


            // If no line of information in the 4 lines => normal display in 4 lines swapped
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                bInstrumentalStarted = false;
                bCountDown = false;


                // Draw y1 line: active & highlighted line                
                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);

                // Line y2 must be drawned active when
                bool IsActive = ((_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3)) ? true : false;

                // Draw y2 line: active when before line y1 (already sung), inactive when after line y1 (not yet sung)
                if (idx2 >= 0)
                    DrawInactiveLineWithShadow(e, idx2, y2, IsActive);


                // Draw lines y3 and y4 (always inactives)
                if (idx3 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithShadow(e, idx3, y3);
                if (idx4 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithShadow(e, idx4, y4);
                
                #endregion Normal drawing

            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();


                TimeSpan tm = DateTime.Now - _startTime;

                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top

                    case 0:                             // Instrumental is on line 0
                        // y1 * information
                        // y2 information
                        // y3 normal
                        // y4 normal
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * information1 new
                                // y2 information2   new
                                // y3 normal old than new
                                // y4 normal old than new
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 2, y3, true);         // keep old line 1 sec
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 1, y4, true);         // keep olf line 1 sec

                                        }
                                    }
                                }

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                // Except if introduction (_FirstLineToShow = 0) show
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }

                                DrawInactiveLineWithShadow(e, idx3, y3);      // Draw new line before the end of instrumental         
                                DrawInactiveLineWithShadow(e, idx4, y4);      // Draw new line before the end of instrumental
                                break;

                            case 1:
                                // y2 information1
                                // y1 * information2 no update
                                // y3 normal 
                                // y4 normal 

                                // draw ("instrumental") on previous line and countdown on current line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1 - _lineHeight);

                                DrawInactiveLineWithShadow(e, idx3, y3);
                                DrawInactiveLineWithShadow(e, idx4, y4);
                                break;

                            case 2:
                                // y3 information1 new
                                // y4 information2 new
                                // y1 * normal old          update y3 & y4 
                                // y2 normal   old

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithShadow(e, idx2, y2, false);

                                // Draw instrumental on line 0 (y3)
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 3:
                                // y3 information1
                                // y4 information2
                                // y2 normal
                                // y1 * normal          no update

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithShadow(e, idx2, y2, true);

                                // Draw instrumental on line 0
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;
                        }
                        break;

                    #endregion Instrumental on top


                    #region Instrumental on bottom

                    case 2:                                                         // instrumental on line 2 (3rd line)
                        // y1 normal
                        // y2 normal
                        // y3 * information1
                        // y4 information2
                        switch (LinePosition)
                        {
                            case 0:                                                 // LinePosition is 0
                                // y1 * normal
                                // y2 normal
                                // y3 information1
                                // y4 information2

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);
                                // line y 2 is inactive (not yet played)
                                DrawInactiveLineWithShadow(e, idx2, y2, false);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 1:                                                 // LinePosition is 1
                                // y1 normal
                                // y2 * normal
                                // y3 information1
                                // y4 information2

                                // line y2 is already played
                                DrawInactiveLineWithShadow(e, idx2, y2, true);
                                // line y1 : active a highlighted line
                                DrawActiveLineWithShadow(e, _FirstLineToShow, y1);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);

                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines
                                        return;
                                    }
                                }
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithShadow(e, idx3, y3);
                                if (idx4 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithShadow(e, idx4, y4);

                                break;

                            case 3:
                                // y1 normal
                                // y2 normal
                                // y3 information1
                                // y4 * information2
                                // Draw "(intrumental)" on active line and countdown on next line                                
                                DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y2);

                                DrawInactiveLineWithShadow(e, idx3, y3);
                                DrawInactiveLineWithShadow(e, idx4, y4);

                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }

            LastLineOfInformationPosition = LineOfInformationPosition;

        }

        private void FlsDrawTextWithNeon(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Center text vertically
            int y0 = VCenterText();

            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int LinePosition = -1;

            // Search for information
            // None                         -2
            // _FirstLineToShow - 1         -1
            // _FirstLineToShow              0
            // _FirstLineToShow + 1          1
            // _FirstLineToShow + 2          2
            // _FirstLineToShow + 3          3
            int LineOfInformationPosition = -2;

            int[] LinesNr = new int[4];

            #region Line layout

            if (_FirstLineToShow % 4 == 0)
            {
                // First line is active                
                LinePosition = 0;
                // Position possible for LineOfInformationPosition

                y1 = y0;                            //          _FirstLineToShow              current         (update 3 & 4)
                y2 = y0 + _lineHeight;              // idx2     _FirstLineToShow + 1      inactive
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 2      inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 3      inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow;
                LinesNr[1] = _FirstLineToShow + 1;
                LinesNr[2] = _FirstLineToShow + 2;
                LinesNr[3] = _FirstLineToShow + 3;

            }
            else if (_FirstLineToShow % 4 == 1)
            {
                // 2nd line is active
                LinePosition = 1;
                // Position not possible for LineOfInformationPosition

                y2 = y0;                            // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + _lineHeight;              //          _FirstLineToShow             current         (no update)
                y3 = y0 + 2 * _lineHeight;          // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + 3 * _lineHeight;          // idx4     _FirstLineToShow + 2     inactive

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow - 1;
                LinesNr[1] = _FirstLineToShow;
                LinesNr[2] = _FirstLineToShow + 1;
                LinesNr[3] = _FirstLineToShow + 2;

            }
            else if (_FirstLineToShow % 4 == 2)
            {
                // 3rd line is active
                LinePosition = 2;
                // Position possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 2     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 3     inactive
                y1 = y0 + 2 * _lineHeight;          //          _FirstLineToShow             current         (update 1 & 2)
                y2 = y0 + 3 * _lineHeight;          // idx2     _FirstLineToShow + 1     inactive

                idx2 = _FirstLineToShow + 1;
                idx3 = _FirstLineToShow + 2;
                idx4 = _FirstLineToShow + 3;

                LinesNr[0] = _FirstLineToShow + 2;
                LinesNr[1] = _FirstLineToShow + 3;
                LinesNr[2] = _FirstLineToShow;
                LinesNr[3] = _FirstLineToShow + 1;


            }
            else if (_FirstLineToShow % 4 == 3)
            {
                // 4th line is active
                LinePosition = 3;
                // Position not possible for LineOfInformationPosition

                y3 = y0;                            // idx3     _FirstLineToShow + 1     inactive
                y4 = y0 + _lineHeight;              // idx4     _FirstLineToShow + 2     inactive
                y2 = y0 + 2 * _lineHeight;          // idx2     _FirstLineToShow - 1     * active
                y1 = y0 + 3 * _lineHeight;          //          _FirstLineToShow             current         (no update)

                idx2 = _FirstLineToShow - 1;
                idx3 = _FirstLineToShow + 1;
                idx4 = _FirstLineToShow + 2;

                LinesNr[0] = _FirstLineToShow + 1;
                LinesNr[1] = _FirstLineToShow + 2;
                LinesNr[2] = _FirstLineToShow - 1;
                LinesNr[3] = _FirstLineToShow;


            }

            #endregion Line layout


            bool bTooMuch = false;
            for (int i = 0; i < LinesNr.Length; i++)
            {
                if (LinesNr[i] >= _kLyrics.Lines.Count)
                {
                    bTooMuch = true;
                    break;
                }
            }

            if (!bTooMuch)
                LineOfInformationPosition = SearchLineOfInformation(LinesNr);
            else
                LineOfInformationPosition = LastLineOfInformationPosition;


            // If no line of information in the 4 lines => normal display in 4 lines swapped
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                bInstrumentalStarted = false;
                bCountDown = false;


                // Draw y1 line: active & highlighted line                
                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);

                // Line y2 must be drawned active when
                bool IsActive = ((_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3)) ? true : false;

                // Draw y2 line: active when before line y1 (already sung), inactive when after line y1 (not yet sung)
                if (idx2 >= 0)
                    DrawInactiveLineWithNeon(e, idx2, y2, IsActive);


                // Draw lines y3 and y4 (always inactives)
                if (idx3 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithNeon(e, idx3, y3);
                if (idx4 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithNeon(e, idx4, y4);
                
                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();


                TimeSpan tm = DateTime.Now - _startTime;

                switch (LineOfInformationPosition)
                {
                    #region instrumental on top

                    case 0:                             // Instrumental is on line 0
                        // y1 * information
                        // y2 information
                        // y3 normal
                        // y4 normal
                        switch (LinePosition)
                        {
                            case 0:
                                // y1 * information1 new
                                // y2 information2   new
                                // y3 normal old than new
                                // y4 normal old than new
                                // Draw "(intrumental)" on active line and countdown on next line
                                //DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, y1);
                                //DrawInformation(e, SecondsBeforeSinging.ToString(), y1 + _lineHeight);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 2, y3, true);         // keep old line 1 sec
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 1, y4, true);         // keep olf line 1 sec

                                        }
                                    }
                                }

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                // Except if introduction (_FirstLineToShow = 0) show
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }

                                DrawInactiveLineWithNeon(e, idx3, y3);      // Draw new line before the end of instrumental         
                                DrawInactiveLineWithNeon(e, idx4, y4);      // Draw new line before the end of instrumental
                                break;

                            case 1:
                                // y2 information1
                                // y1 * information2 no update
                                // y3 normal 
                                // y4 normal 

                                // draw ("instrumental") on previous line and countdown on current line
                                //DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text, y1 - _lineHeight);
                                //DrawInformation(e, SecondsBeforeSinging.ToString(), y1);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1 - _lineHeight);

                                DrawInactiveLineWithNeon(e, idx3, y3);
                                DrawInactiveLineWithNeon(e, idx4, y4);
                                break;

                            case 2:
                                // y3 information1 new
                                // y4 information2 new
                                // y1 * normal old          update y3 & y4 
                                // y2 normal   old

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithNeon(e, idx2, y2, false);

                                // Draw instrumental on line 0 (y3)
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 3:
                                // y3 information1
                                // y4 information2
                                // y2 normal
                                // y1 * normal          no update

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);
                                // Draw y2 line: inactive line  (before or after y1)                                
                                DrawInactiveLineWithNeon(e, idx2, y2, true);

                                // Draw instrumental on line 0
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;
                        }
                        break;

                    #endregion Instrumental on top


                    #region Instrumental on bottom

                    case 2:                                                         // instrumental on line 2 (3rd line)
                        // y1 normal
                        // y2 normal
                        // y3 * information1
                        // y4 information2
                        switch (LinePosition)
                        {
                            case 0:                                                 // LinePosition is 0
                                // y1 * normal
                                // y2 normal
                                // y3 information1
                                // y4 information2

                                // Draw y1 line: active & highlighted line                
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);
                                // line y 2 is inactive (not yet played)
                                DrawInactiveLineWithNeon(e, idx2, y2, false);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);
                                break;

                            case 1:                                                 // LinePosition is 1
                                // y1 normal
                                // y2 * normal
                                // y3 information1
                                // y4 information2

                                // line y2 is already played
                                DrawInactiveLineWithNeon(e, idx2, y2, true);
                                // line y1 : active a highlighted line
                                DrawActiveLineWithNeon(e, _FirstLineToShow, y1);

                                // Draw "(intrumental)" on 3rd line and countdown on next line
                                DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, y3);

                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                //DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, y1);
                                //DrawInformation(e, SecondsBeforeSinging.ToString(), y2);
                                DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = _endTime - DateTime.Now;

                                    if (tm.TotalMilliseconds > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines
                                        return;
                                    }
                                }
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithNeon(e, idx3, y3);
                                if (idx4 < _kLyrics.Lines.Count)
                                    DrawInactiveLineWithNeon(e, idx4, y4);

                                break;

                            case 3:
                                // y1 normal
                                // y2 normal
                                // y3 information1
                                // y4 * information2
                                // Draw "(intrumental)" on active line and countdown on next line                                
                                //DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text, y2);
                                //DrawInformation(e, SecondsBeforeSinging.ToString(), y1);
                                DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text + " " + SecondsBeforeSinging.ToString(), y2);

                                DrawInactiveLineWithNeon(e, idx3, y3);
                                DrawInactiveLineWithNeon(e, idx4, y4);

                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }

            LastLineOfInformationPosition = LineOfInformationPosition;

        }

        #endregion Draw text with Four lines swapped


        #region Draw text with fixed lines

        /// <summary>
        /// Draw text with fixed lines
        /// </summary>
        /// <param name="e"></param>
        private void DrawTextWithFixedLines(PaintEventArgs e)
        {
            switch (FrameType)
            {
                case "NoBorder":
                case "FrameThin":
                case "Frame1":
                case "Frame2":
                case "Frame3":
                case "Frame4":
                case "Frame5":
                    FixDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    FixDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    FixDrawTextWithNeon(e);
                    break; ;

                default:
                    FixDrawTextWithBorder(e);
                    break;
            }
        }
        
        /// <summary>
        /// Fixed lines: Draw text with various borders 
        /// </summary>
        /// <param name="e"></param>
        private void FixDrawTextWithBorder(PaintEventArgs e)
        {
            
            // Center text vertically
            int y0 = VCenterText();

            // Draw active line with borders
            DrawActiveLineWithBorders(e, _FirstLineToShow, y0);


            // Draw Inactive lines with borders
            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                DrawInactiveLineWithBorders(e, i, y0 + (i - _FirstLineToShow) * _lineHeight);
            }                      
        }

        /// <summary>
        /// Fixed lines: Draw text with Shadow effect
        /// </summary>
        /// <param name="e"></param>
        private void FixDrawTextWithShadow(PaintEventArgs e)
        {
            // Center text vertically          
            int y0 = VCenterText();

            // Draw active line with shadow
            DrawActiveLineWithShadow(e, _FirstLineToShow, y0);

            // Draw Inactive lines with shadow
            GraphicsPath path = new GraphicsPath();            
            Brush InactiveColorBrush = new SolidBrush(InactiveColor);            
            Pen ActiveBorderPen = new Pen(new SolidBrush(ActiveBorderColor), _borderthick);
            Pen InactiveBorderPen = new Pen(new SolidBrush(InactiveBorderColor), _borderthick);

            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                DrawInactiveLineWithShadow(e, i, y0 + (i - _FirstLineToShow) * _lineHeight);               
            }

            // Draw the text                    
            e.Graphics.FillPath(InactiveColorBrush, path);

            // Outline the text                    
            e.Graphics.DrawPath(InactiveBorderPen, path);
            
            #region Clean all                 
            InactiveColorBrush.Dispose();
            ActiveBorderPen.Dispose();
            InactiveBorderPen.Dispose();
            path?.Dispose();
            #endregion Clean all
        }

        /// <summary>
        /// Fixed lines: Draw text with neon effect
        /// </summary>
        /// <param name="e"></param>
        private void FixDrawTextWithNeon(PaintEventArgs e)
        {
            // Center text vertically 
            int y0 = VCenterText();

            // Draw active line with Neon
            DrawActiveLineWithNeon(e, _FirstLineToShow, y0);

            
            // Draw Inactive lines with Neon
            GraphicsPath path = new GraphicsPath();

            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {                
                DrawInactiveLineWithNeon(e, i, y0 + (i - _FirstLineToShow) * _lineHeight);            
            }

            Brush InactiveColorBrush = new SolidBrush(InactiveColor);            
            Brush InactiveColorBorderBrush = new SolidBrush(InactiveBorderColor);            
            Pen InactiveBorderPen = new Pen(InactiveColorBorderBrush, _borderthick);

            // Draw the text            )
            e.Graphics.FillPath(InactiveColorBrush, path);

            // Outline the text                    
            e.Graphics.DrawPath(InactiveBorderPen, path);

            #region Clean up resources
            path.Dispose();
            InactiveColorBorderBrush.Dispose();
            InactiveColorBrush.Dispose();
            InactiveBorderPen?.Dispose();
            #endregion Clean up resources

        }

        #endregion Draw text with fixed lines


        #region Effects

        /// <summary>
        /// Create a neon effect
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="e"></param>
        /// <param name="pth"></param>
        private void CreateNeonEffect(Color clr, PaintEventArgs e, GraphicsPath pth)
        {
            //Create a bitmap in a fixed ratio to the original drawing area.
            Bitmap bm = new Bitmap(pBox.ClientSize.Width / 5, pBox.ClientSize.Height / 5);
            //Get the graphics object for the image. 
            Graphics gimg = Graphics.FromImage(bm);

            //Create a matrix that shrinks the drawing output by the fixed ratio. 
            Matrix mx = new Matrix(1.0f / 5, 0, 0, 1.0f / 5, -(1.0f / 5), -(1.0f / 5));

            //Choose an appropriate smoothing mode for the halo. 
            gimg.SmoothingMode = SmoothingMode.AntiAlias;

            //Transform the graphics object so that the same half may be used for both halo and text output. 
            gimg.Transform = mx;

            //Using a suitable pen...
            Color HaloColor = clr;
            Brush HaloBrush = new SolidBrush(HaloColor);

            Pen penHaloColor = new Pen(HaloColor, 3);

            //Draw around the outline of the path
            gimg.DrawPath(penHaloColor, pth);

            //and then fill in for good measure. 
            gimg.FillPath(HaloBrush, pth);

            //We no longer need this graphics object
            //g.Dispose();

            //setup the smoothing mode for path drawing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //and the interpolation mode for the expansion of the halo bitmap
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //expand the halo making the edges nice and fuzzy. 
            e.Graphics.DrawImage(bm, pBox.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// Create a shadow effect
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="font"></param>
        /// <param name="e"></param>
        /// <param name="pth"></param>
        private void CreateShadowEffect(string line, Color clr, int x0, int y0, Font font, float emSize, PaintEventArgs e, GraphicsPath pth)
        {
            Bitmap bm = new Bitmap(pBox.ClientSize.Width / 4, pBox.ClientSize.Height / 4);

            //Get a graphics object for it
            Graphics g = Graphics.FromImage(bm);
            //Graphics ge = e.Graphics;

            // must use an antialiased rendering hint
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //this matrix zooms the text out to 1/4 size and offsets it by a little right and down                
            Matrix mx = new Matrix(0.25f, 0, 0, 0.25f, 1.3f, 1.3f);

            g.Transform = mx;


            //The shadow is drawn
            g.DrawString(line, font, new SolidBrush(clr), x0, y0, sf);

            //Don't need this anymore
            g.Dispose();

            //The destination Graphics uses a high quality mode
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //and draws antialiased text for accurate fitting
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            //The small image is blown up to fill the main client rectangle
            e.Graphics.DrawImage(bm, pBox.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);

            // finally, the text is drawn on top
            //pth.AddString(line, new FontFamily(font.Name), (int)FontStyle.Regular, emSize, new Point(x0, y0), sf);
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

        #endregion Effects


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

        #endregion Control Load Resize paint


        #region Get infos


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

        /// <summary>
        /// Search for biggest line
        /// </summary>
        /// <returns></returns>
        private string GetBiggestLine()
        {
            float max = 0;
            string maxline = string.Empty;
            float L = 0;            
            for (int i = 0; i < _kLyrics.Lines.Count(); i++)
            {
                L = MeasureLine(i);

                // Search line having max characteres
                if (L > max)
                {
                    max = L;
                    maxline = _kLyrics.Lines[i].ToString();
                }
            }
            return maxline;
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
                    //for (int i = 0; i < Lines.Count; i++)
                    for (int i = 0; i < _kLyrics.Lines.Count; i++) 
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


        #region Receive pos from player

        /// <summary>
        ///  player position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPos(double ms)
        {
            // Store player position (ms)
            PlayerPositionMilliseconds = ms;

            SetPosition((int)ms);

        }

        /// <summary>
        /// Calculate fragments, length of fragments
        /// </summary>
        /// <param name="pos"></param>
        private void SetPosition(int pos)
        {
            if (_kLyrics.Lines.Count == 0) return;
            
            // Search _line & index of the next lyric to play
            (_FirstLineToShow, nextindex) = GetNextIndex(pos);

            
            // CurLength:
            // Mesure length of a portion of line (already sung + being sung)
            // used to display the percentage of syllables completed in the line

            // active_fragment, active_fragment_length          part of the line already sung
            // highlight_fragment, highlight_fragment_length    part of the line being sung 
            // inactive_fragment, inactive_fragment_length      part of the line not yet sung
            CurLength = GetCurLength(_FirstLineToShow, nextindex);


            // New word to highlight
            // Warning: in case of full lines, nextindex is allways the same and not different than lastIndex
            if (nextindex != lastindex || _FirstLineToShow != _lastLine)
            {
                // Line changed
                if (_FirstLineToShow != _lastLine)
                {
                    _lastLine = _FirstLineToShow;
                    percent = 0;
                    lastpercent = 0;
                    nextindex = 0;
                    lastindex = 0;
                    lastCurLength = 0;
                    CurLength = 0;

                    _lasttime = _nexttime;
                }

                //_FirstLineToShow = _line;
                _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

                


                // StartTime of nextindex
                if (nextindex < _kLyrics.Lines[_FirstLineToShow].Syllables.Count())                
                    _nexttime = _kLyrics.Lines[_FirstLineToShow].Syllables[nextindex].StartTime;
                

                // Save last value of percent
                lastpercent = percent;

                // Set new value of percent to the end of the previous word
                // And after that, add a small progressive increment in order to increase the percentage

                // |--- last word ---|--- new word --------------------------|
                //                   | percent => percent+pas => percent+pas
                    
                if (_FirstLineToShow < LinesLengths.Count())
                    percent = (lastCurLength / LinesLengths[_FirstLineToShow]);


                // CurLength is the percentage of syllables completed in the line (already sung + being sung)
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
                        _steppercent = (float)(_nexttime - _lasttime) / (d * (float)_timerintervall);
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

                if (percent > (CurLength / LinesLengths[_FirstLineToShow]))
                    percent = (CurLength / LinesLengths[_FirstLineToShow]);
                
                pBox.Invalidate();                
            }
        }


        /// <summary>
        /// Retrieve nextindex of current syllabe in the current line
        /// </summary>
        /// <returns></returns>       
        private (int line, int index) GetNextIndex(int pos)
        {
            if (_kLyrics.Lines.Count == 0) return (0, 0);
            
            // Descending loop for lines
            for (int j = _kLyrics.Lines.Count - 1; j >= 0; j--)
            {
                // Descending loop for Syllables
                for (int i = _kLyrics.Lines[j].Syllables.Count - 1; i >= 0; i--)
                {
                    // Search first index of Syllable for which pos is greater then startTime  
                    if (_kLyrics.Lines[j].Syllables[i].StartTime > 0 && pos > _kLyrics.Lines[j].Syllables[i].StartTime)
                    {
                        // line 10      1000 1500 2000          index 1 2 3
                        // line 11      2500 3000 3500          index 4 5 6

                        // For ex pos = 2600 => pos > 2500 (index  4)
                        // => line = 11 & next index = 5                    OK

                        // For ex pos = 2200 => pos > 2000 (index 3)
                        // => line == 10 & next index = 3 + 1  = 4 which is not in this line
                        // => line = 11 & next index = 0

                        if (i + 1 < _kLyrics.Lines[j].Syllables.Count)
                            return (j, i + 1);
                        else
                        {
                            // End of line
                            // i + 2 seems to work, but why ?
                            return (j, i + 2);
                        }                                                
                    }
                }
            }
            return (0, 2);
        }



        /// <summary>
        /// Mesure length of a portion of line (already sung + being sung)
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private float GetCurLength(int curline, int nextindex)
        {
            float res = 0;

            active_fragment = string.Empty;
            active_fragment_length = 0;
            highlight_fragment = string.Empty;
            highlight_fragment_length = 0;
            inactive_fragment = string.Empty;
            inactive_fragment_length = 0;

            if (_kLyrics.Lines.Count == 0) return 0;

            // Search for the current line
            for (int i = 0; i < _kLyrics.Lines[curline].Syllables.Count(); i++)
            {
                // Fragments before nextindex
                if (i < nextindex)
                {
                    res += MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);

                    if (nextindex >= 0 && i < nextindex - 1)
                    {
                        // Already sung
                        active_fragment += _kLyrics.Lines[curline].Syllables[i].Text;
                        active_fragment_length += MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);
                    }
                    else if (nextindex > 0 && i == nextindex - 1)
                    {
                        // Being sung
                        highlight_fragment = _kLyrics.Lines[curline].Syllables[i].Text;
                        highlight_fragment_length = MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);
                    }
                }
                else if (i >= nextindex)
                {
                    inactive_fragment += _kLyrics.Lines[curline].Syllables[i].Text;
                    inactive_fragment_length += MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);                    
                }
            }

            //if (inactive_fragment.Length > 0) 
            //    Console.WriteLine("inactive_fragment = " + inactive_fragment);

            return res;
        }

        #endregion Receive pos from player


        #region start stop

        // Start Display lyrics
        public void Start()
        {
            
            SecondsBeforeSinging = 0;
            bInstrumentalStarted = false;
            bCountDown = false;
            _endTime = DateTime.Now;
            _startTime = DateTime.Now;
            PlayerPositionMilliseconds = 0;
            TargetPositionMilliseconds = 0;

            _FirstLineToShow = 0;

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
            
            SecondsBeforeSinging = 0;
            bInstrumentalStarted = false;
            bCountDown = false;
            _endTime = DateTime.Now;
            _startTime = DateTime.Now;
            PlayerPositionMilliseconds = 0;
            TargetPositionMilliseconds = 0;

            _FirstLineToShow = 0;
            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

            percent = 0;
            lastpercent = 0;
            nextindex = 0;
            lastindex = 0;
            _lasttime = 0;
            lastCurLength = 0;
            CurLength = 0;

            active_fragment = string.Empty;
            highlight_fragment = string.Empty;
            inactive_fragment = string.Empty;

            pBox.Invalidate();            
        }

     

        #endregion start stop


        #region SlideShow

        private void LoadImageList(string dir)
        {

            // Add to m_ImageFilePaths string array the list of paths of images in the directory
            m_ImageFilePaths = Directory.GetFiles(@dir, "*.jpg");           

            count = 0;
            //mBlend = 0.0F;

            // Add to m_BitmapsArray array the list of images in the directory
            // Initialize the array of images with the size of the number of images in the directory
            m_BitmapsArray = new Bitmap[m_ImageFilePaths.Length];
            for (int i = 0; i < m_ImageFilePaths.Length; ++i)
            {
                m_BitmapsArray[i] = new Bitmap(m_ImageFilePaths[i]);
            }

        }

        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void SetBackground(string dirImages)
        {
            if (dirImages == null || !Directory.Exists(dirImages))
            {
                pBox.BackColor = Color.Black;
                return;
            }
            
            try
            {
                m_CurrentImage = null;
                pBox.Image = null;
                pBox.Invalidate();                
                                
                m_ImageFilePaths = new string[] { };
                m_BitmapsArray = new Bitmap[] { };

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
                        C = m_BitmapsArray.Length;
                    }

                    switch (C)
                    {
                        case 0:
                            // No image, just background color                            
                            break;
                        case 1:
                            // Single image                            
                            pBox.Image = m_BitmapsArray[0]; //  Image.FromFile(m_ImageFilePaths[0]);
                            break;
                        default:                            
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
                Image1 = m_BitmapsArray[count];
                Image2 = m_BitmapsArray[++count];
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


                if ((count + 1) < m_BitmapsArray.Length)
                {
                    Image1 = m_BitmapsArray[count];
                    Image2 = m_BitmapsArray[++count];
                }
                else
                {
                    Image1 = m_BitmapsArray[count];
                    Image2 = m_BitmapsArray[0];
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

        #endregion SlideShow


        #region Terminate

        /// <summary>
        /// Terminate
        /// </summary>
        public void Terminate()
        {
            m_ImageFilePaths = new string[] { };
            m_BitmapsArray = new Bitmap[] { };

            timerChangeImage?.Stop();
            timerTransition?.Stop();
        }

        #endregion Terminate


    }
}
