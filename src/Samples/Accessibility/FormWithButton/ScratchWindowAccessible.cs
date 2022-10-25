// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Accessibility;
using WInterop.Windows;

namespace FormWithButton;

/// <summary>
///  Accessible object for client area of <see cref="ScratchWindowClass"/> window.
/// </summary>
internal class ScratchWindowAccessible : BaseAccessible
{
    private readonly ScratchWindowClass _owner;

    public ScratchWindowAccessible(WindowClass window)
    {
        _owner = (ScratchWindowClass)window;
    }

    // Window object for the button.
    public override int GetChildCount()
    {
        return base.GetChildCount();
    }

    public override BaseAccessible? GetChild(int childId)
    {
        return base.GetChild(childId);
    }

    public override BaseAccessible? HitTest(int x, int y)
    {
        return base.HitTest(x, y);
    }

    public override Rectangle Bounds
    {
        get 
        { 
            return base.Bounds;
        }
    }

    public override string? DefaultAction
    {
        get { return base.DefaultAction; }
    }

    public override string? Description
    {
        get { return base.Description; }
    }

    public override string? Help
    {
        get { return base.Help; }
    }

    public override string ToString() => $"{base.ToString()} {_owner._window.HWND:x}";
}
