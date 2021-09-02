using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Fami.Core;
using SDL2;
using static SDL2.SDL;

namespace Fami
{
    public unsafe class Main : IDisposable
    {
        const uint AUDIO_SAMPLE_FULL_THRESHOLD = 1024;
        const int SAMPLES_PER_CALLBACK = 32;
        static SDL_AudioSpec want, have;
        static uint AudioDevice;

        static IntPtr Window;
        static IntPtr Renderer;
        public const int WIDTH = 256;
        public const int HEIGHT = 240;
        public bool Stretched { get; set; }
        public bool IntegerScaling { get; set; }
        static IntPtr Texture;
        public bool ColorCorrection { get; set; }
        private Cpu6502State nes;
        public PpuState[] PpuStates = new PpuState[256];
        public CpuState CpuState;
        public PpuState PpuState;
        public byte[] MapperState = new byte[16384];

        public uint ppuStateTail = 0;
        public uint ppuStateHead = 0;
        public uint Frames;
        public bool hasState;

        public void SaveState()
        {
            nes.WriteState(ref CpuState);
            nes.Ppu.WriteState(ref PpuState);
            nes.Cartridge.WriteState(ref  MapperState);
            hasState = true;
        }

        public void LoadState()
        {
            if (hasState)
            {
                nes.ReadState(CpuState);
                nes.Ppu.ReadState(PpuState);
                nes.Cartridge.ReadState(MapperState);
            }
        }

