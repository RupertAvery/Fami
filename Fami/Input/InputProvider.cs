using System;
using System.Collections.Generic;
using SDL2;

namespace Fami.Input
{
    public class InputProvider
    {
        private readonly Dictionary<int, Controller> _deviceControllerMapping = new Dictionary<int, Controller>();

        private readonly Keyboard _keyboard = new Keyboard();

        public ControllerEvent ControllerEvent { get; set; }

        public InputProvider(ControllerEvent controllerEvent)
        {
            ControllerEvent = controllerEvent;
        }

        public bool HandleEvent(SDL.SDL_KeyboardEvent keyboardEvent)
        {
            var handled = false;

            switch (keyboardEvent.type)
            {
                case SDL.SDL_EventType.SDL_KEYUP:
                    {
                        if (_keyboard.TryMap(keyboardEvent.keysym.sym, out ControllerButtonEnum mappedInput))
                        {
                            ControllerEvent?.Invoke(null, new ControllerEventArgs() { Event = ControllerButtonEvent.UP, Player = 0, Button = mappedInput });
                            handled = true;
                        }
                        break;
                    }
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    {
                        if (_keyboard.TryMap(keyboardEvent.keysym.sym, out ControllerButtonEnum mappedInput))
                        {
                            ControllerEvent?.Invoke(null, new ControllerEventArgs() { Event = ControllerButtonEvent.DOWN, Player = 0, Button = mappedInput });
                            handled = true;
                        }
                        break;
                    }
            }
            return handled;
        }

        public bool HandleControllerEvent(SDL.SDL_ControllerButtonEvent buttonEvent)
        {
            var handled = false;
            switch (buttonEvent.type)
            {
                case SDL.SDL_EventType.SDL_CONTROLLERBUTTONUP:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out ControllerButtonEnum mappedInput))
                            {
                                ControllerEvent?.Invoke(null, new ControllerEventArgs() { Event = ControllerButtonEvent.UP, Player = controller.ControllerIndex, Button = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

                case SDL.SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out ControllerButtonEnum mappedInput))
                            {
                                ControllerEvent?.Invoke(null, new ControllerEventArgs() { Event = ControllerButtonEvent.DOWN, Player = controller.ControllerIndex, Button = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

            }

            return handled;
        }

        public void HandleDeviceEvent(SDL.SDL_ControllerDeviceEvent deviceEvent)
        {
            //Console.WriteLine($"{deviceEvent.type} {deviceEvent.which}");

            switch (deviceEvent.type)
            {
                case SDL.SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    {
                        if (Controller.TryOpen(deviceEvent.which, out var controller))
                        {
                            _deviceControllerMapping.Add(deviceEvent.which, controller);
                        }

                    }
                    break;
                case SDL.SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                    {
                        if (_deviceControllerMapping.TryGetValue(deviceEvent.which, out var controller))
                        {
                            controller.Close();
                            _deviceControllerMapping.Remove(deviceEvent.which);
                        }
                    }
                    break;
            }
        }
    }
}