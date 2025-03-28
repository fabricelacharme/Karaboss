namespace Karaboss.Mp3
{
    partial class frmMp3Lyrics
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMp3Lyrics));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnEditLyrics = new System.Windows.Forms.Button();
            this.btnExportLyricsToText = new System.Windows.Forms.Button();
            this.btnFrmOptions = new System.Windows.Forms.Button();
            this.btnFrmMin = new System.Windows.Forms.Button();
            this.btnFrmMax = new System.Windows.Forms.Button();
            this.btnFrmClose = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlTitle = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.picBalls = new BallsControl.Balls();
            this.karaokeEffect1 = new keffect.KaraokeEffect();
            this.pnlWindow = new System.Windows.Forms.Panel();
            this.pnlTimer = new System.Windows.Forms.Timer(this.components);
            this.pnlTop.SuspendLayout();
            this.pnlTitle.SuspendLayout();
            this.pnlWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEditLyrics
            // 
            this.btnEditLyrics.BackColor = System.Drawing.Color.Gray;
            this.btnEditLyrics.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnEditLyrics, "btnEditLyrics");
            this.btnEditLyrics.Name = "btnEditLyrics";
            this.btnEditLyrics.TabStop = false;
            this.toolTip1.SetToolTip(this.btnEditLyrics, resources.GetString("btnEditLyrics.ToolTip"));
            this.btnEditLyrics.UseVisualStyleBackColor = false;
            this.btnEditLyrics.Click += new System.EventHandler(this.btnEditLyrics_Click);
            // 
            // btnExportLyricsToText
            // 
            this.btnExportLyricsToText.BackColor = System.Drawing.Color.Gray;
            this.btnExportLyricsToText.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnExportLyricsToText, "btnExportLyricsToText");
            this.btnExportLyricsToText.Name = "btnExportLyricsToText";
            this.btnExportLyricsToText.TabStop = false;
            this.toolTip1.SetToolTip(this.btnExportLyricsToText, resources.GetString("btnExportLyricsToText.ToolTip"));
            this.btnExportLyricsToText.UseVisualStyleBackColor = false;
            this.btnExportLyricsToText.Click += new System.EventHandler(this.btnExportLyricsToText_Click);
            // 
            // btnFrmOptions
            // 
            this.btnFrmOptions.BackColor = System.Drawing.Color.Gray;
            this.btnFrmOptions.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmOptions, "btnFrmOptions");
            this.btnFrmOptions.Name = "btnFrmOptions";
            this.btnFrmOptions.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmOptions, resources.GetString("btnFrmOptions.ToolTip"));
            this.btnFrmOptions.UseVisualStyleBackColor = false;
            this.btnFrmOptions.Click += new System.EventHandler(this.btnFrmOptions_Click);
            // 
            // btnFrmMin
            // 
            this.btnFrmMin.BackColor = System.Drawing.Color.Gray;
            this.btnFrmMin.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmMin, "btnFrmMin");
            this.btnFrmMin.Name = "btnFrmMin";
            this.btnFrmMin.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmMin, resources.GetString("btnFrmMin.ToolTip"));
            this.btnFrmMin.UseVisualStyleBackColor = false;
            this.btnFrmMin.Click += new System.EventHandler(this.btnFrmMin_Click);
            // 
            // btnFrmMax
            // 
            this.btnFrmMax.BackColor = System.Drawing.Color.Gray;
            this.btnFrmMax.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmMax, "btnFrmMax");
            this.btnFrmMax.Name = "btnFrmMax";
            this.btnFrmMax.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmMax, resources.GetString("btnFrmMax.ToolTip"));
            this.btnFrmMax.UseVisualStyleBackColor = false;
            this.btnFrmMax.Click += new System.EventHandler(this.btnFrmMax_Click);
            // 
            // btnFrmClose
            // 
            this.btnFrmClose.BackColor = System.Drawing.Color.Gray;
            this.btnFrmClose.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmClose, "btnFrmClose");
            this.btnFrmClose.Name = "btnFrmClose";
            this.btnFrmClose.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmClose, resources.GetString("btnFrmClose.ToolTip"));
            this.btnFrmClose.UseVisualStyleBackColor = false;
            this.btnFrmClose.Click += new System.EventHandler(this.btnFrmClose_Click);
            this.btnFrmClose.MouseLeave += new System.EventHandler(this.btnFrmClose_MouseLeave);
            this.btnFrmClose.MouseHover += new System.EventHandler(this.btnFrmClose_MouseHover);
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlTitle);
            this.pnlTop.Controls.Add(this.picBalls);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // pnlTitle
            // 
            this.pnlTitle.BackColor = System.Drawing.Color.Black;
            this.pnlTitle.Controls.Add(this.lblTitle);
            resources.ApplyResources(this.pnlTitle, "pnlTitle");
            this.pnlTitle.Name = "pnlTitle";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.BackColor = System.Drawing.Color.Black;
            this.lblTitle.ForeColor = System.Drawing.Color.Teal;
            this.lblTitle.Name = "lblTitle";
            // 
            // picBalls
            // 
            this.picBalls.BallsBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picBalls.BallsNumber = 0;
            this.picBalls.Division = 0F;
            resources.ApplyResources(this.picBalls, "picBalls");
            this.picBalls.Name = "picBalls";
            // 
            // karaokeEffect1
            // 
            this.karaokeEffect1.bColorContour = false;
            this.karaokeEffect1.bforceUppercase = false;
            this.karaokeEffect1.bTextBackGround = false;
            resources.ApplyResources(this.karaokeEffect1, "karaokeEffect1");
            this.karaokeEffect1.FreqDirSlideShow = 10;
            this.karaokeEffect1.imgLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.karaokeEffect1.KaraokeFont = new System.Drawing.Font("Comic Sans MS", 61.33333F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.karaokeEffect1.m_Alpha = 255;
            this.karaokeEffect1.m_CurrentImage = null;
            this.karaokeEffect1.m_DisplayRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.karaokeEffect1.Name = "karaokeEffect1";
            this.karaokeEffect1.nbLyricsLines = 1;
            this.karaokeEffect1.OptionBackground = null;
            this.karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Top;
            this.karaokeEffect1.Position = 0;
            this.karaokeEffect1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.karaokeEffect1.StepPercent = 0.01F;
            this.karaokeEffect1.SyncLine = ((System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>)(resources.GetObject("karaokeEffect1.SyncLine")));
            this.karaokeEffect1.SyncLyrics = ((System.Collections.Generic.List<System.Collections.Generic.List<keffect.KaraokeEffect.kSyncText>>)(resources.GetObject("karaokeEffect1.SyncLyrics")));
            this.karaokeEffect1.timerIntervall = ((long)(50));
            this.karaokeEffect1.TransitionEffect = keffect.KaraokeEffect.TransitionEffects.Progressive;
            this.karaokeEffect1.TransparencyKey = System.Drawing.Color.Lime;
            this.karaokeEffect1.TxtAlreadyPlayedColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(180)))), ((int)(((byte)(51)))));
            this.karaokeEffect1.TxtBackColor = System.Drawing.Color.Empty;
            this.karaokeEffect1.TxtBeingPlayedColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.karaokeEffect1.TxtContourColor = System.Drawing.Color.Empty;
            this.karaokeEffect1.TxtNotYetPlayedColor = System.Drawing.Color.White;
            // 
            // pnlWindow
            // 
            this.pnlWindow.BackColor = System.Drawing.Color.Gray;
            this.pnlWindow.Controls.Add(this.btnEditLyrics);
            this.pnlWindow.Controls.Add(this.btnExportLyricsToText);
            this.pnlWindow.Controls.Add(this.btnFrmOptions);
            this.pnlWindow.Controls.Add(this.btnFrmMin);
            this.pnlWindow.Controls.Add(this.btnFrmMax);
            this.pnlWindow.Controls.Add(this.btnFrmClose);
            resources.ApplyResources(this.pnlWindow, "pnlWindow");
            this.pnlWindow.Name = "pnlWindow";
            // 
            // pnlTimer
            // 
            this.pnlTimer.Tick += new System.EventHandler(this.pnlTimer_Tick);
            // 
            // frmMp3Lyrics
            // 
            resources.ApplyResources(this, "$this");
            this.ControlBox = false;
            this.Controls.Add(this.pnlWindow);
            this.Controls.Add(this.karaokeEffect1);
            this.Controls.Add(this.pnlTop);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMp3Lyrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3Lyrics_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3Lyrics_Load);
            this.Resize += new System.EventHandler(this.frmMp3Lyrics_Resize);
            this.pnlTop.ResumeLayout(false);
            this.pnlTitle.ResumeLayout(false);
            this.pnlTitle.PerformLayout();
            this.pnlWindow.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel pnlTop;
        private BallsControl.Balls picBalls;
        private System.Windows.Forms.Panel pnlTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Timer pnlTimer;
        private keffect.KaraokeEffect karaokeEffect1;
        private System.Windows.Forms.Panel pnlWindow;
        private System.Windows.Forms.Button btnEditLyrics;
        private System.Windows.Forms.Button btnExportLyricsToText;
        private System.Windows.Forms.Button btnFrmOptions;
        private System.Windows.Forms.Button btnFrmMin;
        private System.Windows.Forms.Button btnFrmMax;
        private System.Windows.Forms.Button btnFrmClose;
    }
}