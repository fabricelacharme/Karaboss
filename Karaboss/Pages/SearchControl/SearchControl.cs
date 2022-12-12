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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Serialization;
using Karaboss.Resources.Localization;
using System.Runtime.InteropServices;
using System.Collections;
using System.ComponentModel;
using FlShell;

namespace Karaboss.Search
{
    
    public enum SearchViewStyle
    {
        Author = 1,
        File,
    }

    // Events
    public delegate void SelectedIndexChangedEventHandler(object sender, string fileName);
    public delegate void PlayMidiEventHandler(object sender, FileInfo fi, Playlist pl, bool bplay);
    public delegate void PlayCDGEventHandler(object sender, FileInfo fi, bool bplay);
    public delegate void MidiInfoEventHandler();
    public delegate void ContentChangedEventHandler(object sender, string strContent);
    public delegate void NavigateToEventHandler(Object sender, string path, string file);            // Says to parent to navigate to this folder
    public delegate void SongRootChangedEventHandler(Object sender, string path);                   // Warn aprent that song library has changed


    public partial class SearchControl : UserControl
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        #region events
        // Play a song, a playlist or edit a song
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;
        public event PlayMidiEventHandler PlayMidi;
        public event PlayCDGEventHandler PlayCDG;
        public event MidiInfoEventHandler OnMidiInfo;
        public event ContentChangedEventHandler SearchContentChanged;
        public event NavigateToEventHandler NavigateTo;
        public event SongRootChangedEventHandler SongRootChanged;

        #endregion

        [Serializable]
        public class Recording
        {
            public string Song { get; set; }
            public string Path { get; set; }


            /// <summary>
            /// An empty ctor is needed for serialization.
            /// </summary>
            public Recording()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="test.Recording"/> class.
            /// </summary>
            /// <param name="fileInfo">File info.</param>
            public Recording(string path)
            {
                FileInfo fileInfo = new FileInfo(path);

                this.Path = fileInfo.FullName;
                this.Song = fileInfo.Name;
                // TODO: add and initialize other members
            }
        }

        FlShell.ShellNotificationListener m_ShellListener = new FlShell.ShellNotificationListener();

        private string ScanfileName;

        private List<Recording> allFiles = new List<Recording>(); // Try to serialize above list   
        private List<FileInfo> filesFound = new List<FileInfo>();  // List that will hold the found files and subfiles in path

        #region Playlists
        private ObservableCollection<Playlist> allPlaylists = new ObservableCollection<Playlist>();        
        private PlaylistGroup PlGroup = new PlaylistGroup();
        private PlaylistGroupsHelper PlGroupHelper = new PlaylistGroupsHelper();
        #endregion

        private ContextMenuStrip lvContextMenu;


        private bool bCaseSensitive = false;
        private bool bSearchNameOnly = false;
        private bool bSearchSongOnly = false;
        private bool bChanged = false;

        
        Button btnSearch;
        Button btnClear;
        Button btnSearchDir;

        #region properties
        private string _songroot;
        public string SongRoot
        {
            get { return _songroot; }
            set
            {
                if (Directory.Exists(value))
                {
                    _songroot = value;
                    txtSearchDir.Text = _songroot;
                }
            }
        }

        public string SelectedFile
        {
            get {

                if (listView.SelectedIndices.Count > 0)
                    return listView.SelectedItems[0].Tag.ToString();
                else
                    return null;
            }
        }

        private SearchViewStyle m_View = SearchViewStyle.Author;
        public SearchViewStyle SView
        {
            get { return m_View; }
            set
            {
                m_View = value;
                listView.AllowDrop = (m_View == SearchViewStyle.Author ? true : false);
                RecreateSearchView();
               
            }
        }

        #endregion


        // Declare a Hashtable array in which to store the groups.
        private Hashtable groupTables;
        // Declare a variable to store the current grouping column.
        //int groupColumn = 0;

        #region listView

        private int W_filename = 260;
        private int W_size = 60;
        private int W_modified = 120;
        private int W_type = 200;

       

        #endregion

        // Constructor
        public SearchControl()
        {
            InitializeComponent();

            // initialize listview
            InitListview();

            if (_songroot == "C:\\\\" || Directory.Exists(_songroot) == false)
                _songroot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

            txtSearchDir.Text = _songroot;
           
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Application.ProductName);
            ScanfileName = path + "\\mylibrary.xml";
           
            // Populate songs
            GuessScanFiles();
            
