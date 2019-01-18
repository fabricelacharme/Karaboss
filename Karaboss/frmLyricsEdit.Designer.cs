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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLyricsEdit));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.dTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dReplace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.pnlTop = new System.Windows.Forms.Panel();
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
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLoadTrack = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLoadMelodyText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlBottom);
            this.splitContainer1.Panel2.Controls.Add(this.pnlTop);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dgView
            // 
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dTime,
            this.dType,
            this.dNote,
            this.dText,
            this.dReplace});
            resources.ApplyResources(this.dgView, "dgView");
            this.dgView.Name = "dgView";
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
            // dReplace
            // 
            resources.ApplyResources(this.dReplace, "dReplace");
            this.dReplace.Name = "dReplace";
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.txtResult);
            resources.ApplyResources(this.pnlBottom, "pnlBottom");
            this.pnlBottom.Name = "pnlBottom";
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
            // pnlTop
            // 
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
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.Name = "pnlTop";
            // 
            // BtnFontMoins
            // 
            resources.ApplyResources(this.BtnFontMoins, "BtnFontMoins");
            this.BtnFontMoins.Name = "BtnFontMoins";
            this.BtnFontMoins.UseVisualStyleBackColor = true;
            this.BtnFontMoins.Click += new System.EventHandler(this.BtnFontMoins_Click);
            // 
            // BtnFontPlus
            // 
            resources.ApplyResources(this.BtnFontPlus, "BtnFontPlus");
            this.BtnFontPlus.Name = "BtnFontPlus";
            this.BtnFontPlus.UseVisualStyleBackColor = true;
            this.BtnFontPlus.Click += new System.EventHandler(this.BtnFontPlus_Click);
            // 
            // btnInsertCr
            // 
            resources.ApplyResources(this.btnInsertCr, "btnInsertCr");
            this.btnInsertCr.Name = "btnInsertCr";
            this.btnInsertCr.UseVisualStyleBackColor = true;
            this.btnInsertCr.Click += new System.EventHandler(this.BtnInsert_Click);
            // 
            // btnSpaceRight
            // 
            resources.ApplyResources(this.btnSpaceRight, "btnSpaceRight");
            this.btnSpaceRight.Name = "btnSpaceRight";
            this.btnSpaceRight.UseVisualStyleBackColor = true;
            this.btnSpaceRight.Click += new System.EventHandler(this.BtnSpaceRight_Click);
            // 
            // btnSpaceLeft
            // 
            resources.ApplyResources(this.btnSpaceLeft, "btnSpaceLeft");
            this.btnSpaceLeft.Name = "btnSpaceLeft";
            this.btnSpaceLeft.UseVisualStyleBackColor = true;
            this.btnSpaceLeft.Click += new System.EventHandler(this.BtnSpaceLeft_Click);
            // 
            // optFormatLyrics
            // 
            resources.ApplyResources(this.optFormatLyrics, "optFormatLyrics");
            this.optFormatLyrics.Name = "optFormatLyrics";
            this.optFormatLyrics.UseVisualStyleBackColor = true;
            this.optFormatLyrics.CheckedChanged += new System.EventHandler(this.OptFormatLyrics_CheckedChanged);
            // 
            // btnInsertText
            // 
            resources.ApplyResources(this.btnInsertText, "btnInsertText");
            this.btnInsertText.Name = "btnInsertText";
            this.btnInsertText.UseVisualStyleBackColor = true;
            this.btnInsertText.Click += new System.EventHandler(this.BtnInsertText_Click);
            // 
            // optFormatText
            // 
            resources.ApplyResources(this.optFormatText, "optFormatText");
            this.optFormatText.Checked = true;
            this.optFormatText.Name = "optFormatText";
            this.optFormatText.TabStop = true;
            this.optFormatText.UseVisualStyleBackColor = true;
            this.optFormatText.CheckedChanged += new System.EventHandler(this.OptFormatText_CheckedChanged);
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnPlay
            // 
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // btnView
            // 
            resources.ApplyResources(this.btnView, "btnView");
            this.btnView.Name = "btnView";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.BtnView_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
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
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            resources.ApplyResources(this.mnuFile, "mnuFile");
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
            // mnuFileQuit
            // 
            this.mnuFileQuit.Name = "mnuFileQuit";
            resources.ApplyResources(this.mnuFileQuit, "mnuFileQuit");
            this.mnuFileQuit.Click += new System.EventHandler(this.MnuFileQuit_Click);
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditLoadTrack,
            this.mnuEditLoadMelodyText});
            this.mnuEdit.Name = "mnuEdit";
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            // 
            // mnuEditLoadTrack
            // 
            this.mnuEditLoadTrack.Name = "mnuEditLoadTrack";
            resources.ApplyResources(this.mnuEditLoadTrack, "mnuEditLoadTrack");
            this.mnuEditLoadTrack.Click += new System.EventHandler(this.MnuEditLoadTrack_Click);
            // 
            // mnuEditLoadMelodyText
            // 
            this.mnuEditLoadMelodyText.Name = "mnuEditLoadMelodyText";
            resources.ApplyResources(this.mnuEditLoadMelodyText, "mnuEditLoadMelodyText");
            this.mnuEditLoadMelodyText.Click += new System.EventHandler(this.MnuEditLoadMelodyText_Click);
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
            this.mnuHelpAbout.Click += new System.EventHandler(this.MnuHelpAbout_Click);
            // 
            // frmLyricsEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmLyricsEdit";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn dTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn dType;
        private System.Windows.Forms.DataGridViewTextBoxColumn dNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn dText;
        private System.Windows.Forms.DataGridViewTextBoxColumn dReplace;
        private System.Windows.Forms.RadioButton optFormatText;
        private System.Windows.Forms.RadioButton optFormatLyrics;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button BtnFontMoins;
        private System.Windows.Forms.Button BtnFontPlus;
    }
}