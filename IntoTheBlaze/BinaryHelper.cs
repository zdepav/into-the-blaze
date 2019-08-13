using System;
using System.Collections.Generic;

namespace IntoTheBlaze {

    internal class BinaryHelper {
        
        private static readonly uint[] uintBits;
        private static readonly ulong[] ulongBits;

        static BinaryHelper() {
            uintBits = new uint[32];
            for (uint i = 0, b = 1; i < 32; ++i, b <<= 1) uintBits[i] = b;
            ulongBits = new ulong[64];
            for (ulong i = 0, b = 1; i < 64; ++i, b <<= 1) ulongBits[i] = b;
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 1</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 7</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static byte WithBit(byte i, int bit) {
            if (bit < 0 || bit > 7) throw new ArgumentOutOfRangeException(nameof(bit));
            return (byte)(i | uintBits[bit]);
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 1</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 15</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static ushort WithBit(ushort i, int bit) {
            if (bit < 0 || bit > 15) throw new ArgumentOutOfRangeException(nameof(bit));
            return (ushort)(i | uintBits[bit]);
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 1</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 31</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static uint WithBit(uint i, int bit) {
            if (bit < 0 || bit > 31) throw new ArgumentOutOfRangeException(nameof(bit));
            return i | uintBits[bit];
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 1</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 63</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static ulong WithBit(ulong i, int bit) {
            if (bit < 0 || bit > 63) throw new ArgumentOutOfRangeException(nameof(bit));
            return i | ulongBits[bit];
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 0</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 7</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static byte WithoutBit(byte i, int bit) {
            if (bit < 0 || bit > 7) throw new ArgumentOutOfRangeException(nameof(bit));
            return (byte)(i & ~uintBits[bit]);
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 0</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 15</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static ushort WithoutBit(ushort i, int bit) {
            if (bit < 0 || bit > 15) throw new ArgumentOutOfRangeException(nameof(bit));
            return (ushort)(i & ~uintBits[bit]);
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 0</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 31</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static uint WithoutBit(uint i, int bit) {
            if (bit < 0 || bit > 31) throw new ArgumentOutOfRangeException(nameof(bit));
            return i & ~uintBits[bit];
        }

        /// <summary>Returns <paramref name="i"/> with chosen bit set to 0</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 63</param>
        /// <returns><paramref name="i"/> with chosen bit set to 1</returns>
        public static ulong WithoutBit(ulong i, int bit) {
            if (bit < 0 || bit > 63) throw new ArgumentOutOfRangeException(nameof(bit));
            return i & ~ulongBits[bit];
        }

        /// <summary>Sets the chosen bit of <paramref name="i"/> to 1 if <paramref name="value"/> = true or to 0 otherwise</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 7</param>
        /// <param name="value">value for the bit</param>
        public static void SetBit(ref byte i, int bit, bool value) {
            if (bit < 0 || bit > 7) throw new ArgumentOutOfRangeException(nameof(bit));
            if (value)
                i = (byte)(i | uintBits[bit]);
            else i = (byte)(i & ~uintBits[bit]);
        }

        /// <summary>Sets the chosen bit of <paramref name="i"/> to 1 if <paramref name="value"/> = true or to 0 otherwise</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 15</param>
        /// <param name="value">value for the bit</param>
        public static void SetBit(ref ushort i, int bit, bool value) {
            if (bit < 0 || bit > 15) throw new ArgumentOutOfRangeException(nameof(bit));
            if (value)
                i = (ushort)(i | uintBits[bit]);
            else i = (ushort)(i & ~uintBits[bit]);
        }

        /// <summary>Sets the chosen bit of <paramref name="i"/> to 1 if <paramref name="value"/> = true or to 0 otherwise</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 31</param>
        /// <param name="value">value for the bit</param>
        public static void SetBit(ref uint i, int bit, bool value) {
            if (bit < 0 || bit > 31) throw new ArgumentOutOfRangeException(nameof(bit));
            if (value)
                i |= uintBits[bit];
            else i &= ~uintBits[bit];
        }

        /// <summary>Sets the chosen bit of <paramref name="i"/> to 1 if <paramref name="value"/> = true or to 0 otherwise</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 63</param>
        /// <param name="value">value for the bit</param>
        public static void SetBit(ref ulong i, int bit, bool value) {
            if (bit < 0 || bit > 63) throw new ArgumentOutOfRangeException(nameof(bit));
            if (value)
                i |= ulongBits[bit];
            else i &= ~ulongBits[bit];
        }
        
        /// <summary>Gets the value of the chosen bit</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 7</param>
        public static bool GetBit(byte i, int bit) {
            if (bit < 0 || bit > 7) throw new ArgumentOutOfRangeException(nameof(bit));
            return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Gets the value of the chosen bit</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 15</param>
        public static bool GetBit(ushort i, int bit) {
            if (bit < 0 || bit > 15) throw new ArgumentOutOfRangeException(nameof(bit));
            return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Gets the value of the chosen bit</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 31</param>
        public static bool GetBit(uint i, int bit) {
            if (bit < 0 || bit > 31) throw new ArgumentOutOfRangeException(nameof(bit));
            return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Gets the value of the chosen bit</summary>
        /// <param name="i">number to use</param>
        /// <param name="bit">chosen bit, between 0 and 63</param>
        public static bool GetBit(ulong i, int bit) {
            if (bit < 0 || bit > 63) throw new ArgumentOutOfRangeException(nameof(bit));
            return (i & ulongBits[bit]) != 0;
        }

        /// <summary>Enumerates bits of the given number</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBits(byte i) {
            for (var bit = 0; bit < 8; ++bit)
                yield return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Enumerates bits of the given number</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBits(ushort i) {
            for (var bit = 0; bit < 16; ++bit)
                yield return (i & uintBits[bit]) != 0;
        }

        /// <summary>Enumerates bits of the given number</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBits(uint i) {
            for (var bit = 0; bit < 32; ++bit)
                yield return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Enumerates bits of the given number</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBits(ulong i) {
            for (var bit = 0; bit < 64; ++bit)
                yield return (i & ulongBits[bit]) != 0;
        }

        /// <summary>Enumerates bits of the given number in reverse order</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBitsReverse(byte i) {
            for (var bit = 7; bit >= 0; --bit)
                yield return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Enumerates bits of the given number in reverse order</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBitsReverse(ushort i) {
            for (var bit = 15; bit >= 0; --bit)
                yield return (i & uintBits[bit]) != 0;
        }

        /// <summary>Enumerates bits of the given number in reverse order</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBitsReverse(uint i) {
            for (var bit = 31; bit >= 0; --bit)
                yield return (i & uintBits[bit]) != 0;
        }
        
        /// <summary>Enumerates bits of the given number in reverse order</summary>
        /// <param name="i">number to use</param>
        public IEnumerable<bool> EnumerateBitsReverse(ulong i) {
            for (var bit = 63; bit >= 0; --bit)
                yield return (i & ulongBits[bit]) != 0;
        }

        /// <summary>Stores bits from the given number in an array and returns the array</summary>
        /// <param name="i">number to use</param>
        /// <returns>array with 32 bits of the given number</returns>
        public bool[] GetBits(byte i) {
            var bits = new bool[8];
            for (var bit = 0; bit < 8; ++bit)
                bits[bit] = (i & uintBits[bit]) != 0;
            return bits;
        }
        
        /// <summary>Stores bits from the given number in an array and returns the array</summary>
        /// <param name="i">number to use</param>
        /// <returns>array with 64 bits of the given number</returns>
        public bool[] GetBits(ushort i) {
            var bits = new bool[16];
            for (var bit = 0; bit < 16; ++bit)
                bits[bit] = (i & uintBits[bit]) != 0;
            return bits;
        }

        /// <summary>Stores bits from the given number in an array and returns the array</summary>
        /// <param name="i">number to use</param>
        /// <returns>array with 32 bits of the given number</returns>
        public bool[] GetBits(uint i) {
            var bits = new bool[32];
            for (var bit = 0; bit < 32; ++bit)
                bits[bit] = (i & uintBits[bit]) != 0;
            return bits;
        }
        
        /// <summary>Stores bits from the given number in an array and returns the array</summary>
        /// <param name="i">number to use</param>
        /// <returns>array with 64 bits of the given number</returns>
        public bool[] GetBits(ulong i) {
            var bits = new bool[64];
            for (var bit = 0; bit < 64; ++bit)
                bits[bit] = (i & ulongBits[bit]) != 0;
            return bits;
        }
    }
}
