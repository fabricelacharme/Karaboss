#region License

/* Copyright (c) 2024 Fabrice Lacharme
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
using ChordAnalyser.UI;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmChords : Form
    {

        #region private
        private bool closing = false;
        private bool scrolling = false;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        private int bouclestart = 0;
        private int newstart = 0;
        private int laststart = 0;      // Start time to play
        private int nbstop = 0;


        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,
            NextSong,
            Waiting,
            WaitingPaused
        }
        private PlayerStates PlayerState;

        #region controls
        private Sequence sequence1 = new Sequence();
        private ChordsControl chordAnalyserControl1;
        private ChordsMapControl ChordMapControl1;
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();

        private System.Windows.Forms.Timer timer1;
        private Karaboss.NoSelectButton btnPlay;
        private Karaboss.NoSelectButton btnRewind;
        private Karaboss.NoSelectButton btnZoomPlus;
        private Karaboss.NoSelectButton btnZoomMinus;

        private Label lblNumMeasure;
        private Label lblElapsed;
        private Label lblPercent;
        private Label lblBeat;

        #endregion controls

        //private Panel pnlTop;
        private Panel pnlDisplayHorz;       // chords in horizontal mode
        private Panel pnlBottom;
        
        private Panel pnlDisplayMap = new Panel();        // chords in map mode
        //private Panel pnlBottomMap = new Panel();

        private ColorSlider.ColorSlider positionHScrollBar;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;
        private int _currentMeasure = -1;
        private int _currentTimeInMeasure = -1;
        private int _currentLine = 1;

        #endregion private


        public frmChords(OutputDevice OtpDev, string FileName)
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);

            // Sequence
            //LoadSequencer(seq);
            outDevice = OtpDev;

            // Allow form keydown
            this.KeyPreview = true;

            // Title
            SetTitle(FileName);

            UpdateMidiTimes();
            
        }

        #region Display Controls

        /// <summary>
        /// Sets title of form
        /// </summary>
        /// <param name="fileName"></param>
        private void SetTitle(string fileName)
        {
            Text = "Karaboss - " + Path.GetFileName(fileName);
        }

        private void LoadSequencer(Sequence seq)
        {
            try
            {
                sequence1 = seq;

                sequencer1 = new Sequencer();
                sequencer1.Position = 0;
                sequencer1.Sequence = sequence1;    // primordial !!!!!
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                UpdateMidiTimes();

                //TempoOrig = _tempo;
                //lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

                // Display
                int Min = (int)(_duration / 60);
                int Sec = (int)(_duration - (Min * 60));
                //lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

                // PlayerState = stopped
                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DrawControls()
        {
            // Timer
            timer1 = new Timer();
            timer1.Interval = 20;
            timer1.Tick += new EventHandler(timer1_Tick);

            #region Toolbar

            pnlToolbar.BackColor = Color.FromArgb(70, 77, 95);

            // Button Rewind
            btnRewind = new NoSelectButton();
            btnRewind.FlatAppearance.BorderSize = 0;
            btnRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatStyle = FlatStyle.Flat;
            btnRewind.Parent = pnlToolbar;
            btnRewind.Location = new Point(2, 2);
            btnRewind.Size = new Size(50, 50);
            btnRewind.Image = Properties.Resources.btn_black_prev;
            btnRewind.Click += new EventHandler(btnRewind_Click);
            btnRewind.MouseHover += new EventHandler(btnRewind_MouseHover);
            btnRewind.MouseLeave += new EventHandler(btnRewind_MouseLeave);
            pnlToolbar.Controls.Add(btnRewind);

            // Button play
            btnPlay = new NoSelectButton();
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnPlay.Parent = pnlToolbar;
            btnPlay.Location = new Point(2 + btnRewind.Width, 2);
            btnPlay.Size = new Size(50, 50);
            btnPlay.Image = Properties.Resources.btn_black_play;
            btnPlay.Click += new EventHandler(btnPlay_Click);
            btnPlay.MouseHover += new EventHandler(btnPlay_MouseHover);
            btnPlay.MouseLeave += new EventHandler(btnPlay_MouseLeave);
            pnlToolbar.Controls.Add(btnPlay);


            btnZoomPlus = new NoSelectButton();
            btnZoomPlus.Parent = pnlToolbar;
            btnZoomPlus.Location = new Point(2 + btnPlay.Left + btnPlay.Width, 2);
            btnZoomPlus.Size = new Size(50, 50);
            btnZoomPlus.Text = "+";
            btnZoomPlus.Click += new EventHandler(btnZoomPlus_Click);
            pnlToolbar.Controls.Add(btnZoomPlus);

            btnZoomMinus = new NoSelectButton();
            btnZoomMinus.Parent = pnlToolbar;
            btnZoomMinus.Location = new Point(2 + btnZoomPlus.Left + btnZoomPlus.Width, 2);
            btnZoomMinus.Size = new Size(50, 50);
            btnZoomMinus.Text = "-";
            btnZoomMinus.Click += new EventHandler(btnZoomMinus_Click);
            pnlToolbar.Controls.Add(btnZoomMinus);


            lblNumMeasure = new Label();
            lblNumMeasure.Location = new Point(100 + btnZoomMinus.Left + btnZoomMinus.Width, 2 );
            lblNumMeasure.Text = "measure";
            lblNumMeasure.Parent = pnlToolbar;
            pnlToolbar.Controls.Add(lblNumMeasure);

            lblElapsed = new Label();
            lblElapsed.Location = new Point(100 + lblNumMeasure.Left + lblNumMeasure.Width, 2);
            lblElapsed.Text = "00:00";
            lblElapsed.Parent = pnlToolbar;
            pnlToolbar.Controls.Add(lblElapsed);

            lblPercent = new Label();
            lblPercent.Location = new Point(100 + lblElapsed.Left + lblElapsed.Width, 2);
            lblPercent.Text = "0%";
            lblPercent.Parent = pnlToolbar;
            pnlToolbar.Controls.Add(lblPercent);

            lblBeat = new Label();
            lblBeat.Location = new Point(100 + lblPercent.Left + lblPercent.Width, 1);
            lblBeat.Text = "1";
            lblBeat.Parent = pnlToolbar;
            pnlToolbar.Controls.Add(lblBeat);


            #endregion Toolbar


            #region 1er TAB                    

            #region Panel Display horizontal chords
            pnlDisplayHorz = new Panel();
            pnlDisplayHorz.Parent = tabPageDiagrams;
            pnlDisplayHorz.Location = new Point(tabPageDiagrams.Margin.Left, tabPageDiagrams.Margin.Top);            
            pnlDisplayHorz.Size = new Size(tabPageDiagrams.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right, 150);
            //pnlDisplayHorz.BackColor = Color.FromArgb(70, 77, 95);            
            pnlDisplayHorz.BackColor = Color.Chocolate;
            tabPageDiagrams.Controls.Add(pnlDisplayHorz);
            #endregion Panel Display horizontal chords
            

            #region ChordControl
            chordAnalyserControl1 = new ChordsControl();
            chordAnalyserControl1.Parent = pnlDisplayHorz;
            chordAnalyserControl1.Sequence1 = this.sequence1;
            chordAnalyserControl1.Size = new Size(pnlDisplayHorz.Width, 80);
            chordAnalyserControl1.Location = new Point(0, 0);
            chordAnalyserControl1.WidthChanged += new WidthChangedEventHandler(chordAnalyserControl1_WidthChanged);
            chordAnalyserControl1.HeightChanged += new HeightChangedEventHandler(chordAnalyserControl1_HeightChanged);
            chordAnalyserControl1.MouseDown += new MouseEventHandler(chordAnalyserControl1_MouseDown); 
            chordAnalyserControl1.Cursor = Cursors.Hand;
            pnlDisplayHorz.Controls.Add(chordAnalyserControl1);
            #endregion


            #region positionHScrollBar
            positionHScrollBar = new ColorSlider.ColorSlider();
            positionHScrollBar.Parent = pnlDisplayHorz;
            positionHScrollBar.ThumbImage = Properties.Resources.BTN_Thumb_Blue;
            positionHScrollBar.Size = new Size(pnlDisplayHorz.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right, 20);
            positionHScrollBar.Location = new Point(0, chordAnalyserControl1.Height);
            positionHScrollBar.Value = 0;
            positionHScrollBar.Minimum = 0;

            // Set maximum & visibility
            SetScrollBarValues();

            positionHScrollBar.TickStyle = TickStyle.None;
            positionHScrollBar.SmallChange = 1;
            positionHScrollBar.LargeChange = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.ShowDivisionsText = false;
            positionHScrollBar.ShowSmallScale = false;
            positionHScrollBar.MouseWheelBarPartitions = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(PositionHScrollBar_Scroll);
            pnlDisplayHorz.Controls.Add(positionHScrollBar);

            pnlDisplayHorz.Height = chordAnalyserControl1.Height + positionHScrollBar.Height;

            #endregion


            #region Panel Bottom
            pnlBottom = new Panel();
            pnlBottom.Parent = this.tabPageDiagrams;
            pnlBottom.Location = new Point(tabPageDiagrams.Margin.Left, pnlDisplayHorz.Top + pnlDisplayHorz.Height);
            //pnlBottom.Size = new Size(tabPageDiagrams.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right, tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height);
            pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height;
            pnlBottom.BackColor = Color.Red;
            pnlBottom.Dock = DockStyle.Bottom;
            //pnlBottom.Dock = DockStyle.Fill;
            tabPageDiagrams.Controls.Add(pnlBottom);


            #endregion

            #endregion 1er TAB


            #region 2eme TAB

            /*
            #region display bottom map            
            pnlBottomMap = new Panel();
            pnlBottomMap.Parent = tabPageOverview;
            pnlBottom.Height = 80;
            pnlBottomMap.Dock = DockStyle.Bottom;
            tabPageOverview.Controls.Add(pnlBottomMap);
            #endregion display bottom map
            */

            #region display map chords
            pnlDisplayMap = new Panel();
            pnlDisplayMap.Parent = tabPageOverview;
            pnlDisplayMap.Location = new Point(tabPageOverview.Margin.Left, tabPageOverview.Margin.Top);
            //pnlDisplayMap.Size = new Size(tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right, tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom);
            //pnlDisplayMap.Dock = DockStyle.Fill;
            pnlDisplayMap.BackColor = Color.White;
            pnlDisplayMap.AutoScroll = true;            
            tabPageOverview.Controls.Add(pnlDisplayMap);
            #endregion display map chords


            #region ChordMapControl
            ChordMapControl1 = new ChordsMapControl();
            ChordMapControl1.Parent = pnlDisplayMap;
            ChordMapControl1.Location = new Point(0, 0);            
            //ChordMapControl1.Size = new Size(pnlDisplayMap.Width, pnlDisplayMap.Height);
            //ChordMapControl1.Size = new Size(ChordMapControl1.Width, ChordMapControl1.Height);
            //ChordMapControl1.Dock = DockStyle.Fill;
            ChordMapControl1.WidthChanged += new MapWidthChangedEventHandler(ChordMapControl1_WidthChanged);
            ChordMapControl1.HeightChanged += new MapHeightChangedEventHandler(ChordMapControl1_HeightChanged);            
            ChordMapControl1.MouseDown += new MouseEventHandler(ChordMapControl1_MouseDown);
            ChordMapControl1.Cursor = Cursors.Hand;
            ChordMapControl1.Sequence1 = this.sequence1;
            pnlDisplayMap.Controls.Add(ChordMapControl1);


            #endregion ChordMapControl


            #endregion 2eme TAB

 
        }


        #endregion Display Controls       



        #region timer

        /// <summary>
        /// Timer tick management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!scrolling)
            {
                // Display time elapse               
                DisplayTimeElapse(sequencer1.Position);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        // first page
                        DisplayPositionHScrollBar(sequencer1.Position);
                        DisplayPositionVScrollbar(sequencer1.Position);
                        break;

                    case PlayerStates.Stopped:
                        timer1.Stop();
                        AfterStopped();
                        break;

                    case PlayerStates.Paused:
                        sequencer1.Stop();                        
                        timer1.Stop();
                        break;
                }

               
            }
        }


        #endregion timer


        #region Scroll ChordsControl 

       

        /// <summary>
        /// Get time inside measure
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public float GetTimeInMeasure(int ticks)
        {
            // Num measure
            int curmeasure = 1 + ticks / _measurelen;
            // Temps dans la mesure
            float timeinmeasure = sequence1.Numerator - ((curmeasure * _measurelen - ticks) / (float)(_measurelen / sequence1.Numerator));

            return timeinmeasure;
        }

        #endregion Scroll ChordsControl


        #region handle messages

        /// <summary>
        /// Event: loading of midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //loading = true;
            try
            {
                //toolStripProgressBarPlayer.Value = e.ProgressPercentage;
                //progressBarPlayer.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: loading of midi file terminated: launch song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            LoadSequencer(sequence1);
            
            DrawControls();

            DisplayResults();

            
        }

        /// <summary>
        /// Load the midi file in the sequencer
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncFile(string fileName)
        {
            try
            {
                //progressBarPlayer.Visible = true;

                ResetSequencer();
                if (fileName != "\\")
                {
                    sequence1.LoadAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);             
            }
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            outDevice.Send(e.Message);

        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            _currentMeasure = -1;
            PlayerState = PlayerStates.Stopped;
        }

        #endregion handle messages


        #region buttons
        private void btnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_pause;

        }

        private void btnRewind_MouseHover(object sender, EventArgs e)
        {
            if(PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnRewind.Image = Properties.Resources.btn_blue_prev;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_green_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_pause;

        }

        private void btnRewind_MouseLeave(object sender, EventArgs e)
        {
            btnRewind.Image = Properties.Resources.btn_black_prev;
        }

        private void btnZoomMinus_Click(object sender, EventArgs e)
        {
            chordAnalyserControl1.zoom -= (float)0.1;
            ChordMapControl1.zoom -= (float)0.1;
        }

        private void btnZoomPlus_Click(object sender, EventArgs e)
        {
            chordAnalyserControl1.zoom += (float)0.1;
            ChordMapControl1.zoom += (float)0.1;
        }

        #endregion buttons


        #region Events
        private void chordAnalyserControl1_HeightChanged(object sender, int value)
        {
            positionHScrollBar.Location = new Point(0, chordAnalyserControl1.Height);
            pnlDisplayHorz.Height = chordAnalyserControl1.Height + positionHScrollBar.Height;

            pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height;

        }

        private void ChordMapControl1_HeightChanged(object sender, int value)
        {
            //ChordMapControl1.Size = new Size(ChordMapControl1.Width, ChordMapControl1.Height);

            if (pnlDisplayMap != null)
            {
                //Console.WriteLine(pnlDisplayMap.Width.ToString());
                pnlDisplayMap.Width = tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right;                
                pnlDisplayMap.Height = tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom - 50;
            }

        }

        private void ChordMapControl1_WidthChanged(object sender, int value)
        {
            //ChordMapControl1.Size = new Size(ChordMapControl1.Width, ChordMapControl1.Height);


            if (pnlDisplayMap != null)
            {
                //Console.WriteLine(pnlDisplayMap.Width.ToString());
                pnlDisplayMap.Width = tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right;
                pnlDisplayMap.Height = tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom - 50;
            }
        }

        private void ChordMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void chordAnalyserControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.Location.X;

                newstart = (int)(((chordAnalyserControl1.OffsetX + x) / (float)chordAnalyserControl1.Width) * sequence1.GetLength());
                FirstPlaySong(newstart);


            }
        }
        #endregion Events


        #region PositionVScrollbar

        /// <summary>
        /// Display vertical scrollbar in 2nd page
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayPositionVScrollbar(int pos)
        {          
            // which line ?                                
            int curline = (int)(Math.Ceiling((double)(_currentMeasure + 1) / ChordMapControl1.NbColumns));

            // Change line => offset Chord map
            if (curline != _currentLine)
            {
                _currentLine = curline;
                int LargeurCellule = (int)(chordAnalyserControl1.CellSize) + 1;

                // if control is higher then the panel => scroll
                if (ChordMapControl1.maxStaffHeight > pnlDisplayMap.Height)
                {
                    // offset vertical: ensure to see 2 lines
                    //ChordMapControl1.OffsetY = LargeurCellule * (curline - 2); 
                    int offset = LargeurCellule * (curline - 2);


                    if (pnlDisplayMap.VerticalScroll.Visible && pnlDisplayMap.VerticalScroll.Minimum <= offset && offset <= pnlDisplayMap.VerticalScroll.Maximum)
                        pnlDisplayMap.VerticalScroll.Value = LargeurCellule * (curline - 2);
                }
            }
        }

        #endregion PositionVScrollbar

        #region positionHSCrollBar

        /// <summary>
        /// Display horizontal scrollbar in first page
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayPositionHScrollBar(int pos)
        {
            // pos is in which measure?
            int curmeasure = 1 + pos / _measurelen;                        

            // Quel temps dans la mesure ?
            int timeinmeasure = sequence1.Numerator - (int)((curmeasure * _measurelen - pos) / (_measurelen / sequence1.Numerator));

            lblBeat.Text = timeinmeasure.ToString();

            // change time in measure => draw cell in control
            if (timeinmeasure != _currentTimeInMeasure)
            {
                _currentTimeInMeasure = timeinmeasure;
                
                // Draw gray cell for played note
                chordAnalyserControl1.DisplayNotes(pos, curmeasure, timeinmeasure);
                ChordMapControl1.DisplayNotes(pos,curmeasure,timeinmeasure);
            }

            // Change measure => offset control
            if (curmeasure != _currentMeasure)
            {

                //if (curmeasure != _currentMeasure)
                _currentMeasure = curmeasure;

                int val = 0;
                
                int LargeurCellule = (int)(chordAnalyserControl1.CellSize) + 1;
                int LargeurMesure = LargeurCellule * sequence1.Numerator; // keep one measure on the left
                int offsetx = LargeurCellule + (_currentMeasure - 1) * (LargeurMesure);                    
                
                int course = (int)(positionHScrollBar.Maximum - positionHScrollBar.Minimum);
                int CellsNumber = 1 + NbMeasures * sequence1.Numerator;

                // La première case ne sert qu'à l'affichage
                // La position de la scrollbar doit tenir compte de la première case
                // % de Largeur Cellule par rapport à la course de la scrollbar ?
                // On dessine toutes ces cases : 1 + NbMeasures * Sequence1.Numerator
                // Course de la scrollbar = Largeur1ereCellule +  NbMeasures * sequence1.Numerator * LargeurCellule
                // soit : Course = LargeurCellule * (NbMeasures * sequence1.Numerator + 1)
                // val = Largeur1ereCellule + (int)((_currentMeasure/(float)NbMeasures) * course);
                // Largeur1ereCellule = Course/CellsNumber
                val = (course / CellsNumber) + (int)((_currentMeasure / (float)NbMeasures) * course);

                if (positionHScrollBar.Minimum <= val && val <= positionHScrollBar.Maximum)
                    positionHScrollBar.Value = val;


                // ensure to Keep 1 measure on the left
                if (chordAnalyserControl1.maxStaffWidth > pnlDisplayHorz.Width)
                {                    
                    // offset horizontal
                    if (offsetx > LargeurMesure)
                    {
                        if (offsetx < chordAnalyserControl1.maxStaffWidth - pnlDisplayHorz.Width)
                        {
                            chordAnalyserControl1.OffsetX = offsetx - LargeurMesure;
                        }
                        else
                        {
                            chordAnalyserControl1.OffsetX = chordAnalyserControl1.maxStaffWidth - pnlDisplayHorz.Width;
                        }
                    }                    
                }
                
                lblNumMeasure.Text = "Measure: " + _currentMeasure;
            }
        }

        private void SetScrollBarValues()
        {
            if (pnlDisplayHorz == null || chordAnalyserControl1 == null)
                return;

            // Width of control
            int W = chordAnalyserControl1.maxStaffWidth;

            if (W <= pnlDisplayHorz.Width)
            {
                positionHScrollBar.Visible = false;
                positionHScrollBar.Maximum = 0;
                chordAnalyserControl1.OffsetX = 0;
                positionHScrollBar.Value = 0;
            }
            else if (W > pnlDisplayHorz.Width)
            {
                positionHScrollBar.Maximum = W - pnlDisplayHorz.Width;
                positionHScrollBar.Visible = true;
            }

        }

        /// <summary>
        /// Scroll horizontal scrollbar: move sequencer position to scrollbar value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {            
            chordAnalyserControl1.OffsetX = e.NewValue;

            if (e.Type == ScrollEventType.EndScroll)
            {
                // scrollbar position = fraction  of sequence Length
                float n =  (e.NewValue / (float)(positionHScrollBar.Maximum - positionHScrollBar.Minimum)) * sequence1.GetLength();
                newstart = (int)n;                

                sequencer1.Position = newstart;
                scrolling = false;
            }
            else if (e.Type != ScrollEventType.First)
            {
                // Explain: remove ScrollEventType.First when using the keyboard to pause, start, rewind
                // Without this, scrolling is set to true
                scrolling = true;
            }
        }

      

        /// <summary>
        /// Set positionHScrollbar Width equal to chord control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void chordAnalyserControl1_WidthChanged(object sender, int value)
        {
            positionHScrollBar.Width = (pnlDisplayHorz.Width > chordAnalyserControl1.Width ? chordAnalyserControl1.Width : pnlDisplayHorz.Width);

            // Set maximum & visibility
            SetScrollBarValues();
        }

        #endregion positionHScrollBar


        #region Display results

        private void DisplayResults()
        {

            // Display chords in the textbox
            ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);
            Dictionary<int, (string, string)> Gridchords = Analyser.Gridchords;

            /*
            string res = string.Empty;
            foreach (KeyValuePair<int, (string, string)> pair in Gridchords)
            {
                res += string.Format("{0} - {1}", pair.Key, pair.Value) + "\r\n";
            }
            */

            //Change labels displayed
            for (int i = 1; i <= Gridchords.Count; i++)
            {
                Gridchords[i] = (InterpreteNote(Gridchords[i].Item1), InterpreteNote(Gridchords[i].Item2));
            }

            // Display Chords in boxes
            chordAnalyserControl1.Gridchords = Gridchords;
            ChordMapControl1.Gridchords = Gridchords;

        }


        private string InterpreteNote(string note)
        {                      
            note = note.Replace("sus", "");

            note = note.Replace(" major", "");
            note = note.Replace(" triad", "");
            note = note.Replace("dominant", "");

            note = note.Replace("first inversion", "");
            note = note.Replace("second inversion", "");
            note = note.Replace("third inversion", "");

            note = note.Replace(" seventh", "7");
            note = note.Replace(" minor", "m");
            note = note.Replace("seventh", "7");
            note = note.Replace("sixth", "6");
            note = note.Replace("ninth", "9");
            note = note.Replace("eleventh", "11");

            note = note.Replace("6", "");
            note = note.Replace("9", "");
            note = note.Replace("11", "");


            note = note.Replace("<Chord not found>", "?");


            note = note.Trim();
            return note;
        }

        #endregion Display results


        #region Form load close

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {            
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();
            base.OnClosed(e);
        }

        /// <summary>
        /// Override form load event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            sequence1.LoadProgressChanged += HandleLoadProgressChanged;
            sequence1.LoadCompleted += HandleLoadCompleted;

            LoadAsyncFile(MIDIfileFullPath);

            base.OnLoad(e);
        }

        private void frmChords_Load(object sender, EventArgs e)
        {
            #region setwindowlocation
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmChordsMaximized)
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmChordsSize;
                
            }
            #endregion

            this.Activate();
        }

        private void frmChords_Resize(object sender, EventArgs e)
        {
            // Controle onglets
            tabChordsControl.Top = pnlToolbar.Height;
            tabChordsControl.Width = this.ClientSize.Width;
            tabChordsControl.Height = this.ClientSize.Height - pnlToolbar.Height;

            // Bug: only the selected TabPage is resized, but not others 
            for (int i = 0;i< tabChordsControl.TabCount;i++)
            {
                if (i != tabChordsControl.SelectedIndex)
                {
                    // Fore other tabs to redim
                    tabChordsControl.TabPages[i].Width = tabChordsControl.TabPages[tabChordsControl.SelectedIndex].Width;
                    tabChordsControl.TabPages[i].Height = tabChordsControl.TabPages[tabChordsControl.SelectedIndex].Height;
                }
            }

            if (pnlDisplayHorz != null)
            {
                pnlDisplayHorz.Width = tabPageDiagrams.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right;
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height;
            }

            if (pnlDisplayMap != null)
            {
                pnlDisplayMap.Width = tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right;
                pnlDisplayMap.Height = tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom - 50;
            }


            if (chordAnalyserControl1 != null)
            {
                positionHScrollBar.Width = (pnlDisplayHorz.Width > chordAnalyserControl1.Width ? chordAnalyserControl1.Width : pnlDisplayHorz.Width);
                positionHScrollBar.Top = chordAnalyserControl1.Top + chordAnalyserControl1.Height;
            }

                        
            // Set maximum & visibility
            SetScrollBarValues();
        }

        private void frmChords_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmChordsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmChordsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmChordsLocation = Location;
                    Properties.Settings.Default.frmChordsSize = Size;
                    Properties.Settings.Default.frmChordsMaximized = false;
                }

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

        /// <summary>
        /// Key Up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmChords_KeyUp(object sender, KeyEventArgs e)
        {            
            switch (e.KeyCode)
            {
                case Keys.Space:
                    PlayPauseMusic();
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
            if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped && newstart > 0))
            {
                if (keyData == Keys.Left)
                {
                    Rewind();
                    return true;
                }               
            }

        return base.ProcessCmdKey(ref msg, keyData);

        }
        #endregion Form


        #region Midi

            /// <summary>
            /// Upadate MIDI times
            /// </summary>
            private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds            

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                NbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer
            }
        }


        #endregion Midi


        #region Play stop pause

        private void ResetSequencer()
        {
            if (timer1 != null)
                timer1.Stop();
            scrolling = false;
            newstart = 0;
            laststart = 0;
            _currentMeasure = -1;
            
            if (sequencer1 != null) 
                sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            Rewind();
        }

        private void Rewind()
        {
            scrolling = false;
            newstart = 0;
            StopMusic();
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
                    scrolling = false;
                    PlayerState = PlayerStates.Playing;
                    BtnStatus();
                    timer1.Start();
                    sequencer1.Continue();
                    break;

                default:
                    // First play                
                    FirstPlaySong(newstart);
                    break;
            }
        }

        /// <summary>
        /// PlaySong for first time
        /// </summary>
        public void FirstPlaySong(int ticks)
        {
            try
            {
                PlayerState = PlayerStates.Playing;
                nbstop = 0;
                scrolling = false;
                _currentMeasure = -1;
                BtnStatus();
                sequencer1.Start();

                if (ticks > 0)
                    sequencer1.Position = ticks;

                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Display according to play, pause, stop status
        /// </summary>
        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    btnPlay.Image = Properties.Resources.btn_green_pause;
                    //btnStop.Image = Properties.Resources.btn_black_stop;
                    btnPlay.Enabled = true;  // to allow pause
                    //btnStop.Enabled = true;  // to allow stop 
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnPlay.Enabled = true;  // to allow play
                    //btnStop.Enabled = true;  // to allow stop
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        //btnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                    {
                        //btnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    }
                    break;

                default:
                    break;
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
                sequencer1.Stop();

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
                        decimal pos = newstart + positionHScrollBar.Minimum;
                        if (positionHScrollBar.Minimum <= pos && pos <= positionHScrollBar.Maximum)
                            positionHScrollBar.Value = pos;
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
        /// Things to do at the end of a song
        /// </summary>
        private void AfterStopped()
        {
            // Buttons play & stop 
            BtnStatus();
            
            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);

                DisplayPositionHScrollBar(0);

                _currentMeasure = -1;
                _currentTimeInMeasure = -1;
                positionHScrollBar.Value = positionHScrollBar.Minimum;
                
                chordAnalyserControl1.OffsetX = 0;
                chordAnalyserControl1.DisplayNotes(0, -1, -1);

                pnlDisplayMap.VerticalScroll.Value = pnlDisplayMap.VerticalScroll.Minimum;                
                ChordMapControl1.OffsetY = 0;
                ChordMapControl1.DisplayNotes(0, -1, -1);

                laststart = 0;
                scrolling = false;
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
        }

        /// <summary>
        /// Display Time elapse
        /// </summary>
        private void DisplayTimeElapse(int pos)
        {
            double dpercent = 100 * pos / (double)_totalTicks;
            lblPercent.Text = string.Format("{0}%", (int)dpercent);

            double maintenant = (dpercent * _duration) / 100;  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            lblElapsed.Text = string.Format("{0:00}:{1:00}", Min, Sec);
        }


        #endregion Play stop pause
    }
}
