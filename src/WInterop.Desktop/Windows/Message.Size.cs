﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WInterop.Windows
{
    public static partial class Message
    {
        public readonly ref struct Size
        {
            public System.Drawing.Size NewSize { get; }
            public SizeType SizeType { get;  }

            public Size(WPARAM wParam, LPARAM lParam)
            {
                NewSize = new System.Drawing.Size(lParam.LowWord, lParam.HighWord);
                SizeType = (SizeType)(int)wParam;
            }
        }
    }
}