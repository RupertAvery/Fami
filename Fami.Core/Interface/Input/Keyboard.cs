using System.Collections.Generic;
using System.Linq;
using static SDL2.SDL;

namespace Fami.Core.Interface.Input
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

        public bool TrySetMap(SDL_Keycode key, ControllerButtonEnum target)
        {
            if (TryGetMap(target, out var oldkey))
            {
                _keyMapping.Remove(oldkey.Value);
            }
            _keyMapping[key] = target;
            return true;
        }

        public IEnumerable<KeyValuePair<SDL_Keycode, ControllerButtonEnum>> GetMappings()
        {
            return _keyMapping;
        }

        public bool TryGetMap(ControllerButtonEnum button, out SDL_Keycode? key)
        {
            key = null;

            if (_keyMapping.ContainsValue(button))
            {
                var mapping = _keyMapping.First(m => m.Value == button);
                key = mapping.Key;
                return true;
            }

            return false;
        }

    }
}