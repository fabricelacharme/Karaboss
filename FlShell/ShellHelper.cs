using System;
using System.Drawing;
using System.Windows.Forms;
using FlShell.Interop;
using System.Runtime.InteropServices;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace FlShell
{
    public class ShellHelper
    {
        /// <summary>
        ///  It obtains a DragDropEffects flag variable indicating the input ShellItem's ability to accept a Paste from the Clipboard.
        /// </summary>
        /// <param name="item">The item whose ability to accept a Paste is to be queried.</param>
        /// <returns>A DragDropEffect indicating what actions the input ShellItem is willing to do.</returns>
        /// <remarks>Used to determine if Paste is a valid menu item.</remarks>
        public static DragDropEffects CanDropClipboard(ShellItem item)
        {
            System.Runtime.InteropServices.ComTypes.IDataObject dataObject;
            
            Ole32.OleGetClipboard(out dataObject);

            FlShell.Interop.IDropTarget target = null;

            DragDropEffects retVal = DragDropEffects.None;
            if (GetIDropTarget(item, ref target))
            {
                DragDropEffects effects = DragDropEffects.Copy;

                target.DragEnter(dataObject, MK.MK_CONTROL, new Point(), ref effects);
                //if (target.DragEnter(dataObject, MK.MK_CONTROL, new Point(), ref effects) == (int)HResult.S_OK)
                //{
                    if (effects == DragDropEffects.Copy)
                        retVal = retVal | DragDropEffects.Copy;


                    target.DragLeave();
                //}

                effects = DragDropEffects.Move;
                target.DragEnter(dataObject, MK.MK_SHIFT, new Point(), effects);
                //if (target.DragEnter(dataObject, MK.MK_SHIFT, new Point(), effects) == (int)HResult.S_OK)
                //{
                    if (effects == DragDropEffects.Move)
                        retVal = retVal | DragDropEffects.Move;


                    target.DragLeave();
                //}

                effects = DragDropEffects.Link;
                target.DragEnter(dataObject, MK.MK_ALT, new Point(), effects);
                //if (target.DragEnter(dataObject, MK.MK_ALT, new Point(), effects) == (int)HResult.S_OK)
                //{
                    if (effects == DragDropEffects.Link)
                        retVal = retVal | DragDropEffects.Link;


                    target.DragLeave();
                //}

                Marshal.ReleaseComObject(target);
            }

            return retVal;
        }

        #region "       GetIDropTarget"
        /// <summary>
        /// This method uses the GetUIObjectOf method of IShellFolder to obtain the IDropTarget of a
        /// CShItem. 
        /// </summary>
        /// <param name="item">The item for which to obtain the IDropTarget</param>
        /// <param name="dropTarget">The IDropTarget interface of the input Folder</param>
        /// <returns>true if successful in obtaining the IDropTarget Interface.</returns>
        /// <remarks>The original FileBrowser version of this returned the IntPtr which points to
        /// the interface. This is not needed since GetTypedObjectForIUnknown manages that IntPtr.
        /// For all purposes, the CShItem.GetDropTargetOf routine is more efficient and provides
        /// the same interface.</remarks>
        public static bool GetIDropTarget(ShellItem item, ref FlShell.Interop.IDropTarget dropTarget)
        {
            IntPtr dropTargetPtr = IntPtr.Zero;
            ShellItem parent = item.Parent;
            if (parent == null)
                parent = item;
            IShellFolder folder;
            if (item == ShellItem.Desktop)
                folder = item.GetIShellFolder();
            else
                folder = item.Parent.GetIShellFolder();

            IntPtr relpidl = Shell32.ILFindLastID(item.Pidl);
            if (parent.GetIShellFolder().GetUIObjectOf(IntPtr.Zero, 1, new IntPtr[] { relpidl }, Ole32.IID_IDropTarget, 0, out dropTargetPtr) == 0)
            {
                dropTarget = (FlShell.Interop.IDropTarget)Marshal.GetTypedObjectForIUnknown(dropTargetPtr, typeof(FlShell.Interop.IDropTarget));
                return true;
            }
            else
            {
                dropTarget = null;
                dropTargetPtr = IntPtr.Zero;
                return false;
            }
        }


  


        public static IntPtr GetIDataObject(ShellItem[] items)
        {
            ShellItem parent;
            if (items[0].Parent != null)
                parent = items[0].Parent;
            else
                parent = items[0];


            IntPtr[] pidls = new IntPtr[items.Length];
            int i = 0;
            do
            {
                pidls[i] = Shell32.ILFindLastID(items[i].Pidl);
                i += 1;
            }
            while (i < items.Length);

            IntPtr dataObjectPtr = IntPtr.Zero;
            if (parent.GetIShellFolder().GetUIObjectOf(IntPtr.Zero, (uint)(pidls.Length), pidls, Ole32.IID_IDataObject, 0, out dataObjectPtr) == (int)HResult.S_OK)
                return dataObjectPtr;
            else
                return IntPtr.Zero;
        }

        #endregion

    }
}
