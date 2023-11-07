namespace TrkControl
{
    partial class TrackControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.lblReverb = new System.Windows.Forms.Label();
            this.lblPan = new System.Windows.Forms.Label();
            this.lblVolume = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.knobReverb = new KnobControl.KnobControl();
            this.knobPan = new KnobControl.KnobControl();
            this.knobVolume = new KnobControl.KnobControl();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lstChannels = new System.Windows.Forms.ListBox();
            this.lblTrack = new System.Windows.Forms.Label();
            this.lstInstruments = new System.Windows.Forms.ListBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.lblLight = new System.Windows.Forms.Label();
            this.lblTrackLabel = new System.Windows.Forms.Label();
            this.lblTrackName = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnMaximized = new TrkControl.NoSelectButton();
            this.btnPianoRoll = new TrkControl.NoSelectButton();
            this.btnSolo = new TrkControl.NoSelectButton();
            this.btnDel = new TrkControl.NoSelectButton();
            this.btnInstrument = new TrkControl.NoSelectButton();
            this.btnMut = new TrkControl.NoSelectButton();
            this.pnlBottom.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(87)))), ((int)(((byte)(151)))));
            this.pnlBottom.Controls.Add(this.btnPianoRoll);
            this.pnlBottom.Controls.Add(this.lblReverb);
            this.pnlBottom.Controls.Add(this.lblPan);
            this.pnlBottom.Controls.Add(this.lblVolume);
            this.pnlBottom.Controls.Add(this.label5);
            this.pnlBottom.Controls.Add(this.label4);
            this.pnlBottom.Controls.Add(this.label3);
            this.pnlBottom.Controls.Add(this.knobReverb);
            this.pnlBottom.Controls.Add(this.knobPan);
            this.pnlBottom.Controls.Add(this.knobVolume);
            this.pnlBottom.Controls.Add(this.label2);
            this.pnlBottom.Controls.Add(this.label1);
            this.pnlBottom.Controls.Add(this.btnSolo);
            this.pnlBottom.Controls.Add(this.lstChannels);
            this.pnlBottom.Controls.Add(this.lblTrack);
            this.pnlBottom.Controls.Add(this.btnDel);
            this.pnlBottom.Controls.Add(this.lstInstruments);
            this.pnlBottom.Controls.Add(this.lblChannel);
            this.pnlBottom.Controls.Add(this.lblLight);
            this.pnlBottom.Controls.Add(this.lblTrackLabel);
            this.pnlBottom.Controls.Add(this.btnInstrument);
            this.pnlBottom.Controls.Add(this.btnMut);
            this.pnlBottom.Location = new System.Drawing.Point(0, 23);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(177, 125);
            this.pnlBottom.TabIndex = 1;
            this.pnlBottom.Click += new System.EventHandler(this.panel1_Click);
            this.pnlBottom.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel1_DragDrop);
            this.pnlBottom.DragOver += new System.Windows.Forms.DragEventHandler(this.panel1_DragOver);
            this.pnlBottom.DragLeave += new System.EventHandler(this.panel1_DragLeave);
            this.pnlBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            // 
            // lblReverb
            // 
            this.lblReverb.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReverb.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblReverb.Location = new System.Drawing.Point(102, 107);
            this.lblReverb.Name = "lblReverb";
            this.lblReverb.Size = new System.Drawing.Size(35, 18);
            this.lblReverb.TabIndex = 21;
            this.lblReverb.Text = "0%";
            this.lblReverb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPan
            // 
            this.lblPan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPan.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblPan.Location = new System.Drawing.Point(60, 107);
            this.lblPan.Name = "lblPan";
            this.lblPan.Size = new System.Drawing.Size(35, 18);
            this.lblPan.TabIndex = 20;
            this.lblPan.Text = "0%";
            this.lblPan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVolume
            // 
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolume.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblVolume.Location = new System.Drawing.Point(18, 107);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(35, 18);
            this.lblVolume.TabIndex = 19;
            this.lblVolume.Text = "0%";
            this.lblVolume.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(98, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 15);
            this.label5.TabIndex = 18;
            this.label5.Text = "Reverb";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(63, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "Pan";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(12, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 16;
            this.label3.Text = "Volume";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // knobReverb
            // 
            this.knobReverb.EndAngle = 405F;
            this.knobReverb.ImeMode = System.Windows.Forms.ImeMode.On;
            this.knobReverb.KnobBackColor = System.Drawing.Color.White;
            this.knobReverb.KnobPointerStyle = KnobControl.KnobControl.KnobPointerStyles.line;
            this.knobReverb.LargeChange = 8;
            this.knobReverb.Location = new System.Drawing.Point(102, 71);
            this.knobReverb.Maximum = 127;
            this.knobReverb.Minimum = 0;
            this.knobReverb.Name = "knobReverb";
            this.knobReverb.PointerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.knobReverb.ScaleColor = System.Drawing.Color.Black;
            this.knobReverb.ScaleDivisions = 11;
            this.knobReverb.ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.knobReverb.ScaleSubDivisions = 4;
            this.knobReverb.ShowLargeScale = false;
            this.knobReverb.ShowSmallScale = false;
            this.knobReverb.Size = new System.Drawing.Size(35, 35);
            this.knobReverb.SmallChange = 4;
            this.knobReverb.StartAngle = 135F;
            this.knobReverb.TabIndex = 15;
            this.knobReverb.TabStop = false;
            this.knobReverb.Value = 64;
            this.knobReverb.ValueChanged += new KnobControl.ValueChangedEventHandler(this.knobReverb_ValueChanged);
            // 
            // knobPan
            // 
            this.knobPan.EndAngle = 405F;
            this.knobPan.ImeMode = System.Windows.Forms.ImeMode.On;
            this.knobPan.KnobBackColor = System.Drawing.Color.White;
            this.knobPan.KnobPointerStyle = KnobControl.KnobControl.KnobPointerStyles.line;
            this.knobPan.LargeChange = 8;
            this.knobPan.Location = new System.Drawing.Point(60, 71);
            this.knobPan.Maximum = 127;
            this.knobPan.Minimum = 0;
            this.knobPan.Name = "knobPan";
            this.knobPan.PointerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.knobPan.ScaleColor = System.Drawing.Color.Black;
            this.knobPan.ScaleDivisions = 8;
            this.knobPan.ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.knobPan.ScaleSubDivisions = 4;
            this.knobPan.ShowLargeScale = false;
            this.knobPan.ShowSmallScale = false;
            this.knobPan.Size = new System.Drawing.Size(35, 35);
            this.knobPan.SmallChange = 4;
            this.knobPan.StartAngle = 135F;
            this.knobPan.TabIndex = 14;
            this.knobPan.TabStop = false;
            this.knobPan.Value = 64;
            this.knobPan.ValueChanged += new KnobControl.ValueChangedEventHandler(this.knobPan_ValueChanged);
            // 
            // knobVolume
            // 
            this.knobVolume.EndAngle = 405F;
            this.knobVolume.ImeMode = System.Windows.Forms.ImeMode.On;
            this.knobVolume.KnobBackColor = System.Drawing.Color.White;
            this.knobVolume.KnobPointerStyle = KnobControl.KnobControl.KnobPointerStyles.line;
            this.knobVolume.LargeChange = 8;
            this.knobVolume.Location = new System.Drawing.Point(18, 71);
            this.knobVolume.Maximum = 127;
            this.knobVolume.Minimum = 0;
            this.knobVolume.MouseWheelBarPartitions = 1;
            this.knobVolume.Name = "knobVolume";
            this.knobVolume.PointerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.knobVolume.ScaleColor = System.Drawing.Color.Black;
            this.knobVolume.ScaleDivisions = 11;
            this.knobVolume.ScaleFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.knobVolume.ScaleSubDivisions = 1;
            this.knobVolume.ShowLargeScale = false;
            this.knobVolume.ShowSmallScale = false;
            this.knobVolume.Size = new System.Drawing.Size(35, 35);
            this.knobVolume.SmallChange = 4;
            this.knobVolume.StartAngle = 135F;
            this.knobVolume.TabIndex = 1;
            this.knobVolume.TabStop = false;
            this.knobVolume.Value = 0;
            this.knobVolume.ValueChanged += new KnobControl.ValueChangedEventHandler(this.knobVolume_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Chn:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Trk:";
            // 
            // lstChannels
            // 
            this.lstChannels.FormattingEnabled = true;
            this.lstChannels.Location = new System.Drawing.Point(111, 37);
            this.lstChannels.Name = "lstChannels";
            this.lstChannels.Size = new System.Drawing.Size(12, 4);
            this.lstChannels.TabIndex = 10;
            this.lstChannels.Visible = false;
            // 
            // lblTrack
            // 
            this.lblTrack.BackColor = System.Drawing.Color.AntiqueWhite;
            this.lblTrack.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrack.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblTrack.Location = new System.Drawing.Point(96, 7);
            this.lblTrack.Name = "lblTrack";
            this.lblTrack.Size = new System.Drawing.Size(20, 16);
            this.lblTrack.TabIndex = 9;
            this.lblTrack.Text = "0";
            this.lblTrack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lstInstruments
            // 
            this.lstInstruments.FormattingEnabled = true;
            this.lstInstruments.Location = new System.Drawing.Point(111, 85);
            this.lstInstruments.Name = "lstInstruments";
            this.lstInstruments.Size = new System.Drawing.Size(12, 4);
            this.lstInstruments.TabIndex = 7;
            this.lstInstruments.Visible = false;
            // 
            // lblChannel
            // 
            this.lblChannel.BackColor = System.Drawing.Color.AntiqueWhite;
            this.lblChannel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblChannel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblChannel.Location = new System.Drawing.Point(41, 7);
            this.lblChannel.Name = "lblChannel";
            this.lblChannel.Size = new System.Drawing.Size(20, 16);
            this.lblChannel.TabIndex = 6;
            this.lblChannel.Text = "15";
            this.lblChannel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblChannel.TextChanged += new System.EventHandler(this.lblChannel_TextChanged);
            this.lblChannel.Click += new System.EventHandler(this.lblChannel_Click);
            // 
            // lblLight
            // 
            this.lblLight.BackColor = System.Drawing.Color.Gray;
            this.lblLight.Location = new System.Drawing.Point(0, 0);
            this.lblLight.Name = "lblLight";
            this.lblLight.Size = new System.Drawing.Size(10, 148);
            this.lblLight.TabIndex = 5;
            this.lblLight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTrackLabel
            // 
            this.lblTrackLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrackLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTrackLabel.Location = new System.Drawing.Point(12, 33);
            this.lblTrackLabel.Name = "lblTrackLabel";
            this.lblTrackLabel.Size = new System.Drawing.Size(122, 18);
            this.lblTrackLabel.TabIndex = 4;
            this.lblTrackLabel.Text = "Instrument";
            this.lblTrackLabel.TextChanged += new System.EventHandler(this.lblTrackLabel_TextChanged);
            // 
            // lblTrackName
            // 
            this.lblTrackName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTrackName.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTrackName.Location = new System.Drawing.Point(3, 3);
            this.lblTrackName.Name = "lblTrackName";
            this.lblTrackName.Size = new System.Drawing.Size(122, 18);
            this.lblTrackName.TabIndex = 0;
            this.lblTrackName.Text = "TrackName";
            this.lblTrackName.TextChanged += new System.EventHandler(this.lblTrackName_TextChanged);
            this.lblTrackName.DoubleClick += new System.EventHandler(this.lblTrackName_DoubleClick);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(29)))), ((int)(((byte)(29)))));
            this.pnlTop.Controls.Add(this.btnMaximized);
            this.pnlTop.Controls.Add(this.lblTrackName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(177, 23);
            this.pnlTop.TabIndex = 0;
            // 
            // btnMaximized
            // 
            this.btnMaximized.BackColor = System.Drawing.Color.Silver;
            this.btnMaximized.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMaximized.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMaximized.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnMaximized.Location = new System.Drawing.Point(154, 0);
            this.btnMaximized.Margin = new System.Windows.Forms.Padding(1);
            this.btnMaximized.Name = "btnMaximized";
            this.btnMaximized.Size = new System.Drawing.Size(23, 23);
            this.btnMaximized.TabIndex = 3;
            this.btnMaximized.TabStop = false;
            this.btnMaximized.Text = "-";
            this.btnMaximized.UseVisualStyleBackColor = false;
            this.btnMaximized.Click += new System.EventHandler(this.btnMaximized_Click);
            // 
            // btnPianoRoll
            // 
            this.btnPianoRoll.Image = global::TrkControl.Properties.Resources.piano;
            this.btnPianoRoll.Location = new System.Drawing.Point(144, 91);
            this.btnPianoRoll.Name = "btnPianoRoll";
            this.btnPianoRoll.Size = new System.Drawing.Size(31, 23);
            this.btnPianoRoll.TabIndex = 22;
            this.btnPianoRoll.TabStop = false;
            this.btnPianoRoll.UseVisualStyleBackColor = true;
            this.btnPianoRoll.Click += new System.EventHandler(this.btnPianoRoll_Click);
            // 
            // btnSolo
            // 
            this.btnSolo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(163)))), ((int)(((byte)(0)))));
            this.btnSolo.ForeColor = System.Drawing.Color.Black;
            this.btnSolo.Location = new System.Drawing.Point(144, 28);
            this.btnSolo.Name = "btnSolo";
            this.btnSolo.Size = new System.Drawing.Size(31, 23);
            this.btnSolo.TabIndex = 11;
            this.btnSolo.TabStop = false;
            this.btnSolo.Text = "so";
            this.btnSolo.UseVisualStyleBackColor = false;
            this.btnSolo.Click += new System.EventHandler(this.btnSolo_Click);
            // 
            // btnDel
            // 
            this.btnDel.BackColor = System.Drawing.Color.Black;
            this.btnDel.ForeColor = System.Drawing.Color.White;
            this.btnDel.Location = new System.Drawing.Point(144, 70);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(31, 23);
            this.btnDel.TabIndex = 8;
            this.btnDel.TabStop = false;
            this.btnDel.Text = "x";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnInstrument
            // 
            this.btnInstrument.Location = new System.Drawing.Point(144, 49);
            this.btnInstrument.Name = "btnInstrument";
            this.btnInstrument.Size = new System.Drawing.Size(31, 23);
            this.btnInstrument.TabIndex = 2;
            this.btnInstrument.TabStop = false;
            this.btnInstrument.Text = "in";
            this.btnInstrument.UseVisualStyleBackColor = true;
            this.btnInstrument.Click += new System.EventHandler(this.btnInstrument_Click);
            // 
            // btnMut
            // 
            this.btnMut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(137)))), ((int)(((byte)(239)))));
            this.btnMut.ForeColor = System.Drawing.Color.White;
            this.btnMut.Location = new System.Drawing.Point(144, 7);
            this.btnMut.Name = "btnMut";
            this.btnMut.Size = new System.Drawing.Size(31, 23);
            this.btnMut.TabIndex = 1;
            this.btnMut.TabStop = false;
            this.btnMut.Text = "mu";
            this.btnMut.UseVisualStyleBackColor = false;
            this.btnMut.Click += new System.EventHandler(this.btnMut_Click);
            // 
            // TrackControl
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Name = "TrackControl";
            this.Size = new System.Drawing.Size(177, 148);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.trackControl_KeyDown);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Label lblTrackName;
        private NoSelectButton btnInstrument;               //System.Windows.Forms.Button btnInstrument;
        private NoSelectButton btnMut;                      //System.Windows.Forms.Button btnMut;
        private System.Windows.Forms.Label lblTrackLabel;
        private System.Windows.Forms.Label lblLight;
        private System.Windows.Forms.Label lblChannel;
        private System.Windows.Forms.ListBox lstInstruments;
        private NoSelectButton btnDel;                      //System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Label lblTrack;
        private System.Windows.Forms.ListBox lstChannels;
        private NoSelectButton btnSolo;                     //System.Windows.Forms.Button btnSolo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private KnobControl.KnobControl knobReverb;
        private KnobControl.KnobControl knobPan;
        private KnobControl.KnobControl knobVolume;
        private System.Windows.Forms.Label lblReverb;
        private System.Windows.Forms.Label lblPan;
        private System.Windows.Forms.Label lblVolume;
        private NoSelectButton btnPianoRoll;                //System.Windows.Forms.Button btnPianoRoll;
        private System.Windows.Forms.Panel pnlTop;
        private NoSelectButton btnMaximized;
    }
}
