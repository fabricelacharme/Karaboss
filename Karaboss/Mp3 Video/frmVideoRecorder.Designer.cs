namespace Karaboss
{
    partial class frmVideoRecorder
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_stopWatch = new System.Windows.Forms.Label();
            this.lb_1 = new System.Windows.Forms.Label();
            this.nud_FPS = new System.Windows.Forms.NumericUpDown();
            this.txtSaveFolder = new System.Windows.Forms.TextBox();
            this.bt_Start = new System.Windows.Forms.Button();
            this.cb_BitRate = new System.Windows.Forms.ComboBox();
            this.cb_VideoCodec = new System.Windows.Forms.ComboBox();
            this.bt_Save = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.lblRight = new System.Windows.Forms.Label();
            this.lblLeft = new System.Windows.Forms.Label();
            this.progressBarRecR = new System.Windows.Forms.ProgressBar();
            this.progressBarRecL = new System.Windows.Forms.ProgressBar();
            this.btnStopMp3 = new System.Windows.Forms.Button();
            this.btnPlayMp3 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lstRecDevices = new System.Windows.Forms.ListBox();
            this.txtSaveToDisk = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nud_FPS)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Frames:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(374, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "FPS:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Save in folder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(374, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 26;
            this.label2.Text = "BitRate:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "VideoCodec:";
            // 
            // lb_stopWatch
            // 
            this.lb_stopWatch.AutoSize = true;
            this.lb_stopWatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_stopWatch.Location = new System.Drawing.Point(4, 81);
            this.lb_stopWatch.Name = "lb_stopWatch";
            this.lb_stopWatch.Size = new System.Drawing.Size(233, 31);
            this.lb_stopWatch.TabIndex = 24;
            this.lb_stopWatch.Text = "00:00:00.0000000";
            // 
            // lb_1
            // 
            this.lb_1.AutoSize = true;
            this.lb_1.Location = new System.Drawing.Point(62, 112);
            this.lb_1.Name = "lb_1";
            this.lb_1.Size = new System.Drawing.Size(13, 13);
            this.lb_1.TabIndex = 23;
            this.lb_1.Text = "0";
            // 
            // nud_FPS
            // 
            this.nud_FPS.Location = new System.Drawing.Point(445, 81);
            this.nud_FPS.Name = "nud_FPS";
            this.nud_FPS.Size = new System.Drawing.Size(93, 20);
            this.nud_FPS.TabIndex = 22;
            this.nud_FPS.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // txtSaveFolder
            // 
            this.txtSaveFolder.Location = new System.Drawing.Point(10, 25);
            this.txtSaveFolder.Name = "txtSaveFolder";
            this.txtSaveFolder.Size = new System.Drawing.Size(447, 20);
            this.txtSaveFolder.TabIndex = 21;
            // 
            // bt_Start
            // 
            this.bt_Start.Location = new System.Drawing.Point(463, 224);
            this.bt_Start.Name = "bt_Start";
            this.bt_Start.Size = new System.Drawing.Size(75, 23);
            this.bt_Start.TabIndex = 20;
            this.bt_Start.Text = "Start";
            this.bt_Start.UseVisualStyleBackColor = true;
            this.bt_Start.Click += new System.EventHandler(this.bt_Start_Click);
            // 
            // cb_BitRate
            // 
            this.cb_BitRate.FormattingEnabled = true;
            this.cb_BitRate.Location = new System.Drawing.Point(445, 54);
            this.cb_BitRate.Name = "cb_BitRate";
            this.cb_BitRate.Size = new System.Drawing.Size(93, 21);
            this.cb_BitRate.TabIndex = 19;
            // 
            // cb_VideoCodec
            // 
            this.cb_VideoCodec.FormattingEnabled = true;
            this.cb_VideoCodec.Location = new System.Drawing.Point(86, 54);
            this.cb_VideoCodec.Name = "cb_VideoCodec";
            this.cb_VideoCodec.Size = new System.Drawing.Size(121, 21);
            this.cb_VideoCodec.TabIndex = 18;
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(383, 224);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(75, 23);
            this.bt_Save.TabIndex = 17;
            this.bt_Save.Text = "Save";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.Location = new System.Drawing.Point(463, 25);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFolder.TabIndex = 30;
            this.btnOpenFolder.Text = "Open";
            this.btnOpenFolder.UseVisualStyleBackColor = true;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(4, 202);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(15, 13);
            this.lblRight.TabIndex = 34;
            this.lblRight.Text = "R";
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(4, 184);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(13, 13);
            this.lblLeft.TabIndex = 33;
            this.lblLeft.Text = "L";
            // 
            // progressBarRecR
            // 
            this.progressBarRecR.Location = new System.Drawing.Point(34, 202);
            this.progressBarRecR.Name = "progressBarRecR";
            this.progressBarRecR.Size = new System.Drawing.Size(307, 13);
            this.progressBarRecR.TabIndex = 32;
            // 
            // progressBarRecL
            // 
            this.progressBarRecL.Location = new System.Drawing.Point(34, 184);
            this.progressBarRecL.Name = "progressBarRecL";
            this.progressBarRecL.Size = new System.Drawing.Size(307, 13);
            this.progressBarRecL.TabIndex = 31;
            // 
            // btnStopMp3
            // 
            this.btnStopMp3.Location = new System.Drawing.Point(114, 221);
            this.btnStopMp3.Name = "btnStopMp3";
            this.btnStopMp3.Size = new System.Drawing.Size(75, 23);
            this.btnStopMp3.TabIndex = 36;
            this.btnStopMp3.Text = "Stop Mp3";
            this.btnStopMp3.UseVisualStyleBackColor = true;
            this.btnStopMp3.Click += new System.EventHandler(this.btnStopMp3_Click);
            // 
            // btnPlayMp3
            // 
            this.btnPlayMp3.Location = new System.Drawing.Point(34, 221);
            this.btnPlayMp3.Name = "btnPlayMp3";
            this.btnPlayMp3.Size = new System.Drawing.Size(75, 23);
            this.btnPlayMp3.TabIndex = 35;
            this.btnPlayMp3.Text = "Play Mp3";
            this.btnPlayMp3.UseVisualStyleBackColor = true;
            this.btnPlayMp3.Click += new System.EventHandler(this.btnPlayMp3_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(559, 9);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(291, 238);
            this.listBox1.TabIndex = 37;
            // 
            // lstRecDevices
            // 
            this.lstRecDevices.FormattingEnabled = true;
            this.lstRecDevices.Location = new System.Drawing.Point(386, 142);
            this.lstRecDevices.Name = "lstRecDevices";
            this.lstRecDevices.Size = new System.Drawing.Size(152, 69);
            this.lstRecDevices.TabIndex = 38;
            // 
            // txtSaveToDisk
            // 
            this.txtSaveToDisk.Location = new System.Drawing.Point(34, 161);
            this.txtSaveToDisk.Name = "txtSaveToDisk";
            this.txtSaveToDisk.Size = new System.Drawing.Size(307, 20);
            this.txtSaveToDisk.TabIndex = 39;
            // 
            // frmVideoRecorder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 253);
            this.Controls.Add(this.txtSaveToDisk);
            this.Controls.Add(this.lstRecDevices);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnStopMp3);
            this.Controls.Add(this.btnPlayMp3);
            this.Controls.Add(this.lblRight);
            this.Controls.Add(this.lblLeft);
            this.Controls.Add(this.progressBarRecR);
            this.Controls.Add(this.progressBarRecL);
            this.Controls.Add(this.btnOpenFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lb_stopWatch);
            this.Controls.Add(this.lb_1);
            this.Controls.Add(this.nud_FPS);
            this.Controls.Add(this.txtSaveFolder);
            this.Controls.Add(this.bt_Start);
            this.Controls.Add(this.cb_BitRate);
            this.Controls.Add(this.cb_VideoCodec);
            this.Controls.Add(this.bt_Save);
            this.Name = "frmVideoRecorder";
            this.Text = "frmVideoRecorder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmVideoRecorder_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nud_FPS)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_stopWatch;
        private System.Windows.Forms.Label lb_1;
        private System.Windows.Forms.NumericUpDown nud_FPS;
        private System.Windows.Forms.TextBox txtSaveFolder;
        private System.Windows.Forms.Button bt_Start;
        private System.Windows.Forms.ComboBox cb_BitRate;
        private System.Windows.Forms.ComboBox cb_VideoCodec;
        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Label lblLeft;
        private System.Windows.Forms.ProgressBar progressBarRecR;
        private System.Windows.Forms.ProgressBar progressBarRecL;
        private System.Windows.Forms.Button btnStopMp3;
        private System.Windows.Forms.Button btnPlayMp3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox lstRecDevices;
        private System.Windows.Forms.TextBox txtSaveToDisk;
    }
}