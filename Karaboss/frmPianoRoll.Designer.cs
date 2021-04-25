using Sanford.Multimedia.Midi;
namespace Karaboss
{
    partial class frmPianoRoll
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPianoRoll));
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
            this.CbResolution = new System.Windows.Forms.ComboBox();
            this.lblEdit = new System.Windows.Forms.Label();
            this.lblSaisieNotes = new System.Windows.Forms.Label();
            this.BtnStop = new System.Windows.Forms.Button();
            this.BtnPlay = new System.Windows.Forms.Button();
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.lblNote = new System.Windows.Forms.Label();
            this.CbTracks = new System.Windows.Forms.ComboBox();
            this.lblPointer = new System.Windows.Forms.Label();
            this.lblPen = new System.Windows.Forms.Label();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlScrollView = new System.Windows.Forms.Panel();
            this.pnlPiano = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlTop.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.SuspendLayout();
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
            this.pnlTop.Controls.Add(this.CbResolution);
            this.pnlTop.Controls.Add(this.lblEdit);
            this.pnlTop.Controls.Add(this.lblSaisieNotes);
            this.pnlTop.Controls.Add(this.BtnStop);
            this.pnlTop.Controls.Add(this.BtnPlay);
            this.pnlTop.Controls.Add(this.positionHScrollBar);
            this.pnlTop.Controls.Add(this.lblNote);
            this.pnlTop.Controls.Add(this.CbTracks);
            this.pnlTop.Controls.Add(this.lblPointer);
            this.pnlTop.Controls.Add(this.lblPen);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1117, 100);
            this.pnlTop.TabIndex = 0;
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
            this.pnlDisplay.TabIndex = 37;
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
            this.label1.Location = new System.Drawing.Point(967, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 27);
            this.label1.TabIndex = 30;
            this.label1.Text = "K A R A B O S S";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.lblTempo.TabIndex = 29;
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
            this.btnTempoMinus.TabIndex = 28;
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
            this.lblTempoValue.TabIndex = 27;
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
            this.btnTempoPlus.TabIndex = 26;
            this.btnTempoPlus.Text = "+";
            this.btnTempoPlus.UseVisualStyleBackColor = true;
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // CbResolution
            // 
            this.CbResolution.FormattingEnabled = true;
            this.CbResolution.Location = new System.Drawing.Point(894, 3);
            this.CbResolution.Name = "CbResolution";
            this.CbResolution.Size = new System.Drawing.Size(50, 21);
            this.CbResolution.TabIndex = 21;
            this.CbResolution.TabStop = false;
            this.CbResolution.SelectedIndexChanged += new System.EventHandler(this.CbResolution_SelectedIndexChanged);
            // 
            // lblEdit
            // 
            this.lblEdit.BackColor = System.Drawing.Color.White;
            this.lblEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEdit.Location = new System.Drawing.Point(596, 3);
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(30, 30);
            this.lblEdit.TabIndex = 20;
            this.lblEdit.Text = "Edit";
            this.lblEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEdit.Visible = false;
            this.lblEdit.Click += new System.EventHandler(this.lblEdit_Click);
            // 
            // lblSaisieNotes
            // 
            this.lblSaisieNotes.BackColor = System.Drawing.Color.White;
            this.lblSaisieNotes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSaisieNotes.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.lblSaisieNotes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSaisieNotes.Location = new System.Drawing.Point(628, 3);
            this.lblSaisieNotes.Name = "lblSaisieNotes";
            this.lblSaisieNotes.Size = new System.Drawing.Size(30, 30);
            this.lblSaisieNotes.TabIndex = 19;
            this.lblSaisieNotes.Text = "N";
            this.lblSaisieNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSaisieNotes.Visible = false;
            this.lblSaisieNotes.Click += new System.EventHandler(this.lblSaisieNotes_Click);
            // 
            // BtnStop
            // 
            this.BtnStop.Image = global::Karaboss.Properties.Resources.Media_Controls_Stop_icon;
            this.BtnStop.Location = new System.Drawing.Point(35, 3);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(32, 32);
            this.BtnStop.TabIndex = 18;
            this.BtnStop.UseVisualStyleBackColor = true;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // BtnPlay
            // 
            this.BtnPlay.Image = global::Karaboss.Properties.Resources.Media_Controls_Play_icon;
            this.BtnPlay.Location = new System.Drawing.Point(3, 3);
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.Size = new System.Drawing.Size(32, 32);
            this.BtnPlay.TabIndex = 17;
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
            5,
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
            this.positionHScrollBar.Name = "positionHScrollBar";
            this.positionHScrollBar.ScaleDivisions = new decimal(new int[] {
            10,
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
            this.positionHScrollBar.Size = new System.Drawing.Size(812, 37);
            this.positionHScrollBar.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.positionHScrollBar.TabIndex = 16;
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
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // lblNote
            // 
            this.lblNote.BackColor = System.Drawing.Color.White;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNote.Location = new System.Drawing.Point(0, 80);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(100, 19);
            this.lblNote.TabIndex = 4;
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CbTracks
            // 
            this.CbTracks.FormattingEnabled = true;
            this.CbTracks.Location = new System.Drawing.Point(671, 3);
            this.CbTracks.Name = "CbTracks";
            this.CbTracks.Size = new System.Drawing.Size(220, 21);
            this.CbTracks.TabIndex = 2;
            this.CbTracks.TabStop = false;
            this.CbTracks.SelectedIndexChanged += new System.EventHandler(this.CbTracks_SelectedIndexChanged);
            // 
            // lblPointer
            // 
            this.lblPointer.BackColor = System.Drawing.Color.Red;
            this.lblPointer.Image = global::Karaboss.Properties.Resources.pointer;
            this.lblPointer.Location = new System.Drawing.Point(783, 64);
            this.lblPointer.Name = "lblPointer";
            this.lblPointer.Size = new System.Drawing.Size(32, 32);
            this.lblPointer.TabIndex = 1;
            this.lblPointer.Visible = false;
            this.lblPointer.Click += new System.EventHandler(this.LblPointer_Click);
            // 
            // lblPen
            // 
            this.lblPen.BackColor = System.Drawing.Color.White;
            this.lblPen.Image = global::Karaboss.Properties.Resources.pencil;
            this.lblPen.Location = new System.Drawing.Point(745, 65);
            this.lblPen.Name = "lblPen";
            this.lblPen.Size = new System.Drawing.Size(32, 32);
            this.lblPen.TabIndex = 0;
            this.lblPen.Visible = false;
            this.lblPen.Click += new System.EventHandler(this.LblPen_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.White;
            this.pnlMiddle.Controls.Add(this.pnlScrollView);
            this.pnlMiddle.Controls.Add(this.pnlPiano);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 100);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(1117, 445);
            this.pnlMiddle.TabIndex = 0;
            // 
            // pnlScrollView
            // 
            this.pnlScrollView.Location = new System.Drawing.Point(100, 0);
            this.pnlScrollView.Name = "pnlScrollView";
            this.pnlScrollView.Size = new System.Drawing.Size(200, 100);
            this.pnlScrollView.TabIndex = 0;
            // 
            // pnlPiano
            // 
            this.pnlPiano.Location = new System.Drawing.Point(0, 0);
            this.pnlPiano.Name = "pnlPiano";
            this.pnlPiano.Size = new System.Drawing.Size(100, 100);
            this.pnlPiano.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmPianoRoll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1117, 545);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPianoRoll";
            this.Text = "frmPianoRollcs";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPianoRoll_FormClosing);
            this.Load += new System.EventHandler(this.FrmPianoRoll_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPianoRoll_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmPianoRoll_KeyUp);
            this.Resize += new System.EventHandler(this.FrmPianoRoll_Resize);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblPen;
        private System.Windows.Forms.Label lblPointer;
        private System.Windows.Forms.ComboBox CbTracks;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlPiano;
        private System.Windows.Forms.Panel pnlScrollView;
        private System.Windows.Forms.Button BtnPlay;
        private ColorSlider.ColorSlider positionHScrollBar;
        private System.Windows.Forms.Button BtnStop;
        private System.Windows.Forms.Label lblEdit;
        private System.Windows.Forms.Label lblSaisieNotes;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox CbResolution;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Button btnTempoMinus;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Button btnTempoPlus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblDuration;
    }
}