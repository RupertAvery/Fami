namespace Fami.Core.Mappers
{
    public class MMC3 : BaseMapper
    {
        private uint[] _bankRegister = new uint[8];
        protected readonly uint[] _chrBankOffsets = new uint[8];
        protected uint[] _prgBankOffsets;

        public MMC3(Cartridge cartridge) : base(cartridge)
        {
            cartridge.Cpu.Ppu.ScanLineHandler = ScanLineHandler;
            _prgBankOffsets = new uint[] { 0, 0x2000, _lastBankOffset, _lastBankOffset + 0x2000 };
        }

        public void ScanLineHandler()
        {
            if (irq_reload || irq_counter == 0)
            {
                irq_counter = irq_latch;
                irq_reload = false;
            }
            else
            {
                irq_counter--;
            }
            if (enable_interrupts && irq_counter == 0)
            {
                _cartridge.Cpu.TriggerInterrupt(InterruptTypeEnum.IRQ);
            }
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x6000 && address <= 0x7FFF)
            {
                return (_cartridge.RamBankData[address - 0x6000], true);
            }
            else if (address >= 0x8000 && address <= 0xFFFF)
            {
                var mappedAddress = _prgBankOffsets[(address - 0x8000) / 0x2000] + address % 0x2000;
                return (_cartridge.RomBankData[mappedAddress], true);
            }
            return (0, false);
        }

        private uint R;
        private uint C;
        private uint P;
        private uint M;

        public override bool CpuMapWrite(uint address, uint value)
        {
            bool even = (address & 0x1) == 0;

            if (address >= 0x6000 && address < 0x8000)
            {
                if (enable_sram)
                {
                    _cartridge.RamBankData[address - 0x6000] = (byte)value;
                    return true;
                }
                return false;
            }
            else if (address >= 0x8000 && address <= 0x9FFF)
            {
                if (even)
                {
                    // Bank Select
                    R = value & 7;
                    P = (value >> 6) & 1;
                    C = (value >> 7) & 1;
                }
                else
                {
                    // Bank data
                    _bankRegister[R] = value;

                }
                UpdateOffsets();
                return true;
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (even)
                    _cartridge.Mirror = (value & 1) == 1 ? MirrorEnum.Horizontal : MirrorEnum.Vertical;
                else
                {
                    enable_sram = (value & 0x80) == 0x80;
                    enable_write_protect = (value & 0x40) == 0x40;
                }
                return true;
            }
            else if (address >= 0xC000 && address <= 0xDFFF)
            {
                if (even)
                    irq_latch = value;
                else
                {
                    irq_reload = true;
                    irq_counter = 0;
                }
                return true;
            }
            else if (address >= 0xE000 && address <= 0xFFFF)
            {
                enable_interrupts = !even;
                return true;
            }

            return false;
        }

        private uint irq_counter;
        private bool irq_reload;
        private uint irq_latch;
        private bool enable_interrupts;
        private bool enable_sram;
        private bool enable_write_protect;

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return (_cartridge.VRomBankData[_chrBankOffsets[address / 0x400] + address % 0x400], true);
            }
            return (0, false);
        }

        public override bool PpuMapWrite(uint address, uint value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                _cartridge.VRomBankData[_chrBankOffsets[address / 0x400] + address % 0x400] = (byte)value;
            }
            return false;
        }

        public override void WriteState(ref byte[] buffer)
        {

        }
        public override void ReadState(byte[] buffer)
        {

        }


        protected void UpdateOffsets()
        {
            switch (P)
            {
                case 0:
                    _prgBankOffsets[0] = _bankRegister[6] * 0x2000;
                    _prgBankOffsets[1] = _bankRegister[7] * 0x2000;
                    _prgBankOffsets[2] = _lastBankOffset;
                    _prgBankOffsets[3] = _lastBankOffset + 0x2000;
                    break;
                case 1:
                    _prgBankOffsets[0] = _lastBankOffset;
                    _prgBankOffsets[1] = _bankRegister[7] * 0x2000;
                    _prgBankOffsets[2] = _bankRegister[6] * 0x2000;
                    _prgBankOffsets[3] = _lastBankOffset + 0x2000;
                    break;
            }

            switch (C)
            {
                case 0:
                    _chrBankOffsets[0] = _bankRegister[0] & 0xFE;
                    _chrBankOffsets[1] = _bankRegister[0] | 0x01;
                    _chrBankOffsets[2] = _bankRegister[1] & 0xFE;
                    _chrBankOffsets[3] = _bankRegister[1] | 0x01;
                    _chrBankOffsets[4] = _bankRegister[2];
                    _chrBankOffsets[5] = _bankRegister[3];
                    _chrBankOffsets[6] = _bankRegister[4];
                    _chrBankOffsets[7] = _bankRegister[5];
                    break;
                case 1:
                    _chrBankOffsets[0] = _bankRegister[2];
                    _chrBankOffsets[1] = _bankRegister[3];
                    _chrBankOffsets[2] = _bankRegister[4];
                    _chrBankOffsets[3] = _bankRegister[5];
                    _chrBankOffsets[4] = _bankRegister[0] & 0xFE;
                    _chrBankOffsets[5] = _bankRegister[0] | 0x01;
                    _chrBankOffsets[6] = _bankRegister[1] & 0xFE;
                    _chrBankOffsets[7] = _bankRegister[1] | 0x01;
                    break;
            }

            for (int i = 0; i < _prgBankOffsets.Length; i++)
                _prgBankOffsets[i] %= (uint)_cartridge.RomBankData.Length;

            for (int i = 0; i < _chrBankOffsets.Length; i++)
                _chrBankOffsets[i] = (uint)(_chrBankOffsets[i] * 0x400 % _cartridge.VRomBankData.Length);
        }

    }
}