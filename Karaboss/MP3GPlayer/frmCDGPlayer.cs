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
/*
Modules nécessaires
BASS :
bass.dll
bass.Net.dll
bass_fx.dll
*/
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Un4seen.Bass;
using CDGNet;
using MP3GConverter;
using System.IO;
using System.Text.RegularExpressions;

namespace Karaboss
{
    public partial class frmCDGPlayer : Form
    {


        #region "Private Declarations"

        private CDGFile mCDGFile;

        private long cdgpos = 0;

        //private CdgFileIoStream mCDGStream;
        //private int mSemitones = 0;
        private bool mPaused;
        private long mFrameCount = 0;
        private bool mStop = true;
        private string mCDGFileName;
        private string mMP3FileName;
        private string mTempDir;
        private int mMP3Stream;
        private frmCDGWindow mCDGWindow = new frmCDGWindow(); 
        //private frmExportCDG2AVI mExportForm;

        private bool mBassInitalized = false;

        #endregion

        public frmCDGPlayer(string filename)
        {
            InitializeComponent();
            //mCDGWindow = new frmCDGWindow();
            mCDGWindow.FormClosing += new FormClosingEventHandler(mCDGWindow_FormClosing);

            tbFileName.Text = filename;

        }



        #region "Control Events"


        private void InitBass()
        {
            //'Add registration key here if you have a license
            BassNet.Registration("fabrice.lacharme@gmail.com", "2X1632326152222");
            
            try
            {
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                mBassInitalized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the audio playback system. " + ex.Message);
            }
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            BrowseCDGZip();
        }

        /// <summary>
        /// Export CDG to AVI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRecord_Click(object sender, EventArgs e)
        {
            if (mStop == true)
                ShowExportForm();
        }

        private void ShowExportForm()
        {
            // Affiche le formulaire frmExportCDG2AVI 
            if (Application.OpenForms.OfType<frmExportCDG2AVI>().Count() == 0)
            {
                Form mExportForm = new frmExportCDG2AVI();
                mExportForm.StartPosition = FormStartPosition.CenterScreen;
                mExportForm.Show();
            }
        }

        private void tsbPlay_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            try
            {
                StopPlayback();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void tsbPause_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void trbVolume_Scroll(object sender, EventArgs e)
        {
            AdjustVolume();
        }

        private void nudKey_ValueChanged(object sender, EventArgs e)
        {
            AdjustPitch();
        }

        #endregion


        #region Form Load Close


        private void frmCDGPlayer_Load(object sender, EventArgs e)
        {
            Location = Properties.Settings.Default.frmCDGPlayerLocation;
            // Verify if this windows is visible in extended screens
            Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
            foreach (Screen screen in Screen.AllScreens)
                rect = Rectangle.Union(rect, screen.Bounds);

            if (Location.X > rect.Width)
                Location = new Point(0, Location.Y);
            if (Location.Y > rect.Height)
                Location = new Point(Location.X, 0);

            InitBass();
        }

        private void mCDGWindow_FormClosing(Object sender, FormClosingEventArgs e)
        {
            StopPlayback();
            mCDGWindow.Hide();
            e.Cancel = true;
        }

        private void frmCDGPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopPlayback();
        }

        private void frmCDGPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.frmCDGPlayerLocation = Location;
       
            // Save settings
            Properties.Settings.Default.Save();

        }


        #endregion


        #region "CDG + MP3 Playback Operations"

        private void Pause()
        {
            mPaused = !mPaused;
            if (mMP3Stream != 0)
            {
                if (Bass.BASS_ChannelIsActive(mMP3Stream) != BASSActive.BASS_ACTIVE_PLAYING)
                {
                    Bass.BASS_ChannelPlay(mMP3Stream, false);
                    tsbPause.Text = "Pause";
                }
                else {
                    Bass.BASS_ChannelPause(mMP3Stream);
                    tsbPause.Text = "Resume";
                }
            }
        }


