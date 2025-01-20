namespace Karaboss
{
    partial class frmChords
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChords));
            this.tabChordsControl = new System.Windows.Forms.TabControl();
            this.tabPageChords = new System.Windows.Forms.TabPage();
            this.tabPageMap = new System.Windows.Forms.TabPage();
            this.tabPageLyrics = new System.Windows.Forms.TabPage();
            this.tabPageModify = new System.Windows.Forms.TabPage();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrintLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrintPDF = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAboutSong = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.progressBarPlayer = new System.Windows.Forms.ProgressBar();
            this.tabChordsControl.SuspendLayout();
            this.pnlToolbar.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabChordsControl
            // 
            resources.ApplyResources(this.tabChordsControl, "tabChordsControl");
            this.tabChordsControl.Controls.Add(this.tabPageChords);
            this.tabChordsControl.Controls.Add(this.tabPageMap);
            this.tabChordsControl.Controls.Add(this.tabPageLyrics);
            this.tabChordsControl.Controls.Add(this.tabPageModify);
            this.tabChordsControl.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabChordsControl.Name = "tabChordsControl";
            this.tabChordsControl.SelectedIndex = 0;
            this.tabChordsControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabChordsControl_DrawItem);
            this.tabChordsControl.SelectedIndexChanged += new System.EventHandler(this.tabChordsControl_SelectedIndexChanged);
            // 
            // tabPageChords
            // 
            this.tabPageChords.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.tabPageChords, "tabPageChords");
            this.tabPageChords.Name = "tabPageChords";
            this.tabPageChords.UseVisualStyleBackColor = true;
            // 
            // tabPageMap
            // 
            resources.ApplyResources(this.tabPageMap, "tabPageMap");
            this.tabPageMap.Name = "tabPageMap";
            this.tabPageMap.UseVisualStyleBackColor = true;
            // 
            // tabPageLyrics
            // 
            resources.ApplyResources(this.tabPageLyrics, "tabPageLyrics");
            this.tabPageLyrics.Name = "tabPageLyrics";
            this.tabPageLyrics.UseVisualStyleBackColor = true;
            // 
            // tabPageModify
            // 
            resources.ApplyResources(this.tabPageModify, "tabPageModify");
            this.tabPageModify.Name = "tabPageModify";
            this.tabPageModify.UseVisualStyleBackColor = true;
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlToolbar.Controls.Add(this.progressBarPlayer);
            resources.ApplyResources(this.pnlToolbar, "pnlToolbar");
            this.pnlToolbar.Name = "pnlToolbar";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp,
            this.aboutToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFilePrintLyrics,
            this.mnuFilePrintPDF,
            this.mnuFileSep,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFilePrintLyrics
            // 
            this.mnuFilePrintLyrics.Name = "mnuFilePrintLyrics";
            resources.ApplyResources(this.mnuFilePrintLyrics, "mnuFilePrintLyrics");
            this.mnuFilePrintLyrics.Click += new System.EventHandler(this.mnuFilePrintLyrics_Click);
            // 
            // mnuFilePrintPDF
            // 
            this.mnuFilePrintPDF.Name = "mnuFilePrintPDF";
            resources.ApplyResources(this.mnuFilePrintPDF, "mnuFilePrintPDF");
            this.mnuFilePrintPDF.Click += new System.EventHandler(this.mnuFilePrintPDF_Click);
            // 
            // mnuFileSep
            // 
            this.mnuFileSep.Name = "mnuFileSep";
            resources.ApplyResources(this.mnuFileSep, "mnuFileSep");
            // 
            // mnuFileQuit
            // 
            this.mnuFileQuit.Name = "mnuFileQuit";
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout,
            this.mnuHelpAboutSong});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnuHelpAboutSong
            // 
            this.mnuHelpAboutSong.Name = "mnuHelpAboutSong";
            resources.ApplyResources(this.mnuHelpAboutSong, "mnuHelpAboutSong");
            this.mnuHelpAboutSong.Click += new System.EventHandler(this.mnuHelpAboutSong_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            // 
            // progressBarPlayer
            // 
            resources.ApplyResources(this.progressBarPlayer, "progressBarPlayer");
            this.progressBarPlayer.Name = "progressBarPlayer";
            // 
            // frmChords
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.tabChordsControl);
            this.Name = "frmChords";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChords_FormClosing);
            this.Load += new System.EventHandler(this.frmChords_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmChords_KeyUp);
            this.Move += new System.EventHandler(this.frmChords_Move);
            this.Resize += new System.EventHandler(this.frmChords_Resize);
            this.tabChordsControl.ResumeLayout(false);
            this.pnlToolbar.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabChordsControl;
        private System.Windows.Forms.TabPage tabPageChords;
        private System.Windows.Forms.TabPage tabPageMap;
        private System.Windows.Forms.TabPage tabPageLyrics;
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAboutSong;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrintPDF;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrintLyrics;
        private System.Windows.Forms.TabPage tabPageModify;
        private System.Windows.Forms.SaveFileDialog saveMidiFileDialog;
        private System.Windows.Forms.ProgressBar progressBarPlayer;
    }
}