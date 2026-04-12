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

using Karaboss.MidiLyrics;
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
using kar;

namespace Karaboss
{
    public partial class frmMidiLyrics : Form, IMessageFilter
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
        #endregion


        #region private declarations
        

        public MidiLyricsMgmt myLyricsMgmt { get; set; }               


        private int currentTextPos = 0;
        private Point Mouselocation;

        //private frmLyrOptions frmLyrOptions;
        private List<int> LyricsTimes;

        #endregion private


        #region properties

        #region Internal lyrics separators

        private readonly string _InternalSepLines = "¼";
        private readonly string _InternalSepParagraphs = "½";

        #endregion


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


        #region chords

        // Chord color
        private Color _chordNextColor;
        public Color ChordNextColor
        {
            get { return _chordNextColor; }
            set
            {
                _chordNextColor = value;
                pBox.InactiveChordColor = _chordNextColor;
            }
        }
        // Chord highlight color
        private Color _chordHighlightColor;
        public Color ChordHighlightColor
        {
            get { return _chordHighlightColor; }
            set
            {
                _chordHighlightColor = value;
                pBox.HighlightChordColor = _chordHighlightColor;
            }
        }

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


        #endregion chords


        #region text characteristics

        // Force Uppercase
        //private List<plLyric> _plLyrics;

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

        #endregion Font
      

        // Frame type
        private string _frametype = "Frame1";
        public string FrameType
        {
            get { return _frametype; }
            set { 
                _frametype = value; 
                pBox.FrameType = _frametype;                
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
                pBox.OptionDisplay = (PicControl.pictureBoxControl.OptionsDisplay)_OptionDisplay;
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
                pBox.nbLyricsLines = _nbLyricsLines;
            }
        }

        #endregion


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
                pBox.bTextBackGround = _bTextBackGround;
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

        #endregion text color

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


        #region gradient
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

        #endregion gradient


        #endregion


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

