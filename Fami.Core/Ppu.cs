using System;

namespace Fami.Core
{
    public class Ppu
    {
        private readonly Cpu6502State _state;
        private StatusRegister ppu_status = new();
        private ControlRegister ppu_control = new();
        private MaskRegister ppu_mask = new();
        private uint ppu_addr;
        private uint ppu_data;

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
        private uint[,] tblPattern = new uint[2, 4096];
        private uint[] tblPalette = new uint[32];
        private Cartridge _cart;

        private uint[] palScreen = new uint[]
        {
            0xFF000000 + (84 << 16) + (84 << 8) + 84,
            0xFF000000 + (0 << 16) + (30 << 8) + 116,
            0xFF000000 + (8 << 16) + (16 << 8) + 144,
            0xFF000000 + (48 << 16) + (0 << 8) + 136,
            0xFF000000 + (68 << 16) + (0 << 8) + 100,
            0xFF000000 + (92 << 16) + (0 << 8) + 48,
            0xFF000000 + (84 << 16) + (4 << 8) + 0,
            0xFF000000 + (60 << 16) + (24 << 8) + 0,
            0xFF000000 + (32 << 16) + (42 << 8) + 0,
            0xFF000000 + (8 << 16) + (58 << 8) + 0,
            0xFF000000 + (0 << 16) + (64 << 8) + 0,
            0xFF000000 + (0 << 16) + (60 << 8) + 0,
            0xFF000000 + (0 << 16) + (50 << 8) + 60,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,

            0xFF000000 + (152 << 16) + (150 << 8) + 152,
            0xFF000000 + (8 << 16) + (76 << 8) + 196,
            0xFF000000 + (48 << 16) + (50 << 8) + 236,
            0xFF000000 + (92 << 16) + (30 << 8) + 228,
            0xFF000000 + (136 << 16) + (20 << 8) + 176,
            0xFF000000 + (160 << 16) + (20 << 8) + 100,
            0xFF000000 + (152 << 16) + (34 << 8) + 32,
            0xFF000000 + (120 << 16) + (60 << 8) + 0,
            0xFF000000 + (84 << 16) + (90 << 8) + 0,
            0xFF000000 + (40 << 16) + (114 << 8) + 0,
            0xFF000000 + (8 << 16) + (124 << 8) + 0,
            0xFF000000 + (0 << 16) + (118 << 8) + 40,
            0xFF000000 + (0 << 16) + (102 << 8) + 120,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,

            0xFF000000 + (236 << 16) + (238 << 8) + 236,
            0xFF000000 + (76 << 16) + (154 << 8) + 236,
            0xFF000000 + (120 << 16) + (124 << 8) + 236,
            0xFF000000 + (176 << 16) + (98 << 8) + 236,
            0xFF000000 + (228 << 16) + (84 << 8) + 236,
            0xFF000000 + (236 << 16) + (88 << 8) + 180,
            0xFF000000 + (236 << 16) + (106 << 8) + 100,
            0xFF000000 + (212 << 16) + (136 << 8) + 32,
            0xFF000000 + (160 << 16) + (170 << 8) + 0,
            0xFF000000 + (116 << 16) + (196 << 8) + 0,
            0xFF000000 + (76 << 16) + (208 << 8) + 32,
            0xFF000000 + (56 << 16) + (204 << 8) + 108,
            0xFF000000 + (56 << 16) + (180 << 8) + 204,
            0xFF000000 + (60 << 16) + (60 << 8) + 60,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,

            0xFF000000 + (236 << 16) + (238 << 8) + 236,
            0xFF000000 + (168 << 16) + (204 << 8) + 236,
            0xFF000000 + (188 << 16) + (188 << 8) + 236,
            0xFF000000 + (212 << 16) + (178 << 8) + 236,
            0xFF000000 + (236 << 16) + (174 << 8) + 236,
            0xFF000000 + (236 << 16) + (174 << 8) + 212,
            0xFF000000 + (236 << 16) + (180 << 8) + 176,
            0xFF000000 + (228 << 16) + (196 << 8) + 144,
            0xFF000000 + (204 << 16) + (210 << 8) + 120,
            0xFF000000 + (180 << 16) + (222 << 8) + 120,
            0xFF000000 + (168 << 16) + (226 << 8) + 144,
            0xFF000000 + (152 << 16) + (226 << 8) + 180,
            0xFF000000 + (160 << 16) + (214 << 8) + 228,
            0xFF000000 + (160 << 16) + (162 << 8) + 160,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,
            0xFF000000 + (0 << 16) + (0 << 8) + 0,

            //0xFF000000 + (84 << 16) + (84 << 8) + 84, 0xFF000000 + (0 << 16) + (30 << 8) + 116,
            //0xFF000000 + (8 << 16) + (16 << 8) + 144, 0xFF000000 + (48 << 16) + (0 << 8) + 136,
            //0xFF000000 + (68 << 16) + (0 << 8) + 100, 0xFF000000 + (92 << 16) + (0 << 8) + 48,
            //0xFF000000 + (84 << 16) + (4 << 8) + 0, 0xFF000000 + (60 << 16) + (24 << 8) + 0,
            //0xFF000000 + (32 << 16) + (42 << 8) + 0, 0xFF000000 + (8 << 16) + (58 << 8) + 0,
            //0xFF000000 + (0 << 16) + (64 << 8) + 0, 0xFF000000 + (0 << 16) + (60 << 8) + 0,
            //0xFF000000 + (0 << 16) + (50 << 8) + 60, 0xFF000000 + (0 << 16) + (0 << 8) + 0,
            //0xFF000000 + (152 << 16) + (150 << 8) + 152, 0xFF000000 + (8 << 16) + (76 << 8) + 196,
            //0xFF000000 + (48 << 16) + (50 << 8) + 236, 0xFF000000 + (92 << 16) + (30 << 8) + 228,
            //0xFF000000 + (136 << 16) + (20 << 8) + 176, 0xFF000000 + (160 << 16) + (20 << 8) + 100,
            //0xFF000000 + (152 << 16) + (34 << 8) + 32, 0xFF000000 + (120 << 16) + (60 << 8) + 0,
            //0xFF000000 + (84 << 16) + (90 << 8) + 0, 0xFF000000 + (40 << 16) + (114 << 8) + 0,
            //0xFF000000 + (8 << 16) + (124 << 8) + 0, 0xFF000000 + (0 << 16) + (118 << 8) + 40,
            //0xFF000000 + (0 << 16) + (102 << 8) + 120, 0xFF000000 + (0 << 16) + (0 << 8) + 0,
            //0xFF000000 + (236 << 16) + (238 << 8) + 236, 0xFF000000 + (76 << 16) + (154 << 8) + 236,
            //0xFF000000 + (120 << 16) + (124 << 8) + 236, 0xFF000000 + (176 << 16) + (98 << 8) + 236,
            //0xFF000000 + (228 << 16) + (84 << 8) + 236, 0xFF000000 + (236 << 16) + (88 << 8) + 180,
            //0xFF000000 + (236 << 16) + (106 << 8) + 100, 0xFF000000 + (212 << 16) + (136 << 8) + 32,
            //0xFF000000 + (160 << 16) + (170 << 8) + 0, 0xFF000000 + (116 << 16) + (196 << 8) + 0,
            //0xFF000000 + (76 << 16) + (208 << 8) + 32, 0xFF000000 + (56 << 16) + (204 << 8) + 108,
            //0xFF000000 + (56 << 16) + (180 << 8) + 204, 0xFF000000 + (60 << 16) + (60 << 8) + 60,
            //0xFF000000 + (236 << 16) + (238 << 8) + 236, 0xFF000000 + (168 << 16) + (204 << 8) + 236,
            //0xFF000000 + (188 << 16) + (188 << 8) + 236, 0xFF000000 + (212 << 16) + (178 << 8) + 236,
            //0xFF000000 + (236 << 16) + (174 << 8) + 236, 0xFF000000 + (236 << 16) + (174 << 8) + 212,
            //0xFF000000 + (236 << 16) + (180 << 8) + 176, 0xFF000000 + (228 << 16) + (196 << 8) + 144,
            //0xFF000000 + (204 << 16) + (210 << 8) + 120, 0xFF000000 + (180 << 16) + (222 << 8) + 120,
            //0xFF000000 + (168 << 16) + (226 << 8) + 144, 0xFF000000 + (152 << 16) + (226 << 8) + 180,
            //0xFF000000 + (160 << 16) + (214 << 8) + 228, 0xFF000000 + (160 << 16) + (162 << 8) + 160,
        };

        public Ppu(Cpu6502State state)
        {
            _state = state;
            Renderer = new PpuRenderer();
        }

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
                    break;
                case OAMDATA:
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

                    if (ppu_control.IncrementMode == 1)
                    {
                        vram_addr.CoarseY++;
                    }
                    else
                    {
                        vram_addr.CoarseX++;
                    }
                    //vram_addr.Register += ((ppu_control.IncrementMode == 1) ? 32U : 1U);

                    break;
            }
            return data;
        }

        private Random r = new();

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
                            if ((vram_addr.CoarseY & 0x02) > 0) bg_next_tile_attrib >>= 4;
                            if ((vram_addr.CoarseX & 0x02) > 0) bg_next_tile_attrib >>= 2;
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

            if (cycle > 1 && cycle < 256 && scanline > 0 && scanline < 240)
            {
                Renderer.buffer[cycle - 1 + scanline * 256] =
                    palScreen[PpuRead(0x3F00 + (bg_palette << 2) + bg_pixel) & 0x3F];
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