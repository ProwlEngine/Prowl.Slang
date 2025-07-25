// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System;

namespace Prowl.Slang.Native;


[UUID(0x87ede0e1, 0x4852, 0x44b0, 0x8b, 0xf2, 0xcb, 0x31, 0x87, 0x4d, 0xe2, 0x39)]
internal unsafe interface ISlangCastable : IUnknown
{
    void* CastAs(ref Guid guid);
}
