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
//
//https://www.ipentec.com/document/document.aspx?page=csharp-shell-namespace-create-explorer-list-view-control
//
// dragging with insert
// http://www.cyotek.com/blog/dragging-items-in-a-listview-control-with-visual-insertion-guides
//
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using FlShell.Interop;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using System.Text;
using FlShell.Resources.Localization;
using System.Threading;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms.VisualStyles;

namespace FlShell
{
    // Specific Karaboss    
    // Selected item changed
    public delegate void SelectedIndexChangedEventHandler(object sender, string fileName);
    // Play MIDI or KAR file
    public delegate void PlayMidiEventHandler(object sender, FileInfo fi, bool bplay);
    // Play CDG file
    public delegate void PlayCDGEventHandler(object sender, FileInfo fi, bool bplay);
    // Play abc, mml file
    public delegate void PlayAbcEventHandler(object sender, FileInfo fi, bool bplay);
    // Play musicxml, xml file
    public delegate void PlayXmlEventHandler(object sender, FileInfo fi, bool bplay);
    // Play txt file
    public delegate void PlayTxtEventHandler(object sender, FileInfo fi, bool bplay);
    // Playlists management
    public delegate void AddToPlaylistByNameHandler(object sender, ShellItem[] fls, string plname, string key = null, bool newPlaylist = false);
    // Display number of directories and files
    public delegate void ContentChangedEvenHandler(object sender, string strContent, string strPath);
    // SendK Key to Parent
    public delegate void SenKeyToParentHandler(object sender, Keys k);



    public partial class ShellListView : Control, IDropSource, Interop.IDropTarget
    {
        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

        private WindowsContextMenu m_WindowsContextMenu = new WindowsContextMenu();

        /// <summary>
        /// Represents the method that will handle FilterItem events.
        /// </summary>
        public delegate void FilterItemEventHandler(object sender, FilterItemEventArgs e);

        // Specific Karaboss : Play a song, a playlist or edit a song
        public event SelectedIndexChangedEventHandler SelectedIndexChanged;
        public event PlayMidiEventHandler PlayMidi;
        public event PlayCDGEventHandler PlayCDG;
        public event PlayAbcEventHandler PlayAbc;
        public event PlayTxtEventHandler PlayTxt;
        public event PlayXmlEventHandler PlayXml;
        public event AddToPlaylistByNameHandler AddToPlaylist;
        
        // Send Key to parent
        public event SenKeyToParentHandler SenKeyToParent; 

        // Display number of directories and files
        public event ContentChangedEvenHandler lvContentChanged;


        /// <summary>
        /// Sepecific Karaboss: function keys F3, F4 keydown sent to application
        /// </summary>
        public event lvFunctionKeyEventHandler lvFunctionKeyClicked;
       
        /// <summary>
        /// Occurs when the <see cref="ShellView"/> control is about to 
        /// navigate to a new folder.
        /// </summary>
        public event NavigatingEventHandler Navigating;

        /// <summary>
        /// Occurs when the <see cref="ShellView"/> control navigates to a 
        /// new folder.
        /// </summary>
        public event EventHandler Navigated;


        //avoid Globalization problem-- an empty timevalue
        DateTime EmptyTimeValue = new DateTime(1, 1, 1, 0, 0, 0);

        // Specific Karaboss      
        string[,] m_allPlaylists;
        public string[,] allPlaylists
        {
            set { m_allPlaylists = value; }
        }

        private ListViewColumnSorter lvwColumnSorter;
        

        public ShellListView()
        {
            bDoSelect = true;

            //m_allPlaylists = new List<string>();
            
            m_ListView = new ListView();

            #region listview header
            ColumnHeader chName = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chSize = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chLastModified = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chTypeStr = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chAttributes = new System.Windows.Forms.ColumnHeader();
            ColumnHeader chCreated = new System.Windows.Forms.ColumnHeader();

            m_ListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            chName,
            chSize,
            chLastModified,
            chTypeStr,
            chAttributes,
            chCreated});
            // 
            // chName
            // 
            chName.Tag = "ShellItem.Displayname";
            chName.TextAlign = HorizontalAlignment.Left;
            chName.Text = Strings.Name;            
            chName.Width = 150;
            // 
            // chSize
            // 
            chSize.Text = Strings.Size;
            chSize.TextAlign = HorizontalAlignment.Right;
            chSize.Width = 88;
            // 
            // chLastModified
            // 
            chLastModified.Text = Strings.Modified;
            chLastModified.TextAlign = HorizontalAlignment.Left;
            chLastModified.Width = 122;
            // 
            // chTypeStr
            // 
            chTypeStr.Text = "Type";
            chTypeStr.TextAlign = HorizontalAlignment.Left;
            chTypeStr.Width = 110;
            // 
            // chAttributes
            // 
            chAttributes.Text = "Attributes";
            chAttributes.TextAlign = HorizontalAlignment.Center;
            chAttributes.Width = 80;
            // 
            // chCreated
            // 
            chCreated.Text = Strings.Created;
            chCreated.TextAlign = HorizontalAlignment.Left;
            chCreated.Width = 122;
            // 
            #endregion

            #region ListView properties

            m_ListView.AllowDrop = true;
            m_ListView.Dock = DockStyle.Fill;
            m_ListView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            m_ListView.HideSelection = false;
            m_ListView.HotTracking = false;

            m_ListView.FullRowSelect = m_fullrowselect;

            m_ListView.LabelEdit = true;            

            m_ListView.Parent = this;

            #endregion


            #region listview events
            m_ListView.HandleCreated += new EventHandler(ListView_HandleCreated);

            m_ListView.BeforeLabelEdit += new LabelEditEventHandler(ListView_BeforeLabelEdit);
            m_ListView.AfterLabelEdit += new LabelEditEventHandler(ListView_AfterLabelEdit);

            m_ListView.ItemDrag += new ItemDragEventHandler(ListView_ItemDrag);
            
           
            m_ListView.MouseDown += new MouseEventHandler(ListView_MouseDown);
            m_ListView.MouseUp += new MouseEventHandler(ListView_MouseUp);
            m_ListView.KeyDown += new KeyEventHandler(ListView_KeyDown);

            m_ListView.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(ListView_ItemSelectionChanged);
            m_ListView.ItemMouseHover += new ListViewItemMouseHoverEventHandler(ListView_ItemMouseOver);

            m_ListView.ColumnClick += new ColumnClickEventHandler(ListView_ColumnClick);
            m_ListView.DoubleClick += new EventHandler(ListView_DoubleClick);

            m_ListView.SelectedIndexChanged += new EventHandler(ListView_SelectedItemChanged);

            #endregion
            

            // Create sort method and associate to listview            
            lvwColumnSorter = new ListViewColumnSorter();
            m_ListView.ListViewItemSorter = lvwColumnSorter;

            // Display: default = details
            m_View = ShellViewStyle.Details;            
            //m_View = ShellViewStyle.LargeIcon;
            //m_View = ShellViewStyle.SmallIcon;


            // History of navigation
            m_History = new ShellHistory();
            m_MultiSelect = true;

            Size = new Size(250, 200);
            SystemImageList.UseSystemImageList(m_ListView);

            #region shellListener
            m_ShellListener.DriveAdded += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.DriveRemoved += new ShellItemEventHandler(m_ShellListener_ItemUpdated);

            //m_ShellListener.FolderCreated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.FolderCreated += new ShellItemEventHandler(m_ShellListener_ItemCreated);

            m_ShellListener.FolderDeleted += new ShellItemEventHandler(m_ShellListener_ItemUpdated);