                pBox.DirSlideShow = _dirSlideShow;
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
                pBox.FreqDirSlideShow = _freqSlideShow;
            }
        }


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
                        pBox.DirSlideShow = DirSlideShow;
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

        #endregion dirslideshow


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

       
        #endregion properties


        //public List<pictureBoxControl.plLyric> plLyrics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_myLyricsMgmt"></param>
        public frmMidiLyrics(MidiLyricsMgmt _myLyricsMgmt)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;

            this.myLyricsMgmt = _myLyricsMgmt;

            // Graphic optimization
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            #region Move form without title bar

            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            // UserControls picball & pBox manage themselves this move.            
            controlsToMove.Add(this.pnlTittle);
            controlsToMove.Add(this.lblTittle);

            #endregion


            // colours for text, chords, number of lines etc...
            LoadKarOptions();

            AddMouseMoveHandler(this);
        }


        #region initializations

        /// <summary>
        /// Load options (text color, 
        /// </summary>
        public void LoadKarOptions()
        {
            try
            {
                // Lyrics border effect 
                _frametype = Properties.Settings.Default.FrameType;
                pBox.FrameType = _frametype;               

                // Font
                ftName = Properties.Settings.Default.KaraokeFontName;
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);                
                pBox.KaraokeFont = _karaokeFont;

                pBox.bShowParagraphs = Karaclass.m_ShowParagraph;
                pBox.bShowChords = Karaclass.m_ShowChords;

                // Force Uppercase
                _bForceUppercase = Karaclass.m_ForceUppercase;

                // show balls
                bShowBalls = Karaclass.m_DisplayBalls;

                string bgOption = Properties.Settings.Default.BackGroundOption;
                switch (bgOption)
                {
                    case "Diaporama":
                        // Frequency of slide show
                        FreqSlideShow = Properties.Settings.Default.freqSlideShow;
                        DirSlideShow = Properties.Settings.Default.dirSlideShow;                        
                        OptionBackground = "Diaporama";
                        break;
                    case "SolidColor":
                        OptionBackground = "SolidColor";
                        break;

                    case "Gradient":
                        OptionBackground = "Gradient";
                        break;

                    case "Rhythm":
                        OptionBackground = "Rhythm";
                        break;

                    case "Transparent":
                        OptionBackground = "Transparent";
                        break;

                    default:
                        OptionBackground = "Diaporama";
                        break;
                }
                //OptionBackground = _optionbackground;

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

                // Background colors
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

                // Chords
                _chordNextColor = Parse(Properties.Settings.Default.InactiveChordColor);
                _chordHighlightColor = Parse(Properties.Settings.Default.HighlightChordColor);
                chkChords.Checked = Karaclass.m_ShowChords;


                // Number of Lines to display
                nbLyricsLines = Properties.Settings.Default.TxtNbLines;
               
                // Position image
                SizeMode = Properties.Settings.Default.SizeMode;

                bTopMost = Properties.Settings.Default.frmMidiLyricsTopMost;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion initializations


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
        /// Display singer and song names
        /// </summary>
        /// <param name="text"></param>
        public void DisplaySinger(string text)
        {
            lblTittle.Text = text;
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

        public void DisplayText(string tx, int ticks = 0)
        {
            pBox.DisplayText(tx, ticks);
        }

        /// <summary>
        /// Use case : Plalist
        /// Force Slideshow backgroud if it was requested in the plalist, even if the option is not set in the display options
        /// </summary>
        /// <param name="dirSlideShow"></param>
        public void ForceSlideShow(string dirSlideShow)
        {
            DirSlideShow = dirSlideShow;
            pBox.FreqDirSlideShow = Properties.Settings.Default.freqSlideShow;
            pBox.DirSlideShow = DirSlideShow;
            pBox.OptionBackground = "Diaporama";
            
        }

        /// <summary>
        /// Use case: Plalists
        /// No slide show waq requested in the plalist, but the slideshow was forced for the previous song, so restore background option to the one set in display options
        /// </summary>
        public void RestoreBackgroundAnimation()
        {
            if (_optionbackground == "Diaporama")
            {                                            
                pBox.FreqDirSlideShow = Properties.Settings.Default.freqSlideShow;
                DirSlideShow = Properties.Settings.Default.dirSlideShow;                
                pBox.DirSlideShow = DirSlideShow;
            }
            
            pBox.OptionBackground = _optionbackground;
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


        /// <summary>
        /// Color the syllabe according to song position
        /// </summary>
        /// <param name="songposition"></param>
        public void ColorLyric(int songposition)
        {
            // déclencheur : timer_2
            // IMPERATIF : calculer ici la position de la syllabe, utilisée pour l'animation des balles
            // drivé par timer_2 de frmMidiPplayer            
            currentTextPos = pBox.CurrentTextPos;
            pBox.ColorLyric(songposition);
        }

        /// <summary>
        /// Reset display at begining
        /// </summary>
        public void ResetTop()
        {
            currentTextPos = 0;
            pBox.ResetTop();
        }

        public void PlayStopActions(bool isStopped)
        {
            // Disable buttons for editing lyrics and chords
            btnEditLyrics.Enabled = isStopped;
            btnEditLyricsChords.Enabled = isStopped;
        }


        public void StopDiaporama()
        {
            pBox.Terminate();
        }
     

        #endregion public methods


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


        #region form load close resize

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

            if (Application.OpenForms.OfType<frmLyrOptions>().Count() > 0)
            {
                frmLyrOptions frmLyrOptions = Utilities.FormUtilities.GetForm<frmLyrOptions>();
                frmLyrOptions?.Dispose();
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


        #endregion form load close resize


        #region pnlWindow

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
        /// Open form frmLyrOptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmOptions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Application.OpenForms.OfType<frmLyrOptions>().Count() == 0)
            {
                frmLyrOptions frmLyrOptions = new frmLyrOptions();
                //frmLyrOptions.ShowDialog();
                frmLyrOptions.Show();
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

        #endregion pnlWindow        


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


    }
}
