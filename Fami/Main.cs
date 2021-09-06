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
    public class Main : IDisposable
    {
        public const int WIDTH = 256;
        public const int HEIGHT = 240;

        public const int CYCLES_PER_FRAME = 89341;
        private const double NTSC_SECONDS_PER_FRAME = 1 / 60D;
        private const double PAL_SECONDS_PER_FRAME = 1 / 50D;
        private double _fps;
        private int _cyclesLeft;
        private long _cyclesRan;
        private Thread _emulationThread;
        private readonly AutoResetEvent _threadSync = new AutoResetEvent(false);
        private static bool _sync = true;
        private bool _saveStatePending;
        private bool _loadStatePending;

        private bool _running;
        private string _romPath;
        private bool _resetPending;


        private uint _controller1State = 0;

        private bool _pause;
        private bool _frameAdvance;

        private readonly Cpu6502State _nes;
        public MemoryStream[] States = new MemoryStream[256];

        public uint StateHead = 0;
        public uint StateTail = 0;
        public uint Frames;
        public bool HasState;

        private readonly AudioProvider _audioProvider;
        private readonly VideoProvider _videoProvider;

        private IntPtr Window;

        public void SaveState()
        {
            using (var file = new FileStream("state01.sav", FileMode.Create, FileAccess.Write))
            {
                _nes.WriteState(file);
                _nes.Ppu.WriteState(file);
                _nes.Cartridge.WriteState(file);
            }
            HasState = true;
        }

        public void LoadState()
        {
            if (File.Exists("state01.sav"))
            {
                using (var file = new FileStream("state01.sav", FileMode.Open, FileAccess.Read))
                {
                    _nes.ReadState(file);
                    _nes.Ppu.ReadState(file);
                    _nes.Cartridge.ReadState(file);
                }
            }
        }

        public Main()
        {
            for (var i = 0; i < 256; i++)
            {
                States[i] = new MemoryStream();
            }

            _nes = new Cpu6502State();

            SDL.SDL_Init(SDL.SDL_INIT_AUDIO | SDL.SDL_INIT_VIDEO);

            Window = SDL.SDL_CreateWindow("Fami", 0, 0, WIDTH * 4, HEIGHT * 4,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            _videoProvider = new VideoProvider(Window, WIDTH, HEIGHT);
            _audioProvider = new AudioProvider();

            _videoProvider.Initialize();
            _audioProvider.Initialize();
        }

        public void EmulationThreadHandler()
        {
            try
            {
                while (_running)
                {
                    _threadSync.WaitOne();
                    RunFrame();
                    while (!_sync)
                    {
                        RunFrame();
                        _videoProvider.Render(_nes.Ppu.buffer);

                        _nes.GetSamplesSync(out var lsamples, out int lnsamp);

                        _audioProvider.AudioReady(lsamples);
                    }

                    if (_saveStatePending)
                    {
                        SaveState();
                        _saveStatePending = false;
                    }

                    if (_loadStatePending)
                    {
                        LoadState();
                        _loadStatePending = false;
                    }

                    _videoProvider.Render(_nes.Ppu.buffer);

                    _nes.GetSamplesSync(out var samples, out int nsamp);

                    _audioProvider.AudioReady(samples);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //Excepted = true;
            }
        }
        
        public void RunFrame()
        {
            _cyclesRan += CYCLES_PER_FRAME;
            _cyclesLeft += CYCLES_PER_FRAME;

            while (_cyclesLeft > 0)
            {
                _cyclesLeft -= (int)_nes.Step();
            }

            if (Frames % 4 == 0)
            {
                //_nes.Ppu.WriteState(States[StateHead]);
                StateHead++;
                if (StateHead > 255)
                {
                    StateHead = 0;
                }
            }

            Frames++;
        }

        public void Test()
        {
            _nes.Init();
            var cart = Cartridge.Load("nestest.nes", _nes);
            _nes.LoadCartridge(cart);
            _nes.Reset();
            _nes.PC = 0xC000;
            _nes.Execute();
        }

        public void Load(string rompath)
        {
            Cartridge cart = null;
            _nes.Init();
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
                    cart = Cartridge.Load(s, _nes);
                }
            }
            else
            {
                cart = Cartridge.Load(rompath, _nes);
            }

            _nes.LoadCartridge(cart);
            _nes.Reset();
        }

        public void Run()
        {
            _emulationThread = new Thread(EmulationThreadHandler);
            _emulationThread.Name = "Emulation Core";
            _emulationThread.Start();

            _running = true;

            var nextFrameAt = GetTime();
            double fpsEvalTimer = 0;

            void ResetTimers()
            {
                var time = GetTime();
                nextFrameAt = time;
                fpsEvalTimer = time;
            }

            ResetTimers();

            while (_running)
            {
                SDL_Event evt;
                while (SDL_PollEvent(out evt) != 0)
                {
                    switch (evt.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            _running = false;
                            break;
                        case SDL_EventType.SDL_KEYUP:
                        case SDL_EventType.SDL_KEYDOWN:
                            KeyEvent(evt.key);
                            break;

                        case SDL_EventType.SDL_DROPFILE:
                            var filename = Marshal.PtrToStringUTF8(evt.drop.file);
                            try
                            {
                                _romPath = filename;
                                _resetPending = true;
                            }
                            catch
                            {
                                //Log("An error occurred loading the dropped ROM file.");
                                return;
                            }

                            break;
                    }
                }

                if (_resetPending)
                {
                    Load(_romPath);
                    _resetPending = false;
                }


                if (_pause)
                {
                    if (_frameAdvance)
                    {
                        _frameAdvance = false;
                        RunFrame();
                    }
                }
                else
                {
                    double currentSec = GetTime();

                    // Reset time if behind schedule
                    if (currentSec - nextFrameAt >= NTSC_SECONDS_PER_FRAME)
                    {
                        double diff = currentSec - nextFrameAt;
                        Console.WriteLine("Can't keep up! Skipping " + (int)(diff * 1000) + " milliseconds");
                        nextFrameAt = currentSec;
                    }

                    if (currentSec >= nextFrameAt)
                    {
                        nextFrameAt += NTSC_SECONDS_PER_FRAME;

                        _threadSync.Set();
                    }

                    if (currentSec >= fpsEvalTimer)
                    {
                        double diff = currentSec - fpsEvalTimer + 1;
                        double frames = _cyclesRan / CYCLES_PER_FRAME;
                        _cyclesRan = 0;

                        //double mips = (double)Gba.Cpu.InstructionsRan / 1000000D;
                        //Gba.Cpu.InstructionsRan = 0;

                        // Use Math.Floor to truncate to 2 decimal places
                        _fps = Math.Floor((frames / diff) * 100) / 100;
                        //Mips = Math.Floor((mips / diff) * 100) / 100;
                        UpdateTitle();
                        //Seconds++;

                        fpsEvalTimer += 1;
                    }
                }


                //Draw();
            }

            _threadSync.Close();

        }

        private void UpdateTitle()
        {
            SDL_SetWindowTitle(
                _videoProvider.Window,
                "Fami - " + _fps + " fps" + _audioProvider.GetAudioSamplesInQueue() + " samples queued"
            );
        }

        private void KeyEvent(SDL_KeyboardEvent evtKey)
        {
            if (evtKey.type == SDL_EventType.SDL_KEYDOWN)
            {

                switch (evtKey.keysym.sym)
                {
                    case SDL_Keycode.SDLK_UP:
                        _controller1State |= 0x08U;
                        break;
                    case SDL_Keycode.SDLK_DOWN:
                        _controller1State |= 0x04U;
                        break;
                    case SDL_Keycode.SDLK_LEFT:
                        _controller1State |= 0x02U;
                        break;
                    case SDL_Keycode.SDLK_RIGHT:
                        _controller1State |= 0x01U;
                        break;
                    case SDL_Keycode.SDLK_RETURN:
                        _controller1State |= 0x10U;
                        break;
                    case SDL_Keycode.SDLK_LSHIFT:
                    case SDL_Keycode.SDLK_RSHIFT:
                        _controller1State |= 0x20U;
                        break;
                    case SDL_Keycode.SDLK_z:
                        _controller1State |= 0x80U;
                        break;
                    case SDL_Keycode.SDLK_x:
                        _controller1State |= 0x40U;
                        break;
                    case SDL_Keycode.SDLK_ESCAPE:
                        _nes.Reset();
                        break;
                    case SDL_Keycode.SDLK_SPACE:
                        _sync = false;
                        break;
                    case SDL_Keycode.SDLK_F2:
                        _saveStatePending = true;
                        break;
                    case SDL_Keycode.SDLK_F4:
                        _loadStatePending = true;
                        break;
                    case SDL_Keycode.SDLK_p:
                        _pause = !_pause;
                        Console.WriteLine(_pause ? "Paused" : "Resumed");
                        break;
                    case SDL_Keycode.SDLK_f:
                        if (_pause)
                        {
                            Console.WriteLine("Frame Advanced");
                            _frameAdvance = true;
                        }

                        break;
                }
            }

            if (evtKey.type == SDL_EventType.SDL_KEYUP)
            {

                switch (evtKey.keysym.sym)
                {
                    case SDL_Keycode.SDLK_UP:
                        _controller1State &= ~0x08U;
                        break;
                    case SDL_Keycode.SDLK_DOWN:
                        _controller1State &= ~0x04U;
                        break;
                    case SDL_Keycode.SDLK_LEFT:
                        _controller1State &= ~0x02U;
                        break;
                    case SDL_Keycode.SDLK_RIGHT:
                        _controller1State &= ~0x01U;
                        break;
                    case SDL_Keycode.SDLK_RETURN:
                        _controller1State &= ~0x10U;
                        break;
                    case SDL_Keycode.SDLK_LSHIFT:
                    case SDL_Keycode.SDLK_RSHIFT:
                        _controller1State &= ~0x20U;
                        break;
                    case SDL_Keycode.SDLK_z:
                        _controller1State &= ~0x80U;
                        break;
                    case SDL_Keycode.SDLK_x:
                        _controller1State &= ~0x40U;
                        break;
                    case SDL_Keycode.SDLK_SPACE:
                        _sync = true;
                        break;

                }
            }

            _nes.Controller[0] = _controller1State;

        }

        public static double GetTime()
        {
            return (double)SDL_GetPerformanceCounter() / (double)SDL_GetPerformanceFrequency();
        }
        
        public void Dispose()
        {
            _threadSync.Dispose();
            _audioProvider.Dispose();
            _videoProvider.Dispose();
            SDL_AudioQuit();
            SDL_VideoQuit();
            SDL_DestroyWindow(Window);
            SDL_Quit();
        }

    }
}