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

                main.Run();

                while (running)
                {
                    SDL_Event evt;
                    while (SDL_PollEvent(out evt) != 0)
                    {
                        switch (evt.type)
                        {
                            case SDL_EventType.SDL_QUIT:
                                running = false;
                                break;
                            case SDL_EventType.SDL_KEYUP:
                            case SDL_EventType.SDL_KEYDOWN:
                                //KeyEvent(evt.key);
                                break;

                                //case SDL_EventType.SDL_DROPFILE:
                                //    var filename = Marshal.PtrToStringUTF8(evt.drop.file);
                                //    try
                                //    {
                                //        romPath = filename;
                                //        goto reload;
                                //    }
                                //    catch
                                //    {
                                //        Log("An error occurred loading the dropped ROM file.");
                                //        return;
                                //    }
                        }
                    }

                    main.Fill(255, r, g, b);

                    r = (uint)rand.Next(255);
                    g = (uint)rand.Next(255);
                    b = (uint)rand.Next(255);
                    //r++;
                    //if (r > 255) r = 0;
                    //if (r % 3 == 0) g++;
                    //if (g > 255) g = 0;
                    //if (g % 3 == 0) b++;
                    //if (b > 255) b = 0;

                    main.Draw();
                }
            }
        }
    }
}