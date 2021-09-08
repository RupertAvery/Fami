using System.IO;

namespace Fami.Core.Mappers
{
    public class AxROM : BaseMapper
    {
        private uint _bankOffset;
        private readonly MirrorEnum[] _mirroringModes = { MirrorEnum.Lower, MirrorEnum.Upper };

        public AxROM(Cartridge cartridge) : base(cartridge)
        {
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                var mappedAddress = _bankOffset + (address - 0x8000);
                return (_cartridge.RomBankData[mappedAddress], true);
            }
            return (0, false);
        }

        public override bool CpuMapWrite(uint address, uint value)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                _bankOffset = (value & 0x7) * 0x8000;
                _cartridge.Mirror = _mirroringModes[(value >> 4) & 0x1];
                return true;
            }
            return false;
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            return (0, false);
        }

        public override bool PpuMapWrite(uint address, uint value)
        {
            return false;
        }

        public override void WriteState(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(_bankOffset);
        }
        public override void ReadState(Stream stream)
        {
            var reader = new BinaryReader(stream);
            _bankOffset = reader.ReadUInt32();
        }

        public override void Reset()
        {

        }

    }
}