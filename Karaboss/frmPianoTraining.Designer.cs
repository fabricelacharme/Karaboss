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
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.vPianoRollControl1 = new Sanford.Multimedia.Midi.VPianoRoll.VPianoRollControl();
            this.pnlPiano = new System.Windows.Forms.Panel();
            this.pianoControl1 = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.pnlRedPianoSep = new System.Windows.Forms.Panel();
            this.pnlLeftPiano = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblKaraboss = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.BtnMinusX = new System.Windows.Forms.Button();
            this.BtnPlusX = new System.Windows.Forms.Button();
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
            this.pnlRight = new System.Windows.Forms.Panel();
            this.BtnMinusY = new System.Windows.Forms.Button();
            this.BtnPlusY = new System.Windows.Forms.Button();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlScrollView.SuspendLayout();
            this.pnlPiano.SuspendLayout();
            this.pnlLeftPiano.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlScrollView
            // 
            resources.ApplyResources(this.pnlScrollView, "pnlScrollView");
            this.pnlScrollView.BackColor = System.Drawing.Color.Gray;
            this.pnlScrollView.Controls.Add(this.vScrollBar);
            this.pnlScrollView.Controls.Add(this.vPianoRollControl1);
            this.pnlScrollView.Name = "pnlScrollView";
            // 
            // vScrollBar
            // 
            resources.ApplyResources(this.vScrollBar, "vScrollBar");
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarRoll_Scroll);
            this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBarRoll_ValueChanged);
            // 
            // vPianoRollControl1
            // 
            this.vPianoRollControl1.BackColor = System.Drawing.Color.Gray;
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
            this.pnlPiano.BackColor = System.Drawing.Color.Gray;
            this.pnlPiano.Controls.Add(this.pianoControl1);
            this.pnlPiano.Controls.Add(this.pnlRedPianoSep);
            this.pnlPiano.Controls.Add(this.pnlLeftPiano);
            resources.ApplyResources(this.pnlPiano, "pnlPiano");
            this.pnlPiano.Name = "pnlPiano";
            // 
            // pianoControl1
            // 
            this.pianoControl1.BackColor = System.Drawing.Color.Gray;
            this.pianoControl1.HighNoteID = 42;
            resources.ApplyResources(this.pianoControl1, "pianoControl1");
            this.pianoControl1.LowNoteID = 21;
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
            this.pnlLeftPiano.Controls.Add(this.label7);
            this.pnlLeftPiano.Controls.Add(this.label6);
            this.pnlLeftPiano.Controls.Add(this.label5);
            this.pnlLeftPiano.Controls.Add(this.label4);
            this.pnlLeftPiano.Controls.Add(this.label3);
            this.pnlLeftPiano.Controls.Add(this.label2);
            this.pnlLeftPiano.Controls.Add(this.label1);
            this.pnlLeftPiano.Controls.Add(this.lblKaraboss);
            resources.ApplyResources(this.pnlLeftPiano, "pnlLeftPiano");
            this.pnlLeftPiano.Name = "pnlLeftPiano";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Name = "label6";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // lblKaraboss
            // 
            resources.ApplyResources(this.lblKaraboss, "lblKaraboss");
            this.lblKaraboss.BackColor = System.Drawing.Color.Transparent;
            this.lblKaraboss.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblKaraboss.ForeColor = System.Drawing.Color.White;
            this.lblKaraboss.Name = "lblKaraboss";
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
            this.pnlBottom.Controls.Add(this.BtnMinusX);
            this.pnlBottom.Controls.Add(this.BtnPlusX);
            this.pnlBottom.Controls.Add(this.CbTracks);
            this.pnlBottom.Controls.Add(this.pnlDisplay);
            this.pnlBottom.Controls.Add(this.lblTempo);
            this.pnlBottom.Controls.Add(this.btnTempoMinus);
            this.pnlBottom.Controls.Add(this.lblTempoValue);
            this.pnlBottom.Controls.Add(this.btnTempoPlus);
            this.pnlBottom.Controls.Add(this.BtnStop);
            this.pnlBottom.Controls.Add(this.BtnPlay);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
            // 
            // BtnMinusX
            // 
            resources.ApplyResources(this.BtnMinusX, "BtnMinusX");
            this.BtnMinusX.Name = "BtnMinusX";
            this.BtnMinusX.TabStop = false;
            this.BtnMinusX.UseVisualStyleBackColor = true;
            this.BtnMinusX.Click += new System.EventHandler(this.BtnMinusX_Click);
            // 
            // BtnPlusX
            // 
            resources.ApplyResources(this.BtnPlusX, "BtnPlusX");
            this.BtnPlusX.Name = "BtnPlusX";
            this.BtnPlusX.TabStop = false;
            this.BtnPlusX.UseVisualStyleBackColor = true;
            this.BtnPlusX.Click += new System.EventHandler(this.BtnPlusX_Click);
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
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlRight.Controls.Add(this.BtnMinusY);
            this.pnlRight.Controls.Add(this.BtnPlusY);
            resources.ApplyResources(this.pnlRight, "pnlRight");
            this.pnlRight.Name = "pnlRight";
            // 
            // BtnMinusY
            // 
            resources.ApplyResources(this.BtnMinusY, "BtnMinusY");
            this.BtnMinusY.Name = "BtnMinusY";
            this.BtnMinusY.TabStop = false;
            this.BtnMinusY.UseVisualStyleBackColor = true;
            this.BtnMinusY.Click += new System.EventHandler(this.BtnMinusY_Click);
            // 
            // BtnPlusY
            // 
            resources.ApplyResources(this.BtnPlusY, "BtnPlusY");
            this.BtnPlusY.Name = "BtnPlusY";
            this.BtnPlusY.TabStop = false;
            this.BtnPlusY.UseVisualStyleBackColor = true;
            this.BtnPlusY.Click += new System.EventHandler(this.BtnPlusY_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.Gray;
            this.pnlMiddle.Controls.Add(this.pnlScrollView);
            this.pnlMiddle.Controls.Add(this.pnlPiano);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
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
            this.pnlLeftPiano.ResumeLayout(false);
            this.pnlLeftPiano.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlRight.ResumeLayout(false);
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
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlLeftPiano;
        private System.Windows.Forms.VScrollBar vScrollBar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BtnMinusX;
        private System.Windows.Forms.Button BtnPlusX;
        private System.Windows.Forms.Button BtnMinusY;
        private System.Windows.Forms.Button BtnPlusY;
    }
}