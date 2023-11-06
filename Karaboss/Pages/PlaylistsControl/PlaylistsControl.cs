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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Karaboss.Resources.Localization;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;

namespace Karaboss.playlists
{

    // Events
    public delegate void SelectedIndexChangedEventHandler(object sender, string fileName);
    public delegate void PlayMidiEventHandler(object sender, FileInfo fi, Playlist pl, bool bplay);
    public delegate void PlayAbcEventHandler(object sender, FileInfo fi, Playlist pl, bool bplay);
    public delegate void PlayTxtEventHandler(object sender, FileInfo fi, Playlist pl, bool bplay);
    public delegate void PlayXmlEventHandler(object sender, FileInfo fi, Playlist pl, bool bplay);
    public delegate void PlayCDGEventHandler(object sender, FileInfo fi, bool bplay);
    public delegate void NavigateToEventHandler(Object sender, string path, string file);


    public partial class PlaylistsControl : UserControl
    {
        // Events
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;
        public event PlayMidiEventHandler PlayMidi;
        public event PlayAbcEventHandler PlayAbc;
        public event PlayTxtEventHandler PlayTxt;
        public event PlayXmlEventHandler PlayXml;
        public event PlayCDGEventHandler PlayCDG;
        public event NavigateToEventHandler NavigateTo;

        #region playlists                
        private PlaylistGroup PlGroup = new PlaylistGroup();
        private PlaylistGroupsHelper PlGroupHelper = new PlaylistGroupsHelper();

        private PlaylistGroupItem currentPlaylistGroupItem = new PlaylistGroupItem();
        private Playlist currentPlaylist = new Playlist();
        private PlaylistItem currentplaylistItem = new PlaylistItem();

        private string selectedKey;

        private string currentFilePlaylists = string.Empty;
        #endregion

        #region tvDragDrop
        TreeNode tvDraggedNode;
        private string draggedKey = string.Empty;

        #endregion


        #region OpenDialog
        private string _lastFolder;

        #endregion

        // Context menus
        private ContextMenuStrip lvContextMenu;
        private ContextMenuStrip tvContextMenu;

        #region listView

        private int W_filename = 260;
        private int W_Singer = 120;
        private int W_Duration = 60;
        private int W_size = 60;
        private int W_modified = 120;
        private int W_type = 200;

        // The LVItem being dragged
        private ListViewItem _itemDnD = null;
        private ListViewItem _itemPhantom = null;
        private ListViewItem _itemCurrent = null;

        #endregion

        #region properties
        
        /// <summary>
        /// Duration of the MIDI file
        /// </summary>
        public string SelectedFileLength
        {
            get { return txtLength.Text.Trim(); }
            set
            {
                if (value != txtLength.Text)
                {
                    txtLength.Text = value;
                    currentplaylistItem.Length = txtLength.Text;
                    //UpdateSong();
                    // Udpdate duration in the listview
                    if (listView.Items.Count >= listView.SelectedIndices[0])
                        listView.Items[listView.SelectedIndices[0]].SubItems[2].Text = value;
                    // currentPlaylist Duration
                    lblPlaylistDuration.Text = currentPlaylist.Duration;
                }
            }
        }

        /// <summary>
        /// The selected file in the listview
        /// </summary>
        public string SelectedFile
        {
            get
            {
                if (listView.SelectedIndices.Count > 0)
                    if (listView.SelectedItems[0].Tag != null)
                        return listView.SelectedItems[0].Tag.ToString();
                    else
                        return null;
                else
                    return null;
            }
        }

        private string _songroot = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        /// <summary>
        /// the music library
        /// </summary>
        public string SongRoot
        {
            set { _songroot = value; }
        }

        private string m_selectedPlaylist;
        public string SelectedPlayList
        {
            get
            {
                return m_selectedPlaylist;
            }
        }

        #endregion

        private bool bModified = false;
        private int oldindex = 0;

        // Constructor
        public PlaylistsControl()
        {
            InitializeComponent();

            // Initialize Treeview
            InitTreeview();

        }

        #region functions

        /// <summary>
        /// Force focus to listview with TAB when focus is on treeview
        /// ProcessCmdKey save my life
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                if (tvPlaylistGroup.Focused)
                {
                    listView.Focus();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public override void Refresh() {
            string selectedPl = m_selectedPlaylist;                        
            InitListView();            
            // Load existing playlists
            LoadPlaylists();            
            CheckDrive(Karaclass.M_filePlaylistGroups);                      

            if (PlGroup.Count == 0)
            {
                // Create a single PlaylistGroupItem to add to the collection 
                PlGroup = new PlaylistGroup();                                           
                PlaylistGroupItem plgi = PlGroupHelper.CreateEmptyPlaylistGroupItem();
                PlGroup.Add(plgi);

                // Save all
                SaveAllPlaylist(); 
            }           
            // display playlists in the treeview                       
            PopulatetvPlaylistGroup(PlGroup, selectedKey);

            m_selectedPlaylist = selectedPl;
            tvPlaylistGroup.Focus();
            base.Refresh();
        }


        /// <summary>
        /// Gives "Windows Explorer" style to listview and treeview
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            ShellAPI.SetWindowTheme(listView.Handle, "explorer", null);            
            ShellAPI.SetWindowTheme(tvPlaylistGroup.Handle, "explorer", null);
            base.OnLoad(e);
        }

        /// <summary>
        /// Reset txtboxes of song details
        /// </summary>
        private void EmptyTx()
        {
            txtArtist.Text = "<Artist>";
            txtSong.Text = "<Song>";
            txtFile.Text = "";
            txtAlbum.Text = "<Album>";
            txtLength.Text = "00:00";
            txtNotation.Text = "4";
            txtDirSlideShow.Text = "<DirSlideShow>";
            chkMuteMelody.Checked = false;
            txtKaraokeSinger.Text = "<Song reserved by>";
        }

        /// <summary>
        /// Update display of song Currently played
        /// </summary>
        /// <param name="song"></param>
        public void DisplaySong(string song)
        {
            int index = 0;            
            for (int i = 0; i < listView.Items.Count; i++)
            {
                if (listView.Items[i].Text == song)
                {
                    index = i;
                    break;    
                }
            }
            if (index != -1)
            {
                // Unselect previous
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Selected = false;
                }

                // Select current
                if (listView.Items.Count > index)
                {
                    listView.Items[index].Selected = true;
                    listView.Items[index].EnsureVisible();
                }
            }
        }

        #endregion


        #region Listview

        private void InitListView()
        {
            // Affichage mode détails
            listView.View = View.Details;
            // Autoriser l'édition in place
            listView.LabelEdit = true;
            // keep selection visible
            listView.HideSelection = false;

            // Associer imagelist
            FlShell.SystemImageList.UseSystemImageList(listView);

            listView.FullRowSelect = true;

            listView.Clear();
            listView.Refresh();
            imgWnd.Images.Clear();

            #region header
            //Headers listview     
            ColumnHeader columnHeader0 = new ColumnHeader() {
                Text = Strings.fileName,
                Width = W_filename,
            };

            ColumnHeader columnHeader1 = new ColumnHeader() {
                Text = Strings.Singer,
                Width = W_Singer,
            };

            ColumnHeader columnHeader2 = new ColumnHeader()
            {
                Text = Strings.Duration,
                Width = W_Duration,
                TextAlign = HorizontalAlignment.Right,
            };


            ColumnHeader columnHeader3 = new ColumnHeader() {
                Text = Strings.Size,
                Width = W_size,
                TextAlign = HorizontalAlignment.Right,
            };


            ColumnHeader columnHeader4 = new ColumnHeader() {
                Text = Strings.Modified,
                Width = W_modified,
            };

            ColumnHeader columnHeader5 = new ColumnHeader() {
                Text = Strings.Type,
                Width = W_type,
            };

            // Add the column headers to myListView.
            listView.Columns.AddRange(new ColumnHeader[]
                {columnHeader0, columnHeader1, columnHeader2, columnHeader3, columnHeader4, columnHeader5});

            #endregion

            // NO Sorting
            listView.Sorting = SortOrder.None;
            listView.AllowDrop = true;            
        }

