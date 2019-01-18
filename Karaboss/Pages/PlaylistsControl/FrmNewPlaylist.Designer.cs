namespace Karaboss
{
    partial class FrmNewPlaylist
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNewPlaylist));
            this.txtPlName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.BtnOk = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.lblStyle = new System.Windows.Forms.Label();
            this.txtStyle = new System.Windows.Forms.TextBox();
            this.lblSong = new System.Windows.Forms.Label();
            this.tvPlaylistGroup = new System.Windows.Forms.TreeView();
            this.imgTrv = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtPlName
            // 
            resources.ApplyResources(this.txtPlName, "txtPlName");
            this.txtPlName.Name = "txtPlName";
            // 
            // lblName
            // 
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.ForeColor = System.Drawing.Color.White;
            this.lblName.Name = "lblName";
            // 
            // BtnOk
            // 
            this.BtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.BtnOk, "BtnOk");
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.UseVisualStyleBackColor = true;
            this.BtnOk.Click += new System.EventHandler(this.BtnOk_Click);
            // 
            // BtnCancel
            // 
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.BtnCancel, "BtnCancel");
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            this.BtnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // lblStyle
            // 
            resources.ApplyResources(this.lblStyle, "lblStyle");
            this.lblStyle.ForeColor = System.Drawing.Color.White;
            this.lblStyle.Name = "lblStyle";
            // 
            // txtStyle
            // 
            resources.ApplyResources(this.txtStyle, "txtStyle");
            this.txtStyle.Name = "txtStyle";
            // 
            // lblSong
            // 
            resources.ApplyResources(this.lblSong, "lblSong");
            this.lblSong.ForeColor = System.Drawing.Color.White;
            this.lblSong.Name = "lblSong";
            // 
            // tvPlaylistGroup
            // 
            resources.ApplyResources(this.tvPlaylistGroup, "tvPlaylistGroup");
            this.tvPlaylistGroup.FullRowSelect = true;
            this.tvPlaylistGroup.HideSelection = false;
            this.tvPlaylistGroup.ImageList = this.imgTrv;
            this.tvPlaylistGroup.ItemHeight = 24;
            this.tvPlaylistGroup.LabelEdit = true;
            this.tvPlaylistGroup.Name = "tvPlaylistGroup";
            // 
            // imgTrv
            // 
            this.imgTrv.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTrv.ImageStream")));
            this.imgTrv.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTrv.Images.SetKeyName(0, "folder.png");
            this.imgTrv.Images.SetKeyName(1, "playlist.png");
            this.imgTrv.Images.SetKeyName(2, "edit.png");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Name = "label1";
            // 
            // FrmNewPlaylist
            // 
            this.AcceptButton = this.BtnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CancelButton = this.BtnCancel;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvPlaylistGroup);
            this.Controls.Add(this.lblSong);
            this.Controls.Add(this.txtStyle);
            this.Controls.Add(this.lblStyle);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtPlName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmNewPlaylist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNewPlaylist_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPlName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Button BtnOk;
        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Label lblStyle;
        private System.Windows.Forms.TextBox txtStyle;
        private System.Windows.Forms.Label lblSong;
        private System.Windows.Forms.TreeView tvPlaylistGroup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imgTrv;
    }
}