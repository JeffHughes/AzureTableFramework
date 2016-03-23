﻿using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace AzureTableFramework.Core
{
    public static partial class Utils
    {
        public static string CharactersOnly(this string val)
        {
            return val.ToCharArray().Where(Char.IsLetterOrDigit).Aggregate("", (current, c) => current + c);
        }

        public static UInt32 Hash(Byte[] data)
        {
            return Hash(data, 0xc58f1a7b);
        }

        private const UInt32 m = 0x5bd1e995;
        private const Int32 r = 24;

        [StructLayout(LayoutKind.Explicit)]
        private struct BytetoUInt32Converter
        {
            [FieldOffset(0)]
            public Byte[] Bytes;

            [FieldOffset(0)]
            public UInt32[] UInts;
        }

        public static UInt32 Hash(Byte[] data, UInt32 seed)
        {
            Int32 length = data.Length;
            if (length == 0)
                return 0;
            UInt32 h = seed ^ (UInt32)length;
            Int32 currentIndex = 0;
            // array will be length of Bytes but contains Uints
            // therefore the currentIndex will jump with +1 while length will jump with +4
            UInt32[] hackArray = new BytetoUInt32Converter { Bytes = data }.UInts;
            while (length >= 4)
            {
                UInt32 k = hackArray[currentIndex++];
                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;
                length -= 4;
            }
            currentIndex *= 4; // fix the length
            switch (length)
            {
                case 3:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex++] << 8);
                    h ^= (UInt32)data[currentIndex] << 16;
                    h *= m;
                    break;

                case 2:
                    h ^= (UInt16)(data[currentIndex++] | data[currentIndex] << 8);
                    h *= m;
                    break;

                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;

                default:
                    break;
            }

            // Do a few final mixes of the hash to ensure the last few
            // bytes are well-incorporated.

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return h;
        }
    }
}