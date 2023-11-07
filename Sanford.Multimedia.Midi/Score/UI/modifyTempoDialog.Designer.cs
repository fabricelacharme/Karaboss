namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class modifyTempoDialog
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
            this.txtTempo = new System.Windows.Forms.TextBox();
            this.lblTempo = new System.Windows.Forms.Label();
            this.txtDivision = new System.Windows.Forms.TextBox();
            this.lblDivision = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBpm = new System.Windows.Forms.Label();
            this.txtBpm = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtTempo
            // 
            this.txtTempo.Location = new System.Drawing.Point(77, 59);
            this.txtTempo.Name = "txtTempo";
            this.txtTempo.Size = new System.Drawing.Size(60, 20);
            this.txtTempo.TabIndex = 14;
            this.txtTempo.Text = "500000";
            this.txtTempo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTempo.TextChanged += new System.EventHandler(this.txtTempo_TextChanged);
            this.txtTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTempo_KeyPress);
            // 
            // lblTempo
            // 
            this.lblTempo.AutoSize = true;
            this.lblTempo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempo.Location = new System.Drawing.Point(28, 63);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(43, 13);
            this.lblTempo.TabIndex = 12;
            this.lblTempo.Text = "Tempo:";
            // 
            // txtDivision
            // 
            this.txtDivision.Location = new System.Drawing.Point(197, 59);
            this.txtDivision.Name = "txtDivision";
            this.txtDivision.Size = new System.Drawing.Size(60, 20);
            this.txtDivision.TabIndex = 15;
            this.txtDivision.Text = "480";
            this.txtDivision.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDivision.TextChanged += new System.EventHandler(this.txtDivision_TextChanged);
            this.txtDivision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDivision_KeyPress);
            // 
            // lblDivision
            // 
            this.lblDivision.AutoSize = true;
            this.lblDivision.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDivision.Location = new System.Drawing.Point(150, 63);
            this.lblDivision.Name = "lblDivision";
            this.lblDivision.Size = new System.Drawing.Size(47, 13);
            this.lblDivision.TabIndex = 13;
            this.lblDivision.Text = "Division:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(182, 151);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOk.Location = new System.Drawing.Point(36, 151);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 16;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblTitle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(112, 21);
            this.lblTitle.TabIndex = 18;
            this.lblTitle.Text = "Modify a score";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBpm
            // 
            this.lblBpm.AutoSize = true;
            this.lblBpm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBpm.Location = new System.Drawing.Point(28, 95);
            this.lblBpm.Name = "lblBpm";
            this.lblBpm.Size = new System.Drawing.Size(31, 13);
            this.lblBpm.TabIndex = 20;
            this.lblBpm.Text = "Bpm:";
            // 
            // txtBpm
            // 
            this.txtBpm.Location = new System.Drawing.Point(75, 92);
            this.txtBpm.Name = "txtBpm";
            this.txtBpm.Size = new System.Drawing.Size(60, 20);
            this.txtBpm.TabIndex = 19;
            this.txtBpm.Text = "120";
            this.txtBpm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBpm.TextChanged += new System.EventHandler(this.txtBpm_TextChanged);
            this.txtBpm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBpm_KeyPress);
            // 
            // modifyTempoDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 185);
            this.Controls.Add(this.lblBpm);
            this.Controls.Add(this.txtBpm);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtTempo);
            this.Controls.Add(this.lblTempo);
            this.Controls.Add(this.txtDivision);
            this.Controls.Add(this.lblDivision);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "modifyTempoDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Karaboss - Modify tempo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTempo;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.TextBox txtDivision;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBpm;
        private System.Windows.Forms.TextBox txtBpm;
    }
}