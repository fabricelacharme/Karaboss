namespace ChordAnalyser.UI
{
    partial class frmDisplayChords
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
            this.txtOverview = new System.Windows.Forms.TextBox();
            this.tabChordsControl.SuspendLayout();
            this.tabPageOverview.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabChordsControl
            // 
            this.tabChordsControl.Controls.Add(this.tabPageDiagrams);
            this.tabChordsControl.Controls.Add(this.tabPageOverview);
            this.tabChordsControl.Controls.Add(this.tabPageEdit);
            this.tabChordsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabChordsControl.Location = new System.Drawing.Point(0, 0);
            this.tabChordsControl.Name = "tabChordsControl";
            this.tabChordsControl.SelectedIndex = 0;
            this.tabChordsControl.Size = new System.Drawing.Size(778, 419);
            this.tabChordsControl.TabIndex = 0;
            // 
            // tabPageDiagrams
            // 
            this.tabPageDiagrams.Location = new System.Drawing.Point(4, 22);
            this.tabPageDiagrams.Name = "tabPageDiagrams";
            this.tabPageDiagrams.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDiagrams.Size = new System.Drawing.Size(770, 393);
            this.tabPageDiagrams.TabIndex = 0;
            this.tabPageDiagrams.Text = "Diagrams";
            this.tabPageDiagrams.UseVisualStyleBackColor = true;
            // 
            // tabPageOverview
            // 
            this.tabPageOverview.Controls.Add(this.txtOverview);
            this.tabPageOverview.Location = new System.Drawing.Point(4, 22);
            this.tabPageOverview.Name = "tabPageOverview";
            this.tabPageOverview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOverview.Size = new System.Drawing.Size(770, 393);
            this.tabPageOverview.TabIndex = 1;
            this.tabPageOverview.Text = "Overview";
            this.tabPageOverview.UseVisualStyleBackColor = true;
            // 
            // tabPageEdit
            // 
            this.tabPageEdit.Location = new System.Drawing.Point(4, 22);
            this.tabPageEdit.Name = "tabPageEdit";
            this.tabPageEdit.Size = new System.Drawing.Size(770, 393);
            this.tabPageEdit.TabIndex = 2;
            this.tabPageEdit.Text = "Edit";
            this.tabPageEdit.UseVisualStyleBackColor = true;
            // 
            // txtOverview
            // 
            this.txtOverview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOverview.Location = new System.Drawing.Point(3, 3);
            this.txtOverview.Multiline = true;
            this.txtOverview.Name = "txtOverview";
            this.txtOverview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOverview.Size = new System.Drawing.Size(764, 387);
            this.txtOverview.TabIndex = 0;
            // 
            // frmDisplayChords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 419);
            this.Controls.Add(this.tabChordsControl);
            this.Name = "frmDisplayChords";
            this.Text = "Chords";
            this.Resize += new System.EventHandler(this.frmDisplayChords_Resize);
            this.tabChordsControl.ResumeLayout(false);
            this.tabPageOverview.ResumeLayout(false);
            this.tabPageOverview.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabChordsControl;
        private System.Windows.Forms.TabPage tabPageDiagrams;
        private System.Windows.Forms.TabPage tabPageOverview;
        private System.Windows.Forms.TabPage tabPageEdit;
        private System.Windows.Forms.TextBox txtOverview;
    }
}