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
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Xml.Linq;

namespace PrgAutoUpdater
{
    #region The delegate
    public delegate void ShowHandler();
    #endregion

    public class AutoUpdater : IAutoUpdater
    {

        #region The private fields
        private Config config = null;

        private LocalFile localfile = null;

        private bool bNeedRestart = false;
        private bool bDownload = false;
        #endregion


        #region The public event
        public event ShowHandler OnShow;
        #endregion

        private string AppName = string.Empty;
        private Version version = null;
        private string newversion = string.Empty;

        private long size = 0;
        private long newsize = 0;

        #region properties
        // FAB 30/9/17
        private string _serverUrl = string.Empty;
        
        #endregion

        #region The constructor of AutoUpdater

        public AutoUpdater(string tAppName, Version tversion, long tsize, string tRemoteUrl)
        {

            AppName = tAppName;
            version = tversion;
            size = tsize;            

            // Load local config file AutoUpdater.config
            string AppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);


            // Check existence of file !! before
            string file = Path.Combine(AppDataDir, ConstFile.FILENAME);
            FileInfo F = new FileInfo(file);
            if (F.Exists == false)
            {
                config = new Config();
                config.ServerUrl = tRemoteUrl;          // File on remote server
                config.Enabled = true;

                config.SaveConfig(file);
                config = null;
            }

            // Config file
            config = Config.LoadConfig(file);

            // current program set with old values version & size
            localfile = new LocalFile();
            localfile.LastVer = version.ToString();
            localfile.Size = size;                    
        }

        #endregion


        #region The public method
        public void Update()
        {
            if (!config.Enabled)
                return;

            // Retrieve list of remote files to update
            Dictionary<string, RemoteFile> listRemotFile = ParseRemoteXml(config.ServerUrl);




            // Create download list                        
            DownloadFileInfo downloadList = null;

            if (listRemotFile.ContainsKey(AppName))
            {
                RemoteFile rf = listRemotFile[AppName];

                // FAB
                this._serverUrl = rf.Url;
                Uri uri = new Uri(@rf.Url);
                _serverUrl = "http://" + uri.Host + "/AutoupdateService.xml";

                Version v1 = new Version(rf.LastVer);
                Version v2 = new Version(localfile.LastVer);
                if (v1 > v2)
                {
                    // Add to download list all files having a greater version
                    downloadList = new DownloadFileInfo(rf.Url, localfile.Path, rf.Url, rf.LastVer, rf.Size);                    
                    localfile.LastVer = rf.LastVer;
                    localfile.Size = rf.Size;

                    newversion = rf.LastVer;
                    newsize = rf.Size;

                    if (rf.NeedRestart)
                        bNeedRestart = true;

                    bDownload = true;
                }
                // Remove from list
                listRemotFile.Remove(AppName);
            }
           

            

            // Display form download confirm
            if (bDownload)
            {
                DownloadConfirm dc = new DownloadConfirm(downloadList, AppName, newversion, newsize);

                if (this.OnShow != null)
                    this.OnShow();

                if (DialogResult.OK == dc.ShowDialog())
                {
                    StartDownload(downloadList);
                }
            }

        }

        public string getRemoteUrl()
        {
            return _serverUrl;
        }
        #endregion


        #region The private method
        string newfilepath = string.Empty;

        /// <summary>
        /// Retrieve informations in the remote file AutoupdateService.xml
        /// <?xml version="1.0" encoding="utf-8"?>
        /// <Config xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        ///   <LocalFile app = "Karaboss" url="http://ds.lacharme.net/pub/karaboss1.0.1.1-setup.exe" lastver="1.0.1.2" size="933136" needRestart="true" />
        /// </Config>
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        private Dictionary<string, RemoteFile> ParseRemoteXml(string xml)
        {

            Dictionary<string, RemoteFile> list = new Dictionary<string, RemoteFile>();

            try
            {
                XmlTextReader rd;
                HttpWebRequest wrq;

                wrq = (HttpWebRequest)WebRequest.Create(xml);

                System.Net.IWebProxy iwpxy = WebRequest.GetSystemWebProxy();
                wrq.Proxy = iwpxy;                            
                wrq.Proxy.Credentials = CredentialCache.DefaultCredentials;

                rd = new XmlTextReader(wrq.GetResponse().GetResponseStream());

                XmlDocument document = new XmlDocument();
                document.Load(xml);

                foreach (XmlNode node in document.DocumentElement.ChildNodes)
                {
                    list.Add(node.Attributes["app"].Value, new RemoteFile(node));
                }
            }
            catch (Exception err)
            {
                Console.Write(err.Message);
                string tx = "Error when trying to load remote file: " + xml + "\n";
                tx += err.Message;
                MessageBox.Show(tx);
            }

            return list;
        }

        /// <summary>
        /// Download program
        /// </summary>
        /// <param name="downloadList"></param>
        private void StartDownload(DownloadFileInfo downloadList)
        {
            DownloadProgress dp = new DownloadProgress(downloadList, AppName, newversion, newsize);
            if (dp.ShowDialog() == DialogResult.OK)
            {
                //
                if (DialogResult.Cancel == dp.ShowDialog())
                {
                    return;
                }           

                if (bNeedRestart)
                {
                    MessageBox.Show(ConstFile.APPLYTHEUPDATE, ConstFile.MESSAGETITLE, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Launch setup of application downloaded
                    string setupexe = Path.Combine(downloadList.TargetUrl, downloadList.FileName);
                    System.Diagnostics.Process.Start(setupexe);

                    // Stop this instance of the application
                    CommonUnitity.StopApplication();
                }
            }
        }

        #endregion

    }
}
