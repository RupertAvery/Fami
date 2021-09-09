namespace Fami.Core.Video
{
    public struct MaskRegister
    {
        public byte Grayscale { get; set; }
        public byte RenderBackgroundLeft { get; set; }
        public byte RenderSpritesleft { get; set; }
        public byte RenderBackground { get; set; }
        public byte RenderSprites { get; set; }
        public byte EnhanceRed { get; set; }
        public byte EnhanceGreen { get; set; }
        public byte EnhanceBlue { get; set; }

        public byte Register
        {
            get
            {
                return (byte)(
                    (Grayscale & 0b1) +
                    ((RenderBackgroundLeft & 0b1) << 1) +
                    ((RenderSpritesleft & 0b1) << 2) +
                    ((RenderBackground & 0b1) << 3) +
                    ((RenderSprites & 0b1) << 4) +
                    ((EnhanceRed & 0b1) << 5) +
                    ((EnhanceGreen & 0b1) << 6) +
                    ((EnhanceBlue & 0b1) << 7)
                    );
            }
            set
            {
                Grayscale = (byte)((value) & 0b1);
                RenderBackgroundLeft = (byte)((value >> 1) & 0b1);
                RenderSpritesleft = (byte)((value >> 2) & 0b1);
                RenderBackground = (byte)((value >> 3) & 0b1);
                RenderSprites = (byte)((value >> 4) & 0b1);
                EnhanceRed = (byte)((value >> 5) & 0b1);
                EnhanceGreen = (byte)((value >> 6) & 0b1);
                EnhanceBlue = (byte)((value >> 7) & 0b1);
            }
        }
    }
}