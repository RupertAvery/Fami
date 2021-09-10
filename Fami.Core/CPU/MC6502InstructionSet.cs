using System;
using System.Runtime.CompilerServices;

namespace Fami.Core.CPU
{

    public static partial class MC6502InstructionSet
    {
        const byte __ = 0;
        public const byte IMM = 0;
        public const byte ACC = 1;
        public const byte IMP = 2;
        public const byte IDX = 3;
        public const byte IDY = 4;
        public const byte DP_ = 5;
        public const byte DPX = 6;
        public const byte DPY = 7;
        public const byte ABS = 8;
        public const byte ABX = 9;
        public const byte ABY = 10;
        public const byte REL = 11;
        public const byte IND = 12;
        public const byte UNK = 0XFF;

        public static byte[] addrmodes =
        {
            // 00   01   02   03   04   05   06   07   08   09   0A   0B   0C   0D   0E   0F
               IMP, IDX, UNK, IDX, DP_, DP_, DP_, DP_, IMP, IMM, ACC, IMM, ABS, ABS, ABS, ABS, // 00
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // 10
               ABS, IDX, UNK, IDX, DP_, DP_, DP_, DP_, IMP, IMM, ACC, IMM, ABS, ABS, ABS, ABS, // 20
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // 30
               IMP, IDX, UNK, IDX, DP_, DP_, DP_, DP_, IMP, IMM, ACC, IMM, ABS, ABS, ABS, ABS, // 40
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // 50
               IMP, IDX, UNK, IDX, DP_, DP_, DP_, DP_, IMP, IMM, ACC, IMM, IND, ABS, ABS, ABS, // 60
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // 70
               IMM, IDX, IMM, IDX, DP_, DP_, DP_, DP_, IMP, IMM, IMP, IMM, ABS, ABS, ABS, ABS, // 80
               REL, IDY, UNK, IDY, DPX, DPX, DPY, DPY, IMP, ABY, IMP, ABY, ABX, ABX, ABY, ABY, // 90
               IMM, IDX, IMM, IDX, DP_, DP_, DP_, DP_, IMP, IMM, IMP, IMM, ABS, ABS, ABS, ABS, // A0
               REL, IDY, UNK, IDY, DPX, DPX, DPY, DPY, IMP, ABY, IMP, ABY, ABX, ABX, ABY, ABY, // B0
               IMM, IDX, IMM, IDX, DP_, DP_, DP_, DP_, IMP, IMM, IMP, IMM, ABS, ABS, ABS, ABS, // C0
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // D0
               IMM, IDX, IMM, IDX, DP_, DP_, DP_, DP_, IMP, IMM, IMP, IMM, ABS, ABS, ABS, ABS, // E0
               REL, IDY, UNK, IDY, DPX, DPX, DPX, DPX, IMP, ABY, IMP, ABY, ABX, ABX, ABX, ABX, // F0
        };

        public static uint[] cycles = new uint[]
        {
            // 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F
               07, 06, __, 08, 03, 03, 05, 05, 03, 02, 02, 02, 04, 04, 06, 06, // 00
               02, 05, __, 08, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // 10
               06, 06, __, 08, 03, 03, 05, 05, 04, 02, 02, 02, 04, 04, 06, 06, // 20
               02, 05, __, 08, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // 30
               06, 06, __, 08, 03, 03, 05, 05, 03, 02, 02, 02, 03, 04, 06, 06, // 40
               02, 05, __, 08, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // 50
               06, 06, __, 08, 03, 03, 05, 05, 04, 02, 02, 02, 05, 04, 06, 06, // 60
               02, 05, __, 08, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // 70
               02, 06, 02, 06, 03, 03, 03, 03, 02, 02, 02, 02, 04, 04, 04, 04, // 80
               02, 06, __, 06, 04, 04, 04, 04, 02, 05, 02, 05, 05, 05, 05, 05, // 90
               02, 06, 02, 06, 03, 03, 03, 03, 02, 02, 02, 02, 04, 04, 04, 04, // A0
               02, 05, __, 05, 04, 04, 04, 04, 02, 04, 02, 04, 04, 04, 04, 04, // B0
               02, 06, 02, 08, 03, 03, 05, 05, 02, 02, 02, 02, 04, 04, 03, 06, // C0
               02, 05, __, 08, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // D0
               02, 06, 02, 08, 03, 03, 05, 05, 02, 02, 02, 02, 04, 04, 06, 06, // E0
               02, 05, __, 04, 04, 04, 06, 06, 02, 04, 02, 07, 04, 04, 07, 07, // F0
        };

