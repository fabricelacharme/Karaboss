namespace Karaboss.Mp3.Mp3Lyrics
{
    partial class frmMp3LyricsEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMp3LyricsEdit));
            this.pnlEdit = new System.Windows.Forms.Panel();
            this.BtnFontMoins = new System.Windows.Forms.Button();
            this.BtnFontPlus = new System.Windows.Forms.Button();
            this.btnDeleteAllLyrics = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnInsertParagraph = new System.Windows.Forms.Button();
            this.btnInsertCr = new System.Windows.Forms.Button();
            this.btnInsertText = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolstrip1 = new System.Windows.Forms.ToolStrip();
            this.mnuImport = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuImportLrcFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuImportRawLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuExportLRCMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExportLrcNoMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSwitchSyncEdit = new System.Windows.Forms.ToolStripButton();
            this.lblMode = new System.Windows.Forms.Label();
            this.lblLyricsOrigin = new System.Windows.Forms.Label();
            this.pnlSync = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLanguage = new System.Windows.Forms.ComboBox();
            this.txtAlbum = new System.Windows.Forms.TextBox();
            this.txtYourName = new System.Windows.Forms.TextBox();
            this.txtArtist = new System.Windows.Forms.TextBox();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblHotkeys = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.lblLyrics = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSaveAsLrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditImportLrcFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditImportRawLyrics = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditExportAsLrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlEdit.SuspendLayout();
            this.toolstrip1.SuspendLayout();
            this.pnlSync.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlEdit
            // 
            this.pnlEdit.BackColor = System.Drawing.Color.LightGray;
            this.pnlEdit.Controls.Add(this.BtnFontMoins);
            this.pnlEdit.Controls.Add(this.BtnFontPlus);
            this.pnlEdit.Controls.Add(this.btnDeleteAllLyrics);
            this.pnlEdit.Controls.Add(this.btnSave);
            this.pnlEdit.Controls.Add(this.btnInsertParagraph);
            this.pnlEdit.Controls.Add(this.btnInsertCr);
            this.pnlEdit.Controls.Add(this.btnInsertText);
            this.pnlEdit.Controls.Add(this.btnDelete);
            resources.ApplyResources(this.pnlEdit, "pnlEdit");
            this.pnlEdit.Name = "pnlEdit";
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
            // btnDeleteAllLyrics
            // 
            this.btnDeleteAllLyrics.Image = global::Karaboss.Properties.Resources.delete_icon;
            resources.ApplyResources(this.btnDeleteAllLyrics, "btnDeleteAllLyrics");
            this.btnDeleteAllLyrics.Name = "btnDeleteAllLyrics";
            this.toolTip1.SetToolTip(this.btnDeleteAllLyrics, resources.GetString("btnDeleteAllLyrics.ToolTip"));
            this.btnDeleteAllLyrics.UseVisualStyleBackColor = true;
            this.btnDeleteAllLyrics.Click += new System.EventHandler(this.btnDeleteAllLyrics_Click);
            // 
            // btnSave
            // 
            this.btnSave.Image = global::Karaboss.Properties.Resources.floppy_icon;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnInsertParagraph
            // 
            resources.ApplyResources(this.btnInsertParagraph, "btnInsertParagraph");
            this.btnInsertParagraph.Name = "btnInsertParagraph";
            this.toolTip1.SetToolTip(this.btnInsertParagraph, resources.GetString("btnInsertParagraph.ToolTip"));
            this.btnInsertParagraph.UseVisualStyleBackColor = true;
            this.btnInsertParagraph.Click += new System.EventHandler(this.btnInsertParagraph_Click);
            // 
            // btnInsertCr
            // 
            resources.ApplyResources(this.btnInsertCr, "btnInsertCr");
            this.btnInsertCr.Name = "btnInsertCr";
            this.btnInsertCr.TabStop = false;
            this.toolTip1.SetToolTip(this.btnInsertCr, resources.GetString("btnInsertCr.ToolTip"));
            this.btnInsertCr.UseVisualStyleBackColor = true;
            this.btnInsertCr.Click += new System.EventHandler(this.btnInsertCr_Click);
            // 
            // btnInsertText
            // 
            resources.ApplyResources(this.btnInsertText, "btnInsertText");
            this.btnInsertText.Name = "btnInsertText";
            this.toolTip1.SetToolTip(this.btnInsertText, resources.GetString("btnInsertText.ToolTip"));
            this.btnInsertText.UseVisualStyleBackColor = true;
            this.btnInsertText.Click += new System.EventHandler(this.btnInsertText_Click);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.toolTip1.SetToolTip(this.btnDelete, resources.GetString("btnDelete.ToolTip"));
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // toolstrip1
            // 
            resources.ApplyResources(this.toolstrip1, "toolstrip1");
            this.toolstrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuImport,
            this.mnuExport,
            this.btnSwitchSyncEdit});
            this.toolstrip1.Name = "toolstrip1";
            this.toolstrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // mnuImport
            // 
            this.mnuImport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuImportLrcFile,
            this.mnuImportRawLyrics});
            resources.ApplyResources(this.mnuImport, "mnuImport");
            this.mnuImport.Name = "mnuImport";
            // 
            // mnuImportLrcFile
            // 
            this.mnuImportLrcFile.Name = "mnuImportLrcFile";
            resources.ApplyResources(this.mnuImportLrcFile, "mnuImportLrcFile");
            this.mnuImportLrcFile.Click += new System.EventHandler(this.mnuImportLrcFile_Click);
            // 
            // mnuImportRawLyrics
            // 
            this.mnuImportRawLyrics.Name = "mnuImportRawLyrics";
            resources.ApplyResources(this.mnuImportRawLyrics, "mnuImportRawLyrics");
            this.mnuImportRawLyrics.Click += new System.EventHandler(this.mnuImportRawLyrics_Click);
            // 
            // mnuExport
            // 
            this.mnuExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExportLRCMeta,
            this.mnuExportLrcNoMeta});
            resources.ApplyResources(this.mnuExport, "mnuExport");
            this.mnuExport.Name = "mnuExport";
            // 
            // mnuExportLRCMeta
            // 
            this.mnuExportLRCMeta.Name = "mnuExportLRCMeta";
            resources.ApplyResources(this.mnuExportLRCMeta, "mnuExportLRCMeta");
            this.mnuExportLRCMeta.Click += new System.EventHandler(this.mnuExportLRCMeta_Click);
            // 
            // mnuExportLrcNoMeta
            // 
            this.mnuExportLrcNoMeta.Name = "mnuExportLrcNoMeta";
            resources.ApplyResources(this.mnuExportLrcNoMeta, "mnuExportLrcNoMeta");
            this.mnuExportLrcNoMeta.Click += new System.EventHandler(this.mnuExportLrcNoMeta_Click);
            // 
            // btnSwitchSyncEdit
            // 
            this.btnSwitchSyncEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.btnSwitchSyncEdit, "btnSwitchSyncEdit");
            this.btnSwitchSyncEdit.Name = "btnSwitchSyncEdit";
            this.btnSwitchSyncEdit.Click += new System.EventHandler(this.btnSwitchSyncEdit_Click);
            // 
            // lblMode
            // 
            resources.ApplyResources(this.lblMode, "lblMode");
            this.lblMode.ForeColor = System.Drawing.Color.White;
            this.lblMode.Name = "lblMode";
            // 
            // lblLyricsOrigin
            // 
            resources.ApplyResources(this.lblLyricsOrigin, "lblLyricsOrigin");
            this.lblLyricsOrigin.ForeColor = System.Drawing.Color.White;
            this.lblLyricsOrigin.Name = "lblLyricsOrigin";
            // 
            // pnlSync
            // 
            this.pnlSync.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlSync.Controls.Add(this.pnlEdit);
            this.pnlSync.Controls.Add(this.label6);
            this.pnlSync.Controls.Add(this.label5);
            this.pnlSync.Controls.Add(this.label4);
            this.pnlSync.Controls.Add(this.label3);
            this.pnlSync.Controls.Add(this.label1);
            this.pnlSync.Controls.Add(this.cbLanguage);
            this.pnlSync.Controls.Add(this.txtAlbum);
            this.pnlSync.Controls.Add(this.txtYourName);
            this.pnlSync.Controls.Add(this.txtArtist);
            this.pnlSync.Controls.Add(this.txtAuthor);
            this.pnlSync.Controls.Add(this.txtTitle);
            this.pnlSync.Controls.Add(this.lblHotkeys);
            resources.ApplyResources(this.pnlSync, "pnlSync");
            this.pnlSync.Name = "pnlSync";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbLanguage
            // 
            this.cbLanguage.FormattingEnabled = true;
            resources.ApplyResources(this.cbLanguage, "cbLanguage");
            this.cbLanguage.Name = "cbLanguage";
            // 
            // txtAlbum
            // 
            resources.ApplyResources(this.txtAlbum, "txtAlbum");
            this.txtAlbum.Name = "txtAlbum";
            this.toolTip1.SetToolTip(this.txtAlbum, resources.GetString("txtAlbum.ToolTip"));
            // 
            // txtYourName
            // 
            resources.ApplyResources(this.txtYourName, "txtYourName");
            this.txtYourName.Name = "txtYourName";
            // 
            // txtArtist
            // 
            resources.ApplyResources(this.txtArtist, "txtArtist");
            this.txtArtist.Name = "txtArtist";
            this.toolTip1.SetToolTip(this.txtArtist, resources.GetString("txtArtist.ToolTip"));
            // 
            // txtAuthor
            // 
            resources.ApplyResources(this.txtAuthor, "txtAuthor");
            this.txtAuthor.Name = "txtAuthor";
            this.toolTip1.SetToolTip(this.txtAuthor, resources.GetString("txtAuthor.ToolTip"));
            // 
            // txtTitle
            // 
            resources.ApplyResources(this.txtTitle, "txtTitle");
            this.txtTitle.Name = "txtTitle";
            this.toolTip1.SetToolTip(this.txtTitle, resources.GetString("txtTitle.ToolTip"));
            // 
            // lblHotkeys
            // 
            this.lblHotkeys.BackColor = System.Drawing.Color.Silver;
            resources.ApplyResources(this.lblHotkeys, "lblHotkeys");
            this.lblHotkeys.Name = "lblHotkeys";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtResult);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dgView
            // 
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgView, "dgView");
            this.dgView.Name = "dgView";
            this.dgView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellEndEdit);
            this.dgView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellEnter);
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.Color.Black;
            this.txtResult.Cursor = System.Windows.Forms.Cursors.Default;
            this.txtResult.DetectUrls = false;
            resources.ApplyResources(this.txtResult, "txtResult");
            this.txtResult.ForeColor = System.Drawing.Color.MediumTurquoise;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            // 
            // lblLyrics
            // 
            resources.ApplyResources(this.lblLyrics, "lblLyrics");
            this.lblLyrics.ForeColor = System.Drawing.Color.White;
            this.lblLyrics.Name = "lblLyrics";
            // 
            // lblTimes
            // 
            resources.ApplyResources(this.lblTimes, "lblTimes");
            this.lblTimes.ForeColor = System.Drawing.Color.White;
            this.lblTimes.Name = "lblTimes";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuHelp});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileSave,
            this.mnuFileSep2,
            this.mnuFileSaveAsLrc,
            this.mnuFileSep1,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            resources.ApplyResources(this.mnuFileSave, "mnuFileSave");
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSep2
            // 
            this.mnuFileSep2.Name = "mnuFileSep2";
            resources.ApplyResources(this.mnuFileSep2, "mnuFileSep2");
            // 
            // mnuFileSaveAsLrc
            // 
            this.mnuFileSaveAsLrc.Name = "mnuFileSaveAsLrc";
            resources.ApplyResources(this.mnuFileSaveAsLrc, "mnuFileSaveAsLrc");
            this.mnuFileSaveAsLrc.Click += new System.EventHandler(this.mnuFileSaveAsLrc_Click);
            // 
            // mnuFileSep1
            // 
            this.mnuFileSep1.Name = "mnuFileSep1";
            resources.ApplyResources(this.mnuFileSep1, "mnuFileSep1");
            // 
            // mnuFileQuit
            // 
            this.mnuFileQuit.Name = "mnuFileQuit";
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Click += new System.EventHandler(this.mnuFileQuit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditImportLrcFile,
            this.mnuEditImportRawLyrics,
            this.toolStripSeparator1,
            this.mnuEditExportAsLrc});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditImportLrcFile
            // 
            this.mnuEditImportLrcFile.Name = "mnuEditImportLrcFile";
            resources.ApplyResources(this.mnuEditImportLrcFile, "mnuEditImportLrcFile");
            this.mnuEditImportLrcFile.Click += new System.EventHandler(this.mnuEditLoadLRCFile_Click);
            // 
            // mnuEditImportRawLyrics
            // 
            this.mnuEditImportRawLyrics.Name = "mnuEditImportRawLyrics";
            resources.ApplyResources(this.mnuEditImportRawLyrics, "mnuEditImportRawLyrics");
            this.mnuEditImportRawLyrics.Click += new System.EventHandler(this.mnuEditImportRawLyrics_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // mnuEditExportAsLrc
            // 
            this.mnuEditExportAsLrc.Name = "mnuEditExportAsLrc";
            resources.ApplyResources(this.mnuEditExportAsLrc, "mnuEditExportAsLrc");
            this.mnuEditExportAsLrc.Click += new System.EventHandler(this.mnuEditExportAsLrc_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            resources.ApplyResources(this.mnuHelpAbout, "mnuHelpAbout");
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.toolstrip1);
            this.pnlTop.Controls.Add(this.lblMode);
            this.pnlTop.Controls.Add(this.lblLyricsOrigin);
            this.pnlTop.Controls.Add(this.pnlSync);
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlBottom.Controls.Add(this.lblLyrics);
            this.pnlBottom.Controls.Add(this.lblTimes);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
            // 
            // frmMp3LyricsEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMp3LyricsEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3LyricsEdit_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3LyricsEdit_Load);
            this.Resize += new System.EventHandler(this.frmMp3LyricsEdit_Resize);
            this.pnlEdit.ResumeLayout(false);
            this.toolstrip1.ResumeLayout(false);
            this.toolstrip1.PerformLayout();
            this.pnlSync.ResumeLayout(false);
            this.pnlSync.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAsLrc;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditImportLrcFile;
        private System.Windows.Forms.Label lblLyricsOrigin;
        private System.Windows.Forms.ToolStripMenuItem mnuEditImportRawLyrics;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditExportAsLrc;
        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlEdit;
        private System.Windows.Forms.Button BtnFontMoins;
        private System.Windows.Forms.Button BtnFontPlus;
        private System.Windows.Forms.Button btnDeleteAllLyrics;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnInsertParagraph;
        private System.Windows.Forms.Button btnInsertCr;
        private System.Windows.Forms.Button btnInsertText;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Panel pnlSync;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLanguage;
        private System.Windows.Forms.TextBox txtAlbum;
        private System.Windows.Forms.TextBox txtYourName;
        private System.Windows.Forms.TextBox txtArtist;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblHotkeys;
        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ToolStrip toolstrip1;
        private System.Windows.Forms.ToolStripDropDownButton mnuImport;
        private System.Windows.Forms.ToolStripMenuItem mnuImportLrcFile;
        private System.Windows.Forms.ToolStripMenuItem mnuImportRawLyrics;
        private System.Windows.Forms.ToolStripDropDownButton mnuExport;
        private System.Windows.Forms.ToolStripMenuItem mnuExportLRCMeta;
        private System.Windows.Forms.ToolStripMenuItem mnuExportLrcNoMeta;
        private System.Windows.Forms.ToolStripButton btnSwitchSyncEdit;
        private System.Windows.Forms.Label lblLyrics;
        private System.Windows.Forms.Label lblTimes;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlBottom;
    }
}