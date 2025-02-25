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
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;

namespace Karaboss
{
    public partial class frmLyrOptions : Form
    {

        #region private properties

        private Karaclass.OptionsDisplay OptionDisplay;        

        private string bgOption = "Diaporama";
        // Font
        private Font _karaokeFont;

        // Text color
        private Color TxtNextColor;
        // Text to sing color
        private Color TxtHighlightColor;
        // Text sung color
        private Color TxtBeforeColor;
        // Background color
        private Color TxtBackColor;


        // Chord color
        private Color _chordNextColor;
        private Color _chordHighlightColor;

        private bool _bShowChords = false;



        // Force Uppercase
        private bool bForceUppercase = false;

        // Contour color
        private bool bColorContour = true;
        private Color TxtContourColor;

        // Number of lines to display
        private int NbLines;
        //Slideshow
        private string dirSlideShow;
        // Frequency
        private int freqSlideShow;

        // Size mode of the picture background
        private PictureBoxSizeMode SizeMode;

        private frmLyric frmLyric;
        
        #endregion private properties

        public frmLyrOptions()
        {
            InitializeComponent();            
            LoadOptions();     
            SetOptions();

            pBox.SetBackground(dirSlideShow);
            pBox.bDemo = true;
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
                _karaokeFont = Properties.Settings.Default.KaraokeFont;
                pBox.KaraokeFont = _karaokeFont;
                txtFont.Text = _karaokeFont.Name;

                // Force Uppercase
                bForceUppercase = Karaclass.m_ForceUppercase;
              

                // Display balls on lyrics
                chkDisplayBalls.Checked = Karaclass.m_DisplayBalls;

                TxtBackColor = Properties.Settings.Default.TxtBackColor;
                // Colors
                TxtNextColor = Properties.Settings.Default.TxtNextColor;
                TxtHighlightColor = Properties.Settings.Default.TxtHighlightColor;
                TxtBeforeColor = Properties.Settings.Default.TxtBeforeColor;

                // Chords
                _chordNextColor = Properties.Settings.Default.ChordNextColor;
                _chordHighlightColor = Properties.Settings.Default.ChordHighlightColor;
                _bShowChords = Properties.Settings.Default.bShowChords;

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
                TxtBackColor = Color.White;
                TxtNextColor = Color.Black;
                TxtHighlightColor = Color.Red;
                TxtBeforeColor = Color.YellowGreen;
                TxtContourColor = Color.Black;
                NbLines = 3;
                bColorContour = true;
                
                dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName) + "\\slideshow";

                freqSlideShow = 10;
                SizeMode = PictureBoxSizeMode.Zoom;
            }
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

                Properties.Settings.Default.KaraokeFont = _karaokeFont;

                Properties.Settings.Default.TxtBackColor = TxtBackColor;

                Properties.Settings.Default.TxtNextColor = TxtNextColor;
                Properties.Settings.Default.TxtHighlightColor = TxtHighlightColor;
                Properties.Settings.Default.TxtBeforeColor = TxtBeforeColor;

                // chords
                Properties.Settings.Default.ChordNextColor = _chordNextColor;
                Properties.Settings.Default.ChordHighlightColor = _chordHighlightColor;
                Properties.Settings.Default.bShowChords = _bShowChords;

                // Contour
                Properties.Settings.Default.bColorContour = bColorContour;
                Properties.Settings.Default.TxtContourColor = TxtContourColor;

                // Force Uppercase
                Properties.Settings.Default.bForceUppercase = bForceUppercase;

                Properties.Settings.Default.TxtNbLines = NbLines;

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

                // buttons
                pictBackColor.BackColor = TxtBackColor;
                pictBefore.BackColor = TxtBeforeColor;

                chkContour.Checked = bColorContour;
                pictContour.BackColor = TxtContourColor;

                pictHighlight.BackColor = TxtHighlightColor;
                pictNext.BackColor = TxtNextColor;

                // Chords
                picChordBefore.BackColor = _chordNextColor;
                picChordHighlight.BackColor = _chordHighlightColor;
                pBox.bShowChords = _bShowChords;

                // Force uppercase
                chkTextUppercase.Checked = bForceUppercase;
                pBox.bforceUppercase = bForceUppercase;

                // picturebox            
                pBox.FreqDirSlideShow = freqSlideShow;
                pBox.TxtNbLines = NbLines;
                pBox.CurrentTime = 30;


