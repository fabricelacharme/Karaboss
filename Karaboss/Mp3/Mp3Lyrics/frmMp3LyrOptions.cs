using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Karaboss.Mp3.Mp3Lyrics;
using keffect;
using TagLib.Mpeg4;

namespace Karaboss.Mp3
{
    public partial class frmMp3LyrOptions : Form
    {
        List<keffect.KaraokeEffect.kSyncText> SyncLine = new List<keffect.KaraokeEffect.kSyncText>();
        List<List<keffect.KaraokeEffect.kSyncText>> SyncLyrics = new List<List<keffect.KaraokeEffect.kSyncText>>();

        #region options

        // Text color
        private Color TxtNextColor;
        // Text to sing color
        private Color TxtHighlightColor;
        // Text sung color
        private Color TxtBeforeColor;
        // Background color
        private Color TxtBackColor;

        private Karaclass.OptionsDisplay OptionDisplay;
        private string bgOption = "Diaporama";

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

        // Font
        private Font _karaokeFont;

        // Contour color
        private bool bColorContour = true;
        private Color TxtContourColor;

        // Force Uppercase
        private bool bForceUppercase = false;

        private frmMp3Lyrics frmMp3Lyrics;
        
        #endregion options

        public frmMp3LyrOptions()
        {
            InitializeComponent();
            LoadDefaultOptions();

            LoadOptions();
            SetOptions();
        }

        private void LoadDefaultOptions ()
        {
            // Nb lines to display
            karaokeEffect1.nbLyricsLines = Properties.Settings.Default.TxtNbLines;            

            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(0, "Lorem"), new keffect.KaraokeEffect.kSyncText(500, " ipsum"), new keffect.KaraokeEffect.kSyncText(1000, " dolor"), new keffect.KaraokeEffect.kSyncText(1500, " sit"), new keffect.KaraokeEffect.kSyncText(2000, " amet") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(2500, "consectetur"), new keffect.KaraokeEffect.kSyncText(3000, " adipisicing"), new keffect.KaraokeEffect.kSyncText(3500, " elit") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(4000, "sed"), new keffect.KaraokeEffect.kSyncText(4500, " do"), new keffect.KaraokeEffect.kSyncText(5000, " eiusmod"), new keffect.KaraokeEffect.kSyncText(5500, " tempor"), new keffect.KaraokeEffect.kSyncText(6000, " incididunt") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(6500, "ut"), new keffect.KaraokeEffect.kSyncText(7000, " labore"), new keffect.KaraokeEffect.kSyncText(7500, "et"), new keffect.KaraokeEffect.kSyncText(8000, " dolore"), new keffect.KaraokeEffect.kSyncText(8500, " magna"), new keffect.KaraokeEffect.kSyncText(9000, " aliqua.") } ;
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(9200, "Ut"), new keffect.KaraokeEffect.kSyncText(9500, " enim"), new keffect.KaraokeEffect.kSyncText(10000, " ad"), new keffect.KaraokeEffect.kSyncText(10500, " minim"), new keffect.KaraokeEffect.kSyncText(11000, " veniam") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(11500, "quis"), new keffect.KaraokeEffect.kSyncText(12000, " nostrud"), new keffect.KaraokeEffect.kSyncText(12500, " exercitation"), new keffect.KaraokeEffect.kSyncText(13000, " ullamco") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(14000, "laboris"), new keffect.KaraokeEffect.kSyncText(14500, " nisi"), new keffect.KaraokeEffect.kSyncText(15000, " ut"), new keffect.KaraokeEffect.kSyncText(15500, " aliquip") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(16000, "ex"), new keffect.KaraokeEffect.kSyncText(16500, " ea"), new keffect.KaraokeEffect.kSyncText(17000, " commodo"), new keffect.KaraokeEffect.kSyncText(17500, " consequat.") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(18000, "Duis"), new keffect.KaraokeEffect.kSyncText(18500, " aute"), new keffect.KaraokeEffect.kSyncText(19000, " irure"), new keffect.KaraokeEffect.kSyncText(19500, " dolor"), new keffect.KaraokeEffect.kSyncText(20000, " in"), new keffect.KaraokeEffect.kSyncText(20500, " reprehenderit") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(20600, "in"), new keffect.KaraokeEffect.kSyncText(21000, "voluptate"), new keffect.KaraokeEffect.kSyncText(21500, " velit"), new keffect.KaraokeEffect.kSyncText(22000, " esse"), new keffect.KaraokeEffect.kSyncText(22500, " cillum"), new keffect.KaraokeEffect.kSyncText(23000, " dolore") };
            SyncLyrics.Add(SyncLine);
            SyncLine = new List<keffect.KaraokeEffect.kSyncText> { new keffect.KaraokeEffect.kSyncText(23500, "eu"), new keffect.KaraokeEffect.kSyncText(24000, " fugiat"), new keffect.KaraokeEffect.kSyncText(24500, " nulla"), new keffect.KaraokeEffect.kSyncText(25000, " pariatur.") };
            SyncLyrics.Add(SyncLine);

            karaokeEffect1.SyncLyrics = SyncLyrics;

