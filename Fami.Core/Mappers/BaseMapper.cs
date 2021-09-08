using System.IO;

namespace Fami.Core.Mappers
{
    public abstract class BaseMapper
    {
        protected readonly Cartridge _cartridge;
        protected readonly uint _lastBankOffset;

        protected BaseMapper(Cartridge cartridge)
        {
            _cartridge = cartridge;
            _lastBankOffset = (uint)_cartridge.RomBankData.Length - 0x4000;
        }

        public abstract (uint value, bool handled) CpuMapRead(uint address);
        public abstract bool CpuMapWrite(uint address, uint value);
        public abstract (uint value, bool handled) PpuMapRead(uint address);
        public abstract bool PpuMapWrite(uint address, uint value);

        public abstract void Reset();
        public abstract void WriteState(Stream stream);
        public abstract void ReadState(Stream stream);
    }
}