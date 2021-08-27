namespace Fami.Core.Mappers
{
    public class Mapper000 : BaseMapper
    {
        private readonly int _prgBanks;

        public Mapper000(int prgBanks)
        {
            _prgBanks = prgBanks;
        }

        public override (int value, bool handled) CpuMapRead(int address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                return (address & (_prgBanks > 1 ? 0x7FFF : 0x3FFF), true);
            }
            return (address, false);
        }

        public override (int value, bool handled) CpuMapWrite(int address)
        {
            return (address, false);
        }

        public override (int value, bool handled) PpuMapRead(int address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return (address, true);
            }
            return (address, false);
        }

        public override (int value, bool handled) PpuMapWrite(int address)
        {
            return (address, false);
        }
    }
}