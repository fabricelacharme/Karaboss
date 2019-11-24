#region License
// The MIT License (MIT)
// 
// Copyright (c) 2014 Emma 'Eniko' Maassen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TextPlayer;
using System.Threading;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;

namespace Karaboss.Pages.ABCnotation
{

    /*
     *  voir https://github.com/micdah/midi-dot-net pour une dll midi.dll plus récente
     * 
     * Attention, si il manque Q : 90 tous les temps sont à zéro !!!!!!
     * 
     * 
     * 
     * 
     */

    public enum SongFormat {
        MML, ABC
    }



    public partial class FrmTextPlayer : Form {

        #region private decl

        private volatile IMidiPlayer player;
        private object playerLock = new object();
        private volatile bool stopPlaying = false;
        private Thread backgroundThread;
        private int filterIndex = 1;
        private bool? isLotroSong;

        private bool closing = false;
        private bool bClosingRequired = false;
        private bool loading = false; // loading file in progress
        public bool bfilemodified = false;

        private bool bTextEditorAlwaysOn = false;
        private bool bForceShowTextEditor = false;
        //private bool bKaraokeAlwaysOn = true;

        // Dimensions
        private int leftWidth = 179;
        private int SimpleTextPlayerWidth = 850;
        private int SimpleTextPlayerHeight = 170;

        #endregion

        #region controls       
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();
        private Sequence sequence1 = new Sequence();
        #endregion


        public FrmTextPlayer(OutputDevice outdeviceText, string path) {
            InitializeComponent();
            outDevice = outdeviceText;

            txtEditText.Multiline = true;
            txtEditText.WordWrap = false;
            txtEditText.ScrollBars = System.Windows.Forms.ScrollBars.Both;

            using (StreamReader reader = File.OpenText(path))
            {     
                if (Path.GetExtension(path).ToLowerInvariant() == ".abc")
                    LoadFile(reader, SongFormat.ABC);
                else
                    LoadFile(reader, SongFormat.MML);
                lblFile.Text = "File: " + Path.GetFileName(path);                
            }

            txtEditText.Text = LoadSrcFile(path);
        }

        delegate void SetScrollValueDelegate(int val);

        private void SetScrollValue(int val) {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (scrSeek.InvokeRequired) {
                try {
                    SetScrollValueDelegate d = new SetScrollValueDelegate(SetScrollValue);
                    this.Invoke(d, new object[] { val });
                }
                catch { }
            }
            else {
                scrSeek.Value = Math.Max(scrSeek.Minimum, Math.Min(scrSeek.Maximum, val));
                SetTimeText();
            }
        }

        private void SetTimeText() {
            var playerRef = player;
            if (playerRef != null) {
                lblTime.Text = playerRef.Elapsed.ToString("mm':'ss") + " / " + playerRef.Duration.ToString("mm':'ss");
            }
            lblTime.Left = scrSeek.Right - lblTime.Width;
        }

        
        #region Menus

