using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using FlShell.Interop;

namespace FlShell
{
    /// <summary>
    /// WindowsContextMenu provides the infrastucture for displaying a Windows Context Menu on a Control
    /// and for Invoking a Command selected by the user from that Context Menu. Cascaded sub-menus are created,
    /// displayed, and responded to as required.
    /// The Context Menu applies to all ShellItems
    /// passed to the ShowMenu Function and all ShellItems must be in the same Folder. 
    /// </summary>
    /// <remarks>Though specifically designed for ListView and TreeView Controls, this Class will work for any Control which
    ///          is associated with a single Folder and can provide ShellItems from that Folder.</remarks>
    public class WindowsContextMenu
    {

        public IContextMenu winMenu = null;
        public IContextMenu2 winMenu2 = null;
        public IContextMenu3 winMenu3 = null;
        public IContextMenu newMenu = null;
        public IContextMenu2 newMenu2 = null;
        public IContextMenu3 newMenu3 = null;
        public IntPtr newMenuPtr = IntPtr.Zero;
        private int min = 1;
        private int max = 100000;

        /// <summary>
        /// If this method returns true then the caller must call ReleaseMenu
        /// </summary>
        /// <param name="hwnd">The handle to the control to host the ContextMenu</param>
        /// <param name="items">The items for which to show the ContextMenu. These items must be in the same folder.</param>
        /// <param name="pt">The point where the ContextMenu should appear</param>
        /// <param name="allowrename">Set if (the ContextMenu should contain the Rename command where appropriate</param>
        /// <param name="cmi">The command information for the users selection</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool ShowMenu(IntPtr hwnd, ShellItem[] items, System.Drawing.Point pt, bool allowRename, ref CMInvokeCommandInfoEx cmi)
        {

            bool bShowMenu = false;

            Debug.Assert(items.Length > 0);

            IntPtr comContextMenu = User32.CreatePopupMenu();
            IntPtr[] pidls = new System.IntPtr[items.Length];
            int i;
            IShellFolder folder = null;

            if (items[0] == ShellItem.Desktop)
                folder = items[0].GetIShellFolder();
            else
                folder = items[0].Parent.GetIShellFolder();

            for (i = 0; i <= items.Length - 1; i++)
            {
                if (!items[i].CanRename)
                    allowRename = false;
                pidls[i] = Shell32.ILFindLastID(items[i].Pidl);
            }
            int prgf = 0;
            IntPtr pIcontext = IntPtr.Zero;
            int HR = folder.GetUIObjectOf(IntPtr.Zero, (uint)pidls.Length, pidls, Shell32.IID_IContextMenu, (uint)prgf, out pIcontext);

            if (HR != (int)HResult.S_OK)
            {
#if DEBUG
                Marshal.ThrowExceptionForHR(HR);
#endif
                //GoTo FAIL;
            }

            winMenu = (IContextMenu)Marshal.GetObjectForIUnknown(pIcontext);

            IntPtr p = IntPtr.Zero;

            Marshal.QueryInterface(pIcontext, ref Shell32.IID_IContextMenu2, out p);
            if (p != IntPtr.Zero)
                winMenu2 = (IContextMenu2)Marshal.GetObjectForIUnknown(p);


            Marshal.QueryInterface(pIcontext, ref Shell32.IID_IContextMenu3, out p);
            if (p != IntPtr.Zero)
                winMenu3 = (IContextMenu3)Marshal.GetObjectForIUnknown(p);


            if (!pIcontext.Equals(IntPtr.Zero))
                pIcontext = IntPtr.Zero;

            //Check item count - should always be 0 but check just in case
            uint startIndex = User32.GetMenuItemCount(comContextMenu);
            //Fill the context menu
            CMF flags = (CMF.NORMAL | CMF.EXPLORE);
            if (allowRename)
                flags = flags | CMF.CANRENAME;
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                flags = flags | CMF.EXTENDEDVERBS;
            int idCount = (int)winMenu.QueryContextMenu(comContextMenu, startIndex, min, max, flags);
            int cmd = User32.TrackPopupMenuEx(comContextMenu, TPM.TPM_RETURNCMD, pt.X, pt.Y, hwnd, IntPtr.Zero);

            if (cmd >= min && cmd <= idCount)
            {
                cmi = new CMInvokeCommandInfoEx();
                cmi.cbSize = Marshal.SizeOf(cmi);
                cmi.lpVerb = (IntPtr)(cmd - min);
                cmi.lpVerbW = (IntPtr)(cmd - min);
                cmi.nShow = (int)SW.SHOWNORMAL;
                cmi.fMask = (int)(CMIC.UNICODE | CMIC.PTINVOKE);
                cmi.ptInvoke = new System.Drawing.Point(pt.X, pt.Y);
                bShowMenu = true;
            }
            else
            {
                //FAIL:
                if (winMenu != null)
                {
                    Marshal.ReleaseComObject(winMenu);
                    winMenu = null;
                }
                bShowMenu = false;
            }
            if (winMenu2 != null)
            {
                Marshal.ReleaseComObject(winMenu2);
                winMenu2 = null;
            }
            if (winMenu3 != null)
            {
                Marshal.ReleaseComObject(winMenu3);
                winMenu3 = null;
            }
            if (!comContextMenu.Equals(IntPtr.Zero))
            {
                Marshal.Release(comContextMenu);
                comContextMenu = IntPtr.Zero;
            }

            return bShowMenu;
        }

