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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Karaboss.Resources.Localization;

namespace Karaboss.xplorer
{
    // Events
    public delegate void SelectedIndexChangedEventHandler(object sender, string fileName);   
    public delegate void PlayMidiEventHandler(object sender, FileInfo fi, bool bplay);
    public delegate void PlayCDGEventHandler(object sender, FileInfo fi, bool bplay);
    public delegate void PlayTextEventHandler(object sender, FileInfo fi, bool bplay);
    public delegate void ContentChangedEventHandler(object sender, string strContent, string strPath);
    public delegate void CreateNewMidiFileEventHandler(object sender);


    public partial class xplorerControl : UserControl
    {

        // Play a song, a playlist or edit a song        
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;        
        public event PlayMidiEventHandler PlayMidi;
        public event PlayCDGEventHandler PlayCDG;
        public event PlayTextEventHandler PlayText;
        public event ContentChangedEventHandler LvContentChanged;
        public event CreateNewMidiFileEventHandler CreateNewMidiFile;
        

        #region properties
        private int _splitterdistance;
        public int SplitterDistance
        {
            get { return _splitterdistance; }
            set
            {
                if (value > 0)
                {
                    _splitterdistance = value;
                    splitContainerFiles.SplitterDistance = _splitterdistance;
                }
            }
        }


        public FlShell.ShellItem SelectedFolder
        {
            get { return treeView.SelectedFolder; }

        }


        public string CurrentFolder
        {
            get
            {
                return shellListView.CurrentFolder.FileSystemPath;

            }
        }

        private string m_CurrentContent;
        public string CurrentContent
        {
            get
            {
                return m_CurrentContent;
            }
        }


        public FlShell.ShellItem[] SelectedItems
        {
            get { return shellListView.SelectedItems; }
        }


        public string SelectedFile
        {
            get
            {
                if (shellListView.SelectedItem.IsFolder || shellListView.SelectedItem.IsDisk)
                    return null; 
                else
                    return shellListView.SelectedItem.FileSystemPath; 

            }
        }

        private bool _bDoSelect = true;
        public bool bDoSelect
        {
            set {
                _bDoSelect = value;
                shellListView.bDoSelect = value;
            }
        }

        #endregion


        private ObservableCollection<Playlist> allPlaylists = new ObservableCollection<Playlist>();        
        private PlaylistGroup PlGroup = new PlaylistGroup();
        private PlaylistGroupsHelper PlGroupHelper = new PlaylistGroupsHelper();


        public xplorerControl()
        {
            InitializeComponent();

            shellListView.AddToPlaylist += new FlShell.AddToPlaylistByNameHandler(ShellListView_AddToPlaylist);
            shellListView.PlayMidi += new FlShell.PlayMidiEventHandler(ShellListView_PlayMidi);
            shellListView.PlayCDG += new FlShell.PlayCDGEventHandler(ShellListView_PlayCDG);
            shellListView.PlayText += new FlShell.PlayTextEventHandler(ShellListView_PlayText);
            shellListView.lvContentChanged += new FlShell.ContentChangedEvenHandler(ShellListView_ContentChanged);
            shellListView.SelectedIndexChanged += new FlShell.SelectedIndexChangedEventHandler(ShellListView_SelectedIndexChanged);
                       
            // F3, F4, F6            
            shellListView.lvFunctionKeyClicked += new FlShell.lvFunctionKeyEventHandler(ShellListView_lvFunctionKeyClicked);
            shellListView.SenKeyToParent += new FlShell.SenKeyToParentHandler(shellListView_SendKeyToParent);

            treeView.tvFunctionKeyClicked += new FlShell.tvFunctionKeyEventHandler(TreeView_tvFunctionKeyClicked);

            // Load existing playlists
            LoadPlaylists();            
        }


        #region public functions    

        /// <summary>
        /// Load playlists and refresh the ShellListView "Add to playlists" menu
        /// </summary>
        public override void Refresh()
        {
            LoadPlaylists();
            SelectFirstItem();            
            base.Refresh();
        }

        
        public void SelectFirstItem()
        {
            shellListView.SelectFirstItem();                       
        }

        public void Navigate(string path)
        {
            shellListView.Navigate(path);
        }

        public void SetColumnWidth(int column, int width)
        {
            shellListView.SetColumnWidth(column, width);
        }

        public int GetColumnWidth(int column)
        {
            return shellListView.GetColumnWidth(column);
        }

