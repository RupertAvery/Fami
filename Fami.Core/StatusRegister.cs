namespace Fami.Core
{
    public class StatusRegister
    {
        public uint SpriteOverflow { get; set; }
        public uint SpriteZeroHit { get; set; }
        public uint VerticalBlank { get; set; }
        public uint Register
        {
            get
            {
                return
                    ((SpriteOverflow & 0b1) << 5) +
                    ((SpriteZeroHit & 0b1) << 6) +
                    ((VerticalBlank & 0b1) << 7)
                    ;
            }
            set
            {
                SpriteOverflow = (value >> 5) & 0b1;
                SpriteZeroHit = (value >> 6) & 0b1;
                VerticalBlank = (value >> 7) & 0b1;
            }
        }
    }
}