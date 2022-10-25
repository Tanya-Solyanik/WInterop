// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using WInterop.Accessibility;
using WInterop.Windows;
using Message = WInterop.Windows.Message;

namespace FormWithButton;

// https://devblogs.microsoft.com/oldnewthing/20030723-00
/// <summary>
///  This class simulated a form with a button.
/// </summary>
internal class ScratchWindowClass : BaseWindowClass
{
    /// <summary>
    ///  Child class that simulated a button.
    /// </summary>
    protected WindowHandle ChildWindow { get; set; }

    private const int ButtonId = 123;

    public ScratchWindowClass()
        : base(className: nameof(ScratchWindowClass), moduleInstance: default)
    {
    }
    
    /// <summary>
    ///  Applications will typically override this and maybe even create a child window.
    /// </summary>
    public virtual bool OnCreate(WindowHandle window, Message.Create message)
    {
        _window = window;

        ScratchButtonClass child = new();

        ChildWindow = child.CreateWindow(
            new Rectangle(30, 20, 200, 50),
            windowName: "Push me!",
            style: WindowStyles.Overlapped |
                   WindowStyles.TabStop |
                   WindowStyles.Visible |
                   WindowStyles.Child |
                   (WindowStyles)ButtonStyles.DefaultPushButton,
            parentWindow: window,
            menuHandle: (MenuHandle)ButtonId);

        return true;
    }

    protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
    {
        switch (message)
        {
            case MessageType.Create:
                OnCreate(window, new Message.Create(lParam));
                break;
            case MessageType.Command:
                switch (wParam.LowWord)
                {
                    case ButtonId:
                        Debug.WriteLine("Button pushed.");
                        return 0;
                }
                break;
        }

        // Let the base class handle any other messages
        return base.WindowProcedure(window, message, wParam, lParam);
    }

    protected override BaseAccessible CreateClientAccessible()
        => _clientAccessible ??= new ScratchWindowAccessible(this);
}