        // 0xFC = 04 cycles?

        public static uint[] bytes = new uint[]
        {
            // 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F
               01, 02, 01, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // 00
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // 10
               03, 02, 01, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // 20
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // 30
               01, 02, 01, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // 40
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // 50
               01, 02, 01, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // 60
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // 70
               02, 02, 02, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // 80
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // 90
               02, 02, 02, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // A0
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // B0
               02, 02, 02, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // C0
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // D0
               02, 02, 02, 02, 02, 02, 02, 02, 01, 02, 01, 02, 03, 03, 03, 03, // E0
               02, 02, 01, 02, 02, 02, 02, 02, 01, 03, 01, 03, 03, 03, 03, 03, // F0
        };


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestN(uint value, MC6502State cpu)
        {
            cpu.N = (value >> 7) & 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestV(uint value, MC6502State cpu)
        {
            cpu.V = (value >> 6) & 1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestZ(uint value, MC6502State cpu)
        {
            cpu.Z = (value == 0b00000000) ? 1U : 0U;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BRK(MC6502State cpu)
        {
            cpu.BusWrite(cpu.S, (byte)(cpu.PC & 0xff));
            cpu.BusWrite(cpu.S + 1, (byte)((cpu.PC >> 8) & 0xff));
            var flags = cpu.P | 0b00110100;
            cpu.BusWrite(cpu.S + 2, flags);
            cpu.S += 3;
            return 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ORA(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A = arg | cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AND(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A = arg & cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ASL(MC6502State cpu)
        {
            cpu.A <<= 1;
            cpu.C = (cpu.A >> 8) & 1;
            cpu.A &= 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ASL_Mem(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            arg = arg << 1;
            cpu.C = (arg >> 8) & 1;
            arg = arg & 0xFF;
            TestN(arg, cpu);
            TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PHP(MC6502State cpu)
        {
            cpu.Push(cpu.P | 0b00110000);
            //cpu.BusWrite(cpu.S + 0x100, flags);
            //cpu.S -= 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BPL(MC6502State cpu)
        {
            if (cpu.N == 0)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CLC(MC6502State cpu)
        {
            cpu.C = 0;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint JSR(MC6502State cpu)
        {
            // PC already points to the address AFTER the last byte of the instruction, so subtract 1
            var pc = cpu.PC - 1;
            cpu.Push((pc >> 8) & 0xFF);
            cpu.Push(pc & 0xFF);
            //cpu.BusWrite(0x100 + cpu.S, (pc >> 8) & 0xFF);
            //cpu.BusWrite(0x100 + cpu.S - 1, pc & 0xFF);
            //cpu.S -= 2;
            cpu.PC = cpu.EffectiveAddr;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BIT(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            TestN(arg, cpu);
            TestV(arg, cpu);
            TestZ(cpu.A & arg, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ROL(MC6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = (cpu.A >> 7) & 1;
            cpu.A = ((cpu.A << 1) | temp & 1) & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ROL_Mem(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.C;
            cpu.C = (arg >> 7) & 1;
            arg = ((arg << 1) | temp & 1) & 0xFF;
            TestN(arg, cpu);
            TestZ(arg, cpu);
            cpu.BusWrite(cpu.EffectiveAddr, arg);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PLP(MC6502State cpu)
        {
            //cpu.S += 1;
            //cpu.P = cpu.BusRead(cpu.S) | 0b00100000;
            //cpu.P = cpu.Pop() | 0b00100000;
            //cpu.P = cpu.Pop() | 0b00110000;
            cpu.P = cpu.Pop();
            cpu.B = 0;
            cpu.U = 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BMI(MC6502State cpu)
        {
            if (cpu.N == 1)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SEC(MC6502State cpu)
        {
            cpu.C = 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RTI(MC6502State cpu)
        {
            //The status register is pulled with the break flag and bit 5 ignored.
            cpu.P = cpu.Pop();
            cpu.B = 0;
            cpu.U = 1;   // FCEUX sets this bit to 1 on RTI?
            // Then PC is pulled from the stack.
            cpu.PC = cpu.Pop();
            cpu.PC += cpu.Pop() * 0x100;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint EOR(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A ^= arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LSR(MC6502State cpu)
        {
            cpu.C = cpu.A & 1;
            cpu.A = (cpu.A >> 1) & 0xFF;
            cpu.N = 0;
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LSR_Mem(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.C = arg & 1;
            arg = (arg >> 1) & 0xFF;
            cpu.N = 0;
            TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PHA(MC6502State cpu)
        {
            cpu.Push(cpu.A);
            //cpu.BusWrite(cpu.S, cpu.A);
            //cpu.S--;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint JMP(MC6502State cpu)
        {
            cpu.PC = cpu.EffectiveAddr;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BVC(MC6502State cpu)
        {
            if (cpu.V == 0)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CLI(MC6502State cpu)
        {
            cpu.I = 0;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RTS(MC6502State cpu)
        {
            cpu.PC = cpu.Pop();
            cpu.PC += cpu.Pop() * 0x100;

            //cpu.S += 1;
            //cpu.PC = cpu.BusRead(cpu.S + 0x100);
            //cpu.S += 1;
            //cpu.PC += cpu.BusRead(cpu.S + 0x100) * 0x100;
            cpu.PC++;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ADC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);

            var temp = arg + cpu.A + cpu.C;

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((((cpu.A ^ arg) & 0x80) ^ 0x80) & ((cpu.A ^ temp) & 0x80)) >> 7 & 1;
            cpu.C = (temp >> 8) & 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ROR(MC6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = cpu.A & 1;
            cpu.A = ((cpu.A >> 1) | temp << 7) & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ROR_Mem(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.C;
            cpu.C = arg & 1;
            arg = ((arg >> 1) | temp << 7) & 0xFF;
            TestN(arg, cpu);
            TestZ(arg, cpu);
            cpu.BusWrite(cpu.EffectiveAddr, arg);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PLA(MC6502State cpu)
        {
            cpu.A = cpu.Pop();
            //cpu.S += 1;
            //cpu.A = cpu.BusRead(cpu.S);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BVS(MC6502State cpu)
        {
            if (cpu.V == 1)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SEI(MC6502State cpu)
        {
            cpu.I = 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint STA(MC6502State cpu)
        {
            cpu.BusWrite(cpu.EffectiveAddr, cpu.A);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint STY(MC6502State cpu)
        {
            cpu.BusWrite(cpu.EffectiveAddr, cpu.Y);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint STX(MC6502State cpu)
        {
            cpu.BusWrite(cpu.EffectiveAddr, cpu.X);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DEY(MC6502State cpu)
        {
            cpu.Y -= 1;
            cpu.Y &= 0xFF;

            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TXA(MC6502State cpu)
        {
            cpu.A = cpu.X;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BCC(MC6502State cpu)
        {
            if (cpu.C == 0)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TYA(MC6502State cpu)
        {
            cpu.A = cpu.Y;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TXS(MC6502State cpu)
        {
            cpu.S = cpu.X;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LDY(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.Y = arg;
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LDA(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A = arg & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LDX(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.X = arg;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TAY(MC6502State cpu)
        {
            cpu.Y = cpu.A;
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TAX(MC6502State cpu)
        {
            cpu.X = cpu.A;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BCS(MC6502State cpu)
        {
            if (cpu.C == 1)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetRel(MC6502State cpu)
        {
            var rel = cpu.BusRead(cpu.EffectiveAddr);
            if ((rel & 0x80) == 0x80)
            {
                rel |= 0xFFFFFF80;
            }
            var x = cpu.PC >> 8;
            var y = (cpu.PC + (int)rel) >> 8;

            if (x != y)
            {
                cpu.PageBoundsCrossed = true;
            }
            var temp = cpu.PC + (int)rel;
            switch (temp)
            {
                case > 0xFFFF:
                    temp -= 0xFFFF;
                    break;
                case < 0:
                    temp += 0xFFFF;
                    break;
            }

            return (uint)(temp & 0xFFFF);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CLV(MC6502State cpu)
        {
            cpu.V = 0;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TSX(MC6502State cpu)
        {
            cpu.X = cpu.S & 0xFF;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CPY(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.Y - arg;
            cpu.C = cpu.Y >= arg ? 1U : 0;
            TestN(temp & 0xFFFF, cpu);
            TestZ(temp & 0xFFFF, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CMP(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.A - arg;
            cpu.C = cpu.A >= arg ? 1U : 0;
            TestN(temp & 0xFF, cpu);
            TestZ(temp & 0xFF, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DEC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = arg - 1;
            temp &= 0xff;

            TestN(temp, cpu);
            TestZ(temp, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, temp);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint INY(MC6502State cpu)
        {
            cpu.Y += 1;
            cpu.Y &= 0xFF;
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DEX(MC6502State cpu)
        {
            cpu.X -= 1;
            cpu.X &= 0xFF;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BNE(MC6502State cpu)
        {
            if (cpu.Z == 0)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CLD(MC6502State cpu)
        {
            cpu.D = 0;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CPX(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.X - arg;
            cpu.C = cpu.X >= arg ? 1U : 0U;
            TestN(temp & 0xFF, cpu);
            TestZ(temp & 0xFF, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SBC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.A - arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint INC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            arg += 1;
            if (arg > 0xff)
            {
                arg = 0x00;
            }
            TestN(arg, cpu);
            TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint INX(MC6502State cpu)
        {
            cpu.X += 1;
            cpu.X &= 0xFF;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint NOP(MC6502State cpu)
        {
            // Do nothing
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint BEQ(MC6502State cpu)
        {
            if (cpu.Z == 1)
            {
                cpu.PC = GetRel(cpu);
                return 1;
            }
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SED(MC6502State cpu)
        {
            cpu.D = 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint HLT(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SLO(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            arg = arg << 1;
            cpu.C = (arg >> 8) & 1;
            arg = arg & 0xFF;
            //TestN(arg, cpu);
            //TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);

            cpu.A = arg | cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SRE(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.C = arg & 1;
            arg = (arg >> 1) & 0xFF;
            //cpu.N = 0;
            //TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);

            cpu.A ^= arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint USB(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            // Technically the same as SBC Immediate/E9
            var temp = cpu.A - arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint TAS(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LAX(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A = arg;
            cpu.X = arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SAX(MC6502State cpu)
        {
            var temp = cpu.A & cpu.X;
            cpu.BusWrite(cpu.EffectiveAddr, temp);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DCP(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = arg - 1;
            temp &= 0xff;

            //TestN(temp, cpu);
            //TestZ(temp, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, temp);

            var temp2 = cpu.A - temp;
            cpu.C = cpu.A >= temp ? 1U : 0;

            TestN(temp2 & 0xFF, cpu);
            TestZ(temp2 & 0xFF, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ISC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            arg += 1;
            if (arg > 0xff)
            {
                arg = 0x00;
            }
            //TestN(arg, cpu);
            //TestZ(arg, cpu);

            cpu.BusWrite(cpu.EffectiveAddr, arg);

            var temp = cpu.A - arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RLA(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.C;
            cpu.C = (arg >> 7) & 1;
            arg = ((arg << 1) | temp & 1) & 0xFF;
            //TestN(arg, cpu);
            //TestZ(arg, cpu);
            cpu.BusWrite(cpu.EffectiveAddr, arg);

            cpu.A = arg & cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ANC(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            cpu.A = arg & cpu.A;

            cpu.C = (cpu.A >> 8) & 1;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ALR(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RRA(MC6502State cpu)
        {
            var arg = cpu.BusRead(cpu.EffectiveAddr);
            var temp = cpu.C;
            cpu.C = arg & 1;
            arg = ((arg >> 1) | temp << 7) & 0xFF;
            //TestN(arg, cpu);
            //TestZ(arg, cpu);
            cpu.BusWrite(cpu.EffectiveAddr, arg);

            var temp2 = arg + cpu.A + cpu.C;

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((((cpu.A ^ arg) & 0x80) ^ 0x80) & ((cpu.A ^ temp2) & 0x80)) >> 7 & 1;
            cpu.C = (temp2 >> 8) & 1;

            cpu.A = (byte)(temp2 & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ARR(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ANE(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SHA(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SHY(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SHX(MC6502State cpu)
        {

            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LXA(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint LAS(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint SBX(MC6502State cpu)
        {
            throw new NotImplementedException();
        }

    }
}