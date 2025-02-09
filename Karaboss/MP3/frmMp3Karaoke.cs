using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss.MP3
{
    public partial class frmMp3Karaoke : Form
    {

        private Timer timer;
        
        private string[] words;
        private long[] times;

        private int currentIndex;
        private int LastIndex;        
        private Font font;
        private float lineHeight;
                        
        bool bLineFeedRequired = false;
        int Offsetline = 0;
        int keeplines = 1;
              
        double millisecondsElapsed;

        public frmMp3Karaoke(string[] Lyrics, long[] Times)
        {
            InitializeComponent();
            InitializeKaraokeTextHighlighter(Lyrics, Times);
        }

        private void InitializeKaraokeTextHighlighter(string[] Lyrics, long[] Times)
        {
            words = Lyrics; 
            times = Times;

            // Set default font for drawing
            font = new Font("Arial", 16,FontStyle.Regular ,GraphicsUnit.Point);
            lineHeight = font.GetHeight();            

            // Set up the Timer
            timer = new Timer();
            timer.Interval = 100; // 1000 milliseconds = 1sec    // 500 milliseconds = (0.5 seconds)
            timer.Tick += Timer_Tick;

            // Starttime
            LastIndex = -1;
            currentIndex = -1;                        

            // Redraw the PictureBox
            pictureBox1.Paint += PictureBox1_Paint;
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
            pictureBox1.Invalidate(); // Trigger a repaint
            timer.Stop();
        }

        public void GetPositionFromPlayer(double position)
        {
            millisecondsElapsed = position * 1000;
        }

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
                                
                pictureBox1.Invalidate(); // Trigger a repaint
            }
            else
            {
                // Stop the timer when all words are highlighted
                currentIndex = 0;
                Offsetline = 0;                
                bLineFeedRequired = false;
                pictureBox1.Invalidate(); // Trigger a repaint
                timer.Stop();
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            float x = 0;
            float y = 0;
            SizeF size;
            string Lyric;
            long pltime;

            bool IsNewLine = false;

            for (int i = 0; i < words.Length; i++)
            {                                
                Lyric = words[i];
                pltime = times[i];

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
                    x = 0;
                    y += lineHeight;
                    Lyric = Lyric.Replace("\r\n", "");                    
                }
                
                size = e.Graphics.MeasureString(Lyric, font);

                if (i < currentIndex)
                {
                    // Draw rectangle around the word
                    e.Graphics.FillRectangle(Brushes.LightBlue, x, y + Offsetline, size.Width, size.Height);
                }
                else if (i == currentIndex)
                {
                    if (IsNewLine)
                    {
                        bLineFeedRequired = true;                        
                    }

                    // Draw rectangle around the word
                    e.Graphics.FillRectangle(Brushes.Yellow, x, y + Offsetline, size.Width, size.Height);

                }

                // Create a StringFormat object with the each line of text, and the block
                // of text centered on the page.
                //StringFormat stringFormat = new StringFormat();
                //stringFormat.Alignment = StringAlignment.Center;
                //stringFormat.LineAlignment = StringAlignment.Center;


                // Draw the word
                //e.Graphics.DrawString(Lyric + " ", font, Brushes.Black, x, y + Offsetline, stringFormat);
                //e.Graphics.DrawString(Lyric + " ", font, Brushes.Black, x, y + Offsetline);
                e.Graphics.DrawString(Lyric, font, Brushes.Black, x, y + Offsetline);

                x += size.Width;               
            }
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(584, 561);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // frmMp3Karaoke
            // 
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.pictureBox1);
            this.Name = "frmMp3Karaoke";
            this.Text = "Karaoke Text Highlighter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Karaoke_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3Karaoke_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.PictureBox pictureBox1;

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
    }
}