        public Main()
        {
            for (var i = 0; i < 256; i++)
            {
                PpuStates[i] = PpuState.New();
            }

            CpuState = CpuState.New();
            PpuState = PpuState.New();


            nes = new Cpu6502State(AudioReady);

            SDL_Init(SDL_INIT_AUDIO | SDL_INIT_VIDEO);

            Window = SDL_CreateWindow("Fami", 0, 0, WIDTH * 4, HEIGHT * 4, SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
            SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            Renderer = SDL_CreateRenderer(Window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            Texture = SDL_CreateTexture(Renderer, SDL_PIXELFORMAT_ABGR8888, (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, WIDTH, HEIGHT);

            //SDL_SetWindowMinimumSize(Window, WIDTH, HEIGHT);
            //SDL_SetWindowSize(Window, WIDTH * 4, HEIGHT * 4);

            want.channels = 2;
            want.freq = 32768;
            want.samples = SAMPLES_PER_CALLBACK;
            want.format = AUDIO_S16LSB;
            // want.callback = NeedMoreAudioCallback;
            AudioDevice = SDL_OpenAudioDevice(null, 0, ref want, out have, (int)SDL_AUDIO_ALLOW_FORMAT_CHANGE);
            SDL_PauseAudioDevice(AudioDevice, 0);

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
                    var p = nes.Ppu.buffer[x + y * WIDTH];
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
        private static bool Sync = true;
        private bool SaveStatePending;
        private bool LoadStatePending;
        public void EmulationThreadHandler()
        {
            try
            {
                while (running)
                {
                    ThreadSync.WaitOne();
                    RunFrame();
                    while (!Sync)
                    {
                        RunFrame();
                    }
                    if (SaveStatePending)
                    {
                        SaveState();
                        SaveStatePending = false;
                    }
                    if (LoadStatePending)
                    {
                        LoadState();
                        LoadStatePending = false;
                    }
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
        //const double SecondsPerFrame = 1D / (5369318D / 89341D);
        private const double SecondsPerFrame = 1 / 59.97D;
        private double Fps;

        public void RunFrame()
        {
            CyclesRan += CyclesPerFrame;
            CyclesLeft += CyclesPerFrame;
            //while (CyclesLeft > 0)
            //{
            //    CyclesLeft -= (int)nes.Step();
            //}
            //89342
            //CyclesLeft = 89341 + ((Frames % 2 == 0 )? 1 : 0);
            while (CyclesLeft > 0)
            {
                CyclesLeft -= (int)nes.Step();
            }

            //for (var i = -1; i < 261; i++)
            //{
            //    for (var j = 0; j < 341; j++)
            //    {
            //        nes.Step();
            //    }
            //}


            if (Frames % 4 == 0)
            {
                nes.Ppu.WriteState(ref PpuStates[ppuStateHead]);
                ppuStateHead++;
                if (ppuStateHead > 255)
                {
                    ppuStateHead = 0;
                }
            }

            Frames++;
        }

        public void Test()
        {
            nes.Init();
            var cart = Cartridge.Load("nestest.nes", nes);
            nes.LoadCartridge(cart);
            nes.Reset();
            nes.PC = 0xC000;
            nes.Execute();
        }

        public void Load(string rompath)
        {
            Cartridge cart = null;
            nes.Init();
            if (Path.GetExtension(rompath).ToLower() == ".zip")
            {
                using var zipFile = ZipFile.Open(rompath, ZipArchiveMode.Read);
                var nesFile = zipFile.Entries.FirstOrDefault(z => Path.GetExtension(z.Name).ToLower() == ".nes");
                if (nesFile == default)
                {

                }
                else
                {
                    using var s = nesFile.Open();
                    cart = Cartridge.Load(s, nes);
                }
            }
            else
            {
                cart = Cartridge.Load(rompath, nes);
            }
            nes.LoadCartridge(cart);
            nes.Reset();
        }

        private bool running;
        private string romPath;
        private bool resetPending;
        public void Run()
        {
            EmulationThread = new Thread(EmulationThreadHandler);
            EmulationThread.Name = "Emulation Core";
            EmulationThread.Start();

            running = true;

            var nextFrameAt = GetTime();
            double fpsEvalTimer = 0;

            void resetTimers()
            {
                var time = GetTime();
                nextFrameAt = time;
                fpsEvalTimer = time;
            }
            resetTimers();

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

                        case SDL_EventType.SDL_DROPFILE:
                            var filename = Marshal.PtrToStringUTF8(evt.drop.file);
                            try
                            {
                                romPath = filename;
                                resetPending = true;
                            }
                            catch
                            {
                                //Log("An error occurred loading the dropped ROM file.");
                                return;
                            }
                            break;
                    }
                }

                if (resetPending)
                {
                    Load(romPath);
                    resetPending = false;
                }


                if (Pause)
                {
                    if (FrameAdvance)
                    {
                        FrameAdvance = false;
                        RunFrame();
                    }
                }
                else
                {
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

                    if (currentSec >= fpsEvalTimer)
                    {
                        double diff = currentSec - fpsEvalTimer + 1;
                        double frames = CyclesRan / CyclesPerFrame;
                        CyclesRan = 0;

                        //double mips = (double)Gba.Cpu.InstructionsRan / 1000000D;
                        //Gba.Cpu.InstructionsRan = 0;

                        // Use Math.Floor to truncate to 2 decimal places
                        Fps = Math.Floor((frames / diff) * 100) / 100;
                        //Mips = Math.Floor((mips / diff) * 100) / 100;
                        UpdateTitle();
                        //Seconds++;

                        fpsEvalTimer += 1;
                    }
                }





                Draw();
            }

            ThreadSync.Close();

        }

        private void UpdateTitle()
        {
            SDL_SetWindowTitle(
                Window,
                "Fami - " + Fps + " fps"
            );
        }

        private uint controller1state = 0;

        private bool Pause;
        private bool FrameAdvance;

        private void KeyEvent(SDL_KeyboardEvent evtKey)
        {
            if (evtKey.type == SDL_EventType.SDL_KEYDOWN)
            {

                switch (evtKey.keysym.sym)
                {
                    case SDL_Keycode.SDLK_UP:
                        controller1state |= 0x08U;
                        break;
                    case SDL_Keycode.SDLK_DOWN:
                        controller1state |= 0x04U;
                        break;
                    case SDL_Keycode.SDLK_LEFT:
                        controller1state |= 0x02U;
                        break;
                    case SDL_Keycode.SDLK_RIGHT:
                        controller1state |= 0x01U;
                        break;
                    case SDL_Keycode.SDLK_RETURN:
                        controller1state |= 0x10U;
                        break;
                    case SDL_Keycode.SDLK_LSHIFT:
                    case SDL_Keycode.SDLK_RSHIFT:
                        controller1state |= 0x20U;
                        break;
                    case SDL_Keycode.SDLK_z:
                        controller1state |= 0x80U;
                        break;
                    case SDL_Keycode.SDLK_x:
                        controller1state |= 0x40U;
                        break;
                    case SDL_Keycode.SDLK_ESCAPE:
                        nes.Reset();
                        break;
                    case SDL_Keycode.SDLK_SPACE:
                        Sync = false;
                        break;
                    case SDL_Keycode.SDLK_F2:
                        SaveStatePending = true;
                        break;
                    case SDL_Keycode.SDLK_F4:
                        LoadStatePending = true;
                        break;
                    case SDL_Keycode.SDLK_p:
                        Pause = !Pause;
                        Console.WriteLine(Pause ? "Paused" : "Resumed");
                        break;
                    case SDL_Keycode.SDLK_f:
                        if (Pause)
                        {
                            Console.WriteLine("Frame Advanced");
                            FrameAdvance = true;
                        }
                        break;
                }
            }
            if (evtKey.type == SDL_EventType.SDL_KEYUP)
            {

                switch (evtKey.keysym.sym)
                {
                    case SDL_Keycode.SDLK_UP:
                        controller1state &= ~0x08U;
                        break;
                    case SDL_Keycode.SDLK_DOWN:
                        controller1state &= ~0x04U;
                        break;
                    case SDL_Keycode.SDLK_LEFT:
                        controller1state &= ~0x02U;
                        break;
                    case SDL_Keycode.SDLK_RIGHT:
                        controller1state &= ~0x01U;
                        break;
                    case SDL_Keycode.SDLK_RETURN:
                        controller1state &= ~0x10U;
                        break;
                    case SDL_Keycode.SDLK_LSHIFT:
                    case SDL_Keycode.SDLK_RSHIFT:
                        controller1state &= ~0x20U;
                        break;
                    case SDL_Keycode.SDLK_z:
                        controller1state &= ~0x80U;
                        break;
                    case SDL_Keycode.SDLK_x:
                        controller1state &= ~0x40U;
                        break;
                    case SDL_Keycode.SDLK_SPACE:
                        Sync = true;
                        break;

                }
            }

            nes.Controller[0] = controller1state;

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
            Marshal.FreeHGlobal(AudioTempBufPtr);
            ThreadSync.Dispose();
            Destroy();
        }

        static IntPtr AudioTempBufPtr = Marshal.AllocHGlobal(16384);

        static void AudioReady(short[] data)
        {
            // Don't queue audio if too much is in buffer
            if (Sync || GetAudioSamplesInQueue() < AUDIO_SAMPLE_FULL_THRESHOLD)
            {
                int bytes = sizeof(short) * data.Length;

                Marshal.Copy(data, 0, AudioTempBufPtr, data.Length);

                // Console.WriteLine("Outputting samples to SDL");

                SDL_QueueAudio(AudioDevice, AudioTempBufPtr, (uint)bytes);
            }
        }


        public static uint GetAudioSamplesInQueue()
        {
            return SDL_GetQueuedAudioSize(AudioDevice) / sizeof(short);
        }
    }

}