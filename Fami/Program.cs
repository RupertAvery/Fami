using System;
using System.IO;
using static SDL2.SDL;

namespace Fami
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
                for (var i = 0; i < h.RomBank; i++)
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
            var rand = new Random();

            using (var main = new Main())
            {
                bool running = true;
                uint r = 0;
                uint g = 0;
                uint b = 0;

                while (running)
                {
                    SDL_Event evt;
                    while (SDL_PollEvent(out evt) != 0)
                    {
                        switch (evt.type)
                        {
                            case SDL_EventType.SDL_QUIT:
                                running = false;
                                break;
                            case SDL_EventType.SDL_KEYUP:
                            case SDL_EventType.SDL_KEYDOWN:
                                //KeyEvent(evt.key);
                                break;

                                //case SDL_EventType.SDL_DROPFILE:
                                //    var filename = Marshal.PtrToStringUTF8(evt.drop.file);
                                //    try
                                //    {
                                //        romPath = filename;
                                //        goto reload;
                                //    }
                                //    catch
                                //    {
                                //        Log("An error occurred loading the dropped ROM file.");
                                //        return;
                                //    }
                        }
                    }

                    main.Fill(255, r, g, b);

                    r = (uint)rand.Next(255);
                    g = (uint)rand.Next(255);
                    b = (uint)rand.Next(255);
                    //r++;
                    //if (r > 255) r = 0;
                    //if (r % 3 == 0) g++;
                    //if (g > 255) g = 0;
                    //if (g % 3 == 0) b++;
                    //if (b > 255) b = 0;

                    main.Draw();
                }
            }
        }
    }
}