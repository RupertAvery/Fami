using System;

namespace Fami.Core.Audio
{
    public class Sequencer
    {
        public uint sequence;
        public int timer;
        public int reload;
        public uint output;
        public uint NewSequence { get; set; }

        public uint Clock(bool enabled, Func<uint, uint> manip)
        {
            if (enabled)
            {
                timer--;
                if (timer < 0)
                {
                    timer = reload;
                    sequence = manip(sequence);
                    output = sequence & 1;
                }
                return output;
            }

            return 0;
        }
    }
}