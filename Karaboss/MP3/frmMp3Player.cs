using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using static Karaboss.Pages.ABCnotation.MyMidi;

namespace Karaboss
{
    public partial class frmMp3Player : Form
    {

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
        private int newstart;
        private int nbstop;
        private int _duration;
        private double _totalSeconds;
        private float _frequency;

        #region Bass
        private bool mBassInitalized = false;
        private int mMP3Stream;
        #endregion Bass


        public frmMp3Player(string FileName, bool bplay)
        {
            InitializeComponent();

            Mp3FullPath = FileName;
            SetTitle(FileName);

            InitControls();

            // If true, launch player
            bPlayNow = bplay;

            // the user asked to play the song immediately                
            if (bPlayNow)
                PlayPauseMusic();

        }

        #region Bass

        /// <summary>
        /// Initialize Bass
        /// </summary>
        private void InitBass()
        {
            //'Add registration key here if you have a license
            BassNet.Registration("fabrice.lacharme@gmail.com", "2X1632326152222");

            try
            {
                // Initalize with frequency = 44100
                Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                mBassInitalized = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initialize the audio playback system. " + ex.Message);
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
                PlayerState = PlayerStates.Stopped;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        #endregion Bass


        #region Form load close resize

        private void frmMp3Player_Load(object sender, EventArgs e)
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

            InitBass();
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
                 Properties.Settings.Default.frmMp3PlayerLocation = Location;
                // Save settings
                Properties.Settings.Default.Save();
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
            StopPlaybackBass();
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
                    if (newstart == 0)                    
                        btnStop.Image = Properties.Resources.btn_red_stop;                    
                    else
                        btnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 

                    pnlDisplay.DisplayStatus("Stopped");
                    break;

                default:
                    break;
            }
        }

        public void FirstPlaySong(int ticks)
        {
            try
            {
                PlayerState = PlayerStates.Playing;
                nbstop = 0;
                BtnStatus();

                if (!InitPlayerMp3(Mp3FullPath)) return;
                StartMp3Player();
                //sequencer1.Start();

                if (ticks > 0)
                {
                    //sequencer1.Position = ticks;
                }

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
                StopMp3Player();
                //sequencer1.Stop();

                // Si point de départ n'est pas le début du morceau
                if (newstart > 0)
                {
                    if (nbstop > 0)
                    {
                        newstart = 0;
                        nbstop = 0;
                        AfterStopped();
                    }
                    else
                    {
                        positionHScrollBar.Value = newstart + positionHScrollBar.Minimum;
                        nbstop = 1;
                    }
                }
                else
                {
                    // Point de départ = début du morceau
                    AfterStopped();
                }
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
                    nbstop = 0;
                    PlayerState = PlayerStates.Playing;
                    BtnStatus();
                    Timer1.Start();
                    //sequencer1.Continue();
                    break;

                default:
                    // First play                
                    FirstPlaySong(newstart);
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
            //sheetmusic.BPlaying = false;

            // Clear all
            //ClearInstruments();

            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);

                positionHScrollBar.Value = positionHScrollBar.Minimum;
                //laststart = 0;
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
        }

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

        }

        private void btnPrev_Click(object sender, EventArgs e)
        {

        }

        private void btnTempoPlus_Click(object sender, EventArgs e)
        {

        }

        private void btnTempoMinus_Click(object sender, EventArgs e)
        {

        }

        private void btnTranspoPlus_Click(object sender, EventArgs e)
        {

        }

        private void btnTranspoMinus_Click(object sender, EventArgs e)
        {

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

        #endregion buttons play stop pause


        #region positionHScrollBar
        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void positionHScrollBar_ValueChanged(object sender, EventArgs e)
        {

        }
        #endregion positionHScrollBar


        #region volume
        private void sldMainVolume_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void sldMainVolume_ValueChanged(object sender, EventArgs e)
        {

        }

        #endregion volume


        #region menus
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
        }




        #endregion Draw controls


        #region mp3 infos

        private bool InitPlayerMp3(string mp3FileName)
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

                    // Get Length
                    // length in bytes
                    long byteslen = Bass.BASS_ChannelGetLength(mMP3Stream, BASSMode.BASS_POS_BYTE);
                    // the time length
                    _totalSeconds = Bass.BASS_ChannelBytes2Seconds(mMP3Stream, byteslen);

                    positionHScrollBar.Maximum = (int)_totalSeconds;

                    TimeSpan t = TimeSpan.FromSeconds(_totalSeconds);
                    string duration = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                    pnlDisplay.DisplayDuration(duration);

                    return true;
                }
                else
                {
                    throw new Exception(String.Format("Stream error: {0}", Bass.BASS_ErrorGetCode()));
                }

            }
            return false;
        }

        private void StartMp3Player()
        {
            if (mMP3Stream == 0) return;
            Bass.BASS_ChannelPlay(mMP3Stream, false);
        }

        /// <summary>
        /// Free resources
        /// </summary>
        private void StopMp3Player()
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

        /// <summary>
        /// Get player position
        /// </summary>
        /// <returns></returns>
        private double Mp3PlayerGetPosition()
        {
            if (mMP3Stream == 0) return 0;

            // length in bytes
            long byteslen = Bass.BASS_ChannelGetPosition(mMP3Stream, BASSMode.BASS_POS_BYTE); 
            // the time length
            return Bass.BASS_ChannelBytes2Seconds(mMP3Stream, byteslen);

            
        }

        #endregion mp3 infos


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
            if (!scrolling)
            {
                // Display time elapse
                double pos = Mp3PlayerGetPosition();
                double dpercent = pos / _totalSeconds;                

                DisplayTimeElapse(dpercent);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        //ScrollView();
                        break;

                    case PlayerStates.Stopped:
                        Timer1.Stop();
                        AfterStopped();
                        break;

                    case PlayerStates.Paused:
                        //sequencer1.Stop();
                        Timer1.Stop();
                        break;
                }

                #region position hscrollbar
                try
                {
                    if (PlayerState == PlayerStates.Playing && (int)pos < positionHScrollBar.Maximum - positionHScrollBar.Minimum)
                    {
                        positionHScrollBar.Value = (int)pos + positionHScrollBar.Minimum;
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("Error positionHScrollBarNew.Value - " + ex.Message);
                }
                #endregion position hscrollbar
            }
        }

        #endregion Timer
    }
}
