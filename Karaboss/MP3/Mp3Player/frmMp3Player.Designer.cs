namespace Karaboss.Mp3
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
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLRCGenerator = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.pBox = new System.Windows.Forms.PictureBox();
            this.lblPlaylist = new System.Windows.Forms.Label();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.lblTranspoValue = new System.Windows.Forms.Label();
            this.lblTransp = new System.Windows.Forms.Label();
            this.VuPeakVolumeRight = new VU_MeterLibrary.VuMeter();
            this.VuPeakVolumeLeft = new VU_MeterLibrary.VuMeter();
            this.lblMainVolume = new System.Windows.Forms.Label();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.sldMainVolume = new ColorSlider.ColorSlider();
            this.lnlVol = new System.Windows.Forms.Label();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.txtAlbum = new System.Windows.Forms.TextBox();
            this.txtEditing = new System.Windows.Forms.TextBox();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.txtYourName = new System.Windows.Forms.TextBox();
            this.lblHotkeys = new System.Windows.Forms.Label();
            this.lblLyrics = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.lblMode = new System.Windows.Forms.Label();
            this.toolstrip1 = new System.Windows.Forms.ToolStrip();
            this.mnuImport = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuImportLrcFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImportRawLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuExportLRCMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExportLrcNoMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSwitchSyncEdit = new System.Windows.Forms.ToolStripButton();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.lvLyrics = new ListViewEx.ListViewEx();
            this.btnTempoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTempoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.btnTranspoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTranspoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.pnlDisplay = new Karaboss.Display.PanelPlayer();
            this.btnPrev = new Karaboss.NoSelectButton();
            this.btnNext = new Karaboss.NoSelectButton();
            this.btnStop = new Karaboss.NoSelectButton();
            this.btnPlay = new Karaboss.NoSelectButton();
            this.menuStrip1.SuspendLayout();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).BeginInit();
            this.pnlMiddle.SuspendLayout();
            this.toolstrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
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
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditLyrics,
            this.mnuEditLRCGenerator});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditLyrics
            // 
            this.mnuEditLyrics.Name = "mnuEditLyrics";
            resources.ApplyResources(this.mnuEditLyrics, "mnuEditLyrics");
            this.mnuEditLyrics.Click += new System.EventHandler(this.mnuEditLyrics_Click);
            // 
            // mnuEditLRCGenerator
            // 
            this.mnuEditLRCGenerator.Name = "mnuEditLRCGenerator";
            resources.ApplyResources(this.mnuEditLRCGenerator, "mnuEditLRCGenerator");
            this.mnuEditLRCGenerator.Click += new System.EventHandler(this.mnuEditLRCGenerator_Click);
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
            this.pnlControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlControls.Controls.Add(this.pBox);
            this.pnlControls.Controls.Add(this.lblPlaylist);
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
            // pBox
            // 
            resources.ApplyResources(this.pBox, "pBox");
            this.pBox.Name = "pBox";
            this.pBox.TabStop = false;
            // 
            // lblPlaylist
            // 
            resources.ApplyResources(this.lblPlaylist, "lblPlaylist");
            this.lblPlaylist.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblPlaylist.Name = "lblPlaylist";
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
            // Timer1
            // 
            this.Timer1.Interval = 50;
            this.Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // txtTitle
            // 
            resources.ApplyResources(this.txtTitle, "txtTitle");
            this.txtTitle.Name = "txtTitle";
            this.toolTip1.SetToolTip(this.txtTitle, resources.GetString("txtTitle.ToolTip"));
            // 
            // txtAuthor
            // 
            resources.ApplyResources(this.txtAuthor, "txtAuthor");
            this.txtAuthor.Name = "txtAuthor";
            this.toolTip1.SetToolTip(this.txtAuthor, resources.GetString("txtAuthor.ToolTip"));
            // 
            // txtArtist
            // 
            resources.ApplyResources(this.txtArtist, "txtArtist");
            this.txtArtist.Name = "txtArtist";
            this.toolTip1.SetToolTip(this.txtArtist, resources.GetString("txtArtist.ToolTip"));
            // 
            // txtAlbum
            // 
            resources.ApplyResources(this.txtAlbum, "txtAlbum");
            this.txtAlbum.Name = "txtAlbum";
            this.toolTip1.SetToolTip(this.txtAlbum, resources.GetString("txtAlbum.ToolTip"));
            // 
            // txtEditing
            // 
            resources.ApplyResources(this.txtEditing, "txtEditing");
            this.txtEditing.Name = "txtEditing";
            this.toolTip1.SetToolTip(this.txtEditing, resources.GetString("txtEditing.ToolTip"));
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.LightGray;
            this.pnlMiddle.Controls.Add(this.txtEditing);
            this.pnlMiddle.Controls.Add(this.label6);
            this.pnlMiddle.Controls.Add(this.label5);
            this.pnlMiddle.Controls.Add(this.label4);
            this.pnlMiddle.Controls.Add(this.label3);
            this.pnlMiddle.Controls.Add(this.label1);
            this.pnlMiddle.Controls.Add(this.cbLanguage);
            this.pnlMiddle.Controls.Add(this.txtAlbum);
            this.pnlMiddle.Controls.Add(this.txtYourName);
            this.pnlMiddle.Controls.Add(this.txtArtist);
            this.pnlMiddle.Controls.Add(this.txtAuthor);
            this.pnlMiddle.Controls.Add(this.txtTitle);
            this.pnlMiddle.Controls.Add(this.lblHotkeys);
            this.pnlMiddle.Controls.Add(this.lvLyrics);
            this.pnlMiddle.Controls.Add(this.lblLyrics);
            this.pnlMiddle.Controls.Add(this.lblTimes);
            this.pnlMiddle.Controls.Add(this.lblMode);
            this.pnlMiddle.Controls.Add(this.toolstrip1);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbLanguage
            // 
            this.cbLanguage.FormattingEnabled = true;
            resources.ApplyResources(this.cbLanguage, "cbLanguage");
            this.cbLanguage.Name = "cbLanguage";
            // 
            // txtYourName
            // 
            resources.ApplyResources(this.txtYourName, "txtYourName");
            this.txtYourName.Name = "txtYourName";
            // 
            // lblHotkeys
            // 
            this.lblHotkeys.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.lblHotkeys, "lblHotkeys");
            this.lblHotkeys.Name = "lblHotkeys";
            // 
            // lblLyrics
            // 
            resources.ApplyResources(this.lblLyrics, "lblLyrics");
            this.lblLyrics.Name = "lblLyrics";
            // 
            // lblTimes
            // 
            resources.ApplyResources(this.lblTimes, "lblTimes");
            this.lblTimes.Name = "lblTimes";
            // 
            // lblMode
            // 
            resources.ApplyResources(this.lblMode, "lblMode");
            this.lblMode.Name = "lblMode";
            // 
            // toolstrip1
            // 
            resources.ApplyResources(this.toolstrip1, "toolstrip1");
            this.toolstrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuImport,
            this.mnuExport,
            this.btnSwitchSyncEdit});
            this.toolstrip1.Name = "toolstrip1";
            this.toolstrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // mnuImport
            // 
            this.mnuImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuImportLrcFile,
            this.mnuImportRawLyrics});
            resources.ApplyResources(this.mnuImport, "mnuImport");
            this.mnuImport.Name = "mnuImport";
            // 
            // mnuImportLrcFile
            // 
            this.mnuImportLrcFile.Name = "mnuImportLrcFile";
            resources.ApplyResources(this.mnuImportLrcFile, "mnuImportLrcFile");
            this.mnuImportLrcFile.Click += new System.EventHandler(this.mnuImportLrcFile_Click);
            // 
            // mnuImportRawLyrics
            // 
            this.mnuImportRawLyrics.Name = "mnuImportRawLyrics";
            resources.ApplyResources(this.mnuImportRawLyrics, "mnuImportRawLyrics");
            this.mnuImportRawLyrics.Click += new System.EventHandler(this.mnuImportRawLyrics_Click);
            // 
            // mnuExport
            // 
            this.mnuExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExportLRCMeta,
            this.mnuExportLrcNoMeta});
            resources.ApplyResources(this.mnuExport, "mnuExport");
            this.mnuExport.Name = "mnuExport";
            // 
            // mnuExportLRCMeta
            // 
            this.mnuExportLRCMeta.Name = "mnuExportLRCMeta";
            resources.ApplyResources(this.mnuExportLRCMeta, "mnuExportLRCMeta");
            this.mnuExportLRCMeta.Click += new System.EventHandler(this.mnuExportLRCMeta_Click);
            // 
            // mnuExportLrcNoMeta
            // 
            this.mnuExportLrcNoMeta.Name = "mnuExportLrcNoMeta";
            resources.ApplyResources(this.mnuExportLrcNoMeta, "mnuExportLrcNoMeta");
            this.mnuExportLrcNoMeta.Click += new System.EventHandler(this.mnuExportLrcNoMeta_Click);
            // 
            // btnSwitchSyncEdit
            // 
            this.btnSwitchSyncEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.btnSwitchSyncEdit, "btnSwitchSyncEdit");
            this.btnSwitchSyncEdit.Name = "btnSwitchSyncEdit";
            this.btnSwitchSyncEdit.Click += new System.EventHandler(this.btnSwitchSyncEdit_Click);
            // 
            // lvLyrics
            // 
            this.lvLyrics.AllowColumnReorder = true;
            this.lvLyrics.DoubleClickActivation = true;
            this.lvLyrics.FullRowSelect = true;
            this.lvLyrics.HideSelection = false;
            resources.ApplyResources(this.lvLyrics, "lvLyrics");
            this.lvLyrics.Name = "lvLyrics";
            this.lvLyrics.UseCompatibleStateImageBehavior = false;
            this.lvLyrics.View = System.Windows.Forms.View.Details;
            this.lvLyrics.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lvLyrics_MouseMove);
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
            // frmMp3Player
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "frmMp3Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Player_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMp3Player_FormClosed);
            this.Load += new System.EventHandler(this.frmMp3Player_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMp3Player_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMp3Player_KeyUp);
            this.Resize += new System.EventHandler(this.frmMp3Player_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pBox)).EndInit();
            this.pnlMiddle.ResumeLayout(false);
            this.pnlMiddle.PerformLayout();
            this.toolstrip1.ResumeLayout(false);
            this.toolstrip1.PerformLayout();
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
        private System.Windows.Forms.Label lblPlaylist;
        private System.Windows.Forms.PictureBox pBox;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLyrics;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLRCGenerator;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.ToolStrip toolstrip1;
        private System.Windows.Forms.ToolStripDropDownButton mnuImport;
        private System.Windows.Forms.ToolStripMenuItem mnuImportLrcFile;
        private System.Windows.Forms.ToolStripMenuItem mnuImportRawLyrics;
        private System.Windows.Forms.ToolStripDropDownButton mnuExport;
        private System.Windows.Forms.ToolStripMenuItem mnuExportLRCMeta;
        private System.Windows.Forms.ToolStripMenuItem mnuExportLrcNoMeta;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label lblLyrics;
        private System.Windows.Forms.Label lblTimes;
        private ListViewEx.ListViewEx lvLyrics;
        private System.Windows.Forms.Label lblHotkeys;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtYourName;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.TextBox txtAlbum;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEditing;
        private System.Windows.Forms.ToolStripButton btnSwitchSyncEdit;
    }
}