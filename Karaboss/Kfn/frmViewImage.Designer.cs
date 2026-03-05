namespace Karaboss.Kfn
{
    partial class frmViewImage
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
            this.imgElement = new System.Windows.Forms.PictureBox();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.imgElement)).BeginInit();
            this.SuspendLayout();
            // 
            // imgElement
            // 
            this.imgElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imgElement.Location = new System.Drawing.Point(0, 0);
            this.imgElement.Name = "imgElement";
            this.imgElement.Size = new System.Drawing.Size(425, 293);
            this.imgElement.TabIndex = 0;
            this.imgElement.TabStop = false;
            this.imgElement.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgElement_MouseDown);
            // 
            // frmViewImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 293);
            this.Controls.Add(this.imgElement);
            this.Name = "frmViewImage";
            this.Text = "frmViewImage";
            ((System.ComponentModel.ISupportInitialize)(this.imgElement)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox imgElement;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
    }
}