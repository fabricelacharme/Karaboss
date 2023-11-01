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
            this.mnuExplorer = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuFileRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditRenameAll = new System.Windows.Forms.ToolStripMenuItem();
            this.invertAuthorAndSongToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditReplaceAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplaySearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayExplore = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayPlaylist = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDisplayConnected = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDisplayPianoTraining = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuDisplayGuitarTraining = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
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
            this.mnuExplorer.SuspendLayout();
            this.pnlFileInfos.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssLeft,
            this.tssMiddle,
            this.tssRight});
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.Name = "statusBar";
            // 
            // tssLeft
            // 
            this.tssLeft.Name = "tssLeft";
            resources.ApplyResources(this.tssLeft, "tssLeft");
            // 
            // tssMiddle
            // 
            this.tssMiddle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tssMiddle.Name = "tssMiddle";
            resources.ApplyResources(this.tssMiddle, "tssMiddle");
            this.tssMiddle.Spring = true;
            // 
            // tssRight
            // 
            this.tssRight.Name = "tssRight";
            resources.ApplyResources(this.tssRight, "tssRight");
            // 
            // mnuExplorer
            // 
            this.mnuExplorer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuDisplay,
            this.mnuTools,
            this.mnuMidi,
            this.mnuHelp});
            resources.ApplyResources(this.mnuExplorer, "mnuExplorer");
            this.mnuExplorer.Name = "mnuExplorer";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.mnuFileEdit,
            this.MnuFileSep1,
            this.MnuFileRecentFiles,
            this.MnuFileSep2,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Name = "mnuFileNew";
            resources.ApplyResources(this.mnuFileNew, "mnuFileNew");
            this.mnuFileNew.Click += new System.EventHandler(this.MnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            resources.ApplyResources(this.mnuFileOpen, "mnuFileOpen");
            this.mnuFileOpen.Click += new System.EventHandler(this.MnuFileOpen_Click);
            // 
            // mnuFileEdit
            // 
            this.mnuFileEdit.Name = "mnuFileEdit";
            resources.ApplyResources(this.mnuFileEdit, "mnuFileEdit");
            this.mnuFileEdit.Click += new System.EventHandler(this.MnuFileEdit_Click);
            // 
            // MnuFileSep1
            // 
            this.MnuFileSep1.Name = "MnuFileSep1";
            resources.ApplyResources(this.MnuFileSep1, "MnuFileSep1");
            // 
            // MnuFileRecentFiles
            // 
            this.MnuFileRecentFiles.Name = "MnuFileRecentFiles";
            resources.ApplyResources(this.MnuFileRecentFiles, "MnuFileRecentFiles");
            // 
            // MnuFileSep2
            // 
            this.MnuFileSep2.Name = "MnuFileSep2";
            resources.ApplyResources(this.MnuFileSep2, "MnuFileSep2");
            // 
            // mnuFileQuit
            // 
            this.mnuFileQuit.Name = "mnuFileQuit";
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Click += new System.EventHandler(this.MnuFileQuit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditRename,
            this.mnuEditReplaceAll});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditRename
            // 
            this.mnuEditRename.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditRenameAll,
            this.invertAuthorAndSongToolStripMenuItem});
            this.mnuEditRename.Name = "mnuEditRename";
            resources.ApplyResources(this.mnuEditRename, "mnuEditRename");
            // 
            // mnuEditRenameAll
            // 
            this.mnuEditRenameAll.Name = "mnuEditRenameAll";
            resources.ApplyResources(this.mnuEditRenameAll, "mnuEditRenameAll");
            this.mnuEditRenameAll.Click += new System.EventHandler(this.mnuEditRenameAll_Click);
            // 
            // invertAuthorAndSongToolStripMenuItem
            // 
            this.invertAuthorAndSongToolStripMenuItem.Name = "invertAuthorAndSongToolStripMenuItem";
            resources.ApplyResources(this.invertAuthorAndSongToolStripMenuItem, "invertAuthorAndSongToolStripMenuItem");
            this.invertAuthorAndSongToolStripMenuItem.Click += new System.EventHandler(this.mnuInvertAuthorSong_Click);
            // 
            // mnuEditReplaceAll
            // 
            this.mnuEditReplaceAll.Name = "mnuEditReplaceAll";
            resources.ApplyResources(this.mnuEditReplaceAll, "mnuEditReplaceAll");
            this.mnuEditReplaceAll.Click += new System.EventHandler(this.mnuEditReplaceAll_Click);
            // 
            // mnuDisplay
            // 
            this.mnuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplaySearch,
            this.mnuDisplayExplore,
            this.mnuDisplayPlaylist,
            this.MnuDisplayConnected,
            this.MnuDisplayPianoTraining,
            this.MnuDisplayGuitarTraining});
            this.mnuDisplay.Name = "mnuDisplay";
            resources.ApplyResources(this.mnuDisplay, "mnuDisplay");
            // 
            // mnuDisplaySearch
            // 
            this.mnuDisplaySearch.Name = "mnuDisplaySearch";
            resources.ApplyResources(this.mnuDisplaySearch, "mnuDisplaySearch");
            this.mnuDisplaySearch.Click += new System.EventHandler(this.MnuDisplaySearch_Click);
            // 
            // mnuDisplayExplore
            // 
            this.mnuDisplayExplore.Name = "mnuDisplayExplore";
            resources.ApplyResources(this.mnuDisplayExplore, "mnuDisplayExplore");
            this.mnuDisplayExplore.Click += new System.EventHandler(this.MnuDisplayExplore_Click);
            // 
            // mnuDisplayPlaylist
            // 
            this.mnuDisplayPlaylist.Name = "mnuDisplayPlaylist";
            resources.ApplyResources(this.mnuDisplayPlaylist, "mnuDisplayPlaylist");
            this.mnuDisplayPlaylist.Click += new System.EventHandler(this.MnuDisplayPlaylist_Click);
            // 
            // MnuDisplayConnected
            // 
            this.MnuDisplayConnected.Name = "MnuDisplayConnected";
            resources.ApplyResources(this.MnuDisplayConnected, "MnuDisplayConnected");
            this.MnuDisplayConnected.Click += new System.EventHandler(this.MnuDisplayConnected_Click);
            // 
            // MnuDisplayPianoTraining
            // 
            this.MnuDisplayPianoTraining.Name = "MnuDisplayPianoTraining";
            resources.ApplyResources(this.MnuDisplayPianoTraining, "MnuDisplayPianoTraining");
            this.MnuDisplayPianoTraining.Click += new System.EventHandler(this.MnuDisplayPianoTraining_Click);
            // 
            // MnuDisplayGuitarTraining
            // 
            this.MnuDisplayGuitarTraining.Name = "MnuDisplayGuitarTraining";
            resources.ApplyResources(this.MnuDisplayGuitarTraining, "MnuDisplayGuitarTraining");
            this.MnuDisplayGuitarTraining.Click += new System.EventHandler(this.MnuDisplayGuitarTraining_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOption,
            this.MnuToolsSep1,
            this.mnuToolsMngtFiles,
            this.toolStripMenuItem1});
            this.mnuTools.Name = "mnuTools";
            resources.ApplyResources(this.mnuTools, "mnuTools");
            // 
            // mnuToolsOption
            // 
            this.mnuToolsOption.Name = "mnuToolsOption";
            resources.ApplyResources(this.mnuToolsOption, "mnuToolsOption");
            this.mnuToolsOption.Click += new System.EventHandler(this.MnuToolsOption_Click);
            // 
            // MnuToolsSep1
            // 
            this.MnuToolsSep1.Name = "MnuToolsSep1";
            resources.ApplyResources(this.MnuToolsSep1, "MnuToolsSep1");
            // 
            // mnuToolsMngtFiles
            // 
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
            resources.ApplyResources(this.mnuToolsMngtFiles, "mnuToolsMngtFiles");
            // 
            // mnuToolsMngtFilesDeleteEmptyDirs
            // 
            this.mnuToolsMngtFilesDeleteEmptyDirs.Name = "mnuToolsMngtFilesDeleteEmptyDirs";
            resources.ApplyResources(this.mnuToolsMngtFilesDeleteEmptyDirs, "mnuToolsMngtFilesDeleteEmptyDirs");
            this.mnuToolsMngtFilesDeleteEmptyDirs.Click += new System.EventHandler(this.MnuToolsMngtFilesDeleteEmptyDirs_Click);
            // 
            // MnuToolsSep2
            // 
            this.MnuToolsSep2.Name = "MnuToolsSep2";
            resources.ApplyResources(this.MnuToolsSep2, "MnuToolsSep2");
            // 
            // mnuToolsMngtFilesSearchDoubles
            // 
            this.mnuToolsMngtFilesSearchDoubles.Name = "mnuToolsMngtFilesSearchDoubles";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchDoubles, "mnuToolsMngtFilesSearchDoubles");
            this.mnuToolsMngtFilesSearchDoubles.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchDoubles_Click);
            // 
            // mnuToolsMngtFilesSearchDoublesSingle
            // 
            this.mnuToolsMngtFilesSearchDoublesSingle.Name = "mnuToolsMngtFilesSearchDoublesSingle";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchDoublesSingle, "mnuToolsMngtFilesSearchDoublesSingle");
            this.mnuToolsMngtFilesSearchDoublesSingle.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchDoublesSingle_Click);
            // 
            // MnuToolsSep3
            // 
            this.MnuToolsSep3.Name = "MnuToolsSep3";
            resources.ApplyResources(this.MnuToolsSep3, "MnuToolsSep3");
            // 
            // mnuToolsMngtFilesSearchSameSizeComparedToReference
            // 
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference.Name = "mnuToolsMngtFilesSearchSameSizeComparedToReference";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameSizeComparedToReference, "mnuToolsMngtFilesSearchSameSizeComparedToReference");
            this.mnuToolsMngtFilesSearchSameSizeComparedToReference.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameSizeComparedToReference_Click);
            // 
            // mnuToolsMngtFilesSearchSameSizeInASingleDirectory
            // 
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory.Name = "mnuToolsMngtFilesSearchSameSizeInASingleDirectory";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory, "mnuToolsMngtFilesSearchSameSizeInASingleDirectory");
            this.mnuToolsMngtFilesSearchSameSizeInASingleDirectory.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameSizeInASingleDirectory_Click);
            // 
            // MnuToolsSep4
            // 
            this.MnuToolsSep4.Name = "MnuToolsSep4";
            resources.ApplyResources(this.MnuToolsSep4, "MnuToolsSep4");
            // 
            // mnuToolsMngtFilesSearchSameNameComparedToReference
            // 
            this.mnuToolsMngtFilesSearchSameNameComparedToReference.Name = "mnuToolsMngtFilesSearchSameNameComparedToReference";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameNameComparedToReference, "mnuToolsMngtFilesSearchSameNameComparedToReference");
            this.mnuToolsMngtFilesSearchSameNameComparedToReference.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameNameComparedToReference_Click);
            // 
            // mnuToolsMngtFilesSearchSameNameInASingleDirectory
            // 
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory.Name = "mnuToolsMngtFilesSearchSameNameInASingleDirectory";
            resources.ApplyResources(this.mnuToolsMngtFilesSearchSameNameInASingleDirectory, "mnuToolsMngtFilesSearchSameNameInASingleDirectory");
            this.mnuToolsMngtFilesSearchSameNameInASingleDirectory.Click += new System.EventHandler(this.MnuToolsMngtFilesSearchSameNameInASingleDirectory_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // mnuMidi
            // 
            this.mnuMidi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMidiInputDevice,
            this.mnuMidiOutputDevice,
            this.MnuMidiSep1,
            this.MnuMidiExternal});
            this.mnuMidi.Name = "mnuMidi";
            resources.ApplyResources(this.mnuMidi, "mnuMidi");
            // 
            // mnuMidiInputDevice
            // 
            this.mnuMidiInputDevice.Name = "mnuMidiInputDevice";
            resources.ApplyResources(this.mnuMidiInputDevice, "mnuMidiInputDevice");
            this.mnuMidiInputDevice.Click += new System.EventHandler(this.MnuMidiInputDevice_Click);
            // 
            // mnuMidiOutputDevice
            // 
            this.mnuMidiOutputDevice.Name = "mnuMidiOutputDevice";
            resources.ApplyResources(this.mnuMidiOutputDevice, "mnuMidiOutputDevice");
            this.mnuMidiOutputDevice.Click += new System.EventHandler(this.MnuMidiOutputDevice_Click);
            // 
            // MnuMidiSep1
            // 
            this.MnuMidiSep1.Name = "MnuMidiSep1";
            resources.ApplyResources(this.MnuMidiSep1, "MnuMidiSep1");
            // 
            // MnuMidiExternal
            // 
            this.MnuMidiExternal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuMidiExternalPlay,
            this.MnuMidiExternalRecord});
            this.MnuMidiExternal.Name = "MnuMidiExternal";
            resources.ApplyResources(this.MnuMidiExternal, "MnuMidiExternal");
            // 
            // MnuMidiExternalPlay
            // 
            this.MnuMidiExternalPlay.Name = "MnuMidiExternalPlay";
            resources.ApplyResources(this.MnuMidiExternalPlay, "MnuMidiExternalPlay");
            this.MnuMidiExternalPlay.Click += new System.EventHandler(this.MnuMidiExternalPlay_Click);
            // 
            // MnuMidiExternalRecord
            // 
            this.MnuMidiExternalRecord.Name = "MnuMidiExternalRecord";
            resources.ApplyResources(this.MnuMidiExternalRecord, "MnuMidiExternalRecord");
            this.MnuMidiExternalRecord.Click += new System.EventHandler(this.MnuMidiExternalRecord_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuHelpCheckNewVersion,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // MnuHelpCheckNewVersion
            // 
            this.MnuHelpCheckNewVersion.Name = "MnuHelpCheckNewVersion";
            resources.ApplyResources(this.MnuHelpCheckNewVersion, "MnuHelpCheckNewVersion");
            this.MnuHelpCheckNewVersion.Click += new System.EventHandler(this.MnuHelpCheckNewVersion_Click);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Click += new System.EventHandler(this.MnuHelpAbout_Click);
            // 
            // sequence1
            // 
            this.sequence1.Copyright = null;
            this.sequence1.Denominator = 0;
            this.sequence1.Division = 24;
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
            resources.ApplyResources(this.pnlFileInfos, "pnlFileInfos");
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
            this.sideBarControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            resources.ApplyResources(this.sideBarControl, "sideBarControl");
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
            this.searchControl.SongRoot = "C:\\Users\\Fabrice\\OneDrive\\Music";
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
            this.Controls.Add(this.mnuExplorer);
            this.MainMenuStrip = this.mnuExplorer;
            this.Name = "frmExplorer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmExplorer_FormClosing);
            this.Load += new System.EventHandler(this.FrmExplorer_Load);
            this.Resize += new System.EventHandler(this.FrmExplorer_Resize);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.mnuExplorer.ResumeLayout(false);
            this.mnuExplorer.PerformLayout();
            this.pnlFileInfos.ResumeLayout(false);
            this.pnlFileInfos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.MenuStrip mnuExplorer;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;

        private System.Windows.Forms.ToolStripMenuItem mnuDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayPlaylist;

        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsOption;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplaySearch;

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
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayExplore;
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
        private System.Windows.Forms.ToolStripMenuItem MnuDisplayConnected;
        private System.Windows.Forms.ToolStripMenuItem MnuDisplayPianoTraining;
        private System.Windows.Forms.ToolStripMenuItem MnuHelpCheckNewVersion;
        private System.Windows.Forms.ToolStripSeparator MnuFileSep1;
        private System.Windows.Forms.ToolStripSeparator MnuMidiSep1;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternal;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternalPlay;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiExternalRecord;
        private System.Windows.Forms.ToolStripSeparator MnuToolsSep1;
        private System.Windows.Forms.ToolStripMenuItem MnuDisplayGuitarTraining;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem MnuFileRecentFiles;
        private System.Windows.Forms.ToolStripSeparator MnuFileSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditRename;
        private System.Windows.Forms.ToolStripMenuItem mnuEditReplaceAll;
        private System.Windows.Forms.ToolStripMenuItem mnuEditRenameAll;
        private System.Windows.Forms.ToolStripMenuItem invertAuthorAndSongToolStripMenuItem;
    }
}