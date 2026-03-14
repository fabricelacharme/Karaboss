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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmKfnCreate));
            this.lblMp3File = new System.Windows.Forms.Label();
            this.txtMp3File = new System.Windows.Forms.TextBox();
            this.btnImportMp3File = new System.Windows.Forms.Button();
            this.btnImportSongINIFile = new System.Windows.Forms.Button();
            this.txtSongINIFile = new System.Windows.Forms.TextBox();
            this.lblSongINI = new System.Windows.Forms.Label();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnCreateKfn = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbControl = new System.Windows.Forms.TabControl();
            this.tbPageMp3 = new System.Windows.Forms.TabPage();
            this.lblKFNFile = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnTb1Next = new System.Windows.Forms.Button();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbPageLyrics = new System.Windows.Forms.TabPage();
            this.btnTb2Next = new System.Windows.Forms.Button();
            this.btnTb2Previous = new System.Windows.Forms.Button();
            this.tbPageImages = new System.Windows.Forms.TabPage();
            this.btnImportImage = new System.Windows.Forms.Button();
            this.lblImage = new System.Windows.Forms.Label();
            this.txtImageFile = new System.Windows.Forms.TextBox();
            this.btnTb3Previous = new System.Windows.Forms.Button();
            this.tbControl.SuspendLayout();
            this.tbPageMp3.SuspendLayout();
            this.tbPageLyrics.SuspendLayout();
            this.tbPageImages.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMp3File
            // 
            this.lblMp3File.AutoSize = true;
            this.lblMp3File.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMp3File.ForeColor = System.Drawing.Color.White;
            this.lblMp3File.Location = new System.Drawing.Point(4, 40);
            this.lblMp3File.Name = "lblMp3File";
            this.lblMp3File.Size = new System.Drawing.Size(50, 14);
            this.lblMp3File.TabIndex = 0;
            this.lblMp3File.Text = "mp3 file";
            // 
            // txtMp3File
            // 
            this.txtMp3File.Location = new System.Drawing.Point(80, 37);
            this.txtMp3File.Name = "txtMp3File";
            this.txtMp3File.Size = new System.Drawing.Size(500, 23);
            this.txtMp3File.TabIndex = 1;
            // 
            // btnImportMp3File
            // 
            this.btnImportMp3File.Location = new System.Drawing.Point(590, 35);
            this.btnImportMp3File.Name = "btnImportMp3File";
            this.btnImportMp3File.Size = new System.Drawing.Size(75, 23);
            this.btnImportMp3File.TabIndex = 2;
            this.btnImportMp3File.Text = "Import";
            this.btnImportMp3File.UseVisualStyleBackColor = true;
            this.btnImportMp3File.Click += new System.EventHandler(this.btnImportMp3File_Click);
            // 
            // btnImportSongINIFile
            // 
            this.btnImportSongINIFile.Location = new System.Drawing.Point(590, 35);
            this.btnImportSongINIFile.Name = "btnImportSongINIFile";
            this.btnImportSongINIFile.Size = new System.Drawing.Size(75, 23);
            this.btnImportSongINIFile.TabIndex = 5;
            this.btnImportSongINIFile.Text = "Import";
            this.btnImportSongINIFile.UseVisualStyleBackColor = true;
            this.btnImportSongINIFile.Click += new System.EventHandler(this.btnImportSongINIFile_Click);
            // 
            // txtSongINIFile
            // 
            this.txtSongINIFile.Location = new System.Drawing.Point(80, 37);
            this.txtSongINIFile.Name = "txtSongINIFile";
            this.txtSongINIFile.Size = new System.Drawing.Size(500, 23);
            this.txtSongINIFile.TabIndex = 4;
            // 
            // lblSongINI
            // 
            this.lblSongINI.AutoSize = true;
            this.lblSongINI.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSongINI.ForeColor = System.Drawing.Color.White;
            this.lblSongINI.Location = new System.Drawing.Point(4, 40);
            this.lblSongINI.Name = "lblSongINI";
            this.lblSongINI.Size = new System.Drawing.Size(51, 15);
            this.lblSongINI.TabIndex = 3;
            this.lblSongINI.Text = "Song.ini";
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.FileName = "openFileDialog1";
            // 
            // btnCreateKfn
            // 
            this.btnCreateKfn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreateKfn.Location = new System.Drawing.Point(205, 286);
            this.btnCreateKfn.Name = "btnCreateKfn";
            this.btnCreateKfn.Size = new System.Drawing.Size(110, 23);
            this.btnCreateKfn.TabIndex = 9;
            this.btnCreateKfn.Text = "Create KFN";
            this.btnCreateKfn.UseVisualStyleBackColor = true;
            this.btnCreateKfn.Click += new System.EventHandler(this.btnCreateKfn_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(352, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(99, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Quit";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tbControl
            // 
            this.tbControl.Controls.Add(this.tbPageMp3);
            this.tbControl.Controls.Add(this.tbPageLyrics);
            this.tbControl.Controls.Add(this.tbPageImages);
            this.tbControl.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbControl.Location = new System.Drawing.Point(4, 25);
            this.tbControl.Name = "tbControl";
            this.tbControl.SelectedIndex = 0;
            this.tbControl.Size = new System.Drawing.Size(685, 239);
            this.tbControl.TabIndex = 11;
            // 
            // tbPageMp3
            // 
            this.tbPageMp3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.tbPageMp3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tbPageMp3.Controls.Add(this.lblKFNFile);
            this.tbPageMp3.Controls.Add(this.label5);
            this.tbPageMp3.Controls.Add(this.btnTb1Next);
            this.tbPageMp3.Controls.Add(this.txtComment);
            this.tbPageMp3.Controls.Add(this.label2);
            this.tbPageMp3.Controls.Add(this.txtTitle);
            this.tbPageMp3.Controls.Add(this.label1);
            this.tbPageMp3.Controls.Add(this.txtMp3File);
            this.tbPageMp3.Controls.Add(this.lblMp3File);
            this.tbPageMp3.Controls.Add(this.btnImportMp3File);
            this.tbPageMp3.Location = new System.Drawing.Point(4, 24);
            this.tbPageMp3.Name = "tbPageMp3";
            this.tbPageMp3.Padding = new System.Windows.Forms.Padding(3);
            this.tbPageMp3.Size = new System.Drawing.Size(677, 211);
            this.tbPageMp3.TabIndex = 0;
            this.tbPageMp3.Text = "mp3";
            // 
            // lblKFNFile
            // 
            this.lblKFNFile.AutoSize = true;
            this.lblKFNFile.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKFNFile.ForeColor = System.Drawing.Color.White;
            this.lblKFNFile.Location = new System.Drawing.Point(77, 71);
            this.lblKFNFile.Name = "lblKFNFile";
            this.lblKFNFile.Size = new System.Drawing.Size(65, 14);
            this.lblKFNFile.TabIndex = 12;
            this.lblKFNFile.Text = "KfnFile.kfn";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(4, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 14);
            this.label5.TabIndex = 11;
            this.label5.Text = "KFN";
            // 
            // btnTb1Next
            // 
            this.btnTb1Next.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTb1Next.Location = new System.Drawing.Point(593, 184);
            this.btnTb1Next.Name = "btnTb1Next";
            this.btnTb1Next.Size = new System.Drawing.Size(75, 23);
            this.btnTb1Next.TabIndex = 10;
            this.btnTb1Next.Text = "Next";
            this.btnTb1Next.UseVisualStyleBackColor = true;
            this.btnTb1Next.Click += new System.EventHandler(this.btnTb1Next_Click);
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(80, 138);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(503, 23);
            this.txtComment.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(4, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Comment:";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(80, 108);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(281, 23);
            this.txtTitle.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(4, 110);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Title:";
            // 
            // tbPageLyrics
            // 
            this.tbPageLyrics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.tbPageLyrics.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tbPageLyrics.Controls.Add(this.btnTb2Next);
            this.tbPageLyrics.Controls.Add(this.btnTb2Previous);
            this.tbPageLyrics.Controls.Add(this.txtSongINIFile);
            this.tbPageLyrics.Controls.Add(this.lblSongINI);
            this.tbPageLyrics.Controls.Add(this.btnImportSongINIFile);
            this.tbPageLyrics.Location = new System.Drawing.Point(4, 24);
            this.tbPageLyrics.Name = "tbPageLyrics";
            this.tbPageLyrics.Padding = new System.Windows.Forms.Padding(3);
            this.tbPageLyrics.Size = new System.Drawing.Size(677, 211);
            this.tbPageLyrics.TabIndex = 1;
            this.tbPageLyrics.Text = "Lyrics";
            // 
            // btnTb2Next
            // 
            this.btnTb2Next.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTb2Next.Location = new System.Drawing.Point(593, 184);
            this.btnTb2Next.Name = "btnTb2Next";
            this.btnTb2Next.Size = new System.Drawing.Size(75, 23);
            this.btnTb2Next.TabIndex = 12;
            this.btnTb2Next.Text = "Next";
            this.btnTb2Next.UseVisualStyleBackColor = true;
            this.btnTb2Next.Click += new System.EventHandler(this.btnTb2Next_Click);
            // 
            // btnTb2Previous
            // 
            this.btnTb2Previous.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTb2Previous.Location = new System.Drawing.Point(512, 184);
            this.btnTb2Previous.Name = "btnTb2Previous";
            this.btnTb2Previous.Size = new System.Drawing.Size(75, 23);
            this.btnTb2Previous.TabIndex = 11;
            this.btnTb2Previous.Text = "Previous";
            this.btnTb2Previous.UseVisualStyleBackColor = true;
            this.btnTb2Previous.Click += new System.EventHandler(this.btnTb2Previous_Click);
            // 
            // tbPageImages
            // 
            this.tbPageImages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.tbPageImages.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.tbPageImages.Controls.Add(this.btnImportImage);
            this.tbPageImages.Controls.Add(this.lblImage);
            this.tbPageImages.Controls.Add(this.txtImageFile);
            this.tbPageImages.Controls.Add(this.btnTb3Previous);
            this.tbPageImages.Location = new System.Drawing.Point(4, 24);
            this.tbPageImages.Name = "tbPageImages";
            this.tbPageImages.Padding = new System.Windows.Forms.Padding(3);
            this.tbPageImages.Size = new System.Drawing.Size(677, 211);
            this.tbPageImages.TabIndex = 2;
            this.tbPageImages.Text = "Images";
            // 
            // btnImportImage
            // 
            this.btnImportImage.Location = new System.Drawing.Point(590, 35);
            this.btnImportImage.Name = "btnImportImage";
            this.btnImportImage.Size = new System.Drawing.Size(75, 23);
            this.btnImportImage.TabIndex = 15;
            this.btnImportImage.Text = "Import";
            this.btnImportImage.UseVisualStyleBackColor = true;
            this.btnImportImage.Click += new System.EventHandler(this.btnImportImage_Click);
            // 
            // lblImage
            // 
            this.lblImage.AutoSize = true;
            this.lblImage.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImage.ForeColor = System.Drawing.Color.White;
            this.lblImage.Location = new System.Drawing.Point(4, 40);
            this.lblImage.Name = "lblImage";
            this.lblImage.Size = new System.Drawing.Size(24, 15);
            this.lblImage.TabIndex = 13;
            this.lblImage.Text = "Jpg";
            // 
            // txtImageFile
            // 
            this.txtImageFile.Location = new System.Drawing.Point(80, 37);
            this.txtImageFile.Name = "txtImageFile";
            this.txtImageFile.Size = new System.Drawing.Size(500, 23);
            this.txtImageFile.TabIndex = 14;
            // 
            // btnTb3Previous
            // 
            this.btnTb3Previous.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnTb3Previous.Location = new System.Drawing.Point(593, 184);
            this.btnTb3Previous.Name = "btnTb3Previous";
            this.btnTb3Previous.Size = new System.Drawing.Size(75, 23);
            this.btnTb3Previous.TabIndex = 12;
            this.btnTb3Previous.Text = "Previous";
            this.btnTb3Previous.UseVisualStyleBackColor = true;
            this.btnTb3Previous.Click += new System.EventHandler(this.btnTb3Previous_Click);
            // 
            // frmKfnCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.ClientSize = new System.Drawing.Size(695, 316);
            this.Controls.Add(this.tbControl);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreateKfn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmKfnCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Karaboss - Create a new KFN file";
            this.tbControl.ResumeLayout(false);
            this.tbPageMp3.ResumeLayout(false);
            this.tbPageMp3.PerformLayout();
            this.tbPageLyrics.ResumeLayout(false);
            this.tbPageLyrics.PerformLayout();
            this.tbPageImages.ResumeLayout(false);
            this.tbPageImages.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblMp3File;
        private System.Windows.Forms.TextBox txtMp3File;
        private System.Windows.Forms.Button btnImportMp3File;
        private System.Windows.Forms.Button btnImportSongINIFile;
        private System.Windows.Forms.TextBox txtSongINIFile;
        private System.Windows.Forms.Label lblSongINI;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.Button btnCreateKfn;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tbControl;
        private System.Windows.Forms.TabPage tbPageMp3;
        private System.Windows.Forms.Button btnTb1Next;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tbPageLyrics;
        private System.Windows.Forms.Button btnTb2Previous;
        private System.Windows.Forms.TabPage tbPageImages;
        private System.Windows.Forms.Button btnTb2Next;
        private System.Windows.Forms.Button btnTb3Previous;
        private System.Windows.Forms.Button btnImportImage;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.TextBox txtImageFile;
        private System.Windows.Forms.Label lblKFNFile;
        private System.Windows.Forms.Label label5;
    }
}