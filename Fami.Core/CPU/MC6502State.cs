using System.IO;
using System.Runtime.CompilerServices;
using BizHawk.Emulation.Cores.Nintendo.NES;
using Fami.Core.Video;

namespace Fami.Core.CPU
{
    public enum InterruptTypeEnum
    {
        NMI,
        IRQ,
        BRK
    }

    public partial class MC6502State
    {
        public byte[] RAM = new byte[0x800];
        public uint A;
        public uint X;
        public uint Y;
        public uint S;

        public uint PC;
        public uint N;  // bit 7
        public uint V;  // bit 6
        public uint U;  // bit 5
        public uint B;  // bit 4
        public uint D;  // bit 3
        public uint I { get; set; }  // bit 2
        public uint Z;  // bit 1
        public uint C;  // bit 0

        public uint Cycles;
        public uint Instructions;


        public uint dma_page;
        public uint dma_address;
        public byte dma_data;

        public bool dma_transfer;
        public bool dma_dummy;

        public uint[] Controller = new uint[2];
        public uint[] ControllerRegister = new uint[2];

        public uint P
        {
            get =>
                (uint)(
                    (N << 7) +
                    (V << 6) +
                    (U << 5) +
                    (B << 4) +
                    (D << 3) +
                    (I << 2) +
                    (Z << 1) +
                    (C << 0)
                );
            set
            {
                N = (value >> 7 & 1);
                V = (value >> 6 & 1);
                U = (value >> 5 & 1);
                B = (value >> 4 & 1);
                D = (value >> 3 & 1);
                I = (value >> 2 & 1);
                Z = (value >> 1 & 1);
                C = (value >> 0 & 1);
            }
        }


        public uint EffectiveAddr;
        public bool PageBoundsCrossed;

        public Cartridge Cartridge;
        public Ppu Ppu;
        public APU Apu;

        public bool[] _interrupts = new bool[3];

        public void TriggerInterrupt(InterruptTypeEnum type)
        {
            if (I != 1 || type == InterruptTypeEnum.NMI)
            {
                _interrupts[(int)type] = true;
            }
        }

        public void WriteState(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(RAM, 0, RAM.Length);
            w.Write((byte)A);
            w.Write((byte)X);
            w.Write((byte)Y);
            w.Write((byte)S);
            w.Write((byte)P);
            w.Write((ushort)PC);
            w.Write(Cycles);
        }

        public void ReadState(Stream stream)
        {
            var w = new BinaryReader(stream);
            w.Read(RAM, 0, RAM.Length);
            A = w.ReadByte();
            X = w.ReadByte();
            Y = w.ReadByte();
            S = w.ReadByte();
            P = w.ReadByte();
            PC = w.ReadUInt16();
            Cycles = w.ReadUInt32();
        }

        public void Reset()
        {
            Cartridge.Reset();
            Ppu.Reset();
            // https://www.pagetable.com/?p=410
            S = 0xFD; // Actually 0xFF, but 3 Stack Pushes are done with writes supressed, 
            P = 0x24; // Just to align with nestest.log: I is set, U shouldn't exist, but...
            PC = BusRead(0xFFFC) + BusRead(0xFFFD) * 0x100; // Fetch the reset vector
            I = 1;
            Cycles = 7; // takes 7 cycles to reset
            _instructionCyclesLeft = 0;
            dma_page = 0;
            dma_address = 0x00;
            dma_transfer = false;
            for (var i = 0; i < _interrupts.Length; i++)
            {
                _interrupts[i] = false;
            }
        }

        public uint BusRead(uint address)
        {
            uint data = 0;
            var (val, handled) = Cartridge.CpuRead(address);
            if (handled)
            {
                data = val;
            }
            else if (address < 0x2000)
            {
                data = RAM[address & 0x07FF];
            }
            else if (address >= 0x2000 && address <= 0x3FFF)
            {
                data = Ppu.Read(address);
            }
            else if (address == 0x4015)
            {
                data = Apu.ReadReg((int)address);
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                // Get the current value from the highest bit
                data = (ControllerRegister[address & 0x0001] & 0x80) > 0 ? 1U : 0;

                // Every time we read from this port, shift the corresponding controller register to get the next button value
                // This assumes that the controller reading logic in the game will alawys read 8 button states out of this 
                // register, otherwise the button reads will be out of sync and corrupted
                ControllerRegister[address & 0x0001] <<= 1;

                if (address == 0x4017)
                {
                    CheckLightGun(ref data);
                }
            }
            return data;
        }

        public void BusWrite(uint address, uint value)
        {
            var handled = Cartridge.CpuWrite(address, value);
            if (handled)
            {

            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                RAM[address & 0x07FF] = (byte)value;
            }
            else if (address >= 0x2000 && address <= 0x3000)
            {
                Ppu.Write(address, value);
            }
            else if ((address >= 0x4000 && address <= 0x4013) || address == 0x4015)
            {
                Apu.WriteReg((int)address, (byte)value);
            }
            else if (address == 0x4014)
            {
                dma_page = value;
                dma_address = 0x00;
                dma_transfer = true;
                //dma_dummy = cycles % 2 == 1;
                //for (uint i = 0; i <= 255; i++)
                //{
                //    var _dma_data = (byte)(BusRead(dma_page << 8 | i));
                //    Ppu.pOAM[i] = _dma_data;
                //}
                //cycles += 513 + cycles % 2;
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                ControllerRegister[address & 0x1] = Controller[address & 01];
                if (address == 0x4017)
                {
                    Apu.WriteReg((int)address, (byte)(value & 0xC0));
                }
            }
        }


        public uint NonMaskableInterrupt()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            Push(P);
            I = 1;
            PC = BusRead(0xFFFA) + BusRead(0xFFFB) * 0x100; // Jump to NMI handler
            return 7;
        }


        public uint InterruptRequest()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC
            B = 0;
            U = 1;
            Push(P);
            I = 1;
            PC = BusRead(0xFFFE) + BusRead(0xFFFF) * 0x100; // Jump to IRQ handler
            return 7;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Push(uint value)
        {
            BusWrite(S + 0x100, value);
            S -= 1;
            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Pop()
        {
            S += 1;
            return BusRead(S + 0x100);
        }
    }
}
