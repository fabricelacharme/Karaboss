namespace Karaboss
{
    partial class frmExternalMidiRecord
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
            this.channelListBox = new System.Windows.Forms.ListBox();
            this.channelMessageGroupBox = new System.Windows.Forms.GroupBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.sysExGroupBox = new System.Windows.Forms.GroupBox();
            this.sysExRichTextBox = new System.Windows.Forms.RichTextBox();
            this.systemCommonGroupBox = new System.Windows.Forms.GroupBox();
            this.sysCommonListBox = new System.Windows.Forms.ListBox();
            this.sysRealtimeGroupBox = new System.Windows.Forms.GroupBox();
            this.sysRealtimeListBox = new System.Windows.Forms.ListBox();
            this.timerPlayer = new System.Windows.Forms.Timer(this.components);
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.btnTempoMinus = new System.Windows.Forms.Button();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.btnTempoPlus = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.timerRecorder = new System.Windows.Forms.Timer(this.components);
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.channelMessageGroupBox.SuspendLayout();
            this.sysExGroupBox.SuspendLayout();
            this.systemCommonGroupBox.SuspendLayout();
            this.sysRealtimeGroupBox.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.SuspendLayout();
            // 
            // channelListBox
            // 
            this.channelListBox.FormattingEnabled = true;
            this.channelListBox.Location = new System.Drawing.Point(6, 19);
            this.channelListBox.Name = "channelListBox";
            this.channelListBox.Size = new System.Drawing.Size(258, 147);
            this.channelListBox.TabIndex = 0;
            this.channelListBox.TabStop = false;
            // 
            // channelMessageGroupBox
            // 
            this.channelMessageGroupBox.Controls.Add(this.channelListBox);
            this.channelMessageGroupBox.Location = new System.Drawing.Point(16, 18);
            this.channelMessageGroupBox.Name = "channelMessageGroupBox";
            this.channelMessageGroupBox.Size = new System.Drawing.Size(270, 176);
            this.channelMessageGroupBox.TabIndex = 1;
            this.channelMessageGroupBox.TabStop = false;
            this.channelMessageGroupBox.Text = "Channel Messages";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(578, 219);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 23);
            this.startButton.TabIndex = 2;
            this.startButton.TabStop = false;
            this.startButton.Text = "Start record";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(578, 248);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.TabStop = false;
            this.stopButton.Text = "Stop record";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // sysExGroupBox
            // 
            this.sysExGroupBox.Controls.Add(this.sysExRichTextBox);
            this.sysExGroupBox.Location = new System.Drawing.Point(292, 18);
            this.sysExGroupBox.Name = "sysExGroupBox";
            this.sysExGroupBox.Size = new System.Drawing.Size(367, 176);
            this.sysExGroupBox.TabIndex = 4;
            this.sysExGroupBox.TabStop = false;
            this.sysExGroupBox.Text = "SysEx Messages";
            // 
            // sysExRichTextBox
            // 
            this.sysExRichTextBox.Location = new System.Drawing.Point(6, 19);
            this.sysExRichTextBox.Name = "sysExRichTextBox";
            this.sysExRichTextBox.Size = new System.Drawing.Size(355, 147);
            this.sysExRichTextBox.TabIndex = 7;
            this.sysExRichTextBox.TabStop = false;
            this.sysExRichTextBox.Text = "";
            // 
            // systemCommonGroupBox
            // 
            this.systemCommonGroupBox.Controls.Add(this.sysCommonListBox);
            this.systemCommonGroupBox.Location = new System.Drawing.Point(16, 200);
            this.systemCommonGroupBox.Name = "systemCommonGroupBox";
            this.systemCommonGroupBox.Size = new System.Drawing.Size(270, 176);
            this.systemCommonGroupBox.TabIndex = 5;
            this.systemCommonGroupBox.TabStop = false;
            this.systemCommonGroupBox.Text = "System Common Messages";
            // 
            // sysCommonListBox
            // 
            this.sysCommonListBox.FormattingEnabled = true;
            this.sysCommonListBox.Location = new System.Drawing.Point(6, 19);
            this.sysCommonListBox.Name = "sysCommonListBox";
            this.sysCommonListBox.Size = new System.Drawing.Size(258, 147);
            this.sysCommonListBox.TabIndex = 0;
            this.sysCommonListBox.TabStop = false;
            // 
            // sysRealtimeGroupBox
            // 
            this.sysRealtimeGroupBox.Controls.Add(this.sysRealtimeListBox);
            this.sysRealtimeGroupBox.Location = new System.Drawing.Point(298, 200);
            this.sysRealtimeGroupBox.Name = "sysRealtimeGroupBox";
            this.sysRealtimeGroupBox.Size = new System.Drawing.Size(268, 176);
            this.sysRealtimeGroupBox.TabIndex = 6;
            this.sysRealtimeGroupBox.TabStop = false;
            this.sysRealtimeGroupBox.Text = "System Realtime Messages";
            // 
            // sysRealtimeListBox
            // 
            this.sysRealtimeListBox.FormattingEnabled = true;
            this.sysRealtimeListBox.Location = new System.Drawing.Point(6, 19);
            this.sysRealtimeListBox.Name = "sysRealtimeListBox";
            this.sysRealtimeListBox.Size = new System.Drawing.Size(258, 147);
            this.sysRealtimeListBox.TabIndex = 0;
            this.sysRealtimeListBox.TabStop = false;
            // 
            // timerPlayer
            // 
            this.timerPlayer.Tick += new System.EventHandler(this.timerPlayer_Tick);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.pnlDisplay);
            this.pnlTop.Controls.Add(this.label1);
            this.pnlTop.Controls.Add(this.lblTempo);
            this.pnlTop.Controls.Add(this.btnTempoMinus);
            this.pnlTop.Controls.Add(this.lblTempoValue);
            this.pnlTop.Controls.Add(this.btnTempoPlus);
            this.pnlTop.Controls.Add(this.BtnStop);
            this.pnlTop.Controls.Add(this.BtnPlay);
            this.pnlTop.Controls.Add(this.positionHScrollBar);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(770, 80);
            this.pnlTop.TabIndex = 8;
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblDuration);
            this.pnlDisplay.Location = new System.Drawing.Point(381, 3);
            this.pnlDisplay.Name = "pnlDisplay";
            this.pnlDisplay.Size = new System.Drawing.Size(200, 32);
            this.pnlDisplay.TabIndex = 36;
            // 
            // lblElapsed
            // 
            this.lblElapsed.BackColor = System.Drawing.Color.Transparent;
            this.lblElapsed.Font = new System.Drawing.Font("Consolas", 12F);
            this.lblElapsed.ForeColor = System.Drawing.Color.White;
            this.lblElapsed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblElapsed.Location = new System.Drawing.Point(1, 6);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(60, 19);
            this.lblElapsed.TabIndex = 2;
            this.lblElapsed.Text = "00:00";
            this.lblElapsed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPercent
            // 
            this.lblPercent.AutoSize = true;
            this.lblPercent.BackColor = System.Drawing.Color.Transparent;
            this.lblPercent.Font = new System.Drawing.Font("Consolas", 8F);
            this.lblPercent.ForeColor = System.Drawing.Color.White;
            this.lblPercent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblPercent.Location = new System.Drawing.Point(91, 10);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(19, 13);
            this.lblPercent.TabIndex = 8;
            this.lblPercent.Text = "0%";
            // 
            // lblDuration
            // 
            this.lblDuration.BackColor = System.Drawing.Color.Transparent;
            this.lblDuration.Font = new System.Drawing.Font("Consolas", 12F);
            this.lblDuration.ForeColor = System.Drawing.Color.White;
            this.lblDuration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDuration.Location = new System.Drawing.Point(142, 6);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(60, 19);
            this.lblDuration.TabIndex = 7;
            this.lblDuration.Text = "00:00";
            this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(600, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 27);
            this.label1.TabIndex = 26;
            this.label1.Text = "K A R A B O S S";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTempo
            // 
            this.lblTempo.BackColor = System.Drawing.Color.Black;
            this.lblTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempo.ForeColor = System.Drawing.Color.White;
            this.lblTempo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempo.Location = new System.Drawing.Point(214, 3);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(165, 32);
            this.lblTempo.TabIndex = 25;
            this.lblTempo.Text = "Tempo: 750000 - BPM: 85";
            this.lblTempo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTempoMinus
            // 
            this.btnTempoMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.btnTempoMinus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTempoMinus.Location = new System.Drawing.Point(162, 3);
            this.btnTempoMinus.Name = "btnTempoMinus";
            this.btnTempoMinus.Size = new System.Drawing.Size(32, 32);
            this.btnTempoMinus.TabIndex = 24;
            this.btnTempoMinus.TabStop = false;
            this.btnTempoMinus.Text = "-";
            this.btnTempoMinus.UseVisualStyleBackColor = true;
            this.btnTempoMinus.Click += new System.EventHandler(this.btnTempoMinus_Click);
            // 
            // lblTempoValue
            // 
            this.lblTempoValue.BackColor = System.Drawing.Color.Black;
            this.lblTempoValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempoValue.ForeColor = System.Drawing.Color.White;
            this.lblTempoValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempoValue.Location = new System.Drawing.Point(99, 3);
            this.lblTempoValue.Name = "lblTempoValue";
            this.lblTempoValue.Size = new System.Drawing.Size(64, 32);
            this.lblTempoValue.TabIndex = 23;
            this.lblTempoValue.Text = "100%";
            this.lblTempoValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnTempoPlus
            // 
            this.btnTempoPlus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.btnTempoPlus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnTempoPlus.Location = new System.Drawing.Point(67, 3);
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.Size = new System.Drawing.Size(32, 32);
            this.btnTempoPlus.TabIndex = 22;
            this.btnTempoPlus.TabStop = false;
            this.btnTempoPlus.Text = "+";
            this.btnTempoPlus.UseVisualStyleBackColor = true;
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Image = global::Karaboss.Properties.Resources.Media_Controls_Stop_icon;
            this.BtnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnStop.Location = new System.Drawing.Point(35, 3);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(32, 32);
            this.BtnStop.TabIndex = 21;
            this.BtnStop.TabStop = false;
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BtnPlay
            // 
            this.BtnPlay.Image = global::Karaboss.Properties.Resources.Media_Controls_Play_icon;
            this.BtnPlay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnPlay.Location = new System.Drawing.Point(3, 3);
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.Size = new System.Drawing.Size(32, 32);
            this.BtnPlay.TabIndex = 20;
            this.BtnPlay.TabStop = false;
            this.BtnPlay.UseVisualStyleBackColor = true;
            this.BtnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.btnSave);
            this.pnlMiddle.Controls.Add(this.channelMessageGroupBox);
            this.pnlMiddle.Controls.Add(this.startButton);
            this.pnlMiddle.Controls.Add(this.stopButton);
            this.pnlMiddle.Controls.Add(this.sysRealtimeGroupBox);
            this.pnlMiddle.Controls.Add(this.sysExGroupBox);
            this.pnlMiddle.Controls.Add(this.systemCommonGroupBox);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 80);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(770, 485);
            this.pnlMiddle.TabIndex = 9;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(578, 300);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.TabStop = false;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // timerRecorder
            // 
            this.timerRecorder.Tick += new System.EventHandler(this.timerRecorder_Tick);
            // 
            // positionHScrollBar
            // 
            this.positionHScrollBar.BackColor = System.Drawing.Color.Transparent;
            this.positionHScrollBar.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.positionHScrollBar.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.positionHScrollBar.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.positionHScrollBar.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.positionHScrollBar.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.positionHScrollBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.positionHScrollBar.ForeColor = System.Drawing.Color.White;
            this.positionHScrollBar.LargeChange = ((uint)(10u));
            this.positionHScrollBar.Location = new System.Drawing.Point(3, 40);
            this.positionHScrollBar.MouseWheelBarPartitions = 1;
            this.positionHScrollBar.Name = "positionHScrollBar";
            this.positionHScrollBar.ScaleDivisions = 5;
            this.positionHScrollBar.ScaleSubDivisions = 5;
            this.positionHScrollBar.ShowDivisionsText = true;
            this.positionHScrollBar.ShowSmallScale = false;
            this.positionHScrollBar.Size = new System.Drawing.Size(600, 40);
            this.positionHScrollBar.SmallChange = ((uint)(10u));
            this.positionHScrollBar.TabIndex = 19;
            this.positionHScrollBar.TabStop = false;
            this.positionHScrollBar.Text = "colorSlider1";
            this.positionHScrollBar.ThumbImage = global::Karaboss.Properties.Resources.BTN_Thumb_Blue;
            this.positionHScrollBar.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.positionHScrollBar.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.ThumbSize = new System.Drawing.Size(16, 16);
            this.positionHScrollBar.TickAdd = 0F;
            this.positionHScrollBar.TickColor = System.Drawing.Color.White;
            this.positionHScrollBar.TickDivide = 0F;
            this.positionHScrollBar.Value = 0;
            this.positionHScrollBar.ValueChanged += new System.EventHandler(this.positionHScrollBar_ValueChanged);
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // frmExternalMidiRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.ClientSize = new System.Drawing.Size(770, 565);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Name = "frmExternalMidiRecord";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Midi Watcher";            
            this.Resize += new System.EventHandler(this.frmExternalMidiRecord_Resize);
            this.channelMessageGroupBox.ResumeLayout(false);
            this.sysExGroupBox.ResumeLayout(false);
            this.systemCommonGroupBox.ResumeLayout(false);
            this.sysRealtimeGroupBox.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox channelListBox;
        private System.Windows.Forms.GroupBox channelMessageGroupBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.GroupBox sysExGroupBox;
        private System.Windows.Forms.GroupBox systemCommonGroupBox;
        private System.Windows.Forms.ListBox sysCommonListBox;
        private System.Windows.Forms.GroupBox sysRealtimeGroupBox;
        private System.Windows.Forms.ListBox sysRealtimeListBox;
        private System.Windows.Forms.RichTextBox sysExRichTextBox;
        private System.Windows.Forms.Timer timerPlayer;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Button btnTempoMinus;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Button btnTempoPlus;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Button BtnPlay;
        private ColorSlider.ColorSlider positionHScrollBar;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Timer timerRecorder;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.SaveFileDialog saveMidiFileDialog;
    }
}