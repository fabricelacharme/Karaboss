namespace Karaboss
{
    partial class frmKokOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKokOptions));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkRemoveAccents = new System.Windows.Forms.CheckBox();
            this.chkUpperCase = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkLowerCase = new System.Windows.Forms.CheckBox();
            this.chkAlphaNumeric = new System.Windows.Forms.CheckBox();
            this.cbEncoding = new System.Windows.Forms.ComboBox();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.chkLowerCase);
            this.groupBox2.Controls.Add(this.chkAlphaNumeric);
            this.groupBox2.Controls.Add(this.chkRemoveAccents);
            this.groupBox2.Controls.Add(this.chkUpperCase);
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
            // cbEncoding
            // 
            resources.ApplyResources(this.cbEncoding, "cbEncoding");
            this.cbEncoding.FormattingEnabled = true;
            this.cbEncoding.Name = "cbEncoding";
            this.cbEncoding.SelectedIndexChanged += new System.EventHandler(this.cbEncoding_SelectedIndexChanged);
            // 
            // lblEncoding
            // 
            resources.ApplyResources(this.lblEncoding, "lblEncoding");
            this.lblEncoding.Name = "lblEncoding";
            // 
            // frmKokOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblEncoding);
            this.Controls.Add(this.cbEncoding);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmKokOptions";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKokOptions_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkRemoveAccents;
        private System.Windows.Forms.CheckBox chkUpperCase;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAlphaNumeric;
        private System.Windows.Forms.CheckBox chkLowerCase;
        private System.Windows.Forms.ComboBox cbEncoding;
        private System.Windows.Forms.Label lblEncoding;
    }
}