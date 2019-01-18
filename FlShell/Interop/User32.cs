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
using System.Drawing;
using System.Runtime.InteropServices;

#pragma warning disable 1591

namespace FlShell.Interop
{
    [Flags]
    public enum LVIF
    {
        LVIF_TEXT = 0x0001,
        LVIF_IMAGE = 0x0002,
        LVIF_PARAM = 0x0004,
        LVIF_STATE = 0x0008,
        LVIF_INDENT = 0x0010,
        LVIF_GROUPID = 0x0100,
        LVIF_COLUMNS = 0x0200,
        LVIF_NORECOMPUTE = 0x0800,
        LVIF_DI_SETITEM = 0x1000,
        LVIF_COLFMT = 0x00010000,
    }

    [Flags]
    public enum LVIS
    {
        LVIS_FOCUSED = 0x0001,
        LVIS_SELECTED = 0x0002,
        LVIS_CUT = 0x0004,
        LVIS_DROPHILITED = 0x0008,
        LVIS_ACTIVATING = 0x0020,
        LVIS_OVERLAYMASK = 0x0F00,
        LVIS_STATEIMAGEMASK = 0xF000,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct LVITEMA
    {
        public LVIF mask;
        public int iItem;
        public int iSubItem;
        public LVIS state;
        public LVIS stateMask;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pszText;
        public int cchTextMax;
        public int iImage;
        public int lParam;
    }

    public enum LVSIL
    {
        LVSIL_NORMAL = 0,
        LVSIL_SMALL = 1,
        LVSIL_STATE = 2,
    }

    public enum CMIC
    {
        HOTKEY = 0x00000020,
        ICON = 0x00000010,
        FLAG_NO_UI = 0x00000400,
        UNICODE = 0x00004000,
        NO_CONSOLE = 0x00008000,
        ASYNCOK = 0x00100000,
        NOZONECHECKS = 0x00800000,
        SHIFT_DOWN = 0x10000000,
        CONTROL_DOWN = 0x40000000,
        FLAG_LOG_USAGE = 0x04000000,
        PTINVOKE = 0x20000000,
    }


    public enum SW
    {
        HIDE = 0,
        SHOWNORMAL = 1,
        SHOW = 5,
        SHOWDEFAULT = 10,
        SHOWMAXIMIZED = 3,
        SHOWMINIMIZED = 2,
        SHOWMINNOACTIVE = 7,
        SHOWNOACTIVATE = 4,
    }


  

    [StructLayout(LayoutKind.Sequential)]
    public struct MENUINFO
    {
        public int cbSize;
        public MIM fMask;
        public int dwStyle;
        public int cyMax;
        public IntPtr hbrBack;
        public int dwContextHelpID;
        public int dwMenuData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        public static MENUITEMINFO New(string text)
        {

            MENUITEMINFO ret = new MENUITEMINFO();
            ret.cbSize = sizeOf;
            ret.dwTypeData = text;
            ret.cch = (uint)text.Length;
            ret.fMask = 0;
            ret.fType = 0;
            ret.fState = 0;
            ret.wID = 0;
            ret.hSubMenu = IntPtr.Zero;
            ret.hbmpChecked = IntPtr.Zero;
            ret.hbmpUnchecked = IntPtr.Zero;
            ret.dwItemData = IntPtr.Zero;
            ret.hbmpItem = IntPtr.Zero;
            return ret;

        }

        public int cbSize;
        public MIIM fMask;
        public uint fType;
        public uint fState;
        public int wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        //public int hbmpChecked;
        //public int hbmpUnchecked;
        //public int dwItemData;
        public string dwTypeData;
        public uint cch;
        //public int hbmpItem;
        public IntPtr hbmpItem;