            // Needed to put exactly these 4 positions in order to have "Lorem ipsum" in red and "dolor" in green
            // I don't even understand how my creation works            
            karaokeEffect1.TransitionEffect = keffect.KaraokeEffect.TransitionEffects.None;
            
            karaokeEffect1.SetPos(10);   // index, _line, _lastline put to 0
            karaokeEffect1.SetPos(510);  // after Lorem
            karaokeEffect1.SetPos(1010); // after ipsum
            karaokeEffect1.SetPos(1510); // after dolor

        }

        private void UpDownNbLines_ValueChanged(object sender, EventArgs e)
        {
            _nbLyricsLines = (int)UpDownNbLines.Value;
            karaokeEffect1.nbLyricsLines = _nbLyricsLines;
        }


        #region apply changes
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

      
        /// <summary>
        /// Apply colors to option form
        /// </summary>
        private void ApplyNewColors()
        {
            // picturebox

            karaokeEffect1.TxtBackColor = TxtBackColor;

            karaokeEffect1.bColorContour = bColorContour;
            karaokeEffect1.TxtContourColor = TxtContourColor;
            karaokeEffect1.TxtNotYetPlayedColor = TxtNextColor;
            karaokeEffect1.TxtBeingPlayedColor = TxtHighlightColor;
            karaokeEffect1.TxtAlreadyPlayedColor = TxtBeforeColor;

            karaokeEffect1.OptionDisplay = (keffect.KaraokeEffect.OptionsDisplay)OptionDisplay;
           

            //Color of buttons
            pictBackColor.BackColor = TxtBackColor;
            pictBefore.BackColor = TxtBeforeColor;
            pictContour.BackColor = TxtContourColor;
            pictHighlight.BackColor = TxtHighlightColor;
            pictNext.BackColor = TxtNextColor;
        }


        private bool IsNumeric(string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        /// <summary>
        /// Load options stored in properties
        /// </summary>
        private void LoadOptions()
        {
            try
            {
                _karaokeFont = Properties.Settings.Default.KaraokeFont;
                karaokeEffect1.KaraokeFont = _karaokeFont;
                txtFont.Text = _karaokeFont.Name;

                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;

                TxtBackColor = Properties.Settings.Default.TxtBackColor;
                // Colors
                TxtNextColor = Properties.Settings.Default.TxtNextColor;
                TxtHighlightColor = Properties.Settings.Default.TxtHighlightColor;
                TxtBeforeColor = Properties.Settings.Default.TxtBeforeColor;

                bColorContour = Properties.Settings.Default.bColorContour;
                TxtContourColor = Properties.Settings.Default.TxtContourColor;
                chkContour.Checked = bColorContour;

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
                TxtBackColor = Color.White;
                TxtNextColor = Color.Black;
                TxtHighlightColor = Color.Red;
                TxtBeforeColor = Color.YellowGreen;
                TxtContourColor = Color.Black;
                _nbLyricsLines = 3;
                bColorContour = true;

                dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName) + "\\slideshow";

                freqSlideShow = 10;
                SizeMode = PictureBoxSizeMode.Zoom;
            }
        }


        /// <summary>
        /// Appply options to option form
        /// </summary>
        private void SetOptions()
        {
            try
           {                
                // Nombre de lignes à afficher
                UpDownNbLines.Value = _nbLyricsLines;

                // Slideshow
                txtSlideShow.Text = dirSlideShow;
                txtSlideShowFreq.Text = freqSlideShow.ToString();

                // buttons
                pictBackColor.BackColor = TxtBackColor;
                pictBefore.BackColor = TxtBeforeColor;

                chkContour.Checked = bColorContour;
                pictContour.BackColor = TxtContourColor;

                pictHighlight.BackColor = TxtHighlightColor;
                pictNext.BackColor = TxtNextColor;


                // Force uppercase
                chkTextUppercase.Checked = bForceUppercase;
                karaokeEffect1.bforceUppercase = bForceUppercase;

                // picturebox            
                karaokeEffect1.FreqDirSlideShow = freqSlideShow;
                karaokeEffect1.nbLyricsLines = _nbLyricsLines;                


                karaokeEffect1.TxtBackColor = TxtBackColor;

                karaokeEffect1.bColorContour = bColorContour;
                karaokeEffect1.TxtContourColor = TxtContourColor;

                karaokeEffect1.TxtNotYetPlayedColor = TxtNextColor; 
                karaokeEffect1.TxtBeingPlayedColor = TxtHighlightColor;
                karaokeEffect1.TxtAlreadyPlayedColor = TxtBeforeColor;


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
        /// Apply changes to frmMp3Lyrics
        /// </summary>
        private void ApplyChanges()
        {
            SaveOptions();

            if (Application.OpenForms.OfType<frmMp3Lyrics>().Count() > 0)
            {
                Cursor.Current = Cursors.WaitCursor;
                frmMp3Lyrics = Utilities.FormUtilities.GetForm<frmMp3Lyrics>();

                frmMp3Lyrics.KaraokeFont = _karaokeFont;

                // Text colors                
                frmMp3Lyrics.TxtBackColor = TxtBackColor;

                frmMp3Lyrics.TxtNextColor = TxtNextColor;
                frmMp3Lyrics.TxtHighlightColor = TxtHighlightColor;
                frmMp3Lyrics.TxtBeforeColor = TxtBeforeColor;

                frmMp3Lyrics.bColorContour = bColorContour;
                frmMp3Lyrics.TxtContourColor = TxtContourColor;
           
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

        private void SaveOptions ()
        {
            try
            {
               // Background type (Diaporama, Solidcolor, Transparent
                Properties.Settings.Default.BackGroundOption = bgOption;

                Properties.Settings.Default.KaraokeFont = _karaokeFont;

                Properties.Settings.Default.TxtBackColor = TxtBackColor;

                Properties.Settings.Default.TxtNextColor = TxtNextColor;
                Properties.Settings.Default.TxtHighlightColor = TxtHighlightColor;
                Properties.Settings.Default.TxtBeforeColor = TxtBeforeColor;

               
                // Contour
                Properties.Settings.Default.bColorContour = bColorContour;
                Properties.Settings.Default.TxtContourColor = TxtContourColor;

                // Force Uppercase
                Properties.Settings.Default.bForceUppercase = bForceUppercase;

                Properties.Settings.Default.TxtNbLines = _nbLyricsLines;

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

                Properties.Settings.Default.bLyricsBackGround = chkTextBackground.Checked;

                // Save all
                Properties.Settings.Default.Save();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion Apply changes


        #region select colors

        /// <summary>
        /// Dialog get color (+ custom colors 27/02/2025)
        /// </summary>
        /// <param name="defColor"></param>
        /// <returns></returns>
        private Color DlgGetColor(Color defColor)
        {
            ColorDialog MyDialog;

            // Custom color (BGR instead of RGB !!!!!)
            Int32 key = defColor.B << 16 | defColor.G << 8 | defColor.R;
            int[] bg_colors = { key };


            if (defColor.IsKnownColor)
            {
                MyDialog = new ColorDialog()
                {
                    AllowFullOpen = true,
                    ShowHelp = true,
                    Color = defColor,
                };
            }
            else
            {
                MyDialog = new ColorDialog()
                {
                    AllowFullOpen = true,
                    ShowHelp = true,
                    Color = defColor,
                    CustomColors = bg_colors,
                   
                };
            }


            if (MyDialog.ShowDialog() == DialogResult.OK)
                return MyDialog.Color;
            else
                return defColor;
        }        

        private void btnFonts_Click(object sender, EventArgs e)
        {
            try
            {
                // Show the dialog.
                fontDialog1.Font = _karaokeFont;

                if (fontDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtFont.Text = fontDialog1.Font.Name;
                    _karaokeFont = fontDialog1.Font;
                    karaokeEffect1.KaraokeFont = _karaokeFont;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _karaokeFont = new Font("Arial", this.Font.Size);
            }
        }

        private void btnSungColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtBeforeColor);
            if (clr == TxtBeforeColor)
                return;
            TxtBeforeColor = clr;
            pictBefore.BackColor = clr;
            ApplyNewColors();
        }

        private void btnSingColor_Click(object sender, EventArgs e)
        {

            Color clr = DlgGetColor(TxtHighlightColor);
            if (clr == TxtHighlightColor)
                return;
            TxtHighlightColor = clr;
            pictHighlight.BackColor = clr;
            ApplyNewColors();
        }

        private void btnForeColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtNextColor);
            if (clr == TxtNextColor)
                return;
            TxtNextColor = clr;
            pictNext.BackColor = clr;
            ApplyNewColors();
        }


        /// <summary>
        /// Set color of text contour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnContourColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtContourColor);
            if (clr == TxtContourColor)
                return;
            TxtContourColor = clr;
            pictContour.BackColor = clr;
            ApplyNewColors();
        }

        #endregion select colors


        private void chkContour_CheckedChanged(object sender, EventArgs e)
        {
            bColorContour = chkContour.Checked;
            ApplyNewColors();
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

        private void radioDiaporama_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDiaporama.Checked)
            {
                btnBackColor.Visible = false;
                pictBackColor.Visible = false;

                karaokeEffect1.OptionBackground = "Diaporama";
                bgOption = "Diaporama";
                karaokeEffect1.SetBackground(dirSlideShow);
            }
        }

        private void btnBackColor_Click(object sender, EventArgs e)
        {

            Color clr = DlgGetColor(TxtBackColor);
            if (clr == TxtBackColor)
                return;
            TxtBackColor = clr;
            pictBackColor.BackColor = clr;
            ApplyNewColors();
        }

        private void btnDirSlideShow_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = dirSlideShow;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                dirSlideShow = folderBrowserDialog1.SelectedPath;

                Cursor.Current = Cursors.WaitCursor;
                txtSlideShow.Text = dirSlideShow;
                karaokeEffect1.SetBackground(dirSlideShow);
            }
        }

        private void btnResetDir_Click(object sender, EventArgs e)
        {

            dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            txtSlideShow.Text = dirSlideShow;
            karaokeEffect1.SetBackground(dirSlideShow);
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

      
    }
}
