using Karaboss.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.MP3
{
    public partial class frmMp3Karaoke : Form
    {

        private string DefaultDirSlideShow;
        private List<string> m_ImageFilePaths;
        private string[] bgFiles;
        private Font m_font;
        private float emSize; // Size of the font
        private Font _karaokeFont;
        private StringFormat sf;
        private string _maxline = "";

        private Timer timer;
        
        private string[] words;
        private long[] times;

        private int currentIndex;
        private int LastIndex;        
        
        //private Font font;
        private Brush KaraokeNormalBrush;
        private Brush KaraokeHighlightBrush;
        private Brush KaraokeDoneBrush;

        private float lineHeight;
                        
        bool bLineFeedRequired = false;
        int Offsetline = 0;
        int keeplines = 1;
              
        double millisecondsElapsed;

        public frmMp3Karaoke(string[] Lyrics, long[] Times)
        {
            InitializeComponent();

            LoadDefaultImage();

            InitializeKaraokeTextHighlighter(Lyrics, Times);
        }

        private void InitializeKaraokeTextHighlighter(string[] Lyrics, long[] Times)
        {
            words = Lyrics; 
            times = Times;

            _maxline = GetMaxLineLength();

            // Set default font for drawing
            _karaokeFont = new Font("Comic Sans MS", 16,FontStyle.Regular ,GraphicsUnit.Point);
            AjustText(_maxline);
            lineHeight = _karaokeFont.GetHeight();

            KaraokeNormalBrush = new SolidBrush(Color.White);
            KaraokeHighlightBrush = new SolidBrush(Color.Red);
            KaraokeDoneBrush = new SolidBrush(Color.Gray);
            

            // Set up the Timer
            timer = new Timer();
            timer.Interval = 100; // 1000 milliseconds = 1sec    // 500 milliseconds = (0.5 seconds)
            timer.Tick += Timer_Tick;

            // Starttime
            LastIndex = -1;
            currentIndex = -1;                        

            // Redraw the PictureBox
            pBox.Paint += PictureBox1_Paint;
        }
       
        public void Start()
        {            
            timer.Start();
        }

        public void Stop()
        {
            currentIndex = 0;
            Offsetline = 0;            
            bLineFeedRequired = false;
            pBox.Invalidate(); // Trigger a repaint
            timer.Stop();
        }

        public void GetPositionFromPlayer(double position)
        {
            millisecondsElapsed = position * 1000;
        }

        #region Timer
        private void Timer_Tick(object sender, EventArgs e)
        {
            int linenumber = -1;            

            // Compare with Times            
            if (millisecondsElapsed >= 0)
            {
                for (int i = 0; i < times.Count(); i++)
                {
                    // Count line number of the time
                    if (words[i].StartsWith("\r\n"))
                        linenumber ++;

                    if (millisecondsElapsed < times[i])
                    {                                                                        
                        if (i > 0)
                            currentIndex = i - 1;

                        if (currentIndex == LastIndex)
                        {
                            return;
                        }
                        LastIndex = currentIndex;                        
                        break;
                    }
                }
            }
            
            if (currentIndex >=0 && currentIndex < words.Length)
            {
                // If new line, decrease offsetline                
                if (bLineFeedRequired)
                {                                        
                    if (linenumber >= keeplines)
                    {                        
                        Offsetline = -(int)lineHeight * (linenumber - keeplines + 1);                        
                    }
                    bLineFeedRequired = false;
                }                                
                pBox.Invalidate(); // Trigger a repaint                

            }
            else
            {
                // Stop the timer when all words are highlighted
                currentIndex = 0;
                Offsetline = 0;                
                bLineFeedRequired = false;
                pBox.Invalidate(); // Trigger a repaint
                timer.Stop();
            }
        }

        #endregion

        #region Paint
        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.White);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            float x = 0;
            float y = 0;
            SizeF size;
            string Lyric;
            string s;
            //long pltime;
            int LineNumber = 0;

            bool IsNewLine = false;
            int LineLen = 0;                         

            for (int i = 0; i < words.Length; i++)
            {                                
                Lyric = words[i];
                //pltime = times[i];

                IsNewLine = false;


                // If next word is crlf, go to next line
                if (i < words.Length - 1) 
                { 
                    string l = words[i + 1];
                    if (l.StartsWith("\r\n"))
                    {
                        IsNewLine = true;
                    }
                }               

                // If crlf, go to next line
                if (Lyric.StartsWith("\r\n"))
                {                                        
                    LineNumber++;

                    y += lineHeight;
                    Lyric = Lyric.Replace("\r\n", "");

                    // Calculate the length of the line
                    int idx = i;
                    LineLen = 0;
                    do 
                    {                            
                        s = words[idx];
                        LineLen += (int)e.Graphics.MeasureString(s, _karaokeFont).Width;
                        idx++;
                    } while (idx < words.Length && !words[idx].StartsWith("\r\n"));

                    // Center the line
                    x = (pBox.Width - LineLen) / 2;

                }

                // Size of current Lyric
                size = e.Graphics.MeasureString(Lyric, _karaokeFont);

                if (i < currentIndex)
                {
                    // Draw rectangle around the word
                    e.Graphics.FillRectangle(KaraokeDoneBrush, x, y + Offsetline, size.Width, size.Height);
                }
                else if (i == currentIndex)
                {
                    if (IsNewLine)
                    {
                        // Used to calculate the offset line
                        bLineFeedRequired = true;                        
                    }

                    // Draw rectangle around the word
                    e.Graphics.FillRectangle(KaraokeHighlightBrush, x, y + Offsetline, size.Width, size.Height);
                }

                // Draw the word
                e.Graphics.DrawString(Lyric, _karaokeFont, KaraokeNormalBrush, x, y + Offsetline);

                x += size.Width;               
            }
        }

        #endregion Paint

        private void InitializeComponent()
        {
            this.pBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pBox
            // 
            this.pBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pBox.Location = new System.Drawing.Point(0, 0);
            this.pBox.Name = "pBox";
            this.pBox.Size = new System.Drawing.Size(584, 561);
            this.pBox.TabIndex = 0;
            this.pBox.TabStop = false;
            // 
            // frmMp3Karaoke
            // 
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.pBox);
            this.Name = "frmMp3Karaoke";
            this.Text = "Karaoke Text Highlighter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Karaoke_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3Karaoke_Load);
            this.Resize += new System.EventHandler(this.frmMp3Karaoke_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.PictureBox pBox;


        #region Form Events
        private void frmMp3Karaoke_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMp3KaraokeLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMp3KaraokeMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMp3KaraokeLocation = Location;
                    Properties.Settings.Default.frmMp3KaraokeSize = Size;
                    Properties.Settings.Default.frmMp3KaraokeMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }

        private void frmMp3Karaoke_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmMp3KaraokeMaximized)
            {
                Location = Properties.Settings.Default.frmMp3KaraokeLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMp3KaraokeLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMp3KaraokeSize;
            }
        }

        private void frmMp3Karaoke_Resize(object sender, EventArgs e)
        {
            AjustText(_maxline);
            lineHeight = _karaokeFont.GetHeight();
        }

        #endregion Form Events


        #region Images

        private void LoadDefaultImage()
        {
            m_ImageFilePaths = new List<string>();
            DefaultDirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            SetBackground(DefaultDirSlideShow);
        }

        public void SetBackground(string dirImages)
        {
            m_ImageFilePaths.Clear();
            LoadImageList(dirImages);
            if ( m_ImageFilePaths.Count > 0)
            {
                pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pBox.Image = Image.FromFile(m_ImageFilePaths[0]);
            }

        }

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

        #endregion Images


        #region Karaoke Text Highlighter
        /// <summary>
        /// Ajuste la taille de la fonte en fonction de la taille de pBox
        /// </summary>
        /// <param name="S"></param>
        private void AjustText(string S)
        {            
            if (S != "" && pBox != null)
            {
                Graphics g = pBox.CreateGraphics();
                float femsize;

                long inisize = (long)pBox.Font.Size;
                femsize = g.DpiX * inisize / 72;

                float textSize = MeasureString(S, femsize);
                long comp = (long)(0.90 * pBox.ClientSize.Width);

                // Texte trop large
                if (textSize > comp)
                {
                    do
                    {
                        inisize--; 
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
                        inisize++;                         
                        femsize = g.DpiX * inisize / 72;
                        textSize = MeasureString(S, femsize);
                    } while (textSize < comp);
                }
               
                if (inisize > 0)
                {
                    emSize = g.DpiY * inisize / 72;
                    emSize = (int)emSize - 2;
                    _karaokeFont = new Font(_karaokeFont.FontFamily, emSize, FontStyle.Regular, GraphicsUnit.Pixel);
                      
                    
                }
                g.Dispose();
            }
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
                using (Graphics g = pBox.CreateGraphics())
                {
                    m_font = new Font(_karaokeFont.FontFamily, femSize, FontStyle.Regular, GraphicsUnit.Pixel);

                    SizeF sz = g.MeasureString(line, m_font, new Point(0, 0), sf);
                    ret = sz.Width;

                    g.Dispose();
                }

            }
            return ret;
        }

        private string GetMaxLineLength()
        {
            
            int max = 0;
            int linelen = 0;
            string lyric;
            string line = "";
            string maxline = "";

            for (int i = 0; i < times.Count(); i++)
            {
                lyric = words[i];

                // Start a new line
                if (words[i].StartsWith("\r\n"))
                {
                    lyric = lyric.Replace("\r\n", "");
                    // Previous line is longer
                    
                    if (linelen > max)
                    {
                        maxline = line;
                        max = linelen;
                    }

                    line = lyric;
                    linelen = lyric.Length;

                }
                else
                {
                    line += lyric;
                    linelen += lyric.Length;
                }
            }

            return maxline;
        }

        #endregion

    }
}
