#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using PicControl;
using System.Runtime.InteropServices;
using System.Linq;
using Karaboss.Lyrics;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Karaboss
{
    public partial class frmLyric : Form, IMessageFilter
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

        private LyricsMgmt myLyricsMgmt;

        private Font _karaokeFont;
        private string lyrics;
        private int currentTextPos = 0;
        private Point Mouselocation;

        private bool _bplaylist;

        private bool closing = false;

        //private bool busy = false;

        #region properties

        #region Internal lyrics separators

        private string _InternalSepLines = "¼";        
        private string _InternalSepParagraphs = "½";
        
        #endregion

        // Show balls
        private bool _bShowBalls = true;
        public bool bShowBalls
        {
            get { return _bShowBalls; }
            set { _bShowBalls = value;
                pnlBalls.Visible = _bShowBalls;
            }
        }

        #region chords

        // Chord color
        private Color _chordNextColor;
        public Color ChordNextColor
        {
            get { return _chordNextColor; }
            set
            {
                _chordNextColor = value;
                pBox.ChordNextColor = _chordNextColor;
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
                pBox.ChordHighlightColor = _chordHighlightColor;
            }
        }

        private bool _bShowChords = false;
        public bool bShowChords
        {
            get { return _bShowChords; } 
            set {
                if (value != _bShowChords)
                {
                    _bShowChords = value;
                    
                    ResetDisplayChordsOptions();
                    //pBox.bShowChords = _bShowChords;
                    

                }
            }
        }

        #endregion chords


        #region text characteristics

        // Force Uppercase
        private List<plLyric> _plLyrics;

        private bool _bForceUppercase = false;
        public bool bForceUppercase
        {
            get { return _bForceUppercase; }
            set { 
                _bForceUppercase = value;
                pBox.bforceUppercase = _bForceUppercase;
                LoadSong(_plLyrics);
            }
        }


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

        private Karaclass.OptionsDisplay _OptionDisplay;
        /// <summary>
        /// Display lyrics option: top, Center, Bottom
        /// </summary>
        public Karaclass.OptionsDisplay OptionDisplay
        {
            get { return _OptionDisplay; }
            set { _OptionDisplay = value;
                pBox.OptionDisplay = (PicControl.pictureBoxControl.OptionsDisplay)_OptionDisplay;
            }
        }

        private int _NbLines = 1;
        // number of lines to display
        public int TxtNbLines
        {
            get { return _NbLines; }
            set
            {
                _NbLines = value;
                pBox.TxtNbLines = _NbLines;
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
            set { _bTextBackGround = value;
                pBox.bTextBackGround = _bTextBackGround;
            }
        }

        // Text color
        private Color _txtHighlightColor;
        public Color TxtHighlightColor {
            get { return _txtHighlightColor; }
            set { _txtHighlightColor = value;
                pBox.TxtHighlightColor = _txtHighlightColor;
            }
        }
    
        // Text to sing color
        private Color _txtNextColor;
        public Color TxtNextColor { get { return _txtNextColor; } set {_txtNextColor = value;
                pBox.TxtNextColor = _txtNextColor;
            }
        }
        // Text sung color
        private Color _txtBeforeColor;
        public Color TxtBeforeColor { get {return _txtBeforeColor; } set {_txtBeforeColor = value;
                pBox.TxtBeforeColor = _txtBeforeColor;
            }
        }
        // Contour
        private bool _bColorContour = true;
        public bool bColorContour
        {
            get
            { return _bColorContour; }
            set
            {
                _bColorContour = value;
                pBox.bColorContour = _bColorContour;
            }
        }
        // Text contour
        private Color _txtContourColor;
        public Color TxtContourColor { get {return _txtContourColor; } set { _txtContourColor = value;
                pBox.TxtContourColor = _txtContourColor;
            }
        }
        
        // Background color
        private Color _txtBackColor;
        public Color TxtBackColor { get { return _txtBackColor; }
            set {_txtBackColor = value;
                pBox.TxtBackColor = _txtBackColor;
            }
        }


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
            set {

                // Change only if not in playlist mode
                //if (_bplaylist)
                //    return;

                if (value == null || value == "")
                    value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

                if (Directory.Exists(value))
                    _dirSlideShow = value;
                else                         
                    _dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);                    
                
                pBox.SetBackground(_dirSlideShow);
            }
        }
        
        // SlideShow frequency
        private int _freqSlideShow;
        public int FreqSlideShow {
            get { return _freqSlideShow; }
            set { _freqSlideShow = value;
                pBox.FreqDirSlideShow = _freqSlideShow;
            }
        }
        #endregion

        private PictureBoxSizeMode _sizeMode;
        public PictureBoxSizeMode SizeMode {
            get {return _sizeMode; }
            set {
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
                        pBox.OptionBackground = "Diaporama";
                        break;
                    case "SolidColor":
                        pBox.OptionBackground = "SolidColor";
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


        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set {
                _beatDuration = value;
                pBox.BeatDuration = _beatDuration;
            }
        }


        #endregion properties

        public List<pictureBoxControl.plLyric> plLyrics;
        private List<int> LyricsTimes;

        
        private frmLyrOptions frmLyrOptions;               
       
        public frmLyric(LyricsMgmt myLyricsMgmt, bool bPlayList = false)
        {
            InitializeComponent();

            this.myLyricsMgmt = myLyricsMgmt;

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

            // Check if a playlist is played
            _bplaylist = bPlayList;

            // couleurs pour texte, nombre de lignes
            LoadKarOptions();

            // parameters of chords included in lyrics
            SetDisplayChordsOptions();

            AddMouseMoveHandler(this);           
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
  
        #region methods        

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
        /// Load options (text color, 
        /// </summary>
        private void LoadKarOptions()
        {
            try
            {                
                _karaokeFont = Properties.Settings.Default.KaraokeFont;
                pBox.KaraokeFont = _karaokeFont;
                pBox.bShowParagraphs = Karaclass.m_ShowParagraph;


                // Force Uppercase
                _bForceUppercase = Karaclass.m_ForceUppercase;


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

                TxtBackColor = Properties.Settings.Default.TxtBackColor;

                // Text colors
                TxtNextColor = Properties.Settings.Default.TxtNextColor;
                TxtHighlightColor = Properties.Settings.Default.TxtHighlightColor;
                TxtBeforeColor = Properties.Settings.Default.TxtBeforeColor;
                bColorContour = Properties.Settings.Default.bColorContour;
                TxtContourColor = Properties.Settings.Default.TxtContourColor;

                // Chords
                _chordNextColor = Properties.Settings.Default.ChordNextColor;
                _chordHighlightColor = Properties.Settings.Default.ChordHighlightColor;
                bShowChords = Properties.Settings.Default.bShowChords;
                chkChords.Checked = bShowChords;              

                // Number of Lines to display
                TxtNbLines = Properties.Settings.Default.TxtNbLines;
                // Frequency of slide show
                FreqSlideShow = Properties.Settings.Default.freqSlideShow;
                // Position image
                SizeMode = Properties.Settings.Default.SizeMode;

                // Show panel balls
                bShowBalls = Karaclass.m_DisplayBalls;
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
        }

        /// <summary>
        /// Remet les options courante pour le cas des playlists
        /// La cinématique d'attente bouzille tout
        /// </summary>
        /// <param name="dirSlideShow"></param>
        public void SetKarOptions(string dirSlideShow)
        {
            LoadKarOptions();

            //AlloModifyDirSlideShow = true;
            DirSlideShow = dirSlideShow;
            //AlloModifyDirSlideShow = false;
        }

        /// <summary>
        /// Load song in picturebox control
        ///  1/4 = LineFeed
        ///  1/2 = Paragraph
        /// </summary>
        public void LoadSong(List<plLyric> plLyrics)
        {
            string lyric;
            string chord;            
            string lastlyric = "<>";
            bool bAdd = false;
            
            //bool isChord = false;
            int nbChords = 0;
            //plLyric.CharTypes LastType;

            _plLyrics = plLyrics;
            currentTextPos = 0;
            lyrics = "";
            for (int i = 0; i < plLyrics.Count; i++)
            {
                lyrics += plLyrics[i].Element.Item2; 
            }
                        
            List<pictureBoxControl.plLyric> pcLyrics = new List<pictureBoxControl.plLyric>();           
            
            for (int i = 0; i < plLyrics.Count; i++)
            //foreach (plLyric plL in plLyrics)
            {
                if (i == 32)
                    Console.Write("ici");
                
                plLyric plL = plLyrics[i];
                
                pictureBoxControl.plLyric pcL = new pictureBoxControl.plLyric();
                pcL.Type = (pictureBoxControl.plLyric.Types)plL.CharType;

                bAdd = true;     
                //isChord = false;
                //LastType = plL.CharType;
                                
                // Chord, lyric
                chord = plL.Element.Item1;
                lyric = plL.Element.Item2;


                if (bShowChords)
                {

                    // if bShowChords, the chords will be displayed above the lyrics, so clean chords included in lyrics
                    if (myLyricsMgmt != null && myLyricsMgmt.bHasChordsInLyrics)
                    {
                        lyric = Regex.Replace(lyric, myLyricsMgmt.RemoveChordPattern, @"");
                    }


                    if (chord != "")
                    {
                        // Add character '-' to lyrics when a chord and no lyric
                        if (lyric.Trim() == "")
                        {
                            lyric = new string('-', chord.Length) + new string('-', chord.Length);
                            nbChords++;
                        }
                        else if (lyric.Trim() == "-")
                        {
                            lyric = new string('-', chord.Length) + new string('-', plL.Element.Item1.Length);
                            nbChords++;
                        }
                        
                    }
                    else
                    {
                        // No chord in this element
                        nbChords = 0;
                    }
                }

                // Remove empty lines
                if (chord.Trim() == "" && lyric.Trim() == "")
                {
                    bAdd = false;
                }                    
                                
                if (bAdd)
                {
                    // Remove numerous linefeeds
                    if (lyric.Trim() == _InternalSepLines)
                    {
                        if (lastlyric == _InternalSepLines)
                            bAdd = false;

                        lastlyric = _InternalSepLines;
                    }
                    else
                    {
                        lastlyric = "<>";
                    }
                }              


                if (bAdd)
                {
                                                                               
                    // Linefeed before Text following an instrumental
                    if (pcL.Type == pictureBoxControl.plLyric.Types.Text && nbChords > 1 && lyric.IndexOf("-") == -1)
                    {
                        
                        pictureBoxControl.plLyric pcL2 = new pictureBoxControl.plLyric();

                        pcL2.Type = pictureBoxControl.plLyric.Types.LineFeed;
                        pcL2.Element = ("", _InternalSepLines);
                        pcL2.TicksOn = plL.TicksOn;
                        pcL2.TicksOff = plL.TicksOff;
                        pcLyrics.Add(pcL2);
                    }

                    // Start of an instrumental
                    

                    pcL.Element = (chord, lyric);
                    pcL.TicksOn = plL.TicksOn;
                    pcL.TicksOff = plL.TicksOff;

                    pcLyrics.Add(pcL);

                    // Linefeed too many chords
                    if (i < plLyrics.Count - 1)
                    {
                        plLyric p = plLyrics[i + 1];
                        if (p.CharType != plLyric.CharTypes.LineFeed && p.CharType != plLyric.CharTypes.ParagraphSep)
                        {

                            if ((myLyricsMgmt.Numerator > 2 && nbChords > myLyricsMgmt.Numerator - 1) || myLyricsMgmt.Numerator < 3 && nbChords > 3)
                            {
                                nbChords = 0;

                                pictureBoxControl.plLyric pcL2 = new pictureBoxControl.plLyric();

                                pcL2.Type = pictureBoxControl.plLyric.Types.LineFeed;
                                pcL2.Element = ("", _InternalSepLines);
                                pcL2.TicksOn = plL.TicksOn;
                                pcL2.TicksOff = plL.TicksOff;
                                pcLyrics.Add(pcL2);
                            }
                        }
                        
                    }
                }
                
            }

            // Load song
            // Force Uppercase
            pBox.bforceUppercase = _bForceUppercase;
            pBox.LoadSong(pcLyrics);
            
            //Initial position
            pBox.CurrentTextPos = -1;
           
        }

        /// <summary>
        /// Load times for the Ball animation
        /// </summary>
        /// <param name="plLyrics"></param>
        public void LoadBallsTimes(List<plLyric> plLyrics)
        {
            if (plLyrics.Count > 0)
            {
                LyricsTimes = new List<int>();

                plLyric.CharTypes plType = plLyric.CharTypes.Text;
                int plTime = 0;

                for (int i = 0; i < plLyrics.Count; i++)
                {
                    plType = plLyrics[i].CharType;
                    plTime = plLyrics[i].TicksOn;

                    if (plType == plLyric.CharTypes.Text)
                    {
                        LyricsTimes.Add(plTime);
                    }
                }
                picBalls.LoadTimes(LyricsTimes);

                // FAB 26/10/16
                picBalls.Start();
            }
        }

        /// <summary>
        /// Color the syllabe according to song position
        /// </summary>
        /// <param name="songposition"></param>
        public void ColorLyric(int songposition)
        {
            //if (busy) return;
            
            // déclencheur : timer_2
            // IMPERATIF : calculer ici la position de la syllabe, utilisée pour l'animation des balles
            // drivé par timer_2 de frmplayer            
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

        public void StopDiaporama()
        {
            pBox.Terminate();
        }

        /// <summary>
        /// Send to picturebox the parameters of chords included in lyrics if any
        /// </summary>
        private void SetDisplayChordsOptions()
        {
            /*
            if (myLyricsMgmt == null) 
                return;

            pBox.bShowChords = bShowChords;

            if (bShowChords)
            {                
                // If no chords in lyrics, add embedded chords
                if (myLyricsMgmt.bHasChordsInLyrics)
                {
                    
                    myLyricsMgmt.FillGridBeatChordsWithLyrics();
                    myLyricsMgmt.CleanGridBeatChords();
                }
                else if (!myLyricsMgmt.bHasChordsInLyrics)
                {
                    myLyricsMgmt.PopulateEmbeddedChords();

                }
            }
            */
        }

        private void ResetDisplayChordsOptions()
        {
            if (myLyricsMgmt == null)
                return;

            chkChords.Checked = bShowChords;
            pBox.bShowChords = bShowChords;

            if (bShowChords)
            {
                // Show chords                                
                if (myLyricsMgmt.bHasChordsInLyrics)
                {                    
                    // chords are included in lyrics
                    myLyricsMgmt.FillGridBeatChordsWithLyrics();
                    myLyricsMgmt.CleanGridBeatChords();
                }
                else if (!myLyricsMgmt.bHasChordsInLyrics)
                {
                    // If no chords in lyrics, add detected chords
                    myLyricsMgmt.PopulateEmbeddedChords();                    
                }
                
                // Reload lyrics
                _plLyrics = myLyricsMgmt.plLyrics;
                LoadSong(_plLyrics);

            }
            else
            {
                // Do not show chords
                // Remove characters added by chords in the lyrics
                // > reload song                
                if (!myLyricsMgmt.bHasChordsInLyrics)
                {
                    // Remove detected chords
                    myLyricsMgmt.RemoveEnbeddedChords();
                    _plLyrics = myLyricsMgmt.plLyrics;
                }
                LoadSong(_plLyrics);
            }
            
        }


        #region balls
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
            picBalls.pBallsNumber = 22;
            picBalls.Start();
        }

        public void StopTimerBalls()
        {
            picBalls.Stop();
        }

        #endregion

        #endregion methods


        #region form load close resize

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
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
            frmLyrOptions? .Dispose();
            

            base.OnClosed(e);
        }

        private void FrmLyric_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmLyricMaximized)
            {
                
                Location = Properties.Settings.Default.frmLyricLocation;
                WindowState = FormWindowState.Maximized;
                
            }
            else
            {
                Location = Properties.Settings.Default.frmLyricLocation;
                // Verify if this windows is visible in extended screens 
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmLyricSize;
            }
        }

        private void FrmLyric_FormClosing(object sender, FormClosingEventArgs e)
        {            
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmLyricLocation = RestoreBounds.Location;                       
                    Properties.Settings.Default.frmLyricMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmLyricLocation = Location;
                    Properties.Settings.Default.frmLyricSize = Size;
                    Properties.Settings.Default.frmLyricMaximized = false;

                    //Properties.Settings.Default.ChordNextColor = Color.FromArgb(255, 196, 13);
                    //Properties.Settings.Default.ChordHighlightColor = Color.FromArgb(238, 17, 17);

                }
                // Save settings
                Properties.Settings.Default.Save();
            }

            //pBox.Terminate();

            Dispose();
            
        }

        private void FrmLyric_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized )
            {
                btnFrmMax.Image = Properties.Resources.MaxNormal;
            }
            else
            {
                btnFrmMax.Image = global::Karaboss.Properties.Resources.Max;
            }
        }     

        #endregion form load close resize


        #region loadfile
        
        private string LoadLyricsFile()
        {            
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            return ReadFile(file);
        }

        private string ReadFile(string file)
        {
            string retval = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    retval = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            return retval;
        }
        #endregion loadfile

   
        #region pnlWindow

        bool bPnlVisible = false;
        DateTime startTime;        


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
        
        private void BtnFrmClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnFrmMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }

        private void BtnFrmMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnFrmOptions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmLyrOptions = new frmLyrOptions();
            frmLyrOptions.ShowDialog();
        }

        /// <summary>
        /// Export words in notepad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFrmWords_Click(object sender, EventArgs e)
        {
            #region check
            if (lyrics == null)
                return;
            #endregion

            //mnuWords.Show(btnFrmWords, 1, btnFrmWords.Height);

            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            tx = lyrics;
            tx = tx.Replace(_InternalSepParagraphs, "\r\n\r\n");
            tx = tx.Replace(_InternalSepLines, "\r\n");
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

        #region deleteme
        /*
        /// <summary>
        /// Display only words
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuDisplayWords_Click(object sender, EventArgs e)
        {
            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            tx = lyrics;           
            tx = tx.Replace(_InternalSepParagraphs, "\r\n\r\n");
            tx = tx.Replace(_InternalSepLines, "\r\n");
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

        private void mnuDisplayWordsAndChords_Click(object sender, EventArgs e)
        {
            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            // Chords are in the lyrics
            if (myLyricsMgmt.bHasChordsInLyrics)
            {
                //tx = myLyricsMgmt.GetLyricsLinesWithChords();                
                tx = myLyricsMgmt.DisplayWordsAndChords();
            }
            else
            {
                // Chords have to be guessed with a vertical search
                myLyricsMgmt.PopulateEmbeddedChords();
                
                //tx = myLyricsMgmt.GetLyricsLinesWithChords();                
                myLyricsMgmt.CleanGridBeatChords();
                tx = myLyricsMgmt.DisplayWordsAndChords();
            }

            tx = tx.Replace(_InternalSepParagraphs, "\r\n\r\n");
            tx = tx.Replace(_InternalSepLines, "\r\n");
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
        */
        #endregion deleteme

        /// <summary>
        /// Display chords when checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkChords_CheckedChanged(object sender, EventArgs e)
        {
            bShowChords = chkChords.Checked;
            btnLyricsChords.Visible = chkChords.Checked;
            
            // Save option
            Properties.Settings.Default.bShowChords = bShowChords;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Display Words with lyrics in a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLyricsChords_Click(object sender, EventArgs e)
        {
            string tx;
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            string file = path + "\\lyrics.txt";

            // Chords are in the lyrics
            if (myLyricsMgmt.bHasChordsInLyrics)
            {
                //tx = myLyricsMgmt.GetLyricsLinesWithChords();                
                tx = myLyricsMgmt.DisplayWordsAndChords();
            }
            else
            {
                // Chords have to be guessed with a vertical search
                myLyricsMgmt.PopulateEmbeddedChords();

                //tx = myLyricsMgmt.GetLyricsLinesWithChords();                
                myLyricsMgmt.CleanGridBeatChords();
                tx = myLyricsMgmt.DisplayWordsAndChords();
            }

            tx = tx.Replace(_InternalSepParagraphs, "\r\n\r\n");
            tx = tx.Replace(_InternalSepLines, "\r\n");
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


        private void PnlWindow_Resize(object sender, EventArgs e)
        {
            btnFrmClose.Top = 1;
            btnFrmMax.Top = btnFrmClose.Top + btnFrmClose.Height + 1;
            btnFrmMin.Top = btnFrmMax.Top + btnFrmMax.Height + 1;
            btnFrmOptions.Top = btnFrmMin.Top + btnFrmMin.Height + 1;
            btnFrmWords.Top = btnFrmOptions.Top + btnFrmOptions.Height + 1;
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
      
        /// <summary>
        /// Change lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeWords_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                frmPlayer frmPlayer = GetForm<frmPlayer>();
                frmPlayer.DisplayEditLyricsForm();
            }

        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }


        #endregion

       
    }


}
