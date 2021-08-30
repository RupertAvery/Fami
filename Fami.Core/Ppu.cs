using System;

namespace Fami.Core
{
    public class OAM
    {
        public uint Y { get; set; }
        public uint Id { get; set; }
        public uint Attribute { get; set; }
        public uint X { get; set; }
    }

    public class Ppu
    {
        private readonly Cpu6502State _state;
        
        private StatusRegister ppu_status = new();
        private ControlRegister ppu_control = new();
        private MaskRegister ppu_mask = new();
        private LoopyRegister vram_addr = new();
        private LoopyRegister tram_addr = new();

        private uint fine_x;

        private const uint PPUCTRL = 0x2000;
        private const uint PPUMASK = 0x2001;
        private const uint PPUSTATUS = 0x2002;
        private const uint OAMADDR = 0x2003;
        private const uint OAMDATA = 0x2004;
        private const uint PPUSCROLL = 0x2005;
        private const uint PPUADDR = 0x2006;
        private const uint PPUDATA = 0x2007;

        private const uint OAMDMA = 0x4014;

        private uint bg_shifter_pattern_lo;
        private uint bg_next_tile_lsb;
        private uint bg_shifter_pattern_hi;
        private uint bg_next_tile_msb;
        private uint bg_shifter_attrib_lo;
        private uint bg_shifter_attrib_hi;
        private uint bg_next_tile_attrib;

        private uint bg_next_tile_id;

        // 341 ppuccs = 1 scanline
        // 262 scanlines = 1 frame
        // 89342 ppuccs = 29780 cpuccs
        public PpuRenderer Renderer { get; private set; }
        public ulong cycles;

        private uint[,] tblName = new uint[2, 1024];
        private uint[] tblPalette = new uint[32];
        private uint[,] tblPattern = new uint[2, 4096];
        private uint[] tblOAM = new uint[1];

        private Cartridge _cart;

        private uint[] palScreen = new uint[]
        {
            (84 << 0) + (84 << 8) + (84 << 16),
            (0 << 0) + (30 << 8) + (116 << 16),
            (8 << 0) + (16 << 8) + (144 << 16),
            (48 << 0) + (0 << 8) + (136 << 16),
            (68 << 0) + (0 << 8) + (100 << 16),
            (92 << 0) + (0 << 8) + (48 << 16),
            (84 << 0) + (4 << 8) + (0 << 16),
            (60 << 0) + (24 << 8) + (0 << 16),
            (32 << 0) + (42 << 8) + (0 << 16),
            (8 << 0) + (58 << 8) + (0 << 16),
            (0 << 0) + (64 << 8) + (0 << 16),
            (0 << 0) + (60 << 8) + (0 << 16),
            (0 << 0) + (50 << 8) + (60 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),

            (152 << 0) + (150 << 8) + (152 << 16),
            (8 << 0) + (76 << 8) + (196 << 16),
            (48 << 0) + (50 << 8) + (236 << 16),
            (92 << 0) + (30 << 8) + (228 << 16),
            (136 << 0) + (20 << 8) + (176 << 16),
            (160 << 0) + (20 << 8) + (100 << 16),
            (152 << 0) + (34 << 8) + (32 << 16),
            (120 << 0) + (60 << 8) + (0 << 16),
            (84 << 0) + (90 << 8) + (0 << 16),
            (40 << 0) + (114 << 8) + (0 << 16),
            (8 << 0) + (124 << 8) + (0 << 16),
            (0 << 0) + (118 << 8) + (40 << 16),
            (0 << 0) + (102 << 8) + (120 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),

            (236 << 0) + (238 << 8) + (236 << 16),
            (76 << 0) + (154 << 8) + (236 << 16),
            (120 << 0) + (124 << 8) + (236 << 16),
            (176 << 0) + (98 << 8) + (236 << 16),
            (228 << 0) + (84 << 8) + (236 << 16),
            (236 << 0) + (88 << 8) + (180 << 16),
            (236 << 0) + (106 << 8) + (100 << 16),
            (212 << 0) + (136 << 8) + (32 << 16),
            (160 << 0) + (170 << 8) + (0 << 16),
            (116 << 0) + (196 << 8) + (0 << 16),
            (76 << 0) + (208 << 8) + (32 << 16),
            (56 << 0) + (204 << 8) + (108 << 16),
            (56 << 0) + (180 << 8) + (204 << 16),
            (60 << 0) + (60 << 8) + (60 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),

            (236 << 0) + (238 << 8) + (236 << 16),
            (168 << 0) + (204 << 8) + (236 << 16),
            (188 << 0) + (188 << 8) + (236 << 16),
            (212 << 0) + (178 << 8) + (236 << 16),
            (236 << 0) + (174 << 8) + (236 << 16),
            (236 << 0) + (174 << 8) + (212 << 16),
            (236 << 0) + (180 << 8) + (176 << 16),
            (228 << 0) + (196 << 8) + (144 << 16),
            (204 << 0) + (210 << 8) + (120 << 16),
            (180 << 0) + (222 << 8) + (120 << 16),
            (168 << 0) + (226 << 8) + (144 << 16),
            (152 << 0) + (226 << 8) + (180 << 16),
            (160 << 0) + (214 << 8) + (228 << 16),
            (160 << 0) + (162 << 8) + (160 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
            (0 << 0) + (0 << 8) + (0 << 16),
        };

        public Ppu(Cpu6502State state)
        {
            _state = state;
            Renderer = new PpuRenderer();
            for (var i = 0; i < 64; i++)
            {
                spriteScanline[i] = new SpriteScanline();
            }
        }

        private uint oam_addr;

        public void Write(uint address, uint data)
        {
            switch (address)
            {
                case PPUCTRL:
                    ppu_control.Register = data;
                    tram_addr.NametableX = ppu_control.NametableX;
                    tram_addr.NametableY = ppu_control.NametableY;
                    break;
                case PPUMASK:
                    ppu_mask.Register = data;
                    break;
                case PPUSTATUS:
                    break;
                case OAMADDR:
                    oam_addr = data;
                    break;
                case OAMDATA:
                    pOAM[oam_addr] = data;
                    break;
                case PPUSCROLL:
                    if (address_latch == 0)
                    {
                        // First write to scroll register contains X offset in pixel space
                        // which we split into coarse and fine x values
                        fine_x = data & 0x07;
                        tram_addr.CoarseX = data >> 3;
                        address_latch = 1;
                    }
                    else
                    {
                        // First write to scroll register contains Y offset in pixel space
                        // which we split into coarse and fine Y values
                        tram_addr.FineY = data & 0x07;
                        tram_addr.CoarseY = data >> 3;
                        address_latch = 0;
                    }
                    break;
                case PPUADDR:
                    if (address_latch == 0)
                    {
                        // PPU address bus can be accessed by CPU via the ADDR and DATA
                        // registers. The fisrt write to this register latches the high byte
                        // of the address, the second is the low byte. Note the writes
                        // are stored in the tram register...
                        tram_addr.Register = ((data & 0x3F) << 8) | (tram_addr.Register & 0x00FF);
                        address_latch = 1;
                    }
                    else
                    {
                        // ...when a whole address has been written, the internal vram address
                        // buffer is updated. Writing to the PPU is unwise during rendering
                        // as the PPU will maintam the vram address automatically whilst
                        // rendering the scanline position.
                        tram_addr.Register = (tram_addr.Register & 0xFF00) | data;
                        vram_addr.Register = tram_addr.Register;

                        address_latch = 0;
                    }
                    break;
                case PPUDATA:

                    PpuWrite(vram_addr.Register, data);
                    // All writes from PPU data automatically increment the nametable
                    // address depending upon the mode set in the control register.
                    // If set to vertical mode, the increment is 32, so it skips
                    // one whole nametable row; in horizontal mode it just increments
                    // by 1, moving to the next column
                    vram_addr.Register += ((ppu_control.IncrementMode == 1) ? 32U : 1U);
                    break;
            }
        }

        private uint address_latch;
        private uint ppu_data_buffer;

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
                    data = (ppu_status.Register & 0xE0) | (ppu_data_buffer & 0x1F);
                    ppu_status.VerticalBlank = 0; // Clear v-blank
                    address_latch = 0;
                    break;
                case OAMADDR:
                    break;
                case OAMDATA:
                    data = pOAM[oam_addr];
                    break;
                case PPUSCROLL:
                    break;
                case PPUADDR:
                    break;
                case PPUDATA:
                    data = ppu_data_buffer;
                    ppu_data_buffer = PpuRead(vram_addr.Register);

                    if (vram_addr.Register > 0x3F00)
                    {
                        data = ppu_data_buffer;
                    }

                    vram_addr.Register += ((ppu_control.IncrementMode == 1) ? 32U : 1U);

                    break;
            }
            return data;
        }

