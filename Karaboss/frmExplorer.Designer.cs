namespace Karaboss
{
    partial class frmExplorer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExplorer));
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tssLeft = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssMiddle = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssRight = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditExplore = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuEditConnected = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuEditPianoTraining = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuEditGuitarTraining = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOption = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuToolsSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsMngtFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsMngtFilesDeleteEmptyDirs = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuToolsSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsMngtFilesSearchDoubles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsMngtFilesSearchDoublesSingle = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuToolsSep3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuToolsSep4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsMngtFilesSearchSameNameComparedToReference = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidi = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidiInputDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidiOutputDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuMidiExternal = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiExternalPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiExternalRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuHelpCheckNewVersion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.sequence1 = new Sanford.Multimedia.Midi.Sequence();
            this.pnlFileInfos = new System.Windows.Forms.Panel();
            this.lblLyrics = new System.Windows.Forms.Label();
            this.lblWtags = new System.Windows.Forms.Label();
            this.lblVtags = new System.Windows.Forms.Label();
            this.lblTtags = new System.Windows.Forms.Label();
            this.lblLtags = new System.Windows.Forms.Label();
            this.lblKtags = new System.Windows.Forms.Label();
            this.lblItags = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblFormat = new System.Windows.Forms.Label();
            this.lblTracks = new System.Windows.Forms.Label();
            this.sideBarControl = new VBarControl.SideBarControl.SideBarControl();
            this.connectedControl = new Karaboss.Pages.ConnectedControl();
            this.xplorerControl = new Karaboss.xplorer.xplorerControl();
            this.playlistsControl = new Karaboss.playlists.PlaylistsControl();
            this.searchControl = new Karaboss.Search.SearchControl();
            this.statusBar.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.pnlFileInfos.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLeft,
            this.tssMiddle,
            this.tssRight});
            this.statusBar.Name = "statusBar";
            // 
            // tssLeft
            // 
            resources.ApplyResources(this.tssLeft, "tssLeft");
            this.tssLeft.Name = "tssLeft";
            // 
            // tssMiddle
            // 
            resources.ApplyResources(this.tssMiddle, "tssMiddle");
            this.tssMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssMiddle.Name = "tssMiddle";
            this.tssMiddle.Spring = true;
            // 
            // tssRight
            // 
            resources.ApplyResources(this.tssRight, "tssRight");
            this.tssRight.Name = "tssRight";
            // 
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuTools,
            this.mnuMidi,
            this.mnuHelp});
            this.menuStrip.Name = "menuStrip";
            // 
            // mnuFile
            // 
            resources.ApplyResources(this.mnuFile, "mnuFile");
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileEdit,
            this.MnuFileSep1,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            // 
            // mnuFileNew
            // 
            resources.ApplyResources(this.mnuFileNew, "mnuFileNew");
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.Click += new System.EventHandler(this.MnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            resources.ApplyResources(this.mnuFileOpen, "mnuFileOpen");
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Click += new System.EventHandler(this.MnuFileOpen_Click);
            // 
            // mnuFileEdit
            // 
            resources.ApplyResources(this.mnuFileEdit, "mnuFileEdit");
            this.mnuFileEdit.Name = "mnuFileEdit";
            this.mnuFileEdit.Click += new System.EventHandler(this.MnuFileEdit_Click);
            // 
            // MnuFileSep1
            // 
            resources.ApplyResources(this.MnuFileSep1, "MnuFileSep1");
            this.MnuFileSep1.Name = "MnuFileSep1";
            // 
            // mnuFileQuit
            // 
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Name = "mnuFileQuit";
            this.mnuFileQuit.Click += new System.EventHandler(this.MnuFileQuit_Click);
            // 
            // mnuEdit
            // 
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditSearch,
            this.mnuEditExplore,
            this.mnuEditPlaylist,
            this.MnuEditConnected,
            this.MnuEditPianoTraining,
            this.MnuEditGuitarTraining});
            this.mnuEdit.Name = "mnuEdit";
            // 
            // mnuEditSearch
            // 
            resources.ApplyResources(this.mnuEditSearch, "mnuEditSearch");
            this.mnuEditSearch.Name = "mnuEditSearch";
            this.mnuEditSearch.Click += new System.EventHandler(this.MnuSearch_Click);
            // 
            // mnuEditExplore
            // 
            resources.ApplyResources(this.mnuEditExplore, "mnuEditExplore");
            this.mnuEditExplore.Name = "mnuEditExplore";
            this.mnuEditExplore.Click += new System.EventHandler(this.MnuEditExplore_Click);
            // 
            // mnuEditPlaylist
            // 
            resources.ApplyResources(this.mnuEditPlaylist, "mnuEditPlaylist");
            this.mnuEditPlaylist.Name = "mnuEditPlaylist";
            this.mnuEditPlaylist.Click += new System.EventHandler(this.MnuEditPlaylist_Click);
            // 
            // MnuEditConnected
            // 
            resources.ApplyResources(this.MnuEditConnected, "MnuEditConnected");
            this.MnuEditConnected.Name = "MnuEditConnected";
            this.MnuEditConnected.Click += new System.EventHandler(this.MnuEditConnected_Click);
            // 
            // MnuEditPianoTraining
            // 
            resources.ApplyResources(this.MnuEditPianoTraining, "MnuEditPianoTraining");
            this.MnuEditPianoTraining.Name = "MnuEditPianoTraining";
            this.MnuEditPianoTraining.Click += new System.EventHandler(this.MnuEditPianoTraining_Click);
            // 
            // MnuEditGuitarTraining
            // 
            resources.ApplyResources(this.MnuEditGuitarTraining, "MnuEditGuitarTraining");
            this.MnuEditGuitarTraining.Name = "MnuEditGuitarTraining";
            this.MnuEditGuitarTraining.Click += new System.EventHandler(this.MnuEditGuitarTraining_Click);
            // 
            // mnuTools
            // 
            resources.ApplyResources(this.mnuTools, "mnuTools");
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOption,
            this.MnuToolsSep1,
            this.mnuToolsMngtFiles});
            this.mnuTools.Name = "mnuTools";
            // 
            // mnuToolsOption
            // 
            resources.ApplyResources(this.mnuToolsOption, "mnuToolsOption");
            this.mnuToolsOption.Name = "mnuToolsOption";
            this.mnuToolsOption.Click += new System.EventHandler(this.MnuToolsOption_Click);
            // 
            // MnuToolsSep1
            // 
            resources.ApplyResources(this.MnuToolsSep1, "MnuToolsSep1");
            this.MnuToolsSep1.Name = "MnuToolsSep1";
            // 
            // mnuToolsMngtFiles
            // 
            resources.ApplyResources(this.mnuToolsMngtFiles, "mnuToolsMngtFiles");
            this.mnuToolsMngtFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsMngtFilesDeleteEmptyDirs,
            this.MnuToolsSep2,
            this.mnuToolsMngtFilesSearchDoubles,
            this.mnuToolsMngtFilesSearchDoublesSingle,
            this.MnuToolsSep3,
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference,
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory,
            this.MnuToolsSep4,
            this.mnuToolsMngtFilesSearchSameNameComparedToReference,
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory});
            this.mnuToolsMngtFiles.Name = "mnuToolsMngtFiles";
            // 
            // mnuToolsMngtFilesDeleteEmptyDirs
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesDeleteEmptyDirs, "mnuToolsMngtFilesDeleteEmptyDirs");
            this.mnuToolsMngtFilesDeleteEmptyDirs.Name = "mnuToolsMngtFilesDeleteEmptyDirs";
            this.mnuToolsMngtFilesDeleteEmptyDirs.Click += new System.EventHandler(this.MnuToolsMngtFilesDeleteEmptyDirs_Click);
            // 
            // MnuToolsSep2
            // 
            resources.ApplyResources(this.MnuToolsSep2, "MnuToolsSep2");
            this.MnuToolsSep2.Name = "MnuToolsSep2";
            // 
            // mnuToolsMngtFilesSearchDoubles
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchDoubles, "mnuToolsMngtFilesSearchDoubles");
            this.mnuToolsMngtFilesSearchDoubles.Name = "mnuToolsMngtFilesSearchDoubles";
            this.mnuToolsMngtFilesSearchDoubles.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchDoubles_Click);
            // 
            // mnuToolsMngtFilesSearchDoublesSingle
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchDoublesSingle, "mnuToolsMngtFilesSearchDoublesSingle");
            this.mnuToolsMngtFilesSearchDoublesSingle.Name = "mnuToolsMngtFilesSearchDoublesSingle";
            this.mnuToolsMngtFilesSearchDoublesSingle.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchDoublesSingle_Click);
            // 
            // MnuToolsSep3
            // 
            resources.ApplyResources(this.MnuToolsSep3, "MnuToolsSep3");
            this.MnuToolsSep3.Name = "MnuToolsSep3";
            // 
            // mnuToolsMngtFilesSearchSameSizeComparedToReference
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameSizeComparedToReference, "mnuToolsMngtFilesSearchSameSizeComparedToReference");
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference.Name = "mnuToolsMngtFilesSearchSameSizeComparedToReference";
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameSizeComparedToReference_Click);
            // 
            // mnuToolsMngtFilesSearchSameSizeInASingleDirectory
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory, "mnuToolsMngtFilesSearchSameSizeInASingleDirectory");
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory.Name = "mnuToolsMngtFilesSearchSameSizeInASingleDirectory";
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameSizeInASingleDirectory_Click);
            // 
            // MnuToolsSep4
            // 
            resources.ApplyResources(this.MnuToolsSep4, "MnuToolsSep4");
            this.MnuToolsSep4.Name = "MnuToolsSep4";
            // 
            // mnuToolsMngtFilesSearchSameNameComparedToReference
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameNameComparedToReference, "mnuToolsMngtFilesSearchSameNameComparedToReference");
            this.mnuToolsMngtFilesSearchSameNameComparedToReference.Name = "mnuToolsMngtFilesSearchSameNameComparedToReference";
            this.mnuToolsMngtFilesSearchSameNameComparedToReference.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameNameComparedToReference_Click);
            // 
            // mnuToolsMngtFilesSearchSameNameInASingleDirectory
            // 
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameNameInASingleDirectory, "mnuToolsMngtFilesSearchSameNameInASingleDirectory");
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory.Name = "mnuToolsMngtFilesSearchSameNameInASingleDirectory";
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameNameInASingleDirectory_Click);
            // 
            // mnuMidi
            // 
            resources.ApplyResources(this.mnuMidi, "mnuMidi");
            this.mnuMidi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMidiInputDevice,
            this.mnuMidiOutputDevice,
            this.MnuMidiSep1,
            this.MnuMidiExternal});
            this.mnuMidi.Name = "mnuMidi";
            // 
            // mnuMidiInputDevice
            // 
            resources.ApplyResources(this.mnuMidiInputDevice, "mnuMidiInputDevice");
            this.mnuMidiInputDevice.Name = "mnuMidiInputDevice";
            this.mnuMidiInputDevice.Click += new System.EventHandler(this.MnuMidiInputDevice_Click);
            // 
            // mnuMidiOutputDevice
            // 
            resources.ApplyResources(this.mnuMidiOutputDevice, "mnuMidiOutputDevice");
            this.mnuMidiOutputDevice.Name = "mnuMidiOutputDevice";
            this.mnuMidiOutputDevice.Click += new System.EventHandler(this.MnuMidiOutputDevice_Click);
            // 
            // MnuMidiSep1
            // 
            resources.ApplyResources(this.MnuMidiSep1, "MnuMidiSep1");
            this.MnuMidiSep1.Name = "MnuMidiSep1";
            // 
            // MnuMidiExternal
            // 
            resources.ApplyResources(this.MnuMidiExternal, "MnuMidiExternal");
            this.MnuMidiExternal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuMidiExternalPlay,
            this.MnuMidiExternalRecord});
            this.MnuMidiExternal.Name = "MnuMidiExternal";
            // 
            // MnuMidiExternalPlay
            // 
            resources.ApplyResources(this.MnuMidiExternalPlay, "MnuMidiExternalPlay");
            this.MnuMidiExternalPlay.Name = "MnuMidiExternalPlay";
            this.MnuMidiExternalPlay.Click += new System.EventHandler(this.MnuMidiExternalPlay_Click);
            // 
            // MnuMidiExternalRecord
            // 
            resources.ApplyResources(this.MnuMidiExternalRecord, "MnuMidiExternalRecord");
            this.MnuMidiExternalRecord.Name = "MnuMidiExternalRecord";
            this.MnuMidiExternalRecord.Click += new System.EventHandler(this.MnuMidiExternalRecord_Click);
            // 
            // mnuHelp
            // 
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuHelpCheckNewVersion,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            // 
            // MnuHelpCheckNewVersion
            // 
            resources.ApplyResources(this.MnuHelpCheckNewVersion, "MnuHelpCheckNewVersion");
            this.MnuHelpCheckNewVersion.Name = "MnuHelpCheckNewVersion";
            this.MnuHelpCheckNewVersion.Click += new System.EventHandler(this.MnuHelpCheckNewVersion_Click);
            // 
            // mnuHelpAbout
            // 
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Click += new System.EventHandler(this.MnuHelpAbout_Click);
            // 
            // sequence1
            // 
            this.sequence1.Copyright = null;
            this.sequence1.Denominator = 0;
            this.sequence1.Format = 1;
            this.sequence1.ITag = null;
            this.sequence1.KTag = null;
            this.sequence1.Log = null;
            this.sequence1.LTag = null;
            this.sequence1.Numerator = 0;
            this.sequence1.Orig_Tempo = 0;
            this.sequence1.OrigFormat = 0;
            this.sequence1.Quarternote = 0;
            this.sequence1.SplitHands = false;
            this.sequence1.TagAlbum = null;
            this.sequence1.TagArtist = null;
            this.sequence1.TagComment = null;
            this.sequence1.TagCopyright = null;
            this.sequence1.TagDate = null;
            this.sequence1.TagEditor = null;
            this.sequence1.TagEvaluation = null;
            this.sequence1.TagGenre = null;
            this.sequence1.TagTitle = null;
            this.sequence1.Tempo = 0;
            this.sequence1.TextEncoding = null;
            this.sequence1.Time = null;
            this.sequence1.TTag = null;
            this.sequence1.VTag = null;
            this.sequence1.WTag = null;
            // 
            // pnlFileInfos
            // 
            resources.ApplyResources(this.pnlFileInfos, "pnlFileInfos");
            this.pnlFileInfos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(244)))), ((int)(((byte)(255)))));
            this.pnlFileInfos.Controls.Add(this.lblLyrics);
            this.pnlFileInfos.Controls.Add(this.lblWtags);
            this.pnlFileInfos.Controls.Add(this.lblVtags);
            this.pnlFileInfos.Controls.Add(this.lblTtags);
            this.pnlFileInfos.Controls.Add(this.lblLtags);
            this.pnlFileInfos.Controls.Add(this.lblKtags);
            this.pnlFileInfos.Controls.Add(this.lblItags);
            this.pnlFileInfos.Controls.Add(this.lblDuration);
            this.pnlFileInfos.Controls.Add(this.lblFormat);
            this.pnlFileInfos.Controls.Add(this.lblTracks);
            this.pnlFileInfos.Name = "pnlFileInfos";
            // 
            // lblLyrics
            // 
            resources.ApplyResources(this.lblLyrics, "lblLyrics");
            this.lblLyrics.Name = "lblLyrics";
            // 
            // lblWtags
            // 
            resources.ApplyResources(this.lblWtags, "lblWtags");
            this.lblWtags.Name = "lblWtags";
            // 
            // lblVtags
            // 
            resources.ApplyResources(this.lblVtags, "lblVtags");
            this.lblVtags.Name = "lblVtags";
            // 
            // lblTtags
            // 
            resources.ApplyResources(this.lblTtags, "lblTtags");
            this.lblTtags.Name = "lblTtags";
            // 
            // lblLtags
            // 
            resources.ApplyResources(this.lblLtags, "lblLtags");
            this.lblLtags.Name = "lblLtags";
            // 
            // lblKtags
            // 
            resources.ApplyResources(this.lblKtags, "lblKtags");
            this.lblKtags.Name = "lblKtags";
            // 
            // lblItags
            // 
            resources.ApplyResources(this.lblItags, "lblItags");
            this.lblItags.Name = "lblItags";
            // 
            // lblDuration
            // 
            resources.ApplyResources(this.lblDuration, "lblDuration");
            this.lblDuration.Name = "lblDuration";
            // 
            // lblFormat
            // 
            resources.ApplyResources(this.lblFormat, "lblFormat");
            this.lblFormat.Name = "lblFormat";
            // 
            // lblTracks
            // 
            resources.ApplyResources(this.lblTracks, "lblTracks");
            this.lblTracks.Name = "lblTracks";
            // 
            // sideBarControl
            // 
            resources.ApplyResources(this.sideBarControl, "sideBarControl");
            this.sideBarControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.sideBarControl.Name = "sideBarControl";
            this.sideBarControl.TabStop = false;
            this.sideBarControl.ToolTipTextHome = "home";
            // 
            // connectedControl
            // 
            resources.ApplyResources(this.connectedControl, "connectedControl");
            this.connectedControl.Name = "connectedControl";
            // 
            // xplorerControl
            // 
            resources.ApplyResources(this.xplorerControl, "xplorerControl");
            this.xplorerControl.Name = "xplorerControl";
            this.xplorerControl.SplitterDistance = 0;
            // 
            // playlistsControl
            // 
            resources.ApplyResources(this.playlistsControl, "playlistsControl");
            this.playlistsControl.Name = "playlistsControl";
            this.playlistsControl.SelectedFileLength = "00:00";
            // 
            // searchControl
            // 
            resources.ApplyResources(this.searchControl, "searchControl");
            this.searchControl.Name = "searchControl";
            this.searchControl.SongRoot = "C:\\Users\\a453868\\Music";
            this.searchControl.SView = Karaboss.Search.SearchViewStyle.Author;
            // 
            // frmExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.connectedControl);
            this.Controls.Add(this.pnlFileInfos);
            this.Controls.Add(this.xplorerControl);
            this.Controls.Add(this.sideBarControl);
            this.Controls.Add(this.playlistsControl);
            this.Controls.Add(this.searchControl);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "frmExplorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmExplorer_FormClosing);
            this.Load += new System.EventHandler(this.FrmExplorer_Load);
            this.Resize += new System.EventHandler(this.FrmExplorer_Resize);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.pnlFileInfos.ResumeLayout(false);
            this.pnlFileInfos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;

        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditPlaylist;

        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOption;
        private System.Windows.Forms.ToolStripMenuItem mnuEditSearch;

        private System.Windows.Forms.ToolStripMenuItem mnuMidi;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiInputDevice;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiOutputDevice;

        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private Sanford.Multimedia.Midi.Sequence sequence1;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesDeleteEmptyDirs;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchDoubles;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchDoublesSingle;
        private System.Windows.Forms.ToolStripSeparator MnuToolsSep2;
        private System.Windows.Forms.ToolStripSeparator MnuToolsSep3;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchSameSizeComparedToReference;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchSameSizeInASingleDirectory;
        private System.Windows.Forms.ToolStripSeparator MnuToolsSep4;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchSameNameComparedToReference;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsMngtFilesSearchSameNameInASingleDirectory;
        private Search.SearchControl searchControl;
        private playlists.PlaylistsControl playlistsControl;
        private VBarControl.SideBarControl.SideBarControl sideBarControl;
        private xplorer.xplorerControl xplorerControl;
        private System.Windows.Forms.ToolStripStatusLabel tssLeft;
        private System.Windows.Forms.ToolStripStatusLabel tssMiddle;
        private System.Windows.Forms.ToolStripStatusLabel tssRight;
        private System.Windows.Forms.ToolStripMenuItem mnuEditExplore;
        private System.Windows.Forms.Panel pnlFileInfos;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblFormat;
        private System.Windows.Forms.Label lblTracks;
        private System.Windows.Forms.Label lblItags;
        private System.Windows.Forms.Label lblWtags;
        private System.Windows.Forms.Label lblVtags;
        private System.Windows.Forms.Label lblTtags;
        private System.Windows.Forms.Label lblLtags;
        private System.Windows.Forms.Label lblKtags;
        private System.Windows.Forms.Label lblLyrics;
        private Pages.ConnectedControl connectedControl;
        private System.Windows.Forms.ToolStripMenuItem MnuEditConnected;
        private System.Windows.Forms.ToolStripMenuItem MnuEditPianoTraining;
        private System.Windows.Forms.ToolStripMenuItem MnuHelpCheckNewVersion;
        private System.Windows.Forms.ToolStripSeparator MnuFileSep1;
        private System.Windows.Forms.ToolStripSeparator MnuMidiSep1;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternal;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternalPlay;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternalRecord;
        private System.Windows.Forms.ToolStripSeparator MnuToolsSep1;
        private System.Windows.Forms.ToolStripMenuItem MnuEditGuitarTraining;
    }
}