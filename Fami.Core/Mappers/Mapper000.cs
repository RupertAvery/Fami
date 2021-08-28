namespace Fami.Core.Mappers
{
    public class Mapper000 : BaseMapper
    {
        private readonly int _prgBanks;

        public Mapper000(int prgBanks)
        {
            _prgBanks = prgBanks;
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                return (address & (uint)(_prgBanks > 1 ? 0x7FFF : 0x3FFF), true);
            }
            return (address, false);
        }

        public override (uint value, bool handled) CpuMapWrite(uint address)
        {
            return (address, false);
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return (address, true);
            }
            return (address, false);
        }

        public override (uint value, bool handled) PpuMapWrite(uint address)
        {
            return (address, false);
        }
    }
}