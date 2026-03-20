using Hqub.MusicBrainz.API.Entities;
using Karaboss.Mp3;
using Karaboss.Resources.Localization;
using KFNV;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    public partial class frmKfnCreate : Form
    {      
        private string fPath;      
        private readonly string strColorRegex = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
        Regex regexColor;
        string ftName;
        uint ftSize;

        public frmKfnCreate(string path)
        {
            InitializeComponent();

            TopMost = true;                       
            fPath = path;            

            regexColor = new Regex(strColorRegex);

            InitControls();
        }

        /// <summary>
        /// Initialize controls
        /// </summary>
        private void InitControls()
        {
            OpenFileDialog.InitialDirectory = fPath;

            tbControl.SizeMode = TabSizeMode.Fixed;
            tbControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tbControl.ItemSize = new Size((tbControl.Width / tbControl.TabCount) - 1, tbControl.ItemSize.Height);

            string tx = Strings.kfnCreateTb1;
            tx = string.Format(tx, Environment.NewLine);
            lblHelpTb1.Text = tx;

            lblHelpTb2.Text = Karaboss.Resources.Localization.Strings.kfnCreateTb2;

            tx = Strings.kfnCreateTb3;
            tx = string.Format(tx, Environment.NewLine);
            lblHelpTb3.Text = tx;

            tx = Strings.kfnCreateTb4;
            tx = string.Format(tx, Environment.NewLine);
            lblHelpTb4.Text = tx;

            ftName = "Arial Black";
            ftSize = 20;

            PopulateFonts();
        }

        #region select audios

        /// <summary>
        /// Import audio file with vocals
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportAudio1_Click(object sender, EventArgs e)
        {
            string FileName;

            try
            {

                OpenFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
                OpenFileDialog.FileName = string.Empty;
                OpenFileDialog.Title = "Vocal audio";

                if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

                FileName = OpenFileDialog.FileName;
                OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

                txtAudio1.Text = FileName;

                SetTitleFromFile(FileName);


                btnLyricsUpdate.Visible = true;
                lblLyricsUpdate.Visible = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Import audio file instrumental
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportAudio2_Click(object sender, EventArgs e)
        {
            string FileName;
            try
            {
                OpenFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
                OpenFileDialog.FileName = string.Empty;
                OpenFileDialog.Title = "Instrumental audio";

                if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

                FileName = OpenFileDialog.FileName;
                OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

                txtAudio2.Text = FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Starting from the file name, sets the title of song and the name of the future KFN file
        /// </summary>
        /// <param name="FileName"></param>
        private void SetTitleFromFile(string FileName)
        {
                        
            if (txtTitle.Text.Trim().Length > 0) return;


            string artist = string.Empty;
            string song = string.Empty;
            string sname = Path.GetFileNameWithoutExtension(FileName);

            int n = sname.IndexOf(" - ");
            if (n > 0)
            {
                artist = sname.Substring(0, n);
                song = sname.Substring(n + 3);

            }
            else
            {
                song = sname;
            }

            txtTitle.Text = song;
            txtArtist.Text = artist;

            string kfnFile = Path.ChangeExtension(Path.GetFileName(FileName), ".kfn");
            txtKfnFileName.Text = kfnFile;
        }


        /// <summary>
        /// Import Song.ini file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportLyrics_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "LRC files (*.lrc)|*.lrc|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

            FileName = OpenFileDialog.FileName;
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

            txtLyrics.Text = FileName;
        }

        #endregion select audios


        #region select images

        /// <summary>
        /// Import image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportImage_Click(object sender, EventArgs e)
        {
            string FileName;
            
            OpenFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.tif;...|All files (*.*)|*.*";
            OpenFileDialog.FileName = string.Empty;

            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

            FileName = OpenFileDialog.FileName;
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

            txtImageFile.Text = FileName;

            // Load image into picImage
            try
            {
                Image img = Image.FromFile(FileName);
                lblSize.Text = "Size: " + img.Width.ToString() + " x " + img.Height.ToString();

                double ratio = img.Width / (double)img.Height;
                lblRatio.Text = "Ratio: " + String.Format("{0:N2}", ratio);

                picImage.SizeMode = PictureBoxSizeMode.StretchImage;
                picImage.Image = img;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        #endregion select images


        #region Button Create Play
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCreateKfn_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageAudios;

            // Create KFN File
            CreateKfnFile();
        }

        /// <summary>
        /// Create KFN file
        /// </summary>
        /// <param name="AudioFiles"></param>
        /// <param name="LyricsFileName"></param>
        /// <param name="imageFileName"></param>
        /// <param name="BgColor"></param>
        private void CreateKfnFile()
        {
            string AudioFileName1;
            string AudioFileName2;
            string LyricsFileName;
            string ImageFileName;
            string BgColor;
            string Year;
            string Author;
            string Title;
            string Artist;
            string Comment;
            (string, uint) fontName;

            #region guard

            AudioFileName1 = txtAudio1.Text.Trim();
            if (!File.Exists(AudioFileName1))
            {
                MessageBox.Show("Invalid audio file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AudioFileName2 = txtAudio2.Text.Trim();
            if (AudioFileName2 != string.Empty && !File.Exists(AudioFileName2))
            {
                MessageBox.Show("Invalid audio file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            LyricsFileName = txtLyrics.Text.Trim();
            if (!File.Exists(LyricsFileName))
            {
                MessageBox.Show("Invalid lyrics file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImageFileName = txtImageFile.Text.Trim();
            if (ImageFileName != string.Empty && !File.Exists(ImageFileName))
            {
                MessageBox.Show("Invalid image file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check background color            
            if (regexColor.IsMatch(txtBgColor.Text.Trim()))
                BgColor = txtBgColor.Text.Trim();
            else
            {
                BgColor = "#000000";    // Black default
                txtBgColor.Text = BgColor;
            }

            #endregion guard


            List<string> lstAudioFiles = new List<string>() { AudioFileName1 };
            if (AudioFileName2 != string.Empty)
                lstAudioFiles.Add(AudioFileName2);

            // Title
            Title = txtTitle.Text.Trim();
            
            // Artist
            Artist = txtArtist.Text.Trim();
            
            // Comment
            Comment = txtComment.Text.Trim();
            
            // Year
            Year = txtYear.Text.Trim();
            
            // Author
            Author = txtAuthor.Text.Trim();

            // Background color: BgColor
            // See guard

            // Font
            fontName = (txtLoremIpsum.Font.Name, (uint)txtLoremIpsum.Font.Size);


            string tx = "Create a new KFN file" + Environment.NewLine;

            for (int i = 0; i < lstAudioFiles.Count; i++)
            {
                tx += Environment.NewLine + string.Format("Audio{0}: {1}", i + 1, Path.GetFileName(lstAudioFiles[i]));
            }

            tx += Environment.NewLine + "Lyrics: " + Path.GetFileName(LyricsFileName);
            tx += Environment.NewLine + "Title: " + Title;
            tx += Environment.NewLine + "Artist: " + Artist;
            tx += Environment.NewLine + "Comment: " + Comment;
            tx += Environment.NewLine + "Author: " + Author;

            if (MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes) return;

            // Create a kfn file using the name of the audio1 file ?
            string kfnFile = Path.Combine(Path.GetDirectoryName(AudioFileName1), txtKfnFileName.Text);

            // List of images (only 1 for the moment)
            List<string> lstImages = new List<string>();
            if (ImageFileName != string.Empty)
                lstImages = new List<string>() { ImageFileName };

            Cursor = Cursors.WaitCursor;
            // Initialize Writer
            KfnWriter Writer = new KfnWriter(kfnFile, lstAudioFiles, LyricsFileName, lstImages, Title, Artist, Comment, Year, Author, BgColor, fontName);
            if (Writer != null)
            {
                string result = Writer.CreateKFN();

                if (result != null)
                {
                    txtKfnFileName.Text = result;
                    btnPlay.Visible = true;
                }
            }
            
            Cursor = Cursors.Default;
        }


        private void btnPlay_Click(object sender, EventArgs e)
        {
            //string path = Path.GetDirectoryName(txtAudio1.Text.Trim());
            string FileName = txtKfnFileName.Text.Trim(); //       Path.Combine (path, txtKfnFileName.Text);
            
            try
            {
                //string FileName = Path.Combine(fPath, txtKfnFileName.Text);
                if (File.Exists(FileName))
                {
                    System.Diagnostics.Process.Start(FileName);
                }
                else
                {
                    MessageBox.Show("File not found:\n" + FileName, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLyricsUpdate_Click(object sender, EventArgs e)
        {            
            string AudioFileName1 = txtAudio1.Text.Trim();
            string AudioFileName2 = txtAudio2.Text.Trim();
            string AudioFileName = AudioFileName1;

            // Check lyrics
            string LyricsFileName = txtLyrics.Text.Trim();
            if (!File.Exists(LyricsFileName))
                LyricsFileName = null;

            // At least, audio1 must exists
            if (!File.Exists(AudioFileName1))
            {
                MessageBox.Show("Invalid audio1 file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error); return;
            }


            // Close frmMp3Player if displayed
            if (Application.OpenForms.OfType<frmMp3Player>().Count() > 0)
            {
                frmMp3Player frm = Utilities.FormUtilities.GetForm<frmMp3Player>();
                frm.Close();
            }

            // If audio2 exists, select audio2 because it contains vocals
            // It's easier for lyrics synchronisation.
            if (File.Exists(AudioFileName2))
                AudioFileName = AudioFileName2;


            // If lyrics are set, take them
            frmMp3Player frmMp3Player = new frmMp3Player(AudioFileName,null, false);

            frmMp3Player.Show();



        }

        #endregion Button Create Play


        #region navigation
        // Page1 
        private void btnTb1Next_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageLyrics;
        }

        // Page 2
        private void btnTb2Next_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageImages;
        }

        private void btnTb2Previous_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageAudios;
        }

        // Page 3
        private void btnTb3Previous_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageLyrics;
        }

        private void btnTb3Next_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageBackground;
        }

        // Page 4

        private void btnTb4Previous_Click(object sender, EventArgs e)
        {
            tbControl.SelectedTab = tbPageImages;
        }


        #endregion navigation


        #region background color

        private void btnBgColorSelect_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.FullOpen = true;
            dlg.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dlg.Color = picBgColor.BackColor;

            if (dlg.ShowDialog() != DialogResult.OK) return;

            picBgColor.BackColor = dlg.Color;
            txtBgColor.Text = ToHex(dlg.Color);
        }

        private static String ToHex(System.Drawing.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

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


        private void btnBgColorPicker_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmFullScreen frmFullScreen = new frmFullScreen();
            frmFullScreen.Show();            
        }

        public void GetColorFromPicker(Color c)
        {
            txtBgColor.Text = ToHex(c);
            this.Show();
        }

        private void txtBgColor_TextChanged(object sender, EventArgs e)
        {
            picBgColor.BackColor = Parse(txtBgColor.Text);
            txtLoremIpsum.BackColor = picBgColor.BackColor;

        }

        private void PopulateFonts()
        {
            // Karafun seems to support only a few fonts
            foreach (System.Drawing.FontFamily fnt in System.Drawing.FontFamily.Families)
            {
                cbFontName.Items.Add(fnt.Name);
            }

            //List<string> fontNames = new List<string>() { "Arial", "Arial Black", "Arial Unicode MS", "Courier New", "Georgia", "Impact", "Tahoma", "Times New Roman", "Verdana" };
            //cbFontName.DataSource = fontNames;

            cbFontName.SelectedIndex = cbFontName.FindString("Arial Black");
        }


        private void cbFontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ftName = cbFontName.SelectedItem.ToString();
            txtLoremIpsum.Font = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        private void UpDownFontSize_ValueChanged(object sender, EventArgs e)
        {
            ftSize = (uint)UpDownFontSize.Value;
            txtLoremIpsum.Font = new Font(ftName, ftSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }


        #endregion background color


        #region form load close
        private void frmKfnCreate_Load(object sender, EventArgs e)
        {
            try
            {

                txtAuthor.Text = Properties.Settings.Default.KfnAuthor;
                txtComment.Text = Properties.Settings.Default.KfnComment;
                txtBgColor.Text = Properties.Settings.Default.KfnBgColor;
                
                UpDownFontSize.Value = Properties.Settings.Default.KfnFontSize;

                string f = Properties.Settings.Default.KfnFontName;
                //cbFontName.SelectedItem = cbFontName.FindString(f);       // Fix find Arial before Arial Black
                for (int i = 0; i < cbFontName.Items.Count; i++)
                {
                    if (cbFontName.Items[i].ToString() == f)
                    {
                        cbFontName.SelectedIndex = i;
                        break;
                    }
                }
                


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void frmKfnCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtBgColor.Text.Trim().Length > 0) 
            {
                string BgColor;
                // Check background color            
                if (regexColor.IsMatch(txtBgColor.Text.Trim()))
                    BgColor = txtBgColor.Text.Trim();
                else
                {
                    BgColor = "#000000";    // Black default                    
                }
                Properties.Settings.Default.KfnBgColor = BgColor;
            }
            
            if (txtAuthor.Text.Trim().Length > 0) 
                Properties.Settings.Default.KfnAuthor = txtAuthor.Text.Trim();
            if (txtComment.Text.Trim().Length > 0)
                Properties.Settings.Default.KfnComment = txtComment.Text.Trim();

            Properties.Settings.Default.KfnFontName = cbFontName.SelectedItem.ToString();
            Properties.Settings.Default.KfnFontSize = (int)UpDownFontSize.Value;


            Properties.Settings.Default.Save();
        }

        #endregion form load close


        #region tabControl
        private void tbControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This event is called once for each tab button in your tab control
            // First paint the background with a color based on the current tab
            // e.Index is the index of the tab in the TabPages collection.
            //#f1c40f

            switch (e.Index)
            {
                case 0:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#2ECC71")), e.Bounds);
                    break;
                case 1:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#3399ff")), e.Bounds);
                    break;
                case 2:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#c0392b")), e.Bounds);
                    break;
                case 3:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#e67e22")), e.Bounds);
                    break;
                default:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#d35400")), e.Bounds);
                    break;
            }

            // Then draw the current tab button text 

            
            Rectangle paddedBounds = e.Bounds;
            paddedBounds.Inflate(-2, -2);
            Font f = new Font("Sego UI", 14, FontStyle.Regular, GraphicsUnit.Pixel);
            Brush b = new SolidBrush(Color.White);                       
            e.Graphics.DrawString(tbControl.TabPages[e.Index].Text, f, b, paddedBounds);

        }

        #endregion tabControl


        #region menus

        private void mnuFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

              
        private void mnuHelpForums_Click(object sender, EventArgs e)
        {
            Karaclass.DisplayUrl(Karaclass.url_forums);            
        }


        private void mnuHelpDocumentation_Click(object sender, EventArgs e)
        {
            Karaclass.DisplayUrl(Karaclass.url_documentation);
        }

        #endregion menus


    }
}
