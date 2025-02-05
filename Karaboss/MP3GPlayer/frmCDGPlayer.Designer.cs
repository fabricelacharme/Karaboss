namespace Karaboss
{
    partial class frmCDGPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCDGPlayer));
            this.btRecord = new System.Windows.Forms.Button();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btBrowse = new System.Windows.Forms.Button();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbStop = new System.Windows.Forms.ToolStripButton();
            this.nudKey = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.trbVolume = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.VuMasterPeakVolume = new VU_MeterLibrary.VuMeter();
            this.sldMainVolume = new ColorSlider.ColorSlider();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lnlVol = new System.Windows.Forms.Label();
            this.lblMainVolume = new System.Windows.Forms.Label();
            this.pnlDisplay = new Karaboss.Display.PanelPlayer();
            this.btnPrev = new Karaboss.NoSelectButton();
            this.btnNext = new Karaboss.NoSelectButton();
            this.btnStop = new Karaboss.NoSelectButton();
            this.btnPlay = new Karaboss.NoSelectButton();
            this.ToolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).BeginInit();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btRecord
            // 
            this.btRecord.Location = new System.Drawing.Point(384, 97);
            this.btRecord.Name = "btRecord";
            this.btRecord.Size = new System.Drawing.Size(63, 24);
            this.btRecord.TabIndex = 6;
            this.btRecord.Text = "Record";
            this.btRecord.UseVisualStyleBackColor = true;
            this.btRecord.Click += new System.EventHandler(this.btRecord_Click);
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(3, 10);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(453, 20);
            this.tbFileName.TabIndex = 0;
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(461, 9);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(63, 23);
            this.btBrowse.TabIndex = 3;
            this.btBrowse.Text = "Browse";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbPlay,
            this.tsbPause,
            this.tsbStop});
            this.ToolStrip1.Location = new System.Drawing.Point(9, 52);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(81, 25);
            this.ToolStrip1.TabIndex = 5;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // tsbPlay
            // 
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Image = ((System.Drawing.Image)(resources.GetObject("tsbPlay.Image")));
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.Size = new System.Drawing.Size(23, 22);
            this.tsbPlay.Text = "Play";
            this.tsbPlay.Click += new System.EventHandler(this.tsbPlay_Click);
            // 
            // tsbPause
            // 
            this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPause.Image = ((System.Drawing.Image)(resources.GetObject("tsbPause.Image")));
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(23, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            // 
            // tsbStop
            // 
            this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStop.Image = ((System.Drawing.Image)(resources.GetObject("tsbStop.Image")));
            this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStop.Name = "tsbStop";
            this.tsbStop.Size = new System.Drawing.Size(23, 22);
            this.tsbStop.Text = "Stop";
            this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
            // 
            // nudKey
            // 
            this.nudKey.Location = new System.Drawing.Point(376, 8);
            this.nudKey.Name = "nudKey";
            this.nudKey.Size = new System.Drawing.Size(51, 20);
            this.nudKey.TabIndex = 4;
            this.nudKey.ValueChanged += new System.EventHandler(this.nudKey_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(335, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Key";
            // 
            // trbVolume
            // 
            this.trbVolume.Location = new System.Drawing.Point(194, 36);
            this.trbVolume.Maximum = 100;
            this.trbVolume.Name = "trbVolume";
            this.trbVolume.Size = new System.Drawing.Size(128, 45);
            this.trbVolume.TabIndex = 2;
            this.trbVolume.Value = 100;
            this.trbVolume.Scroll += new System.EventHandler(this.trbVolume_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(146, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Volume";
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.ToolStrip1);
            this.pnlTop.Controls.Add(this.trbVolume);
            this.pnlTop.Controls.Add(this.label1);
            this.pnlTop.Controls.Add(this.tbFileName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(843, 87);
            this.pnlTop.TabIndex = 4;
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // Timer1
            // 
            this.Timer1.Interval = 50;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
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
            this.positionHScrollBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.positionHScrollBar.ForeColor = System.Drawing.Color.White;
            this.positionHScrollBar.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBar.Location = new System.Drawing.Point(8, 54);
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
            this.positionHScrollBar.Name = "positionHScrollBar";
            this.positionHScrollBar.ScaleDivisions = new decimal(new int[] {
            10,
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
            this.positionHScrollBar.Size = new System.Drawing.Size(200, 20);
            this.positionHScrollBar.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.positionHScrollBar.TabIndex = 10;
            this.positionHScrollBar.TabStop = false;
            this.positionHScrollBar.Text = "colorSlider1";
            this.positionHScrollBar.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.positionHScrollBar.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.ThumbSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.TickAdd = 0F;
            this.positionHScrollBar.TickColor = System.Drawing.Color.White;
            this.positionHScrollBar.TickDivide = 0F;
            this.positionHScrollBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.positionHScrollBar.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            // 
            // VuMasterPeakVolume
            // 
            this.VuMasterPeakVolume.AnalogMeter = false;
            this.VuMasterPeakVolume.DialBackground = System.Drawing.Color.White;
            this.VuMasterPeakVolume.DialTextNegative = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.DialTextPositive = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.DialTextZero = System.Drawing.Color.DarkGreen;
            this.VuMasterPeakVolume.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuMasterPeakVolume.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.VuMasterPeakVolume.Led1Count = 6;
            this.VuMasterPeakVolume.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuMasterPeakVolume.Led2ColorOn = System.Drawing.Color.Yellow;
            this.VuMasterPeakVolume.Led2Count = 6;
            this.VuMasterPeakVolume.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuMasterPeakVolume.Led3ColorOn = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.Led3Count = 4;
            this.VuMasterPeakVolume.LedSize = new System.Drawing.Size(6, 14);
            this.VuMasterPeakVolume.LedSpace = 3;
            this.VuMasterPeakVolume.Level = 0;
            this.VuMasterPeakVolume.LevelMax = 65535;
            this.VuMasterPeakVolume.Location = new System.Drawing.Point(229, 7);
            this.VuMasterPeakVolume.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuMasterPeakVolume.Name = "VuMasterPeakVolume";
            this.VuMasterPeakVolume.NeedleColor = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.PeakHold = true;
            this.VuMasterPeakVolume.Peakms = 1000;
            this.VuMasterPeakVolume.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.ShowDialOnly = false;
            this.VuMasterPeakVolume.ShowLedPeak = false;
            this.VuMasterPeakVolume.ShowTextInDial = false;
            this.VuMasterPeakVolume.Size = new System.Drawing.Size(12, 275);
            this.VuMasterPeakVolume.TabIndex = 11;
            this.VuMasterPeakVolume.TextInDial = new string[] {
        "-40",
        "-20",
        "-10",
        "-5",
        "0",
        "+6"};
            this.VuMasterPeakVolume.UseLedLight = false;
            this.VuMasterPeakVolume.VerticalBar = true;
            this.VuMasterPeakVolume.VuText = "VU";
            // 
            // sldMainVolume
            // 
            this.sldMainVolume.BackColor = System.Drawing.Color.Transparent;
            this.sldMainVolume.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sldMainVolume.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sldMainVolume.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sldMainVolume.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.sldMainVolume.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.sldMainVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.sldMainVolume.ForeColor = System.Drawing.Color.White;
            this.sldMainVolume.LargeChange = new decimal(new int[] {
            26,
            0,
            0,
            0});
            this.sldMainVolume.Location = new System.Drawing.Point(249, 25);
            this.sldMainVolume.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.sldMainVolume.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.sldMainVolume.Name = "sldMainVolume";
            this.sldMainVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.sldMainVolume.ScaleDivisions = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.sldMainVolume.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sldMainVolume.ShowDivisionsText = false;
            this.sldMainVolume.ShowSmallScale = false;
            this.sldMainVolume.Size = new System.Drawing.Size(24, 80);
            this.sldMainVolume.SmallChange = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.sldMainVolume.TabIndex = 12;
            this.sldMainVolume.TabStop = false;
            this.sldMainVolume.Text = "colorSlider1";
            this.sldMainVolume.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.sldMainVolume.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sldMainVolume.ThumbSize = new System.Drawing.Size(16, 16);
            this.sldMainVolume.TickAdd = 0F;
            this.sldMainVolume.TickColor = System.Drawing.Color.White;
            this.sldMainVolume.TickDivide = 0F;
            this.sldMainVolume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sldMainVolume.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.sldMainVolume.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sldMainVolume_Scroll);
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.lblMainVolume);
            this.pnlBottom.Controls.Add(this.btRecord);
            this.pnlBottom.Controls.Add(this.nudKey);
            this.pnlBottom.Controls.Add(this.label2);
            this.pnlBottom.Controls.Add(this.lnlVol);
            this.pnlBottom.Controls.Add(this.pnlDisplay);
            this.pnlBottom.Controls.Add(this.btnPrev);
            this.pnlBottom.Controls.Add(this.btnNext);
            this.pnlBottom.Controls.Add(this.positionHScrollBar);
            this.pnlBottom.Controls.Add(this.btnStop);
            this.pnlBottom.Controls.Add(this.VuMasterPeakVolume);
            this.pnlBottom.Controls.Add(this.btnPlay);
            this.pnlBottom.Controls.Add(this.sldMainVolume);
            this.pnlBottom.Location = new System.Drawing.Point(9, 147);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(454, 130);
            this.pnlBottom.TabIndex = 45;
            // 
            // lnlVol
            // 
            this.lnlVol.Font = new System.Drawing.Font("Consolas", 8F);
            this.lnlVol.ForeColor = System.Drawing.Color.White;
            this.lnlVol.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lnlVol.Location = new System.Drawing.Point(237, 8);
            this.lnlVol.Name = "lnlVol";
            this.lnlVol.Size = new System.Drawing.Size(50, 13);
            this.lnlVol.TabIndex = 45;
            this.lnlVol.Text = "Volume";
            this.lnlVol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMainVolume
            // 
            this.lblMainVolume.Font = new System.Drawing.Font("Consolas", 8F);
            this.lblMainVolume.ForeColor = System.Drawing.Color.White;
            this.lblMainVolume.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblMainVolume.Location = new System.Drawing.Point(237, 110);
            this.lblMainVolume.Name = "lblMainVolume";
            this.lblMainVolume.Size = new System.Drawing.Size(50, 13);
            this.lblMainVolume.TabIndex = 46;
            this.lblMainVolume.Text = "60%";
            this.lblMainVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.Location = new System.Drawing.Point(8, 7);
            this.pnlDisplay.Name = "pnlDisplay";
            this.pnlDisplay.Size = new System.Drawing.Size(200, 45);
            this.pnlDisplay.TabIndex = 9;
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPrev.FlatAppearance.BorderSize = 0;
            this.btnPrev.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPrev.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrev.Image = global::Karaboss.Properties.Resources.btn_black_prev;
            this.btnPrev.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPrev.Location = new System.Drawing.Point(8, 71);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(50, 50);
            this.btnPrev.TabIndex = 44;
            this.btnPrev.TabStop = false;
            this.btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Image = global::Karaboss.Properties.Resources.btn_black_next;
            this.btnNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnNext.Location = new System.Drawing.Point(158, 71);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(50, 50);
            this.btnNext.TabIndex = 43;
            this.btnNext.TabStop = false;
            this.btnNext.UseVisualStyleBackColor = false;
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.Image = global::Karaboss.Properties.Resources.btn_black_stop;
            this.btnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnStop.Location = new System.Drawing.Point(108, 71);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(50, 50);
            this.btnStop.TabIndex = 42;
            this.btnStop.TabStop = false;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Image = global::Karaboss.Properties.Resources.btn_black_play;
            this.btnPlay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPlay.Location = new System.Drawing.Point(58, 71);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(50, 50);
            this.btnPlay.TabIndex = 41;
            this.btnPlay.TabStop = false;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // frmCDGPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.ClientSize = new System.Drawing.Size(843, 286);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.btBrowse);
            this.Controls.Add(this.pnlTop);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmCDGPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCDGPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCDGPlayer_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmCDGPlayer_FormClosed);
            this.Load += new System.EventHandler(this.frmCDGPlayer_Load);
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudKey)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trbVolume)).EndInit();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRecord;
        private System.Windows.Forms.TextBox tbFileName;
        private System.Windows.Forms.Button btBrowse;
        internal System.Windows.Forms.ToolStrip ToolStrip1;
        internal System.Windows.Forms.ToolStripButton tsbPlay;
        internal System.Windows.Forms.ToolStripButton tsbStop;
        internal System.Windows.Forms.ToolStripButton tsbPause;
        private System.Windows.Forms.NumericUpDown nudKey;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trbVolume;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlTop;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Windows.Forms.Timer Timer1;
        private Display.PanelPlayer pnlDisplay;
        private ColorSlider.ColorSlider positionHScrollBar;
        private VU_MeterLibrary.VuMeter VuMasterPeakVolume;
        private ColorSlider.ColorSlider sldMainVolume;
        private NoSelectButton btnPrev;
        private NoSelectButton btnNext;
        private NoSelectButton btnStop;
        private NoSelectButton btnPlay;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lnlVol;
        private System.Windows.Forms.Label lblMainVolume;
    }
}