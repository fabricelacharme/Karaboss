﻿namespace Karaboss.Configuration
{
    partial class ChordsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChordsControl));
            this.lblHelp = new System.Windows.Forms.Label();
            this.lblPauseSongs = new System.Windows.Forms.Label();
            this.chkDisplayChords = new System.Windows.Forms.CheckBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDisplayXmlChords = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblHelp
            // 
            resources.ApplyResources(this.lblHelp, "lblHelp");
            this.lblHelp.Name = "lblHelp";
            // 
            // lblPauseSongs
            // 
            resources.ApplyResources(this.lblPauseSongs, "lblPauseSongs");
            this.lblPauseSongs.Name = "lblPauseSongs";
            // 
            // chkDisplayChords
            // 
            resources.ApplyResources(this.chkDisplayChords, "chkDisplayChords");
            this.chkDisplayChords.Name = "chkDisplayChords";
            this.chkDisplayChords.UseVisualStyleBackColor = true;
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
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
            // chkDisplayXmlChords
            // 
            resources.ApplyResources(this.chkDisplayXmlChords, "chkDisplayXmlChords");
            this.chkDisplayXmlChords.Checked = true;
            this.chkDisplayXmlChords.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDisplayXmlChords.Name = "chkDisplayXmlChords";
            this.chkDisplayXmlChords.UseVisualStyleBackColor = true;
            // 
            // ChordsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkDisplayXmlChords);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblHelp);
            this.Controls.Add(this.lblPauseSongs);
            this.Controls.Add(this.chkDisplayChords);
            this.Name = "ChordsControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHelp;
        private System.Windows.Forms.Label lblPauseSongs;
        private System.Windows.Forms.CheckBox chkDisplayChords;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkDisplayXmlChords;
    }
}
