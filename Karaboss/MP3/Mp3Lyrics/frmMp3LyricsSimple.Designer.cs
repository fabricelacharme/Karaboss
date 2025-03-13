namespace Karaboss.Mp3
{
    partial class frmMp3LyricsSimple
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtLyrics = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtLyrics
            // 
            this.txtLyrics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLyrics.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLyrics.Location = new System.Drawing.Point(0, 0);
            this.txtLyrics.Multiline = true;
            this.txtLyrics.Name = "txtLyrics";
            this.txtLyrics.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLyrics.Size = new System.Drawing.Size(800, 450);
            this.txtLyrics.TabIndex = 0;
            // 
            // frmMp3LyricsSimple
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtLyrics);
            this.Name = "frmMp3LyricsSimple";
            this.Text = "frmMp3Lyrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMp3LyricsSimple_FormClosing);
            this.Load += new System.EventHandler(this.frmMp3LyricsSimple_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLyrics;
    }
}