// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrCodeTable.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> code table.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.Data
{
    using System;

    using NLog;

    /// <summary>
    /// The <c>QR</c> code table.
    /// </summary>
    public class QrCodeTable
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The table.
        /// </summary>
        private readonly char[] table;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrCodeTable"/> class.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        public QrCodeTable(QrType type)
        {
            if (type == QrType.AlphaNumeric)
            {
                this.table = new char[45];

                this.table[0] = '0';
                this.table[1] = '1';
                this.table[2] = '2';
                this.table[3] = '3';
                this.table[4] = '4';
                this.table[5] = '5';
                this.table[6] = '6';
                this.table[7] = '7';
                this.table[8] = '8';
                this.table[9] = '9';
                this.table[10] = 'A';
                this.table[11] = 'B';
                this.table[12] = 'C';
                this.table[13] = 'D';
                this.table[14] = 'E';
                this.table[15] = 'F';
                this.table[16] = 'G';
                this.table[17] = 'H';
                this.table[18] = 'I';
                this.table[19] = 'J';
                this.table[20] = 'K';
                this.table[21] = 'L';
                this.table[22] = 'M';
                this.table[23] = 'N';
                this.table[24] = 'O';
                this.table[25] = 'P';
                this.table[26] = 'Q';
                this.table[27] = 'R';
                this.table[28] = 'S';
                this.table[29] = 'T';
                this.table[30] = 'U';
                this.table[31] = 'V';
                this.table[32] = 'W';
                this.table[33] = 'X';
                this.table[34] = 'Y';
                this.table[35] = 'Z';
                this.table[36] = ' ';
                this.table[37] = '$';
                this.table[38] = '%';
                this.table[39] = '*';
                this.table[40] = '+';
                this.table[41] = '-';
                this.table[42] = '.';
                this.table[43] = '/';
                this.table[44] = ':';
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get char by code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="char"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If code is out of bounds, argument exception is thrown.
        /// </exception>
        public char GetCharByCode(int code)
        {
            if (code > this.table.Length)
            {
                throw new ArgumentException("Invalid code argument, out of bounds.");
            }

            return this.table[code];
        }

        /// <summary>
        /// The get char count.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetCharCount()
        {
            return this.table.Length;
        }

        /// <summary>
        /// The get code by char.
        /// </summary>
        /// <param name="chr">
        /// The character.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetCodeByChar(char chr)
        {
            for (int i = 0; i < this.table.Length; i++)
            {
                if (chr == this.table[i])
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}