using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
// GongSolutions.Shell - A Windows Shell library for .Net.
// Copyright (C) 2007-2009 Steven J. Kirk
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either 
// version 2 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free 
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor,  
// Boston, MA 2110-1301, USA.
//
using Microsoft.Win32;
using FlShell.Interop;
using ComTypes = System.Runtime.InteropServices.ComTypes;


namespace FlShell
{
    /// <summary>
    /// Provides a tree view of a computer's folders.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// The <see cref="ShellTreeView"/> control allows you to embed Windows 
    /// Explorer functionality in your Windows Forms applications. The
    /// control provides a tree view of the computer's folders, as it would 
    /// appear in the left-hand pane in Explorer.
    /// </para>
    /// </remarks>
    public class ShellTreeView : Control, Interop.IDropSource, Interop.IDropTarget
    {

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, String pszSubAppName, String pszSubIdList);

        /// <summary>
        /// FAB: function keys F3, F4 keydown sent to application
        /// </summary>
        public event tvFunctionKeyEventHandler tvFunctionKeyClicked;

        /// <summary>
        /// Occurs when the <see cref="SelectedFolder"/> property changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        enum ScrollDirection
        {
            None = -1, Up, Down
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShellTreeView"/> class.
        /// </summary>
        public ShellTreeView()
        {
            m_TreeView = new TreeView();            
            m_TreeView.Dock = DockStyle.Fill;
                        
            SetFontScheme();

            m_TreeView.HideSelection = false;            
            m_TreeView.HotTracking = true;
            m_TreeView.Parent = this;
            m_TreeView.ShowRootLines = false;            

            m_TreeView.ShowLines = false;
            m_TreeView.FullRowSelect = true; // highlight all row (ignored if Showlines is set to true)

            m_TreeView.AfterSelect += new TreeViewEventHandler(m_TreeView_AfterSelect);
            m_TreeView.BeforeExpand += new TreeViewCancelEventHandler(m_TreeView_BeforeExpand);
            m_TreeView.ItemDrag += new ItemDragEventHandler(m_TreeView_ItemDrag);
            m_TreeView.MouseDown += new MouseEventHandler(m_TreeView_MouseDown);
            m_TreeView.MouseUp += new MouseEventHandler(m_TreeView_MouseUp);
            m_TreeView.HandleCreated += new EventHandler(TreeView_HandleCreated);

            m_TreeView.KeyDown += new KeyEventHandler(m_TreeView_KeyDown);
            m_TreeView.AfterLabelEdit += new NodeLabelEditEventHandler(m_TreeView_AfterLabelEdit);

            // FAB
            m_TreeView.LabelEdit = true;

            m_ScrollTimer.Interval = 250;
            m_ScrollTimer.Tick += new EventHandler(m_ScrollTimer_Tick);
            Size = new Size(120, 100);
            SystemImageList.UseSystemImageList(m_TreeView);

            m_ShellListener.DriveAdded += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.DriveRemoved += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.FolderCreated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.FolderDeleted += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.FolderRenamed += new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);
            m_ShellListener.FolderUpdated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.ItemCreated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.ItemDeleted += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.ItemRenamed += new ShellItemChangeEventHandler(m_ShellListener_ItemRenamed);
            m_ShellListener.ItemUpdated += new ShellItemEventHandler(m_ShellListener_ItemUpdated);
            m_ShellListener.SharingChanged += new ShellItemEventHandler(m_ShellListener_ItemUpdated);

            // Accessibility : manage user size of font
            Microsoft.Win32.SystemEvents.UserPreferenceChanged += new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            // Setting AllowDrop to true then false makes sure OleInitialize()
            // is called for the thread: it must be called before we can use
            // RegisterDragDrop. There is probably a neater way of doing this.
            m_TreeView.AllowDrop = true;
            m_TreeView.AllowDrop = false;

            
            // FAB: Mandatory to allowdrop to treeview 
            this.AllowDrop = true;

            CreateItems();
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



                Microsoft.Win32.SystemEvents.UserPreferenceChanged -= new Microsoft.Win32.UserPreferenceChangedEventHandler(this.UserPreferenceChanged);

            }

