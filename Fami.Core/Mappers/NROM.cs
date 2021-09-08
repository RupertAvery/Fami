using System.IO;

namespace Fami.Core.Mappers
{
    public class NROM : BaseMapper
    {
        private readonly int _prgBanks;

        public NROM(Cartridge cartridge) : base(cartridge)
        {
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

        public override void WriteState(Stream stream)
        {

        }
        public override void ReadState(Stream stream)
        {

        }

        public override void Reset()
        {

        }

    }
}