namespace VBarControl.NavButton
{
    partial class NavButton
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavButton));
            this.pnlButton = new System.Windows.Forms.Panel();
            this.picButton = new System.Windows.Forms.PictureBox();
            this.lblButton = new System.Windows.Forms.Label();
            this.pnlButton.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picButton)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlButton
            // 
            this.pnlButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.pnlButton.Controls.Add(this.picButton);
            this.pnlButton.Controls.Add(this.lblButton);
            resources.ApplyResources(this.pnlButton, "pnlButton");
            this.pnlButton.Name = "pnlButton";
            // 
            // picButton
            // 
            this.picButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            resources.ApplyResources(this.picButton, "picButton");
            this.picButton.Name = "picButton";
            this.picButton.TabStop = false;
            // 
            // lblButton
            // 
            this.lblButton.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.lblButton, "lblButton");
            this.lblButton.Name = "lblButton";
            // 
            // NavButton
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlButton);
            this.Name = "NavButton";
            this.pnlButton.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlButton;
        private System.Windows.Forms.Label lblButton;
        private System.Windows.Forms.PictureBox picButton;
    }
}
