// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Windows;

namespace FormWithButton;

internal class ScratchButtonClass : BaseWindowClass
{
    public ScratchButtonClass()
        : base(registeredClassName: "BUTTON")
    {
    }

    protected override LResult WindowProcedure(WindowHandle window, MessageType message, WParam wParam, LParam lParam)
    {
        switch (message)
        {
            case MessageType.Create:
                _window = window;
                break;
            case MessageType.GetObject:
                return 0;
        }

        // Let the base class handle any other messages.
        return base.WindowProcedure(window, message, wParam, lParam);
    }
}
