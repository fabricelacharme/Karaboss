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
using Karaboss.Mp3.Mp3Lyrics;
using Karaboss.Themes;
using Karaboss.Utilities;
using keffect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Mp3
{    
    public partial class frmMp3Lyrics : Form, IMessageFilter
    {
        #region Declarations

        #region balls
        // Show balls
        private bool _bShowBalls = true;
        public bool bShowBalls
        {
            get { return _bShowBalls; }
            set
            {
                _bShowBalls = value;
                pnlTop.Visible = _bShowBalls;
            }
        }

        #endregion balls


        #region Colors            

        #region Background & text background color

        private bool _bTextBackGround = false;
        /// <summary>
        /// Black background of text
        /// </summary>
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set
            {
                _bTextBackGround = value;
                karaokeEffect1.bTextBackGround = _bTextBackGround;
            }
        }


        // Background color
        private Color _BgColor;
        public Color BgColor
        {
            get { return _BgColor; }
            set
            {
                _BgColor = value;
                karaokeEffect1.BgColor = _BgColor;
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
                karaokeEffect1.Grad0Color = _grad0Color;
            }
        }
        private Color _grad1Color;
        public Color Grad1Color
        {
            get { return _grad1Color; }
            set
            {
                _grad1Color = value;
                karaokeEffect1.Grad1Color = _grad1Color;
            }
        }
        private Color _Rhythm0Color;
        public Color Rhythm0Color
        {
            get { return _Rhythm0Color; }
            set
            {
                _Rhythm0Color = value;
                karaokeEffect1.Rhythm0Color = _Rhythm0Color;
            }
        }
        private Color _rhythm1Color;
        public Color Rhythm1Color
        {
            get { return _rhythm1Color; }
            set
            {
                _rhythm1Color = value;
                karaokeEffect1.Rhythm1Color = _rhythm1Color;
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
                karaokeEffect1.ActiveInstrumentalColor = _ActiveInstrumentalColor;
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
                karaokeEffect1.ActiveColor = _ActiveColor;
            }
        }

        private Color _HighlightColor;
        public Color HighlightColor
        {
            get { return _HighlightColor; }
            set
            {
                _HighlightColor = value;
                karaokeEffect1.HighlightColor = _HighlightColor;
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
                karaokeEffect1.InactiveColor = _InactiveColor;
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
                karaokeEffect1.ActiveBorderColor = _ActiveBorderColor;
            }
        }

        private Color _InactiveBorderColor;
        public Color InactiveBorderColor
        {
            get { return _InactiveBorderColor; }
            set
            {
                _InactiveBorderColor = value;
                karaokeEffect1.InactiveBorderColor = _InactiveBorderColor;
            }
        }

        #endregion text color

        #endregion Colors


        #region Draw filename

        private bool _bShowSongName = true;
        public bool bShowSongName
        {
            get { return _bShowSongName; }
            set
            {
                _bShowSongName = value;
                karaokeEffect1.bShowSongName = _bShowSongName;
            }
        }

        #endregion Draw filename


        #region Font
        private string ftName = "Arial Black";
        private uint ftSize = 20;

        private Font _karaokeFont;
        public Font KaraokeFont
        {
            get { return _karaokeFont; }
            set
            {
                try
                {
                    _karaokeFont = value;
                    // Redraw
                    karaokeEffect1.KaraokeFont = _karaokeFont;
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
                karaokeEffect1.FontStretching = _FontStretching;                
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

        #region Move form 

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private readonly HashSet<Control> controlsToMove = new HashSet<Control>();

        private Point Mouselocation;

        #endregion Move form

        #endregion Form


        #region Instrumental
        
        private bool _bShowHints = true;
        public bool bShowHints
        {
            get { return _bShowHints; }
            set
            {
                _bShowHints = value;
                karaokeEffect1.bShowHints = _bShowHints;
            }
        }

        #endregion Instrumental


        #region Karaoke display layout

        // Karaoke display types
        private string _karaokeDisplayType = "None";
        public string KaraokeDisplayType
        {
            get { return _karaokeDisplayType; }
            set
            {
                _karaokeDisplayType = value;

                switch (_karaokeDisplayType)
                {
                    case "FourLinesSwapped":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.FourLinesSwapped; break;
                    case "ConstantScrolling":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.ConstantScrolling; break;
                    case "DynamicScrolling":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.DynamicScrolling; break;
                    case "TwoLinesSwapped":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.TwoLinesSwapped; break;
                    case "FixedLines":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.FixedLines; break;
                    case "None":
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.None; break;
                    default:
                        karaokeEffect1.KaraokeDisplayType = kar.KaraokeDisplayTypes.FourLinesSwapped; break;
                }
            }
        }

        #endregion Karaoke display layout


        #region MP3

        // Duration in milliseconds (org Bass)
        private double _duration;
        public double Duration
        {
            get { return _duration; }

            set
            {
                _duration = value;
                karaokeEffect1.Duration = _duration;

            }
        }

        private int _bitrate;   // genre 192
        public int BitRate
        {
            get { return _bitrate; }
            set
            {

                _bitrate = value;
                karaokeEffect1.BitRate = _bitrate;
            }
        }

        // Frequency
        private float _frequency;
        public float Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                karaokeEffect1.Frequency = _frequency;
            }
        }

        #endregion MP3


        #region Picture

        private PictureBoxSizeMode _sizeMode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                karaokeEffect1.SizeMode = _sizeMode;
            }
        }

        #endregion Picture


        #region Playlists

        // Playlists
        private Playlist _currentPlaylist;
        public Playlist currentPlaylist
        {
            get { return _currentPlaylist; }
            set { _currentPlaylist = value; }
        }
        private PlaylistItem _currentPlaylistItem;
        public PlaylistItem currentPlaylistItem
        {
            get { return _currentPlaylistItem; }
            set { _currentPlaylistItem = value; }
        }

        #endregion Playlists


        #region Slideshow

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
                    karaokeEffect1.SingleImagePath = _SingleImagePath;
                }
            }
        }

        #endregion Single image


        #region SlideShow

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

                karaokeEffect1.SetDirectoryBackground(_dirSlideShow);
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
                karaokeEffect1.FreqSlideShow = _freqSlideShow;
            }
        }

        #endregion SlideShow

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
                        karaokeEffect1.OptionBackground = "Image";
                        break;

                    case "Diaporama":
                        karaokeEffect1.OptionBackground = "Diaporama";
                        break;
                    case "SolidColor":
                        karaokeEffect1.OptionBackground = "SolidColor";
                        break;

                    case "Gradient":
                        karaokeEffect1.OptionBackground = "Gradient";
                        break;
                    case "Rhythm":
                        karaokeEffect1.OptionBackground = "Rhythm";
                        break;

                    case "Transparent":
                        TransparencyKey = karaokeEffect1.TransparencyKey;
                        BackColor = karaokeEffect1.TransparencyKey;
                        karaokeEffect1.OptionBackground = "Transparent";
                        break;
                    default:
                        karaokeEffect1.OptionBackground = "Diaporama";
                        break;
                }
            }
        }

        #endregion dirslideshow


        #region Themes

        private ThemesListHelper _ThListHelper = new ThemesListHelper();
        private ThemesList _ThemesList; // = new ThemesList();
        private ThemeItem _currentTheme; // = new ThemeItem();

        #endregion Themes


        #region Text transform

        private long _timerintervall = 50;
        private int currentTextPos = 0;

        #region Frame type

        // lyrics non border, border 1px, 2px ... shadow, neon
        private string _frametype = "Frame1";
        public string FrameType
        {
            get { return _frametype; }
            set
            {
                _frametype = value;
                karaokeEffect1.FrameType = _frametype;                                
            }
        }

        #endregion


        #region Text position
        // Display lyrics top center bottom
        private Karaclass.OptionsDisplay _OptionDisplay;
        public Karaclass.OptionsDisplay OptionDisplay
        {
            get { return _OptionDisplay; }
            set
            {
                _OptionDisplay = value;
                karaokeEffect1.OptionDisplay = (keffect.KaraokeEffect.OptionsDisplay)_OptionDisplay;
            }
        }
        #endregion Text position

        // Force Uppercase
        private bool _bForceUppercase = false;
        public bool bForceUppercase
        {
            get { return _bForceUppercase; }
            set
            {

                if (value != _bForceUppercase)
                {
                    _bForceUppercase = value;
                    karaokeEffect1.bforceUppercase = _bForceUppercase;                    
                }
            }
        }

        private bool _bprogressivehighlight = false;
        public bool bProgressiveHighlight
        {
            get { return _bprogressivehighlight; }
            set 
            {  
                _bprogressivehighlight= value;
                karaokeEffect1.TransitionEffect = _bprogressivehighlight? keffect.KaraokeEffect.TransitionEffects.Progressive : keffect.KaraokeEffect.TransitionEffects.None;
            }
        }

        private int _nbLyricsLines = 3;
        // number of lines to display
        public int nbLyricsLines
        {
            get { return _nbLyricsLines; }
            set
            {
                _nbLyricsLines = value;
                karaokeEffect1.nbLyricsLines = _nbLyricsLines;
            }
        }

        #endregion Text transform
            

        #endregion Declarations


        /// <summary>
        /// Constructor
        /// </summary>
        public frmMp3Lyrics(string fileName, Playlist myPlayList = null)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;

            // Set song name in title
            karaokeEffect1.FileName = Path.GetFileNameWithoutExtension(fileName);

            #region Graphic optimization

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            #endregion Graphic optimization


            #region Move form without title bar

            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            // UserControls picball & pBox manage themselves this move.
            controlsToMove.Add(this.pnlWindow);

            #endregion Move form without title bar


            #region Events

            karaokeEffect1.DoubleClick += new DoubleClickEventHandler(karaokeEffect1_DoubleClick);
            this.karaokeEffect1.Close += new CloseEventHandler(karaokeEffect1_Close);
            this.karaokeEffect1.FullScreen += new FullScreenEventHandler(karaokeEffect1_FullScreen);
            this.karaokeEffect1.Options += new OptionsEventHandler(karaokeEffect1_Options);
            this.karaokeEffect1.TopMost += new TopMostEventHandler(karaokeEffect1_TopMost);

            AddMouseMoveHandler(this);

            #endregion Events


            #region playlists
            if (myPlayList != null)
            {
                // Playlists
                currentPlaylist = myPlayList;
                // Search file to play with its filename                
                currentPlaylistItem = currentPlaylist.Songs.Where(z => z.File == fileName).FirstOrDefault();            // ERROR FILENAME IS NOT THE FULL PATH !!!!
            }
            #endregion Playlists


            // Load options
            LoadOptions();            
        }


        #region balls

        /// <summary>
        /// Load balls times
        /// </summary>
        /// <param name="SyncLyrics"></param>
        public void LoadBallsTimes(kLyrics SyncLyrics)
        {
            #region guard
            if (!_bShowBalls || SyncLyrics.Lines.Count == 0) return;
            #endregion guard


            kLine syncline = new kLine();
            List<int> LyricsTimes = new List<int>();

            currentTextPos = 0;

            for (int i = 0; i < SyncLyrics.Lines.Count; i++)
            {
                syncline = SyncLyrics.Lines[i];

                for (int j = 0; j < syncline.Syllables.Count; j++)
                {
                    LyricsTimes.Add((int)syncline.Syllables[j].StartTime);
                }
            }

            picBalls.Division = 480; // myLyricsMgmt.Division;    // Equivalent for Division in mp3 ?????
            picBalls.LoadTimes(LyricsTimes);
            picBalls.Start();
        }


        /// <summary>
        /// Move balls according to songposition
        /// </summary>
        /// <param name="songposition"></param>
        public void MoveBalls(int songposition)
        {
            // Find syllabe related to songposition
            currentTextPos = FindIndexSyllabe(songposition);

            // déclencheur : timer_3
            // 21 balls: 1 fix, 20 moving to the fix one  
            // la position currentTextPos est calculée avec timer_2 et non pas timer_3 trop rapide    
            if (Karaclass.m_DisplayBalls)
                picBalls.MoveBallsToLyrics(songposition, currentTextPos);
        }

        /// <summary>
        /// Find syllabe related to songposition
        /// </summary>
        /// <param name="songposition"></param>
        /// <returns></returns>
        private int FindIndexSyllabe(int songposition)
        {
            int i = 0;
            int j = 0;

            int idx = 0;

            //if (Mp3LyricsMgmtHelper.SyncLyrics == null) return 0;
            if (Mp3LyricsMgmtHelper.mp3KaraokeLyrics == null) return 0;

            kLine syncline = new kLine();

            for (i = 0; i < Mp3LyricsMgmtHelper.mp3KaraokeLyrics.Lines.Count; i++)
            {
                syncline = Mp3LyricsMgmtHelper.mp3KaraokeLyrics.Lines[i];
                for (j = 0; j < syncline.Syllables.Count; j++)
                {
                    if (songposition < syncline.Syllables[j].StartTime)
                    {
                        return idx;

                    }
                    else
                    {
                        idx++;
                    }
                }
            }
            return 0;
        }

        public void UnlightFixedBall()
        {
            picBalls.UnlightFixedBall();
        }

        public void StartTimerBalls()
        {
            picBalls.BallsNumber = 22;
            picBalls.Start();
        }

        public void StopTimerBalls()
        {
            picBalls.Stop();
        }

        #endregion


        #region diaporama

        /// <summary>
        /// Stop diaporama
        /// </summary>
        public void StopDiaporama()
        {
            karaokeEffect1.Terminate();
        }


        #endregion diaporama


        #region Events

        private void karaokeEffect1_TopMost(object sender, bool bTopMost, EventArgs e)
        {
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                frmMp3Player frmMp3Player = FormUtilities.GetForm<frmMp3Player>();
                if (bTopMost)
                {
                    frmMp3Player.RemoveOwnedForms();
                }
                else
                {
                    frmMp3Player.RestoreOwnedForms();
                }

            }
        }

        private void karaokeEffect1_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else WindowState = FormWindowState.Maximized;
        }

        private void karaokeEffect1_Options(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Application.OpenForms.OfType<frmMp3LyrOptions>().Count() == 0)
            {
                frmMp3LyrOptions frmMp3LyrOptions = new frmMp3LyrOptions();
                frmMp3LyrOptions.Show();
            }
        }

        private void karaokeEffect1_FullScreen(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void karaokeEffect1_Close(object sender, EventArgs e)
        {
            Close();
        }

        #endregion Events


        #region initializations

        /// <summary>
        /// Load options
        /// </summary>
        private void LoadOptions()
        {
            try
            {
                // Load colors lyrics, backgrounds from current Theme
                LoadColorsFromCurrentTheme();


                // Karaoke display type
                KaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;               // setting this property set the karaokeEffect1.KaraokeDisplayType property

                // Lyrics border effect 
                _frametype = Properties.Settings.Default.FrameType;
                karaokeEffect1.FrameType = _frametype;

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);
                karaokeEffect1.KaraokeFont = _karaokeFont;

                // Font stretching
                FontStretching = Properties.Settings.Default.FontStretching;

                // Show hints for instrumental parts
                bShowHints = Properties.Settings.Default.bShowHints;

                // Show paragraphs
                karaokeEffect1.bShowParagraphs = Karaclass.m_ShowParagraph;

                // Display file name in lyrics as title
                karaokeEffect1.bShowSongName = Properties.Settings.Default.bShowSongName;

                // Progressive highlight
                bProgressiveHighlight = Properties.Settings.Default.bProgressiveHighlight;

                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;

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

                karaokeEffect1.timerIntervall = _timerintervall;

                // Load balls times
                if (_bShowBalls)
                    LoadBallsTimes(Mp3LyricsMgmtHelper.mp3KaraokeLyrics);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public void ApplyFromOptionsForm()
        {
            try
            {

                Cursor.Current = Cursors.WaitCursor;

                // Show balls
                bShowBalls = Karaclass.m_DisplayBalls;                

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                KaraokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);
                FontStretching = Properties.Settings.Default.FontStretching;


                // Borders
                FrameType = Properties.Settings.Default.FrameType;

                // Load colors lyrics, backgrounds from current Theme
                LoadColorsFromCurrentTheme();


                // force uppercase
                bForceUppercase = Properties.Settings.Default.bForceUppercase;

                // Show hints (introduction, instrumental, ending)
                bShowHints = Properties.Settings.Default.bShowHints;

                bShowSongName = Properties.Settings.Default.bShowSongName;

                //Window lyrics TopMost
                bTopMost = Properties.Settings.Default.frmMidiLyricsTopMost;


                nbLyricsLines = Properties.Settings.Default.TxtNbLines;

                SizeMode = Properties.Settings.Default.SizeMode;

                // Image, Diaporama, Backcolor ou transparent
                if (_currentPlaylistItem == null)
                    OptionBackground = Properties.Settings.Default.BackGroundOption;

                // Text display: Center, Top, Bottom
                string opd = Properties.Settings.Default.LyricsOptionDisplay;
                switch (opd)
                {
                    case "Top":
                        OptionDisplay = Karaclass.OptionsDisplay.Top;
                        break;
                    case "Center":
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        break;
                    case "Bottom":
                        OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                        break;
                    default:
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        break;
                }

                bTextBackGround = Properties.Settings.Default.bLyricsBackGround;

                // Display single image as background
                SingleImagePath = Properties.Settings.Default.SingleImagePath;

                // SlideShow frequency
                FreqSlideShow = Properties.Settings.Default.freqSlideShow;

                // directory for slide show
                if (_currentPlaylistItem == null)
                    DirSlideShow = Properties.Settings.Default.dirSlideShow;

                // Karaoke display type (FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped ..)
                KaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;

                Cursor.Current = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #endregion initializations


        #region lyrics        

        /// <summary>
        /// Load lyrics into karaokeEffect1.KLyrics
        /// </summary>
        /// <param name="lyrics"></param>
        public void SetLyrics(kLyrics lyrics)
        {
            karaokeEffect1.KLyrics = lyrics;
        }

        #endregion lyrics


        #region Move Window

        bool bPnlVisible = false;
        DateTime startTime;

        /// <summary>
        /// Show panel on mouse move with a timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {

            if (bPnlVisible == false && e.Location != Mouselocation)
            {
                Mouselocation = e.Location;
                Cursor.Show();

                bPnlVisible = true;
                pnlWindow.Visible = true;
                startTime = DateTime.Now;

                pnlTimer.Enabled = true;
                pnlTimer.Start();
            }
        }

        private void pnlTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan dur = DateTime.Now - startTime;
            if (dur > TimeSpan.FromSeconds(3))
            {
                pnlTimer.Stop();

                pnlWindow.Visible = false;
                bPnlVisible = false;

                Cursor.Hide();
            }
        }


        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private void pnlWindow_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void pnlWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void pnlWindow_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void pnlWindow_Resize(object sender, EventArgs e)
        {
            btnFrmClose.Top = 1;
            btnFrmMax.Top = btnFrmClose.Top + btnFrmClose.Height + 1;
            btnFrmMin.Top = btnFrmMax.Top + btnFrmMax.Height + 1;
            btnFrmOptions.Top = btnFrmMin.Top + btnFrmMin.Height + 1;
            btnExportLyricsToText.Top = btnFrmOptions.Top + btnFrmOptions.Height + 1;
        }



        /// <summary>
        /// Move form without title bar
        /// UserControls of the form manage themselves this move
        /// by sending the message to their parent form (this.ParentForm.Handle)
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        private void AddMouseMoveHandler(Control c)
        {
            c.MouseMove += MouseMoveHandler;
            if (c.Controls.Count > 0)
            {
                foreach (Control ct in c.Controls)
                    AddMouseMoveHandler(ct);
            }
        }


        #endregion Move Window


        #region public methods

        public void LoadWaitSong(int sec)
        {
            karaokeEffect1.LoadWaitSong(sec);
        }


        /// <summary>
        /// Displays a visual representation of a beat on the associated PictureBox control.
        /// </summary>
        /// <remarks>This method triggers the <c>OnBeat</c> method of the associated PictureBox control to
        /// display a beat. If the PictureBox control is not initialized, an error message is displayed to the
        /// user.</remarks>
        public void DisplayBeat(int beat, int bpm)
        {
            if (karaokeEffect1 == null)
            {
                MessageBox.Show("PictureBox control is not initialized.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            karaokeEffect1.OnBeat(beat, bpm);
        }

        /// <summary>
        /// Display singer and song names
        /// </summary>
        /// <param name="text"></param>
        public void DisplayText(List<string> Lines)
        {
            karaokeEffect1.DisplayText(Lines);
        }

        public void PlayStopActions(bool isStopped)
        {
            // Disable buttons for editing lyrics and chords
            btnEditLyrics.Enabled = isStopped;
        }
        public void Start()
        {
            karaokeEffect1.Start();
            
        }

        public void Stop()
        {
            karaokeEffect1.Stop();
            PlayStopActions(true);
        }

        public void SendPlayerPositionToKaraoke(double position)
        {
            karaokeEffect1.SetPos(position * 1000);

        }

        #endregion public method


        #region options

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

        #endregion options
                      

        #region Form Events
        private void frmMp3Lyrics_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMp3LyricsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMp3LyricsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMp3LyricsLocation = Location;
                    Properties.Settings.Default.frmMp3LyricsSize = Size;
                    Properties.Settings.Default.frmMp3LyricsMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }

        private void frmMp3Lyrics_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmMp3LyricsMaximized)
            {
                Location = Properties.Settings.Default.frmMp3LyricsLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMp3LyricsLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMp3LyricsSize;
            }
        }

        private void frmMp3Lyrics_Resize(object sender, EventArgs e)
        {
            
        }

        private void frmMp3Lyrics_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (WindowState == FormWindowState.Maximized)
                    {
                        WindowState = FormWindowState.Normal;
                    }
                    break;
            }
        }

        #endregion Form Events
             

        #region pnlWindow Events

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFrmClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFrmClose_MouseHover(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.CloseOver;
        }

        private void btnFrmClose_MouseLeave(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.Close;
        }

        /// <summary>
        /// Maximize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFrmMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }

        private void btnFrmMax_MouseLeave(object sender, EventArgs e)
        {
            btnFrmMax.Image = Properties.Resources.Max;
        }

        /// <summary>
        /// Minimize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFrmMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnFrmOptions_Click(object sender, EventArgs e)
        {            
            if (Application.OpenForms.OfType<frmMp3LyrOptions>().Count() == 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                frmMp3LyrOptions frmMp3LyrOptions = new frmMp3LyrOptions();
                //frmMp3LyrOptions.ShowDialog();
                frmMp3LyrOptions.Show();
            }
            else
            {
                frmMp3LyrOptions frmMp3LyrOptions = Utilities.FormUtilities.GetForm<frmMp3LyrOptions>();
                frmMp3LyrOptions.Focus();
            }
        }

        /// <summary>
        /// Export lyrics to text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportLyricsToText_Click(object sender, EventArgs e)
        {            
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                frmMp3Player frmMp3Player = Utilities.FormUtilities.GetForm<frmMp3Player>();
                frmMp3Player.ExportLyricsTags();
            }
        }

        /// <summary>
        /// Open form mp3 Lyrics edition on the same form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditLyrics_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                frmMp3Player frmMp3Player = Utilities.FormUtilities.GetForm<frmMp3Player>();
                frmMp3Player.DisplayMp3EditLyricsForm();
            }
        }



        #endregion


        #region SlideShow

        /// <summary>
        /// Remet les options courante pour le cas des playlists
        /// La cinématique d'attente bouzille tout
        /// </summary>
        /// <param name="dirSlideShow"></param>
        public void SetSlideShow(string dirSlideShow)
        {
            DirSlideShow = dirSlideShow;
        }


        #endregion SlideShow


        #region Themes Color

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



            //  MessageBox.Show("Theme not found for colors", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error) ;

        }

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

    }
}
