using System;
using System.Runtime.CompilerServices;

namespace Fami.Core
{

    public static partial class Cpu6502InstructionSet
    {
        const byte __ = 0;
        const byte _c = 2;

        public static byte[] cycles = new byte[]
        {
            // 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F
            07, 06, __, __, 03, 03, 05, __, 03, 02, 02, __, 04, 04, 06, __, // 00
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // 10
            06, 06, __, __, 03, 03, 05, __, 04, 02, 02, __, 04, 04, 06, __, // 20
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // 30
            06, 06, __, __, 03, 03, 05, __, 03, 02, 02, __, 03, 04, 06, __, // 40
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // 50
            06, 06, __, __, 03, 03, 05, __, 04, 02, 02, __, 05, 04, 06, __, // 60
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // 70
            02, 06, 02, __, 03, 03, 03, __, 02, 02, 02, __, 04, 04, 04, __, // 80
            02, 06, __, __, 04, 04, 04, __, 02, 05, 02, __, __, 05, __, __, // 90
            02, 06, 02, __, 03, 03, 03, __, 02, 02, 02, __, 04, 04, 04, __, // A0
            02, 05, __, __, 04, 04, 04, __, 02, 04, 02, __, 04, 04, 04, __, // B0
            02, 06, 02, __, 03, 03, 05, __, 02, 02, 02, __, 04, 04, 03, __, // C0
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // D0
            02, 06, 02, __, 03, 03, 05, __, 02, 02, 02, __, 04, 04, 06, __, // E0
            02, 05, __, __, 04, 04, 06, __, 02, 04, 02, __, 04, 04, 07, __, // F0
        };


        public static byte[] bytes = new byte[]
        {
            // 00  01  02  03  04  05  06  07  08  09  0A  0B  0C  0D  0E  0F
            01, 02, 01, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // 00
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // 10
            03, 02, 01, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // 20
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // 30
            01, 02, 01, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // 40
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // 50
            01, 02, 01, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // 60
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // 70
            02, 02, 02, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // 80
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, __, 03, __, __, // 90
            02, 02, 02, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // A0
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // B0
            02, 02, 02, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // C0
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // D0
            02, 02, 02, __, 02, 02, 02, __, 01, 02, 01, __, 03, 03, 03, __, // E0
            02, 02, 01, __, 02, 02, 02, __, 01, 03, 01, __, 03, 03, 03, __, // F0
        };

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
               IMP, IDX, UNK, UNK, DP_, DP_, DP_, UNK, IMP, IMM, ACC, UNK, ABX, ABS, ABS, UNK, // 00
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // 10
               ABS, IDX, UNK, UNK, DP_, DP_, DP_, UNK, IMP, IMM, ACC, UNK, ABS, ABS, ABS, UNK, // 20
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // 30
               IMP, IDX, UNK, UNK, DP_, DP_, DP_, UNK, IMP, IMM, ACC, UNK, ABS, ABS, ABS, UNK, // 40
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // 50
               IMP, IDX, UNK, UNK, DP_, DP_, DP_, UNK, IMP, IMM, ACC, UNK, IND, ABS, ABS, UNK, // 60
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // 70
               IMM, IDX, IMM, UNK, DP_, DP_, DP_, UNK, IMP, IMM, IMP, UNK, ABS, ABS, ABS, UNK, // 80
               REL, IDY, UNK, UNK, DPX, DPX, DPY, UNK, IMP, ABY, IMP, UNK, UNK, ABX, UNK, UNK, // 90
               IMM, IDX, IMM, UNK, DP_, DP_, DP_, UNK, IMP, IMM, IMP, UNK, ABS, ABS, ABS, UNK, // A0
               REL, IDY, UNK, UNK, DPX, DPX, DPY, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABY, UNK, // B0
               IMM, IDX, IMM, UNK, DP_, DP_, DP_, UNK, IMP, IMM, IMP, UNK, ABS, ABS, ABS, UNK, // C0
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // D0
               IMM, IDX, IMM, UNK, DP_, DP_, DP_, UNK, IMP, IMM, IMP, UNK, ABS, ABS, ABS, UNK, // E0
               REL, IDY, UNK, UNK, DPX, DPX, DPX, UNK, IMP, ABY, IMP, UNK, ABX, ABX, ABX, UNK, // F0
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestN(int value, Cpu6502State cpu)
        {
            if ((value & 0b10000000) == 0b10000000)
            {
                cpu.N = 1;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void TestZ(int value, Cpu6502State cpu)
        {
            if ((value & 0b00000000) == 0b00000000)
            {
                cpu.Z = 1;
            }
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
            SetC(cpu.A >> 7 & 1, cpu);
            cpu.A = (cpu.A << 1) & 0xFF;
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ASL_Mem(Cpu6502State cpu)
        {
            SetC(cpu.arg >> 7 & 1, cpu);
            cpu.arg = (cpu.arg << 1) & 0xFF;
            TestN(cpu.arg, cpu);
            TestZ(cpu.arg, cpu);
            cpu.Memory.Write(cpu.EffectiveAddr, cpu.arg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PHP(Cpu6502State cpu)
        {
            var flags = cpu.P | 0b00110000;
            cpu.Memory.Write(cpu.S, flags);
            cpu.S -= 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BPL(Cpu6502State cpu)
        {
            if (cpu.N == 0)
            {
                int temp = cpu.PC;
                temp += cpu.rel;
                switch (temp)
                {
                    case > 0xFFFF:
                        temp -= 0xFFFF;
                        break;
                    case < 0:
                        temp += 0xFFFF;
                        break;
                }

                cpu.PC += (ushort)(temp & 0xFFFF);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLC(Cpu6502State cpu)
        {
            cpu.C = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JSR(Cpu6502State cpu)
        {
            var pc = cpu.PC + 2;
            cpu.Memory.Write(cpu.S, pc & 0xFF);
            cpu.Memory.Write(cpu.S - 1, (pc / 0x100) & 0xFF);
            cpu.S -= 2;
            cpu.PC = cpu.EffectiveAddr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BIT(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROL(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PLP(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BMI(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SEC(Cpu6502State cpu)
        {
            cpu.C = 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RTI(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EOR(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LSR(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PHA(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void JMP(Cpu6502State cpu)
        {
            cpu.PC = cpu.EffectiveAddr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BVC(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLI(Cpu6502State cpu)
        {
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RTS(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ADC(Cpu6502State cpu)
        {
            var temp = cpu.arg + cpu.A + cpu.C;
            cpu.A = (byte)(temp & 0xFF);
            SetC(temp >> 8 & 1, cpu);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ROR(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PLA(Cpu6502State cpu)
        {
            cpu.S += 1;
            cpu.A = cpu.Memory.Read(cpu.S);
            TestN(cpu.A, cpu);
            TestZ(cpu.A, cpu);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BVS(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SEI(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STA(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STY(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void STX(Cpu6502State cpu)
        {
            cpu.X = cpu.arg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEY(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TXA(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BCC(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TYA(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TXS(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LDY(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LDA(Cpu6502State cpu)
        {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TAX(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BCS(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLV(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TSX(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPY(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CMP(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEC(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INY(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DEX(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BNE(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CLD(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CPX(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SBC(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INC(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void INX(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NOP(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BEQ(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SED(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HLT(Cpu6502State cpu)
        {
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SLO(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SRE(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void USB(Cpu6502State cpu)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TAS(Cpu6502State cpu)
        {
        }
    }
}