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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lvBlocks
            // 
            this.lvBlocks.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lvBlocks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvBlocks.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvBlocks.ForeColor = System.Drawing.Color.White;
            this.lvBlocks.HideSelection = false;
            this.lvBlocks.Location = new System.Drawing.Point(3, 30);
            this.lvBlocks.Name = "lvBlocks";
            this.lvBlocks.Size = new System.Drawing.Size(380, 312);
            this.lvBlocks.TabIndex = 1;
            this.lvBlocks.UseCompatibleStateImageBehavior = false;
            this.lvBlocks.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvBlocks_MouseUp);
            // 
            // txtBlockContent
            // 
            this.txtBlockContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtBlockContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBlockContent.Font = new System.Drawing.Font("Calibri Light", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBlockContent.ForeColor = System.Drawing.Color.White;
            this.txtBlockContent.Location = new System.Drawing.Point(420, 30);
            this.txtBlockContent.Multiline = true;
            this.txtBlockContent.Name = "txtBlockContent";
            this.txtBlockContent.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBlockContent.Size = new System.Drawing.Size(644, 312);
            this.txtBlockContent.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Blocks";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(417, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Content";
            // 
            // frmKfnSongINI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.ClientSize = new System.Drawing.Size(1076, 343);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBlockContent);
            this.Controls.Add(this.lvBlocks);
            this.Name = "frmKfnSongINI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}