        private void PlayMP3Bass(string mp3FileName)
        {
            if (mBassInitalized || Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                mMP3Stream = 0;
                mMP3Stream = Bass.BASS_StreamCreateFile(mp3FileName, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);
                mMP3Stream = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(mMP3Stream, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_SAMPLE_LOOP);
                if (mMP3Stream != 0)
                {
                    AdjustPitch();
                    AdjustVolume();


                    ShowCDGWindow();

                    Bass.BASS_ChannelPlay(mMP3Stream, false);
                }
                else {
                    throw new Exception(String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()));
                }
            }
        }

        private void StopPlaybackBass()
        {
            try
            {
                Bass.BASS_Stop();
                Bass.BASS_StreamFree(mMP3Stream);
                Bass.BASS_Free();
                mMP3Stream = 0;
                mBassInitalized = false;
            } 
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void StopPlayback()
        {
            Console.Write("\nStopping play back...");

            mStop = true;
            HideCDGWindow();
            StopPlaybackBass();

            if (mCDGFile != null)
                mCDGFile.Dispose();
            CleanUp();
        }

        private void PausePlayback()
        {
            Bass.BASS_Pause();
        }

        private void ResumePlayback()
        {
            Bass.BASS_Pause();
        }

        private void Play()
        {
            try
            {
                if ((mMP3Stream != 0) && Bass.BASS_ChannelIsActive(mMP3Stream) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    StopPlayback();
                }

                PreProcessFiles();
                if (mCDGFileName == null || mMP3FileName == null)
                {
                    MessageBox.Show("Cannot find a CDG and MP3 file to play together.");
                    StopPlayback();
                    return;
                }

                mPaused = false;
                mStop = false;
                mFrameCount = 0;
                mCDGFile = new CDGFile(mCDGFileName);

                cdgpos = 0;
                long cdgLength = mCDGFile.getTotalDuration();

                // Display progress                
                startProgress(cdgLength);

                // Show frmCDGWindow ici
                PlayMP3Bass(mMP3FileName);

                DateTime startTime = DateTime.Now;
                DateTime endTime = startTime.AddMilliseconds(cdgLength);
                long millisecondsRemaining = cdgLength;

                while (millisecondsRemaining > 0)
                {
                    if (mStop)
                    {
                        break;
                    }
                    millisecondsRemaining = (long)endTime.Subtract(DateTime.Now).TotalMilliseconds;
                    cdgpos = cdgLength - millisecondsRemaining;

                    while (mPaused)
                    {
                        endTime = DateTime.Now.AddMilliseconds(millisecondsRemaining);
                        Application.DoEvents();
                    }
                    mCDGFile.renderAtPosition(cdgpos);
                    mFrameCount += 1;
                    mCDGWindow.PictureBox1.Image = mCDGFile.RGBImage;

                    Bitmap mbmp = new Bitmap(mCDGFile.RGBImage);
                    mCDGWindow.PictureBox1.BackColor = mbmp.GetPixel(1, 1);

                    mCDGWindow.PictureBox1.Refresh();

                    // TODO
                    //float myFrameRate = (float)Math.Round(mFrameCount / (pos / 1000), 1);
                    Application.DoEvents();
                }
                StopPlayback();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }

        private void AdjustPitch()
        {
            if (mMP3Stream != 0)
            {
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (float)nudKey.Value);
            }
        }

        private void AdjustVolume()
        {
            if (mMP3Stream != 0)
            {
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_VOL, trbVolume.Value == 0 ? 0 : (trbVolume.Value / 100));
            }
        }

        #endregion


        #region "File Access"

        private void BrowseCDGZip()
        {
            OpenFileDialog1.Filter = "CDG or Zip Files (*.zip, *.cdg)|*.zip;*.cdg";
            OpenFileDialog1.ShowDialog();
            tbFileName.Text = OpenFileDialog1.FileName;
        }

        private void PreProcessFiles()
        {
            string myCDGFileName = string.Empty;

            if (Regex.IsMatch(tbFileName.Text, "\\.zip$"))
            {
                string myTempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(myTempDir);
                mTempDir = myTempDir;
                myCDGFileName = Unzip.UnzipMP3GFiles(tbFileName.Text, myTempDir);

                // GO TO
                string myMP3FileName = Regex.Replace(myCDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(myMP3FileName))
                {
                    mMP3FileName = myMP3FileName;
                    mCDGFileName = myCDGFileName;
                    mTempDir = "";
                }


            }
            else if (Regex.IsMatch(tbFileName.Text, "\\.cdg$"))
            {
                myCDGFileName = tbFileName.Text;

                // GOTO
                string myMP3FileName = Regex.Replace(myCDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(myMP3FileName))
                {
                    mMP3FileName = myMP3FileName;
                    mCDGFileName = myCDGFileName;
                    mTempDir = "";
                }
            }
        }

        private void CleanUp()
        {
            if (mTempDir != null && mTempDir != "")
            {
                try
                {
                    Directory.Delete(mTempDir, true);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }
            mTempDir = "";
        }

        #endregion


        #region "CDG Graphics Window"

        private void ShowCDGWindow()
        {
            mCDGWindow.Show();
        }

        private void HideCDGWindow()
        {
            mCDGWindow.PictureBox1.Image = null;
            mCDGWindow.Hide();
        }






        #endregion


        #region ProgressBar
        private void startProgress(long max)
        {
            // Display progress                
            //progressBar1.Maximum = Convert.ToInt32(max);
            //progressBar1.Value = 0;

            positionHScrollBar.Maximum = Convert.ToInt32(max);
            positionHScrollBar.Value = 0;


            TimeSpan t = TimeSpan.FromMilliseconds(max);
            string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);


            //lblDuration.Text = duration;
            pnlDisplay.DisplayDuration(duration);

            Timer1.Start();
        }

        private void stopProgress()
        {
            Timer1.Stop();
            //progressBar1.Value = 0;
            positionHScrollBar.Value = 0;
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (mStop)
                stopProgress();
            else if (cdgpos <= positionHScrollBar.Maximum)    //cdgpos <= progressBar1.Maximum &&
            {
                //progressBar1.Value = Convert.ToInt32(cdgpos);
                positionHScrollBar.Value = Convert.ToInt32(cdgpos);

                TimeSpan t = TimeSpan.FromMilliseconds(cdgpos);
                string pos = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                
                //lblPos.Text = pos;
                pnlDisplay.displayElapsed(pos);
            }
        }


        #endregion ProgressBar

        private void btnPlay_Click(object sender, EventArgs e)
        {
            Play();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Pause();
        }

        private void sldMainVolume_Scroll(object sender, ScrollEventArgs e)
        {

            //AdjustVolume();
            if (mMP3Stream != 0)
            {
                float volume = (float)sldMainVolume.Value;
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_VOL, volume == 0 ? 0 : (volume / 100));
            }
        }
    }
}
