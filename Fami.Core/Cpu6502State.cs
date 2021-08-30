namespace Fami.Core
{
    public class Cpu6502State
    {
        public uint[] Controller { get; set; } = new uint[2];
        public uint[] ControllerState { get; set; } = new uint[2];

        public uint A { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public uint S { get; set; }

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

        public uint PC { get; set; }

        //public Cpu6502Memory Memory { get; set; }

        public uint N { get; set; }  // bit 7
        public uint V { get; set; }  // bit 6
        public uint U { get; set; }  // bit 5
        public uint B { get; set; }  // bit 4
        public uint D { get; set; }  // bit 3
        public uint I { get; set; }  // bit 2
        public uint Z { get; set; }  // bit 1
        public uint C { get; set; }  // bit 0

        public uint EffectiveAddr { get; set; }
        public bool PageBoundsCrossed { get; set; }

        //public sbyte rel;
        //public uint arg;
        public uint cycles;
        public uint instructions;

        public void Init()
        {
            //Memory = new Cpu6502Memory();
            Ppu = new Ppu(this);
        }

        public void Reset()
        {
            // https://www.pagetable.com/?p=410
            cycles = 0;
            S = 0xFD;
            P = 0x24;
            PC = Read(0xFFFC) + Read(0xFFFD) * 0x100;
            cycles = 7;
            Ppu.Reset();
        }


        private uint[] ram = new uint[0x800];
        private Cartridge _cart;
        public bool NMI;
        public Ppu Ppu { get; private set; }

        public uint Read(uint address)
        {
            uint data = 0;
            var (val, handled) = _cart.CpuRead(address);
            if (handled)
            {
                data = val;
            }
            else if (address < 0x2000)
            {
                data = ram[address & 0x07FF];
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


        public void Write(uint address, uint value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                ram[address & 0x07FF] = value;
            }
            else if (address >= 0x2000 && address <= 0x3000)
            {
                Ppu.Write(address, value);
            }
            else if (address == 0x4014)
            {
                dma_page = dma_data;
                dma_address = 0x00;
                dma_transfer = true;
            }
            else if (address >= 0x4016 && address <= 0x4017)
            {
                ControllerState[address & 0x1] = Controller[address & 01];
            }
        }

        public uint dma_page;
        public uint dma_address;
        public uint dma_data;

        public bool dma_transfer;
        public bool dma_dummy;

        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
            Ppu.LoadCartridge(cart);
        }

        public uint NonMaskableInterrupt()
        {
            Cpu6502InstructionSet.Push(this, (PC >> 8) & 0xFF); // Push the high byte of the PC
            Cpu6502InstructionSet.Push(this, (PC & 0xFF)); // Push the low byte of the PC

            B = 0;
            U = 1;
            I = 1;
            Cpu6502InstructionSet.Push(this, P);
            PC = Read(0xFFFA) + Read(0xFFFB) * 0x100; // Jump to NMI handler
            return 8;
        }
    }
}
