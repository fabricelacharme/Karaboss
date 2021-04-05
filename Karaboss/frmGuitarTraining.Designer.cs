namespace Karaboss
{
    partial class frmGuitarTraining
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGuitarTraining));
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlAppName = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.btnTempoMinus = new System.Windows.Forms.Button();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.btnTempoPlus = new System.Windows.Forms.Button();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlTop.SuspendLayout();
            this.pnlAppName.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.pnlAppName);
            this.pnlTop.Controls.Add(this.pnlDisplay);
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
            this.pnlTop.Size = new System.Drawing.Size(1135, 80);
            this.pnlTop.TabIndex = 4;
            // 
            // pnlAppName
            // 
            this.pnlAppName.Controls.Add(this.label1);
            this.pnlAppName.Controls.Add(this.lblVersion);
            this.pnlAppName.Location = new System.Drawing.Point(860, 3);
            this.pnlAppName.Name = "pnlAppName";
            this.pnlAppName.Size = new System.Drawing.Size(200, 31);
            this.pnlAppName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(9, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 25);
            this.label1.TabIndex = 26;
            this.label1.Text = "K A R A B O S S";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.White;
            this.lblVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblVersion.Location = new System.Drawing.Point(146, 10);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 17);
            this.lblVersion.TabIndex = 37;
            this.lblVersion.Text = "1.0.6.1";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblDuration);
            this.pnlDisplay.Location = new System.Drawing.Point(384, 3);
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
            // lblTempo
            // 
            this.lblTempo.BackColor = System.Drawing.Color.Black;
            this.lblTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempo.ForeColor = System.Drawing.Color.White;
            this.lblTempo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempo.Location = new System.Drawing.Point(208, 3);
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
            this.btnTempoMinus.Location = new System.Drawing.Point(176, 3);
            this.btnTempoMinus.Name = "btnTempoMinus";
            this.btnTempoMinus.Size = new System.Drawing.Size(32, 32);
            this.btnTempoMinus.TabIndex = 24;
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
            this.lblTempoValue.Location = new System.Drawing.Point(112, 3);
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
            this.btnTempoPlus.Location = new System.Drawing.Point(80, 3);
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.Size = new System.Drawing.Size(32, 32);
            this.btnTempoPlus.TabIndex = 22;
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
            this.positionHScrollBar.LargeChange = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.positionHScrollBar.Location = new System.Drawing.Point(3, 40);
            this.positionHScrollBar.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.positionHScrollBar.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBar.MouseWheelBarPartitions = 1;
            this.positionHScrollBar.Name = "positionHScrollBar";
            this.positionHScrollBar.ScaleDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBar.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.positionHScrollBar.ShowDivisionsText = true;
            this.positionHScrollBar.ShowSmallScale = false;
            this.positionHScrollBar.Size = new System.Drawing.Size(600, 44);
            this.positionHScrollBar.SmallChange = new decimal(new int[] {
            10,
            0,
            0,
            0});
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
            this.positionHScrollBar.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.positionHScrollBar.ValueChanged += new System.EventHandler(this.positionHScrollBar_ValueChanged);
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // pnlBottom
            // 
            this.pnlBottom.AutoScroll = true;
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Location = new System.Drawing.Point(0, 80);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1135, 412);
            this.pnlBottom.TabIndex = 5;
            // 
            // frmGuitarTraining
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1135, 492);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmGuitarTraining";
            this.Text = "FrmBand";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGuitarTraining_FormClosing);
            this.Load += new System.EventHandler(this.frmGuitarTraining_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmGuitarTraining_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmGuitarTraining_KeyUp);
            this.Resize += new System.EventHandler(this.frmGuitarTraining_Resize);
            this.pnlTop.ResumeLayout(false);
            this.pnlAppName.ResumeLayout(false);
            this.pnlAppName.PerformLayout();
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Panel pnlAppName;
    }
}