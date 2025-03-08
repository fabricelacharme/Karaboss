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
using Karaboss.Mp3.Mp3Lyrics;
using Karaboss.Resources.Localization;
using Karaboss.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ListViewEx;


namespace Karaboss.Mp3
{
    public partial class frmMp3Player : Form
    {
        #region declarations
        // Listviws editing
        private Control[] Editors;
        private List<string> lstSaveItems = new List<string>();

        // Size player
        // 514;127
        private enum PlayerAppearances
        {
            Player,
            LrcGenerator,
        }
        private PlayerAppearances PlayerAppearance;
        // Dimensions        
        private readonly int SimpleMp3PlayerWidth = 517;
        private readonly int SimpleMp3PlayerHeight = 194;

        private int _LrcMillisecondsDigits = 2;

        private Mp3LyricsTypes Mp3LyricsType;
        public bool bfilemodified = false;

        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Stopped,            // Default
            Playing,
            Paused,            
            NextSong,           // select next song of a playlist
            Waiting,            // count down running between 2 songs of a playlist
            WaitingPaused,      // count down paused between 2 songs of a playlist
            LaunchNextSong      // pause between 2 songs of a playlist
        }
        private PlayerStates PlayerState;

        private readonly bool bPlayNow = false;

        private bool scrolling = false;
        private bool closing = false;
        private bool loading = false;

        private string Mp3FullPath;
        private string Mp3FileName;

        private int bouclestart = 0;
        private int laststart = 0;      // Start time to play        
                       
        private double _totalSeconds;        

        private int TransposeValue = 0;
        private long FrequencyRatio = 100;

        private readonly string _InternalSepLines = "¼";

        // Playlists
        private readonly Playlist currentPlaylist;
        private PlaylistItem currentPlaylistItem;

        //forms
        private frmMp3LyricsSimple frmMp3LyricsSimple;
        private frmMp3Lyrics frmMp3Lyrics;


        // SlideShow directory
        public string dirSlideShow;
        
        #endregion declarations

        #region Bass

        //private SYNCPROC _OnEndingSync;
        private Mp3Player Player;
        //private BassAudioEngine AudioEngine;

        #endregion Bass

       

        public frmMp3Player(string FileName, Playlist myPlayList, bool bplay)
        {
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            lvLyrics.SubItemClicked += new ListViewEx.SubItemEventHandler(lvLyrics_SubItemClicked);
            lvLyrics.SubItemEndEditing += new ListViewEx.SubItemEndEditingEventHandler(lvLyrics_SubItemEndEditing);

            // Allow form keydown
            this.KeyPreview = true;

            Mp3FullPath = FileName;
            SetTitle(FileName);

            InitControls();

            // Player appearance is normal player
            PlayerAppearance = PlayerAppearances.Player;            

            // Create mp3 Player instance                                    
            Player = new Mp3Player(FileName);
            // Create event for playing completed
            Player.PlayingCompleted += new EndingSyncHandler(HandlePlayingCompleted);            

            DisplayMp3Characteristics();
            DisplayOtherInfos(Mp3FullPath);            

            #region playlists

            if (myPlayList != null)
            {                
                currentPlaylist = myPlayList;
                // Search file to play with its filename                
                currentPlaylistItem = currentPlaylist.Songs.Where(z => z.File == Mp3FullPath).FirstOrDefault();
                
                lblPlaylist.Visible = true;
                int idx = currentPlaylist.SelectedIndex(currentPlaylistItem) + 1;
                lblPlaylist.Text = "PLAYLIST: " + idx + "/" + currentPlaylist.Count;
            }
            else
            {
                lblPlaylist.Visible = false;
            }
            #endregion playlists

            // If true, launch player
            bPlayNow = bplay;
            // the user asked to play the song immediately                
            if (bPlayNow)
            {                
                PlayPauseMusic();
            }
        }

     


        /// <summary>
        /// Display duration and set HScrollbar maximum
        /// </summary>
        private void DisplayMp3Characteristics()
        {
            if (Player == null) return;
            _totalSeconds = Player.Seconds;
            positionHScrollBar.Maximum = (int)_totalSeconds;

            TimeSpan t = TimeSpan.FromSeconds(_totalSeconds);
            string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            pnlDisplay.DisplayDuration(duration);
        }


        #region Form load close resize

        private void frmMp3Player_Load(object sender, EventArgs e)
        {
            // Set window location and size
            #region window size & location
            // If window is maximized
            if (Properties.Settings.Default.frmMp3PlayerMaximized)
            {
                Location = Properties.Settings.Default.frmMp3PlayerLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmMp3PlayerLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmMp3PlayerSize;
            }
            #endregion


            // Redim form according to the visibility of the LRC Generator
            SetPlayerAppearance();

            Editors = new Control[] {
                                txtEditing,     //numericUpDown1,		// for column 1                    
                                txtEditing,		// for column 2									
                                //dateTimePicker1,	// for column 3
								//comboBox1,			// for column 4																	
									};

            lvLyrics.DoubleClickActivation = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        private void frmMp3Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmMp3PlayerLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmMp3PlayerMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmMp3PlayerLocation = Location;

                    // SDave only if not default size
                    if (Height != SimpleMp3PlayerHeight)
                        Properties.Settings.Default.frmMp3PlayerSize = Size;

                    Properties.Settings.Default.frmMp3PlayerMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }

