using System;
using System.Runtime.InteropServices;
using static SDL2.SDL;

namespace Fami.Core.Interface
{
    public class AudioProvider : IDisposable
    {
        private const uint AUDIO_SAMPLE_FULL_THRESHOLD = 2048;
        private const int SAMPLES_PER_CALLBACK = 32;  // 735 = 44100 samples / 60 fps // 367.5? 1470

        private readonly IntPtr _audioTempBufPtr = Marshal.AllocHGlobal(16384);
        private SDL_AudioSpec _want, _have;
        private uint _audioDevice;

        public void Initialize()
        {
            _want.channels = 2;
            _want.freq = 44100;
            _want.samples = SAMPLES_PER_CALLBACK;
            _want.format = AUDIO_S16LSB;
            _audioDevice = SDL_OpenAudioDevice(null, 0, ref _want, out _have, (int)SDL_AUDIO_ALLOW_FORMAT_CHANGE);
            SDL_PauseAudioDevice(_audioDevice, 0);
        }

        public void AudioReady(short[] data)
        {
            // Don't queue audio if too much is in buffer
            if (GetAudioSamplesInQueue() < AUDIO_SAMPLE_FULL_THRESHOLD)
            {
                int bytes = sizeof(short) * data.Length;

                Marshal.Copy(data, 0, _audioTempBufPtr, data.Length);

                // Console.WriteLine("Outputting samples to SDL");

                SDL_QueueAudio(_audioDevice, _audioTempBufPtr, (uint)bytes);
            }
        }

        public uint GetAudioSamplesInQueue()
        {
            return SDL_GetQueuedAudioSize(_audioDevice) / sizeof(short);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_audioTempBufPtr);
        }
    }
}