using GradientApp;
using Karaboss.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Mp3
{
    public partial class frmMp3LyrOptions : Form
    {
        keffect.KaraokeLine mp3KaraokeLine = new keffect.KaraokeLine();
        keffect.KaraokeLyrics mp3KaraokeLyrics = new keffect.KaraokeLyrics();

        #region private declarations

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
       

        // Lyrics TopMost
        private bool _bTopMost = false;

        // Force Uppercase
        private bool bForceUppercase = false;

        private bool bProgressiveHighlight = false;

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
                karaokeEffect1.SizeMode = _sizeMode;
            }
        }

        // Number of lines to display
        private int _nbLyricsLines;



        private frmMp3Lyrics frmMp3Lyrics;
        
        #endregion private declarations


        /// <summary>
        /// Constructor
        /// </summary>
        public frmMp3LyrOptions()
        {
            InitializeComponent();

            TopMost = true;

            LoadDefaultOptions();

            LoadOptions();
            SetOptions();
        }


        #region option form settings

        private void LoadDefaultOptions()
        {
            // Nb lines to display
            karaokeEffect1.nbLyricsLines = Properties.Settings.Default.TxtNbLines;


            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("Lorem", 0));
            mp3KaraokeLine.Add(new keffect.Syllable(" ipsum", 500));
            mp3KaraokeLine.Add(new keffect.Syllable(" dolor", 1000));
            mp3KaraokeLine.Add(new keffect.Syllable(" sit", 1500));
            mp3KaraokeLine.Add(new keffect.Syllable(" amet", 2000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("consectetur", 2500));
            mp3KaraokeLine.Add(new keffect.Syllable(" adipisicing", 3000));
            mp3KaraokeLine.Add(new keffect.Syllable(" elit", 3500));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("sed", 4000));
            mp3KaraokeLine.Add(new keffect.Syllable(" do", 4500));
            mp3KaraokeLine.Add(new keffect.Syllable(" eiusmod", 5000));
            mp3KaraokeLine.Add(new keffect.Syllable(" tempor", 5500));
            mp3KaraokeLine.Add(new keffect.Syllable(" incididunt", 6000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("ut", 6500));
            mp3KaraokeLine.Add(new keffect.Syllable(" labore", 7000));
            mp3KaraokeLine.Add(new keffect.Syllable("et", 7500));
            mp3KaraokeLine.Add(new keffect.Syllable(" dolore", 8000));
            mp3KaraokeLine.Add(new keffect.Syllable(" magna", 8500));
            mp3KaraokeLine.Add(new keffect.Syllable(" aliqua.", 9000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("Ut", 9200));
            mp3KaraokeLine.Add(new keffect.Syllable(" enim", 9500));
            mp3KaraokeLine.Add(new keffect.Syllable(" ad", 10000));
            mp3KaraokeLine.Add(new keffect.Syllable(" minim", 10500));
            mp3KaraokeLine.Add(new keffect.Syllable(" veniam", 11000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("quis", 11500));
            mp3KaraokeLine.Add(new keffect.Syllable(" nostrud", 12000));
            mp3KaraokeLine.Add(new keffect.Syllable(" exercitation", 12500));
            mp3KaraokeLine.Add(new keffect.Syllable(" ullamco", 13000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("laboris", 14000));
            mp3KaraokeLine.Add(new keffect.Syllable(" nisi", 14500));
            mp3KaraokeLine.Add(new keffect.Syllable(" ut", 15000));
            mp3KaraokeLine.Add(new keffect.Syllable(" aliquip", 15500));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("ex", 16000));
            mp3KaraokeLine.Add(new keffect.Syllable(" ea", 16500));
            mp3KaraokeLine.Add(new keffect.Syllable(" commodo", 17000));
            mp3KaraokeLine.Add(new keffect.Syllable(" consequat.", 17500));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("Duis", 18000));
            mp3KaraokeLine.Add(new keffect.Syllable(" aute", 18500));
            mp3KaraokeLine.Add(new keffect.Syllable(" irure", 19000));
            mp3KaraokeLine.Add(new keffect.Syllable(" dolor", 19500));
            mp3KaraokeLine.Add(new keffect.Syllable(" in", 20000));
            mp3KaraokeLine.Add(new keffect.Syllable(" reprehenderit", 20500));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("in", 20600));
            mp3KaraokeLine.Add(new keffect.Syllable("voluptate", 21000));
            mp3KaraokeLine.Add(new keffect.Syllable(" velit", 21500));
            mp3KaraokeLine.Add(new keffect.Syllable(" esse", 22000));
            mp3KaraokeLine.Add(new keffect.Syllable(" cillum", 22500));
            mp3KaraokeLine.Add(new keffect.Syllable(" dolore", 23000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            mp3KaraokeLine = new keffect.KaraokeLine();
            mp3KaraokeLine.Add(new keffect.Syllable("eu", 23500));
            mp3KaraokeLine.Add(new keffect.Syllable(" fugiat", 24000));
            mp3KaraokeLine.Add(new keffect.Syllable(" nulla", 24500));
            mp3KaraokeLine.Add(new keffect.Syllable(" pariatur.", 25000));
            mp3KaraokeLyrics.Add(mp3KaraokeLine);

            karaokeEffect1.mp3KaraokeLyrics = mp3KaraokeLyrics;

         
           

            // Needed to put exactly these 4 positions in order to have "Lorem ipsum" in red and "dolor" in green
            // I don't even understand how my creation works            
            karaokeEffect1.TransitionEffect = keffect.KaraokeEffect.TransitionEffects.None;

            karaokeEffect1.SetPos(10);   // index, _line, _lastline put to 0
            karaokeEffect1.SetPos(510);  // after Lorem
            karaokeEffect1.SetPos(1010); // after ipsum
            karaokeEffect1.SetPos(1510); // after dolor

        }


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
                karaokeEffect1.KaraokeFont = _karaokeFont;


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

                // textBox => pic 
                picBgColor.BackColor = Parse(txtBgColor.Text);

                picActiveColor.BackColor = Parse(txtActiveColor.Text);
                picHighlightColor.BackColor = Parse(txtHighlightColor.Text);
                picInactiveColor.BackColor = Parse(txtInactiveColor.Text);

                picActiveBorderColor.BackColor = Parse(txtActiveBorderColor.Text);
                picInactiveBorderColor.BackColor = Parse(txtInactiveBorderColor.Text);

                // pic => variables
                BgColor = picBgColor.BackColor;

                ActiveColor = picActiveColor.BackColor;
                HighlightColor = picHighlightColor.BackColor;
                InactiveColor = picInactiveColor.BackColor;

                ActiveBorderColor = picActiveBorderColor.BackColor;
                InactiveBorderColor = picInactiveBorderColor.BackColor;

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
                        karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Top;
                        break;
                    case "Center":
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        cbOptionsTextDisplay.SelectedIndex = 1;
                        karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Center;
                        break;
                    case "Bottom":
                        OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                        cbOptionsTextDisplay.SelectedIndex = 2;
                        karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Bottom;
                        break;
                    default:
                        OptionDisplay = Karaclass.OptionsDisplay.Center;
                        cbOptionsTextDisplay.SelectedIndex = 1;
                        karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Center;
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
                _nbLyricsLines = Properties.Settings.Default.TxtNbLines;


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
                _nbLyricsLines = 3;
               
                dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName) + "\\slideshow";

                freqSlideShow = 10;
                SizeMode = PictureBoxSizeMode.Zoom;
            }
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

            /*
            //List<string> lstBorders = new List<string>() { "Aucune bordure", "Fine bordure", "Bordure 1 pixel", "Bordure 2 pixel", "Bordure 3 pixel", "Bordure 4 pixel", "Bordure 5 pixel", "Ombré", "Neon" };
            List<string> lstBorders = new List<string>() { "NoBorder", "FrameThin", "Frame1", "Frame2", "Frame3", "Frame4", "Frame5", "Shadow", "Neon" };
            //List<string> lstBorders = new List<string>() { "No border", "Thin border", "1 - pixel border", "2 - pixel border", "3 - pixel border", "4 - pixel border", "5 - pixel border", "Shaded", "Neon" };
            cbFrame.DataSource = lstBorders;
            */

            cbFrameType.DataSource = new BindingSource(Frames, null);
            cbFrameType.ValueMember = "Key";
            cbFrameType.DisplayMember = "Value";

            if (cbFrameType.Items.Count > 2)
                cbFrameType.SelectedIndex = 2; // 1 pixel
        }

        /// <summary>
        /// Save options in properties
        /// </summary>
        private void SaveOptions ()
        {
            try
            {
                // Display balls
                Properties.Settings.Default.DisplayBalls = Karaclass.m_DisplayBalls;

                // Background type (Diaporama, Solidcolor, Transparent
                Properties.Settings.Default.BackGroundOption = bgOption;

                // Font                
                Properties.Settings.Default.KaraokeFontName = ftName;

                // Background color
                Properties.Settings.Default.BgColor = ToHex(BgColor);
                Properties.Settings.Default.Grad0Color = Grad0Color;
                Properties.Settings.Default.Grad1Color = Grad1Color;
                Properties.Settings.Default.Rhythm0Color = Rhythm0Color;
                Properties.Settings.Default.Rhythm1Color = Rhythm1Color;

                Properties.Settings.Default.ActiveColor = ToHex(ActiveColor);                
                Properties.Settings.Default.HighlightColor = ToHex(HighlightColor);
                Properties.Settings.Default.InactiveColor = ToHex(InactiveColor);

                Properties.Settings.Default.bProgressiveHighlight = bProgressiveHighlight;  // Progressive highlight                
               
                // Contour                
                Properties.Settings.Default.ActiveBorderColor = ToHex(ActiveBorderColor);
                Properties.Settings.Default.InactiveBorderColor = ToHex(InactiveBorderColor);

                // FrameType
                Properties.Settings.Default.FrameType = FrameType;

                // window lyrics topmost
                Properties.Settings.Default.frmMp3LyricsTopMost = _bTopMost;

                // Force Uppercase
                Properties.Settings.Default.bForceUppercase = bForceUppercase;

                // Number of lines to display
                Properties.Settings.Default.TxtNbLines = _nbLyricsLines;

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
                UpDownNbLines.Value = _nbLyricsLines;

                // Slideshow
                txtSlideShow.Text = dirSlideShow;
                txtSlideShowFreq.Text = freqSlideShow.ToString();

                // buttons
                picBgColor.BackColor = BgColor;

                picActiveColor.BackColor = ActiveColor;
                picHighlightColor.BackColor = HighlightColor;
                picInactiveColor.BackColor = InactiveColor;

                picActiveBorderColor.BackColor = ActiveBorderColor;
                picInactiveBorderColor.BackColor = InactiveBorderColor;

                // Window Lyrics TopMost
                chkTopMost.Checked = _bTopMost;

                // Force uppercase
                chkTextUppercase.Checked = bForceUppercase;
                karaokeEffect1.bforceUppercase = bForceUppercase;

                // Progressive highlight
                chkHighLightProgressive.Checked = bProgressiveHighlight;
                karaokeEffect1.TransitionEffect = bProgressiveHighlight ? keffect.KaraokeEffect.TransitionEffects.Progressive : keffect.KaraokeEffect.TransitionEffects.None;


                // picturebox            
                karaokeEffect1.FreqDirSlideShow = freqSlideShow;
                karaokeEffect1.nbLyricsLines = _nbLyricsLines;

                // Backgrounds
                karaokeEffect1.BgColor = BgColor;
                karaokeEffect1.Grad0Color = Grad0Color;
                karaokeEffect1.Grad1Color = Grad1Color;
                karaokeEffect1.Rhythm0Color = Rhythm0Color;
                karaokeEffect1.Rhythm1Color = Rhythm1Color;

                karaokeEffect1.ActiveBorderColor = ActiveBorderColor;
                karaokeEffect1.InactiveBorderColor = InactiveBorderColor;

                karaokeEffect1.ActiveColor = ActiveColor;
                karaokeEffect1.HighlightColor = HighlightColor;
                karaokeEffect1.InactiveColor = InactiveColor;

                // Frame type
                karaokeEffect1.FrameType = FrameType;                                       

                cbSizeMode.SelectedText = SizeMode.ToString();
                
                karaokeEffect1.OptionBackground = bgOption;
                
                karaokeEffect1.SetBackground(dirSlideShow);

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
            karaokeEffect1.BgColor = BgColor;
            karaokeEffect1.Grad0Color = Grad0Color;
            karaokeEffect1.Grad1Color = Grad1Color;
            karaokeEffect1.Rhythm0Color = Rhythm0Color;
            karaokeEffect1.Rhythm1Color = Rhythm1Color;

            karaokeEffect1.ActiveColor = ActiveColor;
            karaokeEffect1.HighlightColor = HighlightColor;
            karaokeEffect1.InactiveColor = InactiveColor;

            karaokeEffect1.ActiveBorderColor = ActiveBorderColor;
            karaokeEffect1.InactiveBorderColor = InactiveBorderColor;                              
            
            karaokeEffect1.OptionDisplay = (keffect.KaraokeEffect.OptionsDisplay)OptionDisplay;
            
            //Color of buttons
            picBgColor.BackColor = BgColor;

            picActiveColor.BackColor = ActiveColor;
            picInactiveColor.BackColor = InactiveColor;
            picHighlightColor.BackColor = HighlightColor;

            picActiveBorderColor.BackColor = ActiveBorderColor;
            picInactiveBorderColor.BackColor = InactiveBorderColor;

        }


        #endregion option form settings



        #region buttons

        private void btnDirSlideShow_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = dirSlideShow;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                // Force option display to Diaporama when changing the directory for slideshow
                radioDiaporama.Checked = true;

                dirSlideShow = folderBrowserDialog1.SelectedPath;

                Cursor.Current = Cursors.WaitCursor;
                txtSlideShow.Text = dirSlideShow;
            }
        }


        private void btnResetDir_Click(object sender, EventArgs e)
        {
            // Force option display to Diaporama when selecting a directory for slideshow
            radioDiaporama.Checked = true;

            dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            txtSlideShow.Text = dirSlideShow;
            karaokeEffect1.SetBackground(dirSlideShow);
        }


        /// <summary>
        /// Apply changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }


        /// <summary>
        /// Apply changes and exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            Close();
        }



        /// <summary>
        /// Apply changes to frmMp3Lyrics
        /// </summary>
        private void ApplyChanges()
        {
            SaveOptions();

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                frmMp3Lyrics = Utilities.FormUtilities.GetForm<frmMp3Lyrics>();

                frmMp3Lyrics.bShowBalls = Karaclass.m_DisplayBalls;

                frmMp3Lyrics.KaraokeFont = _karaokeFont;

                
                // Borders
                frmMp3Lyrics.FrameType = FrameType;

                // Text colors                
                frmMp3Lyrics.BgColor = BgColor;
                frmMp3Lyrics.Grad0Color = Grad0Color;
                frmMp3Lyrics.Grad1Color = Grad1Color;
                frmMp3Lyrics.Rhythm0Color = Rhythm0Color;
                frmMp3Lyrics.Rhythm1Color = Rhythm1Color;

                frmMp3Lyrics.ActiveColor = ActiveColor;
                frmMp3Lyrics.HighlightColor = HighlightColor;
                frmMp3Lyrics.InactiveColor = InactiveColor;
                
                frmMp3Lyrics.bProgressiveHighlight = bProgressiveHighlight;     // Progressive highlight
                
                frmMp3Lyrics.ActiveBorderColor = ActiveBorderColor;
                frmMp3Lyrics.InactiveBorderColor = InactiveBorderColor;
                               

                // force uppercase
                frmMp3Lyrics.bForceUppercase = bForceUppercase;

                _nbLyricsLines = Convert.ToInt32(UpDownNbLines.Value);
                frmMp3Lyrics.nbLyricsLines = _nbLyricsLines;

                frmMp3Lyrics.SizeMode = SizeMode;

                // Diaporam, Backcolor ou transparent
                frmMp3Lyrics.OptionBackground = bgOption;

                // Text display: Center, Top, Bottom
                frmMp3Lyrics.OptionDisplay = OptionDisplay;

                frmMp3Lyrics.bTextBackGround = chkTextBackground.Checked;

                // SlideShow frequency
                frmMp3Lyrics.FreqSlideShow = freqSlideShow;

                // directory for slide show
                frmMp3Lyrics.DirSlideShow = dirSlideShow;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        #endregion buttons


        #region form load close


        /// <summary>
        /// Form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMp3LyrOptions_Load(object sender, EventArgs e)
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
        private void frmMp3LyrOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            karaokeEffect1.Terminate();

            // Active le formulaire frmMp3Player
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                // Restore form
                if (Application.OpenForms["frmMp3Player"].WindowState != FormWindowState.Minimized)
                {
                    Application.OpenForms["frmMp3Player"].Restore();
                    Application.OpenForms["frmMp3Player"].Activate();
                }
            }

            // Active le formulaire frmMp3Lyrics
            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmMp3Lyrics"].Restore();
                Application.OpenForms["frmMp3Lyrics"].Activate();
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
        private void radioDiaporama_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDiaporama.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                karaokeEffect1.OptionBackground = "Diaporama";
                bgOption = "Diaporama";
                karaokeEffect1.SetBackground(dirSlideShow);
            }
        }

        private void radioSolidColor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSolidColor.Checked)
            {
                btnBgColor.Visible = true;
                btnBgColorPicker.Visible = true;
                picBgColor.Visible = true;
                txtBgColor.Visible = true;

                karaokeEffect1.OptionBackground = "SolidColor";
                bgOption = "SolidColor";
            }
        }

        private void radioGradient_CheckedChanged(object sender, EventArgs e)
        {
            if (radioGradient.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                karaokeEffect1.OptionBackground = "Gradient";
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

                karaokeEffect1.OptionBackground = "Rhythm";
                bgOption = "Rhythm";
            }
        }

        private void radioTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTransparent.Checked)
            {
                btnBgColor.Visible = false;
                btnBgColorPicker.Visible = false;
                picBgColor.Visible = false;
                txtBgColor.Visible = false;

                karaokeEffect1.OptionBackground = "Transparent";
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

        private void txtSlideShowFreq_TextChanged(object sender, EventArgs e)     
        {
            string f = txtSlideShowFreq.Text;
            f = f.Trim();
            if (f != "" && IsNumeric(f))
            {
                try
                {
                    int freq = Convert.ToInt32(f);

                    freqSlideShow = freq;
                    karaokeEffect1.FreqDirSlideShow = freqSlideShow;
                }
                catch (Exception eee)
                {
                    Console.Write(eee.Message);
                }
            }
        }


        private void cbSizeMode_SelectedIndexChanged(object sender, EventArgs e)
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
            karaokeEffect1.SizeMode = SizeMode;
        }


        private void chkTopMost_CheckedChanged(object sender, EventArgs e)
        {
            _bTopMost = chkTopMost.Checked;
        }


        private void UpDownNbLines_ValueChanged(object sender, EventArgs e)
        {
            _nbLyricsLines = (int)UpDownNbLines.Value;
            karaokeEffect1.nbLyricsLines = _nbLyricsLines;
        }


        private void txtSlideShow_TextChanged(object sender, EventArgs e)
        {            
            string tx = txtSlideShow.Text;
            tx = tx.Trim();
            dirSlideShow = tx;

            // Only if option Diaporama is selected
            if (radioDiaporama.Checked)
                karaokeEffect1.SetBackground(dirSlideShow);
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
            switch (cbOptionsTextDisplay.SelectedIndex)
            {
                case 0:
                    OptionDisplay = Karaclass.OptionsDisplay.Top;
                    karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Top;
                    break;
                case 1:
                    OptionDisplay = Karaclass.OptionsDisplay.Center;
                    karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Center;
                    break;
                case 2:
                    OptionDisplay = Karaclass.OptionsDisplay.Bottom;
                    karaokeEffect1.OptionDisplay = keffect.KaraokeEffect.OptionsDisplay.Bottom;
                    break;
            }
        }


        private void chkTextBackground_CheckedChanged(object sender, EventArgs e)
        {
            karaokeEffect1.bTextBackGround = chkTextBackground.Checked;
        }

        private void chkTextUppercase_CheckedChanged(object sender, EventArgs e)
        {
            bForceUppercase = chkTextUppercase.Checked;
            karaokeEffect1.bforceUppercase = bForceUppercase;
            Karaclass.m_ForceUppercase = bForceUppercase;
        }

          
        private void chkHighLightProgressive_CheckedChanged(object sender, EventArgs e)
        {
            bProgressiveHighlight = chkHighLightProgressive.Checked;
        }
                       
     

        #endregion events


        #region gradient
        private void cbGrad0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Color0 of the gradient panel
            if (cbGrad0.SelectedItem is Color selectedColor)
            {
                Grad0Color = selectedColor; // Update the Grad0Color variable
                karaokeEffect1.Grad0Color = Grad0Color; // Update the gradient panel color
            }
        }

        private void cbGrad1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Grad1Color
            if (cbGrad1.SelectedItem is Color selectedColor)
            {
                Grad1Color = selectedColor; // Update the Grad1Color variable
                karaokeEffect1.Grad1Color = Grad1Color; // Update the gradient panel color
            }
        }

        private void cbRhythm0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm0Color
            if (cbRhythm0.SelectedItem is Color selectedColor)
            {
                Rhythm0Color = selectedColor; // Update the Rhythm0Color variable
                karaokeEffect1.Rhythm0Color = Rhythm0Color; // Update the gradient panel color
            }
        }

        private void cbRhythm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm1Color of the gradient panel
            if (cbRhythm1.SelectedItem is Color selectedColor)
            {
                Rhythm1Color = selectedColor; // Update the Rhythm1Color variable
                karaokeEffect1.Rhythm1Color = Rhythm1Color; // Update the gradient panel color
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
            karaokeEffect1.FrameType = FrameType;

        }

        #endregion FrameType


        #region font
        private void cbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ftName = cbFontName.SelectedItem.ToString();

            _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular);
            karaokeEffect1.KaraokeFont = _karaokeFont;
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
