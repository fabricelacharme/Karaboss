#region License

/* Copyright (c) 2018 Fabrice Lacharme
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
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using PrgAutoUpdater;
using System.Net;
using System.Xml;
using System.IO;
using Si;
using System.Linq;

namespace Karaboss
{
    static class Program
    {

        /*
         * The program checks if there is an instance already running and send it the arguments
         * otherwise, 
         * it selects the appropriate language
         * it checks if a new version is available on http://karaoke.lacharme.net
         * it displays a splash screen
         * it displays the first form: the explorer (frmExplorer) 
         * 
         */

        // Splashscreen of application
        public static frmSplashScreen frmSplashScreen = null;

        // Language
        private class State
        {
            public CultureInfo Result { get; set; }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {            
            // Unique GUID to identify if application is already launched
            Guid guid = new Guid("{B5357686-DD48-4E98-85FB-8F0476419F73}");
            using (SingleInstance singleInstance = new SingleInstance(guid))
            {                                                
                // If application is not running, this is the first instance
                if (singleInstance.IsFirstInstance)
                {
                    singleInstance.ArgumentsReceived += SingleInstance_ArgumentsReceived;
                    singleInstance.ListenForArgumentsFromSuccessiveInstances();            

                    NormalLoadind(args, 1);
                }
                else
                {
                    // Application is already running, send arguments to the existing instance
                    if (args != null)
                    {
                        if (Control.ModifierKeys == Keys.Shift)
                        {                            
                            NormalLoadind(args, 2);
                        }
                        else
                        {
                            singleInstance.PassArgumentsToFirstInstance(args);
                        }
                    }
                }
            }
        }


        static void NormalLoadind(string[] args, int numinstance)
        {
            #region normal loading

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Retrieve previous settings if update
            // (settings are lost if new version (thanks to MS)
            #region restore previous settings if app update

            PrgAutoUpdater.Util.DoUpgrade(Properties.Settings.Default);

            #endregion

            // Select language
            #region culture
            // Get current culture
            string m_lang = Properties.Settings.Default.lang;

            if (m_lang == "Undefined") //default value of Karaboss
            {
                ///The ClearCachedData method does not refresh the information in the Thread.CurrentCulture property for existing threads
                ///So you will need to first call the function and then start a new thread.
                ///In this new thread you can use the CurrentCulture to obtain the fresh values of the culture.
                Thread.CurrentThread.CurrentCulture.ClearCachedData();
                var thread = new Thread(
                    s => ((State)s).Result = Thread.CurrentThread.CurrentCulture);
                thread.IsBackground = true;

                var state = new State();
                thread.Start(state);
                thread.Join();
                var culture = state.Result;
                if (culture.IetfLanguageTag == "fr-FR")
                {
                    m_lang = "Français";
                }
                else
                {
                    m_lang = "English";
                }

                Properties.Settings.Default.lang = m_lang;
                Properties.Settings.Default.Save();

            }

            // Change the culture for the App domain
            // Allow to manage languages for usercontrols
            switch (m_lang)
            {
                case "English":
                    RuntimeLocalizer.ChangeCulture("en-US");
                    break;
                case "Français":
                    RuntimeLocalizer.ChangeCulture("fr-FR");
                    break;
                default:
                    RuntimeLocalizer.ChangeCulture("en-US");
                    break;
            }            

          


            #endregion

            // check if update is available
            #region check and download new version program
            bool bCheckUpdates = Properties.Settings.Default.CheckForUpdates;

            Configuration.UpdFrequency UpdateFrequency = new Configuration.UpdFrequency(Properties.Settings.Default.UpdFrequency);

            // If check for update && date is overcome
            if (bCheckUpdates == true && UpdateFrequency.searchForUpdate())
            {

                // Save last check date
                UpdateFrequency.saveUpdDate(DateTime.Now);

                bool bHasError = false;

                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                string fileName = System.Reflection.Assembly.GetEntryAssembly().Location;
                FileInfo f = new FileInfo(fileName);
                long size = f.Length;

                string RemoteUrl = Properties.Settings.Default.RemoteUrl;

                IAutoUpdater autoUpdater = new AutoUpdater(Application.ProductName, version, size, RemoteUrl);

                try
                {
                    autoUpdater.Update();

                    // check if remote URL has changed
                    string url = autoUpdater.getRemoteUrl();
                    if (url.ToLower() != RemoteUrl.ToLower())
                    {
                        Properties.Settings.Default.RemoteUrl = url;
                        Properties.Settings.Default.Save();
                    }
                }
                catch (WebException exp)
                {
                    MessageBox.Show("Can not find the specified resource. " + exp.Message);
                    bHasError = true;
                }
                catch (XmlException exp)
                {
                    bHasError = true;
                    MessageBox.Show("Download the upgrade file error. " + exp.Message);
                }
                catch (NotSupportedException exp)
                {
                    bHasError = true;
                    MessageBox.Show("Upgrade address configuration error. " + exp.Message);
                }
                catch (ArgumentException exp)
                {
                    bHasError = true;
                    MessageBox.Show("Download the upgrade file error. " + exp.Message);
                }
                catch (Exception exp)
                {
                    bHasError = true;
                    MessageBox.Show("An error occurred during the upgrade process. " + exp.Message);
                }
                finally
                {
                    if (bHasError == true)
                    {
                        try
                        {
                            //autoUpdater.RollBack();
                        }
                        catch (Exception)
                        {
                            //Log the message to your file or database
                        }
                    }

                }

            } // bCheckUpdates
            #endregion


            //show splash screen
            #region splash

            Thread splashThread = new Thread(new ThreadStart(
                delegate
                {
                    frmSplashScreen = new frmSplashScreen();
                    Application.Run(frmSplashScreen);
                }
                ));
            splashThread.IsBackground = true;

            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();
            #endregion

            // Display main form
            frmExplorer frmExplorer = new frmExplorer(args, numinstance);
            frmExplorer.Load += new EventHandler(frmExplorer_Load);

            frmExplorer.Show();
            frmExplorer.Activate();
            frmExplorer.BringToFront();

            Application.Run(frmExplorer);
            #endregion
        }


        /// <summary>
        /// Form frmExplorer: explorer
        /// </summary>
        static frmExplorer form;

        /// <summary>
        /// Event treatment of singleInstance.ArgumentsReceived
        /// Send the arguments to form frmExplorer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SingleInstance_ArgumentsReceived(object sender, ArgumentsReceivedEventArgs e)
        {
            
            form  = GetForm<frmExplorer>();
            if (form == null)
                return;

            try
            {
                Action<String[]> updateForm = arguments =>
                {
                    form.WindowState = FormWindowState.Normal;
                    form.UseNewArguments(arguments);
                };
                form.Invoke(updateForm, (Object)e.Args); //Execute our delegate on the forms thread!
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// Event tratment: unload splash screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void frmExplorer_Load(object sender, EventArgs e)
        {
            //close splash
            if (frmSplashScreen == null)            
                return;            

            frmSplashScreen.Invoke(new Action(frmSplashScreen.Close));
            frmSplashScreen.Dispose();
            frmSplashScreen = null;
        }


        /// <summary>
        /// Return if a form exists
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        static TForm GetForm<TForm>()
        where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }
    }
}