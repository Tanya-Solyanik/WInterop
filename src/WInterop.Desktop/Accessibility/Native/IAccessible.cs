// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using WInterop.Errors;

namespace WInterop.Accessibility.Native;

[Guid("618736E0-3C3D-11CF-810C-00AA00389B71"), InterfaceType(ComInterfaceType.InterfaceIsIDispatch), ComImport()]
[SupportedOSPlatform("windows5.0")]
internal interface IAccessible
{
    [PreserveSig]
    unsafe HResult get_accParent(winmdroot.System.Com.IDispatch** ppdispParent);
    [PreserveSig]
    unsafe HResult get_accChildCount(int* pcountChildren);
    [PreserveSig]
    unsafe HResult get_accChild(winmdroot.System.Com.VARIANT varChild, winmdroot.System.Com.IDispatch** ppdispChild);
    [PreserveSig]
    unsafe HResult get_accName(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszName);
    [PreserveSig]
    unsafe HResult get_accValue(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszValue);
    [PreserveSig]
    unsafe HResult get_accDescription(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszDescription);
    [PreserveSig]
    unsafe HResult get_accRole(winmdroot.System.Com.VARIANT varChild, winmdroot.System.Com.VARIANT* pvarRole);
    [PreserveSig]
    unsafe HResult get_accState(winmdroot.System.Com.VARIANT varChild, winmdroot.System.Com.VARIANT* pvarState);
    [PreserveSig]
    unsafe HResult get_accHelp(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszHelp);
    [PreserveSig]
    unsafe HResult get_accHelpTopic(winmdroot.Foundation.BSTR* pszHelpFile, winmdroot.System.Com.VARIANT varChild, int* pidTopic);
    [PreserveSig]
    unsafe HResult get_accKeyboardShortcut(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszKeyboardShortcut);
    [PreserveSig]
    unsafe HResult get_accFocus(winmdroot.System.Com.VARIANT* pvarChild);
    [PreserveSig]
    unsafe HResult get_accSelection(winmdroot.System.Com.VARIANT* pvarChildren);
    [PreserveSig]
    unsafe HResult get_accDefaultAction(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR* pszDefaultAction);
    [PreserveSig]
    HResult accSelect(int flagsSelect, winmdroot.System.Com.VARIANT varChild);
    [PreserveSig]
    unsafe HResult accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, winmdroot.System.Com.VARIANT varChild);
    [PreserveSig]
    unsafe HResult accNavigate(int navDir, winmdroot.System.Com.VARIANT varStart, winmdroot.System.Com.VARIANT* pvarEndUpAt);
    [PreserveSig]
    unsafe HResult accHitTest(int xLeft, int yTop, winmdroot.System.Com.VARIANT* pvarChild);
    [PreserveSig]
    HResult accDoDefaultAction(winmdroot.System.Com.VARIANT varChild);
    [PreserveSig]
    HResult put_accName(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR szName);
    [PreserveSig]
    HResult put_accValue(winmdroot.System.Com.VARIANT varChild, winmdroot.Foundation.BSTR szValue);
}