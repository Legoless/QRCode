// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QrReedSolomon.cs" company="arvystate.net">
//   arvystate.net
// </copyright>
// <summary>
//   The <c>QR</c> reed solomon algorithm.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace QRCode.ErrorCorrection
{
    using System;
    using System.Collections.Generic;

    using NLog;

    using QRCode.Extensions;

    /// <summary>
    /// The <c>QR</c> reed solomon algorithm.
    /// </summary>
    public class QrReedSolomon
    {
        #region Static Fields

        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>
        /// The _field.
        /// </summary>
        private readonly QrGaloisField field;

        /// <summary>
        /// The _generator cache.
        /// </summary>
        private readonly List<QrGaloisFieldPoly> generatorCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="QrReedSolomon"/> class.
        /// </summary>
        public QrReedSolomon()
        {
            this.field = new QrGaloisField();

            // Add 0 poly, as a starting point, later we are only multiplying
            this.generatorCache = new List<QrGaloisFieldPoly> { new QrGaloisFieldPoly(this.field, new[] { 1 }) };
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The calculate method.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="numEcc">
        /// The number of <c>ecc</c> codes.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public byte[] Calculate(Poly message, int numEcc)
        {
            // Get message poly coefficients
            int[] messageCoefficients = new int[message.Terms.Length];

            for (int i = 0; i < message.Terms.Length; i++)
            {
                messageCoefficients[i] = message.Terms[i].Coefficient;
            }

            // Convert message poly coefficients into Galois Field (get a coefficients from numbers)
            QrGaloisFieldPoly info = new QrGaloisFieldPoly(this.field, messageCoefficients);

            // Increase level of the poly by number of ECC
            info = info.MultiplyByMonomial(numEcc, 1);

            // Get generator poly
            QrGaloisFieldPoly generatorPoly = this.BuildGeneratorPolynomial(numEcc);

            // Divite by generator poly
            QrGaloisFieldPoly remainder = info.Divide(generatorPoly)[1];

            // Return remainder coefficients in bytes
            int[] coefficients = remainder.GetCoefficients();

            byte[] eccCodes = new byte[coefficients.Length];

            for (int i = 0; i < coefficients.Length; i++)
            {
                eccCodes[i] = Convert.ToByte(coefficients[i]);
            }

            return eccCodes;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The build generator polynomial.
        /// </summary>
        /// <param name="power">
        /// The power.
        /// </param>
        /// <returns>
        /// The <see cref="QrGaloisFieldPoly"/>.
        /// </returns>
        private QrGaloisFieldPoly BuildGeneratorPolynomial(int power)
        {
            if (power >= this.generatorCache.Count)
            {
                QrGaloisFieldPoly lastGenerator = this.generatorCache[this.generatorCache.Count - 1];

                for (int i = this.generatorCache.Count; i <= power; i++)
                {
                    QrGaloisFieldPoly nextGenerator =
                        lastGenerator.Multiply(
                            new QrGaloisFieldPoly(this.field, new[] { 1, this.field.GetExponent(i - 1) }));
                    this.generatorCache.Add(nextGenerator);
                    lastGenerator = nextGenerator;
                }
            }

            return this.generatorCache[power];
        }

        #endregion
    }
}