namespace Fami.Core.Audio
{
    public class Envelope
    {
        public void Clock(bool bLoop)
        {
            if (!start)
            {
                if (divider_count == 0)
                {
                    divider_count = volume;

                    if (decay_count == 0)
                    {
                        if (bLoop)
                        {
                            decay_count = 15;
                        }

                    }
                    else
                        decay_count--;
                }
                else
                    divider_count--;
            }
            else
            {
                start = false;
                decay_count = 15;
                divider_count = volume;
            }

            if (disable)
            {
                output = volume;
            }
            else
            {
                output = decay_count;
            }
        }

        public bool start = false;
        public bool disable = false;
        public uint divider_count = 0;
        public uint volume = 0;
        public uint output = 0;
        public uint decay_count = 0;
    };
}