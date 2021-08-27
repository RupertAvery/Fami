namespace Fami.Core
{
    public class Cpu6502State
    {
        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int S { get; set; }

        public int P
        {
            get =>
                (
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

        public int PC { get; set; }

        //public Cpu6502Memory Memory { get; set; }

        public int N { get; set; }
        public int V { get; set; }
        public int B2 { get; set; }
        public int B1 { get; set; }
        public int D { get; set; }
        public int I { get; set; }
        public int Z { get; set; }
        public int C { get; set; }
        public int EffectiveAddr { get; set; }
        public bool PageBoundsCrossed { get; set; }
        public bool Branched { get; set; }

        public sbyte rel;
        public int arg;
        public int cycles;

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


        private int[] ram = new int[0x800];
        private Cartridge _cart;
        public Ppu Ppu { get; private set; }

        public int Read(int address)
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

        public void Write(int address, int value)
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
        }
    }
}
