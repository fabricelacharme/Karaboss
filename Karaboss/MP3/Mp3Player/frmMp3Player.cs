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
using TagLib.Id3v2;
using TagLib;
using FlShell.Interop;



namespace Karaboss.Mp3
{
    public partial class frmMp3Player : Form
    {
        #region declarations
     
        private List<(string, string)> lstSaveTimestamps = new List<(string, string)>();

        #region dgview

        private ContextMenuStrip dgContextMenu;

        #region dgView Colors

        Color dgViewHeaderBackColor = Color.FromArgb(43, 87, 151);
        Color dgViewHeaderForeColor = Color.White;
        Color dgViewSelectionBackColor = Color.FromArgb(45, 137, 239);

        Color SepLinesColor = Color.FromArgb(239, 244, 255);
        Color SepParagrColor = Color.LightGray;

        Font dgViewHeaderFont = new Font("Arial", 12F, FontStyle.Regular);
        Font dgViewCellsFont = new Font("Arial", 16F, GraphicsUnit.Pixel);

        #endregion dgViewColors

        int COL_MS = 0;
        int COL_TIME = 1;
        int COL_TEXT = 2;

        #endregion dgview

        string _lrcFileName;

        #region lrc generator

        int _index = 0;

        private enum LrcModes
        {
            Sync,
            Edit
        }
        private LrcModes LrcMode;

        #endregion lrc generator

        // txtResult, BtnFontPlus
        private Font _lyricseditfont;
        private float _fontSize = 8.25f;

        // Manage locally lyrics
        List<List<keffect.KaraokeEffect.kSyncText>> localSyncLyrics;

        private readonly string m_SepLine = "/";
        private readonly string m_SepParagraph = "\\";
       
        private int _LrcMillisecondsDigits = 2;

        private Mp3LyricsTypes Mp3LyricsType;
        public bool bfilemodified = false;

        // SlideShow directory
        public string dirSlideShow;


        #region player

        // Size player
        // 514;127
        private enum PlayerAppearances
        {
            Player,
            LrcGenerator,
        }
        private PlayerAppearances PlayerAppearance;

        // Dimensions        
        private readonly int SimpleMp3PlayerWidth = 530;
        private readonly int SimpleMp3PlayerHeight = 194;

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

        private int bouclestart = 0;
        private int laststart = 0;      // Start time to play        
        private double _totalSeconds;
        private int TransposeValue = 0;
        private long FrequencyRatio = 100;

        // Playlists
        private readonly Playlist currentPlaylist;
        private PlaylistItem currentPlaylistItem;
        private readonly string _InternalSepLines = "¼";

        #endregion player


        #region forms
        private bool scrolling = false;
        private bool closing = false;
        private bool loading = false;

        private string Mp3FullPath;
        private string Mp3FileName;
       
        //forms
        private frmMp3LyricsSimple frmMp3LyricsSimple;
        private frmMp3Lyrics frmMp3Lyrics;


        #endregion forms

       
        #region Bass

        private Mp3Player Player;

        #endregion Bass

        #endregion declarations


        public frmMp3Player(string FileName, Playlist myPlayList, bool bplay)
        {
            InitializeComponent();

            // Allow form keydown
            this.KeyPreview = true;

            Mp3FullPath = FileName;
            SetTitle(FileName);

            // Init controls
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
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        private void frmMp3Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bfilemodified == true && txtResult.Text.Length > 0)
            {
                //string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                String tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;

                DialogResult dr = MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    e.Cancel = true;

                    // Save LRC file
                    //SaveMp3Lyrics(Mp3FullPath);
                    GetLrcSaveOptions();

                    return;
                }                
            }

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

                // Adapt width of last column
                int W = dgView.RowHeadersWidth + 19;
                int WP = dgView.Parent.Width;
                for (int i = 0; i < dgView.Columns.Count - 1; i++)
                {
                    W += dgView.Columns[i].Width;
                }
                if (WP - W > 0)
                    dgView.Columns[dgView.Columns.Count - 1].Width = WP - W;
                            

                lblMode.Width = pnlTop.Width - lblMode.Left - 6;

                pnlSync.Left = 0;
                pnlEdit.Left = pnlSync.Left;

                pnlSync.Top = 62;
                pnlEdit.Top = pnlSync.Top;


                pnlSync.Width = pnlTop.Width;
                pnlEdit.Width = pnlSync.Width;
                

            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            // Adapt width of last column
            int W = dgView.RowHeadersWidth + 19;
            int WP = dgView.Parent.Width;
            for (int i = 0; i < dgView.Columns.Count - 1; i++)
            {
                W += dgView.Columns[i].Width;
            }
            if (WP - W > 0)
                dgView.Columns[dgView.Columns.Count - 1].Width = WP - W;
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
                    pnlLrc.Visible = false;


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
                    pnlLrc.Visible = true;

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
                    
                    // Poluplate gridview and textbox
                    PopulateDataGridView();
                    localSyncLyrics = GetUniqueSource();
                    PopulateTextBox(localSyncLyrics);

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
                
                
                _index = 0;  // for timestamps entering
                lstSaveTimestamps.Clear();       // Clear timestamps 


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

            _index = 0;

            // Volume to 0
            VuPeakVolumeLeft.Level = 0;
            VuPeakVolumeRight.Level = 0;

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
        /// Open or close LRC Generator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>       
        private void mnuEditLyrics_Click(object sender, EventArgs e)
        {
            OpenCloseLrcGenerator();           
        }


        private void OpenCloseLrcGenerator()
        {
            mnuEditLyrics.Checked = !mnuEditLyrics.Checked;

            // If LRC Generator visible
            if (mnuEditLyrics.Checked)
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
            OpenCloseLrcGenerator();

            /*
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
            */
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

            //if (Tag == null) return;

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

            btnSwitchSyncEdit.Text = Strings.SwitchToSyncMode;
            lblMode.Text = Strings.DescrEditMode; // "Edit mode: load an LRC file to be modified or lyrics to be synchronised";

            #region dgview

            InitGridView();

            #endregion dgview

        }


        /// <summary>
        /// Initialize gridview
        /// </summary>
        private void InitGridView()
        {
            dgView.Rows.Clear();
            dgView.Refresh();

            // Header color
            dgView.ColumnHeadersDefaultCellStyle.BackColor = dgViewHeaderBackColor;
            dgView.ColumnHeadersDefaultCellStyle.ForeColor = dgViewHeaderForeColor;

            dgView.ColumnHeadersDefaultCellStyle.Font = dgViewHeaderFont;
            dgView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Header Column width (with rows numbers)
            dgView.RowHeadersWidth = 60;

            dgView.RowsAdded += new DataGridViewRowsAddedEventHandler(dgView_RowsAdded);
            dgView.RowsRemoved += new DataGridViewRowsRemovedEventHandler(dgView_RowsRemoved);

            // Selection
            dgView.DefaultCellStyle.SelectionBackColor = dgViewSelectionBackColor;

            dgView.EnableHeadersVisualStyles = false;

            // Chords edition
            dgView.ColumnCount = 3;

            

            dgView.Columns[COL_MS].Name = "dMs";
            dgView.Columns[COL_MS].HeaderText = "Ms";
            dgView.Columns[COL_MS].ToolTipText = "Milliseconds";            
            dgView.Columns[COL_MS].Width = 70;
            dgView.Columns[COL_MS].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgView.Columns[COL_TIME].Name = "dTime";
            dgView.Columns[COL_TIME].HeaderText = "Timestamp";
            dgView.Columns[COL_TIME].ToolTipText = "Timestamp";
            dgView.Columns[COL_TIME].Width = 90;
            dgView.Columns[COL_TIME].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgView.Columns[COL_TEXT].Name = "dText";
            dgView.Columns[COL_TEXT].HeaderText = "Text";
            dgView.Columns[COL_TEXT].ToolTipText = "Text";
            dgView.Columns[COL_TEXT].Width = 200;

            //Change cell font
            foreach (DataGridViewColumn c in dgView.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;                     // header not sortable
                c.DefaultCellStyle.Font = dgViewCellsFont;
                c.ReadOnly = false;
            }
            
            
            ResizeMe();

            lblLyrics.Text = "0";
            lblTimes.Text = lblLyrics.Text;
        }


