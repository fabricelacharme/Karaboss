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

namespace PicControl
{

    public delegate void DoubleClickEventHandler(object sender, EventArgs e);
    public delegate void CloseEventHandler(object sender, EventArgs e);
    public delegate void FullScreenEventHandler(object sender, EventArgs e);
    public delegate void OptionsEventHandler(object sender, EventArgs e);
    public delegate void TopMostEventHandler(object sender, bool bTopMost, EventArgs e);

    public partial class pictureBoxControl : UserControl, IMessageFilter, IDisposable
    {
        /*
         * timer5_Tick de frmMidiPlayer appelle la fonction colorLyric de frmMidiLyrics 
         * La fenetre frmMidiLyrics appelle la fonction ColorLyric(songposition) de picturebox control
         * Si songposition <> currenttextpos (syllabe active a changé) => redessine
         */

                      
        #region classes

        // Syllabes
        public class syllabe
        {
            public string chord;        // Chord to play with this syllabe
            public string text;         // piece of text (text of Syllabe)
            public int time;            // temps de la syllabe 
            public int line;            // num de ligne  
            public int posline;         // position dans la ligne
            public int pos;             // position dans la chanson
            public int SylCount;        // Nombre de syllabes sur la meme ligne
            public int last;            // position derniére syllabe
            public int offset;          // offset horizontal
        }

        #endregion classes


        #region Colors

        #region Background color

        // Background color        
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

        /// <summary>
        /// Transparency color
        /// </summary>
        private Color _transparencykey = Color.Lime;
        public Color TransparencyKey
        {
            get { return _transparencykey; }
            set { _transparencykey = value; }
        }

        #endregion Background color


        #region Chord color

        // Display chords or not
        public bool OptionShowChords { get; set; }

        /// <summary>
        /// Text to sing color
        /// </summary>
        private Color _InactiveChordColor;
        public Color InactiveChordColor
        {
            get
            { return _InactiveChordColor; }
            set
            {
                _InactiveChordColor = value;
                pBox.Invalidate();
            }
        }

        /// <summary>
        /// Chord Highligt color
        /// </summary>
        private Color _HighlightChordColor;
        public Color HighlightChordColor
        {
            get
            { return _HighlightChordColor; }
            set
            {
                _HighlightChordColor = value;
                pBox.Invalidate();
            }
        }

        private bool _bShowChords = false;
        public bool bShowChords
        {
            get { return _bShowChords; }
            set
            {
                if (value != _bShowChords)
                {
                    _bShowChords = value;
                    pBox?.Invalidate();
                }
            }
        }

        #endregion Chord color


        #region Gradient color

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
       
        #endregion Gradient color


        #region Instrumentals color

        private Color _ActiveInstrumentalColor;
        public Color ActiveInstrumentalColor
        {
            get { return _ActiveInstrumentalColor; }
            set
            {
                _ActiveInstrumentalColor = value;
                pBox.Invalidate();
            }
        }

        #endregion Instrumentals color


        #region TextColor

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

        #endregion Textcolor       

        #endregion Colors


        #region Draw filename

        private bool _bShowSongName = true;
        public bool bShowSongName           
        {
            get { return _bShowSongName; }
            set
            {
                _bShowSongName = value;
                pBox.Invalidate();
            }
        }

