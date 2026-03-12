namespace Karaboss.Kfn
{
    partial class frmKfnCreate
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
            this.lblMp3File = new System.Windows.Forms.Label();
            this.txtMp3File = new System.Windows.Forms.TextBox();
            this.btnImportMp3File = new System.Windows.Forms.Button();
            this.btnImportSongINIFile = new System.Windows.Forms.Button();
            this.txtSongINIFile = new System.Windows.Forms.TextBox();
            this.lblSongINI = new System.Windows.Forms.Label();
            this.btnImportImage = new System.Windows.Forms.Button();
            this.txtImageFile = new System.Windows.Forms.TextBox();
            this.lblImage = new System.Windows.Forms.Label();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnCreateKfn = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMp3File
            // 
            this.lblMp3File.AutoSize = true;
            this.lblMp3File.Location = new System.Drawing.Point(54, 80);
            this.lblMp3File.Name = "lblMp3File";
            this.lblMp3File.Size = new System.Drawing.Size(43, 13);
            this.lblMp3File.TabIndex = 0;
            this.lblMp3File.Text = "mp3 file";
            // 
            // txtMp3File
            // 
            this.txtMp3File.Location = new System.Drawing.Point(103, 77);
            this.txtMp3File.Name = "txtMp3File";
            this.txtMp3File.Size = new System.Drawing.Size(211, 20);
            this.txtMp3File.TabIndex = 1;
            // 
            // btnImportMp3File
            // 
            this.btnImportMp3File.Location = new System.Drawing.Point(320, 77);
            this.btnImportMp3File.Name = "btnImportMp3File";
            this.btnImportMp3File.Size = new System.Drawing.Size(75, 23);
            this.btnImportMp3File.TabIndex = 2;
            this.btnImportMp3File.Text = "Import";
            this.btnImportMp3File.UseVisualStyleBackColor = true;
            this.btnImportMp3File.Click += new System.EventHandler(this.btnImportMp3File_Click);
            // 
            // btnImportSongINIFile
            // 
            this.btnImportSongINIFile.Location = new System.Drawing.Point(320, 103);
            this.btnImportSongINIFile.Name = "btnImportSongINIFile";
            this.btnImportSongINIFile.Size = new System.Drawing.Size(75, 23);
            this.btnImportSongINIFile.TabIndex = 5;
            this.btnImportSongINIFile.Text = "Import";
            this.btnImportSongINIFile.UseVisualStyleBackColor = true;
            this.btnImportSongINIFile.Click += new System.EventHandler(this.btnImportSongINIFile_Click);
            // 
            // txtSongINIFile
            // 
            this.txtSongINIFile.Location = new System.Drawing.Point(103, 103);
            this.txtSongINIFile.Name = "txtSongINIFile";
            this.txtSongINIFile.Size = new System.Drawing.Size(211, 20);
            this.txtSongINIFile.TabIndex = 4;
            // 
            // lblSongINI
            // 
            this.lblSongINI.AutoSize = true;
            this.lblSongINI.Location = new System.Drawing.Point(54, 106);
            this.lblSongINI.Name = "lblSongINI";
            this.lblSongINI.Size = new System.Drawing.Size(45, 13);
            this.lblSongINI.TabIndex = 3;
            this.lblSongINI.Text = "Song.ini";
            // 
            // btnImportImage
            // 
            this.btnImportImage.Location = new System.Drawing.Point(320, 129);
            this.btnImportImage.Name = "btnImportImage";
            this.btnImportImage.Size = new System.Drawing.Size(75, 23);
            this.btnImportImage.TabIndex = 8;
            this.btnImportImage.Text = "Import";
            this.btnImportImage.UseVisualStyleBackColor = true;
            this.btnImportImage.Click += new System.EventHandler(this.btnImportImage_Click);
            // 
            // txtImageFile
            // 
            this.txtImageFile.Location = new System.Drawing.Point(103, 129);
            this.txtImageFile.Name = "txtImageFile";
            this.txtImageFile.Size = new System.Drawing.Size(211, 20);
            this.txtImageFile.TabIndex = 7;
            // 
            // lblImage
            // 
            this.lblImage.AutoSize = true;
            this.lblImage.Location = new System.Drawing.Point(54, 132);
            this.lblImage.Name = "lblImage";
            this.lblImage.Size = new System.Drawing.Size(24, 13);
            this.lblImage.TabIndex = 6;
            this.lblImage.Text = "Jpg";
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "openFileDialog1";
            // 
            // btnCreateKfn
            // 
            this.btnCreateKfn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreateKfn.Location = new System.Drawing.Point(128, 193);
            this.btnCreateKfn.Name = "btnCreateKfn";
            this.btnCreateKfn.Size = new System.Drawing.Size(75, 23);
            this.btnCreateKfn.TabIndex = 9;
            this.btnCreateKfn.Text = "Create KFN";
            this.btnCreateKfn.UseVisualStyleBackColor = true;
            this.btnCreateKfn.Click += new System.EventHandler(this.btnCreateKfn_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(247, 193);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmKfnCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 228);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateKfn);
            this.Controls.Add(this.btnImportImage);
            this.Controls.Add(this.txtImageFile);
            this.Controls.Add(this.lblImage);
            this.Controls.Add(this.btnImportSongINIFile);
            this.Controls.Add(this.txtSongINIFile);
            this.Controls.Add(this.lblSongINI);
            this.Controls.Add(this.btnImportMp3File);
            this.Controls.Add(this.txtMp3File);
            this.Controls.Add(this.lblMp3File);
            this.Name = "frmKfnCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmKfnCreate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMp3File;
        private System.Windows.Forms.TextBox txtMp3File;
        private System.Windows.Forms.Button btnImportMp3File;
        private System.Windows.Forms.Button btnImportSongINIFile;
        private System.Windows.Forms.TextBox txtSongINIFile;
        private System.Windows.Forms.Label lblSongINI;
        private System.Windows.Forms.Button btnImportImage;
        private System.Windows.Forms.TextBox txtImageFile;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Button btnCreateKfn;
        private System.Windows.Forms.Button btnCancel;
    }
}