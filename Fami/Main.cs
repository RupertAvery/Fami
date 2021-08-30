using System;
using System.Threading;
using Fami.Core;
using static SDL2.SDL;

namespace Fami
{
    public unsafe class Main : IDisposable
    {
        static IntPtr Window;
        static IntPtr Renderer;
        public const int WIDTH = 256;
        public const int HEIGHT = 240;
        public bool Stretched { get; set; }
        public bool IntegerScaling { get; set; }
        static IntPtr Texture;
        public bool ColorCorrection { get; set; }
        private CpuEmu emu;

        public Main()
        {

            emu = new CpuEmu();

            SDL_Init(SDL_INIT_AUDIO | SDL_INIT_VIDEO);

            Window = SDL_CreateWindow("Fami", 0, 0, WIDTH * 4, HEIGHT * 4, SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            Texture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_ABGR8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, WIDTH, HEIGHT);

            //SDL_SetWindowMinimumSize(Window, WIDTH, HEIGHT);
            //SDL_SetWindowSize(Window, WIDTH * 4, HEIGHT * 4);

        }

        static uint[] DisplayBuf = new uint[WIDTH * HEIGHT];

        //public static void CopyPixels(ushort* src, uint[] dest, uint pixels, bool colorCorrection)
        //{
        //    var lut = colorCorrection ? PpuRenderer.ColorLutCorrected : PpuRenderer.ColorLut;

        //    for (uint i = 0; i < pixels; i++)
        //    {
        //        DisplayBuf[i] = lut[src[i] & 0x7FFF];
        //    }
        //}

        public void Fill(uint a, uint r, uint g, uint b)
        {
            for (uint i = 0; i < WIDTH * HEIGHT; i++)
            {
                DisplayBuf[i] = a << 24 | b << 16 | g << 8 | r;
            }
        }

        public void Draw()
        {
            for (var y = 0; y < HEIGHT; y++)
            {
                for (var x = 0; x < WIDTH; x++)
                {
                    var p = emu.Cpu.Ppu.Renderer.buffer[x + y * WIDTH];
                    //var r = p >> 16 & 0xFF;
                    //var g = p >> 8 & 0xFF;
                    //var b = p & 0xFF;
                    //var q = DisplayBuf[x + y * WIDTH];
                    //var i = q >> 16 & 0xFF;
                    //var j = q >> 8 & 0xFF;
                    //var k = q & 0xFF;

                    //if (r < 150 & g < 150 & b < 150)
                    //{
                    //    r = (uint)(i * 0.99);
                    //    g = (uint)(j * 0.99);
                    //    b = (uint)(k * 0.99);
                    //}

                    //p = 0xFF000000 | (r << 16) | (g << 8) | b;
                    DisplayBuf[x + y * WIDTH] = p;
                }
            }

            //CopyPixels(Gba.Ppu.Renderer.ScreenFront, DisplayBuf, WIDTH * HEIGHT, ColorCorrection);
            fixed (void* ptr = DisplayBuf)
                SDL_UpdateTexture(Texture, IntPtr.Zero, (IntPtr)ptr, WIDTH * 4);


            SDL_Rect dest = new SDL_Rect();
            SDL_GetWindowSize(Window, out int w, out int h);
            double ratio = Math.Min((double)h / (double)HEIGHT, (double)w / (double)WIDTH);
            int fillWidth;
            int fillHeight;
            if (!Stretched)
            {
                if (IntegerScaling)
                {
                    fillWidth = ((int)(ratio * WIDTH) / WIDTH) * WIDTH;
                    fillHeight = ((int)(ratio * HEIGHT) / HEIGHT) * HEIGHT;
                }
                else
                {
                    fillWidth = (int)(ratio * WIDTH);
                    fillHeight = (int)(ratio * HEIGHT);
                }
                dest.w = fillWidth;
                dest.h = fillHeight;
                dest.x = (int)((w - fillWidth) / 2);
                dest.y = (int)((h - fillHeight) / 2);
            }
            else
            {
                dest.w = w;
                dest.h = h;
                dest.x = 0;
                dest.y = 0;
            }

            SDL_RenderClear(Renderer);
            SDL_RenderCopy(Renderer, Texture, IntPtr.Zero, ref dest);
            SDL_RenderPresent(Renderer);
        }


        static int CyclesLeft;
        static long CyclesRan;
        static Thread EmulationThread;
        static AutoResetEvent ThreadSync = new AutoResetEvent(false);

