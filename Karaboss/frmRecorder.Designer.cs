namespace Karaboss
{
    partial class frmRecorder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRecorder));
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnRecord = new System.Windows.Forms.Button();
            this.btnStopRecord = new System.Windows.Forms.Button();
            this.btnPlayLame = new System.Windows.Forms.Button();
            this.btnStopLame = new System.Windows.Forms.Button();
            this.progressBarRecL = new System.Windows.Forms.ProgressBar();
            this.progressBarRecR = new System.Windows.Forms.ProgressBar();
            this.btnPlayPcm = new System.Windows.Forms.Button();
            this.btnStopPcm = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblLeft = new System.Windows.Forms.Label();
            this.lblRight = new System.Windows.Forms.Label();
            this.chkSaveToDisk = new System.Windows.Forms.CheckBox();
            this.txtSaveToDisk = new System.Windows.Forms.TextBox();
            this.btnPlayMp3 = new System.Windows.Forms.Button();
            this.btnStopMp3 = new System.Windows.Forms.Button();
            this.lstRecDevices = new System.Windows.Forms.ListBox();
            this.lstDevices = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(495, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(291, 238);
            this.listBox1.TabIndex = 0;
            // 
            // btnRecord
            // 
            this.btnRecord.Location = new System.Drawing.Point(12, 12);
            this.btnRecord.Name = "btnRecord";
            this.btnRecord.Size = new System.Drawing.Size(75, 23);
            this.btnRecord.TabIndex = 1;
            this.btnRecord.Text = "Record";
            this.btnRecord.UseVisualStyleBackColor = true;
            this.btnRecord.Click += new System.EventHandler(this.btnRecord_Click);
            // 
            // btnStopRecord
            // 
            this.btnStopRecord.Location = new System.Drawing.Point(93, 12);
            this.btnStopRecord.Name = "btnStopRecord";
            this.btnStopRecord.Size = new System.Drawing.Size(75, 23);
            this.btnStopRecord.TabIndex = 2;
            this.btnStopRecord.Text = "StopRecord";
            this.btnStopRecord.UseVisualStyleBackColor = true;
            this.btnStopRecord.Click += new System.EventHandler(this.btnStopRecord_Click);
            // 
            // btnPlayLame
            // 
            this.btnPlayLame.Location = new System.Drawing.Point(93, 57);
            this.btnPlayLame.Name = "btnPlayLame";
            this.btnPlayLame.Size = new System.Drawing.Size(75, 23);
            this.btnPlayLame.TabIndex = 3;
            this.btnPlayLame.Text = "Play Lame";
            this.btnPlayLame.UseVisualStyleBackColor = true;
            this.btnPlayLame.Click += new System.EventHandler(this.btnPlayLame_Click);
            // 
            // btnStopLame
            // 
            this.btnStopLame.Location = new System.Drawing.Point(93, 86);
            this.btnStopLame.Name = "btnStopLame";
            this.btnStopLame.Size = new System.Drawing.Size(75, 23);
            this.btnStopLame.TabIndex = 4;
            this.btnStopLame.Text = "Stop Lame";
            this.btnStopLame.UseVisualStyleBackColor = true;
            this.btnStopLame.Click += new System.EventHandler(this.btnStopLame_Click);
            // 
            // progressBarRecL
            // 
            this.progressBarRecL.Location = new System.Drawing.Point(39, 224);
            this.progressBarRecL.Name = "progressBarRecL";
            this.progressBarRecL.Size = new System.Drawing.Size(307, 13);
            this.progressBarRecL.TabIndex = 5;
            // 
            // progressBarRecR
            // 
            this.progressBarRecR.Location = new System.Drawing.Point(39, 242);
            this.progressBarRecR.Name = "progressBarRecR";
            this.progressBarRecR.Size = new System.Drawing.Size(307, 13);
            this.progressBarRecR.TabIndex = 6;
            // 
            // btnPlayPcm
            // 
            this.btnPlayPcm.Location = new System.Drawing.Point(12, 57);
            this.btnPlayPcm.Name = "btnPlayPcm";
            this.btnPlayPcm.Size = new System.Drawing.Size(75, 23);
            this.btnPlayPcm.TabIndex = 7;
            this.btnPlayPcm.Text = "Play Pcm";
            this.btnPlayPcm.UseVisualStyleBackColor = true;
            this.btnPlayPcm.Click += new System.EventHandler(this.btnPlayPcm_Click);
            // 
            // btnStopPcm
            // 
            this.btnStopPcm.Location = new System.Drawing.Point(12, 86);
            this.btnStopPcm.Name = "btnStopPcm";
            this.btnStopPcm.Size = new System.Drawing.Size(75, 23);
            this.btnStopPcm.TabIndex = 8;
            this.btnStopPcm.Text = "Stop Pcm";
            this.btnStopPcm.UseVisualStyleBackColor = true;
            this.btnStopPcm.Click += new System.EventHandler(this.btnStopPcm_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(9, 224);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(13, 13);
            this.lblLeft.TabIndex = 9;
            this.lblLeft.Text = "L";
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(9, 242);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(15, 13);
            this.lblRight.TabIndex = 10;
            this.lblRight.Text = "R";
            // 
            // chkSaveToDisk
            // 
            this.chkSaveToDisk.AutoSize = true;
            this.chkSaveToDisk.Checked = true;
            this.chkSaveToDisk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSaveToDisk.Location = new System.Drawing.Point(12, 129);
            this.chkSaveToDisk.Name = "chkSaveToDisk";
            this.chkSaveToDisk.Size = new System.Drawing.Size(85, 17);
            this.chkSaveToDisk.TabIndex = 11;
            this.chkSaveToDisk.Text = "Save to disk";
            this.chkSaveToDisk.UseVisualStyleBackColor = true;
            this.chkSaveToDisk.CheckedChanged += new System.EventHandler(this.chkSaveToDisk_CheckedChanged);
            // 
            // txtSaveToDisk
            // 
            this.txtSaveToDisk.Location = new System.Drawing.Point(12, 152);
            this.txtSaveToDisk.Name = "txtSaveToDisk";
            this.txtSaveToDisk.Size = new System.Drawing.Size(334, 20);
            this.txtSaveToDisk.TabIndex = 12;
            // 
            // btnPlayMp3
            // 
            this.btnPlayMp3.Location = new System.Drawing.Point(13, 179);
            this.btnPlayMp3.Name = "btnPlayMp3";
            this.btnPlayMp3.Size = new System.Drawing.Size(75, 23);
            this.btnPlayMp3.TabIndex = 13;
            this.btnPlayMp3.Text = "Play Mp3";
            this.btnPlayMp3.UseVisualStyleBackColor = true;
            this.btnPlayMp3.Click += new System.EventHandler(this.btnPlayMp3_Click);
            // 
            // btnStopMp3
            // 
            this.btnStopMp3.Location = new System.Drawing.Point(93, 179);
            this.btnStopMp3.Name = "btnStopMp3";
            this.btnStopMp3.Size = new System.Drawing.Size(75, 23);
            this.btnStopMp3.TabIndex = 14;
            this.btnStopMp3.Text = "Stop Mp3";
            this.btnStopMp3.UseVisualStyleBackColor = true;
            this.btnStopMp3.Click += new System.EventHandler(this.btnStopMp3_Click);
            // 
            // lstRecDevices
            // 
            this.lstRecDevices.FormattingEnabled = true;
            this.lstRecDevices.Location = new System.Drawing.Point(337, 12);
            this.lstRecDevices.Name = "lstRecDevices";
            this.lstRecDevices.Size = new System.Drawing.Size(152, 95);
            this.lstRecDevices.TabIndex = 15;
            // 
            // lstDevices
            // 
            this.lstDevices.FormattingEnabled = true;
            this.lstDevices.Location = new System.Drawing.Point(179, 12);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.Size = new System.Drawing.Size(152, 95);
            this.lstDevices.TabIndex = 16;
            // 
            // frmRecorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 267);
            this.Controls.Add(this.lstDevices);
            this.Controls.Add(this.lstRecDevices);
            this.Controls.Add(this.btnStopMp3);
            this.Controls.Add(this.btnPlayMp3);
            this.Controls.Add(this.txtSaveToDisk);
            this.Controls.Add(this.chkSaveToDisk);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.btnStopPcm);
            this.Controls.Add(this.btnPlayPcm);
            this.Controls.Add(this.progressBarRecR);
            this.Controls.Add(this.progressBarRecL);
            this.Controls.Add(this.btnStopLame);
            this.Controls.Add(this.btnPlayLame);
            this.Controls.Add(this.btnStopRecord);
            this.Controls.Add(this.btnRecord);
            this.Controls.Add(this.listBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRecorder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmRecorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRecorder_FormClosing);
            this.Load += new System.EventHandler(this.frmRecorder_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnRecord;
        private System.Windows.Forms.Button btnStopRecord;
        private System.Windows.Forms.Button btnPlayLame;
        private System.Windows.Forms.Button btnStopLame;
        private System.Windows.Forms.ProgressBar progressBarRecL;
        private System.Windows.Forms.ProgressBar progressBarRecR;
        private System.Windows.Forms.Button btnPlayPcm;
        private System.Windows.Forms.Button btnStopPcm;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.CheckBox chkSaveToDisk;
        private System.Windows.Forms.TextBox txtSaveToDisk;
        private System.Windows.Forms.Button btnPlayMp3;
        private System.Windows.Forms.Button btnStopMp3;
        private System.Windows.Forms.ListBox lstRecDevices;
        private System.Windows.Forms.ListBox lstDevices;
    }
}