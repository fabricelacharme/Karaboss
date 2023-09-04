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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class frmToools : Form
    {

        public class MFile
        {
            
            public MFile(string path, string filename, string song ,string md5, long size)
            {
                this._path = path;              // Full path
                this._filename = filename;      // file with extension (artist - song.kar)
                this._song = song;              // only song
                this._md5 = md5;
                this._size = size;
            }
            

            public string _path { get; set; }
            public string _filename { get; set; }
            public string _song { get; set; }
            public string _md5 { get; set; }
            public long _size { get; set; }
        }

        public class MyCollection
        {
            public class CurrentState
            {
                public int LinesCounted;
                public int LinesTotal;
                public string status;
                public string task;
            }

            public List<MFile> lstMyFiles;
            public string path;
         
            public void Create(BackgroundWorker worker, DoWorkEventArgs e)
            {

                // Initialize the variables.
                CurrentState state = new CurrentState();

                string fpath = string.Empty;
                string filename = string.Empty;
                string song = string.Empty;
                long size = 0;
                string md5 = string.Empty;


                // Get all files recursively in path
                state.task = "Searching recursively files in path " + path;
                state.LinesCounted = 0;
                worker.ReportProgress(0, state);

                List<string> lstf = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => file.ToLower().EndsWith("mid") || file.ToLower().EndsWith("kar"))
                .ToList();

                // Report the final count values. 
                state.task = lstf.Count + " files found in path " + path;               
                state.LinesCounted = lstf.Count;
                worker.ReportProgress(0, state);

                // redimension lstMyFiles
                lstMyFiles = new List<MFile>();

                // Get informations for each file
                state.LinesCounted = 0;
                state.LinesTotal = lstf.Count;
                state.task = "Searching files informations for path " + path;

                int elapsedTime = 20;
                DateTime lastReportDateTime = DateTime.Now;                

                for (int i = 0; i < lstf.Count; i++)
                {
                    fpath = lstf[i].Trim().ToLower();

                    try
                    {
                        FileInfo fi = new FileInfo(fpath);
                        size = fi.Length;
                        filename = fi.Name.ToLower();

                        int pos = filename.IndexOf("- ") + 2;
                        if (pos > 0)
                            song = filename.Substring(pos);
                        else
                            song = filename;

                        MFile data = new MFile(fpath, filename, song, GetMD5(fpath), size);
                        lstMyFiles.Add(data);


                        // Raise an event so the form can monitor progress.
                        int compare = DateTime.Compare(DateTime.Now, lastReportDateTime.AddMilliseconds(elapsedTime));
                        if (compare > 0)
                        {
                            state.LinesCounted = i;
                            worker.ReportProgress(0, state);
                            lastReportDateTime = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write(fpath + "\n" + ex.Message);
                    }

                }

                // Report the final count values.                
                state.LinesCounted = lstf.Count;
                state.task = "End of task";          
                worker.ReportProgress(0, state);
            }

            // Calculate MD5 for a file
            private string GetMD5(string filename)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream));
                    }
                }
            }
        }


        #region declarations
        private string referencepath;
        private string selectedpath;
        private string Rtfexplanation =string.Empty;
        private string SearchMode;

        #region Lists
        private List<MFile> listRefFiles;
        private List<MFile> listSelFiles;   
        
        private List<string> lstDoubles;
        #endregion List

        private BackgroundWorker bwGetCOLLECTION;   
        private MyCollection MC;
      

        private string Context = string.Empty;

        private string CR = @" \line ";
        private string BOLD0 = @" \b ";
        private string BOLD1 = @" \b0 ";

        #endregion


        public frmToools(string refpath, string selpath, string SMode)
        {
            InitializeComponent();

            rtBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            referencepath = refpath.Trim().ToLower();
            selectedpath = selpath.Trim().ToLower(); ;
            SearchMode = SMode;

            btnCancel.Enabled = false;

            // Explanation of what the function does
            switch (SearchMode) {
                case "SearchMD5Doubles":
                    #region MD5
                    if (referencepath != selectedpath)
                    {
                        // Case of search doubles in another directory and compare to reference directory
                        string tx = BOLD0 + "Search MD5 doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their MD5 value in your default songs directory:" + CR;
                        tx += BOLD0 + referencepath.Replace('\\','/') + BOLD1 + CR + CR;
                        tx += "and the directory that you have selected:" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them";
                        tx += ", either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under it.";
                        Rtfexplanation = tx;
                    }
                    else
                    {
                        // Case of search doubles in a single directory
                        string tx = BOLD0 + "Search MD5 doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their MD5 value in the selected directory" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them, either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under this directory.";
                        Rtfexplanation = tx;

                    }
                    #endregion
                    break;

                case "SearchSizeDoubles":
                    #region size
                    if (referencepath != selectedpath)
                    {
                        // Case of search doubles in another directory and compare to reference directory
                        string tx = BOLD0 + "Search SIZE doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their size value in your default songs directory" + CR;
                        tx += BOLD0 + referencepath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "and the directory that you have selected:" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them, either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under it.";
                        Rtfexplanation = tx;

                    }
                    else
                    {
                        // Case of search doubles in a single directory
                        string tx = BOLD0 + "Search SIZE doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their size value in the selected directory" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them, either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under this directory.";
                        Rtfexplanation = tx;
                    }
                    #endregion
                    break;

                case "SearchNameDoubles":
                    #region name
                    if (referencepath != selectedpath)
                    {
                        // Case of search doubles in another directory and compare to reference directory
                        string tx = BOLD0 + "Search NAME doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their name in your default songs directory" + CR;
                        tx += BOLD0 + referencepath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "and the directory that you have selected:" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them, either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under it.";
                        Rtfexplanation = tx;

                    }
                    else
                    {
                        // Case of search doubles in a single directory
                        string tx = BOLD0 + "Search NAME doubles" + BOLD1 + CR + CR;
                        tx += "This function will search for double files using their name in the selected directory" + CR;
                        tx += BOLD0 + selectedpath.Replace('\\', '/') + BOLD1 + CR + CR;
                        tx += "If double files are found, you will have the choice, either to display them, either to moved them from the selected directory ";
                        tx += "to a directory called 'Doubles' under this directory.";
                        Rtfexplanation = tx;
                    }
                    #endregion
                    break;

                default:
                    break;
            }               
            DisplayExplanation(Rtfexplanation);
        }


        #region backgroundworker


        #region getLstFiles

        private void StartThread_getCollection(string path)
        {
            this.bwGetCOLLECTION = new BackgroundWorker();
            this.bwGetCOLLECTION.DoWork += new DoWorkEventHandler(BwGetCOLLECTION_DoWork);
            this.bwGetCOLLECTION.ProgressChanged += new ProgressChangedEventHandler(BwGetCOLLECTION_ProgressChanged);
            this.bwGetCOLLECTION.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BwGetCOLLECTION_RunWorkerCompleted);
            this.bwGetCOLLECTION.WorkerReportsProgress = true;

            MC = new MyCollection()
            {
                path = path,
            };

            bwGetCOLLECTION.RunWorkerAsync(MC); 
        }
     
        private void BwGetCOLLECTION_DoWork(object sender, DoWorkEventArgs e)
        {
            // This event handler is where the actual work is done.
            // This method runs on the background thread.
            BackgroundWorker worker = (BackgroundWorker)sender;

            // Get the BackgroundWorker object that raised this event.
            MC = (MyCollection)e.Argument;
            MC.Create(worker, e);
        }

        private void BwGetCOLLECTION_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MyCollection.CurrentState state = (MyCollection.CurrentState)e.UserState;
            lblCurTask.Text = state.task;
            lblStatus.Text = state.LinesCounted.ToString() + "/" + state.LinesTotal.ToString() ;
        }

        private void BwGetCOLLECTION_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This event handler is called when the background thread finishes.
            // This method runs on the main thread.
            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message);
            else if (e.Cancelled)
                MessageBox.Show("Collection generation canceled.");
            else
            {
                // Finished generation list of files
                if (Context == "get files for reference")
                {
                    listRefFiles = MC.lstMyFiles;
                    
                    // Suite
                    if (selectedpath != referencepath)
                    {
                        Context = "get files for selected";
                        StartThread_getCollection(selectedpath);
                    }
                    else
                    {
                        // Suite
                        SuiteJob();
                    }
                } 
                else if (Context == "get files for selected")
                {
                    listSelFiles = MC.lstMyFiles;
                    
                    // Suite
                    SuiteJob();
                }
            }
        }

        #endregion getLstFiles

        #endregion

   
        private void DisplayExplanation(string tx)
        {
            //lblExplanation.Text = tx;
            //rtBox.Text = tx;

            //@"{\rtf1\ansi This is in \b bold\b0.}";
            rtBox.Rtf = @"{\rtf1\ansi " + tx + "} ";
        }


        #region buttons

        private void BtnLaunch_Click(object sender, EventArgs e)
        {
            // Check if not a subdirectory is selected
            if (StartPrepare())
            {
                btnLaunch.Enabled = false;
                btnCancel.Enabled = true;
                btnQuit.Enabled = false;

                StartJob(referencepath, selectedpath);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            btnLaunch.Enabled = true;
            btnQuit.Enabled = true;
        }

        private void BtnQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
        
        private bool StartPrepare()
        {
            if (referencepath != selectedpath && (referencepath.IndexOf(selectedpath) == 0 || selectedpath.IndexOf(referencepath) == 0))
            {
                string tx = "Path cannot be a sub directory because all files will be doubles!";
                MessageBox.Show(tx);
                return false;
            }

            // Convert to plain text
            string Explanation = rtBox.Text;            

            if (MessageBox.Show(Explanation, "Karaboss", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                return true;
            }
            return false;
        }

        private void StartJob(string refpath, string selpath)
        {
            Context = "get files for reference";            
            StartThread_getCollection(refpath);
        }

        private void SuiteJob()
        {
            switch (SearchMode)
            {
                case "SearchMD5Doubles":
                    this.lblStatus.Text = "Comparing MD5 in progress...";
                    CompareMD5(referencepath, selectedpath);
                    break;

                case "SearchSizeDoubles":
                    CompareSize(referencepath, selectedpath);
                    break;

                case "SearchNameDoubles":
                    CompareName(referencepath, selectedpath);
                    break;
                default:
                    break;
            }

            this.lblStatus.Text = "Move or display doubles in progress...";
            MoveDoublesQuestion(referencepath, selectedpath);

            this.lblStatus.Text = "Job completed";
            EndOfJob();
        }

        private void EndOfJob()
        {
            btnLaunch.Enabled = true;
            btnCancel.Enabled = false;
            btnQuit.Enabled = true;
        }

        #region functions

        #region MD5
        
        /// <summary>
        /// Build list lstDoubles by searching duplicates MD5
        /// </summary>
        /// <param name="refpath"></param>
        /// <param name="selpath"></param>
        private void CompareMD5(string refpath, string selpath)
        {
            int nb = 0;
            int idx = 0;

            lstDoubles = new List<string>();

            if (selpath != refpath)
            {
                // Case of comparison between reference directory with a second directory
                // => delete all doubles in the second directory

                for (int i = 0; i < listSelFiles.Count; i++)
                {
                    if (listRefFiles.Any(F => F._md5 == listSelFiles[i]._md5) )
                    {
                        lstDoubles.Add(listSelFiles[i]._path);
                        nb++;
                    }
                }                            
            }
            else
            {
                // Single directory choosen
                // we must keep one file and eliminate all doubles                

                // cherche les doubles dans une seule liste (la liste de référence)
                List<string> lstDoublesMD5 = ExFindDuplicates(listRefFiles, p => p._md5);
               
                // Amélioration :
                // Conserver celui qui a le nom le plus long au lieu de se contenter du 1er trouvé
                List<MFile> tmpl = new List<MFile>();

                for (int i = 0; i < lstDoublesMD5.Count; i++)
                {
                    // On garni la liste avec tous les doubles
                    tmpl.Clear();

                    // Search for all doubles

                    // 1st double, not counted
                    idx = listRefFiles.FindIndex(item => item._md5 == lstDoublesMD5[i]);
                    tmpl.Add(listRefFiles[idx]);
                    do
                    {
                        idx = listRefFiles.FindIndex(idx + 1, item => item._md5 == lstDoublesMD5[i]);
                        if (idx >= 0)
                        {
                            // Store in tmpl the other occurences
                            tmpl.Add(listRefFiles[idx]);
                            nb++;               // count other doubles, except the first 
                        }
                    } while (idx >= 0);

                    // On trie la liste par ordre de longeur de fichier                    
                    var newlist = tmpl.OrderByDescending(x => x._filename.Length).ToList();
                    
                    // On garni la liste de doubles
                    // the first double is nenamed with "org-"
                    for (int j = 0; j < newlist.Count; j++)
                    {
                        if (j == 0)
                            lstDoubles.Add("\norg-" + newlist[j]._path);
                        else
                            lstDoubles.Add(newlist[j]._path);
                    }
                }                
            }
            Console.Write("Doubles found = " + nb + "\n");
        }

        #endregion


        #region Size

        /// <summary>
        /// Build list lstDoubles by searching duplicates Size
        /// </summary>
        /// <param name="refpath"></param>
        /// <param name="selpath"></param>
        private void CompareSize(string refpath, string selpath)
        {
            int nb = 0;

            lstDoubles = new List<string>();

            if (selpath != refpath)
            {
                // Case of comparison between reference directory with a second directory
                // => delete all doubles in the second directory

                for (int i = 0; i < listSelFiles.Count; i++)
                {
                    if (listRefFiles.Any(F => F._size == listSelFiles[i]._size))
                    {
                        lstDoubles.Add(listSelFiles[i]._path);
                        nb++;
                    }
                }

            }
            else
            {
                // Single directory choosen
                // we must keep one file and eliminate all doubles                

                // cherche les doubles dans la liste de référence
                List<long> lstDoublesSIZE = ExFindDuplicates(listRefFiles, p => p._size);
                for (int i = 0; i < lstDoublesSIZE.Count; i++)
                {
                    // keep 1st occurence 
                    int idx = listRefFiles.FindIndex(item => item._size == lstDoublesSIZE[i]);

                    lstDoubles.Add("\norg-" + listRefFiles[idx]._path);

                    // Search for all other doubles
                    do
                    {
                        idx = listRefFiles.FindIndex(idx + 1, item => item._size == lstDoublesSIZE[i]);
                        if (idx >= 0)
                        {
                            // Store in lstdoubles the occuences
                            lstDoubles.Add(listRefFiles[idx]._path);
                            nb++;
                        }

                    } while (idx >= 0);

                }

            }

            Console.Write("Doubles found = " + nb + "\n");

        }

        #endregion


        #region Name

        private void CompareName(string refpath, string selpath)
        {
            int nb = 0;

            lstDoubles = new List<string>();

            if (selpath != refpath)
            {
                // Case of comparison between reference directory with a second directory
                // => delete all doubles in the second directory

                for (int i = 0; i < listSelFiles.Count; i++)
                {
                    if (listRefFiles.Any(F => F._song == listSelFiles[i]._song))
                    {
                        lstDoubles.Add(listSelFiles[i]._path);
                        nb++;
                    }
                }

            }
            else
            {
                // Single directory choosen
                // we must keep one file and eliminate all doubles                

                // cherche les doubles dans la liste de référence
                List<string> lstDoublesName = ExFindDuplicates(listRefFiles, p => p._song);
                for (int i = 0; i < lstDoublesName.Count; i++)
                {
                    // keep 1st occurence 
                    int idx = listRefFiles.FindIndex(item => item._song == lstDoublesName[i]);

                    lstDoubles.Add("\norg-" + listRefFiles[idx]._path);

                    // Search for all other doubles
                    do
                    {
                        idx = listRefFiles.FindIndex(idx + 1, item => item._song == lstDoublesName[i]);
                        if (idx >= 0)
                        {
                            // Store in lstdoubles the occuences
                            lstDoubles.Add(listRefFiles[idx]._path);
                            nb++;
                        }

                    } while (idx >= 0);

                }
            }
        }
        #endregion

        // var duplicateNames = list.exFindDuplicates(p => p.Name);
        //var duplicateAges = list.exFindDuplicates(p => p.Age);
        public List<U> ExFindDuplicates<T, U>(List<T> list, Func<T, U> keySelector)
        {
            return list.GroupBy(keySelector)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key).ToList();
        }
        

        private List<string> FindDuplicates(List<string> list)
        {
            return list.GroupBy(x => x)
                        .Where(group => group.Count() > 1)
                        .Select(group => group.Key).ToList();
        }


        #endregion

        #region move files
   
        private void MoveDoublesQuestion(string refpath, string selpath)
        {
            int nb = lstDoubles.Count;
            string tx = string.Empty;
            string question = string.Empty;

            // No doubles found
            if (nb == 0)
            {
                if (refpath != selpath)
                {
                    tx = "No double found between directory\n";
                    tx += refpath + "\n\n";
                    tx += "and directory\n";
                    tx += selpath + "\n";
                }
                else
                {
                    tx = "No double found in this directory\n";
                    tx += refpath;
                }

                MessageBox.Show(tx);
                return;
            }

    
            // Question move or display 
            tx = nb + " doubles were found!\n\n";
            tx += "Do you want to move them in a directory called 'Doubles' under the directory\n";
            tx += selpath + "?\n\n";
            tx += "If you reply NO, they will only be displayed.";
            question = tx;              
            

            if (MessageBox.Show(question, "Karaboss - Doubles", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string fullpath = CreateFolder(selpath, "Doubles");
                if (fullpath != null)
                    MoveFiles(fullpath);
            }
            else
            {
                DisplayDoubles();
            }


        }


        private void MoveFiles(string path)
        {
            string srcpath = string.Empty;
            string destpath = string.Empty;
            string filenameWithoutExtension = string.Empty;
            string fileExtension = string.Empty;

            int nbmoved = 0;
            int notmoved = 0;

            for (int i = 0; i < lstDoubles.Count; i++)
            {
                srcpath = lstDoubles[i];

                if (srcpath.Substring(0, 5) != "\norg-")
                {
                    DirectoryInfo di = new DirectoryInfo(srcpath);
                    filenameWithoutExtension = Path.GetFileNameWithoutExtension(srcpath);
                    filenameWithoutExtension = filenameWithoutExtension.Replace("{", "");
                    filenameWithoutExtension = filenameWithoutExtension.Replace("}", "");

                    fileExtension = Path.GetExtension(srcpath);

                    int suffix = 0;
                    do
                    {
                        if (suffix == 0)
                        {
                            destpath = string.Format("{0}\\" + filenameWithoutExtension + "{1}", path, fileExtension);
                            ++suffix;
                        }
                        else
                            destpath = string.Format("{0}\\" + filenameWithoutExtension + " ({1})" + "{2}", path, ++suffix, fileExtension);

                    } while (Directory.Exists(destpath) || File.Exists(destpath));


                    try
                    {
                        Directory.Move(srcpath, destpath);
                        nbmoved++;
                    }
                    catch (Exception ex)
                    {
                        Console.Write(ex.Message);
                        notmoved++;
                    }
                }
            }

            string tx = nbmoved + " files were moved to\n" + path;
            if (notmoved > 0)
                tx += "\n" + notmoved + " files were not moved due to errors.";
            MessageBox.Show(tx);

        }

        private string CreateFolder(string path, string name)
        {
            int suffix = 0;
            string fullpath = string.Empty;

            do
            {

                if (suffix == 0)
                {
                    fullpath = string.Format("{0}\\" + name, path);
                    suffix++;
                }
                else
                    fullpath = string.Format("{0}\\" + name + " ({1})", path, ++suffix);

            } while (Directory.Exists(fullpath) || File.Exists(fullpath));


            try
            {
                Directory.CreateDirectory(fullpath);
                return fullpath;

            }
            catch (Exception ex)
            {
                //Console.Write(ex.Message);
                MessageBox.Show(ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Display list of doubles in default text viewer
        /// </summary>
        private void DisplayDoubles()
        {
            string tx = string.Empty;
            string file = string.Empty;

            for (int i = 0; i < lstDoubles.Count; i++)
            {
                tx += lstDoubles[i] + "\n";
            }

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);


            switch (SearchMode)
            {
                case "SearchMD5Doubles":
                    file = path + "\\DoublesMD5.txt";
                    break;
                case "SearchSizeDoubles":
                    file = path + "\\DoublesSize.txt";
                    break;
                case "SearchNameDoubles":
                    file = path + "\\DoublesName.txt";
                    break;
                default:
                    break;
            }

            try
            {
                System.IO.File.WriteAllText(@file, tx);
                System.Diagnostics.Process.Start(@file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion



    }
}
