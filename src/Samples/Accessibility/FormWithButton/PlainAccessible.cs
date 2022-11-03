// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define TRACE_ACCESSIBLE

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using WInterop.Accessibility.Native;
using WInterop.Com;
using WInterop.Errors;

namespace FormWithButton;

/// <summary>
///  <see cref="IAccessible"/> base class. Simplified version of <see cref="AccessibleObject"/> that only
///  provides the <see cref="IAccessible"/> interface.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class PlainAccessible : IAccessible
{
    unsafe HResult IAccessible.get_accParent(IntPtr* ppdispParent) => throw new NotImplementedException();
    unsafe HResult IAccessible.get_accChildCount(int* pcountChildren) => throw new NotImplementedException();
    unsafe HResult IAccessible.get_accChild(Variant varChild, IntPtr* ppdispChild) => throw new NotImplementedException();
    HResult IAccessible.get_accName(Variant varChild, IntPtr pszName) => throw new NotImplementedException();
    HResult IAccessible.get_accValue(Variant varChild, IntPtr pszValue) => throw new NotImplementedException();
    HResult IAccessible.get_accDescription(Variant varChild, IntPtr pszDescription) => throw new NotImplementedException();
    HResult IAccessible.get_accRole(Variant varChild, IntPtr pvarRole) => throw new NotImplementedException();
    unsafe HResult IAccessible.get_accState(Variant varChild, IntPtr pvarState) => throw new NotImplementedException();
    HResult IAccessible.get_accHelp(Variant varChild, IntPtr pszHelp) => throw new NotImplementedException();
    unsafe HResult IAccessible.get_accHelpTopic(IntPtr pszHelpFile, Variant varChild, int* pidTopic) => throw new NotImplementedException();
    HResult IAccessible.get_accKeyboardShortcut(Variant varChild, IntPtr pszKeyboardShortcut) => throw new NotImplementedException();
    HResult IAccessible.get_accFocus(IntPtr pvarChild) => throw new NotImplementedException();
    HResult IAccessible.get_accSelection(IntPtr pvarChildren) => throw new NotImplementedException();
    HResult IAccessible.get_accDefaultAction(Variant varChild, IntPtr pszDefaultAction) => throw new NotImplementedException();
    HResult IAccessible.accSelect(int flagsSelect, Variant varChild) => throw new NotImplementedException();
    unsafe HResult IAccessible.accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, Variant varChild) => throw new NotImplementedException();
    HResult IAccessible.accNavigate(int navDir, Variant varStart, IntPtr pvarEndUpAt) => throw new NotImplementedException();
    HResult IAccessible.accHitTest(int xLeft, int yTop, IntPtr pvarChild) => throw new NotImplementedException();
    HResult IAccessible.accDoDefaultAction(Variant varChild) => throw new NotImplementedException();
    HResult IAccessible.put_accName(Variant varChild, string szName) => throw new NotImplementedException();
    HResult IAccessible.put_accValue(Variant varChild, string szValue) => throw new NotImplementedException();

    [Conditional("TRACE_ACCESSIBLE")]
    protected void Trace(string message)
        => Debug.WriteLine($"{ToString()}: {message}");

    public override string ToString() => $"{GetType().Name}";

    private string DebuggerDisplay
        => $"{ToString()}";

}
