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
    public partial class pictureBoxControl : UserControl, IMessageFilter, IDisposable
    {
        /*
         * timer5_Tick de frmMidiPlayer appelle la fonction colorLyric de frmMidiLyrics 
         * La fenetre frmMidiLyrics appelle la fonction ColorLyric(songposition) de picturebox control
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


        #region KaraokeLyrics

        private kLyrics _kLyrics;
        public kLyrics KLyrics 
        { 
            get { return _kLyrics; } 
            set 
            {
                if (value == null) return;
                _kLyrics = value; 
                LoadSong();
            } 
        }

        #endregion KaraokeLyrics


        #region Slideshow

        /// <summary>
        /// SlideShow directory
        /// </summary>
        private string dirSlideShow;
        public string DirSlideShow
        {
            get
            { return dirSlideShow; }
            set
            {
                if (value == null) return;
                if (value != dirSlideShow)
                {
                    dirSlideShow = value;
                    
                    InitSlideShow(dirSlideShow);
                    pboxWnd.Invalidate();
                    
                }
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
            {
                freqSlideShow = value;                
            }
        }


        private string[] bgFiles;
        private string DefaultDirSlideShow;

        private List<string> m_ImageFilePaths;

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
        private Bitmap[] pictures;

        #endregion slideshow


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
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
            }
        }

        #endregion Textcolor       


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
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
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
                    pboxWnd?.Invalidate();
                }
            }
        }

        #endregion Chord color


        #region Text transform

        private int _totalLyricsLines;

        private int _nbLyricsLines = 3;
        [Description("number of lines to display")]
        public int nbLyricsLines
        {
            get
            { return _nbLyricsLines; }
            set
            {
                _nbLyricsLines = value;
                ajustTextAgain();
                pboxWnd.Invalidate();
            }
        }
        
        private bool _bforceuppercase;
        [Description("force uppercase for lyrics")]
        public bool bforceUppercase
        {
            get { return _bforceuppercase; }
            set
            {
                _bforceuppercase = value;
                if (_bdemo)
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
                pboxWnd?.Invalidate();
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
                    pboxWnd?.Invalidate();
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
                pboxWnd.Invalidate();
            }
        }
        
        private Color _Grad1Color;
        public Color Grad1Color
        {
            get { return _Grad1Color; }
            set
            {
                _Grad1Color = value;
                pboxWnd.Invalidate();
            }
        }
        
        private Color _Rhythm0Color;
        public Color Rhythm0Color
        {
            get { return _Rhythm0Color; }
            set
            {
                _Rhythm0Color = value;
                pboxWnd.BackColor = _Rhythm0Color;
                ResetSize();
                pboxWnd.Invalidate();
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
                pboxWnd.Invalidate();
            }
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
        public float GradientAngle { get { return _angle; } set { _angle = value; pboxWnd.Invalidate(); } }       

        #endregion Gradient


        #region Background

        // Background color
        private int _bpm;
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
                    pboxWnd.BackColor = _BgColor;
                    pboxWnd.Invalidate();
                }
            }
        }

        // Color beside text (background of text)
        private bool _bTextBackGround = true;
        public bool bTextBackGround
        {
            get { return _bTextBackGround; }
            set
            {
                _bTextBackGround = value;
                pboxWnd.Invalidate();
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
                        if (dirSlideShow != null && Directory.Exists(dirSlideShow) && freqSlideShow > 0)
                            InitSlideShow(dirSlideShow);
                        break;

                    case "SolidColor":
                        //m_Cancel = true;
                        Terminate();
                        _timerGradient.Stop();
                        pboxWnd.Image = null;
                        m_CurrentImage = null;
                        pboxWnd.BackColor = _BgColor;
                        pboxWnd.Invalidate();
                        break;

                    case "Gradient":
                        //m_Cancel = true;
                        Terminate();
                        pboxWnd.Image = null;
                        m_CurrentImage = null;
                        _timerGradient.Start();
                        pboxWnd.Invalidate();
                        break;

                    case "Rhythm":
                        //m_Cancel = true;
                        Terminate();
                        _timerGradient.Start();
                        pboxWnd.Image = null;
                        m_CurrentImage = null;
                        ResetSize();
                        pboxWnd.BackColor = _Rhythm0Color;
                        pboxWnd.Invalidate();
                        break;

                    case "Transparent":
                        //m_Cancel = true;
                        Terminate();
                        _timerGradient.Stop();
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
                pboxWnd.Invalidate();
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
                pboxWnd.SizeMode = _sizemode;
            }
        }

        #endregion Image display


        #region Font


        private Font m_font;
        private float emSize; // Size of the font
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


        #endregion Font


        #region Demo

        private bool _bdemo = false;
        public bool bDemo
        {
            get { return _bdemo; }
            set { _bdemo = value; }
        }

        #endregion Demo


        #region Internal lyrics separators

        private string _InternalSepLines = "¼";
        private string _InternalSepParagraphs = "½";

        #endregion


        #region Others

        public ImageLayout imgLayout { get; set; }
        public Image m_CurrentImage { get; set; }
                       
        public Rectangle m_DisplayRectangle { get; set; }        
                        
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
                              

        private bool disposed = false;                  

        private int vOffset = 0;
        private int _lineHeight = 0;

        
        private int _linesHeight = 0;        // Full song height (number of lines * line height)

        private bool bEndOfLine = false;
        private bool bHighLight = false;
        private int nextStartOfLineTime = 0;        
        private int TimeToNextLineDuration = 0;


        private List<syllabe> syllabes;
        private List<string> lstLyricsLines;    // Liste de lignes
        private List<string> lstChordsLines;    // List of lines of chords (same number of lines as lstLyricsLines but with chords instead of lyrics)


        private int currentLine = 0;
        private string lineMax; // Ligne longueur max
        
        
        private List<RectangleF> rRect;
        private List<RectangleF> rNextRect;        

        private int _beatNumber = 1;

        #endregion Others


        /// <summary>
        /// Constructor of pictureBoxControl
        /// </summary>
        public pictureBoxControl()
        {
            InitializeComponent();

            // Dipslay chords or not            
            OptionShowChords = true;

            _karaokeFont = new Font("Arial", this.Font.Size);
            _chordFont = new Font("Comic Sans MS", this._karaokeFont.Size);

            #region Move form without title bar

            Application.AddMessageFilter(this);
            controlsToMove.Add(this);
            controlsToMove.Add(this.pboxWnd);
            
            #endregion
            
            m_ImageFilePaths = new List<string>();
            //m_Alpha = 255;
            imgLayout = ImageLayout.Stretch;

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
        }


        #region Timer gradient
        private void _timerGradient_Tick(object sender, EventArgs e)
        {
            switch (_optionbackground)
            {
                case "Gradient":
                    // For diagonal gradients, we can use the angle property to set the gradient direction
                    _angle = (_angle + 1) % 360; // Increment the angle by 1 degree, wrapping around if it exceeds 360 degrees
                    pboxWnd.Invalidate(); // Force the panel to redraw with the new gradient
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
            W = ClientRectangle.Width/2;
            H = ClientRectangle.Height/2;            
        }

        #endregion Timer gradient


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

        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void InitSlideShow(string dirImages)
        {
            try
            {
                m_CurrentImage = null;                 

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
                        //C = m_ImageFilePaths.Count;
                        C = pictures.Length;
                    }

                    switch (C)
                    {
                        case 0:
                            // No image, just background color
                            //m_Cancel = true;
                            break;
                        case 1:
                            // Single image
                            //m_Cancel = true;

                            m_CurrentImage = Image.FromFile(m_ImageFilePaths[0]);
                            //pboxWnd.Image = m_CurrentImage; // Image.FromFile(m_ImageFilePaths[0]);
                            //pboxWnd.Image = pictures[0];
                            break;
                        default:

                            /*                            
                            // Slideshow => backgroundworker                            
                            m_Cancel = false;
                            // Initialize backgroundworker
                            InitBackGroundWorker();
                            random = new Random();
                            StartBgW();
                            */

                            LaunchSlideShow();

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
        public void LoadSong(bool bDemoMode = false)
        {            
            lstLyricsLines = new List<string>();
            lstChordsLines = new List<string>();
            syllabes = new List<syllabe>();

            if (_kLyrics != null && _kLyrics.Count > 0)
            {
               
                // store lines in a specific list
                if (_kLyrics != null)
                {
                    lstLyricsLines = StoreLyricsLines(_kLyrics);
                    lstChordsLines = StoreChordLines(_kLyrics);
                }
                // Number total of lines (offset calculation)
                _totalLyricsLines = lstLyricsLines.Count - 1;

                // ajust font size
                lineMax = GetMaxLength();
                AjustText(lineMax);

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

                // Create rectangles
                createListRectangles(0);
                //if (syllabes != null && syllabes.Count > 0)
                //    createListNextRectangles(syllabes[0].last + 1);
            }

        }

        #endregion public methods


        #region SlideShow with timer       

        // New Slideshow
        private void LaunchSlideShow()
        {
            mBlend = 0;
            count = 0;

            timerChangeImage?.Dispose();
            timerChangeImage = new System.Timers.Timer();
            timerChangeImage.Interval = freqSlideShow * 1000;
            timerChangeImage.Elapsed += (sender, e) => OnTimerChangeImage();

            timerTransition?.Dispose();
            timerTransition = new System.Timers.Timer();
            timerTransition.Interval = 50;
            timerTransition.Elapsed += (sender, e) => OnTimerTransition();

            try
            {
                Image1 = pictures[count];
                Image2 = pictures[++count];
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

                if ((count + 1) < pictures.Length)
                {
                    Image1 = pictures[count];
                    Image2 = pictures[++count];
                }
                else if (count < pictures.Length)
                {
                    Image1 = pictures[count];
                    Image2 = pictures[0];
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
            //mBlend = 0.0F;
            pictures = new Bitmap[bgFiles.Length];
            for (int i = 0; i < bgFiles.Length; ++i)
            {
                pictures[i] = new Bitmap(bgFiles[i]);
            }

        }

        #endregion SlideShow functions


        #endregion SlideShow with timer      


        #region demo, wait

        /// <summary>
        /// Count Down: decreasing numbers to wait for next song to start 
        /// </summary>
        /// <param name="sec">Count down max </param>
        public void LoadWaitSong(int sec)
        {
            _nbLyricsLines = 1;
            dirSlideShow = null;
            InitSlideShow(null);           

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
            _kLyrics = StoreDemoText(lines);
            LoadSong(true);
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
        private kLyrics StoreDemoText(List<string> lines, int tcks = 0)
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
                    ticks = tcks + (i + 1) * (j + 1) * 10;
                    syll = new Syllable() { Text = w, TicksOn = ticks };
                    kLine.Add(syll);
                }
                KL.Add(kLine);
            }

            return KL;
        }

       
        /// <summary>
        /// Set default values for demonstration purpose
        /// </summary>
        private void SetDefaultValues()
        {           
            _BgColor = Color.Black;     
                        
            _ActiveColor = Color.FromArgb(153, 180, 51);      // modern ui light green
            _HighlightColor = Color.FromArgb(238, 17, 17);    // modern ui dark Red;
            _InactiveColor = Color.White;

            _ActiveBorderColor = Color.Black;
            _InactiveBorderColor = Color.Black;

            _InactiveChordColor = Color.FromArgb(255, 196, 13);         // modern ui Orange
            _HighlightChordColor = Color.FromArgb(238, 17, 17);    // modern ui dark Red
            
            
            _nbLyricsLines = 3;         

            

            // Default dir for slide show
            freqSlideShow = 5 * 1000;
            DefaultDirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            DirSlideShow = DefaultDirSlideShow;

            //OptionBackground = "SolidColor";
            OptionBackground = "Diaporama";


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
            _kLyrics = StoreDemoText(lines);
           
            // Load song with demo text
            LoadSong(true);           
        }

        /// <summary>
        /// Display a text from another windows form (used in playlists to display song title and artist during the wait time before the song starts)
        /// </summary>
        /// <param name="tx"></param>
        public void DisplayText(string tx, int ticks = 0)
        {
            List<string> lines = new List<string>();

            string[] ArrayLines = tx.Split( _InternalSepLines.ToCharArray());
            for (int i = 0; i < ArrayLines.Length; i++ )
            {
                lines.Add(ArrayLines[i]);
            }
                                   

            //List<plLyric> plLyrics = StoreDemoText(tx, ticks);
            _kLyrics = StoreDemoText(lines, ticks);
                        
            LoadSong();

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
                else if (lineContent != "")
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
            string lineChords = string.Empty;
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
            List<syllabe> lstSyllabes = new List<syllabe>();

            for (int i = 0; i < kl.Lines.Count; i++)
            {
                kLine line = kl.Lines[i];

                for (int j = 0; j < line.Syllables.Count; j++)
                {
                    Syllable kSyl = line.Syllables[j];

                    syllabe syl = new syllabe();

                    syl.chord = kSyl.Chord;
                    syl.line = i;                                                                   // line number of syllabe
                    syl.posline = j;                                                                // position dans la ligne
                    syl.pos = lstSyllabes.Count;                                                    // position dans la chanson
                    syl.text = kSyl.CharType == Syllable.CharTypes.ParagraphSep ? " " : kSyl.Text;           // text of syllabe, space if paragraph
                    syl.time = kSyl.TicksOn;                                                        // time of syllabe
                    syl.SylCount = line.Syllables.Count;                                            // number of syllabes in this line                      
                    syl.last = line.Syllables.Count - 1;                                            // position of last syllabe
                    for (int k = 0; k < i; k++)
                    {
                        syl.last += kl.Lines[k].Syllables.Count;
                    }
                    syl.offset = 0;
                    lstSyllabes.Add(syl);
                }
            }
            return lstSyllabes;
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
                long comp = (long)(0.94*pboxWnd.ClientSize.Width);                

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
                    totaltextHeight = (int)2.5*totaltextHeight;
                }

                long compHeight = (long)(0.95*pboxWnd.ClientSize.Height);
                
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
                    float Offset = CenterLine(strLine, emSize);           // Offset de la ligne (centré)

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

            int x0; // = 0;
            // Si retour arriere ou avance
            if (syllabeposition < 0)
                syllabeposition = 0;

            if (syllabeposition >= 0 && syllabeposition < syllabes.Count)
            {
                if (syllabes[syllabeposition].line != currentLine)
                {
                    currentLine = syllabes[syllabeposition].line;
                    
                    // Beginning of line
                    x0 = syllabeposition - syllabes[syllabeposition].posline;
                    // Create list of rectangles for current line
                    createListRectangles(x0);

                    // Create list of rectangles for next line
                    //createListNextRectangles(syllabes[syllabeposition].last + 1);
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
        private int CenterLine(string tx, float femsize)
        {
            float ret; // = 0;
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
                            ret = (H - ((_nbLyricsLines - 1) * (h + 10))) / 2;
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

            int W;
            int H;

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
                
                if (line > _kLyrics.Lines.Count -1) return;

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
                
                x1 = CenterLine(lineContent, emSize);

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
            Bitmap bm = new Bitmap(pboxWnd.ClientSize.Width / 5, pboxWnd.ClientSize.Height / 5);
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
            e.Graphics.DrawImage(bm, pboxWnd.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);
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
            Bitmap bm = new Bitmap(pboxWnd.ClientSize.Width / 4, pboxWnd.ClientSize.Height / 4);

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
            ge.DrawImage(bm, pboxWnd.ClientRectangle, 0, 0, bm.Width, bm.Height, GraphicsUnit.Pixel);

            // finally, the text is drawn on top
            //pth.AddString(line, new FontFamily(font.Name), (int)FontStyle.Regular, emSize, new Point(x0, y0), sf);
        }

        #endregion effects

      
        #region paint resize

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

                /*
                // Check if syllabes[ctp] is a paragraph separator
                if (syllabes[ctp].SylCount == 1 && syllabes[ctp].text == " ")
                {
                    _currentTextPos = ctp - 1; // Skip paragraph separator
                }
                else 
                {                     // Update current text position
                    _currentTextPos = ctp;
                }
                */
                
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
            // Draw background image
            #region draw background image or gradient

            // Create a GraphicsPath to define the area to fill
            GraphicsPath gp;

            switch (_optionbackground) 
            {                
            
                case "SolidColor":                    
                    e.Graphics.FillRectangle(new SolidBrush(_BgColor), new Rectangle(0, 0, this.Width, this.Height));
                    break;

                case "Diaporama":                                                                             
                    if (pictures != null && pictures.Length == 1)
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

            DrawText(e);                    

            #endregion

            // Call the base class OnPaint method to ensure proper rendering            
            base.OnPaint(e);
        }

        private void DrawText(PaintEventArgs e)
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
                    // Rectangles of next line
                    if (syllabes != null && syllabes.Count > 0)
                    {
                        pos = syllabes[0].last + 1;
                        // Rectangles for other lines
                        //createListNextRectangles(pos);
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
                    //createListNextRectangles(pos);
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
                
                pboxWnd.Invalidate(); // Invalidate the panel to force a redraw with the new size
            }


            if (this.ParentForm != null && this.ParentForm.WindowState != FormWindowState.Minimized)
                ajustTextAgain();

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

        #endregion paint resize


        #region Dispose        

        /// <summary>
        /// Terminate 
        /// </summary>
        public void Terminate()
        {
            //m_Cancel = true;
            //m_Restart = false;

            m_ImageFilePaths = new List<string>();
            /*
            if (m_ImageStream != null)
            {
                m_ImageStream.Dispose();
                m_ImageStream = null;
            }
            */

            timerChangeImage?.Stop();
            timerTransition?.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                /*
                if (disposing)
                {                    
                    if (m_ImageStream != null)
                    {
                        m_ImageStream.Dispose();
                        m_ImageStream = null;
                    }                                      
                }
                */

                _karaokeFont? .Dispose();
                m_font?.Dispose(); 
                m_CurrentImage? .Dispose();
                pboxWnd? .Dispose ();
                
                timerChangeImage?.Stop();
                timerTransition?.Stop();
                timerChangeImage?.Dispose();
                timerTransition?.Dispose();

                disposed = true;
            }
        }

        public new void Dispose()
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