        private void mnuFileNew_Click(object sender, EventArgs e)
        {

        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            OpenTextFile();
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {

        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {

        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {

        }

        private void mnuDisplayText_Click(object sender, EventArgs e)
        {
            DisplayTextEditor();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Open a Text file
        /// </summary>
        private void OpenTextFile()
        {
            var diag = new OpenFileDialog();

            diag.InitialDirectory = ".";
            diag.Filter = "Song files|*.mml;*.abc|MML files (*.mml)|*.mml|ABC files (*.abc)|*.abc|All files (*.*)|*.*";
            diag.FilterIndex = filterIndex;
            diag.RestoreDirectory = true;

            Stream stream = null;
            if (diag.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = diag.OpenFile()) != null)
                    {
                        using (stream)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                if (Path.GetExtension(diag.FileName).ToLowerInvariant() == ".abc")
                                    LoadFile(reader, SongFormat.ABC);
                                else
                                    LoadFile(reader, SongFormat.MML);
                                lblFile.Text = "File: " + Path.GetFileName(diag.FileName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message + System.Environment.NewLine + ex.StackTrace);
                    if (player != null)
                        player.CloseDevice();
                }
            }

            filterIndex = diag.FilterIndex;
        }

        /// <summary>
        /// Load a file and start to play
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="format"></param>
        private void LoadFile(StreamReader reader, SongFormat format)
        {
            StopPlaying();
            stopPlaying = false;

            #region guard
            if (cmbInstruments.SelectedItem == null)
            {
                foreach (var instrument in Enum.GetValues(typeof(Midi.Instrument)))
                {
                    string s = instrument.ToString();
                    cmbInstruments.Items.Add(s);
                    if (s == default(Midi.Instrument).ToString())
                    {
                        cmbInstruments.SelectedItem = s;
                    }
                }
            }
            if (cmbMMLMode.SelectedItem == null)
            {
                cmbMMLMode.SelectedIndex = 0;
            }
            #endregion

            if (format == SongFormat.MML)
            {
                var mml = new PlayerMML(outDevice);
                mml.Settings.MaxDuration = TimeSpan.MaxValue;
                mml.Settings.MaxSize = int.MaxValue;
                mml.Mode = (TextPlayer.MML.MMLMode)Enum.Parse(typeof(TextPlayer.MML.MMLMode), cmbMMLMode.SelectedItem.ToString());
                mml.Load(reader, true);
                player = mml;
                isLotroSong = null;
            }
            else
            {
                var abc = new PlayerABC(outDevice);
                abc.Settings.MaxDuration = TimeSpan.MaxValue;
                abc.Settings.MaxSize = int.MaxValue;
                abc.Load(reader);
                isLotroSong = abc.LotroCompatible;
                abc.LotroCompatible = chkLotroDetect.Checked;
                player = abc;
            }

            player.SetInstrument((Midi.Instrument)Enum.Parse(typeof(Midi.Instrument), cmbInstruments.SelectedItem.ToString()));
            player.Normalize = chkNormalize.Checked;
            player.Loop = chkLoop.Checked;
            player.CalculateNormalization();
            SetTimeText();
            scrSeek.Maximum = (int)Math.Ceiling(player.Duration.TotalSeconds);
            scrSeek.Minimum = 0;
            scrSeek.Value = 0;
            backgroundThread = new Thread(Play);
            backgroundThread.Start();
        }

        private string LoadSrcFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            StreamReader sr = new StreamReader(fi.FullName);
            string s = sr.ReadToEnd();
            s = StringExtensions.ConvertNonDosFile(s);

            return s;
        }


        /// <summary>
        /// Resize according to sequencer visible or not
        /// </summary>
        /// <param name="bSequencerAlwaysOn"></param>
        private void RedimIfSequencerVisible()
        {
            /*
            * Bug quand on veut cacher le sequencer en faisant F12 lorsque
            * bSequencerAlwaysOn = false && bForceShowSequencer = true                           
            * 
            */

            if (bTextEditorAlwaysOn == true)
            {
                this.MaximizeBox = true;
                this.FormBorderStyle = FormBorderStyle.Sizable;

                // Show sequencer
                pnlTop.Visible = true;
                pnlMiddle.Visible = true;

                pnlMiddle.Top = menuStrip1.Height +  pnlTop.Height;
                pnlMiddle.Width = pnlTop.Width;
                pnlMiddle.Height = this.ClientSize.Height - menuStrip1.Height - pnlTop.Height - pnlBottom.Height;
                /*
                pnlEditText.Top = pnlMiddle.Top;
                txtEditText.Top = pnlEditText.Top;
                txtEditText.Height = pnlEditText.Height;
                txtEditText.Width = pnlEditText.Width;
                */

                #region window size & location
                // If window is maximized
                if (Properties.Settings.Default.FrmTextPlayerMaximized)
                {
                    Location = Properties.Settings.Default.FrmTextPlayerLocation;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    try
                    {
                        if (Properties.Settings.Default.FrmTextPlayerSize.Height == SimpleTextPlayerHeight)
                        {
                            this.Size = new Size(Properties.Settings.Default.FrmTextPlayerSize.Width, 600);
                        }
                        else
                            Size = Properties.Settings.Default.FrmTextPlayerSize;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
                #endregion

                if (bTextEditorAlwaysOn == true)
                    mnuDisplayText.Checked = true;
            }
            else
            {

                // Hide sequencer

                // Save size                
                #region save size
                // Copy window location to app settings                
                if (WindowState != FormWindowState.Minimized)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Properties.Settings.Default.FrmTextPlayerLocation = RestoreBounds.Location;
                        Properties.Settings.Default.FrmTextPlayerMaximized = true;

                    }
                    else if (WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.FrmTextPlayerLocation = Location;
                        if (Height != SimpleTextPlayerHeight)
                            Properties.Settings.Default.FrmTextPlayerSize = Size;
                        Properties.Settings.Default.FrmTextPlayerMaximized = false;
                    }

                    // Show sequencer
                    Properties.Settings.Default.ShowTextEditor = bTextEditorAlwaysOn;
                    //Properties.Settings.Default.ShowKaraoke = bKaraokeAlwaysOn;

                    // Save settings
                    Properties.Settings.Default.Save();
                }
                #endregion

                this.MaximizeBox = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;


                pnlTop.Visible = false;
                pnlMiddle.Visible = false;

                if (this.WindowState == FormWindowState.Maximized)
                    WindowState = FormWindowState.Normal;

                // Redim size to simple player
                this.Size = new Size(SimpleTextPlayerWidth, SimpleTextPlayerHeight);

                //mnuDisplaySequencer.Checked = false;
                //MnuEditScore.Checked = false;

                /*
                if (sheetmusic != null)
                    DspEdit(false);

                if (sheetmusic != null)
                    sheetmusic.bEditMode = false;
                */
            }
        }

        private void SaveFileProc()
        {

        }

        private void DisplayTextEditor()
        {
            bTextEditorAlwaysOn = !bTextEditorAlwaysOn;
            // bForceShowSequencer was true, but user decided to hide the sequencer by clicking on the menu
            if (bTextEditorAlwaysOn == false && bForceShowTextEditor == true)
                bForceShowTextEditor = false;

            RedimIfSequencerVisible();
        }


        #endregion



        #region form load unload
        private void FrmTextPlayer_Load(object sender, EventArgs e) {

            #region set values
            foreach (var instrument in Enum.GetValues(typeof(Midi.Instrument)))
            {
                string s = instrument.ToString();
                cmbInstruments.Items.Add(s);
                if (s == default(Midi.Instrument).ToString())
                {
                    cmbInstruments.SelectedItem = s;
                }
            }

            cmbMMLMode.SelectedIndex = 0;
            #endregion

            // Set window location and size
            #region window size & location
            // If window is maximized
            if (Properties.Settings.Default.FrmTextPlayerMaximized)
            {
                Location = Properties.Settings.Default.FrmTextPlayerLocation;                
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.FrmTextPlayerLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmPlayerSize;
            }
            #endregion

            // Ne pas tenir compte si new file ou edit file
            bTextEditorAlwaysOn = Properties.Settings.Default.ShowTextEditor;

            // Redim form according to the visibility of the sequencer
            RedimIfSequencerVisible();

        }

        protected override void OnClosed(EventArgs e)
        {
            StopPlaying();

            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();

            base.OnClosed(e);
        }

        private void FrmTextPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (loading)
            {
                e.Cancel = true;
            }
            else
            {
                if (bfilemodified == true)
                {
                    string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                    if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        e.Cancel = true;
                        // turlututu
                        bClosingRequired = true;
                        SaveFileProc();
                        return;
                    }
                }

                // enregistre la taille et la position de la forme
                // Copy window location to app settings                
                if (WindowState != FormWindowState.Minimized)
                {
                    if (WindowState == FormWindowState.Maximized)
                    {
                        Properties.Settings.Default.FrmTextPlayerLocation = RestoreBounds.Location;
                        Properties.Settings.Default.FrmTextPlayerMaximized = true;

                    }
                    else if (WindowState == FormWindowState.Normal)
                    {
                        Properties.Settings.Default.FrmTextPlayerLocation = Location;

                        // SDave only if not default size
                        if (Height != SimpleTextPlayerHeight)
                            Properties.Settings.Default.FrmTextPlayerSize = Size;

                        Properties.Settings.Default.FrmTextPlayerMaximized = false;
                    }

                    // Show sequencer
                    Properties.Settings.Default.ShowTextEditor = bTextEditorAlwaysOn;
                    //Properties.Settings.Default.ShowKaraoke = bKaraokeAlwaysOn;

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
        }


        private void FrmTextPlayer_Resize(object sender, EventArgs e)
        {
            if (pnlMiddle.Visible)
            {
                pnlMiddle.Top = menuStrip1.Height + pnlTop.Height;
                pnlMiddle.Width = pnlTop.Width;
                pnlMiddle.Height = this.ClientSize.Height - menuStrip1.Height - pnlTop.Height - pnlBottom.Height;

            }
        }

        #endregion



        #region Buttons Play Pause Stop

        private void btnPlay_Click(object sender, EventArgs e)
        {
            lock (playerLock)
            {
                if (player != null)
                {
                    if (player.Playing && !player.Paused)
                        player.Stop();
                    player.Play(new TimeSpan(MusicPlayer.Time.Ticks));
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            lock (playerLock)
            {
                if (player != null)
                {
                    player.Stop();
                }
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            lock (playerLock)
            {
                if (player != null)
                {
                    if (player.Paused)
                        player.Unpause();
                    else if (player.Playing)
                        player.Pause();
                }
            }
        }

        private void StopPlaying()
        {
            stopPlaying = true;
            if (backgroundThread != null)
            {
                backgroundThread.Join(TimeSpan.FromSeconds(1));
            }
        }

        private void Play()
        {
            try
            {
                TimeSpan now = new TimeSpan(MusicPlayer.Time.Ticks);
                player.Play(now);

                while (player != null && !stopPlaying)
                {
                    lock (playerLock)
                    {
                        if (player.Playing)
                        {
                            player.Update(now);
                            int scrollVal = (int)player.Elapsed.TotalSeconds;
                            if (scrollVal != scrSeek.Value)
                            {
                                SetScrollValue(scrollVal);
                            }
                        }
                        else
                        {
                            if (scrSeek.Value != 0)
                            {
                                SetScrollValue(0);
                            }
                        }
                    }
                    Thread.Sleep(1);
                    now = new TimeSpan(MusicPlayer.Time.Ticks);
                }

                lock (playerLock)
                {
                    if (player.Playing)
                    {
                        player.Stop();
                        //Console.WriteLine("Closed while playing, stopPlaying: " + stopPlaying + ", player was null: " + (player == null));
                    }
                    else
                    {
                        //Console.WriteLine("Closed due to done playing");
                    }
                }
            }
#if DEBUG
            catch (Exception e)
            {
                Console.WriteLine("Background thread terminated: " + e.ToString());
            }
#else
            catch {
            }
#endif
            finally
            {
                player.CloseDevice();
                player = null;
            }
        }


        #endregion

        #region Others

        private void cmbInstruments_SelectionChangeCommitted(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null)
                    player.SetInstrument((Midi.Instrument)Enum.Parse(typeof(Midi.Instrument), cmbInstruments.SelectedItem.ToString()));
            }
        }

        private void cmbMMLMode_SelectionChangeCommitted(object sender, EventArgs e) {
            lock (playerLock) {
                var mmlPlayer = player as PlayerMML;
                if (mmlPlayer != null) {
                    mmlPlayer.Mode = (TextPlayer.MML.MMLMode)Enum.Parse(typeof(TextPlayer.MML.MMLMode), cmbMMLMode.SelectedItem.ToString());
                    mmlPlayer.CalculateNormalization();
                    mmlPlayer.RecalculateDuration();
                }
            }
        }

        private void chkNormalize_CheckedChanged(object sender, EventArgs e) {
            var playerRef = player;
            if (playerRef != null)
                playerRef.Normalize = chkNormalize.Checked;
        }

        private void chkLoop_CheckedChanged(object sender, EventArgs e) {
            var playerRef = player;
            if (playerRef != null)
                playerRef.Loop = chkLoop.Checked;
        }


        private void chkMute_CheckedChanged(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    player.Muted = chkMute.Checked;
                }
            }
        }

        private void scrSeek_MouseDown(object sender, MouseEventArgs e) {
            lock (playerLock) {
                if (player != null) {
                    double perc = e.X / (double)scrSeek.Width;
                    int seconds = (int)(perc * player.Duration.TotalSeconds);
                    player.Seek(new TimeSpan(MusicPlayer.Time.Ticks), TimeSpan.FromSeconds(seconds));
                    SetScrollValue(seconds + 1);
                    SetScrollValue(seconds);
                }
            }
        }


        private void chkLotroDetect_CheckedChanged(object sender, EventArgs e) {
            lock (playerLock) {
                if (player != null && isLotroSong.HasValue && isLotroSong.Value) {
                    var abc = player as PlayerABC;
                    if (abc != null) {
                        abc.LotroCompatible = chkLotroDetect.Checked;
                    }
                }
            }
        }


        #endregion

      
    }
}
