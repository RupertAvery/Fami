namespace Fami.Core
{
    public class Ppu
    {
        private const int PPUCTRL = 0x2000;
        private const int PPUMASK = 0x2001;
        private const int PPUSTATUS = 0x2002;
        private const int OAMADDR = 0x2003;
        private const int OAMDATA = 0x2004;
        private const int PPUSCROLL = 0x2005;
        private const int PPUADDR = 0x2006;
        private const int PPUDATA = 0x2007;

        private const int OAMDMA = 0x4014;
        // 341 ppuccs = 1 scanline
        // 262 scanlines = 1 frame
        // 89342 ppuccs = 29780 cpuccs

        private int cycles;
        private uint[,] tableName = new uint[2, 1024];
        private uint[] palette = new uint[32];
        private Cartridge _cart;

        public void Write(int address, int value)
        {
            switch (address)
            {
                case PPUCTRL:
                    break;
                case PPUMASK:
                    break;
                case PPUSTATUS:
                    break;
                case OAMADDR:
                    break;
                case OAMDATA:
                    break;
                case PPUSCROLL:
                    break;
                case PPUADDR:
                    break;
                case PPUDATA:
                    break;
            }
        }

        public int Read(int address)
        {
            switch (address)
            {
                case PPUCTRL:
                    break;
                case PPUMASK:
                    break;
                case PPUSTATUS:
                    break;
                case OAMADDR:
                    break;
                case OAMDATA:
                    break;
                case PPUSCROLL:
                    break;
                case PPUADDR:
                    break;
                case PPUDATA:
                    break;
            }
            return 0;
        }

        public void Reset()
        {

        }

        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
        }

        private int cycle;
        private int scanline;
        private bool frameComplete;
          
        public void Clock()
        {
            cycle++;
            if (cycle >= 341)
            {
                cycle = 0;
                scanline++;
                if (scanline >= 261)
                {
                    scanline = -1;
                    frameComplete = true;
                }
            }
        }

    }
}