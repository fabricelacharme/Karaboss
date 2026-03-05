using Karaboss.Properties;

namespace Karaboss.Kfn
{
    partial class frmKfnView
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
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExport = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExportToEMZ = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExportToMP3LRC = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExportKFN = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuResourceEncoding = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.chkDecryptKFN = new System.Windows.Forms.CheckBox();
            this.pnlTopProperties = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lvProperties = new System.Windows.Forms.ListView();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.lvResources = new System.Windows.Forms.ListView();
            this.pnlTopResources = new System.Windows.Forms.Panel();
            this.btnViewConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlTopProperties.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlTopResources.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuExport,
            this.mnuTools,
            this.mnuResourceEncoding});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "File";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(128, 22);
            this.mnuFileOpen.Text = "Open KFN";
            this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpenKFN_Click);
            // 
            // mnuExport
            // 
            this.mnuExport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExportToEMZ,
            this.mnuExportToMP3LRC,
            this.mnuExportKFN});
            this.mnuExport.Name = "mnuExport";
            this.mnuExport.Size = new System.Drawing.Size(52, 20);
            this.mnuExport.Text = "Export";
            // 
            // mnuExportToEMZ
            // 
            this.mnuExportToEMZ.Name = "mnuExportToEMZ";
            this.mnuExportToEMZ.Size = new System.Drawing.Size(143, 22);
            this.mnuExportToEMZ.Text = "To EMZ";
            this.mnuExportToEMZ.Click += new System.EventHandler(this.mnuExportToEMZ_Click);
            // 
            // mnuExportToMP3LRC
            // 
            this.mnuExportToMP3LRC.Name = "mnuExportToMP3LRC";
            this.mnuExportToMP3LRC.Size = new System.Drawing.Size(143, 22);
            this.mnuExportToMP3LRC.Text = "To MP3+LRC";
            this.mnuExportToMP3LRC.Click += new System.EventHandler(this.mnuExportToMP3LRC_Click);
            // 
            // mnuExportKFN
            // 
            this.mnuExportKFN.Name = "mnuExportKFN";
            this.mnuExportKFN.Size = new System.Drawing.Size(143, 22);
            this.mnuExportKFN.Text = "To KFN";
            this.mnuExportKFN.Click += new System.EventHandler(this.mnuExportKFN_Click);
            // 
            // mnuTools
            // 
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(47, 20);
            this.mnuTools.Text = "Tools";
            // 
            // mnuResourceEncoding
            // 
            this.mnuResourceEncoding.Name = "mnuResourceEncoding";
            this.mnuResourceEncoding.Size = new System.Drawing.Size(120, 20);
            this.mnuResourceEncoding.Text = "Resource encoding";
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.pnlTop.Controls.Add(this.chkDecryptKFN);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.ForeColor = System.Drawing.Color.Gray;
            this.pnlTop.Location = new System.Drawing.Point(0, 24);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(784, 35);
            this.pnlTop.TabIndex = 1;
            // 
            // chkDecryptKFN
            // 
            this.chkDecryptKFN.AutoSize = true;
            this.chkDecryptKFN.Checked = true;
            this.chkDecryptKFN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDecryptKFN.Enabled = false;
            this.chkDecryptKFN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDecryptKFN.ForeColor = System.Drawing.Color.DarkGray;
            this.chkDecryptKFN.Location = new System.Drawing.Point(12, 15);
            this.chkDecryptKFN.Name = "chkDecryptKFN";
            this.chkDecryptKFN.Size = new System.Drawing.Size(203, 19);
            this.chkDecryptKFN.TabIndex = 0;
            this.chkDecryptKFN.Text = "Decrypt KFN while export to KFN";
            this.chkDecryptKFN.UseVisualStyleBackColor = true;
            // 
            // pnlTopProperties
            // 
            this.pnlTopProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.pnlTopProperties.Controls.Add(this.label1);
            this.pnlTopProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopProperties.Location = new System.Drawing.Point(0, 0);
            this.pnlTopProperties.Name = "pnlTopProperties";
            this.pnlTopProperties.Size = new System.Drawing.Size(344, 39);
            this.pnlTopProperties.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Properties:";
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.lvProperties);
            this.pnlLeft.Controls.Add(this.pnlTopProperties);
            this.pnlLeft.Location = new System.Drawing.Point(12, 67);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(344, 319);
            this.pnlLeft.TabIndex = 4;
            // 
            // lvProperties
            // 
            this.lvProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lvProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProperties.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvProperties.ForeColor = System.Drawing.Color.White;
            this.lvProperties.HideSelection = false;
            this.lvProperties.Location = new System.Drawing.Point(0, 39);
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(344, 280);
            this.lvProperties.TabIndex = 3;
            this.lvProperties.UseCompatibleStateImageBehavior = false;
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.lvResources);
            this.pnlRight.Controls.Add(this.pnlTopResources);
            this.pnlRight.Location = new System.Drawing.Point(373, 67);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(332, 319);
            this.pnlRight.TabIndex = 5;
            // 
            // lvResources
            // 
            this.lvResources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.lvResources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResources.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvResources.ForeColor = System.Drawing.Color.White;
            this.lvResources.HideSelection = false;
            this.lvResources.Location = new System.Drawing.Point(0, 39);
            this.lvResources.Name = "lvResources";
            this.lvResources.Size = new System.Drawing.Size(332, 280);
            this.lvResources.TabIndex = 6;
            this.lvResources.UseCompatibleStateImageBehavior = false;
            this.lvResources.DoubleClick += new System.EventHandler(this.lvResources_DoubleClick);
            this.lvResources.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lvResources_MouseUp);
            // 
            // pnlTopResources
            // 
            this.pnlTopResources.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.pnlTopResources.Controls.Add(this.btnViewConfig);
            this.pnlTopResources.Controls.Add(this.label2);
            this.pnlTopResources.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopResources.Location = new System.Drawing.Point(0, 0);
            this.pnlTopResources.Name = "pnlTopResources";
            this.pnlTopResources.Size = new System.Drawing.Size(332, 39);
            this.pnlTopResources.TabIndex = 4;
            // 
            // btnViewConfig
            // 
            this.btnViewConfig.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnViewConfig.Location = new System.Drawing.Point(245, 10);
            this.btnViewConfig.Name = "btnViewConfig";
            this.btnViewConfig.Size = new System.Drawing.Size(75, 23);
            this.btnViewConfig.TabIndex = 2;
            this.btnViewConfig.Text = "View Config";
            this.btnViewConfig.UseVisualStyleBackColor = true;
            this.btnViewConfig.Click += new System.EventHandler(this.btnViewConfig_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Resources:";
            // 
            // frmKfnView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 391);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmKfnView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKfnView_FormClosing);
            this.Load += new System.EventHandler(this.frmKfnView_Load);
            this.Resize += new System.EventHandler(this.frmKfnView_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlTopProperties.ResumeLayout(false);
            this.pnlTopProperties.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.pnlTopResources.ResumeLayout(false);
            this.pnlTopResources.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuExport;
        private System.Windows.Forms.ToolStripMenuItem mnuExportToEMZ;
        private System.Windows.Forms.ToolStripMenuItem mnuExportToMP3LRC;
        private System.Windows.Forms.ToolStripMenuItem mnuExportKFN;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuResourceEncoding;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlTopProperties;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel pnlTopResources;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkDecryptKFN;
        private System.Windows.Forms.ListView lvProperties;
        private System.Windows.Forms.ListView lvResources;
        private System.Windows.Forms.Button btnViewConfig;
    }
}