            base.Dispose(disposing);
        }

        #endregion


        #region Public
        /// <summary>
        /// Refreses the contents of the <see cref="ShellTreeView"/>.
        /// </summary>
        public void RefreshContents()
        {
            RefreshItem(m_TreeView.Nodes[0]);
        }

        #endregion


        #region Properties

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
                            Marshal.ThrowExceptionForHR(Ole32.RegisterDragDrop(m_TreeView.Handle, this));
                        }
                        catch (Exception ex)
                        {
                            Console.Write("\nShellTreeView: " + ex.Message);
                        }
                    }
                    else
                    {
                        Marshal.ThrowExceptionForHR(
                            Ole32.RevokeDragDrop(m_TreeView.Handle));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a tree node label takes on
        /// the appearance of a hyperlink as the mouse pointer passes over it.
        /// </summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        public bool HotTracking
        {
            get { return m_TreeView.HotTracking; }
            set { m_TreeView.HotTracking = value; }
        }

        /// <summary>
        /// Gets or sets the root folder that is displayed in the 
        /// <see cref="ShellTreeView"/>.
        /// </summary>
        [Category("Appearance")]
        public ShellItem RootFolder
        {
            get { return m_RootFolder; }
            set
            {
                m_RootFolder = value;
                CreateItems();
            }
        }


        
        /// <summary>
        /// Gets/sets a <see cref="ShellView"/> whose navigation should be
        /// controlled by the treeview.
        /// </summary>
        [DefaultValue(null), Category("Behaviour")]
        public ShellView ShellView
        {
            get { return m_ShellView; }
            set
            {
                if (m_ShellView != null)
                {
                    m_ShellView.Navigated -= new EventHandler(m_ShellView_Navigated);
                }

                m_ShellView = value;

                if (m_ShellView != null)
                {
                    m_ShellView.Navigated += new EventHandler(m_ShellView_Navigated);
                    m_ShellView_Navigated(m_ShellView, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets/sets a <see cref="ShellView"/> whose navigation should be
        /// controlled by the treeview.
        /// </summary>
        [DefaultValue(null), Category("Behaviour")]
        public ShellListView ShellListView
        {
            get { return m_ShellListView; }
            set
            {
                if (m_ShellListView != null)
                {
                    m_ShellListView.Navigated -= new EventHandler(m_ShellListView_Navigated);
                }

                m_ShellListView = value;

                if (m_ShellListView != null)
                {
                    m_ShellListView.Navigated += new EventHandler(m_ShellListView_Navigated);
                    m_ShellListView_Navigated(m_ShellListView, EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Gets or sets the selected folder in the 
        /// <see cref="ShellTreeView"/>.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Editor(typeof(ShellItemEditor), typeof(UITypeEditor))]
        public ShellItem SelectedFolder
        {
            get {
                return (ShellItem)m_TreeView.SelectedNode.Tag;
            }
            set { SelectItem(value); }
        }
        

        /// <summary>
        /// Gets or sets a value indicating whether hidden folders should
        /// be displayed in the tree.
        /// </summary>
        [DefaultValue(ShowHidden.System), Category("Appearance")]
        public ShowHidden ShowHidden
        {
            get { return m_ShowHidden; }
            set
            {
                m_ShowHidden = value;
                RefreshContents();
            }
        }

        #endregion
    

        #region Hidden Properties

        /// <summary>
        /// This property does not apply to the <see cref="ShellTreeView"/> 
        /// class.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #endregion


        #region Create
        void CreateItems()
        {
            m_TreeView.BeginUpdate();

            try
            {
                m_TreeView.Nodes.Clear();
                CreateItem(null, m_RootFolder);
                m_TreeView.Nodes[0].Expand();
                m_TreeView.SelectedNode = m_TreeView.Nodes[0];
            }
            finally
            {
                m_TreeView.EndUpdate();
            }
        }

        void CreateItem(TreeNode parent, ShellItem folder)
        {
            string displayName = folder.DisplayName;
            TreeNode node;

            if (parent != null)
            {
                node = InsertNode(parent, folder, displayName);
            }
            else
            {
                node = m_TreeView.Nodes.Add(displayName);
            }

            if (folder.HasSubFolders)
            {
                node.Nodes.Add("");
            }

            node.Tag = folder;
            SetNodeImage(node);
        }

        void CreateChildren(TreeNode node)
        {
            if ((node.Nodes.Count == 1) && (node.Nodes[0].Tag == null))
            {
                ShellItem folder = (ShellItem)node.Tag;

                if (folder != null)
                {
                    IEnumerator<ShellItem> e = GetFolderEnumerator(folder);

                    node.Nodes.Clear();
                    while (e.MoveNext())
                    {
                        CreateItem(node, e.Current);
                    }
                }

            }
        }

        #endregion


        #region Internal

        private void UserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            SetFontScheme();
        }

        private void SetFontScheme()
        {
            m_TreeView.Font = new System.Drawing.Font("Segoe UI", SystemFonts.MenuFont.Size, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            
        }

        void RefreshItem(TreeNode node)
        {
            try
            {
                ShellItem folder = (ShellItem)node.Tag;

                // patch FAB 29/06/2016
                if (folder != null)
                {
                    node.Text = folder.DisplayName;
                    SetNodeImage(node);


                    if (NodeHasChildren(node))
                    {
                        IEnumerator<ShellItem> e = GetFolderEnumerator(folder);
                        ArrayList nodesToRemove = new ArrayList(node.Nodes);

                        while (e.MoveNext())
                        {
                            TreeNode childNode = FindItem(e.Current, node);

                            if (childNode != null)
                            {
                                RefreshItem(childNode);
                                nodesToRemove.Remove(childNode);
                            }
                            else
                            {
                                CreateItem(node, e.Current);
                            }
                        }

                        foreach (TreeNode n in nodesToRemove)
                        {
                            n.Remove();
                        }
                    }
                    else if (node.Nodes.Count == 0)
                    {
                        if (folder.HasSubFolders)
                        {
                            node.Nodes.Add("");
                        }
                    }

                    
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        TreeNode InsertNode(TreeNode parent, ShellItem folder, string displayName)
        {
            ShellItem parentFolder = (ShellItem)parent.Tag;
            IntPtr folderRelPidl = Shell32.ILFindLastID(folder.Pidl);
            TreeNode result = null;

            foreach (TreeNode child in parent.Nodes)
            {
                ShellItem childFolder = (ShellItem)child.Tag;
                IntPtr childRelPidl = Shell32.ILFindLastID(childFolder.Pidl);
                short compare = parentFolder.GetIShellFolder().CompareIDs(0,
                       folderRelPidl, childRelPidl);

                if (compare < 0)
                {
                    result = parent.Nodes.Insert(child.Index, displayName);
                    break;
                }
            }

            if (result == null)
            {
                result = parent.Nodes.Add(displayName);
            }

            return result;
        }

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

        void SetNodeImage(TreeNode node)
        {
            TVITEMW itemInfo = new TVITEMW();
            ShellItem folder = (ShellItem)node.Tag;

            if (folder != null)
            {

                // We need to set the images for the item by sending a 
                // TVM_SETITEMW message, as we need to set the overlay images,
                // and the .Net TreeView API does not support overlays.
                itemInfo.mask = TVIF.TVIF_IMAGE | TVIF.TVIF_SELECTEDIMAGE |
                                TVIF.TVIF_STATE;
                itemInfo.hItem = node.Handle;
                itemInfo.iImage = folder.GetSystemImageListIndex(
                    ShellIconType.SmallIcon, ShellIconFlags.OverlayIndex);
                itemInfo.iSelectedImage = folder.GetSystemImageListIndex(
                    ShellIconType.SmallIcon, ShellIconFlags.OpenIcon);
                itemInfo.state = (TVIS)(itemInfo.iImage >> 16);
                itemInfo.stateMask = TVIS.TVIS_OVERLAYMASK;
                User32.SendMessage(m_TreeView.Handle, MSG.TVM_SETITEMW, 0, ref itemInfo);
            }
        }

        void SelectItem(ShellItem value)
        {
            TreeNode node = m_TreeView.Nodes[0];
            ShellItem folder = (ShellItem)node.Tag;

            if (folder !=null && folder == value)
            {
                m_TreeView.SelectedNode = node;
            }
            else
            {
                SelectItem(node, value);
            }
        }

        void SelectItem(TreeNode node, ShellItem value)
        {
            CreateChildren(node);

            foreach (TreeNode child in node.Nodes)
            {
                ShellItem folder = (ShellItem)child.Tag;

                if (folder != null)
                {
                    if (folder == value)
                    {
                        m_TreeView.SelectedNode = child;
                        child.EnsureVisible();
                        child.Expand();
                        return;
                    }
                    else if (folder.IsParentOf(value))
                    {
                        SelectItem(child, value);
                        return;
                    }
                }
            }
        }

        TreeNode FindItem(ShellItem item, TreeNode parent)
        {
            // Patch
            if (item == null) return null;

            if ((ShellItem)parent.Tag == item)
            {
                return parent;
            }

            foreach (TreeNode node in parent.Nodes)
            {
                if ((ShellItem)node.Tag == item)
                {
                    return node;
                }
                else
                {
                    TreeNode found = FindItem(item, node);
                    if (found != null) return found;
                }
            }
            return null;
        }

        bool NodeHasChildren(TreeNode node)
        {
            // FAB 27/11/2023
            return (node.Nodes.Count > 0) && (node.Nodes[0].Tag != null);
            
        }

        void ScrollTreeView(ScrollDirection direction)
        {
            User32.SendMessage(m_TreeView.Handle, MSG.WM_VSCROLL,
                (int)direction, 0);
        }

        void CheckDragScroll(Point location)
        {
            int scrollArea = (int)(m_TreeView.Nodes[0].Bounds.Height * 1.5);
            ScrollDirection scroll = ScrollDirection.None;

            if (location.Y < scrollArea)
            {
                scroll = ScrollDirection.Up;
            }
            else if (location.Y > m_TreeView.ClientRectangle.Height - scrollArea)
            {
                scroll = ScrollDirection.Down;
            }

            if (scroll != ScrollDirection.None)
            {
                if (m_ScrollDirection == ScrollDirection.None)
                {
                    ScrollTreeView(scroll);
                    m_ScrollTimer.Enabled = true;
                }
            }
            else
            {
                m_ScrollTimer.Enabled = false;
            }

            m_ScrollDirection = scroll;
        }

        ShellItem[] ParseShellIDListArray(ComTypes.IDataObject pDataObj)
        {
            List<ShellItem> result = new List<ShellItem>();
            ComTypes.FORMATETC format = new ComTypes.FORMATETC();
            ComTypes.STGMEDIUM medium = new ComTypes.STGMEDIUM();

            format.cfFormat = (short)User32.RegisterClipboardFormat("Shell IDList Array");
            format.dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT;
            format.lindex = 0;
            format.ptd = IntPtr.Zero;
            format.tymed = ComTypes.TYMED.TYMED_HGLOBAL;

            pDataObj.GetData(ref format, out medium);
            Kernel32.GlobalLock(medium.unionmember);

            try
            {
                ShellItem parentFolder = null;
                int count = Marshal.ReadInt32(medium.unionmember);
                int offset = 4;

                for (int n = 0; n <= count; ++n)
                {
                    int pidlOffset = Marshal.ReadInt32(medium.unionmember, offset);
                    int pidlAddress = (int)medium.unionmember + pidlOffset;

                    if (n == 0)
                    {
                        parentFolder = new ShellItem(new IntPtr(pidlAddress));
                    }
                    else
                    {
                        result.Add(new ShellItem(parentFolder, new IntPtr(pidlAddress)));
                    }

                    offset += 4;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(medium.unionmember);
            }

            return result.ToArray();
        }

        bool ShouldSerializeRootFolder()
        {
            return m_RootFolder != ShellItem.Desktop;
        }

        #endregion


        #region Events

        /// <summary>
        /// Gives Windows Explorer style to Treeview
        /// </summary>
        /// <param name="e"></param>
        private void TreeView_HandleCreated(object sender, EventArgs e)
        {
            SetWindowTheme(m_TreeView.Handle, "explorer", null);
            // Increase row height to 18 + 6 = 24
            m_TreeView.ItemHeight += 6;
        }


        void m_TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

            if (!m_Navigating)
            {
                if (m_ShellView != null)
                {
                    m_Navigating = true;
                    try

                    {
                        m_ShellView.CurrentFolder = SelectedFolder;
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\nError ShellTreeView.cs\n" + ex.Message);

                        SelectedFolder = m_ShellView.CurrentFolder;
                    }
                    finally
                    {

                        //Console.Write("\nError ShellTreeView.cs\n");
                        m_Navigating = false;
                    }
                }
                if (m_ShellListView != null)
                {
                    m_Navigating = true;
                    try

                    {
                        m_ShellListView.CurrentFolder = SelectedFolder;
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\nError ShellTreeView.cs\n" + ex.Message);

                        SelectedFolder = m_ShellListView.CurrentFolder;
                    }
                    finally
                    {

                        //Console.Write("\nError ShellTreeView.cs\n");
                        m_Navigating = false;
                    }
                }



            }

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        void m_TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                CreateChildren(e.Node);
            }
            catch (Exception ex)
            {
                Console.Write("\nError ShellTreeView.cs\n" + ex.Message);

                e.Cancel = true;
            }
        }
  
        void m_TreeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                m_RightClickNode = m_TreeView.GetNodeAt(e.Location);
            }
        }

        void m_TreeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = m_TreeView.GetNodeAt(e.Location);

                if ((node != null) && (node == m_RightClickNode))
                {
                    ShellItem folder = (ShellItem)node.Tag;

                    if (folder != null)
                    {
                        try
                        {
                            string ret = new ShellContextMenu(folder).ShowContextMenu(m_TreeView, e.Location);
                            if (ret == "rename")
                            {
                                m_TreeView.SelectedNode.BeginEdit();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Write("\n" + ex.Message);
                        }
                    }
                }
            }
        }

        private void m_TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_TreeView.SelectedNode != null)
            {
                Keys keyCode = e.KeyCode;

                switch (keyCode)
                {
                    case Keys.F2:
                        m_TreeView.SelectedNode.BeginEdit();
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        break;
                    
                    case Keys.F3:
                    case Keys.F4:
                        // FAB: rename all, replace all
                        tvFunctionKeyClicked(this, keyCode); //F3 rename all, F4, replace all (Karaboss function keys)
                        break;
                }
            }
        }

        private void m_TreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {

            TreeNode node = e.Node;
            if (node.Parent == null)
                return;

            ShellItem parentFolder = (ShellItem)node.Parent.Tag;
            ShellItem item = (ShellItem)node.Tag;
            
            string NewName = e.Label;
            IntPtr newPidl = IntPtr.Zero;

            if (NewName != null)
            {                
                try
                {
                    uint res = parentFolder.GetIShellFolder().SetNameOf(m_TreeView.Handle, Shell32.ILFindLastID(item.Pidl), NewName, SHGDN.NORMAL, out newPidl);
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
            }
        }

        void m_ScrollTimer_Tick(object sender, EventArgs e)
        {
            ScrollTreeView(m_ScrollDirection);
        }
             
        void m_ShellView_Navigated(object sender, EventArgs e)
        {
            if (!m_Navigating)
            {
                m_Navigating = true;
                SelectedFolder = m_ShellView.CurrentFolder;
                m_Navigating = false;
            }
        }

        // FAB new control
        void m_ShellListView_Navigated(object sender, EventArgs e)
        {
            if (!m_Navigating)
            {
                m_Navigating = true;
                SelectedFolder = m_ShellListView.CurrentFolder;
                m_Navigating = false;
            }
        }

        #endregion


        #region ShellListener

        void m_ShellListener_ItemRenamed(object sender, ShellItemChangeEventArgs e)
        {
            //TreeNode node;
            if (e.OldItem.IsFolder)
            {
                TreeNode parent = FindItem(e.OldItem.Parent, m_TreeView.Nodes[0]);

                ShellItem CurItem = (ShellItem)m_TreeView.SelectedNode.Tag;
                TreeNode CurNode = FindItem(CurItem, m_TreeView.Nodes[0]);

                //if (parent != null)
                //    RefreshItem(parent);

                // FAB 19/01/19
                if (parent != null && CurNode == parent)
                    RefreshItem(parent);

            }
            
            /*
            node = FindItem(e.OldItem, m_TreeView.Nodes[0]);
            if (node != null)
                RefreshItem(node);
                */                                        
        }

        void m_ShellListener_ItemUpdated(object sender, ShellItemEventArgs e)
        {
            if (e.Item.IsFolder)
            {
                TreeNode parent = FindItem(e.Item.Parent, m_TreeView.Nodes[0]);

                ShellItem CurItem = (ShellItem)m_TreeView.SelectedNode.Tag;
                TreeNode CurNode = FindItem(CurItem, m_TreeView.Nodes[0]);

                //if (parent != null)
                //    RefreshItem(parent);

                // FAB 19/01/19
                if (parent != null && CurNode == parent)
                    RefreshItem(parent);

            }
        }

        #endregion


        #region Drag

        void m_TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = (TreeNode)e.Item;
            ShellItem folder = (ShellItem)node.Tag;
            DragDropEffects effect;

            if (folder != null)
                Ole32.DoDragDrop(folder.GetIDataObject(), this, DragDropEffects.All, out effect);
        }

        #endregion


        #region IDropSource Members

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

        HResult IDropSource.GiveFeedback(DragDropEffects dwEffect)
        {
            return HResult.DRAGDROP_S_USEDEFAULTCURSORS;
        }

        #endregion


        #region IDropTarget Members

        void Interop.IDropTarget.DragEnter(ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            Point clientLocation = m_TreeView.PointToClient(pt);
            TreeNode node = m_TreeView.HitTest(clientLocation).Node;

            DragTarget.Data = pDataObj;
            m_TreeView.HideSelection = true;

            if (node != null)
            {
                m_DragTarget = new DragTarget(node, grfKeyState, pt, ref pdwEffect);
            }
            else
            {
                pdwEffect = 0;
                //return -1;
            }
            //return 0;
        }

        void Interop.IDropTarget.DragOver(MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            Point clientLocation = m_TreeView.PointToClient(pt);
            TreeNode node = m_TreeView.HitTest(clientLocation).Node;

            CheckDragScroll(clientLocation);

            if (node != null)
            {
                if ((m_DragTarget == null) ||
                    (node != m_DragTarget.Node))
                {

                    if (m_DragTarget != null)
                    {
                        m_DragTarget.Dispose();
                    }

                    m_DragTarget = new DragTarget(node, grfKeyState,
                                                  pt, ref pdwEffect);
                }
                else
                {
                    m_DragTarget.DragOver(grfKeyState, pt, ref pdwEffect);
                }
            }
            else
            {
                pdwEffect = 0;
            }
        }

        void Interop.IDropTarget.DragLeave()
        {
            if (m_DragTarget != null)
            {
                m_DragTarget.Dispose();
                m_DragTarget = null;
            }
            m_TreeView.HideSelection = false;
        }


        void Interop.IDropTarget.Drop(ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect)
        {
            if (m_DragTarget != null)
            {
                m_DragTarget.DragDrop(pDataObj, grfKeyState, pt,
                    ref pdwEffect);
                m_DragTarget.Dispose();
                m_DragTarget = null;
            }
        }

        

        #endregion


        #region DragTarget
        class DragTarget : IDisposable
        {
            public DragTarget(TreeNode node, MK keyState, Point pt,  ref DragDropEffects effect)
            {
                m_Node = node;
                m_Node.BackColor = SystemColors.Highlight;
                m_Node.ForeColor = SystemColors.HighlightText;

                m_DragExpandTimer = new Timer();
                m_DragExpandTimer.Interval = 1000;
                m_DragExpandTimer.Tick += new EventHandler(m_DragExpandTimer_Tick);
                m_DragExpandTimer.Start();

                try
                {
                    m_DropTarget = Folder.GetIDropTarget(node.TreeView);
                    m_DropTarget.DragEnter(m_Data, keyState, pt, ref effect);
                }
                catch (Exception ex)
                {
                    Console.Write("\nError ShellTreeView.cs\n" + ex.Message);
                }
            }

            public void Dispose()
            {
                m_Node.BackColor = m_Node.TreeView.BackColor;
                m_Node.ForeColor = m_Node.TreeView.ForeColor;
                m_DragExpandTimer.Dispose();

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

            public void DragDrop(ComTypes.IDataObject data, MK keyState, Point pt, ref DragDropEffects effect)
            {
                m_DropTarget.Drop(data, keyState, pt, ref effect);
            }

            public ShellItem Folder
            {
                get { return (ShellItem)m_Node.Tag; }
            }

            public TreeNode Node
            {
                get { return m_Node; }
            }

            public static ComTypes.IDataObject Data
            {
                get { return m_Data; }
                set { m_Data = value; }
            }

            void m_DragExpandTimer_Tick(object sender, EventArgs e)
            {
                m_Node.Expand();
                m_DragExpandTimer.Stop();
            }

            TreeNode m_Node;
            Interop.IDropTarget m_DropTarget;
            Timer m_DragExpandTimer;
            static ComTypes.IDataObject m_Data;
        }

        #endregion


        TreeView m_TreeView;
        TreeNode m_RightClickNode;
        DragTarget m_DragTarget;
        Timer m_ScrollTimer = new Timer();
        ScrollDirection m_ScrollDirection = ScrollDirection.None;
        ShellItem m_RootFolder = ShellItem.Desktop;
        
        ShellView m_ShellView;
        ShellListView m_ShellListView;          // FAB new control

        ShowHidden m_ShowHidden = ShowHidden.System;
        bool m_Navigating;
        bool m_AllowDrop;
        ShellNotificationListener m_ShellListener = new ShellNotificationListener();

    }

    /// Describes whether hidden files/folders should be displayed in a 
    /// control.
    public enum ShowHidden
    {
        /// <summary>
        /// Hidden files/folders should not be displayed.
        /// </summary>
        False,

        /// <summary>
        /// Hidden files/folders should be displayed.
        /// </summary>
        True,

        /// <summary>
        /// The Windows Explorer "Show hidden files" setting should be used
        /// to determine whether to show hidden files/folders.
        /// </summary>
        System
    }

    // FAB: F3, F4
    public delegate void tvFunctionKeyEventHandler(object sender, Keys keyCode);


}
