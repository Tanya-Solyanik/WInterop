// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using WInterop.Errors;

public static class Oleaut32
{
    [ComImport]
    [Guid("00020404-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe interface IEnumVariant
    {
        [PreserveSig]
        HResult Next(uint celt, IntPtr rgVar, uint* pCeltFetched);

        [PreserveSig]
        HResult Skip(uint celt);

        [PreserveSig]
        HResult Reset();

        [PreserveSig]
        HResult Clone([Out, MarshalAs(UnmanagedType.LPArray)] IEnumVariant[]? ppEnum);
    }
}