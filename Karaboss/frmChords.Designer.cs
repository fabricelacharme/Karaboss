namespace Karaboss
{
    partial class frmChords
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChords));
            this.tabChordsControl = new System.Windows.Forms.TabControl();
            this.tabPageDiagrams = new System.Windows.Forms.TabPage();
            this.tabPageOverview = new System.Windows.Forms.TabPage();
            this.tabPageEdit = new System.Windows.Forms.TabPage();
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.tabChordsControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabChordsControl
            // 
            this.tabChordsControl.Controls.Add(this.tabPageDiagrams);
            this.tabChordsControl.Controls.Add(this.tabPageOverview);
            this.tabChordsControl.Controls.Add(this.tabPageEdit);
            resources.ApplyResources(this.tabChordsControl, "tabChordsControl");
            this.tabChordsControl.Name = "tabChordsControl";
            this.tabChordsControl.SelectedIndex = 0;
            // 
            // tabPageDiagrams
            // 
            resources.ApplyResources(this.tabPageDiagrams, "tabPageDiagrams");
            this.tabPageDiagrams.Name = "tabPageDiagrams";
            this.tabPageDiagrams.UseVisualStyleBackColor = true;
            // 
            // tabPageOverview
            // 
            resources.ApplyResources(this.tabPageOverview, "tabPageOverview");
            this.tabPageOverview.Name = "tabPageOverview";
            this.tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // tabPageEdit
            // 
            resources.ApplyResources(this.tabPageEdit, "tabPageEdit");
            this.tabPageEdit.Name = "tabPageEdit";
            this.tabPageEdit.UseVisualStyleBackColor = true;
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            resources.ApplyResources(this.pnlToolbar, "pnlToolbar");
            this.pnlToolbar.Name = "pnlToolbar";
            // 
            // frmChords
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.tabChordsControl);
            this.Name = "frmChords";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChords_FormClosing);
            this.Load += new System.EventHandler(this.frmChords_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmChords_KeyUp);
            this.Resize += new System.EventHandler(this.frmChords_Resize);
            this.tabChordsControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabChordsControl;
        private System.Windows.Forms.TabPage tabPageDiagrams;
        private System.Windows.Forms.TabPage tabPageOverview;
        private System.Windows.Forms.TabPage tabPageEdit;
        private System.Windows.Forms.Panel pnlToolbar;
    }
}