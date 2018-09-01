﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace WInterop.Gdi.Unsafe
{
    public readonly struct HRGN
    {
        public IntPtr Handle { get; }

        public HRGN(IntPtr handle)
        {
            Handle = handle;
        }

        public bool IsInvalid => Handle == IntPtr.Zero;

        public static implicit operator HGDIOBJ(HRGN handle) => new HGDIOBJ(handle.Handle);
        public static explicit operator HRGN(HGDIOBJ handle) => new HRGN(handle.Handle);
    }
}