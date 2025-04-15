namespace Karaboss
{
    partial class frmLyricsSelectTrack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLyricsSelectTrack));
            this.cbSelectTrack = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblSelectLyricFormat = new System.Windows.Forms.Label();
            this.optTextFormat = new System.Windows.Forms.RadioButton();
            this.optLyricFormat = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // cbSelectTrack
            // 
            this.cbSelectTrack.FormattingEnabled = true;
            resources.ApplyResources(this.cbSelectTrack, "cbSelectTrack");
            this.cbSelectTrack.Name = "cbSelectTrack";
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblHelp
            // 
            resources.ApplyResources(this.lblHelp, "lblHelp");
            this.lblHelp.Name = "lblHelp";
            // 
            // lblSelectLyricFormat
            // 
            resources.ApplyResources(this.lblSelectLyricFormat, "lblSelectLyricFormat");
            this.lblSelectLyricFormat.Name = "lblSelectLyricFormat";
            // 
            // optTextFormat
            // 
            resources.ApplyResources(this.optTextFormat, "optTextFormat");
            this.optTextFormat.Name = "optTextFormat";
            this.optTextFormat.UseVisualStyleBackColor = true;
            // 
            // optLyricFormat
            // 
            resources.ApplyResources(this.optLyricFormat, "optLyricFormat");
            this.optLyricFormat.Checked = true;
            this.optLyricFormat.Name = "optLyricFormat";
            this.optLyricFormat.TabStop = true;
            this.optLyricFormat.UseVisualStyleBackColor = true;
            // 
            // frmLyricsSelectTrack
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.optLyricFormat);
            this.Controls.Add(this.optTextFormat);
            this.Controls.Add(this.lblSelectLyricFormat);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cbSelectTrack);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLyricsSelectTrack";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmSelectTrack_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSelectTrack;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Label lblSelectLyricFormat;
        private System.Windows.Forms.RadioButton optTextFormat;
        private System.Windows.Forms.RadioButton optLyricFormat;
    }
}