namespace Karaboss
{
    partial class frmMp3Player
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMp3Player));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.btnTempoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTempoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.btnTranspoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTranspoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.lblTranspoValue = new System.Windows.Forms.Label();
            this.lblTransp = new System.Windows.Forms.Label();
            this.VuPeakVolumeRight = new VU_MeterLibrary.VuMeter();
            this.VuPeakVolumeLeft = new VU_MeterLibrary.VuMeter();
            this.lblMainVolume = new System.Windows.Forms.Label();
            this.pnlDisplay = new Karaboss.Display.PanelPlayer();
            this.btnPrev = new Karaboss.NoSelectButton();
            this.btnNext = new Karaboss.NoSelectButton();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.btnStop = new Karaboss.NoSelectButton();
            this.btnPlay = new Karaboss.NoSelectButton();
            this.sldMainVolume = new ColorSlider.ColorSlider();
            this.lnlVol = new System.Windows.Forms.Label();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen,
            this.MnuFileSep,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            resources.ApplyResources(this.mnuFileOpen, "mnuFileOpen");
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // MnuFileSep
            // 
            this.MnuFileSep.Name = "MnuFileSep";
            resources.ApplyResources(this.MnuFileSep, "MnuFileSep");
            // 
            // mnuFileQuit
            // 
            this.mnuFileQuit.Name = "mnuFileQuit";
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Click += new System.EventHandler(this.mnuFileQuit_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.btnTempoMinus);
            this.pnlControls.Controls.Add(this.btnTempoPlus);
            this.pnlControls.Controls.Add(this.lblTempoValue);
            this.pnlControls.Controls.Add(this.lblTemp);
            this.pnlControls.Controls.Add(this.btnTranspoMinus);
            this.pnlControls.Controls.Add(this.btnTranspoPlus);
            this.pnlControls.Controls.Add(this.lblTranspoValue);
            this.pnlControls.Controls.Add(this.lblTransp);
            this.pnlControls.Controls.Add(this.VuPeakVolumeRight);
            this.pnlControls.Controls.Add(this.VuPeakVolumeLeft);
            this.pnlControls.Controls.Add(this.lblMainVolume);
            this.pnlControls.Controls.Add(this.pnlDisplay);
            this.pnlControls.Controls.Add(this.btnPrev);
            this.pnlControls.Controls.Add(this.btnNext);
            this.pnlControls.Controls.Add(this.positionHScrollBar);
            this.pnlControls.Controls.Add(this.btnStop);
            this.pnlControls.Controls.Add(this.btnPlay);
            this.pnlControls.Controls.Add(this.sldMainVolume);
            this.pnlControls.Controls.Add(this.lnlVol);
            resources.ApplyResources(this.pnlControls, "pnlControls");
            this.pnlControls.Name = "pnlControls";
            // 
            // btnTempoMinus
            // 
            resources.ApplyResources(this.btnTempoMinus, "btnTempoMinus");
            this.btnTempoMinus.Name = "btnTempoMinus";
            this.btnTempoMinus.Click += new System.EventHandler(this.btnTempoMinus_Click);
            // 
            // btnTempoPlus
            // 
            resources.ApplyResources(this.btnTempoPlus, "btnTempoPlus");
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // lblTempoValue
            // 
            this.lblTempoValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblTempoValue, "lblTempoValue");
            this.lblTempoValue.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblTempoValue.Name = "lblTempoValue";
            // 
            // lblTemp
            // 
            resources.ApplyResources(this.lblTemp, "lblTemp");
            this.lblTemp.ForeColor = System.Drawing.Color.White;
            this.lblTemp.Name = "lblTemp";
            // 
            // btnTranspoMinus
            // 
            resources.ApplyResources(this.btnTranspoMinus, "btnTranspoMinus");
            this.btnTranspoMinus.Name = "btnTranspoMinus";
            this.btnTranspoMinus.Click += new System.EventHandler(this.btnTranspoMinus_Click);
            // 
            // btnTranspoPlus
            // 
            resources.ApplyResources(this.btnTranspoPlus, "btnTranspoPlus");
            this.btnTranspoPlus.Name = "btnTranspoPlus";
            this.btnTranspoPlus.Click += new System.EventHandler(this.btnTranspoPlus_Click);
            // 
            // lblTranspoValue
            // 
            this.lblTranspoValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblTranspoValue, "lblTranspoValue");
            this.lblTranspoValue.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblTranspoValue.Name = "lblTranspoValue";
            // 
            // lblTransp
            // 
            resources.ApplyResources(this.lblTransp, "lblTransp");
            this.lblTransp.ForeColor = System.Drawing.Color.White;
            this.lblTransp.Name = "lblTransp";
            // 
            // VuPeakVolumeRight
            // 
            this.VuPeakVolumeRight.AnalogMeter = false;
            this.VuPeakVolumeRight.BackColor = System.Drawing.Color.DimGray;
            this.VuPeakVolumeRight.DialBackground = System.Drawing.Color.White;
            this.VuPeakVolumeRight.DialTextNegative = System.Drawing.Color.Red;
            this.VuPeakVolumeRight.DialTextPositive = System.Drawing.Color.Black;
            this.VuPeakVolumeRight.DialTextZero = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeRight.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeRight.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.VuPeakVolumeRight.Led1Count = 14;
            this.VuPeakVolumeRight.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuPeakVolumeRight.Led2ColorOn = System.Drawing.Color.Yellow;
            this.VuPeakVolumeRight.Led2Count = 14;
            this.VuPeakVolumeRight.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuPeakVolumeRight.Led3ColorOn = System.Drawing.Color.Red;
            this.VuPeakVolumeRight.Led3Count = 6;
            this.VuPeakVolumeRight.LedSize = new System.Drawing.Size(12, 2);
            this.VuPeakVolumeRight.LedSpace = 1;
            this.VuPeakVolumeRight.Level = 0;
            this.VuPeakVolumeRight.LevelMax = 127;
            resources.ApplyResources(this.VuPeakVolumeRight, "VuPeakVolumeRight");
            this.VuPeakVolumeRight.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuPeakVolumeRight.Name = "VuPeakVolumeRight";
            this.VuPeakVolumeRight.NeedleColor = System.Drawing.Color.Black;
            this.VuPeakVolumeRight.PeakHold = true;
            this.VuPeakVolumeRight.Peakms = 1000;
            this.VuPeakVolumeRight.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuPeakVolumeRight.ShowDialOnly = false;
            this.VuPeakVolumeRight.ShowLedPeak = false;
            this.VuPeakVolumeRight.ShowTextInDial = false;
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
            // 
            // VuPeakVolumeLeft
            // 
            this.VuPeakVolumeLeft.AnalogMeter = false;
            this.VuPeakVolumeLeft.BackColor = System.Drawing.Color.DimGray;
            this.VuPeakVolumeLeft.DialBackground = System.Drawing.Color.White;
            this.VuPeakVolumeLeft.DialTextNegative = System.Drawing.Color.Red;
            this.VuPeakVolumeLeft.DialTextPositive = System.Drawing.Color.Black;
            this.VuPeakVolumeLeft.DialTextZero = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeLeft.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuPeakVolumeLeft.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.VuPeakVolumeLeft.Led1Count = 14;
            this.VuPeakVolumeLeft.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuPeakVolumeLeft.Led2ColorOn = System.Drawing.Color.Yellow;
            this.VuPeakVolumeLeft.Led2Count = 14;
            this.VuPeakVolumeLeft.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuPeakVolumeLeft.Led3ColorOn = System.Drawing.Color.Red;
            this.VuPeakVolumeLeft.Led3Count = 6;
            this.VuPeakVolumeLeft.LedSize = new System.Drawing.Size(12, 2);
            this.VuPeakVolumeLeft.LedSpace = 1;
            this.VuPeakVolumeLeft.Level = 0;
            this.VuPeakVolumeLeft.LevelMax = 127;
            resources.ApplyResources(this.VuPeakVolumeLeft, "VuPeakVolumeLeft");
            this.VuPeakVolumeLeft.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuPeakVolumeLeft.Name = "VuPeakVolumeLeft";
            this.VuPeakVolumeLeft.NeedleColor = System.Drawing.Color.Black;
            this.VuPeakVolumeLeft.PeakHold = true;
            this.VuPeakVolumeLeft.Peakms = 1000;
            this.VuPeakVolumeLeft.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuPeakVolumeLeft.ShowDialOnly = false;
            this.VuPeakVolumeLeft.ShowLedPeak = false;
            this.VuPeakVolumeLeft.ShowTextInDial = false;
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
            // 
            // lblMainVolume
            // 
            resources.ApplyResources(this.lblMainVolume, "lblMainVolume");
            this.lblMainVolume.ForeColor = System.Drawing.Color.White;
            this.lblMainVolume.Name = "lblMainVolume";
            // 
            // pnlDisplay
            // 
            resources.ApplyResources(this.pnlDisplay, "pnlDisplay");
            this.pnlDisplay.Name = "pnlDisplay";
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPrev.FlatAppearance.BorderSize = 0;
            this.btnPrev.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPrev.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnPrev, "btnPrev");
            this.btnPrev.Image = global::Karaboss.Properties.Resources.btn_black_prev;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.TabStop = false;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.btnPrev.MouseLeave += new System.EventHandler(this.btnPrev_MouseLeave);
            this.btnPrev.MouseHover += new System.EventHandler(this.btnPrev_MouseHover);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Image = global::Karaboss.Properties.Resources.btn_black_next;
            this.btnNext.Name = "btnNext";
            this.btnNext.TabStop = false;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.MouseLeave += new System.EventHandler(this.btnNext_MouseLeave);
            this.btnNext.MouseHover += new System.EventHandler(this.btnNext_MouseHover);
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
            5,
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
            this.positionHScrollBar.SmallChange = new decimal(new int[] {
            1,
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
            this.positionHScrollBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.positionHScrollBar.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Image = global::Karaboss.Properties.Resources.btn_black_stop;
            this.btnStop.Name = "btnStop";
            this.btnStop.TabStop = false;
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.MouseLeave += new System.EventHandler(this.btnStop_MouseLeave);
            this.btnStop.MouseHover += new System.EventHandler(this.btnStop_MouseHover);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Image = global::Karaboss.Properties.Resources.btn_black_play;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.TabStop = false;
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlay.MouseLeave += new System.EventHandler(this.btnPlay_MouseLeave);
            this.btnPlay.MouseHover += new System.EventHandler(this.btnPlay_MouseHover);
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
            resources.ApplyResources(this.sldMainVolume, "sldMainVolume");
            this.sldMainVolume.ForeColor = System.Drawing.Color.White;
            this.sldMainVolume.LargeChange = new decimal(new int[] {
            26,
            0,
            0,
            0});
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
            this.sldMainVolume.SmallChange = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.sldMainVolume.TabStop = false;
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
            this.sldMainVolume.ValueChanged += new System.EventHandler(this.sldMainVolume_ValueChanged);
            this.sldMainVolume.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sldMainVolume_Scroll);
            // 
            // lnlVol
            // 
            resources.ApplyResources(this.lnlVol, "lnlVol");
            this.lnlVol.ForeColor = System.Drawing.Color.White;
            this.lnlVol.Name = "lnlVol";
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
            // frmMp3Player
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.menuStrip1);
            this.MaximizeBox = false;
            this.Name = "frmMp3Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Player_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMp3Player_FormClosed);
            this.Load += new System.EventHandler(this.frmMp3Player_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator MnuFileSep;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.Panel pnlControls;
        private Buttons.MinusButtonControl btnTempoMinus;
        private Buttons.PlusButtonControl btnTempoPlus;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Label lblTemp;
        private Buttons.MinusButtonControl btnTranspoMinus;
        private Buttons.PlusButtonControl btnTranspoPlus;
        private System.Windows.Forms.Label lblTranspoValue;
        private System.Windows.Forms.Label lblTransp;
        private VU_MeterLibrary.VuMeter VuPeakVolumeRight;
        private VU_MeterLibrary.VuMeter VuPeakVolumeLeft;
        private System.Windows.Forms.Label lblMainVolume;
        private Display.PanelPlayer pnlDisplay;
        private NoSelectButton btnPrev;
        private NoSelectButton btnNext;
        private ColorSlider.ColorSlider positionHScrollBar;
        private NoSelectButton btnStop;
        private NoSelectButton btnPlay;
        private ColorSlider.ColorSlider sldMainVolume;
        private System.Windows.Forms.Label lnlVol;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Windows.Forms.Timer Timer1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}