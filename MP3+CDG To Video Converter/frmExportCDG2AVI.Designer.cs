namespace MP3GConverter
{
    partial class frmExportCDG2AVI
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
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.tbFileName = new System.Windows.Forms.TextBox();
            this.btBrowseCDG = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.chkBackGraph = new System.Windows.Forms.CheckBox();
            this.tbBackGroundImg = new System.Windows.Forms.TextBox();
            this.btBrowseImg = new System.Windows.Forms.Button();
            this.chkBackGround = new System.Windows.Forms.CheckBox();
            this.tbBackGroundAVI = new System.Windows.Forms.TextBox();
            this.btBackGroundBrowse = new System.Windows.Forms.Button();
            this.lbSaveAs = new System.Windows.Forms.Label();
            this.tbAVIFile = new System.Windows.Forms.TextBox();
            this.btOutputAVI = new System.Windows.Forms.Button();
            this.tbFPS = new System.Windows.Forms.TextBox();
            this.lbFPS = new System.Windows.Forms.Label();
            this.btConvert = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.pbAVI = new System.Windows.Forms.ProgressBar();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.tbFileName);
            this.GroupBox2.Controls.Add(this.btBrowseCDG);
            this.GroupBox2.Location = new System.Drawing.Point(3, 2);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(571, 40);
            this.GroupBox2.TabIndex = 19;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "MP3 + CDG File";
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(9, 13);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.ReadOnly = true;
            this.tbFileName.Size = new System.Drawing.Size(475, 20);
            this.tbFileName.TabIndex = 0;
            // 
            // btBrowseCDG
            // 
            this.btBrowseCDG.Location = new System.Drawing.Point(490, 11);
            this.btBrowseCDG.Name = "btBrowseCDG";
            this.btBrowseCDG.Size = new System.Drawing.Size(68, 23);
            this.btBrowseCDG.TabIndex = 1;
            this.btBrowseCDG.Text = "Browse...";
            this.btBrowseCDG.UseVisualStyleBackColor = true;
            this.btBrowseCDG.Click += new System.EventHandler(this.btBrowseCDG_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.chkBackGraph);
            this.GroupBox3.Controls.Add(this.tbBackGroundImg);
            this.GroupBox3.Controls.Add(this.btBrowseImg);
            this.GroupBox3.Controls.Add(this.chkBackGround);
            this.GroupBox3.Controls.Add(this.tbBackGroundAVI);
            this.GroupBox3.Controls.Add(this.btBackGroundBrowse);
            this.GroupBox3.Controls.Add(this.lbSaveAs);
            this.GroupBox3.Controls.Add(this.tbAVIFile);
            this.GroupBox3.Controls.Add(this.btOutputAVI);
            this.GroupBox3.Controls.Add(this.tbFPS);
            this.GroupBox3.Controls.Add(this.lbFPS);
            this.GroupBox3.Controls.Add(this.btConvert);
            this.GroupBox3.Location = new System.Drawing.Point(3, 54);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(571, 145);
            this.GroupBox3.TabIndex = 20;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "AVI Settings";
            // 
            // chkBackGraph
            // 
            this.chkBackGraph.AutoSize = true;
            this.chkBackGraph.Location = new System.Drawing.Point(7, 79);
            this.chkBackGraph.Name = "chkBackGraph";
            this.chkBackGraph.Size = new System.Drawing.Size(122, 17);
            this.chkBackGraph.TabIndex = 23;
            this.chkBackGraph.Text = "Background graphic";
            this.chkBackGraph.UseVisualStyleBackColor = true;
            this.chkBackGraph.CheckedChanged += new System.EventHandler(this.chkBackGraph_CheckedChanged);
            // 
            // tbBackGroundImg
            // 
            this.tbBackGroundImg.Enabled = false;
            this.tbBackGroundImg.Location = new System.Drawing.Point(128, 77);
            this.tbBackGroundImg.Name = "tbBackGroundImg";
            this.tbBackGroundImg.Size = new System.Drawing.Size(356, 20);
            this.tbBackGroundImg.TabIndex = 21;
            // 
            // btBrowseImg
            // 
            this.btBrowseImg.Enabled = false;
            this.btBrowseImg.Location = new System.Drawing.Point(490, 75);
            this.btBrowseImg.Name = "btBrowseImg";
            this.btBrowseImg.Size = new System.Drawing.Size(75, 23);
            this.btBrowseImg.TabIndex = 22;
            this.btBrowseImg.Text = "Browse...";
            this.btBrowseImg.UseVisualStyleBackColor = true;
            this.btBrowseImg.Click += new System.EventHandler(this.btBrowseImg_Click);
            // 
            // chkBackGround
            // 
            this.chkBackGround.AutoSize = true;
            this.chkBackGround.Location = new System.Drawing.Point(7, 51);
            this.chkBackGround.Name = "chkBackGround";
            this.chkBackGround.Size = new System.Drawing.Size(115, 17);
            this.chkBackGround.TabIndex = 20;
            this.chkBackGround.Text = "Background movie";
            this.chkBackGround.UseVisualStyleBackColor = true;
            this.chkBackGround.CheckedChanged += new System.EventHandler(this.chkBackGround_CheckedChanged);
            // 
            // tbBackGroundAVI
            // 
            this.tbBackGroundAVI.Enabled = false;
            this.tbBackGroundAVI.Location = new System.Drawing.Point(128, 49);
            this.tbBackGroundAVI.Name = "tbBackGroundAVI";
            this.tbBackGroundAVI.Size = new System.Drawing.Size(356, 20);
            this.tbBackGroundAVI.TabIndex = 17;
            // 
            // btBackGroundBrowse
            // 
            this.btBackGroundBrowse.Enabled = false;
            this.btBackGroundBrowse.Location = new System.Drawing.Point(490, 47);
            this.btBackGroundBrowse.Name = "btBackGroundBrowse";
            this.btBackGroundBrowse.Size = new System.Drawing.Size(75, 23);
            this.btBackGroundBrowse.TabIndex = 18;
            this.btBackGroundBrowse.Text = "Browse...";
            this.btBackGroundBrowse.UseVisualStyleBackColor = true;
            this.btBackGroundBrowse.Click += new System.EventHandler(this.btBackGroundBrowse_Click);
            // 
            // lbSaveAs
            // 
            this.lbSaveAs.AutoSize = true;
            this.lbSaveAs.Location = new System.Drawing.Point(76, 22);
            this.lbSaveAs.Name = "lbSaveAs";
            this.lbSaveAs.Size = new System.Drawing.Size(46, 13);
            this.lbSaveAs.TabIndex = 16;
            this.lbSaveAs.Text = "Save as";
            // 
            // tbAVIFile
            // 
            this.tbAVIFile.Location = new System.Drawing.Point(128, 19);
            this.tbAVIFile.Name = "tbAVIFile";
            this.tbAVIFile.Size = new System.Drawing.Size(356, 20);
            this.tbAVIFile.TabIndex = 9;
            // 
            // btOutputAVI
            // 
            this.btOutputAVI.Location = new System.Drawing.Point(490, 17);
            this.btOutputAVI.Name = "btOutputAVI";
            this.btOutputAVI.Size = new System.Drawing.Size(75, 23);
            this.btOutputAVI.TabIndex = 10;
            this.btOutputAVI.Text = "Browse...";
            this.btOutputAVI.UseVisualStyleBackColor = true;
            this.btOutputAVI.Click += new System.EventHandler(this.btOutputAVI_Click);
            // 
            // tbFPS
            // 
            this.tbFPS.Location = new System.Drawing.Point(128, 108);
            this.tbFPS.Name = "tbFPS";
            this.tbFPS.Size = new System.Drawing.Size(67, 20);
            this.tbFPS.TabIndex = 15;
            this.tbFPS.Text = "15";
            this.tbFPS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFPS_KeyPress);
            // 
            // lbFPS
            // 
            this.lbFPS.AutoSize = true;
            this.lbFPS.Location = new System.Drawing.Point(201, 111);
            this.lbFPS.Name = "lbFPS";
            this.lbFPS.Size = new System.Drawing.Size(94, 13);
            this.lbFPS.TabIndex = 12;
            this.lbFPS.Text = "frames per second";
            // 
            // btConvert
            // 
            this.btConvert.Location = new System.Drawing.Point(490, 106);
            this.btConvert.Name = "btConvert";
            this.btConvert.Size = new System.Drawing.Size(75, 23);
            this.btConvert.TabIndex = 13;
            this.btConvert.Text = "Create AVI";
            this.btConvert.UseVisualStyleBackColor = true;
            this.btConvert.Click += new System.EventHandler(this.btConvert_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.pbAVI);
            this.GroupBox1.Location = new System.Drawing.Point(3, 205);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(571, 48);
            this.GroupBox1.TabIndex = 21;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Progress";
            // 
            // pbAVI
            // 
            this.pbAVI.Location = new System.Drawing.Point(7, 19);
            this.pbAVI.Name = "pbAVI";
            this.pbAVI.Size = new System.Drawing.Size(555, 23);
            this.pbAVI.TabIndex = 14;
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "openFileDialog1";
            // 
            // frmExportCDG2AVI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 253);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.MaximizeBox = false;
            this.Name = "frmExportCDG2AVI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MP3+CDG To Video Converter";
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.TextBox tbFileName;
        internal System.Windows.Forms.Button btBrowseCDG;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.CheckBox chkBackGraph;
        internal System.Windows.Forms.TextBox tbBackGroundImg;
        internal System.Windows.Forms.Button btBrowseImg;
        internal System.Windows.Forms.CheckBox chkBackGround;
        internal System.Windows.Forms.TextBox tbBackGroundAVI;
        internal System.Windows.Forms.Button btBackGroundBrowse;
        internal System.Windows.Forms.Label lbSaveAs;
        internal System.Windows.Forms.TextBox tbAVIFile;
        internal System.Windows.Forms.Button btOutputAVI;
        internal System.Windows.Forms.TextBox tbFPS;
        internal System.Windows.Forms.Label lbFPS;
        internal System.Windows.Forms.Button btConvert;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.ProgressBar pbAVI;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog1;
    }
}