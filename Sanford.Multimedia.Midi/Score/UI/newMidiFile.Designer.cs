namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class frmNewMidiFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewMidiFile));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtDivision = new System.Windows.Forms.TextBox();
            this.lblDivision = new System.Windows.Forms.Label();
            this.txtTempo = new System.Windows.Forms.TextBox();
            this.lblTempo = new System.Windows.Forms.Label();
            this.lblMeasures = new System.Windows.Forms.Label();
            this.updMeasures = new System.Windows.Forms.NumericUpDown();
            this.lblHelpTime = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.updNumerator = new System.Windows.Forms.NumericUpDown();
            this.updDenominator = new System.Windows.Forms.NumericUpDown();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtBpm = new System.Windows.Forms.TextBox();
            this.lblBpm = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updMeasures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updNumerator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updDenominator)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtDivision
            // 
            resources.ApplyResources(this.txtDivision, "txtDivision");
            this.txtDivision.Name = "txtDivision";
            this.txtDivision.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDivision_KeyPress);
            // 
            // lblDivision
            // 
            resources.ApplyResources(this.lblDivision, "lblDivision");
            this.lblDivision.Name = "lblDivision";
            // 
            // txtTempo
            // 
            resources.ApplyResources(this.txtTempo, "txtTempo");
            this.txtTempo.Name = "txtTempo";
            this.txtTempo.TextChanged += new System.EventHandler(this.txtTempo_TextChanged);
            this.txtTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTempo_KeyPress);
            // 
            // lblTempo
            // 
            resources.ApplyResources(this.lblTempo, "lblTempo");
            this.lblTempo.Name = "lblTempo";
            // 
            // lblMeasures
            // 
            resources.ApplyResources(this.lblMeasures, "lblMeasures");
            this.lblMeasures.Name = "lblMeasures";
            // 
            // updMeasures
            // 
            resources.ApplyResources(this.updMeasures, "updMeasures");
            this.updMeasures.Name = "updMeasures";
            this.updMeasures.Value = new decimal(new int[] {
            35,
            0,
            0,
            0});
            // 
            // lblHelpTime
            // 
            resources.ApplyResources(this.lblHelpTime, "lblHelpTime");
            this.lblHelpTime.Name = "lblHelpTime";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // updNumerator
            // 
            resources.ApplyResources(this.updNumerator, "updNumerator");
            this.updNumerator.Name = "updNumerator";
            this.updNumerator.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.updNumerator.ValueChanged += new System.EventHandler(this.updNumerator_ValueChanged);
            // 
            // updDenominator
            // 
            resources.ApplyResources(this.updDenominator, "updDenominator");
            this.updDenominator.Name = "updDenominator";
            this.updDenominator.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.updDenominator.ValueChanged += new System.EventHandler(this.updDenominator_ValueChanged);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTitle.Name = "lblTitle";
            // 
            // txtBpm
            // 
            resources.ApplyResources(this.txtBpm, "txtBpm");
            this.txtBpm.Name = "txtBpm";
            this.txtBpm.TextChanged += new System.EventHandler(this.txtBpm_TextChanged);
            this.txtBpm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBpm_KeyPress);
            // 
            // lblBpm
            // 
            resources.ApplyResources(this.lblBpm, "lblBpm");
            this.lblBpm.Name = "lblBpm";
            // 
            // frmNewMidiFile
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblBpm);
            this.Controls.Add(this.txtBpm);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.updDenominator);
            this.Controls.Add(this.updNumerator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblHelpTime);
            this.Controls.Add(this.updMeasures);
            this.Controls.Add(this.lblMeasures);
            this.Controls.Add(this.txtTempo);
            this.Controls.Add(this.lblTempo);
            this.Controls.Add(this.txtDivision);
            this.Controls.Add(this.lblDivision);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewMidiFile";
            ((System.ComponentModel.ISupportInitialize)(this.updMeasures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updNumerator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updDenominator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtDivision;
        private System.Windows.Forms.Label lblDivision;
        private System.Windows.Forms.TextBox txtTempo;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Label lblMeasures;
        private System.Windows.Forms.NumericUpDown updMeasures;
        private System.Windows.Forms.Label lblHelpTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown updNumerator;
        private System.Windows.Forms.NumericUpDown updDenominator;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtBpm;
        private System.Windows.Forms.Label lblBpm;
    }
}