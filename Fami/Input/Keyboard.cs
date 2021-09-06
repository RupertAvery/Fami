using System.Collections.Generic;
using static SDL2.SDL;

namespace Fami.Input
{
    public class Keyboard
    {
        private readonly Dictionary<SDL_Keycode, ControllerButtonEnum> _keyMapping = new Dictionary<SDL_Keycode, ControllerButtonEnum>();

        public Keyboard()
        {
            _keyMapping.Add(SDL_Keycode.SDLK_UP, ControllerButtonEnum.Up);
            _keyMapping.Add(SDL_Keycode.SDLK_DOWN, ControllerButtonEnum.Down);
            _keyMapping.Add(SDL_Keycode.SDLK_LEFT, ControllerButtonEnum.Left);
            _keyMapping.Add(SDL_Keycode.SDLK_RIGHT, ControllerButtonEnum.Right);
            _keyMapping.Add(SDL_Keycode.SDLK_v, ControllerButtonEnum.Start);
            _keyMapping.Add(SDL_Keycode.SDLK_c, ControllerButtonEnum.Select);
            _keyMapping.Add(SDL_Keycode.SDLK_z, ControllerButtonEnum.A);
            _keyMapping.Add(SDL_Keycode.SDLK_x, ControllerButtonEnum.B);
        }

        public bool TryMap(SDL_Keycode keyCode, out ControllerButtonEnum mappedInput)
        {
            return _keyMapping.TryGetValue(keyCode, out mappedInput);
        }
    }
}