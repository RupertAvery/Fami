namespace Fami.Core.Mappers
{
    public abstract class BaseMapper
    {
        public abstract (int value, bool handled) CpuMapRead(int address);
        public abstract (int value, bool handled) CpuMapWrite(int address);
        public abstract (int value, bool handled) PpuMapRead(int address);
        public abstract (int value, bool handled) PpuMapWrite(int address);
    }
}