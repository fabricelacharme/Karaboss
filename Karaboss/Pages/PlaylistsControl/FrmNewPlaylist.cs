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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Karaboss
{
    public partial class FrmNewPlaylist : Form
    {

        private string _playlistname;
        public string PlaylistName
        {
            get { return _playlistname; }
        }

        private PlaylistGroup PlGroup = new PlaylistGroup();
        private PlaylistGroupsHelper PlGroupHelper = new PlaylistGroupsHelper();

        private List<string> Songs;
        private string SelectedKey;

        private bool CancelClose = false;
        
        /// <summary>
        /// Constuctor 
        /// </summary>
        /// <param name="filenames = list of files to add to a new playlist"></param>
        public FrmNewPlaylist(List<string> filenames, string Key = null)
        {
            InitializeComponent();

            InitTreeview();

            LoadPlaylists();

            Songs = filenames;
            if (Songs != null && Songs.Count > 0)
            {                
                lblSong.Text = string.Format(Karaboss.Resources.Localization.Strings.AddSongToNewPlaylist, Songs.Count);
            }
            else
            {
                lblSong.Text = Karaboss.Resources.Localization.Strings.CreateNewPlaylist;  // "Create a new playlist";
            }

            SelectedKey = Key;
            PopulatetvPlaylistGroup(PlGroup, SelectedKey);
            txtPlName.Text = FindNewName();            
        }

        /// <summary>
        /// Gives "Windows Explorer" style to listview and treeview
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            Karaboss.playlists.ShellAPI.SetWindowTheme(tvPlaylistGroup.Handle, "explorer", null);
            base.OnLoad(e);
        }

        /// <summary>
        /// Load existing playlists 
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
        private void SaveAllPlaylists()
        {
            string fName = Karaclass.M_filePlaylistGroups;
            PlGroupHelper.Save(fName, PlGroup);           
        }


        /// <summary>
        /// Find a new name not used
        /// </summary>
        /// <returns></returns>
        private string FindNewName()
        {
            if (tvPlaylistGroup.Nodes.Count == 0)
                return "Playlist1" ;

            TreeNode tn = tvPlaylistGroup.SelectedNode;
            if (tn == null)
                tn = tvPlaylistGroup.Nodes[0];

            string Key = string.Empty;
            if (tn.Tag.ToString() == "playlist")
            {
                Key = tn.Parent.Tag.ToString();
            }
            else
            {
                Key = tn.Tag.ToString();
            }
            PlaylistGroupItem pli = PlGroupHelper.Find(PlGroup, Key);

            string name = string.Empty;
            int suffix = 0;            
            do
            {
                name = string.Format("Playlist{0}", ++suffix);
            } while (PlGroupHelper.PlaylistExist(pli.Playlists, name) == true);

            return name;
        }


        #region Treeview

        private void InitTreeview()
        {
            tvPlaylistGroup.HotTracking = true;
            tvPlaylistGroup.HideSelection = false;
            tvPlaylistGroup.Font = new Font("Segoe UI", 9F);
            tvPlaylistGroup.Indent = 19;
            tvPlaylistGroup.ItemHeight = 24;
            //tvPlaylistGroup

        }

        /// <summary>
        /// Populate Treeview (list of Playlists)
        /// </summary>
        /// <param name="plg"></param>
        /// <param name="selectedKey"></param>
        private void PopulatetvPlaylistGroup(PlaylistGroup plg, string selectedKey = null)
        {
            TreeNode tnPl;

            tvPlaylistGroup.Nodes.Clear();
            tvPlaylistGroup.BeginUpdate();

            for (int i = 0; i < plg.Count; i++)
            {
                PlaylistGroupItem item = plg.plGroupItems[i];

                // 1 - Create folder node named item.Name
                TreeNode tnFolder = new TreeNode(item.Name) {
                    Name = item.Name,
                    ImageIndex = 0,
                    Tag = item.Key,
                };

                // Position new node under item.Parent;
                TreeNode tnParent = null;
                if (item.ParentKey != null && tvPlaylistGroup.Nodes.Count > 0)
                {
                    // Search for a treenode aving a tag equal to ParentKey 
                    foreach (TreeNode node in tvPlaylistGroup.Nodes)
                    {
                        var result = FindString(item.ParentKey, node);
                        if (result != null)
                        {
                            tnParent = result;
                            break;
                        }
                    }
                }

                // 2 - Add folder node to root or to an existing folder node
                if (tnParent != null)
                    tnParent.Nodes.Add(tnFolder);
                else
                    tvPlaylistGroup.Nodes.Add(tnFolder);

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
            }

            tvPlaylistGroup.EndUpdate();
            tvPlaylistGroup.ExpandAll();

            // Set focus to selected node
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
                tvPlaylistGroup.SelectedNode = tns;
                if (tns.LastNode != null)
                    tns.LastNode.EnsureVisible();
                else
                    tns.EnsureVisible();
            }
            else if (tvPlaylistGroup.Nodes.Count > 0)
                tvPlaylistGroup.SelectedNode = tvPlaylistGroup.Nodes[0];
        }

        /// <summary>
        /// Find a string in nodes tag recursively
        /// </summary>
        /// <param name="strSearch"></param>
        /// <param name="rootNode"></param>
        /// <returns></returns>
        public TreeNode FindString(string strSearch, TreeNode rootNode)
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

        #endregion


        #region buttons
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// OK: create new playlist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, EventArgs e)
        {
            string name = txtPlName.Text;
            string NewStyle = txtStyle.Text;

            name = name.Trim();
            NewStyle = NewStyle.Trim();

            TreeNode tn = tvPlaylistGroup.SelectedNode;
            if (tn == null)
                return;


            string Key = tn.Tag.ToString() == "playlist" ? tn.Parent.Tag.ToString() : tn.Tag.ToString();
            /*
            string Key = string.Empty;
            if (tn.Tag.ToString() == "playlist")
            {
                Key = tn.Parent.Tag.ToString();
            }
            else
            {
                Key = tn.Tag.ToString();
            }
            */

            // check if the new playlist already exists in the selected folder (PlaylistGroupItem)
            PlaylistGroupItem pli = PlGroupHelper.Find(PlGroup, Key);

            if (PlGroupHelper.PlaylistExist(pli.Playlists, name))
            {
                string tx = string.Format("The playlist <{0}> already exists in the folder <{1}>", name, pli.Name);
                MessageBox.Show(tx, "Karaboss", MessageBoxButtons.OK);
                CancelClose = true;
                return;
            }

            int nbAdded = 0;
            int nbTotal = 0;

            if (Songs != null)
                nbTotal = Songs.Count;


            // Create playlist and add it to the PlaylistGroupItem
            Playlist pl = new Playlist() {
                Name = name,
                Style = NewStyle,
            };
            
            string iSong = string.Empty;
            // If songs have to be added to the new playlist
            if (nbTotal > 0 && Songs != null)
            {
                for (int i = 0; i < Songs.Count; i++)
                {
                    iSong = Path.GetFileNameWithoutExtension(Songs[i]);
                    if (pl.Add("<Artist>", iSong, Songs[i], "<Album>", "00:00", 4, "<DirSlideShow>", false, "<Song reserved by>"))
                        nbAdded++;
                }
            }
            else
            {
                // Add an empty song                                                
                pl.Add("<Artist>", "<Song>", "", "<Album>", "00:00", 4, "<DirSlideShow>", false, "<Karaoke singer>");                
            }

            
            pli.Playlists.Add(pl);

            // Sort playlists
            pli.Sort();

            // Save all
            SaveAllPlaylists();

            if (Songs == null)
                MessageBox.Show("Playlist <" + pl.Name + "> created", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(nbAdded + "/" + nbTotal + " songs added to <" + pl.Name + ">", "Karaboss", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _playlistname = pl.Name;

            this.Close();
            
        }

        #endregion


        #region form load close

        private void FrmNewPlaylist_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CancelClose)
            {
                e.Cancel = true;
                CancelClose = false;
            }
            else
                Dispose();
        }
        
        #endregion


    }
}
