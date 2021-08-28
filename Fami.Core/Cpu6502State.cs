namespace Fami.Core
{
    public class Cpu6502State
    {
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
                    (B2 << 5) +
                    (B1 << 4) +
                    (D << 3) +
                    (I << 2) +
                    (Z << 1) +
                    (C << 0)
                );
            set
            {
                N = (value >> 7 & 1);
                V = (value >> 6 & 1);
                B2 = (value >> 5 & 1);
                B1 = (value >> 4 & 1);
                D = (value >> 3 & 1);
                I = (value >> 2 & 1);
                Z = (value >> 1 & 1);
                C = (value >> 0 & 1);
            }
        }

        public uint PC { get; set; }

        //public Cpu6502Memory Memory { get; set; }

        public uint N { get; set; }
        public uint V { get; set; }
        public uint B2 { get; set; }
        public uint B1 { get; set; }
        public uint D { get; set; }
        public uint I { get; set; }
        public uint Z { get; set; }
        public uint C { get; set; }

        public uint EffectiveAddr { get; set; }
        public bool PageBoundsCrossed { get; set; }
        public bool Branched { get; set; }

        public sbyte rel;
        public uint arg;
        public uint cycles;
        public uint instructions;

        public void Init()
        {
            //Memory = new Cpu6502Memory();
            Ppu = new Ppu();
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
        public Ppu Ppu { get; private set; }

        public uint Read(uint address)
        {
            var (val, handled) = _cart.CpuRead(address);
            if (handled)
            {
                return val;
            }
            else if (address < 0x2000)
            {
                return ram[address & 0x07FF];
            }
            else if (address <= 0x3000)
            {
                return Ppu.Read(address);
            }

            return 0;
        }

        public void Write(uint address, uint value)
        {
            if (address < 0x2000)
            {
                ram[address & 0x07FF] = value;
            }
            else if (address <= 0x3000)
            {
                Ppu.Write(address, value);
            }
        }

        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
            Ppu.LoadCartridge(cart);
        }
    }
}
