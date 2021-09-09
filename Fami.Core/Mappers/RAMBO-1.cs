using System.IO;
using Fami.Core.CPU;

namespace Fami.Core.Mappers
{
    public class RAMBO1 : BaseMapper
    {
        private byte _bankRegisterSelect;
        private byte _kSelect;
        private byte _chrSelect;
        private byte _prgSelect;

        private byte irq_counter;
        private bool irq_reload;
        private byte irq_latch;
        private bool enable_interrupts;
        private bool enable_sram;
        private bool enable_write_protect;

        private byte[] _bankRegister = new byte[16];
        protected uint[] _chrBankOffsets = new uint[8];
        protected uint[] _prgBankOffsets;

        public RAMBO1(Cartridge cartridge) : base(cartridge)
        {
            cartridge.Cpu.Ppu.ScanLineHandler = ScanLineHandler;
            _prgBankOffsets = new uint[] { 0, 0, 0, _lastBankOffset + 0x2000 };
        }

        public void ScanLineHandler()
        {
            if (irq_reload || irq_counter == 0)
            {
                irq_counter = irq_latch;
                irq_reload = false;
            }
            else
            {
                irq_counter--;
            }
            if (enable_interrupts && irq_counter == 0)
            {
                // TODO: implement scanline mode vs CPU cycle mode
                // Delay triggering by 4 CPU cycles?
                _cartridge.Cpu.TriggerInterrupt(InterruptTypeEnum.IRQ);
            }
        }

        public override (uint value, bool handled) CpuMapRead(uint address)
        {
            if (address >= 0x6000 && address <= 0x7FFF)
            {
                return (_cartridge.RamBankData[address - 0x6000], true);
            }
            else if (address >= 0x8000 && address <= 0xFFFF)
            {
                var prgBank = (address - 0x8000) / 0x2000;
                var baseAddress = _prgBankOffsets[prgBank];
                var mappedAddress = baseAddress + address % 0x2000;
                return (_cartridge.RomBankData[mappedAddress], true);
            }
            return (0, false);
        }


        public override bool CpuMapWrite(uint address, uint value)
        {
            bool even = (address & 0x1) == 0;

            if (address >= 0x6000 && address < 0x8000)
            {
                //if (enable_sram)
                //{
                //    _cartridge.RamBankData[address - 0x6000] = (byte)value;
                //    return true;
                //}
                return false;
            }
            else if (address >= 0x8000 && address <= 0x9FFF)
            {
                if (even)
                {
                    // Bank Select
                    _bankRegisterSelect = (byte)(value & 0xF);
                    _kSelect = (byte)((value >> 5) & 1);
                    _prgSelect = (byte)((value >> 6) & 1);
                    _chrSelect = (byte)((value >> 7) & 1);
                }
                else
                {
                    // Bank data
                    _bankRegister[_bankRegisterSelect] = (byte)value;
                }
                UpdateOffsets();
                return true;
            }
            else if (address >= 0xA000 && address <= 0xBFFF)
            {
                if (even)
                    _cartridge.Mirror = (value & 1) == 1 ? MirrorEnum.Horizontal : MirrorEnum.Vertical;
                else
                {
                    //enable_sram = (value & 0x80) == 0x80;
                    //enable_write_protect = (value & 0x40) == 0x40;
                }
                return true;
            }
            else if (address >= 0xC000 && address <= 0xDFFF)
            {
                if (even)
                    irq_latch = (byte)value;
                else
                {
                    irq_reload = true;
                    irq_counter = 0;
                }
                return true;
            }
            else if (address >= 0xE000 && address <= 0xFFFF)
            {
                enable_interrupts = !even;
                return true;
            }

            return false;
        }


