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
            this.tabChordsControl.SuspendLayout();
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
            this.toolTip1.SetToolTip(this.tabChordsControl, resources.GetString("tabChordsControl.ToolTip"));
            this.tabChordsControl.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabChordsControl_DrawItem);
            this.tabChordsControl.SelectedIndexChanged += new System.EventHandler(this.tabChordsControl_SelectedIndexChanged);
            // 
            // tabPageChords
            // 
            resources.ApplyResources(this.tabPageChords, "tabPageChords");
            this.tabPageChords.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tabPageChords.Name = "tabPageChords";
            this.toolTip1.SetToolTip(this.tabPageChords, resources.GetString("tabPageChords.ToolTip"));
            this.tabPageChords.UseVisualStyleBackColor = true;
            // 
            // tabPageMap
            // 
            resources.ApplyResources(this.tabPageMap, "tabPageMap");
            this.tabPageMap.Name = "tabPageMap";
            this.toolTip1.SetToolTip(this.tabPageMap, resources.GetString("tabPageMap.ToolTip"));
            this.tabPageMap.UseVisualStyleBackColor = true;
            // 
            // tabPageLyrics
            // 
            resources.ApplyResources(this.tabPageLyrics, "tabPageLyrics");
            this.tabPageLyrics.Name = "tabPageLyrics";
            this.toolTip1.SetToolTip(this.tabPageLyrics, resources.GetString("tabPageLyrics.ToolTip"));
            this.tabPageLyrics.UseVisualStyleBackColor = true;
            // 
            // tabPageModify
            // 
            resources.ApplyResources(this.tabPageModify, "tabPageModify");
            this.tabPageModify.Name = "tabPageModify";
            this.toolTip1.SetToolTip(this.tabPageModify, resources.GetString("tabPageModify.ToolTip"));
            this.tabPageModify.UseVisualStyleBackColor = true;
            // 
            // pnlToolbar
            // 
            resources.ApplyResources(this.pnlToolbar, "pnlToolbar");
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlToolbar.Name = "pnlToolbar";
            this.toolTip1.SetToolTip(this.pnlToolbar, resources.GetString("pnlToolbar.ToolTip"));
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuHelp,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.toolTip1.SetToolTip(this.menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // mnuFile
            // 
            resources.ApplyResources(this.mnuFile, "mnuFile");
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFilePrintLyrics,
            this.mnuFilePrintPDF,
            this.mnuFileSep,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            // 
            // mnuFilePrintLyrics
            // 
            resources.ApplyResources(this.mnuFilePrintLyrics, "mnuFilePrintLyrics");
            this.mnuFilePrintLyrics.Name = "mnuFilePrintLyrics";
            this.mnuFilePrintLyrics.Click += new System.EventHandler(this.mnuFilePrintLyrics_Click);
            // 
            // mnuFilePrintPDF
            // 
            resources.ApplyResources(this.mnuFilePrintPDF, "mnuFilePrintPDF");
            this.mnuFilePrintPDF.Name = "mnuFilePrintPDF";
            this.mnuFilePrintPDF.Click += new System.EventHandler(this.mnuFilePrintPDF_Click);
            // 
            // mnuFileSep
            // 
            resources.ApplyResources(this.mnuFileSep, "mnuFileSep");
            this.mnuFileSep.Name = "mnuFileSep";
            // 
            // mnuFileQuit
            // 
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Name = "mnuFileQuit";
            // 
            // mnuHelp
            // 
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout,
            this.mnuHelpAboutSong});
            this.mnuHelp.Name = "mnuHelp";
            // 
            // mnuHelpAbout
            // 
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // mnuHelpAboutSong
            // 
            resources.ApplyResources(this.mnuHelpAboutSong, "mnuHelpAboutSong");
            this.mnuHelpAboutSong.Name = "mnuHelpAboutSong";
            this.mnuHelpAboutSong.Click += new System.EventHandler(this.mnuHelpAboutSong_Click);
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            // 
            // frmChords
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.tabChordsControl);
            this.Name = "frmChords";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChords_FormClosing);
            this.Load += new System.EventHandler(this.frmChords_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmChords_KeyUp);
            this.Resize += new System.EventHandler(this.frmChords_Resize);
            this.tabChordsControl.ResumeLayout(false);
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
    }
}