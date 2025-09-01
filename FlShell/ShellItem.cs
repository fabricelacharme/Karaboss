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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using FlShell.Interop;
using System.Collections;

namespace FlShell
{
    /// <summary>
    /// Represents an item in the Windows Shell namespace.
    /// </summary>
    [TypeConverter(typeof(ShellItemConverter))]
    public class ShellItem : IEnumerable<ShellItem>
    {
        // Indicates whether DisplayName, TypeName, SortFlag have been set up
        private object m_Tag;
        private int m_SortFlag;       // Used in comparisons
        private bool m_HasDispType;
        private bool m_IsBrowsable = true;
        private string m_TypeName;
        
                                      //  to determine what the equivalent string is
        private static string m_strSystemFolder = string.Empty;

        // My Computer is also commonly used (though not internally),
        //  so save & expose its name on the current machine
        //private static string m_strMyComputer;

        // To get My Documents sorted first, we need to know the Locale 
        // specif (ic name of that folder.
        private static string m_strMyDocuments = string.Empty;


        private int m_IconIndexNormal = -1;        // index into the SystemImageListManager list for Normal icon
        private int m_IconIndexNormalOrig = -1;    // index into the System Image list for Normal icon
        private int m_IconIndexOpenOrig = -1;      // index into the SystemImage list for Open icon
        private int m_IconIndexOpen = -1;          // index into the SystemImageListManager list for Open icon
                                                   //private bool m_HasDispType;                // Indicates whether DisplayName, TypeName, SortFlag have been set up

        // The following elements are only filled in on demand
        private bool m_XtrInfo;
        private DateTime m_LastWriteTime;
        private DateTime m_CreationTime;
        private DateTime m_LastAccessTime;
        private long m_Length;
        private FileAttributes m_Attributes;   // Added 10/09/2011 // true FileAttributes from FileInfo
        private string m_DisplayName = string.Empty;
        private W32Find_Data m_W32Data;


