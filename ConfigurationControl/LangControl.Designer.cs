namespace Slaks.Windows.Forms
{
    public partial class LangControl
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
            components = new System.ComponentModel.Container();

            this.m_langCB = new System.Windows.Forms.ComboBox();
            this.m_langL = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // m_lang
            //
            this.m_langCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.m_langCB.Location = new System.Drawing.Point(184, 88);
            this.m_langCB.Name = "m_langCB";
            this.m_langCB.Size = new System.Drawing.Size(150, 21);
            this.m_langCB.TabIndex = 20;
            // 
            // m_langL
            // 
            this.m_langL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
            this.m_langL.Location = new System.Drawing.Point(8, 88);
            this.m_langL.Name = "m_langL";
            this.m_langL.Size = new System.Drawing.Size(168, 23);
            this.m_langL.TabIndex = 21;
            this.m_langL.Text = "Select Language:";
            //
            // SongLanguageControl
            //
            this.Controls.Add(this.m_langCB);
            this.Controls.Add(this.m_langL);
            this.Name = "LangControl";
            this.Size = new System.Drawing.Size(600, 152);


            this.ResumeLayout(false);
        }

        #endregion
    }
}
