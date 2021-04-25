﻿#region License

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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.PianoRoll;
using Sanford.Multimedia.Midi.UI;
using System.IO;

namespace Karaboss
{

    public partial class frmPianoRoll : Form
    {
        #region private decl

        private int TempoDelta = 100;
        private int TempoOrig = 0;
      

        private int resolution = 4;
        private int yScale = 20;
        private float zoomx = 1.0f;
        private float zoomy = 1.0f;                
        private bool bAlltracks;
        private Track SingleTrack;
        private int SingleTrackNumber;
        private int SingleTrackChannel;
        private bool closing = false;
        private bool scrolling = false;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;

        private int _measurelen = 0;

        private int mesurelen = 80;        
        private int _timeLineHeight = 20;        
        private int leftWidth = 100;
        private bool bShowVScrollBar = false;
        private bool bShowHScrollBar = false;

        private bool bEditScore = false;
        private bool bEnterNotes = false;

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

        private int bouclestart = 0;
        private int newstart = 0;
        private int laststart = 0;      // Start time to play
        private int nbstop = 0;

        private enum Editstatus
        {
            None,
            Arrow,
            Pen,
        }
        private Editstatus EditStatus;

        private string CurrentPath = string.Empty;        
        

        #endregion

        #region controls
        private frmPlayer frmPlayer;
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();
        private Sequence sequence1 = new Sequence();

        // Creation dynamique de controles       
        Panel pnlHScroll;               // panel horizontal
        Panel pnlTimeLeft;
        Panel pnlTimeLine;        
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;
        PianoControl pianoControl1;
        PianoRollControl pianoRollControl1;
        Sanford.Multimedia.Midi.Score.tlControl tlControl1;
        #endregion


        public frmPianoRoll(Sequence seq, int tracknum, OutputDevice outdevicePiano, string fileName)
        {
            InitializeComponent();          

            CurrentPath = fileName;
            this.DoubleBuffered = true;

            this.MouseWheel += new MouseEventHandler(FrmPianoRoll_MouseWheel);
            this.KeyPreview = true;  // Allow form keydown
            this.KeyDown += new KeyEventHandler(FrmPianoRoll_KeyDown);
            this.KeyUp += new KeyEventHandler(FrmPianoRoll_KeyUp);

            timer1.Interval = 20;

            // Sequence
            LoadSequencer(seq);
            outDevice = outdevicePiano;            
         
            //zoomx = 1.0f;
            //zoomy = 1.0f;
            resolution = 4;

            // 20 (yScale) pixels par noire (quarter note)
            mesurelen = (yScale * 4 * sequence1.Numerator)/sequence1.Denominator;

            if (tracknum == -1)
            {
                // All tracks                      
                SingleTrack = null;
                SingleTrackNumber = -1;               
                SingleTrackChannel = -1;
                bAlltracks = true;
            }
            else
            {
                // One track               
                SingleTrack = sequence1.tracks[tracknum];
                SingleTrackNumber = tracknum;
                SingleTrackChannel = SingleTrack.MidiChannel;
                bAlltracks = false;
            }            

            lblEdit.Visible = !bAlltracks;
            lblSaisieNotes.Visible = !bAlltracks;

            InitCbTracks();

            // Display track
            CbTracks.SelectedIndex = tracknum == -1 ? 0 : tracknum + 1;

            SetTitle(fileName);

            // Draw all controls
            DrawControls();
            FinalizeControls();

            // Draw notes
            pianoRollControl1.TrackNum = tracknum;

        }


        /// <summary>
        /// Remove flickering https://stackoverflow.com/questions/2612487/how-to-fix-the-flickering-in-user-controls
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }


        #region DrawControls

        private void LoadSequencer(Sequence seq)
        {
            try
            {
                sequence1 = seq;

                sequencer1 = new Sequencer();
                sequencer1.Sequence = sequence1;    // primordial !!!!!
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                UpdateMidiTimes();
                
                TempoOrig = _tempo;
                lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

                // Display
                int Min = (int)(_duration / 60);
                int Sec = (int)(_duration - (Min * 60));
                lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);                

                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Upadate MIDI times
        /// </summary>
        private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            TempoOrig = _tempo;
            _ppqn = sequence1.Division;
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _bpm = GetBPM(_tempo);

            if (sequence1.Time != null)
                _measurelen = sequence1.Time.Measure;
        }


        /// <summary>
        /// Calculate BPM 
        /// </summary>
        /// <param name="tempo"></param>
        /// <returns></returns>
        private int GetBPM(int tempo)
        {
            // see http://midi.teragonaudio.com/tech/midifile/ppqn.htm
            const float kOneMinuteInMicroseconds = 60000000;
            float kTimeSignatureNumerator = (float)sequence1.Numerator;
            float kTimeSignatureDenominator = (float)sequence1.Denominator;

            //float BPM = (kOneMinuteInMicroseconds / (float)tempo) * (kTimeSignatureDenominator / 4.0f);            
            float BPM = kOneMinuteInMicroseconds / (float)tempo;

            return (int)BPM;
        }


