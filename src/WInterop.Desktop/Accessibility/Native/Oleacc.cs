// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;

namespace WInterop.Accessibility.Native;

public static class Oleacc
{
    public static Guid IID_IAccessible = new("{618736E0-3C3D-11CF-810C-00AA00389B71}");
    internal static readonly Guid IAccessibleGuid = new Guid(0x618736E0, 0x3C3D, 0x11CF, 0x81, 0x0C, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71);

    public const int CHILDID_SELF = 0;

    [DllImport(Libraries.Oleacc, ExactSpelling = true)]
    public static extern nint LresultFromObject(in Guid refiid, nuint wParam, IntPtr pAcc);
}