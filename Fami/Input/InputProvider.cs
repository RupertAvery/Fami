using System;
using System.Collections.Generic;
using static SDL2.SDL;

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

        public bool HandleEvent(SDL_MouseMotionEvent motionEvent)
        {
            // Get the position on screen where the Zapper is pointed at
            ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.AIM, ZapperX = motionEvent.x, ZapperY = motionEvent.y });
            return true;
        }

        public bool HandleEvent(SDL_MouseButtonEvent buttonEvent)
        {
            ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.TRIGGER });
            return true;
        }

        public bool HandleEvent(SDL_KeyboardEvent keyboardEvent)
        {
            var handled = false;

            switch (keyboardEvent.type)
            {
                case SDL_EventType.SDL_KEYUP:
                    {
                        if (_keyboard.TryMap(keyboardEvent.keysym.sym, out ControllerButtonEnum mappedInput))
                        {
                            ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.BUTTON_UP, Player = 0, Button = mappedInput });
                            handled = true;
                        }
                        break;
                    }
                case SDL_EventType.SDL_KEYDOWN:
                    {
                        if (_keyboard.TryMap(keyboardEvent.keysym.sym, out ControllerButtonEnum mappedInput))
                        {
                            ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.BUTTON_DOWN, Player = 0, Button = mappedInput });
                            handled = true;
                        }
                        break;
                    }
            }
            return handled;
        }

        public bool HandleControllerEvent(SDL_ControllerButtonEvent buttonEvent)
        {
            var handled = false;
            switch (buttonEvent.type)
            {
                case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out ControllerButtonEnum mappedInput))
                            {
                                ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.BUTTON_UP, Player = controller.ControllerIndex, Button = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

                case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                    Console.WriteLine($"{buttonEvent.type} {buttonEvent.button} {buttonEvent.which}");
                    {
                        if (_deviceControllerMapping.TryGetValue(buttonEvent.which, out var controller))
                        {
                            if (controller.TryMap(buttonEvent.button, out ControllerButtonEnum mappedInput))
                            {
                                ControllerEvent?.Invoke(null, new ControllerEventArgs() { EventType = ControllerEventType.BUTTON_DOWN, Player = controller.ControllerIndex, Button = mappedInput });
                                handled = true;
                            }
                        }
                    }
                    break;

            }

            return handled;
        }

        public void HandleDeviceEvent(SDL_ControllerDeviceEvent deviceEvent)
        {
            //Console.WriteLine($"{deviceEvent.type} {deviceEvent.which}");

            switch (deviceEvent.type)
            {
                case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    {
                        if (Controller.TryOpen(deviceEvent.which, out var controller))
                        {
                            _deviceControllerMapping.Add(deviceEvent.which, controller);
                        }

                    }
                    break;
                case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
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