        /// <summary>
        /// Sets title of form
        /// </summary>
        /// <param name="fileName"></param>
        private void SetTitle(string fileName)
        {
            string tx = string.Empty;
            if (SingleTrack != null)
            {
                string InstrumentName = MidiFile.PCtoInstrument(SingleTrack.ProgramChange);
                tx = "Karaboss - (Track " + SingleTrackNumber.ToString() + ") - " + InstrumentName + " - " + Path.GetFileName(fileName);
            }
            else
            {
                tx = "Karaboss - " + Path.GetFileName(fileName);
            }
            this.Text = tx;
        }

        /// <summary>
        /// Draw controls
        /// </summary>
        private void DrawControls()
        {

            #region top            
            CbResolution.Items.Add("Beat");
            CbResolution.Items.Add("1/2");
            CbResolution.Items.Add("1/3");
            CbResolution.Items.Add("1/4");
            CbResolution.Items.Add("1/6");
            CbResolution.Items.Add("1/8");
            CbResolution.Items.Add("1/9");
            CbResolution.Items.Add("1/12");
            CbResolution.Items.Add("1/16");
            CbResolution.SelectedIndex = 3;


            // Maximum of horizontal scroll bar is a multiple of measures            
            int dur = sequence1.GetLength();

            int lastenoteticks = sequence1.GetLastNoteEndTime();

            // Conversion to int gives strange results for the number of measures
            // because it round to the closest integer, so 4.1 measures will gives only 4 measures instead of 5
            int nbmeasures = Convert.ToInt32(lastenoteticks / _measurelen);
            // compares time for all measures with time of last note
            int totaltimemeasures = nbmeasures * _measurelen;
            if (lastenoteticks > totaltimemeasures)
                nbmeasures++;

            totaltimemeasures = nbmeasures * _measurelen;

            //positionHScrollBar.Value = 0;

            positionHScrollBar.Maximum = totaltimemeasures + _measurelen;
            positionHScrollBar.Minimum = _measurelen;
            positionHScrollBar.TickStyle = TickStyle.TopLeft;
            positionHScrollBar.ScaleDivisions = nbmeasures;
            positionHScrollBar.TickDivide = _measurelen;
            positionHScrollBar.SmallChange = (uint)nbmeasures;
            positionHScrollBar.LargeChange = (uint)nbmeasures;
            if (_measurelen > 0)
                positionHScrollBar.MouseWheelBarPartitions = _measurelen;

            #endregion

            #region time line
            if (pnlTimeLine != null)
                pnlTimeLine.Dispose();

            pnlTimeLine = new Panel() {
                Parent = pnlTop,
                Location = new Point(0, 0),                
                Size = new Size(300, _timeLineHeight),
            };

            pnlTimeLeft = new Panel() {
                Location = new Point(0, 0),
                Size = new Size(leftWidth, _timeLineHeight),
                BackColor = Color.DarkGray,
                Parent = pnlTimeLine,
            };
            pnlTimeLine.Controls.Add(pnlTimeLeft);

            tlControl1 = new Sanford.Multimedia.Midi.Score.tlControl() {
                Location = new Point(leftWidth, 0),
                Size = new Size(100, _timeLineHeight),
                Parent = pnlTimeLine,
                mesurelen = mesurelen,
            };        
            pnlTimeLine.Controls.Add(tlControl1);

            pnlTop.Controls.Add(pnlTimeLine);
            pnlTimeLine.Dock = DockStyle.Bottom;

            #endregion time line                   

            #region left piano control
           
            // Add piano control to pnlPiano       
            if (pianoControl1 != null)
                pianoControl1.Dispose();

            pianoControl1 = new PianoControl() {
                Parent = pnlPiano,
                Location = new Point(0, 0),                
                Orientation = Orientation.Vertical,
            };

            pianoControl1.Size = new Size(leftWidth, pianoControl1.totalLength);           
            pianoControl1.PianoKeyDown += new EventHandler<PianoKeyEventArgs>(PianoControl1_PianoKeyDown);
            pianoControl1.PianoKeyUp += new EventHandler<PianoKeyEventArgs>(PianoControl1_PianoKeyUp);
            pnlPiano.Controls.Add(pianoControl1);
            pnlPiano.Height = pianoControl1.Height;

            #endregion left


            #region right  pianoRoll control     

           pnlMiddle.Size = new Size(pnlScrollView.Parent.ClientSize.Width - leftWidth, pnlScrollView.Parent.ClientSize.Height);
       
            // Horizontal scrollbar
            if (pnlHScroll != null)
                pnlHScroll.Dispose();

            pnlHScroll = new Panel() {
                Parent = pnlMiddle,
                AutoScroll = false,
                AutoSize = false,
                BackColor = Color.Black,                
            };
            pnlMiddle.Controls.Add(pnlHScroll);
            pnlHScroll.Dock = DockStyle.Bottom;
            pnlHScroll.Size = new Size(pnlMiddle.Width - pnlPiano.Width, 17);


            hScrollBar = new HScrollBar() {
                Parent = pnlHScroll,
                Left = leftWidth,
                Top = 0,
                Minimum = 0,
            };
            pnlHScroll.Controls.Add(hScrollBar);

            hScrollBar.Scroll += new ScrollEventHandler(HScrollBar_Scroll);
            hScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);

