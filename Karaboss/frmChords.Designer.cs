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
            this.tabChordsControl.Font = new System.Drawing.Font("Arial Narrow", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabChordsControl.ItemSize = new System.Drawing.Size(71, 40);
            this.tabChordsControl.Location = new System.Drawing.Point(0, 76);
            this.tabChordsControl.Margin = new System.Windows.Forms.Padding(4);
            this.tabChordsControl.Name = "tabChordsControl";
            this.tabChordsControl.SelectedIndex = 0;
            this.tabChordsControl.Size = new System.Drawing.Size(1067, 478);
            this.tabChordsControl.TabIndex = 0;
            // 
            // tabPageDiagrams
            // 
            this.tabPageDiagrams.Location = new System.Drawing.Point(4, 44);
            this.tabPageDiagrams.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageDiagrams.Name = "tabPageDiagrams";
            this.tabPageDiagrams.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageDiagrams.Size = new System.Drawing.Size(1059, 430);
            this.tabPageDiagrams.TabIndex = 0;
            this.tabPageDiagrams.Text = "Diagrams";
            this.tabPageDiagrams.UseVisualStyleBackColor = true;
            // 
            // tabPageOverview
            // 
            this.tabPageOverview.Location = new System.Drawing.Point(4, 44);
            this.tabPageOverview.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageOverview.Name = "tabPageOverview";
            this.tabPageOverview.Padding = new System.Windows.Forms.Padding(4);
            this.tabPageOverview.Size = new System.Drawing.Size(1059, 430);
            this.tabPageOverview.TabIndex = 1;
            this.tabPageOverview.Text = "Overview";
            this.tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // tabPageEdit
            // 
            this.tabPageEdit.Location = new System.Drawing.Point(4, 44);
            this.tabPageEdit.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageEdit.Name = "tabPageEdit";
            this.tabPageEdit.Size = new System.Drawing.Size(1059, 430);
            this.tabPageEdit.TabIndex = 2;
            this.tabPageEdit.Text = "Edit";
            this.tabPageEdit.UseVisualStyleBackColor = true;
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.DarkGray;
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 0);
            this.pnlToolbar.Margin = new System.Windows.Forms.Padding(4);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(1067, 68);
            this.pnlToolbar.TabIndex = 1;
            // 
            // frmChords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.tabChordsControl);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmChords";
            this.Text = "Chords";
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