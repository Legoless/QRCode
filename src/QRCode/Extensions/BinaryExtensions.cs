// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExtensions.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   Extensions to work with bits (boolean) types
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Extensions to work with bits (boolean) types.
    /// </summary>
    internal static class BinaryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Value is converted to binary and appended to current bit set.
        /// </summary>
        /// <param name="bits">
        /// Bit set to append to
        /// </param>
        /// <param name="value">
        /// Value to be converted
        /// </param>
        /// <param name="num">
        /// Number of bits to append
        /// </param>
        /// <returns>
        /// The <see cref="BitArray"/>.
        /// </returns>
        public static BitArray AppendBits(BitArray bits, int value, int num)
        {
            List<bool> bitList = new List<bool>(BitArrayToBoolArray(bits));

            bitList.AddRange(DecToBin(value, num));

            return new BitArray(bitList.ToArray());
        }

        /// <summary>
        /// The array to string.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ArrayToString(Array array)
        {
            StringBuilder text = new StringBuilder();

            for (int i = 0; i < array.GetLength(0); i++)
            {
                if (i != array.GetLength(0) - 1)
                {
                    text.Append(array.GetValue(i) + ", ");
                }
                else
                {
                    text.Append(array.GetValue(i));
                }
            }

            return text.ToString();
        }

        /// <summary>
        /// Converts list of booleans to decimal number.
        /// </summary>
        /// <param name="bin">
        /// List of booleans.
        /// </param>
        /// <returns>
        /// Decimal number.
        /// </returns>
        public static int BinToDec(List<bool> bin)
        {
            return BinToDec(bin.ToArray());
        }

        /// <summary>
        /// Converts queue of booleans to decimal number.
        /// </summary>
        /// <param name="bin">
        /// Queue of booleans.
        /// </param>
        /// <returns>
        /// Decimal number.
        /// </returns>
        public static int BinToDec(Queue<bool> bin)
        {
            return BinToDec(bin.ToArray());
        }

        /// <summary>
        /// Converts array of booleans to number
        /// </summary>
        /// <param name="bin">
        /// Array of <c>bools</c>
        /// </param>
        /// <returns>
        /// Decimal number
        /// </returns>
        public static int BinToDec(bool[] bin)
        {
            int dec = 0;

            for (int i = 0; i < bin.Length; i++)
            {
                if (bin[i])
                {
                    dec += (int)Math.Pow(2, bin.Length - i - 1);
                }
            }

            return dec;
        }

        /// <summary>
        /// Returns string from list of bits, used to debugging.
        /// </summary>
        /// <param name="bits">
        /// Queue of bits.
        /// </param>
        /// <returns>
        /// Binary string.
        /// </returns>
        public static string BinaryToString(Queue<bool> bits)
        {
            return BinaryToString(bits.ToArray());
        }

        /// <summary>
        /// Returns string from list of bits, used to debugging.
        /// </summary>
        /// <param name="bits">
        /// List of bits.
        /// </param>
        /// <returns>
        /// Binary string.
        /// </returns>
        public static string BinaryToString(List<bool> bits)
        {
            return BinaryToString(bits.ToArray());
        }

        /// <summary>
        /// Returns string from array of bits
        /// </summary>
        /// <param name="bits">
        /// bit array
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string BinaryToString(bool[] bits)
        {
            StringBuilder s = new StringBuilder();

            foreach (bool t in bits)
            {
                s.Append(t ? "1" : "0");
            }

            return s.ToString();
        }

        /// <summary>
        /// Converts BitArray object into <c>bool</c> array.
        /// </summary>
        /// <param name="bits">
        /// BitArray array
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool[] BitArrayToBoolArray(BitArray bits)
        {
            bool[] bitArray = new bool[bits.Count];

            for (int i = 0; i < bits.Count; i++)
            {
                bitArray[i] = bits[i];
            }

            return bitArray;
        }

        /// <summary>
        /// Converts BitArray to ByteArray.
        /// </summary>
        /// <param name="bits">
        /// BitArray collection.
        /// </param>
        /// <returns>
        /// Byte array.
        /// </returns>
        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;

            if (bits.Count % 8 != 0)
            {
                numBytes++;
            }

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                {
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));
                }

                bitIndex++;

                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }

        /// <summary>
        /// Converts decimal number to list of bits.
        /// </summary>
        /// <param name="dec">
        /// Decimal number.
        /// </param>
        /// <param name="length">
        /// Length of bits.
        /// </param>
        /// <returns>
        /// List collection of booleans.
        /// </returns>
        public static List<bool> DecToBin(int dec, int length)
        {
            List<bool> con = DecToBin(dec);

            while (con.Count < length)
            {
                con.Insert(0, false);
            }

            return con;
        }

        /// <summary>
        /// Converts decimal number to <c>bool</c> list.
        /// </summary>
        /// <param name="dec">
        /// Number to convert.
        /// </param>
        /// <returns>
        /// List of <c>bools</c>.
        /// </returns>
        public static List<bool> DecToBin(int dec)
        {
            List<bool> con = new List<bool>();

            while (dec > 0)
            {
                int bit = dec % 2;
                con.Add(bit == 1);
                dec = dec / 2;
            }

            con.Reverse();

            return con;
        }

        #endregion
    }
}