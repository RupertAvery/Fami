namespace Fami.Core
{
    public class Cpu6502State
    {
        public int A { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int S { get; set; }

        public byte P
        {
            get =>
                (byte)(
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

        public Cpu6502Memory Memory { get; set; }

        public int N { get; set; }
        public int V { get; set; }
        public int B2 { get; set; }
        public int B1 { get; set; }
        public int D { get; set; }
        public int I { get; set; }
        public int Z { get; set; }
        public int C { get; set; }
        public int EffectiveAddr { get; set; }

        public sbyte rel;
        public int arg;

        public void Init()
        {
            Memory = new Cpu6502Memory();

        }

        public void Reset()
        {
            S = 0xFD;
            P = 0x24;
            PC = Memory.Read(0xFFFC) + Memory.Read(0xFFFD) * 0x100;
        }
    }
}