                pBox.TxtBackColor = TxtBackColor;

                pBox.bColorContour = bColorContour;
                pBox.TxtContourColor = TxtContourColor;

                pBox.TxtNextColor = TxtNextColor;
                pBox.TxtHighlightColor = TxtHighlightColor;
                pBox.TxtBeforeColor = TxtBeforeColor;

                // Chords
                pBox.ChordNextColor = _chordNextColor;
                pBox.ChordHighlightColor = _chordHighlightColor;
                chkForceShowChords.Checked = _bShowChords;

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
            // picturebox
            
            pBox.TxtBackColor = TxtBackColor;

            pBox.bColorContour = bColorContour;
            pBox.TxtContourColor = TxtContourColor;
            pBox.TxtNextColor = TxtNextColor;
            pBox.TxtHighlightColor = TxtHighlightColor;
            pBox.TxtBeforeColor = TxtBeforeColor;

            pBox.OptionDisplay = (PicControl.pictureBoxControl.OptionsDisplay)OptionDisplay;

            // Chords
            pBox.ChordNextColor = _chordNextColor;
            pBox.ChordHighlightColor= _chordHighlightColor;


            //Color of buttons
            pictBackColor.BackColor = TxtBackColor;
            pictBefore.BackColor = TxtBeforeColor;
            pictContour.BackColor = TxtContourColor;
            pictHighlight.BackColor = TxtHighlightColor;
            pictNext.BackColor = TxtNextColor;           
        }


        #endregion option form settings


        #region select colors

        /// <summary>
        /// Dialog get color
        /// </summary>
        /// <param name="defColor"></param>
        /// <returns></returns>
        private Color DlgGetColor(Color defColor)
        {
            ColorDialog MyDialog = new ColorDialog()
            {
                AllowFullOpen = true,
                ShowHelp = true,
                Color = defColor,
            };
            
            if (MyDialog.ShowDialog() == DialogResult.OK)
                return MyDialog.Color;
            else
                return defColor;
        }

        /// <summary>
        /// Backcolor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBackColor_Click(object sender, EventArgs e)
        {
            
            Color clr = DlgGetColor(TxtBackColor);
            if (clr == TxtBackColor)
                return;
            TxtBackColor = clr;
            pictBackColor.BackColor = clr;
            ApplyNewColors();
        }

        /// <summary>
        /// Text color: before
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSungColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtBeforeColor);
            if (clr == TxtBeforeColor)
                return;
            TxtBeforeColor = clr;
            pictBefore.BackColor = clr;
            ApplyNewColors();
        }

        /// <summary>
        /// Text color: highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSingColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtHighlightColor);
            if (clr == TxtHighlightColor)
                return;
            TxtHighlightColor = clr;
            pictHighlight.BackColor = clr;
            ApplyNewColors();                        
        }

        /// <summary>
        /// Text color: after
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnForeColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtNextColor);
            if (clr == TxtNextColor)
                return;
            TxtNextColor = clr;
            pictNext.BackColor = clr;
            ApplyNewColors();            
        }

        /// <summary>
        /// Text color: contour
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnContourColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(TxtContourColor);
            if (clr == TxtContourColor)
                return;
            TxtContourColor = clr;
            pictContour.BackColor = clr;
            ApplyNewColors();            
        }

        /// <summary>
        /// Draw contour or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkContour_CheckedChanged(object sender, EventArgs e)
        {
            bColorContour = chkContour.Checked;
            ApplyNewColors();
        }

        #endregion select colors


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
                dirSlideShow = folderBrowserDialog1.SelectedPath;
                
                Cursor.Current = Cursors.WaitCursor;
                txtSlideShow.Text = dirSlideShow;
                pBox.SetBackground(dirSlideShow);
            }
        }

        /// <summary>
        /// Reset directory for slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetDir_Click(object sender, EventArgs e)
        {
            dirSlideShow = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            txtSlideShow.Text = dirSlideShow;
            pBox.SetBackground(dirSlideShow);
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

        // Apply changes to frmLyric
        private void ApplyChanges()
        {
            SaveOptions();

            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                frmLyric = Utilities.FormUtilities.GetForm<frmLyric>();

                frmLyric.bShowBalls = Karaclass.m_DisplayBalls;

                frmLyric.KaraokeFont = _karaokeFont;

                // Text colors                
                frmLyric.TxtBackColor = TxtBackColor;
                                
                frmLyric.TxtNextColor = TxtNextColor;
                frmLyric.TxtHighlightColor = TxtHighlightColor;
                frmLyric.TxtBeforeColor = TxtBeforeColor;

                frmLyric.bColorContour = bColorContour;
                frmLyric.TxtContourColor = TxtContourColor;

                // Chords
                frmLyric.ChordNextColor = _chordNextColor;
                frmLyric.ChordHighlightColor = _chordHighlightColor;
                frmLyric.bShowChords = _bShowChords;
                

                // force uppercase
                frmLyric.bForceUppercase = bForceUppercase;

                NbLines = Convert.ToInt32(UpDownNbLines.Value);
                frmLyric.TxtNbLines = NbLines;

                frmLyric.SizeMode = SizeMode;

                // Diaporam, Backcolor ou transparent
                frmLyric.OptionBackground = bgOption;

                // Text display: Center, Top, Bottom
                frmLyric.OptionDisplay = OptionDisplay;

                frmLyric.bTextBackGround = chkTextBackground.Checked;
                                               
                // SlideShow frequency
                frmLyric.FreqSlideShow = freqSlideShow;               
                
                // directory for slide show

                frmLyric.DirSlideShow = dirSlideShow;
                
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

        }

        /// <summary>
        /// Form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLyrOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            pBox.Terminate();

            // Active le formulaire frmPlayer
            if (Application.OpenForms.OfType<frmPlayer>().Count() > 0)
            {
                // Restore form
                if (Application.OpenForms["frmPlayer"].WindowState != FormWindowState.Minimized)
                {                    
                    Application.OpenForms["frmPlayer"].Restore();
                    Application.OpenForms["frmPlayer"].Activate();
                }
            }

            // Active le formulaire frmLyric
            if (Application.OpenForms.OfType<frmLyric>().Count() > 0)
            {
                // Restore form
                Application.OpenForms["frmLyric"].Restore();
                Application.OpenForms["frmLyric"].Activate();
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
                btnBackColor.Visible = false;
                pictBackColor.Visible = false;

                pBox.OptionBackground = "Diaporama";
                bgOption = "Diaporama";
                pBox.SetBackground(dirSlideShow);
            }
        }

        private void RadioSolidColor_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSolidColor.Checked)
            {
                btnBackColor.Visible = true;
                pictBackColor.Visible = true;

                pBox.OptionBackground = "SolidColor";
                bgOption = "SolidColor";
            }
        }

        private void RadioTransparent_CheckedChanged(object sender, EventArgs e)
        {
            if (radioTransparent.Checked)
            {
                btnBackColor.Visible = false;
                pictBackColor.Visible = false;

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
        /// Number of Karaoke lines to display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpDownNbLines_ValueChanged(object sender, EventArgs e)
        {
            NbLines = (int)UpDownNbLines.Value;
            pBox.TxtNbLines = NbLines;
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
             

        private void BtnFonts_Click(object sender, EventArgs e)
        {
            try
            {
                // Show the dialog.
                fontDialog1.Font = _karaokeFont;

                if (fontDialog1.ShowDialog() == DialogResult.OK)
                {
                    txtFont.Text = fontDialog1.Font.Name;
                    _karaokeFont = fontDialog1.Font;
                    pBox.KaraokeFont = _karaokeFont;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                _karaokeFont = new Font("Arial", this.Font.Size);

            }
        
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

        #endregion


        #region chords
        private void chkForceShowChords_CheckedChanged(object sender, EventArgs e)
        {
            _bShowChords = chkForceShowChords.Checked;
            pBox.bShowChords = _bShowChords;

        }

        private void btnChordNormalColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(_chordNextColor);
            if (clr == _chordNextColor)
                return;
            _chordNextColor = clr;            
            picChordBefore.BackColor = clr;
            ApplyNewColors();
        }

        private void btnChordHighlightColor_Click(object sender, EventArgs e)
        {
            Color clr = DlgGetColor(_chordHighlightColor);
            if (clr == _chordHighlightColor)
                return;
            _chordHighlightColor = clr;
            picChordHighlight.BackColor = clr;
            ApplyNewColors();
        }
        #endregion chords

    }
}
