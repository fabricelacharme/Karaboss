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
        //private int _timeLineHeight = 40;        
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

        //private int bouclestart = 0;
        private int newstart = 0;
        //private int laststart = 0;      // Start time to play
        private int nbstop = 0;

        private enum Editstatus
        {
            None,
            Arrow,
            Pen,
        }
        //private Editstatus EditStatus;

        private string CurrentPath = string.Empty;        
        

        #endregion

        #region controls
        private frmPlayer frmPlayer;
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();
        private Sequence sequence1 = new Sequence();

        // Creation dynamique de controles           
        VScrollBar vScrollBar;
        HScrollBar hScrollBar;

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

            // Draw notes
            pianoRollControl2.TrackNum = tracknum;

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

            // --------------------------------
            // CbResolution
            // --------------------------------
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

            // ----------------------------------
            // posisitionHScrollBar
            // ----------------------------------
            positionHScrollBar.Left = pnlPianoTop.Width;
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


            #region left piano control

            // ------------------------------------
            // PianoControl
            // ------------------------------------
            pianoControl2.Left = 0;
            pianoControl2.Top = 0;

            // Notes du piano
            pianoControl2.LowNoteID = pianoRollControl2.LowNoteID = 23;     //  23
            pianoControl2.HighNoteID = pianoRollControl2.HighNoteID = 108;  // 108            


            pianoControl2.Size = new Size(leftWidth, pianoControl2.TotalLength);           
            pianoControl2.PianoKeyDown += new EventHandler<PianoKeyEventArgs>(pianoControl2_PianoKeyDown);
            pianoControl2.PianoKeyUp += new EventHandler<PianoKeyEventArgs>(pianoControl2_PianoKeyUp);

            // --------------------
            // pnlPiano
            // --------------------
            pnlPiano.Top = pnlPianoTop.Height;
            pnlPiano.Left = 0;
            pnlPiano.Width = leftWidth;
            pnlPiano.Height = pianoControl2.TotalLength;

            #endregion left


            #region right  pianoRollControl     

            // ---------------------------------------------
            // pnlScrollView
            // ---------------------------------------------
            pnlScrollView.Top = 0;
            pnlScrollView.Left = 0;
            pnlScrollView.Width = pnlCenter.Width;
            pnlScrollView.Height = pnlPiano.Height + pianoRollControl2.TimeLineY;


            // -------------------------
            // Horizontal scrollbar
            // ------------------------- 
            hScrollBar = new HScrollBar() {
                Parent = pnlCenter,
                Left = 0,
                Top = 0,
                Minimum = 0,
            };
                        
            
            pnlCenter.Controls.Add(hScrollBar);
            hScrollBar.BringToFront();
            hScrollBar.Dock = DockStyle.Bottom;

            hScrollBar.Scroll += new ScrollEventHandler(HScrollBar_Scroll);
            hScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);

            hScrollBar.Maximum = _totalTicks + 2 * _measurelen;
            hScrollBar.Minimum = _measurelen;
            hScrollBar.SmallChange = _measurelen / pianoRollControl2.Resolution;
            hScrollBar.LargeChange = _measurelen;
            hScrollBar.Value = hScrollBar.Minimum;



            // ----------------------
            // Vertical Scrollbar
            // ----------------------
            vScrollBar = new VScrollBar() {
                Parent = pnlCenter,
                Top = 0,
                Left = 0,
                Minimum = 0,
            };
            
            pnlCenter.Controls.Add(vScrollBar);
            vScrollBar.BringToFront();            
            vScrollBar.Dock = DockStyle.Right;            

            vScrollBar.Scroll += new ScrollEventHandler(VScrollBar_Scroll);
            vScrollBar.ValueChanged += new EventHandler(VScrollBar_ValueChanged);

            vScrollBar.Maximum = pianoControl2.Height;
            vScrollBar.Minimum = 0;            
            vScrollBar.Value = vScrollBar.Minimum;

            // --------------------------------------------------
            // PianoRollControl on the pnlScrollView
            // --------------------------------------------------
            pianoRollControl2.Top = 0;
            pianoRollControl2.Left = 0;

            pianoRollControl2.MouseDownPlayNote += new MouseDownPlayNoteEventHandler(pianoRollControl2_MouseDownPlayNote);
            pianoRollControl2.MouseUpStopNote += new MouseUpStopNoteEventHandler(pianoRollControl2_MouseUpStopNote);
            pianoRollControl2.SequenceModified += new EventHandler(pianoRollControl2_SequenceModified);            
            pianoRollControl2.InfoNote += new InfoNoteEventHandler(pianoRollControl2_InfoNote);

            pianoRollControl2.OnMouseMoved += new Sanford.Multimedia.Midi.PianoRoll.MouseMoveEventHandler(pianoRollControl2_MouseMove);

            // properties
            pianoRollControl2.Velocity = Karaclass.m_Velocity;
            
           
            
            // Sequence pour pianoRoll 
            pianoRollControl2.Sequence1 = sequence1;

            pianoRollControl2.yScale = yScale;
            pianoControl2.Scale = yScale;

            pianoRollControl2.zoomx = zoomx;
            pianoControl2.Zoom = zoomy;

            pianoRollControl2.yScale = pianoControl2.Scale;

            pianoRollControl2.Width = pnlScrollView.Width;
            pianoRollControl2.Height = pianoControl2.Height + pianoRollControl2.TimeLineY;
            

            #endregion right

            SetScrollBarValues();
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
            if (pianoRollControl2 == null)
                return;

            sequencer1.AllSoundOff();
            pianoControl2.Reset();

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

            lblEdit.Visible = !bAlltracks;
            lblSaisieNotes.Visible = !bAlltracks;

            // Track pour pianoRoll
            if (tracknum != -1)
            {                
                pianoRollControl2.TrackNum = tracknum;                
            }
            else
            {
                pianoRollControl2.TrackNum = -1;
            }
            CbTracks.Parent.Focus();
        }
        
        #endregion  


        #region scroll events
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            pnlPiano.Top = -vScrollBar.Value + pianoRollControl2.TimeLineY;
            pnlScrollView.Top = -vScrollBar.Value;
            pianoRollControl2.OffsetY = -vScrollBar.Value;
            pnlPianoTop.BringToFront();
        }

        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            pnlPiano.Top = -vScrollBar.Value + pianoRollControl2.TimeLineY;
            pnlScrollView.Top = -vScrollBar.Value;
            pianoRollControl2.OffsetY = -vScrollBar.Value;
            pnlPianoTop.BringToFront();
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.Type == ScrollEventType.EndScroll)
            {
                if (hScrollBar.Value > positionHScrollBar.Maximum)
                    hScrollBar.Value = (int)positionHScrollBar.Maximum;

                positionHScrollBar.Value = hScrollBar.Value;
            }
        }

        private void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (hScrollBar.Value > positionHScrollBar.Maximum)
                hScrollBar.Value = (int)positionHScrollBar.Maximum;

            positionHScrollBar.Value = hScrollBar.Value;
        }

        /// <summary>
        /// Show or Hide scrollBars
        /// </summary>
        private void SetScrollBarValues()
        {
            if (pnlScrollView == null || pianoRollControl2 == null)
                return;

            int hH = 0;
            int vW = 0;
            int v = 0;

            // Width of pianoRollControl
            int W = pianoRollControl2.maxStaffWidth;
            // Display width
            int wMiddle = pnlCenter.Width;

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
            if (pnlCenter.Height > pnlScrollView.Height)
                bShowVScrollBar = false;
            else if (pnlCenter.Height < pnlScrollView.Height)
                bShowVScrollBar = true;
            else
                bShowVScrollBarIndetermined = true;


            if (bShowVScrollBarIndetermined && bShowHScrollBarIndetermined)
            {
                bShowHScrollBar = false;
                bShowVScrollBar = false;
            }
            else if (bShowVScrollBarIndetermined && pnlCenter.Height > pnlScrollView.Height)
                bShowVScrollBar = false;
            else if (bShowHScrollBarIndetermined && wMiddle > W)
                bShowHScrollBar = false;


            if (bShowVScrollBar && wMiddle - vScrollBar.Width < W)
                bShowHScrollBar = true;

            if (bShowHScrollBar && pnlCenter.Height - hScrollBar.Height < pnlScrollView.Height)
                bShowVScrollBar = true;

            if (bShowVScrollBar == false)
            {
                vScrollBar.Visible = false;
                pnlPiano.Top = pianoRollControl2.TimeLineY;
                pnlScrollView.Top = 0;
            }
            else
            {
                vScrollBar.Visible = true;
                vScrollBar.Left = pnlCenter.Width - vScrollBar.Width;               // position de la vertical scroll bar
                if (bShowHScrollBar)
                    vScrollBar.Height = pnlCenter.Height - hScrollBar.Height;
                else
                    vScrollBar.Height = pnlCenter.Height;
            }

            if (bShowHScrollBar == false)
            {
                hScrollBar.Visible = false;
                //pnlHScroll.Visible = false;                                                
                pianoRollControl2.OffsetX = 0;
                           
            }
            else
            {
                hScrollBar.Visible = true;
                //pnlHScroll.Visible = true;
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
                vScrollBar.Maximum = pnlScrollView.Height - (pnlCenter.Height - hH) + vScrollBar.LargeChange;

                // Aggrandissement vertical de la fenetre = > la scrollbar verticale doit se positionner en bas
                if (pianoRollControl2.Height - vScrollBar.Value < pnlCenter.Height)
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
                
            }
        }

        #endregion scroll events


        #region Edit

        private void DisableEditButtons()
        {
            // None selection
            //EditStatus = Editstatus.None;
            Cursor = Cursors.Default;

            lblSaisieNotes.BackColor = Color.White;
            bEnterNotes = false;
            pianoRollControl2.NotesEdition = false;
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
                pianoRollControl2.NotesEdition = false;

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
                pianoRollControl2.NotesEdition = true;

                // Enter edit mode
                DspEdit(true);

                lblSaisieNotes.BackColor = Color.Red;            
                this.Cursor = Cursors.Hand;
               
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
            if (pianoControl2 != null)
                pianoRollControl2.Resolution = resolution;
        }

        #endregion Edit


        #region pianoRollControl

        private void pianoRollControl2_MouseMove(object sender, int note, MouseEventArgs e)
        {
            pianoControl2.ResetIsOver(note);
            pianoControl2.IsOverPianoKey(note);
        }


        private void pianoRollControl2_MouseDownPlayNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
        {
            #region Guard
            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, nnote, 127));
        }

        private void pianoRollControl2_MouseUpStopNote(int starttime, int channel, int nnote, int duration, MouseEventArgs e)
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
        private void pianoRollControl2_SequenceModified(Object sender, EventArgs e)
        {
            UpdateFrmPlayer();
        }

        private void pianoRollControl2_InfoNote(string NoteInfo)
        {
            lblNote.Text = NoteInfo;
        }

        #endregion pianoRollControl


        #region pianoControl

        protected override void OnKeyDown(KeyEventArgs e)
        {
            pianoControl2.PressPianoKey(e.KeyCode);
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            pianoControl2.ReleasePianoKey(e.KeyCode);
            base.OnKeyUp(e);
        }


        private void pianoControl2_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if (PlayerState == PlayerStates.Playing || outDevice == null)
            {
                return;
            }
            #endregion
            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void pianoControl2_PianoKeyUp(object sender, PianoKeyEventArgs e)
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
            pianoRollControl2.ManageKeyDown(sender, e);

        }

        private void FrmPianoRoll_KeyUp(object sender, KeyEventArgs e)
        {
            pianoRollControl2.ManageKeyUp(sender, e);

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

                zoomy = pianoControl2.Zoom;
                zoomy += (e.Delta > 0 ? 0.1f : -0.1f);
                pianoControl2.Zoom = zoomy;
                pianoControl2.Height = pianoControl2.TotalLength;

                pianoRollControl2.yScale = pianoControl2.Scale;
                pianoRollControl2.Height =  pianoControl2.Height + pianoRollControl2.TimeLineY;

                // Adjust heights                
                pnlPiano.Height = pianoControl2.TotalLength;
                pnlScrollView.Height = pianoControl2.TotalLength + pianoRollControl2.TimeLineY;

                
                // Adjust scrollbars values
                SetScrollBarValues();

            }            
            else if (rectPianoRoll.Contains(PpianoRoll))
            {
                // If Mouse over pianoRoll                                                      

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        break;
                    case PlayerStates.Paused:
                        break;
                    case PlayerStates.Stopped:
                        zoomx += (e.Delta > 0 ? 0.1f : -0.1f);
                        pianoRollControl2.zoomx = zoomx;
                        // Adjust scrollbar values
                        SetScrollBarValues();
                        break;
                }
                            
            }
        }

        private int SetProperValue(int val)
        {
            if (val < positionHScrollBar.Minimum) return (int)positionHScrollBar.Minimum;
            else if (val > positionHScrollBar.Maximum) return (int)positionHScrollBar.Maximum;
            else return val;
        }

        /// <summary>
        /// Window and scrollbar startup position
        /// </summary>
        public void StartupPosition(int ticks, int note = 0)
        {
            // VERTICAL
            if (note > 0)
            {
                float y = (note - pianoControl2.LowNoteID) / (float)(pianoControl2.HighNoteID - pianoControl2.LowNoteID);
                int middleScroll = (int)(y * vScrollBar.Maximum);
                vScrollBar.Value = vScrollBar.Maximum - middleScroll;
            }

            // HORIZONTAL                        
            float pcent = (float)ticks / sequence1.GetLength();
            uint v = (uint)(pcent * (float)(positionHScrollBar.Maximum - positionHScrollBar.Minimum)); ;
            if (positionHScrollBar.Minimum <= v && v <= positionHScrollBar.Maximum)
                positionHScrollBar.Value = (uint)(pcent * (float)(positionHScrollBar.Maximum - positionHScrollBar.Minimum));
        }

        private void FrmPianoRoll_Resize(object sender, EventArgs e)
        {
            int vW = 0;

            positionHScrollBar.Width = pnlTop.Width - positionHScrollBar.Left - 40;

            if (pnlScrollView != null && pnlScrollView.Parent != null)
            {
                if (bShowVScrollBar)
                    vW = vScrollBar.Width;
                
                pnlScrollView.Height = pnlPiano.Height + pianoRollControl2.TimeLineY;
                pnlScrollView.Width = pnlCenter.Width;
                pianoRollControl2.Width = pnlScrollView.Width;                
            }

            if (pianoRollControl2 != null)
                pianoRollControl2.Redraw();
            
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
                pianoRollControl2.zoomx = zoomx;                

                pianoControl2.Zoom = zoomy;
                pianoControl2.Height = pianoControl2.TotalLength;

                pianoRollControl2.yScale = pianoControl2.Scale;
                pianoRollControl2.Height = pianoControl2.Height + pianoRollControl2.TimeLineY;

                // Adjust heights
                //pnlPiano.Height = pnlScrollView.Height = pianoControl2.totalLength;
                pnlPiano.Height = pianoControl2.TotalLength;
                pnlScrollView.Height = pianoControl2.TotalLength + pianoRollControl2.TimeLineY;

                // Adjust scrollbars values
                SetScrollBarValues();
            }
            catch (Exception ex)
            {
                zoomx = 1.0f;
                zoomy = 1.0f;
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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


        #region positionHScrollBar

        private void positionHScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (scrolling) return;

            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    break;
                case PlayerStates.Paused:
                    break;
                case PlayerStates.Stopped:
                    newstart = (int)(positionHScrollBar.Value - positionHScrollBar.Minimum);
                    pianoRollControl2.OffsetX = Convert.ToInt32(newstart * pianoRollControl2.xScale);

                    if (hScrollBar != null && positionHScrollBar.Value < hScrollBar.Maximum)
                        hScrollBar.Value = (int)positionHScrollBar.Value;

                    double dpercent = 100 * newstart / (double)_totalTicks;
                    DisplayTimeElapse(dpercent);
                    break;
            }
        }

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
                        pianoRollControl2.OffsetX = Convert.ToInt32(newstart * pianoRollControl2.xScale);
                        nbstop = 0;

                        break;
                    case PlayerStates.Stopped:
                        newstart = e.NewValue - (int)positionHScrollBar.Minimum;
                        pianoRollControl2.OffsetX = Convert.ToInt32(newstart * pianoRollControl2.xScale);
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

        #endregion


        #region handle messages

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
                pianoControl2.Send(e.Message);
            }
            else if (e.Message.MidiChannel == SingleTrackChannel)
            {
                outDevice.Send(e.Message);
                pianoControl2.Send(e.Message);
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
                //pianoControl2.Send(message);
            }
        }
        

        #endregion


        #region Play stop pause

        private void BtnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();            
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            StopMusic();            
        }

        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    BtnPlay.Image = Properties.Resources.btn_green_play;
                    BtnStop.Image = Properties.Resources.btn_black_stop;
                    BtnPlay.Enabled = true;  // to allow pause
                    BtnStop.Enabled = true;  // to allow stop 
                    break;

                case PlayerStates.Paused:
                    BtnPlay.Image = Properties.Resources.btn_red_pause;
                    BtnPlay.Enabled = true;  // to allow play
                    BtnStop.Enabled = true;  // to allow stop
                    break;

                case PlayerStates.Stopped:
                    BtnPlay.Image = Properties.Resources.btn_black_play;
                    BtnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        BtnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                        BtnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
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
            //laststart = 0;
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
                        pianoControl2.Reset();
                        positionHScrollBar.Value = newstart + positionHScrollBar.Minimum;
                        hScrollBar.Value = (int)positionHScrollBar.Value;
                        pianoRollControl2.OffsetX = Convert.ToInt32(newstart * pianoRollControl2.xScale);                        
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
            
            pianoControl2.Reset();

            // Stopped to start of score
            if (newstart <= 0)
            {                
                positionHScrollBar.Value = positionHScrollBar.Minimum; 
                hScrollBar.Value = hScrollBar.Minimum;
                pianoRollControl2.OffsetX = 0;                
                //laststart = 0;                
            }
            else
            {
                // Stop to start point newstart (ticks)             
            }
        }

        #endregion


        #region timer   

        private void ScrollView()
        {
            int offset = Convert.ToInt32(sequencer1.Position * pianoRollControl2.xScale);
            pianoRollControl2.OffsetX = offset;

        }

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
                        //ScrollTimeBar();
                        ScrollView();
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
                    {
                        positionHScrollBar.Value = sequencer1.Position + positionHScrollBar.Minimum;
                        hScrollBar.Value = sequencer1.Position + hScrollBar.Minimum;
                    }

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
            int offset = Convert.ToInt32(sequencer1.Position * pianoRollControl2.xScale);
            pianoRollControl2.OffsetX = offset;
            
            if (offset > hScrollBar.Minimum && offset < hScrollBar.Maximum)
                hScrollBar.Value = pianoRollControl2.OffsetX;
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
            _tempo = TempoDelta * TempoOrig / 100;          

            // Change clock tempo
            sequencer1.Tempo = _tempo;

            lblTempoValue.Text = string.Format("{0}%", TempoDelta);

            // Update Midi Times            
            _bpm = GetBPM(_tempo);
            lblTempo.Text = string.Format("Tempo: {0} - BPM: {1}", _tempo, _bpm);

            // Update display duration
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);          

        }

        #endregion


        #region boutons
        private void BtnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                BtnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                BtnPlay.Image = Properties.Resources.btn_blue_pause;
            else if (PlayerState == PlayerStates.Playing)
                BtnPlay.Image = Properties.Resources.btn_blue_play;
        }

        private void BtnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                BtnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                BtnPlay.Image = Properties.Resources.btn_red_pause;
            else if (PlayerState == PlayerStates.Playing)
                BtnPlay.Image = Properties.Resources.btn_green_play;
        }

        private void BtnStop_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                BtnStop.Image = Properties.Resources.btn_blue_stop;
        }

        private void BtnStop_MouseLeave(object sender, EventArgs e)
        {
            BtnStop.Image = Properties.Resources.btn_black_stop;
        }

        #endregion
    
    }
}
