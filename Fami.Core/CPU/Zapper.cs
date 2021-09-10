using System.Runtime.CompilerServices;

namespace Fami.Core.CPU
{
    public partial class MC6502State
    {
        //private int locked_gun_cycle;
        //private int locked_gun_scanline;
        //private int locked_gun_timeout;
        private int trigger_timeout;
        public int gun_cycle;
        public int gun_scanline;
        private int gun_offscreen_timeout;

        public void TriggerLightGun(bool offscreen = false)
        {
            // The official Zapper has a trigger mechanism that ensures that the trigger switch is
            // only activated for around 100ms. This value is arbitrary and was chosen based on 
            // the ruder.nes test rom to have a trigger held time of 5, which is the same value as seen
            // in FCEUX. We decrement this value whenever we read from the controller port $4016/17
            trigger_timeout = 100;
            //locked_gun_cycle = gun_cycle;
            //locked_gun_scanline = gun_scanline;
            //locked_gun_timeout = 4500;

            if (offscreen)
            {
                // Prevent the sensor from seeing anything for some duration.
                // Maybe we can bind this to a key instead?
                // This is an arbitrary value chosen by running the emulator in release mode
                // and testing Duck Hunt. Firing off-screen should select the next game mode.

                gun_offscreen_timeout = 4500;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckLightGun(ref uint data)
        {

            // Zapper/Light gun logic.
            // Currently this activates for port 2

            // Always pull bit 4 (sense) high, low = detected
            data |= 0x08;

            var sense = 1;

            var _gun_cycle = gun_cycle;
            var _gun_scanline = gun_scanline;

            //if (locked_gun_timeout > 0)
            //{
            //    _gun_cycle = locked_gun_cycle;
            //    _gun_scanline = locked_gun_scanline;
            //    locked_gun_timeout--;
            //}

            if (_gun_cycle >= 0 && _gun_cycle < 256 && _gun_scanline >= 0 && _gun_scanline < 240)
            {
                // Read the RGB values at the target area, at the time the port is accessed
                var pixel = Ppu.buffer[_gun_cycle + _gun_scanline * 256];
                var r = (pixel >> 16) & 0xFF;
                var g = (pixel >> 8) & 0xFF;
                var b = (pixel) & 0xFF;

                // Inverted sense (sensed = 0)
                sense = r > 0x10 && g > 0x10 && b > 0x10 ? 0 : 1;

            }

            // technically a hack. Setting this above 0 prevents the sensor from "seeing" anything.
            // This is set during trigger to a high value, probably enough to cover an entire frame
            if (gun_offscreen_timeout == 0)
            {
                // Make sure that the sensor isn't triggered during HBlank or VBlank
                if (Ppu.cycle > 0 && Ppu.cycle <= 256 && Ppu.scanline > -1 && Ppu.scanline <= 240)
                {
                    // We only want to pull the actual data line low when PPU is at the area of interest.
                    // the cycle and scanline values will never actually be exactly equal(?), since cycles might not
                    // exactly align so we're good enough with checking if we're past the point
                    if (sense == 0 && Ppu.cycle >= _gun_cycle && Ppu.scanline >= _gun_scanline)
                    {
                        // Pull the sense bit low
                        data &= ~((uint)0x08);
                    }
                }
            }
            else
            {
                gun_offscreen_timeout--;
            }

            if (trigger_timeout > 0)
            {
                // pull the trigger bit high
                data |= 0x10;
                trigger_timeout--;
            }


        }

    }
}