        /// <summary>
        /// Refresh ontent of ShellListView
        /// </summary>
        /// <param name="fullPath"></param>
        public void RefreshContents(string fullPath = "")
        {
            shellListView.RefreshContents(fullPath);
        }

        #endregion


        #region shellListView

        private void ShellListView_SelectedIndexChanged(object sender, string fileName)
        {
            SelectedIndexChanged?.Invoke(this, fileName);
        }

        private void ShellListView_ContentChanged(object sender, string strContent, string strPath)
        {
            m_CurrentContent = strContent;
            LvContentChanged?.Invoke(this, strContent, strPath);
        }

        private void ShellListView_PlayCDG(object sender, FileInfo fi, bool bplay)
        {
            PlayCDG?.Invoke(this, fi, bplay);
        }

        private void ShellListView_PlayMidi(object sender, FileInfo fi, bool bplay)
        {
            PlayMidi?.Invoke(this, fi, bplay);
        }
        private void ShellListView_PlayText(object sender, FileInfo fi, bool bplay)
        {
            PlayText?.Invoke(this, fi, bplay);
        }
        private void ShellListView_AddToPlaylist(object sender, FlShell.ShellItem[] fls, string plname, string key, bool bnewPlaylist)
        {
            if (bnewPlaylist == true)
            {
                List<string> li = new List<string>();
                foreach (FlShell.ShellItem shi in fls)
                {
                    li.Add(shi.FileSystemPath);
                }

                FrmNewPlaylist frmNewPlaylist = new FrmNewPlaylist(li);
                frmNewPlaylist.ShowDialog();

                LoadPlaylists();
                
            }
            else
            {
                Playlist pltarget = PlGroupHelper.getPlaylistByName(PlGroup, plname, key);
                AddToExistingPlaylist(fls, pltarget);               
            }
        }

        private void ShellListView_Navigated(object sender, EventArgs e)
        {
            backButton.Enabled = shellListView.CanNavigateBack;
            forwardButton.Enabled = shellListView.CanNavigateForward;
            upButton.Enabled = shellListView.CanNavigateParent;

            SelectFirstItem();
        }

        private void ShellListView_lvFunctionKeyClicked(object sender, Keys keyCode, Keys KeyData)
        {
            switch (keyCode)
            {
                case Keys.N:
                    if (KeyData == (Keys.N | Keys.Control))
                        CreateNewMidiFile?.Invoke(this);
                    break;               

                case Keys.F3:
                    RenameAllQuestion();                    
                    break;

                case Keys.F4:
                    ReplaceAllQuestion();                    
                    break;

                case Keys.F6:
                    InvertAuthor();
                    break;

                default:
                    break;
            }
        }      

        #endregion


        #region treeview

        private void TreeView_tvFunctionKeyClicked(object sender, Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.F3:
                    RenameAllQuestion();
                    break;
                case Keys.F4:
                    ReplaceAllQuestion();
                    break;
             
                default:
                    break;
            }
        }

