namespace Karaboss
{
    partial class frmExternalMidiPlay
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
            this.pnlTop = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cbInstruments = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.timerReceiver = new System.Windows.Forms.Timer(this.components);
            this.saveMidiFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.channelMessageGroupBox.SuspendLayout();
            this.sysExGroupBox.SuspendLayout();
            this.systemCommonGroupBox.SuspendLayout();
            this.sysRealtimeGroupBox.SuspendLayout();
            this.pnlTop.SuspendLayout();
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
            this.startButton.Location = new System.Drawing.Point(16, 12);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(91, 23);
            this.startButton.TabIndex = 2;
            this.startButton.TabStop = false;
            this.startButton.Text = "Start receiving";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(113, 12);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(91, 23);
            this.stopButton.TabIndex = 3;
            this.stopButton.TabStop = false;
            this.stopButton.Text = "Stop receiving";
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
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.label2);
            this.pnlTop.Controls.Add(this.cbInstruments);
            this.pnlTop.Controls.Add(this.label1);
            this.pnlTop.Controls.Add(this.stopButton);
            this.pnlTop.Controls.Add(this.startButton);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(770, 48);
            this.pnlTop.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 27;
            this.label2.Text = "Choose intrument to play:";
            // 
            // cbInstruments
            // 
            this.cbInstruments.FormattingEnabled = true;
            this.cbInstruments.Location = new System.Drawing.Point(386, 12);
            this.cbInstruments.Name = "cbInstruments";
            this.cbInstruments.Size = new System.Drawing.Size(121, 21);
            this.cbInstruments.TabIndex = 10;
            this.cbInstruments.TabStop = false;
            this.cbInstruments.SelectedIndexChanged += new System.EventHandler(this.cbInstruments_SelectedIndexChanged);
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
            // pnlMiddle
            // 
            this.pnlMiddle.Controls.Add(this.channelMessageGroupBox);
            this.pnlMiddle.Controls.Add(this.sysRealtimeGroupBox);
            this.pnlMiddle.Controls.Add(this.sysExGroupBox);
            this.pnlMiddle.Controls.Add(this.systemCommonGroupBox);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 48);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(770, 517);
            this.pnlMiddle.TabIndex = 9;
            // 
            // timerReceiver
            // 
            this.timerReceiver.Tick += new System.EventHandler(this.timerReceiver_Tick);
            // 
            // frmExternalMidiPlay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.ClientSize = new System.Drawing.Size(770, 565);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Name = "frmExternalMidiPlay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Midi Watcher";
            this.channelMessageGroupBox.ResumeLayout(false);
            this.sysExGroupBox.ResumeLayout(false);
            this.systemCommonGroupBox.ResumeLayout(false);
            this.sysRealtimeGroupBox.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
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
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Timer timerReceiver;
        private System.Windows.Forms.SaveFileDialog saveMidiFileDialog;
        private System.Windows.Forms.ComboBox cbInstruments;
        private System.Windows.Forms.Label label2;
    }
}