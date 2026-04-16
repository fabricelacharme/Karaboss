#region License

/* Copyright (c) 2025 Fabrice Lacharme
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
using GradientApp;
using kar;
using Karaboss.Mp3;
using Karaboss.Resources.Localization;
using keffect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmLyrOptions : Form
    {

        #region private declarations

        private Dictionary<string, string> KaraokeTypes = new Dictionary<string, string>();

        private Karaclass.OptionsDisplay OptionDisplay;        
        private string bgOption = "Diaporama";
        
        // Font
        private Font _karaokeFont;
        private string ftName = "Arial Black";
        private uint ftSize = 20;

        // Frame
        private string FrameType;        

        // Text color
        private Color InactiveColor;
        // Text to sing color
        private Color HighlightColor;
        // Text sung color
        private Color ActiveColor;

        // Border color        
        private Color ActiveBorderColor;
        private Color InactiveBorderColor;


        #region background colors

        // Background colors
        private Color BgColor;        
        private Color Grad0Color;
        private Color Grad1Color;
        private Color Rhythm0Color;
        private Color Rhythm1Color;

        #endregion background colors


        #region Chords
        // Chord color
        private Color InactiveChordColor;
        private Color HighlightChordColor;
        private bool _bShowChords = false;

        #endregion Chords

        // Lyrics TopMost
        private bool _bTopMost = false;

        // Force Uppercase
        private bool bForceUppercase = false;

        // Number of lines to display
        private int NbLines;
        //Slideshow
        private string dirSlideShow;
        // Frequency
        private int freqSlideShow;

        // Size mode of the picture background
        private PictureBoxSizeMode _sizeMode;
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                pBox.SizeMode = _sizeMode;
            }
        }

        // Number of lines to display
        private int _nbLyricsLines;

        // Karaoke display type (FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped
        private string KaraokeDisplayType;

        #endregion private declarations

        public frmLyrOptions()
        {
            InitializeComponent();  
            
            TopMost = true;

            LoadOptions();     
            SetOptions();

            pBox.DirSlideShow = dirSlideShow;
            pBox.bIsSettings = true;
            pBox.LoadDemoText();
        }


        #region option form settings
       
        /// <summary>
        /// Load options stored in properties
        /// </summary>
        private void LoadOptions()
        {
            try
            {               
                // Fonts
                PopulateFonts();
                                                
                string f = Properties.Settings.Default.KaraokeFontName;
                for (int i = 0; i < cbFontName.Items.Count; i++)
                {
                    if (cbFontName.Items[i].ToString() == f)
                    {
                        cbFontName.SelectedIndex = i;
                        break;
                    }
                }
                _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular);                
                pBox.KaraokeFont = _karaokeFont;


                // Frames
                PopulateLyricBorders();
                
                // Lyrics border effect 
                FrameType = Properties.Settings.Default.FrameType;                                         
                foreach (KeyValuePair<string, string> valuePair in cbFrameType.Items)
                {
                    if (!string.IsNullOrEmpty(valuePair.Key))
                    {
                        if (valuePair.Key == FrameType)
                        {
                            cbFrameType.SelectedItem = valuePair;
                            break;
                        }
                    }
                }


                // Karaoke display types
                // karaokeEffect1 is updated by the options form when changing the display type, so we need to set it before setting the selected item in the combo box
                PopulateKaraokeDisplayTypes();
                KaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;
                foreach (KeyValuePair<string, string> valuePair in cbKaraokeType.Items)
                {
                    if (!string.IsNullOrEmpty(valuePair.Key))
                    {
                        if (valuePair.Key == KaraokeDisplayType)
                        {
                            cbKaraokeType.SelectedItem = valuePair;
                            break;
                        }
                    }
                }

                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;
              
                // Display balls on lyrics
                chkDisplayBalls.Checked = Karaclass.m_DisplayBalls;

                // Background type (Diaporama, Solidcolor, Transparent)                
                Grad0Color = Properties.Settings.Default.Grad0Color;
                Grad1Color = Properties.Settings.Default.Grad1Color;
                Rhythm0Color = Properties.Settings.Default.Rhythm0Color;
                Rhythm1Color = Properties.Settings.Default.Rhythm1Color;

                // Colors: Properties => textBox                
                txtBgColor.Text = Properties.Settings.Default.BgColor;
                txtActiveColor.Text = Properties.Settings.Default.ActiveColor;
                txtHighlightColor.Text = Properties.Settings.Default.HighlightColor;
                txtInactiveColor.Text = Properties.Settings.Default.InactiveColor;
                txtActiveBorderColor.Text = Properties.Settings.Default.ActiveBorderColor;
                txtInactiveBorderColor.Text = Properties.Settings.Default.InactiveBorderColor;

                txtInactiveChordColor.Text = Properties.Settings.Default.InactiveChordColor;
                txtHighlightChordColor.Text = Properties.Settings.Default.HighlightChordColor;


                // textBox => pic 
                picBgColor.BackColor = Parse(txtBgColor.Text);

                picActiveColor.BackColor = Parse(txtActiveColor.Text);
                picHighlightColor.BackColor = Parse(txtHighlightColor.Text);
                picInactiveColor.BackColor = Parse(txtInactiveColor.Text);
                picActiveBorderColor.BackColor = Parse(txtActiveBorderColor.Text);
                picInactiveBorderColor.BackColor = Parse(txtInactiveBorderColor.Text);
                
                picInactiveChordColor.BackColor = Parse(txtInactiveChordColor.Text);
                picHighlightChordColor.BackColor = Parse(txtHighlightChordColor.Text);

                // pic => variables
                BgColor = picBgColor.BackColor;

                ActiveColor = picActiveColor.BackColor;
                HighlightColor = picHighlightColor.BackColor;
                InactiveColor = picInactiveColor.BackColor;

                ActiveBorderColor = picActiveBorderColor.BackColor;
                InactiveBorderColor = picInactiveBorderColor.BackColor;


                // Chords
                InactiveChordColor = picInactiveChordColor.BackColor;
                HighlightChordColor = picHighlightChordColor.BackColor;
                _bShowChords = Properties.Settings.Default.bShowChords;
                

                // Window lyris topmost
                _bTopMost = Properties.Settings.Default.frmMidiLyricsTopMost;
                chkTopMost.Checked = _bTopMost;

                // Backgroud color beside lyrics to help to read when an image is displayed
                chkTextBackground.Checked = Properties.Settings.Default.bLyricsBackGround;

                switch (Properties.Settings.Default.LyricsOptionDisplay)
                {
                    case "Top":
                        OptionDisplay = Karaclass.OptionsDisplay.Top;
                        cbOptionsTextDisplay.SelectedIndex = 0;
                        pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Top;
                        break;
                    case "Center":
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        cbOptionsTextDisplay.SelectedIndex = 1;
                        pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Center;
                        break;
                    case "Bottom":
                        OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                        cbOptionsTextDisplay.SelectedIndex = 2;
                        pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Bottom;
                        break;
                    default:
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        cbOptionsTextDisplay.SelectedIndex = 1;
                        pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Center;
                        break;
                }

                // Background
                string bgOption = Properties.Settings.Default.BackGroundOption;

                switch (bgOption)
                {
                    case "Diaporama":
                        radioDiaporama.Checked = true;
                        break;
                    case "SolidColor":
                        radioSolidColor.Checked = true;
                        break;
                    case "Gradient":
                        radioGradient.Checked = true;
                        break;
                    case "Rhythm":
                        radioRhythm.Checked = true;
                        break;
                    case "Transparent":
                        radioTransparent.Checked = true;
                        break;
                    default:
                        bgOption = "Diaporama";
                        break;
                }


                // Nb lines to display
                NbLines = Properties.Settings.Default.TxtNbLines;


                // SlideShow directory
                dirSlideShow = Properties.Settings.Default.dirSlideShow;

                if (Directory.Exists(dirSlideShow) == false)
                    dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);


                freqSlideShow = Properties.Settings.Default.freqSlideShow;


                switch (Properties.Settings.Default.SizeMode)
                {
                    case PictureBoxSizeMode.Normal:
                        cbSizeMode.SelectedText = "Normal";
                        cbSizeMode.Text = "Normal";
                        break;
                    case PictureBoxSizeMode.AutoSize:
                        cbSizeMode.SelectedText = "AutoSize";
                        cbSizeMode.Text = "AutoSize";
                        break;
                    case PictureBoxSizeMode.CenterImage:
                        cbSizeMode.SelectedText = "CenterImage";
                        cbSizeMode.Text = "CenterImage";
                        break;
                    case PictureBoxSizeMode.StretchImage:
                        cbSizeMode.SelectedText = "StretchImage";
                        cbSizeMode.Text = "StretchImage";
                        break;
                    case PictureBoxSizeMode.Zoom:
                        cbSizeMode.SelectedText = "Zoom";
                        cbSizeMode.Text = "Zoom";
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
                BgColor = Color.White;
                Grad0Color = Color.Blue;
                Grad1Color = Color.Green;
                Rhythm0Color = Color.Blue;
                Rhythm1Color = Color.Green;

                ActiveColor = Color.Black;
                HighlightColor = Color.Red;
                InactiveColor = Color.YellowGreen;
                ActiveBorderColor = Color.Black;
                NbLines = 3;                
                
                dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName) + "\\slideshow";

                freqSlideShow = 10;
                SizeMode = PictureBoxSizeMode.Zoom;
            }
        }


        private void PopulateKaraokeDisplayTypes()
        {
            // Populate karaoke display types cbKaraokeType         
            KaraokeTypes = new Dictionary<string, string>();
            KaraokeTypes.Add("FixedLines", Strings.KTypesFixedLines);
            KaraokeTypes.Add("ScrollingLinesBottomUp", Strings.KTypesScrollingLinesBottomUp);
            KaraokeTypes.Add("ScrollingLinesTopDown", Strings.KTypesScrollingLinesTopDown);
            KaraokeTypes.Add("TwoLinesSwapped", Strings.KTypesTwoLinesSwapped);
            KaraokeTypes.Add("FourLinesSwapped", Strings.KTypesFourLinesSwapped);
            cbKaraokeType.DataSource = new BindingSource(KaraokeTypes, null);
            cbKaraokeType.ValueMember = "Key";
            cbKaraokeType.DisplayMember = "Value";
        }


        private void PopulateFonts()
        {
            // Karafun seems to support only a few fonts
            foreach (System.Drawing.FontFamily fnt in System.Drawing.FontFamily.Families)
            {
                cbFontName.Items.Add(fnt.Name);
            }
            
            for (int i = 0; i < cbFontName.Items.Count; i++)
            {
                if (cbFontName.Items[i].ToString() == "Arial Black")
                {
                    cbFontName.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// Frames
        /// </summary>
        private void PopulateLyricBorders()
        {
            Dictionary<string, string> Frames = new Dictionary<string, string>();
            Frames.Add("NoBorder", Strings.KfnBorderNoBorder);
            Frames.Add("FrameThin", Strings.KfnBorderFrameThin);
            Frames.Add("Frame1", Strings.KfnBorderFrame1);
            Frames.Add("Frame2", Strings.KfnBorderFrame2);
            Frames.Add("Frame3", Strings.KfnBorderFrame3);
            Frames.Add("Frame4", Strings.KfnBorderFrame4);
            Frames.Add("Frame5", Strings.KfnBorderFrame5);
            Frames.Add("Shadow", Strings.KfnBorderShadow);
            Frames.Add("Neon", Strings.KfnBorderNeon);

        
            cbFrameType.DataSource = new BindingSource(Frames, null);
            cbFrameType.ValueMember = "Key";
            cbFrameType.DisplayMember = "Value";

            if (cbFrameType.Items.Count > 2)
                cbFrameType.SelectedIndex = 2; // 1 pixel
        }

        /// <summary>
        /// Save options in properties
        /// </summary>
        private void SaveOptions()
        {
            try
            {
                // Display balls
                Properties.Settings.Default.DisplayBalls = Karaclass.m_DisplayBalls;

                // Background type (Diaporama, Solidcolor, Transparent
                Properties.Settings.Default.BackGroundOption = bgOption;

                // Font                
                Properties.Settings.Default.KaraokeFontName = ftName;                


                // Background colors
                Properties.Settings.Default.BgColor = ToHex(BgColor);
                Properties.Settings.Default.Grad0Color = Grad0Color;
                Properties.Settings.Default.Grad1Color = Grad1Color;
                Properties.Settings.Default.Rhythm0Color = Rhythm0Color;
                Properties.Settings.Default.Rhythm1Color = Rhythm1Color;


                Properties.Settings.Default.ActiveColor = ToHex(ActiveColor);
                Properties.Settings.Default.HighlightColor = ToHex(HighlightColor);
                Properties.Settings.Default.InactiveColor = ToHex(InactiveColor);

                // chords
                Properties.Settings.Default.InactiveChordColor = ToHex(InactiveChordColor);
                Properties.Settings.Default.HighlightChordColor = ToHex(HighlightChordColor);
                Properties.Settings.Default.bShowChords = _bShowChords;

                // Contour                
                Properties.Settings.Default.ActiveBorderColor = ToHex(ActiveBorderColor);
                Properties.Settings.Default.InactiveBorderColor = ToHex(InactiveBorderColor);

                
                // FrameType
                Properties.Settings.Default.FrameType = FrameType;

                // window lyrics topmost
                Properties.Settings.Default.frmMidiLyricsTopMost = _bTopMost;


                // Force Uppercase
                Properties.Settings.Default.bForceUppercase = bForceUppercase;

                // Number of lines to display
                Properties.Settings.Default.TxtNbLines = NbLines;

                // SlideShow
                dirSlideShow = txtSlideShow.Text.Trim();
                if (Directory.Exists(dirSlideShow) == false)
                    dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
                Properties.Settings.Default.dirSlideShow = dirSlideShow;

                Properties.Settings.Default.freqSlideShow = freqSlideShow;
                Properties.Settings.Default.SizeMode = SizeMode;

                switch (OptionDisplay)
                {
                    case Karaclass.OptionsDisplay.Top:
                        Properties.Settings.Default.LyricsOptionDisplay = "Top";
                        break;
                    case Karaclass.OptionsDisplay.Center:
                        Properties.Settings.Default.LyricsOptionDisplay = "Center";
                        break;
                    case Karaclass.OptionsDisplay.Bottom:
                        Properties.Settings.Default.LyricsOptionDisplay = "Bottom";
                        break;
                    default:
                        Properties.Settings.Default.LyricsOptionDisplay = "Center";
                        break;
                }

                // Lyrics background
                Properties.Settings.Default.bLyricsBackGround = chkTextBackground.Checked;

                // Karaoke display type
                Properties.Settings.Default.KaraokeDisplayType = KaraokeDisplayType;

                // Save all
                Properties.Settings.Default.Save();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }
        }


        /// <summary>
        /// Appply options to option form
        /// </summary>
        private void SetOptions()
        {
            try
            {
                pnlBalls.Visible = chkDisplayBalls.Checked;

                // Nombre de lignes à afficher
                UpDownNbLines.Value = NbLines;

                // Slideshow
                txtSlideShow.Text = dirSlideShow;
                txtSlideShowFreq.Text = freqSlideShow.ToString();

                // Background buttons
                picBgColor.BackColor = BgColor;
                                       

                picActiveColor.BackColor = ActiveColor;
                picHighlightColor.BackColor = HighlightColor;
                picInactiveColor.BackColor = InactiveColor;

                picActiveBorderColor.BackColor = ActiveBorderColor;
                picInactiveBorderColor.BackColor = InactiveBorderColor;

                // Chords
                picInactiveChordColor.BackColor = InactiveChordColor;
                picHighlightChordColor.BackColor = HighlightChordColor;
                pBox.bShowChords = _bShowChords;

                // Window Lyrics TopMost
                chkTopMost.Checked = _bTopMost;

                // Force uppercase
                chkTextUppercase.Checked = bForceUppercase;
                pBox.bforceUppercase = bForceUppercase;

                // picturebox            
                pBox.FreqDirSlideShow = freqSlideShow;
                pBox.nbLyricsLines = NbLines;
                pBox.CurrentTime = 30;

                // Backgrounds
                pBox.BgColor = BgColor;
                pBox.Grad0Color = Grad0Color;
                pBox.Grad1Color = Grad1Color;
                pBox.Rhythm0Color = Rhythm0Color;
                pBox.Rhythm1Color = Rhythm1Color;
                
                pBox.ActiveBorderColor = ActiveBorderColor;
                pBox.InactiveBorderColor = InactiveBorderColor;

                pBox.ActiveColor = ActiveColor;
                pBox.HighlightColor = HighlightColor;
                pBox.InactiveColor = InactiveColor;

                // Chords
                pBox.InactiveChordColor = InactiveChordColor;
                pBox.HighlightChordColor = HighlightChordColor;
                chkForceShowChords.Checked = _bShowChords;

                // Frame type
                pBox.FrameType = FrameType;

               
                cbSizeMode.SelectedText = SizeMode.ToString();

                pBox.OptionBackground = bgOption;
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);

            }
        }

        /// <summary>
        /// Apply colors to option form
        /// </summary>
        private void ApplyNewColors()
        {                       
            // Backgrounds
            pBox.BgColor = BgColor;
            pBox.Grad0Color = Grad0Color;
            pBox.Grad1Color = Grad1Color;
            pBox.Rhythm0Color = Rhythm0Color;
            pBox.Rhythm1Color = Rhythm1Color;            

            pBox.ActiveColor = ActiveColor;
            pBox.HighlightColor = HighlightColor;
            pBox.InactiveColor = InactiveColor;

            pBox.ActiveBorderColor = ActiveBorderColor;
            pBox.InactiveBorderColor = InactiveBorderColor;                      

            // Top, center, bottom
            pBox.OptionDisplay = (PicControl.pictureBoxControl.OptionsDisplay)OptionDisplay;

            // Chords
            pBox.InactiveChordColor = InactiveChordColor;
            pBox.HighlightChordColor= HighlightChordColor;


            //Color of buttons
            picBgColor.BackColor = BgColor;

            picActiveColor.BackColor = ActiveColor;
            picInactiveColor.BackColor = InactiveColor;
            picHighlightColor.BackColor = HighlightColor;

            picActiveBorderColor.BackColor = ActiveBorderColor;
            picInactiveBorderColor.BackColor = InactiveBorderColor;

            picInactiveChordColor.BackColor = InactiveChordColor;
            picHighlightChordColor.BackColor = HighlightChordColor;               
        }


        #endregion option form settings
     

        #region buttons
        /// <summary>
        /// Select directory for slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDirSlideShow_Click(object sender, EventArgs e)
        {
            
            folderBrowserDialog1.SelectedPath = dirSlideShow;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // Force display to Option Diaporama
                radioDiaporama.Checked = true;

                dirSlideShow = folderBrowserDialog1.SelectedPath;
                
                Cursor.Current = Cursors.WaitCursor;
                txtSlideShow.Text = dirSlideShow;         // pBox is updated in txtSlideShow_TextChanged event

            }
        }

        /// <summary>
        /// Reset directory for slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetDir_Click(object sender, EventArgs e)
        {
            // Force display to Option Diaporama
            radioDiaporama.Checked = true;

            dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            txtSlideShow.Text = dirSlideShow;
            pBox.DirSlideShow = dirSlideShow;
        }

        /// <summary>
        /// Apply changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }

        /// <summary>
        /// Apply changes and exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Dispose();
        }

        // Apply changes to frmMidiLyrics
        private void ApplyChanges()
        {
            SaveOptions();

            if (Application.OpenForms.OfType<frmMidiLyrics>().Count() > 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                frmMidiLyrics frmMidiLyrics = Utilities.FormUtilities.GetForm<frmMidiLyrics>();

                frmMidiLyrics.bShowBalls = Karaclass.m_DisplayBalls;

                frmMidiLyrics.KaraokeFont = _karaokeFont;

                // Borders
                frmMidiLyrics.FrameType = FrameType;
                
                // Text colors                
                frmMidiLyrics.BgColor = BgColor;
                frmMidiLyrics.Grad0Color = Grad0Color;
                frmMidiLyrics.Grad1Color = Grad1Color;
                frmMidiLyrics.Rhythm0Color = Rhythm0Color;
                frmMidiLyrics.Rhythm1Color = Rhythm1Color;


                frmMidiLyrics.ActiveColor = ActiveColor;
                frmMidiLyrics.HighlightColor = HighlightColor;
                frmMidiLyrics.InactiveColor = InactiveColor;
                                                
                frmMidiLyrics.ActiveBorderColor = ActiveBorderColor;
                frmMidiLyrics.InactiveBorderColor = InactiveBorderColor;

                // Chords
                frmMidiLyrics.ChordNextColor = InactiveChordColor;
                frmMidiLyrics.ChordHighlightColor = HighlightChordColor;
                frmMidiLyrics.bShowChords = _bShowChords;                

                // force uppercase
                frmMidiLyrics.bForceUppercase = bForceUppercase;

                //Window lyrics TopMost
                frmMidiLyrics.bTopMost = _bTopMost;

                NbLines = Convert.ToInt32(UpDownNbLines.Value);
                frmMidiLyrics.nbLyricsLines = NbLines;

                frmMidiLyrics.SizeMode = SizeMode;

                // Diaporam, Backcolor ou transparent
                frmMidiLyrics.OptionBackground = bgOption;

                // Text display: Center, Top, Bottom
                frmMidiLyrics.OptionDisplay = OptionDisplay;

                frmMidiLyrics.bTextBackGround = chkTextBackground.Checked;

                // SlideShow frequency
                frmMidiLyrics.FreqSlideShow = freqSlideShow;

                // directory for slide show
                frmMidiLyrics.DirSlideShow = dirSlideShow;

                // Karaoke display type
                frmMidiLyrics.KaraokeDisplayType = KaraokeDisplayType;
            }
         }

        /// <summary>
        /// Cancel changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion buttons
     

        #region form load close

       
        /// <summary>
        /// Form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLyrOptions_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            cboColor.DisplayKnownColors(cbGrad0);
            cboColor.DisplayKnownColors(cbGrad1);
            cboColor.DisplayKnownColors(cbRhythm0);
            cboColor.DisplayKnownColors(cbRhythm1);

            // Select the selected color in the ComboBox
            cbGrad0.SelectedIndex = cbGrad0.Items.IndexOf(Grad0Color);
            cbGrad1.SelectedIndex = cbGrad1.Items.IndexOf(Grad1Color);
            cbRhythm0.SelectedIndex = cbRhythm0.Items.IndexOf(Rhythm0Color);
            cbRhythm1.SelectedIndex = cbRhythm1.Items.IndexOf(Rhythm1Color);

        }

        /// <summary>
        /// Form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLyrOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            pBox.Terminate();

            // Active le formulaire frmMidiPlayer
            if (Application.OpenForms.OfType<frmMidiPlayer>().Count() > 0)
            {
                // Restore form
                if (Application.OpenForms["frmMidiPlayer"].WindowState != FormWindowState.Minimized)
                {                    
                    Application.OpenForms["frmMidiPlayer"].Restore();
                    Application.OpenForms["frmMidiPlayer"].Activate();
                }
            }

            // Active le formulaire frmMidiLyrics
            if (Application.OpenForms.OfType<frmMidiLyrics>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmMidiLyrics"].Restore();
                Application.OpenForms["frmMidiLyrics"].Activate();
            }

            Dispose();
        }



        #endregion form load close
    

        #region Background selection

        /// <summary>
        /// Background selection:
        /// 1 - Diaporama
        /// 2 - Solid Color
        /// 3 - Transparent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioDiaporama_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDiaporama.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                pBox.OptionBackground = "Diaporama";
                bgOption = "Diaporama";
                pBox.DirSlideShow = dirSlideShow;
            }
        }

        private void RadioSolidColor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSolidColor.Checked)
            {
                btnBgColor.Visible = true;
                btnBgColorPicker.Visible = true;
                picBgColor.Visible = true;
                txtBgColor.Visible = true;
                

                pBox.OptionBackground = "SolidColor";
                bgOption = "SolidColor";
            }
        }

        private void radioGradient_CheckedChanged(object sender, EventArgs e)
        {
            if(radioGradient.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                pBox.OptionBackground = "Gradient";
                bgOption = "Gradient";
            }
        }

            
        private void radioRhythm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRhythm.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                pBox.OptionBackground = "Rhythm";
                bgOption = "Rhythm";
            }
        }

        private void RadioTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTransparent.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                pBox.OptionBackground = "Transparent";
                bgOption = "Transparent";
            }
        }


        #endregion Background selection


        #region events

        /// <summary>
        /// Textbox only accept numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSlideShowFreq_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }           
        }       

        private void TxtSlideShowFreq_TextChanged(object sender, EventArgs e)
        {
            string f = txtSlideShowFreq.Text;
            f = f.Trim();
            if (f != "" && IsNumeric(f))
            {
                try
                {
                    int freq = Convert.ToInt32(f);

                    freqSlideShow = freq;
                    pBox.FreqDirSlideShow = freqSlideShow;                    
                }
                catch (Exception eee)
                {
                    Console.Write(eee.Message);
                }

            }
        }

        private void CbSizeMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = cbSizeMode.Text;

            switch (sel)
            {
                case "Normal":
                    SizeMode = PictureBoxSizeMode.Normal;
                    break;
                case "StretchImage":
                    SizeMode = PictureBoxSizeMode.StretchImage;
                    break;
                case "AutoSize":
                    SizeMode = PictureBoxSizeMode.AutoSize;
                    break;
                case "CenterImage":
                    SizeMode = PictureBoxSizeMode.CenterImage;
                    break;
                case "Zoom":
                    SizeMode = PictureBoxSizeMode.Zoom;
                    break;

            }
            pBox.SizeMode = SizeMode;


        }

        /// <summary>
        /// Window lyrics always on top
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            _bTopMost = chkTopMost.Checked;
        }

        /// <summary>
        /// Number of Karaoke lines to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDownNbLines_ValueChanged(object sender, EventArgs e)
        {
            NbLines = (int)UpDownNbLines.Value;
            pBox.nbLyricsLines = NbLines;
        }

        private void TxtSlideShow_TextChanged(object sender, EventArgs e)
        {           
            string tx = txtSlideShow.Text;
            tx = tx.Trim();
            dirSlideShow = tx;
            pBox.DirSlideShow = dirSlideShow;
        }

        private bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        private void chkDisplayBalls_CheckedChanged(object sender, EventArgs e)
        {
            Karaclass.m_DisplayBalls = chkDisplayBalls.Checked;
            pnlBalls.Visible = chkDisplayBalls.Checked;


        }

        private void cbOptionsTextDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbOptionsTextDisplay.SelectedIndex) {
                case 0:
                    OptionDisplay = Karaclass.OptionsDisplay.Top;
                    pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Top;
                    break;
                case 1:
                    OptionDisplay = Karaclass.OptionsDisplay.Center;
                    pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Center;
                    break;
                case 2:
                    OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                    pBox.OptionDisplay = PicControl.pictureBoxControl.OptionsDisplay.Bottom;
                    break;
                }
        }

        private void chkTextBackground_CheckedChanged(object sender, EventArgs e)
        {            
            pBox.bTextBackGround = chkTextBackground.Checked;
        }
              

        /// <summary>
        /// Forece Uppercase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTextUppercase_CheckedChanged(object sender, EventArgs e)
        {
            bForceUppercase = chkTextUppercase.Checked;
            pBox.bforceUppercase = bForceUppercase;
            Karaclass.m_ForceUppercase = bForceUppercase;
        }

        /// <summary>
        /// Karaoke display type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbKaraokeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbKaraokeType.SelectedValue.GetType() == typeof(string))
                {
                    KaraokeDisplayType = cbKaraokeType.SelectedValue.ToString();
                }
                else
                {
                    KaraokeDisplayType = ((KeyValuePair<string, string>)cbKaraokeType.SelectedValue).Key.ToString();

                }

                switch (KaraokeDisplayType)
                {
                    case "FixedLines":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FixedLines;
                        break;
                    case "ScrollingLinesBottomUp":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.ScrollingLinesBottomUp;
                        break;
                    case "ScrollingLinesTopDown":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.ScrollingLinesTopDown;
                        break;
                    case "TwoLinesSwapped":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.TwoLinesSwapped;
                        break;
                    case "FourLinesSwapped":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped;
                        break;

                    default:
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FixedLines;
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        #endregion events


        #region chords
        private void chkForceShowChords_CheckedChanged(object sender, EventArgs e)
        {
            _bShowChords = chkForceShowChords.Checked;
            pBox.bShowChords = _bShowChords;

        }

        private void btnChordNormalColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picInactiveChordColor, txtInactiveChordColor);
            InactiveChordColor = clr;
            ApplyNewColors();
        }

        private void btnChordHighlightColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picHighlightChordColor, txtHighlightChordColor);
            HighlightChordColor = clr;
            ApplyNewColors();
        }

        #endregion chords


        #region gradient
        private void cbGrad0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Color0 of the gradient panel
            if (cbGrad0.SelectedItem is Color selectedColor)
            {                               
                Grad0Color = selectedColor; // Update the Grad0Color variable
                pBox.Grad0Color = Grad0Color; // Update the gradient panel color
            }
        }

        private void cbGrad1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Grad1Color
            if (cbGrad1.SelectedItem is Color selectedColor)
            {                
                Grad1Color = selectedColor; // Update the Grad1Color variable
                pBox.Grad1Color = Grad1Color; // Update the gradient panel color
            }
        }

        private void cbRhythm0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm0Color
            if (cbRhythm0.SelectedItem is Color selectedColor)
            {
                Rhythm0Color = selectedColor; // Update the Rhythm0Color variable
                pBox.Rhythm0Color = Rhythm0Color; // Update the gradient panel color
            }
        }

        private void cbRhythm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm1Color of the gradient panel
            if (cbRhythm1.SelectedItem is Color selectedColor)
            {
                Rhythm1Color = selectedColor; // Update the Rhythm1Color variable
                pBox.Rhythm1Color = Rhythm1Color; // Update the gradient panel color
            }
        }

        #endregion gradient


        #region FrameType
        private void cbFrameType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // "NoBorder":
            // "FrameThin":
            // "Frame1":
            // "Frame2":
            // "Frame3":
            // "Frame4":
            // "Frame5":
            // "Shadow":
            // "Neon":

            FrameType = ((KeyValuePair<string, string>)cbFrameType.SelectedItem).Key;
            pBox.FrameType = FrameType;
        }

        #endregion FrameType


        #region font

        private void cbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ftName = cbFontName.SelectedItem.ToString();

            _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular);
            pBox.KaraokeFont = _karaokeFont;
        }

        #endregion font


        #region Lyrics decoration 

        #region text events

        private void txtBgColor_TextChanged(object sender, EventArgs e)
        {
            BgColor = Parse(txtBgColor.Text);
            ApplyNewColors();
        }

        private void txtActiveColor_TextChanged(object sender, EventArgs e)
        {
            ActiveColor = Parse(txtActiveColor.Text);
            ApplyNewColors();
        }

        private void txtHighlightColor_TextChanged(object sender, EventArgs e)
        {
            HighlightColor = Parse(txtHighlightColor.Text);
            ApplyNewColors();
        }

        private void txtInactiveColor_TextChanged(object sender, EventArgs e)
        {
            InactiveColor = Parse(txtInactiveColor.Text);
            ApplyNewColors();
        }

        private void txtActiveBorderColor_TextChanged(object sender, EventArgs e)
        {
            ActiveBorderColor = Parse(txtActiveBorderColor.Text);
            ApplyNewColors();
        }

        private void txtInactiveBorderColor_TextChanged(object sender, EventArgs e)
        {
            InactiveBorderColor = Parse(txtInactiveBorderColor.Text);
            ApplyNewColors();
        }

        private void txtInactiveChordColor_TextChanged(object sender, EventArgs e)
        {
            InactiveChordColor = Parse(txtInactiveChordColor.Text);
            ApplyNewColors();
        }

        private void txtHighlightChordColor_TextChanged(object sender, EventArgs e)
        {            
            HighlightChordColor = Parse(txtHighlightChordColor.Text); ;
            ApplyNewColors();
        }

        #endregion text events


        #region select color with button

        /// <summary>
        /// Backcolor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBgColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picBgColor, txtBgColor);
            BgColor = clr;
            ApplyNewColors();
        }


        /// <summary>
        /// Text color: before
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>      
        private void btnActiveColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picActiveColor, txtActiveColor);
            ActiveColor = clr;
            ApplyNewColors();
        }

        /// <summary>
        /// Text color: highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>       
        private void btnHighlightColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picHighlightColor, txtHighlightColor);
            HighlightColor = clr;
            ApplyNewColors();            
        }

        /// <summary>
        /// Text color: after
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInactiveColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picInactiveColor, txtInactiveColor);
            InactiveColor = clr;
            ApplyNewColors();
        }

        /// <summary>
        /// Text color: contour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnActiveBorderColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picActiveBorderColor, txtActiveBorderColor);
            ActiveBorderColor = clr;
            ApplyNewColors();
        }

        private void btnInactiveBorderColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picInactiveBorderColor, txtInactiveBorderColor);
            InactiveBorderColor = clr;
            ApplyNewColors();
        }

        #endregion select color with button


        #region select color with picker

        private void btnActiveColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtActiveColor);
        }

        private void btnHighlightColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtHighlightColor);
        }

        private void btnInactiveColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtInactiveColor);
        }

        private void btnActiveColorBorderPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtActiveBorderColor);
        }

        private void btnInactiveBoderColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtInactiveBorderColor);
        }

        private void btnInactiveChordColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtInactiveChordColor);
        }

        private void btnHighlightChordColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtHighlightChordColor);
        }

        private void btnBgColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtBgColor);
        }


        #endregion select color with picker


        #endregion Lyrics decoration


        #region functions

        /// <summary>
        /// Check text representing a color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color Parse(string input)
        {
            input = input.Trim();
            string strColorRegex = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            Regex re = new Regex(strColorRegex);
            if (re.IsMatch(input))
            {
                return ColorTranslator.FromHtml(input);
            }

            Color named = Color.FromName(input);
            if (named.IsKnownColor || named.IsNamedColor)
            {
                return named;
            }
            throw new ArgumentException($"Unsupported color value: {input}", nameof(input));
        }

        /// <summary>
        /// Select a color with the ColorDialog box and update colors for picBox and textBox
        /// </summary>
        /// <param name="picBox"></param>
        /// <param name="textBox"></param>
        private Color SelectColorFromButton(PictureBox picBox, TextBox textBox)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.FullOpen = true;
            dlg.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dlg.Color = picBox.BackColor;

            if (dlg.ShowDialog() != DialogResult.OK) return picBox.BackColor;

            picBox.BackColor = dlg.Color;
            textBox.Text = ToHex(dlg.Color);

            return dlg.Color;

        }

        /// <summary>
        /// Translate color to hexa
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static String ToHex(System.Drawing.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        private void SelectColorFromPicker(TextBox textBox)
        {
            this.Hide();
            Karaboss.Kfn.frmFullScreen frmFullScreen = new Karaboss.Kfn.frmFullScreen(textBox);
            frmFullScreen.Show();
        }
      
        public void GetColorFromPicker(Color c, TextBox txb)
        {
            txb.Text = ToHex(c);            
            this.Show();
        }



        #endregion functions

       
    }
}
