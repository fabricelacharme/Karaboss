namespace Karaboss.playlists
{
    partial class PlaylistsControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistsControl));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvPlaylistGroup = new System.Windows.Forms.TreeView();
            this.imgTrv = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnCreatePlaylistGroup = new System.Windows.Forms.ToolStripButton();
            this.btnDeletePlaylistGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCreatePlaylist = new System.Windows.Forms.ToolStripButton();
            this.BtnDeletePlaylist1 = new System.Windows.Forms.ToolStripButton();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.lblPlaylistDuration = new System.Windows.Forms.Label();
            this.txtPlaylistFile = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblPlaylistCount = new System.Windows.Forms.Label();
            this.lblPlName = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPlaylist = new System.Windows.Forms.Label();
            this.lblDriveInfos = new System.Windows.Forms.Label();
            this.lblUsualDrive = new System.Windows.Forms.Label();
            this.lblDrive = new System.Windows.Forms.Label();
            this.tabPlaylist = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView = new System.Windows.Forms.ListView();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.btnRemoveFromPlaylist = new System.Windows.Forms.ToolStripButton();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnutbAddtoPlayList = new System.Windows.Forms.ToolStripDropDownButton();
            this.pnlSongDetails = new System.Windows.Forms.Panel();
            this.btnReplaceSong = new System.Windows.Forms.Button();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.txtSong = new System.Windows.Forms.TextBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.txtAlbum = new System.Windows.Forms.TextBox();
            this.lblSinger = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.txtKaraokeSinger = new System.Windows.Forms.TextBox();
            this.txtNotation = new System.Windows.Forms.TextBox();
            this.chkMuteMelody = new System.Windows.Forms.CheckBox();
            this.btnUpd = new System.Windows.Forms.Button();
            this.lblMuteMelody = new System.Windows.Forms.Label();
            this.lblTags = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddSong = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDirSlideShow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDirSlideShow = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.imgWnd = new System.Windows.Forms.ImageList(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.pnlSubject = new System.Windows.Forms.Panel();
            this.picSubject = new System.Windows.Forms.PictureBox();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblStyle = new System.Windows.Forms.Label();
            this.TxtStyle = new System.Windows.Forms.TextBox();
            this.TxtPlaylistName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.tabPlaylist.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.pnlSongDetails.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.tvPlaylistGroup);
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel1.Controls.Add(this.pnlTop);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabPlaylist);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip2);
            this.splitContainer1.Panel2.Controls.Add(this.pnlSongDetails);
            this.splitContainer1.TabStop = false;
            // 
            // tvPlaylistGroup
            // 
            this.tvPlaylistGroup.AllowDrop = true;
            resources.ApplyResources(this.tvPlaylistGroup, "tvPlaylistGroup");
            this.tvPlaylistGroup.FullRowSelect = true;
            this.tvPlaylistGroup.HideSelection = false;
            this.tvPlaylistGroup.ImageList = this.imgTrv;
            this.tvPlaylistGroup.ItemHeight = 24;
            this.tvPlaylistGroup.LabelEdit = true;
            this.tvPlaylistGroup.Name = "tvPlaylistGroup";
            this.tvPlaylistGroup.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TvPlaylistGroup_AfterLabelEdit);
            this.tvPlaylistGroup.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.TvPlaylistGroup_ItemDrag);
            this.tvPlaylistGroup.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TvPlaylistGroup_AfterSelect);
            this.tvPlaylistGroup.DragDrop += new System.Windows.Forms.DragEventHandler(this.TvPlaylistGroup_DragDrop);
            this.tvPlaylistGroup.DragEnter += new System.Windows.Forms.DragEventHandler(this.TvPlaylistGroup_DragEnter);
            this.tvPlaylistGroup.DragOver += new System.Windows.Forms.DragEventHandler(this.TvPlaylistGroup_DragOver);
            this.tvPlaylistGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TvPlaylistGroup_KeyDown);
            this.tvPlaylistGroup.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TvPlaylistGroup_MouseDown);
            // 
            // imgTrv
            // 
            this.imgTrv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTrv.ImageStream")));
            this.imgTrv.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTrv.Images.SetKeyName(0, "folder.png");
            this.imgTrv.Images.SetKeyName(1, "playlist.png");
            this.imgTrv.Images.SetKeyName(2, "edit.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCreatePlaylistGroup,
            this.btnDeletePlaylistGroup,
            this.toolStripSeparator2,
            this.btnCreatePlaylist,
            this.BtnDeletePlaylist1});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // btnCreatePlaylistGroup
            // 
            this.btnCreatePlaylistGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCreatePlaylistGroup.Image = global::Karaboss.Properties.Resources.Action_folder241;
            resources.ApplyResources(this.btnCreatePlaylistGroup, "btnCreatePlaylistGroup");
            this.btnCreatePlaylistGroup.Name = "btnCreatePlaylistGroup";
            this.btnCreatePlaylistGroup.Click += new System.EventHandler(this.BtnCreatePlaylistGroup_Click);
            // 
            // btnDeletePlaylistGroup
            // 
            this.btnDeletePlaylistGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnDeletePlaylistGroup.Image = global::Karaboss.Properties.Resources.Action_folder_delete24;
            resources.ApplyResources(this.btnDeletePlaylistGroup, "btnDeletePlaylistGroup");
            this.btnDeletePlaylistGroup.Name = "btnDeletePlaylistGroup";
            this.btnDeletePlaylistGroup.Click += new System.EventHandler(this.BtnDeletePlaylistGroup_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // btnCreatePlaylist
            // 
            this.btnCreatePlaylist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnCreatePlaylist, "btnCreatePlaylist");
            this.btnCreatePlaylist.Name = "btnCreatePlaylist";
            this.btnCreatePlaylist.Click += new System.EventHandler(this.BtnCreatePlaylist_Click);
            // 
            // BtnDeletePlaylist1
            // 
            this.BtnDeletePlaylist1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnDeletePlaylist1.Image = global::Karaboss.Properties.Resources.Action_playlist_delete24;
            resources.ApplyResources(this.BtnDeletePlaylist1, "BtnDeletePlaylist1");
            this.BtnDeletePlaylist1.Name = "BtnDeletePlaylist1";
            this.BtnDeletePlaylist1.Click += new System.EventHandler(this.BtnDeletePlaylist_Click);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.pnlTop.Controls.Add(this.TxtPlaylistName);
            this.pnlTop.Controls.Add(this.TxtStyle);
            this.pnlTop.Controls.Add(this.lblStyle);
            this.pnlTop.Controls.Add(this.label10);
            this.pnlTop.Controls.Add(this.lblPlaylistDuration);
            this.pnlTop.Controls.Add(this.txtPlaylistFile);
            this.pnlTop.Controls.Add(this.label9);
            this.pnlTop.Controls.Add(this.lblPlaylistCount);
            this.pnlTop.Controls.Add(this.lblPlName);
            this.pnlTop.Controls.Add(this.label8);
            this.pnlTop.Controls.Add(this.lblPlaylist);
            this.pnlTop.Controls.Add(this.lblDriveInfos);
            this.pnlTop.Controls.Add(this.lblUsualDrive);
            this.pnlTop.Controls.Add(this.lblDrive);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // lblPlaylistDuration
            // 
            this.lblPlaylistDuration.BackColor = System.Drawing.Color.White;
            this.lblPlaylistDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblPlaylistDuration, "lblPlaylistDuration");
            this.lblPlaylistDuration.ForeColor = System.Drawing.Color.Black;
            this.lblPlaylistDuration.Name = "lblPlaylistDuration";
            // 
            // txtPlaylistFile
            // 
            resources.ApplyResources(this.txtPlaylistFile, "txtPlaylistFile");
            this.txtPlaylistFile.Name = "txtPlaylistFile";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // lblPlaylistCount
            // 
            this.lblPlaylistCount.BackColor = System.Drawing.Color.White;
            this.lblPlaylistCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblPlaylistCount, "lblPlaylistCount");
            this.lblPlaylistCount.ForeColor = System.Drawing.Color.Black;
            this.lblPlaylistCount.Name = "lblPlaylistCount";
            // 
            // lblPlName
            // 
            resources.ApplyResources(this.lblPlName, "lblPlName");
            this.lblPlName.Name = "lblPlName";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // lblPlaylist
            // 
            resources.ApplyResources(this.lblPlaylist, "lblPlaylist");
            this.lblPlaylist.ForeColor = System.Drawing.Color.White;
            this.lblPlaylist.Name = "lblPlaylist";
            // 
            // lblDriveInfos
            // 
            resources.ApplyResources(this.lblDriveInfos, "lblDriveInfos");
            this.lblDriveInfos.ForeColor = System.Drawing.Color.White;
            this.lblDriveInfos.Name = "lblDriveInfos";
            // 
            // lblUsualDrive
            // 
            resources.ApplyResources(this.lblUsualDrive, "lblUsualDrive");
            this.lblUsualDrive.ForeColor = System.Drawing.Color.White;
            this.lblUsualDrive.Name = "lblUsualDrive";
            // 
            // lblDrive
            // 
            resources.ApplyResources(this.lblDrive, "lblDrive");
            this.lblDrive.ForeColor = System.Drawing.Color.White;
            this.lblDrive.Name = "lblDrive";
            // 
            // tabPlaylist
            // 
            this.tabPlaylist.Controls.Add(this.tabPage1);
            resources.ApplyResources(this.tabPlaylist, "tabPlaylist");
            this.tabPlaylist.Name = "tabPlaylist";
            this.tabPlaylist.SelectedIndex = 0;
            this.tabPlaylist.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.AllowDrop = true;
            resources.ApplyResources(this.listView, "listView");
            this.listView.Name = "listView";
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.ListView_ItemDrag);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.ListView_SelectedIndexChanged);
            this.listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.ListView_DragDrop);
            this.listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.ListView_DragEnter);
            this.listView.DragOver += new System.Windows.Forms.DragEventHandler(this.ListView_DragOver);
            this.listView.DragLeave += new System.EventHandler(this.ListView_DragLeave);
            this.listView.DoubleClick += new System.EventHandler(this.ListView_DoubleClick);
            this.listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListView_KeyDown);
            this.listView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ListView_MouseDown);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRemoveFromPlaylist,
            this.btnPlay,
            this.btnEdit,
            this.toolStripSeparator1,
            this.mnutbAddtoPlayList});
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.Name = "toolStrip2";
            // 
            // btnRemoveFromPlaylist
            // 
            this.btnRemoveFromPlaylist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.btnRemoveFromPlaylist, "btnRemoveFromPlaylist");
            this.btnRemoveFromPlaylist.Name = "btnRemoveFromPlaylist";
            this.btnRemoveFromPlaylist.Click += new System.EventHandler(this.BtnRemoveFromPlaylist_Click);
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
            // mnutbAddtoPlayList
            // 
            this.mnutbAddtoPlayList.Image = global::Karaboss.Properties.Resources.Action_Playlist_icon;
            resources.ApplyResources(this.mnutbAddtoPlayList, "mnutbAddtoPlayList");
            this.mnutbAddtoPlayList.Name = "mnutbAddtoPlayList";
            this.mnutbAddtoPlayList.Click += new System.EventHandler(this.MnutbAddtoPlayList_Click);
            // 
            // pnlSongDetails
            // 
            this.pnlSongDetails.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(165)))), ((int)(((byte)(166)))));
            this.pnlSongDetails.Controls.Add(this.btnReplaceSong);
            this.pnlSongDetails.Controls.Add(this.txtArtist);
            this.pnlSongDetails.Controls.Add(this.txtSong);
            this.pnlSongDetails.Controls.Add(this.txtFile);
            this.pnlSongDetails.Controls.Add(this.txtAlbum);
            this.pnlSongDetails.Controls.Add(this.lblSinger);
            this.pnlSongDetails.Controls.Add(this.txtLength);
            this.pnlSongDetails.Controls.Add(this.txtKaraokeSinger);
            this.pnlSongDetails.Controls.Add(this.txtNotation);
            this.pnlSongDetails.Controls.Add(this.chkMuteMelody);
            this.pnlSongDetails.Controls.Add(this.btnUpd);
            this.pnlSongDetails.Controls.Add(this.lblMuteMelody);
            this.pnlSongDetails.Controls.Add(this.lblTags);
            this.pnlSongDetails.Controls.Add(this.label1);
            this.pnlSongDetails.Controls.Add(this.btnAddSong);
            this.pnlSongDetails.Controls.Add(this.label2);
            this.pnlSongDetails.Controls.Add(this.btnDirSlideShow);
            this.pnlSongDetails.Controls.Add(this.label3);
            this.pnlSongDetails.Controls.Add(this.label7);
            this.pnlSongDetails.Controls.Add(this.label4);
            this.pnlSongDetails.Controls.Add(this.txtDirSlideShow);
            this.pnlSongDetails.Controls.Add(this.label5);
            this.pnlSongDetails.Controls.Add(this.label6);
            resources.ApplyResources(this.pnlSongDetails, "pnlSongDetails");
            this.pnlSongDetails.Name = "pnlSongDetails";
            this.pnlSongDetails.Resize += new System.EventHandler(this.PnlSongDetails_Resize);
            // 
            // btnReplaceSong
            // 
            resources.ApplyResources(this.btnReplaceSong, "btnReplaceSong");
            this.btnReplaceSong.Name = "btnReplaceSong";
            this.btnReplaceSong.UseVisualStyleBackColor = true;
            this.btnReplaceSong.Click += new System.EventHandler(this.BtnReplaceSong_Click);
            // 
            // txtArtist
            // 
            resources.ApplyResources(this.txtArtist, "txtArtist");
            this.txtArtist.Name = "txtArtist";
            this.txtArtist.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtArtist_KeyPress);
            // 
            // txtSong
            // 
            resources.ApplyResources(this.txtSong, "txtSong");
            this.txtSong.Name = "txtSong";
            // 
            // txtFile
            // 
            resources.ApplyResources(this.txtFile, "txtFile");
            this.txtFile.Name = "txtFile";
            // 
            // txtAlbum
            // 
            resources.ApplyResources(this.txtAlbum, "txtAlbum");
            this.txtAlbum.Name = "txtAlbum";
            this.txtAlbum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtAlbum_KeyPress);
            // 
            // lblSinger
            // 
            resources.ApplyResources(this.lblSinger, "lblSinger");
            this.lblSinger.Name = "lblSinger";
            // 
            // txtLength
            // 
            resources.ApplyResources(this.txtLength, "txtLength");
            this.txtLength.Name = "txtLength";
            this.txtLength.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtLength_KeyPress);
            // 
            // txtKaraokeSinger
            // 
            resources.ApplyResources(this.txtKaraokeSinger, "txtKaraokeSinger");
            this.txtKaraokeSinger.Name = "txtKaraokeSinger";
            this.txtKaraokeSinger.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtKaraokeSinger_KeyPress);
            // 
            // txtNotation
            // 
            resources.ApplyResources(this.txtNotation, "txtNotation");
            this.txtNotation.Name = "txtNotation";
            this.txtNotation.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtNotation_KeyPress);
            // 
            // chkMuteMelody
            // 
            resources.ApplyResources(this.chkMuteMelody, "chkMuteMelody");
            this.chkMuteMelody.Name = "chkMuteMelody";
            this.chkMuteMelody.UseVisualStyleBackColor = true;
            // 
            // btnUpd
            // 
            resources.ApplyResources(this.btnUpd, "btnUpd");
            this.btnUpd.Name = "btnUpd";
            this.btnUpd.UseVisualStyleBackColor = true;
            this.btnUpd.Click += new System.EventHandler(this.BtnUpd_Click);
            // 
            // lblMuteMelody
            // 
            resources.ApplyResources(this.lblMuteMelody, "lblMuteMelody");
            this.lblMuteMelody.Name = "lblMuteMelody";
            // 
            // lblTags
            // 
            resources.ApplyResources(this.lblTags, "lblTags");
            this.lblTags.Name = "lblTags";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // btnAddSong
            // 
            resources.ApplyResources(this.btnAddSong, "btnAddSong");
            this.btnAddSong.Name = "btnAddSong";
            this.btnAddSong.UseVisualStyleBackColor = true;
            this.btnAddSong.Click += new System.EventHandler(this.BtnAddSong_Click);
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtDirSlideShow
            // 
            resources.ApplyResources(this.txtDirSlideShow, "txtDirSlideShow");
            this.txtDirSlideShow.Name = "txtDirSlideShow";
            this.txtDirSlideShow.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtDirSlideShow_KeyPress);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // imgWnd
            // 
            this.imgWnd.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imgWnd, "imgWnd");
            this.imgWnd.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pnlSubject
            // 
            this.pnlSubject.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
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
            // lblStyle
            // 
            resources.ApplyResources(this.lblStyle, "lblStyle");
            this.lblStyle.Name = "lblStyle";
            // 
            // TxtStyle
            // 
            resources.ApplyResources(this.TxtStyle, "TxtStyle");
            this.TxtStyle.Name = "TxtStyle";
            this.TxtStyle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtStyle_KeyDown);
            // 
            // TxtPlaylistName
            // 
            resources.ApplyResources(this.TxtPlaylistName, "TxtPlaylistName");
            this.TxtPlaylistName.Name = "TxtPlaylistName";
            this.TxtPlaylistName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtPlaylistName_KeyDown);
            // 
            // PlaylistsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlSubject);
            this.Name = "PlaylistsControl";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tabPlaylist.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.pnlSongDetails.ResumeLayout(false);
            this.pnlSongDetails.PerformLayout();
            this.pnlSubject.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSubject)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ImageList imgWnd;
        private System.Windows.Forms.Panel pnlSongDetails;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.TextBox txtSong;
        private System.Windows.Forms.TextBox txtFile;
        private System.Windows.Forms.TextBox txtAlbum;
        private System.Windows.Forms.Label lblSinger;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.TextBox txtKaraokeSinger;
        private System.Windows.Forms.TextBox txtNotation;
        private System.Windows.Forms.CheckBox chkMuteMelody;
        private System.Windows.Forms.Button btnUpd;
        private System.Windows.Forms.Label lblMuteMelody;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddSong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDirSlideShow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDirSlideShow;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPlaylist;
        private System.Windows.Forms.Label lblDriveInfos;
        private System.Windows.Forms.Label lblUsualDrive;
        private System.Windows.Forms.Label lblDrive;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton btnRemoveFromPlaylist;
        private System.Windows.Forms.ToolStripButton btnCreatePlaylist;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDlg;
        private System.Windows.Forms.Panel pnlSubject;
        private System.Windows.Forms.PictureBox picSubject;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.TabControl tabPlaylist;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnReplaceSong;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblPlaylistCount;
        private System.Windows.Forms.Label lblPlName;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtPlaylistFile;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton mnutbAddtoPlayList;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblPlaylistDuration;
        private System.Windows.Forms.ImageList imgTrv;
        private System.Windows.Forms.ToolStripButton btnCreatePlaylistGroup;
        private System.Windows.Forms.ToolStripButton btnDeletePlaylistGroup;
        private System.Windows.Forms.TreeView tvPlaylistGroup;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton BtnDeletePlaylist1;
        private System.Windows.Forms.Label lblStyle;
        private System.Windows.Forms.TextBox TxtStyle;
        private System.Windows.Forms.TextBox TxtPlaylistName;
    }
}
