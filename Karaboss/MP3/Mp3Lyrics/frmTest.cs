using GradientApp;
using kar;
using MusicXml.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Mpeg4;

namespace Karaboss.Mp3
{
    public partial class frmTest : Form
    {

        #region Colors

        Color _BgColor;

        Color _ActiveColor;
        Color _HighlightColor;
        Color _InactiveColor;

        Color _ActiveBorderColor;
        Color _InactiveBorderColor;
       
        Color _ActiveInstrumentalColor;

        #endregion Colors


        #region Instrumentals

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


        #region Draw filename

        public bool bDrawFileName = true;

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
                    //Init();

                }
                pBox?.Invalidate();
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


        #region Font

        Font m_font;
        private float emSize = 40; // Size of the font
        private StringFormat sf;
        Font _karaokeFont;

        #endregion Font


        #region Form

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


        #region SlideShow

        // Paths of images        
        private List<string> m_ImageFilePaths;
        // Array of bitmaps (images as backgound image)
        private Bitmap[] m_BitmapsArray;


        #endregion SlideShow


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

        public frmTest(string fileName, double duration, kLyrics kls)
        {
            InitializeComponent();

            FileName = fileName;
            Duration = duration;
            KLyrics = kls;
            bDrawFileName = true;

            SetDefaultValues();
        }


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


        private kLyrics RemoveParagraphs(kLyrics kls)
        {
            kLyrics klsNoParagraphs = new kLyrics();
            kLine line;
            for (int i = 0; i < kls.Lines.Count; i++)
            {
                //if (kls.Lines[i].Syllables.First().CharType != Syllable.CharTypes.ParagraphSep)
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
            //double introDurationMinimum = 1000;
            double t; // = 0;
            kLyrics klsWithinstrumentals = new kLyrics();
            kLine line;
            //string text; // = string.Empty;
            double tend; // = 0;

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

                            if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
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

            if (_duration * 1000 - t > _MinimumInstrumentalDuration)
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

                if (KaraokeDisplayType != KaraokeDisplayTypes.TwoLinesSwapped)
                {
                    // 2nd line 1 sec before the end of the song
                    tend = _duration * 1000 - 1000;
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
            emSize = this.Font.Size;
            _karaokeFont = new Font("Arial Black", emSize, FontStyle.Regular, GraphicsUnit.Pixel);


            KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped;


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
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
                default:
                    _nbLyricsLines = _nbLyricsLinesOrg;
                    break;
            }


            // Colors
            _BgColor = Parse("#FCA903");

            _ActiveColor = Parse("#00ACFF");
            _HighlightColor = Parse("#FFFF00");
            _InactiveColor = Parse("#FFFFFF");
            
            _ActiveBorderColor = Parse("#010101");
            _InactiveBorderColor = Parse("#8000FF");

            _ActiveInstrumentalColor = Parse("#808080");

           

            _bIsSettings = false;


            // Do not display paragraphs for some cases
            if (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped || !bShowParagraphs)
            {
                if (!_bIsSettings)
                    _kLyrics = RemoveParagraphs(_kLyrics);
            }


            // Analyse lyrics to find introduction, instrumentals etc..
            if (!_bIsSettings && (KaraokeDisplayType == KaraokeDisplayTypes.TwoLinesSwapped || KaraokeDisplayType == KaraokeDisplayTypes.FourLinesSwapped))
                _kLyrics = SearchForInstrumentals(_kLyrics);


            // Store all lines lengths
            LinesLengths = new float[_kLyrics.Lines.Count];
            _AverageWidth = GetAverageWidth(_kLyrics, _karaokeFont.Size);

            AdjustText(_AverageWidth, _nbLyricsLines);


        }


        #endregion initializations



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

            //if (inactive_fragment.Length > 0) 
            //    Console.WriteLine("inactive_fragment = " + inactive_fragment);

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



        #region Paint
        private void picBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            
            #region draw text

            switch (KaraokeDisplayType)
            {
                case KaraokeDisplayTypes.FixedLines:
                    //DrawTextWithFixedLines(e);
                    break;
                case KaraokeDisplayTypes.ScrollingLinesBottomUp:
                    //DrawTextWithScrollingLinesBottomUp(e);
                    break;
                case KaraokeDisplayTypes.ScrollingLinesTopDown:
                    //DrawTextWithScrollingLinesTopDown(e);
                    break;
                case KaraokeDisplayTypes.TwoLinesSwapped:
                    //DrawTextWithTwoLinesSwapped(e);
                    break;
                case KaraokeDisplayTypes.FourLinesSwapped:
                    DrawTextWithFourLinesSwapped(e);
                    break;
            }

            #endregion draw text
        }
              
      
        private void DrawFileName(PaintEventArgs e, string FileName, float femSize)
        {
            //sf = null;
            int x0 = 0;
            Color BorderColor = _ActiveBorderColor;
            Color FillColor = _InactiveColor;
            Pen penBorder = new Pen(BorderColor, 2);
            var path = new GraphicsPath();


            int y0 = (int)MeasureStringHeight(FileName, 0.038f * _karaokeFont.Size);

            // Measure FileName
            float w = MeasureString( FileName, femSize);

            float maxLength = 0.41f * pBox.Width;    // 41 % of width            

            // Allow to adapt the length of the text to the width allowed 
            // If the width of the text is greater than maxLength, ScaleTransform reduce it
            float scale = maxLength / w;    // Ratio maxLength vs  width of text
            e.Graphics.ScaleTransform(scale, 1);

            // Left position of the text                        
            x0 = (int)(0.58f * pBox.Width / scale);

            // Add string to path
            path.AddString(FileName, _karaokeFont.FontFamily, (int)_karaokeFont.Style, femSize, new Point(x0, y0), sf);


            // Draw the text            
            e.Graphics.FillPath(new SolidBrush(FillColor), path);

            // Outline the text            
            e.Graphics.DrawPath(penBorder, path);

            e.Graphics.ResetTransform();
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

            int x0 = 0;
            GraphicsPath pth = new GraphicsPath();

            string s;

            #endregion declarations


            if (lineIndex >= _kLyrics.Lines.Count()) return;
            s = _kLyrics.Lines[lineIndex].ToString();


            #region Scale font size to fit text in picture box

            // ************************  Set ScaleTransform
            float w = MeasureString(s, _karaokeFont.Size);            
            // Example: if the text is greater than the width of the picture box, we reduce the size of the text to fit it in the picture box
            float scale = (pBox.Width - 50) / w;
            if (w > 0 && w > pBox.Width - 50)
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

            pth.AddString(s, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0 , y1), sf);

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

