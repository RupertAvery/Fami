using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Fami.Core.CPU;
using Fami.Core.Interface.Input;
using static SDL2.SDL;

namespace Fami.Core.Interface
{
    public class Main : IDisposable
    {
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

        private bool _paused;
        private bool _frameAdvance;
        private bool _rewind;
        private bool _fastForward;
        private int _stateSlot = 1;

        private MC6502State _nes;

        private uint _frames;
        private bool _hasState;

        public Action<int> StateChanged { get; set; }
        private AudioProvider _audioProvider;
        private VideoProvider _videoProvider;

        private IntPtr Window;

        public InputProvider InputProvider { get; private set; }

        private Playback _playback;

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
            return Path.Combine(_romDirectory, $"{_romFilename}.s{_stateSlot:00}");
        }

        public void SaveState()
        {
            using (var file = new FileStream(GetStateSavePath(), FileMode.Create, FileAccess.Write))
            {
                SaveState(file);
                _videoProvider.SetMessage($"Saved State #{_stateSlot}");
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
                    _videoProvider.SetMessage($"Loaded State #{_stateSlot}");
                }
            }
        }

        private int _scale = 4;

        public void SetMapping(ControllerButtonEnum o)
        {
            InputProvider.SetMapping(o);
        }

        public void Initialize(IMainInterface form)
        {

            form.OnHostResize = () =>
            {
                _videoProvider.Destroy();
                _videoProvider.Initialize();
                _videoProvider.Clear();
            };

            form.ChangeSlot = i =>
            {
                _stateSlot = i;
                _videoProvider.SetMessage($"Slot #{_stateSlot}");
            };

            form.SaveState = SaveState;
            form.LoadState = LoadState;
            //form.ResizeWindow = (width, height) =>
            //{
            //    _paused = true;
            //    Thread.Sleep(200);
            //    //_videoProvider.Resize(width, height);
            //    _paused = false;
            //};
            form.SetMapping = o =>
            {
                InputProvider.SetMapping(o);
            };

            form.LoadRom = s =>
            {
                try
                {
                    LoadInternal(s);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            };

            _playback = new Playback()
            {
                LoadState = LoadState,
                SaveState = SaveState
            };

            _nes = new MC6502State();

            _nes.Init();

            SDL_Init(SDL_INIT_AUDIO | SDL_INIT_VIDEO | SDL_INIT_GAMECONTROLLER | SDL_INIT_JOYSTICK);

            SDL2.SDL_ttf.TTF_Init();

            Window = SDL_CreateWindowFrom(form.Handle);

            //Window = SDL_CreateWindow("Fami", 0, 0, WIDTH * _scale, HEIGHT * _scale,
            //    SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_RESIZABLE );

            _videoProvider = new VideoProvider(Window);
            _audioProvider = new AudioProvider();
            InputProvider = new InputProvider(ControllerEvent);

            _videoProvider.Initialize();
            _videoProvider.Clear();
            _audioProvider.Initialize();

            form.ChangeSlot(1);
        }

        private bool _justLoaded;

        private void LoadInternal(string path)
        {
            _paused = true;
            // Ensure frame rendering has completed so we don't overwrite anything while the emulation thread is busy
            Thread.Sleep(200);
            Load(path);
            _justLoaded = true;
            // prevent the first click fron firing when loading light gun games
            _paused = false;
        }

        public Main()
        {
        }

        private void ControllerEvent(object sender, ControllerEventArgs args)
        {
            switch (args.EventType)
            {
                case ControllerEventType.BUTTON_UP:
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
                case ControllerEventType.BUTTON_DOWN:
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
            void Render()
            {
                _videoProvider.Render(_nes.Ppu.buffer);

                _nes.GetSamplesSync(out var lsamples, out int lnsamp);

                _audioProvider.AudioReady(lsamples);
            }

            while (_running)
            {
                try
                {
                    _threadSync.WaitOne();

                    if (_resetPending)
                    {
                        _nes.Reset();
                        _resetPending = false;
                    }

                    RunFrame();

                    while (_fastForward)
                    {
                        RunFrame();
                        Render();
                    }

                    _playback.PerFrame(ref _rewind);

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

                    Render();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    //Excepted = true;
                }
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

            _videoProvider.SetMessage($"Loaded {_romFilename}");

            if (_emulationThread == null)
            {
                _emulationThread = new Thread(EmulationThreadHandler);
                _emulationThread.Name = "Emulation Core";
                _emulationThread.Start();
            }
        }


        public void Run()
        {

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
                try
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
                                InputProvider.HandleDeviceEvent(evt.cdevice);
                                break;
                            case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                            case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                                InputProvider.HandleControllerEvent(evt.cbutton);
                                break;
                            case SDL_EventType.SDL_MOUSEMOTION:
                                // Get the position on screen where the Zapper is pointed at
                                (_nes.gun_cycle, _nes.gun_scanline) = _videoProvider.ToScreenCoordinates(evt.motion.x, evt.motion.y);
                                break;
                            case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                                // prevent the first click fron firing when loading light gun games
                                if (_justLoaded)
                                {
                                    _justLoaded = false;
                                    break;
                                }

                                //Console.WriteLine($"{evt.button.button}");

                                _nes.TriggerLightGun(evt.button.button == 3);

                                break;

                            case SDL_EventType.SDL_DROPFILE:
                                var filename = Marshal.PtrToStringAnsi(evt.drop.file);
                                LoadInternal(filename);

                                break;
                            default:
                                //Console.WriteLine(evt.type);
                                break;
                        }
                    }


                    if (_paused)
                    {
                        if (_frameAdvance)
                        {
                            _frameAdvance = false;
                            _threadSync.Set();
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
                            // UpdateTitle();
                            //Seconds++;

                            fpsEvalTimer += 1;
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
                if (!InputProvider.HandleEvent(evtKey))
                {
                    switch (evtKey.keysym.sym)
                    {
                        case SDL_Keycode.SDLK_r:
                            _resetPending = true;
                            _videoProvider.SetMessage("Reset");
                            break;

                        case SDL_Keycode.SDLK_BACKSLASH:
                            _fastForward = true;
                            _videoProvider.SetMessage("Fast Forward");
                            break;

                        case SDL_Keycode.SDLK_F2:
                            _saveStatePending = true;
                            break;

                        case SDL_Keycode.SDLK_F4:
                            _loadStatePending = true;
                            break;

                        case { } keyCode when keyCode >= SDL_Keycode.SDLK_0 && keyCode <= SDL_Keycode.SDLK_9:
                            if (keyCode == SDL_Keycode.SDLK_0)
                            {
                                _stateSlot = 10;
                            }
                            else
                            {
                                _stateSlot = keyCode - SDL_Keycode.SDLK_0;
                            }
                            _videoProvider.SetMessage($"Slot #{_stateSlot}");
                            StateChanged?.Invoke(_stateSlot);
                            break;

                        case SDL_Keycode.SDLK_p:
                            _paused = !_paused;
                            _videoProvider.SetMessage(_paused ? "Paused" : "Resumed");
                            break;
                        case SDL_Keycode.SDLK_f:
                            if (_paused)
                            {
                                _videoProvider.SetMessage("Frame Advanced");
                                _frameAdvance = true;
                            }
                            break;
                        case SDL_Keycode.SDLK_BACKSPACE:
                            if (!_rewind)
                            {
                                _videoProvider.SetMessage("Rewinding...");
                                _rewind = true;
                                if (_paused)
                                {
                                    _frameAdvance = true;
                                }
                            }
                            break;
                    }
                }
            }

            if (evtKey.type == SDL_EventType.SDL_KEYUP)
            {
                if (!InputProvider.HandleEvent(evtKey))
                {
                    switch (evtKey.keysym.sym)
                    {
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
            SDL2.SDL_ttf.TTF_Quit();
            SDL_AudioQuit();
            SDL_VideoQuit();
            SDL_DestroyWindow(Window);
            SDL_Quit();
        }

        public void Redraw()
        {
            if (_emulationThread == null)
            {
                _videoProvider.Clear();
            }
        }
    }
}