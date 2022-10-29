// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CodeDom;
using WInterop.Accessibility;
using WInterop.Accessibility.Native;
using WInterop.Windows;
using System.Runtime.InteropServices;
using WInterop.Errors;
using Accessibility;
using TerraFX.Interop.DirectX;

namespace FormWithButton;

/// <summary>
///  Accessible object for client area of <see cref="ScratchWindowClass"/> window.
///  ScratchWindow
///       |
///  ScratchButton - swallows WM_GETOBJECT and does not even forward it to the
///                default WinProc. This button is treated as a simple element.
/// </summary>
internal class ScratchWindowAccessible : BaseAccessible
{
    private readonly ScratchWindowClass _owner;
    private const int ButtonId = 1;

    public ScratchWindowAccessible(WindowClass window)
    {
        _owner = (ScratchWindowClass)window;
    }

    public override int GetChildCount()
    {
        // Element for the button.
        return 1;
    }

    protected override bool IsChildElement(int id)
    {
        return id == ButtonId; 
    }

    public override object? HitTest(int x, int y)
    {
        if (Bounds.Contains(x, y))
        {
            Rectangle r = _owner.ChildWindow.GetClientRectangle();
            return r.Contains(x, y) ? ButtonId : Oleacc.CHILDID_SELF;
        }

        // The vt member of pvarID is VT_EMPTY.
        var e = new COMException("The point is outside of the object's boundaries.", errorCode: (int)HResult.S_FALSE)
        {
            HResult = (int)HResult.S_FALSE
        };
        throw e;
    }

    public override Rectangle Bounds
        => _owner._window.GetClientRectangle();

    public override string? DefaultAction(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => "None",
            ButtonId => "Click",
            _ => null,
        };
    }

    public override string? Description(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => "Scratch form.",
            ButtonId => "Scratch button.",
            _ => null,
        };
    }

    public override string? Help(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => "Scratch form help.",
            ButtonId => "Scratch button help.",
            _ => null,
        };
    }

    public override string? KeyboardShortcut(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => null,
            ButtonId => "P",
            _ => null,
        };
    }

    private readonly string?[] _names = new string?[2];

    public override string? GetName(int id)
    {
        return _names[id];
    }

    public override void SetName(int id, string? name)
    {
        _names[id] = name;
    }

    // TODO: get standard object?
    public override IAccessible? Parent => null;

    public override AccessibleRole? Role(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => AccessibleRole.Window,
            ButtonId => AccessibleRole.PushButton,
            _ => null,
        };
    }

    // TODO: these values are random
    public override AccessibleStates? State(int id)
    {
        return id switch
        {
            Oleacc.CHILDID_SELF => AccessibleStates.Focused,
            ButtonId => AccessibleStates.Focusable,
            _ => AccessibleStates.None,
        };
    }

    // TODO: get selected from the std obj
    public override object[]? GetSelected()
    {
        return new object[] { Oleacc.CHILDID_SELF };
    }

    private readonly string?[] _values = new string?[2];

    public override string? GetValue(int id)
    {
        return _values[id];
    }

    public override void SetValue(int id, string? newValue)
    {
        _values[id] = newValue;
    }

    // TODO: just a random action
    public override void Select(AccessibleSelection flags, int id)
    {
        if (id == Oleacc.CHILDID_SELF)
        {
            _owner._window.SetFocus();
        }
        else if (id == ButtonId)
        {
            _owner.ChildWindow.SetFocus();
        }
    }

    public override void DoDefaultAction(int id)
    {
        if (id == ButtonId)
        {
            _owner._window.SendMessage(MessageType.Command, wParam: new WParam(0, 123));
        }
    }

    public override object? GetFocused()
    {
        return null;
    }

    public override void GetChildLocation(out int left, out int top, out int width, out int height, int id)
    {
        left = 0;
        top = 0;
        width = 0;
        height = 0;

        if (id != ButtonId) 
        {
            return;
        }

        Rectangle r = _owner.ChildWindow.GetClientRectangle();
        left = r.Left;
        top = r.Top;
        width = r.Width;
        height = r.Height;
    }

    public override object? Navigate(AccessibleNavigation direction, int id)
    {
        if (id == ButtonId)
        {
            return null;
        }
        else if (id == Oleacc.CHILDID_SELF)
        {
            switch (direction)
            {
                case AccessibleNavigation.FirstChild:
                case AccessibleNavigation.LastChild:
                    return ButtonId;
            }
        }
        return null;
    }
    public override string ToString() => $"{base.ToString()} {_owner._window.HWND:x}";
}
