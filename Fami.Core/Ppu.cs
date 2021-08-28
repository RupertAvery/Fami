using System;

namespace Fami.Core
{
    public class PpuRenderer
    {
        public uint[] buffer = new uint[256 * 240];
    }

    public class Ppu
    {
        private uint status;
        private uint control;
        private uint mask;
        private uint addr;
        private uint data;

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
        public PpuRenderer Renderer { get; private set; }
        public int cycles;
        private uint[,] tblName = new uint[2, 1024];
        private uint[,] tblPattern = new uint[2, 4096];
        private Cartridge _cart;

        private uint[] tblPalette = new uint[]
        {
            0xFF000000 + (84 << 16) + (84 << 8) + 84, 0xFF000000 + (0 << 16) + (30 << 8) + 116,
            0xFF000000 + (8 << 16) + (16 << 8) + 144, 0xFF000000 + (48 << 16) + (0 << 8) + 136,
            0xFF000000 + (68 << 16) + (0 << 8) + 100, 0xFF000000 + (92 << 16) + (0 << 8) + 48,
            0xFF000000 + (84 << 16) + (4 << 8) + 0, 0xFF000000 + (60 << 16) + (24 << 8) + 0,
            0xFF000000 + (32 << 16) + (42 << 8) + 0, 0xFF000000 + (8 << 16) + (58 << 8) + 0,
            0xFF000000 + (0 << 16) + (64 << 8) + 0, 0xFF000000 + (0 << 16) + (60 << 8) + 0,
            0xFF000000 + (0 << 16) + (50 << 8) + 60, 0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (152 << 16) + (150 << 8) + 152, 0xFF000000 + (8 << 16) + (76 << 8) + 196,
            0xFF000000 + (48 << 16) + (50 << 8) + 236, 0xFF000000 + (92 << 16) + (30 << 8) + 228,
            0xFF000000 + (136 << 16) + (20 << 8) + 176, 0xFF000000 + (160 << 16) + (20 << 8) + 100,
            0xFF000000 + (152 << 16) + (34 << 8) + 32, 0xFF000000 + (120 << 16) + (60 << 8) + 0,
            0xFF000000 + (84 << 16) + (90 << 8) + 0, 0xFF000000 + (40 << 16) + (114 << 8) + 0,
            0xFF000000 + (8 << 16) + (124 << 8) + 0, 0xFF000000 + (0 << 16) + (118 << 8) + 40,
            0xFF000000 + (0 << 16) + (102 << 8) + 120, 0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (236 << 16) + (238 << 8) + 236, 0xFF000000 + (76 << 16) + (154 << 8) + 236,
            0xFF000000 + (120 << 16) + (124 << 8) + 236, 0xFF000000 + (176 << 16) + (98 << 8) + 236,
            0xFF000000 + (228 << 16) + (84 << 8) + 236, 0xFF000000 + (236 << 16) + (88 << 8) + 180,
            0xFF000000 + (236 << 16) + (106 << 8) + 100, 0xFF000000 + (212 << 16) + (136 << 8) + 32,
            0xFF000000 + (160 << 16) + (170 << 8) + 0, 0xFF000000 + (116 << 16) + (196 << 8) + 0,
            0xFF000000 + (76 << 16) + (208 << 8) + 32, 0xFF000000 + (56 << 16) + (204 << 8) + 108,
            0xFF000000 + (56 << 16) + (180 << 8) + 204, 0xFF000000 + (60 << 16) + (60 << 8) + 60,
            0xFF000000 + (236 << 16) + (238 << 8) + 236, 0xFF000000 + (168 << 16) + (204 << 8) + 236,
            0xFF000000 + (188 << 16) + (188 << 8) + 236, 0xFF000000 + (212 << 16) + (178 << 8) + 236,
            0xFF000000 + (236 << 16) + (174 << 8) + 236, 0xFF000000 + (236 << 16) + (174 << 8) + 212,
            0xFF000000 + (236 << 16) + (180 << 8) + 176, 0xFF000000 + (228 << 16) + (196 << 8) + 144,
            0xFF000000 + (204 << 16) + (210 << 8) + 120, 0xFF000000 + (180 << 16) + (222 << 8) + 120,
            0xFF000000 + (168 << 16) + (226 << 8) + 144, 0xFF000000 + (152 << 16) + (226 << 8) + 180,
            0xFF000000 + (160 << 16) + (214 << 8) + 228, 0xFF000000 + (160 << 16) + (162 << 8) + 160,
        };

        public Ppu()
        {
            Renderer = new PpuRenderer();
        }

        public void Write(uint address, uint value)
        {
            switch (address)
            {
                case PPUCTRL:
                    control = value;
                    break;
                case PPUMASK:
                    mask = value;
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
                    if (address_latch == 0)
                    {
                        // write low byte
                        ppu_address = (ppu_address & 0x00FF) | data;
                        address_latch = 1;
                    }
                    else
                    {
                        // write high byte
                        ppu_address = (ppu_address & 0xFF00) | (data << 8);
                        address_latch = 0;
                    }
                    break;
                case PPUDATA:
                    PpuWrite((int)ppu_address, data);
                    ppu_address++;
                    break;
            }
        }

