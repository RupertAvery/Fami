using System.IO;
using Fami.Core.Mappers;

namespace Fami.Core
{
    public enum MirrorEnum
    {
        Horizontal = 0,
        Vertical = 1,
    }

    public class Cartridge
    {
        private const int ROMBANK_SIZE = 16384;
        private const int VROMBANK_SIZE = 8192;

        public byte RomBanks { get; private set; }
        public byte VRomBanks { get; private set; }
        public byte Flags6 { get; private set; }
        public byte Flags7 { get; private set; }
        public byte RamBank { get; private set; }
        public byte Region { get; private set; }
        public byte[] RomBankData { get; private set; }
        public byte[] VRomBankData { get; private set; }
        public byte[] RamBankData { get; private set; }
        public byte[] SaveRamData { get; private set; }
        public BaseMapper Mapper { get; private set; }
        public MirrorEnum Mirror { get;set;}

        public Cartridge()
        {

        }

        public void Reset()
        {
        }

        public static Cartridge Load(string path)
        {
            using (var f = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var r = new BinaryReader(f);
                var header = r.ReadBytes(4);
                var h = new Cartridge();
                h.RomBanks = r.ReadByte();
                h.RomBankData = new byte[h.RomBanks * ROMBANK_SIZE];
                h.VRomBanks = r.ReadByte();
                h.VRomBankData = new byte[h.VRomBanks * VROMBANK_SIZE];
                h.Flags6 = r.ReadByte();
                h.Flags7 = r.ReadByte();
                h.RamBank = r.ReadByte();
                h.Region = r.ReadByte();
                r.ReadBytes(6);
                h.RomBankData = r.ReadBytes(h.RomBanks * ROMBANK_SIZE);
                h.VRomBankData = r.ReadBytes(h.VRomBanks * VROMBANK_SIZE);
                h.Mirror = (MirrorEnum)(h.Flags6 & 0x01);
                h.RamBankData = new byte[8192];

                var mapperId = ((h.Flags6 >> 4) & 0x0F) | (h.Flags7 & 0xF0);
                h.Mapper = mapperId switch
                {
                    0 => new Mapper000(h),
                    2 => new Mapper002(h),
                    _ => new Mapper000(h)
                };
                return h;
            }
        }

        public (uint value, bool handled) CpuRead(uint address)
        {
            return Mapper.CpuMapRead(address);
        }

        public bool CpuWrite(uint address, uint value)
        {
            return Mapper.CpuMapWrite(address, value);
        }

        public (uint value, bool handled) PpuRead(uint address)
        {
            return Mapper.PpuMapRead(address);
        }

        public bool PpuWrite(uint address, uint value)
        {
            return Mapper.PpuMapWrite(address, value);
        }
    }
}