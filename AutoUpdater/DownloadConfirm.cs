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

namespace PrgAutoUpdater
{
    public partial class DownloadConfirm : Form
    {
        #region The private fields
        private string AppName = string.Empty;
        private string newversion = string.Empty;
        private long newsize = 0;

        DownloadFileInfo downloadFileList = null;
        #endregion

        #region The constructor of DownloadConfirm
        public DownloadConfirm(DownloadFileInfo downloadfileList, string tAppName, string tversion, long tsize)
        {
            AppName = tAppName;
            newversion = tversion;
            newsize = tsize;

            InitializeComponent();

            downloadFileList = downloadfileList;

            this.label2.Text = "A new version of " + AppName + " is available: (" + newversion.ToString() + ")";
            this.label3.Text = "Name:  " + AppName;

        }
        #endregion

        #region The private method
        private void OnLoad(object sender, EventArgs e)
        {
            ListViewItem item = new ListViewItem(new string[] { downloadFileList.FileName, downloadFileList.LastVer, downloadFileList.Size.ToString() });

            this.Activate();
            this.Focus();
        }
        #endregion
    }
}