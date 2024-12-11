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
using ChordAnalyser;
using ChordAnalyser.UI;
using Karaboss.Display;
using Karaboss.Lyrics;
using MusicTxt;
using MusicXml;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace Karaboss
{
    public partial class frmChords : Form
    {

        #region private dcl
        MusicXmlReader MXmlReader; 
        MusicTxtReader MTxtReader;


        private bool closing = false;
        private bool scrolling = false;

        // Current file beeing edited
        private string MIDIfileName = string.Empty;
        private string MIDIfilePath = string.Empty;
        private string MIDIfileFullPath = string.Empty;

        //private int bouclestart = 0;
        private int newstart = 0;
        //private int laststart = 0;      // Start time to play
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
        private OutputDevice outDevice;
        private Sequencer sequencer1 = new Sequencer();

        private System.Windows.Forms.Timer timer1;
        private NoSelectButton btnPlay;
        private NoSelectButton btnRewind;

        private NoSelectButton btnZoomPlus;
        private NoSelectButton btnZoomMinus;

        private NoSelectButton btnPrintTXT;
        private NoSelectButton btnPrintPDF;


        // 1 rst TAB
        private PanelPlayer panelPlayer;
        private ColorSlider.ColorSlider positionHScrollBar;
        private ChordsControl ChordControl1;        
        private ChordRenderer ChordRendererGuitar; // Display bitmaps of chords used in the song played
        private ChordRenderer ChordRendererPiano;

        //Panels
        private Panel pnlDisplayHorz;           // chords in horizontal mode
        private int padding = 10;
        private Panel pnlDisplayImagesOfChords; // images of chords
        private Panel pnlBottom;                // Lyrics

        // Tabpage for image of chord (Guitar & Piano)        
        private TabControl tbPChords;
        private TabPage TabPageGuitar = new TabPage();
        private TabPage TabPagePiano = new TabPage();


        private Label lblLyrics;
        private Label lblOtherLyrics;
        

        // 2 nd TAB 
        private ChordsMapControl ChordMapControl1;
        private Panel pnlDisplayMap;       // chords in map mode        

        // 3 rd TAB
        private Panel pnlDisplayWords;
        private System.Windows.Forms.TextBox txtDisplayWords;
        

        #endregion controls


        // Midifile characteristics
        private double _duration = 0;  // en secondes
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

        // Search by half measure
        // int measure
        // string Chord 1st half measure
        // string chord 2nd half measure
        Dictionary<int, (string, string)> Gridchords;
        
        // New search (by beat)
        //public Dictionary<int, List<string>> GridBeatChords;
        public Dictionary<int, string> GridBeatChords;

        #endregion private dcl

        public frmChords(OutputDevice OtpDev, string FileName)
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            // Sequence
            MIDIfileFullPath = FileName;
            MIDIfileName = Path.GetFileName(FileName);
            MIDIfilePath = Path.GetDirectoryName(FileName);
                        
            outDevice = OtpDev;

            // Allow form keydown
            this.KeyPreview = true;

            // Title
            SetTitle(FileName);                        
        }

        #region Display Controls

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

                sequencer1 = new Sequencer();
                sequencer1.Position = 0;
                sequencer1.Sequence = sequence1;    // primordial !!!!!
                this.sequencer1.PlayingCompleted += new System.EventHandler(this.HandlePlayingCompleted);
                this.sequencer1.ChannelMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.ChannelMessageEventArgs>(this.HandleChannelMessagePlayed);                
                this.sequencer1.SysExMessagePlayed += new System.EventHandler<Sanford.Multimedia.Midi.SysExMessageEventArgs>(this.HandleSysExMessagePlayed);
                this.sequencer1.Chased += new System.EventHandler<Sanford.Multimedia.Midi.ChasedEventArgs>(this.HandleChased);
                this.sequencer1.Stopped += new System.EventHandler<Sanford.Multimedia.Midi.StoppedEventArgs>(this.HandleStopped);

                sequence1.Clean();
                UpdateMidiTimes();


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
            timer1 = new Timer();
            timer1.Interval = 20;
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

            panelPlayer = new PanelPlayer();
            panelPlayer.Parent = pnlToolbar;
            panelPlayer.Location = new Point(30 + btnPlay.Left + btnPlay.Width, 5);
            pnlToolbar.Controls.Add(panelPlayer);

            #endregion PanelPlay


            #region zoom

            btnZoomPlus = new NoSelectButton();
            btnZoomPlus.Parent = pnlToolbar;
            btnZoomPlus.Image = Karaboss.Properties.Resources.magnifyplus24;
            toolTip1.SetToolTip(btnZoomPlus, "100%");
            btnZoomPlus.UseVisualStyleBackColor = true;
            btnZoomPlus.Location = new Point(34 + panelPlayer.Left + panelPlayer.Width, 2);
            btnZoomPlus.Size = new Size(50, 50);
            btnZoomPlus.Text = "";            
            btnZoomPlus.Click += new EventHandler(btnZoomPlus_Click);
            
            pnlToolbar.Controls.Add(btnZoomPlus);

            btnZoomMinus = new NoSelectButton();
            btnZoomMinus.Parent = pnlToolbar;
            btnZoomMinus.Image = Karaboss.Properties.Resources.magnifyminus24;
            toolTip1.SetToolTip(btnZoomMinus, "100%");
            btnZoomMinus.UseVisualStyleBackColor = true;
            btnZoomMinus.Location = new Point(2 + btnZoomPlus.Left + btnZoomPlus.Width, 2);
            btnZoomMinus.Size = new Size(50, 50);
            btnZoomMinus.Text = "";
            btnZoomMinus.Click += new EventHandler(btnZoomMinus_Click);
            pnlToolbar.Controls.Add(btnZoomMinus);

            #endregion zoom

            #region export pdf text
            btnPrintPDF = new NoSelectButton();
            btnPrintPDF.Parent = pnlToolbar;
            //btnPrintPDF.Image = Properties.Resources.Apps_Pdf_icon;
            btnPrintPDF.Image = Properties.Resources.export_pdf32;
            toolTip1.SetToolTip(btnPrintPDF, "Export to PDF");
            btnPrintPDF.UseVisualStyleBackColor = true;
            btnPrintPDF.Location = new Point(2 + btnZoomMinus.Left + btnZoomMinus.Width);
            btnPrintPDF.Size = new Size(50, 50);
            btnPrintPDF.Text = "";
            btnPrintPDF.Click += new EventHandler(btnPrintPDF_Click);
            btnPrintPDF.Visible = false;
            pnlToolbar.Controls.Add((btnPrintPDF));

            btnPrintTXT = new NoSelectButton();
            btnPrintTXT.Parent = pnlToolbar;
            //btnPrintTXT.Image = Properties.Resources.table_multiple;
            btnPrintTXT.Image = Properties.Resources.export_txt48_2;
            toolTip1.SetToolTip(btnPrintTXT, "Export to Text");
            btnPrintTXT.UseVisualStyleBackColor = true;
            btnPrintTXT.Location = new Point(2 + btnPrintPDF.Left + btnPrintPDF.Width);
            btnPrintTXT.Size = new Size(50, 50);
            btnPrintTXT.Text = "";
            btnPrintTXT.Click += new EventHandler(btnPrintTXT_Click);
            btnPrintTXT.Visible = false;
            pnlToolbar.Controls.Add((btnPrintTXT));

            #endregion export pdf text

            #endregion Toolbar

            tabChordsControl.Top = pnlToolbar.Top + pnlToolbar.Height;
            tabChordsControl.Height =  this.ClientSize.Height - menuStrip1.Height - pnlToolbar.Height;
            tabChordsControl.Width = this.ClientSize.Width;

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
            pnlDisplayHorz = new Panel();
            pnlDisplayHorz.Parent = tabPageDiagrams;
            pnlDisplayHorz.Location = new Point(tabPageDiagrams.Margin.Left, tabPageDiagrams.Margin.Top);            
            pnlDisplayHorz.Size = new Size(tabPageDiagrams.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right, 150);                    
            pnlDisplayHorz.BackColor = Color.FromArgb(239, 244, 255); //Color.Chocolate;
            tabPageDiagrams.Controls.Add(pnlDisplayHorz);
            #endregion Panel Display horizontal chords
            

            #region ChordControl
            // 2 : add a chord control on top
            ChordControl1 = new ChordsControl();
            ChordControl1.Parent = pnlDisplayHorz;
            ChordControl1.Location = new Point(0, 0);            

            // Set size mandatory ??? unless, the control is not shown correctly
            ChordControl1.Size = new Size(pnlDisplayHorz.Width, ChordControl1.Height);
            
            ChordControl1.WidthChanged += new WidthChangedEventHandler(ChordControl_WidthChanged);
            ChordControl1.HeightChanged += new HeightChangedEventHandler(ChordControl_HeightChanged);
            ChordControl1.MouseDown += new MouseEventHandler(ChordControl_MouseDown);

            ChordControl1.ColumnWidth = 180;
            ChordControl1.ColumnHeight = 180;

            ChordControl1.Cursor = Cursors.Hand;
            ChordControl1.Sequence1 = this.sequence1;
            pnlDisplayHorz.Controls.Add(ChordControl1);
            #endregion


            #region positionHScrollBar
            // 3 : add a colorslider
            positionHScrollBar = new ColorSlider.ColorSlider();
            positionHScrollBar.Parent = pnlDisplayHorz;
            positionHScrollBar.ThumbImage = Properties.Resources.BTN_Thumb_Blue;
            positionHScrollBar.Size = new Size(pnlDisplayHorz.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right, 20);
            positionHScrollBar.Location = new Point(0, ChordControl1.Height);
            positionHScrollBar.Value = 0;
            positionHScrollBar.Minimum = 0;

            // Set maximum & visibility
            SetScrollBarValues();

            positionHScrollBar.TickStyle = TickStyle.None;
            positionHScrollBar.SmallChange = 1;
            positionHScrollBar.LargeChange = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.ShowDivisionsText = false;
            positionHScrollBar.ShowSmallScale = false;
            positionHScrollBar.MouseWheelBarPartitions = 1 + NbMeasures * sequence1.Numerator;
            positionHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(PositionHScrollBar_Scroll);
            pnlDisplayHorz.Controls.Add(positionHScrollBar);

            pnlDisplayHorz.Height = ChordControl1.Height + positionHScrollBar.Height + padding;

            #endregion


            #region bitmaps of chords
            // 4 : Add a panel in the middle
            // This panel will display a diagram for chords being played
            // 278; //248 + 30 : 248 pour accord de guitare (200 taille de l'image + 1.24 * 200 accord jouée 25% plus gros et + 30 pour les onglets            
            int htotale = 280;
            
            pnlDisplayImagesOfChords = new Panel();
            pnlDisplayImagesOfChords.Parent = tabPageDiagrams;
            pnlDisplayImagesOfChords.Location = new Point(tabPageDiagrams.Margin.Left, pnlDisplayHorz.Top + pnlDisplayHorz.Height);
            pnlDisplayImagesOfChords.Height = htotale;
            pnlDisplayImagesOfChords.BackColor = Color.FromArgb(239, 244, 255);
            pnlDisplayImagesOfChords.Width = pnlDisplayHorz.Width;
            tabPageDiagrams.Controls.Add(pnlDisplayImagesOfChords);


            #region tabPage to select Guitar or Piano
            // =======================================
            tbPChords = new TabControl();
            tbPChords.Parent = pnlDisplayImagesOfChords;
            tbPChords.Location = new Point(0, 0);            

            TabPage TabPageGuitar = new TabPage("Guitar");
            tbPChords.Controls.Add(TabPageGuitar);
            TabPage TabPagePiano = new TabPage("Piano");
            tbPChords.Controls.Add(TabPagePiano);

            tbPChords.Dock = DockStyle.Fill;
            pnlDisplayImagesOfChords.Controls.Add(tbPChords);

            #endregion tabpage


            #region ChordRenderer Guitar
            // =======================================
            ChordRendererGuitar = new ChordRenderer();            
            ChordRendererGuitar.Parent = TabPageGuitar;            
            ChordRendererGuitar.Location = new Point(TabPageGuitar.Margin.Left, TabPageGuitar.Margin.Top);
            
            ChordRendererGuitar.Height = htotale;
            ChordRendererGuitar.HeightChanged += new HeightChangedEventHandler(ChordRendererGuitar_HeightChanged);
            
            ChordRendererGuitar.ColumnWidth = 162;  
            ChordRendererGuitar.ColumnHeight = 186; 
            
            ChordRendererGuitar.DisplayMode = ChordRenderer.DiplayModes.Guitar;
            TabPageGuitar.Controls.Add(ChordRendererGuitar);
            #endregion ChordRenderer Guitar


            #region ChordRenderer Piano
            // =======================================
            // Tabpages dimension are not set if not visible => force redim
            TabPagePiano.Width = TabPageGuitar.Width;

            ChordRendererPiano = new ChordRenderer();
            ChordRendererPiano.Parent = TabPagePiano;
            ChordRendererPiano.Location = new Point(TabPagePiano.Margin.Left, TabPagePiano.Margin.Top);
            ChordRendererPiano.Width = TabPagePiano.ClientSize.Width;
            ChordRendererPiano.Height = htotale;
            ChordRendererPiano.HeightChanged += new HeightChangedEventHandler(ChordRendererPiano_HeightChanged);
            
            ChordRendererPiano.ColumnWidth = 286;   
            ChordRendererPiano.ColumnHeight = 137;  

            ChordRendererPiano.DisplayMode = ChordRenderer.DiplayModes.Piano;
            TabPagePiano.Controls.Add(ChordRendererPiano);
            #endregion ChordRenderer Guitar


            #endregion bitmaps of chords



            #region Panel Bottom
            // 5 : add a panel at the bottom
            // This panel will host the lyrics
            pnlBottom = new Panel();
            pnlBottom.Parent = this.tabPageDiagrams;
            pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            pnlBottom.BackColor = Color.White;
            pnlBottom.Dock = DockStyle.Bottom;            
            tabPageDiagrams.Controls.Add(pnlBottom);

            // 6 add a label for text being sung
            lblLyrics = new Label();
            lblLyrics.Parent = pnlBottom;
            lblLyrics.Location = new Point(0, 0);
            lblLyrics.BackColor = Color.FromArgb(239, 244, 255);
            lblLyrics.AutoSize = false;            
            Font fontLyrics = new Font("Arial", 32, FontStyle.Regular, GraphicsUnit.Pixel);
            lblLyrics.Height =fontLyrics.Height + 20;
            lblLyrics.Font = fontLyrics;
            lblLyrics.TextAlign = ContentAlignment.MiddleCenter;           
            lblLyrics.Dock = DockStyle.Top;
            lblLyrics.Text = "AD Lorem ipsus";
            
            

            // 7 : add a label for text to be sung
            lblOtherLyrics = new Label();
            lblOtherLyrics.Parent = pnlBottom;
            lblOtherLyrics.Location = new Point(0, lblLyrics.Height);
            lblOtherLyrics.Size = new Size(pnlBottom.Width, pnlBottom.Height - lblLyrics.Height);
            //lblOtherLyrics.BackColor = Color.YellowGreen; 
            lblOtherLyrics.BackColor = Color.FromArgb(0, 163, 0);
            lblOtherLyrics.AutoSize = false;                      
            lblOtherLyrics.Font = fontLyrics;
            lblOtherLyrics.TextAlign = ContentAlignment.TopCenter;                        
            lblOtherLyrics.Dock = DockStyle.Fill;
            lblOtherLyrics.Text = "Other lyrics";            
            
            
            // Add controls in reverse order, to insure that DockStyle.Fill will work properly
            pnlBottom.Controls.Add(lblOtherLyrics);
            pnlBottom.Controls.Add(lblLyrics);


            #endregion

            #endregion 1er TAB


            #region 2eme TAB

            #region display map chords
            pnlDisplayMap = new Panel();
            pnlDisplayMap.Parent = tabPageOverview;
            pnlDisplayMap.Location = new Point(tabPageOverview.Margin.Left, tabPageOverview.Margin.Top);
            pnlDisplayMap.Size = new Size(tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right, tabPageOverview.Height -  tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom);    
            pnlDisplayMap.BackColor = Color.White;
            pnlDisplayMap.AutoScroll = true;            
            tabPageOverview.Controls.Add(pnlDisplayMap);
            #endregion display map chords


            #region ChordMapControl
            ChordMapControl1 = new ChordsMapControl();
            ChordMapControl1.Parent = pnlDisplayMap;
            ChordMapControl1.Location = new Point(0, 0);            
            

            ChordMapControl1.WidthChanged += new MapWidthChangedEventHandler(ChordMapControl1_WidthChanged);
            ChordMapControl1.HeightChanged += new MapHeightChangedEventHandler(ChordMapControl1_HeightChanged);            
            ChordMapControl1.MouseDown += new MouseEventHandler(ChordMapControl1_MouseDown);

            ChordMapControl1.ColumnWidth = 80;
            ChordMapControl1.ColumnHeight = 80;

            ChordMapControl1.Cursor = Cursors.Hand;
            ChordMapControl1.Sequence1 = this.sequence1;
            ChordMapControl1.Size = new Size(ChordMapControl1.Width, ChordMapControl1.Height);            
            pnlDisplayMap.Size = new Size(tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right, tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom);
            pnlDisplayMap.Controls.Add(ChordMapControl1);
            #endregion ChordMapControl

            #endregion 2eme TAB 


            #region 3eme TAB
            pnlDisplayWords = new Panel();
            pnlDisplayWords.Parent = tabPageEdit;
            pnlDisplayWords.Location = new Point(tabPageEdit.Margin.Left, tabPageEdit.Margin.Top);
            pnlDisplayWords.Size = new Size(tabPageEdit.Width - tabPageEdit.Margin.Left - tabPageEdit.Margin.Right, tabPageEdit.Height - tabPageEdit.Margin.Top - tabPageEdit.Margin.Bottom);
            pnlDisplayWords.BackColor = Color.Coral;
            pnlDisplayWords.AutoScroll = true;
            tabPageEdit.Controls.Add(pnlDisplayWords);

            Font fontWords = new Font("Courier New", 22, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDisplayWords = new System.Windows.Forms.TextBox();
            txtDisplayWords.Parent = pnlDisplayWords;
            txtDisplayWords.Location = new Point(0, 0);
            txtDisplayWords.Multiline = true;
            txtDisplayWords.TextAlign = HorizontalAlignment.Center;
            txtDisplayWords.ScrollBars = ScrollBars.Both;
            txtDisplayWords.Size = new Size(pnlDisplayWords.Width, pnlDisplayWords.Height); 
            txtDisplayWords.Font = fontWords;
            txtDisplayWords.Text = "La petite maison dans la prairie\r\nIl était une fois dans l'ouest";
            txtDisplayWords.Dock = DockStyle.Fill;
            pnlDisplayWords.Controls.Add(txtDisplayWords);

            #endregion 3eme TAB
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


        #region Display Notes

        
        private void DisplayChords()
        {
            ChordsAnalyser.ChordAnalyser Analyser = new ChordsAnalyser.ChordAnalyser(sequence1);            
            // It can be used in DisplayChords if there are chords embedded in lyrics
            myLyricsMgmt = new LyricsMgmt(sequence1, true);

            // favors chords included in lyrics
            if (myLyricsMgmt != null && myLyricsMgmt.bHasChordsInLyrics)
            {
                // Chords by beat
                GridBeatChords = myLyricsMgmt.FillGridBeatChordsWithLyrics();
            }
            else
            {
                // No chords in lyrics => vertical analyse of notes to build a chord map
                // Display chords in the textbox                
                               
                // Chords by beat
                GridBeatChords = Analyser.GridBeatChords;            
            }


            //Change labels displayed
            for (int i = 1; i <= GridBeatChords.Count; i++)
            {
                GridBeatChords[i] = InterpreteChord(GridBeatChords[i]);
            }


            // Display Chords in horizontal cells            
            ChordControl1.GridBeatChords = GridBeatChords;           

            // Display chords for guitar & piano                        
            ChordRendererGuitar.GridBeatChords = GridBeatChords;
            ChordRendererPiano.GridBeatChords = GridBeatChords;

            ChordRendererGuitar.FilterChords();
            ChordRendererPiano.FilterChords();
            
            ChordMapControl1.GridBeatChords = GridBeatChords;
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
                return;                        
                                 
            if (e.Error == null && e.Cancelled == false)
            {                
                CommonLoadCompleted(MXmlReader.seq);
            }
            else
            {
                if (e.Error != null)
                    MessageBox.Show(e.Error.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            
            if (MTxtReader.seq == null) return;
            
            if (e.Error == null && e.Cancelled == false)
            {                
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
            myLyricsMgmt.LoadLyricsPerBeat();
            myLyricsMgmt.LoadLyricsLines();

            // Display lyrics on first tab
            ChordControl1.GridLyrics = myLyricsMgmt.Gridlyrics;

            DisplayLineLyrics(0);
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
            float zoom = ChordControl1.zoom;
            zoom -= (float)0.1;

            ChordControl1.zoom = zoom; //-= (float)0.1;
            ChordRendererGuitar.zoom = zoom;
            ChordRendererPiano.zoom = zoom;
            ChordMapControl1.zoom = zoom; // -= (float)0.1;

            SetScrollBarValues();

            toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
            toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
        }

        private void btnZoomPlus_Click(object sender, EventArgs e)
        {
            float zoom = ChordControl1.zoom;
            zoom += (float)0.1;

            ChordControl1.zoom = zoom; //+= (float)0.1;
            ChordRendererGuitar.zoom = zoom;
            ChordRendererPiano.zoom = zoom;
            ChordMapControl1.zoom = zoom; // += (float)0.1;

            SetScrollBarValues();

            toolTip1.SetToolTip(btnZoomPlus, string.Format("{0:P2}", zoom));
            toolTip1.SetToolTip(btnZoomMinus, string.Format("{0:P2}", zoom));
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

        #region chordcontrol
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
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }

        }

        #endregion chordcontrol


        #region chordrenderer events
        
        private void ChordRendererGuitar_HeightChanged(object sender, int value)
        {                        

            if (pnlBottom != null && pnlDisplayImagesOfChords != null && pnlDisplayHorz != null)
            {
                pnlBottom.Location = new Point(0, pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height);
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }
        }
                       
        
        private void ChordRendererPiano_HeightChanged(object sender, int value)
        {

            if (pnlBottom != null && pnlDisplayImagesOfChords != null && pnlDisplayHorz != null)
            {
                pnlBottom.Location = new Point(0, pnlDisplayImagesOfChords.Top + pnlDisplayImagesOfChords.Height);
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }
        }
      
        #endregion chordrenderer events



        #region chordmapcontrol
        private void ChordMapControl1_HeightChanged(object sender, int value)
        {            
            

        }

        private void ChordMapControl1_WidthChanged(object sender, int value)
        {            

        }

        private void ChordMapControl1_MouseDown(object sender, MouseEventArgs e)
        {            
            if (e.Button == MouseButtons.Left)
            {
                int x = e.Location.X;  //Horizontal
                int y = e.Location.Y + ChordMapControl1.OffsetY;  // Vertical

                // Calculate start time                
                int HauteurCellule = (int)(ChordMapControl1.ColumnHeight) + 1;
                int LargeurCellule = (int)(ChordMapControl1.ColumnWidth) + 1;
                int line = (int)Math.Ceiling(y / (double)HauteurCellule);
                int prevmeasures = -1 + (line - 1) * ChordMapControl1.NbColumns;
                int cellincurrentline = (int)Math.Ceiling(x / (double)LargeurCellule);

                newstart = _measurelen * prevmeasures + (_measurelen / sequence1.Numerator) * cellincurrentline;
                FirstPlaySong(newstart);
            }
        }


        #endregion chordmapcontrol


        /// <summary>
        /// A new tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabChordsControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            btnPrintPDF.Visible = (tabChordsControl.SelectedIndex != 0);
            btnPrintTXT.Visible = (tabChordsControl.SelectedIndex == 2);

            btnZoomPlus.Visible = (tabChordsControl.SelectedIndex != 2);
            btnZoomMinus.Visible = (tabChordsControl.SelectedIndex != 2);

            mnuFilePrintLyrics.Visible = (tabChordsControl.SelectedIndex == 2);
            mnuFilePrintPDF.Visible = (tabChordsControl.SelectedIndex != 0);
        }
       
       
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
            //int curline = (int)(Math.Ceiling((double)(curmeasure + 1) / ChordMapControl1.NbColumns));
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
                    //int offset = HauteurCellule * (curline - 2);
                    int offset = HauteurCellule * (curline - 1);

                    if (pnlDisplayMap.VerticalScroll.Visible && pnlDisplayMap.VerticalScroll.Minimum <= offset && offset <= pnlDisplayMap.VerticalScroll.Maximum)
                    {
                        //pnlDisplayMap.VerticalScroll.Value = HauteurCellule * (curline - 2);
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
                int LargeurCellule = (int)(ChordControl1.ColumnWidth * ChordControl1.zoom) + 1;
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
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;

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
                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
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
                Application.DoEvents();
                LoadAsyncXmlFile(MIDIfileFullPath);
            }
            else if (ext == ".txt")
            {
                Cursor.Current = Cursors.WaitCursor;
                Application.DoEvents();
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
            for (int i = 0; i < tabChordsControl.TabCount;i++)
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
                pnlDisplayHorz.Width = tabPageDiagrams.Width - tabPageDiagrams.Margin.Left - tabPageDiagrams.Margin.Right;
                pnlDisplayImagesOfChords.Width = pnlDisplayHorz.Width;

                for (int i = 0; i < tbPChords.TabCount; i++)
                {
                    if (i != tbPChords.SelectedIndex)
                    {
                        // Force other tabs to redim
                        tbPChords.TabPages[i].Width = tbPChords.TabPages[tbPChords.SelectedIndex].Width;
                    }
                }


                //ChordRendererGuitar.Width = tbPChords.TabPages[0].Width;
                ChordRendererPiano.Width = tbPChords.TabPages[1].Width;


                pnlBottom.Height = tabPageDiagrams.Height - tabPageDiagrams.Margin.Top - tabPageDiagrams.Margin.Bottom - pnlDisplayHorz.Height - pnlDisplayImagesOfChords.Height;
            }

            if (ChordControl1 != null)
            {
                positionHScrollBar.Width = (pnlDisplayHorz.Width > ChordControl1.Width ? ChordControl1.Width : pnlDisplayHorz.Width);
                positionHScrollBar.Top = ChordControl1.Top + ChordControl1.Height;
            }

            

            // 2nd TAB
            if (pnlDisplayMap != null)
            {
                pnlDisplayMap.Width = tabPageOverview.Width - tabPageOverview.Margin.Left - tabPageOverview.Margin.Right;                
                pnlDisplayMap.Height = tabPageOverview.Height - tabPageOverview.Margin.Top - tabPageOverview.Margin.Bottom;
            }


            // 3rd TAB
            if (pnlDisplayWords != null)
            {
                pnlDisplayWords.Width = tabPageEdit.Width - tabPageEdit.Margin.Left - tabPageEdit.Margin.Right;
                pnlDisplayWords.Height = tabPageEdit.Height - tabPageEdit.Margin.Top - tabPageEdit.Margin.Bottom;
            }


            // Set maximum & visibility
            SetScrollBarValues();
        }

        private void frmChords_FormClosing(object sender, FormClosingEventArgs e)
        {
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

            // Active le formulaire frmExplorer
            if (Application.OpenForms.OfType<frmExplorer>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmExplorer"].Restore();
                Application.OpenForms["frmExplorer"].Activate();

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
            _duration = _tempo * (_totalTicks / _ppqn) / 1000000; //seconds                        

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
            if (timer1 != null)
                timer1.Stop();
            scrolling = false;
            newstart = 0;
            //laststart = 0;
            _currentMeasure = -1;
            
            if (sequencer1 != null) 
                sequencer1.Stop();
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
                    //btnStop.Image = Properties.Resources.btn_black_stop;
                    btnPlay.Enabled = true;  // to allow pause
                    //btnStop.Enabled = true;  // to allow stop 
                    
                    //lblStatus.Text = "Playing";
                    //lblStatus.ForeColor = Color.LightGreen;
                    panelPlayer.DisplayStatus("Playing");
                    break;

                case PlayerStates.Paused:
                    btnPlay.Image = Properties.Resources.btn_green_play;
                    btnPlay.Enabled = true;  // to allow play
                    //btnStop.Enabled = true;  // to allow stop

                    //lblStatus.Text = "Paused";
                    //lblStatus.ForeColor = Color.Yellow;
                    panelPlayer.DisplayStatus("Paused");
                    break;

                case PlayerStates.Stopped:
                    btnPlay.Image = Properties.Resources.btn_black_play;
                    btnPlay.Enabled = true;   // to allow play
                    if (newstart == 0)
                    {
                        //btnStop.Image = Properties.Resources.btn_red_stop;
                    }
                    else
                    {
                        //btnStop.Enabled = true;   // to enable real stop because stop point not at the beginning of the song 
                    }

                    //lblStatus.Text = "Stopped";
                    //lblStatus.ForeColor = Color.Red;
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
            string message = string.Empty;
            string initname = Path.GetFileNameWithoutExtension(MIDIfileFullPath);
            initname += ".txt";

            SaveFileDialog dialog = new SaveFileDialog()
            {
                ShowHelp = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                DefaultExt = "txt",
                Filter = "Text Document (*.txt)|*.txt",
            };
            dialog.FileName = initname;

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
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);


                string filename = dialog.FileName;
                try
                {                    
                    string title = Path.GetFileName(filename);

                    System.IO.File.WriteAllText(@filename, tx);

                    progressBar.PerformStep();
                    Application.DoEvents();
                                     
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
            string message = string.Empty;
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


            SaveFileDialog dialog = new SaveFileDialog()
            {
                ShowHelp = true,
                CreatePrompt = false,
                OverwritePrompt = true,
                DefaultExt = "pdf",
                Filter = "PDF Document (*.pdf)|*.pdf",
            };

            dialog.FileName = initname;
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
                Application.DoEvents();
                System.Threading.Thread.Sleep(500);


                string filename = dialog.FileName;
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Create);
                    string title = Path.GetFileName(filename);

                    Karaboss.PDFWithImages pdfdocument = new PDFWithImages(stream, title, numpages);

                    pdfdocument.DocWidth = width;
                    pdfdocument.DocHeight = height;


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

                    pdfdocument.AddImage(MemoryImage);
                    MemoryImage.Dispose();
                    progressBar.PerformStep();
                    Application.DoEvents();

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

      
    }
}
