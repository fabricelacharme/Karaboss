namespace Karaboss.Pages.ABCnotation
{
    partial class FrmABCnotation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmABCnotation));
            this.lstFiles = new System.Windows.Forms.ListView();
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rteEdit = new MarkedEditBox.RegexTaggedEdit();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitPerform = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitPerform)).BeginInit();
            this.splitPerform.Panel1.SuspendLayout();
            this.splitPerform.Panel2.SuspendLayout();
            this.splitPerform.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstFiles
            // 
            this.lstFiles.CheckBoxes = true;
            this.lstFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Title,
            this.File,
            this.Index});
            this.lstFiles.HideSelection = false;
            this.lstFiles.Location = new System.Drawing.Point(31, 43);
            this.lstFiles.MultiSelect = false;
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.ShowItemToolTips = true;
            this.lstFiles.Size = new System.Drawing.Size(218, 95);
            this.lstFiles.TabIndex = 0;
            this.lstFiles.UseCompatibleStateImageBehavior = false;
            this.lstFiles.View = System.Windows.Forms.View.Details;
            // 
            // Title
            // 
            this.Title.Text = "Title";
            // 
            // File
            // 
            this.File.Text = "File";
            // 
            // Index
            // 
            this.Index.Text = "Song Index";
            // 
            // rteEdit
            // 
            this.rteEdit.AutoTag = true;
            this.rteEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rteEdit.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rteEdit.HoverDelay = 1500;
            this.rteEdit.InsertionCol = 0;
            this.rteEdit.InsertionRow = 0;
            this.rteEdit.Location = new System.Drawing.Point(60, 27);
            this.rteEdit.Name = "rteEdit";
            this.rteEdit.Size = new System.Drawing.Size(189, 83);
            this.rteEdit.TabIndex = 1;
            this.rteEdit.Tags = new MarkedEditBox.RegexTag[] {
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags1"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags2"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags3"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags4"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags5"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags6"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags7"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags8"))),
        ((MarkedEditBox.RegexTag)(resources.GetObject("rteEdit.Tags9")))};
            this.rteEdit.Text = "";
            this.rteEdit.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitPerform
            // 
            this.splitPerform.Location = new System.Drawing.Point(44, 38);
            this.splitPerform.Name = "splitPerform";
            this.splitPerform.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitPerform.Panel1
            // 
            this.splitPerform.Panel1.Controls.Add(this.lstFiles);
            // 
            // splitPerform.Panel2
            // 
            this.splitPerform.Panel2.Controls.Add(this.rteEdit);
            this.splitPerform.Size = new System.Drawing.Size(721, 375);
            this.splitPerform.SplitterDistance = 187;
            this.splitPerform.TabIndex = 3;
            // 
            // FrmABCnotation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitPerform);
            this.Controls.Add(this.statusStrip1);
            this.Name = "FrmABCnotation";
            this.Text = "FrmABCnotation";
            this.splitPerform.Panel1.ResumeLayout(false);
            this.splitPerform.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPerform)).EndInit();
            this.splitPerform.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.ColumnHeader File;
        private System.Windows.Forms.ColumnHeader Index;
        private MarkedEditBox.RegexTaggedEdit rteEdit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitPerform;
    }
}