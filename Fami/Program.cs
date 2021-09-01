using System;
using System.IO;
using static SDL2.SDL;

namespace Fami
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var main = new Main())
            {
                //main.Test();
                ////main.Load(args[0]);
                //var rom = @"D:\roms\NES\nes-test-roms\scanline\scanline.nes";
                var rom = @"D:\roms\NES\zelda.nes";
                //var rom = @"D:\roms\NES\smb.nes";
                //var rom = @"D:\roms\NES\donkey.nes";
                ////var rom = @"D:\roms\NES\nes-test-roms\branch_timing_tests\1.Branch_Basics.nes";
                main.Load(rom);
                main.Run();
            }
        }
    }
}