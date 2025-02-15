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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnDeleteAllLyrics = new System.Windows.Forms.Button();
            this.btnInsertParagraph = new System.Windows.Forms.Button();
            this.btnInsertCr = new System.Windows.Forms.Button();
            this.btnInsertText = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSaveAsLrc = new System.Windows.Forms.ToolStripMenuItem();
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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.clTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlRightBottom = new System.Windows.Forms.Panel();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.pnlRightTop = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.pnlRightBottom.SuspendLayout();
            this.pnlRightTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDeleteAllLyrics
            // 
            this.btnDeleteAllLyrics.Image = global::Karaboss.Properties.Resources.delete_icon;
            resources.ApplyResources(this.btnDeleteAllLyrics, "btnDeleteAllLyrics");
            this.btnDeleteAllLyrics.Name = "btnDeleteAllLyrics";
            this.toolTip1.SetToolTip(this.btnDeleteAllLyrics, resources.GetString("btnDeleteAllLyrics.ToolTip"));
            this.btnDeleteAllLyrics.UseVisualStyleBackColor = true;
            // 
            // btnInsertParagraph
            // 
            resources.ApplyResources(this.btnInsertParagraph, "btnInsertParagraph");
            this.btnInsertParagraph.Name = "btnInsertParagraph";
            this.toolTip1.SetToolTip(this.btnInsertParagraph, resources.GetString("btnInsertParagraph.ToolTip"));
            this.btnInsertParagraph.UseVisualStyleBackColor = true;
            // 
            // btnInsertCr
            // 
            resources.ApplyResources(this.btnInsertCr, "btnInsertCr");
            this.btnInsertCr.Name = "btnInsertCr";
            this.btnInsertCr.TabStop = false;
            this.toolTip1.SetToolTip(this.btnInsertCr, resources.GetString("btnInsertCr.ToolTip"));
            this.btnInsertCr.UseVisualStyleBackColor = true;
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
            // btnSave
            // 
            this.btnSave.Image = global::Karaboss.Properties.Resources.floppy_icon;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.mnuFileSaveAs,
            this.mnuFileSep2,
            this.mnuFileSaveAsLrc,
            this.MnuSaveAsText,
            this.mnuFileSep1,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            resources.ApplyResources(this.mnuFileSave, "mnuFileSave");
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            resources.ApplyResources(this.mnuFileSaveAs, "mnuFileSaveAs");
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
            // 
            // MnuSaveAsText
            // 
            this.MnuSaveAsText.Name = "MnuSaveAsText";
            resources.ApplyResources(this.MnuSaveAsText, "MnuSaveAsText");
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
            this.mnuEditLoadTrack,
            this.mnuEditSep1,
            this.mnuEditLoadMelodyText,
            this.mnuEditLoadLRCFile});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditLoadTrack
            // 
            this.mnuEditLoadTrack.Name = "mnuEditLoadTrack";
            resources.ApplyResources(this.mnuEditLoadTrack, "mnuEditLoadTrack");
            // 
            // mnuEditSep1
            // 
            this.mnuEditSep1.Name = "mnuEditSep1";
            resources.ApplyResources(this.mnuEditSep1, "mnuEditSep1");
            // 
            // mnuEditLoadMelodyText
            // 
            this.mnuEditLoadMelodyText.Name = "mnuEditLoadMelodyText";
            resources.ApplyResources(this.mnuEditLoadMelodyText, "mnuEditLoadMelodyText");
            // 
            // mnuEditLoadLRCFile
            // 
            this.mnuEditLoadLRCFile.Name = "mnuEditLoadLRCFile";
            resources.ApplyResources(this.mnuEditLoadLRCFile, "mnuEditLoadLRCFile");
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
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.splitContainer1);
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Name = "pnlMiddle";
            // 
            // splitContainer1
            // 
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
            this.splitContainer1.Panel2.Controls.Add(this.pnlRightBottom);
            this.splitContainer1.Panel2.Controls.Add(this.pnlRightTop);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dgView
            // 
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clTime,
            this.clText});
            resources.ApplyResources(this.dgView, "dgView");
            this.dgView.Name = "dgView";
            // 
            // clTime
            // 
            resources.ApplyResources(this.clTime, "clTime");
            this.clTime.Name = "clTime";
            // 
            // clText
            // 
            resources.ApplyResources(this.clText, "clText");
            this.clText.Name = "clText";
            // 
            // pnlRightBottom
            // 
            this.pnlRightBottom.Controls.Add(this.txtResult);
            resources.ApplyResources(this.pnlRightBottom, "pnlRightBottom");
            this.pnlRightBottom.Name = "pnlRightBottom";
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
            // pnlRightTop
            // 
            this.pnlRightTop.Controls.Add(this.btnDeleteAllLyrics);
            this.pnlRightTop.Controls.Add(this.btnInsertParagraph);
            this.pnlRightTop.Controls.Add(this.btnInsertCr);
            this.pnlRightTop.Controls.Add(this.btnInsertText);
            this.pnlRightTop.Controls.Add(this.btnDelete);
            this.pnlRightTop.Controls.Add(this.btnSave);
            resources.ApplyResources(this.pnlRightTop, "pnlRightTop");
            this.pnlRightTop.Name = "pnlRightTop";
            // 
            // frmMp3LyricsEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMp3LyricsEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3LyricsEdit_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3LyricsEdit_Load);
            this.Resize += new System.EventHandler(this.frmMp3LyricsEdit_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            this.pnlRightBottom.ResumeLayout(false);
            this.pnlRightTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAsLrc;
        private System.Windows.Forms.ToolStripMenuItem MnuSaveAsText;
        private System.Windows.Forms.ToolStripSeparator mnuFileSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileQuit;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadTrack;
        private System.Windows.Forms.ToolStripSeparator mnuEditSep1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadMelodyText;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadLRCFile;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgView;
        private System.Windows.Forms.Panel pnlRightBottom;
        private System.Windows.Forms.Panel pnlRightTop;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.Button btnDeleteAllLyrics;
        private System.Windows.Forms.Button btnInsertParagraph;
        private System.Windows.Forms.Button btnInsertCr;
        private System.Windows.Forms.Button btnInsertText;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn clTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn clText;
    }
}