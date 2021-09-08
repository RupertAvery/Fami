using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using Fami.Core.Utility;

namespace Fami.Core.Mappers
{
    public class MMC1 : BaseMapper
    {
        public enum ChipType { MMC1, MMC1A, MMC1B, MMC1C }
        public enum CHRBankingMode { Single, Double }
        public enum PRGBankingMode { Switch32Kb, Switch16KbFixFirst, Switch16KbFixLast }


        private uint _offset;
        private uint shift_reg = 0;
        private uint shift = 0;
        private uint _lastWritePC;
        private uint _serialData;
        private int _serialPos;
        private uint _control;

        private uint[] _chrBankOffsets = new uint[2];
        private uint[] _chrBanks = new uint[2];
        private uint[] _prgBankOffsets = new uint[2];
        private uint _prgBank;

        private bool _prgRAMEnabled;

        private readonly ChipType _type;
        private CHRBankingMode _chrBankingMode;
        private PRGBankingMode _prgBankingMode;


        public MMC1(Cartridge cartridge) : base(cartridge)
        {
            UpdateControl(0x0F);
            _cartridge.Mirror = MirrorEnum.Horizontal;
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            return address switch
            {
                >= 0x6000 and <= 0x7FFF => (_cartridge.RamBankData[address - 0x6000], true),
                >= 0x8000 and <= 0xFFFF => (_cartridge.RomBankData[_prgBankOffsets[(address - 0x8000) / 0x4000] + address % 0x4000], true),
                _ => (address, false)
            };
        }


        public override bool CpuMapWrite(uint address, uint value)
        {
            switch (address)
            {
                case >= 0x6000 and <= 0x7FFF:
                    _cartridge.RamBankData[address - 0x6000] = (byte)value;
                    return true;
                case >= 0x8000 and <= 0xFFFF:
                    var cycle = _cartridge.Cpu.PC;
                    if (cycle == _lastWritePC)
                        return false;
                    _lastWritePC = cycle;

                    if ((value & 0x80) > 0)
                    {
                        _serialData = 0;
                        _serialPos = 0;
                        UpdateControl(_control | 0x0C);
                    }
                    else
                    {
                        _serialData |= (uint)((value & 0x1) << _serialPos);
                        _serialPos++;

                        if (_serialPos == 5)
                        {
                            // Address is incompletely decoded
                            address &= 0x6000;
                            if (address == 0x0000)
                                UpdateControl(_serialData);
                            else if (address == 0x2000)
                                UpdateCHRBank(0, _serialData);
                            else if (address == 0x4000)
                                UpdateCHRBank(1, _serialData);
                            else if (address == 0x6000)
                                UpdatePRGBank(_serialData);

                            _serialData = 0;
                            _serialPos = 0;
                        }
                    }
                    return true;
                default:
                    return false;
            }
        }

        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            return address switch
            {
                >= 0x0000 and <= 0x1FFF => (_cartridge.VRomBankData[_chrBankOffsets[address / 0x1000] + address % 0x1000], true),
                _ => (0, false)
            };
        }


        public override bool PpuMapWrite(uint address, uint value)
        {
            bool map()
            {
                _cartridge.VRomBankData[_chrBankOffsets[address / 0x1000] + address % 0x1000] = (byte)value;
                return true;
            }

            return address switch
            {
                >= 0x0000 and <= 0x1FFF => map(),
                _ => false
            };
        }

        private readonly MirrorEnum[] _mirroringModes = { MirrorEnum.Lower, MirrorEnum.Upper, MirrorEnum.Vertical, MirrorEnum.Horizontal };

        private void UpdateControl(uint value)
        {
            _control = value;

            _cartridge.Mirror = _mirroringModes[value & 0x3];

            _chrBankingMode = (CHRBankingMode)((value >> 4) & 0x1);

            var prgMode = (value >> 2) & 0x3;
            // Both 0 and 1 are 32Kb switch
            if (prgMode == 0) prgMode = 1;
            _prgBankingMode = (PRGBankingMode)(prgMode - 1);

            UpdateCHRBank(1, _chrBanks[1]);
            UpdateCHRBank(0, _chrBanks[0]);
            UpdatePRGBank(_prgBank);
        }

        private void UpdatePRGBank(uint value)
        {
            _prgBank = value;

            _prgRAMEnabled = (value & 0x10) == 0;
            value &= 0xF;

            switch (_prgBankingMode)
            {
                case PRGBankingMode.Switch32Kb:
                    value >>= 1;
                    value *= 0x4000;
                    _prgBankOffsets[0] = value;
                    _prgBankOffsets[1] = value + 0x4000;
                    break;
                case PRGBankingMode.Switch16KbFixFirst:
                    _prgBankOffsets[0] = 0;
                    _prgBankOffsets[1] = value * 0x4000;
                    break;
                case PRGBankingMode.Switch16KbFixLast:
                    _prgBankOffsets[0] = value * 0x4000;
                    _prgBankOffsets[1] = _lastBankOffset;
                    break;
            }
        }

        private void UpdateCHRBank(uint bank, uint value)
        {
            _chrBanks[bank] = value;

            // TODO FIXME: I feel like this branch should only be taken
            // when bank == 0, but this breaks Final Fantasy
            // When can banking mode change without UpdateCHRBank being called?
            if (_chrBankingMode == CHRBankingMode.Single)
            {
                value = _chrBanks[0];
                value >>= 1;
                value *= 0x1000;
                _chrBankOffsets[0] = value;
                _chrBankOffsets[1] = value + 0x1000;
            }
            else
            {
                _chrBankOffsets[bank] = value * 0x1000;
            }
        }

        public override void WriteState(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(_offset);
            w.Write(shift_reg);
            w.Write(shift);
            w.Write(_lastWritePC);
            w.Write(_serialData);
            w.Write(_serialPos);
            w.Write(_control);

            w.Write(_chrBankOffsets, 0, _chrBankOffsets.Length);
            w.Write(_chrBanks, 0, _chrBanks.Length);
            w.Write(_prgBankOffsets, 0, _chrBankOffsets.Length);
            w.Write(_prgBank);

            w.Write(_prgRAMEnabled);

            w.Write((byte)_chrBankingMode);
            w.Write((byte)_prgBankingMode);

            w.Write(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);
        }
        public override void ReadState(Stream stream)
        {
            var w = new BinaryReader(stream);
            _offset = w.ReadUInt32();
            shift_reg = w.ReadUInt32();
            shift = w.ReadUInt32();
            _lastWritePC = w.ReadUInt32();
            _serialData = w.ReadUInt32();
            _serialPos = w.ReadInt32();
            _control = w.ReadUInt32();

            _chrBankOffsets = w.ReadUInt32Array(_chrBankOffsets.Length);
            _chrBanks = w.ReadUInt32Array(_chrBanks.Length);
            _prgBankOffsets = w.ReadUInt32Array(_chrBankOffsets.Length);
            _prgBank = w.ReadUInt32();

            _prgRAMEnabled = w.ReadBoolean();

            _chrBankingMode = (CHRBankingMode)w.ReadByte();
            _prgBankingMode = (PRGBankingMode)w.ReadByte();
            w.Read(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);
        }

        public override void Reset()
        {

        }

    }
}