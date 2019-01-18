namespace Karaboss.Configuration
{
    partial class KaraokeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KaraokeControl));
            this.lblMuteMelody = new System.Windows.Forms.Label();
            this.chkMuteMelody = new System.Windows.Forms.CheckBox();
            this.lblDisplayBalls = new System.Windows.Forms.Label();
            this.chkDisplayBalls = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMuteMelody
            // 
            resources.ApplyResources(this.lblMuteMelody, "lblMuteMelody");
            this.lblMuteMelody.Name = "lblMuteMelody";
            // 
            // chkMuteMelody
            // 
            resources.ApplyResources(this.chkMuteMelody, "chkMuteMelody");
            this.chkMuteMelody.Name = "chkMuteMelody";
            this.chkMuteMelody.UseVisualStyleBackColor = true;
            // 
            // lblDisplayBalls
            // 
            resources.ApplyResources(this.lblDisplayBalls, "lblDisplayBalls");
            this.lblDisplayBalls.Name = "lblDisplayBalls";
            // 
            // chkDisplayBalls
            // 
            resources.ApplyResources(this.chkDisplayBalls, "chkDisplayBalls");
            this.chkDisplayBalls.Name = "chkDisplayBalls";
            this.chkDisplayBalls.UseVisualStyleBackColor = true;
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
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // KaraokeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblDisplayBalls);
            this.Controls.Add(this.chkDisplayBalls);
            this.Controls.Add(this.lblMuteMelody);
            this.Controls.Add(this.chkMuteMelody);
            this.Name = "KaraokeControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDisplayBalls;
        private System.Windows.Forms.CheckBox chkDisplayBalls;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDescription;
    }
}
