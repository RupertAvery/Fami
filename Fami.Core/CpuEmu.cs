using System;
using System.IO;
using System.Text;

namespace Fami.Core
{
    public partial class Cpu6502State
    {
        private bool running;
        private StringBuilder log;

        public Cpu6502State()
        {
            log = new StringBuilder();
        }

        public void Init()
        {
            Ppu = new Ppu(this);
            running = true;
            Cpu6502InstructionSet.InitCpu();
        }


        public void LoadCartridge(Cartridge cart)
        {
            Cartridge = cart;
            Ppu.LoadCartridge(cart);
        }

        private int ticksleft;

        public uint Step()
        {
            Ppu.Clock();

            if (Ppu.cycles % 3 == 0)
            {
                if (dma_transfer)
                {
                    if (dma_dummy)
                    {
                        if (Ppu.cycles % 2 == 1)
                        {
                            dma_dummy = false;
                        }
                    }
                    else
                    {
                        if (Ppu.cycles % 2 == 0)
                        {
                            dma_data = (byte)(BusRead(dma_page << 8 | dma_address));
                        }
                        else
                        {
                            Ppu.pOAM[dma_address] = dma_data;
                            dma_address++;
                            if (dma_address == 256)
                            {
                                dma_transfer = false;
                                dma_dummy = true;
                            }
                        }
                    }
                }
                else
                {
                    if (ticksleft-- > 0) return 1;
                    ticksleft += (int)Dispatch();
                    cycles += (uint)ticksleft;
                    instructions++;
                }
            }


            return 1;
        }

        private bool Debug { get; set; }

        public void Execute()
        {
            try
            {
                Debug = true;
                running = true;
                while (running)
                {
                    Step();

                    //if (cycles % 1000 == 0)
                    //{

                    //    var sb = new StringBuilder();
                    //    uint i = 0;
                    //    var chr = BusRead(0x6000);
                    //    while (chr != 0)
                    //    {
                    //        sb.Append((char)chr);
                    //        i++;
                    //        chr = BusRead(0x6000 + i);
                    //    }

                    //    Console.WriteLine(sb.ToString());
                    //}

                    if (PC == 0x0001)
                    {
                        running = false;
                    }
                }

                var l = BusRead(02);
                var h = BusRead(03);
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

            if (NMI)
            {
                NMI = false;
                NonMaskableInterrupt();
                return 0;
            }


            var lastPC = PC;

            //switch (PC)
            //{
            //    //case 0xC6BC:
            //    //case 0xE928:
            //    case 0xF2FC:
            //        //case 0xFA4D:
            //        //case 0xC79B:
            //        //case 0xF55E:
            //        //case 0xFAF1:
            //        var x = 1;
            //        break;
            //}

            var ins = BusRead(PC);
            var bytes = Cpu6502InstructionSet.bytes[ins];

            if (Debug)
            {
                Log(bytes);
            }

            // This could be moved into each instruction, but we would need to implement all 255 instructions separately
            switch (Cpu6502InstructionSet.addrmodes[ins])
            {
                case Cpu6502InstructionSet.ACC:
                case Cpu6502InstructionSet.IMP:
                    break;
                case Cpu6502InstructionSet.IMM:
                    AddrModeImmediate();
                    break;
                case Cpu6502InstructionSet.DP_:
                    AddrModeZeroPage();
                    break;
                case Cpu6502InstructionSet.DPX:
                    AddrModeZeroPageX();
                    break;
                case Cpu6502InstructionSet.DPY:
                    AddrModeZeroPageY();
                    break;
                case Cpu6502InstructionSet.IND:
                    if (ins == 0x6c)
                    {
                        AddrModeIndirect_JMP();
                    }
                    else
                    {
                        AddrModeIndirect();
                    }
                    break;
                case Cpu6502InstructionSet.IDX:
                    AddrModeIndirectX();
                    break;
                case Cpu6502InstructionSet.IDY:
                    AddrModeIndirectY();
                    break;
                case Cpu6502InstructionSet.ABS:
                    AddrModeAbsolute();
                    break;
                case Cpu6502InstructionSet.ABX:
                    AddrModeAbsoluteX();
                    break;
                case Cpu6502InstructionSet.ABY:
                    AddrModeAbsoluteY();
                    break;
                case Cpu6502InstructionSet.REL:
                    AddrModeRelative();
                    break;
                default:
                    //File.WriteAllText("mario.log", log.ToString());

                    throw new NotImplementedException();
            }

            PC += bytes;

            var pcycles = Cpu6502InstructionSet.cycles[ins];

            pcycles += Cpu6502InstructionSet.OpCodes[ins](this);

            if (PageBoundsCrossed)
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
                    case 0xF0:
                        pcycles++;
                        break;
                }
                PageBoundsCrossed = false;
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

        private void Log(uint bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0u; i < bytes; i++)
            {
                sb.Append($"{BusRead(PC + i):X2} ");
            }

            for (var i = 0; i < 3 - bytes; i++)
            {
                sb.Append($"   ");
            }

            log.AppendLine($"{PC:X4}  {sb} A:{A:X2} X:{X:X2} Y:{Y:X2} P:{P:X2} SP:{S:X2} PPU:{Ppu.scanline + 1},{Ppu.cycle} CYC:{cycles}");

        }

    }
}