        /// <summary>
        /// Initializes a new instance of the <see cref="ShellItem"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Takes a <see cref="Uri"/> containing the location of the ShellItem. 
        /// This constructor accepts URIs using two schemes:
        /// 
        /// - file: A file or folder in the computer's filesystem, e.g.
        ///         file:///D:/Folder
        /// - shell: A virtual folder, or a file or folder referenced from 
        ///          a virtual folder, e.g. shell:///Personal/file.txt
        /// </remarks>
        /// 
        /// <param name="uri">
        /// A <see cref="Uri"/> containing the location of the ShellItem.
        /// </param>
        public ShellItem(Uri uri)
        {
            Initialize(uri);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellItem"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Takes a <see cref="string"/> containing the location of the ShellItem. 
        /// This constructor accepts URIs using two schemes:
        /// 
        /// - file: A file or folder in the computer's filesystem, e.g.
        ///         file:///D:/Folder
        /// - shell: A virtual folder, or a file or folder referenced from 
        ///          a virtual folder, e.g. shell:///Personal/file.txt
        /// </remarks>
        /// 
        /// <param name="path">
        /// A string containing a Uri with the location of the ShellItem.
        /// </param>
        public ShellItem(string path)
        {
            try
            {
                if ((path.Length > 8 && path.Substring(0, 8) != "file:///") && (path.Length > 9 && path.Substring(0, 9) != "shell:///"))
                    path = "shell:///Desktop";

                if (path == "" || path == "file:///C://")
                    path = "shell:///Desktop";

                /*
                if (path.Contains("file:///C:/Users/Fabrice/Music/Karaoke"))
                {
                    path = "shell:///My Music";
                }
                */

                Initialize(new Uri(path));
            }
            catch (Exception ex)
            {
                Console.Write("\n" + ex.Message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellItem"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Takes an <see cref="Environment.SpecialFolder"/> containing the 
        /// location of the folder.
        /// </remarks>
        /// 
        /// <param name="folder">
        /// An <see cref="Environment.SpecialFolder"/> containing the 
        /// location of the folder.
        /// </param>
        public ShellItem(Environment.SpecialFolder folder)
        {
            IntPtr pidl;

            if (Shell32.SHGetSpecialFolderLocation(IntPtr.Zero,
                (CSIDL)folder, out pidl) == HResult.S_OK)
            {
                try
                {
                    m_ComInterface = CreateItemFromIDList(pidl);
                }
                finally
                {
                    Shell32.ILFree(pidl);
                }
            }
            else
            {
                // SHGetSpecialFolderLocation does not support many common
                // CSIDL values on Windows 98, but SHGetFolderPath in 
                // ShFolder.dll does, so fall back to it if necessary. We
                // try SHGetSpecialFolderLocation first because it returns
                // a PIDL which is preferable to a path as it can express
                // virtual folder locations.
                StringBuilder path = new StringBuilder();
                Marshal.ThrowExceptionForHR((int)Shell32.SHGetFolderPath(
                    IntPtr.Zero, (CSIDL)folder, IntPtr.Zero, 0, path));
                m_ComInterface = CreateItemFromParsingName(path.ToString());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellItem"/> class.
        /// </summary>
        /// 
        /// <remarks>
        /// Creates a ShellItem which is a named child of <paramref name="parent"/>.
        /// </remarks>
        /// 
        /// <param name="parent">
        /// The parent folder of the item.
        /// </param>
        /// 
        /// <param name="name">
        /// The name of the child item.
        /// </param>
        public ShellItem(ShellItem parent, string name)
        {
            if (parent.IsFileSystem)
            {
                // If the parent folder is in the file system, our best 
                // chance of success is to use the FileSystemPath to 
                // create the new item. Folders other than Desktop don't 
                // seem to implement ParseDisplayName properly.
                m_ComInterface = CreateItemFromParsingName(
                    Path.Combine(parent.FileSystemPath, name));
            }
            else
            {
                IShellFolder folder = parent.GetIShellFolder();
                uint eaten;
                IntPtr pidl;
                uint attributes = 0;

                folder.ParseDisplayName(IntPtr.Zero, IntPtr.Zero,
                    name, out eaten, out pidl, ref attributes);

                try
                {
                    m_ComInterface = CreateItemFromIDList(pidl);
                }
                finally
                {
                    Shell32.ILFree(pidl);
                }
            }
        }

        internal ShellItem(IntPtr pidl)
        {
            m_ComInterface = CreateItemFromIDList(pidl);
        }

        internal ShellItem(ShellItem parent, IntPtr pidl)
        {
            m_ComInterface = CreateItemWithParent(parent, pidl);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ShellItem"/> class.
        /// </summary>
        /// 
        /// <param name="comInterface">
        /// An <see cref="IShellItem"/> representing the folder.
        /// </param>
        public ShellItem(IShellItem comInterface)
        {
            m_ComInterface = comInterface;
        }

        /// <summary>
        /// Compares two <see cref="IShellItem"/>s. The comparison is carried
        /// out by display order.
        /// </summary>
        /// 
        /// <param name="item">
        /// The item to compare.
        /// </param>
        /// 
        /// <returns>
        /// 0 if the two items are equal. A negative number if 
        /// <see langword="this"/> is before <paramref name="item"/> in 
        /// display order. A positive number if 
        /// <see langword="this"/> comes after <paramref name="item"/> in 
        /// display order.
        /// </returns>
        public int Compare(ShellItem item)
        {

            return m_ComInterface.Compare(item.ComInterface, SICHINT.DISPLAY);
        }

        /// <summary>
        /// Determines whether two <see cref="ShellItem"/>s refer to
        /// the same shell folder.
        /// </summary>
        /// 
        /// <param name="obj">
        /// The item to compare.
        /// </param>
        /// 
        /// <returns>
        /// <see langword="true"/> if the two objects refer to the same
        /// folder, <see langword="false"/> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {

            if (obj is ShellItem)
            {
                bool result = false;
                ShellItem otherItem = (ShellItem)obj;



                if (IsFileSystem && otherItem.IsFileSystem && (FileSystemPath == otherItem.FileSystemPath) == false)
                    return false;

                if (otherItem.ComInterface is IShellItem)
                {

                    try
                    {
                        //Console.Write("\n" + this.DisplayName);
                        //result = m_ComInterface.Compare(otherItem.ComInterface, SICHINT.DISPLAY) == 0;
                        result = otherItem.ComInterface.Compare(m_ComInterface, SICHINT.DISPLAY) == 0;

                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n Error1 ShellItem.cs - Equals\n" + ex.Message);
                        //Console.Write("\nPidl = " + otherItem.Pidl + "\n");
                        //Console.Write("\nPidl = " + Pidl + "\n");

                    }
                }

                // Sometimes, folders are reported as being unequal even when
                // they refer to the same folder, so double check by comparing
                // the file system paths. (This was showing up on Windows XP in 
                // the SpecialFolders() test)
                if (!result)
                {
                    try
                    {
                        result = IsFileSystem && otherItem.IsFileSystem && (FileSystemPath == otherItem.FileSystemPath);
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n Error2 ShellItem.cs - Equals\n" + ex.Message);
                        return false;
                    }
                }

                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the name of the item in the specified style.
        /// </summary>
        /// 
        /// <param name="sigdn">
        /// The style of display name to return.
        /// </param>
        /// 
        /// <returns>
        /// A string containing the display name of the item.
        /// </returns>
        public string GetDisplayName(SIGDN sigdn)
        {

            try
            {
                IntPtr resultPtr = m_ComInterface.GetDisplayName(sigdn);
                string result = Marshal.PtrToStringUni(resultPtr);
                Marshal.FreeCoTaskMem(resultPtr);              
                return result;
            }
            catch (Exception ex)
            {
                Console.Write("\nError ShellItem.cs\n" + ex.Message);

                return string.Empty;
            }
        }

        /// <summary>
        /// Returns an enumerator detailing the child items of the
        /// <see cref="ShellItem"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This method returns all child item including hidden
        /// items.
        /// </remarks>
        /// 
        /// <returns>
        /// An enumerator over all child items.
        /// </returns>
        public IEnumerator<ShellItem> GetEnumerator()
        {
            return GetEnumerator(SHCONTF.FOLDERS | SHCONTF.INCLUDEHIDDEN |
                SHCONTF.NONFOLDERS);
        }

        /// <summary>
        /// Returns an enumerator detailing the child items of the
        /// <see cref="ShellItem"/>.
        /// </summary>
        /// 
        /// <param name="filter">
        /// A filter describing the types of child items to be included.
        /// </param>
        /// 
        /// <returns>
        /// An enumerator over all child items.
        /// </returns>
        public IEnumerator<ShellItem> GetEnumerator(SHCONTF filter)
        {
            IShellFolder folder = GetIShellFolder();
            IEnumIDList enumId = GetIEnumIDList(folder, filter);
            uint count;
            IntPtr pidl;
            HResult result;

            if (enumId == null)
            {
                yield break;
            }

            result = enumId.Next(1, out pidl, out count);
            while (result == HResult.S_OK)
            {
                yield return new ShellItem(this, pidl);
                Shell32.ILFree(pidl);
                result = enumId.Next(1, out pidl, out count);
            }

            if (result != HResult.S_FALSE)
            {
                Marshal.ThrowExceptionForHR((int)result);
            }

            yield break;
        }

        ///  <summary>Computes the Sort key of this ShellItem, based on its attributes</summary>         
        private int ComputeSortFlag()
        {

            int rVal = 0;
            if (IsDisk)
                rVal = 0x100000;

            if (m_TypeName == strSystemFolder)
            {
                if (!m_IsBrowsable)
                {
                    rVal = rVal | 0x100000;

                    if (m_strMyDocuments == m_DisplayName)
                        rVal = rVal | 0x1;
                }

                else
                    rVal = rVal | 0x1000;
            }


            if (IsFolder)
                rVal = rVal | 0x100;

            return rVal;
        }



        /// <summary>
        /// Returns an enumerator detailing the child items of the
        /// <see cref="ShellItem"/>.
        /// </summary>
        /// 
        /// <remarks>
        /// This method returns all child item including hidden
        /// items.
        /// </remarks>
        /// 
        /// <returns>
        /// An enumerator over all child items.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an <see cref="ComTypes.IDataObject"/> representing the
        /// item. This object is used in drag and drop operations.
        /// </summary>
        public ComTypes.IDataObject GetIDataObject()
        {
            IntPtr result = m_ComInterface.BindToHandler(IntPtr.Zero, BHID.SFUIObject, typeof(ComTypes.IDataObject).GUID);
            return (ComTypes.IDataObject)Marshal.GetTypedObjectForIUnknown(result, typeof(ComTypes.IDataObject));
        }

       

        /// <summary>
        /// Returns an <see cref="IDropTarget"/> representing the
        /// item. This object is used in drag and drop operations.
        /// </summary>
        public IDropTarget GetIDropTarget(System.Windows.Forms.Control control)
        {
            try
            {
                IntPtr result = GetIShellFolder().CreateViewObject(control.Handle, typeof(IDropTarget).GUID);
                return (IDropTarget)Marshal.GetTypedObjectForIUnknown(result, typeof(IDropTarget));
            }
            catch (Exception ex )
            {
                Console.Write("\n" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns an <see cref="IShellFolder"/> representing the
        /// item.
        /// </summary>
        public IShellFolder GetIShellFolder()
        {

            try
            {
                if (m_ComInterface != null)
                {
                    IntPtr result = m_ComInterface.BindToHandler(IntPtr.Zero, BHID.SFObject, typeof(IShellFolder).GUID);
                    return (IShellFolder)Marshal.GetTypedObjectForIUnknown(result, typeof(IShellFolder));
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.Write("\n" + ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Gets the index in the system image list of the icon representing
        /// the item.
        /// </summary>
        /// 
        /// <param name="type">
        /// The type of icon to retrieve.
        /// </param>
        /// 
        /// <param name="flags">
        /// Flags detailing additional information to be conveyed by the icon.
        /// </param>
        /// 
        /// <returns></returns>
        public int GetSystemImageListIndex(ShellIconType type, ShellIconFlags flags)
        {
            SHFILEINFO info = new SHFILEINFO();

            // Patch FAB : Remove SHGFI.ICON
            // https://sourceforge.net/p/gong-shell/patches/2/
            //To eliminate GDI object leaks 
            //(which prevent diplaying folder trees with more than a few thousand subfolders), 
            /*
            IntPtr result = Shell32.SHGetFileInfo(Pidl, 0, out info,
                Marshal.SizeOf(info),
                SHGFI.ICON | SHGFI.SYSICONINDEX | SHGFI.OVERLAYINDEX | SHGFI.PIDL |
                (SHGFI)type | (SHGFI)flags);
            */
            IntPtr result = Shell32.SHGetFileInfo(Pidl, 0, out info,
                Marshal.SizeOf(info),
                SHGFI.SYSICONINDEX | SHGFI.OVERLAYINDEX | SHGFI.PIDL |
                (SHGFI)type | (SHGFI)flags);


            if (result == IntPtr.Zero)
            {
                throw new Exception("Error retreiving shell folder icon");
            }

            return info.iIcon;
        }

        /// <summary>
        /// Tests whether the <see cref="ShellItem"/> is the immediate parent 
        /// of another item.
        /// </summary>
        /// 
        /// <param name="item">
        /// The potential child item.
        /// </param>
        public bool IsImmediateParentOf(ShellItem item)
        {
            return IsFolder && Shell32.ILIsParent(Pidl, item.Pidl, true);
        }

        /// <summary>
        /// Tests whether the <see cref="ShellItem"/> is the parent of 
        /// another item.
        /// </summary>
        /// 
        /// <param name="item">
        /// The potential child item.
        /// </param>
        public bool IsParentOf(ShellItem item)
        {
            return IsFolder && Shell32.ILIsParent(Pidl, item.Pidl, false);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="ShellItem"/>.
        /// </summary>
        public override string ToString()
        {
            return ToUri().ToString();
        }

        /// <summary>
        /// Returns a URI representation of the <see cref="ShellItem"/>.
        /// </summary>
        public Uri ToUri()
        {
            KnownFolderManager manager = new KnownFolderManager();
            StringBuilder path = new StringBuilder("shell:///");
            KnownFolder knownFolder = manager.FindNearestParent(this);

            if (knownFolder != null)
            {
                List<string> folders = new List<string>();
                ShellItem knownFolderItem = knownFolder.CreateShellItem();
                ShellItem item = this;

                while (item != knownFolderItem && item != null)
                {
                    folders.Add(item.GetDisplayName(SIGDN.PARENTRELATIVEPARSING));
                    item = item.Parent;
                }

                folders.Reverse();
                path.Append(knownFolder.Name);
                foreach (string s in folders)
                {
                    path.Append('/');
                    path.Append(s);
                }

                return new Uri(path.ToString());
            }
            else
            {
                // FAB: 16/03/18
                if (FileSystemPath != "")
                    return new Uri(FileSystemPath);
                else
                    return new Uri("shell:///");
            }
        }

        /// <summary>
        /// Gets a child item.
        /// </summary>
        /// 
        /// <param name="name">
        /// The name of the child item.
        /// </param>
        public ShellItem this[string name]
        {
            get
            {
                return new ShellItem(this, name);
            }
        }

        /// <summary>
        /// Tests if two <see cref="ShellItem"/>s refer to the same folder.
        /// </summary>
        /// 
        /// <param name="a">
        /// The first folder.
        /// </param>
        /// 
        /// <param name="b">
        /// The second folder.
        /// </param>
        public static bool operator !=(ShellItem a, ShellItem b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return !object.ReferenceEquals(b, null);
            }
            else
            {
                return !a.Equals(b);
            }
        }

        /// <summary>
        /// Tests if two <see cref="ShellItem"/>s refer to the same folder.
        /// </summary>
        /// 
        /// <param name="a">
        /// The first folder.
        /// </param>
        /// 
        /// <param name="b">
        /// The second folder.
        /// </param>
        public static bool operator ==(ShellItem a, ShellItem b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }
            else
            {
                return a.Equals(b);
            }
        }



        public int CompareTo(object obj)
        {

            if (obj == null)
                return 1; // non-existant is always low

            ShellItem Other = (ShellItem)obj;


            //  UPDATE: Error Handling for ShellItem.CompareTo

            if (Other == null)
            {
#if DEBUG
                throw new ArgumentException("Invalid argument for ShellItem.CompareTo");
#endif
                return 0; //  Ignore this in release builds
            }

            if (!m_HasDispType)
                SetDispType();

            int cmp = Other.SortFlag - m_SortFlag; // Note the reversal

            if (cmp != 0)
                return cmp;
            else
            {
                if (IsDisk)  // implies that both are
                    return string.Compare(this.FileSystemPath, Other.FileSystemPath);
                else
                {
                    //   return String.Compare(m_DisplayName, Other.DisplayName)
                    return StringLogicalComparer.CompareStrings(this.DisplayName, Other.DisplayName);
                }
            }

        }

        private int SortFlag
        {
            get
            {
                if (!m_HasDispType)
                    SetDispType();

                return m_SortFlag;
            }
        }

      

        /// <summary>
        /// Gets the underlying <see cref="IShellItem"/> COM interface.
        /// </summary>
        public IShellItem ComInterface
        {
            get { return m_ComInterface; }
        }

        /// <summary>
        /// Gets the normal display name of the item.
        /// </summary>
        public string DisplayName
        {
            get { return GetDisplayName(SIGDN.NORMALDISPLAY); }
        }

        /// <summary>
        /// Gets the file system path of the item.
        /// </summary>
        public string FileSystemPath
        {
            get
            {
                try
                {
                    if (IsFileSystem)
                        return GetDisplayName(SIGDN.FILESYSPATH);
                    else
                        return "";
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return "";
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item has subfolders.
        /// </summary>
        public bool HasSubFolders
        {
            get { return m_ComInterface.GetAttributes(SFGAO.HASSUBFOLDER) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the item is a file system item.
        /// </summary>
        public bool IsFileSystem
        {
            get {
                if (m_ComInterface != null)
                    return m_ComInterface.GetAttributes(SFGAO.FILESYSTEM) != 0;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is a file system item
        /// or the child of a file system item.
        /// </summary>
        public bool IsFileSystemAncestor
        {
            get { return m_ComInterface.GetAttributes(SFGAO.FILESYSANCESTOR) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the item is a folder.
        /// </summary>
        public bool IsFolder
        {
            get {

                if (m_ComInterface == null)
                    return false;

                try
                {
                    return m_ComInterface.GetAttributes(SFGAO.FOLDER) != 0;
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                    return false;
                }

            }
        }

        // FAB ADD
        /// <summary>
        /// Gets a value indicating wheter the item is a stream
        /// </summary>
        public bool IsStream
        {
            get { return m_ComInterface.GetAttributes(SFGAO.STREAM) != 0; }
        }                
        
        public bool IsLink
        {
            get { return m_ComInterface.GetAttributes(SFGAO.LINK) != 0; }
        }

        public bool IsShared
        {
            get { return m_ComInterface.GetAttributes(SFGAO.SHARE) != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the item is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return m_ComInterface.GetAttributes(SFGAO.READONLY) != 0; }
        }

        // FAB ADD 01/10/2023
        public bool IsHidden
        {
            get { return m_ComInterface.GetAttributes(SFGAO.HIDDEN) !=0;  }
        }

        // FAB ADD
        /// <summary>
        /// Get a value indicating whether the item can be renamed
        /// </summary>
        public bool CanRename
        {
            get { return m_ComInterface.GetAttributes(SFGAO.CANRENAME) != 0; }
        }

        // FAB ADD
        public bool IsDisk
        {
            get { return this.FileSystemPath.Length == 3 && this.FileSystemPath.EndsWith(":\\"); }
        }

        /// <summary>
        /// Gets the item's parent.
        /// </summary>
        public ShellItem Parent
        {
            get
            {
                if (m_ComInterface == null)
                    return null;

                IShellItem item;
                HResult result = m_ComInterface.GetParent(out item);

                if (result == HResult.S_OK)
                {
                    return new ShellItem(item);
                }
                else if (result == HResult.MK_E_NOOBJECT)
                {
                    return null;
                }
                else
                {
                    Marshal.ThrowExceptionForHR((int)result);
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the item's parsing name.
        /// </summary>
        public string ParsingName
        {
            get { return GetDisplayName(SIGDN.DESKTOPABSOLUTEPARSING); }
        }

        /// <summary>
        /// Gets a PIDL representing the item.
        /// </summary>
        public IntPtr Pidl
        {
            get { return GetIDListFromObject(m_ComInterface); }
        }


        #region icon

        /// <summary>
        /// Gets the item's shell icon.
        /// </summary>
        public Icon ShellIcon
        {
            get
            {
                SHFILEINFO info = new SHFILEINFO();
                IntPtr result = Shell32.SHGetFileInfo(Pidl, 0, out info,
                    Marshal.SizeOf(info),
                    SHGFI.ADDOVERLAYS | SHGFI.ICON |
                    SHGFI.SHELLICONSIZE | SHGFI.PIDL);

                if (result == IntPtr.Zero)
                {
                    throw new Exception("Error retreiving shell folder icon");
                }

                try
                {
                    return Icon.FromHandle(info.hIcon);
                }
                catch (Exception ex)
                {
                    Console.Write("ShellItem.cs - get ShellIcon: " + ex.Message);
                    return null;
                }
            }
        }

        public int IconIndexNormalOrig
        {
            get
            {

                if (m_IconIndexNormalOrig < 0)
                {

                    //if (!m_HasDispType)
                    //    SetDispType();

                    SHFILEINFO shfi = new SHFILEINFO();
                    SHGFI dwflag = SHGFI.PIDL | SHGFI.SYSICONINDEX;
                    int dwAttr = 0;

                    if (IsFileSystem && !IsFolder)
                    {
                        dwflag = dwflag | SHGFI.USEFILEATTRIBUTES;
                        dwAttr = (int)SHGFI.FILE_ATTRIBUTE_NORMAL;
                    }


                    int cbFileInfo = Marshal.SizeOf(typeof(SHFILEINFO));

                    
                    IntPtr H = Shell32.SHGetFileInfo(Pidl, dwAttr, out shfi, cbFileInfo, dwflag);

                    m_IconIndexNormalOrig = shfi.iIcon;

                    if (m_IconIndexNormal < 0)
                        m_IconIndexNormal = SystemImageListManager.GetIconIndex(this);

                }

                return m_IconIndexNormalOrig;

            }

        }

        public int IconIndexOpenOrig
        {

            get
            {

                if (m_IconIndexOpenOrig < 0)
                {

                    //if (!m_HasDispType)
                    //    SetDispType();

                    if (!IsDisk && IsFileSystem && IsFolder)
                    {
                        SHGFI dwflag = SHGFI.SYSICONINDEX | SHGFI.PIDL;
                        SHFILEINFO shfi = new SHFILEINFO();


                        int cbFileInfo = Marshal.SizeOf(typeof(SHFILEINFO));

                        IntPtr H = Shell32.SHGetFileInfo(Pidl, 0,
                                   out shfi, cbFileInfo,
                                    dwflag | SHGFI.OPENICON);

                        m_IconIndexOpenOrig = shfi.iIcon;

                        if (m_IconIndexOpen < 0)
                            m_IconIndexOpen = SystemImageListManager.GetIconIndex(this, true);
                        else
                            m_IconIndexOpenOrig = m_IconIndexNormalOrig;

                    }
                }
                return m_IconIndexOpenOrig;
            }
        }

        #endregion

        //private string text;
       
        public string Text { get { return DisplayName; } }


        public object Tag
        {

            get
            {
                return m_Tag;
            }

            set
            { //ByVal value As Object
                m_Tag = value;
            }
        }

        ///  <summary>
        ///  Obtains information available from FileInfo. Uses data from W32Data rather than FileInfo/DirectoryInfo if ( W32Data is present.
        ///  </summary>
        private void FillDemandInfo()
        {
            if (m_W32Data != null && IsFileSystem)
            {  // 04/24/2012 - changed to use m_W32Data rather than .Tag
                W32Find_Data W_32 = m_W32Data;
                m_LastWriteTime = W_32.LastWriteTime;
                m_LastAccessTime = W_32.LastAccesTime;
                m_CreationTime = W_32.CreationTime;
                if (!IsFolder)
                    m_Length = W_32.Length;
                m_Attributes = W_32.Attributes;
                m_W32Data = null;      // have what we need. clear for updates
            }
            else if (IsFileSystem && !IsFolder)
            {
                // in this case, it// s a file
                //this.FileSystemPath
                if (File.Exists(FileSystemPath))
                {
                    FileInfo fi = new FileInfo(FileSystemPath);
                    m_LastWriteTime = fi.LastWriteTime;
                    m_LastAccessTime = fi.LastAccessTime;
                    m_CreationTime = fi.CreationTime;
                    m_Length = fi.Length;
                    m_Attributes = fi.Attributes;          // Added 10/09/2011
                }
            }
            else if (IsFileSystem && IsFolder)
            {
                if (Directory.Exists(FileSystemPath))
                {
                    DirectoryInfo di = new DirectoryInfo(FileSystemPath);
                    m_LastWriteTime = di.LastWriteTime;
                    m_LastAccessTime = di.LastAccessTime;
                    m_CreationTime = di.CreationTime;
                    m_Attributes = di.Attributes;          // Added 10/09/2011
                }
            }
            m_XtrInfo = true;            // 05/15/2012 even if ( there were errors, we have what we can get (long file name problem)
        }

        ///  <summary>
        ///  Contains the LastWriteTime (Last Modif (ied) DateTime of this instance
        ///  </summary>
        ///  <returns>The LastWriteTime (Last Modif (ied) DateTime of this instance</returns>
        ///  <remarks>With other information, Filled by FillDemandInfo on first Get</remarks>
        public DateTime LastWriteTime
        {
            get
            {
                if (!m_XtrInfo)
                    FillDemandInfo();

                return m_LastWriteTime;
            }
        }

        ///  <summary>
        ///  Contains the LastAccessTime DateTime of this instance
        ///  </summary>
        ///  <returns>The LastAccessTime DateTime of this instance</returns>
        ///  <remarks>With other information, Filled by FillDemandInfo on first Get</remarks>
        public DateTime LastAccessTime
        {
            get
            {
                if (!m_XtrInfo)
                    FillDemandInfo();

                return m_LastAccessTime;
            }
        }

        public DateTime CreationTime
        {
            get
            {
                if (!m_XtrInfo)
                    FillDemandInfo();

                return m_CreationTime;
            }
        }


        ///  <summary>
        ///  Contains the FileSize of this instance
        ///  </summary>
        ///  <returns>The FileSize of this instance</returns>
        ///  <remarks>With other information, Filled by FillDemandInfo on first Get</remarks>
        public long Length
        {
            get
            {
                if (!m_XtrInfo)
                    FillDemandInfo();

                return m_Length;
            }
        }

        ///  <summary>
        ///  The Windows TypeName (eg "Text File")
        ///  </summary>
        ///  <returns>The Windows TypeName</returns>
        public string TypeName
        {

            get
            {

                if (!m_HasDispType)
                    SetDispType();

                return m_TypeName;

            }

        }

        ///  <summary>
        ///  Sets DisplayName, TypeName, and SortFlag when actually needed
        ///  </summary>        
        private void SetDispType()
        {

            // Get Displayname, TypeName
            SHFILEINFO shfi = new SHFILEINFO();
            SHGFI dwflag = SHGFI.DISPLAYNAME |
                                       SHGFI.TYPENAME |
                                       SHGFI.PIDL;
            int dwAttr = 0;

            if (IsFileSystem && !IsFolder)
            {
                dwflag = dwflag | SHGFI.USEFILEATTRIBUTES;
                dwAttr = (int)SHGFI.FILE_ATTRIBUTE_NORMAL;
            }

            int cbFileInfo = Marshal.SizeOf(typeof(SHFILEINFO));
            IntPtr H = Shell32.SHGetFileInfo(Pidl, dwAttr, out shfi, cbFileInfo, dwflag);

            m_DisplayName = shfi.szDisplayName;
            m_TypeName = shfi.szTypeName;

            // fix DisplayName
            if (m_DisplayName == "")
                m_DisplayName = FileSystemPath;

            // Fix TypeName
            // if ( m_IsFolder And m_TypeName.Equals("File") Then
            //     m_TypeName = "File Folder"
            // }

            m_SortFlag = ComputeSortFlag();
            m_HasDispType = true;

        }

        public FileAttributes Attributes
        { // Added 10/09/2011
            get
            {
                if (!m_XtrInfo)
                    FillDemandInfo();

                return m_Attributes;
            }
        }

        public static string strSystemFolder
        {
            get
            {
                return m_strSystemFolder;
            }
        }


        /*
        private void SetText(ShellItem item)
        {
            IntPtr strr = Marshal.AllocCoTaskMem(Shell32.MAX_PATH * 2 + 4);
            STRRET mystrret = new STRRET();

            Marshal.WriteInt32(strr, 0, 0);
            StringBuilder buf = new StringBuilder(Shell32.MAX_PATH);

            IntPtr relativePidl = Shell32.ILFindLastID(Pidl);

            if (item.Parent.GetIShellFolder().GetDisplayNameOf(
                            relativePidl,
                            SHGNO.INFOLDER,
                            out mystrret) == 0)
            {
                ShlWapi.StrRetToBuf(ref mystrret, relativePidl, buf, Shell32.MAX_PATH);
                item.text = buf.ToString();
            }

            Marshal.FreeCoTaskMem(strr);
        }
        */

        /// <summary>
        /// Gets the item's tooltip text.
        /// </summary>
        public string ToolTipText
        {
            get
            {
                IntPtr result;
                IQueryInfo queryInfo;
                IntPtr infoTipPtr;
                string infoTip;

                // FAB: Prevent from some strange errors
                if (Parent == null || this.DisplayName == "Documents") 
                    return string.Empty;


                try
                {
                    IntPtr relativePidl = Shell32.ILFindLastID(Pidl);
                    Parent.GetIShellFolder().GetUIObjectOf(IntPtr.Zero, 1, new IntPtr[] { relativePidl }, typeof(IQueryInfo).GUID, 0, out result);
                }
                catch (Exception ex)
                {
                    Console.Write("\nError ShellItem.cs\n" + ex.Message);
                    return string.Empty;
                }

                queryInfo = (IQueryInfo)Marshal.GetTypedObjectForIUnknown(result, typeof(IQueryInfo));
                queryInfo.GetInfoTip(0, out infoTipPtr);
                infoTip = Marshal.PtrToStringUni(infoTipPtr);
                Ole32.CoTaskMemFree(infoTipPtr);
                return infoTip;
            }
        }

        /// <summary>
        /// Gets the Desktop folder.
        /// </summary>
        public static ShellItem Desktop
        {
            get
            {
                if (m_Desktop == null)
                {
                    IShellItem item;
                    IntPtr pidl;

                    Shell32.SHGetSpecialFolderLocation(
                         IntPtr.Zero, (CSIDL)Environment.SpecialFolder.Desktop,
                         out pidl);

                    try
                    {
                        item = CreateItemFromIDList(pidl);
                    }
                    finally
                    {
                        Shell32.ILFree(pidl);
                    }

                    m_Desktop = new ShellItem(item);
                }
                return m_Desktop;
            }
        }



        void Initialize(Uri uri)
        {
            if (uri.Scheme == "file")
            {
                m_ComInterface = CreateItemFromParsingName(uri.LocalPath);
            }
            else if (uri.Scheme == "shell")
            {
                InitializeFromShellUri(uri);
            }
            else
            {
                //throw new InvalidOperationException("Invalid uri scheme");
                Console.Write("\nShellitem.cs - void Initialize - Invalid uri scheme");
                
            }
        }

        void InitializeFromShellUri(Uri uri)
        {
            KnownFolderManager manager = new KnownFolderManager();
            string path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            string knownFolder;
            string restOfPath;
            int separatorIndex = path.IndexOf('/');

            if (separatorIndex != -1)
            {
                knownFolder = path.Substring(0, separatorIndex);
                restOfPath = path.Substring(separatorIndex + 1);
            }
            else
            {
                knownFolder = path;
                restOfPath = string.Empty;
            }

            m_ComInterface = manager.GetFolder(knownFolder).CreateShellItem().ComInterface;

            if (restOfPath != string.Empty)
            {
                m_ComInterface = this[restOfPath.Replace('/', '\\')].ComInterface;
            }
        }

        static IShellItem CreateItemFromIDList(IntPtr pidl)
        {
            try
            {
                return Shell32.SHCreateItemFromIDList(pidl,
                    typeof(IShellItem).GUID);
            }
            catch { return null; }
        }

        static IShellItem CreateItemFromParsingName(string path)
        {
            if (path.Length < 2)
                return null;

            try
            {
                return Shell32.SHCreateItemFromParsingName(path, IntPtr.Zero,
                    typeof(IShellItem).GUID);
            }
            catch (Exception ex)
            {
                Console.Write("\nError ShellItem.cs\n" + ex.Message);

                path = "";
                return Shell32.SHCreateItemFromParsingName(path, IntPtr.Zero,
                    typeof(IShellItem).GUID);

            }


        }

        static IShellItem CreateItemWithParent(ShellItem parent, IntPtr pidl)
        {
            try
            {
                IShellFolder ish = parent.GetIShellFolder();                
                return Shell32.SHCreateItemWithParent(IntPtr.Zero, ish, pidl, typeof(IShellItem).GUID);
            }
            catch (Exception ex)
            {
                Console.Write("\nError ShellItem.cs\n" + ex.Message);
                return null;
            }

        }

        static IntPtr GetIDListFromObject(IShellItem item)
        {
            if (item != null)
                return Shell32.SHGetIDListFromObject(item);
            else
                return IntPtr.Zero;

        }

        static IEnumIDList GetIEnumIDList(IShellFolder folder, SHCONTF flags)
        {
            IEnumIDList result;

            if (folder != null &&  folder.EnumObjects(IntPtr.Zero, flags, out result) == HResult.S_OK)
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        IShellItem m_ComInterface;
        static ShellItem m_Desktop;
    }

    class ShellItemConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
                                            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                                          Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo culture,
                                           object value)
        {
            if (value is string)
            {
                string s = (string)value;

                if (s.Length == 0)
                {
                    return ShellItem.Desktop;
                }
                else
                {
                    return new ShellItem(s);
                }
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }


        public override object ConvertTo(ITypeDescriptorContext context,
                                         CultureInfo culture,
                                         object value,
                                         Type destinationType)
        {
            if (value is ShellItem)
            {
                Uri uri = ((ShellItem)value).ToUri();

                if (destinationType == typeof(string))
                {
                    if (uri.Scheme == "file")
                    {
                        return uri.LocalPath;
                    }
                    else
                    {
                        return uri.ToString();
                    }
                }
                else if (destinationType == typeof(InstanceDescriptor))
                {
                    return new InstanceDescriptor(
                        typeof(ShellItem).GetConstructor(new Type[] { typeof(string) }),
                        new object[] { uri.ToString() });
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Enumerates the types of shell icons.
    /// </summary>
    public enum ShellIconType
    {
        /// <summary>The system large icon type</summary>
        LargeIcon = SHGFI.LARGEICON,

        /// <summary>The system shell icon type</summary>
        ShellIcon = SHGFI.SHELLICONSIZE,

        /// <summary>The system small icon type</summary>
        SmallIcon = SHGFI.SMALLICON,
    }

    /// <summary>
    /// Enumerates the optional styles that can be applied to shell icons.
    /// </summary>
    [Flags]
    public enum ShellIconFlags
    {
        /// <summary>The icon is displayed opened.</summary>
        OpenIcon = SHGFI.OPENICON,

        /// <summary>Get the overlay for the icon as well.</summary>
        OverlayIndex = SHGFI.OVERLAYINDEX
    }
}
