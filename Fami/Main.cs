using System;
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

        public Main()
        {
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

        public void Run()
        {
            var emu = new CpuEmu();
            emu.Init();

            var rom = INesFile.Read("nestest.nes");

            var addr = 0x10000;
            addr = addr - rom.RomBank * 16384;

            for (var i = 0; i < rom.RomBank; i++)
            {
                var romBank = rom.RomBanks[i];

                for (var j = 0; j < romBank.Data.Length; j++)
                {
                    emu.Cpu.Memory.Write(addr + j, romBank.Data[j]);
                }
            }

            //emu.Cpu.Memory.Write(0xFFFC, 0x00);
            //emu.Cpu.Memory.Write(0xFFFD, 0xC0);

            //emu.Cpu.Memory.Write(0xC000, 0x4C);
            //emu.Cpu.Memory.Write(0xC001, 0xF5);
            //emu.Cpu.Memory.Write(0xC002, 0xC5);
            //emu.Cpu.Memory.Write(0xC5F5, 0xA2);
            //emu.Cpu.Memory.Write(0xC5F6, 0x00);
            //emu.Cpu.Memory.Write(0xC5F7, 0x86);
            //emu.Cpu.Memory.Write(0xC5F8, 0x00);
            //emu.Cpu.Memory.Write(0xC5F9, 0x86);
            //emu.Cpu.Memory.Write(0xC5FA, 0x10);
            //emu.Cpu.Memory.Write(0xC5FB, 0x86);
            //emu.Cpu.Memory.Write(0xC5FC, 0x11);
            //emu.Cpu.Memory.Write(0xC5FD, 0x20);
            //emu.Cpu.Memory.Write(0xC5FE, 0x2D);
            //emu.Cpu.Memory.Write(0xC5FF, 0xC7);
            emu.Reset();

            emu.Cpu.PC = 0xc000;

            emu.Execute();
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
            Destroy();
        }
    }
}