#region License

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
/*
Modules nécessaires
BASS :
bass.dll
bass.Net.dll
bass_fx.dll

Documentation CDG : https://jbum.com//cdg_revealed.html
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
using Karaboss.Mp3;
using Karaboss.Utilities;

namespace Karaboss
{
    public partial class frmCDGPlayer : Form
    {

        #region "Private Declarations"

        private CDGFile mCDGFile;
        private enum Directions
        {
            Forward,
            Backward
        }
        private Directions _direction;

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
        
        // Forms
        private frmCDGWindow mCDGWindow = new frmCDGWindow();
        //private frmExportCDG2AVI mExportForm;

        private bool scrolling = false;
        private int newstart = 0;
        private int bouclestart = 0;
        private int laststart = 0;      // Start time to play     

        string CDGFullPath;
        string CDGFileName;

        long _duration;             // Duration of song
        float _frequency = 0;        // Frequency of song
        
        private int TransposeValue = 0;
        private long FrequencyRatio = 100;

        private readonly bool bPlayNow = false;

        // Playlists
        private readonly Playlist currentPlaylist;
        private PlaylistItem currentPlaylistItem;
        private readonly string _InternalSepLines = "¼";


        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,
            NextSong,           // select next song of a playlist
            Waiting,            // count down running between 2 songs of a playlist
            WaitingPaused,      // count down paused between 2 songs of a playlist
            LaunchNextSong      // pause between 2 songs of a playlist
        }
        private PlayerStates PlayerState;


        private bool mBassInitalized = false;

        #endregion

        public frmCDGPlayer(string filename, Playlist myPlayList, bool bplay)
        {
            InitializeComponent();            
            
            CDGFullPath = filename;
            SetTitle(filename);

            InitControls();

            #region playlists
            if (myPlayList != null)
            {
                currentPlaylist = myPlayList;
                // Search file to play with its filename                
                currentPlaylistItem = currentPlaylist.Songs.Where(z => z.File == CDGFullPath).FirstOrDefault();

                lblPlaylist.Visible = true;
                int idx = currentPlaylist.SelectedIndex(currentPlaylistItem) + 1;
                lblPlaylist.Text = "PLAYLIST: " + idx + "/" + currentPlaylist.Count;

                // play asap, pause, countdown
                performPlaylistChainingChoice();
            }
            else
            {
                lblPlaylist.Visible = false;
                // If true, launch player
                bPlayNow = bplay;
                // the user asked to play the song immediately                
                if (bPlayNow)
                {
                    PlayPauseMusic();
                }
            }

            #endregion playlists
            

        }


        #region BASS

        /// <summary>
        /// Initialize Bass
        /// </summary>
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

        #endregion


        #region Export to AVI (not used)
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

        #endregion Export to AVI


       
        #region Form Load Close

        private void SetTitle(string displayName)
        {
            int NumInstance = 1;

            try
            {
                displayName = Path.GetFileName(displayName);
                if (displayName != null)
                {
                    displayName = displayName.Replace("__", ": ");
                    displayName = displayName.Replace("_", " ");
                }
                if (NumInstance > 1)
                    Text = "Karaboss CDG Player (" + NumInstance + ") - " + displayName;
                else
                    Text = "Karaboss CDG Player - " + displayName;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

            // the user asked to play the song immediately 
            //if (bPlayNow)
            //{
            //    PlayPauseMusic();
            //}
        }

       

        private void frmCDGPlayer_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopPlayback();
        }

        private void frmCDGPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                Properties.Settings.Default.frmCDGPlayerLocation = Location;

                // Save settings
                Properties.Settings.Default.Save();
            }

            //StopPlayback();

            if (Application.OpenForms.OfType<frmCDGWindow>().Count() > 0)
            {
                Application.OpenForms["frmCDGWindow"].Close();
            }

            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmExplorer"].Restore();
                Application.OpenForms["frmExplorer"].Activate();
            }

            Dispose();

        }


        #endregion


        #region "CDG + MP3 Playback Operations"     
        private void PlayMP3Bass(string mp3FileName)
        {
            if (mBassInitalized || Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, Handle))
            {
                mMP3Stream = 0;
                mMP3Stream = Bass.BASS_StreamCreateFile(mp3FileName, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);
                mMP3Stream = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(mMP3Stream, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_SAMPLE_LOOP);
                if (mMP3Stream != 0)
                {

                    // Get frequency usually 44100
                    Bass.BASS_ChannelGetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_FREQ, ref _frequency);
                    //FrequencyRatio = 100;                    
                    
                    // Transpose
                    AdjustMp3Pitch(0);
                    AdjustVolume();

                    // Display video form
                    ShowCDGWindow();

                    Bass.BASS_ChannelPlay(mMP3Stream, false);
                }
                else {
                    throw new Exception(String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()));
                }
            }
        }

        
        /// <summary>
        /// Free resources
        /// </summary>
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

      

        private void PausePlayback()
        {
            Bass.BASS_Pause();
        }

        private void ResumePlayback()
        {
            Bass.BASS_Pause();
        }
       

       
        private void AdjustVolume()
        {           
            if (mMP3Stream != 0)
            {
                float volume = (float)sldMainVolume.Value;
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_VOL, volume == 0 ? 0 : (volume / 100));

                int level = Bass.BASS_ChannelGetLevel(mMP3Stream);

            }
        }

        private void SetPosition(double pos)
        {
            if (mMP3Stream != 0)
            {
                //cdgpos = (int)pos;

                //mCDGFile.renderAtPosition(cdgpos);

                //Bass.BASS_ChannelSetPosition(mMP3Stream, pos);
                //Bass.BASS_ChannelSetPosition(mMP3Stream, Bass.BASS_ChannelSeconds2Bytes(mMP3Stream, 20.20), BASSMode.BASS_POS_BYTE);
                //Bass.BASS_ChannelSetPosition(mMP3Stream, Bass.BASS_ChannelSeconds2Bytes(mMP3Stream, pos), BASSMode.BASS_POS_BYTE);
            }
        }
        


        #endregion


        #region File Access

        private void BrowseCDGZip()
        {
            OpenFileDialog1.Title = "Open CDG file";
            OpenFileDialog1.Filter = "CDG or Zip Files (*.zip, *.cdg)|*.zip;*.cdg";
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                CDGFullPath = OpenFileDialog1.FileName;

                SetTitle(CDGFullPath);
            }
            
        }

        private void PreProcessFiles()
        {
            string myCDGFileName = string.Empty;

            if (Regex.IsMatch(CDGFullPath, "\\.zip$"))
            {
                string myTempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                Directory.CreateDirectory(myTempDir);
                mTempDir = myTempDir;
                myCDGFileName = Unzip.UnzipMP3GFiles(CDGFullPath, myTempDir);

                // GO TO
                string myMP3FileName = Regex.Replace(myCDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(myMP3FileName))
                {
                    mMP3FileName = myMP3FileName;
                    mCDGFileName = myCDGFileName;
                    mTempDir = "";
                }
            }
            else if (Regex.IsMatch(CDGFullPath, "\\.cdg$"))
            {
                myCDGFileName = CDGFullPath;

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


        #region CDG Graphics Window

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


        #region mouse Hover, Leave

        private void BtnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_play;
        }

        private void BtnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_red_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_play;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMusic();
        }

        private void BtnStop_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnStop.Image = Properties.Resources.btn_blue_stop;
        }

        private void BtnStop_MouseLeave(object sender, EventArgs e)
        {
            btnStop.Image = Properties.Resources.btn_black_stop;
        }
        private void btnNext_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing)
                btnNext.Image = Properties.Resources.btn_blue_next;
        }

        private void btnNext_MouseLeave(object sender, EventArgs e)
        {
            btnNext.Image = Properties.Resources.btn_black_next;
        }

        private void btnPrev_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing)
                btnPrev.Image = Properties.Resources.btn_blue_prev;
        }

        private void btnPrev_MouseLeave(object sender, EventArgs e)
        {
            btnPrev.Image = Properties.Resources.btn_black_prev;
        }



        #endregion mouse Hover, Leave


        #region buttons play stop pause

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }       

        private void PlayPauseMusic()
        {
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    // If playing => pause
                    PlayerState = PlayerStates.Paused;
                    BtnStatus();
                    break;
                case PlayerStates.Paused:
                    // if paused => play
                    PlayerState = PlayerStates.Playing;
                    BtnStatus();
                    Timer1.Start();
                    ResumeMusic();
                    break;               
                default:
                    // First play                
                    FirstPlaySong(newstart);
                    break;
            }          
        }

        private void PauseResumeMusic()
        {
            mPaused = !mPaused;
            if (mMP3Stream != 0)
            {
                if (Bass.BASS_ChannelIsActive(mMP3Stream) != BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // Resume
                    Bass.BASS_ChannelPlay(mMP3Stream, false);
                    //tsbPause.Text = "Pause";
                }
                else
                {
                    // Pause
                    Bass.BASS_ChannelPause(mMP3Stream);
                    //tsbPause.Text = "Resume";
                }
            }
        }

       private void PauseMusic()
        {
            if (mMP3Stream != 0)
            {
                mPaused = true;
                // Pause
                Bass.BASS_ChannelPause(mMP3Stream);
                //tsbPause.Text = "Resume";
            }
        }

        private void ResumeMusic()
        {
            if (mMP3Stream != 0)
            {
                mPaused = false;
                // Resume
                Bass.BASS_ChannelPlay(mMP3Stream, false);
                //tsbPause.Text = "Pause";
            }
        }

        private void FirstPlaySong(int start)
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

                ValideMenus(false);

                this.Show();
                this.Activate();
                
                mCDGFile = new CDGFile(mCDGFileName);                

                cdgpos = 0;
                long cdgLength = mCDGFile.getTotalDuration();
                _duration = cdgLength;          // Duration of song
                
                // Display progress                
                startProgress(cdgLength);

                // Show frmCDGWindow ici
                PlayMP3Bass(mMP3FileName);


                #region adjust transpose & freq
                // Reset freq/tempo/pitch
                //TransposeValue = 0;
                AdjustMp3Pitch(TransposeValue);

                //FrequencyRatio = 100;
                AdjustMp3Freq(FrequencyRatio);
                #endregion adjust transpose & freq


                DateTime startTime = DateTime.Now;
                DateTime endTime = startTime.AddMilliseconds(cdgLength);
                long millisecondsRemaining = cdgLength;

                while (millisecondsRemaining > 0)
                {
                    if (mStop)
                    {
                        break;
                    }
                    millisecondsRemaining = ((long)endTime.Subtract(DateTime.Now).TotalMilliseconds);
                                        
                    
                    //cdgpos = cdgLength - millisecondsRemaining;
                    cdgpos = (cdgLength - millisecondsRemaining) * FrequencyRatio/100;        // Fonctionne si freq mise au départ, pas pendant le jeu

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

                // stop or play next song 
                PlayerState = PlayerStates.NextSong;
                //StopPlayback();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        }

        private void StopMusic()
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

        private void StopPlayback()
        {
            Console.Write("\nStopping play back...");

            mStop = true;
            HideCDGWindow();
            StopPlaybackBass();

            if (mCDGFile != null)
                mCDGFile.Dispose();
            CleanUp();

            PlayerState = PlayerStates.Stopped;
            AfterStopped();
        }


        private void AfterStopped()
        {
            ValideMenus(true);
        }

        private void startProgress(long max)
        {
            // Display progress                
            positionHScrollBar.Maximum = Convert.ToInt32(max);
            positionHScrollBar.Value = 0;

            // Duration of song
            TimeSpan t = TimeSpan.FromMilliseconds(max);
            string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            pnlDisplay.DisplayDuration(duration);
            pnlDisplay.DisplayPercent(string.Format("{0}%", 0));

            PlayerState = PlayerStates.Playing;
            BtnStatus();
            Timer1.Start();
        }

        private void stopProgress()
        {
            PlayerState = PlayerStates.Stopped;
            BtnStatus();            
            positionHScrollBar.Value = 0;

        }

        private void BtnStatus()
        {
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnStop.Image = Properties.Resources.btn_black_stop;
                    btnPlay.Enabled = true;  // to allow pause
                    btnStop.Enabled = true;  // to allow stop 
                    pnlDisplay.DisplayStatus("Playing");                    
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Enabled = true;  // to allow stop
                    pnlDisplay.DisplayStatus("Paused");
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play
                    btnStop.Image = Properties.Resources.btn_red_stop;
                    VuPeakVolumeLeft.Level = 0;
                    VuPeakVolumeRight.Level = 0;
                    pnlDisplay.DisplayStatus("Stopped");
                    break;

                case PlayerStates.LaunchNextSong:
                    // pause between 2 songs of a playlist
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Image = Properties.Resources.btn_black_stop;
                    btnStop.Enabled = true;   // to allow stop                    
                    pnlDisplay.DisplayStatus("Paused");
                    break;

                case PlayerStates.Waiting:
                    // Count down running
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnPlay.Enabled = true;  // to allow pause
                    btnStop.Enabled = true;   // to allow stop
                    pnlDisplay.DisplayStatus("Next");
                    break;

                case PlayerStates.WaitingPaused:
                    btnPlay.Image = Properties.Resources.btn_red_pause;
                    btnPlay.Enabled = true;  // to allow play
                    btnStop.Enabled = true;   // to allow stop
                    pnlDisplay.DisplayStatus("Paused");
                    break;

                case PlayerStates.NextSong:     // Select next song of a playlist
                    //VuMasterPeakVolume.Level = 0;
                    break;


                default:
                    break;
            }
        }


        /// <summary>
        /// Play next or previous mp3 in the current directory
        /// </summary>
        /// <param name="_direction"></param>
        private void PlayCDGSong(Directions _direction)
        {
            string folder;
            int index;

            // We have this information : Mp3FullPath which is the path of the file being played                
            if (Application.OpenForms.OfType<frmExplorer>().Count() == 0) return;

            frmExplorer frmExplorer = Application.OpenForms.OfType<frmExplorer>().First();

            // List of mp3 files filtered by mp3 extension
            folder = Path.GetDirectoryName(CDGFullPath);
            var files = Directory
                .EnumerateFiles(folder) //<--- .NET 4.5
                 .Where(file => file.ToLower().EndsWith("zip"))
                 .ToList();

            if (!files.Contains(CDGFullPath)) return;
            index = files.IndexOf(CDGFullPath);

            try
            {
                switch (_direction)
                {
                    // Next file
                    case Directions.Forward:
                        if (index >= files.Count - 1) return;
                        CDGFullPath = files[index + 1];
                        break;

                    // Previous file
                    case Directions.Backward:
                        if (index == 0) return;
                        CDGFullPath = files[index - 1];
                        break;

                }

                // Stop player
                StopMusic();

                // Select new file in the explorer
                CDGFileName = Path.GetFileName(CDGFullPath);
                string path = Path.GetDirectoryName(CDGFullPath);
                path = "file:///" + path.Replace("\\", "/");
                frmExplorer.NavigateTo(path, CDGFileName);

                // Update display
                SetTitle(CDGFullPath);
                //DisplayMp3Characteristics();
                //DisplayOtherInfos(CDGFullPath);

                // Play file
                PlayPauseMusic();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopMusic();
            }
        }

        /// <summary>
        /// Called by button PREVIOUS, play immediately the previous song
        /// </summary>
        private void PlayPrevSong()
        {
            if (PlayerState == PlayerStates.Paused)
            {
                laststart = bouclestart;
                //sequencer1.Position = laststart;
                return;
            }

            PlaylistItem pli = currentPlaylistItem;
            if (pli == null)
                return;

            currentPlaylistItem = currentPlaylist.Previous(pli);

            if (currentPlaylist == null || pli == currentPlaylistItem)
                return;

            StopMusic();
            //Player.Reset();

            CDGFileName = currentPlaylistItem.Song;
            CDGFullPath = currentPlaylistItem.File;

            // Update form
            SetTitle(CDGFileName);
            UpdatePlayListsForm(currentPlaylistItem.Song);

            // close frmKaraoke
            //if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            //{
            //    frmMp3Lyrics.Close();
            //}

            PlayerState = PlayerStates.Playing;
            SelectFileToLoadAsync(CDGFullPath);

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // play next song in the directory
            if (currentPlaylist == null)
            {
                PlayCDGSong(Directions.Forward);
            }
            else
            {

                // Play next song of the playlist
                // if currentPlaylist completed : all stops
                PlayNextPlaylistSong();
            }
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (currentPlaylist == null)
            {
                PlayCDGSong(Directions.Backward);
            }
            else
            {
                // Playlist
                PlayPrevSong();
            }
        }


        #endregion buttons play stop pause


        #region peak level

        private void sldMainVolume_ValueChanged(object sender, EventArgs e)
        {
            //SetMidiMasterVolume((int)sldMainVolume.Value);
            AdjustVolume();
            lblMainVolume.Text = String.Format("{0}%", 100 * sldMainVolume.Value / sldMainVolume.Maximum);
        }

        private void sldMainVolume_Scroll(object sender, ScrollEventArgs e)
        {
            AdjustVolume();
        }

        /// <summary>
        /// Get master peak volume from provider of sound (Karaboss itself or an external one such as VirtualMidiSynth)
        /// </summary>
        private void GetPeakVolume()
        {
            if (mMP3Stream != 0)
            {                                
                int level = Bass.BASS_ChannelGetLevel(mMP3Stream);
                int LeftLevel = LOWORD(level);
                int RightLevel= HIWORD(level);

                VuPeakVolumeLeft.Level = LeftLevel;
                VuPeakVolumeRight.Level = RightLevel;
                
            }
        }
        

        private static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        private static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        /// <summary>
        /// Initialize control peak volume level
        /// </summary>
        private void InitControls()
        {

            PlayerState = PlayerStates.Stopped;
            pnlDisplay.DisplayBeat("");

            pnlControls.Top = menuStrip1.Height;
            pnlControls.Left = 0;

            
            #region volume

            sldMainVolume.Maximum = 130;    // Closer to 127
            sldMainVolume.Minimum = 0;
            sldMainVolume.ScaleDivisions = 13;
            sldMainVolume.Value = 104;
            sldMainVolume.SmallChange = 13;
            sldMainVolume.LargeChange = 13;
            sldMainVolume.MouseWheelBarPartitions = 10;
            sldMainVolume.Size = new Size(24, 80);

            lblMainVolume.Text = String.Format("{0}%", 100 * sldMainVolume.Value / sldMainVolume.Maximum);

            #endregion


            #region Peak volume

            this.VuPeakVolumeLeft.AnalogMeter = false;
            this.VuPeakVolumeLeft.BackColor = System.Drawing.Color.DimGray;
            this.VuPeakVolumeLeft.DialBackground = System.Drawing.Color.White;
            this.VuPeakVolumeLeft.DialTextNegative = System.Drawing.Color.Red;
            this.VuPeakVolumeLeft.DialTextPositive = System.Drawing.Color.Black;
            this.VuPeakVolumeLeft.DialTextZero = System.Drawing.Color.DarkGreen;

            // LED 1
            this.VuPeakVolumeLeft.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeLeft.Led1ColorOn = System.Drawing.Color.LimeGreen;
            //this.VuMasterPeakVolume.Led1Count = 12;
            this.VuPeakVolumeLeft.Led1Count = 14;

            // LED 2
            this.VuPeakVolumeLeft.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuPeakVolumeLeft.Led2ColorOn = System.Drawing.Color.Yellow;
            //this.VuMasterPeakVolume.Led2Count = 12;
            this.VuPeakVolumeLeft.Led2Count = 14;

            // LED 3
            this.VuPeakVolumeLeft.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuPeakVolumeLeft.Led3ColorOn = System.Drawing.Color.Red;
            //this.VuMasterPeakVolume.Led3Count = 8;
            this.VuPeakVolumeLeft.Led3Count = 10;

            // LED size
            this.VuPeakVolumeLeft.LedSize = new System.Drawing.Size(12, 2);

            this.VuPeakVolumeLeft.LedSpace = 1;
            this.VuPeakVolumeLeft.Level = 0;
            this.VuPeakVolumeLeft.LevelMax = 32768;

            //this.VuMasterPeakVolume.Location = new System.Drawing.Point(220, 33);
            this.VuPeakVolumeLeft.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuPeakVolumeLeft.Name = "VuMasterPeakVolume";
            this.VuPeakVolumeLeft.NeedleColor = System.Drawing.Color.Black;
            this.VuPeakVolumeLeft.PeakHold = false;
            this.VuPeakVolumeLeft.Peakms = 1000;
            this.VuPeakVolumeLeft.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuPeakVolumeLeft.ShowDialOnly = false;
            this.VuPeakVolumeLeft.ShowLedPeak = false;
            this.VuPeakVolumeLeft.ShowTextInDial = false;
            this.VuPeakVolumeLeft.Size = new System.Drawing.Size(14, 120);
            this.VuPeakVolumeLeft.TabIndex = 5;
            this.VuPeakVolumeLeft.TextInDial = new string[] {
            "-40",
            "-20",
            "-10",
            "-5",
            "0",
            "+6"};
            this.VuPeakVolumeLeft.UseLedLight = false;
            this.VuPeakVolumeLeft.VerticalBar = true;
            this.VuPeakVolumeLeft.VuText = "VU";
            //this.VuPeakVolumeLeft.Location = new Point(220, 7);



            // Right
            this.VuPeakVolumeRight.AnalogMeter = false;
            this.VuPeakVolumeRight.BackColor = System.Drawing.Color.DimGray;
            this.VuPeakVolumeRight.DialBackground = System.Drawing.Color.White;
            this.VuPeakVolumeRight.DialTextNegative = System.Drawing.Color.Red;
            this.VuPeakVolumeRight.DialTextPositive = System.Drawing.Color.Black;
            this.VuPeakVolumeRight.DialTextZero = System.Drawing.Color.DarkGreen;

            // LED 1
            this.VuPeakVolumeRight.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeRight.Led1ColorOn = System.Drawing.Color.LimeGreen;
            //this.VuMasterPeakVolume.Led1Count = 12;
            this.VuPeakVolumeRight.Led1Count = 14;

            // LED 2
            this.VuPeakVolumeRight.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuPeakVolumeRight.Led2ColorOn = System.Drawing.Color.Yellow;
            //this.VuMasterPeakVolume.Led2Count = 12;
            this.VuPeakVolumeRight.Led2Count = 14;

            // LED 3
            this.VuPeakVolumeRight.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuPeakVolumeRight.Led3ColorOn = System.Drawing.Color.Red;
            //this.VuMasterPeakVolume.Led3Count = 8;
            this.VuPeakVolumeRight.Led3Count = 10;

            // LED size
            this.VuPeakVolumeRight.LedSize = new System.Drawing.Size(12, 2);

            this.VuPeakVolumeRight.LedSpace = 1;
            this.VuPeakVolumeRight.Level = 0;
            this.VuPeakVolumeRight.LevelMax = 32768;

            //this.VuMasterPeakVolume.Location = new System.Drawing.Point(220, 33);
            this.VuPeakVolumeRight.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuPeakVolumeRight.Name = "VuMasterPeakVolume";
            this.VuPeakVolumeRight.NeedleColor = System.Drawing.Color.Black;
            this.VuPeakVolumeRight.PeakHold = false;
            this.VuPeakVolumeRight.Peakms = 1000;
            this.VuPeakVolumeRight.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuPeakVolumeRight.ShowDialOnly = false;
            this.VuPeakVolumeRight.ShowLedPeak = false;
            this.VuPeakVolumeRight.ShowTextInDial = false;
            this.VuPeakVolumeRight.Size = new System.Drawing.Size(14, 120);
            this.VuPeakVolumeRight.TabIndex = 5;
            this.VuPeakVolumeRight.TextInDial = new string[] {
            "-40",
            "-20",
            "-10",
            "-5",
            "0",
            "+6"};
            this.VuPeakVolumeRight.UseLedLight = false;
            this.VuPeakVolumeRight.VerticalBar = true;
            this.VuPeakVolumeRight.VuText = "VU";
            //this.VuPeakVolumeRight.Location = new Point(220, 7);
            
            #endregion Peak volume
        }

        #endregion peak level


        #region Menus

        /// <summary>
        /// Valid or not some menus if playing or not
        /// </summary>
        /// <param name="enabled"></param>
        private void ValideMenus(bool enabled)
        {
            menuStrip1.Visible = enabled;            
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            BrowseCDGZip();
        }

        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }



        #endregion Menus


        #region Timer              

        private void DisplayTimeElapse(long cpos)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(cpos);
            string pos = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            pnlDisplay.DisplayElapsed(pos);

            double dpercent = 100 * cpos / (double)_duration;
            pnlDisplay.DisplayPercent(string.Format("{0}%", (int)dpercent));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (scrolling) return;

            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    GetPeakVolume();
                    if (cdgpos <= positionHScrollBar.Maximum && cdgpos >= positionHScrollBar.Minimum)
                    {
                        positionHScrollBar.Value = Convert.ToInt32(cdgpos);
                        // Display time elapse               
                        DisplayTimeElapse(cdgpos);
                    }
                    break;

                case PlayerStates.Paused:
                    PauseMusic();
                    Timer1.Stop();
                    break;

                case PlayerStates.Stopped:
                    if (mStop)
                        stopProgress();
                    Timer1.Stop();
                    AfterStopped();
                    break;

                case PlayerStates.NextSong:
                    PlayNextPlaylistSong();
                    break;

                default:
                    break;
            }

        }


       

        #endregion Timer

        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                // scrollbar position = fraction  of sequence Length
                double n = (e.NewValue / (float)(positionHScrollBar.Maximum - positionHScrollBar.Minimum)) * _duration;
                //newstart = (int)n;

                //SetPosition(n);
                
                scrolling = false;
            }
            else if (e.Type != ScrollEventType.First)
            {
                // Explain: remove ScrollEventType.First when using the keyboard to pause, start, rewind
                // Without this, scrolling is set to true
                scrolling = true;
            }
        }


        #region Transpose song

        /// <summary>
        /// Transpose +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranspoPlus_Click(object sender, EventArgs e)
        {
            //if (mMP3Stream == 0) return;
                            
            TransposeValue++;
            if (TransposeValue > 100) return;
                
            AdjustMp3Pitch(TransposeValue);
            
        }

        /// <summary>
        /// Transpose -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranspoMinus_Click(object sender, EventArgs e)
        {
            //if (mMP3Stream == 0) return;
                            
            TransposeValue--;                                
            AdjustMp3Pitch(TransposeValue);            
        }

        /// <summary>
        /// Transpose song
        /// </summary>
        /// <param name="amount"></param>
        private void AdjustMp3Pitch(float amount)
        {                                                
            lblTranspoValue.Text = string.Format("{0}", amount);

            if (mMP3Stream == 0) return;
            try
            {
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, amount);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        #endregion Transpose song


        #region Tempo/freq

        /// <summary>
        /// Tempo/freq +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoPlus_Click(object sender, EventArgs e)
        {
            //freq Samplerate in Hz(must be within 5 % to 5000 % of the original sample rate - Usually 44100)
            //if (mMP3Stream == 0) return;
            
            FrequencyRatio += 10;
            if (FrequencyRatio > 5000) return;

            AdjustMp3Freq(FrequencyRatio);
        }

        /// <summary>
        /// Tempo/freq -
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoMinus_Click(object sender, EventArgs e)
        {
            //freq Samplerate in Hz(must be within 5 % to 5000 % of the original sample rate - Usually 44100)
            //if (mMP3Stream == 0) return;

            FrequencyRatio -= 10;
            if (FrequencyRatio < 5) return;

            AdjustMp3Freq(FrequencyRatio);            
        }

        /// <summary>
        /// Ajust Tempo/freq
        /// </summary>
        /// <param name="amount"></param>
        private void AdjustMp3Freq(long amount)
        {
            //freq Samplerate in Hz(must be within 5 % to 5000 % of the original sample rate - Usually 44100)
                                            
            if (amount < 5 || amount > 5000) return;

            lblTempoValue.Text = string.Format("{0}%", amount);


            if (mMP3Stream == 0) return;
            double d = _duration / (amount / 100.0);            
            TimeSpan t = TimeSpan.FromMilliseconds(d);
            string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            pnlDisplay.DisplayDuration(duration);

            try
            {
                Bass.BASS_ChannelSetAttribute(mMP3Stream, BASSAttribute.BASS_ATTRIB_FREQ, _frequency * amount / 100);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        #endregion Tempo freq


        #region Playlists  

        /// <summary>
        /// Select action to perform between 2 songs according to user's choices
        /// Pause, Count Down, play asap
        /// </summary>
        private void performPlaylistChainingChoice()
        {
            // If mode pause between songs of a playlist 
            // Display a waiting information (not the words)
            if (Karaclass.m_PauseBetweenSongs)
            {
                PlayerState = PlayerStates.LaunchNextSong;
                BtnStatus();
                

                // Focus on paused windows
                this.Restore();
                this.Activate();
            }
            else
            {
                // NO PAUSE MODE
                if (Karaclass.m_CountdownSongs == 0)
                {
                    // NO Timer => play                    
                    PlayerState = PlayerStates.Stopped;
                    PlayPauseMusic();
                }
                else
                {
                    // No pause mode between songs of a playlist, but a timer 
                    // Lauch Countdown timer
                    StartCountDownTimer();
                }
            }
        }


        /// <summary>
        /// Start count down before playing
        /// </summary>
        private void StartCountDownTimer()
        {
            PlayerState = PlayerStates.Waiting;
            BtnStatus();
            /*
            w_tick = 0;
            int sec = Karaclass.m_CountdownSongs;  // wait for x seconds
            w_wait = sec + 4;

            if (Application.OpenForms.OfType<frmLyric>().Count() == 0)
            {
                frmLyric = new frmLyric(myLyricsMgmt);
                frmLyric.Show();
            }

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                // Display song & singer
                string nextsong = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                string txt = "Next song: " + nextsong + " - Next singer: " + currentPlaylistItem.KaraokeSinger;
                frmLyric.DisplaySinger(txt);

                frmLyric.LoadWaitSong(sec);
            }

            timer5.Interval = 1000;  // interval = 1 sec      
            timer5.Enabled = true;
            */
        }


        /// <summary>
        /// Common to button next and end of playing a song
        /// </summary>
        private void PlayNextPlaylistSong()
        {
            // If single song (no playlist) => STOP
            if (currentPlaylist == null)
            {
                StopMusic();
                return;
            }

            // Select next song of the playlist
            PlaylistItem pli = currentPlaylistItem;
            if (pli == null)
                return;

            currentPlaylistItem = currentPlaylist.Next(pli);

            // Stop if no other song to play
            if (pli == currentPlaylistItem)
            {
                StopMusic();
                return;
            }

            StopMusic();

            //Next song of the playlist
            CDGFileName = currentPlaylistItem.Song;

            // Update form
            SetTitle(CDGFileName);
            UpdatePlayListsForm(currentPlaylistItem.Song);

            // close frmMp3Lyrics
            if (Application.OpenForms.OfType<frmCDGWindow>().Count() > 0)
            {
                //mCDGWindow.Close();
            }

            
            PlayerState = PlayerStates.Playing;
            CDGFullPath = currentPlaylistItem.File;

            SelectFileToLoadAsync(CDGFullPath);
        }


        /// <summary>
        /// Update display of frmPlaylist
        /// </summary>
        /// <param name="song"></param>
        private void UpdatePlayListsForm(string song)
        {
            int idx = currentPlaylist.SelectedIndex(currentPlaylistItem) + 1;
            lblPlaylist.Text = "PLAYLIST: " + idx + "/" + currentPlaylist.Count;

            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                frmExplorer frmExplorer = FormUtilities.GetForm<frmExplorer>();
                frmExplorer.DisplaySong(song);
            }
        }

        private void SelectFileToLoadAsync(string FileName)
        {
            // Load file and after launch player taking account things to do betwween 2 songs                                  
            // Nothing here


            // things to do betwween 2 songs
            performPlaylistChainingChoice();
        }


        #endregion Playlists



       

        
    }
}
