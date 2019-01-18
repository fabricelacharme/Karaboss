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
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.Score;
using System.IO;

namespace Karaboss
{
    public partial class frmPrint : Form
    {
        private SheetMusic sheetmusic;
        private MidiOptions options;
        //private float zoom = 1.0f;
        bool ScrollVert = true;
        private int staffHeight = 100;

        private Sequence sequence1;
        private string CurrentPath = string.Empty;

        public frmPrint(Sequence seq, string fileName)
        {
            InitializeComponent();
            CurrentPath = fileName;

            // Allow scrollbars for panel containing score
            pnlScrollView.AutoScroll = true;
            pnlScrollView.AutoSize = true;

            sequence1 = seq;
            DrawScore();
            DrawTitle(CurrentPath);

        }

        #region draw score

        /// <summary>
        /// Draw the score sheetMusic on the panel pnlScrollView
        /// </summary>
        private void DrawScore()
        {
            if (sequence1 == null)
                return;

            Cursor = Cursors.WaitCursor;

            // set options
            options = new MidiOptions(sequence1)
            {
                scrollVert = ScrollVert,
                transpose = 0,
                key = -1,
                time = sequence1.Time,
            };

            // Create object sheetMusic
            sheetmusic = new SheetMusic(sequence1, options, staffHeight)
            {
                Parent = pnlScrollView,
            };

            Cursor = Cursors.Default;
        }


        private void DrawTitle(string fileName)
        {          
            string title = Path.GetFileName(fileName);
            title = title.Replace(".mid", "").Replace("_", " ").Replace(".kar", "");
            
            lblSong.Font = new Font("Segoe UI", 15);
            lblSong.AutoSize = false;
            lblSong.TextAlign = ContentAlignment.MiddleCenter;
            lblSong.Text = title;
        }

        #endregion



        #region form load close resize

        private void FrmPrint_FormClosing(object sender, FormClosingEventArgs e)
        {           
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmPrintLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmPrintMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmPrintLocation = Location;
                    Properties.Settings.Default.frmPrintSize = Size;
                    Properties.Settings.Default.frmPrintMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }

            Dispose();
        }

        private void FrmPrint_Load(object sender, EventArgs e)
        {
            // Set window location and size
            #region window size & location
            // If window is maximized
            if (Properties.Settings.Default.frmPrintMaximized)
            {
                Location = Properties.Settings.Default.frmPrintLocation;                
                WindowState = FormWindowState.Maximized;

            }
            else
            {
                Location = Properties.Settings.Default.frmPrintLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmPrintSize;
            }
            #endregion
        }


        private void FrmPrint_Resize(object sender, EventArgs e)
        {
            if (sheetmusic!= null)
            {
                if (pnlScrollView.Width > sheetmusic.Width)
                    sheetmusic.Left = (pnlScrollView.Width - sheetmusic.Width) / 2;
                else
                    sheetmusic.Left = 0;

              
                lblSong.Width = Width - lblSong.Left;
              
            }
        }
        #endregion


        #region create PDF
        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            string message = string.Empty;

            int numpages = sheetmusic.GetTotalPages();
            SaveFileDialog dialog = new SaveFileDialog()
            {
                ShowHelp = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                DefaultExt = "pdf",
                Filter = "PDF Document (*.pdf)|*.pdf",
            };

            /* The initial filename in the dialog will be <midi filename>.pdf */
            string initname = Path.GetFileName(CurrentPath);
            initname = initname.Replace(".mid", "") + ".pdf";
            dialog.FileName = initname;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Create a dialog with a progress bar 
                Form progressDialog = new Form()
                {
                    Text = "Generating PDF Document...",
                    BackColor = Color.White,
                    Size = new Size(400, 80),
                };

                ProgressBar progressBar = new ProgressBar()
                {
                    Parent = progressDialog,
                    Size = new Size(300, 20),
                    Location = new Point(10, 10),
                    Minimum = 1,
                    Maximum = numpages + 2,
                    Value = 2,
                    Step = 1,
                };

                progressDialog.Show();
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);


                string filename = dialog.FileName;
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Create);
                    string title = Path.GetFileName(filename);

                    Karaboss.PDFWithImages pdfdocument = new PDFWithImages(stream, title, numpages);
                    for (int page = 1; page <= numpages; page++)
                    {
                        Bitmap bitmap = new Bitmap(SheetMusic.PageWidth + 40,
                                                   SheetMusic.PageHeight + 40);
                        Graphics g = Graphics.FromImage(bitmap);
                        sheetmusic.DoPrint(g, CurrentPath ,page, numpages);
                        pdfdocument.AddImage(bitmap);
                        g.Dispose();
                        bitmap.Dispose();
                        progressBar.PerformStep();
                        Application.DoEvents();
                    }
                    pdfdocument.Save();
                    stream.Close();
                    System.Threading.Thread.Sleep(500);
                }
                catch (System.IO.IOException ep)
                {
                    message = "";
                    message += "MidiSheetMusic was unable to save to file " + filename;
                    message += " because:\n" + ep.Message + "\n";

                    MessageBox.Show(message, "Error Saving File",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                progressDialog.Dispose();
            }

        }

        #endregion

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
