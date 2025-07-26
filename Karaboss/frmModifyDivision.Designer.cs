namespace Karaboss
{
    partial class frmModifyDivision
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModifyDivision));
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblDivision = new System.Windows.Forms.Label();
            this.updDivision = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).BeginInit();
            this.SuspendLayout();
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(137)))), ((int)(((byte)(239)))));
            this.lblTitle.Name = "lblTitle";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblDivision
            // 
            resources.ApplyResources(this.lblDivision, "lblDivision");
            this.lblDivision.ForeColor = System.Drawing.Color.White;
            this.lblDivision.Name = "lblDivision";
            // 
            // updDivision
            // 
            resources.ApplyResources(this.updDivision, "updDivision");
            this.updDivision.Increment = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.updDivision.Maximum = new decimal(new int[] {
            24000,
            0,
            0,
            0});
            this.updDivision.Minimum = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.updDivision.Name = "updDivision";
            this.updDivision.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.updDivision.ValueChanged += new System.EventHandler(this.updDivision_ValueChanged);
            this.updDivision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.updDivision_KeyPress);
            // 
            // frmModifyDivision
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblDivision);
            this.Controls.Add(this.updDivision);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmModifyDivision";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.NumericUpDown updDivision;
    }
}