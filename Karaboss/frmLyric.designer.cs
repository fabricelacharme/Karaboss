namespace Karaboss
{
    partial class frmLyric
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLyric));
            this.pnlBalls = new System.Windows.Forms.Panel();
            this.pnlTittle = new System.Windows.Forms.Panel();
            this.lblTittle = new System.Windows.Forms.Label();
            this.picBalls = new BallsControl.Balls();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlWindow = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLyricsChords = new System.Windows.Forms.Button();
            this.chkChords = new System.Windows.Forms.CheckBox();
            this.btnEditLyrics = new System.Windows.Forms.Button();
            this.btnFrmWords = new System.Windows.Forms.Button();
            this.btnFrmOptions = new System.Windows.Forms.Button();
            this.btnFrmMin = new System.Windows.Forms.Button();
            this.btnFrmMax = new System.Windows.Forms.Button();
            this.btnFrmClose = new System.Windows.Forms.Button();
            this.pBox = new PicControl.pictureBoxControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBalls.SuspendLayout();
            this.pnlTittle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlWindow.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBalls
            // 
            this.pnlBalls.Controls.Add(this.pnlTittle);
            this.pnlBalls.Controls.Add(this.picBalls);
            resources.ApplyResources(this.pnlBalls, "pnlBalls");
            this.pnlBalls.Name = "pnlBalls";
            // 
            // pnlTittle
            // 
            this.pnlTittle.BackColor = System.Drawing.Color.Black;
            this.pnlTittle.Controls.Add(this.lblTittle);
            resources.ApplyResources(this.pnlTittle, "pnlTittle");
            this.pnlTittle.Name = "pnlTittle";
            // 
            // lblTittle
            // 
            resources.ApplyResources(this.lblTittle, "lblTittle");
            this.lblTittle.BackColor = System.Drawing.Color.Black;
            this.lblTittle.ForeColor = System.Drawing.Color.Teal;
            this.lblTittle.Name = "lblTittle";
            // 
            // picBalls
            // 
            this.picBalls.Division = 0F;
            resources.ApplyResources(this.picBalls, "picBalls");
            this.picBalls.Name = "picBalls";
            this.picBalls.pBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picBalls.pBallsNumber = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlWindow);
            this.panel1.Controls.Add(this.pBox);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pnlWindow
            // 
            this.pnlWindow.BackColor = System.Drawing.Color.Gray;
            this.pnlWindow.Controls.Add(this.label1);
            this.pnlWindow.Controls.Add(this.btnLyricsChords);
            this.pnlWindow.Controls.Add(this.chkChords);
            this.pnlWindow.Controls.Add(this.btnEditLyrics);
            this.pnlWindow.Controls.Add(this.btnFrmWords);
            this.pnlWindow.Controls.Add(this.btnFrmOptions);
            this.pnlWindow.Controls.Add(this.btnFrmMin);
            this.pnlWindow.Controls.Add(this.btnFrmMax);
            this.pnlWindow.Controls.Add(this.btnFrmClose);
            resources.ApplyResources(this.pnlWindow, "pnlWindow");
            this.pnlWindow.Name = "pnlWindow";
            this.pnlWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlWindow_MouseDown);
            this.pnlWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PnlWindow_MouseMove);
            this.pnlWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PnlWindow_MouseUp);
            this.pnlWindow.Resize += new System.EventHandler(this.PnlWindow_Resize);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnLyricsChords
            // 
            this.btnLyricsChords.BackColor = System.Drawing.Color.Gray;
            this.btnLyricsChords.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnLyricsChords, "btnLyricsChords");
            this.btnLyricsChords.Name = "btnLyricsChords";
            this.btnLyricsChords.TabStop = false;
            this.toolTip1.SetToolTip(this.btnLyricsChords, resources.GetString("btnLyricsChords.ToolTip"));
            this.btnLyricsChords.UseVisualStyleBackColor = false;
            this.btnLyricsChords.Click += new System.EventHandler(this.btnLyricsChords_Click);
            // 
            // chkChords
            // 
            resources.ApplyResources(this.chkChords, "chkChords");
            this.chkChords.Name = "chkChords";
            this.toolTip1.SetToolTip(this.chkChords, resources.GetString("chkChords.ToolTip"));
            this.chkChords.UseVisualStyleBackColor = true;
            this.chkChords.CheckedChanged += new System.EventHandler(this.chkChords_CheckedChanged);
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
            // btnFrmWords
            // 
            this.btnFrmWords.BackColor = System.Drawing.Color.Gray;
            this.btnFrmWords.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            resources.ApplyResources(this.btnFrmWords, "btnFrmWords");
            this.btnFrmWords.Name = "btnFrmWords";
            this.btnFrmWords.TabStop = false;
            this.toolTip1.SetToolTip(this.btnFrmWords, resources.GetString("btnFrmWords.ToolTip"));
            this.btnFrmWords.UseVisualStyleBackColor = false;
            this.btnFrmWords.Click += new System.EventHandler(this.BtnFrmWords_Click);
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
            this.btnFrmOptions.Click += new System.EventHandler(this.BtnFrmOptions_Click);
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
            this.btnFrmMin.Click += new System.EventHandler(this.BtnFrmMin_Click);
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
            this.btnFrmMax.Click += new System.EventHandler(this.BtnFrmMax_Click);
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
            this.btnFrmClose.Click += new System.EventHandler(this.BtnFrmClose_Click);
            this.btnFrmClose.MouseLeave += new System.EventHandler(this.BtnFrmClose_MouseLeave);
            this.btnFrmClose.MouseHover += new System.EventHandler(this.BtnFrmClose_MouseHover);
            // 
            // pBox
            // 
            this.pBox.bColorContour = true;
            this.pBox.bDemo = false;
            this.pBox.BeatDuration = 0;
            this.pBox.bforceUppercase = false;
            this.pBox.bShowChords = false;
            this.pBox.bShowParagraphs = true;
            this.pBox.bTextBackGround = true;
            this.pBox.ChordFont = new System.Drawing.Font("Comic Sans MS", 8.25F);
            this.pBox.ChordHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.pBox.ChordNextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(196)))), ((int)(((byte)(13)))));
            this.pBox.CurrentTextPos = 2;
            this.pBox.CurrentTime = 3;
            this.pBox.DirSlideShow = null;
            resources.ApplyResources(this.pBox, "pBox");
            this.pBox.FreqDirSlideShow = 0;
            this.pBox.imgLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pBox.KaraokeFont = new System.Drawing.Font("Segoe Print", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pBox.LyricsTimes = ((System.Collections.Generic.List<int>)(resources.GetObject("pBox.LyricsTimes")));
            this.pBox.LyricsWords = ((System.Collections.Generic.List<string>)(resources.GetObject("pBox.LyricsWords")));
            this.pBox.m_Alpha = 255;
            this.pBox.m_CurrentImage = null;
            this.pBox.m_DisplayRectangle = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.pBox.Name = "pBox";
            this.pBox.OptionBackground = "SolidColor";
            this.pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Top;
            this.pBox.OptionShowChords = true;
            this.pBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
            this.pBox.TransparencyKey = System.Drawing.Color.Lime;
            this.pBox.Txt = "";
            this.pBox.TxtBackColor = System.Drawing.Color.White;
            this.pBox.TxtBeforeColor = System.Drawing.Color.YellowGreen;
            this.pBox.TxtContourColor = System.Drawing.Color.DarkTurquoise;
            this.pBox.TxtHighlightColor = System.Drawing.Color.Red;
            this.pBox.TxtNbLines = 3;
            this.pBox.TxtNextColor = System.Drawing.Color.White;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // frmLyric
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlBalls);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLyric";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLyric_FormClosing);
            this.Load += new System.EventHandler(this.FrmLyric_Load);
            this.Resize += new System.EventHandler(this.FrmLyric_Resize);
            this.pnlBalls.ResumeLayout(false);
            this.pnlTittle.ResumeLayout(false);
            this.pnlTittle.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.pnlWindow.ResumeLayout(false);
            this.pnlWindow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBalls;
        private System.Windows.Forms.Panel panel1;
        private BallsControl.Balls picBalls;
        private System.Windows.Forms.Panel pnlTittle;
        private System.Windows.Forms.Label lblTittle;
        private PicControl.pictureBoxControl pBox;
        private System.Windows.Forms.Panel pnlWindow;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnFrmMin;
        private System.Windows.Forms.Button btnFrmMax;
        private System.Windows.Forms.Button btnFrmClose;
        private System.Windows.Forms.Button btnFrmWords;
        private System.Windows.Forms.Button btnFrmOptions;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnEditLyrics;
        private System.Windows.Forms.Button btnLyricsChords;
        private System.Windows.Forms.CheckBox chkChords;
        private System.Windows.Forms.Label label1;
    }
}