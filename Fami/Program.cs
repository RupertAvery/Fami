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
                string rom;
                //main.Test();
                ////main.Load(args[0]);
                //rom = @"D:\roms\NES\nes-test-roms\scanline\scanline.nes";
                //rom = @"D:\roms\NES\Chip 'n Dale Rescue Rangers (U) [o2].zip";
                //rom = @"D:\roms\NES\Super Mario Bros (E).nes";
                //rom = @"D:\roms\NES\Super Mario Bros. (JU) (PRG0) [!].nes";
                //rom = @"D:\roms\NES\zelda.nes";
                //rom = @"D:\roms\NES\nestest.nes";
                //rom = @"D:\roms\NES\nes-test-roms\soundtest\SNDTEST.NES";
                //rom = @"D:\roms\NES\nes-test-roms\240pee\240pee.nes";
                //var rom = @"D:\roms\NES\nes-test-roms\scrolltest\scroll.nes";
                //var rom = @"D:\roms\NES\nes-test-roms\mmc3_irq_tests\1.Clocking.nes";
                //rom = @"D:\roms\NES\Adventure Island 3.zip";
                //rom = @"D:\roms\NES\Super Mario Bros 3.zip";
                rom = @"D:\roms\NES\Final Fantasy 3 (J).zip";
                //rom = @"D:\roms\NES\donkey.nes";
                //rom = @"D:\roms\NES\Wizards & Warriors 1.zip";
                //rom = @"D:\roms\NES\nes-test-roms\branch_timing_tests\1.Branch_Basics.nes";
                main.Load(rom);
                main.Run();
            }
        }
    }
}