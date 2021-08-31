using System.Runtime.CompilerServices;

namespace Fami.Core
{
    public struct CpuState
    {
        private uint[] RAM;
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
        public uint I;  // bit 2
        public uint Z;  // bit 1
        public uint C;  // bit 0
    }

    public partial class Cpu6502State
    {
        public uint[] RAM = new uint[0x800];
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
        public uint I;  // bit 2
        public uint Z;  // bit 1
        public uint C;  // bit 0

        public uint cycles;
        public uint instructions;

        public uint[] Controller = new uint[2];
        public uint[] ControllerState = new uint[2];

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

        public uint dma_page;
        public uint dma_address;
        public byte dma_data;

        public bool dma_transfer;
        public bool dma_dummy;

        public uint EffectiveAddr { get; set; }
        public bool PageBoundsCrossed { get; set; }

        private Cartridge _cart;

        public bool NMI;
        public Ppu Ppu { get; private set; }


        public void Reset()
        {
            // https://www.pagetable.com/?p=410
            cycles = 0;
            S = 0xFD;
            P = 0x24;
            PC = BusRead(0xFFFC) + BusRead(0xFFFD) * 0x100;
            cycles = 7;
            Ppu.Reset();
        }

        public uint BusRead(uint address)
        {
            uint data = 0;
            var (val, handled) = _cart.CpuRead(address);
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
            else if (address >= 0x4016 && address <= 0x4017)
            {
                data = (ControllerState[address & 0x0001] & 0x80) > 0 ? 1U : 0;
                ControllerState[address & 0x0001] <<= 1;
            }
            return data;
        }


        public void BusWrite(uint address, uint value)
        {
            var handled = _cart.CpuWrite(address, value);
            if (handled)
            {

            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                RAM[address & 0x07FF] = value;
            }
            else if (address >= 0x2000 && address <= 0x3000)
            {
                Ppu.Write(address, value);
            }
            else if (address == 0x4014)
            {
                dma_page = value;
                dma_address = 0x00;
                dma_transfer = true;
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                ControllerState[address & 0x1] = Controller[address & 01];
            }
        }
        
        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
            Ppu.LoadCartridge(cart);
        }

        public uint NonMaskableInterrupt()
        {
            Push((PC >> 8) & 0xFF); // Push the high byte of the PC
            Push((PC & 0xFF)); // Push the low byte of the PC

            B = 0;
            U = 1;
            I = 1;
            Push(P);
            PC = BusRead(0xFFFA) + BusRead(0xFFFB) * 0x100; // Jump to NMI handler
            return 8;
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