            m_ShellListener.FolderRenamed += new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);            

            m_ShellListener.FolderUpdated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);

            //m_ShellListener.ItemCreated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.ItemCreated += new ShellItemEventHandler(m_ShellListener_ItemCreated);

            m_ShellListener.ItemDeleted += new ShellItemEventHandler(m_ShellListener_ItemUpdated);

            m_ShellListener.ItemRenamed += new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);

            m_ShellListener.ItemUpdated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.SharingChanged += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            #endregion


            // Allow drag & drop
            m_ListView.AllowDrop = true;
            m_ListView.AllowDrop = false;
            this.AllowDrop = true;

            selectedOrder = new ArrayList();

            Navigate(ShellItem.Desktop);
        }
       

        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_ShellListener.DriveAdded -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.DriveRemoved -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.FolderCreated -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.FolderDeleted -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);

                m_ShellListener.FolderRenamed -= new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);

                m_ShellListener.FolderUpdated -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.ItemCreated -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.ItemDeleted -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);

                m_ShellListener.ItemRenamed -= new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);

                m_ShellListener.ItemUpdated -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);
                m_ShellListener.SharingChanged -= new ShellItemEventHandler(m_ShellListener_ItemUpdated);

                //Marshal.ThrowExceptionForHR(Ole32.RevokeDragDrop(m_ListView.Handle));
            }

            base.Dispose(disposing);
        }
        #endregion
     

        #region Public

        /// <summary>
        /// Select first file in the listView
        /// </summary>
        public void SelectFirstItem()
        {
            //if (m_ListView.Items.Count > 0 && SelectedItems.Length == 0) 
            if (m_ListView.Items.Count > 0 && m_ListView.SelectedItems.Count == 0)
            {               
                // Item is nearly selected (grayed)
                m_ListView.Items[0].Selected = true;

                if (_bDoSelect)
                {
                    // Item is really selected (blue)              
                    m_ListView.Focus();
                }
            }            
        }


        public void ClearSelections()
        {
            selectedOrder.Clear();
            selectedOrder.Capacity = 0;
        }
        
        /// <summary>
        /// Creates a new folder in the folder currently being browsed.
        /// </summary>
        public void CreateNewFolder()
        {
            string name = "New Folder";
            int suffix = 0;

            do
            {
                name = string.Format("{0}\\New Folder ({1})",
                    CurrentFolder.FileSystemPath, ++suffix);
            } while (Directory.Exists(name) || File.Exists(name));

            try
            {
                Directory.CreateDirectory(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Title", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation);

            }            
        }

        /// <summary>
        /// Refreshes the contents of the <see cref="ShellView"/>.
        /// </summary>
        public void RefreshContents(string FullPath = "")
        {                        
            RecreateShellView(CurrentFolder, FullPath);
        }

        #endregion


        #region on

        /// <summary>
        /// Creates the actual shell view control.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (m_CurrentFolder != null)
            {
                CreateShellView(m_CurrentFolder);
                OnNavigated();
            }
        }

     

        #endregion


        #region Navigate

        void Navigate(ShellItem folder, string item = "")
        {        
            NavigatingEventArgs e = new NavigatingEventArgs(folder);
            Navigating?.Invoke(this, e);

            if (!e.Cancel)
            {
                ShellItem previous = m_CurrentFolder;
                m_CurrentFolder = folder;
               
                try
                {
                    RecreateShellView(folder, item);

                    m_History.Add(folder);
                    OnNavigated();
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    Console.Write("Erreur dans module ShellListView.cs / Navigate" + ex.Message);
                    m_CurrentFolder = previous;
                    RecreateShellView(folder);
                    
                }
                finally
                {                    
                    Cursor.Current = Cursors.Default;
                }
            }
        }


        void OnNavigated()
        {
            Navigated?.Invoke(this, EventArgs.Empty);
        }



        /// <summary>
        /// Navigates to the specified filesystem directory.
        /// </summary>
        /// 
        /// <param name="path">
        /// The path of the directory to navigate to.
        /// </param>
        /// 
        /// <exception cref="DirectoryNotFoundException">
        /// <paramref name="path"/> is not a valid folder.
        /// </exception>
        public void Navigate(string path, string file = "")
        {
            Navigate(new ShellItem(path), file);
        }

        /// <summary>
        /// Navigates to the specified standard location.
        /// </summary>
        /// 
        /// <param name="location">
        /// The <see cref="Environment.SpecialFolder"/> to which to navigate.
        /// </param>
        /// 
        /// <remarks>
        /// Standard locations are virtual folders which may be located in 
        /// different places in different versions of Windows. For example 
        /// the "My Documents" folder is normally located at C:\My Documents 
        /// on Windows 98, but is located in the user's "Documents and 
        /// Settings" folder in Windows XP. Using a standard 
        /// <see cref="Environment.SpecialFolder"/> to refer to such folders 
        /// ensures that your application will behave correctly on all 
        /// versions of Windows.
        /// </remarks>
        public void Navigate(Environment.SpecialFolder location)
        {

            // CSIDL_MYDOCUMENTS was introduced in Windows XP but doesn't work 
            // even on that platform. Use CSIDL_PERSONAL instead.
            if (location == Environment.SpecialFolder.MyDocuments)
            {
                location = Environment.SpecialFolder.Personal;
            }

            Navigate(new ShellItem(location));
        }

        /// <summary>
        /// Navigates the <see cref="ShellView"/> control to the previous folder 
        /// in the navigation history. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The WebBrowser control maintains a history list of all the folders
        /// visited during a session. You can use the <see cref="NavigateBack"/>
        /// method to implement a <b>Back</b> button similar to the one in 
        /// Windows Explorer, which will allow your users to return to a 
        /// previous folder in the navigation history. 
        /// </para>
        /// 
        /// <para>
        /// Use the <see cref="CanNavigateBack"/> property to determine whether 
        /// the navigation history is available and contains a previous page. 
        /// This property is useful, for example, to change the enabled state 
        /// of a Back button when the ShellView control navigates to or leaves 
        /// the beginning of the navigation history.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is no history to navigate backwards through.
        /// </exception>
        public void NavigateBack()
        {
            m_CurrentFolder = m_History.MoveBack();
            RecreateShellView(m_CurrentFolder);
            OnNavigated();
        }

        /// <summary>
        /// Navigates the <see cref="ShellView"/> control backwards to the 
        /// requested folder in the navigation history. 
        /// </summary>
        /// 
        /// <remarks>
        /// The WebBrowser control maintains a history list of all the folders
        /// visited during a session. You can use the <see cref="NavigateBack"/>
        /// method to implement a drop-down menu on a <b>Back</b> button similar 
        /// to the one in Windows Explorer, which will allow your users to return 
        /// to a previous folder in the navigation history. 
        /// </remarks>
        /// 
        /// <param name="folder">
        /// The folder to navigate to.
        /// </param>
        /// 
        /// <exception cref="Exception">
        /// The requested folder is not present in the 
        /// <see cref="ShellView"/>'s 'back' history.
        /// </exception>
        public void NavigateBack(ShellItem folder)
        {
            m_History.MoveBack(folder);
            m_CurrentFolder = folder;
            RecreateShellView(m_CurrentFolder);
            OnNavigated();
        }

        /// <summary>
        /// Navigates the <see cref="ShellView"/> control to the next folder 
        /// in the navigation history. 
        /// </summary>
        /// 
        /// <remarks>
        /// <para>
        /// The WebBrowser control maintains a history list of all the folders
        /// visited during a session. You can use the <see cref="NavigateForward"/> 
        /// method to implement a <b>Forward</b> button similar to the one 
        /// in Windows Explorer, allowing your users to return to the next 
        /// folder in the navigation history after navigating backward.
        /// </para>
        /// 
        /// <para>
        /// Use the <see cref="CanNavigateForward"/> property to determine 
        /// whether the navigation history is available and contains a folder 
        /// located after the current one.  This property is useful, for 
        /// example, to change the enabled state of a <b>Forward</b> button 
        /// when the ShellView control navigates to or leaves the end of the 
        /// navigation history.
        /// </para>
        /// </remarks>
        /// 
        /// <exception cref="InvalidOperationException">
        /// There is no history to navigate forwards through.
        /// </exception>
        public void NavigateForward()
        {
            m_CurrentFolder = m_History.MoveForward();
            RecreateShellView(m_CurrentFolder);
            OnNavigated();
        }

        /// <summary>
        /// Navigates the <see cref="ShellView"/> control forwards to the 
        /// requested folder in the navigation history. 
        /// </summary>
        /// 
        /// <remarks>
        /// The WebBrowser control maintains a history list of all the folders
        /// visited during a session. You can use the 
        /// <see cref="NavigateForward"/> method to implement a drop-down menu 
        /// on a <b>Forward</b> button similar to the one in Windows Explorer, 
        /// which will allow your users to return to a folder in the 'forward'
        /// navigation history. 
        /// </remarks>
        /// 
        /// <param name="folder">
        /// The folder to navigate to.
        /// </param>
        /// 
        /// <exception cref="Exception">
        /// The requested folder is not present in the 
        /// <see cref="ShellView"/>'s 'forward' history.
        /// </exception>
        public void NavigateForward(ShellItem folder)
        {
            m_History.MoveForward(folder);
            m_CurrentFolder = folder;
            RecreateShellView(m_CurrentFolder);
            OnNavigated();
        }

        /// <summary>
        /// Navigates to the parent of the currently displayed folder.
        /// </summary>
        public void NavigateParent()
        {
            Navigate(m_CurrentFolder.Parent);
        }

        /// <summary>
        /// Navigates to the folder currently selected in the 
        /// <see cref="ShellView"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// If the <see cref="ShellView"/>'s <see cref="MultiSelect"/>
        /// property is set, and more than one item is selected in the
        /// ShellView, the first Folder found will be navigated to.
        /// </remarks>
        /// 
        /// <returns>
        /// <see langword="true"/> if a selected folder could be
        /// navigated to, <see langword="false"/> otherwise.
        /// </returns>
        public bool NavigateSelectedFolder()
        {
            //if (SelectedItems.Length > 0)
            if (m_ListView.SelectedItems.Count > 0)
            {
                foreach (ShellItem i in SelectedItems)
                {
                    if (i.IsFolder)
                    {
                        Navigate(i);
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion


        #region create

        void RecreateShellView(ShellItem folder, string FullPath = "")
        {
            Cursor.Current = Cursors.WaitCursor;

            // Selected item
            ListView.SelectedListViewItemCollection lsvi = new ListView.SelectedListViewItemCollection(m_ListView);
            List<int> ls = new List<int>();

            string tx = string.Empty;
            if (FullPath != "")
            {
                tx = FullPath;
            }
            else if (m_ListView.SelectedItems.Count > 0)
            {
                tx = m_ListView.SelectedItems[0].Text;
                
                lsvi = m_ListView.SelectedItems;
                for (int i = 0; i < m_ListView.Items.Count; i++)
                {
                    if (m_ListView.Items[i].Selected)
                    {
                        ls.Add(i);
                    }                    
                }
            }
                            

            m_ListView.BeginUpdate();
            
            // Performances: remove sorter before adding items !!!!
            // The listview call sorter each time an item is added
            m_ListView.ListViewItemSorter = null;

            CreateShellView(folder);

            // Performances: restore sorter
            m_ListView.ListViewItemSorter = lvwColumnSorter;
            
            // restore selected item
            if (ls.Count > 0)
            {
                for (int i = 0; i < ls.Count; i++)
                {
                    m_ListView.Items[ls[i]].Selected = true;                    
                }
            }

            m_ListView.EndUpdate();

            Cursor.Current = Cursors.Default;

            OnNavigated();

            m_PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentFolder"));
        }

        /// <summary>
        /// Populate listview
        /// </summary>
        /// <param name="folder"></param>
        void CreateShellView(ShellItem folder)
        {
            
            m_ListView.Items.Clear();
            m_ListView.Refresh();
            

            #region select display style
            switch (m_View)
            {
                case ShellViewStyle.Details:
                    m_ListView.View = System.Windows.Forms.View.Details;
                    break;
                case ShellViewStyle.LargeIcon:
                    m_ListView.View = System.Windows.Forms.View.LargeIcon;
                    break;
                case ShellViewStyle.List:
                    m_ListView.View = System.Windows.Forms.View.List;
                    break;
                case ShellViewStyle.SmallIcon:
                    m_ListView.View = System.Windows.Forms.View.SmallIcon;
                    break;
                case ShellViewStyle.Tile:
                    m_ListView.View = System.Windows.Forms.View.Tile;
                    break;
                default:
                    m_ListView.View = System.Windows.Forms.View.LargeIcon;
                    break;
            }
            #endregion

            if (folder != null)
            {
                var items = new List<ListViewItem>();

                IEnumerator<ShellItem> e = folder.GetEnumerator(SHCONTF.FOLDERS);
                int d = 0;
                int f = 0;

                // Folders enumerationg                
                while (e.MoveNext())
                {
                  items.Add ( CreateItem(e.Current));
                    d++;
                }

                // Files enumerating
                e = folder.GetEnumerator(SHCONTF.INCLUDEHIDDEN | SHCONTF.NONFOLDERS);

                

                while (e.MoveNext())
                {
                    if (!e.Current.IsHidden)
                    {
                        items.Add(CreateItem(e.Current));
                        f++;
                    }
                }

                //Console.Write("");

                m_ListView.Items.AddRange(items.ToArray());

                string ct = d + " Directories " + f + " Files";
                string pt = folder.FileSystemPath;

                lvContentChanged?.Invoke(this, ct, pt);

            }
        }

        /// <summary>
        /// Create one item of listview
        /// </summary>
        /// <param name="item"></param>
        private ListViewItem CreateItem(ShellItem item)
        {
            ListViewItem lvi = new ListViewItem(item.DisplayName);
            lvi.Text = item.DisplayName;
            lvi.ImageIndex = SystemImageListManager.GetIconIndex(item, false);
            lvi.Tag = item;


            // false = Authorize change color for subitems
            lvi.UseItemStyleForSubItems = false;

            
            #region Length
            // 1 - Length
            if (!item.IsDisk && item.IsFileSystem && !item.IsFolder)
            {                
                lvi.SubItems.Add(FileSizeToString(item.Length));
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = item.Length;
            }
            else
            {
                lvi.SubItems.Add("");
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = 0L;
            }
            lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = Color.Gray;
            
            #endregion

            #region modified
            //2 - Set LastWriteTime
            if (item.IsDisk || item.LastWriteTime == EmptyTimeValue)
            { 
                lvi.SubItems.Add("");
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = EmptyTimeValue;
            }
            else
            {
                lvi.SubItems.Add(item.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = item.LastWriteTime;
            }
            lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = Color.Gray;
            #endregion

            #region type
            // 3 - Set Type
            lvi.SubItems.Add(item.TypeName);
            lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = Color.Gray;
            #endregion

            #region attributes
            //4 - Set Attributes
            if (!item.IsDisk && item.IsFileSystem)
            {
                StringBuilder SB = new StringBuilder();
                try
                {
                    FileAttributes attr = item.Attributes;
                    if ((attr & FileAttributes.System) == FileAttributes.System)
                        SB.Append("S");
                    if ((attr & FileAttributes.Hidden) == FileAttributes.Hidden)
                        SB.Append("H");
                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        SB.Append("R");
                    if ((attr & FileAttributes.Archive) == FileAttributes.Archive)
                        SB.Append("A");
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
                lvi.SubItems.Add(SB.ToString());
            }
            else
            {
                lvi.SubItems.Add("");
            }
            lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = Color.Gray;
            #endregion

            #region created
            //5 - Set CreationTime
            if (item.IsDisk || item.CreationTime == EmptyTimeValue)  
            {
                lvi.SubItems.Add("");
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = EmptyTimeValue;
            }
            else
            {
                lvi.SubItems.Add(item.CreationTime.ToString("dd/MM/yyyy HH:mm"));
                lvi.SubItems[lvi.SubItems.Count - 1].Tag = item.CreationTime;
            }
            lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = Color.Gray;
            #endregion
            
            return lvi;

        }

        /// <summary>
        /// Convert file size to Byte, Kb, Mb, Gb, Tb
        /// </summary>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        private string FileSizeToString(long sizeInBytes)
        {
            string[] sizes = { "Bytes", "Ko", "Mo", "Go", "To", "Po" };
            double len = sizeInBytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            // Take upper size
            len = Math.Ceiling(len);
            string result = String.Format("{0:0} {1}", len, sizes[order]);
            return result;
            
        }

        #endregion

        bool ShouldShowHidden()
        {
            if (m_ShowHidden == ShowHidden.System)
            {
                RegistryKey reg = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");

                if (reg != null)
                {
                    return ((int)reg.GetValue("Hidden", 2)) == 1;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return m_ShowHidden == ShowHidden.True;
            }
        }

        IEnumerator<ShellItem> GetFolderEnumerator(ShellItem folder)
        {
            SHCONTF filter = SHCONTF.FOLDERS;
            if (ShouldShowHidden()) filter |= SHCONTF.INCLUDEHIDDEN;
            return folder.GetEnumerator(filter);
        }



        #region properties

        private bool _bDoSelect = true;
        public bool bDoSelect
        {
            set { _bDoSelect = value; }
        }


        public ShellItem SelectedItem
        {
            get {
                //if (SelectedItems.Length > 0)
                if (m_ListView.SelectedItems.Count > 0)
                    return SelectedItems[0];
                else if (m_CurrentFolder != null)
                    return m_CurrentFolder;
                else
                    return null;
            }
        }

        private int lvfilenamecolumn;
        public int lvFileNameColumn
        {

            get { return lvfilenamecolumn; }
            set
            {
                lvfilenamecolumn = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a new folder can be created in
        /// the folder currently being browsed by th <see cref="ShellView"/>.
        /// </summary>
        [Browsable(false)]
        public bool CanCreateFolder
        {
            get
            {
                return m_CurrentFolder.IsFileSystem && !m_CurrentFolder.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a previous page in navigation 
        /// history is available, which allows the <see cref="NavigateBack"/> 
        /// method to succeed. 
        /// </summary>
        [Browsable(false)]
        public bool CanNavigateBack
        {
            get { return m_History.CanNavigateBack; }
        }

        /// <summary>
        /// Gets a value indicating whether a subsequent page in navigation 
        /// history is available, which allows the <see cref="NavigateForward"/> 
        /// method to succeed. 
        /// </summary>
        [Browsable(false)]
        public bool CanNavigateForward
        {
            get { return m_History.CanNavigateForward; }
        }

        /// <summary>
        /// Gets a value indicating whether the folder currently being browsed
        /// by the <see cref="ShellView"/> has parent folder which can be
        /// navigated to by calling <see cref="NavigateParent"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanNavigateParent
        {
            get
            {
                // Patch FAB
                //return m_CurrentFolder != ShellItem.Desktop;
                return m_CurrentFolder.Parent != null;
            }
        }

        /// <summary>
        /// Gets/sets a <see cref="ShellItem"/> describing the folder 
        /// currently being browsed by the <see cref="ShellView"/>.
        /// </summary>
        [Editor(typeof(ShellItemEditor), typeof(UITypeEditor))]
        public ShellItem CurrentFolder
        {
            get { return m_CurrentFolder; }
            set
            {
                if (value != m_CurrentFolder)
                {
                    Navigate(value);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ShellView"/>'s navigation history.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ShellHistory History
        {
            get { return m_History; }
        }

        /// <summary>
        /// Gets a list of the items currently selected in the 
        /// <see cref="ShellView"/>
        /// </summary>
        /// 
        /// <value>
        /// A <see cref="ShellItem"/> array detailing the items currently 
        /// selected in the control. If no items are currently selected, 
        /// an empty array is returned. 
        /// </value>
        [Browsable(false)]
        public ShellItem[] SelectedItems
        {
            get
            {
                int itemCount = m_ListView.SelectedItems.Count;
                ShellItem[] result = new ShellItem[itemCount];
                for (int i = 0; i < itemCount; i++)
                {
                    result[i] = (ShellItem)m_ListView.SelectedItems[i].Tag;
                }

                return result;                
            }
        }




        [Browsable(false)]
        public ArrayList SelectedOrder
        {
            get { return selectedOrder; }
        }


        /// <summary>
        /// Gets/sets a value indicating whether multiple items can be selected
        /// by the user.
        /// </summary>
        [DefaultValue(true), Category("Behaviour")]
        public bool MultiSelect
        {
            get { return m_MultiSelect; }
            set
            {
                m_MultiSelect = value;
                RecreateShellView(m_CurrentFolder);
                OnNavigated();
            }
        }

        /// <summary>
        /// Gets or sets how items are displayed in the control. 
        /// </summary>
        [DefaultValue(ShellViewStyle.LargeIcon), Category("Appearance")]
        public ShellViewStyle View
        {
            get { return m_View; }
            set
            {
                m_View = value;

                if (value == ShellViewStyle.Details)
                {
                    foreach (ColumnHeader col in m_ListView.Columns)
                        if (col.Width == 0)
                            col.Width = 120;
                }

                RecreateShellView(m_CurrentFolder);
                OnNavigated();
            }
        }

        /// <summary>
        /// Gets/sets a value indicating whether drag/drop operations are
        /// allowed on the control.
        /// </summary>
        [DefaultValue(false)]
        public override bool AllowDrop
        {
            get { return m_AllowDrop; }
            set
            {
                if (value != m_AllowDrop)
                {
                    m_AllowDrop = value;

                    if (m_AllowDrop)
                    {
                        try
                        {                            
                            Marshal.ThrowExceptionForHR(Ole32.RegisterDragDrop(m_ListView.Handle, this));
                        }
                        catch (Exception ex)
                        {
                            Console.Write("\nERROR: ShellListView AllowDrop: " + ex.Message);
                        }
                    }
                    else
                    {
                        Marshal.ThrowExceptionForHR(Ole32.RevokeDragDrop(m_ListView.Handle));
                    }
                }
            }
        }

        private bool m_fullrowselect = false;

        [Description("Indicates whether all SubItems are highlighted, along with the item when selected."), Category("Data")]
        [DefaultValue(false)]
        public bool FullRowSelect
        {
            get { return m_fullrowselect; }
            set { m_fullrowselect = value;
                m_ListView.FullRowSelect = m_fullrowselect;
            }
        }

        #endregion properties


        private ListViewItem FindItem(ShellItem item)
        {
            if (item == null) return null;

            foreach (ListViewItem lvi in m_ListView.Items)
            {
                if ((ShellItem)lvi.Tag == item)
                    return lvi;
            }

            return null;

        }

        private void RefreshItem(ShellItem OldItem, ShellItem NewItem)
        {
            ListViewItem oldlvi = FindItem(OldItem);
            if (oldlvi != null)
            {
                m_ListView.BeginUpdate();

                m_ListView.ListViewItemSorter = null;
                oldlvi.Text = NewItem.Text;
                oldlvi.ImageIndex = SystemImageListManager.GetIconIndex(NewItem, false);
                oldlvi.Tag = NewItem;
                
                m_ListView.ListViewItemSorter = lvwColumnSorter;
                m_ListView.EndUpdate();


            }
            else
            {
                Console.Write("\nOldItem " + OldItem.Text + " not found");
            }
        }


        #region menus


        /// <summary>
        /// Copies the currently selected items to the clipboard.
        /// </summary>
        public void CopySelectedItems()
        {
            ShellContextMenu contextMenu = new ShellContextMenu(SelectedItems);
            contextMenu.InvokeCopy();
        }

        /// <summary>
        /// Cuts the currently selected items.
        /// </summary>
        public void CutSelectedItems()
        {
            ShellContextMenu contextMenu = new ShellContextMenu(SelectedItems);
            contextMenu.InvokeCut(); ;
        }

        /// <summary>
        /// Deletes the item currently selected in the <see cref="ShellView"/>.
        /// </summary>
        public void DeleteSelectedItems()
        {
            ShellContextMenu contextMenu = new ShellContextMenu(SelectedItems);
            contextMenu.InvokeDelete();
        }

        /// <summary>
        /// Selects all items in the <see cref="ShellView"/>.
        /// </summary>
        public void SelectAll()
        {
            foreach (ListViewItem item in m_ListView.Items)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// Pastes the contents of the clipboard into the current folder.
        /// </summary>
        public void PasteClipboard()
        {
            ShellContextMenu contextMenu = new ShellContextMenu(m_CurrentFolder);
            contextMenu.InvokePaste();
        }

        /// <summary>
        /// Begins a rename on the item currently selected in the 
        /// <see cref="ShellView"/>.
        /// </summary>
        public void RenameSelectedItem()
        {
            User32.EnumChildWindows(m_ListView.Handle, RenameCallback, IntPtr.Zero);
        }

        bool RenameCallback(IntPtr hwnd, IntPtr lParam)
        {
            int itemCount = User32.SendMessage(hwnd,
                MSG.LVM_GETITEMCOUNT, 0, 0);

            for (int n = 0; n < itemCount; ++n)
            {
                LVITEMA item = new LVITEMA();
                item.mask = LVIF.LVIF_STATE;
                item.iItem = n;
                item.stateMask = LVIS.LVIS_SELECTED;
                User32.SendMessage(hwnd, MSG.LVM_GETITEMA,
                    0, ref item);

                if (item.state != 0)
                {
                    User32.SendMessage(hwnd, MSG.LVM_EDITLABEL, n, 0);
                    return false;
                }
            }

            return true;
        }

        #endregion


        #region columns

        /// <summary>
        /// Constant passed to <see cref="SetColumnWidth"/> which causes a column to be auto-sized.
        /// </summary>
        public const int ColumnAutoSize = -1;

        /// <summary>
        /// Constant passed to <see cref="SetColumnWidth"/> which causes a column to be auto-sized
        /// to fit the column header text width.
        /// </summary>
        public const int ColumnAutoSizeToHeader = -2;

        /// <summary>
        /// Gets the width of the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>Width in pixel</returns>
        public int GetColumnWidth(int column)
        {
            //IntPtr wnd = User32.GetWindow(m_ListView.Handle, GetWindow_Cmd.GW_CHILD);// Get listview
            //return User32.SendMessage(wnd, MSG.LVM_GETCOLUMNWIDTH, column, 0);
            return m_ListView.Columns[column].Width;
        }

        /// <summary>
        /// Resizes the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="width">
        /// The width (in pixels) of the column or a special value : <see cref="ColumnAutoSize"/>,
        /// <see cref="ColumnAutoSizeToHeader"/>
        /// </param>
        public void SetColumnWidth(int column, int width)
        {
            //IntPtr wnd = User32.GetWindow(m_ListView.Handle, GetWindow_Cmd.GW_CHILD);// Get listview
            //User32.SendMessage(wnd, MSG.LVM_SETCOLUMNWIDTH, column, width);
            m_ListView.Columns[column].Width = width;

            // FAB - to set the width of the first column (name of file)
            lvFileNameColumn = width;
        }

        #endregion


        #region Events

        /// <summary>
        /// List all files taken into account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_SelectedItemChanged(object sender, EventArgs e)
        {
            //if (SelectedItems.Length > 0)
            if (m_ListView.SelectedItems.Count == 1)
            {
                if (!SelectedItems[0].IsFolder && !SelectedItems[0].IsDisk)
                {
                    string file = SelectedItem.FileSystemPath;
                    string ext = Path.GetExtension(file);
                    switch (ext.ToLower())
                    {
                        case ".mid":
                        case ".kar":
                        case ".xml":
                        case ".musicxml":
                        case ".txt":
                            SelectedIndexChanged?.Invoke(this, file);
                            break;
                        default:
                            SelectedIndexChanged?.Invoke(this, null);
                            break;
                    }
                }
                else
                {
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }

        private void ListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Déterminer si la colonne sélectionnée est déjà la colonne triée.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Inverser le sens de tri en cours pour cette colonne.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Définir le numéro de colonne à trier ; par défaut sur croissant.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Procéder au tri avec les nouvelles options.
            if (e.Column == 1)
                lvwColumnSorter.SortColumnBy = ListViewColumnSorter.SortBy.Tag;
            else
                lvwColumnSorter.SortColumnBy = ListViewColumnSorter.SortBy.Text;

            m_ListView.Sort();

        }
   
        private void ListView_HandleCreated(object sender, EventArgs e)
        {
            SetWindowTheme(m_ListView.Handle, "explorer", null);            
        }

        private void ListView_DoubleClick(object sender, EventArgs e)
        {            
            //if (SelectedItems.Length > 0)
            if (m_ListView.SelectedItems.Count > 0)
            {
                ShellItem item = (ShellItem)m_ListView.SelectedItems[0].Tag;
                if (item != null && item.IsFolder)
                    Navigate(item);
                else
                {
                    invokePlayEdit(true);
                }
            }            
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {                                                  
            switch (e.KeyCode)
            {               
                case Keys.Delete:                    
                    if (m_ListView.SelectedItems.Count > 0)
                        DeleteSelectedItems();
                    break;

                case Keys.Enter:                    
                    if (m_ListView.SelectedItems.Count > 0)
                    {
                        ShellItem item = (ShellItem)m_ListView.SelectedItems[0].Tag;
                        if (item.IsFolder)
                            Navigate(item);
                        else if (item.IsFileSystem)
                            invokePlayEdit(true);
                    }
                    break;

                case Keys.F2:                    
                    if (m_ListView.SelectedItems.Count > 0)
                    {
                        m_ListView.SelectedItems[0].BeginEdit();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;

                case Keys.Space:
                case Keys.F3:
                case Keys.F4:
                case Keys.F6:
                case Keys.F7:
                    // FAB: rename all, replace all
                    lvFunctionKeyClicked(this, e.KeyCode, e.KeyData); //F3 rename all, F4, replace all (Karaboss function keys)
                    break;

                case Keys.F5:
                    RefreshContents();
                    break;
            }

            if (e.Control) { 
                /*
                if (e.Control && e.KeyCode == Keys.A) SelectAll();
                if (e.Control && e.KeyCode == Keys.C) CopySelectedItems();
                if (e.Control && e.KeyCode == Keys.N) lvFunctionKeyClicked(this, e.KeyCode, e.KeyData);
                if (e.Control && e.KeyCode == Keys.V) PasteClipboard();
                if (e.Control && e.KeyCode == Keys.X) CutSelectedItems();
                */
                switch (e.KeyCode)
                {
                    case Keys.A:
                        SelectAll();
                        break;
                    case Keys.C:
                        CopySelectedItems();
                        break;

                    case Keys.K:
                        // Ctrl + k => rename .mid to .kar and reverse
                        SenKeyToParent(this, e.KeyCode);
                        break;

                    case Keys.N:
                        lvFunctionKeyClicked(this, e.KeyCode, e.KeyData);
                        break;
                    case Keys.V:
                        PasteClipboard();
                        break;
                    case Keys.X:
                        CutSelectedItems();
                        break;
                }

            }

        }       

        private void ListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Click on a ListViewItem
                ListViewItem lvi = m_ListView.GetItemAt(e.X, e.Y);
                if (lvi != null)
                {
                    ShellItem item = (ShellItem)lvi.Tag;
                    bool bShowKarMenu = true;
                    if (item.IsFolder || item.IsDisk)
                        bShowKarMenu = false;

                    if (item != null && lvi == m_RightClickLvi)
                    {
                        // ListViewItem ContextMenu for a click on a ListViewItem
                        GetListViewItemMenu(item, e.Location, bShowKarMenu);                      
                    }
                }
                else
                {
                    // Click outside a ListViewItem => ListView context menu
                    GetListViewMenu(MousePosition);                    
                }
            }
        }

        private void ListView_MouseDown(object sender, MouseEventArgs e)
        {
            m_RightClickLvi = m_ListView.GetItemAt(e.X, e.Y);
        }

        private void ListView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            ShellItem item = m_ListView.Items[e.Item].Tag as ShellItem;

            if (!item.CanRename)
            {
                e.CancelEdit = true;
                System.Media.SystemSounds.Beep.Play();
                return;
            }
            else if (item.IsDisk)
            {
                //int editHandle = User32.SendMessage(m_ListView.Handle, MSG.LVM_GETEDITCONTROL, 0, IntPtr.Zero);
                //User32.SendMessage(editHandle, MSG.WM_SETTEXT, 0, Marshal.StringToHGlobalAuto(item.Text.Substring(0, item.Text.LastIndexOf(' '))));
            }
        }

        private void ListView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            ShellItem item = m_ListView.Items[e.Item].Tag as ShellItem;

            //if (e.Label != null && e.Label != String.Empty && SelectedItems.Length == 1)
            if (e.Label != null && e.Label != String.Empty && m_ListView.SelectedItems.Count == 1)
            {
                #region Rename One

                m_CreateNew = false;

                string NewName = e.Label.Trim();
                IntPtr newPidl = IntPtr.Zero;
                try
                {
                    uint res = item.Parent.GetIShellFolder().SetNameOf(m_ListView.Handle, Shell32.ILFindLastID(item.Pidl), NewName, SHGDN.NORMAL, out newPidl);
                }
                catch (COMException ex)
                {
                    // Ignore the exception raised when the user cancels
                    // a delete operation.
                    if (ex.ErrorCode != unchecked((int)0x800704C7) &&
                        ex.ErrorCode != unchecked((int)0x80270000))
                    {
                        throw;
                    }
                    e.CancelEdit = true;
                }

                #endregion
            }
        }

        private void ListView_ItemMouseOver(object sender, ListViewItemMouseHoverEventArgs e)
        {            
            ToolTip tp = new ToolTip();
            tp.Show( ((ShellItem)e.Item.Tag).ToolTipText, m_ListView);                    
            
        }

        private void ListView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
                selectedOrder.Insert(0, e.Item);
            else
                selectedOrder.Remove(e.Item);
        }

        #endregion


        #region Menu

        /// <summary>
        /// Specific Karaboss Play or Edit file
        /// </summary>
        /// <param name="bplay"></param>
        private void invokePlayEdit(bool bplay)
        {
            if (SelectedItem != null && SelectedItem.IsFileSystem)
            {
                string file = SelectedItem.FileSystemPath;
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
                    case ".mml":
                    case ".abc":
                        {
                            PlayAbc?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }
                    case ".musicxml":
                    case ".xml":
                        {
                            PlayXml?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }
                    case ".txt":
                        {
                            PlayTxt?.Invoke(this, new FileInfo(file), bplay);
                            break;
                        }
                    default:
                        try
                        {
                            System.Diagnostics.Process.Start(@file);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        break;
                }
            }
        }


        #region ListView menu
        /// <summary>
        /// ListView ContextMenu for a Click on the ListView itself (not on a ListViewItem)
        /// </summary>
        /// <param name="pt">System.Drawing.Point</param>
        private void GetListViewMenu(System.Drawing.Point pt)
        {
            int HR;
            int min = 1;
            //int max = 100000;
            bool bCleanup = false;

            CMInvokeCommandInfoEx cmi = new CMInvokeCommandInfoEx();
            IntPtr comContextMenu = User32.CreatePopupMenu();           // main context menu
            IntPtr viewSubMenu = User32.CreatePopupMenu();              // sub context menu of view 

            //Check item count - should always be 0 but check just in case
            uint startIndex = User32.GetMenuItemCount(comContextMenu);
            

            #region menu cascading view        
            MENUITEMINFO itemInfo = MENUITEMINFO.New(Strings.View);                  

            itemInfo.fMask = (MIIM.MIIM_SUBMENU | MIIM.MIIM_STRING);
            itemInfo.hSubMenu = viewSubMenu;

            User32.InsertMenuItem(comContextMenu, 0, true, ref itemInfo);
            #endregion

            #region menu view choice
            MFT ichecked = MFT.MFT_BYCOMMAND;
            if (m_ListView.View == System.Windows.Forms.View.Tile)
                ichecked = (MFT.MFT_RADIOCHECK | MFT.MFT_CHECKED);
            User32.AppendMenu(viewSubMenu, ichecked, (int)CMD.TILES, Strings.Tiles);

            ichecked = MFT.MFT_BYCOMMAND;
            if (m_ListView.View == System.Windows.Forms.View.LargeIcon)
                ichecked = (MFT.MFT_RADIOCHECK | MFT.MFT_CHECKED);

            User32.AppendMenu(viewSubMenu, ichecked, (int)CMD.LARGEICON, Strings.LargeIcons);

            ichecked = MFT.MFT_BYCOMMAND;
            if (m_ListView.View == System.Windows.Forms.View.List)
                ichecked = (MFT.MFT_RADIOCHECK | MFT.MFT_CHECKED);

            User32.AppendMenu(viewSubMenu, ichecked, (int)CMD.LIST, Strings.List);

            ichecked = MFT.MFT_BYCOMMAND;
            if (m_ListView.View == System.Windows.Forms.View.Details)
                ichecked = (MFT.MFT_RADIOCHECK | MFT.MFT_CHECKED);
            User32.AppendMenu(viewSubMenu, ichecked, (int)CMD.DETAILS, Strings.Details);

            //ichecked = MFT.MFT_BYCOMMAND;
            #endregion

            User32.AppendMenu(comContextMenu, MFT.MFT_SEPARATOR, 0, string.Empty);

            #region menu refresh
            User32.AppendMenu(comContextMenu, MFT.MFT_BYCOMMAND, (int)CMD.REFRESH, Strings.Refresh);
            #endregion

            User32.AppendMenu(comContextMenu, MFT.MFT_SEPARATOR, 0, string.Empty);

            #region menu paste
            MFT enabled = MFT.MFT_GRAYED;
            DragDropEffects effects = new DragDropEffects();

            if (SelectedItem == null)
                enabled = MFT.MFT_BYCOMMAND;
            else
            {
                try
                {
                    effects = ShellHelper.CanDropClipboard(SelectedItem);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }

                if (((effects & DragDropEffects.Copy) == DragDropEffects.Copy) || ((effects & DragDropEffects.Move) == DragDropEffects.Move)) // Enable paste for stand-alone ExpList
                    enabled = (int)MFT.MFT_BYCOMMAND;

            }
            User32.AppendMenu(comContextMenu, enabled, (int)CMD.PASTE, Strings.Paste);
            #endregion

            #region menu paste link
            if (SelectedItem != null)
            {
                enabled = MFT.MFT_GRAYED;
                if ((effects & DragDropEffects.Link) == DragDropEffects.Link)
                    enabled = (int)MFT.MFT_BYCOMMAND;
            }

            User32.AppendMenu(comContextMenu, enabled, (int)CMD.PASTELINK, Strings.PasteLink);
            #endregion

            User32.AppendMenu(comContextMenu, MFT.MFT_SEPARATOR, 0, String.Empty);

            #region menu new
            // Add the 'New' menu
            if (SelectedItem.IsFolder && ((!SelectedItem.FileSystemPath.StartsWith("::")) || (SelectedItem == ShellItem.Desktop)))
            {
                uint xIndex = User32.GetMenuItemCount(comContextMenu);
                m_WindowsContextMenu.SetUpNewMenu(SelectedItem, comContextMenu, (int)xIndex); // 6) // 7)
                User32.AppendMenu(comContextMenu, MFT.MFT_SEPARATOR, 0, String.Empty);
            }
            #endregion

            #region menu properties
            // Properties menu
            User32.AppendMenu(comContextMenu, (int)MFT.MFT_BYCOMMAND, (int)CMD.PROPERTIES, Strings.Properties);
            #endregion


            #region respond menu
            // Track selected choice
            int cmdID = User32.TrackPopupMenuEx(comContextMenu, TPM.TPM_RETURNCMD, pt.X, pt.Y, this.Handle, IntPtr.Zero);

            
            if (cmdID >= min)
            {
                cmi = new CMInvokeCommandInfoEx();
                cmi.cbSize = Marshal.SizeOf(cmi);
                cmi.nShow = (int)SW.SHOWNORMAL;
                cmi.fMask = (int)(CMIC.UNICODE | CMIC.PTINVOKE);
                cmi.ptInvoke = new System.Drawing.Point(pt.X, pt.Y);

                switch (cmdID)
                {
                    case (int)CMD.TILES:
                        m_ListView.View = System.Windows.Forms.View.Tile;
                        bCleanup = true;
                        break;

                    case (int)CMD.LARGEICON:
                        m_ListView.View = System.Windows.Forms.View.LargeIcon;
                        bCleanup = true;
                        break;

                    case (int)CMD.LIST:
                        m_ListView.View = System.Windows.Forms.View.List;
                        bCleanup = true;
                        break;

                    case (int)CMD.DETAILS:
                        m_ListView.View = System.Windows.Forms.View.Details;
                        bCleanup = true;
                        break;

                    case (int)CMD.REFRESH:
                        //if (SelectedItem != null)
                        //    SelectedItem.UpdateRefresh();
                        //SortLVItems();
                        bCleanup = true;
                        break;

                    case (int)CMD.PASTE:
                        if (SelectedItem != null)
                        {
                            cmi.lpVerb = Marshal.StringToHGlobalAnsi("paste");
                            cmi.lpVerbW = Marshal.StringToHGlobalUni("paste");
                        }
                        else
                        {
                            bCleanup = true;
                        }
                        break;

                    case (int)CMD.PASTELINK:
                        cmi.lpVerb = Marshal.StringToHGlobalAnsi("pastelink");
                        cmi.lpVerbW = Marshal.StringToHGlobalUni("pastelink");
                        break;

                    case (int)CMD.PROPERTIES:
                        cmi.lpVerb = Marshal.StringToHGlobalAnsi("properties");
                        cmi.lpVerbW = Marshal.StringToHGlobalUni("properties");
                        break;
                    
                    
                    default:
                        cmdID -= 1; //12/15/2010 Change
                        cmi.lpVerb = (IntPtr)cmdID;
                        cmi.lpVerbW = (IntPtr)cmdID;

                        // New item created
                        m_CreateNew = true;

                        HR = m_WindowsContextMenu.newMenu.InvokeCommand(cmi);
#if DEBUG
                        //if (HR != (int)HResult.S_OK)
                        //    Marshal.ThrowExceptionForHR(HR);
#endif
                        bCleanup = true;
                        break;

                }

                // Invoke the Paste, Paste Shortcut or Properties command
                if (SelectedItem != null)
                {
                    int prgf = 0;
                    IntPtr iunk = IntPtr.Zero;
                    IShellFolder folder = null;
                    if (SelectedItem == ShellItem.Desktop)
                        folder = SelectedItem.GetIShellFolder();
                    else
                        folder = SelectedItem.Parent.GetIShellFolder();


                    IntPtr relPidl = Shell32.ILFindLastID(SelectedItem.Pidl);
                    HR = folder.GetUIObjectOf(IntPtr.Zero, 1, new IntPtr[] { relPidl }, Shell32.IID_IContextMenu, (uint)prgf, out iunk);
#if DEBUG
                    if (HR != (int)HResult.S_OK)
                        Marshal.ThrowExceptionForHR(HR);

#endif

                    m_WindowsContextMenu.winMenu = (IContextMenu)Marshal.GetObjectForIUnknown(iunk);
                    HR = m_WindowsContextMenu.winMenu.InvokeCommand(cmi);
                    m_WindowsContextMenu.ReleaseMenu();

#if DEBUG
                    if (HR != (int)HResult.S_OK)
                    {
                        try
                        {
                            //Marshal.ThrowExceptionForHR(HR);
                        }
                        catch
                        {

                        }
                    }

#endif
                }
            }     //12/15/2010 change

            #endregion 

            //CLEANUP:
            if (bCleanup)
            {
                m_WindowsContextMenu.ReleaseNewMenu();

                Marshal.Release(comContextMenu);
                comContextMenu = IntPtr.Zero;
                Marshal.Release(viewSubMenu);
                viewSubMenu = IntPtr.Zero;
            }
        }

        /// <summary>
        /// ListViewItem ContextMenu for a click on a ListViewItem
        /// </summary>
        /// <param name="pt"></param>
        private void GetListViewItemMenu(ShellItem item, System.Drawing.Point pt, bool bShowKarMenu)
        {
            const int m_CmdFirst = 0x8000;

            ShellContextMenu shm = new ShellContextMenu(SelectedItems);  

            ContextMenu menu = new ContextMenu();       // main menu
            IntPtr plmenu = User32.CreatePopupMenu();   // submenu of playlists

            Point pos = this.PointToScreen(pt);
            uint PLCMD = 1000;
            int plmin = (int)PLCMD;
            int plmax = plmin;

            // Populate
            shm.RemoveShellMenuItems(menu);

            if (bShowKarMenu)
            {

                #region menu cascading "Add to playlist"        
                MENUITEMINFO itemInfo = MENUITEMINFO.New(Strings.addToPlaylist);

                itemInfo.fMask = (MIIM.MIIM_SUBMENU | MIIM.MIIM_STRING);
                itemInfo.hSubMenu = plmenu;

                User32.InsertMenuItem(menu.Handle, 0, true, ref itemInfo);
                #endregion


                #region menu playlist items
                string plName = string.Empty;
                MFT ichecked = MFT.MFT_BYCOMMAND;
                
                if ( m_allPlaylists.GetLength(0) > 0)
                {
                    // display existing playlists                    
                    for (int i = 0; i < m_allPlaylists.GetLength(0); i++)
                    {                        
                        plName = m_allPlaylists[i, 1];
                        User32.AppendMenu(plmenu, ichecked, PLCMD, plName);
                        PLCMD++;
                    }                    
                }

                // Separator
                // Add a separator
                User32.AppendMenu(plmenu, MFT.MFT_SEPARATOR, 0, string.Empty);

                // Add to a new playlist
                plName = Strings.NewPlaylist;
                User32.AppendMenu(plmenu, ichecked, PLCMD, plName);
                PLCMD++;

                plmax = (int)PLCMD;

                #endregion


                #region menu play, edit
                User32.InsertMenu(menu.Handle, 1, (int)(MFT.MFT_BYPOSITION), (IntPtr)100, Strings.Play);
                User32.InsertMenu(menu.Handle, 2, (int)(MFT.MFT_BYPOSITION), (IntPtr)101, Strings.Edit);                              

                User32.InsertMenu(menu.Handle, 3, (int)(MFT.MFT_BYPOSITION | MFT.MFT_SEPARATOR), (IntPtr)0, string.Empty);
                #endregion

            }


            #region respond menu
            uint idx = 0;
            if (bShowKarMenu)
                idx = 4;

            shm.ComInterface.QueryContextMenu(
                   menu.Handle,
                   idx,
                   m_CmdFirst,
                   int.MaxValue,
                   CMF.EXPLORE |
                   CMF.CANRENAME |
                   ((Control.ModifierKeys & Keys.Shift) != 0 ?
                   CMF.EXTENDEDVERBS : 0));

            int command = User32.TrackPopupMenuEx(menu.Handle,
                  TPM.TPM_RETURNCMD, pos.X, pos.Y, this.Handle,
                  IntPtr.Zero);

            #region execute command
            if (command > 0)
            {
                if (command == 100)
                {
                    invokePlayEdit(true);
                }
                else if (command == 101)
                {
                    invokePlayEdit(false);
                }
                else if (command >= plmin && command < plmax)
                {
                    int id = command - 1000;
                    string plname = string.Empty;

                    ShellItem[] fls =  SelectedItems;
                    
                    if (id < m_allPlaylists.GetLength(0))
                    {
                        // Add to an existing playlist
                        plname = m_allPlaylists[command - 1000, 1];
                        string key = m_allPlaylists[command - 1000, 0];
                        AddToPlaylist?.Invoke(this, fls, plname, key, false);
                    }
                    else
                    {
                        // Add to a new playlist
                        AddToPlaylist?.Invoke(this, fls, string.Empty, null ,true);
                    }

                    Console.Write(plname);

                }
                else if (command - m_CmdFirst == 18)
                {
                    // Rename item
                    m_ListView.SelectedItems[0].BeginEdit();
                }
                else
                {
                    // Other menus entries
                    shm.InvokeCommand(command - m_CmdFirst);
                }
            }
            #endregion

            #endregion

        }
       

        #endregion

        /// <summary>
        /// Handles Windows Messages having to do with the display of Cascading menus of the Context Menu.
        /// </summary>
        /// <param name="m">The Windows Message</param>
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //For send to menu in the ListView context menu
            HResult hr = 0;
            if ((m.Msg == (int)WM.INITMENUPOPUP) || (m.Msg == (int)WM.MEASUREITEM) || (m.Msg == (int)WM.DRAWITEM))
            {
                if (m_WindowsContextMenu.winMenu2 != null)
                {
                    hr = m_WindowsContextMenu.winMenu2.HandleMenuMsg(m.Msg, m.WParam, m.LParam);
                    if (hr == 0)
                        return;
                }
                else if ((m.Msg == (int)WM.INITMENUPOPUP) && (m.WParam == m_WindowsContextMenu.newMenuPtr) ||
                     (m.Msg == (int)WM.MEASUREITEM) || (m.Msg == (int)WM.DRAWITEM))
                {
                    if (m_WindowsContextMenu.newMenu2 != null)
                    {
                        hr = m_WindowsContextMenu.newMenu2.HandleMenuMsg(m.Msg, m.WParam, m.LParam);
                        if (hr == 0)
                            return;

                    }
                }
            }
            else if (m.Msg == (int)WM.MENUCHAR)
            {
                if (m_WindowsContextMenu.winMenu3 != null)
                {
                    IntPtr lpresult = IntPtr.Zero;
                    hr = m_WindowsContextMenu.winMenu3.HandleMenuMsg2(m.Msg, m.WParam, m.LParam, out lpresult);
                    if (hr == 0)
                        return;

                }
            }
            base.WndProc(ref m);
        }

        #endregion


        #region ShellListener
        private void m_ShellListener_ItemRenamed(object sender, ShellItemChangeEventArgs e)
        {
            RefreshItem(e.OldItem, e.NewItem);

            // FAB 15/11/23
            // Décoche ligne cochée           
            Navigate(m_CurrentFolder);

        }

        private void m_ShellListener_ItemUpdated(object sender, ShellItemEventArgs e)
        {
            
            if (m_CurrentFolder != null)
            {

                // FAB: 14/09/17 removed because folder content not updated when copy of several folders
                // exit if item updated is not in the current folder
                //if (e.Item.Parent != m_CurrentFolder)
                //    return;                

                // FAB 15/11/23
                // Décoche ligne cochée
                RecreateShellView(m_CurrentFolder);

                // FAB 15/11/23
                // Coche lignes non cochées
                //if (e.Item.ToString() != "shell:///" && e.Item.FileSystemPath.IndexOf(m_CurrentFolder.FileSystemPath) >= 0)
                //    Navigate(m_CurrentFolder);

            }

        }



        private void m_ShellListener_ItemCreated(object sender, ShellItemEventArgs e)
        {
            if (m_CurrentFolder != null)
            {

                // FAB 15/11/23
                // Coche lignes non cochées
                // exit if item updated is not in the current folder
                //if (e.Item.Parent != m_CurrentFolder)
                //    return;
                
                RecreateShellView(m_CurrentFolder);

                if (m_CreateNew)
                {
                    m_EditItem = e.Item;

                    ListViewItem lvi = FindItem(e.Item);
                    if (lvi != null)
                        lvi.BeginEdit();
                }
            }
        }

        #endregion


        #region Drag

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {                                 
            DragDropEffects effect;

            // Drag multiple items located in the same folder
            IntPtr dataObjectPtr = IntPtr.Zero;
            dataObjectPtr = ShellHelper.GetIDataObject(SelectedItems);            
            Ole32.DoDragDrop(dataObjectPtr, this, DragDropEffects.All, out effect);                    
        }


        #endregion


        #region IDropSource Members

        HResult IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return HResult.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        HResult IDropSource.QueryContinueDrag(bool fEscapePressed, MK grfKeyState)
        {
            if (fEscapePressed)
            {
                return HResult.DRAGDROP_S_CANCEL;
            }
            else if ((grfKeyState & (MK.MK_LBUTTON | MK.MK_RBUTTON)) == 0)
            {
                return HResult.DRAGDROP_S_DROP;
            }
            else
            {
                return HResult.S_OK;
            }
        }
        #endregion


        #region IDropTarget Members
        
        void Interop.IDropTarget.DragEnter(ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            Point clientLocation = m_ListView.PointToClient(pt);
            ListViewItem item = m_ListView.HitTest(clientLocation).Item;           

            DragTarget.Data = pDataObj;
            m_ListView.HideSelection = true;

            m_DragTarget = new DragTarget(m_ListView, CurrentFolder, item, grfKeyState, pt, ref pdwEffect);           
        }        
        
        void Interop.IDropTarget.DragOver(MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            Point clientLocation = m_ListView.PointToClient(pt);
            ListViewItem item = m_ListView.HitTest(clientLocation).Item;          

            if (item != null)
            {
                if ((m_DragTarget == null) ||
                    (item != m_DragTarget.Item))
                {

                    if (m_DragTarget != null)
                    {
                        m_DragTarget.Dispose();
                    }

                    m_DragTarget = new DragTarget(m_ListView, CurrentFolder, item, grfKeyState, pt, ref pdwEffect);
                }
                else
                {
                    m_DragTarget.DragOver(grfKeyState, pt, ref pdwEffect);
                }
            }           
        }

        void Interop.IDropTarget.DragLeave()
        {
            if (m_DragTarget != null)
            {
                m_DragTarget.Dispose();
                m_DragTarget = null;
            }
            m_ListView.HideSelection = false;
        }

        void Interop.IDropTarget.Drop(ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            try
            {
                if (m_DragTarget != null)
                {
                    m_DragTarget.Drop(pDataObj, grfKeyState, pt, ref pdwEffect);
                    m_DragTarget.Dispose();
                    m_DragTarget = null;
                }
            }
            catch (COMException ce)
            {
                // Ignore the exception raised when the user cancels
                // a delete operation.
                if (ce.ErrorCode != unchecked((int)0x800704C7) &&
                    ce.ErrorCode != unchecked((int)0x80270000))
                {
                    throw;
                }

                if (m_DragTarget != null)
                {                 
                    m_DragTarget.Dispose();
                    m_DragTarget = null;
                }
            }
        }

        #endregion


        #region DragTarget

        class DragTarget : IDisposable
        {
            public DragTarget(Control ctrl ,ShellItem parentfolder, ListViewItem item, MK keyState, Point pt, ref DragDropEffects effect)
            {               

                m_parentFolder = parentfolder;

                if (item != null)
                {
                    m_Item = item;
                    m_Item.BackColor = SystemColors.Highlight;
                    m_Item.ForeColor = SystemColors.HighlightText;
                }

                try
                {                    
                    m_DropTarget = Folder.GetIDropTarget(ctrl);
                    m_DropTarget.DragEnter(m_Data, keyState, pt, ref effect);                    
                }
                catch (Exception ex)
                {
                    Console.Write("\nERROR: ShellListView.cs\n" + ex.Message);
                }
            }

            public void Dispose()
            {
                if (m_Item != null && m_Item.ListView != null)
                {
                    m_Item.BackColor = m_Item.ListView.BackColor;
                    m_Item.ForeColor = m_Item.ListView.ForeColor;
                }
             

                if (m_DropTarget != null)
                {
                    m_DropTarget.DragLeave();
                }
            }

            public void DragOver(MK keyState, Point pt, ref DragDropEffects effect)
            {
                if (m_DropTarget != null)
                {
                    m_DropTarget.DragOver(keyState, pt, ref effect);
                }
                else
                {
                    effect = 0;
                }
            }

            public void Drop(ComTypes.IDataObject data, MK keyState, Point pt, ref DragDropEffects effect)
            {
                if (m_DropTarget != null)
                    m_DropTarget.Drop(data, keyState, pt, ref effect);
            }

            public ShellItem Folder
            {
                get {
                    
                    if (m_Item == null)
                        return m_parentFolder;

                    ShellItem shi = (ShellItem)m_Item.Tag;
                    if (shi.IsFolder)
                        return shi;
                    else
                        return shi.Parent;
                }
            }

            public ListViewItem Item
            {
                get { return m_Item; }
            }

            public static ComTypes.IDataObject Data
            {
                get { return m_Data; }
                set { m_Data = value; }
            }


            ListViewItem m_Item;
            ShellItem m_parentFolder;
            Interop.IDropTarget m_DropTarget;           
            static ComTypes.IDataObject m_Data;
        }

        #endregion


        ListView m_ListView;
        ListViewItem m_RightClickLvi;
        DragTarget m_DragTarget;        
        bool m_AllowDrop = false;
        ShellViewStyle m_View;

        ShellNotificationListener m_ShellListener = new ShellNotificationListener();

        ShellHistory m_History;
        bool m_MultiSelect;
        ShellItem m_CurrentFolder;
        PropertyChangedEventHandler m_PropertyChanged;
        ShowHidden m_ShowHidden = ShowHidden.System;

        // The arraylist to store the order by which ListViewItems has been selected
        private ArrayList selectedOrder;

        bool m_CreateNew = false;
        ShellItem m_EditItem = null;

    }


}
