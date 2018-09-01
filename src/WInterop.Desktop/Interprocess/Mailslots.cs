﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using WInterop.Errors;
using WInterop.Interprocess.Unsafe;

namespace WInterop.Interprocess
{
    public static partial class Mailslots
    {
        /// <summary>
        /// Create a mailslot.
        /// </summary>
        /// <param name="name">Name of the mailslot.</param>
        /// <param name="maxMessageSize">Maximum size, in bytes, of messages that can be posted to the mailslot. 0 means any size.</param>
        /// <param name="readTimeout">
        /// Timeout, in milliseconds, that a read operation will wait for a message to be posted. 0 means do not wait, uint.MaxValue means
        /// wait indefinitely.
        /// </param>
        public static SafeMailslotHandle CreateMailslot(string name, uint maxMessageSize = 0, uint readTimeout = 0)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            SafeMailslotHandle handle;

            unsafe
            {
                handle = Imports.CreateMailslotW(name, maxMessageSize, readTimeout, null);
            }

            if (handle.IsInvalid)
                throw Error.GetIoExceptionForLastError(name);

            return handle;
        }

        /// <summary>
        /// Get information for the given mailslot.
        /// </summary>
        public static MailslotInfo GetMailslotInfo(SafeMailslotHandle mailslotHandle)
        {
            MailslotInfo info = new MailslotInfo();

            unsafe
            {
                if (!Imports.GetMailslotInfo(
                    hMailslot: mailslotHandle,
                    lpMaxMessageSize: &info.MaxMessageSize,
                    lpNextSize: &info.NextSize,
                    lpMessageCount: &info.MessageCount,
                    lpReadTimeout: &info.ReadTimeout))
                    throw Error.GetIoExceptionForLastError();
            }

            return info;
        }

        /// <summary>
        /// Set the given mailslot's read timeout.
        /// </summary>
        /// <param name="readTimeout">Timeout for read operations in milliseconds. Set to uint.MaxValue for infinite timeout.</param>
        public static void SetMailslotTimeout(SafeMailslotHandle mailslotHandle, uint readTimeout)
        {
            if (!Imports.SetMailslotInfo(mailslotHandle, readTimeout))
                throw Error.GetIoExceptionForLastError();
        }
    }
}