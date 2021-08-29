using System;
using System.IO;
using System.Text;

namespace Fami.Core
{
    public class CpuEmu
    {
        public Cpu6502State Cpu { get; private set; }

        private bool running;
        private StringBuilder log;

        public CpuEmu()
        {
            Cpu = new Cpu6502State();
            log = new StringBuilder();
        }

        public void Init()
        {

            running = true;
            Cpu.Init();
            Cpu6502InstructionSet.InitCpu();
        }

        public void Reset()
        {
            Cpu.Reset();
        }

        public uint Step()
        {
            var cycles = 0U;
            Cpu.Ppu.Clock();
         
            if (Cpu.Ppu.cycles % 3 == 0)
            {
                cycles = Dispatch();
                Cpu.cycles += cycles;
                Cpu.instructions++;
            }

            if (Cpu.NMI)
            {
                Cpu.NMI = false;
                Cpu.NonMaskableInterrupt();
            }
            
            
            return cycles;
        }

        public void Execute()
        {
            try
            {
                running = true;
                while (running)
                {
                    Cpu.Ppu.Clock();
                    if (Cpu.Ppu.cycles % 3 == 0)
                    {
                        Cpu.cycles += Dispatch();
                        Cpu.instructions++;
                    }

                    if (Cpu.PC == 0x0001)
                    {
                        running = false;
                    }
                }

                var l = Cpu.Read(02);
                var h = Cpu.Read(03);
                Console.WriteLine($"{l:X2}{h:X2}h");

                File.WriteAllText("fami.log", log.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                File.WriteAllText("fami.log", log.ToString());
            }


        }

        /// <summary>
        /// Executes one instruction and return the number of cycles consumed
        /// </summary>
        /// <returns></returns>
        public uint Dispatch()
        {
            var lastPC = Cpu.PC;

            switch (Cpu.PC)
            {
                case 0x8057:
                //case 0xC79B:
                //case 0xF55E:
                //case 0xFAF1:
                    var x = 1;
                    break;
            }

            var ins = Cpu.Read(Cpu.PC);
            var bytes = Cpu6502InstructionSet.bytes[ins];

            //Log(bytes);

            // This could be moved into each instruction, but we would need to implement all 255 instructions separately
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
                    //File.WriteAllText("mario.log", log.ToString());

                    throw new NotImplementedException();
            }

            Cpu.PC += bytes;

            Cpu6502InstructionSet.OpCodes[ins](Cpu);

            //if (Cpu.PC == lastPC)
            //{
            //    throw new Exception("PC not updated!");
            //}

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

            switch (ins)
            {
                // Just to align with nestest.log
                case 0xce:
                    pcycles += 3;
                    break;
                case 0xf3:
                    pcycles += 4;
                    break;
            }

            return pcycles;
        }

        private void Log(int bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0u; i < bytes; i++)
            {
                sb.Append($"{Cpu.Read(Cpu.PC + i):X2} ");
            }

            for (var i = 0; i < 3 - bytes; i++)
            {
                sb.Append($"   ");
            }

            log.AppendLine($"{Cpu.PC:X4}  {sb} A:{Cpu.A:X2} X:{Cpu.X:X2} Y:{Cpu.Y:X2} P:{Cpu.P:X2} SP:{Cpu.S:X2} CYC:{Cpu.cycles}");

        }

        public void LoadCartridge(Cartridge cart)
        {
            Cpu.LoadCartridge(cart);
        }
    }
}