            // Load existing playlists
            LoadPlaylists();                                
        }


        #region override

        /// <summary>
        /// Force focus to listview with TAB when focus is on txtSearch
        /// ProcessCmdKey save my life
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (txtSearch.Focused)
                {
                    if (listView.Items.Count > 0)
                    {                                                                        
                        listView.Focus();
                        return true;
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void Refresh()
        {
            LoadPlaylists();

            base.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_ShellListener.ItemDeleted -= new FlShell.ShellItemEventHandler(M_ShellListener_ItemUpdated);
                m_ShellListener.ItemRenamed -= new FlShell.ShellItemChangeEventHandler(M_ShellListener_ItemRenamed);
                m_ShellListener.ItemUpdated -= new FlShell.ShellItemEventHandler(M_ShellListener_ItemUpdated);

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }



        #endregion


        #region shell

        private void M_ShellListener_ItemRenamed(object sender, ShellItemChangeEventArgs e)
        {
            if (!this.Visible || filesFound.Count == 0)
                return;

            if (e.OldItem.IsFileSystem)
            {
                string fullname = e.OldItem.FileSystemPath;                
                FileInfo finfo = filesFound.Where(z => z.FullName == fullname).FirstOrDefault();

                string oldPath = e.OldItem.FileSystemPath;
                string newPath = e.NewItem.FileSystemPath;

                
                int idx = allFiles.FindIndex(z => z.Path == oldPath);

                if (idx == -1)
                    return;

                // Update listView
                foreach (ListViewItem lvi in listView.Items)
                {
                    if ((string)lvi.Tag == e.OldItem.FileSystemPath)
                    {

                        switch (m_View)
                        {
                            case SearchViewStyle.Author:
                                string song = GetSongName(e.NewItem.FileSystemPath);
                                lvi.Text = song;
                                lvi.Tag = e.NewItem.FileSystemPath;
                                break;
                            case SearchViewStyle.File:
                                lvi.Text = e.NewItem.Text;
                                lvi.Tag = e.NewItem.FileSystemPath;
                                break;
                        }
                        break;
                    }
                }

                // Upadate allFiles
                allFiles[idx].Path = newPath;
                allFiles[idx].Song = Path.GetFileName(newPath);                
                SaveScanFiles(ScanfileName);
                bChanged = false;


              
            }
        }
      
        private void M_ShellListener_ItemUpdated(object sender, ShellItemEventArgs e)
        {
            if (e.Item.IsFileSystem)
            {

            }
        }

        #endregion
      
      
        #region edit an item

        /// <summary>
        /// Rename One file in the same diretory
        /// </summary>
        /// <param name="oldfileName"></param>
        /// <param name="newfileName"></param>
        /// <param name="physicalpath"></param>
        private string RenameOne(string oldfileName, string newfileName, string path)
        {
            // Is identical, do nothing
            if (oldfileName.ToUpper() == newfileName.ToUpper())
                return string.Empty;

            try
            {
                // can rename only if old file exists
                if (File.Exists(Path.Combine(path, oldfileName)))
                {
                    // if new name of file does not exists => rename
                    if (!File.Exists(Path.Combine(path, newfileName)))
                    {
                        System.IO.File.Move(Path.Combine(path, oldfileName), Path.Combine(path, newfileName));
                    }
                    else
                    {
                        // new name already exists => search for an unexistant one in windows behaviour: "name.txt" to "name (2).txt"
                        newfileName = GetUniqueFileName(Path.Combine(path, newfileName));
                        System.IO.File.Move(Path.Combine(path, oldfileName), Path.Combine(path, newfileName));
                    }
                    return newfileName;
                }
                return string.Empty;
            }
            catch (Exception eo)
            {
                Console.WriteLine("The process failed: {0}", eo.ToString());
                MessageBox.Show(eo.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Add a (2) to (n) to a file until it is new
        /// </summary>
        /// <param name="fullPath"></param>
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
        /// Delete a file or a list of files menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public void DeleteFile()
        {
            string tx = string.Empty;
            string fileName = string.Empty;
            int n = listView.SelectedItems.Count;

            if (n == 0)
            {
                return;
            }
            else if (n == 1)
            {
                fileName = listView.SelectedItems[0].Text;
                tx = "Do you really want to place this file into the Trash?\n" + fileName;
            }
            else
            {
                tx = String.Format("Do you really want to place these {0} files into the Trash?", n);
            }

            if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                return;

            for (int i = 0; i < n; i++)
            {

                fileName = (string)listView.SelectedItems[i].Tag;

                if (File.Exists(fileName))
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ed)
                    {
                        Console.WriteLine("The process failed: {0}", ed.ToString());
                        MessageBox.Show(ed.Message);
                    }
                }

            }

           
        }



        #endregion


        #region ListView

        #region listview functions

        private void InitListview()
        {            
            // Affichage mode détails
            listView.View = View.Details;
            // Autoriser l'édition in place
            listView.LabelEdit = true;
            // keep selection visible
            listView.HideSelection = false;            

            listView.FullRowSelect = true;

            listView.GridLines = false;

            // Sorting            
            listView.Sorting = SortOrder.Ascending;

            #region header
            //Headers listview     
            ColumnHeader columnHeader0 = new ColumnHeader() {
                Text = Strings.fileName,
                Width = W_filename,
            };

            ColumnHeader columnHeader1 = new ColumnHeader() {
                Text = Strings.Size,
                Width = W_size,
                TextAlign = HorizontalAlignment.Right,
            };

            ColumnHeader columnHeader2 = new ColumnHeader()
            {
                Text = Strings.Modified,
                Width = W_modified,
            };

            ColumnHeader columnHeader3 = new ColumnHeader() {
                Text = Strings.Type,
                Width = W_type,
            };

            // Add the column headers to myListView.
            listView.Columns.AddRange(new ColumnHeader[]
                {columnHeader0, columnHeader1, columnHeader2, columnHeader3});

            #endregion

            m_ShellListener.ItemDeleted += new FlShell.ShellItemEventHandler(M_ShellListener_ItemUpdated);
            m_ShellListener.ItemRenamed += new FlShell.ShellItemChangeEventHandler(M_ShellListener_ItemRenamed);
            m_ShellListener.ItemUpdated += new FlShell.ShellItemEventHandler(M_ShellListener_ItemUpdated);

            listView.GroupHeaderClick += new EventHandler<int>(ListView_GroupHeaderClick);

            // Associer imagelist            
            FlShell.SystemImageList.UseSystemImageList(listView);
        }

        /// <summary>
        /// Click on a Group => navigate to the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_GroupHeaderClick(object sender, int e)
        {
            int i = 0;
            string path = string.Empty;

            foreach (ListViewGroup lvg in listView.Groups)
            {
                //Console.Write("\n" + lvg.Header);
                i = PropertyHelper.GetPrivatePropertyValue<int>(lvg, "ID");
                if (i == e)
                {
                    path = lvg.Tag.ToString();                     
                    if (!Directory.Exists(path))
                    {
                        MessageBox.Show("This path does not exists:" + "\n<" + path + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    string file = Path.GetFileName(path);

                    NavigateTo?.Invoke(this, path, file);
                    return;
                }
            }            
        }

        /// <summary>
        /// Init listview
        /// </summary>
        private void ResetListView()
        {                       
            listView.Items.Clear();            
        }

        /// <summary>
        /// Switch view to another style (gouped by author or detailed list of files)
        /// </summary>
        private void RecreateSearchView()
        {
            Cursor.Current = Cursors.WaitCursor;            
            listView.ShowGroups = false;
            listView.BeginUpdate();
            ResetListView();
            PopulateListView();

            if (m_View == SearchViewStyle.Author)
            {
                listView.ShowGroups = true;
                CreateSearchViewByGroups();                
            }

            listView.EndUpdate();

            // select first element
            if (listView.Items.Count > 0)
            {

                if (listView.Groups.Count > 0)
                {
                    if (listView.Groups[0].Items.Count > 0)
                        listView.Groups[0].Items[0].Selected = true;
                }
                else
                    listView.Items[0].Selected = true;
            }
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Populate listview
        /// </summary>
        private void PopulateListView()
        {
            string strExtension = string.Empty; //Extension d'un fichier
            string fullname = string.Empty;
            string shellFullName = string.Empty;
            string fileName = string.Empty;
            string Song = string.Empty;
            long fileSize = 0;
            string fSize = string.Empty;
            string fType = string.Empty;
            string fName = string.Empty;
            string fLastWriteTime = string.Empty;

            int n = 0;
            ListViewItem item;
            
            ResetListView();

            try
            {
                for (int i = 0; i < filesFound.Count; i++)
                {
                    fullname = filesFound[i].FullName;

                    //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                    shellFullName = "file:///" + fullname.Replace("\\", "/");
                    FlShell.ShellItem shitem = new FlShell.ShellItem(shellFullName);

                    FileInfo finfo = filesFound[i];
                    fileName = finfo.Name;
                    
                    switch (m_View)
                    {
                        // View sorted by author
                        case SearchViewStyle.Author:
                            string sname = Path.GetFileNameWithoutExtension(fullname);
                            n = sname.IndexOf(" - ");
                            if (n > 0)
                            {
                                Song = sname.Substring(n + 3);
                            }
                            else
                            {
                                Song = sname;
                            }
                            fName = Song;
                            break;

                        case SearchViewStyle.File:
                            fName = fileName;
                            break;
                    }

                    fType = shitem.TypeName;
                    fileSize = finfo.Length;

                    if (fileSize > 1024)
                        fSize = string.Format("{0: #,### Ko}", fileSize / 1024);
                    else
                        fSize = string.Format("{0: ##0 Bytes}", fileSize);

                    fLastWriteTime = shitem.LastWriteTime.ToString("dd/MM/yyyy HH:mm");

                    item = new ListViewItem(new[] { fName, fSize, fLastWriteTime, fType })
                    {
                        // fore color
                        UseItemStyleForSubItems = false
                    };
                    item.SubItems[1].ForeColor = Color.Gray;
                    item.SubItems[2].ForeColor = Color.Gray;
                    item.SubItems[3].ForeColor = Color.Gray;

                    // Put the full path in the tag
                    item.Tag = fullname; //finfo.FullName;               

                    // Icon
                    item.ImageIndex = FlShell.SystemImageListManager.GetIconIndex(shitem, false);

                    // Finally add the listviewitem
                    listView.Items.Add(item);
                }                
            }
            catch (Exception ee)
            {
                string tx = ee.Message;
                tx += "\n\nYou should scan again your library.";
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Cursor.Current = Cursors.Default;
            }
        }

        #endregion


        #region Listview events

        private void ListView_Keydown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                // Initialize text entering in the list view
                case Keys.F2:                    
                    if (listView.SelectedItems.Count > 0)
                    {
                        listView.SelectedItems[0].BeginEdit();                      
                    }
                    break;
                    

                case Keys.F4:
                    ReplaceAllQuestion();
                    break;

                // Abort text entering in the list view
                case Keys.Escape:
                    {
                        
                        break;
                    }

                case Keys.Delete:
                    {
                        DeleteFile();
                        break;
                    }

                case Keys.Space:
                    {
                        // Info on midi file
                        OnMidiInfo?.Invoke();
                        break;
                    }

                case Keys.Enter:
                    // Raise event Play
                    InvokePlayEdit(true);
                    break;
            }

        }

        private void ListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            
        }

        private void ListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {            
            string fullname = (string)listView.Items[e.Item].Tag;
            string path = Path.GetDirectoryName(fullname);
            string oldName = Path.GetFileName(fullname);
            string NewName = string.Empty;
            

            //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
            string shellFullName = "file:///" + fullname.Replace("\\", "/");
            FlShell.ShellItem shitem = new FlShell.ShellItem(shellFullName);

            if (e.Label != null && e.Label != String.Empty && listView.SelectedItems.Count == 1)
            {
                #region Rename One               

                switch (m_View)
                {
                    case SearchViewStyle.Author:
                        // only the "author - song" is being changed   
                        try
                        {
                            NewName = listView.Items[e.Item].Group.Header + " - " + e.Label.Trim() + Path.GetExtension(fullname);
                        }
                        catch (Exception ex)
                        {
                            // Sometime the file does not respect the format "author - song", so listView.Items[e.Item].Group.Header is null
                            MessageBox.Show("Rename not done, unable to find the author of this song.", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            e.CancelEdit = true;
                            return;
                        }
                        break;

                    case SearchViewStyle.File:
                        NewName = e.Label.Trim();
                        break;
                }
                

                IntPtr newPidl = IntPtr.Zero;
                try
                {
                    bChanged = true;

                    uint res = shitem.Parent.GetIShellFolder().SetNameOf(listView.Handle, FlShell.Interop.Shell32.ILFindLastID(shitem.Pidl), NewName, FlShell.Interop.SHGDN.NORMAL, out newPidl);                   

                }
                catch (COMException ex)
                {
                    // Ignore the exception raised when the user cancels
                    // a delete operation.
                    if (ex.ErrorCode != unchecked((int)0x800704C7) &&
                        ex.ErrorCode != unchecked((int)0x80270000) &&
                        ex.ErrorCode != unchecked((int)0x8000FFFF))
                    {
                        throw;
                    }
                    e.CancelEdit = true;

                }

                #endregion
            }
        }

        /// <summary>
        /// Extract song from full file name 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private string GetSongName(string fullName)
        {            
            string song = string.Empty;
            string sname = Path.GetFileNameWithoutExtension(fullName);
            int n = sname.IndexOf(" - ");
            if (n > 0)
            {
                song = sname.Substring(n + 3);
            }
            else
            {
                song = sname;
            }
            return song;
        }


        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedFile != null)
            {
                lblDirSong.Text = SelectedFile;
                SelectedIndexChanged?.Invoke(this, SelectedFile);
            }
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            // Raise event Play
            InvokePlayEdit(true);
        }

        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && SelectedFile != null)
            {

                string song = SelectedFile;
                if (File.Exists(song) == true)
                {
                    LoadPlaylists();

                    lvContextMenu = new ContextMenuStrip();
                    lvContextMenu.Items.Clear();

                    // Menu delete                
                    ToolStripMenuItem menuDelete = new ToolStripMenuItem(Strings.deleteMenu);
                    lvContextMenu.Items.Add(menuDelete);

                    menuDelete.Click += new System.EventHandler(MnuDeleteSearchlistItem_Click);
                    menuDelete.ShortcutKeys = Keys.Delete;
                    menuDelete.Image = Karaboss.Properties.Resources.Actions_delete_icon;


                    // Menu Play
                    ToolStripMenuItem menuPlay = new ToolStripMenuItem(Strings.play);
                    lvContextMenu.Items.Add(menuPlay);

                    menuPlay.Click += new EventHandler(MnuPlaySearchlistItem_Click);
                    menuPlay.Image = Karaboss.Properties.Resources.Action_Play_icon24;

                    // Menu Edit
                    ToolStripMenuItem menuEdit = new ToolStripMenuItem(Strings.edit);
                    lvContextMenu.Items.Add(menuEdit);

                    menuEdit.Click += new EventHandler(MnuEditSearchlistItem_Click);
                    menuEdit.Image = Karaboss.Properties.Resources.Action_Edit;

                    // Playlists
                    // Menu add to playlist
                                        
                    ToolStripMenuItem mnulvAddToPlayList = new ToolStripMenuItem(Strings.addToPlaylist);
                    lvContextMenu.Items.Add(mnulvAddToPlayList);
                    mnulvAddToPlayList.Image = Karaboss.Properties.Resources.Action_Playlist_icon;

                    CreateMenusAddToPlaylists(mnulvAddToPlayList, PlGroup);                  

                    // Open folder
                    ToolStripMenuItem menuOpenFolder = new ToolStripMenuItem(Strings.OpenFolder);
                    lvContextMenu.Items.Add(menuOpenFolder);

                    menuOpenFolder.Click += new System.EventHandler(MnuOpenFolder_Click);                    
                    menuOpenFolder.Image = Karaboss.Properties.Resources.Action_Folder_icon;


                    // Display menu on the listview                    
                    lvContextMenu.Show(listView, listView.PointToClient(Cursor.Position));
                }
            }
            else if (e.Button == MouseButtons.Left && SelectedFile != null)
            {
                //ListViewItem lvi = listView.SelectedItems[0]; 
                //NavigateFolder(lvi);
            }
        }

        private void MnuOpenFolder_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0)
                return;

            ListViewItem lvi = listView.SelectedItems[0];
            NavigateFolder(lvi);
        }

        /// <summary>
        /// Navigate to a folder
        /// </summary>
        /// <param name="lvi"></param>
        private void NavigateFolder(ListViewItem lvi)
        {
            string FullPath = lvi.Tag.ToString();

            if (FullPath == string.Empty)            
                return;            

            string path = Path.GetDirectoryName(FullPath);
            if (!Directory.Exists(path))
            {
                MessageBox.Show("This path does not exists:" + "\n<" + path + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string file = Path.GetFileName(FullPath);

            NavigateTo?.Invoke(this, path, file);
        }

        private void MnuEditSearchlistItem_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(false);
        }

        private void MnuPlaySearchlistItem_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(true);
        }

        private void MnuDeleteSearchlistItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0)
                return;

            foreach (ListViewItem eachItem in listView.SelectedItems)
            {
                listView.Items.Remove(eachItem);
            }
        }

        /// <summary>
        /// Cursor = hand when mouse is over a group, indcating that we can navigate to its folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_View != SearchViewStyle.Author)
                return;

            Point cp = listView.PointToClient(Cursor.Position);
            ListViewItem itemOver = listView.GetItemAt(cp.X, cp.Y);            

            if (itemOver != null)            
                Cursor = Cursors.Default;                                
            else
               Cursor = Cursors.Hand;                                                     
        }

        private void ListView_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        #endregion


        #region Drag Drop

        // idea: possibility to drag & drop files from and to the search results area

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {            
            List<string> selection = new List<string>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                //int imgIndex = item.ImageIndex;
                selection.Add(item.Tag.ToString());
                //selection.Add(item.Text);
            }
            DataObject data = new DataObject(DataFormats.FileDrop, selection.ToArray());
            DoDragDrop(data, DragDropEffects.Move);
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {

        }

        private void ListView_DragLeave(object sender, EventArgs e)
        {

        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            
            Point cp = listView.PointToClient(new Point(e.X, e.Y));
            ListViewItem itemOver = listView.GetItemAt(cp.X, cp.Y);            

            if (itemOver == null)
                return;

            //Console.Write(itemOver.Text);
            //Console.Write(itemOver.Tag);
            string DestFile = itemOver.Tag.ToString();

            if (DestFile == string.Empty)            
                return;

            string OrgPath = string.Empty;
            string OrgFileName = string.Empty;
            string DestPath = Path.GetDirectoryName(DestFile);
            if (!Directory.Exists(DestPath))
            {
                MessageBox.Show("This path does not exists:" + "\n<" + DestPath + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                var Files = (string[])e.Data.GetData(DataFormats.FileDrop);
                
                // Move each dragged file
                foreach (var OrgFullFileName in Files)
                {
                    Console.WriteLine($"  File={OrgFullFileName}");
                    OrgPath = Path.GetDirectoryName(OrgFullFileName);
                    OrgFileName = Path.GetFileName(OrgFullFileName);

                    // Files already exists ?
                    try 
                    {
                        System.IO.File.Move(Path.Combine(OrgPath, OrgFileName), Path.Combine(DestPath, OrgFileName));
                    }
                    catch (Exception ex)
                    {

                    }                    
                }
            }
        }

        #endregion

        #endregion


        #region buttons

        // ADD a button Search
        protected override void OnLoad(EventArgs e)
        {
            btnSearch = new Button() {
                Size = new Size(25, txtSearch.ClientSize.Height + 2),
                Location = new Point(txtSearch.ClientSize.Width - 25, -1),
                Cursor = Cursors.Default,
                Image = Properties.Resources.Action_Search_icon,                
            };
            
            txtSearch.Controls.Add(btnSearch);

            btnSearch.Click += new EventHandler(btnSearch_Click);

            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(txtSearch.Handle, 0xd3, (IntPtr)2, (IntPtr)(btnSearch.Width << 16));

            btnClear = new Button()
            {
                Size = new Size(25, txtSearch.ClientSize.Height + 2),
                Location = new Point(txtSearch.ClientSize.Width - 50, -1),
                Cursor = Cursors.Default,
                Image = Properties.Resources.delete_icon,
            };
            txtSearch.Controls.Add(btnClear);
            btnClear.Click += new EventHandler(btnClear_Click);


            btnSearchDir = new Button() {
                Size = new Size(25, txtSearchDir.ClientSize.Height + 2),
                Location = new Point(txtSearchDir.ClientSize.Width - 25, -1),
                Cursor = Cursors.Default,
                Image = Properties.Resources.Action_Folder_icon,
            };
            txtSearchDir.Controls.Add(btnSearchDir);

            btnSearchDir.Click += new EventHandler(BtnSearchDir_Click);

            // Send EM_SETMARGINS to prevent text from disappearing underneath the button
            SendMessage(txtSearch.Handle, 0xd3, (IntPtr)2, (IntPtr)(btnSearchDir.Width << 16));

            // listview appearance
            ShellAPI.SetWindowTheme(listView.Handle, "explorer", null);
            
            base.OnLoad(e);
        }

        /// <summary>
        /// Change Library path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearchDir_Click(object sender, EventArgs e)
        {
            string oldpath = txtSearchDir.Text.Trim();

            string inipath = GetRootDirectory(txtSearchDir.Text.Trim());
            if (inipath != null && inipath != oldpath)
            {
                txtSearchDir.Text = inipath;
                Properties.Settings.Default.SongRoot = inipath;
                Properties.Settings.Default.Save();

                // Library has changed => rescan
                BtnScanMethod();

                // Save scanned file                
                SaveScanFiles(ScanfileName);

                // Raise event songRoot changed
                _songroot = inipath;
                SongRootChanged?.Invoke(this, _songroot);
            }
        }

        /// <summary>
        /// Search for files using file names
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            BtnSearchMethod();
        }

        /// <summary>
        /// Clear Search box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            this.txtSearch.Clear();
            txtSearch.Focus();
        }



        /// <summary>
        /// Button: scan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnScan_Click(object sender, EventArgs e)
        {
            // Search files
            BtnScanMethod();

            //Save scanned files            
            SaveScanFiles(ScanfileName);
        }
        
        /// <summary>
        /// Button: launch the scan of a directory and sub-directories
        /// </summary>
        private void BtnScanMethod()
        {            
            string path = txtSearchDir.Text;
            if (Directory.Exists(path) == false)
            {
                MessageBox.Show("Please select a valid folder for the search", "Karaboss", MessageBoxButtons.OK);
                return;
            }
            
            DirectoryInfo di = new DirectoryInfo(path);

            string[] filesExtension = ".mid,.kar".Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); //Get all files with specified extensions 

            // Launch search            
            Cursor.Current = Cursors.WaitCursor;
            lblIndexedFilesNumber.Text = "scanning in progress...";
            lblIndexedFilesNumber.Refresh();
            btnScan.Enabled = false;
            
            // Clear lists
            allFiles.Clear();
            
            // scan directories and stor result in files, folders and allFiles
            FullDirList(di, filesExtension);            

            lblIndexedFilesNumber.Text = allFiles.Count.ToString();
            // file renamed taken into account 
            bChanged = false;
            
            SearchContentChanged?.Invoke(this, "Scan completed: " + lblIndexedFilesNumber.Text + " files found");
            
            Cursor.Current = Cursors.Default;
            btnScan.Enabled = true;            
        }   

        /// <summary>
        /// Toolbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnutbAddtoPlayList_Click(object sender, EventArgs e)
        {            
            CreateMenusAddToPlaylists(mnutbAddtoPlayList, PlGroup);
        }

        /// <summary>
        /// Remove song from results (not really deleted)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoveFromSearch_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0)
                return;

            foreach (ListViewItem eachItem in listView.SelectedItems)
            {
                listView.Items.Remove(eachItem);
            }
        }

        /// <summary>
        /// Play song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(true);
        }

        /// <summary>
        /// Edit song
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(false);
        }

        private void InvokePlayEdit(bool bplay)
        {
            if (SelectedFile != null)
            {
                string file = SelectedFile;
                string ext = Path.GetExtension(file);
                switch (ext.ToLower())
                {
                    case ".mid":
                    case ".kar":
                        {
                            PlayMidi?.Invoke(this, new FileInfo(file), null, bplay);
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
        }


        #endregion


        #region methods


        #region scanning 
        /// <summary>
        /// Guess if scan all files is necessary
        /// </summary>
        private void GuessScanFiles()
        {            
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)                            
                return;           

            try
            {                                
                if ( !File.Exists(ScanfileName) )
                {                    
                    // Populate songs
                    BtnScanMethod();

                    // Save scanned file
                    SaveScanFiles(ScanfileName);
                }
                else
                {                    
                    allFiles = LoadScanFiles(ScanfileName);
                    lblIndexedFilesNumber.Text = allFiles.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// Load xml file containing all files of the library
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private List<Recording> LoadScanFiles(string fileName)
        {
            List<Recording> pls;
            // Open file containing all playlists "playlists.xml"
            try
            {
                using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<Recording>));
                    if (fs.Length > 0)
                        pls = (List<Recording>)xml.Deserialize(fs);
                    else
                        pls = new List<Recording>();
                }
                return pls;
            }
            catch (Exception enn)
            {

                Console.Write("Error loading Scan Files" + enn.Message);
                //return null;
                return new List<Recording>();
            }
        }

        /// <summary>
        /// Save xml file scanned
        /// </summary>
        /// <param name="fileName"></param>
        private void SaveScanFiles(string fileName)
        {
            try
            {                
                XmlSerializer serializer = new XmlSerializer(typeof(List<Recording>));
                TextWriter textWriter = new StreamWriter(@fileName);
                serializer.Serialize(textWriter, allFiles);
                textWriter.Close();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Dialog to change Library path 
        /// </summary>
        /// <returns></returns>
        private string GetRootDirectory(string iniPath)
        {
            if (iniPath == "C:\\")
                fldDialog.RootFolder = Environment.SpecialFolder.Desktop;
            else
                fldDialog.SelectedPath = iniPath;

            if (this.fldDialog.ShowDialog() == DialogResult.OK)
            {
                return fldDialog.SelectedPath;
            }
            else
                return null;
        }

        /// <summary>
        /// Scan loop directories and add files matching the extensions to a "FileInfo" List 
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filesExtension"></param>
        private void FullDirList(DirectoryInfo dir, string[] filesExtension)
        {
            // list the files
            try
            {
                foreach (FileInfo f in dir.EnumerateFiles().Where(f => filesExtension.Contains(f.Extension.ToLower())).ToArray())
                {
                    //files.Add(f);
                    allFiles.Add(new Recording(f.FullName));
                }
            }
            catch
            {
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                //folders.Add(d);
                FullDirList(d, filesExtension);
            }
        }
        #endregion


        #region searching and grouping

        /// <summary>
        /// Button: search
        /// </summary>
        private void BtnSearchMethod()
        {            
            string searchPattern = txtSearch.Text;

            #region verification
            if (bChanged)
            {
                MessageBox.Show("A file was modified, please scan again files before searching", "Karaboss", MessageBoxButtons.OK);
                return;
            }

            if (lblIndexedFilesNumber.Text == "0")
            {
                MessageBox.Show("Please scan files before searching", "Karaboss", MessageBoxButtons.OK);
                return;
            }
            
            if (searchPattern.Trim() == "")
            {
                MessageBox.Show("Nothing to search?", "Karaboss", MessageBoxButtons.OK);
                return;
            }
            #endregion

            Cursor.Current = Cursors.WaitCursor;

            filesFound.Clear();
            btnSearch.Enabled = false;

            // Search pattern in allfiles
            Application.DoEvents();
            SearchContentChanged?.Invoke(this, "Search pattern");
            Application.DoEvents();
            SearchFiles(searchPattern);


            // Début peuplement
            listView.BeginUpdate();

            // Populate result
            Application.DoEvents();
            SearchContentChanged?.Invoke(this, "Populate ListView");
            Application.DoEvents();

            PopulateListView();


            // Group by artist
            if (m_View == SearchViewStyle.Author)
                 CreateSearchViewByGroups();                                                      

            // end of Listview update
            listView.EndUpdate();

            Cursor.Current = Cursors.Default;


            // select first element
            if (listView.Items.Count > 0)
            {

                if (listView.Groups.Count > 0)
                {
                    if (listView.Groups[0].Items.Count > 0)
                        listView.Groups[0].Items[0].Selected = true;
                }
                else
                    listView.Items[0].Selected = true;
            }

            // display end of job
            Application.DoEvents();
            SearchContentChanged?.Invoke(this, "Search completed: " + listView.Items.Count + " files found");
            Application.DoEvents();

            Cursor.Current = Cursors.Default;
            btnSearch.Enabled = true;
        }


        private void CreateSearchViewByGroups()
        {
      
            // Create the groupsTable
            Application.DoEvents();
            SearchContentChanged?.Invoke(this, "Create the groups table by artist");
            Application.DoEvents();
            groupTables = CreateGroupsTable(0);

            // Start with the groups created for the Title column.
            SearchContentChanged?.Invoke(this, "Display data according to groups/artist");
            SetGroups(0);

        }

        /// <summary>
        /// Search a string pattern in the list of files and populate filesFound
        /// </summary>
        /// <param name="searchPattern"></param>
        private void SearchFiles(string searchPattern)
        {

            // Search case insensitive and accent insensitive (!)
            // Source: http://tijaquim.blogspot.fr/2007/12/c-procurar-strings-desprezando-os.html

            // input : allFiles
            // output : filesFound

            CompareInfo ci;
            CompareOptions co;
            if (bCaseSensitive == false)
            {
                ci = new CultureInfo("en-US").CompareInfo;
                co = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;
            }
            else
            {
                ci = new CultureInfo("fr-FR").CompareInfo;
                co = CompareOptions.None;
            }

            int pos = 0;
            string fName = string.Empty;
            string fNameOnly = string.Empty;
           
            foreach (Recording f in allFiles)
            {
                if (bSearchNameOnly)
                    fName = Path.GetFileNameWithoutExtension(f.Song);
                else if (bSearchSongOnly)
                {
                    fName = GetSongName(f.Song);
                }
                else
                    fName = f.Song;

                pos = ci.IndexOf(fName, searchPattern, co);
                if (pos >= 0)
                {
                    FileInfo fi = new FileInfo(f.Path);
                    filesFound.Add(fi);                    
                }
            }
            
        }           

        // Creates a Hashtable object with one entry for each unique
        // subitem value (or initial letter for the parent item)
        // in the specified column.
        private Hashtable CreateGroupsTable(int column)
        {
            // Create a Hashtable object.
            Hashtable groups = new Hashtable();
            string Artist = string.Empty;
            int n = 0;
            string file = string.Empty;
            string fullpath = string.Empty;
            string path = string.Empty;

            // Iterate through the items in myListView.
            foreach (ListViewItem item in listView.Items)
            {
                // Retrieve the text value for the column.
                string subItemText = item.SubItems[column].Text;

                fullpath = item.Tag.ToString();
                file = Path.GetFileName(fullpath);
                path = Path.GetDirectoryName(fullpath);

                // Bug if artist has a "-" in his name or if the song has also one ...

                //n = file.IndexOf("-");
                n = file.IndexOf(" - ");

                if (n > 0)
                {
 
                    Artist = file.Substring(0, n);
                    // If the groups table does not already contain a group
                    // for the subItemText value, add a new group using the 
                    // subItemText value for the group header and Hashtable key.
                    if (!groups.Contains(Artist))
                    {
                        ListViewGroup lvg = new ListViewGroup();
                        lvg.Header = Artist;
                        lvg.HeaderAlignment = HorizontalAlignment.Left;
                        lvg.Tag = path;

                        //groups.Add(Artist, new ListViewGroup(Artist, HorizontalAlignment.Left));
                        groups.Add(Artist, lvg);
                    }

                }              
            }

            // Return the Hashtable object.
            return groups;
        }

        // Sets myListView to the groups created for the specified column.
        private void SetGroups(int column)
        {
            int n = 0;
            string Artist = string.Empty;
            string file = string.Empty;

            // Remove the current groups.
            listView.Groups.Clear();

            // Retrieve the hash table corresponding to the column.
            Hashtable groups = groupTables;

            // Copy the groups for the column to an array.
            ListViewGroup[] groupsArray = new ListViewGroup[groups.Count];
            groups.Values.CopyTo(groupsArray, 0);

            // Sort the groups and add them to myListView.
            Array.Sort(groupsArray, new ListViewGroupSorter(listView.Sorting));
            listView.Groups.AddRange(groupsArray);

            // Iterate through the items in myListView, assigning each 
            // one to the appropriate group.
            foreach (ListViewItem item in listView.Items)
            {
                // Retrieve the subitem text corresponding to the column.
                string subItemText = item.SubItems[column].Text;

                file = item.Tag.ToString();
                file = Path.GetFileName(file);

                n = file.IndexOf(" - ");
                if (n > 0)
                {
                    Artist = file.Substring(0, n);
                    item.Group = (ListViewGroup)groups[Artist];
                }
                                 
            }

            // Clear all
            groups.Clear();
            groupsArray = new ListViewGroup[0];

        }

        #endregion


        // Sorts ListViewGroup objects by header value.
        private class ListViewGroupSorter : IComparer
        {
            private SortOrder order;

            // Stores the sort order.
            public ListViewGroupSorter(SortOrder theOrder)
            {
                order = theOrder;
            }

            // Compares the groups by header value, using the saved sort
            // order to return the correct value.
            public int Compare(object x, object y)
            {
                int result = String.Compare(
                    ((ListViewGroup)x).Header,
                    ((ListViewGroup)y).Header
                );
                if (order == SortOrder.Ascending)
                {
                    return result;
                }
                else
                {
                    return -result;
                }
            }
        }

        /// <summary>
        /// Locate form
        /// </summary>
        /// <typeparam name="TForm"></typeparam>
        /// <returns></returns>
        private TForm GetForm<TForm>()
            where TForm : Form
        {
            return (TForm)Application.OpenForms.OfType<TForm>().FirstOrDefault();
        }

        #endregion methods


        #region playlists

        /// <summary>
        /// Load existing playlists into allPlaylists collection
        /// </summary>
        private void LoadPlaylists()
        {
            // not executed in DesignMode
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            try
            {
                string fileName = Karaclass.GetPlaylistGroupFile(PlGroupHelper.File);
                PlGroupHelper.File = fileName;
                PlGroup = PlGroupHelper.Load(fileName);                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        /// <summary>
        /// Save all playlists
        /// </summary>
        private void SaveAllPlaylist()
        {
            string fName = Karaclass.M_filePlaylistGroups;
            PlGroupHelper.Save(fName, PlGroup);           
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

                ToolStripMenuItem folderMenu = new ToolStripMenuItem() {
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

                ToolStripMenuItem folderMenu = new ToolStripMenuItem() {
                    Text = folder,
                    Tag = Key,
                };


                // Add submenus composed of playlist names
                for (int j = 0; j < plgi.Playlists.Count; j++)
                {
                    ToolStripMenuItem plitem = new ToolStripMenuItem() {
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
        /// Add a song to an existing playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuAddToExistingPlaylist_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0)
                return;

            int nbAdded = 0;
            int nbTotal = listView.SelectedItems.Count;

            // Name of the target playlist
            ToolStripMenuItem m = (ToolStripMenuItem)sender;

            string Key = m.Tag.ToString();
            string name = m.Text;
            PlaylistGroupItem plg = PlGroupHelper.Find(PlGroup, Key);

            Playlist pltarget = plg.Playlists.Where(z => z.Name == name).FirstOrDefault();

            foreach (ListViewItem eachItem in listView.SelectedItems)
            {
                string Artist = "<Artist>";
                string File = eachItem.Tag.ToString();
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
            MessageBox.Show(nbAdded + "/" + nbTotal + " songs added to <" + name + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Add the songs to a new playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuAddToNewPlaylist_Click(object sender, EventArgs e)
        {
            if (SelectedFile != null)
            {
                List<string> li = new List<string>();
                foreach (ListViewItem eachItem in listView.SelectedItems)
                {
                    li.Add(eachItem.Tag.ToString());
                }

                FrmNewPlaylist frmNewPlaylist = new FrmNewPlaylist(li);
                frmNewPlaylist.ShowDialog();

                LoadPlaylists();
            }
        }


        #endregion playlists


        #region events

        /// <summary>
        /// Lanch search with keyboard ENTER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        BtnSearchMethod();
                        break;
                    }
            }
        }

        /// <summary>
        /// Allow case sensitive search or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            bCaseSensitive = chkCaseSensitive.Checked;
        }

        /// <summary>
        /// Search only in the filename (exclude extension ".mid", ".kar" ...)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkNameOnly_CheckedChanged(object sender, EventArgs e)
        {
            bSearchNameOnly = chkNameOnly.Checked;
            if (bSearchNameOnly)
            {
                bSearchSongOnly = false;
                chkSongOnly.Checked = false;
            }
        }

        private void ChkSongOnly_CheckedChanged(object sender, EventArgs e)
        {
            bSearchSongOnly = chkSongOnly.Checked;
            if (bSearchSongOnly)
            {
                bSearchNameOnly = false;
                chkNameOnly.Checked = false;
            }
        }

        /// <summary>
        /// Set focus on textbox used to enter search item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                txtSearch.Focus();
        }

        #endregion


        #region menu
        /// <summary>
        /// Search results sorted by author
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuViewByAuthor_Click(object sender, EventArgs e)
        {
            SView = SearchViewStyle.Author;
            mnuViewByAuthor.Checked = true;
            mnuViewByFile.Checked = false;
        }

        /// <summary>
        /// Search results sorted by file name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuViewByFile_Click(object sender, EventArgs e)
        {
            SView = SearchViewStyle.File;
            mnuViewByFile.Checked = true;
            mnuViewByAuthor.Checked = false;
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
            if (filesFound.Count > 0)
            {                             
                string tx = string.Empty;                    
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
                            int iren = ReplaceAll(txSearch, txReplace, filesFound);
                            RecreateSearchView();

                            tx = iren + " files renamed.";
                            if (iren > 0)
                            {
                                bChanged = true;
                                tx += "\n\n Please scan files again to take into account this renaming.";
                            }
                            MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
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
        private int ReplaceAll(string txSearch, string txReplace, List<FileInfo> files)
        {
            string newfileName = string.Empty;
            int iren = 0;

            try
            {              
                foreach (FileInfo file in files)
                {
                    string oldfileName = file.Name;
                    string ext = Path.GetExtension(oldfileName);
                    string wext = Path.GetFileNameWithoutExtension(oldfileName);
                    string nwext = string.Empty;
                    string physicalpath = file.FullName;

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
                        iren++;
                    }
                }

                return iren;
            }
            catch (Exception ep)
            {
                Console.WriteLine("The process failed: {0}", ep.ToString());
                MessageBox.Show(ep.Message);
                return iren;
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

                Form prompt = new Form() {
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

       
    }
}
