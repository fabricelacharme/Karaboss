using FlShell.Interop;
using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FlShell
{
    public class SystemImageListManager
    {

        #region "       ImageList Related Constants"
        // For ImageList manipulation
        private const int LVM_FIRST = 0x0001000;
        private const int LVM_SETIMAGELIST = (LVM_FIRST + 3);

        private const int LVSIL_NORMAL = 0;
        private const int LVSIL_SMALL = 1;
        private const int LVSIL_STATE = 2;
        private const int LVSIL_GROUPHEADER = 3;

        private const int TV_FIRST = 0x00001100;
        private const int TVM_SETIMAGELIST = (TV_FIRST + 9);

        private const int TVSIL_NORMAL = 0;
        private const int TVSIL_STATE = 2;
        #endregion

        #region "   private Fields"
        private static bool m_Initialized = false;
        private static IntPtr m_smImgList = IntPtr.Zero; //Handle to System Small ImageList
        private static IntPtr m_lgImgList = IntPtr.Zero; //Handle to System Large ImageList
                                                         //UPDATE: Add m_xlgImgList
        private static IntPtr m_xlgImgList = IntPtr.Zero; //Handle to System XtraLarge ImageList
        private static Hashtable m_Table = new Hashtable(128);
        private static IntPtr m_jumboImgList = IntPtr.Zero; //Handle to System Jumbo ImageList
        //private Shared m_Mutex As New Mutex()
        private static object SILMLock = new object();


        [DllImport("comctl32.dll")]
        private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, ILD flags);

        [DllImport("comctl32.dll")]
        public static extern int ImageList_ReplaceIcon(IntPtr hImageList, int IconIndex, IntPtr hIcon);

        [DllImport("comctl32.dll")]
        public static extern int ImageList_GetImageCount(IntPtr hImageList);

        public enum LVSIL
        {
            Normal = 0,
            Small = 1,
            State = 2,
            GroupHeader = 3,
        }

        public enum SHIL
        {
            Small = 1,
            Large = 0,
            XLarge = 2,
            Jumbo = 4,
        }

        private static int cbFileInfo = Marshal.SizeOf(typeof(SHFILEINFO));
        
        #endregion

        #region "   New"
        /// <summary>
        /// Summary of Initializer.
        /// </summary       
        private static void Initializer()
        {
            if (m_Initialized)
                return;


            int dwFlag = (int)(SHGFI.USEFILEATTRIBUTES | SHGFI.SYSICONINDEX | SHGFI.SMALLICON);
            SHFILEINFO shfi = new SHFILEINFO();
            m_smImgList = Shell32.SHGetFileInfo(".txt",
                               (int)SHGFI.FILE_ATTRIBUTE_NORMAL,
                               ref shfi,
                               cbFileInfo,
                               dwFlag);
            
            //Debug.Assert((Not m_smImgList.Equals(IntPtr.Zero)), "Failed to create Image Small ImageList")
            if (m_smImgList.Equals(IntPtr.Zero))
                throw new Exception("Failed to create Small ImageList");


            dwFlag = (int)(SHGFI.USEFILEATTRIBUTES | SHGFI.SYSICONINDEX | SHGFI.LARGEICON);
            m_lgImgList = Shell32.SHGetFileInfo(".txt",
                               (int)SHGFI.FILE_ATTRIBUTE_NORMAL,
                               ref shfi,
                               cbFileInfo,
                               dwFlag);
            
            //Debug.Assert((Not m_lgImgList.Equals(IntPtr.Zero)), "Failed to create Image Small ImageList")
            if (m_lgImgList.Equals(IntPtr.Zero))
                throw new Exception("Failed to create Large ImageList");

            
            //UPDATE: Get the System IImageList object from the Shell for XLarge Icons:
            Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            Shell32.SHGetImageListHandle(2, ref iidImageList, ref m_xlgImgList);
            //Debug.Assert((Not m_xlgImgList.Equals(IntPtr.Zero)), "Failed to create Image XLarge ImageList")
            if (m_xlgImgList.Equals(IntPtr.Zero))
                throw new Exception("Failed to create XLarge ImageList");

            Shell32.SHGetImageListHandle(4, ref iidImageList, ref m_jumboImgList);
            //Debug.Assert((Not m_jumboImgList.Equals(IntPtr.Zero)), "Failed to create Image Jumbo ImageList")
            if (m_jumboImgList.Equals(IntPtr.Zero))
                throw new Exception("Failed to create Jumbo ImageList");
           
            m_Initialized = true;
            //Call here; SHGetIconOverlayIndex requies that the System ImageList is initialized...
            GetOverlayIndices();
        }
        #endregion

        #region "   public Properties"
        /// <summary>
        /// The Handle (as IntPtr) of the per process System Image List containing Small Icons.
        /// </summary>
        public static IntPtr hSmallImageList
        {
            get
            {
                return m_smImgList;
            }
        }

        /// <summary>
        /// The Handle (as IntPtr) of the per process System Image List containing Large Icons.
        /// </summary>
        public static IntPtr hLargeImageList
        {
            get
            {
                return m_lgImgList;
            }
        }

        /// <summary>
        /// The Handle (as IntPtr) of the per process System Image List containing Extra Large Icons.
        /// </summary>
        public static IntPtr hXLargeImageList
        {
            get
            {
                return m_xlgImgList;
            }
        }

        /// <summary>
        /// The Handle (as IntPtr) of the per process System Image List containing Jumbo Icons.
        /// </summary>
        public static IntPtr hJumboImageList
        {
            get
            {
                return m_jumboImgList;
            }
        }
        #endregion

        #region "   public Methods"
        #region "       GetIconIndex"
        private static int mCnt;
        private static int bCnt;
        /// <summary>
        /// Location of the SHIL's overlay icons.
        /// </summary>
        /// <remarks>http://msdn.microsoft.com/en-us/library/windows/desktop/bb762183(v=vs.85).aspx </remarks>
        public static int ovlShare;
        public static int ovlLink;
        public static int ovlSlow;
        public static int ovlDefault;

        private static void GetOverlayIndices()
        {
            ovlLink = SHGetIconOverlayIndex(null, (int)IDO_SHGIOI.IDO_SHGIOI_LINK);
            ovlShare = SHGetIconOverlayIndex(null, (int)IDO_SHGIOI.IDO_SHGIOI_SHARE);
            ovlSlow = SHGetIconOverlayIndex(null, (int)IDO_SHGIOI.IDO_SHGIOI_SLOWFILE);
            ovlDefault = SHGetIconOverlayIndex(null, (int)IDO_SHGIOI.IDO_SHGIOI_DEFAULT);
        }

        /// <summary>
        /// Queries the internal Hashtable of IConIndexes and returns the IconIndex for the requested ShellItem.
        /// </summary>
        /// <param name="item">The ShellItem for which the IconIndex is requested</param>
        /// <param name="GetOpenIcon">true if the "open" IconIndex is requested</param>
        /// <param name="GetSelectedIcon">true if the "Selected" Icon is requested</param>
        /// <returns>The true IConIndex into the per process ImageList for the ShellItem given as a parameter</returns>
        public static int GetIconIndex(ShellItem item, bool GetOpenIcon = false, bool GetSelectedIcon = false)
        {

            Initializer();
            bool HasOverlay = false;  //true if it's an overlay
            int rVal;     //The returned Index

            SHGFI dwflag = SHGFI.SYSICONINDEX | SHGFI.PIDL | SHGFI.ICON;
            int dwAttr = 0;
            //build Key into HashTable for this Item
            int Key = (int)(!GetOpenIcon ? item.IconIndexNormalOrig * 256 : item.IconIndexOpenOrig * 256);
            //With item
            if (item.IsLink)
            {
                Key = Key | 1;
                dwflag = dwflag | SHGFI.LINKOVERLAY;
                HasOverlay = true;
            }
            if (item.IsShared)
            {
                Key = Key | 2;
                dwflag = dwflag | SHGFI.ADDOVERLAYS;
                HasOverlay = true;
            }
            if (GetSelectedIcon)
            {
                Key = Key | 4;
                dwflag = dwflag | SHGFI.SELECTED;
                HasOverlay = true;   //not really an overlay, but handled the same
            }
            if (m_Table.ContainsKey(Key))
            {
                rVal = (int)m_Table[Key];
                mCnt += 1;
            }
            else if (!HasOverlay)  //for non-overlay icons, we already have
            {
                rVal = Key / 256;        //  the right index -- put in table
                m_Table[Key] = rVal;
                bCnt += 1;
            }
            else        //don't have iconindex for an overlay, get it.
            {
                //This is the tricky part -- add overlaid Icon to systemimagelist
                //  use of SmallImageList from Calum McLellan
                SHFILEINFO shfi = new SHFILEINFO();
                SHFILEINFO shfi_small = new SHFILEINFO();
                IntPtr HR;
                IntPtr HR_SMALL;
                if (item.IsFileSystem && !item.IsDisk && !item.IsFolder)
                {
                    dwflag = dwflag | SHGFI.USEFILEATTRIBUTES;
                    dwAttr = (int)SHGFI.FILE_ATTRIBUTE_NORMAL;
                }
                //UPDATE: OpenIcon with overlay
                if (GetOpenIcon)
                    dwflag = dwflag | SHGFI.OPENICON;

                HR = Shell32.SHGetFileInfo(item.Pidl, dwAttr, out shfi, cbFileInfo, dwflag);
                HR_SMALL = Shell32.SHGetFileInfo(item.Pidl, dwAttr, out shfi_small, cbFileInfo, dwflag | SHGFI.SMALLICON);
                //m_Mutex.WaitOne()
                int rVal2;
                lock (SILMLock)
                {

                    rVal = ImageList_ReplaceIcon(m_smImgList, -1, shfi_small.hIcon);
                    //Debug.Assert(rVal > -1, "Failed to add overlaid small icon");

                    rVal2 = ImageList_ReplaceIcon(m_lgImgList, -1, shfi.hIcon);
                

                  

                    // Jens' version

                    if (m_xlgImgList != IntPtr.Zero)  //Not set on Windows earlier than XP
                    {
                        ILD flags = ILD.NORMAL; //= 0    //5/9/2013 - JDP
                        if (item.IsLink)
                            flags = flags | (ILD)INDEXTOOVERLAYMASK(ovlLink);
                        if (item.IsShared)
                            flags = flags | (ILD)INDEXTOOVERLAYMASK(ovlShare);
                        IntPtr hIcon = IntPtr.Zero;
                        rVal = GetNonOverlayIndex(ref item, GetOpenIcon);
                        hIcon = ImageList_GetIcon(m_xlgImgList, rVal, flags);
                        rVal = ImageList_ReplaceIcon(m_xlgImgList, -1, hIcon);
                        int rCnt = ImageList_GetImageCount(m_jumboImgList);
                        //Debug.Assert(rVal > -1, "Failed to add overlaid xl icon");
                        User32.DestroyIcon(hIcon);
                        //Debug.Assert(rVal == rVal2, "XL & Large Icon Indices are Different");
                    }

                    // This fails at rVal = ImageList_ReplaceIcon (incomplete implementation of interface??)
                    if (m_jumboImgList != IntPtr.Zero)  //Not set on Windows earlier than XP
                    {
                        IntPtr hIcon = IntPtr.Zero;
                        rVal = GetNonOverlayIndex(ref item, GetOpenIcon);

                        ILD flags = ILD.NORMAL;
                        if (item.IsLink)
                            flags = flags | (ILD)INDEXTOOVERLAYMASK(ovlLink);

                        if (item.IsShared)
                            flags = flags | (ILD)INDEXTOOVERLAYMASK(ovlShare);

                        hIcon = ImageList_GetIcon(m_jumboImgList, rVal, flags);
                        rVal = ImageList_ReplaceIcon(m_jumboImgList, -1, hIcon);
                        if (rVal < 0)        //5/11/2013 - JDP
                            rVal = rVal2;        //5/11/2013 - JDP
                                                 //5/11/2013 - JDP
                                                 //Debug.Assert(rVal > -1, "Failed to add overlaid Jumbo icon");
                        User32.DestroyIcon(hIcon);
                        //Debug.Assert(rVal == rVal2, "Jumbo & Large Icon Indices are Different");

                    }


                }
                //m_Mutex.ReleaseMutex()
                User32.DestroyIcon(shfi.hIcon);
                User32.DestroyIcon(shfi_small.hIcon);
                if (rVal < 0 || rVal != rVal2)
                {
                    //  FAB
                    //throw new ApplicationException("Failed to add Icon for " + item.DisplayName);
                    //MessageBox.Show("Failed to add Icon for " + item.DisplayName);
                    Console.Write("\nFailed to add Icon for " + item.DisplayName);
                }

                m_Table[Key] = rVal;

            }
            //End With
            return rVal;
        }

        //UPDATE: Add GetNonOverlayIndex
        //returns the normal non-overlay Icon for XL overlay Icons
        public static int GetNonOverlayIndex(ref ShellItem item, bool GetOpenIcon = false)
        {

            Initializer();
            int rVal;     //The returned Index

            //build Key into HashTable for this Item
            int Key = (int)(!GetOpenIcon ? item.IconIndexNormalOrig * 256 : item.IconIndexOpenOrig * 256);

            if (m_Table.ContainsKey(Key))
            {
                rVal = (int)m_Table[Key];
                mCnt += 1;
            }
            else                        //  for non-overlay icons, we already have
            {
                rVal = Key / 256;        //  the right index -- put in table
                m_Table[Key] = rVal;
                bCnt += 1;
            }
            return rVal;
        }


        /// <summary>
        /// returns the index of the overlay icon in the system image list.
        /// OBS! The System ImageList must be instantiated for this method to work!
        /// </summary>
        /// <param name="pszIconPath">A pointer to a null-terminated string of maximum length MAX_PATH that contains the fully qualified path of the file that contains the icon, or null to retrieve one of then standard overlay icons.</param>
        /// <param name="iIconIndex">The icon's index in the file pointed to by pszIconPath.To request a standard overlay icon, set pszIconPath to NULL, and iIconIndex to one of the <seealso cref = "SystemImageListManager.IDO_SHGIOI " /> flags.</ param >
        /// <returns>returns the index of the overlay icon in the system image list if successful, or -1 otherwise.</returns>
        /// <remarks>Icon overlays are part of the system image list. They have two identifiers. The first is a one-based overlay index that identifies the overlay relative to other overlays in the image list. The other is an image index that identifies the actual image. These two indexes are equivalent to the values that you assign to the iOverlay and iImage parameters, respectively, when you add an icon overlay to a private image list with ImageList_SetOverlayImage. SHGetIconOverlayIndex returns the overlay index. To convert an overlay index to its equivalent image index, call <seealso  cref= "INDEXTOOVERLAYMASK " />. 
        /// Note: After the image has been loaded into the system image list during initialization, it cannot be changed. The file name and index specified by pszIconPath and iIconIndex are used only to identify the icon overlay. SHGetIconOverlayIndex cannot be used to modify the system image list.
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/bb762183(v=vs.85).aspx </remarks>

        [DllImport("shell32.dll")]
        static extern int SHGetIconOverlayIndex(string pszIconPath, int iIconIndex);

        //[System.Runtime.InteropServices.DllImportAttribute("Shell32.dll", EntryPoint = "SHGetIconOverlayIndex")]
        //public int SHGetIconOverlayIndex(<System.Runtime.InteropServices.InAttribute(), System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.LPTStr)> string pszIconPath, int iIconIndex) ;


        //private Shared Function INDEXTOOVERLAYMASK(ByVal i As Integer) As Integer
        /// <summary>
        /// Mockup of Shell Macros.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks>Prepares the index of an overlay mask so that ImageList_GetIcon and ImageList_Draw can use it. </remarks>
        public static int INDEXTOOVERLAYMASK(int i)
        {
            return ((i) << 8);
        }
        public static int INDEXTOSTATEIMAGEMASK(int i)
        {
            return ((i) << 12);
        }

        /// <summary>
        /// Used by <see cref="SHGetIconOverlayIndex "/> to request a standard overlay icon: 
        /// Set pszIconPath to NULL, and iIconIndex to one of the following values:
        /// </summary>
        /// <remarks></remarks>
        public enum IDO_SHGIOI
        {
            IDO_SHGIOI_SHARE = 0x0FFFFFFF,
            IDO_SHGIOI_LINK = 0x0FFFFFFE,
            IDO_SHGIOI_SLOWFILE = 0x0FFFFFFD,
            IDO_SHGIOI_DEFAULT = 0x0FFFFFFC,
        }


        //private Shared Sub DebugShowImages(ByVal useSmall As Boolean, ByVal iFrom As Integer, ByVal iTo As Integer)
        //    Dim RightIcon As Icon = GetIcon(iFrom, Not useSmall)
        //    Dim rightIndex As Integer = iFrom
        //    Do While iFrom <= iTo
        //        Dim ico As Icon = GetIcon(iFrom, useSmall)
        //        Dim fShow As New frmDebugShowImage(rightIndex, RightIcon, ico, IIf(useSmall, "Small ImageList", "Large ImageList"), iFrom)
        //        fShow.ShowDialog()
        //        fShow.Dispose()
        //        iFrom += 1
        //    Loop
        //}
        #endregion

        #region "       GetIcon"
        /// <summary>
        /// returns a GDI+ copy of a Large or Small icon from the ImageList
        /// at the specified index.</summary>
        /// <param name="Index">The IconIndex of the desired Icon</param>
        /// <param name="smallIcon">Optional, default = false. If true, return the
        ///   icon from the Small ImageList rather than the Large.</param>
        /// <returns>The specified Icon or null</returns>
        public static Icon GetIcon(int Index, bool smallIcon = false)
        {

            Initializer();
            Icon icon = null;
            IntPtr hIcon = IntPtr.Zero;
            //Customisation to return a small image
            if (smallIcon)
                hIcon = ImageList_GetIcon(m_smImgList, Index, 0);
            else
                hIcon = ImageList_GetIcon(m_lgImgList, Index, 0);

            if (hIcon != null)
                icon = System.Drawing.Icon.FromHandle(hIcon);

            return icon;
        }

        /// <summary>
        ///returns a GDI+ copy of an Extra Large Icon from the ImageList 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The desired Icon or null</returns>
        /// <remarks></remarks>
        public static Icon GetXLIcon(int index)
        {
            Initializer();
            Icon icon = null;
            if (m_xlgImgList != IntPtr.Zero)
            {
                IntPtr hIcon = IntPtr.Zero;
                hIcon = ImageList_GetIcon(m_xlgImgList, index, 0);
                if (hIcon != null)
                    icon = System.Drawing.Icon.FromHandle(hIcon);

            }
            return icon;
        }

        /// <summary>
        ///returns a GDI+ copy of an Jumbo Icon from the ImageList 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The desired Icon or null</returns>
        /// <remarks></remarks>
        public static Icon GetJumboIcon(int index)
        {
            Initializer();
            Icon icon = null;
            if (m_jumboImgList != IntPtr.Zero)
            {
                IntPtr hIcon = IntPtr.Zero;
                hIcon = ImageList_GetIcon(m_jumboImgList, index, 0);
                if (hIcon != null)
                    icon = System.Drawing.Icon.FromHandle(hIcon);

            }
            return icon;
        }

        #endregion

        #region "       SetListViewImageList"
        ///   <summary>
        ///    Associates a SysImageList with a ListView control
        ///    </summary>
        ///    <param name="listView">ListView control to associate ImageList with</param>
        ///    <param name="forLargeIcons">true=Set Large Icon List
        ///                   false=Set Small Icon List</param>
        ///    <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetListViewImageList(ListView listView, bool forLargeIcons, bool forStateImages)
        {

            Initializer();
            int wParam = LVSIL_NORMAL;
            IntPtr HImageList = m_lgImgList;
            if (!forLargeIcons)
            {
                wParam = LVSIL_SMALL;
                HImageList = m_smImgList;
            }
            if (forStateImages)
                wParam = LVSIL_STATE;

            User32.SendMessage(listView.Handle, MSG.LVM_SETIMAGELIST, wParam, HImageList);
        }

        ///   <summary>
        ///    Associates a SysImageList with a ListView control
        ///    </summary>
        ///    <param name="listView">ListView control to associate ImageList with</param>
        ///    <param name="Usage">State, Group, Normal, Small
        ///                   false=Set Small Icon List</param>
        ///    <param name="IIlSize">Size of Images</param>
        public static void SetListViewImageList(ListView listView, LVSIL Usage, SHIL IIlSize)
        {

            Initializer();
            int wParam = (int)Usage;
            IntPtr HImageList = m_lgImgList;
            if (IIlSize == SHIL.Small)
                HImageList = m_smImgList;
            else if (IIlSize == SHIL.Jumbo)
                HImageList = m_jumboImgList;
            else if (IIlSize == SHIL.XLarge)
                HImageList = m_xlgImgList;

            User32.SendMessage(listView.Handle, MSG.LVM_SETIMAGELIST, wParam, HImageList);
        }
        #endregion

        #region "       SetTreeViewImageList"
        /// <summary>
        /// Associates a SysImageList with a TreeView control
        /// </summary>
        /// <param name="treeView">TreeView control to associate the ImageList with</param>
        /// <param name="forStateImages">Whether to add ImageList as StateImageList</param>
        public static void SetTreeViewImageList(TreeView treeView, bool forStateImages)
        {

            Initializer();
            int wParam = LVSIL_NORMAL;
            if (forStateImages)
                wParam = LVSIL_STATE;

            //Dim HR As Integer                      //12/31/2013
            //HR = SendMessage(treeView.Handle, _    //12/31/2013
            User32.SendMessage(treeView.Handle, MSG.TVM_SETIMAGELIST, wParam, m_smImgList);
        }

        #endregion

        #endregion
    }
}
