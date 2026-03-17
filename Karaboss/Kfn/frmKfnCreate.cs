using KFNViewer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Karaboss.Kfn
{
    public partial class frmKfnCreate : Form
    {      

        private string fPath;
        private bool bPickColor = false;

        public frmKfnCreate(string path)
        {
            InitializeComponent();

            TopMost = true;            

            tbControl.SizeMode = TabSizeMode.Fixed;
            tbControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tbControl.ItemSize = new Size((tbControl.Width / tbControl.TabCount) - 1, tbControl.ItemSize.Height);

            fPath = path;
            OpenFileDialog.InitialDirectory = fPath;
        }

        #region select audios

        /// <summary>
        /// Import mp3 file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnImportAudio1_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

            FileName = OpenFileDialog.FileName;
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

            txtAudio1.Text = FileName;

            SetTitleFromFile(FileName);
        }

        private void btnImportAudio2_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

            FileName = OpenFileDialog.FileName;
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);


            txtAudio2.Text = FileName;
        }

        /// <summary>
        /// Starting from the file name, sets the title of song and the name of the future KFN file
        /// </summary>
        /// <param name="FileName"></param>
        private void SetTitleFromFile(string FileName)
        {
            string Title = txtTitle.Text.Trim();
            if (Title.Length == 0)
                Title = Path.GetFileNameWithoutExtension(FileName);

            txtTitle.Text = Title;

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
            OpenFileDialog.Filter = "Jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;

            FileName = OpenFileDialog.FileName;
            OpenFileDialog.InitialDirectory = Path.GetDirectoryName(FileName);

            txtImageFile.Text = FileName;
        }

        #endregion select images

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCreateKfn_Click(object sender, EventArgs e)
        {
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

            BgColor = txtBgColor.Text.Trim();

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

            string tx = "Create a new KFN file" + Environment.NewLine;

            for (int i = 0; i < lstAudioFiles.Count; i++)
            {
                tx += Environment.NewLine + string.Format("Audio{0}: {1}", i + 1, Path.GetFileName(lstAudioFiles[i]));
            }

            tx += Environment.NewLine + "Lyrics: " + Path.GetFileName(LyricsFileName);
            tx += Environment.NewLine + "Title: " + Title;
            tx += Environment.NewLine + "Artist: " + Artist;
            tx += Environment.NewLine + "Comment: " + Comment;

            if (MessageBox.Show(tx, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes) return;

            // Create a kfn file using the name of the mp3 file ?
            string kfnFile = Path.Combine(Path.GetDirectoryName(AudioFileName1), txtKfnFileName.Text);

            // List of images (only 1 for the moment)
            List<string> lstImages = new List<string>() { ImageFileName };


            // Initialize Writer
            KfnWriter Writer = new KfnWriter(kfnFile, lstAudioFiles, LyricsFileName, lstImages, Title, Artist, Comment, Year, Author, BgColor);
            if (Writer != null)
                Writer.CreateKFN();


            // Creeate 3 resources
            //string                              type,          string name                             , int enclength, int length, int offset, bool encrypted, bool aSource = false
            //ResourceFile res = new ResourceFile("Image"        , "Cadre_or_640_480.png"                , 51456        , 51456     , 0         , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "- SpriteNinieblhzor.png"             , 2144         , 2136      , 51456     , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "v35-2.jpg"                           , 37968        , 37965     , 53600     , true, false);
            //ResourceFile res = new ResourceFile("Visualization", "Eo.S. - ether_phat_edit.milk"        , 6608         , 6598      , 91568     , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "h35-1.jpg"                           , 38192        , 38183     , 98176     , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "h35-2.jpg"                           , 38768        , 38757     , 136368    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "movingb01.png"                       , 56384        , 56374     , 175136    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "v35-1.jpg"                           , 40352        , 40349     , 231520    , true, false);
            //ResourceFile res = new ResourceFile("Font"         , "HUNII017.TTF"                        , 41328        , 41328     , 271872    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "__black_screen__.png"                , 96           , 92        , 313200    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "cadre or fin 640_460.png"            , 13536        , 13524     , 313296    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "Hor_or_640.jpg"                      , 1408         , 1407      , 326832    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "mae_lune_kfn.jpg"                    , 63440        , 63434     , 328240    , true, false);
            //ResourceFile res = new ResourceFile("Image"        , "mae_bouquet.png"                     , 303296       , 303289    , 391680    , true, false);
            //ResourceFile res = new ResourceFile("Audio"        , "Christophe Maé - La lune (vocal).mp3", 3884160      , 3884146   , 694976    , true, false);
            //ResourceFile res = new ResourceFile("Audio"        , "Christophe Maé - La lune.mp3"        , 4338720      , 4338712   , 4579136   , true, true);
            //ResourceFile res = new ResourceFile("Config"       , "Song.ini"                            , 316080       , 316065    , 8917856   , true, false);

            // L'offset est caclulé en additionnant enclength avec l'offset précédant


            /*
            ResourceFile res = new ResourceFile("Audio", "Chiens - Louane.mp3", 2943507, 2943507, 0, false, true);
            Writer.Resources.Add(res);
            
            res = new ResourceFile("Config", "Song.ini", 5654, 5654, 2943507, false, false);
            Writer.Resources.Add(res);
            */
        }


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
            string strRegex = @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
            Regex re = new Regex(strRegex);
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

        }

        #endregion background color


        #region form load close
        private void frmKfnCreate_Load(object sender, EventArgs e)
        {
            txtAuthor.Text = Properties.Settings.Default.KfnAuthor;
            txtComment.Text  = Properties.Settings.Default.KfnComment;
            txtBgColor.Text = Properties.Settings.Default.KfnBgColor;
        }

    

        private void frmKfnCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (txtBgColor.Text.Trim().Length > 0)
                Properties.Settings.Default.KfnBgColor = txtBgColor.Text.Trim();
            
            if (txtAuthor.Text.Trim().Length > 0) 
                Properties.Settings.Default.KfnAuthor = txtAuthor.Text.Trim();
            if (txtComment.Text.Trim().Length > 0)
                Properties.Settings.Default.KfnComment = txtComment.Text.Trim();
            Properties.Settings.Default.Save();
        }

        #endregion form load close

        private void tbControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            // This event is called once for each tab button in your tab control
            // First paint the background with a color based on the current tab
            // e.Index is the index of the tab in the TabPages collection.
            //#f1c40f

            switch (e.Index)
            {
                case 0:
                    e.Graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#99cc33")), e.Bounds);
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
    }
}
