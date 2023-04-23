namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class ChangeTimeSignature
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeTimeSignature));
            this.lblActual = new System.Windows.Forms.Label();
            this.updNumerator = new System.Windows.Forms.NumericUpDown();
            this.lblSlash = new System.Windows.Forms.Label();
            this.updDenominator = new System.Windows.Forms.NumericUpDown();
            this.lblNew = new System.Windows.Forms.Label();
            this.lblActualValue = new System.Windows.Forms.Label();
            this.lblExplanation = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.updNumerator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updDenominator)).BeginInit();
            this.SuspendLayout();
            // 
            // lblActual
            // 
            resources.ApplyResources(this.lblActual, "lblActual");
            this.lblActual.Name = "lblActual";
            // 
            // updNumerator
            // 
            resources.ApplyResources(this.updNumerator, "updNumerator");
            this.updNumerator.Name = "updNumerator";
            this.updNumerator.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // lblSlash
            // 
            resources.ApplyResources(this.lblSlash, "lblSlash");
            this.lblSlash.Name = "lblSlash";
            // 
            // updDenominator
            // 
            resources.ApplyResources(this.updDenominator, "updDenominator");
            this.updDenominator.Name = "updDenominator";
            this.updDenominator.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // lblNew
            // 
            resources.ApplyResources(this.lblNew, "lblNew");
            this.lblNew.Name = "lblNew";
            // 
            // lblActualValue
            // 
            resources.ApplyResources(this.lblActualValue, "lblActualValue");
            this.lblActualValue.Name = "lblActualValue";
            // 
            // lblExplanation
            // 
            resources.ApplyResources(this.lblExplanation, "lblExplanation");
            this.lblExplanation.Name = "lblExplanation";
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
            // 
            // ChangeTimeSignature
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lblExplanation);
            this.Controls.Add(this.lblActualValue);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.updDenominator);
            this.Controls.Add(this.lblSlash);
            this.Controls.Add(this.updNumerator);
            this.Controls.Add(this.lblActual);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeTimeSignature";
            ((System.ComponentModel.ISupportInitialize)(this.updNumerator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updDenominator)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblActual;
        private System.Windows.Forms.NumericUpDown updNumerator;
        private System.Windows.Forms.Label lblSlash;
        private System.Windows.Forms.NumericUpDown updDenominator;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.Label lblActualValue;
        private System.Windows.Forms.Label lblExplanation;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}