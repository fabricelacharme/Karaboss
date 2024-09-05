namespace BallsControl
{
    partial class Balls
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        /*
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        */

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.picWnd = new BallsControl.BallsWnd();
            ((System.ComponentModel.ISupportInitialize)(this.picWnd)).BeginInit();
            this.SuspendLayout();
            // 
            // picWnd
            // 
            this.picWnd.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.picWnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picWnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picWnd.Location = new System.Drawing.Point(0, 0);
            this.picWnd.Name = "picWnd";
            this.picWnd.Size = new System.Drawing.Size(405, 43);
            this.picWnd.TabIndex = 0;
            this.picWnd.TabStop = false;
            this.picWnd.Resize += new System.EventHandler(this.picWnd_Resize);
            // 
            // balls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picWnd);
            this.Name = "balls";
            this.Size = new System.Drawing.Size(405, 43);
            this.Load += new System.EventHandler(this.picWnd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picWnd)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        // code d'origine
        //private System.Windows.Forms.PictureBox picWnd;
        private BallsWnd picWnd;

    }
}
