namespace Fami.Core.Mappers
{
    public abstract class BaseMapper
    {
        public abstract (uint value, bool handled) CpuMapRead(uint address);
        public abstract (uint value, bool handled) CpuMapWrite(uint address);
        public abstract (uint value, bool handled) PpuMapRead(uint address);
        public abstract (uint value, bool handled) PpuMapWrite(uint address);
    }
}