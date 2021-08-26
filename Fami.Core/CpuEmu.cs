using System;
using System.IO;
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
        private StringBuilder log;
        public CpuEmu()
        {
            Cpu = new Cpu6502State();
            Ppu = new Ppu();
            log = new StringBuilder();
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
            try
            {
                while (running)
                {
                    cycles += Dispatch();
                    if (cycles == 26724)
                    {
                        running = false;
                    }
                }
                File.WriteAllText("fami.log", log.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                File.WriteAllText("fami.log", log.ToString());
            }


        }

        public int Dispatch()
        {
            var lastPC = Cpu.PC;

            switch (Cpu.PC)
            {
                case 0xF55E:
                case 0xFAF1:
                    var x = 1;
                    break;
            }

            var ins = Cpu.Memory.Read(Cpu.PC);
            var bytes = Cpu6502InstructionSet.bytes[ins];

            Log(bytes);

            switch (Cpu6502InstructionSet.addrmodes[ins])
            {
                case Cpu6502InstructionSet.ACC:
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
                    if (ins == 0x6c)
                    {
                        Cpu.arg = Cpu.ReadIndirect_JMP();
                    }
                    else
                    {
                        Cpu.arg = Cpu.ReadIndirect();
                    }
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

            Cpu.PC += bytes;

            Cpu6502InstructionSet.OpCodes[ins](Cpu);

            if (Cpu.PC == lastPC)
            {
                throw new Exception("PC not updated!");
            }


            var pcycles = Cpu6502InstructionSet.cycles[ins];

            // replace with ints so we can just add?
            if (Cpu.Branched)
            {
                Cpu.Branched = false;
                pcycles++;
            }

            if (Cpu.PageBoundsCrossed)
            {
                switch (ins)
                {
                    /*
                         Affected:
                        ADC
                        AND
                        CMP
                        EOR
                        LAX
                        LDA
                        LDX
                        LDY
                        NOP
                        ORA
                        SBC
                        (indirect),Y
                        absolute,X
                        absolute,Y
                     */
                    case 0x71:
                    case 0x7D:
                    case 0x79:
                    case 0x31:
                    case 0x3D:
                    case 0x39:
                    case 0xD1:
                    case 0xDD:
                    case 0xD9:
                    case 0x51:
                    case 0x5D:
                    case 0x59:
                    case 0xB3:
                    case 0xBF:
                    case 0xB1:
                    case 0xBD:
                    case 0xB9:
                    case 0xBE:
                    case 0xBC:
                    case 0x1C:
                    case 0x3C:
                    case 0x5C:
                    case 0x7C:
                    case 0xDC:
                    case 0xFC:
                    case 0x11:
                    case 0x1D:
                    case 0x19:
                    case 0xF1:
                    case 0xFD:
                    case 0xF9:
                        pcycles++;
                        break;
                }
                Cpu.PageBoundsCrossed = false;
            }



            // Just to align with nestest.log
            if (ins == 0xce)
            {
                pcycles += 3;
            }

            //switch (Cpu.PC)
            //{
            //    case 0xD922:
            //    case 0xD982:
            //    case 0xD993:
            //    case 0xD9A7:
            //    case 0xD9BD:
            //    case 0xD9D6:
            //        pcycles -= 1;
            //        break;
            //}

            //if (Cpu.PC == 0xDD95)
            //{
            //    pcycles += 1;
            //}
            return pcycles;
        }

        private void Log(int bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes; i++)
            {
                sb.Append($"{Cpu.Memory.Read(Cpu.PC + i):X2} ");
            }

            for (var i = 0; i < 3 - bytes; i++)
            {
                sb.Append($"   ");
            }

            log.AppendLine($"{Cpu.PC:X4}  {sb} A:{Cpu.A:X2} X:{Cpu.X:X2} Y:{Cpu.Y:X2} P:{Cpu.P:X2} SP:{Cpu.S:X2} CYC:{cycles}");

        }

    }
}