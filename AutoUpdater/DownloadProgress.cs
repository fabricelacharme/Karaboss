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
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

namespace PrgAutoUpdater
{
    public partial class DownloadProgress : Form
    {
        #region The private fields
        private bool isFinished = false;
        private DownloadFileInfo downloadFileList = null;
        private DownloadFileInfo allFileList = null;
        private ManualResetEvent evtDownload = null;
        private ManualResetEvent evtPerDonwload = null;
        private WebClient clientDownload = null;
                
        private string AppName = string.Empty;
        private string newversion = string.Empty;
        private long newsize = 0;

        #endregion

        #region The constructor of DownloadProgress
        public DownloadProgress(DownloadFileInfo downloadFileListTemp, string tAppName, string tversion, long tsize)
        {
            AppName = tAppName;
            newversion = tversion;
            newsize = tsize;            

            InitializeComponent();

            this.label3.Text = "Updating " + AppName;
            this.label7.Text = "Name: " +  AppName;
            this.Text = "Updating " + AppName;

            this.downloadFileList = downloadFileListTemp;
            allFileList = new DownloadFileInfo(downloadFileListTemp.DownloadUrl, downloadFileListTemp.TargetUrl, downloadFileListTemp.FileName, downloadFileListTemp.LastVer, downloadFileListTemp.Size);

        }
        #endregion

        #region The method and event
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isFinished && DialogResult.No == MessageBox.Show(ConstFile.CANCELORNOT, ConstFile.MESSAGETITLE, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                e.Cancel = true;
                return;
            }
            else
            {
                if (clientDownload != null)
                    clientDownload.CancelAsync();

                evtDownload.Set();
                evtPerDonwload.Set();
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            evtDownload = new ManualResetEvent(true);
            evtDownload.Reset();
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ProcDownload));
        }

        long total = 0;
        long nDownloadedTotal = 0;

        private void ProcDownload(object o)
        {            

            evtPerDonwload = new ManualResetEvent(false);

            if (downloadFileList != null)
                total += downloadFileList.Size;


            try
            {
                while (!evtDownload.WaitOne(0, false))
                {

                    if (downloadFileList == null)
                        break;

                    DownloadFileInfo file = downloadFileList;

                    this.ShowCurrentDownloadFileName(file.FileName);

                    //Download
                    clientDownload = new WebClient();

                    //Added the function to support proxy
                    System.Net.IWebProxy iwpxy = WebRequest.GetSystemWebProxy();
                    clientDownload.Proxy = iwpxy;

                    //clientDownload.Proxy = WebProxy.GetDefaultProxy();


                    clientDownload.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    clientDownload.Credentials = System.Net.CredentialCache.DefaultCredentials;
                    //End added


                    // Progress changed
                    clientDownload.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                    {
                        try
                        {
                            this.SetProcessBar(e.ProgressPercentage, (int)((nDownloadedTotal + e.BytesReceived) * 100 / total));
                        }
                        catch
                        {
                            //log the error message,you can use the application's log code
                        }

                    };


                    // Download completed
                    clientDownload.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                    {
                        try
                        {
                           
                            DownloadFileInfo dfile = e.UserState as DownloadFileInfo;
                            nDownloadedTotal += dfile.Size;
                            this.SetProcessBar(0, (int)(nDownloadedTotal * 100 / total));
                            evtPerDonwload.Set();
                        }
                        catch (Exception)
                        {
                            //log the error message,you can use the application's log code
                        }

                    };

                    evtPerDonwload.Reset();

                    //Download                 

                    // Telecharge le fichier dans le repertoire downloads
                    clientDownload.DownloadFileAsync(new Uri(file.DownloadUrl), Path.Combine(file.TargetUrl, file.FileName), file);

                    //Wait for the download complete
                    evtPerDonwload.WaitOne();

                    clientDownload.Dispose();
                    clientDownload = null;

                    downloadFileList = null;


                }

            }
            catch (Exception)
            {
                ShowErrorAndRestartApplication();
                //throw;
            }

            //After dealed with all files, clear the data
            allFileList = null;

            if (downloadFileList == null)
                Exit(true);
            else
                Exit(false);

            evtDownload.Set();
        }

 
        delegate void ShowCurrentDownloadFileNameCallBack(string name);
        private void ShowCurrentDownloadFileName(string name)
        {
            if (this.labelCurrentItem.InvokeRequired)
            {
                ShowCurrentDownloadFileNameCallBack cb = new ShowCurrentDownloadFileNameCallBack(ShowCurrentDownloadFileName);
                this.Invoke(cb, new object[] { name });
            }
            else
            {
                this.labelCurrentItem.Text = name;
            }
        }

        delegate void SetProcessBarCallBack(int current, int total);
        private void SetProcessBar(int current, int total)
        {
            if (this.progressBarCurrent.InvokeRequired)
            {
                SetProcessBarCallBack cb = new SetProcessBarCallBack(SetProcessBar);
                this.Invoke(cb, new object[] { current, total });
            }
            else
            {
                if (current <= progressBarCurrent.Maximum)
                    progressBarCurrent.Value = current;

                if (total <= progressBarTotal.Maximum)
                    progressBarTotal.Value = total;
            }
        }

        delegate void ExitCallBack(bool success);
        private void Exit(bool success)
        {
            if (this.InvokeRequired)
            {
                ExitCallBack cb = new ExitCallBack(Exit);
                this.Invoke(cb, new object[] { success });
            }
            else
            {
                this.isFinished = success;
                this.DialogResult = success ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            }
        }

        private void OnCancel(object sender, EventArgs e)
        {   
            ShowErrorAndRestartApplication();
        }

 

        private void ShowErrorAndRestartApplication()
        {
            MessageBox.Show(ConstFile.NOTNETWORK,ConstFile.MESSAGETITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);
            //CommonUnitity.RestartApplication();
            CommonUnitity.StopApplication();
        }

        #endregion
    }
}