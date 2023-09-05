using Sanford.Multimedia.Midi;

namespace Karaboss
{
    partial class frmPlayer
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
            if(disposing && (components != null))
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlayer));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileImportMidiFromText = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileExportMidiToText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAsPDF = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnufileProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuFileSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditAddLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MnuEditScore = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuEditEnterNotes = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayLyricsWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplaySequencer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplaySeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDisplayPianoRoll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDisplayZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayScrolling = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayScrollingHorz = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayScrollingVert = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidi = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidiNewTrack = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiImportTracks = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiImporTracksFromMidi = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuMidiImportTracksFromText = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMidiAddMeasures = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMIDIAddTimeLine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMidiModifyTempo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidiTimeSignature = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMidiRemoveFader = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMidiSplitHands = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAboutSong = new System.Windows.Forms.ToolStripMenuItem();
            this.openMidiFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblRecord = new System.Windows.Forms.Label();
            this.lblMuteMelody = new System.Windows.Forms.Label();
            this.btnMute1 = new Karaboss.Buttons.CheckButtonControl();
            this.btnTranspoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTranspoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.btnTempoMinus = new Karaboss.Buttons.MinusButtonControl();
            this.btnTempoPlus = new Karaboss.Buttons.PlusButtonControl();
            this.lblTranspoValue = new System.Windows.Forms.Label();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.lblOutputDevice = new System.Windows.Forms.Label();
            this.VuMasterPeakVolume = new VU_MeterLibrary.VuMeter();
            this.btnPrev = new Karaboss.NoSelectButton();
            this.btnNext = new Karaboss.NoSelectButton();
            this.btnStop = new Karaboss.NoSelectButton();
            this.btnPlay = new Karaboss.NoSelectButton();
            this.progressBarPlayer = new System.Windows.Forms.ProgressBar();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblBeat = new System.Windows.Forms.Label();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblTransp = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.lnlVol = new System.Windows.Forms.Label();
            this.positionHScrollBarNew = new ColorSlider.ColorSlider();
            this.lblChangesInfos = new System.Windows.Forms.Label();
            this.lblPlaylist = new System.Windows.Forms.Label();
            this.lblMainVolume = new System.Windows.Forms.Label();
            this.sldMainVolume = new ColorSlider.ColorSlider();
            this.btnStartRec = new System.Windows.Forms.Button();
            this.lblLyricsInfos = new System.Windows.Forms.Label();
            this.lblInfosF = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.timer4 = new System.Windows.Forms.Timer(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblBass = new System.Windows.Forms.Label();
            this.lblTreble = new System.Windows.Forms.Label();
            this.lblCurNoteInfo = new System.Windows.Forms.Label();
            this.lblDotted = new System.Windows.Forms.Label();
            this.lblEdit = new System.Windows.Forms.Label();
            this.lblSaisieNotes = new System.Windows.Forms.Label();
            this.lblBecarre = new System.Windows.Forms.Label();
            this.lblQuadrupleCrocheNote = new System.Windows.Forms.Label();
            this.lblTripleCrocheNote = new System.Windows.Forms.Label();
            this.lblDoubleCrocheNote = new System.Windows.Forms.Label();
            this.lblBemol = new System.Windows.Forms.Label();
            this.lblDiese = new System.Windows.Forms.Label();
            this.lblRondNote = new System.Windows.Forms.Label();
            this.lblCrocheNote = new System.Windows.Forms.Label();
            this.lblBlackNote = new System.Windows.Forms.Label();
            this.lblWhiteNote = new System.Windows.Forms.Label();
            this.lblGomme = new System.Windows.Forms.Label();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.TimeStartVLine = new System.Windows.Forms.Panel();
            this.TimeVLine = new System.Windows.Forms.Panel();
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.timer5 = new System.Windows.Forms.Timer(this.components);
            this.sequence1 = new Sanford.Multimedia.Midi.Sequence();
            this.sequencer1 = new Sanford.Multimedia.Midi.Sequencer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.MnuFileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip1.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuDisplay,
            this.mnuMidi,
            this.mnuHelp});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.MnuFileSeparator1,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.mnuSaveAsPDF,
            this.MnuFileSeparator2,
            this.MnuFileImportMidiFromText,
            this.MnuFileExportMidiToText,
            this.MnuFileSeparator3,
            this.mnufileProperties,
            this.MnuFileSeparator4,
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
            // MnuFileImportMidiFromText
            // 
            this.MnuFileImportMidiFromText.Name = "MnuFileImportMidiFromText";
            resources.ApplyResources(this.MnuFileImportMidiFromText, "MnuFileImportMidiFromText");
            this.MnuFileImportMidiFromText.Click += new System.EventHandler(this.MnuFileImportMidiFromText_Click);
            // 
            // MnuFileSeparator1
            // 
            this.MnuFileSeparator1.Name = "MnuFileSeparator1";
            resources.ApplyResources(this.MnuFileSeparator1, "MnuFileSeparator1");
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            resources.ApplyResources(this.mnuFileSave, "mnuFileSave");
            this.mnuFileSave.Click += new System.EventHandler(this.MnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            resources.ApplyResources(this.mnuFileSaveAs, "mnuFileSaveAs");
            this.mnuFileSaveAs.Click += new System.EventHandler(this.MnuFileSaveAs_Click);
            // 
            // MnuFileExportMidiToText
            // 
            this.MnuFileExportMidiToText.Name = "MnuFileExportMidiToText";
            resources.ApplyResources(this.MnuFileExportMidiToText, "MnuFileExportMidiToText");
            this.MnuFileExportMidiToText.Click += new System.EventHandler(this.MnuFileExportMidiToText_Click);
            // 
            // mnuSaveAsPDF
            // 
            this.mnuSaveAsPDF.Name = "mnuSaveAsPDF";
            resources.ApplyResources(this.mnuSaveAsPDF, "mnuSaveAsPDF");
            this.mnuSaveAsPDF.Click += new System.EventHandler(this.MnuSaveAsPDF_Click);
            // 
            // MnuFileSeparator2
            // 
            this.MnuFileSeparator2.Name = "MnuFileSeparator2";
            resources.ApplyResources(this.MnuFileSeparator2, "MnuFileSeparator2");
            // 
            // mnufileProperties
            // 
            this.mnufileProperties.Name = "mnufileProperties";
            resources.ApplyResources(this.mnufileProperties, "mnufileProperties");
            this.mnufileProperties.Click += new System.EventHandler(this.MnufileProperties_Click);
            // 
            // MnuFileSeparator4
            // 
            this.MnuFileSeparator4.Name = "MnuFileSeparator4";
            resources.ApplyResources(this.MnuFileSeparator4, "MnuFileSeparator4");
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
            this.mnuEditAddLyrics,
            this.mnuEditSeparator1,
            this.MnuEditScore,
            this.MnuEditEnterNotes});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditAddLyrics
            // 
            this.mnuEditAddLyrics.Name = "mnuEditAddLyrics";
            resources.ApplyResources(this.mnuEditAddLyrics, "mnuEditAddLyrics");
            this.mnuEditAddLyrics.Click += new System.EventHandler(this.MnuEditAddLyrics_Click);
            // 
            // mnuEditSeparator1
            // 
            this.mnuEditSeparator1.Name = "mnuEditSeparator1";
            resources.ApplyResources(this.mnuEditSeparator1, "mnuEditSeparator1");
            // 
            // MnuEditScore
            // 
            this.MnuEditScore.Name = "MnuEditScore";
            resources.ApplyResources(this.MnuEditScore, "MnuEditScore");
            this.MnuEditScore.Click += new System.EventHandler(this.MnuEditScore_Click);
            // 
            // MnuEditEnterNotes
            // 
            this.MnuEditEnterNotes.Name = "MnuEditEnterNotes";
            resources.ApplyResources(this.MnuEditEnterNotes, "MnuEditEnterNotes");
            this.MnuEditEnterNotes.Click += new System.EventHandler(this.MnuEditEnterNotes_Click);
            // 
            // mnuDisplay
            // 
            this.mnuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplayLyricsWindows,
            this.mnuDisplaySequencer,
            this.mnuDisplaySeparator1,
            this.mnuDisplayPianoRoll,
            this.toolStripSeparator5,
            this.mnuDisplayZoom,
            this.mnuDisplayScrolling});
            this.mnuDisplay.Name = "mnuDisplay";
            resources.ApplyResources(this.mnuDisplay, "mnuDisplay");
            // 
            // mnuDisplayLyricsWindows
            // 
            this.mnuDisplayLyricsWindows.Name = "mnuDisplayLyricsWindows";
            resources.ApplyResources(this.mnuDisplayLyricsWindows, "mnuDisplayLyricsWindows");
            this.mnuDisplayLyricsWindows.Click += new System.EventHandler(this.MnuDisplayLyricsWindows_Click);
            // 
            // mnuDisplaySequencer
            // 
            this.mnuDisplaySequencer.Name = "mnuDisplaySequencer";
            resources.ApplyResources(this.mnuDisplaySequencer, "mnuDisplaySequencer");
            this.mnuDisplaySequencer.Click += new System.EventHandler(this.MnuDisplaySequencer_Click);
            // 
            // mnuDisplaySeparator1
            // 
            this.mnuDisplaySeparator1.Name = "mnuDisplaySeparator1";
            resources.ApplyResources(this.mnuDisplaySeparator1, "mnuDisplaySeparator1");
            // 
            // mnuDisplayPianoRoll
            // 
            this.mnuDisplayPianoRoll.Name = "mnuDisplayPianoRoll";
            resources.ApplyResources(this.mnuDisplayPianoRoll, "mnuDisplayPianoRoll");
            this.mnuDisplayPianoRoll.Click += new System.EventHandler(this.MnuDisplayPianoRoll_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // mnuDisplayZoom
            // 
            this.mnuDisplayZoom.Name = "mnuDisplayZoom";
            resources.ApplyResources(this.mnuDisplayZoom, "mnuDisplayZoom");
            this.mnuDisplayZoom.Click += new System.EventHandler(this.MnuDisplayZoom_Click);
            // 
            // mnuDisplayScrolling
            // 
            this.mnuDisplayScrolling.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplayScrollingHorz,
            this.mnuDisplayScrollingVert});
            this.mnuDisplayScrolling.Name = "mnuDisplayScrolling";
            resources.ApplyResources(this.mnuDisplayScrolling, "mnuDisplayScrolling");
            // 
            // mnuDisplayScrollingHorz
            // 
            this.mnuDisplayScrollingHorz.Checked = true;
            this.mnuDisplayScrollingHorz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuDisplayScrollingHorz.Name = "mnuDisplayScrollingHorz";
            resources.ApplyResources(this.mnuDisplayScrollingHorz, "mnuDisplayScrollingHorz");
            this.mnuDisplayScrollingHorz.Click += new System.EventHandler(this.MnuDisplayScrollingHorz_Click);
            // 
            // mnuDisplayScrollingVert
            // 
            this.mnuDisplayScrollingVert.Name = "mnuDisplayScrollingVert";
            resources.ApplyResources(this.mnuDisplayScrollingVert, "mnuDisplayScrollingVert");
            this.mnuDisplayScrollingVert.Click += new System.EventHandler(this.MnuDisplayScrollingVert_Click);
            // 
            // mnuMidi
            // 
            this.mnuMidi.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMidiNewTrack,
            this.MnuMidiImportTracks,
            this.toolStripMenuItem1,
            this.mnuMidiAddMeasures,
            this.mnuMIDIAddTimeLine,
            this.toolStripSeparator3,
            this.mnuMidiModifyTempo,
            this.mnuMidiTimeSignature,
            this.mnuMidiRemoveFader,
            this.toolStripSeparator4,
            this.mnuMidiSplitHands});
            this.mnuMidi.Name = "mnuMidi";
            resources.ApplyResources(this.mnuMidi, "mnuMidi");
            // 
            // mnuMidiNewTrack
            // 
            this.mnuMidiNewTrack.Name = "mnuMidiNewTrack";
            resources.ApplyResources(this.mnuMidiNewTrack, "mnuMidiNewTrack");
            this.mnuMidiNewTrack.Click += new System.EventHandler(this.MnuMidiNewTrack_Click);
            // 
            // MnuMidiImportTracks
            // 
            this.MnuMidiImportTracks.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnuMidiImporTracksFromMidi,
            this.MnuMidiImportTracksFromText});
            this.MnuMidiImportTracks.Name = "MnuMidiImportTracks";
            resources.ApplyResources(this.MnuMidiImportTracks, "MnuMidiImportTracks");
            // 
            // MnuMidiImporTracksFromMidi
            // 
            this.MnuMidiImporTracksFromMidi.Name = "MnuMidiImporTracksFromMidi";
            resources.ApplyResources(this.MnuMidiImporTracksFromMidi, "MnuMidiImporTracksFromMidi");
            this.MnuMidiImporTracksFromMidi.Click += new System.EventHandler(this.MnuMidiImporTracksFromMidi_Click);
            // 
            // MnuMidiImportTracksFromText
            // 
            this.MnuMidiImportTracksFromText.Name = "MnuMidiImportTracksFromText";
            resources.ApplyResources(this.MnuMidiImportTracksFromText, "MnuMidiImportTracksFromText");
            this.MnuMidiImportTracksFromText.Click += new System.EventHandler(this.MnuMidiImportTracksFromText_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // mnuMidiAddMeasures
            // 
            this.mnuMidiAddMeasures.Name = "mnuMidiAddMeasures";
            resources.ApplyResources(this.mnuMidiAddMeasures, "mnuMidiAddMeasures");
            this.mnuMidiAddMeasures.Click += new System.EventHandler(this.MnuMidiAddMeasures_Click);
            // 
            // mnuMIDIAddTimeLine
            // 
            this.mnuMIDIAddTimeLine.Name = "mnuMIDIAddTimeLine";
            resources.ApplyResources(this.mnuMIDIAddTimeLine, "mnuMIDIAddTimeLine");
            this.mnuMIDIAddTimeLine.Click += new System.EventHandler(this.MnuMIDIAddTimeLine_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // mnuMidiModifyTempo
            // 
            this.mnuMidiModifyTempo.Name = "mnuMidiModifyTempo";
            resources.ApplyResources(this.mnuMidiModifyTempo, "mnuMidiModifyTempo");
            this.mnuMidiModifyTempo.Click += new System.EventHandler(this.MnuMidiModifyTempo_Click);
            // 
            // mnuMidiTimeSignature
            // 
            this.mnuMidiTimeSignature.Name = "mnuMidiTimeSignature";
            resources.ApplyResources(this.mnuMidiTimeSignature, "mnuMidiTimeSignature");
            this.mnuMidiTimeSignature.Click += new System.EventHandler(this.MnuMidiTimeSignature_Click);
            // 
            // mnuMidiRemoveFader
            // 
            this.mnuMidiRemoveFader.Name = "mnuMidiRemoveFader";
            resources.ApplyResources(this.mnuMidiRemoveFader, "mnuMidiRemoveFader");
            this.mnuMidiRemoveFader.Click += new System.EventHandler(this.MnuMidiRemoveFader_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // mnuMidiSplitHands
            // 
            this.mnuMidiSplitHands.Name = "mnuMidiSplitHands";
            resources.ApplyResources(this.mnuMidiSplitHands, "mnuMidiSplitHands");
            this.mnuMidiSplitHands.Click += new System.EventHandler(this.MnuMidiSplitHands_Click);
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
            this.mnuHelpAbout.Click += new System.EventHandler(this.MnuHelpAbout_Click);
            // 
            // mnuHelpAboutSong
            // 
            this.mnuHelpAboutSong.Name = "mnuHelpAboutSong";
            resources.ApplyResources(this.mnuHelpAboutSong, "mnuHelpAboutSong");
            this.mnuHelpAboutSong.Click += new System.EventHandler(this.MnuHelpAboutSong_Click);
            // 
            // openMidiFileDialog
            // 
            this.openMidiFileDialog.DefaultExt = "kar";
            resources.ApplyResources(this.openMidiFileDialog, "openMidiFileDialog");
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // lblPercent
            // 
            resources.ApplyResources(this.lblPercent, "lblPercent");
            this.lblPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblPercent.ForeColor = System.Drawing.Color.White;
            this.lblPercent.Name = "lblPercent";
            // 
            // lblDuration
            // 
            this.lblDuration.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblDuration, "lblDuration");
            this.lblDuration.ForeColor = System.Drawing.Color.White;
            this.lblDuration.Name = "lblDuration";
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlBottom.Controls.Add(this.lblRecord);
            this.pnlBottom.Controls.Add(this.lblMuteMelody);
            this.pnlBottom.Controls.Add(this.btnMute1);
            this.pnlBottom.Controls.Add(this.btnTranspoMinus);
            this.pnlBottom.Controls.Add(this.btnTranspoPlus);
            this.pnlBottom.Controls.Add(this.btnTempoMinus);
            this.pnlBottom.Controls.Add(this.btnTempoPlus);
            this.pnlBottom.Controls.Add(this.lblTranspoValue);
            this.pnlBottom.Controls.Add(this.lblTempoValue);
            this.pnlBottom.Controls.Add(this.lblOutputDevice);
            this.pnlBottom.Controls.Add(this.VuMasterPeakVolume);
            this.pnlBottom.Controls.Add(this.btnPrev);
            this.pnlBottom.Controls.Add(this.btnNext);
            this.pnlBottom.Controls.Add(this.btnStop);
            this.pnlBottom.Controls.Add(this.btnPlay);
            this.pnlBottom.Controls.Add(this.progressBarPlayer);
            this.pnlBottom.Controls.Add(this.pnlDisplay);
            this.pnlBottom.Controls.Add(this.lblTransp);
            this.pnlBottom.Controls.Add(this.lblTemp);
            this.pnlBottom.Controls.Add(this.lnlVol);
            this.pnlBottom.Controls.Add(this.positionHScrollBarNew);
            this.pnlBottom.Controls.Add(this.lblChangesInfos);
            this.pnlBottom.Controls.Add(this.lblPlaylist);
            this.pnlBottom.Controls.Add(this.lblMainVolume);
            this.pnlBottom.Controls.Add(this.sldMainVolume);
            this.pnlBottom.Controls.Add(this.btnStartRec);
            this.pnlBottom.Controls.Add(this.lblLyricsInfos);
            this.pnlBottom.Controls.Add(this.lblInfosF);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
            // 
            // lblRecord
            // 
            resources.ApplyResources(this.lblRecord, "lblRecord");
            this.lblRecord.ForeColor = System.Drawing.Color.White;
            this.lblRecord.Name = "lblRecord";
            // 
            // lblMuteMelody
            // 
            resources.ApplyResources(this.lblMuteMelody, "lblMuteMelody");
            this.lblMuteMelody.ForeColor = System.Drawing.Color.White;
            this.lblMuteMelody.Name = "lblMuteMelody";
            // 
            // btnMute1
            // 
            this.btnMute1.Checked = true;
            resources.ApplyResources(this.btnMute1, "btnMute1");
            this.btnMute1.Name = "btnMute1";
            this.btnMute1.Click += new System.EventHandler(this.btnMute1_Click);
            // 
            // btnTranspoMinus
            // 
            resources.ApplyResources(this.btnTranspoMinus, "btnTranspoMinus");
            this.btnTranspoMinus.Name = "btnTranspoMinus";
            this.btnTranspoMinus.Click += new System.EventHandler(this.btnTranspoMinus_Click);
            // 
            // btnTranspoPlus
            // 
            resources.ApplyResources(this.btnTranspoPlus, "btnTranspoPlus");
            this.btnTranspoPlus.Name = "btnTranspoPlus";
            this.btnTranspoPlus.Click += new System.EventHandler(this.btnTranspoPlus_Click);
            // 
            // btnTempoMinus
            // 
            resources.ApplyResources(this.btnTempoMinus, "btnTempoMinus");
            this.btnTempoMinus.Name = "btnTempoMinus";
            this.btnTempoMinus.Click += new System.EventHandler(this.btnTempoMinus_Click);
            // 
            // btnTempoPlus
            // 
            resources.ApplyResources(this.btnTempoPlus, "btnTempoPlus");
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // lblTranspoValue
            // 
            this.lblTranspoValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblTranspoValue, "lblTranspoValue");
            this.lblTranspoValue.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblTranspoValue.Name = "lblTranspoValue";
            // 
            // lblTempoValue
            // 
            this.lblTempoValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblTempoValue, "lblTempoValue");
            this.lblTempoValue.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblTempoValue.Name = "lblTempoValue";
            // 
            // lblOutputDevice
            // 
            resources.ApplyResources(this.lblOutputDevice, "lblOutputDevice");
            this.lblOutputDevice.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblOutputDevice.Name = "lblOutputDevice";
            // 
            // VuMasterPeakVolume
            // 
            this.VuMasterPeakVolume.AnalogMeter = false;
            this.VuMasterPeakVolume.DialBackground = System.Drawing.Color.White;
            this.VuMasterPeakVolume.DialTextNegative = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.DialTextPositive = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.DialTextZero = System.Drawing.Color.DarkGreen;
            this.VuMasterPeakVolume.Led1ColorOff = System.Drawing.Color.DarkGreen;
            this.VuMasterPeakVolume.Led1ColorOn = System.Drawing.Color.LimeGreen;
            this.VuMasterPeakVolume.Led1Count = 6;
            this.VuMasterPeakVolume.Led2ColorOff = System.Drawing.Color.Olive;
            this.VuMasterPeakVolume.Led2ColorOn = System.Drawing.Color.Yellow;
            this.VuMasterPeakVolume.Led2Count = 6;
            this.VuMasterPeakVolume.Led3ColorOff = System.Drawing.Color.Maroon;
            this.VuMasterPeakVolume.Led3ColorOn = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.Led3Count = 4;
            this.VuMasterPeakVolume.LedSize = new System.Drawing.Size(6, 14);
            this.VuMasterPeakVolume.LedSpace = 3;
            this.VuMasterPeakVolume.Level = 0;
            this.VuMasterPeakVolume.LevelMax = 127;
            resources.ApplyResources(this.VuMasterPeakVolume, "VuMasterPeakVolume");
            this.VuMasterPeakVolume.MeterScale = VU_MeterLibrary.MeterScale.Log10;
            this.VuMasterPeakVolume.Name = "VuMasterPeakVolume";
            this.VuMasterPeakVolume.NeedleColor = System.Drawing.Color.Black;
            this.VuMasterPeakVolume.PeakHold = true;
            this.VuMasterPeakVolume.Peakms = 1000;
            this.VuMasterPeakVolume.PeakNeedleColor = System.Drawing.Color.Red;
            this.VuMasterPeakVolume.ShowDialOnly = false;
            this.VuMasterPeakVolume.ShowLedPeak = false;
            this.VuMasterPeakVolume.ShowTextInDial = false;
            this.VuMasterPeakVolume.TextInDial = new string[] {
        "-40",
        "-20",
        "-10",
        "-5",
        "0",
        "+6"};
            this.VuMasterPeakVolume.UseLedLight = false;
            this.VuMasterPeakVolume.VerticalBar = true;
            this.VuMasterPeakVolume.VuText = "VU";
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPrev.FlatAppearance.BorderSize = 0;
            this.btnPrev.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPrev.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnPrev, "btnPrev");
            this.btnPrev.Image = global::Karaboss.Properties.Resources.btn_black_prev;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.TabStop = false;
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.BtnPrev_Click);
            this.btnPrev.MouseLeave += new System.EventHandler(this.BtnPrev_MouseLeave);
            this.btnPrev.MouseHover += new System.EventHandler(this.BtnPrev_MouseHover);
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Image = global::Karaboss.Properties.Resources.btn_black_next;
            this.btnNext.Name = "btnNext";
            this.btnNext.TabStop = false;
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            this.btnNext.MouseLeave += new System.EventHandler(this.BtnNext_MouseLeave);
            this.btnNext.MouseHover += new System.EventHandler(this.BtnNext_MouseHover);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnStop, "btnStop");
            this.btnStop.Image = global::Karaboss.Properties.Resources.btn_black_stop;
            this.btnStop.Name = "btnStop";
            this.btnStop.TabStop = false;
            this.toolTip1.SetToolTip(this.btnStop, resources.GetString("btnStop.ToolTip"));
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            this.btnStop.MouseLeave += new System.EventHandler(this.BtnStop_MouseLeave);
            this.btnStop.MouseHover += new System.EventHandler(this.BtnStop_MouseHover);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Image = global::Karaboss.Properties.Resources.btn_black_play;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.TabStop = false;
            this.toolTip1.SetToolTip(this.btnPlay, resources.GetString("btnPlay.ToolTip"));
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            this.btnPlay.MouseLeave += new System.EventHandler(this.BtnPlay_MouseLeave);
            this.btnPlay.MouseHover += new System.EventHandler(this.BtnPlay_MouseHover);
            // 
            // progressBarPlayer
            // 
            resources.ApplyResources(this.progressBarPlayer, "progressBarPlayer");
            this.progressBarPlayer.Name = "progressBarPlayer";
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.Controls.Add(this.lblStatus);
            this.pnlDisplay.Controls.Add(this.lblBeat);
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblDuration);
            resources.ApplyResources(this.pnlDisplay, "pnlDisplay");
            this.pnlDisplay.Name = "pnlDisplay";
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Name = "lblStatus";
            // 
            // lblBeat
            // 
            this.lblBeat.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblBeat, "lblBeat");
            this.lblBeat.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblBeat.Name = "lblBeat";
            // 
            // lblElapsed
            // 
            this.lblElapsed.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblElapsed, "lblElapsed");
            this.lblElapsed.ForeColor = System.Drawing.Color.White;
            this.lblElapsed.Name = "lblElapsed";
            // 
            // lblTransp
            // 
            resources.ApplyResources(this.lblTransp, "lblTransp");
            this.lblTransp.ForeColor = System.Drawing.Color.White;
            this.lblTransp.Name = "lblTransp";
            // 
            // lblTemp
            // 
            resources.ApplyResources(this.lblTemp, "lblTemp");
            this.lblTemp.ForeColor = System.Drawing.Color.White;
            this.lblTemp.Name = "lblTemp";
            // 
            // lnlVol
            // 
            resources.ApplyResources(this.lnlVol, "lnlVol");
            this.lnlVol.ForeColor = System.Drawing.Color.White;
            this.lnlVol.Name = "lnlVol";
            // 
            // positionHScrollBarNew
            // 
            this.positionHScrollBarNew.BackColor = System.Drawing.Color.Transparent;
            this.positionHScrollBarNew.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.positionHScrollBarNew.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.positionHScrollBarNew.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.positionHScrollBarNew.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBarNew.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.positionHScrollBarNew.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            resources.ApplyResources(this.positionHScrollBarNew, "positionHScrollBarNew");
            this.positionHScrollBarNew.ForeColor = System.Drawing.Color.White;
            this.positionHScrollBarNew.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBarNew.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.positionHScrollBarNew.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBarNew.Name = "positionHScrollBarNew";
            this.positionHScrollBarNew.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.positionHScrollBarNew.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBarNew.ShowDivisionsText = true;
            this.positionHScrollBarNew.ShowSmallScale = false;
            this.positionHScrollBarNew.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.positionHScrollBarNew.TabStop = false;
            this.positionHScrollBarNew.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.positionHScrollBarNew.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBarNew.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBarNew.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBarNew.ThumbSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBarNew.TickAdd = 0F;
            this.positionHScrollBarNew.TickColor = System.Drawing.Color.White;
            this.positionHScrollBarNew.TickDivide = 0F;
            this.positionHScrollBarNew.TickStyle = System.Windows.Forms.TickStyle.None;
            this.positionHScrollBarNew.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBarNew.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PositionHScrollBarNew_Scroll);
            // 
            // lblChangesInfos
            // 
            this.lblChangesInfos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblChangesInfos, "lblChangesInfos");
            this.lblChangesInfos.Name = "lblChangesInfos";
            // 
            // lblPlaylist
            // 
            resources.ApplyResources(this.lblPlaylist, "lblPlaylist");
            this.lblPlaylist.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblPlaylist.Name = "lblPlaylist";
            // 
            // lblMainVolume
            // 
            resources.ApplyResources(this.lblMainVolume, "lblMainVolume");
            this.lblMainVolume.ForeColor = System.Drawing.Color.White;
            this.lblMainVolume.Name = "lblMainVolume";
            // 
            // sldMainVolume
            // 
            this.sldMainVolume.BackColor = System.Drawing.Color.Transparent;
            this.sldMainVolume.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sldMainVolume.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sldMainVolume.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sldMainVolume.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.sldMainVolume.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            resources.ApplyResources(this.sldMainVolume, "sldMainVolume");
            this.sldMainVolume.ForeColor = System.Drawing.Color.White;
            this.sldMainVolume.LargeChange = new decimal(new int[] {
            26,
            0,
            0,
            0});
            this.sldMainVolume.Maximum = new decimal(new int[] {
            130,
            0,
            0,
            0});
            this.sldMainVolume.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.sldMainVolume.Name = "sldMainVolume";
            this.sldMainVolume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.sldMainVolume.ScaleDivisions = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.sldMainVolume.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sldMainVolume.ShowDivisionsText = false;
            this.sldMainVolume.ShowSmallScale = false;
            this.sldMainVolume.SmallChange = new decimal(new int[] {
            13,
            0,
            0,
            0});
            this.sldMainVolume.TabStop = false;
            this.sldMainVolume.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.sldMainVolume.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.sldMainVolume.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sldMainVolume.ThumbSize = new System.Drawing.Size(16, 16);
            this.sldMainVolume.TickAdd = 0F;
            this.sldMainVolume.TickColor = System.Drawing.Color.White;
            this.sldMainVolume.TickDivide = 0F;
            this.sldMainVolume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.sldMainVolume.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.sldMainVolume.ValueChanged += new System.EventHandler(this.SldMainVolume_ValueChanged);
            // 
            // btnStartRec
            // 
            this.btnStartRec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.btnStartRec.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.btnStartRec, "btnStartRec");
            this.btnStartRec.Name = "btnStartRec";
            this.btnStartRec.TabStop = false;
            this.btnStartRec.UseVisualStyleBackColor = false;
            this.btnStartRec.Click += new System.EventHandler(this.BtnStartRec_Click);
            // 
            // lblLyricsInfos
            // 
            this.lblLyricsInfos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblLyricsInfos, "lblLyricsInfos");
            this.lblLyricsInfos.Name = "lblLyricsInfos";
            // 
            // lblInfosF
            // 
            this.lblInfosF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.lblInfosF, "lblInfosF");
            this.lblInfosF.Name = "lblInfosF";
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.Timer3_Tick);
            // 
            // timer4
            // 
            this.timer4.Tick += new System.EventHandler(this.Timer4_Tick);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.lblHelp);
            this.pnlTop.Controls.Add(this.lblBass);
            this.pnlTop.Controls.Add(this.lblTreble);
            this.pnlTop.Controls.Add(this.lblCurNoteInfo);
            this.pnlTop.Controls.Add(this.lblDotted);
            this.pnlTop.Controls.Add(this.lblEdit);
            this.pnlTop.Controls.Add(this.lblSaisieNotes);
            this.pnlTop.Controls.Add(this.lblBecarre);
            this.pnlTop.Controls.Add(this.lblQuadrupleCrocheNote);
            this.pnlTop.Controls.Add(this.lblTripleCrocheNote);
            this.pnlTop.Controls.Add(this.lblDoubleCrocheNote);
            this.pnlTop.Controls.Add(this.lblBemol);
            this.pnlTop.Controls.Add(this.lblDiese);
            this.pnlTop.Controls.Add(this.lblRondNote);
            this.pnlTop.Controls.Add(this.lblCrocheNote);
            this.pnlTop.Controls.Add(this.lblBlackNote);
            this.pnlTop.Controls.Add(this.lblWhiteNote);
            this.pnlTop.Controls.Add(this.lblGomme);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // lblHelp
            // 
            this.lblHelp.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblHelp, "lblHelp");
            this.lblHelp.Name = "lblHelp";
            // 
            // lblBass
            // 
            this.lblBass.BackColor = System.Drawing.Color.White;
            this.lblBass.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBass.Image = global::Karaboss.Properties.Resources.bass30;
            resources.ApplyResources(this.lblBass, "lblBass");
            this.lblBass.Name = "lblBass";
            this.lblBass.Click += new System.EventHandler(this.LblBass_Click);
            // 
            // lblTreble
            // 
            this.lblTreble.BackColor = System.Drawing.Color.White;
            this.lblTreble.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTreble.Image = global::Karaboss.Properties.Resources.treble30;
            resources.ApplyResources(this.lblTreble, "lblTreble");
            this.lblTreble.Name = "lblTreble";
            this.lblTreble.Click += new System.EventHandler(this.LblTreble_Click);
            // 
            // lblCurNoteInfo
            // 
            resources.ApplyResources(this.lblCurNoteInfo, "lblCurNoteInfo");
            this.lblCurNoteInfo.BackColor = System.Drawing.Color.White;
            this.lblCurNoteInfo.Name = "lblCurNoteInfo";
            // 
            // lblDotted
            // 
            this.lblDotted.BackColor = System.Drawing.Color.White;
            this.lblDotted.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblDotted, "lblDotted");
            this.lblDotted.Name = "lblDotted";
            this.lblDotted.Click += new System.EventHandler(this.LblDotted_Click);
            // 
            // lblEdit
            // 
            this.lblEdit.BackColor = System.Drawing.Color.White;
            this.lblEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblEdit, "lblEdit");
            this.lblEdit.Name = "lblEdit";
            this.toolTip1.SetToolTip(this.lblEdit, resources.GetString("lblEdit.ToolTip"));
            this.lblEdit.Click += new System.EventHandler(this.LblEdit_Click);
            // 
            // lblSaisieNotes
            // 
            this.lblSaisieNotes.BackColor = System.Drawing.Color.White;
            this.lblSaisieNotes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.lblSaisieNotes, "lblSaisieNotes");
            this.lblSaisieNotes.Name = "lblSaisieNotes";
            this.toolTip1.SetToolTip(this.lblSaisieNotes, resources.GetString("lblSaisieNotes.ToolTip"));
            this.lblSaisieNotes.Click += new System.EventHandler(this.LblSaisieNotes_Click);
            // 
            // lblBecarre
            // 
            this.lblBecarre.BackColor = System.Drawing.Color.White;
            this.lblBecarre.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBecarre.Image = global::Karaboss.Properties.Resources.becarre;
            resources.ApplyResources(this.lblBecarre, "lblBecarre");
            this.lblBecarre.Name = "lblBecarre";
            this.lblBecarre.Click += new System.EventHandler(this.LblBecarre_Click);
            // 
            // lblQuadrupleCrocheNote
            // 
            this.lblQuadrupleCrocheNote.BackColor = System.Drawing.Color.White;
            this.lblQuadrupleCrocheNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblQuadrupleCrocheNote.Image = global::Karaboss.Properties.Resources.note64;
            resources.ApplyResources(this.lblQuadrupleCrocheNote, "lblQuadrupleCrocheNote");
            this.lblQuadrupleCrocheNote.Name = "lblQuadrupleCrocheNote";
            this.lblQuadrupleCrocheNote.Click += new System.EventHandler(this.LblQuadrupleCrocheNote_Click);
            // 
            // lblTripleCrocheNote
            // 
            this.lblTripleCrocheNote.BackColor = System.Drawing.Color.White;
            this.lblTripleCrocheNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblTripleCrocheNote.Image = global::Karaboss.Properties.Resources.note32;
            resources.ApplyResources(this.lblTripleCrocheNote, "lblTripleCrocheNote");
            this.lblTripleCrocheNote.Name = "lblTripleCrocheNote";
            this.lblTripleCrocheNote.Click += new System.EventHandler(this.LblTripleCrocheNote_Click);
            // 
            // lblDoubleCrocheNote
            // 
            this.lblDoubleCrocheNote.BackColor = System.Drawing.Color.White;
            this.lblDoubleCrocheNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDoubleCrocheNote.Image = global::Karaboss.Properties.Resources.note16;
            resources.ApplyResources(this.lblDoubleCrocheNote, "lblDoubleCrocheNote");
            this.lblDoubleCrocheNote.Name = "lblDoubleCrocheNote";
            this.lblDoubleCrocheNote.Click += new System.EventHandler(this.LblDoubleCrocheNote_Click);
            // 
            // lblBemol
            // 
            this.lblBemol.BackColor = System.Drawing.Color.White;
            this.lblBemol.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBemol.Image = global::Karaboss.Properties.Resources.bemol;
            resources.ApplyResources(this.lblBemol, "lblBemol");
            this.lblBemol.Name = "lblBemol";
            this.lblBemol.Click += new System.EventHandler(this.LblBemol_Click);
            // 
            // lblDiese
            // 
            this.lblDiese.BackColor = System.Drawing.Color.White;
            this.lblDiese.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblDiese.Image = global::Karaboss.Properties.Resources.diese;
            resources.ApplyResources(this.lblDiese, "lblDiese");
            this.lblDiese.Name = "lblDiese";
            this.lblDiese.Click += new System.EventHandler(this.LblDiese_Click);
            // 
            // lblRondNote
            // 
            this.lblRondNote.BackColor = System.Drawing.Color.White;
            this.lblRondNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblRondNote.Image = global::Karaboss.Properties.Resources.note1;
            resources.ApplyResources(this.lblRondNote, "lblRondNote");
            this.lblRondNote.Name = "lblRondNote";
            this.lblRondNote.Click += new System.EventHandler(this.LblRondNote_Click);
            // 
            // lblCrocheNote
            // 
            this.lblCrocheNote.BackColor = System.Drawing.Color.White;
            this.lblCrocheNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCrocheNote.Image = global::Karaboss.Properties.Resources.note8;
            resources.ApplyResources(this.lblCrocheNote, "lblCrocheNote");
            this.lblCrocheNote.Name = "lblCrocheNote";
            this.lblCrocheNote.Click += new System.EventHandler(this.LblCrocheNote_Click);
            // 
            // lblBlackNote
            // 
            this.lblBlackNote.BackColor = System.Drawing.Color.White;
            this.lblBlackNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblBlackNote.Image = global::Karaboss.Properties.Resources.note4;
            resources.ApplyResources(this.lblBlackNote, "lblBlackNote");
            this.lblBlackNote.Name = "lblBlackNote";
            this.lblBlackNote.Click += new System.EventHandler(this.LblBlackNote_Click);
            // 
            // lblWhiteNote
            // 
            this.lblWhiteNote.BackColor = System.Drawing.Color.White;
            this.lblWhiteNote.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblWhiteNote.Image = global::Karaboss.Properties.Resources.note2;
            resources.ApplyResources(this.lblWhiteNote, "lblWhiteNote");
            this.lblWhiteNote.Name = "lblWhiteNote";
            this.lblWhiteNote.Click += new System.EventHandler(this.LblWhiteNote_Click);
            // 
            // lblGomme
            // 
            this.lblGomme.BackColor = System.Drawing.Color.White;
            this.lblGomme.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblGomme.Image = global::Karaboss.Properties.Resources.erase;
            resources.ApplyResources(this.lblGomme, "lblGomme");
            this.lblGomme.Name = "lblGomme";
            this.toolTip1.SetToolTip(this.lblGomme, resources.GetString("lblGomme.ToolTip"));
            this.lblGomme.Click += new System.EventHandler(this.LblGomme_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlMiddle.Controls.Add(this.TimeStartVLine);
            this.pnlMiddle.Controls.Add(this.TimeVLine);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
            // 
            // TimeStartVLine
            // 
            this.TimeStartVLine.BackColor = System.Drawing.Color.Blue;
            resources.ApplyResources(this.TimeStartVLine, "TimeStartVLine");
            this.TimeStartVLine.Name = "TimeStartVLine";
            // 
            // TimeVLine
            // 
            this.TimeVLine.BackColor = System.Drawing.Color.Red;
            resources.ApplyResources(this.TimeVLine, "TimeVLine");
            this.TimeVLine.Name = "TimeVLine";
            // 
            // timer5
            // 
            this.timer5.Tick += new System.EventHandler(this.Timer5_Tick);
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
            // sequencer1
            // 
            this.sequencer1.Position = 0;
            this.sequencer1.Sequence = this.sequence1;
            this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
            this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);
            this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
            this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
            this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);
            // 
            // MnuFileSeparator3
            // 
            this.MnuFileSeparator3.Name = "MnuFileSeparator3";
            resources.ApplyResources(this.MnuFileSeparator3, "MnuFileSeparator3");
            // 
            // frmPlayer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPlayer_FormClosing);
            this.Load += new System.EventHandler(this.FrmPlayer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmPlayer_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmPlayer_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator MnuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.OpenFileDialog openMidiFileDialog;
        private System.Windows.Forms.ToolStripMenuItem mnuMidi;
        private Sequence sequence1;
        private Sequencer sequencer1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Timer timer4;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblBeat;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblMainVolume;
        private System.Windows.Forms.Label lblInfosF;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.SaveFileDialog saveMidiFileDialog;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditAddLyrics;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAboutSong;
        private System.Windows.Forms.Timer timer5;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiNewTrack;
        private System.Windows.Forms.Label lblCrocheNote;
        private System.Windows.Forms.Label lblBlackNote;
        private System.Windows.Forms.Label lblWhiteNote;
        private System.Windows.Forms.Label lblGomme;
        private System.Windows.Forms.Label lblRondNote;
        private System.Windows.Forms.ToolStripMenuItem mnuMIDIAddTimeLine;
        private System.Windows.Forms.Label lblBemol;
        private System.Windows.Forms.Label lblDiese;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.Label lblDoubleCrocheNote;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayLyricsWindows;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayZoom;
        private System.Windows.Forms.Label lblQuadrupleCrocheNote;
        private System.Windows.Forms.Label lblTripleCrocheNote;
        private System.Windows.Forms.Label lblBecarre;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiAddMeasures;
        private System.Windows.Forms.Label lblSaisieNotes;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplaySequencer;
        private System.Windows.Forms.Label lblLyricsInfos;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayPianoRoll;
        private System.Windows.Forms.Button btnStartRec;
        private System.Windows.Forms.ToolStripMenuItem mnufileProperties;
        private System.Windows.Forms.ToolStripSeparator MnuFileSeparator4;
        private System.Windows.Forms.Panel TimeVLine;
        private System.Windows.Forms.Panel TimeStartVLine;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiModifyTempo;
        private System.Windows.Forms.Label lblEdit;
        private System.Windows.Forms.Label lblDotted;
        private System.Windows.Forms.Label lblCurNoteInfo;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveAsPDF;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiSplitHands;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayScrolling;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayScrollingHorz;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayScrollingVert;
        private System.Windows.Forms.Label lblBass;
        private System.Windows.Forms.Label lblTreble;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiRemoveFader;
        private System.Windows.Forms.Label lblPlaylist;
        private System.Windows.Forms.ToolStripMenuItem mnuMidiTimeSignature;
        private System.Windows.Forms.Label lblChangesInfos;
        private ColorSlider.ColorSlider positionHScrollBarNew;
        private ColorSlider.ColorSlider sldMainVolume;
        private System.Windows.Forms.Label lblTransp;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.Label lnlVol;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.ProgressBar progressBarPlayer;
        private NoSelectButton btnPlay;
        private NoSelectButton btnStop;
        private NoSelectButton btnNext;
        private NoSelectButton btnPrev;
        private VU_MeterLibrary.VuMeter VuMasterPeakVolume;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem MnuEditScore;
        private System.Windows.Forms.ToolStripMenuItem MnuEditEnterNotes;
        private System.Windows.Forms.Label lblOutputDevice;
        private System.Windows.Forms.ToolStripSeparator MnuFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem MnuFileExportMidiToText;
        private System.Windows.Forms.ToolStripMenuItem MnuFileImportMidiFromText;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiImportTracks;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiImporTracksFromMidi;
        private System.Windows.Forms.ToolStripMenuItem MnuMidiImportTracksFromText;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Label lblTranspoValue;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator1;
        private System.Windows.Forms.ToolStripSeparator mnuDisplaySeparator1;
        private Buttons.PlusButtonControl btnTempoPlus;
        private Buttons.MinusButtonControl btnTempoMinus;
        private Buttons.MinusButtonControl btnTranspoMinus;
        private Buttons.PlusButtonControl btnTranspoPlus;
        private Buttons.CheckButtonControl btnMute1;
        private System.Windows.Forms.Label lblMuteMelody;
        private System.Windows.Forms.Label lblRecord;
        private System.Windows.Forms.ToolStripSeparator MnuFileSeparator3;
    }
}

