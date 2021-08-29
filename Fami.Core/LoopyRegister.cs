namespace Fami.Core
{
    public class LoopyRegister
    {
        public uint CoarseX { get; set; }
        public uint CoarseY { get; set; }
        public uint NametableX { get; set; }
        public uint NametableY { get; set; }
        public uint FineY { get; set; }

        public uint Register
        {
            get
            {
                return
                    (CoarseX & 0b11111) +
                    ((CoarseY & 0b11111) << 5) +
                    ((NametableX & 0b1) << 10) +
                    ((NametableY & 0b1) << 11) +
                    ((FineY & 0b111) << 12) 
                    ;
            }
            set
            {
                CoarseX = value & 0b11111;
                CoarseY = (value >> 5) & 0b11111;
                NametableX = (value >> 10) & 0b1;
                NametableY = (value >> 11) & 0b1;
                FineY = (value >> 12) & 0b111;
            }
        }
    }
}