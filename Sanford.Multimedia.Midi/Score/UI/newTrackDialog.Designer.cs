namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class frmNewTrackDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewTrackDialog));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbChannels = new System.Windows.Forms.ComboBox();
            this.lblChannels = new System.Windows.Forms.Label();
            this.lblTrackName = new System.Windows.Forms.Label();
            this.txtTrackName = new System.Windows.Forms.TextBox();
            this.lblInstruments = new System.Windows.Forms.Label();
            this.cbInstruments = new System.Windows.Forms.ComboBox();
            this.lblClef = new System.Windows.Forms.Label();
            this.cbClef = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.updIndex = new System.Windows.Forms.NumericUpDown();
            this.lblIndex = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbChannels
            // 
            this.cbChannels.FormattingEnabled = true;
            resources.ApplyResources(this.cbChannels, "cbChannels");
            this.cbChannels.Name = "cbChannels";
            this.cbChannels.SelectedIndexChanged += new System.EventHandler(this.cbChannels_SelectedIndexChanged);
            // 
            // lblChannels
            // 
            resources.ApplyResources(this.lblChannels, "lblChannels");
            this.lblChannels.Name = "lblChannels";
            // 
            // lblTrackName
            // 
            resources.ApplyResources(this.lblTrackName, "lblTrackName");
            this.lblTrackName.Name = "lblTrackName";
            // 
            // txtTrackName
            // 
            resources.ApplyResources(this.txtTrackName, "txtTrackName");
            this.txtTrackName.Name = "txtTrackName";
            // 
            // lblInstruments
            // 
            resources.ApplyResources(this.lblInstruments, "lblInstruments");
            this.lblInstruments.Name = "lblInstruments";
            // 
            // cbInstruments
            // 
            this.cbInstruments.FormattingEnabled = true;
            resources.ApplyResources(this.cbInstruments, "cbInstruments");
            this.cbInstruments.Name = "cbInstruments";
            this.cbInstruments.SelectedIndexChanged += new System.EventHandler(this.cbInstruments_SelectedIndexChanged);
            // 
            // lblClef
            // 
            resources.ApplyResources(this.lblClef, "lblClef");
            this.lblClef.Name = "lblClef";
            // 
            // cbClef
            // 
            this.cbClef.FormattingEnabled = true;
            this.cbClef.Items.AddRange(new object[] {
            resources.GetString("cbClef.Items"),
            resources.GetString("cbClef.Items1")});
            resources.ApplyResources(this.cbClef, "cbClef");
            this.cbClef.Name = "cbClef";
            this.cbClef.SelectedIndexChanged += new System.EventHandler(this.cbClef_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.lblTitle.Name = "lblTitle";
            // 
            // updIndex
            // 
            resources.ApplyResources(this.updIndex, "updIndex");
            this.updIndex.Name = "updIndex";
            this.updIndex.ValueChanged += new System.EventHandler(this.updIndex_ValueChanged);
            // 
            // lblIndex
            // 
            resources.ApplyResources(this.lblIndex, "lblIndex");
            this.lblIndex.Name = "lblIndex";
            // 
            // frmNewTrackDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.updIndex);
            this.Controls.Add(this.lblIndex);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblClef);
            this.Controls.Add(this.cbClef);
            this.Controls.Add(this.lblInstruments);
            this.Controls.Add(this.cbInstruments);
            this.Controls.Add(this.txtTrackName);
            this.Controls.Add(this.lblTrackName);
            this.Controls.Add(this.lblChannels);
            this.Controls.Add(this.cbChannels);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewTrackDialog";
            ((System.ComponentModel.ISupportInitialize)(this.updIndex)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbChannels;
        private System.Windows.Forms.Label lblChannels;
        private System.Windows.Forms.Label lblTrackName;
        private System.Windows.Forms.TextBox txtTrackName;
        private System.Windows.Forms.Label lblInstruments;
        private System.Windows.Forms.ComboBox cbInstruments;
        private System.Windows.Forms.Label lblClef;
        private System.Windows.Forms.ComboBox cbClef;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.NumericUpDown updIndex;
        private System.Windows.Forms.Label lblIndex;
    }
}