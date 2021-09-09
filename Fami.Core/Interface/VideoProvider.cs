using System;
using System.IO;
using static SDL2.SDL_ttf;
using static SDL2.SDL;

namespace Fami.Core.Interface
{
    public class VideoProvider : IDisposable
    {
        private const int WIDTH = 256;
        private const int HEIGHT = 240;


        private const double RATIO_4x3 = 4 / 3d;
        private const double RATIO_1x1 = 1;
        private const double RATIO_8x7 = 8 / 7d;

        private uint[] _displayBuf;
        public IntPtr Window;
        private IntPtr _renderer;
        private IntPtr _texture;
        private IntPtr _font;

        public VideoProvider(IntPtr window)
        {
            Window = window;
            _displayBuf = new uint[WIDTH * HEIGHT];
            _font = TTF_OpenFont(Path.Combine("Fonts", "Modeseven.ttf"), 24);

            //string fontsfolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            //_font = TTF_OpenFont(Path.Combine(fontsfolder, "LUCON.TTF"), 24);

        }

        public DisplayMode DisplayMode { get; set; }
        public bool IntegerScaling { get; set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int TextWidth { get; private set; }
        public int TextHeight { get; private set; }

        public int CanvasWidth { get; private set; }
        public int CanvasHeight { get; private set; }

        public double Ratio { get; private set; }

        public (int X, int Y) ToScreenCoordinates(int x, int y)
        {
            var sx = (x - (CanvasWidth - Width) / 2) / Ratio;
            var sy = y / Ratio;

            return ((int)sx, (int)sy);
        }

        public void Initialize()
        {
            //SDL_SetWindowPosition(Window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            _renderer = SDL_CreateRenderer(Window, -1,
                SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            _texture = SDL_CreateTexture(_renderer, SDL_PIXELFORMAT_ABGR8888,
                (int)SDL_TextureAccess.SDL_TEXTUREACCESS_STREAMING, WIDTH, HEIGHT);

            SDL_GetWindowSize(Window, out int w, out int h);

            CanvasWidth = w;
            CanvasHeight = h;

            Ratio = Math.Min((double)h / (double)HEIGHT, (double)w / (double)WIDTH);

            double aspect_ratio = RATIO_1x1;

            switch (DisplayMode)
            {
                case DisplayMode.Ratio1x1:
                    {
                        int fillWidth;
                        int fillHeight;
                        if (IntegerScaling)
                        {
                            fillWidth = ((int)(Ratio * WIDTH) / WIDTH) * WIDTH;
                            fillHeight = ((int)(Ratio * HEIGHT) / HEIGHT) * HEIGHT;
                        }
                        else
                        {
                            fillWidth = (int)(Ratio * WIDTH);
                            fillHeight = (int)(Ratio * HEIGHT);
                        }

                        fillWidth = (int)(fillWidth * aspect_ratio);

                        Width = fillWidth;
                        Height = fillHeight;
                        X = (int)((w - fillWidth) / 2);
                        Y = (int)((h - fillHeight) / 2);
                        break;
                    }
                case DisplayMode.Stretched:
                    Width = w;
                    Height = h;
                    X = 0;
                    Y = 0;
                    break;
            }

        }

        public void Clear()
        {
            SDL_RenderClear(_renderer);
            SDL_RenderPresent(_renderer);
            SDL_UpdateWindowSurface(Window);
        }

        public unsafe void Render(uint[] buffer)
        {
            for (var y = 0; y < HEIGHT; y++)
            {
                for (var x = 0; x < WIDTH; x++)
                {
                    var p = buffer[x + y * WIDTH];
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
                    _displayBuf[x + y * WIDTH] = p;
                }
            }

            //CopyPixels(Gba.Ppu.Renderer.ScreenFront, DisplayBuf, WIDTH * HEIGHT, ColorCorrection);
            fixed (void* ptr = _displayBuf)
                SDL_UpdateTexture(_texture, IntPtr.Zero, (IntPtr)ptr, WIDTH * 4);

            SDL_Rect dest = new();

            switch (DisplayMode)
            {
                case DisplayMode.Ratio1x1:
                    dest.w = Width;
                    dest.h = Height;
                    dest.x = X;
                    dest.y = Y;
                    break;
                case DisplayMode.Stretched:
                    dest.w = Width;
                    dest.h = Height;
                    dest.x = 0;
                    dest.y = 0;
                    break;
            }

            SDL_RenderClear(_renderer);
            SDL_RenderCopy(_renderer, _texture, IntPtr.Zero, ref dest);

            if (_messageTimeout > 0)
            {
                SDL_Rect textLocation = new SDL_Rect() { x = 0, y = dest.h - TextHeight, h = TextHeight, w = TextWidth };
                if (_messageTimeout < 50)
                {
                    SDL_SetTextureAlphaMod(textTexture, (byte)(((_messageTimeout) / 50f) * 255));
                }

                var result = SDL_RenderCopy(_renderer, textTexture, IntPtr.Zero, ref textLocation);
                //var result = SDL_BlitSurface(pTexture, IntPtr.Zero, _texture, ref textLocation);
                if (result != 0)
                {
                    Console.WriteLine(SDL_GetError());
                }

                _messageTimeout--;
            }
            else
            {
                CleanMessage();
            }

            SDL_RenderPresent(_renderer);
            SDL_UpdateWindowSurface(Window);
        }

        private string _message;
        private int _messageTimeout;
        private IntPtr textSurface = IntPtr.Zero;
        private IntPtr textTexture = IntPtr.Zero;

        private void CleanMessage()
        {
            if (textSurface != IntPtr.Zero)
            {
                SDL_FreeSurface(textSurface);
                SDL_DestroyTexture(textTexture);
                textSurface = IntPtr.Zero;
                textTexture = IntPtr.Zero;
            }
        }

        public void SetMessage(string message)
        {
            if (textSurface == IntPtr.Zero)
            {
                _message = message;
                _messageTimeout = 200;

                SDL_Color foregroundColor = new SDL_Color() {a = 0xFF, r = 0xFF, g = 0xFF, b = 0xFF};
                SDL_Color backgroundColor = new SDL_Color() {a = 0x00, r = 0x00, g = 0x00, b = 0x00};

                textSurface = TTF_RenderText_Blended(_font, _message, foregroundColor);
                textTexture = SDL_CreateTextureFromSurface(_renderer, textSurface);
                SDL_QueryTexture(textTexture, out uint format, out int access, out int width, out int height);
                TextHeight = height;
                TextWidth = width;
                SDL_SetTextureAlphaMod(textTexture, 255);
            }
        }


        public void Destroy()
        {
            SDL_DestroyTexture(_texture);
            SDL_DestroyRenderer(_renderer);
        }

        public void Dispose()
        {
            TTF_CloseFont(_font);
            Destroy();
        }

        public void Resize(int width, int height)
        {
            //Destroy();
            //SDL_SetWindowSize(Window, width, height);
            //Initialize();
        }
    }
}