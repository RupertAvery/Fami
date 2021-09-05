using System;
using SDL2;

namespace Fami
{
    public class VideoProvider : IDisposable
    {
        private const double RATIO_4x3 = 4 / 3d;
        private const double RATIO_1x1 = 1;
        private const double RATIO_8x7 = 8 / 7d;

        public int Width;
        public int Height;

        private uint[] _displayBuf;
        public IntPtr Window;
        private IntPtr _renderer;
        private IntPtr _texture;

        public VideoProvider(IntPtr window, int width, int height)
        {
            Window = window;
            Width = width;
            Height = height;
        }

        public DisplayMode DisplayMode { get; set; }
        public bool IntegerScaling { get; set; }

        public void Initialize()
        {
            _displayBuf = new uint[Width * Height];
            SDL.SDL_SetWindowPosition(Window, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED);
            _renderer = SDL.SDL_CreateRenderer(Window, -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            _texture = SDL.SDL_CreateTexture(_renderer, SDL.SDL_PIXELFORMAT_ABGR8888,
                (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, Width, Height);
        }


        public void Fill(uint a, uint r, uint g, uint b)
        {
            for (uint i = 0; i < Width * Height; i++)
            {
                _displayBuf[i] = a << 24 | b << 16 | g << 8 | r;
            }
        }

        public unsafe void Render(uint[] buffer)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    var p = buffer[x + y * Width];
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
                    _displayBuf[x + y * Width] = p;
                }
            }

            //CopyPixels(Gba.Ppu.Renderer.ScreenFront, DisplayBuf, WIDTH * HEIGHT, ColorCorrection);
            fixed (void* ptr = _displayBuf)
                SDL.SDL_UpdateTexture(_texture, IntPtr.Zero, (IntPtr)ptr, Width * 4);


            SDL.SDL_Rect dest = new();

            SDL.SDL_GetWindowSize(Window, out int w, out int h);

            double ratio = Math.Min((double)h / (double)Height, (double)w / (double)Width);

            double aspect_ratio = RATIO_1x1;

            switch (DisplayMode)
            {
                case DisplayMode.Ratio1x1:
                    {
                        int fillWidth;
                        int fillHeight;
                        if (IntegerScaling)
                        {
                            fillWidth = ((int)(ratio * Width) / Width) * Width;
                            fillHeight = ((int)(ratio * Height) / Height) * Height;
                        }
                        else
                        {
                            fillWidth = (int)(ratio * Width);
                            fillHeight = (int)(ratio * Height);
                        }

                        fillWidth = (int)(fillWidth * aspect_ratio);

                        dest.w = fillWidth;
                        dest.h = fillHeight;
                        dest.x = (int)((w - fillWidth) / 2);
                        dest.y = (int)((h - fillHeight) / 2);
                        break;
                    }
                case DisplayMode.Stretched:
                    dest.w = w;
                    dest.h = h;
                    dest.x = 0;
                    dest.y = 0;
                    break;
            }

            SDL.SDL_RenderClear(_renderer);
            SDL.SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, ref dest);
            SDL.SDL_RenderPresent(_renderer);
            //SDL_UpdateWindowSurface(Window);
        }


        public void Dispose()
        {
            SDL.SDL_DestroyTexture(_texture);
            SDL.SDL_DestroyRenderer(_renderer);
        }

    }
}