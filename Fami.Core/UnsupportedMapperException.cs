using System;

namespace Fami.Core
{
    public class UnsupportedMapperException : Exception
    {
        private readonly int _mapperId;

        public UnsupportedMapperException(int mapperId) : base($"Unsupported Mapper {mapperId}")
        {
            _mapperId = mapperId;
        }
    }
}