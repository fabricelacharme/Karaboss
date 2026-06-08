namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class frmNoteEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNoteEdit));
            this.btnPnlNoteCancel = new System.Windows.Forms.Button();
            this.btnPnlNoteOk = new System.Windows.Forms.Button();
            this.upDownNoteVelocity = new System.Windows.Forms.NumericUpDown();
            this.lblNoteVelocity = new System.Windows.Forms.Label();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.lblNoteDuration = new System.Windows.Forms.Label();
            this.txtTicks = new System.Windows.Forms.TextBox();
            this.lblNoteTicks = new System.Windows.Forms.Label();
            this.txtTime = new System.Windows.Forms.TextBox();
            this.lbNotelTime = new System.Windows.Forms.Label();
            this.lblNoteString = new System.Windows.Forms.Label();
            this.upDownNoteValue = new System.Windows.Forms.NumericUpDown();
            this.lblNoteValue = new System.Windows.Forms.Label();
            this.lblTrackNumber = new System.Windows.Forms.Label();
            this.lblSelection = new System.Windows.Forms.Label();
            this.btnDefVlocity = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnAddVelocity = new System.Windows.Forms.Button();
            this.txtAddVelocity = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.optFadingThisTrack = new System.Windows.Forms.RadioButton();
            this.optFadingAllTracks = new System.Windows.Forms.RadioButton();
            this.upDownFadingStep = new System.Windows.Forms.NumericUpDown();
            this.lblStep = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRemoveFadingOut = new System.Windows.Forms.Button();
            this.btnSetFadingOut = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStartTicks = new System.Windows.Forms.Label();
            this.optFadingToTicks = new System.Windows.Forms.RadioButton();
            this.txtStartFadingTime = new System.Windows.Forms.TextBox();
            this.optFadingToEndOfSong = new System.Windows.Forms.RadioButton();
            this.lblEndFadingMeasure = new System.Windows.Forms.Label();
            this.lblStartMeasure = new System.Windows.Forms.Label();
            this.txtEndFadingTime = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtPitchBends = new System.Windows.Forms.TextBox();
            this.btnRemovePitchBend = new System.Windows.Forms.Button();
            this.btnSetPitchBend = new System.Windows.Forms.Button();
            this.lblHsPitchBend = new System.Windows.Forms.Label();
            this.hsPitchBend = new ColorSlider.ColorSlider();
            this.chkPitchBend = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteVelocity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteValue)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownFadingStep)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPnlNoteCancel
            // 
            resources.ApplyResources(this.btnPnlNoteCancel, "btnPnlNoteCancel");
            this.btnPnlNoteCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnPnlNoteCancel.Name = "btnPnlNoteCancel";
            this.btnPnlNoteCancel.UseVisualStyleBackColor = true;
            this.btnPnlNoteCancel.Click += new System.EventHandler(this.btnPnlNoteCancel_Click);
            // 
            // btnPnlNoteOk
            // 
            resources.ApplyResources(this.btnPnlNoteOk, "btnPnlNoteOk");
            this.btnPnlNoteOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnPnlNoteOk.Name = "btnPnlNoteOk";
            this.btnPnlNoteOk.UseVisualStyleBackColor = true;
            this.btnPnlNoteOk.Click += new System.EventHandler(this.btnPnlNoteOk_Click);
            // 
            // upDownNoteVelocity
            // 
            resources.ApplyResources(this.upDownNoteVelocity, "upDownNoteVelocity");
            this.upDownNoteVelocity.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.upDownNoteVelocity.Name = "upDownNoteVelocity";
            // 
            // lblNoteVelocity
            // 
            resources.ApplyResources(this.lblNoteVelocity, "lblNoteVelocity");
            this.lblNoteVelocity.ForeColor = System.Drawing.Color.White;
            this.lblNoteVelocity.Name = "lblNoteVelocity";
            // 
            // txtDuration
            // 
            resources.ApplyResources(this.txtDuration, "txtDuration");
            this.txtDuration.Name = "txtDuration";
            // 
            // lblNoteDuration
            // 
            resources.ApplyResources(this.lblNoteDuration, "lblNoteDuration");
            this.lblNoteDuration.ForeColor = System.Drawing.Color.White;
            this.lblNoteDuration.Name = "lblNoteDuration";
            // 
            // txtTicks
            // 
            resources.ApplyResources(this.txtTicks, "txtTicks");
            this.txtTicks.Name = "txtTicks";
            // 
            // lblNoteTicks
            // 
            resources.ApplyResources(this.lblNoteTicks, "lblNoteTicks");
            this.lblNoteTicks.ForeColor = System.Drawing.Color.White;
            this.lblNoteTicks.Name = "lblNoteTicks";
            // 
            // txtTime
            // 
            resources.ApplyResources(this.txtTime, "txtTime");
            this.txtTime.Name = "txtTime";
            // 
            // lbNotelTime
            // 
            resources.ApplyResources(this.lbNotelTime, "lbNotelTime");
            this.lbNotelTime.ForeColor = System.Drawing.Color.White;
            this.lbNotelTime.Name = "lbNotelTime";
            // 
            // lblNoteString
            // 
            resources.ApplyResources(this.lblNoteString, "lblNoteString");
            this.lblNoteString.ForeColor = System.Drawing.Color.White;
            this.lblNoteString.Name = "lblNoteString";
            // 
            // upDownNoteValue
            // 
            resources.ApplyResources(this.upDownNoteValue, "upDownNoteValue");
            this.upDownNoteValue.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.upDownNoteValue.Name = "upDownNoteValue";
            this.upDownNoteValue.ValueChanged += new System.EventHandler(this.upDownNoteValue_ValueChanged);
            // 
            // lblNoteValue
            // 
            resources.ApplyResources(this.lblNoteValue, "lblNoteValue");
            this.lblNoteValue.ForeColor = System.Drawing.Color.White;
            this.lblNoteValue.Name = "lblNoteValue";
            // 
            // lblTrackNumber
            // 
            resources.ApplyResources(this.lblTrackNumber, "lblTrackNumber");
            this.lblTrackNumber.ForeColor = System.Drawing.Color.White;
            this.lblTrackNumber.Name = "lblTrackNumber";
            // 
            // lblSelection
            // 
            resources.ApplyResources(this.lblSelection, "lblSelection");
            this.lblSelection.ForeColor = System.Drawing.Color.White;
            this.lblSelection.Name = "lblSelection";
            // 
            // btnDefVlocity
            // 
            resources.ApplyResources(this.btnDefVlocity, "btnDefVlocity");
            this.btnDefVlocity.Name = "btnDefVlocity";
            this.btnDefVlocity.UseVisualStyleBackColor = true;
            this.btnDefVlocity.Click += new System.EventHandler(this.btnDefVlocity_Click);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.tabPage1.Controls.Add(this.btnAddVelocity);
            this.tabPage1.Controls.Add(this.txtAddVelocity);
            this.tabPage1.Controls.Add(this.btnDefVlocity);
            this.tabPage1.Controls.Add(this.lblNoteValue);
            this.tabPage1.Controls.Add(this.lblSelection);
            this.tabPage1.Controls.Add(this.upDownNoteValue);
            this.tabPage1.Controls.Add(this.lblTrackNumber);
            this.tabPage1.Controls.Add(this.lblNoteString);
            this.tabPage1.Controls.Add(this.btnPnlNoteCancel);
            this.tabPage1.Controls.Add(this.lbNotelTime);
            this.tabPage1.Controls.Add(this.btnPnlNoteOk);
            this.tabPage1.Controls.Add(this.txtTime);
            this.tabPage1.Controls.Add(this.upDownNoteVelocity);
            this.tabPage1.Controls.Add(this.lblNoteTicks);
            this.tabPage1.Controls.Add(this.lblNoteVelocity);
            this.tabPage1.Controls.Add(this.txtTicks);
            this.tabPage1.Controls.Add(this.txtDuration);
            this.tabPage1.Controls.Add(this.lblNoteDuration);
            this.tabPage1.Name = "tabPage1";
            // 
            // btnAddVelocity
            // 
            resources.ApplyResources(this.btnAddVelocity, "btnAddVelocity");
            this.btnAddVelocity.Name = "btnAddVelocity";
            this.btnAddVelocity.UseVisualStyleBackColor = true;
            this.btnAddVelocity.Click += new System.EventHandler(this.btnAddVelocity_Click);
            // 
            // txtAddVelocity
            // 
            resources.ApplyResources(this.txtAddVelocity, "txtAddVelocity");
            this.txtAddVelocity.Name = "txtAddVelocity";
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.upDownFadingStep);
            this.tabPage2.Controls.Add(this.lblStep);
            this.tabPage2.Controls.Add(this.label1);
            this.tabPage2.Controls.Add(this.btnRemoveFadingOut);
            this.tabPage2.Controls.Add(this.btnSetFadingOut);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Name = "tabPage2";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.optFadingThisTrack);
            this.groupBox2.Controls.Add(this.optFadingAllTracks);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // optFadingThisTrack
            // 
            resources.ApplyResources(this.optFadingThisTrack, "optFadingThisTrack");
            this.optFadingThisTrack.ForeColor = System.Drawing.Color.White;
            this.optFadingThisTrack.Name = "optFadingThisTrack";
            this.optFadingThisTrack.UseVisualStyleBackColor = true;
            this.optFadingThisTrack.CheckedChanged += new System.EventHandler(this.optFadingThisTrack_CheckedChanged);
            // 
            // optFadingAllTracks
            // 
            resources.ApplyResources(this.optFadingAllTracks, "optFadingAllTracks");
            this.optFadingAllTracks.Checked = true;
            this.optFadingAllTracks.ForeColor = System.Drawing.Color.White;
            this.optFadingAllTracks.Name = "optFadingAllTracks";
            this.optFadingAllTracks.TabStop = true;
            this.optFadingAllTracks.UseVisualStyleBackColor = true;
            this.optFadingAllTracks.CheckedChanged += new System.EventHandler(this.optFadingAllTracks_CheckedChanged);
            // 
            // upDownFadingStep
            // 
            resources.ApplyResources(this.upDownFadingStep, "upDownFadingStep");
            this.upDownFadingStep.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.upDownFadingStep.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.upDownFadingStep.Name = "upDownFadingStep";
            this.upDownFadingStep.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // lblStep
            // 
            resources.ApplyResources(this.lblStep, "lblStep");
            this.lblStep.ForeColor = System.Drawing.Color.White;
            this.lblStep.Name = "lblStep";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // btnRemoveFadingOut
            // 
            resources.ApplyResources(this.btnRemoveFadingOut, "btnRemoveFadingOut");
            this.btnRemoveFadingOut.Name = "btnRemoveFadingOut";
            this.btnRemoveFadingOut.UseVisualStyleBackColor = true;
            this.btnRemoveFadingOut.Click += new System.EventHandler(this.btnRemoveFadingOut_Click);
            // 
            // btnSetFadingOut
            // 
            resources.ApplyResources(this.btnSetFadingOut, "btnSetFadingOut");
            this.btnSetFadingOut.Name = "btnSetFadingOut";
            this.btnSetFadingOut.UseVisualStyleBackColor = true;
            this.btnSetFadingOut.Click += new System.EventHandler(this.btnSetFadingOut_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.lblStartTicks);
            this.groupBox1.Controls.Add(this.optFadingToTicks);
            this.groupBox1.Controls.Add(this.txtStartFadingTime);
            this.groupBox1.Controls.Add(this.optFadingToEndOfSong);
            this.groupBox1.Controls.Add(this.lblEndFadingMeasure);
            this.groupBox1.Controls.Add(this.lblStartMeasure);
            this.groupBox1.Controls.Add(this.txtEndFadingTime);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblStartTicks
            // 
            resources.ApplyResources(this.lblStartTicks, "lblStartTicks");
            this.lblStartTicks.ForeColor = System.Drawing.Color.White;
            this.lblStartTicks.Name = "lblStartTicks";
            // 
            // optFadingToTicks
            // 
            resources.ApplyResources(this.optFadingToTicks, "optFadingToTicks");
            this.optFadingToTicks.ForeColor = System.Drawing.Color.White;
            this.optFadingToTicks.Name = "optFadingToTicks";
            this.optFadingToTicks.UseVisualStyleBackColor = true;
            // 
            // txtStartFadingTime
            // 
            resources.ApplyResources(this.txtStartFadingTime, "txtStartFadingTime");
            this.txtStartFadingTime.Name = "txtStartFadingTime";
            // 
            // optFadingToEndOfSong
            // 
            resources.ApplyResources(this.optFadingToEndOfSong, "optFadingToEndOfSong");
            this.optFadingToEndOfSong.Checked = true;
            this.optFadingToEndOfSong.ForeColor = System.Drawing.Color.White;
            this.optFadingToEndOfSong.Name = "optFadingToEndOfSong";
            this.optFadingToEndOfSong.TabStop = true;
            this.optFadingToEndOfSong.UseVisualStyleBackColor = true;
            this.optFadingToEndOfSong.CheckedChanged += new System.EventHandler(this.optFadingToEndOfSong_CheckedChanged);
            // 
            // lblEndFadingMeasure
            // 
            resources.ApplyResources(this.lblEndFadingMeasure, "lblEndFadingMeasure");
            this.lblEndFadingMeasure.ForeColor = System.Drawing.Color.White;
            this.lblEndFadingMeasure.Name = "lblEndFadingMeasure";
            // 
            // lblStartMeasure
            // 
            resources.ApplyResources(this.lblStartMeasure, "lblStartMeasure");
            this.lblStartMeasure.ForeColor = System.Drawing.Color.White;
            this.lblStartMeasure.Name = "lblStartMeasure";
            // 
            // txtEndFadingTime
            // 
            resources.ApplyResources(this.txtEndFadingTime, "txtEndFadingTime");
            this.txtEndFadingTime.Name = "txtEndFadingTime";
            // 
            // tabPage3
            // 
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.tabPage3.Controls.Add(this.txtPitchBends);
            this.tabPage3.Controls.Add(this.btnRemovePitchBend);
            this.tabPage3.Controls.Add(this.btnSetPitchBend);
            this.tabPage3.Controls.Add(this.lblHsPitchBend);
            this.tabPage3.Controls.Add(this.hsPitchBend);
            this.tabPage3.Controls.Add(this.chkPitchBend);
            this.tabPage3.Name = "tabPage3";
            // 
            // txtPitchBends
            // 
            resources.ApplyResources(this.txtPitchBends, "txtPitchBends");
            this.txtPitchBends.Name = "txtPitchBends";
            // 
            // btnRemovePitchBend
            // 
            resources.ApplyResources(this.btnRemovePitchBend, "btnRemovePitchBend");
            this.btnRemovePitchBend.Name = "btnRemovePitchBend";
            this.btnRemovePitchBend.UseVisualStyleBackColor = true;
            this.btnRemovePitchBend.Click += new System.EventHandler(this.btnRemovePitchBend_Click);
            // 
            // btnSetPitchBend
            // 
            resources.ApplyResources(this.btnSetPitchBend, "btnSetPitchBend");
            this.btnSetPitchBend.Name = "btnSetPitchBend";
            this.btnSetPitchBend.UseVisualStyleBackColor = true;
            this.btnSetPitchBend.Click += new System.EventHandler(this.btnSetPitchBend_Click);
            // 
            // lblHsPitchBend
            // 
            resources.ApplyResources(this.lblHsPitchBend, "lblHsPitchBend");
            this.lblHsPitchBend.ForeColor = System.Drawing.Color.White;
            this.lblHsPitchBend.Name = "lblHsPitchBend";
            // 
            // hsPitchBend
            // 
            resources.ApplyResources(this.hsPitchBend, "hsPitchBend");
            this.hsPitchBend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.hsPitchBend.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.hsPitchBend.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.hsPitchBend.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.hsPitchBend.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.hsPitchBend.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.hsPitchBend.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.hsPitchBend.ForeColor = System.Drawing.Color.White;
            this.hsPitchBend.LargeChange = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.hsPitchBend.Maximum = new decimal(new int[] {
            8192,
            0,
            0,
            0});
            this.hsPitchBend.Minimum = new decimal(new int[] {
            8192,
            0,
            0,
            -2147483648});
            this.hsPitchBend.MouseWheelBarPartitions = 16;
            this.hsPitchBend.Name = "hsPitchBend";
            this.hsPitchBend.ScaleDivisions = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.hsPitchBend.ScaleSubDivisions = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.hsPitchBend.ShowDivisionsText = false;
            this.hsPitchBend.ShowSmallScale = true;
            this.hsPitchBend.SmallChange = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.hsPitchBend.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.hsPitchBend.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.hsPitchBend.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.hsPitchBend.ThumbSize = new System.Drawing.Size(12, 12);
            this.hsPitchBend.TickAdd = 0F;
            this.hsPitchBend.TickColor = System.Drawing.Color.White;
            this.hsPitchBend.TickDivide = 0F;
            this.hsPitchBend.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.hsPitchBend.ValueChanged += new System.EventHandler(this.hsPitchBend_ValueChanged);
            // 
            // chkPitchBend
            // 
            resources.ApplyResources(this.chkPitchBend, "chkPitchBend");
            this.chkPitchBend.ForeColor = System.Drawing.Color.White;
            this.chkPitchBend.Name = "chkPitchBend";
            this.chkPitchBend.UseVisualStyleBackColor = true;
            this.chkPitchBend.CheckedChanged += new System.EventHandler(this.chkPitchBend_CheckedChanged);
            // 
            // frmNoteEdit
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNoteEdit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNoteEdit_FormClosing);
            this.Load += new System.EventHandler(this.frmNoteEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteVelocity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteValue)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownFadingStep)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPnlNoteCancel;
        private System.Windows.Forms.Button btnPnlNoteOk;
        private System.Windows.Forms.NumericUpDown upDownNoteVelocity;
        private System.Windows.Forms.Label lblNoteVelocity;
        private System.Windows.Forms.TextBox txtDuration;
        private System.Windows.Forms.Label lblNoteDuration;
        private System.Windows.Forms.TextBox txtTicks;
        private System.Windows.Forms.Label lblNoteTicks;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.Label lbNotelTime;
        private System.Windows.Forms.Label lblNoteString;
        private System.Windows.Forms.NumericUpDown upDownNoteValue;
        private System.Windows.Forms.Label lblNoteValue;
        private System.Windows.Forms.Label lblTrackNumber;
        private System.Windows.Forms.Label lblSelection;
        private System.Windows.Forms.Button btnDefVlocity;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.CheckBox chkPitchBend;
        private ColorSlider.ColorSlider hsPitchBend;
        private System.Windows.Forms.Label lblHsPitchBend;
        private System.Windows.Forms.Button btnRemovePitchBend;
        private System.Windows.Forms.Button btnSetPitchBend;
        private System.Windows.Forms.TextBox txtPitchBends;
        private System.Windows.Forms.Button btnAddVelocity;
        private System.Windows.Forms.TextBox txtAddVelocity;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnRemoveFadingOut;
        private System.Windows.Forms.Button btnSetFadingOut;
        private System.Windows.Forms.RadioButton optFadingAllTracks;
        private System.Windows.Forms.RadioButton optFadingThisTrack;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStartTicks;
        private System.Windows.Forms.TextBox txtStartFadingTime;
        private System.Windows.Forms.TextBox txtEndFadingTime;
        private System.Windows.Forms.Label lblStartMeasure;
        private System.Windows.Forms.Label lblEndFadingMeasure;
        private System.Windows.Forms.NumericUpDown upDownFadingStep;
        private System.Windows.Forms.Label lblStep;
        private System.Windows.Forms.RadioButton optFadingToTicks;
        private System.Windows.Forms.RadioButton optFadingToEndOfSong;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}