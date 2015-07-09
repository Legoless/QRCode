using System;
using System.Collections;
using System.Collections.Generic;

namespace QRCode.Extensions
{
    /// <summary>
    /// Extensions to work with bits (boolean) types
    /// </summary>
    internal static class BinaryExtensions
    {
        /// <summary>
        /// Converts BitArray to ByteArray
        /// </summary>
        /// <param name="bits">BitArray collection</param>
        /// <returns>byte array</returns>
        public static byte[] BitArrayToByteArray (BitArray bits)
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
                    bytes[byteIndex] |= (byte) (1 << (7 - bitIndex));
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
        /// Converts decimal number to list of bits
        /// </summary>
        /// <param name="dec">number</param>
        /// <param name="length">length of bits</param>
        /// <returns>list collection of booleans</returns>
        public static List<bool> DecToBin (int dec, int length)
        {
            List<bool> con = DecToBin (dec);

            while (con.Count < length)
            {
                con.Insert (0, false);
            }

            return con;
        }

        /// <summary>
        /// Converts decimal number to bool list
        /// </summary>
        /// <param name="dec">number to convert</param>
        /// <returns>list of bools</returns>
        public static List<bool> DecToBin (int dec)
        {
            List<bool> con = new List<bool> ();

            while (dec > 0)
            {
                int bit = dec % 2;
                con.Add ((bit == 1));
                dec = dec / 2;
            }

            con.Reverse ();

            return con;
        }

        /// <summary>
        /// Converts list of booleans to decimal number
        /// </summary>
        /// <param name="bin">list of booleans</param>
        /// <returns>number</returns>
        public static int BinToDec (List<bool> bin)
        {
            return BinToDec (bin.ToArray ());
        }

        /// <summary>
        /// Converts queue of booleans to decimal number
        /// </summary>
        /// <param name="bin">list of booleans</param>
        /// <returns>number</returns>
        public static int BinToDec (Queue<bool> bin)
        {
            return BinToDec (bin.ToArray ());
        }

        /// <summary>
        /// Converts array of booleans to number
        /// </summary>
        /// <param name="bin">array of bools</param>
        /// <returns>number</returns>
        public static int BinToDec (bool[] bin)
        {
            int dec = 0;

            for (int i = 0; i < bin.Length; i++)
            {
                if (bin[i])
                {
                    dec += (int) Math.Pow (2, bin.Length - i - 1);
                }
            }

            return dec;
        }

        /// <summary>
        /// Value is converted to binary and appended to current bitset.
        /// </summary>
        /// <param name="bits">Bitset</param>
        /// <param name="value">value</param>
        /// <param name="num">number of bits</param>
        /// <returns></returns>
        public static BitArray AppendBits (BitArray bits, int value, int num)
        {
            List<bool> bitList = new List<bool> (BitArrayToBoolArray (bits));

            bitList.AddRange (DecToBin (value, num));

            return new BitArray (bitList.ToArray ());
        }

        /// <summary>
        /// Returns string from queue of bits
        /// </summary>
        /// <param name="bits">queue of bits</param>
        /// <returns>string</returns>
        public static string BinaryToString (Queue<bool> bits)
        {
            return BinaryToString (bits.ToArray ());
        }

        /// <summary>
        /// Returns string from list of bits
        /// </summary>
        /// <param name="bits">list of bits</param>
        /// <returns>string</returns>
        public static string BinaryToString (List<bool> bits)
        {
            return BinaryToString (bits.ToArray ());
        }

        /// <summary>
        /// Returns string from array of bits
        /// </summary>
        /// <param name="bits">bit array</param>
        /// <returns></returns>
        public static string BinaryToString (bool[] bits)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder ();

            foreach (bool t in bits)
            {
                s.Append (t ? "1" : "0");
            }

            return s.ToString ();
        }

        /// <summary>
        /// Converst BitArray object into bool array
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static bool[] BitArrayToBoolArray (BitArray bits)
        {
            bool[] bitArray = new bool[bits.Count];

            for (int i = 0; i < bits.Count; i++)
            {
                bitArray[i] = bits[i];
            }

            return bitArray;
        }

        public static string ArrayToString (Array arr)
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder ();

            for (int i = 0; i < arr.GetLength (0); i++)
            {
                if (i != arr.GetLength (0) - 1)
                {
                    text.Append (arr.GetValue (i) + ", ");
                }
                else
                {
                    text.Append (arr.GetValue (i).ToString ());
                }
            }

            return text.ToString ();
        }
    }
}