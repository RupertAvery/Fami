namespace Fami.Core.CPU
{
    public class Cpu6502Memory
    {
        public int[] Memory { get; }

        public Cpu6502Memory()
        {
            Memory = new int[0x10000];
        }

        public int Read(int addr)
        {
            return Memory[addr];
        }

        public void Write(int addr, int val)
        {
            Memory[addr] = val;
        }

        public int ReadWord(int addr)
        {
            return 0;
        }

        public void WriteWord(int addr, int val)
        {

        }
    }
}