        private void TvToolbar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            {
                if (e.ClickedItem == backButton)
                {
                    shellListView.NavigateBack();
                }
                else if (e.ClickedItem == forwardButton)
                {
                    shellListView.NavigateForward();
                }
                else if (e.ClickedItem == upButton)
                {
                    shellListView.NavigateParent();
                }
            }
        }

        #endregion


        #region invert author and song

        private void InvertAuthor()
        {
            if (treeView.SelectedFolder != null && treeView.SelectedFolder.IsFolder)
            {
                string tx = string.Empty;
                tx = "This function replace the format\n'song (author).mid'\nto the format\n'author - song.mid'.\n\n";
                tx += "Continue?";

                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    return;


                string physicalPath = this.treeView.SelectedFolder.FileSystemPath;
                
                string filename = string.Empty;
                string author = string.Empty;
                string song = string.Empty;
                string extension = string.Empty;
                string newfileName = string.Empty;
                string oldFileName = string.Empty;

                try
                {
                    string[] files = Directory.GetFiles(physicalPath);
                    foreach (string file in files)
                    {
                        oldFileName = Path.GetFileName(file);
                        filename = Path.GetFileNameWithoutExtension(file);
                        extension = Path.GetExtension(file);                        

                        if (filename.IndexOf("(") > 0 && filename.IndexOf(")") > 0)
                        {
                            author = GetBetween(filename, "(", ")");
                            if (author.Length > 1)
                            {
                                song = filename.Substring(0, filename.IndexOf("(")).Trim();
                                author = GetBetween(filename, "(", ")");
                                newfileName = author + " - " + song + extension;
                                newfileName = GetUniqueFileName(Path.Combine(physicalPath, newfileName));

                                RenameFile(oldFileName, newfileName, physicalPath);
                            }
                        }
                    }

                    RefreshContents();

                }
                catch (Exception er)
                {
                    Console.WriteLine("The process failed: {0}", er.ToString());
                }

            }
        }

        private string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start).Trim();
            }
            else
            {
                return "";
            }
        }

        #endregion

        /// <summary>
        /// REplace .mid by .kar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="k"></param>
        private void shellListView_SendKeyToParent(object sender, Keys k)
        {
            if (k == Keys.K)
            {
                try
                {
                    if (shellListView.SelectedItems.Length > 0)
                    {
                        FlShell.ShellItem shi = shellListView.SelectedItems[0];
                        string newfileName = string.Empty;

                        string oldFileName = shi.FileSystemPath;
                        string physicalPath = Path.GetDirectoryName(oldFileName);

                        if (Karaclass.IsMidiExtension(oldFileName))
                        {
                            if (Path.GetExtension(oldFileName).ToLower() == ".mid")
                            {
                                newfileName = oldFileName.Replace(".mid", ".kar");
                                newfileName = GetUniqueFileName(newfileName);

                                RenameFile(oldFileName, newfileName, physicalPath);                                
                            }
                            else if (Path.GetExtension(oldFileName).ToLower() == ".kar")
                            {
                                newfileName = oldFileName.Replace(".kar", ".mid");
                                newfileName = GetUniqueFileName(newfileName);

                                RenameFile(oldFileName, newfileName, physicalPath);                                                                                               
                            }

                            // Refresh & Set SelectedItem                            
                            RefreshContents(Path.GetFileName(newfileName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }     


        #region rename all

        /// <summary>
        /// Ask confirmation to rename all files
        /// </summary>
        public void RenameAllQuestion()
        {
            if (treeView.SelectedFolder != null && treeView.SelectedFolder.IsFolder)
            {
                string fullpath = this.treeView.SelectedFolder.FileSystemPath;

                
                if (Directory.GetFiles(fullpath).Length > 0 ||
                Directory.GetDirectories(fullpath).Length > 0)
                {

                    string tx = string.Empty;
                    string file = string.Empty;
                    string path = string.Empty;

                    fullpath = this.treeView.SelectedFolder.First().FileSystemPath;
                                  

                    path = Path.GetDirectoryName(fullpath);
                    file = Path.GetFileName(fullpath);

                    string[] directories = path.Split(Path.DirectorySeparatorChar);
                    string pfx = directories.Last();

                    // every first letter to upper case
                    pfx = ToTitleCase(pfx);

                    tx = "This function use the name of the upper directory to rename all the files.\n";
                    tx += "The prefix <" + pfx + "> will be added to all the files.\n\n";
                    tx += "Result:\n" + pfx + " - " + file + "\n\n";
                    tx += "Continue?";

                    if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        RenameAllFiles(pfx, path);
                        RefreshContents();

                    }
                }
            }
        }

        /// <summary>
        /// Rename all files
        /// </summary>
        /// <param name="pfx"></param>
        /// <param name="physicalPath"></param>
        private void RenameAllFiles(string pfx, string physicalPath)
        {
            // 1 - Clean all by keeping only the song name
            CleanAllFiles(pfx, physicalPath);
            string dir = GetCleanedDirectory(pfx);

            // 2 - Clean the name of the upper directory
            DirectoryInfo diParentDir = Directory.GetParent(physicalPath);
            string parentDir = diParentDir.FullName;
            string newPhysicalPath = parentDir + "\\" + dir;
            RenameDir(physicalPath, newPhysicalPath);

            // 3 - Apply format : <upper directory> - <song name>
            FormateFiles(dir, newPhysicalPath);

        }


        /// <summary>
        /// Clean and rename all files in a directory, keep only the song name
        /// </summary>
        /// <param name="pfx">Upper directory</param>
        /// <param name="physicalPath">Full path</param>
        private void CleanAllFiles(string pfx, string physicalPath)
        {
            string newfileName = string.Empty;
            string oldFileName = string.Empty;

            try
            {
                string[] files = Directory.GetFiles(physicalPath);
                foreach (string file in files)
                {
                    // Clean song (remove singer & clean)
                    newfileName = GetCleanSongName(pfx, file);
                    oldFileName = Path.GetFileName(file);

                    // if newFile already exist => increment (1)
                    if (newfileName.ToUpper() != oldFileName.ToUpper())
                    {
                        newfileName = GetUniqueFileName(Path.Combine(physicalPath, newfileName));
                    }
                    // rewrite it
                    RenameFile(oldFileName, newfileName, physicalPath);

                }
            }
            catch (Exception er)
            {
                Console.WriteLine("The process failed: {0}", er.ToString());
            }
        }


        /// <summary>
        /// Apply format for files
        /// </summary>
        /// <param name="pfx"></param>
        /// <param name="physicalPath"></param>
        private void FormateFiles(string pfx, string physicalPath)
        {
            string newfileName = string.Empty;
            string oldFileName = string.Empty;

            try
            {
                string[] files = Directory.GetFiles(physicalPath);
                foreach (string file in files)
                {
                    // apply format
                    oldFileName = Path.GetFileName(file);
                    newfileName = pfx + " - " + oldFileName;
                    // rewrite it
                    RenameFile(oldFileName, newfileName, physicalPath);
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("The process failed: {0}", er.ToString());
            }

        }


        /// <summary>
        /// every first letter to upper case
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Remove accents to a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Clean song name: remove singer, remove ".", "()", "( )"
        /// </summary>
        /// <param name="pfx"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetCleanSongName(string dir, string fileName)
        {
            string newfileName = string.Empty;

            try
            {
                string fileExtension = Path.GetExtension(fileName).ToLower();
                string fileNameShort = Path.GetFileNameWithoutExtension(fileName);

                
                // Nom du fichier en majuscules
                string tstF = fileNameShort.ToUpper();
                // Nom du répertoire en majuscules
                string tstD = dir.ToUpper();
                string tstD2 = RemoveDiacritics(tstD);

                // Normal list of singer
                string[] split = tstD.Split(new Char[] { ' ' });
                // Without accent list of singer
                string[] split2 = tstD2.Split(new Char[] { ' ' });
                // With - separator list of singer
                string[] split3 = tstD.Split(new Char[] { '-' });

                // Présence du nom du repertoire dans le nom du fichier => remove
                // Normal list

                if (tstF.IndexOf(tstD + " - ") == 0)
                {
                    tstF = tstF.Replace(tstD + " - ", "");
                    newfileName = tstF.Trim();
                }
                else
                {

                    if (split.Length > 0)
                    {

                        foreach (string s in split)
                        {
                            string st = s.Trim();

                            if (st.Length > 1 && st.ToUpper() != "THE" && st.ToUpper() != "AND")
                            {
                                if (tstF.IndexOf(st) > -1)
                                {
                                    do
                                    {
                                        tstF = tstF.Replace(st, "");
                                    } while (tstF.IndexOf(st) > -1);
                                }


                                tstF = tstF.Trim();
                            }
                        }

                        newfileName = tstF.Trim();
                    }

                    if (split2.Length > 0)
                    {

                        foreach (string s in split2)
                        {
                            string st = s.Trim();

                            if (st.Length > 1 && st.ToUpper() != "THE" && st.ToUpper() != "AND")
                            {
                                if (tstF.IndexOf(st) > -1)
                                {
                                    do
                                    {
                                        tstF = tstF.Replace(st, "");
                                    } while (tstF.IndexOf(st) > -1);
                                }
                                tstF = tstF.Trim();
                            }
                        }

                        newfileName = tstF.Trim();
                    }

                    if (split3.Length > 0)
                    {

                        foreach (string s in split3)
                        {
                            string st = s.Trim();

                            if (st.Length > 1 && st.ToUpper() != "THE" && st.ToUpper() != "AND")
                            {
                                if (tstF.IndexOf(st) > -1)
                                {
                                    do
                                    {
                                        tstF = tstF.Replace(st, "");
                                    } while (tstF.IndexOf(st) > -1);
                                }
                                tstF = tstF.Trim();
                            }
                        }

                        newfileName = tstF.Trim();
                    }
                }

                newfileName = newfileName.Replace(".", "");
                newfileName = newfileName.Replace("_", " ");
                newfileName = newfileName.Replace("-", " ");
                newfileName = newfileName.Replace("  ", " ");
                newfileName = newfileName.Replace("( )", "");
                newfileName = newfileName.Replace("()", "");

                
                newfileName = newfileName.Trim();




                //every first letter to upper case
                newfileName = ToTitleCase(newfileName) + fileExtension;

                // Replace "toto K.mid" by "toto.kar"
                if (newfileName.IndexOf(" K.mid") > 0)
                {
                    newfileName = newfileName.Replace(" K.mid", ".kar");
                }


                return newfileName;
            }
            catch (Exception er)
            {
                Console.WriteLine("Clean fileName failed: {0}", er.ToString());
                return fileName;
            }
        }


        /// <summary>
        /// Get a clen name for a directory
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        private string GetCleanedDirectory(string dir)
        {
            string newDirName = string.Empty;
            newDirName = dir.Replace("_", " ");
            newDirName = newDirName.Replace("--", " ");
            newDirName = newDirName.Replace("  ", " ");
            newDirName = newDirName.Trim();
            //every first letter to upper case
            newDirName = ToTitleCase(newDirName);
            return newDirName;
        }

        /// <summary>
        /// Add a (2) to (n) to a file until it is new
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="physicalPath"></param>
        /// <returns>new file name</returns>
        private string GetUniqueFileName(string fullPath)
        {
            int count = 2;

            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0} ({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return Path.GetFileName(newFullPath); //newFullPath;
        }

        /// <summary>
        /// Rename a directory
        /// </summary>
        /// <param name="oldDir"></param>
        /// <param name="newDir"></param>
        private void RenameDir(string oldDir, string newDir)
        {
            try
            {
                if (oldDir.ToUpper() != newDir.ToUpper())
                    System.IO.Directory.Move(@oldDir, @newDir);
                else
                {
                    var dir = new DirectoryInfo(@oldDir);
                    dir.MoveTo(@oldDir + "xyz");
                    dir.MoveTo(@newDir);

                }
            }
            catch (Exception er)
            {
                Console.WriteLine("Rename directory failed: {0}", er.ToString());
            }
        }

  

        /// <summary>
        /// Rename a file
        /// </summary>
        /// <param name="oldFileName"></param>
        /// <param name="newFileName"></param>
        /// <param name="physicalPath"></param>
        private void RenameFile(string oldFileName, string newFileName, string physicalPath)
        {
            try
            {
                System.IO.File.Move(Path.Combine(physicalPath, oldFileName), Path.Combine(physicalPath, newFileName));
            }
            catch (Exception er)
            {
                Console.WriteLine("Rename fileName failed: {0}", er.ToString());
                MessageBox.Show(er.Message);
            }
        }


        #endregion


        #region Replace All

        /// <summary>
        /// Ask confirmation to search & replace in all files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ReplaceAllQuestion()
        {
            if ((treeView.SelectedFolder != null) && treeView.SelectedFolder.IsFolder)
            {
                string fullpath = treeView.SelectedFolder.FileSystemPath;

                //if (this.TreeView.SelectedFolder.First() != null)
                if (Directory.GetFiles(fullpath).Length > 0 ||
                Directory.GetDirectories(fullpath).Length > 0)
                {

                    string tx = string.Empty;
                    string file = string.Empty;
                    string path = string.Empty;

                    fullpath = treeView.SelectedFolder.First().FileSystemPath;

                    path = Path.GetDirectoryName(fullpath);
                    file = Path.GetFileName(fullpath);

                    string[] txvalues = new string[2];

                    // Display dialg Search & replace
                    if (Prompt.ShowDialog("Karaboss - Replace", ref txvalues) == DialogResult.OK)
                    {
                        string txSearch = txvalues[0];
                        string txReplace = txvalues[1];

                        if (txSearch != "")
                        {
                            tx = "This function replace each occurence of <" + txSearch + ">\n";
                            tx += "by <" + txReplace + ">\n";
                            tx += "Continue?";

                            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel) == DialogResult.OK)
                            {
                                ReplaceAll(txSearch, txReplace, path);
                                RefreshContents();
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Replace all file procedure
        /// </summary>
        /// <param name="txSearch"></param>
        /// <param name="txReplace"></param>
        /// <param name="physicalpath"></param>
        private void ReplaceAll(string txSearch, string txReplace, string physicalpath)
        {
            string newfileName = string.Empty;
            try
            {
                string[] files = Directory.GetFiles(physicalpath);
                foreach (string file in files)
                {
                    string oldfileName = Path.GetFileName(file);
                    string ext = Path.GetExtension(oldfileName);
                    string wext = Path.GetFileNameWithoutExtension(oldfileName);
                    string nwext = string.Empty;

                    // txSearch présent?
                    if (wext.IndexOf(txSearch) != -1)
                    {
                        nwext = wext.Replace(txSearch, txReplace);
                        newfileName = nwext + ext;
                        
                        // Files already exists ?
                        if (!File.Exists(Path.Combine(physicalpath, newfileName)))
                        {
                            System.IO.File.Move(Path.Combine(physicalpath, oldfileName), Path.Combine(physicalpath, newfileName));
                        }
                        else
                        {
                            newfileName = GetUniqueFileName(Path.Combine(physicalpath, newfileName));
                            System.IO.File.Move(Path.Combine(physicalpath, oldfileName), Path.Combine(physicalpath, newfileName));                            
                        }
                    }
                }
            }
            catch (Exception ep)
            {
                Console.WriteLine("The process failed: {0}", ep.ToString());
                MessageBox.Show(ep.Message);
            }

        }

        #endregion


        #region prompt
        /// <summary>
        /// Promt dialog window to get replacement string
        /// </summary>
        private static class Prompt
        {
            public static DialogResult ShowDialog(string caption, ref string[] value)
            {
                int wd = 400;
                int ht = 200;

                Form prompt = new Form()
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    MinimizeBox = false,
                    MaximizeBox = false,
                    Width = wd,
                    Height = ht,
                    Text = caption,
                };

                string[] result = new string[2];

                Label textLabel1 = new Label() { Left = 10, Top = 20, Text = "String to search" };
                TextBox inputBox1 = new TextBox() { Left = 120, Top = 20, Width = 180 };
                inputBox1.Text = value[0];

                Label textLabel2 = new Label() { Left = 10, Top = 50, Text = "String to replace" };
                TextBox inputBox2 = new TextBox() { Left = 120, Top = 50, Width = 180 };
                inputBox2.Text = value[1];

                Button btnConfirmation = new Button() { Text = "Ok", Left = wd / 2 - 100 - 10, Width = 100, Top = 100 };
                Button btnCancel = new Button() { Text = "Cancel", Left = wd / 2 + 10, Width = 100, Top = 100 };

                btnConfirmation.DialogResult = DialogResult.OK;
                btnCancel.DialogResult = DialogResult.Cancel;

                btnConfirmation.Click += (sender, e) => { prompt.Close(); };
                btnCancel.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(btnConfirmation);
                prompt.Controls.Add(btnCancel);
                prompt.Controls.Add(textLabel1);
                prompt.Controls.Add(inputBox1);
                prompt.Controls.Add(textLabel2);
                prompt.Controls.Add(inputBox2);

                prompt.AcceptButton = btnConfirmation;
                prompt.CancelButton = btnCancel;

                textLabel1.TabIndex = 0;
                inputBox1.TabIndex = 1;
                textLabel2.TabIndex = 2;
                inputBox2.TabIndex = 3;
                btnConfirmation.TabIndex = 4;
                btnCancel.TabIndex = 5;


                DialogResult dialogResult = prompt.ShowDialog();



                value[0] = inputBox1.Text;
                value[1] = inputBox2.Text;

                //return result;
                return dialogResult;
            }
        }

        #endregion


        #region buttons

        /// <summary>
        /// Buton create a new Midi file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNewMidiFile_Click(object sender, EventArgs e)
        {
            CreateNewMidiFile?.Invoke(this);
        }

        /// <summary>
        /// Button PLAY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(true);
        }

        /// <summary>
        /// Button EDIT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(false);
        }

        /// <summary>
        /// Rename files 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NamingButton_Click(object sender, EventArgs e)
        {
            RenameAllQuestion();
        }

        /// <summary>
        /// Replace in file names
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceButton_Click(object sender, EventArgs e)
        {
            ReplaceAllQuestion();
        }

        /// <summary>
        /// Launch play or edit
        /// </summary>
        /// <param name="bplay"></param>
        private void InvokePlayEdit(bool bplay)
        {
            FlShell.ShellItem[] fls = shellListView.SelectedItems;

            if (fls.Length > 0)
            {
                string file = fls[0].FileSystemPath;

                if (file == "")
                    file = fls[0].ParsingName;

                string ext = Path.GetExtension(file);
                switch (ext.ToLower())
                {
                    case ".mid":
                    case ".kar":
                        {
                            PlayMidi?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }

                    case ".zip":
                    case ".cdg":
                        {
                            PlayCDG?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }

                    default:
                        System.Diagnostics.Process.Start(@file);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Please select a file", "Karaboss", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Navigate to Downloads folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownloads_Click(object sender, EventArgs e)
        {
            string path = KnownFolders.GetPath(KnownFolder.Downloads);
            //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
            path = "file:///" + path.Replace("\\", "/");
            Navigate(path);
        }

        #endregion


        #region playlists

        /// <summary>
        /// Display the menu playlists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnutbAddToPlayList_Click(object sender, EventArgs e)
        {
            CreateMenusAddToPlaylists(mnutbAddToPlayList, PlGroup);
        }


        /// <summary>
        /// Create the tree of menus Add to playlists
        /// </summary>
        /// <param name="tbtn"></param>
        /// <param name="plg"></param>
        public void CreateMenusAddToPlaylists(ToolStripDropDownButton tbtn, PlaylistGroup plg)
        {
            tbtn.DropDownItems.Clear();

            // First: propose to add the song to the existing playlists                
            for (int i = 0; i < plg.Count; i++)
            {
                PlaylistGroupItem plgi = plg.plGroupItems[i];

                string folder = plgi.Name;
                string Key = plgi.Key;
                string ParentKey = plgi.ParentKey;

                ToolStripMenuItem folderMenu = new ToolStripMenuItem()
                {
                    Text = folder,
                    Tag = Key,
                };


                // Add submenus composed of playlist names
                for (int j = 0; j < plgi.Playlists.Count; j++)
                {
                    ToolStripMenuItem plitem = new ToolStripMenuItem()
                    {
                        Text = plgi.Playlists[j].Name,
                        // Tag to retrieve the folder
                        Tag = Key,
                    };

                    plitem.Click += new EventHandler(MnuAddToExistingPlaylist_Click);
                    folderMenu.DropDownItems.Add(plitem);
                }

                // Add the menu to its parent
                if (ParentKey == null)
                    tbtn.DropDownItems.Add(folderMenu);
                else
                {
                    foreach (ToolStripMenuItem mnu in tbtn.DropDownItems)
                    {
                        ToolStripMenuItem m = FindMenu(ParentKey, mnu);
                        if (m != null) m.DropDownItems.Add(folderMenu);
                    }
                }
            }

            ToolStripSeparator mnusep1 = new ToolStripSeparator();
            tbtn.DropDownItems.Add(mnusep1);

            // Second: propose to create a new playlist and then add the song to it
            ToolStripMenuItem mnuAddToNewPlaylist = new ToolStripMenuItem(Strings.NewPlaylist);
            tbtn.DropDownItems.Add(mnuAddToNewPlaylist);
            mnuAddToNewPlaylist.Click += new System.EventHandler(this.MnuAddToNewPlaylist_Click);
        }

        public void CreateMenusAddToPlaylists(ToolStripMenuItem tbtn, PlaylistGroup plg)
        {
            tbtn.DropDownItems.Clear();

            // First: propose to add the song to the existing playlists                
            for (int i = 0; i < plg.Count; i++)
            {
                PlaylistGroupItem plgi = plg.plGroupItems[i];

                string folder = plgi.Name;
                string Key = plgi.Key;
                string ParentKey = plgi.ParentKey;

                ToolStripMenuItem folderMenu = new ToolStripMenuItem()
                {
                    Text = folder,
                    Tag = Key,
                };


                // Add submenus composed of playlist names
                for (int j = 0; j < plgi.Playlists.Count; j++)
                {
                    ToolStripMenuItem plitem = new ToolStripMenuItem()
                    {
                        Text = plgi.Playlists[j].Name,
                        // Tag to retrieve the folder
                        Tag = Key,
                    };
                    plitem.Click += new EventHandler(MnuAddToExistingPlaylist_Click);
                    folderMenu.DropDownItems.Add(plitem);
                }

                // Add the menu to its parent
                if (ParentKey == null)
                    tbtn.DropDownItems.Add(folderMenu);
                else
                {
                    foreach (ToolStripMenuItem mnu in tbtn.DropDownItems)
                    {
                        ToolStripMenuItem m = FindMenu(ParentKey, mnu);
                        if (m != null) m.DropDownItems.Add(folderMenu);
                    }
                }
            }

            ToolStripSeparator mnusep1 = new ToolStripSeparator();
            tbtn.DropDownItems.Add(mnusep1);

            // Second: propose to create a new playlist and then add the song to it
            ToolStripMenuItem mnuAddToNewPlaylist = new ToolStripMenuItem(Strings.NewPlaylist);
            tbtn.DropDownItems.Add(mnuAddToNewPlaylist);
            mnuAddToNewPlaylist.Click += new System.EventHandler(this.MnuAddToNewPlaylist_Click);
        }

        /// <summary>
        /// Search key recursively in menus
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="mnuRoot"></param>
        /// <returns></returns>
        private ToolStripMenuItem FindMenu(string Key, ToolStripMenuItem mnuRoot)
        {
            if (mnuRoot.Tag != null && mnuRoot.Tag.ToString() == Key) return mnuRoot;

            foreach (ToolStripMenuItem mnu in mnuRoot.DropDownItems)
            {
                if (mnu.Tag != null && mnu.Tag.ToString() == Key) return mnu;

                ToolStripMenuItem mnunext = FindMenu(Key, mnu);
                if (mnunext != null) return mnunext;
            }
            return null;
        }

      
        /// <summary>
        /// Load existing playlists into allPlaylists collection
        /// </summary>
        public void LoadPlaylists()
        {
            // not executed in DesignMode            
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            try
            {
                string fileName = Karaclass.GetPlaylistGroupFile(PlGroupHelper.File);
                PlGroupHelper.File = fileName;
                PlGroup = PlGroupHelper.Load(fileName);
               

                int id = 0;
                for (int i = 0; i < PlGroup.plGroupItems.Count; i++)
                {
                    id += PlGroup.plGroupItems[i].Playlists.Count;
                }
                string[,] arrayMenu = new string[id, 2];

                string key = string.Empty;
                int idx = 0;
                for (int i = 0; i < PlGroup.plGroupItems.Count; i++)
                {
                    PlaylistGroupItem plgi = PlGroup.plGroupItems[i];
                    key = plgi.Key;
                    for (int j = 0; j < plgi.Playlists.Count; j++)
                    {
                        Playlist pl = plgi.Playlists[j];
                        arrayMenu[idx, 0] = key;
                        arrayMenu[idx, 1] = pl.Name;
                        idx++;
                    }
                }
                this.shellListView.allPlaylists = arrayMenu;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }


        /// <summary>
        /// Menu: add songs (ShellItems list) to a new playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuAddToNewPlaylist_Click(object sender, EventArgs e)
        {
            FlShell.ShellItem[] fls = shellListView.SelectedItems;

            if (fls.Length > 0)
            {
                List<string> li = new List<string>();

                foreach (FlShell.ShellItem shi in fls)
                {
                    li.Add(shi.FileSystemPath);
                }

                FrmNewPlaylist frmNewPlaylist = new FrmNewPlaylist(li);
                frmNewPlaylist.ShowDialog();

                LoadPlaylists();
                //updateMenusPlayLists();
            }
        }

        /// <summary>
        /// Menu: Add songs (ShellItems list) to an existing playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuAddToExistingPlaylist_Click(object sender, EventArgs e)
        {
            FlShell.ShellItem[] fls = shellListView.SelectedItems;

            if (fls.Length == 0)
                return;


            // Name of the target playlist
            ToolStripMenuItem m = (ToolStripMenuItem)sender;

            string Key = m.Tag.ToString();
            string name = m.Text;
            Playlist pltarget = PlGroupHelper.getPlaylistByName(PlGroup, name, Key);

            AddToExistingPlaylist(fls, pltarget);
        }


        private void AddToExistingPlaylist(FlShell.ShellItem[] fls, Playlist pltarget)
        {
            int nbAdded = 0;
            int nbTotal = fls.Length;

            foreach (FlShell.ShellItem eachItem in fls)
            {
                string Artist = "<Artist>";
                string File = eachItem.FileSystemPath;
                string Song = Path.GetFileNameWithoutExtension(File);
                string Album = "<Album>";
                string Length = "00:00";
                string sNotation = "4";
                string sDirSlideShow = "";
                bool bMelodyMute = Karaclass.m_MuteMelody;

                if (pltarget.Add(Artist, Song, File, Album, Length, Convert.ToInt32(sNotation), sDirSlideShow, bMelodyMute, "<Song reserved by>"))
                    nbAdded++;
            }
            SaveAllPlaylist();
            MessageBox.Show(nbAdded + "/" + nbTotal + " songs added to <" + pltarget.Name + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

                 
        /// <summary>
        /// Save all playlists
        /// </summary>
        private void SaveAllPlaylist()
        {
            string fName = Karaclass.M_filePlaylistGroups;
            PlGroupHelper.Save(fName, PlGroup);                        
        }





        #endregion playlists

       
    }
}
