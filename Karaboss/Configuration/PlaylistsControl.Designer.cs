namespace Karaboss.Configuration
{
    partial class PlaylistsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaylistsControl));
            this.label1 = new System.Windows.Forms.Label();
            this.lblPauseSongs = new System.Windows.Forms.Label();
            this.chkPauseSongs = new System.Windows.Forms.CheckBox();
            this.CountDownSong = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CountDownSong)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblPauseSongs
            // 
            resources.ApplyResources(this.lblPauseSongs, "lblPauseSongs");
            this.lblPauseSongs.Name = "lblPauseSongs";
            // 
            // chkPauseSongs
            // 
            resources.ApplyResources(this.chkPauseSongs, "chkPauseSongs");
            this.chkPauseSongs.Name = "chkPauseSongs";
            this.chkPauseSongs.UseVisualStyleBackColor = true;
            // 
            // CountDownSong
            // 
            resources.ApplyResources(this.CountDownSong, "CountDownSong");
            this.CountDownSong.Name = "CountDownSong";
            this.CountDownSong.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // PlaylistsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CountDownSong);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPauseSongs);
            this.Controls.Add(this.chkPauseSongs);
            this.Name = "PlaylistsControl";
            ((System.ComponentModel.ISupportInitialize)(this.CountDownSong)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblPauseSongs;
        private System.Windows.Forms.CheckBox chkPauseSongs;
        private System.Windows.Forms.NumericUpDown CountDownSong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDescription;
    }
}
