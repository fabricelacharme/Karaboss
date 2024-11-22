namespace Karaboss
{
    partial class frmLyricsEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLyricsEdit));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.dTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dRealTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnDisplayOtherLyrics = new System.Windows.Forms.Button();
            this.btnDeleteAllLyrics = new System.Windows.Forms.Button();
            this.btnInsertParagraph = new System.Windows.Forms.Button();
            this.BtnFontMoins = new System.Windows.Forms.Button();
            this.BtnFontPlus = new System.Windows.Forms.Button();
            this.btnInsertCr = new System.Windows.Forms.Button();
            this.btnSpaceRight = new System.Windows.Forms.Button();
            this.btnSpaceLeft = new System.Windows.Forms.Button();
            this.optFormatLyrics = new System.Windows.Forms.RadioButton();
            this.btnInsertText = new System.Windows.Forms.Button();
            this.optFormatText = new System.Windows.Forms.RadioButton();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSaveAsLrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAsLrcLines = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAsLrcSyllabes = new System.Windows.Forms.ToolStripMenuItem();
            this.MnuSaveAsText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLoadTrack = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditLoadMelodyText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLoadLRCFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnSaveTags = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.txtWTag = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtTTag = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtVTag = new System.Windows.Forms.TextBox();
            this.txtITag = new System.Windows.Forms.TextBox();
            this.txtLTag = new System.Windows.Forms.TextBox();
            this.txtKTag = new System.Windows.Forms.TextBox();
            this.pnlMenus = new System.Windows.Forms.Panel();
            this.lblSelectTrack = new System.Windows.Forms.Label();
            this.cbSelectTrack = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.pnlMenus.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.Controls.Add(this.dgView);
            this.toolTip1.SetToolTip(this.splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.ToolTip"));
            // 
            // splitContainer1.Panel2
            // 
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.Controls.Add(this.pnlBottom);
            this.splitContainer1.Panel2.Controls.Add(this.pnlTop);
            this.toolTip1.SetToolTip(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.ToolTip"));
            this.toolTip1.SetToolTip(this.splitContainer1, resources.GetString("splitContainer1.ToolTip"));
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dgView
            // 
            resources.ApplyResources(this.dgView, "dgView");
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dTime,
            this.dRealTime,
            this.dType,
            this.dNote,
            this.dText});
            this.dgView.Name = "dgView";
            this.toolTip1.SetToolTip(this.dgView, resources.GetString("dgView.ToolTip"));
            this.dgView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgView_CellEndEdit);
            this.dgView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgView_CellEnter);
            this.dgView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgView_KeyDown);
            this.dgView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgView_MouseDown);
            // 
            // dTime
            // 
            resources.ApplyResources(this.dTime, "dTime");
            this.dTime.Name = "dTime";
            // 
            // dRealTime
            // 
            resources.ApplyResources(this.dRealTime, "dRealTime");
            this.dRealTime.Name = "dRealTime";
            // 
            // dType
            // 
            resources.ApplyResources(this.dType, "dType");
            this.dType.MaxInputLength = 3;
            this.dType.Name = "dType";
            // 
            // dNote
            // 
            resources.ApplyResources(this.dNote, "dNote");
            this.dNote.Name = "dNote";
            // 
            // dText
            // 
            resources.ApplyResources(this.dText, "dText");
            this.dText.Name = "dText";
            // 
            // pnlBottom
            // 
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Controls.Add(this.txtResult);
            this.pnlBottom.Name = "pnlBottom";
            this.toolTip1.SetToolTip(this.pnlBottom, resources.GetString("pnlBottom.ToolTip"));
            // 
            // txtResult
            // 
            resources.ApplyResources(this.txtResult, "txtResult");
            this.txtResult.BackColor = System.Drawing.Color.Black;
            this.txtResult.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtResult.DetectUrls = false;
            this.txtResult.ForeColor = System.Drawing.Color.MediumTurquoise;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.toolTip1.SetToolTip(this.txtResult, resources.GetString("txtResult.ToolTip"));
            // 
            // pnlTop
            // 
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Controls.Add(this.btnDisplayOtherLyrics);
            this.pnlTop.Controls.Add(this.btnDeleteAllLyrics);
            this.pnlTop.Controls.Add(this.btnInsertParagraph);
            this.pnlTop.Controls.Add(this.BtnFontMoins);
            this.pnlTop.Controls.Add(this.BtnFontPlus);
            this.pnlTop.Controls.Add(this.btnInsertCr);
            this.pnlTop.Controls.Add(this.btnSpaceRight);
            this.pnlTop.Controls.Add(this.btnSpaceLeft);
            this.pnlTop.Controls.Add(this.optFormatLyrics);
            this.pnlTop.Controls.Add(this.btnInsertText);
            this.pnlTop.Controls.Add(this.optFormatText);
            this.pnlTop.Controls.Add(this.btnDelete);
            this.pnlTop.Controls.Add(this.btnPlay);
            this.pnlTop.Controls.Add(this.btnView);
            this.pnlTop.Controls.Add(this.btnSave);
            this.pnlTop.Name = "pnlTop";
            this.toolTip1.SetToolTip(this.pnlTop, resources.GetString("pnlTop.ToolTip"));
            // 
            // btnDisplayOtherLyrics
            // 
            resources.ApplyResources(this.btnDisplayOtherLyrics, "btnDisplayOtherLyrics");
            this.btnDisplayOtherLyrics.Name = "btnDisplayOtherLyrics";
            this.toolTip1.SetToolTip(this.btnDisplayOtherLyrics, resources.GetString("btnDisplayOtherLyrics.ToolTip"));
            this.btnDisplayOtherLyrics.UseVisualStyleBackColor = true;
            this.btnDisplayOtherLyrics.Click += new System.EventHandler(this.btnDisplayOtherLyrics_Click);
            // 
            // btnDeleteAllLyrics
            // 
            resources.ApplyResources(this.btnDeleteAllLyrics, "btnDeleteAllLyrics");
            this.btnDeleteAllLyrics.Image = global::Karaboss.Properties.Resources.delete_icon;
            this.btnDeleteAllLyrics.Name = "btnDeleteAllLyrics";
            this.toolTip1.SetToolTip(this.btnDeleteAllLyrics, resources.GetString("btnDeleteAllLyrics.ToolTip"));
            this.btnDeleteAllLyrics.UseVisualStyleBackColor = true;
            this.btnDeleteAllLyrics.Click += new System.EventHandler(this.btnDeleteAllLyrics_Click);
            // 
            // btnInsertParagraph
            // 
            resources.ApplyResources(this.btnInsertParagraph, "btnInsertParagraph");
            this.btnInsertParagraph.Name = "btnInsertParagraph";
            this.toolTip1.SetToolTip(this.btnInsertParagraph, resources.GetString("btnInsertParagraph.ToolTip"));
            this.btnInsertParagraph.UseVisualStyleBackColor = true;
            this.btnInsertParagraph.Click += new System.EventHandler(this.btnInsertParagraph_Click);
            // 
            // BtnFontMoins
            // 
            resources.ApplyResources(this.BtnFontMoins, "BtnFontMoins");
            this.BtnFontMoins.Name = "BtnFontMoins";
            this.toolTip1.SetToolTip(this.BtnFontMoins, resources.GetString("BtnFontMoins.ToolTip"));
            this.BtnFontMoins.UseVisualStyleBackColor = true;
            this.BtnFontMoins.Click += new System.EventHandler(this.BtnFontMoins_Click);
            // 
            // BtnFontPlus
            // 
            resources.ApplyResources(this.BtnFontPlus, "BtnFontPlus");
            this.BtnFontPlus.Name = "BtnFontPlus";
            this.toolTip1.SetToolTip(this.BtnFontPlus, resources.GetString("BtnFontPlus.ToolTip"));
            this.BtnFontPlus.UseVisualStyleBackColor = true;
            this.BtnFontPlus.Click += new System.EventHandler(this.BtnFontPlus_Click);
            // 
            // btnInsertCr
            // 
            resources.ApplyResources(this.btnInsertCr, "btnInsertCr");
            this.btnInsertCr.Name = "btnInsertCr";
            this.btnInsertCr.TabStop = false;
            this.toolTip1.SetToolTip(this.btnInsertCr, resources.GetString("btnInsertCr.ToolTip"));
            this.btnInsertCr.UseVisualStyleBackColor = true;
            this.btnInsertCr.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // btnSpaceRight
            // 
            resources.ApplyResources(this.btnSpaceRight, "btnSpaceRight");
            this.btnSpaceRight.Name = "btnSpaceRight";
            this.toolTip1.SetToolTip(this.btnSpaceRight, resources.GetString("btnSpaceRight.ToolTip"));
            this.btnSpaceRight.UseVisualStyleBackColor = true;
            this.btnSpaceRight.Click += new System.EventHandler(this.BtnSpaceRight_Click);
            // 
            // btnSpaceLeft
            // 
            resources.ApplyResources(this.btnSpaceLeft, "btnSpaceLeft");
            this.btnSpaceLeft.Name = "btnSpaceLeft";
            this.toolTip1.SetToolTip(this.btnSpaceLeft, resources.GetString("btnSpaceLeft.ToolTip"));
            this.btnSpaceLeft.UseVisualStyleBackColor = true;
            this.btnSpaceLeft.Click += new System.EventHandler(this.BtnSpaceLeft_Click);
            // 
            // optFormatLyrics
            // 
            resources.ApplyResources(this.optFormatLyrics, "optFormatLyrics");
            this.optFormatLyrics.Name = "optFormatLyrics";
            this.toolTip1.SetToolTip(this.optFormatLyrics, resources.GetString("optFormatLyrics.ToolTip"));
            this.optFormatLyrics.UseVisualStyleBackColor = true;
            this.optFormatLyrics.CheckedChanged += new System.EventHandler(this.OptFormatLyrics_CheckedChanged);
            // 
            // btnInsertText
            // 
            resources.ApplyResources(this.btnInsertText, "btnInsertText");
            this.btnInsertText.Name = "btnInsertText";
            this.toolTip1.SetToolTip(this.btnInsertText, resources.GetString("btnInsertText.ToolTip"));
            this.btnInsertText.UseVisualStyleBackColor = true;
            this.btnInsertText.Click += new System.EventHandler(this.BtnInsertText_Click);
            // 
            // optFormatText
            // 
            resources.ApplyResources(this.optFormatText, "optFormatText");
            this.optFormatText.Checked = true;
            this.optFormatText.Name = "optFormatText";
            this.optFormatText.TabStop = true;
            this.toolTip1.SetToolTip(this.optFormatText, resources.GetString("optFormatText.ToolTip"));
            this.optFormatText.UseVisualStyleBackColor = true;
            this.optFormatText.CheckedChanged += new System.EventHandler(this.OptFormatText_CheckedChanged);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnPlay
            // 
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Image = global::Karaboss.Properties.Resources.control_play_blue;
            this.btnPlay.Name = "btnPlay";
            this.toolTip1.SetToolTip(this.btnPlay, resources.GetString("btnPlay.ToolTip"));
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // btnView
            // 
            resources.ApplyResources(this.btnView, "btnView");
            this.btnView.Name = "btnView";
            this.toolTip1.SetToolTip(this.btnView, resources.GetString("btnView.ToolTip"));
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.BtnView_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Image = global::Karaboss.Properties.Resources.floppy_icon;
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuHelp});
            this.menuStrip1.Name = "menuStrip1";
            this.toolTip1.SetToolTip(this.menuStrip1, resources.GetString("menuStrip1.ToolTip"));
            // 
            // mnuFile
            // 
            resources.ApplyResources(this.mnuFile, "mnuFile");
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.mnuFileSep2,
            this.mnuFileSaveAsLrc,
            this.MnuSaveAsText,
            this.mnuFileSep1,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            // 
            // mnuFileSave
            // 
            resources.ApplyResources(this.mnuFileSave, "mnuFileSave");
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.Click += new System.EventHandler(this.MnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            resources.ApplyResources(this.mnuFileSaveAs, "mnuFileSaveAs");
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.MnuFileSaveAs_Click);
            // 
            // mnuFileSep2
            // 
            resources.ApplyResources(this.mnuFileSep2, "mnuFileSep2");
            this.mnuFileSep2.Name = "mnuFileSep2";
            // 
            // mnuFileSaveAsLrc
            // 
            resources.ApplyResources(this.mnuFileSaveAsLrc, "mnuFileSaveAsLrc");
            this.mnuFileSaveAsLrc.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileSaveAsLrcLines,
            this.mnuFileSaveAsLrcSyllabes});
            this.mnuFileSaveAsLrc.Name = "mnuFileSaveAsLrc";
            // 
            // mnuFileSaveAsLrcLines
            // 
            resources.ApplyResources(this.mnuFileSaveAsLrcLines, "mnuFileSaveAsLrcLines");
            this.mnuFileSaveAsLrcLines.Name = "mnuFileSaveAsLrcLines";
            this.mnuFileSaveAsLrcLines.Click += new System.EventHandler(this.mnuFileSaveAsLrcLines_Click);
            // 
            // mnuFileSaveAsLrcSyllabes
            // 
            resources.ApplyResources(this.mnuFileSaveAsLrcSyllabes, "mnuFileSaveAsLrcSyllabes");
            this.mnuFileSaveAsLrcSyllabes.Name = "mnuFileSaveAsLrcSyllabes";
            this.mnuFileSaveAsLrcSyllabes.Click += new System.EventHandler(this.mnuFileSaveAsLrcSyllabes_Click);
            // 
            // MnuSaveAsText
            // 
            resources.ApplyResources(this.MnuSaveAsText, "MnuSaveAsText");
            this.MnuSaveAsText.Name = "MnuSaveAsText";
            this.MnuSaveAsText.Click += new System.EventHandler(this.MnuSaveAsText_Click);
            // 
            // mnuFileSep1
            // 
            resources.ApplyResources(this.mnuFileSep1, "mnuFileSep1");
            this.mnuFileSep1.Name = "mnuFileSep1";
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
            this.mnuEditLoadTrack,
            this.mnuEditSep1,
            this.mnuEditLoadMelodyText,
            this.mnuEditLoadLRCFile});
            this.mnuEdit.Name = "mnuEdit";
            // 
            // mnuEditLoadTrack
            // 
            resources.ApplyResources(this.mnuEditLoadTrack, "mnuEditLoadTrack");
            this.mnuEditLoadTrack.Name = "mnuEditLoadTrack";
            this.mnuEditLoadTrack.Click += new System.EventHandler(this.MnuEditLoadTrack_Click);
            // 
            // mnuEditSep1
            // 
            resources.ApplyResources(this.mnuEditSep1, "mnuEditSep1");
            this.mnuEditSep1.Name = "mnuEditSep1";
            // 
            // mnuEditLoadMelodyText
            // 
            resources.ApplyResources(this.mnuEditLoadMelodyText, "mnuEditLoadMelodyText");
            this.mnuEditLoadMelodyText.Name = "mnuEditLoadMelodyText";
            this.mnuEditLoadMelodyText.Click += new System.EventHandler(this.MnuEditLoadMelodyText_Click);
            // 
            // mnuEditLoadLRCFile
            // 
            resources.ApplyResources(this.mnuEditLoadLRCFile, "mnuEditLoadLRCFile");
            this.mnuEditLoadLRCFile.Name = "mnuEditLoadLRCFile";
            this.mnuEditLoadLRCFile.Click += new System.EventHandler(this.mnuEditLoadLRCFile_Click);
            // 
            // mnuHelp
            // 
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            // 
            // mnuHelpAbout
            // 
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Click += new System.EventHandler(this.MnuHelpAbout_Click);
            // 
            // saveMidiFileDialog
            // 
            resources.ApplyResources(this.saveMidiFileDialog, "saveMidiFileDialog");
            // 
            // openFileDialog
            // 
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, resources.GetString("tabControl1.ToolTip"));
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Name = "tabPage1";
            this.toolTip1.SetToolTip(this.tabPage1, resources.GetString("tabPage1.ToolTip"));
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.btnSaveTags);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.txtWTag);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.txtTTag);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.txtVTag);
            this.tabPage2.Controls.Add(this.txtITag);
            this.tabPage2.Controls.Add(this.txtLTag);
            this.tabPage2.Controls.Add(this.txtKTag);
            this.tabPage2.Name = "tabPage2";
            this.toolTip1.SetToolTip(this.tabPage2, resources.GetString("tabPage2.ToolTip"));
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnSaveTags
            // 
            resources.ApplyResources(this.btnSaveTags, "btnSaveTags");
            this.btnSaveTags.Name = "btnSaveTags";
            this.toolTip1.SetToolTip(this.btnSaveTags, resources.GetString("btnSaveTags.ToolTip"));
            this.btnSaveTags.UseVisualStyleBackColor = true;
            this.btnSaveTags.Click += new System.EventHandler(this.btnSaveTags_Click);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            this.toolTip1.SetToolTip(this.label15, resources.GetString("label15.ToolTip"));
            // 
            // txtWTag
            // 
            resources.ApplyResources(this.txtWTag, "txtWTag");
            this.txtWTag.Name = "txtWTag";
            this.toolTip1.SetToolTip(this.txtWTag, resources.GetString("txtWTag.ToolTip"));
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            this.toolTip1.SetToolTip(this.label14, resources.GetString("label14.ToolTip"));
            // 
            // txtTTag
            // 
            resources.ApplyResources(this.txtTTag, "txtTTag");
            this.txtTTag.Name = "txtTTag";
            this.toolTip1.SetToolTip(this.txtTTag, resources.GetString("txtTTag.ToolTip"));
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.toolTip1.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.toolTip1.SetToolTip(this.label12, resources.GetString("label12.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // txtVTag
            // 
            resources.ApplyResources(this.txtVTag, "txtVTag");
            this.txtVTag.Name = "txtVTag";
            this.toolTip1.SetToolTip(this.txtVTag, resources.GetString("txtVTag.ToolTip"));
            // 
            // txtITag
            // 
            resources.ApplyResources(this.txtITag, "txtITag");
            this.txtITag.Name = "txtITag";
            this.toolTip1.SetToolTip(this.txtITag, resources.GetString("txtITag.ToolTip"));
            // 
            // txtLTag
            // 
            resources.ApplyResources(this.txtLTag, "txtLTag");
            this.txtLTag.Name = "txtLTag";
            this.toolTip1.SetToolTip(this.txtLTag, resources.GetString("txtLTag.ToolTip"));
            // 
            // txtKTag
            // 
            resources.ApplyResources(this.txtKTag, "txtKTag");
            this.txtKTag.Name = "txtKTag";
            this.toolTip1.SetToolTip(this.txtKTag, resources.GetString("txtKTag.ToolTip"));
            // 
            // pnlMenus
            // 
            resources.ApplyResources(this.pnlMenus, "pnlMenus");
            this.pnlMenus.Controls.Add(this.lblSelectTrack);
            this.pnlMenus.Controls.Add(this.cbSelectTrack);
            this.pnlMenus.Name = "pnlMenus";
            this.toolTip1.SetToolTip(this.pnlMenus, resources.GetString("pnlMenus.ToolTip"));
            // 
            // lblSelectTrack
            // 
            resources.ApplyResources(this.lblSelectTrack, "lblSelectTrack");
            this.lblSelectTrack.Name = "lblSelectTrack";
            this.toolTip1.SetToolTip(this.lblSelectTrack, resources.GetString("lblSelectTrack.ToolTip"));
            // 
            // cbSelectTrack
            // 
            resources.ApplyResources(this.cbSelectTrack, "cbSelectTrack");
            this.cbSelectTrack.FormattingEnabled = true;
            this.cbSelectTrack.Name = "cbSelectTrack";
            this.toolTip1.SetToolTip(this.cbSelectTrack, resources.GetString("cbSelectTrack.ToolTip"));
            this.cbSelectTrack.SelectedIndexChanged += new System.EventHandler(this.cbSelectTrack_SelectedIndexChanged);
            // 
            // frmLyricsEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMenus);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmLyricsEdit";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmLyricsEdit_FormClosing);
            this.Load += new System.EventHandler(this.FrmLyricsEdit_Load);
            this.Resize += new System.EventHandler(this.frmLyricsEdit_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.pnlMenus.ResumeLayout(false);
            this.pnlMenus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnInsertCr;
        private System.Windows.Forms.Button btnInsertText;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.SaveFileDialog saveMidiFileDialog;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSpaceLeft;
        private System.Windows.Forms.Button btnSpaceRight;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadTrack;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadMelodyText;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.RadioButton optFormatText;
        private System.Windows.Forms.RadioButton optFormatLyrics;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button BtnFontMoins;
        private System.Windows.Forms.Button BtnFontPlus;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAsLrc;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep1;
        private System.Windows.Forms.ToolStripSeparator mnuEditSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadLRCFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn dTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dRealTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn dText;
        private System.Windows.Forms.Button btnInsertParagraph;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtWTag;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtTTag;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtVTag;
        private System.Windows.Forms.TextBox txtITag;
        private System.Windows.Forms.TextBox txtLTag;
        private System.Windows.Forms.TextBox txtKTag;
        private System.Windows.Forms.Button btnSaveTags;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAsLrcLines;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAsLrcSyllabes;
        private System.Windows.Forms.Button btnDeleteAllLyrics;
        private System.Windows.Forms.ToolStripMenuItem MnuSaveAsText;
        private System.Windows.Forms.Panel pnlMenus;
        private System.Windows.Forms.ComboBox cbSelectTrack;
        private System.Windows.Forms.Label lblSelectTrack;
        private System.Windows.Forms.Button btnDisplayOtherLyrics;
    }
}