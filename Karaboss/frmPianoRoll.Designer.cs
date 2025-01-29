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
            this.positionHScrollBar = new ColorSlider.ColorSlider();
            this.CbResolution = new System.Windows.Forms.ComboBox();
            this.lblEdit = new System.Windows.Forms.Label();
            this.lblSaisieNotes = new System.Windows.Forms.Label();
            this.CbTracks = new System.Windows.Forms.ComboBox();
            this.lblNote = new System.Windows.Forms.Label();
            this.pnlDisplay = new System.Windows.Forms.Panel();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.btnTempoMinus = new System.Windows.Forms.Button();
            this.lblTempoValue = new System.Windows.Forms.Label();
            this.btnTempoPlus = new System.Windows.Forms.Button();
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlCenter = new System.Windows.Forms.Panel();
            this.pnlScrollView = new System.Windows.Forms.Panel();
            this.pianoRollControl2 = new Sanford.Multimedia.Midi.PianoRoll.PianoRollControl();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.pnlPiano = new System.Windows.Forms.Panel();
            this.pianoControl2 = new Sanford.Multimedia.Midi.UI.PianoControl();
            this.pnlPianoTop = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.BtnStop = new Karaboss.NoSelectButton();
            this.BtnPlay = new Karaboss.NoSelectButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlTop.SuspendLayout();
            this.pnlDisplay.SuspendLayout();
            this.pnlMiddle.SuspendLayout();
            this.pnlCenter.SuspendLayout();
            this.pnlScrollView.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.pnlPiano.SuspendLayout();
            this.pnlPianoTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlTop.Controls.Add(this.positionHScrollBar);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1149, 50);
            this.pnlTop.TabIndex = 0;
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
            this.positionHScrollBar.Location = new System.Drawing.Point(100, 5);
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
            this.positionHScrollBar.Size = new System.Drawing.Size(812, 44);
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
            this.positionHScrollBar.ValueChanged += new System.EventHandler(this.positionHScrollBar_ValueChanged);
            this.positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.positionHScrollBar_Scroll);
            // 
            // CbResolution
            // 
            this.CbResolution.FormattingEnabled = true;
            this.CbResolution.Location = new System.Drawing.Point(1087, 16);
            this.CbResolution.Name = "CbResolution";
            this.CbResolution.Size = new System.Drawing.Size(50, 21);
            this.CbResolution.TabIndex = 21;
            this.CbResolution.TabStop = false;
            this.toolTip1.SetToolTip(this.CbResolution, "Resolution");
            this.CbResolution.SelectedIndexChanged += new System.EventHandler(this.CbResolution_SelectedIndexChanged);
            // 
            // lblEdit
            // 
            this.lblEdit.BackColor = System.Drawing.Color.White;
            this.lblEdit.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblEdit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblEdit.Location = new System.Drawing.Point(1001, 11);
            this.lblEdit.Name = "lblEdit";
            this.lblEdit.Size = new System.Drawing.Size(30, 30);
            this.lblEdit.TabIndex = 20;
            this.lblEdit.Text = "Edit";
            this.lblEdit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblEdit, "Edit");
            this.lblEdit.Visible = false;
            this.lblEdit.Click += new System.EventHandler(this.lblEdit_Click);
            // 
            // lblSaisieNotes
            // 
            this.lblSaisieNotes.BackColor = System.Drawing.Color.White;
            this.lblSaisieNotes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSaisieNotes.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.lblSaisieNotes.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSaisieNotes.Location = new System.Drawing.Point(1037, 11);
            this.lblSaisieNotes.Name = "lblSaisieNotes";
            this.lblSaisieNotes.Size = new System.Drawing.Size(30, 30);
            this.lblSaisieNotes.TabIndex = 19;
            this.lblSaisieNotes.Text = "N";
            this.lblSaisieNotes.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lblSaisieNotes, "Enter notes");
            this.lblSaisieNotes.Visible = false;
            this.lblSaisieNotes.Click += new System.EventHandler(this.lblSaisieNotes_Click);
            // 
            // CbTracks
            // 
            this.CbTracks.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CbTracks.FormattingEnabled = true;
            this.CbTracks.Location = new System.Drawing.Point(609, 16);
            this.CbTracks.Name = "CbTracks";
            this.CbTracks.Size = new System.Drawing.Size(240, 23);
            this.CbTracks.TabIndex = 2;
            this.CbTracks.TabStop = false;
            this.toolTip1.SetToolTip(this.CbTracks, "Select track to edit");
            this.CbTracks.SelectedIndexChanged += new System.EventHandler(this.CbTracks_SelectedIndexChanged);
            // 
            // lblNote
            // 
            this.lblNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.lblNote.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNote.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNote.ForeColor = System.Drawing.Color.White;
            this.lblNote.Location = new System.Drawing.Point(0, 0);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(100, 19);
            this.lblNote.TabIndex = 4;
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlDisplay
            // 
            this.pnlDisplay.BackColor = System.Drawing.Color.Black;
            this.pnlDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDisplay.Controls.Add(this.lblElapsed);
            this.pnlDisplay.Controls.Add(this.lblPercent);
            this.pnlDisplay.Controls.Add(this.lblDuration);
            this.pnlDisplay.Location = new System.Drawing.Point(400, 11);
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
            // lblTempo
            // 
            this.lblTempo.BackColor = System.Drawing.Color.Black;
            this.lblTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempo.ForeColor = System.Drawing.Color.White;
            this.lblTempo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTempo.Location = new System.Drawing.Point(235, 11);
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
            this.btnTempoMinus.Location = new System.Drawing.Point(196, 11);
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
            this.lblTempoValue.Location = new System.Drawing.Point(132, 11);
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
            this.btnTempoPlus.Location = new System.Drawing.Point(100, 11);
            this.btnTempoPlus.Name = "btnTempoPlus";
            this.btnTempoPlus.Size = new System.Drawing.Size(32, 32);
            this.btnTempoPlus.TabIndex = 26;
            this.btnTempoPlus.Text = "+";
            this.btnTempoPlus.UseVisualStyleBackColor = true;
            this.btnTempoPlus.Click += new System.EventHandler(this.btnTempoPlus_Click);
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.Red;
            this.pnlMiddle.Controls.Add(this.pnlCenter);
            this.pnlMiddle.Controls.Add(this.pnlRight);
            this.pnlMiddle.Controls.Add(this.pnlLeft);
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(0, 50);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Size = new System.Drawing.Size(1149, 441);
            this.pnlMiddle.TabIndex = 0;
            // 
            // pnlCenter
            // 
            this.pnlCenter.BackColor = System.Drawing.Color.Gray;
            this.pnlCenter.Controls.Add(this.pnlScrollView);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(100, 0);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(1009, 441);
            this.pnlCenter.TabIndex = 1;
            // 
            // pnlScrollView
            // 
            this.pnlScrollView.BackColor = System.Drawing.Color.Black;
            this.pnlScrollView.Controls.Add(this.pianoRollControl2);
            this.pnlScrollView.Location = new System.Drawing.Point(20, 9);
            this.pnlScrollView.Name = "pnlScrollView";
            this.pnlScrollView.Size = new System.Drawing.Size(291, 240);
            this.pnlScrollView.TabIndex = 0;
            // 
            // pianoRollControl2
            // 
            this.pianoRollControl2.BackColor = System.Drawing.Color.Gray;
            this.pianoRollControl2.Cursor = System.Windows.Forms.Cursors.Default;
            this.pianoRollControl2.HighNoteID = 108;
            this.pianoRollControl2.Location = new System.Drawing.Point(0, 0);
            this.pianoRollControl2.LowNoteID = 23;
            this.pianoRollControl2.Name = "pianoRollControl2";
            this.pianoRollControl2.NotesEdition = false;
            this.pianoRollControl2.OffsetX = 0;
            this.pianoRollControl2.OffsetY = 0;
            this.pianoRollControl2.Resolution = 4;
            this.pianoRollControl2.Sequence1 = null;
            this.pianoRollControl2.Size = new System.Drawing.Size(206, 240);
            this.pianoRollControl2.TabIndex = 0;
            this.pianoRollControl2.Text = "pianoRollControl2";
            this.pianoRollControl2.TimeLineY = 40;
            this.pianoRollControl2.TrackNum = -1;
            this.pianoRollControl2.Velocity = 100;
            this.pianoRollControl2.xScale = 0.1D;
            this.pianoRollControl2.yScale = 20;
            this.pianoRollControl2.Zoomx = 1F;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlRight.Controls.Add(this.panel1);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(1109, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(40, 441);
            this.pnlRight.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label14);
            this.panel1.Controls.Add(this.label13);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Location = new System.Drawing.Point(5, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(31, 146);
            this.panel1.TabIndex = 52;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.Transparent;
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label14.ForeColor = System.Drawing.Color.White;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(5, 121);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 17);
            this.label14.TabIndex = 66;
            this.label14.Text = "S";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.Transparent;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label13.ForeColor = System.Drawing.Color.White;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(5, 104);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(15, 17);
            this.label13.TabIndex = 65;
            this.label13.Text = "S";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.Transparent;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(4, 87);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(18, 17);
            this.label12.TabIndex = 64;
            this.label12.Text = "O";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(5, 70);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 17);
            this.label11.TabIndex = 63;
            this.label11.Text = "B";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(5, 53);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 17);
            this.label10.TabIndex = 62;
            this.label10.Text = "A";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(5, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 17);
            this.label9.TabIndex = 61;
            this.label9.Text = "R";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(5, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 17);
            this.label8.TabIndex = 60;
            this.label8.Text = "A";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.Transparent;
            this.label15.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.label15.ForeColor = System.Drawing.Color.White;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(5, 2);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 17);
            this.label15.TabIndex = 59;
            this.label15.Text = "K";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlLeft
            // 
            this.pnlLeft.BackColor = System.Drawing.Color.Gray;
            this.pnlLeft.Controls.Add(this.pnlPiano);
            this.pnlLeft.Controls.Add(this.pnlPianoTop);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(100, 441);
            this.pnlLeft.TabIndex = 1;
            // 
            // pnlPiano
            // 
            this.pnlPiano.BackColor = System.Drawing.Color.Gold;
            this.pnlPiano.Controls.Add(this.pianoControl2);
            this.pnlPiano.Location = new System.Drawing.Point(3, 46);
            this.pnlPiano.Name = "pnlPiano";
            this.pnlPiano.Size = new System.Drawing.Size(91, 224);
            this.pnlPiano.TabIndex = 0;
            // 
            // pianoControl2
            // 
            this.pianoControl2.HighNoteID = 108;
            this.pianoControl2.Location = new System.Drawing.Point(12, 9);
            this.pianoControl2.LowNoteID = 23;
            this.pianoControl2.Name = "pianoControl2";
            this.pianoControl2.NoteOnColor = System.Drawing.Color.SkyBlue;
            this.pianoControl2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.pianoControl2.Scale = 20;
            this.pianoControl2.Size = new System.Drawing.Size(67, 200);
            this.pianoControl2.TabIndex = 0;
            this.pianoControl2.Text = "pianoControl2";
            this.pianoControl2.Zoom = 1F;
            // 
            // pnlPianoTop
            // 
            this.pnlPianoTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlPianoTop.Controls.Add(this.lblNote);
            this.pnlPianoTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPianoTop.Location = new System.Drawing.Point(0, 0);
            this.pnlPianoTop.Name = "pnlPianoTop";
            this.pnlPianoTop.Size = new System.Drawing.Size(100, 40);
            this.pnlPianoTop.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(77)))), ((int)(((byte)(95)))));
            this.pnlBottom.Controls.Add(this.BtnStop);
            this.pnlBottom.Controls.Add(this.CbResolution);
            this.pnlBottom.Controls.Add(this.BtnPlay);
            this.pnlBottom.Controls.Add(this.lblSaisieNotes);
            this.pnlBottom.Controls.Add(this.lblEdit);
            this.pnlBottom.Controls.Add(this.pnlDisplay);
            this.pnlBottom.Controls.Add(this.btnTempoPlus);
            this.pnlBottom.Controls.Add(this.lblTempoValue);
            this.pnlBottom.Controls.Add(this.CbTracks);
            this.pnlBottom.Controls.Add(this.lblTempo);
            this.pnlBottom.Controls.Add(this.btnTempoMinus);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 491);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1149, 54);
            this.pnlBottom.TabIndex = 1;
            // 
            // BtnStop
            // 
            this.BtnStop.FlatAppearance.BorderSize = 0;
            this.BtnStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnStop.Image = global::Karaboss.Properties.Resources.btn_black_stop;
            this.BtnStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnStop.Location = new System.Drawing.Point(905, 2);
            this.BtnStop.Name = "BtnStop";
            this.BtnStop.Size = new System.Drawing.Size(50, 50);
            this.BtnStop.TabIndex = 52;
            this.BtnStop.TabStop = false;
            this.toolTip1.SetToolTip(this.BtnStop, "Stop");
            this.BtnStop.UseVisualStyleBackColor = false;
            this.BtnStop.Click += new System.EventHandler(this.BtnStop_Click);
            this.BtnStop.MouseLeave += new System.EventHandler(this.BtnStop_MouseLeave);
            this.BtnStop.MouseHover += new System.EventHandler(this.BtnStop_MouseHover);
            // 
            // BtnPlay
            // 
            this.BtnPlay.FlatAppearance.BorderSize = 0;
            this.BtnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.BtnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.BtnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPlay.Image = global::Karaboss.Properties.Resources.btn_black_play;
            this.BtnPlay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BtnPlay.Location = new System.Drawing.Point(854, 1);
            this.BtnPlay.Name = "BtnPlay";
            this.BtnPlay.Size = new System.Drawing.Size(50, 50);
            this.BtnPlay.TabIndex = 51;
            this.BtnPlay.TabStop = false;
            this.toolTip1.SetToolTip(this.BtnPlay, "Play/pause");
            this.BtnPlay.UseVisualStyleBackColor = false;
            this.BtnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            this.BtnPlay.MouseLeave += new System.EventHandler(this.BtnPlay_MouseLeave);
            this.BtnPlay.MouseHover += new System.EventHandler(this.BtnPlay_MouseHover);
            // 
            // frmPianoRoll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1149, 545);
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmPianoRoll";
            this.Text = "Karaboss - Piano Roll";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPianoRoll_FormClosing);
            this.Load += new System.EventHandler(this.FrmPianoRoll_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmPianoRoll_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmPianoRoll_KeyUp);
            this.Resize += new System.EventHandler(this.FrmPianoRoll_Resize);
            this.pnlTop.ResumeLayout(false);
            this.pnlDisplay.ResumeLayout(false);
            this.pnlDisplay.PerformLayout();
            this.pnlMiddle.ResumeLayout(false);
            this.pnlCenter.ResumeLayout(false);
            this.pnlScrollView.ResumeLayout(false);
            this.pnlRight.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.pnlPiano.ResumeLayout(false);
            this.pnlPianoTop.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.ComboBox CbTracks;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Panel pnlPiano;
        private System.Windows.Forms.Panel pnlScrollView;
        private ColorSlider.ColorSlider positionHScrollBar;
        private System.Windows.Forms.Label lblEdit;
        private System.Windows.Forms.Label lblSaisieNotes;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox CbResolution;
        private System.Windows.Forms.Label lblTempo;
        private System.Windows.Forms.Button btnTempoMinus;
        private System.Windows.Forms.Label lblTempoValue;
        private System.Windows.Forms.Button btnTempoPlus;
        private System.Windows.Forms.Panel pnlDisplay;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblPercent;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlPianoTop;
        private System.Windows.Forms.Panel pnlRight;
        private Sanford.Multimedia.Midi.PianoRoll.PianoRollControl pianoRollControl2;
        private Sanford.Multimedia.Midi.UI.PianoControl pianoControl2;
        private System.Windows.Forms.Panel pnlCenter;
        private NoSelectButton BtnStop;
        private NoSelectButton BtnPlay;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}