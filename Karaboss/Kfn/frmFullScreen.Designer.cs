namespace Karaboss.Kfn
{
    partial class frmFullScreen
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
            this.picScreen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).BeginInit();
            this.SuspendLayout();
            // 
            // picScreen
            // 
            this.picScreen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picScreen.Location = new System.Drawing.Point(0, 0);
            this.picScreen.Name = "picScreen";
            this.picScreen.Size = new System.Drawing.Size(800, 450);
            this.picScreen.TabIndex = 0;
            this.picScreen.TabStop = false;
            this.picScreen.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picScreen_MouseDown);
            // 
            // frmFullScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.picScreen);
            this.Name = "frmFullScreen";
            this.Text = "frmFullScreen";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFullScreen_FormClosing);
            this.Load += new System.EventHandler(this.frmFullScreen_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFullScreen_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.picScreen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picScreen;
    }
}