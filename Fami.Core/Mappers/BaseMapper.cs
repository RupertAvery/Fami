namespace Fami.Core.Mappers
{
    public abstract class BaseMapper
    {
        public abstract (uint value, bool handled) CpuMapRead(uint address);
        public abstract bool CpuMapWrite(uint address, uint value);
        public abstract (uint value, bool handled) PpuMapRead(uint address);
        public abstract bool PpuMapWrite(uint address, uint value);
    }
}