        public void Reset()
        {
            cycle = 0;
            cycles = 0;
            scanline = -1;
        }

        public void LoadCartridge(Cartridge cart)
        {
            _cart = cart;
        }

        public uint PpuRead(uint address)
        {
            uint data = 0x00;
            address &= 0x3FFF;

            var (value, handled) = _cart.PpuRead(address);
            if (handled)
            {
                data = value;
            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                // If the cartridge cant map the address, have
                // a physical location ready here
                data = tblPattern[(address & 0x1000) >> 12, address & 0x0FFF];
            }
            else if (address >= 0x2000 && address <= 0x3EFF)
            {
                address &= 0x0FFF;

                if (_cart.Mirror == MirrorEnum.Vertical)
                {
                    // Vertical
                    if (address >= 0x0000 && address <= 0x03FF)
                        data = tblName[0, address & 0x03FF];
                    if (address >= 0x0400 && address <= 0x07FF)
                        data = tblName[1, address & 0x03FF];
                    if (address >= 0x0800 && address <= 0x0BFF)
                        data = tblName[0, address & 0x03FF];
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        data = tblName[1, address & 0x03FF];
                }
                else if (_cart.Mirror == MirrorEnum.Horizontal)
                {
                    // Horizontal
                    if (address >= 0x0000 && address <= 0x03FF)
                        data = tblName[0, address & 0x03FF];
                    if (address >= 0x0400 && address <= 0x07FF)
                        data = tblName[0, address & 0x03FF];
                    if (address >= 0x0800 && address <= 0x0BFF)
                        data = tblName[1, address & 0x03FF];
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        data = tblName[1, address & 0x03FF];
                }
            }
            else if (address >= 0x3F00 && address <= 0x3FFF)
            {
                address &= 0x001F;
                if (address == 0x0010) address = 0x0000;
                if (address == 0x0014) address = 0x0004;
                if (address == 0x0018) address = 0x0008;
                if (address == 0x001C) address = 0x000C;
                data = tblPalette[address] & ((ppu_mask.Grayscale == 1) ? 0x30U : 0x3FU);
            }

            return data;
        }

