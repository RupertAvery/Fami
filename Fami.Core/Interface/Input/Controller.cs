using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace Fami.Core.Interface.Input
{
    public class Controller
    {
        private IntPtr gamepad;
        private IntPtr joystick;
        private int instanceId;

        private readonly Dictionary<int, ControllerButtonEnum> _buttonMapping = new Dictionary<int, ControllerButtonEnum>();
        
        public int ControllerIndex { get; set; }

        public Controller()
        {
            _buttonMapping.Add(11, ControllerButtonEnum.Up);
            _buttonMapping.Add(12, ControllerButtonEnum.Down);
            _buttonMapping.Add(13, ControllerButtonEnum.Left);
            _buttonMapping.Add(14, ControllerButtonEnum.Right);
            _buttonMapping.Add(3, ControllerButtonEnum.B);
            _buttonMapping.Add(2, ControllerButtonEnum.A);
            _buttonMapping.Add(1, ControllerButtonEnum.B);
            _buttonMapping.Add(0, ControllerButtonEnum.A);
            _buttonMapping.Add(4, ControllerButtonEnum.Select);
            _buttonMapping.Add(6, ControllerButtonEnum.Start);
        }

        public bool TryMap(int button, out ControllerButtonEnum mappedInput)
        {
            return _buttonMapping.TryGetValue(button, out mappedInput);
        }

        public static bool TryOpen(int deviceId, out Controller controller)
        {
            controller = new Controller();
            controller.gamepad = SDL_GameControllerOpen(deviceId);
            controller.joystick = SDL_GameControllerGetJoystick(controller.gamepad);
            controller.instanceId = SDL_JoystickInstanceID(controller.joystick);
            return true;
        }

        public void Close()
        {
            SDL_GameControllerClose(gamepad);
        }
    }
}