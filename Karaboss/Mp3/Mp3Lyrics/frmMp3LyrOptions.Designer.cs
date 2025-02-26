namespace Karaboss.Mp3
{
    partial class frmMp3LyrOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMp3LyrOptions));
            this.label9 = new System.Windows.Forms.Label();
            this.pictBackColor = new System.Windows.Forms.PictureBox();
            this.radioTransparent = new System.Windows.Forms.RadioButton();
            this.radioSolidColor = new System.Windows.Forms.RadioButton();
            this.radioDiaporama = new System.Windows.Forms.RadioButton();
            this.btnBackColor = new System.Windows.Forms.Button();
            this.btnResetDir = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cbSizeMode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSlideShowFreq = new System.Windows.Forms.TextBox();
            this.btnDirSlideShow = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSlideShow = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.chkContour = new System.Windows.Forms.CheckBox();
            this.UpDownNbLines = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.pictContour = new System.Windows.Forms.PictureBox();
            this.pictNext = new System.Windows.Forms.PictureBox();
            this.pictHighlight = new System.Windows.Forms.PictureBox();
            this.pictBefore = new System.Windows.Forms.PictureBox();
            this.btnContourColor = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.btnSingColor = new System.Windows.Forms.Button();
            this.btnSungColor = new System.Windows.Forms.Button();
            this.chkTextUppercase = new System.Windows.Forms.CheckBox();
            this.txtFont = new System.Windows.Forms.TextBox();
            this.btnFonts = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.chkTextBackground = new System.Windows.Forms.CheckBox();
            this.cbOptionsTextDisplay = new System.Windows.Forms.ComboBox();
            this.karaokeEffect1 = new keffect.KaraokeEffect();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictBackColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownNbLines)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictContour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictHighlight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBefore)).BeginInit();
            this.SuspendLayout();
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // pictBackColor
            // 
            this.pictBackColor.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pictBackColor, "pictBackColor");
            this.pictBackColor.Name = "pictBackColor";
            this.pictBackColor.TabStop = false;
            // 
            // radioTransparent
            // 
            resources.ApplyResources(this.radioTransparent, "radioTransparent");
            this.radioTransparent.Name = "radioTransparent";
            this.radioTransparent.UseVisualStyleBackColor = true;
            // 
            // radioSolidColor
            // 
            resources.ApplyResources(this.radioSolidColor, "radioSolidColor");
            this.radioSolidColor.Name = "radioSolidColor";
            this.radioSolidColor.UseVisualStyleBackColor = true;
            // 
            // radioDiaporama
            // 
            resources.ApplyResources(this.radioDiaporama, "radioDiaporama");
            this.radioDiaporama.Checked = true;
            this.radioDiaporama.Name = "radioDiaporama";
            this.radioDiaporama.TabStop = true;
            this.radioDiaporama.UseVisualStyleBackColor = true;
            this.radioDiaporama.CheckedChanged += new System.EventHandler(this.radioDiaporama_CheckedChanged);
            // 
            // btnBackColor
            // 
            resources.ApplyResources(this.btnBackColor, "btnBackColor");
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.UseVisualStyleBackColor = true;
            this.btnBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // btnResetDir
            // 
            resources.ApplyResources(this.btnResetDir, "btnResetDir");
            this.btnResetDir.Name = "btnResetDir";
            this.btnResetDir.UseVisualStyleBackColor = true;
            this.btnResetDir.Click += new System.EventHandler(this.btnResetDir_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            this.cbSizeMode.SelectedIndexChanged += new System.EventHandler(this.cbSizeMode_SelectedIndexChanged);
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
            // txtSlideShowFreq
            // 
            resources.ApplyResources(this.txtSlideShowFreq, "txtSlideShowFreq");
            this.txtSlideShowFreq.Name = "txtSlideShowFreq";
            this.txtSlideShowFreq.TextChanged += new System.EventHandler(this.txtSlideShowFreq_TextChanged);
            // 
            // btnDirSlideShow
            // 
            resources.ApplyResources(this.btnDirSlideShow, "btnDirSlideShow");
            this.btnDirSlideShow.Name = "btnDirSlideShow";
            this.btnDirSlideShow.UseVisualStyleBackColor = true;
            this.btnDirSlideShow.Click += new System.EventHandler(this.btnDirSlideShow_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtSlideShow
            // 
            resources.ApplyResources(this.txtSlideShow, "txtSlideShow");
            this.txtSlideShow.Name = "txtSlideShow";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
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
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // pictContour
            // 
            this.pictContour.BackColor = System.Drawing.Color.Black;
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
            // btnContourColor
            // 
            resources.ApplyResources(this.btnContourColor, "btnContourColor");
            this.btnContourColor.Name = "btnContourColor";
            this.btnContourColor.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnForeColor
            // 
            resources.ApplyResources(this.btnForeColor, "btnForeColor");
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.UseVisualStyleBackColor = true;
            this.btnForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // btnSingColor
            // 
            resources.ApplyResources(this.btnSingColor, "btnSingColor");
            this.btnSingColor.Name = "btnSingColor";
            this.btnSingColor.UseVisualStyleBackColor = true;
            this.btnSingColor.Click += new System.EventHandler(this.btnSingColor_Click);
            // 
            // btnSungColor
            // 
            resources.ApplyResources(this.btnSungColor, "btnSungColor");
            this.btnSungColor.Name = "btnSungColor";
            this.btnSungColor.UseVisualStyleBackColor = true;
            this.btnSungColor.Click += new System.EventHandler(this.btnSungColor_Click);
            // 
            // chkTextUppercase
            // 
            resources.ApplyResources(this.chkTextUppercase, "chkTextUppercase");
            this.chkTextUppercase.Checked = true;
            this.chkTextUppercase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTextUppercase.Name = "chkTextUppercase";
            this.chkTextUppercase.UseVisualStyleBackColor = true;
            this.chkTextUppercase.CheckedChanged += new System.EventHandler(this.chkTextUppercase_CheckedChanged);
            // 
            // txtFont
            // 
            resources.ApplyResources(this.txtFont, "txtFont");
            this.txtFont.Name = "txtFont";
            // 
            // btnFonts
            // 
            resources.ApplyResources(this.btnFonts, "btnFonts");
            this.btnFonts.Name = "btnFonts";
            this.btnFonts.UseVisualStyleBackColor = true;
            this.btnFonts.Click += new System.EventHandler(this.btnFonts_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
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
            // karaokeEffect1
            // 
            this.karaokeEffect1.bColorContour = false;
            this.karaokeEffect1.bforceUppercase = false;
            this.karaokeEffect1.bTextBackGround = false;
            this.karaokeEffect1.FreqDirSlideShow = 10;            
            this.karaokeEffect1.imgLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.karaokeEffect1.KaraokeFont = new System.Drawing.Font("Comic Sans MS", 78.66666F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            resources.ApplyResources(this.karaokeEffect1, "karaokeEffect1");
            this.karaokeEffect1.m_Alpha = 255;
            this.karaokeEffect1.m_CurrentImage = null;
            this.karaokeEffect1.Name = "karaokeEffect1";
            this.karaokeEffect1.nbLyricsLines = 1;
            this.karaokeEffect1.OptionBackground = null;
            this.karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Top;
            this.karaokeEffect1.Position = 0;
            this.karaokeEffect1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.karaokeEffect1.StepPercent = 0.01F;
            this.karaokeEffect1.SyncLine = ((System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>)(resources.GetObject("karaokeEffect1.SyncLine")));
            this.karaokeEffect1.SyncLyrics = ((System.Collections.Generic.List<System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>>)(resources.GetObject("karaokeEffect1.SyncLyrics")));
            this.karaokeEffect1.TransitionEffect = keffect.KaraokeEffect.TransitionEffects.Progressive;
            this.karaokeEffect1.TransparencyKey = System.Drawing.Color.Lime;
            this.karaokeEffect1.TxtAlreadyPlayedColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(51)))));
            this.karaokeEffect1.TxtBackColor = System.Drawing.Color.Empty;
            this.karaokeEffect1.TxtBeingPlayedColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.karaokeEffect1.TxtContourColor = System.Drawing.Color.Empty;
            this.karaokeEffect1.TxtNotYetPlayedColor = System.Drawing.Color.White;
            // 
            // frmMp3LyrOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.karaokeEffect1);
            this.Controls.Add(this.chkTextUppercase);
            this.Controls.Add(this.txtFont);
            this.Controls.Add(this.btnFonts);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.chkTextBackground);
            this.Controls.Add(this.cbOptionsTextDisplay);
            this.Controls.Add(this.chkContour);
            this.Controls.Add(this.UpDownNbLines);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.pictContour);
            this.Controls.Add(this.pictNext);
            this.Controls.Add(this.pictHighlight);
            this.Controls.Add(this.pictBefore);
            this.Controls.Add(this.btnContourColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnForeColor);
            this.Controls.Add(this.btnSingColor);
            this.Controls.Add(this.btnSungColor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnResetDir);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbSizeMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtSlideShowFreq);
            this.Controls.Add(this.btnDirSlideShow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSlideShow);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictBackColor);
            this.Controls.Add(this.radioTransparent);
            this.Controls.Add(this.radioSolidColor);
            this.Controls.Add(this.radioDiaporama);
            this.Controls.Add(this.btnBackColor);
            this.Name = "frmMp3LyrOptions";
            ((System.ComponentModel.ISupportInitialize)(this.pictBackColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownNbLines)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictContour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictHighlight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictBefore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.PictureBox pictBackColor;
        private System.Windows.Forms.RadioButton radioTransparent;
        private System.Windows.Forms.RadioButton radioSolidColor;
        private System.Windows.Forms.RadioButton radioDiaporama;
        private System.Windows.Forms.Button btnBackColor;
        private System.Windows.Forms.Button btnResetDir;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbSizeMode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSlideShowFreq;
        private System.Windows.Forms.Button btnDirSlideShow;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSlideShow;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.CheckBox chkContour;
        private System.Windows.Forms.NumericUpDown UpDownNbLines;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.PictureBox pictContour;
        private System.Windows.Forms.PictureBox pictNext;
        private System.Windows.Forms.PictureBox pictHighlight;
        private System.Windows.Forms.PictureBox pictBefore;
        private System.Windows.Forms.Button btnContourColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnForeColor;
        private System.Windows.Forms.Button btnSingColor;
        private System.Windows.Forms.Button btnSungColor;
        private System.Windows.Forms.CheckBox chkTextUppercase;
        private System.Windows.Forms.TextBox txtFont;
        private System.Windows.Forms.Button btnFonts;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkTextBackground;
        private System.Windows.Forms.ComboBox cbOptionsTextDisplay;
        private keffect.KaraokeEffect karaokeEffect1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FontDialog fontDialog1;
    }
}