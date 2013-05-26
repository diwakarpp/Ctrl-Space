﻿using System.Runtime.InteropServices;

namespace Ctrl_Space.Input.Devices.RawStructures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RawKeyboard
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort MakeCode;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Flags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort Reserved;
        [MarshalAs(UnmanagedType.U2)]
        public ushort VKey;
        [MarshalAs(UnmanagedType.U4)]
        public uint Message;
        [MarshalAs(UnmanagedType.U4)]
        public uint ExtraInformation;
    }
}
