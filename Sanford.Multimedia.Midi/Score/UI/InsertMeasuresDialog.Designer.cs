namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class frmInsertMeasuresDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInsertMeasuresDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.updNumberMeasures = new System.Windows.Forms.NumericUpDown();
            this.radioButtonThisTrack = new System.Windows.Forms.RadioButton();
            this.radioButtonAllTracks = new System.Windows.Forms.RadioButton();
            this.updStartMeasure = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updNumberMeasures)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updStartMeasure)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // updNumberMeasures
            // 
            resources.ApplyResources(this.updNumberMeasures, "updNumberMeasures");
            this.updNumberMeasures.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.updNumberMeasures.Name = "updNumberMeasures";
            // 
            // radioButtonThisTrack
            // 
            resources.ApplyResources(this.radioButtonThisTrack, "radioButtonThisTrack");
            this.radioButtonThisTrack.Checked = true;
            this.radioButtonThisTrack.Name = "radioButtonThisTrack";
            this.radioButtonThisTrack.TabStop = true;
            this.radioButtonThisTrack.UseVisualStyleBackColor = true;
            // 
            // radioButtonAllTracks
            // 
            resources.ApplyResources(this.radioButtonAllTracks, "radioButtonAllTracks");
            this.radioButtonAllTracks.Name = "radioButtonAllTracks";
            this.radioButtonAllTracks.TabStop = true;
            this.radioButtonAllTracks.UseVisualStyleBackColor = true;
            // 
            // updStartMeasure
            // 
            resources.ApplyResources(this.updStartMeasure, "updStartMeasure");
            this.updStartMeasure.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updStartMeasure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updStartMeasure.Name = "updStartMeasure";
            this.updStartMeasure.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // frmInsertMeasuresDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.updStartMeasure);
            this.Controls.Add(this.radioButtonAllTracks);
            this.Controls.Add(this.radioButtonThisTrack);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.updNumberMeasures);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInsertMeasuresDialog";
            ((System.ComponentModel.ISupportInitialize)(this.updNumberMeasures)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updStartMeasure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown updNumberMeasures;
        private System.Windows.Forms.RadioButton radioButtonThisTrack;
        private System.Windows.Forms.RadioButton radioButtonAllTracks;
        private System.Windows.Forms.NumericUpDown updStartMeasure;
        private System.Windows.Forms.Label label3;
    }
}