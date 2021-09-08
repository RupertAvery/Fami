using System.Collections.Generic;
using System.Windows.Forms;
using SDL2;

namespace Fami.UI
{
    public static class KeyCodeMappings
    {
        private static Dictionary<Keys, SDL.SDL_Keycode> mappings;

        public static bool TryGetKeyCode(Keys key, out SDL.SDL_Keycode code)
        {
            return mappings.TryGetValue(key, out code);
        }

        static KeyCodeMappings()
        {
            mappings = new Dictionary<Keys, SDL.SDL_Keycode>()
            {
                {Keys.A, SDL.SDL_Keycode.SDLK_a},
                {Keys.B, SDL.SDL_Keycode.SDLK_b},
                {Keys.C, SDL.SDL_Keycode.SDLK_c},
                {Keys.D, SDL.SDL_Keycode.SDLK_d},
                {Keys.E, SDL.SDL_Keycode.SDLK_e},
                {Keys.F, SDL.SDL_Keycode.SDLK_f},
                {Keys.G, SDL.SDL_Keycode.SDLK_g},
                {Keys.H, SDL.SDL_Keycode.SDLK_h},
                {Keys.I, SDL.SDL_Keycode.SDLK_i},
                {Keys.J, SDL.SDL_Keycode.SDLK_j},
                {Keys.K, SDL.SDL_Keycode.SDLK_k},
                {Keys.L, SDL.SDL_Keycode.SDLK_l},
                {Keys.M, SDL.SDL_Keycode.SDLK_m},
                {Keys.N, SDL.SDL_Keycode.SDLK_n},
                {Keys.O, SDL.SDL_Keycode.SDLK_o},
                {Keys.P, SDL.SDL_Keycode.SDLK_p},
                {Keys.Q, SDL.SDL_Keycode.SDLK_q},
                {Keys.R, SDL.SDL_Keycode.SDLK_r},
                {Keys.S, SDL.SDL_Keycode.SDLK_s},
                {Keys.T, SDL.SDL_Keycode.SDLK_t},
                {Keys.U, SDL.SDL_Keycode.SDLK_u},
                {Keys.V, SDL.SDL_Keycode.SDLK_v},
                {Keys.W, SDL.SDL_Keycode.SDLK_w},
                {Keys.X, SDL.SDL_Keycode.SDLK_x},
                {Keys.Y, SDL.SDL_Keycode.SDLK_y},
                {Keys.Z, SDL.SDL_Keycode.SDLK_z},
                {Keys.Up, SDL.SDL_Keycode.SDLK_UP},
                {Keys.Down, SDL.SDL_Keycode.SDLK_DOWN},
                {Keys.Left, SDL.SDL_Keycode.SDLK_LEFT},
                {Keys.Right, SDL.SDL_Keycode.SDLK_RIGHT},
                {Keys.LShiftKey, SDL.SDL_Keycode.SDLK_LSHIFT},
                {Keys.RShiftKey, SDL.SDL_Keycode.SDLK_RSHIFT},
                {Keys.LControlKey, SDL.SDL_Keycode.SDLK_LCTRL},
                {Keys.RControlKey, SDL.SDL_Keycode.SDLK_RCTRL},
                {Keys.Add, SDL.SDL_Keycode.SDLK_PLUS},
                {Keys.Subtract, SDL.SDL_Keycode.SDLK_MINUS},
                {Keys.NumPad0, SDL.SDL_Keycode.SDLK_KP_0},
                {Keys.NumPad1, SDL.SDL_Keycode.SDLK_KP_1},
                {Keys.NumPad2, SDL.SDL_Keycode.SDLK_KP_2},
                {Keys.NumPad3, SDL.SDL_Keycode.SDLK_KP_3},
                {Keys.NumPad4, SDL.SDL_Keycode.SDLK_KP_4},
                {Keys.NumPad5, SDL.SDL_Keycode.SDLK_KP_5},
                {Keys.NumPad6, SDL.SDL_Keycode.SDLK_KP_6},
                {Keys.NumPad7, SDL.SDL_Keycode.SDLK_KP_7},
                {Keys.NumPad8, SDL.SDL_Keycode.SDLK_KP_8},
                {Keys.NumPad9, SDL.SDL_Keycode.SDLK_KP_9},
                {Keys.D0, SDL.SDL_Keycode.SDLK_0},
                {Keys.D1, SDL.SDL_Keycode.SDLK_1},
                {Keys.D2, SDL.SDL_Keycode.SDLK_2},
                {Keys.D3, SDL.SDL_Keycode.SDLK_3},
                {Keys.D4, SDL.SDL_Keycode.SDLK_4},
                {Keys.D5, SDL.SDL_Keycode.SDLK_5},
                {Keys.D6, SDL.SDL_Keycode.SDLK_6},
                {Keys.D7, SDL.SDL_Keycode.SDLK_7},
                {Keys.D8, SDL.SDL_Keycode.SDLK_8},
                {Keys.D9, SDL.SDL_Keycode.SDLK_9},
                {Keys.Tab, SDL.SDL_Keycode.SDLK_TAB},
                {Keys.Escape, SDL.SDL_Keycode.SDLK_ESCAPE},
                {Keys.Back, SDL.SDL_Keycode.SDLK_BACKSPACE},
                {Keys.Return, SDL.SDL_Keycode.SDLK_RETURN},
                {Keys.PageUp, SDL.SDL_Keycode.SDLK_PAGEUP},
                {Keys.PageDown, SDL.SDL_Keycode.SDLK_PAGEDOWN},
                {Keys.End, SDL.SDL_Keycode.SDLK_END},
                {Keys.Delete, SDL.SDL_Keycode.SDLK_DELETE},
            };
        }
    }
}