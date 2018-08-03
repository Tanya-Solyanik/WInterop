﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Drawing;
using WInterop.Gdi;
using WInterop.Windows;

namespace Beeper
{

    class Beeper1 : WindowClass
    {
        bool fFlipFlop = false;
        const int ID_TIMER = 1;

        protected override LRESULT WindowProcedure(WindowHandle window, WindowMessage message, WPARAM wParam, LPARAM lParam)
        {
            switch (message)
            {
                case WindowMessage.Create:
                    window.SetTimer(ID_TIMER, 1000);
                    return 0;
                case WindowMessage.Timer:
                    Windows.MessageBeep();
                    fFlipFlop = !fFlipFlop;
                    window.Invalidate();
                    return 0;
                case WindowMessage.Paint:
                    using (DeviceContext dc = window.BeginPaint())
                    {
                        using (BrushHandle brush = Gdi.CreateSolidBrush(fFlipFlop ? Color.Red : Color.Blue))
                        {
                            dc.FillRectangle(window.GetClientRectangle(), brush);
                        }
                    }
                    return 0;
                case WindowMessage.Destroy:
                    window.KillTimer(ID_TIMER);
                    break;
            }

            return base.WindowProcedure(window, message, wParam, lParam);
        }
    }
}