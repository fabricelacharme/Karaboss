using kar;
using Karaboss.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Mp3
{
    public partial class frmTest : Form
    {

        #region Balls
        // Show balls
        private bool _bShowBalls = true;
        public bool bShowBalls
        {
            get { return _bShowBalls; }
            set
            {
                _bShowBalls = value;
                //pnlTop.Visible = _bShowBalls;
            }
        }
        #endregion Balls


        #region Colors

        #region Background & text background color

        // Background color
        private Color _BgColor;
        public Color BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;                
            }
        }

        #endregion Background & text background color
       

        #region Gradient color

        private Color _grad0Color;
        public Color Grad0Color
        {
            get { return _grad0Color; }
            set
            {
                _grad0Color = value;                
            }
        }
        private Color _grad1Color;
        public Color Grad1Color
        {
            get { return _grad1Color; }
            set
            {
                _grad1Color = value;                
            }
        }
        private Color _Rhythm0Color;
        public Color Rhythm0Color
        {
            get { return _Rhythm0Color; }
            set
            {
                _Rhythm0Color = value;                
            }
        }
        private Color _rhythm1Color;
        public Color Rhythm1Color
        {
            get { return _rhythm1Color; }
            set
            {
                _rhythm1Color = value;                
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
            }
        }

        #endregion Instrumentals color


        #region Text color

        // Text sung color
        private Color _ActiveColor;
        public Color ActiveColor
        {
            get { return _ActiveColor; }
            set
            {
                _ActiveColor = value;               
            }
        }

        private Color _HighlightColor;
        public Color HighlightColor
        {
            get { return _HighlightColor; }
            set
            {
                _HighlightColor = value;                
            }
        }

        // Text to sing color
        private Color _InactiveColor;
        public Color InactiveColor
        {
            get { return _InactiveColor; }
            set
            {
                _InactiveColor = value;                
            }
        }


        // Text border
        private Color _ActiveBorderColor;
        public Color ActiveBorderColor
        {
            get { return _ActiveBorderColor; }
            set
            {
                _ActiveBorderColor = value;                
            }
        }

        private Color _InactiveBorderColor;
        public Color InactiveBorderColor
        {
            get { return _InactiveBorderColor; }
            set
            {
                _InactiveBorderColor = value;                
            }
        }

        #endregion text color

        #endregion Colors

       
        #region Draw filename

        public bool bShowSongName  = true;

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
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

        //private int _CurrentLineToShow = 0;
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

        #endregion Draw syllables
              

        #region Font

        private string ftName = "Arial Black";
        private uint ftSize = 20;

        Font m_font;
        private float emSize = 40; // Size of the font

        private StringFormat sf;
        Font _karaokeFont;

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

        private bool _bTopMost = false;
        public bool bTopMost
        {
            get { return _bTopMost; }
            set
            {
                _bTopMost = value;
                this.TopMost = _bTopMost;
            }
        }

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

        #endregion Form


        #region Instrumentals

        private bool _bShowHints = true;
        public bool bShowHints
        {
            get { return _bShowHints; }
            set
            {
                _bShowHints = value;
                //pBox.bShowHints = _bShowHints;
            }
        }


        private DateTime _endTime;                      // used by countdown
        private DateTime _startTime;                    // used by countdown

        private double PlayerPositionMilliseconds;      // current player position in ms
        private double TargetPositionMilliseconds;      // position to reach in ms

        private bool bInstrumentalStarted = false;
        private int SecondsBeforeSinging = 0;
        private bool bCountDown = false;
        private readonly int _DelayBeforeEndOfInstrumental = 4000; // Delay to draw lines before the end of an instrumental: 4 sec
        private readonly int _MinimumInstrumentalDuration = 5000;  // The minimum duration between two consecutive vocal phrases that mark an instrumental interlude : 5 sec
        private int LastLineOfInformationPosition = 0;    // Used to store the last valid Instrumental line position (to manage end of song)

        private readonly int _MinimumIntroDuration = 3000;

        #endregion Instrumentals


        #region Karaoke display layout

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
                    //Init();

                }
                pBox?.Invalidate();
            }
        }


        private string _strkaraokeDisplayType = "None";
        public string strKaraokeDisplayType
        {
            get { return _strkaraokeDisplayType; }
            set
            {
                _strkaraokeDisplayType = value;

                switch (_strkaraokeDisplayType)
                {
                    case "None":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.None;
                        break;
                    case "FixedLines":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.FixedLines;
                        break;
                    case "ScrollingLinesTopDown":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.ScrollingLinesTopDown;
                        break;
                    case "ScrollingLinesBottomUp":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.ScrollingLinesBottomUp;
                        break;
                    case "TwoLinesSwapped":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.TwoLinesSwapped; break;
                    case "FourLinesSwapped":
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.FourLinesSwapped; break;
                    default:
                        KaraokeDisplayType = kar.KaraokeDisplayTypes.FixedLines;
                        break;
                }
            }
        }

        #endregion Karaoke display layout


        #region Karaoke lyrics

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

        #endregion Karaoke lyrics


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


        #region MP3

        // Duration in seconds (org Bass)
        private double _duration;
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        private int _bitrate;   // genre 192
        public int BitRate { get { return _bitrate; } set { _bitrate = value; } }

        // Frequency
        private float _frequency;
        public float Frequency { get { return _frequency; } set { _frequency = value; } }

        #endregion MP3


        #region Picture

        private PictureBoxSizeMode _sizeMode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                //karaokeEffect1.SizeMode = _sizeMode;
            }
        }

        #endregion Picture


        #region SlideShow

        // Paths of images        
        private List<string> m_ImageFilePaths;
        // Array of bitmaps (images as backgound image)
        private Bitmap[] m_BitmapsArray;

        #region Select background

        // Background option : Diaporama, SolidColor, Transparent        
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
                        //karaokeEffect1.OptionBackground = "Image";
                        break;

                    case "Diaporama":
                        //karaokeEffect1.OptionBackground = "Diaporama";
                        break;
                    case "SolidColor":
                        //karaokeEffect1.OptionBackground = "SolidColor";
                        break;

                    case "Gradient":
                        //karaokeEffect1.OptionBackground = "Gradient";
                        break;
                    case "Rhythm":
                        //karaokeEffect1.OptionBackground = "Rhythm";
                        break;

                    case "Transparent":
                        //TransparencyKey = karaokeEffect1.TransparencyKey;
                        //BackColor = karaokeEffect1.TransparencyKey;
                        //karaokeEffect1.OptionBackground = "Transparent";
                        break;
                    default:
                        //karaokeEffect1.OptionBackground = "Diaporama";
                        break;
                }
            }
        }

        #endregion Select background


        #region Single image

        private string _SingleImagePath;
        public string SingleImagePath
        {
            get => _SingleImagePath;
            set
            {
                if (System.IO.File.Exists(value))
                {
                    _SingleImagePath = value;                   
                }
            }
        }

        #endregion Single image


        #region SlideShow images

        private bool _allowModifyDirSlideShow = true;
        public bool AlloModifyDirSlideShow
        {
            get { return _allowModifyDirSlideShow; }
            set { _allowModifyDirSlideShow = value; }
        }

        // SlideShow directory
        private string _dirSlideShow = string.Empty;
        public string DirSlideShow
        {
            get { return _dirSlideShow; }
            set
            {
                if (value == null || value == "")
                    value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

                if (Directory.Exists(value))
                    _dirSlideShow = value;
                else
                    _dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

                //karaokeEffect1.SetDirectoryBackground(_dirSlideShow);
            }
        }

        // SlideShow frequency
        private int _freqSlideShow;
        public int FreqSlideShow
        {
            get { return _freqSlideShow; }
            set
            {
                _freqSlideShow = value;
                //karaokeEffect1.FreqDirSlideShow = _freqSlideShow;
            }
        }

        #endregion SlideShow images

       
        #endregion SlideShow


        #region Themes

        private ThemesListHelper _ThListHelper = new ThemesListHelper();
        private ThemesList _ThemesList; // = new ThemesList();
        private ThemeItem _currentTheme; // = new ThemeItem();

        #endregion Themes


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


                //AjustText(_biggestLine);      // pourquoi ? mystère. Mais ça marche
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

        private Karaclass.OptionsDisplay _OptionDisplay;
        /// <summary>
        /// Display lyrics option: top, Center, Bottom
        /// </summary>
        public Karaclass.OptionsDisplay OptionDisplay
        {
            get { return _OptionDisplay; }
            set
            {
                _OptionDisplay = value;
                pBox.Invalidate();
            }
        }

        #endregion Text position

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
                    //if (_bIsSettings)
                    //    LoadDemoText();

                }
            }
        }
        

        private bool _bprogressivehighlight = false;
        public bool bProgressiveHighlight
        {
            get { return _bprogressivehighlight; }
            set
            {
                _bprogressivehighlight = value;                
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


        public frmTest(string fileName, double duration, kLyrics kls)
        {
            InitializeComponent();

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            

            _bIsSettings = false;

            SetDefaultValues();
            LoadOptions();

            FileName = fileName;
            Duration = duration * 1000; // Convert to milliseconds
            KLyrics = kls;              // This will launch Init
            bShowSongName = true;
        }


        #region Color Functions

        /// <summary>
        /// Check text representing a color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color Parse(string input)
        {
            input = input.Trim();
            string strColorRegex = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            Regex re = new Regex(strColorRegex);
            if (re.IsMatch(input))
            {
                return ColorTranslator.FromHtml(input);
            }

            Color named = Color.FromName(input);
            if (named.IsKnownColor || named.IsNamedColor)
            {
                return named;
            }
            throw new ArgumentException($"Unsupported color value: {input}", nameof(input));
        }


        #endregion Color functions


        #region initializations

        private void SetDefaultValues()
        {
            m_ImageFilePaths = new List<string>();
            m_BitmapsArray = new Bitmap[] { };

            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };
            
            _karaokeFont = new Font("Arial Black", emSize, FontStyle.Regular, GraphicsUnit.Pixel);

            _steppercent = 0.01F;
            _transitionEffect = TransitionEffects.None;
        }

        private void LoadOptions()
        {
            try
            {
                // Load colors lyrics, backgrounds from current Theme
                LoadColorsFromCurrentTheme();


                // Karaoke display type
                strKaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;     

                // Lyrics border effect 
                _frametype = Properties.Settings.Default.FrameType;
                FrameType = _frametype;

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);

                FontStretching = Properties.Settings.Default.FontStretching;

                bShowParagraphs = Karaclass.m_ShowParagraph;


                // Display file name in lyrics as title
                bShowSongName = Properties.Settings.Default.bShowSongName;

                // Progressive highlight
                bProgressiveHighlight = Properties.Settings.Default.bProgressiveHighlight;

                // Force Uppercase
                bforceUppercase = Karaclass.m_ForceUppercase;

                // show balls
                bShowBalls = Karaclass.m_DisplayBalls;

                // Backgrounds (image, diaporama, solid color, gradient, rhythm, transparent)
                OptionBackground = Properties.Settings.Default.BackGroundOption;


                #region Lyrics vertical position

                switch (Properties.Settings.Default.LyricsOptionDisplay)
                {
                    case "Top":
                        _OptionDisplay = Karaclass.OptionsDisplay.Top;
                        break;
                    case "Center":
                        _OptionDisplay = Karaclass.OptionsDisplay.Center;
                        break;
                    case "Bottom":
                        _OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                        break;
                    default:
                        _OptionDisplay = Karaclass.OptionsDisplay.Center;
                        break;
                }
                OptionDisplay = _OptionDisplay;

                #endregion Lyrics vertical position


                bTextBackGround = Properties.Settings.Default.bLyricsBackGround;

                // Number of Lines to display
                nbLyricsLines = Properties.Settings.Default.TxtNbLines;


                SingleImagePath = Properties.Settings.Default.SingleImagePath;
                // Frequency of slide show
                FreqSlideShow = Properties.Settings.Default.freqSlideShow;
                // Position image
                SizeMode = Properties.Settings.Default.SizeMode;

                bTopMost = Properties.Settings.Default.frmMp3LyricsTopMost;
              

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadColorsFromCurrentTheme()
        {
            #region Retrieve theme

            // Load all available color themes
            _ThemesList = LoadThemes();

            // Load default Theme name
            string currentThemeName = Properties.Settings.Default.Theme;

            // Retrieve Theme from ThList with its name
            _currentTheme = _ThemesList.GetThemeByName(currentThemeName);

            #endregion Retrieve theme

            if (_currentTheme == null)
            {
                // If null (file themes.xml lost for ex) => Default
                _currentTheme = _ThemesList.Themes[0];
            }

            // Get colors from the current theme
            #region Get colors from them

            // Text colors
            ActiveColor = Parse(_currentTheme.ActiveColor);
            HighlightColor = Parse(_currentTheme.HighlightColor);
            InactiveColor = Parse(_currentTheme.InactiveColor);
            ActiveBorderColor = Parse(_currentTheme.ActiveBorderColor);
            InactiveBorderColor = Parse(_currentTheme.InactiveBorderColor);

            // Instrumental
            ActiveInstrumentalColor = Parse(_currentTheme.ActiveInstrumentalColor);

            // Static background
            BgColor = Parse(_currentTheme.BgColor);

            // Dynamic background
            Grad0Color = Parse(_currentTheme.Grad0Color);
            Grad1Color = Parse(_currentTheme.Grad1Color);
            Rhythm0Color = Parse(_currentTheme.Rhythm0Color);
            Rhythm1Color = Parse(_currentTheme.Rhythm1Color);

            // Chords
            //InactiveChordColor = Parse(_currentTheme.InactiveChordColor);
            //HighlightChordColor = Parse(_currentTheme.HighlightChordColor);

            #endregion Get colors from theme                                                          

        }

        #region Themes Color

        private ThemesList LoadThemes()
        {
            try
            {
                string fileName = Karaclass.GetThemesListFile(_ThListHelper.File);
                _ThListHelper.File = fileName;
                return _ThListHelper.Load(fileName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion Themes Color

        private kLyrics RemoveParagraphs(kLyrics kls)
        {
            kLyrics klsNoParagraphs = new kLyrics();
            kLine line;
            for (int i = 0; i < kls.Lines.Count; i++)
            {                
                if (!(kls.Lines[i].Syllables.Count == 1 && kls.Lines[i].Syllables.First().Text == string.Empty))
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


        private kLyrics SearchForInstrumentals(kLyrics kls)
        {
            double tOnPrevious = 0;
            double duration = 0;            
            double t; // = 0;
            kLyrics klsWithinstrumentals = new kLyrics();
            kLine line;            
            double tend;

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
                        if (i == 0 && j == 0)
                        {
                            #region Introduction
                            // Start of intro
                            line.Add(new Syllable() { Text = "(introduction)", StartTime = 0, CharType = Syllable.CharTypes.Information });
                            klsWithinstrumentals.Add(line);

                            if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                            {
                                // end of intro = just before first lyric
                                tend = t;
                                if (tend > _MinimumIntroDuration)
                                    tend = tend - _MinimumIntroDuration;
                                line = new kLine();
                                line.Add(new Syllable() { Text = "", StartTime = tend, CharType = Syllable.CharTypes.Information });
                                klsWithinstrumentals.Add(line);
                            }
                            line = new kLine();
                            #endregion Introduction
                        }
                        else if (t - tOnPrevious > _MinimumInstrumentalDuration)
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

                            if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
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

            if (_duration - t > _MinimumInstrumentalDuration)
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

                #region ENDING
                // First line
                line = new kLine();
                line.Add(new Syllable() { Text = "(ending)", StartTime = t + duration, CharType = Syllable.CharTypes.Information });
                klsWithinstrumentals.Add(line);

                if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                {
                    // 2nd line 1 sec before the end of the song
                    tend = _duration - 1000;
                    line = new kLine();
                    line.Add(new Syllable() { Text = "", StartTime = tend, CharType = Syllable.CharTypes.Information });
                    klsWithinstrumentals.Add(line);
                }
                #endregion ENDING

            }
            return klsWithinstrumentals;
        }


        private void Init()
        {
            // Fonts
            //emSize = this.Font.Size;
            //_karaokeFont = new Font("Arial Black", emSize, FontStyle.Regular, GraphicsUnit.Pixel);            


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
                    //_nbLyricsLines = _kLyrics.Lines.Count;
                    _nbLyricsLines = 6;
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    //_nbLyricsLines = _kLyrics.Lines.Count;
                    _nbLyricsLines = 6;
                    break;
                default:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
            }

            _LastLineToShow = SetLastLineToShow(_FirstLineToShow, _kLyrics.Lines.Count, _nbLyricsLines);

            

            #endregion Karaoke display type
           

            // Do not display paragraphs for some cases
            if (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped 
                || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped 
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown
                || !bShowParagraphs
                )
            {
                if (!_bIsSettings)
                    _kLyrics = RemoveParagraphs(_kLyrics);
            }


            // Analyse lyrics to find introduction, instrumentals etc..
            if (!_bIsSettings && (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped 
                || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp 
                || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown
                )
                 _kLyrics = SearchForInstrumentals(_kLyrics);


            // Store all lines lengths
            LinesLengths = new float[_kLyrics.Lines.Count];
            //_AverageWidth = GetAverageWidth(_kLyrics, _karaokeFont.Size);

            _biggestLine = GetBiggestLine();
            AdjustFontSize(_nbLyricsLines);


            if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
            {
                // For scrolling display, we need to have all lines in order to scroll them
                //_nbLyricsLines = _kLyrics.Lines.Count;
                InitScrollMode();
            }

        }

        #endregion initializations


        #region form load close

        private void frmTest_Resize(object sender, EventArgs e)
        {

            AdjustFontSize(_nbLyricsLines);

            if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
                InitScrollMode();
        }



        #endregion form load close


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
                            TargetPositionMilliseconds = _kLyrics.Lines[_FirstLineToShow + 1].Syllables.First().StartTime;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionMilliseconds = _duration;
                    }
                    else if (KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped)
                    {
                        // (_FirstLineToShow + 1 cannot be used because it is the 2nd line of information)
                        if (_FirstLineToShow + 2 < _kLyrics.Lines.Count)
                            TargetPositionMilliseconds = _kLyrics.Lines[_FirstLineToShow + 2].Syllables.First().StartTime;   // Position in the song to reach = next real syllable                                                               
                        else
                            TargetPositionMilliseconds = _duration;                                                  // Position in the song to reach = end of song
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


        #region Lyrics and position
        public void SetLyrics(kLyrics lyrics)
        {
            KLyrics = lyrics;
        }


        public void SetPos(double ms)
        {
            // Store player position (ms)
            PlayerPositionMilliseconds = ms;

            // Store player position (ms)            
            SetPosition((int)ms);
        }

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

       
        #region Measure

        private int VCenterText()
        {
            int y = 0;

            // Height of control minus height of lines to show
            switch (_OptionDisplay)
            {
                case Karaclass.OptionsDisplay.Center:
                    y = (pBox.ClientSize.Height - (_nbLyricsLines) * _lineHeight) / 2;
                    break;

                case Karaclass.OptionsDisplay.Top:
                    if (bShowSongName)
                        y = (int)(_titleMarginTop * pBox.Height);
                    else
                        y = 0;
                    break;

                case Karaclass.OptionsDisplay.Bottom:
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
            if (kls.Lines.Count == 0) return 0;

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

        #endregion Measure


        #region Paint
        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            
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
                    //DrawTextWithScrollingLinesTopDown(e);
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
                          

        #region fragments
        private void DrawActiveLineWithBorders(PaintEventArgs e, int lineIndex, int y1)
        {
            #region declarations

            int Wbg;
            RectangleF Rbg;

            Region r;
            RectangleF rect;

            Brush ActiveColorBrush = new SolidBrush(_ActiveColor);
            Brush HighlightColorBrush = new SolidBrush(_HighlightColor);
            Brush InactiveColorBrush = new SolidBrush(_InactiveColor);

            Pen ActiveBorderPen = new Pen(new SolidBrush(_ActiveBorderColor), _borderthick);
            Pen InactiveBorderPen = new Pen(new SolidBrush(_InactiveBorderColor), _borderthick);

            float x0 = _marginLeft * pBox.Width;
            GraphicsPath pth = new GraphicsPath();

            string s;

            #endregion declarations

            if (lineIndex < 0 || lineIndex >= _kLyrics.Lines.Count()) return;            

            s = _kLyrics.Lines[lineIndex].ToString();


            #region Scale font size to fit text in picture box

            // ************************  Set ScaleTransform
            float w = MeasureString(s, _karaokeFont.Size);            
            // Example: if the text is greater than the width of the picture box, we reduce the size of the text to fit it in the picture box
            float scale = ((1 - 2*_marginLeft)*pBox.Width) / w;
            
            if (_FontStretching == "Large" && w > 0 && w > ((1 - 2*_marginLeft)*pBox.Width))
            {
                // No need to center text, it is already centered by ScaleTransform
                e.Graphics.ScaleTransform(scale, 1);
            }
            else
            {
                scale = 1;
                // Center text horizontally
                x0 = (int)((pBox.Width - w) / 2);
            }            

            #endregion Scale font size to fit text in picture box


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

            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0 , y1), sf);

            // Draw full line in white if no active and highlight fragments
            if (active_fragment == string.Empty && highlight_fragment == string.Empty && inactive_fragment == string.Empty)
            {
                #region Draw static text (no active and highlight fragments)

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
                // Create a rectangle of the graphical path
                rect = r.GetBounds(e.Graphics);

                
                #region draw active text
                
                if (active_fragment != string.Empty)
                {
                    GraphicsPath pathActive = new GraphicsPath();                    

                    pathActive.AddString(active_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0, y1), sf);

                    #region Paint in ActiveColor

                    // Rectangle for text before highlighted text (rect.Width * lastpercent)
                    //RectangleF intersectRectBefore = new RectangleF(rect.X / scale, rect.Y, rect.Width * lastpercent * scale, rect.Height);                    

                    // update region on the intersection between region and 2nd rectangle
                    //r.Intersect(intersectRectBefore);

                    // Fill updated region in green
                    //e.Graphics.FillRegion(ActiveColorBrush, r);

                    #endregion Paint in ActiveColor

                   
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
                    //RectangleF intersectRect = new RectangleF( (rect.X + rect.Width * lastpercent)/scale, rect.Y, rect.Width * (percent - lastpercent) * scale, rect.Height);                    

                    // update region on the intersection between region and 2nd rectangle
                    //r.Intersect(intersectRect);

                    // Fill updated region in highlight color => percent portion of text is highlighted            
                    //e.Graphics.FillRegion(HighlightColorBrush, r);

                    #endregion Paint in HighlightColor

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
                    //RectangleF intersectRectAfter = new RectangleF( (rect.X + rect.Width * percent)/scale, rect.Y, rect.Width - rect.Width * percent * scale, rect.Height);                   

                    // update region on the intersection between region and 2nd rectangle
                    //r.Intersect(intersectRectAfter);

                    // Fill updated region in InactiveColor
                    //e.Graphics.FillRegion(InactiveColorBrush, r);

                    #endregion Paint in InactiveColor

                   
                    // Draw the text               
                    e.Graphics.FillPath(InactiveColorBrush, pathInactive);

                    // Outline the text
                    if (_borderthick > 0)
                        e.Graphics.DrawPath(InactiveBorderPen, pathInactive);

                    pathInactive.Dispose();

                }
                #endregion Draw inactive text
                

                r.Dispose();


                // ************************  Reset ScaleTransform
                e.Graphics.ResetTransform();

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
            float x0 = _marginLeft * pBox.Width;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
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
            float scale = ((1 - 2*_marginLeft)*pBox.Width) / w;
            if (_FontStretching == "Large" && w > 0 && w > ((1 - 2*_marginLeft)*pBox.Width))
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
            path.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point((int)x0, y2), sf);


            // Draw the text            
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text
            if (_borderthick > 0)
                e.Graphics.DrawPath(penBorder, path);

            // ************************  Reset ScaleTransform
            e.Graphics.ResetTransform();
            

            #region Clean up resources

            path.Dispose();
            penBorder.Dispose();

            #endregion Clean up resources
        }

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

        #endregion fragments


        #region Draw text with Four lines swapped

        private void DrawTextWithFourLinesSwapped(PaintEventArgs e)
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
                    FlsDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    //FlsDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    //FlsDrawTextWithNeon(e);
                    break; ;

                default:
                    FlsDrawTextWithBorder(e);
                    break;
            }
        }
            
        
        private void FlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

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

                #endregion Normal drwaing
            }
            else
            {
                #region Instrumental drawing
                
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.
                CheckIfInstrumentalBegins();
                // Update the CountDown
                UpdateCountDown();

                TimeSpan tm = DateTime.Now - _startTime;

                switch (LineOfInformationPosition)
                {
                    #region Instrumental on top                        
                    case 0:                             // Instrumental is on line 0
                                                                              
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
                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);                                

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

                #endregion Instrumental drawing

            }

            // Save last line of information position for end of song
            LastLineOfInformationPosition = LineOfInformationPosition;
        }


        #endregion Draw text with Four lines swapped


        #region Draw text with two lines swapped

        private void DrawTextWithTwoLinesSwapped(PaintEventArgs e)
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
                    TlsDrawTextWithBorder(e);
                    break;

                case "Shadow":
                    //TlsDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    //TlsDrawTextWithNeon(e);
                    break; ;

                default:
                    //TlsDrawTextWithBorder(e);
                    break;
            }
        }

        private void TlsDrawTextWithBorder(PaintEventArgs e)
        {
            if (_kLyrics.Lines.Count == 0) return;

            #region Declarations
            // Center text vertically
            int y0 = VCenterText();

            int y1;    // y1 is the y coordinate of the active line to display (line _FirstLineToShow)
            int y2;    // y2 is the y coordinate of the inactive line to display (line _FirstLineToShow + 1)

            int LineOfInformationPosition = -2;
            int LinePosition;

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
                if (percent > 0)
                    DrawInactiveLineWithBorders(e, _FirstLineToShow + 1, y2);

                #endregion Normal drawing
            }
            else
            {
                // Checks whether an instrumental section has begun and updates the countdown and timing state accordingly.                
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
                                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, y1);

                                if (bCountDown)
                                {
                                    // Keep last active line 2 when instrumental has began for 1 second
                                    if (tm.TotalMilliseconds < 1000)                                                                                                                    
                                        DrawInactiveLineWithBorders(e, _FirstLineToShow - 1, y2, true);         // keep old line 1 sec                                                                                                                       
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
                                    if (tm.TotalMilliseconds < 1000)
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

        #endregion Draw text with two lines swapped


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
                    //FixDrawTextWithShadow(e);
                    break; ;

                case "Neon":
                    //FixDrawTextWithNeon(e);
                    break; ;

                default:
                    //FixDrawTextWithBorder(e);
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

        #endregion Draw text with fixed lines


        #region Draw scrolling text

        #region Draw scrolling text bottom up
        
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
                    ScrollingBottomUpDrawTextWithBorder(e);
                    break;
                case "Shadow":
                    //ScrollingBottomUpDrawTextWithShadow(e);
                    break; ;
                case "Neon":
                    //ScrollingBottomUpDrawTextWithNeon(e);
                    break; ;
                default:
                    //ScrollingBottomUpDrawTextWithBorder(e);
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
            {                                
                DrawInformation(e, _FirstLineToShow, SecondsBeforeSinging, pBox.ClientRectangle.Top + pBox.ClientRectangle.Height / 2);
            }



            // Calculate vertical position of the lines according to the position of the song in the current line
            vposition = (float)((PlayerPositionMilliseconds) * (_linesHeight / (_kLyrics.Lines.Last().Syllables.First().StartTime)));

            for (int i = 0; i < _kLyrics.Lines.Count; i++)
            {                          
                y = pBox.ClientRectangle.Top + pBox.ClientRectangle.Height/2 + (int)(linesYCoordinates[i]) ;


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

                if ( y - vposition > BottomMargin)                                  //pBox.ClientRectangle.Bottom + _lineHeight)
                    break; // Do not draw lines that are out of the control

                #endregion Do not draw lines that are out of the control


                if (i == _FirstLineToShow)
                {                       
                    e.Graphics.TranslateTransform(0, -vposition);
                    DrawActiveLineWithBorders(e, i, y);
                }
                else
                {

                    e.Graphics.TranslateTransform(0, -vposition);
                    DrawInactiveLineWithBorders(e, i, y);
                }
                
            }
            
            e.Graphics.ResetTransform();                     
        }


        #endregion Draw scrolling text bottom up


        #region Draw scrolling text top down

        #endregion Draw scrolling text top down


        #endregion Draw scrolling text


        #endregion Paint

                       

        #region Start , Stop

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
            //_CurrentLineToShow = -1;

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

        #endregion Start , Stop


        #region Scrolling

        private void InitScrollMode()
        {
            if (KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesBottomUp || KaraokeDisplayType == KaraokeDisplayTypes.ScrollingLinesTopDown)
            {
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
                    t = (float)_kLyrics.Lines[i].Syllables.First().StartTime;
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
                    t = (float)_kLyrics.Lines[i].Syllables.First().StartTime;

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

    }
}
