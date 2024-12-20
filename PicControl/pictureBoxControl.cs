#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace PicControl
{
    public partial class pictureBoxControl : UserControl, IMessageFilter, IDisposable
    {
        /*
         * timer5_Tick de frmPlayer appelle la fonction colorLyric de frmLyrics 
         * La fenetre frmLyrics appelle la fonction ColorLyric(songposition) de picturebox control
         * Si songposition <> currenttextpos (syllabe active a changé) => redessine
         */

        

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


       

        #region classes
        public class plLyric
        {
            public enum Types
            {
                Text = 1,
                LineFeed = 2,
                Paragraph = 3,
            }
            public Types Type { get; set; }
            public (string, string) Element { get; set; }    // item1 = chord, item2 = lyric
            public int TicksOn { get; set; }
            public int TicksOff { get; set; }
        }

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


        #region properties


        #region Internal lyrics separators

        private string _InternalSepLines = "¼";
        private string _InternalSepParagraphs = "½";

        #endregion

        public ImageLayout imgLayout { get; set; }
        public Image m_CurrentImage { get; set; }
        public Rectangle m_DisplayRectangle { get; set; }
        public int m_Alpha { get; set; }

        // Display chords or not
        public bool OptionShowChords { get; set; }

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
            set { _OptionDisplay = value;
                pboxWnd.Invalidate();
            }
        }

        private bool _bTextBackGround = true;
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set { _bTextBackGround = value;
                pboxWnd.Invalidate();
            }
        }

        public bool IsBusy
        {
            get {

                if (backgroundWorkerSlideShow != null)
                    return backgroundWorkerSlideShow.IsBusy;
                else
                    return false;
            }
        }

        public List<string> LyricsWords { get; set; }  // Liste non dégrossie des syllabes
        public List<int> LyricsTimes { get; set; }     // Liste non dégrossie des temps

        private string slyrics = string.Empty;
        public string Txt {
            get
            {
                return slyrics;
            }
            set
            {
                slyrics = value;
            }
        }

        //private int _realCurrentTime;
        private int _currentPosition;
        public int CurrentTime {
            get
            { return _currentPosition; }
            set
            {
                _currentPosition = value;
            }
        }


        private int _beatDuration = 0;
        public int BeatDuration
        {
            get { return _beatDuration; }
            set { _beatDuration = value; }
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
            set { _optionbackground = value;

                switch (_optionbackground)
                {
                    case "Diaporama":
                        break;
                    case "SolidColor":
                        m_Cancel = true;
                        Terminate();
                        pboxWnd.Image = null;
                        m_CurrentImage = null;
                        pboxWnd.BackColor = txtBackColor;
                        pboxWnd.Invalidate();
                        break;
                    case "Transparent":
                        m_Cancel = true;
                        Terminate();
                        pboxWnd.Image = null;
                        m_CurrentImage = null;
                        pboxWnd.BackColor = _transparencykey;
                        pboxWnd.Invalidate();
                        break;
                    default:
                        break;
                }

            }
        }

        #region TextColor

        /// <summary>
        /// Text color
        /// </summary>
        private Color txtHighlightColor;
        public Color TxtHighlightColor
        {
            get
            { return txtHighlightColor; }
            set
            {
                txtHighlightColor = value;
                pboxWnd.Invalidate();
            }
        }

        /// <summary>
        /// Text to sing color
        /// </summary>
        private Color txtNextColor;
        public Color TxtNextColor {
            get
            { return txtNextColor; }
            set
            {
                txtNextColor = value;
                pboxWnd.Invalidate();
            }
        }

        #region chords

        /// <summary>
        /// Text to sing color
        /// </summary>
        private Color _chordNextColor;
        public Color ChordNextColor
        {
            get
            { return _chordNextColor; }
            set
            {
                _chordNextColor = value;
                pboxWnd.Invalidate();
            }
        }

        /// <summary>
        /// Chord Highligt color
        /// </summary>
        private Color _chordHighlightColor;
        public Color ChordHighlightColor
        {
            get
            { return _chordHighlightColor; }
            set
            {
                _chordHighlightColor = value;
                pboxWnd.Invalidate();
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
                    pboxWnd?.Invalidate();
                }
            }
         }

        #endregion chords

        /// <summary>
        /// Text sung color
        /// </summary>
        private Color txtBeforeColor;
        public Color TxtBeforeColor {
            get
            { return txtBeforeColor; }
            set
            {
                txtBeforeColor = value;
                pboxWnd.Invalidate();
            }
        }

        private bool _bColorContour = true;
        public bool bColorContour
        {
            get
            { return _bColorContour; }
            set
            {
                _bColorContour = value;
                pboxWnd.Invalidate();
            }
        }

        // Text contour
        private Color txtContourColor;
        public Color TxtContourColor {
            get
            { return txtContourColor; }
            set
            {
                txtContourColor = value;
                pboxWnd.Invalidate();
            }
        }

        #endregion Textcolor       

        #region Text others


        private bool _bdemo = false;
        public bool bDemo
        {
            get { return _bdemo; }
            set { _bdemo = value; }
        }

        // Lyrics : converts characters to uppercase
        private bool _bforceuppercase;
        public bool bforceUppercase {
            get { return _bforceuppercase; }
            set { _bforceuppercase = value;
                if (_bdemo)
                    LoadDemoText();
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
                    pboxWnd.Invalidate();
                }
                catch (Exception e)
                {
                    Console.Write("Error: " + e.Message);
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
                    pboxWnd.Invalidate();
                }
                catch (Exception e)
                {
                    Console.Write("Error: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Background color
        /// </summary>
        private Color txtBackColor;
        public Color TxtBackColor {
            get
            { return txtBackColor; }
            set
            {
                txtBackColor = value;
                if (_optionbackground == "SolidColor")
                {
                    pboxWnd.BackColor = txtBackColor;
                    pboxWnd.Invalidate();
                }
            }
        }

        /// <summary>
        /// Number of lines to display
        /// </summary>
        private int _txtNbLines;
        public int TxtNbLines {
            get
            { return _txtNbLines; }
            set
            {
                _txtNbLines = value;
                ajustTextAgain();
                pboxWnd.Invalidate();
            }
        }

        // Show a blank line between paragraphs
        private bool _bshowparagraphs = true;
        public bool bShowParagraphs
        {
            get { return _bshowparagraphs; }
            set { _bshowparagraphs = value; }
        }

        #endregion

        /// <summary>
        /// SlideShow directory
        /// </summary>
        private string dirSlideShow;
        public string DirSlideShow {
            get
            { return dirSlideShow; }
            set
            {
                dirSlideShow = value;
                pboxWnd.Invalidate();
            }
        }
        
        /// <summary>
        /// SlideShow frequency
        /// </summary>
        private int freqSlideShow;
        public int FreqDirSlideShow
        {
            get
            { return freqSlideShow; }
            set
            { freqSlideShow = value;
            if (freqSlideShow > 0)
                {                    
                    PAUSE_TIME = 1000 * freqSlideShow;
                }
            }
        }

        /// <summary>
        /// Size mode of picturebox
        /// </summary>
        private PictureBoxSizeMode _sizemode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizemode; }
            set { _sizemode = value;
            pboxWnd.SizeMode = _sizemode;
            }
        }



        #endregion properties
    

        #region SlideShow       

        private Random random;
        private string[] bgFiles;
        private string DefaultDirSlideShow;       
        
        private List<string> m_ImageFilePaths;
        private MemoryStream m_ImageStream = null;        

        //private AutoResetEvent m_FinishEvent = new AutoResetEvent(false);
        private ManualResetEvent m_FinishEvent = new ManualResetEvent(false);

        private bool m_Cancel = false;
        private bool m_Restart = false;       

        //private int m_step = 51;          // 0 à 255 par step de 3, 5, 15, 17, 51
        //private bool m_wait = false;
        
        delegate void UpdateTimerEnableCallback(bool enabled);
        #endregion SlideShow


        #region private

        private bool disposed = false;

        private BackgroundWorker backgroundWorkerSlideShow;

        private string strCurrentImage; // current image to insure that random will provide a different one
        private int rndIter = 0;


        private int vOffset = 0;
        private int _lineHeight = 0;

        private Font _karaokeFont;
        private int _linesHeight = 0;
        private int _nbLyricsLines = 0;

        private bool bEndOfLine = false;
        private bool bHighLight = false;
        private int nextStartOfLineTime = 0;
        private int _lastLinePosition = 0;
        private int TimeToNextLineDuration = 0;
        private int PAUSE_TIME;

        private List<string> lstLyricsLines;    // Liste de lignes
        
        private int currentLine = 0;
        private string lineMax; // Ligne longueur max
        
 
        private List<syllabe> syllabes;

        private Font m_font;
        private float emSize; // Size of the font
        private StringFormat sf;

        private int percent = 0;
        private float steps = 10;
        //private float step = 0;      

        private List<RectangleF> rRect;
        private List<RectangleF> rNextRect;
        private List<RectangleF>[] rListNextRect;

        #endregion private

        // Constructor
        public pictureBoxControl()
        {
            InitializeComponent();

            // Dipslay chords or not
            //OptionShowChords = false;
            OptionShowChords = true;

            _karaokeFont = new Font("Arial", this.Font.Size);
            _chordFont = new Font("Comic Sans MS", this._karaokeFont.Size);

            #region Move form without title bar
            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.pboxWnd);
            #endregion
            
            m_ImageFilePaths = new List<string>();
            m_Alpha = 255;
            imgLayout = ImageLayout.Stretch;
            
            
            this.SetStyle(
                  System.Windows.Forms.ControlStyles.UserPaint |
                  System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                  System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                  true);       
            
            SetDefaultValues();
        }


        #region methods

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

        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void SetBackground(string dirImages)
        {
            try
            {
                UpdateTimerEnable(false);

                m_Cancel = true;
                m_Restart = true;

                m_CurrentImage = null;
                strCurrentImage = string.Empty;
                rndIter = 0;

                pboxWnd.Image = null;
                pboxWnd.Invalidate();
                m_ImageFilePaths.Clear();

                if (dirImages == null)
                {
                    pboxWnd.BackColor = Color.Black;
                }
                else if (Directory.Exists(dirImages))
                {
                    int C = 0;

                    if (_optionbackground == "Diaporama")
                    {
                        LoadImageList(dirImages);
                        C = m_ImageFilePaths.Count;
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
                            pboxWnd.Image = Image.FromFile(m_ImageFilePaths[0]);
                            break;
                        default:
                            // Slideshow => backgroundworker

                            //m_Cancel = true;
                            m_Cancel = false;

                            // Initialize backgroundworker
                            InitBackGroundWorker();
                            random = new Random();
                            Start();
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }

        }

        private void InitBackGroundWorker()
        {
            backgroundWorkerSlideShow = new System.ComponentModel.BackgroundWorker();
            backgroundWorkerSlideShow.WorkerSupportsCancellation = true;
            backgroundWorkerSlideShow.WorkerReportsProgress = true;            
            backgroundWorkerSlideShow.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSlideShow_DoWork);
            backgroundWorkerSlideShow.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerSlideShow_RunWorkerCompleted);
        }

        /// <summary>
        /// Color the syllabe according to song position
        /// </summary>
        /// <param name="songposition"></param>
        public void ColorLyric(int songposition)
        {                        
            _currentPosition = songposition;           
            SetOffset();
        }

        /// <summary>
        /// Reset display at begining
        /// </summary>
        public void ResetTop()
        {            
            bEndOfLine = false;

            vOffset = 0;
            nextStartOfLineTime = 0;

            _currentPosition = 0;
            _currentTextPos = -1;
            pboxWnd.Invalidate();
        }
        
        /// <summary>
        /// Load text of song
        /// </summary>
        /// <param name="toto"></param>
        public void LoadSong(List<plLyric> plLyrics, bool bDemoMode = false)
        {
            string lyrics = string.Empty;          

            if (plLyrics.Count > 0)
            {                
                
                lyrics = string.Empty;
                pboxWnd.Invalidate();

                for (int i = 0; i < plLyrics.Count; i++)
                {
                    // Force uppercase
                    if (_bforceuppercase)
                        plLyrics[i].Element = (plLyrics[i].Element.Item1, plLyrics[i].Element.Item2.ToUpper());

                    lyrics += plLyrics[i].Element.Item2;
                }

                this.Txt = lyrics;
                // store lines in a specific list
                StoreLyricsLines(lyrics);

                // ajust font size
                lineMax = GetMaxLength();
                AjustText(lineMax);

                //TestCheckTimes(plLyrics);

                // Store syllabes
                StoreLyricsSyllabes(plLyrics);


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

                // Create rectangles
                createListRectangles(0);       
                if (syllabes != null && syllabes.Count > 0)
                    createListNextRectangles(syllabes[0].last + 1);
                            
            } 
                     
        }

        #endregion methods


        #region tests

        private void TestCheckTimes(List<plLyric> plLyrics)
        {
            int lastTime = -1;
            int t = -1;
            for (int i = 0; i < plLyrics.Count; i++)
            {
                t = plLyrics[i].TicksOn;
                if (t < lastTime)
                {
                    MessageBox.Show("Error: times not in order", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                lastTime = t;
            }
        }
        #endregion tests


        #region SlideShow functions

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

        #endregion SlideShow functions


        #region demo, wait

        /// <summary>
        /// Count Down: decreasing numbers to wait for next song to start 
        /// </summary>
        /// <param name="sec">Count down max </param>
        public void LoadWaitSong(int sec)
        {           
            string tx = string.Empty;

            // 10|9|8|7|6|5|4|3|2|1|0|                     
            for (int i = sec; i >= 0; i--)
            {
                tx += i.ToString() + _InternalSepLines;
            }
            

            List<plLyric> plLyrics = StoreDemoText(tx);
            
            _txtNbLines = 1;
            dirSlideShow = null;
            SetBackground(null);

            LoadSong(plLyrics);

            // Initial position
            _currentTextPos = -1;
            vOffset = 0;
            nextStartOfLineTime = 0;

            //m_wait = true;
        }

        public void endDemoText()
        {            
            syllabes = null;
        }
        
        /// <summary>
        /// Store demo text
        /// '|' are carriage return
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        private List<plLyric> StoreDemoText(string tx, int ticks = 0)
        {
            // replace spaces and carriage return 
            // tata toto<cr>titi tutu devient
            // tata toto titi<cr>' ' tutu devient
            // tata]toto<cr>[,titi],tutu

            // protect spaces, replaced by ']' + space
            string m_ProtectSpace = "¾";
            string S = tx.Replace(" ", m_ProtectSpace + " ");

            // _InternalSepLines = replaced by _InternalSepLines + space
            S = S.Replace(_InternalSepLines, _InternalSepLines + " ");

            // Split syllabes by spaces
            string[] strLyricSyllabes = S.Split(new Char[] { ' ' });

            LyricsWords = new List<string>();
            LyricsTimes = new List<int>();

            // load lists syllabes and times
            List<plLyric> plLyrics = new List<plLyric>();

            string sx = string.Empty;
            (string, string) plElement = (string.Empty, string.Empty);
            int plTime = 0;
            plLyric.Types plType = plLyric.Types.Text;

            for (int i = 0; i < strLyricSyllabes.Length; i++)
            {
                sx = strLyricSyllabes[i];
                sx = sx.Replace(m_ProtectSpace, " ");    // retrieve spaces

                plElement = ("", sx);
                plTime = ticks + (i + 1) * 10;        // time each 10 ticks

                if (sx.Length > 1 && sx.Substring(sx.Length - 1, 1) == _InternalSepLines)
                {
                    // String ended by _InternalSepLines
                    string reste = sx.Substring(0, sx.Length - 1);
                    
                    plType = plLyric.Types.Text;
                    plElement = ("", reste);
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });

                    plType =  plLyric.Types.LineFeed;
                    plElement = ("", _InternalSepLines);
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });

                }
                else
                {
                    if (sx == _InternalSepLines)
                        plType = plLyric.Types.LineFeed;
                    else
                        plType = plLyric.Types.Text;
                    
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });
                }                
            }
            return plLyrics;
        }

        /// <summary>
        /// Set default values for demonstration purpose
        /// 1/4 = LineFeed
        /// 1/2 = Paragraph
        /// </summary>
        private void SetDefaultValues()
        {           
            txtBackColor = Color.Black;     
            txtContourColor = Color.Black;
            txtNextColor = Color.White;
            txtBeforeColor = Color.FromArgb(153, 180, 51);      // modern ui light green
            txtHighlightColor = Color.FromArgb(238, 17, 17);    // modern ui dark Red;

            _chordNextColor = Color.FromArgb(255, 196, 13);         // modern ui Orange
            _chordHighlightColor = Color.FromArgb(238, 17, 17);    // modern ui dark Red
            //_chordFont = new Font(Comic)
            
            _txtNbLines = 3;         

            OptionBackground = "SolidColor";                                    

            // Default dir for slide show
            DefaultDirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

            freqSlideShow = 5 * 1000;
            m_Cancel = false;
            
            emSize = 4;            
            m_font = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);

            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

                        
            pboxWnd.Font = new Font(Name = _karaokeFont.Name, emSize);
            pboxWnd.SizeMode = PictureBoxSizeMode.Zoom;            

            // Initial conditions
            _currentPosition = 30;
            currentLine = 1;
            _currentTextPos = 2;           

            pboxWnd.Invalidate();
        }

        public void LoadDemoText()
        {
            // Default text
            string tx = "Lorem ipsum dolor sit amet," + _InternalSepLines;
            tx += "consectetur adipisicing elit," + _InternalSepLines;
            tx += "sed do eiusmod tempor incididunt" + _InternalSepLines;
            tx += "ut labore et dolore magna aliqua." + _InternalSepLines;
            tx += "Ut enim ad minim veniam," + _InternalSepLines;
            tx += "quis nostrud exercitation ullamco" + _InternalSepLines;
            tx += "laboris nisi ut aliquip" + _InternalSepLines;
            tx += "ex ea commodo consequat." + _InternalSepLines;
            tx += "Duis aute irure dolor in reprehenderit" + _InternalSepLines;
            tx += "in voluptate velit esse cillum dolore" + _InternalSepLines;
            tx += "eu fugiat nulla pariatur.";

            if (_bforceuppercase)
                tx = tx.ToUpper();

            List<plLyric> plLyrics = StoreDemoText(tx);

            LoadSong(plLyrics, true);

        }

        /// <summary>
        /// Display a text
        /// </summary>
        /// <param name="tx"></param>
        public void DisplayText(string tx, int ticks = 0)
        {            
            List<plLyric> plLyrics = StoreDemoText(tx, ticks);
            LoadSong(plLyrics);

            // Initial position
            _currentPosition = 0;
            currentLine = 1;
            _currentTextPos = 0;

            pboxWnd.Invalidate();
        }

        #endregion demo wait


        #region Text

        /// <summary>
        /// Store lyrics lines in a list called lstLyricsLines
        /// </summary>
        /// <param name="ly"></param>
        private void StoreLyricsLines(string ly)
        {
            lstLyricsLines = new List<string>();

            string tx = string.Empty;

            /*
            * A back slash "\" character marks the end of a line of lyrics, as displayed by a Karaoke viewer/player program.
            *
            * A forward slash "/" character marks the end of a "paragraph" of lyrics. 
            * Some Karaoke viewer / player programs interpret this to mean that the screen should be refreshed starting with the next line of lyrics at the top.
            *
            * Dash characters at the end of syllables are removed by the Karaoke viewer/player program, and the syllables are joined together. 
            */
            
            string lyr = string.Empty;
                             

            lyr = ly;
            if (_bshowparagraphs) 
            {
                // Replace new paragraph by newline + new paragraph + new line to allow split by linefeed
                lyr = lyr.Replace(_InternalSepParagraphs, _InternalSepLines + _InternalSepParagraphs + _InternalSepLines);

            }
            else
            {
                lyr = ly.Replace(_InternalSepParagraphs, _InternalSepLines); 
            }

            // TO BE MODIFIED
            char ChrSepLines = Convert.ToChar(_InternalSepLines);
            string[] strLyricsLines = lyr.Split(new Char[] { ChrSepLines }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < strLyricsLines.Length; i++)
            {
                // FAB CHORD
                // DO NOT TRIM BECAUSE OF CHORDS whith no text
                //tx = strLyricsLines[i].Trim();                
                tx = strLyricsLines[i];

                if (_bshowparagraphs && tx == _InternalSepParagraphs)
                {
                    // new paragraph = empty line (space)
                    lstLyricsLines.Add(" ");
                }
                else if (tx != "")
                {                                        
                    lstLyricsLines.Add(tx);
                }
            }

            // Number of lines (offset calculation)
            _nbLyricsLines = lstLyricsLines.Count - 1;

        }

        private int GetMaxSyllabesInLine(int ind, int line, List<plLyric> plLyrics)
        {
            int max = 0;
            // Recherche le nombre max de syllabes dans la ligne            
            int lastpos = 0;
            string tx;
            int pos = 0;
            string strline = lstLyricsLines[line];
            string strwrkline = strline;

            if (strwrkline.Trim().Length == 0)
                return 0;

            do
            {
                if (ind < plLyrics.Count)
                {                    
                    tx = plLyrics[ind].Element.Item2;
                    //tx = tx.Trim();

                    if (plLyrics[ind].Type == plLyric.Types.LineFeed || plLyrics[ind].Type == plLyric.Types.Paragraph)
                    {
                        break;
                    }
                    else if (tx != "")
                    {
                        // Si toutes les syllabes sont identiques dans la ligne (ex la la la la)
                        // , c'est faux .... lastpos reste à zéro
                        pos = strwrkline.IndexOf(tx, lastpos);

                        if (pos != -1)
                        {
                            max++; // Nombre de syllabes
                            lastpos = pos;
                            ind++;

                            // Replace used letters by a "#"
                            string rep = new string('#', tx.Length);
                            var regex = new Regex(Regex.Escape(tx));
                            strwrkline = regex.Replace(strwrkline, rep, 1);

                        }
                    }
                    else
                    {
                        ind++;
                    }
                }
            } while (pos != -1 && ind < plLyrics.Count);


            return max;
        }

        /// <summary>
        /// Store syllabes in a list, each item being a class called syllabe
        /// </summary>
        /// <param name="plLyrics"></param>
        private void StoreLyricsSyllabes(List<plLyric> plLyrics)
        {
            syllabes = new List<syllabe>();

            string chordName = string.Empty;
            string tx;
            int itime = 0;
            int idx = -1;
            int firstitem = 0;
            int indexSyllabe = 0;
            int offset = 0;
            int max;
            int pos;
            int iline;
            int lastpos;
            int line = 0;            

            try
            {
                
                if (lstLyricsLines.Count == 0)
                    return;

                // Loop line by line           
                do
                {
                    string strline = lstLyricsLines[line];
                    string strwrkline = strline;
                    pos = 0;
                    max = 0;
                    iline = -1;

                    // ==============================
                    // Paragraph = empty line
                    // ==============================
                    if (plLyrics[indexSyllabe].Type == plLyric.Types.Paragraph)
                    {
                        #region add paragraph                        

                        idx++;
                        // Paragraphe = ligne vide
                        // Crée un nouvel item syllabe
                        syllabe syl = new syllabe();

                        syl.chord = "";
                        syl.line = line;                // line number of syllabe
                        syl.pos = idx;                      // position dans la chanson
                        syl.posline = 0;                    // position dans la ligne
                        syl.text = " ";
                        syl.SylCount = 1;                   // number of syllabes in this line
                        syl.last = idx;                     // position of last syllabe                        
                        syl.time = plLyrics[indexSyllabe].TicksOn; // time of syllabe
                        syl.offset = offset;
                        syllabes.Add(syl);

                        indexSyllabe++;
                        
                        line++;
                        #endregion add paragraph

                    }
                    else if (plLyrics[indexSyllabe].Type == plLyric.Types.LineFeed)
                    {
                        #region linefeed
                        // ==============================
                        // LINEFEED
                        // ==============================
                        indexSyllabe++;                        

                        #endregion linefeed
                    }
                    else
                    {
                        #region Text
                        // ==============================
                        // Normal line
                        // ==============================                                                                                              

                        // Search for number of syllabes in this line                    
                        max = 0;
                        if (plLyrics[indexSyllabe].Element.Item2.Trim() != "")
                        {
                            max = GetMaxSyllabesInLine(indexSyllabe, line, plLyrics);

                            if (max == 0)
                            {
                                MessageBox.Show("Error: Syllabe not found", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);    
                                return;
                            }
                        }

                        //nbLineFeeds = 0;
                        lastpos = 0;
                        // Offset de la ligne
                        offset = 0;
                        pos = 0;
                        strwrkline = strline;

                        // Loop for each syllabe in the same line                        
                        do
                        {
                            if (indexSyllabe < plLyrics.Count)
                            {
                                chordName = plLyrics[indexSyllabe].Element.Item1;
                                tx = plLyrics[indexSyllabe].Element.Item2;
                                string trimtx = tx.Trim();
                                itime = plLyrics[indexSyllabe].TicksOn;

                               
                                // ====================
                                // NORMAL TEXT
                                // ====================
                                pos = strwrkline.IndexOf(trimtx, lastpos);

                                if (pos != -1)
                                {
                                    offset = 0; // Offset de la ligne
                                    lastpos = pos;

                                    // Crée un nouvel item syllabe
                                    syllabe syl = new syllabe();

                                    idx++;
                                    iline++;

                                    if (iline == 0)
                                        firstitem = idx;

                                    if (max == 0)
                                        max = 1;


                                    syl.chord = chordName;
                                    syl.line = line;                    // line number of syllabe
                                    syl.posline = iline;                // position dans la ligne
                                    syl.pos = idx;                      // position dans la chanson
                                    syl.text = tx;                      // text of syllabe
                                    syl.time = itime;                   // time of syllabe
                                    syl.SylCount = max;                 // number of syllabes in this line
                                    syl.last = firstitem + max - 1;     // position of last syllabe
                                    syl.offset = offset;

                                    syllabes.Add(syl);

                                    if (syl.line == _nbLyricsLines && syl.posline == 0)
                                    {
                                        _lastLinePosition = syl.time;
                                    }

                                    // incrémente index
                                    indexSyllabe++;

                                    #region exit if CR
                                    // if next syllabe is a linefeed => next line
                                    if (indexSyllabe < plLyrics.Count && plLyrics[indexSyllabe].Type != plLyric.Types.Text)
                                    {
                                        line++;
                                        break;
                                    }

                                    /*
                                    if (indexSyllabe < plLyrics.Count && plLyrics[indexSyllabe].Type == plLyric.Types.LineFeed)
                                    {                                        
                                        line++;
                                        break;
                                    }

                                    if (indexSyllabe < plLyrics.Count && plLyrics[indexSyllabe].Type == plLyric.Types.Paragraph)
                                    {                                        
                                        line++;
                                        break;
                                    }
                                    */
                                    #endregion exit if CR

                                    // Replace used letters by a "#"
                                    string rep = new string('#', trimtx.Length);
                                    var regex = new Regex(Regex.Escape(trimtx));
                                    strwrkline = regex.Replace(strwrkline, rep, 1);
                                }
                                
                            }
                        } while (pos != -1 && indexSyllabe < plLyrics.Count);

                        #endregion Text
                    }

                } while (line <= lstLyricsLines.Count - 1 && indexSyllabe < plLyrics.Count);
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Ajuste la taille de la fonte en fonction de la taille de pictureBox1
        /// </summary>
        /// <param name="S"></param>
        private void AjustText(string S)
        {
            if (S != "" && pboxWnd != null)
            {
                Graphics g = pboxWnd.CreateGraphics();
                float femsize;

                long inisize = (long)pboxWnd.Font.Size;                
                femsize = g.DpiX * inisize / 72;

                float textSize = MeasureString(S, femsize);
                long comp = (long)(0.95*pboxWnd.ClientSize.Width);                

                // Texte trop large
                if (textSize > comp)
                {
                    do
                    {
                        inisize = inisize - 1;
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
                        inisize = inisize + 1;                        
                        femsize = g.DpiX * inisize / 72;                        
                        textSize = MeasureString(S, femsize);
                    } while (textSize < comp);
                }


                // ------------------------------
                // Ajustement in height 
                // ------------------------------

                float textHeight = MeasureStringHeight(S, inisize);
                float totaltextHeight;
                totaltextHeight = _txtNbLines * (textHeight + 10);

                if (_bShowChords)
                {
                    // FAB CHORD
                    totaltextHeight = (int)2.5*totaltextHeight;
                }

                long compHeight = (long)(0.95*pboxWnd.ClientSize.Height);
                
                if (totaltextHeight > compHeight)
                {
                    do
                    {
                        inisize = inisize - 1;
                        if (inisize > 0)
                        {                            
                            femsize = g.DpiY * inisize / 72;                            
                            textHeight = MeasureStringHeight(S, femsize);
                            
                            totaltextHeight = _txtNbLines * (textHeight + 10);
                            if (_bShowChords)
                            {
                                // FAB CHROD
                                totaltextHeight = (int)2.5*totaltextHeight;
                            }

                        }
                    } while (totaltextHeight > compHeight && inisize > 0);
                }


                if (inisize > 0)
                {
                    emSize = g.DpiY * inisize / 72;                    
                    m_font = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);                    
                    pboxWnd.Font = new Font(Name = _karaokeFont.Name, emSize);

                    // Vertical distance between lines
                    _lineHeight = (int)emSize + 10;
                    // Height of the full song
                    _linesHeight = _nbLyricsLines * _lineHeight;                                        

                }
                g.Dispose();
            }
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
                using (Graphics g = pboxWnd.CreateGraphics())
                {
                    string tx = string.Empty;
                    
                    rRect = new List<RectangleF>();

                    int line = syllabes[pos].line;
                    string strLine = lstLyricsLines[line];
                    float Offset = getOffset(strLine, emSize);           // Offset de la ligne (centré)

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
                            rect.X = Offset - 1;
                        }
                        else
                        {
                            rect.X = rRect[idx - 1].X + rRect[idx - 1].Width - 1;
                        }
                        rRect.Add(rect);
                    }
                    g.Dispose();
                }
            }
        }

       
        /// <summary>
        /// Create rectangles for next lines
        /// </summary>
        /// <param name="pos"></param>
        private void createListNextRectangles(int pos)
        {
            if (pos < 0)
                pos = 0;

            if (pos < syllabes.Count)
            {
                using (Graphics g = pboxWnd.CreateGraphics())
                {
                    string strLine;
                    string tx = string.Empty;
                    float Offset;
                    rListNextRect = new List<RectangleF>[_txtNbLines];
                    int line = syllabes[pos].line;
                    
                    // si la ligne précédante est " " car saut de paragraphe, il faudrait ajouter une liste de rectangle fictogve à l'indice 0                    
                    int start = 0;
                    int end = _txtNbLines;


                    rListNextRect = new List<RectangleF>[end];
                    

                    for (int k = start; k < end; k++)
                    {
                        strLine = lstLyricsLines[line];
                        
                        Offset = getOffset(strLine, emSize);           // Offset de la ligne (centré)
                        
                        rNextRect = new List<RectangleF>();

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

                            rect.Width = sz.Width + 1;
                            rect.Height = sz.Height + 1;

                            if (idx == 0)
                            {
                                rect.X = Offset - 1;
                            }
                            else
                            {
                                rect.X = rNextRect[idx - 1].X + rNextRect[idx - 1].Width - 1;
                            }
                            rNextRect.Add(rect);                            
                        }

                        rListNextRect[k] = rNextRect;

                        line++;

                        if (line > lstLyricsLines.Count - 1)
                            break;

                        pos = syllabes[pos].last + 1;
                        if (pos >= syllabes.Count)
                            break;                        
                    }                    

                    g.Dispose();   
                }                

            }
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
        /// Create rectangles when line changes
        /// </summary>
        /// <param name="res"></param>
        private void synchronize(int syllabeposition)
        {
            if (syllabes == null)
                return;

            int x0 = 0;
            // Si retour arriere ou avance
            if (syllabeposition < 0)
                syllabeposition = 0;

            if (syllabeposition >= 0 && syllabeposition < syllabes.Count)
            {
                if (syllabes[syllabeposition].line != currentLine)
                {
                    currentLine = syllabes[syllabeposition].line;

                    /*
                    Console.WriteLine("************* line : " + currentLine );

                    if (currentLine == 2) 
                    {
                        Console.WriteLine("ici");
                    }
                    */

                    // Beginning of line
                    x0 = syllabeposition - syllabes[syllabeposition].posline;
                    // Create list of rectangles for current line
                    createListRectangles(x0);

                    // Create list of rectangles for next line
                    createListNextRectangles(syllabes[syllabeposition].last + 1);
                }
            }
        }

        #endregion Text


        #region measures

        /// <summary>
        /// Get offset to center text
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        private int getOffset(string tx, float femsize)
        {
            float ret = 0;
            float L = MeasureString(tx, femsize);
            float W = pboxWnd.ClientSize.Width;

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
            long H = (long)pboxWnd.ClientSize.Height;



            switch (_OptionDisplay)
            {
                case OptionsDisplay.Top:
                    if (_txtNbLines == 1)
                        ret = 10;
                    else
                        ret = _lineHeight;
                    break;

                case OptionsDisplay.Center:
                    if (_txtNbLines == 1)
                    {
                        ret = (H - ((_txtNbLines) * (h + 10))) / 2;
                    }
                    else
                    {
                        if (_bShowChords)
                            ret = (H - ((2 * _txtNbLines - 1) * (h + 10))) / 2;
                        else
                            ret = (H - ((_txtNbLines - 1) * (h + 10))) / 2;
                    }
                    break;

                case OptionsDisplay.Bottom:
                    if (_bShowChords)
                        ret = (H - (int)(2.5 * _txtNbLines) * _lineHeight) - 10;
                    else
                        ret = (H - _txtNbLines * _lineHeight) - 10;

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
        private float MeasureString(string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pboxWnd.CreateGraphics())
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
        private float MeasureStringHeight(string line, float femSize)
        {
            float ret = 0;

            if (line != "")
            {
                using (Graphics g = pboxWnd.CreateGraphics())
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

            for (int i = 0; i < lstLyricsLines.Count; i++)
            {
                if (lstLyricsLines[i].Length > max)
                {
                    max = lstLyricsLines[i].Length;
                    tx = lstLyricsLines[i];
                }
            }


            return tx;
        }



        #endregion measures


        #region draw

        /// <summary>
        /// Draw current line, syllabe by syllabe
        /// already sung: TxtBeforeColor
        /// Currently sung: TxtHighlightColor
        /// Not yet sung: txtNextColor
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="syl"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="e"></param>
        private void drawSyllabe(Color clr, syllabe syl, int x0, int y0, int W, int H, PaintEventArgs e)
        {
            var path = new GraphicsPath();
            string tx = syl.text;
                        
            try
            {
                //float x0 = rRect[syl.posline].X;

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
                path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, y0), sf);
                e.Graphics.FillPath(new SolidBrush(clr), path);
                
                if (_bColorContour)
                    e.Graphics.DrawPath(new Pen(txtContourColor), path); 

                path.Dispose();
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
                //path.AddString(tx, m_font.FontFamily, (int)m_font.Style, 3 * emSize / 4, new Point((int)x0, y0), sf);
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
        /// Draw syllabes on next lines
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="syl"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <param name="e"></param>
        private void drawSyllabeNextLines(Color clr, syllabe syl, int x0, int y0, int W, int H, PaintEventArgs e)
        {
            var path = new GraphicsPath();
            string tx = syl.text;            

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
                
                path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, y0), sf);
                e.Graphics.FillPath(new SolidBrush(clr), path);
                
                if (_bColorContour)
                    e.Graphics.DrawPath(new Pen(txtContourColor), path);

                path.Dispose();
                
                #endregion
            }
            catch (Exception ed)
            {
                Console.Write("Error: " + ed.Message);
            }
        }

        /// <summary>
        /// Draw chord on next lines
        /// </summary>
        /// <param name="clr"></param>
        /// <param name="syl"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="e"></param>
        private void drawChordNextLines(Color clr, syllabe syl, int x0, int y0, PaintEventArgs e)
        {
            var path = new GraphicsPath();
            string tx = syl.chord;

            try
            {
                #region Draw text of chord
                //path.AddString(tx, m_font.FontFamily, (int)m_font.Style, 3*emSize/4, new Point((int)x0, y0), sf);
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
        /// Pas utilisé
        /// </summary>
        /// <param name="singclr"></param>
        /// <param name="nextclr"></param>
        /// <param name="syl"></param>
        /// <param name="y0"></param>
        /// <param name="e"></param>
        private void drawFilledSyllabe(Color singclr, Color nextclr ,syllabe syl, int y0, PaintEventArgs e)
        {
            var path = new GraphicsPath();
            string tx = syl.text;

            try
            {
                float x0 = rRect[syl.posline].X;
               
                
                // Not filled = nextclr
                path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, y0), sf);
                e.Graphics.FillPath(new SolidBrush(nextclr), path);

                // Filled
                Region rb = new Region(path);
                RectangleF rectb = rb.GetBounds(e.Graphics);
                float W = rectb.Width * (percent / steps);

                RectangleF intersectRectb = new RectangleF(rectb.X, rectb.Y, W, rectb.Height);
                rb.Intersect(intersectRectb);
                e.Graphics.FillRegion(new SolidBrush(singclr), rb);

                // Entourage
                e.Graphics.DrawPath(new Pen(txtContourColor), path);

                rb.Dispose();        
                
                path.Dispose();

            }
            catch (Exception edf)
            {
                Console.Write("Error: " + edf.Message);
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
            string tx = string.Empty;
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
                                drawChord(_chordNextColor, syllab, (int)x1, y0, e);
                            
                            drawSyllabe(txtBeforeColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                            // déjà chanté
                        }
                        else
                        {
                            drawSyllabe(txtBeforeColor, syllab, (int)x1, y0, W, H, e);                                            // déjà chanté
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
                                    drawChord(_chordHighlightColor, syllab, (int)x1, y0, e);
                                
                                drawSyllabe(txtHighlightColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                       // surbrillance
                            }
                            else
                            {
                                drawSyllabe(txtHighlightColor, syllab, (int)x1, y0, W, H, e);                                         // surbrillance     
                            }
                        }
                        else
                        {
                            if (_bShowChords)
                            {
                                if (syllab.chord != "")
                                    drawChord(_chordNextColor, syllab, (int)x1, y0, e);
                                
                                drawSyllabe(txtNextColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);
                            }
                            else
                            {
                                drawSyllabe(txtNextColor, syllab, (int)x1, y0, W, H, e);
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
                                drawChord(_chordNextColor, syllab, (int)x1, (int)y0, e);
                            
                            drawSyllabe(txtNextColor, syllab, (int)x1, y0 + 2 * offset / 3, W, H, e);                           // pas encore chanté
                        }
                        else
                        {
                            drawSyllabe(txtNextColor, syllab, (int)x1, y0, W, H, e);                                      // pas encore chanté
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
        /// Draw next full lines with color txtNextColor
        /// </summary>
        /// <param name="e"></param>
        private void DrawNextLines(int y0, PaintEventArgs e)
        {            
            int x0 = 0;
            int i;
            int offset = _lineHeight;

            int W;
            int H;

            float x1;
            float y1;

            int ChordOffset = offset;  // To manage when offset = 0 

            if (_txtNbLines == 1)
                offset = 0;                      


            // Draw sentence                           
            #region draw lyrics

            if (syllabes == null | _currentTextPos >= syllabes.Count)
                return;
            
            if (_currentTextPos >= 0)
                x0 = _currentTextPos - syllabes[_currentTextPos].posline;
            

            for (int k = 0; k < _txtNbLines; k++)
            {              
                int line = currentLine + k + 1;
                // k = 0;
                // Si currentline = 0 => line = 1
                // mais si il y a un séparateur paragraphe, 
                // on parcourt la boucle for (i = x0; i < syllabes.Count; i++) sans rien faire 
                // du coup, k passe à 1 et on utilise les rectangles de la ligne suivante


                if (_txtNbLines == 1)
                {
                    if (line > currentLine + 1) break;
                }
                else
                {
                    if (line > currentLine + _txtNbLines - 1) break;
                }

                for (i = x0; i < syllabes.Count; i++)
                {
                    if (syllabes[i].line == line)
                    {                        
                        int pos = syllabes[i].posline;
                        if (pos < rListNextRect[k].Count)
                        {
                            // Rectangle for next lines
                            x1 = rListNextRect[k][pos].X;
                            W = (int)rListNextRect[k][pos].Width;
                            H = (int)rListNextRect[k][pos].Height;

                            if (_bShowChords)
                            {
                                y1 = y0 + (k + 1) * offset + (k + 1) * offset;
                                
                                // Draw chord above
                                if (syllabes[i].chord != "")
                                    drawChordNextLines(_chordNextColor, syllabes[i], (int)x1, (int)y1, e);

                                // Draw syllabe below at 2*ChordOffset/3
                                drawSyllabeNextLines(txtNextColor, syllabes[i], (int)x1, (int)y1 + 2 * ChordOffset / 3, W, H, e);
                            }
                            else
                            {
                                // No chords
                                y1 = y0 + (k + 1) * offset;
                                drawSyllabeNextLines(txtNextColor, syllabes[i], (int)x1, (int)y1, W, H, e);
                            }
                                                                                        
                        }
                    }
                    else if (syllabes[i].line > line)
                    {
                        x0 = i;
                        break;
                    }
                }

            }

            #endregion draw lyrics               

        }

        #endregion draw


        #region backgroundworker

        private string SelectRndFile(List<string> files)
        {
            string retfile = string.Empty;
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
                Stop();                

                int C = m_ImageFilePaths.Count;

                switch (C)
                {
                    case 0:                        
                        break;
                    case 1:                        
                        pboxWnd.Image = Image.FromFile(m_ImageFilePaths[0]);
                        break;
                    default:                        
                        Start();
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
                        pboxWnd.Invalidate();
                    }                                       
                }
                catch (Exception op)
                {
                    m_CurrentImage = null;
                    Console.Write("Error opening image " + op.Message);
                }
                
                
                // do not launch if new slideshow is required
                if (m_Restart == false)
                {                   
                    //UpdateTimerEnable(true);
                    //m_FinishEvent.WaitOne();
                    m_FinishEvent.Reset();

                    PAUSE_TIME = 1000 * freqSlideShow;
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
                    pboxWnd.Invoke(d, new object[] { enabled });
                }
                catch (Exception u)
                {
                    Console.Write("Error UpdateTimerEnable " + u.Message);
                }
            }           
        }


        private void Start()
        {
            if (backgroundWorkerSlideShow.IsBusy)
            {
                Stop();                
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
                Console.Write("Error starting backgroundworker: " + est.Message);
            }
        }

        private void Stop()
        {
            if (backgroundWorkerSlideShow.IsBusy)
            {
                backgroundWorkerSlideShow.CancelAsync();

                m_Cancel = true;
            }
        }


       

        #endregion backgroundworker


        #region paint resize terminate

        /// <summary>
        /// Guess if picturebox should be paint.
        /// Paint should be done only if syllable has changed
        /// </summary>
        private void SetOffset()
        {                                  
            int ctp = findPosition(_currentPosition);  // index syllabe à chanter
            int newvOffset = 0;

            // If vertical Offset change => redraw
            // Time to next line            
            float CurrentTimeToNextLineDuration = nextStartOfLineTime - _currentPosition;
            if (CurrentTimeToNextLineDuration > 0 && TimeToNextLineDuration > 0)
            {
                // As time passes, CurrentTimeToNextLineDuration decreases, so newvOffset increases
                newvOffset = Convert.ToInt32(_lineHeight - (CurrentTimeToNextLineDuration / TimeToNextLineDuration) * _lineHeight);
                if (newvOffset > vOffset) {         
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
            pboxWnd.Invalidate();
        }
        
        /// <summary>
        /// picturebox Paint event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxWnd_Paint(object sender, PaintEventArgs e)
        {
            int x;
            int y;

            // Draw background image
            #region draw background image
            if (m_CurrentImage != null)
            {
                #region sizemode
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
                #endregion

                try
                {                    
                    e.Graphics.DrawImage(m_CurrentImage, m_DisplayRectangle, 0, 0, m_CurrentImage.Width, m_CurrentImage.Height, GraphicsUnit.Pixel);
                    
                }
                catch (Exception dr)
                {
                    Console.Write("Error drawing image: " + dr.Message);
                }
            }
            #endregion


            // draw text
            #region draw text           

            if (lstLyricsLines is null || lstLyricsLines.Count == 0)
                return;

            try
            {                               
                // Create list of rectangles when line changes
                synchronize(_currentTextPos);

                // Antialiasing
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Calculate offset to center the text vertically
                int y0 = getOffsetHeight(emSize);

                if (_txtNbLines > 1)
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
            #endregion
        }


        private void ajustTextAgain()
        {
            if (lineMax != null && syllabes != null)
            {
                int pos = 0;
                AjustText(lineMax);

                if (_currentTextPos < 0)
                {
                    // Rectangles of current line
                    createListRectangles(0);
                    // Rectangles of next line
                    if (syllabes != null && syllabes.Count > 0)
                    {
                        pos = syllabes[0].last + 1;
                        // Rectangles for other lines
                        createListNextRectangles(pos);
                    }
                }
                else
                {
                    // Rectangles of current line
                    pos = _currentTextPos - syllabes[_currentTextPos].posline;
                    createListRectangles(pos);
                    
                    // Rectangles of next line
                    pos = syllabes[_currentTextPos].last + 1;
                    // Rectangles for other lines 
                    createListNextRectangles(pos);
                }
            }

        }
        
        /// <summary>
        /// picturebox resize event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pboxWnd_Resize(object sender, EventArgs e)
        {
            if (this.ParentForm != null && this.ParentForm.WindowState != FormWindowState.Minimized)
                ajustTextAgain();

            #region redraw image
            if (m_CurrentImage != null)
            {
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
            }
            #endregion

        }              

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

            if (backgroundWorkerSlideShow != null)
            {
                backgroundWorkerSlideShow.CancelAsync();                
            }
            
        }

        #endregion paint resize


        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                    if (m_ImageStream != null)
                    {
                        m_ImageStream.Dispose();
                        m_ImageStream = null;
                    }
                                       
                }

                _karaokeFont? .Dispose();
                m_font?.Dispose(); 
                m_CurrentImage? .Dispose();
                pboxWnd? .Dispose ();


                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~pictureBoxControl()
        {
            Dispose(false);
        }

        #endregion Dispose
    }
}
