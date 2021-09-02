namespace Fami.Core.Audio
{
    public class Sweeper
    {
        public bool enabled = false;
        public bool down = false;
        public bool reload = false;
        public byte shift = 0x00;
        public uint timer = 0x00;
        public uint period = 0x00;
        public int change = 0;
        public bool mute = false;

        public void track(int target)
        {
            if (enabled)
            {
                change = target >> shift;
                mute = (target < 8) || (target > 0x7FF);
            }
        }

        public bool clock(ref int target, int channel)
        {
            bool changed = false;
            if (timer == 0 && enabled && shift > 0 && !mute)
            {
                if (target >= 8 && change < 0x07FF)
                {
                    if (down)
                    {
                        target -= change - channel;
                    }
                    else
                    {
                        target += change;
                    }
                    changed = true;
                }
            }

            //if (enabled)
            {
                if (timer == 0 || reload)
                {
                    timer = period;
                    reload = false;
                }
                else
                    timer--;

                mute = (target < 8) || (target > 0x7FF);
            }

            return changed;
        }
    };
}