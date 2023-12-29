using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using ColorSlider;
using Karaboss;
using ChordAnalyser.Properties;


namespace ChordAnalyser.UI
{
    public partial class frmDisplayChords : Form
    {


        #region private
        private Sequence sequence1 = new Sequence();
        private ChordAnalyser.UI.ChordsControl chordAnalyserControl1;

        private Panel pnlTop;
        private Panel pnlDisplay;
        private Panel pnlBottom;

        private ColorSlider.ColorSlider positionHScrollBar;

        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private int _totalTicks = 0;
        private int _bpm = 0;
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;


        #endregion private


        public frmDisplayChords(Sequence seq)
        {
            InitializeComponent();
            this.sequence1 = seq;
            UpdateMidiTimes();

            DisplayChordControl();

        }

        private void DisplayChordControl()
        {
            // Panel Top
            pnlTop = new Panel();
            pnlTop.Location = new Point(0, 0);
            pnlTop.Size = new Size(this.Width, 100);
            pnlTop.BackColor = Color.Green;
            pnlTop.Dock = DockStyle.Top;
            this.Controls.Add(pnlTop);

            // Panel Bottom
            pnlBottom = new Panel();
            pnlBottom.Location = new Point(0, 0);
            pnlBottom.Size = new Size(this.Width, 100);
            pnlBottom.BackColor = Color.Red;
            pnlBottom.Dock = DockStyle.Bottom;
            this.Controls.Add(pnlBottom);


            // Panel Display
            pnlDisplay = new Panel();   
            pnlDisplay.Location = new Point(0, pnlTop.Height);
            pnlDisplay.Size = new Size(this.Width, 300);
            pnlDisplay.BackColor = Color.FromArgb(70, 77, 95);            
            this.Controls.Add(pnlDisplay);


            // MIDDLE

            // ChordControl
            chordAnalyserControl1 = new ChordsControl();
            chordAnalyserControl1.Sequence1 = this.sequence1;
            //chordAnalyserControl1.Size = new Size(chordAnalyserControl1.maxStaffWidth, 80);
            chordAnalyserControl1.Size = new Size(this.Width, 80);
            chordAnalyserControl1.Location = new Point(0, 0);
            chordAnalyserControl1.WidthChanged += new WidthChangedEventHandler(chordAnalyserControl1_WidthChanged);

            pnlDisplay.Controls.Add(chordAnalyserControl1);
            

            // positionHScrollBar
            positionHScrollBar = new ColorSlider.ColorSlider();
            positionHScrollBar.ThumbImage = Resources.BTN_Thumb_Blue;
            positionHScrollBar.Size = new Size(this.Width - 16, 20);
            positionHScrollBar.Location = new Point(0, chordAnalyserControl1.Height);
            positionHScrollBar.Value = 0;
            positionHScrollBar.Minimum = 0;
            positionHScrollBar.Maximum = NbMeasures * sequence1.Numerator;
            positionHScrollBar.TickStyle = TickStyle.None;
            //positionHScrollBar.ScaleDivisions = NbMeasures;
            //positionHScrollBar.ScaleSubDivisions = sequence1.Numerator - 1;
            positionHScrollBar.SmallChange = 1;
            positionHScrollBar.LargeChange = 1;// NbMeasures * sequence1.Numerator;
            positionHScrollBar.ShowDivisionsText = false;
            positionHScrollBar.ShowSmallScale = false;
            //positionHScrollBar.MouseWheelBarPartitions = NbMeasures * sequence1.Numerator;
            //positionHScrollBar.TickDivide = 0;
            positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PositionHScrollBar_Scroll);
            pnlDisplay.Controls.Add(positionHScrollBar);
            

        }



        #region HSCrollBar
        private void PositionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            //throw new NotImplementedException();
            chordAnalyserControl1.OffsetX = e.NewValue;
        }


        /// <summary>
        /// Set positionHScrollbar Width equal to chord control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void chordAnalyserControl1_WidthChanged(object sender, int value)
        {
            //positionHScrollBar.Width = value;
        }


        #endregion HScrollBar



        #region Form
        private void frmDisplayChords_Resize(object sender, EventArgs e)
        {
            pnlDisplay.Width = this.Width;
            positionHScrollBar.Width = (this.Width > chordAnalyserControl1.Width ? chordAnalyserControl1.Width : this.Width - 16) ;

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
                NbMeasures = _totalTicks / _measurelen;
            }
        }

        #endregion Midi
    }
}
