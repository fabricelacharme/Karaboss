namespace Karaboss.Kfn
{
    partial class frmKfnExport
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
            this.lblVideo = new System.Windows.Forms.Label();
            this.lblAudio = new System.Windows.Forms.Label();
            this.lblLyric = new System.Windows.Forms.Label();
            this.cbVideoSelect = new System.Windows.Forms.ComboBox();
            this.cbAudioSelect = new System.Windows.Forms.ComboBox();
            this.cbLyricSelect = new System.Windows.Forms.ComboBox();
            this.btnPlayVideo = new System.Windows.Forms.Button();
            this.btnPlayAudio = new System.Windows.Forms.Button();
            this.chkDeleteID3Tags = new System.Windows.Forms.CheckBox();
            this.lblArtist = new System.Windows.Forms.Label();
            this.cbArtistSelect = new System.Windows.Forms.ComboBox();
            this.cbTitleSelect = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cbEncSelect = new System.Windows.Forms.ComboBox();
            this.lblEnc = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtLyricPreview = new System.Windows.Forms.TextBox();
            this.lblLyricPreview = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblVideo
            // 
            this.lblVideo.AutoSize = true;
            this.lblVideo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVideo.ForeColor = System.Drawing.Color.White;
            this.lblVideo.Location = new System.Drawing.Point(12, 36);
            this.lblVideo.Name = "lblVideo";
            this.lblVideo.Size = new System.Drawing.Size(45, 17);
            this.lblVideo.TabIndex = 0;
            this.lblVideo.Text = "Video:";
            // 
            // lblAudio
            // 
            this.lblAudio.AutoSize = true;
            this.lblAudio.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAudio.ForeColor = System.Drawing.Color.White;
            this.lblAudio.Location = new System.Drawing.Point(12, 63);
            this.lblAudio.Name = "lblAudio";
            this.lblAudio.Size = new System.Drawing.Size(45, 17);
            this.lblAudio.TabIndex = 1;
            this.lblAudio.Text = "Audio:";
            // 
            // lblLyric
            // 
            this.lblLyric.AutoSize = true;
            this.lblLyric.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLyric.ForeColor = System.Drawing.Color.White;
            this.lblLyric.Location = new System.Drawing.Point(12, 90);
            this.lblLyric.Name = "lblLyric";
            this.lblLyric.Size = new System.Drawing.Size(36, 17);
            this.lblLyric.TabIndex = 2;
            this.lblLyric.Text = "Lyric:";
            // 
            // cbVideoSelect
            // 
            this.cbVideoSelect.FormattingEnabled = true;
            this.cbVideoSelect.Location = new System.Drawing.Point(63, 33);
            this.cbVideoSelect.Name = "cbVideoSelect";
            this.cbVideoSelect.Size = new System.Drawing.Size(422, 21);
            this.cbVideoSelect.TabIndex = 3;
            this.cbVideoSelect.SelectedIndexChanged += new System.EventHandler(this.cbVideoSelect_SelectedIndexChanged);
            // 
            // cbAudioSelect
            // 
            this.cbAudioSelect.FormattingEnabled = true;
            this.cbAudioSelect.Location = new System.Drawing.Point(63, 60);
            this.cbAudioSelect.Name = "cbAudioSelect";
            this.cbAudioSelect.Size = new System.Drawing.Size(422, 21);
            this.cbAudioSelect.TabIndex = 4;
            this.cbAudioSelect.SelectedIndexChanged += new System.EventHandler(this.cbAudioSelect_SelectedIndexChanged);
            // 
            // cbLyricSelect
            // 
            this.cbLyricSelect.FormattingEnabled = true;
            this.cbLyricSelect.Location = new System.Drawing.Point(63, 87);
            this.cbLyricSelect.Name = "cbLyricSelect";
            this.cbLyricSelect.Size = new System.Drawing.Size(422, 21);
            this.cbLyricSelect.TabIndex = 5;
            this.cbLyricSelect.SelectedIndexChanged += new System.EventHandler(this.cbLyricSelect_SelectedIndexChanged);
            // 
            // btnPlayVideo
            // 
            this.btnPlayVideo.Image = global::Karaboss.Properties.Resources.Media_Controls_Play_icon;
            this.btnPlayVideo.Location = new System.Drawing.Point(491, 33);
            this.btnPlayVideo.Name = "btnPlayVideo";
            this.btnPlayVideo.Size = new System.Drawing.Size(51, 23);
            this.btnPlayVideo.TabIndex = 6;
            this.btnPlayVideo.UseVisualStyleBackColor = true;
            this.btnPlayVideo.Click += new System.EventHandler(this.btnPlayVideo_Click);
            // 
            // btnPlayAudio
            // 
            this.btnPlayAudio.Image = global::Karaboss.Properties.Resources.Media_Controls_Play_icon;
            this.btnPlayAudio.Location = new System.Drawing.Point(491, 60);
            this.btnPlayAudio.Name = "btnPlayAudio";
            this.btnPlayAudio.Size = new System.Drawing.Size(51, 23);
            this.btnPlayAudio.TabIndex = 7;
            this.btnPlayAudio.UseVisualStyleBackColor = true;
            this.btnPlayAudio.Click += new System.EventHandler(this.btnPlayAudio_Click);
            // 
            // chkDeleteID3Tags
            // 
            this.chkDeleteID3Tags.AutoSize = true;
            this.chkDeleteID3Tags.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDeleteID3Tags.ForeColor = System.Drawing.Color.White;
            this.chkDeleteID3Tags.Location = new System.Drawing.Point(15, 10);
            this.chkDeleteID3Tags.Name = "chkDeleteID3Tags";
            this.chkDeleteID3Tags.Size = new System.Drawing.Size(133, 21);
            this.chkDeleteID3Tags.TabIndex = 8;
            this.chkDeleteID3Tags.Text = "Delete all ID3 tags";
            this.chkDeleteID3Tags.UseVisualStyleBackColor = true;
            this.chkDeleteID3Tags.Visible = false;
            this.chkDeleteID3Tags.CheckedChanged += new System.EventHandler(this.chkDeleteID3Tags_CheckedChanged);
            // 
            // lblArtist
            // 
            this.lblArtist.AutoSize = true;
            this.lblArtist.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblArtist.ForeColor = System.Drawing.Color.White;
            this.lblArtist.Location = new System.Drawing.Point(60, 125);
            this.lblArtist.Name = "lblArtist";
            this.lblArtist.Size = new System.Drawing.Size(41, 17);
            this.lblArtist.TabIndex = 9;
            this.lblArtist.Text = "Artist:";
            // 
            // cbArtistSelect
            // 
            this.cbArtistSelect.FormattingEnabled = true;
            this.cbArtistSelect.Location = new System.Drawing.Point(107, 125);
            this.cbArtistSelect.Name = "cbArtistSelect";
            this.cbArtistSelect.Size = new System.Drawing.Size(378, 21);
            this.cbArtistSelect.TabIndex = 10;
            this.cbArtistSelect.SelectedIndexChanged += new System.EventHandler(this.cbArtistSelect_SelectedIndexChanged);
            // 
            // cbTitleSelect
            // 
            this.cbTitleSelect.FormattingEnabled = true;
            this.cbTitleSelect.Location = new System.Drawing.Point(107, 152);
            this.cbTitleSelect.Name = "cbTitleSelect";
            this.cbTitleSelect.Size = new System.Drawing.Size(378, 21);
            this.cbTitleSelect.TabIndex = 12;
            this.cbTitleSelect.SelectedIndexChanged += new System.EventHandler(this.cbTitleSelect_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(60, 152);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(35, 17);
            this.lblTitle.TabIndex = 11;
            this.lblTitle.Text = "Title:";
            // 
            // cbEncSelect
            // 
            this.cbEncSelect.FormattingEnabled = true;
            this.cbEncSelect.Location = new System.Drawing.Point(200, 179);
            this.cbEncSelect.Name = "cbEncSelect";
            this.cbEncSelect.Size = new System.Drawing.Size(285, 21);
            this.cbEncSelect.TabIndex = 14;
            this.cbEncSelect.SelectedIndexChanged += new System.EventHandler(this.cbEncSelect_SelectedIndexChanged);
            // 
            // lblEnc
            // 
            this.lblEnc.AutoSize = true;
            this.lblEnc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnc.ForeColor = System.Drawing.Color.White;
            this.lblEnc.Location = new System.Drawing.Point(60, 179);
            this.lblEnc.Name = "lblEnc";
            this.lblEnc.Size = new System.Drawing.Size(134, 17);
            this.lblEnc.TabIndex = 13;
            this.lblEnc.Text = "Export with encoding:";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(138, 229);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 15;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(262, 229);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtLyricPreview
            // 
            this.txtLyricPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.txtLyricPreview.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLyricPreview.ForeColor = System.Drawing.Color.White;
            this.txtLyricPreview.Location = new System.Drawing.Point(548, 34);
            this.txtLyricPreview.Multiline = true;
            this.txtLyricPreview.Name = "txtLyricPreview";
            this.txtLyricPreview.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLyricPreview.Size = new System.Drawing.Size(452, 404);
            this.txtLyricPreview.TabIndex = 17;
            // 
            // lblLyricPreview
            // 
            this.lblLyricPreview.AutoSize = true;
            this.lblLyricPreview.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLyricPreview.ForeColor = System.Drawing.Color.White;
            this.lblLyricPreview.Location = new System.Drawing.Point(545, 11);
            this.lblLyricPreview.Name = "lblLyricPreview";
            this.lblLyricPreview.Size = new System.Drawing.Size(85, 17);
            this.lblLyricPreview.TabIndex = 18;
            this.lblLyricPreview.Text = "Lyric preview:";
            // 
            // frmKfnExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.ClientSize = new System.Drawing.Size(1012, 450);
            this.Controls.Add(this.lblLyricPreview);
            this.Controls.Add(this.txtLyricPreview);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.cbEncSelect);
            this.Controls.Add(this.lblEnc);
            this.Controls.Add(this.cbTitleSelect);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cbArtistSelect);
            this.Controls.Add(this.lblArtist);
            this.Controls.Add(this.chkDeleteID3Tags);
            this.Controls.Add(this.btnPlayAudio);
            this.Controls.Add(this.btnPlayVideo);
            this.Controls.Add(this.cbLyricSelect);
            this.Controls.Add(this.cbAudioSelect);
            this.Controls.Add(this.cbVideoSelect);
            this.Controls.Add(this.lblLyric);
            this.Controls.Add(this.lblAudio);
            this.Controls.Add(this.lblVideo);
            this.Name = "frmKfnExport";
            this.Text = "frmKfnExport";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmKfnExport_FormClosing);
            this.Load += new System.EventHandler(this.frmKfnExport_Load);
            this.Resize += new System.EventHandler(this.frmKfnExport_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVideo;
        private System.Windows.Forms.Label lblAudio;
        private System.Windows.Forms.Label lblLyric;
        private System.Windows.Forms.ComboBox cbVideoSelect;
        private System.Windows.Forms.ComboBox cbAudioSelect;
        private System.Windows.Forms.ComboBox cbLyricSelect;
        private System.Windows.Forms.Button btnPlayVideo;
        private System.Windows.Forms.Button btnPlayAudio;
        private System.Windows.Forms.CheckBox chkDeleteID3Tags;
        private System.Windows.Forms.Label lblArtist;
        private System.Windows.Forms.ComboBox cbArtistSelect;
        private System.Windows.Forms.ComboBox cbTitleSelect;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cbEncSelect;
        private System.Windows.Forms.Label lblEnc;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtLyricPreview;
        private System.Windows.Forms.Label lblLyricPreview;
    }
}