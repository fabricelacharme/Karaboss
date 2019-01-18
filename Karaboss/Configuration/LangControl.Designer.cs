namespace Karaboss.Configuration
{
    partial class LangControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LangControl));
            this.m_langCB = new System.Windows.Forms.ComboBox();
            this.m_langL = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_langCB
            // 
            resources.ApplyResources(this.m_langCB, "m_langCB");
            this.m_langCB.Name = "m_langCB";
            // 
            // m_langL
            // 
            resources.ApplyResources(this.m_langL, "m_langL");
            this.m_langL.Name = "m_langL";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // LangControl
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.m_langCB);
            this.Controls.Add(this.m_langL);
            this.Name = "LangControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDescription;
    }
}
