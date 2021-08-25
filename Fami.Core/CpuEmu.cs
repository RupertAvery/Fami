﻿using System;
using System.Text;

namespace Fami.Core
{
    public class Ppu
    {
        private const int PPUCTRL = 0x2000;
        private const int PPUMASK = 0x2001;
        private const int PPUSTATUS = 0x2002;
        private const int OAMADDR = 0x2003;
        private const int OAMDATA = 0x2004;
        private const int PPUSCROLL = 0x2005;
        private const int PPUADDR = 0x2006;
        private const int PPUDATA = 0x2007;
        private const int OAMDMA = 0x4014;
        // 341 ppuccs = 1 scanline
        // 262 scanlines = 1 frame
        // 89342 ppuccs = 29780 cpuccs

        private int cycles;

        public void Write(int register)
        {

        }

        public void Reset()
        {

        }

    }

    public class CpuEmu
    {
        public Cpu6502State Cpu { get; set; }
        public Ppu Ppu { get; set; }
        private int cycles;
        private bool running;

        public CpuEmu()
        {
            Cpu = new Cpu6502State();
            Ppu = new Ppu();
        }

        public void Init()
        {
            cycles = 0;
            running = true;
            Cpu.Init();
            Cpu6502InstructionSet.InitCpu();
        }

        public void Reset()
        {
            // https://www.pagetable.com/?p=410


            Cpu.Reset();
            Ppu.Reset();

            cycles += 7;
        }

        public void Execute()
        {

            while (running)
            {
                cycles += Dispatch();
            }


        }

        public int Dispatch()
        {
            var ins = Cpu.Memory.Read(Cpu.PC);
            var bytes = Cpu6502InstructionSet.bytes[ins];

            var sb = new StringBuilder();
            for (var i = 0; i < bytes; i++)
            {
                sb.Append($"{Cpu.Memory.Read(Cpu.PC + i):X2} ");
            }

            for (var i = 0; i < 3 - bytes; i++)
            {
                sb.Append($"   ");
            }

            Console.WriteLine($"{Cpu.PC:X4}  {sb}  A:{Cpu.A:X2} X:{Cpu.X:X2} Y:{Cpu.Y:X2} P:{Cpu.P:X2} S:{Cpu.S:X2} CYC:{cycles}");


            switch (Cpu6502InstructionSet.addrmodes[ins])
            {
                case Cpu6502InstructionSet.IMP:
                    break;
                case Cpu6502InstructionSet.IMM:
                    Cpu.arg = Cpu.ReadImmediate();
                    break;
                case Cpu6502InstructionSet.DP_:
                    Cpu.arg = Cpu.ReadZeroPage();
                    break;
                case Cpu6502InstructionSet.DPX:
                    Cpu.arg = Cpu.ReadZeroPageX();
                    break;
                case Cpu6502InstructionSet.DPY:
                    Cpu.arg = Cpu.ReadZeroPageY();
                    break;
                case Cpu6502InstructionSet.IND:
                    Cpu.arg = Cpu.ReadIndirect();
                    break;
                case Cpu6502InstructionSet.IDX:
                    Cpu.arg = Cpu.ReadIndirectX();
                    break;
                case Cpu6502InstructionSet.IDY:
                    Cpu.arg = Cpu.ReadIndirectY();
                    break;
                case Cpu6502InstructionSet.ABS:
                    Cpu.arg = Cpu.ReadAbsolute();
                    break;
                case Cpu6502InstructionSet.ABX:
                    Cpu.arg = Cpu.ReadAbsoluteX();
                    break;
                case Cpu6502InstructionSet.ABY:
                    Cpu.arg = Cpu.ReadAbsoluteY();
                    break;
                case Cpu6502InstructionSet.REL:
                    Cpu.rel = Cpu.ReadRelative();
                    break;
                default:
                    throw new NotImplementedException();
            }

            Cpu6502InstructionSet.OpCodes[ins](Cpu);

            switch (ins)
            {
                case 0x4C:
                case 0x20:
                    break;
                default:
                    Cpu.PC += bytes;
                    break;
            }



            return Cpu6502InstructionSet.cycles[ins];
        }

    }
}