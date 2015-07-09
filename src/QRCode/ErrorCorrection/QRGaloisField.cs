using System;
using QRCode.Data;

namespace QRCode.ErrorCorrection
{
    internal class QRGaloisField
    {
        private readonly int _primitive;
        private readonly int _size;

        private int[] _antiLogTable;
        private int[] _logTable;

        public QRGaloisField ()
        {
            _primitive = QRConst.QRCodeField;
            _size = QRConst.GfFieldCount;

            //
            // Generate log and antilog tables
            //

            Init ();
        }

        public QRGaloisFieldPoly Zero
        {
            get;
            set;
        }

        public QRGaloisFieldPoly One
        {
            get;
            set;
        }

        public int Size
        {
            get;
            set;
        }

        public QRGaloisFieldPoly BuildMonomial (int power, int coefficient)
        {
            if (coefficient == 0)
            {
                return Zero;
            }

            int[] coefficients = new int[power + 1];
            coefficients[0] = coefficient;

            return new QRGaloisFieldPoly (this, coefficients);
        }

        public static int AddOrSubtract (int a, int b)
        {
            return a ^ b;
        }

        public int GetExponent (int a)
        {
            return _antiLogTable[a];
        }

        public int GetLog (int a)
        {
            if (a == 0)
            {
                throw new ArgumentException ();
            }

            return _logTable[a];
        }

        public int Inverse (int a)
        {
            if (a == 0)
            {
                throw new ArithmeticException ();
            }

            return _antiLogTable[_size - _logTable[a] - 1];
        }

        public int Multiply (int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }

            if ((a < 0) || (b < 0) || (a >= _size) || (b >= _size))
            {
                a++;
            }

            int logSum = _logTable[a] + _logTable[b];
            return _antiLogTable[(logSum % _size) + logSum / _size];
        }

        private void Init ()
        {
            _antiLogTable = new int[_size];
            _logTable = new int[_size];

            int x = 1;

            for (int i = 0; i < _size; i++)
            {
                _antiLogTable[i] = x;

                x = x << 1;

                if (x >= _size)
                {
                    x ^= _primitive;
                    x &= _size - 1;
                }
            }
            for (int i = 0; i < _size - 1; i++)
            {
                _logTable[_antiLogTable[i]] = i;
            }

            //
            // LogTable[0] should never be used
            //

            Zero = new QRGaloisFieldPoly (this, new[] {0});
            One = new QRGaloisFieldPoly (this, new[] {1});
        }
    }
}