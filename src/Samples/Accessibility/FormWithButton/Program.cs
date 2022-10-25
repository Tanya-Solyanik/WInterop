// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using WInterop.Gdi;
using WInterop.Windows;
using System.Drawing;

namespace FormWithButton;

internal class Program
{
    [STAThread]
    private static void Main()
    {
        Windows.CreateMainWindowAndRun(
            new ScratchWindowClass(),
            bounds: new Rectangle(100, 100, 500, 200),
            windowTitle: "Scratch");
    }
}
