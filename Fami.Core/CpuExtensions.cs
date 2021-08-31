﻿using System.Runtime.CompilerServices;

namespace Fami.Core
{
    public partial class Cpu6502State
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeImmediate()
        {
            EffectiveAddr = PC + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeZeroPage()
        {
            EffectiveAddr = BusRead(PC + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeZeroPageX()
        {
            EffectiveAddr = (BusRead(PC + 1) + X) % 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeZeroPageY()
        {
            EffectiveAddr = (BusRead(PC + 1) + Y) % 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeAbsolute()
        {
            EffectiveAddr = BusRead(PC + 1) + BusRead(PC + 2) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeAbsoluteX()
        {
            var basemem = BusRead(PC + 1) + BusRead(PC + 2) * 0x100;
            var basememPage = (basemem >> 8) & 0xff;
            EffectiveAddr = basemem + X;
            var effAddPage = (EffectiveAddr >> 8) & 0xff;

            //var x = PC >> 8;
            //var y = EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                PageBoundsCrossed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeAbsoluteY()
        {
            var basemem = BusRead(PC + 1) + BusRead(PC + 2) * 0x100;
            var basememPage = (basemem >> 8) & 0xFF;
            EffectiveAddr = (basemem + Y) & 0xFFFF;
            var effAddPage = (EffectiveAddr >> 8) & 0xFF;

            //var x = PC >> 8;
            //var y = EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                PageBoundsCrossed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeRelative()
        {
            EffectiveAddr = PC + 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeIndirect()
        {
            var arg = BusRead(PC + 1) + BusRead(PC + 2) * 0x100;
            EffectiveAddr = BusRead(arg) + BusRead(arg + 1) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeIndirect_JMP()
        {
            var arg = BusRead(PC + 1);
            arg += BusRead(PC + 2) * 0x100;
            if ((arg & 0xFF) == 0xFF)
            {
                EffectiveAddr = BusRead(arg) + BusRead(arg + 1 - 0x100) * 0x100;
            }
            else
            {
                EffectiveAddr = BusRead(arg) + BusRead(arg + 1) * 0x100;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeIndirectX()
        {
            var arg = BusRead(PC + 1);
            var ix = arg + X;
            EffectiveAddr = BusRead(ix % 0x100) + BusRead((ix + 1) % 0x100) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddrModeIndirectY()
        {
            var arg = BusRead(PC + 1);
            var basemem = BusRead(arg) + BusRead((arg + 1) % 0x100) * 0x100;
            var basememPage = (basemem >> 8) & 0xFF;
            EffectiveAddr = (basemem + Y) & 0xFFFF;
            var effAddPage = (EffectiveAddr >> 8) & 0xff;

            if (basememPage != effAddPage)
            {
                PageBoundsCrossed = true;
            }
        }

    }
}