        public void PpuWrite(uint address, uint data)
        {
            address &= 0x3FFF;

            var handled = _cart.PpuWrite(address, data);
            if (handled)
            {
            }
            else if (address >= 0x0000 && address <= 0x1FFF)
            {
                // If the cartridge cant map the address, have
                // a physical location ready here
                tblPattern[(address & 0x1000) >> 12, address & 0x0FFF] = data;
            }
            else if (address >= 0x2000 && address <= 0x3EFF)
            {
                address &= 0x0FFF;

                if (_cart.Mirror == MirrorEnum.Vertical)
                {
                    // Vertical
                    if (address >= 0x0000 && address <= 0x03FF)
                        tblName[0, address & 0x03FF] = data;
                    if (address >= 0x0400 && address <= 0x07FF)
                        tblName[1, address & 0x03FF] = data;
                    if (address >= 0x0800 && address <= 0x0BFF)
                        tblName[0, address & 0x03FF] = data;
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        tblName[1, address & 0x03FF] = data;
                }
                else if (_cart.Mirror == MirrorEnum.Horizontal)
                {
                    // Horizontal
                    if (address >= 0x0000 && address <= 0x03FF)
                        tblName[0, address & 0x03FF] = data;
                    if (address >= 0x0400 && address <= 0x07FF)
                        tblName[0, address & 0x03FF] = data;
                    if (address >= 0x0800 && address <= 0x0BFF)
                        tblName[1, address & 0x03FF] = data;
                    if (address >= 0x0C00 && address <= 0x0FFF)
                        tblName[1, address & 0x03FF] = data;
                }
            }
            else if (address >= 0x3F00 && address <= 0x3FFF)
            {
                address &= 0x001F;
                if (address == 0x0010) address = 0x0000;
                if (address == 0x0014) address = 0x0004;
                if (address == 0x0018) address = 0x0008;
                if (address == 0x001C) address = 0x000C;
                tblPalette[address] = data;
            }
        }

        private int cycle;
        private int scanline;
        private bool frameComplete;
        public uint[] pOAM = new uint[256];
        public uint sprite_count;
        public SpriteScanline[] spriteScanline = new SpriteScanline[64];
        public uint[] sprite_shifter_pattern_lo = new uint[64];
        public uint[] sprite_shifter_pattern_hi = new uint[64];
        public bool bSpriteZeroBeingRendered;
        public bool bSpriteZeroHitPossible;

        public class SpriteScanline
        {
            public uint X { get; set; }
            public uint Y { get; set; }
            public uint Attribute { get; set; }
            public uint Id { get; set; }
        }

