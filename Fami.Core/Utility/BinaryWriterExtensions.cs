using System;
using System.IO;

namespace Fami.Core.Utility
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, uint[] values, int offset, int length)
        {
            for (var i = offset; i < length; i++)
            {
                writer.Write(values[i]);
            }
        }

        public static uint[] ReadUInt32Array(this BinaryReader writer, int length)
        {
            var result = new uint[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = writer.ReadUInt32();
            }
            return result;
        }

        public static void Write2DArray(this BinaryWriter writer, byte[,] array, int outer, int inner)
        {
            var buffer = new byte[outer * inner];
            Buffer.BlockCopy(array, 0, buffer, 0, outer * inner);
            writer.Write(buffer, 0, buffer.Length);
        }

        public static void Write2DArraySlow(this BinaryWriter writer, byte[,] array, int outer, int inner)
        {
            for (var i = 0; i < outer; i++)
            {
                for (var j = 0; j < inner; j++)
                {
                    writer.Write(array[i, j]);
                }
            }
        }

        public static void Read2DArray(this BinaryReader writer, byte[,] array, int outer, int inner)
        {
            var buffer = new byte[outer * inner];
            writer.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, array, 0, outer * inner);
        }

        public static byte[,] Read2DArray(this BinaryReader writer, int outer, int inner)
        {
            var result = new byte[outer, inner];
            var buffer = new byte[outer * inner];
            writer.Read(buffer, 0, buffer.Length);
            Buffer.BlockCopy(buffer, 0, result, 0, outer * inner);
            return result;
        }

        public static byte[,] Read2DArraySlow(this BinaryReader reader, int outer, int inner)
        {
            var result = new byte[outer, inner];
            for (var i = 0; i < outer; i++)
            {
                for (var j = 0; j < inner; j++)
                {
                    result[i, j] = reader.ReadByte();
                }
            }
            return result;
        }

    }
}