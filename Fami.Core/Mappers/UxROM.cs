using System;
using System.IO;

namespace Fami.Core.Mappers
{
    public class UxROM : BaseMapper
    {
        private uint _bankOffset;
        public UxROM(Cartridge cartridge) : base(cartridge)
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

        public override void WriteState(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(_bankOffset);
            w.Write(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);
        }

        public override void ReadState(Stream stream)
        {
            var w = new BinaryReader(stream);
            _bankOffset = w.ReadUInt32();
            w.Read(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);
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

        public override void Reset()
        {
            _bankOffset = 0;
            //for (var i = 0; i < _cartridge.RamBankData.Length; i++)
            //{
            //    _cartridge.RamBankData[i] = 0x00;
            //}
        }

    }
}