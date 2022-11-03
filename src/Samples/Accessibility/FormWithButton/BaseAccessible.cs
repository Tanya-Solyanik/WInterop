// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define TRACE_ACCESSIBLE

using Accessibility;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using WInterop.Errors;
using Oleacc = WInterop.Accessibility.Native.Oleacc;

namespace FormWithButton;

/// <summary>
///  <see cref="IAccessible"/> base class. Simplified version of <see cref="AccessibleObject"/> that only
///  provides the <see cref="IAccessible"/> interface.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class BaseAccessible : IAccessible
{
    private const int DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004);

    private static int ChildIdToInt(object childId)
    {
        // TODO: not a VT_I4 should return E_INVALIDARG
        int id = childId is int @int
            ? @int
            : Oleacc.CHILDID_SELF;

        if (id == DISP_E_PARAMNOTFOUND)
        {
            id = Oleacc.CHILDID_SELF;
        }

        return id;
    }

    public virtual int GetChildCount() => 0;

    /// <summary>
    ///  The IAccessible::get_accChildCount method retrieves the number of children that belong to this object.
    ///  All objects must support this property.
    /// </summary>
    int IAccessible.accChildCount
    {
        get
        {
            int count = GetChildCount();
            Trace($"ChildCount {count}");
            return count;
        }
    }

    // Difference from the GetChild method is that this method can return a simple element index.
    protected virtual bool IsChildElement(int childId) => false;

    // TODO: if the child is not an accessible object, but an element, it should be represented by the parent and
    // this method should throw COMException(S_FALSE)
    // https://learn.microsoft.com/windows/win32/api/oleacc/nf-oleacc-iaccessible-get_accchild#return-value
    object? IAccessible.get_accChild(object childID)
    {
        int id = ChildIdToInt(childID);
        object? child;
        if (id == Oleacc.CHILDID_SELF)
        {
            child = this;
            Trace($"Child self {child?.ToString() ?? "<null>"}");
        }
        else if (IsChildElement(id))
        {
            Trace($"Child {id} is a known element.");
        }

        return null;
//        else if (IsChildElement(id))
//        {
//            Trace($"Child {id} is a known element.");

//            var e = new COMException("This is a simple element.", errorCode: (int)HResult.S_FALSE)
//            {
//                HResult = (int)HResult.S_FALSE
//            };
//            throw e;
//        }

//        Trace($"Invalid child id {childID ?? "<null>"}");
        
//        // E_INVALIDARG
//        // https://learn.microsoft.com/dotnet/framework/interop/how-to-map-hresults-and-exceptions
//#pragma warning disable CA2208
//        // Argument name matches that in the native IAccessible definition.
//        throw new ArgumentException($"Invalid child id {childID??"<null>"}.", "varChild");
//#pragma warning restore CA2208
    }

    /// <summary>
    ///  Gets the bounds of the accessible object, in screen coordinates.
    /// </summary>
    public virtual Rectangle Bounds => default;

    /// <summary>
    ///  Return the child object at the given screen coordinates.
    /// </summary>
    // TODO: object - because I want to return element id.
    public virtual object? HitTest(int x, int y)
    {
        return Bounds.Contains(x, y) ? Oleacc.CHILDID_SELF : null;
    }

    /// <devdoc>
    ///  The Win32 AccessibleObjectFromPoint() API finds IAccessible
    ///  
    ///   - Calls WindowFromPhysicalPoint() to get the HWND at the given point
    ///   - Calls GetAncestor(GA_ROOT) until there is no parent or the parent is the desktop
    ///   - Calls AccessibleObjectFromWindow(OBJID_WINDOW) on the found handle to get IAccessible
    ///   - Calls IAccessible::accHitTest() to find the return values
    /// </devdoc>
    object? IAccessible.accHitTest(int xLeft, int yTop)
    {
        object? hit = HitTest(xLeft, yTop);

        Trace($"HitTest {hit?.ToString() ?? "<null>"}");
        return hit;
    }

    /// <summary>
    ///  Gets the default action for an object.
    /// </summary>
    public virtual string? DefaultAction(int id) => null;

    string? IAccessible.get_accDefaultAction(object childID)
    {
        string? action = DefaultAction(ChildIdToInt(childID));
        Trace($"DefaultAction {action?.ToString() ?? "<null>"}");
        return action;
    }

    /// <summary>
    ///  Gets a description of the object's visual appearance to the user.
    /// </summary>
    public virtual string? Description(int id) => null;

    string? IAccessible.get_accDescription(object childID)
    {
        string? description = Description(ChildIdToInt(childID));
        Trace($"Description {description?.ToString() ?? "<null>"}");
        return description;
    }

    /// <summary>
    ///  Gets a description of what the object does or how the object is used.
    /// </summary>
    public virtual string? Help(int id) => null;

    string? IAccessible.get_accHelp(object childID)
    {
        string? help = Help(ChildIdToInt(childID));
        Trace($"Help {help?.ToString() ?? "<null>"}");
        return help;
    }

    /// <summary>
    ///  Gets an identifier for a Help topic and the path to the Help file associated with this accessible object.
    /// </summary>
    public virtual int GetHelpTopic(int id, out string? fileName)
    {
        fileName = default;
        return -1;
    }

    int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
    {
        int topicId = GetHelpTopic(ChildIdToInt(childID), out string? fileName);
        Trace($"GetHelpTopic {topicId}");

        pszHelpFile = topicId != -1 ? fileName : null;
        return topicId;
    }

    /// <summary>
    ///  Gets the object shortcut key or access key for an accessible object.
    /// </summary>
    public virtual string? KeyboardShortcut(int id) => null;

    string? IAccessible.get_accKeyboardShortcut(object childID)
    {
        string? keyboardShortcut = KeyboardShortcut(ChildIdToInt(childID));
        return keyboardShortcut;
    }

    /// <summary>
    ///  Gets or sets the object name.
    /// </summary>
    public virtual void SetName(int id, string? name) { }
    public virtual string? GetName(int id) => null;

    string? IAccessible.get_accName(object childID)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return null;
        }

        string? name = GetName(id);
        Trace($"Get Name {name?.ToString() ?? "<null>"}");
        return name;
    }

    void IAccessible.set_accName(object childID, string newName)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return;
        }

        SetName(id, newName);
        Trace($"Set Name to {newName?.ToString() ?? "<null>"}");
        return;
    }

    /// <summary>
    ///  The parent of the accessible object.
    /// </summary>
    /// <devdoc>
    ///  Returning <see cref="IAccessible"/> here instead of <see cref="BaseAccessible"/> to allow easier
    ///  adapter class implementations. Moving other virtuals to <see cref="IAccessible"/> is a little more
    ///  awkward as implementations are depending on hitting other virtual methods on returned objects.
    /// </devdoc>
    public virtual IAccessible? Parent => null;

    object? IAccessible.accParent
    {
        get
        {
            var parent = Parent;
            if (parent is not null)
            {
                Debug.Assert(parent != this);
                if (parent == this)
                {
                    // Returning self as parent can hang accessibility clients
                    parent = null;
                }
            }

            Trace($"Parent {parent?.ToString() ?? "<null>"}");
            return parent;
        }
    }

    /// <summary>
    ///  Gets the role of this accessible object.
    /// </summary>
    public virtual AccessibleRole? Role(int id) => AccessibleRole.None;

    object? IAccessible.get_accRole(object childID)
    {
        AccessibleRole? role = Role(ChildIdToInt(childID));

        Trace($"Role {role?.ToString() ?? "<null>"}");
        return role;
    }

    /// <summary>
    ///  Gets the state of this accessible object.
    /// </summary>
    public virtual AccessibleStates? State(int id) => AccessibleStates.None;

    object? IAccessible.get_accState(object childID)
    {
        object? state = State(ChildIdToInt(childID));
        Trace($"State {state?.ToString() ?? "<null>"}");
        return state;
    }

    /// <summary>
    ///  Gets the currently selected child.
    /// </summary>
    public virtual object[]? GetSelected()
    {
        return null;
    }

    object? IAccessible.accSelection
    {
        get
        {
            object? selection = GetSelected();
            return selection;
        }
    }

    /// <summary>
    ///  Gets or sets the value of an accessible object.
    /// </summary>
    public virtual string? GetValue(int id) => string.Empty;
    public virtual void SetValue(int id, string? newValue) { }

    string? IAccessible.get_accValue(object childID)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return null;
        }

        string? value = GetValue(id);
        Trace($"Get Value {value?.ToString() ?? "<null>"}");
        return value;
    }

    void IAccessible.set_accValue(object childID, string newValue)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return;
        }

        SetValue(id, newValue);
    }

    /// <summary>
    ///  Selects this accessible object.
    /// </summary>
    public virtual void Select(AccessibleSelection flags, int id) { }

    void IAccessible.accSelect(int flagsSelect, object childID)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return;
        }

        Select((AccessibleSelection)flagsSelect, id);
    }

    /// <summary>
    ///  Performs the default action associated with this accessible object.
    /// </summary>
    public virtual void DoDefaultAction(int id) { }

    void IAccessible.accDoDefaultAction(object childID)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return;
        }

        DoDefaultAction(id);
    }

    /// <summary>
    ///  Gets the object that has the keyboard focus.
    /// </summary>
    public virtual object? GetFocused()
    {
        return null;
    }

    object? IAccessible.accFocus
    {
        get
        {
            object? focused = GetFocused();
            Trace($"Focused {focused?.ToString() ?? "<null>"}");
            return focused;
        }
    }

    public virtual void GetChildLocation(
        out int left,
        out int top,
        out int width,
        out int height,
        int id)
    {
        left = 0;
        top = 0;
        width = 0;
        height = 0;
    }

    void IAccessible.accLocation(
        out int pxLeft,
        out int pyTop,
        out int pcxWidth,
        out int pcyHeight,
        object childID)
    {
        pxLeft = 0;
        pyTop = 0;
        pcxWidth = 0;
        pcyHeight = 0;

        int id = ChildIdToInt(childID);
        if (id == Oleacc.CHILDID_SELF) 
        {
            Rectangle bounds = Bounds;
            pxLeft = bounds.X;
            pyTop = bounds.Y;
            pcxWidth = bounds.Width;
            pcyHeight = bounds.Height;
            Trace($"Location {bounds}");

            return;
        }

        if (IsChildElement(id))
        {
            GetChildLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, id);
        }

        return;
    }

    /// <summary>
    ///  When overridden in a derived class, navigates to another object.
    /// </summary>
    public virtual object? Navigate(AccessibleNavigation direction, int id)
    {
        return null;
    }

    object? IAccessible.accNavigate(int navDir, object childID)
    {
        int id = ChildIdToInt(childID);
        if (id != Oleacc.CHILDID_SELF && !IsChildElement(id))
        {
            return null;
        }

        // Note that per the documentation S_FALSE (1) should be returned if an element isn't found. As we're using
        // the public IAccessible definition we can't explicitly do this. Throwing a COMException with the right
        // errorCode might do this, but haven't validated.
        object? result = Navigate((AccessibleNavigation)navDir, id);
        Trace($"Navigate ({Enum.GetName<AccessibleNavigation>((AccessibleNavigation)navDir)} {result?.ToString() ?? "<null>"}");

        return result;
    }

    [Conditional("TRACE_ACCESSIBLE")]
    protected void Trace(string message)
        => Debug.WriteLine($"{ToString()}: {message}");

    public override string ToString() => $"{GetType().Name}";

    private string DebuggerDisplay
        => $"{ToString()}";
}
