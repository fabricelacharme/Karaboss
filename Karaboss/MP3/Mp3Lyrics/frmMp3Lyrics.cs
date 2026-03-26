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

using Karaboss.Mp3.Mp3Lyrics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TagLib.Mpeg4;
using static keffect.KaraokeEffect;

namespace Karaboss.Mp3
{    

    public partial class frmMp3Lyrics : Form, IMessageFilter
    {
        #region Move form without title bar
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private readonly HashSet<Control> controlsToMove = new HashSet<Control>();

        private Point Mouselocation;

        #endregion
        
        private long _timerintervall = 50;

        private frmMp3LyrOptions frmMp3LyrOptions;

        private int currentTextPos = 0;
             

        #region properties

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

        #endregion Font


        // Frame type
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
                karaokeEffect1.OptionDisplay = (keffect.KaraokeEffect.OptionsDisplay)_OptionDisplay;
            }
        }


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


        #region text color

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
       

        #region gradient
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


        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set
            {
                _beatDuration = value;
                karaokeEffect1.BeatDuration = _beatDuration;
            }
        }

        #endregion gradient


        #endregion


        #region text characteristics

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
                karaokeEffect1.TransitionEffect = _bprogressivehighlight? TransitionEffects.Progressive : TransitionEffects.None;
            }
        }

        #endregion text characteristics


        #region dirslideshow

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

                // Change only if not in playlist mode
                //if (_bplaylist)
                //    return;

                if (value == null || value == "")
                    value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

                if (Directory.Exists(value))
                    _dirSlideShow = value;
                else
                    _dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

                karaokeEffect1.SetBackground(_dirSlideShow);
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
                karaokeEffect1.FreqDirSlideShow = _freqSlideShow;
            }
        }


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

        /// <summary>
        /// Background option : Diaporam, SolidColor, Transparent
        /// </summary>
        private string _optionbackground = "Diaporama";
        public string OptionBackground
        {
            get { return _optionbackground; }
            set
            {
                _optionbackground = value;

                switch (_optionbackground)
                {
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

        #endregion properties


        /// <summary>
        /// Constructor
        /// </summary>
        public frmMp3Lyrics()
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            #region Move form without title bar

            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            // UserControls picball & pBox manage themselves this move.            
            controlsToMove.Add(this.pnlTitle);
            controlsToMove.Add(this.lblTitle);
            
            #endregion

            LoadLyrics();
            
            AddMouseMoveHandler(this);
            LoadOptions();
            SetOptions();            
        }
        

        #region initializations

        /// <summary>
        /// Load options
        /// </summary>
        private void LoadOptions()
        {
            try
            {
                // Lyrics border effect 
                _frametype = Properties.Settings.Default.FrameType;
                karaokeEffect1.FrameType = _frametype;

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);                
                karaokeEffect1.KaraokeFont = _karaokeFont;
                
                // Progressive highlight
                bProgressiveHighlight = Properties.Settings.Default.bProgressiveHighlight;

                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;

                // show balls
                bShowBalls = Karaclass.m_DisplayBalls;

                string bgOption = Properties.Settings.Default.BackGroundOption;
                switch (bgOption)
                {

                    case "Diaporama":
                        _optionbackground = "Diaporama";
                        break;
                    case "SolidColor":
                        _optionbackground = "SolidColor";
                        break;

                    case "Gradient":
                        _optionbackground = "Gradient";
                        break;

                    case "Rhythm":
                        _optionbackground = "Rhythm";
                        break;

                    case "Transparent":
                        _optionbackground = "Transparent";
                        break;

                    default:
                        _optionbackground = "Diaporama";
                        break;                   
                }
                OptionBackground = _optionbackground;

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

                bTextBackGround = Properties.Settings.Default.bLyricsBackGround;

                // Background                
                BgColor = Parse(Properties.Settings.Default.BgColor);

                Grad0Color = Properties.Settings.Default.Grad0Color;
                Grad1Color = Properties.Settings.Default.Grad1Color;
                Rhythm0Color = Properties.Settings.Default.Rhythm0Color;
                Rhythm1Color = Properties.Settings.Default.Rhythm1Color;

                // Text colors
                InactiveColor = Parse(Properties.Settings.Default.InactiveColor);
                HighlightColor = Parse(Properties.Settings.Default.HighlightColor);
                ActiveColor = Parse(Properties.Settings.Default.ActiveColor);
                ActiveBorderColor = Parse(Properties.Settings.Default.ActiveBorderColor);
                InactiveBorderColor = Parse(Properties.Settings.Default.InactiveBorderColor);


                // Number of Lines to display
                _nbLyricsLines = Properties.Settings.Default.TxtNbLines;
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

        #endregion initializations


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
        public void DisplaySinger(string text)
        {
            lblTitle.Text = text;
        }

        public void Start()
        {
            karaokeEffect1.Start();
        }

        public void Stop()
        {
            karaokeEffect1.Stop();
        }

        public void GetPositionFromPlayer(double position)
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


        private void SetOptions()
        {
            karaokeEffect1.nbLyricsLines = _nbLyricsLines;
            karaokeEffect1.KaraokeFont = _karaokeFont;
            karaokeEffect1.timerIntervall = _timerintervall;

            // Load balls times
            if (_bShowBalls)
                LoadBallsTimes(Mp3LyricsMgmtHelper.SyncLyrics);
        }


        #endregion options


        #region lyrics
        /// <summary>
        /// Load lyrics from Mp3LyricsMgmtHelper.SyncTexts
        /// They must begin with \r\n because of PictureBox1_Paint
        /// </summary>
        private void LoadLyrics()
        {           
            if (Mp3LyricsMgmtHelper.SyncLyrics == null) return;

            // Karaoke Effect
            karaokeEffect1.TransitionEffect = TransitionEffects.None;                        
            karaokeEffect1.SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;                        

            karaokeEffect1.nbLyricsLines = 3;
        }

        /// <summary>
        /// Send lyrics to KaraokeEffect
        /// </summary>
        /// <param name="lyrics"></param>
        public void SetLyrics(List<List<kSyncText>> lyrics)
        {
            karaokeEffect1.SyncLyrics = lyrics;
        }
        #endregion lyrics

                                           
        #region diaporama

        /// <summary>
        /// Stop diaporama
        /// </summary>
        public void StopDiaporama()
        {
            karaokeEffect1.Terminate();
        }


        #endregion diaporama


        #region balls

        /// <summary>
        /// Load balls times
        /// </summary>
        /// <param name="SyncLyrics"></param>
        public void LoadBallsTimes(List<List<kSyncText>> SyncLyrics)
        {
            #region guard
            if (!_bShowBalls || SyncLyrics.Count == 0) return;
            #endregion guard


            List<kSyncText> syncline = new List<kSyncText>();
            List<int> LyricsTimes = new List<int>();

            currentTextPos = 0;

            for (int i = 0; i < SyncLyrics.Count; i++)
            {
                syncline = SyncLyrics[i];

                for (int j = 0; j < syncline.Count; j++)
                {
                    LyricsTimes.Add((int)syncline[j].Time);
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

            List<kSyncText> syncline = new List<kSyncText>();
            for (i = 0; i < Mp3LyricsMgmtHelper.SyncLyrics.Count; i++)
            {
                syncline = Mp3LyricsMgmtHelper.SyncLyrics[i];
                for (j = 0; j < syncline.Count; j++)
                {
                    if (songposition < syncline[j].Time)
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


        #region Images

        /// <summary>
        /// Remet les options courante pour le cas des playlists
        /// La cinématique d'attente bouzille tout
        /// </summary>
        /// <param name="dirSlideShow"></param>
        public void SetSlideShow(string dirSlideShow)
        {
            DirSlideShow = dirSlideShow;
        }

      
        /*
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
        */

        #endregion Images
       

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
            Cursor.Current = Cursors.WaitCursor;
            frmMp3LyrOptions = new frmMp3LyrOptions();
            //frmMp3LyrOptions.ShowDialog();
            frmMp3LyrOptions.Show();
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

       
    }
}