        // return the size of the structure
        public static int sizeOf
        {
            get { return Marshal.SizeOf(typeof(MENUITEMINFO)); }
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CMInvokeCommandInfoEx
    {
        public int cbSize;
        public int fMask;
        public IntPtr hwnd;
        public IntPtr lpVerb;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpParameters;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDirectory;

        public int nShow;
        public int dwHotKey;
        public IntPtr hIcon;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpTitle;
        public IntPtr lpVerbW;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpParametersW;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDirectoryW;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpTitleW;

        public System.Drawing.Point ptInvoke;
    }


    public enum FCIDM : uint
    {
        FCIDM_GLOBALFIRST = 0x8000,
        FCIDM_GLOBALLAST = 0x9fff,
        FCIDM_MENU_FILE	 = FCIDM_GLOBALFIRST,
    }

    public enum MF
    {
        MF_BYCOMMAND = 0x00000000,
        MF_BYPOSITION = 0x00000400,
    }

    /*
    [Flags]
    public enum MenuFlags : uint
    {
        MF_STRING = 0,
        MF_BYPOSITION = 0x400,
        MF_SEPARATOR = 0x800,
        MF_REMOVE = 0x1000,
    }
    */

    public enum MFT : uint
    {
        MFT_STRING = 0x00000000,
        MFT_GRAYED = 0x00000003,
        MFT_DISABLED = 0x00000003,
        MFT_CHECKED = 0x00000008,
        MFT_SEPARATOR = 0x00000800,
        MFT_RADIOCHECK = 0x00000200,
        MFT_BITMAP = 0x00000004,
        MFT_OWNERDRAW = 0x00000100,
        MFT_MENUBARBREAK = 0x00000020,
        MFT_MENUBREAK = 0x00000040,
        MFT_RIGHTORDER = 0x00002000,
        MFT_BYCOMMAND = 0x00000000,
        MFT_BYPOSITION = 0x00000400,
        MFT_POPUP = 0x00000010,
        MFT_REMOVE = 0x1000,
    }


  

    public enum MIIM : uint
    {
        MIIM_STATE = 0x00000001,
        MIIM_ID = 0x00000002,
        MIIM_SUBMENU = 0x00000004,
        MIIM_CHECKMARKS = 0x00000008,
        MIIM_TYPE = 0x00000010,
        MIIM_DATA = 0x00000020,
        MIIM_STRING = 0x00000040,
        MIIM_BITMAP = 0x00000080,
        MIIM_FTYPE = 0x00000100,
    }

  

    
    public enum FileAttribute
    {
        //
        // Summary:
        //     Normal (default for Dir and SetAttr). No special characteristics apply to this
        //     file. This member is equivalent to the Visual Basic constant vbNormal.
        Normal = 0,
        //
        // Summary:
        //     Read only. This member is equivalent to the Visual Basic constant vbReadOnly.
        ReadOnly = 1,
        //
        // Summary:
        //     Hidden. This member is equivalent to the Visual Basic constant vbHidden.
        Hidden = 2,
        //
        // Summary:
        //     System file. This member is equivalent to the Visual Basic constant vbSystem.
        System = 4,
        //
        // Summary:
        //     Volume label. This attribute is not valid when used with SetAttr. This member
        //     is equivalent to the Visual Basic constant vbVolume.
        Volume = 8,
        //
        // Summary:
        //     Directory or folder. This member is equivalent to the Visual Basic constant vbDirectory.
        Directory = 16,
        //
        // Summary:
        //     File has changed since last backup. This member is equivalent to the Visual Basic
        //     constant vbArchive.
        Archive = 32,
    }


    public enum MIM : uint
    {
        MIM_MAXHEIGHT = 0x00000001,
        MIM_BACKGROUND = 0x00000002,
        MIM_HELPID = 0x00000004,
        MIM_MENUDATA = 0x00000008,
        MIM_STYLE = 0x00000010,
        MIM_APPLYTOSUBMENUS = 0x80000000,
    }

    public enum CMD
    {
        TILES = 100001,
        LARGEICON = 100002,
        LIST = 100003,
        DETAILS = 100004,
        THUMBNAILS = 100005,
        REFRESH = 100006,
        PASTE = 100007,
        PASTELINK = 100008,
        PROPERTIES = 100009,
        ARRANGEICONS = 100010,
    }


    

    [Flags]
    public enum CLSCTX
    {
        // Fields
        ALL = 23,
        DISABLE_AAA = 32768,
        ENABLE_AAA = 65536,
        ENABLE_CODE_DOWNLOAD = 8192,
        FROM_DEFAULT_CONTEXT = 131072,
        INPROC = 3,
        INPROC_HANDLER = 2,
        INPROC_HANDLER16 = 32,
        INPROC_SERVER = 1,
        INPROC_SERVER16 = 8,
        LOCAL_SERVER = 4,
        NO_CODE_DOWNLOAD = 1024,
        NO_CUSTOM_MARSHAL = 4096,
        NO_FAILURE_LOG = 16384,
        REMOTE_SERVER = 16,
        RESERVED1 = 64,
        RESERVED2 = 128,
        RESERVED3 = 256,
        RESERVED4 = 512,
        RESERVED5 = 2048,
        SERVER = 21,
    }



    public enum GCS
    {
        VERBA = 0,
        HELPTEXTA = 1,
        VALIDATEA = 2,
        VERBW = 4,
        HELPTEXTW = 5,
        VALIDATEW = 6,
    }


    public enum SHCNF
    {
        IDLIST = 0x00000000,
        FLUSH = 0x00001000,
    }

    

    public enum MK
    {
        MK_LBUTTON = 0x0001,
        MK_RBUTTON = 0x0002,
        MK_SHIFT = 0x0004,
        MK_CONTROL = 0x0008,
        MK_MBUTTON = 0x0010,
        MK_ALT = 0x1000,
    }


    public enum MSG
    {
        WM_COMMAND = 0x0111,
        WM_VSCROLL = 0x0115,
        EM_UNDO = 0x00C7,
        WM_CUT = 0x0300,
        WM_COPY = 0x0301,
        WM_PASTE = 0x0302,
        WM_CONTEXTMENU = 0x007B,
        WM_SETTEXT = 0x000C,
        LVM_FIRST = 0x1000,
        LVM_SETIMAGELIST = 0x1003,
        LVM_GETITEMCOUNT = 0x1004,
        LVM_GETITEMA = 0x1005,
        LVM_EDITLABEL = 0x1017,
        LVM_GETCOLUMNWIDTH = LVM_FIRST + 29,
        LVM_SETCOLUMNWIDTH = LVM_FIRST + 30,
        LVM_GETEDITCONTROL = LVM_FIRST + 24,
        LVM_SETTEXTBKCOLOR = LVM_FIRST + 38,
        LVM_SETITEMSTATE = LVM_FIRST + 43,
        LVM_SETITEMPOSITION32 = LVM_FIRST + 49,
        LVM_SETICONSPACING = LVM_FIRST + 53,
        LVM_SETBKIMAGE = LVM_FIRST + 68,
        LVM_SETSELECTEDCOLUMN = LVM_FIRST + 140,
        LVM_INSERTGROUP = LVM_FIRST + 145,
        LVM_ENABLEGROUPVIEW = LVM_FIRST + 157,                
        LVM_GETHEADER = 4127,
        LVM_SETCOLUMN = 4122,
        TVM_SETIMAGELIST = 4361,
        TVM_SETITEMW = 4415
    }

    [Flags]
    public enum TPM
    {
        TPM_LEFTBUTTON = 0x0000,
        TPM_RIGHTBUTTON = 0x0002,

        TPM_LEFTALIGN = 0x0000,
        TPM_CENTERALIGN = 0x004,
        TPM_RIGHTALIGN = 0x008,
        TPM_TOPALIGN = 0x0000,
        TPM_VCENTERALIGN = 0x0010,
        TPM_BOTTOMALIGN = 0x0020,

        TPM_HORIZONTAL = 0x0000,
        TPM_VERTICAL = 0x0040,
        TPM_NONOTIFY = 0x0080,
        TPM_RETURNCMD = 0x0100,
        TPM_RECURSE = 0x0001,
        TPM_HORPOSANIMATION = 0x0400,
        TPM_HORNEGANIMATION = 0x0800,
        TPM_VERPOSANIMATION = 0x1000,
        TPM_VERNEGANIMATION = 0x2000,
        TPM_NOANIMATION = 0x4000,
        TPM_LAYOUTRTL = 0x8000,
    }

  

    [Flags]
    public enum TVIF
    {
        TVIF_TEXT = 0x0001,
        TVIF_IMAGE = 0x0002,
        TVIF_PARAM = 0x0004,
        TVIF_STATE = 0x0008,
        TVIF_HANDLE = 0x0010,
        TVIF_SELECTEDIMAGE = 0x0020,
        TVIF_CHILDREN = 0x0040,
        TVIF_INTEGRAL = 0x0080,
    }

    [Flags]
    public enum TVIS
    {
        TVIS_SELECTED = 0x0002,
        TVIS_CUT = 0x0004,
        TVIS_DROPHILITED = 0x0008,
        TVIS_BOLD = 0x0010,
        TVIS_EXPANDED = 0x0020,
        TVIS_EXPANDEDONCE = 0x0040,
        TVIS_EXPANDPARTIAL = 0x0080,
        TVIS_OVERLAYMASK = 0x0F00,
        TVIS_STATEIMAGEMASK = 0xF000,
        TVIS_USERMASK = 0xF000,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct TVITEMW
    {
        public TVIF mask;
        public IntPtr hItem;
        public TVIS state;
        public TVIS stateMask;
        public string pszText;
        public int cchTextMax;
        public int iImage;
        public int iSelectedImage;
        public int cChildren;
        public int lParam;
    }

    internal enum GetWindow_Cmd : uint
    {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }


   

    public enum WM
    {
        NULL = 0x0000,
        CREATE = 0x0001,
        DESTROY = 0x0002,
        MOVE = 0x0003,
        SIZE = 0x0005,
        ACTIVATE = 0x0006,
        SETFOCUS = 0x0007,
        KILLFOCUS = 0x0008,
        ENABLE = 0x000A,
        SETREDRAW = 0x000B,
        SETTEXT = 0x000C,
        GETTEXT = 0x000D,
        GETTEXTLENGTH = 0x000E,
        PAINT = 0x000F,
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        QUERYOPEN = 0x0013,
        ENDSESSION = 0x0016,
        QUIT = 0x0012,
        ERASEBKGND = 0x0014,
        SYSCOLORCHANGE = 0x0015,
        SHOWWINDOW = 0x0018,
        WININICHANGE = 0x001A,
        SETTINGCHANGE = WININICHANGE,
        DEVMODECHANGE = 0x001B,
        ACTIVATEAPP = 0x001C,
        FONTCHANGE = 0x001D,
        TIMECHANGE = 0x001E,
        CANCELMODE = 0x001F,
        SETCURSOR = 0x0020,
        MOUSEACTIVATE = 0x0021,
        CHILDACTIVATE = 0x0022,
        QUEUESYNC = 0x0023,
        GETMINMAXINFO = 0x0024,
        PAINTICON = 0x0026,
        ICONERASEBKGND = 0x0027,
        NEXTDLGCTL = 0x0028,
        SPOOLERSTATUS = 0x002A,
        DRAWITEM = 0x002B,
        MEASUREITEM = 0x002C,
        DELETEITEM = 0x002D,
        VKEYTOITEM = 0x002E,
        CHARTOITEM = 0x002F,
        SETFONT = 0x0030,
        GETFONT = 0x0031,
        SETHOTKEY = 0x0032,
        GETHOTKEY = 0x0033,
        QUERYDRAGICON = 0x0037,
        COMPAREITEM = 0x0039,
        GETOBJECT = 0x003D,
        COMPACTING = 0x0041,
        COMMNOTIFY = 0x0044,
        WINDOWPOSCHANGING = 0x0046,
        WINDOWPOSCHANGED = 0x0047,
        POWER = 0x0048,
        COPYDATA = 0x004A,
        CANCELJOURNAL = 0x004B,
        NOTIFY = 0x004E,
        INPUTLANGCHANGEREQUEST = 0x0050,
        INPUTLANGCHANGE = 0x0051,
        TCARD = 0x0052,
        HELP = 0x0053,
        USERCHANGED = 0x0054,
        NOTIFYFORMAT = 0x0055,
        CONTEXTMENU = 0x007B,
        STYLECHANGING = 0x007C,
        STYLECHANGED = 0x007D,
        DISPLAYCHANGE = 0x007E,
        GETICON = 0x007F,
        SETICON = 0x0080,
        NCCREATE = 0x0081,
        NCDESTROY = 0x0082,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCPAINT = 0x0085,
        NCACTIVATE = 0x0086,
        GETDLGCODE = 0x0087,
        SYNCPAINT = 0x0088,


        NCMOUSEMOVE = 0x00A0,
        NCLBUTTONDOWN = 0x00A1,
        NCLBUTTONUP = 0x00A2,
        NCLBUTTONDBLCLK = 0x00A3,
        NCRBUTTONDOWN = 0x00A4,
        NCRBUTTONUP = 0x00A5,
        NCRBUTTONDBLCLK = 0x00A6,
        NCMBUTTONDOWN = 0x00A7,
        NCMBUTTONUP = 0x00A8,
        NCMBUTTONDBLCLK = 0x00A9,
        NCXBUTTONDOWN = 0x00AB,
        NCXBUTTONUP = 0x00AC,
        NCXBUTTONDBLCLK = 0x00AD,

        INPUT_DEVICE_CHANGE = 0x00FE,
        INPUT = 0x00FF,

        KEYFIRST = 0x0100,
        KEYDOWN = 0x0100,
        KEYUP = 0x0101,
        CHAR = 0x0102,
        DEADCHAR = 0x0103,
        SYSKEYDOWN = 0x0104,
        SYSKEYUP = 0x0105,
        SYSCHAR = 0x0106,
        SYSDEADCHAR = 0x0107,
        UNICHAR = 0x0109,
        KEYLAST = 0x0109,

        IME_STARTCOMPOSITION = 0x010D,
        IME_ENDCOMPOSITION = 0x010E,
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,

        INITDIALOG = 0x0110,
        COMMAND = 0x0111,
        SYSCOMMAND = 0x0112,
        TIMER = 0x0113,
        HSCROLL = 0x0114,
        VSCROLL = 0x0115,
        INITMENU = 0x0116,
        INITMENUPOPUP = 0x0117,
        MENUSELECT = 0x011F,
        MENUCHAR = 0x0120,
        ENTERIDLE = 0x0121,
        MENURBUTTONUP = 0x0122,
        MENUDRAG = 0x0123,
        MENUGETOBJECT = 0x0124,
        UNINITMENUPOPUP = 0x0125,
        MENUCOMMAND = 0x0126,

        CHANGEUISTATE = 0x0127,
        UPDATEUISTATE = 0x0128,
        QUERYUISTATE = 0x0129,

        CTLCOLORMSGBOX = 0x0132,
        CTLCOLOREDIT = 0x0133,
        CTLCOLORLISTBOX = 0x0134,
        CTLCOLORBTN = 0x0135,
        CTLCOLORDLG = 0x0136,
        CTLCOLORSCROLLBAR = 0x0137,
        CTLCOLORSTATIC = 0x0138,
        MN_GETHMENU = 0x01E1,

        MOUSEFIRST = 0x0200,
        MOUSEMOVE = 0x0200,
        LBUTTONDOWN = 0x0201,
        LBUTTONUP = 0x0202,
        LBUTTONDBLCLK = 0x0203,
        RBUTTONDOWN = 0x0204,
        RBUTTONUP = 0x0205,
        RBUTTONDBLCLK = 0x0206,
        MBUTTONDOWN = 0x0207,
        MBUTTONUP = 0x0208,
        MBUTTONDBLCLK = 0x0209,
        MOUSEWHEEL = 0x020A,
        XBUTTONDOWN = 0x020B,
        XBUTTONUP = 0x020C,
        XBUTTONDBLCLK = 0x020D,
        MOUSEHWHEEL = 0x020E,

        PARENTNOTIFY = 0x0210,
        ENTERMENULOOP = 0x0211,
        EXITMENULOOP = 0x0212,

        NEXTMENU = 0x0213,
        SIZING = 0x0214,
        CAPTURECHANGED = 0x0215,
        MOVING = 0x0216,

        POWERBROADCAST = 0x0218,

        DEVICECHANGE = 0x0219,

        MDICREATE = 0x0220,
        MDIDESTROY = 0x0221,
        MDIACTIVATE = 0x0222,
        MDIRESTORE = 0x0223,
        MDINEXT = 0x0224,
        MDIMAXIMIZE = 0x0225,
        MDITILE = 0x0226,
        MDICASCADE = 0x0227,
        MDIICONARRANGE = 0x0228,
        MDIGETACTIVE = 0x0229,


        MDISETMENU = 0x0230,
        ENTERSIZEMOVE = 0x0231,
        EXITSIZEMOVE = 0x0232,
        DROPFILES = 0x0233,
        MDIREFRESHMENU = 0x0234,

        IME_SETCONTEXT = 0x0281,
        IME_NOTIFY = 0x0282,
        IME_CONTROL = 0x0283,
        IME_COMPOSITIONFULL = 0x0284,
        IME_SELECT = 0x0285,
        IME_CHAR = 0x0286,
        IME_REQUEST = 0x0288,
        IME_KEYDOWN = 0x0290,
        IME_KEYUP = 0x0291,

        MOUSEHOVER = 0x02A1,
        MOUSELEAVE = 0x02A3,
        NCMOUSEHOVER = 0x02A0,
        NCMOUSELEAVE = 0x02A2,

        WTSSESSION_CHANGE = 0x02B1,

        TABLET_FIRST = 0x02c0,
        TABLET_LAST = 0x02df,

        CUT = 0x0300,
        COPY = 0x0301,
        PASTE = 0x0302,
        CLEAR = 0x0303,
        UNDO = 0x0304,
        RENDERFORMAT = 0x0305,
        RENDERALLFORMATS = 0x0306,
        DESTROYCLIPBOARD = 0x0307,
        DRAWCLIPBOARD = 0x0308,
        PAINTCLIPBOARD = 0x0309,
        VSCROLLCLIPBOARD = 0x030A,
        SIZECLIPBOARD = 0x030B,
        ASKCBFORMATNAME = 0x030C,
        CHANGECBCHAIN = 0x030D,
        HSCROLLCLIPBOARD = 0x030E,
        QUERYNEWPALETTE = 0x030F,
        PALETTEISCHANGING = 0x0310,
        PALETTECHANGED = 0x0311,
        HOTKEY = 0x0312,

        PRINT = 0x0317,
        PRINTCLIENT = 0x0318,

        APPCOMMAND = 0x0319,

        THEMECHANGED = 0x031A,

        CLIPBOARDUPDATE = 0x031D,

        DWMCOMPOSITIONCHANGED = 0x031E,
        DWMNCRENDERINGCHANGED = 0x031F,
        DWMCOLORIZATIONCOLORCHANGED = 0x0320,
        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        GETTITLEBARINFOEX = 0x033F,

        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,

        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,

        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,

        APP = 0x8000,

        USER = 0x0400,

        REFLECT = USER + 0x1C00,
    }

    class User32
    {

        [DllImport("user32.dll")]
        public static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);


        // GOOD
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AppendMenu(IntPtr hMenu, MFT uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern IntPtr CreatePopupMenu();

        [DllImport("user32", EntryPoint = "InsertMenuA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern bool InsertMenu(System.IntPtr hMenu, int uPosition, int uFlags, System.IntPtr uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        public static extern int InsertMenuItem(IntPtr hMenu, uint uItem, bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll")]
        public static extern bool DeleteMenu(IntPtr hMenu, int uPosition,
            MF uFlags);

        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        [DllImport("user32.dll")]
        public static extern IntPtr EnumChildWindows(IntPtr parentHandle,
            Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetMenuInfo(IntPtr hmenu,
            ref MENUINFO lpcmi);

        [DllImport("user32.dll")]
        public static extern uint GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        public static extern bool GetMenuItemInfo(IntPtr hMenu, int uItem,
            bool fByPosition, ref MENUITEMINFO lpmii);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll")]
        public static extern uint RegisterClipboardFormat(string lpszFormat);

        #region sendmessage

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, MSG Msg,
            int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, MSG Msg,
            int wParam, ref LVITEMA lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, MSG Msg,
            int wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, MSG Msg,
            int wParam, ref TVITEMW lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, MSG Msg,
            int wParam, ref Point lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


        #endregion

        [DllImport("user32.dll")]
        public static extern bool SetMenuInfo(IntPtr hmenu,
            ref MENUINFO lpcmi);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd,
            IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
            uint uFlags);

        [DllImport("user32.dll")]
        public static extern int TrackPopupMenuEx(IntPtr hmenu,
            TPM fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);


        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
    }
}