                    pathActive.AddString(active_fragment, _karaokeFont.FontFamily, (int)_karaokeFont.Style, _karaokeFont.Size, new Point(x0, y1), sf);

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
            int x0 = 0;
            Pen penBorder = new Pen(BorderColor, _borderthick); // pen for border color
            int Wbg;
            RectangleF Rbg;
            string s;
            
            #endregion Declarations

            if (lineIndex < _kLyrics.Lines.Count())
            {                
                s = _kLyrics.Lines[lineIndex].ToString();
                float w = MeasureString(s, _karaokeFont.Size);

                // ************************  Set ScaleTransform
                // Example: if the text is greater than the width of the picture box, we reduce the size of the text to fit it in the picture box
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

                // ************************  Reset ScaleTransform
                e.Graphics.ResetTransform();
            }

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

            float lineSpacing = 1.17f;
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
            //y0 = (int)MeasureStringHeight(FileName, 0.038f * _karaokeFont.Size);

            if (bDrawFileName)
                DrawFileName(e, FileName, 0.33f * _karaokeFont.Size);

            #endregion Draw FileName


            #region Line layout

            // Draw lines starting from this position
            y0 = (int)(0.36f * _lineHeight);

            // 4 lines positioning depending on the value of _FirstLineToShow % 4          
            int l = _FirstLineToShow;

            int a1 = y0;
            int a2 = y0 + _lineHeight;
            int a3 = y0 + (int)(_lineHeight * (1 + lineSpacing));   //_lineHeight + (int)(lineSpacing * _lineHeight);
            int a4 = y0 + (int)(_lineHeight * (2 + lineSpacing));   //_lineHeight + (int)(lineSpacing * _lineHeight) + _lineHeight;

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
                    y3 = y2 + (int)(lineSpacing * _lineHeight);     // idx3     _FirstLineToShow + 2      inactive                                           
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
                    y3 = y1 + (int)(lineSpacing * _lineHeight);     // idx3     _FirstLineToShow + 1     inactive                                          
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
                    y1 = y4 + (int)(lineSpacing * _lineHeight);     // idx1     _FirstLineToShow             current         (update 1 & 2)                        
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
                    y2 = y4 + (int)(lineSpacing * _lineHeight);     // idx2     _FirstLineToShow - 1      active                                                
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
                {
                    DrawInactiveLineWithBorders(e, idx2, y2, IsActive);
                }

                // Draw lines y3 and y4 (always inactives)
                if (idx3 < _kLyrics.Lines.Count)
                {
                    DrawInactiveLineWithBorders(e, idx3, y3);
                }
                if (idx4 < _kLyrics.Lines.Count)
                {
                    DrawInactiveLineWithBorders(e, idx4, y4);
                }

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


        #endregion Paint


        #region Measure

        private int HCenterText(Control pBox, string s)
        {
            int res = -(int)_karaokeFont.Size / 2 + (pBox.ClientSize.Width - (int)MeasureString(s, _karaokeFont.Size)) / 2;
            return res > 0 ? res : 0;
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
        private void AdjustText(float averageWidth,  int NbLines) 
        { 
            if (pBox == null) return;

            string S = "!/(123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Graphics g = pBox.CreateGraphics();
            float femsize;
            float inisize = _karaokeFont.Size;            
            femsize = g.DpiY * inisize / 72;

            // Try to fit inside 90% of client Height
            float ClientHeight = pBox.ClientSize.Height;

            float mult = 1.3f; // 1.2 is the default line spacing in Windows Forms
            float textHeight = MeasureStringHeight( S, femsize);
            float linesHeight = mult * textHeight * NbLines;
            

            if (linesHeight > ClientHeight)
            {
                do
                {
                    inisize--;
                    if (inisize > 0)
                    {
                        femsize = g.DpiY * inisize / 72;
                        linesHeight =  mult * MeasureStringHeight(S, femsize) * NbLines;

                    }
                } while (linesHeight > ClientHeight && inisize > 0);
            }
            else
            {
                do
                {
                    inisize++;                         
                    femsize = g.DpiY * inisize / 72;
                    linesHeight = mult * MeasureStringHeight( S, femsize) * NbLines;
                } while (linesHeight < ClientHeight);
            }

            // ------------------------------
            // Ajustement in width with AverageWidth
            // ------------------------------            
            float ClientWidth = 0.95f * pBox.ClientSize.Width;
            float textWidth = averageWidth;

            if (averageWidth > ClientWidth)
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
                _lineHeight = (int)(1.55 * emSize);
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

        
        private float MeasureLine(int curline, float femSize)
        {
            return MeasureString(_kLyrics.Lines[curline].ToString(), femSize);
        }


        #endregion Measure


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


        #region form load close

        private void frmTest_Resize(object sender, EventArgs e)
        {
           
            AdjustText(_AverageWidth, _nbLyricsLines);
            pBox.Invalidate();
           
        }

       

        #endregion form load close
    }
}
