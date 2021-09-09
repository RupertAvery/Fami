namespace Fami.Core.Video
{
    public struct LoopyRegister
    {
        public byte CoarseX { get; set; }
        public byte CoarseY { get; set; }
        public byte NametableX { get; set; }
        public byte NametableY { get; set; }
        public byte FineY { get; set; }

        public uint Register
        {
            get
            {
                return (uint)(
                    (CoarseX & 0b11111) +
                    ((CoarseY & 0b11111) << 5) +
                    ((NametableX & 0b1) << 10) +
                    ((NametableY & 0b1) << 11) +
                    ((FineY & 0b111) << 12) 
                    );
            }
            set
            {
                CoarseX = (byte)(value & 0b11111);
                CoarseY = (byte)((value >> 5) & 0b11111);
                NametableX = (byte)((value >> 10) & 0b1);
                NametableY = (byte)((value >> 11) & 0b1);
                FineY = (byte)((value >> 12) & 0b111);
            }
        }
    }
}