        private void PopulateListView(Playlist pl)
        {
            string strExtension = string.Empty; //Extension d'un fichier            
            Cursor.Current = Cursors.WaitCursor;
            InitListView();
            // Début peuplement           
            listView.BeginUpdate();

            try
            {
                string file = string.Empty;
                string fullname = string.Empty;
                string fileName = string.Empty;
                string KaraokeSinger = string.Empty;
                string Duration = String.Empty;
                //long fileSize = 0;
                //string fSize = string.Empty;
                //string fType = string.Empty;
                //ListViewItem item;
               
                var itemsToAdd = new List<ListViewItem>();
                // Optimization
                listView.ListViewItemSorter = null;

                for (int i = 0; i < pl.Songs.Count; i++)
                {
                    file = pl.Songs[i].File;
                    fileName = pl.Songs[i].Song;
                    KaraokeSinger = pl.Songs[i].KaraokeSinger;

                    itemsToAdd.Add(CreateItem(file, fileName, KaraokeSinger, Duration));
                }
                                
                listView.Items.AddRange(itemsToAdd.ToArray());                
            }
            catch (Exception ee)
            {
                Console.WriteLine("PopulateListview: the process failed: {0}", ee.ToString());
            }

            // fin peuplement           
            listView.EndUpdate();
            Cursor.Current = Cursors.Default;
        }

        private ListViewItem CreateItem(string file, string fileName, string KaraokeSinger, string Duration)
        {
            ListViewItem lvi;

            if (File.Exists(file))
            {
                FileInfo finfo = new FileInfo(file);
                //path must be like "file:///c:/users/a453868/Music/karaoke/sasin";
                string fullname = "file:///" + file.Replace("\\", "/");
                FlShell.ShellItem shitem = new FlShell.ShellItem(fullname);

                string fType = shitem.TypeName;
                long fileSize = finfo.Length;
                string fSize;
                if (fileSize > 1024)
                    fSize = string.Format("{0: #,### Ko}", fileSize / 1024);
                else
                    fSize = string.Format("{0: ##0 Bytes}", fileSize);


                lvi = new ListViewItem(new[] { fileName, KaraokeSinger, Duration, fSize, File.GetLastWriteTime(file).ToString("dd/MM/yyyy HH:mm"), fType })
                {
                    // false = Authorize change color for subitems
                    UseItemStyleForSubItems = false
                };
                lvi.SubItems[2].ForeColor = Color.Gray;
                lvi.SubItems[3].ForeColor = Color.Gray;
                lvi.SubItems[4].ForeColor = Color.Gray;
                lvi.SubItems[5].ForeColor = Color.Gray;
                // Put the full path in the tag
                lvi.Tag = finfo.FullName;
                // Icon
                lvi.ImageIndex = FlShell.SystemImageListManager.GetIconIndex(shitem, false);
            }
            else
            {
                // the file no more exists
                lvi = new ListViewItem(new[] { fileName, KaraokeSinger, "???", "???", "???", "???" })
                {
                    ForeColor = Color.Red,
                    // false = Authorize change color for subitems
                    UseItemStyleForSubItems = false,
                };

                lvi.SubItems[2].ForeColor = Color.Gray;
                lvi.SubItems[3].ForeColor = Color.Gray;
                lvi.SubItems[4].ForeColor = Color.Gray;
                lvi.SubItems[5].ForeColor = Color.Gray;
                lvi.Tag = file;
            }


            return lvi;
        }


