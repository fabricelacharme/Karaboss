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
    public partial class pictureBoxControl : UserControl, IMessageFilter
    {
        /*
         * timer5_Tick de frmPlayer appelle la fonction colorLyric de frmLyrics 
         * La fenetre frmLyrics appelle la fonction ColorLyric(songposition) de picturebox control
         * Si songposition <> currenttextpos (syllabe active a changé) => redessine
         */
        //private Sanford.Multimedia.Timers.Timer timerFill;
        private BackgroundWorker backgroundWorkerSlideShow;

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


        private string strCurrentImage; // current image to insure that random will provide a different one
        private int rndIter = 0;
        

        public class plLyric
        {
            public string Type { get; set; }
            public string Element { get; set; }
            public int TicksOn { get; set; }
            public int TicksOff { get; set; }
        }

        #region properties

        public ImageLayout imgLayout { get; set; }       
        public Image m_CurrentImage { get; set; }
        public Rectangle m_DisplayRectangle { get; set; }
        public int m_Alpha { get; set; }

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
            get { return _bTextBackGround;}
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

        public int currentTextPos;        
        public int textPos 
        {
            get
            { return currentTextPos; }
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

        private int m_step = 51;          // 0 à 255 par step de 3, 5, 15, 17, 51
        private bool m_wait = false;
        
        delegate void UpdateTimerEnableCallback(bool enabled);
        #endregion SlideShow


        private int vOffset = 0;
        private int _lineHeight = 0;

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

        // Syllabes
        public class syllabe
        {
            public string text;
            public int time;            // temps de la syllabe 
            public int line;            // num de ligne  
            public int posline;         // position dans la ligne
            public int pos;             // position dans la chanson
            public int SylCount;        // Nombre de syllabes sur la meme ligne
            public int last;            // position derniére syllabe
            public int offset;          // offset horizontal
        }
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

        // Constructor
        public pictureBoxControl()
        {
            InitializeComponent();

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


        #region methods

        /// <summary>
        /// Define new slideShow directory and frequency
        /// </summary>
        /// <param name="dirImages"></param>
        public void SetBackground(string dirImages)
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
            guessInvalidate();
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
            currentTextPos = -1;
            pboxWnd.Invalidate();
        }

        /// <summary>
        /// Load text of song
        /// </summary>
        /// <param name="toto"></param>
        public void LoadSong(List<plLyric> plLyrics)
        {
            string lyrics = string.Empty;
            m_wait = false;

            if (plLyrics.Count > 0)
            {
                lyrics = string.Empty;
                pboxWnd.Invalidate();

                for (int i = 0; i < plLyrics.Count; i++)
                {
                    lyrics += plLyrics[i].Element;
                }

                this.Txt = lyrics;
                // store lines in a specific list
                StoreLyricsLines(lyrics);

                // ajust font size
                lineMax = GetMaxLength();
                AjustText(lineMax);

                // Store syllabes
                StoreLyricsSyllabes(plLyrics);

                // Position initiale 
                currentTextPos = -1;
                // Create rectangles
                createListRectangles(0);       
                if (syllabes != null && syllabes.Count > 0)
                    createListNextRectangles(syllabes[0].last + 1);
            }  
        }

        #endregion methods

        
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


        /// <summary>
        /// Count Down: decreasing numbers to wait for next song to start 
        /// </summary>
        /// <param name="sec">Count down max </param>
        public void LoadWaitSong(int sec)
        {
            vOffset = 0;
            nextStartOfLineTime = 0;

            string tx = string.Empty;

            // |10|9|8|7|6|5|4|3|2|1|0|
            //tx = "|";           
            for (int i = sec; i >= 0; i--)
            {
                tx += i.ToString() + "|";
            }
            tx += "|";

            List<plLyric> plLyrics = StoreDemoText(tx);
            
            _txtNbLines = 1;
            dirSlideShow = null;
            SetBackground(null);
            LoadSong(plLyrics);
            
            m_wait = true;
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
            // tata toto|titi tutu devient
            // tata] toto[ titi] tutu devient
            // tata], toto[, titi], tutu

            // protect spaces, replaced by ']' + space
            string S = tx.Replace(" ", "] ");
            
            // '|' = Carriage return, replaced by '[' + space
            S = S.Replace("|", "[ ");

            // Split syllabes by spaces
            string[] strLyricSyllabes = S.Split(new Char[] { ' ' });

            LyricsWords = new List<string>();
            LyricsTimes = new List<int>();

            // load lists syllabes and times
            List<plLyric> plLyrics = new List<plLyric>();

            string sx = string.Empty;
            string plElement = string.Empty;
            int plTime = 0;
            string plType = string.Empty;

            for (int i = 0; i < strLyricSyllabes.Length; i++)
            {
                sx = strLyricSyllabes[i];
                sx = sx.Replace("]", " ");    // spaces
                sx = sx.Replace("[", "\r");   // carriage return

                plElement = sx;
                plTime = ticks + (i + 1) * 10;        // time each 10 ticks

                if (sx.Length > 1 && sx.Substring(sx.Length - 1, 1) == "\r")
                {
                    // chaine Fini par \r
                    string reste = sx.Substring(0, sx.Length - 1);
                    
                    plType = "text";
                    plElement = reste;
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });

                    plType = "cr";
                    plElement = "\r";
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });

                }
                else
                {
                    if (sx == "\r")
                        plType = "cr";
                    else
                        plType = "text";
                    
                    plLyrics.Add(new plLyric() { Type = plType, Element = plElement, TicksOn = plTime });
                }                
            }
            return plLyrics;
        }
        
        /// <summary>
        /// Set default values for demonstration purpose
        /// </summary>
        private void SetDefaultValues()
        {
            _currentPosition = 30;
            currentLine = 1;
            currentTextPos = 0;

            txtBackColor = Color.Black;     
            txtContourColor = Color.White;
            txtNextColor = Color.White;
            txtBeforeColor = Color.YellowGreen;
            txtHighlightColor = Color.Red;
            _txtNbLines = 3;         

            OptionBackground = "SolidColor";                                    

            // Default dir for slide show
            DefaultDirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);

            freqSlideShow = 5 * 1000;
            m_Cancel = false;
            
            emSize = 4;
            m_font = new Font("Arial", emSize, FontStyle.Regular, GraphicsUnit.Pixel);

            sf = new StringFormat(StringFormat.GenericTypographic) { FormatFlags = StringFormatFlags.MeasureTrailingSpaces };

            pboxWnd.Font = new Font(Name = "Arial", emSize);            
            pboxWnd.SizeMode = PictureBoxSizeMode.Zoom;

            // Default text
            string tx = "Lorem ipsum dolor sit amet,|";
            tx += "consectetur adipisicing elit,|";
            tx += "sed do eiusmod tempor incididunt|";
            tx += "ut labore et dolore magna aliqua.|";
            tx += "Ut enim ad minim veniam,";

            List<plLyric> plLyrics = StoreDemoText(tx);
                        
            LoadSong(plLyrics);

            pboxWnd.Invalidate();
        }

        /// <summary>
        /// Display a text
        /// </summary>
        /// <param name="tx"></param>
        public void DisplayText(string tx, int ticks = 0)
        {
            _currentPosition = 0;
            currentLine = 1;
            currentTextPos = 0;
            
            List<plLyric> plLyrics = StoreDemoText(tx, ticks);
            LoadSong(plLyrics);
            pboxWnd.Invalidate();
        }


        #region Text

        /// <summary>
        /// Store lyrics lines in a list called lstLyricsLines
        /// </summary>
        /// <param name="ly"></param>
        private void StoreLyricsLines(string ly)
        {
            lstLyricsLines = new List<string>();

            string tx = string.Empty;
            string lyr = ly.Replace("\r\n", "¼");
            lyr = lyr.Replace("\r", "¼");
            lyr = lyr.Replace("\n", "¼");

            string[] strLyricsLines = lyr.Split(new Char[] { '¼' });  

            for (int i = 0; i < strLyricsLines.Length; i++)
            {
                tx = strLyricsLines[i].Trim();
                if (tx != "")
                {
                    lstLyricsLines.Add(tx);
                }
            }

            // Number of lines (offset calculation)
            _nbLyricsLines = lstLyricsLines.Count - 1;

        }

        /// <summary>
        /// Store syllabes in a list, each item being a class called syllabe
        /// </summary>
        /// <param name="plLyrics"></param>
        private void StoreLyricsSyllabes(List<pictureBoxControl.plLyric> plLyrics)
        {
            syllabes = new List<syllabe>();

            string tx = string.Empty;
            int itime = 0;
            int idx = -1;
            int firstitem = 0;
            int indexSyllabe = 0;
            int offset = 0;
            int max = 0;
            int pos = 0;
            int iline = -1;
            int lastpos = 0;

            // Pour chaque ligne
            for (int line = 0; line < lstLyricsLines.Count; line++)
            {
                string strline = lstLyricsLines[line];
                string strwrkline = strline;
                pos = 0;
                max = 0;
                iline = -1;

                // Recherche le nombre max de syllabes dans la ligne
                int ind = indexSyllabe;
                lastpos = 0;
                do
                {
                    if (ind < plLyrics.Count)
                    {
                        tx = plLyrics[ind].Element;
                        tx = tx.Trim();
                        if (tx != "" && plLyrics[ind].Type != "cr")
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

                lastpos = 0;

                // Offset de la ligne
                offset = 0;                
                pos = 0;

                strwrkline = strline;

                // Rechercher dans cette ligne l'occurence d'une syllabe la liste des syllabes
                do
                {
                    if (indexSyllabe < plLyrics.Count)
                    {
                        tx = plLyrics[indexSyllabe].Element;
                        string trimtx = tx.Trim();
                        itime = plLyrics[indexSyllabe].TicksOn;


                        if (trimtx != "" && plLyrics[indexSyllabe].Type != "cr")
                        {
                            pos = strwrkline.IndexOf(trimtx, lastpos);

                            if (pos != -1)
                            {
                                offset = 0; // Offset de la ligne
                                lastpos = pos;

                                // Crée un nouvel item syllabe
                                syllabe syl = new syllabe();

                                idx++; //
                                iline++;

                                if (iline == 0)
                                    firstitem = idx;

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


                                // Replace used letters by a "#"
                                string rep = new string('#', trimtx.Length);
                                var regex = new Regex(Regex.Escape(trimtx));
                                strwrkline = regex.Replace(strwrkline, rep, 1);
                            }

                        }
                        else
                        {
                            indexSyllabe++;
                        }
                    }

                } while (pos != -1 && indexSyllabe < plLyrics.Count);
            }
        }

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
                        ret = (H - ((_txtNbLines) * (h + 10))) / 2;
                    else
                        ret = (H - ((_txtNbLines - 1) * (h + 10))) / 2;
                    break;

                case OptionsDisplay.Bottom:
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

                    m_font = new Font("Arial", femSize, FontStyle.Regular, GraphicsUnit.Pixel);

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
                        m_font = new Font("Arial", femSize, FontStyle.Regular, GraphicsUnit.Pixel);

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

            if (max > 60)
            {
                max = 60;
                tx = String.Empty.PadRight(max, 'X');
            }
            return tx;
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
                femsize = g.DpiY * inisize / 72;
                
                float textSize = MeasureString(S, femsize);                
                long comp = (long)(0.9*pboxWnd.ClientSize.Width);
                
                
                // Texte trop large
                if (textSize > comp)
                {
                    do
                    {
                        inisize = inisize - 1;
                        if (inisize > 0)
                        {                            
                            femsize = g.DpiY * inisize / 72;                            
                            textSize = MeasureString(S, femsize);
                        }
                    } while (textSize > comp && inisize > 0);
                }
                else
                {
                    do
                    {
                        inisize = inisize + 1;                        
                        femsize = g.DpiY * inisize / 72;                        
                        textSize = MeasureString(S, femsize);
                    } while (textSize < comp);
                }


                // ------------------------------
                // Ajustement in height 
                // ------------------------------

                float textHeight = MeasureStringHeight(S, inisize);
                float totaltextHeight;
                totaltextHeight = _txtNbLines * (textHeight + 10);                

                long compHeight = (long)(0.9*pboxWnd.ClientSize.Height);
                
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
                        }
                    } while (totaltextHeight > compHeight && inisize > 0);
                }


                if (inisize > 0)
                {
                    emSize = g.DpiY * inisize / 72;
                    m_font = new Font("Arial", emSize, FontStyle.Regular, GraphicsUnit.Pixel);
                    pboxWnd.Font = new Font(Name = "Arial", emSize);

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
                    string tx = string.Empty;
                    rListNextRect = new List<RectangleF>[_txtNbLines];
                    int line = 0;

                    for (int k = 0; k < _txtNbLines; k++)
                    {
                        line = syllabes[pos].line;
                        string strLine = lstLyricsLines[line];
                        float Offset = getOffset(strLine, emSize);           // Offset de la ligne (centré)
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
        /// </summary>
        /// <param name="itime"></param>
        /// <returns></returns>
        private int findPosition(int itime)
        {
            if (syllabes == null)
                return 0;

            int x0 = 0;

            // optimisation : partir de la dernière position connue si le temps de celle-ci est inférieur au temps actuel
            if (currentTextPos > 0 && syllabes[currentTextPos].time < itime)
                x0 = currentTextPos - 1;

            for (int i = x0; i < syllabes.Count; i++)
            {
                syllabe syllab = syllabes[i];

                // cherche la première syllabe dont le temps est supérieur à itime
                // prend la précédente
                if (itime < syllab.time)
                {
                    // Cas 1 : La première syllabe dont le temps est supérieur au temps courant est située sur la prochaine ligne
                    // Cela signifie que l'on vient de jouer la dernière syllabe de la ligne.
                    // Si "fin de ligne" et temps écoulé supérieur à 2 noires
                    // prendre la première syllabe dont le temps est supérieur au temps courant, soit l'indice "i"
                    // indiquer également qu'il ne faut pas encore colorer cette syllabe
                    if (i > 0 && syllab.posline == 0 && itime > syllabes[i - 1].time + 2 * _beatDuration)
                    {
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
                    // Beginning of line
                    x0 = syllabeposition - syllabes[syllabeposition].posline;
                    // Create list of rectangles for current line
                    createListRectangles(x0);

                    // Create list of rectangles for next line
                    createListNextRectangles(syllabes[syllabeposition].last + 1);
                }
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

            string tx = string.Empty;
            int x0 = 0;            
            int i;
            syllabe syllab;

            if (currentTextPos >= 0)
                x0 = currentTextPos - syllabes[currentTextPos].posline;
                        
            for (i = x0; i < syllabes.Count; i++)            
            {
                // dessine la ligne courante en mettant en surbrillance la syllabe correspondante à currentTextPos
                syllab = syllabes[i];

                // It is the current line
                if (syllab.line == currentLine)
                {
                    
                    if (syllabes[i].pos < currentTextPos)
                    {
                        // syllabes avant celle active
                        drawSyllabe(txtBeforeColor, syllab, y0, e);                            // déjà chanté
                    }                    
                    else if (syllab.pos == currentTextPos)
                    {
                        
                        // Surbrillance normale                            
                        if (bHighLight)
                            drawSyllabe(txtHighlightColor, syllab,y0, e);                       // surbrillance                                                        
                        else
                            drawSyllabe(txtNextColor, syllab, y0, e);


                        #region EndOfLine & calculations

                        // Calculations are made on active syllabe (celle qui correspond à currentpos !)

                        // End of line
                        if (syllab.posline == syllab.SylCount - 1)
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
                        drawSyllabe(txtNextColor, syllab, y0, e);                           // pas encore chanté
                    }                   
                }
                // Ligne immédiatement suivante
                else if (syllab.line > currentLine + 1)
                {
                    break;
                }
            }
        }


        #endregion Text


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
        private void drawSyllabe(Color clr, syllabe syl, int y0, PaintEventArgs e)
        {

            var path = new GraphicsPath();
            string tx = syl.text;            

            try
            {
                float x0 = rRect[syl.posline].X;

                #region background of syllabe                              
                if (_bTextBackGround)
                {
                    // Black background to make text more visible
                    RectangleF R = new RectangleF(x0, y0, rRect[syl.posline].Width, rRect[syl.posline].Height);
                    // background
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), R);
                }
                #endregion

                #region Draw text of syllabe
                path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point((int)x0, y0), sf);
                e.Graphics.FillPath(new SolidBrush(clr), path);
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
        /// Draw next full lines with color txtNextColor
        /// </summary>
        /// <param name="e"></param>
        private void DrawNextLines(int y0, PaintEventArgs e)
        {

            int x0 = 0;
            int i;
            int offset = _lineHeight;

            if (_txtNbLines == 1)
                offset = 0;

            // Background
            #region draw background            
            
            if (_bTextBackGround && syllabes != null)
            {
                if (currentTextPos >= 0)
                    x0 = currentTextPos - syllabes[currentTextPos].posline;


                for (int k = 0; k < _txtNbLines; k++)
                {
                    int line = currentLine + k + 1;

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
                                float x1 = rListNextRect[k][pos].X;
                                float y1 = y0 + (k + 1) * offset;
                                // Black background to make text more visible
                                RectangleF R = new RectangleF(x1, y1, rListNextRect[k][pos].Width, rListNextRect[k][pos].Height);
                                // background
                                e.Graphics.FillRectangle(new SolidBrush(Color.Black), R);
                            }
                        }
                        else if (syllabes[i].line > line)
                        {
                            x0 = i;
                            break;
                        }
                    }

                }                
            }
            #endregion


            // Draw sentence
            if (_txtNbLines > 1)
            {
                for (i = 1; i < _txtNbLines; i++)
                {
                    int idx = currentLine + i;
                    if (idx < lstLyricsLines.Count)
                    {
                        string tx = lstLyricsLines[idx];
                        x0 = getOffset(tx, emSize);

                        var path = new GraphicsPath();
                        path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point(x0, y0 + i * offset), sf);
                        e.Graphics.FillPath(new SolidBrush(txtNextColor), path);
                        e.Graphics.DrawPath(new Pen(txtContourColor), path);
                        path.Dispose();
                    }
                }
            }
            else
            {
                int idx = currentLine + 1;
                if (idx < lstLyricsLines.Count)
                {
                    string tx = lstLyricsLines[idx];
                    x0 = getOffset(tx, emSize);

                    var path = new GraphicsPath();
                    path.AddString(tx, m_font.FontFamily, (int)m_font.Style, emSize, new Point(x0, y0 + offset), sf);
                    e.Graphics.FillPath(new SolidBrush(txtNextColor), path);
                    e.Graphics.DrawPath(new Pen(txtContourColor), path);
                    path.Dispose();

                }
            }
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


        #region paint resize

        /// <summary>
        /// Guess if picturebox should be paint
        /// Paint only if syllable has changed
        /// </summary>
        private void guessInvalidate()
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
            if (ctp != currentTextPos)
            {  
                if (bEndOfLine)
                {
                    bEndOfLine = false;                    
                    vOffset = 0;
                }
                currentTextPos = ctp;                
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

            if (lstLyricsLines.Count == 0)
                return;

            try
            {
                // Syllabe position vs current time
                //currentTextPos = findPosition(_currentPosition);          // A priori pas utile, déjà calculé dans guessInvalidate
               
                // Create list of rectangles when line changes
                synchronize(currentTextPos);

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

                if (currentTextPos < 0)
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
                    pos = currentTextPos - syllabes[currentTextPos].posline;
                    createListRectangles(pos);
                    
                    // Rectangles of next line
                    pos = syllabes[currentTextPos].last + 1;
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

       

        #endregion paint resize


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
       
    
    }
}
