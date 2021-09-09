namespace Fami.Core.Video
{
    public struct ControlRegister
    {
        public byte NametableX { get; set; }
        public byte NametableY { get; set; }
        public byte IncrementMode { get; set; }
        public byte PatternSprite { get; set; }
        public byte PatternBackground { get; set; }
        public byte SpriteSize { get; set; }
        public byte SlaveMode { get; set; }
        public byte EnableNmi { get; set; }

        public byte Register
        {
            get
            {
                return (byte)(
                    (NametableX & 0b1) +
                    ((NametableY & 0b1) << 1)  +
                    ((IncrementMode & 0b1) << 2) +
                    ((PatternSprite & 0b1) << 3) +
                    ((PatternBackground & 0b1) << 4) +
                    ((SpriteSize & 0b1) << 5) +
                    ((SlaveMode & 0b1) << 6) +
                    ((EnableNmi & 0b1) << 7) 
                    );
            }
            set
            {
                NametableX = (byte)((value) & 0b1);
                NametableY = (byte)((value >> 1) & 0b1);
                IncrementMode = (byte)((value >> 2) & 0b1);
                PatternSprite = (byte)((value >> 3) & 0b1);
                PatternBackground = (byte)((value >> 4) & 0b1);
                SpriteSize = (byte)((value >> 5) & 0b1);
                SlaveMode = (byte)((value >> 6) & 0b1);
                EnableNmi = (byte)((value >> 7) & 0b1);
            }
        }
    }
}