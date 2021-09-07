using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Fami.Core;
using Fami.Input;
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
        private bool _saveStatePending;
        private bool _loadStatePending;

        private bool _running;
        private string _romPath;
        private bool _resetPending;

        private bool _pause;
        private bool _frameAdvance;
        private bool _rewind;
        private bool _fastForward;
        private int stateSlot = 1;

        private readonly Cpu6502State _nes;
        private const int MAX_REWIND_BUFFER = 512;
        private readonly MemoryStream[] _rewindStateBuffer = new MemoryStream[MAX_REWIND_BUFFER];

        private int _rewindStateHead = 0;
        private int _rewindStateTail = 0;
        private uint _frames;
        private bool _hasState;

        private readonly AudioProvider _audioProvider;
        private readonly VideoProvider _videoProvider;
        private readonly InputProvider _inputProvider;

        private readonly IntPtr Window;

        private void SaveState(Stream stream)
        {
            _nes.WriteState(stream);
            _nes.Ppu.WriteState(stream);
            _nes.Cartridge.WriteState(stream);
        }

        private void LoadState(Stream stream)
        {
            _nes.ReadState(stream);
            _nes.Ppu.ReadState(stream);
            _nes.Cartridge.ReadState(stream);
        }

        private string GetStateSavePath()
        {
            return Path.Join(_romDirectory, $"{_romFilename}.s{stateSlot:00}");
        }

        public void SaveState()
        {
            using (var file = new FileStream(GetStateSavePath(), FileMode.Create, FileAccess.Write))
            {
                SaveState(file);
            }
            _hasState = true;
        }

        public void LoadState()
        {
            var path = GetStateSavePath();
            if (File.Exists(path))
            {
                using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    LoadState(file);
                }
            }
        }

        private int _scale = 4;

        public Main()
        {
            for (var i = 0; i < MAX_REWIND_BUFFER; i++)
            {
                _rewindStateBuffer[i] = new MemoryStream();
            }

            _nes = new Cpu6502State();

            SDL_Init(SDL_INIT_AUDIO | SDL_INIT_VIDEO | SDL_INIT_GAMECONTROLLER | SDL_INIT_JOYSTICK);

            Window = SDL_CreateWindow("Fami", 0, 0, WIDTH * _scale, HEIGHT * _scale,
                SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            _videoProvider = new VideoProvider(Window, WIDTH, HEIGHT);
            _audioProvider = new AudioProvider();
            _inputProvider = new InputProvider(ControllerEvent);

            _videoProvider.Initialize();
            _audioProvider.Initialize();
        }

        private void ControllerEvent(object sender, ControllerEventArgs args)
        {
            switch (args.Event)
            {
                case ControllerButtonEvent.UP:
                    if (((uint)args.Button & 0x100) == 0x100)
                    {
                        _nes.Controller[args.Player] &= ~(uint)args.Button;
                    }
                    else
                    {
                        switch (args.Button)
                        {
                            case ControllerButtonEnum.Rewind:
                                _rewind = false;
                                break;
                            case ControllerButtonEnum.FastForward:
                                _fastForward = false;
                                break;
                        }
                    }
                    break;
                case ControllerButtonEvent.DOWN:
                    if (((uint)args.Button & 0x100) == 0x100)
                    {
                        _nes.Controller[args.Player] |= (uint)args.Button;
                    }
                    else
                    {
                        switch (args.Button)
                        {
                            case ControllerButtonEnum.Rewind:
                                _rewind = true;
                                break;
                            case ControllerButtonEnum.FastForward:
                                _fastForward = true;
                                break;
                            case ControllerButtonEnum.SaveState:
                                _saveStatePending = true;
                                break;
                            case ControllerButtonEnum.LoadState:
                                _loadStatePending = true;
                                break;
                            case ControllerButtonEnum.NextState:
                                break;
                            case ControllerButtonEnum.PreviousState:
                                break;
                        }
                    }

                    break;
            }
        }

        public void EmulationThreadHandler()
        {
            try
            {
                while (_running)
                {
                    _threadSync.WaitOne();
                    RunFrame();

                    if (_rewind)
                    {
                        _rewindStateHead--;
                        if (_rewindStateHead < 0)
                        {
                            _rewindStateHead = MAX_REWIND_BUFFER - 1;
                        }
                        if (_rewindStateHead == _rewindStateTail)
                        {
                            _rewind = false;
                        }
                        _rewindStateBuffer[_rewindStateHead].Position = 0;
                        LoadState(_rewindStateBuffer[_rewindStateHead]);
                    }


                    while (_fastForward)
                    {
                        RunFrame();
                        _videoProvider.Render(_nes.Ppu.buffer);

                        _nes.GetSamplesSync(out var lsamples, out int lnsamp);

                        _audioProvider.AudioReady(lsamples);
                    }

                    if (!_rewind)
                    {
                        _rewindStateBuffer[_rewindStateHead].Position = 0;
                        SaveState(_rewindStateBuffer[_rewindStateHead]);

                        _rewindStateHead++;
                        if (_rewindStateHead >= MAX_REWIND_BUFFER)
                        {
                            _rewindStateHead = 0;
                        }

                        if (_rewindStateHead == _rewindStateTail)
                        {
                            _rewindStateTail = _rewindStateHead + 1;
                            if (_rewindStateTail >= MAX_REWIND_BUFFER)
                            {
                                _rewindStateTail = 0;
                            }
                        }
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


            _frames++;
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

        private string _romDirectory;
        private string _romFilename;

        public void Load(string rompath)
        {
            _romDirectory = "";
            _romFilename = "";

            Cartridge cart = null;
            _nes.Init();

            if (Path.GetExtension(rompath).ToLower() == ".zip")
            {
                using var zipFile = ZipFile.Open(rompath, ZipArchiveMode.Read);
                var nesFile = zipFile.Entries.FirstOrDefault(z => Path.GetExtension(z.Name).ToLower() == ".nes");
                if (nesFile == default)
                {
                    throw new Exception("No ROM found in archive!");
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

            _romDirectory = Path.GetDirectoryName(rompath);
            _romFilename = Path.GetFileNameWithoutExtension(rompath);

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
                        case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                        case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                            _inputProvider.HandleDeviceEvent(evt.cdevice);
                            break;
                        case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                            _inputProvider.HandleControllerEvent(evt.cbutton);
                            break;
                        case SDL_EventType.SDL_MOUSEMOTION:
                            // Get the position on screen where the Zapper is pointed at
                            _nes.gun_cycle = evt.motion.x / _scale;
                            _nes.gun_scanline = evt.motion.y / _scale;
                            break;
                        case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            //Console.WriteLine($"{evt.button.button}");

                            // The official Zapper has a trigger mechanism that ensures that the trigger switch is
                            // only activated for around 100ms. This value is arbitrary and was chosen based on 
                            // the ruder.nes test rom to have a trigger time of 5, which is the same value as seen
                            // in FCEUX. We decrement this value whenever we read from the controller port $4016/17
                            _nes.trigger_timeout = 200;

                            // Right-click emulates pointing the Zapper away from the screen while firing
                            if (evt.button.button == 3)
                            {
                                // Prevent the sensor from seeing anything for some duration.
                                // Maybe we can bind this to a key instead?
                                // This is an arbitrary value chosen by running the emulator in release mode
                                // and testing Duck Hunt. Firing off-screen should select the next game mode.
                                _nes.gun_offscreen_timeout = 4500;
                            }
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
                        default:
                            //Console.WriteLine(evt.type);
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
                if (!_inputProvider.HandleEvent(evtKey))
                {
                    switch (evtKey.keysym.sym)
                    {
                        //case SDL_Keycode.SDLK_UP:
                        //    _controller1State &= ~0x08U;
                        //    break;
                        //case SDL_Keycode.SDLK_DOWN:
                        //    _controller1State &= ~0x04U;
                        //    break;
                        //case SDL_Keycode.SDLK_LEFT:
                        //    _controller1State &= ~0x02U;
                        //    break;
                        //case SDL_Keycode.SDLK_RIGHT:
                        //    _controller1State &= ~0x01U;
                        //    break;
                        //case SDL_Keycode.SDLK_RETURN:
                        //    _controller1State &= ~0x10U;
                        //    break;
                        //case SDL_Keycode.SDLK_LSHIFT:
                        //case SDL_Keycode.SDLK_RSHIFT:
                        //    _controller1State &= ~0x20U;
                        //    break;
                        //case SDL_Keycode.SDLK_z:
                        //    _controller1State &= ~0x80U;
                        //    break;
                        //case SDL_Keycode.SDLK_x:
                        //    _controller1State &= ~0x40U;
                        //    break;
                            break;
                        case SDL_Keycode.SDLK_r:
                            _nes.Reset();
                            break;
                        case SDL_Keycode.SDLK_BACKSLASH:
                            _fastForward = true;
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
                        case SDL_Keycode.SDLK_BACKSPACE:
                            if (!_rewind)
                            {
                                Console.WriteLine("Rewinding...");
                                _rewind = true;
                            }
                            break;
                    }
                }
            }

            if (evtKey.type == SDL_EventType.SDL_KEYUP)
            {
                if (!_inputProvider.HandleEvent(evtKey))
                {
                    switch (evtKey.keysym.sym)
                    {
                        //case SDL_Keycode.SDLK_UP:
                        //    _controller1State &= ~0x08U;
                        //    break;
                        //case SDL_Keycode.SDLK_DOWN:
                        //    _controller1State &= ~0x04U;
                        //    break;
                        //case SDL_Keycode.SDLK_LEFT:
                        //    _controller1State &= ~0x02U;
                        //    break;
                        //case SDL_Keycode.SDLK_RIGHT:
                        //    _controller1State &= ~0x01U;
                        //    break;
                        //case SDL_Keycode.SDLK_RETURN:
                        //    _controller1State &= ~0x10U;
                        //    break;
                        //case SDL_Keycode.SDLK_LSHIFT:
                        //case SDL_Keycode.SDLK_RSHIFT:
                        //    _controller1State &= ~0x20U;
                        //    break;
                        //case SDL_Keycode.SDLK_z:
                        //    _controller1State &= ~0x80U;
                        //    break;
                        //case SDL_Keycode.SDLK_x:
                        //    _controller1State &= ~0x40U;
                        //    break;
                        case SDL_Keycode.SDLK_BACKSLASH:
                            _fastForward = false;
                            break;
                        case SDL_Keycode.SDLK_BACKSPACE:
                            _rewind = false;
                            break;
                    }
                }
            }
        }

        private static double GetTime()
        {
            return (double)SDL_GetPerformanceCounter() / (double)SDL_GetPerformanceFrequency();
        }

        public void Dispose()
        {
            // Wait for renderers to finish what they're doing
            // Not guaranteed, but it works
            Thread.Sleep(500);
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