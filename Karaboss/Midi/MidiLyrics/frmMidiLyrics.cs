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
using Karaboss.MidiLyrics;
using Karaboss.Themes;
using Karaboss.Utilities;
using PicControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmMidiLyrics : Form, IMessageFilter
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
                pnlBalls.Visible = _bShowBalls;
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
                pBox.bTextBackGround = _bTextBackGround;
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
                pBox.BgColor = _BgColor;
            }
        }


        #endregion Background & text background color


        #region Chords color

        // Chord color
        private Color _InactiveChordColor;
        public Color InactiveChordColor
        {
            get { return _InactiveChordColor; }
            set
            {
                _InactiveChordColor = value;
                pBox.InactiveChordColor = _InactiveChordColor;
            }
        }

        // Chord highlight color
        private Color _HighlightchordColor;
        public Color HighlightChordColor
        {
            get { return _HighlightchordColor; }
            set
            {
                _HighlightchordColor = value;
                pBox.HighlightChordColor = _HighlightchordColor;
            }
        }

        #endregion Chords color


        #region Gradients color

        private Color _grad0Color;
        public Color Grad0Color
        {
            get { return _grad0Color; }
            set
            {
                _grad0Color = value;
                pBox.Grad0Color = _grad0Color;
            }
        }
        private Color _grad1Color;
        public Color Grad1Color
        {
            get { return _grad1Color; }
            set
            {
                _grad1Color = value;
                pBox.Grad1Color = _grad1Color;
            }
        }
        private Color _Rhythm0Color;
        public Color Rhythm0Color
        {
            get { return _Rhythm0Color; }
            set
            {
                _Rhythm0Color = value;
                pBox.Rhythm0Color = _Rhythm0Color;
            }
        }
        private Color _rhythm1Color;
        public Color Rhythm1Color
        {
            get { return _rhythm1Color; }
            set
            {
                _rhythm1Color = value;
                pBox.Rhythm1Color = _rhythm1Color;
            }
        }

        #endregion Gradients color


        #region Instrumentals color

        private Color _ActiveInstrumentalColor;
        public Color ActiveInstrumentalColor
        {
            get { return _ActiveInstrumentalColor; }
            set
            {
                _ActiveInstrumentalColor = value;
                pBox.ActiveInstrumentalColor = _ActiveInstrumentalColor;
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
                pBox.ActiveColor = _ActiveColor;
            }
        }

        private Color _HighlightColor;
        public Color HighlightColor
        {
            get { return _HighlightColor; }
            set
            {
                _HighlightColor = value;
                pBox.HighlightColor = _HighlightColor;
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
                pBox.InactiveColor = _InactiveColor;
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
                pBox.ActiveBorderColor = _ActiveBorderColor;
            }
        }

        private Color _InactiveBorderColor;
        public Color InactiveBorderColor
        {
            get { return _InactiveBorderColor; }
            set
            {
                _InactiveBorderColor = value;
                pBox.InactiveBorderColor = _InactiveBorderColor;
            }
        }

        #endregion Text color

        #endregion Colors


        #region Draw filename

        private bool _bShowSongName = true;
        public bool bShowSongName
        {
            get { return _bShowSongName; }
            set
            {
                _bShowSongName = value;
                pBox.bShowSongName = _bShowSongName;
            }
        }

        private string _fileName = "Song name";
        public string FileName                  // Name of the song to display on the screen (Filename without extension)
        {
            get { return _fileName; }
            set
            {
                if (value != null)
                {
                    _fileName = value;
                    if (_bShowSongName)                    
                        pBox.FileName = _fileName;                                            
                }
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
                    pBox.KaraokeFont = _karaokeFont;
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
                pBox.FontStretching = _FontStretching;                
            }
        }

        #endregion Font


        #region Form

        public bool bShowChords
        {
            set
            {
                if (value != Karaclass.m_ShowChords)
                {
                    chkChords.Checked = value;
                }
            }
        }

        #region TopMost
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

        #endregion TopMost


        #region Move form without title bar

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private readonly HashSet<Control> controlsToMove = new HashSet<Control>();
        #endregion

        #endregion Form


        #region Instrumental

        private bool _bShowHints = true;
        public bool bShowHints
        {
            get { return _bShowHints; }
            set
            {
                _bShowHints = value;
                pBox.bShowHints = _bShowHints;
            }
        }

        #endregion Instrumental


        #region Karaoke display layout

        // Karaoke display Laout
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
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped; break;
                    case "ConstantScrolling":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.ConstantScrolling; break;
                    case "DynamicScrolling":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.DynamicScrolling; break;
                    case "TwoLinesSwapped":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.TwoLinesSwapped; break;
                    case "FixedLines":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FixedLines; break;
                    case "None":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.None; break;
                    default:
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped; break;
                }
            }
        }

        #endregion Karaoke display layout


        #region MIDI

        public MidiLyricsMgmt myLyricsMgmt { get; set; }

        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set
            {
                _beatDuration = value;
                pBox.BeatDuration = _beatDuration;
            }
        }

        private int _TotalTicks = 0;
        public int TotalTicks
        {
            get { return _TotalTicks; }
            set
            {
                if (value > 0)
                {
                    _TotalTicks = value;
                    pBox.TotalTicks = _TotalTicks;
                }
            }
        }

        private double _duration = 0;
        public double Duration
        {
            get { return _duration; }
            set
            {
                if (value > 0)
                {
                    _duration = value;
                    pBox.Duration = _duration;
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
                pBox.FirstMelodyNoteTicksOn = _FirstMelodyNoteTicksOn;
            }
        }

        #endregion MIDI


        #region Picture

        private PictureBoxSizeMode _sizeMode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                pBox.SizeMode = _sizeMode;
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

        private string _SingleImagePath;
        public string SingleImagePath
        {
            get => _SingleImagePath;
            set
            {
                if (File.Exists(value))
                {
                    _SingleImagePath = value;
                    pBox.SingleImagePath = _SingleImagePath;
                }
            }
        }


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
                
                pBox.SetDirectoryBackground(_dirSlideShow);
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
                pBox.FreqSlideShow = _freqSlideShow;
            }
        }


        /// <summary>
        /// Background option : Diaporam, SolidColor, Transparent
        /// </summary>
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
                        pBox.OptionBackground = "Image";
                        break;

                    case "Diaporama":                        
                        pBox.OptionBackground = "Diaporama";
                        break;
                    case "SolidColor":
                        pBox.OptionBackground = "SolidColor";
                        break;

                    case "Gradient":
                        pBox.OptionBackground = "Gradient";
                        break;
                    case "Rhythm":
                        pBox.OptionBackground = "Rhythm";
                        break;

                    case "Transparent":
                        TransparencyKey = pBox.TransparencyKey;
                        BackColor = pBox.TransparencyKey;
                        pBox.OptionBackground = "Transparent";
                        break;
                    default:
                        pBox.OptionBackground = "Diaporama";
                        break;
                }
            }
        }

        #endregion Slideshow


        #region Themes

        private ThemesListHelper _ThListHelper = new ThemesListHelper();
        private ThemesList _ThemesList; // = new ThemesList();
        private ThemeItem _currentTheme; // = new ThemeItem();

        #endregion Themes


        #region Text transform

        #region Internal lyrics separators

        private readonly string _InternalSepLines = "¼";
        private readonly string _InternalSepParagraphs = "½";

        #endregion Internal lyrics separators

        private long _timerintervall = 50;
        private int currentTextPos = 0;

        private Point Mouselocation;

        private List<int> LyricsTimes;

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
                    pBox.bforceUppercase = _bForceUppercase;
                    LoadSong(myLyricsMgmt.KLyrics);
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

        #region Frame type
        // Frame type
        private string _frametype = "Frame1";
        public string FrameType
        {
            get { return _frametype; }
            set
            {
                _frametype = value;
                pBox.FrameType = _frametype;
            }
        }

        #endregion Frame type


        #region Display top bottom center

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
                pBox.OptionDisplay = (PicControl.pictureBoxControl.OptionsDisplay)_OptionDisplay;
            }
        }

        #endregion Display top bottom center

        private int _nbLyricsLines = 3;
        // number of lines to display
        public int nbLyricsLines
        {
            get { return _nbLyricsLines; }
            set
            {
                _nbLyricsLines = value;
                pBox.nbLyricsLines = _nbLyricsLines;
            }
        }

        #endregion Text transform

      

        #endregion Declarations


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_myLyricsMgmt"></param>
        public frmMidiLyrics(MidiLyricsMgmt _myLyricsMgmt, string fileName, Playlist myPlayList = null)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;

            this.myLyricsMgmt = _myLyricsMgmt;

            // Set song name in title
            pBox.FileName = Path.GetFileNameWithoutExtension(fileName);

            #region MIDI

            BeatDuration = myLyricsMgmt.Division;
            TotalTicks = myLyricsMgmt.TotalTicks;
            Duration = myLyricsMgmt.Duration;
            FirstMelodyNoteTicksOn = myLyricsMgmt.FirstMelodyNoteTicksOn;

            #endregion MIDI

            
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
            //controlsToMove.Add(this.pnlTittle);
            //controlsToMove.Add(this.lblTittle);

            #endregion


            #region Events

            this.pBox.DoubleClick += new DoubleClickEventHandler(pBox_DoubleClick);
            this.pBox.Close += new CloseEventHandler(pBox_Close);
            this.pBox.FullScreen += new FullScreenEventHandler(pBox_FullScreen);
            this.pBox.Options += new OptionsEventHandler(pBox_Options);
            this.pBox.TopMost += new TopMostEventHandler(pBox_TopMost);

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

            // colours for text, chords, number of lines etc...
            LoadOptions();            
        }


        #region balls

        /// <summary>
        /// Load times for the Ball animation
        /// </summary>
        /// <param name="plLyrics"></param>
        public void LoadBallsTimes(kLyrics kl)
        {
            if (!bShowBalls || kl.Lines.Count == 0)
            { return; }

            LyricsTimes = new List<int>();
            /*
            for (int i = 0; i < kl.Lines.Count; i++)
            {
                for (int j = 0; j < kl.Lines[i].Syllables.Count; j++)
                {
                    Syllable syllable = kl.Lines[i].Syllables[j];
                    if (syllable.CharType == Syllable.CharTypes.Text || syllable.CharType == Syllable.CharTypes.ParagraphSep)
                    {
                        LyricsTimes.Add(syllable.TicksOn);
                    }
                }
            }
            */

            // Take lyrics times from the pBox which are transformed (trailing spaces added, instrumentals etc...)
            for (int i = 0; i < pBox.KLyrics.Lines.Count; i++)
            {
                for (int j = 0; j < pBox.KLyrics.Lines[i].Syllables.Count; j++)
                {
                    Syllable syllable = pBox.KLyrics.Lines[i].Syllables[j];
                    if (syllable.CharType == Syllable.CharTypes.Text || syllable.CharType == Syllable.CharTypes.ParagraphSep)
                    {
                        LyricsTimes.Add(syllable.TicksOn);
                    }
                }
            }

            picBalls.Division = myLyricsMgmt.Division;
            picBalls.LoadTimes(LyricsTimes);

            picBalls.Start();

        }

        /// <summary>
        /// Moves the balls to their positions based on the current song position.
        /// </summary>
        /// <remarks>This method updates the positions of the balls, with one fixed ball and others moving
        /// toward it, based on the provided song position. The actual position of the lyrics is calculated using a
        /// separate timer.</remarks>
        /// <param name="songposition">The sequencer position.</param>
        public void MoveBalls(int songposition)
        {
            // déclencheur : timer_3
            // 21 balls: 1 fix, 20 moving to the fix one  
            // la position currentTextPos est calculée avec timer_2 et non pas timer_3 trop rapide    
            if (Karaclass.m_DisplayBalls)
                picBalls.MoveBallsToLyrics(songposition, currentTextPos);
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


        #region Colors

        /// <summary>
        /// Check text representing a color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color Parse(string input)
        {
            if (input == null)
                return Color.Black;

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

        #endregion Colors


        #region Events

        private void pBox_TopMost(object sender, bool bTopMost, EventArgs e)
        {
            if (Application.OpenForms.OfType<frmMidiPlayer>().Count() > 0)
            {
                frmMidiPlayer frmMidiPlayer = FormUtilities.GetForm<frmMidiPlayer>();
                if (bTopMost)
                {
                    frmMidiPlayer.RemoveOwnedForms();
                }
                else
                {
                    frmMidiPlayer.RestoreOwnedForms();
                }

            }
        }

        private void pBox_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else WindowState = FormWindowState.Maximized;
        }

        private void pBox_Options(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Application.OpenForms.OfType<frmMidiLyrOptions>().Count() == 0)
            {
                frmMidiLyrOptions frmMidiLyrOptions = new frmMidiLyrOptions();
                frmMidiLyrOptions.Show();
            }
        }

        private void pBox_FullScreen(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
        }

        private void pBox_Close(object sender, EventArgs e)
        {
            Close();
        }


        #endregion Events


        #region Form events

        protected override void OnClosing(CancelEventArgs e)
        {
            //closing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.RemoveMessageFilter(this);

            timer1.Stop();
            timer1.Dispose();

            _karaokeFont?.Dispose();

            pBox.Terminate();

            // FAB 05/09/2024
            pBox.Dispose();
            picBalls.Stop();
            picBalls.Dispose();

            if (Application.OpenForms.OfType<frmMidiLyrOptions>().Count() > 0)
            {
                frmMidiLyrOptions frmMidiLyrOptions = Utilities.FormUtilities.GetForm<frmMidiLyrOptions>();
                frmMidiLyrOptions?.Dispose();
            }

            base.OnClosed(e);
        }

        private void frmMidiLyrics_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmMidiLyricsMaximized)
            {
                Location = Properties.Settings.Default.frmMidiLyricsLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMidiLyricsLocation;
                // Verify if this windows is visible in extended screens 
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMidiLyricsSize;
            }
        }

        private void frmMidiLyrics_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMidiLyricsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMidiLyricsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMidiLyricsLocation = Location;
                    Properties.Settings.Default.frmMidiLyricsSize = Size;
                    Properties.Settings.Default.frmMidiLyricsMaximized = false;

                }
                // Save settings
                Properties.Settings.Default.Save();
            }

            Dispose();

        }

        private void frmMidiLyrics_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                btnFrmMax.Image = Properties.Resources.MaxNormal;
            }
            else
            {
                btnFrmMax.Image = global::Karaboss.Properties.Resources.Max;
            }
        }

        private void frmMidiLyrics_KeyDown(object sender, KeyEventArgs e)
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


        #endregion Form events


        #region initializations

        /// <summary>
        /// Load options (text color, 
        /// </summary>
        public void LoadOptions()
        {
            try
            {
                // Load colors lyrics, backgrounds from current Theme
                LoadColorsFromCurrentTheme();
                
                // Karaoke display type
                KaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;               // setting this property set the karaokeEffect1.KaraokeDisplayType property

                // Lyrics border effect 
                _frametype = Properties.Settings.Default.FrameType;
                pBox.FrameType = _frametype;               

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);                
                pBox.KaraokeFont = _karaokeFont;

                // Font stretching
                FontStretching = Properties.Settings.Default.FontStretching;

                // Display hints for instrumental parts
                bShowHints = Properties.Settings.Default.bShowHints;

                // Display paragraphs
                pBox.bShowParagraphs = Karaclass.m_ShowParagraph;

                // Display file name in lyrics as title
                pBox.bShowSongName = Properties.Settings.Default.bShowSongName;

                // Progressive highlight
                bProgressiveHighlight = Properties.Settings.Default.bProgressiveHighlight;

                // Display chords ?
                chkChords.Checked = Karaclass.m_ShowChords;
                pBox.bShowChords = Karaclass.m_ShowChords;

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
                        _OptionDisplay = Karaclass.OptionsDisplay.Top; break;
                    case "Center":
                        _OptionDisplay = Karaclass.OptionsDisplay.Center; break;
                    case "Bottom":
                        _OptionDisplay = Karaclass.OptionsDisplay.Bottom; break;
                    default:
                        _OptionDisplay = Karaclass.OptionsDisplay.Center; break;
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

                bTopMost = Properties.Settings.Default.frmMidiLyricsTopMost;

                pBox.timerIntervall = _timerintervall;

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
                
                // Show chords
                bShowChords = Properties.Settings.Default.bShowChords;

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
       

        #region public methods

        /// <summary>
        /// Displays a visual representation of a beat on the associated PictureBox control.
        /// </summary>
        /// <remarks>This method triggers the <c>OnBeat</c> method of the associated PictureBox control to
        /// display a beat. If the PictureBox control is not initialized, an error message is displayed to the
        /// user.</remarks>
        public void DisplayBeat(int beat, int bpm)
        {
            if (pBox == null)
            {
                MessageBox.Show("PictureBox control is not initialized.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pBox.OnBeat(beat, bpm);
        }


      
        /// <summary>
        /// Count Down: decreasing numbers to wait for next song to start
        /// </summary>
        /// <param name="sec"></param>
        public void LoadWaitSong(int sec)
        {
            pBox.LoadWaitSong(sec);
        }

        public void EndWaitSong()
        {
            pBox.endDemoText();
        }

        public void DisplayText(List<string>Lines)
        {
            pBox.DisplayText(Lines);
        }

       
        /// <summary>
        /// Load song in picturebox control
        ///  1/4 = LineFeed
        ///  1/2 = Paragraph
        /// </summary>
        public void LoadSong(kLyrics kl)
        {
            currentTextPos = 0;
                   
            // Load kLyrics with kLyrics to have all the information for chords and lyrics positions, used for balls animation
            pBox.KLyrics = kl;

            // Force Uppercase         
            pBox.bforceUppercase = _bForceUppercase;
            pBox.bShowChords = Karaclass.m_ShowChords;

            //Initial position
            pBox.CurrentTextPos = -1;

            if (bShowBalls)
                LoadBallsTimes(kl);
        }

        /*
        /// <summary>
        /// Color the syllabe according to song position
        /// </summary>
        /// <param name="songposition"></param>
        public void ColorLyric(int SequencerPosition)
        {
            // déclencheur : timer_2
            // IMPERATIF : calculer ici la position de la syllabe, utilisée pour l'animation des balles
            // drivé par timer_2 de frmMidiPplayer            
            currentTextPos = pBox.CurrentTextPos;
            //pBox.ColorLyric(songposition);
            pBox.SetPos(SequencerPosition);
        }
        */

        /*
        /// <summary>
        /// Reset display at begining
        /// </summary>
        public void ResetTop()
        {
            currentTextPos = 0;
            pBox.ResetTop();
        }
        */
        public void PlayStopActions(bool isStopped)
        {
            // Disable buttons for editing lyrics and chords
            btnEditLyrics.Enabled = isStopped;
            btnEditLyricsChords.Enabled = isStopped;
        }


        public void Start()
        {
            pBox.Start();
        }

        public void Stop()
        {
            pBox.Stop();
            PlayStopActions(true);
        }

        /// <summary>
        /// Color the syllabe according to song position
        /// </summary>
        /// <param name="songposition"></param>
        public void SendPlayerPositionToKaraoke(int SequencerPosition)
        {
            pBox.SetPos(SequencerPosition);

        }


        public void StopDiaporama()
        {
            pBox.Terminate();
        }
     

        #endregion public methods
               

        #region Move Window
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


        #region pnlWindow Events

        /// <summary>
        /// Edit lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditLyrics_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<frmMidiPlayer>().Count() > 0)
            {
                frmMidiPlayer frmMidiPlayer = Utilities.FormUtilities.GetForm<frmMidiPlayer>();
                frmMidiPlayer.DisplayEditLyricsForm();
            }
        }


        /// <summary>
        /// Export lyrics to text editor (notepad, notepad++ etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void btnExportLyricsToText_Click(object sender, EventArgs e)
        {
            #region check
            if (myLyricsMgmt.Lyrics == null || myLyricsMgmt.Lyrics == "")
                return;
            #endregion            

            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            // Lyrics not modified
            tx = myLyricsMgmt.Lyrics;

            tx = tx.Replace(_InternalSepParagraphs, "\r\n\r\n");
            tx = tx.Replace(_InternalSepLines, "\r\n");
            tx = tx.Replace("[]", "");                                  // Why are these characters exists ?
            System.IO.File.WriteAllText(@file, tx);

            try
            {
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region lyrics & chords

        /// <summary>
        /// CheckBox: Display chords when checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkChords_CheckedChanged(object sender, EventArgs e)
        {
            // Hide or Show the button for displaying lyrics + chords
            btnExportLyricsChords.Visible = chkChords.Checked;
            btnEditLyricsChords.Visible = chkChords.Checked;

            // User has manually changed the display of chords
            if (chkChords.Checked != Karaclass.m_ShowChords)
            {
                // Set cursor as hourglass
                Cursor.Current = Cursors.WaitCursor;

                Karaclass.m_ShowChords = chkChords.Checked;
                pBox.bShowChords = Karaclass.m_ShowChords;

                // Save option
                Properties.Settings.Default.bShowChords = Karaclass.m_ShowChords;
                Properties.Settings.Default.Save();

                // Reload lyrics with choosen options
                myLyricsMgmt.ResetDisplayChordsOptions(chkChords.Checked);

                // Load modified lyrics into the picturebox
                LoadSong(myLyricsMgmt.KLyrics);

                // Refresh score with or without chords
                frmMidiPlayer frmMidiPlayer = Utilities.FormUtilities.GetForm<frmMidiPlayer>();
                frmMidiPlayer.RefreshChordsSheetMusic();

                // Set cursor as default
                Cursor.Current = Cursors.Default;

            }

        }

        /// <summary>
        /// Export lyrics and chords to text editor (notepad, notepad++, etc...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>       
        private void btnExportLyricsChordsToText_Click(object sender, EventArgs e)
        {
            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            // Chords are in the lyrics
            if (myLyricsMgmt.bHasChordsInLyrics)
            {
                if (myLyricsMgmt.GridBeatChords == null)
                {
                    myLyricsMgmt.FillGridBeatChordsWithLyricsChords();
                }
            }
            else
            {
                // Chords have to be guessed with a vertical search                
                myLyricsMgmt.KLyrics = myLyricsMgmt.PopulateDetectedChords(myLyricsMgmt.KLyrics);
                myLyricsMgmt.CleanLyricsWithChords();
            }

            tx = myLyricsMgmt.GetLyricsLinesWithChords();
            System.IO.File.WriteAllText(@file, tx);

            try
            {
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Edit lyrics & chords
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditLyricsChords_Click(object sender, EventArgs e)
        {
            frmMidiPlayer frmMidiPlayer = Utilities.FormUtilities.GetForm<frmMidiPlayer>();
            frmMidiPlayer.DisplayEditLyricsChordsForm();
        }

        #endregion lyrivs & chords


        #region panel events

        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Maximize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Minimize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Open form frmMidiLyrOptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmOptions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Application.OpenForms.OfType<frmMidiLyrOptions>().Count() == 0)
            {
                frmMidiLyrOptions frmMidiLyrOptions = new frmMidiLyrOptions();                
                frmMidiLyrOptions.Show();
            }
        }


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

                timer1.Enabled = true;
                timer1.Start();
            }
        }

        /// <summary>
        /// Timer used to hide panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            TimeSpan dur = DateTime.Now - startTime;
            if (dur > TimeSpan.FromSeconds(3))
            {
                timer1.Stop();

                pnlWindow.Visible = false;
                bPnlVisible = false;

                Cursor.Hide();
            }
        }
        private void PnlWindow_Resize(object sender, EventArgs e)
        {
            btnFrmClose.Top = 1;
            btnFrmMax.Top = btnFrmClose.Top + btnFrmClose.Height + 1;
            btnFrmMin.Top = btnFrmMax.Top + btnFrmMax.Height + 1;
            btnFrmOptions.Top = btnFrmMin.Top + btnFrmMin.Height + 1;
            btnExportLyricsToText.Top = btnFrmOptions.Top + btnFrmOptions.Height + 1;
        }

        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private void PnlWindow_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void PnlWindow_MouseMove(object sender, MouseEventArgs e)
        {
            //this.Cursor = Cursors.Hand;

            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void PnlWindow_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void BtnFrmClose_MouseHover(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.CloseOver;
        }

        private void BtnFrmClose_MouseLeave(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.Close;
        }


        #endregion panel events


        #endregion pnlWindow Events       


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

        /// <summary>
        /// Use case : Plalist
        /// Force Slideshow backgroud if it was requested in the playlist, even if the option is not set in the display options
        /// </summary>
        /// <param name="dirSlideShow"></param>
        public void ForceSlideShow(string dirSlideShow)
        {
            OptionBackground = "Diaporama";
            DirSlideShow = dirSlideShow;            
            pBox.FreqSlideShow = Properties.Settings.Default.freqSlideShow;            
            
        }

        /// <summary>
        /// Use case: Playlists
        /// No slide show was requested in the playlist, but the slideshow was forced for the previous song, so restore background option to the one set in display options
        /// </summary>
        public void RestoreBackgroundAnimation()
        {
            _optionbackground = Properties.Settings.Default.BackGroundOption;
            
            if (_optionbackground == "Diaporama")
            {
                pBox.FreqSlideShow = Properties.Settings.Default.freqSlideShow;
                DirSlideShow = Properties.Settings.Default.dirSlideShow;                
            }

            pBox.OptionBackground = _optionbackground;
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
            InactiveChordColor = Parse(_currentTheme.InactiveChordColor);
            HighlightChordColor = Parse(_currentTheme.HighlightChordColor);

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
