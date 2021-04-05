namespace Karaboss.Search
{
    partial class SearchControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /*
        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        */

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.chkSongOnly = new System.Windows.Forms.CheckBox();
            this.chkNameOnly = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.chkCaseSensitive = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblIndexedFilesNumber = new System.Windows.Forms.Label();
            this.lblIndexedFiles = new System.Windows.Forms.Label();
            this.lblScan = new System.Windows.Forms.Label();
            this.btnScan = new System.Windows.Forms.Button();
            this.lblSearchDir = new System.Windows.Forms.Label();
            this.txtSearchDir = new System.Windows.Forms.TextBox();
            //this.listView = new System.Windows.Forms.ListView();
            this.listView = new MyListView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRemoveFromSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnutbAddtoPlayList = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuView = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuViewByAuthor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewByFile = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlFolder = new System.Windows.Forms.Panel();
            this.lblDirSong = new System.Windows.Forms.Label();
            this.fldDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.imgWnd = new System.Windows.Forms.ImageList(this.components);
            this.pnlSubject = new System.Windows.Forms.Panel();
            this.picSubject = new System.Windows.Forms.PictureBox();
            this.lblSubject = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlFolder.SuspendLayout();
            this.pnlSubject.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSubject)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.chkSongOnly);
            this.splitContainer1.Panel1.Controls.Add(this.chkNameOnly);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.lblSearch);
            this.splitContainer1.Panel1.Controls.Add(this.chkCaseSensitive);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearch);
            this.splitContainer1.Panel1.Controls.Add(this.lblIndexedFilesNumber);
            this.splitContainer1.Panel1.Controls.Add(this.lblIndexedFiles);
            this.splitContainer1.Panel1.Controls.Add(this.lblScan);
            this.splitContainer1.Panel1.Controls.Add(this.btnScan);
            this.splitContainer1.Panel1.Controls.Add(this.lblSearchDir);
            this.splitContainer1.Panel1.Controls.Add(this.txtSearchDir);
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.listView);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // chkSongOnly
            // 
            resources.ApplyResources(this.chkSongOnly, "chkSongOnly");
            this.chkSongOnly.Name = "chkSongOnly";
            this.chkSongOnly.UseVisualStyleBackColor = true;
            this.chkSongOnly.CheckedChanged += new System.EventHandler(this.ChkSongOnly_CheckedChanged);
            // 
            // chkNameOnly
            // 
            resources.ApplyResources(this.chkNameOnly, "chkNameOnly");
            this.chkNameOnly.Name = "chkNameOnly";
            this.chkNameOnly.UseVisualStyleBackColor = true;
            this.chkNameOnly.CheckedChanged += new System.EventHandler(this.ChkNameOnly_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblSearch
            // 
            resources.ApplyResources(this.lblSearch, "lblSearch");
            this.lblSearch.Name = "lblSearch";
            // 
            // chkCaseSensitive
            // 
            resources.ApplyResources(this.chkCaseSensitive, "chkCaseSensitive");
            this.chkCaseSensitive.Name = "chkCaseSensitive";
            this.chkCaseSensitive.UseVisualStyleBackColor = true;
            this.chkCaseSensitive.CheckedChanged += new System.EventHandler(this.ChkCaseSensitive_CheckedChanged);
            // 
            // txtSearch
            // 
            resources.ApplyResources(this.txtSearch, "txtSearch");
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSearch_KeyDown);
            // 
            // lblIndexedFilesNumber
            // 
            resources.ApplyResources(this.lblIndexedFilesNumber, "lblIndexedFilesNumber");
            this.lblIndexedFilesNumber.Name = "lblIndexedFilesNumber";
            // 
            // lblIndexedFiles
            // 
            resources.ApplyResources(this.lblIndexedFiles, "lblIndexedFiles");
            this.lblIndexedFiles.Name = "lblIndexedFiles";
            // 
            // lblScan
            // 
            resources.ApplyResources(this.lblScan, "lblScan");
            this.lblScan.Name = "lblScan";
            // 
            // btnScan
            // 
            resources.ApplyResources(this.btnScan, "btnScan");
            this.btnScan.Name = "btnScan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.BtnScan_Click);
            // 
            // lblSearchDir
            // 
            resources.ApplyResources(this.lblSearchDir, "lblSearchDir");
            this.lblSearchDir.Name = "lblSearchDir";
            // 
            // txtSearchDir
            // 
            resources.ApplyResources(this.txtSearchDir, "txtSearchDir");
            this.txtSearchDir.Name = "txtSearchDir";
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
            this.listView.AllowDrop = true;
            this.listView.HideSelection = false;
            this.listView.Name = "listView";
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ListView_AfterLabelEdit);
            this.listView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.ListView_BeforeLabelEdit);
            this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ListView_ItemDrag);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView_DragEnter);
            this.listView.DragOver += new System.Windows.Forms.DragEventHandler(this.ListView_DragOver);
            this.listView.DragLeave += new System.EventHandler(this.ListView_DragLeave);
            this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView_Keydown);
            this.listView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseDown);
            this.listView.MouseLeave += new System.EventHandler(this.ListView_MouseLeave);
            this.listView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseMove);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemoveFromSearch,
            this.toolStripSeparator2,
            this.btnPlay,
            this.btnEdit,
            this.toolStripSeparator1,
            this.mnutbAddtoPlayList,
            this.mnuView});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // btnRemoveFromSearch
            // 
            resources.ApplyResources(this.btnRemoveFromSearch, "btnRemoveFromSearch");
            this.btnRemoveFromSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemoveFromSearch.Image = global::Karaboss.Properties.Resources.Actions_delete_icon;
            this.btnRemoveFromSearch.Name = "btnRemoveFromSearch";
            this.btnRemoveFromSearch.Click += new System.EventHandler(this.BtnRemoveFromSearch_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // btnPlay
            // 
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::Karaboss.Properties.Resources.Action_Play_icon24;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // btnEdit
            // 
            resources.ApplyResources(this.btnEdit, "btnEdit");
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = global::Karaboss.Properties.Resources.Action_Edit;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // mnutbAddtoPlayList
            // 
            resources.ApplyResources(this.mnutbAddtoPlayList, "mnutbAddtoPlayList");
            this.mnutbAddtoPlayList.Image = global::Karaboss.Properties.Resources.Action_Playlist_icon;
            this.mnutbAddtoPlayList.Name = "mnutbAddtoPlayList";
            this.mnutbAddtoPlayList.Click += new System.EventHandler(this.MnutbAddtoPlayList_Click);
            // 
            // mnuView
            // 
            resources.ApplyResources(this.mnuView, "mnuView");
            this.mnuView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewByAuthor,
            this.mnuViewByFile});
            this.mnuView.Name = "mnuView";
            // 
            // mnuViewByAuthor
            // 
            resources.ApplyResources(this.mnuViewByAuthor, "mnuViewByAuthor");
            this.mnuViewByAuthor.Checked = true;
            this.mnuViewByAuthor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuViewByAuthor.Name = "mnuViewByAuthor";
            this.mnuViewByAuthor.Click += new System.EventHandler(this.MnuViewByAuthor_Click);
            // 
            // mnuViewByFile
            // 
            resources.ApplyResources(this.mnuViewByFile, "mnuViewByFile");
            this.mnuViewByFile.Name = "mnuViewByFile";
            this.mnuViewByFile.Click += new System.EventHandler(this.MnuViewByFile_Click);
            // 
            // pnlFolder
            // 
            resources.ApplyResources(this.pnlFolder, "pnlFolder");
            this.pnlFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFolder.Controls.Add(this.lblDirSong);
            this.pnlFolder.Name = "pnlFolder";
            // 
            // lblDirSong
            // 
            resources.ApplyResources(this.lblDirSong, "lblDirSong");
            this.lblDirSong.Name = "lblDirSong";
            // 
            // fldDialog
            // 
            resources.ApplyResources(this.fldDialog, "fldDialog");
            // 
            // imgWnd
            // 
            this.imgWnd.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imgWnd, "imgWnd");
            this.imgWnd.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pnlSubject
            // 
            resources.ApplyResources(this.pnlSubject, "pnlSubject");
            this.pnlSubject.Controls.Add(this.picSubject);
            this.pnlSubject.Controls.Add(this.lblSubject);
            this.pnlSubject.Name = "pnlSubject";
            // 
            // picSubject
            // 
            resources.ApplyResources(this.picSubject, "picSubject");
            this.picSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.picSubject.Name = "picSubject";
            this.picSubject.TabStop = false;
            // 
            // lblSubject
            // 
            resources.ApplyResources(this.lblSubject, "lblSubject");
            this.lblSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblSubject.ForeColor = System.Drawing.Color.White;
            this.lblSubject.Name = "lblSubject";
            // 
            // SearchControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlSubject);
            this.Controls.Add(this.pnlFolder);
            this.Name = "SearchControl";
            this.VisibleChanged += new System.EventHandler(this.SearchControl_VisibleChanged);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlFolder.ResumeLayout(false);
            this.pnlFolder.PerformLayout();
            this.pnlSubject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSubject)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtSearchDir;
        private System.Windows.Forms.Label lblSearchDir;
        private System.Windows.Forms.Label lblIndexedFilesNumber;
        private System.Windows.Forms.Label lblIndexedFiles;
        private System.Windows.Forms.Label lblScan;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.CheckBox chkCaseSensitive;
        private System.Windows.Forms.TextBox txtSearch;
        //private System.Windows.Forms.ListView listView;
        private MyListView listView;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.FolderBrowserDialog fldDialog;
        private System.Windows.Forms.Label lblDirSong;
        private System.Windows.Forms.ImageList imgWnd;
        private System.Windows.Forms.Panel pnlFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlSubject;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.PictureBox picSubject;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton mnutbAddtoPlayList;
        private System.Windows.Forms.ToolStripButton btnRemoveFromSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewByAuthor;
        private System.Windows.Forms.ToolStripMenuItem mnuViewByFile;
        private System.Windows.Forms.CheckBox chkNameOnly;
        private System.Windows.Forms.CheckBox chkSongOnly;
        private System.Windows.Forms.Label label2;
    }
}
