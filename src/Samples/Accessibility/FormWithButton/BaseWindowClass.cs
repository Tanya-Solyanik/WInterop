// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Accessibility;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using WInterop.Accessibility;
using WInterop.Accessibility.Native;
using WInterop.Modules;
using WInterop.Windows;

namespace FormWithButton;

internal class BaseWindowClass : WindowClass
{
    /// <summary>
    ///  Myself.
    /// </summary>
    internal WindowHandle _window;

    [AllowNull]
    protected BaseAccessible _clientAccessible;

    public BaseWindowClass(string? className, ModuleInstance? moduleInstance)
        : base(className: className,
            moduleInstance,
            classStyle: ClassStyle.HorizontalRedraw | ClassStyle.VerticalRedraw,
            backgroundBrush: default,
            icon: default,
            smallIcon: default,
            cursor: default,
            menuName: null,
            menuId: 0,
            classExtraBytes: 0,
            windowExtraBytes: 0)
    { }

    public BaseWindowClass(string registeredClassName)
        : base(registeredClassName)
    { }

    protected virtual BaseAccessible CreateClientAccessible()
        => _clientAccessible ??= new();

    protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
    {
        switch (message)
        {
            case MessageType.GetObject:
                nint lResult = TryGetAccessibilityObject((uint)wParam, (int)(uint)lParam);
                if (0 != lResult)
                {
                    // We are able to create the CLIENT accessibility object,
                    // and we don't want to create the WINDOW accessibility object,
                    // so don't pass the message on to the base class class if we
                    // created an object.
                    return lResult;
                }
                break;
            case MessageType.Destroy:
                break;
        }

        // Let the base class handle any other messages.
        return base.WindowProcedure(window, message, wParam, lParam);
    }

    private nint TryGetAccessibilityObject(uint flags, int identifier)
    {
        IAccessible? accessible = null;

        if (identifier == (int)ObjectIdentifier.Client)
        {
            accessible = CreateClientAccessible();
        }

        Debug.WriteLine(
            $"{GetType().Name} WM_GETOBJECT for {Utilities.ObjectIdentifierToString(identifier)}");

        if (accessible is null)
        {
            return 0;
        }

        IntPtr unknown = Marshal.GetIUnknownForObject(accessible);
        try
        {
            nint result = Oleacc.LresultFromObject(in Oleacc.IID_IAccessible, wParam: flags, unknown);
            if (result > 0)
            {
                return result;
            }

            Debug.Assert(result > 0);
        }
        finally
        {
            Marshal.Release(unknown);
        }

        return 0;
    }
}
