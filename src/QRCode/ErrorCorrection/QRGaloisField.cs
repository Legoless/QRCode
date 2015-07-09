// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrGaloisField.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The galois field.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.ErrorCorrection
{
    using System;

    using NLog;

    using QRCode.Data;

    /// <summary>
    /// The galois field.
    /// </summary>
    internal class QrGaloisField
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The _primitive.
        /// </summary>
        private readonly int primitive;

        /// <summary>
        /// The _size.
        /// </summary>
        private readonly int size;

        /// <summary>
        /// The _anti log table.
        /// </summary>
        private int[] antiLogTable;

        /// <summary>
        /// The _log table.
        /// </summary>
        private int[] logTable;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrGaloisField"/> class.
        /// </summary>
        public QrGaloisField()
        {
            this.primitive = QrConst.QrCodeField;
            this.size = QrConst.GfFieldCount;

            // Generate log and antilog tables
            this.Init();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the one.
        /// </summary>
        public QrGaloisFieldPoly One { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the zero.
        /// </summary>
        public QrGaloisFieldPoly Zero { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add or subtract.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int AddOrSubtract(int a, int b)
        {
            return a ^ b;
        }

        /// <summary>
        /// The build monomial.
        /// </summary>
        /// <param name="power">
        /// The power.
        /// </param>
        /// <param name="coefficient">
        /// The coefficient.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly"/>.
        /// </returns>
        public QrGaloisFieldPoly BuildMonomial(int power, int coefficient)
        {
            if (coefficient == 0)
            {
                return this.Zero;
            }

            int[] coefficients = new int[power + 1];
            coefficients[0] = coefficient;

            return new QrGaloisFieldPoly(this, coefficients);
        }

        /// <summary>
        /// The get exponent.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetExponent(int a)
        {
            return this.antiLogTable[a];
        }

        /// <summary>
        /// The get log.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Exception if A is zero.
        /// </exception>
        public int GetLog(int a)
        {
            if (a == 0)
            {
                throw new ArgumentException();
            }

            return this.logTable[a];
        }

        /// <summary>
        /// The inverse.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// Exception if A is zero.
        /// </exception>
        public int Inverse(int a)
        {
            if (a == 0)
            {
                throw new ArithmeticException();
            }

            return this.antiLogTable[this.size - this.logTable[a] - 1];
        }

        /// <summary>
        /// The multiply.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Multiply(int a, int b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }

            if ((a < 0) || (b < 0) || (a >= this.size) || (b >= this.size))
            {
                a++;
            }

            int logSum = this.logTable[a] + this.logTable[b];
            return this.antiLogTable[(logSum % this.size) + (logSum / this.size)];
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialization method.
        /// </summary>
        private void Init()
        {
            this.antiLogTable = new int[this.size];
            this.logTable = new int[this.size];

            int x = 1;

            for (int i = 0; i < this.size; i++)
            {
                this.antiLogTable[i] = x;

                x = x << 1;

                if (x >= this.size)
                {
                    x ^= this.primitive;
                    x &= this.size - 1;
                }
            }

            for (int i = 0; i < this.size - 1; i++)
            {
                this.logTable[this.antiLogTable[i]] = i;
            }

            // LogTable[0] should never be used
            this.Zero = new QrGaloisFieldPoly(this, new[] { 0 });
            this.One = new QrGaloisFieldPoly(this, new[] { 1 });
        }

        #endregion
    }
}