            if (Application.OpenForms.OfType<frmMp3LyricsSimple>().Count() > 0)
            {
                Application.OpenForms["frmMp3LyricsSimple"].Close();
            }

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                Application.OpenForms["frmMp3Lyrics"].Close();
            }

            if (Application.OpenForms.OfType<frmMp3LyricsEdit>().Count() > 0)
            {
                Application.OpenForms["frmMp3LyricsEdit"].Close();
            }


            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmExplorer"].Restore();
                Application.OpenForms["frmExplorer"].Activate();
            }

            //_OnEndingSync = null;


            Dispose();
        }

        private void frmMp3Player_FormClosed(object sender, FormClosedEventArgs e)
        {
            Player.Reset();            
        }

        private void frmMp3Player_Resize(object sender, EventArgs e)
        {
            if (PlayerAppearance == PlayerAppearances.LrcGenerator)
            {
                lvLyrics.Width = this.ClientSize.Width - lvLyrics.Left - 6;
                lvLyrics.Columns[0].Width = 90;
                lvLyrics.Columns[1].Width = lvLyrics.ClientSize.Width - 17 - lvLyrics.Columns[0].Width;

                lvLyrics.Height = pnlMiddle.Height - lvLyrics.Top - lblTimes.Height - 10;

                lblTimes.Top = lvLyrics.Top + lvLyrics.Height + 5;
                lblLyrics.Top = lblTimes.Top;

                lblMode.Width = ClientSize.Width - lblMode.Left - 6;
            }
        }

        /// <summary>
        /// Redim form if simple player or LRc generator
        /// </summary>
        private void SetPlayerAppearance()
        {
            switch (PlayerAppearance)
            {
                case PlayerAppearances.Player:
                    
                    // Hide LRC Generator

                    // Save size                
                    #region save size
                    // Copy window location to app settings                
                    if (WindowState != FormWindowState.Minimized)
                    {
                        if (WindowState == FormWindowState.Maximized)
                        {
                            Properties.Settings.Default.frmMp3PlayerLocation = RestoreBounds.Location;
                            Properties.Settings.Default.frmMp3PlayerMaximized = true;

                        }
                        else if (WindowState == FormWindowState.Normal)
                        {
                            Properties.Settings.Default.frmMp3PlayerLocation = Location;
                            if (Height != SimpleMp3PlayerHeight)
                                Properties.Settings.Default.frmMp3PlayerSize = Size;
                            Properties.Settings.Default.frmMp3PlayerMaximized = false;
                        }

                        // Save settings
                        Properties.Settings.Default.Save();
                    }
                    #endregion

                    this.MaximizeBox = false;
                    this.FormBorderStyle = FormBorderStyle.FixedSingle;
                    pnlMiddle.Visible = false;


                    if (this.WindowState == FormWindowState.Maximized)
                        WindowState = FormWindowState.Normal;

                    // Redim size to simple player
                    this.Size = new Size(SimpleMp3PlayerWidth, SimpleMp3PlayerHeight);
                    break;


                case PlayerAppearances.LrcGenerator:
                    // Show LRC Generator
                    this.MaximizeBox = true;
                    this.FormBorderStyle = FormBorderStyle.Sizable;

                    // Show LRC Generator                    
                    pnlMiddle.Visible = true;

                    #region window size & location
                    // If window is maximized
                    if (Properties.Settings.Default.frmMp3PlayerMaximized)
                    {
                        Location = Properties.Settings.Default.frmMp3PlayerLocation;
                        WindowState = FormWindowState.Maximized;
                    }
                    else
                    {
                        try
                        {
                            if (Properties.Settings.Default.frmMp3PlayerSize.Height == SimpleMp3PlayerHeight)
                            {
                                this.Size = new Size(Properties.Settings.Default.frmMp3PlayerSize.Width, 600);
                            }
                            else
                                Size = Properties.Settings.Default.frmMp3PlayerSize;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    #endregion

                    InitLrcGenerator();

                    break;                    
            }
        }


        #endregion Form load close resize


        #region buttons play stop pause

        /// <summary>
        /// Display according to play, pause, stop status
        /// </summary>
        private void BtnStatus()
        {
            // Play and pause are same button
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
                    pnlDisplay.DisplayStatus("Stopped");
                    break;

                default:
                    break;
            }
        }

        public void FirstPlaySong()
        {
            try
            {
                PlayerState = PlayerStates.Playing;                
                BtnStatus();
                ValideMenus(false);                
                Player.Play(Mp3FullPath);
                StartKaraoke();
                Timer1.Start();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Stop Music player
        /// </summary>
        private void StopMusic()
        {
            PlayerState = PlayerStates.Stopped;
            try
            {                     
                Player.Stop();
                AfterStopped();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Button play clicked: manage actions according to player status 
        /// </summary>
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
                    Player.Resume();
                    break;

                default:
                    // First play                
                    FirstPlaySong();
                    break;
            }
        }


        /// <summary>
        /// Things to do at the end of a song
        /// </summary>
        private void AfterStopped()
        {
            // Buttons play & stop 
            BtnStatus();                      
           
            DisplayTimeElapse(0);
            StopKaraoke();                
            ValideMenus(true);

            positionHScrollBar.Value = positionHScrollBar.Minimum;
           
        }


        #region buttons
        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMusic();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //PlayNextSong();
            PlayNextPlaylistSong();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            PlayPrevSong();
        }

        #endregion buttons


        #region Tempo/freq
        /// <summary>
        /// Tempo/freq +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoPlus_Click(object sender, EventArgs e)
        {
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

            // Display new duration
            double d = _totalSeconds / (amount / 100.0);
            TimeSpan t = TimeSpan.FromSeconds(d);
            string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            pnlDisplay.DisplayDuration(duration);

            Player.ChangeFrequency(amount);           
        }

        #endregion Tempo/freq


        #region Transpose

        /// <summary>
        /// Transpose +
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTranspoPlus_Click(object sender, EventArgs e)
        {            
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

            Player.AdjustPitch(amount);          
        }

        #endregion Transpose


        #region MouseHover
        private void btnPrev_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing)
                btnPrev.Image = Properties.Resources.btn_blue_prev;
        }

        private void btnPrev_MouseLeave(object sender, EventArgs e)
        {
            btnPrev.Image = Properties.Resources.btn_black_prev;
        }

        private void btnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_play;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_red_pause;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_play;
        }

        private void btnStop_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnStop.Image = Properties.Resources.btn_blue_stop;
        }

        private void btnStop_MouseLeave(object sender, EventArgs e)
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

        #endregion MouseHover

        #endregion buttons play stop pause


        #region positionHScrollBar

        /// <summary>
        /// Change player position when scrolling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                Player.SetPosition((double)e.NewValue);                
                scrolling = false;
            }
            else
            {
                scrolling = true;
            }            
        }
        #endregion positionHScrollBar


        #region volume
        private void sldMainVolume_Scroll(object sender, ScrollEventArgs e)
        {
            AdjustVolume();
        }

        private void sldMainVolume_ValueChanged(object sender, EventArgs e)
        {
            AdjustVolume();
            lblMainVolume.Text = String.Format("{0}%", 100 * sldMainVolume.Value / sldMainVolume.Maximum);
        }

        private void AdjustVolume()
        {
            if (Player == null) return;

            Player.AdjustVolume((float)sldMainVolume.Value);                        
        }

        /// <summary>
        /// Get master peak volume from provider of sound (Karaboss itself or an external one such as VirtualMidiSynth)
        /// </summary>
        private void GetPeakVolume()
        {   
            
            int level = Player.Volume;
            int LeftLevel = LOWORD(level);
            int RightLevel = HIWORD(level);

            VuPeakVolumeLeft.Level = LeftLevel;
            VuPeakVolumeRight.Level = RightLevel;
            
        }

        private static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        private static int LOWORD(int n)
        {
            return n & 0xffff;
        }



        #endregion volume


        #region menus

        /// <summary>
        /// Open LRC Generator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditLRCGenerator_Click(object sender, EventArgs e)
        {
            mnuEditLRCGenerator.Checked = !mnuEditLRCGenerator.Checked;

            // If LRC Generator visible
            if (mnuEditLRCGenerator.Checked)
            {
                // Display Lrc Generator
                PlayerAppearance = PlayerAppearances.LrcGenerator;
            }
            else
            {
                // Hide Lrc Generator
                PlayerAppearance = PlayerAppearances.Player;
            }
            SetPlayerAppearance();          
        }

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
            OpenBrowseMp3();
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
                    Text = "Karaboss mp3 Player (" + NumInstance + ") - " + displayName;
                else
                    Text = "Karaboss mp3 Player - " + displayName;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion menus


        #region Files Save Open

        /// <summary>
        /// Open mp3 file
        /// </summary>
        private void OpenBrowseMp3()
        {
            OpenFileDialog1.Title = "Open mp3 file";
            OpenFileDialog1.Filter = "mp3 Files (*.mp3)|*.mp3";
            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Mp3FullPath = OpenFileDialog1.FileName;

                SetTitle(Mp3FullPath);
            }
        }

        #endregion Files Save open


        #region Display lyrics

        /// <summary>
        /// Display form frmMp3LyricsEdit
        /// </summary>
        public void DisplayMp3EditLyricsForm()
        {
            // Display lyrics editor
            if (Application.OpenForms.OfType<frmMp3LyricsEdit>().Count() == 0)
            {
                try
                {
                    frmMp3LyricsEdit frmMp3LyricsEdit;
                    frmMp3LyricsEdit = new frmMp3LyricsEdit(Mp3FullPath);
                    frmMp3LyricsEdit.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                if (Application.OpenForms["frmMp3LyricsEdit"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmMp3LyricsEdit"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmMp3LyricsEdit"].Show();
                Application.OpenForms["frmMp3LyricsEdit"].Activate();
            }
        }

        /// <summary>
        /// Export mp3 Lyrics tags to a text file / or from lrc having the same name 
        /// </summary>
        public void ExportLyricsTags()
        {
            switch (Mp3LyricsMgmtHelper.m_mp3lyricstype) {
                case Mp3LyricsTypes.LyricsWithTimeStamps:            
                    // Lyrics included in the mp3 file
                    TagLib.Id3v2.SynchronisedLyricsFrame SyncLyricsFrame = Player.SyncLyricsFrame;
                    Mp3LyricsMgmtHelper.ExportSyncLyricsToText(SyncLyricsFrame);
                    break;

                case Mp3LyricsTypes.LRCFile:
                    Mp3LyricsMgmtHelper.ExportSyncLyricsToText(Mp3LyricsMgmtHelper.SyncLyrics);
                    break;
            }
        }

        /// <summary>
        /// Display lyrics
        /// </summary>
        /// <param name="FileName"></param>
        private void DisplayOtherInfos(string FileName)
        {
            // Reset static infos
            Mp3LyricsMgmtHelper.SyncLyrics = new System.Collections.Generic.List<System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>>();
            Mp3LyricsMgmtHelper.SyncLine = new System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>();
            
            Player.GetMp3Infos(Mp3FullPath);
            pBox.Image = Player.AlbumArtImage;
            TagLib.Tag Tag = Player.Tag;

            if (Tag == null) return;

            Mp3LyricsType = Mp3LyricsTypes.None;

            string lrcfile = string.Empty;
            string TagLyrics = string.Empty;
            string TagSubTitles = string.Empty;

            // Mp3 sync lyrics with time stamps
            TagLib.Id3v2.SynchronisedLyricsFrame SyncLyricsFrame = Player.SyncLyricsFrame;
            Mp3LyricsMgmtHelper.MySyncLyricsFrame = SyncLyricsFrame;

            // Get lyrics type origin
            Mp3LyricsType = Mp3LyricsMgmtHelper.GetLyricsType(SyncLyricsFrame, TagLyrics, TagSubTitles, FileName);
            
            // Save info
            Mp3LyricsMgmtHelper.m_mp3lyricstype = Mp3LyricsType;
            
            switch (Mp3LyricsType)
            {
                case Mp3LyricsTypes.LyricsWithTimeStamps:                    
                    Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectSyncLyrics(SyncLyricsFrame);    // KaraokeEffect
                    DisplayFrmMp3Lyrics();
                    break;

                case Mp3LyricsTypes.LRCFile:                    
                    Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectLrcLyrics(FileName);
                    DisplayFrmMp3Lyrics();
                    break;
                
                case Mp3LyricsTypes.LyricsWithoutTimeStamps:                    
                    string tx = string.Empty;
                    if (TagLyrics != null)
                        tx += TagLyrics;
                    if (TagSubTitles != null)
                        tx += TagSubTitles;
                    DisplayFrmSimpleLyrics(tx);
                    break;
                

                default:
                    //Mp3LyricsMgmtHelper.SyncTexts = null;
                    
                    // Close form if exists
                    if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                        Application.OpenForms["frmMp3Lyrics"].Close();

                    if (Application.OpenForms.OfType<frmMp3LyricsSimple>().Count() > 0)                    
                        Application.OpenForms["frmMp3LyricsSimple"].Close();
                    
                    break;
            }         
        }


        private void mnuEditLyrics_Click(object sender, EventArgs e)
        {
            DisplayFrmMp3Lyrics();
            DisplayMp3EditLyricsForm();            
        }


        /// <summary>
        /// Display Karaoke form
        /// </summary>
        /// <param name="Lyrics"></param>
        /// <param name="Times"></param>
        private void DisplayFrmMp3Lyrics()
        {
            string sSong = string.Empty;
            string sSinger = string.Empty;

            if (currentPlaylistItem != null)
            {
                sSong = currentPlaylistItem.Song;
                sSinger = currentPlaylistItem.KaraokeSinger;
            }
            else
            {
                sSong = Mp3FullPath;
            }

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                Application.OpenForms["frmMp3Lyrics"].Close();
            }
            frmMp3Lyrics = new frmMp3Lyrics();
            frmMp3Lyrics.Show();

            // Display song & current singer on top label
            string tx;
            sSong = Path.GetFileNameWithoutExtension(sSong);
            if (sSinger == "" || sSinger == "<Song reserved by>")
                tx = sSong;
            else
                tx = sSong + " - " + Strings.Singer + ": " + sSinger;

            frmMp3Lyrics.DisplaySinger(tx);

            // cas d'une playlist ou non : met à jour le diaporama
            SetSlideShow();

            StartKaraoke();
        }

        /// <summary>
        /// SliadeShow of frmMp3Lyrics
        /// </summary>
        private void SetSlideShow()
        {
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                // cas d'une playlist ou non : met à jour le diaporama
                if (currentPlaylistItem != null)
                    dirSlideShow = currentPlaylistItem.DirSlideShow;
                else
                    dirSlideShow = Properties.Settings.Default.dirSlideShow;

                frmMp3Lyrics.SetSlideShow(dirSlideShow);

            }
        }

        /// <summary>
        /// Display with simple lyrics
        /// </summary>
        /// <param name="Lyrics"></param>
        private void DisplayFrmSimpleLyrics(string Lyrics)
        {
            if (Application.OpenForms.OfType<frmMp3LyricsSimple>().Count() > 0)
            {
                Application.OpenForms["frmMp3LyricsSimple"].Close();
            }
            frmMp3LyricsSimple = new frmMp3LyricsSimple();
            frmMp3LyricsSimple.Show();
            frmMp3LyricsSimple.DisplayText(Lyrics);
        }

       
        #endregion Display lyrics


        #region Draw controls
        private void InitControls()
        {            

            PlayerState = PlayerStates.Stopped;
            pnlDisplay.DisplayBeat("");

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


            #region listview

            #region listview header
            ColumnHeader chTimeStamp = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chLyric = new System.Windows.Forms.ColumnHeader();

            lvLyrics.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            chTimeStamp,
            chLyric,});
            // 
            // chTime
            // 
            chTimeStamp.Tag = "Lyric.TimeStamp";
            chTimeStamp.TextAlign = HorizontalAlignment.Left;
            chTimeStamp.Text = "Timestamp";
            chTimeStamp.Width = 90;
            // 
            // chLyric
            // 
            chLyric.Text = "Lyrics";
            chLyric.TextAlign = HorizontalAlignment.Center;
            chLyric.Width = 350;

            #endregion listview header

            //lvArtists.Dock = DockStyle.Fill;

            lvLyrics.Font = new Font("Segoe UI", 12F);

            // Set the view to show details.
            lvLyrics.View = View.Details;

            // Allow the user to edit item text.
            lvLyrics.LabelEdit = true;

            // Allow the user to rearrange columns.
            lvLyrics.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            lvLyrics.FullRowSelect = true;

            // Display grid lines.
            lvLyrics.GridLines = true;

            // Keep selection active
            lvLyrics.HideSelection = false;

            // Sort the items in the list in ascending order.
            //lvLyrics.Sorting = SortOrder.Ascending;

            #region add langs
            string[] langs = new string[] {
                "Arabic",
                "Armenian",
                "Bulgarian",
                "Cambodian",
                "Chinese",
                "Croatian",
                "Czech",
                "Danish",
                "Dutch",
                "English,American",
                "Esperanto",
                "Estonian",
                "Finnish",
                "French",
                "Georgian",
                "German",
                "Greek",
                "Hebrew",
                "Hungarian",
                "Indonesian",
                "Irish",
                "Italian",
                "Japanese",
                "Korean",
                "Laothian",
                "Lithuanian",
                "Maori",
                "Norwegian",
                "Portuguese",
                "Romanian",
                "Russian",
                "Serbian",
                "Slovenian",
                "Spanish",
                "Swedish",
                "Thai",
                "Tibetan",
                "Turkish",
                "Ukrainian",
                "Uzbek",
                "Vietnamese",
                "Wolof",
                "Yiddish",
                "Zulu",
                "Abkhazian",
                "Afar",
                "Afrikaans",
                "Albanian",
                "Amharic",
                "Assamese",
                "Aymara",
                "Azerbaijani",
                "Bashkir",
                "Basque",
                "Bengali,Bangla",
                "Bhutani",
                "Bihari",
                "Bislama",
                "Breton",
                "Burmese",
                "Byelorussian",
                "Catalan",
                "Corsican",
                "Faeroese",
                "Fiji",
                "Frisian",
                "Gaelic(ScotsGaelic)",
                "Galician",
                "Greenlandic",
                "Guarani",
                "Gujarati",
                "Hausa",
                "Hindi",
                "Icelandic",
                "Interlingua",
                "Interlingue",
                "Inupiak",
                "Javanese",
                "Kannada",
                "Kashmiri",
                "Kazakh",
                "Kinyarwanda",
                "Kirghiz",
                "Kirundi",
                "Kurdish",
                "Latin",
                "Latvian,Lettish",
                "Lingala",
                "Macedonian",
                "Malagasy",
                "Malay",
                "Malayalam",
                "Maltese",
                "Marathi",
                "Moldavian",
                "Mongolian",
                "Nauru",
                "Nepali",
                "Occitan",
                "Oriya",
                "Oromo,Afan",
                "Pashto,Pushto",
                "Persian",
                "Polish",
                "Punjabi",
                "Quechua",
                "Rhaeto-Romance",
                "Samoan",
                "Sangro",
                "Sanskrit",
                "Serbo-Croatian",
                "Sesotho",
                "Setswana",
                "Shona",
                "Sindhi",
                "Singhalese",
                "Siswati",
                "Slovak",
                "Somali",
                "Sudanese",
                "Swahili",
                "Tagalog",
                "Tajik",
                "Tamil",
                "Tatar",
                "Telugu",
                "Tigrinya",
                "Tonga",
                "Tsonga",
                "Turkmen",
                "Twi",
                "Urdu",
                "Volapuk",
                "Welsh",
                "Xhosa",
                "Yoruba",
            };
            #endregion Add langs

            cbLanguage.Items.AddRange(langs);
            cbLanguage.SelectedItem = "French";


            #endregion listview


            #region editing

            txtEditing.Font = lvLyrics.Font;
            txtEditing.TextAlign = HorizontalAlignment.Center;

            // Passer en mode synchro
            btnSwitchSyncEdit.Text = Strings.SwitchToSyncMode; // "Switch to sync mode";
            // Mode édition: chargez un fichier LRC à modifier ou des paroles à synchroniser
            lblMode.Text = Strings.DescrEditMode; // "Edit mode: load an LRC file to be modified or lyrics to be synchronised";



            #endregion editing

        }


        #endregion Draw controls


        #region Playlists     

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
            Mp3FileName = currentPlaylistItem.Song;

            // Update form
            SetTitle(Mp3FileName);
            UpdatePlayListsForm(currentPlaylistItem.Song);

            // close frmKaraoke
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                frmMp3Lyrics.Close();
            }

            PlayerState = PlayerStates.Playing;
            Mp3FullPath = currentPlaylistItem.File;            

            SelectFileToLoadAsync(Mp3FullPath);
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

            Mp3FileName = currentPlaylistItem.Song;
            Mp3FullPath = currentPlaylistItem.File;

            // Update form
            SetTitle(Mp3FileName);
            UpdatePlayListsForm(currentPlaylistItem.Song);

            // close frmKaraoke
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                frmMp3Lyrics.Close();
            }

            PlayerState = PlayerStates.Playing;
            SelectFileToLoadAsync(Mp3FullPath);

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

            DisplayMp3Characteristics();

            DisplayOtherInfos(Mp3FullPath);


            // things to do betwween 2 songs
            performPlaylistChainingChoice();
        }

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

                #region display singer in the Lyrics form
                // Display the Lyric form even if no lyrics in order to display the singer
                /*
                if (Application.OpenForms.OfType<frmLyric>().Count() == 0)
                {
                    frmLyric = new frmLyric(myLyricsMgmt);
                    frmLyric.Show();
                }
                */

                if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
                {
                    // During the waiting time, display informations about the next singer
                    int nbLines;
                    string toptxt;
                    string centertxt;

                    if (currentPlaylistItem.KaraokeSinger == "" || currentPlaylistItem.KaraokeSinger == "<Song reserved by>")
                    {
                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        nbLines = 1;
                    }
                    else
                    {

                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song) + " - Next singer: " + currentPlaylistItem.KaraokeSinger;
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song)
                            + _InternalSepLines + Karaboss.Resources.Localization.Strings.SungBy
                            + _InternalSepLines + currentPlaylistItem.KaraokeSinger;
                        nbLines = 4;
                    }
                    
                }
                #endregion

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

        #endregion Playlists


        #region Handle events        

       


        /// <summary>
        /// Play mp3 completed
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void HandlePlayingCompleted(int handle, int channel, int data, IntPtr user)
        {
            PlayerState = PlayerStates.NextSong;
        }

        #endregion Handle events 


        #region Timer

        /// <summary>
        /// Display Time Elapse
        /// </summary>
        private void DisplayTimeElapse(double dpercent)
        {            
            pnlDisplay.DisplayPercent(string.Format("{0}%", (int)(dpercent * 100)));

            double maintenant = (dpercent * _totalSeconds);  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            pnlDisplay.DisplayElapsed(string.Format("{0:00}:{1:00}", Min, Sec));
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (scrolling) return;            
            
            double pos = Player.Position;

            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    // Display time elapse                        
                    double dpercent = pos / _totalSeconds;
                    DisplayTimeElapse(dpercent);

                    // Display volume
                    GetPeakVolume();
                    break;

                case PlayerStates.Stopped:
                    Timer1.Stop();
                    AfterStopped();
                    break;

                case PlayerStates.Paused:                    
                    Player.Pause();
                    Timer1.Stop();
                    break;

                case PlayerStates.NextSong:                                       
                    PlayNextPlaylistSong();
                    break;
            }

            #region position hscrollbar
            try
            {
                
                if (PlayerState == PlayerStates.Playing && (int)pos < positionHScrollBar.Maximum - positionHScrollBar.Minimum && (int)pos > positionHScrollBar.Minimum)
                {
                    positionHScrollBar.Value = (int)pos + positionHScrollBar.Minimum;

                    // Send position to karaoke
                    SendPositionToKaraoke(pos);
                }
                
            }
            catch (Exception ex)
            {
                Console.Write("Error positionHScrollBar.Value - " + ex.Message);
            }
            #endregion position hscrollbar
            
        }

        private void SendPositionToKaraoke(double pos)
        {
            if (Player == null) return;

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                frmMp3Lyrics.GetPositionFromPlayer(pos);

        }

        private void StopKaraoke()
        {
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                frmMp3Lyrics.Stop();
        }

        private void StartKaraoke()
        {                        
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                frmMp3Lyrics.Start();
        }

        #endregion Timer


        #region LRC generator

        string _lrcFileName;
        int _index = 0;

        private enum LrcModes
        {
            Sync,
            Edit
        }
        private LrcModes LrcMode;

        private void InitLrcGenerator()
        {
            if (PlayerAppearance != PlayerAppearances.LrcGenerator) return;

            LrcMode = LrcModes.Edit;

            lvLyrics.Items.Clear();
            _index = 0;
            _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;


            lblHotkeys.Font = new Font("Courier New", 9);
            lblHotkeys.Text = "<ENTER>" + " " + "Add a new timestamp" +"\r\n" + "<SPACE>" + " "+ "Pause Music" + "\r\n" + "<-" + "      " + "Stop Music";

            txtTitle.Text = Player.Tag.Title;
            txtAlbum.Text = Player.Tag.Album;
            if (Player.Tag.Performers.Count() > 0)
            {
                txtArtist.Text = Player.Tag.Performers[0].ToString();
                txtAuthor.Text = Player.Tag.Performers[0].ToString();
            }
            
        }

        /// <summary>
        /// Key Enter was hit
        /// </summary>
        private void AddNewLrcTimeStamp() 
        {
            if (PlayerState != PlayerStates.Playing) return;

            if (lvLyrics.Items.Count == 0)
            {
                MessageBox.Show("Please add lyrics before entering timestamps", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }                        

            TimeSpan ts;            
            string tsp;
            

            double time = Player.Position;
            ts = TimeSpan.FromMilliseconds(time * 1000);
            if (_LrcMillisecondsDigits == 2)
                tsp = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            else
                tsp = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

                      
            if (_index < lvLyrics.Items.Count)
            {
                lvLyrics.Items[_index].Text = tsp;
                if (_index < lvLyrics.Items.Count - 2)
                    lvLyrics.EnsureVisible(_index + 2);
                else
                    lvLyrics.EnsureVisible(_index);


                lblTimes.Text = _index.ToString(); // count number of lines done
                
                // Select line (paint in blue)
                lvLyrics.Items[_index].Selected = true;
                lvLyrics.Focus();

                _index++;
            }            
        }

        /// <summary>
        /// Import an LRC file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuImportLrcFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog1.Title = "Open a .lrc file";
            OpenFileDialog1.DefaultExt = "lrc";
            OpenFileDialog1.Filter = "lrc files|*.lrc|All files|*.*";

            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _lrcFileName = OpenFileDialog1.FileName;

                Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectLrcLyrics(_lrcFileName);
                List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;

                lvLyrics.Items.Clear();

                string text;
                long time;
                string sTime;
                TimeSpan ts;
                string tsp;
                ListViewItem lvi;

                // Clear save times list
                lstSaveItems.Clear();

                // For each line
                for (int j = 0; j < SyncLyrics.Count; j++)
                {
                    // For each syllabes
                    for (int i = 0; i < SyncLyrics[j].Count; i++)
                    {
                        time = SyncLyrics[j][i].Time;
                        ts = TimeSpan.FromMilliseconds(time);
                        if (_LrcMillisecondsDigits == 2)
                            tsp = string.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                        else
                            tsp = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

                        sTime = tsp;  // Transform to [00:00.000] format

                        text = SyncLyrics[j][i].Text;
                        
                        // Put "/" everywhere
                        text = text.Replace("\r\n", "");
                        text = text.Replace("\r", "");
                        text = text.Replace("\n", "");
                        text = text.Trim();

                        lvi = new ListViewItem( new string[] { sTime, text });
                        
                        lvLyrics.Items.Add(lvi);
                        
                        // Save times
                        lstSaveItems.Add(sTime);
                    }
                }

                lblLyrics.Text = lvLyrics.Items.Count.ToString();
                lblTimes.Text = lblLyrics.Text;
            }
        }


        /// <summary>
        /// Import a text file (no timestamps)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuImportRawLyrics_Click(object sender, EventArgs e)
        {
            OpenFileDialog1.Title = "Open a .txt file";
            OpenFileDialog1.DefaultExt = "txt";
            OpenFileDialog1.Filter = "text files|*.txt|All files|*.*";

            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _lrcFileName = OpenFileDialog1.FileName;

             
                lvLyrics.Items.Clear();
                ListViewItem lvi;

                string[] lines = System.IO.File.ReadAllLines(_lrcFileName);
                string line;                

                for (int i = 0; i < lines.Count(); i++)
                {
                    line = lines[i].Trim();
                    if (line != "")
                    {                        

                        // Add lyrics to listview
                        lvi = new ListViewItem();
                        lvi.Text = "";
                        lvi.SubItems.Add(line);
                        lvLyrics.Items.Add(lvi);
                    }
                }

                lblLyrics.Text = lvLyrics.Items.Count.ToString();
                lblTimes.Text = "0";
                
                lvLyrics.Items[0].Selected = true;
                lvLyrics.Focus();
            }
        
            if (MessageBox.Show(Strings.SwitchToSyncMode + "?", "Karaboss", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SetSyncEditMode();
            }

        }

        /// <summary>
        /// Export lrc file with metadata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExportLRCMeta_Click(object sender, EventArgs e)
        {
            SaveLRCFile();

        }

        private void SaveLRCFile()
        {
            
            if (lvLyrics.Items.Count == 0)
            {
                //MessageBox.Show("Some lines have no timestamp", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("Nothing to save", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

            #region Read data
            List<(double, string)> lstDgRows = new List<(double, string)>();
            string sLyric;
            string sTime;
            double time;
            string m_SepLine = "/";
            string m_SepParagraph = "\\";

            bool bParagraph = true; // true for the first line in order to avoid a linefeed

            ListViewItem lvi = new ListViewItem();
            for (int i = 0; i < lvLyrics.Items.Count; i++)
            {
                lvi = lvLyrics.Items[i];
                sTime = lvi.Text;
                time = Mp3LyricsMgmtHelper.TimeToMs(sTime);
                sLyric = "/" + lvi.SubItems[1].Text;                 //BUG !!!!!!!!!!!!!!!!


                // Use case : lyrics begins with a linefeed
                // like "/it's been a hard..."
                if (sLyric.Length > 1 && sLyric.StartsWith("/"))
                {
                    // If the previous line was not a paragraph, we add a linefeed
                    if (!bParagraph)
                        lstDgRows.Add((time, m_SepLine));
                    bParagraph = false;

                    // Lyric = lyric without the "/" character
                    sLyric = sLyric.Substring(1) + " ";
                    lstDgRows.Add((time, sLyric));
                }
                else if (sLyric == m_SepLine)
                {
                    // If lyric = "/", than we change it to paragraph
                    sLyric = m_SepParagraph;
                    lstDgRows.Add((time, sLyric));
                    bParagraph = true;
                }
            }
            #endregion Read data

            #region select filename

            string defExt = ".lrc";
            string fName = "New" + defExt;
            string fPath = Path.GetDirectoryName(Mp3FullPath);

            string fullName;
            string defName;

            #region search name

            if (fPath == null || fPath == "")
            {
                if (Directory.Exists(CreateNewMidiFile._DefaultDirectory))
                    fPath = CreateNewMidiFile._DefaultDirectory;
                else
                    fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }
            else
            {
                fName = Path.GetFileName(Mp3FullPath);
            }

            // Extension forced to lrc            
            string fullPath = fPath + "\\" + Path.GetFileNameWithoutExtension(fName) + defExt;
            fullName = Utilities.Files.FindUniqueFileName(fullPath);                            // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);                               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "LRC files (*.lrc)|*.lrc|All files (*.*)|*.*";

            saveFileDialog1.Title = "Save to LRC format";
            saveFileDialog1.Filter = defFilter;
            saveFileDialog1.DefaultExt = defExt;
            saveFileDialog1.InitialDirectory = @fPath;
            saveFileDialog1.FileName = defName;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            #endregion select filename

            #region save LRC
            string Tag_Tool = "Karaboss https://karaboss.lacharme.net";

            string Tag_Title = string.Empty;
            string Tag_Artist = string.Empty;
            string Tag_Album = string.Empty;
            string Tag_Lang = string.Empty;
            string Tag_By = string.Empty;
            string Tag_DPlus = string.Empty;

            fullPath = saveFileDialog1.FileName;

            if (Tag_Artist == "" && Tag_Title == "")
            {
                List<string> lstTags = Utilities.LyricsUtilities.GetTagsFromFileName(fullPath);
                Tag_Artist = lstTags[0];
                Tag_Title = lstTags[1];
            }

            bool bRemoveAccents = false;
            bool bUpperCase = false;
            bool bLowerCase = false;
            bool bRemoveNonAlphaNumeric = false;
            bool bCutLines = false;
            int LrcCutLinesChars = 32;

            Utilities.LyricsUtilities.SaveLRCLines(fullPath, lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus, bCutLines, LrcCutLinesChars, _LrcMillisecondsDigits, null);

            #endregion save lrc
        }


        /// <summary>
        /// Export lrc file without metadata
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExportLrcNoMeta_Click(object sender, EventArgs e)
        {
            SaveLRCFile();
        }
             

        private void frmMp3Player_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    //PlayPauseMusic();
                    break;

                case Keys.F12:
                    //bSequencerAlwaysOn = !bSequencerAlwaysOn;
                    // bForceShowSequencer was true, but user decided to hide the sequencer by clicking on the menu
                    //if (bSequencerAlwaysOn == false && bForceShowSequencer == true)
                    //    bForceShowSequencer = false;
                    //RedimIfSequencerVisible();
                    break;
            }
        }

        private void frmMp3Player_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    PlayPauseMusic();
                    break;

                case Keys.Left:
                    if (PlayerState == PlayerStates.Paused)
                        StopMusic();
                    break;

                case Keys.Enter:
                    if (PlayerAppearance == PlayerAppearances.LrcGenerator)
                    {
                        // Add a new timestamp
                        AddNewLrcTimeStamp();
                    }
                    break;

                case Keys.Add:
                case Keys.Subtract:
                case Keys.D6:
                case Keys.Decimal:
                    // Tempo +-
                    //KeyboardSelectTempo(e);
                    break;
            }
        }

        /// <summary>
        /// I am able to detect alpha-numeric keys. However i am not able to detect arrow keys
        /// ProcessCmdKey save my life
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
           
            if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped))
            {
                if (keyData == Keys.Left)
                {
                    StopMusic();
                    return true;
                }
            }
                       
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        /// <summary>
        ///  Switch to sync or edit mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwitchSyncEdit_Click(object sender, EventArgs e)
        {
            SetSyncEditMode();
        }

        private void SetSyncEditMode()
        {
            switch (LrcMode)
            {
                case LrcModes.Edit:
                    
                    if (lvLyrics.Items.Count == 0)
                    {
                        MessageBox.Show("Please load an LRC file or lyrics before", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                                        
                    // Swithc to sync mode
                    LrcMode = LrcModes.Sync;

                    // Passer en mode édition
                    btnSwitchSyncEdit.Text = Strings.SwitchToEditMode; // "Switch to edit mode"; 
                    // Mode synchro : lancer la musique et taper la touche ENTREE à chque fois que vous entendez une ligne des paroles affichée.
                    lblMode.Text = Strings.DescSyncMode; // "Sync mode: start the music and press ENTER each time you hear a line of lyrics displayed.";

                    for (int i = 0; i < lvLyrics.Items.Count; i++)
                    {                        
                        lvLyrics.Items[i].Text = "";
                    }

                    for (int i = 0; i < lvLyrics.Items.Count; i++)
                    {
                        lvLyrics.Items[i].Selected = false;
                    }

                    if (lvLyrics.Items.Count > 0)
                    {
                        lvLyrics.Items[0].Selected = true;
                        lvLyrics.Focus();
                    }

                    break;

                case LrcModes.Sync:
                    // Switch to edit mode
                    LrcMode = LrcModes.Edit;

                    // Passer en mode synchro
                    btnSwitchSyncEdit.Text = Strings.SwitchToSyncMode; // "Switch to sync mode";  
                    // Mode édition: chargez un fichier LRC à modifier ou des paroles à synchroniser
                    lblMode.Text = Strings.DescrEditMode; // "Edit mode: load an LRC file to be modified or lyrics to be synchronised";
                    
                    for (int i = 0; i < lstSaveItems.Count; i++)
                    {
                        lvLyrics.Items[i].Text = lstSaveItems[i];
                    }

                    for (int i = 0; i < lvLyrics.Items.Count; i++)
                    {
                        lvLyrics.Items[i].Selected = false;
                    }
                    break;
            }
        }

        private void lvLyrics_SubItemEndEditing(object sender, SubItemEndEditingEventArgs e)
        {
            
        }

        private void lvLyrics_SubItemClicked(object sender, SubItemEventArgs e)
        {
            if (LrcMode == LrcModes.Edit)
                lvLyrics.StartEditing(Editors[e.SubItem], e.Item, e.SubItem);
        }

        private void lvLyrics_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {           
            ListViewItem item;
            int idx = lvLyrics.GetSubItemAt(e.X, e.Y, out item);

            toolTip1.SetToolTip(lvLyrics, null);
        }






        #endregion LRC generator

       
    }
}
