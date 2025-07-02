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
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmCDGLyrics : Form
    {
        public frmCDGLyrics(string FileName)
        {
            InitializeComponent();

            // Set the form's text to the file name
            SetTitle(FileName);
            
        }


        private void SetTitle(string Title)
        {
            // Set the form's text to the title
            Title = "Karaboss - " + Path.GetFileNameWithoutExtension(Title);            
            Text = Title;
        }

        private void frmCDGLyrics_DoubleClick(object sender, EventArgs e)
        {
            AutoSizeWindow();
        }

        private void AutoSizeWindow()
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
                TopMost = true;
                Refresh();
            }
            else {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
                Refresh();
            }
        }

        private void frmCDGLyrics_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                //FormBorderStyle = FormBorderStyle.None;
                TopMost = true;
            }
            else {
                //FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
            }
        }

        private void frmCDGLyrics_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmCDGLyricsMaximized)
            {
                Location = Properties.Settings.Default.frmCDGLyricsLocation;                
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmCDGLyricsLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmCDGLyricsSize;
            }
        }

        private void frmCDGLyrics_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmCDGLyricsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmCDGLyricsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmCDGLyricsLocation = Location;
                    Properties.Settings.Default.frmCDGLyricsSize = Size;
                    Properties.Settings.Default.frmCDGLyricsMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }
        }
    }
}
