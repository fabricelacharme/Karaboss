﻿#region License

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
using System.Windows.Forms;
using keffect;
using static keffect.KaraokeEffect;
using TagLib.Mpeg4;
using System.Xml.Linq;

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

        private string DefaultDirSlideShow;
        private List<string> m_ImageFilePaths;
        private string[] bgFiles;
        private Font _karaokeFont;

        private frmMp3LyrOptions frmMp3LyrOptions;

        #region properties
        
        private Font _karaokefont;
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

        #endregion properties

        public frmMp3Lyrics()
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

            LoadLyrics();
         
            InitializeKaraokeText();

            AddMouseMoveHandler(this);

            LoadOptions();

        }


        private void LoadOptions()
        {
            try
            {
                KaraokeFont = Properties.Settings.Default.KaraokeFont;
                nbLyricsLines = Properties.Settings.Default.TxtNbLines;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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
        /// Display singer and song names
        /// </summary>
        /// <param name="text"></param>
        public void DisplaySinger(string text)
        {
            lblTitle.Text = text;
        }
        private void InitializeKaraokeText()
        {
          
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
                //pBox.SizeMode = PictureBoxSizeMode.StretchImage;
                //pBox.Image = Image.FromFile(m_ImageFilePaths[0]);
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
        /// Open form frmMp3LyricsEdit
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

        private void btnFrmOptions_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            frmMp3LyrOptions = new frmMp3LyrOptions();
            frmMp3LyrOptions.ShowDialog();
        }
    }
}
