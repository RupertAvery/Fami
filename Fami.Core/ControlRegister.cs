namespace Fami.Core
{
    public class ControlRegister
    {
        public uint NametableX { get; set; }
        public uint NametableY { get; set; }
        public uint IncrementMode { get; set; }
        public uint PatternSprite { get; set; }
        public uint PatternBackground { get; set; }
        public uint SpriteSize { get; set; }
        public uint SlaveMode { get; set; }
        public uint EnableNmi { get; set; }

        public uint Register
        {
            get
            {
                return
                    (NametableX & 0b1) +
                    ((NametableY & 0b1) << 1)  +
                    ((IncrementMode & 0b1) << 2) +
                    ((PatternSprite & 0b1) << 3) +
                    ((PatternBackground & 0b1) << 4) +
                    ((SpriteSize & 0b1) << 5) +
                    ((SlaveMode & 0b1) << 6) +
                    ((EnableNmi & 0b1) << 7) 
                    ;
            }
            set
            {
                NametableX = (value) & 0b1;
                NametableY = (value >> 1) & 0b1;
                IncrementMode = (value >> 2) & 0b1;
                PatternSprite = (value >> 3) & 0b1;
                PatternBackground = (value >> 4) & 0b1;
                SpriteSize = (value >> 5) & 0b1;
                SlaveMode = (value >> 6) & 0b1;
                EnableNmi = (value >> 7) & 0b1;
            }
        }
    }
}