            // --------------------------------------------------
            // Add first pnlScrollView
            // then pianoControl
            // --------------------------------------------------
            pnlPiano.BringToFront();
            pnlHScroll.BringToFront();


            // Vertical Scrollbar
            if (vScrollBar != null)
                vScrollBar.Dispose();

            vScrollBar = new VScrollBar() {
                Parent = pnlMiddle,
                Top = 0,
                Minimum = 0,
            };
            pnlMiddle.Controls.Add(vScrollBar);
            vScrollBar.Dock = DockStyle.Right;

            vScrollBar.BringToFront();

            vScrollBar.Scroll += new ScrollEventHandler(VScrollBar_Scroll);
            vScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            // --------------------------------------------------
            // PianoRoll on the pnlScrollView
            // --------------------------------------------------
            if (pianoRollControl1 != null)
                pianoRollControl1.Dispose();

            pianoRollControl1 = new PianoRollControl() {
                Parent = pnlScrollView,
                Location = new Point(0, 0),
                Size = new Size(50, 50),                              
            };
            
            pianoRollControl1.MouseDownPlayNote += new MouseDownPlayNoteEventHandler(PianoRollControl1_MouseDownPlayNote);
            pianoRollControl1.MouseUpStopNote += new MouseUpStopNoteEventHandler(PianoRollControl1_MouseUpStopNote);
            pianoRollControl1.SequenceModified += new EventHandler(PianoRollControl1_SequenceModified);            
            pianoRollControl1.InfoNote += new InfoNoteEventHandler(PianoRollControl1_InfoNote);

            // properties
            pianoRollControl1.Velocity = Karaclass.m_Velocity;


            pnlScrollView.Controls.Add(pianoRollControl1);
            
            #endregion right

