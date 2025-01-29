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
            this.chkRemoveAccents = new System.Windows.Forms.CheckBox();
            this.chkUpperCase = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAlphaNumeric = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // 
            // OptFormatSyllabes
            // 
            resources.ApplyResources(this.OptFormatSyllabes, "OptFormatSyllabes");
            this.OptFormatSyllabes.Checked = true;
            this.OptFormatSyllabes.Name = "OptFormatSyllabes";
            this.OptFormatSyllabes.TabStop = true;
            this.OptFormatSyllabes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.OptFormatLines);
            this.groupBox1.Controls.Add(this.OptFormatSyllabes);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
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
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkAlphaNumeric);
            this.groupBox2.Controls.Add(this.chkRemoveAccents);
            this.groupBox2.Controls.Add(this.chkUpperCase);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chkAlphaNumeric
            // 
            resources.ApplyResources(this.chkAlphaNumeric, "chkAlphaNumeric");
            this.chkAlphaNumeric.Name = "chkAlphaNumeric";
            this.chkAlphaNumeric.UseVisualStyleBackColor = true;
            // 
            // frmLrcOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

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
    }
}