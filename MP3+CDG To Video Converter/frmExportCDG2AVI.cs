#region License

/* Copyright (c) 2016 Fabrice Lacharme
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
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace MP3GConverter
{
    public partial class frmExportCDG2AVI : Form
    {

        #region "Private Declarations"

        //private CDGFile mCDGFile;
        //private CdgFileIoStream mCDGStream;
        private string mCDGFileName;
        private string mMP3FileName;
        private string mTempDir;
        private ExportAVI mExportAVI;

        #endregion

        public frmExportCDG2AVI()
        {
            InitializeComponent();

            mExportAVI = new ExportAVI();
            //mExportAVI.Status += new ExportAVI.StatusChangedEventHandler(mExportAVI_Status);
            mExportAVI.Status += new StatusChangedEventHandler(mExportAVI_Status);

        }



        #region "Control Events"

        private void btOutputAVI_Click(object sender, System.EventArgs e)
        {
            SelectOutputAVI();
        }


        private void btBackGroundBrowse_Click(System.Object sender, System.EventArgs e)
        {
            SelectBackGroundAVI();
        }

        private void btConvert_Click(System.Object sender, System.EventArgs e)
        {
            ConvertAVI();
        }

        private void tbFPS_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (((Keys)e.KeyChar >= Keys.D0 && (Keys)e.KeyChar <= Keys.D9) || ((Keys)e.KeyChar == Keys.Back) || (e.KeyChar.ToString() == "."))
            {
                e.Handled = false;
            }
            else {
                e.Handled = true;
            }
        }

        private void btBrowseCDG_Click(System.Object sender, System.EventArgs e)
        {
            OpenFileDialog1.Filter = "CDG or Zip Files (*.zip, *.cdg)|*.zip;*.cdg";
            OpenFileDialog1.ShowDialog();
            tbFileName.Text = OpenFileDialog1.FileName;
        }

        private void chkBackGraph_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkBackGround.Checked && chkBackGraph.Checked)
            {
                chkBackGround.Checked = false;
            }
            ToggleCheckBox();
        }

        private void chkBackGround_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (chkBackGraph.Checked && chkBackGround.Checked)
            {
                chkBackGraph.Checked = false;
            }
            ToggleCheckBox();
        }

        private void btBrowseImg_Click(System.Object sender, System.EventArgs e)
        {
            SelectBackGroundGraphic();
        }

        #endregion


        #region "Events"

        private void mExportAVI_Status(string message)
        {
            pbAVI.Value = (Convert.ToInt32(message));
        }

        #endregion


        #region "Private Methods"

        private void SelectOutputAVI()
        {
            SaveFileDialog1.Filter = "AVI Files (*.avi)|*.avi";
            SaveFileDialog1.ShowDialog();
            tbAVIFile.Text = SaveFileDialog1.FileName;
        }

        private void SelectBackGroundAVI()
        {
            OpenFileDialog1.Filter = "Movie Files (*.avi, *.mpg, *.wmv)|*.avi;*.mpg;*.wmv";
            OpenFileDialog1.ShowDialog();
            tbBackGroundAVI.Text = OpenFileDialog1.FileName;
        }

        private void SelectBackGroundGraphic()
        {
            OpenFileDialog1.Filter = "Graphic Files|*.jpg;*.bmp;*.png;*.tif;*.tiff;*.gif;*.wmf";
            OpenFileDialog1.ShowDialog();
            tbBackGroundImg.Text = OpenFileDialog1.FileName;
        }

        private void ConvertAVI()
        {
            try
            {
                PreProcessFiles();
                if (string.IsNullOrEmpty(mCDGFileName) | string.IsNullOrEmpty(mMP3FileName))
                {
                    MessageBox.Show("Cannot find a CDG and MP3 file to convert together.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            mExportAVI = new ExportAVI();
            pbAVI.Value = 0;
            string backGroundFilename = "";
            if (chkBackGraph.Checked)
                backGroundFilename = tbBackGroundImg.Text;
            if (chkBackGround.Checked)
                backGroundFilename = tbBackGroundAVI.Text;
            mExportAVI.CDGtoAVI(tbAVIFile.Text, mCDGFileName, mMP3FileName, Convert.ToDouble(tbFPS.Text), backGroundFilename);
            pbAVI.Value = 0;
            try
            {
                CleanUp();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void CleanUp()
        {
            if (!string.IsNullOrEmpty(mTempDir))
            {
                try
                {
                    Directory.Delete(mTempDir, true);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            mTempDir = "";
        }

        private void PreProcessFiles()
        {
            string myCDGFileName = "";
            if (Regex.IsMatch(tbFileName.Text, "\\.zip$"))
            {
                string myTempDir = Path.GetTempPath() + Path.GetRandomFileName();
                Directory.CreateDirectory(myTempDir);
                mTempDir = myTempDir;
                myCDGFileName = Unzip.UnzipMP3GFiles(tbFileName.Text, myTempDir);
                //goto PairUpFiles;
                string myMP3FileName = Regex.Replace(myCDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(myMP3FileName))
                {
                    mMP3FileName = myMP3FileName;
                    mCDGFileName = myCDGFileName;
                    mTempDir = "";
                }


            }
            else if (Regex.IsMatch(tbFileName.Text, "\\.cdg$"))
            {
                myCDGFileName = tbFileName.Text;


                //PairUpFiles:
                string myMP3FileName = Regex.Replace(myCDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(myMP3FileName))
                {
                    mMP3FileName = myMP3FileName;
                    mCDGFileName = myCDGFileName;
                    mTempDir = "";
                }


            }
        }


        private void ToggleCheckBox()
        {
            tbBackGroundAVI.Enabled = chkBackGround.Checked;
            btBackGroundBrowse.Enabled = chkBackGround.Checked;
            tbBackGroundImg.Enabled = chkBackGraph.Checked;
            btBrowseImg.Enabled = chkBackGraph.Checked;
        }

        #endregion

       
    }
}
