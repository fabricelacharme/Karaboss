#region License

/* Copyright (c) 2026 Fabrice Lacharme
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
using Hqub.MusicBrainz.API.Entities;
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
using TagLib;
using TagLib.Id3v2;
using kar;

namespace Karaboss.Mp3
{
    public partial class frmMp3Player : Form
    {
        #region declarations
     
        private List<(string, string)> lstSaveTimestamps = new List<(string, string)>();


        #region MP3

        private double _duration = 0;   // mp3 duration in ms
        private float _frequency = 0;
        private int _bitrate = 0;

        #endregion MP3


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


        private enum Directions
        {
            Forward,
            Backward
        }
        //private Directions _direction;

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
        private float _fontSize = 11f;

        // Manage locally lyrics
        //List<List<keffect.KaraokeEffect.kSyncText>> localSyncLyrics;
        kLyrics localKaraokeLyrics;


        private readonly string m_SepLine = "/";
        private readonly string m_SepParagraph = "\\";
       
        private int _LrcMillisecondsDigits = 2;

        private int m_MillisecondsOffset = 100; // Default offset in milliseconds to display lyrics
        
        public bool bfilemodified = false;

        // SlideShow directory
        public string dirSlideShow;


        #region player

        // Size player
        // 530;194
        private enum PlayerAppearances
        {
            Player,
            LyricsEditor,
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
        //private bool closing = false;
        //private bool loading = false;

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

            // Create mp3 Player instance, init bass and load file                                    
            Player = new Mp3Player(FileName);
            _duration = Player.Seconds; // * 1000;
            _bitrate = Player.BitRate;
            _frequency = Player.Frequency;

            // Create event for playing completed
            Player.PlayingCompleted += new EndingSyncHandler(HandlePlayingCompleted);
            
            DisplayMp3Characteristics();
            ExtractMp3Lyrics(Mp3FullPath);

            PopulateMetadataTags();

            #region playlists

            if (myPlayList != null)
            {                
                currentPlaylist = myPlayList;
                // Search file to play with its filename                
                currentPlaylistItem = currentPlaylist.Songs.Where(z => z.File == Mp3FullPath).FirstOrDefault();
                
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
                else
                {
                    // If not playing, display lyrics editor
                    mnuEditLyrics.Checked = true;
                    LrcMode = LrcModes.Edit;
                    // Show LRC Generator
                    PlayerAppearance = PlayerAppearances.LyricsEditor;                                        
                }
            }
            #endregion playlists
           
        }

        /// <summary>
        /// Populate textboxes Title, Artist, Album, Year
        /// </summary>
        private void PopulateMetadataTags()
        {
            if (Mp3LyricsMgmtHelper.mp3KaraokeLyrics == null || Mp3LyricsMgmtHelper.mp3KaraokeLyrics.Lines.Count == 0)
                return;

            string title = Mp3LyricsMgmtHelper.Title;
            string artist = Mp3LyricsMgmtHelper.Artist;
            string album = Mp3LyricsMgmtHelper.Album;
            string tool = Mp3LyricsMgmtHelper.Tool;
            uint year = Mp3LyricsMgmtHelper.Year;

            if (title != "")
                txtTitle.Text = title;
            if (artist != "")
                txtArtist.Text = artist;
            if (album != "")
                txtAlbum.Text = album;
            if (year != 0)
                txtYear.Text = year.ToString();
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
            //closing = true;
            base.OnClosing(e);
        }

        private void frmMp3Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bfilemodified == true)
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

            if (Application.OpenForms.OfType<frmMp3LyrOptions>().Count() > 0)
            {
                Application.OpenForms["frmMp3LyrOptions"].Close();
            }


            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmExplorer"].Restore();
                Application.OpenForms["frmExplorer"].Activate();
            }


            // Save settings
            Properties.Settings.Default.LyricsEditFont = _lyricseditfont;
            Properties.Settings.Default.Save();


            Dispose();
        }

        private void frmMp3Player_FormClosed(object sender, FormClosedEventArgs e)
        {
            Player.Reset();            
        }

        private void frmMp3Player_Resize(object sender, EventArgs e)
        {
            if (PlayerAppearance == PlayerAppearances.LyricsEditor)
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

                pnlSync.Left = 6;
                pnlEdit.Left = pnlSync.Left;

                pnlSync.Top = 62;
                pnlEdit.Top = pnlSync.Top;


                pnlSync.Width = pnlTop.Width - 2*pnlSync.Left;
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

            // Show hide edition menus according to LRC Generator visibility
            mnuFileExportLyrics.Visible = mnuEditLyrics.Checked;
            MnuFileSep1.Visible = mnuEditLyrics.Checked;

            mnuEditSep1.Visible = mnuEditLyrics.Checked;
            mnuEditSep2.Visible = mnuEditLyrics.Checked;
            mnuEditInsertNewLine.Visible = mnuEditLyrics.Checked;
            mnuEditDeleteCurrentLine.Visible = mnuEditLyrics.Checked;
            mnuEditImportLyrics.Visible = mnuEditLyrics.Checked;

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


                case PlayerAppearances.LyricsEditor:
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


                    InitEditor();

                    // Populate gridview and textbox
                    PopulateDataGridView();
                    
                    // Update local lyrics list from datagridview content                    
                    //localSyncLyrics = GetCurrentDgViewContent(dgView, COL_MS, COL_TEXT);
                    localKaraokeLyrics = GetCurrentDgViewContent2(dgView, COL_MS, COL_TEXT);

                    // Populate textbox with local lyrics
                    //PopulateTextBox(localSyncLyrics);
                    PopulateTextBox2(localKaraokeLyrics);

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

        public void FirstPlaySong(double start = 0)
        {
            try
            {
                PlayerState = PlayerStates.Playing;
                                
                _index = 0;  // for timestamps entering
                lstSaveTimestamps.Clear();       // Clear timestamps 

                BtnStatus();
                ValideMenus(false);                
                Player.Play(Mp3FullPath, start);

                // Set Volume, frequency & transpose
                SetInitialListenValues();

                StartKaraoke();
                Timer1.Start();

                // Start balls
                StartTimerBalls();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void SetInitialListenValues()
        {
            // Frequency
            //FrequencyRatio = 100;            
            if (FrequencyRatio != 100)
                AdjustMp3Freq(FrequencyRatio);

            // Transpose
            //TransposeValue = 0;            
            if (TransposeValue != 0)
                AdjustMp3Pitch(TransposeValue);

            // Volume
            //sldMainVolume.Value = 104;            
            if (sldMainVolume.Value != 104)
                Player.AdjustVolume((float)sldMainVolume.Value);
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
        private void PlayPauseMusic(double start = 0)
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
                    Timer3.Start(); // Balls
                    Player.Resume();
                    break;

                default:
                    // First play                
                    FirstPlaySong(start);
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
            
            // Stop balls
            StopTimerBalls();

            ValideMenus(true);

            positionHScrollBar.Value = positionHScrollBar.Minimum;
           
        }


        #region buttons
        private void btnPlay_Click(object sender, EventArgs e)
        {
            // Check if we are currently editing lyrics
            if (PlayerAppearance == PlayerAppearances.LyricsEditor && LrcMode == LrcModes.Edit && dgView.Rows.Count > 1)
            {
                int Row = dgView.CurrentRow.Index;
                double time = 0;
                string sTime = string.Empty; ;

                if (dgView.Rows[Row].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_MS].Value.ToString()))
                {
                    // Load frmMp3Lyrics
                    DisplayFrmMp3Lyrics();                    
                    
                    // Reload modified lyrics before playing
                    if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                    {                                                                        
                        frmMp3Lyrics.SetLyrics(localKaraokeLyrics);
                    }
                    

                    // play from a specific time
                    time = double.Parse(dgView.Rows[Row].Cells[COL_MS].Value.ToString());
                    PlayPauseMusic(time/1000);          // time in seconds
                    return;
                }
            }
            
            // Play from start
            PlayPauseMusic();
            
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopMusic();
        }

      
        /// <summary>
        /// Play next or previous mp3 in the current directory
        /// </summary>
        /// <param name="_direction"></param>
        private void PlayMp3Song(Directions _direction)
        {
            string folder;            
            int index;

            // We have this information : Mp3FullPath which is the path of the file being played                
            if (Application.OpenForms.OfType<frmExplorer>().Count() == 0) return;
            
            frmExplorer frmExplorer = Application.OpenForms.OfType<frmExplorer>().First();

            // List of mp3 files filtered by mp3 extension
            folder = Path.GetDirectoryName(Mp3FullPath);
            var files = Directory
                .EnumerateFiles(folder) //<--- .NET 4.5
                 .Where(file => file.ToLower().EndsWith("mp3"))
                 .ToList();

            if (!files.Contains(Mp3FullPath)) return;            
            index = files.IndexOf(Mp3FullPath);

            try
            {
                switch (_direction)
                {
                    // Next file
                    case Directions.Forward:
                        if (index >= files.Count - 1) return;
                        Mp3FullPath = files[index + 1];                        
                        break;

                    // Previous file
                    case Directions.Backward:
                        if (index == 0) return;
                        Mp3FullPath = files[index - 1];
                        break;

                }

                // Stop player
                StopMusic();

                // Select new file in the explorer
                Mp3FileName = Path.GetFileName(Mp3FullPath);
                string path = Path.GetDirectoryName(Mp3FullPath);
                path = "file:///" + path.Replace("\\", "/");
                frmExplorer.NavigateTo(path, Mp3FileName);

                // Update display
                SetTitle(Mp3FullPath);
                DisplayMp3Characteristics();
                ExtractMp3Lyrics(Mp3FullPath);
                
                // Play file
                PlayPauseMusic();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopMusic();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {            

            // play next song in the directory
            if (currentPlaylist == null)
            {
                PlayMp3Song(Directions.Forward);               
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
                PlayMp3Song(Directions.Backward);                             
            }
            else
            {
                // Playlist
                PlayPrevSong();
            }
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

        private void KeyboardSelectTempo(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    FrequencyRatio += 10;
                    AdjustMp3Freq(FrequencyRatio);
                    break;

                case Keys.D6:
                case Keys.OemMinus:
                case Keys.Subtract:
                    FrequencyRatio -= 10;
                    AdjustMp3Freq(FrequencyRatio);
                    break;
            }
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


        #region menu File
       
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenBrowseMp3();
        }

        private void mnuFileQuit_Click(object sender, EventArgs e)
        {            
            Close();
        }


        #endregion menu File


        #region menu Edit

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
                PlayerAppearance = PlayerAppearances.LyricsEditor;
            }
            else
            {
                // Hide Lrc Generator
                PlayerAppearance = PlayerAppearances.Player;
            }

            // Redim form according to player or editor
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


        private void mnuEditInsertNewLine_Click(object sender, EventArgs e)
        {
            InsertTextLine();
        }

        private void mnuEditDeleteCurrentLine_Click(object sender, EventArgs e)
        {            
            DeleteSelectedLines();
        }


        #endregion

        #region menu Help

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }
       
        /// <summary>
        /// Set titloe of form
        /// </summary>
        /// <param name="displayName"></param>
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

        private void mnuHelpForums_Click(object sender, EventArgs e)
        {
            Karaclass.DisplayUrl(Karaclass.url_forums);
        }

        private void mnuHelpDocumentation_Click(object sender, EventArgs e)
        {
            Karaclass.DisplayUrl(Karaclass.url_documentation);
        }


        #endregion menu Help

        #endregion menus


        #region Files Save Open

        /// <summary>
        /// Open mp3 file
        /// </summary>
        private void OpenBrowseMp3()
        {
            OpenFileDialog.Title = "Open mp3 file";
            OpenFileDialog.Filter = "mp3 Files (*.mp3)|*.mp3";
            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                Mp3FullPath = OpenFileDialog.FileName;

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
        }

        /// <summary>
        /// Export mp3 Lyrics tags to a text file / or from lrc having the same name 
        /// </summary>
        public void ExportLyricsTags()
        {
            switch (Mp3LyricsMgmtHelper.m_mp3lyricstype) {

                // Lyrics included in the mp3 file
                case Mp3LyricsTypes.LyricsWithTimeStamps:                                
                    TagLib.Id3v2.SynchronisedLyricsFrame SyncLyricsFrame = Player.SyncLyricsFrame;
                    Mp3LyricsMgmtHelper.ExportSyncLyricsToText(SyncLyricsFrame);
                    break;

                // Lyrics from a LRC file
                case Mp3LyricsTypes.LRCFile:
                    Mp3LyricsMgmtHelper.ExportSyncLyricsToText(Mp3LyricsMgmtHelper.mp3KaraokeLyrics);
                    break;
            }
        }

        /// <summary>
        /// Extract lyrics from any source found
        /// </summary>
        /// <param name="FileName"></param>
        private void ExtractMp3Lyrics(string FileName)
        {
            // Reset static infos
            //Mp3LyricsMgmtHelper.SyncLyrics = new System.Collections.Generic.List<System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>>();
            //Mp3LyricsMgmtHelper.SyncLine = new System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>();

            Mp3LyricsMgmtHelper.mp3KaraokeLyrics = new kLyrics();
            Mp3LyricsMgmtHelper.mp3KaraokeLine = new kLine();


            Player.GetMp3Infos(Mp3FullPath);                        
            pBox.Image = Player.AlbumArtImage;            
            TagLib.Tag Tag = Player.Tag;            
                        
            
            Mp3LyricsMgmtHelper.m_mp3lyricstype = Mp3LyricsTypes.None;

            string lrcfile = string.Empty;
            string TagLyrics = string.Empty;
            if (Tag != null)
                TagLyrics = Tag.Lyrics;
            string TagSubTitles = string.Empty;

            // Mp3 sync lyrics with time stamps
            TagLib.Id3v2.SynchronisedLyricsFrame SyncLyricsFrame = Player.SyncLyricsFrame;
            Mp3LyricsMgmtHelper.MySyncLyricsFrame = SyncLyricsFrame;

           
            // Get lyrics type origin
            Mp3LyricsMgmtHelper.m_mp3lyricstype = Mp3LyricsMgmtHelper.GetLyricsType(SyncLyricsFrame, TagLyrics, TagSubTitles, FileName);
          
            
            switch (Mp3LyricsMgmtHelper.m_mp3lyricstype)
            {
                // Synchronized Lyrics included in the mp3 file
                case Mp3LyricsTypes.LyricsWithTimeStamps:                                        
                    // This one returns lyrics without separators
                    //Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetLyricsFromMp3File(SyncLyricsFrame);    // KaraokeEffect
                    Mp3LyricsMgmtHelper.mp3KaraokeLyrics = Mp3LyricsMgmtHelper.GetLyricsFromMp3File(SyncLyricsFrame);    // KaraokeLyrics class used for display in frmMp3Lyrics
                    DisplayFrmMp3Lyrics();
                    break;

                // Lyrics brought by a lrc file in the same directory & having the same name
                case Mp3LyricsTypes.LRCFile:
                    // TODO: return lyrics without separators as previous case
                    //Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetKEffectLrcLyrics(FileName);
                    //Mp3LyricsMgmtHelper.SyncLyrics = Mp3LyricsMgmtHelper.GetLyricsFromLrcFile(FileName);
                    Mp3LyricsMgmtHelper.mp3KaraokeLyrics = Mp3LyricsMgmtHelper.GetLyricsFromLrcFile(FileName);                    

                    DisplayFrmMp3Lyrics();
                    
                    // Load lyrics in KaraokeEffect of frmMp3Myrics
                    if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                    {
                        frmMp3Lyrics.SetLyrics(Mp3LyricsMgmtHelper.mp3KaraokeLyrics);
                    }
                    break;
                
                case Mp3LyricsTypes.KOKFile:
                    break;
              
                // Non synchronized Lyrics included in the mp3 file
                case Mp3LyricsTypes.LyricsWithoutTimeStamps:                    
                    string tx = string.Empty;
                    if (TagLyrics != null)
                        tx += TagLyrics;
                    if (TagSubTitles != null)
                        tx += TagSubTitles;                    
                    Mp3LyricsMgmtHelper.mp3KaraokeLyrics = Mp3LyricsMgmtHelper.GetKEffectStringLyrics(tx);
                    
                    DisplayFrmSimpleLyrics(tx);
                    break;
                

                default:                                        
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


            // MP3 caracteristics
            frmMp3Lyrics.Duration = _duration; // mp3 duration in ms
            frmMp3Lyrics.Frequency = _frequency;
            frmMp3Lyrics.BitRate = _bitrate;

            // Load lyrics from Mp3LyricsMgmtHelper.mp3KaraokeLyrics
            //frmMp3Lyrics.LoadLyrics();

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
            frmMp3LyricsSimple = new frmMp3LyricsSimple( Path.GetFileName(Mp3FullPath));
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
            sldMainVolume.ShowDivisionsText = false;
            sldMainVolume.ShowSmallScale = false;
            sldMainVolume.TickStyle = TickStyle.Both;
            sldMainVolume.TickColor = Color.White;
            sldMainVolume.TickAdd = 0;
            sldMainVolume.TickDivide = 0;

            sldMainVolume.Orientation = Orientation.Vertical;
            sldMainVolume.Maximum = 130;    // Closer to 127
            sldMainVolume.Minimum = 0;
            sldMainVolume.ScaleDivisions = 13;
            sldMainVolume.ScaleSubDivisions = 5;
            sldMainVolume.Value = 104;
            sldMainVolume.SmallChange = 13;
            sldMainVolume.LargeChange = 13;
            sldMainVolume.MouseWheelBarPartitions = 10;

            sldMainVolume.Left = 272;
            sldMainVolume.Top = 25;
            sldMainVolume.Width = 24;
            sldMainVolume.Height = 80;

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
            this.VuPeakVolumeLeft.Location = new Point(220, 7);



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
            this.VuPeakVolumeRight.Location = new Point(236, 7);

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
            
            
            ResizeDgView();

            lblLyrics.Text = "0";
            lblTimes.Text = lblLyrics.Text;
        }


        private void ResizeDgView()
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

            // close frmMp3Lyrics
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                frmMp3Lyrics.Close();
            }

            // Close form frmMp3LyricsSimple
            if (Application.OpenForms.OfType<frmMp3LyricsSimple>().Count() > 0)
            {
                frmMp3LyricsSimple.Close();
            }

            PlayerState = PlayerStates.Playing;
            Mp3FullPath = currentPlaylistItem.File;            

            SelectFileToLoadAsync(Mp3FullPath);
        }

        private void SelectFileToLoadAsync(string FileName)
        {
            // Load file and after launch player taking account things to do betwween 2 songs                                  
            Player.FileName = FileName;
            
            DisplayMp3Characteristics();
            ExtractMp3Lyrics(FileName);

          
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
                

                if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                {
                    // During the waiting time, display informations about the next singer
                    //int nbLines;
                    string toptxt;
                    string centertxt;

                    if (currentPlaylistItem.KaraokeSinger == "" || currentPlaylistItem.KaraokeSinger == "<Song reserved by>")
                    {
                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song);
                        //nbLines = 1;
                    }
                    else
                    {

                        toptxt = "Next song: " + Path.GetFileNameWithoutExtension(currentPlaylistItem.Song) + " - Next singer: " + currentPlaylistItem.KaraokeSinger;
                        centertxt = Path.GetFileNameWithoutExtension(currentPlaylistItem.Song)
                            + _InternalSepLines + Karaboss.Resources.Localization.Strings.SungBy
                            + _InternalSepLines + currentPlaylistItem.KaraokeSinger;
                        //nbLines = 4;
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

            
        /// <summary>
        /// Start count down before playing
        /// </summary>
        private void StartCountDownTimer()
        {
            PlayerState = PlayerStates.Waiting;
            BtnStatus();
            
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
                    Timer3.Stop(); // Balls 
                    AfterStopped();
                    break;

                case PlayerStates.Paused:                    
                    Player.Pause();
                    Timer1.Stop();
                    Timer3.Stop(); // Balls 
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

        /// <summary>
        /// Timer 3: for balls animation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer3_Tick(object sender, EventArgs e)
        {
            // 21 balls: 1 fix, 20 moving to the fix one
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                frmMp3Lyrics?.MoveBalls((int)(Player.Position * 1000));
            }
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


        #region import export lyrics


        #region kok import export

        #region import kok

        /*
            * KOK format example:
            * 
            * Times:
            * Exemple
            * Dig;207,3672; in;207,5166; the;207,8154; dan;207,9648;cing;208,4130; queen;208,8612;
            * the last item is 208,8612
            * It means 208 seconds and 8612 milliseconds
            * 
            * 
            * You;26.294; can;26.892; dance;27.191;
            * You;28.685; can;29.282; jive;29.581;
            * Ha;31.075;ving;31.523; the;31.972; time;32.27; of;32.719; your;33.167; life;33.466;
            * Ooh,;34.661; see;35.856; that;36.304; girl,;36.752; watch;38.246; that;38.695; scene.;39.143;
            * Dig;40.039; in;40.189; the;40.487; dan;40.637;cing;41.085; queen;41.533;
            * Fri;50.198;day;50.497; night;50.647; and;50.945; the;51.244; lights;51.394; are;51.692; low;51.991;
            * Loo;54.979;king;55.278; out;55.427; for;55.726; a;56.025; place;56.174; to;56.473; go;56.772;
            * Oh,;59.013; where;59.76; they;60.059; play;60.208; the;60.507; right;60.656; mu;60.955;sic;61.254;
            * Get;62.15;ting;62.449; in;62.599; the;62.897; swing;63.047;
        */

        private void mnuEditImportLyricsKok_Click(object sender, EventArgs e)
        {            
            ImportLyricsToKokFormat();
        }      

        private void mnuImportLyricsKok_Click(object sender, EventArgs e)
        {
            ImportLyricsToKokFormat();
        }

        /// <summary>
        /// Imports lyrics from a selected .kok file and updates the data grid view and associated text box with the
        /// file's content.
        /// </summary>
        /// <remarks>This method prompts the user to choose a .kok file using an open file dialog. Upon
        /// successful selection, it loads the file, updates display counters, selects the first row in the data grid
        /// view, and displays the current lyrics in the text box. The method does not perform any action if the user
        /// cancels the file selection dialog.</remarks>
        private void ImportLyricsToKokFormat()
        {
            string fileName;

            #region select filename

            OpenFileDialog.Title = "Open a .kok file";
            OpenFileDialog.DefaultExt = "kok";
            OpenFileDialog.Filter = "Kok files|*.kok|All files|*.*";
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog.ShowDialog() != DialogResult.OK)
                return;

            
            fileName = OpenFileDialog.FileName;

            #endregion select filename

            // Load KOK file and populate datagridview
            LoadKokFile(fileName);

            // Update counters            
            lblTimes.Text = "0";

            // Select first row
            dgView.Rows[0].Selected = true;

            // Load lyrics in the text box
            //localSyncLyrics = GetCurrentDgViewContent(dgView, COL_MS, COL_TEXT);
            //PopulateTextBox(localSyncLyrics);  
            
            localKaraokeLyrics = GetCurrentDgViewContent2(dgView, COL_MS, COL_TEXT);
            PopulateTextBox2(localKaraokeLyrics);

        }

        /// <summary>
        /// Loads a KOK file and populates the data grid view with its contents.
        /// </summary>
        /// <remarks>If an error occurs during the loading process, an error message is displayed to the
        /// user.</remarks>
        /// <param name="fileName">The name of the KOK file to load. This parameter must not be null or empty.</param>
        private void LoadKokFile(string fileName)
        {
            try
            {              
                // NEW                
                //List <List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = LyricsUtilities.ReadKokFromFile(fileName, _duration);
                kLyrics karaokeLyrics = LyricsUtilities.ReadKokFromFile(fileName, _duration);

                // Populate DataGridView
                //PopulateDataGridView(SyncLyrics);
                PopulateDataGridView(karaokeLyrics);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading KOK file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reads a file containing pairs of words and timestamps, and returns a list of tuples representing each word
        /// and its associated timestamp.
        /// </summary>
        /// <remarks>Each line in the file should be formatted as 'word;timestamp; word;timestamp; ...'.
        /// The method processes each line, pairing words with their timestamps in the order they appear. The first word
        /// of each line is prefixed with a line separator ('/').</remarks>
        /// <param name="fileName">The path to the file to read. The file must exist and be accessible.</param>
        /// <returns>A list of tuples, where each tuple contains a word and its corresponding timestamp extracted from the file.</returns>
        private List<(string, string)> KokReadFile(string fileName)
        {
            string word;
            string timestamp;
            bool paragraphSepFound = false;

            List<(string, string)> lstDgRows = new List<(string, string)>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {

                    // Case empty line; this is a paragraph
                    // The next line will be the first line of the new paragraph, and must be prefixed with a line separator ('/'), except if it is the first line of the file
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        paragraphSepFound = true;
                        continue;
                    }

                    // Each line is expected to be in the format: "word;timestamp; word;timestamp; ..."
                    string[] parts = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length - 1; i += 2)
                    {

                        timestamp = parts[i + 1].Trim();

                        // Each first word of a line must starts with a line separator, except the first one of the file 
                        // No trim, because the space is used to separate words
                        if (i == 0 && lstDgRows.Count > 0)
                        {
                            
                            if (paragraphSepFound)
                            {
                                word = m_SepParagraph + parts[i];
                                paragraphSepFound = false;
                            }
                            else
                            {
                                word = m_SepLine + parts[i];
                            }
                                                        
                        }
                        else
                        {
                            word = parts[i];
                        }
                        
                        lstDgRows.Add((word, timestamp));
                    }
                }
            }
            return lstDgRows;
        }

        /// <summary>
        /// Populates the data grid view with rows containing words and their corresponding timestamps.
        /// </summary>
        /// <remarks>The method clears the existing rows in the data grid view before adding new rows.
        /// Each timestamp is converted to a formatted string in the mm:ss:ms format and also to milliseconds for
        /// display.</remarks>
        /// <param name="lstDgRows">A list of tuples, where each tuple contains a word and its associated timestamp in seconds.</param>
        private void KokPopulateDgView(List<(string, string)> lstDgRows)
        {
            string sTimeStamp;
            string lyric;
            dgView.Rows.Clear();
            foreach (var (word, timestamp) in lstDgRows)
            {

                // In the second column, the time is in format mm:ss:ms
                // Convert timestamp to mm:ss:ms
                sTimeStamp = TimeSpan.FromSeconds(Convert.ToDouble(timestamp)).ToString(@"mm\:ss\.fff");

                // Convert timestamp to milliseconds if necessary
                double ms = Convert.ToDouble(timestamp) * 1000; // Assuming timestamp is in seconds

                lyric = word.Replace(" ", "_");

                dgView.Rows.Add(ms, sTimeStamp, lyric);
            }
        }


        #endregion import kok


        #region export kok

        /// <summary>
        /// Handles the event when the Export Lyrics to KOK Format menu item is clicked, initiating the export of lyrics
        /// in KOK format.
        /// </summary>
        /// <remarks>This method triggers the export process, converting the lyrics into a specific KOK
        /// format suitable for further processing or storage.</remarks>
        /// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void mnuExportLyricsKok_Click(object sender, EventArgs e)
        {
            /*
             * KOK format example:
             * You;26.294; can;26.892; dance;27.191;
             * You;28.685; can;29.282; jive;29.581;
             * Ha;31.075;ving;31.523; the;31.972; time;32.27; of;32.719; your;33.167; life;33.466;
             * Ooh,;34.661; see;35.856; that;36.304; girl,;36.752; watch;38.246; that;38.695; scene.;39.143;
             * Dig;40.039; in;40.189; the;40.487; dan;40.637;cing;41.085; queen;41.533;
             * Fri;50.198;day;50.497; night;50.647; and;50.945; the;51.244; lights;51.394; are;51.692; low;51.991;
             * Loo;54.979;king;55.278; out;55.427; for;55.726; a;56.025; place;56.174; to;56.473; go;56.772;
             * Oh,;59.013; where;59.76; they;60.059; play;60.208; the;60.507; right;60.656; mu;60.955;sic;61.254;
             * Get;62.15;ting;62.449; in;62.599; the;62.897; swing;63.047;
            */

            //ExportLyricsToKokFormat();
            GetKokSaveOptions();
        }

        /// <summary>
        /// Handles the click event for exporting lyrics to the Kok format.
        /// </summary>
        /// <remarks>This method triggers the export process to convert and save the lyrics in the Kok
        /// format.</remarks>
        /// <param name="sender">The source of the event, typically the control that raised the event.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void mnuFileExportLyricsKok_Click(object sender, EventArgs e)
        {
            //ExportLyricsToKokFormat();
            GetKokSaveOptions();
        }


        /// <summary>
        /// Displays a dialog for configuring options related to saving Kok format lyrics and processes the selected
        /// options.
        /// </summary>
        /// <remarks>This method presents a user interface for setting preferences such as removing
        /// accents, forcing upper or lower case, and removing non-alphanumeric characters. The selected options are
        /// then used to format the lyrics accordingly. If the dialog is canceled, no changes are made.</remarks>
        private void GetKokSaveOptions()
        {
            DialogResult dr;
            frmKokOptions KokOptionsDialog = new frmKokOptions();
            dr = KokOptionsDialog.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.Cancel)
                return;

            // Remove accents
            bool bRemoveAccents = KokOptionsDialog.bRemoveAccents;
            // Force Upper Case
            bool bUpperCase = KokOptionsDialog.bUpperCase;
            // Force Lower Case
            bool bLowerCase = KokOptionsDialog.bLowerCase;
            // Remove all non-alphanumeric characters
            bool bRemoveNonAlphaNumeric = KokOptionsDialog.bRemoveNonAlphaNumeric;

            ExportLyricsToKokFormat(bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric);
        }

        /// <summary>
        /// Exports the current lyrics to a file in KOK format, prompting the user to select the destination and
        /// filename.
        /// </summary>
        /// <remarks>If the specified path is invalid or empty, the method defaults to the user's music
        /// directory. The method handles file naming conflicts by generating a unique filename. After saving the lyrics
        /// in KOK format, the file is opened automatically. If an error occurs during saving or opening, an error
        /// message is displayed.</remarks>
        private void ExportLyricsToKokFormat(bool bRemoveAccents, bool bUpperCase, bool bLowerCase, bool bRemoveNonAlphaNumeric)
        {
            #region select filename

            string defExt = ".kok";
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

            // Extension forced to kok            
            string fullPath = fPath + "\\" + Path.GetFileNameWithoutExtension(fName) + defExt;
            fullName = Utilities.Files.FindUniqueFileName(fullPath);                            // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);                               // Default name to propose to dialog

            #endregion search name                   

            string defFilter = "KOK files (*.kok)|*.kok|All files (*.*)|*.*";

            SaveFileDialog.Title = "Save to KOK format";
            SaveFileDialog.Filter = defFilter;
            SaveFileDialog.DefaultExt = defExt;
            SaveFileDialog.InitialDirectory = @fPath;
            SaveFileDialog.FileName = defName;

            if (SaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion select filename

            fullPath = SaveFileDialog.FileName;

            // For each line of lyric, read all the syllabes and their timestamps
            // and store the result in a list            
            List<(double Time, string lyric)> lstDgRows = LyricsUtilities.ReadDataGridContent(dgView, COL_TIME, COL_TEXT);
            if (lstDgRows == null || lstDgRows.Count == 0)
            {
                MessageBox.Show("No lyric to export", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Save to KOK file
            LyricsUtilities.SaveKOKSyllabes(fullPath, lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, _LrcMillisecondsDigits, null);
           
        }

        #endregion export kok

        #endregion kok import export


        #region lrc import export

        #region import lrc

        /// <summary>
        /// Handles the Click event to import lyrics from an LRC file.
        /// </summary>
        /// <remarks>This method invokes the ImportLyricsFromLrc method to perform the import operation
        /// when the user selects the corresponding menu item.</remarks>
        /// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
        /// <param name="e">An EventArgs object that contains the event data.</param>
        private void mnuEditImportLyricsLrc_Click(object sender, EventArgs e)
        {
            ImportLyricsFromLrc();
        }


        /// <summary>
        /// Import lyrics form an LRC file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuImportLrcFile_Click(object sender, EventArgs e)
        {
            ImportLyricsFromLrc();
        }


        /// <summary>
        /// Imports synchronized lyrics from a selected .lrc file and updates the user interface to display the loaded
        /// lyrics.
        /// </summary>
        /// <remarks>This method prompts the user to choose a .lrc file, loads the lyrics, and refreshes
        /// the data grid view and related UI elements to reflect the imported lyrics. The file dialog's initial
        /// directory is set to the location of the currently loaded MP3 file. The method also resets the lyrics frame
        /// and updates display counters to ensure the UI is synchronized with the newly imported lyrics.</remarks>
        private void ImportLyricsFromLrc()
        {
            #region Open file
            OpenFileDialog.Title = "Open a .lrc file";
            OpenFileDialog.DefaultExt = "lrc";
            OpenFileDialog.Filter = "Lrc files|*.lrc|All files|*.*";
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion Open file
            
            string FileName = OpenFileDialog.FileName;

            Mp3LyricsMgmtHelper.MySyncLyricsFrame = null;

            // Load LRC file
            //Mp3LyricsMgmtHelper.SyncLyrics = LyricsUtilities.ReadLrcFromFile(FileName);

            // TEST load LRC with new KaraokeLyrics class
            kLyrics SyncLyrics = LyricsUtilities.ReadLrcFromFile(FileName);


            PopulateMetadataTags();
            
            PopulateDataGridView();


            // Update counters            
            lblTimes.Text = "0";

            // Select first row
            dgView.Rows[0].Selected = true;

            //localSyncLyrics = GetCurrentDgViewContent(dgView, COL_MS, COL_TEXT);
            //PopulateTextBox(localSyncLyrics);            

            localKaraokeLyrics = GetCurrentDgViewContent2(dgView, COL_MS, COL_TEXT);
            PopulateTextBox2(localKaraokeLyrics);
        }
             
             

        /// <summary>
        /// Store dgView content
        /// </summary>
        /// <returns></returns>
        
        /*
        private List<List<keffect.KaraokeEffect.kSyncText>> GetCurrentDgViewContent(DataGridView dgView, int colMs, int colText)
        {
            object otext;
            object otime;

            string lyric;
            long time;
            string sTime;
            keffect.KaraokeEffect.kSyncText sct;
            
            bool bNewLine = false;
            bool bNewParagraph = false;

            // Single line
            List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
            // List of lines
            List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();

           
            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                

                otext = dgView.Rows[i].Cells[colText].Value;
                otime = dgView.Rows[i].Cells[colMs].Value;

                if (otext == null) continue;

                lyric = otext.ToString();
                sTime = otime.ToString();
                if (IsNumeric(sTime))
                    time = long.Parse(sTime);
                else
                    time = 0;

                bNewLine = false;
                bNewParagraph = false;

                if (lyric.Trim() != "")
                {

                    lyric = lyric.Replace("_", " ");
                    
                    // Search for new lines
                    if (lyric.StartsWith(m_SepParagraph))
                    {
                        lyric = lyric.Substring(1);
                        bNewParagraph = true;                       
                    }
                    else if (lyric.StartsWith(m_SepLine))
                    {
                        lyric = lyric.Substring(1);
                        bNewLine = true;
                    }
                    
                    sct = new keffect.KaraokeEffect.kSyncText(time, lyric);
                    
                    if (bNewParagraph)
                    {
                        // Add the previous line to the list
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // add an empty line for the paragraph separation
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(new keffect.KaraokeEffect.kSyncText(time, ""));                        
                        SyncLyrics.Add(SyncLine);

                        // create a new line
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        // Add syllabe to the line
                        SyncLine.Add(sct);

                        bNewParagraph = false;
                    }
                    else  if (bNewLine)
                    {
                        // Add the previous line to the list
                        if (SyncLine.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // create a new line
                        SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
                        // Add syllabe to the line
                        SyncLine.Add(sct);

                        bNewLine = false;
                    }
                    else
                    {
                        // Add syllabe to the line
                        SyncLine.Add(sct);
                    }
                }
            }

            // Store last line
            if (SyncLine.Count > 0)
                SyncLyrics.Add(SyncLine);

            return SyncLyrics;
        }
        */

        private kLyrics GetCurrentDgViewContent2(DataGridView dgView, int colMs, int colText)
        {
            object otext;
            object otime;
            string lyric;
            long time;
            string sTime;            
            bool bNewLine = false;
            bool bNewParagraph = false;

            // Syllable
            Syllable sct;
            // Single line
            kLine SyncLine = new kLine();
            // List of lines
            kLyrics SyncLyrics = new kLyrics();

            for (int i = 0; i < dgView.Rows.Count; i++)
            {
                otext = dgView.Rows[i].Cells[colText].Value;
                otime = dgView.Rows[i].Cells[colMs].Value;

                if (otext == null) continue;

                lyric = otext.ToString();
                sTime = otime.ToString();
                if (IsNumeric(sTime))
                    time = long.Parse(sTime);
                else
                    time = 0;

                bNewLine = false;
                bNewParagraph = false;

                if (lyric.Trim() != "")
                {

                    lyric = lyric.Replace("_", " ");

                    // Search for new lines
                    if (lyric.StartsWith(m_SepParagraph))
                    {
                        lyric = lyric.Substring(1);
                        bNewParagraph = true;
                    }
                    else if (lyric.StartsWith(m_SepLine))
                    {
                        lyric = lyric.Substring(1);
                        bNewLine = true;
                    }

                    sct = new Syllable(lyric, time);  

                    if (bNewParagraph)
                    {
                        // Add the previous line to the list
                        if (SyncLine.Syllables.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // add an empty line for the paragraph separation
                        SyncLine = new kLine();  //new List<keffect.KaraokeEffect.kSyncText>();
                        SyncLine.Add(new Syllable("", time)); // new keffect.KaraokeEffect.kSyncText(time, ""));
                        SyncLyrics.Add(SyncLine);

                        // create a new line
                        SyncLine = new kLine(); // List<keffect.KaraokeEffect.kSyncText>();
                        // Add syllabe to the line
                        SyncLine.Add(sct);

                        bNewParagraph = false;
                    }
                    else if (bNewLine)
                    {
                        // Add the previous line to the list
                        if (SyncLine.Syllables.Count > 0)
                            SyncLyrics.Add(SyncLine);

                        // create a new line
                        SyncLine = new kLine();
                        // Add syllabe to the line
                        SyncLine.Add(sct);

                        bNewLine = false;
                    }
                    else
                    {
                        // Add syllabe to the line
                        SyncLine.Add(sct);
                    }
                }
            }

            // Store last line
            if (SyncLine.Syllables.Count > 0)
                SyncLyrics.Add(SyncLine);

            return SyncLyrics;
        }


        #endregion import lrc


        #region export lrc

        /// <summary>
        /// Save as lrc file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void mnuExportAsLrc_Click(object sender, EventArgs e)
        {
            GetLrcSaveOptions();
        }

        private void mnuFileExportLyricsLrc_Click(object sender, EventArgs e)
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

            SaveFileDialog.Title = "Save to LRC format";
            SaveFileDialog.Filter = defFilter;
            SaveFileDialog.DefaultExt = defExt;
            SaveFileDialog.InitialDirectory = @fPath;
            SaveFileDialog.FileName = defName;

            if (SaveFileDialog.ShowDialog() != DialogResult.OK)
                return;

            #endregion

            fullPath = SaveFileDialog.FileName;

            #region metadata
            string Tag_Tool = string.Empty;
            string Tag_Title = string.Empty;
            string Tag_Artist = string.Empty;
            string Tag_Album = string.Empty;
            string Tag_Lang = string.Empty;
            string Tag_By = string.Empty;
            uint Tag_Year = 0;
            string Tag_DPlus = string.Empty;

            if (bWithMetadata)
            {
                Tag_Tool = "Karaboss https://karaboss.lacharme.net";
                Tag_Title = txtTitle.Text;
                Tag_Artist = txtArtist.Text;
                Tag_Album = txtAlbum.Text;
                
                if (IsNumeric(txtYear.Text))
                    Tag_Year = Convert.ToUInt32(txtYear.Text);
                Tag_Lang = cbLanguage.Text;

                Tag_DPlus = string.Empty;

                // If no tags in the mp3 file, tries to get them with the file name
                if (Tag_Artist == "" && Tag_Title == "")
                {
                    List<string> lstTags = Utilities.LyricsUtilities.GetTagsFromFileName(fullPath);
                    Tag_Artist = lstTags[0];
                    Tag_Title = lstTags[1];
                }
            }
            #endregion metadata


            // Read DatagridView Content            
            List<(double Time, string lyric)> lstDgRows = LyricsUtilities.ReadDataGridContent(dgView, COL_TIME, COL_TEXT);
            if (lstDgRows == null || lstDgRows.Count == 0)
            {
                MessageBox.Show("No lyric to export", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            switch (LrcLinesSyllabesFormat)
            {
                case LrcLinesSyllabesFormats.Lines:
                    //SaveLRCLines(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Year, Tag_DPlus, bCutLines, LrcCutLinesChars);
                    Utilities.LyricsUtilities.SaveLRCLines(fullPath, lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_Year, Tag_DPlus, bCutLines, LrcCutLinesChars, _LrcMillisecondsDigits, null);
                    break;
                case LrcLinesSyllabesFormats.Syllabes:
                    //SaveLRCSyllabes(fullPath, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_Year, Tag_DPlus);
                    Utilities.LyricsUtilities.SaveLRCSyllabes(fullPath, lstDgRows, bRemoveAccents, bUpperCase, bLowerCase, bRemoveNonAlphaNumeric, Tag_Tool, Tag_Title, Tag_Artist, Tag_Album, Tag_Lang, Tag_By, Tag_Year, Tag_DPlus, _LrcMillisecondsDigits, null);
                    break;
            }
        }
      

        #endregion export lrc


        #endregion lrc import export



        #region txt import export

        #region import txt

        /// <summary>
        /// Import a text file (no timestamps)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuEditImportLyricsTxt_Click(object sender, EventArgs e)
        {
            ImportLyricsFromTxt();

        }

        private void ImportLyricsFromTxt()
        {

            OpenFileDialog.Title = "Open a .txt file";
            OpenFileDialog.DefaultExt = "txt";
            OpenFileDialog.Filter = "Text files|*.txt|All files|*.*";
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(Mp3FullPath);

            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                string FileName = OpenFileDialog.FileName;

                // Reset
                InitGridView();

                string[] lines = System.IO.File.ReadAllLines(FileName);
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

                //localSyncLyrics = GetCurrentDgViewContent(dgView, COL_MS, COL_TEXT);
                //PopulateTextBox(localSyncLyrics);

                localKaraokeLyrics = GetCurrentDgViewContent2(dgView, COL_MS, COL_TEXT);
                PopulateTextBox2(localKaraokeLyrics);


                if (MessageBox.Show(Strings.SwitchToSyncMode + "?", "Karaboss", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SetSyncEditMode();
                }
            }
        }
     

        #endregion import txt

        #region export txt
        private void mnuExportLyricsTxt_Click(object sender, EventArgs e)
        {

        }

        private void mnuFileExportLyricsTxt_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion txt import export



        #endregion import export lyrics


        #region Editor

        private void InitEditor()
        {
            if (PlayerAppearance != PlayerAppearances.LyricsEditor) return;

            LrcMode = LrcModes.Edit;

            _lyricseditfont = Properties.Settings.Default.LyricsEditFont;
            if (_lyricseditfont == null)
                _lyricseditfont = new Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            
                
            _fontSize = _lyricseditfont.Size;
            txtResult.Font = _lyricseditfont;

            // index for times entering
            _index = 0;

            // 2 or 3 digits for timestamp format 00:00.00 or 00:00.000
            _LrcMillisecondsDigits = Properties.Settings.Default.LrcMillisecondsDigits;

            pnlEdit.Visible = true;

            // first hotkeys
            lblHotkeys.Font = new Font("Courier New", 9);                        
            string tx = Strings.Mp3TimeStampHotKey1;
            tx = string.Format(tx, Environment.NewLine);            
            lblHotkeys.Text = tx; // "<ENTER>" + " " + "Add a new timestamp" + "\r\n" + "<DEL>" + "   " + "Remove current timestamp" + "\r\n" + "<SPACE>" + " "+ "Pause Music" ;

            // 2nd hotkeys
            lblHotkeysOthers.Font = lblHotkeys.Font;
            tx = Strings.Mp3TimeStampHotKey2;
            tx = string.Format(tx, Environment.NewLine);
            lblHotkeysOthers.Text = tx; // "+" + "       " + "Accelerate" + "\r\n" + "-" + "       " + "Slow down" + "\r\n" + "<-" + "      " + "Stop Music";


            // Display mp3 infos
            if (Player.Tag != null)
            {
                if (Player.Tag.Title != null && txtTitle.Text.Trim() == string.Empty)
                    txtTitle.Text = Player.Tag.Title;
                
                if (Player.Tag.Album != null)
                    txtAlbum.Text = Player.Tag.Album;
                
                if (Player.Tag.AlbumArtists.Count() > 0)
                {
                    txtArtist.Text = Player.Tag.AlbumArtists[0].ToString();

                }

                if (Player.Tag.Performers.Count() > 0)
                {
                    txtArtist.Text = Player.Tag.Performers[0].ToString();
                    
                }
                
                if (Player.Tag.Year > 0)
                    txtYear.Text = Player.Tag.Year.ToString();
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
            //tsp = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);
            tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);

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

        /// <summary>
        /// Remove current timestamp in case we have hit ENTER too soon
        /// </summary>
        private void RemoveCurrentLrcTimeStamp()
        {
            if (PlayerState != PlayerStates.Playing || PlayerAppearance != PlayerAppearances.LyricsEditor) return;
            if (dgView.Rows.Count == 1)
            {
                MessageBox.Show("Please add lyrics before entering timestamps", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_index > 0)
            {
                _index--;
                dgView.Rows[_index].Cells[COL_MS].Value = "";
                dgView.Rows[_index].Cells[COL_TIME].Value = "";

                // Select line (paint in blue) and scroll
                dgView.Rows[_index].Selected = true;
                dgView.CurrentCell = dgView.Rows[_index].Cells[0];
            }
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
                    
                    // If editing lyrics: exit
                    if (PlayerAppearance == PlayerAppearances.LyricsEditor && LrcMode == LrcModes.Edit)
                        return;
                                        
                    PlayPauseMusic();
                    break;

                case Keys.Left:
                    if (PlayerState == PlayerStates.Paused)
                        StopMusic();
                    break;

                case Keys.Enter:
                    if (PlayerAppearance == PlayerAppearances.LyricsEditor && LrcMode == LrcModes.Sync && PlayerState == PlayerStates.Playing)
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
                    KeyboardSelectTempo(e);
                    break;

                case Keys.Delete:
                    if (PlayerAppearance == PlayerAppearances.LyricsEditor && PlayerState == PlayerStates.Playing)
                    {
                        // Remove currentline of timestamp in case we have hit ENTER too soon
                        RemoveCurrentLrcTimeStamp();
                    }
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



        #endregion Editor


        #region Rich Textbox

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
        /*
        private void PopulateTextBox(List<List<keffect.KaraokeEffect.kSyncText>> lSyncLyrics)
        {
            string line = string.Empty;
            string tx = string.Empty;
            string cr = "\r\n";
            string Element;

            if (lSyncLyrics == null) return;
            
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
        */

        private void PopulateTextBox2(kLyrics lSyncLyrics)
        {
            string line = string.Empty;
            string tx = string.Empty;
            string cr = "\r\n";
            string Element;

            if (lSyncLyrics == null) return;

            // For each line
            for (int j = 0; j < lSyncLyrics.Lines.Count; j++)
            {
                line = string.Empty;

                // For each item of a line
                for (int i = 0; i < lSyncLyrics.Lines[j].Syllables.Count; i++)
                {
                    Element = lSyncLyrics.Lines[j].Syllables[i].Text;
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
        /*
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
        */
        private kLyrics LoadModifiedLyrics2()
        {

            #region check lines number

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

            #endregion check lines number

            long time;
            string text;
            string cr = "\r\n";
            int iParagraph;
            int iLineFeed;

            kLine SyncLine = new kLine();
            kLyrics lSyncLyrics = new kLyrics();
            Syllable kst;

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
                    if (SyncLine.Syllables.Count > 0)
                        lSyncLyrics.Add(SyncLine);

                    // 1. add empty line for the first cr
                    SyncLine = new kLine();
                    kst = new Syllable("", time);
                    SyncLine.Add(kst);
                    lSyncLyrics.Add(SyncLine);

                    //2. create new line
                    SyncLine = new kLine();

                    // new item with cr for the second cr
                    text = text.Replace(m_SepParagraph, "");
                    text = cr + text;

                }
                // If linefeed
                else if (iLineFeed != -1)
                {
                    // Save previous
                    if (SyncLine.Syllables.Count > 0)
                        lSyncLyrics.Add(SyncLine);

                    // Create new line
                    SyncLine = new kLine();

                    // new item with cr
                    text = text.Replace(m_SepLine, "");
                    text = cr + text;
                }

                // Add new item to the current line 
                text = text.Replace("_", " ");
                kst = new Syllable(text, time); 
                SyncLine.Add(kst);
            }

            // Save last line
            if (SyncLine.Syllables.Count > 0)
                lSyncLyrics.Add(SyncLine);

            return lSyncLyrics;
        }

        #endregion Rich Textbox


        #region functions

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

        /// <summary>
        /// Check if input string is numeric
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsNumeric(string input)
        {
            return int.TryParse(input, out int test);
        }


        #endregion functions


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

            SaveFileDialog = new SaveFileDialog();
            SaveFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            SaveFileDialog.FilterIndex = 1;
            SaveFileDialog.RestoreDirectory = true;
            SaveFileDialog.InitialDirectory = Path.GetDirectoryName(mp3file);
            SaveFileDialog.FileName = Path.GetFileName(mp3file);

            if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {

                string filename = SaveFileDialog.FileName;

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

            // Update tags information            
            string AlbumArtists = txtArtist.Text.Trim();
            string Title = txtTitle.Text.Trim();
            string Album = txtAlbum.Text.Trim();
            uint Year = 0;

            try
            {
                if (IsNumeric(txtYear.Text.Trim()))
                    if (uint.TryParse(txtYear.Text.Trim(), out uint j))
                        Year = (uint)j;
            }
            catch (Exception)
            {
                Year = 0;
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
                    if (dgView.Rows[i].Cells[COL_TEXT].Value != null)
                    {

                        lyric = dgView.Rows[i].Cells[COL_TEXT].Value.ToString();
                        
                        lyric = lyric.Replace(m_SepParagraph, "\n\n");
                        lyric = lyric.Replace(m_SepLine, "\n");
                        
                        lyric = lyric.Replace("_", " ");
                        Mp3LyricsMgmtHelper.MySyncLyricsFrame.Text[i].Text = lyric;
                    }
                }
            }

            

            // Save the frame into the file
            if (Mp3LyricsMgmtHelper.SetTags(FileName, AlbumArtists, Title, Album, Year, Mp3LyricsMgmtHelper.MySyncLyricsFrame))
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
            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);

            localKaraokeLyrics = LoadModifiedLyrics2();
            if (localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);


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
            //object otext;

            string time = string.Empty;
            string tsp = string.Empty;
            string text = string.Empty;

            otime = dgView.CurrentRow.Cells[COL_MS].Value;
            if (otime != null) time = otime.ToString();
            otsp = dgView.CurrentRow.Cells[COL_TIME].Value;
            if (otsp != null) tsp = otsp.ToString();


            int line = dgView.CurrentRow.Index;
            dgView.Rows.Insert(line, time, tsp, text);

            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);

            localKaraokeLyrics = LoadModifiedLyrics2();
            if (localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);


            FileModified();
        }

        /// <summary>
        /// Delete a line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {            
            DeleteSelectedLines();
        }


        private void DeleteSelectedLines()
        {

            if (dgView.SelectedRows.Count == 0) 
            { 
                MessageBox.Show("Please select at least one line to delete", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                foreach (DataGridViewRow row in dgView.SelectedRows)
                {
                    dgView.Rows.RemoveAt(row.Index);
                }
                //localSyncLyrics = LoadModifiedLyrics();
                //if (localSyncLyrics != null)
                //    PopulateTextBox(localSyncLyrics);
                
                localKaraokeLyrics = LoadModifiedLyrics2();
                if (localKaraokeLyrics != null)
                    PopulateTextBox2(localKaraokeLyrics);

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



        #region Major or minor timestamps
        /// <summary>
        /// Add 100 ms to timestamps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMsPlus_Click(object sender, EventArgs e)
        {            

            if (dgView.CurrentRow == null)
                return;
           
            //Declare the menu items and the shortcut menu.
            Button btnSender = (Button)sender;           
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);

            ContextMenuStrip ctButtonAddMs = new ContextMenuStrip();
            var mnuAddMsThisLine = new ToolStripMenuItem(Strings.ThisLine);
            var mnuAddMsAllLines = new ToolStripMenuItem(Strings.AllLines);            
            ctButtonAddMs.Items.AddRange(new ToolStripMenuItem[] { mnuAddMsThisLine, mnuAddMsAllLines });

            mnuAddMsAllLines.Click += mnuAddMsAllLines_Click;
            mnuAddMsThisLine.Click += mnuAddMsThisLine_Click;
                    
            ctButtonAddMs.Show(ptLowerLeft);

        }

        /// <summary>
        /// Add 100 ms to timestamp of the current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAddMsThisLine_Click(object sender, EventArgs e)
        {
            string tsp;
            int Row = dgView.CurrentRow.Index;
            
            if (dgView.Rows[Row].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_MS].Value.ToString()))
            {
                double time = double.Parse(dgView.Rows[Row].Cells[COL_MS].Value.ToString());
                time += m_MillisecondsOffset;
                dgView.Rows[Row].Cells[COL_MS].Value = time;

                tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                dgView.Rows[Row].Cells[COL_TIME].Value = tsp;

                //localSyncLyrics = LoadModifiedLyrics();
                localKaraokeLyrics = LoadModifiedLyrics2();
            }            
        }

        /// <summary>
        /// Add 100 ms to timestamps starting from position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAddMsAllLines_Click(object sender, EventArgs e)
        {
            string tsp;
            int Row = dgView.CurrentRow.Index;

            for (int i = Row; i < dgView.Rows.Count; i++)
            {
                if (dgView.Rows[i].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[i].Cells[COL_MS].Value.ToString()))
                {
                    double time = double.Parse(dgView.Rows[i].Cells[COL_MS].Value.ToString());
                    time += m_MillisecondsOffset;
                    dgView.Rows[i].Cells[COL_MS].Value = time;

                    tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                    dgView.Rows[i].Cells[COL_TIME].Value = tsp;
                }
            }

            //localSyncLyrics = LoadModifiedLyrics();
            localKaraokeLyrics = LoadModifiedLyrics2();

        }



        /// <summary>
        /// Substract 100 ms to timestamps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMsMinor_Click(object sender, EventArgs e)
        {            
            if (dgView.CurrentRow == null)
                return;


            //Declare the menu items and the shortcut menu.
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);

            ContextMenuStrip ctButtonAddMs = new ContextMenuStrip();
            var mnuMinorMsAllLines = new ToolStripMenuItem( Strings.AllLines);
            var mnuMinorMsThisLine = new ToolStripMenuItem(Strings.ThisLine);
            ctButtonAddMs.Items.AddRange(new ToolStripMenuItem[] { mnuMinorMsThisLine, mnuMinorMsAllLines });

            mnuMinorMsAllLines.Click += mnuMinorMsAllLines_Click;
            mnuMinorMsThisLine.Click += mnuMinorMsThisLine_Click;

            ctButtonAddMs.Show(ptLowerLeft);           
        }

        /// <summary>
        /// Substract 100 ms to timestamp of the current line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMinorMsThisLine_Click(object sender, EventArgs e)
        {
            string tsp;
            int Row = dgView.CurrentRow.Index;
           
            if (dgView.Rows[Row].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[Row].Cells[COL_MS].Value.ToString()))
            {
                double time = double.Parse(dgView.Rows[Row].Cells[COL_MS].Value.ToString());
                time -= m_MillisecondsOffset;
                dgView.Rows[Row].Cells[COL_MS].Value = time;

                tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                dgView.Rows[Row].Cells[COL_TIME].Value = tsp;

                //localSyncLyrics = LoadModifiedLyrics();
                localKaraokeLyrics = LoadModifiedLyrics2();
            }
            
        }

        /// <summary>
        /// Substract 100 ms to timestamps starting from position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMinorMsAllLines_Click(object sender, EventArgs e)
        {
            string tsp;
            int Row = dgView.CurrentRow.Index;

            for (int i = Row; i < dgView.Rows.Count; i++)
            {
                if (dgView.Rows[i].Cells[COL_MS].Value != null && IsNumeric(dgView.Rows[i].Cells[COL_MS].Value.ToString()))
                {
                    double time = double.Parse(dgView.Rows[i].Cells[COL_MS].Value.ToString());
                    time -= m_MillisecondsOffset;
                    dgView.Rows[i].Cells[COL_MS].Value = time;

                    tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                    dgView.Rows[i].Cells[COL_TIME].Value = tsp;
                }
            }
            
            //localSyncLyrics = LoadModifiedLyrics();
            localKaraokeLyrics = LoadModifiedLyrics2();
        }

        


        private void btnOffsetPlus_Click(object sender, EventArgs e)
        {
            m_MillisecondsOffset += 100;
            
            btnMsPlus.Text = "+" + m_MillisecondsOffset.ToString() + " ms";
            btnMsMinor.Text = "-" + m_MillisecondsOffset.ToString() + " ms";
        }

        private void btnOffsetMinus_Click(object sender, EventArgs e)
        {
            if (m_MillisecondsOffset > 100)
                m_MillisecondsOffset -= 100;

            btnMsPlus.Text = "+" + m_MillisecondsOffset.ToString() + " ms";
            btnMsMinor.Text = "-" + m_MillisecondsOffset.ToString() + " ms";
        }

        #endregion Major or minor timestamps


        #endregion buttons edition


        #region populate dgView
        

        /// <summary>
        /// Populate gridview with lyrics
        /// </summary>                                     
        private void PopulateDataGridView()
        {
            double time;
            string sTime;
            string text;

            InitGridView();

            // Origine = lrc            
            //List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = Mp3LyricsMgmtHelper.SyncLyrics;
            // Origine = lrc
            kLyrics myKaraokeLyrics = Mp3LyricsMgmtHelper.mp3KaraokeLyrics;

            // Origin = synchronized lyrics frame
            SynchronisedLyricsFrame SynchedLyrics = Mp3LyricsMgmtHelper.MySyncLyricsFrame;

            // Origin = non synchronized lyrics
            string TagLyrics = string.Empty;
            TagLib.Tag Tag = Player.Tag;
            if (Tag != null)
                TagLyrics = Tag.Lyrics;


            // 1. Syncronized lyrics included in the mp3
            if (SynchedLyrics != null && SynchedLyrics.Text.Count() > 0)
            {
                for (int i = 0; i < SynchedLyrics.Text.Count(); i++)
                {
                    // Put "/" everywhere
                    text = SynchedLyrics.Text[i].Text;
                    text = text.Replace("\n\n", m_SepParagraph);
                    text = text.Replace("\r\n", m_SepLine);
                    text = text.Replace("\r", m_SepLine);
                    text = text.Replace("\n", m_SepLine);
                    text = text.Replace(" ", "_");

                    time = SynchedLyrics.Text[i].Time;
                    sTime = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);

                    dgView.Rows.Add(time, sTime, text);
                }
            }
            
            // 2. Lyrics coming from a lrc file
            else if (myKaraokeLyrics != null && myKaraokeLyrics.Lines.Count > 0)
            {
                kLine SyncLine;
                bool bParagraph = false;
                for (int i = 0; i < myKaraokeLyrics.Lines.Count; i++)
                {
                    SyncLine = myKaraokeLyrics.Lines[i];
                    
                    for (int j = 0; j < SyncLine.Syllables.Count; j++)
                    {
                        time = SyncLine.Syllables[j].StartTime; 
                        sTime = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                        text = SyncLine.Syllables[j].Text;
                        if (i > 0 && j == 0)
                        {
                            if (text.Trim() == "")
                            {
                                bParagraph = true;
                                continue;
                            }
                            if (bParagraph)
                            {
                                text = m_SepParagraph + text;
                                bParagraph = false;
                            }
                            else
                                text = m_SepLine + text;
                        }
                        text = text.Replace(" ", "_");
                        dgView.Rows.Add(time, sTime, text);
                    }
                }
            }
            /*
            else if (SyncLyrics != null && SyncLyrics.Count > 0)
            {
                List<keffect.KaraokeEffect.kSyncText> SyncLine;
                bool bParagraph = false;

                for (int i = 0; i < SyncLyrics.Count; i++)
                {
                    SyncLine = SyncLyrics[i];
                    for (int j = 0; j < SyncLine.Count; j++)
                    {
                        time = SyncLine[j].Time;
                        sTime = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                        text = SyncLine[j].Text;
                        if (i > 0 && j == 0)
                        {
                            if (text.Trim() == "")
                            {
                                bParagraph = true;
                                continue;
                            }

                            if (bParagraph)
                            {
                                text = m_SepParagraph + text;
                                bParagraph = false;
                            }
                            else
                                text = m_SepLine + text;                            
                        }                        
                        text = text.Replace(" ", "_");
                        dgView.Rows.Add(time, sTime, text);
                    }
                }               
            }
            */
            // Non synchronized lyrics coming from the mp3 file
            else if (TagLyrics != null && TagLyrics != "")
            {
                string[] lines = TagLyrics.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string line;

                for (int i = 0; i < lines.Count(); i++)
                {
                    line = lines[i].Trim();

                    line = line.Replace(" ", "_");
                    line = m_SepLine + line;
                    dgView.Rows.Add("", "", line);

                }
            }

        }

        /// <summary>
        /// Populate DataGridView with SyncLyrics
        /// </summary>
        /// <param name="SyncLyrics"></param>      
        private void PopulateDataGridView(kLyrics SyncLyrics)
        {
            double time;
            string sTime;
            string text;

            // Reset DataGridView
            InitGridView();

            // Tags
            string TagLyrics = string.Empty;
            TagLib.Tag Tag = Player.Tag;
            if (Tag != null)
                TagLyrics = Tag.Lyrics;

            if (SyncLyrics == null || SyncLyrics.Lines.Count == 0)
            {
                MessageBox.Show("No Lyrics to display", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            kLine SyncLine;
            bool bParagraph = false;

            for (int i = 0; i < SyncLyrics.Lines.Count; i++)
            {
                SyncLine = SyncLyrics.Lines[i];

                for (int j = 0; j < SyncLine.Syllables.Count; j++)
                {
                    time = SyncLine.Syllables[j].StartTime;
                    sTime = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);

                    text = SyncLine.Syllables[j].Text;
                    if (i > 0 && j == 0)
                    {
                        if (text.Trim() == "")
                        {
                            bParagraph = true;
                            continue;
                        }

                        if (bParagraph)
                        {
                            text = m_SepParagraph + text;
                            bParagraph = false;
                        }
                        else
                            text = m_SepLine + text;
                    }

                    text = text.Replace(" ", "_");
                    dgView.Rows.Add(time, sTime, text);
                }
            }

        }


        private void dgView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            NumberRows();
        }

        private void dgView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            NumberRows();
        }

        /// <summary>
        /// Number rows of gridview
        /// </summary>
        private void NumberRows()
        {
            foreach (DataGridViewRow r in dgView.Rows)
            {
                dgView.Rows[r.Index].HeaderCell.Value =
                                    (r.Index + 1).ToString();
            }
        }

        #endregion populate dgView


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
                    //tsp = Mp3LyricsMgmtHelper.MsToTime(time, _LrcMillisecondsDigits);
                    tsp = LyricsUtilities.MsToTime(time, _LrcMillisecondsDigits);
                    dgView.CurrentRow.Cells[COL_TIME].Value = tsp;
                }
            }
            // Modify timestamps
            else if (dgView.CurrentCell.ColumnIndex == COL_TIME && dgView.CurrentRow.Cells[COL_TIME].Value != null)
            {
                otime = dgView.CurrentRow.Cells[COL_TIME].Value;
                sTime = otime.ToString();
                //time = Mp3LyricsMgmtHelper.TimeToMs(sTime);
                time = LyricsUtilities.TimeToMs(sTime);
                dgView.CurrentRow.Cells[COL_MS].Value = time;

            }
            // Modify text
            else if (dgView.CurrentCell.ColumnIndex == COL_TEXT && dgView.CurrentRow.Cells[COL_TEXT].Value != null)
            {
                lyric = dgView.CurrentRow.Cells[COL_TEXT].Value.ToString();
                lyric = lyric.Replace(" ", "_");
                dgView.CurrentRow.Cells[COL_TEXT].Value = lyric;
            }

            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);

            localKaraokeLyrics = LoadModifiedLyrics2();
            if(localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);

            FileModified();
        }

        #endregion dgView edition


        #region dgview context menu

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
                menuInsertTextLine.ShowShortcutKeys = true;
                menuInsertTextLine.ShortcutKeys = Keys.Control | Keys.I;

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
                menuDeleteLine.ShowShortcutKeys = true;
                menuDeleteLine.ShortcutKeys = Keys.Control | Keys.D;


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
            DeleteSelectedLines();
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
            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);
        
            localKaraokeLyrics = LoadModifiedLyrics2();
            if (localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);

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
            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);

            localKaraokeLyrics = LoadModifiedLyrics2();
            if (localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);

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
            //localSyncLyrics = LoadModifiedLyrics();
            //if (localSyncLyrics != null)
            //    PopulateTextBox(localSyncLyrics);

            localKaraokeLyrics = LoadModifiedLyrics2();
            if (localKaraokeLyrics != null)
                PopulateTextBox2(localKaraokeLyrics);

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

                // Trim and add slash at the beginning of each line
                //lines = FormatClipboard(lines);

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
                string plRealTime = string.Empty;                
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
                                    c = c.Replace("\r", "");
                                    c = c.Replace(" ", "_");


                                    if (iCol + i == COL_TEXT && !c.Trim().StartsWith("/"))
                                    {
                                        // If text does not start with a slash, add it
                                        c = "/" + c;
                                    }
                                    
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
                        //localSyncLyrics = LoadModifiedLyrics();
                        //if (localSyncLyrics != null)
                        //    PopulateTextBox(localSyncLyrics);

                        localKaraokeLyrics = LoadModifiedLyrics2();
                        if (localKaraokeLyrics != null)
                            PopulateTextBox2(localKaraokeLyrics);

                        // File was modified
                        FileModified();
                        break;
                    }
            }
        }




        #endregion dgview context menu


        #region aniballs

        /// <summary>
        /// Start balls animation
        /// </summary>
        private void StartTimerBalls()
        {
            Timer3.Interval = 1;
            Timer3.Start();

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                frmMp3Lyrics.StartTimerBalls();
        }

        /// <summary>
        /// Terminate balls animation
        /// </summary>
        private void StopTimerBalls()
        {
            Timer3.Stop();

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
                frmMp3Lyrics.StopTimerBalls();
        }





        #endregion aniballs

     
    }
}
