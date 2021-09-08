using System;
using System.IO;

namespace Fami.Core.Interface
{
    public class Playback
    {
        private const int MAX_REWIND_BUFFER = 512;
        private readonly MemoryStream[] _rewindStateBuffer = new MemoryStream[MAX_REWIND_BUFFER];

        private int _rewindStateHead = 0;
        private int _rewindStateTail = 0;
        
        public Action<Stream> SaveState { get; set; }
        public Action<Stream> LoadState { get; set; }

        public Playback()
        {
            for (var i = 0; i < MAX_REWIND_BUFFER; i++)
            {
                _rewindStateBuffer[i] = new MemoryStream();
            }
        }

        public void PerFrame(ref bool rewind)
        {
            switch (rewind)
            {
                case true:
                {
                    _rewindStateHead--;
                    if (_rewindStateHead < 0)
                    {
                        _rewindStateHead = MAX_REWIND_BUFFER - 1;
                    }
                    if (_rewindStateHead == _rewindStateTail)
                    {
                        rewind = false;
                    }
                    _rewindStateBuffer[_rewindStateHead].Position = 0;
                    LoadState(_rewindStateBuffer[_rewindStateHead]);
                    break;
                }
                case false:
                {
                    _rewindStateBuffer[_rewindStateHead].Position = 0;
                    SaveState(_rewindStateBuffer[_rewindStateHead]);

                    _rewindStateHead++;

                    if (_rewindStateHead >= MAX_REWIND_BUFFER)
                    {
                        _rewindStateHead = 0;
                    }
                    else if (_rewindStateHead == _rewindStateTail)
                    {
                        _rewindStateTail = _rewindStateHead + 1;
                        if (_rewindStateTail >= MAX_REWIND_BUFFER)
                        {
                            _rewindStateTail = 0;
                        }
                    }

                    break;
                }
            }
        }
    }
}