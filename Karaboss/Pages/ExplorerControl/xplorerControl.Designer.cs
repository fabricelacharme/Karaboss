namespace Karaboss.xplorer
{
    partial class xplorerControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(xplorerControl));
            this.splitContainerFiles = new System.Windows.Forms.SplitContainer();
            this.treeView = new FlShell.ShellTreeView();
            this.shellListView = new FlShell.ShellListView();
            this.tvToolbar = new System.Windows.Forms.ToolStrip();
            this.backButton = new System.Windows.Forms.ToolStripButton();
            this.forwardButton = new System.Windows.Forms.ToolStripButton();
            this.upButton = new System.Windows.Forms.ToolStripButton();
            this.btnDownloads = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lvToolbar = new System.Windows.Forms.ToolStrip();
            this.BtnNewMidiFile = new System.Windows.Forms.ToolStripButton();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnutbAddToPlayList = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRename = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuRenameFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuInvertAuthorSong = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceButton = new System.Windows.Forms.ToolStripButton();
            this.pnlSubject = new System.Windows.Forms.Panel();
            this.picSubject = new System.Windows.Forms.PictureBox();
            this.lblSubject = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFiles)).BeginInit();
            this.splitContainerFiles.Panel1.SuspendLayout();
            this.splitContainerFiles.Panel2.SuspendLayout();
            this.splitContainerFiles.SuspendLayout();
            this.tvToolbar.SuspendLayout();
            this.lvToolbar.SuspendLayout();
            this.pnlSubject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSubject)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainerFiles
            // 
            resources.ApplyResources(this.splitContainerFiles, "splitContainerFiles");
            this.splitContainerFiles.Name = "splitContainerFiles";
            // 
            // splitContainerFiles.Panel1
            // 
            this.splitContainerFiles.Panel1.Controls.Add(this.treeView);
            this.splitContainerFiles.Panel1.Controls.Add(this.tvToolbar);
            // 
            // splitContainerFiles.Panel2
            // 
            this.splitContainerFiles.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainerFiles.Panel2.Controls.Add(this.shellListView);
            this.splitContainerFiles.Panel2.Controls.Add(this.lvToolbar);
            this.splitContainerFiles.TabStop = false;
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            resources.ApplyResources(this.treeView, "treeView");
            this.treeView.Name = "treeView";
            this.treeView.ShellListView = this.shellListView;
            // 
            // shellListView
            // 
            this.shellListView.AllowDrop = true;
            this.shellListView.CurrentFolder = new FlShell.ShellItem("shell:///Desktop");
            resources.ApplyResources(this.shellListView, "shellListView");
            this.shellListView.lvFileNameColumn = 0;
            this.shellListView.Name = "shellListView";
            this.shellListView.View = FlShell.ShellViewStyle.Details;
            this.shellListView.Navigated += new System.EventHandler(this.ShellListView_Navigated);
            // 
            // tvToolbar
            // 
            this.tvToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backButton,
            this.forwardButton,
            this.upButton,
            this.btnDownloads});
            resources.ApplyResources(this.tvToolbar, "tvToolbar");
            this.tvToolbar.Name = "tvToolbar";
            this.tvToolbar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.TvToolbar_ItemClicked);
            // 
            // backButton
            // 
            this.backButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.Name = "backButton";
            // 
            // forwardButton
            // 
            this.forwardButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.forwardButton, "forwardButton");
            this.forwardButton.Name = "forwardButton";
            // 
            // upButton
            // 
            this.upButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.Name = "upButton";
            // 
            // btnDownloads
            // 
            this.btnDownloads.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDownloads.Image = global::Karaboss.Properties.Resources.folder_classic_down;
            resources.ApplyResources(this.btnDownloads, "btnDownloads");
            this.btnDownloads.Name = "btnDownloads";
            this.btnDownloads.Click += new System.EventHandler(this.BtnDownloads_Click);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // lvToolbar
            // 
            this.lvToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnNewMidiFile,
            this.btnPlay,
            this.btnEdit,
            this.toolStripSeparator1,
            this.mnutbAddToPlayList,
            this.toolStripSeparator2,
            this.mnuRename,
            this.toolStripSeparator3,
            this.replaceButton});
            resources.ApplyResources(this.lvToolbar, "lvToolbar");
            this.lvToolbar.Name = "lvToolbar";
            // 
            // BtnNewMidiFile
            // 
            this.BtnNewMidiFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnNewMidiFile.Image = global::Karaboss.Properties.Resources.Action_New_File_icon;
            resources.ApplyResources(this.BtnNewMidiFile, "BtnNewMidiFile");
            this.BtnNewMidiFile.Name = "BtnNewMidiFile";
            this.BtnNewMidiFile.Click += new System.EventHandler(this.BtnNewMidiFile_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::Karaboss.Properties.Resources.Action_Play_icon24;
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = global::Karaboss.Properties.Resources.Action_Edit;
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // mnutbAddToPlayList
            // 
            this.mnutbAddToPlayList.Image = global::Karaboss.Properties.Resources.Action_Playlist_icon;
            resources.ApplyResources(this.mnutbAddToPlayList, "mnutbAddToPlayList");
            this.mnutbAddToPlayList.Name = "mnutbAddToPlayList";
            this.mnutbAddToPlayList.Click += new System.EventHandler(this.MnutbAddToPlayList_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // mnuRename
            // 
            this.mnuRename.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRenameFiles,
            this.mnuInvertAuthorSong});
            this.mnuRename.Image = global::Karaboss.Properties.Resources.Action_Playlist_icon;
            resources.ApplyResources(this.mnuRename, "mnuRename");
            this.mnuRename.Name = "mnuRename";
            // 
            // mnuRenameFiles
            // 
            this.mnuRenameFiles.Name = "mnuRenameFiles";
            resources.ApplyResources(this.mnuRenameFiles, "mnuRenameFiles");
            this.mnuRenameFiles.Click += new System.EventHandler(this.mnuRenameFiles_Click);
            // 
            // mnuInvertAuthorSong
            // 
            this.mnuInvertAuthorSong.Name = "mnuInvertAuthorSong";
            resources.ApplyResources(this.mnuInvertAuthorSong, "mnuInvertAuthorSong");
            this.mnuInvertAuthorSong.Click += new System.EventHandler(this.mnuInvertAuthorSong_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // replaceButton
            // 
            this.replaceButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.replaceButton, "replaceButton");
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
            // 
            // pnlSubject
            // 
            this.pnlSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.pnlSubject.Controls.Add(this.picSubject);
            this.pnlSubject.Controls.Add(this.lblSubject);
            resources.ApplyResources(this.pnlSubject, "pnlSubject");
            this.pnlSubject.Name = "pnlSubject";
            // 
            // picSubject
            // 
            resources.ApplyResources(this.picSubject, "picSubject");
            this.picSubject.Name = "picSubject";
            this.picSubject.TabStop = false;
            // 
            // lblSubject
            // 
            resources.ApplyResources(this.lblSubject, "lblSubject");
            this.lblSubject.ForeColor = System.Drawing.Color.White;
            this.lblSubject.Name = "lblSubject";
            // 
            // xplorerControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerFiles);
            this.Controls.Add(this.pnlSubject);
            this.Name = "xplorerControl";
            this.splitContainerFiles.Panel1.ResumeLayout(false);
            this.splitContainerFiles.Panel1.PerformLayout();
            this.splitContainerFiles.Panel2.ResumeLayout(false);
            this.splitContainerFiles.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFiles)).EndInit();
            this.splitContainerFiles.ResumeLayout(false);
            this.tvToolbar.ResumeLayout(false);
            this.tvToolbar.PerformLayout();
            this.lvToolbar.ResumeLayout(false);
            this.lvToolbar.PerformLayout();
            this.pnlSubject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSubject)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlSubject;
        private System.Windows.Forms.PictureBox picSubject;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.SplitContainer splitContainerFiles;
        private FlShell.ShellTreeView treeView;
        private System.Windows.Forms.ToolStrip tvToolbar;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripButton upButton;
        private System.Windows.Forms.ToolStrip lvToolbar;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton mnutbAddToPlayList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton replaceButton;
        private FlShell.ShellListView shellListView;
        private System.Windows.Forms.ToolStripButton btnDownloads;
        private System.Windows.Forms.ToolStripButton BtnNewMidiFile;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton mnuRename;
        private System.Windows.Forms.ToolStripMenuItem mnuRenameFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuInvertAuthorSong;
    }
}
