namespace Fami.Core
{
    public class MaskRegister
    {
        public uint Grayscale { get; set; }
        public uint RenderBackgroundLeft { get; set; }
        public uint RenderSpritesleft { get; set; }
        public uint RenderBackground { get; set; }
        public uint RenderSprites { get; set; }
        public uint EnhanceRed { get; set; }
        public uint EnhanceGreen { get; set; }
        public uint EnhanceBlue { get; set; }

        public uint Register
        {
            get
            {
                return
                    (Grayscale & 0b1) +
                    ((RenderBackgroundLeft & 0b1) << 1) +
                    ((RenderSpritesleft & 0b1) << 2) +
                    ((RenderBackground & 0b1) << 3) +
                    ((RenderSprites & 0b1) << 4) +
                    ((EnhanceRed & 0b1) << 6) +
                    ((EnhanceGreen & 0b1) << 7) +
                    ((EnhanceBlue & 0b1) << 7)
                    ;
            }
            set
            {
                Grayscale = (value) & 0b1;
                RenderBackgroundLeft = (value >> 1) & 0b1;
                RenderSpritesleft = (value >> 2) & 0b1;
                RenderBackground = (value >> 3) & 0b1;
                RenderSprites = (value >> 4) & 0b1;
                EnhanceRed = (value >> 5) & 0b1;
                EnhanceGreen = (value >> 6) & 0b1;
                EnhanceBlue = (value >> 7) & 0b1;
            }
        }
    }
}