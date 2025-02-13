using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Karaboss.Mp3
{
    public partial class frmMp3Karaoke : Form, IMessageFilter
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

        //string[] _lyrics;
        //long[] _times;

        public frmMp3Karaoke(string[] Lyrics, long[] Times)
        {
            InitializeComponent();

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


            LoadDefaultImage();

            InitializeKaraokeTextHighlighter(Lyrics, Times);

            AddMouseMoveHandler(this);
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
            int LineNumber = 0;

            bool IsNewLine = false;
            int LineLen = 0;                         

            for (int i = 0; i < words.Length; i++)
            {                                
                Lyric = words[i];                
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMp3Karaoke));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnEditLyrics = new System.Windows.Forms.Button();
            this.btnExportLyricsToText = new System.Windows.Forms.Button();
            this.btnFrmOptions = new System.Windows.Forms.Button();
            this.btnFrmMin = new System.Windows.Forms.Button();
            this.btnFrmMax = new System.Windows.Forms.Button();
            this.btnFrmClose = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picBalls = new BallsControl.Balls();
            this.pBox = new System.Windows.Forms.PictureBox();
            this.pnlTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlWindow = new System.Windows.Forms.Panel();
            this.pnlTop.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.pnlWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEditLyrics
            // 
            this.btnEditLyrics.BackColor = System.Drawing.Color.Gray;
            this.btnEditLyrics.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnEditLyrics, "btnEditLyrics");
            this.btnEditLyrics.Name = "btnEditLyrics";
            this.btnEditLyrics.TabStop = false;
            this.toolTip1.SetToolTip(this.btnEditLyrics, resources.GetString("btnEditLyrics.ToolTip"));
            this.btnEditLyrics.UseVisualStyleBackColor = false;
            // 
            // btnExportLyricsToText
            // 
            this.btnExportLyricsToText.BackColor = System.Drawing.Color.Gray;
            this.btnExportLyricsToText.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnExportLyricsToText, "btnExportLyricsToText");
            this.btnExportLyricsToText.Name = "btnExportLyricsToText";
            this.btnExportLyricsToText.TabStop = false;
            this.toolTip1.SetToolTip(this.btnExportLyricsToText, resources.GetString("btnExportLyricsToText.ToolTip"));
            this.btnExportLyricsToText.UseVisualStyleBackColor = false;
            this.btnExportLyricsToText.Click += new System.EventHandler(this.btnExportLyricsToText_Click);
            // 
            // btnFrmOptions
            // 
            this.btnFrmOptions.BackColor = System.Drawing.Color.Gray;
            this.btnFrmOptions.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmOptions, "btnFrmOptions");
            this.btnFrmOptions.Name = "btnFrmOptions";
            this.btnFrmOptions.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmOptions, resources.GetString("btnFrmOptions.ToolTip"));
            this.btnFrmOptions.UseVisualStyleBackColor = false;
            // 
            // btnFrmMin
            // 
            this.btnFrmMin.BackColor = System.Drawing.Color.Gray;
            this.btnFrmMin.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmMin, "btnFrmMin");
            this.btnFrmMin.Name = "btnFrmMin";
            this.btnFrmMin.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmMin, resources.GetString("btnFrmMin.ToolTip"));
            this.btnFrmMin.UseVisualStyleBackColor = false;
            this.btnFrmMin.Click += new System.EventHandler(this.btnFrmMin_Click);
            // 
            // btnFrmMax
            // 
            this.btnFrmMax.BackColor = System.Drawing.Color.Gray;
            this.btnFrmMax.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmMax, "btnFrmMax");
            this.btnFrmMax.Name = "btnFrmMax";
            this.btnFrmMax.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmMax, resources.GetString("btnFrmMax.ToolTip"));
            this.btnFrmMax.UseVisualStyleBackColor = false;
            this.btnFrmMax.Click += new System.EventHandler(this.btnFrmMax_Click);
            // 
            // btnFrmClose
            // 
            this.btnFrmClose.BackColor = System.Drawing.Color.Gray;
            this.btnFrmClose.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmClose, "btnFrmClose");
            this.btnFrmClose.Name = "btnFrmClose";
            this.btnFrmClose.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmClose, resources.GetString("btnFrmClose.ToolTip"));
            this.btnFrmClose.UseVisualStyleBackColor = false;
            this.btnFrmClose.Click += new System.EventHandler(this.btnFrmClose_Click);
            this.btnFrmClose.MouseLeave += new System.EventHandler(this.btnFrmClose_MouseLeave);
            this.btnFrmClose.MouseHover += new System.EventHandler(this.btnFrmClose_MouseHover);
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlTitle);
            this.pnlTop.Controls.Add(this.picBalls);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.Black;
            this.pnlTitle.Controls.Add(this.lblTitle);
            resources.ApplyResources(this.pnlTitle, "pnlTitle");
            this.pnlTitle.Name = "pnlTitle";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.BackColor = System.Drawing.Color.Black;
            this.lblTitle.ForeColor = System.Drawing.Color.Teal;
            this.lblTitle.Name = "lblTitle";
            // 
            // picBalls
            // 
            this.picBalls.BallsBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picBalls.BallsNumber = 0;
            this.picBalls.Division = 0F;
            resources.ApplyResources(this.picBalls, "picBalls");
            this.picBalls.Name = "picBalls";
            // 
            // pBox
            // 
            resources.ApplyResources(this.pBox, "pBox");
            this.pBox.Name = "pBox";
            this.pBox.TabStop = false;
            // 
            // pnlTimer
            // 
            this.pnlTimer.Tick += new System.EventHandler(this.pnlTimer_Tick);
            // 
            // pnlWindow
            // 
            this.pnlWindow.BackColor = System.Drawing.Color.Gray;
            this.pnlWindow.Controls.Add(this.btnEditLyrics);
            this.pnlWindow.Controls.Add(this.btnExportLyricsToText);
            this.pnlWindow.Controls.Add(this.btnFrmOptions);
            this.pnlWindow.Controls.Add(this.btnFrmMin);
            this.pnlWindow.Controls.Add(this.btnFrmMax);
            this.pnlWindow.Controls.Add(this.btnFrmClose);
            resources.ApplyResources(this.pnlWindow, "pnlWindow");
            this.pnlWindow.Name = "pnlWindow";
            this.pnlWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlWindow_MouseDown);
            this.pnlWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlWindow_MouseMove);
            this.pnlWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlWindow_MouseUp);
            this.pnlWindow.Resize += new System.EventHandler(this.pnlWindow_Resize);
            // 
            // frmMp3Karaoke
            // 
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.pnlWindow);
            this.Controls.Add(this.pBox);
            this.Controls.Add(this.pnlTop);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMp3Karaoke";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Karaoke_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3Karaoke_Load);
            this.Resize += new System.EventHandler(this.frmMp3Karaoke_Resize);
            this.pnlTop.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            this.pnlWindow.ResumeLayout(false);
            this.ResumeLayout(false);

        }


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
                if (lyric == null || lyric.Length == 0)
                    continue;

                if (lyric.StartsWith("\r\n"))
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

        private void btnFrmClose_MouseHover(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.CloseOver;
        }

        private void btnFrmClose_MouseLeave(object sender, EventArgs e)
        {
            btnFrmClose.Image = Properties.Resources.Close;
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

        /// <summary>
        /// Minimize form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFrmMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }




        #endregion


        
        private void btnExportLyricsToText_Click(object sender, EventArgs e)
        {            
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                frmMp3Player frmMp3Player = GetForm<frmMp3Player>();
                frmMp3Player.ExportLyricsTags();
            }
        }


        #region Locate form
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

        #endregion Locate form
    }
}
