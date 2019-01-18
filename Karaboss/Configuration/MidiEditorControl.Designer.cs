namespace Karaboss.Configuration
{
    partial class MidiEditorControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MidiEditorControl));
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblParamDescr = new System.Windows.Forms.Label();
            this.lblParam = new System.Windows.Forms.Label();
            this.UpDownTransposeAmount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.UpDownVelocity = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownTransposeAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownVelocity)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblParamDescr
            // 
            resources.ApplyResources(this.lblParamDescr, "lblParamDescr");
            this.lblParamDescr.Name = "lblParamDescr";
            // 
            // lblParam
            // 
            resources.ApplyResources(this.lblParam, "lblParam");
            this.lblParam.Name = "lblParam";
            // 
            // UpDownTransposeAmount
            // 
            resources.ApplyResources(this.UpDownTransposeAmount, "UpDownTransposeAmount");
            this.UpDownTransposeAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.UpDownTransposeAmount.Name = "UpDownTransposeAmount";
            this.UpDownTransposeAmount.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // UpDownVelocity
            // 
            resources.ApplyResources(this.UpDownVelocity, "UpDownVelocity");
            this.UpDownVelocity.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.UpDownVelocity.Name = "UpDownVelocity";
            this.UpDownVelocity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // MidiEditorControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.UpDownVelocity);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblParamDescr);
            this.Controls.Add(this.lblParam);
            this.Controls.Add(this.UpDownTransposeAmount);
            this.Name = "MidiEditorControl";
            ((System.ComponentModel.ISupportInitialize)(this.UpDownTransposeAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UpDownVelocity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblParamDescr;
        private System.Windows.Forms.Label lblParam;
        private System.Windows.Forms.NumericUpDown UpDownTransposeAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown UpDownVelocity;
    }
}
