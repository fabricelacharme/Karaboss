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
            this.lblDivision = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBpm = new System.Windows.Forms.Label();
            this.txtBpm = new System.Windows.Forms.TextBox();
            this.updDivision = new System.Windows.Forms.NumericUpDown();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnPrevTempo = new System.Windows.Forms.Button();
            this.btnNextTempo = new System.Windows.Forms.Button();
            this.lblTempoNumber = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtTempo
            // 
            this.txtTempo.Location = new System.Drawing.Point(58, 19);
            this.txtTempo.Name = "txtTempo";
            this.txtTempo.Size = new System.Drawing.Size(60, 20);
            this.txtTempo.TabIndex = 2;
            this.txtTempo.Text = "500000";
            this.txtTempo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTempo.TextChanged += new System.EventHandler(this.txtTempo_TextChanged);
            this.txtTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTempo_KeyPress);
            // 
            // lblTempo
            // 
            this.lblTempo.AutoSize = true;
            this.lblTempo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempo.Location = new System.Drawing.Point(9, 23);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(43, 13);
            this.lblTempo.TabIndex = 1;
            this.lblTempo.Text = "Tempo:";
            // 
            // lblDivision
            // 
            this.lblDivision.AutoSize = true;
            this.lblDivision.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDivision.Location = new System.Drawing.Point(131, 23);
            this.lblDivision.Name = "lblDivision";
            this.lblDivision.Size = new System.Drawing.Size(47, 13);
            this.lblDivision.TabIndex = 3;
            this.lblDivision.Text = "Division:";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCancel.Location = new System.Drawing.Point(175, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnOk.Location = new System.Drawing.Point(24, 210);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 9;
            this.btnOk.Text = "&Update";
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
            this.lblTitle.Size = new System.Drawing.Size(176, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Manage tempo changes";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBpm
            // 
            this.lblBpm.AutoSize = true;
            this.lblBpm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblBpm.Location = new System.Drawing.Point(9, 55);
            this.lblBpm.Name = "lblBpm";
            this.lblBpm.Size = new System.Drawing.Size(31, 13);
            this.lblBpm.TabIndex = 5;
            this.lblBpm.Text = "Bpm:";
            // 
            // txtBpm
            // 
            this.txtBpm.Location = new System.Drawing.Point(69, 52);
            this.txtBpm.Name = "txtBpm";
            this.txtBpm.Size = new System.Drawing.Size(47, 20);
            this.txtBpm.TabIndex = 6;
            this.txtBpm.Text = "120";
            this.txtBpm.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtBpm.TextChanged += new System.EventHandler(this.txtBpm_TextChanged);
            this.txtBpm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBpm_KeyPress);
            // 
            // updDivision
            // 
            this.updDivision.Increment = new decimal(new int[] {
            24,
            0,
            0,
            0});
            this.updDivision.Location = new System.Drawing.Point(178, 21);
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
            this.updDivision.Size = new System.Drawing.Size(60, 20);
            this.updDivision.TabIndex = 4;
            this.updDivision.Value = new decimal(new int[] {
            480,
            0,
            0,
            0});
            this.updDivision.ValueChanged += new System.EventHandler(this.updDivision_ValueChanged);
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblStartTime.Location = new System.Drawing.Point(9, 88);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(54, 13);
            this.lblStartTime.TabIndex = 7;
            this.lblStartTime.Text = "Start time:";
            // 
            // txtStartTime
            // 
            this.txtStartTime.Location = new System.Drawing.Point(69, 85);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(47, 20);
            this.txtStartTime.TabIndex = 8;
            this.txtStartTime.Text = "-1";
            this.txtStartTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStartTime.TextChanged += new System.EventHandler(this.txtStartTime_TextChanged);
            this.txtStartTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStartTime_KeyPress);
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
            this.groupBox1.Location = new System.Drawing.Point(12, 80);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 120);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(163, 83);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnPrevTempo
            // 
            this.btnPrevTempo.Location = new System.Drawing.Point(24, 44);
            this.btnPrevTempo.Name = "btnPrevTempo";
            this.btnPrevTempo.Size = new System.Drawing.Size(50, 23);
            this.btnPrevTempo.TabIndex = 13;
            this.btnPrevTempo.Text = "<<";
            this.btnPrevTempo.UseVisualStyleBackColor = true;
            this.btnPrevTempo.Click += new System.EventHandler(this.btnPrevTempo_Click);
            // 
            // btnNextTempo
            // 
            this.btnNextTempo.Location = new System.Drawing.Point(200, 44);
            this.btnNextTempo.Name = "btnNextTempo";
            this.btnNextTempo.Size = new System.Drawing.Size(50, 23);
            this.btnNextTempo.TabIndex = 14;
            this.btnNextTempo.Text = ">>";
            this.btnNextTempo.UseVisualStyleBackColor = true;
            this.btnNextTempo.Click += new System.EventHandler(this.btnNextTempo_Click);
            // 
            // lblTempoNumber
            // 
            this.lblTempoNumber.AutoSize = true;
            this.lblTempoNumber.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempoNumber.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTempoNumber.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempoNumber.Location = new System.Drawing.Point(95, 48);
            this.lblTempoNumber.Name = "lblTempoNumber";
            this.lblTempoNumber.Size = new System.Drawing.Size(48, 17);
            this.lblTempoNumber.TabIndex = 15;
            this.lblTempoNumber.Text = "Tempo";
            this.lblTempoNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // modifyTempoDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 241);
            this.Controls.Add(this.lblTempoNumber);
            this.Controls.Add(this.btnNextTempo);
            this.Controls.Add(this.btnPrevTempo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "modifyTempoDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Karaboss - Modify tempo";
            ((System.ComponentModel.ISupportInitialize)(this.updDivision)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtTempo;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBpm;
        private System.Windows.Forms.TextBox txtBpm;
        private System.Windows.Forms.NumericUpDown updDivision;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.TextBox txtStartTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnPrevTempo;
        private System.Windows.Forms.Button btnNextTempo;
        private System.Windows.Forms.Label lblTempoNumber;
    }
}