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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgView = new System.Windows.Forms.DataGridView();
            this.clTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlRightBottom = new System.Windows.Forms.Panel();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.pnlRightTop = new System.Windows.Forms.Panel();
            this.BtnFontMoins = new System.Windows.Forms.Button();
            this.BtnFontPlus = new System.Windows.Forms.Button();
            this.btnDeleteAllLyrics = new System.Windows.Forms.Button();
            this.btnInsertParagraph = new System.Windows.Forms.Button();
            this.btnInsertCr = new System.Windows.Forms.Button();
            this.btnInsertText = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSaveAsLrc = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditLoadLRCFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblLyricsOrigin = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).BeginInit();
            this.pnlRightBottom.SuspendLayout();
            this.pnlRightTop.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
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
            this.splitContainer1.Panel2.Controls.Add(this.pnlRightBottom);
            this.splitContainer1.Panel2.Controls.Add(this.pnlRightTop);
            this.toolTip1.SetToolTip(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.ToolTip"));
            this.toolTip1.SetToolTip(this.splitContainer1, resources.GetString("splitContainer1.ToolTip"));
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dgView
            // 
            resources.ApplyResources(this.dgView, "dgView");
            this.dgView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clTime,
            this.clText});
            this.dgView.Name = "dgView";
            this.toolTip1.SetToolTip(this.dgView, resources.GetString("dgView.ToolTip"));
            this.dgView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellEndEdit);
            this.dgView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgView_CellEnter);
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
            resources.ApplyResources(this.pnlRightBottom, "pnlRightBottom");
            this.pnlRightBottom.Controls.Add(this.txtResult);
            this.pnlRightBottom.Name = "pnlRightBottom";
            this.toolTip1.SetToolTip(this.pnlRightBottom, resources.GetString("pnlRightBottom.ToolTip"));
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
            // pnlRightTop
            // 
            resources.ApplyResources(this.pnlRightTop, "pnlRightTop");
            this.pnlRightTop.Controls.Add(this.BtnFontMoins);
            this.pnlRightTop.Controls.Add(this.BtnFontPlus);
            this.pnlRightTop.Controls.Add(this.btnDeleteAllLyrics);
            this.pnlRightTop.Controls.Add(this.btnInsertParagraph);
            this.pnlRightTop.Controls.Add(this.btnInsertCr);
            this.pnlRightTop.Controls.Add(this.btnInsertText);
            this.pnlRightTop.Controls.Add(this.btnDelete);
            this.pnlRightTop.Controls.Add(this.btnSave);
            this.pnlRightTop.Name = "pnlRightTop";
            this.toolTip1.SetToolTip(this.pnlRightTop, resources.GetString("pnlRightTop.ToolTip"));
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
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Image = global::Karaboss.Properties.Resources.floppy_icon;
            this.btnSave.Name = "btnSave";
            this.toolTip1.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
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
            this.mnuFileSep2,
            this.mnuFileSaveAsLrc,
            this.mnuFileSep1,
            this.mnuFileQuit});
            this.mnuFile.Name = "mnuFile";
            // 
            // mnuFileSave
            // 
            resources.ApplyResources(this.mnuFileSave, "mnuFileSave");
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSep2
            // 
            resources.ApplyResources(this.mnuFileSep2, "mnuFileSep2");
            this.mnuFileSep2.Name = "mnuFileSep2";
            // 
            // mnuFileSaveAsLrc
            // 
            resources.ApplyResources(this.mnuFileSaveAsLrc, "mnuFileSaveAsLrc");
            this.mnuFileSaveAsLrc.Name = "mnuFileSaveAsLrc";
            this.mnuFileSaveAsLrc.Click += new System.EventHandler(this.mnuFileSaveAsLrc_Click);
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
            this.mnuFileQuit.Click += new System.EventHandler(this.mnuFileQuit_Click);
            // 
            // mnuEdit
            // 
            resources.ApplyResources(this.mnuEdit, "mnuEdit");
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditLoadLRCFile});
            this.mnuEdit.Name = "mnuEdit";
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
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // pnlMiddle
            // 
            resources.ApplyResources(this.pnlMiddle, "pnlMiddle");
            this.pnlMiddle.Controls.Add(this.splitContainer1);
            this.pnlMiddle.Name = "pnlMiddle";
            this.toolTip1.SetToolTip(this.pnlMiddle, resources.GetString("pnlMiddle.ToolTip"));
            // 
            // pnlTop
            // 
            resources.ApplyResources(this.pnlTop, "pnlTop");
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.lblLyricsOrigin);
            this.pnlTop.Name = "pnlTop";
            this.toolTip1.SetToolTip(this.pnlTop, resources.GetString("pnlTop.ToolTip"));
            // 
            // lblLyricsOrigin
            // 
            resources.ApplyResources(this.lblLyricsOrigin, "lblLyricsOrigin");
            this.lblLyricsOrigin.ForeColor = System.Drawing.Color.White;
            this.lblLyricsOrigin.Name = "lblLyricsOrigin";
            this.toolTip1.SetToolTip(this.lblLyricsOrigin, resources.GetString("lblLyricsOrigin.ToolTip"));
            // 
            // openFileDialog
            // 
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // frmMp3LyricsEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.menuStrip1);
            this.Name = "frmMp3LyricsEdit";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3LyricsEdit_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3LyricsEdit_Load);
            this.Resize += new System.EventHandler(this.frmMp3LyricsEdit_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgView)).EndInit();
            this.pnlRightBottom.ResumeLayout(false);
            this.pnlRightTop.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditLoadLRCFile;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblLyricsOrigin;
        private System.Windows.Forms.Button BtnFontMoins;
        private System.Windows.Forms.Button BtnFontPlus;
    }
}