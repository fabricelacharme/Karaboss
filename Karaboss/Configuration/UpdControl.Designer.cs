namespace Karaboss.Configuration
{
    partial class UpdControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdControl));
            this.chkUpdateProgram = new System.Windows.Forms.CheckBox();
            this.lblWebSite = new System.Windows.Forms.Label();
            this.txtWebSite = new System.Windows.Forms.TextBox();
            this.lblUpdFreq = new System.Windows.Forms.Label();
            this.chkUpdFreq = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkUpdateProgram
            // 
            resources.ApplyResources(this.chkUpdateProgram, "chkUpdateProgram");
            this.chkUpdateProgram.Name = "chkUpdateProgram";
            this.chkUpdateProgram.UseVisualStyleBackColor = true;
            // 
            // lblWebSite
            // 
            resources.ApplyResources(this.lblWebSite, "lblWebSite");
            this.lblWebSite.Name = "lblWebSite";
            // 
            // txtWebSite
            // 
            resources.ApplyResources(this.txtWebSite, "txtWebSite");
            this.txtWebSite.Name = "txtWebSite";
            // 
            // lblUpdFreq
            // 
            resources.ApplyResources(this.lblUpdFreq, "lblUpdFreq");
            this.lblUpdFreq.Name = "lblUpdFreq";
            // 
            // chkUpdFreq
            // 
            this.chkUpdFreq.FormattingEnabled = true;
            this.chkUpdFreq.Items.AddRange(new object[] {
            resources.GetString("chkUpdFreq.Items"),
            resources.GetString("chkUpdFreq.Items1"),
            resources.GetString("chkUpdFreq.Items2")});
            resources.ApplyResources(this.chkUpdFreq, "chkUpdFreq");
            this.chkUpdFreq.Name = "chkUpdFreq";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // UpdControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.chkUpdFreq);
            this.Controls.Add(this.lblUpdFreq);
            this.Controls.Add(this.txtWebSite);
            this.Controls.Add(this.lblWebSite);
            this.Controls.Add(this.chkUpdateProgram);
            this.Name = "UpdControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkUpdateProgram;
        private System.Windows.Forms.Label lblWebSite;
        private System.Windows.Forms.TextBox txtWebSite;
        private System.Windows.Forms.Label lblUpdFreq;
        private System.Windows.Forms.ComboBox chkUpdFreq;
        private System.Windows.Forms.Label lblDescription;
    }
}
