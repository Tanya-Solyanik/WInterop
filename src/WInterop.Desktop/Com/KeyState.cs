﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WInterop.Com
{
    public enum KeyState : uint
    {
        /// <summary>
        ///  [MK_LBUTTON]
        /// </summary>
        LeftButton          = 0x0001,

        /// <summary>
        ///  [MK_RBUTTON]
        /// </summary>
        RightButton         = 0x0002,

        /// <summary>
        ///  [MK_SHIFT]
        /// </summary>
        Shift               = 0x0004,

        /// <summary>
        ///  [MK_CONTROL]
        /// </summary>
        Control             = 0x0008,

        /// <summary>
        ///  [MK_MBUTTON]
        /// </summary>
        MiddleButton        = 0x0010,

        /// <summary>
        ///  [MK_ALT]
        /// </summary>
        Alt                 = 0x0020
    }
}