        private void ResizeMe()
        {
            // Adapt width of last column
            int W = dgView.RowHeadersWidth + 19;
            int WP = dgView.Parent.Width;
            for (int i = 0; i < dgView.Columns.Count - 1; i++)
            {
                W += dgView.Columns[i].Width;
            }
            if (WP - W > 0)
                dgView.Columns[dgView.Columns.Count - 1].Width = WP - W;

           
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
      
        private void InitLrcGenerator()
        {
            if (PlayerAppearance != PlayerAppearances.LrcGenerator) return;

            LrcMode = LrcModes.Edit;

            _lyricseditfont = Properties.Settings.Default.LyricsEditFont;
            _fontSize = _lyricseditfont.Size;
            txtResult.Font = _lyricseditfont;

            // index for times entering
            _index = 0;

            // 2 or 3 digits for timestamp format 00:00.00 or 00:00.000
            _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;

            pnlEdit.Visible = true;

            lblHotkeys.Font = new Font("Courier New", 9);
            lblHotkeys.Text = "<ENTER>" + " " + "Add a new timestamp" +"\r\n" + "<SPACE>" + " "+ "Pause Music" + "\r\n" + "<-" + "      " + "Stop Music";

            if (Player.Tag != null)
            {
                txtTitle.Text = Player.Tag.Title;
                txtAlbum.Text = Player.Tag.Album;
                if (Player.Tag.Performers.Count() > 0)
                {
                    txtArtist.Text = Player.Tag.Performers[0].ToString();
                    txtAuthor.Text = Player.Tag.Performers[0].ToString();
                }
            }
            
        }

        /// <summary>
        /// Key Enter was hit
        /// </summary>
        private void AddNewLrcTimeStamp() 
        {
            if (PlayerState != PlayerStates.Playing) return;

            if (dgView.Rows.Count == 1)
            {
                MessageBox.Show("Please add lyrics before entering timestamps", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }                        
            
            string tsp;            
            double time = Player.Position * 1000;
            tsp = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);
                                 
            if (_index < dgView.Rows.Count) // lvLyrics.Items.Count)
            {
                dgView.Rows[_index].Cells[COL_MS].Value = (long)time;
                dgView.Rows[_index].Cells[COL_TIME].Value = tsp;
                //lvLyrics.Items[_index].Text = tsp;

                
                // Scroll to 2 lines before bottom to ensure visibility
                if (_index < dgView.Rows.Count - 2)
                    dgView.CurrentCell = dgView.Rows[_index + 2].Cells[0];
                else
                    dgView.CurrentCell = dgView.Rows[_index].Cells[0];
               

                lblTimes.Text = (_index + 1).ToString(); // count number of lines done

                // Select line (paint in blue) and scroll
                dgView.Rows[_index].Selected = true;
                dgView.CurrentCell = dgView.Rows[_index].Cells[0];
            

                _index++;
            }            
        }


        #region load lrc

        /// <summary>
        /// Import an LRC file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuImportLrcFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog1.Title = "Open a .lrc file";
            OpenFileDialog1.DefaultExt = "lrc";
            OpenFileDialog1.Filter = "Lrc files|*.lrc|All files|*.*";            
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _lrcFileName = OpenFileDialog1.FileName;

                LoadLRCFile(_lrcFileName);

                // Update counters
                //lblLyrics.Text = lvLyrics.Items.Count.ToString();
                lblTimes.Text = "0";

                // Select first row
                dgView.Rows[0].Selected = true;

                localSyncLyrics = GetCurrentDgViewContent();
                PopulateTextBox(localSyncLyrics);
               
            }
        }


        /// <summary>
        /// Load a LRC file (timestamps + lyrics)
        /// </summary>
        /// <param name="Source"></param>
        private void LoadLRCFile(string FileName)
        {
            long time;
            string sTime;
            string text;

            Cursor.Current = Cursors.WaitCursor;

            Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectLrcLyrics(FileName);
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;

            InitGridView();

            // For each line
            for (int j = 0; j < SyncLyrics.Count; j++)
            {
                // For each syllabes
                for (int i = 0; i < SyncLyrics[j].Count; i++)
                {
                    time = SyncLyrics[j][i].Time;
                    sTime = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);
                    text = SyncLyrics[j][i].Text;

                    // Put "/" everywhere
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    dgView.Rows.Add(time, sTime, text);
                }
            }

            Cursor.Current = Cursors.Default;
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
            OpenFileDialog1.Filter = "Text files|*.txt|All files|*.*";
            OpenFileDialog1.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _lrcFileName = OpenFileDialog1.FileName;

                // Reset
                InitGridView();

                string[] lines = System.IO.File.ReadAllLines(_lrcFileName);
                string line;                

                for (int i = 0; i < lines.Count(); i++)
                {
                    line = lines[i].Trim();
                    if (line != "")
                    {
                        line = line.Replace(" ", "_");
                        line = m_SepLine + line;
                        dgView.Rows.Add("", "", line);
                    }
                }

                // Update counters
                //lblLyrics.Text = lvLyrics.Items.Count.ToString();
                lblTimes.Text = "0";

                // Select first row
                dgView.Rows[0].Selected = true;

                localSyncLyrics = GetCurrentDgViewContent();
                PopulateTextBox(localSyncLyrics);


                if (MessageBox.Show(Strings.SwitchToSyncMode + "?", "Karaboss", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SetSyncEditMode();
                }
            }
        }

        /// <summary>
        /// Store dgView content
        /// </summary>
        /// <returns></returns>
        private List<List<keffect.KaraokeEffect.kSyncText>> GetCurrentDgViewContent()
        {
            object otext;
            object otime;

            string lyric;
            long time;
            string sTime;
            keffect.KaraokeEffect.kSyncText sct;
            bool bNewLine = false;
            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();


            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                otext = dgView.Rows[i].Cells[COL_TEXT].Value;
                otime = dgView.Rows[i].Cells[COL_MS].Value;

                if (otext == null) continue;

                lyric = otext.ToString();
                sTime = otime.ToString();
                if (IsNumeric(sTime))
                    time = long.Parse(sTime);
                else
                    time = 0;

                bNewLine = false;

                if (lyric.Trim() != "")
                {

                    lyric = lyric.Replace("_", " ");
                    // Search for new lines
                    if (lyric.StartsWith(m_SepLine))
                    {
                        lyric = lyric.Substring(1);
                        bNewLine = true;
                    }

                    if (lyric.EndsWith(m_SepLine))
                    {
                        lyric = lyric.Substring(0, lyric.Length - 1);
                        bNewLine = true;
                    }

                    sct = new keffect.KaraokeEffect.kSyncText(time, lyric);
                    if (bNewLine)
                    {
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(sct);
                    }
                    else
                    {
                        SyncLine.Add(sct);
                    }
                }
            }

            // Store last line
            if (SyncLine.Count > 0)
                SyncLyrics.Add(SyncLine);

            return SyncLyrics;
        }

        #endregion load lrc


        #region save lrc

        /// <summary>
        /// Save as lrc file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void mnuExportAsLrc_Click(object sender, EventArgs e)
        {
            GetLrcSaveOptions();
        }



        /// <summary>
        /// Get save lrc options
        /// </summary>
        /// <param name="LrcExportFormat"></param>
        private void GetLrcSaveOptions()
        {
            #region guard
            object otime;
            object otsp;            
            
            if (txtResult.Text.Length == 0)
            {
                MessageBox.Show("Nothing to save", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bfilemodified = false;
                return;
            }

            for (int i = 0; i < dgView.Rows.Count - 1; i++)
            {
                otime = dgView.Rows[i].Cells[COL_MS].Value;
                otsp = dgView.Rows[i].Cells[COL_TIME].Value;
                if (otime == null || otsp == null)
                {
                    MessageBox.Show("Empty timestamp at line " + i + 1, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if ((otime != null && otime.ToString() == "") || (otsp != null && otsp.ToString() == ""))
                {
                    MessageBox.Show("Empty timestamp at line " + i + 1, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            #endregion guard


            DialogResult dr;
            frmLrcOptions LrcOptionsDialog = new frmLrcOptions();
            dr = LrcOptionsDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            // Remove accents
            bool bRemoveAccents = LrcOptionsDialog.bRemoveAccents;
            // Force Upper Case
            bool bUpperCase = LrcOptionsDialog.bUpperCase;
            // Force Lower Case
            bool bLowerCase = LrcOptionsDialog.bLowerCase;
            // Remove all non-alphanumeric characters
            bool bRemoveNonAlphaNumeric = LrcOptionsDialog.bRemoveNonAlphaNumeric;
            // Save to line or to syllabes
            LrcLinesSyllabesFormats LrcLinesSyllabesFormat = LrcOptionsDialog.LrcLinesSyllabesFormat;

            _LrcMillisecondsDigits = LrcOptionsDialog.LrcMillisecondsDigits;

            // Cut lines over x characters
            bool bCutLines = LrcOptionsDialog.bCutLines;
            int LrcCutLinesChars = LrcOptionsDialog.LrcCutLinesChars;

            bool bWithMetadata = LrcOptionsDialog.bSaveMetadata;

            SaveLrcFileName(bWithMetadata, LrcLinesSyllabesFormat, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, bCutLines, LrcCutLinesChars);
        }


        /// <summary>
        /// Select file to save and format tags
        /// </summary>
        /// <param name="LrcExportFormat"></param>
        /// <param name="LrcLinesSyllabesFormat"></param>
        /// <param name="bRemoveAccents"></param>
        /// <param name="bUpperCase"></param>
        /// <param name="bLowerCase"></param>
        /// <param name="bRemoveNonAlphaNumeric"></param>
        /// <param name="bCutLines"></param>
        /// <param name="LrcCutLinesChars"></param>
        private void SaveLrcFileName(bool bWithMetadata, LrcLinesSyllabesFormats LrcLinesSyllabesFormat, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, bool bCutLines, int LrcCutLinesChars)
        {
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

            #endregion

            fullPath = saveFileDialog1.FileName;

            #region metadata
            string Tag_Tool = string.Empty;
            string Tag_Title = string.Empty;
            string Tag_Artist = string.Empty;
            string Tag_Album = string.Empty;
            string Tag_Lang = string.Empty;
            string Tag_By = string.Empty;
            string Tag_DPlus = string.Empty;            
          
            if (bWithMetadata)
            {
                Tag_Tool = "Karaboss https://karaboss.lacharme.net";
                Tag_Title = txtTitle.Text;
                Tag_Artist = txtArtist.Text;
                Tag_Album = txtAlbum.Text;
                Tag_Lang = cbLanguage.Text;
                Tag_By = txtAuthor.Text;
                Tag_DPlus = string.Empty;                

                if (Tag_Artist == "" && Tag_Title == "")
                {
                    List<string> lstTags = Utilities.LyricsUtilities.GetTagsFromFileName(fullPath);
                    Tag_Artist = lstTags[0];
                    Tag_Title = lstTags[1];
                }
            }
            #endregion metadata


            switch (LrcLinesSyllabesFormat)
            {
                case LrcLinesSyllabesFormats.Lines:
                    SaveLRCLines(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus, bCutLines, LrcCutLinesChars);
                    break;
                case LrcLinesSyllabesFormats.Syllabes:
                    SaveLRCSyllabes(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus);
                    break;
            }
        }

        /// <summary>
        /// Save Lyrics .lrc file format and by lines
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Tag_Title"></param>
        /// <param name="Tag_Artist"></param>
        /// <param name="Tag_Album"></param>
        /// <param name="Tag_Lang"></param>
        /// <param name="Tag_By"></param>
        /// <param name="Tag_DPlus"></param>
        private void SaveLRCLines(string File, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus, bool bControlLength, int MaxLength)
        {
            string sLine;
            string lrcs;
            string cr = "\r\n";

            #region meta data

            // List to store lines
            List<string> lstHeaderLines = new List<string>();

            // Store meta datas
            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Album, Tag_DPlus };
            List<string> TagsNames = new List<string> { "Tool:", "Ti:", "Ar:", "Al:", "La:", "By:", "D+:" };
            string Tag;
            string TagName;
            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                {
                    sLine = "[" + TagName + Tag + "]";
                    lstHeaderLines.Add(sLine);
                }
            }
            #endregion meta data

            // Store rows of dgView in a list
            // the aim is to have the same procedure between frmLyricsEdit and frmMp3LyricsEdit

            #region Read dgView

            // the aim is to have the same procedure between frmLyricsEdit and frmMp3LyricsEdit
            List<(double, string)> lstDgRows =ReadDataFromDgView();
           
            #endregion Read dgView


            // Make treatment of lyrics (same for frmLyricsEdit and frmMp3LyricsEdit)
            List<string> lstLyricsItems = Utilities.LyricsUtilities.LrcExtractDgRows(lstDgRows, _LrcMillisecondsDigits, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, null);

            // Store lyrics in lines (remove timestamps from lines, except for the first word)
            // [00:04.59]It's_been_a_hard_day's_night
            List<string> lstLines = Utilities.LyricsUtilities.GetLrcLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store timestamps + lyrics in lines (add spaces if not existing)
            // initial [00:04.59]It's[00:04.83]_been[00:05.05]_a[00:05.27]_hard[00:06.15]_day's[00:06.81]_night[00:08.14]
            // result [00:04.59]It's [00:04.83]_been [00:05.05]_a [00:05.27]_hard [00:06.15]_day's [00:06.81]_night [00:08.14]
            List<string> lstTimeLines = Utilities.LyricsUtilities.GetLrcTimeLines(lstLyricsItems, _LrcMillisecondsDigits);

            // Store lyrics by line and cut lines to MaxLength characters using lstTimeLines
            List<string> lstLinesCut = new List<string>();
            if (bControlLength)
            {
                lstLinesCut = Utilities.LyricsUtilities.GetLrcLinesCut(lstTimeLines, MaxLength, _LrcMillisecondsDigits);
            }


            #region send all to string 
            // Header
            lrcs = string.Empty;
            for (int i = 0; i < lstHeaderLines.Count; i++)
            {
                lrcs += lstHeaderLines[i] + cr;
            }

            // Lines
            if (bControlLength)
            {
                for (int i = 0; i < lstLinesCut.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"
                    lrcs += lstLinesCut[i].Replace("_", " ") + cr;
                }
            }
            else
            {
                for (int i = 0; i < lstLines.Count; i++)
                {
                    // Replace underscores located in the middle of the lyrics
                    // ex: " the_air,_(get_to_poppin')"
                    lrcs += lstLines[i].Replace("_", " ") + cr;
                }
            }
            #endregion send all to string


            #region open file

            try
            {
                System.IO.File.WriteAllText(File, lrcs);
                System.Diagnostics.Process.Start(@File);
                
                // Reset title
                bfilemodified = false;
                SetTitle(Mp3FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            #endregion open file
        }

        /// <summary>
        /// Save lyrics to new LRC format [01:54.60]Pa<01:55.32>ro<01:56.15>les
        /// </summary>
        /// <param name="FileName"></param>
        private void SaveLRCSyllabes(string File, bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric, string Tag_Tool, string Tag_Title, string Tag_Artist, string Tag_Album, string Tag_Lang, string Tag_By, string Tag_DPlus)
        {
            string sTime;
            long time;
            TimeSpan ts;
            string tsp;

            string sType;
            string sLyric;

            object vLyric;
            object vTime;
            object vType;

            string lrcs = string.Empty;
            string cr = "\r\n";
            string strSpaceBetween;
            bool bSpaceBetwwen = false;
            string lines = string.Empty;

            // Space between time and lyrics [00:02.872]lyric
            if (bSpaceBetwwen)
                strSpaceBetween = " ";
            else
                strSpaceBetween = string.Empty;

            List<string> TagsList = new List<string> { Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_DPlus };
            List<string> TagsNames = new List<string> { "Tool:", "Ti:", "Ar:", "Al:", "La:", "By:", "D+:" };
            string Tag;
            string TagName;
            for (int i = 0; i < TagsList.Count; i++)
            {
                Tag = TagsList[i];
                TagName = TagsNames[i];
                Tag = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(Tag) : Tag;
                Tag = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(Tag) : Tag;
                if (Tag != "")
                    lrcs += "[" + TagName + strSpaceBetween + Tag + "]" + cr;
            }

            bool bLineFeed = true;

            // new format of lrc
            // [01:54.60]La <01:55.32>petite <01:56.15>maison
            // Start line is [01:54.60]La
            // syllabes are <01:55.32>petite <01:56.15>maison

            // separate words and syllabes
            // Store results in a list
            List<(string stime, string lyric)> results = new List<(string, string)>();

            // Apply treatments choosen
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                vLyric = dgView.Rows[i].Cells[COL_TEXT].Value;
                vTime = dgView.Rows[i].Cells[COL_MS].Value;
                vType = dgView.Rows[i].Cells[COL_TEXT].Value;

                if (vTime != null && vLyric != null && vTime.ToString() != "" && vLyric.ToString() != "")
                {

                    sLyric = vLyric.ToString();
                    sLyric = sLyric.Replace("_", " ");

                    sTime = vTime.ToString();
                    time = long.Parse(sTime);


                    sTime = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);

                    // /hey
                    // jude
                    sType = vType.ToString().Trim();
                    if (sType.IndexOf(m_SepParagraph) != -1 || sType.IndexOf(m_SepLine) != -1)
                        bLineFeed = true;


                    sLyric = sLyric.Replace(m_SepParagraph, "");
                    sLyric = sLyric.Replace(m_SepLine, "");


                    // Remove accents
                    sLyric = bRemoveAccents ? Utilities.LyricsUtilities.RemoveDiacritics(sLyric) : sLyric;

                    //Uppercase letters
                    sLyric = bUpperCase ? sLyric.ToUpper() : sLyric;

                    // Lowercase letters
                    sLyric = bLowerCase ? sLyric.ToLower() : sLyric;

                    // Remove non-alphanumeric chars
                    sLyric = bRemoveNonAlphaNumeric ? Utilities.LyricsUtilities.RemoveNonAlphaNumeric(sLyric) : sLyric;

                    // Save also empty lyrics
                    //sTime = vTime.ToString();

                    if (bLineFeed)
                    {
                        // Format of timestamp is []                                                        
                        results.Add(("[" + sTime + "]", sLyric));

                    }
                    else
                    {
                        // Format of timestamp is <> + space before
                        results.Add(("<" + sTime + ">", sLyric));
                    }

                    bLineFeed = false;

                }
            }

            string nextLyric = string.Empty;
            string nextTime = string.Empty;

            bool bKeepForNextSyllabe = false;
            string keepLyric = string.Empty;
            string keepTime = string.Empty;


            // Add a trailing "-" to syllabes without space with the next syllabe (ie it is a word composed of several syllabes)
            for (int i = 0; i < results.Count; i++)
            {
                sTime = results[i].stime;
                sLyric = results[i].lyric;

                if (i < results.Count - 1)
                {
                    nextLyric = results[i + 1].lyric;
                    nextTime = results[i + 1].stime;
                }
                else
                {
                    nextLyric = "";
                    nextTime = "";
                }
                // No trailing space in the current, no starting space in the next and the next is not a new line ([]) 
                // => this syllabe must be merged with the next one
                if (!sLyric.EndsWith(" ") && nextLyric.Length > 0 && !nextLyric.StartsWith(" ") && nextTime.IndexOf("[") == -1)
                {
                    results[i] = (results[i].stime, results[i].lyric + "-");
                }
            }

            for (int i = 0; i < results.Count; i++)
            {
                sTime = results[i].stime;
                sLyric = results[i].lyric;

                // Keep all syllabes ending with a trailing "-" until a syllabe without a "-"
                if (sLyric.EndsWith("-"))
                {
                    sLyric = sLyric.Substring(0, sLyric.Length - 1).Trim();  // remove the "-"
                    keepLyric += sLyric;                                     // add syllabe to previous ones   
                    if (keepTime == "")
                        keepTime = sTime;                                    // keep only the first timestamp (beginning of the word)   

                    if (sTime.IndexOf("[") > -1)                             // if new line, store previous one
                    {
                        // Store previous line 
                        if (lrcs.Trim().Length > 0)
                        {
                            lines += lrcs + cr;
                        }
                        lrcs = "";
                    }

                    // Skip 
                    continue;
                }
                else if (keepLyric != "")
                {
                    // no trailing "-" and there are syllabes into keeplyric => this is the last syllabe of a word
                    bKeepForNextSyllabe = true;
                }

                // This is the start of a new line
                if (sTime.IndexOf("[") > -1)
                {
                    // Store previous line 
                    if (lrcs.Trim().Length > 0)
                    {
                        lines += lrcs + cr;
                    }
                    // Format of timestamp is [] 
                    lrcs = sTime + sLyric.Trim();
                }
                else
                {
                    // This is a normal syllabe 
                    if (!bKeepForNextSyllabe)
                    {
                        // Format of timestamp is <> + space before
                        lrcs += " " + sTime + sLyric.Trim();
                    }
                    else
                    {
                        // this is The last syllabe of a word
                        if (keepTime.IndexOf("[") > -1)
                        {
                            // if the word stored in keeplyric was a starting line []
                            lrcs += keepTime + keepLyric + sLyric.Trim();
                        }
                        else
                        {
                            // if the word stored in keeplyric was a normal word, add a space before the <00:00.000>  
                            lrcs += " " + keepTime + keepLyric + sLyric.Trim();
                        }

                        // Reset variables used to store syllabes of a word
                        bKeepForNextSyllabe = false;
                        keepLyric = "";
                        keepTime = "";
                    }
                }
            }

            if (lrcs.Trim().Length > 0)
                lines += lrcs + cr;

            try
            {
                System.IO.File.WriteAllText(File, lines);
                System.Diagnostics.Process.Start(@File);
                
                // Reset title
                bfilemodified = false;
                SetTitle(Mp3FullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// REad data form dgView to be saved to LRC file
        /// </summary>
        /// <returns></returns>
        List<(double, string)> ReadDataFromDgView()
        {
            object otext;
            object otime;            

            List<(double, string)> lstDgRows = new List<(double, string)>();

            string sLyric;
            double time;
            string m_SepLine = "/";
            string m_SepParagraph = "\\";
                       

            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                otext = dgView.Rows[i].Cells[COL_TEXT].Value;
                otime = dgView.Rows[i].Cells[COL_MS].Value;

                if (otext == null || otime == null) continue;

                time = double.Parse(otime.ToString());
                sLyric = otext.ToString();

                // Objective : separate linefeeds & pagragraphs from text in order to retrieve
                // the same behaviour as midi edition and use the same code

                if (sLyric.IndexOf(m_SepParagraph) != -1)
                {
                    // add a new line with a paragraph (except first line)
                    if (i > 0)
                        lstDgRows.Add((time, m_SepParagraph));
                    
                    sLyric = sLyric.Replace(m_SepParagraph, "");
                }
                else if (sLyric.IndexOf(m_SepLine) != -1)
                {
                    // Add a new line with a linefeed (except first line)
                    if (i > 0)
                    lstDgRows.Add((time, m_SepLine));
                    
                    sLyric = sLyric.Replace(m_SepLine, "");
                }

                lstDgRows.Add((time, sLyric));

            }
            return lstDgRows;
        }

      
        #endregion save lrc

   
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
                    if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)                       
                        PlayPauseMusic();
                    break;

                case Keys.Left:
                    if (PlayerState == PlayerStates.Paused)
                        StopMusic();
                    break;

                case Keys.Enter:
                    if (PlayerAppearance == PlayerAppearances.LrcGenerator && PlayerState == PlayerStates.Playing)
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
            if ((PlayerState == PlayerStates.Paused))
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

        /// <summary>
        /// Display things according to mode sync or edition
        /// </summary>
        private void SetSyncEditMode()
        {
            switch (LrcMode)
            {
                case LrcModes.Edit:                                        
                    if (dgView.Rows.Count == 1)
                    {
                        MessageBox.Show("Please load an LRC file or lyrics before", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                                        
                    // Swithc to sync mode
                    LrcMode = LrcModes.Sync;

                    pnlSync.Visible = true;
                    pnlEdit.Visible = false;

                    // Passer en mode édition
                    btnSwitchSyncEdit.Text = Strings.SwitchToEditMode; // "Switch to edit mode"; 

                    // Mode synchro : lancer la musique et taper la touche ENTREE à chque fois que vous entendez une ligne des paroles affichée.
                    lblMode.Text = Strings.DescSyncMode; // "Sync mode: start the music and press ENTER each time you hear a line of lyrics displayed.";                   

                    lstSaveTimestamps.Clear();
                    object otime;
                    object otsp;
                    for (int i = 0; i < dgView.Rows.Count; i++)
                    {
                        otime = dgView.Rows[i].Cells[COL_MS].Value;
                        otsp = dgView.Rows[i].Cells[COL_TIME].Value;
                        if (otime == null || otsp == null)
                            lstSaveTimestamps.Add(("", ""));
                        else
                            lstSaveTimestamps.Add((otime.ToString(), otsp.ToString()));

                        dgView.Rows[i].Cells[COL_MS].Value = "";
                        dgView.Rows[i].Cells[COL_TIME].Value = ""   ;
                    }

                    break;

                case LrcModes.Sync:
                    
                    // Switch to edit mode
                    LrcMode = LrcModes.Edit;

                    pnlEdit.Visible = true;
                    pnlSync.Visible = false;


                    // Passer en mode synchro
                    btnSwitchSyncEdit.Text = Strings.SwitchToSyncMode; // "Switch to sync mode";  

                    // Mode édition: chargez un fichier LRC à modifier ou des paroles à synchroniser
                    lblMode.Text = Strings.DescrEditMode; // "Edit mode: load an LRC file to be modified or lyrics to be synchronised";
                    
                    // Restore values
                    for (int i = 0; i < lstSaveTimestamps.Count; i++)
                    {
                        dgView.Rows[i].Cells[COL_MS].Value = lstSaveTimestamps[i].Item1;
                        dgView.Rows[i].Cells[COL_TIME].Value = lstSaveTimestamps[i].Item2;                        
                    }

                    
                    break;
            }
        }

       

        #endregion LRC generator


        #region populate dgView

        /// <summary>
        /// Returns list of lyrics according to its origin: mp3 or mrc
        /// </summary>
        /// <returns></returns>
        private List<List<keffect.KaraokeEffect.kSyncText>> GetUniqueSource()
        {
            // Origin = synchronized lyrics frame            
            if (Mp3LyricsMgmtHelper.MySyncLyricsFrame != null)
            {
                return Mp3LyricsMgmtHelper.GetKEffectSyncLyrics(Mp3LyricsMgmtHelper.MySyncLyricsFrame);
            }
            else if (Mp3LyricsMgmtHelper.SyncLyrics != null)
            {
                return Mp3LyricsMgmtHelper.SyncLyrics;
            }

            return null;
        }

        /// <summary>
        /// Populate gridview with lyrics
        /// </summary>              
        private void PopulateDataGridView2(List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics)
        {
            long time;
            string sTime;
            string text;

            // For each line
            for (int j = 0; j < SyncLyrics.Count; j++)
            {
                // For each syllabes
                for (int i = 0; i < SyncLyrics[j].Count; i++)
                {
                    time = SyncLyrics[j][i].Time;
                    sTime = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);

                    text = SyncLyrics[j][i].Text;

                    // Put "/" everywhere
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    //if (!text.StartsWith(m_SepLine)) text = m_SepLine + text;

                    dgView.Rows.Add(time, sTime, text);
                }
            }

            lblLyrics.Text = (dgView.Rows.Count - 1).ToString();
            lblTimes.Text = lblLyrics.Text;
        }

        
        private void PopulateDataGridView()
        {
            long time;
            string sTime;
            string text;

            // Origine = lrc            
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;

            // Origin = synchronized lyrics frame
            SynchronisedLyricsFrame SynchedLyrics = Mp3LyricsMgmtHelper.MySyncLyricsFrame;

            if (SynchedLyrics != null)
            {
                for (int i = 0; i < SynchedLyrics.Text.Count(); i++)
                {
                    // Put "/" everywhere
                    text = SynchedLyrics.Text[i].Text;
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    time = SynchedLyrics.Text[i].Time;
                    sTime = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);

                    dgView.Rows.Add(time, sTime, text);
                }
            }
            else if (SyncLyrics != null)
            {

                // For each line
                for (int j = 0; j < SyncLyrics.Count; j++)
                {
                    // For each syllabes
                    for (int i = 0; i < SyncLyrics[j].Count; i++)
                    {
                        time = SyncLyrics[j][i].Time;
                        sTime = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);

                        text = SyncLyrics[j][i].Text;

                        // Put "/" everywhere
                        text = text.Replace("\r\n", m_SepLine);
                        text = text.Replace("\r", m_SepLine);
                        text = text.Replace("\n", m_SepLine);
                        text = text.Replace(" ", "_");

                        dgView.Rows.Add(time, sTime, text);
                    }
                }
            }

            lblLyrics.Text = (dgView.Rows.Count - 1).ToString();
            lblTimes.Text = lblLyrics.Text;

        }
               


        private void dgView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NumberRows();
        }

        private void dgView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            NumberRows();
        }

        private void NumberRows()
        {
            foreach (DataGridViewRow r in dgView.Rows)
            {
                dgView.Rows[r.Index].HeaderCell.Value =
                                    (r.Index + 1).ToString();
            }
        }



        #endregion dgView


        #region Text

        /// <summary>
        /// Show line of texbox currently edited
        /// </summary>
        private void ShowCurrentLine()
        {
            int line = dgView.CurrentCell.RowIndex;

            // Text before current
            string tx = string.Empty;
            string s;

            for (int row = 0; row <= line; row++)
            {
                s = string.Empty;

                if (dgView.Rows[row].Cells[COL_TEXT].Value != null && dgView.Rows[row].Cells[COL_TEXT].Value.ToString() != "")
                {
                    s = dgView.Rows[row].Cells[COL_TEXT].Value.ToString();

                    if (row == 0)
                    {
                        if (s.StartsWith(m_SepLine))
                            s = s.Replace(m_SepLine, "");   // J'me comprends
                    }

                    s = s.Replace(m_SepParagraph, "\n\n");
                    s = s.Replace(m_SepLine, "\n");
                }

                s = s.Replace("_", " ");
                tx += s;
            }

            if (tx != "")
            {
                //Check if line is visible

                //get the first visible char index
                int firstVisibleChar = txtResult.GetCharIndexFromPosition(new Point(0, 0));
                //get the line index from the char index
                int firstVisibleLine = txtResult.GetLineFromCharIndex(firstVisibleChar);
                

                int lastVisibleCharIndex = txtResult.GetCharIndexFromPosition(new Point(0, txtResult.Height));
                int lastVisibleLine = txtResult.GetLineFromCharIndex(lastVisibleCharIndex);
                


                int start = txtResult.Text.IndexOf(tx);
                if (start == 0)
                {                   
                    int L = tx.Length;
                    txtResult.SelectionColor = txtResult.ForeColor;
                    txtResult.SelectionStart = 0;
                    txtResult.SelectionLength = L;                    
                    txtResult.SelectionColor = Color.White;

                    // If line is before the first visible line
                    // If line is after the last visible line
                    if (line < firstVisibleLine || line > lastVisibleLine)
                    {
                        txtResult.SelectedText = tx;
                        txtResult.ScrollToCaret();

                        txtResult.SelectionColor = txtResult.ForeColor;
                        txtResult.SelectionStart = 0;
                        txtResult.SelectionLength = L;
                        txtResult.SelectionColor = Color.White;
                    }
                }
            }
        }


        /// <summary>
        /// Display text into the rich textbox
        /// </summary>
        /// <param name="lLyrics"></param>
        private void PopulateTextBox(List<List<keffect.KaraokeEffect.kSyncText>> lSyncLyrics)
        {
            string line = string.Empty;
            string tx = string.Empty;
            string cr = "\r\n";
            string Element;

            // For each line
            for (int j = 0; j < lSyncLyrics.Count; j++)
            {
                line = string.Empty;

                // For each item of a line
                for (int i = 0; i < lSyncLyrics[j].Count; i++)
                {
                    Element = lSyncLyrics[j][i].Text;
                    
                    Element = Element.Replace(Environment.NewLine, "");
                    
                    line += Element;
                }
                tx += line + cr;
            }

            txtResult.Text = tx;

            txtResult.SelectAll();
            txtResult.SelectionAlignment = HorizontalAlignment.Center;
        }

        /// <summary>
        /// Store gridview into 
        /// </summary>
        /// <returns></returns>
        private List<List<keffect.KaraokeEffect.kSyncText>> LoadModifiedLyrics()
        {
            int line;
            if (!CheckTimes(out line))
            {
                MessageBox.Show("Time on line " + line + " is incorrect", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    dgView.CurrentCell = dgView.Rows[line - 1].Cells[COL_MS];
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                return null;
            }

            long time;
            string text;
            string cr = "\r\n";
            int iParagraph;
            int iLineFeed;


            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            List<List<keffect.KaraokeEffect.kSyncText>> lSyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();
            keffect.KaraokeEffect.kSyncText kst;

            for (int i = 0; i < dgView.RowCount; i++)
            {
                if (dgView.Rows[i].Cells[COL_MS].Value == null
                    || dgView.Rows[i].Cells[COL_TEXT].Value == null
                    || !IsNumeric(dgView.Rows[i].Cells[COL_MS].Value.ToString())) continue;

                time = Convert.ToInt32(dgView.Rows[i].Cells[COL_MS].Value);
                text = dgView.Rows[i].Cells[COL_TEXT].Value.ToString();

                iLineFeed = text.IndexOf(m_SepLine);
                iParagraph = text.IndexOf(m_SepParagraph);

                
                // If paragraph
                // Create 2 lines: an empty line + a line
                if (iParagraph != -1)
                {
                    // Save previous
                    if (SyncLine.Count > 0)
                        lSyncLyrics.Add(SyncLine);

                    // 1. add empty line for the first cr
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();                    
                    kst = new keffect.KaraokeEffect.kSyncText(time, "");
                    SyncLine.Add(kst);
                    lSyncLyrics.Add(SyncLine);

                    //2. create new line
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();

                    // new item with cr for the second cr
                    text = text.Replace(m_SepParagraph, "");
                    text = cr + text;

                }
                // If linefeed
                else if (iLineFeed != -1)
                {
                    // Save previous
                    if (SyncLine.Count > 0)
                        lSyncLyrics.Add(SyncLine);
                    
                    // Create new line
                    SyncLine = new List<keffect.KaraokeEffect.kSyncText>();                   
                    
                    // new item with cr
                    text = text.Replace(m_SepLine, "");                    
                    text = cr + text;
                }                

                // Add new item to the current line 
                text = text.Replace("_", " ");
                kst = new keffect.KaraokeEffect.kSyncText(time, text);
                SyncLine.Add(kst);
            }

            // Save last line
            if (SyncLine.Count > 0)
                lSyncLyrics.Add(SyncLine);

            return lSyncLyrics;
        }

        public bool IsNumeric(string input)
        {
            return int.TryParse(input, out int test);
        }

        /// <summary>
        /// Check if times in dgview are greater than previous ones
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool CheckTimes(out int line)
        {
            long time;
            long lasttime = -1;

            try
            {
                for (int i = 0; i < dgView.RowCount; i++)
                {
                    if (dgView.Rows[i].Cells[COL_MS].Value == null || dgView.Rows[i].Cells[COL_MS].Value.ToString() == "") continue;

                    time = Convert.ToInt32(dgView.Rows[i].Cells[COL_MS].Value);

                    if (time > lasttime)
                        lasttime = time;
                    else if (time < lasttime)
                    {
                        line = i + 1;
                        return false;
                    }
                }
                line = -1;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                line = -1;
                return false;
            }
        }
     

        #region ddView edition
        private void dgView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            ShowCurrentLine();
        }


        private void dgView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (LrcMode == LrcModes.Sync || PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                e.Cancel = true;
        }

        /// <summary>
        /// Cell edition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object otime;
            string sTime;
            string tsp;
            double time;
            string lyric;
            
            // Modify milliseconds
            if (dgView.CurrentCell.ColumnIndex == COL_MS && dgView.CurrentRow.Cells[COL_MS].Value != null)
            {                
                otime = dgView.CurrentRow.Cells[COL_MS].Value;
                sTime = otime.ToString();
                if (IsNumeric(sTime))
                {
                    time = double.Parse(sTime);
                    tsp = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);
                    dgView.CurrentRow.Cells[COL_TIME].Value = tsp;
                }
            }
            // Modify timestamps
            else if (dgView.CurrentCell.ColumnIndex == COL_TIME && dgView.CurrentRow.Cells[COL_TIME].Value != null)
            {                
                otime = dgView.CurrentRow.Cells[COL_TIME].Value;
                sTime = otime.ToString();
                time = Mp3LyricsMgmtHelper.TimeToMs(sTime);
                dgView.CurrentRow.Cells[COL_MS].Value = time;

            }
            // Modify text
            else if (dgView.CurrentCell.ColumnIndex == COL_TEXT && dgView.CurrentRow.Cells[COL_TEXT].Value != null)
            {                
                lyric = dgView.CurrentRow.Cells[COL_TEXT].Value.ToString();
                lyric = lyric.Replace(" ", "_");
                dgView.CurrentRow.Cells[COL_TEXT].Value = lyric;               
            }

            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

            FileModified();
        }

        #endregion dgView edition


        #region buttons edition

        /// <summary>
        /// Delete all lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAllLyrics_Click(object sender, EventArgs e)
        {
            string tx = Karaboss.Resources.Localization.Strings.DeleteAllLyrics;
            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {

                InitGridView();
                txtResult.Text = string.Empty;

                // File was modified
                FileModified();
            }
        }

        /// <summary>
        /// TxtResult: Increase font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFontPlus_Click(object sender, EventArgs e)
        {
            _fontSize++;
            _lyricseditfont = new Font(_lyricseditfont.FontFamily, _fontSize);
            txtResult.Font = _lyricseditfont;
        }

        /// <summary>
        /// TxtResult: decrease font size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFontMoins_Click(object sender, EventArgs e)
        {
            if (_fontSize > 5)
            {
                _fontSize--;
                _lyricseditfont = new Font(_lyricseditfont.FontFamily, _fontSize);
                txtResult.Font = _lyricseditfont;
            }
        }

        /// <summary>
        /// Insert a LineFeed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertCr_Click(object sender, EventArgs e)
        {
            InsertSepLine("cr");
        }

        /// <summary>
        /// Insert a Paragraph
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertParagraph_Click(object sender, EventArgs e)
        {
            InsertSepLine("par");
        }

        /// <summary>
        /// Insert Linefeed or Paragraph
        /// </summary>
        /// <param name="sep"></param>
        private void InsertSepLine(string sep)
        {
            if (dgView.CurrentRow == null)
                return;

            int Row = dgView.CurrentRow.Index;
            double time = 0;
            string lyric;
            string sTime = string.Empty; ;

            if (dgView.Rows[Row].Cells[COL_TIME].Value != null)
                sTime = dgView.Rows[Row].Cells[COL_TIME].Value.ToString();


            if (dgView.Rows[Row].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_MS].Value.ToString()))
            {
                time = double.Parse(dgView.Rows[Row].Cells[COL_MS].Value.ToString());
            }

            if (sep == "cr")
                lyric = m_SepLine;
            else
                lyric = m_SepParagraph;

            // time, type, note, text, text
            dgView.Rows.Insert(Row, time, sTime, lyric);


            //Load modification into local list of lyrics
            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

            // Modify height of cells according to durations
            //HeightsToDurations();

            // Color separators
            //ColorSepRows();

            // File was modified
            FileModified();
        }

        /// <summary>
        /// Insert a new line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInsertText_Click(object sender, EventArgs e)
        {
            InsertTextLine();
        }

        private void InsertTextLine()
        {
            if (dgView.CurrentRow == null)
                return;

            object otime;
            object otsp;
            object otext;

            string time = string.Empty;
            string tsp = string.Empty;
            string text = string.Empty;

            otime = dgView.CurrentRow.Cells[COL_MS].Value;
            if (otime != null) time = otime.ToString();
            otsp = dgView.CurrentRow.Cells[COL_TIME].Value;
            if (otsp != null) tsp = otsp.ToString();
           

            int line = dgView.CurrentRow.Index;
            dgView.Rows.Insert(line, time, tsp, text);

            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

            FileModified();
        }

        /// <summary>
        /// Delete a line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        private void DeleteLine()
        {
            try
            {
                int row = dgView.CurrentRow.Index;
                dgView.Rows.RemoveAt(row);

                localSyncLyrics = LoadModifiedLyrics();
                if (localSyncLyrics != null)
                    PopulateTextBox(localSyncLyrics);

                FileModified();
            }
            catch (Exception Ex)
            {
                string message = "Error : " + Ex.Message;
                MessageBox.Show(message, "Error deleting line",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Delete all lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteAllLyrics_Click_1(object sender, EventArgs e)
        {
            string tx = Karaboss.Resources.Localization.Strings.DeleteAllLyrics;
            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {

                InitGridView();
                txtResult.Text = string.Empty;

                // File was modified
                FileModified();
            }
        }

        #endregion buttons edition

        #endregion Text


        #region Save Mp3 lyrics

        /// <summary>
        /// File was modified
        /// </summary>
        private void FileModified()
        {

            bfilemodified = true;
            string fName = Path.GetFileName(Mp3FullPath);
            if (fName != null && fName != "")
            {
                string fExt = Path.GetExtension(fName);             // Extension
                fName = Path.GetFileNameWithoutExtension(fName);    // name without extension

                string fShortName = fName.Replace("*", "");
                if (fShortName == fName)
                    fName += "*";

                fName += fExt;
                SetTitle(fName);
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveMp3Lyrics(Mp3FullPath);
        }

        /// <summary>
        /// Save mp3 lyrics
        /// </summary>
        private void SaveMp3Lyrics(string FileName)
        {
            // it is not possible to save the file on the same file (file locked)
            string mp3file = Files.FindUniqueFileName(FileName);

            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(mp3file);
            saveFileDialog1.FileName = Path.GetFileName(mp3file);

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string filename = saveFileDialog1.FileName;

                try
                {
                    // Copy file to another name                
                    System.IO.File.Copy(FileName, filename, true);

                    // Save sync lyrics into the copy of initial file
                    SaveFrame(filename);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Save changes to the synchronized lyrics frame
        /// </summary>
        /// <param name="FileName"></param>
        private void SaveFrame(string FileName)
        {
            string lyric;

            TagLib.File file = TagLib.File.Create(FileName);
            TagLib.Tag _tag = file.GetTag(TagTypes.Id3v2);

            // Reset frame text
            if (Mp3LyricsMgmtHelper.MySyncLyricsFrame == null)
            {
                Mp3LyricsMgmtHelper.MySyncLyricsFrame = new SynchronisedLyricsFrame("Karaboss", "en", SynchedTextType.Lyrics);
            }

            // How many valid lines ?            
            int lines = 0;
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                if (dgView.Rows[i].Cells[COL_MS].Value != null)
                    lines++;
            }

            Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text = new SynchedText[lines];

            // Read all rows and store into the frame
            for (int i = 0; i < lines; i++)
            {
                if (dgView.Rows[i].Cells[COL_MS].Value != null)
                {
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i] = new SynchedText();
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i].Time = long.Parse(dgView.Rows[i].Cells[COL_MS].Value.ToString());

                    // Modify lyrics
                    // \ => '\n'
                    // _ => " "
                    lyric = dgView.Rows[i].Cells[COL_TEXT].Value.ToString();
                    lyric = lyric.Replace(m_SepLine, "\n");
                    lyric = lyric.Replace("_", " ");
                    Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i].Text = lyric;
                }
            }

            if (Mp3LyricsMgmtHelper.SetTags(FileName, Mp3LyricsMgmtHelper.MySyncLyricsFrame))
            {
                string tx = Karaboss.Resources.Localization.Strings.LyricsWereRecorded;
                //string tx = "Les paroles ont été enregistrées dans le fichier";
                MessageBox.Show(tx + "\n" + Path.GetFileName(FileName), "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Reset Title
            bfilemodified = false;
            SetTitle(Mp3FullPath);
        }



        #endregion Mp3 Lyrics

        private void dgView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dgContextMenu = new ContextMenuStrip();
                dgContextMenu.Items.Clear();


                // Insert Text line
                ToolStripMenuItem menuInsertTextLine = new ToolStripMenuItem(Strings.InsertNewLine);
                dgContextMenu.Items.Add(menuInsertTextLine);
                menuInsertTextLine.Click += new System.EventHandler(this.MnuInsertTextLine_Click);

                // Insert LineFeed
                ToolStripMenuItem menuInsertLineBreak = new ToolStripMenuItem(Strings.InsertLineBreak);
                dgContextMenu.Items.Add(menuInsertLineBreak);
                menuInsertLineBreak.Click += new System.EventHandler(this.MnuInsertLineBreak_Click);


                // Insert paragraph
                ToolStripMenuItem menuInsertParagraph = new ToolStripMenuItem(Strings.InsertNewParagraph);
                dgContextMenu.Items.Add(menuInsertParagraph);
                menuInsertParagraph.Click += new System.EventHandler(this.MnuInsertParagraph_Click);

                // Delete line
                ToolStripMenuItem menuDeleteLine = new ToolStripMenuItem(Strings.DeleteLine);
                dgContextMenu.Items.Add(menuDeleteLine);
                menuDeleteLine.Click += new System.EventHandler(this.MnuDeleteLine_Click);


                ToolStripSeparator menusep1 = new ToolStripSeparator();
                dgContextMenu.Items.Add(menusep1);

                // Décaler vers le haut
                ToolStripMenuItem menuOffsetUp = new ToolStripMenuItem(Strings.OffsetUp);
                dgContextMenu.Items.Add(menuOffsetUp);
                menuOffsetUp.Click += new System.EventHandler(this.MnuOffsetUp_Click);

                // Décaler vers le bas
                ToolStripMenuItem menuOffsetDown = new ToolStripMenuItem(Strings.OffsetDown);
                dgContextMenu.Items.Add(menuOffsetDown);
                menuOffsetDown.Click += new System.EventHandler(this.MnuOffsetDown_Click);

                ToolStripSeparator menusep2 = new ToolStripSeparator();
                dgContextMenu.Items.Add(menusep2);


                // Copier
                ToolStripMenuItem menuCopy = new ToolStripMenuItem(Strings.Copy);
                dgContextMenu.Items.Add(menuCopy);
                menuCopy.Click += new System.EventHandler(this.MnuCopy_Click);

                // Coller
                ToolStripMenuItem menuPaste = new ToolStripMenuItem(Strings.Paste);
                dgContextMenu.Items.Add(menuPaste);
                menuPaste.Click += new System.EventHandler(this.MnuPaste_Click);


                // Display menu on the listview
                dgContextMenu.Show(dgView, dgView.PointToClient(Cursor.Position));

            }
        }

        /// <summary>
        /// Insert text line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertTextLine_Click(object sender, EventArgs e)
        {
            InsertTextLine();
        }
        private void MnuInsertLineBreak_Click(object sender, EventArgs e)
        {
            InsertSepLine("cr");
        }

        /// <summary>
        /// Insert paragraph separator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuInsertParagraph_Click(object sender, EventArgs e)
        {
            InsertSepLine("par");
        }

        /// <summary>
        /// Delete current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuDeleteLine_Click(object sender, EventArgs e)
        {
            DeleteLine();
        }

        /// <summary>
        /// Offset up the third column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuOffsetUp_Click(object sender, EventArgs e)
        {
            int r = dgView.CurrentRow.Index;
            int row; // = 0;

            for (row = r; row <= dgView.Rows.Count - 2; row++)
            {
                dgView.Rows[row].Cells[COL_TEXT].Value = dgView.Rows[row + 1].Cells[COL_TEXT].Value;
            }

            //Load modification into local list of lyrics
            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);
        }

        /// <summary>
        /// Offset down the third column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuOffsetDown_Click(object sender, EventArgs e)
        {
            int r = dgView.CurrentRow.Index;
            int row; // = 0;

            for (row = dgView.Rows.Count - 1; row > r; row--)
            {
                dgView.Rows[row].Cells[COL_TEXT].Value = dgView.Rows[row - 1].Cells[COL_TEXT].Value;
            }
            dgView.Rows[r].Cells[COL_TEXT].Value = "";

            //Load modification into local list of lyrics
            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);

        }

        private void MnuCopy_Click(object sender, EventArgs e)
        {
            //DGV = this.dgView.SelectedCells;         

            if (dgView.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(this.dgView.GetClipboardContent());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The Clipboard could not be accessed. Please try again.\n" + ex.Message);
                }
            }
        }

        private void MnuPaste_Click(object sender, EventArgs e)
        {
            // Paste from Clipboard
            PasteClipboard();

            //Load modification into local list of lyrics
            localSyncLyrics = LoadModifiedLyrics();
            if (localSyncLyrics != null)
                PopulateTextBox(localSyncLyrics);


            // Color separators
            //ColorSepRows();

            // File was modified
            FileModified();
        }

        /// <summary>
        /// Paste from Clipboard
        /// </summary>
        private void PasteClipboard()
        {
            try
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');

                int iFail = 0;
                int iRow = 0;
                int iCol = 0;
                if (dgView.CurrentCell != null)
                {
                    iRow = dgView.CurrentCell.RowIndex;
                    iCol = dgView.CurrentCell.ColumnIndex;
                }
                DataGridViewCell oCell;

                string c = string.Empty;

                string plType = string.Empty;
                //int plTicksOn = 0;
                string plRealTime = string.Empty;
                //int plNote = 0;
                string strplnote = string.Empty;
                string plElement = string.Empty;


                if (dgView.Rows.Count < lines.Length)
                    dgView.Rows.Add(lines.Length - 1);

                foreach (string line in lines)
                {
                    if (iRow < dgView.RowCount && line.Length > 0)
                    {
                        string[] sCells = line.Split('\t');

                        for (int i = 0; i < sCells.GetLength(0); ++i)
                        {
                            if (iCol + i < this.dgView.ColumnCount)
                            {
                                oCell = dgView[iCol + i, iRow];
                                if (!oCell.ReadOnly)
                                {
                                    c = sCells[i];
                                    //c = c.Trim();
                                    c = c.Replace("\r", "");
                                    c = c.Replace(" ", "_");

                                    oCell.Value = c;
                                }
                            }
                            else
                            { break; }
                        }
                        iRow++;
                    }
                    else
                    { break; }


                }
                if (iFail > 0)
                    MessageBox.Show(string.Format("{0} updates failed due" +
                                    " to read only column setting", iFail));
            }
            catch (FormatException)
            {
                MessageBox.Show("The data you pasted is in the wrong format for the cell");
                return;
            }
        }


        /// <summary>
        /// Keydown event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    {
                        foreach (DataGridViewCell C in dgView.SelectedCells)
                        {
                            if (C.ColumnIndex != 0)
                                C.Value = "";
                        }

                        //Load modification into local list of lyrics
                        localSyncLyrics = LoadModifiedLyrics();
                        if (localSyncLyrics != null)
                            PopulateTextBox(localSyncLyrics);

                        // File was modified
                        FileModified();
                        break;
                    }
            }
        }
    }
}
