// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define TRACE_ACCESSIBLE

using System.CodeDom;
using WInterop.Accessibility;
using WInterop.Windows;
using System.Runtime.InteropServices;
using WInterop.Errors;
using Accessibility;
using TerraFX.Interop.DirectX;
using Oleacc = WInterop.Accessibility.Native.Oleacc;

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
        Trace($"HitTest {x}, {y}");

        if (Bounds.Contains(x, y))
        {
            return ChildBounds.Contains(x, y) ? ButtonId : Oleacc.CHILDID_SELF;
        }

        return null;
#if false
        // The vt member of pvarID is VT_EMPTY.
        var e = new COMException("The point is outside of the object boundaries.", errorCode: (int)HResult.S_FALSE)
        {
            HResult = (int)HResult.S_FALSE
        };
        throw e;
#endif
    }

    public override Rectangle Bounds
    {
        get
        {
            WindowHandle handle = _owner._window;
            Rectangle client = handle.GetClientRectangle();
            Trace($"Client ({client.X}, {client.Y}, {client.Width}, {client.Height}).");

            Rectangle windowRect = handle.GetWindowRectangle();
            Trace($"Window ({windowRect.X}, {windowRect.Y}, {windowRect.Width}, {windowRect.Height}).");

            return windowRect;
        }
    }

    private Rectangle ChildBounds
    {
        get
        {
            WindowHandle handle = _owner.ChildWindow;

            Rectangle windowRect = handle.GetWindowRectangle();
            Trace($"Child window ({windowRect.X}, {windowRect.Y}, {windowRect.Width}, {windowRect.Height}).");

            return windowRect;
        }
    }

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
        Trace($"Description {id}");
        return id switch
        {
            Oleacc.CHILDID_SELF => "Scratch form description.",
            ButtonId => "Scratch button description.",
            _ => null,
        };
    }

    public override string? Help(int id)
    {
        Trace($"Help {id}");
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

    private readonly string?[] _names = new string?[2]
    {
        "Scratch form",
        "Scratch button"
    };

    public override string? GetName(int id)
    {
        return _names[id];
    }

    public override void SetName(int id, string? name)
    {
        _names[id] = name;
    }

    // TODO: get standard WINDOW object?
    public override IAccessible? Parent => null;

    public override AccessibleRole? Role(int id)
    {
        Trace($"Role {id}");

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

    private readonly string?[] _values = new string?[2]
    {
        "Scratch form value",
        "Scratch button value"
    };

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

        Rectangle r = _owner.ChildWindow.GetWindowRectangle();
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

        if (id == Oleacc.CHILDID_SELF)
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

    public override string ToString() 
        => $"{base.ToString()} {_owner._window.HWND:x}";
}
