#region License

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

#endregion

#region Contact

/*
 * Fabrice Lacharme
 * Email: fabrice.lacharme@gmail.com
 */

#endregion
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

#pragma warning disable 1591

namespace FlShell.Interop
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000122-0000-0000-C000-000000000046")]
    public interface IDropTarget
    {
        //void DragEnter(IDataObject pDataObj, int grfKeyState, Point pt, ref int pdwEffect);
        //void DragOver(int grfKeyState, Point pt, ref int pdwEffect);
        //void DragLeave();
        //void Drop(IDataObject pDataObj, int grfKeyState,  Point pt, ref int pdwEffect);

        // Determines whether a drop can be accepted and its effect if it is accepted
        //[PreserveSig]
        void DragEnter(System.Runtime.InteropServices.ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect);

        // Provides target feedback to the user through the DoDragDrop function
        //[PreserveSig]
        void DragOver(MK grfKeyState, Point pt, ref DragDropEffects pdwEffect);

        // Causes the drop target to suspend its feedback actions
        //[PreserveSig]
        void DragLeave();

        // Drops the data into the target window
        //[PreserveSig]
        void Drop(System.Runtime.InteropServices.ComTypes.IDataObject pDataObj, MK grfKeyState, Point pt, ref DragDropEffects pdwEffect);


    }
}