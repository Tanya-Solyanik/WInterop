// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#define TRACE_ACCESSIBLE

using Accessibility;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WInterop.Accessibility.Native;

namespace WInterop.Accessibility;

/// <summary>
///  <see cref="IAccessible"/> base class. Simplified version of <see cref="AccessibleObject"/> that only
///  provides the <see cref="IAccessible"/> interface.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal class BaseAccessible : IAccessible
{
    private const int DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004);

    [AllowNull]
    private AccessibleObject _accessibleObject;

    // This trick is not used because we return our internal type.

    // Get the constructor (currently internal) that allows wrapping AccessibleObject around IAccessible.
    private static readonly ConstructorInfo s_accessibleConstructor
        = typeof(AccessibleObject).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            // Default binder
            binder: null,
            new[] { typeof(IAccessible) },
            modifiers: null)
        ?? throw new InvalidOperationException($"Constructor for {nameof(AccessibleObject)} not available.");

    /// <summary>
    ///  Pass our experimental implementation of IAccessible into the .NET's Accessible object wrapper.
    /// </summary>
    /// <returns>The framework wrapper.</returns>
    [MemberNotNull(nameof(_accessibleObject))]
    public AccessibleObject AsAccessibleObject()
        => _accessibleObject ??= (AccessibleObject)s_accessibleConstructor.Invoke(new[] { (object)this });

    public virtual int GetChildCount() => -1;

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

    /// <summary>
    ///  When overridden in a derived class, gets the accessible child corresponding to the specified identifier.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Unlike <see cref="AccessibleObject"/> we do not decrement the identifier and treat it like an index.
    ///  </para>
    ///  <para>
    ///   Note that the default <see cref="Control.ControlAccessibleObject"/> does NOT override this so it does not
    ///   support navigating children by index. <see cref="ListBox"/>, <see cref="TabControl"/> and other "indexable"
    ///   controls implement this pattern.
    ///  </para>
    /// </remarks>
    public virtual BaseAccessible? GetChild(int childId) => null;

    // TODO: if the child is not an accessible object, but an element, it should be represented by the parent and
    // this method should throw COMException(S_FALSE)
    // https://learn.microsoft.com/windows/win32/api/oleacc/nf-oleacc-iaccessible-get_accchild#return-value
    object? IAccessible.get_accChild(object childID)
    {
        var child = GetAccessibleObject(childID);
        Trace($"Child {child?.ToString() ?? "<null>"}");
        return child;
    }

    /// <summary>
    ///  Gets the bounds of the accessible object, in screen coordinates.
    /// </summary>
    public virtual Rectangle Bounds => default;

    /// <summary>
    ///  Return the child object at the given screen coordinates.
    /// </summary>
    public virtual BaseAccessible? HitTest(int x, int y)
    {
        // If we have children, return the first one successfully hit tested.
        int count = GetChildCount();
        if (count >= 0)
        {
            for (int index = 0; index < count; ++index)
            {
                if (GetChild(index)?.HitTest(x, y) is { } accessible)
                {
                    return accessible;
                }
            }
        }

        return Bounds.Contains(x, y) ? this : null;
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
        object? hit = null;

        if (HitTest(xLeft, yTop) is { } accessible)
        {
            hit = AsVariant(accessible);
        }

        Trace($"HitTest {hit?.ToString() ?? "<null>"}");
        return hit;
    }

    /// <summary>
    ///  Returns the given accessible object or the identifier for "self" if accessible is this instance.
    /// </summary>
    private object? AsVariant(IAccessible? accessible) => accessible == this ? Oleacc.CHILDID_SELF : accessible;

    /// <summary>
    ///  Gets the default action for an object.
    /// </summary>
    public virtual string? DefaultAction => null;

    string? IAccessible.get_accDefaultAction(object childID)
    {
        string? action = GetAccessibleObject(childID)?.DefaultAction;
        Trace($"DefaultAction {action?.ToString() ?? "<null>"}");
        return action;
    }

    /// <summary>
    ///  Gets a description of the object's visual appearance to the user.
    /// </summary>
    public virtual string? Description => null;

    string? IAccessible.get_accDescription(object childID)
    {
        string? description = GetAccessibleObject(childID)?.Description;
        Trace($"Description {description?.ToString() ?? "<null>"}");
        return description;
    }

    /// <devdoc>
    ///  This function convertes different child ID values to several expected ones,
    ///  must be the first function to call by the rest of the IAccessible methods. the
    /// </devdoc>
    private BaseAccessible? GetAccessibleObject(object childId)
    {
        int id = childId is int @int
            ? @int
            : Oleacc.CHILDID_SELF;

        if (id == DISP_E_PARAMNOTFOUND)
        {
            id = Oleacc.CHILDID_SELF;
        }

        if (id == Oleacc.CHILDID_SELF)
        {
            return this;
        }

        // TODO: AccessibleObject's default behavior is to treat childId as a 0-based index.
        return GetChild(id);
    }

    /// <summary>
    ///  Gets a description of what the object does or how the object is used.
    /// </summary>
    public virtual string? Help => null;

    string? IAccessible.get_accHelp(object childID)
    {
        string? help = GetAccessibleObject(childID)?.Help;
        Trace($"Help {help?.ToString() ?? "<null>"}");
        return help;
    }

    /// <summary>
    ///  Gets an identifier for a Help topic and the path to the Help file associated with this accessible object.
    /// </summary>
    public virtual int GetHelpTopic(out string? fileName)
    {
        fileName = default;
        return -1;
    }

    int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
    {
        if (GetAccessibleObject(childID) is { } accessible)
        {
            int result = GetHelpTopic(out pszHelpFile);
            return result;
        }

        pszHelpFile = null;
        return -1;
    }

    /// <summary>
    ///  Gets the object shortcut key or access key for an accessible object.
    /// </summary>
    public virtual string? KeyboardShortcut => null;

    string? IAccessible.get_accKeyboardShortcut(object childID)
    {
        string? shortcut = GetAccessibleObject(childID)?.KeyboardShortcut;
        return shortcut;
    }

    /// <summary>
    ///  Gets or sets the object name.
    /// </summary>
    public virtual string? Name { get; set; }

    string? IAccessible.get_accName(object childID)
    {
        // Fall back to this.Name if we can't find the child object.
        string? name = GetAccessibleObject(childID) is { } accessible ? accessible.Name : Name;

        Trace($"Get Name {name?.ToString() ?? "<null>"}");
        return name;
    }

    void IAccessible.set_accName(object childID, string newName)
    {
        var accessible = GetAccessibleObject(childID);
        if (accessible is not null)
        {
            accessible.Name = newName;
        }
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
    public virtual AccessibleRole Role => AccessibleRole.None;

    object? IAccessible.get_accRole(object childID)
    {
        object? role = null;
        var accessible = GetAccessibleObject(childID);
        if (accessible is not null)
        {
            role = (int)accessible.Role;
        }

        Trace($"Role {role?.ToString() ?? "<null>"}");
        return role;
    }

    /// <summary>
    ///  Gets the state of this accessible object.
    /// </summary>
    public virtual AccessibleStates State => AccessibleStates.None;

    object? IAccessible.get_accState(object childID)
    {
        object? state = null;
        var accessible = GetAccessibleObject(childID);
        if (accessible is not null)
        {
            state = (int)accessible.State;
        }

        Trace($"State {state?.ToString() ?? "<null>"}");
        return state;
    }

    /// <summary>
    ///  Gets the currently selected child.
    /// </summary>
    public virtual BaseAccessible? GetSelected()
    {
        int count = GetChildCount();
        if (count >= 0)
        {
            for (int index = 0; index < count; ++index)
            {
                var child = GetChild(index);
                if (child is not null && child.State.HasFlag(AccessibleStates.Selected))
                {
                    return child;
                }
            }

            if (State.HasFlag(AccessibleStates.Selected))
            {
                return this;
            }
        }

        return null;
    }

    object? IAccessible.accSelection
    {
        get
        {
            object? selection = null;
            if (GetSelected() is { } accessible)
            {
                selection = AsVariant(accessible);
            }

            Trace($"Selection {selection?.ToString() ?? "<null>"}");
            return selection;
        }
    }

    /// <summary>
    ///  Gets or sets the value of an accessible object.
    /// </summary>
    public virtual string? Value
    {
        get => string.Empty;
        set { }
    }

    string? IAccessible.get_accValue(object childID)
    {
        string? value = GetAccessibleObject(childID)?.Value;
        Trace($"Get Value {value?.ToString() ?? "<null>"}");
        return value;
    }

    void IAccessible.set_accValue(object childID, string newValue)
    {
        var accessible = GetAccessibleObject(childID);
        if (accessible is not null)
        {
            accessible.Value = newValue;
        }
    }

    /// <summary>
    ///  Selects this accessible object.
    /// </summary>
    public virtual void Select(AccessibleSelection flags) { }

    void IAccessible.accSelect(int flagsSelect, object childID)
    {
        if (GetAccessibleObject(childID) is { } accessible)
        {
            accessible.Select((AccessibleSelection)flagsSelect);
        }
    }

    /// <summary>
    ///  Performs the default action associated with this accessible object.
    /// </summary>
    public virtual void DoDefaultAction() { }

    void IAccessible.accDoDefaultAction(object childID)
    {
        if (GetAccessibleObject(childID) is { } accessible)
        {
            accessible.DoDefaultAction();
        }
    }

    /// <summary>
    ///  Gets the object that has the keyboard focus.
    /// </summary>
    public virtual BaseAccessible? GetFocused()
    {
        if (GetChildCount() >= 0)
        {
            int count = GetChildCount();
            for (int index = 0; index < count; ++index)
            {
                var child = GetChild(index);
                if (child is not null && child.State.HasFlag(AccessibleStates.Focused))
                {
                    return child;
                }
            }

            if (State.HasFlag(AccessibleStates.Focused))
            {
                return this;
            }
        }

        return null;
    }

    object? IAccessible.accFocus
    {
        get
        {
            object? focused = null;
            if (GetFocused() is { } accessible)
            {
                focused = AsVariant(accessible);
            }

            Trace($"Focused {focused?.ToString() ?? "<null>"}");
            return focused;
        }
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

        if (GetAccessibleObject(childID) is { } accessible)
        {
            Rectangle bounds = Bounds;
            pxLeft = bounds.X;
            pyTop = bounds.Y;
            pcxWidth = bounds.Width;
            pcyHeight = bounds.Height;
            Trace($"Location {bounds}");
        }
    }

    /// <summary>
    ///  When overridden in a derived class, navigates to another object.
    /// </summary>
    public virtual BaseAccessible? Navigate(AccessibleNavigation direction)
    {
        int count = GetChildCount();

        if (count >= 0)
        {
            switch (direction)
            {
                case AccessibleNavigation.FirstChild:
                    return GetChild(0);
                case AccessibleNavigation.LastChild:
                    //TOOO(TanyaSo) - GetChild does not process -1
                    return GetChild(count - 1);
            }
        }

        return null;
    }

    object? IAccessible.accNavigate(int navDir, object childID)
    {
        // Note that per the documentation S_FALSE (1) should be returned if an element isn't found. As we're using
        // the public IAccessible definition we can't explicitly do this. Throwing a COMException with the right
        // errorCode might do this, but haven't validated.
        object? result = null;

        if (GetAccessibleObject(childID) is { } startObject
            && Navigate((AccessibleNavigation)navDir) is { } accessible)
        {
            result = AsVariant(accessible);
        }

        Trace($"Navigate ({navDir}) {result?.ToString() ?? "<null>"}");

        return result;
    }

    [Conditional("TRACE_ACCESSIBLE")]
    private void Trace(string message)
        => Debug.WriteLine($"{ToString()}: {message}");

    public override string ToString() => $"{GetType().Name}";

    private string DebuggerDisplay
        => $"{ToString()}";
}
