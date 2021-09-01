namespace Fami.Core
{
    public struct PpuState
    {
        public static PpuState New()
        {
            return new PpuState()
            {
                //buffer = new uint[256 * 240],
                ppu_status = default,
                ppu_control = default,
                ppu_mask = default,
                vram_addr = default,
                tram_addr = default,
                tblName = new byte[2, 1024],
                tblPalette = new byte[32],
                tblPattern = new byte[2, 4096],
                fine_x = 0,
                cycles = 0,
                cycle = 0,
                scanline = 0,
                pOAM = new byte[256],
                bg_shifter_pattern_lo = 0,
                bg_next_tile_lsb = 0,
                bg_shifter_pattern_hi = 0,
                bg_next_tile_msb = 0,
                bg_shifter_attrib_lo = 0,
                bg_shifter_attrib_hi = 0,
                bg_next_tile_attrib = 0,
                bg_next_tile_id = 0,
                address_latch = 0,
                ppu_data_buffer = 0,
                oam_addr = 0,
                sprite_count = 0,
                bSpriteZeroBeingRendered = false,
                bSpriteZeroHitPossible = false,
                spriteScanline = new SpriteScanline[64],
                sprite_shifter_pattern_lo = new uint[64],
                sprite_shifter_pattern_hi = new uint[64],
            };
        }

        //public uint[] buffer;
        public StatusRegister ppu_status;
        public ControlRegister ppu_control;
        public MaskRegister ppu_mask;
        public LoopyRegister vram_addr;
        public LoopyRegister tram_addr;

        public byte[,] tblName;
        public byte[] tblPalette;
        public byte[,] tblPattern;

        public uint fine_x;
        public ulong cycles;
        public int cycle;
        public int scanline;
        public byte[] pOAM;
        public uint bg_shifter_pattern_lo;
        public uint bg_next_tile_lsb;
        public uint bg_shifter_pattern_hi;
        public uint bg_next_tile_msb;
        public uint bg_shifter_attrib_lo;
        public uint bg_shifter_attrib_hi;
        public uint bg_next_tile_attrib;
        public uint bg_next_tile_id;
        public uint address_latch;
        public uint ppu_data_buffer;
        public uint oam_addr;
        public uint sprite_count;
        public bool bSpriteZeroBeingRendered;
        public bool bSpriteZeroHitPossible;
        public SpriteScanline[] spriteScanline;
        public uint[] sprite_shifter_pattern_lo;
        public uint[] sprite_shifter_pattern_hi;
    }
}