            SetScrollBarValues();
        }

        private void FinalizeControls()
        {
            // Notes du piano
            pianoControl1.LowNoteID = pianoRollControl1.LowNoteID = 23;     //  23
            pianoControl1.HighNoteID = pianoRollControl1.HighNoteID = 108;  // 108            

            // Sequence pour pianoRoll & tlControl
            pianoRollControl1.Sequence1 = sequence1;
            tlControl1.Sequence1 = sequence1;

            pianoRollControl1.yScale = yScale;
            pianoControl1.Scale = yScale;

            pianoRollControl1.zoomx = zoomx;
            pianoControl1.zoom = zoomy;            
            pianoControl1.Height = pianoControl1.totalLength;            
            pianoRollControl1.yScale = pianoControl1.Scale;

            SetScrollBarValues();

            // Adjust Width
            int vW = 0;
            if (bShowVScrollBar)
                vW = vScrollBar.Width;

            // Adjust height
            pnlScrollView.Left = leftWidth;

            pianoRollControl1.Height = pianoControl1.Height;
            pnlScrollView.Height = pnlPiano.Height;
            pnlScrollView.Width = pnlMiddle.Width - leftWidth - vW;
            pianoRollControl1.Width = pnlScrollView.Width;
            tlControl1.Width = pnlScrollView.Width;                                         
        }

        #endregion


        #region CbTracks


        private void InitCbTracks()
        {
            int i = 1;
            string N;
            CbTracks.Items.Clear();
            CbTracks.Items.Add("All tracks");
            foreach (Track trk in sequence1.tracks)
            {
                N = "<NoName>";
                if (trk.Name != null)
                {
                    if (trk.Name.Trim() != "")
                        N = trk.Name.Trim();
                }
                CbTracks.Items.Add(i.ToString("00") + " " + "[" + trk.MidiChannel.ToString("00") + "]" + " - " + N + " - " + "(" + MidiFile.PCtoInstrument(trk.ProgramChange) + ")");
                i++;
            }
        }

        /// <summary>
        /// Click on list of tracks = display selected track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pianoRollControl1 == null)
                return;

            sequencer1.stopper.AllSoundOff();
            pianoControl1.Reset();

            int tracknum = CbTracks.SelectedIndex;

            if (tracknum == 0)
            {
                // All tracks                      
                SingleTrack = null;
                SingleTrackNumber = -1;
                tracknum = -1;
                SingleTrackChannel = -1;
                bAlltracks = true;
            }
            else
            {
                // One track
                tracknum = tracknum - 1;
                SingleTrack = sequence1.tracks[tracknum];
                SingleTrackNumber = tracknum;
                SingleTrackChannel = SingleTrack.MidiChannel;
                bAlltracks = false;
            }

            //lblPen.Visible = !bAlltracks;
            //lblPointer.Visible = !bAlltracks;
            lblEdit.Visible = !bAlltracks;
            lblSaisieNotes.Visible = !bAlltracks;

            // Track pour pianoRoll
            if (tracknum != -1)
            {                
                pianoRollControl1.TrackNum = tracknum;                
            }
            else
            {
                pianoRollControl1.TrackNum = -1;
            }
            CbTracks.Parent.Focus();
        }
        
        #endregion  


        #region scroll events
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            pnlPiano.Top = -vScrollBar.Value;
            pnlScrollView.Top = -vScrollBar.Value;
        }

        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            pnlPiano.Top = -vScrollBar.Value;
            pnlScrollView.Top = -vScrollBar.Value;
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            pianoRollControl1.OffsetX = hScrollBar.Value;
            tlControl1.OffsetX = hScrollBar.Value;
            pianoRollControl1.Refresh();
            tlControl1.Refresh();                         
        }

        private void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            pianoRollControl1.OffsetX = hScrollBar.Value;
            tlControl1.OffsetX = hScrollBar.Value;            
        }

        /// <summary>
        /// Set scrollBar maxi values
        /// </summary>
        private void SetScrollBarValues()
        {
            if (pnlScrollView == null || pianoRollControl1 == null)
                return;

            int hH = 0;
            int vW = 0;
            int v = 0;

            // Width of pianoRollControl
            int W = pianoRollControl1.maxStaffWidth;
            // Display width
            int wMiddle = pnlMiddle.Width - pnlPiano.Width;

            bool bShowHScrollBarIndetermined = false;

            // If display width > pianoRollControl width => remove horizontal scrollbar
            if (wMiddle > W + vScrollBar.Width) 
                bShowHScrollBar = false;
            else if (wMiddle < W)
                bShowHScrollBar = true;
            else
                bShowHScrollBarIndetermined = true;


            bool bShowVScrollBarIndetermined = false;

            // If display height > pianoRollControl height => remove vertical scrollbar
            if (pnlMiddle.Height > pnlScrollView.Height + hScrollBar.Height)
                bShowVScrollBar = false;
            else if (pnlMiddle.Height < pnlScrollView.Height)
                bShowVScrollBar = true;
            else
                bShowVScrollBarIndetermined = true;


            if (bShowVScrollBarIndetermined && bShowHScrollBarIndetermined)
            {
                bShowHScrollBar = false;
                bShowVScrollBar = false;
            }
            else if (bShowVScrollBarIndetermined && pnlMiddle.Height > pnlScrollView.Height)
                bShowVScrollBar = false;
            else if (bShowHScrollBarIndetermined && wMiddle > W)
                bShowHScrollBar = false;


            if (bShowVScrollBar && wMiddle - vScrollBar.Width < W)
                bShowHScrollBar = true;

            if (bShowHScrollBar && pnlMiddle.Height - hScrollBar.Height < pnlScrollView.Height)
                bShowVScrollBar = true;

            if (bShowVScrollBar == false)
            {
                vScrollBar.Visible = false;
                pnlPiano.Top = 0;
                pnlScrollView.Top = 0;
            }
            else
            {
                vScrollBar.Visible = true;
                vScrollBar.Left = pnlMiddle.Width - vScrollBar.Width;
                if (bShowHScrollBar)
                    vScrollBar.Height = pnlMiddle.Height - hScrollBar.Height;
                else
                    vScrollBar.Height = pnlMiddle.Height;
            }

            if (bShowHScrollBar == false)
            {
                pnlHScroll.Visible = false;                

                pnlScrollView.Left = pnlPiano.Width;
                
                pianoRollControl1.OffsetX = 0;
                tlControl1.OffsetX = 0;                
            }
            else
            {
                pnlHScroll.Visible = true;
                if (bShowVScrollBar)
                    hScrollBar.Width = wMiddle - vScrollBar.Width;
                else
                    hScrollBar.Width = wMiddle;                
            }

            if (bShowVScrollBar)
                vW = vScrollBar.Width;
            if (bShowHScrollBar)
                hH = hScrollBar.Height;

            if (bShowVScrollBar)
            {
                vScrollBar.SmallChange = pnlScrollView.Height / 20;
                vScrollBar.LargeChange = pnlScrollView.Height / 10;
                vScrollBar.Maximum = pnlScrollView.Height - (pnlMiddle.Height - hH) + vScrollBar.LargeChange;

                // Aggrandissement vertical de la fenetre = > la scrollbar verticale doit se positionner en bas
                if (pianoRollControl1.Height - vScrollBar.Value < pnlMiddle.Height)
                {
                    v = vScrollBar.Maximum - vScrollBar.LargeChange;
                    if (v >= 0)
                        vScrollBar.Value = v;
                }
            }

            if (bShowHScrollBar)
            {
                int SC = W / 20;
                int LC = W / 10;

                // Width is limited to 65535                
                hScrollBar.Maximum = W - (wMiddle - vW) + LC;
                hScrollBar.SmallChange = SC;
                hScrollBar.LargeChange = LC;


                // Aggrandissement horizontal de la fenetre => la scrollbar horizontale doit se positionner à droite 
                if (W - hScrollBar.Value < wMiddle)
                {
                    v = hScrollBar.Maximum - hScrollBar.LargeChange;
                    if (v >= 0)
                        hScrollBar.Value = v;
                }
            }
        }

        #endregion scroll events


        #region Edit

        private void DisableEditButtons()
        {
            // None selection
            EditStatus = Editstatus.None;
            Cursor = Cursors.Default;

            lblSaisieNotes.BackColor = Color.White;
            bEnterNotes = false;
            pianoRollControl1.NotesEdition = false;
        }


        private void lblEdit_Click(object sender, EventArgs e)
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            DspEdit(!bEditScore);
        }

        /// <summary>
        /// Validate or invalidate EditMode
        /// </summary>
        /// <param name="status"></param>
        private void DspEdit(bool status)
        {
            if (status == true)
            {
                // Enter edit mode
                bEditScore = true;

                lblEdit.BackColor = Color.Red;
                lblEdit.ForeColor = Color.White;                

                // Allow sheetmusic editing
            }
            else
            {
                // Quit edit mode
                bEditScore = false;

                lblEdit.BackColor = Color.White;
                lblEdit.ForeColor = Color.Black;

                // Quit enter notes
                DisableEditButtons();
                bEnterNotes = false;
                pianoRollControl1.NotesEdition = false;

                // Disallow sheetmusic edit mode

            }
        }

        private void lblSaisieNotes_Click(object sender, EventArgs e)
        {
            DspEnterNotes();
        }

        /// <summary>
        /// Validate or invalidate Entering notes
        /// </summary>
        private void DspEnterNotes()
        {
            if (PlayerState != PlayerStates.Stopped)
                return;
            
            if (bEnterNotes == true)
            {
                // stop notes entering 
                DisableEditButtons();
                bEnterNotes = false;
            }
            else
            {
                // start notes entering 
                DisableEditButtons();
                bEnterNotes = true;
                pianoRollControl1.NotesEdition = true;

                // Enter edit mode
                DspEdit(true);

                lblSaisieNotes.BackColor = Color.Red;            
                this.Cursor = Cursors.Hand;
               
            }
        }


        private void LblPointer_Click(object sender, EventArgs e)
        {
            if (EditStatus == Editstatus.Arrow)
            {
                // None selection
                DisableEditButtons();
            }
            else
            {
                // Pointer selection
                EditStatus = Editstatus.Arrow;
                pianoRollControl1.NotesEdition = false;
                Cursor = Cursors.Default;

                lblPen.BackColor = Color.White;
                lblPointer.BackColor = Color.Red;
            }
        }

        private void LblPen_Click(object sender, EventArgs e)
        {
            if (EditStatus == Editstatus.Pen)
            {
                // None selection
                DisableEditButtons();
            }                
            else
            {
                // Pen selection
                EditStatus = Editstatus.Pen;
                pianoRollControl1.NotesEdition = true;
                Cursor = Cursors.Hand;

                lblPen.BackColor = Color.Red;
                lblPointer.BackColor = Color.White;
            }
        }
   

        private void CbResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            String strResolution = CbResolution.Text;
            SetResolution(strResolution);
            CbResolution.Parent.Focus();
        }


        public void SetResolution(string strResolution)
        {
            switch (strResolution)
            {
                case "Beat":
                    //resolution of 1 by quarter note
                    resolution = 1;
                    break;
                case "1/2":
                    //resolution of 2 by quarter note
                    resolution = 2;
                    break;
                case "1/3":
                    //resolution of 3 by quarter note
                    resolution = 3;
                    break;
                case "1/4":
                    //resolution of 4 by quarter note
                    resolution = 4;
                    break;
                case "1/6":
                    //resolution of 6 by quarter note
                    resolution = 6;
                    break;
                case "1/8":
                    //resolution of 8 by quarter note
                    resolution = 8;
                    break;
                case "1/9":
                    //resolution of 9 by quarter note
                    resolution = 9;
                    break;
                case "1/12":
                    //resolution of 12 by quarter note
                    resolution = 12;
                    break;
                case "1/16":
                    //resolution of 16 by quarter note
                    resolution = 16;
                    break;
            }

            //lblResolution.Text = strResolution;
            if (pianoControl1 != null)
                pianoRollControl1.Resolution = resolution;
        }

        #endregion Edit

  
        #region pianoRollControl

        private void PianoRollControl1_MouseDownPlayNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            #region Guard
            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, nnote, 127));
        }

        private void PianoRollControl1_MouseUpStopNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            #region Guard
            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, nnote, 0));
        }
    
        /// <summary>
        /// Send modified notes to score on frmPlayer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PianoRollControl1_SequenceModified(Object sender, EventArgs e)
        {
            UpdateFrmPlayer();
        }

        private void PianoRollControl1_InfoNote(string NoteInfo)
        {
            lblNote.Text = NoteInfo;
        }

        #endregion pianoRollControl


        #region pianoControl

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pianoControl1.PressPianoKey(e.KeyCode);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pianoControl1.ReleasePianoKey(e.KeyCode);
            base.OnKeyUp(e);
        }


        private void PianoControl1_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void PianoControl1_PianoKeyUp(object sender, PianoKeyEventArgs e)
        {
            #region Guard
            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        #endregion pianoControl


        #region Form load close MouseWheel

        private void FrmPianoRoll_KeyDown(object sender, KeyEventArgs e)
        {
            pianoRollControl1.ManageKeyDown(sender, e);

        }

        private void FrmPianoRoll_KeyUp(object sender, KeyEventArgs e)
        {
            pianoRollControl1.ManageKeyUp(sender, e);

        }

        /// <summary>
        /// Mouse Wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPianoRoll_MouseWheel(object sender, MouseEventArgs e)
        {
            Point PpianoRoll = pnlScrollView.PointToClient(Cursor.Position);
            Point Ppiano = pnlPiano.PointToClient(Cursor.Position);

            Rectangle rectPianoRoll = new Rectangle(0, 0, pnlScrollView.Width, pnlScrollView.Height);
            Rectangle rectPiano = new Rectangle(0, 0, pnlPiano.Width, pnlPiano.Height);
            
            
            if (rectPiano.Contains(Ppiano))
            {
                // If Mouse over piano

                float oldOffset = ((float)vScrollBar.Value / (float)vScrollBar.Maximum);

                zoomy = pianoControl1.zoom;
                zoomy += (e.Delta > 0 ? 0.1f : -0.1f);
                pianoControl1.zoom = zoomy;
                pianoControl1.Height = pianoControl1.totalLength;

                pianoRollControl1.yScale = pianoControl1.Scale;
                pianoRollControl1.Height =  pianoControl1.Height;

                // Adjust heights
                pnlPiano.Height = pnlScrollView.Height = pianoControl1.totalLength;                

                // Adjust scrollbars values
                SetScrollBarValues();

                if (bShowVScrollBar)
                {
                    float newOffset = oldOffset * vScrollBar.Maximum;
                    if (newOffset >= 0 && newOffset < vScrollBar.Maximum)
                    {
                        vScrollBar.Value = (int)newOffset;
                    }
                }
            }            
            else if (rectPianoRoll.Contains(PpianoRoll))
            {
                // If Mouse over pianoRoll                                                      
                
                float oldOffset = ((float)hScrollBar.Value/(float)hScrollBar.Maximum);

                zoomx += (e.Delta > 0 ? 0.1f : -0.1f);                
                tlControl1.zoom = zoomx;
                pianoRollControl1.zoomx = zoomx;
                           
                // Adjust scrollbar values
                SetScrollBarValues();                

                if (bShowHScrollBar)
                {
                    float newOffset = oldOffset * hScrollBar.Maximum;
                    if (newOffset >= 0 && newOffset < hScrollBar.Maximum)
                    {
                        hScrollBar.Value = (int)newOffset;
                    }
                }
            }
        }

        /// <summary>
        /// Window and scrollbar startup position
        /// </summary>
        public void StartupPosition(float ticks = 0)
        {                       
            int middleScroll = vScrollBar.Maximum / 2;
            vScrollBar.Value = middleScroll;

            pianoRollControl1.OffsetX = Convert.ToInt32(ticks * pianoRollControl1.XScale);
            if (pianoRollControl1.OffsetX >= hScrollBar.Minimum && pianoRollControl1.OffsetX <= hScrollBar.Maximum)
            {
                tlControl1.OffsetX = pianoRollControl1.OffsetX;
                hScrollBar.Value = pianoRollControl1.OffsetX;
            }
        }

        private void FrmPianoRoll_Resize(object sender, EventArgs e)
        {
            int vW = 0;

            positionHScrollBar.Width = pnlTop.Width - positionHScrollBar.Left - 40;

            if (pnlScrollView != null && pnlScrollView.Parent != null)
            {
                if (bShowVScrollBar)
                    vW = vScrollBar.Width;

                pnlScrollView.Width = pnlMiddle.Width - leftWidth - vW;
                pianoRollControl1.Width = pnlScrollView.Width;
                tlControl1.Width = pnlScrollView.Width;                
            }

            if (pianoRollControl1 != null)
                pianoRollControl1.Redraw();

            tlControl1.Redraw();
            SetScrollBarValues();            
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
            if (keyData == Keys.Left)
            {
                if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped && newstart > 0))
                {
                    StopMusic();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmPianoRoll_KeyDown(object sender, KeyEventArgs e)
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

                case Keys.Add:
                case Keys.Subtract:
                case Keys.D6:
                case Keys.Decimal:
                    // Tempo +-
                    KeyboardSelectTempo(e);
                    break;
            }
        }
       

        private void frmPianoRoll_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Shift)
            {
                switch (e.KeyCode)
                {
                    case Keys.Oemplus:
                        // Tempo +-
                        KeyboardSelectTempo(e);
                        break;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            //sequence1.Dispose();
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();

            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }
               
        /// <summary>
        /// Update modified notes on frmPlayer
        /// </summary>
        private void UpdateFrmPlayer()
        {
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {                
                frmPlayer = GetForm<frmPlayer>();
                frmPlayer.RefreshDisplay();
                frmPlayer.FileModified();
                this.Focus();
            }
        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

        
        /// <summary>
        /// Form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPianoRoll_Load(object sender, EventArgs e)
        {
            // Récupère la taille et position de la forme
            // Set window location

            // If window is maximized
            if (Properties.Settings.Default.frmPianoRollMaximized)
            {
                
                Location = Properties.Settings.Default.frmPianoRollLocation;
                //Size = Properties.Settings.Default.frmPianoRollSize;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmPianoRollLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmPianoRollSize;                
            }

            try
            {
                zoomx = Properties.Settings.Default.PianoRollZoomX;
                zoomy = Properties.Settings.Default.PianoRollZoomY;
                pianoRollControl1.zoomx = zoomx;
                tlControl1.zoom = zoomx;

                pianoControl1.zoom = zoomy;
                pianoControl1.Height = pianoControl1.totalLength;

                pianoRollControl1.yScale = pianoControl1.Scale;
                pianoRollControl1.Height = pianoControl1.Height;

                // Adjust heights
                pnlPiano.Height = pnlScrollView.Height = pianoControl1.totalLength;
                // Adjust scrollbars values
                SetScrollBarValues();
            }
            catch (Exception ex)
            {
                zoomx = 1.0f;
                zoomy = 1.0f;
            }

        }
       
        /// <summary>
        /// Form close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPianoRoll_FormClosing(object sender, FormClosingEventArgs e)
        {
            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmPianoRollLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmPianoRollMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmPianoRollLocation = Location;
                    Properties.Settings.Default.frmPianoRollSize = Size;
                    Properties.Settings.Default.frmPianoRollMaximized = false;
                }

                // Zoom
                Properties.Settings.Default.PianoRollZoomX = zoomx;
                Properties.Settings.Default.PianoRollZoomY = zoomy;
                
                // Save settings
                Properties.Settings.Default.Save();
            }

        }

        #endregion Form load close


        #region handle messages

        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        sequencer1.Position = e.NewValue - (int)positionHScrollBar.Minimum;
                        break;
                    case PlayerStates.Paused:
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        sequencer1.Position = newstart;
                        pianoRollControl1.OffsetX = Convert.ToInt32(newstart * pianoRollControl1.XScale);
                        tlControl1.OffsetX = pianoRollControl1.OffsetX;
                        nbstop = 0;
                        
                        break;
                    case PlayerStates.Stopped:
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        pianoRollControl1.OffsetX = Convert.ToInt32(newstart * pianoRollControl1.XScale);
                        tlControl1.OffsetX = pianoRollControl1.OffsetX;
                        nbstop = 0;                        
                        break;
                }
                positionHScrollBar.Parent.Focus();
                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }            

        /// <summary>
        /// Event: playing midi file completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            // Passe éventuellement au morceau suivant, sinon s'arrête
            PlayerState = PlayerStates.Stopped;
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            if (bAlltracks)
            {
                outDevice.Send(e.Message);
                pianoControl1.Send(e.Message);
            }
            else if (e.Message.MidiChannel == SingleTrackChannel)
            {
                outDevice.Send(e.Message);
                pianoControl1.Send(e.Message);
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
            //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                //pianoControl1.Send(message);
            }
        }
        

        #endregion


        #region Play stop pause

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
            BtnPlay.Parent.Focus();
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopMusic();
            BtnStop.Parent.Focus();
        }

        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Play_icon;
                    //btnStop.Image = Properties.Resources.btn_black_stop;
                    BtnPlay.Enabled = true;  // to allow pause
                    BtnStop.Enabled = true;  // to allow stop 
                    //lblStatus.Text = "Playing";
                    //lblStatus.ForeColor = Color.LightGreen;
                    //SetStartVLinePos(0);
                    //sldMainTempo.Enabled = false;       // tempo change not allowed
                    //sldTranspose.Enabled = false;       // Transpose not allowed
                    //btnStartRec.Enabled = false;
                    break;

                case PlayerStates.Paused:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Pause_icon;
                    BtnPlay.Enabled = true;  // to allow play
                    BtnStop.Enabled = true;  // to allow stop

                    //lblStatus.Text = "Paused";
                    //lblStatus.ForeColor = Color.Yellow;
                    break;

                case PlayerStates.Stopped:
                    BtnPlay.Image = Properties.Resources.Media_Controls_Play_icon;
                    BtnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        //btnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                        BtnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    //lblStatus.Text = "Stopped";
                    //lblStatus.ForeColor = Color.Red;
                    //sldMainTempo.Enabled = true;        // Tempo change allowed
                    //sldTranspose.Enabled = true;        // Transpose allowed
                    //btnStartRec.Enabled = true;
                    //VuMasterPeakVolume.Level = 0;
                    break;

                

                default:
                    break;
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
                    timer1.Start();
                    sequencer1.Continue();
                    break;

                case PlayerStates.Waiting:
                    // stop chrono wait : status = WaitingPaused
                    PlayerState = PlayerStates.WaitingPaused;
                    BtnStatus();                    
                    break;

                case PlayerStates.WaitingPaused:
                    // restart chrono wait : status = Waiting
                    PlayerState = PlayerStates.Waiting;
                    BtnStatus();                    
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
        /// Initialize sequencer
        /// </summary>
        private void ResetSequencer()
        {
            newstart = 0;
            laststart = 0;
            sequencer1.Stop();
            PlayerState = PlayerStates.Stopped;
        }

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
                        // back to 0 position if a second hit on left key
                        newstart = 0;
                        nbstop = 0;
                        AfterStopped();
                    }
                    else
                    {
                        // Back to stored position on it is the first hit on left key
                        pianoControl1.Reset();
                        positionHScrollBar.Value = newstart + positionHScrollBar.Minimum;
                        pianoRollControl1.OffsetX = Convert.ToInt32(newstart * pianoRollControl1.XScale);
                        tlControl1.OffsetX = pianoRollControl1.OffsetX;
                        // left key was hit one time
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
            //sheetmusic.BPlaying = false;
            pianoControl1.Reset();

            // Stopped to start of score
            if (newstart <= 0)
            {
                //ScrollTimeBar(0);
                //lblTimeLyric.Text = "00:00";
                //lblBeat.Text = "1|" + sequence1.Numerator;

                positionHScrollBar.Value = positionHScrollBar.Minimum; 
                hScrollBar.Value = 0;
                //pianoRollControl1.OffsetX = 0;

                //SetTimeVLinePos(0);
                laststart = 0;
                //SetStartVLinePos(0);

                //if (PlayerStates == PlrStatus.Stopped)
                //    ValideMenus(true);

            }
            else
            {
                // Stop to start point newstart (ticks)
                //ScrollTo(newstart);
                //int x_midle = (pnlMiddle.Width - pnlTracks.Width) / 2;
                // blue bar
                //SetStartVLinePos(x_midle);
                // red bar
                //SetTimeVLinePos(0);
            }
        }

        #endregion


        #region timer player     

        /// <summary>
        /// Display Time Elapse
        /// </summary>
        private void DisplayTimeElapse(double dpercent)
        {
            lblPercent.Text = string.Format("{0}%", (int)dpercent);

            double maintenant = (dpercent * _duration) / 100;  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            lblElapsed.Text = string.Format("{0:00}:{1:00}", Min, Sec);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!scrolling)
            {
                // Display time elapse
                double dpercent = 100 * sequencer1.Position / (double)_totalTicks;
                DisplayTimeElapse(dpercent);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        ScrollTimeBar();
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

                #region position hscrollbar
                try
                {
                    if (PlayerState == PlayerStates.Playing && sequencer1.Position < positionHScrollBar.Maximum - positionHScrollBar.Minimum) 
                        positionHScrollBar.Value = sequencer1.Position + _measurelen;
                }
                catch (Exception ex)
                {
                    Console.Write("Error positionHScrollBarNew.Value - " + ex.Message);
                }
                #endregion position hscrollbar

            }
        }

        /// <summary>
        /// Scroll vertical time bar and sheet music when playing
        /// </summary>
        /// <param name="curtime"></param>
        private void ScrollTimeBar()
        {
            int offset = Convert.ToInt32(sequencer1.Position * pianoRollControl1.XScale);
            pianoRollControl1.OffsetX = offset;
            
            if (offset < hScrollBar.Maximum)
                hScrollBar.Value = pianoRollControl1.OffsetX;
        }

        #endregion


        #region tempo

        private void KeyboardSelectTempo(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Oemplus:
                case Keys.Add:
                    if (TempoDelta > 10)
                        TempoDelta -= 10;
                    ModTempo();
                    break;

                case Keys.D6:
                case Keys.OemMinus:
                case Keys.Subtract:
                    if (TempoDelta < 400)
                        TempoDelta += 10;
                    ModTempo();
                    break;
            }
        }

        /// <summary>
        /// Speed up 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoPlus_Click(object sender, EventArgs e)
        {
            if (TempoDelta > 10)
                TempoDelta -= 10;
            ModTempo();
        }

        /// <summary>
        /// Minus speed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTempoMinus_Click(object sender, EventArgs e)
        {
            if (TempoDelta < 400)
                TempoDelta += 10;
            ModTempo();
        }

        private void ModTempo()
        {
            int tempo = TempoDelta * TempoOrig / 100;

            // If no change => out
            if (tempo == sequence1.Tempo)
                return;


            lblTempoValue.Text = string.Format("{0}%", TempoDelta);
            _bpm = GetBPM(tempo);
            lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", tempo, _bpm);

            // Stop sequencer if it was playing
            if (PlayerState == PlayerStates.Playing)
                sequencer1.Stop();

            _tempo = tempo;
            sequence1.Tempo = _tempo;
            sequence1.Time = new TimeSignature(sequence1.Numerator, sequence1.Denominator, sequence1.Division, _tempo);

            // Remove all tempo events
            foreach (Track trk in sequence1.tracks)
            {
                trk.RemoveTempoEvent();
            }

            // Insert new tempo event in track 0
            sequence1.tracks[0].insertTempo(_tempo);

            // Update Midi Times
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            _bpm = GetBPM(_tempo);
            // Update display duration
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);

            // Restart sequencer if it was playing
            if (PlayerState == PlayerStates.Playing)
                sequencer1.Continue();

        }



        #endregion
      
    }

  



}
