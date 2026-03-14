namespace Karaboss.Kfn
{
    partial class frmKfnViewText
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKfnViewText));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.cbTextSize = new System.Windows.Forms.ComboBox();
            this.txtElement = new System.Windows.Forms.TextBox();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.cbTextSize);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(375, 37);
            this.pnlTop.TabIndex = 1;
            // 
            // cbTextSize
            // 
            this.cbTextSize.FormattingEnabled = true;
            this.cbTextSize.Location = new System.Drawing.Point(12, 10);
            this.cbTextSize.Name = "cbTextSize";
            this.cbTextSize.Size = new System.Drawing.Size(233, 21);
            this.cbTextSize.TabIndex = 0;
            this.cbTextSize.SelectedIndexChanged += new System.EventHandler(this.cbTextSize_SelectedIndexChanged);
            // 
            // txtElement
            // 
            this.txtElement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtElement.Location = new System.Drawing.Point(0, 37);
            this.txtElement.Multiline = true;
            this.txtElement.Name = "txtElement";
            this.txtElement.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtElement.Size = new System.Drawing.Size(375, 250);
            this.txtElement.TabIndex = 2;
            // 
            // frmKfnViewText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(375, 287);
            this.Controls.Add(this.txtElement);
            this.Controls.Add(this.pnlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmKfnViewText";
            this.Text = "frmKfnViewText";
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.ComboBox cbTextSize;
        private System.Windows.Forms.TextBox txtElement;
    }
}