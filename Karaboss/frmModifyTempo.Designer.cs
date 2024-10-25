namespace Karaboss
{
    partial class frmModifyTempo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmModifyTempo));
            this.lblTempoNumber = new System.Windows.Forms.Label();
            this.btnNextTempo = new System.Windows.Forms.Button();
            this.btnPrevTempo = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.txtTempo = new System.Windows.Forms.TextBox();
            this.lblDivision = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.txtBpm = new System.Windows.Forms.TextBox();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.lblBpm = new System.Windows.Forms.Label();
            this.updDivision = new System.Windows.Forms.NumericUpDown();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTempoNumber
            // 
            resources.ApplyResources(this.lblTempoNumber, "lblTempoNumber");
            this.lblTempoNumber.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTempoNumber.Name = "lblTempoNumber";
            // 
            // btnNextTempo
            // 
            resources.ApplyResources(this.btnNextTempo, "btnNextTempo");
            this.btnNextTempo.Name = "btnNextTempo";
            this.btnNextTempo.UseVisualStyleBackColor = true;
            this.btnNextTempo.Click += new System.EventHandler(this.btnNextTempo_Click);
            // 
            // btnPrevTempo
            // 
            resources.ApplyResources(this.btnPrevTempo, "btnPrevTempo");
            this.btnPrevTempo.Name = "btnPrevTempo";
            this.btnPrevTempo.UseVisualStyleBackColor = true;
            this.btnPrevTempo.Click += new System.EventHandler(this.btnPrevTempo_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Controls.Add(this.txtTempo);
            this.groupBox1.Controls.Add(this.lblDivision);
            this.groupBox1.Controls.Add(this.lblTempo);
            this.groupBox1.Controls.Add(this.lblStartTime);
            this.groupBox1.Controls.Add(this.txtBpm);
            this.groupBox1.Controls.Add(this.txtStartTime);
            this.groupBox1.Controls.Add(this.lblBpm);
            this.groupBox1.Controls.Add(this.updDivision);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // btnDelete
            // 
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // txtTempo
            // 
            resources.ApplyResources(this.txtTempo, "txtTempo");
            this.txtTempo.Name = "txtTempo";
            this.txtTempo.TextChanged += new System.EventHandler(this.txtTempo_TextChanged);
            this.txtTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTempo_KeyPress);
            // 
            // lblDivision
            // 
            resources.ApplyResources(this.lblDivision, "lblDivision");
            this.lblDivision.Name = "lblDivision";
            // 
            // lblTempo
            // 
            resources.ApplyResources(this.lblTempo, "lblTempo");
            this.lblTempo.Name = "lblTempo";
            // 
            // lblStartTime
            // 
            resources.ApplyResources(this.lblStartTime, "lblStartTime");
            this.lblStartTime.Name = "lblStartTime";
            // 
            // txtBpm
            // 
            resources.ApplyResources(this.txtBpm, "txtBpm");
            this.txtBpm.Name = "txtBpm";
            this.txtBpm.TextChanged += new System.EventHandler(this.txtBpm_TextChanged);
            this.txtBpm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBpm_KeyPress);
            // 
            // txtStartTime
            // 
            resources.ApplyResources(this.txtStartTime, "txtStartTime");
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.TextChanged += new System.EventHandler(this.txtStartTime_TextChanged);
            this.txtStartTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStartTime_KeyPress);
            // 
            // lblBpm
            // 
            resources.ApplyResources(this.lblBpm, "lblBpm");
            this.lblBpm.Name = "lblBpm";
            // 
            // updDivision
            // 
            this.updDivision.Increment = new decimal(new int[] {
            24,
            0,
            0,
            0});
            resources.ApplyResources(this.updDivision, "updDivision");
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
            this.updDivision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDivision_KeyPress);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTitle.Name = "lblTitle";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // frmModifyTempo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblTempoNumber);
            this.Controls.Add(this.btnNextTempo);
            this.Controls.Add(this.btnPrevTempo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmModifyTempo";
            this.TopMost = true;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTempoNumber;
        private System.Windows.Forms.Button btnNextTempo;
        private System.Windows.Forms.Button btnPrevTempo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.TextBox txtTempo;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.TextBox txtBpm;
        private System.Windows.Forms.TextBox txtStartTime;
        private System.Windows.Forms.Label lblBpm;
        private System.Windows.Forms.NumericUpDown updDivision;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
    }
}