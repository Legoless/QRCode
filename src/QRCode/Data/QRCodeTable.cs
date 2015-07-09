using System;

namespace QRCode.Data
{
    public class QRCodeTable
    {
        private readonly char[] _table;

        public QRCodeTable (QRType type)
        {
            if (type == QRType.AlphaNumeric)
            {
                _table = new char[45];

                _table[0] = '0';
                _table[1] = '1';
                _table[2] = '2';
                _table[3] = '3';
                _table[4] = '4';
                _table[5] = '5';
                _table[6] = '6';
                _table[7] = '7';
                _table[8] = '8';
                _table[9] = '9';
                _table[10] = 'A';
                _table[11] = 'B';
                _table[12] = 'C';
                _table[13] = 'D';
                _table[14] = 'E';
                _table[15] = 'F';
                _table[16] = 'G';
                _table[17] = 'H';
                _table[18] = 'I';
                _table[19] = 'J';
                _table[20] = 'K';
                _table[21] = 'L';
                _table[22] = 'M';
                _table[23] = 'N';
                _table[24] = 'O';
                _table[25] = 'P';
                _table[26] = 'Q';
                _table[27] = 'R';
                _table[28] = 'S';
                _table[29] = 'T';
                _table[30] = 'U';
                _table[31] = 'V';
                _table[32] = 'W';
                _table[33] = 'X';
                _table[34] = 'Y';
                _table[35] = 'Z';
                _table[36] = ' ';
                _table[37] = '$';
                _table[38] = '%';
                _table[39] = '*';
                _table[40] = '+';
                _table[41] = '-';
                _table[42] = '.';
                _table[43] = '/';
                _table[44] = ':';
            }
        }

        public int GetCharCount ()
        {
            return _table.Length;
        }

        public char GetCharByCode (int code)
        {
            if (code > _table.Length)
            {
                throw new ArgumentException ("Invalid code argument, out of bounds.");
            }

            return _table[code];
        }

        public int GetCodeByChar (char chr)
        {
            for (int i = 0; i < _table.Length; i++)
            {
                if (chr == _table[i])
                {
                    return i;
                }
            }

            return -1;
        }
    }
}