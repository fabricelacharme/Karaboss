namespace Karaboss
{
    partial class frmLrcOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLrcOptions));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.OptFormatLines = new System.Windows.Forms.RadioButton();
            this.OptFormatSyllabes = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCutLines = new System.Windows.Forms.Label();
            this.UpdCutLines = new System.Windows.Forms.NumericUpDown();
            this.chkCutLines = new System.Windows.Forms.CheckBox();
            this.chkRemoveAccents = new System.Windows.Forms.CheckBox();
            this.chkUpperCase = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkLowerCase = new System.Windows.Forms.CheckBox();
            this.chkAlphaNumeric = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.OptFormat2Digits = new System.Windows.Forms.RadioButton();
            this.OptFormat3Digits = new System.Windows.Forms.RadioButton();
            this.chkMetadata = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdCutLines)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // OptFormatLines
            // 
            resources.ApplyResources(this.OptFormatLines, "OptFormatLines");
            this.OptFormatLines.Name = "OptFormatLines";
            this.OptFormatLines.TabStop = true;
            this.OptFormatLines.UseVisualStyleBackColor = true;
            this.OptFormatLines.CheckedChanged += new System.EventHandler(this.OptFormatLines_CheckedChanged);
            // 
            // OptFormatSyllabes
            // 
            resources.ApplyResources(this.OptFormatSyllabes, "OptFormatSyllabes");
            this.OptFormatSyllabes.Checked = true;
            this.OptFormatSyllabes.Name = "OptFormatSyllabes";
            this.OptFormatSyllabes.TabStop = true;
            this.OptFormatSyllabes.UseVisualStyleBackColor = true;
            this.OptFormatSyllabes.CheckedChanged += new System.EventHandler(this.OptFormatSyllabes_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCutLines);
            this.groupBox1.Controls.Add(this.UpdCutLines);
            this.groupBox1.Controls.Add(this.chkCutLines);
            this.groupBox1.Controls.Add(this.OptFormatLines);
            this.groupBox1.Controls.Add(this.OptFormatSyllabes);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // lblCutLines
            // 
            resources.ApplyResources(this.lblCutLines, "lblCutLines");
            this.lblCutLines.Name = "lblCutLines";
            // 
            // UpdCutLines
            // 
            resources.ApplyResources(this.UpdCutLines, "UpdCutLines");
            this.UpdCutLines.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.UpdCutLines.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.UpdCutLines.Name = "UpdCutLines";
            this.UpdCutLines.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // chkCutLines
            // 
            resources.ApplyResources(this.chkCutLines, "chkCutLines");
            this.chkCutLines.Name = "chkCutLines";
            this.chkCutLines.UseVisualStyleBackColor = true;
            this.chkCutLines.CheckedChanged += new System.EventHandler(this.chkCutLines_CheckedChanged);
            // 
            // chkRemoveAccents
            // 
            resources.ApplyResources(this.chkRemoveAccents, "chkRemoveAccents");
            this.chkRemoveAccents.Name = "chkRemoveAccents";
            this.chkRemoveAccents.UseVisualStyleBackColor = true;
            // 
            // chkUpperCase
            // 
            resources.ApplyResources(this.chkUpperCase, "chkUpperCase");
            this.chkUpperCase.Name = "chkUpperCase";
            this.chkUpperCase.UseVisualStyleBackColor = true;
            this.chkUpperCase.CheckedChanged += new System.EventHandler(this.chkUpperCase_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkLowerCase);
            this.groupBox2.Controls.Add(this.chkAlphaNumeric);
            this.groupBox2.Controls.Add(this.chkRemoveAccents);
            this.groupBox2.Controls.Add(this.chkUpperCase);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkLowerCase
            // 
            resources.ApplyResources(this.chkLowerCase, "chkLowerCase");
            this.chkLowerCase.Name = "chkLowerCase";
            this.chkLowerCase.UseVisualStyleBackColor = true;
            this.chkLowerCase.CheckedChanged += new System.EventHandler(this.chkLowerCase_CheckedChanged);
            // 
            // chkAlphaNumeric
            // 
            resources.ApplyResources(this.chkAlphaNumeric, "chkAlphaNumeric");
            this.chkAlphaNumeric.Name = "chkAlphaNumeric";
            this.chkAlphaNumeric.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.OptFormat2Digits);
            this.groupBox3.Controls.Add(this.OptFormat3Digits);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // OptFormat2Digits
            // 
            resources.ApplyResources(this.OptFormat2Digits, "OptFormat2Digits");
            this.OptFormat2Digits.Checked = true;
            this.OptFormat2Digits.Name = "OptFormat2Digits";
            this.OptFormat2Digits.TabStop = true;
            this.OptFormat2Digits.UseVisualStyleBackColor = true;
            this.OptFormat2Digits.CheckedChanged += new System.EventHandler(this.OptFormat2Digits_CheckedChanged);
            // 
            // OptFormat3Digits
            // 
            resources.ApplyResources(this.OptFormat3Digits, "OptFormat3Digits");
            this.OptFormat3Digits.Name = "OptFormat3Digits";
            this.OptFormat3Digits.UseVisualStyleBackColor = true;
            this.OptFormat3Digits.CheckedChanged += new System.EventHandler(this.OptFormat3Digits_CheckedChanged);
            // 
            // chkMetadata
            // 
            resources.ApplyResources(this.chkMetadata, "chkMetadata");
            this.chkMetadata.Name = "chkMetadata";
            this.chkMetadata.UseVisualStyleBackColor = true;
            this.chkMetadata.CheckedChanged += new System.EventHandler(this.chkMetadata_CheckedChanged);
            // 
            // frmLrcOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.chkMetadata);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLrcOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLrcOptions_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UpdCutLines)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton OptFormatLines;
        private System.Windows.Forms.RadioButton OptFormatSyllabes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkRemoveAccents;
        private System.Windows.Forms.CheckBox chkUpperCase;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAlphaNumeric;
        private System.Windows.Forms.Label lblCutLines;
        private System.Windows.Forms.NumericUpDown UpdCutLines;
        private System.Windows.Forms.CheckBox chkCutLines;
        private System.Windows.Forms.CheckBox chkLowerCase;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton OptFormat2Digits;
        private System.Windows.Forms.RadioButton OptFormat3Digits;
        private System.Windows.Forms.CheckBox chkMetadata;
    }
}