        public void ReleaseMenu()
        {
            if (winMenu != null)
            {
                Marshal.ReleaseComObject(winMenu);
                winMenu = null;
            }
        }

        /// <summary>
        /// Invokes a specific command from an IContextMenu
        /// </summary>
        /// <param name="iContextMenu">the IContextMenu containing the item</param>
        /// <param name="cmd">the index of the command to invoke</param>
        /// <param name="parentDir">the parent directory from where to invoke</param>
        /// <param name="ptInvoke">the point (in screen coördinates) from which to invoke</param>
        public void InvokeCommand(IContextMenu iContextMenu, IntPtr cmd, string parentDir, System.Drawing.Point ptInvoke)
        {
            CMInvokeCommandInfoEx invoke = new CMInvokeCommandInfoEx();
            invoke.cbSize = Shell32.cbInvokeCommand;
            invoke.lpVerb = cmd;
            invoke.lpDirectory = parentDir;
            invoke.lpVerbW = cmd;
            invoke.lpDirectoryW = parentDir;
            invoke.fMask = (int)(CMIC.UNICODE | CMIC.PTINVOKE);
            if ((Control.ModifierKeys & Keys.Control) != 0)
                invoke.fMask = invoke.fMask | (int)CMIC.CONTROL_DOWN;
            if ((Control.ModifierKeys & Keys.Shift) != 0)
                invoke.fMask = invoke.fMask | (int)CMIC.SHIFT_DOWN;
            invoke.ptInvoke = new System.Drawing.Point(ptInvoke.X, ptInvoke.Y);
            invoke.nShow = (int)SW.SHOWNORMAL;

            iContextMenu.InvokeCommand(ref invoke);
        }

        /// <summary>
        /// If this method returns true then the caller must call ReleaseNewMenu
        /// </summary>
        /// <param name="itm"></param>
        /// <param name="contextMenu"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool SetUpNewMenu(ShellItem itm, IntPtr contextMenu, int index)
        {

            int HR;
            HResult idCount;
            newMenuPtr = IntPtr.Zero;
            HR = Ole32.CoCreateInstance(Shell32.CLSID_NewMenu, IntPtr.Zero, (uint)CLSCTX.INPROC_SERVER, Shell32.IID_IContextMenu, out newMenuPtr);
            if (HR == (int)HResult.S_OK)
            {
                newMenu = (IContextMenu)Marshal.GetObjectForIUnknown(newMenuPtr);

                IntPtr p = IntPtr.Zero;
                Marshal.QueryInterface(newMenuPtr, ref Shell32.IID_IContextMenu2, out p);
                if (p != IntPtr.Zero)
                    newMenu2 = (IContextMenu2)Marshal.GetObjectForIUnknown(p);


                Marshal.QueryInterface(newMenuPtr, ref Shell32.IID_IContextMenu3, out p);
                if (p != IntPtr.Zero)
                    newMenu3 = (IContextMenu3)Marshal.GetObjectForIUnknown(p);


                if (!p.Equals(IntPtr.Zero))
                {
                    Marshal.Release(p);
                    p = IntPtr.Zero;
                }

                IntPtr iShellExtInitPtr = IntPtr.Zero;
                HR = (Marshal.QueryInterface(newMenuPtr, ref Shell32.IID_IShellExtInit, out iShellExtInitPtr));
                if (HR == (int)HResult.S_OK)
                {
                    IShellExtInit shellExtInit = (IShellExtInit)Marshal.GetObjectForIUnknown(iShellExtInitPtr);
                    shellExtInit.Initialize(itm.Pidl, IntPtr.Zero, 0);

                    Marshal.ReleaseComObject(shellExtInit);
                    Marshal.Release(iShellExtInitPtr);
                }
                if (!newMenuPtr.Equals(IntPtr.Zero))
                {
                    Marshal.Release(newMenuPtr);
                    newMenuPtr = IntPtr.Zero;
                }
            }

            if (HR != (int)HResult.S_OK)
            {
                ReleaseNewMenu();
#if DEBUG
                Marshal.ThrowExceptionForHR(HR);
#endif
                return false;
            }

            idCount = newMenu.QueryContextMenu(contextMenu, (uint)index, min, max, (int)CMF.NORMAL);
            newMenuPtr = User32.GetSubMenu(contextMenu, index);

            return true;

        }

        public void ReleaseNewMenu()
        {
            if (newMenu != null)
            {
                Marshal.ReleaseComObject(newMenu);
                newMenu = null;
            }
            if (newMenu2 != null)
            {
                Marshal.ReleaseComObject(newMenu2);
                newMenu2 = null;
            }
            if (newMenu3 != null)
            {
                Marshal.ReleaseComObject(newMenu3);
                newMenu3 = null;
            }
            if (!newMenuPtr.Equals(IntPtr.Zero))
            {
                Marshal.Release(newMenuPtr);
                newMenuPtr = IntPtr.Zero;
            }
        }


    }
}
