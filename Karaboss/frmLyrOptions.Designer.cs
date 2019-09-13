namespace Karaboss
{
    partial class frmLyrOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLyrOptions));
            this.btnBackColor = new System.Windows.Forms.Button();
            this.btnSungColor = new System.Windows.Forms.Button();
            this.btnSingColor = new System.Windows.Forms.Button();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.txtSlideShow = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDirSlideShow = new System.Windows.Forms.Button();
            this.btnContourColor = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSlideShowFreq = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbSizeMode = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnResetDir = new System.Windows.Forms.Button();
            this.radioDiaporama = new System.Windows.Forms.RadioButton();
            this.radioSolidColor = new System.Windows.Forms.RadioButton();
            this.radioTransparent = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.UpDownNbLines = new System.Windows.Forms.NumericUpDown();
            this.chkDisplayBalls = new System.Windows.Forms.CheckBox();
            this.pnlBalls = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictBackColor = new System.Windows.Forms.PictureBox();
            this.pictContour = new System.Windows.Forms.PictureBox();
            this.pictNext = new System.Windows.Forms.PictureBox();
            this.pictHighlight = new System.Windows.Forms.PictureBox();
            this.pictBefore = new System.Windows.Forms.PictureBox();
            this.cbOptionsTextDisplay = new System.Windows.Forms.ComboBox();
            this.chkTextBackground = new System.Windows.Forms.CheckBox();
            this.chkContour = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pBox = new PicControl.pictureBoxControl();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.btnFonts = new System.Windows.Forms.Button();
            this.txtFont = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownNbLines)).BeginInit();
            this.pnlBalls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBackColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictContour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictHighlight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBefore)).BeginInit();
            this.SuspendLayout();
            // 
            // btnBackColor
            // 
            resources.ApplyResources(this.btnBackColor, "btnBackColor");
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.UseVisualStyleBackColor = true;
            this.btnBackColor.Click += new System.EventHandler(this.BtnBackColor_Click);
            // 
            // btnSungColor
            // 
            resources.ApplyResources(this.btnSungColor, "btnSungColor");
            this.btnSungColor.Name = "btnSungColor";
            this.btnSungColor.UseVisualStyleBackColor = true;
            this.btnSungColor.Click += new System.EventHandler(this.BtnSungColor_Click);
            // 
            // btnSingColor
            // 
            resources.ApplyResources(this.btnSingColor, "btnSingColor");
            this.btnSingColor.Name = "btnSingColor";
            this.btnSingColor.UseVisualStyleBackColor = true;
            this.btnSingColor.Click += new System.EventHandler(this.BtnSingColor_Click);
            // 
            // btnForeColor
            // 
            resources.ApplyResources(this.btnForeColor, "btnForeColor");
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.UseVisualStyleBackColor = true;
            this.btnForeColor.Click += new System.EventHandler(this.BtnForeColor_Click);
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtSlideShow
            // 
            resources.ApplyResources(this.txtSlideShow, "txtSlideShow");
            this.txtSlideShow.Name = "txtSlideShow";
            this.txtSlideShow.TextChanged += new System.EventHandler(this.TxtSlideShow_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnDirSlideShow
            // 
            resources.ApplyResources(this.btnDirSlideShow, "btnDirSlideShow");
            this.btnDirSlideShow.Name = "btnDirSlideShow";
            this.btnDirSlideShow.UseVisualStyleBackColor = true;
            this.btnDirSlideShow.Click += new System.EventHandler(this.BtnDirSlideShow_Click);
            // 
            // btnContourColor
            // 
            resources.ApplyResources(this.btnContourColor, "btnContourColor");
            this.btnContourColor.Name = "btnContourColor";
            this.btnContourColor.UseVisualStyleBackColor = true;
            this.btnContourColor.Click += new System.EventHandler(this.BtnContourColor_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtSlideShowFreq
            // 
            resources.ApplyResources(this.txtSlideShowFreq, "txtSlideShowFreq");
            this.txtSlideShowFreq.Name = "txtSlideShowFreq";
            this.txtSlideShowFreq.TextChanged += new System.EventHandler(this.TxtSlideShowFreq_TextChanged);
            this.txtSlideShowFreq.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSlideShowFreq_KeyPress);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // cbSizeMode
            // 
            this.cbSizeMode.FormattingEnabled = true;
            this.cbSizeMode.Items.AddRange(new object[] {
            resources.GetString("cbSizeMode.Items"),
            resources.GetString("cbSizeMode.Items1"),
            resources.GetString("cbSizeMode.Items2"),
            resources.GetString("cbSizeMode.Items3"),
            resources.GetString("cbSizeMode.Items4")});
            resources.ApplyResources(this.cbSizeMode, "cbSizeMode");
            this.cbSizeMode.Name = "cbSizeMode";
            this.cbSizeMode.SelectedIndexChanged += new System.EventHandler(this.CbSizeMode_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // btnResetDir
            // 
            resources.ApplyResources(this.btnResetDir, "btnResetDir");
            this.btnResetDir.Name = "btnResetDir";
            this.btnResetDir.UseVisualStyleBackColor = true;
            this.btnResetDir.Click += new System.EventHandler(this.BtnResetDir_Click);
            // 
            // radioDiaporama
            // 
            resources.ApplyResources(this.radioDiaporama, "radioDiaporama");
            this.radioDiaporama.Checked = true;
            this.radioDiaporama.Name = "radioDiaporama";
            this.radioDiaporama.TabStop = true;
            this.radioDiaporama.UseVisualStyleBackColor = true;
            this.radioDiaporama.CheckedChanged += new System.EventHandler(this.RadioDiaporama_CheckedChanged);
            // 
            // radioSolidColor
            // 
            resources.ApplyResources(this.radioSolidColor, "radioSolidColor");
            this.radioSolidColor.Name = "radioSolidColor";
            this.radioSolidColor.UseVisualStyleBackColor = true;
            this.radioSolidColor.CheckedChanged += new System.EventHandler(this.RadioSolidColor_CheckedChanged);
            // 
            // radioTransparent
            // 
            resources.ApplyResources(this.radioTransparent, "radioTransparent");
            this.radioTransparent.Name = "radioTransparent";
            this.radioTransparent.UseVisualStyleBackColor = true;
            this.radioTransparent.CheckedChanged += new System.EventHandler(this.RadioTransparent_CheckedChanged);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // UpDownNbLines
            // 
            resources.ApplyResources(this.UpDownNbLines, "UpDownNbLines");
            this.UpDownNbLines.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownNbLines.Name = "UpDownNbLines";
            this.UpDownNbLines.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.UpDownNbLines.ValueChanged += new System.EventHandler(this.UpDownNbLines_ValueChanged);
            // 
            // chkDisplayBalls
            // 
            resources.ApplyResources(this.chkDisplayBalls, "chkDisplayBalls");
            this.chkDisplayBalls.Name = "chkDisplayBalls";
            this.chkDisplayBalls.UseVisualStyleBackColor = true;
            this.chkDisplayBalls.CheckedChanged += new System.EventHandler(this.chkDisplayBalls_CheckedChanged);
            // 
            // pnlBalls
            // 
            this.pnlBalls.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlBalls.Controls.Add(this.pictureBox2);
            this.pnlBalls.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.pnlBalls, "pnlBalls");
            this.pnlBalls.Name = "pnlBalls";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::Karaboss.Properties.Resources.ball;
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Karaboss.Properties.Resources.ball;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // pictBackColor
            // 
            this.pictBackColor.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pictBackColor, "pictBackColor");
            this.pictBackColor.Name = "pictBackColor";
            this.pictBackColor.TabStop = false;
            // 
            // pictContour
            // 
            this.pictContour.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pictContour, "pictContour");
            this.pictContour.Name = "pictContour";
            this.pictContour.TabStop = false;
            // 
            // pictNext
            // 
            this.pictNext.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.pictNext, "pictNext");
            this.pictNext.Name = "pictNext";
            this.pictNext.TabStop = false;
            // 
            // pictHighlight
            // 
            this.pictHighlight.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.pictHighlight, "pictHighlight");
            this.pictHighlight.Name = "pictHighlight";
            this.pictHighlight.TabStop = false;
            // 
            // pictBefore
            // 
            this.pictBefore.BackColor = System.Drawing.Color.LightGreen;
            resources.ApplyResources(this.pictBefore, "pictBefore");
            this.pictBefore.Name = "pictBefore";
            this.pictBefore.TabStop = false;
            // 
            // cbOptionsTextDisplay
            // 
            this.cbOptionsTextDisplay.FormattingEnabled = true;
            this.cbOptionsTextDisplay.Items.AddRange(new object[] {
            resources.GetString("cbOptionsTextDisplay.Items"),
            resources.GetString("cbOptionsTextDisplay.Items1"),
            resources.GetString("cbOptionsTextDisplay.Items2")});
            resources.ApplyResources(this.cbOptionsTextDisplay, "cbOptionsTextDisplay");
            this.cbOptionsTextDisplay.Name = "cbOptionsTextDisplay";
            this.cbOptionsTextDisplay.SelectedIndexChanged += new System.EventHandler(this.cbOptionsTextDisplay_SelectedIndexChanged);
            // 
            // chkTextBackground
            // 
            resources.ApplyResources(this.chkTextBackground, "chkTextBackground");
            this.chkTextBackground.Checked = true;
            this.chkTextBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextBackground.Name = "chkTextBackground";
            this.chkTextBackground.UseVisualStyleBackColor = true;
            this.chkTextBackground.CheckedChanged += new System.EventHandler(this.chkTextBackground_CheckedChanged);
            // 
            // chkContour
            // 
            resources.ApplyResources(this.chkContour, "chkContour");
            this.chkContour.Checked = true;
            this.chkContour.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkContour.Name = "chkContour";
            this.chkContour.UseVisualStyleBackColor = true;
            this.chkContour.CheckedChanged += new System.EventHandler(this.chkContour_CheckedChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // pBox
            // 
            this.pBox.BackColor = System.Drawing.Color.Black;
            this.pBox.bColorContour = true;
            this.pBox.BeatDuration = 0;
            this.pBox.bTextBackGround = true;
            this.pBox.CurrentTextPos = 2;
            this.pBox.CurrentTime = 30;
            this.pBox.DirSlideShow = null;
            this.pBox.FreqDirSlideShow = 0;
            this.pBox.imgLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pBox.KaraokeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            resources.ApplyResources(this.pBox, "pBox");
            this.pBox.LyricsTimes = ((System.Collections.Generic.List<int>)(resources.GetObject("pBox.LyricsTimes")));
            this.pBox.LyricsWords = ((System.Collections.Generic.List<string>)(resources.GetObject("pBox.LyricsWords")));
            this.pBox.m_Alpha = 255;
            this.pBox.m_CurrentImage = null;
            this.pBox.m_DisplayRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.pBox.Name = "pBox";
            this.pBox.OptionBackground = null;
            this.pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Bottom;
            this.pBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pBox.TransparencyKey = System.Drawing.Color.Lime;
            this.pBox.Txt = "Lorem ipsum dolor sit amet,\rconsectetur adipisicing elit,\rsed do eiusmod tempor i" +
    "ncididunt\rut labore et dolore magna aliqua.\rUt enim ad minim veniam,";
            this.pBox.TxtBackColor = System.Drawing.Color.Black;
            this.pBox.TxtBeforeColor = System.Drawing.Color.Red;
            this.pBox.TxtContourColor = System.Drawing.Color.White;
            this.pBox.TxtHighlightColor = System.Drawing.Color.Coral;
            this.pBox.TxtNbLines = 3;
            this.pBox.TxtNextColor = System.Drawing.Color.YellowGreen;
            // 
            // btnFonts
            // 
            resources.ApplyResources(this.btnFonts, "btnFonts");
            this.btnFonts.Name = "btnFonts";
            this.btnFonts.UseVisualStyleBackColor = true;
            this.btnFonts.Click += new System.EventHandler(this.BtnFonts_Click);
            // 
            // txtFont
            // 
            resources.ApplyResources(this.txtFont, "txtFont");
            this.txtFont.Name = "txtFont";
            // 
            // frmLyrOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtFont);
            this.Controls.Add(this.btnFonts);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkContour);
            this.Controls.Add(this.chkTextBackground);
            this.Controls.Add(this.cbOptionsTextDisplay);
            this.Controls.Add(this.pnlBalls);
            this.Controls.Add(this.chkDisplayBalls);
            this.Controls.Add(this.UpDownNbLines);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictBackColor);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.radioTransparent);
            this.Controls.Add(this.radioSolidColor);
            this.Controls.Add(this.radioDiaporama);
            this.Controls.Add(this.pictContour);
            this.Controls.Add(this.pictNext);
            this.Controls.Add(this.pictHighlight);
            this.Controls.Add(this.pictBefore);
            this.Controls.Add(this.btnResetDir);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbSizeMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSlideShowFreq);
            this.Controls.Add(this.pBox);
            this.Controls.Add(this.btnContourColor);
            this.Controls.Add(this.btnDirSlideShow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSlideShow);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnForeColor);
            this.Controls.Add(this.btnSingColor);
            this.Controls.Add(this.btnSungColor);
            this.Controls.Add(this.btnBackColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLyrOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLyrOptions_FormClosing);
            this.Load += new System.EventHandler(this.FrmLyrOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.UpDownNbLines)).EndInit();
            this.pnlBalls.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBackColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictContour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictHighlight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBefore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnBackColor;
        private System.Windows.Forms.Button btnSungColor;
        private System.Windows.Forms.Button btnSingColor;
        private System.Windows.Forms.Button btnForeColor;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox txtSlideShow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDirSlideShow;
        private System.Windows.Forms.Button btnContourColor;
        private PicControl.pictureBoxControl pBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSlideShowFreq;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbSizeMode;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnResetDir;
        private System.Windows.Forms.PictureBox pictBefore;
        private System.Windows.Forms.PictureBox pictHighlight;
        private System.Windows.Forms.PictureBox pictNext;
        private System.Windows.Forms.PictureBox pictContour;
        private System.Windows.Forms.RadioButton radioDiaporama;
        private System.Windows.Forms.RadioButton radioSolidColor;
        private System.Windows.Forms.RadioButton radioTransparent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictBackColor;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown UpDownNbLines;
        private System.Windows.Forms.CheckBox chkDisplayBalls;
        private System.Windows.Forms.Panel pnlBalls;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.ComboBox cbOptionsTextDisplay;
        private System.Windows.Forms.CheckBox chkTextBackground;
        private System.Windows.Forms.CheckBox chkContour;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Button btnFonts;
        private System.Windows.Forms.TextBox txtFont;
    }
}