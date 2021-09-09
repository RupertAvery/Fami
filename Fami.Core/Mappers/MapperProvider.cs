using System;
using System.Collections.Generic;
using System.Text;

namespace Fami.Core.Mappers
{
    public class MapperProvider
    {
        public static BaseMapper Resolve(Cartridge cart, int mapperId)
        {
            return mapperId switch
            {
                0 => new NROM(cart),
                1 => new MMC1(cart),
                2 => new UxROM(cart),
                3 => new CNROM(cart),
                4 => new MMC3(cart),
                7 => new AxROM(cart),
                64 => new RAMBO1(cart),
                87 => new Mapper87(cart),
                _ => throw new UnsupportedMapperException(mapperId)
            };

        }
    }
}
