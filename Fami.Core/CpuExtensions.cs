using System.Runtime.CompilerServices;

namespace Fami.Core
{
    public static class CpuExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadImmediate(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadZeroPage(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read(cpu.PC + 1);
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteZeroPage(this Cpu6502State cpu, byte value)
        {
            cpu.Memory.Write(cpu.EffectiveAddr, value);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadZeroPageX(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read((cpu.Memory.Read(cpu.PC + 1) + cpu.X) % 0x100);
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadZeroPageY(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read((cpu.Memory.Read(cpu.PC + 1) + cpu.Y) % 0x100);
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAbsolute(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC + 2) * 0x100;
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAbsoluteX(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC + 2) * 0x100 + cpu.X;

            var x = cpu.PC >> 8;
            var y = cpu.EffectiveAddr >> 8;

            if (x != y)
            {
                cpu.PageBoundsCrossed = true;
            }

            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadAbsoluteY(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC + 2) * 0x100 + cpu.Y;

            var x = cpu.PC >> 8;
            var y = cpu.EffectiveAddr >> 8;

            if (x != y)
            {
                cpu.PageBoundsCrossed = true;
            }


            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte ReadRelative(this Cpu6502State cpu)
        {
            cpu.EffectiveAddr = cpu.PC + 1;
            return (sbyte)cpu.Memory.Read(cpu.EffectiveAddr);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIndirect(this Cpu6502State cpu)
        {
            var arg = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC + 2) * 0x100;
            cpu.EffectiveAddr = arg;
            return arg;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIndirectX(this Cpu6502State cpu)
        {
            var arg = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC + 2) * 0x100;
            var ix = arg + cpu.X;
            cpu.EffectiveAddr = cpu.Memory.Read(ix % 0x100) + cpu.Memory.Read((ix + 1) % 0x100) * 0x100;
            return cpu.Memory.Read(cpu.EffectiveAddr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReadIndirectY(this Cpu6502State cpu)
        {
            var arg = cpu.Memory.Read(cpu.PC + 1) + cpu.Memory.Read(cpu.PC +2) * 0x100;
            var iy = arg;
            cpu.EffectiveAddr = cpu.Memory.Read(iy) + cpu.Memory.Read((iy + 1) % 0x100) * 0x100 + cpu.Y;

            var x = cpu.PC >> 8;
            var y = cpu.EffectiveAddr >> 8;

            if (x != y)
            {
                cpu.PageBoundsCrossed = true;
            }

            return cpu.Memory.Read(cpu.EffectiveAddr);
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