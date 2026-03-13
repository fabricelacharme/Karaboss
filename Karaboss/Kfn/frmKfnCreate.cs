using KFNViewer;
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
using static KFN;

namespace Karaboss.Kfn
{
    public partial class frmKfnCreate : Form
    {
        public frmKfnCreate()
        {
            InitializeComponent();
        }

        private void btnImportMp3File_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            FileName = OpenFileDialog.FileName;

            txtMp3File.Text = FileName;

        }

        private void btnImportSongINIFile_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "Ini files (*.ini)|*.ini|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            FileName = OpenFileDialog.FileName;
            txtSongINIFile.Text = FileName;
        }

        private void btnImportImage_Click(object sender, EventArgs e)
        {
            string FileName;
            OpenFileDialog.Filter = "Jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (OpenFileDialog.ShowDialog() != DialogResult.OK) return;
            FileName = OpenFileDialog.FileName;
            txtImageFile.Text = FileName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCreateKfn_Click(object sender, EventArgs e)
        {
            string Mp3FileName;
            string SongINIFileName;
            string ImageFileName;

            Mp3FileName = txtMp3File.Text.Trim();
            if (!File.Exists(Mp3FileName))
            {
                MessageBox.Show("Invalid mp3 file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }

            SongINIFileName = txtSongINIFile.Text.Trim();
            if (!File.Exists(SongINIFileName))
            {
                MessageBox.Show("Invalid Song.ini file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ImageFileName = txtImageFile.Text.Trim();
            if (ImageFileName != string.Empty && !File.Exists(ImageFileName))
            {
                MessageBox.Show("Invalid image file", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            CreateKfnFile(Mp3FileName, SongINIFileName, ImageFileName);
        }

        private void CreateKfnFile(string mp3FileName, string songINIFileName , string imageFileName)
        {

            // Create a kfn file using the name of the mp3 file ?
            string kfnFile = Path.ChangeExtension(mp3FileName, ".kfn");
            
            KfnWriter Writer = new KfnWriter(kfnFile, mp3FileName, songINIFileName, new List<string> { imageFileName });



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

            Writer.CreateKFN();
        }

    }
}
