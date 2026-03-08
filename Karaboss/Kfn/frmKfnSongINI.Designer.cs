namespace Karaboss.Kfn
{
    partial class frmKfnSongINI
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
            this.lvBlocks = new System.Windows.Forms.ListView();
            this.txtBlockContent = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lvBlocks
            // 
            this.lvBlocks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lvBlocks.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvBlocks.ForeColor = System.Drawing.Color.White;
            this.lvBlocks.HideSelection = false;
            this.lvBlocks.Location = new System.Drawing.Point(3, 12);
            this.lvBlocks.Name = "lvBlocks";
            this.lvBlocks.Size = new System.Drawing.Size(333, 230);
            this.lvBlocks.TabIndex = 1;
            this.lvBlocks.UseCompatibleStateImageBehavior = false;
            this.lvBlocks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvBlocks_MouseUp);
            // 
            // txtBlockContent
            // 
            this.txtBlockContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtBlockContent.Font = new System.Drawing.Font("Calibri Light", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBlockContent.ForeColor = System.Drawing.Color.White;
            this.txtBlockContent.Location = new System.Drawing.Point(342, 12);
            this.txtBlockContent.Multiline = true;
            this.txtBlockContent.Name = "txtBlockContent";
            this.txtBlockContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBlockContent.Size = new System.Drawing.Size(446, 426);
            this.txtBlockContent.TabIndex = 2;
            // 
            // frmKfnSongINI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtBlockContent);
            this.Controls.Add(this.lvBlocks);
            this.Name = "frmKfnSongINI";
            this.Text = "Song.ini";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKfnSongINI_FormClosing);
            this.Load += new System.EventHandler(this.frmKfnSongINI_Load);
            this.Resize += new System.EventHandler(this.frmKfnSongINI_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvBlocks;
        private System.Windows.Forms.TextBox txtBlockContent;
    }
}