        /// <summary>
        /// Selected index changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectedFile != null)
            {                
                int index = listView.SelectedItems[0].Index;
                string song = listView.SelectedItems[0].Text;

                // Save changes
                if (bModified)
                {
                    
                    currentplaylistItem.Artist = txtArtist.Text.Trim();
                    currentplaylistItem.Album = txtAlbum.Text.Trim();
                    currentplaylistItem.Length = txtLength.Text.Trim();

                    try
                    {
                        currentplaylistItem.Notation = Convert.ToInt32(txtNotation.Text.Trim());
                    }
                    catch
                    {
                        currentplaylistItem.Notation = 4;
                    }

                    currentplaylistItem.MelodyMute = chkMuteMelody.Checked;
                    currentplaylistItem.DirSlideShow = txtDirSlideShow.Text.Trim();
                    currentplaylistItem.KaraokeSinger = txtKaraokeSinger.Text.Trim();

                    SaveAllPlaylist();
                    
                    listView.Items[oldindex].SubItems[1].Text = currentplaylistItem.KaraokeSinger;                   
                }

                if (song != null)
                {
                    currentplaylistItem = GetPlaylistItem(song);
                    ShowPlaylistItem(currentplaylistItem);                   
                }

                // raise event
                SelectedIndexChanged?.Invoke(this, SelectedFile);
            }
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {
            LaunchPlayList();
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                listView.MultiSelect = true;
                foreach (ListViewItem item in listView.Items)
                {
                    item.Selected = true;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        // Raise event Play
                        LaunchPlayList();
                        break;

                    case Keys.Delete:
                        DeletePlaylistItems();
                        break;
                }
            }
        }
      

        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && SelectedFile != null)
            {
                lvContextMenu = new ContextMenuStrip();
                lvContextMenu.Items.Clear();

                // Menu delete                
                ToolStripMenuItem menuDelete = new ToolStripMenuItem(Strings.deleteMenu);
                lvContextMenu.Items.Add(menuDelete);

                menuDelete.Click += new System.EventHandler(MnuDeletePlaylistItem_Click);
                menuDelete.ShortcutKeys = Keys.Delete;
                menuDelete.Image = Karaboss.Properties.Resources.Actions_delete_icon;


                // Menu Play
                ToolStripMenuItem menuPlay = new ToolStripMenuItem(Strings.play);
                lvContextMenu.Items.Add(menuPlay);

                menuPlay.Click += new EventHandler(MnuPlayPlaylistItem_Click);
                menuPlay.Image = Karaboss.Properties.Resources.Action_Play_icon24;

                // Menu Edit
                ToolStripMenuItem menuEdit = new ToolStripMenuItem(Strings.edit);
                lvContextMenu.Items.Add(menuEdit);
                menuEdit.Click += new EventHandler(MnuEditPlaylistItem_Click);
                menuEdit.Image = Karaboss.Properties.Resources.Action_Edit;


                // Playlists
                #region playlists

                // Menu add to playlist
                ToolStripMenuItem mnulvAddToPlayList = new ToolStripMenuItem(Strings.addToPlaylist);
                lvContextMenu.Items.Add(mnulvAddToPlayList);
                mnulvAddToPlayList.Image = Karaboss.Properties.Resources.Action_Playlist_icon;
            
                CreateMenusAddToPlaylists(mnulvAddToPlayList, PlGroup);                         

                #endregion

                // Open folder
                ToolStripMenuItem menuOpenFolder = new ToolStripMenuItem(Strings.OpenFolder);
                lvContextMenu.Items.Add(menuOpenFolder);

                menuOpenFolder.Click += new System.EventHandler(MnuOpenFolder_Click);
                menuOpenFolder.Image = Karaboss.Properties.Resources.Action_Folder_icon;


                // Display menu
                lvContextMenu.Show(listView, listView.PointToClient(Cursor.Position));

            }
        }

        /// <summary>
        /// Navigate to the folder of the selected file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuOpenFolder_Click(object sender, EventArgs e)
        {
            if (listView.SelectedIndices.Count == 0)
                return;

            ListViewItem lvi = listView.SelectedItems[0];
            string FullPath = lvi.Tag.ToString();
            
            if (FullPath == string.Empty)
            {
                return;
            }

            string path = Path.GetDirectoryName(FullPath);
            if (!Directory.Exists(path))
            {
                MessageBox.Show("This path does not exists:" + "\n<" + path + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            string file = Path.GetFileName(FullPath);

            NavigateTo?.Invoke(this, path, file);            
        }


        #region Drag Drop

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _itemDnD = (ListViewItem)e.Item;
            _itemPhantom = null;
            _itemCurrent = null;
            
            DoDragDrop(listView.SelectedItems, DragDropEffects.Move);
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// End drag drop : move the drag item to its target location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            Point cp = listView.PointToClient(new Point(e.X, e.Y));
            ListViewItem itemOver = listView.GetItemAt(cp.X, cp.Y);


            if (itemOver == null || _itemDnD == null)
            {
                if (_itemPhantom != null)
                {
                    listView.Items.Remove(_itemPhantom);
                    _itemPhantom = null;
                }
                return;
            }

            if (_itemDnD == itemOver)
            {
                if (_itemPhantom != null)
                {
                    listView.Items.Remove(_itemPhantom);
                    _itemPhantom = null;
                }
                return;
            }

            int dropIndex = itemOver.Index;
            listView.Items.Remove(_itemDnD);
            listView.Items.Insert(dropIndex, _itemDnD);

            if (_itemPhantom != null)
            {
                listView.Items.Remove(_itemPhantom);
                _itemPhantom = null;
            }
        }

        /// <summary>
        /// Mouse move over the control: create a phantom item to show the place where to drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            Point cp = listView.PointToClient(new Point(e.X, e.Y));
            ListViewItem itemOver = listView.GetItemAt(cp.X, cp.Y);
            

            if (itemOver != _itemPhantom && itemOver != _itemCurrent && itemOver != _itemDnD)
            {

                if (_itemPhantom != null)
                {
                    listView.Items.Remove(_itemPhantom);
                    _itemPhantom = itemOver;
                }

                ListViewItem item = new ListViewItem(new[] { "---> insert here", "-", "-", "-", "-", "-" })
                {
                    Tag = "",
                };

                _itemPhantom = item;

                if (itemOver != null)
                {
                    if (_itemDnD != null && itemOver.Index != _itemDnD.Index + 1)
                        listView.Items.Insert(itemOver.Index, item);
                }
                else
                    listView.Items.Add(item);

                _itemCurrent = itemOver;
            }
            
        }

        /// <summary>
        /// Mouse is out of control => delete phantom
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_DragLeave(object sender, EventArgs e)
        {
            if (_itemPhantom != null)
            {
                listView.Items.Remove(_itemPhantom);
                _itemPhantom = null;
            }
        }
        #endregion

        #endregion


        #region treeView            

        private void InitTreeview()
        {
            // Treeview                    
            tvPlaylistGroup.HotTracking = true;
            tvPlaylistGroup.HideSelection = false;
            tvPlaylistGroup.Font = new Font("Segoe UI", 9F);
            tvPlaylistGroup.Indent = 19;
            tvPlaylistGroup.ItemHeight = 24;
            tvPlaylistGroup.AllowDrop = true;

        }

        private void PopulatetvPlaylistGroup(PlaylistGroup plg, string selectedKey = null)
        {
            PlaylistGroup myplg = new PlaylistGroup();
          
            
            for (int i = 0; i < plg.Count; i++)
            {
                myplg.Add(plg.plGroupItems[i]);
            }            

            int items = myplg.Count;

            tvPlaylistGroup.Nodes.Clear();
            tvPlaylistGroup.BeginUpdate();

            do
            {
                TryPopulatetvPlaylistGroup(myplg);
                items = myplg.Count;
            } while (items > 0);

            tvPlaylistGroup.EndUpdate();
            tvPlaylistGroup.ExpandAll();

            // Set focus to selected node
            SelectActiveNode(selectedKey);
        }

        private void TryPopulatetvPlaylistGroup( PlaylistGroup plg)
        {
            TreeNode tnPl;            
            bool bfoundParent = false;
           
            for (int i = 0; i < plg.Count; i++)
            {
                bfoundParent = false;

                PlaylistGroupItem item = plg.plGroupItems[i];
                               
                // Search for parent 
                TreeNode tnParent = null;
                if (item.ParentKey != null)
                {
                    if (tvPlaylistGroup.Nodes.Count > 0)
                    {
                        // Search for a treenode having a tag equal to ParentKey 
                        foreach (TreeNode node in tvPlaylistGroup.Nodes)
                        {
                            var result = FindString(item.ParentKey, node);
                            if (result != null)
                            {
                                tnParent = result;
                                bfoundParent = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.Write("\nItem skipped: unable to find parent node of " + item.Name);
                        bfoundParent = false;
                    }
                }
                else
                    bfoundParent = true;

                // 
                if (bfoundParent)
                {
                    // 1 - Create folder node named item.Name
                    TreeNode tnFolder = new TreeNode(item.Name) {
                        Name = item.Name,
                        ImageIndex = 0,
                        Tag = item.Key,
                    };

                    // 2 - Add folder node to parent (root or existing folder node)
                    if (tnParent != null)
                        tnParent.Nodes.Add(tnFolder);
                    else
                        tvPlaylistGroup.Nodes.Add(tnFolder);

                    #region add child playlists
                    // 3 - Add child nodes (playlists)
                    for (int j = 0; j < item.Playlists.Count; j++)
                    {
                        Playlist pl = item.Playlists[j];
                        tnPl = new TreeNode(pl.Name)
                        {
                            Tag = "playlist",
                        };

                        // image for treenode not selected
                        tnPl.ImageIndex = 1;
                        // image for selected treenode
                        tnPl.SelectedImageIndex = 2;

                        tnFolder.Nodes.Add(tnPl);
                    }
                    #endregion


                    // Remove from list
                    plg.plGroupItems.Remove(item);
                    break;                    
                }
            }                                            
        }

        private void SelectActiveNode(string selectedKey)
        {
            TreeNode tns = null;
            if (selectedKey != null)
            {
                if (tvPlaylistGroup.Nodes.Count > 0)
                {
                    foreach (TreeNode node in tvPlaylistGroup.Nodes)
                    {
                        var result = FindString(selectedKey, node);
                        if (result != null)
                        {
                            tns = result;
                            break;
                        }
                    }
                }
            }
            if (tns != null)
            {
                // Search m_selectedPlaylist 
                if (m_selectedPlaylist != null)
                {
                    foreach (TreeNode node in tns.Nodes)
                    {
                        if (node.Text == m_selectedPlaylist)
                        {
                            tvPlaylistGroup.SelectedNode = node;
                            node.EnsureVisible();
                            return;
                        }
                    }                                    
                }
                                
                tvPlaylistGroup.SelectedNode = tns;
                if (tns.LastNode != null)
                    tns.LastNode.EnsureVisible();
                else
                    tns.EnsureVisible();

                // Select first Playlist of this folder
                if (tns.Nodes.Count > 0 && tns.FirstNode.Tag.ToString() == "playlist")
                    tvPlaylistGroup.SelectedNode = tns.FirstNode;

            }
            else if (tvPlaylistGroup.Nodes.Count > 0)
            {                
                TreeNode tn = FindFirstPlaylist();
                if (tn != null)
                    tvPlaylistGroup.SelectedNode = tn;
                else
                    tvPlaylistGroup.SelectedNode = tvPlaylistGroup.Nodes[0];
            }
        }

        /// <summary>
        /// Find a string in nodes tag recursively
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        private TreeNode FindString(string strSearch, TreeNode rootNode)
        {
            if (rootNode.Tag.ToString() == strSearch) return rootNode;

            foreach (TreeNode node in rootNode.Nodes)
            {
                if (node.Tag.ToString() != "playlist")
                {
                    if (node.Tag.ToString().Equals(strSearch)) return node;

                    TreeNode next = FindString(strSearch, node);
                    if (next != null) return next;
                }
            }
            return null;
        }

        private TreeNode FindFirstPlaylist()
        {
            foreach (TreeNode node in tvPlaylistGroup.Nodes)
            {
                var result = SearchTag("playlist", node);
                if (result != null)                
                    return result;                                    
            }
            return null;
        }

        private TreeNode SearchTag(string Tag, TreeNode tnRoot)
        {
            if (tnRoot.Tag.ToString() == Tag) return tnRoot;
            foreach (TreeNode tn in tnRoot.Nodes)
            {
                if (tn.Tag.ToString() == Tag) return tn;
                TreeNode next = SearchTag(Tag, tn);
                if (next != null) return next;
            }
            return null;
        }

        /// <summary>
        /// Select another playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = tvPlaylistGroup.SelectedNode;
            
            if (tn != null && tn.Tag.ToString() == "playlist")
            {
                bModified = false;
                pnlSongDetails.Enabled = true;

                string plName = tn.Text;
                string Key = string.Empty;
                if (tn.Parent != null)
                    Key = tn.Parent.Tag.ToString();
                else
                    Key = tvPlaylistGroup.Nodes[0].Tag.ToString();

                selectedKey = Key;

                EmptyTx();

                if (plName != null)
                {
                    if (PlGroup != null)
                    {
                        currentPlaylistGroupItem = PlGroupHelper.Find(PlGroup, Key);
                        currentPlaylist = PlGroupHelper.getPlaylistByName(PlGroup, plName, Key);

                        if (currentPlaylist != null)
                        {
                            m_selectedPlaylist = currentPlaylist.Name;
                            ShowPlContent(currentPlaylist, 0);

                            // Display playlist infos
                            DisplayPlaylistInfos();
                        }
                    }
                }
            } else if (tn != null)
            {
                m_selectedPlaylist = null;
                listView.Clear();
                DisplayPlaylistInfos(true);
                EmptyTx();
                pnlSongDetails.Enabled = false;
            }
        }

        // Display playlist infos
        private void DisplayPlaylistInfos(bool bClear = false)
        {
            if (bClear)
            {
                tabPage1.Text = "";                
                TxtPlaylistName.Text = "";
                lblPlaylistCount.Text = "";
                lblPlaylistDuration.Text = "";
                TxtStyle.Text = "";

            }
            else
            {
                tabPage1.Text = currentPlaylist.Name;
                TxtPlaylistName.Text = currentPlaylist.Name;
                lblPlaylistCount.Text = currentPlaylist.Count.ToString();
                lblPlaylistDuration.Text = currentPlaylist.Duration;
                TxtStyle.Text = currentPlaylist.Style;
            }
        }
           
        /// <summary>
        /// Treeview: End of Label edition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            this.BeginInvoke(new Action(() => AfterAfterEdit(e) ));            
        }

        private void AfterAfterEdit(NodeLabelEditEventArgs e)
        {
            string Key = string.Empty;

            if (e.Node.Tag.ToString() == "playlist")
            {
                // Playlist was renamed
                Key = e.Node.Parent.Tag.ToString();
                PlaylistGroupItem plgi = PlGroupHelper.Find(PlGroup, Key);
                
                // Check duplicate name
                if (PlGroupHelper.PlaylistExist(plgi.Playlists, e.Node.Text))
                {
                    // Dupliwate = cancel edit
                    e.CancelEdit = true;
                }
                else
                {
                    // OK => rename node
                    m_selectedPlaylist = e.Node.Text;
                    currentPlaylist.Name = e.Node.Text;

                    plgi.Sort();
                    SaveAllPlaylist();
                    LoadPlaylists();
                    PopulatetvPlaylistGroup(PlGroup, plgi.Key);
                }
            }
            else
            {
                // folder was renamed
                // Check duplicate name
                Key = e.Node.Tag.ToString();
                PlaylistGroupItem plgi = PlGroupHelper.Find(PlGroup, Key);
                
                if (PlGroup.PlaylistGroupItemExists(plgi, e.Node.Text))
                {
                    // Cancel edition
                    e.Node.Text = plgi.Name;
                    e.CancelEdit = true;
                }
                else
                {
                    // Accept rename
                    plgi.Name = e.Node.Text;

                    PlGroup.Sort();
                    SaveAllPlaylist();
                    LoadPlaylists();
                    PopulatetvPlaylistGroup(PlGroup, plgi.Key);
                }
            }           
        }

        /// <summary>
        /// Keydown Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F2:
                    tvPlaylistGroup.SelectedNode.BeginEdit();
                    break;

                case Keys.Delete:
                    DeletePlaylistGroup();
                    break;
            }

        }

        /// <summary>
        /// Display context menu on treeview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_MouseDown(object sender, MouseEventArgs e)
        {
            TreeNode tn = tvPlaylistGroup.GetNodeAt(e.X, e.Y);  // tvPlaylists.SelectedNode;

            if (e.Button == MouseButtons.Right && tn != null)
            {
                tvPlaylistGroup.SelectedNode = tn;

                tvContextMenu = new ContextMenuStrip();
                tvContextMenu.Items.Clear();

                // Menu rename playlist or folder
                ToolStripMenuItem menuRenamePl = new ToolStripMenuItem(Strings.renameMenu);
                tvContextMenu.Items.Add(menuRenamePl);
                menuRenamePl.Click += new System.EventHandler(MnuRenamePlaylists_Click);
                menuRenamePl.Image = Karaboss.Properties.Resources.Action_Edit;

                ToolStripSeparator menusep1 = new ToolStripSeparator();
                tvContextMenu.Items.Add(menusep1);

                // Menu create a folder
                ToolStripMenuItem menuCreateNewPlGroup = new ToolStripMenuItem(Strings.CreateNewFolder);
                tvContextMenu.Items.Add(menuCreateNewPlGroup);
                menuCreateNewPlGroup.Click += new System.EventHandler(MnuCreatePlaylistGroup_Click);
                menuCreateNewPlGroup.Image = Karaboss.Properties.Resources.Action_folder241;


                // Menu delete a folder
                ToolStripMenuItem menuDeletePlGroup = new ToolStripMenuItem(Strings.DeleteFolderAndPlaylists);
                tvContextMenu.Items.Add(menuDeletePlGroup);
                menuDeletePlGroup.Click += new System.EventHandler(MnuDeletePlaylistGroup_Click);
                menuDeletePlGroup.ShortcutKeys = Keys.Delete;
                menuDeletePlGroup.Image = Karaboss.Properties.Resources.Action_folder_delete24;

                ToolStripSeparator menusep2 = new ToolStripSeparator();
                tvContextMenu.Items.Add(menusep2);

                // Menu create a new playlist
                ToolStripMenuItem menuCreateNewPl = new ToolStripMenuItem(Strings.CreateNewPlaylist);
                tvContextMenu.Items.Add(menuCreateNewPl);
                menuCreateNewPl.Click += new System.EventHandler(MnutvCreatePlaylist_Click);
                menuCreateNewPl.Image = Karaboss.Properties.Resources.Action_New_File_icon;


                // Menu delete playlist                
                ToolStripMenuItem menuDelete = new ToolStripMenuItem(Strings.DeletePlaylist);
                tvContextMenu.Items.Add(menuDelete);
                menuDelete.Click += new System.EventHandler(MnutvDeletePlaylist_Click);
                menuDelete.ShortcutKeys = Keys.Delete;
                menuDelete.Image = Karaboss.Properties.Resources.Action_playlist_delete24;

                // Display menu
                tvContextMenu.Show(listView, listView.PointToClient(Cursor.Position));
            }

        }
        

        #region Context menus tvPlaylistGroup

        private void MnuDeletePlaylistGroup_Click(object sender, EventArgs e)
        {
            DeletePlaylistGroup();
        }

        private void MnuCreatePlaylistGroup_Click(object sender, EventArgs e)
        {
            CreateNewPlaylistGroup();
        }

        /// <summary>
        /// tvPlaylistgroup context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnutvCreatePlaylist_Click(object sender, EventArgs e)
        {
            CreateNewPlayListInit();
        }

        private void MnuRenamePlaylists_Click(object sender, EventArgs e)
        {
            tvPlaylistGroup.SelectedNode.BeginEdit();
        }

        /// <summary>
        /// Treview tvPlaylistGrpups: delete a playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnutvDeletePlaylist_Click(object sender, EventArgs e)
        {
            DeletePlaylist();
        }

        #endregion


        #region Dragdrop

        /// <summary>
        /// Start drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_ItemDrag(object sender, ItemDragEventArgs e)
        {
            tvDraggedNode = (TreeNode)e.Item;
            if (tvDraggedNode.Tag.ToString() == "playlist")
                draggedKey = tvDraggedNode.Parent.Tag.ToString();
            else
                draggedKey = tvDraggedNode.Tag.ToString();

            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        /// <summary>
        /// End drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvPlaylistGroup_DragDrop(object sender, DragEventArgs e)
        {
            bool bRefresh = false;
            string targetKey = string.Empty;

            // Retrieve the client coordinates of the drop location.
            Point targetPoint = tvPlaylistGroup.PointToClient(new Point(e.X, e.Y));

            // Retrieve the node at the drop location.
            TreeNode targetNode = tvPlaylistGroup.GetNodeAt(targetPoint);

            // Retrieve the node that was dragged.            
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            ListView.SelectedListViewItemCollection draggedLVI = (ListView.SelectedListViewItemCollection)e.Data.GetData(typeof(ListView.SelectedListViewItemCollection));

            PlaylistGroupItem draggedplgi = PlGroupHelper.Find(PlGroup, draggedKey);

            
            if (draggedNode != null)
            {
                #region dragdrop nodes
                
                // Confirm that the node at the drop location is not 
                // the dragged node and that target node isn't null
                // (for example if you drag outside the control)
                if (!draggedNode.Equals(targetNode) && targetNode != null)
                {
                    // Remove the node from its current 
                    // location and add it to the node at the drop location.
                    draggedNode.Remove();
                    if (targetNode.Tag.ToString() == "playlist")
                        targetNode = targetNode.Parent;

                    targetNode.Nodes.Add(draggedNode);

                    targetKey = targetNode.Tag.ToString();
                    PlaylistGroupItem targetplgi; //= PlGroupHelper.Find(PlGroup, targetKey);

                    // A playlist is dragged
                    if (draggedNode.Tag.ToString() == "playlist")
                    {
                        // Remove playlist from source
                        targetplgi = PlGroupHelper.Find(PlGroup, targetKey);

                        Playlist draggedplaylist = PlGroupHelper.getPlaylistByName(PlGroup, draggedNode.Text, draggedKey);
                        draggedplgi.Playlists.Remove(draggedplaylist);

                        // Add playlist to target                    
                        targetplgi.Playlists.Add(draggedplaylist);

                        targetplgi.Sort();

                        selectedKey = targetplgi.Key;

                    }
                    else
                    {                        
                        // A folder is dragged
                        // Parent key of dragged group = target group key
                        targetplgi = PlGroupHelper.Find(PlGroup, targetNode.Tag.ToString());

                        draggedplgi.ParentKey = targetplgi.Key;
                        // Sort folders if folder dragged
                        PlGroup.Sort();

                        selectedKey = draggedplgi.Key;

                    }

                    // Expand the node at the location 
                    // to show the dropped node.
                    targetNode.Expand();

                    bRefresh = true;
                    

                }
                else if (targetNode == null && draggedNode.Level != 0 && draggedNode.Tag.ToString() != "playlist")
                {
                    // Move a node to the root

                    // Remove the node from its current 
                    // location and add it to the node at the drop location.
                    draggedNode.Remove();

                    tvPlaylistGroup.Nodes.Add(draggedNode);

                    // Expand the node at the location 
                    // to show the dropped node.
                    draggedNode.Expand();

                    // TODO set draggedplgi
                    draggedplgi.ParentKey = null;

                    bRefresh = true;
                    selectedKey = draggedplgi.Key;
                }
                
                #endregion
            }
            else if (draggedLVI != null)
            {
                #region drag drop songs from the listview

                if (targetNode.Tag.ToString() != "playlist")
                    return;

                int nbTotal = draggedLVI.Count;
                int nbAdded = 0;

                Playlist pltarget = PlGroupHelper.getPlaylistByName(PlGroup, targetNode.Text, targetNode.Parent.Tag.ToString());
                foreach (ListViewItem lvi in draggedLVI)
                {
                                        
                    string fileName = lvi.Tag.ToString();                    
                    if (!pltarget.PlaylistItemExists(fileName)) 
                    {
                        string Artist = "<Artist>";
                        string File = fileName;
                        string Song = Path.GetFileNameWithoutExtension(fileName);
                        string Album = "<Album>";
                        string Length = "00:00";
                        string sNotation = "4";
                        string sDirSlideShow = "";
                        bool bMelodyMute = Karaclass.m_MuteMelody;

                        if (pltarget.Add(Artist, Song, File, Album, Length, Convert.ToInt32(sNotation), sDirSlideShow, bMelodyMute, "<Song reserved by>"))
                            nbAdded++;                        
                    }                    
                }
                
                MessageBox.Show(nbAdded + "/" + nbTotal + " songs added to <" + pltarget.Name + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (nbAdded > 0)
                {
                    bRefresh = true;
                    selectedKey = targetNode.Parent.Tag.ToString();
                    m_selectedPlaylist = pltarget.Name;
                }
                #endregion
            }                            

            if (bRefresh)
            {
                SaveAllPlaylist();
                LoadPlaylists();
                PopulatetvPlaylistGroup(PlGroup, selectedKey);
            }
        }

        private void TvPlaylistGroup_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void TvPlaylistGroup_DragOver(object sender, DragEventArgs e)
        {

        }
        #endregion

        #endregion
        

        #region playlist

        private void LaunchPlayList()
        {
            if (SelectedFile != null)
            {
                string songname = listView.SelectedItems[0].Text;

                PlaylistItem pli = GetPlaylistItem(songname);

                if (pli == null)
                    return;             

                // Check if this file exists
                if (File.Exists(pli.File))
                {

                    //Check if other files exist
                    if (CheckAllPath(currentPlaylist) == true)
                    {
                        // Raise event Play 
                        bool bplay = true;
                        FileInfo fi = new FileInfo(pli.File);
                        string file = fi.FullName;
                        string ext = Path.GetExtension(file);
                        switch (ext.ToLower())
                        {
                            case ".mid":
                            case ".kar":
                                {
                                    PlayMidi?.Invoke(this, fi, currentPlaylist, bplay);
                                    break;
                                }

                            case ".zip":
                            case ".cdg":
                                {
                                    PlayCDG?.Invoke(this, fi, bplay);
                                    break;
                                }

                            case ".mml":
                            case ".abc":
                                {
                                    PlayAbc?.Invoke(this, fi, currentPlaylist, bplay);
                                    break;
                                }
                            case ".txt":
                                {
                                    PlayTxt?.Invoke(this, fi, currentPlaylist, bplay);
                                    break;
                                }
                            case ".musicxml":
                            case ".xml":
                                {
                                    PlayXml?.Invoke(this, fi, currentPlaylist, bplay);
                                    break;
                                }
                            default:
                                try
                                {
                                    System.Diagnostics.Process.Start(@file);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                break;
                        }                                                                                              
                    }                        
                }
                else
                {
                    string tx = "<" + songname + "> was not found in\r\r";
                    tx += pli.File;

                    MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        /// <summary>
        /// Check if all path of playlist exist
        /// </summary>
        /// <param name="pl"></param>
        /// <returns></returns>
        private bool CheckAllPath(Playlist pl)
        {
            bool bNotFound = false;
            string tx = string.Empty;
            int N = 0;

            for (int i = 0; i < pl.Songs.Count; i++)
            {
                PlaylistItem pli = pl.Songs[i];
                if (System.IO.File.Exists(pli.File) == false)
                {
                    N++;
                    bNotFound = true;
                    if (N < 6)
                        tx += pli.Song + "\r";
                }
            }
            if (bNotFound == true)
            {
                tx += "\r" + N.ToString() + " songs not found, continue?";
                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == System.Windows.Forms.DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Check if playlists drive has changed
        /// Assuming that playlists.xml is on same drive than song files ...
        /// </summary>
        private void CheckDrive(string fileName)
        {
            bool bChanged = false;
            string tx = "<No path changed>";
            string errortx = string.Empty;
            string driveSong = string.Empty;
            string driveSlideShow = string.Empty;

            string olddrivePlaylists = Karaclass.m_drivePlaylists;
            string drivePlaylists = Karaclass.m_drivePlaylists;

            try
            {
                drivePlaylists = Directory.GetDirectoryRoot(@fileName);         // drive of playlists file
                DriveInfo drvinfo = new DriveInfo(@fileName);

                Karaclass.m_drivePlaylists = drivePlaylists;

                // Si on est sur un disque amovible                    
                if (drvinfo.DriveType != DriveType.Fixed)
                {

                    tx = "<Using removeable drive>";

                    #region a corriger

                    // Regarde le drive de chaque morceau
                    for (int i = 0; i < PlGroup.plGroupItems.Count; i++)
                    {
                        for (int k = 0; k < PlGroup.plGroupItems[i].Playlists.Count; k++)
                        {
                            Playlist pl = PlGroup.plGroupItems[i].Playlists[k];                                                
                            for (int j = 0; j < pl.Songs.Count; j++)
                            {
                                // Fichier de la playlist
                                string F = pl.Songs[j].File;
                                if (F.Contains("<") == false || F.Contains(">") == false)
                                {

                                    // Si le fichier de la playlist est introuvable
                                    if (File.Exists(@F) == false)
                                    {
                                        driveSong = Directory.GetDirectoryRoot(F);
                                        // Si le drive du morceau est différent de celui de la playlist
                                        if (driveSong != drivePlaylists)
                                        {
                                            bChanged = true;
                                            string MyPath = @F; // \\networkmachine\foo\bar OR C:\foo\bar
                                            string MyPathWithoutDriveOrNetworkShare = MyPath.Substring(Path.GetPathRoot(MyPath).Length);
                                            // Replace drive on song path
                                            F = drivePlaylists + MyPathWithoutDriveOrNetworkShare;

                                            pl.Songs[j].File = F;
                                        }
                                    }
                                }

                                // Drive du répertoire SlideShow
                                string S = pl.Songs[j].DirSlideShow;
                                if (S != "")
                                {
                                    if (S.Contains("<") == false || S.Contains(">") == false)
                                    {
                                        if (Directory.Exists(@S))
                                        {
                                            driveSlideShow = Directory.GetDirectoryRoot(S);
                                            // Si le drive du slideshow est différent de celui de la playlist
                                            if (driveSlideShow != drivePlaylists)
                                            {
                                                bChanged = true;
                                                string MyPath = @S; // \\networkmachine\foo\bar OR C:\foo\bar
                                                string MyPathWithoutDriveOrNetworkShare = MyPath.Substring(Path.GetPathRoot(MyPath).Length);
                                                // Replace drive on song path
                                                S = drivePlaylists + MyPathWithoutDriveOrNetworkShare;

                                                pl.Songs[j].DirSlideShow = S;

                                            }
                                        }
                                    }
                                }

                            }  // for j
                        }
                    }

                    
                    #endregion a corriger

                    // Save playlists with drive changed
                    if (bChanged == true)
                    {
                        tx += "<All path changed>";
                        SaveAllPlaylist();
                    }
                    else
                    {
                        tx += "<No path changed>";
                    }
                }   // drive not fixed
                else
                {
                    tx += "<Using fixed drive>";
                }


                // Save properties
                Karaclass.m_drivePlaylists = drivePlaylists;
                Properties.Settings.Default.drivePlaylists = drivePlaylists;
                Properties.Settings.Default.Save();

                // Update labels drive property
                lblUsualDrive.Text = "Usual drive: " + olddrivePlaylists;            // Usual drive
                lblDrive.Text = "Current drive: " + Karaclass.m_drivePlaylists;      // Current drive
                lblDriveInfos.Text = tx;
            }
            catch (Exception e)
            {
                tx = e.Message;
                tx += "\r" + "Proc: Check drive - playlists file = " + fileName;

                MessageBox.Show(tx, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Load existing playlists into PlGroup collection
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

                txtPlaylistFile.Text = fileName;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
      
        /// <summary>
        /// Display the list of songs in the listView
        /// </summary>
        /// <param name="pl"></param>
        private void ShowPlContent(Playlist pl, int itemIndex)
        {
            PopulateListView(pl);

            if (listView.Items.Count > 0 && itemIndex >= 0)
                listView.Items[itemIndex].Selected = true;            
        }


        /// <summary>
        /// Display song details
        /// </summary>
        /// <param name="pi"></param>
        private void ShowPlaylistItem(PlaylistItem pi)
        {
            string tx = string.Empty;
            EmptyTx();

            if (pi != null)
            {
                txtArtist.Text = pi.Artist;
                txtSong.Text = pi.Song;
                txtFile.Text = pi.File;
                txtAlbum.Text = pi.Album;
                txtLength.Text = pi.Length;
                txtNotation.Text = pi.Notation.ToString();
                txtDirSlideShow.Text = pi.DirSlideShow;
               
                chkMuteMelody.Checked = pi.MelodyMute;
                txtKaraokeSinger.Text = pi.KaraokeSinger;
            }

        }

        /// <summary>
        /// Save all playlists
        /// </summary>
        private void SaveAllPlaylist()
        {            
            string fName = Karaclass.M_filePlaylistGroups;
            PlGroupHelper.Save(fName, PlGroup);               
            DisplayPlaylistInfos();
            bModified = false;
        }

        /// <summary>
        /// Get PlaylistItem with its song name
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        private PlaylistItem GetPlaylistItem(string song)
        {
            PlaylistItem pitarget = currentPlaylist.Songs.Where(z => z.Song == song).FirstOrDefault();
            return pitarget ?? null;
        }


        /// <summary>
        /// Reorder items of a playlist
        /// </summary>
        private void PlReorder()
        {
            Playlist newpl = new Playlist() {
                Name = currentPlaylist.Name,
                Style = currentPlaylist.Style,
            };

            for (int i = 0; i < listView.Items.Count; i++)
            {
                string sng = listView.Items[i].Name;
                PlaylistItem pi = GetPlaylistItem(sng);
                newpl.Songs.Add(pi);
            }
            currentPlaylist = newpl;
            
            SaveAllPlaylist();
            ShowPlContent(currentPlaylist, 0);
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
                // List of selected files
                List<string> li = new List<string>();
                foreach (ListViewItem eachItem in listView.SelectedItems)
                {
                    li.Add(eachItem.Tag.ToString());
                }

                FrmNewPlaylist frmNewPlaylist = new FrmNewPlaylist(li);
                frmNewPlaylist.ShowDialog();

                LoadPlaylists();

                // display playlists names                            
                PopulatetvPlaylistGroup(PlGroup);
            }
        }

        #endregion playlist


        #region  Toolbar left menus Folders & Playlists

        #region Toolbarfolders

        /// <summary>
        /// Create a new folder for playlists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreatePlaylistGroup_Click(object sender, EventArgs e)
        {
            CreateNewPlaylistGroup();
        }

        /// <summary>
        /// Create a folder
        /// </summary>
        private void CreateNewPlaylistGroup()
        {
            TreeNode tn = tvPlaylistGroup.SelectedNode;
            if (tn == null && tvPlaylistGroup.Nodes.Count > 0)
                tn = tvPlaylistGroup.Nodes[0];

            string Key = string.Empty;

            if (tn != null && tn.Parent != null)
                Key = GetKey();

            string name = "myPlaylists1";


            // Create a new node, with a unique name
            if (tn != null)
            {
                int suffix = 0;
                TreeNode tChild;
                do
                {
                    name = string.Format("myPlaylists{0}", ++suffix);
                    if (tn.Parent == null)
                        tChild = tvPlaylistGroup.Nodes.Find(name, false).FirstOrDefault();
                    else
                        tChild = tn.Parent.Nodes.Find(name, false).FirstOrDefault();
                } while (tChild != null);
            }

            PlaylistGroupItem plgi;
            if (tn != null && tn.Parent != null)
            {
                PlaylistGroupItem plgiParent = PlGroupHelper.Find(PlGroup, Key);
                plgi = PlGroupHelper.CreateEmptyPlaylistGroupItem(name, plgiParent);
            }
            else
            {
                plgi = PlGroupHelper.CreateEmptyPlaylistGroupItem(name, null);
            }
            PlGroup.Add(plgi);

            // Sort PlGroup
            PlGroup.Sort();
            SaveAllPlaylist();
            LoadPlaylists();
            PopulatetvPlaylistGroup(PlGroup, plgi.Key);
        }

        /// <summary>
        /// Delete a folder of playlists
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeletePlaylistGroup_Click(object sender, EventArgs e)
        {
            DeletePlaylistGroup();
        }

        /// <summary>
        /// Delete recusrsively the folder and its childs
        /// </summary>
        private void DeletePlaylistGroup()
        {
            if (tvPlaylistGroup.SelectedNode != null)
            {
                // If item selected is a playlist
                if (tvPlaylistGroup.SelectedNode.Tag.ToString() == "playlist")
                {
                    DeletePlaylist();
                    return;
                }

                string Key = tvPlaylistGroup.SelectedNode.Tag.ToString();
                PlaylistGroupItem plgiRoot = PlGroupHelper.Find(PlGroup, Key);

                string tx = string.Format("Do you want to remove the folder {0} and all its content?", plgiRoot.Name);
                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    // Remove all childs
                    string lastkey = string.Empty;
                    do
                    {
                        lastkey = PlGroupHelper.SearchLastChildKey(PlGroup, Key);
                        if (lastkey != null)
                        {
                            PlaylistGroupItem plgi = PlGroupHelper.Find(PlGroup, lastkey);
                            PlGroup.Remove(plgi);
                        }
                    }
                    while (lastkey != null);

                    // Finaly Remove root
                    PlGroup.Remove(plgiRoot);

                    SaveAllPlaylist();
                    LoadPlaylists();
                    PopulatetvPlaylistGroup(PlGroup, plgiRoot.ParentKey);
                }
            }
        }

        #endregion

        #region Toolbar Playlists
        private void BtnCreatePlaylist_Click(object sender, EventArgs e)
        {
            CreateNewPlayListInit();
        }

        /// <summary>
        /// Create a new playlist
        /// </summary>
        private void CreateNewPlayListInit()
        {
            if (tvPlaylistGroup.Nodes.Count == 0)
                return;

            // What is the selected PlaylistGroupItem ?
            string Key = GetKey();
            selectedKey = Key;

            FrmNewPlaylist frmNewPlaylist = new FrmNewPlaylist(null, Key);
            if (frmNewPlaylist.ShowDialog() == DialogResult.Cancel)
                return;

            // new playlist is added by code in frmNewPlaylist
            // all is saved also and sorted
            m_selectedPlaylist = frmNewPlaylist.PlaylistName;

            LoadPlaylists();
            PopulatetvPlaylistGroup(PlGroup, selectedKey);
        }

        /// <summary>
        /// Return Key
        /// </summary>
        /// <returns></returns>
        private string GetKey()
        {
            // What is the selected PlaylistGroupItem ?
            TreeNode tn = tvPlaylistGroup.SelectedNode;
            if (tn == null && tvPlaylistGroup.Nodes.Count > 0)
                tn = tvPlaylistGroup.Nodes[0];
            else if (tvPlaylistGroup.Nodes.Count == 0)
                return null;
            
            return tn.Tag.ToString() == "playlist" ? tn.Parent.Tag.ToString() : tn.Tag.ToString();            
        }

        /// <summary>
        /// Button delete a playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeletePlaylist_Click(object sender, EventArgs e)
        {
            DeletePlaylist();
        }

        /// <summary>
        /// toolbar button or tvPlaylistGroup
        /// </summary>
        private void DeletePlaylist()
        {
            if (tvPlaylistGroup.SelectedNode != null)
            {
                string name = tvPlaylistGroup.SelectedNode.Text;
                string Key = string.Empty;

                // If item selected is a playlist
                if (tvPlaylistGroup.SelectedNode.Tag.ToString() == "playlist")
                {

                    Key = tvPlaylistGroup.SelectedNode.Parent.Tag.ToString();
                    Playlist pl = PlGroupHelper.getPlaylistByName(PlGroup, name, Key);

                    string tx = "Do you want to remove playlist <" + pl.Name + ">?";
                    if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        PlaylistGroupItem pli = PlGroupHelper.Find(PlGroup, Key);
                        pli.Playlists.Remove(pl);

                        SaveAllPlaylist();
                        LoadPlaylists();

                        m_selectedPlaylist = null;
                        PopulatetvPlaylistGroup(PlGroup, Key);
                    }
                }
            }
        }
        #endregion

        #endregion


        #region Toolbar Right items

        private void MnuDeletePlaylistItem_Click(object sender, EventArgs e)
        {
            DeletePlaylistItems();
        }

        private void MnuEditPlaylistItem_Click(object sender, EventArgs e)
        {
            InvokePlayEdit(false);
        }

        private void MnuPlayPlaylistItem_Click(object sender, EventArgs e)
        {            
            LaunchPlayList();
        }

        private void BtnRemoveFromPlaylist_Click(object sender, EventArgs e)
        {
            DeletePlaylistItems();
        }

        /// <summary>
        /// Delete selected items in listview
        /// </summary>
        private void DeletePlaylistItems()
        {
            if (SelectedFile != null)
            {
                int nbDel = listView.SelectedItems.Count;

                string songname = string.Empty;
                PlaylistItem pli;

                string tx = "Do you want to remove " + nbDel + " songs from the playlist <" + currentPlaylist.Name + "> ?";
                if (MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {

                    foreach (ListViewItem eachItem in listView.SelectedItems)
                    {
                        songname = eachItem.Text;
                        pli = GetPlaylistItem(songname);
                        currentPlaylist.Songs.Remove(pli);
                    }

                    EmptyTx();
                    SaveAllPlaylist();
                    ShowPlContent(currentPlaylist, 0);
                }
            }

        }
        
             
        /// <summary>
        /// Button: Update song currently edited
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpd_Click(object sender, EventArgs e)
        {
            UpdateSong();
        }

        /// <summary>
        /// Update current song
        /// </summary>
        private void UpdateSong()
        {
            if (tvPlaylistGroup.SelectedNode != null)
            {
                if (txtSong.Text != "<Song>")
                {
                    // Common
                    string sDirSlideShow = txtDirSlideShow.Text;
                    if (sDirSlideShow == "<DirSlideShow>")
                    {
                        sDirSlideShow = "";
                        txtDirSlideShow.Text = "";
                    }

                    string sNotation = txtNotation.Text;
                    if (sNotation == "<Notation>")
                        sNotation = "4";

                    int Notation = 4;
                    try
                    {
                        Notation = Convert.ToInt32(sNotation);
                    }
                    catch
                    {
                        Notation = 4;
                    }

                    string Song = txtSong.Text;

                    string Artist = txtArtist.Text;
                    if (Artist == "<Artist>")
                    {
                        int n = Song.IndexOf("-");
                        if (n > 0)
                        {
                            Artist = Song.Substring(0, n - 1);
                            txtArtist.Text = Artist;
                        }
                    }

                    int selectedindex = 0;
                    if (listView.Items.Count > 0)
                        selectedindex = listView.SelectedItems[0].Index;

                    // Update existing song
                    currentplaylistItem.Artist = Artist;
                    currentplaylistItem.Song = Song;
                    currentplaylistItem.File = txtFile.Text;
                    currentplaylistItem.Album = txtAlbum.Text;
                    currentplaylistItem.Length = txtLength.Text;

                    currentplaylistItem.Notation = Notation;
                    currentplaylistItem.DirSlideShow = sDirSlideShow;

                    currentplaylistItem.MelodyMute = chkMuteMelody.Checked;
                    currentplaylistItem.KaraokeSinger = txtKaraokeSinger.Text;


                    SaveAllPlaylist();
                    ShowPlContent(currentPlaylist, selectedindex);

                }
            }
        }
     
        private void BtnDirSlideShow_Click(object sender, EventArgs e)
        {
            string DirSlideShow = txtDirSlideShow.Text;
            if (Directory.Exists(DirSlideShow))
            {
                folderBrowserDialog1.SelectedPath = DirSlideShow;
            }

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                DirSlideShow = folderBrowserDialog1.SelectedPath;
                txtDirSlideShow.Text = DirSlideShow;
                currentplaylistItem.DirSlideShow = DirSlideShow;
                SaveAllPlaylist();
            }
        }

        /// <summary>
        /// Add a new song to the playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddSong_Click(object sender, EventArgs e)
        {
            openFileDlg.Title = "Open MIDI file";
            openFileDlg.DefaultExt = "kar";
            openFileDlg.Filter = "Kar files|*.kar|MIDI files|*.mid|All files|*.*";
            openFileDlg.FileName = "";

            if (_lastFolder == null || _lastFolder == "")
                openFileDlg.InitialDirectory = _songroot;
            else
                openFileDlg.InitialDirectory = _lastFolder;

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {

                string f = openFileDlg.FileName;
                _lastFolder = Path.GetDirectoryName(f);
                txtFile.Text = f;
                txtSong.Text = Path.GetFileName(f);

                AddNewSong();
            }
        }

           /// <summary>
        /// Add a new song to the playlist
        /// </summary>
        private void AddNewSong()
        {
            if (tvPlaylistGroup.SelectedNode != null)
            {
                if (txtSong.Text != "<Song>")
                {

                    // Common
                    string DirSlideShow = txtDirSlideShow.Text;
                    if (DirSlideShow == "<DirSlideShow>")
                    {
                        DirSlideShow = "";
                        txtDirSlideShow.Text = "";
                    }

                    string sNotation = txtNotation.Text;
                    if (sNotation == "<Notation>")
                        sNotation = "4";
                    int Notation = 4;
                    try
                    {
                        Notation = Convert.ToInt32(sNotation);
                    }
                    catch
                    {
                        Notation = 4;
                    }

                    string Song = txtSong.Text;

                    // Artist
                    string Artist = txtArtist.Text;
                    int n = Song.IndexOf("-");
                    if (n > 0)
                    {
                        Artist = Song.Substring(0, n - 1);
                        txtArtist.Text = Artist;
                    }
                    
                    int selectedindex = 0;

                    if (listView.Items.Count > 0)
                        selectedindex = listView.SelectedItems[0].Index;

                    string File = txtFile.Text;
                    string Album = txtAlbum.Text;
                    string Length = txtLength.Text.Trim();                    
                    bool MelodyMute = chkMuteMelody.Checked;
                    string KaraokeSinger = txtKaraokeSinger.Text;

                    currentPlaylist.Add(Artist, Song, File, Album, Length, Notation, DirSlideShow, MelodyMute, KaraokeSinger);

                    SaveAllPlaylist();
                    ShowPlContent(currentPlaylist, selectedindex);

                }
            }
        }

        /// <summary>
        /// Replace current song by another one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReplaceSong_Click(object sender, EventArgs e)
        {
            openFileDlg.Title = "Open MIDI file";
            openFileDlg.DefaultExt = "kar";
            openFileDlg.Filter = "Kar files|*.kar|MIDI files|*.mid|All files|*.*";
            openFileDlg.InitialDirectory = _songroot;

            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                string f = openFileDlg.FileName;
                txtFile.Text = f;
                txtSong.Text = Path.GetFileName(f);

                UpdateSong();               
            }
        }

        private void BtnPlay_Click(object sender, EventArgs e)
        {           
            LaunchPlayList();
        }

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

                    case ".mml":
                    case ".abc":
                        {
                            PlayAbc?.Invoke(this, new FileInfo(file), null, bplay);
                            break;
                        }
                    case ".musicxml":
                    case ".xml":
                        {
                            PlayXml?.Invoke(this, new FileInfo(file), null, bplay);
                            break;
                        }
                    case ".txt":
                        {
                            PlayTxt?.Invoke(this, new FileInfo(file), null, bplay);
                            break;
                        }
                    case ".zip":
                    case ".cdg":
                        {
                            PlayCDG?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }
                    default:
                        try
                        {
                            System.Diagnostics.Process.Start(@file);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                }
            }
        }


        #region Toolbar: Menus Add to playlists

        /// <summary>
        /// Toolbar: create the menu add to playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnutbAddtoPlayList_Click(object sender, EventArgs e)
        {
            CreateMenusAddToPlaylists(mnutbAddtoPlayList, PlGroup);
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

        #endregion



        #endregion


        #region textbox           

        private void TxtKaraokeSinger_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }
            
        private void TxtArtist_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }

        private void TxtAlbum_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }

        private void TxtLength_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }

        private void TxtNotation_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }

        private void TxtDirSlideShow_KeyPress(object sender, KeyPressEventArgs e)
        {
            oldindex = listView.SelectedItems[0].Index;
            bModified = true;
        }

        /// <summary>
        /// Style of Playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtStyle_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    currentPlaylist.Style = TxtStyle.Text.Trim();
                    break;
            }
        }

        /// <summary>
        /// Name of playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPlaylistName_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    RenamePlaylist(TxtPlaylistName.Text.Trim());                    
                    break;
            }
        }

        private void RenamePlaylist(string Name)
        {
            TreeNode tn = tvPlaylistGroup.SelectedNode;
            if (tn == null)
                return;

            // Playlist was renamed
            string Key = tn.Parent.Tag.ToString();
            PlaylistGroupItem plgi = PlGroupHelper.Find(PlGroup, Key);

            // Check duplicate name
            if (PlGroupHelper.PlaylistExist(plgi.Playlists, Name))
            {
                // Duplicate = cancel edit
                return;
            }
            else
            {
                // OK => rename node
                m_selectedPlaylist = Name;
                currentPlaylist.Name = Name;

                plgi.Sort();
                SaveAllPlaylist();
                LoadPlaylists();
                PopulatetvPlaylistGroup(PlGroup, plgi.Key);
            }
        }


        #endregion


        #region appearance

        /// <summary>
        /// Update Width of song path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PnlSongDetails_Resize(object sender, EventArgs e)
        {
            int w = pnlSongDetails.Width - txtFile.Left - 10;
            if (w > 576)
                txtFile.Width = w;
        }


        #endregion

       
    }
}
