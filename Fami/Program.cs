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
                //main.Test();
                main.Load(args[0]);
                main.Run();
            }
        }
    }
}