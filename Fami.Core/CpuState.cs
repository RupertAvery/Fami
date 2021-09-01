namespace Fami.Core
{
    public struct CpuState
    {
        public static CpuState New()
        {
            return new CpuState
            {
                RAM = new uint[0x800],
                A = 0,
                X = 0,
                Y = 0,
                S = 0,
                PC = 0,
                N = 0,
                V = 0,
                U = 0,
                B = 0,
                D = 0,
                I = 0,
                Z = 0,
                C = 0,
                cycles = 0
            };
        }

        public uint[] RAM;
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
    }
}