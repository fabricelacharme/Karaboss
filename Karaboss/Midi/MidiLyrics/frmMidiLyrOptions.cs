#region License

/* Copyright (c) 2026 Fabrice Lacharme
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
using Karaboss.Properties;
using Karaboss.Resources.Localization;
using Karaboss.Themes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmMidiLyrOptions : Form
    {

        #region Declarations


        #region Backgrounds SlideShow

        private string bgOption = "Image";

        // Single image as background
        private string SingleImagePath;

        //Slideshow
        private string dirSlideShow;
        // Frequency
        private int freqSlideShow;

        #endregion Backgrounds SlideShow


        #region Colors

        #region Chords color
        // Chord color
        private Color InactiveChordColor;
        private Color HighlightChordColor;

        #endregion Chords color

        // Background color
        private Color BgColor;

        #region Gradient colors

        private Color Grad0Color;
        private Color Grad1Color;
        private Color Rhythm0Color;
        private Color Rhythm1Color;

        #endregion Gradient colors


        #region Instrumental color

        private Color ActiveInstrumentalColor;

        #endregion Instrumental color


        #region Lyrics color

        // Text color
        private Color InactiveColor;
        // Text to sing color
        private Color HighlightColor;
        // Text sung color
        private Color ActiveColor;

        // Border color        
        private Color ActiveBorderColor;
        private Color InactiveBorderColor;

        #endregion Lyrics color

        #endregion Colors


        #region Draw filename

        private bool bShowSongName = true;

        #endregion Draw filename


        #region Form

        // Form TopMost
        private bool _bTopMost = false;

        // Display chords with lyrics
        private bool _bShowChords = false;

        #endregion Form


        #region Fonts

        private Font _karaokeFont;
        private string ftName = "Arial Black";
        private uint ftSize = 20;

        // Font stretching (Small (no stetching), Medium (some stretching), Large (most stretching)
        private string _FontStretching = "Large";
        public string FontStretching
        {
            get { return _FontStretching; }
            set
            {
                _FontStretching = value;                
            }
        }


        #endregion Fonts


        #region Instrumental

        // Show hints (introduction, instrumental, ending)
        private bool bShowHints = true;

        #endregion Instrumental


        #region Karaoke display Layout

        private Dictionary<string, string> KaraokeTypes = new Dictionary<string, string>();

        // Karaoke display Layout (FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped
        private string KaraokeDisplayType;

        #endregion Karaoke display Layout


        #region Picture

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

        private bool _bShowLogo;
        public bool bShowLogo
        {
            get { return _bShowLogo; }
            set
            {
                _bShowLogo = value;
                pBox.bShowLogo = _bShowLogo;
            }
        }
        #endregion Picture


        #region Text transform

        // Display Top, Center, Bottom
        private Karaclass.OptionsDisplay OptionDisplay;

        // Text decoration (No border, border 1px, 2px, .. Shadow, Neon ...) 
        private string FrameType;

        // Force Uppercase
        private bool bForceUppercase = false;

        // Number of lines to display
        private int _nbLyricsLines;

        #endregion Text transform                  


        #region Themes

        // Track changes
        bool bColorModified = false;

        private ThemesListHelper _ThemesListHelper = new ThemesListHelper();
        private ThemesList _ThemesList = new ThemesList();
        private ThemeItem _currentTheme = new ThemeItem();

        #endregion Themes

        #endregion Declarations


        /// <summary>
        /// Constructor
        /// </summary>
        public frmMidiLyrOptions()
        {
            InitializeComponent();
           
            TopMost = true;

            LoadOptions();     
            SetOptions();            

            pBox.bIsSettings = true;
            pBox.LoadDemoText();

            pBox.Populate(pBox.KLyrics, pBox.Duration, pBox.TotalTicks, pBox.BeatDuration, pBox.FirstMelodyNoteTicksOn, pBox.FileName, bForceUppercase, _bShowChords);

        }


        #region Background selection

        private void radioImage_CheckedChanged(object sender, EventArgs e)
        {
            txtImage.Visible = radioImage.Checked;
            btnSelectImage.Visible = radioImage.Checked;

            if (radioImage.Checked)
            {
                pBox.OptionBackground = "Image";
                bgOption = "Image";
            }
        }

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
                pBox.OptionBackground = "Diaporama";
                bgOption = "Diaporama";
                pBox.SetDirectoryBackground(dirSlideShow);
            }
        }

        private void RadioSolidColor_CheckedChanged(object sender, EventArgs e)
        {
            txtBgColor.Visible = radioSolidColor.Checked;
            picBgColor.Visible = radioSolidColor.Checked;
            btnBgColor.Visible = radioSolidColor.Checked;
            btnBgColorPicker.Visible = radioSolidColor.Checked;

            if (radioSolidColor.Checked)
            {
                pBox.OptionBackground = "SolidColor";
                bgOption = "SolidColor";
            }
        }

        private void radioGradient_CheckedChanged(object sender, EventArgs e)
        {
            if (radioGradient.Checked)
            {
                pBox.OptionBackground = "Gradient";
                bgOption = "Gradient";
            }
        }


        private void radioRhythm_CheckedChanged(object sender, EventArgs e)
        {
            if (radioRhythm.Checked)
            {
                pBox.OptionBackground = "Rhythm";
                bgOption = "Rhythm";
            }
        }

        private void RadioTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTransparent.Checked)
            {
                pBox.OptionBackground = "Transparent";
                bgOption = "Transparent";
            }
        }


        #endregion Background selection


        #region buttons

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...|All files (*.*)|*.*";
                openFileDialog.FileName = string.Empty;

                var AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);                              
                if (SingleImagePath != null && SingleImagePath.Trim() != string.Empty && System.IO.File.Exists(SingleImagePath))
                {
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(SingleImagePath);
                }
                else if (Directory.Exists(AppDataFolder))
                {
                    openFileDialog.InitialDirectory = AppDataFolder;
                }
                else
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    radioImage.Checked = true;
                    SingleImagePath = openFileDialog.FileName;
                    txtImage.Text = Path.GetFileName(SingleImagePath);
                    pBox.SingleImagePath = SingleImagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


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
            pBox.SetDirectoryBackground(dirSlideShow);
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


        #region Color Themes

        private void ThemeModified(bool bModified)
        {
            if (_currentTheme.Name == "Default")
            {
                bModified = false;
            }


            bColorModified = bModified;

            string Title = "Karaboss - Lyrics options";
            if (bModified)
            {
                this.Text = Title + " *";
            }
            else
            {
                this.Text = Title;
            }
        }


        private void PopulateThemes()
        {
            // Load all available color themes
            _ThemesList = LoadThemes();

            // Load default Theme name
            string currentThemeName = Properties.Settings.Default.Theme;

            ThemeItem thi = _ThemesList.GetThemeByName(currentThemeName);
            if (thi == null || thi.Name != currentThemeName)
                currentThemeName = "Default";

            foreach (ThemeItem theme in _ThemesList.Themes)
            {
                cbTheme.Items.Add(theme.Name);
            }
            cbTheme.SelectedItem = currentThemeName;
        }


        private void cbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region Save previous theme
            // the user has selected another theme in the combo,
            // but the previous theme was modified            
            if (bColorModified && _currentTheme.Name != "Default")
            {
                // Theme <{0}> has been modified, save changes?
                string tx = Strings.SaveThemeQuestion;

                switch (MessageBox.Show(string.Format(tx, _currentTheme.Name), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        // Save previous theme and continue
                        SaveTheme(_currentTheme.Name);
                        break;
                    case DialogResult.No:
                        // Do not save previous theme and continue
                        ThemeModified(false);
                        break;
                    case DialogResult.Cancel:
                        // Cancel select a new theme                        
                        return;
                }
            }
            #endregion Save previous theme

            _currentTheme = _ThemesList.GetThemeByName(cbTheme.Text);

            LoadColorsFromCurrentTheme();

            // Apply colors to combos gradient
            UpdateGradientCombosToTheme();

            ThemeModified(false);

        }

        private void UpdateGradientCombosToTheme()
        {
            // Select the gradient colors of the current theme in the ComboBox
            int argb;
            argb = Grad0Color.ToArgb();
            foreach (var item in cbGrad0.Items)
            {
                if (((Color)item).ToArgb() == argb)
                {
                    cbGrad0.SelectedItem = item;
                    break;
                }
            }
            argb = Grad1Color.ToArgb();
            foreach (var item in cbGrad1.Items)
            {
                if (((Color)item).ToArgb() == argb)
                {
                    cbGrad1.SelectedItem = item;
                    break;
                }
            }
            argb = Rhythm0Color.ToArgb();
            foreach (var item in cbRhythm0.Items)
            {
                if (((Color)item).ToArgb() == argb)
                {
                    cbRhythm0.SelectedItem = item;
                    break;
                }
            }
            argb = Rhythm1Color.ToArgb();
            foreach (var item in cbRhythm1.Items)
            {
                if (((Color)item).ToArgb() == argb)
                {
                    cbRhythm1.SelectedItem = item;
                    break;
                }
            }
        }


        private void LoadColorsFromCurrentTheme()
        {
            if (_currentTheme != null)
            {
                #region Set colors of current theme

                txtActiveColor.Text = _currentTheme.ActiveColor;
                txtHighlightColor.Text = _currentTheme.HighlightColor;
                txtInactiveColor.Text = _currentTheme.InactiveColor;


                // Background type (Diaporama, Solidcolor, Transparent)                
                Grad0Color = Parse(_currentTheme.Grad0Color);
                Grad1Color = Parse(_currentTheme.Grad1Color);
                Rhythm0Color = Parse(_currentTheme.Rhythm0Color);
                Rhythm1Color = Parse(_currentTheme.Rhythm1Color);

                // Colors: Properties => textBox                
                txtBgColor.Text = _currentTheme.BgColor;


                txtActiveBorderColor.Text = _currentTheme.ActiveBorderColor;
                txtInactiveBorderColor.Text = _currentTheme.InactiveBorderColor;

                txtInactiveChordColor.Text = _currentTheme.InactiveChordColor;
                txtHighlightChordColor.Text = _currentTheme.HighlightChordColor;

                // Instrumental color
                txtActiveInstrumentalColor.Text = _currentTheme.ActiveInstrumentalColor;

                #endregion Set colors of current theme
            }
            else
            {
                // Error: no color theme was found
                string tx = Strings.ErrorNoThemeFound;
                MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnSaveTheme_Click(object sender, EventArgs e)
        {
            ThemeItem item = _ThemesList.GetThemeByName(cbTheme.SelectedItem.ToString());
            SaveTheme(item.Name);
        }


        private void SaveTheme(string ThemeName)
        {
            bool bSaveTheme = false;
            string Name; // = string.Empty;

            ThemeItem item = _ThemesList.GetThemeByName(ThemeName);

            if (item == null) return;

            if (item.Name == "Default")
            {
                // If closing and selected theme is default, do not change

                // If Default was selected => create a new theme
                Name = GetNameForNewTheme();

                if (Name == null || Name.Length == 0) return;

                // Initialize the new theme with current values
                item = new ThemeItem()
                {
                    Name = Name,
                };

                item = UpdateThemeWithCurrentValues(item);

                _ThemesList.Add(item);
                cbTheme.Items.Add(item.Name);
                cbTheme.SelectedItem = item.Name;

                //bSaveTheme = true;
                ThemeModified(true);
            }
            else
            {
                // Item is not default.

                // 1. Check if changes
                if (item.ActiveColor != ToHex(ActiveColor) ||
                    item.HighlightColor != ToHex(HighlightColor) ||
                    item.InactiveColor != ToHex(InactiveColor) ||
                    item.ActiveBorderColor != ToHex(ActiveBorderColor) ||
                    item.InactiveBorderColor != ToHex(InactiveBorderColor) ||
                    item.ActiveInstrumentalColor != ToHex(ActiveInstrumentalColor) ||
                    item.BgColor != ToHex(BgColor) ||
                    item.Grad0Color != ToHex(Grad0Color) ||
                    item.Grad1Color != ToHex(Grad1Color) ||
                    item.Rhythm0Color != ToHex(Rhythm0Color) ||
                    item.Rhythm1Color != ToHex(Rhythm1Color) ||
                    item.InactiveChordColor != ToHex(InactiveChordColor) ||
                    item.HighlightChordColor != ToHex(HighlightChordColor))


                    bSaveTheme = true;

                // Update values of current Theme
                item = UpdateThemeWithCurrentValues(item);
            }

            if (!bSaveTheme)
            {
                ThemeModified(false);
                return;
            }

            // Save list of themes
            //if (_ThemesListHelper.Save(_ThemesListHelper.File, _ThemesList))
            if (SaveThemesList())
            {
                // The theme <{0}> was successfully saved
                string tx = Strings.ThemeSuccessfullySaved;
                MessageBox.Show(string.Format(tx, item.Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset changes                
                ThemeModified(false);
            }
        }

        private bool SaveThemesList()
        {
            try
            {
                _ThemesListHelper.Save(_ThemesListHelper.File, _ThemesList);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        private ThemeItem UpdateThemeWithCurrentValues(ThemeItem item)
        {
            // Update values of current Theme
            item.ActiveColor = ToHex(ActiveColor);
            item.HighlightColor = ToHex(HighlightColor);
            item.InactiveColor = ToHex(InactiveColor);
            item.ActiveBorderColor = ToHex(ActiveBorderColor);
            item.InactiveBorderColor = ToHex(InactiveBorderColor);
            item.ActiveInstrumentalColor = ToHex(ActiveInstrumentalColor);
            item.BgColor = ToHex(BgColor);
            item.Grad0Color = ToHex(Grad0Color);
            item.Grad1Color = ToHex(Grad1Color);
            item.Rhythm0Color = ToHex(Rhythm0Color);
            item.Rhythm1Color = ToHex(Rhythm1Color);
            item.InactiveChordColor = ToHex(InactiveChordColor);
            item.HighlightChordColor = ToHex(HighlightChordColor);

            return item;
        }


        private string GetNameForNewTheme()
        {
            // open a dialog asking for a name            

            // Enter a name for the new theme
            string tx = Strings.NameForNewTheme;

            this.TopMost = false;
            frmDialog frmDialog = new frmDialog(tx);

            if (frmDialog.ShowDialog() != DialogResult.OK)
            {
                this.TopMost = true;
                return null;
            }
            this.TopMost = true;
            string Name = frmDialog.Response.Trim();

            if (_ThemesList.GetThemeByName(Name) != null)
            {
                // Error: the theme <{0}> already exists!
                tx = Strings.ErrorThemeAlreadyExists;

                MessageBox.Show(string.Format(tx, Name), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            return Name;

        }

        // Create a new theme
        private void btnNewTheme_Click(object sender, EventArgs e)
        {

            #region Save previous theme
            // the user has selected another theme in the combo,
            // but the previous theme was modified            
            if (bColorModified && _currentTheme.Name != "Default")
            {
                // Theme <{0}> has been modified, save changes?
                string tx = Strings.SaveThemeQuestion;

                switch (MessageBox.Show(string.Format(tx, _currentTheme.Name), Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        // Save previous theme and continue
                        SaveTheme(_currentTheme.Name);
                        break;
                    case DialogResult.No:
                        // Do not save previous theme and continue
                        ThemeModified(false);
                        break;
                    case DialogResult.Cancel:
                        // Cancel select a new theme                        
                        return;
                }
            }
            #endregion Save previous theme


            string Name = GetNameForNewTheme();
            if (Name == null || Name.Length == 0) return;

            // Initialize with current values
            ThemeItem item = new ThemeItem()
            {
                Name = Name,
            };

            item = UpdateThemeWithCurrentValues(item);

            _ThemesList.Add(item);
            cbTheme.Items.Add(item.Name);
            cbTheme.SelectedItem = item.Name;
        }

        private void btnDeleteTheme_Click(object sender, EventArgs e)
        {
            if (cbTheme.SelectedItem == null) return;

            ThemeItem item = _ThemesList.GetThemeByName(cbTheme.SelectedItem.ToString());
            if (item == null) return;

            // Delete the theme <{0}>?
            string tx = Strings.DeleteThemeQuestion;

            if (MessageBox.Show(string.Format(tx, item.Name), Application.ProductName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (_ThemesList.Remove(item.Name))
                {
                    cbTheme.Items.Remove(item.Name);
                    cbTheme.SelectedIndex = 0;
                    SaveThemesList();
                }
            }
        }

        #endregion Color Themes


        #region Color functions

        /// <summary>
        /// Check text representing a color
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color Parse(string input)
        {
            if (input == null)
                return Color.Black;

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
            ColorDialog dlg = new ColorDialog
            {
                FullOpen = true,
                ShowHelp = true,
                Color = picBox.BackColor,       // Sets the initial color select to the current text color.
            };


            if (dlg.ShowDialog() != DialogResult.OK) return picBox.BackColor;

            picBox.BackColor = dlg.Color;
            textBox.Text = ToHex(dlg.Color);

            ThemeModified(true);

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
            ThemeModified(true);
            txb.Text = ToHex(c);
            this.Show();
        }


        #endregion Color functions


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
                    pBox.FreqSlideShow = freqSlideShow;
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
            _nbLyricsLines = (int)UpDownNbLines.Value;
            pBox.nbLyricsLines = _nbLyricsLines;
        }

        private void TxtSlideShow_TextChanged(object sender, EventArgs e)
        {
            string tx = txtSlideShow.Text;
            tx = tx.Trim();
            dirSlideShow = tx;

            // Only if option Diaporama is selected
            if (radioDiaporama.Checked)
                pBox.SetDirectoryBackground(dirSlideShow);
        }

        private bool IsNumeric(string s)
        {
            //float output;
            return float.TryParse(s, out float output);
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

        // Show hints (introduction, instrumental, ending)
        private void chkShowHints_CheckedChanged(object sender, EventArgs e)
        {
            bShowHints = chkShowHints.Checked;
            pBox.bShowHints = bShowHints;
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
                    case "FourLinesSwapped":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped;
                        UpDownNbLines.Visible = false;
                        break;
                    case "ConstantScrolling":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.ConstantScrolling;
                        break;
                    case "DynamicScrolling":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.DynamicScrolling;
                        break;
                    case "TwoLinesSwapped":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.TwoLinesSwapped;
                        UpDownNbLines.Visible = false;
                        break;
                    case "FixedLines":
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FixedLines;
                        UpDownNbLines.Visible = true;
                        lblNumberOfLines.Visible = true;
                        break;

                    default:
                        pBox.KaraokeDisplayType = KaraokeDisplayTypes.FourLinesSwapped;
                        break;
                }

                lblNumberOfLines.Visible = UpDownNbLines.Visible;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Display song name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowSongName_CheckedChanged(object sender, EventArgs e)
        {
            bShowSongName = chkShowSongName.Checked;
            pBox.bShowSongName = bShowSongName;
        }

        /// <summary>
        /// Display Logo image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShowLogo_CheckedChanged(object sender, EventArgs e)
        {
            bShowLogo = chkShowLogo.Checked;
            pBox.bShowLogo = bShowLogo;
        }


        #endregion events


        #region form load close


        /// <summary>
        /// Form load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMidiLyrOptions_Load(object sender, EventArgs e)
        {
            lblNumberOfLines.Visible = UpDownNbLines.Visible;
            this.TopMost = true;                       
        }

        /// <summary>
        /// Form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMidiLyrOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region save current theme
            if (bColorModified == true && _currentTheme.Name != "Default")
            {
                //string tx = "Le fichier a été modifié, voulez-vous l'enregistrer ?";
                string tx = Karaboss.Resources.Localization.Strings.QuestionSavefile;
                DialogResult dr = MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (dr == DialogResult.Cancel)
                {
                    // Do not save but cancel close
                    e.Cancel = true;
                    return;
                }
                else if (dr == DialogResult.Yes)
                {
                    // Save and close                    
                    SaveTheme(_currentTheme.Name);                                        
                }
                else if (dr == DialogResult.No) 
                {
                    // Do not save and close
                }
            }
            #endregion save current theme

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


        #region font

        private void cbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ftName = cbFontName.SelectedItem.ToString();

            _karaokeFont = new Font(ftName, ftSize, FontStyle.Regular);
            pBox.KaraokeFont = _karaokeFont;
        }

        private void cbFontStretching_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFontStretching.SelectedValue.GetType() == typeof(string))
            {
                FontStretching = cbFontStretching.SelectedValue.ToString();
            }
            else
            {
                FontStretching = ((KeyValuePair<string, string>)cbFontStretching.SelectedValue).Key.ToString();
            }

            pBox.FontStretching = FontStretching;
        }

        #endregion font 
      

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


        #region gradient
        private void cbGrad0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Color0 of the gradient panel
            if (cbGrad0.SelectedItem is Color selectedColor)
            {
                Grad0Color = selectedColor; // Update the Grad0Color variable
                pBox.Grad0Color = Grad0Color; // Update the gradient panel color
                ThemeModified(true);
            }
        }

        private void cbGrad1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Grad1Color
            if (cbGrad1.SelectedItem is Color selectedColor)
            {
                Grad1Color = selectedColor; // Update the Grad1Color variable
                pBox.Grad1Color = Grad1Color; // Update the gradient panel color
                ThemeModified(true);
            }
        }

        private void cbRhythm0_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm0Color
            if (cbRhythm0.SelectedItem is Color selectedColor)
            {
                Rhythm0Color = selectedColor; // Update the Rhythm0Color variable
                pBox.Rhythm0Color = Rhythm0Color; // Update the gradient panel color
                ThemeModified(true);
            }
        }

        private void cbRhythm1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Apply the selected color from the ComboBox to Rhythm1Color of the gradient panel
            if (cbRhythm1.SelectedItem is Color selectedColor)
            {
                Rhythm1Color = selectedColor; // Update the Rhythm1Color variable
                pBox.Rhythm1Color = Rhythm1Color; // Update the gradient panel color
                ThemeModified(true);
            }
        }

        #endregion gradient



        #region Lyrics decoration 

        #region text events


        private void txtBgColor_TextChanged(object sender, EventArgs e)
        {
            BgColor = Parse(txtBgColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }


        private void txtActiveColor_TextChanged(object sender, EventArgs e)
        {
            ActiveColor = Parse(txtActiveColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtHighlightColor_TextChanged(object sender, EventArgs e)
        {
            HighlightColor = Parse(txtHighlightColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtInactiveColor_TextChanged(object sender, EventArgs e)
        {
            InactiveColor = Parse(txtInactiveColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtActiveBorderColor_TextChanged(object sender, EventArgs e)
        {
            ActiveBorderColor = Parse(txtActiveBorderColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtInactiveBorderColor_TextChanged(object sender, EventArgs e)
        {
            InactiveBorderColor = Parse(txtInactiveBorderColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtInactiveChordColor_TextChanged(object sender, EventArgs e)
        {
            InactiveChordColor = Parse(txtInactiveChordColor.Text);
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtHighlightChordColor_TextChanged(object sender, EventArgs e)
        {            
            HighlightChordColor = Parse(txtHighlightChordColor.Text); ;
            ApplyNewColors();
            ThemeModified(true);
        }

        private void txtActiveInstrumentalColor_TextChanged(object sender, EventArgs e)
        {
            ActiveInstrumentalColor = Parse(txtActiveInstrumentalColor.Text);
            ApplyNewColors();
            ThemeModified(true);
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


        private void btnActiveInstrumentalColor_Click(object sender, EventArgs e)
        {
            Color clr = SelectColorFromButton(picActiveInstrumentalColor, txtActiveInstrumentalColor);
            ActiveInstrumentalColor = clr;
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

        private void btnActiveInstrumentalColorPicker_Click(object sender, EventArgs e)
        {
            SelectColorFromPicker(txtActiveInstrumentalColor);
        }

        #endregion select color with picker


        #endregion Lyrics decoration
       

        #region option form settings

        // Apply changes to frmMidiLyrics
        private void ApplyChanges()
        {
            // Save first
            SaveOptions();

            // Apply to lyrics form
            if (Application.OpenForms.OfType<frmMidiLyrics>().Count() > 0) {
                frmMidiLyrics frmMidiLyrics = Utilities.FormUtilities.GetForm<frmMidiLyrics>();
                frmMidiLyrics.ApplyFromOptionsForm();
            }          
        }


        #region Themes Color
        private ThemesList LoadThemes()
        {
            try
            {
                string fileName = Karaclass.GetThemesListFile(_ThemesListHelper.File);
                _ThemesListHelper.File = fileName;
                return _ThemesListHelper.Load(fileName);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        #endregion Themes Color


        /// <summary>
        /// Load options stored in properties
        /// </summary>
        private void LoadOptions()
        {
            try
            {
                // Fonts
                #region Fonts

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


                // Font stretching
                PopulateFontStretching();
                FontStretching = Properties.Settings.Default.FontStretching;
                for (int i = 0; i < cbFontStretching.Items.Count; i++)
                {
                    if (((KeyValuePair<string, string>)cbFontStretching.Items[i]).Key == FontStretching)
                    {
                        cbFontStretching.SelectedIndex = i;
                        break;
                    }
                }


                #endregion Fonts


                // Frames (no border, 1 pixel border, neon etc)
                #region Frames
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
                #endregion Frames


                // Karaoke display types (FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped
                // karaokeEffect1 is updated by the options form when changing the display type, so we need to set it before setting the selected item in the combo box
                #region Layout
                PopulateKaraokeDisplayTypes();

                KaraokeDisplayType = Properties.Settings.Default.KaraokeDisplayType;

                foreach (KeyValuePair<string, string> valuePair in cbKaraokeType.Items)
                {
                    if (!string.IsNullOrEmpty(valuePair.Key))
                    {
                        if (valuePair.Key == KaraokeDisplayType.ToString())
                        {
                            cbKaraokeType.SelectedItem = valuePair;
                            break;
                        }
                    }
                }

                #endregion Layout

                // Populate Combos with known colors
                cboColor.DisplayKnownColors(cbGrad0);
                cboColor.DisplayKnownColors(cbGrad1);
                cboColor.DisplayKnownColors(cbRhythm0);
                cboColor.DisplayKnownColors(cbRhythm1);


                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;

                // Show hints (introduction, instrumental, ending)
                bShowHints = Properties.Settings.Default.bShowHints;

                // Show song name
                chkShowSongName.Checked = Properties.Settings.Default.bShowSongName;

                // Show logo
                bShowLogo = Properties.Settings.Default.bShowLogo;
                chkShowLogo.Checked = bShowLogo;


                // Display balls on lyrics
                chkDisplayBalls.Checked = Karaclass.m_DisplayBalls;


                // Load colors from theme
                PopulateThemes();

                // textBox => pic 
                picBgColor.BackColor = Parse(txtBgColor.Text);

                picActiveColor.BackColor = Parse(txtActiveColor.Text);
                picHighlightColor.BackColor = Parse(txtHighlightColor.Text);
                picInactiveColor.BackColor = Parse(txtInactiveColor.Text);
                picActiveBorderColor.BackColor = Parse(txtActiveBorderColor.Text);
                picInactiveBorderColor.BackColor = Parse(txtInactiveBorderColor.Text);

                picInactiveChordColor.BackColor = Parse(txtInactiveChordColor.Text);
                picHighlightChordColor.BackColor = Parse(txtHighlightChordColor.Text);

                // Instrumental color
                picActiveInstrumentalColor.BackColor = Parse(txtActiveInstrumentalColor.Text);

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

                // Instrumental Color
                ActiveInstrumentalColor = picActiveInstrumentalColor.BackColor;


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

                // Display single image
                SingleImagePath = Properties.Settings.Default.SingleImagePath;
                if (File.Exists(SingleImagePath))
                {
                    txtImage.Text = Path.GetFileName(SingleImagePath);
                    pBox.SingleImagePath = SingleImagePath;
                }


                // Background (Image, Diaporama, SolidColor...)
                #region Backgrounds
                bgOption = Properties.Settings.Default.BackGroundOption;

                switch (bgOption)
                {
                    case "Image":
                        radioImage.Checked = true; break;
                    case "Diaporama":
                        radioDiaporama.Checked = true; break;
                    case "SolidColor":
                        radioSolidColor.Checked = true; break;
                    case "Gradient":
                        radioGradient.Checked = true; break;
                    case "Rhythm":
                        radioRhythm.Checked = true; break;
                    case "Transparent":
                        radioTransparent.Checked = true; break;
                    default:
                        bgOption = "Diaporama"; break;
                }
                #endregion Backgrounds

                // Nb lines to display
                _nbLyricsLines = Properties.Settings.Default.TxtNbLines;


                // SlideShow directory
                dirSlideShow = Properties.Settings.Default.dirSlideShow;
                if (!Directory.Exists(dirSlideShow))
                    dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
                //pBox.DirSlideShow = dirSlideShow;

                freqSlideShow = Properties.Settings.Default.freqSlideShow;

                #region SizeMode
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
                #endregion SizeMode
               
                // Cancel changes
                ThemeModified(false);

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

        /// <summary>
        /// FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped
        /// </summary>
        private void PopulateKaraokeDisplayTypes()
        {
            // Populate karaoke display types cbKaraokeType         
            KaraokeTypes = new Dictionary<string, string>()
            {
                { "FourLinesSwapped", Strings.KTypesFourLinesSwapped },
                { "ConstantScrolling", Strings.KTypesConstantScrolling },
                { "DynamicScrolling", Strings.KTypesDynamicScrolling },
                { "TwoLinesSwapped", Strings.KTypesTwoLinesSwapped },
                { "FixedLines", Strings.KTypesFixedLines },
            };
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

        private void PopulateFontStretching()
        {
            Dictionary<string, string> dicFontStretching = new Dictionary<string, string>();
            dicFontStretching.Add("Small", Strings.FontStretchingSmall);
            dicFontStretching.Add("Large", Strings.FontStretchingLarge);
            cbFontStretching.DataSource = new BindingSource(dicFontStretching, null);
            cbFontStretching.ValueMember = "Key";
            cbFontStretching.DisplayMember = "Value";
        }


        /// <summary>
        /// Frames
        /// </summary>
        private void PopulateLyricBorders()
        {
            Dictionary<string, string> Frames = new Dictionary<string, string>()
            {
                { "NoBorder", Strings.KfnBorderNoBorder },
                { "FrameThin", Strings.KfnBorderFrameThin },
                { "Frame1", Strings.KfnBorderFrame1 },
                { "Frame2", Strings.KfnBorderFrame2 },
                { "Frame3", Strings.KfnBorderFrame3 },
                { "Frame4", Strings.KfnBorderFrame4 },
                { "Frame5", Strings.KfnBorderFrame5 },
                { "Shadow", Strings.KfnBorderShadow },
                { "Neon", Strings.KfnBorderNeon }
            };


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

                // Background option (Image, diaporama, transperent, gradient etc...)
                Properties.Settings.Default.BackGroundOption = bgOption;

                // Font                
                Properties.Settings.Default.KaraokeFontName = ftName;
                Properties.Settings.Default.FontStretching = FontStretching;

                // Show chords
                Properties.Settings.Default.bShowChords = _bShowChords;

                // FrameType
                Properties.Settings.Default.FrameType = FrameType;

                // window lyrics topmost
                Properties.Settings.Default.frmMidiLyricsTopMost = _bTopMost;

                // Force Uppercase
                Properties.Settings.Default.bForceUppercase = bForceUppercase;

                // Show hints (introduction, instrumental, ending)
                Properties.Settings.Default.bShowHints = bShowHints;

                // Show song name
                Properties.Settings.Default.bShowSongName = chkShowSongName.Checked;

                // Show logo
                Properties.Settings.Default.bShowLogo = chkShowLogo.Checked;

                // Number of lines to display
                Properties.Settings.Default.TxtNbLines = _nbLyricsLines;


                // Display single Image
                if (File.Exists(SingleImagePath))
                {
                    Properties.Settings.Default.SingleImagePath = SingleImagePath;
                }

                // SlideShow
                dirSlideShow = txtSlideShow.Text.Trim();
                if (Directory.Exists(dirSlideShow) == false)
                    dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
                Properties.Settings.Default.dirSlideShow = dirSlideShow;

                Properties.Settings.Default.freqSlideShow = freqSlideShow;
                Properties.Settings.Default.SizeMode = SizeMode;

                #region Text position

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

                #endregion Text position

                // Lyrics background
                Properties.Settings.Default.bLyricsBackGround = chkTextBackground.Checked;

                // Karaoke display type (FixedLines, ScrollingLinesBottomUp, ScrollingLinesTopDown, TwoLinesSwapped
                Properties.Settings.Default.KaraokeDisplayType = KaraokeDisplayType;

                // Save theme name
                Properties.Settings.Default.Theme = cbTheme.SelectedItem.ToString();

                // Save colors of lyrics, backgrounds, chords in themes (not if theme Default is active)
                ThemeItem item = _ThemesList.GetThemeByName(cbTheme.SelectedItem.ToString());
                if (item != null && item.Name != "Default")
                    SaveTheme(item.Name);

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

                // Instrumental color
                picActiveInstrumentalColor.BackColor = ActiveInstrumentalColor;

                // Window Lyrics TopMost
                chkTopMost.Checked = _bTopMost;

                // Force uppercase
                chkTextUppercase.Checked = bForceUppercase;
                pBox.bforceUppercase = bForceUppercase;

                // Show hints (introduction, instrumental, ending)
                chkShowHints.Checked = bShowHints;
                pBox.bShowHints = bShowHints;

                // SingleImage
                pBox.SingleImagePath = SingleImagePath;

                // SlideShow
                pBox.FreqSlideShow = freqSlideShow;
                pBox.nbLyricsLines = _nbLyricsLines;
                //pBox.CurrentTime = 30;

                // Backgrounds
                pBox.BgColor = BgColor;

                // Gradients
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

                // Instrumental color
                pBox.ActiveInstrumentalColor = ActiveInstrumentalColor;

                // Frame type
                pBox.FrameType = FrameType;


                cbSizeMode.SelectedText = SizeMode.ToString();

                pBox.OptionBackground = bgOption;

                pBox.SetDirectoryBackground(dirSlideShow);
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
            pBox.HighlightChordColor = HighlightChordColor;

            // Instrumental color
            pBox.ActiveInstrumentalColor = ActiveInstrumentalColor;

            //Color of buttons
            picBgColor.BackColor = BgColor;

            picActiveColor.BackColor = ActiveColor;
            picInactiveColor.BackColor = InactiveColor;
            picHighlightColor.BackColor = HighlightColor;

            picActiveBorderColor.BackColor = ActiveBorderColor;
            picInactiveBorderColor.BackColor = InactiveBorderColor;

            picInactiveChordColor.BackColor = InactiveChordColor;
            picHighlightChordColor.BackColor = HighlightChordColor;

            picActiveInstrumentalColor.BackColor = ActiveInstrumentalColor;
        }



        #endregion option form settings

       
    }
}
