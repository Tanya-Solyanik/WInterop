﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace WInterop.Windows.Types
{
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600.aspx
    [Flags]
    public enum WindowStyle : uint
    {
        WS_OVERLAPPED      = 0x00000000,
        WS_POPUP           = 0x80000000,
        WS_CHILD           = 0x40000000,
        WS_MINIMIZE        = 0x20000000,
        WS_VISIBLE         = 0x10000000,
        WS_DISABLED        = 0x08000000,
        WS_CLIPSIBLINGS    = 0x04000000,
        WS_CLIPCHILDREN    = 0x02000000,
        WS_MAXIMIZE        = 0x01000000,
        WS_CAPTION         = 0x00C00000,
        WS_BORDER          = 0x00800000,
        WS_DLGFRAME        = 0x00400000,
        WS_VSCROLL         = 0x00200000,
        WS_HSCROLL         = 0x00100000,
        WS_SYSMENU         = 0x00080000,
        WS_THICKFRAME      = 0x00040000,
        WS_GROUP           = 0x00020000,
        WS_TABSTOP         = 0x00010000,
        WS_MINIMIZEBOX     = 0x00020000,
        WS_MAXIMIZEBOX     = 0x00010000,
        WS_TILED           = WS_OVERLAPPED,
        WS_ICONIC          = WS_MINIMIZE,
        WS_SIZEBOX         = WS_THICKFRAME,
        WS_TILEDWINDOW     = WS_OVERLAPPEDWINDOW,
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME
                                | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUPWINDOW     = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_CHILDWINDOW     = WS_CHILD
    }
}