        private uint address_latch;
        private uint ppu_data_buffer;
        private uint ppu_address;

        public uint Read(uint address)
        {
            uint data = 0;
            switch (address)
            {
                case PPUCTRL:
                    break;
                case PPUMASK:
                    break;
                case PPUSTATUS:
                    // TEST: Set VBlank for now
                    status |= 0x80;
                    data = (status & 0xE0) | (ppu_data_buffer & 0x1F);
                    status &= 0x7E; // Reset VBlank
                    address_latch = 0;
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
                    data = ppu_data_buffer;
                    ppu_data_buffer = PpuRead(ppu_address);

                    if (ppu_address > 0x3F00)
                    {
                        data = ppu_data_buffer;
                    }
                    ppu_address++;

                    break;
            }
            return data;
        }

        private Random r = new();

        public void Reset()
        {
            cycles = 0;
            cycles = 0;
            scanline = -1;
        }

        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
        }

        public uint PpuRead(uint addr)
        {
            uint data = 0x00;
            addr &= 0x3FFF;

            var (value, handled) = _cart.PpuRead(addr);
            if (handled)
            {
                data = value;
            }
            else if (addr >= 0x0000 && addr <= 0x1FFF)
            {
                // If the cartridge cant map the address, have
                // a physical location ready here
                data = tblPattern[(addr & 0x1000) >> 12, addr & 0x0FFF];
            }
            else if (addr >= 0x2000 && addr <= 0x3EFF)
            {
                addr &= 0x0FFF;

                if (_cart.Mirror == MirrorEnum.Vertical)
                {
                    // Vertical
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        data = tblName[1, addr & 0x03FF];
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        data = tblName[1, addr & 0x03FF];
                }
                else if (_cart.Mirror == MirrorEnum.Horizontal)
                {
                    // Horizontal
                    if (addr >= 0x0000 && addr <= 0x03FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0400 && addr <= 0x07FF)
                        data = tblName[0, addr & 0x03FF];
                    if (addr >= 0x0800 && addr <= 0x0BFF)
                        data = tblName[1, addr & 0x03FF];
                    if (addr >= 0x0C00 && addr <= 0x0FFF)
                        data = tblName[1, addr & 0x03FF];
                }
            }
            else if (addr >= 0x3F00 && addr <= 0x3FFF)
            {
                addr &= 0x001F;
                if (addr == 0x0010) addr = 0x0000;
                if (addr == 0x0014) addr = 0x0004;
                if (addr == 0x0018) addr = 0x0008;
                if (addr == 0x001C) addr = 0x000C;
                data = tblPalette[addr] & (/*mask.grayscale ? 0x30 :*/ 0x3F);
            }

            return data;
        }

        public void PpuWrite(int address, uint value)
        {
            _cart.PpuWrite(address, value);
        }

        private int cycle;
        private int scanline;
        private bool frameComplete;
        private uint tileY;
        private uint tileX;
        private uint row;
        private uint col;
        private uint offset;
        private uint tileLsb;
        private uint tileMsb;
        private uint paletteId = 0;
        private uint tableId = 0;

        public void Clock()
        {
            cycle++;

            //if (scanline >= 0 && scanline < 240 & cycle >= 0 && cycle < 256)
            //{
            //    Renderer.buffer[cycle + scanline * 256] = (uint)PpuRead(cycle + scanline * 256);

            //    //Renderer.buffer[cycle + scanline * 256] = Palette[r.Next(32)];
            //}



            col++;

            if (col >= 8)
            {
                col = 0;
                row++;

                if (row >= 8)
                {
                    row = 0;
                    tileX++;

                    if (tileX >= 16)
                    {
                        tileX = 0;
                        tileY++;

                        offset = tileY * 256 + tileX * 16;
                        tileLsb = PpuRead(tableId * 0x1000 + offset + row + 0);
                        tileMsb = PpuRead(tableId * 0x1000 + offset + row + 8);
                    }
                    else
                    {
                        offset = tileY * 256 + tileX * 16;
                        tileLsb = PpuRead(tableId * 0x1000 + offset + row + 0);
                        tileMsb = PpuRead(tableId * 0x1000 + offset + row + 8);
                    }
                }
                else
                {
                    offset = tileY * 256 + tileX * 16;
                    tileLsb = PpuRead(tableId * 0x1000 + offset + row + 0);
                    tileMsb = PpuRead(tableId * 0x1000 + offset + row + 8);
                }
            }

            var pixel = (tileLsb & 0x01) + (tileMsb & 0x01);
            tileLsb >>= 1;
            tileMsb >>= 1;

            if (tileY < 16)
            {
                var palIdx = PpuRead((0x3F00 + (paletteId << 2) + pixel));
                Renderer.buffer[(tileX * 8 + (7 - col)) + (tileY * 8 + row) * 256] = tblPalette[palIdx];
            }


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
            cycles++;
        }

    }
}