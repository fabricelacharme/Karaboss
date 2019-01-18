namespace Sanford.Multimedia.Midi.Score.UI
{
    partial class selectMeasuresDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(selectMeasuresDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.updFrom = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.updTo = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.chkAllMeasures = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.updFrom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updTo)).BeginInit();
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
            // updFrom
            // 
            resources.ApplyResources(this.updFrom, "updFrom");
            this.updFrom.Name = "updFrom";
            this.updFrom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updFrom.ValueChanged += new System.EventHandler(this.updFrom_ValueChanged);
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
            // updTo
            // 
            resources.ApplyResources(this.updTo, "updTo");
            this.updTo.Name = "updTo";
            this.updTo.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updTo.ValueChanged += new System.EventHandler(this.updTo_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // chkAllMeasures
            // 
            resources.ApplyResources(this.chkAllMeasures, "chkAllMeasures");
            this.chkAllMeasures.Name = "chkAllMeasures";
            this.chkAllMeasures.UseVisualStyleBackColor = true;
            this.chkAllMeasures.CheckedChanged += new System.EventHandler(this.chkAllMeasures_CheckedChanged);
            // 
            // selectMeasuresDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.chkAllMeasures);
            this.Controls.Add(this.updTo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.updFrom);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "selectMeasuresDialog";
            ((System.ComponentModel.ISupportInitialize)(this.updFrom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updTo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.NumericUpDown updFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown updTo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAllMeasures;
    }
}