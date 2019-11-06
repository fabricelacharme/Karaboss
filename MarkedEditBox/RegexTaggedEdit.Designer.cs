namespace MarkedEditBox
{
    partial class RegexTaggedEdit
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.richedit = new System.Windows.Forms.RichTextBox();
            this.rtfBackBuffer = new System.Windows.Forms.RichTextBox();
            this.tt = new System.Windows.Forms.ToolTip(this.components);
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mniCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mniPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mniSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mntContextInfo = new System.Windows.Forms.ToolStripTextBox();
            this.mnuContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // richedit
            // 
            this.richedit.ContextMenuStrip = this.mnuContext;
            this.richedit.Location = new System.Drawing.Point(0, 0);
            this.richedit.Name = "richedit";
            this.richedit.Size = new System.Drawing.Size(100, 96);
            this.richedit.TabIndex = 0;
            this.richedit.Text = "";
            this.tt.SetToolTip(this.richedit, "LOMM");
            this.richedit.WordWrap = false;
            // 
            // rtfBackBuffer
            // 
            this.rtfBackBuffer.BackColor = System.Drawing.Color.Yellow;
            this.rtfBackBuffer.Location = new System.Drawing.Point(0, 0);
            this.rtfBackBuffer.Name = "rtfBackBuffer";
            this.rtfBackBuffer.Size = new System.Drawing.Size(100, 96);
            this.rtfBackBuffer.TabIndex = 0;
            this.rtfBackBuffer.Text = "";
            this.rtfBackBuffer.Visible = false;
            this.rtfBackBuffer.WordWrap = false;
            // 
            // tt
            // 
            this.tt.Popup += new System.Windows.Forms.PopupEventHandler(this.OnToolTipPopup);
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniCut,
            this.mniCopy,
            this.mniPaste,
            this.mniSelectAll,
            this.mnSep1,
            this.mntContextInfo});
            this.mnuContext.Name = "mnuContext";
            this.mnuContext.ShowImageMargin = false;
            this.mnuContext.Size = new System.Drawing.Size(140, 98);
            this.mnuContext.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.OnContextMenuClosed);
            this.mnuContext.Opening += new System.ComponentModel.CancelEventHandler(this.OnContextMenuOpening);
            // 
            // mniCut
            // 
            this.mniCut.Name = "mniCut";
            this.mniCut.Size = new System.Drawing.Size(139, 22);
            this.mniCut.Text = "Cut";
            // 
            // mniCopy
            // 
            this.mniCopy.Name = "mniCopy";
            this.mniCopy.Size = new System.Drawing.Size(139, 22);
            this.mniCopy.Text = "Copy";
            // 
            // mniPaste
            // 
            this.mniPaste.Name = "mniPaste";
            this.mniPaste.Size = new System.Drawing.Size(139, 22);
            this.mniPaste.Text = "Paste";
            // 
            // mniSelectAll
            // 
            this.mniSelectAll.Name = "mniSelectAll";
            this.mniSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mniSelectAll.Size = new System.Drawing.Size(139, 22);
            this.mniSelectAll.Text = "Select &All";
            // 
            // mnSep1
            // 
            this.mnSep1.Name = "mnSep1";
            this.mnSep1.Size = new System.Drawing.Size(136, 6);
            // 
            // mntContextInfo
            // 
            this.mntContextInfo.Name = "mntContextInfo";
            this.mntContextInfo.Size = new System.Drawing.Size(100, 23);
            // 
            // RegexTaggedEdit
            // 
            this.Size = new System.Drawing.Size(314, 166);
            this.WordWrap = false;
            this.mnuContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richedit;
        private System.Windows.Forms.RichTextBox rtfBackBuffer;
        private System.Windows.Forms.ToolTip tt;
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.ToolStripMenuItem mniCut;
        private System.Windows.Forms.ToolStripMenuItem mniCopy;
        private System.Windows.Forms.ToolStripMenuItem mniPaste;
        private System.Windows.Forms.ToolStripMenuItem mniSelectAll;
        private System.Windows.Forms.ToolStripSeparator mnSep1;
        private System.Windows.Forms.ToolStripTextBox mntContextInfo;
    }
}
