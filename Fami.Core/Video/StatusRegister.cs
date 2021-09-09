namespace Fami.Core.Video
{
    public struct StatusRegister
    {
        public byte SpriteOverflow { get; set; }
        public byte SpriteZeroHit { get; set; }
        public byte VerticalBlank { get; set; }
        public byte Register
        {
            get
            {
                return (byte)
                    (
                        ((SpriteOverflow & 0b1) << 5) +
                        ((SpriteZeroHit & 0b1) << 6) +
                        ((VerticalBlank & 0b1) << 7)
                    );
            }
            set
            {
                SpriteOverflow = (byte)((value >> 5) & 0b1);
                SpriteZeroHit = (byte)((value >> 6) & 0b1);
                VerticalBlank = (byte)((value >> 7) & 0b1);
            }
        }
    }
}