using System;
using System.IO;
using static SDL2.SDL;

namespace Fami
{
 

    class Program
    {

        static void Main(string[] args)
        {
            var rand = new Random();

            using (var main = new Main())
            {
                bool running = true;
                uint r = 0;
                uint g = 0;
                uint b = 0;

                //main.Test();
                main.Load("nestest.nes");
                main.Run();
            }
        }
    }
}