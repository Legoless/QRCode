using System;
using System.Text;

namespace QRCode.ErrorCorrection
{
    internal class QRGaloisFieldPoly
    {
        private readonly int[] _coefficients;
        private readonly QRGaloisField _field;

        public QRGaloisFieldPoly (QRGaloisField field, int[] coefficients)
        {
            if (coefficients.Length == 0)
            {
                throw new ArgumentException ();
            }

            _field = field;

            int coefficientsLength = coefficients.Length;

            if (coefficientsLength > 1 && coefficients[0] == 0)
            {
                // Leading term must be non-zero for anything except the constant polynomial "0"
                int firstNonZero = 1;

                while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0)
                {
                    firstNonZero++;
                }

                if (firstNonZero == coefficientsLength)
                {
                    _coefficients = field.Zero._coefficients;
                }
                else
                {
                    _coefficients = new int[coefficientsLength - firstNonZero];

                    Array.Copy (coefficients, firstNonZero, _coefficients, 0, _coefficients.Length);
                }
            }
            else
            {
                _coefficients = coefficients;
            }
        }

        public int[] GetCoefficients ()
        {
            return _coefficients;
        }

        public int GetPower ()
        {
            return _coefficients.Length - 1;
        }

        public bool IsZero ()
        {
            return _coefficients[0] == 0;
        }

        public int GetCoefficient (int power)
        {
            return _coefficients[_coefficients.Length - 1 - power];
        }

        public int EvaluateAt (int a)
        {
            // Just return the x^0 coefficient
            if (a == 0)
            {
                return GetCoefficient (0);
            }

            int size = _coefficients.Length;

            if (a == 1)
            {
                // Just the sum of the coefficients
                int txt = 0;

                for (int i = 0; i < size; i++)
                {
                    txt = QRGaloisField.AddOrSubtract (txt, _coefficients[i]);
                }

                return txt;
            }

            int res = _coefficients[0];

            for (int i = 1; i < size; i++)
            {
                res = QRGaloisField.AddOrSubtract (_field.Multiply (a, res), _coefficients[i]);
            }

            return res;
        }

        public QRGaloisFieldPoly AddOrSubtract (QRGaloisFieldPoly other)
        {
            if (IsZero ())
            {
                return other;
            }

            if (other.IsZero ())
            {
                return this;
            }

            int[] smallerCoefficients = _coefficients;
            int[] largerCoefficients = other._coefficients;

            if (smallerCoefficients.Length > largerCoefficients.Length)
            {
                int[] temp = smallerCoefficients;
                smallerCoefficients = largerCoefficients;
                largerCoefficients = temp;
            }

            int[] sumDiff = new int[largerCoefficients.Length];
            int lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;

            // Copy high-order terms only found in higher-power polynomial's coefficients

            Array.Copy (largerCoefficients, 0, sumDiff, 0, lengthDiff);

            for (int i = lengthDiff; i < largerCoefficients.Length; i++)
            {
                sumDiff[i] = QRGaloisField.AddOrSubtract (smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
            }

            return new QRGaloisFieldPoly (_field, sumDiff);
        }

        public QRGaloisFieldPoly Multiply (QRGaloisFieldPoly other)
        {
            if (IsZero () || other.IsZero ())
            {
                return _field.Zero;
            }

            int[] aCoefficients = _coefficients;
            int aLength = aCoefficients.Length;

            int[] bCoefficients = other._coefficients;

            int bLength = bCoefficients.Length;
            int[] product = new int[aLength + bLength - 1];

            for (int i = 0; i < aLength; i++)
            {
                int aCoeff = aCoefficients[i];

                for (int j = 0; j < bLength; j++)
                {
                    product[i + j] = QRGaloisField.AddOrSubtract (product[i + j], _field.Multiply (aCoeff, bCoefficients[j]));
                }
            }

            return new QRGaloisFieldPoly (_field, product);
        }

        public QRGaloisFieldPoly Multiply (int scalar)
        {
            if (scalar == 0)
            {
                return _field.Zero;
            }

            if (scalar == 1)
            {
                return this;
            }

            int size = _coefficients.Length;
            int[] product = new int[size];

            for (int i = 0; i < size; i++)
            {
                product[i] = _field.Multiply (_coefficients[i], scalar);
            }

            return new QRGaloisFieldPoly (_field, product);
        }

        public QRGaloisFieldPoly MultiplyByMonomial (int power, int coefficient)
        {
            if (power < 0)
            {
                throw new ArgumentException ();
            }

            if (coefficient == 0)
            {
                return _field.Zero;
            }

            int size = _coefficients.Length;
            int[] product = new int[size + power];

            for (int i = 0; i < size; i++)
            {
                product[i] = _field.Multiply (_coefficients[i], coefficient);
            }

            return new QRGaloisFieldPoly (_field, product);
        }

        public QRGaloisFieldPoly[] Divide (QRGaloisFieldPoly other)
        {
            if (other.IsZero ())
            {
                throw new ArgumentException ("Divide by 0");
            }

            QRGaloisFieldPoly quotient = _field.Zero;
            QRGaloisFieldPoly remainder = this;

            int denominatorLeadingTerm = other.GetCoefficient (other.GetPower ());
            int inverseDenominatorLeadingTerm = _field.Inverse (denominatorLeadingTerm);

            while ((remainder.GetPower () >= other.GetPower ()) && !remainder.IsZero ())
            {
                int powerDifference = remainder.GetPower () - other.GetPower ();
                int scale = _field.Multiply (remainder.GetCoefficient (remainder.GetPower ()), inverseDenominatorLeadingTerm);

                QRGaloisFieldPoly term = other.MultiplyByMonomial (powerDifference, scale);
                QRGaloisFieldPoly iterationQuotient = _field.BuildMonomial (powerDifference, scale);

                quotient = quotient.AddOrSubtract (iterationQuotient);
                remainder = remainder.AddOrSubtract (term);
            }

            return new[]
                       {
                           quotient,
                           remainder
                       };
        }

        public override string ToString ()
        {
            StringBuilder txt = new StringBuilder (8 * GetPower ());

            for (int power = GetPower (); power >= 0; power--)
            {
                int coefficient = GetCoefficient (power);

                if (coefficient != 0)
                {
                    if (coefficient < 0)
                    {
                        txt.Append (" - ");
                        coefficient = -coefficient;
                    }
                    else
                    {
                        if (txt.Length > 0)
                        {
                            txt.Append (" + ");
                        }
                    }
                    if ((power == 0) || (coefficient != 1))
                    {
                        int alphaPower = _field.GetLog (coefficient);

                        if (alphaPower == 0)
                        {
                            txt.Append ('1');
                        }
                        else if (alphaPower == 1)
                        {
                            txt.Append ('a');
                        }
                        else
                        {
                            txt.Append ("a^");
                            txt.Append (alphaPower);
                        }
                    }

                    if (power != 0)
                    {
                        if (power == 1)
                        {
                            txt.Append ('x');
                        }
                        else
                        {
                            txt.Append ("x^");
                            txt.Append (power);
                        }
                    }
                }
            }

            return txt.ToString ();
        }
    }
}