        public override (uint value, bool handled) PpuMapRead(uint address)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                return (_cartridge.VRomBankData[_chrBankOffsets[address / 0x400] + address % 0x400], true);
            }
            return (0, false);
        }

        public override bool PpuMapWrite(uint address, uint value)
        {
            if (address >= 0x0000 && address <= 0x1FFF)
            {
                _cartridge.VRomBankData[_chrBankOffsets[address / 0x400] + address % 0x400] = (byte)value;
            }
            return false;
        }

        public override void WriteState(Stream stream)
        {
            var writer = new BinaryWriter(stream);

            writer.Write(_bankRegisterSelect);
            writer.Write(_chrSelect);
            writer.Write(_prgSelect);
            writer.Write(irq_counter);
            writer.Write(irq_latch);
            writer.Write(irq_reload);
            writer.Write(enable_interrupts);
            writer.Write(enable_sram);
            writer.Write(enable_write_protect);

            writer.Write((byte)_cartridge.Mirror);

            writer.Write(_bankRegister, 0, _bankRegister.Length);
            //writer.Write(_chrBankOffsets, 0, _chrBankOffsets.Length);
            //writer.Write(_prgBankOffsets, 0, _prgBankOffsets.Length);
            writer.Write(_cartridge.VRomBankData, 0, _cartridge.VRomBankData.Length);
            writer.Write(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);
        }

        public override void ReadState(Stream stream)
        {
            var reader = new BinaryReader(stream);

            _bankRegisterSelect = reader.ReadByte();
            _chrSelect = reader.ReadByte();
            _prgSelect = reader.ReadByte();
            irq_counter = reader.ReadByte();
            irq_latch = reader.ReadByte();
            irq_reload = reader.ReadBoolean();
            enable_interrupts = reader.ReadBoolean();
            enable_sram = reader.ReadBoolean();
            enable_write_protect = reader.ReadBoolean();

            _cartridge.Mirror = (MirrorEnum)reader.ReadByte();

            reader.Read(_bankRegister, 0, _bankRegister.Length);
            //_chrBankOffsets = reader.ReadUInt32Array(_chrBankOffsets.Length);
            //_prgBankOffsets = reader.ReadUInt32Array(_prgBankOffsets.Length);
            reader.Read(_cartridge.VRomBankData, 0, _cartridge.VRomBankData.Length);
            reader.Read(_cartridge.RamBankData, 0, _cartridge.RamBankData.Length);

            UpdateOffsets();
        }


        protected void UpdateOffsets()
        {
            switch (_prgSelect)
            {
                case 0:
                    _prgBankOffsets[0] = _bankRegister[0x6] * 0x2000U; // R6
                    _prgBankOffsets[1] = _bankRegister[0x7] * 0x2000U; // R7
                    _prgBankOffsets[2] = _bankRegister[0xF] * 0x2000U; // RF
                    _prgBankOffsets[3] = _lastBankOffset + 0x2000;
                    break;
                case 1:
                    _prgBankOffsets[0] = _bankRegister[0xF] * 0x2000U; // RF
                    _prgBankOffsets[1] = _bankRegister[0x7] * 0x2000U; // R7
                    _prgBankOffsets[2] = _bankRegister[0x6] * 0x2000U; // R6
                    _prgBankOffsets[3] = _lastBankOffset + 0x2000;
                    break;
            }

            switch (_chrSelect)
            {
                case 0:
                    _chrBankOffsets[0] = _bankRegister[0] & 0xFEU;
                    _chrBankOffsets[1] = (_kSelect == 0 ? _bankRegister[0] & 0xFEU : _bankRegister[8]);
                    _chrBankOffsets[2] = _bankRegister[1] & 0xFEU;
                    _chrBankOffsets[3] = (_kSelect == 0 ? _bankRegister[1] & 0xFEU : _bankRegister[9]) ;
                    _chrBankOffsets[4] = _bankRegister[2];
                    _chrBankOffsets[5] = _bankRegister[3];
                    _chrBankOffsets[6] = _bankRegister[4];
                    _chrBankOffsets[7] = _bankRegister[5];
                    break;
                case 1:
                    _chrBankOffsets[0] = _bankRegister[2];
                    _chrBankOffsets[1] = _bankRegister[3];
                    _chrBankOffsets[2] = _bankRegister[4];
                    _chrBankOffsets[3] = _bankRegister[5];
                    _chrBankOffsets[4] = _bankRegister[0] & 0xFEU;
                    _chrBankOffsets[5] = (_kSelect == 0 ? _bankRegister[0] & 0xFEU : _bankRegister[8]);
                    _chrBankOffsets[6] = _bankRegister[1] & 0xFEU;
                    _chrBankOffsets[7] = (_kSelect == 0 ? _bankRegister[1] & 0xFEU : _bankRegister[9]);
                    break;
            }

            for (int i = 0; i < _prgBankOffsets.Length; i++)
                _prgBankOffsets[i] %= (uint)_cartridge.RomBankData.Length;

            for (int i = 0; i < _chrBankOffsets.Length; i++)
                _chrBankOffsets[i] = (uint)(_chrBankOffsets[i] * 0x400 % _cartridge.VRomBankData.Length);
        }

        public override void Reset()
        {

        }

    }
}