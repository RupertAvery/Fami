using System;
using System.IO;
using Fami.Core;

namespace Nestor
{
    public class RomBank
    {
        public RomBank()
        {
            Data = new byte[16384];
        }
        public byte[] Data;
    }

    public class VRomBank
    {
        public VRomBank()
        {
            Data = new byte[8192];
        }

        public byte[] Data;
    }

    public class INesRom
    {
        public byte RomBank { get; set; }
        public byte VRomBank { get; set; }
        public byte FlagsL { get; set; }
        public byte FlagsH { get; set; }
        public byte RamBank { get; set; }
        public byte Region { get; set; }
        public RomBank[] RomBanks { get; set; }
        public VRomBank[] VRomBanks { get; set; }
    }

    public class INesFile
    {
        public static INesRom Read(string path)
        {
            var h = new INesRom();
            using (var f = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var r = new BinaryReader(f);
                r.ReadBytes(4);
                h.RomBank = r.ReadByte();
                h.RomBanks = new RomBank[h.RomBank];
                h.VRomBank = r.ReadByte();
                h.VRomBanks = new VRomBank[h.VRomBank];
                h.FlagsL = r.ReadByte();
                h.FlagsH = r.ReadByte();
                h.RamBank = r.ReadByte();
                h.Region = r.ReadByte();
                r.ReadBytes(6);
                for(var i =0; i < h.RomBank; i++)
                {
                    h.RomBanks[i] = new RomBank();
                    h.RomBanks[i].Data = r.ReadBytes(16384);
                }
                for (var i = 0; i < h.VRomBank; i++)
                {
                    h.VRomBanks[i] = new VRomBank();
                    h.VRomBanks[i].Data = r.ReadBytes(8192);
                }
            }

            return h;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var emu = new CpuEmu();
            emu.Init();

            var rom = INesFile.Read("nestest.nes");

            var addr = 0x10000;
            addr = addr - rom.RomBank * 16384;

            for (var i=0;i < rom.RomBank; i++)
            {
                var romBank = rom.RomBanks[i];

                for (var j = 0; j < romBank.Data.Length; j++)
                {
                    emu.Cpu.Memory.Write(addr + j, romBank.Data[j]);
                }
            }

            //emu.Cpu.Memory.Write(0xFFFC, 0x00);
            //emu.Cpu.Memory.Write(0xFFFD, 0xC0);

            //emu.Cpu.Memory.Write(0xC000, 0x4C);
            //emu.Cpu.Memory.Write(0xC001, 0xF5);
            //emu.Cpu.Memory.Write(0xC002, 0xC5);
            //emu.Cpu.Memory.Write(0xC5F5, 0xA2);
            //emu.Cpu.Memory.Write(0xC5F6, 0x00);
            //emu.Cpu.Memory.Write(0xC5F7, 0x86);
            //emu.Cpu.Memory.Write(0xC5F8, 0x00);
            //emu.Cpu.Memory.Write(0xC5F9, 0x86);
            //emu.Cpu.Memory.Write(0xC5FA, 0x10);
            //emu.Cpu.Memory.Write(0xC5FB, 0x86);
            //emu.Cpu.Memory.Write(0xC5FC, 0x11);
            //emu.Cpu.Memory.Write(0xC5FD, 0x20);
            //emu.Cpu.Memory.Write(0xC5FE, 0x2D);
            //emu.Cpu.Memory.Write(0xC5FF, 0xC7);
            emu.Reset();

            emu.Cpu.PC = 0xc000;

            emu.Execute();
        }
    }
}