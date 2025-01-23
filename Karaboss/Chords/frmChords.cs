#region License

/* Copyright (c) 2024 Fabrice Lacharme
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using ChordAnalyser.UI;
using Karaboss.Display;
using Karaboss.Lyrics;
using Karaboss.Utilities;
using MusicTxt;
using MusicXml;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Karaboss
{
    public partial class frmChords : Form
    {
        #region private dcl

        MusicXmlReader MXmlReader; 
        MusicTxtReader MTxtReader;

        private bool bfilemodified = false;

        private bool closing = false;
        private bool bClosingRequired = false; // Quit after saving file
        private bool scrolling = false;

        // Current file beeing edited
        private string MIDIfileName; // = string.Empty;
        private string MIDIfilePath; // = string.Empty;
        private string MIDIfileFullPath; // = string.Empty;

        private readonly string m_SepLine = "/";
        private readonly string m_SepParagraph = "\\";

        private int newstart = 0;
        private int nbstop = 0;


        /// <summary>
        /// Player status
        /// </summary>
        private enum PlayerStates
        {
            Playing,
            Paused,
            Stopped,
            NextSong,
            Waiting,
            WaitingPaused
        }
        private PlayerStates PlayerState;

        #region controls
        private Sequence sequence1 = new Sequence();               
        private readonly OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();

        private System.Windows.Forms.Timer timer1;
        private NoSelectButton btnPlay;
        private NoSelectButton btnRewind;

        private NoSelectButton btnZoomPlus;
        private NoSelectButton btnZoomMinus;

        private NoSelectButton btnPrintTXT;
        private NoSelectButton btnPrintPDF;

        private Label lblMeasures;
        private NumericUpDown UpDMeasures;

        private Label lblDisplayLyrics;
        private CheckBox chkDisplayLyrics;

        private Label lblCellsize;
        private NumericUpDown UpdCellsize;


        // 1 rst TAB
        private PanelPlayer panelPlayer;
        private ColorSlider.ColorSlider positionHScrollBar;
        private ChordsControl ChordControl1;        
        private ChordRenderer ChordRendererGuitar; // Display bitmaps of chords used in the song played
        private ChordRenderer ChordRendererPiano;

        //Panels
        private Panel pnlDisplayHorz;           // chords in horizontal mode
        private readonly int padding = 10;
        private Panel pnlDisplayImagesOfChords; // images of chords
        private Panel pnlBottom;                // Lyrics

        // Tabpage for image of chord (Guitar & Piano)        
        private TabControl tbPChords;

        private Label lblLyrics;
        private Label lblOtherLyrics;
        

        // 2 nd TAB 
        private ChordsMapControl ChordMapControl1;
        private Panel pnlDisplayMap;       // chords in map mode        

        // 3 rd TAB
        private Panel pnlDisplayWords;
        private System.Windows.Forms.TextBox txtDisplayWords;

        // 4th TAB
        private ChordsMapControl ChordMapControlModify;
        private Panel pnlModifyMap;       // chords in map mode  
        private frmEditChord frmEditChord;

        private int frmxPos;
        private int frmyPos;
        private int xPos; 
        private int yPos;

        #endregion controls


        // Midifile characteristics
        private double _duration = 0;  // en secondes
        private double _durationPercent = 0;
        private int _totalTicks = 0;        
        private double _ppqn;
        private int _tempo;
        private int _measurelen = 0;
        private int NbMeasures;
        private int _currentMeasure = -1;        
        private int _currentTimeInMeasure = -1;
        private int _currentLine = 1;

        // Lyrics 
        private LyricsMgmt myLyricsMgmt;
        private frmExplorer frmExplorer;
        
        // New search (by beat)        
        public Dictionary<int, (string, int)> GridBeatChords;

        #endregion private dcl

        public frmChords(OutputDevice OtpDev, string FileName)
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);

            if (FileName != null)
            {
                string ext = Path.GetExtension(MIDIfileFullPath).ToLower();
                switch (ext)
                {
                    case ".musicxml":
                    case ".mxl":
                    case ".xml":
                        //mnuFileSave.Enabled = false;
                        //mnuFileSaveAs.Enabled = false;
                        break;
                }
            }

            outDevice = OtpDev;

            // Allow form keydown
            this.KeyPreview = true;

            // Title
            SetTitle(FileName);
           
        }


        #region Display Controls

        private void LoadProperties()
        {
            try
            {
                UpdCellsize.Value = Properties.Settings.Default.ChordsMapCellSize;
                UpDMeasures.Value = Properties.Settings.Default.ChordsMapColumns;

                ChordMapControl1.Zoom = Properties.Settings.Default.ChordsMapZoom;
                ChordMapControlModify.Zoom = Properties.Settings.Default.ChordsMapModifyZoom;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

        }

        /// <summary>
        /// Sets title of form
        /// </summary>
        /// <param name="fileName"></param>
        private void SetTitle(string fileName)
        {
            Text = "Karaboss - " + Path.GetFileName(fileName);
        }

        private void LoadSequencer(Sequence seq)
        {
            try
            {
                sequence1 = seq;

                sequencer1 = new Sequencer() {
                    Position = 0,
                    Sequence = sequence1,    // primordial !!!!!
                };
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);                
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                sequence1.Clean();               

                // PlayerState = stopped
                ResetSequencer();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
       

        /// <summary>
        /// Draw all controls
        /// </summary>
        private void DrawControls()
        {
            // Timer
            timer1 = new Timer() {
                Interval = 20,
            };
            timer1.Tick += new EventHandler(timer1_Tick);
            

            #region Toolbar
            pnlToolbar.Location = new Point(0, menuStrip1.Height);
            pnlToolbar.Size = new Size(Width, 55);
            pnlToolbar.BackColor = Color.FromArgb(70, 77, 95);

            // Button Rewind
            btnRewind = new NoSelectButton();
            btnRewind.FlatAppearance.BorderSize = 0;
            btnRewind.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnRewind.FlatStyle = FlatStyle.Flat;
            btnRewind.Parent = pnlToolbar;
            btnRewind.Location = new Point(2, 2);
            btnRewind.Size = new Size(50, 50);
            btnRewind.Image = Properties.Resources.btn_black_prev;
            btnRewind.Click += new EventHandler(btnRewind_Click);
            btnRewind.MouseHover += new EventHandler(btnRewind_MouseHover);
            btnRewind.MouseLeave += new EventHandler(btnRewind_MouseLeave);
            pnlToolbar.Controls.Add(btnRewind);

            // Button play
            btnPlay = new NoSelectButton();
            btnPlay.FlatAppearance.BorderSize = 0;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            btnPlay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            btnPlay.Parent = pnlToolbar;
            btnPlay.Location = new Point(2 + btnRewind.Width, 2);
            btnPlay.Size = new Size(50, 50);
            btnPlay.Image = Properties.Resources.btn_black_play;
            btnPlay.Click += new EventHandler(btnPlay_Click);
            btnPlay.MouseHover += new EventHandler(btnPlay_MouseHover);
            btnPlay.MouseLeave += new EventHandler(btnPlay_MouseLeave);
            pnlToolbar.Controls.Add(btnPlay);

            #region PanelPlay

            panelPlayer = new PanelPlayer() {
                Parent = pnlToolbar,
                Location = new Point(30 + btnPlay.Left + btnPlay.Width, 5),
            };
            pnlToolbar.Controls.Add(panelPlayer);

            #endregion PanelPlay


            #region zoom

            btnZoomPlus = new NoSelectButton()
            {
                Parent = pnlToolbar,
                Image = Karaboss.Properties.Resources.magnifyplus24,
                UseVisualStyleBackColor = true,
                Location = new Point(34 + panelPlayer.Left + panelPlayer.Width, 2),
                Size = new Size(50, 50),
                Text = "",
                //Visible = false,
            };
            btnZoomPlus.Click += new EventHandler(btnZoomPlus_Click);            
            pnlToolbar.Controls.Add(btnZoomPlus);
            toolTip1.SetToolTip(btnZoomPlus, "100%");

            btnZoomMinus = new NoSelectButton() {
                Parent = pnlToolbar,
                Image = Karaboss.Properties.Resources.magnifyminus24,
                UseVisualStyleBackColor = true,
                Location = new Point(2 + btnZoomPlus.Left + btnZoomPlus.Width, 2),
                Size = new Size(50, 50),
                Text = "",
                //Visible = false,
            };                        
            btnZoomMinus.Click += new EventHandler(btnZoomMinus_Click);
            pnlToolbar.Controls.Add(btnZoomMinus);
            
            toolTip1.SetToolTip(btnZoomMinus, "100%");

            #endregion zoom


            #region export pdf text

            btnPrintPDF = new NoSelectButton() {
                Parent = pnlToolbar,
                Image = Properties.Resources.export_pdf32,
                UseVisualStyleBackColor = true,
                Location = new Point(2 + btnZoomMinus.Left + btnZoomMinus.Width),
                Size = new Size(50, 50),
                Text = "",
                Visible = false,
            };

            btnPrintPDF.Click += new EventHandler(btnPrintPDF_Click);            
            pnlToolbar.Controls.Add((btnPrintPDF));
            toolTip1.SetToolTip(btnPrintPDF, "Export to PDF");

            btnPrintTXT = new NoSelectButton() {
                Parent = pnlToolbar,
                Image = Properties.Resources.export_txt48_2,
                UseVisualStyleBackColor = true,
                Location = new Point(2 + btnPrintPDF.Left + btnPrintPDF.Width),
                Size = new Size(50, 50),
                Text = "",  
                Visible = false,
            };
            btnPrintTXT.Click += new EventHandler(btnPrintTXT_Click);
            pnlToolbar.Controls.Add((btnPrintTXT));
            toolTip1.SetToolTip(btnPrintTXT, "Export to Text");

            #endregion export pdf text


            #region Tools for editing chords

            // ==============================
            // Number of measures per line
            // ==============================
            lblMeasures = new Label()
            {
                Parent = pnlToolbar,
                Location = new Point(2 + btnPrintTXT.Left + btnPrintTXT.Width, 8),
                Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel),
                Text = Karaboss.Resources.Localization.Strings.MeasuresPerLine, // "Measures per line",
                AutoSize = true,
                ForeColor = Color.White,          
                Visible=false,
            };
            pnlToolbar.Controls.Add(lblMeasures);

            UpDMeasures = new NumericUpDown()
            {
                Parent = pnlToolbar,
                Location = new Point(10 + lblMeasures.Left + lblMeasures.Width, 6),                
                Minimum = 1,
                Value = 4,
                Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel),
                Size = new Size(43, 22),
                Visible = false,
            };
            UpDMeasures.ValueChanged += new EventHandler(UpdMeasures_ValueChanged);
            pnlToolbar.Controls.Add(UpDMeasures);


            // ==============================
            // Display lyrics
            // ==============================
            lblDisplayLyrics = new Label()
            {
                Parent = pnlToolbar,
                Location = new Point(lblMeasures.Left, 33),
                Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel),
                Text = Karaboss.Resources.Localization.Strings.DisplayLyrics, // "Display lyrics",
                AutoSize = true,
                ForeColor = Color.White,
                Visible = false,
            };
            pnlToolbar.Controls.Add(lblDisplayLyrics);

            chkDisplayLyrics = new CheckBox()
            {
                Parent = pnlToolbar,
                Location = new Point(UpDMeasures.Left, 29),
                Checked = true,
                Visible = false,
            };
            pnlToolbar.Controls.Add(chkDisplayLyrics);
            chkDisplayLyrics.CheckedChanged += new EventHandler(chkDisplayLyrics_CheckedChanged);


            // ==============================
            // Cell size
            // ==============================
            lblCellsize = new Label()
            {
                Parent = pnlToolbar,
                Location = new Point(20 + UpDMeasures.Left + UpDMeasures.Width, 8),
                Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel),
                Text = Karaboss.Resources.Localization.Strings.CellSize,  //"Cell size",
                AutoSize = true,
                ForeColor = Color.White,
                Visible = false,
            };
            pnlToolbar.Controls.Add(lblCellsize);

            this.UpdCellsize = new NumericUpDown()
            {
                Parent = pnlToolbar,
                Location = new Point(2 + lblCellsize.Left + lblCellsize.Width, 6),
                Minimum = 1,
                Maximum = 500,
                Value = 100,
                Font = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel),
                Size = new Size(43, 22),
                Visible = false,
            };
            UpdCellsize.ValueChanged += new EventHandler(UpdCellsize_ValueChanged);
            pnlToolbar.Controls.Add(UpdCellsize);

            #endregion Tools for editing chords

            #endregion Toolbar



            tabChordsControl.Top = pnlToolbar.Top + pnlToolbar.Height;
            tabChordsControl.Height =  this.ClientSize.Height - menuStrip1.Height - pnlToolbar.Height;
            tabChordsControl.Width = this.ClientSize.Width;
            
            // Mandatory to color headers
            tabChordsControl.DrawMode = TabDrawMode.OwnerDrawFixed;

            #region 1er TAB                    
            // * tabPageDiagrams
            // * -- pnlDisplayHorz
            //      -- ChordControl1
            //      -- positionHScrollBar
            // * -- pnlDisplayImagesOfChords
            //      -- tbpChord
            // * -- pnlBottom
            //      -- lblLyrics
            //      -- lblOtherLyrics

            #region Panel Display horizontal chords
            // 1 : add a panel on top
            // this panel will host the chrod control and the colorslider
            pnlDisplayHorz = new Panel() {
                Parent = tabPageChords,
                Location = new Point(tabPageChords.Margin.Left, tabPageChords.Margin.Top),
                Size = new Size(tabPageChords.Width - tabPageChords.Margin.Left - tabPageChords.Margin.Right, 150),
                BackColor = Color.FromArgb(239, 244, 255), //Color.Chocolate;
            };
            tabPageChords.Controls.Add(pnlDisplayHorz);

            #endregion Panel Display horizontal chords


            #region ChordControl
            // 2 : add a chord control on top
            ChordControl1 = new ChordsControl() {
                Parent = pnlDisplayHorz,
                Location = new Point(0, 0),
                ColumnWidth = 180,
                ColumnHeight = 180,

                Cursor = Cursors.Hand,
                Sequence1 = this.sequence1,
            };

            // Set size mandatory ??? unless, the control is not shown correctly
            ChordControl1.Size = new Size(pnlDisplayHorz.Width, ChordControl1.Height);

            pnlDisplayHorz.Controls.Add(ChordControl1);

            ChordControl1.WidthChanged += new WidthChangedEventHandler(ChordControl_WidthChanged);
            ChordControl1.HeightChanged += new HeightChangedEventHandler(ChordControl_HeightChanged);
            ChordControl1.MouseDown += new MouseEventHandler(ChordControl_MouseDown);


            #endregion


            #region positionHScrollBar
            // 3 : add a colorslider
            positionHScrollBar = new ColorSlider.ColorSlider() {
                Parent = pnlDisplayHorz,
                ThumbImage = Properties.Resources.BTN_Thumb_Blue,
                Size = new Size(pnlDisplayHorz.Width - tabPageChords.Margin.Left - tabPageChords.Margin.Right, 20),
                Location = new Point(0, ChordControl1.Height),
                Value = 0,
                Minimum = 0,

                TickStyle = TickStyle.None,
                SmallChange = 1,
                LargeChange = 1 + NbMeasures * sequence1.Numerator,
                ShowDivisionsText = false,
                ShowSmallScale = false,
                MouseWheelBarPartitions = 1 + NbMeasures * sequence1.Numerator,
            };
            
            pnlDisplayHorz.Controls.Add(positionHScrollBar);
            positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(PositionHScrollBar_Scroll);
            // Set maximum & visibility
            SetScrollBarValues();

            pnlDisplayHorz.Height = ChordControl1.Height + positionHScrollBar.Height + padding;

            #endregion


            #region bitmaps of chords
            // 4 : Add a panel in the middle
            // This panel will display a diagram for chords being played
            // 278; //248 + 30 : 248 pour accord de guitare (200 taille de l'image + 1.24 * 200 accord jouée 25% plus gros et + 30 pour les onglets            
            int htotale = 280;

            pnlDisplayImagesOfChords = new Panel() {
                Parent = tabPageChords,
                Location = new Point(tabPageChords.Margin.Left, pnlDisplayHorz.Top + pnlDisplayHorz.Height),
                Height = htotale,
                BackColor = Color.FromArgb(239, 244, 255),
                Width = pnlDisplayHorz.Width,
            };
            tabPageChords.Controls.Add(pnlDisplayImagesOfChords);


            #region tabPage to select Guitar or Piano
            // =======================================
            tbPChords = new TabControl() {
                Parent = pnlDisplayImagesOfChords,
                Location = new Point(0, 0),
            };
            TabPage TabPageGuitar = new TabPage("Guitar");
            tbPChords.Controls.Add(TabPageGuitar);
            TabPage TabPagePiano = new TabPage("Piano");
            tbPChords.Controls.Add(TabPagePiano);

            tbPChords.Dock = DockStyle.Fill;
            pnlDisplayImagesOfChords.Controls.Add(tbPChords);

            #endregion tabpage


            #region ChordRenderer Guitar
            // =======================================
            ChordRendererGuitar = new ChordRenderer() {
                Parent = TabPageGuitar,
                Location = new Point(TabPageGuitar.Margin.Left, TabPageGuitar.Margin.Top),
                Height = htotale,
                ColumnWidth = 162,
                ColumnHeight = 186,
                DisplayMode = ChordRenderer.DiplayModes.Guitar,
            };
            TabPageGuitar.Controls.Add(ChordRendererGuitar);
            ChordRendererGuitar.HeightChanged += new HeightChangedEventHandler(ChordRendererGuitar_HeightChanged);

            #endregion ChordRenderer Guitar


            #region ChordRenderer Piano
            // =======================================
            // Tabpages dimension are not set if not visible => force redim
            TabPagePiano.Width = TabPageGuitar.Width;

            ChordRendererPiano = new ChordRenderer()
            {
                Parent = TabPagePiano,
                Location = new Point(TabPagePiano.Margin.Left, TabPagePiano.Margin.Top),
                Width = TabPagePiano.ClientSize.Width,
                Height = htotale,
                ColumnWidth = 286,
                ColumnHeight = 137,
                DisplayMode = ChordRenderer.DiplayModes.Piano,
            };
            TabPagePiano.Controls.Add(ChordRendererPiano);
            ChordRendererPiano.HeightChanged += new HeightChangedEventHandler(ChordRendererPiano_HeightChanged);

            #endregion ChordRenderer Guitar


            #endregion bitmaps of chords



            #region Panel Bottom
            // 5 : add a panel at the bottom
            // This panel will host the lyrics
            pnlBottom = new Panel() {
                Parent = this.tabPageChords,
                Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height,
                BackColor = Color.White,
                Dock = DockStyle.Bottom,
            };
            tabPageChords.Controls.Add(pnlBottom);

            // 6 - add a label for text being sung
            Font fontLyrics = new Font("Arial", 32, FontStyle.Regular, GraphicsUnit.Pixel);

            lblLyrics = new Label() {
                Parent = pnlBottom,
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(239, 244, 255),
                AutoSize = false,
                Height = fontLyrics.Height + 20,
                Font = fontLyrics,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Text = "AD Lorem ipsus",
            };


            // 7 : add a label for text to be sung
            lblOtherLyrics = new Label() {
                Parent = pnlBottom,
                Location = new Point(0, lblLyrics.Height),
                Size = new Size(pnlBottom.Width, pnlBottom.Height - lblLyrics.Height),
                BackColor = Color.FromArgb(0, 163, 0),
                AutoSize = false,
                Font = fontLyrics,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Fill,
                Text = "Other lyrics",
            };
            
            // Add controls in reverse order, to insure that DockStyle.Fill will work properly
            pnlBottom.Controls.Add(lblOtherLyrics);
            pnlBottom.Controls.Add(lblLyrics);


            #endregion

            #endregion 1er TAB


            #region 2eme TAB MAP

            #region display map chords
            pnlDisplayMap = new Panel() {
                Parent = tabPageMap,
                Location = new Point(tabPageMap.Margin.Left, tabPageMap.Margin.Top),
                Size = new Size(tabPageMap.Width - tabPageMap.Margin.Left - tabPageMap.Margin.Right, tabPageMap.Height - tabPageMap.Margin.Top - tabPageMap.Margin.Bottom),
                BackColor = Color.White,
                AutoScroll = true,
            };
            tabPageMap.Controls.Add(pnlDisplayMap);

            #endregion display map chords


            #region ChordMapControl
            ChordMapControl1 = new ChordsMapControl(MIDIfileName) {
                Parent = pnlDisplayMap,
                Location = new Point(0, 0),
                ColumnWidth = 100,
                ColumnHeight = 80,
                HeaderHeight = 100,
                Cursor = Cursors.Hand,
                Sequence1 = this.sequence1,
            };

            ChordMapControl1.Size = new Size(ChordMapControl1.Width, ChordMapControl1.Height);
            pnlDisplayMap.Size = new Size(tabPageMap.Width - tabPageMap.Margin.Left - tabPageMap.Margin.Right, tabPageMap.Height - tabPageMap.Margin.Top - tabPageMap.Margin.Bottom);

            ChordMapControl1.WidthChanged += new MapWidthChangedEventHandler(ChordMapControl1_WidthChanged);
            ChordMapControl1.HeightChanged += new MapHeightChangedEventHandler(ChordMapControl1_HeightChanged);
            ChordMapControl1.MouseDown += new MouseEventHandler(ChordMapControl1_MouseDown);

            pnlDisplayMap.Controls.Add(ChordMapControl1);

            #endregion ChordMapControl

            #endregion 2eme TAB 


            #region 3eme TAB LYRICS

            pnlDisplayWords = new Panel() {
                Parent = tabPageLyrics,
                Location = new Point(tabPageLyrics.Margin.Left, tabPageLyrics.Margin.Top),
                Size = new Size(tabPageLyrics.Width - tabPageLyrics.Margin.Left - tabPageLyrics.Margin.Right, tabPageLyrics.Height - tabPageLyrics.Margin.Top - tabPageLyrics.Margin.Bottom),
                BackColor = Color.Coral,
                AutoScroll = true,
            };
            tabPageLyrics.Controls.Add(pnlDisplayWords);

            Font fontWords = new Font("Courier New", 22, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDisplayWords = new System.Windows.Forms.TextBox() {
                Parent = pnlDisplayWords,
                Location = new Point(0, 0),
                Multiline = true,
                TextAlign = HorizontalAlignment.Center,
                ScrollBars = ScrollBars.Both,
                Size = new Size(pnlDisplayWords.Width, pnlDisplayWords.Height),
                Font = fontWords,
                Text = "La petite maison dans la prairie\r\nIl était une fois dans l'ouest",
                Dock = DockStyle.Fill,
            };
            pnlDisplayWords.Controls.Add(txtDisplayWords);

            #endregion 3eme TAB

            // =================================
            #region 4eme TAB MODIFY
            // =================================

            #region Modify map chords
            pnlModifyMap = new Panel()
            {
                Parent = tabPageMap,
                Location = new Point(tabPageModify.Margin.Left, tabPageModify.Margin.Top),
                Size = new Size(tabPageModify.Width - tabPageModify.Margin.Left - tabPageModify.Margin.Right, tabPageModify.Height - tabPageModify.Margin.Top - tabPageModify.Margin.Bottom),
                BackColor = Color.White,
                AutoScroll = true,
            };
            tabPageModify.Controls.Add(pnlModifyMap);

            pnlModifyMap.Scroll += new ScrollEventHandler(pnlModifyMap_Scroll);
            pnlModifyMap.MouseWheel += new MouseEventHandler(pnlModifyMap_MouseWheel);


            #endregion display map chords


            #region ChordMapControl
            ChordMapControlModify = new ChordsMapControl(MIDIfileName)
            {
                Parent = pnlDisplayMap,
                Location = new Point(0, 0),
                ColumnWidth = 100,
                ColumnHeight = 80,
                HeaderHeight = 100,
                Cursor = Cursors.Hand,
                Sequence1 = this.sequence1,
            };

            UpDMeasures.Value = ChordMapControlModify.NbColumns;

            ChordMapControlModify.Size = new Size(ChordMapControlModify.Width, ChordMapControlModify.Height);
            pnlModifyMap.Size = new Size(tabPageModify.Width - tabPageModify.Margin.Left - tabPageModify.Margin.Right, tabPageModify.Height - tabPageModify.Margin.Top - tabPageModify.Margin.Bottom);

            ChordMapControlModify.WidthChanged += new MapWidthChangedEventHandler(ChordMapControlModify_WidthChanged);
            ChordMapControlModify.HeightChanged += new MapHeightChangedEventHandler(ChordMapControlModify_HeightChanged);
            ChordMapControlModify.MouseDown += new MouseEventHandler(ChordMapControlModify_MouseDown);

            pnlModifyMap.Controls.Add(ChordMapControlModify);

            #endregion ChordMapControl

            #endregion 4eme TAB

        }
     

        #endregion Display Controls       


        #region timer

        /// <summary>
        /// Timer tick management
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!scrolling)
            {
                // Display time elapse               
                DisplayTimeElapse(sequencer1.Position);

                switch (PlayerState)
                {
                    case PlayerStates.Playing:
                        // first page
                        int p = sequencer1.Position;
                        DisplayCurrentBeat(p);
                        DisplayLineLyrics(p);                        
                        DisplayPositionHScrollBar(p);
                        DisplayPositionVScrollbar(p);
                        break;

                    case PlayerStates.Stopped:
                        timer1.Stop();
                        AfterStopped();
                        break;

                    case PlayerStates.Paused:
                        sequencer1.Stop();                        
                        timer1.Stop();
                        break;
                }

               
            }
        }


        #endregion timer


        #region Display Chords
        
        private void DisplayChords()
        {
            //ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);            
            // It can be used in DisplayChords if there are chords embedded in lyrics
            //myLyricsMgmt = new LyricsMgmt(sequence1, true);
            
            // This will only extract lyrics and chords if in lyrics or embedded in xml
            myLyricsMgmt.ResetDisplayChordsOptions(true);
            
            
            switch (myLyricsMgmt.ChordsOriginatedFrom)
            {
                case LyricsMgmt.ChordsOrigins.Lyrics:
                    GridBeatChords = myLyricsMgmt.FillGridBeatChordsWithLyricsChords();
                    break;
                case LyricsMgmt.ChordsOrigins.XmlEmbedded:
                    GridBeatChords = myLyricsMgmt.FillGridBeatChordsWithLyricsChords();
                    break;
                case LyricsMgmt.ChordsOrigins.Discovery:
                    // For discovery, we need to call ChordAnalyser
                    ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);
                    GridBeatChords = Analyser.GridBeatChords;
                    break;
                default:
                    break;
            }

            UpdateDisplayOfChords();
           
        }

        private void UpdateDisplayOfChords()
        {
            //Change labels displayed
            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                GridBeatChords[i] = (InterpreteChord(GridBeatChords[i].Item1), GridBeatChords[i].Item2);
            }


            // Display Chords in horizontal cells            
            ChordControl1.GridBeatChords = GridBeatChords;

            // Display chords for guitar & piano                        
            ChordRendererGuitar.GridBeatChords = GridBeatChords;
            ChordRendererPiano.GridBeatChords = GridBeatChords;

            ChordRendererGuitar.FilterChords();
            ChordRendererPiano.FilterChords();

            // Display chords map
            ChordMapControl1.GridBeatChords = GridBeatChords;
            // Modify chords
            ChordMapControlModify.GridBeatChords = GridBeatChords;
        }


        /// <summary>
        /// Remove useless strings
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        private string InterpreteChord(string chord)
        {
            /*
            chord = chord.Replace("sus", "");

            chord = chord.Replace(" major", "");
            chord = chord.Replace(" triad", "");
            chord = chord.Replace("dominant", "");

            chord = chord.Replace("first inversion", "");
            chord = chord.Replace("second inversion", "");
            chord = chord.Replace("third inversion", "");

            chord = chord.Replace(" seventh", "7");
            chord = chord.Replace(" minor", "m");
            chord = chord.Replace("seventh", "7");
            chord = chord.Replace("sixth", "6");
            chord = chord.Replace("ninth", "9");
            chord = chord.Replace("eleventh", "11");

            chord = chord.Replace("6", "");
            chord = chord.Replace("9", "");
            chord = chord.Replace("11", "");
            */

            //chord = chord.Replace("<Chord not found>", "?");
            chord = chord.Replace("<Chord not found>", "");

            //int i = chord.IndexOf("/");
            //if (i > 0) { chord = chord.Substring(0, i); }
            
            //chord = chord.Replace("maj", "");
            //chord = chord.Replace("Eb", "D#");

            chord = chord.Trim();
            return chord;
        }
        

        /// <summary>
        /// Display gray cells
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayCurrentBeat(int pos)
        {
            // pos is in which measure?
            int curmeasure = 1 + pos / _measurelen;

            // Quel temps dans la mesure ?
            int timeinmeasure = sequence1.Numerator - (int)((curmeasure * _measurelen - pos) / (_measurelen / sequence1.Numerator));


            // Labels            
            panelPlayer.DisplayBeat(timeinmeasure.ToString() + "|" + sequence1.Numerator);                            

            // change time in measure => draw cell in control
            if (timeinmeasure != _currentTimeInMeasure)
            {
                _currentTimeInMeasure = timeinmeasure;

                // Draw gray cell for played note
                ChordControl1.DisplayNotes(pos, curmeasure, timeinmeasure);
                ChordRendererGuitar.OffsetControl(sequence1.Numerator , pos, curmeasure, timeinmeasure);
                ChordRendererPiano.OffsetControl(sequence1.Numerator, pos, curmeasure, timeinmeasure);
                
                ChordMapControl1.DisplayNotes(pos, curmeasure, timeinmeasure);
            }
        }

        #endregion Display Notes
       

        #region handle messages

        /// <summary>
        /// Event: loading of midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
        }


        /// <summary>
        /// Event: loading of midi file terminated: launch song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                myLyricsMgmt = new LyricsMgmt(sequence1);
                CommonLoadCompleted(sequence1);                
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

       
        /// <summary>
        /// Load the midi file in the sequencer
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncMidiFile(string fileName)
        {
            try
            {                
                ResetSequencer();
                if (fileName != "\\")
                {
                    sequence1.LoadAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: end loading XML music file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadXmlCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Cursor = Cursors.Arrow;

            if (MXmlReader.seq == null)
            {
                MessageBox.Show("Invalid xml file", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                                 
            if (e.Error == null && e.Cancelled == false)
            {
                myLyricsMgmt = new LyricsMgmt(MXmlReader.seq);
                LoadXmlChordsInLyrics();
                CommonLoadCompleted(MXmlReader.seq);
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Load chords embedded in Xml file
        /// </summary>
        private void LoadXmlChordsInLyrics()
        {
            #region guard
            if (myLyricsMgmt == null) return;
            if (MXmlReader == null) return;
            #endregion guard

            if (MXmlReader.bHasXmlChords)
            {
                // infos
                // MXmlReader.lstChords
                // MXmlReader.TrackChordsNumber
                myLyricsMgmt.ChordsOriginatedFrom = LyricsMgmt.ChordsOrigins.XmlEmbedded;

                myLyricsMgmt.lstXmlChords = MXmlReader.lstChords;
            }
        }

        /// <summary>
        /// Load async a XML file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncXmlFile(string fileName)
        {
            try
            {                
                ResetSequencer();
                if (fileName != "\\")
                {                    
                    MXmlReader = new MusicXmlReader();

                    // Show Xml chords?
                    MXmlReader.PlayXmlChords = Karaclass.m_ShowXmlChords;

                    MXmlReader.LoadXmlCompleted += HandleLoadXmlCompleted;                    
                    MXmlReader.LoadXmlAsync(fileName, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Event: TXT dump sequence loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLoadTxtCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Cursor= Cursors.Arrow;

            if (MTxtReader.seq == null)
            {
                MessageBox.Show("Invalid text file", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (e.Error == null && e.Cancelled == false)
            {
                myLyricsMgmt = new LyricsMgmt(MTxtReader.seq);
                CommonLoadCompleted(MTxtReader.seq);
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        /// <summary>
        /// Load async a TXT file
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadAsyncTxtFile(string fileName)
        {
            try
            {                

                ResetSequencer();
                if (fileName != "\\")
                {
                    MTxtReader = new MusicTxtReader(fileName);
                    MTxtReader.LoadTxtCompleted += HandleLoadTxtCompleted;

                    MTxtReader.LoadTxtAsync(fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }


        /// <summary>
        /// Display song duration
        /// </summary>
        private void DisplaySongDuration()
        {
            // Display
            int Min = (int)(_duration / 60);
            int Sec = (int)(_duration - (Min * 60));
            //lblDuration.Text = string.Format("{0:00}:{1:00}", Min, Sec);
            //lblBeat.Text = "1|" + sequence1.Numerator;

            panelPlayer.DisplayDuration(string.Format("{0:00}:{1:00}", Min, Sec));
            panelPlayer.DisplayBeat("1|" + sequence1.Numerator);
        }


        /// <summary>
        /// Common to MIDI, XML 
        /// </summary>
        /// <param name="seq"></param>& TXT
        private void CommonLoadCompleted(Sequence seq)
        {
            if (seq == null) return;

            try
            {
                LoadSequencer(seq);

                DrawControls();
               
                LoadProperties();

                UpdateMidiTimes();

                DisplaySongDuration();

                //TAB1, TAB2
                DisplayChords();

                // TAB1
                DisplayLyrics();

                //TAB3
                DisplayWordsAndChords();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);             
            }
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach (ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            #region Guard
            if (closing)
            {
                return;
            }
            #endregion

            outDevice.Send(e.Message);

        }

        private void HandlePlayingCompleted(object sender, EventArgs e)
        {
            newstart = 0;
            _currentMeasure = -1;
            PlayerState = PlayerStates.Stopped;
        }

        #endregion handle messages


        #region DisplayLyrics

        /// <summary>
        /// TAB1: Display lyrics
        /// </summary>
        private void DisplayLyrics()
        {            
            // New
            myLyricsMgmt.FullExtractLyrics(true);
            
            myLyricsMgmt.LoadLyricsPerBeat();
            myLyricsMgmt.LoadLyricsLines();

            // Display lyrics on first tab
            ChordControl1.GridLyrics = myLyricsMgmt.Gridlyrics;            
            DisplayLineLyrics(0);

            // Display lyrics on chords map
            ChordMapControl1.GridLyrics = myLyricsMgmt.Gridlyrics;
            ChordMapControlModify.GridLyrics = myLyricsMgmt.Gridlyrics;

        }            
         
        /// <summary>
        /// TAB1: Display current line of lyrics in Label Lyrics 
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayLineLyrics(int pos)
        {
            lblLyrics.Text = myLyricsMgmt.DisplayLineLyrics(pos);

            lblOtherLyrics.Text = myLyricsMgmt.DisplayOtherLinesLyrics(pos);
        }

        /// <summary>
        /// TAB3 : display words + chords
        /// </summary>
        private void DisplayWordsAndChords()
        {
            string cr = Environment.NewLine;
            string tx = ExtractTMidiInfos();
            string title = MIDIfileName;

            title = Path.GetFileNameWithoutExtension(title);            

            myLyricsMgmt.GridBeatChords = GridBeatChords;

            if (tx != "")
            {
                tx += cr + myLyricsMgmt.DisplayWordsAndChords();
            }
            else
            {                
                tx = title + cr + cr + myLyricsMgmt.DisplayWordsAndChords();
            }
            txtDisplayWords.Text = tx;
        }


        #endregion DisplayLyrics


        #region buttons
        private void btnPlay_MouseHover(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_blue_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_blue_pause;

        }

        private void btnRewind_MouseHover(object sender, EventArgs e)
        {
            if(PlayerState == PlayerStates.Playing || PlayerState == PlayerStates.Paused)
                btnRewind.Image = Properties.Resources.btn_blue_prev;
        }

        private void btnPlay_MouseLeave(object sender, EventArgs e)
        {
            if (PlayerState == PlayerStates.Stopped)
                btnPlay.Image = Properties.Resources.btn_black_play;
            else if (PlayerState == PlayerStates.Paused)
                btnPlay.Image = Properties.Resources.btn_green_play;
            else if (PlayerState == PlayerStates.Playing)
                btnPlay.Image = Properties.Resources.btn_green_pause;

        }

        private void btnRewind_MouseLeave(object sender, EventArgs e)
        {
            btnRewind.Image = Properties.Resources.btn_black_prev;
        }

        private void btnZoomMinus_Click(object sender, EventArgs e)
        {
            float zoom;
            switch (this.tabChordsControl.SelectedIndex) 
            {
                case 0:
                    zoom = ChordControl1.Zoom;
                    zoom -= (float)0.1;

                    ChordControl1.Zoom = zoom;
                    ChordRendererGuitar.zoom = zoom;
                    ChordRendererPiano.zoom = zoom;
                    SetScrollBarValues();
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;

                case 1:
                    zoom = ChordMapControl1.Zoom;
                    zoom -= (float)0.1;
                    
                    ChordMapControl1.Zoom = zoom;                    
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;

                case 3:
                    zoom = ChordMapControlModify.Zoom;
                    zoom -= (float)0.1;

                    ChordMapControlModify.Zoom = zoom;                    
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;
            }
        }

        private void btnZoomPlus_Click(object sender, EventArgs e)
        {
            float zoom;

            switch (this.tabChordsControl.SelectedIndex) 
            {
                case 0:
                    zoom = ChordControl1.Zoom;
                    zoom += (float)0.1;

                    ChordControl1.Zoom = zoom;
                    ChordRendererGuitar.zoom = zoom;
                    ChordRendererPiano.zoom = zoom;                    
                    SetScrollBarValues();
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;

                case 1:
                    zoom = ChordMapControl1.Zoom;
                    zoom += (float)0.1;

                    ChordMapControl1.Zoom = zoom;
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;
                
                case 3:
                    zoom = ChordMapControlModify.Zoom;
                    zoom += (float)0.1;

                    ChordMapControlModify.Zoom = zoom;
                    toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                    toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
                    break;
            }
        }

        private void btnPrintPDF_Click(object sender, EventArgs e)
        {
            PrintPDF(); 
        }

        private void btnPrintTXT_Click(object sender, EventArgs e)
        {
            PrintText();
        }

        #endregion buttons


        #region Events

        #region TAB1 chordcontrol
        private void ChordControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.Location.X;

                newstart = (int)(((ChordControl1.OffsetX + x) / (float)ChordControl1.Width) * sequence1.GetLength());
                FirstPlaySong(newstart);


            }
        }
        private void ChordControl_HeightChanged(object sender, int value)
        {
            if (positionHScrollBar != null)
            {
                positionHScrollBar.Location = new Point(0, ChordControl1.Height);
                pnlDisplayHorz.Height = ChordControl1.Height + positionHScrollBar.Height + padding;
                
                pnlDisplayImagesOfChords.Top = pnlDisplayHorz.Top + pnlDisplayHorz.Height;
            }            

            if (pnlBottom != null && pnlDisplayImagesOfChords != null && pnlDisplayHorz != null)
            {
                pnlBottom.Location = new Point(0, pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height);
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }

        }

        #endregion chordcontrol


        #region TAB1 chordrenderer events
        
        private void ChordRendererGuitar_HeightChanged(object sender, int value)
        {                        

            if (pnlBottom != null && pnlDisplayImagesOfChords != null && pnlDisplayHorz != null)
            {
                pnlBottom.Location = new Point(0, pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height);
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }
        }
                       
        
        private void ChordRendererPiano_HeightChanged(object sender, int value)
        {

            if (pnlBottom != null && pnlDisplayImagesOfChords != null && pnlDisplayHorz != null)
            {
                pnlBottom.Location = new Point(0, pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height);
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }
        }
      
        #endregion TAB1 chordrenderer events


        #region TAB2 chordmapcontrol
        private void ChordMapControl1_HeightChanged(object sender, int value)
        {            
            

        }

        private void ChordMapControl1_WidthChanged(object sender, int value)
        {            

        }

        /// <summary>
        /// Launch player on mouse down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChordMapControl1_MouseDown(object sender, MouseEventArgs e)
        {            
            if (e.Button == MouseButtons.Left)
            {
                int x = e.Location.X - ChordMapControl1.LeftMargin;  //Horizontal
                int y = e.Location.Y + ChordMapControl1.OffsetY - ChordMapControl1.HeaderHeight;  // Vertical

                // Calculate start time                
                int HauteurCellule = (int)(ChordMapControl1.ColumnHeight) + 1;
                int LargeurCellule = (int)(ChordMapControl1.ColumnWidth) + 1;
                                
                int line = (int)Math.Ceiling(y / (double)HauteurCellule);                
                
                int prevmeasures = (line - 1) * ChordMapControl1.NbColumns;    // measures in previous lines

                int cellincurrentline = -1 +  (int)Math.Ceiling(x / (double)LargeurCellule);  // Cell number in current line               

                newstart = _measurelen * prevmeasures + (_measurelen / sequence1.Numerator) * cellincurrentline;                

                FirstPlaySong(newstart);
            }
        }


        #endregion chordmapcontrol


        #region TAB4 ChordMapControlModify

        /// <summary>
        /// Change cells size
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdCellsize_ValueChanged(object sender, EventArgs e)
        {
            int CellSize = (int)UpdCellsize.Value;
            // Shared values
            ChordMapControl1.ColumnWidth = CellSize;
            ChordMapControlModify.ColumnWidth = CellSize;

            // Save option
            Properties.Settings.Default.ChordsMapCellSize = CellSize;
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Change option display lyrics
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDisplayLyrics_CheckedChanged(object sender, EventArgs e)
        {
            switch (tabChordsControl.SelectedIndex)
            {
                case 1:
                    ChordMapControl1.Displaylyrics = chkDisplayLyrics.Checked;
                    break;
                case 3:
                    ChordMapControlModify.Displaylyrics = chkDisplayLyrics.Checked;
                    break;
            }                        
        }

        /// <summary>
        /// Change number of measures per line
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdMeasures_ValueChanged(object sender, EventArgs e)
        {
            int Columns = (int)UpDMeasures.Value;

            // Shared values
            ChordMapControl1.NbColumns = Columns;
            ChordMapControlModify.NbColumns = Columns;

            // Save option
            Properties.Settings.Default.ChordsMapColumns = Columns;
            Properties.Settings.Default.Save();
        }


        /// <summary>
        /// Open windows to modify a chord name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ChordMapControlModify_MouseDown(object sender, MouseEventArgs e)
        {
            int beat;
            string ChordName;
            
            if (e.Button == MouseButtons.Left)
            {
                
                if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
                {
                    frmEditChord frmEditChord = GetForm<frmEditChord>();
                    frmEditChord.Close();
                    return;
                }               

                int x = e.Location.X - ChordMapControlModify.LeftMargin;  //Horizontal
                int y = e.Location.Y + ChordMapControlModify.OffsetY - ChordMapControlModify.HeaderHeight;  // Vertical

                // Calculate start time                
                int HauteurCellule = (int)(ChordMapControlModify.ColumnHeight) + 1;
                int LargeurCellule = (int)(ChordMapControlModify.ColumnWidth) + 1;


                int line = (int)Math.Ceiling(y / (double)HauteurCellule);
                int prevmeasures = -1 + (line - 1) * ChordMapControlModify.NbColumns;
                int cellincurrentline = (int)Math.Ceiling(x / (double)LargeurCellule);


                // Calculate x & y with cells bounds
                frmxPos = Left;
                frmyPos = Top;

                xPos = 12 + Left + tabPageModify.Left + ChordMapControlModify.LeftMargin + (cellincurrentline - 1) * LargeurCellule;            // why 12 ?????????????
                yPos = 35 + Top + tabChordsControl.Top + tabPageModify.Top + ChordMapControlModify.HeaderHeight + (line * HauteurCellule);    //why 12 ??????????????                

                beat = (line - 1) * (ChordMapControlModify.NbColumns * sequence1.Numerator) + cellincurrentline;
                
                if (ChordMapControlModify.GridBeatChords.ContainsKey(beat))
                {                    
                    ChordName = ChordMapControlModify.GridBeatChords[beat].Item1;                    
                    frmEditChord = new frmEditChord(ChordName, beat, xPos + pnlModifyMap.AutoScrollPosition.X, yPos + pnlModifyMap.AutoScrollPosition.Y);
                    frmEditChord.Show();
                }
            }
        }

        private void pnlModifyMap_Scroll(object sender, ScrollEventArgs e)
        {                       
            if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
            {                               
                frmEditChord.Location = new Point(xPos + pnlModifyMap.AutoScrollPosition.X + (Left - frmxPos), yPos + pnlModifyMap.AutoScrollPosition.Y + (Top - frmyPos));                
                frmEditChord.Visible = frmEditChord.Top > 222 && frmEditChord.Top < 800;
            }
                
        }

        private void pnlModifyMap_MouseWheel(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
            {
                frmEditChord.Location = new Point(xPos + pnlModifyMap.AutoScrollPosition.X + (Left - frmxPos), yPos + pnlModifyMap.AutoScrollPosition.Y + (Top - frmyPos));               

                frmEditChord.Visible = frmEditChord.Top > 222;
                
            }
        }

        private void ChordMapControlModify_HeightChanged(object sender, int value)
        {
            //throw new NotImplementedException();
        }

        private void ChordMapControlModify_WidthChanged(object sender, int value)
        {
            //throw new NotImplementedException();
        }

        #endregion TAB4 ChordMapControlModify


        #region All TABS TabChordsControl

        /// <summary>
        /// Color selected header cell of TabControl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabChordsControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This event is called once for each tab button in your tab control
            // First paint the background with a color based on the current tab
            // e.Index is the index of the tab in the TabPages collection.

            Color a = Color.FromArgb(255, 196, 13); // Yellow
            Color b = Color.FromArgb(153, 180, 51); // Light Green
            Color c = Color.FromArgb(239, 244, 255); // Light blue
            Color d = Color.FromArgb(45, 137, 239); // Blue


            TabPage page = tabChordsControl.TabPages[e.Index];
            Rectangle paddedBounds = e.Bounds;

            if (e.State == DrawItemState.Selected)
            {
                e.Graphics.FillRectangle(new SolidBrush(a), e.Bounds);
                TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, Color.White);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(c), e.Bounds);
                TextRenderer.DrawText(e.Graphics, page.Text, e.Font, paddedBounds, Color.Black);
            }
        }


        /// <summary>
        /// A new tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabChordsControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            float zoom;            

            try
            {
                switch (tabChordsControl.SelectedIndex)
                {
                    case 0: // Chords line
                        btnPrintTXT.Visible = false;
                        btnPrintPDF.Visible = false;
                        mnuFilePrintLyrics.Visible = false;
                        mnuFilePrintPDF.Visible = false;
                        lblMeasures.Visible = false;
                        UpDMeasures.Visible = false;
                        lblDisplayLyrics.Visible = false;
                        chkDisplayLyrics.Visible = false;
                        lblCellsize.Visible = false;
                        UpdCellsize.Visible = false;

                        btnZoomPlus.Visible = true;
                        btnZoomMinus.Visible = true;

                        zoom = ChordControl1.Zoom;
                        toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                        toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));

                        break;
                    
                    case 1: // Map                                                
                        mnuFilePrintLyrics.Visible = false;
                        btnPrintTXT.Visible = false;

                        mnuFilePrintPDF.Visible= true;
                        btnPrintPDF.Visible = true;
                        btnZoomPlus.Visible = true;
                        btnZoomMinus.Visible = true;
                        
                        zoom = ChordMapControl1.Zoom;
                        toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                        toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));

                        lblMeasures.Visible = true;
                        lblDisplayLyrics.Visible = true;
                        lblCellsize.Visible = true;
                        
                        ChordMapControl1.NbColumns = (int)UpDMeasures.Value;
                        UpDMeasures.Visible = true;

                        ChordMapControl1.ColumnWidth = (int)UpdCellsize.Value;
                        UpdCellsize.Visible = true;

                        ChordMapControl1.Displaylyrics = chkDisplayLyrics.Checked;
                        chkDisplayLyrics.Visible = true;
                        break;
                    
                    case 2: //Words
                        lblMeasures.Visible = false;
                        UpDMeasures.Visible = false;
                        lblDisplayLyrics.Visible = false;
                        chkDisplayLyrics.Visible = false;
                        lblCellsize.Visible = false;
                        UpdCellsize.Visible = false;


                        mnuFilePrintLyrics.Visible = true;
                        mnuFilePrintPDF.Visible = true;
                        btnPrintTXT.Visible = true;
                        btnPrintPDF.Visible = true;                        
                        break;
                    
                    case 3: // Modify map                        
                        mnuFilePrintLyrics.Visible = false;
                        btnPrintTXT.Visible = false;

                        mnuFilePrintPDF.Visible = true;
                        btnPrintPDF.Visible = true;
                        btnZoomPlus.Visible = true;
                        btnZoomMinus.Visible = true;

                        lblMeasures.Visible = true;
                        lblDisplayLyrics.Visible = true;
                        lblCellsize.Visible = true;                        

                        ChordMapControlModify.NbColumns = (int)UpDMeasures.Value;
                        UpDMeasures.Visible = true;

                        ChordMapControlModify.ColumnWidth = (int)UpdCellsize.Value;
                        UpdCellsize.Visible = true;

                        ChordMapControlModify.Displaylyrics = chkDisplayLyrics.Checked;
                        chkDisplayLyrics.Visible = true;

                        zoom = ChordMapControlModify.Zoom;
                        toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
                        toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));

                        break;
                }



                if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
                {
                    frmEditChord.Visible = (tabChordsControl.SelectedIndex == 3);

                }

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        #endregion All TABS TabChordsControl


        #endregion Events


        #region PositionVScrollbar

        /// <summary>
        /// Display vertical scrollbar in 2nd page
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayPositionVScrollbar(int pos)
        {
            // pos is in which measure?
            int curmeasure = 1 + pos / _measurelen;

            // which line ?                                            
            int curline = (int)(Math.Ceiling((double)(curmeasure) / ChordMapControl1.NbColumns));

            // Change line => offset Chord map
            if (curline != _currentLine)
            {
                _currentLine = curline;
                int HauteurCellule = (int)(ChordMapControl1.ColumnHeight) + 1;

                
                // if control is higher then the panel => scroll
                if (ChordMapControl1.Height > pnlDisplayMap.Height)
                {
                    // offset vertical: ensure to see 2 lines                    
                    int offset = ChordMapControl1.HeaderHeight +  HauteurCellule * (curline - 1);

                    if (pnlDisplayMap.VerticalScroll.Visible && pnlDisplayMap.VerticalScroll.Minimum <= offset && offset <= pnlDisplayMap.VerticalScroll.Maximum)
                    {
                        pnlDisplayMap.VerticalScroll.Value = offset;
                    }
                }                
            }
        }

        #endregion PositionVScrollbar


        #region positionHSCrollBar

        /// <summary>
        /// Display horizontal scrollbar in first tab
        /// </summary>
        /// <param name="pos"></param>
        private void DisplayPositionHScrollBar(int pos)
        {
            // pos is in which measure?
            int curmeasure = 1 + pos / _measurelen;                        
            
            // Change measure => offset control
            if (curmeasure != _currentMeasure)
            {
                
                _currentMeasure = curmeasure;                
                
                
                // Calculations for ChordControl1
                int LargeurCellule = (int)(ChordControl1.ColumnWidth * ChordControl1.Zoom) + 1;
                int LargeurMesure = LargeurCellule * sequence1.Numerator; // keep one measure on the left
                int offsetx = LargeurCellule + (_currentMeasure - 1) * (LargeurMesure);                    
                
                int course = (int)(positionHScrollBar.Maximum - positionHScrollBar.Minimum);
                int CellsNumber = 1 + NbMeasures * sequence1.Numerator;

                // La première case ne sert qu'à l'affichage
                // La position de la scrollbar doit tenir compte de la première case
                // % de Largeur Cellule par rapport à la course de la scrollbar ?
                // On dessine toutes ces cases : 1 + NbMeasures * Sequence1.Numerator
                // Course de la scrollbar = Largeur1ereCellule +  NbMeasures * sequence1.Numerator * LargeurCellule
                // soit : Course = LargeurCellule * (NbMeasures * sequence1.Numerator + 1)
                // val = Largeur1ereCellule + (int)((_currentMeasure/(float)NbMeasures) * course);
                // Largeur1ereCellule = Course/CellsNumber
                int val = (course / CellsNumber) + (int)((_currentMeasure / (float)NbMeasures) * course);

                if (positionHScrollBar.Minimum <= val && val <= positionHScrollBar.Maximum)
                    positionHScrollBar.Value = val;


                // Aftert firts measure, the curseur is on le left
                if (ChordControl1.maxStaffWidth > pnlDisplayHorz.Width)
                {                    
                    // offset horizontal
                    if (offsetx > LargeurMesure)
                    {
                        if (offsetx < ChordControl1.maxStaffWidth - pnlDisplayHorz.Width)
                        {                            
                            ChordControl1.OffsetX = offsetx;
                        }
                        else
                        {
                            ChordControl1.OffsetX = ChordControl1.maxStaffWidth - pnlDisplayHorz.Width;
                        }
                    }                    
                }                

            }
        }

        private void SetScrollBarValues()
        {
            if (pnlDisplayHorz == null || ChordControl1 == null || ChordRendererGuitar == null || ChordRendererPiano == null)
                return;

            // Width of control
            int W = ChordControl1.maxStaffWidth;

            if (W <= pnlDisplayHorz.Width)
            {
                positionHScrollBar.Visible = false;
                positionHScrollBar.Maximum = 0;
                
                pnlDisplayHorz.Height = ChordControl1.Height + padding;
                pnlDisplayImagesOfChords.Top = pnlDisplayHorz.Top + pnlDisplayHorz.Height;
                pnlBottom.Top = pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height;
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;

                ChordControl1.OffsetX = 0;
                ChordRendererGuitar.OffsetX = 0;
                ChordRendererPiano.OffsetX = 0;

                positionHScrollBar.Value = 0;
            }
            else if (W > pnlDisplayHorz.Width)
            {
                positionHScrollBar.Maximum = W - pnlDisplayHorz.Width;
                positionHScrollBar.Visible = true;

                pnlDisplayHorz.Height = ChordControl1.Height + positionHScrollBar.Height + padding;
                pnlDisplayImagesOfChords.Top = pnlDisplayHorz.Top + pnlDisplayHorz.Height;
                pnlBottom.Top = pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height;
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }

        }

        /// <summary>
        /// Scroll horizontal scrollbar: move sequencer position to scrollbar value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PositionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {            
            ChordControl1.OffsetX = e.NewValue;

            if (e.Type == ScrollEventType.EndScroll)
            {
                // scrollbar position = fraction  of sequence Length
                float n =  (e.NewValue / (float)(positionHScrollBar.Maximum - positionHScrollBar.Minimum)) * sequence1.GetLength();
                newstart = (int)n;               

                sequencer1.Position = newstart;
                scrolling = false;
            }
            else if (e.Type != ScrollEventType.First)
            {
                // Explain: remove ScrollEventType.First when using the keyboard to pause, start, rewind
                // Without this, scrolling is set to true
                scrolling = true;
            }
        }

      

        /// <summary>
        /// Set positionHScrollbar Width equal to chord control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        private void ChordControl_WidthChanged(object sender, int value)
        {
            if (positionHScrollBar != null)
            {
                positionHScrollBar.Width = (pnlDisplayHorz.Width > ChordControl1.Width ? ChordControl1.Width : pnlDisplayHorz.Width);

                // Set maximum & visibility
                SetScrollBarValues();
            }
        }

        #endregion positionHScrollBar


        #region Form load close

        protected override void OnClosing(CancelEventArgs e)
        {
            closing = true;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {            
            ResetSequencer();
            sequencer1.Dispose();
            if (outDevice != null && !outDevice.IsDisposed)
                outDevice.Reset();
            base.OnClosed(e);
        }

        /// <summary>
        /// Override form load event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {            
            if (outDevice == null)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }
            else
            {
                try
                {
                    sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;

                    // ACTIONS TO PERFORM
                    SelectActionOnLoad();                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Close();
                }
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// Select what to do on load: new score, play single file, or playlist 
        /// </summary>
        private void SelectActionOnLoad()
        {
            string ext = Path.GetExtension(MIDIfileFullPath).ToLower();
            if (ext == ".mid" || ext == ".kar")
            {
                // Play a single MIDI file
                LoadAsyncMidiFile(MIDIfileFullPath);
            }
            else if (ext == ".xml" || ext == ".musicxml")
            {
                Cursor.Current = Cursors.WaitCursor;
                System.Windows.Forms.Application.DoEvents();
                LoadAsyncXmlFile(MIDIfileFullPath);
            }
            else if (ext == ".mxl")
            {
                // mxl file must be unzipped before
                string myXMLFileName = Files.UnzipFile(MIDIfileFullPath);
                if (File.Exists(myXMLFileName))
                {
                    Cursor.Current = Cursors.WaitCursor;
                    System.Windows.Forms.Application.DoEvents();
                    LoadAsyncXmlFile(myXMLFileName);
                }
            }
            else if (ext == ".txt")
            {
                Cursor.Current = Cursors.WaitCursor;
                System.Windows.Forms.Application.DoEvents();
                LoadAsyncTxtFile(MIDIfileFullPath);
            }
            else
            {
                MessageBox.Show("Unknown extension");
            }
        }


        private void frmChords_Load(object sender, EventArgs e)
        {
            #region setwindowlocation
            // Récupère la taille et position de la forme
            // Set window location
            if (Properties.Settings.Default.frmChordsMaximized)
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                Location = Properties.Settings.Default.frmChordsLocation;
                // Verify if this windows is visible in extended screens
                Rectangle rect = new Rectangle(int.MaxValue, int.MaxValue, int.MinValue, int.MinValue);
                foreach (Screen screen in Screen.AllScreens)
                    rect = Rectangle.Union(rect, screen.Bounds);

                if (Location.X > rect.Width)
                    Location = new Point(0, Location.Y);
                if (Location.Y > rect.Height)
                    Location = new Point(Location.X, 0);

                Size = Properties.Settings.Default.frmChordsSize;
                
            }
            #endregion

            this.Activate();
        }

        private void frmChords_Resize(object sender, EventArgs e)
        {
            // Controle onglets
            tabChordsControl.Top = menuStrip1.Height + pnlToolbar.Height;
            tabChordsControl.Width = this.ClientSize.Width;
            tabChordsControl.Height = this.ClientSize.Height - menuStrip1.Height - pnlToolbar.Height;

            // Bug: only the selected TabPage is resized, but not others 
            for (int i = 0; i < tabChordsControl.TabCount; i++)
            {
                if (i != tabChordsControl.SelectedIndex)
                {
                    // Force other tabs to redim
                    tabChordsControl.TabPages[i].Width = tabChordsControl.TabPages[tabChordsControl.SelectedIndex].Width;
                    tabChordsControl.TabPages[i].Height = tabChordsControl.TabPages[tabChordsControl.SelectedIndex].Height;
                }
            }
            
            if (pnlToolbar != null)
            {
                pnlToolbar.Width = this.ClientSize.Width;
            }

            // 1st TAB
            if (pnlDisplayHorz != null && tbPChords != null)
            {
                pnlDisplayHorz.Width = tabPageChords.Width - tabPageChords.Margin.Left - tabPageChords.Margin.Right;
                pnlDisplayImagesOfChords.Width = pnlDisplayHorz.Width;

                for (int i = 0; i < tbPChords.TabCount; i++)
                {
                    if (i != tbPChords.SelectedIndex)
                    {
                        // Force other tabs to redim
                        tbPChords.TabPages[i].Width = tbPChords.TabPages[tbPChords.SelectedIndex].Width;
                    }
                }
                
                ChordRendererPiano.Width = tbPChords.TabPages[1].Width;
                pnlBottom.Height = tabPageChords.Height - tabPageChords.Margin.Top - tabPageChords.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }

            if (ChordControl1 != null)
            {
                positionHScrollBar.Width = (pnlDisplayHorz.Width > ChordControl1.Width ? ChordControl1.Width : pnlDisplayHorz.Width);
                positionHScrollBar.Top = ChordControl1.Top + ChordControl1.Height;
            }

            // ==================================
            // 2nd TAB
            // ==================================
            if (pnlDisplayMap != null)
            {
                pnlDisplayMap.Width = tabPageMap.Width - tabPageMap.Margin.Left - tabPageMap.Margin.Right;                
                pnlDisplayMap.Height = tabPageMap.Height - tabPageMap.Margin.Top - tabPageMap.Margin.Bottom;
            }

            // ==================================
            // 3rd TAB
            // ==================================
            if (pnlDisplayWords != null)
            {
                pnlDisplayWords.Width = tabPageLyrics.Width - tabPageLyrics.Margin.Left - tabPageLyrics.Margin.Right;
                pnlDisplayWords.Height = tabPageLyrics.Height - tabPageLyrics.Margin.Top - tabPageLyrics.Margin.Bottom;
            }

            // ==================================
            // 4th TAB
            // ==================================
            if (pnlModifyMap != null)
            {
                pnlModifyMap.Width = tabPageModify.Width - tabPageModify.Margin.Left - tabPageModify.Margin.Right;
                pnlModifyMap.Height = tabPageModify.Height - tabPageModify.Margin.Top - tabPageModify.Margin.Bottom;
            }

            // Set maximum & visibility
            SetScrollBarValues();
        }

        private void frmChords_Move(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
            {
                frmEditChord.Location = new Point(xPos + pnlModifyMap.AutoScrollPosition.X + (Left - frmxPos), yPos + pnlModifyMap.AutoScrollPosition.Y + (Top - frmyPos) );
                
            }
        }

        private void frmChords_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (bfilemodified == true)
            {
                // string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                String tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;
                DialogResult dr = MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    e.Cancel = true;
                    // turlututu
                    bClosingRequired = true;

                    StoreChordsInLyrics();


                    SaveFileProc();
                    return;
                }
            }

            // enregistre la taille et la position de la forme
            // Copy window location to app settings                
            if (WindowState != FormWindowState.Minimized)
            {
                if (WindowState == FormWindowState.Maximized)
                {
                    Properties.Settings.Default.frmChordsLocation = RestoreBounds.Location;
                    Properties.Settings.Default.frmChordsMaximized = true;

                }
                else if (WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.frmChordsLocation = Location;
                    Properties.Settings.Default.frmChordsSize = Size;
                    Properties.Settings.Default.frmChordsMaximized = false;
                }

                // Save settings
                Properties.Settings.Default.Save();
            }

            Properties.Settings.Default.ChordsMapZoom = ChordMapControl1.Zoom;
            Properties.Settings.Default.ChordsMapModifyZoom = ChordMapControlModify.Zoom;
            Properties.Settings.Default.Save();

            if (System.Windows.Forms.Application.OpenForms.OfType<frmEditChord>().Count() > 0)
            {
                System.Windows.Forms.Application.OpenForms["frmEditChord"].Close();
            }

            // Active le formulaire frmExplorer
            if (System.Windows.Forms.Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                System.Windows.Forms.Application.OpenForms["frmExplorer"].Restore();
                System.Windows.Forms.Application.OpenForms["frmExplorer"].Activate();
            }

            Dispose();
        }

        /// <summary>
        /// Key Up event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmChords_KeyUp(object sender, KeyEventArgs e)
        {            
            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (tabChordsControl.SelectedIndex != 2)
                        PlayPauseMusic();
                    break;
            }            
        }


        /// <summary>
        /// I am able to detect alpha-numeric keys. However i am not able to detect arrow keys
        /// ProcessCmdKey save my life
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((PlayerState == PlayerStates.Paused) || (PlayerState == PlayerStates.Stopped && newstart > 0))
            {
                if (keyData == Keys.Left)
                {
                    Rewind();
                    return true;
                }               
            }
                        
            return base.ProcessCmdKey(ref msg, keyData);

        }
        #endregion Form


        #region Midi

            /// <summary>
            /// Upadate MIDI times
            /// </summary>
            private void UpdateMidiTimes()
        {
            _totalTicks = sequence1.GetLength();
            _tempo = sequence1.Tempo;
            _ppqn = sequence1.Division;


            // Load tempos map
            TempoUtilities.lstTempos = TempoUtilities.GetAllTempoChanges(sequence1);

            _durationPercent = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds                        
            _duration = TempoUtilities.GetMidiDuration(_totalTicks, _ppqn);

            if (sequence1.Time != null)
            {
                _measurelen = sequence1.Time.Measure;
                NbMeasures = Convert.ToInt32(Math.Ceiling((double)_totalTicks / _measurelen)); // rounds up to the next full integer
            }
        }


        #endregion Midi


        #region Play stop pause

        private void ResetSequencer()
        {
            //if (timer1 != null)
            //    timer1.Stop();
            timer1?.Stop();

            scrolling = false;
            newstart = 0;
            //laststart = 0;
            _currentMeasure = -1;
            
            //if (sequencer1 != null) 
            //    sequencer1.Stop();
            sequencer1?.Stop();
            PlayerState = PlayerStates.Stopped;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            PlayPauseMusic();
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            Rewind();
        }

        private void Rewind()
        {
            scrolling = false;
            newstart = 0;
            StopMusic();
        }

        /// <summary>
        /// Button play clicked: manage actions according to player status 
        /// </summary>
        private void PlayPauseMusic()
        {
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    // If playing => pause
                    PlayerState = PlayerStates.Paused;
                    BtnStatus();
                    break;

                case PlayerStates.Paused:
                    // if paused => play                
                    nbstop = 0;
                    scrolling = false;
                    PlayerState = PlayerStates.Playing;
                    BtnStatus();
                    timer1.Start();
                    sequencer1.Continue();
                    break;

                default:
                    // First play                
                    FirstPlaySong(newstart);
                    break;
            }
        }

        /// <summary>
        /// PlaySong for first time
        /// </summary>
        public void FirstPlaySong(int ticks)
        {
            try
            {
                PlayerState = PlayerStates.Playing;
                nbstop = 0;
                scrolling = false;
                _currentMeasure = -1;
                BtnStatus();
                sequencer1.Start();

                if (ticks > 0)
                    sequencer1.Position = ticks;

                timer1.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Display according to play, pause, stop status
        /// </summary>
        private void BtnStatus()
        {
            // Play and pause are same button
            switch (PlayerState)
            {
                case PlayerStates.Playing:
                    btnPlay.Image = Properties.Resources.btn_green_pause;
                    btnPlay.Enabled = true;  // to allow pause                    
                    tabChordsControl.TabPages.Remove(tabPageModify);
                    panelPlayer.DisplayStatus("Playing");
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnPlay.Enabled = true;  // to allow play
                    panelPlayer.DisplayStatus("Paused");
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play

                    if (!tabChordsControl.TabPages.Contains(tabPageModify))
                    {
                        tabChordsControl.TabPages.Add(tabPageModify);
                        // Redim it 
                        pnlModifyMap.Width = tabPageModify.Width - tabPageModify.Margin.Left - tabPageModify.Margin.Right;
                        pnlModifyMap.Height = tabPageModify.Height - tabPageModify.Margin.Top - tabPageModify.Margin.Bottom;
                    }
                    
                    panelPlayer.DisplayStatus("Stopped");
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Stop Music player
        /// </summary>
        private void StopMusic()
        {
            PlayerState = PlayerStates.Stopped;
            try
            {
                sequencer1.Stop();

                // Si point de départ n'est pas le début du morceau
                if (newstart > 0)
                {
                    if (nbstop > 0)
                    {
                        newstart = 0;
                        nbstop = 0;
                        AfterStopped();
                    }
                    else
                    {
                        decimal pos = newstart + positionHScrollBar.Minimum;
                        if (positionHScrollBar.Minimum <= pos && pos <= positionHScrollBar.Maximum)
                            positionHScrollBar.Value = pos;
                        nbstop = 1;
                    }
                }
                else
                {
                    // Point de départ = début du morceau
                    AfterStopped();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
        
        /// <summary>
        /// Things to do at the end of a song
        /// </summary>
        private void AfterStopped()
        {
            // Buttons play & stop 
            BtnStatus();
            
            // Stopped to begining of score
            if (newstart <= 0)
            {
                DisplayTimeElapse(0);
                DisplayPositionHScrollBar(0);
                //lblBeat.Text = "1|" + sequence1.Numerator;
                panelPlayer.DisplayBeat("1|" + sequence1.Numerator);

                _currentMeasure = -1;
                _currentTimeInMeasure = -1;
                positionHScrollBar.Value = positionHScrollBar.Minimum;
                
                ChordControl1.OffsetX = 0;
                ChordControl1.DisplayNotes(0, -1, -1);

                ChordRendererGuitar.AfterStopped();
                ChordRendererPiano.AfterStopped();
                
                pnlDisplayMap.VerticalScroll.Value = pnlDisplayMap.VerticalScroll.Minimum;
                pnlDisplayMap.VerticalScroll.Visible = false;
                pnlDisplayMap.VerticalScroll.Value = pnlDisplayMap.VerticalScroll.Minimum;
                pnlDisplayMap.VerticalScroll.Visible = true;

                ChordMapControl1.OffsetY = 0;
                ChordMapControl1.DisplayNotes(0, -1, -1);
                ChordMapControl1.Playing = false;

                ChordMapControlModify.Playing = false;

                DisplayLineLyrics(0);                

                //laststart = 0;
                scrolling = false;
            }
            else
            {
                // Stop to start point newstart (ticks)                            
            }
        }

        /// <summary>
        /// Display Time elapse
        /// </summary>
        private void DisplayTimeElapse(int pos)
        {
            double dpercent = 100 * pos / (double)_totalTicks;
            //lblPercent.Text = string.Format("{0}%", (int)dpercent);
            panelPlayer.DisplayPercent(string.Format("{0}%", (int)dpercent));

            double maintenant = (dpercent * _duration) / 100;  //seconds
            int Min = (int)(maintenant / 60);
            int Sec = (int)(maintenant - (Min * 60));
            //lblElapsed.Text = string.Format("{0:00}:{1:00}", Min, Sec);
            panelPlayer.displayElapsed(string.Format("{0:00}:{1:00}", Min, Sec));
        }


        #endregion Play stop pause


        #region menus

        #region mnu file

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            openMidiFileDialog.Title = "Open MIDI file";
            openMidiFileDialog.DefaultExt = "kar";
            openMidiFileDialog.Filter = "Kar files|*.kar|MIDI files|*.mid|Xml files|*.xml|MusicXml files|*.musicxml|Compressed MusicXml files|*.mxl|Text files|*.txt|All files|*.*";


            if (openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;

                MIDIfileName = Path.GetFileName(fileName);
                MIDIfilePath = Path.GetDirectoryName(fileName);
                MIDIfileFullPath = fileName;

                // Load file
                sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                sequence1.LoadCompleted += HandleLoadCompleted;

                SelectActionOnLoad();

            }
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            StoreChordsInLyrics();
            SaveFileProc();
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            StoreChordsInLyrics();
            SaveAsFileProc();
        }

        /// <summary>
        /// TAB 3: send lyrics to notepad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFilePrintLyrics_Click(object sender, EventArgs e)
        {
            PrintText();
        }

        /// <summary>
        /// TAB 2 : Print Chord Map to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuFilePrintPDF_Click(object sender, EventArgs e)
        {
            PrintPDF();
        }

        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion mnu File

        #region mnu Help
        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            frmAboutDialog dlg = new frmAboutDialog();
            dlg.ShowDialog();
        }

        private void mnuHelpAboutSong_Click(object sender, EventArgs e)
        {
            string tx = ExtractMidiInfos();

            MessageBox.Show(tx, "About this song", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string ExtractMidiInfos()
        {
            string tx = string.Empty;
            int i;
            string cr = Environment.NewLine;

            // Karaoke infos
            for (i = 0; i < sequence1.KTag.Count; i++)
            {
                tx += sequence1.KTag[i] + cr;
            }

            tx += cr;
            // Version
            for (i = 0; i < sequence1.VTag.Count; i++)
            {
                tx += sequence1.VTag[i] + cr;
            }
            // Lang
            for (i = 0; i < sequence1.LTag.Count; i++)
            {
                tx += sequence1.LTag[i] + cr;
            }

            tx += cr;
            // Copyright of karaoke
            for (i = 0; i < sequence1.WTag.Count; i++)
            {
                tx += sequence1.WTag[i] + cr;
            }

            tx += cr;
            // Song infos
            for (i = 0; i < sequence1.TTag.Count; i++)
            {
                tx += sequence1.TTag[i] + cr;
            }

            tx += cr;
            // Infos
            for (i = 0; i < sequence1.ITag.Count; i++)
            {
                tx += sequence1.ITag[i] + cr;
            }

            return tx;
        }


        private string ExtractTMidiInfos()
        {
            string tx = string.Empty;
            int i;
            string cr = Environment.NewLine;
            
            // Copyright of karaoke
            for (i = 0; i < sequence1.WTag.Count; i++)
            {
                if (sequence1.WTag[i] != "")
                    tx += sequence1.WTag[i] + cr;
            }

            if (tx != "")
                tx += cr;

            // Song infos
            for (i = 0; i < sequence1.TTag.Count; i++)
            {
                if (sequence1.TTag[i] != "")
                    tx += sequence1.TTag[i] + cr;
            }

            if (tx != "")
                tx += cr;

            // Infos
            for (i = 0; i < sequence1.ITag.Count; i++)
            {
                if (sequence1.ITag[i] != "")
                    tx += sequence1.ITag[i] + cr;
            }

            return tx;
        }

        #endregion mnu Help

        #endregion menus


        #region print text pdf

        /// <summary>
        /// Print words in a text file
        /// </summary>
        private void PrintText()
        {
            String tx = txtDisplayWords.Text;
            string message; // = string.Empty;
            string initname = Path.GetFileNameWithoutExtension(MIDIfileFullPath);
            initname += ".txt";

            SaveFileDialog dialog = new SaveFileDialog()
            {
                ShowHelp = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                DefaultExt = "txt",
                Filter = "Text Document (*.txt)|*.txt",
                FileName = initname,
            };
            //dialog.FileName = initname;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Create a dialog with a progress bar 
                Form progressDialog = new Form()
                {
                    Text = "Generating Text Document...",
                    BackColor = Color.White,
                    Size = new Size(400, 80),
                };

                System.Windows.Forms.ProgressBar progressBar = new System.Windows.Forms.ProgressBar()
                {
                    Parent = progressDialog,
                    Size = new Size(300, 20),
                    Location = new Point(10, 10),
                    Minimum = 1,
                    Maximum = 2, //numpages + 2,
                    Value = 2,
                    Step = 1,
                };

                progressDialog.Show();
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(500);


                string filename = dialog.FileName;
                try
                {                    
                    string title = Path.GetFileName(filename);

                    System.IO.File.WriteAllText(@filename, tx);

                    progressBar.PerformStep();
                    System.Windows.Forms.Application.DoEvents();
                                     
                    System.Threading.Thread.Sleep(500);
                }
                catch (System.IO.IOException ep)
                {
                    message = "";
                    message += "Karaboss was unable to save to file " + filename;
                    message += " because:\n" + ep.Message + "\n";

                    MessageBox.Show(message, "Error Saving File",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                progressDialog.Dispose();


                // Display created text file
                System.Diagnostics.Process.Start(@filename);
            }            
        }

        /// <summary>
        /// Save chords map to PDF format
        /// </summary>
        private void PrintPDF()
        {
            string message; // = string.Empty;
            string initname = Path.GetFileNameWithoutExtension(MIDIfileFullPath);
            //initname += ".pdf";

            int width = 0;
            int height = 0;
            int oldheight = 0;

            // Calculate height & width of controls in order to make a bitmap
            // corresponding to these dimensions
            if (tabChordsControl.SelectedIndex == 1)
            {
                //Chords Map
                width = ChordMapControl1.Width;
                height = ChordMapControl1.Height;
                initname += "-chords.pdf";
            }
            else if (tabChordsControl.SelectedIndex == 2)
            {
                // Lyrics
                width = txtDisplayWords.Width;                
                
                // The textbox has scrollbars: it has not the required dimensions
                // We have to Caculate the height of the text hosted in the textbox
                StringFormat sf = new StringFormat();
                Graphics gr = txtDisplayWords.CreateGraphics();
                SizeF sz = gr.MeasureString(txtDisplayWords.Lines[0], txtDisplayWords.Font, new Point(0, 0), sf);
                height = (int)sz.Height * txtDisplayWords.Lines.Count();
                // Save initial height of panel hosting the textbox
                oldheight = pnlDisplayWords.Height;
                // Apply calculated heihgt of text
                pnlDisplayWords.Height = height;
                initname += "-Lyrics.pdf";
            }
            else if (tabChordsControl.SelectedIndex == 3)
            {
                //Chords Map Modify
                width = ChordMapControlModify.Width;
                height = ChordMapControlModify.Height;
                initname += "-chords.pdf";
            }
            else
            {
                MessageBox.Show("Error printing PDF", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            SaveFileDialog dialog = new SaveFileDialog()
            {
                ShowHelp = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                DefaultExt = "pdf",
                Filter = "PDF Document (*.pdf)|*.pdf",
                FileName = initname,
            };

            //dialog.FileName = initname;
            int numpages = 2; // (int)Math.Ceiling(height / (float)PageHeight);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                // Create a dialog with a progress bar 
                Form progressDialog = new Form()
                {
                    Text = "Generating PDF Document...",
                    BackColor = Color.White,
                    Size = new Size(400, 80),
                };

                System.Windows.Forms.ProgressBar progressBar = new System.Windows.Forms.ProgressBar()
                {
                    Parent = progressDialog,
                    Size = new Size(300, 20),
                    Location = new Point(10, 10),
                    Minimum = 1,
                    Maximum = 2, //numpages + 2,
                    Value = 2,
                    Step = 1,
                };

                progressDialog.Show();
                System.Windows.Forms.Application.DoEvents();
                System.Threading.Thread.Sleep(500);


                string filename = dialog.FileName;
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Create);
                    string title = Path.GetFileName(filename);

                    Karaboss.PDFWithImages pdfdocument = new PDFWithImages(stream, title, numpages) {

                        DocWidth = width,
                        DocHeight = height,
                    };

                    Bitmap MemoryImage = new Bitmap(width, height);
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);

                    if (tabChordsControl.SelectedIndex == 1)
                    {                        
                        // Chords map
                        ChordMapControl1.DrawToBitmap(MemoryImage, new Rectangle(0, 0, width, height));
                    }
                    else if (tabChordsControl.SelectedIndex == 2)
                    {
                        // Words
                        pnlDisplayWords.DrawToBitmap(MemoryImage, new Rectangle(0, 0, width, height));
                        
                    }
                    else if (tabChordsControl.SelectedIndex == 3) 
                    {
                        ChordMapControlModify.DrawToBitmap(MemoryImage, new Rectangle(0, 0, width, height));
                    }
                    else
                    {
                        MessageBox.Show("Error printing PDF", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    pdfdocument.AddImage(MemoryImage);
                    MemoryImage.Dispose();
                    progressBar.PerformStep();
                    System.Windows.Forms.Application.DoEvents();

                    pdfdocument.Save();
                    stream.Close();
                    System.Threading.Thread.Sleep(500);
                }
                catch (System.IO.IOException ep)
                {
                    message = "";
                    message += "Karaboss was unable to save to file " + filename;
                    message += " because:\n" + ep.Message + "\n";

                    MessageBox.Show(message, "Error Saving File",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                progressDialog.Dispose();


                // Restore initial height of the panel (changed to fit text height)
                if (tabChordsControl.SelectedIndex == 2)
                    pnlDisplayWords.Height = oldheight;

                // Display created PDF
                System.Diagnostics.Process.Start(@filename);
            }

        }



        #endregion print text pdf


        #region Locate form
        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)System.Windows.Forms.Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

        #endregion Locate form


        #region Save file
                        
        /// <summary>
        /// File was modified
        /// </summary>
        public void FileModified()
        {
            bfilemodified = true;
            string fName = MIDIfileName;
            if (fName != null && fName != "")
            {
                string fExt = Path.GetExtension(fName);             // Extension
                fName = Path.GetFileNameWithoutExtension(fName);    // name without extension

                string fShortName = fName.Replace("*", "");
                if (fShortName == fName)
                    fName += "*";

                fName += fExt;
                SetTitle(fName);
            }
        }

        /// <summary>
        /// Save File
        /// </summary>
        private void SaveFileProc()
        {
            string fName = MIDIfileName;
            string fPath = MIDIfilePath;

            if (MIDIfileFullPath == null && fName != "" && fPath != "")
                MIDIfileFullPath = fPath + "\\" + fName;

            // Save all formats to Midi format
            string ext = Path.GetExtension(MIDIfileFullPath).ToLower();
            switch (ext)
            {
                case ".txt":
                case ".musicxml":
                case ".mxl":
                case ".xml":                   
                    MessageBox.Show("Your file will be saved in Midi format", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SaveAsFileProc();
                    return;                    
            }


            if (fPath == null || fPath == "" || fName == null || fName == "" || !File.Exists(MIDIfileFullPath))
            {
                SaveAsFileProc();
                return;
            }
            InitSaveFile(MIDIfileFullPath);
        }


        private void SaveAsFileProc()
        {
            string fName = MIDIfileName;
            string fPath = MIDIfilePath;
            string fullPath = MIDIfileFullPath;

            string fullName;
            string defName;


            // search path
            if (fPath == null || fPath == "")
                fPath = Utilities.CreateNewMidiFile._DefaultDirectory;

            // Search name
            if (MIDIfileName == null || MIDIfileName == "")
                fName = "New.mid";

            string ext = Path.GetExtension(MIDIfileFullPath).ToLower();
            switch (ext)
            {
                case ".musicxml":
                case ".mxl":
                case ".xml":
                    fName = Path.GetFileNameWithoutExtension(MIDIfileFullPath) + ".mid";
                    MIDIfileName = fName;
                    fPath = MIDIfilePath;
                    MIDIfileFullPath = fPath + "\\" + fName;
                    break;
            }


            #region search name

            string defExt = Path.GetExtension(fName);                           // Extension
            fullName = Utilities.Files.FindUniqueFileName(fullPath);    // Add (2), (3) etc.. if necessary    
            defName = Path.GetFileNameWithoutExtension(fullName);               // Default name to propose to dialog

            #endregion search name

            string defFilter = "MIDI files (*.mid)|*.mid|Kar files (*.kar)|*.kar|All files (*.*)|*.*";
            if (defExt == ".kar")
                defFilter = "Kar files (*.kar)|*.kar|MIDI files (*.mid)|*.mid|All files (*.*)|*.*";

            saveMidiFileDialog.Title = "Save MIDI file";
            saveMidiFileDialog.Filter = defFilter;
            saveMidiFileDialog.DefaultExt = defExt;
            saveMidiFileDialog.InitialDirectory = @fPath;
            saveMidiFileDialog.FileName = defName;

            if (saveMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveMidiFileDialog.FileName;

                MIDIfileFullPath = fileName;
                MIDIfileName = Path.GetFileName(fileName);
                MIDIfilePath = Path.GetDirectoryName(fileName);

                InitSaveFile(fileName);
            }
        }


        /// <summary>
        /// Save file: initialize events
        /// </summary>
        /// <param name="fileName"></param>
        public void InitSaveFile(string fileName)
        {
            progressBarPlayer.Visible = true;
            sequence1.SaveProgressChanged += HandleSaveProgressChanged;
            sequence1.SaveCompleted += HandleSaveCompleted;
            SaveFile(fileName);
        }

        /// <summary>
        /// Save the midi file
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveFile(string fileName)
        {
            try
            {
                if (fileName != "")
                {
                    sequence1.SaveAsync(fileName);
                }
            }
            catch (Exception errsave)
            {
                Console.Write(errsave.Message);
            }
        }

        /// <summary>
        /// Event: saving midi file in progress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (e.ProgressPercentage >= progressBarPlayer.Minimum && e.ProgressPercentage <= progressBarPlayer.Maximum)
                    progressBarPlayer.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Event: save midi file terminated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSaveCompleted(object sender, AsyncCompletedEventArgs e)
        {

            if (progressBarPlayer != null)
            {
                try
                {
                    progressBarPlayer.Value = 0;
                    progressBarPlayer.Visible = false;

                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

            }

            if (e.Error == null)
            {
                bfilemodified = false;
                if (bClosingRequired == true)
                {
                    this.Close();
                    return;
                }

                SetTitle(MIDIfileName);

                // Active le formulaire frmExplorer
                if (System.Windows.Forms.Application.OpenForms.OfType<frmExplorer>().Count() > 0)
                {
                    frmExplorer = GetForm<frmExplorer>();
                    frmExplorer.RefreshExplorer();
                }

            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        #endregion Save file


        #region Save lyrics

        #region Update Chord

        /// <summary>
        /// Insert new chord into gribeatchords
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="ChordName"></param>
        public void UpdateChord(int beat, string ChordName)
        {
            int ticks;
            int nbBeatsPerMeasure = sequence1.Numerator;

            if (nbBeatsPerMeasure == 0)
                return;
            
            int beatDuration = _measurelen / nbBeatsPerMeasure;

            if (GridBeatChords[beat].Item2 == 0)
                ticks = (beat - 1) * beatDuration;
            else
                ticks = GridBeatChords[beat].Item2;

            GridBeatChords[beat] = (ChordName, ticks);


            UpdateDisplayOfChords();
            DisplayWordsAndChords();

            FileModified();
        }

        #endregion Update Chord


        /// <summary>
        /// Put Chords of GridBeatChords in lyrics
        /// </summary>
        private void StoreChordsInLyrics()
        {
            // Source GridBeatChords
            if (myLyricsMgmt.MelodyTrackNum == -1)
                myLyricsMgmt.MelodyTrackNum = 0;

            Track track = sequence1.tracks[myLyricsMgmt.MelodyTrackNum];
            
            if (myLyricsMgmt.plLyrics.Count == 0)
                myLyricsMgmt.FullExtractLyrics(true);

            #region check
            if (myLyricsMgmt.ChordDelimiter == (null, null) || myLyricsMgmt.ChordDelimiter == ("", "") || myLyricsMgmt.RemoveChordPattern == null)
            {                                
                MessageBox.Show("Format of chords delimiters not found: [] or ()", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion check

            myLyricsMgmt.PopulateUpdatedChords(GridBeatChords);
            myLyricsMgmt.CleanLyrics();

           

            // Insert new lyrics in the sequence
            ReplaceLyrics(myLyricsMgmt.plLyrics, LyricTypes.Lyric, myLyricsMgmt.MelodyTrackNum);

        }


        /// <summary>
        /// Replace existing lyrics by others
        /// MelodyTrackNum: track hosting the melody
        /// LyricsTrackNum: track hosting the lyrics (text or lyric types)
        /// The target is to host the lyrics in the melody track
        /// </summary>
        /// <param name="pLyrics"></param>
        private void ReplaceLyrics(List<plLyric> newpLyrics, LyricTypes newLyricType, int melodytracknum)
        {
            // LyricType has changed => refresh display
            bool bRefreshDisplay = (newLyricType != myLyricsMgmt.LyricType);

            // Delete all lyrics of all types
            foreach (Track T in sequence1.tracks)
            {
                T.deleteLyrics();
                T.LyricsText.Clear();
                T.Lyrics.Clear();
            }
            // Tags associated to the sequence have been deleted
            restoreSequenceTags();

            // By default, insert the lyrics (either text or lyric) into the melodytrack
            Track track = sequence1.tracks[melodytracknum];

            // Insert all lyric events
            TrkInsertLyrics(track, newpLyrics, newLyricType);

            // Reload myLyricMgmt
            myLyricsMgmt = new LyricsMgmt(sequence1);


            // Refresh frmLyric
            if (myLyricsMgmt.OrgplLyrics.Count > 0)
            {
                // Reset display
                myLyricsMgmt.ResetDisplayChordsOptions(Karaclass.m_ShowChords);               
            }
            
            // File was modified
            FileModified();
        }


        /// <summary>
        /// Insert new lyrics in the target track
        /// </summary>
        /// <param name="Track"></param>
        /// <param name="l"></param>
        /// <param name="LyricType"></param>
        private void TrkInsertLyrics(Track Track, List<plLyric> l, LyricTypes LyricType)
        {
            int currentTick;
            int lastcurrenttick = 0;

            string currentElement;
            string currentCR = string.Empty;

            Track.Lyrics.Clear();
            Track.LyricsText.Clear();

            Track.TotalLyricsL = "";
            Track.TotalLyricsT = "";


            // Recréé tout les textes et lyrics
            for (int idx = 0; idx < l.Count; idx++)
            {
                plLyric pll = l[idx];

                // Si c'est un CR, le stocke et le collera au prochain lyric
                if (pll.CharType == plLyric.CharTypes.LineFeed)
                {
                    if (LyricType == LyricTypes.Text)
                        currentCR = m_SepLine;
                    else
                        currentCR = "\r";

                    // Update Track.Lyrics List
                    Track.Lyric L = new Track.Lyric()
                    {
                        Element = pll.Element.Item2,
                        TicksOn = pll.TicksOn,
                        Type = (Track.Lyric.Types)pll.CharType,
                    };

                    if (LyricType == LyricTypes.Text)
                    {
                        // si lyrics de type text                     
                        Track.LyricsText.Add(L);
                    }
                    else
                    {
                        // si lyrics de type lyrics
                        Track.Lyrics.Add(L);
                    }

                }
                else if (pll.CharType == plLyric.CharTypes.ParagraphSep)
                {
                    if (LyricType == LyricTypes.Text)
                        currentCR = m_SepParagraph;
                    else
                        currentCR = "\r\r";


                    // Update Track.Lyrics List
                    Track.Lyric L = new Track.Lyric()
                    {
                        Element = pll.Element.Item2,
                        TicksOn = pll.TicksOn,
                        Type = (Track.Lyric.Types)pll.CharType,
                    };

                    if (LyricType == LyricTypes.Text)
                    {
                        // si lyrics de type text                     
                        Track.LyricsText.Add(L);
                    }
                    else
                    {
                        // si lyrics de type lyrics
                        Track.Lyrics.Add(L);
                    }
                }
                else if (pll.CharType == plLyric.CharTypes.Text)
                {
                    // C'est un lyric
                    currentTick = pll.TicksOn;
                    if (currentTick >= lastcurrenttick)
                    {
                        string plElement;
                       
                        // Fix done in PopulateUpdatedChords                                    NON !!!!!
                        // Add chord name to the lyric: Replace lyric '-- ' by '[A]-- '
                        plElement = pll.Element.Item2;

                        lastcurrenttick = currentTick;
                        currentElement = currentCR + plElement;

                        // Transforme en byte la nouvelle chaine
                        // ERROR FAB 16-01-2021 : must tyake into accout encoding selected by end user !!!
                        byte[] newdata; // = Encoding.Default.GetBytes(currentElement);

                        switch (OpenMidiFileOptions.TextEncoding)
                        {
                            case "Ascii":
                                //sy = System.Text.Encoding.Default.GetString(data);
                                newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                                break;
                            case "Chinese":
                                System.Text.Encoding chinese = System.Text.Encoding.GetEncoding("gb2312");
                                newdata = chinese.GetBytes(currentElement);
                                break;
                            case "Japanese":
                                System.Text.Encoding japanese = System.Text.Encoding.GetEncoding("shift_jis");
                                newdata = japanese.GetBytes(currentElement);
                                break;
                            case "Korean":
                                System.Text.Encoding korean = System.Text.Encoding.GetEncoding("ks_c_5601-1987");
                                newdata = korean.GetBytes(currentElement);
                                break;
                            case "Vietnamese":
                                System.Text.Encoding vietnamese = System.Text.Encoding.GetEncoding("windows-1258");
                                newdata = vietnamese.GetBytes(currentElement);
                                break;
                            default:
                                newdata = System.Text.Encoding.Default.GetBytes(currentElement);
                                break;
                        }


                        MetaMessage mtMsg;

                        // Update Track.Lyrics List
                        Track.Lyric L = new Track.Lyric()
                        {
                            Element = plElement,
                            TicksOn = pll.TicksOn,
                            Type = (Track.Lyric.Types)pll.CharType,
                        };


                        if (LyricType == LyricTypes.Text)
                        {
                            // si lyrics de type text
                            mtMsg = new MetaMessage(MetaType.Text, newdata);
                            Track.LyricsText.Add(L);
                        }
                        else
                        {
                            // si lyrics de type lyrics
                            mtMsg = new MetaMessage(MetaType.Lyric, newdata);
                            Track.Lyrics.Add(L);
                        }

                        // Insert new message
                        Track.Insert(currentTick, mtMsg);
                    }
                    currentCR = "";
                }
            }
        }


        /// <summary>
        /// Rewrite tags level sequence
        /// </summary>
        private void restoreSequenceTags()
        {
            string tx;
            int i;

            if (sequence1.ITag != null)
            {
                for (i = sequence1.ITag.Count - 1; i >= 0; i--)
                {
                    tx = "@I" + sequence1.ITag[i];
                    AddTag(tx);
                }
            }

            if (sequence1.KTag != null)
            {
                for (i = sequence1.KTag.Count - 1; i >= 0; i--)
                {
                    tx = "@K" + sequence1.KTag[i];
                    AddTag(tx);
                }
            }
            if (sequence1.LTag != null)
            {
                for (i = sequence1.LTag.Count - 1; i >= 0; i--)
                {
                    tx = "@L" + sequence1.LTag[i];
                    AddTag(tx);
                }
            }
            if (sequence1.TTag != null)
            {
                for (i = sequence1.TTag.Count - 1; i >= 0; i--)
                {
                    tx = "@T" + sequence1.TTag[i];
                    AddTag(tx);
                }
            }
            if (sequence1.VTag != null)
            {
                for (i = sequence1.VTag.Count - 1; i >= 0; i--)
                {
                    tx = "@V" + sequence1.VTag[i];
                    AddTag(tx);
                }
            }
            if (sequence1.WTag != null)
            {
                for (i = sequence1.WTag.Count - 1; i >= 0; i--)
                {
                    tx = "@W" + sequence1.WTag[i];
                    AddTag(tx);
                }
            }

        }

        /// <summary>
        /// Insert Tag at tick 0
        /// </summary>
        /// <param name="strTag"></param>
        private void AddTag(string strTag)
        {
            Track track = sequence1.tracks[0];
            int currentTick = 0;
            string currentElement = strTag;

            // Transforme en byte la nouvelle chaine
            byte[] newdata = new byte[currentElement.Length];
            for (int u = 0; u < newdata.Length; u++)
            {
                newdata[u] = (byte)currentElement[u];
            }

            MetaMessage mtMsg;

            mtMsg = new MetaMessage(MetaType.Text, newdata);

            // Insert new message
            track.Insert(currentTick, mtMsg);
        }

        #endregion Save lyrics

       
    }
}
