using System;

namespace Fami.Core.Mappers
{
    public class Mapper002 : BaseMapper
    {
        private uint _bankOffset;
        public Mapper002(Cartridge cartridge) : base(cartridge)
        {
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            return address switch
            {
                >= 0x6000 and <= 0x7FFF => (_cartridge.RamBankData[address - 0x6000], true),
                >= 0x8000 and <= 0xBFFF => (_cartridge.RomBankData[_bankOffset + (address - 0x8000)], true),
                >= 0xC000 and <= 0xFFFF => (
                    _cartridge.RomBankData[_cartridge.RomBankData.Length - 0x4000 + (address - 0xC000)], true),
                _ => (address, false)
            };
        }

        public override void WriteState(ref byte[] buffer)
        {
            buffer[0] = (byte)(_bankOffset & 0xff);
            buffer[1] = (byte)((_bankOffset >> 8) & 0xff);
            buffer[2] = (byte)((_bankOffset >> 16) & 0xff);
            buffer[3] = (byte)((_bankOffset >> 24) & 0xff);
            Array.Copy(_cartridge.RamBankData, 0, buffer, 4, 8192);
        }

        public override void ReadState(byte[] buffer)
        {
            _bankOffset = (uint)(buffer[3] << 24) | (uint)(buffer[2] << 16) | (uint)(buffer[1] << 8) | buffer[0];
            Array.Copy(buffer, 4, _cartridge.RamBankData, 0, 8192);
        }

        public override bool CpuMapWrite(uint address, uint value)
        {
            switch (address)
            {
                case >= 0x6000 and <= 0x7FFF:
                    _cartridge.RamBankData[address - 0x6000] = (byte)value;
                    return true;
                case >= 0x8000 and <= 0xFFFF:
                    _bankOffset = (value & 0xF) * 0x4000;
                    return true;
                default:
                    return false;
            }
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            return (address, false);
        }

        public override bool PpuMapWrite(uint address, uint value)
        {
            return false;
        }
    }
}