namespace Karaboss.Display
{
    partial class PanelPlayer
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
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblBeat = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.pnlDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Controls.Add(this.lblDuration);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblBeat);
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblStatus);
            this.pnlDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDisplay.Location = new System.Drawing.Point(0, 0);
            this.pnlDisplay.Name = "pnlDisplay";
            this.pnlDisplay.Size = new System.Drawing.Size(200, 45);
            this.pnlDisplay.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Red;
            this.lblStatus.Location = new System.Drawing.Point(1, 2);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(80, 19);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Stopped";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblElapsed
            // 
            this.lblElapsed.BackColor = System.Drawing.Color.Transparent;
            this.lblElapsed.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblElapsed.ForeColor = System.Drawing.Color.White;
            this.lblElapsed.Location = new System.Drawing.Point(1, 22);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(60, 19);
            this.lblElapsed.TabIndex = 1;
            this.lblElapsed.Text = "00:00";
            this.lblElapsed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblBeat
            // 
            this.lblBeat.BackColor = System.Drawing.Color.Transparent;
            this.lblBeat.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBeat.ForeColor = System.Drawing.Color.PaleGreen;
            this.lblBeat.Location = new System.Drawing.Point(142, 2);
            this.lblBeat.Name = "lblBeat";
            this.lblBeat.Size = new System.Drawing.Size(60, 19);
            this.lblBeat.TabIndex = 2;
            this.lblBeat.Text = "15|16";
            this.lblBeat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblPercent.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercent.ForeColor = System.Drawing.Color.White;
            this.lblPercent.Location = new System.Drawing.Point(94, 26);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(19, 13);
            this.lblPercent.TabIndex = 3;
            this.lblPercent.Text = "0%";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDuration
            // 
            this.lblDuration.BackColor = System.Drawing.Color.Transparent;
            this.lblDuration.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDuration.ForeColor = System.Drawing.Color.White;
            this.lblDuration.Location = new System.Drawing.Point(142, 22);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(60, 19);
            this.lblDuration.TabIndex = 4;
            this.lblDuration.Text = "00:00";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PanelPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlDisplay);
            this.Name = "PanelPlayer";
            this.Size = new System.Drawing.Size(200, 45);
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblBeat;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblDuration;
    }
}