        public void EmulationThreadHandler()
        {
            try
            {
                while (running)
                {
                    ThreadSync.WaitOne();

                    RunFrame();

                    //while (!Sync)
                    //{
                    //    RunFrame();
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Excepted = true;
            }
        }

        //public const int CyclesPerFrame = 29780;

        //1.662607MHz = 1662607Hz
        //1662607Hz/59.9Hz = 27756cycles/frame
        //public const int CyclesPerFrame = 27756;
        //public const int CyclesPerFrame = 27756;
        public const int CyclesPerFrame = 89341;
        const double SecondsPerFrame = 1D / (5369318D / 89341D);
        //private const double SecondsPerFrame = 1 / 59.97D;

        public void RunFrame()
        {
            CyclesRan += CyclesPerFrame;
            CyclesLeft += CyclesPerFrame;
            while (CyclesLeft > 0)
            {
                CyclesLeft -= (int)emu.Step();
            }
        }

        public void Test()
        {
            emu.Init();
            var cart = Cartridge.Load("nestest.nes");
            emu.LoadCartridge(cart);
            emu.Reset();
            emu.Cpu.PC = 0xC000;
            emu.Execute();
        }

        public void Load(string rompath)
        {
            emu.Init();
            var cart = Cartridge.Load(rompath);
            emu.LoadCartridge(cart);
            emu.Reset();
        }

        private bool running;
        public void Run()
        {
            EmulationThread = new Thread(EmulationThreadHandler);
            EmulationThread.Name = "Emulation Core";
            EmulationThread.Start();

            running = true;

            var nextFrameAt = GetTime();

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
                            KeyEvent(evt.key);
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

                //main.Fill(255, r, g, b);

                //r = (uint)rand.Next(255);
                //g = (uint)rand.Next(255);
                //b = (uint)rand.Next(255);
                //r++;
                //if (r > 255) r = 0;
                //if (r % 3 == 0) g++;
                //if (g > 255) g = 0;
                //if (g % 3 == 0) b++;
                //if (b > 255) b = 0;

                double currentSec = GetTime();

                // Reset time if behind schedule
                if (currentSec - nextFrameAt >= SecondsPerFrame)
                {
                    double diff = currentSec - nextFrameAt;
                    Console.WriteLine("Can't keep up! Skipping " + (int)(diff * 1000) + " milliseconds");
                    nextFrameAt = currentSec;
                }

                if (currentSec >= nextFrameAt)
                {
                    nextFrameAt += SecondsPerFrame;

                    ThreadSync.Set();
                }

                //if (currentSec >= fpsEvalTimer)
                //{
                //    double diff = currentSec - fpsEvalTimer + 1;
                //    double frames = CyclesRan / CyclesPerFrameGba;
                //    CyclesRan = 0;

                //    double mips = (double)Gba.Cpu.InstructionsRan / 1000000D;
                //    Gba.Cpu.InstructionsRan = 0;

                //    // Use Math.Floor to truncate to 2 decimal places
                //    Fps = Math.Floor((frames / diff) * 100) / 100;
                //    Mips = Math.Floor((mips / diff) * 100) / 100;
                //    UpdateTitle();
                //    Seconds++;
                //    UpdatePlayingRpc();

                //    fpsEvalTimer += 1;
                //}

                Draw();
            }

            ThreadSync.Close();

        }

        private void KeyEvent(SDL_KeyboardEvent evtKey)
        {
            uint controller1state = 0;
            var held = evtKey.type == SDL_EventType.SDL_KEYDOWN;

            switch (evtKey.keysym.sym)
            {
                case SDL_Keycode.SDLK_UP:
                    controller1state |= held ? 0x08U : 00;
                    break;
                case SDL_Keycode.SDLK_DOWN:
                    controller1state |= held ? 0x04U : 00;
                    break;
                case SDL_Keycode.SDLK_LEFT:
                    controller1state |= held ? 0x02U : 00;
                    break;
                case SDL_Keycode.SDLK_RIGHT:
                    controller1state |= held ? 0x01U : 00;
                    break;
                case SDL_Keycode.SDLK_RETURN:
                    controller1state |= held ? 0x10U : 00;
                    break;
                case SDL_Keycode.SDLK_LSHIFT:
                case SDL_Keycode.SDLK_RSHIFT:
                    controller1state |= held ? 0x20U : 00;
                    break;
                case SDL_Keycode.SDLK_z:
                    controller1state |= held ? 0x80U : 00;
                    break;
                case SDL_Keycode.SDLK_x:
                    controller1state |= held ? 0x40U : 00;
                    break;
            }

            emu.Cpu.Controller[0] = controller1state;

        }

        public static double GetTime()
        {
            return (double)SDL_GetPerformanceCounter() / (double)SDL_GetPerformanceFrequency();
        }

        private void Destroy()
        {
            SDL_DestroyTexture(Texture);
            SDL_DestroyRenderer(Renderer);
            SDL_DestroyWindow(Window);
            SDL_AudioQuit();
            SDL_VideoQuit();
            SDL_Quit();
        }

        public void Dispose()
        {
            ThreadSync.Dispose();
            Destroy();
        }
    }
}