using System;
using System.Runtime.CompilerServices;

namespace Fami.Core
{

    public static partial class Cpu6502InstructionSet
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

        public static byte[] cycles = new byte[]
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

        public static byte[] bytes = new byte[]
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
        private static void TestN(int value, Cpu6502State cpu)
        {
            cpu.N = (value & 0b10000000) >> 7;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestV(int value, Cpu6502State cpu)
        {
            cpu.V = (value & 0b01000000) >> 6;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestZ(int value, Cpu6502State cpu)
        {
            cpu.Z = value == 0b00000000 ? 1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetC(int value, Cpu6502State cpu)
        {
            cpu.C = value;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BRK(Cpu6502State cpu)
        {
            cpu.Memory.Write(cpu.S, (byte)(cpu.PC & 0xff));
            cpu.Memory.Write(cpu.S + 1, (byte)((cpu.PC >> 8) & 0xff));
            var flags = cpu.P | 0b00110100;
            cpu.Memory.Write(cpu.S + 2, flags);
            cpu.S += 3;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ORA(Cpu6502State cpu)
        {
            cpu.A = cpu.arg | cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AND(Cpu6502State cpu)
        {
            cpu.A = cpu.arg & cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ASL(Cpu6502State cpu)
        {
            cpu.A <<= 1;
            cpu.C = (cpu.A >> 8) & 1;
            cpu.A &= 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ASL_Mem(Cpu6502State cpu)
        {
            cpu.arg = cpu.arg << 1;
            cpu.C = (cpu.arg >> 8) & 1;
            cpu.arg = cpu.arg & 0xFF;
            TestN(cpu.arg, cpu);
            TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PHP(Cpu6502State cpu)
        {
            Push(cpu, cpu.P | 0b00110000);
            //cpu.Memory.Write(cpu.S + 0x100, flags);
            //cpu.S -= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BPL(Cpu6502State cpu)
        {
            if (cpu.N == 0)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Push(Cpu6502State cpu, int value)
        {
            cpu.Memory.Write(cpu.S + 0x100, value);
            cpu.S -= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Pop(Cpu6502State cpu)
        {
            cpu.S += 1;
            return cpu.Memory.Read(cpu.S + 0x100);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLC(Cpu6502State cpu)
        {
            cpu.C = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JSR(Cpu6502State cpu)
        {
            // PC already points to the address AFTER the last byte of the instruction, so subtract 1
            var pc = cpu.PC - 1;
            Push(cpu, (pc >> 8) & 0xFF);
            Push(cpu, pc & 0xFF);
            //cpu.Memory.Write(0x100 + cpu.S, (pc >> 8) & 0xFF);
            //cpu.Memory.Write(0x100 + cpu.S - 1, pc & 0xFF);
            //cpu.S -= 2;
            cpu.PC = cpu.EffectiveAddr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BIT(Cpu6502State cpu)
        {
            TestN(cpu.arg, cpu);
            TestV(cpu.arg, cpu);
            TestZ(cpu.A & cpu.arg, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROL(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = (cpu.A >> 7) & 1;
            cpu.A = ((cpu.A << 1) | temp & 1) & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROL_Mem(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = (cpu.arg >> 7) & 1;
            cpu.arg = ((cpu.arg << 1) | temp & 1) & 0xFF;
            TestN(cpu.arg, cpu);
            TestZ(cpu.arg, cpu);
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PLP(Cpu6502State cpu)
        {
            //cpu.S += 1;
            //cpu.P = cpu.Memory.Read(cpu.S) | 0b00100000;
            //cpu.P = Pop(cpu) | 0b00100000;
            //cpu.P = Pop(cpu) | 0b00110000;
            cpu.P = Pop(cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BMI(Cpu6502State cpu)
        {
            if (cpu.N == 1)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SEC(Cpu6502State cpu)
        {
            cpu.C = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RTI(Cpu6502State cpu)
        {
            //The status register is pulled with the break flag and bit 5 ignored.
            cpu.P = Pop(cpu) & 0b11001111 | 0b00100000;
            // Then PC is pulled from the stack.
            cpu.PC = Pop(cpu);
            cpu.PC += Pop(cpu) * 0x100;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EOR(Cpu6502State cpu)
        {
            cpu.A ^= cpu.arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LSR(Cpu6502State cpu)
        {
            cpu.C = cpu.A & 1;
            cpu.A = (cpu.A >> 1) & 0xFF;
            cpu.N = 0;
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LSR_Mem(Cpu6502State cpu)
        {
            cpu.C = cpu.arg & 1;
            cpu.arg = (cpu.arg >> 1) & 0xFF;
            cpu.N = 0;
            TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PHA(Cpu6502State cpu)
        {
            Push(cpu, cpu.A);
            //cpu.Memory.Write(cpu.S, cpu.A);
            //cpu.S--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JMP(Cpu6502State cpu)
        {
            cpu.PC = cpu.EffectiveAddr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BVC(Cpu6502State cpu)
        {
            if (cpu.V == 0)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLI(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RTS(Cpu6502State cpu)
        {
            cpu.PC = Pop(cpu);
            cpu.PC += Pop(cpu) * 0x100;

            //cpu.S += 1;
            //cpu.PC = cpu.Memory.Read(cpu.S + 0x100);
            //cpu.S += 1;
            //cpu.PC += cpu.Memory.Read(cpu.S + 0x100) * 0x100;
            cpu.PC++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ADC(Cpu6502State cpu)
        {
            var temp = cpu.arg + cpu.A + cpu.C;

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((((cpu.A ^ cpu.arg) & 0x80) ^ 0x80) & ((cpu.A ^ temp) & 0x80)) >> 7 & 1;
            cpu.C = (temp >> 8) & 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROR(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = cpu.A & 1;
            cpu.A = ((cpu.A >> 1) | temp << 7) & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROR_Mem(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = cpu.arg & 1;
            cpu.arg = ((cpu.arg >> 1) | temp << 7) & 0xFF;
            TestN(cpu.arg, cpu);
            TestZ(cpu.arg, cpu);
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PLA(Cpu6502State cpu)
        {
            cpu.A = Pop(cpu);
            //cpu.S += 1;
            //cpu.A = cpu.Memory.Read(cpu.S);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BVS(Cpu6502State cpu)
        {
            if (cpu.V == 1)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SEI(Cpu6502State cpu)
        {
            cpu.I = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STA(Cpu6502State cpu)
        {
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.A);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STY(Cpu6502State cpu)
        {
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STX(Cpu6502State cpu)
        {
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEY(Cpu6502State cpu)
        {
            cpu.Y -= 1;
            if (cpu.Y < 0x00)
            {
                cpu.Y = 0xFF;
            }
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TXA(Cpu6502State cpu)
        {
            cpu.A = cpu.X;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BCC(Cpu6502State cpu)
        {
            if (cpu.C == 0)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TYA(Cpu6502State cpu)
        {
            cpu.A = cpu.Y;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TXS(Cpu6502State cpu)
        {
            cpu.S = cpu.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LDY(Cpu6502State cpu)
        {
            cpu.Y = cpu.arg;
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LDA(Cpu6502State cpu)
        {
            cpu.A = cpu.arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LDX(Cpu6502State cpu)
        {
            cpu.X = cpu.arg;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TAY(Cpu6502State cpu)
        {
            cpu.Y = cpu.A;
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TAX(Cpu6502State cpu)
        {
            cpu.X = cpu.A;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BCS(Cpu6502State cpu)
        {
            if (cpu.C == 1)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetRel(Cpu6502State cpu)
        {
            var x = cpu.PC >> 8;
            var y = (cpu.PC + cpu.rel) >> 8;

            if (x != y)
            {
                cpu.PageBoundsCrossed = true;
            }
            var temp = cpu.PC + cpu.rel;
            switch (temp)
            {
                case > 0xFFFF:
                    temp -= 0xFFFF;
                    break;
                case < 0:
                    temp += 0xFFFF;
                    break;
            }

            return temp & 0xFFFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLV(Cpu6502State cpu)
        {
            cpu.V = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TSX(Cpu6502State cpu)
        {
            cpu.X = cpu.S & 0xFF;
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPY(Cpu6502State cpu)
        {
            var temp = cpu.Y - cpu.arg;
            cpu.C = cpu.Y >= cpu.arg ? 1 : 0;
            TestN(temp & 0xFFFF, cpu);
            TestZ(temp & 0xFFFF, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CMP(Cpu6502State cpu)
        {
            var temp = cpu.A - cpu.arg;
            cpu.C = cpu.A >= cpu.arg ? 1 : 0;
            TestN(temp & 0xFF, cpu);
            TestZ(temp & 0xFF, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEC(Cpu6502State cpu)
        {
            var temp = cpu.arg - 1;
            if (temp < 0x00)
            {
                temp = temp & 0xff;
            }

            TestN(temp, cpu);
            TestZ(temp, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, temp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INY(Cpu6502State cpu)
        {
            cpu.Y += 1;
            if (cpu.Y > 0xFF)
            {
                cpu.Y = 0;
            }
            TestN(cpu.Y, cpu);
            TestZ(cpu.Y, cpu);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEX(Cpu6502State cpu)
        {
            cpu.X -= 1;
            if (cpu.X < 0x00)
            {
                cpu.X = 0xFF;
            }
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BNE(Cpu6502State cpu)
        {
            if (cpu.Z == 0)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLD(Cpu6502State cpu)
        {
            cpu.D = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPX(Cpu6502State cpu)
        {
            var temp = cpu.X - cpu.arg;
            cpu.C = cpu.X >= cpu.arg ? 1 : 0;
            TestN(temp & 0xFF, cpu);
            TestZ(temp & 0xFF, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SBC(Cpu6502State cpu)
        {
            var temp = cpu.A - cpu.arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ cpu.arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INC(Cpu6502State cpu)
        {
            cpu.arg += 1;
            if (cpu.arg > 0xff)
            {
                cpu.arg = 0x00;
            }
            TestN(cpu.arg, cpu);
            TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INX(Cpu6502State cpu)
        {
            cpu.X += 1;
            if (cpu.X > 0xFF)
            {
                cpu.X = 0;
            }
            TestN(cpu.X, cpu);
            TestZ(cpu.X, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NOP(Cpu6502State cpu)
        {
            // Do nothing
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BEQ(Cpu6502State cpu)
        {
            if (cpu.Z == 1)
            {
                cpu.Branched = true;
                cpu.PC = GetRel(cpu);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SED(Cpu6502State cpu)
        {
            cpu.D = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HLT(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SLO(Cpu6502State cpu)
        {
            cpu.arg = cpu.arg << 1;
            cpu.C = (cpu.arg >> 8) & 1;
            cpu.arg = cpu.arg & 0xFF;
            //TestN(cpu.arg, cpu);
            //TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

            cpu.A = cpu.arg | cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SRE(Cpu6502State cpu)
        {
            cpu.C = cpu.arg & 1;
            cpu.arg = (cpu.arg >> 1) & 0xFF;
            //cpu.N = 0;
            //TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

            cpu.A ^= cpu.arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void USB(Cpu6502State cpu)
        {
            // Technically the same as SBC Immediate/E9
            var temp = cpu.A - cpu.arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ cpu.arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TAS(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LAX(Cpu6502State cpu)
        {
            cpu.A = cpu.arg;
            cpu.X = cpu.arg;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SAX(Cpu6502State cpu)
        {
            var temp = cpu.A & cpu.X;
            cpu.Memory.Write(cpu.EffectiveAddr, temp);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DCP(Cpu6502State cpu)
        {
            var temp = cpu.arg - 1;
            if (temp < 0x00)
            {
                temp = temp & 0xff;
            }

            //TestN(temp, cpu);
            //TestZ(temp, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, temp);
            
            var temp2 = cpu.A - temp;
            cpu.C = cpu.A >= temp ? 1 : 0;

            TestN(temp2 & 0xFF, cpu);
            TestZ(temp2 & 0xFF, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ISC(Cpu6502State cpu)
        {
            cpu.arg += 1;
            if (cpu.arg > 0xff)
            {
                cpu.arg = 0x00;
            }
            //TestN(cpu.arg, cpu);
            //TestZ(cpu.arg, cpu);

            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

            var temp = cpu.A - cpu.arg - ((cpu.C ^ 1) & 1);

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((cpu.A ^ temp) & (cpu.A ^ cpu.arg) & 0x80) >> 7 & 1;
            cpu.C = (temp >> 8) & 1 ^ 1;

            cpu.A = (byte)(temp & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RLA(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = (cpu.arg >> 7) & 1;
            cpu.arg = ((cpu.arg << 1) | temp & 1) & 0xFF;
            //TestN(cpu.arg, cpu);
            //TestZ(cpu.arg, cpu);
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

            cpu.A = cpu.arg & cpu.A;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ANC(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ALR(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RRA(Cpu6502State cpu)
        {
            var temp = cpu.C;
            cpu.C = cpu.arg & 1;
            cpu.arg = ((cpu.arg >> 1) | temp << 7) & 0xFF;
            //TestN(cpu.arg, cpu);
            //TestZ(cpu.arg, cpu);
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);

            var temp2 = cpu.arg + cpu.A + cpu.C;

            // Stolen from FCEUX
            // http://www.righto.com/2012/12/the-6502-overflow-flag-explained.html
            cpu.V = ((((cpu.A ^ cpu.arg) & 0x80) ^ 0x80) & ((cpu.A ^ temp2) & 0x80)) >> 7 & 1;
            cpu.C = (temp2 >> 8) & 1;

            cpu.A = (byte)(temp2 & 0xFF);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ARR(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ANE(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SHA(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SHY(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SHX(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LXA(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LAS(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SBX(Cpu6502State cpu)
        {
            throw new NotImplementedException();
        }

    }
}