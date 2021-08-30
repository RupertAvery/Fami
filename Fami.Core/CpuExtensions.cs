using System.Runtime.CompilerServices;

namespace Fami.Core
{
    public static class CpuExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeImmediate(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeZeroPage(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Read(cpu.PC + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeZeroPageX(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = (cpu.Read(cpu.PC + 1) + cpu.X) % 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeZeroPageY(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = (cpu.Read(cpu.PC + 1) + cpu.Y) % 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeAbsolute(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeAbsoluteX(this Cpu6502State cpu)
        {
            var basemem = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            var basememPage = (basemem >> 8) & 0xff;
            cpu.EffectiveAddr = basemem + cpu.X;
            var effAddPage = (cpu.EffectiveAddr >> 8) & 0xff;

            //var x = cpu.PC >> 8;
            //var y = cpu.EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                cpu.PageBoundsCrossed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeAbsoluteY(this Cpu6502State cpu)
        {
            var basemem = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            var basememPage = (basemem >> 8) & 0xFF;
            cpu.EffectiveAddr = (basemem + cpu.Y) & 0xFFFF;
            var effAddPage = (cpu.EffectiveAddr >> 8) & 0xFF;

            //var x = cpu.PC >> 8;
            //var y = cpu.EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                cpu.PageBoundsCrossed = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeRelative(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeIndirect(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            cpu.EffectiveAddr = cpu.Read(arg) + cpu.Read(arg + 1) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeIndirect_JMP(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1);
            arg += cpu.Read(cpu.PC + 2) * 0x100;
            if ((arg & 0xFF) == 0xFF)
            {
                cpu.EffectiveAddr = cpu.Read(arg) + cpu.Read(arg + 1 - 0x100) * 0x100;
            }
            else
            {
                cpu.EffectiveAddr = cpu.Read(arg) + cpu.Read(arg + 1) * 0x100;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeIndirectX(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1);
            var ix = arg + cpu.X;
            cpu.EffectiveAddr = cpu.Read(ix % 0x100) + cpu.Read((ix + 1) % 0x100) * 0x100;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddrModeIndirectY(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1);
            var basemem = cpu.Read(arg) + cpu.Read((arg + 1) % 0x100) * 0x100;
            var basememPage = (basemem >> 8) & 0xFF;
            cpu.EffectiveAddr = (basemem + cpu.Y) & 0xFFFF;
            var effAddPage = (cpu.EffectiveAddr >> 8) & 0xff;

            if (basememPage != effAddPage)
            {
                cpu.PageBoundsCrossed = true;
            }
        }

    }
}