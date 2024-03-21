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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChords));
            this.tabChordsControl = new System.Windows.Forms.TabControl();
            this.tabPageDiagrams = new System.Windows.Forms.TabPage();
            this.tabPageOverview = new System.Windows.Forms.TabPage();
            this.tabPageEdit = new System.Windows.Forms.TabPage();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAboutSong = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabChordsControl.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabChordsControl
            // 
            this.tabChordsControl.Controls.Add(this.tabPageDiagrams);
            this.tabChordsControl.Controls.Add(this.tabPageOverview);
            this.tabChordsControl.Controls.Add(this.tabPageEdit);
            resources.ApplyResources(this.tabChordsControl, "tabChordsControl");
            this.tabChordsControl.Name = "tabChordsControl";
            this.tabChordsControl.SelectedIndex = 0;
            // 
            // tabPageDiagrams
            // 
            resources.ApplyResources(this.tabPageDiagrams, "tabPageDiagrams");
            this.tabPageDiagrams.Name = "tabPageDiagrams";
            this.tabPageDiagrams.UseVisualStyleBackColor = true;
            // 
            // tabPageOverview
            // 
            resources.ApplyResources(this.tabPageOverview, "tabPageOverview");
            this.tabPageOverview.Name = "tabPageOverview";
            this.tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // tabPageEdit
            // 
            resources.ApplyResources(this.tabPageEdit, "tabPageEdit");
            this.tabPageEdit.Name = "tabPageEdit";
            this.tabPageEdit.UseVisualStyleBackColor = true;
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
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
            this.mnuFilePrint,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Name = "mnuFilePrint";
            resources.ApplyResources(this.mnuFilePrint, "mnuFilePrint");
            this.mnuFilePrint.Click += new System.EventHandler(this.mnuFilePrint_Click);
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
            this.Resize += new System.EventHandler(this.frmChords_Resize);
            this.tabChordsControl.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabChordsControl;
        private System.Windows.Forms.TabPage tabPageDiagrams;
        private System.Windows.Forms.TabPage tabPageOverview;
        private System.Windows.Forms.TabPage tabPageEdit;
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrint;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAboutSong;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}