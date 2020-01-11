namespace Karaboss.Pages.ABCnotation
{
    partial class FrmTextPlayer {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.cmbInstruments = new System.Windows.Forms.ComboBox();
            this.chkNormalize = new System.Windows.Forms.CheckBox();
            this.chkLoop = new System.Windows.Forms.CheckBox();            
            this.chkMute = new System.Windows.Forms.CheckBox();
            this.scrSeek = new System.Windows.Forms.ProgressBar();
            this.btnPlay = new Karaboss.NoSelectButton();
            this.btnPause = new Karaboss.NoSelectButton();
            this.btnStop = new Karaboss.NoSelectButton();
            this.lblFile = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMMLMode = new System.Windows.Forms.ComboBox();
            this.chkLotroDetect = new System.Windows.Forms.CheckBox();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlEditText = new System.Windows.Forms.Panel();
            this.txtEditText = new System.Windows.Forms.TextBox();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.nmuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlBottom.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlEditText.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbInstruments
            // 
            this.cmbInstruments.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstruments.FormattingEnabled = true;
            this.cmbInstruments.Location = new System.Drawing.Point(338, 30);
            this.cmbInstruments.Margin = new System.Windows.Forms.Padding(2);
            this.cmbInstruments.Name = "cmbInstruments";
            this.cmbInstruments.Size = new System.Drawing.Size(186, 21);
            this.cmbInstruments.TabIndex = 1;
            this.cmbInstruments.SelectionChangeCommitted += new System.EventHandler(this.cmbInstruments_SelectionChangeCommitted);
            // 
            // chkNormalize
            // 
            this.chkNormalize.AutoSize = true;
            this.chkNormalize.Checked = true;
            this.chkNormalize.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNormalize.Location = new System.Drawing.Point(254, 31);
            this.chkNormalize.Margin = new System.Windows.Forms.Padding(2);
            this.chkNormalize.Name = "chkNormalize";
            this.chkNormalize.Size = new System.Drawing.Size(72, 17);
            this.chkNormalize.TabIndex = 2;
            this.chkNormalize.Text = "Normalize";
            this.chkNormalize.UseVisualStyleBackColor = true;
            this.chkNormalize.CheckedChanged += new System.EventHandler(this.chkNormalize_CheckedChanged);
            // 
            // chkLoop
            // 
            this.chkLoop.AutoSize = true;
            this.chkLoop.Location = new System.Drawing.Point(254, 53);
            this.chkLoop.Margin = new System.Windows.Forms.Padding(2);
            this.chkLoop.Name = "chkLoop";
            this.chkLoop.Size = new System.Drawing.Size(50, 17);
            this.chkLoop.TabIndex = 3;
            this.chkLoop.Text = "Loop";
            this.chkLoop.UseVisualStyleBackColor = true;
            this.chkLoop.CheckedChanged += new System.EventHandler(this.chkLoop_CheckedChanged);
            // 
            // chkMute
            // 
            this.chkMute.AutoSize = true;
            this.chkMute.Location = new System.Drawing.Point(254, 75);
            this.chkMute.Margin = new System.Windows.Forms.Padding(2);
            this.chkMute.Name = "chkMute";
            this.chkMute.Size = new System.Drawing.Size(50, 17);
            this.chkMute.TabIndex = 5;
            this.chkMute.Text = "Mute";
            this.chkMute.UseVisualStyleBackColor = true;
            this.chkMute.CheckedChanged += new System.EventHandler(this.chkMute_CheckedChanged);
            // 
            // scrSeek
            // 
            this.scrSeek.Location = new System.Drawing.Point(10, 31);
            this.scrSeek.Margin = new System.Windows.Forms.Padding(2);
            this.scrSeek.Name = "scrSeek";
            this.scrSeek.Size = new System.Drawing.Size(200, 10);
            this.scrSeek.TabIndex = 6;
            this.scrSeek.MouseDown += new System.Windows.Forms.MouseEventHandler(this.scrSeek_MouseDown);
            // 
            // btnPlay
            // 
            this.btnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPlay.Image = global::Karaboss.Properties.Resources.btn_black_play;
            this.btnPlay.Location = new System.Drawing.Point(10, 48);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(2);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(50, 53);
            this.btnPlay.TabIndex = 7;
            this.btnPlay.TabStop = false;
            this.toolTip1.SetToolTip(this.btnPlay, "Play");
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            this.btnPlay.MouseLeave += new System.EventHandler(this.BtnPlay_MouseLeave);
            this.btnPlay.MouseHover += new System.EventHandler(this.BtnPlay_MouseHover);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnPause.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnPause.Image = global::Karaboss.Properties.Resources.btn_black_pause;
            this.btnPause.Location = new System.Drawing.Point(85, 48);
            this.btnPause.Margin = new System.Windows.Forms.Padding(2);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(50, 53);
            this.btnPause.TabIndex = 4;
            this.btnPause.TabStop = false;
            this.toolTip1.SetToolTip(this.btnPause, "Pause");
            this.btnPause.UseVisualStyleBackColor = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            this.btnPause.MouseLeave += new System.EventHandler(this.BtnPause_MouseLeave);
            this.btnPause.MouseHover += new System.EventHandler(this.BtnPause_MouseHover);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnStop.FlatAppearance.BorderSize = 0;
            this.btnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.btnStop.Image = global::Karaboss.Properties.Resources.btn_black_stop;
            this.btnStop.Location = new System.Drawing.Point(160, 48);
            this.btnStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(50, 53);
            this.btnStop.TabIndex = 8;
            this.btnStop.TabStop = false;
            this.toolTip1.SetToolTip(this.btnStop, "Stop");
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            this.btnStop.MouseLeave += new System.EventHandler(this.BtnStop_MouseLeave);
            this.btnStop.MouseHover += new System.EventHandler(this.BtnStop_MouseHover);
            // 
            // lblFile
            // 
            this.lblFile.AutoEllipsis = true;
            this.lblFile.Location = new System.Drawing.Point(11, 12);
            this.lblFile.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(236, 15);
            this.lblFile.TabIndex = 9;
            this.lblFile.Text = "File: ";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(188, 12);
            this.lblTime.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(48, 13);
            this.lblTime.TabIndex = 10;
            this.lblTime.Text = "--:-- / --:--";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(336, 14);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Instrument:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(336, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "MML Mode:";
            // 
            // cmbMMLMode
            // 
            this.cmbMMLMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMMLMode.FormattingEnabled = true;
            this.cmbMMLMode.Items.AddRange(new object[] {
            "Mabinogi",
            "ArcheAge"});
            this.cmbMMLMode.Location = new System.Drawing.Point(338, 73);
            this.cmbMMLMode.Margin = new System.Windows.Forms.Padding(2);
            this.cmbMMLMode.Name = "cmbMMLMode";
            this.cmbMMLMode.Size = new System.Drawing.Size(186, 21);
            this.cmbMMLMode.TabIndex = 12;
            this.cmbMMLMode.SelectionChangeCommitted += new System.EventHandler(this.cmbMMLMode_SelectionChangeCommitted);
            // 
            // chkLotroDetect
            // 
            this.chkLotroDetect.AutoSize = true;
            this.chkLotroDetect.Checked = true;
            this.chkLotroDetect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLotroDetect.Location = new System.Drawing.Point(542, 30);
            this.chkLotroDetect.Margin = new System.Windows.Forms.Padding(2);
            this.chkLotroDetect.Name = "chkLotroDetect";
            this.chkLotroDetect.Size = new System.Drawing.Size(191, 17);
            this.chkLotroDetect.TabIndex = 14;
            this.chkLotroDetect.Text = "LOTRO song detection (octave fix)";
            this.chkLotroDetect.UseVisualStyleBackColor = true;
            this.chkLotroDetect.CheckedChanged += new System.EventHandler(this.chkLotroDetect_CheckedChanged);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlBottom.Controls.Add(this.lblFile);
            this.pnlBottom.Controls.Add(this.cmbInstruments);
            this.pnlBottom.Controls.Add(this.chkLotroDetect);
            this.pnlBottom.Controls.Add(this.chkNormalize);
            this.pnlBottom.Controls.Add(this.label1);
            this.pnlBottom.Controls.Add(this.chkLoop);
            this.pnlBottom.Controls.Add(this.cmbMMLMode);
            this.pnlBottom.Controls.Add(this.btnPause);
            this.pnlBottom.Controls.Add(this.label3);
            this.pnlBottom.Controls.Add(this.chkMute);
            this.pnlBottom.Controls.Add(this.lblTime);
            this.pnlBottom.Controls.Add(this.scrSeek);
            this.pnlBottom.Controls.Add(this.btnPlay);
            this.pnlBottom.Controls.Add(this.btnStop);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 350);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(834, 110);
            this.pnlBottom.TabIndex = 3;
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlMiddle.Controls.Add(this.pnlEditText);
            this.pnlMiddle.Location = new System.Drawing.Point(0, 74);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(834, 266);
            this.pnlMiddle.TabIndex = 2;
            this.pnlMiddle.Visible = false;
            // 
            // pnlEditText
            // 
            this.pnlEditText.BackColor = System.Drawing.Color.White;
            this.pnlEditText.Controls.Add(this.txtEditText);
            this.pnlEditText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEditText.Location = new System.Drawing.Point(0, 0);
            this.pnlEditText.Name = "pnlEditText";
            this.pnlEditText.Size = new System.Drawing.Size(834, 266);
            this.pnlEditText.TabIndex = 0;
            // 
            // txtEditText
            // 
            this.txtEditText.AcceptsReturn = true;
            this.txtEditText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEditText.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEditText.Location = new System.Drawing.Point(0, 0);
            this.txtEditText.Multiline = true;
            this.txtEditText.Name = "txtEditText";
            this.txtEditText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEditText.Size = new System.Drawing.Size(834, 266);
            this.txtEditText.TabIndex = 0;
            this.txtEditText.Text = "toto";
            this.txtEditText.TextChanged += new System.EventHandler(this.txtEditText_TextChanged);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 24);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(834, 50);
            this.pnlTop.TabIndex = 1;
            this.pnlTop.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nmuFile,
            this.mnuDisplay,
            this.mnuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // nmuFile
            // 
            this.nmuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNew,
            this.mnuFileOpen,
            this.toolStripMenuItem2,
            this.mnuFileSave,
            this.mnuFileSaveAs,
            this.toolStripMenuItem1,
            this.mnuFileExit});
            this.nmuFile.Name = "nmuFile";
            this.nmuFile.Size = new System.Drawing.Size(37, 20);
            this.nmuFile.Text = "&File";
            // 
            // mnuFileNew
            // 
            this.mnuFileNew.Name = "mnuFileNew";
            this.mnuFileNew.Size = new System.Drawing.Size(138, 22);
            this.mnuFileNew.Text = "&New";
            this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(138, 22);
            this.mnuFileOpen.Text = "&Open";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 6);
            // 
            // mnuFileSave
            // 
            this.mnuFileSave.Name = "mnuFileSave";
            this.mnuFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuFileSave.Size = new System.Drawing.Size(138, 22);
            this.mnuFileSave.Text = "&Save";
            this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
            // 
            // mnuFileSaveAs
            // 
            this.mnuFileSaveAs.Name = "mnuFileSaveAs";
            this.mnuFileSaveAs.Size = new System.Drawing.Size(138, 22);
            this.mnuFileSaveAs.Text = "Save as...";
            this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(138, 22);
            this.mnuFileExit.Text = "&Exit";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            // 
            // mnuDisplay
            // 
            this.mnuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplayText});
            this.mnuDisplay.Name = "mnuDisplay";
            this.mnuDisplay.Size = new System.Drawing.Size(57, 20);
            this.mnuDisplay.Text = "&Display";
            // 
            // mnuDisplayText
            // 
            this.mnuDisplayText.Name = "mnuDisplayText";
            this.mnuDisplayText.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.mnuDisplayText.Size = new System.Drawing.Size(120, 22);
            this.mnuDisplayText.Text = "Text";
            this.mnuDisplayText.Click += new System.EventHandler(this.mnuDisplayText_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(116, 22);
            this.mnuHelpAbout.Text = "&About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // FrmTextPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 460);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "FrmTextPlayer";
            this.Text = "MidiPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTextPlayer_FormClosing);
            this.Load += new System.EventHandler(this.FrmTextPlayer_Load);
            this.Resize += new System.EventHandler(this.FrmTextPlayer_Resize);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.pnlEditText.ResumeLayout(false);
            this.pnlEditText.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbInstruments;
        private System.Windows.Forms.CheckBox chkNormalize;
        private System.Windows.Forms.CheckBox chkLoop;
        private NoSelectButton btnPause;
        private System.Windows.Forms.CheckBox chkMute;
        private System.Windows.Forms.ProgressBar scrSeek;
        private NoSelectButton btnPlay;
        private NoSelectButton btnStop;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMMLMode;
        private System.Windows.Forms.CheckBox chkLotroDetect;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nmuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayText;
        private System.Windows.Forms.Panel pnlEditText;
        private System.Windows.Forms.TextBox txtEditText;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSave;
        private System.Windows.Forms.ToolStripMenuItem mnuFileSaveAs;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNew;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

