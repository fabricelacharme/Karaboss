namespace Karaboss
{
    partial class frmPianoTraining
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPianoTraining));
            this.pnlScrollView = new System.Windows.Forms.Panel();
            this.vScrollBarRoll = new System.Windows.Forms.VScrollBar();
            this.vPianoRollControl1 = new Sanford.Multimedia.Midi.VPianoRoll.VPianoRollControl();
            this.pnlPiano = new System.Windows.Forms.Panel();
            this.pianoControl1 = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.pnlRedPianoSep = new System.Windows.Forms.Panel();
            this.pnlLeftPiano = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.colorSliderX = new ColorSlider.ColorSlider();
            this.CbTracks = new System.Windows.Forms.ComboBox();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.btnTempoMinus = new System.Windows.Forms.Button();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.btnTempoPlus = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.lblKaraboss = new System.Windows.Forms.Label();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.colorSliderY = new ColorSlider.ColorSlider();
            this.pnlPianoroll = new System.Windows.Forms.Panel();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.hScrollBarRoll = new System.Windows.Forms.HScrollBar();
            this.pnlScrollView.SuspendLayout();
            this.pnlPiano.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlPianoroll.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlScrollView
            // 
            resources.ApplyResources(this.pnlScrollView, "pnlScrollView");
            this.pnlScrollView.BackColor = System.Drawing.Color.Gray;
            this.pnlScrollView.Controls.Add(this.hScrollBarRoll);
            this.pnlScrollView.Controls.Add(this.vScrollBarRoll);
            this.pnlScrollView.Controls.Add(this.vPianoRollControl1);
            this.pnlScrollView.Name = "pnlScrollView";
            // 
            // vScrollBarRoll
            // 
            resources.ApplyResources(this.vScrollBarRoll, "vScrollBarRoll");
            this.vScrollBarRoll.Name = "vScrollBarRoll";
            this.vScrollBarRoll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarRoll_Scroll);
            this.vScrollBarRoll.ValueChanged += new System.EventHandler(this.vScrollBarRoll_ValueChanged);
            // 
            // vPianoRollControl1
            // 
            this.vPianoRollControl1.HighNoteID = 108;
            resources.ApplyResources(this.vPianoRollControl1, "vPianoRollControl1");
            this.vPianoRollControl1.LowNoteID = 23;
            this.vPianoRollControl1.Name = "vPianoRollControl1";
            this.vPianoRollControl1.OffsetX = 0;
            this.vPianoRollControl1.OffsetY = 0;
            this.vPianoRollControl1.Resolution = 4;
            this.vPianoRollControl1.Sequence1 = null;
            this.vPianoRollControl1.TimeLineX = 40;
            this.vPianoRollControl1.TrackNum = -1;
            this.vPianoRollControl1.xScale = 0.1D;
            this.vPianoRollControl1.yScale = 0D;
            // 
            // pnlPiano
            // 
            this.pnlPiano.Controls.Add(this.pianoControl1);
            this.pnlPiano.Controls.Add(this.pnlRedPianoSep);
            this.pnlPiano.Controls.Add(this.pnlLeftPiano);
            resources.ApplyResources(this.pnlPiano, "pnlPiano");
            this.pnlPiano.Name = "pnlPiano";
            // 
            // pianoControl1
            // 
            resources.ApplyResources(this.pianoControl1, "pianoControl1");
            this.pianoControl1.HighNoteID = 108;
            this.pianoControl1.LowNoteID = 23;
            this.pianoControl1.Name = "pianoControl1";
            this.pianoControl1.NoteOnColor = System.Drawing.Color.SkyBlue;
            this.pianoControl1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.pianoControl1.Scale = 20;
            this.pianoControl1.Zoom = 1F;
            // 
            // pnlRedPianoSep
            // 
            this.pnlRedPianoSep.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.pnlRedPianoSep, "pnlRedPianoSep");
            this.pnlRedPianoSep.Name = "pnlRedPianoSep";
            // 
            // pnlLeftPiano
            // 
            this.pnlLeftPiano.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.pnlLeftPiano, "pnlLeftPiano");
            this.pnlLeftPiano.Name = "pnlLeftPiano";
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.positionHScrollBar);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // positionHScrollBar
            // 
            this.positionHScrollBar.BackColor = System.Drawing.Color.Transparent;
            this.positionHScrollBar.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.positionHScrollBar.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.positionHScrollBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.positionHScrollBar.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.positionHScrollBar.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            resources.ApplyResources(this.positionHScrollBar, "positionHScrollBar");
            this.positionHScrollBar.ForeColor = System.Drawing.Color.White;
            this.positionHScrollBar.LargeChange = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.positionHScrollBar.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.positionHScrollBar.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBar.MouseWheelBarPartitions = 1;
            this.positionHScrollBar.Name = "positionHScrollBar";
            this.positionHScrollBar.ScaleDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBar.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBar.ShowDivisionsText = true;
            this.positionHScrollBar.ShowSmallScale = false;
            this.positionHScrollBar.SmallChange = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.positionHScrollBar.TabStop = false;
            this.positionHScrollBar.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.positionHScrollBar.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.ThumbSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.TickAdd = 0F;
            this.positionHScrollBar.TickColor = System.Drawing.Color.White;
            this.positionHScrollBar.TickDivide = 0F;
            this.positionHScrollBar.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBar.ValueChanged += new System.EventHandler(this.positionHScrollBar_ValueChanged);
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlBottom.Controls.Add(this.colorSliderX);
            this.pnlBottom.Controls.Add(this.CbTracks);
            this.pnlBottom.Controls.Add(this.pnlDisplay);
            this.pnlBottom.Controls.Add(this.lblTempo);
            this.pnlBottom.Controls.Add(this.btnTempoMinus);
            this.pnlBottom.Controls.Add(this.lblTempoValue);
            this.pnlBottom.Controls.Add(this.btnTempoPlus);
            this.pnlBottom.Controls.Add(this.BtnStop);
            this.pnlBottom.Controls.Add(this.BtnPlay);
            this.pnlBottom.Controls.Add(this.lblKaraboss);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
            // 
            // colorSliderX
            // 
            this.colorSliderX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.colorSliderX.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.colorSliderX.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.colorSliderX.BorderRoundRectSize = new System.Drawing.Size(1, 1);
            resources.ApplyResources(this.colorSliderX, "colorSliderX");
            this.colorSliderX.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderX.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.colorSliderX.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.colorSliderX.ForeColor = System.Drawing.Color.White;
            this.colorSliderX.LargeChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderX.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.colorSliderX.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.colorSliderX.MouseWheelBarPartitions = 20;
            this.colorSliderX.Name = "colorSliderX";
            this.colorSliderX.ScaleDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderX.ScaleSubDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderX.ShowDivisionsText = false;
            this.colorSliderX.ShowSmallScale = false;
            this.colorSliderX.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderX.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderX.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderX.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
            this.colorSliderX.ThumbSize = new System.Drawing.Size(7, 20);
            this.colorSliderX.TickAdd = 0F;
            this.colorSliderX.TickColor = System.Drawing.Color.White;
            this.colorSliderX.TickDivide = 0F;
            this.colorSliderX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.colorSliderX.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.colorSliderX.ValueChanged += new System.EventHandler(this.colorSliderX_ValueChanged);
            // 
            // CbTracks
            // 
            this.CbTracks.FormattingEnabled = true;
            resources.ApplyResources(this.CbTracks, "CbTracks");
            this.CbTracks.Name = "CbTracks";
            this.CbTracks.TabStop = false;
            this.CbTracks.SelectedIndexChanged += new System.EventHandler(this.CbTracks_SelectedIndexChanged);
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblDuration);
            resources.ApplyResources(this.pnlDisplay, "pnlDisplay");
            this.pnlDisplay.Name = "pnlDisplay";
            // 
            // lblElapsed
            // 
            this.lblElapsed.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblElapsed, "lblElapsed");
            this.lblElapsed.ForeColor = System.Drawing.Color.White;
            this.lblElapsed.Name = "lblElapsed";
            // 
            // lblPercent
            // 
            resources.ApplyResources(this.lblPercent, "lblPercent");
            this.lblPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblPercent.ForeColor = System.Drawing.Color.White;
            this.lblPercent.Name = "lblPercent";
            // 
            // lblDuration
            // 
            this.lblDuration.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblDuration, "lblDuration");
            this.lblDuration.ForeColor = System.Drawing.Color.White;
            this.lblDuration.Name = "lblDuration";
            // 
            // lblTempo
            // 
            this.lblTempo.BackColor = System.Drawing.Color.Black;
            this.lblTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempo.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblTempo, "lblTempo");
            this.lblTempo.Name = "lblTempo";
            // 
            // btnTempoMinus
            // 
            resources.ApplyResources(this.btnTempoMinus, "btnTempoMinus");
            this.btnTempoMinus.Name = "btnTempoMinus";
            this.btnTempoMinus.UseVisualStyleBackColor = true;
            this.btnTempoMinus.Click += new System.EventHandler(this.btnTempoMinus_Click);
            // 
            // lblTempoValue
            // 
            this.lblTempoValue.BackColor = System.Drawing.Color.Black;
            this.lblTempoValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempoValue.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblTempoValue, "lblTempoValue");
            this.lblTempoValue.Name = "lblTempoValue";
            // 
            // btnTempoPlus
            // 
            resources.ApplyResources(this.btnTempoPlus, "btnTempoPlus");
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.UseVisualStyleBackColor = true;
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Image = global::Karaboss.Properties.Resources.Media_Controls_Stop_icon;
            resources.ApplyResources(this.BtnStop, "BtnStop");
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.TabStop = false;
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BtnPlay
            // 
            this.BtnPlay.Image = global::Karaboss.Properties.Resources.Media_Controls_Play_icon;
            resources.ApplyResources(this.BtnPlay, "BtnPlay");
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.TabStop = false;
            this.BtnPlay.UseVisualStyleBackColor = true;
            this.BtnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // lblKaraboss
            // 
            resources.ApplyResources(this.lblKaraboss, "lblKaraboss");
            this.lblKaraboss.BackColor = System.Drawing.Color.Transparent;
            this.lblKaraboss.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKaraboss.ForeColor = System.Drawing.Color.White;
            this.lblKaraboss.Name = "lblKaraboss";
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlRight.Controls.Add(this.colorSliderY);
            resources.ApplyResources(this.pnlRight, "pnlRight");
            this.pnlRight.Name = "pnlRight";
            // 
            // colorSliderY
            // 
            this.colorSliderY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.colorSliderY.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.colorSliderY.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.colorSliderY.BorderRoundRectSize = new System.Drawing.Size(1, 1);
            resources.ApplyResources(this.colorSliderY, "colorSliderY");
            this.colorSliderY.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderY.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.colorSliderY.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.colorSliderY.ForeColor = System.Drawing.Color.White;
            this.colorSliderY.LargeChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.colorSliderY.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.colorSliderY.MouseWheelBarPartitions = 1000;
            this.colorSliderY.Name = "colorSliderY";
            this.colorSliderY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.colorSliderY.ScaleDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderY.ScaleSubDivisions = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderY.ShowDivisionsText = false;
            this.colorSliderY.ShowSmallScale = false;
            this.colorSliderY.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.colorSliderY.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderY.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.colorSliderY.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
            this.colorSliderY.ThumbSize = new System.Drawing.Size(20, 7);
            this.colorSliderY.TickAdd = 0F;
            this.colorSliderY.TickColor = System.Drawing.Color.White;
            this.colorSliderY.TickDivide = 0F;
            this.colorSliderY.TickStyle = System.Windows.Forms.TickStyle.None;
            this.colorSliderY.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.colorSliderY.ValueChanged += new System.EventHandler(this.colorSliderY_ValueChanged);
            // 
            // pnlPianoroll
            // 
            this.pnlPianoroll.Controls.Add(this.pnlScrollView);
            resources.ApplyResources(this.pnlPianoroll, "pnlPianoroll");
            this.pnlPianoroll.Name = "pnlPianoroll";
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.pnlPianoroll);
            this.pnlMiddle.Controls.Add(this.pnlPiano);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
            // 
            // hScrollBarRoll
            // 
            resources.ApplyResources(this.hScrollBarRoll, "hScrollBarRoll");
            this.hScrollBarRoll.Name = "hScrollBarRoll";
            // 
            // frmPianoTraining
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "frmPianoTraining";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPianoTraining_FormClosing);
            this.Load += new System.EventHandler(this.frmPianoTraining_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPianoTraining_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmPianoTraining_KeyUp);
            this.Resize += new System.EventHandler(this.frmPianoTraining_Resize);
            this.pnlScrollView.ResumeLayout(false);
            this.pnlPiano.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlPianoroll.ResumeLayout(false);
            this.pnlMiddle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlScrollView;
        private System.Windows.Forms.Panel pnlPiano;
        private System.Windows.Forms.Panel pnlTop;
        private Sanford.Multimedia.Midi.VPianoRoll.VPianoRollControl vPianoRollControl1;
        private Sanford.Multimedia.Midi.UI.PianoControl pianoControl1;
        private System.Windows.Forms.Timer timer1;
        private ColorSlider.ColorSlider positionHScrollBar;
        private System.Windows.Forms.Panel pnlRedPianoSep;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Button btnTempoMinus;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Button btnTempoPlus;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Button BtnPlay;
        private System.Windows.Forms.Label lblKaraboss;
        private System.Windows.Forms.ComboBox CbTracks;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlPianoroll;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlLeftPiano;
        private System.Windows.Forms.VScrollBar vScrollBarRoll;
        private ColorSlider.ColorSlider colorSliderY;
        private ColorSlider.ColorSlider colorSliderX;
        private System.Windows.Forms.HScrollBar hScrollBarRoll;
    }
}