        private string _fileName;
        public string FileName                  // Name of the song to display on the screen (Filename without extension)
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                if (_bShowSongName)
                    pBox.Invalidate();
            }
        }

        #endregion Draw filename


        #region Draw syllables

        private float _AverageWidth;

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


        public Rectangle m_DisplayRectangle { get; set; }

        private List<RectangleF> rRect;
        //private List<RectangleF> rNextRect;

        private int _currentPosition;
        public int CurrentTime
        {
            get
            { return _currentPosition; }
            set
            {
                _currentPosition = value;
            }
        }

        public int _currentTextPos;
        public int CurrentTextPos
        {
            get
            { return _currentTextPos; }
            set
            {
                _currentTextPos = value;
            }
        }

        private int vOffset = 0;

        private bool bEndOfLine = false;
        private bool bHighLight = false;
        private int nextStartOfLineTime = 0;
        private int TimeToNextLineDuration = 0;


        private List<syllabe> syllabes;
        private List<string> lstLyricsLines;    // Liste de lignes
        private List<string> lstChordsLines;    // List of lines of chords (same number of lines as lstLyricsLines but with chords instead of lyrics)


        private int currentLine = 0;
        private string lineMax; // Ligne longueur max                             

        #endregion Draw syllables


        #region Events

        public new event DoubleClickEventHandler DoubleClick;
        public event CloseEventHandler Close;
        public event FullScreenEventHandler FullScreen;
        public event OptionsEventHandler Options;
        public event TopMostEventHandler TopMost;

        #endregion Events


        #region Font

        private string ftName = "Arial Black";
        private uint ftSize = 20;

        private Font m_font;
        private float emSize = 40; // Size of the font

        private StringFormat sf;

        private Font _karaokeFont;
        [Description("Karaoke font")]
        public Font KaraokeFont
        {
            get { return _karaokeFont; }
            set
            {
                try
                {
                    _karaokeFont = value;
                    pBox.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error setting karaoke font: " + ex.Message);
                }
            }
        }

        private Font _chordFont;
        public Font ChordFont
        {
            get { return _chordFont; }
            set
            {
                try
                {
                    _chordFont = value;
                    pBox.Invalidate();
                }
                catch (Exception e)
                {
                    Console.Write("Error: " + e.Message);
                }
            }
        }

        // Font stretching (None, Small (no stetching), Medium (some stretching), Large (most stretching)
        private string _FontStretching = "None";
        public string FontStretching
        {
            get { return _FontStretching; }
            set
            {
                _FontStretching = value;
                pBox.Invalidate();
            }
        }
        #endregion Font


        #region Form

        private bool bTopMostChecked = true;
        
        #region Context menus
        private ContextMenu picContextMenu;
        #endregion Context menus


        #region Is used for settings?

        private bool _bIsSettings = false;
        [Description("When true, pictureBoxControl is used in a settings window")]
        public bool bIsSettings
        {
            get { return _bIsSettings; }
            set { _bIsSettings = value; }
        }

        #endregion Is used for settings?


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

        #endregion Form


        #region Gradients

        readonly System.Windows.Forms.Timer _timerGradient = new System.Windows.Forms.Timer();

        // Default angle for the gradient

        private int W;
        private int H;
        private int speed;

        private int _beat = 200;
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


        // Used by Rhythm to display different effect according to beat number
        private int _beatNumber = 1;

        // Used by beat effect
        private int _bpm;

        #endregion Gradients


        #region Instrumentals

        // Show hints for instrumental parts (e.g. display "(introduction, instrumental, ending)" on the screen)
        private bool _bShowHints = true;
        public bool bShowHints
        {
            get { return _bShowHints; }
            set
            {
                _bShowHints = value;
                pBox.Invalidate();
            }
        }

        private int _endTime;               // used by countdown
        private int _startTime;             // used by countdown

        private int PlayerPositionTicks;    // current player position in ticks
        private int TargetPositionTicks;    // position to reach in ticks

        private bool bInstrumentalStarted = false;
        private int SecondsBeforeSinging = 0;
        private bool bCountDown = false;
        private int _DelayBeforeEndOfInstrumental = 0; // Delay to draw lines before the end of an instrumental: 4 sec
        private int _MinimumInstrumentalDuration = 0;  // The minimum duration between two consecutive vocal phrases that mark an instrumental interlude : 5 sec
        private int LastLineOfInformationPosition = 0; // Used to store the last valid Instrumental line position (to manage end of song)
        
        private int _MinimumIntroDuration = 0;          // 3 sec minimum duration for an intro (to avoid counting a short instrumental at the beginning of the song as an intro)

        #endregion Instrumentals


        #region Karaoke display layout

        // Fixed lines, scrolling lines, 4 lines swapped, 2 lines swapped
        private kar.KaraokeDisplayTypes _karaokeDisplayType = KaraokeDisplayTypes.FixedLines;
        public kar.KaraokeDisplayTypes KaraokeDisplayType
        {
            get { return _karaokeDisplayType; }
            set
            {
                _karaokeDisplayType = value;

                if (_kLyricsOrg != null)
                {
                    _kLyrics = _kLyricsOrg.Clone();
                    Init();
                }
                pBox.Invalidate();
            }
        }

        #endregion Karaoke display layout


        #region Karaoke Lyrics

        private kLyrics _kLyricsOrg;
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
                _kLyricsOrg = _kLyrics.Clone();
                if (_kLyrics != null && _kLyrics.Lines.Count > 0)
                    Init();
            }
        }

        #endregion Karaoke Lyrics


        #region Margins

        // Margins and spacing. General margins are defined as a ratio of the control size to be adaptable to all sizes of control. Some specific margins are defined in pixels to be more precise when needed
        private float _lineHeightMultiplier = 1.55f;   // Ration between line spacing and font size. 1.55 is the default value for a single line, but it can be increased to have more space between lines when several lines are displayed        
        private float _marginLeft = 0.03f;   // Margin left for lyrics (ratio of the width of the control)
        private float _marginTop = 0.36f;    // Margin top for lyrics when 4 lines swapped are displayed (ratio of the height of the control)

        // Only used for FourLinesSwapped layout: Additional line spacing between 2 firsts lines and 2 last lines
        private float _fourLinesSpacing = 1.17f;   // Multiplier to apply to line spacing when 4 lines swapped are displayed (instead of 1.55) to avoid too much space between lines. 

        // Positioning for drawing the FileName if option selected.
        private float _titleMaxLength = 0.41f;
        private float _titleMarginLeft = 0.58f;
        private float _titleMarginTop = 0.038f;

        #endregion Margins


        #region MIDI

        // Ticks for 1 sec
        private int TicksPerSecond = 0; // TotalTicks / Duration

        // Duration in seconds 
        private double _duration;
        public double Duration
        {
            get { return _duration; }
            set {

                if (value > 0)
                {
                    _duration = value;
                    if (_TotalTicks > 0)
                    {
                        TicksPerSecond = (int)(_TotalTicks / _duration);
                        ResetDefaultTimings();
                    }
                }
            }
        }

        private int _TotalTicks;
        public int TotalTicks
        {
            get { return _TotalTicks; }
            set 
            {
                if (value > 0)
                {
                    _TotalTicks = value;
                    if (_duration > 0)
                    {
                        TicksPerSecond = (int)(_TotalTicks / _duration);
                        ResetDefaultTimings();
                    }
                }
            }
        }

        private int _FirstMelodyNoteTicksOn = 0;
        public int FirstMelodyNoteTicksOn
        {
            get { return _FirstMelodyNoteTicksOn; }
            set
            {
                _FirstMelodyNoteTicksOn = value;                
            }
        }


        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set
            {
                if (value > 0)
                {
                    _beatDuration = value;
                }
            }
        }

        #endregion MIDI


        #region Picture

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

        
        public Image m_CurrentImage { get; set; }

        #endregion Picture


        #region Slideshow

        private string[] bgFiles;
        private string DefaultDirSlideShow;
        // Paths of images
        private List<string> m_ImageFilePaths;
        // Array of bitmaps (images as backgound image)
        private Bitmap[] m_BitmapsArray;


        #region Select background  

        // Background option : image, diaporama, solidColor, transparent 
        private string _optionbackground = "Image";
        public string OptionBackground
        {
            get { return _optionbackground; }
            set
            {
                _optionbackground = value;

                switch (_optionbackground)
                {
                    case "Image":
                        SetImageBackground(_SingleImagePath);
                        pBox.Invalidate();
                        break;

                    case "Diaporama":
                        //if (_dirSlideShow != null && Directory.Exists(_dirSlideShow) && _freqSlideShow > 0)
                        //    SetDirectoryBackground(_dirSlideShow);
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

        #endregion Select background


        #region Single image
        // Display a single image as background        
        private string _SingleImagePath = string.Empty;
        public string SingleImagePath
        {
            get => _SingleImagePath;
            set
            {
                if (File.Exists(value) && value != _SingleImagePath)
                {
                    _SingleImagePath = value;
                    SetImageBackground(_SingleImagePath);
                    pBox.Invalidate();
                }
            }
        }

        #endregion Single image


        #region SlideShow images
        
        /*
        // SlideShow directory        
        private string _dirSlideShow;
        public string DirSlideShow
        {
            get
            { return _dirSlideShow; }
            set
            {
                if (value == null) return;
                if (value != _dirSlideShow)
                {
                    _dirSlideShow = value;

                    SetDirectoryBackground(_dirSlideShow);
                    pBox.Invalidate();                    
                }
            }
        }
        */
        
        // SlideShow frequency        
        private int _freqSlideShow;
        public int FreqSlideShow
        {
            get
            { 
                return _freqSlideShow; 
            }
            set
            {
                _freqSlideShow = value;                
            }
        }

        #endregion SlideShow images

        
        #region Transition effect

        System.Timers.Timer  timerTransition; 
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

        #endregion Transition effect      

        #endregion slideshow


        #region Text transform

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


        #region Text position        
        // Display lyrics option: top, Center, Bottom        
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

        #endregion Text position


        #region Internal lyrics separators

        private string _InternalSepLines = "¼";
        private string _InternalSepParagraphs = "½";

        #endregion

        private int _nbLyricsLinesOrg;
        private int _nbLyricsLines = 3;
        [Description("number of lines to display")]
        public int nbLyricsLines
        {
            get
            { return _nbLyricsLines; }
            set
            {
                _nbLyricsLines = value;
                _nbLyricsLinesOrg = value;
                if (bIsSettings)
                    Init();
                ajustTextAgain();
                pBox.Invalidate();
            }
        }
        
        private bool _bforceUppercase;
        [Description("force uppercase for lyrics")]
        public bool bforceUppercase
        {
            get { return _bforceUppercase; }
            set
            {
                _bforceUppercase = value;
                if (_bIsSettings)
                    LoadDemoText();
            }
        }
             
        private bool _bshowparagraphs = true;
        [Description("show a blank line between paragraphs")]
        public bool bShowParagraphs
        {
            get { return _bshowparagraphs; }
            set { _bshowparagraphs = value; }
        }

        // Draw a black background beside text?
        private bool _bTextBackGround = true;
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set
            {
                _bTextBackGround = value;
                pBox.Invalidate();
            }
        }


        #region Lyrics transition effects

        private float percent = 0;
        private float lastpercent = 0;

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

        // Speed of progressive color of lyric being sung used by _steppercent
        private long _timerintervall = 50;      
        public long timerIntervall
        {
            get { return _timerintervall; }
            set
            {

                if (value >= 10)
                    _timerintervall = value;
            }
        }


        #endregion Lyrics transition effects        

        #endregion Text transform       


        #region Vertical scrolling

        private float[] linesYCoordinates;
        private float vposition = 0;

        #endregion Vertical scrolling


        /// <summary>
        /// Constructor of pictureBoxControl
        /// </summary>
        public pictureBoxControl()
        {
            InitializeComponent();

           
            #region Move form without title bar

            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.pBox);

            #endregion


            #region Graphic optimization 
            /*
            this.SetStyle(
                  System.Windows.Forms.ControlStyles.UserPaint |
                  System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                  System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                  true);
            */
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            #endregion Graphic optimization

            SetDefaultValues();

            if (_kLyrics != null && _kLyrics.Lines.Count > 0)
                Init();
        }


        #region Ajust text deprecated

        /// <summary>
        /// Ajuste la taille de la fonte en fonction de la taille de pictureBox1
        /// </summary>
        /// <param name="S"></param>
        private void AjustText(string S)
        {
            if (S != "" && pBox != null)
            {
                Graphics g = pBox.CreateGraphics();
                float femsize;

                long inisize = (long)pBox.Font.Size;
                femsize = g.DpiX * inisize / 72;

                float textSize = MeasureString(S, femsize);
                long comp = (long)(0.94 * pBox.ClientSize.Width);

                // Texte trop large
                if (textSize > comp)
                {
                    do
                    {
                        inisize--; //= inisize - 1;
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

                if (_bShowChords)
                {
                    // FAB CHORD
                    totaltextHeight = (int)2.5 * totaltextHeight;
                }

                long compHeight = (long)(0.95 * pBox.ClientSize.Height);

                if (totaltextHeight > compHeight)
                {
                    do
                    {
                        inisize--; //= inisize - 1;
                        if (inisize > 0)
                        {
                            femsize = g.DpiY * inisize / 72;
                            textHeight = MeasureStringHeight(S, femsize);

                            totaltextHeight = _nbLyricsLines * (textHeight + 10);
                            if (_bShowChords)
                            {
                                // FAB CHORD
                                totaltextHeight = (int)2.5 * totaltextHeight;
                            }

                        }
                    } while (totaltextHeight > compHeight && inisize > 0);
                }


                if (inisize > 0)
                {
                    emSize = g.DpiY * inisize / 72;
                    m_font = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);
                    pBox.Font = new Font(Name = _karaokeFont.Name, emSize);

                    // Vertical distance between lines
                    // https://pimpmytype.com/line-length-line-height/ they say 1.6 is the best                         
                    //_lineHeight = (int)emSize + 10;                    
                    _lineHeight = (int)(1.55 * emSize);

                    // Height of the full song
                    _linesHeight = _nbLyricsLines * _lineHeight;

                    // Calculate the length of each line
                    for (int i = 0; i < LinesLengths.Length; i++)
                    {
                        LinesLengths[i] = MeasureLine(i);
                    }

                }
                g.Dispose();
            }
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

        /// <summary>
        /// Get offset to center text
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        private int HCenterText(string tx, float femsize)
        {
            float ret;
            float L = MeasureString(tx, femsize);
            float W = pBox.ClientSize.Width;

            ret = (W - L) / 2;

            if (ret < 0)
                ret = 0;

            return (int)ret;
        }

        /// <summary>
        /// Get offset height
        /// </summary>
        /// <param name="femsize"></param>
        /// <returns></returns>
        private int getOffsetHeight(float femsize)
        {
            float ret = 0;

            float h = MeasureStringHeight("ABCDEFGHIJKLMNOPQRSTUVWXYZ", femsize);
            long H = (long)pBox.ClientSize.Height;

            switch (_OptionDisplay)
            {
                case OptionsDisplay.Top:
                    if (_nbLyricsLines == 1)
                        ret = 10;
                    else
                        ret = _lineHeight;
                    break;

                case OptionsDisplay.Center:
                    if (_nbLyricsLines == 1)
                    {
                        ret = (H - ((_nbLyricsLines) * (h + 10))) / 2;
                    }
                    else
                    {
                        if (_bShowChords)
                            ret = (H - ((2 * _nbLyricsLines - 1) * (h + 10))) / 2;
                        else
                        {
                            //ret = (H - ((_nbLyricsLines - 1) * (h + 10))) / 2;

                            ret = (H - (_nbLyricsLines * _lineHeight)) / 2;

                        }
                    }
                    break;

                case OptionsDisplay.Bottom:
                    if (_bShowChords)
                        ret = (H - (int)(2.5 * _nbLyricsLines) * _lineHeight) - 10;
                    else
                        ret = (H - _nbLyricsLines * _lineHeight) - 10;

                    break;
            }

            return (int)ret;
        }

        /// <summary>
        /// Measure the length of a string
        /// </summary>
        /// <param name="line"></param>
        /// <param name="fSize"></param>
        /// <returns></returns>
        private float MeasureString2(string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pBox.CreateGraphics())
                {
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
        private float MeasureStringHeight2(string line, float femSize)
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
        /// Return the line with maxi number of characters
        /// </summary>
        /// <returns></returns>
        private string GetMaxLength()
        {
            int max = 0;
            string tx = string.Empty;

            //for (int i = 0; i < lstLyricsLines.Count; i++)
            for (int i = 0; i < _kLyrics.Lines.Count; i++)
            {

                if (_kLyrics.Lines[i].ToString().Length > max)
                {
                    max = _kLyrics.Lines[i].ToString().Length; // lstLyricsLines[i].Length;
                    tx = _kLyrics.Lines[i].ToString();
                }
            }
            return tx;
        }

        #endregion Ajust text deprecated


        #region Context menu
        private void pBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (_bIsSettings) return;


            if (e.Button == MouseButtons.Right)
            {
                picContextMenu = new ContextMenu();
                picContextMenu.MenuItems.Clear();

                // Close
                MenuItem mnuClose = new MenuItem("Close");
                mnuClose.Click += new System.EventHandler(this.mnuClose_Click);
                picContextMenu.MenuItems.Add(mnuClose);

                // Full screen
                MenuItem mnuFullScreen = new MenuItem("FullScreen");
                mnuFullScreen.Click += new System.EventHandler(this.mnuFullScreen_Click);
                picContextMenu.MenuItems.Add(mnuFullScreen);


                // Top most
                MenuItem mnuTopMost = new MenuItem("TopMost");
                mnuTopMost.Click += new System.EventHandler(this.mnuTopMost_Click);
                mnuTopMost.Checked = bTopMostChecked;
                picContextMenu.MenuItems.Add(mnuTopMost);


                // Options
                MenuItem mnuOptions = new MenuItem("Options");
                mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
                picContextMenu.MenuItems.Add(mnuOptions);

                this.ContextMenu = picContextMenu;
            }
        }


        private void mnuTopMost_Click(object sender, EventArgs e)
        {
            TopMost?.Invoke(this, bTopMostChecked, e);
            bTopMostChecked = !bTopMostChecked;
        }

        private void mnuOptions_Click(object sender, EventArgs e)
        {
            Options?.Invoke(this, e);
        }

        private void mnuFullScreen_Click(object sender, EventArgs e)
        {
            FullScreen?.Invoke(this, e);
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            Close?.Invoke(this, e);
        }

        #endregion Context menu


        #region Control load and resize

        /// <summary>
        /// picturebox resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxWnd_Resize(object sender, EventArgs e)
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


            if (this.ParentForm != null && this.ParentForm.WindowState != FormWindowState.Minimized)
            {
                //ajustTextAgain();
                AdjustFontSize(_nbLyricsLines);


                if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
                    InitScrollMode();

                pBox.Invalidate();
            }

            #region redraw image
            if (m_CurrentImage != null)
            {

                //m_DisplayRectangle = GetRectangleForSizeMode(m_CurrentImage.Width, m_CurrentImage.Height);

                /*
                int x;
                int y;

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
                */
            }
            #endregion

        }

        #endregion Control load and resize


        #region demo, wait

        /// <summary>
        /// Count Down: decreasing numbers to wait for next song to start 
        /// </summary>
        /// <param name="sec">Count down max </param>
        public void LoadWaitSong(int sec)
        {
            _nbLyricsLines = 1;
            //_dirSlideShow = null;
            SetDirectoryBackground(null);

            // Initial position
            _currentTextPos = -1;
            vOffset = 0;
            nextStartOfLineTime = 0;


            List<string> lines = new List<string>();
            // 10|9|8|7|6|5|4|3|2|1|0|       
            for (int i = sec; i >= 0; i--)
            {
                lines.Add(i.ToString());
            }

            // Do not use KLyrics but _kLyrics to be able to use the same LoadSong method for demo and real text
            _kLyrics = StoreDemoText(lines, 500);
            Init(true);
        }

        public void endDemoText()
        {
            syllabes = null;
        }



        /// <summary>
        /// Display a text from another windows form (used in playlists to display song title and artist during the wait time before the song starts)
        /// </summary>
        /// <param name="tx"></param>
        public void DisplayText(string tx, int ticks = 0)
        {
            List<string> lines = new List<string>();

            string[] ArrayLines = tx.Split(_InternalSepLines.ToCharArray());
            for (int i = 0; i < ArrayLines.Length; i++)
            {
                lines.Add(ArrayLines[i]);
            }


            //List<plLyric> plLyrics = StoreDemoText(tx, ticks);
            _kLyrics = StoreDemoText(lines, ticks);

            Init();

            // Initial position
            _currentPosition = 0;
            currentLine = 1;
            _currentTextPos = 0;

            pBox.Invalidate();
        }

        #endregion demo wait


        #region Events

        private void pboxWnd_DoubleClick(object sender, EventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        #endregion Events


        #region Initializations

        private void ResetDefaultTimings()
        {
            // Calculate ticks per second
            if (_duration > 0 && _TotalTicks > 0)
            {
                TicksPerSecond = (int)(_TotalTicks / _duration);

                _DelayBeforeEndOfInstrumental = 4 * TicksPerSecond;
                _MinimumInstrumentalDuration = 6 * TicksPerSecond;
                _MinimumIntroDuration = 3 * TicksPerSecond;
            }
            else
            {
                MessageBox.Show("Invalid Duration", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// Set default values for demonstration purpose
        /// </summary>
        private void SetDefaultValues()
        {

            m_ImageFilePaths = new List<string>();
            m_BitmapsArray = new Bitmap[] { };
            

            #region Font

            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
            _karaokeFont = new Font("Arial Black", emSize, FontStyle.Regular, GraphicsUnit.Pixel);

            #endregion Font


            #region Chords   

            OptionShowChords = true;
            _chordFont = new Font("Comic Sans MS", this._karaokeFont.Size);

            #endregion Chords


            #region Gradient colors

            Beat = 200; // Default speed for rhythm animation

            _timerGradient.Interval = 60; // 60 ms
            _timerGradient.Tick += new EventHandler(_timerGradient_Tick);

            #endregion Gradient colors


            #region Initial text position
            
            _currentPosition = 30;
            currentLine = 1;
            _currentTextPos = 2;

            #endregion Initial text position


            pBox.Invalidate();
        }


        public void LoadDemoText()
        {
            List<string> lines = new List<string>
            {
                "Lorem ipsum dolor sit amet,",
                "consectetur adipisicing elit,",
                "sed do eiusmod tempor incididunt",
                "ut labore et dolore magna aliqua.",
                "Ut enim ad minim veniam,",
                "quis nostrud exercitation ullamco",
                "laboris nisi ut aliquip",
                "ex ea commodo consequat.",
                "Duis aute irure dolor in reprehenderit",
                "in voluptate velit esse cillum dolore",
                "eu fugiat nulla pariatur.",
            };
            
            _kLyrics = StoreDemoText(lines, 500);

            // Load song with demo text
            Init(true);
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
            kLine kLine; // = new kLine();
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
                if (!(kls.Lines[i].Syllables.Count == 1 && kls.Lines[i].Syllables.First().CharType == Syllable.CharTypes.ParagraphSep))
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
        /// Search for Introduction, Instrumentals
        /// </summary>
        /// <param name="kls"></param>
        /// <returns></returns>       
        private kLyrics SearchForInstrumentals(kLyrics kls)
        {
            int tOnPrevious = 0;
            int duration = 0;
            int t;
            kLyrics klsWithinstrumentals = new kLyrics();
            kLine line;            
            int tend; 

            
            if (TicksPerSecond == 0)
            {
                MessageBox.Show("Invalid Duration", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return kls;
            }

            // Introduction                        
            for (int i = 0; i < kls.Lines.Count; i++)
            {
                line = new kLine();
                for (int j = 0; j < kls.Lines[i].Syllables.Count; j++)
                {
                    if (kls.Lines[i].Syllables[j].CharType != Syllable.CharTypes.ParagraphSep) 
                    { 
                        
                        t = kls.Lines[i].Syllables[j].TicksOn;                        

                        // Create two lines for introduction (if no syllable at t = 0)                        
                        if (i == 0 && j == 0)
                        {
                            #region Introduction
                            // Start of intro
                            line.Add(new Syllable() { Text = "(introduction)", TicksOn = 0, CharType = Syllable.CharTypes.Information });
                            klsWithinstrumentals.Add(line);

                            if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                            {
                                // end of intro = just before first lyric
                                tend = t;
                                if (tend > _MinimumIntroDuration)
                                    tend = tend - _MinimumIntroDuration;
                                line = new kLine();
                                line.Add(new Syllable() { Text = "", TicksOn = tend, CharType = Syllable.CharTypes.Information });
                                klsWithinstrumentals.Add(line);
                            }
                            line = new kLine();
                            #endregion Introduction
                        }                        
                        else if (t > _FirstMelodyNoteTicksOn && t - tOnPrevious > _MinimumInstrumentalDuration)
                        {
                            // Instrumental must be on line 0 or 2 => create additional blank lines in order to have instrumental on the right position

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
                                    line.Add(new Syllable() { Text = "", TicksOn = t + 45, CharType = Syllable.CharTypes.Information });
                                    klsWithinstrumentals.Add(line);

                                    line = new kLine();

                                }
                                else if (c % 4 == 3)
                                {
                                    if (line.Syllables.Count > 0)
                                        klsWithinstrumentals.Add(line);

                                    // If on line 3
                                    line = new kLine();
                                    line.Add(new Syllable() { Text = "", TicksOn = t + 45, CharType = Syllable.CharTypes.Information });
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
                            line.Add(new Syllable() { Text = "(instrumental)", TicksOn = tOnPrevious + duration, CharType = Syllable.CharTypes.Information });
                            klsWithinstrumentals.Add(line);

                            if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                            {
                                // Second line instrumental, 2 seconds before the end
                                tend = t;
                                if (tend - 2 * TicksPerSecond > 0)
                                    tend = tend - 2 * TicksPerSecond;

                                line = new kLine();
                                line.Add(new Syllable() { Text = "", TicksOn = tend, CharType = Syllable.CharTypes.Information });
                                klsWithinstrumentals.Add(line);
                            }

                            line = new kLine();
                        }

                        tOnPrevious = t;    // Start time of previous lyric
                        duration = kls.Lines[i].Syllables[j].TicksOff - kls.Lines[i].Syllables[j].TicksOn; // Duration of previous lyric
                    }
                    line.Add(kls.Lines[i].Syllables[j]);
                }
                klsWithinstrumentals.Add(line);
            }
            
            #region ENDING
            
            t = _kLyrics.Lines.Last().Syllables.Last().TicksOn;

            if (_TotalTicks - t > _MinimumInstrumentalDuration)
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
                        line.Add(new Syllable() { Text = "", TicksOn = t + 45, CharType = Syllable.CharTypes.Information });
                        klsWithinstrumentals.Add(line);
                    }
                    else if (c % 4 == 3)
                    {
                        // If on line 3
                        line = new kLine();
                        line.Add(new Syllable() { Text = "", TicksOn = t + 45, CharType = Syllable.CharTypes.Information });
                        klsWithinstrumentals.Add(line);
                    }
                }

                #region ENDING
                // First line
                line = new kLine();
                line.Add(new Syllable() { Text = "(ending)", TicksOn = t + duration, CharType = Syllable.CharTypes.Information });
                klsWithinstrumentals.Add(line);

                if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
                {
                    // 2nd line 1 sec before the end of the song
                    tend = _TotalTicks - TicksPerSecond;
                    line = new kLine();
                    line.Add(new Syllable() { Text = "", TicksOn = tend, CharType = Syllable.CharTypes.Information });
                    klsWithinstrumentals.Add(line);
                }
                #endregion ENDING
            }

            #endregion ENDING

            return klsWithinstrumentals;
        }

        private kLyrics AddTrailingSyllable(kLyrics kls)
        {
            kLyrics klsWithTrailingSyllable = new kLyrics();
            kLine line;
            Syllable syll;
            int ticksOn; // = 0;
            int ticksOff; // = 0;

            for (int i = 0; i < kls.Lines.Count; i++)
            {
                line = new kLine();
                
                for (int j = 0; j < kls.Lines[i].Syllables.Count; j++)
                {
                    line.Add(kls.Lines[i].Syllables[j]);    
                }

                // Add a new syllable when line of Text
                if (kls.Lines[i].Syllables.Last().CharType == Syllable.CharTypes.Text)
                {
                   
                    if (i + 1 < kls.Lines.Count)
                    {
                        if (kls.Lines[i + 1].Syllables.First().TicksOn > kls.Lines[i].Syllables.Last().TicksOff)
                        {

                            ticksOn = kls.Lines[i].Syllables.Last().TicksOff + 1;
                            //ticksOff = ticksOn;                                                        
                            ticksOff = kls.Lines[i + 1].Syllables.First().TicksOn - 1;
                            
                            syll = new Syllable() { Text = " ", TicksOn = ticksOn, TicksOff = ticksOff, CharType = Syllable.CharTypes.Text };
                            line.Add(syll);                        
                        }
                       
                    } 
                }

                klsWithTrailingSyllable.Add(line);
            }

            return klsWithTrailingSyllable;
        }


        /// <summary>
        /// Load text of song
        /// </summary>
        /// <param name="toto"></param>     
        private void Init(bool bDemoMode = false)
        {
            if (_kLyrics == null) return;
            if (_kLyrics.Lines == null) return;
            if (_kLyrics.Lines.Count == 0) return;

            #region Karaoke display type
            // Update _nbLyricsLines if layout changed in options            
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
                    //_nbLyricsLines = _nbLyricsLinesOrg;
                    _nbLyricsLines = 6;
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    //_nbLyricsLines = _nbLyricsLinesOrg;
                    _nbLyricsLines = 6;
                    break;
                default:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
            }
            #endregion Karaoke display type

            // Do not display paragraphs for some cases
            if (!_bIsSettings && 
                  (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped
                || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown
                || !bShowParagraphs
                ))            
                _kLyrics = RemoveParagraphs(_kLyrics);
            

            // If Upper case required
            if (_bforceUppercase)
                _kLyrics = ForceUpperCase(_kLyrics);


            // Analyse lyrics to find introduction, instrumentals etc..
            if (!_bIsSettings && 
                  (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped
                || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown
                ))
                _kLyrics = SearchForInstrumentals(_kLyrics);


            // Add a syllable to each end of lines
            _kLyrics = AddTrailingSyllable(_kLyrics);
          

            lstLyricsLines = new List<string>();
            lstChordsLines = new List<string>();
            LinesLengths = new float[_kLyrics.Lines.Count];
            syllabes = new List<syllabe>();

            if (_kLyrics != null && _kLyrics.Count > 0)
            {
                // store lines in a specific list
                if (_kLyrics != null)
                {
                    lstLyricsLines = StoreLyricsLines(_kLyrics);
                    lstChordsLines = StoreChordLines(_kLyrics);
                }

                LinesLengths = new float[_kLyrics.Lines.Count];
                _biggestLine = GetBiggestLine();
                AdjustFontSize(_nbLyricsLines);

                // ajust font size
                //lineMax = GetMaxLength();
                //AjustText(lineMax);



                // Store syllabes                
                if (_kLyrics != null)
                    syllabes = StoreLyricsSyllabes(_kLyrics);

                if (bDemoMode)
                {
                    bHighLight = true;
                }
                else
                {
                    bHighLight = false;
                    // Position initiale                 
                    _currentTextPos = -1;
                }

                // Create rectangles for drawing active line
                createListRectangles(0);

                if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
                    InitScrollMode();
            }

        }


        #endregion Initializations


        #region Instrumental functions

        /// <summary>
        /// Search for a line containing an instrumental
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private int SearchLineOfInformation(int[] lines)
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

                    if (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp)
                    {
                        if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                            TargetPositionTicks = _kLyrics.Lines[_FirstLineToShow + 1].Syllables.First().TicksOn;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionTicks = _TotalTicks;
                    }
                    else if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                    {
                        // (_FirstLineToShow + 1 cannot be used because it is the 2nd line of information)
                        if (_FirstLineToShow + 2 < _kLyrics.Lines.Count)
                            TargetPositionTicks = _kLyrics.Lines[_FirstLineToShow + 2].Syllables.First().TicksOn;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionTicks = _TotalTicks;                                                  // Position in the song to reach = end of song
                    }                    

                    _startTime = PlayerPositionTicks;
                    SecondsBeforeSinging = (TargetPositionTicks - PlayerPositionTicks) / BeatDuration; //  (int)tm.TotalSeconds;
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
                // Recalculates the remaining time with PlayerPosition
                _endTime = TargetPositionTicks - PlayerPositionTicks;

                
                if (_endTime < 0)
                {
                    _endTime = 0;
                    _startTime = 0;
                    bInstrumentalStarted = false;
                    SecondsBeforeSinging = -1;
                    bCountDown = false;
                }
                else
                {
                    // Calculate countdown                    
                    int s = _endTime / TicksPerSecond;

                    if (s != SecondsBeforeSinging)
                    {
                        SecondsBeforeSinging = s;
                    }
                }
            }
        }

        #endregion Instrumental functions


        #region Lyrics and position        

        /// <summary>
        ///  player position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPos(int ticks)
        {
            // Store player position (ms)
            _currentPosition = ticks;

            PlayerPositionTicks = ticks;

            // new from mp3 module
            SetPosition(ticks);

            // old
            SetOffset();
        }

        /// <summary>
        /// Calculate fragments, length of fragments
        /// </summary>
        /// <param name="pos"></param>
        private void SetPosition(int pos)
        {
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
            // Descending loop for lines
            for (int j = _kLyrics.Lines.Count - 1; j >= 0; j--)
            {
                // Descending loop for Syllables
                for (int i = _kLyrics.Lines[j].Syllables.Count - 1; i >= 0; i--)
                {
                    // Search first index of Syllable for which pos is greater then startTime  
                    if (_kLyrics.Lines[j].Syllables[i].TicksOn > 0 && pos > _kLyrics.Lines[j].Syllables[i].TicksOn)
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
                        //active_fragment_length += MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);
                    }
                    else if (nextindex > 0 && i == nextindex - 1)
                    {
                        // Being sung
                        highlight_fragment = _kLyrics.Lines[curline].Syllables[i].Text;
                        //highlight_fragment_length = MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);
                    }
                }
                else if (i >= nextindex)
                {
                    inactive_fragment += _kLyrics.Lines[curline].Syllables[i].Text;
                    //inactive_fragment_length += MeasureString(_kLyrics.Lines[curline].Syllables[i].Text, _karaokeFont.Size);
                }
            }

            active_fragment_length = MeasureString(active_fragment, _karaokeFont.Size);
            highlight_fragment_length = MeasureString(highlight_fragment, _karaokeFont.Size);
            inactive_fragment_length = MeasureString(inactive_fragment, _karaokeFont.Size);

            return res;
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

        #endregion Lyrics and position
       

        #region public methods

        #region Move Windows

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

        #endregion MoveWindows


        private void SetImageBackground(string ImagePath)
        {
            try
            {
                if (!File.Exists(ImagePath))
                {
                    pBox.BackColor = Color.Black;
                    return;
                }
               
                m_ImageFilePaths.Clear();
                m_ImageFilePaths.Add(ImagePath);
                m_CurrentImage = Image.FromFile(m_ImageFilePaths[0]);

            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
        }


        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void SetDirectoryBackground(string dirImages)
        {
            try
            {                           
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
                        C = m_BitmapsArray.Length;
                    }

                    switch (C)
                    {
                        case 0:
                            // No image, just background color
                            break;
                        case 1:
                            m_CurrentImage = Image.FromFile(m_ImageFilePaths[0]);
                            break;
                        default:
                            InitSlideShow();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
        }

             
        /// <summary>
        /// Guess if picturebox should be paint.
        /// Paint should be done only if syllable has changed
        /// </summary>
        private void SetOffset()
        {
            int ctp = findPosition(_currentPosition);  // index syllabe à chanter
            int newvOffset; // = 0;

            // If vertical Offset change => redraw
            // Time to next line            
            float CurrentTimeToNextLineDuration = nextStartOfLineTime - _currentPosition;
            if (CurrentTimeToNextLineDuration > 0 && TimeToNextLineDuration > 0)
            {
                // As time passes, CurrentTimeToNextLineDuration decreases, so newvOffset increases
                newvOffset = Convert.ToInt32(_lineHeight - (CurrentTimeToNextLineDuration / TimeToNextLineDuration) * _lineHeight);
                if (newvOffset > vOffset)
                {
                    vOffset = newvOffset;
                }
            }

            // If syllabe change => redraw
            if (ctp != _currentTextPos)
            {
                if (bEndOfLine)
                {
                    bEndOfLine = false;
                    vOffset = 0;
                }
                _currentTextPos = ctp;
            }
          
            // Redraw the display
            pBox.Invalidate();
        }

        /// <summary>
        /// Find index of syllabe to sing according to time
        /// TODO : remove chords ?
        /// </summary>
        /// <param name="itime"></param>
        /// <returns></returns>
        private int findPosition(int itime)
        {
            if (syllabes == null)
                return 0;

            int x0 = 0;

            // optimisation : partir de la dernière position connue si le temps de celle-ci est inférieur au temps actuel
            if (_currentTextPos > 0 && _currentTextPos < syllabes.Count && syllabes[_currentTextPos].time < itime)
                x0 = _currentTextPos - 1;

            for (int i = x0; i < syllabes.Count; i++)
            {
                syllabe syllab = syllabes[i];

                // cherche la première syllabe dont le temps est supérieur à itime
                // prend la précédente
                if (itime < syllab.time)
                {

                    if (i > 0 && syllab.posline == 0 && syllab.SylCount == 1)
                    {
                        //  LRC : 1 ligne = 1 seule syllabe                        
                        bHighLight = true;
                        return i - 1;
                    }
                    else if (i > 0 && syllab.posline == 0 && itime > syllabes[i - 1].time + 2 * _beatDuration)
                    {
                        // Cas 1 : La première syllabe dont le temps est supérieur au temps courant est située sur la prochaine ligne
                        // Cela signifie que l'on vient de jouer la dernière syllabe de la ligne.
                        // Si "fin de ligne" et temps écoulé supérieur à 2 noires
                        // prendre la première syllabe dont le temps est supérieur au temps courant, soit l'indice "i"
                        // indiquer également qu'il ne faut pas encore colorer cette syllabe
                        // On force le changement de ligne au bout de 2 temps                       
                        bHighLight = false;   // Ne pas mettre en surbrillance la syllabe tant que son temps n'est pas arrivé
                        return i;
                    }
                    else
                    {
                        // Sinon prendre "i - 1" et la syllabe doit être colorée
                        bHighLight = true;
                        return i - 1;
                    }
                }
            }

            return syllabes.Count - 1;
        }

        /// <summary>
        /// Reset display at begining
        /// </summary>
        public void ResetTop()
        {            
            bEndOfLine = false;

            SecondsBeforeSinging = -1;
            bInstrumentalStarted = false;
            bCountDown = false;
            _endTime = 0;
            _startTime = 0;
            _FirstLineToShow = 0;

            vOffset = 0;
            nextStartOfLineTime = 0;

            _currentPosition = 0;
            _currentTextPos = -1;
            pBox.Invalidate();
        }
              
        

        #endregion public methods
                     

        #region measure

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
                    if (bShowSongName)
                        y = (int)(_titleMarginTop * pBox.Height);
                    else
                        y = 0;
                    break;

                case OptionsDisplay.Bottom:
                    y = pBox.ClientSize.Height - (_nbLyricsLines * (_lineHeight + 1));
                    break;
            }
            return y > 0 ? y : 0;
        }


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


        private float GetAverageWidth(kLyrics kls, float femSize)
        {
            float L = 0;

            if (pBox == null) return 0;
            if (kls == null || kls.Lines.Count == 0) return 0;

            // Calculation of the average length of the lines
            for (int i = 0; i < kls.Lines.Count(); i++)
            {
                L += MeasureString(kls.Lines[i].ToString(), femSize);
            }
            return L / kls.Lines.Count;
        }


        /// <summary>
        /// Ajust font size to fit NbLines in height and the average line lenght in width
        /// Bigger lines will be shrinked and smaller lines not changed
        /// </summary>
        /// <param name="pBox"></param>
        /// <param name="S"></param>
        /// <param name="NbLines"></param>
        private void AdjustFontSize(int NbLines)
        {
            if (FontStretching == "Large")
                AdjustFontSizeWithStretching(NbLines);
            else
            {
                AdjustFontWithoutStretching(_biggestLine, NbLines);
            }

        }


        private void AdjustFontWithoutStretching(string biggestLine, int NbLines)
        {
            if (pBox == null) return;
            if (_kLyrics == null || _kLyrics.Lines.Count == 0) return;
            if (biggestLine == string.Empty) return;

            string S = biggestLine;

            Graphics g = pBox.CreateGraphics();
            float femsize;
            float inisize = _karaokeFont.Size;
            femsize = g.DpiY * inisize / 72;


            float mult = 1.3f; // 1.2 is the default line spacing in Windows Forms
            float textWidth = MeasureString(S, femsize);

            // Try to fit inside 90% of client width
            float ClientWidth = 0.90f * pBox.ClientSize.Width;

            if (textWidth > ClientWidth)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiY * inisize / 72;
                        textWidth = MeasureString(S, femsize);

                    }
                } while (textWidth > ClientWidth && inisize > 0);
            }
            else
            {
                do
                {
                    inisize++;
                    femsize = g.DpiY * inisize / 72;
                    textWidth = MeasureString(S, femsize);
                } while (textWidth < ClientWidth);
            }

            // ------------------------------
            // Ajustement in Height
            // ------------------------------
            float textHeight = MeasureStringHeight(S, inisize);
            float totaltextHeight;
            totaltextHeight = _nbLyricsLines * (textHeight + 10);

            float compHeight = 0.95f * pBox.ClientSize.Height;

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
                emSize = g.DpiX * inisize / 72;
                _karaokeFont = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);

                // Vertical distance between lines          1.6 is
                // https://pimpmytype.com/line-length-line-height/ they say 1.6 is the best 
                _lineHeight = (int)(_lineHeightMultiplier * emSize);
                // Height of the full song
                _linesHeight = _nbLyricsLines * _lineHeight;


                // Update horizontal measure of lines                
                for (int i = 0; i < _kLyrics.Lines.Count; i++)
                {
                    LinesLengths[i] = MeasureString(_kLyrics.Lines[i].ToString(), _karaokeFont.Size);
                }

            }
            g.Dispose();
        }

        private void AdjustFontSizeWithStretching(int NbLines)
        {
            if (pBox == null) return;
            if (_kLyrics == null || _kLyrics.Lines.Count == 0) return;

            // Calculate Font size as if there is only 6 lines to display in order to have bigger font size.
            if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
                NbLines = 6;


            string S = "!/(123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Graphics g = pBox.CreateGraphics();
            float femsize;
            float inisize = _karaokeFont.Size;
            femsize = g.DpiY * inisize / 72;

            // Try to fit inside 90% of client Height
            float ClientHeight = pBox.ClientSize.Height;

            float mult = 1.3f; // 1.2 is the default line spacing in Windows Forms
            float textHeight = MeasureStringHeight(S, femsize);
            float linesHeight = mult * textHeight * NbLines;


            if (linesHeight > ClientHeight)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiY * inisize / 72;
                        linesHeight = mult * MeasureStringHeight(S, femsize) * NbLines;

                    }
                } while (linesHeight > ClientHeight && inisize > 0);
            }
            else
            {
                do
                {
                    inisize++;
                    femsize = g.DpiY * inisize / 72;
                    linesHeight = mult * MeasureStringHeight(S, femsize) * NbLines;
                } while (linesHeight < ClientHeight);
            }

            // ------------------------------
            // Ajustement in width with AverageWidth
            // ------------------------------

            // Calculate average width of lines and try to fit inside 95% of client width
            femsize = g.DpiX * inisize / 72;
            _AverageWidth = GetAverageWidth(_kLyrics, femsize);

            float ClientWidth = (1 - 2 * _marginLeft) * pBox.ClientSize.Width;
            float textWidth = _AverageWidth;

            if (_AverageWidth > ClientWidth)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiX * inisize / 72;
                        textWidth = GetAverageWidth(_kLyrics, femsize);
                    }
                } while (textWidth > ClientWidth && inisize > 0);
            }


            if (inisize > 0)
            {
                emSize = g.DpiX * inisize / 72;
                _karaokeFont = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);

                // Vertical distance between lines          1.6 is
                // https://pimpmytype.com/line-length-line-height/ they say 1.6 is the best 
                _lineHeight = (int)(_lineHeightMultiplier * emSize);
                // Height of the full song
                _linesHeight = _nbLyricsLines * _lineHeight;


                // Update horizontal measure of lines                
                for (int i = 0; i < _kLyrics.Lines.Count; i++)
                {
                    LinesLengths[i] = MeasureString(_kLyrics.Lines[i].ToString(), _karaokeFont.Size);
                }

            }
            g.Dispose();
        }


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


        private string GetBiggestLine()
        {
            int max = 0;
            string tx = string.Empty;

            for (int i = 0; i < _kLyrics.Lines.Count; i++)
            {
                if (_kLyrics.Lines[i].ToString().Length > max)
                {
                    max = _kLyrics.Lines[i].ToString().Length; // lstLyricsLines[i].Length;
                    tx = _kLyrics.Lines[i].ToString();
                }
            }
            return tx;
        }

        #endregion measure

                             
        #region Paint Control

        /// <summary>
        /// picturebox Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxWnd_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background image
            #region draw background image or gradient

            // Create a GraphicsPath to define the area to fill
            GraphicsPath gp;

            switch (_optionbackground) 
            {

                case "Image":
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
                    break;

                case "SolidColor":                    
                    e.Graphics.FillRectangle(new SolidBrush(_BgColor), new Rectangle(0, 0, this.Width, this.Height));
                    break;

                case "Diaporama":                                                                             
                    if (m_BitmapsArray != null && m_BitmapsArray.Length == 1)
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
                    int d = Math.Min(2*W, 2*H);

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
            
            #endregion
            

            #region draw text           

            if (lstLyricsLines is null || lstLyricsLines.Count == 0)
                return;
            

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


            #endregion

            // Call the base class OnPaint method to ensure proper rendering            
            base.OnPaint(e);
        }


        #region draw lyrics & chords

        /// <summary>
        /// Draw current line, syllabe by syllabe
        /// already sung: _ActiveColor
        /// Currently sung: _HighlightColor
        /// Not yet sung: _InactiveColor
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="syl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="e"></param>
        private void drawSyllabe(string kind, Color clr, syllabe syl, int x0, int y0, int W, int H, PaintEventArgs e)
        {
            var pth = new GraphicsPath();
            string tx = syl.text;

            Color BorderColor = _ActiveBorderColor;

            switch (kind)
            {
                case "Active":
                case "Highlight":
                    // outline            
                    BorderColor = _ActiveBorderColor;
                    break;
                case "Inactive":
                    // outline            
                    BorderColor = _InactiveBorderColor;
                    break;
            }
            Pen penContour = new Pen(BorderColor, _borderthick);

            try
            {

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    // Black background to make text more visible
                    RectangleF R = new RectangleF(x0, y0, W, H);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), R);
                }
                #endregion

                #region Draw text of syllabe

                // Add syllable to the Graphics path
                pth.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, y0), sf);

                #region Apply effects               

                if (FrameType == "Neon")
                    CreateNeonEffect(BorderColor, e, pth);
                else if (FrameType == "Shadow")
                    CreateShadowEffect(tx, BorderColor, x0, y0, m_font, emSize, e, pth);

                #endregion Apply effect

                // Draw text
                e.Graphics.FillPath(new SolidBrush(clr), pth);

                // Outline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penContour, pth);

                pth.Dispose();
                #endregion
            }
            catch (Exception ed)
            {
                Console.Write("Error: " + ed.Message);
            }
        }

        /// <summary>
        /// Draw chords on current line
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="syl"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="e"></param>
        private void drawChord(Color clr, syllabe syl, int x0, int y0, PaintEventArgs e)
        {
            var path = new GraphicsPath();
            string tx = syl.chord;

            try
            {
                #region Draw text of chord                
                path.AddString(tx, _chordFont.FontFamily, (int)_chordFont.Style, 3 * emSize / 4, new Point((int)x0, y0), sf);
                e.Graphics.FillPath(new SolidBrush(clr), path);

                path.Dispose();
                #endregion
            }
            catch (Exception ed)
            {
                Console.Write("Error: " + ed.Message);
            }
        }


        /// <summary>
        /// Draw syllabes of the current line according to its position
        /// already sung, currently sung and not yet sung
        /// </summary>
        /// <param name="itime"></param>
        /// <param name="e"></param>
        private void DrawCurrentLine(int itime, int y0, PaintEventArgs e)
        {
            if (syllabes == null) return;

            int W;
            int H;

            float x1;
            //string tx; // = string.Empty;
            int x0 = 0;
            int i;
            syllabe syllab;
            int offset = _lineHeight;

            if (_currentTextPos >= syllabes.Count)
                return;

            if (_currentTextPos >= 0)
                x0 = _currentTextPos - syllabes[_currentTextPos].posline;


            for (i = x0; i < syllabes.Count; i++)
            {
                // dessine la ligne courante en mettant en surbrillance la syllabe correspondante à currentTextPos
                syllab = syllabes[i];

                // It is the current line
                if (syllab.line == currentLine)
                {

                    // Rectangle
                    x1 = rRect[syllab.posline].X;
                    W = (int)rRect[syllab.posline].Width;
                    H = (int)rRect[syllab.posline].Height;

                    if (syllabes[i].pos < _currentTextPos)
                    {
                        // syllabes avant celle active
                        if (_bShowChords)
                        {
                            if (syllab.chord != "")
                                drawChord(_InactiveChordColor, syllab, (int)x1, y0, e);

                            drawSyllabe("Active", _ActiveColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                            // déjà chanté
                        }
                        else
                        {
                            drawSyllabe("Active", _ActiveColor, syllab, (int)x1, y0, W, H, e);                                            // déjà chanté
                        }
                    }
                    else if (syllab.pos == _currentTextPos)
                    {

                        // Surbrillance normale   
                        if (bHighLight)
                        {
                            if (_bShowChords)
                            {
                                if (syllab.chord != "")
                                    drawChord(_HighlightChordColor, syllab, (int)x1, y0, e);

                                drawSyllabe("Highlight", _HighlightColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                       // surbrillance
                            }
                            else
                            {
                                drawSyllabe("Highlignt", _HighlightColor, syllab, (int)x1, y0, W, H, e);                                         // surbrillance     
                            }
                        }
                        else
                        {
                            if (_bShowChords)
                            {
                                if (syllab.chord != "")
                                    drawChord(_InactiveChordColor, syllab, (int)x1, y0, e);

                                drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);
                            }
                            else
                            {
                                drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y0, W, H, e);
                            }
                        }


                        #region EndOfLine & calculations

                        // Calculations are made on active syllabe (celle qui correspond à currentpos !)

                        // End of line
                        if (syllab.SylCount == 1)
                        {
                            bEndOfLine = true;
                        }
                        else if (syllab.SylCount > 1 && syllab.posline == syllab.SylCount - 1)
                        {
                            bEndOfLine = true;
                        }
                        else
                        {
                            bEndOfLine = false;


                            #region calculate next line

                            // Start of line
                            if (syllab.posline == 0)
                            {
                                // calculate next time for next start of line                           
                                int next = syllab.SylCount;
                                if (i + next < syllabes.Count)
                                {
                                    // Time of start of next line
                                    // Soit le début de la prochaine ligne, soit la fin de la ligne courante
                                    int t1 = syllabes[i + next].time;
                                    int t2 = syllabes[i + next - 1].time + 2 * _beatDuration; // on passe à la ligne au bout de 2 temps

                                    nextStartOfLineTime = t1 < t2 ? t1 : t2;

                                    // Duration until next line 
                                    TimeToNextLineDuration = nextStartOfLineTime - _currentPosition;
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    // syllabes après celle active
                    else
                    {
                        if (_bShowChords)
                        {
                            if (syllab.chord != "")
                                drawChord(_InactiveChordColor, syllab, (int)x1, (int)y0, e);

                            drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                           // pas encore chanté
                        }
                        else
                        {
                            drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y0, W, H, e);                                      // pas encore chanté
                        }
                    }
                }
                // Ligne immédiatement suivante
                else if (syllab.line > currentLine + 1)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Draw next full lines with color _InactiveColor
        /// </summary>
        /// <param name="e"></param>               
        private void DrawNextLines(int y0, PaintEventArgs e)
        {
            #region declarations
            GraphicsPath pth = new GraphicsPath();
            GraphicsPath pthc = new GraphicsPath();

            // outline            
            Pen penContour = new Pen(_InactiveBorderColor, _borderthick);

            kLine kline;
            string lineContent;
            string lineChords;

            int x0;
            int offset = _lineHeight;

            //int W;
            //int H;

            float x1;
            float y1;

            int ChordOffset = offset;  // To manage when offset = 0 
            #endregion declarations

            if (_nbLyricsLines == 1)
                offset = 0;

            // Draw sentence                           
            #region draw lyrics

            if (syllabes == null || _currentTextPos >= syllabes.Count)
                return;

            if (_currentTextPos >= 0)
                x0 = _currentTextPos - syllabes[_currentTextPos].posline;


            for (int linenr = 0; linenr < _nbLyricsLines; linenr++)
            {
                int line = currentLine + linenr + 1;

                if (line > _kLyrics.Lines.Count - 1) return;

                // linenr = 0;
                // Si currentline = 0 => line = 1
                // mais si il y a un séparateur paragraphe, 
                // on parcourt la boucle for (i = x0; i < syllabes.Count; i++) sans rien faire 
                // du coup, k passe à 1 et on utilise les rectangles de la ligne suivante
                if (_nbLyricsLines == 1)
                {
                    if (line > currentLine + 1) break;
                }
                else
                {
                    if (line > currentLine + _nbLyricsLines - 1) break;
                }

                // Line content
                kline = _kLyrics.Lines[line];

                // If there is a paragraph separator, we don't display the line
                if (kline.Syllables.First().CharType == Syllable.CharTypes.ParagraphSep) continue;

                lineContent = kline.ToString();
                lineChords = lstChordsLines[line];          // TODO : à revoir pour les accords directement dans la classe KLyrics

                x1 = HCenterText(lineContent, emSize);

                // Draw line content
                if (_bShowChords)
                {
                    #region draw chords

                    y1 = y0 + (linenr + 1) * offset + (linenr + 1) * offset;

                    // Draw chord above
                    // Add lines of lyrics to the Graphics path                                        
                    pthc.AddString(lineChords, _chordFont.FontFamily, (int)_chordFont.Style, 3 * emSize / 4, new Point((int)x1, (int)y1), sf);
                    e.Graphics.FillPath(new SolidBrush(_InactiveChordColor), pthc);

                    pthc.Dispose();

                    #endregion draw chords

                    #region draw text
                    // Draw syllabe below at 2*ChordOffset/3
                    y1 = y1 + 2 * ChordOffset / 3;

                    // Add lines of lyrics to the Graphics path
                    pth.AddString(lineContent, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x1, (int)y1), sf);

                    #region Apply effects               

                    if (FrameType == "Neon")
                        CreateNeonEffect(_InactiveBorderColor, e, pth);
                    else if (FrameType == "Shadow")
                        CreateShadowEffect(lineContent, _InactiveBorderColor, (int)x1, (int)y1, m_font, emSize, e, pth);

                    #endregion Apply effect

                    // Draw text
                    // Color clr is always InactiveColor for "NextLines"
                    e.Graphics.FillPath(new SolidBrush(_InactiveColor), pth);

                    // Outiline the text
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(penContour, pth);
                    #endregion draw text
                }
                else
                {

                    #region draw text
                    // No chords
                    y1 = y0 + (linenr + 1) * offset;

                    // Add lines of lyrics to the Graphics path
                    pth.AddString(lineContent, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x1, (int)y1), sf);

                    #region Apply effects               

                    if (FrameType == "Neon")
                        CreateNeonEffect(_InactiveBorderColor, e, pth);
                    else if (FrameType == "Shadow")
                        CreateShadowEffect(lineContent, _InactiveBorderColor, (int)x1, (int)y1, m_font, emSize, e, pth);

                    #endregion Apply effect

                    // Draw text
                    // Color clr is always InactiveColor for "NextLines"
                    e.Graphics.FillPath(new SolidBrush(_InactiveColor), pth);

                    // Outiline the text
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(penContour, pth);
                    #endregion draw text

                }
            }

            pth.Dispose();
            #endregion draw lyrics               
        }


        #endregion draw lyrics & chords


        #region effects

        /// <summary>
        /// Create a neon effect
        /// </summary>
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
            Graphics ge = e.Graphics;

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
            ge.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //and draws antialiased text for accurate fitting
            ge.TextRenderingHint = TextRenderingHint.AntiAlias;

            //The small image is blown up to fill the main client rectangle
            ge.DrawImage(bm, pBox.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);

            // finally, the text is drawn on top
            //pth.AddString(line, new FontFamily(font.Name), (int)FontStyle.Regular, emSize, new Point(x0, y0), sf);
        }

        #endregion effects


        #region Code fragments

        private void DrawActiveLineWithBorders(PaintEventArgs e, int lineIndex, int y1)
        {
            #region Declarations
            int x0 = 0;
            float x1;
            syllabe syllab;
            int W;
            int H;
            #endregion Declarations


            if (syllabes == null) return;
            if (_currentTextPos >= syllabes.Count)
                return;

            // Code removed: x0 is wrong
            //if (_currentTextPos >= 0)
            //    x0 = _currentTextPos - syllabes[_currentTextPos].posline;
            if (_currentTextPos >= 0 && _currentTextPos < syllabes.Count && syllabes[_currentTextPos].line == lineIndex)
            {
                x0 = _currentTextPos - syllabes[_currentTextPos].posline;
            }



            for (int i = x0; i < syllabes.Count; i++)
            {
                // dessine la ligne courante en mettant en surbrillance la syllabe correspondante à currentTextPos
                syllab = syllabes[i];

                // It is the current line
                if (syllab.line == lineIndex)  //currentLine)
                {
                    
                    // Bug fixed: force to recreate list of rectangles for the first item of a line
                    if (syllab.posline == 0)
                        createListRectangles(i - syllabes[i].posline);
                    

                    x1 = rRect[syllab.posline].X;                    
                    W = (int)rRect[syllab.posline].Width;
                    H = (int)rRect[syllab.posline].Height;

                    if (syllabes[i].pos < _currentTextPos)
                    {
                        // syllabes avant celle active
                        if (_bShowChords)
                        {
                            if (syllab.chord != "")
                                drawChord(_InactiveChordColor, syllab, (int)x1, y1, e);

                            drawSyllabe("Active", _ActiveColor, syllab, (int)x1, y1 + 2 * _lineHeight / 3, W, H, e);                            // déjà chanté
                        }
                        else
                        {
                            drawSyllabe("Active", _ActiveColor, syllab, (int)x1, y1, W, H, e);                                            // déjà chanté
                        }
                    }
                    else if (syllab.pos == _currentTextPos)
                    {

                        // Surbrillance normale   
                        if (bHighLight)
                        {
                            if (_bShowChords)
                            {
                                if (syllab.chord != "")
                                    drawChord(_HighlightChordColor, syllab, (int)x1, y1, e);

                                drawSyllabe("Highlight", _HighlightColor, syllab, (int)x1, y1 + 2 * _lineHeight / 3, W, H, e);                       // surbrillance
                            }
                            else
                            {
                                drawSyllabe("Highlignt", _HighlightColor, syllab, (int)x1, y1, W, H, e);                                         // surbrillance     
                            }
                        }
                        else
                        {
                            if (_bShowChords)
                            {
                                if (syllab.chord != "")
                                    drawChord(_InactiveChordColor, syllab, (int)x1, y1, e);

                                drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y1 + 2 * _lineHeight / 3, W, H, e);
                            }
                            else
                            {
                                drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y1, W, H, e);
                            }
                        }


                        #region EndOfLine & calculations

                        // Calculations are made on active syllabe (celle qui correspond à currentpos !)

                        // End of line
                        if (syllab.SylCount == 1)
                        {
                            bEndOfLine = true;
                        }
                        else if (syllab.SylCount > 1 && syllab.posline == syllab.SylCount - 1)
                        {
                            bEndOfLine = true;
                        }
                        else
                        {
                            bEndOfLine = false;


                            #region calculate next line

                            // Start of line
                            if (syllab.posline == 0)
                            {
                                // calculate next time for next start of line                           
                                int next = syllab.SylCount;
                                if (i + next < syllabes.Count)
                                {
                                    // Time of start of next line
                                    // Soit le début de la prochaine ligne, soit la fin de la ligne courante
                                    int t1 = syllabes[i + next].time;
                                    int t2 = syllabes[i + next - 1].time + 2 * _beatDuration; // on passe à la ligne au bout de 2 temps

                                    nextStartOfLineTime = t1 < t2 ? t1 : t2;

                                    // Duration until next line 
                                    TimeToNextLineDuration = nextStartOfLineTime - _currentPosition;
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    // syllabes après celle active
                    else
                    {
                        if (_bShowChords)
                        {
                            if (syllab.chord != "")
                                drawChord(_InactiveChordColor, syllab, (int)x1, (int)y1, e);

                            drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y1 + 2 * _lineHeight / 3, W, H, e);                           // pas encore chanté
                        }
                        else
                        {
                            drawSyllabe("Inactive", _InactiveColor, syllab, (int)x1, y1, W, H, e);                                      // pas encore chanté
                        }
                    }
                }
                // Ligne immédiatement suivante
                else if (syllab.line > lineIndex + 1) //currentLine + 1)
                {                    
                    break;
                }
            }

        }
       
        private void DrawActiveLineWithShadow(PaintEventArgs e, int lineIndex, int y1) 
        {
            DrawActiveLineWithBorders(e, lineIndex, y1);
        }

        private void DrawActiveLineWithNeon(PaintEventArgs e, int lineIndex, int y1) 
        {
            DrawActiveLineWithBorders(e, lineIndex, y1);
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

            GraphicsPath pthc = new GraphicsPath(); // Chords path
            GraphicsPath pth = new GraphicsPath(); // Lyrics path
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for inactive border color
                        
            string lineChords;
            float x0 = _marginLeft * pBox.Width;
            int Wbg;
            RectangleF Rbg;
            string s;

            #endregion Declarations

            if (lineIndex < 0 || lineIndex >= _kLyrics.Lines.Count()) return;
            s = _kLyrics.Lines[lineIndex].ToString();


            #region Scale font size to fit text in picture box

            float w = MeasureString(s, _karaokeFont.Size);
            // ************************  Set ScaleTransform
            // Example: if the text is greater than the width of the picture box, we reduce the size of the text to fit it in the picture box
            float scale = ((1 - 2 * _marginLeft) * pBox.Width) / w;
            if (_FontStretching == "Large" && w > 0 && w > ((1 - 2 * _marginLeft) * pBox.Width))
            {
                // No need to center text, it is already centered by ScaleTransform
                e.Graphics.ScaleTransform(scale, 1);
            }
            else
            {
                // Center text horizontally
                x0 = (int)((pBox.Width - w) / 2);
            }
            #endregion Scale font size to fit text in picture box


            #region Show chords

            if (_bShowChords)
            {                
                lineChords = lstChordsLines[lineIndex];                 // TODO : à revoir pour les accords directement dans la classe KLyrics

                // Draw chord above the text                                                         
                pthc.AddString(lineChords, _chordFont.FontFamily, (int)_chordFont.Style, 3 * emSize / 4, new Point((int)x0, y2), sf);
                e.Graphics.FillPath(new SolidBrush(_InactiveChordColor), pthc);

                pthc.Dispose();

                // Draw syllabe below at 2 * ChordOffset / 3
                y2 = y2 + 2 * _lineHeight / 3;                               
            }

            #endregion Show chords


            #region draw text

            #region background of text  

            if (_bTextBackGround)
            {
                Wbg = (int)(1.04 * LinesLengths[lineIndex]);
                // Black background to make text more visible
                Rbg = new RectangleF((int)(0.94 * x0), (int)(1.04 * y2), Wbg, _lineHeight);
                // background
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), Rbg);
            }

            #endregion background of text

            // Add lines of lyrics to the Graphics path
            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0, y2), sf);

            #region apply effect

            if (_frametype == "Shadow")
                CreateShadowEffect(s, BorderColor, (int)x0, y2, _karaokeFont, _karaokeFont.Size, e, pth);
            else if (_frametype == "Neon")
                CreateNeonEffect(BorderColor, e, pth);

            #endregion apply effect


            // Draw the text                    
            e.Graphics.FillPath(new SolidBrush(FillColor), pth);

            // Outline the text
            if (_borderthick > 0)
                e.Graphics.DrawPath(penBorder, pth);

            // ************************  Reset ScaleTransform
            e.Graphics.ResetTransform();

            #endregion draw text


            #region Clean up resources

            pth.Dispose();
            pthc.Dispose();
            penBorder.Dispose();
            
            #endregion Clean up resources
        }

        private void DrawInactiveLineWithShadow(PaintEventArgs e, int lineIndex, int y2, bool IsActive = false)
        {

            DrawInactiveLineWithBorders(e, lineIndex, y2, IsActive);

            /*
            #region Declarations
            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            GraphicsPath pthc = new GraphicsPath(); // Chords path
            GraphicsPath pth = new GraphicsPath(); // Lyrics path
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for inactive border color

            string lineContent;
            string lineChords;
            int x0;
            int Wbg;
            RectangleF Rbg;
            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                lineContent = _kLyrics.Lines[lineIndex].ToString();  // kline.ToString();                        
                lineChords = lstChordsLines[lineIndex];          // TODO : à revoir pour les accords directement dans la classe KLyrics

                x0 = HCenterText(lineContent, emSize);

                // Draw line content
                if (_bShowChords)
                {
                    #region Draw Chords                

                    // Draw chord above the text                                                         
                    pthc.AddString(lineChords, _chordFont.FontFamily, (int)_chordFont.Style, 3 * emSize / 4, new Point((int)x0, (int)y2), sf);
                    e.Graphics.FillPath(new SolidBrush(_InactiveChordColor), pthc);

                    pthc.Dispose();

                    // Draw syllabe below at 2 * ChordOffset / 3
                    y2 = y2 + 2 * _lineHeight / 3;

                    #endregion Draw Chords                  
                }

                #region draw text                

                #region background of line  

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
                pth.AddString(lineContent, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, (int)y2), sf);

                #region Apply effects               

                CreateShadowEffect(lineContent, _InactiveBorderColor, (int)x0, (int)y2, m_font, emSize, e, pth);

                #endregion Apply effect


                // Draw the text                    
                e.Graphics.FillPath(new SolidBrush(FillColor), pth);

                // Outiline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, pth);

                #endregion draw text

            }

            #region Clean up resources

            pth.Dispose();
            pthc.Dispose();
            penBorder.Dispose();

            #endregion Clean up resources

            */
        }

        private void DrawInactiveLineWithNeon(PaintEventArgs e, int lineIndex, int y2, bool IsActive = false)
        {
            DrawInactiveLineWithBorders(e, lineIndex, y2, IsActive);
            /*
            #region Declarations
            Color BorderColor = _InactiveBorderColor;
            Color FillColor = _InactiveColor;

            if (IsActive)
            {
                BorderColor = _ActiveBorderColor;
                FillColor = _ActiveColor;
            }

            GraphicsPath pthc = new GraphicsPath(); // Chords path
            GraphicsPath pth = new GraphicsPath(); // Lyrics path
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for inactive border color

            string lineContent;
            string lineChords;
            int x0;
            int Wbg;
            RectangleF Rbg;
            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {
                lineContent = _kLyrics.Lines[lineIndex].ToString();  // kline.ToString();                        
                lineChords = lstChordsLines[lineIndex];          // TODO : à revoir pour les accords directement dans la classe KLyrics

                x0 = HCenterText(lineContent, emSize);

                // Draw line content
                if (_bShowChords)
                {
                    #region Draw Chords                

                    // Draw chord above the text                                                         
                    pthc.AddString(lineChords, _chordFont.FontFamily, (int)_chordFont.Style, 3 * emSize / 4, new Point((int)x0, (int)y2), sf);
                    e.Graphics.FillPath(new SolidBrush(_InactiveChordColor), pthc);

                    pthc.Dispose();

                    // Draw syllabe below at 2 * ChordOffset / 3
                    y2 = y2 + 2 * _lineHeight / 3;

                    #endregion Draw Chords               
                }

                #region draw text            

                #region background of line  

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
                pth.AddString(lineContent, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, (int)y2), sf);

                #region Apply effects               

                CreateNeonEffect(_InactiveBorderColor, e, pth);

                #endregion Apply effect


                // Draw the text                    
                e.Graphics.FillPath(new SolidBrush(FillColor), pth);

                // Outiline the text
                if (_borderthick > 0)
                    e.Graphics.DrawPath(penBorder, pth);

                #endregion draw text
            }

            #region Clean up resources

            pth.Dispose();
            pthc.Dispose();
            penBorder.Dispose();

            #endregion Clean up resources
            */

        }


        /// <summary>
        /// Draw a line of information like (introduction, instrumental, ending) on a single line
        /// </summary>
        /// <param name="e"></param>
        ///  <param name="infotext"</param>
        /// <param name="y"></param>
        /*
        private void DrawInformation(PaintEventArgs e, string infotext, int seconds, int y0) 
        {
            // Seconds
            // value    Display                     Color
            //  > 0:    (instrumental) seconds      Active
            //  = 0:    (instrumental)              highlight
            // = -1:    (instrumental)              Active

            GraphicsPath path = new GraphicsPath();
            int x0;
            Pen penBorder = new Pen(ActiveBorderColor);
            Color FillColor;
            
            switch (seconds)
            {
                case -1:
                    FillColor = _ActiveInstrumentalColor;
                    break;
                case 0:
                    FillColor = _HighlightColor; 
                    break;
                default:
                    FillColor = _ActiveInstrumentalColor;
                    break;
            }

            // if 0 or -1, do not display seconds
            infotext = seconds > 0 ? infotext + " " + seconds.ToString() : infotext;
            x0 = HCenterText(infotext, emSize);

            // Add lines of lyrics to the Graphics path
            path.AddString(infotext, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, (int)y0), sf);

            // Draw the text                    
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text
            if (_borderthick > 0)
                e.Graphics.DrawPath(penBorder, path);

        }
        */

        private void DrawInformation(PaintEventArgs e, int lineIndex, int seconds, int y0)
        {
            // Seconds
            // value    Display                     Color
            //  > 0:    (instrumental) seconds      Active
            //  = 0:    (instrumental)              highlight
            // = -1:    (instrumental)              Active

            if (lineIndex < 0 || lineIndex >= _kLyrics.Lines.Count()) return;


            GraphicsPath path = new GraphicsPath();
            int x0 = 0;
            Pen penBorder = new Pen(_ActiveBorderColor);
            Color FillColor;
            string s = _kLyrics.Lines[lineIndex].Syllables.Last().Text;

            switch (seconds)
            {
                case -1:
                    FillColor = _ActiveInstrumentalColor;
                    break;
                case 0:
                    FillColor = _HighlightColor;
                    break;
                default:
                    FillColor = _ActiveInstrumentalColor;
                    break;
            }

            // if 0 or -1, do not display seconds
            s = seconds > 0 ? s + " " + seconds.ToString() : s;

            // ************************  Set ScaleTransform
            float w = MeasureString(s, _karaokeFont.Size);
            float scale = (pBox.Width - 50) / w;
            if (w > 0 && w > pBox.Width - 50)
            {
                // No need to center text, it is already centered by ScaleTransform
                e.Graphics.ScaleTransform(scale, 1);
            }
            else
            {
                // Center text horizontally
                x0 = (int)((pBox.Width - w) / 2);
            }

            // Add lines of lyrics to the Graphics path
            path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0, (int)y0), sf);

            // Draw the text                    
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text
            if (_borderthick > 0)
                e.Graphics.DrawPath(penBorder, path);

            // ************************  Reset ScaleTransform
            e.Graphics.ResetTransform();

        }

        private void DrawFileName(PaintEventArgs e, string FileName, float femSize)
        {
            int x0 = 0;
            Color BorderColor = _ActiveBorderColor;
            Color FillColor = _InactiveColor;
            Pen penBorder = new Pen(BorderColor, 2);
            var path = new GraphicsPath();


            int y0 = (int)MeasureStringHeight(FileName, _titleMarginTop * _karaokeFont.Size);

            // Measure FileName
            float w = MeasureString(FileName, femSize);
            if (w == 0) return;

            float maxLength = _titleMaxLength * pBox.Width;    // 41 % of width            

            // Allow to adapt the length of the text to the width allowed 
            // If the width of the text is greater than maxLength, ScaleTransform reduce it
            float scale = maxLength / w;    // Ratio maxLength vs  width of text
            e.Graphics.ScaleTransform(scale, 1);

            // Left position of the text                        
            x0 = (int)(_titleMarginLeft * pBox.Width / scale);

            // Add string to path
            path.AddString(FileName, _karaokeFont.FontFamily, (int)_karaokeFont.Style, femSize, new Point(x0, y0), sf);


            // Draw the text            
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text            
            e.Graphics.DrawPath(penBorder, path);

            e.Graphics.ResetTransform();
        }

        #endregion Code fragments


        #region Draw text with fixed lines

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

        private void FixDrawTextWithBorder(PaintEventArgs e)
        {

            if (_kLyrics.Lines.Count == 0) return;

            #region Draw FileName

            // Draw file name if required

            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName

        

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);
            
            // Draw active line with borders
            DrawActiveLineWithBorders(e, _FirstLineToShow, y0);
            
            // Draw next  inactives lines with borders
            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

            int y2;
            for (int i = _FirstLineToShow + 1; i <= _LastLineToShow; i++)
            {
                if (i < lstLyricsLines.Count)
                {
                    y2 = y0 + (i - _FirstLineToShow) * _lineHeight;
                    DrawInactiveLineWithBorders(e, i, y2);                    
                }
            }          
        }

        private void FixDrawTextWithShadow(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            #region Draw FileName

            // Draw file name if required

            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            // Draw active line with borders
            DrawActiveLineWithBorders(e, _FirstLineToShow, y0);


            // Draw next  inactives lines with borders           
            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

            int y2; 

            for (int i = _FirstLineToShow; i <= _LastLineToShow; i++)
            {
                if (i < lstLyricsLines.Count)
                {

                    y2 = y0 + (i - _FirstLineToShow) * _lineHeight;
                    DrawInactiveLineWithBorders(e, i, y2);
                }
            }           
        }

        private void FixDrawTextWithNeon(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            #region Draw FileName

            // Draw file name if required

            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            // Draw active line with borders
            DrawActiveLineWithBorders(e, _FirstLineToShow, y0);


            // Draw next  inactives lines with borders
            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

            int y2;
            for (int i = _FirstLineToShow; i <= _LastLineToShow; i++)
            {
                if (i < lstLyricsLines.Count)
                {
                    y2 = y0 + (i - _FirstLineToShow) * _lineHeight;
                    DrawInactiveLineWithBorders(e, i, y2);
                }
            }
        }

        #endregion Draw text with fixed lines

       
        #region Draw text with Four lines swapped

        private void DrawTextWithFourLinesSwapped(PaintEventArgs e)
        {
            _nbLyricsLines = 4;

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
        
     
        /// <summary>
        /// Draw four lines swapped
        /// </summary>
        /// <param name="e"></param>
        private void FlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

                       // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            #region Declarations

            int LineOfInformationPosition;
            int[] LinesNr = new int[4];
            float w;

            int y0;
            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx1 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int tm;
            #endregion Declarations


            #region Draw FileName

            // Draw file name if required

            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName


            #region Line layout

            // Draw lines starting from this position
            y0 = (int)(_marginTop * _lineHeight);

            // 4 lines positioning depending on the value of _FirstLineToShow % 4          
            int l = _FirstLineToShow;

            int a1 = y0;
            int a2 = y0 + _lineHeight;
            int a3 = y0 + (int)(_lineHeight * (1 + _fourLinesSpacing));   
            int a4 = y0 + (int)(_lineHeight * (2 + _fourLinesSpacing));   

            int LinePosition = _FirstLineToShow % 4;

            switch (LinePosition)
            {                
                case 0:

                    (y1, y2, y3, y4) = (a1, a2, a3, a4);
                    (idx1, idx2, idx3, idx4) = (l, l + 1, l + 2, l + 3);
                    LinesNr = new int[] { idx1, idx2, idx3, idx4 };

                    /*
                    y1 = y0;                                        // idx1     _FirstLineToShow              current         (update 3 & 4)                
                    y2 = y0 + _lineHeight;                          // idx2     _FirstLineToShow + 1      inactive
                    y3 = y2 + (int)(_fourLinesSpacing * _lineHeight);     // idx3     _FirstLineToShow + 2      inactive                                           
                    y4 = y3 + _lineHeight;                          // idx4     _FirstLineToShow + 3      inactive                                           
                    */
                    break;
                
                case 1:
                    (y1, y2, y3, y4) = (a2, a1, a3, a4);
                    (idx1, idx2, idx3, idx4) = (l, l - 1, l + 1, l + 2);
                    LinesNr = new int[] { idx2, idx1, idx3, idx4 };

                    /*
                    y2 = y0;                                        // idx2     _FirstLineToShow - 1     active
                    y1 = y0 + _lineHeight;                          // idx1     _FirstLineToShow             current         (no update)
                    y3 = y1 + (int)(_fourLinesSpacing * _lineHeight);     // idx3     _FirstLineToShow + 1     inactive                                          
                    y4 = y3 + _lineHeight;                          // idx4     _FirstLineToShow + 2     inactive                                          
                    */
                    break;
                
                case 2:
                    (y1, y2, y3, y4) = (a3, a4, a1, a2);
                    (idx1, idx2, idx3, idx4) = (l, l + 1, l + 2, l + 3);                    
                    LinesNr = new int[] { idx3, idx4, idx1, idx2 };

                    /*
                    y3 = y0;                                        // idx3     _FirstLineToShow + 2     inactive
                    y4 = y0 + _lineHeight;                          // idx4     _FirstLineToShow + 3     inactive
                    y1 = y4 + (int)(_fourLinesSpacing * _lineHeight);     // idx1     _FirstLineToShow             current         (update 1 & 2)                        
                    y2 = y1 + _lineHeight;                          // idx2     _FirstLineToShow + 1     inactive                                                  
                   
                    */
                    break;
                
                case 3:
                    (y1, y2, y3, y4) = (a4, a3, a1, a2);
                    (idx1, idx2, idx3, idx4) = (l, l - 1, l + 1, l + 2);                    
                    LinesNr = new int[] { idx3, idx4, idx2, idx1 };

                    /*
                    y3 = y0;                                        // idx3     _FirstLineToShow + 1     inactive
                    y4 = y0 + _lineHeight;                          // idx4     _FirstLineToShow + 2     inactive
                    y2 = y4 + (int)(_fourLinesSpacing * _lineHeight);     // idx2     _FirstLineToShow - 1      active                                                
                    y1 = y2 + _lineHeight;                          // idx1     _FirstLineToShow             current         (no update)                        
                    */
                    break;
                
            }
            #endregion Line layout


            #region Search line of information
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
            #endregion Search line of information


            // If no line of information in the 4 lines => normal display in 4 lines swapped
            if (LineOfInformationPosition == -2)
            {
                #region Normal drawing

                bInstrumentalStarted = false;
                bCountDown = false;

                // Draw y1 line: active & highlighted line               
                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);

                // Line y2 must be drawned active when
                bool IsActive = ((_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3)); // ? true : false;

                // Draw y2 line: active when before line y1 (already sung), inactive when after line y1 (not yet sung)
                if (idx2 >= 0)
                    DrawInactiveLineWithBorders(e, idx2, y2, IsActive);


                // Draw lines y3 and y4 (always inactives)
                if (idx3 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithBorders(e, idx3, y3);
                if (idx4 < _kLyrics.Lines.Count)
                    DrawInactiveLineWithBorders(e, idx4, y4);

                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();
                

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

                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);                                

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 3 second
                                    tm = PlayerPositionTicks - _startTime;
                                    if (tm < TicksPerSecond)
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
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                DrawInformation(e, _FirstLineToShow - 1, SecondsBeforeSinging, y1 - _lineHeight);

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
                                DrawInformation(e, idx3, -1, y3);
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
                                DrawInformation(e, idx3, -1, y3);
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
                                DrawInformation(e, idx3, -1, y3);
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
                                DrawInformation(e, idx3, -1, y3);
                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    tm = PlayerPositionTicks - _startTime;
                                    if (tm < TicksPerSecond)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines
                                        return;
                                    }
                                }                                

                                DrawInactiveLineWithBorders(e, idx3, y3);                                
                                DrawInactiveLineWithBorders(e, idx4, y4);
                                break;

                            case 3:
                                // y1 normal
                                // y2 normal
                                // y3 information1
                                // y4 * information2
                                
                                // Draw "(intrumental)" on active line and countdown on next line
                                DrawInformation(e, idx2, SecondsBeforeSinging, y2);

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
            FlsDrawTextWithBorder(e);

            /*
            if (_kLyrics.Lines.Count == 0) return;

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            
            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int LinePosition = -1;
            float lineSpacing = 1.2f;

            // Search for information
            // None                         -2
            // _FirstLineToShow - 1         -1
            // _FirstLineToShow              0
            // _FirstLineToShow + 1          1
            // _FirstLineToShow + 2          2
            // _FirstLineToShow + 3          3
            int LineOfInformationPosition; // = -2;

            int[] LinesNr = new int[4];

            #region Line layout

            if (_FirstLineToShow % 4 == 0)
            {
                // First line is active                
                LinePosition = 0;
                // Position possible for LineOfInformationPosition

                y1 = y0;                            //          _FirstLineToShow              current         (update 3 & 4)
                y2 = y0 + _lineHeight;              // idx2     _FirstLineToShow + 1      inactive
                y3 = y2 + (int)(lineSpacing * _lineHeight);          // idx3     _FirstLineToShow + 2      inactive
                y4 = y3 + _lineHeight;          // idx4     _FirstLineToShow + 3      inactive

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
                y3 = y1 + (int)(lineSpacing * _lineHeight);          // idx3     _FirstLineToShow + 1     inactive
                y4 = y3 + _lineHeight;          // idx4     _FirstLineToShow + 2     inactive

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
                y1 = y4 + (int)(lineSpacing * _lineHeight);          //          _FirstLineToShow             current         (update 1 & 2)
                y2 = y1 + _lineHeight;          // idx2     _FirstLineToShow + 1     inactive

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
                y2 = y4 + (int)(lineSpacing * _lineHeight);          // idx2     _FirstLineToShow - 1     * active
                y1 = y2 + _lineHeight;          //          _FirstLineToShow             current         (no update)

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
                bool IsActive = ((_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3)); // ? true : false;

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


                int tm = PlayerPositionTicks - _startTime;

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
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (_FirstLineToShow - 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text, SecondsBeforeSinging, y1 - _lineHeight);

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
                                if (idx3 <  _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);

                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithShadow(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (idx2 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text, SecondsBeforeSinging, y2);

                                DrawInactiveLineWithShadow(e, idx3, y3);
                                DrawInactiveLineWithShadow(e, idx4, y4);

                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }

            LastLineOfInformationPosition = LineOfInformationPosition;
            */
        }

        private void FlsDrawTextWithNeon(PaintEventArgs e)
        {

            FlsDrawTextWithBorder(e);

            /*
            if (_kLyrics.Lines.Count == 0) return;

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);
           
            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            int y1 = 0;
            int y2 = 0;
            int y3 = 0;
            int y4 = 0;
            int idx2 = 0;
            int idx3 = 0;
            int idx4 = 0;

            int LinePosition = -1;
            float lineSpacing = 1.2f;

            // Search for information
            // None                         -2
            // _FirstLineToShow - 1         -1
            // _FirstLineToShow              0
            // _FirstLineToShow + 1          1
            // _FirstLineToShow + 2          2
            // _FirstLineToShow + 3          3
            int LineOfInformationPosition; // = -2;

            int[] LinesNr = new int[4];

            #region Line layout

            if (_FirstLineToShow % 4 == 0)
            {
                // First line is active                
                LinePosition = 0;
                // Position possible for LineOfInformationPosition

                y1 = y0;                            //          _FirstLineToShow              current         (update 3 & 4)
                y2 = y0 + _lineHeight;              // idx2     _FirstLineToShow + 1      inactive
                y3 = y2 + (int)(lineSpacing * _lineHeight);          // idx3     _FirstLineToShow + 2      inactive
                y4 = y3 + _lineHeight;          // idx4     _FirstLineToShow + 3      inactive

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
                y3 = y1 + (int)(lineSpacing * _lineHeight);          // idx3     _FirstLineToShow + 1     inactive
                y4 = y3 + _lineHeight;          // idx4     _FirstLineToShow + 2     inactive

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
                y1 = y4 + (int)(lineSpacing * _lineHeight);          //          _FirstLineToShow             current         (update 1 & 2)
                y2 = y1 + _lineHeight;          // idx2     _FirstLineToShow + 1     inactive

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
                y2 = y4 + (int)(lineSpacing * _lineHeight);          // idx2     _FirstLineToShow - 1     * active
                y1 = y2 + _lineHeight;          //          _FirstLineToShow             current         (no update)

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
                bool IsActive = (_FirstLineToShow % 4 == 1) || (_FirstLineToShow % 4 == 3); // ? true : false;

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


                int tm = PlayerPositionTicks - _startTime;

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
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (_FirstLineToShow - 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow - 1].Syllables.Last().Text, SecondsBeforeSinging, y1 - _lineHeight);

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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);
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
                                if (idx3 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx3].Syllables.Last().Text, -1, y3);

                                break;

                            case 2:                                                 // LinePosition is 2   = LineOfInformationPosition                                                                                                                
                                // y1 normal
                                // y2 normal
                                // y3 * information1
                                // y4 information2
                                if (bCountDown)
                                {
                                    // Keep last actives lines 3 & 4 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
                                    {
                                        if (_FirstLineToShow - 2 >= 0)
                                        {
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 2, y3, true);
                                            DrawInactiveLineWithNeon(e, _FirstLineToShow - 1, y4, true);
                                        }
                                    }
                                }
                                // Draw "(intrumental)" on active line and countdown on next line
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                // Draw lines y3 and y4 only if they are less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (idx2 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[idx2].Syllables.Last().Text, SecondsBeforeSinging, y2);

                                DrawInactiveLineWithNeon(e, idx3, y3);
                                DrawInactiveLineWithNeon(e, idx4, y4);

                                break;
                        }
                        break;

                    #endregion Instrumental on bottom
                }
            }

            LastLineOfInformationPosition = LineOfInformationPosition;
            */
        }

        #endregion Draw text with Four lines swapped


        #region Draw text with Two lines swapped

        private void DrawTextWithTwoLinesSwapped(PaintEventArgs e)
        {
            _nbLyricsLines = 2;

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


        private void TlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            #region Declarations

            // Center text vertically
            int y0 = VCenterText();

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition; // = -1;

            #endregion Declarations


            #region Draw FileName

            // Draw file name if required

            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName


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

                // Draw active line with borders
                DrawActiveLineWithBorders(e, _FirstLineToShow, y1);                

                // Draw Inactive line with borders
                //if (_currentTextPos > -1)
                    DrawInactiveLineWithBorders(e, _FirstLineToShow + 1, y2);                

                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.                
                CheckIfInstrumentalBegins();

                // Update the CountDown
                UpdateCountDown();

                int tm = PlayerPositionTicks - _startTime;                

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
                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);
                                
                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)                                                                            
                                        DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y2, true);         // keep old line 1 sec                                                                                                                        
                                }

                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                // Except for introduction
                                if (bInstrumentalStarted && _FirstLineToShow > 0)
                                {                                    
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                DrawInformation(e, _FirstLineToShow + 1, -1, y2);

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
                                DrawInformation(e, _FirstLineToShow + 1, -1, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm  < TicksPerSecond)
                                    {
                                        if (_FirstLineToShow - 1 >= 0)
                                        {
                                            DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y1 - _lineHeight, true);         // keep old line 1 sec                                            
                                        }
                                    }
                                }
                                // Draw line y2 only if it is less than 4 sec before the end of an instrumental
                                if (bInstrumentalStarted)
                                {
                                    //tm = _endTime - PlayerPositionTicks;
                                    tm = TargetPositionTicks - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
                                    {
                                        // Do not display next lines until _DelayBeforeEndOfInstrumental
                                        return;
                                    }
                                }
                                //Console.WriteLine("Display 2nd line");
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
            TlsDrawTextWithBorder(e);

            /*
            if (_kLyrics.Lines.Count == 0) return;

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            // Synchronise can modify currentLine, so we need to recalculate it after synchronize
            //int _FirstLineToShow = currentLine;

            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition; // = -1;

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
                if (CurrentTextPos > -1)
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

                int tm = PlayerPositionTicks - _startTime;

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
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = _endTime - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, -1, y2);
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
                                if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, -1, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = _endTime - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
        
            */
        }

        private void TlsDrawTextWithNeon(PaintEventArgs e)
        {
            TlsDrawTextWithBorder(e);

            /*
            if (_kLyrics.Lines.Count == 0) return;

            // Antialiasing
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Create list of rectangles when line changes
            synchronize(_currentTextPos);

            // Synchronise can modify currentLine, so we need to recalculate it after synchronize
            //int _FirstLineToShow = currentLine;

            // Calculate offset to center the text vertically
            int y0 = getOffsetHeight(emSize);

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition; // = -1;

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
                if (CurrentTextPos > -1)
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

                int tm = PlayerPositionTicks - _startTime;

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
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = _endTime - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
                                if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, -1, y2);
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
                                if (_FirstLineToShow + 1 < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow + 1].Syllables.Last().Text, -1, y1 + _lineHeight);
                                break;

                            case 1:
                                // y2 normal old than new
                                // y1 * information
                                if (_FirstLineToShow < _kLyrics.Lines.Count)
                                    DrawInformation(e, _kLyrics.Lines[_FirstLineToShow].Syllables.Last().Text, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm < TicksPerSecond)
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
                                    tm = _endTime - PlayerPositionTicks;

                                    if (tm > _DelayBeforeEndOfInstrumental)
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
        
            */
        }
     

        #endregion Draw text with Two lines swapped


        #region Draw text with Scrolling lines top down

        private void DrawTextWithScrollingLinesTopDown(PaintEventArgs e)
        {
        }

        private void SltDrawTextWithBorder(PaintEventArgs e)
        {
            try
            {
                // Create list of rectangles when line changes
                synchronize(_currentTextPos);

                // Antialiasing
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Calculate offset to center the text vertically
                int y0 = getOffsetHeight(emSize);

                if (_nbLyricsLines > 1)
                {
                    // Several lines to display
                    // progressive offset - vOffset increases, so y0 decreases
                    y0 = y0 - vOffset;

                    // Draw current line                    
                    DrawCurrentLine(_currentPosition, y0, e);

                    // Draw next lines                 
                    DrawNextLines(y0, e);
                }
                else
                {
                    // A single line to display
                    // Draw current line until end of line
                    if (!bEndOfLine)
                        DrawCurrentLine(_currentPosition, y0, e);
                    else
                        DrawNextLines(y0, e);
                }
            }
            catch (Exception ep)
            {
                Console.Write("Error drawing text on image: " + ep.Message);
            }
        }

        private void SltDrawTextWithShadow(PaintEventArgs e)
        {
        }

        private void SltDrawTextWithNeon(PaintEventArgs e)
        {
        }

        #endregion Draw text with Scrolling lines top down


        #region Draw text with Scrolling lines bottom up

        private void DrawTextWithScrollingLinesBottomUp(PaintEventArgs e)
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
                //ScrollingBottomUpDrawTextWithBorder(e);
                //break;
                case "Shadow":
                //ScrollingBottomUpDrawTextWithShadow(e);
                //break; ;
                case "Neon":
                    //ScrollingBottomUpDrawTextWithNeon(e);
                    ScrollingBottomUpDrawTextWithBorder(e);
                    break;

                default:
                    ScrollingBottomUpDrawTextWithBorder(e);
                    break;
            }
        }

        private bool bShowInformation = false;

        private void ScrollingBottomUpDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;
            if (linesYCoordinates == null) return;
            int y = 0;

            int TopMargin = bShowSongName ? pBox.ClientRectangle.Top + (int)(_titleMarginTop * pBox.Height) : pBox.ClientRectangle.Top;
            int BottomMargin = pBox.ClientRectangle.Bottom;

            #region Draw FileName

            // Draw file name if required
            if (bShowSongName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName


            #region check whether to show information and update instrumental and countdown state

            if (_kLyrics.Lines[_FirstLineToShow].Syllables.Last().CharType == Syllable.CharTypes.Information)
            {
                bShowInformation = true;
                CheckIfInstrumentalBegins();
                UpdateCountDown();
            }
            else
            {
                bShowInformation = false;
            }
            #endregion check whether to show information and update instrumental and countdown state


            if (bShowInformation)            
                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, pBox.ClientRectangle.Top + pBox.ClientRectangle.Height / 2);            


            // Calculate vertical position of the lines according to the position of the song in the current line
            vposition = (float)((PlayerPositionTicks) * ((float)_linesHeight / (_kLyrics.Lines.Last().Syllables.First().TicksOn)));

            //Console.WriteLine("vposition: " + vposition + " - PlayerPositionTicks: " + PlayerPositionTicks + " - _linesHeight: " + _linesHeight + " - _kLyrics.Lines.Last().Syllables.First().TicksOn: " + _kLyrics.Lines.Last().Syllables.First().TicksOn);

            for (int i = 0; i < _kLyrics.Lines.Count; i++)
            {
                y = pBox.ClientRectangle.Top + pBox.ClientRectangle.Height / 2 + (int)(linesYCoordinates[i]);


                #region Do not draw lines that are out of the control

                //if (y - vposition < pBox.ClientRectangle.Top - _lineHeight)
                if (y - vposition < TopMargin)
                {
                    // Do not draw lines that are out of the control
                    continue;
                }
                if (_kLyrics.Lines[i].Syllables.Last().CharType == Syllable.CharTypes.Information)
                {
                    // Do not draw lines of information                    
                    continue;
                }

                if (y - vposition > BottomMargin)                                  //pBox.ClientRectangle.Bottom + _lineHeight)
                    break; // Do not draw lines that are out of the control

                #endregion Do not draw lines that are out of the control


                if (i == _FirstLineToShow)
                {
                    e.Graphics.TranslateTransform(0, -vposition);
                    DrawActiveLineWithBorders(e, i, y);
                }
                else
                {
                    bool IsActive = (i < _FirstLineToShow) ? true : false;
                    e.Graphics.TranslateTransform(0, -vposition);
                    DrawInactiveLineWithBorders(e, i, y, IsActive);
                }
            }

            e.Graphics.ResetTransform();
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
                    //y = 0;
                    return new Rectangle(x, 0, (int)(imgWidth * zoomFactor), this.ClientSize.Height);
                   
                default:
                    return new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height); 
            }            
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


        private void ajustTextAgain()
        {
            if (lineMax != null && syllabes != null)
            {
                int pos;
                AjustText(lineMax);

                if (_currentTextPos < 0)
                {
                    // Rectangles of current line
                    createListRectangles(0);
                    /*
                    // Rectangles of next line
                    if (syllabes != null && syllabes.Count > 0)
                    {
                        pos = syllabes[0].last + 1;
                        // Rectangles for other lines
                        //createListNextRectangles(pos);
                    }
                    */
                }
                else
                {
                    // Rectangles of current line
                    pos = _currentTextPos - syllabes[_currentTextPos].posline;
                    createListRectangles(pos);
                    
                    /*
                    // Rectangles of next line
                    pos = syllabes[_currentTextPos].last + 1;
                    // Rectangles for other lines 
                    //createListNextRectangles(pos);
                    */
                }
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

        #endregion Paint Control

              
        #region Scrolling

        private void InitScrollMode()
        {
            if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
            {
                if (_kLyrics == null || _kLyrics.Lines.Count == 0) return;

                // calculate the y-coordinate of each line according to its start time and the current position of the player
                List<float> intervals = new List<float>();


                linesYCoordinates = new float[_kLyrics.Lines.Count];
                float t = 0;
                float last_t = 0;
                float min = (float)_duration;

                //Search for the minimum duration between 2 lines
                // This minimpum will be equivalent to the line height and will be used to calculate the y-coordinate of each line according to its start time and the current position of the player
                for (int i = 0; i < _kLyrics.Lines.Count; i++)
                {
                    t = (float)_kLyrics.Lines[i].Syllables.First().TicksOn;
                    intervals.Add(t - last_t);
                    //if (t > 0 && last_t > 0 && t != last_t && t - last_t < min)
                    //    min = t - last_t;
                    last_t = t;
                }

                intervals.Sort();
                if (intervals.Count > 10)
                    min = intervals[10]; // take the 4th minimum to avoid too small intervals that could be due to errors in the timing of the lines


                // Calculate the y-coordinate of each line: multiple of the minimum line height (_lineHeight) between 2 lines =  _lineHeight * (t - last_t) / min
                last_t = 0;

                float y0 = 0;
                for (int i = 0; i < _kLyrics.Lines.Count; i++)
                {
                    t = (float)_kLyrics.Lines[i].Syllables.First().TicksOn;

                    y0 += ((t - last_t) / min) * _lineHeight;
                    linesYCoordinates[i] = y0;
                    last_t = t;
                }

                // Calculate the total height of the full song in scrolling mode
                // ie the sum of the distances between lines
                _linesHeight = (int)linesYCoordinates[linesYCoordinates.Length - 1];

                pBox.Invalidate();
            }
        }

        #endregion Scrolling


        #region SlideShow with timer       

        // New Slideshow
        private void InitSlideShow()
        {
            mBlend = 0;
            count = 0;

            timerChangeImage?.Dispose();
            timerChangeImage = new System.Timers.Timer()
            {
                Interval = _freqSlideShow * 1000
            };
            timerChangeImage.Elapsed += (sender, e) => OnTimerChangeImage();

            timerTransition?.Dispose();
            timerTransition = new System.Timers.Timer()
            {
                Interval = 50
            };
            timerTransition.Elapsed += (sender, e) => OnTimerTransition();

            try
            {
                Image1 = m_BitmapsArray[count];
                Image2 = m_BitmapsArray[++count];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading images: " + ex.Message);
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
                else if (count < m_BitmapsArray.Length)
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

        #region SlideShow functions

        /// <summary>
        /// Load all images into a list
        /// </summary>
        /// <param name="dir"></param>
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

            m_BitmapsArray = new Bitmap[bgFiles.Length];
            for (int i = 0; i < bgFiles.Length; ++i)
            {
                m_BitmapsArray[i] = new Bitmap(bgFiles[i]);
            }

        }

        #endregion SlideShow functions


        #endregion SlideShow with timer      


        #region Terminate        

        /// <summary>
        /// Terminate 
        /// </summary>
        public void Terminate()
        {
            m_ImageFilePaths = new List<string>();
            m_BitmapsArray = new Bitmap[] { };

            timerChangeImage?.Stop();
            timerChangeImage?.Dispose();
            timerChangeImage = null;
            timerTransition?.Stop();
            timerTransition?.Dispose();
            timerTransition = null;
        }


        #endregion Terminate


        #region Text and Chords management

        /// <summary>
        /// Store lyrics lines in a list called lstLyricsLines
        /// </summary>
        /// <param name="ly"></param>    
        private List<string> StoreLyricsLines(kLyrics kl)
        {

            /*
           * A back slash "\" character marks the end of a line of lyrics, as displayed by a Karaoke viewer/player program.
           *
           * A forward slash "/" character marks the end of a "paragraph" of lyrics. 
           * Some Karaoke viewer / player programs interpret this to mean that the screen should be refreshed starting with the next line of lyrics at the top.
           *
           * Dash characters at the end of syllables are removed by the Karaoke viewer/player program, and the syllables are joined together. 
           */

            List<string> lstLines = new List<string>();

            for (int i = 0; i < kl.Lines.Count; i++)
            {
                string lineContent = kl.Lines[i].ToString();

                if (_bshowparagraphs && lineContent == _InternalSepParagraphs)
                {
                    // new paragraph = empty line (space)
                    lstLines.Add(" ");
                }
                else //if (lineContent != "")
                {
                    lstLines.Add(lineContent);
                }
            }
            return lstLines;
        }

        private List<string> StoreChordLines(kLyrics kl)
        {
            string chord;
            string lyric;
            string lineChords; // = string.Empty;
            List<string> lstChords = new List<string>();

            for (int i = 0; i < kl.Lines.Count; i++)
            {
                lineChords = string.Empty;

                for (int j = 0; j < kl.Lines[i].Syllables.Count; j++)
                {
                    Syllable syll = kl.Lines[i].Syllables[j];
                    if (syll.CharType == Syllable.CharTypes.Text)
                    {
                        chord = syll.Chord;
                        lyric = syll.Text;

                        if (chord.Length > lyric.Length)
                        {
                            lyric += new string(' ', chord.Length - lyric.Length);
                        }
                        else if (chord.Length < lyric.Length)
                        {
                            chord += new string(' ', lyric.Length - chord.Length);
                        }
                        lineChords += chord;
                    }
                }
                lstChords.Add(lineChords);
            }

            return lstChords;
        }


        /// <summary>
        /// Store syllabes in a list, each item being a class called syllabe
        /// </summary>
        /// <param name="plLyrics"></param>      
        List<syllabe> StoreLyricsSyllabes(kLyrics kl)
        {

            syllabe syl;
            Syllable kSyl;
            List<syllabe> lstSyllabes = new List<syllabe>();

            for (int i = 0; i < kl.Lines.Count; i++)
            {
                kLine line = kl.Lines[i];

                for (int j = 0; j < line.Syllables.Count; j++)
                {
                    kSyl = line.Syllables[j];

                    syl = new syllabe()
                    {

                        chord = kSyl.Chord,
                        line = i,                                                                   // line number of syllabe
                        posline = j,                                                                // position dans la ligne
                        pos = lstSyllabes.Count,                                                    // position dans la chanson
                        text = kSyl.CharType == Syllable.CharTypes.ParagraphSep ? " " : kSyl.Text,           // text of syllabe, space if paragraph
                        time = kSyl.TicksOn,                                                        // time of syllabe
                        SylCount = line.Syllables.Count,                                            // number of syllabes in this line                      
                        last = line.Syllables.Count - 1,
                        offset = 0,
                    };

                    // position of last syllabe
                    for (int k = 0; k < i; k++)
                    {
                        syl.last += kl.Lines[k].Syllables.Count;
                    }

                    lstSyllabes.Add(syl);

                }
            }
            return lstSyllabes;
        }


        /// <summary>
        /// Crée une liste de rectangles pour chaque syllable de la ligne en cours 
        /// </summary>
        /// <param name="pos"></param>
        private void createListRectangles(int pos)
        {
            if (pos < 0)
                pos = 0;

            if (pos < syllabes.Count)
            {
                using (Graphics g = pBox.CreateGraphics())
                {
                    string tx = string.Empty;

                    rRect = new List<RectangleF>();

                    int line = syllabes[pos].line;
                    string strLine = lstLyricsLines[line];
                    float Offset = HCenterText(strLine, emSize);           // Offset de la ligne (centré)

                    int idx = -1;
                    float x = Offset;

                    for (int i = pos; i < syllabes[pos].SylCount + pos; i++)
                    {
                        idx++;

                        // Taille de l'expace = caractère tiret
                        tx = syllabes[i].text;

                        RectangleF rect = new RectangleF();

                        SizeF sz = g.MeasureString(tx, m_font, new Point(0, 0), sf);
                        x += sz.Width;

                        rect.Width = sz.Width;
                        rect.Height = sz.Height + 1;


                        if (idx == 0)
                        {
                            //rect.X = Offset - 1;
                            rect.X = Offset;
                        }
                        else
                        {
                            //rect.X = rRect[idx - 1].X + rRect[idx - 1].Width - 1;
                            rect.X = rRect[idx - 1].X + rRect[idx - 1].Width;

                        }
                        rRect.Add(rect);
                    }
                    g.Dispose();
                }
            }
        }


        /// <summary>
        /// Create rectangles when line changes
        /// </summary>
        /// <param name="res"></param>
        private void synchronize(int syllabeposition)
        {
            if (syllabes == null)
                return;

            int x0; // = 0;
            // Si retour arriere ou avance
            if (syllabeposition < 0)
                syllabeposition = 0;

            if (syllabeposition >= 0 && syllabeposition < syllabes.Count)
            {
                if (syllabes[syllabeposition].line != currentLine)
                {
                    currentLine = syllabes[syllabeposition].line;
                    //currentLine = _FirstLineToShow;

                    // Beginning of line
                    x0 = syllabeposition - syllabes[syllabeposition].posline;
                    // Create list of rectangles for current line
                    createListRectangles(x0);

                    // Create list of rectangles for next line
                    //createListNextRectangles(syllabes[syllabeposition].last + 1);
                }
            }
        }


        #endregion Text and Chords management


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

    }
}
