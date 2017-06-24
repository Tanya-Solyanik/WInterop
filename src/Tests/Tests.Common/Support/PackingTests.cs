﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.Runtime.InteropServices;
using Xunit;

namespace Tests.Support
{
    public class PackingTests
    {
        // https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.structlayoutattribute.pack.aspx

        // Type alignment is the size of it's largest element
        // Fields must align with fields of their own size or the type alignment, whichever is smaller

        private struct IntLong
        {
            public int Int;
            public long Long;
        }

        private struct IntInt
        {
            public int Int1;
            public int Int2;
        }


        // typedef union _LARGE_INTEGER {
        //     struct {
        //         DWORD LowPart;
        //         LONG HighPart;
        //     } DUMMYSTRUCTNAME;
        //     struct {
        //         DWORD LowPart;
        //         LONG HighPart;
        //     } u;
        //     LONGLONG QuadPart;
        // } LARGE_INTEGER;
        [StructLayout(LayoutKind.Explicit)]
        private struct LARGE_INTEGER
        {
            [FieldOffset(0)]
            public uint LowPart;

            [FieldOffset(sizeof(uint))]
            public int HighPart;

            [FieldOffset(0)]
            public long QuadPart;
        }

        private struct IntLargeInt
        {
            public int Int;
            public LARGE_INTEGER LargeInt;
        }

        [Fact]
        public unsafe void UnionsAlignOnLargestType()
        {
            IntLargeInt i = new IntLargeInt();
            void* start = &i;
            void* largeInt = &i.LargeInt;
            ((ulong)largeInt - (ulong)start).Should().Be(8u);
        }
    }
}
