// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WInterop.Com;
using WInterop.Errors;

namespace WInterop.Accessibility.Native;

// winmdroot.System.Com.IDispatch* --->> IntPtr
// winmdroot.System.Com.VARIANT -->> Variant
// IntPtr   -->> IntPtr
// Variant* -->> IntPtr
[Guid("618736E0-3C3D-11CF-810C-00AA00389B71"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), ComImport()]
[SupportedOSPlatform("windows5.0")]
public interface IAccessible
{
    [PreserveSig]
    unsafe HResult get_accParent(IntPtr* ppdispParent);

    [PreserveSig]
    unsafe HResult get_accChildCount(int* pcountChildren);

    [PreserveSig]
    unsafe HResult get_accChild(Variant varChild, IntPtr* ppdispChild);

    [PreserveSig]
    unsafe HResult get_accName(Variant varChild, IntPtr pszName);

    [PreserveSig]
    unsafe HResult get_accValue(Variant varChild, IntPtr pszValue);

    [PreserveSig]
    unsafe HResult get_accDescription(Variant varChild, IntPtr pszDescription);

    [PreserveSig]
    unsafe HResult get_accRole(Variant varChild, IntPtr pvarRole);

    [PreserveSig]
    unsafe HResult get_accState(Variant varChild, IntPtr pvarState);

    [PreserveSig]
    unsafe HResult get_accHelp(Variant varChild, IntPtr pszHelp);

    [PreserveSig]
    unsafe HResult get_accHelpTopic(IntPtr pszHelpFile, Variant varChild, int* pidTopic);

    [PreserveSig]
    unsafe HResult get_accKeyboardShortcut(Variant varChild, IntPtr pszKeyboardShortcut);

    [PreserveSig]
    unsafe HResult get_accFocus(IntPtr pvarChild);

    [PreserveSig]
    unsafe HResult get_accSelection(IntPtr pvarChildren);

    [PreserveSig]
    unsafe HResult get_accDefaultAction(Variant varChild, IntPtr pszDefaultAction);

    [PreserveSig]
    HResult accSelect(int flagsSelect, Variant varChild);

    [PreserveSig]
    unsafe HResult accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, Variant varChild);

    [PreserveSig]
    unsafe HResult accNavigate(int navDir, Variant varStart, IntPtr pvarEndUpAt);

    [PreserveSig]
    unsafe HResult accHitTest(int xLeft, int yTop, IntPtr pvarChild);

    [PreserveSig]
    HResult accDoDefaultAction(Variant varChild);

    [PreserveSig]
    HResult put_accName(Variant varChild, string szName);

    [PreserveSig]
    HResult put_accValue(Variant varChild, string szValue);
}