        public void Clock()
        {
            void IncrementScrollX()
            {
                // Note: pixel perfect scrolling horizontally is handled by the 
                // data shifters. Here we are operating in the spatial domain of 
                // tiles, 8x8 pixel blocks.

                // Ony if rendering is enabled
                if ((ppu_mask.RenderBackground == 1) || (ppu_mask.RenderSprites == 1))
                {
                    // A single name table is 32x30 tiles. As we increment horizontally
                    // we may cross into a neighbouring nametable, or wrap around to
                    // a neighbouring nametable

                    if (vram_addr.CoarseX == 31)
                    {
                        // Leaving nametable so wrap address round
                        vram_addr.CoarseX = 0;
                        // Flip target nametable bit
                        vram_addr.NametableX = ~vram_addr.NametableX;
                    }
                    else
                    {
                        // Staying in current nametable, so just increment
                        vram_addr.CoarseX++;
                    }
                }
            };

            void IncrementScrollY()
            {
                if ((ppu_mask.RenderBackground == 1) || (ppu_mask.RenderSprites == 1))
                {
                    // If possible, just increment the fine y offset
                    if (vram_addr.FineY < 7)
                    {
                        vram_addr.FineY++;
                    }
                    else
                    {
                        // If we have gone beyond the height of a row, we need to
                        // increment the row, potentially wrapping into neighbouring
                        // vertical nametables. Dont forget however, the bottom two rows
                        // do not contain tile information. The coarse y offset is used
                        // to identify which row of the nametable we want, and the fine
                        // y offset is the specific "scanline"

                        // Reset fine y offset
                        vram_addr.FineY = 0;

                        // Check if we need to swap vertical nametable targets
                        if (vram_addr.CoarseY == 29)
                        {
                            // We do, so reset coarse y offset
                            vram_addr.CoarseY = 0;
                            // And flip the target nametable bit
                            vram_addr.NametableY = ~vram_addr.NametableY;
                        }
                        else if (vram_addr.CoarseY == 31)
                        {
                            // In case the pointer is in the attribute memory, we
                            // just wrap around the current nametable
                            vram_addr.CoarseY = 0;
                        }
                        else
                        {
                            // None of the above boundary/wrapping conditions apply
                            // so just increment the coarse y offset
                            vram_addr.CoarseY++;
                        }
                    }
                }
            };

            // ==============================================================================
            // Transfer the temporarily stored horizontal nametable access information
            // into the "pointer". Note that fine x scrolling is not part of the "pointer"
            // addressing mechanism
            void TransferAddressX()
            {
                // Ony if rendering is enabled
                if ((ppu_mask.RenderBackground == 1) || (ppu_mask.RenderSprites == 1))
                {
                    vram_addr.NametableX = tram_addr.NametableX;
                    vram_addr.CoarseX = tram_addr.CoarseX;
                }
            };

            // ==============================================================================
            // Transfer the temporarily stored vertical nametable access information
            // into the "pointer". Note that fine y scrolling is part of the "pointer"
            // addressing mechanism
            void TransferAddressY()
            {
                // Ony if rendering is enabled
                if ((ppu_mask.RenderBackground == 1) || (ppu_mask.RenderSprites == 1))
                {
                    vram_addr.FineY = tram_addr.FineY;
                    vram_addr.NametableY = tram_addr.NametableY;
                    vram_addr.CoarseY = tram_addr.CoarseY;
                }
            };


            // ==============================================================================
            // Prime the "in-effect" background tile shifters ready for outputting next
            // 8 pixels in scanline.
            void LoadBackgroundShifters()
            {
                // Each PPU update we calculate one pixel. These shifters shift 1 bit along
                // feeding the pixel compositor with the binary information it needs. Its
                // 16 bits wide, because the top 8 bits are the current 8 pixels being drawn
                // and the bottom 8 bits are the next 8 pixels to be drawn. Naturally this means
                // the required bit is always the MSB of the shifter. However, "fine x" scrolling
                // plays a part in this too, whcih is seen later, so in fact we can choose
                // any one of the top 8 bits.
                bg_shifter_pattern_lo = (bg_shifter_pattern_lo & 0xFF00) | bg_next_tile_lsb;
                bg_shifter_pattern_hi = (bg_shifter_pattern_hi & 0xFF00) | bg_next_tile_msb;

                // Attribute bits do not change per pixel, rather they change every 8 pixels
                // but are synchronised with the pattern shifters for convenience, so here
                // we take the bottom 2 bits of the attribute word which represent which 
                // palette is being used for the current 8 pixels and the next 8 pixels, and 
                // "inflate" them to 8 bit words

                bg_shifter_attrib_lo = (uint)((bg_shifter_attrib_lo & 0xFF00) | (((bg_next_tile_attrib & 0b01) == 0b01) ? 0xFF : 0x00));
                bg_shifter_attrib_hi = (uint)((bg_shifter_attrib_hi & 0xFF00) | (((bg_next_tile_attrib & 0b10) == 0b10) ? 0xFF : 0x00));
            };


            // ==============================================================================
            // Every cycle the shifters storing pattern and attribute information shift
            // their contents by 1 bit. This is because every cycle, the output progresses
            // by 1 pixel. This means relatively, the state of the shifter is in sync
            // with the pixels being drawn for that 8 pixel section of the scanline.
            void UpdateShifters()
            {
                if (ppu_mask.RenderBackground == 1)
                {
                    // Shifting background tile pattern row
                    bg_shifter_pattern_lo <<= 1;
                    bg_shifter_pattern_hi <<= 1;

                    // Shifting palette attributes by 1
                    bg_shifter_attrib_lo <<= 1;
                    bg_shifter_attrib_hi <<= 1;
                }
                if (ppu_mask.RenderSprites == 1 && cycle >= 1 && cycle < 258)
                {
                    for (int i = 0; i < sprite_count; i++)
                    {
                        if (spriteScanline[i].X > 0)
                        {
                            spriteScanline[i].X--;
                        }
                        else
                        {
                            sprite_shifter_pattern_lo[i] <<= 1;
                            sprite_shifter_pattern_hi[i] <<= 1;
                        }
                    }
                }
            };

            // All but 1 of the secanlines is visible to the user. The pre-render scanline
            // at -1, is used to configure the "shifters" for the first visible scanline, 0.
            if (scanline >= -1 && scanline < 240)
            {
                if (scanline == 0 && cycle == 0)
                {
                    // "Odd Frame" cycle skip
                    cycle = 1;
                }

                if (scanline == -1 && cycle == 1)
                {
                    // Effectively start of new frame, so clear vertical blank flag
                    ppu_status.VerticalBlank = 0;

                    // Clear sprite overflow flag
                    ppu_status.SpriteOverflow = 0;

                    // Clear the sprite zero hit flag
                    ppu_status.SpriteZeroHit = 0;

                    // Clear Shifters
                    for (int i = 0; i < 8; i++)
                    {
                        sprite_shifter_pattern_lo[i] = 0;
                        sprite_shifter_pattern_hi[i] = 0;
                    }
                }


                if ((cycle >= 2 && cycle < 258) || (cycle >= 321 && cycle < 338))
                {
                    UpdateShifters();


                    // In these cycles we are collecting and working with visible data
                    // The "shifters" have been preloaded by the end of the previous
                    // scanline with the data for the start of this scanline. Once we
                    // leave the visible region, we go dormant until the shifters are
                    // preloaded for the next scanline.

                    // Fortunately, for background rendering, we go through a fairly
                    // repeatable sequence of events, every 2 clock cycles.
                    switch ((cycle - 1) % 8)
                    {
                        case 0:
                            // Load the current background tile pattern and attributes into the "shifter"
                            LoadBackgroundShifters();

                            // Fetch the next background tile ID
                            // "(vram_addr.reg & 0x0FFF)" : Mask to 12 bits that are relevant
                            // "| 0x2000"                 : Offset into nametable space on PPU address bus
                            bg_next_tile_id = PpuRead(0x2000 | (vram_addr.Register & 0x0FFF));

                            // Explanation:
                            // The bottom 12 bits of the loopy register provide an index into
                            // the 4 nametables, regardless of nametable mirroring configuration.
                            // nametable_y(1) nametable_x(1) coarse_y(5) coarse_x(5)
                            //
                            // Consider a single nametable is a 32x32 array, and we have four of them
                            //   0                1
                            // 0 +----------------+----------------+
                            //   |                |                |
                            //   |                |                |
                            //   |    (32x32)     |    (32x32)     |
                            //   |                |                |
                            //   |                |                |
                            // 1 +----------------+----------------+
                            //   |                |                |
                            //   |                |                |
                            //   |    (32x32)     |    (32x32)     |
                            //   |                |                |
                            //   |                |                |
                            //   +----------------+----------------+
                            //
                            // This means there are 4096 potential locations in this array, which 
                            // just so happens to be 2^12!
                            break;
                        case 2:
                            // Fetch the next background tile attribute. OK, so this one is a bit
                            // more involved :P

                            // Recall that each nametable has two rows of cells that are not tile 
                            // information, instead they represent the attribute information that
                            // indicates which palettes are applied to which area on the screen.
                            // Importantly (and frustratingly) there is not a 1 to 1 correspondance
                            // between background tile and palette. Two rows of tile data holds
                            // 64 attributes. Therfore we can assume that the attributes affect
                            // 8x8 zones on the screen for that nametable. Given a working resolution
                            // of 256x240, we can further assume that each zone is 32x32 pixels
                            // in screen space, or 4x4 tiles. Four system palettes are allocated
                            // to background rendering, so a palette can be specified using just
                            // 2 bits. The attribute byte therefore can specify 4 distinct palettes.
                            // Therefore we can even further assume that a single palette is
                            // applied to a 2x2 tile combination of the 4x4 tile zone. The very fact
                            // that background tiles "share" a palette locally is the reason why
                            // in some games you see distortion in the colours at screen edges.

                            // As before when choosing the tile ID, we can use the bottom 12 bits of
                            // the loopy register, but we need to make the implementation "coarser"
                            // because instead of a specific tile, we want the attribute byte for a 
                            // group of 4x4 tiles, or in other words, we divide our 32x32 address
                            // by 4 to give us an equivalent 8x8 address, and we offset this address
                            // into the attribute section of the target nametable.

                            // Reconstruct the 12 bit loopy address into an offset into the
                            // attribute memory

                            // "(vram_addr.coarse_x >> 2)"        : integer divide coarse x by 4, 
                            //                                      from 5 bits to 3 bits
                            // "((vram_addr.coarse_y >> 2) << 3)" : integer divide coarse y by 4, 
                            //                                      from 5 bits to 3 bits,
                            //                                      shift to make room for coarse x

                            // Result so far: YX00 00yy yxxx

                            // All attribute memory begins at 0x03C0 within a nametable, so OR with
                            // result to select target nametable, and attribute byte offset. Finally
                            // OR with 0x2000 to offset into nametable address space on PPU bus.				
                            bg_next_tile_attrib = PpuRead(0x23C0 | (vram_addr.NametableY  << 11)
                                                                 | (vram_addr.NametableX << 10)
                                                                 | ((vram_addr.CoarseY >> 2) << 3)
                                                                 | (vram_addr.CoarseX >> 2));

                            // Right we've read the correct attribute byte for a specified address,
                            // but the byte itself is broken down further into the 2x2 tile groups
                            // in the 4x4 attribute zone.

                            // The attribute byte is assembled thus: BR(76) BL(54) TR(32) TL(10)
                            //
                            // +----+----+			    +----+----+
                            // | TL | TR |			    | ID | ID |
                            // +----+----+ where TL =   +----+----+
                            // | BL | BR |			    | ID | ID |
                            // +----+----+			    +----+----+
                            //
                            // Since we know we can access a tile directly from the 12 bit address, we
                            // can analyse the bottom bits of the coarse coordinates to provide us with
                            // the correct offset into the 8-bit word, to yield the 2 bits we are
                            // actually interested in which specifies the palette for the 2x2 group of
                            // tiles. We know if "coarse y % 4" < 2 we are in the top half else bottom half.
                            // Likewise if "coarse x % 4" < 2 we are in the left half else right half.
                            // Ultimately we want the bottom two bits of our attribute word to be the
                            // palette selected. So shift as required...				
                            if ((vram_addr.CoarseY & 0x02) == 0x02) bg_next_tile_attrib >>= 4;
                            if ((vram_addr.CoarseX & 0x02) == 0x02) bg_next_tile_attrib >>= 2;
                            bg_next_tile_attrib &= 0x03;
                            break;

                        // Compared to the last two, the next two are the easy ones... :P

                        case 4:
                            // Fetch the next background tile LSB bit plane from the pattern memory
                            // The Tile ID has been read from the nametable. We will use this id to 
                            // index into the pattern memory to find the correct sprite (assuming
                            // the sprites lie on 8x8 pixel boundaries in that memory, which they do
                            // even though 8x16 sprites exist, as background tiles are always 8x8).
                            //
                            // Since the sprites are effectively 1 bit deep, but 8 pixels wide, we 
                            // can represent a whole sprite row as a single byte, so offsetting
                            // into the pattern memory is easy. In total there is 8KB so we need a 
                            // 13 bit address.

                            // "(control.pattern_background << 12)"  : the pattern memory selector 
                            //                                         from control register, either 0K
                            //                                         or 4K offset
                            // "((uint16_t)bg_next_tile_id << 4)"    : the tile id multiplied by 16, as
                            //                                         2 lots of 8 rows of 8 bit pixels
                            // "(vram_addr.fine_y)"                  : Offset into which row based on
                            //                                         vertical scroll offset
                            // "+ 0"                                 : Mental clarity for plane offset
                            // Note: No PPU address bus offset required as it starts at 0x0000
                            bg_next_tile_lsb = PpuRead((ppu_control.PatternBackground << 12)
                                                       + (bg_next_tile_id << 4)
                                                       + (vram_addr.FineY) + 0);

                            break;
                        case 6:
                            // Fetch the next background tile MSB bit plane from the pattern memory
                            // This is the same as above, but has a +8 offset to select the next bit plane
                            bg_next_tile_msb = PpuRead((ppu_control.PatternBackground << 12)
                                                       + (bg_next_tile_id << 4)
                                                       + (vram_addr.FineY) + 8);
                            break;
                        case 7:
                            // Increment the background tile "pointer" to the next tile horizontally
                            // in the nametable memory. Note this may cross nametable boundaries which
                            // is a little complex, but essential to implement scrolling
                            IncrementScrollX();
                            break;
                    }
                }

                // End of a visible scanline, so increment downwards...
                if (cycle == 256)
                {
                    IncrementScrollY();
                }

                //...and reset the x position
                if (cycle == 257)
                {
                    LoadBackgroundShifters();
                    TransferAddressX();
                }

                // Superfluous reads of tile id at end of scanline
                if (cycle == 338 || cycle == 340)
                {
                    bg_next_tile_id = PpuRead(0x2000 | (vram_addr.Register & 0x0FFF));
                }

                if (scanline == -1 && cycle >= 280 && cycle < 305)
                {
                    // End of vertical blank period so reset the Y address ready for rendering
                    TransferAddressY();
                }
            }


            // Foreground Rendering ========================================================
            // I'm gonna cheat a bit here, which may reduce compatibility, but greatly
            // simplifies delivering an intuitive understanding of what exactly is going
            // on. The PPU loads sprite information successively during the region that
            // background tiles are not being drawn. Instead, I'm going to perform
            // all sprite evaluation in one hit. THE NES DOES NOT DO IT LIKE THIS! This makes
            // it easier to see the process of sprite evaluation.
            if (cycle == 257 && scanline >= 0)
            {
                // We've reached the end of a visible scanline. It is now time to determine
                // which sprites are visible on the next scanline, and preload this info
                // into buffers that we can work with while the scanline scans the row.

                // Firstly, clear out the sprite memory. This memory is used to store the
                // sprites to be rendered. It is not the OAM.
                //std::memset(spriteScanline, 0xFF, 8 * sizeof(sObjectAttributeEntry));

                foreach (var sprite in spriteScanline)
                {
                    sprite.Y = 0xff;
                    sprite.Id = 0xff;
                    sprite.Attribute = 0xff;
                    sprite.X = 0xff;
                }
                // The NES supports a maximum number of sprites per scanline. Nominally
                // this is 8 or fewer sprites. This is why in some games you see sprites
                // flicker or disappear when the scene gets busy.
                sprite_count = 0;

                // Secondly, clear out any residual information in sprite pattern shifters
                for (uint i = 0; i < 8; i++)
                {
                    sprite_shifter_pattern_lo[i] = 0;
                    sprite_shifter_pattern_hi[i] = 0;
                }

                // Thirdly, Evaluate which sprites are visible in the next scanline. We need
                // to iterate through the OAM until we have found 8 sprites that have Y-positions
                // and heights that are within vertical range of the next scanline. Once we have
                // found 8 or exhausted the OAM we stop. Now, notice I count to 9 sprites. This
                // is so I can set the sprite overflow flag in the event of there being > 8 sprites.
                uint nOAMEntry = 0;

                // New set of sprites. Sprite zero may not exist in the new set, so clear this
                // flag.
                bSpriteZeroHitPossible = false;

                while (nOAMEntry < 64 && sprite_count < 9)
                {
                    // Note the conversion to signed numbers here
                    int diff = ((int)scanline - (int)spriteScanline[nOAMEntry].Y);

                    // If the difference is positive then the scanline is at least at the
                    // same height as the sprite, so check if it resides in the sprite vertically
                    // depending on the current "sprite height mode"
                    // FLAGGED

                    if (diff >= 0 && diff < (ppu_control.SpriteSize == 1 ? 16 : 8))
                    {
                        // Sprite is visible, so copy the attribute entry over to our
                        // scanline sprite cache. Ive added < 8 here to guard the array
                        // being written to.
                        if (sprite_count < 8)
                        {
                            // Is this sprite sprite zero?
                            if (nOAMEntry == 0)
                            {
                                // It is, so its possible it may trigger a 
                                // sprite zero hit when drawn
                                bSpriteZeroHitPossible = true;
                            }

                            spriteScanline[sprite_count].Y = pOAM[nOAMEntry * 4];
                            spriteScanline[sprite_count].Id = pOAM[nOAMEntry * 4 + 1];
                            spriteScanline[sprite_count].Attribute = pOAM[nOAMEntry * 4 + 2];
                            spriteScanline[sprite_count].X = pOAM[nOAMEntry * 4 + 3];
                            sprite_count++;
                        }
                    }

                    nOAMEntry++;
                } // End of sprite evaluation for next scanline

                // Set sprite overflow flag
                ppu_status.SpriteOverflow = (sprite_count > 8) ? 1U : 0;

                // Now we have an array of the 8 visible sprites for the next scanline. By 
                // the nature of this search, they are also ranked in priority, because
                // those lower down in the OAM have the higher priority.

                // We also guarantee that "Sprite Zero" will exist in spriteScanline[0] if
                // it is evaluated to be visible. 
            }

            if (cycle == 340)
            {
                // Now we're at the very end of the scanline, I'm going to prepare the 
                // sprite shifters with the 8 or less selected sprites.

                for (uint i = 0; i < sprite_count; i++)
                {
                    // We need to extract the 8-bit row patterns of the sprite with the
                    // correct vertical offset. The "Sprite Mode" also affects this as
                    // the sprites may be 8 or 16 rows high. Additionally, the sprite
                    // can be flipped both vertically and horizontally. So there's a lot
                    // going on here :P

                    uint sprite_pattern_bits_lo, sprite_pattern_bits_hi;
                    uint sprite_pattern_addr_lo, sprite_pattern_addr_hi;

                    // Determine the memory addresses that contain the byte of pattern data. We
                    // only need the lo pattern address, because the hi pattern address is always
                    // offset by 8 from the lo address.
                    if (ppu_control.SpriteSize == 0)
                    {
                        // 8x8 Sprite Mode - The control register determines the pattern table
                        if ((spriteScanline[i].Attribute & 0x80) == 0)
                        {
                            // Sprite is NOT flipped vertically, i.e. normal    
                            sprite_pattern_addr_lo = (uint)(
                              (ppu_control.PatternSprite << 12)  // Which Pattern Table? 0KB or 4KB offset
                            | (spriteScanline[i].Id << 4)  // Which Cell? Tile ID * 16 (16 bytes per tile)
                            | (scanline - spriteScanline[i].Y)); // Which Row in cell? (0->7)

                        }
                        else
                        {
                            // Sprite is flipped vertically, i.e. upside down
                            sprite_pattern_addr_lo = (uint)(
                              (ppu_control.PatternSprite << 12)  // Which Pattern Table? 0KB or 4KB offset
                            | (spriteScanline[i].Id << 4)  // Which Cell? Tile ID * 16 (16 bytes per tile)
                            | (7 - (scanline - spriteScanline[i].Y))); // Which Row in cell? (7->0)
                        }

                    }
                    else
                    {
                        // 8x16 Sprite Mode - The sprite attribute determines the pattern table
                        if ((spriteScanline[i].Attribute & 0x80) == 0)
                        {
                            // Sprite is NOT flipped vertically, i.e. normal
                            if (scanline - spriteScanline[i].Y < 8)
                            {
                                // Reading Top half Tile
                                sprite_pattern_addr_lo = (uint)(
                                  ((spriteScanline[i].Id & 0x01) << 12)  // Which Pattern Table? 0KB or 4KB offset
                                | ((spriteScanline[i].Id & 0xFE) << 4)  // Which Cell? Tile ID * 16 (16 bytes per tile)
                                | ((scanline - spriteScanline[i].Y) & 0x07)); // Which Row in cell? (0->7)
                            }
                            else
                            {
                                // Reading Bottom Half Tile
                                sprite_pattern_addr_lo = (uint)(
                                  ((spriteScanline[i].Id & 0x01) << 12)  // Which Pattern Table? 0KB or 4KB offset
                                | (((spriteScanline[i].Id & 0xFE) + 1) << 4)  // Which Cell? Tile ID * 16 (16 bytes per tile)
                                | ((scanline - spriteScanline[i].Y) & 0x07)); // Which Row in cell? (0->7)
                            }
                        }
                        else
                        {
                            // Sprite is flipped vertically, i.e. upside down
                            if (scanline - spriteScanline[i].Y < 8)
                            {
                                // Reading Top half Tile
                                sprite_pattern_addr_lo = (uint)(
                                  ((spriteScanline[i].Id & 0x01) << 12)    // Which Pattern Table? 0KB or 4KB offset
                                | (((spriteScanline[i].Id & 0xFE) + 1) << 4)    // Which Cell? Tile ID * 16 (16 bytes per tile)
                                | (7 - (scanline - spriteScanline[i].Y) & 0x07)); // Which Row in cell? (0->7)
                            }
                            else
                            {
                                // Reading Bottom Half Tile
                                sprite_pattern_addr_lo = (uint)(
                                  ((spriteScanline[i].Id & 0x01) << 12)    // Which Pattern Table? 0KB or 4KB offset
                                | ((spriteScanline[i].Id & 0xFE) << 4)    // Which Cell? Tile ID * 16 (16 bytes per tile)
                                | (7 - (scanline - spriteScanline[i].Y) & 0x07)); // Which Row in cell? (0->7)
                            }
                        }
                    }

                    // Phew... XD I'm absolutely certain you can use some fantastic bit 
                    // manipulation to reduce all of that to a few one liners, but in this
                    // form it's easy to see the processes required for the different
                    // sizes and vertical orientations

                    // Hi bit plane equivalent is always offset by 8 bytes from lo bit plane
                    sprite_pattern_addr_hi = sprite_pattern_addr_lo + 8;

                    // Now we have the address of the sprite patterns, we can read them
                    sprite_pattern_bits_lo = PpuRead(sprite_pattern_addr_lo);
                    sprite_pattern_bits_hi = PpuRead(sprite_pattern_addr_hi);

                    // If the sprite is flipped horizontally, we need to flip the 
                    // pattern bytes. 
                    if ((spriteScanline[i].Attribute & 0x40) == 0x40)
                    {
                        // This little lambda function "flips" a byte
                        // so 0b11100000 becomes 0b00000111. It's very
                        // clever, and stolen completely from here:
                        // https://stackoverflow.com/a/2602885
                        uint flipbyte(uint b)
                        {
                            b = (b & 0xF0) >> 4 | (b & 0x0F) << 4;
                            b = (b & 0xCC) >> 2 | (b & 0x33) << 2;
                            b = (b & 0xAA) >> 1 | (b & 0x55) << 1;
                            return b;
                        };

                        // Flip Patterns Horizontally
                        sprite_pattern_bits_lo = flipbyte(sprite_pattern_bits_lo);
                        sprite_pattern_bits_hi = flipbyte(sprite_pattern_bits_hi);
                    }

                    // Finally! We can load the pattern into our sprite shift registers
                    // ready for rendering on the next scanline
                    sprite_shifter_pattern_lo[i] = sprite_pattern_bits_lo;
                    sprite_shifter_pattern_hi[i] = sprite_pattern_bits_hi;
                }
            }
        


            if (scanline == 240)
            {
                // Post Render Scanline - Do Nothing!
            }

            if (scanline >= 241 && scanline < 261)
            {
                if (scanline == 241 && cycle == 1)
                {
                    // Effectively end of frame, so set vertical blank flag
                    ppu_status.VerticalBlank = 1;

                    // If the control register tells us to emit a NMI when
                    // entering vertical blanking period, do it! The CPU
                    // will be informed that rendering is complete so it can
                    // perform operations with the PPU knowing it wont
                    // produce visible artefacts
                    if (ppu_control.EnableNmi == 1)
                        _state.NMI = true;
                }
            }



            // Composition - We now have background pixel information for this cycle
            // At this point we are only interested in background

            uint bg_pixel = 0x00;   // The 2-bit pixel to be rendered
            uint bg_palette = 0x00; // The 3-bit index of the palette the pixel indexes

            // We only render backgrounds if the PPU is enabled to do so. Note if 
            // background rendering is disabled, the pixel and palette combine
            // to form 0x00. This will fall through the colour tables to yield
            // the current background colour in effect
            if (ppu_mask.RenderBackground == 1)
            {
                // Handle Pixel Selection by selecting the relevant bit
                // depending upon fine x scolling. This has the effect of
                // offsetting ALL background rendering by a set number
                // of pixels, permitting smooth scrolling
                uint bit_mux = (uint)(0x8000 >> (int)fine_x);

                // Select Plane pixels by extracting from the shifter 
                // at the required location. 
                uint p0_pixel = (bg_shifter_pattern_lo & bit_mux) > 0 ? 1U : 0;
                uint p1_pixel = (bg_shifter_pattern_hi & bit_mux) > 0 ? 1U : 0;

                // Combine to form pixel index
                bg_pixel = (p1_pixel << 1) | p0_pixel;

                // Get palette
                uint bg_pal0 = (bg_shifter_attrib_lo & bit_mux) > 0 ? 1U : 0;
                uint bg_pal1 = (bg_shifter_attrib_hi & bit_mux) > 0 ? 1U : 0;
                bg_palette = (bg_pal1 << 1) | bg_pal0;
            }


            // Foreground =============================================================
            uint fg_pixel = 0x00;   // The 2-bit pixel to be rendered
            uint fg_palette = 0x00; // The 3-bit index of the palette the pixel indexes
            uint fg_priority = 0x00;// A bit of the sprite attribute indicates if its
                                       // more important than the background
            if (ppu_mask.RenderSprites == 1)
            {
                // Iterate through all sprites for this scanline. This is to maintain
                // sprite priority. As soon as we find a non transparent pixel of
                // a sprite we can abort

                bSpriteZeroBeingRendered = false;

                for (uint i = 0; i < sprite_count; i++)
                {
                    // Scanline cycle has "collided" with sprite, shifters taking over
                    if (spriteScanline[i].X == 0)
                    {
                        // Note Fine X scrolling does not apply to sprites, the game
                        // should maintain their relationship with the background. So
                        // we'll just use the MSB of the shifter

                        // Determine the pixel value...
                        uint fg_pixel_lo = (sprite_shifter_pattern_lo[i] & 0x80) > 0 ? 1U : 0;
                        uint fg_pixel_hi = (sprite_shifter_pattern_hi[i] & 0x80) > 0 ? 1U : 0;
                        fg_pixel = (fg_pixel_hi << 1) | fg_pixel_lo;

                        // Extract the palette from the bottom two bits. Recall
                        // that foreground palettes are the latter 4 in the 
                        // palette memory.
                        fg_palette = (spriteScanline[i].Attribute & 0x03) + 0x04;
                        fg_priority = (spriteScanline[i].Attribute & 0x20) == 0 ? 1U : 0;

                        // If pixel is not transparent, we render it, and dont
                        // bother checking the rest because the earlier sprites
                        // in the list are higher priority
                        if (fg_pixel != 0)
                        {
                            if (i == 0) // Is this sprite zero?
                            {
                                bSpriteZeroBeingRendered = true;
                            }

                            break;
                        }
                    }
                }
            }

            // Now we have a background pixel and a foreground pixel. They need
            // to be combined. It is possible for sprites to go behind background
            // tiles that are not "transparent", yet another neat trick of the PPU
            // that adds complexity for us poor emulator developers...

            uint pixel = 0x00;   // The FINAL Pixel...
            uint palette = 0x00; // The FINAL Palette...

            if (bg_pixel == 0 && fg_pixel == 0)
            {
                // The background pixel is transparent
                // The foreground pixel is transparent
                // No winner, draw "background" colour
                pixel = 0x00;
                palette = 0x00;
            }
            else if (bg_pixel == 0 && fg_pixel > 0)
            {
                // The background pixel is transparent
                // The foreground pixel is visible
                // Foreground wins!
                pixel = fg_pixel;
                palette = fg_palette;
            }
            else if (bg_pixel > 0 && fg_pixel == 0)
            {
                // The background pixel is visible
                // The foreground pixel is transparent
                // Background wins!
                pixel = bg_pixel;
                palette = bg_palette;
            }
            else if (bg_pixel > 0 && fg_pixel > 0)
            {
                // The background pixel is visible
                // The foreground pixel is visible
                // Hmmm...
                if (fg_priority == 1)
                {
                    // Foreground cheats its way to victory!
                    pixel = fg_pixel;
                    palette = fg_palette;
                }
                else
                {
                    // Background is considered more important!
                    pixel = bg_pixel;
                    palette = bg_palette;
                }

                // Sprite Zero Hit detection
                if (bSpriteZeroHitPossible && bSpriteZeroBeingRendered)
                {
                    // Sprite zero is a collision between foreground and background
                    // so they must both be enabled
                    if (ppu_mask.RenderBackground == 1 & ppu_mask.RenderSprites == 1)
                    {
                        // The left edge of the screen has specific switches to control
                        // its appearance. This is used to smooth inconsistencies when
                        // scrolling (since sprites x coord must be >= 0)
                        if (!(ppu_mask.RenderBackgroundLeft == 1 | ppu_mask.RenderSpritesleft == 1))
                        {
                            if (cycle >= 9 && cycle < 258)
                            {
                                ppu_status.SpriteZeroHit = 1;
                            }
                        }
                        else
                        {
                            if (cycle >= 1 && cycle < 258)
                            {
                                ppu_status.SpriteZeroHit = 1;
                            }
                        }
                    }
                }
            }




            if (cycle > 1 && cycle < 256 && scanline > 0 && scanline < 240)
            {
                Renderer.buffer[cycle - 1 + scanline * 256] =
                    palScreen[PpuRead(0x3F00 + (palette << 2) + pixel) & 0x3F];
            }

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
            cycles++;
        }

    }
}