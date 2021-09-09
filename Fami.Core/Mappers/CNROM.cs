using System.IO;

namespace Fami.Core.Mappers
{
    public class CNROM : BaseMapper
    {
        private uint _bankOffset;
        private readonly MirrorEnum[] _mirroringModes = { MirrorEnum.Lower, MirrorEnum.Upper };

        public CNROM(Cartridge cartridge) : base(cartridge)
        {
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                return (_cartridge.RomBankData[address - 0x8000], true);
            }
            return (0, false);
        }

        public override bool CpuMapWrite(uint address, uint value)
        {
            if (address >= 0x8000 && address <= 0xFFFF)
            {
                _bankOffset = (value & 0x3) * 0x2000;
                return true;
            }
            return false;
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                var mappedAddress = _bankOffset + address;
                return (_cartridge.VRomBankData[mappedAddress], true);
            }
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