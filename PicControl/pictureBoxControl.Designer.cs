namespace PicControl
{
    partial class pictureBoxControl
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
            this.pboxWnd = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pboxWnd)).BeginInit();
            this.SuspendLayout();
            // 
            // pboxWnd
            // 
            this.pboxWnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pboxWnd.Location = new System.Drawing.Point(0, 0);
            this.pboxWnd.Name = "pboxWnd";
            this.pboxWnd.Size = new System.Drawing.Size(150, 150);
            this.pboxWnd.TabIndex = 0;
            this.pboxWnd.TabStop = false;
            this.pboxWnd.Paint += new System.Windows.Forms.PaintEventHandler(this.pboxWnd_Paint);
            this.pboxWnd.Resize += new System.EventHandler(this.pboxWnd_Resize);
            // 
            // pictureBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pboxWnd);
            this.DoubleBuffered = true;
            this.Name = "pictureBoxControl";
            ((System.ComponentModel.ISupportInitialize)(this.pboxWnd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pboxWnd;
    }
}
