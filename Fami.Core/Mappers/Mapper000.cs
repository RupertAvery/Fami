﻿namespace Fami.Core.Mappers
{
    public class Mapper000 : BaseMapper
    {
        private readonly Cartridge _cartridge;
        private readonly int _prgBanks;

        public Mapper000(Cartridge cartridge)
        {
            _cartridge = cartridge;
            _prgBanks = cartridge.RomBanks;
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                var mappedAddress = address & (uint)(_prgBanks > 1 ? 0x7FFF : 0x3FFF);
                return (_cartridge.RomBankData[mappedAddress], true);
            }
            return (0, false);
        }

        public override bool CpuMapWrite(uint address, uint value)
        {
            return false;
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return (_cartridge.VRomBankData[address], true);
            }
            return (0, false);
        }

        public override bool PpuMapWrite(uint address, uint value)
        {
            return false;
        }

    }
}