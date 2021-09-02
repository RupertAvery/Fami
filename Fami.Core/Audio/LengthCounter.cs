namespace Fami.Core.Audio
{
    public class LengthCounter
    {
        public uint counter = 0x00;
        public uint clock(bool bEnable, bool bHalt)
        {
            if (!bEnable)
                counter = 0;
            else
            if (counter > 0 && !bHalt)
                counter--;
            return counter;
        }
    };
}