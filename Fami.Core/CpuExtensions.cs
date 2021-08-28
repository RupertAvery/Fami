using System.Runtime.CompilerServices;

namespace Fami.Core
{
    public static class CpuExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadImmediate(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadZeroPage(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Read(cpu.PC + 1);
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteZeroPage(this Cpu6502State cpu, byte value)
        {
            cpu.Write(cpu.EffectiveAddr, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadZeroPageX(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = (cpu.Read(cpu.PC + 1) + cpu.X) % 0x100;
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadZeroPageY(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = (cpu.Read(cpu.PC + 1) + cpu.Y) % 0x100;
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadAbsolute(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadAbsoluteX(this Cpu6502State cpu)
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

            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadAbsoluteY(this Cpu6502State cpu)
        {
            var basemem = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            var basememPage = (basemem >> 8) & 0xff;
            cpu.EffectiveAddr = basemem + cpu.Y;
            var effAddPage = (cpu.EffectiveAddr >> 8) & 0xff;

            //var x = cpu.PC >> 8;
            //var y = cpu.EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                cpu.PageBoundsCrossed = true;
            }


            return cpu.Read(cpu.EffectiveAddr & 0xFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadRelative(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
            return (sbyte)cpu.Read(cpu.EffectiveAddr);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadIndirect(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            cpu.EffectiveAddr = cpu.Read(arg) + cpu.Read(arg + 1) * 0x100;
            return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadIndirect_JMP(this Cpu6502State cpu)
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
            return cpu.Read(cpu.EffectiveAddr);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadIndirectX(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1);
            var ix = arg + cpu.X;
            cpu.EffectiveAddr = cpu.Read(ix % 0x100) + cpu.Read((ix + 1) % 0x100) * 0x100;
            return cpu.Read(cpu.EffectiveAddr);


            //var arg = cpu.Read(cpu.PC + 1) + cpu.Read(cpu.PC + 2) * 0x100;
            //var ix = arg + cpu.X;
            //cpu.EffectiveAddr = cpu.Read(ix % 0x100) + cpu.Read((ix + 1) % 0x100) * 0x100;
            //return cpu.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ReadIndirectY(this Cpu6502State cpu)
        {
            var arg = cpu.Read(cpu.PC + 1);
            var basemem = cpu.Read(arg) + cpu.Read((arg + 1) % 0x100) * 0x100;
            var basememPage = (basemem >> 8) & 0xff;
            cpu.EffectiveAddr = basemem + cpu.Y;
            var effAddPage = (cpu.EffectiveAddr >> 8) & 0xff;

            //var x = cpu.PC >> 8;
            //var y = cpu.EffectiveAddr >> 8;

            if (basememPage != effAddPage)
            {
                cpu.PageBoundsCrossed = true;
            }

            return cpu.Read(cpu.EffectiveAddr & 0xFFFF);
        }


        //public static byte GetStatusFlags(this Cpu6502State cpu)
        //{
        //    return (byte)(
        //        (cpu.N ? 1 << 7 : 0) +
        //        (cpu.V ? 1 << 6 : 0) +
        //        (cpu.D ? 1 << 3 : 0) +
        //        (cpu.I ? 1 << 2 : 0) +
        //        (cpu.Z ? 1 << 1 : 0) +
        //        (cpu.C ? 1 << 0 : 0)
        //    );
        //}

        //public static void SetStatusFlags(this Cpu6502State cpu, byte value)
        //{
        //    cpu.N = (value >> 7 & 1) == 1;
        //    cpu.V = (value >> 6 & 1) == 1;
        //    cpu.D = (value >> 3 & 1) == 1;
        //    cpu.I = (value >> 2 & 1) == 1;
        //    cpu.Z = (value >> 1 & 1) == 1;
        //    cpu.C = (value >> 0 & 1) == 1;
        //}
    }
}