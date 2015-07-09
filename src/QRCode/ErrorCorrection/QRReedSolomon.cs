using System;
using System.Collections.Generic;
using QRCode.Extensions;

namespace QRCode.ErrorCorrection
{
    public class QRReedSolomon
    {
        private readonly QRGaloisField _field;
        private readonly List<QRGaloisFieldPoly> _generatorCache;

        public QRReedSolomon ()
        {
            _field = new QRGaloisField ();

            // Add 0 poly, as a starting point, later we are only multiplying
            _generatorCache = new List<QRGaloisFieldPoly>
                                  {
                                      new QRGaloisFieldPoly (_field, new[] {1})
                                  };
        }

        public byte[] Calculate (Poly message, int numEcc)
        {
            //
            // Get message poly coefficients
            //

            int[] messageCoefficients = new int[message.Terms.Length];

            for (int i = 0; i < message.Terms.Length; i++)
            {
                messageCoefficients[i] = message.Terms[i].Coefficient;
            }

            //
            // Convert message poly coefficients into Galois Field (get a coefficients from numbers)
            //

            QRGaloisFieldPoly info = new QRGaloisFieldPoly (_field, messageCoefficients);

            //
            // Increase level of the poly by number of ECC
            //

            info = info.MultiplyByMonomial (numEcc, 1);

            //
            // Get generator poly
            //

            QRGaloisFieldPoly generatorPoly = BuildGeneratorPolynomial (numEcc);

            //
            // Divite by generator poly
            //

            QRGaloisFieldPoly remainder = info.Divide (generatorPoly)[1];

            //
            // Return remainder coefficients in bytes
            //

            int[] coefficients = remainder.GetCoefficients ();

            byte[] eccCodes = new byte[coefficients.Length];

            for (int i = 0; i < coefficients.Length; i++)
            {
                eccCodes[i] = Convert.ToByte (coefficients[i]);
            }

            return eccCodes;
        }

        private QRGaloisFieldPoly BuildGeneratorPolynomial (int power)
        {
            if (power >= _generatorCache.Count)
            {
                QRGaloisFieldPoly lastGenerator = _generatorCache[(_generatorCache.Count - 1)];

                for (int i = _generatorCache.Count; i <= power; i++)
                {
                    QRGaloisFieldPoly nextGenerator = lastGenerator.Multiply (new QRGaloisFieldPoly (_field, new[] {1, _field.GetExponent (i - 1)}));
                    _generatorCache.Add (nextGenerator);
                    lastGenerator = nextGenerator;
                }
            }

            return _generatorCache[power];
        }
    }
}