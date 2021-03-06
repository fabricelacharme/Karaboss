﻿namespace Sanford.Multimedia.Midi.Score.UI
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
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteVelocity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNoteValue)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPnlNoteCancel
            // 
            this.btnPnlNoteCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnPnlNoteCancel, "btnPnlNoteCancel");
            this.btnPnlNoteCancel.Name = "btnPnlNoteCancel";
            this.btnPnlNoteCancel.UseVisualStyleBackColor = true;
            this.btnPnlNoteCancel.Click += new System.EventHandler(this.btnPnlNoteCancel_Click);
            // 
            // btnPnlNoteOk
            // 
            this.btnPnlNoteOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnPnlNoteOk, "btnPnlNoteOk");
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
            this.lbNotelTime.Name = "lbNotelTime";
            // 
            // lblNoteString
            // 
            resources.ApplyResources(this.lblNoteString, "lblNoteString");
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
            this.lblNoteValue.Name = "lblNoteValue";
            // 
            // lblTrackNumber
            // 
            resources.ApplyResources(this.lblTrackNumber, "lblTrackNumber");
            this.lblTrackNumber.Name = "lblTrackNumber";
            // 
            // lblSelection
            // 
            resources.ApplyResources(this.lblSelection, "lblSelection");
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
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
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBox1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
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
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}